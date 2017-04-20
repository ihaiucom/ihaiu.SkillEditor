using System;
using UnityEngine;
public class PlayMakerCollisionStay2D : PlayMakerProxyBase
{
	public void OnCollisionStay2D(Collision2D collisionInfo)
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Active && playMakerFSM.Fsm.HandleCollisionStay2D)
			{
				playMakerFSM.Fsm.OnCollisionStay2D(collisionInfo);
			}
		}
	}
}
