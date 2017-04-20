using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.Editor
{
	[Localizable(false)]
	public class SearchBox
	{
		public EditorApplication.CallbackFunction SearchChanged;
		private EditorWindow window;
		private string saveKey;
		private bool focusSearchBox;
		private bool searchBoxHasFocus;
		private string searchString = "";
		private int searchMode;
		private bool searchChanged;
		private GUIContent[] searchModesMenuItems;
		private string[] searchModes = new string[]
		{
			""
		};
		private Rect searchModePopupButton = default(Rect);
		private GUIStyle searchFieldStyle;
		private GUIStyle searchFieldCancelButtonStyle;
		private GUIStyle searchFieldCancelButtonEmptyStyle;
		public string SearchString
		{
			get
			{
				return this.searchString;
			}
		}
		public bool HasPopupSearchModes
		{
			get;
			set;
		}
		public string[] SearchModes
		{
			get
			{
				return this.searchModes;
			}
			set
			{
				this.searchModes = value;
			}
		}
		public int SearchMode
		{
			get
			{
				return this.searchMode;
			}
			set
			{
				this.SelectSearchMode(null, null, value);
			}
		}
		public SearchBox(EditorWindow window)
		{
			this.window = window;
			this.saveKey = window.GetType().ToString();
			this.SetSearchFilter(EditorPrefs.GetString(this.saveKey + ".SearchString", ""));
			this.SearchMode = EditorPrefs.GetInt(this.saveKey + ".SearchMode", 0);
		}
		public void Clear()
		{
			this.SetSearchFilter("");
		}
		public void Focus()
		{
			this.focusSearchBox = true;
		}
		public void OnGUI()
		{
			this.InitStyles();
			this.DoSearchBox(GUILayoutUtility.GetRect(1f, 200f, 16f, 16f, this.searchFieldStyle));
			if (this.searchChanged && Event.get_current().get_type() == 8)
			{
				if (this.SearchChanged != null)
				{
					this.SearchChanged.Invoke();
				}
				this.searchChanged = false;
			}
		}
		private void DoSearchBox(Rect position)
		{
			if (Event.get_current().get_type() == null && position.Contains(Event.get_current().get_mousePosition()))
			{
				this.focusSearchBox = true;
			}
			if (Event.get_current().get_type() == 13 && Event.get_current().get_commandName() == "Find")
			{
				Event.get_current().Use();
				return;
			}
			if (Event.get_current().get_type() == 14 && Event.get_current().get_commandName() == "Find")
			{
				this.focusSearchBox = true;
			}
			GUI.SetNextControlName("SearchField");
			if (this.focusSearchBox)
			{
				EditorHacks.FocusTextInControl("SearchField");
				if (Event.get_current().get_type() == 7)
				{
					this.focusSearchBox = false;
				}
			}
			this.searchBoxHasFocus = (GUI.GetNameOfFocusedControl() == "SearchField");
			if (Event.get_current().get_type() == 4 && Event.get_current().get_keyCode() == 27)
			{
				if (this.searchBoxHasFocus)
				{
					this.SetSearchFilter(string.Empty);
					GUIUtility.set_keyboardControl(0);
					Event.get_current().Use();
				}
				else
				{
					if (string.IsNullOrEmpty(this.searchString))
					{
						this.searchMode = 0;
					}
					this.SetSearchFilter(string.Empty);
				}
			}
			string text = this.ToolbarSearchField(position, this.searchString);
			if (this.searchString != text)
			{
				this.SetSearchFilter(text);
				this.window.Repaint();
			}
		}
		private string ToolbarSearchField(Rect position, string text)
		{
			if (this.HasPopupSearchModes && this.DoSearchModePopup(position))
			{
				this.DoSearchModeMenu(position);
			}
			Rect rect = position;
			rect.set_width(rect.get_width() - 14f);
			text = EditorGUI.TextField(rect, text, this.searchFieldStyle);
			bool flag = text == string.Empty;
			if (flag && (!this.searchBoxHasFocus || EditorWindow.get_focusedWindow() != this.window || this.searchMode != 0) && Event.get_current().get_type() == 7)
			{
				EditorGUI.BeginDisabledGroup(true);
				Color backgroundColor = GUI.get_backgroundColor();
				GUI.set_backgroundColor(Color.get_clear());
				if (!this.searchBoxHasFocus || EditorWindow.get_focusedWindow() != this.window)
				{
					this.searchFieldStyle.Draw(rect, this.searchModes[this.searchMode], false, false, false, false);
				}
				else
				{
					if (this.searchMode > 0)
					{
						this.searchFieldStyle.set_alignment(2);
						this.searchFieldStyle.Draw(rect, this.searchModes[this.searchMode] + '\u00a0', false, false, false, false);
						this.searchFieldStyle.set_alignment(0);
					}
				}
				EditorGUI.EndDisabledGroup();
				GUI.set_backgroundColor(backgroundColor);
			}
			rect = position;
			rect.set_x(rect.get_x() + (position.get_width() - 14f));
			rect.set_width(14f);
			if (!flag || this.searchMode != 0)
			{
				if (GUI.Button(rect, GUIContent.none, this.searchFieldCancelButtonStyle))
				{
					if (flag)
					{
						this.searchMode = 0;
					}
					else
					{
						text = string.Empty;
						GUIUtility.set_keyboardControl(0);
					}
				}
			}
			else
			{
				if (GUI.Button(rect, GUIContent.none, this.searchFieldCancelButtonEmptyStyle))
				{
					this.focusSearchBox = true;
				}
			}
			return text;
		}
		private void InitStyles()
		{
			if (this.searchFieldStyle == null || this.searchFieldCancelButtonStyle == null || this.searchFieldCancelButtonEmptyStyle == null)
			{
				this.searchFieldStyle = (this.HasPopupSearchModes ? "ToolbarSeachTextFieldPopup" : "ToolbarSeachTextField");
				this.searchFieldCancelButtonStyle = "ToolbarSeachCancelButton";
				this.searchFieldCancelButtonEmptyStyle = "ToolbarSeachCancelButtonEmpty";
			}
		}
		private bool DoSearchModePopup(Rect position)
		{
			this.searchModePopupButton.Set(position.get_xMin(), position.get_yMin(), 20f, position.get_height());
			return this.searchModes.Length > 0 && Event.get_current().get_type() == null && this.searchModePopupButton.Contains(Event.get_current().get_mousePosition());
		}
		private void DoSearchModeMenu(Rect position)
		{
			if (this.searchModesMenuItems == null)
			{
				this.searchModesMenuItems = new GUIContent[this.searchModes.Length];
				for (int i = 0; i < this.searchModes.Length; i++)
				{
					this.searchModesMenuItems[i] = new GUIContent(this.searchModes[i]);
				}
			}
			EditorUtility.DisplayCustomMenu(position, this.searchModesMenuItems, this.searchMode, new EditorUtility.SelectMenuItemFunction(this.SelectSearchMode), null);
			Event.get_current().Use();
		}
		private void SelectSearchMode(object userData, string[] options, int selected)
		{
			if (this.searchMode == selected)
			{
				return;
			}
			this.searchMode = selected;
			this.searchChanged = true;
			this.window.Repaint();
			EditorPrefs.SetInt(this.saveKey + ".SearchMode", this.searchMode);
		}
		private void SetSearchFilter(string searchFilter)
		{
			if (searchFilter == this.searchString)
			{
				return;
			}
			this.searchString = searchFilter;
			this.searchBoxHasFocus = false;
			this.searchChanged = true;
			this.window.Repaint();
			EditorPrefs.SetString(this.saveKey + ".SearchString", this.searchString);
		}
	}
}
