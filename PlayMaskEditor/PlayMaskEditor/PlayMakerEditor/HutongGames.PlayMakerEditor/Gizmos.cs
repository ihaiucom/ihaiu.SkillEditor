using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[InitializeOnLoad]
	public class Gizmos
	{
		private static readonly Dictionary<int, int> lookupFsmCount;
		private static bool enableHierarchyItemGizmos;
		private static Rect itemRect;
		private static readonly EditorApplication.HierarchyWindowItemCallback hierarchyWindowItemCallback;
		private static Texture2D wanIcon;
		public static bool EnableHierarchyItemGizmos
		{
			get
			{
				return Gizmos.enableHierarchyItemGizmos;
			}
			set
			{
				Gizmos.enableHierarchyItemGizmos = value;
				if (Gizmos.enableHierarchyItemGizmos)
				{
					EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Remove(EditorApplication.hierarchyWindowItemOnGUI, Gizmos.hierarchyWindowItemCallback);
					EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, Gizmos.hierarchyWindowItemCallback);
					return;
				}
				EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Remove(EditorApplication.hierarchyWindowItemOnGUI, Gizmos.hierarchyWindowItemCallback);
			}
		}
		private static Texture2D WanIcon
		{
			get
			{
				Texture2D arg_1D_0;
				if ((arg_1D_0 = Gizmos.wanIcon) == null)
				{
					arg_1D_0 = (Gizmos.wanIcon = Files.LoadTextureFromDll("wanIcon", 15, 15));
				}
				return arg_1D_0;
			}
		}
		public static void ClearFsmLookupCache()
		{
			Gizmos.lookupFsmCount.Clear();
		}
		static Gizmos()
		{
			Gizmos.lookupFsmCount = new Dictionary<int, int>();
			Gizmos.itemRect = default(Rect);
			Gizmos.hierarchyWindowItemCallback = new EditorApplication.HierarchyWindowItemCallback(Gizmos.HierarchyWindowItemCallback);
			Gizmos.EnableHierarchyItemGizmos = EditorPrefs.GetBool(EditorPrefStrings.get_DrawPlaymakerGizmoInHierarchy(), true);
		}
		private static void HierarchyWindowItemCallback(int instanceID, Rect selectionRect)
		{
			int num;
			if (!Gizmos.lookupFsmCount.TryGetValue(instanceID, ref num))
			{
				GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
				if (gameObject == null)
				{
					return;
				}
				num = gameObject.GetComponents<PlayMakerFSM>().Length;
				Gizmos.lookupFsmCount.Add(instanceID, num);
			}
			if (num > 0)
			{
				Gizmos.itemRect.Set(selectionRect.get_x() + selectionRect.get_width() - 15f, selectionRect.get_y(), 15f, 15f);
				GUI.DrawTexture(Gizmos.itemRect, Gizmos.WanIcon);
			}
		}
		private static void ProjectWindowItemCallback(string guid, Rect selectionRect)
		{
			if (SkillPrefabs.AssetHasPlayMakerFSM(guid))
			{
				Rect rect = new Rect(selectionRect.get_x() + selectionRect.get_width() - 20f, selectionRect.get_y(), 15f, 15f);
				GUI.Box(rect, Gizmos.WanIcon, GUIStyle.get_none());
			}
		}
	}
}
