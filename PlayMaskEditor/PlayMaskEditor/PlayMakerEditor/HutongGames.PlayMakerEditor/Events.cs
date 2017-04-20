using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public static class Events
	{
		public static Skill GetFsmTarget(Skill fsm, SkillEventTarget fsmEventTarget)
		{
			if (fsmEventTarget == null)
			{
				return fsm;
			}
			switch (fsmEventTarget.target)
			{
			case 0:
				return fsm;
			case 1:
				return null;
			case 2:
				return null;
			case 3:
				if (!(fsmEventTarget.fsmComponent != null))
				{
					return null;
				}
				return fsmEventTarget.fsmComponent.get_Fsm();
			case 4:
				return null;
			default:
				return null;
			}
		}
		public static bool FsmStateRespondsToEvent(SkillState state, SkillEvent fsmEvent)
		{
			if (SkillEvent.IsNullOrEmpty(fsmEvent))
			{
				return false;
			}
			SkillTransition[] globalTransitions = state.get_Fsm().get_GlobalTransitions();
			for (int i = 0; i < globalTransitions.Length; i++)
			{
				SkillTransition fsmTransition = globalTransitions[i];
				if (fsmTransition.get_EventName() == fsmEvent.get_Name())
				{
					bool result = true;
					return result;
				}
			}
			SkillTransition[] transitions = state.get_Transitions();
			for (int j = 0; j < transitions.Length; j++)
			{
				SkillTransition fsmTransition2 = transitions[j];
				if (fsmTransition2.get_EventName() == fsmEvent.get_Name())
				{
					bool result = true;
					return result;
				}
			}
			return false;
		}
		public static bool FsmRespondsToEvent(Skill fsm, SkillEvent fsmEvent)
		{
			return fsm != null && !SkillEvent.IsNullOrEmpty(fsmEvent) && Events.FsmRespondsToEvent(fsm, fsmEvent.get_Name());
		}
		public static bool FsmRespondsToEvent(Skill fsm, string fsmEventName)
		{
			if (fsm == null || string.IsNullOrEmpty(fsmEventName))
			{
				return false;
			}
			SkillTransition[] globalTransitions = fsm.get_GlobalTransitions();
			for (int i = 0; i < globalTransitions.Length; i++)
			{
				SkillTransition fsmTransition = globalTransitions[i];
				if (fsmTransition.get_EventName() == fsmEventName)
				{
					bool result = true;
					return result;
				}
			}
			SkillState[] states = fsm.get_States();
			for (int j = 0; j < states.Length; j++)
			{
				SkillState fsmState = states[j];
				SkillTransition[] transitions = fsmState.get_Transitions();
				for (int k = 0; k < transitions.Length; k++)
				{
					SkillTransition fsmTransition2 = transitions[k];
					if (fsmTransition2.get_EventName() == fsmEventName)
					{
						bool result = true;
						return result;
					}
				}
			}
			return false;
		}
		public static List<SkillEvent> GetGlobalEventList()
		{
			List<SkillEvent> list = new List<SkillEvent>();
			using (List<SkillEvent>.Enumerator enumerator = SkillEvent.get_EventList().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					if (current.get_IsGlobal())
					{
						list.Add(current);
					}
				}
			}
			return list;
		}
		public static List<SkillEvent> GetGlobalEventList(Skill fsm)
		{
			if (fsm == null)
			{
				return Events.GetGlobalEventList();
			}
			List<SkillEvent> list = new List<SkillEvent>();
			SkillEvent[] events = fsm.get_Events();
			for (int i = 0; i < events.Length; i++)
			{
				SkillEvent fsmEvent = events[i];
				if (fsmEvent.get_IsGlobal())
				{
					list.Add(fsmEvent);
				}
			}
			return list;
		}
		public static List<SkillEvent> GetEventList(Skill fsm)
		{
			if (fsm != null)
			{
				return Events.GetGlobalEventList(fsm);
			}
			return Events.GetGlobalEventList();
		}
		public static List<SkillEvent> GetEventList(PlayMakerFSM fsmComponent)
		{
			if (!(fsmComponent == null))
			{
				return Events.GetGlobalEventList(fsmComponent.get_Fsm());
			}
			return Events.GetGlobalEventList();
		}
		public static List<SkillEvent> GetEventList(GameObject go)
		{
			return Events.GetGlobalEventList();
		}
		public static GUIContent[] GetEventNamesFromList(List<SkillEvent> eventList)
		{
			List<GUIContent> list = new List<GUIContent>();
			list.Add(new GUIContent(Strings.get_Label_None()));
			List<GUIContent> list2 = list;
			using (List<SkillEvent>.Enumerator enumerator = eventList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					list2.Add(new GUIContent(current.get_Name()));
				}
			}
			return list2.ToArray();
		}
		public static bool EventListContainsEventName(List<SkillEvent> eventList, string fsmEventName)
		{
			using (List<SkillEvent>.Enumerator enumerator = eventList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					if (current.get_Name() == fsmEventName)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
