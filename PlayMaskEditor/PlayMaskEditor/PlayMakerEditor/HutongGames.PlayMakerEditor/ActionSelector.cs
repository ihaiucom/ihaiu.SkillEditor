using HutongGames.Editor;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class ActionSelector : BaseEditorWindow
	{
		public const string FavoritesCategory = "Favorites";
		public const string RecentCategory = "Recent";
		private const string selectedActionKey = "PlayMaker.ActionSelector.SelectedAction";
		private const string selectedActionCategoryKey = "PlayMaker.ActionSelector.SelectedActionCategory";
		private static ActionSelector instance;
		private Vector2 mousePos;
		private Type mouseOverAction;
		private bool addingAction;
		private Vector2 scrollPosition;
		private float scrollViewHeight;
		private float descriptionHeight;
		private Rect selectedRect;
		private bool autoScroll;
		private float previewScrollViewHeight;
		private Vector2 previewScrollPosition;
		private float previewActionGUIHeight;
		private Type selectedAction;
		private Type prevAction;
		private Type beforeSelected;
		private Type afterSelected;
		private string selectedActionCategory;
		private string prevActionCategory;
		private string beforeSelectedCategory;
		private string afterSelectedCategory;
		private bool prevActionWasSelected;
		private SkillStateAction previewAction;
		private static Type findActionType;
		private SearchBox searchBox;
		private string searchString = "";
		public override void Initialize()
		{
			ActionSelector.instance = this;
			base.set_minSize(new Vector2(200f, 200f));
			base.set_wantsMouseMove(true);
			this.isToolWindow = true;
			this.InitWindowTitle();
			this.InitSearchBox();
			Actions.BuildList();
			this.FilterActionList();
			this.NeedToFindAction();
			this.RestoreSelectedAction();
			base.Repaint();
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
		public void InitWindowTitle()
		{
			base.SetTitle(Strings.get_ActionSelector_Title());
		}
		public static void FindAction(Type actionType)
		{
			ActionSelector.findActionType = actionType;
		}
		private void NeedToFindAction()
		{
			if (ActionSelector.findActionType != null)
			{
				this.FindAction();
				ActionSelector.findActionType = null;
			}
		}
		private void OnFocus()
		{
			if (!this.Initialized || (EditorApplication.get_isPlaying() && FsmEditorSettings.DisableActionBrowerWhenPlaying))
			{
				return;
			}
			this.InitSearchBox();
			this.searchBox.Focus();
			this.FilterActionList();
			this.NeedToFindAction();
			base.Repaint();
		}
		public override void DoGUI()
		{
			if (EditorApplication.get_isPlaying() && FsmEditorSettings.DisableActionBrowerWhenPlaying)
			{
				GUILayout.Label(Strings.get_ActionSelector_Disabled_when_playing(), new GUILayoutOption[0]);
				FsmEditorSettings.DisableActionBrowerWhenPlaying = !GUILayout.Toggle(!FsmEditorSettings.DisableActionBrowerWhenPlaying, Strings.get_ActionSelector_Enable_Action_Browser_When_Playing(), new GUILayoutOption[0]);
				if (GUI.get_changed())
				{
					FsmEditorSettings.SaveSettings();
				}
				return;
			}
			this.HandleKeyboardInput();
			this.DoMainToolbar();
			this.DoActionList();
			this.HandleDragAndDrop();
			this.DoBottomPanel();
		}
		private void OnDisable()
		{
			string text = (this.selectedAction != null) ? this.selectedAction.get_FullName() : "";
			EditorPrefs.SetString("PlayMaker.ActionSelector.SelectedAction", text);
			EditorPrefs.SetString("PlayMaker.ActionSelector.SelectedActionCategory", this.selectedActionCategory);
			ActionSelector.instance = null;
		}
		private void RestoreSelectedAction()
		{
			string @string = EditorPrefs.GetString("PlayMaker.ActionSelector.SelectedAction", "");
			string string2 = EditorPrefs.GetString("PlayMaker.ActionSelector.SelectedActionCategory", "");
			if (!string.IsNullOrEmpty(@string))
			{
				Type globalType = ReflectionUtils.GetGlobalType(@string);
				if (globalType != null)
				{
					this.SelectAction(globalType, string2);
				}
			}
		}
		private void HandleDragAndDrop()
		{
			if (this.eventType == 9 || this.eventType == 10)
			{
				if (DragAndDropManager.mode == DragAndDropManager.DragMode.MoveActions)
				{
					DragAndDrop.set_visualMode(16);
					if (this.eventType == 10)
					{
						DragAndDrop.AcceptDrag();
						SkillEditor.StateInspector.DeleteSelectedActions(true);
					}
				}
				Event.get_current().Use();
			}
		}
		private void DoMainToolbar()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[0]);
			this.searchBox.OnGUI();
			if (SkillEditorGUILayout.ToolbarSettingsButton())
			{
				this.GenerateSettingsMenu().ShowAsContext();
			}
			GUILayout.Space(-5f);
			EditorGUILayout.EndHorizontal();
		}
		private void UpdateSearchResults()
		{
			this.searchString = this.searchBox.SearchString;
			if (!string.IsNullOrEmpty(this.searchString))
			{
				this.FilterActionList();
				this.SelectFirstMatchingAction();
				return;
			}
			FsmEditorSettings.SelectedActionCategory = Actions.GetCategoryIndex(this.selectedActionCategory);
			FsmEditorSettings.SaveSettings();
			this.autoScroll = true;
		}
		private void DoActionList()
		{
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, false, true, new GUILayoutOption[0]);
			this.mouseOverAction = null;
			if (Event.get_current().get_isMouse())
			{
				this.mousePos = Event.get_current().get_mousePosition();
			}
			if (string.IsNullOrEmpty(this.searchString))
			{
				this.DoFullActionList();
			}
			else
			{
				this.DoFilteredActionList();
			}
			GUILayout.EndScrollView();
			this.DoAutoScroll();
		}
		public static void AddToFavorites(object actionType)
		{
			Actions.AddActionToCategory("Favorites", actionType as Type, -1);
			ActionSelector.RefreshFilteredList();
		}
		public static void RemoveFromFavorites(object actionType)
		{
			Actions.RemoveActionFromCategory("Favorites", actionType as Type);
			ActionSelector.RefreshFilteredList();
		}
		private void DoFilteredActionList()
		{
			if (Actions.FilteredCategories.get_Count() == 0)
			{
				GUILayout.Label(Strings.get_Label_No_search_results_for__() + this.searchString, new GUILayoutOption[0]);
				return;
			}
			using (List<string>.Enumerator enumerator = Actions.FilteredCategories.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.get_Current();
					GUILayout.Button(current, new GUILayoutOption[0]);
					List<Type> actionsInCategoryFiltered = Actions.GetActionsInCategoryFiltered(current);
					for (int i = 0; i < actionsInCategoryFiltered.get_Count(); i++)
					{
						this.DoActionButton(current, actionsInCategoryFiltered.get_Item(i));
					}
				}
			}
		}
		private void DoFullActionList()
		{
			for (int i = 0; i < Actions.Categories.get_Count(); i++)
			{
				string text = Actions.Categories.get_Item(i);
				if (GUILayout.Toggle(FsmEditorSettings.SelectedActionCategory == i, text, "Button", new GUILayoutOption[0]))
				{
					this.OpenCategory(i);
				}
				else
				{
					this.CloseCategory(i);
				}
				if (FsmEditorSettings.SelectedActionCategory == i)
				{
					List<Type> actionsInCategory = Actions.GetActionsInCategory(text);
					this.DoActionList(text, actionsInCategory);
				}
			}
		}
		private void OpenCategory(int i)
		{
			if (FsmEditorSettings.SelectedActionCategory == i)
			{
				return;
			}
			FsmEditorSettings.SelectedActionCategory = i;
			string text = Actions.Categories.get_Item(i);
			List<Type> actionsInCategory = Actions.GetActionsInCategory(text);
			if (actionsInCategory.get_Count() > 0)
			{
				this.SelectAction(actionsInCategory.get_Item(0), text);
			}
			FsmEditorSettings.SaveSettings();
			base.Repaint();
		}
		private void CloseCategory(int i)
		{
			if (FsmEditorSettings.SelectedActionCategory != i)
			{
				return;
			}
			FsmEditorSettings.SelectedActionCategory = -1;
			FsmEditorSettings.SaveSettings();
			this.selectedAction = null;
			base.Repaint();
		}
		private void DoAutoScroll()
		{
			if (Event.get_current().get_type() == 7)
			{
				this.scrollViewHeight = GUILayoutUtility.GetLastRect().get_height();
				if (this.autoScroll)
				{
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
		}
		private void DoActionList(string category, List<Type> actions)
		{
			if (actions == null)
			{
				return;
			}
			for (int i = 0; i < actions.get_Count(); i++)
			{
				this.DoActionButton(category, actions.get_Item(i));
			}
		}
		private void DoActionButton(string category, Type actionType)
		{
			string actionLabel = Labels.GetActionLabel(actionType);
			bool flag = actionType == this.selectedAction && category == this.selectedActionCategory;
			GUIStyle gUIStyle = flag ? SkillEditorStyles.ActionItemSelected : SkillEditorStyles.ActionItem;
			GUIStyle gUIStyle2 = flag ? SkillEditorStyles.ActionLabelSelected : SkillEditorStyles.ActionLabel;
			GUILayout.BeginHorizontal(gUIStyle, new GUILayoutOption[0]);
			int usageCount = Actions.GetUsageCount(actionType);
			string text = (usageCount > 0) ? string.Format(Strings.get_ActionSelector_Count_Postfix(), usageCount) : "";
			float num = base.get_position().get_width() - 42f;
			if (usageCount > 0)
			{
				num -= gUIStyle2.CalcSize(new GUIContent(text)).x + 3f;
			}
			GUILayout.Label(actionLabel, gUIStyle2, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(num)
			});
			if (usageCount > 0)
			{
				GUILayout.FlexibleSpace();
				GUILayout.Label(text, gUIStyle2, new GUILayoutOption[0]);
			}
			GUILayout.EndHorizontal();
			Rect lastRect = GUILayoutUtility.GetLastRect();
			if (this.mousePos.y > this.scrollPosition.y && this.mousePos.y < this.scrollPosition.y + this.scrollViewHeight && lastRect.Contains(this.mousePos))
			{
				this.mouseOverAction = actionType;
			}
			if (this.mouseOverAction == actionType)
			{
				if (this.eventType == null)
				{
					this.SelectAction(actionType, category);
					if (Event.get_current().get_button() == 1 || EditorGUI.get_actionKey())
					{
						this.GenerateActionContextMenu().ShowAsContext();
					}
					if (Event.get_current().get_clickCount() > 1 && !FsmGraphView.EditingDisable)
					{
						this.AddSelectedActionToState();
						this.addingAction = true;
					}
					GUIUtility.ExitGUI();
					return;
				}
				if (!this.addingAction && this.eventType == 3)
				{
					this.DragAction(actionType, category);
					GUIUtility.ExitGUI();
					return;
				}
				if (this.eventType == 1)
				{
					DragAndDropManager.Reset();
					this.addingAction = false;
				}
			}
			if (flag)
			{
				this.beforeSelected = this.prevAction;
				this.beforeSelectedCategory = this.prevActionCategory;
				if (Event.get_current().get_type() == 7)
				{
					this.selectedRect = GUILayoutUtility.GetLastRect();
					this.selectedRect.set_y(this.selectedRect.get_y() - (this.scrollPosition.y + 20f));
					this.selectedRect.set_height(this.selectedRect.get_height() + 20f);
				}
			}
			if (this.prevActionWasSelected)
			{
				this.afterSelected = actionType;
				this.afterSelectedCategory = category;
			}
			this.prevAction = actionType;
			this.prevActionCategory = category;
			this.prevActionWasSelected = flag;
		}
		private void DoBottomPanel()
		{
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			if (this.selectedAction != null)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label(Labels.GetActionLabel(this.selectedAction), SkillEditorStyles.ActionPreviewTitle, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(base.get_position().get_width() - 30f)
				});
				GUILayout.FlexibleSpace();
				if (SkillEditorGUILayout.HelpButton("Online Help"))
				{
					EditorCommands.OpenWikiPage(this.previewAction);
				}
				GUILayout.EndHorizontal();
				string tooltip = Actions.GetTooltip(this.selectedAction);
				GUILayout.Box(tooltip, SkillEditorStyles.LabelWithWordWrap, new GUILayoutOption[0]);
				if (Event.get_current().get_type() == 7)
				{
					this.descriptionHeight = GUILayoutUtility.GetLastRect().get_height();
				}
				ActionEditor.PreviewMode = true;
				EditorGUILayout.Space();
				SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
				if (FsmEditorSettings.ShowActionPreview)
				{
					this.DoSelectedActionPreview();
				}
				ActionEditor.PreviewMode = false;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			bool flag = GUILayout.Toggle(FsmEditorSettings.ShowActionPreview, Strings.get_ActionSelector_Preview(), new GUILayoutOption[0]);
			if (flag != FsmEditorSettings.ShowActionPreview)
			{
				FsmEditorSettings.ShowActionPreview = flag;
				FsmEditorSettings.SaveSettings();
			}
			EditorGUI.BeginDisabledGroup(SkillEditor.SelectedState == null || this.selectedAction == null || FsmGraphView.EditingDisable);
			if (GUILayout.Button(new GUIContent(Strings.get_ActionSelector_Add_Action_To_State(), Strings.get_ActionSelector_Add_Action_To_State_Tooltip()), new GUILayoutOption[0]))
			{
				this.AddSelectedActionToState();
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
		private void DoSelectedActionPreview()
		{
			SkillEditorGUILayout.LabelWidth(150f);
			this.previewScrollViewHeight = Mathf.Min(base.get_position().get_height() - 150f - this.descriptionHeight, this.previewActionGUIHeight);
			this.previewScrollPosition = EditorGUILayout.BeginScrollView(this.previewScrollPosition, new GUILayoutOption[]
			{
				GUILayout.Height(this.previewScrollViewHeight)
			});
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			bool enabled = GUI.get_enabled();
			GUI.set_enabled(false);
			SkillEditor.ActionEditor.OnGUI(this.previewAction);
			GUI.set_enabled(enabled);
			GUILayout.EndVertical();
			if (Event.get_current().get_type() == 7)
			{
				float num = this.previewActionGUIHeight;
				this.previewActionGUIHeight = GUILayoutUtility.GetLastRect().get_height() + 5f;
				if (Math.Abs(this.previewActionGUIHeight - num) > 1f)
				{
					base.Repaint();
					this.autoScroll = true;
				}
			}
			EditorGUILayout.EndScrollView();
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			EditorGUILayout.Space();
		}
		private void HandleKeyboardInput()
		{
			this.prevAction = null;
			int controlID = GUIUtility.GetControlID(1);
			if (Event.get_current().GetTypeForControl(controlID) == 4)
			{
				KeyCode keyCode = Event.get_current().get_keyCode();
				if (keyCode != 13)
				{
					if (keyCode == 27)
					{
						base.Close();
						GUIUtility.ExitGUI();
						return;
					}
					switch (keyCode)
					{
					case 271:
						break;
					case 272:
						return;
					case 273:
						Event.get_current().Use();
						this.SelectPreviousAction();
						GUIUtility.ExitGUI();
						return;
					case 274:
						Event.get_current().Use();
						this.SelectNextAction();
						GUIUtility.ExitGUI();
						return;
					default:
						return;
					}
				}
				this.AddSelectedActionToState();
				return;
			}
		}
		public static void RefreshFilteredList()
		{
			if (ActionSelector.instance == null)
			{
				return;
			}
			ActionSelector.instance.FilterActionList();
		}
		private void FilterActionList()
		{
			Actions.FilterActions(this.searchString, this.searchBox.SearchMode);
		}
		private void SelectFirstMatchingAction()
		{
			if (Actions.FilteredCategories.get_Count() == 0)
			{
				return;
			}
			string text = Actions.FilteredCategories.get_Item(0);
			List<Type> actionsInCategoryFiltered = Actions.GetActionsInCategoryFiltered(text);
			this.SelectAction(actionsInCategoryFiltered.get_Item(0), text);
		}
		private void SelectAction(Type actionType, string category)
		{
			this.autoScroll = true;
			base.Repaint();
			if (actionType == this.selectedAction && category == this.selectedActionCategory)
			{
				return;
			}
			this.selectedAction = actionType;
			this.selectedActionCategory = category;
			this.previewAction = (SkillStateAction)Activator.CreateInstance(actionType);
			this.previewAction.Reset();
			FsmEditorSettings.SelectedActionCategory = Actions.GetCategoryIndex(category);
			FsmEditorSettings.SaveSettings();
		}
		[Localizable(false)]
		private void DragAction(Type actionType, string category)
		{
			DragAndDropManager.SetMode(DragAndDropManager.DragMode.AddAction);
			DragAndDrop.PrepareStartDrag();
			DragAndDrop.set_objectReferences(new Object[0]);
			DragAndDrop.SetGenericData("AddAction", this.selectedAction);
			DragAndDrop.StartDrag("Drag Action");
			Event.get_current().Use();
			this.SelectAction(actionType, category);
		}
		private void SelectPreviousAction()
		{
			if (this.selectedAction == null || this.beforeSelected == null)
			{
				return;
			}
			this.SelectAction(this.beforeSelected, this.beforeSelectedCategory);
		}
		private void SelectNextAction()
		{
			if (this.selectedAction == null || this.afterSelected == null)
			{
				return;
			}
			this.SelectAction(this.afterSelected, this.afterSelectedCategory);
		}
		public void ClearSearch()
		{
			this.searchBox.Clear();
			this.searchString = "";
			this.UpdateSearchResults();
			base.Repaint();
		}
		public void SelectNone()
		{
			this.previewAction = null;
			this.selectedAction = null;
			this.selectedActionCategory = "";
			base.Repaint();
		}
		public void FindAction()
		{
			this.ClearSearch();
			this.SelectAction(ActionSelector.findActionType, Actions.FindFirstCategory(ActionSelector.findActionType));
		}
		private GenericMenu GenerateActionContextMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			if (Actions.GetUsageCount(this.selectedAction) == 0)
			{
				genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Find_Action()));
			}
			else
			{
				List<SkillInfo> usage = Actions.GetUsage(this.selectedAction);
				using (List<SkillInfo>.Enumerator enumerator = usage.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SkillInfo current = enumerator.get_Current();
						genericMenu.AddItem(new GUIContent(string.Format("{0}/{1}", Strings.get_Menu_Find_Action(), Labels.GetFullStateLabel(current.state))), false, new GenericMenu.MenuFunction2(SkillInfo.SelectFsmInfo), current);
					}
				}
			}
			if (SkillEditor.SelectedFsm == null || SkillEditor.SelectedState == null)
			{
				genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Select_a_State_to_add_Actions()));
			}
			else
			{
				genericMenu.AddSeparator("");
				genericMenu.AddItem(new GUIContent(Strings.get_Menu_Add_to_Top_of_Action_List()), false, new GenericMenu.MenuFunction(this.AddSelectedActionToTop));
				if (SkillEditor.StateInspector.SelectedAction == null)
				{
					genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Add_Before_Selected_Action()));
					genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Add_After_Selected_Action()));
				}
				else
				{
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_Add_Before_Selected_Action()), false, new GenericMenu.MenuFunction(this.AddSelectedActionBefore));
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_Add_After_Selected_Action()), false, new GenericMenu.MenuFunction(this.AddSelectedActionAfter));
				}
				genericMenu.AddItem(new GUIContent(Strings.get_Menu_Add_to_End_of_Action_List()), false, new GenericMenu.MenuFunction(this.AddSelectedActionToEnd));
			}
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Find_Script()), false, new GenericMenu.MenuFunction2(ActionScripts.PingAssetByType), this.selectedAction);
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Select_Script()), false, new GenericMenu.MenuFunction2(ActionScripts.SelectAssetByType), this.selectedAction);
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Edit_Script()), false, new GenericMenu.MenuFunction2(ActionScripts.EditAssetByType), this.selectedAction);
			genericMenu.AddSeparator("");
			if (Actions.CategoryContainsAction("Favorites", this.selectedAction))
			{
				genericMenu.AddItem(new GUIContent("Remove From Favorites"), false, new GenericMenu.MenuFunction2(ActionSelector.RemoveFromFavorites), this.selectedAction);
			}
			else
			{
				genericMenu.AddItem(new GUIContent("Add To Favorites"), false, new GenericMenu.MenuFunction2(ActionSelector.AddToFavorites), this.selectedAction);
			}
			return genericMenu;
		}
		public static void FinishAddAction()
		{
			if (ActionSelector.instance == null)
			{
				return;
			}
			ActionSelector.AddActionToRecent(ActionSelector.instance.selectedAction);
			if (FsmEditorSettings.CloseActionBrowserOnEnter)
			{
				ActionSelector.instance.Close();
				if (Event.get_current() != null)
				{
					GUIUtility.ExitGUI();
				}
			}
		}
		public static void AddActionToRecent(Type actionType)
		{
			Actions.InsertActionAtTopOfCategory("Recent", actionType);
			ActionSelector.EnforceRecentCategorySize();
			ActionSelector.RefreshFilteredList();
		}
		private void AddSelectedActionToState()
		{
			if (this.selectedAction == null || SkillEditor.SelectedState == null)
			{
				return;
			}
			SkillEditor.StateInspector.AddAction(this.selectedAction);
			ActionSelector.FinishAddAction();
		}
		private void AddSelectedActionToTop()
		{
			if (this.selectedAction == null || SkillEditor.SelectedState == null)
			{
				return;
			}
			SkillEditor.StateInspector.AddActionToTop(this.selectedAction);
			ActionSelector.FinishAddAction();
		}
		private void AddSelectedActionToEnd()
		{
			if (this.selectedAction == null || SkillEditor.SelectedState == null)
			{
				return;
			}
			SkillEditor.StateInspector.AddActionToEnd(this.selectedAction);
			ActionSelector.FinishAddAction();
		}
		private void AddSelectedActionBefore()
		{
			if (this.selectedAction == null || SkillEditor.SelectedState == null)
			{
				return;
			}
			SkillEditor.StateInspector.AddActionBeforeSelectedAction(this.selectedAction);
			ActionSelector.FinishAddAction();
		}
		private void AddSelectedActionAfter()
		{
			if (this.selectedAction == null || SkillEditor.SelectedState == null)
			{
				return;
			}
			SkillEditor.StateInspector.AddActionAfterSelectedAction(this.selectedAction);
			ActionSelector.FinishAddAction();
		}
		private GenericMenu GenerateSettingsMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Show_Preview()), FsmEditorSettings.ShowActionPreview, new GenericMenu.MenuFunction(this.ToggleShowActionPreview));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Hide_Obsolete_Actions()), FsmEditorSettings.HideObsoleteActions, new GenericMenu.MenuFunction(this.ToggleHideObsoleteActions));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Close_Window_After_Adding_Action()), FsmEditorSettings.CloseActionBrowserOnEnter, new GenericMenu.MenuFunction(ActionSelector.ToggleCloseActionBrowser));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Auto_Refresh_Action_Usage()), FsmEditorSettings.AutoRefreshActionUsage, new GenericMenu.MenuFunction(ActionSelector.ToggleAutoRefreshActionUsage));
			genericMenu.AddItem(new GUIContent("Recent Category Size/10"), FsmEditorSettings.ActionBrowserRecentSize == 10, new GenericMenu.MenuFunction2(ActionSelector.SetRecentSize), 10);
			genericMenu.AddItem(new GUIContent("Recent Category Size/20"), FsmEditorSettings.ActionBrowserRecentSize == 20, new GenericMenu.MenuFunction2(ActionSelector.SetRecentSize), 20);
			genericMenu.AddItem(new GUIContent("Recent Category Size/50"), FsmEditorSettings.ActionBrowserRecentSize == 50, new GenericMenu.MenuFunction2(ActionSelector.SetRecentSize), 50);
			genericMenu.AddItem(new GUIContent("Recent Category Size/100"), FsmEditorSettings.ActionBrowserRecentSize == 100, new GenericMenu.MenuFunction2(ActionSelector.SetRecentSize), 100);
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Disable_Window_When_Playing()), FsmEditorSettings.DisableActionBrowerWhenPlaying, new GenericMenu.MenuFunction(ActionSelector.ToggleDisableWhenPlayingSetting));
			return genericMenu;
		}
		private static void SetRecentSize(object userdata)
		{
			FsmEditorSettings.ActionBrowserRecentSize = (int)userdata;
			FsmEditorSettings.SaveSettings();
			ActionSelector.EnforceRecentCategorySize();
		}
		private static void EnforceRecentCategorySize()
		{
			int actionBrowserRecentSize = FsmEditorSettings.ActionBrowserRecentSize;
			List<Type> actionsInCategory = Actions.GetActionsInCategory("Recent");
			if (actionsInCategory.get_Count() > actionBrowserRecentSize)
			{
				actionsInCategory.RemoveRange(actionBrowserRecentSize, actionsInCategory.get_Count() - actionBrowserRecentSize);
			}
		}
		private void ToggleShowActionPreview()
		{
			FsmEditorSettings.ShowActionPreview = !FsmEditorSettings.ShowActionPreview;
			FsmEditorSettings.SaveSettings();
			base.Repaint();
		}
		private void ToggleHideObsoleteActions()
		{
			FsmEditorSettings.HideObsoleteActions = !FsmEditorSettings.HideObsoleteActions;
			FsmEditorSettings.SaveSettings();
			Actions.BuildList();
			this.FilterActionList();
			base.Repaint();
		}
		private static void ToggleCloseActionBrowser()
		{
			FsmEditorSettings.CloseActionBrowserOnEnter = !FsmEditorSettings.CloseActionBrowserOnEnter;
			FsmEditorSettings.SaveSettings();
		}
		private static void ToggleAutoRefreshActionUsage()
		{
			FsmEditorSettings.AutoRefreshActionUsage = !FsmEditorSettings.AutoRefreshActionUsage;
			FsmEditorSettings.SaveSettings();
		}
		private static void ToggleDisableWhenPlayingSetting()
		{
			FsmEditorSettings.DisableActionBrowerWhenPlaying = !FsmEditorSettings.DisableActionBrowerWhenPlaying;
			FsmEditorSettings.SaveSettings();
		}
	}
}
