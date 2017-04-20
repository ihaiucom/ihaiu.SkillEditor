using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public static class GlobalsAsset
	{
		private const string copiedGlobalsFilename = "Assets/PlaymakerGlobals_EXPORTED.asset";
		private const string exportedUnitypackage = "PlayMakerGlobals.unitypackage";
		private static PlayMakerGlobals projectGlobals;
		[Localizable(false)]
		public static void Export()
		{
			AssetDatabase.Refresh();
			SkillEditor.SaveGlobals();
			AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(PlayMakerGlobals.get_Instance()), "Assets/PlaymakerGlobals_EXPORTED.asset");
			AssetDatabase.Refresh();
			string text = EditorUtility.SaveFilePanel(Strings.get_Dialog_Export_Globals(), "", "PlayMakerGlobals.unitypackage", "unitypackage");
			if (text.get_Length() == 0)
			{
				return;
			}
			AssetDatabase.ExportPackage("Assets/PlaymakerGlobals_EXPORTED.asset", text);
			AssetDatabase.DeleteAsset("Assets/PlaymakerGlobals_EXPORTED.asset");
			AssetDatabase.Refresh();
			Dialogs.OkDialog(Strings.get_Labels_Use_Import_Globals_());
		}
		[Localizable(false)]
		public static void Import()
		{
			AssetDatabase.Refresh();
			GlobalsAsset.projectGlobals = PlayMakerGlobals.get_Instance();
			string text = EditorUtility.OpenFilePanel(Strings.get_Dialog_Import_Globals(), "", "unitypackage");
			if (text.get_Length() == 0)
			{
				return;
			}
			AssetDatabase.ImportPackage("PlayMakerGlobals.unitypackage", false);
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(GlobalsAsset.DoImport));
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(GlobalsAsset.DoImport));
		}
		private static void DoImport()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(GlobalsAsset.DoImport));
			Debug.Log(Strings.get_Label_Importing_Globals_());
			PlayMakerGlobals[] array = (PlayMakerGlobals[])Resources.FindObjectsOfTypeAll(typeof(PlayMakerGlobals));
			if (array.Length == 1)
			{
				Dialogs.OkDialog(Strings.get_Dialog_No_Globals_to_import());
				return;
			}
			string text = "";
			PlayMakerGlobals[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				PlayMakerGlobals playMakerGlobals = array2[i];
				if (playMakerGlobals != GlobalsAsset.projectGlobals)
				{
					Debug.Log(Strings.get_Label_Importing_() + AssetDatabase.GetAssetPath(playMakerGlobals));
					text += SkillBuilder.MergeGlobals(playMakerGlobals, GlobalsAsset.projectGlobals);
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				Dialogs.OkDialog(Strings.get_Dialog_ImportGlobals_Error() + Environment.get_NewLine() + text);
			}
			PlayMakerGlobals[] array3 = array;
			for (int j = 0; j < array3.Length; j++)
			{
				PlayMakerGlobals playMakerGlobals2 = array3[j];
				if (playMakerGlobals2 != GlobalsAsset.projectGlobals)
				{
					AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(playMakerGlobals2));
				}
			}
			SkillEditor.SaveGlobals();
		}
	}
}
