using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Serializable]
	internal static class FsmEditorSettings
	{
		private enum categories
		{
			General,
			GraphView,
			ErrorChecking,
			Debugging,
			Colors
		}
		public const string ProductUrl = "http://hutonggames.com/";
		public const string OnlineStoreUrl = "http://www.hutonggames.com/store.html";
		public const string AssetStoreUrl = "http://u3d.as/content/hutong-games-llc/playmaker/1Az";
		public const int MaxStateNameLength = 100;
		public const float GraphViewMinZoom = 0.3f;
		public const float GraphViewMaxZoom = 1f;
		private static bool settingsLoaded;
		public static string ProductName = Strings.get_ProductName();
		public static string ProductCopyright = Strings.get_ProductCopyright();
		[Localizable(false)]
		private static readonly string[] supportedCultures = new string[]
		{
			"en-US",
			"zh-CN",
			"zh-TW",
			"nl",
			"fr-FR",
			"de-DE",
			"it",
			"ja-JP",
			"pt-BR",
			"es-ES",
			"sv-SE"
		};
		[Localizable(false)]
		private static readonly string[] cultureNames = new string[]
		{
			"English",
			"Chinese Simplified",
			"Chinese Traditional",
			"Dutch",
			"French",
			"German",
			"Italian",
			"Japanese",
			"Portuguese Brazilian",
			"Spanish",
			"Swedish"
		};
		private static int selectedCulture;
		private static string selectedCultureName;
		private static string selectedCultureTranslators;
		private static bool selectedCultureSupportedInMenus;
		private static FsmEditorSettings.categories selectedCategory;
		private static bool showStateLabelsInGameView;
		private static Vector2 scrollPosition;
		public static int ActionBrowserRecentSize
		{
			get;
			set;
		}
		public static FsmDebugger.FsmStepMode DebuggerStepMode
		{
			get;
			set;
		}
		public static bool DisableUndoRedo
		{
			get;
			set;
		}
		public static bool MaximizeFileCompatibility
		{
			get;
			set;
		}
		public static bool DrawAssetThumbnail
		{
			get;
			set;
		}
		public static bool DrawLinksBehindStates
		{
			get;
			set;
		}
		public static bool DimFinishedActions
		{
			get;
			set;
		}
		public static bool AutoRefreshFsmInfo
		{
			get;
			set;
		}
		public static bool ConfirmEditingPrefabInstances
		{
			get;
			set;
		}
		public static bool DrawFrameAroundGraph
		{
			get;
			set;
		}
		public static bool GraphViewShowMinimap
		{
			get;
			set;
		}
		public static float GraphViewMinimapSize
		{
			get;
			set;
		}
		public static string ScreenshotsPath
		{
			get;
			set;
		}
		public static bool ShowStateDescription
		{
			get;
			set;
		}
		public static bool ShowActionParameters
		{
			get;
			set;
		}
		public static bool DebugActionParameters
		{
			get;
			set;
		}
		public static bool DebugVariables
		{
			get;
			set;
		}
		public static bool HideUnusedEvents
		{
			get;
			set;
		}
		public static bool ShowActionPreview
		{
			get;
			set;
		}
		public static int SelectedActionCategory
		{
			get;
			set;
		}
		public static bool SelectFSMInGameView
		{
			get;
			set;
		}
		public static Color DebugLookAtColor
		{
			get;
			set;
		}
		public static Color DebugRaycastColor
		{
			get;
			set;
		}
		public static bool HideUnusedParams
		{
			get;
			set;
		}
		public static bool AutoAddPlayMakerGUI
		{
			get;
			set;
		}
		public static bool DimUnusedActionParameters
		{
			get;
			set;
		}
		public static bool AddPrefabLabel
		{
			get;
			set;
		}
		public static bool UnloadPrefabs
		{
			get;
			set;
		}
		public static bool AutoLoadPrefabs
		{
			get;
			set;
		}
		public static int StateMaxWidth
		{
			get;
			set;
		}
		public static bool ShowScrollBars
		{
			get;
			set;
		}
		public static bool EnableWatermarks
		{
			get;
			set;
		}
		public static int SnapGridSize
		{
			get;
			set;
		}
		public static bool SnapToGrid
		{
			get;
			set;
		}
		public static bool EnableLogging
		{
			get;
			set;
		}
		public static bool ColorLinks
		{
			get;
			set;
		}
		public static bool HideObsoleteActions
		{
			get;
			set;
		}
		public static bool LockGraphView
		{
			get;
			set;
		}
		public static GraphViewLinkStyle GraphViewLinkStyle
		{
			get;
			private set;
		}
		public static string StartStateName
		{
			get;
			private set;
		}
		public static string NewStateName
		{
			get;
			private set;
		}
		public static int GameStateIconSize
		{
			get;
			private set;
		}
		public static bool AutoSelectGameObject
		{
			get;
			private set;
		}
		public static bool SelectStateOnActivated
		{
			get;
			private set;
		}
		public static bool JumpToBreakpoint
		{
			get;
			private set;
		}
		public static bool FrameSelectedState
		{
			get;
			private set;
		}
		public static bool SyncLogSelection
		{
			get;
			private set;
		}
		public static bool BreakpointsEnabled
		{
			get;
			set;
		}
		public static bool ShowFsmDescriptionInGraphView
		{
			get;
			private set;
		}
		public static bool ShowCommentsInGraphView
		{
			get;
			private set;
		}
		public static bool DrawPlaymakerGizmos
		{
			get;
			set;
		}
		public static bool DrawPlaymakerGizmoInHierarchy
		{
			get;
			set;
		}
		public static bool ShowEditWhileRunningWarning
		{
			get;
			private set;
		}
		public static bool MirrorDebugLog
		{
			get;
			private set;
		}
		public static float EdgeScrollSpeed
		{
			get;
			set;
		}
		public static float EdgeScrollZone
		{
			get;
			set;
		}
		public static int MaxLoopCount
		{
			get;
			set;
		}
		public static SkillEditorStyles.ColorScheme ColorScheme
		{
			get;
			set;
		}
		public static bool ShowStateLabelsInGameView
		{
			get
			{
				if (!PlayMakerGUI.get_Enabled())
				{
					return FsmEditorSettings.showStateLabelsInGameView;
				}
				return PlayMakerGUI.get_EnableStateLabels();
			}
			set
			{
				FsmEditorSettings.showStateLabelsInGameView = value;
				PlayMakerGUI.set_EnableStateLabels(value);
			}
		}
		public static bool EnableRealtimeErrorChecker
		{
			get;
			set;
		}
		public static bool DisableErrorCheckerWhenPlaying
		{
			get;
			set;
		}
		public static bool CheckForRequiredComponent
		{
			get;
			set;
		}
		public static bool CheckForRequiredField
		{
			get;
			set;
		}
		public static bool CheckForTransitionMissingEvent
		{
			get;
			set;
		}
		public static bool CheckForTransitionMissingTarget
		{
			get;
			set;
		}
		public static bool CheckForDuplicateTransitionEvent
		{
			get;
			set;
		}
		public static bool CheckForMouseEventErrors
		{
			get;
			set;
		}
		public static bool CheckForCollisionEventErrors
		{
			get;
			set;
		}
		public static bool CheckForEventNotUsed
		{
			get;
			set;
		}
		public static bool CheckForPrefabRestrictions
		{
			get;
			set;
		}
		public static bool CheckForObsoleteActions
		{
			get;
			set;
		}
		public static bool CheckForMissingActions
		{
			get;
			set;
		}
		public static bool CheckForNetworkSetupErrors
		{
			get;
			set;
		}
		public static bool DisableActionBrowerWhenPlaying
		{
			get;
			set;
		}
		public static bool DisableEventBrowserWhenPlaying
		{
			get;
			set;
		}
		public static bool DisableEditorWhenPlaying
		{
			get;
			set;
		}
		public static bool DisableInspectorWhenPlaying
		{
			get;
			set;
		}
		public static bool DisableToolWindowsWhenPlaying
		{
			get;
			set;
		}
		public static bool ShowHints
		{
			get;
			set;
		}
		public static bool CloseActionBrowserOnEnter
		{
			get;
			set;
		}
		public static bool AutoRefreshActionUsage
		{
			get;
			set;
		}
		public static bool LogPauseOnSelect
		{
			get;
			set;
		}
		public static bool LogShowSentBy
		{
			get;
			set;
		}
		public static bool LogShowExit
		{
			get;
			set;
		}
		public static bool LogShowTimecode
		{
			get;
			set;
		}
		public static bool EnableDebugFlow
		{
			get;
			set;
		}
		public static bool EnableTransitionEffects
		{
			get;
			set;
		}
		public static bool ShowStateLoopCounts
		{
			get;
			set;
		}
		public static int ConsoleActionReportSortOptionIndex
		{
			get;
			set;
		}
		public static bool LoadAllPrefabs
		{
			get;
			set;
		}
		public static bool SelectNewVariables
		{
			get;
			set;
		}
		public static bool FsmBrowserShowFullPath
		{
			get;
			set;
		}
		public static bool MouseWheelScrollsGraphView
		{
			get;
			set;
		}
		public static float GraphViewZoomSpeed
		{
			get;
			set;
		}
		public static void OnGUI()
		{
			FsmEditorSettings.selectedCategory = (FsmEditorSettings.categories)EditorGUILayout.EnumPopup(FsmEditorSettings.selectedCategory, new GUILayoutOption[0]);
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			FsmEditorSettings.scrollPosition = GUILayout.BeginScrollView(FsmEditorSettings.scrollPosition, new GUILayoutOption[0]);
			EditorGUIUtility.set_labelWidth(180f);
			bool changed = GUI.get_changed();
			GUI.set_changed(false);
			switch (FsmEditorSettings.selectedCategory)
			{
			case FsmEditorSettings.categories.General:
				FsmEditorSettings.DoGeneralSettings();
				break;
			case FsmEditorSettings.categories.GraphView:
				FsmEditorSettings.DoGraphViewSettings();
				break;
			case FsmEditorSettings.categories.ErrorChecking:
				FsmEditorSettings.DoErrorCheckSettings();
				break;
			case FsmEditorSettings.categories.Debugging:
				FsmEditorSettings.DoDebuggingSettings();
				break;
			case FsmEditorSettings.categories.Colors:
				FsmEditorSettings.DoColorSettings();
				break;
			}
			if (GUI.get_changed())
			{
				FsmEditorSettings.ValidateSettings();
				FsmEditorSettings.ApplySettings();
				FsmEditorSettings.SaveSettings();
			}
			else
			{
				GUI.set_changed(changed);
			}
			GUILayout.EndScrollView();
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button(Strings.get_FsmEditorSettings_Restore_Default_Settings(), new GUILayoutOption[0]))
			{
				FsmEditorSettings.ResetDefaults();
				FsmEditorSettings.SaveSettings();
			}
			if (SkillEditorGUILayout.HelpButton("Online Help"))
			{
				EditorCommands.OpenWikiPage(WikiPages.Preferences);
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
		private static void DoColorSettings()
		{
			GUILayout.Label(Strings.get_FsmEditorSettings_Default_Colors(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			for (int i = 0; i < 8; i++)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				PlayMakerPrefs.get_ColorNames()[i] = EditorGUILayout.TextField(PlayMakerPrefs.get_ColorNames()[i], new GUILayoutOption[0]);
				PlayMakerPrefs.get_Colors()[i] = EditorGUILayout.ColorField(PlayMakerPrefs.get_Colors()[i], new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
			}
			if (GUILayout.Button(Strings.get_FsmEditorSettings_Reset_Default_Colors(), new GUILayoutOption[0]))
			{
				PlayMakerPrefs.get_Instance().ResetDefaultColors();
				Keyboard.ResetFocus();
				GUI.set_changed(true);
			}
			GUILayout.Label(Strings.get_FsmEditorSettings_Custom_Colors(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			for (int j = 8; j < 24; j++)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				PlayMakerPrefs.get_ColorNames()[j] = EditorGUILayout.TextField(PlayMakerPrefs.get_ColorNames()[j], new GUILayoutOption[0]);
				PlayMakerPrefs.get_Colors()[j] = EditorGUILayout.ColorField(PlayMakerPrefs.get_Colors()[j], new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
			}
			EditorGUILayout.HelpBox(Strings.get_FsmEditorSettings_Custom_Colors_Name_Hint(), 1);
			if (GUI.get_changed())
			{
				FsmEditorSettings.SavePlayMakerPrefs();
			}
		}
		private static void SavePlayMakerPrefs()
		{
			PlayMakerPrefs.SaveChanges();
			EditorUtility.SetDirty(PlayMakerPrefs.get_Instance());
			if (!AssetDatabase.Contains(PlayMakerPrefs.get_Instance()))
			{
				string text = Path.Combine(SkillPaths.ResourcesPath, "PlayMakerPrefs.asset");
				SkillEditor.CreateAsset(PlayMakerPrefs.get_Instance(), ref text);
				Debug.Log(Strings.get_FsmEditorSettings_Creating_PlayMakerPrefs_Asset() + text);
			}
		}
		private static void DoDebuggingSettings()
		{
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_Debugger_Settings(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			FsmEditorSettings.ShowStateLabelsInGameView = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Show_State_Labels_in_Game_View(), Strings.get_FsmEditorSettings_DoDebuggingSettings_Show_State_Labels_Tooltip()), FsmEditorSettings.ShowStateLabelsInGameView);
			FsmEditorSettings.EnableLogging = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Enable_Logging(), Strings.get_FsmEditorSettings_Enable_Logging_Tooltip()), FsmEditorSettings.EnableLogging);
			FsmEditorSettings.EnableDebugFlow = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Enable_DebugFlow(), Strings.get_FsmEditorSettings_Enable_DebugFlow_Tooltip()), FsmEditorSettings.EnableDebugFlow);
			FsmEditorSettings.EnableTransitionEffects = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Enable_Transition_Effects(), Strings.get_FsmEditorSettings_Enable_Transition_Effects_Tooltip()), FsmEditorSettings.EnableTransitionEffects);
			FsmEditorSettings.JumpToBreakpoint = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Jump_to_Breakpoint_Error(), Strings.get_FsmEditorSettings_Jump_to_Breakpoint_Error_Tooltip()), FsmEditorSettings.JumpToBreakpoint);
			FsmEditorSettings.MirrorDebugLog = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Forward_Playmaker_Log_to_Unity_Log(), Strings.get_FsmEditorSettings_Forward_Playmaker_Log_to_Unity_Log_Tooltip()), FsmEditorSettings.MirrorDebugLog);
			FsmEditorSettings.DebugLookAtColor = EditorGUILayout.ColorField(Strings.get_FsmEditorSettings_Debug_Look_At_Color(), FsmEditorSettings.DebugLookAtColor, new GUILayoutOption[0]);
			FsmEditorSettings.DebugRaycastColor = EditorGUILayout.ColorField(Strings.get_FsmEditorSettings_Debug_Raypick_Color(), FsmEditorSettings.DebugRaycastColor, new GUILayoutOption[0]);
		}
		private static void DoErrorCheckSettings()
		{
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_Error_Checker_Settings(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			bool changed = GUI.get_changed();
			FsmEditorSettings.EnableRealtimeErrorChecker = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Enable_Real_Time_Error_Checker(), Strings.get_FsmEditorSettings_Enable_Real_Time_Error_Checker_Tooltip()), FsmEditorSettings.EnableRealtimeErrorChecker);
			FsmEditorSettings.DisableErrorCheckerWhenPlaying = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Disable_Error_Checker_When_Game_Is_Playing(), Strings.get_FsmEditorSettings_Disable_Error_Checker_When_Game_Is_Playing_Tooltip()), FsmEditorSettings.DisableErrorCheckerWhenPlaying);
			FsmEditorSettings.CheckForRequiredComponent = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Check_for_Required_Components(), Strings.get_FsmEditorSettings_Check_for_Required_Components_Tooltip()), FsmEditorSettings.CheckForRequiredComponent);
			FsmEditorSettings.CheckForRequiredField = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Check_for_Required_Action_Fields(), Strings.get_FsmEditorSettings_Check_for_Required_Action_Fields_Tooltip()), FsmEditorSettings.CheckForRequiredField);
			FsmEditorSettings.CheckForEventNotUsed = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Check_for_Events_Not_Used_by_Target_FSM(), Strings.get_FsmEditorSettings_Check_for_Events_Not_Used_by_Target_FSM_Tooltip()), FsmEditorSettings.CheckForEventNotUsed);
			FsmEditorSettings.CheckForTransitionMissingEvent = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Check_for_Transitions_Missing_Events(), Strings.get_FsmEditorSettings_Check_for_Transitions_Missing_Events_Tooltip()), FsmEditorSettings.CheckForTransitionMissingEvent);
			FsmEditorSettings.CheckForTransitionMissingTarget = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Check_for_Transitions_Missing_Targets(), Strings.get_FsmEditorSettings_Check_for_Transitions_Missing_Targets_Tooltip()), FsmEditorSettings.CheckForTransitionMissingTarget);
			FsmEditorSettings.CheckForDuplicateTransitionEvent = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Check_for_Duplicate_Transition_Events(), Strings.get_FsmEditorSettings_Check_for_Duplicate_Transition_Events_Tooltip()), FsmEditorSettings.CheckForDuplicateTransitionEvent);
			FsmEditorSettings.CheckForMouseEventErrors = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Check_for_Setup_Errors_With_Mouse_Events(), Strings.get_FsmEditorSettings_Check_for_Setup_Errors_With_Mouse_Events_Tooltip()), FsmEditorSettings.CheckForMouseEventErrors);
			FsmEditorSettings.CheckForCollisionEventErrors = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Check_for_Setup_Errors_With_Collision_Events(), Strings.get_FsmEditorSettings_Check_for_Setup_Errors_With_Collision_Events_Tooltip()), FsmEditorSettings.CheckForCollisionEventErrors);
			FsmEditorSettings.CheckForPrefabRestrictions = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Check_for_Prefab_Restrictions(), Strings.get_FsmEditorSettings_Check_for_Prefab_Restrictions_Tooltip()), FsmEditorSettings.CheckForPrefabRestrictions);
			FsmEditorSettings.CheckForObsoleteActions = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Check_for_Obsolete_Actions(), Strings.get_FsmEditorSettings_Check_for_Obsolete_Actions_Tooltip()), FsmEditorSettings.CheckForObsoleteActions);
			FsmEditorSettings.CheckForMissingActions = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Check_for_Missing_Actions(), Strings.get_FsmEditorSettings_Check_for_Missing_Actions_Tooltip()), FsmEditorSettings.CheckForMissingActions);
			FsmEditorSettings.CheckForNetworkSetupErrors = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Check_for_Network_Setup_Errors(), Strings.get_FsmEditorSettings_Check_for_Network_Setup_Errors_Tooltip()), FsmEditorSettings.CheckForNetworkSetupErrors);
			if (GUI.get_changed())
			{
				FsmErrorChecker.Refresh();
				SkillEditor.RepaintAll();
				return;
			}
			GUI.set_changed(changed);
		}
		private static void DoGeneralSettings()
		{
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_FsmEditorHint_General_Settings(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			int num = EditorGUILayout.Popup(Strings.get_Label_Language(), FsmEditorSettings.selectedCulture, FsmEditorSettings.cultureNames, new GUILayoutOption[0]);
			if (num != FsmEditorSettings.selectedCulture)
			{
				FsmEditorSettings.SetCulture(num);
			}
			if (FsmEditorSettings.selectedCultureTranslators != "")
			{
				EditorGUILayout.HelpBox(Strings.get_FsmEditorSettings_Translators() + FsmEditorSettings.selectedCultureTranslators, 0);
			}
			if (!FsmEditorSettings.selectedCultureSupportedInMenus)
			{
				EditorGUILayout.HelpBox(Strings.get_FsmEditorSettings_Selected_language_not_yet_supported_in_menus(), 0);
			}
			GUILayout.Label(Strings.get_FsmEditorSettings_Category_Components_and_Gizmos(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			FsmEditorSettings.AutoAddPlayMakerGUI = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Auto_Add_PlayMakerGUI_to_Scene(), Strings.get_FsmEditorSettings_Auto_Add_PlayMakerGUI_to_Scene_Tooltip()), FsmEditorSettings.AutoAddPlayMakerGUI);
			FsmEditorSettings.ShowStateLabelsInGameView = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Show_State_Labels_in_Game_View(), Strings.get_FsmEditorSettings_DoDebuggingSettings_Show_State_Labels_Tooltip()), FsmEditorSettings.ShowStateLabelsInGameView);
			bool flag = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Draw_Playmaker_Gizmos_in_Scene_View(), Strings.get_FsmEditorSettings_Draw_Playmaker_Gizmos_in_Scene_View_Tooltip()), FsmEditorSettings.DrawPlaymakerGizmos);
			if (flag != FsmEditorSettings.DrawPlaymakerGizmos)
			{
				FsmEditorSettings.DrawPlaymakerGizmos = flag;
				PlayMakerFSM.set_DrawGizmos(flag);
				GUI.set_changed(true);
			}
			flag = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Draw_Playmaker_Gizmos_in_Hierarchy(), Strings.get_FsmEditorSettings_Draw_Playmaker_Gizmos_in_Hierarchy_Tooltip()), FsmEditorSettings.DrawPlaymakerGizmoInHierarchy);
			if (flag != FsmEditorSettings.DrawPlaymakerGizmoInHierarchy)
			{
				Gizmos.EnableHierarchyItemGizmos = flag;
				FsmEditorSettings.DrawPlaymakerGizmoInHierarchy = flag;
				EditorApplication.RepaintHierarchyWindow();
			}
			GUILayout.Label(Strings.get_FsmEditorSettings_Category_When_Game_Is_Playing(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			FsmEditorSettings.ShowEditWhileRunningWarning = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Show_Editing_While_Running_Warning(), Strings.get_FsmEditorSettings_Show_Editing_While_Running_Warning_Tooltip()), FsmEditorSettings.ShowEditWhileRunningWarning);
			FsmEditorSettings.DisableEditorWhenPlaying = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Disable_PlayMaker_Editor_When_Game_Is_Playing(), Strings.get_FsmEditorSettings_Disable_PlayMaker_Editor_When_Game_Is_Playing_Tooltip()), FsmEditorSettings.DisableEditorWhenPlaying);
			FsmEditorSettings.DisableInspectorWhenPlaying = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Disable_the_Inspector_Panel_When_Game_Is_Playing(), Strings.get_FsmEditorSettings_Disable_the_Inspector_Panel_When_Game_Is_Playing_Tooltip()), FsmEditorSettings.DisableInspectorWhenPlaying);
			FsmEditorSettings.DisableToolWindowsWhenPlaying = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Disable_Tool_Windows_When_Game_Is_Playing(), Strings.get_FsmEditorSettings_Disable_Tool_Windows_When_Game_Is_Playing_Tooltip()), FsmEditorSettings.DisableToolWindowsWhenPlaying);
			FsmEditorSettings.DisableErrorCheckerWhenPlaying = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Disable_Error_Checker_When_Game_Is_Playing(), Strings.get_FsmEditorSettings_Disable_Error_Checker_When_Game_Is_Playing_Tooltip()), FsmEditorSettings.DisableErrorCheckerWhenPlaying);
			FsmEditorSettings.DimFinishedActions = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Dim_Finished_Actions(), Strings.get_FsmEditorSettings_Dim_Finished_Actions_Tooltip()), FsmEditorSettings.DimFinishedActions);
			GUILayout.Label(Strings.get_FsmEditorSettings_Category_Selection(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			FsmEditorSettings.AutoSelectGameObject = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Select_GameObject_When_FSM_Selected(), Strings.get_FsmEditorSettings_Select_GameObject_When_FSM_Selected_Tooltip()), FsmEditorSettings.AutoSelectGameObject);
			FsmEditorSettings.SelectStateOnActivated = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Select_State_On_Activated(), Strings.get_FsmEditorSettings_Select_State_On_Activated_Tooltip()), FsmEditorSettings.SelectStateOnActivated);
			FsmEditorSettings.FrameSelectedState = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Auto_Frame_Selected_State(), Strings.get_FsmEditorSettings_Auto_Frame_Selected_State_Tooltip()), FsmEditorSettings.FrameSelectedState);
			FsmEditorSettings.SelectFSMInGameView = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Select_Game_Objects_With_FSMs_in_Game_View(), Strings.get_FsmEditorSettings_Select_Game_Objects_With_FSMs_in_Game_View_Tooltip()), FsmEditorSettings.SelectFSMInGameView);
			GUILayout.Label(Strings.get_FsmEditorSettings_Category_Prefabs(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			FsmEditorSettings.ConfirmEditingPrefabInstances = SkillEditorGUILayout.RightAlignedToggle(SkillEditorContent.ConfirmEditPrefabInstance, FsmEditorSettings.ConfirmEditingPrefabInstances);
			FsmEditorSettings.LoadAllPrefabs = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Load_All_PlayMakerFSM_Prefabs_When_Refactoring(), Strings.get_FsmEditorSettings_Load_All_PlayMakerFSM_Prefabs_When_Refactoring_Tooltip()), FsmEditorSettings.LoadAllPrefabs);
			FsmEditorSettings.AutoLoadPrefabs = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Auto_Load_Prefabs_in_Scene(), Strings.get_FsmEditorSettings_Auto_Load_Prefabs_in_Scene_Tooltip()), FsmEditorSettings.AutoLoadPrefabs);
			FsmEditorSettings.AddPrefabLabel = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Add_Prefab_Labels(), Strings.get_FsmEditorSettings_Add_Prefab_Labels_Tooltip()), FsmEditorSettings.AddPrefabLabel);
			GUILayout.Label(Strings.get_FsmEditorSettings_Cetegory_Paths(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			GUILayout.Label(new GUIContent(Strings.get_FsmEditorSettings_FSM_Screenshots_Directory(), Strings.get_FsmEditorSettings_FSM_Screenshots_Directory_Tooltip()), new GUILayoutOption[0]);
			FsmEditorSettings.ScreenshotsPath = EditorGUILayout.TextField(FsmEditorSettings.ScreenshotsPath, new GUILayoutOption[0]);
			GUILayout.Label(Strings.get_FsmEditorSettings_Experimental(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			FsmEditorSettings.DisableUndoRedo = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Disable_Undo_Redo(), Strings.get_FsmEditorSettings_Disable_Undo_Redo_Tooltip()), FsmEditorSettings.DisableUndoRedo);
		}
		[Localizable(false)]
		private static void SetCulture(int cultureIndex)
		{
			if (cultureIndex >= FsmEditorSettings.cultureNames.Length)
			{
				cultureIndex = 0;
			}
			FsmEditorSettings.selectedCulture = cultureIndex;
			FsmEditorSettings.selectedCultureName = FsmEditorSettings.cultureNames[cultureIndex];
			FsmEditorSettings.selectedCultureTranslators = FsmEditorSettings.GetTranslators(FsmEditorSettings.selectedCultureName);
			FsmEditorSettings.selectedCultureSupportedInMenus = true;
			FsmEditorSettings.selectedCultureName == "Japanese";
			Strings.set_Culture(new CultureInfo(FsmEditorSettings.supportedCultures[cultureIndex]));
			InspectorPanel.Init();
			BugReportWindow.frequencyChoices = new string[]
			{
				Strings.get_BugReportWindow_FrequencyChoices_Always(),
				Strings.get_BugReportWindow_FrequencyChoices_Sometimes__but_not_always(),
				Strings.get_BugReportWindow_FrequencyChoices_This_is_the_first_time()
			};
			SkillEditorStyles.Reinitialize();
			SkillEditor.RepaintAll();
			FsmEditorSettings.SaveSettings();
			SkillEditor.ChangeLanguage();
		}
		[Localizable(false)]
		private static string GetTranslators(string cultureName)
		{
			if (cultureName != null)
			{
				if (<PrivateImplementationDetails>{9F518054-9A7A-4388-8A0B-9CF872B8F518}.$$method0x6000671-1 == null)
				{
					Dictionary<string, int> expr_1B = new Dictionary<string, int>(11);
					expr_1B.Add("English", 0);
					expr_1B.Add("Chinese Simplified", 1);
					expr_1B.Add("Chinese Traditional", 2);
					expr_1B.Add("Dutch", 3);
					expr_1B.Add("French", 4);
					expr_1B.Add("German", 5);
					expr_1B.Add("Italian", 6);
					expr_1B.Add("Japanese", 7);
					expr_1B.Add("Spanish", 8);
					expr_1B.Add("Swedish", 9);
					expr_1B.Add("Portuguese Brazilian", 10);
					<PrivateImplementationDetails>{9F518054-9A7A-4388-8A0B-9CF872B8F518}.$$method0x6000671-1 = expr_1B;
				}
				int num;
				if (<PrivateImplementationDetails>{9F518054-9A7A-4388-8A0B-9CF872B8F518}.$$method0x6000671-1.TryGetValue(cultureName, ref num))
				{
					switch (num)
					{
					case 0:
						return "";
					case 1:
						return "黄峻";
					case 2:
						return "黄峻";
					case 3:
						return "VisionaiR3D";
					case 4:
						return "Jean Fabre";
					case 5:
						return "Steven 'Nightreaver' Barthen, Marc 'Dreamora' Schaerer";
					case 6:
						return "Marcello Gavioli";
					case 7:
						return "gamesonytablet";
					case 8:
						return "Eugenio 'Ryo567' Martínez";
					case 9:
						return "Damiangto";
					case 10:
						return "Nilton Felicio, Andre Dantas Lima";
					}
				}
			}
			return "";
		}
		private static void DoGraphViewSettings()
		{
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_Graph_View_Settings(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			GUILayout.Label(Strings.get_FsmEditorSettings_Category_Graph_Styles(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			SkillEditorStyles.ColorScheme colorScheme = (SkillEditorStyles.ColorScheme)EditorGUILayout.EnumPopup(new GUIContent(Strings.get_FsmEditorSettings_Color_Scheme(), Strings.get_FsmEditorSettings_Color_Scheme_Tooltip()), FsmEditorSettings.ColorScheme, new GUILayoutOption[0]);
			if (colorScheme != FsmEditorSettings.ColorScheme)
			{
				FsmEditorSettings.ColorScheme = colorScheme;
				SkillEditorStyles.Init();
			}
			FsmEditorSettings.GraphViewLinkStyle = (GraphViewLinkStyle)EditorGUILayout.EnumPopup(new GUIContent(Strings.get_FsmEditorSettings_Link_Style(), Strings.get_FsmEditorSettings_Link_Style_Tooltip()), FsmEditorSettings.GraphViewLinkStyle, new GUILayoutOption[0]);
			FsmEditorSettings.ColorLinks = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Color_Links_With_State_Color(), Strings.get_FsmEditorSettings_Color_Links_With_State_Color_Tooltip()), FsmEditorSettings.ColorLinks);
			FsmEditorSettings.EnableWatermarks = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Enable_Watermarks(), Strings.get_FsmEditorSettings_Enable_Watermarks_Tooltip()), FsmEditorSettings.EnableWatermarks);
			FsmEditorSettings.ShowFsmDescriptionInGraphView = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Show_FSM_Description_in_Graph_View(), ""), FsmEditorSettings.ShowFsmDescriptionInGraphView);
			bool flag = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Show_Comments_in_Graph_View(), Strings.get_FsmEditorSettings_Show_Comments_in_Graph_View_Tooltip()), FsmEditorSettings.ShowCommentsInGraphView);
			if (flag != FsmEditorSettings.ShowCommentsInGraphView)
			{
				FsmEditorSettings.ShowCommentsInGraphView = flag;
				SkillEditor.GraphView.UpdateAllStateSizes();
				GUI.set_changed(true);
			}
			FsmEditorSettings.DrawFrameAroundGraph = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Draw_Frame_Around_FSM()), FsmEditorSettings.DrawFrameAroundGraph);
			FsmEditorSettings.DrawLinksBehindStates = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Draw_Links_Behind_States()), FsmEditorSettings.DrawLinksBehindStates);
			GUILayout.Label(Strings.get_FsmEditorSettings_Scrolling(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			FsmEditorSettings.MouseWheelScrollsGraphView = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Mouse_Wheel_Scrolls_Graph_View(), Strings.get_FsmEditorSettings_DoGraphViewSettings_Tooltip()), FsmEditorSettings.MouseWheelScrollsGraphView);
			FsmEditorSettings.ShowScrollBars = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Show_Scrollbars_All_The_Time(), Strings.get_FsmEditorSettings_Show_Scrollbars_All_The_Time_Tooltip()), FsmEditorSettings.ShowScrollBars);
			FsmEditorSettings.EdgeScrollSpeed = EditorGUILayout.FloatField(new GUIContent(Strings.get_FsmEditorSettings_Edge_Scroll_Speed(), Strings.get_FsmEditorSettings_Edge_Scroll_Speed_Tooltip()), FsmEditorSettings.EdgeScrollSpeed, new GUILayoutOption[0]);
			FsmEditorSettings.GraphViewZoomSpeed = EditorGUILayout.FloatField(new GUIContent(Strings.get_FsmEditorSettings_Zoom_Speed(), Strings.get_FsmEditorSettings_DoGraphViewSettings_Zoom_Speed_Tooltip()), FsmEditorSettings.GraphViewZoomSpeed, new GUILayoutOption[0]);
			GUILayout.Label(Strings.get_FsmEditorSettings_Minimap(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			FsmEditorSettings.GraphViewShowMinimap = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Show_Minimap()), FsmEditorSettings.GraphViewShowMinimap);
			FsmEditorSettings.GraphViewMinimapSize = EditorGUILayout.FloatField(new GUIContent(Strings.get_FsmEditorSettings_Minimap_Size(), Strings.get_FsmEditorSettings_Minimap_Size_Tooltip()), FsmEditorSettings.GraphViewMinimapSize, new GUILayoutOption[0]);
			FsmEditorSettings.GraphViewMinimapSize = Mathf.Clamp(FsmEditorSettings.GraphViewMinimapSize, 50f, 1000f);
			GUILayout.Label(Strings.get_FsmEditorSettings_Category_Editing(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			FsmEditorSettings.NewStateName = EditorGUILayout.TextField(new GUIContent(Strings.get_FsmEditorSettings_New_State_Name(), Strings.get_FsmEditorSettings_New_State_Name_Tooltip()), FsmEditorSettings.NewStateName, new GUILayoutOption[0]);
			int num = EditorGUILayout.IntField(new GUIContent(Strings.get_FsmEditorSettings_Max_State_Width(), Strings.get_FsmEditorSettings_Max_State_Width_Tooltip()), FsmEditorSettings.StateMaxWidth, new GUILayoutOption[0]);
			if (num != FsmEditorSettings.StateMaxWidth)
			{
				FsmEditorSettings.StateMaxWidth = num;
				SkillEditor.GraphView.UpdateAllStateSizes();
			}
			FsmEditorSettings.SnapGridSize = EditorGUILayout.IntField(new GUIContent(Strings.get_FsmEditorSettings_Snap_Grid_Size(), Strings.get_FsmEditorSettings_Snap_Grid_Size_Tooltip()), FsmEditorSettings.SnapGridSize, new GUILayoutOption[0]);
			FsmEditorSettings.SnapToGrid = SkillEditorGUILayout.RightAlignedToggle(new GUIContent(Strings.get_FsmEditorSettings_Snap_To_Grid()), FsmEditorSettings.SnapToGrid);
		}
		private static void ResetDefaults()
		{
			FsmEditorSettings.DisableUndoRedo = false;
			FsmEditorSettings.MaximizeFileCompatibility = true;
			FsmEditorSettings.DrawAssetThumbnail = true;
			FsmEditorSettings.DrawLinksBehindStates = true;
			FsmEditorSettings.DimFinishedActions = true;
			FsmEditorSettings.ConfirmEditingPrefabInstances = true;
			FsmEditorSettings.ShowStateLoopCounts = false;
			FsmEditorSettings.GraphViewZoomSpeed = 0.01f;
			FsmEditorSettings.MouseWheelScrollsGraphView = false;
			FsmEditorSettings.GraphViewLinkStyle = GraphViewLinkStyle.BezierLinks;
			FsmEditorSettings.NewStateName = Strings.get_FsmEditorSettings_Default_State_Name();
			FsmEditorSettings.AutoSelectGameObject = true;
			FsmEditorSettings.SelectStateOnActivated = true;
			FsmEditorSettings.JumpToBreakpoint = true;
			FsmEditorSettings.MirrorDebugLog = false;
			FsmEditorSettings.ShowEditWhileRunningWarning = true;
			FsmEditorSettings.SelectFSMInGameView = true;
			FsmEditorSettings.ShowFsmDescriptionInGraphView = true;
			FsmEditorSettings.ShowCommentsInGraphView = true;
			FsmEditorSettings.DrawPlaymakerGizmos = true;
			FsmEditorSettings.DrawPlaymakerGizmoInHierarchy = true;
			FsmEditorSettings.AutoAddPlayMakerGUI = true;
			FsmEditorSettings.DimUnusedActionParameters = false;
			FsmEditorSettings.DebugLookAtColor = Color.get_yellow();
			FsmEditorSettings.DebugRaycastColor = Color.get_red();
			FsmEditorSettings.ShowHints = true;
			FsmEditorSettings.CloseActionBrowserOnEnter = false;
			FsmEditorSettings.SelectNewVariables = true;
			FsmEditorSettings.AddPrefabLabel = true;
			FsmEditorSettings.LoadAllPrefabs = true;
			FsmEditorSettings.AutoLoadPrefabs = true;
			FsmEditorSettings.UnloadPrefabs = true;
			FsmEditorSettings.StateMaxWidth = 400;
			FsmEditorSettings.ShowScrollBars = true;
			FsmEditorSettings.EnableWatermarks = true;
			FsmEditorSettings.ColorLinks = false;
			FsmEditorSettings.SnapGridSize = 16;
			FsmEditorSettings.SnapToGrid = false;
			FsmEditorSettings.EnableLogging = true;
			FsmEditorSettings.HideUnusedEvents = false;
			FsmEditorSettings.EdgeScrollSpeed = 1f;
			FsmEditorSettings.EdgeScrollZone = 100f;
			FsmEditorSettings.EnableRealtimeErrorChecker = true;
			FsmEditorSettings.DisableErrorCheckerWhenPlaying = true;
			FsmEditorSettings.CheckForRequiredComponent = true;
			FsmEditorSettings.CheckForRequiredField = true;
			FsmEditorSettings.CheckForEventNotUsed = true;
			FsmEditorSettings.CheckForTransitionMissingEvent = true;
			FsmEditorSettings.CheckForTransitionMissingTarget = true;
			FsmEditorSettings.CheckForDuplicateTransitionEvent = true;
			FsmEditorSettings.CheckForMouseEventErrors = true;
			FsmEditorSettings.CheckForCollisionEventErrors = true;
			FsmEditorSettings.CheckForPrefabRestrictions = true;
			FsmEditorSettings.CheckForObsoleteActions = true;
			FsmEditorSettings.CheckForMissingActions = true;
			FsmEditorSettings.CheckForNetworkSetupErrors = true;
			FsmEditorSettings.DisableEditorWhenPlaying = false;
			FsmEditorSettings.DisableInspectorWhenPlaying = false;
			FsmEditorSettings.DisableToolWindowsWhenPlaying = true;
			FsmEditorSettings.DisableActionBrowerWhenPlaying = false;
			FsmEditorSettings.DisableEventBrowserWhenPlaying = false;
			FsmEditorSettings.ScreenshotsPath = Strings.get_FsmEditorSettings_Default_Screenshots_Path();
			FsmEditorSettings.EnableDebugFlow = true;
			FsmEditorSettings.ApplySettings();
		}
		[Localizable(false)]
		public static void SaveSettings()
		{
			if (!FsmEditorSettings.settingsLoaded)
			{
				Debug.LogWarning("PlayMaker: Attempting to SaveSettings before LoadSettings.");
				return;
			}
			FsmEditorSettings.ValidateSettings();
			EditorPrefs.SetInt("PlayMaker.ActionBrowserRecentSize", FsmEditorSettings.ActionBrowserRecentSize);
			EditorPrefs.SetInt("PlayMaker.DebuggerStepMode", (int)FsmEditorSettings.DebuggerStepMode);
			EditorPrefs.SetBool("PlayMaker.DisableUndoRedo", FsmEditorSettings.DisableUndoRedo);
			EditorPrefs.SetBool("PlayMaker.MaximizeFileCompatibility", FsmEditorSettings.MaximizeFileCompatibility);
			EditorPrefs.SetBool("PlayMaker.DrawAssetThumbnail", FsmEditorSettings.DrawAssetThumbnail);
			EditorPrefs.SetBool("PlayMaker.DrawLinksBehindStates", FsmEditorSettings.DrawLinksBehindStates);
			EditorPrefs.SetBool("PlayMaker.DimFinishedActions", FsmEditorSettings.DimFinishedActions);
			EditorPrefs.SetBool("PlayMaker.AutoRefreshFsmInfo", FsmEditorSettings.AutoRefreshFsmInfo);
			EditorPrefs.SetBool("PlayMaker.ConfirmEditingPrefabInstances", FsmEditorSettings.ConfirmEditingPrefabInstances);
			EditorPrefs.SetBool("PlayMaker.ShowStateLoopCounts", FsmEditorSettings.ShowStateLoopCounts);
			EditorPrefs.SetBool("PlayMaker.DrawFrameAroundGraph", FsmEditorSettings.DrawFrameAroundGraph);
			EditorPrefs.SetBool("PlayMaker.GraphViewShowMinimap", FsmEditorSettings.GraphViewShowMinimap);
			EditorPrefs.SetFloat("PlayMaker.GraphViewMinimapSize", FsmEditorSettings.GraphViewMinimapSize);
			EditorPrefs.SetFloat("PlayMaker.GraphViewZoomSpeed", FsmEditorSettings.GraphViewZoomSpeed);
			EditorPrefs.SetBool("PlayMaker.MouseWheelScrollsGraphView", FsmEditorSettings.MouseWheelScrollsGraphView);
			EditorPrefs.SetString("PlayMaker.Language", FsmEditorSettings.supportedCultures[FsmEditorSettings.selectedCulture]);
			EditorPrefs.SetString("PlayMaker.ScreenshotsPath", FsmEditorSettings.ScreenshotsPath);
			EditorPrefs.SetString("PlayMaker.StartStateName", FsmEditorSettings.StartStateName);
			EditorPrefs.SetString("PlayMaker.NewStateName", FsmEditorSettings.NewStateName);
			EditorPrefs.SetBool("PlayMaker.AutoSelectGameObject", FsmEditorSettings.AutoSelectGameObject);
			EditorPrefs.SetBool("PlayMaker.SelectStateOnActivated", FsmEditorSettings.SelectStateOnActivated);
			EditorPrefs.SetBool("PlayMaker.GotoBreakpoint", FsmEditorSettings.JumpToBreakpoint);
			EditorPrefs.SetInt("PlayMaker.GameStateIconSize", FsmEditorSettings.GameStateIconSize);
			EditorPrefs.SetBool("PlayMaker.FrameSelectedState", FsmEditorSettings.FrameSelectedState);
			EditorPrefs.SetBool("PlayMaker.SyncLogSelection", FsmEditorSettings.SyncLogSelection);
			EditorPrefs.SetBool("PlayMaker.BreakpointsEnabled", FsmEditorSettings.BreakpointsEnabled);
			EditorPrefs.SetBool("PlayMaker.MirrorDebugLog", FsmEditorSettings.MirrorDebugLog);
			EditorPrefs.SetBool("PlayMaker.LockGraphView", FsmEditorSettings.LockGraphView);
			EditorPrefs.SetInt("PlayMaker.GraphLinkStyle", (int)FsmEditorSettings.GraphViewLinkStyle);
			EditorPrefs.SetBool("PlayMaker.ShowFsmDescriptionInGraphView", FsmEditorSettings.ShowFsmDescriptionInGraphView);
			EditorPrefs.SetBool("PlayMaker.ShowCommentsInGraphView", FsmEditorSettings.ShowCommentsInGraphView);
			EditorPrefs.SetBool("PlayMaker.ShowStateLabelsInGameView", FsmEditorSettings.ShowStateLabelsInGameView);
			EditorPrefs.SetBool("PlayMaker.ShowStateDescription", FsmEditorSettings.ShowStateDescription);
			EditorPrefs.SetBool("PlayMaker.ShowActionParameters", FsmEditorSettings.ShowActionParameters);
			EditorPrefs.SetBool("PlayMaker.DebugActionParameters", FsmEditorSettings.DebugActionParameters);
			EditorPrefs.SetBool("PlayMaker.DrawPlaymakerGizmos", FsmEditorSettings.DrawPlaymakerGizmos);
			EditorPrefs.SetBool("PlayMaker.DrawPlaymakerGizmoInHierarchy", FsmEditorSettings.DrawPlaymakerGizmoInHierarchy);
			EditorPrefs.SetBool("PlayMaker.ShowEditWhileRunningWarning", FsmEditorSettings.ShowEditWhileRunningWarning);
			EditorPrefs.SetBool("PlayMaker.HideUnusedEvents", FsmEditorSettings.HideUnusedEvents);
			EditorPrefs.SetBool("PlayMaker.ShowActionPreview", FsmEditorSettings.ShowActionPreview);
			EditorPrefs.SetInt("PlayMaker.SelectedActionCategory", FsmEditorSettings.SelectedActionCategory);
			EditorPrefs.SetBool("PlayMaker.SelectFSMInGameView", FsmEditorSettings.SelectFSMInGameView);
			EditorPrefs.SetInt("PlayMaker.DebugLookAtColor", FsmEditorSettings.PackColorIntoInt(FsmEditorSettings.DebugLookAtColor));
			EditorPrefs.SetInt("PlayMaker.DebugRaycastColor", FsmEditorSettings.PackColorIntoInt(FsmEditorSettings.DebugRaycastColor));
			EditorPrefs.SetBool("PlayMaker.HideUnusedParams", FsmEditorSettings.HideUnusedParams);
			EditorPrefs.SetBool("PlayMaker.EnableRealtimeErrorChecker", FsmEditorSettings.EnableRealtimeErrorChecker);
			EditorPrefs.SetBool("PlayMaker.AutoAddPlayMakerGUI", FsmEditorSettings.AutoAddPlayMakerGUI);
			EditorPrefs.SetBool("PlayMaker.DimUnusedParameters", FsmEditorSettings.DimUnusedActionParameters);
			EditorPrefs.SetBool("PlayMaker.AddPrefabLabel", FsmEditorSettings.AddPrefabLabel);
			EditorPrefs.SetBool("PlayMaker.AutoLoadPrefabs", FsmEditorSettings.AutoLoadPrefabs);
			EditorPrefs.SetBool("PlayMaker.LoadAllPrefabs", FsmEditorSettings.LoadAllPrefabs);
			EditorPrefs.SetBool("PlayMaker.UnloadPrefabs", FsmEditorSettings.UnloadPrefabs);
			EditorPrefs.SetInt("PlayMaker.StateMaxWidth", FsmEditorSettings.StateMaxWidth);
			EditorPrefs.SetBool("PlayMaker.ShowScrollBars", FsmEditorSettings.ShowScrollBars);
			EditorPrefs.SetBool("PlayMaker.ShowWatermark", FsmEditorSettings.EnableWatermarks);
			EditorPrefs.SetInt("PlayMaker.SnapGridSize", FsmEditorSettings.SnapGridSize);
			EditorPrefs.SetBool("PlayMaker.SnapToGrid", FsmEditorSettings.SnapToGrid);
			EditorPrefs.SetBool("PlayMaker.EnableLogging", FsmEditorSettings.EnableLogging);
			EditorPrefs.SetBool("PlayMaker.DisableErrorCheckerWhenPlaying", FsmEditorSettings.DisableErrorCheckerWhenPlaying);
			EditorPrefs.SetBool("PlayMaker.ColorLinks", FsmEditorSettings.ColorLinks);
			EditorPrefs.SetBool("PlayMaker.HideObsoleteActions", FsmEditorSettings.HideObsoleteActions);
			EditorPrefs.SetFloat("PlayMaker.EdgeScrollSpeed", FsmEditorSettings.EdgeScrollSpeed);
			EditorPrefs.SetFloat("PlayMaker.AutoPanZone", FsmEditorSettings.EdgeScrollZone);
			EditorPrefs.SetBool("PlayMaker.CheckForRequiredComponent", FsmEditorSettings.CheckForRequiredComponent);
			EditorPrefs.SetBool("PlayMaker.CheckForRequiredField", FsmEditorSettings.CheckForRequiredField);
			EditorPrefs.SetBool("PlayMaker.CheckForTransitionMissingEvent", FsmEditorSettings.CheckForTransitionMissingEvent);
			EditorPrefs.SetBool("PlayMaker.CheckForTransitionMissingTarget", FsmEditorSettings.CheckForTransitionMissingTarget);
			EditorPrefs.SetBool("PlayMaker.CheckForDuplicateTransitionEvent", FsmEditorSettings.CheckForDuplicateTransitionEvent);
			EditorPrefs.SetBool("PlayMaker.CheckForMouseEventErrors", FsmEditorSettings.CheckForMouseEventErrors);
			EditorPrefs.SetBool("PlayMaker.CheckForCollisionEventErrors", FsmEditorSettings.CheckForCollisionEventErrors);
			EditorPrefs.SetBool("PlayMaker.CheckForEventNotUsed", FsmEditorSettings.CheckForEventNotUsed);
			EditorPrefs.SetBool("PlayMaker.CheckForPrefabRestrictions", FsmEditorSettings.CheckForPrefabRestrictions);
			EditorPrefs.SetBool("PlayMaker.CheckForObsoleteActions", FsmEditorSettings.CheckForObsoleteActions);
			EditorPrefs.SetBool("PlayMaker.CheckForMissingActions", FsmEditorSettings.CheckForMissingActions);
			EditorPrefs.SetBool("PlayMaker.CheckForNetworkSetupErrors", FsmEditorSettings.CheckForNetworkSetupErrors);
			EditorPrefs.SetInt("PlayMaker.ColorScheme", (int)FsmEditorSettings.ColorScheme);
			EditorPrefs.SetBool("PlayMaker.DisableEditorWhenPlaying", FsmEditorSettings.DisableEditorWhenPlaying);
			EditorPrefs.SetBool("PlayMaker.DisableInspectorWhenPlaying", FsmEditorSettings.DisableInspectorWhenPlaying);
			EditorPrefs.SetBool("PlayMaker.DisableToolWindowsWhenPlaying", FsmEditorSettings.DisableToolWindowsWhenPlaying);
			EditorPrefs.SetBool("PlayMaker.DisableActionBrowerWhenPlaying", FsmEditorSettings.DisableActionBrowerWhenPlaying);
			EditorPrefs.SetBool("PlayMaker.DisableEventBrowserWhenPlaying", FsmEditorSettings.DisableEventBrowserWhenPlaying);
			EditorPrefs.SetBool("PlayMaker.ShowHints", FsmEditorSettings.ShowHints);
			EditorPrefs.SetBool("PlayMaker.CloseActionBrowserOnEnter", FsmEditorSettings.CloseActionBrowserOnEnter);
			EditorPrefs.SetBool("PlayMaker.LogPauseOnSelect", FsmEditorSettings.LogPauseOnSelect);
			EditorPrefs.SetBool("PlayMaker.LogShowSentBy", FsmEditorSettings.LogShowSentBy);
			EditorPrefs.SetBool("PlayMaker.LogShowExit", FsmEditorSettings.LogShowExit);
			EditorPrefs.SetBool("PlayMaker.LogShowTimecode", FsmEditorSettings.LogShowTimecode);
			EditorPrefs.SetBool("PlayMaker.EnableDebugFlow", FsmEditorSettings.EnableDebugFlow);
			EditorPrefs.SetBool("PlayMaker.EnableTransitionEffects", FsmEditorSettings.EnableTransitionEffects);
			EditorPrefs.SetInt("PlayMaker.ConsoleActionReportSortOptionIndex", FsmEditorSettings.ConsoleActionReportSortOptionIndex);
			EditorPrefs.SetBool("PlayMaker.DebugVariables", FsmEditorSettings.DebugVariables);
			EditorPrefs.SetBool("PlayMaker.SelectNewVariables", FsmEditorSettings.SelectNewVariables);
			EditorPrefs.SetBool("PlayMaker.FsmBrowserShowFullPath", FsmEditorSettings.FsmBrowserShowFullPath);
			EditorPrefs.SetBool("PlayMaker.AutoRefreshActionUsage", FsmEditorSettings.AutoRefreshActionUsage);
			SkillEditor.Repaint(true);
		}
		[Localizable(false)]
		public static void LoadSettings()
		{
			if (FsmEditorSettings.settingsLoaded)
			{
				return;
			}
			FsmEditorSettings.settingsLoaded = true;
			FsmEditorSettings.selectedCulture = 0;
			string @string = EditorPrefs.GetString("PlayMaker.Language", "");
			for (int i = 0; i < FsmEditorSettings.supportedCultures.Length; i++)
			{
				string text = FsmEditorSettings.supportedCultures[i];
				if (text == @string)
				{
					FsmEditorSettings.selectedCulture = i;
				}
			}
			FsmEditorSettings.ActionBrowserRecentSize = EditorPrefs.GetInt("PlayMaker.ActionBrowserRecentSize", 20);
			FsmEditorSettings.DebuggerStepMode = (FsmDebugger.FsmStepMode)EditorPrefs.GetInt("PlayMaker.DebuggerStepMode", 0);
			FsmEditorSettings.DisableUndoRedo = EditorPrefs.GetBool("PlayMaker.DisableUndoRedo", false);
			FsmEditorSettings.MaximizeFileCompatibility = EditorPrefs.GetBool("PlayMaker.MaximizeFileCompatibility", true);
			FsmEditorSettings.DrawAssetThumbnail = EditorPrefs.GetBool("PlayMaker.DrawAssetThumbnail", true);
			FsmEditorSettings.DrawLinksBehindStates = EditorPrefs.GetBool("PlayMaker.DrawLinksBehindStates", true);
			FsmEditorSettings.DimFinishedActions = EditorPrefs.GetBool("PlayMaker.DimFinishedActions", true);
			FsmEditorSettings.AutoRefreshFsmInfo = EditorPrefs.GetBool("PlayMaker.AutoRefreshFsmInfo", true);
			FsmEditorSettings.ConfirmEditingPrefabInstances = EditorPrefs.GetBool("PlayMaker.ConfirmEditingPrefabInstances", true);
			FsmEditorSettings.ShowStateLoopCounts = EditorPrefs.GetBool("PlayMaker.ShowStateLoopCounts", false);
			FsmEditorSettings.DrawFrameAroundGraph = EditorPrefs.GetBool("PlayMaker.DrawFrameAroundGraph", false);
			FsmEditorSettings.GraphViewShowMinimap = EditorPrefs.GetBool("PlayMaker.GraphViewShowMinimap", true);
			FsmEditorSettings.GraphViewMinimapSize = EditorPrefs.GetFloat("PlayMaker.GraphViewMinimapSize", 300f);
			FsmEditorSettings.GraphViewZoomSpeed = EditorPrefs.GetFloat("PlayMaker.GraphViewZoomSpeed", 0.01f);
			FsmEditorSettings.MouseWheelScrollsGraphView = EditorPrefs.GetBool("PlayMaker.MouseWheelScrollsGraphView", false);
			FsmEditorSettings.ScreenshotsPath = EditorPrefs.GetString("PlayMaker.ScreenshotsPath", "PlayMaker/Screenshots");
			FsmEditorSettings.DebugVariables = EditorPrefs.GetBool("PlayMaker.DebugVariables", false);
			FsmEditorSettings.ConsoleActionReportSortOptionIndex = EditorPrefs.GetInt("PlayMaker.ConsoleActionReportSortOptionIndex", 1);
			FsmEditorSettings.LogPauseOnSelect = EditorPrefs.GetBool("PlayMaker.LogPauseOnSelect", true);
			FsmEditorSettings.LogShowSentBy = EditorPrefs.GetBool("PlayMaker.LogShowSentBy", true);
			FsmEditorSettings.LogShowExit = EditorPrefs.GetBool("PlayMaker.LogShowExit", true);
			FsmEditorSettings.LogShowTimecode = EditorPrefs.GetBool("PlayMaker.LogShowTimecode", false);
			FsmEditorSettings.ShowHints = EditorPrefs.GetBool("PlayMaker.ShowHints", true);
			FsmEditorSettings.CloseActionBrowserOnEnter = EditorPrefs.GetBool("PlayMaker.CloseActionBrowserOnEnter", false);
			FsmEditorSettings.DisableEditorWhenPlaying = EditorPrefs.GetBool("PlayMaker.DisableEditorWhenPlaying", false);
			FsmEditorSettings.DisableInspectorWhenPlaying = EditorPrefs.GetBool("PlayMaker.DisableInspectorWhenPlaying", false);
			FsmEditorSettings.DisableToolWindowsWhenPlaying = EditorPrefs.GetBool("PlayMaker.DisableToolWindowsWhenPlaying", true);
			FsmEditorSettings.DisableActionBrowerWhenPlaying = EditorPrefs.GetBool("PlayMaker.DisableActionBrowerWhenPlaying", false);
			FsmEditorSettings.DisableEventBrowserWhenPlaying = EditorPrefs.GetBool("PlayMaker.DisableEventBrowserWhenPlaying", false);
			FsmEditorSettings.ColorScheme = (SkillEditorStyles.ColorScheme)EditorPrefs.GetInt("PlayMaker.ColorScheme", 0);
			FsmEditorSettings.EnableRealtimeErrorChecker = EditorPrefs.GetBool("PlayMaker.EnableRealtimeErrorChecker", true);
			FsmEditorSettings.CheckForRequiredComponent = EditorPrefs.GetBool("PlayMaker.CheckForRequiredComponent", true);
			FsmEditorSettings.CheckForRequiredField = EditorPrefs.GetBool("PlayMaker.CheckForRequiredField", true);
			FsmEditorSettings.CheckForEventNotUsed = EditorPrefs.GetBool("PlayMaker.CheckForEventNotUsed", true);
			FsmEditorSettings.CheckForTransitionMissingEvent = EditorPrefs.GetBool("PlayMaker.CheckForTransitionMissingEvent", true);
			FsmEditorSettings.CheckForTransitionMissingTarget = EditorPrefs.GetBool("PlayMaker.CheckForTransitionMissingTarget", true);
			FsmEditorSettings.CheckForDuplicateTransitionEvent = EditorPrefs.GetBool("PlayMaker.CheckForDuplicateTransitionEvent", true);
			FsmEditorSettings.CheckForMouseEventErrors = EditorPrefs.GetBool("PlayMaker.CheckForMouseEventErrors", true);
			FsmEditorSettings.CheckForCollisionEventErrors = EditorPrefs.GetBool("PlayMaker.CheckForCollisionEventErrors", true);
			FsmEditorSettings.CheckForPrefabRestrictions = EditorPrefs.GetBool("PlayMaker.CheckForPrefabRestrictions", true);
			FsmEditorSettings.CheckForObsoleteActions = EditorPrefs.GetBool("PlayMaker.CheckForObsoleteActions", true);
			FsmEditorSettings.CheckForMissingActions = EditorPrefs.GetBool("PlayMaker.CheckForMissingActions", true);
			FsmEditorSettings.CheckForNetworkSetupErrors = EditorPrefs.GetBool("PlayMaker.CheckForNetworkSetupErrors", true);
			FsmEditorSettings.EdgeScrollZone = EditorPrefs.GetFloat("PlayMaker.AutoPanZone", 100f);
			FsmEditorSettings.EdgeScrollSpeed = EditorPrefs.GetFloat("PlayMaker.EdgeScrollSpeed", 1f);
			FsmEditorSettings.HideObsoleteActions = EditorPrefs.GetBool("PlayMaker.HideObsoleteActions", true);
			FsmEditorSettings.ColorLinks = EditorPrefs.GetBool("PlayMaker.ColorLinks", false);
			FsmEditorSettings.DisableErrorCheckerWhenPlaying = EditorPrefs.GetBool("PlayMaker.DisableErrorCheckerWhenPlaying", true);
			FsmEditorSettings.EnableLogging = EditorPrefs.GetBool("PlayMaker.EnableLogging", true);
			FsmEditorSettings.SnapGridSize = EditorPrefs.GetInt("PlayMaker.SnapGridSize", 16);
			FsmEditorSettings.SnapToGrid = EditorPrefs.GetBool("PlayMaker.SnapToGrid", false);
			FsmEditorSettings.ShowScrollBars = EditorPrefs.GetBool("PlayMaker.ShowScrollBars", true);
			FsmEditorSettings.EnableWatermarks = EditorPrefs.GetBool("PlayMaker.ShowWatermark", true);
			FsmEditorSettings.StateMaxWidth = EditorPrefs.GetInt("PlayMaker.StateMaxWidth", 400);
			FsmEditorSettings.AddPrefabLabel = EditorPrefs.GetBool("PlayMaker.AddPrefabLabel", true);
			FsmEditorSettings.AutoLoadPrefabs = EditorPrefs.GetBool("PlayMaker.AutoLoadPrefabs", true);
			FsmEditorSettings.LoadAllPrefabs = EditorPrefs.GetBool("PlayMaker.LoadAllPrefabs", true);
			FsmEditorSettings.UnloadPrefabs = EditorPrefs.GetBool("PlayMaker.UnloadPrefabs", true);
			FsmEditorSettings.StartStateName = EditorPrefs.GetString("PlayMaker.StartStateName", "State 1");
			FsmEditorSettings.NewStateName = EditorPrefs.GetString("PlayMaker.NewStateName", Strings.get_FsmEditorSettings_Default_State_Name());
			FsmEditorSettings.AutoSelectGameObject = EditorPrefs.GetBool("PlayMaker.AutoSelectGameObject", true);
			FsmEditorSettings.SelectStateOnActivated = EditorPrefs.GetBool("PlayMaker.SelectStateOnActivated", true);
			FsmEditorSettings.JumpToBreakpoint = EditorPrefs.GetBool("PlayMaker.GotoBreakpoint", true);
			FsmEditorSettings.GameStateIconSize = EditorPrefs.GetInt("PlayMaker.GameStateIconSize", 32);
			FsmEditorSettings.FrameSelectedState = EditorPrefs.GetBool("PlayMaker.FrameSelectedState", false);
			FsmEditorSettings.SyncLogSelection = EditorPrefs.GetBool("PlayMaker.SyncLogSelection", true);
			FsmEditorSettings.BreakpointsEnabled = EditorPrefs.GetBool("PlayMaker.BreakpointsEnabled", true);
			FsmEditorSettings.MirrorDebugLog = EditorPrefs.GetBool("PlayMaker.MirrorDebugLog", false);
			FsmEditorSettings.LockGraphView = EditorPrefs.GetBool("PlayMaker.LockGraphView", false);
			FsmEditorSettings.GraphViewLinkStyle = (GraphViewLinkStyle)EditorPrefs.GetInt("PlayMaker.GraphLinkStyle", 0);
			FsmEditorSettings.ShowFsmDescriptionInGraphView = EditorPrefs.GetBool("PlayMaker.ShowFsmDescriptionInGraphView", true);
			FsmEditorSettings.ShowCommentsInGraphView = EditorPrefs.GetBool("PlayMaker.ShowCommentsInGraphView", true);
			FsmEditorSettings.ShowStateLabelsInGameView = EditorPrefs.GetBool("PlayMaker.ShowStateLabelsInGameView", true);
			FsmEditorSettings.DrawPlaymakerGizmos = EditorPrefs.GetBool("PlayMaker.DrawPlaymakerGizmos", true);
			FsmEditorSettings.DrawPlaymakerGizmoInHierarchy = EditorPrefs.GetBool("PlayMaker.DrawPlaymakerGizmoInHierarchy", true);
			FsmEditorSettings.ShowEditWhileRunningWarning = EditorPrefs.GetBool("PlayMaker.ShowEditWhileRunningWarning", true);
			FsmEditorSettings.ShowStateDescription = EditorPrefs.GetBool("PlayMaker.ShowStateDescription", true);
			FsmEditorSettings.ShowActionParameters = true;
			FsmEditorSettings.DebugActionParameters = EditorPrefs.GetBool("PlayMaker.DebugActionParameters", false);
			FsmEditorSettings.HideUnusedEvents = EditorPrefs.GetBool("PlayMaker.HideUnusedEvents", false);
			FsmEditorSettings.ShowActionPreview = EditorPrefs.GetBool("PlayMaker.ShowActionPreview", true);
			FsmEditorSettings.SelectedActionCategory = EditorPrefs.GetInt("PlayMaker.SelectedActionCategory", 0);
			FsmEditorSettings.SelectFSMInGameView = EditorPrefs.GetBool("PlayMaker.SelectFSMInGameView", true);
			FsmEditorSettings.DebugLookAtColor = FsmEditorSettings.UnpackColorFromInt(EditorPrefs.GetInt("PlayMaker.DebugLookAtColor", FsmEditorSettings.PackColorIntoInt(Color.get_gray())));
			FsmEditorSettings.DebugRaycastColor = FsmEditorSettings.UnpackColorFromInt(EditorPrefs.GetInt("PlayMaker.DebugRaycastColor", FsmEditorSettings.PackColorIntoInt(Color.get_gray())));
			FsmEditorSettings.HideUnusedParams = EditorPrefs.GetBool("PlayMaker.HideUnusedParams", false);
			FsmEditorSettings.AutoAddPlayMakerGUI = EditorPrefs.GetBool("PlayMaker.AutoAddPlayMakerGUI", true);
			FsmEditorSettings.DimUnusedActionParameters = EditorPrefs.GetBool("PlayMaker.DimUnusedParameters", false);
			FsmEditorSettings.SelectNewVariables = EditorPrefs.GetBool("PlayMaker.SelectNewVariables", true);
			FsmEditorSettings.FsmBrowserShowFullPath = EditorPrefs.GetBool("PlayMaker.FsmBrowserShowFullPath", true);
			FsmEditorSettings.EnableDebugFlow = EditorPrefs.GetBool("PlayMaker.EnableDebugFlow", true);
			FsmEditorSettings.EnableTransitionEffects = EditorPrefs.GetBool("PlayMaker.EnableTransitionEffects", true);
			FsmEditorSettings.AutoRefreshActionUsage = EditorPrefs.GetBool("PlayMaker.AutoRefreshActionUsage", true);
			FsmEditorSettings.ValidateSettings();
			FsmEditorSettings.ApplySettings();
			FsmEditorSettings.SaveSettings();
		}
		private static void ValidateSettings()
		{
			if (string.IsNullOrEmpty(FsmEditorSettings.NewStateName) || FsmEditorSettings.StateMaxWidth == 0)
			{
				Debug.LogWarning(Strings.get_FsmEditorSettings_Preferences_Reset());
				FsmEditorSettings.ResetDefaults();
			}
			FsmEditorSettings.EdgeScrollSpeed = Mathf.Clamp(FsmEditorSettings.EdgeScrollSpeed, 0.1f, 10f);
		}
		private static void ApplySettings()
		{
			FsmDebugger.Instance.StepMode = FsmEditorSettings.DebuggerStepMode;
			SkillLog.set_MirrorDebugLog(FsmEditorSettings.MirrorDebugLog);
			SkillLog.set_LoggingEnabled(FsmEditorSettings.EnableLogging);
			SkillLog.set_EnableDebugFlow(FsmEditorSettings.EnableDebugFlow);
			Skill.set_BreakpointsEnabled(FsmEditorSettings.BreakpointsEnabled);
			PlayMakerFSM.set_DrawGizmos(FsmEditorSettings.DrawPlaymakerGizmos);
			Skill.set_DebugLookAtColor(FsmEditorSettings.DebugLookAtColor);
			Skill.set_DebugRaycastColor(FsmEditorSettings.DebugRaycastColor);
			PlayMakerGUI.set_EnableStateLabels(FsmEditorSettings.ShowStateLabelsInGameView);
			FsmEditorSettings.SetCulture(FsmEditorSettings.selectedCulture);
			if (SkillEditor.Instance != null)
			{
				SkillEditor.GraphView.ApplySettings();
			}
		}
		public static int PackColorIntoInt(Color color)
		{
			int num = (int)(color.r * 255f);
			int num2 = (int)(color.g * 255f);
			int num3 = (int)(color.b * 255f);
			return num << 16 | num2 << 8 | num3;
		}
		public static Color UnpackColorFromInt(int packedValue)
		{
			int num = packedValue >> 16;
			int num2 = packedValue >> 8 & 255;
			int num3 = packedValue & 255;
			return new Color((float)num / 255f, (float)num2 / 255f, (float)num3 / 255f, 1f);
		}
	}
}
