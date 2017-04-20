using HutongGames.Editor;
using System;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class NewEventWindow : EditorWindow
	{
		private string titleLabel;
		private TextField editField;
		private string validationError;
		private Vector3[] framePoints;
		public TextField.EditCommitedCallback EditCommited
		{
			get
			{
				return this.editField.EditCommited;
			}
			set
			{
				this.editField.EditCommited = value;
			}
		}
		public static NewEventWindow CreateDropdown(string title, Rect buttonRect, string eventName)
		{
			NewEventWindow newEventWindow = ScriptableObject.CreateInstance<NewEventWindow>();
			newEventWindow.Init(title, eventName);
			newEventWindow.ShowAsDropDown(buttonRect, new Vector2(194f, 70f));
			return newEventWindow;
		}
		private void Init(string windowTitle, string eventName)
		{
			this.titleLabel = windowTitle;
			this.editField = new TextField(this, GUIContent.none, "");
			this.editField.Focus();
			this.editField.EditCommited = new TextField.EditCommitedCallback(this.CommitNewEvent);
			this.editField.EditCanceled = new TextField.EditingCancelledCallback(this.Cancel);
			this.editField.ValidateText = new TextField.ValidateCallback(this.ValidateEvent);
			this.editField.Text = eventName;
			this.editField.Validate();
			this.framePoints = new Vector3[5];
			this.framePoints[0] = new Vector3(0f, 1f);
			this.framePoints[1] = new Vector3(base.get_position().get_width() - 1f, 1f);
			this.framePoints[2] = new Vector3(base.get_position().get_width() - 1f, base.get_position().get_height() - 1f);
			this.framePoints[3] = new Vector3(1f, base.get_position().get_height() - 1f);
			this.framePoints[4] = new Vector3(1f, 1f);
		}
		public void OnGUI()
		{
			if (!SkillEditorStyles.IsInitialized())
			{
				SkillEditorStyles.Init();
			}
			GUILayout.Label(this.titleLabel, EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			this.editField.OnGUI(new GUILayoutOption[0]);
			if (!string.IsNullOrEmpty(this.validationError))
			{
				GUILayout.Box(this.validationError, SkillEditorStyles.ErrorBox, new GUILayoutOption[0]);
			}
			else
			{
				if (GUILayout.Button(Strings.get_Label_Create_Event(), new GUILayoutOption[0]))
				{
					this.editField.CommitEdit();
				}
			}
			if (Event.get_current().get_type() == 7)
			{
				Color labelTextColor = SkillEditorStyles.LabelTextColor;
				labelTextColor.a = 0.5f;
				Handles.set_color(labelTextColor);
				Handles.DrawPolyLine(this.framePoints);
			}
		}
		private void CommitNewEvent(TextField textField)
		{
			base.Close();
		}
		private void Cancel(TextField textField)
		{
			base.Close();
		}
		private bool ValidateEvent(string eventName)
		{
			this.validationError = "";
			if (string.IsNullOrEmpty(eventName))
			{
				this.validationError = Strings.get_Error_Invalid_Event_Name();
				return false;
			}
			return true;
		}
	}
}
