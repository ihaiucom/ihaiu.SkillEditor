using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	[Serializable]
	public class SkillSelectionHistory
	{
		[Serializable]
		public class HistoryItem
		{
			[SerializeField]
			private int fsmComponentID;
			[SerializeField]
			private PlayMakerFSM fsmComponent;
			[SerializeField]
			private SkillTemplate fsmTemplate;
			[NonSerialized]
			private Skill subFsm;
			public GameObject GameObject
			{
				get
				{
					if (this.fsm == null)
					{
						return null;
					}
					return this.fsm.get_GameObject();
				}
			}
			public PlayMakerFSM FsmComponent
			{
				get
				{
					if (this.fsmComponent == null && this.fsmComponentID != 0)
					{
						this.fsmComponent = (EditorUtility.InstanceIDToObject(this.fsmComponentID) as PlayMakerFSM);
					}
					return this.fsmComponent;
				}
			}
			public Skill fsm
			{
				get
				{
					if (this.subFsm != null)
					{
						return this.subFsm;
					}
					if (this.FsmComponent != null)
					{
						return this.FsmComponent.get_Fsm();
					}
					if (!(this.fsmTemplate != null))
					{
						return null;
					}
					return this.fsmTemplate.fsm;
				}
			}
			public HistoryItem(Skill fsm)
			{
				if (fsm != null)
				{
					if (fsm.get_IsSubFsm())
					{
						this.subFsm = fsm;
						return;
					}
					this.fsmTemplate = fsm.get_UsedInTemplate();
					this.fsmComponent = (fsm.get_Owner() as PlayMakerFSM);
					this.fsmComponentID = ((this.fsmComponent != null) ? this.fsmComponent.GetInstanceID() : 0);
				}
			}
			public bool IsFor(Skill testFsm)
			{
				if (testFsm == null)
				{
					return false;
				}
				if (testFsm.get_IsSubFsm())
				{
					return testFsm == this.subFsm;
				}
				return (this.fsmTemplate != null && testFsm.get_UsedInTemplate() == this.fsmTemplate) || (this.FsmComponent != null && testFsm.get_Owner() == this.FsmComponent);
			}
		}
		[SerializeField]
		private List<SkillSelectionHistory.HistoryItem> backList = new List<SkillSelectionHistory.HistoryItem>();
		[SerializeField]
		private List<SkillSelectionHistory.HistoryItem> forwardList = new List<SkillSelectionHistory.HistoryItem>();
		[SerializeField]
		private List<SkillSelection> selectionCache = new List<SkillSelection>();
		[SerializeField]
		private List<SkillSelectionHistory.HistoryItem> recentlySelectedList = new List<SkillSelectionHistory.HistoryItem>();
		public int RecentlySelectedCount
		{
			get
			{
				return this.recentlySelectedList.get_Count();
			}
		}
		public List<Skill> GetRecentlySelectedFSMs()
		{
			List<Skill> list = new List<Skill>();
			using (List<SkillSelectionHistory.HistoryItem>.Enumerator enumerator = this.recentlySelectedList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillSelectionHistory.HistoryItem current = enumerator.get_Current();
					list.Add(current.fsm);
				}
			}
			return list;
		}
		public Skill GetFsmSelection(GameObject go)
		{
			if (go == null)
			{
				return null;
			}
			using (List<SkillSelectionHistory.HistoryItem>.Enumerator enumerator = this.recentlySelectedList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillSelectionHistory.HistoryItem current = enumerator.get_Current();
					if (current.fsm != null && current.fsm.get_GameObject() == go)
					{
						Skill fsm = current.fsm;
						return fsm;
					}
				}
			}
			PlayMakerFSM playMakerFSM = SkillSelection.FindFsmComponentOnGameObject(go);
			if (!(playMakerFSM != null))
			{
				return null;
			}
			return playMakerFSM.get_Fsm();
		}
		public SkillSelection SelectFsm(Skill fsm)
		{
			this.AddHistoryItem(fsm);
			return this.GetSelection(fsm);
		}
		public bool CanMoveBack()
		{
			return this.backList.get_Count() > 1;
		}
		public bool CanMoveForward()
		{
			return this.forwardList.get_Count() > 0;
		}
		public Skill MoveBack()
		{
			SkillSelectionHistory.HistoryItem historyItem = this.backList.get_Item(0);
			this.backList.RemoveAt(0);
			this.forwardList.Insert(0, historyItem);
			return this.backList.get_Item(0).fsm;
		}
		public Skill MoveForward()
		{
			SkillSelectionHistory.HistoryItem historyItem = this.forwardList.get_Item(0);
			this.forwardList.RemoveAt(0);
			this.backList.Insert(0, historyItem);
			return historyItem.fsm;
		}
		public void SanityCheck()
		{
			using (List<SkillSelection>.Enumerator enumerator = this.selectionCache.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillSelection current = enumerator.get_Current();
					current.SanityCheck();
				}
			}
			this.selectionCache.RemoveAll((SkillSelection r) => r.IsOrphaned);
			this.backList.RemoveAll((SkillSelectionHistory.HistoryItem r) => r.fsm == null);
			this.forwardList.RemoveAll((SkillSelectionHistory.HistoryItem r) => r.fsm == null);
			this.recentlySelectedList.RemoveAll((SkillSelectionHistory.HistoryItem r) => r.fsm == null);
		}
		public void Clear()
		{
			this.backList.Clear();
			this.forwardList.Clear();
			this.selectionCache.Clear();
			this.recentlySelectedList.Clear();
		}
		public void DebugGUI()
		{
			GUILayout.Label("Back List: ", EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			using (List<SkillSelectionHistory.HistoryItem>.Enumerator enumerator = this.backList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillSelectionHistory.HistoryItem current = enumerator.get_Current();
					GUILayout.Label(Labels.GetFullFsmLabelWithInstanceID(current.fsm), new GUILayoutOption[0]);
				}
			}
			GUILayout.Label("Forward List: ", EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			using (List<SkillSelectionHistory.HistoryItem>.Enumerator enumerator2 = this.forwardList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					SkillSelectionHistory.HistoryItem current2 = enumerator2.get_Current();
					GUILayout.Label(Labels.GetFullFsmLabelWithInstanceID(current2.fsm), new GUILayoutOption[0]);
				}
			}
			GUILayout.Label("RecentlySelectedList: ", EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			using (List<SkillSelectionHistory.HistoryItem>.Enumerator enumerator3 = this.recentlySelectedList.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					SkillSelectionHistory.HistoryItem current3 = enumerator3.get_Current();
					GUILayout.Label(Labels.GetFullFsmLabelWithInstanceID(current3.fsm), new GUILayoutOption[0]);
				}
			}
			GUILayout.Label("SelectionCache: ", EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
			using (List<SkillSelection>.Enumerator enumerator4 = this.selectionCache.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					SkillSelection current4 = enumerator4.get_Current();
					GUILayout.Label(Labels.GetFullFsmLabelWithInstanceID(current4.ActiveFsm), new GUILayoutOption[0]);
				}
			}
		}
		private SkillSelection GetSelection(Skill fsm)
		{
			if (fsm == null)
			{
				return SkillSelection.None;
			}
			using (List<SkillSelection>.Enumerator enumerator = this.selectionCache.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillSelection current = enumerator.get_Current();
					if (current.IsFor(fsm))
					{
						return current;
					}
				}
			}
			SkillSelection fsmSelection = new SkillSelection(fsm);
			this.selectionCache.Add(fsmSelection);
			return fsmSelection;
		}
		private void AddHistoryItem(Skill fsm)
		{
			if (fsm == null || (this.backList.get_Count() > 0 && this.backList.get_Item(0).IsFor(fsm)))
			{
				return;
			}
			this.forwardList.Clear();
			this.backList.Insert(0, new SkillSelectionHistory.HistoryItem(fsm));
			this.recentlySelectedList.RemoveAll((SkillSelectionHistory.HistoryItem r) => r.fsm == fsm);
			this.recentlySelectedList.Insert(0, new SkillSelectionHistory.HistoryItem(fsm));
		}
	}
}
