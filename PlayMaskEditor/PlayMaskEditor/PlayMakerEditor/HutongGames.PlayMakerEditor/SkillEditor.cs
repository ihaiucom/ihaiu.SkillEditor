using HutongGames.Editor;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	[Serializable]
	public class SkillEditor
	{
		public delegate void RepaintDelegate();
		public delegate void UpdateDelegate();
		public delegate void FsmChanged(Skill fsm);
		public const float InspectorPanelWidth = 350f;
		public bool Initialized;
		public static bool IgnoreHierarchyChange;
		private EditorWindow window;
		private static SkillEditor instance;
		[SerializeField]
		private bool initializedOnce;
		private static bool playerStarting;
		private static bool playerStopping;
		private static Skill dirtyFsmPrefab;
		private static List<PlayMakerFSM> fsmComponentList;
		private static List<Skill> fsmList;
		private static SkillTemplate selectTemplateDelayed;
		private static Skill selectFsmDelayed;
		private static string loadedScenesHash;
		private static string sendWindowCommandEvent;
		[SerializeField]
		private SkillSelection fsmSelection;
		[SerializeField]
		private SkillSelectionHistory selectionHistory;
		private readonly SkillBuilder builder;
		private readonly MainToolbar toolbar;
		private readonly FsmGraphView graphView;
		private readonly InspectorPanel inspector;
		private readonly StateInspector stateInspector;
		private readonly ActionEditor actionEditor;
		private readonly FsmEventManager eventManager;
		private readonly VariableManager variableManager;
		[SerializeField]
		private FsmDebugger debugger;
		private bool openReport;
		private EditorApplication.CallbackFunction playmodeDelegate;
		private bool repaint;
		private bool repaintAll;
		private bool mouseUp;
		private bool mouseDown;
		private static bool updateIsModifiedPrefabInstance;
		private static bool updateActionUsage;
		private static bool updateFsmInfo;
		private static bool warnedAboutEditingWhileRunning;
		public static SkillEditor.RepaintDelegate OnRepaint;
		public static SkillEditor.UpdateDelegate OnUpdate;
		private Object lastSelectedObject;
		public static SkillEditor.FsmChanged OnFsmChanged;
		public static SkillEditor Instance
		{
			get
			{
				return SkillEditor.instance;
			}
		}
		public static List<PlayMakerFSM> FsmComponentList
		{
			get
			{
				if (SkillEditor.fsmComponentList == null)
				{
					SkillEditor.RebuildFsmList();
				}
				return SkillEditor.fsmComponentList;
			}
		}
		public static List<Skill> FsmList
		{
			get
			{
				if (SkillEditor.fsmList == null)
				{
					SkillEditor.RebuildFsmList();
				}
				return SkillEditor.fsmList;
			}
		}
		public static List<Skill> SortedFsmList
		{
			get
			{
				List<Skill> list = new List<Skill>(SkillEditor.FsmList);
				list.Sort();
				return list;
			}
		}
		public static bool NeedRepaint
		{
			get
			{
				return SkillEditor.instance.repaint || SkillEditor.instance.repaintAll;
			}
		}
		public static bool MouseOverInspector
		{
			get;
			private set;
		}
		public static bool InspectorHasFocus
		{
			get;
			set;
		}
		public static SkillSelectionHistory SelectionHistory
		{
			get
			{
				if (SkillEditor.instance == null)
				{
					return null;
				}
				if (SkillEditor.instance.selectionHistory == null)
				{
					SkillEditor.instance.selectionHistory = new SkillSelectionHistory();
				}
				return SkillEditor.instance.selectionHistory;
			}
		}
		public static Object SelectedFsmOwner
		{
			get
			{
				if (SkillEditor.instance == null)
				{
					return null;
				}
				if (SkillEditor.instance.fsmSelection.ActiveFsmComponent != null)
				{
					return SkillEditor.instance.fsmSelection.ActiveFsmComponent;
				}
				if (SkillEditor.instance.fsmSelection.ActiveFsmTemplate != null)
				{
					return SkillEditor.instance.fsmSelection.ActiveFsmTemplate;
				}
				return null;
			}
		}
		public static PlayMakerFSM SelectedFsmComponent
		{
			get
			{
				if (SkillEditor.instance != null)
				{
					return SkillEditor.instance.fsmSelection.ActiveFsmComponent;
				}
				return null;
			}
		}
		public static bool SelectedFsmUsesTemplate
		{
			get
			{
				return SkillEditor.SelectedFsmComponent != null && SkillEditor.SelectedFsmComponent.get_UsesTemplate();
			}
		}
		public static Skill SelectedFsm
		{
			get
			{
				if (SkillEditor.instance != null)
				{
					return SkillEditor.instance.fsmSelection.ActiveFsm;
				}
				return null;
			}
		}
		public static bool SelectedFsmIsLocked
		{
			get
			{
				return SkillEditor.SelectedFsm != null && SkillEditor.SelectedFsm.get_Locked();
			}
		}
		public static bool SelectedFsmDebugFlowEnabled
		{
			get
			{
				return SkillEditor.SelectedFsm != null && SkillEditor.SelectedFsm.EnableDebugFlow;
			}
		}
		public static SkillTemplate SelectedTemplate
		{
			get
			{
				if (SkillEditor.instance != null)
				{
					return SkillEditor.Selection.ActiveFsmTemplate;
				}
				return null;
			}
		}
		public static SkillState SelectedState
		{
			get
			{
				if (SkillEditor.instance != null)
				{
					return SkillEditor.instance.fsmSelection.ActiveState;
				}
				return null;
			}
		}
		public static string SelectedStateName
		{
			get
			{
				if (SkillEditor.SelectedState != null)
				{
					return SkillEditor.SelectedState.get_Name();
				}
				return "";
			}
		}
		public static List<SkillState> SelectedStates
		{
			get
			{
				if (SkillEditor.instance != null)
				{
					return SkillEditor.Selection.States;
				}
				return null;
			}
		}
		public static SkillTransition SelectedTransition
		{
			get
			{
				if (SkillEditor.instance != null)
				{
					return SkillEditor.Selection.ActiveTransition;
				}
				return null;
			}
		}
		public static List<SkillStateAction> SelectedActions
		{
			get
			{
				if (SkillEditor.instance != null)
				{
					return StateInspector.SelectedActions;
				}
				return null;
			}
		}
		public static GameObject SelectedFsmGameObject
		{
			get
			{
				if (SkillEditor.instance != null)
				{
					return SkillEditor.Selection.ActiveFsmGameObject;
				}
				return null;
			}
		}
		public static int SelectedFsmInstanceID
		{
			get
			{
				if (!(SkillEditor.SelectedFsmGameObject != null))
				{
					return -1;
				}
				return SkillEditor.SelectedFsmGameObject.GetInstanceID();
			}
		}
		public static EditorWindow Window
		{
			get
			{
				if (SkillEditor.instance != null)
				{
					return SkillEditor.instance.window;
				}
				return null;
			}
		}
		public static SkillBuilder Builder
		{
			get
			{
				if (SkillEditor.instance != null)
				{
					return SkillEditor.instance.builder;
				}
				return null;
			}
		}
		public static bool MouseDown
		{
			get
			{
				return SkillEditor.instance.mouseDown;
			}
		}
		public static bool MouseUp
		{
			get
			{
				return SkillEditor.instance.mouseUp;
			}
		}
		internal static SkillSelection Selection
		{
			get
			{
				if (SkillEditor.instance == null)
				{
					return null;
				}
				return SkillEditor.instance.fsmSelection ?? new SkillSelection(null);
			}
		}
		internal static FsmGraphView GraphView
		{
			get
			{
				return SkillEditor.instance.graphView;
			}
		}
		internal static InspectorPanel Inspector
		{
			get
			{
				return SkillEditor.instance.inspector;
			}
		}
		internal static StateInspector StateInspector
		{
			get
			{
				return SkillEditor.instance.stateInspector;
			}
		}
		public static ActionEditor ActionEditor
		{
			get
			{
				return SkillEditor.instance.actionEditor;
			}
		}
		internal static FsmEventManager EventManager
		{
			get
			{
				return SkillEditor.instance.eventManager;
			}
		}
		internal static VariableManager VariableManager
		{
			get
			{
				return SkillEditor.instance.variableManager;
			}
		}
		internal static FsmDebugger Debugger
		{
			get
			{
				return SkillEditor.instance.debugger;
			}
		}
		public SkillEditor()
		{
			SkillEditor.instance = this;
			this.builder = new SkillBuilder();
			this.actionEditor = new ActionEditor();
			this.stateInspector = new StateInspector();
			this.eventManager = new FsmEventManager();
			this.variableManager = new VariableManager();
			this.graphView = new FsmGraphView();
			this.inspector = new InspectorPanel();
			this.toolbar = new MainToolbar();
			this.selectionHistory = new SkillSelectionHistory();
			this.fsmSelection = SkillSelection.None;
		}
		public void InitWindow(EditorWindow fsmEditorWindow)
		{
			this.window = fsmEditorWindow;
			this.window.set_titleContent(new GUIContent(FsmEditorSettings.ProductName));
			this.window.set_minSize(new Vector2(800f, 200f));
			this.window.set_wantsMouseMove(true);
			this.Initialized = true;
		}
		public void OnEnable()
		{
			this.debugger = FsmDebugger.Instance;
			SkillEditor.OnUpdate = (SkillEditor.UpdateDelegate)Delegate.Remove(SkillEditor.OnUpdate, new SkillEditor.UpdateDelegate(this.FirstUpdate));
			SkillEditor.OnUpdate = (SkillEditor.UpdateDelegate)Delegate.Combine(SkillEditor.OnUpdate, new SkillEditor.UpdateDelegate(this.FirstUpdate));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(SkillEditor.UndoRedoPerformed));
			SkillEditor.RebuildFsmList();
			if (!SkillEditor.FsmList.Contains(SkillEditor.SelectedFsm))
			{
				SkillEditor.SelectNone();
			}
			SkillSearch.Invalidate();
			if (!this.initializedOnce)
			{
				SkillEvent.get_EventList().Sort();
				this.initializedOnce = true;
			}
			SkillEditor.GraphView.ApplySettings();
			GameStateTracker.Update();
			CustomActionEditors.ClearCache();
			if (!SkillEditor.playerStarting)
			{
				this.OnSelectionChange();
				SkillEditor.SanityCheck();
				FsmErrorChecker.ClearErrors(true);
				FsmErrorChecker.CheckForErrors();
			}
			this.playmodeDelegate = new EditorApplication.CallbackFunction(this.PlaymodeChanged);
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.playmodeStateChanged, this.playmodeDelegate);
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUICallback));
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUICallback));
			SceneView.RepaintAll();
			SkillEditor.loadedScenesHash = this.GetLoadedScenesHash();
			SkillEditor.Inspector.SetMode((InspectorMode)EditorPrefs.GetInt(EditorPrefStrings.get_InspectorMode(), 0));
			this.graphView.Init();
			this.ResetViews();
			SkillEditor.RepaintAll();
		}
		public void OnGUI()
		{
			if (Event.get_current().get_type() == 7 && SkillEditor.OnRepaint != null)
			{
				SkillEditor.OnRepaint();
			}
			SkillEditor.Builder.SetTarget(SkillEditor.SelectedFsm);
			if (SkillEditor.SelectedState != null)
			{
				SkillEditor.SelectedState.set_Fsm(SkillEditor.SelectedFsm);
			}
			this.graphView.EnableEditing();
			if (SkillEditorGUILayout.DoEditorDisabledGUI())
			{
				return;
			}
			Color color = GUI.get_color();
			GUI.set_color(Color.get_white());
			EditorGUI.BeginDisabledGroup(EditorApplication.get_isCompiling());
			if (this.repaint)
			{
				this.DoRepaint();
			}
			SkillEditorStyles.Init();
			if (SkillEditor.Selection.IsGameObjectNull)
			{
				SkillEditor.SelectNone();
			}
			this.HandleMouseInput();
			this.toolbar.OnGUI();
			this.inspector.OnGUI(new Rect(this.window.get_position().get_width() - 350f, 0f, 350f, this.window.get_position().get_height()));
			this.graphView.OnGUI(new Rect(0f, EditorStyles.get_toolbar().get_fixedHeight(), this.window.get_position().get_width() - 350f, this.window.get_position().get_height() - EditorStyles.get_toolbar().get_fixedHeight() * 2f));
			DebugToolbar.OnGUI(this.window.get_position().get_width() - 350f);
			SkillEditor.HandleKeyboardInput();
			GUI.set_color(color);
			EditorGUI.EndDisabledGroup();
		}
		private string GetLoadedScenesHash()
		{
			Scene[] allScenes = SceneManager.GetAllScenes();
			return Enumerable.Aggregate<Scene, string>(allScenes, "", (string current, Scene scene) => current + scene.get_name() + "_");
		}
		private void DoRepaint()
		{
			this.window.Repaint();
			HandleUtility.Repaint();
			SceneView.RepaintAll();
			this.repaint = false;
		}
		public void OnSceneGUICallback(SceneView scnView)
		{
			SkillEditor.OnSceneGUI();
		}
		public static void OnSceneGUI()
		{
			if (SkillEditor.SelectedState == null || ActionEditor.PreviewMode || SkillEditor.SelectedFsm == null || !SkillEditor.SelectedState.get_IsInitialized())
			{
				return;
			}
			SkillStateAction[] actions = SkillEditor.SelectedState.get_Actions();
			for (int i = 0; i < actions.Length; i++)
			{
				SkillStateAction fsmStateAction = actions[i];
				if (CustomActionEditors.HasCustomEditor(fsmStateAction.GetType()) && fsmStateAction.get_Enabled() && fsmStateAction.get_IsOpen())
				{
					fsmStateAction.set_Fsm(SkillEditor.SelectedFsm);
					CustomActionEditor customEditor = CustomActionEditors.GetCustomEditor(fsmStateAction);
					customEditor.OnSceneGUI();
				}
			}
		}
		private void HandleMouseInput()
		{
			EventType type = Event.get_current().get_type();
			if (Event.get_current().get_isMouse() || type == 9 || type == 10)
			{
				SkillEditor.MouseOverInspector = (Event.get_current().get_mousePosition().x > this.window.get_position().get_width() - 350f);
			}
			if (Event.get_current().get_rawType() == 1)
			{
				this.mouseUp = true;
			}
			if (type == null)
			{
				this.mouseDown = true;
				SkillEditor.InspectorHasFocus = SkillEditor.MouseOverInspector;
			}
		}
		public static void MouseUpHandled()
		{
			if (SkillEditor.instance != null)
			{
				SkillEditor.instance.mouseUp = false;
			}
		}
		private static void HandleKeyboardInput()
		{
			Keyboard.Update();
			if (Keyboard.IsGuiEventKeyboardShortcut())
			{
				KeyCode keyCode = Event.get_current().get_keyCode();
				if (keyCode != 8 && keyCode != 127)
				{
					return;
				}
				if (SkillEditor.InspectorHasFocus && SkillEditor.Inspector.Mode == InspectorMode.StateInspector)
				{
					SkillEditor.StateInspector.DeleteSelectedActions(true);
					return;
				}
				EditorCommands.DeleteMultiSelection();
			}
		}
		public void OnInspectorUpdate()
		{
			if (SkillEditor.updateIsModifiedPrefabInstance)
			{
				SkillPrefabs.UpdateIsModifiedPrefabInstance(SkillEditor.SelectedFsm);
				SkillEditor.updateIsModifiedPrefabInstance = false;
				SkillEditor.Repaint(true);
			}
			if (Application.get_isPlaying() && ((FsmEditorSettings.DebugActionParameters && SkillEditor.Inspector.Mode == InspectorMode.StateInspector) || (FsmEditorSettings.DebugVariables && SkillEditor.Inspector.Mode == InspectorMode.VariableManager)))
			{
				SkillEditor.Repaint(true);
			}
			if (SkillEditor.updateActionUsage && FsmEditorSettings.AutoRefreshActionUsage)
			{
				Actions.UpdateUsage();
				SkillEditor.updateActionUsage = false;
			}
			if (SkillEditor.updateFsmInfo)
			{
				SkillSearch.Update(SkillEditor.SelectedFsm);
				SkillEditor.updateFsmInfo = false;
				SkillEditor.RepaintAll();
			}
		}
		public static void PlayerStarting()
		{
			SkillEditor.playerStarting = true;
			EditorPrefs.SetString("PlayMaker.StartTime", DateTime.get_Now().ToString(CultureInfo.get_InvariantCulture()));
		}
		private void PlayerStarted()
		{
			SkillEditor.playerStarting = false;
			if (SkillEditor.instance != null)
			{
				SkillEditor.ReselectFsm();
			}
		}
		[Localizable(false)]
		public static void PlayerStopping()
		{
			SkillEditor.playerStopping = true;
		}
		[Localizable(false)]
		public static void PlayerStopped()
		{
			SkillEditor.playerStopping = false;
			if (SkillEditor.instance != null)
			{
				SkillEditor.ReselectFsm();
			}
		}
		private void FirstUpdate()
		{
			if (!Application.get_isPlaying())
			{
				Templates.LoadAll();
				SkillEditor.RepaintAll();
			}
			SkillEditor.OnUpdate = (SkillEditor.UpdateDelegate)Delegate.Remove(SkillEditor.OnUpdate, new SkillEditor.UpdateDelegate(this.FirstUpdate));
		}
		[Localizable(false)]
		public void Update()
		{
			if (SkillEditor.playerStarting || this.window == null)
			{
				return;
			}
			if (SkillEditor.OnUpdate != null)
			{
				SkillEditor.OnUpdate();
			}
			SkillTime.Update();
			if (SkillEditor.HandleDelayedSelection())
			{
				return;
			}
			if (Application.get_isPlaying() && FsmEditorSettings.DisableEditorWhenPlaying)
			{
				return;
			}
			if (!string.IsNullOrEmpty(SkillEditor.sendWindowCommandEvent))
			{
				this.window.SendEvent(EditorGUIUtility.CommandEvent(SkillEditor.sendWindowCommandEvent));
				SkillEditor.sendWindowCommandEvent = "";
			}
			if (this.openReport)
			{
				this.DoOpenReport();
			}
			if (FsmEditorSettings.SelectFSMInGameView)
			{
				if ((!FsmEditorSettings.LockGraphView || SkillEditor.SelectedFsm == null) && Skill.get_LastClickedObject() != null && SkillEditor.SelectedFsmGameObject != Skill.get_LastClickedObject())
				{
					UnityEditor.Selection.set_activeGameObject(Skill.get_LastClickedObject());
					EditorGUIUtility.PingObject(SkillEditor.SelectedFsmGameObject);
				}
				Skill.set_LastClickedObject(null);
			}
			if (this.repaintAll)
			{
				this.window.SendEvent(EditorGUIUtility.CommandEvent("RepaintAll"));
				this.repaintAll = false;
				this.repaint = false;
			}
			this.graphView.Update();
			this.inspector.Update();
			this.debugger.Update();
			this.mouseUp = false;
			this.mouseDown = false;
			FsmErrorChecker.Update();
			DragAndDropManager.Update();
		}
		private static bool HandleDelayedSelection()
		{
			if (SkillEditor.selectTemplateDelayed)
			{
				SkillEditor.SelectFsm(SkillEditor.selectTemplateDelayed.fsm);
				SkillEditor.selectTemplateDelayed = null;
				return true;
			}
			if (SkillEditor.selectFsmDelayed != null)
			{
				SkillEditor.SelectFsm(SkillEditor.selectFsmDelayed);
				SkillEditor.selectFsmDelayed = null;
				return true;
			}
			return false;
		}
		public void OnSelectionChange()
		{
			if (this.lastSelectedObject == UnityEditor.Selection.get_activeObject())
			{
				return;
			}
			this.lastSelectedObject = UnityEditor.Selection.get_activeObject();
			if (SkillEditor.playerStarting || !this.Initialized || SkillEditor.playerStopping)
			{
				return;
			}
			SkillEditor.Repaint(true);
			if (FsmEditorSettings.LockGraphView && SkillEditor.SelectedFsm != null)
			{
				return;
			}
			Object activeObject = UnityEditor.Selection.get_activeObject();
			GameObject gameObject = activeObject as GameObject;
			if (gameObject != null)
			{
				SkillEditor.SelectFsm(UnityEditor.Selection.get_activeGameObject());
				return;
			}
			SkillTemplate fsmTemplate = activeObject as SkillTemplate;
			if (fsmTemplate != null)
			{
				SkillEditor.SelectFsm(fsmTemplate.fsm);
				return;
			}
			SkillEditor.SelectNone();
		}
		public void OnHierarchyChange()
		{
			if (SkillEditor.IgnoreHierarchyChange)
			{
				SkillEditor.IgnoreHierarchyChange = false;
				return;
			}
			Gizmos.ClearFsmLookupCache();
			if (SkillEditor.playerStarting)
			{
				return;
			}
			if (this.graphView.IsDragging)
			{
				return;
			}
			if (SkillEditor.SelectedFsmComponent != null)
			{
				SkillEditor.SelectFsm(SkillEditor.SelectedFsmComponent.get_Fsm());
			}
			SkillEditor.SanityCheck();
			this.OnSelectionChange();
			if (!Application.get_isPlaying())
			{
				if (FsmEditorSettings.EnableRealtimeErrorChecker)
				{
					FsmErrorChecker.CheckForErrors();
				}
				GlobalEventsWindow.ResetView();
				SkillEditor.UpdateActionUsage();
			}
			GlobalVariablesWindow.ResetView();
			SkillPrefabs.UpdateIsModifiedPrefabInstance(SkillEditor.SelectedFsm);
			if (SkillEditor.loadedScenesHash != this.GetLoadedScenesHash())
			{
				this.OnSceneChange();
			}
		}
		public void OnSceneChange()
		{
			if (FsmEditorSettings.AutoLoadPrefabs)
			{
				SkillPrefabs.LoadUsedPrefabs();
			}
			SkillEditor.loadedScenesHash = this.GetLoadedScenesHash();
			SkillEditor.SanityCheck();
		}
		public void OnFocus()
		{
			SkillEditor.instance = this;
			SkillEditor.DoDirtyFsmPrefab();
			SkillEditor.Repaint(false);
			if (SkillEditor.loadedScenesHash != this.GetLoadedScenesHash())
			{
				this.OnSceneChange();
			}
			SkillEditor.Selection.SanityCheck();
		}
		public void OnProjectChange()
		{
			FsmErrorChecker.ClearErrors(true);
			Actions.BuildList();
			Files.BuildScriptList();
			Templates.InitList();
			Actions.UpdateUsage();
			CustomActionEditors.Rebuild();
			ObjectPropertyDrawers.Rebuild();
			PropertyDrawers.Rebuild();
			SkillEditor.DoDirtyFsmPrefab();
			SkillEditor.SanityCheck();
		}
		public void OnDisable()
		{
			SkillEditor.DoDirtyFsmPrefab();
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.playmodeStateChanged, this.playmodeDelegate);
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(SkillEditor.UndoRedoPerformed));
			SkillEditor.Builder.Cleanup();
			DebugFlow.Cleanup();
			CustomActionEditors.ClearCache();
			SkillEditor.FsmComponentList.Clear();
			SkillEditor.FsmList.Clear();
			SkillEditor.SortedFsmList.Clear();
			SkillEditor.fsmComponentList = null;
			SkillEditor.fsmList = null;
			SkillEditor.instance = null;
			this.Initialized = false;
		}
		public void OnDestroy()
		{
			SkillEditor.SelectNone();
			SkillEditor.FsmComponentList.Clear();
			SkillEditor.FsmList.Clear();
			SkillEditor.SortedFsmList.Clear();
			SkillEditor.fsmComponentList = null;
			SkillEditor.fsmList = null;
			SkillEditor.playerStarting = false;
			SkillEditor.playerStopping = false;
			SkillEditor.instance = null;
			this.Initialized = false;
			this.debugger.OnDestroy();
		}
		private void PlaymodeChanged()
		{
			PlayMakerGlobals.InitApplicationFlags();
			SkillEditor.DoDirtyFsmPrefab();
			GameStateTracker.Update();
			SkillSearch.Invalidate();
			this.lastSelectedObject = null;
			if (EditorApplication.get_isPlayingOrWillChangePlaymode() && GameStateTracker.CurrentState == GameState.Stopped && GameStateTracker.PreviousState == GameState.Stopped)
			{
				SkillEditor.PlayerStarting();
				return;
			}
			if (EditorApplication.get_isPlaying() && !EditorApplication.get_isPlayingOrWillChangePlaymode())
			{
				SkillEditor.PlayerStopping();
				return;
			}
			if (SkillEditor.playerStarting)
			{
				this.PlayerStarted();
			}
			if (SkillEditor.playerStopping)
			{
				SkillEditor.PlayerStopped();
			}
			if (!GameStateTracker.StateChanged)
			{
				DebugFlow.UpdateTime();
				SkillEditor.RepaintAll();
				return;
			}
			SkillEditor.SanityCheck();
			if (FsmEditorSettings.EnableDebugFlow)
			{
				if (EditorApplication.get_isPaused() && GameStateTracker.PreviousState != GameState.Paused)
				{
					DebugFlow.Start(SkillEditor.SelectedFsm);
				}
				if (!EditorApplication.get_isPaused())
				{
					DebugFlow.Stop();
				}
			}
			if (!EditorApplication.get_isPlaying())
			{
				SkillEditor.Window.RemoveNotification();
				PlayMakerFSM.ApplicationIsQuitting = false;
			}
			SkillLog.set_LoggingEnabled(FsmEditorSettings.EnableLogging);
			SkillLog.set_MirrorDebugLog(FsmEditorSettings.MirrorDebugLog);
			SkillLog.set_EnableDebugFlow(FsmEditorSettings.EnableDebugFlow);
			if (GameStateTracker.PreviousState == GameState.Stopped || GameStateTracker.CurrentState == GameState.Stopped)
			{
				ActionReport.ActionReportList.Clear();
				FsmErrorChecker.ClearErrors(true);
				GlobalVariablesWindow.ResetView();
				FsmInspector.Init();
				SkillEditor.warnedAboutEditingWhileRunning = false;
			}
			SkillEditor.RepaintAll();
		}
		private static void ReselectFsm()
		{
			Skill selectedFsm = SkillEditor.SelectedFsm;
			SkillEditor.SelectNone();
			SkillEditor.SelectFsm(selectedFsm);
		}
		public static void SelectFsm(GameObject gameObject)
		{
			SkillEditor.SelectFsm(SkillEditor.SelectionHistory.GetFsmSelection(gameObject));
		}
		public static void SelectFsm(PlayMakerFSM fsmComponent)
		{
			if (fsmComponent != null)
			{
				SkillEditor.SelectFsm(fsmComponent.get_Fsm());
				return;
			}
			SkillEditor.SelectNone();
		}
		public static void SelectNone()
		{
			SkillEditor.DoDirtyFsmPrefab();
			if (SkillEditor.SelectedFsm != null)
			{
				SkillEditor.SelectedFsm.set_EditState(null);
			}
			if (SkillEditor.instance != null)
			{
				SkillEditor.instance.fsmSelection = SkillSelection.None;
				SkillEditor.instance.ResetViews();
			}
			CustomActionEditors.ClearCache();
		}
		public static void SelectFsm(Skill fsm)
		{
			if (SkillEditor.instance == null)
			{
				SkillEditor.selectFsmDelayed = fsm;
				return;
			}
			Labels.Update(fsm);
			if (fsm == SkillEditor.instance.fsmSelection.ActiveFsm)
			{
				return;
			}
			SkillEditor.SelectNone();
			Keyboard.ResetFocus();
			SkillEditor.instance.fsmSelection = SkillEditor.instance.selectionHistory.SelectFsm(fsm);
			PlayMakerGUI.SelectedFSM = SkillEditor.SelectedFsm;
			SkillEditor.Builder.SetTarget(SkillEditor.SelectedFsm);
			if (SkillEditor.SelectedFsm != null)
			{
				SkillEditor.GraphView.UpdateGraphSize();
				SkillEditor.GraphView.SanityCheckGraphBounds();
				SkillEditor.GraphView.UpdateVisibility();
				if (SkillEditor.SelectedTemplate == null)
				{
					SkillEditor.AutoAddPlayMakerGUI();
					if (!EditorApplication.get_isPlayingOrWillChangePlaymode())
					{
						ActionReport.Remove(SkillEditor.SelectedFsmComponent);
						SkillEditor.SelectedFsm.Reinitialize();
					}
				}
				if (SkillEditor.SelectedState != null)
				{
					SkillEditor.SelectedFsm.set_EditState(SkillEditor.SelectedState);
				}
				if (!SkillEditor.SelectedFsm.get_Initialized())
				{
					SkillEditor.SelectedFsm.Init(SkillEditor.SelectedFsmComponent);
				}
				FsmInspector.Init();
				SkillEditor.SanityCheckFsm(SkillEditor.SelectedFsm);
				FsmErrorChecker.CheckFsmForErrors(fsm, false);
			}
			if (!FsmEditorSettings.LockGraphView && FsmEditorSettings.AutoSelectGameObject)
			{
				SkillEditor.Selection.SelectActiveFsmGameObject();
			}
			if (Application.get_isPlaying() && FsmEditorSettings.SelectStateOnActivated && SkillEditor.SelectedFsm != null)
			{
				SkillEditor.SelectState(SkillEditor.SelectedFsm.get_ActiveState(), true);
			}
			VariableManager.SortVariables(SkillEditor.SelectedFsm);
			DebugFlow.SyncFsmLog(SkillEditor.SelectedFsm);
			Skill.set_StepToStateChange(false);
			SkillEditor.instance.ResetViews();
			SkillEditor.Repaint(true);
			SkillEditor.RepaintAll();
		}
		public static void RefreshInspector()
		{
			if (SkillEditor.instance != null)
			{
				SkillEditor.Inspector.ResetView();
			}
		}
		public static void RebuildFsmList()
		{
			SkillEditor.fsmList = new List<Skill>();
			SkillEditor.fsmComponentList = new List<PlayMakerFSM>();
			SkillEditor.fsmComponentList.AddRange(new List<PlayMakerFSM>((PlayMakerFSM[])Resources.FindObjectsOfTypeAll(typeof(PlayMakerFSM))));
			using (List<PlayMakerFSM>.Enumerator enumerator = SkillEditor.fsmComponentList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PlayMakerFSM current = enumerator.get_Current();
					if (current.get_Fsm() != null)
					{
						SkillEditor.fsmList.Add(current.get_Fsm());
					}
				}
			}
			if (!Application.get_isPlaying())
			{
				Templates.InitList();
				using (List<SkillTemplate>.Enumerator enumerator2 = Templates.List.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SkillTemplate current2 = enumerator2.get_Current();
						if (current2.fsm != null)
						{
							SkillEditor.fsmList.Add(current2.fsm);
						}
					}
				}
			}
			using (List<Skill>.Enumerator enumerator3 = SkillEditor.fsmList.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Skill current3 = enumerator3.get_Current();
					current3.CheckIfDirty();
					SkillState[] states = current3.get_States();
					for (int i = 0; i < states.Length; i++)
					{
						SkillState fsmState = states[i];
						fsmState.set_Fsm(current3);
					}
				}
			}
			SkillPrefabs.BuildAssetsWithPlayMakerFSMsList();
			EditorApplication.RepaintHierarchyWindow();
			EditorApplication.RepaintProjectWindow();
		}
		public static SkillState SelectState(SkillState state, bool frameState)
		{
			if (SkillEditor.instance == null || SkillEditor.SelectedFsmIsLocked)
			{
				return null;
			}
			if (state != SkillEditor.SelectedState)
			{
				state = SkillEditor.Selection.SelectState(state, false, false);
			}
			if ((FsmEditorSettings.FrameSelectedState || frameState) && SkillEditor.GraphView.IsStateOffscreen(state))
			{
				SkillEditor.GraphView.FrameState(state, true);
			}
			if (!Application.get_isPlaying())
			{
				SkillEditor.Inspector.SetMode(InspectorMode.StateInspector);
			}
			SkillEditor.Repaint(true);
			return state;
		}
		public static SkillStateAction SelectAction(SkillStateAction action, bool autoScroll = true)
		{
			if (SkillEditor.instance == null)
			{
				return null;
			}
			SkillEditor.instance.stateInspector.SelectAction(action, autoScroll);
			return action;
		}
		public static SkillStateAction SelectAction(SkillState state, int actionIndex, bool autoScroll = true)
		{
			if (state == null || actionIndex < 0 || actionIndex >= state.get_Actions().Length)
			{
				return null;
			}
			SkillStateAction action = state.get_Actions()[actionIndex];
			return SkillEditor.SelectAction(action, autoScroll);
		}
		public static void SelectTemplateDelayed(SkillTemplate template)
		{
			SkillEditor.selectTemplateDelayed = template;
		}
		public static void SelectFsmDelayed(Skill fsm)
		{
			SkillEditor.selectFsmDelayed = fsm;
		}
		public static void AddFsmComponent()
		{
			SkillEditor.RegisterUndo(Strings.get_Command_Add_FSM_Component());
			SkillBuilder.AddFsmToSelected();
		}
		public static void GotoBreakpoint()
		{
			if (Skill.get_BreakAtFsm() == null)
			{
				return;
			}
			SkillEditor.SelectFsm(Skill.get_BreakAtFsm());
			SkillEditor.SelectState(Skill.get_BreakAtState(), true);
		}
		public static void SelectFsm(object userdata)
		{
			Skill fsm = userdata as Skill;
			SkillEditor.SelectFsm(fsm);
		}
		public static void AddFsm()
		{
			SkillEditor.AddFsmComponent();
		}
		public static void SelectStateFromMenu(object userdata)
		{
			SkillEditor.SelectStateByName(userdata as string, true);
			SkillEditor.GraphView.FrameState(SkillEditor.Selection.ActiveState);
		}
		public static SkillState SelectStateByName(string stateName, bool frameState = true)
		{
			SkillState state = SkillEditor.SelectedFsm.GetState(stateName);
			return SkillEditor.SelectState(state, frameState);
		}
		public static bool SelectedFsmIsPrefab()
		{
			return SkillPrefabs.IsPrefab(SkillEditor.SelectedFsm);
		}
		public static bool SelectedFsmIsPrefabOrInstance()
		{
			return SkillPrefabs.IsPrefab(SkillEditor.SelectedFsm) || SkillPrefabs.IsPrefabInstance(SkillEditor.SelectedFsm);
		}
		public static bool SelectedFsmIsPersistent()
		{
			return SkillEditor.SelectedTemplate != null || SkillPrefabs.IsPrefab(SkillEditor.SelectedFsm);
		}
		public static void SelectFsmGameObject()
		{
			if (SkillEditor.SelectedFsmGameObject == null)
			{
				return;
			}
			UnityEditor.Selection.set_activeGameObject(SkillEditor.SelectedFsmGameObject);
			EditorGUIUtility.PingObject(SkillEditor.SelectedFsmGameObject);
		}
		public static void InstantiatePrefab()
		{
			if (SkillEditor.SelectedFsmGameObject == null)
			{
				return;
			}
			GameObject gameObject = (GameObject)PrefabUtility.InstantiatePrefab(SkillEditor.SelectedFsmGameObject);
			Undo.RegisterCreatedObjectUndo(gameObject, Strings.get_Command_Add_Template_to_Selected());
			UnityEditor.Selection.set_activeGameObject(gameObject);
		}
		public static void SelectPrefabParent()
		{
			PlayMakerFSM playMakerFSM = (PlayMakerFSM)PrefabUtility.GetPrefabParent(SkillEditor.SelectedFsmComponent);
			if (playMakerFSM != null)
			{
				SkillEditor.SelectFsm(playMakerFSM);
				UnityEditor.Selection.set_activeGameObject(playMakerFSM.get_gameObject());
			}
		}
		public static void ResetToPrefabState()
		{
			if (SkillEditor.SelectedFsmGameObject != null)
			{
				PrefabUtility.ResetToPrefabState(SkillEditor.SelectedFsm.get_Owner());
				SkillEditor.Inspector.ResetView();
				SkillEditor.RepaintAll();
			}
		}
		public static void ReconnectToLastPrefab()
		{
			if (SkillEditor.SelectedFsmGameObject != null)
			{
				PrefabUtility.ReconnectToLastPrefab(SkillEditor.SelectedFsmGameObject);
			}
		}
		public static void SaveActions()
		{
			SkillEditor.SaveActions(SkillEditor.SelectedFsm);
		}
		public static void SaveActions(Skill fsm)
		{
			if (fsm != null)
			{
				UndoUtility.RegisterUndo(SkillEditor.SelectedFsmOwner, "Edit Action");
				SkillState[] states = fsm.get_States();
				for (int i = 0; i < states.Length; i++)
				{
					SkillState fsmState = states[i];
					fsmState.SaveActions();
				}
				SkillEditor.SetFsmDirty(fsm, true, false, true);
			}
		}
		public static void SaveActions(SkillState state, bool errorCheck = true)
		{
			if (state != null)
			{
				if (state.get_Fsm() != null && state.get_Fsm().get_OwnerObject() != null)
				{
					UndoUtility.RegisterUndo(state.get_Fsm().get_OwnerObject(), "Edit Action");
				}
				state.SaveActions();
				if (state.get_Fsm() != null)
				{
					SkillEditor.SetFsmDirty(state.get_Fsm(), errorCheck, false, true);
				}
			}
		}
		public static void EditingActions()
		{
			if (SkillEditor.instance == null)
			{
				return;
			}
			SkillEditor.StateInspector.EditingActions();
		}
		public static void SetFsmDirty()
		{
			SkillEditor.SetFsmDirty(false, false);
		}
		public static void SetFsmDirty(bool errorCheck, bool checkAll = false)
		{
			if (SkillEditor.SelectedFsm != null)
			{
				if (!SkillEditor.SelectedFsmIsPrefabOrInstance())
				{
					SkillEditor.SetFsmDirty(SkillEditor.SelectedFsm, errorCheck, checkAll, true);
					return;
				}
				if (SkillEditor.dirtyFsmPrefab != null && (SkillEditor.dirtyFsmPrefab != SkillEditor.SelectedFsm || checkAll))
				{
					SkillEditor.DoDirtyFsmPrefab();
				}
				else
				{
					if (errorCheck && FsmEditorSettings.EnableRealtimeErrorChecker)
					{
						if (checkAll)
						{
							FsmErrorChecker.CheckForErrors();
						}
						else
						{
							FsmErrorChecker.CheckFsmForErrors(SkillEditor.SelectedFsm, false);
						}
					}
				}
				SkillEditor.dirtyFsmPrefab = SkillEditor.SelectedFsm;
				SkillEditor.instance.graphView.UpdateVisibility();
				SkillEditor.RepaintAll();
			}
		}
		public static void SetFsmDirty(Skill fsm, bool errorCheck, bool checkAll = false, bool modifiedWarning = true)
		{
			if (fsm == null)
			{
				return;
			}
			Labels.Update(fsm);
			fsm.set_Preprocessed(false);
			if (fsm.get_UsedInTemplate() != null)
			{
				EditorUtility.SetDirty(fsm.get_UsedInTemplate());
			}
			else
			{
				if (fsm.get_Owner() != null)
				{
					EditorUtility.SetDirty(fsm.get_Owner());
				}
				else
				{
					Debug.LogWarning("PlayMaker: Unhandled SetDirty: " + Labels.GetFullFsmLabel(fsm));
				}
			}
			if (SkillEditor.instance != null)
			{
				if (errorCheck && FsmEditorSettings.EnableRealtimeErrorChecker)
				{
					if (checkAll)
					{
						FsmErrorChecker.CheckForErrors();
					}
					else
					{
						FsmErrorChecker.CheckFsmForErrors(fsm, false);
					}
				}
				SkillEditor.instance.graphView.UpdateVisibility();
				SkillEditor.RepaintAll();
				if (Application.get_isPlaying() && modifiedWarning && !SkillEditor.warnedAboutEditingWhileRunning && FsmEditorSettings.ShowEditWhileRunningWarning)
				{
					SkillEditor.Window.ShowNotification(SkillPrefabs.IsPersistent(SkillEditor.SelectedFsm) ? new GUIContent(Strings.get_Dialog_Editing_Prefab_while_game_is_running()) : new GUIContent(Strings.get_Dialog_Editing_FSM_while_game_is_running()));
					SkillEditor.warnedAboutEditingWhileRunning = true;
				}
				SkillEditor.updateIsModifiedPrefabInstance = true;
			}
			if (SkillEditor.OnFsmChanged != null)
			{
				SkillEditor.OnFsmChanged(fsm);
			}
		}
		public static bool CreateAsset(Object asset, ref string path)
		{
			path = path.Replace("//", "/");
			if (asset == null)
			{
				Debug.LogError("Can't save null asset!");
				return false;
			}
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.Refresh();
			return true;
		}
		public static void SaveGlobals()
		{
			if (Application.get_isPlaying())
			{
				return;
			}
			if (!AssetDatabase.Contains(PlayMakerGlobals.get_Instance()))
			{
				string text = Path.Combine(SkillPaths.ResourcesPath, "PlayMakerGlobals.asset");
				SkillEditor.CreateAsset(PlayMakerGlobals.get_Instance(), ref text);
				EditorUtility.DisplayDialog(Strings.get_ProductName(), string.Format(Strings.get_Dialog_SaveGlobals_Created(), text), Strings.get_OK());
			}
			EditorUtility.SetDirty(PlayMakerGlobals.get_Instance());
		}
		public static void Repaint(bool instant = false)
		{
			if (SkillEditor.instance != null)
			{
				if (instant)
				{
					if (SkillEditor.Window != null)
					{
						SkillEditor.Window.Repaint();
						return;
					}
				}
				else
				{
					SkillEditor.instance.repaint = true;
				}
			}
		}
		public static void RepaintAll()
		{
			if (SkillEditor.instance != null)
			{
				SkillEditor.instance.repaintAll = true;
			}
		}
		public static void UpdateActionUsage()
		{
			SkillEditor.updateActionUsage = true;
		}
		public static void UpdateFsmInfo()
		{
			SkillEditor.updateFsmInfo = true;
		}
		public static void FsmInfoUpdated(Skill fsm)
		{
			if (fsm == SkillEditor.SelectedFsm)
			{
				SkillEditor.updateFsmInfo = false;
			}
		}
		public static void Cut()
		{
			if (SkillEditor.InspectorHasFocus && SkillEditor.Inspector.Mode == InspectorMode.StateInspector)
			{
				SkillEditor.StateInspector.CutSelectedActions();
				return;
			}
			EditorCommands.CutStateSelection();
		}
		public static void Copy()
		{
			if (SkillEditor.InspectorHasFocus && SkillEditor.Inspector.Mode == InspectorMode.StateInspector)
			{
				StateInspector.CopySelectedActions();
				return;
			}
			EditorCommands.CopyStateSelection();
		}
		public static void Paste()
		{
			if (!SkillEditor.Builder.CanPaste())
			{
				return;
			}
			if (SkillEditor.InspectorHasFocus && SkillEditor.Inspector.Mode == InspectorMode.StateInspector)
			{
				SkillEditor.StateInspector.PasteActionsAfterSelected(true);
				return;
			}
			EditorCommands.PasteStates();
		}
		public static void SelectAll()
		{
			if (SkillEditor.InspectorHasFocus && SkillEditor.Inspector.Mode == InspectorMode.StateInspector)
			{
				StateInspector.SelectAllActions();
				return;
			}
			EditorCommands.SelectAll();
		}
		public static void SanityCheck()
		{
			SkillEditor.RebuildFsmList();
			SkillEditor.SelectionHistory.SanityCheck();
			StateInspector.SanityCheckActionSelection();
		}
		public static bool FsmListContainsFsm(Skill fsm)
		{
			if (fsm == null)
			{
				return false;
			}
			using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					if (current == fsm)
					{
						return true;
					}
				}
			}
			return false;
		}
		public static bool FsmContainsState(Skill fsm, SkillState state)
		{
			if (fsm == null || state == null)
			{
				return false;
			}
			SkillState[] states = fsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				if (fsmState == state)
				{
					return true;
				}
			}
			return false;
		}
		public static bool StateContainsTransition(SkillState state, SkillTransition transition)
		{
			if (state == null || transition == null)
			{
				return false;
			}
			SkillTransition[] transitions = state.get_Transitions();
			for (int i = 0; i < transitions.Length; i++)
			{
				SkillTransition fsmTransition = transitions[i];
				if (fsmTransition == transition)
				{
					return true;
				}
			}
			return false;
		}
		public static void UndoRedoPerformed()
		{
			SkillEditor.SetFsmDirty(true, false);
			SkillEditor.SanityCheck();
			SkillEditor.UpdateActionUsage();
			SkillEditor.Inspector.ResetView();
			GlobalEventsWindow.ResetView();
			if (SkillEditor.SelectedFsm != null)
			{
				SkillEditor.SelectedFsm.Reinitialize();
			}
			if (FsmEditorSettings.EnableRealtimeErrorChecker)
			{
				FsmErrorChecker.CheckForErrors();
			}
			SkillEditor.GraphView.UpdateGraphSize();
			SkillEditor.GraphView.UpdateVisibility();
			SkillEditor.Window.Repaint();
			SkillEditor.RepaintAll();
		}
		[Localizable(false)]
		internal static void SetSnapshotTarget(string action)
		{
			if (SkillEditor.SelectedFsmOwner != null)
			{
				UndoUtility.SetSnapshotTarget(SkillEditor.SelectedFsmOwner, FsmEditorSettings.ProductName + " : " + action);
			}
		}
		[Localizable(false)]
		internal static void RegisterUndo(string action)
		{
			if (SkillEditor.SelectedFsmOwner != null)
			{
				UndoUtility.RegisterUndo(SkillEditor.SelectedFsmOwner, FsmEditorSettings.ProductName + " : " + action);
			}
		}
		[Localizable(false)]
		internal static void RegisterUndo(Skill fsm, string action)
		{
			if (fsm.get_OwnerObject() == null)
			{
				return;
			}
			UndoUtility.RegisterUndo(fsm.get_OwnerObject(), FsmEditorSettings.ProductName + " : " + action);
		}
		[Localizable(false)]
		internal static void RegisterUndo(Object unityObject, string action)
		{
			if (unityObject == null)
			{
				return;
			}
			UndoUtility.RegisterUndo(unityObject, FsmEditorSettings.ProductName + " : " + action);
		}
		internal static void RegisterActionEditUndo(string action)
		{
			if (SkillEditor.SelectedFsmComponent == null)
			{
				return;
			}
			UndoUtility.RegisterUndo(SkillEditor.SelectedFsmComponent, string.Format(Strings.get_Command_Edit_Action(), action));
		}
		[Localizable(false)]
		internal static void RegisterGlobalsUndo(string action)
		{
			UndoUtility.RegisterUndo(PlayMakerGlobals.get_Instance(), FsmEditorSettings.ProductName + " : " + action);
		}
		public static void ChangeLanguage()
		{
			SkillEditor.sendWindowCommandEvent = "ChangeLanguage";
		}
		public static void OpenWelcomeWindow()
		{
			SkillEditor.sendWindowCommandEvent = CommandEvents.get_OpenWelcomeWindow();
		}
		public static void OpenFsmSelectorWindow()
		{
			SkillEditor.sendWindowCommandEvent = CommandEvents.get_OpenFsmSelectorWindow();
		}
		public static void OpenFsmTemplateWindow()
		{
			SkillEditor.sendWindowCommandEvent = CommandEvents.get_OpenFsmTemplateWindow();
		}
		public static void OpenStateSelectorWindow()
		{
			SkillEditor.sendWindowCommandEvent = CommandEvents.get_OpenStateSelectorWindow();
		}
		public static void OpenActionWindow()
		{
			SkillEditor.sendWindowCommandEvent = CommandEvents.get_OpenActionWindow();
		}
		public static void OpenGlobalVariablesWindow()
		{
			SkillEditor.sendWindowCommandEvent = CommandEvents.get_OpenGlobalVariablesWindow();
		}
		public static void OpenGlobalEventsWindow()
		{
			SkillEditor.sendWindowCommandEvent = CommandEvents.get_OpenGlobalEventsWindow();
		}
		public static void OpenErrorWindow()
		{
			SkillEditor.sendWindowCommandEvent = CommandEvents.get_OpenErrorWindow();
		}
		public static void OpenToolWindow()
		{
			SkillEditor.sendWindowCommandEvent = CommandEvents.get_OpenToolWindow();
		}
		public static void OpenTimelineWindow()
		{
			SkillEditor.sendWindowCommandEvent = "OpenTimelineWindow";
		}
		public static void OpenFsmLogWindow()
		{
			SkillEditor.sendWindowCommandEvent = CommandEvents.get_OpenFsmLogWindow();
		}
		public static void OpenAboutWindow()
		{
			SkillEditor.sendWindowCommandEvent = CommandEvents.get_OpenAboutWindow();
		}
		public static void OpenReportWindow()
		{
			SkillEditor.sendWindowCommandEvent = CommandEvents.get_OpenReportWindow();
		}
		public static void OpenEventManager()
		{
			SkillEditor.Inspector.SetMode(InspectorMode.EventManager);
		}
		public static void OpenSearchWindow()
		{
			EditorWindow.GetWindow<SearchWindow>();
		}
		public static void DoDirtyFsmPrefab()
		{
			if (SkillEditor.dirtyFsmPrefab != null)
			{
				SkillEditor.SetFsmDirty(SkillEditor.dirtyFsmPrefab, false, false, true);
				SkillEditor.dirtyFsmPrefab = null;
				SkillEditor.SelectState(SkillEditor.SelectedState, false);
			}
		}
		private static void AutoAddPlayMakerGUI()
		{
			if (FsmEditorSettings.AutoAddPlayMakerGUI)
			{
				SkillEditor.AddPlayMakerGUI();
			}
		}
		private static void AddPlayMakerGUI()
		{
			if (!Application.get_isPlaying() && Object.FindObjectOfType(typeof(PlayMakerGUI)) as PlayMakerGUI == null)
			{
				PlayMakerGUI.get_Instance().set_enabled(true);
				Debug.Log(Strings.get_LOG_Auto_Added_Playmaker_GUI());
			}
		}
		private void OpenReport()
		{
			this.openReport = true;
		}
		private void DoOpenReport()
		{
			SkillEditor.OpenReportWindow();
			this.openReport = false;
		}
		private void ResetViews()
		{
			SkillSelector.RefreshView();
			this.graphView.RefreshView();
			this.inspector.ResetView();
			GlobalEventsWindow.ResetView();
			SkillLogger.ResetView();
			SkillEditor.RepaintAll();
		}
		private static void SanityCheckFsm(Skill fsm)
		{
			bool flag = false;
			for (int i = 0; i < fsm.get_States().Length; i++)
			{
				SkillState fsmState = fsm.get_States()[i];
				if (float.IsNaN(fsmState.get_Position().get_x()) || float.IsNaN(fsmState.get_Position().get_y()))
				{
					fsmState.set_Position(new Rect(100f, (float)(50 + 100 * i), fsmState.get_Position().get_width(), fsmState.get_Position().get_height()));
					flag = true;
				}
			}
			if (flag)
			{
				SkillEditor.SetFsmDirty(fsm, true, false, true);
				Debug.LogError("PlayMaker: FSM State Positions corrupted! Please submit a Bug Report.");
			}
			List<SkillEvent> list = new List<SkillEvent>();
			using (List<SkillEvent>.Enumerator enumerator = fsm.ExposedEvents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					if (SkillEvent.EventListContainsEvent(new List<SkillEvent>(fsm.get_Events()), current.get_Name()) && !SkillEvent.EventListContainsEvent(list, current.get_Name()))
					{
						list.Add(current);
					}
				}
			}
			fsm.ExposedEvents = list;
		}
		private static void ReloadActionsIfNeeded(SkillState state)
		{
			if (state.get_Fsm() != null && state.get_Actions().Length != state.get_ActionData().get_ActionCount())
			{
				state.LoadActions();
				if (FsmEditorSettings.EnableRealtimeErrorChecker)
				{
					FsmErrorChecker.CheckFsmForErrors(state.get_Fsm(), false);
				}
			}
		}
	}
}
