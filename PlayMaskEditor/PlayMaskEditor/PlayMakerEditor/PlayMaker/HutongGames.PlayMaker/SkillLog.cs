using System;
using System.Collections.Generic;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	public class SkillLog
	{
		private const int MaxLogSize = 100000;
		private static readonly List<SkillLog> Logs;
		private static bool loggingEnabled;
		private List<SkillLogEntry> entries = new List<SkillLogEntry>();
		public static bool LoggingEnabled
		{
			get
			{
				return SkillLog.loggingEnabled;
			}
			set
			{
				SkillLog.loggingEnabled = value;
			}
		}
		public static bool MirrorDebugLog
		{
			get;
			set;
		}
		public static bool EnableDebugFlow
		{
			get;
			set;
		}
		public Skill Fsm
		{
			get;
			private set;
		}
		public List<SkillLogEntry> Entries
		{
			get
			{
				return this.entries;
			}
		}
		public bool Resized
		{
			get;
			set;
		}
		static SkillLog()
		{
			SkillLog.Logs = new List<SkillLog>();
			SkillLog.loggingEnabled = true;
			SkillLog.loggingEnabled = !Application.get_isEditor();
		}
		private SkillLog(Skill fsm)
		{
			this.Fsm = fsm;
		}
		public static SkillLog GetLog(Skill fsm)
		{
			if (fsm == null)
			{
				return null;
			}
			using (List<SkillLog>.Enumerator enumerator = SkillLog.Logs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillLog current = enumerator.get_Current();
					if (current.Fsm == fsm)
					{
						return current;
					}
				}
			}
			SkillLog fsmLog = new SkillLog(fsm);
			SkillLog.Logs.Add(fsmLog);
			return fsmLog;
		}
		public static void ClearLogs()
		{
			using (List<SkillLog>.Enumerator enumerator = SkillLog.Logs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillLog current = enumerator.get_Current();
					current.Clear();
				}
			}
		}
		private void AddEntry(SkillLogEntry entry, bool sendToUnityLog = false)
		{
			entry.Log = this;
			entry.Time = SkillTime.RealtimeSinceStartup;
			entry.FrameCount = Time.get_frameCount();
			if (SkillLog.IsCollisionEvent(entry.Event))
			{
				entry.GameObject = entry.Fsm.CollisionGO;
				entry.GameObjectName = entry.Fsm.CollisionName;
			}
			if (SkillLog.IsTriggerEvent(entry.Event))
			{
				entry.GameObject = entry.Fsm.TriggerGO;
				entry.GameObjectName = entry.Fsm.TriggerName;
			}
			if (SkillLog.IsCollision2DEvent(entry.Event))
			{
				entry.GameObject = entry.Fsm.Collision2dGO;
				entry.GameObjectName = entry.Fsm.Collision2dName;
			}
			if (SkillLog.IsTrigger2DEvent(entry.Event))
			{
				entry.GameObject = entry.Fsm.Trigger2dGO;
				entry.GameObjectName = entry.Fsm.Trigger2dName;
			}
			this.entries.Add(entry);
			if (this.entries.get_Count() > 100000)
			{
				this.entries.RemoveRange(0, 10000);
				this.Resized = true;
			}
			switch (entry.LogType)
			{
			case SkillLogType.Warning:
				Debug.LogWarning(this.FormatUnityLogString(entry.Text));
				return;
			case SkillLogType.Error:
				Debug.LogError(this.FormatUnityLogString(entry.Text));
				return;
			default:
				if ((SkillLog.MirrorDebugLog || sendToUnityLog) && entry.LogType != SkillLogType.Transition)
				{
					Debug.Log(this.FormatUnityLogString(entry.Text));
				}
				return;
			}
		}
		private static bool IsCollisionEvent(SkillEvent fsmEvent)
		{
			return fsmEvent != null && (fsmEvent == SkillEvent.CollisionEnter || fsmEvent == SkillEvent.CollisionStay || fsmEvent == SkillEvent.CollisionExit);
		}
		private static bool IsTriggerEvent(SkillEvent fsmEvent)
		{
			return fsmEvent != null && (fsmEvent == SkillEvent.TriggerEnter || fsmEvent == SkillEvent.TriggerStay || fsmEvent == SkillEvent.TriggerExit);
		}
		private static bool IsCollision2DEvent(SkillEvent fsmEvent)
		{
			return fsmEvent != null && (fsmEvent == SkillEvent.CollisionEnter2D || fsmEvent == SkillEvent.CollisionStay2D || fsmEvent == SkillEvent.CollisionExit2D);
		}
		private static bool IsTrigger2DEvent(SkillEvent fsmEvent)
		{
			return fsmEvent != null && (fsmEvent == SkillEvent.TriggerEnter2D || fsmEvent == SkillEvent.TriggerStay2D || fsmEvent == SkillEvent.TriggerExit2D);
		}
		public void LogEvent(SkillEvent fsmEvent, SkillState state)
		{
			SkillLogEntry entry = new SkillLogEntry
			{
				Log = this,
				LogType = SkillLogType.Event,
				State = state,
				SentByState = Skill.EventData.SentByState,
				Action = Skill.EventData.SentByAction,
				Event = fsmEvent,
				Text = "EVENT: " + fsmEvent.Name
			};
			this.AddEntry(entry, false);
		}
		public void LogSendEvent(SkillState state, SkillEvent fsmEvent, SkillEventTarget eventTarget)
		{
			if (state == null || fsmEvent == null || fsmEvent.IsSystemEvent)
			{
				return;
			}
			SkillLogEntry entry = new SkillLogEntry
			{
				Log = this,
				LogType = SkillLogType.SendEvent,
				State = state,
				Event = fsmEvent,
				Text = "SEND EVENT: " + fsmEvent.Name,
				EventTarget = new SkillEventTarget(eventTarget)
			};
			this.AddEntry(entry, false);
		}
		public void LogExitState(SkillState state)
		{
			if (state == null)
			{
				return;
			}
			SkillLogEntry fsmLogEntry = new SkillLogEntry
			{
				Log = this,
				LogType = SkillLogType.ExitState,
				State = state,
				Text = string.Concat(new string[]
				{
					"EXIT: ",
					state.Name,
					" [",
					string.Format("{0:f2}", SkillTime.RealtimeSinceStartup - state.RealStartTime),
					"s]"
				})
			};
			if (SkillLog.EnableDebugFlow && state.Fsm.EnableDebugFlow && !PlayMakerFSM.ApplicationIsQuitting)
			{
				fsmLogEntry.FsmVariablesCopy = new SkillVariables(state.Fsm.Variables);
				fsmLogEntry.GlobalVariablesCopy = new SkillVariables(SkillVariables.GlobalVariables);
			}
			this.AddEntry(fsmLogEntry, false);
		}
		public void LogEnterState(SkillState state)
		{
			if (state == null)
			{
				return;
			}
			SkillLogEntry fsmLogEntry = new SkillLogEntry
			{
				Log = this,
				LogType = SkillLogType.EnterState,
				State = state,
				Text = "ENTER: " + state.Name
			};
			if (SkillLog.EnableDebugFlow && state.Fsm.EnableDebugFlow)
			{
				fsmLogEntry.FsmVariablesCopy = new SkillVariables(state.Fsm.Variables);
				fsmLogEntry.GlobalVariablesCopy = new SkillVariables(SkillVariables.GlobalVariables);
			}
			this.AddEntry(fsmLogEntry, false);
		}
		public void LogTransition(SkillState fromState, SkillTransition transition)
		{
			SkillLogEntry entry = new SkillLogEntry
			{
				Log = this,
				LogType = SkillLogType.Transition,
				State = fromState,
				Transition = transition
			};
			this.AddEntry(entry, false);
		}
		public void LogBreak()
		{
			SkillLogEntry entry = new SkillLogEntry
			{
				Log = this,
				LogType = SkillLogType.Break,
				State = SkillExecutionStack.ExecutingState,
				Text = "BREAK: " + SkillExecutionStack.ExecutingStateName
			};
			Debug.Log("BREAK: " + this.FormatUnityLogString("Breakpoint"));
			this.AddEntry(entry, false);
		}
		public void LogAction(SkillLogType logType, string text, bool sendToUnityLog = false)
		{
			if (SkillExecutionStack.ExecutingAction != null)
			{
				SkillLogEntry entry = new SkillLogEntry
				{
					Log = this,
					LogType = logType,
					State = SkillExecutionStack.ExecutingState,
					Action = SkillExecutionStack.ExecutingAction,
					Text = SkillUtility.StripNamespace(SkillExecutionStack.ExecutingAction.ToString()) + " : " + text
				};
				this.AddEntry(entry, sendToUnityLog);
				return;
			}
			switch (logType)
			{
			case SkillLogType.Info:
				Debug.Log(text);
				return;
			case SkillLogType.Warning:
				Debug.LogWarning(text);
				return;
			case SkillLogType.Error:
				Debug.LogError(text);
				return;
			default:
				Debug.Log(text);
				return;
			}
		}
		public void Log(SkillLogType logType, string text)
		{
			SkillLogEntry entry = new SkillLogEntry
			{
				Log = this,
				LogType = logType,
				State = SkillExecutionStack.ExecutingState,
				Text = text
			};
			this.AddEntry(entry, false);
		}
		public void LogStart(SkillState startState)
		{
			SkillLogEntry entry = new SkillLogEntry
			{
				Log = this,
				LogType = SkillLogType.Start,
				State = startState,
				Text = "START"
			};
			this.AddEntry(entry, false);
		}
		public void LogStop()
		{
			SkillLogEntry entry = new SkillLogEntry
			{
				Log = this,
				LogType = SkillLogType.Stop,
				Text = "STOP"
			};
			this.AddEntry(entry, false);
		}
		public void Log(string text)
		{
			this.Log(SkillLogType.Info, text);
		}
		public void LogWarning(string text)
		{
			this.Log(SkillLogType.Warning, text);
		}
		public void LogError(string text)
		{
			this.Log(SkillLogType.Error, text);
		}
		private string FormatUnityLogString(string text)
		{
			string text2 = Skill.GetFullFsmLabel(this.Fsm);
			if (SkillExecutionStack.ExecutingState != null)
			{
				text2 = text2 + " : " + SkillExecutionStack.ExecutingStateName;
			}
			if (SkillExecutionStack.ExecutingAction != null)
			{
				text2 += SkillExecutionStack.ExecutingAction.Name;
			}
			return text2 + " : " + text;
		}
		public void Clear()
		{
			if (this.entries != null)
			{
				this.entries.Clear();
			}
		}
		public void OnDestroy()
		{
			SkillLog.Logs.Remove(this);
			this.Clear();
			this.entries = null;
			this.Fsm = null;
		}
	}
}
