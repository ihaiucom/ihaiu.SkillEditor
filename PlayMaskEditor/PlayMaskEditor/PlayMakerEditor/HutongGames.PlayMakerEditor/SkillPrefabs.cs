using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public static class SkillPrefabs
	{
		private static readonly Dictionary<string, bool> assetHasPlayMakerFSMLookup = new Dictionary<string, bool>();
		private static Skill lastSerializedPropertyLookup;
		private static SerializedProperty lastFsmSerializedProperty;
		public static void LoadUsedPrefabs()
		{
			using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					if (current.get_Owner() != null && PrefabUtility.GetPrefabType(current.get_Owner()) == 3)
					{
						PrefabUtility.GetPrefabParent(current.get_Owner());
					}
				}
			}
		}
		[Localizable(false)]
		public static bool IsModifiedPrefabInstance(Skill fsm)
		{
			if (fsm == null || fsm.get_Owner() == null)
			{
				return false;
			}
			Skill arg_19_0 = SkillPrefabs.lastSerializedPropertyLookup;
			SerializedObject serializedObject = new SerializedObject(fsm.get_Owner());
			SkillPrefabs.lastFsmSerializedProperty = serializedObject.FindProperty("fsm");
			SkillPrefabs.lastSerializedPropertyLookup = fsm;
			return SkillPrefabs.lastFsmSerializedProperty.get_prefabOverride();
		}
		public static void UpdateIsModifiedPrefabInstance(Skill fsm)
		{
			if (fsm == null || fsm.get_Owner() == null)
			{
				return;
			}
			fsm.set_IsModifiedPrefabInstance(SkillPrefabs.IsModifiedPrefabInstance(fsm));
		}
		public static bool ShouldModify(Skill fsm)
		{
			if (fsm == null || fsm.get_Owner() == null)
			{
				return false;
			}
			switch (PrefabUtility.GetPrefabType(fsm.get_Owner()))
			{
			case 0:
			case 1:
			case 2:
			case 5:
			case 6:
			case 7:
				return true;
			case 3:
			case 4:
				return fsm.get_IsModifiedPrefabInstance();
			default:
				return true;
			}
		}
		public static bool IsPersistent(Object obj)
		{
			return obj != null && EditorUtility.IsPersistent(obj);
		}
		public static bool IsPrefab(Skill fsm)
		{
			if (fsm == null || fsm.get_Owner() == null)
			{
				return false;
			}
			PrefabType prefabType = PrefabUtility.GetPrefabType(fsm.get_Owner());
			return prefabType == 1;
		}
		public static bool IsPersistent(Skill fsm)
		{
			return fsm != null && (fsm.get_UsedInTemplate() || SkillPrefabs.IsPrefab(fsm));
		}
		public static bool IsPrefabInstance(Skill fsm)
		{
			if (fsm == null || fsm.get_Owner() == null)
			{
				return false;
			}
			PrefabType prefabType = PrefabUtility.GetPrefabType(fsm.get_Owner());
			return prefabType == 3;
		}
		public static bool IsFsmInstanceOfPrefab(Skill fsm, Skill prefab)
		{
			if (fsm == null || fsm.get_Owner() == null || prefab == null)
			{
				return false;
			}
			PrefabType prefabType = PrefabUtility.GetPrefabType(fsm.get_Owner());
			return prefabType == 3 && PrefabUtility.GetPrefabParent(fsm.get_Owner()) == prefab.get_Owner();
		}
		public static void BuildAssetsWithPlayMakerFSMsList()
		{
			SkillPrefabs.assetHasPlayMakerFSMLookup.Clear();
		}
		public static bool AssetHasPlayMakerFSM(string guid)
		{
			bool flag;
			if (SkillPrefabs.assetHasPlayMakerFSMLookup.TryGetValue(guid, ref flag))
			{
				return flag;
			}
			string text = AssetDatabase.GUIDToAssetPath(guid);
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					string assetPath = AssetDatabase.GetAssetPath(current.get_OwnerObject());
					if (string.Compare(assetPath, text, 5) == 0)
					{
						flag = true;
						break;
					}
				}
			}
			SkillPrefabs.assetHasPlayMakerFSMLookup.Add(guid, flag);
			return flag;
		}
		public static bool StateExistsInPrefabParent(SkillState state)
		{
			PlayMakerFSM playMakerFSM = state.get_Fsm().get_Owner() as PlayMakerFSM;
			return PrefabUtility.GetPrefabType(playMakerFSM) == 3;
		}
	}
}
