using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	public class DelayedEvent
	{
		private readonly Skill fsm;
		private readonly SkillEvent fsmEvent;
		private readonly SkillEventTarget eventTarget;
		private SkillEventData eventData;
		private float timer;
		private bool eventFired;
		public SkillEvent FsmEvent
		{
			get
			{
				return this.fsmEvent;
			}
		}
		public float Timer
		{
			get
			{
				return this.timer;
			}
		}
		public bool Finished
		{
			get
			{
				return this.eventFired;
			}
		}
		public DelayedEvent(Skill fsm, SkillEvent fsmEvent, float delay)
		{
			this.fsm = fsm;
			this.timer = delay;
			this.fsmEvent = fsmEvent;
			this.eventData = new SkillEventData(Skill.EventData)
			{
				SentByFsm = SkillExecutionStack.ExecutingFsm,
				SentByState = SkillExecutionStack.ExecutingState,
				SentByAction = SkillExecutionStack.ExecutingAction
			};
		}
		public DelayedEvent(Skill fsm, string fsmEventName, float delay) : this(fsm, SkillEvent.GetFsmEvent(fsmEventName), delay)
		{
		}
		public DelayedEvent(Skill fsm, SkillEventTarget eventTarget, SkillEvent fsmEvent, float delay) : this(fsm, fsmEvent, delay)
		{
			this.eventTarget = eventTarget;
		}
		public DelayedEvent(Skill fsm, SkillEventTarget eventTarget, string fsmEvent, float delay) : this(fsm, fsmEvent, delay)
		{
			this.eventTarget = eventTarget;
		}
		public DelayedEvent(PlayMakerFSM fsm, SkillEvent fsmEvent, float delay) : this(fsm.Fsm, fsmEvent, delay)
		{
		}
		public DelayedEvent(PlayMakerFSM fsm, string fsmEventName, float delay) : this(fsm.Fsm, fsmEventName, delay)
		{
		}
		public void Update()
		{
			this.timer -= Time.get_deltaTime();
			if (this.timer < 0f)
			{
				SkillEventData fsmEventData = Skill.EventData;
				Skill.EventData = this.eventData;
				if (this.eventTarget == null)
				{
					this.fsm.Event(this.fsmEvent);
				}
				else
				{
					this.fsm.Event(this.eventTarget, this.fsmEvent);
				}
				this.fsm.UpdateStateChanges();
				this.eventFired = true;
				this.eventData = null;
				Skill.EventData = fsmEventData;
			}
		}
		public static bool WasSent(DelayedEvent delayedEvent)
		{
			return delayedEvent == null || delayedEvent.eventFired;
		}
	}
}
