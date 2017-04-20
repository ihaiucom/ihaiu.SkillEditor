using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class Skill : INameable, IComparable
	{
		[Flags]
		[Serializable]
		private enum EditorFlags
		{
			none = 0,
			nameIsExpanded = 1,
			controlsIsExpanded = 2,
			debugIsExpanded = 4,
			experimentalIsExpanded = 8
		}
		public const int CurrentDataVersion = 2;
		public const int DefaultMaxLoops = 1000;
		private const string StartStateName = "State 1";
		private MethodInfo updateHelperSetDirty;
		public static SkillEventData EventData = new SkillEventData();
		private static Color debugLookAtColor = Color.get_yellow();
		private static Color debugRaycastColor = Color.get_red();
		[SerializeField]
		private int dataVersion;
		[NonSerialized]
		private MonoBehaviour owner;
		[SerializeField]
		private SkillTemplate usedInTemplate;
		[SerializeField]
		private string name = "FSM";
		[SerializeField]
		private string startState;
		[SerializeField]
		private SkillState[] states = new SkillState[1];
		[SerializeField]
		private SkillEvent[] events = new SkillEvent[0];
		[SerializeField]
		private SkillTransition[] globalTransitions = new SkillTransition[0];
		[SerializeField]
		private SkillVariables variables = new SkillVariables();
		[SerializeField]
		private string description = "";
		[SerializeField]
		private string docUrl;
		[SerializeField]
		private bool showStateLabel = true;
		[SerializeField]
		private int maxLoopCount;
		[SerializeField]
		private string watermark = "";
		[SerializeField]
		private string password;
		[SerializeField]
		private bool locked;
		[SerializeField]
		private bool manualUpdate;
		[SerializeField]
		private bool keepDelayedEventsOnStateExit;
		[SerializeField]
		private bool preprocessed;
		[NonSerialized]
		private Skill host;
		[NonSerialized]
		private Skill rootFsm;
		[NonSerialized]
		private List<Skill> subFsmList;
		[NonSerialized]
		public bool setDirty;
		private bool activeStateEntered;
		public List<SkillEvent> ExposedEvents = new List<SkillEvent>();
		private SkillLog myLog;
		public bool RestartOnEnable = true;
		public bool EnableDebugFlow;
		public bool EnableBreakpoints = true;
		[NonSerialized]
		public bool StepFrame;
		private readonly List<DelayedEvent> delayedEvents = new List<DelayedEvent>();
		private readonly List<DelayedEvent> updateEvents = new List<DelayedEvent>();
		private readonly List<DelayedEvent> removeEvents = new List<DelayedEvent>();
		[SerializeField]
		private Skill.EditorFlags editorFlags = Skill.EditorFlags.nameIsExpanded | Skill.EditorFlags.controlsIsExpanded;
		[NonSerialized]
		private bool initialized;
		[SerializeField]
		private string activeStateName;
		[NonSerialized]
		private SkillState activeState;
		[NonSerialized]
		private SkillState switchToState;
		[NonSerialized]
		private SkillState previousActiveState;
		[Obsolete("Use PlayMakerPrefs.Colors instead.")]
		public static readonly Color[] StateColors = new Color[]
		{
			Color.get_grey(),
			new Color(0.545098066f, 0.670588255f, 0.9411765f),
			new Color(0.243137255f, 0.7607843f, 0.6901961f),
			new Color(0.431372553f, 0.7607843f, 0.243137255f),
			new Color(1f, 0.8745098f, 0.1882353f),
			new Color(1f, 0.5529412f, 0.1882353f),
			new Color(0.7607843f, 0.243137255f, 0.2509804f),
			new Color(0.545098066f, 0.243137255f, 0.7607843f)
		};
		[NonSerialized]
		private SkillState editState;
		[SerializeField]
		private bool mouseEvents;
		[SerializeField]
		private bool handleLevelLoaded;
		[SerializeField]
		private bool handleTriggerEnter2D;
		[SerializeField]
		private bool handleTriggerExit2D;
		[SerializeField]
		private bool handleTriggerStay2D;
		[SerializeField]
		private bool handleCollisionEnter2D;
		[SerializeField]
		private bool handleCollisionExit2D;
		[SerializeField]
		private bool handleCollisionStay2D;
		[SerializeField]
		private bool handleTriggerEnter;
		[SerializeField]
		private bool handleTriggerExit;
		[SerializeField]
		private bool handleTriggerStay;
		[SerializeField]
		private bool handleCollisionEnter;
		[SerializeField]
		private bool handleCollisionExit;
		[SerializeField]
		private bool handleCollisionStay;
		[SerializeField]
		private bool handleParticleCollision;
		[SerializeField]
		private bool handleControllerColliderHit;
		[SerializeField]
		private bool handleJointBreak;
		[SerializeField]
		private bool handleJointBreak2D;
		[SerializeField]
		private bool handleOnGUI;
		[SerializeField]
		private bool handleFixedUpdate;
		[SerializeField]
		private bool handleApplicationEvents;
		private static Dictionary<Skill, RaycastHit2D> lastRaycastHit2DInfoLUT;
		[SerializeField]
		private bool handleAnimatorMove;
		[SerializeField]
		private bool handleAnimatorIK;
		private static readonly SkillEventTarget targetSelf = new SkillEventTarget();
		public static List<Skill> FsmList
		{
			get
			{
				List<Skill> list = new List<Skill>();
				using (List<PlayMakerFSM>.Enumerator enumerator = PlayMakerFSM.FsmList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PlayMakerFSM current = enumerator.get_Current();
						if (current != null && current.Fsm != null)
						{
							list.Add(current.Fsm);
						}
					}
				}
				return list;
			}
		}
		public static List<Skill> SortedFsmList
		{
			get
			{
				List<Skill> fsmList = Skill.FsmList;
				fsmList.Sort();
				return fsmList;
			}
		}
		private MethodInfo UpdateHelperSetDirty
		{
			get
			{
				if (object.ReferenceEquals(this.updateHelperSetDirty, null))
				{
					this.updateHelperSetDirty = ReflectionUtils.GetGlobalType("HutongGames.PlayMaker.UpdateHelper").GetMethod("SetDirty");
				}
				return this.updateHelperSetDirty;
			}
		}
		public bool ManualUpdate
		{
			get
			{
				return this.manualUpdate;
			}
			set
			{
				this.manualUpdate = value;
			}
		}
		public bool KeepDelayedEventsOnStateExit
		{
			get
			{
				return this.keepDelayedEventsOnStateExit;
			}
			set
			{
				this.keepDelayedEventsOnStateExit = value;
			}
		}
		public bool Preprocessed
		{
			get
			{
				return this.preprocessed;
			}
			set
			{
				this.preprocessed = value;
			}
		}
		public Skill Host
		{
			get
			{
				return this.host;
			}
			private set
			{
				this.host = value;
			}
		}
		public string Password
		{
			get
			{
				return this.password;
			}
		}
		public bool Locked
		{
			get
			{
				return this.locked;
			}
		}
		public SkillTemplate Template
		{
			get
			{
				if (!(this.Owner != null))
				{
					return null;
				}
				return ((PlayMakerFSM)this.Owner).FsmTemplate;
			}
		}
		public bool IsSubFsm
		{
			get
			{
				return this.host != null;
			}
		}
		public Skill RootFsm
		{
			get
			{
				Skill arg_19_0;
				if ((arg_19_0 = this.rootFsm) == null)
				{
					arg_19_0 = (this.rootFsm = this.GetRootFsm());
				}
				return arg_19_0;
			}
		}
		public List<Skill> SubFsmList
		{
			get
			{
				List<Skill> arg_18_0;
				if ((arg_18_0 = this.subFsmList) == null)
				{
					arg_18_0 = (this.subFsmList = new List<Skill>());
				}
				return arg_18_0;
			}
		}
		public bool Started
		{
			get;
			private set;
		}
		public List<DelayedEvent> DelayedEvents
		{
			get
			{
				return this.delayedEvents;
			}
		}
		public int DataVersion
		{
			get
			{
				return this.dataVersion;
			}
			set
			{
				this.dataVersion = value;
			}
		}
		public MonoBehaviour Owner
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
		public bool NameIsExpanded
		{
			get
			{
				return (this.editorFlags & Skill.EditorFlags.nameIsExpanded) != Skill.EditorFlags.none;
			}
			set
			{
				if (value)
				{
					this.editorFlags |= Skill.EditorFlags.nameIsExpanded;
					return;
				}
				this.editorFlags &= ~Skill.EditorFlags.nameIsExpanded;
			}
		}
		public bool ControlsIsExpanded
		{
			get
			{
				return (this.editorFlags & Skill.EditorFlags.controlsIsExpanded) != Skill.EditorFlags.none;
			}
			set
			{
				if (value)
				{
					this.editorFlags |= Skill.EditorFlags.controlsIsExpanded;
					return;
				}
				this.editorFlags &= ~Skill.EditorFlags.controlsIsExpanded;
			}
		}
		public bool DebugIsExpanded
		{
			get
			{
				return (this.editorFlags & Skill.EditorFlags.debugIsExpanded) != Skill.EditorFlags.none;
			}
			set
			{
				if (value)
				{
					this.editorFlags |= Skill.EditorFlags.debugIsExpanded;
					return;
				}
				this.editorFlags &= ~Skill.EditorFlags.debugIsExpanded;
			}
		}
		public bool ExperimentalIsExpanded
		{
			get
			{
				return (this.editorFlags & Skill.EditorFlags.experimentalIsExpanded) != Skill.EditorFlags.none;
			}
			set
			{
				if (value)
				{
					this.editorFlags |= Skill.EditorFlags.experimentalIsExpanded;
					return;
				}
				this.editorFlags &= ~Skill.EditorFlags.experimentalIsExpanded;
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
		public SkillTemplate UsedInTemplate
		{
			get
			{
				return this.usedInTemplate;
			}
			set
			{
				this.usedInTemplate = value;
			}
		}
		public string StartState
		{
			get
			{
				return this.startState;
			}
			set
			{
				this.startState = value;
			}
		}
		public SkillState[] States
		{
			get
			{
				return this.states;
			}
			set
			{
				this.states = value;
			}
		}
		public SkillEvent[] Events
		{
			get
			{
				return this.events;
			}
			set
			{
				this.events = value;
			}
		}
		public SkillTransition[] GlobalTransitions
		{
			get
			{
				return this.globalTransitions;
			}
			set
			{
				this.globalTransitions = value;
			}
		}
		public SkillVariables Variables
		{
			get
			{
				return this.variables;
			}
			set
			{
				this.variables = value;
			}
		}
		public SkillEventTarget EventTarget
		{
			get;
			set;
		}
		public bool Initialized
		{
			get
			{
				return this.initialized;
			}
		}
		public bool Active
		{
			get
			{
				return this.owner != null && this.owner.get_enabled() && this.owner.get_gameObject() != null && this.owner.get_gameObject().get_activeInHierarchy() && !this.Finished && this.ActiveState != null;
			}
		}
		public bool Finished
		{
			get;
			private set;
		}
		public bool IsSwitchingState
		{
			get
			{
				return this.switchToState != null;
			}
		}
		public SkillState ActiveState
		{
			get
			{
				if (this.activeState == null && this.activeStateName != "")
				{
					this.activeState = this.GetState(this.activeStateName);
				}
				return this.activeState;
			}
			private set
			{
				this.activeState = value;
				this.activeStateName = ((this.activeState == null) ? "" : this.activeState.Name);
			}
		}
		public string ActiveStateName
		{
			get
			{
				return this.activeStateName;
			}
		}
		public SkillState PreviousActiveState
		{
			get
			{
				return this.previousActiveState;
			}
			private set
			{
				this.previousActiveState = value;
			}
		}
		public SkillTransition LastTransition
		{
			get;
			private set;
		}
		public int MaxLoopCount
		{
			get
			{
				if (this.maxLoopCount <= 0)
				{
					return 1000;
				}
				return this.maxLoopCount;
			}
		}
		public int MaxLoopCountOverride
		{
			get
			{
				return this.maxLoopCount;
			}
			set
			{
				this.maxLoopCount = Mathf.Max(0, value);
			}
		}
		public string OwnerName
		{
			get
			{
				if (!(this.owner != null))
				{
					return "";
				}
				return this.owner.get_name();
			}
		}
		public string OwnerDebugName
		{
			get
			{
				if (PlayMakerFSM.NotMainThread)
				{
					return "";
				}
				if (!(this.owner != null))
				{
					return "[missing Owner]";
				}
				return this.owner.get_name();
			}
		}
		public GameObject GameObject
		{
			get
			{
				if (!(this.Owner != null))
				{
					return null;
				}
				return this.Owner.get_gameObject();
			}
		}
		public string GameObjectName
		{
			get
			{
				if (!(this.Owner != null))
				{
					return "[missing GameObject]";
				}
				return this.Owner.get_gameObject().get_name();
			}
		}
		public Object OwnerObject
		{
			get
			{
				if (this.UsedInTemplate)
				{
					return this.UsedInTemplate;
				}
				return this.Owner;
			}
		}
		public PlayMakerFSM FsmComponent
		{
			get
			{
				return this.Owner as PlayMakerFSM;
			}
		}
		public SkillLog MyLog
		{
			get
			{
				SkillLog arg_19_0;
				if ((arg_19_0 = this.myLog) == null)
				{
					arg_19_0 = (this.myLog = SkillLog.GetLog(this));
				}
				return arg_19_0;
			}
		}
		public bool IsModifiedPrefabInstance
		{
			get;
			set;
		}
		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}
		public string Watermark
		{
			get
			{
				return this.watermark;
			}
			set
			{
				this.watermark = value;
			}
		}
		public bool ShowStateLabel
		{
			get
			{
				return this.showStateLabel;
			}
			set
			{
				this.showStateLabel = value;
			}
		}
		public static Color DebugLookAtColor
		{
			get
			{
				return Skill.debugLookAtColor;
			}
			set
			{
				Skill.debugLookAtColor = value;
			}
		}
		public static Color DebugRaycastColor
		{
			get
			{
				return Skill.debugRaycastColor;
			}
			set
			{
				Skill.debugRaycastColor = value;
			}
		}
		private string GuiLabel
		{
			get
			{
				return this.OwnerName + " : " + this.Name;
			}
		}
		public string DocUrl
		{
			get
			{
				return this.docUrl;
			}
			set
			{
				this.docUrl = value;
			}
		}
		public SkillState EditState
		{
			get
			{
				return this.editState;
			}
			set
			{
				this.editState = value;
			}
		}
		public static GameObject LastClickedObject
		{
			get;
			set;
		}
		public static bool BreakpointsEnabled
		{
			get;
			set;
		}
		public static bool HitBreakpoint
		{
			get;
			set;
		}
		public static Skill BreakAtFsm
		{
			get;
			private set;
		}
		public static SkillState BreakAtState
		{
			get;
			private set;
		}
		public static bool IsBreak
		{
			get;
			private set;
		}
		public static bool IsErrorBreak
		{
			get;
			private set;
		}
		public static string LastError
		{
			get;
			private set;
		}
		public static bool StepToStateChange
		{
			get;
			set;
		}
		public static Skill StepFsm
		{
			get;
			set;
		}
		public bool SwitchedState
		{
			get;
			set;
		}
		public bool MouseEvents
		{
			get
			{
				return this.mouseEvents;
			}
			set
			{
				this.preprocessed = false;
				this.mouseEvents = value;
				if (this.host != null)
				{
					this.host.MouseEvents |= value;
				}
			}
		}
		public bool HandleLevelLoaded
		{
			get
			{
				return this.handleLevelLoaded;
			}
			set
			{
				this.handleLevelLoaded = value;
				if (this.host != null)
				{
					this.host.HandleLevelLoaded |= value;
				}
			}
		}
		public bool HandleTriggerEnter2D
		{
			get
			{
				return this.handleTriggerEnter2D;
			}
			set
			{
				this.preprocessed = false;
				this.handleTriggerEnter2D = value;
				if (this.host != null)
				{
					this.host.HandleTriggerEnter2D |= value;
				}
			}
		}
		public bool HandleTriggerExit2D
		{
			get
			{
				return this.handleTriggerExit2D;
			}
			set
			{
				this.preprocessed = false;
				this.handleTriggerExit2D = value;
				if (this.host != null)
				{
					this.host.HandleTriggerExit2D |= value;
				}
			}
		}
		public bool HandleTriggerStay2D
		{
			get
			{
				return this.handleTriggerStay2D;
			}
			set
			{
				this.preprocessed = false;
				this.handleTriggerStay2D = value;
				if (this.host != null)
				{
					this.host.HandleTriggerStay2D |= value;
				}
			}
		}
		public bool HandleCollisionEnter2D
		{
			get
			{
				return this.handleCollisionEnter2D;
			}
			set
			{
				this.preprocessed = false;
				this.handleCollisionEnter2D = value;
				if (this.host != null)
				{
					this.host.HandleCollisionEnter2D |= value;
				}
			}
		}
		public bool HandleCollisionExit2D
		{
			get
			{
				return this.handleCollisionExit2D;
			}
			set
			{
				this.preprocessed = false;
				this.handleCollisionExit2D = value;
				if (this.host != null)
				{
					this.host.HandleCollisionExit2D |= value;
				}
			}
		}
		public bool HandleCollisionStay2D
		{
			get
			{
				return this.handleCollisionStay2D;
			}
			set
			{
				this.preprocessed = false;
				this.handleCollisionStay2D = value;
				if (this.host != null)
				{
					this.host.HandleCollisionStay2D |= value;
				}
			}
		}
		public bool HandleTriggerEnter
		{
			get
			{
				return this.handleTriggerEnter;
			}
			set
			{
				this.preprocessed = false;
				this.handleTriggerEnter = value;
				if (this.host != null)
				{
					this.host.HandleTriggerEnter |= value;
				}
			}
		}
		public bool HandleTriggerExit
		{
			get
			{
				return this.handleTriggerExit;
			}
			set
			{
				this.preprocessed = false;
				this.handleTriggerExit = value;
				if (this.host != null)
				{
					this.host.HandleTriggerExit |= value;
				}
			}
		}
		public bool HandleTriggerStay
		{
			get
			{
				return this.handleTriggerStay;
			}
			set
			{
				this.preprocessed = false;
				this.handleTriggerStay = value;
				if (this.host != null)
				{
					this.host.HandleTriggerStay |= value;
				}
			}
		}
		public bool HandleCollisionEnter
		{
			get
			{
				return this.handleCollisionEnter;
			}
			set
			{
				this.preprocessed = false;
				this.handleCollisionEnter = value;
				if (this.host != null)
				{
					this.host.HandleCollisionEnter |= value;
				}
			}
		}
		public bool HandleCollisionExit
		{
			get
			{
				return this.handleCollisionExit;
			}
			set
			{
				this.preprocessed = false;
				this.handleCollisionExit = value;
				if (this.host != null)
				{
					this.host.HandleCollisionExit |= value;
				}
			}
		}
		public bool HandleCollisionStay
		{
			get
			{
				return this.handleCollisionStay;
			}
			set
			{
				this.preprocessed = false;
				this.handleCollisionStay = value;
				if (this.host != null)
				{
					this.host.HandleCollisionStay |= value;
				}
			}
		}
		public bool HandleParticleCollision
		{
			get
			{
				return this.handleParticleCollision;
			}
			set
			{
				this.preprocessed = false;
				this.handleParticleCollision = value;
				if (this.host != null)
				{
					this.host.HandleParticleCollision |= value;
				}
			}
		}
		public bool HandleControllerColliderHit
		{
			get
			{
				return this.handleControllerColliderHit;
			}
			set
			{
				this.preprocessed = false;
				this.handleControllerColliderHit = value;
				if (this.host != null)
				{
					this.host.handleControllerColliderHit |= value;
				}
			}
		}
		public bool HandleJointBreak
		{
			get
			{
				return this.handleJointBreak;
			}
			set
			{
				this.preprocessed = false;
				this.handleJointBreak = value;
				if (this.host != null)
				{
					this.host.HandleJointBreak |= value;
				}
			}
		}
		public bool HandleJointBreak2D
		{
			get
			{
				return this.handleJointBreak2D;
			}
			set
			{
				this.preprocessed = false;
				this.handleJointBreak2D = value;
				if (this.host != null)
				{
					this.host.HandleJointBreak2D |= value;
				}
			}
		}
		public bool HandleOnGUI
		{
			get
			{
				return this.handleOnGUI;
			}
			set
			{
				this.preprocessed = false;
				this.handleOnGUI = value;
				if (this.host != null)
				{
					this.host.HandleOnGUI |= value;
				}
			}
		}
		public bool HandleFixedUpdate
		{
			get
			{
				return this.handleFixedUpdate;
			}
			set
			{
				this.preprocessed = false;
				this.handleFixedUpdate = value;
				if (this.host != null)
				{
					this.host.HandleFixedUpdate |= value;
				}
			}
		}
		public bool HandleApplicationEvents
		{
			get
			{
				return this.handleApplicationEvents;
			}
			set
			{
				this.preprocessed = false;
				this.handleApplicationEvents = value;
				if (this.host != null)
				{
					this.host.HandleApplicationEvents |= value;
				}
			}
		}
		public Collision CollisionInfo
		{
			get;
			set;
		}
		public Collider TriggerCollider
		{
			get;
			set;
		}
		public Collision2D Collision2DInfo
		{
			get;
			set;
		}
		public Collider2D TriggerCollider2D
		{
			get;
			set;
		}
		public float JointBreakForce
		{
			get;
			private set;
		}
		public Joint2D BrokenJoint2D
		{
			get;
			private set;
		}
		public GameObject ParticleCollisionGO
		{
			get;
			set;
		}
		public GameObject CollisionGO
		{
			get
			{
				if (this.CollisionInfo == null)
				{
					return null;
				}
				return this.CollisionInfo.get_gameObject();
			}
		}
		public GameObject Collision2dGO
		{
			get
			{
				if (this.Collision2DInfo == null)
				{
					return null;
				}
				return this.Collision2DInfo.get_gameObject();
			}
		}
		public GameObject TriggerGO
		{
			get
			{
				if (!(this.TriggerCollider != null))
				{
					return null;
				}
				return this.TriggerCollider.get_gameObject();
			}
		}
		public GameObject Trigger2dGO
		{
			get
			{
				if (!(this.TriggerCollider2D != null))
				{
					return null;
				}
				return this.TriggerCollider2D.get_gameObject();
			}
		}
		public string TriggerName
		{
			get;
			private set;
		}
		public string CollisionName
		{
			get;
			private set;
		}
		public string Trigger2dName
		{
			get;
			private set;
		}
		public string Collision2dName
		{
			get;
			private set;
		}
		public ControllerColliderHit ControllerCollider
		{
			get;
			set;
		}
		public RaycastHit RaycastHitInfo
		{
			get;
			set;
		}
		public bool HandleAnimatorMove
		{
			get
			{
				return this.handleAnimatorMove;
			}
			set
			{
				this.preprocessed = false;
				this.handleAnimatorMove = value;
				if (this.host != null)
				{
					this.host.HandleAnimatorMove |= value;
				}
			}
		}
		public bool HandleAnimatorIK
		{
			get
			{
				return this.handleAnimatorIK;
			}
			set
			{
				this.preprocessed = false;
				this.handleAnimatorIK = value;
				if (this.host != null)
				{
					this.host.HandleAnimatorIK |= value;
				}
			}
		}
		public void Lock(string pass)
		{
			if (!this.Locked)
			{
				this.password = pass;
				this.locked = true;
			}
		}
		public void Unlock(string pass)
		{
			if (string.IsNullOrEmpty(this.password) || pass == this.password)
			{
				this.locked = false;
			}
		}
		public void KillDelayedEvents()
		{
			this.delayedEvents.Clear();
		}
		private void ResetEventHandlerFlags()
		{
			this.handleApplicationEvents = false;
			this.handleCollisionEnter = false;
			this.HandleCollisionExit = false;
			this.handleCollisionStay = false;
			this.handleCollisionEnter2D = false;
			this.HandleCollisionExit2D = false;
			this.handleCollisionStay2D = false;
			this.handleTriggerEnter = false;
			this.handleTriggerExit = false;
			this.handleTriggerStay = false;
			this.handleTriggerEnter2D = false;
			this.handleTriggerExit2D = false;
			this.handleTriggerStay2D = false;
			this.handleControllerColliderHit = false;
			this.handleFixedUpdate = false;
			this.handleOnGUI = false;
			this.handleAnimatorIK = false;
			this.handleAnimatorMove = false;
			this.handleJointBreak = false;
			this.handleJointBreak2D = false;
			this.handleParticleCollision = false;
			this.preprocessed = false;
		}
		public static void RecordLastRaycastHit2DInfo(Skill fsm, RaycastHit2D info)
		{
			if (Skill.lastRaycastHit2DInfoLUT == null)
			{
				Skill.lastRaycastHit2DInfoLUT = new Dictionary<Skill, RaycastHit2D>();
			}
			Skill.lastRaycastHit2DInfoLUT.set_Item(fsm, info);
		}
		public static RaycastHit2D GetLastRaycastHit2DInfo(Skill fsm)
		{
			if (Skill.lastRaycastHit2DInfoLUT == null)
			{
				return default(RaycastHit2D);
			}
			return Skill.lastRaycastHit2DInfoLUT.get_Item(fsm);
		}
		public static Skill NewTempFsm()
		{
			return new Skill
			{
				dataVersion = 2
			};
		}
		public Skill()
		{
		}
		public Skill(Skill source, SkillVariables overrideVariables = null)
		{
			this.dataVersion = source.DataVersion;
			this.owner = source.Owner;
			this.name = source.Name;
			this.description = source.Description;
			this.startState = source.StartState;
			this.docUrl = source.docUrl;
			this.showStateLabel = source.showStateLabel;
			this.maxLoopCount = source.maxLoopCount;
			this.watermark = source.Watermark;
			this.RestartOnEnable = source.RestartOnEnable;
			this.EnableDebugFlow = source.EnableDebugFlow;
			this.EnableBreakpoints = source.EnableBreakpoints;
			this.states = new SkillState[source.States.Length];
			for (int i = 0; i < source.States.Length; i++)
			{
				source.States[i].Fsm = source;
				this.states[i] = new SkillState(source.States[i]);
			}
			this.events = new SkillEvent[source.Events.Length];
			for (int j = 0; j < source.Events.Length; j++)
			{
				this.events[j] = new SkillEvent(source.Events[j]);
			}
			this.ExposedEvents = new List<SkillEvent>();
			using (List<SkillEvent>.Enumerator enumerator = source.ExposedEvents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					this.ExposedEvents.Add(new SkillEvent(current));
				}
			}
			this.globalTransitions = new SkillTransition[source.globalTransitions.Length];
			for (int k = 0; k < this.globalTransitions.Length; k++)
			{
				this.globalTransitions[k] = new SkillTransition(source.globalTransitions[k]);
			}
			this.variables = new SkillVariables(source.Variables);
			if (overrideVariables != null)
			{
				this.variables.OverrideVariableValues(overrideVariables);
			}
		}
		public Skill CreateSubFsm(SkillTemplateControl templateControl)
		{
			Skill fsm = templateControl.InstantiateFsm();
			fsm.Host = this;
			fsm.Init(this.Owner);
			templateControl.ID = this.SubFsmList.get_Count();
			this.SubFsmList.Add(fsm);
			return fsm;
		}
		private Skill GetRootFsm()
		{
			Skill fsm = this;
			while (fsm.Host != null)
			{
				fsm = fsm.Host;
			}
			return fsm;
		}
		public void CheckIfDirty()
		{
			if (this.setDirty && (!object.ReferenceEquals(this.Owner, null) || !object.ReferenceEquals(this.UsedInTemplate, null)))
			{
				Debug.Log("FSM Updated: " + SkillUtility.GetFullFsmLabel(this) + "\nPlease re-save the scene/project.", this.OwnerObject);
				this.UpdateHelperSetDirty.Invoke(null, new object[]
				{
					this
				});
				this.setDirty = false;
			}
		}
		public void Reset(MonoBehaviour component)
		{
			this.dataVersion = 2;
			this.owner = component;
			this.name = "FSM";
			this.description = "";
			this.docUrl = "";
			this.globalTransitions = new SkillTransition[0];
			this.events = new SkillEvent[0];
			this.variables = new SkillVariables();
			this.states = new SkillState[1];
			this.States[0] = new SkillState(this)
			{
				Fsm = this,
				Name = "State 1",
				Position = new Rect(50f, 100f, 100f, 16f)
			};
			this.startState = "State 1";
			this.EnableDebugFlow = false;
			this.EnableBreakpoints = true;
		}
		public void UpdateDataVersion()
		{
			this.dataVersion = 2;
			this.SaveActions();
		}
		public void SaveActions()
		{
			SkillState[] array = this.States;
			for (int i = 0; i < array.Length; i++)
			{
				SkillState fsmState = array[i];
				fsmState.SaveActions();
			}
		}
		public void Clear(MonoBehaviour component)
		{
			this.dataVersion = 2;
			this.owner = component;
			this.description = "";
			this.docUrl = "";
			this.globalTransitions = new SkillTransition[0];
			this.events = new SkillEvent[0];
			this.variables = new SkillVariables();
			this.states = new SkillState[1];
			this.States[0] = new SkillState(this)
			{
				Fsm = this,
				Name = "State 1",
				Position = new Rect(50f, 100f, 100f, 16f)
			};
			this.startState = "State 1";
		}
		private void FixDataVersion()
		{
			this.dataVersion = this.DeduceDataVersion();
			if (!PlayMakerGlobals.IsBuilding)
			{
				this.setDirty = true;
			}
		}
		private int DeduceDataVersion()
		{
			SkillState[] array = this.States;
			for (int i = 0; i < array.Length; i++)
			{
				SkillState fsmState = array[i];
				if (fsmState.ActionData.UsesDataVersion2())
				{
					int result = 2;
					return result;
				}
			}
			SkillState[] array2 = this.States;
			for (int j = 0; j < array2.Length; j++)
			{
				SkillState fsmState2 = array2[j];
				if (fsmState2.ActionData.ActionCount > 0)
				{
					int result = 1;
					return result;
				}
			}
			return 2;
		}
		public void Preprocess(MonoBehaviour component)
		{
			this.ResetEventHandlerFlags();
			this.owner = component;
			this.InitData();
			this.Preprocess();
		}
		private void Preprocess()
		{
			SkillState[] array = this.states;
			for (int i = 0; i < array.Length; i++)
			{
				SkillState fsmState = array[i];
				SkillStateAction[] actions = fsmState.Actions;
				for (int j = 0; j < actions.Length; j++)
				{
					SkillStateAction fsmStateAction = actions[j];
					fsmStateAction.Init(fsmState);
					fsmStateAction.OnPreprocess();
				}
			}
			this.CheckFsmEventsForEventHandlers();
			this.preprocessed = true;
		}
		private void Awake()
		{
			SkillState[] array = this.states;
			for (int i = 0; i < array.Length; i++)
			{
				SkillState fsmState = array[i];
				SkillStateAction[] actions = fsmState.Actions;
				for (int j = 0; j < actions.Length; j++)
				{
					SkillStateAction fsmStateAction = actions[j];
					fsmStateAction.Init(fsmState);
					fsmStateAction.Awake();
				}
			}
			if (!this.preprocessed)
			{
				this.CheckFsmEventsForEventHandlers();
			}
		}
		public void Init(MonoBehaviour component)
		{
			this.owner = component;
			this.InitData();
			if (!this.preprocessed)
			{
				this.Preprocess();
			}
			this.Awake();
		}
		public void Reinitialize()
		{
			this.initialized = false;
			this.InitData();
		}
		public void InitData()
		{
			if (this.dataVersion == 0)
			{
				this.FixDataVersion();
			}
			if (this.Initialized)
			{
				return;
			}
			this.initialized = true;
			for (int i = 0; i < this.events.Length; i++)
			{
				this.events[i] = SkillEvent.GetFsmEvent(this.events[i]);
			}
			for (int j = 0; j < this.ExposedEvents.get_Count(); j++)
			{
				if (this.ExposedEvents.get_Item(j) != null)
				{
					this.ExposedEvents.set_Item(j, SkillEvent.GetFsmEvent(this.ExposedEvents.get_Item(j)));
				}
			}
			SkillState[] array = this.states;
			for (int k = 0; k < array.Length; k++)
			{
				SkillState fsmState = array[k];
				fsmState.Fsm = this;
				fsmState.LoadActions();
				SkillTransition[] transitions = fsmState.Transitions;
				for (int l = 0; l < transitions.Length; l++)
				{
					SkillTransition fsmTransition = transitions[l];
					if (!string.IsNullOrEmpty(fsmTransition.EventName))
					{
						SkillEvent @event = this.GetEvent(fsmTransition.EventName);
						fsmTransition.FsmEvent = @event;
					}
				}
			}
			SkillTransition[] array2 = this.globalTransitions;
			for (int m = 0; m < array2.Length; m++)
			{
				SkillTransition fsmTransition2 = array2[m];
				fsmTransition2.FsmEvent = this.GetEvent(fsmTransition2.EventName);
			}
			this.CheckIfDirty();
		}
		private void CheckFsmEventsForEventHandlers()
		{
			SkillEvent[] array = this.Events;
			for (int i = 0; i < array.Length; i++)
			{
				SkillEvent fsmEvent = array[i];
				if (fsmEvent.IsSystemEvent)
				{
					if (fsmEvent == SkillEvent.TriggerEnter)
					{
						this.RootFsm.HandleTriggerEnter = true;
					}
					if (fsmEvent == SkillEvent.TriggerExit)
					{
						this.RootFsm.HandleTriggerExit = true;
					}
					if (fsmEvent == SkillEvent.TriggerStay)
					{
						this.RootFsm.HandleTriggerStay = true;
					}
					if (fsmEvent == SkillEvent.CollisionEnter)
					{
						this.RootFsm.HandleCollisionEnter = true;
					}
					if (fsmEvent == SkillEvent.CollisionExit)
					{
						this.RootFsm.HandleCollisionExit = true;
					}
					if (fsmEvent == SkillEvent.CollisionStay)
					{
						this.RootFsm.HandleCollisionStay = true;
					}
					if (fsmEvent == SkillEvent.TriggerEnter2D)
					{
						this.RootFsm.HandleTriggerEnter2D = true;
					}
					if (fsmEvent == SkillEvent.TriggerExit2D)
					{
						this.RootFsm.HandleTriggerExit2D = true;
					}
					if (fsmEvent == SkillEvent.TriggerStay2D)
					{
						this.RootFsm.HandleTriggerStay2D = true;
					}
					if (fsmEvent == SkillEvent.CollisionEnter2D)
					{
						this.RootFsm.HandleCollisionEnter2D = true;
					}
					if (fsmEvent == SkillEvent.CollisionExit2D)
					{
						this.RootFsm.HandleCollisionExit2D = true;
					}
					if (fsmEvent == SkillEvent.CollisionStay2D)
					{
						this.RootFsm.HandleCollisionStay2D = true;
					}
					if (fsmEvent == SkillEvent.ParticleCollision)
					{
						this.RootFsm.HandleParticleCollision = true;
					}
					if (fsmEvent == SkillEvent.ControllerColliderHit)
					{
						this.RootFsm.HandleControllerColliderHit = true;
					}
					if (fsmEvent == SkillEvent.JointBreak)
					{
						this.RootFsm.HandleJointBreak = true;
					}
					if (fsmEvent == SkillEvent.JointBreak2D)
					{
						this.RootFsm.HandleJointBreak2D = true;
					}
					if (fsmEvent.IsMouseEvent)
					{
						this.RootFsm.MouseEvents = true;
					}
					if (fsmEvent.IsApplicationEvent)
					{
						this.RootFsm.HandleApplicationEvents = true;
					}
					if (fsmEvent == SkillEvent.LevelLoaded)
					{
						this.RootFsm.HandleLevelLoaded = true;
					}
				}
			}
			using (List<Skill>.Enumerator enumerator = this.SubFsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					current.CheckFsmEventsForEventHandlers();
				}
			}
		}
		public void OnEnable()
		{
			this.Finished = false;
			if (this.HandleLevelLoaded)
			{
				SceneManager.remove_sceneLoaded(new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoaded));
				SceneManager.add_sceneLoaded(new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoaded));
			}
			if (this.ActiveState == null || this.RestartOnEnable)
			{
				this.ActiveState = this.GetState(this.startState);
				this.activeStateEntered = false;
				if (this.Started)
				{
					this.Start();
				}
			}
		}
		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			this.Event(SkillEvent.LevelLoaded);
		}
		public void Start()
		{
			if (SkillLog.LoggingEnabled)
			{
				this.MyLog.LogStart(this.ActiveState);
			}
			this.Started = true;
			this.Finished = false;
			int stackCount = SkillExecutionStack.StackCount;
			SkillExecutionStack.PushFsm(this);
			if (this.ActiveState == null)
			{
				this.ActiveState = this.GetState(this.startState);
				this.activeStateEntered = false;
			}
			if (Skill.BreakpointsEnabled && this.EnableBreakpoints && this.ActiveState.IsBreakpoint)
			{
				this.DoBreakpoint();
			}
			else
			{
				this.switchToState = this.ActiveState;
				this.UpdateStateChanges();
			}
			SkillExecutionStack.PopFsm();
			int stackCount2 = SkillExecutionStack.StackCount;
			if (stackCount2 != stackCount)
			{
				Debug.LogError("Stack error: " + (stackCount2 - stackCount));
			}
		}
		public void Update()
		{
			SkillTime.RealtimeBugFix();
			if (this.owner == null)
			{
				return;
			}
			int stackCount = SkillExecutionStack.StackCount;
			if (Skill.HitBreakpoint)
			{
				return;
			}
			SkillExecutionStack.PushFsm(this);
			if (!this.activeStateEntered)
			{
				this.Continue();
			}
			this.UpdateDelayedEvents();
			if (this.ActiveState != null)
			{
				this.UpdateState(this.ActiveState);
			}
			SkillExecutionStack.PopFsm();
			int stackCount2 = SkillExecutionStack.StackCount;
			if (stackCount2 != stackCount)
			{
				Debug.LogError("Stack error: " + (stackCount2 - stackCount));
			}
		}
		public void UpdateDelayedEvents()
		{
			this.removeEvents.Clear();
			this.updateEvents.Clear();
			this.updateEvents.AddRange(this.delayedEvents);
			for (int i = 0; i < this.updateEvents.get_Count(); i++)
			{
				DelayedEvent delayedEvent = this.updateEvents.get_Item(i);
				delayedEvent.Update();
				if (delayedEvent.Finished)
				{
					this.removeEvents.Add(delayedEvent);
				}
			}
			for (int j = 0; j < this.removeEvents.get_Count(); j++)
			{
				DelayedEvent delayedEvent2 = this.removeEvents.get_Item(j);
				this.delayedEvents.Remove(delayedEvent2);
			}
		}
		public void ClearDelayedEvents()
		{
			this.delayedEvents.Clear();
		}
		public void FixedUpdate()
		{
			SkillExecutionStack.PushFsm(this);
			if (this.ActiveState != null && this.activeStateEntered)
			{
				this.FixedUpdateState(this.ActiveState);
			}
			SkillExecutionStack.PopFsm();
		}
		public void LateUpdate()
		{
			SkillExecutionStack.PushFsm(this);
			if (this.ActiveState != null && this.activeStateEntered)
			{
				this.LateUpdateState(this.ActiveState);
			}
			SkillExecutionStack.PopFsm();
		}
		public void OnDisable()
		{
			this.Stop();
			if (this.HandleLevelLoaded)
			{
				SceneManager.remove_sceneLoaded(new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoaded));
			}
		}
		public void Stop()
		{
			if (this.RestartOnEnable)
			{
				this.StopAndReset();
			}
			this.Finished = true;
			if (SkillLog.LoggingEnabled)
			{
				this.MyLog.LogStop();
			}
		}
		private void StopAndReset()
		{
			SkillExecutionStack.PushFsm(this);
			if (this.ActiveState != null && this.activeStateEntered)
			{
				this.ExitState(this.ActiveState);
			}
			this.ActiveState = null;
			this.LastTransition = null;
			this.SwitchedState = false;
			Skill.HitBreakpoint = false;
			SkillExecutionStack.PopFsm();
		}
		public bool HasEvent(string eventName)
		{
			if (string.IsNullOrEmpty(eventName))
			{
				return false;
			}
			SkillEvent[] array = this.events;
			for (int i = 0; i < array.Length; i++)
			{
				SkillEvent fsmEvent = array[i];
				if (fsmEvent.Name == eventName)
				{
					return true;
				}
			}
			return false;
		}
		public void ProcessEvent(SkillEvent fsmEvent, SkillEventData eventData = null)
		{
			if (!this.Active || SkillEvent.IsNullOrEmpty(fsmEvent))
			{
				return;
			}
			if (!this.Started)
			{
				this.Start();
			}
			if (!this.Active)
			{
				return;
			}
			if (eventData != null)
			{
				Skill.SetEventDataSentByInfo(eventData);
			}
			SkillExecutionStack.PushFsm(this);
			if (this.ActiveState.OnEvent(fsmEvent))
			{
				SkillExecutionStack.PopFsm();
				return;
			}
			SkillTransition[] array = this.globalTransitions;
			for (int i = 0; i < array.Length; i++)
			{
				SkillTransition fsmTransition = array[i];
				if (fsmTransition.FsmEvent == fsmEvent)
				{
					if (SkillLog.LoggingEnabled)
					{
						this.MyLog.LogEvent(fsmEvent, this.activeState);
					}
					if (this.DoTransition(fsmTransition, true))
					{
						SkillExecutionStack.PopFsm();
						return;
					}
				}
			}
			SkillTransition[] transitions = this.ActiveState.Transitions;
			for (int j = 0; j < transitions.Length; j++)
			{
				SkillTransition fsmTransition2 = transitions[j];
				if (fsmTransition2.FsmEvent == fsmEvent)
				{
					if (SkillLog.LoggingEnabled)
					{
						this.MyLog.LogEvent(fsmEvent, this.activeState);
					}
					if (this.DoTransition(fsmTransition2, false))
					{
						SkillExecutionStack.PopFsm();
						return;
					}
				}
			}
			SkillExecutionStack.PopFsm();
		}
		public static void SetEventDataSentByInfo()
		{
			Skill.EventData.SentByFsm = SkillExecutionStack.ExecutingFsm;
			Skill.EventData.SentByState = SkillExecutionStack.ExecutingState;
			Skill.EventData.SentByAction = SkillExecutionStack.ExecutingAction;
		}
		private static void SetEventDataSentByInfo(SkillEventData eventData)
		{
			Skill.EventData.SentByFsm = eventData.SentByFsm;
			Skill.EventData.SentByState = eventData.SentByState;
			Skill.EventData.SentByAction = eventData.SentByAction;
		}
		private static SkillEventData GetEventDataSentByInfo()
		{
			return new SkillEventData
			{
				SentByFsm = SkillExecutionStack.ExecutingFsm,
				SentByState = SkillExecutionStack.ExecutingState,
				SentByAction = SkillExecutionStack.ExecutingAction
			};
		}
		public void Event(SkillEventTarget eventTarget, string fsmEventName)
		{
			if (!string.IsNullOrEmpty(fsmEventName))
			{
				this.Event(eventTarget, SkillEvent.GetFsmEvent(fsmEventName));
			}
		}
		public void Event(SkillEventTarget eventTarget, SkillEvent fsmEvent)
		{
			Skill.SetEventDataSentByInfo();
			if (eventTarget == null)
			{
				eventTarget = Skill.targetSelf;
			}
			if (SkillLog.LoggingEnabled && eventTarget.target != SkillEventTarget.EventTarget.Self)
			{
				this.MyLog.LogSendEvent(this.activeState, fsmEvent, eventTarget);
			}
			switch (eventTarget.target)
			{
			case SkillEventTarget.EventTarget.Self:
				this.ProcessEvent(fsmEvent, null);
				break;
			case SkillEventTarget.EventTarget.GameObject:
			{
				GameObject ownerDefaultTarget = this.GetOwnerDefaultTarget(eventTarget.gameObject);
				this.BroadcastEventToGameObject(ownerDefaultTarget, fsmEvent, Skill.GetEventDataSentByInfo(), eventTarget.sendToChildren.Value, eventTarget.excludeSelf.Value);
				break;
			}
			case SkillEventTarget.EventTarget.GameObjectFSM:
			{
				GameObject ownerDefaultTarget = this.GetOwnerDefaultTarget(eventTarget.gameObject);
				this.SendEventToFsmOnGameObject(ownerDefaultTarget, eventTarget.fsmName.Value, fsmEvent);
				break;
			}
			case SkillEventTarget.EventTarget.FSMComponent:
				if (eventTarget.fsmComponent != null)
				{
					eventTarget.fsmComponent.Fsm.ProcessEvent(fsmEvent, null);
				}
				break;
			case SkillEventTarget.EventTarget.BroadcastAll:
				this.BroadcastEvent(fsmEvent, eventTarget.excludeSelf.Value);
				break;
			case SkillEventTarget.EventTarget.HostFSM:
				if (this.Host != null)
				{
					this.Host.ProcessEvent(fsmEvent, null);
				}
				break;
			case SkillEventTarget.EventTarget.SubFSMs:
			{
				List<Skill> list = new List<Skill>(this.SubFsmList);
				using (List<Skill>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Skill current = enumerator.get_Current();
						current.ProcessEvent(fsmEvent, null);
					}
				}
				break;
			}
			}
			if (SkillExecutionStack.ExecutingFsm != this)
			{
				SkillExecutionStack.PushFsm(this);
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
			}
		}
		public void Event(string fsmEventName)
		{
			if (!string.IsNullOrEmpty(fsmEventName))
			{
				this.Event(SkillEvent.GetFsmEvent(fsmEventName));
			}
		}
		public void Event(SkillEvent fsmEvent)
		{
			if (fsmEvent != null)
			{
				this.Event(this.EventTarget, fsmEvent);
			}
		}
		public DelayedEvent DelayedEvent(SkillEvent fsmEvent, float delay)
		{
			DelayedEvent delayedEvent = new DelayedEvent(this, fsmEvent, delay);
			this.delayedEvents.Add(delayedEvent);
			return delayedEvent;
		}
		public DelayedEvent DelayedEvent(SkillEventTarget eventTarget, SkillEvent fsmEvent, float delay)
		{
			DelayedEvent delayedEvent = new DelayedEvent(this, eventTarget, fsmEvent, delay);
			this.delayedEvents.Add(delayedEvent);
			return delayedEvent;
		}
		public void BroadcastEvent(string fsmEventName, bool excludeSelf = false)
		{
			if (!string.IsNullOrEmpty(fsmEventName))
			{
				this.BroadcastEvent(SkillEvent.GetFsmEvent(fsmEventName), excludeSelf);
			}
		}
		public void BroadcastEvent(SkillEvent fsmEvent, bool excludeSelf = false)
		{
			SkillEventData eventDataSentByInfo = Skill.GetEventDataSentByInfo();
			List<PlayMakerFSM> list = new List<PlayMakerFSM>(PlayMakerFSM.FsmList);
			using (List<PlayMakerFSM>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PlayMakerFSM current = enumerator.get_Current();
					if (!(current == null) && current.Fsm != null && (!excludeSelf || current.Fsm != this))
					{
						current.Fsm.ProcessEvent(fsmEvent, eventDataSentByInfo);
					}
				}
			}
		}
		public void BroadcastEventToGameObject(GameObject go, string fsmEventName, bool sendToChildren, bool excludeSelf = false)
		{
			if (!string.IsNullOrEmpty(fsmEventName))
			{
				this.BroadcastEventToGameObject(go, SkillEvent.GetFsmEvent(fsmEventName), Skill.GetEventDataSentByInfo(), sendToChildren, excludeSelf);
			}
		}
		public void BroadcastEventToGameObject(GameObject go, SkillEvent fsmEvent, SkillEventData eventData, bool sendToChildren, bool excludeSelf = false)
		{
			if (go == null)
			{
				return;
			}
			List<Skill> list = new List<Skill>();
			using (List<PlayMakerFSM>.Enumerator enumerator = PlayMakerFSM.FsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PlayMakerFSM current = enumerator.get_Current();
					if (current != null && current.get_gameObject() == go)
					{
						list.Add(current.Fsm);
					}
				}
			}
			using (List<Skill>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Skill current2 = enumerator2.get_Current();
					if (!excludeSelf || current2 != this)
					{
						current2.ProcessEvent(fsmEvent, eventData);
					}
				}
			}
			if (sendToChildren)
			{
				for (int i = 0; i < go.get_transform().get_childCount(); i++)
				{
					this.BroadcastEventToGameObject(go.get_transform().GetChild(i).get_gameObject(), fsmEvent, eventData, true, excludeSelf);
				}
			}
		}
		public void SendEventToFsmOnGameObject(GameObject gameObject, string fsmName, string fsmEventName)
		{
			if (!string.IsNullOrEmpty(fsmEventName))
			{
				this.SendEventToFsmOnGameObject(gameObject, fsmName, SkillEvent.GetFsmEvent(fsmEventName));
			}
		}
		public void SendEventToFsmOnGameObject(GameObject gameObject, string fsmName, SkillEvent fsmEvent)
		{
			if (gameObject == null)
			{
				return;
			}
			Skill.SetEventDataSentByInfo();
			List<PlayMakerFSM> list = new List<PlayMakerFSM>(PlayMakerFSM.FsmList);
			if (string.IsNullOrEmpty(fsmName))
			{
				using (List<PlayMakerFSM>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PlayMakerFSM current = enumerator.get_Current();
						if (current != null && current.get_gameObject() == gameObject)
						{
							current.Fsm.ProcessEvent(fsmEvent, null);
						}
					}
					return;
				}
			}
			using (List<PlayMakerFSM>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					PlayMakerFSM current2 = enumerator2.get_Current();
					if (current2 != null && current2.get_gameObject() == gameObject && fsmName == current2.Fsm.Name)
					{
						current2.Fsm.ProcessEvent(fsmEvent, null);
						break;
					}
				}
			}
		}
		public void SetState(string stateName)
		{
			this.SwitchState(this.GetState(stateName));
		}
		public void UpdateStateChanges()
		{
			while (this.IsSwitchingState && !Skill.HitBreakpoint)
			{
				this.SwitchState(this.switchToState);
			}
			SkillState[] array = this.States;
			for (int i = 0; i < array.Length; i++)
			{
				SkillState fsmState = array[i];
				fsmState.ResetLoopCount();
			}
		}
		private bool DoTransition(SkillTransition transition, bool isGlobal)
		{
			SkillState state = this.GetState(transition.ToState);
			if (state == null)
			{
				return false;
			}
			this.LastTransition = transition;
			if (PlayMakerGlobals.IsEditor)
			{
				this.MyLog.LogTransition(isGlobal ? null : this.ActiveState, transition);
			}
			this.switchToState = state;
			if (Skill.EventData.SentByFsm != this)
			{
				this.UpdateStateChanges();
			}
			return true;
		}
		public void SwitchState(SkillState toState)
		{
			if (toState == null)
			{
				return;
			}
			if (this.ActiveState != null && this.activeStateEntered)
			{
				this.ExitState(this.ActiveState);
			}
			this.ActiveState = toState;
			if ((Skill.BreakpointsEnabled && this.EnableBreakpoints && toState.IsBreakpoint) || (Skill.StepToStateChange && (Skill.StepFsm == null || Skill.StepFsm == this)))
			{
				this.DoBreakpoint();
				return;
			}
			this.EnterState(toState);
		}
		public void GotoPreviousState()
		{
			if (this.PreviousActiveState == null)
			{
				return;
			}
			this.SwitchState(this.PreviousActiveState);
		}
		private void EnterState(SkillState state)
		{
			this.EventTarget = null;
			this.SwitchedState = true;
			this.activeStateEntered = true;
			this.switchToState = null;
			if (SkillLog.LoggingEnabled)
			{
				this.MyLog.LogEnterState(state);
			}
			if (state.loopCount >= this.MaxLoopCount)
			{
				this.Owner.set_enabled(false);
				this.MyLog.LogError("Loop count exceeded maximum: " + this.MaxLoopCount + " Default is 1000. Override in Fsm Inspector.");
				return;
			}
			state.Fsm = this;
			state.OnEnter();
		}
		private void FixedUpdateState(SkillState state)
		{
			state.Fsm = this;
			state.OnFixedUpdate();
			this.UpdateStateChanges();
		}
		private void UpdateState(SkillState state)
		{
			state.Fsm = this;
			state.OnUpdate();
			this.UpdateStateChanges();
		}
		private void LateUpdateState(SkillState state)
		{
			state.Fsm = this;
			state.OnLateUpdate();
			this.UpdateStateChanges();
		}
		private void ExitState(SkillState state)
		{
			this.PreviousActiveState = state;
			state.Fsm = this;
			if (SkillLog.LoggingEnabled)
			{
				this.MyLog.LogExitState(state);
			}
			this.ActiveState = null;
			state.OnExit();
			if (!this.keepDelayedEventsOnStateExit)
			{
				this.KillDelayedEvents();
			}
		}
		public Skill GetSubFsm(string subFsmName)
		{
			for (int i = 0; i < this.SubFsmList.get_Count(); i++)
			{
				Skill fsm = this.SubFsmList.get_Item(i);
				if (fsm != null && fsm.name == subFsmName)
				{
					return fsm;
				}
			}
			return null;
		}
		public static string GetFullFsmLabel(Skill fsm)
		{
			if (fsm == null)
			{
				return "None (FSM)";
			}
			return fsm.OwnerName + " : " + fsm.Name;
		}
		public GameObject GetOwnerDefaultTarget(SkillOwnerDefault ownerDefault)
		{
			if (ownerDefault == null)
			{
				return null;
			}
			if (ownerDefault.OwnerOption != OwnerDefaultOption.UseOwner)
			{
				return ownerDefault.GameObject.Value;
			}
			return this.GameObject;
		}
		public SkillState GetState(string stateName)
		{
			SkillState[] array = this.states;
			for (int i = 0; i < array.Length; i++)
			{
				SkillState fsmState = array[i];
				if (fsmState.Name == stateName)
				{
					return fsmState;
				}
			}
			return null;
		}
		public SkillEvent GetEvent(string eventName)
		{
			if (string.IsNullOrEmpty(eventName))
			{
				return null;
			}
			SkillEvent fsmEvent = SkillEvent.GetFsmEvent(eventName);
			List<SkillEvent> list = new List<SkillEvent>(this.events);
			if (!SkillEvent.EventListContainsEvent(list, eventName))
			{
				list.Add(fsmEvent);
			}
			this.events = list.ToArray();
			return fsmEvent;
		}
		public int CompareTo(object obj)
		{
			Skill fsm = obj as Skill;
			if (fsm != null)
			{
				return this.GuiLabel.CompareTo(fsm.GuiLabel);
			}
			return 0;
		}
		public SkillObject GetFsmObject(string varName)
		{
			return this.variables.GetFsmObject(varName);
		}
		public SkillMaterial GetFsmMaterial(string varName)
		{
			return this.variables.GetFsmMaterial(varName);
		}
		public SkillTexture GetFsmTexture(string varName)
		{
			return this.variables.GetFsmTexture(varName);
		}
		public SkillFloat GetFsmFloat(string varName)
		{
			return this.variables.GetFsmFloat(varName);
		}
		public SkillInt GetFsmInt(string varName)
		{
			return this.variables.GetFsmInt(varName);
		}
		public SkillBool GetFsmBool(string varName)
		{
			return this.variables.GetFsmBool(varName);
		}
		public SkillString GetFsmString(string varName)
		{
			return this.variables.GetFsmString(varName);
		}
		public SkillVector2 GetFsmVector2(string varName)
		{
			return this.variables.GetFsmVector2(varName);
		}
		public SkillVector3 GetFsmVector3(string varName)
		{
			return this.variables.GetFsmVector3(varName);
		}
		public SkillRect GetFsmRect(string varName)
		{
			return this.variables.GetFsmRect(varName);
		}
		public SkillQuaternion GetFsmQuaternion(string varName)
		{
			return this.variables.GetFsmQuaternion(varName);
		}
		public SkillColor GetFsmColor(string varName)
		{
			return this.variables.GetFsmColor(varName);
		}
		public SkillGameObject GetFsmGameObject(string varName)
		{
			return this.variables.GetFsmGameObject(varName);
		}
		public SkillArray GetFsmArray(string varName)
		{
			return this.variables.GetFsmArray(varName);
		}
		public SkillEnum GetFsmEnum(string varName)
		{
			return this.variables.GetFsmEnum(varName);
		}
		public void OnDrawGizmos()
		{
			if (this.owner == null)
			{
				return;
			}
			if (PlayMakerFSM.DrawGizmos)
			{
				Gizmos.DrawIcon(this.owner.get_transform().get_position(), "PlaymakerIcon.tiff");
			}
			if (this.EditState != null)
			{
				this.EditState.Fsm = this;
				if (this.EditState.ActionData != null)
				{
					SkillStateAction[] actions = this.EditState.Actions;
					for (int i = 0; i < actions.Length; i++)
					{
						SkillStateAction fsmStateAction = actions[i];
						fsmStateAction.OnDrawActionGizmos();
					}
				}
			}
		}
		public void OnDrawGizmosSelected()
		{
			if (this.EditState != null)
			{
				this.EditState.Fsm = this;
				if (this.EditState.ActionData != null)
				{
					SkillStateAction[] actions = this.EditState.Actions;
					for (int i = 0; i < actions.Length; i++)
					{
						SkillStateAction fsmStateAction = actions[i];
						fsmStateAction.OnDrawActionGizmosSelected();
					}
				}
			}
		}
		public void OnCollisionEnter(Collision collisionInfo)
		{
			SkillExecutionStack.PushFsm(this);
			this.CollisionInfo = collisionInfo;
			this.CollisionName = collisionInfo.get_gameObject().get_name();
			if (this.ActiveState.OnCollisionEnter(collisionInfo))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.CollisionEnter);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnCollisionStay(Collision collisionInfo)
		{
			SkillExecutionStack.PushFsm(this);
			this.CollisionInfo = collisionInfo;
			this.CollisionName = collisionInfo.get_gameObject().get_name();
			if (this.ActiveState.OnCollisionStay(collisionInfo))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.CollisionStay);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnCollisionExit(Collision collisionInfo)
		{
			SkillExecutionStack.PushFsm(this);
			this.CollisionInfo = collisionInfo;
			this.CollisionName = collisionInfo.get_gameObject().get_name();
			if (this.ActiveState.OnCollisionExit(collisionInfo))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.CollisionExit);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnTriggerEnter(Collider other)
		{
			SkillExecutionStack.PushFsm(this);
			this.TriggerCollider = other;
			this.TriggerName = other.get_gameObject().get_name();
			if (this.ActiveState.OnTriggerEnter(other))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.TriggerEnter);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnTriggerStay(Collider other)
		{
			SkillExecutionStack.PushFsm(this);
			this.TriggerCollider = other;
			this.TriggerName = other.get_gameObject().get_name();
			if (this.ActiveState.OnTriggerStay(other))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.TriggerStay);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnTriggerExit(Collider other)
		{
			SkillExecutionStack.PushFsm(this);
			this.TriggerCollider = other;
			this.TriggerName = other.get_gameObject().get_name();
			if (this.ActiveState.OnTriggerExit(other))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.TriggerExit);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnParticleCollision(GameObject other)
		{
			SkillExecutionStack.PushFsm(this);
			this.ParticleCollisionGO = other;
			if (this.ActiveState.OnParticleCollision(other))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.ParticleCollision);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnCollisionEnter2D(Collision2D collisionInfo)
		{
			SkillExecutionStack.PushFsm(this);
			this.Collision2DInfo = collisionInfo;
			this.Collision2dName = collisionInfo.get_gameObject().get_name();
			if (this.ActiveState.OnCollisionEnter2D(collisionInfo))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.CollisionEnter2D);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnCollisionStay2D(Collision2D collisionInfo)
		{
			SkillExecutionStack.PushFsm(this);
			this.Collision2DInfo = collisionInfo;
			this.Collision2dName = collisionInfo.get_gameObject().get_name();
			if (this.ActiveState.OnCollisionStay2D(collisionInfo))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.CollisionStay2D);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnCollisionExit2D(Collision2D collisionInfo)
		{
			SkillExecutionStack.PushFsm(this);
			this.Collision2DInfo = collisionInfo;
			this.Collision2dName = collisionInfo.get_gameObject().get_name();
			if (this.ActiveState.OnCollisionExit2D(collisionInfo))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.CollisionExit2D);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnTriggerEnter2D(Collider2D other)
		{
			SkillExecutionStack.PushFsm(this);
			this.TriggerCollider2D = other;
			this.Trigger2dName = other.get_name();
			if (this.ActiveState.OnTriggerEnter2D(other))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.TriggerEnter2D);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnTriggerStay2D(Collider2D other)
		{
			SkillExecutionStack.PushFsm(this);
			this.TriggerCollider2D = other;
			this.Trigger2dName = other.get_name();
			if (this.ActiveState.OnTriggerStay2D(other))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.TriggerStay2D);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnTriggerExit2D(Collider2D other)
		{
			SkillExecutionStack.PushFsm(this);
			this.TriggerCollider2D = other;
			this.Trigger2dName = other.get_name();
			if (this.ActiveState.OnTriggerExit2D(other))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.TriggerExit2D);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnControllerColliderHit(ControllerColliderHit collider)
		{
			SkillExecutionStack.PushFsm(this);
			this.ControllerCollider = collider;
			if (this.ActiveState.OnControllerColliderHit(collider))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.ControllerColliderHit);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnJointBreak(float breakForce)
		{
			SkillExecutionStack.PushFsm(this);
			this.JointBreakForce = breakForce;
			if (this.ActiveState.OnJointBreak(breakForce))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.JointBreak);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnJointBreak2D(Joint2D brokenJoint)
		{
			SkillExecutionStack.PushFsm(this);
			this.BrokenJoint2D = brokenJoint;
			if (this.ActiveState.OnJointBreak2D(brokenJoint))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			this.Event(SkillEvent.JointBreak2D);
			this.UpdateStateChanges();
			SkillExecutionStack.PopFsm();
		}
		public void OnAnimatorMove()
		{
			SkillExecutionStack.PushFsm(this);
			if (this.ActiveState.OnAnimatorMove())
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			SkillExecutionStack.PopFsm();
		}
		public void OnAnimatorIK(int layerIndex)
		{
			SkillExecutionStack.PushFsm(this);
			if (this.ActiveState.OnAnimatorIK(layerIndex))
			{
				this.UpdateStateChanges();
				SkillExecutionStack.PopFsm();
				return;
			}
			SkillExecutionStack.PopFsm();
		}
		public void OnGUI()
		{
			if (this.ActiveState != null)
			{
				this.ActiveState.OnGUI();
			}
		}
		private void DoBreakpoint()
		{
			this.activeStateEntered = false;
			this.DoBreak();
		}
		public void DoBreakError(string error)
		{
			Skill.IsErrorBreak = true;
			Skill.LastError = error;
			this.DoBreak();
		}
		private void DoBreak()
		{
			Skill.BreakAtFsm = SkillExecutionStack.ExecutingFsm;
			Skill.BreakAtState = SkillExecutionStack.ExecutingState;
			Skill.HitBreakpoint = true;
			Skill.IsBreak = true;
			if (SkillLog.LoggingEnabled)
			{
				this.MyLog.LogBreak();
			}
			Skill.StepToStateChange = false;
		}
		private void Continue()
		{
			this.activeStateEntered = true;
			Skill.HitBreakpoint = false;
			Skill.IsErrorBreak = false;
			Skill.IsBreak = false;
			this.EnterState(this.ActiveState);
		}
		public void OnDestroy()
		{
			if (this.subFsmList != null)
			{
				using (List<Skill>.Enumerator enumerator = this.subFsmList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Skill current = enumerator.get_Current();
						current.OnDestroy();
					}
				}
				this.subFsmList.Clear();
			}
			if (Skill.EventData.SentByFsm == this)
			{
				Skill.EventData = new SkillEventData();
			}
			if (PlayMakerGUI.SelectedFSM == this)
			{
				PlayMakerGUI.SelectedFSM = null;
			}
			if (this.myLog != null)
			{
				this.myLog.OnDestroy();
			}
			if (Skill.lastRaycastHit2DInfoLUT != null)
			{
				Skill.lastRaycastHit2DInfoLUT.Remove(this);
			}
			this.owner = null;
		}
	}
}
