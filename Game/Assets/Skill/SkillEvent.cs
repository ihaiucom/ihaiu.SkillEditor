using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace ihaiu
{
    [Serializable]
    public class SkillEvent : IComparable, INameable
    {
        [SerializeField]
        private string name;
       
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

        int IComparable.CompareTo(object obj)
        {
            return 0;
        }


        public static bool IsNullOrEmpty(SkillEvent fsmEvent)
        {
            return fsmEvent == null || string.IsNullOrEmpty(fsmEvent.name);
        }
       
    }
}
