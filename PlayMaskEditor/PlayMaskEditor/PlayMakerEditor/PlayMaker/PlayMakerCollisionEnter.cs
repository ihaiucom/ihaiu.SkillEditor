using System;
using UnityEngine;
public class PlayMakerCollisionEnter : PlayMakerProxyBase
{
	public void OnCollisionEnter(Collision collisionInfo)
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Active && playMakerFSM.Fsm.HandleCollisionEnter)
			{
				playMakerFSM.Fsm.OnCollisionEnter(collisionInfo);
			}
		}
	}
}
