using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	public static class DebugFlow
	{
		public delegate void LogEntrySelectedHandler(SkillLogEntry logEntry);
		public static DebugFlow.LogEntrySelectedHandler LogEntrySelected;
		private static SkillLogEntry lastEnter;
		private static int lastEnterIndex;
		private static readonly Dictionary<Skill, SkillVariables> variablesCache = new Dictionary<Skill, SkillVariables>();
		private static SkillVariables globalVariablesCache;
		public static bool Active
		{
			get;
			private set;
		}
		public static SkillLog SelectedLog
		{
			get;
			private set;
		}
		public static SkillLogEntry SelectedLogEntry
		{
			get;
			private set;
		}
		public static int SelectedLogEntryIndex
		{
			get;
			private set;
		}
		public static float CurrentDebugTime
		{
			get;
			private set;
		}
		public static SkillState DebugState
		{
			get;
			private set;
		}
		private static int StartedDebugFrame
		{
			get;
			set;
		}
		private static int CurrentDebugFrame
		{
			get;
			set;
		}
		public static bool ActiveAndScrubbing
		{
			get
			{
				return DebugFlow.Active && DebugFlow.SelectedLogEntryIndex < DebugFlow.lastEnterIndex;
			}
		}
		public static void Start(Skill fsm)
		{
			DebugFlow.UpdateTime();
			DebugFlow.Active = true;
			DebugFlow.SyncFsmLog(fsm);
			DebugFlow.StartedDebugFrame = Time.get_frameCount();
			DebugFlow.variablesCache.Clear();
			using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					if (current.EnableDebugFlow)
					{
						DebugFlow.variablesCache.Add(current, new SkillVariables(current.get_Variables()));
					}
				}
			}
			DebugFlow.globalVariablesCache = new SkillVariables(SkillVariables.get_GlobalVariables());
		}
		public static void SyncFsmLog(Skill fsm)
		{
			if (fsm == null)
			{
				return;
			}
			DebugFlow.SelectedLog = fsm.get_MyLog();
			if (DebugFlow.Active && DebugFlow.SelectedLog != null && DebugFlow.SelectedLog.get_Entries() != null)
			{
				DebugFlow.SelectMostRecentLogEntry(DebugFlow.CurrentDebugFrame);
				DebugFlow.lastEnter = DebugFlow.SelectedLogEntry;
				DebugFlow.lastEnterIndex = DebugFlow.SelectedLog.get_Entries().IndexOf(DebugFlow.lastEnter);
			}
		}
		public static void Stop()
		{
			if (!DebugFlow.Active || !Application.get_isPlaying())
			{
				return;
			}
			DebugFlow.SelectedLogEntry = null;
			DebugFlow.Active = false;
			List<Skill> list = new List<Skill>(SkillEditor.FsmList);
			using (List<Skill>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					if (current != null && current.EnableDebugFlow)
					{
						SkillVariables fsmVariables;
						DebugFlow.variablesCache.TryGetValue(current, ref fsmVariables);
						if (fsmVariables != null)
						{
							current.get_Variables().ApplyVariableValues(fsmVariables);
						}
						else
						{
							Debug.LogWarning("DebugFlow: Missing Cached Variables...");
						}
					}
				}
			}
			SkillVariables.get_GlobalVariables().ApplyVariableValues(DebugFlow.globalVariablesCache);
			DebugFlow.variablesCache.Clear();
			DebugFlow.globalVariablesCache = null;
		}
		public static void Cleanup()
		{
			DebugFlow.variablesCache.Clear();
			DebugFlow.globalVariablesCache = null;
			DebugFlow.SelectedLog = null;
			DebugFlow.SelectedLogEntry = null;
			DebugFlow.DebugState = null;
			DebugFlow.lastEnter = null;
		}
		public static void UpdateTime()
		{
			DebugFlow.CurrentDebugTime = SkillTime.get_RealtimeSinceStartup();
			DebugFlow.CurrentDebugFrame = Time.get_frameCount();
		}
		public static void Refresh()
		{
			DebugFlow.SetDebugTime(DebugFlow.CurrentDebugTime);
		}
		public static void SetDebugTime(float time)
		{
			DebugFlow.CurrentDebugTime = time;
			using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					if (current != null)
					{
						SkillLogEntry logEntry = DebugFlow.FindClosestLogEntry(current.get_MyLog(), time);
						if (SkillEditor.SelectedFsm == current)
						{
							DebugFlow.SelectLogEntry(logEntry, false);
						}
						else
						{
							DebugFlow.RestoreNearestVariables(logEntry);
						}
					}
				}
			}
			SkillLogger.SetDebugFlowTime(time);
		}
		public static void SelectLogEntry(SkillLogEntry logEntry, bool updateTime = true)
		{
			if (logEntry != null)
			{
				DebugFlow.SelectedLog = logEntry.get_Log();
				DebugFlow.SelectedLogEntry = logEntry;
				DebugFlow.DebugState = logEntry.get_State();
				DebugFlow.SelectedLogEntryIndex = logEntry.GetIndex();
				if (updateTime)
				{
					DebugFlow.CurrentDebugTime = logEntry.get_Time();
					DebugFlow.CurrentDebugFrame = logEntry.get_FrameCount();
				}
				SkillEditor.SelectState(logEntry.get_State(), true);
				if (logEntry.get_Action() != null)
				{
					SkillEditor.SelectAction(logEntry.get_Action(), true);
				}
				if (FsmEditorSettings.EnableDebugFlow && DebugFlow.SelectedLog.get_Fsm().EnableDebugFlow && DebugFlow.SelectedLogEntryIndex < DebugFlow.lastEnterIndex)
				{
					DebugFlow.RestoreNearestVariables(logEntry);
				}
				if (DebugFlow.LogEntrySelected != null)
				{
					DebugFlow.LogEntrySelected(logEntry);
				}
				SkillEditor.Repaint(true);
			}
		}
		private static void RestoreNearestVariables(SkillLogEntry logEntry)
		{
			if (logEntry == null)
			{
				return;
			}
			if (logEntry.get_LogType() == 6 || logEntry.get_LogType() == 5)
			{
				DebugFlow.RestoreVariables(logEntry);
				return;
			}
			if (logEntry.get_Event() == SkillEvent.get_Finished())
			{
				SkillLogEntry fsmLogEntry = DebugFlow.FindNextLogEntry(logEntry, new SkillLogType[]
				{
					5
				});
				if (fsmLogEntry != null)
				{
					DebugFlow.RestoreVariables(fsmLogEntry);
					return;
				}
			}
			else
			{
				if (DebugFlow.SelectedLogEntryIndex == 0)
				{
					SkillLogEntry fsmLogEntry2 = DebugFlow.FindNextLogEntry(logEntry, new SkillLogType[]
					{
						6
					});
					if (fsmLogEntry2 != null)
					{
						DebugFlow.RestoreVariables(fsmLogEntry2);
						return;
					}
				}
				else
				{
					SkillLogEntry fsmLogEntry3 = DebugFlow.FindPrevLogEntry(logEntry, 6);
					if (fsmLogEntry3 != null)
					{
						DebugFlow.RestoreVariables(fsmLogEntry3);
					}
				}
			}
		}
		private static void RestoreVariables(SkillLogEntry logEntry)
		{
			if (logEntry == null)
			{
				Debug.Log("Bad Log Entry!");
				return;
			}
			Skill fsm = logEntry.get_Fsm();
			if (!fsm.EnableDebugFlow)
			{
				return;
			}
			if (fsm == null)
			{
				Debug.Log("Fsm == null!!");
				return;
			}
			if (logEntry.get_FsmVariablesCopy() != null)
			{
				fsm.get_Variables().ApplyVariableValues(logEntry.get_FsmVariablesCopy());
			}
			else
			{
				Debug.LogError("Missing Local Variables Cache!");
			}
			if (logEntry.get_GlobalVariablesCopy() != null)
			{
				SkillVariables.get_GlobalVariables().ApplyVariableValues(logEntry.get_GlobalVariablesCopy());
			}
			else
			{
				Debug.LogError("Missing global Variables Cache!");
			}
			if (SkillEditor.SelectedFsm == fsm)
			{
				SkillEditor.VariableManager.UpdateView();
				GlobalVariablesWindow.UpdateView();
				SkillEditor.Repaint(false);
			}
		}
		private static void SelectMostRecentLogEntry(int fromFrame)
		{
			DebugFlow.SelectLogEntry(DebugFlow.FindMostRecentLogEntry(fromFrame), false);
		}
		public static void SelectPrevTransition()
		{
			DebugFlow.SelectLogEntry(DebugFlow.FindPrevLogEntry(DebugFlow.SelectedLogEntry, 6), true);
		}
		public static void SelectNextTransition()
		{
			DebugFlow.SelectLogEntry(DebugFlow.FindNextLogEntry(DebugFlow.SelectedLogEntry, new SkillLogType[]
			{
				6,
				7
			}), true);
		}
		private static SkillLogEntry FindClosestLogEntry(SkillLog fsmLog, float time)
		{
			SkillLogEntry result = null;
			using (List<SkillLogEntry>.Enumerator enumerator = fsmLog.get_Entries().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillLogEntry current = enumerator.get_Current();
					if (current.get_Time() > time)
					{
						break;
					}
					if (current.get_LogType() == 6)
					{
						result = current;
					}
				}
			}
			return result;
		}
		private static SkillLogEntry FindMostRecentLogEntry(int fromFrame)
		{
			return DebugFlow.FindMostRecentLogEntry(DebugFlow.SelectedLog, fromFrame);
		}
		private static SkillLogEntry FindMostRecentLogEntry(SkillLog fsmLog, int fromFrame)
		{
			if (fsmLog == null || fsmLog.get_Entries() == null)
			{
				return null;
			}
			SkillLogEntry result = null;
			using (List<SkillLogEntry>.Enumerator enumerator = fsmLog.get_Entries().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillLogEntry current = enumerator.get_Current();
					if (current.get_LogType() == 6 || current.get_LogType() == 8 || current.get_LogType() == 7)
					{
						result = current;
					}
				}
			}
			return result;
		}
		private static SkillLogEntry FindPrevLogEntry(SkillLogEntry fromEntry, SkillLogType logType = 6)
		{
			if (fromEntry == null)
			{
				return null;
			}
			SkillLog log = fromEntry.get_Log();
			if (log == null || log.get_Entries() == null)
			{
				return null;
			}
			SkillLogEntry result = null;
			using (List<SkillLogEntry>.Enumerator enumerator = log.get_Entries().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillLogEntry current = enumerator.get_Current();
					if (current == fromEntry)
					{
						break;
					}
					if (current.get_LogType() == logType)
					{
						result = current;
					}
				}
			}
			return result;
		}
		private static SkillLogEntry FindNextLogEntry(SkillLogEntry fromEntry, params SkillLogType[] logTypes)
		{
			SkillLog log = fromEntry.get_Log();
			if (log == null)
			{
				return null;
			}
			int num = DebugFlow.SelectedLog.get_Entries().IndexOf(fromEntry);
			for (int i = num + 1; i < log.get_Entries().get_Count(); i++)
			{
				SkillLogEntry fsmLogEntry = log.get_Entries().get_Item(i);
				for (int j = 0; j < logTypes.Length; j++)
				{
					SkillLogType fsmLogType = logTypes[j];
					if (fsmLogEntry.get_LogType() == fsmLogType)
					{
						return fsmLogEntry;
					}
				}
			}
			return null;
		}
		public static SkillLogEntry GetLastTransition()
		{
			return DebugFlow.FindPrevLogEntry(DebugFlow.SelectedLogEntry, 4);
		}
	}
}
