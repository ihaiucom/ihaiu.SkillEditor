using System;
using UnityEngine;
public class PlayMakerJointBreak2D : PlayMakerProxyBase
{
	public void OnJointBreak2D(Joint2D brokenJoint)
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Active && playMakerFSM.Fsm.HandleJointBreak2D)
			{
				playMakerFSM.Fsm.OnJointBreak2D(brokenJoint);
			}
		}
	}
}
