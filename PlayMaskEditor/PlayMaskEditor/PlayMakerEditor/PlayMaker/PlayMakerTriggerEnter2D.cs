using System;
using UnityEngine;
public class PlayMakerTriggerEnter2D : PlayMakerProxyBase
{
	public void OnTriggerEnter2D(Collider2D other)
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Active && playMakerFSM.Fsm.HandleTriggerEnter2D)
			{
				playMakerFSM.Fsm.OnTriggerEnter2D(other);
			}
		}
	}
}
