using System;
public class PlayMakerAnimatorIK : PlayMakerProxyBase
{
	public void OnAnimatorIK(int layerIndex)
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Active && playMakerFSM.Fsm.HandleAnimatorIK)
			{
				playMakerFSM.Fsm.OnAnimatorIK(layerIndex);
			}
		}
	}
}
