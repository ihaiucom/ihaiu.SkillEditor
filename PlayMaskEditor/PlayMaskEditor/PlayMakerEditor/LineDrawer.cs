using System;
using UnityEditor;
using UnityEngine;
public class LineDrawer
{
	private Color color = Color.get_white();
	private Color drawColor = Color.get_white();
	private Vector3 lineStart = default(Vector3);
	private Vector3 lineEnd = default(Vector3);
	public void SetColor(Color color)
	{
		this.color = color;
		this.drawColor = color;
		Handles.set_color(color);
	}
	public void SetAlpha(float alpha)
	{
		this.drawColor = this.color;
		this.drawColor.a = alpha;
		Handles.set_color(this.drawColor);
	}
	public void DrawVerticalLine(float alpha, float x, float y0, float y1)
	{
		this.DrawLine(alpha, x, y0, x, y1);
	}
	public void DrawVerticalLine(float x, float y0, float y1)
	{
		this.DrawLine(x, y0, x, y1);
	}
	public void DrawLine(float alpha, float x0, float y0, float x1, float y1)
	{
		this.SetAlpha(alpha);
		this.DrawLine(x0, y0, x1, y1);
	}
	public void DrawLine(float x0, float y0, float x1, float y1)
	{
		this.lineStart.Set(x0, y0, 0f);
		this.lineEnd.Set(x1, y1, 0f);
		Handles.DrawLine(this.lineStart, this.lineEnd);
	}
	public void DrawThickVerticalLine(float alpha, float x0, float y0, float y1)
	{
		this.SetAlpha(alpha);
		this.DrawThickVerticalLine(x0, y0, y1);
	}
	public void DrawThickVerticalLine(float x0, float y0, float y1)
	{
		this.lineStart.Set(x0, y0, 0f);
		this.lineEnd.Set(x0, y1, 0f);
		Handles.DrawLine(this.lineStart, this.lineEnd);
		this.lineStart.x = this.lineStart.x + 1f;
		this.lineEnd.x = this.lineEnd.x + 1f;
		Handles.DrawLine(this.lineStart, this.lineEnd);
	}
}
