using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	public static class PackageExporter
	{
		private const string buildsPath = "/Documents/Unity/Playmaker/Builds/";
		private const string projectsPath = "/Documents/Unity/Playmaker/Projects/";
		private static string userProfileDir;
		private static string runtimeVersion;
		private static string editorVersion;
		private static string runtimeVersionWebGL;
		private static string runtimeVersionWSA;
		private static string runtimeVersionWP8;
		public static void Export(string packageName)
		{
			if (PackageExporter.VerifyProject())
			{
				string unitypackageFileName = PackageExporter.GetUnitypackageFileName(packageName);
				AssetDatabase.ExportPackage("Assets/PlayMaker", unitypackageFileName, 2);
				Debug.Log("Exported Package: " + unitypackageFileName);
			}
		}
		public static void ExportPlugin(string packageName)
		{
			if (PackageExporter.VerifyProject())
			{
				string unitypackageFileName = PackageExporter.GetUnitypackageFileName(packageName);
				AssetDatabase.ExportPackage(new string[]
				{
					"Assets/PlayMaker",
					"Assets/Plugins"
				}, unitypackageFileName, 2);
				Debug.Log("Exported Package: " + unitypackageFileName);
			}
		}
		public static void ExportFullProduct(string packageName)
		{
			string[] array = new string[]
			{
				"Assets/Gizmos",
				"Assets/iTween",
				"Assets/Photon Unity Networking",
				"Assets/PlayMaker",
				"Assets/Plugins"
			};
			if (PackageExporter.VerifyProject())
			{
				string unitypackageFileName = PackageExporter.GetUnitypackageFileName(packageName);
				AssetDatabase.ExportPackage(array, unitypackageFileName, 2);
				Debug.Log("Exported Package: " + unitypackageFileName);
				if (packageName == "Playmaker")
				{
					PackageExporter.CopyBuildToFinalInstall(packageName);
					return;
				}
			}
			else
			{
				Debug.LogError("Failed to export package!");
			}
		}
		private static string GetUnitypackageFileName(string packageName)
		{
			PackageExporter.userProfileDir = Environment.GetEnvironmentVariable("userprofile");
			string text = PackageExporter.userProfileDir + "/Documents/Unity/Playmaker/Builds/Unity" + PackageExporter.GetUnityVersion();
			Directory.CreateDirectory(text);
			return string.Concat(new string[]
			{
				text,
				"/",
				packageName,
				".",
				PackageExporter.runtimeVersion,
				".unitypackage"
			});
		}
		private static string GetUnityVersion()
		{
			string[] array = Application.get_unityVersion().Split(new char[]
			{
				'.'
			});
			return array[0] + "." + array[1];
		}
		private static void CopyBuildToFinalInstall(string packageName)
		{
			string unityVersion = PackageExporter.GetUnityVersion();
			string unitypackageFileName = PackageExporter.GetUnitypackageFileName(packageName);
			packageName = packageName + "." + PackageExporter.GetShortVersionInfo() + ".unitypackage";
			PackageExporter.userProfileDir = Environment.GetEnvironmentVariable("userprofile");
			if (unityVersion == "4.6")
			{
				string text = PackageExporter.userProfileDir + "/Documents/Unity/Playmaker/Projects/Playmaker.final.unity/Assets/PlayMaker/Editor/Install/" + packageName;
				FileUtil.DeleteFileOrDirectory(text);
				FileUtil.CopyFileOrDirectory(unitypackageFileName, text);
				return;
			}
			if (unityVersion == "5.0")
			{
				string text2 = PackageExporter.userProfileDir + "/Documents/Unity/Playmaker/Projects/Unity5.0/Playmaker.final.unity/Assets/PlayMaker/Editor/Install/" + packageName;
				FileUtil.DeleteFileOrDirectory(text2);
				FileUtil.CopyFileOrDirectory(unitypackageFileName, text2);
				return;
			}
			if (unityVersion == "5.3")
			{
				string text3 = PackageExporter.userProfileDir + "/Documents/Unity/Playmaker/Projects/Unity5.3/Playmaker.final.unity/Assets/PlayMaker/Editor/Install/" + packageName;
				FileUtil.DeleteFileOrDirectory(text3);
				FileUtil.CopyFileOrDirectory(unitypackageFileName, text3);
				return;
			}
			if (unityVersion == "5.4")
			{
				string text4 = PackageExporter.userProfileDir + "/Documents/Unity/Playmaker/Projects/Unity5.4/Playmaker.final.unity/Assets/PlayMaker/Editor/Install/" + packageName;
				FileUtil.DeleteFileOrDirectory(text4);
				FileUtil.CopyFileOrDirectory(unitypackageFileName, text4);
				return;
			}
			Debug.LogError("Unrecognized Unity Version: " + unityVersion);
		}
		private static bool VerifyProject()
		{
			PackageExporter.FixLocalizedResources();
			return PackageExporter.CheckVersionInfo() && PackageExporter.VerifyNoPlayMakerPrefs() && PackageExporter.VerifyNoPlayMakerGlobals();
		}
		private static bool CheckVersionInfo()
		{
			PackageExporter.UpdateVersionInfo();
			if (!(PackageExporter.runtimeVersion == PackageExporter.editorVersion) || !(PackageExporter.runtimeVersion == PackageExporter.runtimeVersionWebGL) || !(PackageExporter.runtimeVersion == PackageExporter.runtimeVersionWP8) || !(PackageExporter.runtimeVersion == PackageExporter.runtimeVersionWSA))
			{
				throw new InvalidDataException("DLL VersionInfo mismatch:\n" + PackageExporter.GetVersionInfo());
			}
			return true;
		}
		private static void UpdateVersionInfo()
		{
			bool flag = Application.get_unityVersion().StartsWith("5");
			string text = flag ? "Assets/Plugins/PlayMaker/PlayMaker.dll" : "Assets/PlayMaker/PlayMaker.dll";
			PackageExporter.runtimeVersion = FileVersionInfo.GetVersionInfo(text).get_ProductVersion();
			string text2 = flag ? "Assets/Plugins/PlayMaker/WebGL/PlayMaker.dll" : "Assets/PlayMaker/PlayMaker.dll";
			PackageExporter.runtimeVersionWebGL = FileVersionInfo.GetVersionInfo(text2).get_ProductVersion();
			string text3 = flag ? "Assets/Plugins/PlayMaker/Metro/PlayMaker.dll" : "Assets/Plugins/Metro/PlayMaker.dll";
			PackageExporter.runtimeVersionWSA = FileVersionInfo.GetVersionInfo(text3).get_ProductVersion();
			string text4 = flag ? "Assets/Plugins/PlayMaker/WP8/PlayMaker.dll" : "Assets/PlayMaker/PlayMaker.dll";
			PackageExporter.runtimeVersionWP8 = FileVersionInfo.GetVersionInfo(text4).get_ProductVersion();
			PackageExporter.editorVersion = FileVersionInfo.GetVersionInfo("Assets/PlayMaker/Editor/PlayMakerEditor.dll").get_ProductVersion();
			Debug.Log(PackageExporter.GetVersionInfo());
		}
		private static string GetVersionInfo()
		{
			return string.Concat(new string[]
			{
				"RuntimeVersion: ",
				PackageExporter.runtimeVersion,
				"\nRuntimeVersionWSA: ",
				PackageExporter.runtimeVersionWSA,
				"\nRuntimeVersionWP8: ",
				PackageExporter.runtimeVersionWP8,
				"\nRuntimeVersionWebGL: ",
				PackageExporter.runtimeVersionWebGL,
				"\nEditorVersion: ",
				PackageExporter.editorVersion
			});
		}
		private static string GetShortVersionInfo()
		{
			string[] array = PackageExporter.runtimeVersion.Split(new char[]
			{
				'.'
			});
			return string.Concat(new string[]
			{
				array[0],
				".",
				array[1],
				".",
				array[2]
			});
		}
		private static bool VerifyNoPlayMakerGlobals()
		{
			PlayMakerGlobals[] array = Resources.FindObjectsOfTypeAll<PlayMakerGlobals>();
			if (array.Length > 0)
			{
				Debug.LogError("Project has PlayMakerGlobals which will overwrite user globals!", array[0]);
				Debug.Log(AssetDatabase.GetAssetPath(array[0]), array[0]);
			}
			return array.Length == 0;
		}
		private static bool VerifyNoPlayMakerPrefs()
		{
			Object @object = Resources.Load("PlayMakerPrefs");
			if (@object != null)
			{
				Debug.LogError("Project has PlayMakerPrefs which will overwrite user preferences!", @object);
				Debug.Log(AssetDatabase.GetAssetPath(@object), @object);
			}
			return @object == null;
		}
		private static void FixLocalizedResources()
		{
			Debug.Log("Fix Localized Resources");
			string[] files = Directory.GetFiles(Application.get_dataPath(), "PlayMakerEditorResources.resources.dll", 1);
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				PluginImporter pluginImporter = PackageExporter.GetPluginImporter(text.Substring(Application.get_dataPath().get_Length() - 6));
				pluginImporter.SetCompatibleWithEditor(false);
				pluginImporter.SaveAndReimport();
			}
		}
		private static PluginImporter GetPluginImporter(string pluginPath)
		{
			PluginImporter pluginImporter = (PluginImporter)AssetImporter.GetAtPath(pluginPath);
			if (pluginImporter != null)
			{
				return pluginImporter;
			}
			Debug.LogWarning("Couldn't find plugin: " + pluginPath);
			return null;
		}
	}
}
