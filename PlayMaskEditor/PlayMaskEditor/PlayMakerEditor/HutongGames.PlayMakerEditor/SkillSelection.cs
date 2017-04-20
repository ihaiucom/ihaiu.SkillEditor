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
	public class SkillSelection
	{
		[SerializeField]
		private int fsmComponentID;
		[NonSerialized]
		private PlayMakerFSM fsmComponent;
		[SerializeField]
		private SkillTemplate fsmTemplate;
		[SerializeField]
		private string stateName;
		[SerializeField]
		private List<string> stateNames;
		[SerializeField]
		private int transitionIndex;
		[SerializeField]
		private Vector2 scrollPosition;
		[SerializeField]
		private float zoom;
		[NonSerialized]
		private Skill subFsm;
		[NonSerialized]
		private SkillState activeState;
		[NonSerialized]
		private List<SkillState> states;
		[NonSerialized]
		private SkillTransition activeTransition;
		public static SkillSelection None
		{
			get
			{
				return new SkillSelection(null);
			}
		}
		public PlayMakerFSM FsmComponent
		{
			get
			{
				if (this.fsmComponent == null && this.fsmComponentID != 0)
				{
					this.fsmComponent = (EditorUtility.InstanceIDToObject(this.fsmComponentID) as PlayMakerFSM);
				}
				return this.fsmComponent;
			}
		}
		public bool IsOrphaned
		{
			get
			{
				return this.ActiveFsm == null || (!SkillEditor.FsmComponentList.Contains(this.ActiveFsmComponent) && !this.ActiveFsm.get_UsedInTemplate());
			}
		}
		public bool IsGameObjectNull
		{
			get
			{
				return this.ActiveFsm != null && this.ActiveFsmTemplate == null && this.ActiveFsmGameObject == null;
			}
		}
		public PlayMakerFSM ActiveFsmComponent
		{
			get
			{
				return this.FsmComponent;
			}
		}
		public SkillTemplate ActiveFsmTemplate
		{
			get
			{
				return this.fsmTemplate;
			}
		}
		public Skill ActiveFsm
		{
			get
			{
				if (this.subFsm != null)
				{
					return this.subFsm;
				}
				if (this.FsmComponent != null)
				{
					return this.FsmComponent.get_Fsm();
				}
				if (!(this.fsmTemplate != null))
				{
					return null;
				}
				return this.fsmTemplate.fsm;
			}
			private set
			{
				this.subFsm = null;
				this.fsmTemplate = null;
				this.fsmComponent = null;
				if (value != null)
				{
					if (value.get_IsSubFsm())
					{
						this.subFsm = value;
					}
					if (value.get_UsedInTemplate() != null)
					{
						this.fsmTemplate = value.get_UsedInTemplate();
					}
					else
					{
						this.fsmComponent = (value.get_Owner() as PlayMakerFSM);
						if (this.fsmComponent != null)
						{
							this.fsmComponentID = this.fsmComponent.GetInstanceID();
						}
					}
				}
				this.activeState = ((this.ActiveFsm != null) ? this.ActiveFsm.GetState(this.stateName) : null);
				this.activeTransition = ((this.activeState != null) ? this.activeState.GetTransition(this.transitionIndex) : null);
			}
		}
		public SkillState ActiveState
		{
			get
			{
				if (this.ActiveFsm == null)
				{
					this.activeState = null;
				}
				else
				{
					if (this.activeState == null && this.stateName != null)
					{
						this.activeState = this.ActiveFsm.GetState(this.stateName);
					}
				}
				return this.activeState;
			}
			private set
			{
				if (this.ActiveFsm != null)
				{
					this.stateName = ((value != null) ? value.get_Name() : null);
					this.activeState = this.ActiveFsm.GetState(this.stateName);
				}
			}
		}
		public SkillTransition ActiveTransition
		{
			get
			{
				return this.activeTransition;
			}
			set
			{
				this.activeTransition = value;
				if (this.ActiveState != null)
				{
					this.transitionIndex = this.ActiveState.GetTransitionIndex(value);
					return;
				}
				this.transitionIndex = -1;
			}
		}
		public List<string> StateNames
		{
			get
			{
				List<string> arg_18_0;
				if ((arg_18_0 = this.stateNames) == null)
				{
					arg_18_0 = (this.stateNames = new List<string>());
				}
				return arg_18_0;
			}
		}
		public List<SkillState> States
		{
			get
			{
				if (this.states == null)
				{
					this.states = new List<SkillState>();
					if (this.ActiveFsm != null)
					{
						using (List<string>.Enumerator enumerator = this.StateNames.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								string current = enumerator.get_Current();
								this.states.Add(this.ActiveFsm.GetState(current));
							}
						}
					}
				}
				return this.states;
			}
		}
		public string ActiveFsmName
		{
			get
			{
				if (this.ActiveFsm != null)
				{
					return this.ActiveFsm.get_Name();
				}
				return null;
			}
		}
		public MonoBehaviour ActiveFsmOwner
		{
			get
			{
				if (this.ActiveFsm != null)
				{
					return this.ActiveFsm.get_Owner();
				}
				return null;
			}
		}
		public GameObject ActiveFsmGameObject
		{
			get
			{
				if (this.ActiveFsm != null)
				{
					return this.ActiveFsm.get_GameObject();
				}
				return null;
			}
		}
		public PrefabType ActiveFsmPrefabType
		{
			get
			{
				if (!(this.ActiveFsmGameObject != null))
				{
					return 0;
				}
				return PrefabUtility.GetPrefabType(this.ActiveFsmGameObject);
			}
		}
		public Vector2 ScrollPosition
		{
			get
			{
				return this.scrollPosition;
			}
			set
			{
				this.scrollPosition = value;
			}
		}
		public float Zoom
		{
			get
			{
				return this.zoom;
			}
			set
			{
				this.zoom = value;
			}
		}
		public SkillSelection(Skill fsm)
		{
			this.ActiveFsm = fsm;
			this.Zoom = 1f;
			if (fsm != null)
			{
				this.ActiveState = fsm.GetState(fsm.get_StartState());
			}
		}
		public bool IsFor(Skill testFsm)
		{
			if (testFsm == null)
			{
				return false;
			}
			if (testFsm.get_IsSubFsm())
			{
				return this.subFsm == testFsm;
			}
			return (this.fsmTemplate != null && testFsm.get_UsedInTemplate() == this.fsmTemplate) || (this.FsmComponent != null && testFsm.get_Owner() == this.FsmComponent);
		}
		public List<SkillState> SelectStates(List<SkillState> stateSelection, bool add = false, bool subtract = false)
		{
			if (this.ActiveFsm == null)
			{
				return null;
			}
			if (!add && !subtract)
			{
				this.DeselectStates();
			}
			using (List<SkillState>.Enumerator enumerator = stateSelection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillState current = enumerator.get_Current();
					if (!subtract)
					{
						this.AddState(current);
					}
					else
					{
						this.RemoveState(current);
					}
				}
			}
			this.ActiveTransition = null;
			return this.States;
		}
		public List<SkillState> SelectAllStates()
		{
			if (this.ActiveFsm == null)
			{
				return null;
			}
			this.DeselectStates();
			SkillState[] array = this.ActiveFsm.get_States();
			for (int i = 0; i < array.Length; i++)
			{
				SkillState state = array[i];
				this.AddState(state);
			}
			return this.States;
		}
		public SkillState SelectState(SkillState state, bool add = false, bool subtract = false)
		{
			if (this.ActiveFsm == null)
			{
				if (state != null)
				{
					Debug.LogError("Selecting State with no Fsm selected!");
				}
				return null;
			}
			if (state == null)
			{
				if (!add && !subtract)
				{
					this.DeselectStates();
				}
				return null;
			}
			if (state.get_Fsm() != this.ActiveFsm)
			{
				Debug.LogError("state.Fsm != ActiveFsm");
				state.set_Fsm(this.ActiveFsm);
			}
			if (add)
			{
				this.AddState(state);
			}
			else
			{
				if (subtract)
				{
					this.RemoveState(state);
				}
				else
				{
					if (!this.States.Contains(state))
					{
						this.DeselectStates();
					}
					this.AddState(state);
				}
			}
			this.ActiveTransition = null;
			if (!Application.get_isPlaying() && this.ActiveFsm != null)
			{
				this.ActiveFsm.set_EditState(state);
			}
			SkillEditor.Repaint(false);
			return state;
		}
		public void AddState(SkillState state)
		{
			if (state == null)
			{
				return;
			}
			this.ActiveState = state;
			if (!this.States.Contains(state))
			{
				this.States.Add(state);
				this.StateNames.Add(state.get_Name());
			}
			SkillEditor.StateInspector.Reset();
			SkillEditor.StateInspector.ResetScrollPosition();
			Keyboard.ResetFocus();
		}
		public void RemoveState(SkillState state)
		{
			if (state == null)
			{
				return;
			}
			this.States.Remove(state);
			this.StateNames.Remove(state.get_Name());
			this.ActiveState = null;
			this.ActiveTransition = null;
		}
		public void DeselectStates()
		{
			this.ActiveState = null;
			this.ActiveTransition = null;
			this.States.Clear();
			this.StateNames.Clear();
			this.ActiveFsm.set_EditState(null);
		}
		public SkillTransition SelectTransition(SkillTransition transition)
		{
			if (this.ActiveState == null)
			{
				return null;
			}
			this.ActiveTransition = transition;
			return transition;
		}
		public void SelectActiveFsmGameObject()
		{
			if (this.ActiveFsmGameObject == null || Selection.get_activeGameObject() == this.ActiveFsmGameObject)
			{
				return;
			}
			Selection.set_activeGameObject(this.ActiveFsmGameObject);
		}
		public bool Contains(SkillState testState)
		{
			return this.activeState == testState || this.States.Contains(testState);
		}
		public bool Contains(SkillTransition testTransition)
		{
			return this.ActiveTransition == testTransition;
		}
		public void SanityCheck()
		{
			if (this.fsmComponent != null)
			{
				PlayMakerFSM[] components = this.fsmComponent.get_gameObject().GetComponents<PlayMakerFSM>();
				PlayMakerFSM[] array = components;
				for (int i = 0; i < array.Length; i++)
				{
					PlayMakerFSM playMakerFSM = array[i];
					if (this.fsmComponent == playMakerFSM)
					{
						this.ActiveFsm = playMakerFSM.get_Fsm();
						this.fsmComponent = playMakerFSM;
						break;
					}
				}
			}
			else
			{
				if (this.fsmTemplate != null)
				{
					this.ActiveFsm = this.fsmTemplate.fsm;
				}
			}
			if (this.ActiveFsm != null)
			{
				SkillState[] array2 = this.ActiveFsm.get_States();
				for (int j = 0; j < array2.Length; j++)
				{
					SkillState fsmState = array2[j];
					fsmState.set_Fsm(this.ActiveFsm);
				}
				this.activeState = this.ActiveFsm.GetState(this.stateName);
				this.states = new List<SkillState>();
				using (List<string>.Enumerator enumerator = this.StateNames.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.get_Current();
						this.states.Add(this.ActiveFsm.GetState(current));
					}
					goto IL_137;
				}
			}
			this.activeState = null;
			this.states = new List<SkillState>();
			IL_137:
			this.activeTransition = ((this.activeState != null) ? this.activeState.GetTransition(this.transitionIndex) : null);
			if (!SkillEditor.FsmContainsState(this.ActiveFsm, this.activeState))
			{
				this.activeState = null;
			}
			if (!SkillEditor.StateContainsTransition(this.activeState, this.activeTransition))
			{
				this.activeTransition = null;
			}
		}
		public static Skill FindFsmOnGameObject(GameObject go)
		{
			if (go == null)
			{
				return null;
			}
			PlayMakerFSM component = go.GetComponent<PlayMakerFSM>();
			if (!(component == null))
			{
				return component.get_Fsm();
			}
			return null;
		}
		public static PlayMakerFSM FindFsmComponentOnGameObject(GameObject go)
		{
			if (!(go == null))
			{
				return go.GetComponent<PlayMakerFSM>();
			}
			return null;
		}
		public static Skill FindFsmOnGameObject(GameObject go, string name)
		{
			if (go == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				name = Strings.get_FSM();
			}
			PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
			PlayMakerFSM[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				PlayMakerFSM playMakerFSM = array[i];
				if (playMakerFSM.get_name() == name)
				{
					return playMakerFSM.get_Fsm();
				}
			}
			return null;
		}
		public static bool GameObjectHasFSM(GameObject go)
		{
			if (go == null)
			{
				return false;
			}
			using (List<PlayMakerFSM>.Enumerator enumerator = SkillEditor.FsmComponentList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PlayMakerFSM current = enumerator.get_Current();
					if (current != null && current.get_gameObject() == go)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
