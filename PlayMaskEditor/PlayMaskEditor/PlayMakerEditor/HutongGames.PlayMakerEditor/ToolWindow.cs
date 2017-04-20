using HutongGames.PlayMaker;
using System;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class ToolWindow : BaseEditorWindow
	{
		private static SkillTransition editingTransition;
		private Vector2 scrollPosition;
		public override void Initialize()
		{
			this.isToolWindow = true;
			this.InitWindowTitle();
			base.set_minSize(new Vector2(200f, 100f));
			base.set_maxSize(new Vector2(300f, 2000f));
		}
		public void InitWindowTitle()
		{
			base.SetTitle(Strings.get_ToolWindow_Title());
		}
		public override void DoGUI()
		{
			if (SkillEditor.SelectedFsm == null)
			{
				GUILayout.Label(Strings.get_Hint_Select_FSM(), new GUILayoutOption[0]);
				return;
			}
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_Context_Tools(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]);
			if (SkillEditor.SelectedTransition != null)
			{
				ToolWindow.TransitionTools();
			}
			else
			{
				if (SkillEditor.SelectedState != null)
				{
					ToolWindow.StateTools();
				}
				else
				{
					if (SkillEditor.SelectedFsm != null)
					{
						ToolWindow.FsmTools();
					}
				}
			}
			GUILayout.EndScrollView();
		}
		private static void FsmTools()
		{
			ToolWindow.Header(Strings.get_ToolWindow_Header_FSM_Tools());
			if (GUILayout.Button(Strings.get_Command_Add_New_State(), new GUILayoutOption[0]))
			{
				SkillEditor.GraphView.AddState(FsmGraphView.GetViewCenter());
				SkillEditor.RepaintAll();
			}
			EditorGUI.BeginDisabledGroup(!SkillEditor.Builder.CanPaste());
			if (GUILayout.Button(Strings.get_Command_Paste_States(), new GUILayoutOption[0]))
			{
				EditorCommands.PasteStates(FsmGraphView.GetViewCenter());
				SkillEditor.RepaintAll();
			}
			EditorGUI.EndDisabledGroup();
		}
		private static void StateTools()
		{
			ToolWindow.Header(Strings.get_ToolWindow_Header_State_Tools());
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button(Strings.get_Command_Copy(), new GUILayoutOption[0]))
			{
				EditorCommands.CopyStateSelection();
			}
			EditorGUI.BeginDisabledGroup(!SkillEditor.Builder.CanPaste());
			if (GUILayout.Button(Strings.get_Command_Paste(), new GUILayoutOption[0]))
			{
				EditorCommands.PasteStates(FsmGraphView.GetViewCenter());
				SkillEditor.RepaintAll();
			}
			EditorGUI.EndDisabledGroup();
			if (GUILayout.Button(Strings.get_Command_Delete(), new GUILayoutOption[0]))
			{
				SkillEditor.RepaintAll();
				EditorCommands.DeleteMultiSelection();
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button(Strings.get_Command_Save_Selection_as_Template(), new GUILayoutOption[0]))
			{
				EditorCommands.SaveSelectionAsTemplate();
			}
			if (GUILayout.Button(Strings.get_Command_Set_As_Start_State(), new GUILayoutOption[0]))
			{
				EditorCommands.SetSelectedStateAsStartState();
				SkillEditor.RepaintAll();
			}
			if (GUILayout.Button(Strings.get_Command_Toggle_Breakpoint(), new GUILayoutOption[0]))
			{
				EditorCommands.ToggleBreakpointOnSelectedState();
				SkillEditor.RepaintAll();
			}
			ToolWindow.Divider(Strings.get_ToolWindow_Header_Transitions());
			if (GUILayout.Button(Strings.get_Command_Add_Transition(), new GUILayoutOption[0]))
			{
				EditorCommands.AddTransitionToSelectedState();
				SkillEditor.RepaintAll();
			}
			if (GUILayout.Button(Strings.get_Command_Add_Global_Transition(), new GUILayoutOption[0]))
			{
				EditorCommands.AddGlobalTransitionToSelectedState();
				SkillEditor.RepaintAll();
			}
			if (!SkillEditor.Builder.HasGlobalTransition(SkillEditor.SelectedState))
			{
				return;
			}
			ToolWindow.Divider(Strings.get_ToolWindow_Header_Global_Transitions());
			SkillState selectedState = SkillEditor.SelectedState;
			SkillTransition[] globalTransitions = SkillEditor.SelectedFsm.get_GlobalTransitions();
			for (int i = 0; i < globalTransitions.Length; i++)
			{
				SkillTransition fsmTransition = globalTransitions[i];
				if (!(fsmTransition.get_ToState() != selectedState.get_Name()))
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					if (GUILayout.Button(Labels.GetEventLabel(fsmTransition), EditorStyles.get_popup(), new GUILayoutOption[]
					{
						GUILayout.MinWidth(140f)
					}))
					{
						ToolWindow.editingTransition = fsmTransition;
						SkillEditorGUILayout.GenerateEventSelectionMenu(SkillEditor.SelectedFsm, fsmTransition.get_FsmEvent(), new GenericMenu.MenuFunction2(ToolWindow.SelectGlobalTransitionEvent), new GenericMenu.MenuFunction(SkillEditor.OpenEventManager)).ShowAsContext();
					}
					if (SkillEditorGUILayout.DeleteButton())
					{
						EditorCommands.DeleteGlobalTransition(fsmTransition);
						SkillEditor.RepaintAll();
					}
					GUILayout.EndHorizontal();
				}
			}
		}
		private static void TransitionTools()
		{
			ToolWindow.Header(Strings.get_ToolWindow_Header_Transition_Tools());
			SkillState selectedState = SkillEditor.SelectedState;
			SkillTransition selectedTransition = SkillEditor.SelectedTransition;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(Strings.get_Label_Event(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(40f)
			});
			if (GUILayout.Button(Labels.GetEventLabel(selectedTransition), EditorStyles.get_popup(), new GUILayoutOption[0]))
			{
				ToolWindow.editingTransition = selectedTransition;
				SkillEditorGUILayout.GenerateEventSelectionMenu(SkillEditor.SelectedFsm, selectedTransition.get_FsmEvent(), new GenericMenu.MenuFunction2(ToolWindow.SelectEvent), new GenericMenu.MenuFunction(SkillEditor.OpenEventManager)).ShowAsContext();
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(Strings.get_Label_State(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(40f)
			});
			if (GUILayout.Button(Labels.GetStateLabel(selectedTransition.get_ToState()), EditorStyles.get_popup(), new GUILayoutOption[0]))
			{
				ToolWindow.editingTransition = selectedTransition;
				SkillEditorGUILayout.GenerateStateSelectionMenu(SkillEditor.SelectedFsm, selectedTransition.get_ToState(), new GenericMenu.MenuFunction2(ToolWindow.SelectToState)).ShowAsContext();
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button(Strings.get_Command_Delete(), new GUILayoutOption[0]))
			{
				EditorCommands.DeleteTransition(selectedState, selectedTransition);
				SkillEditor.Selection.SelectTransition(null);
				SkillEditor.RepaintAll();
			}
		}
		private static void SelectGlobalTransitionEvent(object userdata)
		{
			SkillEvent fsmEvent = userdata as SkillEvent;
			EditorCommands.SetTransitionEvent(ToolWindow.editingTransition, fsmEvent);
			SkillEditor.RepaintAll();
		}
		private static void SelectEvent(object userdata)
		{
			SkillEvent fsmEvent = userdata as SkillEvent;
			EditorCommands.SetTransitionEvent(ToolWindow.editingTransition, fsmEvent);
			SkillEditor.RepaintAll();
		}
		private static void SelectToState(object userdata)
		{
			string toState = userdata as string;
			EditorCommands.SetTransitionTarget(ToolWindow.editingTransition, toState);
			SkillEditor.RepaintAll();
		}
		private static void Header(string label)
		{
			GUILayout.Label(label, new GUILayoutOption[0]);
		}
		private static void Divider(string label)
		{
			GUILayout.Label(label, new GUILayoutOption[0]);
		}
	}
}
