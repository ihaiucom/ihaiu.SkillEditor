using HutongGames.PlayMaker;
using System;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public static class Menus
	{
		public static GenericMenu AddColorMenu(GenericMenu menu, string root, int selectedColorIndex, GenericMenu.MenuFunction2 function)
		{
			root = Menus.MakeMenuRoot(root);
			for (int i = 0; i < 8; i++)
			{
				string text = PlayMakerPrefs.get_ColorNames()[i];
				menu.AddItem(new GUIContent(root + text), selectedColorIndex == i, function, i);
			}
			menu.AddSeparator(root);
			for (int j = 8; j < 24; j++)
			{
				string text2 = PlayMakerPrefs.get_ColorNames()[j];
				if (!string.IsNullOrEmpty(text2))
				{
					menu.AddItem(new GUIContent(root + text2), selectedColorIndex == j, function, j);
				}
			}
			return menu;
		}
		public static string MakeMenuRoot(string menuItem)
		{
			if (string.IsNullOrEmpty(menuItem))
			{
				return string.Empty;
			}
			if (menuItem.get_Chars(menuItem.get_Length() - 1) != '/')
			{
				menuItem += '/';
			}
			return menuItem;
		}
		public static void AddCategorizedEventMenuItem(ref GenericMenu menu, string category, SkillEvent fsmEvent)
		{
			if (category != "")
			{
				category += '/';
			}
			menu.AddItem(new GUIContent(category + fsmEvent.get_Name()), SkillEditor.SelectedTransition.get_FsmEvent() == fsmEvent, new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuSelectGlobalEvent), fsmEvent);
		}
	}
}
