using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class ErrorSelector : BaseEditorWindow
	{
		[SerializeField]
		private bool filterByFsm;
		private Vector2 scrollPosition;
		private FsmError selectedError;
		public override void Initialize()
		{
			this.isToolWindow = true;
			this.InitWindowTitle();
			base.set_minSize(new Vector2(200f, 100f));
		}
		public void InitWindowTitle()
		{
			base.SetTitle(Strings.get_ErrorSelector_Title());
		}
		public override void DoGUI()
		{
			this.DoToolbar();
			this.DoErrorHierarchyGUI();
			ErrorSelector.DoBottomPanel();
		}
		private void DoToolbar()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[0]);
			if (GUILayout.Button(Strings.get_ErrorSelector_Refresh(), EditorStyles.get_toolbarButton(), new GUILayoutOption[0]))
			{
				FsmErrorChecker.Refresh();
			}
			GUILayout.FlexibleSpace();
			this.filterByFsm = GUILayout.Toggle(this.filterByFsm, new GUIContent(Strings.get_ErrorSelector_Filter_Selected_FSM_Only(), Strings.get_ErrorSelector_Filter_Selected_FSM()), EditorStyles.get_toolbarButton(), new GUILayoutOption[0]);
			GUILayout.Space(5f);
			if (SkillEditorGUILayout.ToolbarSettingsButton())
			{
				ErrorSelector.GenerateSettingsMenu().ShowAsContext();
			}
			GUILayout.Space(-5f);
			EditorGUILayout.EndHorizontal();
		}
		private void DoErrorHierarchyGUI()
		{
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.GetErrors().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (!this.filterByFsm || current.Fsm == SkillEditor.SelectedFsm)
					{
						GUIStyle gUIStyle = SkillEditorStyles.ActionItem;
						if (this.selectedError == current)
						{
							gUIStyle = SkillEditorStyles.ActionItemSelected;
						}
						SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
						if (GUILayout.Button(current.ErrorString, gUIStyle, new GUILayoutOption[0]) || GUILayout.Button(current.ToString(), gUIStyle, new GUILayoutOption[0]))
						{
							this.selectedError = current;
							ErrorSelector.GotoError(this.selectedError);
						}
					}
				}
			}
			GUILayout.EndVertical();
			EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), 4);
			GUILayout.EndScrollView();
		}
		private static void DoBottomPanel()
		{
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(string.Format(Strings.get_ErrorSelector_Setup_Errors(), FsmErrorChecker.CountSetupErrors()), new GUILayoutOption[0]);
			GUILayout.Space(20f);
			GUILayout.Label(string.Format(Strings.get_ErrorSelector_Runtime_Errors(), FsmErrorChecker.CountRuntimeErrors()), new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label(string.Format(Strings.get_ErrorSelector_Total_Errors(), FsmErrorChecker.CountAllErrors()), new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
		}
		private static void GotoError(FsmError error)
		{
			if (error == null)
			{
				return;
			}
			if (error.Fsm == null)
			{
				return;
			}
			SkillEditor.SelectFsm(error.Fsm);
			if (error.State == null)
			{
				return;
			}
			SkillEditor.SelectState(error.State, true);
			if (error.Action == null)
			{
				return;
			}
			SkillEditor.SelectAction(error.Action, true);
			SkillEditor.Repaint(true);
		}
		private static GenericMenu GenerateSettingsMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Enable_Real_Time_Error_Checker()), FsmEditorSettings.EnableRealtimeErrorChecker, new GenericMenu.MenuFunction(ErrorSelector.ToggleEnableRealtimeErrorChecker));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Disable_Error_Checker_When_Game_Is_Playing()), FsmEditorSettings.DisableErrorCheckerWhenPlaying, new GenericMenu.MenuFunction(ErrorSelector.ToggleDisableErrorCheckerWhenPlaying));
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Check_for_Required_Components()), FsmEditorSettings.CheckForRequiredComponent, new GenericMenu.MenuFunction(ErrorSelector.ToggleCheckForRequiredComponent));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Check_for_Required_Action_Fields()), FsmEditorSettings.CheckForRequiredField, new GenericMenu.MenuFunction(ErrorSelector.ToggleCheckForRequiredField));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Check_for_Events_Not_Used_by_Target_FSM()), FsmEditorSettings.CheckForEventNotUsed, new GenericMenu.MenuFunction(ErrorSelector.ToggleCheckForEventNotUsed));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Check_for_Transitions_Missing_Events()), FsmEditorSettings.CheckForTransitionMissingEvent, new GenericMenu.MenuFunction(ErrorSelector.ToggleCheckForTransitionMissingEvent));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Check_for_Transitions_Missing_Targets()), FsmEditorSettings.CheckForTransitionMissingTarget, new GenericMenu.MenuFunction(ErrorSelector.ToggleCheckForTransitionMissingTarget));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Check_for_Duplicate_Transition_Events()), FsmEditorSettings.CheckForDuplicateTransitionEvent, new GenericMenu.MenuFunction(ErrorSelector.ToggleCheckForDuplicateTransitionEvent));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Check_for_Setup_Errors_With_Mouse_Events()), FsmEditorSettings.CheckForMouseEventErrors, new GenericMenu.MenuFunction(ErrorSelector.ToggleCheckForMouseEventErrors));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Check_for_Setup_Errors_With_Collision_Events()), FsmEditorSettings.CheckForCollisionEventErrors, new GenericMenu.MenuFunction(ErrorSelector.ToggleCheckForCollisionEventErrors));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Check_for_Prefab_Restrictions()), FsmEditorSettings.CheckForPrefabRestrictions, new GenericMenu.MenuFunction(ErrorSelector.ToggleCheckForPrefabRestrictions));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Check_for_Obsolete_Actions()), FsmEditorSettings.CheckForObsoleteActions, new GenericMenu.MenuFunction(ErrorSelector.ToggleCheckForObsoleteActions));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Check_for_Missing_Actions()), FsmEditorSettings.CheckForMissingActions, new GenericMenu.MenuFunction(ErrorSelector.ToggleCheckForMissingActions));
			return genericMenu;
		}
		private static void ToggleEnableRealtimeErrorChecker()
		{
			FsmEditorSettings.EnableRealtimeErrorChecker = !FsmEditorSettings.EnableRealtimeErrorChecker;
			FsmEditorSettings.SaveSettings();
			FsmErrorChecker.Refresh();
		}
		private static void ToggleDisableErrorCheckerWhenPlaying()
		{
			FsmEditorSettings.DisableErrorCheckerWhenPlaying = !FsmEditorSettings.DisableErrorCheckerWhenPlaying;
			FsmEditorSettings.SaveSettings();
			FsmErrorChecker.Refresh();
		}
		private static void ToggleCheckForCollisionEventErrors()
		{
			FsmEditorSettings.CheckForCollisionEventErrors = !FsmEditorSettings.CheckForCollisionEventErrors;
			FsmEditorSettings.SaveSettings();
			FsmErrorChecker.Refresh();
		}
		private static void ToggleCheckForDuplicateTransitionEvent()
		{
			FsmEditorSettings.CheckForDuplicateTransitionEvent = !FsmEditorSettings.CheckForDuplicateTransitionEvent;
			FsmEditorSettings.SaveSettings();
			FsmErrorChecker.Refresh();
		}
		private static void ToggleCheckForEventNotUsed()
		{
			FsmEditorSettings.CheckForEventNotUsed = !FsmEditorSettings.CheckForEventNotUsed;
			FsmEditorSettings.SaveSettings();
			FsmErrorChecker.Refresh();
		}
		private static void ToggleCheckForMouseEventErrors()
		{
			FsmEditorSettings.CheckForMouseEventErrors = !FsmEditorSettings.CheckForMouseEventErrors;
			FsmEditorSettings.SaveSettings();
			FsmErrorChecker.Refresh();
		}
		private static void ToggleCheckForPrefabRestrictions()
		{
			FsmEditorSettings.CheckForPrefabRestrictions = !FsmEditorSettings.CheckForPrefabRestrictions;
			FsmEditorSettings.SaveSettings();
			FsmErrorChecker.Refresh();
		}
		private static void ToggleCheckForRequiredComponent()
		{
			FsmEditorSettings.CheckForRequiredComponent = !FsmEditorSettings.CheckForRequiredComponent;
			FsmEditorSettings.SaveSettings();
			FsmErrorChecker.Refresh();
		}
		private static void ToggleCheckForRequiredField()
		{
			FsmEditorSettings.CheckForRequiredField = !FsmEditorSettings.CheckForRequiredField;
			FsmEditorSettings.SaveSettings();
			FsmErrorChecker.Refresh();
		}
		private static void ToggleCheckForTransitionMissingEvent()
		{
			FsmEditorSettings.CheckForTransitionMissingEvent = !FsmEditorSettings.CheckForTransitionMissingEvent;
			FsmEditorSettings.SaveSettings();
			FsmErrorChecker.Refresh();
		}
		private static void ToggleCheckForTransitionMissingTarget()
		{
			FsmEditorSettings.CheckForTransitionMissingTarget = !FsmEditorSettings.CheckForTransitionMissingTarget;
			FsmEditorSettings.SaveSettings();
			FsmErrorChecker.Refresh();
		}
		private static void ToggleCheckForObsoleteActions()
		{
			FsmEditorSettings.CheckForObsoleteActions = !FsmEditorSettings.CheckForObsoleteActions;
			FsmEditorSettings.SaveSettings();
			FsmErrorChecker.Refresh();
		}
		private static void ToggleCheckForMissingActions()
		{
			FsmEditorSettings.CheckForMissingActions = !FsmEditorSettings.CheckForMissingActions;
			FsmEditorSettings.SaveSettings();
			FsmErrorChecker.Refresh();
		}
	}
}
