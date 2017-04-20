using System;
using UnityEditor.Callbacks;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class SkillBuildCallbacks
	{
		[PostProcessScene(2)]
		public static void OnPostprocessScene()
		{
			PlayMakerGlobals.set_IsBuilding(true);
			PlayMakerGlobals.InitApplicationFlags();
			PlayMakerFSM[] array = Resources.FindObjectsOfTypeAll<PlayMakerFSM>();
			PlayMakerFSM[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				PlayMakerFSM playMakerFSM = array2[i];
				if (!Application.get_isPlaying())
				{
					playMakerFSM.Preprocess();
				}
			}
			PlayMakerGlobals.set_IsBuilding(false);
		}
	}
}
