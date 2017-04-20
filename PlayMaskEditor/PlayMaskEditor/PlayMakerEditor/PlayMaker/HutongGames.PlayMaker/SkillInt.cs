using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillInt : NamedVariable
	{
		[SerializeField]
		private int value;
		public int Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}
		public override object RawValue
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = (int)value;
			}
		}
		public override VariableType VariableType
		{
			get
			{
				return VariableType.Int;
			}
		}
		public override void SafeAssign(object val)
		{
			if (val is int)
			{
				this.value = (int)val;
			}
			if (val is float)
			{
				this.value = Mathf.FloorToInt((float)val);
			}
		}
		public SkillInt()
		{
		}
		public SkillInt(string name) : base(name)
		{
		}
		public SkillInt(SkillInt source) : base(source)
		{
			if (source != null)
			{
				this.value = source.value;
			}
		}
		public override NamedVariable Clone()
		{
			return new SkillInt(this);
		}
		public override string ToString()
		{
			return this.value.ToString();
		}
		public static implicit operator SkillInt(int value)
		{
			return new SkillInt(string.Empty)
			{
				value = value
			};
		}
	}
}
