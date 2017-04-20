using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace HutongGames.Editor
{
	public class PopupWindowManager
	{
		private EditorWindow parentWindow;
		private List<PopupWindow> popupWindows;
		private static FieldInfo focusedWindowField;
		public static int focusedWindow
		{
			get
			{
				if (PopupWindowManager.focusedWindowField == null)
				{
					PopupWindowManager.focusedWindowField = typeof(GUI).GetField("focusedWindow", 40);
				}
				return (int)PopupWindowManager.focusedWindowField.GetValue(null);
			}
		}
		public PopupWindowManager(EditorWindow window)
		{
			this.parentWindow = window;
			this.popupWindows = new List<PopupWindow>();
		}
		public void OnGUI()
		{
			if (this.popupWindows.get_Count() == 0)
			{
				return;
			}
			this.parentWindow.BeginWindows();
			using (List<PopupWindow>.Enumerator enumerator = this.popupWindows.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PopupWindow current = enumerator.get_Current();
					current.DoGUI();
				}
			}
			this.parentWindow.EndWindows();
		}
		public bool HitTestPopups(Vector2 position)
		{
			using (List<PopupWindow>.Enumerator enumerator = this.popupWindows.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PopupWindow current = enumerator.get_Current();
					if (current.Position.Contains(position))
					{
						return true;
					}
				}
			}
			return false;
		}
		public int GetNextPopupID()
		{
			return this.popupWindows.get_Count();
		}
		public PopupWindow AddWindow(Type popupType, Rect position)
		{
			PopupWindow popupWindow = (PopupWindow)Activator.CreateInstance(popupType, new object[]
			{
				this.GetNextPopupID()
			});
			popupWindow.Position = position;
			return this.AddWindow(popupWindow);
		}
		public PopupWindow AddWindow(PopupWindow popup)
		{
			this.popupWindows.Add(popup);
			this.parentWindow.Repaint();
			return popup;
		}
		public void RemoveWindow(PopupWindow popup)
		{
			this.popupWindows.Remove(popup);
			this.parentWindow.Repaint();
		}
	}
}
