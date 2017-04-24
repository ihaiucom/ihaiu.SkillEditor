using System;
using System.Collections.Generic;
using UnityEngine;
namespace ihaiu
{
    [Serializable]
    public class SkillState : INameable
    {
        private bool active;
        private bool finished;
        private SkillStateAction activeAction;
        private int activeActionIndex;
        [NonSerialized]
        private Skill fsm;
        [SerializeField]
        private string name;
        [SerializeField]
        private string description;
        [SerializeField]
        private byte colorIndex;
        [SerializeField]
        private Rect position;
        [SerializeField]
        private bool isBreakpoint;
        [SerializeField]
        private bool isSequence;
        [SerializeField]
        private bool hideUnused;
        [SerializeField]
        private SkillTransition[] transitions = new SkillTransition[0];
        [NonSerialized]
        private SkillStateAction[] actions;
        [SerializeField]
        private ActionData actionData = new ActionData();
        [NonSerialized]
        private List<SkillStateAction> activeActions;
        [NonSerialized]
        private List<SkillStateAction> _finishedActions;
        public float StateTime
        {
            get;
            private set;
        }
        public float RealStartTime
        {
            get;
            private set;
        }
        public int loopCount
        {
            get;
            private set;
        }
        public int maxLoopCount
        {
            get;
            private set;
        }
        public List<SkillStateAction> ActiveActions
        {
            get
            {
                List<SkillStateAction> arg_18_0;
                if ((arg_18_0 = this.activeActions) == null)
                {
                    arg_18_0 = (this.activeActions = new List<SkillStateAction>());
                }
                return arg_18_0;
            }
        }
        private List<SkillStateAction> finishedActions
        {
            get
            {
                List<SkillStateAction> arg_18_0;
                if ((arg_18_0 = this._finishedActions) == null)
                {
                    arg_18_0 = (this._finishedActions = new List<SkillStateAction>());
                }
                return arg_18_0;
            }
        }
        public bool Active
        {
            get
            {
                return this.active;
            }
        }
        public SkillStateAction ActiveAction
        {
            get
            {
                return this.activeAction;
            }
        }
        public bool IsInitialized
        {
            get
            {
                return this.fsm != null;
            }
        }
        public Skill Fsm
        {
            get
            {
                if (this.fsm == null)
                {
                    Debug.LogError("get_fsm: Fsm not initialized: " + this.name);
                }
                return this.fsm;
            }
            set
            {
                if (value == null)
                {
                    Debug.LogWarning("set_fsm: value == null: " + this.name);
                }
                this.fsm = value;
            }
        }
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
        public bool IsSequence
        {
            get
            {
                return this.isSequence;
            }
            set
            {
                this.isSequence = value;
            }
        }
        public int ActiveActionIndex
        {
            get
            {
                return this.activeActionIndex;
            }
        }
        public Rect Position
        {
            get
            {
                return this.position;
            }
            set
            {
                if (!float.IsNaN(value.get_x()) && !float.IsNaN(value.get_y()))
                {
                    this.position = value;
                }
            }
        }
        public bool IsBreakpoint
        {
            get
            {
                return this.isBreakpoint;
            }
            set
            {
                this.isBreakpoint = value;
            }
        }
        public bool HideUnused
        {
            get
            {
                return this.hideUnused;
            }
            set
            {
                this.hideUnused = value;
            }
        }
        public SkillStateAction[] Actions
        {
            get
            {
                if (this.fsm == null)
                {
                    Debug.LogError("get_actions: Fsm not initialized: " + this.name);
                }
                SkillStateAction[] arg_3C_0;
                if ((arg_3C_0 = this.actions) == null)
                {
                    arg_3C_0 = (this.actions = this.actionData.LoadActions(this));
                }
                return arg_3C_0;
            }
            set
            {
                this.actions = value;
            }
        }
        public bool ActionsLoaded
        {
            get
            {
                return this.actions != null;
            }
        }
        public ActionData ActionData
        {
            get
            {
                return this.actionData;
            }
        }
        public SkillTransition[] Transitions
        {
            get
            {
                return this.transitions;
            }
            set
            {
                this.transitions = value;
            }
        }
        public string Description
        {
            get
            {
                string arg_18_0;
                if ((arg_18_0 = this.description) == null)
                {
                    arg_18_0 = (this.description = "");
                }
                return arg_18_0;
            }
            set
            {
                this.description = value;
            }
        }
        public int ColorIndex
        {
            get
            {
                return (int)this.colorIndex;
            }
            set
            {
                this.colorIndex = (byte)value;
            }
        }
        public static string GetFullStateLabel(SkillState state)
        {
            if (state == null)
            {
                return "None (State)";
            }
            return Skill.GetFullFsmLabel(state.Fsm) + " : " + state.Name;
        }
        public SkillState(Skill fsm)
        {
            this.fsm = fsm;
        }
        public SkillState(SkillState source)
        {
            this.fsm = source.Fsm;
            this.name = source.Name;
            this.description = source.description;
            this.colorIndex = source.colorIndex;
            this.position = new Rect(source.position);
            this.hideUnused = source.hideUnused;
            this.isBreakpoint = source.isBreakpoint;
            this.isSequence = source.isSequence;
            this.transitions = new SkillTransition[source.transitions.Length];
            for (int i = 0; i < source.Transitions.Length; i++)
            {
                this.transitions[i] = new SkillTransition(source.Transitions[i]);
            }
            this.actionData = source.actionData.Copy();
        }
        public void CopyActionData(SkillState state)
        {
            this.actionData = state.actionData.Copy();
        }
        public void LoadActions()
        {
            this.actions = this.actionData.LoadActions(this);
        }
        public void SaveActions()
        {
            if (this.actions != null)
            {
                this.actionData.SaveActions(this, this.actions);
            }
        }
        public void OnEnter()
        {
            this.loopCount++;
            if (this.loopCount > this.maxLoopCount)
            {
                this.maxLoopCount = this.loopCount;
            }
            this.active = true;
            this.finished = false;
            this.finishedActions.Clear();
            this.RealStartTime = SkillTime.RealtimeSinceStartup;
            this.StateTime = 0f;
            this.ActiveActions.Clear();
            if (this.ActivateActions(0))
            {
                this.CheckAllActionsFinished();
            }
        }
        private bool ActivateActions(int startIndex)
        {
            for (int i = startIndex; i < this.Actions.Length; i++)
            {
                this.activeActionIndex = i;
                SkillStateAction fsmStateAction = this.Actions[i];
                if (!fsmStateAction.Enabled)
                {
                    fsmStateAction.Finished = true;
                }
                else
                {
                    this.ActiveActions.Add(fsmStateAction);
                    this.activeAction = fsmStateAction;
                    fsmStateAction.Active = true;
                    fsmStateAction.Finished = false;
                    fsmStateAction.Init(this);
                    fsmStateAction.Entered = true;
                    fsmStateAction.OnEnter();
                    if (this.Fsm.IsSwitchingState)
                    {
                        return false;
                    }
                    if (!fsmStateAction.Finished && this.isSequence)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool OnEvent(SkillEvent fsmEvent)
        {
            bool flag = false;
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                flag = fsmStateAction.Event(fsmEvent);
            }
            return this.fsm.IsSwitchingState || flag;
        }
        public void OnFixedUpdate()
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.OnFixedUpdate();
            }
            this.CheckAllActionsFinished();
        }
        public void OnUpdate()
        {
            if (this.finished)
            {
                return;
            }
            this.StateTime += Time.get_deltaTime();
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.OnUpdate();
            }
            this.CheckAllActionsFinished();
        }
        public void OnLateUpdate()
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.OnLateUpdate();
            }
            this.CheckAllActionsFinished();
        }
        public bool OnAnimatorMove()
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoAnimatorMove();
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnAnimatorIK(int layerIndex)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoAnimatorIK(layerIndex);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnCollisionEnter(Collision collisionInfo)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoCollisionEnter(collisionInfo);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnCollisionStay(Collision collisionInfo)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoCollisionStay(collisionInfo);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnCollisionExit(Collision collisionInfo)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoCollisionExit(collisionInfo);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnTriggerEnter(Collider other)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoTriggerEnter(other);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnTriggerStay(Collider other)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoTriggerStay(other);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnTriggerExit(Collider other)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoTriggerExit(other);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnParticleCollision(GameObject other)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoParticleCollision(other);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnCollisionEnter2D(Collision2D collisionInfo)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoCollisionEnter2D(collisionInfo);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnCollisionStay2D(Collision2D collisionInfo)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoCollisionStay2D(collisionInfo);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnCollisionExit2D(Collision2D collisionInfo)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoCollisionExit2D(collisionInfo);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnTriggerEnter2D(Collider2D other)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoTriggerEnter2D(other);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnTriggerStay2D(Collider2D other)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoTriggerStay2D(other);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnTriggerExit2D(Collider2D other)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoTriggerExit2D(other);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnControllerColliderHit(ControllerColliderHit collider)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoControllerColliderHit(collider);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnJointBreak(float force)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoJointBreak(force);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public bool OnJointBreak2D(Joint2D joint)
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.DoJointBreak2D(joint);
            }
            this.RemoveFinishedActions();
            return this.fsm.IsSwitchingState;
        }
        public void OnGUI()
        {
            for (int i = 0; i < this.ActiveActions.get_Count(); i++)
            {
                SkillStateAction fsmStateAction = this.ActiveActions.get_Item(i);
                fsmStateAction.Init(this);
                fsmStateAction.OnGUI();
            }
            this.RemoveFinishedActions();
        }
        public void FinishAction(SkillStateAction action)
        {
            this.finishedActions.Add(action);
        }
        private void RemoveFinishedActions()
        {
            for (int i = 0; i < this.finishedActions.get_Count(); i++)
            {
                this.ActiveActions.Remove(this.finishedActions.get_Item(i));
            }
            this.finishedActions.Clear();
        }
        private void CheckAllActionsFinished()
        {
            if (this.finished || !this.active || this.fsm.IsSwitchingState)
            {
                return;
            }
            this.RemoveFinishedActions();
            if (this.ActiveActions.get_Count() == 0)
            {
                if (this.isSequence && ++this.activeActionIndex < this.actions.Length && !this.ActivateActions(this.activeActionIndex))
                {
                    return;
                }
                this.finished = true;
                this.fsm.Event(SkillEvent.Finished);
            }
        }
        public void OnExit()
        {
            this.active = false;
            this.finished = false;
            SkillStateAction[] array = this.Actions;
            for (int i = 0; i < array.Length; i++)
            {
                SkillStateAction fsmStateAction = array[i];
                if (fsmStateAction.Entered)
                {
                    this.activeAction = fsmStateAction;
                    fsmStateAction.Init(this);
                    fsmStateAction.OnExit();
                }
            }
        }
        public void ResetLoopCount()
        {
            this.loopCount = 0;
        }
        public SkillTransition GetTransition(int transitionIndex)
        {
            if (transitionIndex < 0 || transitionIndex > this.transitions.Length - 1)
            {
                return null;
            }
            return this.transitions[transitionIndex];
        }
        public int GetTransitionIndex(SkillTransition transition)
        {
            if (transition == null)
            {
                return -1;
            }
            for (int i = 0; i < this.transitions.Length; i++)
            {
                SkillTransition fsmTransition = this.transitions[i];
                if (fsmTransition == transition)
                {
                    return i;
                }
            }
            return -1;
        }
        public static int GetStateIndex(SkillState state)
        {
            if (state.Fsm == null)
            {
                return -1;
            }
            for (int i = 0; i < state.Fsm.States.Length; i++)
            {
                SkillState fsmState = state.Fsm.States[i];
                if (fsmState == state)
                {
                    return i;
                }
            }
            Debug.LogError("State not in FSM!");
            return -1;
        }
    }
}
