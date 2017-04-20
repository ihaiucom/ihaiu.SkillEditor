using HutongGames.Editor;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	internal class FsmVariablesEditor
	{
		public delegate void ContextClickVariable(SkillVariable variable);
		private const float NameColumnWidth = 155f;
		private static float tableHeight;
		private readonly EditorWindow window;
		private SkillVariables target;
		private Object owner;
		private bool editingGlobalVariables;
		private bool editingAsset;
		private List<SkillVariable> fsmVariables = new List<SkillVariable>();
		private bool listIsDirty;
		private bool restoreSelection;
		private Vector2 scrollPosition;
		private float scrollViewHeight;
		private Rect selectedRect;
		private bool autoScroll;
		private bool mouseOverTable;
		private SkillVariable selectedFsmVariable;
		private bool isVariableSelected;
		private int selectedIndex = -1;
		private TextField editNameTextField;
		private TextField addVariableTextField;
		private TextField categoryTextField;
		private readonly List<EditableLabel> categoryLabels = new List<EditableLabel>();
		private VariableType newVariableType;
		private bool sortByVariableType;
		private List<SkillVariable> filteredVariables = new List<SkillVariable>();
		private string searchString;
		private SearchBox searchBox;
		public FsmVariablesEditor.ContextClickVariable VariableContextClicked;
		public EditorApplication.CallbackFunction SettingsButtonClicked;
		private PlayMakerFSM fsmComponentOwner
		{
			get
			{
				return this.owner as PlayMakerFSM;
			}
		}
		private SkillTemplate fsmTemplateOwner
		{
			get
			{
				return this.owner as SkillTemplate;
			}
		}
		private PlayMakerGlobals globalsOwner
		{
			get
			{
				return this.owner as PlayMakerGlobals;
			}
		}
		private Skill fsmOwner
		{
			get
			{
				if (this.fsmComponentOwner != null)
				{
					return this.fsmComponentOwner.get_Fsm();
				}
				if (this.fsmTemplateOwner != null)
				{
					return this.fsmTemplateOwner.fsm;
				}
				return null;
			}
		}
		public FsmVariablesEditor(EditorWindow window, Object owner)
		{
			this.window = window;
			this.SetTarget(owner);
		}
		public void SetTarget(Object newOwner)
		{
			if (newOwner == this.owner)
			{
				return;
			}
			this.owner = newOwner;
			this.target = SkillVariable.GetVariables(this.owner);
			this.editingGlobalVariables = (newOwner is PlayMakerGlobals);
			this.editingAsset = (this.editingGlobalVariables || newOwner is SkillTemplate);
			this.Reset();
		}
		public void Reset()
		{
			SkillEditor.UpdateFsmInfo();
			this.BuildFsmVariableList(true);
			this.SanityCheckSelection();
			SkillEditor.Repaint(true);
			SkillEditor.RepaintAll();
			Keyboard.ResetFocus();
			this.restoreSelection = true;
		}
		private void InitControls()
		{
			if (this.searchBox == null)
			{
				this.searchBox = new SearchBox(this.window)
				{
					SearchModes = new string[]
					{
						"Name"
					},
					HasPopupSearchModes = true
				};
				SearchBox expr_41 = this.searchBox;
				expr_41.SearchChanged = (EditorApplication.CallbackFunction)Delegate.Combine(expr_41.SearchChanged, new EditorApplication.CallbackFunction(this.UpdateSearchResults));
				this.searchBox.Focus();
			}
			if (this.categoryTextField == null)
			{
				this.categoryTextField = new TextField(this.window, SkillEditorContent.EditCategoryLabel, "")
				{
					EditCommited = new TextField.EditCommitedCallback(this.SetVariableCategory),
					FocusLost = new TextField.LostFocusCallback(this.SetVariableCategory),
					AutoCompleteStrings = this.target.get_Categories()
				};
				this.editNameTextField = new TextField(this.window, SkillEditorContent.EditVariableLabel, "")
				{
					EditCommited = new TextField.EditCommitedCallback(this.SetVariableName),
					ValidateText = new TextField.ValidateCallback(this.ValidateVariableName),
					ControlName = "EditName"
				};
				this.addVariableTextField = new TextField(this.window, SkillEditorContent.NewVariableLabel, "")
				{
					EditCommited = new TextField.EditCommitedCallback(this.AddVariable),
					ValidateText = new TextField.ValidateCallback(this.ValidateVariableName),
					ControlName = "EditName"
				};
			}
		}
		public void UpdateView()
		{
			this.BuildFsmVariableList(true);
			SkillEditor.Repaint(false);
		}
		private void UpdateSearchResults()
		{
			this.searchString = this.searchBox.SearchString;
			this.BuildFilteredList();
		}
		private void BuildFilteredList()
		{
			if (string.IsNullOrEmpty(this.searchString))
			{
				this.filteredVariables = new List<SkillVariable>(this.fsmVariables);
				return;
			}
			this.filteredVariables.Clear();
			string text = this.searchString.ToUpper();
			using (List<SkillVariable>.Enumerator enumerator = this.fsmVariables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					if (current.Name.ToUpper().Contains(text))
					{
						this.filteredVariables.Add(current);
					}
					else
					{
						if (this.selectedFsmVariable == current)
						{
							this.Deselect();
							this.searchBox.Focus();
						}
					}
				}
			}
		}
		private void SanityCheckSelection()
		{
			if (this.selectedFsmVariable != null)
			{
				this.SelectVariable(this.selectedFsmVariable.Name);
			}
		}
		private void SelectVariable(int index)
		{
			if (index < 0 || index > this.fsmVariables.get_Count() - 1)
			{
				return;
			}
			this.SelectVariable(this.fsmVariables.get_Item(index));
		}
		private void SelectVariable(string name)
		{
			this.SelectVariable(this.GetVariable(name));
		}
		public void SelectVariable(SkillVariable variable)
		{
			if (variable == null)
			{
				this.Deselect();
				return;
			}
			this.isVariableSelected = true;
			this.selectedFsmVariable = variable;
			this.editNameTextField.Text = variable.Name;
			this.categoryTextField.Text = variable.GetCategory();
			Keyboard.ResetFocus();
			this.autoScroll = true;
			this.SaveSelection(variable.Name);
		}
		public void Deselect()
		{
			this.isVariableSelected = false;
			this.selectedFsmVariable = null;
			this.selectedIndex = -1;
			this.addVariableTextField.Text = "";
			SkillEditor.RepaintAll();
			Keyboard.ResetFocus();
			this.SaveSelection("");
		}
		[Localizable(false)]
		private void SaveSelection(string variableName)
		{
			if (this.window != null)
			{
				EditorPrefs.SetString("Playmaker." + this.window.get_titleContent().get_text() + ".SelectedVariable", variableName);
			}
		}
		[Localizable(false)]
		public void RestoreSelection()
		{
			string @string = EditorPrefs.GetString("Playmaker." + this.window.get_titleContent().get_text() + ".SelectedVariable", "");
			this.SelectVariable(@string);
		}
		public void OnGUI()
		{
			if (this.target == null)
			{
				GUILayout.FlexibleSpace();
				return;
			}
			if (this.listIsDirty)
			{
				this.BuildFsmVariableList(true);
				this.SanityCheckSelection();
			}
			this.InitControls();
			if (Event.get_current().get_type() == 8 && this.restoreSelection)
			{
				this.RestoreSelection();
				this.restoreSelection = false;
			}
			this.DoToolbar();
			this.DoTableHeaders();
			this.DoVariableTable();
			this.HandleDragAndDrop();
			if (Event.get_current().get_type() == 16 && this.SettingsButtonClicked != null)
			{
				this.SettingsButtonClicked.Invoke();
			}
			this.DoVariableEditor();
			this.HandleKeyboardInput();
			EditorGUILayout.Space();
			if (this.fsmComponentOwner != null && GUILayout.Button(SkillEditorContent.GlobalVariablesLabel, new GUILayoutOption[0]))
			{
				SkillEditor.OpenGlobalVariablesWindow();
				GUIUtility.ExitGUI();
			}
			EditorGUILayout.Space();
			if (Event.get_current().get_type() == null && Event.get_current().get_mousePosition().y < FsmVariablesEditor.tableHeight && !this.mouseOverTable)
			{
				this.Deselect();
			}
			Keyboard.Update();
		}
		private void DoAutoScroll()
		{
			if (this.selectedFsmVariable == null)
			{
				return;
			}
			if (Event.get_current().get_type() == 7 && this.autoScroll)
			{
				this.scrollViewHeight = GUILayoutUtility.GetLastRect().get_height();
				if (this.selectedRect.get_y() < 0f)
				{
					this.scrollPosition.y = this.scrollPosition.y + this.selectedRect.get_y();
					SkillEditor.Repaint(false);
				}
				else
				{
					if (this.selectedRect.get_y() + this.selectedRect.get_height() > this.scrollViewHeight)
					{
						this.scrollPosition.y = this.scrollPosition.y + (this.selectedRect.get_y() + this.selectedRect.get_height() - this.scrollViewHeight);
						SkillEditor.Repaint(false);
					}
				}
				this.autoScroll = false;
			}
		}
		private void DoToolbar()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[0]);
			this.searchBox.OnGUI();
			if (SkillEditorGUILayout.ToolbarSettingsButton() && this.SettingsButtonClicked != null)
			{
				this.SettingsButtonClicked.Invoke();
			}
			GUILayout.Space(-5f);
			EditorGUILayout.EndHorizontal();
		}
		private void DoTableHeaders()
		{
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(SkillEditorStyles.TableRowBox, new GUILayoutOption[0]);
			this.sortByVariableType = !GUILayout.Toggle(!this.sortByVariableType, SkillEditorContent.VariableNameLabel, SkillEditorStyles.TableRowHeader, new GUILayoutOption[]
			{
				GUILayout.MinWidth(155f)
			});
			GUILayout.FlexibleSpace();
			GUILayout.Label(SkillEditorContent.VariableUseCountLabel, new GUILayoutOption[0]);
			GUILayout.Space(10f);
			this.sortByVariableType = GUILayout.Toggle(this.sortByVariableType, SkillEditorContent.VariableTypeLabel, SkillEditorStyles.TableRowHeader, new GUILayoutOption[0]);
			GUILayout.Space(105f);
			GUILayout.EndHorizontal();
			EditorGUILayout.EndHorizontal();
			if (EditorGUI.EndChangeCheck())
			{
				this.SortVariableList();
			}
		}
		[Localizable(false)]
		private void DoVariableTable()
		{
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]);
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_Variable_Panel(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			if (this.fsmVariables.get_Count() == 0)
			{
				GUILayout.BeginHorizontal(SkillEditorStyles.TableRowBox, new GUILayoutOption[0]);
				GUILayout.Label(Strings.get_Label_None_In_Table(), new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
			}
			else
			{
				if (this.filteredVariables.get_Count() == 0)
				{
					GUILayout.Label("No search results for: " + this.searchString, new GUILayoutOption[0]);
				}
			}
			int num = 0;
			for (int i = 0; i < this.filteredVariables.get_Count(); i++)
			{
				SkillVariable fsmVariable = this.filteredVariables.get_Item(i);
				int categoryID = fsmVariable.CategoryID;
				if (categoryID > 0 && categoryID != num)
				{
					num = categoryID;
					this.categoryLabels.get_Item(num).OnGUI(new GUILayoutOption[0]);
					SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
				}
				bool flag = fsmVariable == this.selectedFsmVariable;
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.BeginHorizontal(flag ? SkillEditorStyles.SelectedEventBox : (FsmEditorSettings.DebugVariables ? SkillEditorStyles.TableRowBoxNoDivider : SkillEditorStyles.TableRowBox), new GUILayoutOption[0]);
				GUIStyle gUIStyle = flag ? SkillEditorStyles.TableRowTextSelected : SkillEditorStyles.TableRowText;
				if (GUILayout.Button(new GUIContent(fsmVariable.Name, fsmVariable.Tooltip), gUIStyle, new GUILayoutOption[]
				{
					GUILayout.MinWidth(155f)
				}))
				{
					this.SelectVariable(fsmVariable);
					if ((Event.get_current().get_button() == 1 || EditorGUI.get_actionKey()) && this.VariableContextClicked != null)
					{
						this.VariableContextClicked(fsmVariable);
					}
				}
				GUILayout.FlexibleSpace();
				int usedCount = this.GetUsedCount(fsmVariable.NamedVar);
				GUILayout.Label((usedCount >= 0) ? usedCount.ToString(CultureInfo.get_CurrentCulture()) : "-", gUIStyle, new GUILayoutOption[0]);
				GUILayout.Space(10f);
				bool changed = GUI.get_changed();
				GUI.set_changed(false);
				VariableType newType = EditorGUILayout.Popup(fsmVariable.Type, SkillVariable.VariableTypeNames, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(114f)
				});
				if (GUI.get_changed())
				{
					this.ChangeVariableType(fsmVariable, newType);
					GUIUtility.ExitGUI();
					return;
				}
				GUI.set_changed(changed);
				if (SkillEditorGUILayout.DeleteButton())
				{
					this.DeleteVariable(fsmVariable, true, true);
					GUIUtility.ExitGUI();
					return;
				}
				GUILayout.EndHorizontal();
				if (FsmEditorSettings.DebugVariables)
				{
					SkillEditorGUILayout.ReadonlyTextField(fsmVariable.NamedVar.ToString(), new GUILayoutOption[0]);
				}
				GUILayout.EndVertical();
				if (flag)
				{
					this.selectedIndex = i;
					if (Event.get_current().get_type() == 7)
					{
						this.selectedRect = GUILayoutUtility.GetLastRect();
						this.selectedRect.set_y(this.selectedRect.get_y() - this.scrollPosition.y);
					}
				}
			}
			Rect lastRect = GUILayoutUtility.GetLastRect();
			this.mouseOverTable = (Event.get_current().get_mousePosition().y < lastRect.get_y() + lastRect.get_height());
			if (this.fsmOwner != null)
			{
				this.DoGlobalVariablesTable();
			}
			GUILayout.Space(20f);
			GUILayout.FlexibleSpace();
			GUILayout.EndScrollView();
			this.DoAutoScroll();
			GUILayout.EndVertical();
			if (Event.get_current().get_type() == 7)
			{
				FsmVariablesEditor.tableHeight = GUILayoutUtility.GetLastRect().get_height() + GUI.get_skin().get_horizontalScrollbar().get_fixedHeight() + EditorStyles.get_toolbar().get_fixedHeight();
			}
		}
		private void DoGlobalVariablesTable()
		{
			if (SkillSearch.GetGlobalVariablesUsedCount(SkillEditor.SelectedFsm) == 0)
			{
				return;
			}
			GUILayout.Space(10f);
			SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(SkillEditorStyles.TableRowBox, new GUILayoutOption[0]);
			GUILayout.Label(SkillEditorContent.GlobalsLabel, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label(SkillEditorContent.VariableUseCountLabel, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			List<NamedVariable> globalVariablesUsed = SkillSearch.GetGlobalVariablesUsed(SkillEditor.SelectedFsm);
			using (List<NamedVariable>.Enumerator enumerator = globalVariablesUsed.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NamedVariable current = enumerator.get_Current();
					GUILayout.BeginHorizontal(SkillEditorStyles.TableRowBox, new GUILayoutOption[0]);
					GUIStyle tableRowText = SkillEditorStyles.TableRowText;
					if (GUILayout.Button(new GUIContent(current.get_Name(), current.get_Tooltip()), tableRowText, new GUILayoutOption[]
					{
						GUILayout.MinWidth(155f)
					}))
					{
						Keyboard.ResetFocus();
						this.Deselect();
						if (Event.get_current().get_button() == 1 || EditorGUI.get_actionKey())
						{
							FsmVariablesEditor.DoGlobalVariableContextMenu(current);
						}
					}
					int globalVariablesUsageCount = SkillSearch.GetGlobalVariablesUsageCount(current);
					GUILayout.FlexibleSpace();
					GUILayout.Label(globalVariablesUsageCount.ToString(CultureInfo.get_CurrentCulture()), tableRowText, new GUILayoutOption[0]);
					GUILayout.Space(10f);
					GUILayout.EndHorizontal();
					if (FsmEditorSettings.DebugVariables)
					{
						SkillEditorGUILayout.ReadonlyTextField(current.ToString(), new GUILayoutOption[0]);
					}
				}
			}
		}
		private static void DoGlobalVariableContextMenu(NamedVariable variable)
		{
			GenericMenu genericMenu = new GenericMenu();
			List<SkillInfo> globalVariablesUsageList = SkillSearch.GetGlobalVariablesUsageList(variable);
			if (globalVariablesUsageList.get_Count() == 0)
			{
				genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_No_FSMs_use_this_variable()));
			}
			else
			{
				using (List<SkillInfo>.Enumerator enumerator = globalVariablesUsageList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SkillInfo current = enumerator.get_Current();
						if (current.fsm == SkillEditor.SelectedFsm)
						{
							genericMenu.AddItem(new GUIContent(current.state.get_Name()), SkillEditor.SelectedState == current.state, new GenericMenu.MenuFunction2(FsmVariablesEditor.SelectState), current.state);
						}
					}
				}
				using (List<SkillInfo>.Enumerator enumerator2 = globalVariablesUsageList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SkillInfo current2 = enumerator2.get_Current();
						if (current2.fsm != SkillEditor.SelectedFsm)
						{
							genericMenu.AddItem(new GUIContent(Labels.GetFullFsmLabel(current2.fsm)), SkillEditor.SelectedFsm == current2.fsm, new GenericMenu.MenuFunction2(EditorCommands.SelectFsm), current2.fsm);
						}
					}
				}
			}
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Make_Local_Variable()), false, new GenericMenu.MenuFunction2(FsmVariablesEditor.MoveToLocal), variable);
			genericMenu.ShowAsContext();
		}
		private static void MoveToLocal(object userdata)
		{
			NamedVariable namedVariable = userdata as NamedVariable;
			if (namedVariable != null && SkillEditor.SelectedFsm != null)
			{
				SkillVariables variables = SkillEditor.SelectedFsm.get_Variables();
				if (variables.Contains(namedVariable.get_Name()))
				{
					Dialogs.OkDialog(Strings.get_Dialog_Make_Local_Variable(), Strings.get_Dialog_Error_Variable_with_same_name_already_exists());
					return;
				}
				SkillVariable.AddVariable(variables, namedVariable.Clone());
				SkillEditor.SelectedFsm.Reinitialize();
				SkillEditor.SetFsmDirty();
				SkillEditor.VariableManager.Reset();
				SkillEditor.RepaintAll();
			}
		}
		private static void SelectState(object userdata)
		{
			EditorCommands.SelectState(userdata);
			SkillEditor.Inspector.SetMode(InspectorMode.StateInspector);
		}
		[Localizable(false)]
		private int GetUsedCount(NamedVariable variable)
		{
			if (this.fsmOwner != null)
			{
				return SkillSearch.GetVariableUseCount(this.fsmOwner, variable);
			}
			if (!(this.globalsOwner != null))
			{
				return -1;
			}
			if (SkillSearch.GlobalVariablesUsageInitialized)
			{
				return SkillSearch.GetGlobalVariablesUsageCount(variable);
			}
			return -1;
		}
		private void HandleKeyboardInput()
		{
			if (!Keyboard.IsGuiEventKeyboardShortcut())
			{
				return;
			}
			KeyCode keyCode = Event.get_current().get_keyCode();
			if (keyCode != 27)
			{
				switch (keyCode)
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
			}
			else
			{
				this.Deselect();
			}
			Event.get_current().Use();
			GUIUtility.ExitGUI();
		}
		private void SelectPrevious()
		{
			if (this.selectedIndex == -1)
			{
				this.SelectFirst();
				return;
			}
			this.SelectVariable(this.selectedIndex - 1);
		}
		private void SelectNext()
		{
			if (this.selectedIndex == -1)
			{
				this.SelectLast();
				return;
			}
			this.SelectVariable(this.selectedIndex + 1);
		}
		private void SelectFirst()
		{
			this.SelectVariable(0);
		}
		private void SelectLast()
		{
			this.SelectVariable(this.fsmVariables.get_Count() - 1);
		}
		private void DoVariableEditor()
		{
			SkillEditorGUILayout.LabelWidth(100f);
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			if (this.isVariableSelected)
			{
				EditorGUI.BeginDisabledGroup(this.editingGlobalVariables);
				this.editNameTextField.OnGUI(new GUILayoutOption[0]);
				EditorGUI.EndDisabledGroup();
				EditorGUI.BeginChangeCheck();
				this.selectedFsmVariable.DoEditorGUI(null, this.editingAsset);
				this.selectedFsmVariable.Tooltip = EditorGUILayout.TextField(SkillEditorContent.TooltipLabel, this.selectedFsmVariable.Tooltip, new GUILayoutOption[0]);
				this.categoryTextField.OnGUI(new GUILayoutOption[0]);
				if (!this.editingGlobalVariables)
				{
					this.selectedFsmVariable.ShowInInspector = EditorGUILayout.Toggle(SkillEditorContent.InspectorLabel, this.selectedFsmVariable.ShowInInspector, new GUILayoutOption[0]);
				}
				if (SkillVariable.CanNetworkSync(this.selectedFsmVariable.NamedVar))
				{
					this.selectedFsmVariable.NetworkSync = EditorGUILayout.Toggle(SkillEditorContent.NetworkSyncLabel, this.selectedFsmVariable.NetworkSync, new GUILayoutOption[0]);
					if (FsmEditorSettings.CheckForNetworkSetupErrors && this.selectedFsmVariable.NetworkSync)
					{
						this.CheckForNetworkSyncErrors();
					}
				}
				else
				{
					bool enabled = GUI.get_enabled();
					GUI.set_enabled(false);
					EditorGUILayout.Toggle(SkillEditorContent.NetworkSyncNotSupportedLabel, false, new GUILayoutOption[0]);
					GUI.set_enabled(enabled);
				}
				if (EditorGUI.EndChangeCheck())
				{
					this.UpdateVariable(this.selectedFsmVariable, "");
				}
				if (!this.editNameTextField.IsValid)
				{
					GUILayout.Box(Strings.get_Error_Variable_Name_Taken(), SkillEditorStyles.ErrorBox, new GUILayoutOption[0]);
				}
			}
			else
			{
				this.addVariableTextField.OnGUI(new GUILayoutOption[0]);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.newVariableType = EditorGUILayout.Popup(SkillEditorContent.EditVariableTypeLabel, this.newVariableType, SkillVariable.VariableTypeNames, new GUILayoutOption[0]);
				EditorGUI.BeginDisabledGroup(!this.addVariableTextField.IsValid || this.addVariableTextField.Text.get_Length() == 0);
				if (GUILayout.Button(SkillEditorContent.AddLabel, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(35f),
					GUILayout.MaxHeight(15f)
				}))
				{
					this.AddVariable(this.addVariableTextField);
				}
				EditorGUI.EndDisabledGroup();
				GUILayout.EndHorizontal();
				if (!this.addVariableTextField.IsValid)
				{
					GUILayout.Box(Strings.get_Error_Variable_Name_Taken(), SkillEditorStyles.ErrorBox, new GUILayoutOption[0]);
				}
			}
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box((!string.IsNullOrEmpty(this.editNameTextField.Text)) ? Strings.get_Hint_Inspector_Usage() : Strings.get_Hint_Variable_Usage(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
		}
		private void CheckForNetworkSyncErrors()
		{
			if (SkillEditor.SelectedFsmGameObject == null)
			{
				return;
			}
			NetworkView networkView = SkillEditor.SelectedFsmGameObject.GetComponent<NetworkView>();
			if (networkView == null)
			{
				if (GUILayout.Button(Strings.get_Label_Fix_Missing_Network_Component(), SkillEditorStyles.ErrorBox, new GUILayoutOption[0]))
				{
					networkView = SkillEditor.SelectedFsmGameObject.AddComponent<NetworkView>();
					networkView.set_observed(SkillEditor.SelectedFsmComponent);
					return;
				}
			}
			else
			{
				if (networkView.get_observed() != SkillEditor.SelectedFsmComponent && GUILayout.Button(Strings.get_Label_Fix_Network_Observe_This_FSM(), SkillEditorStyles.ErrorBox, new GUILayoutOption[0]))
				{
					networkView.set_observed(SkillEditor.SelectedFsmComponent);
				}
			}
		}
		private void SetVariableName(TextField textField)
		{
			if (textField.IsValid)
			{
				this.RenameVariable(this.selectedFsmVariable, textField.Text);
				return;
			}
			textField.CancelTextEdit();
		}
		private void AddVariable(TextField textField)
		{
			SkillVariable variable = this.AddVariable(this.newVariableType, textField.Text, true);
			if (FsmEditorSettings.SelectNewVariables)
			{
				this.SelectVariable(variable);
			}
		}
		private void SetVariableCategory(TextField textField)
		{
			this.RegisterUndo(Strings.get_Label_Set_Variable_Category());
			this.selectedFsmVariable.SetCategory(textField.Text);
			this.RemoveUnusedCategories();
			this.BuildFsmVariableList(true);
		}
		private void CommitCategoryEdit(EditableLabel label)
		{
			label.StopEditing();
			this.RegisterUndo(Strings.get_Label_Rename_Variable_Category());
			this.target.get_Categories()[label.ID] = label.Text;
			this.SetDirty(false);
			this.Reset();
		}
		public void DoCategoryContextMenu(EditableLabel label)
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Rename_Category()), false, new GenericMenu.MenuFunction2(this.RenameCategory), label);
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Delete_Category()), false, new GenericMenu.MenuFunction2(this.DeleteCategory), label);
			genericMenu.ShowAsContext();
		}
		private void RenameCategory(object userdata)
		{
			EditableLabel editableLabel = (EditableLabel)userdata;
			editableLabel.StartEditing();
		}
		private void DeleteCategory(object userdata)
		{
			EditableLabel editableLabel = (EditableLabel)userdata;
			string text = editableLabel.Text;
			using (List<SkillVariable>.Enumerator enumerator = this.fsmVariables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					if (current.Category == text)
					{
						current.SetCategory("");
					}
				}
			}
			this.RemoveUnusedCategories();
			this.BuildFsmVariableList(true);
		}
		private bool ValidateVariableName(string newName)
		{
			if (this.isVariableSelected)
			{
				return this.IsValidVariableRename(this.selectedFsmVariable.Name, newName);
			}
			return !this.VariableNameExists(newName);
		}
		private bool IsValidVariableRename(string currentName, string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}
			using (List<SkillVariable>.Enumerator enumerator = this.fsmVariables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					if (currentName != current.Name && name == current.Name)
					{
						return false;
					}
				}
			}
			return true;
		}
		private bool VariableNameExists(string name)
		{
			using (List<SkillVariable>.Enumerator enumerator = this.fsmVariables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					if (name == current.Name)
					{
						return true;
					}
				}
			}
			return false;
		}
		[Localizable(false)]
		public void BuildFsmVariableList(bool keepSelection = true)
		{
			if (this.target == null)
			{
				return;
			}
			string name = "";
			if (this.selectedFsmVariable != null)
			{
				name = this.selectedFsmVariable.Name;
			}
			this.fsmVariables = SkillVariable.GetFsmVariableList(this.owner);
			this.SortVariableList();
			this.listIsDirty = false;
			this.categoryLabels.Clear();
			while (this.categoryLabels.get_Count() < this.target.get_Categories().Length)
			{
				string text = this.target.get_Categories()[this.categoryLabels.get_Count()];
				EditableLabel editableLabel = new EditableLabel(this.window, text)
				{
					ID = this.categoryLabels.get_Count(),
					EditCommited = new EditableLabel.EditCommitedCallback(this.CommitCategoryEdit),
					ContextClick = new EditableLabel.ContextClickCallback(this.DoCategoryContextMenu),
					Style = EditorStyles.get_boldLabel()
				};
				this.categoryLabels.Add(editableLabel);
			}
			if (keepSelection && this.categoryTextField != null)
			{
				this.SelectVariable(name);
			}
			this.BuildFilteredList();
		}
		[Localizable(false)]
		private void SortVariableList()
		{
			this.fsmVariables = (this.sortByVariableType ? SkillVariable.SortByType(this.target, this.fsmVariables) : SkillVariable.SortByName(this.target, this.fsmVariables));
		}
		private void ChangeVariableType(SkillVariable fsmVariable, VariableType newType)
		{
			if (!this.CheckDeleteVariable(Strings.get_Dialog_Edit_Variable_Type(), Strings.get_Dialog_Edit_Variable_Type_Are_you_sure(), fsmVariable))
			{
				return;
			}
			this.RegisterUndo(Strings.get_Label_Edit_Variable());
			string name = fsmVariable.Name;
			string tooltip = fsmVariable.Tooltip;
			string category = fsmVariable.Category;
			bool showInInspector = fsmVariable.ShowInInspector;
			if (this.fsmOwner != null)
			{
				SkillBuilder.RemoveVariableUsage(fsmVariable.NamedVar);
			}
			else
			{
				SkillSearch.UpdateAll();
				SkillBuilder.RemoveGlobalVariableUsage(fsmVariable.NamedVar);
			}
			SkillVariable.DeleteVariable(this.target, fsmVariable);
			SkillVariable fsmVariable2 = this.AddVariable(newType, name, false);
			fsmVariable2.Tooltip = tooltip;
			fsmVariable2.SetCategory(category);
			fsmVariable2.ShowInInspector = showInInspector;
			this.BuildFsmVariableList(true);
			this.SelectVariable(name);
		}
		private void UpdateVariable(SkillVariable fsmVariable, string newName = "")
		{
			this.RegisterUndo(Strings.get_Label_Edit_Variable());
			if (!string.IsNullOrEmpty(newName))
			{
				SkillBuilder.RenameVariable(fsmVariable.NamedVar, newName);
				SkillVariable.RenameVariable(this.target, fsmVariable, newName);
				this.listIsDirty = true;
			}
			this.SetDirty(true);
		}
		public SkillVariable AddVariable(VariableType type, string name, bool undo = true)
		{
			if (undo)
			{
				SkillEditor.RegisterUndo("Add Variable");
			}
			SkillVariable.AddVariable(this.target, type, name, null, 0);
			SkillVariable.RemapVariableCategories(this.target, this.fsmVariables);
			this.BuildFsmVariableList(false);
			this.SetDirty(true);
			return this.GetVariable(name);
		}
		private void RenameVariable(SkillVariable fsmVariable, string newName)
		{
			if (fsmVariable == null)
			{
				return;
			}
			this.RegisterUndo(Strings.get_Label_Edit_Variable());
			this.UpdateVariable(fsmVariable, newName);
			if (this.fsmOwner != null)
			{
				SkillEditor.SaveActions(this.fsmOwner);
				this.fsmOwner.InitData();
			}
			this.SaveSelection(fsmVariable.Name);
		}
		private bool CheckDeleteVariable(string title, string warning, SkillVariable fsmVariable)
		{
			if (this.globalsOwner != null)
			{
				return Dialogs.YesNoDialog(title, Strings.get_Label_Check_Edit_Global_Variable());
			}
			return this.fsmOwner == null || SkillSearch.GetUnusedVariables(this.fsmOwner).Contains(fsmVariable) || Dialogs.YesNoDialog(title, warning);
		}
		public void DeleteVariable(SkillVariable fsmVariable, bool undo = true, bool checkDialog = true)
		{
			if (checkDialog && !this.CheckDeleteVariable(Strings.get_Dialog_Delete_Variable(), Strings.get_Dialog_Delete_Variable_Are_you_sure(), fsmVariable))
			{
				return;
			}
			if (undo)
			{
				this.RegisterUndo(Strings.get_Dialog_Delete_Variable());
			}
			if (!this.editingGlobalVariables)
			{
				SkillBuilder.RemoveVariableUsage(fsmVariable.NamedVar);
				SkillVariable.DeleteVariable(this.target, fsmVariable);
			}
			else
			{
				SkillBuilder.RemoveGlobalVariableUsage(fsmVariable.NamedVar);
				SkillVariable.DeleteVariable(SkillVariables.get_GlobalVariables(), fsmVariable);
			}
			this.listIsDirty = true;
			this.SetDirty(true);
			this.Reset();
		}
		private SkillVariable GetVariable(string name)
		{
			using (List<SkillVariable>.Enumerator enumerator = this.fsmVariables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					if (current.Name == name)
					{
						return current;
					}
				}
			}
			return null;
		}
		private void HandleDragAndDrop()
		{
			if (!SkillEditor.MouseOverInspector || Event.get_current().get_mousePosition().y > FsmVariablesEditor.tableHeight)
			{
				return;
			}
			EventType type = Event.get_current().get_type();
			if (type == 9 || type == 10)
			{
				this.UpdateDragAndDrop();
			}
		}
		private void UpdateDragAndDrop()
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
			DragAndDrop.set_visualMode(1);
			if (Event.get_current().get_type() == 10)
			{
				this.PerformDrop(@object);
			}
		}
		private string ValidateDrop(Object obj)
		{
			if (this.editingAsset && !EditorUtility.IsPersistent(obj))
			{
				return Strings.get_Error_You_cannot_reference_Scene_Objects_in_Project_Assets();
			}
			return "";
		}
		private void PerformDrop(Object obj)
		{
			string text = this.ValidateDrop(obj);
			if (!string.IsNullOrEmpty(text))
			{
				Dialogs.OkDialog(text);
				return;
			}
			string name = this.MakeVariableNameUnique(obj.get_name());
			SkillVariable fsmVariable = this.AddVariable(12, name, true);
			fsmVariable.TypeName = obj.GetType().ToString();
			fsmVariable.ObjectType = obj.GetType();
			fsmVariable.ObjectValue = obj;
			fsmVariable.UpdateVariableValue();
			this.SetDirty(true);
			this.BuildFsmVariableList(false);
			this.SelectVariable(name);
			GUIUtility.ExitGUI();
		}
		[Localizable(false)]
		private string MakeVariableNameUnique(string name)
		{
			if (!this.VariableNameExists(name))
			{
				return name;
			}
			int num = 1;
			string text = string.Format("{0} {1}", name, num);
			while (this.VariableNameExists(text))
			{
				num++;
				text = string.Format("{0} {1}", name, num);
			}
			return text;
		}
		protected void SetDirty(bool checkAll = false)
		{
			Skill fsmOwner = this.fsmOwner;
			if (fsmOwner != null)
			{
				if (!EditorApplication.get_isPlayingOrWillChangePlaymode() && FsmErrorChecker.FsmHasErrors(fsmOwner))
				{
					ActionReport.Remove(fsmOwner.get_Owner() as PlayMakerFSM);
					fsmOwner.Reinitialize();
				}
				SkillEditor.SetFsmDirty(fsmOwner, checkAll, false, true);
			}
			if (this.globalsOwner != null)
			{
				EditorUtility.SetDirty(this.globalsOwner);
				if (!EditorApplication.get_isPlayingOrWillChangePlaymode() && SkillEditor.SelectedFsmComponent != null && FsmErrorChecker.FsmHasErrors(SkillEditor.SelectedFsm))
				{
					ActionReport.Remove(SkillEditor.SelectedFsmComponent);
					SkillEditor.SelectedFsm.Reinitialize();
				}
			}
		}
		protected void RegisterUndo(string action)
		{
			SkillEditor.RegisterUndo(this.owner, action);
		}
		[Localizable(false)]
		private void DebugFsmVariables(string prefix = "")
		{
			string text = prefix;
			using (List<SkillVariable>.Enumerator enumerator = this.fsmVariables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					string text2 = text;
					text = string.Concat(new string[]
					{
						text2,
						current.Name,
						"[",
						current.Category,
						"],"
					});
				}
			}
			Debug.Log(text);
		}
		[Localizable(false)]
		private void DebugCategories()
		{
			string text = "";
			List<SkillVariable> unsortedFsmVariableList = SkillVariable.GetUnsortedFsmVariableList(this.owner);
			using (List<SkillVariable>.Enumerator enumerator = unsortedFsmVariableList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					string text2 = text;
					text = string.Concat(new string[]
					{
						text2,
						current.Name,
						"[",
						current.Category,
						"],"
					});
				}
			}
			GUILayout.Label(text.Substring(0, text.get_Length() - 1), new GUILayoutOption[0]);
			text = "";
			int[] categoryIDs = this.target.get_CategoryIDs();
			for (int i = 0; i < categoryIDs.Length; i++)
			{
				int num = categoryIDs[i];
				text = text + num + ",";
			}
			GUILayout.Label(text.Substring(0, text.get_Length() - 1), new GUILayoutOption[0]);
			text = "";
			string[] categories = this.target.get_Categories();
			for (int j = 0; j < categories.Length; j++)
			{
				string text3 = categories[j];
				text = text + text3 + ",";
			}
			GUILayout.Label(text.Substring(0, text.get_Length() - 1), new GUILayoutOption[0]);
		}
		private void RemoveUnusedCategories()
		{
			this.RemoveUnusedCategories(this.target, this.fsmVariables);
			this.categoryTextField.AutoCompleteStrings = this.target.get_Categories();
		}
		private void RemoveUnusedCategories(SkillVariables variables, List<SkillVariable> fsmVariables)
		{
			List<string> usedVariableCategories = FsmVariablesEditor.GetUsedVariableCategories(fsmVariables);
			usedVariableCategories.Add("");
			List<string> list = new List<string>(variables.get_Categories());
			list.Sort();
			variables.set_Categories(Enumerable.ToArray<string>(Enumerable.Where<string>(list, new Func<string, bool>(usedVariableCategories.Contains))));
			SkillVariable.RemapVariableCategories(variables, fsmVariables);
		}
		private static List<string> GetUsedVariableCategories(IEnumerable<SkillVariable> fsmVariables)
		{
			List<string> list = new List<string>();
			using (IEnumerator<SkillVariable> enumerator = fsmVariables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					list.Add(current.Category);
				}
			}
			return list;
		}
	}
}
