using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public static class ActionScripts
	{
		private static readonly Dictionary<Type, Object> actionScriptLookup = new Dictionary<Type, Object>();
		public static Dictionary<Type, Object> ActionScriptLookup
		{
			get
			{
				return ActionScripts.actionScriptLookup;
			}
		}
		public static void Init()
		{
			ActionScripts.actionScriptLookup.Clear();
			List<Type> list = new List<Type>(Actions.List);
			MonoScript[] array = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));
			MonoScript[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				MonoScript monoScript = array2[i];
				string text = Labels.NicifyVariableName(monoScript.get_name());
				Type type = null;
				using (List<Type>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Type current = enumerator.get_Current();
						if (text == Labels.GetActionLabel(current))
						{
							if (!ActionScripts.actionScriptLookup.ContainsKey(current))
							{
								ActionScripts.actionScriptLookup.Add(current, monoScript);
							}
							type = current;
						}
					}
				}
				if (type != null)
				{
					list.Remove(type);
					if (list.get_Count() == 0)
					{
						return;
					}
				}
			}
		}
		public static void PingAsset(object userdata)
		{
			SkillStateAction fsmStateAction = (SkillStateAction)userdata;
			Object asset = ActionScripts.GetAsset(fsmStateAction);
			if (asset != null)
			{
				EditorGUIUtility.PingObject(asset);
				return;
			}
			Debug.LogError(string.Format(Strings.get_Error_Missing_Script(), fsmStateAction.get_Name()));
		}
		public static void Edit(Type actionType)
		{
			Object asset = ActionScripts.GetAsset(actionType);
			if (asset != null)
			{
				AssetDatabase.OpenAsset(asset);
				return;
			}
			Debug.LogError(string.Format(Strings.get_Error_Missing_Script(), Labels.GetActionLabel(actionType)));
		}
		public static void EditAsset(object userdata)
		{
			SkillStateAction fsmStateAction = (SkillStateAction)userdata;
			Object asset = ActionScripts.GetAsset(fsmStateAction);
			if (asset != null)
			{
				AssetDatabase.OpenAsset(asset);
				return;
			}
			Debug.LogError(string.Format(Strings.get_Error_Missing_Script(), fsmStateAction.get_Name()));
		}
		public static void PingAssetByType(object userdata)
		{
			Type type = (Type)userdata;
			Object asset = ActionScripts.GetAsset(type);
			if (asset != null)
			{
				EditorGUIUtility.PingObject(asset);
				return;
			}
			Debug.LogError(string.Format(Strings.get_Error_Missing_Script(), type));
		}
		public static void SelectAssetByType(object userdata)
		{
			Type type = (Type)userdata;
			Object asset = ActionScripts.GetAsset(type);
			if (asset != null)
			{
				EditorGUIUtility.PingObject(asset);
				Selection.set_activeObject(asset);
				return;
			}
			Debug.LogError(string.Format(Strings.get_Error_Missing_Script(), type));
		}
		public static void EditAssetByType(object userdata)
		{
			Type type = (Type)userdata;
			Object asset = ActionScripts.GetAsset(type);
			if (asset != null)
			{
				AssetDatabase.OpenAsset(asset);
				return;
			}
			Debug.LogError(string.Format(Strings.get_Error_Missing_Script(), type));
		}
		public static Object GetAsset(SkillStateAction action)
		{
			if (action != null)
			{
				return ActionScripts.GetAsset(action.GetType());
			}
			return null;
		}
		public static Object GetAsset(Type actionType)
		{
			if (ActionScripts.ActionScriptLookup.get_Count() == 0)
			{
				Actions.BuildList();
			}
			Object result;
			ActionScripts.ActionScriptLookup.TryGetValue(actionType, ref result);
			return result;
		}
	}
}
