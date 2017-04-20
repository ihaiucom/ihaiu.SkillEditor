using HutongGames.Editor;
using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	public class PopupListWindow : EditorWindow
	{
		private TextField editField;
		private Vector2 scrollPosition;
		private float scrollViewHeight;
		private Rect selectedRect;
		private bool autoScroll;
		private int selectedIndex = -1;
		public string SelectedText
		{
			get
			{
				if (this.selectedIndex == -1 || this.ListItems.Length == 0)
				{
					return "";
				}
				return this.ListItems[Mathf.Min(this.selectedIndex, this.ListItems.Length - 1)].get_text();
			}
			set
			{
				this.selectedIndex = -1;
				for (int i = 0; i < this.ListItems.Length; i++)
				{
					if (this.ListItems[i].get_text() == value)
					{
						this.selectedIndex = i;
					}
				}
			}
		}
		public GUIContent[] ListItems
		{
			get;
			set;
		}
		public static PopupListWindow CreateWindow(Rect buttonRect, Vector2 windowSize)
		{
			PopupListWindow popupListWindow = ScriptableObject.CreateInstance<PopupListWindow>();
			popupListWindow.ShowAsDropDown(buttonRect, windowSize);
			popupListWindow.Init();
			return popupListWindow;
		}
		private void Init()
		{
			this.scrollPosition = default(Vector2);
			this.editField = new TextField(this, GUIContent.none, "");
			this.editField.Focus();
		}
		private void OnGUI()
		{
			if (!SkillEditorStyles.IsInitialized())
			{
				SkillEditorStyles.Init();
			}
			this.editField.OnGUI(new GUILayoutOption[0]);
			this.DoKeyboardGUI();
			this.DoListView();
		}
		private void DoKeyboardGUI()
		{
			if (GUIUtility.get_keyboardControl() != 0)
			{
				return;
			}
			int controlID = GUIUtility.GetControlID(1);
			if (Event.get_current().GetTypeForControl(controlID) == 4)
			{
				KeyCode keyCode = Event.get_current().get_keyCode();
				if (keyCode != 27)
				{
					switch (keyCode)
					{
					case 273:
						Event.get_current().Use();
						this.SelectPrevious();
						GUIUtility.ExitGUI();
						return;
					case 274:
						Event.get_current().Use();
						this.SelectNext();
						GUIUtility.ExitGUI();
						return;
					default:
						return;
					}
				}
				else
				{
					Event.get_current().Use();
					int arg_7D_0 = GUIUtility.get_keyboardControl();
					GUIUtility.ExitGUI();
				}
			}
		}
		private void DoListView()
		{
			if (this.ListItems == null || this.ListItems.Length == 0)
			{
				return;
			}
			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]);
			GUIContent[] listItems = this.ListItems;
			for (int i = 0; i < listItems.Length; i++)
			{
				GUIContent gUIContent = listItems[i];
				bool flag = gUIContent.get_text() == this.SelectedText;
				GUILayout.BeginHorizontal(flag ? SkillEditorStyles.SelectedEventBox : SkillEditorStyles.TableRowBoxNoDivider, new GUILayoutOption[0]);
				GUIStyle gUIStyle = flag ? SkillEditorStyles.TableRowTextSelected : SkillEditorStyles.TableRowText;
				GUILayout.Label(gUIContent.get_text(), gUIStyle, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				if (flag)
				{
					this.selectedRect = GUILayoutUtility.GetLastRect();
					this.selectedRect.set_y(this.selectedRect.get_y() - this.scrollPosition.y);
					this.selectedRect.set_y(this.selectedRect.get_y() + 20f);
				}
			}
			EditorGUILayout.EndScrollView();
			this.DoAutoScroll();
		}
		private void DoAutoScroll()
		{
			if (string.IsNullOrEmpty(this.SelectedText))
			{
				return;
			}
			if (Event.get_current().get_type() == 7 && this.autoScroll)
			{
				this.scrollViewHeight = GUILayoutUtility.GetLastRect().get_height();
				if (this.selectedRect.get_y() < 0f)
				{
					this.scrollPosition.y = this.scrollPosition.y + this.selectedRect.get_y();
					base.Repaint();
				}
				else
				{
					if (this.selectedRect.get_y() + this.selectedRect.get_height() > this.scrollViewHeight)
					{
						this.scrollPosition.y = this.scrollPosition.y + (this.selectedRect.get_y() + this.selectedRect.get_height() - this.scrollViewHeight);
						base.Repaint();
					}
				}
				this.autoScroll = false;
			}
		}
		private void SelectListItem(string listItem)
		{
			this.SelectedText = listItem;
			this.autoScroll = true;
		}
		private void SelectListItem(int listItemIndex)
		{
			this.selectedIndex = listItemIndex;
			this.autoScroll = true;
		}
		private void SelectPrevious()
		{
			if (this.selectedIndex == -1)
			{
				this.SelectFirst();
				return;
			}
			this.SelectListItem(this.selectedIndex - 1);
		}
		private void SelectNext()
		{
			if (this.selectedIndex == -1)
			{
				this.SelectLast();
				return;
			}
			this.SelectListItem(this.selectedIndex + 1);
		}
		private void SelectFirst()
		{
			this.SelectListItem(0);
		}
		private void SelectLast()
		{
			this.SelectListItem(this.ListItems.Length - 1);
		}
	}
}
