using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ihaiu
{
    [Localizable(false)]
    [Serializable]
    public class SkillSelectionHistory 
    {
        [Serializable]
        public class HistoryItem
        {
            
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
                return this.recentlySelectedList.Count;
            }
        }
    }
}
