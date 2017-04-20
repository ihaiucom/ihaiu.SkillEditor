using HutongGames.PlayMaker;
using System;
using UnityEngine;
[ExecuteInEditMode]
public class PlayMakerOnGUI : MonoBehaviour
{
	public PlayMakerFSM playMakerFSM;
	public bool previewInEditMode = true;
	public void Start()
	{
		if (this.playMakerFSM != null)
		{
			this.playMakerFSM.Fsm.HandleOnGUI = true;
		}
	}
	public void OnGUI()
	{
		if (this.previewInEditMode && !Application.get_isPlaying())
		{
			PlayMakerOnGUI.DoEditGUI();
			return;
		}
		if (this.playMakerFSM != null && this.playMakerFSM.Fsm != null && this.playMakerFSM.Fsm.HandleOnGUI)
		{
			this.playMakerFSM.Fsm.OnGUI();
		}
	}
	private static void DoEditGUI()
	{
		if (PlayMakerGUI.SelectedFSM != null)
		{
			SkillState editState = PlayMakerGUI.SelectedFSM.EditState;
			if (editState != null && editState.IsInitialized)
			{
				SkillStateAction[] actions = editState.Actions;
				for (int i = 0; i < actions.Length; i++)
				{
					SkillStateAction fsmStateAction = actions[i];
					if (fsmStateAction.Active)
					{
						fsmStateAction.OnGUI();
					}
				}
			}
		}
	}
}
