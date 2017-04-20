using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	public static class SkillTime
	{
		private static bool firstUpdateHasHappened;
		private static float totalEditorPlayerPausedTime;
		private static float realtimeLastUpdate;
		private static int frameCountLastUpdate;
		public static float RealtimeSinceStartup
		{
			get
			{
				if (SkillTime.firstUpdateHasHappened)
				{
					if (Time.get_realtimeSinceStartup() < SkillTime.totalEditorPlayerPausedTime)
					{
						SkillTime.totalEditorPlayerPausedTime = 0f;
					}
					return Time.get_realtimeSinceStartup() - SkillTime.totalEditorPlayerPausedTime;
				}
				SkillTime.totalEditorPlayerPausedTime = Time.get_realtimeSinceStartup();
				return 0f;
			}
		}
		public static void RealtimeBugFix()
		{
			SkillTime.firstUpdateHasHappened = true;
		}
		public static void Update()
		{
			float realtimeSinceStartup = Time.get_realtimeSinceStartup();
			if (Time.get_frameCount() == SkillTime.frameCountLastUpdate)
			{
				SkillTime.totalEditorPlayerPausedTime += realtimeSinceStartup - SkillTime.realtimeLastUpdate;
			}
			SkillTime.frameCountLastUpdate = Time.get_frameCount();
			SkillTime.realtimeLastUpdate = Time.get_realtimeSinceStartup();
		}
		public static string FormatTime(float time)
		{
			DateTime dateTime = new DateTime((long)(time * 1E+07f));
			return dateTime.ToString("mm:ss:ff");
		}
		public static void DebugLog()
		{
			Debug.Log("LastFrameCount: " + SkillTime.frameCountLastUpdate);
			Debug.Log("PausedTime: " + SkillTime.totalEditorPlayerPausedTime);
			Debug.Log("Realtime: " + SkillTime.RealtimeSinceStartup);
		}
	}
}
