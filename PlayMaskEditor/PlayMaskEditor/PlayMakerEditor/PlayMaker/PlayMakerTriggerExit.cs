using System;
using UnityEngine;
public class PlayMakerTriggerExit : PlayMakerProxyBase
{
	public void OnTriggerExit(Collider other)
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Active && playMakerFSM.Fsm.HandleTriggerExit)
			{
				playMakerFSM.Fsm.OnTriggerExit(other);
			}
		}
	}
}
