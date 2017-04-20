using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class BugReportWindow : BaseEditorWindow
	{
		private enum ScoutArea
		{
			Editor,
			Runtime,
			Actions,
			API,
			Documentation
		}
		private Vector2 controlsScrollPosition;
		private WWW www;
		private BugReportWindow.ScoutArea area;
		public static string[] frequencyChoices = new string[]
		{
			Strings.get_BugReportWindow_FrequencyChoices_Always(),
			Strings.get_BugReportWindow_FrequencyChoices_Sometimes__but_not_always(),
			Strings.get_BugReportWindow_FrequencyChoices_This_is_the_first_time()
		};
		private int frequencyIndex;
		private string description;
		private string extra;
		private string email;
		private bool isValid;
		private string errorString;
		[Localizable(false)]
		private void Reset()
		{
			string newLine = Environment.get_NewLine();
			this.area = BugReportWindow.ScoutArea.Editor;
			this.frequencyIndex = 0;
			this.description = "";
			this.extra = string.Concat(new string[]
			{
				Strings.get_BugReportWindow_What_happened(),
				newLine,
				newLine,
				newLine,
				newLine,
				Strings.get_BugReportWindow_How_can_we_reproduce_it(),
				newLine,
				newLine
			});
			this.email = EditorPrefs.GetString(EditorPrefStrings.get_UserEmail(), "");
			base.Repaint();
		}
		public override void Initialize()
		{
			base.SetTitle(Strings.get_ProductName());
			base.set_wantsMouseMove(true);
			if (base.get_position().get_height() < 400f)
			{
				base.set_position(new Rect(base.get_position().get_x(), base.get_position().get_y(), base.get_position().get_width(), 600f));
			}
			base.set_minSize(new Vector2(500f, 400f));
			this.Reset();
		}
		public override void DoGUI()
		{
			SkillEditorStyles.Init();
			SkillEditorGUILayout.ToolWindowLargeTitle(this, Strings.get_BugReportWindow_Title());
			SkillEditorGUILayout.LabelWidth(200f);
			this.controlsScrollPosition = EditorGUILayout.BeginScrollView(this.controlsScrollPosition, new GUILayoutOption[0]);
			GUILayout.Label(Strings.get_BugReportWindow_Bug_Title_Label(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			this.description = EditorGUILayout.TextField(this.description, new GUILayoutOption[0]);
			GUILayout.Label(Strings.get_BugReportWindow_Bug_Description_Label(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			this.extra = EditorGUILayout.TextArea(this.extra, SkillEditorStyles.TextAreaWithWordWrap, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true)
			});
			this.area = (BugReportWindow.ScoutArea)EditorGUILayout.EnumPopup(Strings.get_BugReportWindow_Where_does_it_happen(), this.area, new GUILayoutOption[0]);
			this.frequencyIndex = EditorGUILayout.Popup(Strings.get_BugReportWindow_How_often_does_it_happen(), this.frequencyIndex, BugReportWindow.frequencyChoices, new GUILayoutOption[0]);
			this.email = EditorGUILayout.TextField(new GUIContent(Strings.get_BugReportWindow_Your_E_mail(), Strings.get_BugReportWindow_Your_E_mail_Tooltip()), this.email, new GUILayoutOption[0]);
			EditorGUILayout.EndScrollView();
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("PlayMaker: " + VersionInfo.AssemblyVersion, new GUILayoutOption[0]);
			GUILayout.Label("Unity: " + Application.get_unityVersion(), new GUILayoutOption[0]);
			GUILayout.Label("Build Target: " + EditorUserBuildSettings.get_activeBuildTarget(), new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button(Strings.get_BugReportWindow_Submit_Button(), new GUILayoutOption[0]))
			{
				if (!this.isValid)
				{
					EditorUtility.DisplayDialog(Strings.get_BugReportWindow_Title(), this.errorString, Strings.get_OK());
				}
				else
				{
					this.SubmitBugReportByMail();
				}
				GUIUtility.ExitGUI();
				return;
			}
			if (GUILayout.Button(new GUIContent(Strings.get_Command_Copy(), Strings.get_BugReportWindow_Copy_Tooltip()), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(100f)
			}))
			{
				this.CopyReportToClipboard();
			}
			if (GUILayout.Button(new GUIContent(Strings.get_Command_Reset()), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(100f)
			}))
			{
				GUIUtility.set_keyboardControl(0);
				this.Reset();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			if (GUI.get_changed())
			{
				this.UpdateGUI();
				GUIUtility.ExitGUI();
			}
		}
		private void OnFocus()
		{
			this.UpdateGUI();
		}
		private void Update()
		{
			if (this.www == null)
			{
				return;
			}
			if (this.www.get_isDone())
			{
				EditorUtility.ClearProgressBar();
				EditorUtility.DisplayDialog(Strings.get_ProductName(), string.IsNullOrEmpty(this.www.get_error()) ? Strings.get_BugReportWindow_Success() : string.Format(Strings.get_BugReportWindow_Error(), this.www.get_error()), Strings.get_OK());
				this.www = null;
				return;
			}
			EditorUtility.DisplayProgressBar(Strings.get_ProductName(), Strings.get_BugReportWindow_Progress(), this.www.get_uploadProgress());
		}
		private void UpdateGUI()
		{
			this.isValid = this.IsValidSetup();
		}
		private bool IsValidSetup()
		{
			this.errorString = "";
			if (string.IsNullOrEmpty(this.description))
			{
				this.errorString += Strings.get_BugReportWindow_MissingTitle();
			}
			if (string.IsNullOrEmpty(this.extra))
			{
				this.errorString += Strings.get_BugReportWindow_MissingDescription();
			}
			if (string.IsNullOrEmpty(this.email))
			{
				this.errorString += Strings.get_BugReportWindow_MissingEmail();
			}
			return this.errorString == "";
		}
		[Localizable(false)]
		private void SubmitBugReportByMail()
		{
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("object", this.description);
			wWWForm.AddField("text", this.BuildReportBody(false));
			wWWForm.AddField("email", this.email);
			EditorPrefs.SetString(EditorPrefStrings.get_BugReportWindow_UserEmail(), this.email);
			this.www = new WWW("www.hutonggames.com/SubmitBug.php", wWWForm);
		}
		[Localizable(false)]
		private string BuildReportBody(bool offline = false)
		{
			string text = "";
			string newLine = Environment.get_NewLine();
			if (offline)
			{
				text = text + this.description + newLine;
			}
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"Area: ",
				this.area,
				newLine
			});
			text = text + "Frequency: " + BugReportWindow.frequencyChoices[this.frequencyIndex] + newLine;
			text = text + newLine + this.extra + newLine;
			if (offline)
			{
				text = text + this.email + newLine;
			}
			text = text + newLine + "Unity Info:" + newLine;
			text = text + "Unity Version: " + Application.get_unityVersion() + newLine;
			text = text + "Playmaker Version: " + VersionInfo.GetAssemblyInformationalVersion() + newLine;
			object obj2 = text;
			text = string.Concat(new object[]
			{
				obj2,
				"BuildTarget: ",
				EditorUserBuildSettings.get_activeBuildTarget(),
				newLine
			});
			text = text + newLine + "System Info:" + newLine;
			text = text + "OS: " + SystemInfo.get_operatingSystem() + newLine;
			text = text + "Processor: " + SystemInfo.get_processorType() + newLine;
			object obj3 = text;
			text = string.Concat(new object[]
			{
				obj3,
				"System Memory: ",
				SystemInfo.get_systemMemorySize(),
				newLine
			});
			return text + "Graphics Device: " + SystemInfo.get_graphicsDeviceName() + newLine;
		}
		private void CopyReportToClipboard()
		{
			EditorGUIUtility.set_systemCopyBuffer(this.BuildReportBody(true));
		}
	}
}
