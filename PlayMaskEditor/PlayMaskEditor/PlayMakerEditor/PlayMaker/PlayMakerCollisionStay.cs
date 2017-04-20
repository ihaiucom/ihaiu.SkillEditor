using System;
using UnityEngine;
public class PlayMakerCollisionStay : PlayMakerProxyBase
{
	public void OnCollisionStay(Collision collisionInfo)
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Active && playMakerFSM.Fsm.HandleCollisionStay)
			{
				playMakerFSM.Fsm.OnCollisionStay(collisionInfo);
			}
		}
	}
}
