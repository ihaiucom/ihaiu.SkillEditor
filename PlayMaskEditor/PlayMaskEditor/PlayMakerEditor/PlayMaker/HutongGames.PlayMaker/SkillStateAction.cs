using System;
using System.Collections;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	public abstract class SkillStateAction : ISkillStateAction
	{
		private string name;
		private bool enabled = true;
		private bool isOpen = true;
		private bool active;
		private bool finished;
		private bool autoName;
		private GameObject owner;
		[NonSerialized]
		private SkillState fsmState;
		[NonSerialized]
		private Skill fsm;
		[NonSerialized]
		private PlayMakerFSM fsmComponent;
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
		public Skill Fsm
		{
			get
			{
				return this.fsm;
			}
			set
			{
				this.fsm = value;
			}
		}
		public GameObject Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
			}
		}
		public SkillState State
		{
			get
			{
				return this.fsmState;
			}
			set
			{
				this.fsmState = value;
			}
		}
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}
		public bool IsOpen
		{
			get
			{
				return this.isOpen;
			}
			set
			{
				this.isOpen = value;
			}
		}
		public bool IsAutoNamed
		{
			get
			{
				return this.autoName;
			}
			set
			{
				this.autoName = value;
			}
		}
		public bool Entered
		{
			get;
			set;
		}
		public bool Finished
		{
			get
			{
				return this.finished;
			}
			set
			{
				if (value)
				{
					this.active = false;
				}
				this.finished = value;
			}
		}
		public bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				this.active = value;
			}
		}
		public virtual void Init(SkillState state)
		{
			this.fsmState = state;
			this.fsm = state.Fsm;
			this.owner = this.fsm.GameObject;
			this.fsmComponent = this.fsm.FsmComponent;
		}
		public virtual void Reset()
		{
		}
		public virtual void OnPreprocess()
		{
		}
		public virtual void Awake()
		{
		}
		public virtual bool Event(SkillEvent fsmEvent)
		{
			return false;
		}
		public void Finish()
		{
			this.active = false;
			this.finished = true;
			this.State.FinishAction(this);
		}
		public Coroutine StartCoroutine(IEnumerator routine)
		{
			return this.fsmComponent.StartCoroutine("DoCoroutine", routine);
		}
		public void StopCoroutine(Coroutine routine)
		{
			this.fsmComponent.StopCoroutine(routine);
		}
		public virtual void OnEnter()
		{
		}
		public virtual void OnFixedUpdate()
		{
		}
		public virtual void OnUpdate()
		{
		}
		public virtual void OnGUI()
		{
		}
		public virtual void OnLateUpdate()
		{
		}
		public virtual void OnExit()
		{
		}
		public virtual void OnDrawActionGizmos()
		{
		}
		public virtual void OnDrawActionGizmosSelected()
		{
		}
		public virtual string AutoName()
		{
			return null;
		}
		public virtual void OnActionTargetInvoked(object targetObject)
		{
		}
		public virtual void DoCollisionEnter(Collision collisionInfo)
		{
		}
		public virtual void DoCollisionStay(Collision collisionInfo)
		{
		}
		public virtual void DoCollisionExit(Collision collisionInfo)
		{
		}
		public virtual void DoTriggerEnter(Collider other)
		{
		}
		public virtual void DoTriggerStay(Collider other)
		{
		}
		public virtual void DoTriggerExit(Collider other)
		{
		}
		public virtual void DoParticleCollision(GameObject other)
		{
		}
		public virtual void DoCollisionEnter2D(Collision2D collisionInfo)
		{
		}
		public virtual void DoCollisionStay2D(Collision2D collisionInfo)
		{
		}
		public virtual void DoCollisionExit2D(Collision2D collisionInfo)
		{
		}
		public virtual void DoTriggerEnter2D(Collider2D other)
		{
		}
		public virtual void DoTriggerStay2D(Collider2D other)
		{
		}
		public virtual void DoTriggerExit2D(Collider2D other)
		{
		}
		public virtual void DoControllerColliderHit(ControllerColliderHit collider)
		{
		}
		public virtual void DoJointBreak(float force)
		{
		}
		public virtual void DoJointBreak2D(Joint2D joint)
		{
		}
		public virtual void DoAnimatorMove()
		{
		}
		public virtual void DoAnimatorIK(int layerIndex)
		{
		}
		public void Log(string text)
		{
			if (SkillLog.LoggingEnabled)
			{
				this.fsm.MyLog.LogAction(SkillLogType.Info, text, false);
			}
		}
		public void LogWarning(string text)
		{
			if (SkillLog.LoggingEnabled)
			{
				this.fsm.MyLog.LogAction(SkillLogType.Warning, text, false);
			}
		}
		public void LogError(string text)
		{
			if (SkillLog.LoggingEnabled)
			{
				this.fsm.MyLog.LogAction(SkillLogType.Error, text, false);
			}
		}
		public virtual string ErrorCheck()
		{
			return string.Empty;
		}
	}
}
