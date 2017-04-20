using HutongGames.PlayMaker;
using System;
public class PlayMakerMouseEvents : PlayMakerProxyBase
{
	public void OnMouseEnter()
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Fsm.MouseEvents)
			{
				playMakerFSM.Fsm.Event(SkillEvent.MouseEnter);
			}
		}
	}
	public void OnMouseDown()
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Fsm.MouseEvents)
			{
				playMakerFSM.Fsm.Event(SkillEvent.MouseDown);
			}
		}
	}
	public void OnMouseUp()
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Fsm.MouseEvents)
			{
				playMakerFSM.Fsm.Event(SkillEvent.MouseUp);
				Skill.LastClickedObject = base.get_gameObject();
			}
		}
	}
	public void OnMouseUpAsButton()
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Fsm.MouseEvents)
			{
				playMakerFSM.Fsm.Event(SkillEvent.MouseUpAsButton);
				Skill.LastClickedObject = base.get_gameObject();
			}
		}
	}
	public void OnMouseExit()
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Fsm.MouseEvents)
			{
				playMakerFSM.Fsm.Event(SkillEvent.MouseExit);
			}
		}
	}
	public void OnMouseDrag()
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Fsm.MouseEvents)
			{
				playMakerFSM.Fsm.Event(SkillEvent.MouseDrag);
			}
		}
	}
	public void OnMouseOver()
	{
		for (int i = 0; i < this.playMakerFSMs.Length; i++)
		{
			PlayMakerFSM playMakerFSM = this.playMakerFSMs[i];
			if (playMakerFSM.Fsm.MouseEvents)
			{
				playMakerFSM.Fsm.Event(SkillEvent.MouseOver);
			}
		}
	}
}
