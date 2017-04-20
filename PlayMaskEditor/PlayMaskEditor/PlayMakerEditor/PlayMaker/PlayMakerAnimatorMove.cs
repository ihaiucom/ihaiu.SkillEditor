using System;
public class PlayMakerAnimatorMove : PlayMakerProxyBase
{
	public void OnAnimatorMove()
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Active && playMakerFSM.Fsm.HandleAnimatorMove)
			{
				playMakerFSM.Fsm.OnAnimatorMove();
			}
		}
	}
}
