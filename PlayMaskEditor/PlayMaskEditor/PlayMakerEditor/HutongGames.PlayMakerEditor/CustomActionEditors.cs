using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	internal class CustomActionEditors
	{
		private static Dictionary<Type, Type> editorsLookup;
		private static Dictionary<SkillStateAction, CustomActionEditor> customEditors = new Dictionary<SkillStateAction, CustomActionEditor>();
		public static List<string> ActionsWithCustomEditors()
		{
			if (CustomActionEditors.editorsLookup == null)
			{
				CustomActionEditors.Rebuild();
			}
			List<string> list = new List<string>();
			using (Dictionary<Type, Type>.KeyCollection.Enumerator enumerator = CustomActionEditors.editorsLookup.get_Keys().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					list.Add(current.get_Name());
				}
			}
			return list;
		}
		public static int InstanceCount()
		{
			return CustomActionEditors.customEditors.get_Count();
		}
		public static bool HasCustomEditor(Type action)
		{
			if (CustomActionEditors.editorsLookup == null)
			{
				CustomActionEditors.Rebuild();
			}
			return CustomActionEditors.editorsLookup.ContainsKey(action);
		}
		private static Type GetCustomEditor(Type action)
		{
			if (CustomActionEditors.editorsLookup == null)
			{
				CustomActionEditors.Rebuild();
			}
			Type result;
			CustomActionEditors.editorsLookup.TryGetValue(action, ref result);
			return result;
		}
		public static CustomActionEditor GetCustomEditor(SkillStateAction action)
		{
			CustomActionEditor customActionEditor;
			CustomActionEditors.customEditors.TryGetValue(action, ref customActionEditor);
			if (customActionEditor == null)
			{
				Type customEditor = CustomActionEditors.GetCustomEditor(action.GetType());
				if (customEditor != null)
				{
					customActionEditor = (CustomActionEditor)Activator.CreateInstance(customEditor);
					customActionEditor.target = action;
					customActionEditor.OnEnable();
					CustomActionEditors.customEditors.Add(action, customActionEditor);
				}
				else
				{
					Debug.LogError("Could not get Custom Action Editor for: " + action.GetType());
				}
			}
			return customActionEditor;
		}
		public static void ClearCache()
		{
			List<CustomActionEditor> allCustomActionEditors = CustomActionEditors.GetAllCustomActionEditors();
			using (List<CustomActionEditor>.Enumerator enumerator = allCustomActionEditors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CustomActionEditor current = enumerator.get_Current();
					current.OnDisable();
				}
			}
			CustomActionEditors.customEditors.Clear();
		}
		private static void Clear()
		{
			CustomActionEditors.editorsLookup = new Dictionary<Type, Type>();
		}
		public static List<CustomActionEditor> GetAllCustomActionEditors()
		{
			List<CustomActionEditor> list = new List<CustomActionEditor>();
			using (Dictionary<SkillStateAction, CustomActionEditor>.Enumerator enumerator = CustomActionEditors.customEditors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<SkillStateAction, CustomActionEditor> current = enumerator.get_Current();
					if (current.get_Value() != null && !list.Contains(current.get_Value()))
					{
						list.Add(current.get_Value());
					}
				}
			}
			return list;
		}
		public static void Rebuild()
		{
			CustomActionEditors.Clear();
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
						if (typeof(CustomActionEditor).IsAssignableFrom(type) && type.get_IsClass() && !type.get_IsAbstract())
						{
							CustomActionEditorAttribute attribute = CustomAttributeHelpers.GetAttribute<CustomActionEditorAttribute>(type);
							if (attribute != null)
							{
								CustomActionEditors.editorsLookup.Add(attribute.InspectedType, type);
							}
						}
					}
				}
				catch (Exception ex)
				{
					NotSupportedException arg_87_0 = ex as NotSupportedException;
				}
			}
		}
	}
}
