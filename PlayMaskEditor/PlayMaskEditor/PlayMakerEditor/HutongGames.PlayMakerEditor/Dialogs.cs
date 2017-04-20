using System;
using UnityEditor;
namespace HutongGames.PlayMakerEditor
{
	internal class Dialogs
	{
		public static void MissingAction(string actionName)
		{
			Dialogs.OkDialog(Strings.get_Dialogs_Missing_Action(), actionName);
		}
		public static void OkDialog(string message)
		{
			Dialogs.OkDialog(Strings.get_ProductName(), message);
		}
		public static void OkDialog(string title, string message)
		{
			EditorUtility.DisplayDialog(title, message, Strings.get_OK());
		}
		public static bool YesNoDialog(string message)
		{
			return Dialogs.YesNoDialog(Strings.get_ProductName(), message);
		}
		public static bool YesNoDialog(string title, string message)
		{
			return EditorUtility.DisplayDialog(title, message, Strings.get_Yes(), Strings.get_No());
		}
		public static bool AreYouSure(string title)
		{
			return Dialogs.AreYouSure(title, Strings.get_Dialog_Are_you_sure());
		}
		public static bool AreYouSure(string title, string message)
		{
			return EditorUtility.DisplayDialog(title, message, Strings.get_OK(), Strings.get_Command_Cancel());
		}
		public static void PreviewVersion()
		{
			if (EditorUtility.DisplayDialog(Strings.get_Dialogs_PREVIEW_VERSION(), Strings.get_Dialogs_PreviewVersion_Note(), "HutongGames.com", Strings.get_Label_OK()))
			{
				EditorCommands.OpenProductWebPage();
			}
		}
	}
}
