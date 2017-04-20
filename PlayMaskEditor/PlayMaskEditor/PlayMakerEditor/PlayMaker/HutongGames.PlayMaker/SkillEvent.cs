using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillEvent : IComparable, INameable
	{
		private static List<SkillEvent> eventList;
		private static readonly object syncObj = new object();
		[SerializeField]
		private string name;
		[SerializeField]
		private bool isSystemEvent;
		[SerializeField]
		private bool isGlobal;
		public static PlayMakerGlobals GlobalsComponent
		{
			get
			{
				return PlayMakerGlobals.Instance;
			}
		}
		public static List<string> globalEvents
		{
			get
			{
				return PlayMakerGlobals.Instance.Events;
			}
		}
		public static List<SkillEvent> EventList
		{
			get
			{
				if (SkillEvent.eventList == null)
				{
					SkillEvent.Initialize();
				}
				return SkillEvent.eventList;
			}
			private set
			{
				SkillEvent.eventList = value;
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
		public bool IsSystemEvent
		{
			get
			{
				return this.isSystemEvent;
			}
			set
			{
				this.isSystemEvent = value;
			}
		}
		public bool IsMouseEvent
		{
			get
			{
				return this == SkillEvent.MouseDown || this == SkillEvent.MouseDrag || this == SkillEvent.MouseEnter || this == SkillEvent.MouseExit || this == SkillEvent.MouseOver || this == SkillEvent.MouseUp || this == SkillEvent.MouseUpAsButton;
			}
		}
		public bool IsApplicationEvent
		{
			get
			{
				return this == SkillEvent.ApplicationFocus || this == SkillEvent.ApplicationPause;
			}
		}
		public bool IsGlobal
		{
			get
			{
				return this.isGlobal;
			}
			set
			{
				if (value)
				{
					if (!SkillEvent.globalEvents.Contains(this.name))
					{
						SkillEvent.globalEvents.Add(this.name);
					}
				}
				else
				{
					SkillEvent.globalEvents.RemoveAll((string m) => m == this.name);
				}
				this.isGlobal = value;
				SkillEvent.SanityCheckEventList();
			}
		}
		public string Path
		{
			get;
			set;
		}
		public static SkillEvent BecameInvisible
		{
			get;
			private set;
		}
		public static SkillEvent BecameVisible
		{
			get;
			private set;
		}
		public static SkillEvent CollisionEnter
		{
			get;
			private set;
		}
		public static SkillEvent CollisionExit
		{
			get;
			private set;
		}
		public static SkillEvent CollisionStay
		{
			get;
			private set;
		}
		public static SkillEvent CollisionEnter2D
		{
			get;
			private set;
		}
		public static SkillEvent CollisionExit2D
		{
			get;
			private set;
		}
		public static SkillEvent CollisionStay2D
		{
			get;
			private set;
		}
		public static SkillEvent ControllerColliderHit
		{
			get;
			private set;
		}
		public static SkillEvent Finished
		{
			get;
			private set;
		}
		public static SkillEvent LevelLoaded
		{
			get;
			private set;
		}
		public static SkillEvent MouseDown
		{
			get;
			private set;
		}
		public static SkillEvent MouseDrag
		{
			get;
			private set;
		}
		public static SkillEvent MouseEnter
		{
			get;
			private set;
		}
		public static SkillEvent MouseExit
		{
			get;
			private set;
		}
		public static SkillEvent MouseOver
		{
			get;
			private set;
		}
		public static SkillEvent MouseUp
		{
			get;
			private set;
		}
		public static SkillEvent MouseUpAsButton
		{
			get;
			private set;
		}
		public static SkillEvent TriggerEnter
		{
			get;
			private set;
		}
		public static SkillEvent TriggerExit
		{
			get;
			private set;
		}
		public static SkillEvent TriggerStay
		{
			get;
			private set;
		}
		public static SkillEvent TriggerEnter2D
		{
			get;
			private set;
		}
		public static SkillEvent TriggerExit2D
		{
			get;
			private set;
		}
		public static SkillEvent TriggerStay2D
		{
			get;
			private set;
		}
		public static SkillEvent ApplicationFocus
		{
			get;
			private set;
		}
		public static SkillEvent ApplicationPause
		{
			get;
			private set;
		}
		public static SkillEvent ApplicationQuit
		{
			get;
			private set;
		}
		public static SkillEvent ParticleCollision
		{
			get;
			private set;
		}
		public static SkillEvent JointBreak
		{
			get;
			private set;
		}
		public static SkillEvent JointBreak2D
		{
			get;
			private set;
		}
		public static SkillEvent PlayerConnected
		{
			get;
			private set;
		}
		public static SkillEvent ServerInitialized
		{
			get;
			private set;
		}
		public static SkillEvent ConnectedToServer
		{
			get;
			private set;
		}
		public static SkillEvent PlayerDisconnected
		{
			get;
			private set;
		}
		public static SkillEvent DisconnectedFromServer
		{
			get;
			private set;
		}
		public static SkillEvent FailedToConnect
		{
			get;
			private set;
		}
		public static SkillEvent FailedToConnectToMasterServer
		{
			get;
			private set;
		}
		public static SkillEvent MasterServerEvent
		{
			get;
			private set;
		}
		public static SkillEvent NetworkInstantiate
		{
			get;
			private set;
		}
		private static void Initialize()
		{
			PlayMakerGlobals.Initialize();
			SkillEvent.eventList = new List<SkillEvent>();
			SkillEvent.AddSystemEvents();
			SkillEvent.AddGlobalEvents();
		}
		public static bool IsNullOrEmpty(SkillEvent fsmEvent)
		{
			return fsmEvent == null || string.IsNullOrEmpty(fsmEvent.name);
		}
		public SkillEvent(string name)
		{
			object obj;
			Monitor.Enter(obj = SkillEvent.syncObj);
			try
			{
				this.name = name;
				if (!SkillEvent.EventListContainsEvent(SkillEvent.EventList, name))
				{
					SkillEvent.EventList.Add(this);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public SkillEvent(SkillEvent source)
		{
			object obj;
			Monitor.Enter(obj = SkillEvent.syncObj);
			try
			{
				this.name = source.name;
				this.isSystemEvent = source.isSystemEvent;
				this.isGlobal = source.isGlobal;
				SkillEvent fsmEvent = SkillEvent.EventList.Find((SkillEvent x) => x.name == this.name);
				if (fsmEvent == null)
				{
					SkillEvent.EventList.Add(this);
				}
				else
				{
					fsmEvent.isGlobal |= this.isGlobal;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		int IComparable.CompareTo(object obj)
		{
			SkillEvent fsmEvent = (SkillEvent)obj;
			if (this.isSystemEvent && !fsmEvent.isSystemEvent)
			{
				return -1;
			}
			if (!this.isSystemEvent && fsmEvent.isSystemEvent)
			{
				return 1;
			}
			return string.CompareOrdinal(this.name, fsmEvent.name);
		}
		public static bool EventListContainsEvent(List<SkillEvent> fsmEventList, string fsmEventName)
		{
			object obj;
			Monitor.Enter(obj = SkillEvent.syncObj);
			bool result;
			try
			{
				if (fsmEventList == null || string.IsNullOrEmpty(fsmEventName))
				{
					result = false;
				}
				else
				{
					for (int i = 0; i < fsmEventList.get_Count(); i++)
					{
						if (fsmEventList.get_Item(i).Name == fsmEventName)
						{
							result = true;
							return result;
						}
					}
					result = false;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public static void RemoveEventFromEventList(SkillEvent fsmEvent)
		{
			if (fsmEvent.isSystemEvent)
			{
				Debug.LogError("RemoveEventFromEventList: Trying to delete System Event: " + fsmEvent.Name);
			}
			SkillEvent.EventList.Remove(fsmEvent);
		}
		public static SkillEvent FindEvent(string eventName)
		{
			object obj;
			Monitor.Enter(obj = SkillEvent.syncObj);
			SkillEvent result;
			try
			{
				for (int i = 0; i < SkillEvent.EventList.get_Count(); i++)
				{
					SkillEvent fsmEvent = SkillEvent.EventList.get_Item(i);
					if (fsmEvent.name == eventName)
					{
						result = fsmEvent;
						return result;
					}
				}
				result = null;
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public static bool IsEventGlobal(string eventName)
		{
			return SkillEvent.globalEvents.Contains(eventName);
		}
		public static bool EventListContains(string eventName)
		{
			return SkillEvent.FindEvent(eventName) != null;
		}
		public static SkillEvent GetFsmEvent(string eventName)
		{
			object obj;
			Monitor.Enter(obj = SkillEvent.syncObj);
			SkillEvent result;
			try
			{
				for (int i = 0; i < SkillEvent.EventList.get_Count(); i++)
				{
					SkillEvent fsmEvent = SkillEvent.EventList.get_Item(i);
					if (string.CompareOrdinal(fsmEvent.Name, eventName) == 0)
					{
						result = (PlayMakerGlobals.IsPlaying ? fsmEvent : new SkillEvent(fsmEvent));
						return result;
					}
				}
				SkillEvent fsmEvent2 = new SkillEvent(eventName);
				result = (PlayMakerGlobals.IsPlaying ? fsmEvent2 : new SkillEvent(fsmEvent2));
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public static SkillEvent GetFsmEvent(SkillEvent fsmEvent)
		{
			if (fsmEvent == null)
			{
				return null;
			}
			object obj;
			Monitor.Enter(obj = SkillEvent.syncObj);
			SkillEvent result;
			try
			{
				for (int i = 0; i < SkillEvent.EventList.get_Count(); i++)
				{
					SkillEvent fsmEvent2 = SkillEvent.EventList.get_Item(i);
					if (string.CompareOrdinal(fsmEvent2.Name, fsmEvent.Name) == 0)
					{
						result = (PlayMakerGlobals.IsPlaying ? fsmEvent2 : new SkillEvent(fsmEvent));
						return result;
					}
				}
				if (fsmEvent.isSystemEvent)
				{
					Debug.LogError("Missing System Event: " + fsmEvent.Name);
				}
				result = SkillEvent.AddFsmEvent(fsmEvent);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public static SkillEvent AddFsmEvent(SkillEvent fsmEvent)
		{
			SkillEvent.EventList.Add(fsmEvent);
			return fsmEvent;
		}
		private static void AddSystemEvents()
		{
			SkillEvent.Finished = SkillEvent.AddSystemEvent("FINISHED", "System Events");
			SkillEvent.BecameInvisible = SkillEvent.AddSystemEvent("BECAME INVISIBLE", "System Events");
			SkillEvent.BecameVisible = SkillEvent.AddSystemEvent("BECAME VISIBLE", "System Events");
			SkillEvent.LevelLoaded = SkillEvent.AddSystemEvent("LEVEL LOADED", "System Events");
			SkillEvent.MouseDown = SkillEvent.AddSystemEvent("MOUSE DOWN", "System Events");
			SkillEvent.MouseDrag = SkillEvent.AddSystemEvent("MOUSE DRAG", "System Events");
			SkillEvent.MouseEnter = SkillEvent.AddSystemEvent("MOUSE ENTER", "System Events");
			SkillEvent.MouseExit = SkillEvent.AddSystemEvent("MOUSE EXIT", "System Events");
			SkillEvent.MouseOver = SkillEvent.AddSystemEvent("MOUSE OVER", "System Events");
			SkillEvent.MouseUp = SkillEvent.AddSystemEvent("MOUSE UP", "System Events");
			SkillEvent.MouseUpAsButton = SkillEvent.AddSystemEvent("MOUSE UP AS BUTTON", "System Events");
			SkillEvent.CollisionEnter = SkillEvent.AddSystemEvent("COLLISION ENTER", "System Events");
			SkillEvent.CollisionExit = SkillEvent.AddSystemEvent("COLLISION EXIT", "System Events");
			SkillEvent.CollisionStay = SkillEvent.AddSystemEvent("COLLISION STAY", "System Events");
			SkillEvent.ControllerColliderHit = SkillEvent.AddSystemEvent("CONTROLLER COLLIDER HIT", "System Events");
			SkillEvent.TriggerEnter = SkillEvent.AddSystemEvent("TRIGGER ENTER", "System Events");
			SkillEvent.TriggerExit = SkillEvent.AddSystemEvent("TRIGGER EXIT", "System Events");
			SkillEvent.TriggerStay = SkillEvent.AddSystemEvent("TRIGGER STAY", "System Events");
			SkillEvent.CollisionEnter2D = SkillEvent.AddSystemEvent("COLLISION ENTER 2D", "System Events");
			SkillEvent.CollisionExit2D = SkillEvent.AddSystemEvent("COLLISION EXIT 2D", "System Events");
			SkillEvent.CollisionStay2D = SkillEvent.AddSystemEvent("COLLISION STAY 2D", "System Events");
			SkillEvent.TriggerEnter2D = SkillEvent.AddSystemEvent("TRIGGER ENTER 2D", "System Events");
			SkillEvent.TriggerExit2D = SkillEvent.AddSystemEvent("TRIGGER EXIT 2D", "System Events");
			SkillEvent.TriggerStay2D = SkillEvent.AddSystemEvent("TRIGGER STAY 2D", "System Events");
			SkillEvent.PlayerConnected = SkillEvent.AddSystemEvent("PLAYER CONNECTED", "Network Events");
			SkillEvent.ServerInitialized = SkillEvent.AddSystemEvent("SERVER INITIALIZED", "Network Events");
			SkillEvent.ConnectedToServer = SkillEvent.AddSystemEvent("CONNECTED TO SERVER", "Network Events");
			SkillEvent.PlayerDisconnected = SkillEvent.AddSystemEvent("PLAYER DISCONNECTED", "Network Events");
			SkillEvent.DisconnectedFromServer = SkillEvent.AddSystemEvent("DISCONNECTED FROM SERVER", "Network Events");
			SkillEvent.FailedToConnect = SkillEvent.AddSystemEvent("FAILED TO CONNECT", "Network Events");
			SkillEvent.FailedToConnectToMasterServer = SkillEvent.AddSystemEvent("FAILED TO CONNECT TO MASTER SERVER", "Network Events");
			SkillEvent.MasterServerEvent = SkillEvent.AddSystemEvent("MASTER SERVER EVENT", "Network Events");
			SkillEvent.NetworkInstantiate = SkillEvent.AddSystemEvent("NETWORK INSTANTIATE", "Network Events");
			SkillEvent.ApplicationFocus = SkillEvent.AddSystemEvent("APPLICATION FOCUS", "System Events");
			SkillEvent.ApplicationPause = SkillEvent.AddSystemEvent("APPLICATION PAUSE", "System Events");
			SkillEvent.ApplicationQuit = SkillEvent.AddSystemEvent("APPLICATION QUIT", "System Events");
			SkillEvent.ParticleCollision = SkillEvent.AddSystemEvent("PARTICLE COLLISION", "System Events");
			SkillEvent.JointBreak = SkillEvent.AddSystemEvent("JOINT BREAK", "System Events");
			SkillEvent.JointBreak2D = SkillEvent.AddSystemEvent("JOINT BREAK 2D", "System Events");
		}
		private static SkillEvent AddSystemEvent(string eventName, string path = "")
		{
			return new SkillEvent(eventName)
			{
				IsSystemEvent = true,
				Path = (path == "") ? "" : (path + "/")
			};
		}
		private static void AddGlobalEvents()
		{
			for (int i = 0; i < SkillEvent.globalEvents.get_Count(); i++)
			{
				string text = SkillEvent.globalEvents.get_Item(i);
				SkillEvent fsmEvent = new SkillEvent(text);
				fsmEvent.isGlobal = true;
			}
		}
		public static void SanityCheckEventList()
		{
			using (List<SkillEvent>.Enumerator enumerator = SkillEvent.EventList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					if (SkillEvent.IsEventGlobal(current.name))
					{
						current.isGlobal = true;
					}
					if (current.isGlobal && !SkillEvent.globalEvents.Contains(current.name))
					{
						SkillEvent.globalEvents.Add(current.name);
					}
				}
			}
			List<SkillEvent> list = new List<SkillEvent>();
			using (List<SkillEvent>.Enumerator enumerator2 = SkillEvent.EventList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					SkillEvent current2 = enumerator2.get_Current();
					if (!SkillEvent.EventListContainsEvent(list, current2.Name))
					{
						list.Add(current2);
					}
				}
			}
			SkillEvent.EventList = list;
		}
	}
}
