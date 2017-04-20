using HutongGames.PlayMakerEditor;
using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.Editor
{
	[Localizable(false)]
	public class TextField : BaseControl
	{
		public delegate void EditCommitedCallback(TextField textField);
		public delegate void LostFocusCallback(TextField textField);
		public delegate void EditingCancelledCallback(TextField textField);
		public delegate bool ValidateCallback(string text);
		private static GUIStyle hintTextStyle;
		private GUIContent label = GUIContent.none;
		private string text;
		private string[] autoCompleteStrings;
		private bool showBrowseButton;
		public TextField.EditCommitedCallback EditCommited;
		public TextField.LostFocusCallback FocusLost;
		public TextField.EditingCancelledCallback EditCanceled;
		public TextField.ValidateCallback ValidateText;
		private bool hadFocus;
		private string originalText;
		public static GUIStyle HintTextStyle
		{
			get
			{
				if (TextField.hintTextStyle == null)
				{
					TextField.hintTextStyle = new GUIStyle(GUI.get_skin().get_textField());
					TextField.hintTextStyle.get_normal().set_textColor(new Color(0.5f, 0.5f, 0.5f, 0.75f));
				}
				return TextField.hintTextStyle;
			}
			set
			{
				TextField.hintTextStyle = value;
			}
		}
		public GUIContent Label
		{
			get
			{
				return this.label;
			}
			set
			{
				this.label = value;
			}
		}
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				if (this.text != value)
				{
					this.text = value;
					this.DoValidateText();
				}
			}
		}
		public string HintText
		{
			get;
			set;
		}
		public string[] AutoCompleteStrings
		{
			get
			{
				return this.autoCompleteStrings;
			}
			set
			{
				this.autoCompleteStrings = value;
				this.showBrowseButton = (this.autoCompleteStrings != null && this.autoCompleteStrings.Length > 0);
			}
		}
		public bool IsValid
		{
			get;
			private set;
		}
		public TextField(EditorWindow window, string label, string text = "") : base(window)
		{
			this.Label = new GUIContent(label);
			this.Text = text;
		}
		public TextField(EditorWindow window, GUIContent label, string text = "") : base(window)
		{
			this.Label = label;
			this.Text = text;
		}
		public override void OnGUI(params GUILayoutOption[] options)
		{
			base.OnGUI(options);
			EditorGUI.BeginChangeCheck();
			if (this.showBrowseButton)
			{
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			}
			if (base.HasFocus)
			{
				if (!this.hadFocus)
				{
					this.originalText = this.Text;
				}
				if (Event.get_current().get_isKey())
				{
					if (Keyboard.CommitKeyPressed())
					{
						this.CommitEdit();
						Event.get_current().Use();
					}
					else
					{
						if (Event.get_current().get_keyCode() == 27)
						{
							this.CancelEdit();
							Event.get_current().Use();
						}
					}
				}
				this.hadFocus = true;
			}
			else
			{
				if (this.hadFocus)
				{
					this.hadFocus = false;
					this.LostFocus();
				}
			}
			if (!base.HasFocus && string.IsNullOrEmpty(this.Text) && !string.IsNullOrEmpty(this.HintText))
			{
				EditorGUILayout.TextField(this.Label, this.HintText, TextField.HintTextStyle, options);
			}
			else
			{
				this.Text = EditorGUILayout.TextField(this.Label, this.Text, options);
			}
			if (this.showBrowseButton)
			{
				this.DoBrowseButton();
				EditorGUILayout.EndHorizontal();
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.DoValidateText();
			}
			base.UpdateFocus();
		}
		private void DoBrowseButton()
		{
			int num = EditorGUILayout.Popup(-1, this.AutoCompleteStrings, new GUILayoutOption[]
			{
				GUILayout.Width(20f)
			});
			if (num != -1)
			{
				this.Text = this.AutoCompleteStrings[num];
				this.CommitEdit();
			}
		}
		public void CancelTextEdit()
		{
			this.Text = this.originalText;
		}
		public void Validate()
		{
			this.DoValidateText();
		}
		private void DoValidateText()
		{
			this.IsValid = (this.ValidateText == null || this.ValidateText(this.Text));
		}
		public void CommitEdit()
		{
			this.Validate();
			if (this.IsValid)
			{
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoCommitEdit));
			}
		}
		private void CancelEdit()
		{
			if (this.EditCanceled != null)
			{
				this.EditCanceled(this);
			}
			this.CancelTextEdit();
			GUIUtility.set_keyboardControl(0);
			this.hadFocus = false;
		}
		private void LostFocus()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoLostFocus));
		}
		private void DoCommitEdit()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoCommitEdit));
			if (this.EditCommited != null)
			{
				this.EditCommited(this);
			}
			this.hadFocus = false;
		}
		private void DoLostFocus()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoLostFocus));
			if (this.FocusLost != null)
			{
				this.FocusLost(this);
			}
		}
	}
}
