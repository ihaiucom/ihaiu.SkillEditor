using HutongGames.PlayMaker;
using System;
using UnityEditor;
namespace HutongGames.PlayMakerEditor
{
	internal static class GameStateTracker
	{
		public static GameState CurrentState
		{
			get;
			private set;
		}
		public static GameState PreviousState
		{
			get;
			private set;
		}
		public static bool StateChanged
		{
			get
			{
				return GameStateTracker.CurrentState != GameStateTracker.PreviousState;
			}
		}
		public static void Update()
		{
			GameStateTracker.PreviousState = GameStateTracker.CurrentState;
			GameStateTracker.CurrentState = GameStateTracker.GetCurrentState();
		}
		private static GameState GetCurrentState()
		{
			if (EditorApplication.get_isPaused())
			{
				if (Skill.get_IsErrorBreak())
				{
					return GameState.Error;
				}
				if (Skill.get_IsBreak())
				{
					return GameState.Break;
				}
				return GameState.Paused;
			}
			else
			{
				if (!EditorApplication.get_isPlaying())
				{
					return GameState.Stopped;
				}
				return GameState.Running;
			}
		}
	}
}
