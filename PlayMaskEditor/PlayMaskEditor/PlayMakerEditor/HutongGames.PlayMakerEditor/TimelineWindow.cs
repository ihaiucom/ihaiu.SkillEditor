using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class TimelineWindow : BaseEditorWindow
	{
		public enum FsmFilter
		{
			All,
			OnSelectedObject,
			RecentlySelected,
			WithErrors,
			InScene
		}
		private const float minListWidth = 100f;
		private const float minLogWidth = 200f;
		private const float listItemHeight = 20f;
		private static readonly string[] fsmFilterLabels = new string[]
		{
			Strings.get_FilterMenu_All_FSMs(),
			Strings.get_FilterMenu_On_Selected_Objects(),
			Strings.get_FilterMenu_Recently_Selected(),
			Strings.get_FilterMenu_FSMs_With_Errors(),
			Strings.get_FilterMenu_FSMs_in_Scene()
		};
		private static TimelineWindow instance;
		private Vector2 scrollPosition;
		private TimelineWindow.FsmFilter filterMode = TimelineWindow.FsmFilter.InScene;
		private readonly List<Skill> fsmList = new List<Skill>();
		private float listWidth = 100f;
		private float toolbarHeight;
		private float scrollbarHeight;
		private int firstVisibleItem;
		private int lastVisibleItem;
		private int selectedIndex;
		private bool autoScroll;
		private Rect filterRect = default(Rect);
		private Rect timelineRect = default(Rect);
		private Rect splitterRect = default(Rect);
		private bool draggingSplitter;
		private Rect listArea = default(Rect);
		private Rect listItemArea;
		private Rect graphArea = default(Rect);
		private Rect canvasArea = default(Rect);
		private Rect stateBox;
		private TimelineControl timelineControl;
		private LineDrawer line;
		private float lastDebugTime;
		public static TimelineWindow Instance
		{
			get
			{
				return TimelineWindow.instance;
			}
		}
		public List<Skill> FsmList
		{
			get
			{
				if (this.fsmList == null)
				{
					this.RefreshList();
				}
				return this.fsmList;
			}
		}
		private float timeScale
		{
			get
			{
				return this.timelineControl.TimeScale;
			}
		}
		protected override void OnEnable()
		{
			base.OnEnable();
			this.timelineControl = new TimelineControl();
			this.line = new LineDrawer();
			TimelineControl expr_22 = this.timelineControl;
			expr_22.TimelineClicked = (TimelineControl.TimelineClickedHandler)Delegate.Combine(expr_22.TimelineClicked, new TimelineControl.TimelineClickedHandler(this.TimelineClicked));
			DebugFlow.LogEntrySelected = (DebugFlow.LogEntrySelectedHandler)Delegate.Combine(DebugFlow.LogEntrySelected, new DebugFlow.LogEntrySelectedHandler(this.LogEntrySelected));
			this.fsmList.Clear();
			base.Repaint();
		}
		public override void Initialize()
		{
			this.isToolWindow = true;
			TimelineWindow.instance = this;
			this.InitWindowTitle();
			base.set_minSize(new Vector2(300f, 100f));
			this.toolbarHeight = EditorStyles.get_toolbar().get_fixedHeight();
			this.scrollbarHeight = GUI.get_skin().get_horizontalScrollbar().get_fixedHeight();
			this.RefreshList();
			this.LoadSettings();
		}
		public void OnHierarchyChange()
		{
			this.RefreshList();
		}
		public void OnFocus()
		{
			this.RefreshList();
		}
		public void OnDisable()
		{
			TimelineControl expr_06 = this.timelineControl;
			expr_06.TimelineClicked = (TimelineControl.TimelineClickedHandler)Delegate.Remove(expr_06.TimelineClicked, new TimelineControl.TimelineClickedHandler(this.TimelineClicked));
			DebugFlow.LogEntrySelected = (DebugFlow.LogEntrySelectedHandler)Delegate.Remove(DebugFlow.LogEntrySelected, new DebugFlow.LogEntrySelectedHandler(this.LogEntrySelected));
			this.fsmList.Clear();
			this.SaveSettings();
		}
		public void InitWindowTitle()
		{
			base.SetTitle(Strings.get_FsmTimeline_Title());
		}
		public override void DoGUI()
		{
			if (!FsmEditorSettings.EnableLogging)
			{
				TimelineWindow.DoDisabledGUI();
				return;
			}
			this.HandleKeyboardInput();
			this.DoSplitter();
			this.DoMainToolbar();
			this.DoListView();
			this.DoLogView();
			this.DrawDebugLine();
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
		private void DoSplitter()
		{
			if (this.draggingSplitter)
			{
				this.splitterRect.Set(0f, 0f, base.get_position().get_width(), base.get_position().get_height());
			}
			else
			{
				this.splitterRect.Set(this.listWidth - 4f, this.toolbarHeight, 8f, base.get_position().get_height());
			}
			EditorGUIUtility.AddCursorRect(this.splitterRect, 3);
			if (this.splitterRect.Contains(this.currentEvent.get_mousePosition()) && this.eventType == null)
			{
				this.draggingSplitter = true;
				this.currentEvent.Use();
			}
			if (this.draggingSplitter)
			{
				if (this.eventType == 3)
				{
					this.listWidth += this.currentEvent.get_delta().x;
					this.listWidth = Mathf.Clamp(this.listWidth, 100f, base.get_position().get_width() - 200f);
					this.currentEvent.Use();
				}
				if (this.currentEvent.get_rawType() == 1)
				{
					this.draggingSplitter = false;
				}
				base.Repaint();
			}
		}
		private void DoMainToolbar()
		{
			this.filterRect.Set(0f, 0f, this.listWidth, this.toolbarHeight);
			if (GUI.Button(this.filterRect, TimelineWindow.fsmFilterLabels[(int)this.filterMode], EditorStyles.get_toolbarDropDown()))
			{
				this.GenerateFilterMenu().ShowAsContext();
			}
			this.timelineRect.Set(this.listWidth, 0f, base.get_position().get_width() - this.listWidth, base.get_position().get_height());
			this.timelineControl.OnGUI(this.timelineRect);
			if (this.timelineControl.NeedsRepaint)
			{
				base.Repaint();
			}
		}
		private void DoListView()
		{
			this.listArea.Set(0f, this.toolbarHeight, this.listWidth, base.get_position().get_height() - this.toolbarHeight - this.scrollbarHeight);
			this.listItemArea.Set(0f, 0f, this.listWidth, 20f);
			if (this.autoScroll)
			{
				float num = (float)this.selectedIndex * 20f;
				if (num < this.scrollPosition.y)
				{
					this.scrollPosition.y = num;
				}
				else
				{
					if (num > this.scrollPosition.y + this.listArea.get_height() - 20f)
					{
						this.scrollPosition.y = (float)(this.selectedIndex + 1) * 20f - this.listArea.get_height();
					}
				}
				this.autoScroll = false;
			}
			GUI.BeginGroup(this.listArea);
			if (this.FsmList.get_Count() == 0)
			{
				GUILayout.Label(Strings.get_Label_None(), new GUILayoutOption[0]);
			}
			this.firstVisibleItem = Mathf.FloorToInt(this.scrollPosition.y / 20f);
			this.firstVisibleItem = Mathf.Clamp(this.firstVisibleItem, 0, this.FsmList.get_Count());
			this.lastVisibleItem = this.firstVisibleItem + Mathf.CeilToInt(this.listArea.get_height() / 20f) + 1;
			this.lastVisibleItem = Mathf.Clamp(this.lastVisibleItem, 0, this.FsmList.get_Count());
			this.listItemArea.set_y((float)this.firstVisibleItem * 20f - this.scrollPosition.y);
			for (int i = this.firstVisibleItem; i < this.lastVisibleItem; i++)
			{
				Skill fsm = this.FsmList.get_Item(i);
				bool flag = SkillEditor.SelectedFsm == fsm;
				if (flag)
				{
					if (this.eventType == 7)
					{
						GUI.DrawTexture(this.listItemArea, SkillEditorStyles.SelectedBG);
					}
					if (this.selectedIndex != i)
					{
						this.autoScroll = true;
					}
					this.selectedIndex = i;
				}
				if (GUI.Button(this.listItemArea, Labels.GetRuntimeFsmLabelToFit(fsm, this.listWidth, SkillEditorStyles.TableRowText), flag ? SkillEditorStyles.TableRowTextSelected : SkillEditorStyles.TableRowText))
				{
					this.SelectFsm(fsm);
				}
				this.listItemArea.set_y(this.listItemArea.get_y() + 20f);
				this.line.SetColor(SkillEditorStyles.LabelTextColor);
				this.line.DrawLine(0.05f, 0f, this.listItemArea.get_y() - 1f, this.listWidth, this.listItemArea.get_y() - 1f);
			}
			GUI.EndGroup();
			if (this.listArea.Contains(this.currentEvent.get_mousePosition()) && this.eventType == 6)
			{
				this.scrollPosition.y = this.scrollPosition.y + this.currentEvent.get_delta().y;
				base.Repaint();
			}
			this.listArea.Set(0f, base.get_position().get_height() - this.scrollbarHeight, this.listWidth, this.scrollbarHeight);
			GUI.Box(this.listArea, GUIContent.none, EditorStyles.get_toolbar());
			if (GUI.Button(this.listArea, Strings.get_Label_Refresh(), EditorStyles.get_toolbarButton()))
			{
				this.RefreshList();
			}
		}
		private void SelectPrevious()
		{
			if (SkillEditor.SelectedFsm == null)
			{
				return;
			}
			if (this.selectedIndex > 0)
			{
				this.SelectFsm(this.FsmList.get_Item(--this.selectedIndex));
			}
			this.autoScroll = true;
		}
		private void SelectNext()
		{
			if (SkillEditor.SelectedFsm == null)
			{
				return;
			}
			if (this.selectedIndex < this.FsmList.get_Count() - 1)
			{
				this.SelectFsm(this.FsmList.get_Item(++this.selectedIndex));
			}
			this.autoScroll = true;
		}
		private void SelectFsm(Skill fsm)
		{
			if (fsm != SkillEditor.SelectedFsm)
			{
				SkillEditor.SelectFsm(fsm);
				if (EditorApplication.get_isPaused())
				{
					DebugFlow.Refresh();
				}
			}
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
		private void LogEntrySelected(SkillLogEntry logEntry)
		{
			this.timelineControl.FrameTime(logEntry.get_Time(), 0f);
		}
		private void DoLogView()
		{
			this.graphArea.Set(this.listWidth, this.toolbarHeight, base.get_position().get_width() - this.listWidth, base.get_position().get_height() - this.toolbarHeight);
			this.canvasArea.Set(0f, 0f, this.timelineControl.CanvasWidth, Mathf.Max((float)this.FsmList.get_Count() * 20f, this.graphArea.get_height() - this.scrollbarHeight));
			this.scrollPosition.x = this.timelineControl.CanvasOffset;
			this.scrollPosition = GUI.BeginScrollView(this.graphArea, this.scrollPosition, this.canvasArea, true, true);
			this.timelineControl.CanvasOffset = this.scrollPosition.x;
			this.listItemArea.Set(0f, (float)this.firstVisibleItem * 20f, this.canvasArea.get_width(), 20f);
			for (int i = this.firstVisibleItem; i < this.lastVisibleItem; i++)
			{
				this.DoTimelineBar(this.FsmList.get_Item(i), this.listItemArea);
				this.listItemArea.set_y(this.listItemArea.get_y() + 20f);
			}
			GUI.EndScrollView();
		}
		private void DoTimelineBar(Skill fsm, Rect area)
		{
			if (!Application.get_isPlaying())
			{
				return;
			}
			if (fsm == null)
			{
				return;
			}
			SkillLog myLog = fsm.get_MyLog();
			if (myLog == null || myLog.get_Entries() == null)
			{
				return;
			}
			GUI.BeginGroup(area);
			float startTime = 0f;
			SkillState fsmState = null;
			for (int i = 0; i < myLog.get_Entries().get_Count(); i++)
			{
				SkillLogEntry fsmLogEntry = myLog.get_Entries().get_Item(i);
				if (fsmLogEntry.get_LogType() == 5)
				{
					if (fsmLogEntry.get_Time() > this.timelineControl.VisibleRangeStart)
					{
						this.DrawTimelineBar(startTime, fsmLogEntry.get_Time(), fsmState);
					}
					fsmState = null;
				}
				if (fsmLogEntry.get_LogType() == 6)
				{
					if (fsmLogEntry.get_Time() > this.timelineControl.VisibleRangeEnd)
					{
						GUI.EndGroup();
						return;
					}
					fsmState = fsmLogEntry.get_State();
					startTime = fsmLogEntry.get_Time();
				}
				SkillLogType arg_AE_0 = fsmLogEntry.get_LogType();
				SkillLogType arg_B8_0 = fsmLogEntry.get_LogType();
			}
			if (fsmState != null)
			{
				this.DrawTimelineBar(startTime, SkillTime.get_RealtimeSinceStartup(), fsmState);
			}
			GUI.EndGroup();
		}
		private void DrawTimelineBar(float startTime, float endTime, SkillState state)
		{
			if (state == null)
			{
				return;
			}
			this.stateBox.Set(startTime * this.timeScale, 0f, (endTime - startTime) * this.timeScale, 20f);
			GUI.set_backgroundColor(PlayMakerPrefs.get_Colors()[state.get_ColorIndex()]);
			GUI.Box(this.stateBox, new GUIContent("", state.get_Name()), SkillEditorStyles.TimelineBar);
			GUI.set_backgroundColor(Color.get_white());
			GUIStyle gUIStyle = SkillEditorStyles.TimelineBarText;
			if (startTime < this.timelineControl.VisibleRangeStart)
			{
				this.stateBox.set_x(this.timelineControl.VisibleRangeStart * this.timeScale);
				this.stateBox.set_width((endTime - this.timelineControl.VisibleRangeStart) * this.timeScale);
				gUIStyle = SkillEditorStyles.TimelineLabelLeft;
			}
			if (GUI.Button(this.stateBox, state.get_Name(), gUIStyle))
			{
				this.SelectFsm(state.get_Fsm());
				this.TimelineClicked(startTime);
			}
		}
		private void DrawDebugLine()
		{
			if (Application.get_isPlaying() && this.eventType == 7)
			{
				float time;
				if (EditorApplication.get_isPaused())
				{
					GUI.set_backgroundColor(Color.get_yellow());
					time = DebugFlow.CurrentDebugTime;
				}
				else
				{
					GUI.set_backgroundColor(Color.get_white());
					time = SkillTime.get_RealtimeSinceStartup();
				}
				this.timelineControl.DrawDebugLine(time);
			}
		}
		private void TimelineClicked(float time)
		{
			if (this.currentEvent.get_button() == 0 && !Keyboard.AltAction())
			{
				this.SetDebugTime(time);
			}
		}
		private void SetDebugTime(float time)
		{
			if (FsmEditorSettings.LogPauseOnSelect && !EditorApplication.get_isPaused())
			{
				EditorApplication.set_isPaused(true);
			}
			this.timelineControl.FrameTime(time, 0f);
			DebugFlow.SetDebugTime(time);
			base.Repaint();
		}
		private GenericMenu GenerateFilterMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_FilterMenu_All_FSMs()), false, new GenericMenu.MenuFunction2(this.SetFilterMode), TimelineWindow.FsmFilter.All);
			genericMenu.AddItem(new GUIContent(Strings.get_FilterMenu_FSMs_in_Scene()), false, new GenericMenu.MenuFunction2(this.SetFilterMode), TimelineWindow.FsmFilter.InScene);
			genericMenu.AddItem(new GUIContent(Strings.get_FilterMenu_On_Selected_Objects()), false, new GenericMenu.MenuFunction2(this.SetFilterMode), TimelineWindow.FsmFilter.OnSelectedObject);
			genericMenu.AddItem(new GUIContent(Strings.get_FilterMenu_Recently_Selected()), false, new GenericMenu.MenuFunction2(this.SetFilterMode), TimelineWindow.FsmFilter.RecentlySelected);
			return genericMenu;
		}
		private void SetFilterMode(object userdata)
		{
			this.SetFilterMode((TimelineWindow.FsmFilter)userdata);
		}
		private void SetFilterMode(TimelineWindow.FsmFilter mode)
		{
			this.filterMode = mode;
			this.RefreshList();
		}
		public void OnSelectionChange()
		{
			this.RefreshList();
		}
		private void RefreshList()
		{
			this.fsmList.Clear();
			if (this.filterMode == TimelineWindow.FsmFilter.RecentlySelected)
			{
				this.fsmList.AddRange(SkillEditor.SelectionHistory.GetRecentlySelectedFSMs());
			}
			else
			{
				using (List<Skill>.Enumerator enumerator = SkillEditor.SortedFsmList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Skill current = enumerator.get_Current();
						switch (this.filterMode)
						{
						case TimelineWindow.FsmFilter.All:
							this.fsmList.Add(current);
							break;
						case TimelineWindow.FsmFilter.OnSelectedObject:
							if (current.get_GameObject() != null && Selection.Contains(current.get_GameObject()))
							{
								this.fsmList.Add(current);
							}
							break;
						case TimelineWindow.FsmFilter.WithErrors:
							if (FsmErrorChecker.FsmHasErrors(current))
							{
								this.fsmList.Add(current);
							}
							break;
						case TimelineWindow.FsmFilter.InScene:
							if (!SkillPrefabs.IsPersistent(current))
							{
								this.fsmList.Add(current);
							}
							break;
						}
					}
				}
			}
			base.Repaint();
		}
		protected void Update()
		{
			if (EditorApplication.get_isPaused())
			{
				if (Math.Abs(this.lastDebugTime - DebugFlow.CurrentDebugTime) > 1.401298E-45f)
				{
					this.lastDebugTime = DebugFlow.CurrentDebugTime;
					base.Repaint();
					return;
				}
			}
			else
			{
				if (Application.get_isPlaying())
				{
					if (this.timelineControl != null)
					{
						this.timelineControl.SetLength(SkillTime.get_RealtimeSinceStartup());
					}
					base.Repaint();
				}
			}
		}
		[Localizable(false)]
		private void SaveSettings()
		{
			EditorPrefs.SetFloat("PlayMaker.TimelineWindow.TimeScale", this.timelineControl.TimeScale);
			EditorPrefs.SetFloat("PlayMaker.TimelineWindow.Offset", this.timelineControl.Offset);
			EditorPrefs.SetInt("PlayMaker.TimelineWindow.FilterMode", (int)this.filterMode);
		}
		[Localizable(false)]
		private void LoadSettings()
		{
			this.timelineControl.TimeScale = EditorPrefs.GetFloat("PlayMaker.TimelineWindow.TimeScale", 100f);
			this.timelineControl.Offset = EditorPrefs.GetFloat("PlayMaker.TimelineWindow.Offset", 0f);
			this.filterMode = (TimelineWindow.FsmFilter)EditorPrefs.GetInt("PlayMaker.TimelineWindow.FilterMode", 4);
		}
	}
}
