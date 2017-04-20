using System;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	internal class WatermarkSelector
	{
		private const int numColumns = 4;
		private static Vector2 scrollViewPosition;
		private static GUIContent[] watermarkThumbs;
		private static float gridWidth;
		private static float gridHeight;
		private static int selectedWatermarkIndex;
		public static void Init()
		{
			Texture[] textures = Watermarks.GetTextures(true);
			string[] names = Watermarks.GetNames();
			WatermarkSelector.watermarkThumbs = new GUIContent[textures.Length];
			for (int i = 0; i < textures.Length; i++)
			{
				WatermarkSelector.watermarkThumbs[i] = new GUIContent(textures[i], names[i]);
			}
			WatermarkSelector.gridWidth = 326f;
			WatermarkSelector.gridHeight = (float)(textures.Length / 4) * WatermarkSelector.gridWidth / 4f;
		}
		public static void ResetSelection()
		{
			if (WatermarkSelector.watermarkThumbs == null)
			{
				WatermarkSelector.Init();
			}
			if (WatermarkSelector.watermarkThumbs == null)
			{
				Debug.LogError(Strings.get_Error_Could_not_load_watermarks());
				return;
			}
			Texture texture = Watermarks.Get(SkillEditor.SelectedFsm);
			WatermarkSelector.selectedWatermarkIndex = -1;
			for (int i = 0; i < WatermarkSelector.watermarkThumbs.Length; i++)
			{
				if (texture == WatermarkSelector.watermarkThumbs[i].get_image())
				{
					WatermarkSelector.selectedWatermarkIndex = i;
				}
			}
		}
		public static void OnGUI()
		{
			if (!FsmEditorSettings.EnableWatermarks)
			{
				GUILayout.Label(Strings.get_Label_Watermarks_Are_Disabled(), new GUILayoutOption[0]);
				if (GUILayout.Button(Strings.get_Command_Enable_Watermarks(), new GUILayoutOption[0]))
				{
					FsmEditorSettings.EnableWatermarks = true;
				}
				if (GUILayout.Button(Strings.get_Command_Finished(), new GUILayoutOption[0]))
				{
					WatermarkSelector.Cancel();
				}
				GUILayout.FlexibleSpace();
				return;
			}
			GUILayout.Label(Strings.get_Label_Select_A_Watermark(), new GUILayoutOption[0]);
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			WatermarkSelector.scrollViewPosition = GUILayout.BeginScrollView(WatermarkSelector.scrollViewPosition, new GUILayoutOption[0]);
			Color color = GUI.get_color();
			GUI.set_contentColor(SkillEditorStyles.WatermarkTintSolid);
			int num = GUILayout.SelectionGrid(WatermarkSelector.selectedWatermarkIndex, WatermarkSelector.watermarkThumbs, 4, new GUILayoutOption[]
			{
				GUILayout.Width(WatermarkSelector.gridWidth),
				GUILayout.Height(WatermarkSelector.gridHeight)
			});
			if (num != WatermarkSelector.selectedWatermarkIndex)
			{
				WatermarkSelector.SelectWatermark(num);
			}
			GUI.set_contentColor(color);
			GUILayout.EndScrollView();
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			if (GUILayout.Button(Strings.get_Command_Clear_Watermark(), new GUILayoutOption[0]))
			{
				Watermarks.Set(SkillEditor.SelectedFsm, "");
				WatermarkSelector.Cancel();
			}
			if (GUILayout.Button(Strings.get_Command_Finished(), new GUILayoutOption[0]))
			{
				WatermarkSelector.Cancel();
			}
			if (FsmEditorSettings.ShowHints)
			{
				GUILayout.Box(Strings.get_Hint_Watermarks(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
			}
			EditorGUILayout.Space();
		}
		private static void SelectWatermark(int index)
		{
			string[] names = Watermarks.GetNames();
			Watermarks.Set(SkillEditor.SelectedFsm, names[index]);
			SkillEditor.SetFsmDirty(false, false);
			WatermarkSelector.selectedWatermarkIndex = index;
		}
		private static void Cancel()
		{
			SkillEditor.Inspector.SetMode(InspectorMode.FsmInspector);
		}
	}
}
