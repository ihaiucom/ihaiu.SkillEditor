using System;
using UnityEngine;
public class PlayMakerProxyBase : MonoBehaviour
{
	protected PlayMakerFSM[] playMakerFSMs;
	public void Awake()
	{
		this.Reset();
	}
	public void Reset()
	{
		this.playMakerFSMs = base.GetComponents<PlayMakerFSM>();
	}
}
