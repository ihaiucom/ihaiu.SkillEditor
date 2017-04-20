using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace ihaiu
{
    public static class SkillPrefabs
    {
        private static readonly Dictionary<string, bool> assetHasPlayMakerFSMLookup = new Dictionary<string, bool>();
        private static Skill lastSerializedPropertyLookup;
        private static SerializedProperty lastFsmSerializedProperty;
        public static void LoadUsedPrefabs()
        {
          
        }
        [Localizable(false)]
        public static bool IsModifiedPrefabInstance(Skill fsm)
        {
            return false;
        }
        public static void UpdateIsModifiedPrefabInstance(Skill fsm)
        {
            if (fsm == null || fsm.OwnerName == null)
            {
                return;
            }
            fsm.IsModifiedPrefabInstance = SkillPrefabs.IsModifiedPrefabInstance(fsm);
        }
        public static bool ShouldModify(Skill fsm)
        {
            if (fsm == null || fsm.OwnerName == null)
            {
                return false;
            }
            return false;
//            switch (PrefabUtility.GetPrefabType(fsm.OwnerName))
//            {
//                case 0:
//                case 1:
//                case 2:
//                case 5:
//                case 6:
//                case 7:
//                    return true;
//                case 3:
//                case 4:
//                    return fsm.IsModifiedPrefabInstance;
//                default:
//                    return true;
//            }
        }
        public static bool IsPersistent(UnityEngine.Object obj)
        {
            return obj != null && EditorUtility.IsPersistent(obj);
        }
        public static bool IsPrefab(Skill fsm)
        {
            if (fsm == null || fsm.OwnerName == null)
            {
                return false;
            }
            return false;
//            PrefabType prefabType = PrefabUtility.GetPrefabType(fsm.OwnerName);
//            return prefabType == 1;
        }
        public static bool IsPersistent(Skill fsm)
        {
            return fsm != null && (fsm.UsedInTemplate || SkillPrefabs.IsPrefab(fsm));
        }
        public static bool IsPrefabInstance(Skill fsm)
        {
            if (fsm == null || fsm.OwnerName == null)
            {
                return false;
            }
            return false;

//            PrefabType prefabType = PrefabUtility.GetPrefabType(fsm.OwnerName);
//            return prefabType == 3;
        }
        public static bool IsFsmInstanceOfPrefab(Skill fsm, Skill prefab)
        {
            if (fsm == null || fsm.OwnerName == null || prefab == null)
            {
                return false;
            }
            return false;
//            PrefabType prefabType = PrefabUtility.GetPrefabType(fsm.OwnerName);
//            return prefabType == 3 && PrefabUtility.GetPrefabParent(fsm.OwnerName) == prefab.OwnerName;
        }
        public static void BuildAssetsWithPlayMakerFSMsList()
        {
            SkillPrefabs.assetHasPlayMakerFSMLookup.Clear();
        }
    }
}
