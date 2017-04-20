using HutongGames.PlayMaker;
using System;
public class PlayMakerApplicationEvents : PlayMakerProxyBase
{
	public void OnApplicationFocus()
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Fsm.HandleApplicationEvents)
			{
				playMakerFSM.Fsm.Event(SkillEvent.ApplicationFocus);
			}
		}
	}
	public void OnApplicationPause()
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Fsm.HandleApplicationEvents)
			{
				playMakerFSM.Fsm.Event(SkillEvent.ApplicationPause);
			}
		}
	}
}
