using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillColor : NamedVariable
	{
		[SerializeField]
		private Color value = Color.get_black();
		public Color Value
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
				this.value = (Color)value;
			}
		}
		public override VariableType VariableType
		{
			get
			{
				return VariableType.Color;
			}
		}
		public SkillColor()
		{
		}
		public SkillColor(string name) : base(name)
		{
		}
		public SkillColor(SkillColor source) : base(source)
		{
			if (source != null)
			{
				this.value = source.value;
			}
		}
		public override NamedVariable Clone()
		{
			return new SkillColor(this);
		}
		public override string ToString()
		{
			return this.value.ToString();
		}
		public static implicit operator SkillColor(Color value)
		{
			return new SkillColor(string.Empty)
			{
				value = value
			};
		}
	}
}
