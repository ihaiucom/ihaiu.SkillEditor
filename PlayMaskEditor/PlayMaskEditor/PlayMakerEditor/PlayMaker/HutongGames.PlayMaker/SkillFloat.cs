using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillFloat : NamedVariable
	{
		[SerializeField]
		private float value;
		public float Value
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
				this.value = (float)value;
			}
		}
		public override VariableType VariableType
		{
			get
			{
				return VariableType.Float;
			}
		}
		public override void SafeAssign(object val)
		{
			if (val is float)
			{
				this.value = (float)val;
			}
			if (val is int)
			{
				this.value = (float)((int)val);
			}
		}
		public SkillFloat()
		{
		}
		public SkillFloat(string name) : base(name)
		{
		}
		public SkillFloat(SkillFloat source) : base(source)
		{
			if (source != null)
			{
				this.value = source.value;
			}
		}
		public override NamedVariable Clone()
		{
			return new SkillFloat(this);
		}
		public override string ToString()
		{
			return this.value.ToString();
		}
		public static implicit operator SkillFloat(float value)
		{
			return new SkillFloat(string.Empty)
			{
				value = value
			};
		}
	}
}
