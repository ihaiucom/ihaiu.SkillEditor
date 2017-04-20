using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillVector2 : NamedVariable
	{
		[SerializeField]
		private Vector2 value;
		public Vector2 Value
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
				this.value = (Vector2)value;
			}
		}
		public override VariableType VariableType
		{
			get
			{
				return VariableType.Vector2;
			}
		}
		public SkillVector2()
		{
		}
		public SkillVector2(string name) : base(name)
		{
		}
		public SkillVector2(SkillVector2 source) : base(source)
		{
			if (source != null)
			{
				this.value = source.value;
			}
		}
		public override NamedVariable Clone()
		{
			return new SkillVector2(this);
		}
		public override string ToString()
		{
			return this.value.ToString();
		}
		public static implicit operator SkillVector2(Vector2 value)
		{
			return new SkillVector2(string.Empty)
			{
				value = value
			};
		}
	}
}
