using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class VersionInfo
	{
		public static string VersionOverride;
		[Localizable(false)]
		public static string AssemblyVersion
		{
			get
			{
				if (EditorApp.IsSourceCodeVersion)
				{
					return "Source Code Version";
				}
				return VersionInfo.GetAssemblyInformationalVersion();
			}
		}
		public static string PlayMakerVersionLabel
		{
			get
			{
				string versionLabel = PlayMakerFSM.get_VersionLabel();
				if (!string.IsNullOrEmpty(versionLabel))
				{
					return versionLabel;
				}
				return "";
			}
		}
		public static string PlayMakerVersionInfo
		{
			get
			{
				string versionNotes = PlayMakerFSM.get_VersionNotes();
				if (!string.IsNullOrEmpty(versionNotes))
				{
					return versionNotes;
				}
				return "";
			}
		}
		[Localizable(false)]
		public static string GetAssemblyInformationalVersion()
		{
			string text = VersionInfo.VersionOverride;
			try
			{
				if (string.IsNullOrEmpty(text))
				{
					text = VersionInfo.GetInformationalVersion(Assembly.GetExecutingAssembly());
				}
			}
			catch (Exception)
			{
				Debug.LogWarning("Couldn't get Playmaker Version Info!");
				text = "unknown";
			}
			return text + ((VersionInfo.PlayMakerVersionLabel != "") ? (" - " + VersionInfo.PlayMakerVersionLabel) : "");
		}
		public static string GetInformationalVersion(Assembly assembly)
		{
			return FileVersionInfo.GetVersionInfo(assembly.get_Location()).get_ProductVersion();
		}
	}
}
