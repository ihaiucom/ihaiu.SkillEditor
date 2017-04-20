using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public static class Watermarks
	{
		private static readonly Dictionary<Skill, Texture> watermarkTextureLookup = new Dictionary<Skill, Texture>();
		private static Skill lastWatermarkFsm;
		private static Texture lastWatermark;
		public static string GetLabel(PlayMakerFSM fsmComponent, string defaultLabel = "No Watermark")
		{
			if (fsmComponent == null || string.IsNullOrEmpty(fsmComponent.get_Fsm().get_Watermark()))
			{
				return defaultLabel;
			}
			return fsmComponent.get_Fsm().get_Watermark();
		}
		public static Texture Set(Skill fsm, string textureName)
		{
			fsm.set_Watermark(textureName);
			Watermarks.watermarkTextureLookup.Remove(fsm);
			Watermarks.lastWatermarkFsm = null;
			return Watermarks.Get(fsm);
		}
		public static Texture Get(Skill fsm)
		{
			if (Watermarks.lastWatermarkFsm == fsm)
			{
				return Watermarks.lastWatermark;
			}
			if (fsm == null || string.IsNullOrEmpty(fsm.get_Watermark()))
			{
				return null;
			}
			Texture texture;
			Watermarks.watermarkTextureLookup.TryGetValue(fsm, ref texture);
			if (texture != null)
			{
				Watermarks.lastWatermarkFsm = fsm;
				Watermarks.lastWatermark = texture;
				return texture;
			}
			texture = Watermarks.Load(fsm.get_Watermark());
			Watermarks.lastWatermarkFsm = fsm;
			Watermarks.lastWatermark = texture;
			Watermarks.watermarkTextureLookup.Remove(fsm);
			Watermarks.watermarkTextureLookup.Add(fsm, texture);
			return texture;
		}
		public static Texture Load(string name)
		{
			string text = Path.Combine(SkillPaths.WatermarksPath, name);
			Texture texture = (Texture)AssetDatabase.LoadMainAssetAtPath(text);
			if (texture == null)
			{
				Debug.LogError(Strings.get_Error_Failed_to_load_texture__() + text);
			}
			return texture;
		}
		public static string[] GetNames()
		{
			string watermarksFullPath = SkillPaths.WatermarksFullPath;
			string[] files = Files.GetFiles(watermarksFullPath, "*.png|*.jpg|*.jpeg", 0);
			for (int i = 0; i < files.Length; i++)
			{
				files[i] = Path.GetFileName(files[i]);
			}
			return files;
		}
		public static Texture[] GetTextures(bool showProgress = true)
		{
			List<Texture> list = new List<Texture>();
			string watermarksPath = SkillPaths.WatermarksPath;
			string[] files = Files.GetFiles(watermarksPath, "*.png|*.jpg|*.jpeg", 0);
			int num = 0;
			float num2 = (float)files.Length;
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (showProgress)
				{
					EditorUtility.DisplayProgressBar(Strings.get_ProductName(), Strings.get_Label_Loading_Watermark_Textures___(), (float)num++ / num2);
				}
				Texture texture = (Texture)AssetDatabase.LoadMainAssetAtPath(text.Replace(Application.get_dataPath(), "Assets"));
				list.Add(texture);
			}
			if (showProgress)
			{
				EditorUtility.ClearProgressBar();
			}
			return list.ToArray();
		}
	}
}
