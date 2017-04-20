using HutongGames.Editor;
using HutongGames.PlayMaker;
using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	internal static class DebugToolbar
	{
		public static void OnGUI(float width)
		{
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[]
			{
				GUILayout.Width(width)
			});
			DebugToolbar.DoDebugControls();
			GUILayout.Space(10f);
			DebugToolbar.DoPlaybackControls();
			GUILayout.Space(10f);
			DebugToolbar.DoDebugFlowControls();
			GUILayout.EndHorizontal();
		}
		[Localizable(false)]
		private static void DoDebugControls()
		{
			int num = FsmErrorChecker.CountAllErrors();
			string text = Strings.get_DebugToolbar_No_errors();
			if (num > 0)
			{
				text = string.Format("{0} {1}", num, (num > 1) ? Strings.get_DebugToolbar_Label_Errors() : Strings.get_DebugToolbar_Label_Error());
			}
			SkillEditorContent.DebugToolbarErrorCount.set_text(text);
			if (GUILayout.Button(SkillEditorContent.DebugToolbarErrorCount, SkillEditorStyles.ErrorCount, new GUILayoutOption[0]))
			{
				SkillEditor.OpenErrorWindow();
				GUIUtility.ExitGUI();
			}
			if (Event.get_current().get_type() == 7)
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				lastRect.set_x(lastRect.get_x() + 4f);
				lastRect.set_y(lastRect.get_y() + 2f);
				float width;
				lastRect.set_height(width = 14f);
				lastRect.set_width(width);
				GUIHelpers.DrawTexture(lastRect, (num > 0) ? SkillEditorStyles.Errors : SkillEditorStyles.NoErrors, Color.get_white(), 0);
			}
			GUILayout.Space(10f);
			if (GUILayout.Button(SkillEditorContent.DebugToolbarDebug, EditorStyles.get_toolbarDropDown(), new GUILayoutOption[0]))
			{
				DebugToolbar.DoDebugMenu();
			}
		}
		private static void DoPlaybackControls()
		{
			Color contentColor = GUI.get_contentColor();
			GUI.set_contentColor((!SkillEditorStyles.UsingProSkin()) ? Color.get_black() : EditorStyles.get_label().get_normal().get_textColor());
			EditorGUI.BeginChangeCheck();
			bool isPlaying = GUILayout.Toggle(EditorApplication.get_isPlayingOrWillChangePlaymode(), SkillEditorContent.Play, EditorStyles.get_toolbarButton(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(40f)
			});
			if (EditorGUI.EndChangeCheck())
			{
				EditorApplication.set_isPlaying(isPlaying);
			}
			EditorGUI.BeginChangeCheck();
			bool isPaused = GUILayout.Toggle(EditorApplication.get_isPaused(), SkillEditorContent.Pause, EditorStyles.get_toolbarButton(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(40f)
			});
			if (EditorGUI.EndChangeCheck())
			{
				EditorApplication.set_isPaused(isPaused);
			}
			if (GUILayout.Button(SkillEditorContent.Step, EditorStyles.get_toolbarButton(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(40f)
			}))
			{
				FsmDebugger.Instance.Step();
				GUIUtility.ExitGUI();
			}
			GUI.set_contentColor(contentColor);
		}
		private static void DoDebugFlowControls()
		{
			if (GameStateTracker.CurrentState == GameState.Stopped)
			{
				GUILayout.FlexibleSpace();
				return;
			}
			if (DebugFlow.Active)
			{
				GUI.set_contentColor(Color.get_yellow());
				GUILayout.Label(Labels.FormatTime(DebugFlow.CurrentDebugTime), EditorStyles.get_toolbarButton(), new GUILayoutOption[0]);
				GUI.set_contentColor(Color.get_white());
				GUILayout.Space(10f);
				if (GUILayout.Button(SkillEditorContent.DebugToolbarPrev, EditorStyles.get_toolbarButton(), new GUILayoutOption[0]))
				{
					DebugFlow.SelectPrevTransition();
				}
				if (GUILayout.Button(SkillEditorContent.DebugToolbarNext, EditorStyles.get_toolbarButton(), new GUILayoutOption[0]))
				{
					DebugFlow.SelectNextTransition();
				}
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(Strings.get_DebugToolbar_Button_Open_Log(), EditorStyles.get_toolbarButton(), new GUILayoutOption[0]))
				{
					SkillEditor.OpenFsmLogWindow();
					GUIUtility.ExitGUI();
					return;
				}
			}
			else
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(Strings.get_DebugToolbar_Button_Open_Log(), EditorStyles.get_toolbarButton(), new GUILayoutOption[0]))
				{
					SkillEditor.OpenFsmLogWindow();
					GUIUtility.ExitGUI();
				}
			}
		}
		private static void DoDebugMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Enable_Breakpoints()), FsmEditorSettings.BreakpointsEnabled, new GenericMenu.MenuFunction(DebugToolbar.ToggleEnableBreakpoints));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Clear_Breakpoints()), false, new GenericMenu.MenuFunction(EditorCommands.ClearBreakpoints));
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Show_State_Labels_in_Game_View()), FsmEditorSettings.ShowStateLabelsInGameView, new GenericMenu.MenuFunction(DebugToolbar.ToggleShowStateLabels));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Show_State_Loop_Counts()), FsmEditorSettings.ShowStateLoopCounts, new GenericMenu.MenuFunction(DebugToolbar.ToggleShowStateLoopCounts));
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Step_Single_Frame()), FsmDebugger.Instance.StepMode == FsmDebugger.FsmStepMode.StepFrame, new GenericMenu.MenuFunction(DebugToolbar.SetDebuggerStepFrame));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Step_To_Next_State_Change_in_this_FSM()), FsmDebugger.Instance.StepMode == FsmDebugger.FsmStepMode.StepToStateChange, new GenericMenu.MenuFunction(DebugToolbar.SetDebuggerStepToStateChange));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Step_To_Next_State_Change_in_any_FSM()), FsmDebugger.Instance.StepMode == FsmDebugger.FsmStepMode.StepToAnyStateChange, new GenericMenu.MenuFunction(DebugToolbar.SetDebuggerStepToAnyStateChange));
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Open_Log_Window()), false, new GenericMenu.MenuFunction(SkillEditor.OpenFsmLogWindow));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Enable_Logging()), SkillLog.get_LoggingEnabled(), new GenericMenu.MenuFunction(EditorCommands.ToggleLogging));
			genericMenu.ShowAsContext();
		}
		private static void ToggleShowStateLoopCounts()
		{
			FsmEditorSettings.ShowStateLoopCounts = !FsmEditorSettings.ShowStateLoopCounts;
			FsmEditorSettings.SaveSettings();
		}
		private static void ToggleEnableBreakpoints()
		{
			FsmEditorSettings.BreakpointsEnabled = !FsmEditorSettings.BreakpointsEnabled;
			Skill.set_BreakpointsEnabled(FsmEditorSettings.BreakpointsEnabled);
			FsmEditorSettings.SaveSettings();
		}
		private static void ToggleShowStateLabels()
		{
			FsmEditorSettings.ShowStateLabelsInGameView = !FsmEditorSettings.ShowStateLabelsInGameView;
			FsmEditorSettings.SaveSettings();
		}
		private static void SaveStepMode()
		{
			FsmEditorSettings.DebuggerStepMode = FsmDebugger.Instance.StepMode;
			FsmEditorSettings.SaveSettings();
		}
		private static void SetDebuggerStepFrame()
		{
			FsmDebugger.Instance.StepMode = FsmDebugger.FsmStepMode.StepFrame;
			DebugToolbar.SaveStepMode();
		}
		private static void SetDebuggerStepToStateChange()
		{
			FsmDebugger.Instance.StepMode = FsmDebugger.FsmStepMode.StepToStateChange;
			DebugToolbar.SaveStepMode();
		}
		private static void SetDebuggerStepToAnyStateChange()
		{
			FsmDebugger.Instance.StepMode = FsmDebugger.FsmStepMode.StepToAnyStateChange;
			DebugToolbar.SaveStepMode();
		}
	}
}
