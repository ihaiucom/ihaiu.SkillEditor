using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillRect : NamedVariable
	{
		[SerializeField]
		private Rect value;
		public Rect Value
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
				this.value = (Rect)value;
			}
		}
		public override VariableType VariableType
		{
			get
			{
				return VariableType.Rect;
			}
		}
		public SkillRect()
		{
		}
		public SkillRect(string name) : base(name)
		{
		}
		public SkillRect(SkillRect source) : base(source)
		{
			if (source != null)
			{
				this.value = source.value;
			}
		}
		public override NamedVariable Clone()
		{
			return new SkillRect(this);
		}
		public override string ToString()
		{
			return this.value.ToString();
		}
		public static implicit operator SkillRect(Rect value)
		{
			return new SkillRect(string.Empty)
			{
				value = value
			};
		}
	}
}
