using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace HutongGames.Editor
{
	public class EditorHacks
	{
		private static readonly Action<string> focusTextInControl;
		private static MethodInfo getIconForAction;
		public static string[] SortingLayerNames
		{
			get;
			private set;
		}
		static EditorHacks()
		{
			EditorHacks.getIconForAction = typeof(EditorGUIUtility).GetMethod("GetIconForObject", 40);
			MethodInfo method = typeof(EditorGUI).GetMethod("FocusTextInControl", 24);
			if (method != null)
			{
				EditorHacks.focusTextInControl = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), method);
			}
		}
		public static void FocusTextInControl(string controlName)
		{
			if (EditorHacks.focusTextInControl != null)
			{
				EditorHacks.focusTextInControl.Invoke(controlName);
				return;
			}
			GUI.FocusControl(controlName);
		}
		public static Texture GetIconForObject(GameObject go)
		{
			if (go == null)
			{
				return null;
			}
			return EditorHacks.getIconForAction.Invoke(null, new object[]
			{
				go
			}) as Texture;
		}
		public static string[] UpdateSortingLayerNames()
		{
			Type typeFromHandle = typeof(InternalEditorUtility);
			PropertyInfo property = typeFromHandle.GetProperty("sortingLayerNames", 40);
			return EditorHacks.SortingLayerNames = (string[])property.GetValue(null, new object[0]);
		}
	}
}
