using HutongGames.PlayMaker;
using HutongGames.PlayMakerEditor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
[Localizable(false)]
public class PlayMakerDocHelpers : BaseEditorWindow
{
	private string screenshotsSavePath;
	private string htmlSavePath;
	private string imagesUrl;
	private string[] categoryChoices;
	private int selectedCategory;
	private string selectedCategoryName;
	private SkillStateAction previewAction;
	private bool capturingGUI;
	private int actionIndex;
	private Stopwatch stopwatch;
	public void CaptureActionScreenshots()
	{
		this.StartCaptureActionScreenshots();
	}
	public override void Initialize()
	{
		base.SetTitle("PlayMaker Helpers");
		this.screenshotsSavePath = EditorPrefs.GetString("PlayMaker.DocHelpers.ScreenshotsSavePath", "PlayMaker/Screenshots/");
		this.htmlSavePath = EditorPrefs.GetString("PlayMaker.DocHelpers.HtmlSavePath", "PlayMaker/Html/");
		this.imagesUrl = EditorPrefs.GetString("PlayMaker.DocHelpers.ImagesUrl", "http://mywebsite.com/docs/img/");
		this.selectedCategory = Math.Min(EditorPrefs.GetInt("PlayMaker.DocHelpers.SelectedCategory", 0), Actions.Categories.get_Count());
		base.set_minSize(new Vector2(350f, 400f));
		base.set_maxSize(new Vector2(350f, 900f));
		List<string> list = new List<string>(Actions.Categories);
		list.Insert(0, "All Categories");
		list.Remove("Tests");
		this.categoryChoices = list.ToArray();
	}
	private void OnDisable()
	{
		this.SavePreferences();
	}
	public override void DoGUI()
	{
		SkillEditorStyles.Init();
		SkillEditorGUILayout.ToolWindowLargeTitle(this, "Doc Helpers");
		if (SkillEditor.Instance == null)
		{
			GUILayout.Label("Please open the PlayMaker Editor...", new GUILayoutOption[0]);
			return;
		}
		if (!this.capturingGUI)
		{
			EditorGUILayout.HelpBox("This tool generates the screenshots and html required to document actions in the online wiki.", 1);
			GUILayout.Label("Source", EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			this.selectedCategory = EditorGUILayout.Popup("Action Category", this.selectedCategory, this.categoryChoices, new GUILayoutOption[0]);
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			GUILayout.Label("Export Settings", EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			this.screenshotsSavePath = EditorGUILayout.TextField("Save Screenshots", this.screenshotsSavePath, new GUILayoutOption[0]);
			this.htmlSavePath = EditorGUILayout.TextField("Save Html", this.htmlSavePath, new GUILayoutOption[0]);
			this.imagesUrl = EditorGUILayout.TextField("Images Url", this.imagesUrl, new GUILayoutOption[0]);
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			if (GUILayout.Button("Capture Screenshots", new GUILayoutOption[]
			{
				GUILayout.MinHeight(30f)
			}))
			{
				this.StartCaptureActionScreenshots();
			}
			if (GUILayout.Button("Generate Wiki Html", new GUILayoutOption[]
			{
				GUILayout.MinHeight(30f)
			}))
			{
				this.GenerateActionWikiHtml();
				this.GenerateActionsEnum();
			}
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			if (GUI.get_changed())
			{
				this.SavePreferences();
			}
			GUILayout.FlexibleSpace();
			GUILayout.Label("Resize window height to fit largest action screenshot...", new GUILayoutOption[0]);
		}
		else
		{
			ActionEditor.PreviewMode = true;
			if (this.previewAction != null)
			{
				SkillEditorGUILayout.LabelWidth(150f);
				EditorGUILayout.Space();
				EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
				SkillEditor.ActionEditor.OnGUI(this.previewAction);
				EditorGUILayout.EndVertical();
				if (Event.get_current().get_type() == 7)
				{
					this.SaveActionScreenshot();
					this.NextActionScreenshot();
				}
				GUILayout.FlexibleSpace();
				if (this.capturingGUI)
				{
					EditorUtility.DisplayProgressBar("Saving Action Screenshots...", "Press Escape to cancel", (float)this.actionIndex / (float)Actions.List.get_Count());
				}
			}
		}
		ActionEditor.PreviewMode = false;
	}
	private void SavePreferences()
	{
		EditorPrefs.SetString("PlayMaker.DocHelpers.ImagesUrl", this.imagesUrl);
		EditorPrefs.SetInt("PlayMaker.DocHelpers.SelectedCategory", this.selectedCategory);
		EditorPrefs.SetString("PlayMaker.DocHelpers.ScreenshotsSavePath", this.screenshotsSavePath);
		EditorPrefs.SetString("PlayMaker.DocHelpers.HtmlSavePath", this.htmlSavePath);
	}
	private void StartCaptureActionScreenshots()
	{
		if (!Files.CreateFilePath(this.screenshotsSavePath))
		{
			return;
		}
		if (this.selectedCategory > 0)
		{
			this.selectedCategoryName = Actions.Categories.get_Item(this.selectedCategory - 1);
		}
		this.stopwatch = new Stopwatch();
		this.stopwatch.Start();
		this.capturingGUI = true;
		this.actionIndex = 0;
		this.NextActionScreenshot();
	}
	private void GenerateActionCategoriesWikiHtml()
	{
		if (this.selectedCategory == 0)
		{
			List<string> categories = Actions.Categories;
			using (List<string>.Enumerator enumerator = categories.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.get_Current();
					this.GenerateActionCategoryWikiHtml(current);
				}
			}
			return;
		}
		this.selectedCategoryName = Actions.Categories.get_Item(this.selectedCategory - 1);
		this.GenerateActionCategoryWikiHtml(this.selectedCategoryName);
	}
	private void GenerateActionCategoryWikiHtml(string actionCategory)
	{
		string text = Path.Combine(this.htmlSavePath, "Category_" + actionCategory);
		string fullPath = Path.GetFullPath(Application.get_dataPath() + "/../" + text + ".txt");
		if (!Files.CreateFilePath(fullPath))
		{
			return;
		}
		StreamWriter streamWriter = File.CreateText(fullPath);
		streamWriter.WriteLine("<p>TODO: Category description...</p>");
		streamWriter.WriteLine("<div id=\"linkList\"><ul>");
		using (List<Type>.Enumerator enumerator = Actions.List.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Type current = enumerator.get_Current();
				string actionCategory2 = Actions.GetActionCategory(current);
				if (actionCategory2 == actionCategory)
				{
					streamWriter.WriteLine("<li>" + Labels.NicifyVariableName(Labels.StripNamespace(current.ToString())) + "</li>");
				}
			}
		}
		streamWriter.WriteLine("</ul></div>");
		streamWriter.Close();
	}
	private void GenerateActionWikiHtml()
	{
		if (!Files.CreateFilePath(this.htmlSavePath))
		{
			return;
		}
		this.GenerateActionCategoriesWikiHtml();
		if (this.selectedCategory > 0)
		{
			this.selectedCategoryName = Actions.Categories.get_Item(this.selectedCategory - 1);
		}
		this.actionIndex = 0;
		while (this.actionIndex < Actions.List.get_Count())
		{
			EditorUtility.DisplayProgressBar("Saving Action Wiki Html...", "Press Escape to cancel", (float)this.actionIndex / (float)Actions.List.get_Count());
			if (Event.get_current().get_keyCode() == 27)
			{
				return;
			}
			if (this.selectedCategory > 0 && Actions.CategoryLookup.get_Item(this.actionIndex) != this.selectedCategoryName)
			{
				this.actionIndex++;
			}
			else
			{
				Type type = Actions.List.get_Item(this.actionIndex);
				string text = Labels.StripNamespace(type.ToString());
				string text2 = Path.Combine(this.htmlSavePath, text);
				string fullPath = Path.GetFullPath(Application.get_dataPath() + "/../" + text2 + ".txt");
				StreamWriter streamWriter = File.CreateText(fullPath);
				streamWriter.WriteLine("<div id=\"actionImg\"><p><img src=\"" + this.imagesUrl + text + ".png\" title=\"\" /></p></div>");
				string text3 = Actions.GetTooltip(type);
				if (string.IsNullOrEmpty(text3))
				{
					text3 = "TODO";
				}
				streamWriter.WriteLine("<div id=\"actionDesc\">\n<p>");
				streamWriter.WriteLine(text3 + "</p>\n</div>");
				FieldInfo[] fields = ActionData.GetFields(type);
				FieldInfo[] array = fields;
				for (int i = 0; i < array.Length; i++)
				{
					FieldInfo fieldInfo = array[i];
					string text4 = Actions.GetTooltip(fieldInfo);
					if (string.IsNullOrEmpty(text4))
					{
						text4 = "TODO";
					}
					streamWriter.WriteLine("<div id=\"paramRow\">");
					streamWriter.WriteLine("\t<div id=\"paramName\">");
					streamWriter.WriteLine(ObjectNames.NicifyVariableName(fieldInfo.get_Name()) + "</div>");
					streamWriter.WriteLine("\t<div id=\"paramDesc\">");
					streamWriter.WriteLine(text4 + "</div>");
					streamWriter.WriteLine("</div>");
					streamWriter.WriteLine("");
				}
				streamWriter.Close();
				this.actionIndex++;
			}
		}
		EditorUtility.ClearProgressBar();
	}
	private void GenerateActionsEnum()
	{
		StreamWriter streamWriter = File.CreateText(Path.Combine(this.htmlSavePath, "ActionsEnum.txt"));
		for (int i = 0; i < Actions.List.get_Count(); i++)
		{
			if (!(Actions.CategoryLookup.get_Item(i) == "Tests"))
			{
				Type type = Actions.List.get_Item(i);
				string text = Labels.StripNamespace(type.ToString());
				if (Enum.IsDefined(typeof(WikiPages), text))
				{
					WikiPages wikiPages = PlayMakerDocHelpers.StringToEnum<WikiPages>(text);
					streamWriter.WriteLine(string.Concat(new object[]
					{
						"\t\t\t",
						text,
						" = ",
						(int)wikiPages,
						","
					}));
				}
				else
				{
					streamWriter.WriteLine("\t\t\t" + text + " = TODO,");
				}
			}
		}
		streamWriter.Close();
	}
	private static T StringToEnum<T>(string name)
	{
		return (T)((object)Enum.Parse(typeof(T), name));
	}
	private void NextActionScreenshot()
	{
		if (this.CheckFinished())
		{
			this.FinishCapture();
			return;
		}
		if (this.selectedCategory > 0)
		{
			while (Actions.CategoryLookup.get_Item(this.actionIndex) != this.selectedCategoryName)
			{
				this.actionIndex++;
				if (this.CheckFinished())
				{
					this.FinishCapture();
					return;
				}
			}
		}
		Type type = Actions.List.get_Item(this.actionIndex);
		if (Actions.CategoryLookup.get_Item(this.actionIndex) == "Tests")
		{
			this.actionIndex++;
			this.NextActionScreenshot();
		}
		this.previewAction = (SkillStateAction)Activator.CreateInstance(type);
		this.previewAction.Reset();
		this.actionIndex++;
		base.Repaint();
	}
	private bool CheckFinished()
	{
		return this.actionIndex >= Actions.List.get_Count() || (Event.get_current() != null && Event.get_current().get_isKey() && Event.get_current().get_keyCode() == 27);
	}
	private void FinishCapture()
	{
		EditorUtility.ClearProgressBar();
		this.capturingGUI = false;
		this.stopwatch.Stop();
		Debug.Log("Captured action screenshots (" + this.stopwatch.get_Elapsed() + ")");
	}
	private void SaveActionScreenshot()
	{
		this.SaveScreenshot(Labels.StripNamespace(this.previewAction.ToString()), GUILayoutUtility.GetLastRect());
	}
	private void SaveScreenshot(string actionName, Rect region)
	{
		if (region.get_height() < 1f)
		{
			return;
		}
		region.set_y(base.get_position().get_height() - region.get_height() - 55f);
		region.set_height(region.get_height() + 10f);
		Texture2D texture2D = new Texture2D((int)region.get_width(), (int)region.get_height(), 3, false);
		texture2D.ReadPixels(region, 0, 0);
		texture2D.Apply();
		string text = Path.Combine(this.screenshotsSavePath, actionName);
		string fullPath = Path.GetFullPath(Application.get_dataPath() + "/../" + text + ".png");
		if (!Files.CreateFilePath(fullPath))
		{
			return;
		}
		byte[] array = texture2D.EncodeToPNG();
		Object.DestroyImmediate(texture2D, true);
		File.WriteAllBytes(fullPath, array);
	}
}
