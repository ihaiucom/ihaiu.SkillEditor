using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillQuaternion : NamedVariable
	{
		[SerializeField]
		private Quaternion value;
		public Quaternion Value
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
				this.value = (Quaternion)value;
			}
		}
		public override VariableType VariableType
		{
			get
			{
				return VariableType.Quaternion;
			}
		}
		public SkillQuaternion()
		{
		}
		public SkillQuaternion(string name) : base(name)
		{
		}
		public SkillQuaternion(SkillQuaternion source) : base(source)
		{
			if (source != null)
			{
				this.value = source.value;
			}
		}
		public override NamedVariable Clone()
		{
			return new SkillQuaternion(this);
		}
		public override string ToString()
		{
			return this.value.get_eulerAngles().ToString();
		}
		public static implicit operator SkillQuaternion(Quaternion value)
		{
			return new SkillQuaternion(string.Empty)
			{
				value = value
			};
		}
	}
}
