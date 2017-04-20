using System;
using UnityEditor;
using UnityEngine;
namespace HutongGames.Editor
{
	public class EditableLabel : BaseControl
	{
		public delegate void EditCommitedCallback(EditableLabel editableLabel);
		public delegate void ContextClickCallback(EditableLabel editableLabel);
		private bool isEditing;
		private string originalText;
		private bool setIsEditing;
		public EditableLabel.EditCommitedCallback EditCommited;
		public EditableLabel.ContextClickCallback ContextClick;
		public TextField editableTextField
		{
			get;
			private set;
		}
		public string Text
		{
			get;
			set;
		}
		public EditableLabel(EditorWindow window, string text = "") : base(window)
		{
			base.Style = EditorStyles.get_label();
			this.Text = text;
			this.editableTextField = new TextField(window, GUIContent.none, text);
			this.editableTextField.EditCommited = new TextField.EditCommitedCallback(this.CommitEdit);
			this.editableTextField.FocusLost = new TextField.LostFocusCallback(this.CommitEdit);
			this.editableTextField.ControlName = "Category";
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
		}
		public override void OnGUI(params GUILayoutOption[] options)
		{
			base.OnGUI(options);
			if (this.isEditing)
			{
				this.editableTextField.OnGUI(new GUILayoutOption[0]);
			}
			else
			{
				GUI.SetNextControlName("Category");
				GUILayout.Space(2f);
				GUILayout.Label(this.Text, base.Style, options);
				if (GUILayoutUtility.GetLastRect().Contains(Event.get_current().get_mousePosition()))
				{
					if (Event.get_current().get_clickCount() == 2)
					{
						this.StartEditing();
					}
					if (Event.get_current().get_type() == 16)
					{
						if (this.ContextClick != null)
						{
							this.ContextClick(this);
						}
						Event.get_current().Use();
					}
				}
				GUILayout.Space(2f);
			}
			base.UpdateFocus();
		}
		public void StartEditing()
		{
			this.setIsEditing = true;
			this.originalText = this.Text;
			this.editableTextField.Focus();
			this.window.Repaint();
		}
		public void StopEditing()
		{
			this.setIsEditing = false;
			this.window.Repaint();
		}
		public void CancelEditing()
		{
			this.Text = this.originalText;
			this.setIsEditing = false;
			this.window.Repaint();
			GUIHelpers.SafeExitGUI();
		}
		private void CommitEdit(TextField textField)
		{
			this.Text = textField.Text;
			this.setIsEditing = false;
			this.window.Repaint();
			if (this.EditCommited != null)
			{
				this.EditCommited(this);
			}
		}
		public void Update()
		{
			this.isEditing = this.setIsEditing;
		}
	}
}
