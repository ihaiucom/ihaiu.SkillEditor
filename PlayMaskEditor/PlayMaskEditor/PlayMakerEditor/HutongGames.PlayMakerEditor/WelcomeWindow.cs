using HutongGames.PlayMaker;
using System;
using System.ComponentModel;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	public class WelcomeWindow : BaseEditorWindow
	{
		public delegate void LinkFunction(object userData);
		private const string urlSamples = "http://www.hutonggames.com/samples.php";
		private const string urlTutorials = "http://www.hutonggames.com/tutorials.html";
		private const string urlDocs = "https://hutonggames.fogbugz.com/default.asp?W1";
		private const string urlForums = "http://hutonggames.com/playmakerforum/index.php";
		private const string urlPhotonAddon = "https://hutonggames.fogbugz.com/default.asp?W928";
		private const string urlAddonsWiki = "https://hutonggames.fogbugz.com/default.asp?W714";
		private const string urlStore = "http://www.hutonggames.com/store.html";
		private const string blackBerryAddonID = "10530";
		private const string wp8AddonID = "10602";
		private const float windowWidth = 500f;
		private static bool setupPhoton;
		private bool showAtStartup;
		private Rect versionLabelRect;
		private int pageNumber;
		public override void Initialize()
		{
			base.SetTitle(Strings.get_WelcomeWindow_Title());
			base.set_minSize(new Vector2(500f, 440f));
			base.set_maxSize(new Vector2(500f, 441f));
			this.showAtStartup = EditorPrefs.GetBool(EditorPrefStrings.get_ShowWelcomeScreen(), true);
			WelcomeWindow.setupPhoton = (ReflectionUtils.GetGlobalType("PlayMakerPhotonWizard") != null);
			Rect rect = new Rect(base.get_position());
			rect.set_width(495f);
			rect.set_height(20f);
			this.versionLabelRect = rect;
		}
		public override void DoGUI()
		{
			SkillEditorStyles.Init();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			SkillEditorGUILayout.PlaymakerHeader(this);
			GUI.Label(this.versionLabelRect, VersionInfo.PlayMakerVersionLabel, SkillEditorStyles.RightAlignedLabel);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Space(20f);
			GUILayout.FlexibleSpace();
			switch (this.pageNumber)
			{
			case 0:
				WelcomeWindow.Link(SkillEditorStyles.SamplesIcon, Strings.get_WelcomeWindow_Samples(), Strings.get_WelcomeWindow_Samples_More(), new WelcomeWindow.LinkFunction(this.OpenUrl), "http://www.hutonggames.com/samples.php");
				WelcomeWindow.Link(SkillEditorStyles.VideoIcon, Strings.get_WelcomeWindow_Tutorials(), Strings.get_WelcomeWindow_Tutorials_More(), new WelcomeWindow.LinkFunction(this.OpenUrl), "http://www.hutonggames.com/tutorials.html");
				WelcomeWindow.Link(SkillEditorStyles.DocsIcon, Strings.get_WelcomeWindow_Docs(), Strings.get_WelcomeWindow_Docs_More(), new WelcomeWindow.LinkFunction(this.OpenUrl), "https://hutonggames.fogbugz.com/default.asp?W1");
				WelcomeWindow.Link(SkillEditorStyles.ForumIcon, Strings.get_WelcomeWindow_Forums(), Strings.get_WelcomeWindow_Forums_More(), new WelcomeWindow.LinkFunction(this.OpenUrl), "http://hutonggames.com/playmakerforum/index.php");
				WelcomeWindow.Link(SkillEditorStyles.AddonsIcon, Strings.get_WelcomeWindow_Addons(), Strings.get_WelcomeWindow_Addons_More(), new WelcomeWindow.LinkFunction(this.GotoPage), 1);
				break;
			case 1:
				WelcomeWindow.Link(SkillEditorStyles.BlackBerryAddonIcon, "BlackBerry Add-On", "NEW! Develop for the new BlackBerry10 platform.", new WelcomeWindow.LinkFunction(this.OpenInAssetStore), "10530");
				WelcomeWindow.Link(SkillEditorStyles.MetroAddonIcon, "Windows Phone 8 Add-On", "NEW! Develop for Windows Phone 8 devices.", new WelcomeWindow.LinkFunction(this.OpenInAssetStore), "10602");
				WelcomeWindow.Link(SkillEditorStyles.MetroAddonIcon, "Windows Store Apps Add-On", "Coming Soon: Develop Windows 8 Store Apps.", new WelcomeWindow.LinkFunction(this.OpenInAssetStore), "http://www.hutonggames.com/store.html");
				if (WelcomeWindow.setupPhoton)
				{
					WelcomeWindow.Link(SkillEditorStyles.PhotonIcon, Strings.get_WelcomeWindow_Photon_Cloud(), Strings.get_WelcomeWindow_Photon_Cloud_More(), new WelcomeWindow.LinkFunction(this.LaunchPhotonSetupWizard), null);
				}
				else
				{
					WelcomeWindow.Link(SkillEditorStyles.PhotonIcon, Strings.get_WelcomeWindow_Photon_Cloud(), Strings.get_WelcomeWindow_Photon_Cloud_More(), new WelcomeWindow.LinkFunction(this.OpenUrl), "https://hutonggames.fogbugz.com/default.asp?W928");
				}
				WelcomeWindow.Link(SkillEditorStyles.AddonsIcon, "Browse Add-Ons Online", "Find action packs and add-ons for NGUI, 2D Toolkit, Mecanim, Pathfinding, Smooth Moves, Ultimate FPS...", new WelcomeWindow.LinkFunction(this.OpenUrl), "https://hutonggames.fogbugz.com/default.asp?W714");
				break;
			}
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (this.pageNumber > 0)
			{
				GUILayout.Space(10f);
				if (GUILayout.Button("Back to Welcome Screen", EditorStyles.get_label(), new GUILayoutOption[0]))
				{
					this.GotoPage(0);
					GUIUtility.ExitGUI();
					return;
				}
				EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), 4);
			}
			GUILayout.FlexibleSpace();
			bool flag = GUILayout.Toggle(this.showAtStartup, Strings.get_WelcomeWindow_Show_at_Startup(), new GUILayoutOption[0]);
			if (flag != this.showAtStartup)
			{
				this.showAtStartup = flag;
				EditorPrefs.SetBool(EditorPrefStrings.get_ShowWelcomeScreen(), this.showAtStartup);
			}
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndVertical();
		}
		private static void Link(Texture texture, string heading, string body, WelcomeWindow.LinkFunction func, object userData)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(64f);
			GUILayout.Box(texture, GUIStyle.get_none(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(48f)
			});
			GUILayout.Space(10f);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Space(1f);
			GUILayout.Label(heading, EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			GUILayout.Label(body, SkillEditorStyles.LabelWithWordWrap, new GUILayoutOption[0]);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			Rect lastRect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect(lastRect, 4);
			if (Event.get_current().get_type() == null && lastRect.Contains(Event.get_current().get_mousePosition()))
			{
				func(userData);
			}
			GUILayout.Space(10f);
		}
		private void LaunchPhotonSetupWizard(object userData)
		{
			ReflectionUtils.GetGlobalType("PlayMakerPhotonWizard").GetMethod("Init").Invoke(null, null);
		}
		private void OpenUrl(object userData)
		{
			Application.OpenURL(userData as string);
		}
		private void OpenInAssetStore(object userData)
		{
			AssetStore.Open("content/" + userData);
		}
		private void GotoPage(object userData)
		{
			this.pageNumber = (int)userData;
			base.Repaint();
			GUIUtility.ExitGUI();
		}
	}
}
