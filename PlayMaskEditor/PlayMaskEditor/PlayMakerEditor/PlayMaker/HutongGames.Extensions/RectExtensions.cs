using System;
using UnityEngine;
namespace HutongGames.Extensions
{
	public static class RectExtensions
	{
		public static bool Contains(this Rect rect, float x, float y)
		{
			return x > rect.get_xMin() && x < rect.get_xMax() && y > rect.get_yMin() && y < rect.get_yMax();
		}
		public static bool Contains(this Rect rect1, Rect rect2)
		{
			return rect1.get_xMin() <= rect2.get_xMin() && rect1.get_yMin() <= rect2.get_yMin() && rect1.get_xMax() >= rect2.get_xMax() && rect1.get_yMax() >= rect2.get_yMax();
		}
		public static bool IntersectsWith(this Rect rect1, Rect rect2)
		{
			return rect2.get_xMin() <= rect1.get_xMax() && rect2.get_xMax() >= rect1.get_xMin() && rect2.get_yMin() <= rect1.get_yMax() && rect2.get_yMax() >= rect1.get_yMin();
		}
		public static Rect Union(this Rect rect1, Rect rect2)
		{
			return Rect.MinMaxRect(Mathf.Min(rect1.get_xMin(), rect2.get_xMin()), Mathf.Min(rect1.get_yMin(), rect2.get_yMin()), Mathf.Max(rect1.get_xMax(), rect2.get_xMax()), Mathf.Max(rect1.get_yMax(), rect2.get_yMax()));
		}
		public static Rect Scale(this Rect rect, float scale)
		{
			return new Rect(rect.get_x() * scale, rect.get_y() * scale, rect.get_width() * scale, rect.get_height() * scale);
		}
		public static Rect MinSize(this Rect rect, float minWidth, float minHeight)
		{
			return new Rect(rect.get_x(), rect.get_y(), Mathf.Max(rect.get_width(), minWidth), Mathf.Max(rect.get_height(), minHeight));
		}
		public static Rect MinSize(this Rect rect, Vector2 minSize)
		{
			return new Rect(rect.get_x(), rect.get_y(), Mathf.Max(rect.get_width(), minSize.x), Mathf.Max(rect.get_height(), minSize.y));
		}
	}
}
