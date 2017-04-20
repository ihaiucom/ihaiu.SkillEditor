using System;
using UnityEngine;
public class PlayMakerCollisionExit : PlayMakerProxyBase
{
	public void OnCollisionExit(Collision collisionInfo)
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Active && playMakerFSM.Fsm.HandleCollisionExit)
			{
				playMakerFSM.Fsm.OnCollisionExit(collisionInfo);
			}
		}
	}
}
