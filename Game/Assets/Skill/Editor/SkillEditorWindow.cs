using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ihaiu
{
    [System.Serializable]
    public class SkillEditorWindow : BaseEditorWindow
    {
        public static void OpenWindow()
        {
            GetWindow<SkillEditorWindow>();
        }

        private static SkillEditorWindow instance;
        public static bool IsOpen()
        {
            return instance != null;
        }

        [SerializeField]
        private SkillEditor fsmEditor;


        // tool windows (can't open them inside dll)
//
//        [SerializeField] private FsmSelectorWindow fsmSelectorWindow;    
//        [SerializeField] private FsmTemplateWindow fsmTemplateWindow;
//        [SerializeField] private FsmStateWindow stateSelectorWindow;
//        [SerializeField] private FsmActionWindow actionWindow;
//        [SerializeField] private FsmErrorWindow errorWindow;
//        [SerializeField] private TimelineWindow timelineWindow;
//        [SerializeField] private FsmLogWindow logWindow;
//        [SerializeField] private ContextToolWindow toolWindow;
//        [SerializeField] private GlobalEventsWindow globalEventsWindow;
//        [SerializeField] private GlobalVariablesWindow globalVariablesWindow;
//        [SerializeField] private ReportWindow reportWindow;
//        [SerializeField] private AboutWindow aboutWindow;

        public override void Initialize()
        {
            instance = this;

            if (fsmEditor == null)
            {
                fsmEditor = new SkillEditor();
            }

            fsmEditor.InitWindow(this);
            fsmEditor.OnEnable();
        }



        public override void DoGUI()
        {
            fsmEditor.OnGUI();

            /* Debug Repaint events
            if (Event.current.type == EventType.repaint)
            {
                Debug.Log("Repaint");
            }*/

            if (Event.current.type == EventType.ValidateCommand)
            {
                switch (Event.current.commandName)
                {
                    case "UndoRedoPerformed":
                    case "Cut":
                    case "Copy":
                    case "Paste":
                    case "SelectAll":
                        Event.current.Use();
                        break;
                }
            }

            if (Event.current.type == EventType.ExecuteCommand)
            {
                switch (Event.current.commandName)
                {
                    /* replaced with Undo.undoRedoPerformed callback added in Unity 4.3
                    case "UndoRedoPerformed":
                        SkillEditor.UndoRedoPerformed();
                        break;
                    */

//                    case "Cut":
//                        SkillEditor.Cut();
//                        break;
//
//                    case "Copy":
//                        SkillEditor.Copy();
//                        break;
//
//                    case "Paste":
//                        SkillEditor.Paste();
//                        break;
//
//                    case "SelectAll":
//                        SkillEditor.SelectAll();
//                        break;

//                    case "OpenWelcomeWindow":
//                        GetWindow<PlayMakerWelcomeWindow>();
//                        break;
//
//                    case "OpenToolWindow":
//                        toolWindow = GetWindow<ContextToolWindow>();
//                        break;
//
//                    case "OpenFsmSelectorWindow":
//                        fsmSelectorWindow = GetWindow<FsmSelectorWindow>();
//                        fsmSelectorWindow.ShowUtility();
//                        break;
//
//                    case "OpenFsmTemplateWindow":
//                        fsmTemplateWindow = GetWindow<FsmTemplateWindow>();
//                        break;
//
//                    case "OpenStateSelectorWindow":
//                        stateSelectorWindow = GetWindow<FsmStateWindow>();
//                        break;
//
//                    case "OpenActionWindow":
//                        actionWindow = GetWindow<FsmActionWindow>();
//                        break;
//
//                    case "OpenGlobalEventsWindow":
//                        globalEventsWindow = GetWindow<FsmEventsWindow>();
//                        break;
//
//                    case "OpenGlobalVariablesWindow":
//                        globalVariablesWindow = GetWindow<FsmGlobalsWindow>();
//                        break;
//
//                    case "OpenErrorWindow":
//                        errorWindow = GetWindow<FsmErrorWindow>();
//                        break;
//
//                    case "OpenTimelineWindow":
//                        timelineWindow = GetWindow<FsmTimelineWindow>();
//                        break;
//
//                    case "OpenFsmLogWindow":
//                        logWindow = GetWindow<FsmLogWindow>();
//                        break;
//
//                    case "OpenAboutWindow":
//                        aboutWindow = GetWindow<AboutWindow>();
//                        break;
//
//                    case "OpenReportWindow":
//                        reportWindow = GetWindow<ReportWindow>();
//                        break;
//
//                    case "AddFsmComponent":
//                        PlayMakerMainMenu.AddFsmToSelected();
//                        break;

                    case "RepaintAll":
                        RepaintAllWindows();
                        break;

                    case "ChangeLanguage":
                        ResetWindowTitles();
                        break;
                }

                GUIUtility.ExitGUI();
            }
        }

        // called when you change editor language
        public void ResetWindowTitles()
        {
//            if (toolWindow != null)
//            {
//                toolWindow.InitWindowTitle();
//            }
//
//            if (fsmSelectorWindow != null)
//            {
//                fsmSelectorWindow.InitWindowTitle();
//            }
//
//            if (stateSelectorWindow != null)
//            {
//                stateSelectorWindow.InitWindowTitle();
//            }
//
//            if (actionWindow != null)
//            {
//                actionWindow.InitWindowTitle();
//            }
//
//            if (globalEventsWindow != null)
//            {
//                globalEventsWindow.InitWindowTitle();
//            }
//
//            if (globalVariablesWindow != null)
//            {
//                globalVariablesWindow.InitWindowTitle();
//            }
//
//            if (errorWindow != null)
//            {
//                errorWindow.InitWindowTitle();
//            }
//
//            if (timelineWindow != null)
//            {
//                timelineWindow.InitWindowTitle();
//            }
//
//            if (logWindow != null)
//            {
//                logWindow.InitWindowTitle();
//            }
//
//            if (reportWindow != null)
//            {
//                reportWindow.InitWindowTitle();
//            }
//
//            if (fsmTemplateWindow != null)
//            {
//                fsmTemplateWindow.InitWindowTitle();
//            }
        }

        public void RepaintAllWindows()
        {
//            if (toolWindow != null)
//            {
//                toolWindow.Repaint();
//            }
//
//            if (fsmSelectorWindow != null)
//            {
//                fsmSelectorWindow.Repaint();
//            }
//
//            if (stateSelectorWindow != null)
//            {
//                stateSelectorWindow.Repaint();
//            }
//
//            if (actionWindow != null)
//            {
//                actionWindow.Repaint();
//            }
//
//            if (globalEventsWindow != null)
//            {
//                globalEventsWindow.Repaint();
//            }
//
//            if (globalVariablesWindow != null)
//            {
//                globalVariablesWindow.Repaint();
//            }
//
//            if (errorWindow != null)
//            {
//                errorWindow.Repaint();
//            }
//
//            if (timelineWindow != null)
//            {
//                timelineWindow.Repaint();
//            }
//
//            if (logWindow != null)
//            {
//                logWindow.Repaint();
//            }
//
//            if (reportWindow != null)
//            {
//                reportWindow.Repaint();
//            }
//
//            if (fsmTemplateWindow != null)
//            {
//                fsmTemplateWindow.Repaint();
//            }

            Repaint();
        }

        private void Update()
        {
            if (Initialized && fsmEditor != null)
            {
                fsmEditor.Update();
            }
        }

        private void OnInspectorUpdate()
        {
            if (Initialized && fsmEditor != null)
            {
                fsmEditor.OnInspectorUpdate();
            }
        }

        private void OnFocus()
        {
            if (Initialized && fsmEditor != null)
            {
                fsmEditor.OnFocus();
            }
        }

        private void OnSelectionChange()
        {
            if (Initialized && fsmEditor != null)
            {
                fsmEditor.OnSelectionChange();
            }
        }

        private void OnHierarchyChange()
        {
            if (Initialized && fsmEditor != null)
            {
                fsmEditor.OnHierarchyChange();
            }
        }

        private void OnProjectChange()
        {
            if (Initialized && fsmEditor != null)
            {
                fsmEditor.OnProjectChange();
            }
        }

        private void OnDisable()
        {
            if (Initialized && fsmEditor != null)
            {
                fsmEditor.OnDisable();
            }

            instance = null;
        }

        private void OnDestroy()
        {
//            if (toolWindow != null)
//            {
//                toolWindow.SafeClose();
//            }
//
//            if (fsmSelectorWindow != null)
//            {
//                fsmSelectorWindow.SafeClose();
//            }
//
//            if (fsmTemplateWindow != null)
//            {
//                fsmTemplateWindow.SafeClose();
//            }
//
//            if (stateSelectorWindow != null)
//            {
//                stateSelectorWindow.SafeClose();
//            }
//
//            if (actionWindow != null)
//            {
//                actionWindow.SafeClose();
//            }
//
//            if (globalVariablesWindow != null)
//            {
//                globalVariablesWindow.SafeClose();
//            }
//
//            if (globalEventsWindow != null)
//            {
//                globalEventsWindow.SafeClose();
//            }
//
//            if (errorWindow != null)
//            {
//                errorWindow.SafeClose();
//            }
//
//            if (timelineWindow != null)
//            {
//                timelineWindow.SafeClose();
//            }
//
//            if (logWindow != null)
//            {
//                logWindow.SafeClose();
//            }
//
//            if (reportWindow != null)
//            {
//                reportWindow.SafeClose();
//            }
//
//            if (aboutWindow != null)
//            {
//                aboutWindow.SafeClose();
//            }

            if (Initialized && fsmEditor != null)
            {
                fsmEditor.OnDestroy();
            }
        }






        private static Texture2D _lineTexture;
        public static Texture2D LineTexture
        {
            get
            {
                if (_lineTexture == null)
                {
                    _lineTexture = Resources.Load<Texture2D>("line");
                }
                return _lineTexture;
            }
        }



//        Rect windowRect = new Rect (100 + 100, 100, 100, 100);
//        Rect windowRect2 = new Rect (100, 100, 100, 100);
//
//        private void OnGUI()
//        {
//            Handles.BeginGUI();
//            Handles.DrawBezier(windowRect.center, windowRect2.center, new Vector2(windowRect.xMax + 50f,windowRect.center.y), new Vector2(windowRect2.xMin - 50f,windowRect2.center.y),Color.red,null,5f);
//            Handles.EndGUI();
//
//            BeginWindows();
//            windowRect = GUI.Window (0, windowRect, WindowFunction, "Box1");
//            windowRect2 = GUI.Window (1, windowRect2, WindowFunction, "Box2");
//
//            EndWindows();
//
//        }
//        void WindowFunction (int windowID) 
//        {
//            GUI.DragWindow();
//        }
//
//
//        List<Rect> windows = new List<Rect>();
//        List<int> windowsToAttach = new List<int>();
//        List<int> attachedWindows = new List<int>();
//
//
//        void OnGUI() {
//            if (windowsToAttach.Count == 2) {
//                attachedWindows.Add(windowsToAttach[0]);
//                attachedWindows.Add(windowsToAttach[1]);
//                windowsToAttach = new List<int>();
//            }
//
//            if (attachedWindows.Count >= 2) {
//                for (int i = 0; i < attachedWindows.Count; i += 2) {
//                    DrawNodeCurve(windows[attachedWindows[i]], windows[attachedWindows[i + 1]]);
//                }
//            }
//
//            BeginWindows();
//
//            if (GUILayout.Button("Create Node")) {
//                windows.Add(new Rect(10, 10, 100, 100));
//            }
//
//            for (int i = 0; i < windows.Count; i++) {
//                windows[i] = GUI.Window(i, windows[i], DrawNodeWindow, "Window " + i);
//            }
//
//            EndWindows();
//        }
//
//
//        void DrawNodeWindow(int id) {
//            if (GUILayout.Button("Attach")) {
//                windowsToAttach.Add(id);
//            }
//
//            GUI.DragWindow();
//        }
//
//
//        void DrawNodeCurve(Rect start, Rect end) {
//            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
//            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
//            Vector3 startTan = startPos + Vector3.right * 50;
//            Vector3 endTan = endPos + Vector3.left * 50;
//            Color shadowCol = new Color(0, 0, 0, 0.06f);
//
//            for (int i = 0; i < 3; i++) {// Draw a shadow
//                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
//            }
//
//            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
//        }

    }
}
