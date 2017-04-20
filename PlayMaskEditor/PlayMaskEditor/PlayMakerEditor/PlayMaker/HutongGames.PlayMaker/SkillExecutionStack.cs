using System;
using System.Collections.Generic;
namespace HutongGames.PlayMaker
{
	public static class SkillExecutionStack
	{
		private static readonly Stack<Skill> fsmExecutionStack = new Stack<Skill>(256);
		public static Skill ExecutingFsm
		{
			get
			{
				if (SkillExecutionStack.fsmExecutionStack.get_Count() <= 0)
				{
					return null;
				}
				return SkillExecutionStack.fsmExecutionStack.Peek();
			}
		}
		public static SkillState ExecutingState
		{
			get
			{
				if (SkillExecutionStack.ExecutingFsm == null)
				{
					return null;
				}
				return SkillExecutionStack.ExecutingFsm.ActiveState;
			}
		}
		public static string ExecutingStateName
		{
			get
			{
				if (SkillExecutionStack.ExecutingFsm == null)
				{
					return "[none]";
				}
				return SkillExecutionStack.ExecutingFsm.ActiveStateName;
			}
		}
		public static SkillStateAction ExecutingAction
		{
			get
			{
				if (SkillExecutionStack.ExecutingState == null)
				{
					return null;
				}
				return SkillExecutionStack.ExecutingState.ActiveAction;
			}
		}
		public static int StackCount
		{
			get
			{
				return SkillExecutionStack.fsmExecutionStack.get_Count();
			}
		}
		public static int MaxStackCount
		{
			get;
			private set;
		}
		public static void PushFsm(Skill executingFsm)
		{
			SkillExecutionStack.fsmExecutionStack.Push(executingFsm);
			if (SkillExecutionStack.fsmExecutionStack.get_Count() > SkillExecutionStack.MaxStackCount)
			{
				SkillExecutionStack.MaxStackCount = SkillExecutionStack.fsmExecutionStack.get_Count();
			}
		}
		public static void PopFsm()
		{
			SkillExecutionStack.fsmExecutionStack.Pop();
		}
		public static string GetDebugString()
		{
			string text = "";
			text = text + "\nExecutingFsm: " + SkillExecutionStack.ExecutingFsm;
			return text + "\nExecutingState: " + SkillExecutionStack.ExecutingStateName;
		}
	}
}
