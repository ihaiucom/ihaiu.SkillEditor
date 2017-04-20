using System;
using UnityEngine;
namespace HutongGames.Editor
{
	public class PopupWindow
	{
		private int id;
		public Rect Position
		{
			get;
			set;
		}
		public bool UseGUILayout
		{
			get;
			set;
		}
		public GUIContent Title
		{
			get;
			set;
		}
		public GUIStyle Style
		{
			get;
			set;
		}
		public bool CanDrag
		{
			get;
			set;
		}
		public PopupWindow(int id)
		{
			this.id = id;
			this.Title = GUIContent.none;
		}
		public void DoGUI()
		{
			if (this.UseGUILayout)
			{
				this.Position = ((this.Style != null) ? GUILayout.Window(this.id, this.Position, new GUI.WindowFunction(this.DoWindow), this.Title, this.Style, new GUILayoutOption[0]) : GUILayout.Window(this.id, this.Position, new GUI.WindowFunction(this.DoWindow), this.Title, new GUILayoutOption[0]));
				return;
			}
			this.Position = ((this.Style != null) ? GUI.Window(this.id, this.Position, new GUI.WindowFunction(this.DoWindow), this.Title, this.Style) : GUI.Window(this.id, this.Position, new GUI.WindowFunction(this.DoWindow), this.Title));
		}
		public virtual void DoWindow(int id)
		{
			if (this.CanDrag)
			{
				GUI.DragWindow();
			}
		}
	}
}
