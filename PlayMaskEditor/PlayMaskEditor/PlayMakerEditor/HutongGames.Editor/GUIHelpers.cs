using System;
using UnityEngine;
namespace HutongGames.Editor
{
	public static class GUIHelpers
	{
		public static void SafeExitGUI()
		{
			if (Event.get_current() != null)
			{
				GUIUtility.ExitGUI();
			}
		}
		public static void DrawTexture(Rect position, Texture texture, Color tint, ScaleMode scaleMode = 0)
		{
			Color color = GUI.get_color();
			GUI.set_color(tint);
			GUI.DrawTexture(position, texture, scaleMode);
			GUI.set_color(color);
		}
	}
}
