using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	public class SkillDebugUtility
	{
		public static void Log(Skill fsm, string text, bool frameCount = false)
		{
			if (fsm.GameObject != null)
			{
				text = string.Concat(new string[]
				{
					text,
					" : ",
					fsm.GameObject.get_name(),
					" : ",
					fsm.Name
				});
			}
			SkillDebugUtility.Log(text, frameCount);
		}
		public static void Log(string text, bool frameCount = false)
		{
			if (frameCount)
			{
				text = Time.get_frameCount() + " : " + text;
			}
			Debug.Log(text);
		}
		public static void Log(Object obj, string text)
		{
			text = obj.get_name() + " : " + text;
			SkillDebugUtility.Log(text, false);
		}
	}
}
