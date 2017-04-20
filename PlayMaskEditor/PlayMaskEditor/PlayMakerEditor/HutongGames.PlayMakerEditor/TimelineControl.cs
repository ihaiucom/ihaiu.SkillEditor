using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class TimelineControl
	{
		public delegate void TimelineClickedHandler(float time);
		private const float minLabelSpacing = 50f;
		public TimelineControl.TimelineClickedHandler TimelineClicked;
		private float length;
		private float offset;
		private Rect area;
		private float timeScale = 200f;
		private float majorTickUnits;
		private float minorTickUnits;
		private float toolbarHeight;
		private Rect toolbarRect = default(Rect);
		private Rect graphRect = default(Rect);
		private Rect tickLabelRect = new Rect(0f, 0f, 100f, 20f);
		private Rect debugLineRect = default(Rect);
		private Color labelColor;
		private readonly float[] tickUnits = new float[]
		{
			0.01f,
			0.05f,
			0.1f,
			0.5f,
			1f,
			5f,
			10f,
			30f,
			60f,
			300f,
			600f,
			1800f,
			3600f
		};
		private readonly LineDrawer line = new LineDrawer();
		public bool NeedsRepaint
		{
			get;
			private set;
		}
		public float Length
		{
			get
			{
				return this.length;
			}
			set
			{
				this.length = Mathf.Clamp(value, 0f, float.PositiveInfinity);
			}
		}
		public float CanvasWidth
		{
			get
			{
				return Mathf.Max(this.length * this.timeScale, this.offset * this.timeScale + this.area.get_width());
			}
		}
		public float CanvasOffset
		{
			get
			{
				return this.offset * this.timeScale;
			}
			set
			{
				this.offset = value / this.timeScale;
			}
		}
		public float VisibleRangeStart
		{
			get
			{
				return this.Offset;
			}
		}
		public float VisibleRangeEnd
		{
			get
			{
				return this.Offset + this.area.get_width() / this.timeScale;
			}
		}
		public float VisibleTimeSpan
		{
			get
			{
				return this.area.get_width() / this.timeScale;
			}
		}
		public float Offset
		{
			get
			{
				return this.offset;
			}
			set
			{
				this.offset = Mathf.Clamp(value, 0f, this.Length);
			}
		}
		public float TimeScale
		{
			get
			{
				return this.timeScale;
			}
			set
			{
				this.timeScale = Mathf.Clamp(value, 1f, 100000f);
			}
		}
		public float TimeToScreenPosition(float time)
		{
			return (time - this.offset) * this.timeScale;
		}
		public float ScreenPositionToTime(float screenPos)
		{
			return (screenPos + this.offset * this.timeScale) / this.timeScale;
		}
		public void Move(float deltaX)
		{
			this.Offset += deltaX / this.timeScale;
			this.NeedsRepaint = true;
		}
		public void SetLength(float time)
		{
			if (this.length <= this.VisibleRangeEnd && time > this.VisibleRangeEnd - 20f / this.timeScale)
			{
				this.Offset = time - this.area.get_width() / this.timeScale + this.VisibleTimeSpan * 0.5f;
				this.NeedsRepaint = true;
			}
			this.length = time;
		}
		public void Zoom(float center, float delta)
		{
			float num = this.ScreenPositionToTime(center);
			this.TimeScale += delta / this.majorTickUnits;
			this.Offset = num - center / this.timeScale;
			this.NeedsRepaint = true;
		}
		public void FrameTime(float time, float padding = 0f)
		{
			if (time < this.VisibleRangeStart)
			{
				this.Offset = time - padding / this.timeScale;
			}
			if (time > this.VisibleRangeEnd)
			{
				this.Offset = time + padding / this.timeScale - this.VisibleTimeSpan;
			}
		}
		public void OnGUI(Rect area)
		{
			this.area = area;
			Event current = Event.get_current();
			EventType type = Event.get_current().get_type();
			this.toolbarHeight = EditorStyles.get_toolbar().get_fixedHeight();
			this.UpdateTicks();
			this.labelColor = EditorStyles.get_label().get_normal().get_textColor();
			this.line.SetColor(this.labelColor);
			this.toolbarRect.Set(area.get_x(), area.get_y(), area.get_width(), this.toolbarHeight);
			GUI.Box(this.toolbarRect, GUIContent.none, EditorStyles.get_toolbar());
			GUI.BeginGroup(area);
			if (type == 7)
			{
				this.graphRect.Set(0f, this.toolbarHeight, area.get_width(), area.get_height() - this.toolbarHeight);
				SkillEditorStyles.DarkPreviewBg.Draw(this.graphRect, false, false, false, false);
				this.DrawTicks(area);
			}
			if (type == 6 && current.get_mousePosition().x > 0f)
			{
				this.Zoom(current.get_mousePosition().x, -current.get_delta().y);
				current.Use();
			}
			if (type == 3 && (current.get_button() == 2 || Keyboard.AltAction()))
			{
				this.Move(-current.get_delta().x);
				current.Use();
			}
			GUI.EndGroup();
			if (area.Contains(current.get_mousePosition()) && type == null && current.get_mousePosition().y < this.toolbarHeight && this.TimelineClicked != null)
			{
				this.TimelineClicked(this.ScreenPositionToTime(current.get_mousePosition().x - area.get_x()));
			}
		}
		private void DrawTicks(Rect area)
		{
			float num = this.majorTickUnits * this.timeScale;
			float num2 = this.majorTickUnits * Mathf.Floor(this.offset / this.majorTickUnits);
			float num3 = this.TimeToScreenPosition(num2);
			float num4 = this.offset * this.timeScale + area.get_width();
			while (num3 < num4)
			{
				this.tickLabelRect.set_x(num3);
				GUI.Label(this.tickLabelRect, this.FormatTime(num2), EditorStyles.get_miniLabel());
				this.line.DrawVerticalLine(0.75f, num3, 7f, this.toolbarHeight);
				this.line.DrawVerticalLine(0.25f, num3, this.toolbarHeight, area.get_height());
				if (this.minorTickUnits > 0f)
				{
					float num5 = this.majorTickUnits / this.minorTickUnits;
					float num6 = num / num5;
					float num7 = num3 + num6;
					int num8 = 0;
					while ((float)num8 < num5 - 1f)
					{
						this.line.DrawVerticalLine(0.5f, num7, 14f, this.toolbarHeight);
						this.line.DrawVerticalLine(0.15f, num7, this.toolbarHeight, area.get_height());
						num7 += num6;
						num8++;
					}
				}
				num3 += num;
				num2 += this.majorTickUnits;
			}
		}
		[Localizable(false)]
		private string FormatTime(float time)
		{
			return string.Format("{0:0.00}", time).Replace('.', ':');
		}
		private void UpdateTicks()
		{
			this.UpdateTickUnits();
		}
		private void UpdateTickUnits()
		{
			this.majorTickUnits = this.tickUnits[this.tickUnits.Length - 1];
			this.minorTickUnits = 0f;
			for (int i = 0; i < this.tickUnits.Length; i++)
			{
				float num = this.tickUnits[i];
				if (num * this.timeScale > 50f)
				{
					this.majorTickUnits = num;
					if (i > 0)
					{
						this.minorTickUnits = this.tickUnits[i - 1];
					}
					return;
				}
			}
		}
		public void DrawDebugLine(float time)
		{
			float num = this.TimeToScreenPosition(time);
			if (num < 0f)
			{
				return;
			}
			num += this.area.get_x() - 3f;
			this.debugLineRect.Set(num, 0f, num, this.area.get_height() - 15f);
			GUI.set_color(Color.get_white());
			GUI.Box(this.debugLineRect, GUIContent.none, SkillEditorStyles.TimelineDebugLine);
		}
	}
}
