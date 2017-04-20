using System;
using System.Collections.Generic;
using UnityEngine;
namespace HutongGames.Extensions
{
	public static class TextureExtensions
	{
		public struct Point
		{
			public short x;
			public short y;
			public Point(short aX, short aY)
			{
				this.x = aX;
				this.y = aY;
			}
			public Point(int aX, int aY)
			{
				this = new TextureExtensions.Point((short)aX, (short)aY);
			}
		}
		public static void FloodFillArea(this Texture2D aTex, int aX, int aY, Color32 aFillColor)
		{
			int width = aTex.get_width();
			int height = aTex.get_height();
			Color32[] pixels = aTex.GetPixels32();
			Color color = pixels[aX + aY * width];
			Queue<TextureExtensions.Point> queue = new Queue<TextureExtensions.Point>();
			queue.Enqueue(new TextureExtensions.Point(aX, aY));
			while (queue.get_Count() > 0)
			{
				TextureExtensions.Point point = queue.Dequeue();
				for (int i = (int)point.x; i < width; i++)
				{
					Color color2 = pixels[i + (int)point.y * width];
					if (color2 != color || color2 == aFillColor)
					{
						break;
					}
					pixels[i + (int)point.y * width] = aFillColor;
					if ((int)(point.y + 1) < height)
					{
						color2 = pixels[i + (int)point.y * width + width];
						if (color2 == color && color2 != aFillColor)
						{
							queue.Enqueue(new TextureExtensions.Point(i, (int)(point.y + 1)));
						}
					}
					if (point.y - 1 >= 0)
					{
						color2 = pixels[i + (int)point.y * width - width];
						if (color2 == color && color2 != aFillColor)
						{
							queue.Enqueue(new TextureExtensions.Point(i, (int)(point.y - 1)));
						}
					}
				}
				for (int j = (int)(point.x - 1); j >= 0; j--)
				{
					Color color3 = pixels[j + (int)point.y * width];
					if (color3 != color || color3 == aFillColor)
					{
						break;
					}
					pixels[j + (int)point.y * width] = aFillColor;
					if ((int)(point.y + 1) < height)
					{
						color3 = pixels[j + (int)point.y * width + width];
						if (color3 == color && color3 != aFillColor)
						{
							queue.Enqueue(new TextureExtensions.Point(j, (int)(point.y + 1)));
						}
					}
					if (point.y - 1 >= 0)
					{
						color3 = pixels[j + (int)point.y * width - width];
						if (color3 == color && color3 != aFillColor)
						{
							queue.Enqueue(new TextureExtensions.Point(j, (int)(point.y - 1)));
						}
					}
				}
			}
			aTex.SetPixels32(pixels);
		}
	}
}
