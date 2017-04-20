using HutongGames.Editor;
using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	public class SearchWindow : BaseEditorWindow
	{
		private SearchBox searchBox;
		private string searchString = "";
		public void InitWindowTitle()
		{
			base.SetTitle("FSM Search");
		}
		public override void Initialize()
		{
			this.isToolWindow = true;
			this.InitWindowTitle();
			base.set_minSize(new Vector2(200f, 100f));
			this.InitSearchBox();
			base.Repaint();
		}
		private void InitSearchBox()
		{
			if (this.searchBox == null)
			{
				this.searchBox = new SearchBox(this);
				SearchBox expr_1A = this.searchBox;
				expr_1A.SearchChanged = (EditorApplication.CallbackFunction)Delegate.Combine(expr_1A.SearchChanged, new EditorApplication.CallbackFunction(this.UpdateSearchResults));
				this.searchBox.Focus();
			}
		}
		private void UpdateSearchResults()
		{
			Debug.Log("Search!");
			this.searchString = this.searchBox.SearchString;
			string.IsNullOrEmpty(this.searchString);
		}
		public override void DoGUI()
		{
			this.DoMainToolbar();
		}
		private void DoMainToolbar()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.get_toolbar(), new GUILayoutOption[0]);
			this.searchBox.OnGUI();
			SkillEditorGUILayout.ToolbarSettingsButton();
			GUILayout.Space(-5f);
			EditorGUILayout.EndHorizontal();
		}
	}
}
