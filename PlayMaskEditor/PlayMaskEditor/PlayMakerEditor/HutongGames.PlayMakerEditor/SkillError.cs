using HutongGames.PlayMaker;
using System;
using System.ComponentModel;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	internal class FsmError
	{
		public enum ErrorType
		{
			general,
			missingRequiredComponent,
			requiredField,
			missingTransitionEvent,
			eventNotGlobal,
			missingVariable
		}
		public Skill Fsm;
		public SkillState State;
		public SkillStateAction Action;
		public string Parameter;
		public SkillTransition Transition;
		public string ErrorString;
		public GameObject GameObject;
		public Type ObjectType;
		public bool RuntimeError;
		public FsmError.ErrorType Type;
		public string info;
		public FsmError()
		{
		}
		public FsmError(SkillState state, SkillStateAction action, string errorString)
		{
			this.Action = action;
			this.State = state;
			this.Fsm = state.get_Fsm();
			this.ErrorString = errorString;
		}
		public FsmError(SkillState state, SkillStateAction action, string parameter, string errorString)
		{
			this.Action = action;
			this.Parameter = parameter;
			this.State = state;
			this.Fsm = state.get_Fsm();
			this.ErrorString = errorString;
		}
		public FsmError(SkillState state, SkillTransition transition, string errorString)
		{
			this.State = state;
			this.Fsm = state.get_Fsm();
			this.Transition = transition;
			this.ErrorString = errorString;
		}
		public bool SameAs(FsmError error)
		{
			return error != null && this.Fsm == error.Fsm && this.State == error.State && this.Action == error.Action && !(this.Parameter != error.Parameter) && this.Transition == error.Transition && !(this.ErrorString != error.ErrorString) && this.Type == error.Type && this.ObjectType == error.ObjectType;
		}
		[Localizable(false)]
		public override string ToString()
		{
			string text = Labels.GetFullFsmLabel(this.Fsm);
			if (this.State != null)
			{
				text = text + " : " + this.State.get_Name();
			}
			if (this.Action != null)
			{
				text = text + " : " + Labels.StripNamespace(this.Action.ToString());
			}
			if (this.Parameter != null)
			{
				text = text + " : " + this.Parameter;
			}
			return text;
		}
	}
}
