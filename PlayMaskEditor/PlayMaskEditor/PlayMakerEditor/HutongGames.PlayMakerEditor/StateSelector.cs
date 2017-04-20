using HutongGames.PlayMaker;
using System;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class StateSelector : BaseEditorWindow
	{
		private static Vector2 scrollPosition;
		private static float scrollViewHeight;
		private static Rect selectedRect;
		private static bool autoScroll;
		private SkillState lastSelectedState;
		public override void Initialize()
		{
			this.isToolWindow = true;
			this.InitWindowTitle();
			base.set_minSize(new Vector2(200f, 100f));
		}
		public void InitWindowTitle()
		{
			base.SetTitle(Strings.get_StateSelector_Title());
		}
		public override void DoGUI()
		{
			if (Event.get_current().get_type() == null && Event.get_current().get_button() == 0 && GUIUtility.get_hotControl() == 0)
			{
				GUIUtility.set_keyboardControl(0);
			}
			StateSelector.HandleKeyboardInput();
			StateSelector.DoToolbar();
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_State_Selector(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			StateSelector.scrollPosition = GUILayout.BeginScrollView(StateSelector.scrollPosition, new GUILayoutOption[0]);
			if (SkillEditor.SelectedFsm != null)
			{
				SkillState[] states = SkillEditor.SelectedFsm.get_States();
				for (int i = 0; i < states.Length; i++)
				{
					SkillState state = states[i];
					StateSelector.DoStateRow(state);
				}
			}
			GUILayout.EndScrollView();
			this.DoAutoScroll();
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
		private void Update()
		{
			if (FsmEditorSettings.DisableEditorWhenPlaying)
			{
				return;
			}
			if (this.lastSelectedState != SkillEditor.SelectedState)
			{
				this.lastSelectedState = SkillEditor.SelectedState;
				StateSelector.autoScroll = true;
				base.Repaint();
			}
		}
		private void DoAutoScroll()
		{
			if (SkillEditor.SelectedState == null)
			{
				return;
			}
			if (Event.get_current().get_type() == 7 && StateSelector.autoScroll)
			{
				StateSelector.scrollViewHeight = GUILayoutUtility.GetLastRect().get_height();
				if (StateSelector.selectedRect.get_y() < 0f)
				{
					StateSelector.scrollPosition.y = StateSelector.scrollPosition.y + StateSelector.selectedRect.get_y();
					base.Repaint();
				}
				else
				{
					if (StateSelector.selectedRect.get_y() + StateSelector.selectedRect.get_height() > StateSelector.scrollViewHeight)
					{
						StateSelector.scrollPosition.y = StateSelector.scrollPosition.y + (StateSelector.selectedRect.get_y() + StateSelector.selectedRect.get_height() - StateSelector.scrollViewHeight);
						base.Repaint();
					}
				}
				StateSelector.autoScroll = false;
			}
		}
		private static void DoStateRow(SkillState state)
		{
			GUIContent gUIContent = new GUIContent(state.get_Name(), state.get_Name());
			if (SkillEditorGUILayout.TableRow(new GUIContent[]
			{
				gUIContent
			}, new float[]
			{
				1f
			}, SkillEditor.SelectedState == state, FsmErrorChecker.StateHasErrors(state), new GUILayoutOption[0]) >= 0)
			{
				StateSelector.SelectState(state);
				GUIUtility.ExitGUI();
			}
			if (state == SkillEditor.SelectedState && Event.get_current().get_type() == 7)
			{
				StateSelector.selectedRect = GUILayoutUtility.GetLastRect();
				StateSelector.selectedRect.set_y(StateSelector.selectedRect.get_y() - StateSelector.scrollPosition.y);
			}
		}
		private static void DoToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[0]);
			string fullFsmLabel = Labels.GetFullFsmLabel(SkillEditor.SelectedFsm);
			if (GUILayout.Button(fullFsmLabel, EditorStyles.get_toolbarDropDown(), new GUILayoutOption[0]))
			{
				SkillEditorGUILayout.GenerateFsmSelectionMenu(true, false).ShowAsContext();
			}
			GUILayout.EndHorizontal();
		}
		private static void HandleKeyboardInput()
		{
			if (!Keyboard.IsGuiEventKeyboardShortcut())
			{
				return;
			}
			switch (Event.get_current().get_keyCode())
			{
			case 273:
				StateSelector.SelectPrevious();
				break;
			case 274:
				StateSelector.SelectNext();
				break;
			default:
				return;
			}
			Event.get_current().Use();
			GUIUtility.ExitGUI();
		}
		private static void SelectState(SkillState state)
		{
			SkillEditor.SelectState(state, true);
			SkillEditor.RepaintAll();
		}
		private static void SelectPrevious()
		{
			if (SkillEditor.SelectedState == null)
			{
				return;
			}
			int num = Array.IndexOf<SkillState>(SkillEditor.SelectedFsm.get_States(), SkillEditor.SelectedState);
			if (num > 0)
			{
				StateSelector.SelectState(SkillEditor.SelectedFsm.get_States()[num - 1]);
			}
			StateSelector.autoScroll = true;
		}
		private static void SelectNext()
		{
			if (SkillEditor.SelectedState == null)
			{
				return;
			}
			int num = Array.IndexOf<SkillState>(SkillEditor.SelectedFsm.get_States(), SkillEditor.SelectedState);
			if (num < SkillEditor.SelectedFsm.get_States().Length - 1)
			{
				StateSelector.SelectState(SkillEditor.SelectedFsm.get_States()[num + 1]);
			}
			StateSelector.autoScroll = true;
		}
	}
}
