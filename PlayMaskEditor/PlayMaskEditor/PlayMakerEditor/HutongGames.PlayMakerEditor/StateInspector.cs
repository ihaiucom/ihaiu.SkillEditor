using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	internal class StateInspector
	{
		private const string editActionNameControl = "EditActionName";
		private static SkillStateAction mouseOverAction;
		private static bool mouseOverActionTitlebar;
		private static Rect mouseOverActionRect;
		private static bool mouseOverActionPanel;
		private static List<SkillStateAction> selectedActions = new List<SkillStateAction>();
		private static SkillStateAction lastSelectedAction;
		private static SkillStateAction editingActionName;
		private string nameError;
		private static string newActionName;
		private Vector2 mousePos;
		private Vector2 mousePosInScrollView;
		private Rect actionPanelRect;
		private Type draggingNewAction;
		public Vector2 scrollPosition;
		private Rect scrollViewRect;
		private Rect selectedActionRect;
		private Rect lastActionRect;
		private bool autoScroll;
		private bool editingActions;
		private bool actionsDirty;
		private bool takeActionScreenshots;
		private int activeActionIndex;
		private static bool focusControl;
		public static List<SkillStateAction> SelectedActions
		{
			get
			{
				List<SkillStateAction> arg_14_0;
				if ((arg_14_0 = StateInspector.selectedActions) == null)
				{
					arg_14_0 = (StateInspector.selectedActions = new List<SkillStateAction>());
				}
				return arg_14_0;
			}
		}
		public SkillStateAction SelectedAction
		{
			get
			{
				if (StateInspector.SelectedActions.get_Count() != 1)
				{
					return null;
				}
				return StateInspector.SelectedActions.get_Item(0);
			}
		}
		public static bool ShowRequiredFieldFootnote
		{
			get;
			set;
		}
		public static Rect ActionsPanelRect
		{
			get;
			private set;
		}
		public void Reset()
		{
			StateInspector.SanityCheckActionSelection();
		}
		public static void SanityCheckActionSelection()
		{
			if (StateInspector.selectedActions == null)
			{
				StateInspector.selectedActions = new List<SkillStateAction>();
				return;
			}
			if (SkillEditor.SelectedFsm == null || SkillEditor.SelectedState == null)
			{
				StateInspector.selectedActions.Clear();
				return;
			}
			List<SkillStateAction> list = new List<SkillStateAction>();
			using (List<SkillStateAction>.Enumerator enumerator = StateInspector.selectedActions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillStateAction current = enumerator.get_Current();
					if (!ArrayUtility.Contains<SkillStateAction>(SkillEditor.SelectedState.get_Actions(), current))
					{
						list.Add(current);
					}
				}
			}
			using (List<SkillStateAction>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					SkillStateAction current2 = enumerator2.get_Current();
					StateInspector.selectedActions.Remove(current2);
					if (StateInspector.lastSelectedAction == current2)
					{
						StateInspector.lastSelectedAction = null;
					}
				}
			}
		}
		public void EditingActions()
		{
			this.editingActions = true;
		}
		public void Update()
		{
			StateInspector.ShowRequiredFieldFootnote = false;
			this.actionsDirty |= SkillEditor.ActionEditor.Update();
			if (this.actionsDirty && !this.editingActions)
			{
				if (SkillEditor.SelectedState != null)
				{
					SkillEditor.SaveActions(SkillEditor.SelectedState, true);
					SkillStateAction[] actions = SkillEditor.SelectedState.get_Actions();
					for (int i = 0; i < actions.Length; i++)
					{
						SkillStateAction fsmStateAction = actions[i];
						if (fsmStateAction.get_IsAutoNamed())
						{
							fsmStateAction.set_Name(fsmStateAction.AutoName());
						}
					}
				}
				SkillEditor.SetFsmDirty(true, false);
				this.actionsDirty = false;
			}
			this.editingActions = false;
			SkillState selectedState = SkillEditor.SelectedState;
			if (selectedState != null && selectedState.get_IsSequence() && selectedState.get_ActiveActionIndex() != this.activeActionIndex)
			{
				this.activeActionIndex = selectedState.get_ActiveActionIndex();
				SkillEditor.Repaint(true);
			}
		}
		public void OnGUI()
		{
			if (!StateInspector.ValidateSelection())
			{
				return;
			}
			if (Event.get_current().get_isMouse())
			{
				this.mousePos = Event.get_current().get_mousePosition();
				if (DragAndDropManager.mode != DragAndDropManager.DragMode.None)
				{
					DragAndDropManager.Reset();
				}
			}
			if (Event.get_current().get_type() == 9)
			{
				this.mousePos = Event.get_current().get_mousePosition();
			}
			this.DoStateNameGUI(SkillEditor.SelectedState);
			this.DoActionsPanel();
			StateInspector.DoBottomPanel();
			this.HandeKeyboardInput();
			this.HandleDragAndDrop();
			if (Event.get_current().get_type() == null)
			{
				this.DeselectActions();
				GUIUtility.set_keyboardControl(0);
				if (Event.get_current().get_clickCount() > 1)
				{
					SkillEditor.OpenActionWindow();
				}
			}
		}
		private static bool ValidateSelection()
		{
			if (SkillEditor.SelectedFsm == null)
			{
				GUILayout.FlexibleSpace();
				return false;
			}
			if (SkillEditor.SelectedState == null)
			{
				if (FsmEditorSettings.ShowHints)
				{
					GUILayout.Box(Strings.get_Hint_State_Inspector(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
				}
				if (Event.get_current().get_type() == 16)
				{
					SkillEditorGUILayout.GenerateStateSelectionMenu(SkillEditor.SelectedFsm).ShowAsContext();
				}
				GUILayout.FlexibleSpace();
				return false;
			}
			return true;
		}
		private void DoStateNameGUI(SkillState state)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			string text = EditorGUILayout.TextField(state.get_Name(), new GUILayoutOption[0]);
			if (text != state.get_Name())
			{
				this.nameError = SkillEditor.Builder.ValidateNewStateName(state, text);
				if (this.nameError == "")
				{
					EditorCommands.RenameState(state, text);
				}
			}
			Color color = GUI.get_color();
			GUI.set_color(PlayMakerPrefs.get_Colors()[state.get_ColorIndex()]);
			if (GUILayout.Button(GUIContent.none, SkillEditorStyles.ColorSwatch, new GUILayoutOption[]
			{
				GUILayout.Width(15f),
				GUILayout.Height(14f)
			}))
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu = Menus.AddColorMenu(genericMenu, "", state.get_ColorIndex(), new GenericMenu.MenuFunction2(EditorCommands.SetSelectedStatesColorIndex));
				genericMenu.ShowAsContext();
			}
			GUI.set_color(color);
			if (SkillEditorGUILayout.SettingsButtonPadded())
			{
				StateInspector.DoStateSettingsMenu();
			}
			GUILayout.EndHorizontal();
			if (!string.IsNullOrEmpty(this.nameError))
			{
				GUILayout.Box(this.nameError, SkillEditorStyles.ErrorBox, new GUILayoutOption[0]);
			}
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_State_Inspector_Workflow(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			if (FsmEditorSettings.ShowStateDescription)
			{
				string text2 = SkillEditorGUILayout.TextAreaWithHint(state.get_Description(), Strings.get_Label_Description___(), new GUILayoutOption[]
				{
					GUILayout.MinHeight(44f)
				});
				if (text2 != state.get_Description())
				{
					state.set_Description(text2);
					SkillEditor.GraphView.UpdateStateSize(state);
					SkillEditor.SetFsmDirty(false, false);
				}
			}
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
		}
		private void DoActionsPanel()
		{
			if (DebugFlow.ActiveAndScrubbing)
			{
				GUILayout.Box((FsmEditorSettings.EnableDebugFlow && SkillEditor.SelectedFsmDebugFlowEnabled) ? Strings.get_Hint_DebugFlow() : Strings.get_Hint_DebugFlow_Disabled(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			this.DoActionsListGUI(SkillEditor.SelectedState);
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			if (Event.get_current().get_type() == 7)
			{
				StateInspector.ActionsPanelRect = GUILayoutUtility.GetLastRect();
			}
			EventType type = Event.get_current().get_type();
			EventType eventType = type;
			if (eventType != 7)
			{
				if (eventType == 16)
				{
					StateInspector.DoStateSettingsMenu();
				}
			}
			else
			{
				this.actionPanelRect = GUILayoutUtility.GetLastRect();
				if (this.takeActionScreenshots)
				{
					this.takeActionScreenshots = false;
					DocHelpers.EndStateActionListCapture();
				}
			}
			GUI.set_enabled(true);
		}
		private void DoActionsListGUI(SkillState state)
		{
			if (state.get_Actions().Length > 0)
			{
				SkillStateAction fsmStateAction = state.get_Actions()[0];
				if (fsmStateAction != null && fsmStateAction.GetType() == typeof(SkillStateAction))
				{
					Debug.LogWarning("Reinit FsmStateAction");
					SkillEditor.SelectedFsm.Reinitialize();
				}
			}
			if (FsmEditorSettings.ShowHints)
			{
				if (state.get_Actions().Length == 0)
				{
					GUILayout.Box(Strings.get_Hint_Use_Action_Browser_To_Add_Actions(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
				}
				else
				{
					GUILayout.Box((Application.get_platform() == null) ? Strings.get_Hint_Action_Shortcuts_OSX() : Strings.get_Hint_Action_Shortcuts(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
				}
				if (state.get_IsSequence())
				{
					GUILayout.Box(Strings.get_Hint_Sequence_States(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
				}
				SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			}
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]);
			if (DebugFlow.ActiveAndScrubbing)
			{
				GUI.set_enabled(false);
			}
			GUILayout.Space(1f);
			StateInspector.mouseOverAction = null;
			if (Event.get_current().get_isMouse() || Event.get_current().get_type() == 9)
			{
				this.mousePosInScrollView = Event.get_current().get_mousePosition();
			}
			ActionEditor.FSMEventTargetContextGlobal = null;
			int num = 0;
			SkillEditor.ActionEditor.Reset();
			SkillStateAction[] actions = state.get_Actions();
			for (int i = 0; i < actions.Length; i++)
			{
				SkillStateAction fsmStateAction2 = actions[i];
				if (fsmStateAction2 != null)
				{
					try
					{
						this.DoActionGUI(fsmStateAction2);
					}
					catch (Exception ex)
					{
						if (ex is ExitGUIException)
						{
							throw;
						}
						EditorGUILayout.HelpBox(Strings.get_Error_Editing_Action() + Environment.get_NewLine() + ex.get_Message(), 3);
					}
					if (this.takeActionScreenshots && Event.get_current().get_type() == 7)
					{
						DocHelpers.CaptureStateInspectorAction(GUILayoutUtility.GetLastRect(), fsmStateAction2.ToString(), num);
					}
					num++;
				}
			}
			GUILayout.Space(20f);
			this.DrawInsertLine();
			GUILayout.EndScrollView();
			this.DoAutoScroll();
		}
		private void DoAutoScroll()
		{
			if (Event.get_current().get_type() == 7 && this.autoScroll)
			{
				this.scrollViewRect = GUILayoutUtility.GetLastRect();
				float num = this.scrollPosition.y + this.scrollViewRect.get_height();
				if (this.selectedActionRect.get_y() < this.scrollPosition.y)
				{
					this.scrollPosition.y = this.selectedActionRect.get_y();
				}
				else
				{
					if (this.selectedActionRect.get_yMax() > num)
					{
						this.scrollPosition.y = this.scrollPosition.y + (this.selectedActionRect.get_yMax() - num);
						if (this.scrollPosition.y > this.selectedActionRect.get_y())
						{
							this.scrollPosition.y = this.selectedActionRect.get_y();
						}
					}
				}
				SkillEditor.Repaint(false);
				this.autoScroll = false;
			}
		}
		private static bool IsActionBeingDragged(SkillStateAction action)
		{
			return DragAndDropManager.mode == DragAndDropManager.DragMode.MoveActions && StateInspector.selectedActions.Contains(action);
		}
		private void DoActionGUI(SkillStateAction action)
		{
			if (SkillEditor.SelectedState == null)
			{
				return;
			}
			action.Init(SkillEditor.SelectedState);
			Color color = GUI.get_color();
			if (FsmEditorSettings.DimFinishedActions && !DebugFlow.ActiveAndScrubbing && Application.get_isPlaying() && (!action.get_Active() || !SkillEditor.SelectedState.get_Active()))
			{
				GUI.set_color(new Color(1f, 1f, 1f, 0.3f));
			}
			else
			{
				GUI.set_color(Color.get_white());
				GUI.set_contentColor(Color.get_white());
			}
			EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
			bool enabled = GUI.get_enabled();
			bool flag = StateInspector.IsActionBeingDragged(action);
			if (flag)
			{
				GUI.set_enabled(false);
				GUI.set_color(new Color(1f, 1f, 1f, 0.25f));
			}
			else
			{
				GUI.set_enabled(enabled);
			}
			this.DoActionTitlebar(action, flag);
			if (enabled)
			{
				GUI.set_enabled(action.get_Enabled());
			}
			if (FsmEditorSettings.ShowActionParameters && action.get_IsOpen())
			{
				if (SkillEditor.ActionEditor.OnGUI(action))
				{
					this.actionsDirty = true;
				}
				string text = action.ErrorCheck();
				if (!string.IsNullOrEmpty(text))
				{
					GUILayout.Box(text, SkillEditorStyles.ActionErrorBox, new GUILayoutOption[0]);
				}
				List<string> runtimeErrors = FsmErrorChecker.GetRuntimeErrors(action);
				if (runtimeErrors.get_Count() > 0)
				{
					using (List<string>.Enumerator enumerator = runtimeErrors.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string current = enumerator.get_Current();
							GUILayout.Box(current, SkillEditorStyles.ActionErrorBox, new GUILayoutOption[0]);
						}
					}
				}
			}
			GUI.set_color(color);
			GUI.set_enabled(enabled);
			SkillEditorGUILayout.ResetGUIColors();
			if (!action.get_State().get_IsSequence())
			{
				if (FsmEditorSettings.ShowActionParameters)
				{
					SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
				}
			}
			else
			{
				SkillEditorGUILayout.SequenceDivider(new GUILayoutOption[0]);
			}
			EditorGUILayout.EndVertical();
			this.UpdateMouseOver(action);
			if (StateInspector.mouseOverAction == action && StateInspector.mouseOverActionTitlebar)
			{
				switch (Event.get_current().get_type())
				{
				case 0:
					this.SelectAction(action, false);
					if (Event.get_current().get_clickCount() > 1)
					{
						StateInspector.EditActionName(action);
					}
					Event.get_current().Use();
					return;
				case 1:
					if (!Event.get_current().get_shift())
					{
						StateInspector.selectedActions.Clear();
						StateInspector.AddActionToSelection(StateInspector.lastSelectedAction);
					}
					break;
				default:
					return;
				}
			}
		}
		private void UpdateMouseOver(SkillStateAction action)
		{
			Rect lastRect = GUILayoutUtility.GetLastRect();
			if (lastRect.Contains(this.mousePosInScrollView))
			{
				StateInspector.mouseOverAction = action;
				StateInspector.mouseOverActionTitlebar = (this.mousePosInScrollView.y - lastRect.get_y() < 20f);
				StateInspector.mouseOverActionRect = lastRect;
			}
			if (StateInspector.lastSelectedAction == action)
			{
				this.selectedActionRect = lastRect;
			}
			if (action == SkillEditor.SelectedState.get_Actions()[SkillEditor.SelectedState.get_Actions().Length - 1])
			{
				this.lastActionRect = lastRect;
				if (this.mousePosInScrollView.y > this.lastActionRect.get_y() + this.lastActionRect.get_height())
				{
					StateInspector.mouseOverAction = null;
				}
			}
		}
		private void DoActionTitlebar(SkillStateAction action, bool isBeingDragged)
		{
			SkillEditorGUILayout.ResetGUIColorsKeepAlpha();
			EditorGUILayout.BeginHorizontal((StateInspector.IsActionSelected(action) && !isBeingDragged) ? SkillEditorStyles.SelectedRow : SkillEditorStyles.ActionTitle, new GUILayoutOption[0]);
			if (FsmEditorSettings.ShowActionParameters)
			{
				bool flag = SkillEditorGUILayout.ActionFoldout(action.get_IsOpen());
				if (flag != action.get_IsOpen())
				{
					if (EditorGUI.get_actionKey())
					{
						EditorCommands.OpenAllActions(SkillEditor.SelectedState, flag);
					}
					else
					{
						EditorCommands.OpenAction(action, flag);
					}
					Keyboard.ResetFocus();
				}
			}
			else
			{
				GUILayout.Space(2f);
			}
			Color color = GUI.get_color();
			DrawState drawState = DebugFlow.ActiveAndScrubbing ? DrawState.Normal : FsmDrawState.GetDrawState(SkillEditor.SelectedFsm, SkillEditor.SelectedState, action);
			Color color2 = SkillEditorStyles.ActionColors[(int)drawState];
			GUI.set_color(new Color(color2.r, color2.g, color2.b, color.a));
			bool flag2 = GUILayout.Toggle(action.get_Enabled(), "", SkillEditorStyles.ActionToggle, new GUILayoutOption[0]);
			if (flag2 != action.get_Enabled())
			{
				if (EditorGUI.get_actionKey())
				{
					EditorCommands.EnableAllActions(SkillEditor.SelectedState, flag2);
				}
				else
				{
					EditorCommands.EnableAction(action, flag2);
				}
			}
			string actionLabel = Labels.GetActionLabel(action);
			if (StateInspector.editingActionName == action)
			{
				StateInspector.DoEditActionName(action);
			}
			else
			{
				bool flag3 = FsmErrorChecker.ActionHasErrors(action);
				if (flag3)
				{
					GUILayout.Box(SkillEditorStyles.StateErrorIcon, SkillEditorStyles.InlineErrorIcon, new GUILayoutOption[0]);
				}
				else
				{
					GUILayout.Box(GUIContent.none, GUIStyle.get_none(), new GUILayoutOption[0]);
				}
				GUIStyle gUIStyle = SkillEditorStyles.ActionTitle;
				if (StateInspector.IsActionSelected(action))
				{
					gUIStyle = SkillEditorStyles.ActionTitleSelected;
				}
				GUILayout.Box(actionLabel, gUIStyle, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(250f)
				});
				if (Event.get_current().get_keyCode() == 283 && this.SelectedAction == action)
				{
					StateInspector.EditActionName(action);
					SkillEditor.Repaint(true);
				}
			}
			GUILayout.FlexibleSpace();
			GUI.set_color(color);
			GUI.set_contentColor(Color.get_white());
			if (SkillEditorGUILayout.HelpButtonSmall(Actions.GetTooltip(action)))
			{
				EditorCommands.OpenWikiPage(action);
				this.SelectAction(action, false);
			}
			if (SkillEditorGUILayout.SettingsButton())
			{
				this.DoActionContextMenu(action);
				this.SelectAction(action, false);
			}
			EditorGUILayout.EndHorizontal();
		}
		private static void DoEditActionName(SkillStateAction action)
		{
			GUI.SetNextControlName("EditActionName");
			StateInspector.newActionName = EditorGUILayout.TextField(StateInspector.newActionName, new GUILayoutOption[]
			{
				GUILayout.MinWidth(210f)
			});
			if (StateInspector.focusControl)
			{
				GUI.FocusControl("EditActionName");
				StateInspector.focusControl = false;
			}
			if (GUI.GetNameOfFocusedControl() != "EditActionName")
			{
				StateInspector.CommitActionNameEdit(action);
			}
			if (SkillEditorGUILayout.ResetButton())
			{
				EditorCommands.ResetActionName(action);
				StateInspector.editingActionName = null;
				SkillEditor.Repaint(false);
			}
			if (Event.get_current().get_isKey())
			{
				if (Keyboard.EnterKeyPressed())
				{
					StateInspector.CommitActionNameEdit(action);
				}
				if (Event.get_current().get_keyCode() == 27)
				{
					StateInspector.editingActionName = null;
					SkillEditor.Repaint(false);
				}
			}
		}
		private static void CommitActionNameEdit(SkillStateAction action)
		{
			EditorCommands.RenameAction(action, StateInspector.newActionName);
			StateInspector.editingActionName = null;
			SkillEditor.Repaint(false);
		}
		private void DrawInsertLine()
		{
			if (StateInspector.mouseOverActionPanel && DragAndDropManager.mode != DragAndDropManager.DragMode.None)
			{
				GUI.Box((StateInspector.mouseOverAction != null) ? new Rect(0f, StateInspector.mouseOverActionRect.get_y() - 2f, 350f, 4f) : new Rect(0f, this.lastActionRect.get_y() + this.lastActionRect.get_height(), 350f, 4f), "", SkillEditorStyles.InsertLine);
			}
		}
		private void HandleDragAndDrop()
		{
			StateInspector.mouseOverActionPanel = this.actionPanelRect.Contains(this.mousePos);
			if (!StateInspector.mouseOverActionPanel)
			{
				return;
			}
			EventType type = Event.get_current().get_type();
			if (type == 3 && !SkillEditor.GraphView.IsDragging)
			{
				if (StateInspector.mouseOverAction != null && StateInspector.mouseOverActionTitlebar)
				{
					StateInspector.StartDragSelectedActions();
					return;
				}
			}
			else
			{
				if (type == 9 || type == 10)
				{
					this.UpdateDragAndDrop();
				}
			}
		}
		private void UpdateGenericDrag()
		{
			Object[] objectReferences = DragAndDrop.get_objectReferences();
			if (objectReferences == null || objectReferences.Length == 0)
			{
				return;
			}
			Object @object = objectReferences[0];
			if (@object == null)
			{
				return;
			}
			MonoScript monoScript = @object as MonoScript;
			if (monoScript != null)
			{
				Type @class = monoScript.GetClass();
				if (@class.IsSubclassOf(typeof(SkillStateAction)) && !@class.get_IsAbstract())
				{
					DragAndDropManager.SetMode(DragAndDropManager.DragMode.AddAction);
					DragAndDrop.set_objectReferences(new Object[0]);
					DragAndDrop.SetGenericData("AddAction", @class);
					return;
				}
			}
			DragAndDrop.set_visualMode(1);
			if (Event.get_current().get_type() == 10)
			{
				ActionUtility.ShowObjectContextMenu(SkillEditor.SelectedFsm, SkillEditor.SelectedState, @object, StateInspector.mouseOverAction);
			}
		}
		[Localizable(false)]
		private static void StartDragSelectedActions()
		{
			DragAndDropManager.SetMode(DragAndDropManager.DragMode.MoveActions);
			DragAndDrop.PrepareStartDrag();
			DragAndDrop.set_objectReferences(new Object[0]);
			DragAndDrop.SetGenericData("MoveActions", StateInspector.selectedActions);
			DragAndDrop.StartDrag("MoveActions");
			Event.get_current().Use();
		}
		private void UpdateDragAndDrop()
		{
			switch (DragAndDropManager.mode)
			{
			case DragAndDropManager.DragMode.None:
				this.UpdateGenericDrag();
				return;
			case DragAndDropManager.DragMode.AddAction:
				this.UpdateDragNewAction();
				return;
			case DragAndDropManager.DragMode.MoveActions:
				this.UpdateDragSelectedActions();
				return;
			default:
				return;
			}
		}
		private void UpdateDragSelectedActions()
		{
			DragAndDrop.set_visualMode(16);
			if (Event.get_current().get_type() == 10)
			{
				DragAndDrop.AcceptDrag();
				this.DropSelectedActions();
			}
			Event.get_current().Use();
			SkillEditor.Repaint(false);
		}
		[Localizable(false)]
		private void UpdateDragNewAction()
		{
			DragAndDrop.set_visualMode(1);
			this.draggingNewAction = (Type)DragAndDrop.GetGenericData("AddAction");
			if (Event.get_current().get_type() == 10)
			{
				DragAndDrop.AcceptDrag();
				if (this.draggingNewAction != null)
				{
					this.DropNewAction(this.draggingNewAction);
				}
			}
			Event.get_current().Use();
			SkillEditor.Repaint(false);
		}
		private static void DoBottomPanel()
		{
			SkillEditorGUILayout.ResetGUIColors();
			if (StateInspector.ShowRequiredFieldFootnote)
			{
				SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
				GUILayout.Label(Strings.get_Hint_Required_Fields(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			}
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(5f);
			bool flag = GUILayout.Toggle(FsmEditorSettings.DebugActionParameters, new GUIContent(Strings.get_Label_Debug(), Strings.get_Tooltip_Variables_Debug()), new GUILayoutOption[0]);
			if (flag != FsmEditorSettings.DebugActionParameters)
			{
				FsmEditorSettings.DebugActionParameters = flag;
				FsmEditorSettings.SaveSettings();
			}
			GUILayout.Space(5f);
			bool flag2 = GUILayout.Toggle(SkillEditor.SelectedState.get_HideUnused(), new GUIContent(Strings.get_Label_Hide_Unused(), Strings.get_Tooltip_Hide_Unused()), new GUILayoutOption[0]);
			if (flag2 != SkillEditor.SelectedState.get_HideUnused())
			{
				SkillEditor.SelectedState.set_HideUnused(flag2);
				SkillEditor.SetFsmDirty(false, false);
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent(Strings.get_Label_Action_Browser(), Strings.get_Tooltip_Action_Browser()), new GUILayoutOption[0]))
			{
				SkillEditor.OpenActionWindow();
				GUIUtility.ExitGUI();
			}
			EditorGUILayout.EndHorizontal();
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_Action_Browser_Workflow(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			EditorGUILayout.Space();
		}
		public void ResetScrollPosition()
		{
			this.scrollPosition = Vector2.get_zero();
		}
		private void HandeKeyboardInput()
		{
		}
		private static void DoStateSettingsMenu()
		{
			StateInspector.GenerateStateSettingsMenu().ShowAsContext();
		}
		private static GenericMenu GenerateStateSettingsMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Action_Sequence()), SkillEditor.SelectedState.get_IsSequence(), new GenericMenu.MenuFunction2(EditorCommands.ToggleIsSequence), SkillEditor.SelectedState);
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Show_State_Description()), FsmEditorSettings.ShowStateDescription, new GenericMenu.MenuFunction(StateInspector.ToggleShowStateDescription));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Show_Action_Parameters()), FsmEditorSettings.ShowActionParameters, new GenericMenu.MenuFunction(StateInspector.ToggleShowDetails));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Dim_Unused_Parameters()), FsmEditorSettings.DimUnusedActionParameters, new GenericMenu.MenuFunction(StateInspector.ToggleDimUnusedParameters));
			string root = Strings.get_Menu_Set_State_Color().Split(new char[]
			{
				'/'
			})[0];
			genericMenu = Menus.AddColorMenu(genericMenu, root, SkillEditor.SelectedState.get_ColorIndex(), new GenericMenu.MenuFunction2(EditorCommands.SetStateColorIndex));
			genericMenu.AddSeparator("");
			genericMenu.AddItem(SkillEditorContent.MenuSelectAllActions, false, new GenericMenu.MenuFunction(StateInspector.SelectAllActions));
			if (StateInspector.selectedActions.get_Count() > 0)
			{
				genericMenu.AddItem(SkillEditorContent.MenuCopySelectedActions, false, new GenericMenu.MenuFunction(StateInspector.CopySelectedActions));
			}
			else
			{
				genericMenu.AddDisabledItem(SkillEditorContent.MenuCopySelectedActions);
			}
			if (SkillBuilder.ClipboardNumStates == 1)
			{
				genericMenu.AddItem(SkillEditorContent.MenuPasteActions, false, new GenericMenu.MenuFunction(StateInspector.PasteActions));
			}
			else
			{
				genericMenu.AddDisabledItem(SkillEditorContent.MenuPasteActions);
			}
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Delete_All_Actions()), false, new GenericMenu.MenuFunction(StateInspector.DeleteAllActions));
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Action_Browser()), false, new GenericMenu.MenuFunction(SkillEditor.OpenActionWindow));
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Online_Help()), false, new GenericMenu.MenuFunction(StateInspector.OpenOnlineHelp));
			return genericMenu;
		}
		private static void OpenOnlineHelp()
		{
			EditorCommands.OpenWikiPage(WikiPages.StateInspector);
		}
		private static void ToggleShowStateDescription()
		{
			FsmEditorSettings.ShowStateDescription = !FsmEditorSettings.ShowStateDescription;
			FsmEditorSettings.SaveSettings();
		}
		private static void ToggleShowDetails()
		{
			FsmEditorSettings.ShowActionParameters = !FsmEditorSettings.ShowActionParameters;
			FsmEditorSettings.SaveSettings();
		}
		private static void ToggleDimUnusedParameters()
		{
			FsmEditorSettings.DimUnusedActionParameters = !FsmEditorSettings.DimUnusedActionParameters;
			FsmEditorSettings.SaveSettings();
		}
		private void DoActionContextMenu(SkillStateAction action)
		{
			this.GenerateActionContextMenu(action).ShowAsContext();
		}
		private GenericMenu GenerateActionContextMenu(SkillStateAction action)
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Reset()), false, new GenericMenu.MenuFunction2(StateInspector.ResetAction), action);
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Edit_Name()), false, new GenericMenu.MenuFunction2(StateInspector.EditActionName), action);
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Auto_Name()), action.get_IsAutoNamed(), new GenericMenu.MenuFunction2(StateInspector.AutoName), action);
			genericMenu.AddItem(SkillEditorContent.MenuCopySelectedActions, false, new GenericMenu.MenuFunction(StateInspector.CopySelectedActions));
			if (SkillBuilder.ClipboardNumStates == 1)
			{
				genericMenu.AddItem(new GUIContent(Strings.get_Menu_Paste_Actions_Before()), false, new GenericMenu.MenuFunction(this.PasteActionsBeforeSelected));
				genericMenu.AddItem(new GUIContent(Strings.get_Menu_Paste_Actions_Replace()), false, new GenericMenu.MenuFunction(this.PasteActionsReplaceSelected));
				genericMenu.AddItem(new GUIContent(Strings.get_Menu_Paste_Actions_After()), false, new GenericMenu.MenuFunction(this.PasteActionsAfterSelected));
			}
			else
			{
				genericMenu.AddDisabledItem(SkillEditorContent.MenuPasteActions);
			}
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Move_Action_To_Top()), false, new GenericMenu.MenuFunction2(StateInspector.MoveActionToTop), action);
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Move_Action_To_Bottom()), false, new GenericMenu.MenuFunction2(StateInspector.MoveActionToBottom), action);
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Edit_Script()), false, new GenericMenu.MenuFunction2(ActionScripts.EditAsset), action);
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Select_Script()), false, new GenericMenu.MenuFunction2(ActionScripts.SelectAssetByType), action.GetType());
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Find_Script()), false, new GenericMenu.MenuFunction2(ActionScripts.PingAsset), action);
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Find_Action_In_Browser()), false, new GenericMenu.MenuFunction2(StateInspector.FindActionInBrowser), action);
			if (Actions.CategoryContainsAction("Favorites", action.GetType()))
			{
				genericMenu.AddItem(new GUIContent("Remove From Favorites"), false, new GenericMenu.MenuFunction2(ActionSelector.RemoveFromFavorites), action.GetType());
			}
			else
			{
				genericMenu.AddItem(new GUIContent("Add To Favorites"), false, new GenericMenu.MenuFunction2(ActionSelector.AddToFavorites), action.GetType());
			}
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Remove_Action()), false, new GenericMenu.MenuFunction2(StateInspector.RemoveAction), action);
			return genericMenu;
		}
		private static void AutoName(object userdata)
		{
			SkillStateAction action = (SkillStateAction)userdata;
			EditorCommands.AutoNameAction(action);
		}
		private static void EditActionName(object userdata)
		{
			SkillStateAction action = (SkillStateAction)userdata;
			StateInspector.EditActionName(action);
		}
		private static void EditActionName(SkillStateAction action)
		{
			StateInspector.focusControl = true;
			StateInspector.editingActionName = action;
			StateInspector.newActionName = Labels.GetActionLabel(action);
		}
		private static void ResetAction(object userdata)
		{
			SkillStateAction action = (SkillStateAction)userdata;
			EditorCommands.ResetAction(action);
		}
		private static void RemoveAction(object userdata)
		{
			SkillStateAction action = (SkillStateAction)userdata;
			SkillEditor.Builder.DeleteAction(SkillEditor.SelectedState, action);
		}
		private static void FindActionInBrowser(object userdata)
		{
			SkillStateAction fsmStateAction = (SkillStateAction)userdata;
			ActionSelector.FindAction(fsmStateAction.GetType());
			SkillEditor.OpenActionWindow();
		}
		private static void MoveActionToTop(object userdata)
		{
			SkillStateAction action = (SkillStateAction)userdata;
			EditorCommands.MoveActionToTop(SkillEditor.SelectedState, action);
			Keyboard.ResetFocus();
		}
		private static void MoveActionToBottom(object userdata)
		{
			SkillStateAction action = (SkillStateAction)userdata;
			EditorCommands.MoveActionToBottom(SkillEditor.SelectedState, action);
			Keyboard.ResetFocus();
		}
		public static bool IsActionSelected(SkillStateAction action)
		{
			return StateInspector.SelectedActions.Contains(action);
		}
		public void SelectAction(SkillStateAction action, bool frame = false)
		{
			StateInspector.editingActionName = null;
			StateInspector.SanityCheckActionSelection();
			if (Event.get_current() == null || EditorWindow.get_focusedWindow() != SkillEditor.Window)
			{
				StateInspector.lastSelectedAction = action;
				StateInspector.selectedActions.Clear();
				StateInspector.AddActionToSelection(action);
			}
			else
			{
				if (Event.get_current().get_button() == 1 || EditorGUI.get_actionKey())
				{
					if (!StateInspector.selectedActions.Contains(action))
					{
						StateInspector.lastSelectedAction = action;
						StateInspector.selectedActions.Clear();
						StateInspector.selectedActions.Add(action);
					}
					SkillEditor.Window.Repaint();
					this.DoActionContextMenu(action);
				}
				else
				{
					if (Event.get_current().get_shift())
					{
						if (Event.get_current().get_control())
						{
							StateInspector.RemoveAction(action);
						}
						else
						{
							StateInspector.AddActionToSelection(action);
						}
					}
					else
					{
						if (Event.get_current().get_alt())
						{
							StateInspector.RemoveActionFromSelection(action);
						}
						else
						{
							StateInspector.lastSelectedAction = action;
							if (!StateInspector.selectedActions.Contains(action))
							{
								StateInspector.selectedActions.Clear();
								StateInspector.AddActionToSelection(action);
							}
						}
					}
				}
			}
			if (frame)
			{
				this.autoScroll = true;
			}
			StateInspector.SanityCheckActionSelection();
		}
		private static void AddActionToSelection(SkillStateAction action)
		{
			StateInspector.SanityCheckActionSelection();
			if (StateInspector.lastSelectedAction == null)
			{
				StateInspector.lastSelectedAction = action;
			}
			if (!StateInspector.selectedActions.Contains(action))
			{
				StateInspector.selectedActions.Add(action);
			}
		}
		private static void RemoveActionFromSelection(SkillStateAction action)
		{
			StateInspector.SanityCheckActionSelection();
			if (StateInspector.selectedActions.Contains(action))
			{
				StateInspector.selectedActions.Remove(action);
			}
			if (StateInspector.lastSelectedAction == action)
			{
				StateInspector.lastSelectedAction = null;
			}
		}
		public static void SelectAllActions()
		{
			if (SkillEditor.SelectedState == null)
			{
				return;
			}
			SkillStateAction[] actions = SkillEditor.SelectedState.get_Actions();
			for (int i = 0; i < actions.Length; i++)
			{
				SkillStateAction action = actions[i];
				StateInspector.AddActionToSelection(action);
			}
		}
		public void DeselectActions()
		{
			StateInspector.lastSelectedAction = null;
			StateInspector.SelectedActions.Clear();
			SkillEditor.Repaint(false);
		}
		public void AddAction(Type actionType)
		{
			SkillEditor.Inspector.SetMode(InspectorMode.StateInspector);
			if (this.SelectedAction != null)
			{
				this.AddActionBeforeSelectedAction(actionType);
			}
			else
			{
				this.SelectAction(EditorCommands.AddAction(SkillEditor.SelectedState, actionType), false);
				this.scrollPosition.y = 100000f;
			}
			SkillEditor.RepaintAll();
		}
		public void AddActionToTop(Type actionType)
		{
			SkillEditor.Inspector.SetMode(InspectorMode.StateInspector);
			SkillStateAction fsmStateAction = EditorCommands.AddAction(SkillEditor.SelectedState, actionType);
			StateInspector.MoveActionToTop(fsmStateAction);
			this.SelectAction(fsmStateAction, false);
			this.scrollPosition.y = 0f;
			SkillEditor.RepaintAll();
		}
		public void AddActionToEnd(Type actionType)
		{
			SkillEditor.Inspector.SetMode(InspectorMode.StateInspector);
			this.SelectAction(EditorCommands.AddAction(SkillEditor.SelectedState, actionType), false);
			this.scrollPosition.y = 100000f;
			SkillEditor.RepaintAll();
		}
		public void AddActionBeforeSelectedAction(Type actionType)
		{
			SkillEditor.Inspector.SetMode(InspectorMode.StateInspector);
			SkillStateAction action = EditorCommands.AddAction(SkillEditor.SelectedState, actionType);
			EditorCommands.MoveActionBefore(SkillEditor.SelectedState, action, StateInspector.lastSelectedAction);
			this.SelectAction(action, false);
			SkillEditor.RepaintAll();
		}
		public void AddActionAfterSelectedAction(Type actionType)
		{
			SkillEditor.Inspector.SetMode(InspectorMode.StateInspector);
			SkillStateAction action = EditorCommands.AddAction(SkillEditor.SelectedState, actionType);
			EditorCommands.MoveActionAfter(SkillEditor.SelectedState, action, StateInspector.lastSelectedAction);
			this.SelectAction(action, false);
			SkillEditor.RepaintAll();
		}
		private void DropNewAction(Type actionType)
		{
			SkillStateAction action = EditorCommands.InsertAction(SkillEditor.SelectedState, actionType, StateInspector.mouseOverAction);
			this.SelectAction(action, false);
			this.autoScroll = true;
		}
		private void DropSelectedActions()
		{
			if (StateInspector.IsActionSelected(StateInspector.mouseOverAction))
			{
				DragAndDropManager.Reset();
				return;
			}
			StateInspector.CopySelectedActions();
			if (StateInspector.mouseOverAction == null)
			{
				SkillEditor.RegisterUndo(Strings.get_Command_Move_Actions());
				this.DeleteSelectedActions(false);
				StateInspector.PasteActions(false);
			}
			else
			{
				SkillEditor.RegisterUndo(Strings.get_Command_Move_Actions());
				this.DeleteSelectedActions(false);
				this.PasteActionsBefore(StateInspector.mouseOverAction, false);
			}
			DragAndDropManager.Reset();
		}
		public void DeleteSelectedActions(bool undo = true)
		{
			SkillEditor.Builder.DeleteActions(SkillEditor.SelectedState, StateInspector.selectedActions, undo);
		}
		public static void DeleteAllActions()
		{
			if (SkillEditor.SelectedState.get_Actions().Length == 0 || !Dialogs.AreYouSure(Strings.get_Command_Delete_All_Actions()))
			{
				return;
			}
			SkillEditor.Builder.DeleteActions(SkillEditor.SelectedState, SkillEditor.SelectedState.get_Actions(), true);
		}
		private void PasteActionsBeforeSelected()
		{
			this.PasteActionsBeforeSelected(true);
		}
		private void PasteActionsBeforeSelected(bool undo)
		{
			this.PasteActionsBefore(this.SelectedAction, undo);
		}
		private void PasteActionsBefore(SkillStateAction action, bool undo = true)
		{
			int actionIndex = Actions.GetActionIndex(SkillEditor.SelectedState, action);
			StateInspector.selectedActions = SkillEditor.Builder.PasteActionsFromTemplate(SkillBuilder.Clipboard, SkillEditor.SelectedState, actionIndex, undo);
		}
		private void PasteActionsReplaceSelected()
		{
			this.PasteActionsReplaceSelected(true);
		}
		private void PasteActionsReplaceSelected(bool undo)
		{
			int actionIndex = Actions.GetActionIndex(SkillEditor.SelectedState, this.SelectedAction);
			SkillEditor.SelectedState.set_Actions(ArrayUtility.Remove<SkillStateAction>(SkillEditor.SelectedState.get_Actions(), this.SelectedAction));
			StateInspector.selectedActions = SkillEditor.Builder.PasteActionsFromTemplate(SkillBuilder.Clipboard, SkillEditor.SelectedState, actionIndex, undo);
		}
		public void PasteActionsAfterSelected()
		{
			this.PasteActionsAfterSelected(true);
		}
		public void PasteActionsAfterSelected(bool undo)
		{
			if (this.SelectedAction == null)
			{
				StateInspector.selectedActions = SkillEditor.Builder.PasteActionsFromTemplate(SkillBuilder.Clipboard, SkillEditor.SelectedState, SkillEditor.SelectedState.get_Actions().Length, undo);
				return;
			}
			int actionIndex = Actions.GetActionIndex(SkillEditor.SelectedState, this.SelectedAction);
			StateInspector.selectedActions = SkillEditor.Builder.PasteActionsFromTemplate(SkillBuilder.Clipboard, SkillEditor.SelectedState, actionIndex + 1, undo);
		}
		private static void PasteActions()
		{
			StateInspector.PasteActions(true);
		}
		private static void PasteActions(bool undo)
		{
			StateInspector.selectedActions = SkillEditor.Builder.PasteActionsFromTemplate(SkillBuilder.Clipboard, SkillEditor.SelectedState, SkillEditor.SelectedState.get_Actions().Length, undo);
		}
		public void CutSelectedActions()
		{
			StateInspector.SanityCheckActionSelection();
			StateInspector.CopyActions(StateInspector.SelectedActions);
			this.DeleteSelectedActions(true);
		}
		public static void CopySelectedActions()
		{
			StateInspector.SanityCheckActionSelection();
			StateInspector.CopyActions(StateInspector.SelectedActions);
		}
		private static void CopyActions(List<SkillStateAction> actions)
		{
			if (SkillEditor.SelectedState != null && actions != null && actions.get_Count() > 0)
			{
				SkillEditor.Builder.CopyActionsToClipboard(SkillEditor.SelectedState, actions);
			}
		}
	}
}
