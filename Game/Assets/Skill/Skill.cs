using UnityEngine;
using System.Collections;
using System;

namespace ihaiu
{
    [Serializable]
    public class Skill : INameable, IComparable
    {

        [SerializeField]
        private SkillTemplate usedInTemplate;
        [SerializeField]
        private string name = "Skill";
        [NonSerialized]
        private string owner = "";

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

        public string OwnerName
        {
            get
            {
                return owner;
            }

            set
            {
                this.owner = value;
            }
        }

        private string GuiLabel
        {
            get
            {
                return this.OwnerName + " : " + this.Name;
            }
        }
       

        public SkillTemplate UsedInTemplate
        {
            get
            {
                return this.usedInTemplate;
            }
            set
            {
                this.usedInTemplate = value;
            }
        }


        public bool IsModifiedPrefabInstance
        {
            get;
            set;
        }

        public int CompareTo(object obj)
        {
            Skill fsm = obj as Skill;
            if (fsm != null)
            {
                return this.GuiLabel.CompareTo(fsm.GuiLabel);
            }
            return 0;
        }
    }
}
