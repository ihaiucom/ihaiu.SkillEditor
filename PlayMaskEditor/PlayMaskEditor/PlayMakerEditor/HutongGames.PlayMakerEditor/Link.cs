using HutongGames.PlayMaker;
using System;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class Link
	{
		public SkillState FromState
		{
			get;
			set;
		}
		public int TransitionIndex
		{
			get;
			set;
		}
		public static void DrawArrowHead(Texture leftArrow, Vector2 pos, Color color, bool flipTexture, float scale)
		{
			Color color2 = GUI.get_color();
			GUI.set_color(color);
			if (!flipTexture)
			{
				GUI.DrawTexture(new Rect(pos.x, pos.y - (float)leftArrow.get_height() * 0.5f, (float)leftArrow.get_width() * scale, (float)leftArrow.get_height()), leftArrow);
			}
			else
			{
				Matrix4x4 matrix = GUI.get_matrix();
				GUIUtility.ScaleAroundPivot(new Vector2(-1f, 1f), pos);
				GUI.DrawTexture(new Rect(pos.x, pos.y - (float)leftArrow.get_height() * 0.5f, (float)leftArrow.get_width() * scale, (float)leftArrow.get_height()), leftArrow);
				GUI.set_matrix(matrix);
			}
			GUI.set_color(color2);
		}
	}
}
