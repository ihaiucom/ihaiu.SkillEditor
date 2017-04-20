using HutongGames.Editor;
using HutongGames.PlayMaker;
using System;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class NewVariableWindow : EditorWindow
	{
		private string titleLabel;
		private SkillVariables fsmVariables;
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
		public static NewVariableWindow CreateDropdown(string title, Rect buttonRect, SkillVariables fsmVariables, string variableName)
		{
			NewVariableWindow newVariableWindow = ScriptableObject.CreateInstance<NewVariableWindow>();
			newVariableWindow.Init(title, fsmVariables, variableName);
			newVariableWindow.ShowAsDropDown(buttonRect, new Vector2(194f, 70f));
			return newVariableWindow;
		}
		private void Init(string windowTitle, SkillVariables variables, string variableName)
		{
			this.titleLabel = windowTitle;
			this.fsmVariables = variables;
			this.editField = new TextField(this, GUIContent.none, "");
			this.editField.Focus();
			this.editField.EditCommited = new TextField.EditCommitedCallback(this.Commit);
			this.editField.EditCanceled = new TextField.EditingCancelledCallback(this.Cancel);
			this.editField.ValidateText = new TextField.ValidateCallback(this.ValidateVariableName);
			this.editField.Text = variableName;
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
				EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(this.editField.Text));
				if (GUILayout.Button(Strings.get_Label_Create_Variable(), new GUILayoutOption[0]))
				{
					this.editField.CommitEdit();
				}
				EditorGUI.EndDisabledGroup();
			}
			if (Event.get_current().get_type() == 7)
			{
				Color labelTextColor = SkillEditorStyles.LabelTextColor;
				labelTextColor.a = 0.5f;
				Handles.set_color(labelTextColor);
				Handles.DrawPolyLine(this.framePoints);
			}
		}
		private void Commit(TextField textField)
		{
			base.Close();
		}
		private void Cancel(TextField textField)
		{
			base.Close();
		}
		private bool ValidateVariableName(string variableName)
		{
			this.validationError = "";
			if (string.IsNullOrEmpty(variableName))
			{
				return false;
			}
			if (this.fsmVariables.Contains(variableName))
			{
				this.validationError = Strings.get_Error_Name_already_exists();
				return false;
			}
			return true;
		}
	}
}
