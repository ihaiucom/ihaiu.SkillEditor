using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.Editor
{
	[Localizable(false)]
	public abstract class BaseControl
	{
		private static int nextControlID;
		protected readonly EditorWindow window;
		protected string controlName;
		private bool focus;
		private bool hasFocus;
		public string ControlName
		{
			get
			{
				return this.controlName;
			}
			set
			{
				this.controlName = value;
			}
		}
		public bool HasFocus
		{
			get
			{
				return this.hasFocus;
			}
		}
		public GUIStyle Style
		{
			get;
			set;
		}
		public int ID
		{
			get;
			set;
		}
		protected BaseControl(EditorWindow window)
		{
			this.window = window;
			this.controlName = base.GetType().get_Name() + "_" + BaseControl.nextControlID++;
		}
		public virtual void OnGUI(params GUILayoutOption[] options)
		{
			GUI.SetNextControlName(this.controlName);
			this.hasFocus = (GUI.GetNameOfFocusedControl() == this.controlName);
		}
		public void Focus()
		{
			this.focus = true;
		}
		public void UpdateFocus()
		{
			if (this.focus)
			{
				GUI.FocusControl(this.controlName);
				EditorGUI.FocusTextInControl(this.controlName);
				this.focus = false;
			}
		}
		public void Repaint()
		{
			this.window.Repaint();
		}
	}
}
