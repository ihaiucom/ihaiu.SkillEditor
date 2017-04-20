using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class SkillSelector : BaseEditorWindow
	{
		public enum FsmFilter
		{
			All,
			OnSelectedObject,
			RecentlySelected,
			WithErrors
		}
		private const float rowHeight = 15f;
		private const string tab = "     ";
		private const float CheckBoxWidth = 20f;
		private static SkillSelector instance;
		private readonly List<Skill> filteredList = new List<Skill>();
		private SkillSelector.FsmFilter fsmFilter;
		private float fsmColumnWidth;
		private bool showFullPath;
		private Skill lastSelectedFsm;
		private Vector2 scrollPosition;
		private float scrollViewHeight;
		private static Rect selectedRect;
		private static bool autoScroll;
		public static void RefreshView()
		{
			if (SkillSelector.instance != null)
			{
				SkillSelector.instance.BuildFilteredList();
			}
		}
		public void InitWindowTitle()
		{
			base.SetTitle(Strings.get_FsmSelector_Title());
		}
		public override void Initialize()
		{
			this.isToolWindow = true;
			SkillSelector.instance = this;
			this.InitWindowTitle();
			base.set_minSize(new Vector2(200f, 100f));
			this.BuildFilteredList();
		}
		private void OnFocus()
		{
			if (!this.Initialized)
			{
				return;
			}
			this.BuildFilteredList();
		}
		private void OnDisable()
		{
			if (SkillSelector.instance == this)
			{
				SkillSelector.instance = null;
			}
			this.filteredList.Clear();
		}
		public override void DoGUI()
		{
			if (Event.get_current().get_type() == null && Event.get_current().get_button() == 0 && GUIUtility.get_hotControl() == 0)
			{
				GUIUtility.set_keyboardControl(0);
			}
			this.fsmColumnWidth = (base.get_position().get_width() - 20f) * 0.6f;
			this.HandleKeyboardInput();
			this.DoToolbar();
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_FsmSelector(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			this.DoTableHeader();
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]);
			if (this.filteredList.get_Count() == 0)
			{
				GUILayout.BeginHorizontal(SkillEditorStyles.TableRowBox, new GUILayoutOption[0]);
				GUILayout.Label("     " + Strings.get_Label_None_In_Table(), new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
			}
			int num = (int)(base.get_position().get_height() / 15f);
			int num2 = (int)(this.scrollPosition.y / 15f);
			num2 = Mathf.Clamp(num2, 0, Mathf.Max(0, this.filteredList.get_Count() - num));
			GUILayout.Space((float)num2 * 15f);
			for (int i = num2; i < Mathf.Min(this.filteredList.get_Count(), num2 + num); i++)
			{
				this.DoTableRow(this.filteredList.get_Item(i));
			}
			GUILayout.Space(Mathf.Max(0f, (float)(this.filteredList.get_Count() - num2 - num) * 15f));
			GUILayout.EndScrollView();
			this.DoAutoScroll();
			this.DoBottomPanel();
		}
		private void DoBottomPanel()
		{
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			if (SkillEditor.SelectedFsm != null)
			{
				SkillEditor.SelectedFsm.set_Description(EditorGUILayout.TextArea(SkillEditor.SelectedFsm.get_Description(), SkillEditorStyles.TextArea, new GUILayoutOption[]
				{
					GUILayout.MinHeight(44f)
				}));
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				SkillEditor.SelectedFsm.set_DocUrl(EditorGUILayout.TextField(SkillEditor.SelectedFsm.get_DocUrl(), new GUILayoutOption[0]));
				if (string.IsNullOrEmpty(SkillEditor.SelectedFsm.get_DocUrl()))
				{
					GUI.set_enabled(false);
				}
				if (SkillEditorGUILayout.HelpButton("Online Help"))
				{
					Application.OpenURL(SkillEditor.SelectedFsm.get_DocUrl());
				}
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				GUI.set_enabled(false);
				GUILayout.TextArea(Strings.get_Label_Description___(), new GUILayoutOption[]
				{
					GUILayout.MinHeight(44f)
				});
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.TextField(Strings.get_FsmSelector_Label_Url_to_docs(), new GUILayoutOption[0]);
				SkillEditorGUILayout.HelpButton("Online Help");
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Space();
		}
		private void Update()
		{
			if (SkillEditor.Instance == null)
			{
				base.Close();
				return;
			}
			if (FsmEditorSettings.DisableEditorWhenPlaying)
			{
				return;
			}
			if (this.lastSelectedFsm != SkillEditor.SelectedFsm)
			{
				this.lastSelectedFsm = SkillEditor.SelectedFsm;
				SkillSelector.autoScroll = true;
				base.Repaint();
			}
		}
		private void DoAutoScroll()
		{
			if (SkillEditor.SelectedFsm == null)
			{
				return;
			}
			if (Event.get_current().get_type() == 7 && SkillSelector.autoScroll)
			{
				this.scrollViewHeight = GUILayoutUtility.GetLastRect().get_height();
				if (SkillSelector.selectedRect.get_y() < 0f)
				{
					this.scrollPosition.y = this.scrollPosition.y + SkillSelector.selectedRect.get_y();
					base.Repaint();
				}
				else
				{
					if (SkillSelector.selectedRect.get_y() + SkillSelector.selectedRect.get_height() > this.scrollViewHeight)
					{
						this.scrollPosition.y = this.scrollPosition.y + (SkillSelector.selectedRect.get_y() + SkillSelector.selectedRect.get_height() - this.scrollViewHeight);
						base.Repaint();
					}
				}
				SkillSelector.autoScroll = false;
			}
		}
		private void DoToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[0]);
			SkillSelector.FsmFilter fsmFilter = (SkillSelector.FsmFilter)EditorGUILayout.EnumPopup(this.fsmFilter, EditorStyles.get_toolbarPopup(), new GUILayoutOption[0]);
			if (this.fsmFilter != fsmFilter)
			{
				this.SetFilterMode(fsmFilter);
			}
			if (SkillEditorGUILayout.ToolbarSettingsButton())
			{
				this.GenerateSettingsMenu().ShowAsContext();
			}
			GUILayout.Space(-10f);
			GUILayout.EndHorizontal();
		}
		private void SetFilterMode(SkillSelector.FsmFilter mode)
		{
			this.fsmFilter = mode;
			this.BuildFilteredList();
		}
		private void DoTableHeader()
		{
			GUILayout.BeginHorizontal(SkillEditorStyles.TableRowBox, new GUILayoutOption[0]);
			GUILayout.Toggle(false, new GUIContent("", Strings.get_Label_Enabled()), SkillEditorStyles.TableRowHeader, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(16f)
			});
			GUILayout.Toggle(false, new GUIContent(Strings.get_FSM(), ""), SkillEditorStyles.TableRowHeader, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(this.fsmColumnWidth)
			});
			GUILayout.Toggle(false, new GUIContent(Strings.get_Label_State(), Strings.get_FsmSelector_Tootlip_State()), SkillEditorStyles.TableRowHeader, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
		}
		private void DoTableRow(Skill fsm)
		{
			if (fsm == null || fsm.get_Owner() == null)
			{
				return;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Height(15f)
			});
			fsm.get_Owner().set_enabled(GUILayout.Toggle(fsm.get_Owner().get_enabled(), new GUIContent("", Strings.get_Label_Enabled()), SkillEditorStyles.TableRowCheckBox, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(17f)
			}));
			string fullFsmLabel = Labels.GetFullFsmLabel(fsm);
			GUIContent gUIContent = new GUIContent("", fullFsmLabel);
			if (FsmEditorSettings.FsmBrowserShowFullPath && this.showFullPath)
			{
				gUIContent.set_text(fullFsmLabel);
			}
			else
			{
				gUIContent.set_text(Labels.GetFsmLabel(fsm));
			}
			GUIContent gUIContent2 = new GUIContent();
			if (GameStateTracker.CurrentState == GameState.Stopped)
			{
				gUIContent2.set_text((fsm.get_Template() != null) ? fsm.get_Template().fsm.get_StartState() : fsm.get_StartState());
			}
			else
			{
				gUIContent2.set_text((fsm.get_ActiveState() == null) ? Strings.get_FsmSelector_no_active_state() : fsm.get_ActiveState().get_Name());
			}
			gUIContent2.set_tooltip(gUIContent2.get_text());
			switch (SkillEditorGUILayout.TableRow(new GUIContent[]
			{
				gUIContent,
				gUIContent2
			}, new float[]
			{
				0.6f,
				0.4f
			}, SkillEditor.SelectedFsm == fsm, FsmErrorChecker.FsmHasErrors(fsm), new GUILayoutOption[0]))
			{
			case 0:
				SkillEditor.SelectFsm(fsm);
				GUIUtility.ExitGUI();
				break;
			case 1:
				SkillEditor.SelectFsm(fsm);
				SkillEditor.SelectStateByName(gUIContent2.get_text(), true);
				GUIUtility.ExitGUI();
				break;
			}
			GUILayout.EndHorizontal();
			if (fsm == SkillEditor.SelectedFsm && Event.get_current().get_type() == 7)
			{
				SkillSelector.selectedRect = GUILayoutUtility.GetLastRect();
				SkillSelector.selectedRect.set_y(SkillSelector.selectedRect.get_y() - this.scrollPosition.y);
			}
		}
		private void SelectPrevious()
		{
			if (SkillEditor.SelectedFsm == null)
			{
				return;
			}
			int num = this.filteredList.IndexOf(SkillEditor.SelectedFsm);
			if (num > 0)
			{
				SkillEditor.SelectFsm(this.filteredList.get_Item(num - 1));
			}
			SkillSelector.autoScroll = true;
		}
		private void SelectNext()
		{
			if (SkillEditor.SelectedFsm == null)
			{
				return;
			}
			int num = this.filteredList.IndexOf(SkillEditor.SelectedFsm);
			if (num < this.filteredList.get_Count() - 1)
			{
				SkillEditor.SelectFsm(this.filteredList.get_Item(num + 1));
			}
			SkillSelector.autoScroll = true;
		}
		private void HandleKeyboardInput()
		{
			if (!Keyboard.IsGuiEventKeyboardShortcut())
			{
				return;
			}
			switch (Event.get_current().get_keyCode())
			{
			case 273:
				this.SelectPrevious();
				break;
			case 274:
				this.SelectNext();
				break;
			default:
				return;
			}
			Event.get_current().Use();
			GUIUtility.ExitGUI();
		}
		private GenericMenu GenerateSettingsMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Show_Full_FSM_Path()), FsmEditorSettings.FsmBrowserShowFullPath, new GenericMenu.MenuFunction(this.ToggleShowFullPath));
			return genericMenu;
		}
		private void ToggleShowFullPath()
		{
			FsmEditorSettings.FsmBrowserShowFullPath = !FsmEditorSettings.FsmBrowserShowFullPath;
			FsmEditorSettings.SaveSettings();
			base.Repaint();
		}
		private void BuildFilteredList()
		{
			this.filteredList.Clear();
			this.showFullPath = true;
			if (this.fsmFilter == SkillSelector.FsmFilter.RecentlySelected)
			{
				this.filteredList.AddRange(SkillEditor.SelectionHistory.GetRecentlySelectedFSMs());
			}
			else
			{
				using (List<Skill>.Enumerator enumerator = SkillEditor.SortedFsmList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Skill current = enumerator.get_Current();
						if (current != null && !(current.get_UsedInTemplate() != null))
						{
							switch (this.fsmFilter)
							{
							case SkillSelector.FsmFilter.All:
								this.filteredList.Add(current);
								break;
							case SkillSelector.FsmFilter.OnSelectedObject:
								if (Selection.Contains(current.get_GameObject()))
								{
									this.filteredList.Add(current);
								}
								this.showFullPath = false;
								break;
							case SkillSelector.FsmFilter.WithErrors:
								if (FsmErrorChecker.FsmHasErrors(current))
								{
									this.filteredList.Add(current);
								}
								break;
							}
						}
					}
				}
			}
			base.Repaint();
		}
	}
}
