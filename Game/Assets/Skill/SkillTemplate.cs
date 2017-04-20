using System;
using UnityEngine;

namespace ihaiu
{
    [Serializable]
    public class SkillTemplate : ScriptableObject
    {
        [SerializeField]
        private string category;
        [SerializeField]
        private string name;
        public Skill fsm;
        public string Category
        {
            get
            {
                return this.category;
            }
            set
            {
                this.category = value;
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

        public void OnEnable()
        {
            if (this.fsm != null)
            {
                this.fsm.UsedInTemplate = this;
            }
        }
    }
}