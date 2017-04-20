using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillString : NamedVariable
	{
		[SerializeField]
		private string value = "";
		public string Value
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
				this.value = (string)value;
			}
		}
		public override VariableType VariableType
		{
			get
			{
				return VariableType.String;
			}
		}
		public SkillString()
		{
		}
		public SkillString(string name) : base(name)
		{
		}
		public SkillString(SkillString source) : base(source)
		{
			if (source != null)
			{
				this.value = source.value;
			}
		}
		public override NamedVariable Clone()
		{
			return new SkillString(this);
		}
		public override string ToString()
		{
			return this.value;
		}
		public static implicit operator SkillString(string value)
		{
			return new SkillString(string.Empty)
			{
				value = value
			};
		}
		public static bool IsNullOrEmpty(SkillString fsmString)
		{
			return fsmString == null || fsmString.IsNone || string.IsNullOrEmpty(fsmString.value);
		}
	}
}
