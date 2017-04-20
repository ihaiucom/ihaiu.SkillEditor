using System;
using System.Reflection;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillProperty
	{
		public SkillObject TargetObject = new SkillObject();
		public string TargetTypeName = "";
		public Type TargetType;
		public string PropertyName = "";
		public Type PropertyType;
		public SkillBool BoolParameter;
		public SkillFloat FloatParameter;
		public SkillInt IntParameter;
		public SkillGameObject GameObjectParameter;
		public SkillString StringParameter;
		public SkillVector2 Vector2Parameter;
		public SkillVector3 Vector3Parameter;
		public SkillRect RectParamater;
		public SkillQuaternion QuaternionParameter;
		public SkillObject ObjectParameter;
		public SkillMaterial MaterialParameter;
		public SkillTexture TextureParameter;
		public SkillColor ColorParameter;
		public SkillEnum EnumParameter;
		public SkillArray ArrayParameter;
		public bool setProperty;
		private bool initialized;
		[NonSerialized]
		private Object targetObjectCached;
		private MemberInfo[] memberInfo;
		public SkillProperty()
		{
			this.ResetParameters();
		}
		public SkillProperty(SkillProperty source)
		{
			this.setProperty = source.setProperty;
			this.TargetObject = new SkillObject(source.TargetObject);
			this.TargetTypeName = source.TargetTypeName;
			this.TargetType = source.TargetType;
			this.PropertyName = source.PropertyName;
			this.PropertyType = source.PropertyType;
			this.BoolParameter = new SkillBool(source.BoolParameter);
			this.FloatParameter = new SkillFloat(source.FloatParameter);
			this.IntParameter = new SkillInt(source.IntParameter);
			this.GameObjectParameter = new SkillGameObject(source.GameObjectParameter);
			this.StringParameter = new SkillString(source.StringParameter);
			this.Vector2Parameter = new SkillVector2(source.Vector2Parameter);
			this.Vector3Parameter = new SkillVector3(source.Vector3Parameter);
			this.RectParamater = new SkillRect(source.RectParamater);
			this.QuaternionParameter = new SkillQuaternion(source.QuaternionParameter);
			this.ObjectParameter = new SkillObject(source.ObjectParameter);
			this.MaterialParameter = new SkillMaterial(source.MaterialParameter);
			this.TextureParameter = new SkillTexture(source.TextureParameter);
			this.ColorParameter = new SkillColor(source.ColorParameter);
			this.EnumParameter = new SkillEnum(source.EnumParameter);
			this.ArrayParameter = new SkillArray(source.ArrayParameter);
		}
		public void SetVariable(NamedVariable variable)
		{
			if (variable == null)
			{
				this.ResetParameters();
				return;
			}
			switch (variable.VariableType)
			{
			case VariableType.Unknown:
				return;
			case VariableType.Float:
				this.FloatParameter = (variable as SkillFloat);
				return;
			case VariableType.Int:
				this.IntParameter = (variable as SkillInt);
				return;
			case VariableType.Bool:
				this.BoolParameter = (variable as SkillBool);
				return;
			case VariableType.GameObject:
				this.GameObjectParameter = (variable as SkillGameObject);
				return;
			case VariableType.String:
				this.StringParameter = (variable as SkillString);
				return;
			case VariableType.Vector2:
				this.Vector2Parameter = (variable as SkillVector2);
				return;
			case VariableType.Vector3:
				this.Vector3Parameter = (variable as SkillVector3);
				return;
			case VariableType.Color:
				this.ColorParameter = (variable as SkillColor);
				return;
			case VariableType.Rect:
				this.RectParamater = (variable as SkillRect);
				return;
			case VariableType.Material:
				this.MaterialParameter = (variable as SkillMaterial);
				return;
			case VariableType.Texture:
				this.TextureParameter = (variable as SkillTexture);
				return;
			case VariableType.Quaternion:
				this.QuaternionParameter = (variable as SkillQuaternion);
				return;
			case VariableType.Object:
				this.ObjectParameter = (variable as SkillObject);
				return;
			case VariableType.Array:
				this.ArrayParameter = (variable as SkillArray);
				return;
			case VariableType.Enum:
				this.EnumParameter = (variable as SkillEnum);
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		public NamedVariable GetVariable()
		{
			this.CheckForReinitialize();
			if (this.PropertyType.IsAssignableFrom(typeof(bool)))
			{
				return this.BoolParameter;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(int)))
			{
				return this.IntParameter;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(float)))
			{
				return this.FloatParameter;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(string)))
			{
				return this.StringParameter;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(Vector2)))
			{
				return this.Vector2Parameter;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(Vector3)))
			{
				return this.Vector3Parameter;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(Rect)))
			{
				return this.RectParamater;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(Quaternion)))
			{
				return this.QuaternionParameter;
			}
			if (object.ReferenceEquals(this.PropertyType, typeof(GameObject)))
			{
				return this.GameObjectParameter;
			}
			if (object.ReferenceEquals(this.PropertyType, typeof(Material)))
			{
				return this.MaterialParameter;
			}
			if (object.ReferenceEquals(this.PropertyType, typeof(Texture)))
			{
				return this.TextureParameter;
			}
			if (object.ReferenceEquals(this.PropertyType, typeof(Color)))
			{
				return this.ColorParameter;
			}
			if (this.PropertyType.IsSubclassOf(typeof(Object)))
			{
				return this.ObjectParameter;
			}
			if (this.PropertyType.get_IsArray())
			{
				return this.ArrayParameter;
			}
			if (this.PropertyType.get_IsEnum())
			{
				return this.EnumParameter;
			}
			return null;
		}
		public void SetPropertyName(string propertyName)
		{
			this.ResetParameters();
			this.PropertyName = propertyName;
			if (!string.IsNullOrEmpty(this.PropertyName))
			{
				if (!object.ReferenceEquals(this.TargetType, null))
				{
					this.PropertyType = ReflectionUtils.GetPropertyType(this.TargetType, this.PropertyName);
					if (this.TargetType.IsSubclassOf(typeof(Object)) && !object.ReferenceEquals(this.PropertyType, null))
					{
						this.ObjectParameter.ObjectType = this.PropertyType;
					}
					else
					{
						if (this.PropertyType.get_IsArray())
						{
							this.ArrayParameter.ElementType = SkillVar.GetVariableType(this.PropertyType.GetElementType());
						}
					}
				}
			}
			else
			{
				this.PropertyType = null;
			}
			this.Init();
		}
		public void SetValue()
		{
			this.CheckForReinitialize();
			if (this.targetObjectCached == null || this.memberInfo == null)
			{
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(bool)) && !this.BoolParameter.IsNone)
			{
				ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, this.BoolParameter.Value);
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(int)) && !this.IntParameter.IsNone)
			{
				ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, this.IntParameter.Value);
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(float)) && !this.FloatParameter.IsNone)
			{
				ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, this.FloatParameter.Value);
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(string)) && !this.StringParameter.IsNone)
			{
				ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, this.StringParameter.Value);
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(Vector2)) && !this.Vector2Parameter.IsNone)
			{
				ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, this.Vector2Parameter.Value);
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(Vector3)) && !this.Vector3Parameter.IsNone)
			{
				ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, this.Vector3Parameter.Value);
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(Rect)) && !this.RectParamater.IsNone)
			{
				ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, this.RectParamater.Value);
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(Quaternion)) && !this.QuaternionParameter.IsNone)
			{
				ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, this.QuaternionParameter.Value);
				return;
			}
			if (object.ReferenceEquals(this.PropertyType, typeof(GameObject)) && !this.GameObjectParameter.IsNone)
			{
				ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, this.GameObjectParameter.Value);
				return;
			}
			if (object.ReferenceEquals(this.PropertyType, typeof(Material)) && !this.MaterialParameter.IsNone)
			{
				ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, this.MaterialParameter.Value);
				return;
			}
			if (object.ReferenceEquals(this.PropertyType, typeof(Texture)) && !this.TextureParameter.IsNone)
			{
				ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, this.TextureParameter.Value);
				return;
			}
			if (object.ReferenceEquals(this.PropertyType, typeof(Color)) && !this.ColorParameter.IsNone)
			{
				ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, this.ColorParameter.Value);
				return;
			}
			if (this.PropertyType.IsSubclassOf(typeof(Object)) && !this.ObjectParameter.IsNone)
			{
				if (this.ObjectParameter.Value == null)
				{
					ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, null);
					return;
				}
				ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, this.ObjectParameter.Value);
				return;
			}
			else
			{
				if (this.PropertyType.get_IsArray() && !this.ArrayParameter.IsNone)
				{
					object[] values = this.ArrayParameter.Values;
					Array array = Array.CreateInstance(this.PropertyType.GetElementType(), values.Length);
					for (int i = 0; i < values.Length; i++)
					{
						array.SetValue(values[i], i);
					}
					ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, array);
					return;
				}
				if (this.PropertyType.get_IsEnum() && !this.EnumParameter.IsNone)
				{
					ReflectionUtils.SetMemberValue(this.memberInfo, this.targetObjectCached, this.EnumParameter.Value);
				}
				return;
			}
		}
		public void GetValue()
		{
			this.CheckForReinitialize();
			if (this.targetObjectCached == null || this.memberInfo == null)
			{
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(bool)))
			{
				this.BoolParameter.Value = (bool)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(int)))
			{
				this.IntParameter.Value = (int)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(float)))
			{
				this.FloatParameter.Value = (float)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(string)))
			{
				this.StringParameter.Value = (string)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(Vector2)))
			{
				this.Vector2Parameter.Value = (Vector2)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(Vector3)))
			{
				this.Vector3Parameter.Value = (Vector3)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(Rect)))
			{
				this.RectParamater.Value = (Rect)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
				return;
			}
			if (this.PropertyType.IsAssignableFrom(typeof(Quaternion)))
			{
				this.QuaternionParameter.Value = (Quaternion)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
				return;
			}
			if (object.ReferenceEquals(this.PropertyType, typeof(GameObject)))
			{
				this.GameObjectParameter.Value = (GameObject)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
				return;
			}
			if (object.ReferenceEquals(this.PropertyType, typeof(Material)))
			{
				this.MaterialParameter.Value = (Material)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
				return;
			}
			if (object.ReferenceEquals(this.PropertyType, typeof(Texture)))
			{
				this.TextureParameter.Value = (Texture)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
				return;
			}
			if (object.ReferenceEquals(this.PropertyType, typeof(Color)))
			{
				this.ColorParameter.Value = (Color)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
				return;
			}
			if (this.PropertyType.get_IsEnum())
			{
				this.EnumParameter.Value = (Enum)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
				return;
			}
			if (this.PropertyType.get_IsArray())
			{
				Array array = (Array)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
				object[] array2 = new object[array.get_Length()];
				for (int i = 0; i < array.get_Length(); i++)
				{
					array2[i] = array.GetValue(i);
				}
				this.ArrayParameter.Values = array2;
				return;
			}
			if (this.PropertyType.IsSubclassOf(typeof(Object)))
			{
				this.ObjectParameter.Value = (Object)ReflectionUtils.GetMemberValue(this.memberInfo, this.targetObjectCached);
			}
		}
		public void Init()
		{
			if (this.TargetObject == null)
			{
				return;
			}
			this.initialized = true;
			this.targetObjectCached = this.TargetObject.Value;
			if (this.TargetObject.UseVariable)
			{
				this.TargetTypeName = this.TargetObject.TypeName;
				this.TargetType = this.TargetObject.ObjectType;
			}
			else
			{
				if (this.TargetObject.Value != null)
				{
					this.TargetType = this.TargetObject.Value.GetType();
					this.TargetTypeName = this.TargetType.get_FullName();
				}
			}
			if (!string.IsNullOrEmpty(this.PropertyName))
			{
				this.memberInfo = ReflectionUtils.GetMemberInfo(this.TargetType, this.PropertyName);
				if (object.ReferenceEquals(this.memberInfo, null))
				{
					this.PropertyName = "";
					this.PropertyType = null;
					this.ResetParameters();
					return;
				}
				this.PropertyType = ReflectionUtils.GetMemberUnderlyingType(this.memberInfo[this.memberInfo.Length - 1]);
			}
			if (!object.ReferenceEquals(this.PropertyType, null) && this.PropertyType.get_IsEnum() && !SkillString.IsNullOrEmpty(this.StringParameter))
			{
				this.EnumParameter = new SkillEnum("")
				{
					EnumType = this.PropertyType,
					Value = (Enum)Enum.Parse(this.PropertyType, this.StringParameter.Value)
				};
				this.StringParameter.Value = null;
			}
		}
		public void CheckForReinitialize()
		{
			if (!this.initialized || this.targetObjectCached != this.TargetObject.Value || (this.TargetObject.UseVariable && !object.ReferenceEquals(this.TargetType, this.TargetObject.ObjectType)))
			{
				this.Init();
			}
		}
		public void ResetParameters()
		{
			this.BoolParameter = false;
			this.FloatParameter = 0f;
			this.IntParameter = 0;
			this.StringParameter = "";
			this.GameObjectParameter = new SkillGameObject("");
			this.Vector2Parameter = new SkillVector2();
			this.Vector3Parameter = new SkillVector3();
			this.RectParamater = new SkillRect();
			this.QuaternionParameter = new SkillQuaternion();
			this.ObjectParameter = new SkillObject();
			this.MaterialParameter = new SkillMaterial();
			this.TextureParameter = new SkillTexture();
			this.ColorParameter = new SkillColor();
			this.EnumParameter = new SkillEnum();
			this.ArrayParameter = new SkillArray();
		}
	}
}
