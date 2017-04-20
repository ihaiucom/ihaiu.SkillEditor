using System;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
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
			FsmEditorSettings.LoadSettings();
			this.justEnabled = true;
		}
		public abstract void Initialize();
		protected void SetTitle(string titleText)
		{
			base.set_titleContent(new GUIContent(titleText));
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
			this.currentEvent = Event.get_current();
			this.eventType = this.currentEvent.get_type();
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
