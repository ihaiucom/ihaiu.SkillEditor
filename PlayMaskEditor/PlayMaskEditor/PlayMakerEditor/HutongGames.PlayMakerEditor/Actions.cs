using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public static class Actions
	{
		private static readonly List<Type> actionsList = new List<Type>();
		private static readonly List<string> actionCategoryLookup = new List<string>();
		private static readonly List<string> actionCategoryList = new List<string>();
		private static readonly Dictionary<object, string> tooltipLookup = new Dictionary<object, string>();
		private static readonly Dictionary<Type, List<SkillInfo>> actionUsageLookup = new Dictionary<Type, List<SkillInfo>>();
		private static readonly Dictionary<Type, int> actionUsageCount = new Dictionary<Type, int>();
		private static readonly Dictionary<Type, List<string>> actionCategories = new Dictionary<Type, List<string>>();
		private static readonly Dictionary<string, List<Type>> actionsInCategory = new Dictionary<string, List<Type>>();
		private static Dictionary<string, List<Type>> actionsInCategoryFiltered;
		private static readonly List<string> filteredCategories = new List<string>();
		public static List<Type> List
		{
			get
			{
				Actions.BuildListIfNeeded();
				return Actions.actionsList;
			}
		}
		public static List<string> CategoryLookup
		{
			get
			{
				return Actions.actionCategoryLookup;
			}
		}
		public static List<string> Categories
		{
			get
			{
				Actions.BuildListIfNeeded();
				return Actions.actionCategoryList;
			}
		}
		public static List<string> FilteredCategories
		{
			get
			{
				Actions.BuildListIfNeeded();
				return Actions.filteredCategories;
			}
		}
		public static List<SkillInfo> GetUsage(Type actionType)
		{
			List<SkillInfo> result;
			Actions.actionUsageLookup.TryGetValue(actionType, ref result);
			return result;
		}
		public static int GetUsageCount(Type actionType)
		{
			int result;
			Actions.actionUsageCount.TryGetValue(actionType, ref result);
			return result;
		}
		public static void BuildListIfNeeded()
		{
			if (Actions.actionsList == null || Actions.actionsList.get_Count() == 0)
			{
				Actions.BuildList();
			}
		}
		public static void BuildList()
		{
			Actions.actionsList.Clear();
			Assembly[] assemblies = AppDomain.get_CurrentDomain().GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				try
				{
					Type[] exportedTypes = assembly.GetExportedTypes();
					Type[] array = exportedTypes;
					for (int j = 0; j < array.Length; j++)
					{
						Type type = array[j];
						if (typeof(ISkillStateAction).IsAssignableFrom(type) && type.get_IsClass() && !type.get_IsAbstract() && (!FsmEditorSettings.HideObsoleteActions || (FsmEditorSettings.HideObsoleteActions && !CustomAttributeHelpers.IsObsolete(type))))
						{
							Actions.actionsList.Add(type);
						}
					}
				}
				catch (Exception ex)
				{
					NotSupportedException arg_90_0 = ex as NotSupportedException;
				}
			}
			Actions.BuildCategoryList();
			Actions.InitCategories();
			Actions.UpdateUsage();
			ActionScripts.Init();
			ActionTargets.Init();
		}
		private static void BuildCategoryList()
		{
			Actions.actionCategoryLookup.Clear();
			Actions.actionCategoryList.Clear();
			Actions.actionCategories.Clear();
			using (List<Type>.Enumerator enumerator = Actions.actionsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					string actionCategory = Actions.GetActionCategory(current);
					Actions.actionCategoryLookup.Add(actionCategory);
					Dictionary<Type, List<string>> arg_59_0 = Actions.actionCategories;
					Type arg_59_1 = current;
					List<string> list = new List<string>();
					list.Add(actionCategory);
					arg_59_0.Add(arg_59_1, list);
					if (!Actions.actionCategoryList.Contains(actionCategory))
					{
						Actions.actionCategoryList.Add(actionCategory);
					}
				}
			}
			Actions.actionCategoryList.Sort();
			Actions.actionCategoryList.Insert(0, "Favorites");
			Actions.actionCategoryList.Insert(0, "Recent");
		}
		public static void InitCategories()
		{
			Actions.LoadCategory("Recent");
			Actions.LoadCategory("Favorites");
			for (int i = 0; i < Actions.actionsList.get_Count(); i++)
			{
				Type type = Actions.actionsList.get_Item(i);
				string text = Actions.CategoryLookup.get_Item(i);
				List<Type> list;
				if (!Actions.actionsInCategory.TryGetValue(text, ref list))
				{
					Dictionary<string, List<Type>> arg_56_0 = Actions.actionsInCategory;
					string arg_56_1 = text;
					List<Type> list2 = new List<Type>();
					list2.Add(type);
					arg_56_0.Add(arg_56_1, list2);
				}
				else
				{
					if (!list.Contains(type))
					{
						list.Add(type);
					}
				}
			}
		}
		public static List<Type> GetActionsInCategory(string categoryName)
		{
			List<Type> result;
			if (!Actions.actionsInCategory.TryGetValue(categoryName, ref result))
			{
				return new List<Type>();
			}
			return result;
		}
		public static List<Type> GetActionsInCategoryFiltered(string categoryName)
		{
			List<Type> result;
			if (!Actions.actionsInCategoryFiltered.TryGetValue(categoryName, ref result))
			{
				return null;
			}
			return result;
		}
		public static void InsertActionAtTopOfCategory(string categoryName, Type actionType)
		{
			Actions.AddActionToCategory(categoryName, actionType, 0);
		}
		public static void AddActionToCategory(string categoryName, Type actionType, int atIndex = -1)
		{
			if (actionType == null)
			{
				return;
			}
			List<Type> list;
			if (!Actions.actionsInCategory.TryGetValue(categoryName, ref list))
			{
				Dictionary<string, List<Type>> arg_27_0 = Actions.actionsInCategory;
				List<Type> list2 = new List<Type>();
				list2.Add(actionType);
				arg_27_0.Add(categoryName, list2);
			}
			else
			{
				if (atIndex == -1)
				{
					if (!list.Contains(actionType))
					{
						list.Add(actionType);
					}
				}
				else
				{
					list.Remove(actionType);
					list.Insert(atIndex, actionType);
				}
			}
			Actions.actionCategories.get_Item(actionType).Add(categoryName);
			Actions.SaveCategory(categoryName);
		}
		public static void RemoveActionFromCategory(string categoryName, Type actionType)
		{
			if (actionType == null)
			{
				return;
			}
			List<Type> list;
			if (Actions.actionsInCategory.TryGetValue(categoryName, ref list) && list != null)
			{
				list.Remove(actionType);
			}
			Actions.actionCategories.get_Item(actionType).Remove(categoryName);
			Actions.SaveCategory(categoryName);
		}
		public static bool CategoryContainsAction(string categoryName, Type actionType)
		{
			List<Type> list;
			return !string.IsNullOrEmpty(categoryName) && actionType != null && Actions.actionsInCategory.TryGetValue(categoryName, ref list) && list != null && list.Contains(actionType);
		}
		public static void SaveCategories()
		{
			Actions.SaveCategory("Favorites");
			Actions.SaveCategory("Recent");
		}
		private static void LoadCategory(string categoryName)
		{
			List<Type> list = new List<Type>();
			string @string = EditorPrefs.GetString("PlayMaker.ActionCategory." + categoryName, "");
			string[] array = @string.Split(new char[]
			{
				'\n'
			});
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string actionName = array2[i];
				Type type = Actions.FindByName(actionName);
				if (type != null && !list.Contains(type))
				{
					list.Add(type);
					Actions.actionCategories.get_Item(type).Add(categoryName);
				}
			}
			Actions.actionsInCategory.Remove(categoryName);
			Actions.actionsInCategory.Add(categoryName, list);
		}
		private static void SaveCategory(string categoryName)
		{
			List<Type> list;
			if (!Actions.actionsInCategory.TryGetValue(categoryName, ref list))
			{
				Debug.Log("Could not save action category: " + categoryName);
				return;
			}
			string text = "";
			using (List<Type>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					if (current != null)
					{
						text = text + current.get_FullName() + '\n';
					}
				}
			}
			EditorPrefs.SetString("PlayMaker.ActionCategory." + categoryName, text);
		}
		public static void UpdateUsage()
		{
			Actions.actionUsageLookup.Clear();
			Actions.actionUsageCount.Clear();
			using (List<Type>.Enumerator enumerator = Actions.actionsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					List<SkillInfo> list = SkillInfo.FindActionUsage(current);
					if (list != null)
					{
						Actions.actionUsageLookup.Add(current, list);
						Actions.actionUsageCount.Add(current, list.get_Count());
					}
				}
			}
		}
		public static Type FindByName(string actionName)
		{
			using (List<Type>.Enumerator enumerator = Actions.actionsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					if (current.get_FullName() == actionName)
					{
						Type result = current;
						return result;
					}
				}
			}
			actionName = "HutongGames.PlayMaker.Actions." + actionName;
			using (List<Type>.Enumerator enumerator2 = Actions.actionsList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Type current2 = enumerator2.get_Current();
					if (current2.get_FullName() == actionName)
					{
						Type result = current2;
						return result;
					}
				}
			}
			return null;
		}
		public static string GetCategory(Type actionType)
		{
			return Actions.CategoryLookup.get_Item(Actions.List.IndexOf(actionType));
		}
		public static string FindFirstCategory(Type actionType)
		{
			using (List<string>.Enumerator enumerator = Actions.Categories.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.get_Current();
					List<Type> list = Actions.GetActionsInCategory(current);
					if (list.Contains(actionType))
					{
						return current;
					}
				}
			}
			return null;
		}
		public static int GetCategoryIndex(string categoryName)
		{
			return Actions.Categories.FindIndex((string x) => x == categoryName);
		}
		public static List<string> GetCategories(Type actionType)
		{
			return Actions.actionCategories.get_Item(actionType);
		}
		public static int GetActionIndex(SkillState state, SkillStateAction action)
		{
			int num = 0;
			SkillStateAction[] actions = state.get_Actions();
			for (int i = 0; i < actions.Length; i++)
			{
				SkillStateAction fsmStateAction = actions[i];
				if (fsmStateAction == action)
				{
					return num;
				}
				num++;
			}
			return -1;
		}
		public static string GetTooltip(SkillStateAction action)
		{
			string result;
			if (Actions.tooltipLookup.TryGetValue(action, ref result))
			{
				return result;
			}
			return Actions.UpdateTooltip(action);
		}
		public static string UpdateTooltip(SkillStateAction action)
		{
			Actions.tooltipLookup.Remove(action);
			string text = Actions.GetTooltip(action);
			text = string.Format(Strings.get_Tooltip_Action(), Labels.NicifyVariableName(Labels.StripNamespace(action.GetType().ToString())), text);
			Actions.tooltipLookup.Add(action, text);
			return text;
		}
		public static string GetTooltip(object instance)
		{
			if (instance == null)
			{
				return null;
			}
			return Actions.GetTooltip(instance.GetType());
		}
		public static string GetTooltip(Type objType)
		{
			if (objType == null)
			{
				return null;
			}
			return Actions.GetTooltip(CustomAttributeHelpers.GetCustomAttributes(objType));
		}
		public static string GetTooltip(FieldInfo field)
		{
			if (field == null)
			{
				return null;
			}
			return Actions.GetTooltip(CustomAttributeHelpers.GetCustomAttributes(field));
		}
		public static string GetTooltip(object[] attributes)
		{
			for (int i = 0; i < attributes.Length; i++)
			{
				object obj = attributes[i];
				TooltipAttribute tooltipAttribute = obj as TooltipAttribute;
				if (tooltipAttribute != null)
				{
					return tooltipAttribute.get_Text();
				}
			}
			return string.Empty;
		}
		public static string GetActionCategory(Type objType)
		{
			if (objType == null)
			{
				return null;
			}
			return Actions.GetActionCategory(CustomAttributeHelpers.GetCustomAttributes(objType));
		}
		public static string GetActionCategory(object[] attributes)
		{
			for (int i = 0; i < attributes.Length; i++)
			{
				object obj = attributes[i];
				ActionCategoryAttribute actionCategoryAttribute = obj as ActionCategoryAttribute;
				if (actionCategoryAttribute != null)
				{
					return actionCategoryAttribute.get_Category();
				}
			}
			return Strings.get_Label_Misc();
		}
		public static void FilterActions(string searchString, int searchMode)
		{
			string[] filter = searchString.ToUpper().Split(new char[]
			{
				' '
			});
			Actions.actionsInCategoryFiltered = Enumerable.ToDictionary<KeyValuePair<string, List<Type>>, string, List<Type>>(Actions.actionsInCategory, (KeyValuePair<string, List<Type>> x) => x.get_Key(), (KeyValuePair<string, List<Type>> x) => new List<Type>(x.get_Value()));
			using (List<Type>.Enumerator enumerator = Actions.actionsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					if (!Actions.ActionMatchesFilter(current, filter, searchMode))
					{
						List<string> categories = Actions.GetCategories(current);
						using (List<string>.Enumerator enumerator2 = categories.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								string current2 = enumerator2.get_Current();
								Actions.actionsInCategoryFiltered.get_Item(current2).Remove(current);
							}
						}
					}
				}
			}
			Actions.filteredCategories.Clear();
			using (List<string>.Enumerator enumerator3 = Actions.actionCategoryList.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					string current3 = enumerator3.get_Current();
					if (Actions.actionsInCategoryFiltered.get_Item(current3).get_Count() > 0)
					{
						Actions.filteredCategories.Add(current3);
					}
				}
			}
		}
		private static bool ActionMatchesFilter(Type actionType, IEnumerable<string> filter, int searchMode)
		{
			filter = (filter as string[]);
			bool flag = Actions.ActionNameMatchesFilter(actionType, filter);
			if (flag || searchMode == 0)
			{
				return flag;
			}
			return Actions.ActionDescriptionMatchesFilter(actionType, filter);
		}
		private static bool ActionNameMatchesFilter(Type actionType, IEnumerable<string> filter)
		{
			string text = Labels.GetActionLabel(actionType).ToUpper().Replace(" ", "");
			using (IEnumerator<string> enumerator = filter.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.get_Current();
					if (!text.Contains(current))
					{
						return false;
					}
				}
			}
			return true;
		}
		private static bool ActionDescriptionMatchesFilter(Type actionType, IEnumerable<string> filter)
		{
			string text = Actions.GetTooltip(actionType).ToUpper();
			using (IEnumerator<string> enumerator = filter.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.get_Current();
					if (!text.Contains(current))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
