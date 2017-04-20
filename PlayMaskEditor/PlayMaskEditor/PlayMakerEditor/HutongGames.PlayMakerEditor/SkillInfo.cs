using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class SkillInfo
	{
		private static SkillInfo currentInfo;
		private static SkillEventTarget currentEventTarget;
		public PlayMakerFSM fsmComponent
		{
			get;
			set;
		}
		public Skill fsm
		{
			get;
			set;
		}
		public SkillState state
		{
			get;
			set;
		}
		public SkillTransition transition
		{
			get;
			set;
		}
		public SkillStateAction action
		{
			get;
			set;
		}
		public int actionIndex
		{
			get;
			set;
		}
		public object fieldInObject
		{
			get;
			set;
		}
		public FieldInfo field
		{
			get;
			set;
		}
		public NamedVariable variable
		{
			get;
			set;
		}
		public string eventName
		{
			get;
			set;
		}
		public SkillEventTarget eventTarget
		{
			get;
			set;
		}
		public string stateName
		{
			get
			{
				if (this.state == null)
				{
					return "";
				}
				return this.state.get_Name();
			}
		}
		public SkillInfo()
		{
		}
		public SkillInfo(SkillInfo source)
		{
			this.fsmComponent = source.fsmComponent;
			this.fsm = source.fsm;
			this.state = source.state;
			this.transition = source.transition;
			this.action = source.action;
			this.variable = source.variable;
			this.field = source.field;
			this.fieldInObject = source.fieldInObject;
		}
		public void Select()
		{
			if (this.fsm == null)
			{
				return;
			}
			SkillEditor.SelectFsm(this.fsm);
			if (this.state == null)
			{
				return;
			}
			SkillEditor.SelectState(this.state, true);
			if (this.action == null)
			{
				return;
			}
			if (this.actionIndex < this.state.get_Actions().Length)
			{
				SkillEditor.SelectAction(this.state.get_Actions()[this.actionIndex], true);
			}
		}
		public static void SelectFsmInfo(object obj)
		{
			SkillInfo fsmInfo = obj as SkillInfo;
			if (fsmInfo != null)
			{
				fsmInfo.Select();
			}
		}
		public static List<SkillState> FindStatesUsingEvent(Skill fsm, string eventName)
		{
			List<SkillInfo> list = new List<SkillInfo>();
			list.AddRange(SkillInfo.FindTransitionsUsingEvent(fsm, eventName));
			list.AddRange(SkillInfo.FindActionsUsingEvent(fsm, eventName));
			return SkillInfo.GetStateList(list);
		}
		public static List<SkillInfo> FindTransitionsUsingEvent(string eventName)
		{
			return SkillInfo.FindTransitionsUsingEvent(SkillEditor.FsmList, eventName);
		}
		public static List<SkillInfo> FindTransitionsUsingEvent(List<Skill> fsmSelection, string eventName)
		{
			List<SkillInfo> list = new List<SkillInfo>();
			using (List<Skill>.Enumerator enumerator = fsmSelection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					list.AddRange(SkillInfo.FindTransitionsUsingEvent(current, eventName));
				}
			}
			return list;
		}
		public static List<SkillInfo> FindTransitionsUsingEvent(Skill fsm, string eventName)
		{
			List<SkillInfo> list = new List<SkillInfo>();
			SkillState[] states = fsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				SkillTransition[] transitions = fsmState.get_Transitions();
				for (int j = 0; j < transitions.Length; j++)
				{
					SkillTransition fsmTransition = transitions[j];
					if (fsmTransition.get_EventName() == eventName)
					{
						list.Add(new SkillInfo
						{
							fsm = fsm,
							state = fsmState,
							transition = fsmTransition
						});
					}
				}
			}
			SkillTransition[] globalTransitions = fsm.get_GlobalTransitions();
			for (int k = 0; k < globalTransitions.Length; k++)
			{
				SkillTransition fsmTransition2 = globalTransitions[k];
				if (fsmTransition2.get_EventName() == eventName)
				{
					list.Add(new SkillInfo
					{
						fsm = fsm,
						state = fsm.GetState(fsmTransition2.get_ToState()),
						transition = fsmTransition2
					});
				}
			}
			return list;
		}
		public static List<SkillInfo> FindActionsTargetingTransition(SkillState state, string eventName)
		{
			SkillInfo.currentEventTarget = new SkillEventTarget();
			List<SkillInfo> list = SkillInfo.FindActionsUsingEvent(eventName);
			List<SkillInfo> list2 = new List<SkillInfo>();
			using (List<SkillInfo>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillInfo current = enumerator.get_Current();
					if (SkillInfo.DoesActionEventTargetLocalTransition(current, state))
					{
						list2.Add(current);
					}
				}
			}
			return list2;
		}
		private static bool DoesActionEventTargetLocalTransition(SkillInfo actionEvent, SkillState state)
		{
			if (actionEvent.fsm == state.get_Fsm() && actionEvent.state != state)
			{
				return false;
			}
			SkillEventTarget eventTarget = actionEvent.eventTarget;
			switch (eventTarget.target)
			{
			case 0:
				return actionEvent.fsm == state.get_Fsm() && actionEvent.state == state;
			case 1:
			{
				GameObject ownerDefaultTarget = actionEvent.fsm.GetOwnerDefaultTarget(eventTarget.gameObject);
				return ownerDefaultTarget == state.get_Fsm().get_GameObject();
			}
			case 2:
			{
				string value = actionEvent.eventTarget.fsmName.get_Value();
				GameObject ownerDefaultTarget = actionEvent.fsm.GetOwnerDefaultTarget(eventTarget.gameObject);
				return ownerDefaultTarget == state.get_Fsm().get_GameObject() && (string.IsNullOrEmpty(value) || value == state.get_Fsm().get_Name());
			}
			case 3:
				return eventTarget.fsmComponent == state.get_Fsm().get_Owner();
			case 4:
				return !eventTarget.excludeSelf.get_Value() || actionEvent.fsm != state.get_Fsm();
			default:
				return false;
			}
		}
		public static List<SkillInfo> FindActionsTargetingGlobalTransition(Skill fsm, string eventName)
		{
			SkillInfo.currentEventTarget = new SkillEventTarget();
			List<SkillInfo> list = SkillInfo.FindActionsUsingEvent(eventName);
			List<SkillInfo> list2 = new List<SkillInfo>();
			using (List<SkillInfo>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillInfo current = enumerator.get_Current();
					if (SkillInfo.DoesActionEventTargetGlobalTransition(current, fsm))
					{
						list2.Add(current);
					}
				}
			}
			return list2;
		}
		private static bool DoesActionEventTargetGlobalTransition(SkillInfo actionEvent, Skill fsm)
		{
			SkillEventTarget eventTarget = actionEvent.eventTarget;
			switch (eventTarget.target)
			{
			case 0:
				return actionEvent.fsm == fsm;
			case 1:
			{
				GameObject ownerDefaultTarget = actionEvent.fsm.GetOwnerDefaultTarget(eventTarget.gameObject);
				return ownerDefaultTarget == fsm.get_GameObject();
			}
			case 2:
			{
				string value = actionEvent.eventTarget.fsmName.get_Value();
				GameObject ownerDefaultTarget = actionEvent.fsm.GetOwnerDefaultTarget(eventTarget.gameObject);
				return ownerDefaultTarget == fsm.get_GameObject() && (string.IsNullOrEmpty(value) || value == fsm.get_Name());
			}
			case 3:
				return eventTarget.fsmComponent == fsm.get_Owner();
			case 4:
				return !eventTarget.excludeSelf.get_Value() || actionEvent.fsm != fsm;
			default:
				return false;
			}
		}
		public static List<SkillInfo> FindActionsUsingEvent(string eventName)
		{
			return SkillInfo.FindActionsUsingEvent(SkillEditor.FsmList, eventName);
		}
		public static List<SkillInfo> FindActionsUsingEvent(List<Skill> fsmSelection, string eventName)
		{
			List<SkillInfo> list = new List<SkillInfo>();
			using (List<Skill>.Enumerator enumerator = fsmSelection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					list.AddRange(SkillInfo.FindActionsUsingEvent(current, eventName));
				}
			}
			return list;
		}
		public static List<SkillInfo> FindActionsUsingEvent(Skill fsm, string eventName)
		{
			List<SkillInfo> list = new List<SkillInfo>();
			SkillState[] states = fsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				fsmState.set_Fsm(fsm);
				list.AddRange(SkillInfo.FindActionsUsingEvent(fsmState, eventName));
			}
			return list;
		}
		public static List<SkillInfo> FindActionsUsingEvent(SkillState state, string eventName)
		{
			List<SkillInfo> list = new List<SkillInfo>();
			int num = 0;
			SkillStateAction[] actions = state.get_Actions();
			for (int i = 0; i < actions.Length; i++)
			{
				SkillStateAction fsmStateAction = actions[i];
				SkillInfo.currentInfo = new SkillInfo
				{
					fsm = state.get_Fsm(),
					state = state,
					action = fsmStateAction,
					actionIndex = num
				};
				list.AddRange(SkillInfo.FindEventUsage(fsmStateAction, eventName));
				num++;
			}
			return list;
		}
		private static IEnumerable<SkillInfo> FindEventUsage(object obj, string eventName)
		{
			List<SkillInfo> list = new List<SkillInfo>();
			if (obj != null)
			{
				Type type = obj.GetType();
				FieldInfo[] fields = type.GetFields(20);
				FieldInfo[] array = fields;
				for (int i = 0; i < array.Length; i++)
				{
					FieldInfo fieldInfo = array[i];
					Type fieldType = fieldInfo.get_FieldType();
					object value = fieldInfo.GetValue(obj);
					if (fieldType == typeof(SkillEventTarget))
					{
						SkillInfo.currentEventTarget = (value as SkillEventTarget);
					}
					if (SkillInfo.IsEventUsedInField(fieldType, value, eventName))
					{
						list.Add(new SkillInfo(SkillInfo.currentInfo)
						{
							field = fieldInfo,
							eventTarget = SkillInfo.currentEventTarget
						});
					}
					else
					{
						if (fieldType.get_IsClass())
						{
							list.AddRange(SkillInfo.FindEventUsage(value, eventName));
						}
					}
				}
			}
			return list;
		}
		private static bool IsEventUsedInField(Type fieldType, object fieldValue, string eventName)
		{
			if (fieldValue == null)
			{
				return false;
			}
			if (fieldType == typeof(SkillEvent))
			{
				SkillEvent fsmEvent = (SkillEvent)fieldValue;
				return eventName == fsmEvent.get_Name();
			}
			if (fieldType.get_IsArray())
			{
				Array array = (Array)fieldValue;
				Type elementType = fieldType.GetElementType();
				if (elementType == typeof(SkillEvent))
				{
					for (int i = 0; i < array.get_Length(); i++)
					{
						if (SkillInfo.IsEventUsedInField(elementType, array.GetValue(i), eventName))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public static List<SkillInfo> FindActionUsage(Type actionType)
		{
			List<SkillInfo> list = new List<SkillInfo>();
			if (SkillEditor.FsmList != null)
			{
				using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Skill current = enumerator.get_Current();
						list.AddRange(SkillInfo.FindStatesUsingAction(current, actionType));
					}
				}
			}
			return list;
		}
		public static List<SkillInfo> FindStatesUsingAction(Skill fsm, Type actionType)
		{
			List<SkillInfo> list = new List<SkillInfo>();
			SkillState[] states = fsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				if (fsmState.get_ActionsLoaded())
				{
					int num = 0;
					SkillStateAction[] actions = fsmState.get_Actions();
					for (int j = 0; j < actions.Length; j++)
					{
						SkillStateAction fsmStateAction = actions[j];
						if (fsmStateAction.GetType() == actionType)
						{
							list.Add(new SkillInfo
							{
								fsm = fsm,
								state = fsmState,
								action = fsmStateAction,
								actionIndex = num
							});
						}
						num++;
					}
				}
			}
			return list;
		}
		public static List<Skill> GetFsmList(List<SkillInfo> fsmInfoList)
		{
			List<Skill> list = new List<Skill>();
			using (List<SkillInfo>.Enumerator enumerator = fsmInfoList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillInfo current = enumerator.get_Current();
					if (current.fsm != null && !list.Contains(current.fsm))
					{
						list.Add(current.fsm);
					}
				}
			}
			return list;
		}
		public static List<SkillState> GetStateList(List<SkillInfo> fsmInfoList)
		{
			List<SkillState> list = new List<SkillState>();
			using (List<SkillInfo>.Enumerator enumerator = fsmInfoList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillInfo current = enumerator.get_Current();
					if (current.state != null && !list.Contains(current.state))
					{
						list.Add(current.state);
					}
				}
			}
			return list;
		}
		public static List<NamedVariable> GetVariableList(List<SkillInfo> fsmInfoList)
		{
			List<NamedVariable> list = new List<NamedVariable>();
			using (List<SkillInfo>.Enumerator enumerator = fsmInfoList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillInfo current = enumerator.get_Current();
					if (current.variable != null && !list.Contains(current.variable))
					{
						list.Add(current.variable);
					}
				}
			}
			return list;
		}
	}
}
