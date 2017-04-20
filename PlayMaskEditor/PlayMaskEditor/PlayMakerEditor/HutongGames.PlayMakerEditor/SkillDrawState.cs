using HutongGames.PlayMaker;
using System;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	internal static class FsmDrawState
	{
		public static DrawState GetDrawState(Skill fsm)
		{
			if (fsm == null || !fsm.get_Active() || fsm.get_Finished())
			{
				return DrawState.Normal;
			}
			return FsmDrawState.GetDrawState(fsm, false, fsm.get_Active(), Skill.get_BreakAtFsm() == fsm, false);
		}
		public static DrawState GetDrawState(Skill fsm, SkillState state, SkillStateAction action)
		{
			if (fsm == null)
			{
				return DrawState.Normal;
			}
			if (!Application.get_isPlaying())
			{
				return DrawState.Normal;
			}
			if (!state.get_Active() || !action.get_Active())
			{
				return DrawState.Normal;
			}
			if (GameStateTracker.CurrentState == GameState.Break)
			{
				return DrawState.Normal;
			}
			if (GameStateTracker.CurrentState == GameState.Paused)
			{
				return DrawState.Paused;
			}
			if (GameStateTracker.CurrentState == GameState.Running)
			{
				return DrawState.Active;
			}
			if (GameStateTracker.CurrentState == GameState.Error)
			{
				return DrawState.Error;
			}
			return DrawState.Normal;
		}
		public static DrawState GetFsmStateDrawState(Skill fsm, SkillState state, bool selected)
		{
			bool active = fsm.get_ActiveState() == state && fsm.get_Active();
			bool isBreakpoint = Skill.get_BreakAtState() == state;
			return FsmDrawState.GetDrawState(fsm, selected, active, isBreakpoint, false);
		}
		public static DrawState GetFsmTransitionDrawState(Skill fsm, SkillTransition transition, bool selected)
		{
			bool active = false;
			if (fsm.get_SwitchedState() || Skill.get_BreakAtFsm() == fsm)
			{
				active = (fsm.get_LastTransition() == transition && fsm.get_Active());
			}
			return FsmDrawState.GetDrawState(fsm, selected, active, false, false);
		}
		private static DrawState GetDrawState(Skill fsm, bool selected, bool active, bool isBreakpoint, bool invalid)
		{
			if (invalid)
			{
				return DrawState.Error;
			}
			switch (GameStateTracker.CurrentState)
			{
			case GameState.Running:
				if (active)
				{
					return DrawState.Active;
				}
				break;
			case GameState.Break:
				if (Skill.get_BreakAtFsm() == fsm && isBreakpoint)
				{
					return DrawState.Breakpoint;
				}
				if (active)
				{
					return DrawState.Paused;
				}
				break;
			case GameState.Paused:
				if (active)
				{
					return DrawState.Paused;
				}
				break;
			case GameState.Error:
				if (Skill.get_BreakAtFsm() == fsm && isBreakpoint)
				{
					return DrawState.Error;
				}
				if (active)
				{
					return DrawState.Paused;
				}
				break;
			}
			if (!selected)
			{
				return DrawState.Normal;
			}
			return DrawState.Selected;
		}
	}
}
