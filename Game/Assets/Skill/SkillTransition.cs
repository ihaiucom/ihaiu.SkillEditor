using System;
using UnityEngine;
namespace ihaiu
{
    [Serializable]
    public class SkillTransition : IEquatable<SkillTransition>
    {
        public enum CustomLinkStyle : byte
        {
            Default,
            Bezier,
            Circuit
        }
        public enum CustomLinkConstraint : byte
        {
            None,
            LockLeft,
            LockRight
        }
        [SerializeField]
        private SkillEvent fsmEvent;
        [SerializeField]
        private string toState;
        [SerializeField]
        private SkillTransition.CustomLinkStyle linkStyle;
        [SerializeField]
        private SkillTransition.CustomLinkConstraint linkConstraint;
        [SerializeField]
        private byte colorIndex;
        public SkillEvent FsmEvent
        {
            get
            {
                return this.fsmEvent;
            }
            set
            {
                this.fsmEvent = value;
            }
        }
        public string ToState
        {
            get
            {
                return this.toState;
            }
            set
            {
                this.toState = value;
            }
        }
        public SkillTransition.CustomLinkStyle LinkStyle
        {
            get
            {
                return this.linkStyle;
            }
            set
            {
                this.linkStyle = value;
            }
        }
        public SkillTransition.CustomLinkConstraint LinkConstraint
        {
            get
            {
                return this.linkConstraint;
            }
            set
            {
                this.linkConstraint = value;
            }
        }
        public int ColorIndex
        {
            get
            {
                return (int)this.colorIndex;
            }
            set
            {
                this.colorIndex = (byte)Mathf.Clamp(value, 0, PlayMakerPrefs.Colors.Length - 1);
            }
        }
        public string EventName
        {
            get
            {
                if (!SkillEvent.IsNullOrEmpty(this.fsmEvent))
                {
                    return this.fsmEvent.Name;
                }
                return string.Empty;
            }
        }
        public SkillTransition()
        {
        }
        public SkillTransition(SkillTransition source)
        {
            this.fsmEvent = source.FsmEvent;
            this.toState = source.toState;
            this.linkStyle = source.linkStyle;
            this.linkConstraint = source.linkConstraint;
            this.colorIndex = source.colorIndex;
        }
        public bool Equals(SkillTransition other)
        {
            return !object.ReferenceEquals(other, null) && (object.ReferenceEquals(this, other) || (!(other.toState != this.toState) && other.EventName == this.EventName));
        }
    }
}
