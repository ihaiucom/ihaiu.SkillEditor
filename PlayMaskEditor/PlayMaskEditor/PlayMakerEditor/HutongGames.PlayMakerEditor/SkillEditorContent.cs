using System;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public static class SkillEditorContent
	{
		private const string copyShortcut = " %c";
		private const string cutShortcut = " %x";
		private const string pasteShortcut = " %v";
		private const string selectAllShortcut = " %a";
		private static readonly GUIContent tempContent = new GUIContent();
		public static GUIContent FavoritesLabel
		{
			get;
			private set;
		}
		public static GUIContent ManualUpdate
		{
			get;
			private set;
		}
		public static GUIContent KeepDelayedEvents
		{
			get;
			private set;
		}
		public static GUIContent LockFsmButton
		{
			get;
			private set;
		}
		public static GUIContent MenuGraphViewCopyStates
		{
			get;
			private set;
		}
		public static GUIContent MenuGraphViewPasteStates
		{
			get;
			private set;
		}
		public static GUIContent MenuSelectAllActions
		{
			get;
			private set;
		}
		public static GUIContent MenuCopySelectedActions
		{
			get;
			private set;
		}
		public static GUIContent MenuPasteActions
		{
			get;
			private set;
		}
		public static GUIContent EditCategoryLabel
		{
			get;
			private set;
		}
		public static GUIContent GlobalVariableName
		{
			get;
			set;
		}
		public static GUIContent GlobalEventName
		{
			get;
			set;
		}
		public static GUIContent FsmBrowserButton
		{
			get;
			private set;
		}
		public static GUIContent RecentButton
		{
			get;
			private set;
		}
		public static GUIContent BackButton
		{
			get;
			private set;
		}
		public static GUIContent ForwardButton
		{
			get;
			private set;
		}
		public static GUIContent StateBrowserButton
		{
			get;
			private set;
		}
		public static GUIContent BrowseButton
		{
			get;
			private set;
		}
		public static GUIContent HelpButton
		{
			get;
			private set;
		}
		public static GUIContent DeleteButton
		{
			get;
			private set;
		}
		public static GUIContent ResetButton
		{
			get;
			private set;
		}
		public static GUIContent UpButton
		{
			get;
			private set;
		}
		public static GUIContent DownButton
		{
			get;
			private set;
		}
		public static GUIContent VariableButton
		{
			get;
			private set;
		}
		public static GUIContent SettingsButton
		{
			get;
			private set;
		}
		public static GUIContent PopupButton
		{
			get;
			private set;
		}
		public static GUIContent Play
		{
			get;
			private set;
		}
		public static GUIContent Pause
		{
			get;
			private set;
		}
		public static GUIContent Step
		{
			get;
			private set;
		}
		public static GUIContent MainToolbarSelectedGO
		{
			get;
			private set;
		}
		public static GUIContent MainToolbarSelectedFSM
		{
			get;
			private set;
		}
		public static GUIContent MainToolbarLock
		{
			get;
			private set;
		}
		public static GUIContent MainToolbarPrefabTypeNone
		{
			get;
			private set;
		}
		public static GUIContent MainToolbarShowMinimap
		{
			get;
			private set;
		}
		public static GUIContent NewVariableLabel
		{
			get;
			private set;
		}
		public static GUIContent EditVariableLabel
		{
			get;
			private set;
		}
		public static GUIContent GlobalVariablesLabel
		{
			get;
			private set;
		}
		public static GUIContent VariableNameLabel
		{
			get;
			private set;
		}
		public static GUIContent VariableTypeLabel
		{
			get;
			private set;
		}
		public static GUIContent VariableValueLabel
		{
			get;
			private set;
		}
		public static GUIContent GlobalsLabel
		{
			get;
			private set;
		}
		public static GUIContent VariableUseCountLabel
		{
			get;
			private set;
		}
		public static GUIContent TooltipLabel
		{
			get;
			private set;
		}
		public static GUIContent InspectorLabel
		{
			get;
			private set;
		}
		public static GUIContent NetworkSyncNotSupportedLabel
		{
			get;
			private set;
		}
		public static GUIContent EditVariableTypeLabel
		{
			get;
			private set;
		}
		public static GUIContent AddLabel
		{
			get;
			private set;
		}
		public static GUIContent GlobalVariablesNameLabel
		{
			get;
			private set;
		}
		public static GUIContent GlobalVariablesTypeLabel
		{
			get;
			private set;
		}
		public static GUIContent AddGlobalVariableLabel
		{
			get;
			private set;
		}
		public static GUIContent GlobalVariableUseCountLabel
		{
			get;
			private set;
		}
		public static GUIContent EditEventNameLabel
		{
			get;
			private set;
		}
		public static GUIContent AddEventLabel
		{
			get;
			private set;
		}
		public static GUIContent EventInspectorLabel
		{
			get;
			private set;
		}
		public static GUIContent EventBrowserButtonLabel
		{
			get;
			private set;
		}
		public static GUIContent EventBroadcastIcon
		{
			get;
			private set;
		}
		public static GUIContent EventHeaderLabel
		{
			get;
			private set;
		}
		public static GUIContent EventUsedHeaderLabel
		{
			get;
			private set;
		}
		public static GUIContent GlobalEventTooltipLabel
		{
			get;
			private set;
		}
		public static GUIContent DebugToolbarErrorCount
		{
			get;
			private set;
		}
		public static GUIContent DebugToolbarDebug
		{
			get;
			private set;
		}
		public static GUIContent DebugToolbarPrev
		{
			get;
			private set;
		}
		public static GUIContent DebugToolbarNext
		{
			get;
			private set;
		}
		public static GUIContent StateTitleBox
		{
			get;
			private set;
		}
		public static GUIContent StateDescription
		{
			get;
			private set;
		}
		public static GUIContent FsmDescription
		{
			get;
			private set;
		}
		public static GUIContent EventLabel
		{
			get;
			private set;
		}
		public static GUIContent HintGettingStarted
		{
			get;
			private set;
		}
		public static GUIContent HintGraphShortcuts
		{
			get;
			private set;
		}
		public static GUIContent HintGraphCommands
		{
			get;
			private set;
		}
		public static Vector2 HintGettingStartedSize
		{
			get;
			private set;
		}
		public static Vector2 HintGraphShortcutsSize
		{
			get;
			private set;
		}
		public static Vector2 HintGraphCommandsSize
		{
			get;
			private set;
		}
		public static GUIContent BrowseTemplateButton
		{
			get;
			private set;
		}
		public static GUIContent MaxLoopOverrideLabel
		{
			get;
			private set;
		}
		public static GUIContent ShowStateLabelsLabel
		{
			get;
			private set;
		}
		public static GUIContent EnableBreakpointsLabel
		{
			get;
			private set;
		}
		public static GUIContent EnableDebugFlowLabel
		{
			get;
			private set;
		}
		public static GUIContent ResetOnDisableLabel
		{
			get;
			private set;
		}
		public static GUIContent FsmControlsLabel
		{
			get;
			private set;
		}
		public static GUIContent NetworkSyncLabel
		{
			get;
			private set;
		}
		public static GUIContent UseTemplateLabel
		{
			get;
			private set;
		}
		public static GUIContent RefreshTemplateLabel
		{
			get;
			private set;
		}
		public static GUIContent EditFsmButton
		{
			get;
			private set;
		}
		public static GUIContent ConfirmEditPrefabInstance
		{
			get;
			set;
		}
		public static void Init(bool usingProSkin)
		{
			SkillEditorContent.FavoritesLabel = new GUIContent(Strings.get_Label_Favorites(), Strings.get_Tooltip_Favorites());
			SkillEditorContent.ManualUpdate = new GUIContent(Strings.get_Label_Manual_Update(), Strings.get_Tooltip_ManualUpdate());
			SkillEditorContent.KeepDelayedEvents = new GUIContent(Strings.get_Label_KeepDelayedEvents(), Strings.get_Tooltip_KeepDelayedEvents());
			SkillEditorContent.LockFsmButton = new GUIContent(Strings.get_Label_Lock(), Strings.get_Tooltip_Lock_and_password_protect_FSM());
			SkillEditorContent.ConfirmEditPrefabInstance = new GUIContent(Strings.get_Label_Confirm_Editing_Prefab_Instances(), Strings.get_Tooltip_Disable_editing_of_prefab_intances());
			SkillEditorContent.EditFsmButton = new GUIContent(Strings.get_Label_Edit(), Strings.get_Tooltip_Edit_in_the_PlayMaker_Editor());
			SkillEditorContent.RefreshTemplateLabel = new GUIContent(Strings.get_Label_Refresh_Template(), Strings.get_Tooltip_Refresh_Template());
			SkillEditorContent.UseTemplateLabel = new GUIContent(Strings.get_Label_Use_Template(), Strings.get_Tooltip_Use_Template());
			SkillEditorContent.BrowseTemplateButton = new GUIContent(Strings.get_Label_Browse(), Strings.get_Tooltip_Browse_Templates());
			SkillEditorContent.MaxLoopOverrideLabel = new GUIContent(Strings.get_Label_Max_Loop_Override(), Strings.get_Tooltip_Max_Loop_Override());
			SkillEditorContent.ShowStateLabelsLabel = new GUIContent(Strings.get_FsmEditorSettings_Show_State_Labels_in_Game_View(), Strings.get_Tooltip_Show_State_Label());
			SkillEditorContent.EnableBreakpointsLabel = new GUIContent(Strings.get_Label_Enable_Breakpoints(), Strings.get_Tooltip_Enable_Breakpoints());
			SkillEditorContent.EnableDebugFlowLabel = new GUIContent(Strings.get_FsmEditorSettings_Enable_DebugFlow(), Strings.get_FsmEditorSettings_Enable_DebugFlow_Tooltip());
			SkillEditorContent.ResetOnDisableLabel = new GUIContent(Strings.get_Label_Reset_On_Disable(), Strings.get_Tooltip_Reset_On_Disable());
			SkillEditorContent.FsmControlsLabel = new GUIContent(Strings.get_Label_Controls(), Strings.get_Tooltip_Controls());
			SkillEditorContent.NetworkSyncLabel = new GUIContent(Strings.get_Label_Network_Sync(), Strings.get_Tooltip_Variables_Set_To_Network_Sync());
			SkillEditorContent.MenuGraphViewCopyStates = new GUIContent(Strings.get_Menu_GraphView_Copy_States() + " %c");
			SkillEditorContent.MenuGraphViewPasteStates = new GUIContent(Strings.get_Menu_GraphView_Paste_States() + " %v");
			SkillEditorContent.MenuSelectAllActions = new GUIContent(Strings.get_Menu_Select_All_Actions() + " %a");
			SkillEditorContent.MenuPasteActions = new GUIContent(Strings.get_Menu_Paste_Actions() + " %v");
			SkillEditorContent.MenuCopySelectedActions = new GUIContent(Strings.get_Menu_Copy_Selected_Actions() + " %c");
			SkillEditorContent.EditCategoryLabel = new GUIContent(Strings.get_Category(), Strings.get_Category_Tooltip());
			SkillEditorContent.GlobalVariableName = new GUIContent(Strings.get_Variable(), Strings.get_Variable_Tooltip_Warning());
			SkillEditorContent.GlobalEventName = new GUIContent(Strings.get_Event(), Strings.get_Event_Tooltip_Warning());
			SkillEditorContent.StateBrowserButton = new GUIContent(Files.LoadTextureFromDll("browserIcon", 14, 14), Strings.get_Command_State_Browser());
			SkillEditorContent.HelpButton = new GUIContent(Files.LoadTextureFromDll("helpIcon", 14, 14), Strings.get_Tooltip_Doc_Button());
			SkillEditorContent.Play = new GUIContent(Files.LoadTextureFromDll("playButton", 17, 17));
			SkillEditorContent.Pause = new GUIContent(Files.LoadTextureFromDll("pauseButton", 17, 17));
			SkillEditorContent.Step = new GUIContent(Files.LoadTextureFromDll("stepButton", 17, 17));
			SkillEditorContent.NewVariableLabel = new GUIContent(Strings.get_Label_New_Variable(), Strings.get_Tooltip_New_Variable());
			SkillEditorContent.EditVariableLabel = new GUIContent(Strings.get_Label_Name());
			SkillEditorContent.GlobalVariablesLabel = new GUIContent(Strings.get_Label_Global_Variables(), Strings.get_Tooltip_Global_Variables());
			SkillEditorContent.VariableNameLabel = new GUIContent(Strings.get_Label_Name(), Strings.get_Tooltip_Variable_Name());
			SkillEditorContent.VariableTypeLabel = new GUIContent(Strings.get_Label_Type(), Strings.get_Tooltip_Variable_Type());
			SkillEditorContent.VariableValueLabel = new GUIContent(Strings.get_Label_Value());
			SkillEditorContent.GlobalsLabel = new GUIContent(Strings.get_Label_Globals(), Strings.get_Tooltip_Globals());
			SkillEditorContent.VariableUseCountLabel = new GUIContent(Strings.get_Label_Used(), Strings.get_Tooltip_Variable_Used_Count());
			SkillEditorContent.TooltipLabel = new GUIContent(Strings.get_Label_Tooltip(), Strings.get_Tooltip_Tooltip());
			SkillEditorContent.InspectorLabel = new GUIContent(Strings.get_Label_Inspector(), Strings.get_Tooltip_Inspector());
			SkillEditorContent.NetworkSyncLabel = new GUIContent(Strings.get_Label_Network_Sync(), Strings.get_Tooltip_Network_Sync());
			SkillEditorContent.NetworkSyncNotSupportedLabel = new GUIContent(Strings.get_Label_Network_Sync(), Strings.get_Tooltip_Network_Sync_Not_Supported());
			SkillEditorContent.EditVariableTypeLabel = new GUIContent(Strings.get_Label_Variable_Type(), Strings.get_Tooltip_Edit_Variable_Type());
			SkillEditorContent.AddLabel = new GUIContent(Strings.get_Label_Add());
			SkillEditorContent.GlobalVariablesNameLabel = new GUIContent(Strings.get_Label_Name(), Strings.get_Tooltip_Global_Variables_Header());
			SkillEditorContent.GlobalVariablesTypeLabel = new GUIContent(Strings.get_Label_Type(), Strings.get_Tooltip_Global_Variables_Type());
			SkillEditorContent.AddGlobalVariableLabel = new GUIContent(Strings.get_Label_New_Variable(), Strings.get_Tooltip_Add_Global_Variable());
			SkillEditorContent.GlobalVariableUseCountLabel = new GUIContent(Strings.get_Label_Used(), Strings.get_Tooltip_Global_Variables_Used_Count());
			SkillEditorContent.EditEventNameLabel = new GUIContent(Strings.get_Label_Event_Name(), Strings.get_Tooltip_EventManager_Edit_Event());
			SkillEditorContent.AddEventLabel = new GUIContent(Strings.get_Label_Add_Event(), Strings.get_Tooltip_EventManager_Add_Event());
			SkillEditorContent.EventInspectorLabel = new GUIContent(Strings.get_Label_Inspector(), Strings.get_Tooltip_EventManager_Inspector_Checkbox());
			SkillEditorContent.EventBrowserButtonLabel = new GUIContent(Strings.get_Command_Event_Browser(), Strings.get_Tooltip_Event_Browser_Button_in_Events_Tab());
			SkillEditorContent.EventBroadcastIcon = new GUIContent(SkillEditorStyles.BroadcastIcon, Strings.get_Tooltip_Global_Event_Flag());
			SkillEditorContent.EventHeaderLabel = new GUIContent(Strings.get_Label_Event(), Strings.get_Tooltip_Event_GUI());
			SkillEditorContent.EventUsedHeaderLabel = new GUIContent(Strings.get_Label_Used(), Strings.get_Tooltip_Events_Used_By_States());
			SkillEditorContent.GlobalEventTooltipLabel = new GUIContent("", Strings.get_Label_Global());
			if (usingProSkin)
			{
				SkillEditorContent.FsmBrowserButton = new GUIContent(Files.LoadTextureFromDll("browserIcon", 14, 14), Strings.get_Tooltip_Editor_Windows());
				SkillEditorContent.BackButton = new GUIContent(Files.LoadTextureFromDll("backIcon", 10, 14), "Select Previous FSM");
				SkillEditorContent.ForwardButton = new GUIContent(Files.LoadTextureFromDll("forwardIcon", 10, 14), "Select Next FSM");
				SkillEditorContent.RecentButton = new GUIContent(Files.LoadTextureFromDll("recentIcon", 10, 14), Strings.get_MainToolbar_Recent());
				SkillEditorContent.DeleteButton = new GUIContent(Files.LoadTextureFromDll("deleteIcon", 17, 14), Strings.get_Command_Delete());
				SkillEditorContent.ResetButton = new GUIContent(Files.LoadTextureFromDll("deleteIcon", 17, 14), Strings.get_Command_Reset());
				SkillEditorContent.UpButton = new GUIContent(Files.LoadTextureFromDll("upIcon", 17, 14), Strings.get_Command_Move_Up());
				SkillEditorContent.DownButton = new GUIContent(Files.LoadTextureFromDll("downIcon", 17, 14), Strings.get_Command_Move_Down());
				SkillEditorContent.VariableButton = new GUIContent(Files.LoadTextureFromDll("variableIcon", 17, 14), Strings.get_Option_Use_Variable());
				SkillEditorContent.SettingsButton = new GUIContent(Files.LoadTextureFromDll("settingsIcon", 14, 12), Strings.get_Command_Settings());
				SkillEditorContent.BrowseButton = new GUIContent(Files.LoadTextureFromDll("browseIcon", 17, 14), Strings.get_Command_Browse());
				SkillEditorContent.MainToolbarShowMinimap = new GUIContent(Files.LoadTextureFromDll("minimapIcon", 14, 14), Strings.get_Tooltip_Toggle_Minimap());
				SkillEditorContent.PopupButton = new GUIContent(Files.LoadTextureFromDll("browseIcon", 17, 14), "");
			}
			else
			{
				SkillEditorContent.FsmBrowserButton = new GUIContent(Files.LoadTextureFromDll("browserIcon_indie", 14, 14), Strings.get_Tooltip_Editor_Windows());
				SkillEditorContent.BackButton = new GUIContent(Files.LoadTextureFromDll("backIcon_indie", 10, 14));
				SkillEditorContent.ForwardButton = new GUIContent(Files.LoadTextureFromDll("forwardIcon_indie", 10, 14));
				SkillEditorContent.RecentButton = new GUIContent(Files.LoadTextureFromDll("recentIcon_indie", 10, 14), Strings.get_MainToolbar_Recent());
				SkillEditorContent.DeleteButton = new GUIContent(Files.LoadTextureFromDll("deleteIcon_indie", 17, 14), Strings.get_Command_Delete());
				SkillEditorContent.ResetButton = new GUIContent(Files.LoadTextureFromDll("deleteIcon_indie", 17, 14), Strings.get_Command_Reset());
				SkillEditorContent.UpButton = new GUIContent(Files.LoadTextureFromDll("upIcon_indie", 17, 14), Strings.get_Command_Move_Up());
				SkillEditorContent.DownButton = new GUIContent(Files.LoadTextureFromDll("downIcon_indie", 17, 14), Strings.get_Command_Move_Down());
				SkillEditorContent.VariableButton = new GUIContent(Files.LoadTextureFromDll("variableIcon_indie", 17, 14), Strings.get_Option_Use_Variable());
				SkillEditorContent.SettingsButton = new GUIContent(Files.LoadTextureFromDll("settingsIcon_indie", 14, 12), Strings.get_Command_Settings());
				SkillEditorContent.BrowseButton = new GUIContent(Files.LoadTextureFromDll("browseIcon_indie", 17, 14), Strings.get_Command_Browse());
				SkillEditorContent.MainToolbarShowMinimap = new GUIContent(Files.LoadTextureFromDll("minimapIcon_indie", 14, 14), Strings.get_Tooltip_Toggle_Minimap());
				SkillEditorContent.PopupButton = new GUIContent(Files.LoadTextureFromDll("browseIcon_indie", 17, 14), "");
			}
			SkillEditorContent.DebugToolbarErrorCount = new GUIContent("", Strings.get_DebugToolbar_Error_Count_Tooltip());
			SkillEditorContent.DebugToolbarDebug = new GUIContent(Strings.get_DebugToolbar_Label_Debug(), Strings.get_DebugToolbar_Label_Debug_Tooltip());
			SkillEditorContent.DebugToolbarPrev = new GUIContent(Strings.get_DebugToolbar_Button_Prev(), Strings.get_DebugToolbar_Button_Prev_Toolrip());
			SkillEditorContent.DebugToolbarNext = new GUIContent(Strings.get_DebugToolbar_Button_Next(), Strings.get_DebugToolbar_Button_Next_Tooltip());
			SkillEditorContent.StateTitleBox = new GUIContent();
			SkillEditorContent.StateDescription = new GUIContent();
			SkillEditorContent.FsmDescription = new GUIContent();
			SkillEditorContent.EventLabel = new GUIContent();
			SkillEditorContent.MainToolbarSelectedGO = new GUIContent();
			SkillEditorContent.MainToolbarSelectedFSM = new GUIContent();
			SkillEditorContent.MainToolbarLock = new GUIContent(Strings.get_Command_Lock_Selected_FSM(), Strings.get_Tooltip_Lock_Selected_FSM());
			SkillEditorContent.MainToolbarPrefabTypeNone = new GUIContent(Strings.get_Label_Select(), Strings.get_Command_Select_GameObject());
			SkillEditorContent.HintGettingStarted = new GUIContent(Strings.get_Hint_GraphView_Getting_Started());
			SkillEditorContent.HintGraphShortcuts = new GUIContent((Application.get_platform() == null) ? Strings.get_Hint_GraphView_Shortcuts_OSX() : Strings.get_Hint_GraphView_Shortcuts());
			SkillEditorContent.HintGraphCommands = new GUIContent(Strings.get_Hint_GraphView_Shortcut_Description());
			SkillEditorContent.HintGettingStartedSize = SkillEditorContent.CalcLineWrappedContentSize(SkillEditorContent.HintGettingStarted, SkillEditorStyles.HintBox);
			SkillEditorContent.HintGraphShortcutsSize = SkillEditorContent.CalcLineWrappedContentSize(SkillEditorContent.HintGraphShortcuts, SkillEditorStyles.HintBox);
			SkillEditorContent.HintGraphCommandsSize = SkillEditorContent.CalcLineWrappedContentSize(SkillEditorContent.HintGraphCommands, SkillEditorStyles.HintBox);
		}
		public static GUIContent TempContent(string text, string tooltip = "")
		{
			SkillEditorContent.tempContent.set_text(text);
			SkillEditorContent.tempContent.set_tooltip(tooltip);
			return SkillEditorContent.tempContent;
		}
		private static Vector2 CalcLineWrappedContentSize(GUIContent content, GUIStyle guiStyle)
		{
			float num;
			float num2;
			guiStyle.CalcMinMaxWidth(content, ref num, ref num2);
			return new Vector2(num2, guiStyle.CalcHeight(content, num2));
		}
	}
}
