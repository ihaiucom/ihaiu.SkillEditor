using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillVector3 : NamedVariable
	{
		[SerializeField]
		private Vector3 value;
		public Vector3 Value
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
				this.value = (Vector3)value;
			}
		}
		public override VariableType VariableType
		{
			get
			{
				return VariableType.Vector3;
			}
		}
		public SkillVector3()
		{
		}
		public SkillVector3(string name) : base(name)
		{
		}
		public SkillVector3(SkillVector3 source) : base(source)
		{
			if (source != null)
			{
				this.value = source.value;
			}
		}
		public override NamedVariable Clone()
		{
			return new SkillVector3(this);
		}
		public override string ToString()
		{
			return this.value.ToString();
		}
		public static implicit operator SkillVector3(Vector3 value)
		{
			return new SkillVector3(string.Empty)
			{
				value = value
			};
		}
	}
}
