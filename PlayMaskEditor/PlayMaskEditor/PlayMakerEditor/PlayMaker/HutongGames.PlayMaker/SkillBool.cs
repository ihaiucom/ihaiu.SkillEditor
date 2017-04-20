using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillBool : NamedVariable
	{
		[SerializeField]
		private bool value;
		public bool Value
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
				this.value = (bool)value;
			}
		}
		public override VariableType VariableType
		{
			get
			{
				return VariableType.Bool;
			}
		}
		public SkillBool()
		{
		}
		public SkillBool(string name) : base(name)
		{
		}
		public SkillBool(SkillBool source) : base(source)
		{
			if (source != null)
			{
				this.value = source.value;
			}
		}
		public override NamedVariable Clone()
		{
			return new SkillBool(this);
		}
		public override string ToString()
		{
			return this.value.ToString();
		}
		public static implicit operator SkillBool(bool value)
		{
			return new SkillBool(string.Empty)
			{
				value = value
			};
		}
	}
}
