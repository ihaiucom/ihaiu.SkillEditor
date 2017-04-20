using System;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class SceneGUI
	{
		public static void DrawCameraFrustrum(Camera cam)
		{
			Vector3[] array = new Vector3[]
			{
				cam.ViewportToWorldPoint(new Vector3(0f, 0f, cam.get_farClipPlane())),
				cam.ViewportToWorldPoint(new Vector3(0f, 1f, cam.get_farClipPlane())),
				cam.ViewportToWorldPoint(new Vector3(1f, 1f, cam.get_farClipPlane())),
				cam.ViewportToWorldPoint(new Vector3(1f, 0f, cam.get_farClipPlane())),
				cam.ViewportToWorldPoint(new Vector3(0f, 0f, cam.get_nearClipPlane())),
				cam.ViewportToWorldPoint(new Vector3(0f, 1f, cam.get_nearClipPlane())),
				cam.ViewportToWorldPoint(new Vector3(1f, 1f, cam.get_nearClipPlane())),
				cam.ViewportToWorldPoint(new Vector3(1f, 0f, cam.get_nearClipPlane()))
			};
			Handles.DrawLine(array[0], array[1]);
			Handles.DrawLine(array[1], array[2]);
			Handles.DrawLine(array[2], array[3]);
			Handles.DrawLine(array[3], array[0]);
			Handles.DrawLine(array[4], array[5]);
			Handles.DrawLine(array[5], array[6]);
			Handles.DrawLine(array[6], array[7]);
			Handles.DrawLine(array[7], array[4]);
			Handles.DrawLine(array[0], array[4]);
			Handles.DrawLine(array[1], array[5]);
			Handles.DrawLine(array[2], array[6]);
			Handles.DrawLine(array[3], array[7]);
		}
	}
}
