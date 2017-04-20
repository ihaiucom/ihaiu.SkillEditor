using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	public class SkillPaths
	{
		public static string ProjectPath
		{
			get;
			private set;
		}
		public static string RuntimePath
		{
			get;
			private set;
		}
		public static string EditorPath
		{
			get;
			private set;
		}
		public static string EditorResourcesPath
		{
			get;
			private set;
		}
		public static string WatermarksPath
		{
			get;
			private set;
		}
		public static string ResourcesPath
		{
			get;
			private set;
		}
		public static string TemplatesPath
		{
			get;
			private set;
		}
		public static string RuntimeFullPath
		{
			get;
			private set;
		}
		public static string EditorFullPath
		{
			get;
			private set;
		}
		public static string WatermarksFullPath
		{
			get;
			private set;
		}
		public static string ResourcesFullPath
		{
			get;
			private set;
		}
		public static string TemplatesFullPath
		{
			get;
			private set;
		}
		static SkillPaths()
		{
			SkillPaths.LoadPaths();
			SkillPaths.ValidatePaths();
		}
		private static void LoadPaths()
		{
			SkillPaths.RuntimePath = SkillPaths.FixPath(EditorPrefs.GetString("PlayMakerPaths.RuntimePath", "Assets/PlayMaker/"));
			SkillPaths.EditorPath = SkillPaths.FixPath(EditorPrefs.GetString("PlayMakerPaths.EditorPath", "Assets/PlayMaker/Editor/"));
			SkillPaths.EditorResourcesPath = SkillPaths.FixPath(Path.Combine(SkillPaths.EditorPath, "Resources"));
			SkillPaths.WatermarksPath = SkillPaths.FixPath(Path.Combine(SkillPaths.EditorPath, "Watermarks"));
			string text = SkillPaths.EditorPath.Substring(0, SkillPaths.EditorPath.get_Length() - 7);
			SkillPaths.ResourcesPath = SkillPaths.FixPath(Path.Combine(text, "Resources"));
			SkillPaths.TemplatesPath = SkillPaths.FixPath(Path.Combine(text, "Templates"));
			SkillPaths.ProjectPath = Path.Combine(Application.get_dataPath(), "..\\");
			SkillPaths.ProjectPath = SkillPaths.FixPath(Path.GetFullPath(SkillPaths.ProjectPath));
			SkillPaths.RuntimeFullPath = SkillPaths.FixPath(Path.GetFullPath(Path.Combine(SkillPaths.ProjectPath, SkillPaths.RuntimePath)));
			SkillPaths.EditorFullPath = SkillPaths.FixPath(Path.GetFullPath(Path.Combine(SkillPaths.ProjectPath, SkillPaths.EditorPath)));
			SkillPaths.WatermarksFullPath = SkillPaths.FixPath(Path.GetFullPath(Path.Combine(SkillPaths.ProjectPath, SkillPaths.WatermarksPath)));
			SkillPaths.TemplatesFullPath = SkillPaths.FixPath(Path.GetFullPath(Path.Combine(SkillPaths.ProjectPath, SkillPaths.TemplatesPath)));
			SkillPaths.ResourcesFullPath = SkillPaths.FixPath(Path.GetFullPath(Path.Combine(SkillPaths.ProjectPath, SkillPaths.ResourcesPath)));
		}
		private static void SavePaths()
		{
			EditorPrefs.SetString("PlayMakerPaths.RuntimePath", SkillPaths.RuntimePath);
			EditorPrefs.SetString("PlayMakerPaths.EditorPath", SkillPaths.EditorPath);
		}
		private static string FixPath(string path)
		{
			return path.Replace("\\", "/");
		}
		private static void ValidatePaths()
		{
			if (EditorApp.IsSourceCodeVersion)
			{
				SkillPaths.ResetPaths();
				return;
			}
			if (!File.Exists(Path.Combine(SkillPaths.RuntimeFullPath, "PlayMaker.dll")))
			{
				SkillPaths.RuntimeFullPath = SkillPaths.FindPath("PlayMaker.dll");
				SkillPaths.RuntimePath = new Uri(Application.get_dataPath()).MakeRelativeUri(new Uri(SkillPaths.RuntimeFullPath)).ToString();
			}
			if (!File.Exists(Path.Combine(SkillPaths.EditorFullPath, "PlayMakerEditor.dll")))
			{
				SkillPaths.EditorFullPath = SkillPaths.FindPath("PlayMakerEditor.dll");
				SkillPaths.EditorPath = new Uri(Application.get_dataPath()).MakeRelativeUri(new Uri(SkillPaths.EditorFullPath)).ToString();
			}
			SkillPaths.SavePaths();
		}
		public static string GetTemplatesDirectory()
		{
			if (!Directory.Exists(SkillPaths.TemplatesFullPath))
			{
				return Application.get_dataPath();
			}
			return SkillPaths.TemplatesFullPath;
		}
		private static string FindPath(string filename)
		{
			string text = string.Empty;
			IEnumerable<FileInfo> files = new DirectoryInfo(Application.get_dataPath()).GetFiles("*.*", 1);
			IEnumerable<FileInfo> enumerable = Enumerable.OrderBy<FileInfo, string>(Enumerable.Where<FileInfo>(files, (FileInfo file) => file.get_Name() == filename), (FileInfo file) => file.get_Name());
			if (Enumerable.Any<FileInfo>(enumerable))
			{
				text = Path.GetDirectoryName(Enumerable.First<FileInfo>(enumerable).get_FullName());
			}
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogError("PlayMakerPaths: Could not find: " + filename);
			}
			return text;
		}
		private static void ResetPaths()
		{
			SkillPaths.RuntimePath = "Assets/PlayMaker/";
			SkillPaths.EditorPath = "Assets/PlayMaker/Editor/";
			SkillPaths.EditorResourcesPath = "Assets/PlayMaker/Editor/Resources/";
		}
		private static void DebugPaths()
		{
			Debug.Log(string.Concat(new string[]
			{
				"PlayMakerPaths:\nRuntimePath: ",
				SkillPaths.RuntimePath,
				"\nRuntimeFullPath: ",
				SkillPaths.RuntimeFullPath,
				"\nEditorPath: ",
				SkillPaths.EditorPath,
				"\nEditorFullPath: ",
				SkillPaths.EditorFullPath,
				"\nEditorResourcesPath: ",
				SkillPaths.EditorResourcesPath,
				"\nWatermarksPath: ",
				SkillPaths.WatermarksPath,
				"\nWatermarksFullPath: ",
				SkillPaths.WatermarksFullPath,
				"\nResourcesPath: ",
				SkillPaths.ResourcesPath,
				"\nTemplatesPath: ",
				SkillPaths.TemplatesPath,
				"\nResourcesFullPath: ",
				SkillPaths.ResourcesFullPath,
				"\nTemplatesFullPath: ",
				SkillPaths.GetTemplatesDirectory()
			}));
		}
	}
}
