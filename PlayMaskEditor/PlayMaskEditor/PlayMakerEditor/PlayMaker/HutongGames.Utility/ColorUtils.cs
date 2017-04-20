using System;
using UnityEngine;
namespace HutongGames.Utility
{
	public static class ColorUtils
	{
		public static bool Approximately(Color color1, Color color2)
		{
			return Mathf.Approximately(color1.r, color2.r) && Mathf.Approximately(color1.g, color2.g) && Mathf.Approximately(color1.b, color2.b) && Mathf.Approximately(color1.a, color2.a);
		}
		public static Color FromIntRGBA(int r, int g, int b, int a)
		{
			return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f, (float)a / 255f);
		}
	}
}
