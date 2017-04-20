using System;
public class PlayMakerFixedUpdate : PlayMakerProxyBase
{
	public void FixedUpdate()
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Active && playMakerFSM.Fsm.HandleFixedUpdate)
			{
				playMakerFSM.Fsm.FixedUpdate();
			}
		}
	}
}
