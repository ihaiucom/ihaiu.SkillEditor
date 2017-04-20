using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillMaterial : SkillObject
	{
		public new Material Value
		{
			get
			{
				return base.Value as Material;
			}
			set
			{
				base.Value = value;
			}
		}
		public override VariableType VariableType
		{
			get
			{
				return VariableType.Material;
			}
		}
		public SkillMaterial()
		{
		}
		public SkillMaterial(string name) : base(name)
		{
		}
		public SkillMaterial(SkillObject source) : base(source)
		{
		}
		public override NamedVariable Clone()
		{
			return new SkillMaterial(this);
		}
		public override bool TestTypeConstraint(VariableType variableType, Type _objectType = null)
		{
			return variableType == VariableType.Unknown || variableType == VariableType.Material;
		}
	}
}
