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
	public class GlobalEventsWindow : BaseEditorWindow
	{
		private const double doubleClickTime = 0.3;
		private static GlobalEventsWindow instance;
		private Vector2 scrollPosition;
		private SkillEvent selectedEvent;
		private SearchBox searchBox;
		private string searchString = "";
		private double clickTime;
		private List<SkillEvent> filteredEvents = new List<SkillEvent>();
		private readonly Dictionary<SkillEvent, int> usageCount = new Dictionary<SkillEvent, int>();
		private readonly List<SkillEvent> unusedEvents = new List<SkillEvent>();
		private string newEventName = "";
		private bool sortByUsageCount;
		private bool updateEventList;
		public override void Initialize()
		{
			this.isToolWindow = true;
			GlobalEventsWindow.instance = this;
			this.InitWindowTitle();
			base.set_minSize(new Vector2(200f, 200f));
			this.searchString = "";
			this.selectedEvent = null;
			this.InitSearchBox();
			this.BuildFilteredList();
		}
		public void InitWindowTitle()
		{
			base.SetTitle(Strings.get_EventsWindow_Title());
		}
		private void InitSearchBox()
		{
			if (this.searchBox == null)
			{
				this.searchBox = new SearchBox(this)
				{
					SearchModes = new string[]
					{
						"All",
						"Global",
						"Local",
						"System"
					},
					HasPopupSearchModes = true
				};
				SearchBox expr_51 = this.searchBox;
				expr_51.SearchChanged = (EditorApplication.CallbackFunction)Delegate.Combine(expr_51.SearchChanged, new EditorApplication.CallbackFunction(this.UpdateSearchResults));
				this.searchBox.Focus();
			}
		}
		public void OnDisable()
		{
			if (GlobalEventsWindow.instance == this)
			{
				GlobalEventsWindow.instance = null;
			}
		}
		public static void DeselectAll()
		{
			if (GlobalEventsWindow.instance != null)
			{
				GlobalEventsWindow.instance.selectedEvent = null;
				GlobalEventsWindow.instance.Repaint();
			}
		}
		public static void ResetView()
		{
			if (GlobalEventsWindow.instance != null)
			{
				GlobalEventsWindow.instance.selectedEvent = null;
				GlobalEventsWindow.instance.BuildFilteredList();
				GlobalEventsWindow.instance.Repaint();
			}
		}
		public override void DoGUI()
		{
			if (this.updateEventList)
			{
				this.BuildFilteredList();
				this.updateEventList = false;
			}
			if (EditorApplication.get_isPlaying() && FsmEditorSettings.DisableEventBrowserWhenPlaying)
			{
				GUILayout.Label(Strings.get_EventsWindow_Disabled_When_Playing(), new GUILayoutOption[0]);
				FsmEditorSettings.DisableEventBrowserWhenPlaying = !GUILayout.Toggle(!FsmEditorSettings.DisableEventBrowserWhenPlaying, Strings.get_EventsWindow_Enable_When_Playing(), new GUILayoutOption[0]);
				if (GUI.get_changed())
				{
					FsmEditorSettings.SaveSettings();
				}
				return;
			}
			this.DoToolbar();
			this.DoTableHeaders();
			this.DoEventTable();
			if (Event.get_current().get_type() == 16)
			{
				GlobalEventsWindow.GenerateEventManagerMenu().ShowAsContext();
			}
			this.DoBottomPanel();
			if (Event.get_current().get_type() == 1 && Event.get_current().get_button() == 0 && GUIUtility.get_hotControl() == 0)
			{
				GUIUtility.set_keyboardControl(0);
			}
			if (Event.get_current().get_type() == null && GUIUtility.get_keyboardControl() == 0)
			{
				this.selectedEvent = null;
				base.Repaint();
			}
		}
		private void DoToolbar()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[0]);
			this.searchBox.OnGUI();
			if (SkillEditorGUILayout.ToolbarSettingsButton())
			{
				GlobalEventsWindow.GenerateEventManagerMenu().ShowAsContext();
			}
			GUILayout.Space(-5f);
			EditorGUILayout.EndHorizontal();
		}
		private void UpdateSearchResults()
		{
			this.searchString = this.searchBox.SearchString;
			this.BuildFilteredList();
		}
		private void BuildFilteredList()
		{
			SkillEvent.SanityCheckEventList();
			this.UpdateUseCount();
			List<SkillEvent> list;
			switch (this.searchBox.SearchMode)
			{
			case 0:
				list = new List<SkillEvent>(SkillEvent.get_EventList());
				break;
			case 1:
				list = Enumerable.ToList<SkillEvent>(Enumerable.Where<SkillEvent>(SkillEvent.get_EventList(), (SkillEvent p) => p.get_IsGlobal()));
				break;
			case 2:
				list = Enumerable.ToList<SkillEvent>(Enumerable.Where<SkillEvent>(SkillEvent.get_EventList(), (SkillEvent p) => !p.get_IsGlobal()));
				break;
			case 3:
				list = Enumerable.ToList<SkillEvent>(Enumerable.Where<SkillEvent>(SkillEvent.get_EventList(), (SkillEvent p) => p.get_IsSystemEvent()));
				break;
			default:
				list = new List<SkillEvent>(SkillEvent.get_EventList());
				break;
			}
			if (string.IsNullOrEmpty(this.searchString))
			{
				this.filteredEvents = new List<SkillEvent>(list);
				this.filteredEvents.Sort();
				return;
			}
			this.filteredEvents.Clear();
			string text = this.searchString.ToUpper();
			using (List<SkillEvent>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					if (current.get_Name().ToUpper().Contains(text))
					{
						this.filteredEvents.Add(current);
					}
				}
			}
			this.filteredEvents.Sort();
		}
		private void DoTableHeaders()
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			bool changed = GUI.get_changed();
			GUI.set_changed(false);
			GUILayout.BeginHorizontal(SkillEditorStyles.TableRowBox, new GUILayoutOption[0]);
			GUILayout.Label(new GUIContent(SkillEditorStyles.BroadcastIcon, Strings.get_Tooltip_Global_Event_Flag()), new GUILayoutOption[]
			{
				GUILayout.MinWidth(20f)
			});
			this.sortByUsageCount = !GUILayout.Toggle(!this.sortByUsageCount, new GUIContent(Strings.get_Label_Event(), Strings.get_Tooltip_Event_GUI()), SkillEditorStyles.TableRowHeader, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			this.sortByUsageCount = GUILayout.Toggle(this.sortByUsageCount, new GUIContent(Strings.get_Label_Used(), Strings.get_Tooltip_Events_Used()), SkillEditorStyles.TableRowHeader, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(40f)
			});
			GUILayout.Space(20f);
			GUILayout.EndHorizontal();
			if (!GUI.get_changed())
			{
				GUI.set_changed(changed);
			}
			EditorGUILayout.EndHorizontal();
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_Events_Window(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
		}
		private void DoEventTable()
		{
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]);
			using (List<SkillEvent>.Enumerator enumerator = this.filteredEvents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					this.DoEventLine(current);
				}
			}
			GUILayout.Space(20f);
			GUILayout.EndScrollView();
			GUILayout.FlexibleSpace();
		}
		[Localizable(false)]
		private void DoEventLine(SkillEvent fsmEvent)
		{
			int num;
			this.usageCount.TryGetValue(fsmEvent, ref num);
			if (num == 0 && FsmEditorSettings.HideUnusedEvents)
			{
				return;
			}
			GUILayout.BeginHorizontal((this.selectedEvent != null && fsmEvent.get_Name() == this.selectedEvent.get_Name()) ? SkillEditorStyles.SelectedEventBox : SkillEditorStyles.TableRowBox, new GUILayoutOption[0]);
			EditorGUI.BeginDisabledGroup(fsmEvent.get_IsSystemEvent());
			bool flag = GUILayout.Toggle(fsmEvent.get_IsGlobal(), new GUIContent("", Strings.get_Label_Global()), SkillEditorStyles.TableRowCheckBox, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(17f),
				GUILayout.MinWidth(17f)
			});
			if (flag != fsmEvent.get_IsGlobal())
			{
				SkillEditor.Builder.SetEventIsGlobal(null, fsmEvent, flag);
			}
			EditorGUI.EndDisabledGroup();
			GUIStyle gUIStyle = (this.selectedEvent != null && fsmEvent.get_Name() == this.selectedEvent.get_Name()) ? SkillEditorStyles.TableRowTextSelected : SkillEditorStyles.TableRowText;
			if (GUILayout.Button(fsmEvent.get_Name(), gUIStyle, new GUILayoutOption[]
			{
				GUILayout.MinWidth(base.get_position().get_width() - 100f)
			}))
			{
				this.SelectEvent(fsmEvent);
				if (Event.get_current().get_button() == 1 || EditorGUI.get_actionKey())
				{
					this.GenerateUsageContextMenu(this.selectedEvent).ShowAsContext();
				}
				if (EditorApplication.get_timeSinceStartup() - this.clickTime < 0.3)
				{
					this.AddSelectedEventToState();
				}
				this.clickTime = EditorApplication.get_timeSinceStartup();
			}
			GUILayout.FlexibleSpace();
			GUILayout.Label(num.ToString(CultureInfo.get_CurrentCulture()), gUIStyle, new GUILayoutOption[0]);
			GUILayout.Space(10f);
			EditorGUI.BeginDisabledGroup(fsmEvent.get_IsSystemEvent());
			if (SkillEditorGUILayout.DeleteButton() && Dialogs.YesNoDialog(Strings.get_Dialog_Delete_Event(), string.Format(Strings.get_Dialog_Delete_Event_Are_you_sure(), (num > 0) ? string.Concat(new object[]
			{
				"\n\n",
				Strings.get_Dialog_Delete_Event_Used_By(),
				num,
				(num > 1) ? Strings.get_Label_Postfix_FSMs_Plural() : Strings.get_Label_Postfix_FSM()
			}) : "")))
			{
				EditorCommands.DeleteEventFromAll(fsmEvent);
				SkillEditor.EventManager.Reset();
				SkillEvent.RemoveEventFromEventList(fsmEvent);
				if (fsmEvent.get_IsGlobal())
				{
					SkillEvent.get_globalEvents().RemoveAll((string r) => r == fsmEvent.get_Name());
					SkillEditor.SaveGlobals();
				}
				this.BuildFilteredList();
				base.Repaint();
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
		}
		private void DoEventEditor()
		{
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			if (EditorWindow.get_focusedWindow() != this)
			{
				return;
			}
			if (this.selectedEvent == null || this.selectedEvent.get_IsSystemEvent())
			{
				return;
			}
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(Strings.get_Command_Rename(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(80f)
			});
			this.newEventName = EditorGUILayout.TextField(this.newEventName, new GUILayoutOption[0]);
			EditorGUILayout.EndHorizontal();
			string text = SkillEditor.Builder.ValidateRenameEvent(this.selectedEvent, this.newEventName);
			bool flag = string.IsNullOrEmpty(text);
			if (!flag)
			{
				GUILayout.Box(text, SkillEditorStyles.ErrorBox, new GUILayoutOption[0]);
			}
			if (Event.get_current().get_isKey())
			{
				if (flag && Keyboard.EnterKeyPressed())
				{
					this.RenameEvent();
					GUIUtility.ExitGUI();
				}
				if (Event.get_current().get_keyCode() == 27)
				{
					this.newEventName = this.selectedEvent.get_Name();
				}
			}
		}
		public void UpdateUseCount()
		{
			if (EditorApplication.get_isPlayingOrWillChangePlaymode())
			{
				return;
			}
			this.usageCount.Clear();
			this.unusedEvents.Clear();
			using (List<SkillEvent>.Enumerator enumerator = SkillEvent.get_EventList().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					int num = GlobalEventsWindow.CountEventUsage(current);
					this.usageCount.Add(current, num);
					if (num == 0 && !current.get_IsSystemEvent())
					{
						this.unusedEvents.Add(current);
					}
				}
			}
		}
		private static int CountEventUsage(SkillEvent fsmEvent)
		{
			if (fsmEvent == null)
			{
				return 0;
			}
			int num = 0;
			using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					SkillEvent[] events = current.get_Events();
					for (int i = 0; i < events.Length; i++)
					{
						SkillEvent fsmEvent2 = events[i];
						if (fsmEvent.get_Name() == fsmEvent2.get_Name())
						{
							num++;
							break;
						}
					}
				}
			}
			return num;
		}
		private GenericMenu GenerateUsageContextMenu(SkillEvent fsmEvent)
		{
			this.UpdateUseCount();
			GenericMenu genericMenu = new GenericMenu();
			int num;
			this.usageCount.TryGetValue(fsmEvent, ref num);
			if (num == 0)
			{
				genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_No_FSMs_Use_This_Event()));
				genericMenu.AddSeparator("");
				if (SkillEditor.SelectedFsm == null || SkillEditor.SelectedFsm.HasEvent(this.selectedEvent.get_Name()))
				{
					genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Add_Event_to_FSM()));
				}
				else
				{
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_Add_Event_to_FSM()), false, new GenericMenu.MenuFunction(this.AddSelectedEventToState));
				}
				return genericMenu;
			}
			List<string> list = new List<string>();
			using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					if (current != null && current.get_OwnerObject() != null)
					{
						SkillEvent[] events = current.get_Events();
						for (int i = 0; i < events.Length; i++)
						{
							SkillEvent fsmEvent2 = events[i];
							if (fsmEvent2.get_Name() == fsmEvent.get_Name())
							{
								string text = Labels.GenerateUniqueLabel(list, Labels.GetFullFsmLabel(current));
								genericMenu.AddItem(new GUIContent(text), SkillEditor.SelectedFsm == current, new GenericMenu.MenuFunction2(EditorCommands.SelectFsm), current);
								list.Add(text);
							}
						}
					}
				}
			}
			genericMenu.AddSeparator("");
			if (SkillEditor.SelectedFsm == null || SkillEditor.SelectedFsm.HasEvent(this.selectedEvent.get_Name()))
			{
				genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Add_Event_to_FSM()));
			}
			else
			{
				genericMenu.AddItem(new GUIContent(Strings.get_Menu_Add_Event_to_FSM()), false, new GenericMenu.MenuFunction(this.AddSelectedEventToState));
			}
			return genericMenu;
		}
		public static void SyncSelection(SkillEvent fsmEvent)
		{
			if (GlobalEventsWindow.instance != null)
			{
				GlobalEventsWindow.instance.SelectEvent(fsmEvent);
				GlobalEventsWindow.instance.Repaint();
			}
		}
		private void SelectEvent(SkillEvent fsmEvent)
		{
			if (Event.get_current() != null)
			{
				GUIUtility.set_keyboardControl(0);
			}
			if (fsmEvent == null)
			{
				return;
			}
			this.newEventName = ((!fsmEvent.get_IsSystemEvent()) ? fsmEvent.get_Name() : "");
			this.selectedEvent = fsmEvent;
			SkillEditor.EventManager.SelectEvent(fsmEvent, false);
		}
		private void RenameEvent()
		{
			SkillEditor.Builder.RenameEvent(this.selectedEvent.get_Name(), this.newEventName);
			GlobalEventsWindow.DeselectAll();
			SkillEditor.EventManager.DeselectAll();
			SkillEditor.RepaintAll();
		}
		private void AddSelectedEventToState()
		{
			if (this.selectedEvent == null || SkillEditor.SelectedFsm == null || SkillEditor.SelectedFsm.HasEvent(this.selectedEvent.get_Name()))
			{
				return;
			}
			EditorCommands.AddEvent(this.selectedEvent.get_Name());
			SkillEditor.Inspector.ResetView();
		}
		private void DoBottomPanel()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			bool enabled = GUI.get_enabled();
			if (SkillEditor.SelectedFsm == null || this.selectedEvent == null || SkillEditor.SelectedFsm.HasEvent(this.selectedEvent.get_Name()))
			{
				GUI.set_enabled(false);
			}
			if (GUILayout.Button(Strings.get_Command_Add_Selected_Event_To_FSM(), new GUILayoutOption[0]))
			{
				this.AddSelectedEventToState();
			}
			GUI.set_enabled(enabled);
			if (SkillEditorGUILayout.HelpButton("Online Help"))
			{
				EditorCommands.OpenWikiPage(WikiPages.EventBrowser);
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
		private static GenericMenu GenerateEventManagerMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Hide_Unused_Events()), FsmEditorSettings.HideUnusedEvents, new GenericMenu.MenuFunction(GlobalEventsWindow.ToggleHideUnusedEvents));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Disable_Window_When_Playing()), FsmEditorSettings.DisableEventBrowserWhenPlaying, new GenericMenu.MenuFunction(GlobalEventsWindow.ToggleDisableWindow));
			return genericMenu;
		}
		private static void ToggleHideUnusedEvents()
		{
			FsmEditorSettings.HideUnusedEvents = !FsmEditorSettings.HideUnusedEvents;
			FsmEditorSettings.SaveSettings();
		}
		private static void ToggleDisableWindow()
		{
			FsmEditorSettings.DisableEventBrowserWhenPlaying = !FsmEditorSettings.DisableEventBrowserWhenPlaying;
			FsmEditorSettings.SaveSettings();
		}
	}
}
