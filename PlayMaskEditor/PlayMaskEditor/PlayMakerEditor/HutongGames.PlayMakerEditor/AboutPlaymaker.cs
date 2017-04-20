using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	public class AboutPlaymaker : BaseEditorWindow
	{
		private static bool heightHasBeenSet;
		public override void Initialize()
		{
			this.InitWindowTitle();
			base.set_minSize(new Vector2(264f, 292f));
			base.set_maxSize(new Vector2(264f, 292f));
			AboutPlaymaker.heightHasBeenSet = false;
		}
		public void InitWindowTitle()
		{
			base.SetTitle(Strings.get_AboutPlaymaker_Title());
		}
		public override void DoGUI()
		{
			SkillEditorStyles.Init();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			SkillEditorGUILayout.PlaymakerHeader(this);
			GUILayout.Label(FsmEditorSettings.ProductCopyright, EditorStyles.get_miniLabel(), new GUILayoutOption[0]);
			GUILayout.Label((!EditorApp.IsSourceCodeVersion) ? string.Format(Strings.get_AboutPlaymaker_Version_Info(), VersionInfo.GetAssemblyInformationalVersion()) : "Source Code Version", new GUILayoutOption[0]);
			if (VersionInfo.PlayMakerVersionInfo != "")
			{
				EditorGUILayout.HelpBox(VersionInfo.PlayMakerVersionInfo, 0);
			}
			EditorGUILayout.HelpBox(string.Format(Strings.get_AboutPlaymaker_Special_Thanks(), "Erin Ko, Kemal Amarasingham, Bruce Blumberg, Steve Gargolinski, Lee Hepler, Bart Simon, Lucas Meijer, Joachim Ante, Jaydee Alley, James Murchison, XiaoHang Zheng, Andrzej Łukasik, Vanessa Wesley, Marek Ledvina, Bob Berkebile, Jean Fabre, MaDDoX, gamesonytablet, Marc 'Dreamora' Schaerer, Eugenio 'Ryo567' Martínez, Steven 'Nightreaver' Barthen, Damiangto, VisionaiR3D, 黄峻, Nilton Felicio, Andre Dantas Lima, Ramprasad Madhavan, and the PlayMaker Community!\r\n"), 0);
			if (GUILayout.Button(Strings.get_AboutPlaymaker_Release_Notes(), new GUILayoutOption[0]))
			{
				EditorCommands.OpenWikiPage(WikiPages.ReleaseNotes);
			}
			if (GUILayout.Button(Strings.get_AboutPlaymaker_Hutong_Games_Link(), new GUILayoutOption[0]))
			{
				Application.OpenURL("http://hutonggames.com/");
			}
			GUILayout.Space(5f);
			GUILayout.EndVertical();
			if (!AboutPlaymaker.heightHasBeenSet && Event.get_current().get_type() == 7)
			{
				this.SetWindowHeightToFitContents();
			}
		}
		private void SetWindowHeightToFitContents()
		{
			float num = GUILayoutUtility.GetLastRect().get_height() + 10f;
			base.get_position().Set(base.get_position().get_x(), base.get_position().get_y(), 264f, num);
			base.set_minSize(new Vector2(264f, num));
			base.set_maxSize(new Vector2(264f, num + 1f));
			AboutPlaymaker.heightHasBeenSet = true;
		}
	}
}
