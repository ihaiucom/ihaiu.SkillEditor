using HutongGames.PlayMaker;
using System;
using UnityEngine;
[Serializable]
public class SkillTemplate : ScriptableObject
{
	[SerializeField]
	private string category;
	public Skill fsm;
	public string Category
	{
		get
		{
			return this.category;
		}
		set
		{
			this.category = value;
		}
	}
	public void OnEnable()
	{
		if (this.fsm != null)
		{
			this.fsm.UsedInTemplate = this;
		}
	}
}
