using UnityEngine;
using System.Collections;
using UnityEditor;


namespace ihaiu
{
    public abstract class BaseEditorWindow : EditorWindow 
    {
        public bool Initialized;
        protected bool isToolWindow;
        protected Event currentEvent;
        protected EventType eventType;
        private bool justEnabled;
        protected virtual void OnEnable()
        {
            SkillEditorSettings.LoadSettings();
            this.justEnabled = true;
        }

        public abstract void Initialize();
        protected void SetTitle(string titleText)
        {
            base.titleContent = new GUIContent(titleText);
        }

        public void OnGUI()
        {
            if (this.justEnabled)
            {
                this.Initialize();
                this.justEnabled = false;
                this.Initialized = true;
            }

            if (this.isToolWindow && !SkillEditorGUILayout.ToolWindowsCommonGUI(this))
            {
                return;
            }
            this.currentEvent = Event.current;
            this.eventType = this.currentEvent.type;
            this.DoGUI();
        }

        public abstract void DoGUI();
        public void SafeClose()
        {
            if (!this.justEnabled)
            {
                base.Close();
            }
        }
    }
}
