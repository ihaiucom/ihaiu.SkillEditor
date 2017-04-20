using System;
using UnityEditor;
namespace HutongGames.PlayMakerEditor
{
	public static class DragAndDropManager
	{
		public enum DragMode
		{
			None,
			AddAction,
			MoveActions
		}
		private const string AddActionMode = "AddAction";
		private const string MoveActionsMode = "MoveActions";
		public static DragAndDropManager.DragMode mode;
		public static Type AddAction
		{
			get;
			private set;
		}
		public static void SetMode(DragAndDropManager.DragMode newMode)
		{
			DragAndDropManager.mode = newMode;
		}
		public static void Update()
		{
			DragAndDropManager.AddAction = (DragAndDrop.GetGenericData("AddAction") as Type);
			if (DragAndDropManager.AddAction != null)
			{
				DragAndDropManager.mode = DragAndDropManager.DragMode.AddAction;
				return;
			}
			if (DragAndDrop.GetGenericData("MoveActions") != null)
			{
				DragAndDropManager.mode = DragAndDropManager.DragMode.MoveActions;
				return;
			}
			DragAndDropManager.mode = DragAndDropManager.DragMode.None;
		}
		public static void Reset()
		{
			DragAndDropManager.SetMode(DragAndDropManager.DragMode.None);
			DragAndDrop.SetGenericData("MoveActions", null);
			DragAndDrop.SetGenericData("AddAction", null);
		}
	}
}
