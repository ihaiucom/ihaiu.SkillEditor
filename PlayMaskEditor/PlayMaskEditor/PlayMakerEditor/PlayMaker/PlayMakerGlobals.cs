using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayMakerGlobals : ScriptableObject
{
	private static PlayMakerGlobals instance;
	[SerializeField]
	private SkillVariables variables = new SkillVariables();
	[SerializeField]
	private List<string> events = new List<string>();
	public static bool Initialized
	{
		get;
		private set;
	}
	public static bool IsPlayingInEditor
	{
		get;
		private set;
	}
	public static bool IsPlaying
	{
		get;
		private set;
	}
	public static bool IsEditor
	{
		get;
		private set;
	}
	public static bool IsBuilding
	{
		get;
		set;
	}
	public static PlayMakerGlobals Instance
	{
		get
		{
			PlayMakerGlobals.Initialize();
			return PlayMakerGlobals.instance;
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
	public List<string> Events
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
	public static void InitApplicationFlags()
	{
		PlayMakerGlobals.IsPlayingInEditor = (Application.get_isEditor() && Application.get_isPlaying());
		PlayMakerGlobals.IsPlaying = (Application.get_isPlaying() || PlayMakerGlobals.IsBuilding);
		PlayMakerGlobals.IsEditor = Application.get_isEditor();
	}
	public static void Initialize()
	{
		if (!PlayMakerGlobals.Initialized)
		{
			PlayMakerGlobals.InitApplicationFlags();
			Object @object = Resources.Load("PlayMakerGlobals", typeof(PlayMakerGlobals));
			if (@object != null)
			{
				if (PlayMakerGlobals.IsPlayingInEditor)
				{
					PlayMakerGlobals playMakerGlobals = (PlayMakerGlobals)@object;
					PlayMakerGlobals.instance = ScriptableObject.CreateInstance<PlayMakerGlobals>();
					PlayMakerGlobals.instance.Variables = new SkillVariables(playMakerGlobals.variables);
					PlayMakerGlobals.instance.Events = new List<string>(playMakerGlobals.Events);
				}
				else
				{
					PlayMakerGlobals.instance = (@object as PlayMakerGlobals);
				}
			}
			else
			{
				PlayMakerGlobals.instance = ScriptableObject.CreateInstance<PlayMakerGlobals>();
			}
			PlayMakerGlobals.Initialized = true;
		}
	}
	public static void ResetInstance()
	{
		PlayMakerGlobals.instance = null;
	}
	public SkillEvent AddEvent(string eventName)
	{
		this.events.Add(eventName);
		SkillEvent fsmEvent = SkillEvent.FindEvent(eventName) ?? SkillEvent.GetFsmEvent(eventName);
		fsmEvent.IsGlobal = true;
		return fsmEvent;
	}
	public void OnEnable()
	{
	}
	public void OnDestroy()
	{
		PlayMakerGlobals.Initialized = false;
		PlayMakerGlobals.instance = null;
	}
}
