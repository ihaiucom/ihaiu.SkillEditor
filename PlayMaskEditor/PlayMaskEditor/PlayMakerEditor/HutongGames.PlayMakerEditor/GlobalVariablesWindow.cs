using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class GlobalVariablesWindow : BaseEditorWindow
	{
		private static GlobalVariablesWindow instance;
		private FsmVariablesEditor fsmVariablesEditor;
		public void InitWindowTitle()
		{
			base.SetTitle(Strings.get_GlobalsWindow_Title());
		}
		public override void Initialize()
		{
			this.isToolWindow = true;
			GlobalVariablesWindow.instance = this;
			this.InitWindowTitle();
			base.set_minSize(new Vector2(200f, 200f));
			this.fsmVariablesEditor = new FsmVariablesEditor(this, SkillVariables.get_GlobalsComponent());
			FsmVariablesEditor expr_3F = this.fsmVariablesEditor;
			expr_3F.SettingsButtonClicked = (EditorApplication.CallbackFunction)Delegate.Combine(expr_3F.SettingsButtonClicked, new EditorApplication.CallbackFunction(this.DoSettingsMenu));
			FsmVariablesEditor expr_66 = this.fsmVariablesEditor;
			expr_66.VariableContextClicked = (FsmVariablesEditor.ContextClickVariable)Delegate.Combine(expr_66.VariableContextClicked, new FsmVariablesEditor.ContextClickVariable(GlobalVariablesWindow.DoVariableContextMenu));
		}
		public override void DoGUI()
		{
			this.fsmVariablesEditor.SetTarget(SkillVariables.get_GlobalsComponent());
			this.fsmVariablesEditor.OnGUI();
			if (GUILayout.Button(new GUIContent(Strings.get_GlobalVariablesWindow_Refresh_Used_Count_In_This_Scene(), Strings.get_GlobalVariablesWindow_Refresh_Tooltip()), new GUILayoutOption[0]))
			{
				SkillSearch.UpdateAll();
			}
			EditorGUILayout.HelpBox(Strings.get_GlobalVariablesWindow_Note_Asset_Location(), 1);
		}
		public static void ResetView()
		{
			if (GlobalVariablesWindow.instance != null)
			{
				GlobalVariablesWindow.instance.fsmVariablesEditor.Reset();
			}
		}
		public static void UpdateView()
		{
			if (GlobalVariablesWindow.instance != null)
			{
				GlobalVariablesWindow.instance.fsmVariablesEditor.UpdateView();
			}
		}
		private void DoSettingsMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Auto_Refresh_Globals()), FsmEditorSettings.AutoRefreshFsmInfo, new GenericMenu.MenuFunction(EditorCommands.ToggleAutoRefreshFsmInfo));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Debug_Variable_Values()), FsmEditorSettings.DebugVariables, new GenericMenu.MenuFunction(EditorCommands.ToggleDebugVariables));
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Online_Help()), false, new GenericMenu.MenuFunction(GlobalVariablesWindow.OpenOnlineHelp));
			genericMenu.ShowAsContext();
		}
		private static void OpenOnlineHelp()
		{
			EditorCommands.OpenWikiPage(WikiPages.VariableManager);
		}
		private static void DoVariableContextMenu(SkillVariable variable)
		{
			GenericMenu genericMenu = new GenericMenu();
			List<SkillInfo> globalVariablesUsageList = SkillSearch.GetGlobalVariablesUsageList(variable.NamedVar);
			if (globalVariablesUsageList.get_Count() == 0)
			{
				genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_No_FSMs_use_this_variable()));
			}
			else
			{
				using (List<SkillInfo>.Enumerator enumerator = globalVariablesUsageList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SkillInfo current = enumerator.get_Current();
						genericMenu.AddItem(new GUIContent(Labels.GetFullFsmLabel(current.fsm)), SkillEditor.SelectedFsm == current.fsm, new GenericMenu.MenuFunction2(EditorCommands.SelectFsm), current.fsm);
					}
				}
			}
			genericMenu.ShowAsContext();
		}
	}
}
