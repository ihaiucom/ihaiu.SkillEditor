using HutongGames.Editor;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	internal class FsmInspector
	{
		private static bool isInitialized;
		private static Vector2 scrollViewPosition;
		private static bool chooseWatermark;
		private static List<SkillVariable> fsmVariables = new List<SkillVariable>();
		private static SkillTemplate SelectedTemplate
		{
			get
			{
				if (!(SkillEditor.SelectedFsmComponent != null))
				{
					return null;
				}
				return SkillEditor.SelectedFsmComponent.get_FsmTemplate();
			}
		}
		public static void Init()
		{
			if (SkillEditor.SelectedFsm != null)
			{
				FsmInspector.BuildFsmVariableList(SkillEditor.SelectedFsm);
			}
		}
		public static void Reset()
		{
			FsmInspector.RefreshTemplate();
			FsmInspector.Init();
		}
		[Localizable(false)]
		public static void OnGUI()
		{
			EditorGUI.BeginChangeCheck();
			Skill fsm = SkillEditor.SelectedFsm;
			if (fsm == null)
			{
				GUILayout.FlexibleSpace();
				return;
			}
			if (!FsmInspector.isInitialized)
			{
				FsmInspector.isInitialized = true;
				FsmInspector.Init();
			}
			FsmInspector.scrollViewPosition = GUILayout.BeginScrollView(FsmInspector.scrollViewPosition, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			fsm.set_Name(EditorGUILayout.TextField(fsm.get_Name(), new GUILayoutOption[0]));
			if (EditorGUI.EndChangeCheck())
			{
				Labels.Update(fsm);
			}
			if (fsm.get_Owner() != null)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				SkillTemplate fsmTemplate = (SkillTemplate)EditorGUILayout.ObjectField(FsmInspector.SelectedTemplate, typeof(SkillTemplate), false, new GUILayoutOption[0]);
				if (fsmTemplate != FsmInspector.SelectedTemplate)
				{
					FsmInspector.SelectTemplate(fsmTemplate);
				}
				if (GUILayout.Button(SkillEditorContent.BrowseTemplateButton, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(30f),
					GUILayout.Height(16f)
				}))
				{
					Templates.DoSelectTemplateMenu(FsmInspector.SelectedTemplate, new GenericMenu.MenuFunction(FsmInspector.ClearTemplate), new GenericMenu.MenuFunction2(FsmInspector.SelectTemplate));
				}
				GUILayout.EndHorizontal();
			}
			EditorGUI.BeginDisabledGroup(!Application.get_isPlaying() && SkillEditor.SelectedFsmUsesTemplate);
			if (fsm.get_Template() != null)
			{
				fsm = fsm.get_Template().fsm;
			}
			fsm.set_Description(SkillEditorGUILayout.TextAreaWithHint(fsm.get_Description(), Strings.get_Label_Description___(), new GUILayoutOption[]
			{
				GUILayout.MinHeight(80f)
			}));
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsm.set_DocUrl(SkillEditorGUILayout.TextFieldWithHint(fsm.get_DocUrl(), Strings.get_Tooltip_Documentation_Url(), new GUILayoutOption[0]));
			EditorGUI.BeginDisabledGroup(!string.IsNullOrEmpty(fsm.get_DocUrl()));
			if (SkillEditorGUILayout.HelpButton("Online Help"))
			{
				Application.OpenURL(fsm.get_DocUrl());
			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
			EditorGUI.BeginDisabledGroup(!Application.get_isPlaying() && SkillEditor.SelectedFsmUsesTemplate);
			fsm.set_MaxLoopCountOverride(EditorGUILayout.IntField(SkillEditorContent.MaxLoopOverrideLabel, fsm.get_MaxLoopCountOverride(), new GUILayoutOption[0]));
			fsm.RestartOnEnable = GUILayout.Toggle(fsm.RestartOnEnable, SkillEditorContent.ResetOnDisableLabel, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();
			fsm = SkillEditor.SelectedFsm;
			SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
			GUILayout.Label(SkillEditorContent.FsmControlsLabel, EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			if (fsm.ExposedEvents.get_Count() + FsmInspector.fsmVariables.get_Count() == 0)
			{
				SkillEditorGUILayout.DisabledLabel(Strings.get_Label_None_In_Table());
			}
			else
			{
				SkillEditorGUILayout.LabelWidth(100f);
				int num = 0;
				for (int i = 0; i < FsmInspector.fsmVariables.get_Count(); i++)
				{
					SkillVariable fsmVariable = FsmInspector.fsmVariables.get_Item(i);
					if (fsmVariable.ShowInInspector)
					{
						int categoryID = fsmVariable.CategoryID;
						if (categoryID > 0 && categoryID != num)
						{
							num = categoryID;
							GUILayout.Label(SkillEditor.SelectedFsm.get_Variables().get_Categories()[categoryID], EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
							SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
						}
						fsmVariable.DoInspectorGUI(SkillEditorContent.TempContent(fsmVariable.Name, fsmVariable.Name + ((!string.IsNullOrEmpty(fsmVariable.Tooltip)) ? (":\n" + fsmVariable.Tooltip) : "")), false);
					}
				}
				using (List<SkillEvent>.Enumerator enumerator = fsm.ExposedEvents.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SkillEvent current = enumerator.get_Current();
						if (GUILayout.Button(current.get_Name(), new GUILayoutOption[0]))
						{
							fsm.Event(current);
						}
					}
				}
				if (GUI.get_changed())
				{
					SkillEditor.RepaintAll();
				}
			}
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_Expose_Events_and_Variables(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
			GUILayout.Label(SkillEditorContent.NetworkSyncLabel, EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			int num2 = 0;
			SkillBool[] boolVariables = fsm.get_Variables().get_BoolVariables();
			for (int j = 0; j < boolVariables.Length; j++)
			{
				SkillBool fsmBool = boolVariables[j];
				if (fsmBool.get_NetworkSync())
				{
					GUILayout.Label(fsmBool.get_Name(), new GUILayoutOption[0]);
					num2++;
				}
			}
			SkillFloat[] floatVariables = fsm.get_Variables().get_FloatVariables();
			for (int k = 0; k < floatVariables.Length; k++)
			{
				SkillFloat fsmFloat = floatVariables[k];
				if (fsmFloat.get_NetworkSync())
				{
					GUILayout.Label(fsmFloat.get_Name(), new GUILayoutOption[0]);
					num2++;
				}
			}
			SkillInt[] intVariables = fsm.get_Variables().get_IntVariables();
			for (int l = 0; l < intVariables.Length; l++)
			{
				SkillInt fsmInt = intVariables[l];
				if (fsmInt.get_NetworkSync())
				{
					GUILayout.Label(fsmInt.get_Name(), new GUILayoutOption[0]);
					num2++;
				}
			}
			SkillQuaternion[] quaternionVariables = fsm.get_Variables().get_QuaternionVariables();
			for (int m = 0; m < quaternionVariables.Length; m++)
			{
				SkillQuaternion fsmQuaternion = quaternionVariables[m];
				if (fsmQuaternion.get_NetworkSync())
				{
					GUILayout.Label(fsmQuaternion.get_Name(), new GUILayoutOption[0]);
					num2++;
				}
			}
			SkillVector3[] vector3Variables = fsm.get_Variables().get_Vector3Variables();
			for (int n = 0; n < vector3Variables.Length; n++)
			{
				SkillVector3 fsmVector = vector3Variables[n];
				if (fsmVector.get_NetworkSync())
				{
					GUILayout.Label(fsmVector.get_Name(), new GUILayoutOption[0]);
					num2++;
				}
			}
			if (num2 == 0)
			{
				SkillEditorGUILayout.DisabledLabel(Strings.get_Label_None_In_Table());
			}
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_Network_Sync_Variables(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
			GUILayout.Label("Debug", EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			fsm.set_ShowStateLabel(GUILayout.Toggle(fsm.get_ShowStateLabel(), SkillEditorContent.ShowStateLabelsLabel, new GUILayoutOption[0]));
			fsm.EnableBreakpoints = GUILayout.Toggle(fsm.EnableBreakpoints, SkillEditorContent.EnableBreakpointsLabel, new GUILayoutOption[0]);
			fsm.EnableDebugFlow = GUILayout.Toggle(fsm.EnableDebugFlow, SkillEditorContent.EnableDebugFlowLabel, new GUILayoutOption[0]);
			SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
			GUILayout.Label("Experimental", EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			EditorGUILayout.HelpBox(Strings.get_Help_Experimental_Warning(), 0);
			fsm.set_KeepDelayedEventsOnStateExit(GUILayout.Toggle(fsm.get_KeepDelayedEventsOnStateExit(), SkillEditorContent.KeepDelayedEvents, new GUILayoutOption[0]));
			fsm.set_ManualUpdate(GUILayout.Toggle(fsm.get_ManualUpdate(), SkillEditorContent.ManualUpdate, new GUILayoutOption[0]));
			GUILayout.EndScrollView();
			EventType arg_641_0 = Event.get_current().get_type();
			if (EditorGUI.EndChangeCheck())
			{
				SkillEditor.SetFsmDirty(false, false);
			}
			if (Event.get_current().get_type() == null)
			{
				GUIUtility.set_keyboardControl(0);
			}
			SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
			EditorGUI.BeginDisabledGroup(true);
			GUILayout.Label("Data Version: " + fsm.get_DataVersion(), EditorStyles.get_miniLabel(), new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			SkillEditorGUILayout.LockFsmGUI(fsm);
		}
		[Localizable(false)]
		private static void BuildFsmVariableList(Skill fsm)
		{
			FsmInspector.fsmVariables = SkillVariable.GetFsmVariableList(fsm.get_OwnerObject());
			FsmInspector.fsmVariables = Enumerable.ToList<SkillVariable>(Enumerable.Where<SkillVariable>(FsmInspector.fsmVariables, (SkillVariable x) => x.ShowInInspector));
		}
		public static void Update()
		{
			if (FsmInspector.chooseWatermark)
			{
				FsmInspector.chooseWatermark = false;
				EditorCommands.ChooseWatermark();
			}
		}
		private static void SelectTemplate(object userdata)
		{
			FsmInspector.SelectTemplate(userdata as SkillTemplate);
		}
		private static void SelectTemplate(SkillTemplate template)
		{
			if (template == FsmInspector.SelectedTemplate)
			{
				return;
			}
			if (template != null)
			{
				UndoUtility.RegisterUndo(SkillEditor.SelectedFsmComponent, FsmEditorSettings.ProductName + " : Set FSM Template");
				SkillEditor.SelectedFsmComponent.SetFsmTemplate(template);
				FsmInspector.BuildFsmVariableList(SkillEditor.SelectedFsmComponent.get_FsmTemplate().fsm);
				SkillEditor.SetFsmDirty(true, false);
				return;
			}
			FsmInspector.ClearTemplate();
		}
		private static void ClearTemplate()
		{
			SkillEditor.SelectedFsmComponent.Reset();
			SkillEditor.SelectedFsmComponent.SetFsmTemplate(null);
			FsmInspector.BuildFsmVariableList(SkillEditor.SelectedFsmComponent.get_Fsm());
		}
		private static void RefreshTemplate()
		{
			if (FsmInspector.SelectedTemplate == null || Application.get_isPlaying())
			{
				return;
			}
			SkillVariables fsmVariables = new SkillVariables(SkillEditor.SelectedFsm.get_Variables());
			SkillEditor.SelectedFsmComponent.SetFsmTemplate(FsmInspector.SelectedTemplate);
			SkillEditor.SelectedFsm.get_Variables().OverrideVariableValues(fsmVariables);
			FsmInspector.BuildFsmVariableList(SkillEditor.SelectedFsm);
		}
	}
}
