using HutongGames.PlayMaker;
using System;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Serializable]
	internal class InspectorPanel
	{
		private const double tabSwitchDelay = 0.5;
		private InspectorMode inspectorMode;
		public static string[] InspectorModeLabels;
		private Rect errorBox = new Rect(90f, 0f, 20f, 20f);
		private int mouseOverTab = -1;
		private double mouseOverStartTime;
		private static bool editingDisabledForPrefabInstance;
		private static Skill editingPrefabInstance;
		public Rect View
		{
			get;
			private set;
		}
		public InspectorMode Mode
		{
			get
			{
				if (InspectorPanel.InspectorModeLabels == null)
				{
					InspectorPanel.Init();
				}
				return this.inspectorMode;
			}
		}
		public static void Init()
		{
			InspectorPanel.InspectorModeLabels = new string[]
			{
				Strings.get_FSM(),
				Strings.get_Label_State(),
				Strings.get_Label_Events(),
				Strings.get_Label_Variables()
			};
		}
		public void ResetView()
		{
			switch (this.inspectorMode)
			{
			case InspectorMode.FsmInspector:
				FsmInspector.Reset();
				return;
			case InspectorMode.StateInspector:
				SkillEditor.StateInspector.Reset();
				return;
			case InspectorMode.EventManager:
				SkillEditor.EventManager.Reset();
				return;
			case InspectorMode.VariableManager:
				SkillEditor.VariableManager.Reset();
				return;
			case InspectorMode.Preferences:
				break;
			case InspectorMode.Watermarks:
				WatermarkSelector.ResetSelection();
				break;
			default:
				return;
			}
		}
		public void Update()
		{
			if (EditorApplication.get_isPlaying() && FsmEditorSettings.DisableInspectorWhenPlaying)
			{
				return;
			}
			if (this.inspectorMode == InspectorMode.Watermarks && SkillEditor.SelectedFsm == null)
			{
				this.SetMode(InspectorMode.FsmInspector);
			}
			switch (this.inspectorMode)
			{
			case InspectorMode.FsmInspector:
				FsmInspector.Update();
				return;
			case InspectorMode.StateInspector:
				SkillEditor.StateInspector.Update();
				return;
			default:
				return;
			}
		}
		public void OnGUI(Rect area)
		{
			EditorGUI.set_indentLevel(0);
			GUILayout.BeginArea(area);
			if (EditorApplication.get_isPlaying() && FsmEditorSettings.DisableInspectorWhenPlaying)
			{
				GUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.Label(Strings.get_Hint_Inspector_disabled_when_playing(), new GUILayoutOption[0]);
				FsmEditorSettings.DisableInspectorWhenPlaying = !GUILayout.Toggle(!FsmEditorSettings.DisableInspectorWhenPlaying, Strings.get_Hint_Enable_inspector_when_playing(), new GUILayoutOption[0]);
				if (GUI.get_changed())
				{
					FsmEditorSettings.SaveSettings();
				}
				GUILayout.EndArea();
				return;
			}
			this.View = area;
			SkillEditorGUILayout.LabelWidth(150f);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			this.DoModeSelector();
			if (this.inspectorMode != InspectorMode.FsmInspector && this.inspectorMode != InspectorMode.Preferences && !Application.get_isPlaying() && SkillEditor.SelectedFsmUsesTemplate)
			{
				GUILayout.Label(Strings.get_Label_FSM_Uses_Template(), new GUILayoutOption[0]);
				if (FsmEditorSettings.ShowHints)
				{
					GUILayout.Box(Strings.get_InspectorPanel_FSM_Uses_Template(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
				}
				GUILayout.FlexibleSpace();
			}
			else
			{
				if (this.inspectorMode != InspectorMode.Preferences)
				{
					SkillEditorGUILayout.UnlockFsmGUI(SkillEditor.SelectedFsm);
					EditorGUI.BeginDisabledGroup(SkillEditor.SelectedFsm != null && SkillEditor.SelectedFsm.get_Locked());
				}
				switch (this.inspectorMode)
				{
				case InspectorMode.FsmInspector:
					FsmInspector.OnGUI();
					break;
				case InspectorMode.StateInspector:
					if (!SkillEditor.SelectedFsmIsLocked)
					{
						InspectorPanel.BeginPrefabInstanceCheck();
						SkillEditor.StateInspector.OnGUI();
						InspectorPanel.EndPrefabInstanceCheck();
					}
					else
					{
						GUILayout.FlexibleSpace();
					}
					break;
				case InspectorMode.EventManager:
					if (!SkillEditor.SelectedFsmIsLocked)
					{
						InspectorPanel.BeginPrefabInstanceCheck();
						SkillEditor.EventManager.OnGUI();
						InspectorPanel.EndPrefabInstanceCheck();
					}
					else
					{
						GUILayout.FlexibleSpace();
					}
					break;
				case InspectorMode.VariableManager:
					if (!SkillEditor.SelectedFsmIsLocked)
					{
						InspectorPanel.BeginPrefabInstanceCheck();
						SkillEditor.VariableManager.OnGUI();
						InspectorPanel.EndPrefabInstanceCheck();
					}
					else
					{
						GUILayout.FlexibleSpace();
					}
					break;
				case InspectorMode.Preferences:
					FsmEditorSettings.OnGUI();
					break;
				case InspectorMode.Watermarks:
					WatermarkSelector.OnGUI();
					break;
				}
				if (this.inspectorMode != InspectorMode.Preferences)
				{
					EditorGUI.EndDisabledGroup();
				}
			}
			this.DoBottomToolbar();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		private void DoModeSelector()
		{
			InspectorMode inspectorMode = (InspectorMode)GUILayout.Toolbar((int)this.inspectorMode, InspectorPanel.InspectorModeLabels, SkillEditorStyles.ToolbarTab, new GUILayoutOption[0]);
			if (inspectorMode != this.inspectorMode)
			{
				this.SetMode(inspectorMode);
			}
			if (FsmErrorChecker.StateHasActionErrors(SkillEditor.SelectedState))
			{
				GUI.Box(this.errorBox, SkillEditorStyles.StateErrorIcon, SkillEditorStyles.InlineErrorIcon);
			}
			else
			{
				GUI.Box(this.errorBox, GUIContent.none, GUIStyle.get_none());
			}
			if (DragAndDropManager.mode == DragAndDropManager.DragMode.None && Event.get_current().get_type() == 9)
			{
				Vector2 mousePosition = Event.get_current().get_mousePosition();
				if (mousePosition.x > 0f && mousePosition.y > 0f && mousePosition.y < EditorStyles.get_toolbar().get_fixedHeight())
				{
					int num = Mathf.FloorToInt(4f * mousePosition.x / 350f);
					if (this.mouseOverTab != num)
					{
						this.mouseOverStartTime = EditorApplication.get_timeSinceStartup();
						this.mouseOverTab = num;
					}
					if (EditorApplication.get_timeSinceStartup() - this.mouseOverStartTime > 0.5)
					{
						this.mouseOverStartTime = EditorApplication.get_timeSinceStartup();
						switch (num)
						{
						case 0:
							this.SetMode(InspectorMode.FsmInspector);
							GUIUtility.ExitGUI();
							return;
						case 1:
							this.SetMode(InspectorMode.StateInspector);
							GUIUtility.ExitGUI();
							return;
						case 2:
							this.SetMode(InspectorMode.EventManager);
							GUIUtility.ExitGUI();
							return;
						case 3:
							this.SetMode(InspectorMode.VariableManager);
							GUIUtility.ExitGUI();
							return;
						default:
							return;
						}
					}
				}
				else
				{
					this.mouseOverTab = -1;
				}
			}
		}
		public void SetMode(InspectorMode mode)
		{
			if (this.inspectorMode == mode)
			{
				return;
			}
			SkillEditor.DoDirtyFsmPrefab();
			EditorPrefs.SetInt(EditorPrefStrings.get_InspectorMode(), (int)mode);
			Keyboard.ResetFocus();
			this.inspectorMode = mode;
			this.ResetView();
			InspectorMode inspectorMode = this.inspectorMode;
			if (inspectorMode != InspectorMode.FsmInspector)
			{
				if (inspectorMode == InspectorMode.Watermarks)
				{
					WatermarkSelector.Init();
				}
			}
			else
			{
				FsmInspector.Init();
			}
			SkillEditor.Repaint(true);
		}
		private void DoBottomToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[0]);
			bool flag = GUILayout.Toggle(FsmEditorSettings.ShowHints, Strings.get_Command_Toggle_Hints(), EditorStyles.get_toolbarButton(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(100f)
			});
			if (flag != FsmEditorSettings.ShowHints)
			{
				FsmEditorSettings.ShowHints = flag;
				FsmEditorSettings.SaveSettings();
				SkillEditor.RepaintAll();
			}
			if (GUILayout.Button(Strings.get_Command_Preferences(), EditorStyles.get_toolbarButton(), new GUILayoutOption[]
			{
				GUILayout.MinWidth(150f)
			}))
			{
				this.inspectorMode = InspectorMode.Preferences;
			}
			GUILayout.EndHorizontal();
		}
		private static void BeginPrefabInstanceCheck()
		{
			if (FsmEditorSettings.ConfirmEditingPrefabInstances && InspectorPanel.editingPrefabInstance != null && InspectorPanel.editingPrefabInstance == SkillEditor.SelectedFsm)
			{
				EditorGUILayout.HelpBox(Strings.get_Label_Editing_Prefab_Instance(), 1);
				InspectorPanel.editingDisabledForPrefabInstance = false;
			}
			else
			{
				InspectorPanel.editingDisabledForPrefabInstance = (FsmEditorSettings.ConfirmEditingPrefabInstances && SkillPrefabs.IsPrefabInstance(SkillEditor.SelectedFsm));
				if (InspectorPanel.editingDisabledForPrefabInstance)
				{
					EditorGUILayout.HelpBox(Strings.get_Label_Confirm_Editing_of_Prefab_Instance(), 1);
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					if (GUILayout.Button(Strings.get_Label_Edit_Prefab(), new GUILayoutOption[0]))
					{
						SkillEditor.SelectPrefabParent();
					}
					if (GUILayout.Button(Strings.get_Label_Edit_Instance(), new GUILayoutOption[0]))
					{
						InspectorPanel.editingPrefabInstance = SkillEditor.SelectedFsm;
					}
					GUILayout.EndHorizontal();
					SkillEditor.GraphView.DisableEditing(Strings.get_Label_Editing_of_Prefab_Instance_is_disabled());
				}
			}
			EditorGUI.BeginDisabledGroup(InspectorPanel.editingDisabledForPrefabInstance);
		}
		private static void EndPrefabInstanceCheck()
		{
			EditorGUI.EndDisabledGroup();
		}
	}
}
