using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace ihaiu
{
    public class CanvasView
    {
        public class Marker
        {
            public Vector2 pos;
            public Color color;
        }
        private static float verticalScrollbarWidth;
        private static float horizontalScrollbarHeight;
        private EditorWindow parentWindow;
        private Rect parentWindowPosition;
        private float scale = 1f;
        private float minScale = 0.25f;
        private float maxScale = 1f;
        private float zoomSpeed = 0.01f;
        private float contentMargin = 0.5f;
        private Rect viewRect;
        private Vector2 scrollPosition;
        private Vector2 mousePosition;
        private bool mouseOverView;
        private Vector2 oldScrollPosition = Vector2.zero;
        private Rect activeView;
        private bool viewRectInitialized;
        private Vector2 setScrollPosition;
        private bool startPanToPosition;
        private bool isPanningToPosition;
        private Vector2 panToPositionEnd;
        private Vector2 panToPositionStart;
        private float panToPositionLerp;
        private string screenshotFilename;
        private Texture2D screenshotTexture;
        private Rect screenshotCaptureRect;
        private Vector2 screenshotTargetPosition;
        private Vector2 screenshotStartScrollPosition;
        private float screenshotInitialMargin;
        private bool showSavedScreenshotdialog;
        private string savedScreenshot;
        private bool graphRepainted;
        private bool isDocked;
        private bool stylesInitialized;
        private GUIStyle hiddenHorizontalScrollbar;
        private GUIStyle hiddenVerticalScrollbar;
        private float edgeScrollRampUp;
        private readonly List<CanvasView.Marker> markers = new List<CanvasView.Marker>();
        public Vector2 ContentSize
        {
            get;
            set;
        }
        public Rect ContentArea
        {
            get
            {
                return new Rect(this.viewRect.width - this.margin * 0.5f, this.viewRect.height - this.margin * 0.5f, this.ContentSize.x * this.scale, this.ContentSize.y * this.scale);
            }
        }
        public Rect Canvas
        {
            get
            {
                return new Rect(0f, 0f, this.CanvasSize.x, this.CanvasSize.y);
            }
        }
        public Rect ViewRectInCanvas
        {
            get
            {
                return new Rect(this.scrollPosition.x - this.ContentOrigin.x, this.scrollPosition.y - this.ContentOrigin.y, this.View.width, this.View.height);
            }
        }
        public Vector2 ScrollPosition
        {
            get
            {
                return this.scrollPosition;
            }
        }
        public bool ViewChanged
        {
            get;
            set;
        }
        public Vector2 MousePosition
        {
            get
            {
                return this.mousePosition;
            }
        }
        public float ContentMargin
        {
            get
            {
                return this.contentMargin;
            }
            set
            {
                this.contentMargin = value;
            }
        }
        public bool MouseWheelZoomsView
        {
            get;
            set;
        }
        public float Scale
        {
            get
            {
                return this.scale;
            }
        }
        public float MinScale
        {
            get
            {
                return this.minScale;
            }
            set
            {
                this.minScale = Mathf.Max(0.01f, value);
            }
        }
        public float MaxScale
        {
            get
            {
                return this.maxScale;
            }
            set
            {
                this.maxScale = value;
            }
        }
        public float ZoomSpeed
        {
            get
            {
                return this.zoomSpeed;
            }
            set
            {
                this.zoomSpeed = value;
            }
        }
        public Vector2 CanvasSize
        {
            get
            {
                return new Vector2(this.ContentSize.x * this.scale + this.viewRect.width * 2f - this.margin, this.ContentSize.y * this.scale + this.viewRect.height * 2f - this.margin);
            }
        }
        public Vector2 CanvasCenter
        {
            get
            {
                return this.CanvasSize * 0.5f;
            }
        }
        public Vector2 ContentOrigin
        {
            get
            {
                return new Vector2(this.ContentArea.x, this.ContentArea.y);
            }
        }
        public Rect View
        {
            get
            {
                return this.viewRect;
            }
        }
        public Rect ActiveView
        {
            get
            {
                return this.activeView;
            }
        }
        public Vector2 ViewOrigin
        {
            get
            {
                return new Vector2(this.viewRect.x, this.viewRect.y);
            }
        }
        public Vector2 ViewSize
        {
            get
            {
                return new Vector2(this.viewRect.width, this.viewRect.height);
            }
        }
        public Vector2 ViewCenter
        {
            get
            {
                return new Vector2(this.viewRect.width * 0.5f, this.viewRect.height * 0.5f);
            }
        }
        public bool DraggingCanvas
        {
            get;
            private set;
        }
        public bool TakingScreenshot
        {
            get;
            private set;
        }
        public bool ScreenshotFirstTile
        {
            get;
            private set;
        }
        public bool IsEdgeScrolling
        {
            get;
            private set;
        }
        public float EdgeScrollZone
        {
            get;
            set;
        }
        public float EdgeScrollSpeed
        {
            get;
            set;
        }
        private Vector2 screenshotSourcePosition
        {
            get
            {
                return new Vector2(this.scrollPosition.x - this.ContentArea.x, this.scrollPosition.y - this.ContentArea.y);
            }
        }
        private Vector2 viewCenter
        {
            get
            {
                return new Vector2(this.viewRect.width * 0.5f, this.viewRect.height * 0.5f);
            }
        }
        private float margin
        {
            get
            {
                return this.contentMargin * this.scale;
            }
        }
        public void Init(EditorWindow window)
        {
            this.parentWindow = window;
            this.viewRectInitialized = false;
        }
        public Vector2 BeginGUILayout(Rect view, bool showScrollBars)
        {
            this.oldScrollPosition.Set(this.scrollPosition.x - this.CanvasCenter.x, this.scrollPosition.y - this.CanvasCenter.y);
            this.HandleWindowResize();
            this.viewRect = view;
            this.activeView = view;
            if (showScrollBars)
            {
                this.activeView.width = (this.activeView.width - CanvasView.verticalScrollbarWidth);
                this.activeView.height = (this.activeView.height - CanvasView.horizontalScrollbarHeight);
            }
            if (!this.viewRectInitialized)
            {
                this.viewRectInitialized = true;
                this.SetContentScrollPosition(this.setScrollPosition);
                this.isPanningToPosition = false;
            }
            GUILayout.BeginArea(this.viewRect);
            this.InitStyles();
            this.HandleInputEvents();
            if (this.TakingScreenshot)
            {
                showScrollBars = false;
            }
            if (this.DraggingCanvas)
            {
                showScrollBars = true;
                EditorGUIUtility.AddCursorRect(this.Canvas, 13);
            }
            this.scrollPosition = (showScrollBars ? GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]) : GUILayout.BeginScrollView(this.scrollPosition, this.hiddenHorizontalScrollbar, this.hiddenVerticalScrollbar, new GUILayoutOption[0]));
            GUILayout.Box(GUIContent.none, GUIStyle.get_none(), new GUILayoutOption[]
                {
                    GUILayout.Width(this.CanvasSize.x),
                    GUILayout.Height(this.CanvasSize.y)
                });
            if (Event.get_current().get_type() == 7)
            {
                this.graphRepainted = true;
            }
            return this.scrollPosition - this.ContentOrigin;
        }
        public void MarkWorldPosition(Vector2 pos, Color color)
        {
            this.markers.Add(new CanvasView.Marker
                {
                    pos = pos,
                    color = color
                });
        }
        public void ClearMarkers()
        {
            this.markers.Clear();
        }
        public void DrawMarker(Vector2 pos, Color color)
        {
            Color color2 = GUI.get_color();
            GUI.set_color(color);
            GUI.Box(new Rect(pos.x - 5f, pos.y - 5f, 10f, 10f), GUIContent.none);
            GUI.set_color(color2);
        }
        public void EndGUILayout()
        {
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            if (Event.get_current().get_type() == 7 && this.TakingScreenshot)
            {
                this.CaptureScreenshotTile();
            }
            if (Math.Abs(this.scrollPosition.x - this.CanvasCenter.x - this.oldScrollPosition.x) > 1f || Math.Abs(this.scrollPosition.y - this.CanvasCenter.y - this.oldScrollPosition.y) > 1f)
            {
                this.ViewChanged = true;
            }
        }
        private void HandleWindowResize()
        {
            if (this.parentWindowPosition.width == 0f)
            {
                this.parentWindowPosition = this.parentWindow.get_position();
                return;
            }
            float num = this.parentWindow.get_position().width - this.parentWindowPosition.width;
            float num2 = this.parentWindow.get_position().height - this.parentWindowPosition.height;
            this.scrollPosition.Set(this.scrollPosition.x + num, this.scrollPosition.y + num2);
            this.parentWindowPosition = this.parentWindow.get_position();
            if (num != 0f || num2 != 0f)
            {
                this.ViewChanged = true;
            }
        }
        public void Update()
        {
            if (this.startPanToPosition)
            {
                this.PanToPosition(this.panToPositionEnd);
                this.startPanToPosition = false;
            }
            if (this.TakingScreenshot)
            {
                this.parentWindow.Repaint();
                this.ViewChanged = true;
            }
            if (this.isPanningToPosition)
            {
                this.DoPanToPosition();
                this.parentWindow.Repaint();
                this.ViewChanged = true;
            }
            if (this.IsEdgeScrolling)
            {
                this.edgeScrollRampUp += 0.05f;
                if (this.edgeScrollRampUp > 1f)
                {
                    this.edgeScrollRampUp = 1f;
                }
                this.parentWindow.Repaint();
                this.ViewChanged = true;
            }
            if (this.showSavedScreenshotdialog && this.graphRepainted)
            {
                this.showSavedScreenshotdialog = false;
                switch (EditorUtility.DisplayDialogComplex(Strings.get_Dialog_Saved_Screenshot(), this.savedScreenshot, Strings.get_OK(), Strings.get_Command_Open(), Strings.get_Command_Browse_Screenshots()))
                {
                    case 0:
                        break;
                    case 1:
                        EditorUtility.OpenWithDefaultApp(this.savedScreenshot);
                        return;
                    case 2:
                        {
                            string text = EditorUtility.OpenFilePanel(Strings.get_Title_Screenshots(), Path.GetDirectoryName(this.savedScreenshot), "png");
                            if (!string.IsNullOrEmpty(text))
                            {
                                EditorUtility.OpenWithDefaultApp(text);
                            }
                            break;
                        }
                    default:
                        return;
                }
            }
        }
        public void SetScrollPosition(Vector2 pos)
        {
            this.scrollPosition = pos;
        }
        public void CancelAutoPan()
        {
            this.startPanToPosition = false;
            this.isPanningToPosition = false;
        }
        public void SetContentScrollPosition(Vector2 pos)
        {
            if (!this.viewRectInitialized)
            {
                this.setScrollPosition = pos;
                return;
            }
            this.SetScrollPosition(pos + this.ContentOrigin);
        }
        public float ZoomView(Vector2 center, float delta)
        {
            this.CancelAutoPan();
            Vector2 vector = this.ViewToWorldCoordinates(center);
            this.scale = Mathf.Clamp(this.scale + delta, this.MinScale, this.MaxScale);
            this.SetScrollPosition(this.WorldToLocalCoordinates(vector - center / this.scale));
            this.parentWindow.Repaint();
            return this.scale;
        }
        public float SetZoom(float zoom)
        {
            this.scale = Mathf.Clamp(zoom, this.MinScale, this.MaxScale);
            return this.scale;
        }
        public bool IsVisible(Rect rect)
        {
            if (!this.viewRectInitialized)
            {
                return true;
            }
            if (this.scale < 1f)
            {
                return true;
            }
            if (this.scrollPosition.x < 0f)
            {
                return true;
            }
            float num = this.scrollPosition.x - this.ContentOrigin.x;
            float num2 = num + this.View.width / this.scale;
            float num3 = this.scrollPosition.y - this.ContentOrigin.y;
            float num4 = num3 + this.View.height / this.scale;
            return rect.x <= num2 && rect.x + rect.width >= num && rect.y <= num4 && rect.y + rect.height >= num3;
        }
        public void StartPanToPosition(Vector2 pos)
        {
            if (this.DraggingCanvas)
            {
                return;
            }
            this.startPanToPosition = true;
            this.panToPositionEnd = pos;
        }
        private void PanToPosition(Vector2 pos)
        {
            this.panToPositionStart = this.scrollPosition;
            this.panToPositionEnd = this.ContentOrigin + pos * this.scale - this.viewCenter;
            if ((this.panToPositionEnd - this.scrollPosition).get_magnitude() > 50f)
            {
                this.isPanningToPosition = true;
            }
        }
        private void DoPanToPosition()
        {
            this.panToPositionLerp = Mathf.Min(this.panToPositionLerp + 0.06f, 1f);
            float num = Mathf.SmoothStep(0f, 1f, this.panToPositionLerp);
            this.scrollPosition = Vector2.Lerp(this.panToPositionStart, this.panToPositionEnd, num);
            if (this.panToPositionLerp.Equals(1f))
            {
                this.panToPositionLerp = 0f;
                this.isPanningToPosition = false;
            }
        }
        public Vector2 DoEdgeScroll()
        {
            Vector2 vector = default(Vector2);
            if (this.mousePosition.x < this.EdgeScrollZone && this.mousePosition.x > 0f)
            {
                float num = -(this.EdgeScrollZone - this.mousePosition.x) / this.EdgeScrollZone;
                vector.x = this.EdgeScrollSpeed * Mathf.Pow(num, 3f);
            }
            else
            {
                if (this.mousePosition.x > this.viewRect.width - this.EdgeScrollZone && this.mousePosition.x < this.viewRect.width)
                {
                    float num2 = (this.mousePosition.x - (this.viewRect.width - this.EdgeScrollZone)) / this.EdgeScrollZone;
                    vector.x = this.EdgeScrollSpeed * Mathf.Pow(num2, 3f);
                }
            }
            if (this.mousePosition.y < this.EdgeScrollZone && this.mousePosition.y > 0f)
            {
                float num3 = -(this.EdgeScrollZone - this.mousePosition.y) / this.EdgeScrollZone;
                vector.y = this.EdgeScrollSpeed * Mathf.Pow(num3, 3f);
            }
            else
            {
                if (this.mousePosition.y > this.viewRect.height - this.EdgeScrollZone && this.mousePosition.y < this.viewRect.height)
                {
                    float num4 = (this.mousePosition.y - (this.viewRect.height - this.EdgeScrollZone)) / this.EdgeScrollZone;
                    vector.y = this.EdgeScrollSpeed * Mathf.Pow(num4, 3f);
                }
            }
            if (vector.get_sqrMagnitude() > 0f)
            {
                this.IsEdgeScrolling = true;
            }
            vector *= this.edgeScrollRampUp;
            if (vector.get_sqrMagnitude() > 0f)
            {
                this.scrollPosition += vector;
                if (vector.x <= 0f)
                {
                    float arg_209_0 = vector.y;
                }
            }
            return vector;
        }
        public void TakeScreenshot(string filename)
        {
            BindingFlags bindingFlags = 60;
            MethodInfo getMethod = typeof(EditorWindow).GetProperty("docked", bindingFlags).GetGetMethod(true);
            this.isDocked = (bool)getMethod.Invoke(this.parentWindow, null);
            this.CancelAutoPan();
            this.screenshotFilename = filename;
            this.TakingScreenshot = true;
            this.ScreenshotFirstTile = true;
            this.SetZoom(1f);
            Rect rect = new Rect(this.viewRect);
            rect.x = (0f);
            this.screenshotCaptureRect = rect;
            this.screenshotCaptureRect.width = (this.screenshotCaptureRect.width - 1f);
            this.screenshotCaptureRect.y = (this.parentWindow.get_position().height - this.screenshotCaptureRect.height - this.screenshotCaptureRect.y + 1f);
            this.screenshotCaptureRect.height = (this.screenshotCaptureRect.height - 1f);
            if (this.isDocked)
            {
                this.screenshotCaptureRect.width = (this.screenshotCaptureRect.width - 1f);
                this.screenshotCaptureRect.height = (this.screenshotCaptureRect.height - 1f);
            }
            this.screenshotTexture = new Texture2D((int)this.ContentSize.x, (int)this.ContentSize.y, 3, false);
            this.screenshotStartScrollPosition = this.scrollPosition;
            this.screenshotInitialMargin = this.contentMargin;
            this.contentMargin = 0f;
            this.SetContentScrollPosition(Vector2.get_zero());
            this.screenshotTargetPosition = new Vector2(0f, Mathf.Max(this.ContentSize.y - this.screenshotCaptureRect.height, 0f));
            this.parentWindow.Repaint();
            this.ViewChanged = true;
        }
        private void CaptureScreenshotTile()
        {
            Rect rect = new Rect(this.screenshotCaptureRect);
            bool flag = false;
            bool flag2 = false;
            if (this.screenshotSourcePosition.x + this.screenshotCaptureRect.width >= this.ContentArea.width)
            {
                rect.width = (this.ContentArea.width - this.screenshotSourcePosition.x);
                flag = true;
            }
            if (this.screenshotSourcePosition.y + this.screenshotCaptureRect.height >= this.ContentArea.height)
            {
                rect.height = (this.ContentArea.height - this.screenshotSourcePosition.y);
                rect.y = (this.screenshotCaptureRect.y + this.screenshotCaptureRect.height - rect.height);
                if (flag)
                {
                    flag2 = true;
                }
            }
            if (this.isDocked)
            {
                rect.x = (rect.x + 2f);
                rect.y = (rect.y + 2f);
            }
            this.screenshotTexture.ReadPixels(rect, (int)this.screenshotTargetPosition.x, (int)this.screenshotTargetPosition.y);
            this.screenshotTexture.Apply(false);
            this.ScreenshotFirstTile = false;
            if (flag2)
            {
                this.SaveScreenShot(true);
            }
            else
            {
                if (flag)
                {
                    this.scrollPosition.x = this.ContentArea.x;
                    this.scrollPosition.y = this.scrollPosition.y + this.screenshotCaptureRect.height;
                    this.screenshotTargetPosition.x = 0f;
                    this.screenshotTargetPosition.y = this.screenshotTargetPosition.y - this.screenshotCaptureRect.height;
                    if (this.screenshotTargetPosition.y < 0f)
                    {
                        this.screenshotTargetPosition.y = 0f;
                    }
                }
                else
                {
                    this.scrollPosition.x = this.scrollPosition.x + this.screenshotCaptureRect.width;
                    this.screenshotTargetPosition.x = this.screenshotTargetPosition.x + this.screenshotCaptureRect.width;
                }
            }
            this.parentWindow.Repaint();
            this.graphRepainted = false;
        }
        private void SaveScreenShot(bool showConfirmDialog = true)
        {
            if (this.screenshotTexture == null)
            {
                Debug.LogError(Strings.get_Error_Bad_screenshot_texture());
                return;
            }
            string fullPath = Path.GetFullPath(Application.get_dataPath() + "/" + this.screenshotFilename + ".png");
            Debug.Log(Strings.get_Log_Saving_FSM_Screenshot__() + fullPath);
            if (!CanvasView.CreateFilePath(fullPath))
            {
                return;
            }
            byte[] array = this.screenshotTexture.EncodeToPNG();
            Object.DestroyImmediate(this.screenshotTexture, true);
            File.WriteAllBytes(fullPath, array);
            this.TakingScreenshot = false;
            this.SetScrollPosition(this.screenshotStartScrollPosition);
            this.contentMargin = this.screenshotInitialMargin;
            this.parentWindow.Repaint();
            this.ViewChanged = true;
            this.graphRepainted = false;
            this.showSavedScreenshotdialog = showConfirmDialog;
            this.savedScreenshot = fullPath;
        }
        public Vector2 EventToViewCoordinates(Vector2 eventPos)
        {
            return new Vector2(eventPos.x - this.viewRect.x, eventPos.y - this.viewRect.y);
        }
        public Vector2 ContentToViewCoordinates(Vector2 contentPos)
        {
            return contentPos + this.ContentOrigin - this.ScrollPosition;
        }
        public Vector2 ViewToWorldCoordinates(Vector2 viewPos)
        {
            return this.LocalToWorldCoordinates(this.scrollPosition + viewPos);
        }
        public Vector2 LocalToWorldCoordinates(Vector2 localPos)
        {
            return (localPos - this.CanvasCenter) / this.scale;
        }
        public Vector2 WorldToLocalCoordinates(Vector2 worldPos)
        {
            return worldPos * this.scale + this.CanvasCenter;
        }
        public Vector2 WorldToViewCoordinates(Vector2 worldPos)
        {
            return this.WorldToLocalCoordinates(worldPos) - this.scrollPosition;
        }
        private static bool CreateFilePath(string fullFileName)
        {
            string directoryName = Path.GetDirectoryName(fullFileName);
            if (string.IsNullOrEmpty(directoryName))
            {
                Debug.LogError(string.Format(Strings.get_File_Invalid_Path(), fullFileName));
                return false;
            }
            try
            {
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }
            catch (Exception)
            {
                Debug.LogError(string.Format(Strings.get_File_Failed_To_Create_Directory(), directoryName));
                return false;
            }
            return true;
        }
        private void InitStyles()
        {
            if (!this.stylesInitialized)
            {
                CanvasView.horizontalScrollbarHeight = GUI.get_skin().get_horizontalScrollbar().get_fixedHeight();
                CanvasView.verticalScrollbarWidth = GUI.get_skin().get_verticalScrollbar().get_fixedWidth();
                GUIStyle gUIStyle = new GUIStyle(GUI.get_skin().get_horizontalScrollbar());
                gUIStyle.get_normal().set_background(null);
                gUIStyle.set_fixedHeight(0f);
                this.hiddenHorizontalScrollbar = gUIStyle;
                GUIStyle gUIStyle2 = new GUIStyle(GUI.get_skin().get_verticalScrollbar());
                gUIStyle2.get_normal().set_background(null);
                gUIStyle2.set_fixedWidth(0f);
                this.hiddenVerticalScrollbar = gUIStyle2;
                this.stylesInitialized = true;
            }
        }
        private void HandleInputEvents()
        {
            Event current = Event.get_current();
            EventType type = current.get_type();
            this.mousePosition = current.get_mousePosition();
            this.mouseOverView = (this.mousePosition.x < this.viewRect.width && this.mousePosition.y < this.viewRect.height);
            if (this.mouseOverView && type == 6)
            {
                if (this.MouseWheelZoomsView)
                {
                    if (!EditorGUI.get_actionKey())
                    {
                        this.ZoomView(this.mousePosition, -current.get_delta().y * this.zoomSpeed);
                        current.set_delta(new Vector2(0f, 0f));
                    }
                }
                else
                {
                    if (EditorGUI.get_actionKey())
                    {
                        this.ZoomView(this.mousePosition, -current.get_delta().y * this.zoomSpeed);
                        current.set_delta(new Vector2(0f, 0f));
                    }
                }
            }
            if (this.mouseOverView && type == 3 && (current.get_button() == 2 || Keyboard.AltAction()))
            {
                this.CancelAutoPan();
                this.DraggingCanvas = true;
                this.scrollPosition -= current.get_delta();
                this.parentWindow.Repaint();
                this.ViewChanged = true;
            }
            if (type == 1)
            {
                this.DraggingCanvas = false;
                this.IsEdgeScrolling = false;
                this.edgeScrollRampUp = 0f;
            }
            if (GUIUtility.get_keyboardControl() == 0 && type == 4)
            {
                KeyCode keyCode = Event.get_current().get_keyCode();
                KeyCode keyCode2 = keyCode;
                switch (keyCode2)
                {
                    case 43:
                        goto IL_1AF;
                    case 44:
                        return;
                    case 45:
                        break;
                    default:
                        if (keyCode2 == 61)
                        {
                            goto IL_1AF;
                        }
                        switch (keyCode2)
                        {
                            case 269:
                                break;
                            case 270:
                                goto IL_1AF;
                            default:
                                return;
                        }
                        break;
                }
                this.ZoomView(this.mousePosition, -0.1f);
                return;
                IL_1AF:
                this.ZoomView(this.mousePosition, 0.1f);
            }
        }
    }
}
