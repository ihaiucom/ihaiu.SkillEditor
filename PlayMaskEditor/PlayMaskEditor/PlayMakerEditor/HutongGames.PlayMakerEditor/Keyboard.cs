using System;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class Keyboard
	{
		private static bool resetFocus;
		public static bool IsGuiEventKeyboardShortcut()
		{
			if (GUIUtility.get_keyboardControl() != 0)
			{
				return false;
			}
			int controlID = GUIUtility.GetControlID(1);
			return Event.get_current().GetTypeForControl(controlID) == 4;
		}
		public static void ResetFocus()
		{
			Keyboard.resetFocus = true;
		}
		public static void Update()
		{
			if (Keyboard.resetFocus)
			{
				GUIUtility.set_keyboardControl(0);
				Keyboard.resetFocus = false;
			}
		}
		public static bool AltAction()
		{
			return Keyboard.Action() && Keyboard.Alt();
		}
		public static bool Alt()
		{
			return (Event.get_current().get_modifiers() & 4) == 4;
		}
		public static bool Control()
		{
			return (Event.get_current().get_modifiers() & 2) == 2;
		}
		public static bool Action()
		{
			return EditorGUI.get_actionKey();
		}
		public static bool EnterKeyPressed()
		{
			return Event.get_current().get_keyCode() == 13 || Event.get_current().get_keyCode() == 271;
		}
		public static bool CommitKeyPressed()
		{
			return Event.get_current().get_keyCode() == 13 || Event.get_current().get_keyCode() == 271;
		}
	}
}
