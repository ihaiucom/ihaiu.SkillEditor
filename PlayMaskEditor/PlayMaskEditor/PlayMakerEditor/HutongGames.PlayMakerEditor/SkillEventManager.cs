using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	internal class FsmEventManager
	{
		private const int NameColumnWidth = 244;
		private List<SkillEvent> eventList = new List<SkillEvent>();
		private Vector2 scrollPosition;
		private SkillEvent selectedEvent;
		private string newEventName = "";
		private bool sortByUsageCount;
		public void Reset()
		{
			this.DeselectAll();
			this.UpdateEventList();
			SkillEditor.Repaint(true);
			SkillEditor.RepaintAll();
			GlobalEventsWindow.ResetView();
		}
		public void DeselectAll()
		{
			Keyboard.ResetFocus();
			this.selectedEvent = null;
			this.newEventName = "";
			SkillEditor.Repaint(true);
		}
		private void UpdateEventList()
		{
			using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					FsmEventManager.SanityCheckEventList(current);
				}
			}
			if (SkillEditor.SelectedFsm != null)
			{
				this.eventList = new List<SkillEvent>(SkillEditor.SelectedFsm.get_Events());
				this.SortEvents();
				return;
			}
			this.eventList = new List<SkillEvent>();
		}
		private GenericMenu GenerateEventManagerMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Remove_Unused_Events()), false, new GenericMenu.MenuFunction(this.DeleteUnusedEvents));
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Event_Browser()), false, new GenericMenu.MenuFunction(SkillEditor.OpenGlobalEventsWindow));
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Online_Help()), false, new GenericMenu.MenuFunction(FsmEventManager.OpenOnlineHelp));
			return genericMenu;
		}
		private static void OpenOnlineHelp()
		{
			EditorCommands.OpenWikiPage(WikiPages.EventManager);
		}
		private void DeleteUnusedEvents()
		{
			SkillSearch.Update(SkillEditor.SelectedFsm);
			List<string> unusedEvents = SkillSearch.GetUnusedEvents(SkillEditor.SelectedFsm);
			int count = unusedEvents.get_Count();
			if (count == 0)
			{
				EditorUtility.DisplayDialog(Strings.get_Dialog_Delete_Unused_Events(), Strings.get_Dialog_No_unused_events(), Strings.get_OK());
				return;
			}
			if (Dialogs.YesNoDialog(Strings.get_Dialog_Delete_Unused_Events(), string.Format(Strings.get_Dialog_Delete_Unused_Events_Are_you_sure(), count)))
			{
				SkillEditor.RegisterUndo(Strings.get_Dialog_Delete_Unused_Events());
				using (List<string>.Enumerator enumerator = unusedEvents.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.get_Current();
						if (!SkillEvent.IsEventGlobal(current))
						{
							SkillEditor.Builder.DeleteEvent(SkillEditor.SelectedFsm, current);
						}
					}
				}
				this.Reset();
			}
			SkillEditor.SetFsmDirty(true, false);
		}
		private GenericMenu GenerateStateListMenu(SkillEvent fsmEvent)
		{
			GenericMenu genericMenu = new GenericMenu();
			if (SkillSearch.GetEventUseCount(SkillEditor.SelectedFsm, fsmEvent.get_Name()) == 0)
			{
				genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Unused_Event()));
				return genericMenu;
			}
			List<SkillState> list = SkillInfo.FindStatesUsingEvent(SkillEditor.SelectedFsm, fsmEvent.get_Name());
			using (List<SkillState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillState current = enumerator.get_Current();
					genericMenu.AddItem(new GUIContent(current.get_Name()), SkillEditor.SelectedState == current, new GenericMenu.MenuFunction2(EditorCommands.SelectState), current);
				}
			}
			return genericMenu;
		}
		private void SortEvents()
		{
			if (this.sortByUsageCount)
			{
				this.eventList.Sort(new Comparison<SkillEvent>(this.CompareEventsByUseCount));
				return;
			}
			this.eventList.Sort();
		}
		private int CompareEventsByUseCount(SkillEvent event1, SkillEvent event2)
		{
			if (event1 == null)
			{
				if (event2 == null)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (event2 == null)
				{
					return 1;
				}
				int eventUseCount = SkillSearch.GetEventUseCount(SkillEditor.SelectedFsm, event1.get_Name());
				int num = SkillSearch.GetEventUseCount(SkillEditor.SelectedFsm, event2.get_Name()).CompareTo(eventUseCount);
				if (num == 0)
				{
					return string.Compare(event1.get_Name(), event2.get_Name(), 0);
				}
				return num;
			}
		}
		public void OnGUI()
		{
			if (SkillEditor.SelectedFsm == null)
			{
				GUILayout.FlexibleSpace();
				return;
			}
			this.DoTableHeaders();
			this.DoEventTable();
			if (Event.get_current().get_type() == 16)
			{
				this.GenerateEventManagerMenu().ShowAsContext();
			}
			this.DoEventEditor();
			EditorGUILayout.Space();
			if (GUILayout.Button(SkillEditorContent.EventBrowserButtonLabel, new GUILayoutOption[0]))
			{
				SkillEditor.OpenGlobalEventsWindow();
				GUIUtility.ExitGUI();
			}
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Tooltip_Event_Browser_Button_in_Events_Tab(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			EditorGUILayout.Space();
			if (Event.get_current().get_type() == null && GUIUtility.get_keyboardControl() == 0)
			{
				this.Reset();
			}
		}
		private void DoTableHeaders()
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			bool changed = GUI.get_changed();
			GUI.set_changed(false);
			GUILayout.BeginHorizontal(SkillEditorStyles.TableRowBox, new GUILayoutOption[0]);
			GUILayout.Toggle(false, SkillEditorContent.EventBroadcastIcon, SkillEditorStyles.TableRowHeader, new GUILayoutOption[0]);
			this.sortByUsageCount = !GUILayout.Toggle(!this.sortByUsageCount, SkillEditorContent.EventHeaderLabel, SkillEditorStyles.TableRowHeader, new GUILayoutOption[]
			{
				GUILayout.MinWidth(244f)
			});
			this.sortByUsageCount = GUILayout.Toggle(this.sortByUsageCount, SkillEditorContent.EventUsedHeaderLabel, SkillEditorStyles.TableRowHeader, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (SkillEditorGUILayout.SettingsButtonPadded())
			{
				this.GenerateEventManagerMenu().ShowAsContext();
			}
			GUILayout.EndHorizontal();
			if (GUI.get_changed())
			{
				this.SortEvents();
			}
			else
			{
				GUI.set_changed(changed);
			}
			EditorGUILayout.EndHorizontal();
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_EventManager(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
		}
		private void DoEventTable()
		{
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]);
			if (this.eventList.get_Count() == 0)
			{
				GUILayout.BeginHorizontal(SkillEditorStyles.TableRowBox, new GUILayoutOption[0]);
				GUILayout.Label(Strings.get_Label_None_In_Table(), new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
			}
			using (List<SkillEvent>.Enumerator enumerator = this.eventList.GetEnumerator())
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
		private void DoEventLine(SkillEvent fsmEvent)
		{
			int eventUseCount = SkillSearch.GetEventUseCount(SkillEditor.SelectedFsm, fsmEvent.get_Name());
			GUILayout.BeginHorizontal((this.selectedEvent != null && fsmEvent.get_Name() == this.selectedEvent.get_Name()) ? SkillEditorStyles.SelectedEventBox : SkillEditorStyles.TableRowBox, new GUILayoutOption[0]);
			EditorGUI.BeginDisabledGroup(fsmEvent.get_IsSystemEvent());
			bool flag = GUILayout.Toggle(fsmEvent.get_IsGlobal(), SkillEditorContent.GlobalEventTooltipLabel, SkillEditorStyles.TableRowCheckBox, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(17f),
				GUILayout.MinWidth(17f)
			});
			if (flag != fsmEvent.get_IsGlobal())
			{
				SkillEditor.Builder.SetEventIsGlobal(SkillEditor.SelectedFsm, fsmEvent, flag);
				GlobalEventsWindow.ResetView();
			}
			EditorGUI.EndDisabledGroup();
			GUIStyle gUIStyle = (this.selectedEvent != null && fsmEvent.get_Name() == this.selectedEvent.get_Name()) ? SkillEditorStyles.TableRowTextSelected : SkillEditorStyles.TableRowText;
			if (GUILayout.Button(fsmEvent.get_Name(), gUIStyle, new GUILayoutOption[]
			{
				GUILayout.MinWidth(244f)
			}))
			{
				this.SelectEvent(fsmEvent, true);
				GUIUtility.set_keyboardControl(0);
				if (Event.get_current().get_button() == 1 || EditorGUI.get_actionKey())
				{
					this.GenerateStateListMenu(this.selectedEvent).ShowAsContext();
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.Label(eventUseCount.ToString(CultureInfo.get_CurrentCulture()), gUIStyle, new GUILayoutOption[0]);
			GUILayout.Space(5f);
			if (SkillEditorGUILayout.DeleteButton())
			{
				EditorCommands.DeleteEvent(fsmEvent);
				this.Reset();
			}
			GUILayout.EndHorizontal();
		}
		private void DoEventEditor()
		{
			if (EditorWindow.get_focusedWindow() != SkillEditor.Window)
			{
				return;
			}
			SkillEditorGUILayout.LabelWidth(86f);
			bool flag = !SkillEvent.IsNullOrEmpty(this.selectedEvent);
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			if (flag && FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(this.selectedEvent.get_IsSystemEvent() ? Strings.get_Hint_System_Events_cannot_be_renamed() : Strings.get_Hint_Use_Event_Browser_to_rename_globally(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			if (flag && this.selectedEvent.get_IsGlobal())
			{
				SkillEditorGUILayout.ReadonlyTextField(SkillEditorContent.GlobalEventName, 82f, this.newEventName, new GUILayoutOption[0]);
			}
			else
			{
				if (!flag || !this.selectedEvent.get_IsSystemEvent())
				{
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label(flag ? SkillEditorContent.EditEventNameLabel : SkillEditorContent.AddEventLabel, new GUILayoutOption[]
					{
						GUILayout.MaxWidth(80f)
					});
					this.newEventName = EditorGUILayout.TextField(this.newEventName, new GUILayoutOption[0]);
					string text = SkillEditorGUILayout.FsmEventListPopup();
					if (text != "")
					{
						this.AddEvent(text);
						return;
					}
					EditorGUILayout.EndHorizontal();
					if (!flag && FsmEditorSettings.ShowHints)
					{
						GUILayout.Box(Strings.get_Tooltip_EventManager_Add_Event(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
					}
				}
			}
			if (flag)
			{
				bool flag2 = false;
				using (List<SkillEvent>.Enumerator enumerator = SkillEditor.SelectedFsm.ExposedEvents.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SkillEvent current = enumerator.get_Current();
						if (current.get_Name() == this.selectedEvent.get_Name())
						{
							flag2 = true;
							break;
						}
					}
				}
				bool flag3 = EditorGUILayout.Toggle(SkillEditorContent.EventInspectorLabel, flag2, new GUILayoutOption[0]);
				if (flag2 != flag3)
				{
					if (!flag3)
					{
						EditorCommands.RemoveExposedEvent(SkillEditor.SelectedFsm, this.selectedEvent);
					}
					else
					{
						EditorCommands.AddExposedEvent(SkillEditor.SelectedFsm, this.selectedEvent);
					}
					SkillEditor.SetFsmDirty(false, false);
				}
				if (FsmEditorSettings.ShowHints)
				{
					GUILayout.Box(Strings.get_Hint_EventManager_Expose_Events(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
				}
			}
			string text2 = this.ValidateEventName(flag);
			bool flag4 = string.IsNullOrEmpty(text2);
			if (!flag4)
			{
				GUILayout.Box(text2, SkillEditorStyles.ErrorBox, new GUILayoutOption[0]);
			}
			if (Event.get_current().get_isKey())
			{
				if (flag4 && Keyboard.EnterKeyPressed())
				{
					if (!flag)
					{
						this.AddEvent(this.newEventName);
					}
					else
					{
						this.RenameEvent(this.selectedEvent, this.newEventName);
					}
					Event.get_current().Use();
					GUIUtility.ExitGUI();
					return;
				}
				if (Event.get_current().get_keyCode() == 27)
				{
					this.Reset();
				}
			}
		}
		private string ValidateEventName(bool editingEvent)
		{
			if (editingEvent && this.selectedEvent.get_IsSystemEvent())
			{
				return null;
			}
			if (!editingEvent)
			{
				return SkillEditor.Builder.ValidateAddEvent(this.newEventName);
			}
			return SkillEditor.Builder.ValidateRenameEvent(this.selectedEvent, this.newEventName);
		}
		public void SelectEvent(SkillEvent fsmEvent, bool syncSelection = true)
		{
			if (syncSelection)
			{
				GlobalEventsWindow.SyncSelection(fsmEvent);
			}
			this.selectedEvent = fsmEvent;
			if (SkillEvent.IsNullOrEmpty(fsmEvent))
			{
				this.newEventName = "";
				return;
			}
			this.newEventName = ((!fsmEvent.get_IsSystemEvent()) ? fsmEvent.get_Name() : "");
			SkillEditor.Repaint(true);
		}
		[Localizable(false)]
		private void AddEvent(string eventName)
		{
			if (eventName.Replace(" ", "") == "")
			{
				EditorApplication.Beep();
				EditorUtility.DisplayDialog(Strings.get_Label_Add_Event(), Strings.get_Error_Invalid_Name(), Strings.get_OK());
				return;
			}
			EditorCommands.AddEvent(eventName);
			this.Reset();
		}
		private void RenameEvent(SkillEvent fsmEvent, string newName)
		{
			if (fsmEvent == null)
			{
				return;
			}
			if (fsmEvent.get_IsGlobal())
			{
				if (Dialogs.AreYouSure(Strings.get_Dialog_Rename_Event(), Strings.get_Dialog_Rename_Event_Are_you_sure()))
				{
					SkillEditor.Builder.RenameEvent(fsmEvent.get_Name(), newName);
				}
			}
			else
			{
				SkillEditor.Builder.RenameEvent(SkillEditor.SelectedFsm, fsmEvent.get_Name(), newName);
			}
			SkillEditor.SelectedFsm.set_Events(ArrayUtility.Sort<SkillEvent>(SkillEditor.SelectedFsm.get_Events()));
			this.Reset();
		}
		public static void SanityCheckEventList(Skill fsm)
		{
			bool flag = false;
			List<SkillEvent> list = new List<SkillEvent>();
			SkillEvent[] events = fsm.get_Events();
			for (int i = 0; i < events.Length; i++)
			{
				SkillEvent fsmEvent = events[i];
				if (!SkillEvent.EventListContainsEvent(list, fsmEvent.get_Name()))
				{
					list.Add(fsmEvent);
				}
				else
				{
					flag = true;
					Debug.LogError(string.Format(Strings.get_Error_Duplicate_Event_Found__(), fsmEvent.get_Name()));
				}
			}
			if (flag)
			{
				fsm.set_Events(list.ToArray());
				SkillEditor.SetFsmDirty(fsm, false, false, true);
			}
		}
	}
}
