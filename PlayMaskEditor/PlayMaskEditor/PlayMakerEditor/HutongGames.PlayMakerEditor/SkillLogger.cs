using HutongGames.Editor;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class SkillLogger : BaseEditorWindow
	{
		private static SkillLogger instance;
		private Vector2 scrollPosition;
		private float scrollViewHeight;
		private Rect selectedRect;
		private bool autoScroll;
		private SkillState currentState;
		private int numEntriesDrawn;
		private List<float> entryHeights = new List<float>();
		private int firstVisibleEntry;
		private int lastVisibleEntry;
		private float prevWindowHeight;
		private bool updateVisibility;
		private SkillLogEntry prevEntry;
		private SkillLogEntry beforeSelected;
		private SkillLogEntry afterSelected;
		private static SkillLog selectedLog
		{
			get
			{
				return DebugFlow.SelectedLog;
			}
		}
		private SkillLogEntry selectedEntry
		{
			get
			{
				return DebugFlow.SelectedLogEntry;
			}
		}
		private int selectedEntryIndex
		{
			get
			{
				return DebugFlow.SelectedLogEntryIndex;
			}
		}
		public static void ResetView()
		{
			if (SkillLogger.instance != null)
			{
				SkillLogger.instance.entryHeights.Clear();
				SkillLogger.instance.firstVisibleEntry = 0;
				SkillLogger.instance.lastVisibleEntry = 0;
				SkillLogger.instance.updateVisibility = true;
			}
		}
		public static void SetDebugFlowTime(float time)
		{
			if (SkillLogger.instance != null)
			{
				SkillLogger.instance.Repaint();
			}
		}
		public void InitWindowTitle()
		{
			base.SetTitle(Strings.get_FsmLogger_Title());
		}
		public override void Initialize()
		{
			this.isToolWindow = true;
			SkillLogger.instance = this;
			this.InitWindowTitle();
			base.set_minSize(new Vector2(200f, 200f));
			DebugFlow.SyncFsmLog(SkillEditor.SelectedFsm);
		}
		private void OnDisable()
		{
			if (SkillLogger.instance == this)
			{
				SkillLogger.instance = null;
			}
		}
		public override void DoGUI()
		{
			this.DoMainToolbar();
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_FsmLog(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			if (!FsmEditorSettings.EnableLogging)
			{
				SkillLogger.DoDisabledGUI();
				return;
			}
			GUI.set_enabled(SkillLogger.selectedLog != null);
			this.HandleKeyboardInput();
			this.DoLogView();
			this.DoBottomToolbar();
		}
		private void Update()
		{
			if (SkillLogger.selectedLog == null || SkillLogger.selectedLog.get_Entries() == null)
			{
				return;
			}
			if (SkillLogger.selectedLog.get_Resized())
			{
				SkillLogger.ResetView();
				SkillLogger.selectedLog.set_Resized(false);
			}
			this.HandleWindowResize();
			this.UpdateLayout();
			if (this.updateVisibility)
			{
				this.UpdateVisibility();
				base.Repaint();
			}
		}
		private void HandleWindowResize()
		{
			if (this.prevWindowHeight == 0f)
			{
				this.prevWindowHeight = base.get_position().get_height();
				return;
			}
			if (this.prevWindowHeight != base.get_position().get_height())
			{
				this.UpdateVisibility();
			}
			this.prevWindowHeight = base.get_position().get_height();
		}
		private static void DoDisabledGUI()
		{
			GUILayout.Label(Strings.get_Label_Logging_is_disabled_in_Preferences(), new GUILayoutOption[0]);
			if (GUILayout.Button(Strings.get_Action_Enable_Logging(), new GUILayoutOption[0]))
			{
				FsmEditorSettings.EnableLogging = true;
				FsmEditorSettings.SaveSettings();
			}
		}
		private void DoLogView()
		{
			if (SkillLogger.selectedLog == null || SkillLogger.selectedLog.get_Entries() == null || this.entryHeights.get_Count() != SkillLogger.selectedLog.get_Entries().get_Count())
			{
				GUILayout.FlexibleSpace();
				return;
			}
			this.currentState = null;
			this.numEntriesDrawn = 0;
			Vector2 vector = GUILayout.BeginScrollView(this.scrollPosition, false, true, new GUILayoutOption[0]);
			if (vector != this.scrollPosition)
			{
				this.scrollPosition = vector;
				this.updateVisibility = true;
			}
			if (SkillLogger.selectedLog != null && SkillLogger.selectedLog.get_Entries() != null)
			{
				float entryPosition = this.GetEntryPosition(this.firstVisibleEntry);
				GUILayout.Space(entryPosition);
				for (int i = this.firstVisibleEntry; i < this.lastVisibleEntry; i++)
				{
					SkillLogEntry fsmLogEntry = SkillLogger.selectedLog.get_Entries().get_Item(i);
					this.DoLogLine(fsmLogEntry, i);
					if (this.numEntriesDrawn > 0 && this.eventType == 7 && fsmLogEntry == this.selectedEntry)
					{
						this.selectedRect = GUILayoutUtility.GetLastRect();
						this.selectedRect.set_y(this.selectedRect.get_y() - this.scrollPosition.y);
					}
				}
				if (this.lastVisibleEntry < this.entryHeights.get_Count())
				{
					GUILayout.Space(this.GetEntryPosition(this.entryHeights.get_Count()) - this.entryHeights.get_Item(this.lastVisibleEntry));
				}
			}
			GUILayout.EndScrollView();
			if (this.eventType == 7)
			{
				this.scrollViewHeight = GUILayoutUtility.GetLastRect().get_height();
				this.DoAutoScroll();
			}
		}
		private void DoAutoScroll()
		{
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
		[Localizable(false)]
		private void DoLogLine(SkillLogEntry entry, int index)
		{
			if (!this.EntryIsVisible(entry))
			{
				return;
			}
			if (entry.get_LogType() == 6)
			{
				this.currentState = entry.get_State();
				SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			}
			if (this.selectedEntry != null && index > this.selectedEntryIndex)
			{
				GUI.set_color(new Color(1f, 1f, 1f, 0.3f));
			}
			if (entry.get_LogType() == 9 || entry.get_LogType() == 10)
			{
				GUI.set_backgroundColor(SkillEditorStyles.DefaultBackgroundColor);
			}
			else
			{
				GUI.set_backgroundColor((this.currentState != null) ? PlayMakerPrefs.get_Colors()[this.currentState.get_ColorIndex()] : Color.get_grey());
			}
			GUILayout.BeginVertical(SkillEditorStyles.LogBackground, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			Color backgroundColor = GUI.get_backgroundColor();
			GUI.set_backgroundColor(Color.get_white());
			GUIStyle gUIStyle = SkillEditorStyles.GetLogTypeStyles()[entry.get_LogType()];
			GUILayout.Label("", gUIStyle, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(20f)
			});
			GUI.set_backgroundColor(backgroundColor);
			gUIStyle = SkillEditorStyles.LogLine;
			if (GUILayout.Button(FsmEditorSettings.LogShowTimecode ? entry.get_TextWithTimecode() : entry.get_Text(), gUIStyle, new GUILayoutOption[0]))
			{
				this.SelectLogEntry(entry);
			}
			GUILayout.EndHorizontal();
			if (this.ShowSentBy(entry))
			{
				if (string.IsNullOrEmpty(entry.get_Text2()))
				{
					entry.set_Text2(Strings.get_FsmLog_Label_Sent_By() + Labels.GetFullStateLabel(entry.get_SentByState()));
					if (entry.get_Action() != null)
					{
						entry.set_Text2(entry.get_Text2() + " : " + Labels.GetActionLabel(entry.get_Action()));
					}
				}
				if (GUILayout.Button(entry.get_Text2(), SkillEditorStyles.LogLine2, new GUILayoutOption[0]))
				{
					SkillLogger.OnClickSentBy(entry);
				}
				EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), 4);
			}
			else
			{
				if (this.ShowEventTarget(entry))
				{
					if (string.IsNullOrEmpty(entry.get_Text2()))
					{
						entry.set_Text2(Strings.get_FsmLog_Label_Target() + SkillLogger.GetEventTargetLabel(entry));
					}
					if (GUILayout.Button(entry.get_Text2(), SkillEditorStyles.LogLine2, new GUILayoutOption[0]))
					{
						this.OnClickEventTarget(entry);
						GUIUtility.ExitGUI();
						return;
					}
					EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), 4);
				}
				else
				{
					if (this.ShowHitGameObject(entry))
					{
						if (string.IsNullOrEmpty(entry.get_Text2()))
						{
							entry.set_Text2("WITH: " + entry.get_GameObjectName());
							entry.set_GameObjectIcon(EditorHacks.GetIconForObject(entry.get_GameObject()));
						}
						if (entry.get_GameObject() != null)
						{
							if (GUILayout.Button(entry.get_Text2(), SkillEditorStyles.LogLine2, new GUILayoutOption[0]))
							{
								Selection.set_activeGameObject(entry.get_GameObject());
								GUIUtility.ExitGUI();
								return;
							}
							Rect lastRect = GUILayoutUtility.GetLastRect();
							EditorGUIUtility.AddCursorRect(lastRect, 4);
							if (entry.get_GameObjectIcon() != null)
							{
								lastRect.Set(lastRect.get_xMin(), lastRect.get_yMin() + 2f, 27f, lastRect.get_height() - 2f);
								GUI.Label(lastRect, entry.get_GameObjectIcon());
							}
						}
						else
						{
							GUILayout.Label(entry.get_Text2() + " (Destroyed)", SkillEditorStyles.LogLine2, new GUILayoutOption[0]);
						}
					}
				}
			}
			GUILayout.EndVertical();
			if (entry == this.selectedEntry)
			{
				this.beforeSelected = this.prevEntry;
				GUI.set_backgroundColor(Color.get_white());
				GUILayout.Box(GUIContent.none, SkillEditorStyles.LogLineTimeline, new GUILayoutOption[0]);
			}
			if (this.prevEntry == this.selectedEntry)
			{
				this.afterSelected = entry;
			}
			this.prevEntry = entry;
			this.numEntriesDrawn++;
		}
		private void DoBottomToolbar()
		{
			GUI.set_enabled(true);
			GUI.set_backgroundColor(Color.get_white());
			GUI.set_color(Color.get_white());
			GUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[0]);
			if (GUILayout.Button(Strings.get_FsmLog_Clear(), EditorStyles.get_toolbarButton(), new GUILayoutOption[0]))
			{
				this.ClearLog();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		private void HandleKeyboardInput()
		{
			this.prevEntry = null;
			int controlID = GUIUtility.GetControlID(1);
			if (Event.get_current().GetTypeForControl(controlID) == 4)
			{
				KeyCode keyCode = Event.get_current().get_keyCode();
				if (keyCode != 13)
				{
					switch (keyCode)
					{
					case 272:
						return;
					case 273:
						Event.get_current().Use();
						this.SelectPreviousLogEntry();
						GUIUtility.ExitGUI();
						return;
					case 274:
						Event.get_current().Use();
						this.SelectNextLogEntry();
						GUIUtility.ExitGUI();
						return;
					}
				}
				return;
			}
		}
		private void SelectPreviousLogEntry()
		{
			if (this.selectedEntry == null || this.beforeSelected == null)
			{
				return;
			}
			this.SelectLogEntry(this.beforeSelected);
		}
		private void SelectNextLogEntry()
		{
			if (this.selectedEntry == null || this.afterSelected == null)
			{
				return;
			}
			this.SelectLogEntry(this.afterSelected);
		}
		private void DoMainToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[0]);
			string fullFsmLabel = Labels.GetFullFsmLabel(SkillEditor.SelectedFsm);
			if (GUILayout.Button(fullFsmLabel, EditorStyles.get_toolbarDropDown(), new GUILayoutOption[0]))
			{
				SkillEditorGUILayout.GenerateFsmSelectionMenu(false, false).ShowAsContext();
			}
			if (SkillEditorGUILayout.ToolbarSettingsButton())
			{
				SkillLogger.GenerateSettingsMenu().ShowAsContext();
			}
			GUILayout.Space(-5f);
			GUILayout.EndHorizontal();
		}
		private static GenericMenu GenerateSettingsMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_FsmLog_Show_TimeCode()), FsmEditorSettings.LogShowTimecode, new GenericMenu.MenuFunction(SkillLogger.ToggleShowTimecode));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_FsmLog_Show_Sent_By()), FsmEditorSettings.LogShowSentBy, new GenericMenu.MenuFunction(SkillLogger.ToggleShowSentBy));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_FsmLog_Show_State_Exit()), FsmEditorSettings.LogShowExit, new GenericMenu.MenuFunction(SkillLogger.ToggleShowExit));
			return genericMenu;
		}
		private static void ToggleShowTimecode()
		{
			FsmEditorSettings.LogShowTimecode = !FsmEditorSettings.LogShowTimecode;
			FsmEditorSettings.SaveSettings();
		}
		private static void ToggleShowSentBy()
		{
			FsmEditorSettings.LogShowSentBy = !FsmEditorSettings.LogShowSentBy;
			FsmEditorSettings.SaveSettings();
			SkillLogger.instance.ResetLayout();
		}
		private static void ToggleShowExit()
		{
			FsmEditorSettings.LogShowExit = !FsmEditorSettings.LogShowExit;
			FsmEditorSettings.SaveSettings();
			SkillLogger.instance.ResetLayout();
		}
		private static void OnClickSentBy(SkillLogEntry entry)
		{
			if (entry.get_SentByState() != null)
			{
				SkillEditor.SelectFsm(entry.get_SentByState().get_Fsm());
				SkillEditor.SelectState(entry.get_SentByState(), true);
				SkillEditor.SelectAction(entry.get_Action(), true);
			}
		}
		[Localizable(false)]
		private static string GetEventTargetLabel(SkillLogEntry entry)
		{
			SkillEventTarget eventTarget = entry.get_EventTarget();
			switch (eventTarget.target)
			{
			case 0:
				return null;
			case 1:
			{
				GameObject ownerDefaultTarget = SkillEditor.SelectedFsm.GetOwnerDefaultTarget(eventTarget.gameObject);
				if (!(ownerDefaultTarget != null))
				{
					return " GameObject: None";
				}
				return " GameObject: " + ownerDefaultTarget.get_name();
			}
			case 2:
			{
				GameObject ownerDefaultTarget = SkillEditor.SelectedFsm.GetOwnerDefaultTarget(eventTarget.gameObject);
				if (!(ownerDefaultTarget != null))
				{
					return " GameObjectFSM: None";
				}
				return string.Concat(new object[]
				{
					" GameObjectFSM: ",
					ownerDefaultTarget.get_name(),
					" ",
					eventTarget.fsmName
				});
			}
			case 3:
				if (!(eventTarget.fsmComponent != null))
				{
					return " FsmComponent: None";
				}
				return " FsmComponent: " + Labels.GetFullFsmLabel(eventTarget.fsmComponent.get_Fsm());
			case 4:
				return " BroadcastAll";
			case 5:
				return " Host: " + entry.get_State().get_Fsm().get_Host().get_Name();
			case 6:
				return " SubFSMs";
			default:
				return null;
			}
		}
		private void OnClickEventTarget(SkillLogEntry entry)
		{
			switch (entry.get_EventTarget().target)
			{
			case 0:
			case 6:
				break;
			case 1:
				if (entry.get_Event() != null)
				{
					GenericMenu genericMenu = new GenericMenu();
					List<Skill> fsmList = SkillInfo.GetFsmList(SkillInfo.FindTransitionsUsingEvent(entry.get_Event().get_Name()));
					using (List<Skill>.Enumerator enumerator = fsmList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Skill current = enumerator.get_Current();
							genericMenu.AddItem(new GUIContent(Labels.GetFullFsmLabel(current)), false, new GenericMenu.MenuFunction2(SkillEditor.SelectFsm), current);
						}
					}
					genericMenu.ShowAsContext();
					return;
				}
				break;
			case 2:
			{
				GameObject ownerDefaultTarget = SkillEditor.SelectedFsm.GetOwnerDefaultTarget(entry.get_EventTarget().gameObject);
				Skill fsm = SkillSelection.FindFsmOnGameObject(ownerDefaultTarget, entry.get_EventTarget().fsmName.get_Value());
				if (fsm != null)
				{
					SkillEditor.SelectFsm(fsm);
					return;
				}
				break;
			}
			case 3:
				if (entry.get_EventTarget().fsmComponent != null)
				{
					SkillEditor.SelectFsm(entry.get_EventTarget().fsmComponent.get_Fsm());
					return;
				}
				break;
			case 4:
				if (entry.get_Event() != null)
				{
					GenericMenu genericMenu2 = new GenericMenu();
					List<Skill> fsmList2 = SkillInfo.GetFsmList(SkillInfo.FindTransitionsUsingEvent(entry.get_Event().get_Name()));
					using (List<Skill>.Enumerator enumerator2 = fsmList2.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Skill current2 = enumerator2.get_Current();
							genericMenu2.AddItem(new GUIContent(Labels.GetFullFsmLabel(current2)), false, new GenericMenu.MenuFunction2(SkillEditor.SelectFsm), current2);
						}
					}
					genericMenu2.ShowAsContext();
					return;
				}
				break;
			case 5:
				SkillEditor.SelectFsm(entry.get_State().get_Fsm().get_Host());
				break;
			default:
				return;
			}
		}
		private void SelectLogEntry(SkillLogEntry entry)
		{
			if (entry == null)
			{
				return;
			}
			if (FsmEditorSettings.LogPauseOnSelect && !EditorApplication.get_isPaused())
			{
				EditorApplication.set_isPaused(true);
			}
			DebugFlow.SelectLogEntry(entry, true);
			this.autoScroll = true;
		}
		private void ClearLog()
		{
			if (SkillLogger.selectedLog != null)
			{
				SkillLogger.selectedLog.Clear();
			}
		}
		private float CalculateEntryHeight(SkillLogEntry entry)
		{
			if (!this.EntryIsVisible(entry))
			{
				return 0f;
			}
			float num = 20f;
			if (entry.get_LogType() == 6)
			{
				num += 2f;
			}
			return num + 20f * (float)this.ExtraRows(entry);
		}
		private bool EntryIsVisible(SkillLogEntry entry)
		{
			return !string.IsNullOrEmpty(entry.get_Text()) && (entry.get_LogType() != 5 || FsmEditorSettings.LogShowExit);
		}
		private bool ShowSentBy(SkillLogEntry entry)
		{
			return entry.get_LogType() == 3 && FsmEditorSettings.LogShowSentBy && entry.get_SentByState() != null && entry.get_SentByState() != entry.get_State();
		}
		private bool ShowEventTarget(SkillLogEntry entry)
		{
			return entry.get_LogType() == 8 && entry.get_EventTarget().target != null;
		}
		private bool ShowHitGameObject(SkillLogEntry entry)
		{
			return entry.get_GameObject() != null || !string.IsNullOrEmpty(entry.get_GameObjectName());
		}
		private int ExtraRows(SkillLogEntry entry)
		{
			if (this.ShowSentBy(entry) || this.ShowEventTarget(entry) || this.ShowHitGameObject(entry))
			{
				return 1;
			}
			return 0;
		}
		private void ResetLayout()
		{
			this.entryHeights.Clear();
			this.UpdateLayout();
		}
		private void UpdateLayout()
		{
			bool flag = this.ScrollViewIsAtBottom();
			float num = this.GetEntryPosition(this.entryHeights.get_Count());
			float num2 = num;
			while (this.entryHeights.get_Count() < SkillLogger.selectedLog.get_Entries().get_Count())
			{
				SkillLogEntry entry = SkillLogger.selectedLog.get_Entries().get_Item(this.entryHeights.get_Count());
				num += this.CalculateEntryHeight(entry);
				this.entryHeights.Add(num);
				this.updateVisibility = true;
			}
			if (flag)
			{
				this.scrollPosition.y = this.scrollPosition.y + (this.GetEntryPosition(this.entryHeights.get_Count()) - num2);
			}
		}
		private float GetEntryPosition(int entryIndex)
		{
			if (entryIndex <= 0)
			{
				return 0f;
			}
			return this.entryHeights.get_Item(entryIndex - 1);
		}
		private bool ScrollViewIsAtBottom()
		{
			return Math.Abs(this.scrollPosition.y + this.scrollViewHeight - this.GetEntryPosition(this.entryHeights.get_Count())) < 40f;
		}
		private void UpdateVisibility()
		{
			if (this.entryHeights.get_Count() == 0)
			{
				return;
			}
			this.firstVisibleEntry = 0;
			while (this.firstVisibleEntry < this.entryHeights.get_Count() && this.entryHeights.get_Item(this.firstVisibleEntry) < this.scrollPosition.y)
			{
				this.firstVisibleEntry++;
			}
			this.lastVisibleEntry = this.firstVisibleEntry;
			while (this.lastVisibleEntry < this.entryHeights.get_Count() && this.entryHeights.get_Item(this.lastVisibleEntry) < this.scrollPosition.y + base.get_position().get_height())
			{
				this.lastVisibleEntry++;
			}
			this.updateVisibility = false;
		}
	}
}
