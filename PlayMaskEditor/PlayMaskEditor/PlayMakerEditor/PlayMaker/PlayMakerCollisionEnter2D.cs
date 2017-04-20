using System;
using UnityEngine;
public class PlayMakerCollisionEnter2D : PlayMakerProxyBase
{
	public void OnCollisionEnter2D(Collision2D collisionInfo)
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Active && playMakerFSM.Fsm.HandleCollisionEnter2D)
			{
				playMakerFSM.Fsm.OnCollisionEnter2D(collisionInfo);
			}
		}
	}
}
