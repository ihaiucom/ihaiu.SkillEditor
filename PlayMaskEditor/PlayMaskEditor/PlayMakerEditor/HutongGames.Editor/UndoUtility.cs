using HutongGames.PlayMakerEditor;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace HutongGames.Editor
{
	public class UndoUtility
	{
		public static void MarkSceneDirty()
		{
			if (EditorApplication.get_isPlaying())
			{
				return;
			}
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			SkillEditor.IgnoreHierarchyChange = true;
		}
		public static void RegisterUndo(Object objectToUndo, string message)
		{
			if (FsmEditorSettings.DisableUndoRedo)
			{
				UndoUtility.MarkSceneDirty();
				return;
			}
			Undo.RegisterCompleteObjectUndo(objectToUndo, message);
		}
		public static void RegisterUndo(Object[] objectsToUndo, string message)
		{
			if (FsmEditorSettings.DisableUndoRedo)
			{
				UndoUtility.MarkSceneDirty();
				return;
			}
			Undo.RecordObjects(objectsToUndo, message);
		}
		public static void SetSnapshotTarget(Object objectsToUndo, string message)
		{
			if (FsmEditorSettings.DisableUndoRedo)
			{
				UndoUtility.MarkSceneDirty();
				return;
			}
			Undo.RecordObject(objectsToUndo, message);
		}
		public static void CreateSnapshot()
		{
		}
		public static void RegisterSnapshot()
		{
		}
	}
}
