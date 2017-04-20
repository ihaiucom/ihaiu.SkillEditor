using System;
using System.Globalization;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillEnum : NamedVariable
	{
		[SerializeField]
		private string enumName;
		[SerializeField]
		private int intValue;
		private Enum value;
		private Type enumType;
		public override object RawValue
		{
			get
			{
				return this.Value;
			}
			set
			{
				this.Value = (Enum)value;
			}
		}
		public Type EnumType
		{
			get
			{
				if (object.ReferenceEquals(this.enumType, null) || this.enumType.get_IsAbstract() || !this.enumType.get_IsEnum())
				{
					this.InitEnumType();
				}
				return this.enumType;
			}
			set
			{
				if (object.ReferenceEquals(this.enumType, null) || this.enumType.get_IsAbstract() || !this.enumType.get_IsEnum())
				{
					this.InitEnumType();
				}
				if (!object.ReferenceEquals(this.enumType, value))
				{
					this.enumType = (value ?? typeof(None));
					this.enumName = this.enumType.get_FullName();
					this.InitEnumType();
					this.Value = (Enum)Activator.CreateInstance(this.enumType);
				}
			}
		}
		public string EnumName
		{
			get
			{
				return this.enumName;
			}
			set
			{
				this.enumName = value;
			}
		}
		public Enum Value
		{
			get
			{
				if (this.value == null)
				{
					this.value = (Enum)Enum.Parse(this.EnumType, this.intValue.ToString(CultureInfo.get_InvariantCulture()));
				}
				return this.value;
			}
			set
			{
				this.value = value;
				this.intValue = Convert.ToInt32(value);
			}
		}
		public override VariableType VariableType
		{
			get
			{
				return VariableType.Enum;
			}
		}
		public override Type ObjectType
		{
			get
			{
				return this.EnumType;
			}
			set
			{
				this.EnumType = value;
			}
		}
		private void InitEnumType()
		{
			this.enumType = ReflectionUtils.GetGlobalType(this.enumName);
			if (object.ReferenceEquals(this.enumType, null) || this.enumType.get_IsAbstract() || !this.enumType.get_IsEnum())
			{
				this.enumType = typeof(None);
				this.EnumName = this.enumType.get_FullName();
			}
		}
		public void ResetValue()
		{
			this.value = (Enum)Enum.Parse(this.EnumType, this.intValue.ToString(CultureInfo.get_InvariantCulture()));
		}
		public SkillEnum()
		{
		}
		public SkillEnum(string name, Type enumType, int intValue) : base(name)
		{
			this.EnumType = enumType;
			this.Value = (Enum)Enum.Parse(this.EnumType, intValue.ToString(CultureInfo.get_InvariantCulture()));
		}
		public SkillEnum(string name) : base(name)
		{
			this.enumName = typeof(Enum).get_FullName();
			this.enumType = typeof(Enum);
		}
		public SkillEnum(SkillEnum source) : base(source)
		{
			this.EnumType = source.EnumType;
			this.Value = source.Value;
		}
		public override NamedVariable Clone()
		{
			return new SkillEnum(this);
		}
		public override string ToString()
		{
			return this.Value.ToString();
		}
		public override bool TestTypeConstraint(VariableType variableType, Type _enumType = null)
		{
			return variableType == VariableType.Unknown || (base.TestTypeConstraint(variableType, this.enumType) && (object.ReferenceEquals(this.enumType, typeof(Enum)) || object.ReferenceEquals(_enumType, this.EnumType) || object.ReferenceEquals(_enumType, null)));
		}
		public static implicit operator SkillEnum(Enum value)
		{
			return new SkillEnum
			{
				Value = value
			};
		}
	}
}
