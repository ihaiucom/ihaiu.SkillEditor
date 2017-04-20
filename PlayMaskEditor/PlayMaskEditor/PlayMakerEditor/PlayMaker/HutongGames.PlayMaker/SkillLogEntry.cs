using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	public class SkillLogEntry
	{
		private string textWithTimecode;
		public SkillLog Log
		{
			get;
			set;
		}
		public SkillLogType LogType
		{
			get;
			set;
		}
		public Skill Fsm
		{
			get
			{
				return this.Log.Fsm;
			}
		}
		public SkillState State
		{
			get;
			set;
		}
		public SkillState SentByState
		{
			get;
			set;
		}
		public SkillStateAction Action
		{
			get;
			set;
		}
		public SkillEvent Event
		{
			get;
			set;
		}
		public SkillTransition Transition
		{
			get;
			set;
		}
		public SkillEventTarget EventTarget
		{
			get;
			set;
		}
		public float Time
		{
			get;
			set;
		}
		public string Text
		{
			get;
			set;
		}
		public string Text2
		{
			get;
			set;
		}
		public int FrameCount
		{
			get;
			set;
		}
		public SkillVariables FsmVariablesCopy
		{
			get;
			set;
		}
		public SkillVariables GlobalVariablesCopy
		{
			get;
			set;
		}
		public GameObject GameObject
		{
			get;
			set;
		}
		public string GameObjectName
		{
			get;
			set;
		}
		public Texture GameObjectIcon
		{
			get;
			set;
		}
		public string TextWithTimecode
		{
			get
			{
				string arg_2E_0;
				if ((arg_2E_0 = this.textWithTimecode) == null)
				{
					arg_2E_0 = (this.textWithTimecode = SkillTime.FormatTime(this.Time) + " " + this.Text);
				}
				return arg_2E_0;
			}
		}
		public int GetIndex()
		{
			for (int i = 0; i < this.Log.Entries.get_Count(); i++)
			{
				if (this.Log.Entries.get_Item(i) == this)
				{
					return i;
				}
			}
			return -1;
		}
		public void DebugLog()
		{
			Debug.Log("Sent By: " + SkillUtility.GetPath(this.SentByState) + " : " + ((this.Action != null) ? this.Action.Name : "None (Action)"));
		}
	}
}
