using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ihaiu
{
    public class SkillEditorWindow : EditorWindow
    {
        public static void OpenWindow()
        {
            GetWindow<SkillEditorWindow>();
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


        List<Rect> windows = new List<Rect>();
        List<int> windowsToAttach = new List<int>();
        List<int> attachedWindows = new List<int>();


        void OnGUI() {
            if (windowsToAttach.Count == 2) {
                attachedWindows.Add(windowsToAttach[0]);
                attachedWindows.Add(windowsToAttach[1]);
                windowsToAttach = new List<int>();
            }

            if (attachedWindows.Count >= 2) {
                for (int i = 0; i < attachedWindows.Count; i += 2) {
                    DrawNodeCurve(windows[attachedWindows[i]], windows[attachedWindows[i + 1]]);
                }
            }

            BeginWindows();

            if (GUILayout.Button("Create Node")) {
                windows.Add(new Rect(10, 10, 100, 100));
            }

            for (int i = 0; i < windows.Count; i++) {
                windows[i] = GUI.Window(i, windows[i], DrawNodeWindow, "Window " + i);
            }

            EndWindows();
        }


        void DrawNodeWindow(int id) {
            if (GUILayout.Button("Attach")) {
                windowsToAttach.Add(id);
            }

            GUI.DragWindow();
        }


        void DrawNodeCurve(Rect start, Rect end) {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = new Color(0, 0, 0, 0.06f);

            for (int i = 0; i < 3; i++) {// Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
        }

    }
}
