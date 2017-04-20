using System;
using UnityEngine;
public class PlayMakerControllerColliderHit : PlayMakerProxyBase
{
	public void OnControllerColliderHit(ControllerColliderHit hitCollider)
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Active && playMakerFSM.Fsm.HandleControllerColliderHit)
			{
				playMakerFSM.Fsm.OnControllerColliderHit(hitCollider);
			}
		}
	}
}
