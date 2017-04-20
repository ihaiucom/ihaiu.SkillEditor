using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	[Serializable]
	internal class FsmDebugger
	{
		internal enum FsmStepMode
		{
			StepFrame,
			StepToStateChange,
			StepToAnyStateChange
		}
		private static FsmDebugger instance;
		[SerializeField]
		private FsmDebugger.FsmStepMode stepMode;
		public static FsmDebugger Instance
		{
			get
			{
				if (FsmDebugger.instance == null)
				{
					FsmDebugger.instance = new FsmDebugger();
					FsmDebugger.instance.Init();
				}
				return FsmDebugger.instance;
			}
		}
		public FsmDebugger.FsmStepMode StepMode
		{
			get
			{
				return this.stepMode;
			}
			set
			{
				this.stepMode = value;
			}
		}
		public void Init()
		{
			Application.remove_logMessageReceived(new Application.LogCallback(this.HandleLog));
			Application.add_logMessageReceived(new Application.LogCallback(this.HandleLog));
		}
		public void HandleLog(string logEntry, string stackTrace, LogType type)
		{
			if (type != null || SkillExecutionStack.get_ExecutingFsm() == null || GameStateTracker.CurrentState == GameState.Stopped || !stackTrace.Contains("HutongGames.PlayMaker") || stackTrace.Contains("HutongGames.PlayMaker.Fsm:LogError(String)"))
			{
				return;
			}
			SkillExecutionStack.get_ExecutingFsm().DoBreakError(logEntry);
			FsmDebugger.DoBreak();
		}
		public void Update()
		{
			if (GameStateTracker.CurrentState == GameState.Stopped)
			{
				return;
			}
			using (List<PlayMakerFSM>.Enumerator enumerator = PlayMakerFSM.get_FsmList().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PlayMakerFSM current = enumerator.get_Current();
					if (current != null)
					{
						this.Watch(current.get_Fsm());
					}
				}
			}
			if (Skill.get_HitBreakpoint())
			{
				FsmDebugger.DoBreak();
			}
		}
		public void Watch(Skill fsm)
		{
			if (fsm.get_SwitchedState())
			{
				SkillEditor.RepaintAll();
				if (FsmEditorSettings.SelectStateOnActivated && SkillEditor.Instance != null && SkillEditor.SelectedFsm == fsm)
				{
					SkillEditor.SelectState(fsm.get_ActiveState(), false);
				}
				fsm.set_SwitchedState(false);
			}
		}
		private static void DoBreak()
		{
			if (Skill.get_IsErrorBreak())
			{
				FsmErrorChecker.AddRuntimeError(Skill.get_LastError());
			}
			if (FsmEditorSettings.JumpToBreakpoint && SkillEditor.Instance != null)
			{
				SkillEditor.GotoBreakpoint();
			}
			Skill.set_HitBreakpoint(false);
			EditorApplication.set_isPaused(true);
		}
		public void Step()
		{
			switch (this.StepMode)
			{
			case FsmDebugger.FsmStepMode.StepFrame:
				EditorApplication.set_isPaused(false);
				EditorApplication.Step();
				Skill.set_StepToStateChange(false);
				break;
			case FsmDebugger.FsmStepMode.StepToStateChange:
				EditorApplication.set_isPaused(false);
				Skill.set_StepToStateChange(true);
				Skill.set_StepFsm(SkillEditor.SelectedFsm);
				break;
			case FsmDebugger.FsmStepMode.StepToAnyStateChange:
				EditorApplication.set_isPaused(false);
				Skill.set_StepToStateChange(true);
				Skill.set_StepFsm(null);
				break;
			}
			DebugFlow.UpdateTime();
		}
		public void ResetStep()
		{
			Skill.set_StepToStateChange(false);
		}
		public void OnDestroy()
		{
			Application.remove_logMessageReceived(new Application.LogCallback(this.HandleLog));
		}
	}
}
