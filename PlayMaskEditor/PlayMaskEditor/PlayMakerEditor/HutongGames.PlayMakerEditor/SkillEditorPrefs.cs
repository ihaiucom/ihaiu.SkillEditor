using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Obsolete("Use HutongGames.PlayMakerEditor.StartupPrefs instead.")]
	public class SkillEditorPrefs : ScriptableObject
	{
		[SerializeField]
		private bool showWelcomeScreen = true;
		private static SkillEditorPrefs instance;
		[SerializeField]
		private string playmakerVersion;
		public static SkillEditorPrefs Instance
		{
			get
			{
				if (SkillEditorPrefs.instance == null)
				{
					string text = Path.Combine(SkillPaths.EditorPath, "PlayMakerEditorPrefs.asset");
					SkillEditorPrefs.instance = (AssetDatabase.LoadAssetAtPath(text, typeof(SkillEditorPrefs)) as SkillEditorPrefs);
					if (SkillEditorPrefs.instance == null)
					{
						SkillEditorPrefs.instance = ScriptableObject.CreateInstance<SkillEditorPrefs>();
						SkillEditor.CreateAsset(SkillEditorPrefs.instance, ref text);
						Debug.Log("Creating PlayMakerEditorPrefs asset: " + text);
					}
				}
				return SkillEditorPrefs.instance;
			}
		}
		public static string PlaymakerVersion
		{
			get
			{
				return SkillEditorPrefs.Instance.playmakerVersion;
			}
			set
			{
				SkillEditorPrefs.Instance.playmakerVersion = value;
				SkillEditorPrefs.Save();
			}
		}
		public static bool ShowWelcomeScreen
		{
			get
			{
				return SkillEditorPrefs.Instance.showWelcomeScreen;
			}
			set
			{
				if (value == SkillEditorPrefs.Instance.showWelcomeScreen)
				{
					return;
				}
				SkillEditorPrefs.Instance.showWelcomeScreen = value;
				SkillEditorPrefs.Save();
			}
		}
		public static void Save()
		{
			EditorUtility.SetDirty(SkillEditorPrefs.Instance);
		}
	}
}
