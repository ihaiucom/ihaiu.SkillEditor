using HutongGames.PlayMaker;
using HutongGames.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class SkillBuilder
	{
		private static SkillTemplate clipboard;
		private Skill targetFsm;
		public static SkillTemplate Clipboard
		{
			get
			{
				return SkillBuilder.clipboard;
			}
			set
			{
				SkillBuilder.clipboard = value;
			}
		}
		public static int ClipboardNumStates
		{
			get
			{
				if (SkillBuilder.clipboard == null || SkillBuilder.clipboard.fsm == null || SkillBuilder.clipboard.fsm.get_States() == null)
				{
					return 0;
				}
				return SkillBuilder.clipboard.fsm.get_States().Length;
			}
		}
		public void SetTarget(Skill target)
		{
			this.targetFsm = target;
		}
		public SkillState AddState(Vector2 position)
		{
			SkillState fsmState = new SkillState(this.targetFsm);
			fsmState.set_Name(FsmEditorSettings.NewStateName);
			SkillState fsmState2 = fsmState;
			fsmState2.set_Name(this.GenerateUniqueStateName(fsmState2, fsmState2.get_Name()));
			SkillState arg_57_0 = fsmState2;
			Rect position2 = new Rect(fsmState2.get_Position());
			position2.set_x(position.x);
			position2.set_y(position.y);
			arg_57_0.set_Position(position2);
			this.targetFsm.set_States(ArrayUtility.Add<SkillState>(this.targetFsm.get_States(), fsmState2));
			if (this.targetFsm.get_States().Length == 1)
			{
				this.SetStartState(fsmState2.get_Name());
			}
			return fsmState2;
		}
		public void SetStartState(string state)
		{
			this.targetFsm.set_StartState(state);
		}
		public void SetStateName(SkillState state, string name)
		{
			state.set_Name(this.StateNameExists(state, name) ? this.GenerateUniqueStateName(state, name) : name);
		}
		public void RenameState(SkillState state, string newName)
		{
			SkillState[] states = this.targetFsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				SkillTransition[] transitions = fsmState.get_Transitions();
				for (int j = 0; j < transitions.Length; j++)
				{
					SkillTransition fsmTransition = transitions[j];
					if (fsmTransition.get_ToState() == state.get_Name())
					{
						fsmTransition.set_ToState(newName);
					}
				}
			}
			SkillTransition[] globalTransitions = this.targetFsm.get_GlobalTransitions();
			for (int k = 0; k < globalTransitions.Length; k++)
			{
				SkillTransition fsmTransition2 = globalTransitions[k];
				if (fsmTransition2.get_ToState() == state.get_Name())
				{
					fsmTransition2.set_ToState(newName);
				}
			}
			if (this.targetFsm.get_StartState() == state.get_Name())
			{
				this.targetFsm.set_StartState(newName);
			}
			state.set_Name(newName);
		}
		public void DeleteStates(List<SkillState> states)
		{
			using (List<SkillState>.Enumerator enumerator = states.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillState current = enumerator.get_Current();
					this.DeleteState(current);
				}
			}
		}
		public void DeleteState(SkillState state)
		{
			if (state == null)
			{
				Debug.LogError("Trying to delete null state!");
				return;
			}
			if (this.IsStartState(state))
			{
				EditorApplication.Beep();
				EditorUtility.DisplayDialog(Strings.get_Command_Delete_States(), Strings.get_Dialog_Cannot_Delete_Start_State(), Strings.get_OK());
				return;
			}
			SkillState[] states = this.targetFsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				SkillTransition[] transitions = fsmState.get_Transitions();
				for (int j = 0; j < transitions.Length; j++)
				{
					SkillTransition fsmTransition = transitions[j];
					if (fsmTransition.get_ToState() == state.get_Name())
					{
						fsmTransition.set_ToState("");
					}
				}
			}
			SkillTransition[] globalTransitions = this.targetFsm.get_GlobalTransitions();
			for (int k = 0; k < globalTransitions.Length; k++)
			{
				SkillTransition fsmTransition2 = globalTransitions[k];
				if (fsmTransition2.get_ToState() == state.get_Name())
				{
					this.DeleteGlobalTransition(fsmTransition2);
				}
			}
			if (this.targetFsm.get_StartState() == state.get_Name())
			{
				this.targetFsm.set_StartState("");
			}
			SkillTransition[] transitions2 = state.get_Transitions();
			for (int l = 0; l < transitions2.Length; l++)
			{
				SkillTransition transition = transitions2[l];
				this.DeleteTransition(state, transition);
			}
			this.targetFsm.set_States(ArrayUtility.Remove<SkillState>(this.targetFsm.get_States(), state));
		}
		[Localizable(false)]
		public string ValidateNewStateName(SkillState state, string newName)
		{
			if (string.IsNullOrEmpty(newName))
			{
				return Strings.get_Error_Name_is_empty();
			}
			SkillState[] states = this.targetFsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				if (fsmState != state && fsmState.get_Name() == newName)
				{
					return Strings.get_Error_Name_already_used_in_this_FSM();
				}
			}
			if (newName.get_Length() > 100)
			{
				return Strings.get_Error_Name_is_too_long();
			}
			if (!newName.Contains("\\"))
			{
				return "";
			}
			return Strings.get_Error_Name_contains_illegal_character();
		}
		[Localizable(false)]
		public string GenerateUniqueStateName(SkillState state, string newNameRoot)
		{
			int num = 1;
			string text = newNameRoot + " " + num.ToString(CultureInfo.get_InvariantCulture());
			while (this.StateNameExists(state, text))
			{
				text = newNameRoot + " " + num++.ToString(CultureInfo.get_InvariantCulture());
			}
			return text;
		}
		private string GenerateCopyName(SkillState state)
		{
			if (!this.StateNameExists(state, state.get_Name()))
			{
				return state.get_Name();
			}
			string text = StringUtils.IncrementStringCounter(state.get_Name());
			while (this.StateNameExists(state, text))
			{
				text = StringUtils.IncrementStringCounter(text);
			}
			return text;
		}
		private bool StateNameExists(SkillState state, string name)
		{
			SkillState[] states = this.targetFsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				if (fsmState != state && fsmState.get_Name() == name)
				{
					return true;
				}
			}
			return false;
		}
		public bool IsStartState(SkillState fsmState)
		{
			return fsmState != null && this.targetFsm.GetState(this.targetFsm.get_StartState()) == fsmState;
		}
		public SkillState GetStartState()
		{
			if (this.targetFsm != null)
			{
				return this.targetFsm.GetState(this.targetFsm.get_StartState());
			}
			return null;
		}
		public static void SetStateColorIndex(SkillState state, int colorIndex, bool undo = true)
		{
			if (state == null)
			{
				return;
			}
			colorIndex = Mathf.Clamp(colorIndex, 0, PlayMakerPrefs.get_Colors().Length - 1);
			state.set_ColorIndex(colorIndex);
		}
		public static void SetSelectedStatesColorIndex(int colorIndex)
		{
			colorIndex = Mathf.Clamp(colorIndex, 0, PlayMakerPrefs.get_Colors().Length - 1);
			using (List<SkillState>.Enumerator enumerator = SkillEditor.Selection.States.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillState current = enumerator.get_Current();
					current.set_ColorIndex(colorIndex);
				}
			}
		}
		public static void SetTransitionColorIndex(Skill fsm, SkillTransition transition, int colorIndex, bool undo = true)
		{
			if (transition != null)
			{
				if (undo)
				{
					SkillEditor.RegisterUndo(Strings.get_Command_Set_Transition_Color());
				}
				colorIndex = Mathf.Clamp(colorIndex, 0, PlayMakerPrefs.get_Colors().Length - 1);
				transition.set_ColorIndex(colorIndex);
				SkillEditor.SetFsmDirty(fsm, false, false, true);
			}
		}
		public SkillTransition AddTransition(SkillState state)
		{
			return this.AddTransition(state, string.Empty, null);
		}
		public SkillTransition AddTransition(SkillState state, string toState, SkillEvent fsmEvent)
		{
			SkillTransition fsmTransition = new SkillTransition();
			fsmTransition.set_ToState(toState);
			fsmTransition.set_FsmEvent(fsmEvent);
			SkillTransition fsmTransition2 = fsmTransition;
			state.set_Transitions(ArrayUtility.Add<SkillTransition>(state.get_Transitions(), fsmTransition2));
			return fsmTransition2;
		}
		public void SetTransitionEvent(SkillTransition transition, SkillEvent fsmEvent)
		{
			if (transition != null)
			{
				transition.set_FsmEvent(fsmEvent);
			}
		}
		public static SkillState GetTransitionState(Skill fsm, SkillTransition transition)
		{
			if (fsm == null || transition == null)
			{
				return null;
			}
			SkillTransition[] globalTransitions = fsm.get_GlobalTransitions();
			for (int i = 0; i < globalTransitions.Length; i++)
			{
				SkillTransition fsmTransition = globalTransitions[i];
				if (fsmTransition == transition)
				{
					SkillState result = fsm.GetState(fsmTransition.get_ToState());
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
					if (fsmTransition2 == transition)
					{
						SkillState result = fsmState;
						return result;
					}
				}
			}
			return null;
		}
		public void SetTransitionTarget(SkillTransition transition, string toState)
		{
			if (transition != null)
			{
				transition.set_ToState(toState);
			}
		}
		public void DeleteTransition(SkillState state, SkillTransition transition)
		{
			state.set_Transitions(ArrayUtility.Remove<SkillTransition>(state.get_Transitions(), transition));
		}
		public void MoveTransitionUp(SkillState state, SkillTransition transition)
		{
			int transitionIndex = this.GetTransitionIndex(state, transition);
			if (transitionIndex > 0)
			{
				state.set_Transitions(ArrayUtility.MoveItem<SkillTransition>(state.get_Transitions(), transitionIndex, transitionIndex - 1));
			}
		}
		public void MoveTransitionDown(SkillState state, SkillTransition transition)
		{
			int transitionIndex = this.GetTransitionIndex(state, transition);
			if (transitionIndex < state.get_Transitions().Length - 1)
			{
				state.set_Transitions(ArrayUtility.MoveItem<SkillTransition>(state.get_Transitions(), transitionIndex, transitionIndex + 1));
			}
		}
		public SkillTransition FindTransitionToState(SkillState toState)
		{
			SkillTransition[] globalTransitions = this.targetFsm.get_GlobalTransitions();
			for (int i = 0; i < globalTransitions.Length; i++)
			{
				SkillTransition fsmTransition = globalTransitions[i];
				if (fsmTransition.get_ToState() == toState.get_Name())
				{
					SkillTransition result = fsmTransition;
					return result;
				}
			}
			if (this.targetFsm.get_ActiveState() == null)
			{
				return null;
			}
			SkillTransition[] transitions = this.targetFsm.get_ActiveState().get_Transitions();
			for (int j = 0; j < transitions.Length; j++)
			{
				SkillTransition fsmTransition2 = transitions[j];
				if (fsmTransition2.get_ToState() == toState.get_Name())
				{
					SkillTransition result = fsmTransition2;
					return result;
				}
			}
			return null;
		}
		public int GetTransitionIndex(SkillState state, SkillTransition transition)
		{
			int num = 0;
			SkillTransition[] transitions = state.get_Transitions();
			for (int i = 0; i < transitions.Length; i++)
			{
				SkillTransition fsmTransition = transitions[i];
				if (fsmTransition == transition)
				{
					return num;
				}
				num++;
			}
			return -1;
		}
		public SkillTransition AddGlobalTransition(SkillState state, SkillEvent fsmEvent)
		{
			SkillTransition fsmTransition = new SkillTransition();
			fsmTransition.set_ToState(state.get_Name());
			fsmTransition.set_FsmEvent(fsmEvent);
			SkillTransition fsmTransition2 = fsmTransition;
			this.targetFsm.set_GlobalTransitions(ArrayUtility.Add<SkillTransition>(this.targetFsm.get_GlobalTransitions(), fsmTransition2));
			return fsmTransition2;
		}
		public void DeleteGlobalTransition(SkillTransition transition)
		{
			this.targetFsm.set_GlobalTransitions(ArrayUtility.Remove<SkillTransition>(this.targetFsm.get_GlobalTransitions(), transition));
		}
		public List<SkillTransition> GetGlobalTransitions(SkillState state)
		{
			List<SkillTransition> list = new List<SkillTransition>();
			SkillTransition[] globalTransitions = this.targetFsm.get_GlobalTransitions();
			for (int i = 0; i < globalTransitions.Length; i++)
			{
				SkillTransition fsmTransition = globalTransitions[i];
				if (fsmTransition.get_ToState() == state.get_Name())
				{
					list.Add(fsmTransition);
				}
			}
			return list;
		}
		public bool HasGlobalTransition(SkillState state)
		{
			SkillTransition[] globalTransitions = this.targetFsm.get_GlobalTransitions();
			for (int i = 0; i < globalTransitions.Length; i++)
			{
				SkillTransition fsmTransition = globalTransitions[i];
				if (fsmTransition.get_ToState() == state.get_Name())
				{
					return true;
				}
			}
			return false;
		}
		public static void AddExposedEvent(Skill fsm, SkillEvent fsmEvent)
		{
			if (fsm != null)
			{
				fsm.ExposedEvents.Add(fsmEvent);
			}
		}
		public static void RemoveExposedEvent(Skill fsm, SkillEvent fsmEvent)
		{
			List<SkillEvent> list = new List<SkillEvent>();
			using (List<SkillEvent>.Enumerator enumerator = fsm.ExposedEvents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					if (current.get_Name() == fsmEvent.get_Name())
					{
						list.Add(current);
					}
				}
			}
			using (List<SkillEvent>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					SkillEvent current2 = enumerator2.get_Current();
					fsm.ExposedEvents.Remove(current2);
				}
			}
		}
		public static void RenameEventInField(object obj, FieldInfo field, string oldEventName, string newEventName)
		{
			Type fieldType = field.get_FieldType();
			object value = field.GetValue(obj);
			if (value == null)
			{
				return;
			}
			if (fieldType == typeof(SkillEvent))
			{
				SkillEvent fsmEvent = (SkillEvent)value;
				if (fsmEvent.get_Name() == oldEventName)
				{
					fsmEvent.set_Name(newEventName);
					field.SetValue(obj, fsmEvent);
				}
			}
			if (fieldType.get_IsArray())
			{
				Array array = (Array)value;
				Type elementType = fieldType.GetElementType();
				if (elementType == typeof(SkillEvent))
				{
					for (int i = 0; i < array.get_Length(); i++)
					{
						SkillBuilder.RenameEventInArray(array, i, oldEventName, newEventName);
					}
				}
			}
		}
		public static void RenameEventInArray(Array array, int elementIndex, string oldEventName, string newEventName)
		{
			object value = array.GetValue(elementIndex);
			if (value == null)
			{
				return;
			}
			SkillEvent fsmEvent = value as SkillEvent;
			if (fsmEvent != null && fsmEvent.get_Name() == oldEventName)
			{
				fsmEvent.set_Name(newEventName);
				array.SetValue(fsmEvent, elementIndex);
			}
		}
		public SkillEvent AddEvent(string name)
		{
			SkillEvent fsmEvent = SkillEvent.GetFsmEvent(name);
			return this.AddEvent(this.targetFsm, fsmEvent);
		}
		public SkillEvent AddEvent(Skill fsm, SkillEvent fsmEvent)
		{
			if (fsmEvent == null)
			{
				return null;
			}
			SkillEvent[] events = fsm.get_Events();
			for (int i = 0; i < events.Length; i++)
			{
				SkillEvent fsmEvent2 = events[i];
				if (fsmEvent2.get_Name() == fsmEvent.get_Name())
				{
					return fsmEvent2;
				}
			}
			fsm.set_Events(ArrayUtility.Add<SkillEvent>(this.targetFsm.get_Events(), fsmEvent));
			Array.Sort<SkillEvent>(fsm.get_Events());
			return fsmEvent;
		}
		public void DeleteEvent(Skill fsm, string fsmEventName)
		{
			if (fsm == null || string.IsNullOrEmpty(fsmEventName))
			{
				return;
			}
			List<SkillEvent> list = new List<SkillEvent>();
			SkillEvent[] events = fsm.get_Events();
			for (int i = 0; i < events.Length; i++)
			{
				SkillEvent fsmEvent = events[i];
				if (fsmEvent.get_Name() != fsmEventName)
				{
					list.Add(fsmEvent);
				}
			}
			fsm.set_Events(list.ToArray());
			List<SkillEvent> list2 = new List<SkillEvent>();
			using (List<SkillEvent>.Enumerator enumerator = fsm.ExposedEvents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					if (current.get_Name() != fsmEventName)
					{
						list2.Add(current);
					}
				}
			}
			fsm.ExposedEvents = list2;
			SkillTransition[] globalTransitions = fsm.get_GlobalTransitions();
			for (int j = 0; j < globalTransitions.Length; j++)
			{
				SkillTransition fsmTransition = globalTransitions[j];
				if (fsmTransition.get_EventName() == fsmEventName)
				{
					fsmTransition.set_FsmEvent(null);
				}
			}
			SkillState[] states = fsm.get_States();
			for (int k = 0; k < states.Length; k++)
			{
				SkillState fsmState = states[k];
				SkillTransition[] transitions = fsmState.get_Transitions();
				for (int l = 0; l < transitions.Length; l++)
				{
					SkillTransition fsmTransition2 = transitions[l];
					if (fsmTransition2.get_EventName() == fsmEventName)
					{
						fsmTransition2.set_FsmEvent(null);
					}
				}
				SkillStateAction[] actions = fsmState.get_Actions();
				for (int m = 0; m < actions.Length; m++)
				{
					SkillStateAction action = actions[m];
					this.DeleteEvent(fsmState, action, fsmEventName);
				}
			}
		}
		public void DeleteEvent(SkillState state, SkillStateAction action, string fsmEventName)
		{
			if (state == null || action == null || string.IsNullOrEmpty(fsmEventName))
			{
				return;
			}
			this.DeleteEvent(action, fsmEventName);
		}
		public void DeleteEvent(object obj, string fsmEventName)
		{
			if (obj == null || string.IsNullOrEmpty(fsmEventName))
			{
				return;
			}
			Type type = obj.GetType();
			FieldInfo[] fields = ActionData.GetFields(type);
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = array[i];
				Type fieldType = fieldInfo.get_FieldType();
				object value = fieldInfo.GetValue(obj);
				if (value != null && fieldType == typeof(SkillEvent))
				{
					SkillEvent fsmEvent = (SkillEvent)value;
					if (fsmEvent.get_Name() == fsmEventName)
					{
						fieldInfo.SetValue(obj, null);
					}
				}
			}
		}
		public void DeleteEvent(Skill fsm, SkillEvent fsmEvent)
		{
			if (fsm == null || SkillEvent.IsNullOrEmpty(fsmEvent))
			{
				return;
			}
			this.DeleteEvent(fsm, fsmEvent.get_Name());
		}
		public void DeleteEvent(SkillEvent fsmEvent)
		{
			if (fsmEvent == null)
			{
				return;
			}
			this.DeleteEvent(this.targetFsm, fsmEvent);
		}
		public void RenameEvent(string oldEventName, string newEventName)
		{
			if (newEventName == oldEventName)
			{
				return;
			}
			if (FsmEditorSettings.LoadAllPrefabs)
			{
				Files.LoadAllPlaymakerPrefabs();
			}
			List<SkillInfo> list = SkillInfo.FindActionsUsingEvent(oldEventName);
			List<Skill> fsmList = SkillInfo.GetFsmList(list);
			using (List<Skill>.Enumerator enumerator = fsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					Undo.RecordObject(current.get_OwnerObject(), Strings.get_Command_Rename_Event());
				}
			}
			using (List<SkillInfo>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					SkillInfo current2 = enumerator2.get_Current();
					SkillBuilder.RenameEventInField(current2.action, current2.field, oldEventName, newEventName);
				}
			}
			using (List<Skill>.Enumerator enumerator3 = fsmList.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Skill current3 = enumerator3.get_Current();
					SkillEditor.SaveActions(current3);
				}
			}
			List<SkillInfo> list2 = SkillInfo.FindTransitionsUsingEvent(oldEventName);
			using (List<SkillInfo>.Enumerator enumerator4 = list2.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					SkillInfo current4 = enumerator4.get_Current();
					current4.transition.get_FsmEvent().set_Name(newEventName);
				}
			}
			using (List<Skill>.Enumerator enumerator5 = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator5.MoveNext())
				{
					Skill current5 = enumerator5.get_Current();
					SkillEvent[] events = current5.get_Events();
					for (int i = 0; i < events.Length; i++)
					{
						SkillEvent fsmEvent = events[i];
						if (fsmEvent.get_Name() == oldEventName)
						{
							fsmEvent.set_Name(newEventName);
						}
					}
					SkillEditor.GraphView.UpdateStateSizes(current5);
					SkillEditor.SetFsmDirty(current5, true, false, true);
				}
			}
			SkillEvent fsmEvent2 = SkillEvent.FindEvent(oldEventName);
			if (fsmEvent2 != null)
			{
				fsmEvent2.set_Name(newEventName);
			}
			if (SkillEvent.get_globalEvents().Contains(oldEventName))
			{
				SkillEvent.get_globalEvents().Remove(oldEventName);
				if (!SkillEvent.get_globalEvents().Contains(newEventName))
				{
					SkillEvent.get_globalEvents().Add(newEventName);
				}
			}
			SkillEvent.get_EventList().Sort();
		}
		public void RenameEvent(Skill fsmTarget, string oldEventName, string newEventName)
		{
			if (newEventName == oldEventName)
			{
				return;
			}
			Undo.RecordObject(fsmTarget.get_OwnerObject(), Strings.get_Command_Rename_Event());
			List<SkillInfo> list = SkillInfo.FindActionsUsingEvent(fsmTarget, oldEventName);
			using (List<SkillInfo>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillInfo current = enumerator.get_Current();
					SkillBuilder.RenameEventInField(current.action, current.field, oldEventName, newEventName);
				}
			}
			SkillEditor.SaveActions(fsmTarget);
			List<SkillInfo> list2 = SkillInfo.FindTransitionsUsingEvent(fsmTarget, oldEventName);
			using (List<SkillInfo>.Enumerator enumerator2 = list2.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					SkillInfo current2 = enumerator2.get_Current();
					current2.transition.get_FsmEvent().set_Name(newEventName);
				}
			}
			SkillEvent[] events = fsmTarget.get_Events();
			for (int i = 0; i < events.Length; i++)
			{
				SkillEvent fsmEvent = events[i];
				if (fsmEvent.get_Name() == oldEventName)
				{
					fsmEvent.set_Name(newEventName);
				}
			}
			using (List<SkillEvent>.Enumerator enumerator3 = fsmTarget.ExposedEvents.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					SkillEvent current3 = enumerator3.get_Current();
					if (current3.get_Name() == oldEventName)
					{
						current3.set_Name(newEventName);
					}
				}
			}
			SkillEditor.GraphView.UpdateStateSizes(fsmTarget);
			SkillEditor.SetFsmDirty(fsmTarget, true, false, true);
			SkillSearch.Update(fsmTarget);
			SkillEvent.get_EventList().Sort();
		}
		public void SetEventIsGlobal(Skill fsm, SkillEvent fsmEvent, bool isGlobal)
		{
			if (fsmEvent == null)
			{
				return;
			}
			SkillEditor.RegisterGlobalsUndo(Strings.get_Command_Edit_Event_Global_Setting());
			fsmEvent = SkillEvent.GetFsmEvent(fsmEvent.get_Name());
			fsmEvent.set_IsGlobal(isGlobal);
			SkillEditor.SaveGlobals();
			if (fsm != null)
			{
				SkillEvent[] events = fsm.get_Events();
				for (int i = 0; i < events.Length; i++)
				{
					SkillEvent fsmEvent2 = events[i];
					if (fsmEvent2.get_Name() == fsmEvent.get_Name())
					{
						fsmEvent2.set_IsGlobal(isGlobal);
					}
				}
				SkillEditor.SetFsmDirty(fsm, true, false, true);
			}
		}
		public void SetEventIsGlobal(SkillEvent fsmEvent)
		{
			if (fsmEvent == null)
			{
				return;
			}
			fsmEvent = SkillEvent.GetFsmEvent(fsmEvent.get_Name());
			fsmEvent.set_IsGlobal(true);
			using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					bool flag = false;
					SkillEvent[] events = current.get_Events();
					for (int i = 0; i < events.Length; i++)
					{
						SkillEvent fsmEvent2 = events[i];
						if (fsmEvent2.get_Name() != fsmEvent.get_Name())
						{
							fsmEvent2.set_IsGlobal(true);
							flag = true;
						}
					}
					if (flag)
					{
						SkillEditor.SetFsmDirty(current, false, false, true);
					}
				}
			}
			SkillEditor.SaveGlobals();
		}
		[Localizable(false)]
		public string ValidateRenameEvent(SkillEvent fsmEvent, string newEventName)
		{
			if (fsmEvent == null || newEventName == null)
			{
				return Strings.get_Error_Invalid_Name();
			}
			if (fsmEvent.get_Name() == newEventName)
			{
				return "";
			}
			if (newEventName.Replace(" ", "") == "")
			{
				return Strings.get_Error_Invalid_Name();
			}
			return "";
		}
		[Localizable(false)]
		public string ValidateAddEvent(string newEventName)
		{
			if (string.IsNullOrEmpty(newEventName))
			{
				return "";
			}
			if (newEventName.Replace(" ", "") == "")
			{
				return Strings.get_Error_Invalid_Name();
			}
			SkillEvent[] events = this.targetFsm.get_Events();
			for (int i = 0; i < events.Length; i++)
			{
				SkillEvent fsmEvent = events[i];
				if (fsmEvent.get_Name() == newEventName)
				{
					return Strings.get_Error_Event_already_used();
				}
			}
			return "";
		}
		public SkillStateAction AddAction(SkillState state, Type actionType)
		{
			if (state == null)
			{
				return null;
			}
			SkillStateAction fsmStateAction = (SkillStateAction)Activator.CreateInstance(actionType);
			state.set_Actions(ArrayUtility.Add<SkillStateAction>(state.get_Actions(), fsmStateAction));
			fsmStateAction.Init(state);
			fsmStateAction.Reset();
			SkillEditor.SaveActions(fsmStateAction.get_State(), true);
			return fsmStateAction;
		}
		public SkillStateAction InsertAction(SkillState state, Type actionType, SkillStateAction beforeAction)
		{
			if (state == null)
			{
				return null;
			}
			if (beforeAction == null)
			{
				return this.AddAction(state, actionType);
			}
			SkillEditor.RegisterUndo(Strings.get_Command_Insert_Action());
			SkillStateAction fsmStateAction = (SkillStateAction)Activator.CreateInstance(actionType);
			int actionIndex = Actions.GetActionIndex(state, beforeAction);
			if (actionIndex == -1)
			{
				return this.AddAction(state, actionType);
			}
			List<SkillStateAction> list = new List<SkillStateAction>(state.get_Actions());
			list.Insert(actionIndex, fsmStateAction);
			state.set_Actions(list.ToArray());
			fsmStateAction.Init(state);
			fsmStateAction.Reset();
			SkillEditor.SaveActions(fsmStateAction.get_State(), true);
			SkillEditor.UpdateActionUsage();
			return fsmStateAction;
		}
		public void DeleteAction(SkillState state, SkillStateAction action)
		{
			if (state == null || action == null)
			{
				return;
			}
			SkillEditor.RegisterUndo(Strings.get_Command_Delete_Action());
			state.set_Actions(ArrayUtility.Remove<SkillStateAction>(state.get_Actions(), action));
			Keyboard.ResetFocus();
			SkillEditor.SaveActions(state, true);
			SkillEditor.UpdateActionUsage();
			SkillEditor.UpdateFsmInfo();
		}
		public void DeleteActions(SkillState state, IEnumerable<SkillStateAction> actions, bool undo = true)
		{
			if (state == null || actions == null)
			{
				return;
			}
			if (undo)
			{
				SkillEditor.RegisterUndo(Strings.get_Command_Delete_Actions());
			}
			using (IEnumerator<SkillStateAction> enumerator = actions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillStateAction current = enumerator.get_Current();
					state.set_Actions(ArrayUtility.Remove<SkillStateAction>(state.get_Actions(), current));
				}
			}
			Keyboard.ResetFocus();
			SkillEditor.SaveActions(state, true);
			SkillEditor.UpdateActionUsage();
			SkillEditor.UpdateFsmInfo();
		}
		public NamedVariable AddVariable(VariableType varType, string varName)
		{
			return SkillBuilder.AddVariable(this.targetFsm, varType, varName);
		}
		public static NamedVariable AddVariable(Skill fsm, VariableType type, string name)
		{
			List<SkillVariable> fsmVariableList = SkillVariable.GetFsmVariableList(fsm.get_OwnerObject());
			name = SkillVariable.GetUniqueVariableName(fsmVariableList, name);
			SkillVariable.AddVariable(fsm.get_Variables(), type, name, null, 0);
			return fsm.get_Variables().GetVariable(name);
		}
		public static void RemoveGlobalVariableUsage(NamedVariable variable)
		{
			List<SkillInfo> globalVariablesUsageList = SkillSearch.GetGlobalVariablesUsageList(variable);
			SkillBuilder.RemoveVariableUsage(globalVariablesUsageList, variable);
		}
		public static void RemoveVariableUsage(NamedVariable variable)
		{
			List<SkillInfo> variableUsageList = SkillSearch.GetVariableUsageList(SkillEditor.SelectedFsm, variable);
			SkillBuilder.RemoveVariableUsage(variableUsageList, variable);
		}
		public static void RemoveVariableUsage(List<SkillInfo> fsmInfoList, NamedVariable variable)
		{
			using (List<SkillInfo>.Enumerator enumerator = fsmInfoList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillInfo current = enumerator.get_Current();
					SkillBuilder.RemoveVariableUsageInField(current.fieldInObject, current.field, variable);
				}
			}
			List<Skill> fsmList = SkillInfo.GetFsmList(fsmInfoList);
			using (List<Skill>.Enumerator enumerator2 = fsmList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Skill current2 = enumerator2.get_Current();
					SkillEditor.SaveActions(current2);
				}
			}
		}
		public static void RemoveVariableUsageInObject(object obj, NamedVariable variable)
		{
			if (obj == null)
			{
				return;
			}
			IEnumerable<FieldInfo> serializedFields = TypeHelpers.GetSerializedFields(obj.GetType());
			using (IEnumerator<FieldInfo> enumerator = serializedFields.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FieldInfo current = enumerator.get_Current();
					SkillBuilder.RemoveVariableUsageInField(obj, current, variable);
				}
			}
		}
		public static void RemoveVariableUsageInField(object obj, FieldInfo field, NamedVariable variable)
		{
			object value = field.GetValue(obj);
			if (value == null)
			{
				return;
			}
			Type fieldType = field.get_FieldType();
			if (fieldType.IsSubclassOf(typeof(NamedVariable)))
			{
				NamedVariable namedVariable = (NamedVariable)value;
				if (namedVariable.get_UseVariable() && namedVariable.get_Name() == variable.get_Name())
				{
					namedVariable.set_Name(null);
					return;
				}
			}
			else
			{
				if (fieldType == typeof(SkillVar))
				{
					SkillVar fsmVar = (SkillVar)value;
					if (fsmVar.variableName == variable.get_Name())
					{
						fsmVar.set_NamedVar(SkillVariable.GetNewVariableOfSameType(fsmVar.get_NamedVar()));
						return;
					}
				}
				else
				{
					if (fieldType.get_IsArray())
					{
						Array array = (Array)value;
						Type elementType = fieldType.GetElementType();
						for (int i = 0; i < array.get_Length(); i++)
						{
							SkillBuilder.RemoveVariableUsageInArray(array, elementType, i, variable);
						}
						return;
					}
					if (fieldType.get_IsClass())
					{
						SkillBuilder.RemoveVariableUsageInObject(value, variable);
					}
				}
			}
		}
		public static void RemoveVariableUsageInArray(Array array, Type type, int elementIndex, NamedVariable variable)
		{
			object value = array.GetValue(elementIndex);
			if (value == null)
			{
				return;
			}
			if (type.IsSubclassOf(typeof(NamedVariable)))
			{
				NamedVariable namedVariable = (NamedVariable)value;
				if (namedVariable.get_UseVariable() && namedVariable == variable)
				{
					array.SetValue(null, elementIndex);
					return;
				}
			}
			else
			{
				if (type == typeof(SkillVar))
				{
					SkillVar fsmVar = (SkillVar)value;
					if (fsmVar.variableName == variable.get_Name())
					{
						array.SetValue(null, elementIndex);
						return;
					}
				}
				else
				{
					if (type.get_IsClass())
					{
						SkillBuilder.RemoveVariableUsageInObject(value, variable);
					}
				}
			}
		}
		public static void RenameVariable(NamedVariable variable, string newName)
		{
			List<SkillInfo> variableUsageList = SkillSearch.GetVariableUsageList(SkillEditor.SelectedFsm, variable);
			using (List<SkillInfo>.Enumerator enumerator = variableUsageList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillInfo current = enumerator.get_Current();
					SkillBuilder.RenameVariableInField(current.fieldInObject, current.field, variable, newName, 0);
				}
			}
			List<Skill> fsmList = SkillInfo.GetFsmList(variableUsageList);
			using (List<Skill>.Enumerator enumerator2 = fsmList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Skill current2 = enumerator2.get_Current();
					SkillEditor.SaveActions(current2);
				}
			}
		}
		public static void RenameVariableInObject(object obj, NamedVariable variable, string newName, int currentDepth)
		{
			if (obj == null || currentDepth >= 7)
			{
				return;
			}
			IEnumerable<FieldInfo> serializedFields = TypeHelpers.GetSerializedFields(obj.GetType());
			using (IEnumerator<FieldInfo> enumerator = serializedFields.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FieldInfo current = enumerator.get_Current();
					SkillBuilder.RenameVariableInField(obj, current, variable, newName, currentDepth + 1);
				}
			}
		}
		public static void RenameVariableInField(object obj, FieldInfo field, NamedVariable variable, string newName, int currentDepth)
		{
			Type fieldType = field.get_FieldType();
			object value = field.GetValue(obj);
			if (value == null)
			{
				return;
			}
			if (fieldType.IsSubclassOf(typeof(NamedVariable)))
			{
				NamedVariable namedVariable = (NamedVariable)value;
				if (namedVariable.get_Name() == variable.get_Name())
				{
					namedVariable.set_Name(newName);
					return;
				}
			}
			else
			{
				if (fieldType == typeof(SkillVar))
				{
					SkillVar fsmVar = (SkillVar)value;
					if (fsmVar.get_NamedVar().get_Name() == variable.get_Name())
					{
						fsmVar.set_NamedVar(variable);
						fsmVar.variableName = newName;
						return;
					}
				}
				else
				{
					if (fieldType.get_IsArray())
					{
						Array array = (Array)value;
						Type elementType = fieldType.GetElementType();
						for (int i = 0; i < array.get_Length(); i++)
						{
							SkillBuilder.RenameVariableInArray(array, elementType, i, variable, newName, currentDepth);
						}
						return;
					}
					if (fieldType.get_IsClass())
					{
						SkillBuilder.RenameVariableInObject(value, variable, newName, currentDepth);
					}
				}
			}
		}
		public static void RenameVariableInArray(Array array, Type type, int elementIndex, NamedVariable variable, string newName, int currentDepth)
		{
			object value = array.GetValue(elementIndex);
			if (value == null)
			{
				return;
			}
			if (type.IsSubclassOf(typeof(NamedVariable)))
			{
				NamedVariable namedVariable = (NamedVariable)value;
				if (namedVariable.get_Name() == variable.get_Name())
				{
					namedVariable.set_Name(newName);
					array.SetValue(namedVariable, elementIndex);
					return;
				}
			}
			else
			{
				if (type == typeof(SkillVar))
				{
					SkillVar fsmVar = (SkillVar)value;
					if (fsmVar.variableName == variable.get_Name())
					{
						fsmVar.set_NamedVar(variable);
						return;
					}
				}
				else
				{
					if (type.get_IsClass())
					{
						SkillBuilder.RenameVariableInObject(value, variable, newName, currentDepth);
					}
				}
			}
		}
		[Localizable(false)]
		public static SkillTemplate CreateTemplate()
		{
			string templatesDirectory = SkillPaths.GetTemplatesDirectory();
			string text = EditorUtility.SaveFilePanel(Strings.get_Dialog_Save_Template(), templatesDirectory, "template", "asset");
			if (text.get_Length() == 0)
			{
				return null;
			}
			if (!text.Contains(Application.get_dataPath()))
			{
				EditorUtility.DisplayDialog(Strings.get_ProductName(), Strings.get_Dialog_Templates_can_only_be_saved_in_the_Project_s_Assets_folder(), Strings.get_OK());
				return null;
			}
			SkillTemplate fsmTemplate = (SkillTemplate)ScriptableObject.CreateInstance(typeof(SkillTemplate));
			fsmTemplate.fsm = new Skill();
			fsmTemplate.fsm.Reset(null);
			fsmTemplate.fsm.set_UsedInTemplate(fsmTemplate);
			fsmTemplate.set_Category(Strings.get_Label_General());
			AssetDatabase.CreateAsset(fsmTemplate, text.Substring(Application.get_dataPath().get_Length() - 6));
			return fsmTemplate;
		}
		public static SkillTemplate CreateTemplate(Skill fsm)
		{
			SkillTemplate fsmTemplate = SkillBuilder.CreateTemplate();
			if (fsmTemplate == null)
			{
				return null;
			}
			SkillBuilder.CopyFsmToTemplate(fsm, fsmTemplate);
			EditorUtility.SetDirty(fsmTemplate);
			FsmErrorChecker.CheckFsmForErrors(fsmTemplate.fsm, true);
			if (FsmErrorChecker.CountFsmErrors(fsmTemplate.fsm) > 0 && EditorUtility.DisplayDialog(Strings.get_Dialog_Save_Template(), Strings.get_Dialog_CreateTemplate_Errors(), Strings.get_Dialog_Option_Select_Template(), Strings.get_Dialog_Option_Continue()))
			{
				SkillEditor.SelectFsm(fsmTemplate.fsm);
			}
			return fsmTemplate;
		}
		public void CreateTemplate(List<SkillState> states)
		{
			SkillTemplate fsmTemplate = SkillBuilder.CreateTemplate();
			if (fsmTemplate == null)
			{
				return;
			}
			this.CopyStatesToTemplate(states, fsmTemplate);
			EditorUtility.SetDirty(fsmTemplate);
		}
		public static void AddFsmToSelected()
		{
			SkillBuilder.AddFsmToSelected(null);
		}
		public static void AddFsmToSelected(SkillTemplate useTemplate)
		{
			GameObject[] gameObjects = Selection.get_gameObjects();
			if (gameObjects.Length == 0)
			{
				return;
			}
			if (gameObjects.Length > 1)
			{
				if (!Dialogs.AreYouSure(Strings.get_Dialog_Add_FSM(), Strings.get_Dialog_Add_FSM_to_multiple_objects_()))
				{
					return;
				}
				GameObject[] array = gameObjects;
				for (int i = 0; i < array.Length; i++)
				{
					GameObject go = array[i];
					SkillBuilder.AddFsmToGameObject(go, false, useTemplate);
				}
			}
			else
			{
				PlayMakerFSM playMakerFSM = SkillBuilder.AddFsmToGameObject(Selection.get_activeGameObject(), true, useTemplate);
				if (playMakerFSM != null)
				{
					SkillEditor.SelectFsm(playMakerFSM.get_Fsm());
					SkillEditor.SelectState(playMakerFSM.get_Fsm().get_States()[0], false);
					if (!AssetDatabase.Contains(Selection.get_activeGameObject()))
					{
						Undo.RegisterCreatedObjectUndo(playMakerFSM, Strings.get_Command_Add_FSM_to_Selected());
					}
				}
			}
			SkillEditor.RebuildFsmList();
		}
		public static PlayMakerFSM AddFsmToGameObject(GameObject go, bool undo, SkillTemplate useTemplate = null)
		{
			if (go != null)
			{
				string uniqueFsmName = Labels.GetUniqueFsmName(go);
				PlayMakerFSM playMakerFSM = SkillBuilder.DoAddFsmComponent(go);
				playMakerFSM.get_Fsm().set_Name(uniqueFsmName);
				if (useTemplate != null)
				{
					playMakerFSM.SetFsmTemplate(useTemplate);
				}
				if (undo && !AssetDatabase.Contains(go))
				{
					Undo.RegisterCreatedObjectUndo(playMakerFSM, Strings.get_Command_Add_FSM_to_Selected());
				}
				return playMakerFSM;
			}
			return null;
		}
		private static PlayMakerFSM DoAddFsmComponent(GameObject go)
		{
			PlayMakerFSM result = go.AddComponent<PlayMakerFSM>();
			SkillEditor.RebuildFsmList();
			return result;
		}
		public static Skill AddTemplateToSelected(SkillTemplate fsmTemplate)
		{
			if (fsmTemplate == null)
			{
				return null;
			}
			GameObject[] gameObjects = Selection.get_gameObjects();
			if (gameObjects.Length == 0)
			{
				return null;
			}
			if (gameObjects.Length > 1 && !Dialogs.AreYouSure(Strings.get_Dialog_Add_FSM_Template(), Strings.get_Dialog_Add_FSM_Template_to_multiple_objects_()))
			{
				return null;
			}
			return SkillBuilder.DoAddTemplateToSelected(fsmTemplate);
		}
		public static Skill PasteFsmToSelected()
		{
			if (SkillBuilder.clipboard == null)
			{
				return null;
			}
			GameObject[] gameObjects = Selection.get_gameObjects();
			if (gameObjects.Length == 0)
			{
				return null;
			}
			if (gameObjects.Length > 1 && !Dialogs.AreYouSure(Strings.get_Command_Paste_FSM(), Strings.get_Dialog_Paste_FSM_to_multiple_objects()))
			{
				return null;
			}
			return SkillBuilder.DoAddTemplateToSelected(SkillBuilder.clipboard);
		}
		private static Skill DoAddTemplateToSelected(SkillTemplate fsmTemplate)
		{
			Skill fsm = null;
			GameObject[] gameObjects = Selection.get_gameObjects();
			for (int i = 0; i < gameObjects.Length; i++)
			{
				GameObject go = gameObjects[i];
				fsm = SkillBuilder.DoAddTemplate(go, fsmTemplate);
				Undo.RegisterCreatedObjectUndo(fsm.get_OwnerObject(), Strings.get_Command_Add_Template_to_Selected());
			}
			SkillEditor.RebuildFsmList();
			return fsm;
		}
		public static Skill AddTemplate(SkillTemplate fsmTemplate)
		{
			if (fsmTemplate == null || SkillEditor.SelectedFsmGameObject == null)
			{
				return null;
			}
			return SkillBuilder.DoAddTemplate(SkillEditor.SelectedFsmGameObject, fsmTemplate);
		}
		private static Skill DoAddTemplate(GameObject go, SkillTemplate fsmTemplate)
		{
			if (fsmTemplate == null || go == null)
			{
				return null;
			}
			PlayMakerFSM playMakerFSM = SkillBuilder.DoAddFsmComponent(go);
			playMakerFSM.get_Fsm().set_Name(fsmTemplate.fsm.get_Name());
			playMakerFSM.get_Fsm().set_States(SkillBuilder.CopyStates(fsmTemplate.fsm.get_States()));
			playMakerFSM.get_Fsm().set_GlobalTransitions(SkillBuilder.CopyTransitions(fsmTemplate.fsm.get_GlobalTransitions()));
			playMakerFSM.get_Fsm().set_Events(SkillBuilder.CopyEvents(fsmTemplate.fsm.get_Events()));
			playMakerFSM.get_Fsm().set_Variables(SkillBuilder.CopyVariables(fsmTemplate.fsm.get_Variables()));
			playMakerFSM.get_Fsm().set_Description(fsmTemplate.fsm.get_Description());
			playMakerFSM.get_Fsm().set_Watermark(fsmTemplate.fsm.get_Watermark());
			playMakerFSM.get_Fsm().set_StartState(fsmTemplate.fsm.get_StartState());
			playMakerFSM.get_Fsm().ExposedEvents = new List<SkillEvent>(SkillBuilder.CopyEvents(fsmTemplate.fsm.ExposedEvents));
			Undo.RegisterCreatedObjectUndo(playMakerFSM, Strings.get_Menu_Add_Template());
			bool flag = false;
			SkillState[] states = playMakerFSM.get_Fsm().get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				FsmGraphView.TranslateState(fsmState, new Vector2(100f, 100f));
				if (fsmState.get_Name() == playMakerFSM.get_Fsm().get_StartState())
				{
					flag = true;
				}
			}
			if (!flag)
			{
				EditorUtility.DisplayDialog(Strings.get_Command_Set_Start_State(), Strings.get_Dialog_Template_Missing_Start_State(), Strings.get_OK());
				playMakerFSM.get_Fsm().set_StartState(playMakerFSM.get_Fsm().get_States()[0].get_Name());
			}
			return playMakerFSM.get_Fsm();
		}
		private static void CreateClipboard()
		{
			SkillBuilder.clipboard = (SkillTemplate)ScriptableObject.CreateInstance(typeof(SkillTemplate));
		}
		public static void CopyFsmToClipboard(Skill fsm)
		{
			if (fsm == null)
			{
				return;
			}
			if (SkillBuilder.clipboard == null)
			{
				SkillBuilder.CreateClipboard();
			}
			SkillBuilder.CopyFsmToTemplate(fsm, SkillBuilder.clipboard);
		}
		public static void CopyFsmToTemplate(Skill fsm, SkillTemplate template)
		{
			Skill fsm2 = new Skill();
			fsm2.set_Name(fsm.get_Name());
			fsm2.set_Description(fsm.get_Description());
			fsm2.set_Watermark(fsm.get_Watermark());
			fsm2.set_StartState(fsm.get_StartState());
			fsm2.set_States(SkillBuilder.CopyStates(fsm.get_States()));
			fsm2.set_GlobalTransitions(SkillBuilder.CopyTransitions(fsm.get_GlobalTransitions()));
			fsm2.set_Events(SkillBuilder.CopyEvents(fsm.get_Events()));
			fsm2.set_Variables(SkillBuilder.CopyVariables(fsm.get_Variables()));
			fsm2.set_UsedInTemplate(template);
			fsm2.ExposedEvents = new List<SkillEvent>(SkillBuilder.CopyEvents(fsm.ExposedEvents));
			template.fsm = fsm2;
			template.fsm.set_DataVersion(fsm.get_DataVersion());
		}
		public void CopyStatesToClipboard(List<SkillState> states)
		{
			if (SkillBuilder.clipboard == null)
			{
				SkillBuilder.CreateClipboard();
			}
			this.CopyStatesToTemplate(states, SkillBuilder.clipboard);
		}
		public void CopyStatesToTemplate(List<SkillState> states, SkillTemplate template)
		{
			Skill fsm = new Skill();
			fsm.set_DataVersion(SkillEditor.SelectedFsm.get_DataVersion());
			fsm.set_Variables(this.CopyStateVariables(states, this.targetFsm.get_Variables()));
			fsm.set_States(SkillBuilder.CopyStates(states));
			template.fsm = fsm;
			template.fsm.Init(null);
			using (List<SkillState>.Enumerator enumerator = states.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillState current = enumerator.get_Current();
					if (current.get_Fsm().get_StartState() == current.get_Name())
					{
						template.fsm.set_StartState(current.get_Name());
						template.fsm.set_UsedInTemplate(template);
					}
				}
			}
			List<SkillTransition> list = new List<SkillTransition>();
			using (List<SkillState>.Enumerator enumerator2 = states.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					SkillState current2 = enumerator2.get_Current();
					SkillTransition[] globalTransitions = current2.get_Fsm().get_GlobalTransitions();
					for (int i = 0; i < globalTransitions.Length; i++)
					{
						SkillTransition fsmTransition = globalTransitions[i];
						if (fsmTransition.get_ToState() == current2.get_Name())
						{
							list.Add(SkillBuilder.CopyTransition(fsmTransition));
						}
					}
				}
			}
			template.fsm.set_GlobalTransitions(list.ToArray());
			Vector2 vector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
			SkillState[] states2 = template.fsm.get_States();
			for (int j = 0; j < states2.Length; j++)
			{
				SkillState fsmState = states2[j];
				vector.x = Mathf.Min(fsmState.get_Position().get_x(), vector.x);
				vector.y = Mathf.Min(fsmState.get_Position().get_y(), vector.y);
			}
			SkillState[] states3 = template.fsm.get_States();
			for (int k = 0; k < states3.Length; k++)
			{
				SkillState fsmState2 = states3[k];
				fsmState2.set_Position(new Rect(fsmState2.get_Position().get_x() - vector.x, fsmState2.get_Position().get_y() - vector.y, fsmState2.get_Position().get_width(), fsmState2.get_Position().get_height()));
			}
			EditorUtility.SetDirty(template);
		}
		private static SkillState[] CopyStates(IEnumerable<SkillState> states)
		{
			List<SkillState> list = new List<SkillState>();
			using (IEnumerator<SkillState> enumerator = states.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillState current = enumerator.get_Current();
					list.Add(SkillBuilder.CopyState(current));
				}
			}
			return list.ToArray();
		}
		private static SkillState CopyState(SkillState state)
		{
			SkillState fsmState = new SkillState(null);
			fsmState.set_Name(state.get_Name());
			fsmState.set_ColorIndex(state.get_ColorIndex());
			fsmState.set_Description(state.get_Description());
			fsmState.set_HideUnused(state.get_HideUnused());
			fsmState.set_IsSequence(state.get_IsSequence());
			fsmState.set_Transitions(SkillBuilder.CopyTransitions(state.get_Transitions()));
			fsmState.set_Position(new Rect(state.get_Position()));
			SkillState fsmState2 = fsmState;
			fsmState2.CopyActionData(state);
			return fsmState2;
		}
		public void CopyActionsToClipboard(SkillState state, List<SkillStateAction> actions)
		{
			if (SkillBuilder.clipboard == null)
			{
				SkillBuilder.CreateClipboard();
			}
			this.CopyActionsToTemplate(state, actions, SkillBuilder.clipboard);
		}
		public void CopyActionsToTemplate(SkillState state, List<SkillStateAction> actions, SkillTemplate template)
		{
			template.fsm = Skill.NewTempFsm();
			Skill arg_27_0 = template.fsm;
			List<SkillState> list = new List<SkillState>();
			list.Add(state);
			arg_27_0.set_States(SkillBuilder.CopyStates(list));
			template.fsm.set_DataVersion(state.get_Fsm().get_DataVersion());
			template.fsm.set_Variables(new SkillVariables(state.get_Fsm().get_Variables()));
			template.fsm.Init(null);
			SkillState fsmState = template.fsm.get_States()[0];
			List<SkillStateAction> list2 = new List<SkillStateAction>();
			List<SkillStateAction> list3 = new List<SkillStateAction>();
			int num = 0;
			SkillStateAction[] actions2 = state.get_Actions();
			for (int i = 0; i < actions2.Length; i++)
			{
				SkillStateAction fsmStateAction = actions2[i];
				if (!actions.Contains(fsmStateAction))
				{
					list3.Add(fsmState.get_Actions()[num]);
				}
				else
				{
					list2.Add(fsmStateAction);
				}
				num++;
			}
			using (List<SkillStateAction>.Enumerator enumerator = list3.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillStateAction current = enumerator.get_Current();
					fsmState.set_Actions(ArrayUtility.Remove<SkillStateAction>(fsmState.get_Actions(), current));
				}
			}
			Skill arg_137_0 = template.fsm;
			SkillVariables fsmVariables = new SkillVariables();
			fsmVariables.set_Categories(ArrayUtility.Copy<string>(this.targetFsm.get_Variables().get_Categories()));
			arg_137_0.set_Variables(fsmVariables);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<string> list4 = SkillSearch.FindVariablesUsedByActions(this.targetFsm, list2);
			using (List<string>.Enumerator enumerator2 = list4.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					string current2 = enumerator2.get_Current();
					NamedVariable namedVariable = this.targetFsm.get_Variables().FindVariable(current2);
					SkillVariable.AddVariable(template.fsm.get_Variables(), namedVariable);
					string category = SkillVariable.GetCategory(this.targetFsm.get_Variables(), namedVariable);
					if (!string.IsNullOrEmpty(category))
					{
						dictionary.Add(namedVariable.get_Name(), category);
					}
				}
			}
			SkillBuilder.BuildCategories(template.fsm.get_Variables(), dictionary);
			fsmState.SaveActions();
		}
		private static SkillTransition[] CopyTransitions(IEnumerable<SkillTransition> transitions)
		{
			List<SkillTransition> list = new List<SkillTransition>();
			using (IEnumerator<SkillTransition> enumerator = transitions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillTransition current = enumerator.get_Current();
					list.Add(SkillBuilder.CopyTransition(current));
				}
			}
			return list.ToArray();
		}
		private static SkillTransition CopyTransition(SkillTransition transiton)
		{
			SkillTransition fsmTransition = new SkillTransition();
			fsmTransition.set_FsmEvent(new SkillEvent(transiton.get_EventName()));
			fsmTransition.set_ToState(transiton.get_ToState());
			fsmTransition.set_ColorIndex(transiton.get_ColorIndex());
			fsmTransition.set_LinkConstraint(transiton.get_LinkConstraint());
			fsmTransition.set_LinkStyle(transiton.get_LinkStyle());
			return fsmTransition;
		}
		private static SkillEvent[] CopyEvents(IEnumerable<SkillEvent> events)
		{
			List<SkillEvent> list = new List<SkillEvent>();
			using (IEnumerator<SkillEvent> enumerator = events.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					list.Add(new SkillEvent(current));
				}
			}
			return list.ToArray();
		}
		public static SkillVariables CopyVariables(SkillVariables variables)
		{
			return new SkillVariables(variables);
		}
		private SkillVariables CopyStateVariables(SkillState state, SkillVariables variables)
		{
			List<SkillState> list = new List<SkillState>();
			list.Add(state);
			return this.CopyStateVariables(list, variables);
		}
		private SkillVariables CopyStateVariables(List<SkillState> states, SkillVariables variables)
		{
			if (states == null || states.get_Count() == 0 || variables == null)
			{
				return new SkillVariables();
			}
			List<string> list = SkillSearch.FindVariablesUsedByStates(this.targetFsm, states);
			List<SkillFloat> list2 = new List<SkillFloat>();
			List<SkillInt> list3 = new List<SkillInt>();
			List<SkillBool> list4 = new List<SkillBool>();
			List<SkillGameObject> list5 = new List<SkillGameObject>();
			List<SkillColor> list6 = new List<SkillColor>();
			List<SkillVector2> list7 = new List<SkillVector2>();
			List<SkillVector3> list8 = new List<SkillVector3>();
			List<SkillRect> list9 = new List<SkillRect>();
			List<SkillQuaternion> list10 = new List<SkillQuaternion>();
			List<SkillObject> list11 = new List<SkillObject>();
			List<SkillMaterial> list12 = new List<SkillMaterial>();
			List<SkillTexture> list13 = new List<SkillTexture>();
			List<SkillString> list14 = new List<SkillString>();
			List<SkillEnum> list15 = new List<SkillEnum>();
			List<SkillArray> list16 = new List<SkillArray>();
			SkillFloat[] floatVariables = variables.get_FloatVariables();
			for (int i = 0; i < floatVariables.Length; i++)
			{
				SkillFloat fsmFloat = floatVariables[i];
				if (list.Contains(fsmFloat.get_Name()))
				{
					list2.Add(new SkillFloat(fsmFloat));
				}
			}
			SkillInt[] intVariables = variables.get_IntVariables();
			for (int j = 0; j < intVariables.Length; j++)
			{
				SkillInt fsmInt = intVariables[j];
				if (list.Contains(fsmInt.get_Name()))
				{
					list3.Add(new SkillInt(fsmInt));
				}
			}
			SkillBool[] boolVariables = variables.get_BoolVariables();
			for (int k = 0; k < boolVariables.Length; k++)
			{
				SkillBool fsmBool = boolVariables[k];
				if (list.Contains(fsmBool.get_Name()))
				{
					list4.Add(new SkillBool(fsmBool));
				}
			}
			SkillGameObject[] gameObjectVariables = variables.get_GameObjectVariables();
			for (int l = 0; l < gameObjectVariables.Length; l++)
			{
				SkillGameObject fsmGameObject = gameObjectVariables[l];
				if (list.Contains(fsmGameObject.get_Name()))
				{
					list5.Add(new SkillGameObject(fsmGameObject));
				}
			}
			SkillColor[] colorVariables = variables.get_ColorVariables();
			for (int m = 0; m < colorVariables.Length; m++)
			{
				SkillColor fsmColor = colorVariables[m];
				if (list.Contains(fsmColor.get_Name()))
				{
					list6.Add(new SkillColor(fsmColor));
				}
			}
			SkillVector2[] vector2Variables = variables.get_Vector2Variables();
			for (int n = 0; n < vector2Variables.Length; n++)
			{
				SkillVector2 fsmVector = vector2Variables[n];
				if (list.Contains(fsmVector.get_Name()))
				{
					list7.Add(new SkillVector2(fsmVector));
				}
			}
			SkillVector3[] vector3Variables = variables.get_Vector3Variables();
			for (int num = 0; num < vector3Variables.Length; num++)
			{
				SkillVector3 fsmVector2 = vector3Variables[num];
				if (list.Contains(fsmVector2.get_Name()))
				{
					list8.Add(new SkillVector3(fsmVector2));
				}
			}
			SkillRect[] rectVariables = variables.get_RectVariables();
			for (int num2 = 0; num2 < rectVariables.Length; num2++)
			{
				SkillRect fsmRect = rectVariables[num2];
				if (list.Contains(fsmRect.get_Name()))
				{
					list9.Add(new SkillRect(fsmRect));
				}
			}
			SkillQuaternion[] quaternionVariables = variables.get_QuaternionVariables();
			for (int num3 = 0; num3 < quaternionVariables.Length; num3++)
			{
				SkillQuaternion fsmQuaternion = quaternionVariables[num3];
				if (list.Contains(fsmQuaternion.get_Name()))
				{
					list10.Add(new SkillQuaternion(fsmQuaternion));
				}
			}
			SkillObject[] objectVariables = variables.get_ObjectVariables();
			for (int num4 = 0; num4 < objectVariables.Length; num4++)
			{
				SkillObject fsmObject = objectVariables[num4];
				if (list.Contains(fsmObject.get_Name()))
				{
					list11.Add(new SkillObject(fsmObject));
				}
			}
			SkillMaterial[] materialVariables = variables.get_MaterialVariables();
			for (int num5 = 0; num5 < materialVariables.Length; num5++)
			{
				SkillMaterial fsmMaterial = materialVariables[num5];
				if (list.Contains(fsmMaterial.get_Name()))
				{
					list12.Add(new SkillMaterial(fsmMaterial));
				}
			}
			SkillTexture[] textureVariables = variables.get_TextureVariables();
			for (int num6 = 0; num6 < textureVariables.Length; num6++)
			{
				SkillTexture fsmTexture = textureVariables[num6];
				if (list.Contains(fsmTexture.get_Name()))
				{
					list13.Add(new SkillTexture(fsmTexture));
				}
			}
			SkillString[] stringVariables = variables.get_StringVariables();
			for (int num7 = 0; num7 < stringVariables.Length; num7++)
			{
				SkillString fsmString = stringVariables[num7];
				if (list.Contains(fsmString.get_Name()))
				{
					list14.Add(new SkillString(fsmString));
				}
			}
			SkillEnum[] enumVariables = variables.get_EnumVariables();
			for (int num8 = 0; num8 < enumVariables.Length; num8++)
			{
				SkillEnum fsmEnum = enumVariables[num8];
				if (list.Contains(fsmEnum.get_Name()))
				{
					list15.Add(new SkillEnum(fsmEnum));
				}
			}
			SkillArray[] arrayVariables = variables.get_ArrayVariables();
			for (int i = 0; i < arrayVariables.Length; i++)
			{
				SkillArray fsmArray = arrayVariables[i];
				if (list.Contains(fsmArray.get_Name()))
				{
					list16.Add(new SkillArray(fsmArray));
				}
			}
			SkillVariables fsmVariables = new SkillVariables();
			fsmVariables.set_FloatVariables(list2.ToArray());
			fsmVariables.set_IntVariables(list3.ToArray());
			fsmVariables.set_BoolVariables(list4.ToArray());
			fsmVariables.set_GameObjectVariables(list5.ToArray());
			fsmVariables.set_ColorVariables(list6.ToArray());
			fsmVariables.set_Vector2Variables(list7.ToArray());
			fsmVariables.set_Vector3Variables(list8.ToArray());
			fsmVariables.set_RectVariables(list9.ToArray());
			fsmVariables.set_StringVariables(list14.ToArray());
			fsmVariables.set_QuaternionVariables(list10.ToArray());
			fsmVariables.set_ObjectVariables(list11.ToArray());
			fsmVariables.set_MaterialVariables(list12.ToArray());
			fsmVariables.set_TextureVariables(list13.ToArray());
			fsmVariables.set_EnumVariables(list15.ToArray());
			fsmVariables.set_ArrayVariables(list16.ToArray());
			return fsmVariables;
		}
		public static bool VariableInSourceExistsInTarget(Object source, Skill target)
		{
			List<SkillVariable> fsmVariableList = SkillVariable.GetFsmVariableList(source);
			List<SkillVariable> fsmVariableList2 = SkillVariable.GetFsmVariableList(target.get_OwnerObject());
			using (List<SkillVariable>.Enumerator enumerator = fsmVariableList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					if (SkillVariable.VariableNameUsed(fsmVariableList2, current.Name))
					{
						return true;
					}
				}
			}
			return false;
		}
		public static void PasteVariables(Skill toFsm, Object fromFsm, bool overwriteValues = false)
		{
			List<SkillVariable> fsmVariableList = SkillVariable.GetFsmVariableList(fromFsm);
			List<SkillVariable> fsmVariableList2 = SkillVariable.GetFsmVariableList(toFsm.get_OwnerObject());
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			using (List<SkillVariable>.Enumerator enumerator = fsmVariableList2.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					if (!string.IsNullOrEmpty(current.Category))
					{
						dictionary.Add(current.Name, current.Category);
					}
				}
			}
			using (List<SkillVariable>.Enumerator enumerator2 = fsmVariableList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					SkillVariable current2 = enumerator2.get_Current();
					if (!string.IsNullOrEmpty(current2.Category) && !dictionary.ContainsKey(current2.Name))
					{
						dictionary.Add(current2.Name, current2.Category);
					}
				}
			}
			using (List<SkillVariable>.Enumerator enumerator3 = fsmVariableList.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					SkillVariable current3 = enumerator3.get_Current();
					if (SkillVariable.VariableNameUsed(fsmVariableList2, current3.Name))
					{
						if (SkillVariable.GetVariableType(fsmVariableList2, current3.Name) != current3.Type)
						{
							Debug.LogError(string.Format(Strings.get_Error_Variable_name_already_exists_and_is_of_different_type(), current3.Name));
						}
						if (overwriteValues)
						{
							toFsm.get_Variables().GetVariable(current3.Name).set_RawValue(current3.NamedVar.get_RawValue());
						}
					}
					else
					{
						switch (current3.Type)
						{
						case 0:
							toFsm.get_Variables().set_FloatVariables(ArrayUtility.Add<SkillFloat>(toFsm.get_Variables().get_FloatVariables(), new SkillFloat((SkillFloat)current3.NamedVar)));
							break;
						case 1:
							toFsm.get_Variables().set_IntVariables(ArrayUtility.Add<SkillInt>(toFsm.get_Variables().get_IntVariables(), new SkillInt((SkillInt)current3.NamedVar)));
							break;
						case 2:
							toFsm.get_Variables().set_BoolVariables(ArrayUtility.Add<SkillBool>(toFsm.get_Variables().get_BoolVariables(), new SkillBool((SkillBool)current3.NamedVar)));
							break;
						case 3:
							toFsm.get_Variables().set_GameObjectVariables(ArrayUtility.Add<SkillGameObject>(toFsm.get_Variables().get_GameObjectVariables(), new SkillGameObject((SkillGameObject)current3.NamedVar)));
							break;
						case 4:
							toFsm.get_Variables().set_StringVariables(ArrayUtility.Add<SkillString>(toFsm.get_Variables().get_StringVariables(), new SkillString((SkillString)current3.NamedVar)));
							break;
						case 5:
							toFsm.get_Variables().set_Vector2Variables(ArrayUtility.Add<SkillVector2>(toFsm.get_Variables().get_Vector2Variables(), new SkillVector2((SkillVector2)current3.NamedVar)));
							break;
						case 6:
							toFsm.get_Variables().set_Vector3Variables(ArrayUtility.Add<SkillVector3>(toFsm.get_Variables().get_Vector3Variables(), new SkillVector3((SkillVector3)current3.NamedVar)));
							break;
						case 7:
							toFsm.get_Variables().set_ColorVariables(ArrayUtility.Add<SkillColor>(toFsm.get_Variables().get_ColorVariables(), new SkillColor((SkillColor)current3.NamedVar)));
							break;
						case 8:
							toFsm.get_Variables().set_RectVariables(ArrayUtility.Add<SkillRect>(toFsm.get_Variables().get_RectVariables(), new SkillRect((SkillRect)current3.NamedVar)));
							break;
						case 9:
							toFsm.get_Variables().set_MaterialVariables(ArrayUtility.Add<SkillMaterial>(toFsm.get_Variables().get_MaterialVariables(), new SkillMaterial((SkillMaterial)current3.NamedVar)));
							break;
						case 10:
							toFsm.get_Variables().set_TextureVariables(ArrayUtility.Add<SkillTexture>(toFsm.get_Variables().get_TextureVariables(), new SkillTexture((SkillTexture)current3.NamedVar)));
							break;
						case 11:
							toFsm.get_Variables().set_QuaternionVariables(ArrayUtility.Add<SkillQuaternion>(toFsm.get_Variables().get_QuaternionVariables(), new SkillQuaternion((SkillQuaternion)current3.NamedVar)));
							break;
						case 12:
							toFsm.get_Variables().set_ObjectVariables(ArrayUtility.Add<SkillObject>(toFsm.get_Variables().get_ObjectVariables(), new SkillObject((SkillObject)current3.NamedVar)));
							break;
						case 13:
							toFsm.get_Variables().set_ArrayVariables(ArrayUtility.Add<SkillArray>(toFsm.get_Variables().get_ArrayVariables(), new SkillArray((SkillArray)current3.NamedVar)));
							break;
						case 14:
							toFsm.get_Variables().set_EnumVariables(ArrayUtility.Add<SkillEnum>(toFsm.get_Variables().get_EnumVariables(), new SkillEnum((SkillEnum)current3.NamedVar)));
							break;
						}
					}
				}
			}
			SkillBuilder.BuildCategories(toFsm.get_Variables(), dictionary);
		}
		private static void BuildCategories(SkillVariables variables, Dictionary<string, string> categoryMap)
		{
			NamedVariable[] allNamedVariables = variables.GetAllNamedVariables();
			variables.set_CategoryIDs(new int[allNamedVariables.Length]);
			for (int i = 0; i < allNamedVariables.Length; i++)
			{
				NamedVariable namedVariable = variables.GetAllNamedVariables()[i];
				string category;
				if (categoryMap.TryGetValue(namedVariable.get_Name(), ref category))
				{
					variables.get_CategoryIDs()[i] = SkillVariable.GetCategoryIndex(variables, category);
				}
				else
				{
					variables.get_CategoryIDs()[i] = 0;
				}
			}
		}
		public static string MergeVariables(Object source, Object targetOwner)
		{
			string text = "";
			List<SkillVariable> fsmVariableList = SkillVariable.GetFsmVariableList(source);
			List<SkillVariable> fsmVariableList2 = SkillVariable.GetFsmVariableList(targetOwner);
			SkillVariables variables = SkillVariable.GetVariables(targetOwner);
			using (List<SkillVariable>.Enumerator enumerator = fsmVariableList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					if (SkillVariable.VariableNameUsed(fsmVariableList2, current.Name))
					{
						if (SkillVariable.GetVariableType(fsmVariableList2, current.Name) != current.Type)
						{
							Debug.Log(string.Format(Strings.get_Error_Variable_name_already_exists_and_is_of_different_type(), current.Name));
							text = text + string.Format(Strings.get_Error_Variable_name_already_exists_and_is_of_different_type(), current.Name) + Environment.get_NewLine();
						}
					}
					else
					{
						switch (current.Type)
						{
						case 0:
							variables.set_FloatVariables(ArrayUtility.Add<SkillFloat>(variables.get_FloatVariables(), new SkillFloat((SkillFloat)current.NamedVar)));
							break;
						case 1:
							variables.set_IntVariables(ArrayUtility.Add<SkillInt>(variables.get_IntVariables(), new SkillInt((SkillInt)current.NamedVar)));
							break;
						case 2:
							variables.set_BoolVariables(ArrayUtility.Add<SkillBool>(variables.get_BoolVariables(), new SkillBool((SkillBool)current.NamedVar)));
							break;
						case 3:
							variables.set_GameObjectVariables(ArrayUtility.Add<SkillGameObject>(variables.get_GameObjectVariables(), new SkillGameObject((SkillGameObject)current.NamedVar)));
							break;
						case 4:
							variables.set_StringVariables(ArrayUtility.Add<SkillString>(variables.get_StringVariables(), new SkillString((SkillString)current.NamedVar)));
							break;
						case 5:
							variables.set_Vector2Variables(ArrayUtility.Add<SkillVector2>(variables.get_Vector2Variables(), new SkillVector2((SkillVector2)current.NamedVar)));
							break;
						case 6:
							variables.set_Vector3Variables(ArrayUtility.Add<SkillVector3>(variables.get_Vector3Variables(), new SkillVector3((SkillVector3)current.NamedVar)));
							break;
						case 7:
							variables.set_ColorVariables(ArrayUtility.Add<SkillColor>(variables.get_ColorVariables(), new SkillColor((SkillColor)current.NamedVar)));
							break;
						case 8:
							variables.set_RectVariables(ArrayUtility.Add<SkillRect>(variables.get_RectVariables(), new SkillRect((SkillRect)current.NamedVar)));
							break;
						case 9:
							variables.set_MaterialVariables(ArrayUtility.Add<SkillMaterial>(variables.get_MaterialVariables(), new SkillMaterial((SkillMaterial)current.NamedVar)));
							break;
						case 10:
							variables.set_TextureVariables(ArrayUtility.Add<SkillTexture>(variables.get_TextureVariables(), new SkillTexture((SkillTexture)current.NamedVar)));
							break;
						case 11:
							variables.set_QuaternionVariables(ArrayUtility.Add<SkillQuaternion>(variables.get_QuaternionVariables(), new SkillQuaternion((SkillQuaternion)current.NamedVar)));
							break;
						case 12:
							variables.set_ObjectVariables(ArrayUtility.Add<SkillObject>(variables.get_ObjectVariables(), new SkillObject((SkillObject)current.NamedVar)));
							break;
						case 13:
							variables.set_ArrayVariables(ArrayUtility.Add<SkillArray>(variables.get_ArrayVariables(), new SkillArray((SkillArray)current.NamedVar)));
							break;
						case 14:
							variables.set_EnumVariables(ArrayUtility.Add<SkillEnum>(variables.get_EnumVariables(), new SkillEnum((SkillEnum)current.NamedVar)));
							break;
						default:
							Debug.Log(Strings.get_Error_Unknown_variable_type());
							break;
						}
					}
				}
			}
			return text;
		}
		public static string MergeGlobals(PlayMakerGlobals source, PlayMakerGlobals target)
		{
			string result = SkillBuilder.MergeVariables(source, target);
			using (List<string>.Enumerator enumerator = source.get_Events().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.get_Current();
					if (!target.get_Events().Contains(current))
					{
						target.AddEvent(current);
					}
				}
			}
			return result;
		}
		public bool CanPaste()
		{
			return !(SkillBuilder.clipboard == null) && SkillBuilder.clipboard.fsm != null && SkillBuilder.clipboard.fsm.get_States().Length > 0;
		}
		public List<SkillState> PasteStatesFromClipboard(Vector2 position)
		{
			return this.PasteStatesFromTemplate(SkillBuilder.clipboard, position);
		}
		public List<SkillState> PasteStatesFromTemplate(SkillTemplate template, Vector2 position)
		{
			if (template.fsm.get_States().Length == 0)
			{
				return null;
			}
			SkillEditor.RegisterUndo(Strings.get_Menu_GraphView_Paste_States());
			if (this.targetFsm.get_DataVersion() != template.fsm.get_DataVersion())
			{
				template.fsm.set_DataVersion(this.targetFsm.get_DataVersion());
				template.fsm.SaveActions();
			}
			List<SkillState> list = new List<SkillState>();
			SkillState[] states = template.fsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState state = states[i];
				list.Add(SkillBuilder.CopyState(state));
			}
			List<SkillTransition> list2 = new List<SkillTransition>();
			SkillTransition[] globalTransitions = template.fsm.get_GlobalTransitions();
			for (int j = 0; j < globalTransitions.Length; j++)
			{
				SkillTransition transiton = globalTransitions[j];
				list2.Add(SkillBuilder.CopyTransition(transiton));
			}
			List<SkillState> list3 = new List<SkillState>(this.targetFsm.get_States());
			list3.AddRange(list);
			this.targetFsm.set_States(list3.ToArray());
			using (List<SkillState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillState current = enumerator.get_Current();
					current.set_Fsm(this.targetFsm);
					string text = this.GenerateCopyName(current);
					if (template.fsm.get_StartState() == current.get_Name() && Dialogs.YesNoDialog(Strings.get_Dialog_Replace_Start_State(), Strings.get_Dialog_Replace_Start_State_Description()))
					{
						this.targetFsm.set_StartState(text);
					}
					using (List<SkillState>.Enumerator enumerator2 = list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							SkillState current2 = enumerator2.get_Current();
							SkillTransition[] transitions = current2.get_Transitions();
							for (int k = 0; k < transitions.Length; k++)
							{
								SkillTransition fsmTransition = transitions[k];
								if (!SkillBuilder.StateSelectionContainsState(list, fsmTransition.get_ToState()))
								{
									fsmTransition.set_ToState("");
								}
								if (fsmTransition.get_ToState() == current.get_Name())
								{
									fsmTransition.set_ToState(text);
								}
							}
						}
					}
					using (List<SkillTransition>.Enumerator enumerator3 = list2.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							SkillTransition current3 = enumerator3.get_Current();
							if (current3.get_ToState() == current.get_Name())
							{
								current3.set_ToState(text);
							}
						}
					}
					current.set_Name(text);
					FsmGraphView.TranslateState(current, position);
				}
			}
			list2.AddRange(this.targetFsm.get_GlobalTransitions());
			this.targetFsm.set_GlobalTransitions(list2.ToArray());
			using (List<SkillState>.Enumerator enumerator4 = list.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					SkillState current4 = enumerator4.get_Current();
					SkillEditor.GraphView.UpdateStateSize(current4);
				}
			}
			SkillBuilder.PasteVariables(this.targetFsm, template, false);
			this.targetFsm.Reinitialize();
			SkillEditor.SetFsmDirty(true, false);
			SkillEditor.UpdateActionUsage();
			SkillEditor.UpdateFsmInfo();
			Keyboard.ResetFocus();
			return list;
		}
		public List<SkillStateAction> PasteActionsFromTemplate(SkillTemplate template, SkillState toState, int atIndex, bool undo = true)
		{
			if (atIndex == -1 || template == null || template.fsm == null)
			{
				return null;
			}
			SkillState fsmState = SkillBuilder.CopyState(template.fsm.get_States()[0]);
			if (fsmState == null)
			{
				return null;
			}
			if (undo)
			{
				SkillEditor.RegisterUndo(Strings.get_Menu_Paste_Actions());
			}
			fsmState.set_Fsm(template.fsm);
			fsmState.LoadActions();
			List<SkillStateAction> list = new List<SkillStateAction>();
			int actionCount = fsmState.get_ActionData().get_ActionCount();
			for (int i = 0; i < actionCount; i++)
			{
				SkillStateAction fsmStateAction = fsmState.get_ActionData().CreateAction(fsmState, i);
				list.Add(fsmStateAction);
			}
			List<SkillStateAction> list2 = new List<SkillStateAction>(toState.get_Actions());
			list2.InsertRange(atIndex, list);
			toState.set_Actions(list2.ToArray());
			SkillEditor.SaveActions(toState, true);
			SkillBuilder.PasteVariables(this.targetFsm, template, false);
			this.targetFsm.Reinitialize();
			SkillEditor.SetFsmDirty(true, false);
			SkillEditor.UpdateActionUsage();
			SkillEditor.UpdateFsmInfo();
			list = new List<SkillStateAction>();
			for (int j = atIndex; j < atIndex + actionCount; j++)
			{
				list.Add(toState.get_Actions()[j]);
			}
			return list;
		}
		private static bool StateSelectionContainsState(IEnumerable<SkillState> states, string stateName)
		{
			using (IEnumerator<SkillState> enumerator = states.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillState current = enumerator.get_Current();
					if (current.get_Name() == stateName)
					{
						return true;
					}
				}
			}
			return false;
		}
		public void Cleanup()
		{
		}
	}
}
