using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class ActionReportWindow : BaseEditorWindow
	{
		private PlayMakerFSM currentFSM;
		private SkillState currentState;
		private SkillStateAction currentAction;
		private readonly string[] sortOptions = new string[]
		{
			Strings.get_ActionReportWindow_Sort_By_FSM(),
			Strings.get_ActionReportWindow_Sort_By_Action()
		};
		private Vector2 scrollPosition;
		public override void Initialize()
		{
			this.InitWindowTitle();
		}
		public void InitWindowTitle()
		{
			base.SetTitle(Strings.get_ActionReportWindow_Title());
		}
		public override void DoGUI()
		{
			if (SkillEditor.Instance == null)
			{
				base.Close();
				return;
			}
			if (EditorApplication.get_isCompiling())
			{
				GUI.set_enabled(false);
			}
			SkillEditorStyles.Init();
			this.DoToolbar();
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]);
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_ActionReportWindow_Description(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			switch (FsmEditorSettings.ConsoleActionReportSortOptionIndex)
			{
			case 0:
				this.DoSortedByFSM();
				break;
			case 1:
				this.DoSortedByAction();
				break;
			}
			if (ActionReport.ActionReportList.get_Count() == 0)
			{
				GUILayout.Label(Strings.get_ActionReportWindow_No_warnings_or_errors___(), new GUILayoutOption[0]);
			}
			GUILayout.EndScrollView();
		}
		private void DoToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[0]);
			FsmEditorSettings.ConsoleActionReportSortOptionIndex = EditorGUILayout.Popup(FsmEditorSettings.ConsoleActionReportSortOptionIndex, this.sortOptions, EditorStyles.get_toolbarPopup(), new GUILayoutOption[0]);
			if (GUI.get_changed())
			{
				FsmEditorSettings.SaveSettings();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		private void DoSortedByFSM()
		{
			this.currentFSM = null;
			this.currentState = null;
			this.currentAction = null;
			using (List<ActionReport>.Enumerator enumerator = ActionReport.ActionReportList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ActionReport current = enumerator.get_Current();
					if (!(current.fsm == null) && current.state != null && current.action != null)
					{
						if (current.fsm != this.currentFSM)
						{
							this.currentFSM = current.fsm;
							SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
							if (GUILayout.Button(Labels.GetFullFsmLabel(current.fsm.get_Fsm()), EditorStyles.get_label(), new GUILayoutOption[0]))
							{
								ActionReportWindow.SelectReport(current);
								break;
							}
							EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), 4);
						}
						if (current.state != this.currentState)
						{
							this.currentState = current.state;
							SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
							if (GUILayout.Button(Strings.get_Tab() + current.state.get_Name(), EditorStyles.get_label(), new GUILayoutOption[0]))
							{
								ActionReportWindow.SelectReport(current);
								break;
							}
							EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), 4);
						}
						if (current.action != this.currentAction)
						{
							SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
							this.currentAction = current.action;
							if (GUILayout.Button(Strings.get_Tab2() + Labels.GetActionLabel(current.action), EditorStyles.get_label(), new GUILayoutOption[0]))
							{
								ActionReportWindow.SelectReport(current);
								break;
							}
							EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), 4);
						}
						ActionReportWindow.DoReportLine(Strings.get_Tab3(), current);
					}
				}
			}
		}
		private void DoSortedByAction()
		{
			List<Type> list = new List<Type>();
			using (List<ActionReport>.Enumerator enumerator = ActionReport.ActionReportList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ActionReport current = enumerator.get_Current();
					Type type = current.action.GetType();
					if (!list.Contains(type))
					{
						list.Add(type);
					}
				}
			}
			this.currentAction = null;
			using (List<Type>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Type current2 = enumerator2.get_Current();
					SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
					GUILayout.Label(Labels.GetActionLabel(current2), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
					this.currentFSM = null;
					this.currentState = null;
					List<SkillState> list2 = new List<SkillState>();
					List<ActionReport> list3 = new List<ActionReport>();
					List<string> list4 = new List<string>();
					SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
					GUILayout.Label(Strings.get_ActionReportWindow_Action_Changes_Title(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
					using (List<ActionReport>.Enumerator enumerator3 = ActionReport.ActionReportList.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							ActionReport current3 = enumerator3.get_Current();
							Type type2 = current3.action.GetType();
							if (type2 == current2)
							{
								if (!list2.Contains(current3.state))
								{
									list3.Add(current3);
									list2.Add(current3.state);
								}
								if (!list4.Contains(current3.logText))
								{
									ActionReportWindow.DoReportLine(Strings.get_Tab(), current3);
									list4.Add(current3.logText);
								}
							}
						}
					}
					SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
					GUILayout.Label(Strings.get_ActionReportWindow_Effected_States_Title(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
					using (List<ActionReport>.Enumerator enumerator4 = list3.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							ActionReport current4 = enumerator4.get_Current();
							if (current4.state != null && !(current4.fsm == null))
							{
								if (GUILayout.Button(Strings.get_Tab() + Labels.GetFullStateLabel(current4.state), EditorStyles.get_label(), new GUILayoutOption[0]))
								{
									ActionReportWindow.SelectReport(current4);
									return;
								}
								EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), 4);
							}
						}
					}
				}
			}
		}
		private static void DoReportLine(string prefix, ActionReport report)
		{
			if (report.isError)
			{
				GUILayout.Box(prefix + report.logText, SkillEditorStyles.ErrorBox, new GUILayoutOption[0]);
				return;
			}
			GUILayout.Label(prefix + report.logText, new GUILayoutOption[0]);
		}
		private static void SelectReport(ActionReport report)
		{
			SkillEditor.SelectFsm(report.fsm.get_Fsm());
			SkillEditor.SelectState(report.state, true);
			SkillEditor.SelectAction(report.state, report.actionIndex, true);
		}
	}
}
