using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace ihaiu
{
    public static class EditorCommands
    {

        public static void ToggleShowHints()
        {
            SkillEditorSettings.ShowHints = !SkillEditorSettings.ShowHints;
            SkillEditorSettings.SaveSettings();
            SkillEditor.RepaintAll();
        }
    }
}

//        public static void Cancel()
//        {
//        }
//        public static void SelectState(object userdata)
//        {
//            SkillEditor.SelectState((SkillState)userdata, true);
//        }
//        public static void SelectFsm(object userdata)
//        {
//            SkillEditor.SelectFsm((Skill)userdata);
//        }
//        public static SkillState AddState(Vector2 position)
//        {
//            return SkillEditor.GraphView.AddState(position);
//        }
//        public static bool RenameState(SkillState state, string newName)
//        {
//            string text = SkillEditor.Builder.ValidateNewStateName(state, newName);
//            if (text != "")
//            {
//                EditorUtility.DisplayDialog(Strings.get_Error_Cannot_Rename_State(), text, Strings.get_OK());
//                return false;
//            }
//            SkillEditor.RegisterUndo(Strings.get_Command_Rename_State());
//            SkillEditor.Builder.RenameState(state, newName);
//            SkillEditor.GraphView.UpdateStateSize(state);
//            SkillEditor.SetFsmDirty(true, false);
//            return true;
//        }
//        public static void SetStateName(SkillState state, string name)
//        {
//            SkillEditor.Builder.SetStateName(state, name);
//            SkillEditor.GraphView.UpdateStateSize(state);
//        }
//        public static SkillStateAction AddAction(SkillState state, Type actionType)
//        {
//            SkillEditor.RegisterUndo(Strings.get_Command_Add_Action());
//            SkillStateAction result = SkillEditor.Builder.AddAction(state, actionType);
//            ActionSelector.AddActionToRecent(actionType);
//            SkillEditor.UpdateActionUsage();
//            return result;
//        }
//        public static SkillStateAction InsertAction(SkillState state, Type actionType, SkillStateAction beforeAction)
//        {
//            SkillEditor.RegisterUndo(Strings.get_Command_Add_Action());
//            SkillStateAction result = SkillEditor.Builder.InsertAction(SkillEditor.SelectedState, actionType, beforeAction);
//            ActionSelector.AddActionToRecent(actionType);
//            SkillEditor.UpdateActionUsage();
//            return result;
//        }
//        public static void CutStateSelection()
//        {
//            SkillEditor.Builder.CopyStatesToClipboard(SkillEditor.SelectedStates);
//            EditorCommands.DeleteMultiSelection();
//        }
//        public static void CopyStateSelection()
//        {
//            SkillEditor.Builder.CopyStatesToClipboard(SkillEditor.SelectedStates);
//        }
//        public static void CopyFsm()
//        {
//            SkillBuilder.CopyFsmToClipboard(SkillEditor.SelectedFsm);
//        }
//        public static void PasteFsm()
//        {
//            SkillEditor.SelectFsm(SkillBuilder.PasteFsmToSelected());
//        }
//        public static void SaveFsmAsTemplate()
//        {
//            SkillTemplate fsmTemplate = SkillBuilder.CreateTemplate(SkillEditor.SelectedFsm);
//            if (fsmTemplate != null && SkillEditor.SelectedFsmComponent != null && Dialogs.YesNoDialog(Strings.get_Dialog_Save_Template(), Strings.get_Dialog_Use_Saved_Template_in_this_FSM()))
//            {
//                SkillEditor.SelectedFsmComponent.SetFsmTemplate(fsmTemplate);
//                SkillEditor.Inspector.SetMode(InspectorMode.FsmInspector);
//            }
//        }
//        public static void SaveSelectionAsTemplate()
//        {
//            SkillEditor.Builder.CreateTemplate(SkillEditor.SelectedStates);
//        }
//        public static void AddTemplateToSelected(object userdata)
//        {
//            SkillEditor.SelectFsm(SkillBuilder.AddTemplateToSelected(userdata as SkillTemplate));
//        }
//        public static void AddFsmAndUseTemplateWithSelected(object userdata)
//        {
//            SkillBuilder.AddFsmToSelected(userdata as SkillTemplate);
//        }
//        public static void AddTemplate(object userdata)
//        {
//            SkillEditor.SelectFsm(SkillBuilder.AddTemplate(userdata as SkillTemplate));
//        }
//        public static void SelectAll()
//        {
//            SkillEditor.Selection.SelectAllStates();
//            SkillEditor.Repaint(false);
//        }
//        public static void PasteStates()
//        {
//            EditorCommands.PasteStates(FsmGraphView.GetViewCenter());
//        }
//        public static void PasteStates(Vector2 position)
//        {
//            if (!SkillEditor.Builder.CanPaste())
//            {
//                return;
//            }
//            SkillEditor.Selection.SelectStates(SkillEditor.Builder.PasteStatesFromClipboard(position), false, false);
//            SkillEditor.Inspector.ResetView();
//            SkillEditor.GraphView.UpdateGraphBounds();
//        }
//        public static void PasteTemplate(Vector2 position)
//        {
//            EditorCommands.PasteTemplate(SkillEditor.SelectedTemplate, position);
//        }
//        public static void PasteTemplate(SkillTemplate template, Vector2 position)
//        {
//            if (template == null)
//            {
//                return;
//            }
//            SkillEditor.Selection.SelectStates(SkillEditor.Builder.PasteStatesFromTemplate(template, position), false, false);
//            SkillEditor.Inspector.ResetView();
//            SkillEditor.GraphView.UpdateGraphSize();
//        }
//        public static void AddTransitionToSelectedState()
//        {
//            EditorCommands.AddTransitionToState(SkillEditor.SelectedState, "");
//        }
//        public static void AddTransitionToSelectedState(string eventName)
//        {
//            EditorCommands.AddTransitionToState(SkillEditor.SelectedState, eventName);
//        }
//        public static void AddTransitionToState(SkillState state, string eventName = "")
//        {
//            SkillEditor.RegisterUndo(Strings.get_Command_Add_Transition());
//            SkillTransition fsmTransition = SkillEditor.Builder.AddTransition(SkillEditor.SelectedState);
//            fsmTransition.set_FsmEvent(SkillEvent.GetFsmEvent(eventName));
//            SkillEditor.Selection.SelectTransition(fsmTransition);
//            SkillEditor.GraphView.UpdateStateSize(SkillEditor.SelectedState);
//            SkillSearch.Update(SkillEditor.SelectedFsm);
//            SkillEditor.SetFsmDirty(true, false);
//        }
//        public static void MakeEventGlobal(string eventName)
//        {
//            SkillEvent fsmEvent = SkillEvent.GetFsmEvent(eventName);
//            SkillEditor.Builder.SetEventIsGlobal(SkillEditor.SelectedFsm, fsmEvent, true);
//            SkillSearch.Update(SkillEditor.SelectedFsm);
//        }
//        public static SkillTransition AddGlobalTransitionToSelectedState()
//        {
//            return EditorCommands.AddGlobalTransition(SkillEditor.SelectedState, null);
//        }
//        public static SkillTransition AddGlobalTransition(SkillState state, SkillEvent fsmEvent)
//        {
//            SkillEditor.RegisterUndo(Strings.get_Command_Add_Global_Transition());
//            SkillTransition result = SkillEditor.Builder.AddGlobalTransition(state, fsmEvent);
//            SkillEditor.GraphView.UpdateStateSize(state);
//            SkillSearch.Update(state.get_Fsm());
//            SkillEditor.SetFsmDirty(state.get_Fsm(), true, false, true);
//            return result;
//        }
//        public static void DeleteGlobalTransition(SkillTransition transition)
//        {
//            SkillEditor.RegisterUndo(Strings.get_Command_Delete_Global_Transition());
//            SkillState transitionState = SkillBuilder.GetTransitionState(SkillEditor.SelectedFsm, transition);
//            SkillEditor.Builder.DeleteGlobalTransition(transition);
//            SkillSearch.Update(SkillEditor.SelectedFsm);
//            SkillEditor.GraphView.UpdateStateSize(transitionState);
//            SkillEditor.SetFsmDirty(true, false);
//            Keyboard.ResetFocus();
//        }
//        public static void SetTransitionEvent(SkillTransition transition, SkillEvent fsmEvent)
//        {
//            SkillEditor.RegisterUndo(Strings.get_Command_Set_Transition_Event());
//            SkillEditor.Builder.SetTransitionEvent(transition, fsmEvent);
//            SkillEditor.GraphView.UpdateStateSize(SkillEditor.SelectedState);
//            SkillSearch.Update(SkillEditor.SelectedFsm);
//            SkillEditor.SetFsmDirty(true, false);
//        }
//        public static void SetTransitionTarget(SkillTransition transition, string toState)
//        {
//            SkillEditor.RegisterUndo(Strings.get_Command_Set_Transition_Target());
//            SkillEditor.Builder.SetTransitionTarget(transition, toState);
//            SkillSearch.Update(SkillEditor.SelectedFsm);
//            SkillEditor.SetFsmDirty(true, false);
//        }
//        public static void DeleteSelectedTransition()
//        {
//            EditorCommands.DeleteTransition(SkillEditor.SelectedState, SkillEditor.SelectedTransition);
//        }
//        public static void DeleteTransition(SkillState state, SkillTransition transition)
//        {
//            SkillEditor.RegisterUndo(Strings.get_Menu_Delete_Transition());
//            SkillEditor.Builder.DeleteTransition(state, transition);
//            SkillEditor.GraphView.UpdateStateSize(state);
//            SkillSearch.Update(state.get_Fsm());
//            SkillEditor.SetFsmDirty(state.get_Fsm(), true, false, true);
//        }
//        public static void DeleteSelectedState()
//        {
//            if (!Dialogs.AreYouSure(Strings.get_Command_Delete_State()))
//            {
//                return;
//            }
//            SkillState selectedState = SkillEditor.SelectedState;
//            SkillEditor.Selection.RemoveState(selectedState);
//            SkillEditor.RegisterUndo(Strings.get_Command_Delete_State());
//            SkillEditor.Builder.DeleteState(selectedState);
//            SkillEditor.GraphView.UpdateGraphSize();
//            SkillEditor.UpdateActionUsage();
//            SkillEditor.UpdateFsmInfo();
//            SkillEditor.SetFsmDirty(true, false);
//            SkillEditor.Selection.SelectState(null, false, false);
//        }
//        public static void DeleteMultiSelection()
//        {
//            SkillEditor.RegisterUndo(Strings.get_Menu_GraphView_Delete_States());
//            SkillEditor.Builder.DeleteStates(SkillEditor.SelectedStates);
//            SkillEditor.Selection.DeselectStates();
//            SkillEditor.GraphView.UpdateGraphSize();
//            SkillEditor.UpdateActionUsage();
//            SkillEditor.UpdateFsmInfo();
//            SkillEditor.SetFsmDirty(true, false);
//            if (Event.get_current() != null)
//            {
//                GUIUtility.ExitGUI();
//            }
//        }
//        public static void SetSelectedStateAsStartState()
//        {
//            SkillEditor.RegisterUndo(Strings.get_Command_Set_Start_State());
//            SkillEditor.Builder.SetStartState(SkillEditor.SelectedState.get_Name());
//            SkillEditor.SetFsmDirty(true, false);
//        }
//        public static void SetStateColorIndex(object userdata)
//        {
//            int colorIndex = (int)userdata;
//            SkillEditor.RegisterUndo(Strings.get_Command_Set_State_Color());
//            SkillBuilder.SetStateColorIndex(SkillEditor.SelectedState, colorIndex, true);
//            SkillEditor.SetFsmDirty(false, false);
//        }
//        public static void SetSelectedStatesColorIndex(object userdata)
//        {
//            int selectedStatesColorIndex = (int)userdata;
//            SkillEditor.RegisterUndo(Strings.get_Command_Set_Selected_States_Color());
//            SkillBuilder.SetSelectedStatesColorIndex(selectedStatesColorIndex);
//            SkillEditor.SetFsmDirty(false, false);
//        }
//        public static void SetTransitionColorIndex(object userdata)
//        {
//            int colorIndex = (int)userdata;
//            SkillBuilder.SetTransitionColorIndex(SkillEditor.SelectedFsm, SkillEditor.SelectedTransition, colorIndex, true);
//        }
//        public static void ToggleBreakpointOnSelectedState()
//        {
//            if (SkillEditor.SelectedState == null)
//            {
//                return;
//            }
//            EditorCommands.ToggleBreakpoint(SkillEditor.SelectedState);
//        }
//        public static void SetTransitionLinkStyle(object userdata)
//        {
//            FsmGraphView.SetLinkStyle(SkillEditor.SelectedTransition, (SkillTransition.CustomLinkStyle)userdata);
//        }
//        public static void SetTransitionLinkConstraint(object userdata)
//        {
//            FsmGraphView.SetLinkConstraint(SkillEditor.SelectedTransition, (SkillTransition.CustomLinkConstraint)userdata);
//        }
//        public static void MoveTransitionUp(object userdata)
//        {
//            if (userdata == null)
//            {
//                return;
//            }
//            SkillEditor.RegisterUndo(Strings.get_Menu_Move_Transition_Up());
//            SkillEditor.Builder.MoveTransitionUp(SkillEditor.SelectedState, (SkillTransition)userdata);
//            SkillEditor.SetFsmDirty(true, false);
//        }
//        public static void MoveTransitionDown(object userdata)
//        {
//            if (userdata == null)
//            {
//                return;
//            }
//            SkillEditor.RegisterUndo(Strings.get_Menu_Move_Transition_Down());
//            SkillEditor.Builder.MoveTransitionDown(SkillEditor.SelectedState, (SkillTransition)userdata);
//            SkillEditor.SetFsmDirty(true, false);
//        }
//        public static void AddExposedEvent(Skill fsm, SkillEvent fsmEvent)
//        {
//            SkillEditor.RegisterUndo("Set Inspector Flag");
//            SkillBuilder.AddExposedEvent(fsm, fsmEvent);
//            SkillEditor.SetFsmDirty(false, false);
//        }
//        public static void RemoveExposedEvent(Skill fsm, SkillEvent fsmEvent)
//        {
//            SkillEditor.RegisterUndo("Set Inspector Flag");
//            SkillBuilder.RemoveExposedEvent(fsm, fsmEvent);
//            SkillEditor.SetFsmDirty(false, false);
//        }
//        public static SkillEvent AddEvent(string eventName)
//        {
//            SkillEditor.RegisterUndo(Strings.get_Command_Add_Event());
//            SkillEvent result = SkillEditor.Builder.AddEvent(eventName);
//            SkillEditor.SetFsmDirty(true, false);
//            return result;
//        }
//        public static void OpenWikiHelp()
//        {
//            EditorCommands.OpenWikiPage(WikiPages.Home);
//        }
//        [Localizable(false)]
//        public static void SearchWikiHelp(SkillStateAction action)
//        {
//            string text = Labels.NicifyVariableName(Labels.StripNamespace(action.ToString()));
//            Application.OpenURL("https://hutonggames.fogbugz.com/default.asp?ixWiki=1&pg=pgSearchWiki&qWiki=title:" + text);
//        }
//        [Localizable(false)]
//        public static void SearchWikiHelp(string topic)
//        {
//            Application.OpenURL("https://hutonggames.fogbugz.com/default.asp?ixWiki=1&pg=pgSearchWiki&qWiki=" + topic);
//        }
//        public static void OpenWikiPage(SkillStateAction action)
//        {
//            HelpUrlAttribute attribute = CustomAttributeHelpers.GetAttribute<HelpUrlAttribute>(action.GetType());
//            if (attribute != null)
//            {
//                Application.OpenURL(attribute.get_Url());
//                return;
//            }
//            string topic = Labels.StripNamespace(action.ToString());
//            if (!EditorCommands.OpenWikiPage(topic))
//            {
//                EditorCommands.SearchWikiHelp(action);
//            }
//        }
//        public static void ToggleLogging()
//        {
//            FsmEditorSettings.EnableLogging = !FsmEditorSettings.EnableLogging;
//            SkillLog.set_LoggingEnabled(FsmEditorSettings.EnableLogging);
//            FsmEditorSettings.SaveSettings();
//        }
//        public static bool OpenWikiPage(string topic)
//        {
//            int wikiPageNumber = EditorCommands.GetWikiPageNumber(topic);
//            if (wikiPageNumber > 0)
//            {
//                EditorCommands.OpenWikiPage(wikiPageNumber);
//                return true;
//            }
//            return false;
//        }
//        public static int GetWikiPageNumber(string topic)
//        {
//            if (Enum.IsDefined(typeof(WikiPages), topic))
//            {
//                return (int)Enum.Parse(typeof(WikiPages), topic, true);
//            }
//            return 0;
//        }
//        [Localizable(false)]
//        public static void OpenWikiPage(int page)
//        {
//            Application.OpenURL("https://hutonggames.fogbugz.com/default.asp?W" + page);
//        }
//        public static void OpenWikiPage(WikiPages page)
//        {
//            EditorCommands.OpenWikiPage((int)page);
//        }
//        public static void OpenProductWebPage()
//        {
//            Application.OpenURL("http://hutonggames.com/");
//        }
//        public static void OpenOnlineStore()
//        {
//            Application.OpenURL("http://www.hutonggames.com/store.html");
//        }
//        public static void OpenAssetStorePage()
//        {
//            AssetStore.Open("http://u3d.as/content/hutong-games-llc/playmaker/1Az");
//        }
//        public static void RemoveFsmComponent()
//        {
//            if (SkillEditor.SelectedFsm != null)
//            {
//                Undo.DestroyObjectImmediate(SkillEditor.SelectedFsm.get_Owner());
//                SkillEditor.SelectNone();
//            }
//            SkillEditor.RebuildFsmList();
//        }
//        public static void DeleteTemplate()
//        {
//            if (SkillEditor.SelectedTemplate != null && Dialogs.YesNoDialog(Strings.get_Command_DeleteTemplate()))
//            {
//                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(SkillEditor.SelectedTemplate));
//                Templates.InitList();
//            }
//        }
//        public static void ToggleIsSequence(object userData)
//        {
//            EditorCommands.ToggleIsSequence(userData as SkillState);
//        }
//        public static void ToggleIsSequence(SkillState state)
//        {
//            if (state == null)
//            {
//                return;
//            }
//            SkillEditor.RegisterUndo("Toggle Sequence");
//            state.set_IsSequence(!state.get_IsSequence());
//            SkillEditor.SetFsmDirty(false, false);
//        }
//        public static void ToggleBreakpoint(SkillState state)
//        {
//            if (state == null)
//            {
//                return;
//            }
//            SkillEditor.RegisterUndo(Strings.get_Menu_GraphView_Toggle_Breakpoint());
//            state.set_IsBreakpoint(!state.get_IsBreakpoint());
//            SkillEditor.SetFsmDirty(false, false);
//        }
//        public static void ClearBreakpoints()
//        {
//            SkillEditor.RegisterUndo(Strings.get_Command_Clear_Breakpoints());
//            SkillState[] states = SkillEditor.SelectedFsm.get_States();
//            for (int i = 0; i < states.Length; i++)
//            {
//                SkillState fsmState = states[i];
//                fsmState.set_IsBreakpoint(false);
//            }
//            SkillEditor.SetFsmDirty(false, false);
//        }
//        public static void ToggleAutoRefreshFsmInfo()
//        {
//            FsmEditorSettings.AutoRefreshFsmInfo = !FsmEditorSettings.AutoRefreshFsmInfo;
//            FsmEditorSettings.SaveSettings();
//        }
//        public static void ToggleSelectNewVariables()
//        {
//            FsmEditorSettings.SelectNewVariables = !FsmEditorSettings.SelectNewVariables;
//            FsmEditorSettings.SaveSettings();
//        }
//        public static void ToggleDebugVariables()
//        {
//            FsmEditorSettings.DebugVariables = !FsmEditorSettings.DebugVariables;
//            FsmEditorSettings.SaveSettings();
//        }
//        public static void ChooseWatermark()
//        {
//            SkillEditor.Inspector.SetMode(InspectorMode.Watermarks);
//        }
//        public static void ToggleShowHints()
//        {
//            FsmEditorSettings.ShowHints = !FsmEditorSettings.ShowHints;
//            FsmEditorSettings.SaveSettings();
//            SkillEditor.RepaintAll();
//        }
//        public static void UpdateGraphView()
//        {
//            SkillEditor.GraphView.UpdateStateSizes(SkillEditor.SelectedFsm);
//            SkillEditor.GraphView.UpdateGraphBounds();
//        }
//        public static void AutoNameAction(SkillStateAction action)
//        {
//            if (action == null)
//            {
//                return;
//            }
//            SkillEditor.RegisterUndo("Auto Name Action");
//            action.set_IsAutoNamed(!action.get_IsAutoNamed());
//            action.set_Name(action.get_IsAutoNamed() ? action.AutoName() : null);
//            SkillEditor.SaveActions();
//            SkillEditor.SetFsmDirty();
//        }
//        public static void OpenAction(SkillStateAction action, bool openState)
//        {
//            if (action == null)
//            {
//                return;
//            }
//            action.set_IsOpen(openState);
//            SkillEditor.SaveActions(action.get_State(), false);
//        }
//        public static void OpenAllActions(SkillState state, bool openState)
//        {
//            SkillStateAction[] actions = state.get_Actions();
//            for (int i = 0; i < actions.Length; i++)
//            {
//                SkillStateAction fsmStateAction = actions[i];
//                fsmStateAction.set_IsOpen(openState);
//            }
//            SkillEditor.SaveActions(state, false);
//        }
//        public static void EnableAction(SkillStateAction action, bool enabledState)
//        {
//            if (action == null)
//            {
//                return;
//            }
//            SkillEditor.RegisterUndo(Strings.get_Command_Enable_Action());
//            action.set_Enabled(enabledState);
//            SkillEditor.SaveActions(action.get_State(), true);
//        }
//        public static void EnableAllActions(SkillState state, bool enabledState)
//        {
//            SkillEditor.RegisterUndo(Strings.get_Command_Enable_All_Actions());
//            SkillStateAction[] actions = state.get_Actions();
//            for (int i = 0; i < actions.Length; i++)
//            {
//                SkillStateAction fsmStateAction = actions[i];
//                fsmStateAction.set_Enabled(enabledState);
//            }
//            SkillEditor.SaveActions(state, true);
//        }
//        public static void ResetAction(SkillStateAction action)
//        {
//            if (action == null)
//            {
//                return;
//            }
//            SkillEditor.RegisterUndo(Strings.get_Command_Reset_Action());
//            Keyboard.ResetFocus();
//            action.Reset();
//            SkillEditor.SaveActions(action.get_State(), true);
//            Actions.UpdateTooltip(action);
//        }
//        public static void ResetActionName(SkillStateAction action)
//        {
//            if (action == null)
//            {
//                return;
//            }
//            SkillEditor.RegisterUndo(Strings.get_Command_Reset_Action_Name());
//            action.set_Name(action.get_IsAutoNamed() ? action.AutoName() : null);
//            SkillEditor.SaveActions(action.get_State(), false);
//            Actions.UpdateTooltip(action);
//        }
//        public static void RenameAction(SkillStateAction action, string newName)
//        {
//            if (action == null)
//            {
//                return;
//            }
//            SkillEditor.RegisterUndo(Strings.get_Command_Rename_Action());
//            action.set_Name(newName);
//            SkillEditor.SaveActions(action.get_State(), false);
//            Actions.UpdateTooltip(action);
//        }
//        public static void MoveActionUp(SkillState state, SkillStateAction action)
//        {
//            if (state == null || action == null)
//            {
//                return;
//            }
//            List<SkillStateAction> list = new List<SkillStateAction>(state.get_Actions());
//            int num = list.IndexOf(action);
//            if (num == 0)
//            {
//                return;
//            }
//            SkillEditor.RegisterUndo(Strings.get_Command_Move_Action_Up());
//            list.Remove(action);
//            list.Insert(num - 1, action);
//            state.set_Actions(list.ToArray());
//            SkillEditor.SaveActions(action.get_State(), false);
//        }
//        public static void MoveActionBefore(SkillState state, SkillStateAction action, SkillStateAction beforeAction)
//        {
//            if (state == null || action == null || beforeAction == null)
//            {
//                return;
//            }
//            List<SkillStateAction> list = new List<SkillStateAction>(state.get_Actions());
//            list.Remove(action);
//            int num = Mathf.Clamp(list.IndexOf(beforeAction), 0, list.get_Count() - 1);
//            list.Insert(num, action);
//            state.set_Actions(list.ToArray());
//            SkillEditor.SaveActions(action.get_State(), false);
//        }
//        public static void MoveActionAfter(SkillState state, SkillStateAction action, SkillStateAction afterAction)
//        {
//            if (state == null || action == null || afterAction == null)
//            {
//                return;
//            }
//            List<SkillStateAction> list = new List<SkillStateAction>(state.get_Actions());
//            list.Remove(action);
//            int num = Mathf.Clamp(list.IndexOf(afterAction) + 1, 0, list.get_Count());
//            list.Insert(num, action);
//            state.set_Actions(list.ToArray());
//            SkillEditor.SaveActions(action.get_State(), false);
//        }
//        public static void MoveActionDown(SkillState state, SkillStateAction action)
//        {
//            if (state == null || action == null)
//            {
//                return;
//            }
//            List<SkillStateAction> list = new List<SkillStateAction>(state.get_Actions());
//            int num = list.IndexOf(action);
//            if (num == state.get_Actions().Length - 1)
//            {
//                return;
//            }
//            SkillEditor.RegisterUndo(Strings.get_Command_Move_Action_Down());
//            list.Remove(action);
//            list.Insert(num + 1, action);
//            state.set_Actions(list.ToArray());
//            SkillEditor.SaveActions(action.get_State(), false);
//        }
//        public static void MoveActionToTop(SkillState state, SkillStateAction action)
//        {
//            if (state == null || action == null)
//            {
//                return;
//            }
//            List<SkillStateAction> list = new List<SkillStateAction>(state.get_Actions());
//            if (list.IndexOf(action) == 0)
//            {
//                return;
//            }
//            SkillEditor.RegisterUndo(Strings.get_Menu_Move_Action_To_Top());
//            list.Remove(action);
//            list.Insert(0, action);
//            state.set_Actions(list.ToArray());
//            SkillEditor.SaveActions(action.get_State(), false);
//        }
//        public static void MoveActionToBottom(SkillState state, SkillStateAction action)
//        {
//            if (state == null || action == null)
//            {
//                return;
//            }
//            List<SkillStateAction> list = new List<SkillStateAction>(state.get_Actions());
//            int num = list.IndexOf(action);
//            if (num == state.get_Actions().Length - 1)
//            {
//                return;
//            }
//            SkillEditor.RegisterUndo(Strings.get_Menu_Move_Action_To_Bottom());
//            list.Remove(action);
//            list.Add(action);
//            state.set_Actions(list.ToArray());
//            SkillEditor.SaveActions(action.get_State(), false);
//        }
//        public static void DeleteEvent(SkillEvent fsmEvent)
//        {
//            SkillEditor.RegisterUndo(Strings.get_Command_Delete_Event());
//            SkillEditor.Builder.DeleteEvent(fsmEvent);
//            SkillEditor.GraphView.UpdateStateSizes(SkillEditor.SelectedFsm);
//            SkillEditor.SaveActions(SkillEditor.SelectedFsm);
//            Keyboard.ResetFocus();
//        }
//        public static void DeleteEventFromAll(SkillEvent fsmEvent)
//        {
//            if (FsmEditorSettings.LoadAllPrefabs)
//            {
//                Files.LoadAllPlaymakerPrefabs();
//            }
//            using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
//            {
//                while (enumerator.MoveNext())
//                {
//                    Skill current = enumerator.get_Current();
//                    if (SkillPrefabs.ShouldModify(current))
//                    {
//                        Undo.RecordObject(current.get_OwnerObject(), Strings.get_Command_Delete_Event_From_All_FSMs());
//                        SkillEditor.Builder.DeleteEvent(current, fsmEvent);
//                        SkillEditor.SaveActions(current);
//                    }
//                }
//            }
//            SkillEditor.RepaintAll();
//        }
//        public static NamedVariable AddVariable(VariableType type, string name)
//        {
//            SkillEditor.RegisterUndo(Strings.get_Command_Add_Variable());
//            NamedVariable result = SkillEditor.Builder.AddVariable(type, name);
//            SkillEditor.SetFsmDirty(true, false);
//            return result;
//        }
//        public static Component AddComponent(GameObject go, Type componentType)
//        {
//            if (go == null || componentType == null)
//            {
//                return null;
//            }
//            return go.AddComponent(componentType);
//        }
//    }
//}
