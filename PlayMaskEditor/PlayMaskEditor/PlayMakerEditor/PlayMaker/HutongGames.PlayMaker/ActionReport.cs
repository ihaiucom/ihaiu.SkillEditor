using System;
using System.Collections.Generic;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	public class ActionReport
	{
		public static readonly List<ActionReport> ActionReportList = new List<ActionReport>();
		public static int InfoCount;
		public static int ErrorCount;
		public PlayMakerFSM fsm;
		public SkillState state;
		public SkillStateAction action;
		public int actionIndex;
		public string logText;
		public bool isError;
		public string parameter;
		public static void Start()
		{
			ActionReport.ActionReportList.Clear();
			ActionReport.InfoCount = 0;
			ActionReport.ErrorCount = 0;
		}
		public static ActionReport Log(PlayMakerFSM fsm, SkillState state, SkillStateAction action, int actionIndex, string parameter, string logLine, bool isError = false)
		{
			if (!PlayMakerGlobals.IsEditor)
			{
				return null;
			}
			ActionReport actionReport = new ActionReport
			{
				fsm = fsm,
				state = state,
				action = action,
				actionIndex = actionIndex,
				parameter = parameter,
				logText = logLine,
				isError = isError
			};
			if (!ActionReport.ActionReportContains(actionReport))
			{
				ActionReport.ActionReportList.Add(actionReport);
				ActionReport.InfoCount++;
				return actionReport;
			}
			return null;
		}
		private static bool ActionReportContains(ActionReport report)
		{
			using (List<ActionReport>.Enumerator enumerator = ActionReport.ActionReportList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ActionReport current = enumerator.get_Current();
					if (current.SameAs(report))
					{
						return true;
					}
				}
			}
			return false;
		}
		private bool SameAs(ActionReport actionReport)
		{
			return object.ReferenceEquals(actionReport.fsm, this.fsm) && actionReport.state == this.state && actionReport.actionIndex == this.actionIndex && actionReport.logText == this.logText && actionReport.isError == this.isError && actionReport.parameter == this.parameter;
		}
		public static void LogWarning(PlayMakerFSM fsm, SkillState state, SkillStateAction action, int actionIndex, string parameter, string logLine)
		{
			ActionReport.Log(fsm, state, action, actionIndex, parameter, logLine, true);
			Debug.LogWarning(SkillUtility.GetPath(state, action) + logLine, fsm);
			ActionReport.ErrorCount++;
		}
		public static void LogError(PlayMakerFSM fsm, SkillState state, SkillStateAction action, int actionIndex, string parameter, string logLine)
		{
			ActionReport.Log(fsm, state, action, actionIndex, parameter, logLine, true);
			Debug.LogError(SkillUtility.GetPath(state, action) + logLine, fsm);
			ActionReport.ErrorCount++;
		}
		public static void LogError(PlayMakerFSM fsm, SkillState state, SkillStateAction action, int actionIndex, string logLine)
		{
			ActionReport.Log(fsm, state, action, actionIndex, logLine, "", true);
			Debug.LogError(SkillUtility.GetPath(state, action) + logLine, fsm);
			ActionReport.ErrorCount++;
		}
		public static void Clear()
		{
			ActionReport.ActionReportList.Clear();
		}
		public static void Remove(PlayMakerFSM fsm)
		{
			ActionReport.ActionReportList.RemoveAll((ActionReport x) => x.fsm == fsm);
		}
		public static int GetCount()
		{
			return ActionReport.ActionReportList.get_Count();
		}
	}
}
