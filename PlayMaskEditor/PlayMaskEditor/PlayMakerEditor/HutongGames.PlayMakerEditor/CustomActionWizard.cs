using HutongGames.PlayMaker;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class CustomActionWizard : BaseEditorWindow
	{
		private Vector2 controlsScrollPosition;
		private Vector2 previewScrollPosition;
		private Rect previewDividerRect;
		private float previewHeight;
		private bool draggingPreviewDivider;
		private string actionName = "";
		private string tooltip = "";
		private string[] actionCategories;
		private int selectedCategory;
		private string customCategory = "";
		private string rootFolder = "PlayMaker/Actions/";
		private string actionFolder = "Animation";
		private bool folderSameAsCategory = true;
		private bool handlesOnEnter = true;
		private bool handlesOnUpdate;
		private bool handlesOnFixedUpdate;
		private bool handlesOnLateUpdate;
		private bool handlesOnExit;
		private bool hasCustomErrorChecker;
		private bool isValid;
		private string errorString;
		private string code;
		private string fullFileName;
		private string localAssetFilename;
		[Localizable(false)]
		public override void Initialize()
		{
			base.SetTitle(Strings.get_CustomActionWizard_Title());
			this.rootFolder = EditorPrefs.GetString("PlayMaker.CustomActionWizard.RootFolder", "PlayMaker/Actions/");
			this.previewHeight = EditorPrefs.GetFloat("PlayMaker.CustomActionWizard.PreviewHeight", 200f);
			base.set_wantsMouseMove(true);
			List<string> list = Enumerable.ToList<string>(Enum.GetNames(typeof(ActionCategory)));
			list.Sort();
			this.actionCategories = list.ToArray();
			if (base.get_position().get_height() < 400f)
			{
				base.set_position(new Rect(base.get_position().get_x(), base.get_position().get_y(), base.get_position().get_width(), 600f));
			}
			base.set_minSize(new Vector2(500f, 400f));
			this.UpdateGUI();
		}
		public override void DoGUI()
		{
			SkillEditorStyles.Init();
			SkillEditorGUILayout.ToolWindowLargeTitle(this, Strings.get_CustomActionWizard_Full_Title());
			SkillEditorGUILayout.LabelWidth(200f);
			this.HandleDragPreviewDivider();
			this.controlsScrollPosition = EditorGUILayout.BeginScrollView(this.controlsScrollPosition, new GUILayoutOption[0]);
			EditorGUI.set_indentLevel(1);
			CustomActionWizard.ControlGroup(Strings.get_CustomActionWizard_Group_Name_and_Description());
			this.actionName = SkillEditorGUILayout.TextFieldWithHint(this.actionName, Strings.get_CustomActionWizard_Label_Action_Name(), new GUILayoutOption[0]);
			this.tooltip = SkillEditorGUILayout.TextAreaWithHint(this.tooltip, Strings.get_CustomActionWizard_Label_Description(), new GUILayoutOption[]
			{
				GUILayout.Height(80f)
			});
			CustomActionWizard.ControlGroup(Strings.get_CustomActionWizard_Group_Category());
			GUI.set_enabled(string.IsNullOrEmpty(this.customCategory));
			this.selectedCategory = EditorGUILayout.Popup(Strings.get_CustomActionWizard_Select_Category(), this.selectedCategory, this.actionCategories, new GUILayoutOption[0]);
			GUI.set_enabled(true);
			this.customCategory = EditorGUILayout.TextField(Strings.get_CustomActionWizard_Custom_Category(), this.customCategory, new GUILayoutOption[0]);
			CustomActionWizard.ControlGroup(Strings.get_CustomActionWizard_Generated_Code_Folder());
			this.rootFolder = EditorGUILayout.TextField(Strings.get_CustomActionWizard_Root_Folder(), this.rootFolder, new GUILayoutOption[0]);
			GUI.set_enabled(!this.folderSameAsCategory);
			this.actionFolder = EditorGUILayout.TextField(Strings.get_CustomActionWizard_Action_Folder(), this.actionFolder, new GUILayoutOption[0]);
			GUI.set_enabled(true);
			this.folderSameAsCategory = EditorGUILayout.Toggle(Strings.get_CustomActionWizard_Same_as_Category(), this.folderSameAsCategory, new GUILayoutOption[0]);
			CustomActionWizard.ControlGroup(Strings.get_CustomActionWizard_Add_Methods());
			this.handlesOnEnter = EditorGUILayout.Toggle("OnEnter", this.handlesOnEnter, new GUILayoutOption[0]);
			this.handlesOnUpdate = EditorGUILayout.Toggle("OnUpdate", this.handlesOnUpdate, new GUILayoutOption[0]);
			this.handlesOnFixedUpdate = EditorGUILayout.Toggle("OnFixedUpdate", this.handlesOnFixedUpdate, new GUILayoutOption[0]);
			this.handlesOnLateUpdate = EditorGUILayout.Toggle("OnLateUpdate", this.handlesOnLateUpdate, new GUILayoutOption[0]);
			this.handlesOnExit = EditorGUILayout.Toggle("OnExit", this.handlesOnExit, new GUILayoutOption[0]);
			this.hasCustomErrorChecker = EditorGUILayout.Toggle(Strings.get_CustomActionWizard_Custom_Error_Checker(), this.hasCustomErrorChecker, new GUILayoutOption[0]);
			EditorGUILayout.EndScrollView();
			if (!this.isValid)
			{
				SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
				EditorGUI.set_indentLevel(0);
				EditorGUILayout.HelpBox(this.errorString, 3, true);
			}
			GUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[0]);
			GUILayout.Label(Strings.get_CustomActionWizard_Code_Preview(), new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			if (Event.get_current().get_type() == 7)
			{
				this.previewDividerRect = GUILayoutUtility.GetLastRect();
			}
			EditorGUIUtility.AddCursorRect(this.previewDividerRect, 2);
			this.previewScrollPosition = EditorGUILayout.BeginScrollView(this.previewScrollPosition, new GUILayoutOption[]
			{
				GUILayout.MinHeight(this.previewHeight)
			});
			GUILayout.Label(this.code, new GUILayoutOption[0]);
			EditorGUILayout.EndScrollView();
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			GUILayout.Label(Strings.get_CustomActionWizard_File_Path_Prefix() + this.localAssetFilename, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUI.set_enabled(this.isValid);
			if (GUILayout.Button(Strings.get_CustomActionWizard_Save_Button(), new GUILayoutOption[0]))
			{
				this.SaveCustomAction();
				GUIUtility.ExitGUI();
				return;
			}
			GUI.set_enabled(true);
			if (GUILayout.Button(new GUIContent(Strings.get_CustomActionWizard_Find_File(), Strings.get_CustomActionWizard_Find_File_Tooltip()), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(100f)
			}))
			{
				this.PingScriptFile();
			}
			if (GUILayout.Button(new GUIContent(Strings.get_CustomActionWizard_Copy_Code(), Strings.get_CustomActionWizard_Copy_Code_Tooltip()), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(100f)
			}))
			{
				this.CopyCodeToClipboard();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			EditorGUI.set_indentLevel(0);
			if (GUI.get_changed())
			{
				this.UpdateGUI();
				GUIUtility.ExitGUI();
			}
		}
		private void OnFocus()
		{
			if (this.Initialized)
			{
				this.UpdateGUI();
			}
			this.draggingPreviewDivider = false;
		}
		[Localizable(false)]
		private void HandleDragPreviewDivider()
		{
			switch (Event.get_current().get_type())
			{
			case 0:
				if (this.previewDividerRect.Contains(Event.get_current().get_mousePosition()))
				{
					this.draggingPreviewDivider = true;
				}
				break;
			case 1:
				this.draggingPreviewDivider = false;
				EditorPrefs.SetFloat("PlayMaker.CustomActionWizard.PreviewHeight", this.previewHeight);
				break;
			default:
				if (this.draggingPreviewDivider && Event.get_current().get_isMouse())
				{
					this.previewHeight = base.get_position().get_height() - Event.get_current().get_mousePosition().y - 60f;
					base.Repaint();
				}
				break;
			}
			this.previewHeight = Mathf.Clamp(this.previewHeight, 40f, base.get_position().get_height() - 200f);
		}
		private static void ControlGroup(string title)
		{
			GUILayout.Space(10f);
			GUILayout.Label(title, EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			GUILayout.Space(5f);
		}
		[Localizable(false)]
		private void UpdateGUI()
		{
			this.previewHeight = Mathf.Clamp(this.previewHeight, 40f, base.get_position().get_height() - 190f);
			EditorPrefs.SetString("PlayMaker.CustomActionWizard.RootFolder", this.rootFolder);
			if (this.folderSameAsCategory)
			{
				this.actionFolder = (string.IsNullOrEmpty(this.customCategory) ? this.actionCategories[this.selectedCategory] : this.customCategory);
			}
			string text = Path.Combine(this.rootFolder, this.actionFolder);
			this.localAssetFilename = Path.Combine(text, this.actionName + ".cs");
			this.fullFileName = Path.Combine(Application.get_dataPath(), this.localAssetFilename);
			this.localAssetFilename = this.localAssetFilename.Replace('\\', '/');
			this.fullFileName = this.fullFileName.Replace('\\', '/');
			this.BuildCustomAction();
			this.isValid = this.IsValidSetup();
		}
		private bool IsValidSetup()
		{
			this.errorString = "";
			if (string.IsNullOrEmpty(this.actionName))
			{
				this.errorString = this.errorString + Strings.get_CustomActionWizard_Invalid_Action_Name() + Environment.get_NewLine();
			}
			if (!CodeGenerator.IsValidLanguageIndependentIdentifier(this.actionName))
			{
				this.errorString = this.errorString + Strings.get_CustomActionWizard_Action_Name_contains_invalid_characters() + Environment.get_NewLine();
			}
			if (File.Exists(this.fullFileName))
			{
				this.errorString = this.errorString + Strings.get_CustomActionWizard_FileExists_Error() + Environment.get_NewLine();
			}
			return this.errorString == "";
		}
		private void SaveCustomAction()
		{
			Debug.Log(Strings.get_CustomActionWizard_Log_Creating_custom_action__() + this.fullFileName);
			string directoryName = Path.GetDirectoryName(this.fullFileName);
			if (string.IsNullOrEmpty(directoryName))
			{
				Debug.LogError(string.Format(Strings.get_CustomActionWizard_Error_InvalidPath(), this.fullFileName));
				return;
			}
			try
			{
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
			}
			catch (Exception)
			{
				Debug.LogError(Strings.get_CustomActionWizard_Error_Could_not_create_directory() + directoryName);
				return;
			}
			using (StreamWriter streamWriter = new StreamWriter(this.fullFileName))
			{
				streamWriter.Write(this.code);
				streamWriter.Close();
			}
			AssetDatabase.Refresh();
			this.PingScriptFile();
		}
		[Localizable(false)]
		private void PingScriptFile()
		{
			Object @object = AssetDatabase.LoadMainAssetAtPath("Assets/" + this.localAssetFilename);
			EditorGUIUtility.PingObject(@object);
		}
		private void CopyCodeToClipboard()
		{
			EditorGUIUtility.set_systemCopyBuffer(this.code);
		}
		[Localizable(false)]
		private void BuildCustomAction()
		{
			this.code = "using UnityEngine;\n\nnamespace HutongGames.PlayMaker.Actions\n{\n\n";
			if (string.IsNullOrEmpty(this.customCategory))
			{
				this.code = this.code + "[ActionCategory(ActionCategory." + this.actionCategories[this.selectedCategory] + ")]\n";
			}
			else
			{
				this.code = this.code + "[ActionCategory(\"" + this.customCategory + "\")]\n";
			}
			if (!string.IsNullOrEmpty(this.tooltip))
			{
				this.code = this.code + "[Tooltip(\"" + this.tooltip + "\")]\n";
			}
			this.code = this.code + "public class " + this.actionName + " : FsmStateAction\n{\n\n";
			if (this.handlesOnEnter)
			{
				this.code += CustomActionWizard.BuildOverrideMethod("void OnEnter()", "Code that runs on entering the state.", this.HasUpdateMethod() ? "" : "Finish();");
			}
			if (this.handlesOnUpdate)
			{
				this.code += CustomActionWizard.BuildOverrideMethod("void OnUpdate()", "Code that runs every frame.", "");
			}
			if (this.handlesOnFixedUpdate)
			{
				this.code += CustomActionWizard.BuildOverrideMethod("void OnFixedUpdate()", "", "");
			}
			if (this.handlesOnLateUpdate)
			{
				this.code += CustomActionWizard.BuildOverrideMethod("void OnLateUpdate()", "", "");
			}
			if (this.handlesOnExit)
			{
				this.code += CustomActionWizard.BuildOverrideMethod("void OnExit()", "Code that runs when exiting the state.", "");
			}
			if (this.hasCustomErrorChecker)
			{
				this.code += CustomActionWizard.BuildOverrideMethod("string ErrorCheck()", "Perform custom error checking here.", "// Return an error string or null if no error.\n\nreturn null;");
			}
			this.code += "\n}\n\n}\n";
		}
		[Localizable(false)]
		private static string BuildOverrideMethod(string methodName, string comment = "", string body = "")
		{
			string text = "";
			if (!string.IsNullOrEmpty(comment))
			{
				text = text + "\t// " + comment + "\n";
			}
			text = text + "\tpublic override " + methodName + "\n\t{\n";
			text = text + "\t\t" + body.Replace("\n", "\n\t\t") + "\n";
			return text + "\t}\n\n";
		}
		private bool HasUpdateMethod()
		{
			return this.handlesOnUpdate || this.handlesOnFixedUpdate || this.handlesOnLateUpdate;
		}
	}
}
