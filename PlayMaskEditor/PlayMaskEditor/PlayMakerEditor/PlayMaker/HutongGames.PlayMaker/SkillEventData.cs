using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	public class SkillEventData
	{
		public Skill SentByFsm;
		public SkillState SentByState;
		public SkillStateAction SentByAction;
		public bool BoolData;
		public int IntData;
		public float FloatData;
		public Vector2 Vector2Data;
		public Vector3 Vector3Data;
		public string StringData;
		public Quaternion QuaternionData;
		public Rect RectData;
		public Color ColorData;
		public Object ObjectData;
		public GameObject GameObjectData;
		public Material MaterialData;
		public Texture TextureData;
		public NetworkPlayer Player;
		public NetworkDisconnection DisconnectionInfo;
		public NetworkConnectionError ConnectionError;
		public NetworkMessageInfo NetworkMessageInfo;
		public MasterServerEvent MasterServerEvent;
		public SkillEventData()
		{
		}
		public SkillEventData(SkillEventData source)
		{
			this.SentByFsm = source.SentByFsm;
			this.SentByState = source.SentByState;
			this.SentByAction = source.SentByAction;
			this.BoolData = source.BoolData;
			this.IntData = source.IntData;
			this.FloatData = source.FloatData;
			this.Vector2Data = source.Vector2Data;
			this.Vector3Data = source.Vector3Data;
			this.StringData = source.StringData;
			this.QuaternionData = source.QuaternionData;
			this.RectData = source.RectData;
			this.ColorData = source.ColorData;
			this.ObjectData = source.ObjectData;
			this.GameObjectData = source.GameObjectData;
			this.MaterialData = source.MaterialData;
			this.TextureData = source.TextureData;
			this.Player = source.Player;
			this.DisconnectionInfo = source.DisconnectionInfo;
			this.ConnectionError = source.ConnectionError;
			this.NetworkMessageInfo = source.NetworkMessageInfo;
			this.MasterServerEvent = source.MasterServerEvent;
		}
		public void DebugLog()
		{
			Debug.Log("Sent By FSM: " + ((this.SentByFsm != null) ? this.SentByFsm.Name : "None"));
			Debug.Log("Sent By State: " + ((this.SentByState != null) ? this.SentByState.Name : "None"));
			Debug.Log("Sent By Action: " + ((this.SentByAction != null) ? this.SentByAction.GetType().get_Name() : "None"));
		}
	}
}
