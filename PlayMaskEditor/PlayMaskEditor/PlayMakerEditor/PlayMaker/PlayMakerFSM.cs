using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("PlayMaker/PlayMakerFSM")]
public class PlayMakerFSM : MonoBehaviour, ISerializationCallbackReceiver
{
	private static readonly List<PlayMakerFSM> fsmList = new List<PlayMakerFSM>();
	public static bool MaximizeFileCompatibility;
	public static bool ApplicationIsQuitting;
	public static bool NotMainThread;
	[HideInInspector, SerializeField]
	private Skill fsm;
	[SerializeField]
	private SkillTemplate fsmTemplate;
	[SerializeField]
	private bool eventHandlerComponentsAdded;
	public static string VersionNotes
	{
		get
		{
			return "";
		}
	}
	public static string VersionLabel
	{
		get
		{
			return "";
		}
	}
	public static List<PlayMakerFSM> FsmList
	{
		get
		{
			return PlayMakerFSM.fsmList;
		}
	}
	public SkillTemplate FsmTemplate
	{
		get
		{
			return this.fsmTemplate;
		}
	}
	public GUITexture GuiTexture
	{
		get;
		private set;
	}
	public GUIText GuiText
	{
		get;
		private set;
	}
	public static bool DrawGizmos
	{
		get;
		set;
	}
	public Skill Fsm
	{
		get
		{
			this.fsm.Owner = this;
			return this.fsm;
		}
	}
	public string FsmName
	{
		get
		{
			return this.fsm.Name;
		}
		set
		{
			this.fsm.Name = value;
		}
	}
	public string FsmDescription
	{
		get
		{
			return this.fsm.Description;
		}
		set
		{
			this.fsm.Description = value;
		}
	}
	public bool Active
	{
		get
		{
			return this.fsm.Active;
		}
	}
	public string ActiveStateName
	{
		get
		{
			if (this.fsm.ActiveState != null)
			{
				return this.fsm.ActiveState.Name;
			}
			return "";
		}
	}
	public SkillState[] FsmStates
	{
		get
		{
			return this.fsm.States;
		}
	}
	public SkillEvent[] FsmEvents
	{
		get
		{
			return this.fsm.Events;
		}
	}
	public SkillTransition[] FsmGlobalTransitions
	{
		get
		{
			return this.fsm.GlobalTransitions;
		}
	}
	public SkillVariables FsmVariables
	{
		get
		{
			return this.fsm.Variables;
		}
	}
	public bool UsesTemplate
	{
		get
		{
			return this.fsmTemplate != null;
		}
	}
	public static PlayMakerFSM FindFsmOnGameObject(GameObject go, string fsmName)
	{
		using (List<PlayMakerFSM>.Enumerator enumerator = PlayMakerFSM.fsmList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				PlayMakerFSM current = enumerator.get_Current();
				if (current.get_gameObject() == go && current.FsmName == fsmName)
				{
					return current;
				}
			}
		}
		return null;
	}
	public void Reset()
	{
		if (this.fsm == null)
		{
			this.fsm = new Skill();
		}
		this.fsmTemplate = null;
		this.fsm.Reset(this);
	}
	private void Awake()
	{
		PlayMakerGlobals.Initialize();
		if (!PlayMakerGlobals.IsEditor)
		{
			SkillLog.LoggingEnabled = false;
		}
		this.Init();
	}
	public void Preprocess()
	{
		if (this.fsmTemplate != null)
		{
			this.InitTemplate();
		}
		else
		{
			this.InitFsm();
		}
		this.fsm.Preprocess(this);
		this.AddEventHandlerComponents();
	}
	private void Init()
	{
		if (this.fsmTemplate != null)
		{
			if (Application.get_isPlaying())
			{
				this.InitTemplate();
			}
		}
		else
		{
			this.InitFsm();
		}
		if (PlayMakerGlobals.IsEditor)
		{
			this.fsm.Preprocessed = false;
			this.eventHandlerComponentsAdded = false;
		}
		this.fsm.Init(this);
		if (!this.eventHandlerComponentsAdded || !this.fsm.Preprocessed)
		{
			this.AddEventHandlerComponents();
		}
	}
	private void InitTemplate()
	{
		string name = this.fsm.Name;
		bool enableDebugFlow = this.fsm.EnableDebugFlow;
		bool enableBreakpoints = this.fsm.EnableBreakpoints;
		bool showStateLabel = this.fsm.ShowStateLabel;
		this.fsm = new Skill(this.fsmTemplate.fsm, this.fsm.Variables)
		{
			Name = name,
			UsedInTemplate = null,
			EnableDebugFlow = enableDebugFlow,
			EnableBreakpoints = enableBreakpoints,
			ShowStateLabel = showStateLabel
		};
	}
	private void InitFsm()
	{
		if (this.fsm == null)
		{
			this.Reset();
		}
		if (this.fsm == null)
		{
			Debug.LogError("Could not initialize FSM!");
			base.set_enabled(false);
		}
	}
	public void AddEventHandlerComponents()
	{
		if (!PlayMakerGlobals.IsEditor)
		{
			Debug.Log("FSM not Preprocessed: " + SkillUtility.GetFullFsmLabel(this.fsm));
		}
		if (this.fsm.MouseEvents)
		{
			this.AddEventHandlerComponent<PlayMakerMouseEvents>(2);
		}
		if (this.fsm.HandleCollisionEnter)
		{
			this.AddEventHandlerComponent<PlayMakerCollisionEnter>(2);
		}
		if (this.fsm.HandleCollisionExit)
		{
			this.AddEventHandlerComponent<PlayMakerCollisionExit>(2);
		}
		if (this.fsm.HandleCollisionStay)
		{
			this.AddEventHandlerComponent<PlayMakerCollisionStay>(2);
		}
		if (this.fsm.HandleTriggerEnter)
		{
			this.AddEventHandlerComponent<PlayMakerTriggerEnter>(2);
		}
		if (this.fsm.HandleTriggerExit)
		{
			this.AddEventHandlerComponent<PlayMakerTriggerExit>(2);
		}
		if (this.fsm.HandleTriggerStay)
		{
			this.AddEventHandlerComponent<PlayMakerTriggerStay>(2);
		}
		if (this.fsm.HandleCollisionEnter2D)
		{
			this.AddEventHandlerComponent<PlayMakerCollisionEnter2D>(2);
		}
		if (this.fsm.HandleCollisionExit2D)
		{
			this.AddEventHandlerComponent<PlayMakerCollisionExit2D>(2);
		}
		if (this.fsm.HandleCollisionStay2D)
		{
			this.AddEventHandlerComponent<PlayMakerCollisionStay2D>(2);
		}
		if (this.fsm.HandleTriggerEnter2D)
		{
			this.AddEventHandlerComponent<PlayMakerTriggerEnter2D>(2);
		}
		if (this.fsm.HandleTriggerExit2D)
		{
			this.AddEventHandlerComponent<PlayMakerTriggerExit2D>(2);
		}
		if (this.fsm.HandleTriggerStay2D)
		{
			this.AddEventHandlerComponent<PlayMakerTriggerStay2D>(2);
		}
		if (this.fsm.HandleParticleCollision)
		{
			this.AddEventHandlerComponent<PlayMakerParticleCollision>(2);
		}
		if (this.fsm.HandleControllerColliderHit)
		{
			this.AddEventHandlerComponent<PlayMakerControllerColliderHit>(2);
		}
		if (this.fsm.HandleJointBreak)
		{
			this.AddEventHandlerComponent<PlayMakerJointBreak>(2);
		}
		if (this.fsm.HandleJointBreak2D)
		{
			this.AddEventHandlerComponent<PlayMakerJointBreak>(2);
		}
		if (this.fsm.HandleFixedUpdate)
		{
			this.AddEventHandlerComponent<PlayMakerFixedUpdate>(2);
		}
		if (this.fsm.HandleOnGUI && base.GetComponent<PlayMakerOnGUI>() == null)
		{
			PlayMakerOnGUI playMakerOnGUI = base.get_gameObject().AddComponent<PlayMakerOnGUI>();
			playMakerOnGUI.playMakerFSM = this;
		}
		if (this.fsm.HandleApplicationEvents)
		{
			this.AddEventHandlerComponent<PlayMakerApplicationEvents>(2);
		}
		if (this.fsm.HandleAnimatorMove)
		{
			this.AddEventHandlerComponent<PlayMakerAnimatorMove>(2);
		}
		if (this.fsm.HandleAnimatorIK)
		{
			this.AddEventHandlerComponent<PlayMakerAnimatorIK>(2);
		}
		this.eventHandlerComponentsAdded = true;
	}
	private void AddEventHandlerComponent<T>(HideFlags hide = 2) where T : Component
	{
		if (base.GetComponent<T>() != null)
		{
			return;
		}
		if (!PlayMakerGlobals.IsEditor)
		{
			Debug.Log("AddEventHandlerComponent: " + typeof(T));
		}
		T t = base.get_gameObject().AddComponent<T>();
		t.set_hideFlags(hide);
	}
	public void SetFsmTemplate(SkillTemplate template)
	{
		this.fsmTemplate = template;
		this.Fsm.Clear(this);
		if (template != null)
		{
			this.Fsm.Variables = new SkillVariables(this.fsmTemplate.fsm.Variables);
		}
		this.Init();
	}
	private void Start()
	{
		this.GuiTexture = base.GetComponent<GUITexture>();
		this.GuiText = base.GetComponent<GUIText>();
		if (!this.fsm.Started)
		{
			this.fsm.Start();
		}
	}
	private void OnEnable()
	{
		PlayMakerFSM.fsmList.Add(this);
		this.fsm.OnEnable();
	}
	private void Update()
	{
		if (!this.fsm.Finished && !this.fsm.ManualUpdate)
		{
			this.fsm.Update();
		}
	}
	private void LateUpdate()
	{
		SkillVariables.GlobalVariablesSynced = false;
		if (!this.fsm.Finished)
		{
			this.fsm.LateUpdate();
		}
	}
	public IEnumerator DoCoroutine(IEnumerator routine)
	{
		while (true)
		{
			SkillExecutionStack.PushFsm(this.fsm);
			if (!routine.MoveNext())
			{
				break;
			}
			SkillExecutionStack.PopFsm();
			yield return routine.get_Current();
		}
		SkillExecutionStack.PopFsm();
		yield break;
	}
	private void OnDisable()
	{
		PlayMakerFSM.fsmList.Remove(this);
		if (this.fsm != null && !this.fsm.Finished)
		{
			this.fsm.OnDisable();
		}
	}
	private void OnDestroy()
	{
		PlayMakerFSM.fsmList.Remove(this);
		if (this.fsm != null)
		{
			this.fsm.OnDestroy();
		}
	}
	private void OnApplicationQuit()
	{
		this.fsm.Event(SkillEvent.ApplicationQuit);
		PlayMakerFSM.ApplicationIsQuitting = true;
	}
	private void OnDrawGizmos()
	{
		if (this.fsm != null)
		{
			this.fsm.OnDrawGizmos();
		}
	}
	public void SetState(string stateName)
	{
		this.fsm.SetState(stateName);
	}
	public void ChangeState(SkillEvent fsmEvent)
	{
		this.fsm.Event(fsmEvent);
	}
	[Obsolete("Use SendEvent(string) instead.")]
	public void ChangeState(string eventName)
	{
		this.fsm.Event(eventName);
	}
	public void SendEvent(string eventName)
	{
		this.fsm.Event(eventName);
	}
	[RPC]
	public void SendRemoteFsmEvent(string eventName)
	{
		this.fsm.Event(eventName);
	}
	[RPC]
	public void SendRemoteFsmEventWithData(string eventName, string eventData)
	{
		Skill.EventData.StringData = eventData;
		this.fsm.Event(eventName);
	}
	public static void BroadcastEvent(string fsmEventName)
	{
		if (!string.IsNullOrEmpty(fsmEventName))
		{
			PlayMakerFSM.BroadcastEvent(SkillEvent.GetFsmEvent(fsmEventName));
		}
	}
	public static void BroadcastEvent(SkillEvent fsmEvent)
	{
		List<PlayMakerFSM> list = new List<PlayMakerFSM>(PlayMakerFSM.FsmList);
		using (List<PlayMakerFSM>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				PlayMakerFSM current = enumerator.get_Current();
				if (!(current == null) && current.Fsm != null)
				{
					current.Fsm.ProcessEvent(fsmEvent, null);
				}
			}
		}
	}
	private void OnBecameVisible()
	{
		this.fsm.Event(SkillEvent.BecameVisible);
	}
	private void OnBecameInvisible()
	{
		this.fsm.Event(SkillEvent.BecameInvisible);
	}
	private void OnPlayerConnected(NetworkPlayer player)
	{
		Skill.EventData.Player = player;
		this.fsm.Event(SkillEvent.PlayerConnected);
	}
	private void OnServerInitialized()
	{
		this.fsm.Event(SkillEvent.ServerInitialized);
	}
	private void OnConnectedToServer()
	{
		this.fsm.Event(SkillEvent.ConnectedToServer);
	}
	private void OnPlayerDisconnected(NetworkPlayer player)
	{
		Skill.EventData.Player = player;
		this.fsm.Event(SkillEvent.PlayerDisconnected);
	}
	private void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Skill.EventData.DisconnectionInfo = info;
		this.fsm.Event(SkillEvent.DisconnectedFromServer);
	}
	private void OnFailedToConnect(NetworkConnectionError error)
	{
		Skill.EventData.ConnectionError = error;
		this.fsm.Event(SkillEvent.FailedToConnect);
	}
	private void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		Skill.EventData.NetworkMessageInfo = info;
		this.fsm.Event(SkillEvent.NetworkInstantiate);
	}
	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (!SkillVariables.GlobalVariablesSynced)
		{
			SkillVariables.GlobalVariablesSynced = true;
			PlayMakerFSM.NetworkSyncVariables(stream, SkillVariables.GlobalVariables);
		}
		PlayMakerFSM.NetworkSyncVariables(stream, this.Fsm.Variables);
	}
	private static void NetworkSyncVariables(BitStream stream, SkillVariables variables)
	{
		SkillInt[] intVariables;
		SkillQuaternion[] quaternionVariables;
		SkillVector3[] vector3Variables;
		SkillColor[] colorVariables;
		SkillVector2[] vector2Variables;
		if (stream.get_isWriting())
		{
			SkillString[] stringVariables = variables.StringVariables;
			for (int i = 0; i < stringVariables.Length; i++)
			{
				SkillString fsmString = stringVariables[i];
				if (fsmString.NetworkSync)
				{
					char[] array = fsmString.Value.ToCharArray();
					int num = array.Length;
					stream.Serialize(ref num);
					for (int j = 0; j < num; j++)
					{
						stream.Serialize(ref array[j]);
					}
				}
			}
			SkillBool[] boolVariables = variables.BoolVariables;
			for (int k = 0; k < boolVariables.Length; k++)
			{
				SkillBool fsmBool = boolVariables[k];
				if (fsmBool.NetworkSync)
				{
					bool value = fsmBool.Value;
					stream.Serialize(ref value);
				}
			}
			SkillFloat[] floatVariables = variables.FloatVariables;
			for (int l = 0; l < floatVariables.Length; l++)
			{
				SkillFloat fsmFloat = floatVariables[l];
				if (fsmFloat.NetworkSync)
				{
					float value2 = fsmFloat.Value;
					stream.Serialize(ref value2);
				}
			}
			intVariables = variables.IntVariables;
			for (int m = 0; m < intVariables.Length; m++)
			{
				SkillInt fsmInt = intVariables[m];
				if (fsmInt.NetworkSync)
				{
					int value3 = fsmInt.Value;
					stream.Serialize(ref value3);
				}
			}
			quaternionVariables = variables.QuaternionVariables;
			for (int n = 0; n < quaternionVariables.Length; n++)
			{
				SkillQuaternion fsmQuaternion = quaternionVariables[n];
				if (fsmQuaternion.NetworkSync)
				{
					Quaternion value4 = fsmQuaternion.Value;
					stream.Serialize(ref value4);
				}
			}
			vector3Variables = variables.Vector3Variables;
			for (int num2 = 0; num2 < vector3Variables.Length; num2++)
			{
				SkillVector3 fsmVector = vector3Variables[num2];
				if (fsmVector.NetworkSync)
				{
					Vector3 value5 = fsmVector.Value;
					stream.Serialize(ref value5);
				}
			}
			colorVariables = variables.ColorVariables;
			for (int num3 = 0; num3 < colorVariables.Length; num3++)
			{
				SkillColor fsmColor = colorVariables[num3];
				if (fsmColor.NetworkSync)
				{
					Color value6 = fsmColor.Value;
					stream.Serialize(ref value6.r);
					stream.Serialize(ref value6.g);
					stream.Serialize(ref value6.b);
					stream.Serialize(ref value6.a);
				}
			}
			vector2Variables = variables.Vector2Variables;
			for (int num4 = 0; num4 < vector2Variables.Length; num4++)
			{
				SkillVector2 fsmVector2 = vector2Variables[num4];
				if (fsmVector2.NetworkSync)
				{
					Vector2 value7 = fsmVector2.Value;
					stream.Serialize(ref value7.x);
					stream.Serialize(ref value7.y);
				}
			}
			return;
		}
		SkillString[] stringVariables2 = variables.StringVariables;
		for (int num5 = 0; num5 < stringVariables2.Length; num5++)
		{
			SkillString fsmString2 = stringVariables2[num5];
			if (fsmString2.NetworkSync)
			{
				int num6 = 0;
				stream.Serialize(ref num6);
				char[] array2 = new char[num6];
				for (int num7 = 0; num7 < num6; num7++)
				{
					stream.Serialize(ref array2[num7]);
				}
				fsmString2.Value = new string(array2);
			}
		}
		SkillBool[] boolVariables2 = variables.BoolVariables;
		for (int num8 = 0; num8 < boolVariables2.Length; num8++)
		{
			SkillBool fsmBool2 = boolVariables2[num8];
			if (fsmBool2.NetworkSync)
			{
				bool value8 = false;
				stream.Serialize(ref value8);
				fsmBool2.Value = value8;
			}
		}
		SkillFloat[] floatVariables2 = variables.FloatVariables;
		for (int i = 0; i < floatVariables2.Length; i++)
		{
			SkillFloat fsmFloat2 = floatVariables2[i];
			if (fsmFloat2.NetworkSync)
			{
				float value9 = 0f;
				stream.Serialize(ref value9);
				fsmFloat2.Value = value9;
			}
		}
		intVariables = variables.IntVariables;
		for (int i = 0; i < intVariables.Length; i++)
		{
			SkillInt fsmInt2 = intVariables[i];
			if (fsmInt2.NetworkSync)
			{
				int value10 = 0;
				stream.Serialize(ref value10);
				fsmInt2.Value = value10;
			}
		}
		quaternionVariables = variables.QuaternionVariables;
		for (int i = 0; i < quaternionVariables.Length; i++)
		{
			SkillQuaternion fsmQuaternion2 = quaternionVariables[i];
			if (fsmQuaternion2.NetworkSync)
			{
				Quaternion identity = Quaternion.get_identity();
				stream.Serialize(ref identity);
				fsmQuaternion2.Value = identity;
			}
		}
		vector3Variables = variables.Vector3Variables;
		for (int i = 0; i < vector3Variables.Length; i++)
		{
			SkillVector3 fsmVector3 = vector3Variables[i];
			if (fsmVector3.NetworkSync)
			{
				Vector3 zero = Vector3.get_zero();
				stream.Serialize(ref zero);
				fsmVector3.Value = zero;
			}
		}
		colorVariables = variables.ColorVariables;
		for (int i = 0; i < colorVariables.Length; i++)
		{
			SkillColor fsmColor2 = colorVariables[i];
			if (fsmColor2.NetworkSync)
			{
				float num9 = 0f;
				stream.Serialize(ref num9);
				float num10 = 0f;
				stream.Serialize(ref num10);
				float num11 = 0f;
				stream.Serialize(ref num11);
				float num12 = 0f;
				stream.Serialize(ref num12);
				fsmColor2.Value = new Color(num9, num10, num11, num12);
			}
		}
		vector2Variables = variables.Vector2Variables;
		for (int i = 0; i < vector2Variables.Length; i++)
		{
			SkillVector2 fsmVector4 = vector2Variables[i];
			if (fsmVector4.NetworkSync)
			{
				float num13 = 0f;
				stream.Serialize(ref num13);
				float num14 = 0f;
				stream.Serialize(ref num14);
				fsmVector4.Value = new Vector2(num13, num14);
			}
		}
	}
	private void OnMasterServerEvent(MasterServerEvent masterServerEvent)
	{
		Skill.EventData.MasterServerEvent = masterServerEvent;
		this.fsm.Event(SkillEvent.MasterServerEvent);
	}
	public void OnBeforeSerialize()
	{
	}
	public void OnAfterDeserialize()
	{
		PlayMakerFSM.NotMainThread = true;
		if (PlayMakerGlobals.Initialized)
		{
			this.fsm.InitData();
		}
		PlayMakerFSM.NotMainThread = false;
	}
}
