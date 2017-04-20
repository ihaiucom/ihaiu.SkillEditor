using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace HutongGames.PlayMakerEditor
{
	public class SkillSearch
	{
		public const int MAX_SEARCH_DEPTH = 7;
		private static readonly Dictionary<Skill, SkillSearch> fsmCache = new Dictionary<Skill, SkillSearch>();
		private static readonly Dictionary<NamedVariable, List<SkillInfo>> globalVariablesUsage = new Dictionary<NamedVariable, List<SkillInfo>>();
		private readonly Skill fsm;
		private readonly List<string> unusedEvents = new List<string>();
		private List<SkillVariable> fsmVariables = new List<SkillVariable>();
		private readonly List<SkillVariable> unusedVariables = new List<SkillVariable>();
		private readonly Dictionary<string, List<SkillInfo>> eventUsage = new Dictionary<string, List<SkillInfo>>();
		private readonly Dictionary<NamedVariable, List<SkillInfo>> variableUsage = new Dictionary<NamedVariable, List<SkillInfo>>();
		private readonly List<NamedVariable> globalVariablesUsed = new List<NamedVariable>();
		private readonly Dictionary<SkillStateAction, List<SkillInfo>> actionUsage = new Dictionary<SkillStateAction, List<SkillInfo>>();
		private static SkillSearch lastFsmSearch;
		private static SkillInfo currentInfo;
		public static bool GlobalVariablesUsageInitialized
		{
			get;
			private set;
		}
		public static SkillSearch GetSearch(Skill fsm)
		{
			if (SkillSearch.lastFsmSearch != null && fsm == SkillSearch.lastFsmSearch.fsm)
			{
				return SkillSearch.lastFsmSearch;
			}
			SkillSearch fsmSearch;
			SkillSearch.fsmCache.TryGetValue(fsm, ref fsmSearch);
			if (fsmSearch == null)
			{
				fsmSearch = new SkillSearch(fsm);
				SkillSearch.fsmCache.Add(fsm, fsmSearch);
			}
			SkillSearch.lastFsmSearch = fsmSearch;
			return fsmSearch;
		}
		public static void Invalidate()
		{
			SkillSearch.GlobalVariablesUsageInitialized = false;
			SkillSearch.globalVariablesUsage.Clear();
			SkillSearch.fsmCache.Clear();
		}
		public static void UpdateAll()
		{
			SkillSearch.globalVariablesUsage.Clear();
			SkillSearch.GlobalVariablesUsageInitialized = true;
			using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					SkillSearch.Update(current);
				}
			}
		}
		public static void Update(Skill fsm)
		{
			if (fsm != null)
			{
				SkillSearch.GetSearch(fsm).Update();
			}
			SkillEditor.FsmInfoUpdated(fsm);
		}
		public void Clear()
		{
			List<NamedVariable> list = new List<NamedVariable>(SkillSearch.globalVariablesUsage.get_Keys());
			using (List<NamedVariable>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NamedVariable current = enumerator.get_Current();
					List<SkillInfo> globalVariablesUsageList = SkillSearch.GetGlobalVariablesUsageList(current);
					List<SkillInfo> list2 = new List<SkillInfo>(globalVariablesUsageList);
					using (List<SkillInfo>.Enumerator enumerator2 = globalVariablesUsageList.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							SkillInfo current2 = enumerator2.get_Current();
							if (current2.fsm == this.fsm)
							{
								list2.Remove(current2);
							}
						}
					}
					SkillSearch.globalVariablesUsage.set_Item(current, list2);
				}
			}
			this.eventUsage.Clear();
			this.variableUsage.Clear();
			this.globalVariablesUsed.Clear();
			this.actionUsage.Clear();
		}
		public SkillSearch(Skill fsm)
		{
			this.fsm = fsm;
			this.Update();
		}
		public void Update()
		{
			this.Clear();
			this.DoSearch();
		}
		private void DoSearch()
		{
			this.UpdateVariableList();
			this.FindEventsInGlobalTransitions();
			SkillState[] states = this.fsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				fsmState.set_Fsm(this.fsm);
				this.FindEventUsage(fsmState);
				SkillStateAction[] actions = fsmState.get_Actions();
				for (int j = 0; j < actions.Length; j++)
				{
					SkillStateAction fsmStateAction = actions[j];
					SkillSearch.currentInfo = new SkillInfo
					{
						fsm = this.fsm,
						state = fsmState,
						action = fsmStateAction
					};
					this.FindEventsSentBy(fsmStateAction);
					this.FindVariableUsage(fsmStateAction, 0);
				}
			}
			this.UpdateUnusedEvents();
			this.UpdateUnusedVariables();
		}
		public static List<SkillInfo> GetEventUsageList(Skill fsm, string eventName)
		{
			return SkillSearch.GetSearch(fsm).GetEventUsageList(eventName);
		}
		public List<SkillInfo> GetEventUsageList(string eventName)
		{
			List<SkillInfo> list;
			this.eventUsage.TryGetValue(eventName, ref list);
			if (list == null)
			{
				list = new List<SkillInfo>();
				this.eventUsage.Add(eventName, list);
			}
			return list;
		}
		private void FindEventsInGlobalTransitions()
		{
			if (this.fsm == null)
			{
				return;
			}
			SkillTransition[] globalTransitions = this.fsm.get_GlobalTransitions();
			for (int i = 0; i < globalTransitions.Length; i++)
			{
				SkillTransition fsmTransition = globalTransitions[i];
				if (fsmTransition.get_EventName() != string.Empty)
				{
					this.GetEventUsageList(fsmTransition.get_EventName()).Add(new SkillInfo
					{
						fsm = this.fsm,
						state = this.fsm.GetState(fsmTransition.get_ToState()),
						transition = fsmTransition
					});
				}
			}
		}
		private void FindEventsSentBy(SkillStateAction action)
		{
		}
		private void FindEventUsage(SkillState state)
		{
			SkillTransition[] transitions = state.get_Transitions();
			for (int i = 0; i < transitions.Length; i++)
			{
				SkillTransition fsmTransition = transitions[i];
				if (fsmTransition.get_EventName() != string.Empty)
				{
					this.GetEventUsageList(fsmTransition.get_EventName()).Add(new SkillInfo
					{
						fsm = this.fsm,
						state = state,
						transition = fsmTransition
					});
				}
			}
		}
		public static int GetEventUseCount(Skill fsm, string eventname)
		{
			return SkillSearch.GetSearch(fsm).GetEventUseCount(eventname);
		}
		public int GetEventUseCount(string eventName)
		{
			return this.GetEventUsageList(eventName).get_Count();
		}
		private void UpdateUnusedEvents()
		{
			this.unusedEvents.Clear();
			SkillEvent[] events = this.fsm.get_Events();
			for (int i = 0; i < events.Length; i++)
			{
				SkillEvent fsmEvent = events[i];
				if (this.GetEventUseCount(fsmEvent.get_Name()) == 0)
				{
					this.unusedEvents.Add(fsmEvent.get_Name());
				}
			}
		}
		public static List<string> GetUnusedEvents(Skill fsm)
		{
			return SkillSearch.GetSearch(fsm).GetUnusedEvents();
		}
		public List<string> GetUnusedEvents()
		{
			return this.unusedEvents;
		}
		public void UpdateVariableList()
		{
			this.fsmVariables = ((SkillEditor.SelectedFsm != null) ? SkillVariable.GetFsmVariableList(SkillEditor.SelectedFsm.get_OwnerObject()) : new List<SkillVariable>());
		}
		public static List<SkillInfo> GetVariableUsageList(Skill fsm, NamedVariable variable)
		{
			return SkillSearch.GetSearch(fsm).GetVariableUsageList(variable);
		}
		public static List<SkillInfo> GetGlobalVariablesUsageList(NamedVariable variable)
		{
			List<SkillInfo> list;
			SkillSearch.globalVariablesUsage.TryGetValue(variable, ref list);
			if (list == null)
			{
				list = new List<SkillInfo>();
				SkillSearch.globalVariablesUsage.Add(variable, list);
			}
			return list;
		}
		public static int GetGlobalVariablesUsageCount(NamedVariable variable)
		{
			List<SkillInfo> list;
			SkillSearch.globalVariablesUsage.TryGetValue(variable, ref list);
			if (list == null)
			{
				return 0;
			}
			return list.get_Count();
		}
		public static List<NamedVariable> GetGlobalVariablesUsed(Skill fsm)
		{
			return SkillSearch.GetSearch(fsm).globalVariablesUsed;
		}
		public static int GetGlobalVariablesUsedCount(Skill fsm)
		{
			return SkillSearch.GetSearch(fsm).globalVariablesUsed.get_Count();
		}
		public static int GetVariableUseCount(Skill fsm, NamedVariable variable)
		{
			return SkillSearch.GetSearch(fsm).GetVariableUseCount(variable);
		}
		public static List<SkillVariable> GetUnusedVariables(Skill fsm)
		{
			SkillSearch.Update(fsm);
			return SkillSearch.GetSearch(fsm).GetUnusedVariables();
		}
		public List<SkillInfo> GetVariableUsageList(NamedVariable variable)
		{
			List<SkillInfo> list;
			this.variableUsage.TryGetValue(variable, ref list);
			if (list == null)
			{
				list = new List<SkillInfo>();
				this.variableUsage.Add(variable, list);
			}
			return list;
		}
		public int GetVariableUseCount(NamedVariable variable)
		{
			return this.GetVariableUsageList(variable).get_Count();
		}
		private void FindVariableUsage(object obj, int currentDepth = 0)
		{
			if (obj != null && currentDepth < 7)
			{
				NamedVariable namedVariable = obj as NamedVariable;
				if (namedVariable != null && namedVariable.get_UsesVariable())
				{
					this.AddVariableUsage(namedVariable);
					return;
				}
				SkillVar fsmVar = obj as SkillVar;
				if (fsmVar != null && !string.IsNullOrEmpty(fsmVar.variableName))
				{
					this.AddVariableUsage(fsmVar.get_NamedVar());
					return;
				}
				if (obj.GetType().get_IsArray())
				{
					Array array = (Array)obj;
					for (int i = 0; i < array.get_Length(); i++)
					{
						this.FindVariableUsage(array.GetValue(i), currentDepth);
					}
					return;
				}
				IEnumerable<FieldInfo> serializedFields = TypeHelpers.GetSerializedFields(obj.GetType());
				using (IEnumerator<FieldInfo> enumerator = serializedFields.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FieldInfo current = enumerator.get_Current();
						SkillSearch.currentInfo.fieldInObject = obj;
						SkillSearch.currentInfo.field = current;
						object value = current.GetValue(obj);
						this.FindVariableUsage(value, currentDepth + 1);
					}
				}
			}
		}
		private void AddVariableUsage(NamedVariable variable)
		{
			if (variable == null)
			{
				return;
			}
			if (this.IsGlobalVariable(variable))
			{
				SkillSearch.GetGlobalVariablesUsageList(variable).Add(new SkillInfo(SkillSearch.currentInfo));
				if (!this.globalVariablesUsed.Contains(variable))
				{
					this.globalVariablesUsed.Add(variable);
					return;
				}
			}
			else
			{
				this.GetVariableUsageList(variable).Add(new SkillInfo(SkillSearch.currentInfo));
			}
		}
		private bool IsGlobalVariable(NamedVariable variable)
		{
			return SkillVariables.get_GlobalVariables().Contains(variable);
		}
		private void UpdateUnusedVariables()
		{
			this.unusedVariables.Clear();
			using (List<SkillVariable>.Enumerator enumerator = this.fsmVariables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					if (this.GetVariableUseCount(current.NamedVar) == 0)
					{
						this.unusedVariables.Add(current);
					}
				}
			}
		}
		public List<SkillVariable> GetUnusedVariables()
		{
			return this.unusedVariables;
		}
		public static List<string> FindVariablesUsedByStates(Skill fsm, IEnumerable<SkillState> states)
		{
			return SkillSearch.GetSearch(fsm).FindVariablesUsedByStates(states);
		}
		public List<string> FindVariablesUsedByStates(IEnumerable<SkillState> states)
		{
			this.UpdateVariableList();
			this.Update();
			List<string> list = new List<string>();
			using (IEnumerator<SkillState> enumerator = states.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillState current = enumerator.get_Current();
					list.AddRange(this.FindVariablesUsedByState(current));
				}
			}
			return list;
		}
		public List<string> FindVariablesUsedByState(SkillState state)
		{
			List<string> list = new List<string>();
			using (List<SkillVariable>.Enumerator enumerator = this.fsmVariables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					if (this.StateUsesVariable(state, current.NamedVar))
					{
						list.Add(current.NamedVar.get_Name());
					}
				}
			}
			return list;
		}
		public bool StateUsesVariable(SkillState state, NamedVariable variable)
		{
			if (state == null || variable == null)
			{
				return false;
			}
			List<SkillInfo> variableUsageList = this.GetVariableUsageList(variable);
			using (List<SkillInfo>.Enumerator enumerator = variableUsageList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillInfo current = enumerator.get_Current();
					if (current.stateName == state.get_Name())
					{
						return true;
					}
				}
			}
			return false;
		}
		public static List<string> FindVariablesUsedByActions(Skill fsm, IEnumerable<SkillStateAction> actions)
		{
			return SkillSearch.GetSearch(fsm).FindVariablesUsedByActions(actions);
		}
		public List<string> FindVariablesUsedByActions(IEnumerable<SkillStateAction> actions)
		{
			this.UpdateVariableList();
			this.Update();
			List<string> list = new List<string>();
			using (IEnumerator<SkillStateAction> enumerator = actions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillStateAction current = enumerator.get_Current();
					list.AddRange(this.FindVariablesUsedByAction(current));
				}
			}
			return list;
		}
		public List<string> FindVariablesUsedByAction(SkillStateAction action)
		{
			List<string> list = new List<string>();
			using (List<SkillVariable>.Enumerator enumerator = this.fsmVariables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					if (this.ActionUsesVariable(action, current.NamedVar))
					{
						list.Add(current.NamedVar.get_Name());
					}
				}
			}
			return list;
		}
		public bool ActionUsesVariable(SkillStateAction action, NamedVariable variable)
		{
			if (action == null || variable == null)
			{
				return false;
			}
			List<SkillInfo> variableUsageList = this.GetVariableUsageList(variable);
			using (List<SkillInfo>.Enumerator enumerator = variableUsageList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillInfo current = enumerator.get_Current();
					if (current.action == action)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
