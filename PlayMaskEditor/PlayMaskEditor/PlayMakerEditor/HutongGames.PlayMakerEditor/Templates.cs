using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public static class Templates
	{
		private static List<SkillTemplate> list;
		private static List<string> categories;
		public static readonly Dictionary<SkillTemplate, string> CategoryLookup = new Dictionary<SkillTemplate, string>();
		public static List<SkillTemplate> List
		{
			get
			{
				if (Templates.list == null)
				{
					Templates.InitList();
				}
				return Templates.list;
			}
		}
		public static List<string> Categories
		{
			get
			{
				if (Templates.categories == null)
				{
					Templates.categories = new List<string>();
					Templates.InitList();
				}
				return Templates.categories;
			}
		}
		public static void LoadAll()
		{
			Files.LoadAllAssetsOfType("FsmTemplate");
			Templates.InitList();
		}
		public static void InitList()
		{
			Templates.list = new List<SkillTemplate>();
			SkillTemplate[] array = (SkillTemplate[])Resources.FindObjectsOfTypeAll(typeof(SkillTemplate));
			Templates.list.AddRange(array);
			Templates.SortList();
			Templates.CategorizeTemplates();
		}
		public static void SortList()
		{
			Templates.list.Sort((SkillTemplate x, SkillTemplate y) => string.Compare(x.get_Category() + x.get_name(), y.get_Category() + y.get_name(), 4));
		}
		private static void CategorizeTemplates()
		{
			Templates.CategoryLookup.Clear();
			Templates.Categories.Clear();
			using (List<SkillTemplate>.Enumerator enumerator = Templates.List.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillTemplate current = enumerator.get_Current();
					if (!string.IsNullOrEmpty(current.get_name()))
					{
						Templates.AddTemplateToCategory(current, current.get_Category());
					}
				}
			}
		}
		private static void AddTemplateToCategory(SkillTemplate template, string category)
		{
			Templates.CategoryLookup.Remove(template);
			Templates.CategoryLookup.Add(template, category);
			if (!Templates.Categories.Contains(category))
			{
				Templates.Categories.Add(category);
			}
		}
		public static void DoSelectTemplateMenu(SkillTemplate SelectedTemplate, GenericMenu.MenuFunction ClearTemplate, GenericMenu.MenuFunction2 SelectTemplate)
		{
			Templates.LoadAll();
			List<string> list = new List<string>();
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_None()), false, ClearTemplate);
			using (List<SkillTemplate>.Enumerator enumerator = Templates.List.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillTemplate current = enumerator.get_Current();
					string text = current.get_Category() + "/" + current.get_name();
					string text2 = Labels.GenerateUniqueLabelWithNumber(list, text);
					list.Add(text);
					genericMenu.AddItem(new GUIContent(text2), SelectedTemplate == current, SelectTemplate, current);
				}
			}
			genericMenu.ShowAsContext();
		}
	}
}
