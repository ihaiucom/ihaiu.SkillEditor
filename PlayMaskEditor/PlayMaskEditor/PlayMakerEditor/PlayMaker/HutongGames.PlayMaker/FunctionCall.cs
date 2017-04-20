using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class FunctionCall
	{
		public string FunctionName = "";
		[SerializeField]
		private string parameterType;
		public SkillBool BoolParameter;
		public SkillFloat FloatParameter;
		public SkillInt IntParameter;
		public SkillGameObject GameObjectParameter;
		public SkillObject ObjectParameter;
		public SkillString StringParameter;
		public SkillVector2 Vector2Parameter;
		public SkillVector3 Vector3Parameter;
		public SkillRect RectParamater;
		public SkillQuaternion QuaternionParameter;
		public SkillMaterial MaterialParameter;
		public SkillTexture TextureParameter;
		public SkillColor ColorParameter;
		public SkillEnum EnumParameter;
		public SkillArray ArrayParameter;
		public string ParameterType
		{
			get
			{
				return this.parameterType;
			}
			set
			{
				this.parameterType = value;
			}
		}
		public FunctionCall()
		{
			this.ResetParameters();
		}
		public FunctionCall(FunctionCall source)
		{
			this.FunctionName = source.FunctionName;
			this.parameterType = source.parameterType;
			this.BoolParameter = new SkillBool(source.BoolParameter);
			this.FloatParameter = new SkillFloat(source.FloatParameter);
			this.IntParameter = new SkillInt(source.IntParameter);
			this.GameObjectParameter = new SkillGameObject(source.GameObjectParameter);
			this.ObjectParameter = source.ObjectParameter;
			this.StringParameter = new SkillString(source.StringParameter);
			this.Vector2Parameter = new SkillVector2(source.Vector2Parameter);
			this.Vector3Parameter = new SkillVector3(source.Vector3Parameter);
			this.RectParamater = new SkillRect(source.RectParamater);
			this.QuaternionParameter = new SkillQuaternion(source.QuaternionParameter);
			this.MaterialParameter = new SkillMaterial(source.MaterialParameter);
			this.TextureParameter = new SkillTexture(source.TextureParameter);
			this.ColorParameter = new SkillColor(source.ColorParameter);
			this.EnumParameter = new SkillEnum(source.EnumParameter);
			this.ArrayParameter = new SkillArray(source.ArrayParameter);
		}
		public void ResetParameters()
		{
			this.BoolParameter = false;
			this.FloatParameter = 0f;
			this.IntParameter = 0;
			this.StringParameter = "";
			this.GameObjectParameter = new SkillGameObject("");
			this.ObjectParameter = null;
			this.Vector2Parameter = new SkillVector2();
			this.Vector3Parameter = new SkillVector3();
			this.RectParamater = new SkillRect();
			this.QuaternionParameter = new SkillQuaternion();
			this.MaterialParameter = new SkillMaterial();
			this.TextureParameter = new SkillTexture();
			this.ColorParameter = new SkillColor();
			this.EnumParameter = new SkillEnum();
			this.ArrayParameter = new SkillArray();
		}
	}
}
