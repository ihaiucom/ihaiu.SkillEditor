using HutongGames.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class SkillTemplateSelector : BaseEditorWindow
	{
		private SearchBox searchBox;
		private string searchString = "";
		private Vector2 scrollPosition;
		private SkillTemplate selectedTemplate;
		private SkillTemplate prevTemplate;
		private SkillTemplate beforeSelected;
		private SkillTemplate afterSelected;
		private static float tableViewHeight;
		private float scrollViewHeight;
		private Rect selectedRect;
		private bool autoScroll;
		[SerializeField]
		private int selectedCategory;
		private int previousSelectedCateogry;
		private readonly List<SkillTemplate> filteredTemplates = new List<SkillTemplate>();
		private readonly List<string> filteredCategoryLookup = new List<string>();
		private readonly List<string> filteredCategories = new List<string>();
		public override void Initialize()
		{
			this.isToolWindow = true;
			this.InitWindowTitle();
			base.set_minSize(new Vector2(200f, 250f));
			this.InitSearchBox();
			this.SelectTemplate(null);
		}
		public void InitWindowTitle()
		{
			base.SetTitle(Strings.get_TemplateSelector_Title());
		}
		private void InitSearchBox()
		{
			if (this.searchBox == null)
			{
				this.searchBox = new SearchBox(this)
				{
					SearchModes = new string[]
					{
						"Name",
						"Description"
					},
					HasPopupSearchModes = true
				};
				SearchBox expr_41 = this.searchBox;
				expr_41.SearchChanged = (EditorApplication.CallbackFunction)Delegate.Combine(expr_41.SearchChanged, new EditorApplication.CallbackFunction(this.UpdateSearchResults));
				this.searchBox.Focus();
			}
		}
		public override void DoGUI()
		{
			this.HandleKeyboardInput();
			this.DoToolbar();
			this.DoHints();
			this.DoTemplateList();
			this.DoBottomPanel();
		}
		private void OnDisable()
		{
		}
		private void DoHints()
		{
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_Template_Selector(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
		}
		private void DoToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[0]);
			this.searchBox.OnGUI();
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
		}
		private void DoTemplateList()
		{
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, false, true, new GUILayoutOption[0]);
			if (string.IsNullOrEmpty(this.searchString))
			{
				this.DoFullTemplateList();
			}
			else
			{
				this.DoFilteredTemplateList();
			}
			GUILayout.EndScrollView();
			if (this.currentEvent.get_type() == 7)
			{
				SkillTemplateSelector.tableViewHeight = GUILayoutUtility.GetLastRect().get_height();
			}
			if (this.currentEvent.get_type() == null && this.currentEvent.get_mousePosition().y < SkillTemplateSelector.tableViewHeight)
			{
				this.Deselect();
			}
			this.DoAutoScroll();
		}
		private void DoFilteredTemplateList()
		{
			using (List<string>.Enumerator enumerator = this.filteredCategories.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.get_Current();
					GUILayout.Button(current, new GUILayoutOption[0]);
					for (int i = 0; i < this.filteredTemplates.get_Count(); i++)
					{
						if (this.filteredCategoryLookup.get_Item(i) == current)
						{
							this.DoTemplateGUI(this.filteredTemplates.get_Item(i));
						}
					}
				}
			}
		}
		private void DoFullTemplateList()
		{
			for (int i = 0; i < Templates.Categories.get_Count(); i++)
			{
				this.previousSelectedCateogry = this.selectedCategory;
				if (GUILayout.Toggle(this.selectedCategory == i, Templates.Categories.get_Item(i), "Button", new GUILayoutOption[0]))
				{
					this.selectedCategory = i;
					if (this.previousSelectedCateogry != this.selectedCategory)
					{
						this.selectedTemplate = null;
					}
				}
				if (this.selectedCategory == i)
				{
					this.DoTemplateCategory(Templates.Categories.get_Item(i));
				}
			}
		}
		private void DoTemplateCategory(string categoryName)
		{
			using (Dictionary<SkillTemplate, string>.Enumerator enumerator = Templates.CategoryLookup.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<SkillTemplate, string> current = enumerator.get_Current();
					if (current.get_Value() == categoryName)
					{
						SkillTemplate key = current.get_Key();
						this.DoTemplateGUI(key);
					}
				}
			}
		}
		private void DoTemplateGUI(SkillTemplate template)
		{
			if (template == null)
			{
				return;
			}
			GUIStyle gUIStyle = SkillEditorStyles.ActionItem;
			if (this.selectedTemplate == template)
			{
				gUIStyle = SkillEditorStyles.ActionItemSelected;
			}
			if (GUILayout.Button(template.get_name(), gUIStyle, new GUILayoutOption[0]))
			{
				if (this.selectedTemplate == template && (this.currentEvent.get_button() == 1 || EditorGUI.get_actionKey()))
				{
					this.TemplateContextMenu();
					this.currentEvent.Use();
				}
				this.SelectTemplate(template);
			}
			if (Event.get_current().get_type() == 7 && FsmErrorChecker.FsmHasErrors(template.fsm))
			{
				Rect rect = new Rect(GUILayoutUtility.GetLastRect());
				rect.set_width(14f);
				rect.set_height(14f);
				Rect rect2 = rect;
				GUI.DrawTexture(rect2, SkillEditorStyles.Errors);
			}
			if (template == this.selectedTemplate)
			{
				this.beforeSelected = this.prevTemplate;
				if (this.eventType == 7)
				{
					this.selectedRect = GUILayoutUtility.GetLastRect();
					this.selectedRect.set_y(this.selectedRect.get_y() - this.scrollPosition.y);
				}
			}
			if (this.prevTemplate == this.selectedTemplate)
			{
				this.afterSelected = template;
			}
			this.prevTemplate = template;
		}
		private void SelectTemplate(SkillTemplate template)
		{
			this.autoScroll = true;
			base.Repaint();
			this.selectedTemplate = template;
			if (this.selectedTemplate != null)
			{
				SkillEditor.SelectFsm(template.fsm);
			}
		}
		private void SelectPrevious()
		{
			if (this.selectedTemplate == null || this.beforeSelected == null)
			{
				return;
			}
			this.SelectTemplate(this.beforeSelected);
		}
		private void SelectNext()
		{
			if (this.selectedTemplate == null || this.afterSelected == null)
			{
				return;
			}
			this.SelectTemplate(this.afterSelected);
		}
		private void DoAutoScroll()
		{
			if (this.eventType == 7 && this.autoScroll)
			{
				this.scrollViewHeight = GUILayoutUtility.GetLastRect().get_height();
				if (this.selectedRect.get_y() < 0f)
				{
					this.scrollPosition.y = this.scrollPosition.y + this.selectedRect.get_y();
					base.Repaint();
				}
				else
				{
					if (this.selectedRect.get_y() + this.selectedRect.get_height() > this.scrollViewHeight)
					{
						this.scrollPosition.y = this.scrollPosition.y + (this.selectedRect.get_y() + this.selectedRect.get_height() - this.scrollViewHeight);
						base.Repaint();
					}
				}
				this.autoScroll = false;
			}
		}
		private void Deselect()
		{
			this.selectedTemplate = null;
			SkillEditor.Instance.OnSelectionChange();
			Keyboard.ResetFocus();
			base.Repaint();
		}
		private void DoBottomPanel()
		{
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			if (this.selectedTemplate == null)
			{
				EditorGUI.BeginDisabledGroup(true);
				GUILayout.TextField(Strings.get_Label_Category___(), new GUILayoutOption[0]);
				EditorGUILayout.TextArea(Strings.get_Label_Description___(), new GUILayoutOption[]
				{
					GUILayout.MinHeight(44f)
				});
				EditorGUI.EndDisabledGroup();
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				this.selectedTemplate.set_Category(EditorGUILayout.TextField(this.selectedTemplate.get_Category(), new GUILayoutOption[0]));
				this.selectedTemplate.fsm.set_Description(EditorGUILayout.TextArea(this.selectedTemplate.fsm.get_Description(), SkillEditorStyles.TextArea, new GUILayoutOption[]
				{
					GUILayout.MinHeight(44f)
				}));
				if (EditorGUI.EndChangeCheck())
				{
					EditorUtility.SetDirty(this.selectedTemplate);
					Templates.InitList();
				}
				if (this.eventType != 12 && this.currentEvent.get_clickCount() == 2 && SkillEditor.SelectedTemplate != this.selectedTemplate)
				{
					SkillTemplateSelector.AddTemplateToFsm();
					this.currentEvent.Use();
				}
			}
			if (GUILayout.Button(Strings.get_Label_New_Template(), new GUILayoutOption[0]))
			{
				SkillTemplate template = SkillBuilder.CreateTemplate();
				this.SelectTemplate(template);
				base.Repaint();
			}
			if (GUILayout.Button(Strings.get_Label_Load_All_Templates(), new GUILayoutOption[0]))
			{
				Templates.LoadAll();
				base.Repaint();
			}
		}
		private static void AddTemplateToFsm()
		{
			if (SkillEditor.SelectedFsm == null)
			{
				return;
			}
			Vector2 viewCenter = FsmGraphView.GetViewCenter();
			EditorCommands.PasteTemplate(viewCenter);
		}
		private void HandleKeyboardInput()
		{
			int controlID = GUIUtility.GetControlID(1);
			if (this.currentEvent.GetTypeForControl(controlID) == 4)
			{
				KeyCode keyCode = this.currentEvent.get_keyCode();
				if (keyCode != 27)
				{
					switch (keyCode)
					{
					case 273:
						this.currentEvent.Use();
						this.SelectPrevious();
						GUIUtility.ExitGUI();
						return;
					case 274:
						this.currentEvent.Use();
						this.SelectNext();
						GUIUtility.ExitGUI();
						return;
					default:
						return;
					}
				}
				else
				{
					this.currentEvent.Use();
					this.Deselect();
					GUIUtility.ExitGUI();
				}
			}
		}
		private void UpdateSearchResults()
		{
			this.searchString = this.searchBox.SearchString;
			if (!string.IsNullOrEmpty(this.searchString))
			{
				this.BuildFilteredList();
				this.SelectFirstMatchingTemplate();
			}
		}
		[Localizable(false)]
		private void BuildFilteredList()
		{
			this.filteredTemplates.Clear();
			this.filteredCategories.Clear();
			this.filteredCategoryLookup.Clear();
			string[] filter = this.searchString.ToUpper().Split(new char[]
			{
				' '
			});
			using (List<SkillTemplate>.Enumerator enumerator = Templates.List.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillTemplate current = enumerator.get_Current();
					if (this.TemplateMatchesFilter(current, filter))
					{
						this.filteredTemplates.Add(current);
						this.filteredCategoryLookup.Add(current.get_Category());
						if (!this.filteredCategories.Contains(current.get_Category()))
						{
							this.filteredCategories.Add(current.get_Category());
						}
						this.filteredCategories.Sort();
					}
				}
			}
		}
		private bool TemplateMatchesFilter(SkillTemplate template, IEnumerable<string> filter)
		{
			bool flag = SkillTemplateSelector.TemplateNameMatchesFilter(template, filter);
			if (flag || this.searchBox.SearchMode == 0)
			{
				return flag;
			}
			return SkillTemplateSelector.TemplateDescriptionMatchesFilter(template, filter);
		}
		[Localizable(false)]
		private static bool TemplateNameMatchesFilter(SkillTemplate template, IEnumerable<string> filter)
		{
			if (template == null)
			{
				return false;
			}
			string text = template.get_name().ToUpper().Replace(" ", "");
			using (IEnumerator<string> enumerator = filter.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.get_Current();
					if (!text.Contains(current))
					{
						return false;
					}
				}
			}
			return true;
		}
		private static bool TemplateDescriptionMatchesFilter(SkillTemplate template, IEnumerable<string> filter)
		{
			string text = template.fsm.get_Description().ToUpper();
			using (IEnumerator<string> enumerator = filter.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.get_Current();
					if (!text.Contains(current))
					{
						return false;
					}
				}
			}
			return true;
		}
		private void SelectFirstMatchingTemplate()
		{
			if (this.filteredCategories.get_Count() == 0)
			{
				return;
			}
			string text = this.filteredCategories.get_Item(0);
			for (int i = 0; i < this.filteredTemplates.get_Count(); i++)
			{
				if (this.filteredCategoryLookup.get_Item(i) == text)
				{
					this.SelectTemplate(this.filteredTemplates.get_Item(i));
					return;
				}
			}
		}
		private void TemplateContextMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Command_Ping_Template_Asset()), false, new GenericMenu.MenuFunction2(this.PingTemplate), this.selectedTemplate);
			genericMenu.AddItem(new GUIContent(Strings.get_Command_Delete()), false, new GenericMenu.MenuFunction2(this.DeleteTemplate), this.selectedTemplate);
			genericMenu.ShowAsContext();
		}
		private void PingTemplate(object userdata)
		{
			SkillTemplate fsmTemplate = userdata as SkillTemplate;
			if (fsmTemplate != null)
			{
				EditorGUIUtility.PingObject(fsmTemplate);
			}
		}
		private void DeleteTemplate(object userdata)
		{
			if (!Dialogs.AreYouSure(Strings.get_Dialog_Delete_Template(), Strings.get_Label_You_cannot_undo_this_action()))
			{
				return;
			}
			SkillTemplate fsmTemplate = userdata as SkillTemplate;
			if (fsmTemplate != null)
			{
				if (SkillEditor.SelectedTemplate == fsmTemplate)
				{
					SkillEditor.Instance.OnSelectionChange();
				}
				string assetPath = AssetDatabase.GetAssetPath(fsmTemplate);
				AssetDatabase.DeleteAsset(assetPath);
			}
		}
	}
}
