using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillTexture : SkillObject
	{
		public new Texture Value
		{
			get
			{
				return base.Value as Texture;
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
				return VariableType.Texture;
			}
		}
		public SkillTexture()
		{
		}
		public SkillTexture(string name) : base(name)
		{
		}
		public SkillTexture(SkillObject source) : base(source)
		{
		}
		public override NamedVariable Clone()
		{
			return new SkillTexture(this);
		}
		public override bool TestTypeConstraint(VariableType variableType, Type _objectType = null)
		{
			return variableType == VariableType.Unknown || variableType == VariableType.Texture;
		}
	}
}
