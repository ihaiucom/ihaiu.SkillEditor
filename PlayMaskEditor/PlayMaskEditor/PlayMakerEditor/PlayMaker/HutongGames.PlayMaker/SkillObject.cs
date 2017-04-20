using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillObject : NamedVariable
	{
		[SerializeField]
		private string typeName;
		[SerializeField]
		private Object value;
		private Type objectType;
		public override Type ObjectType
		{
			get
			{
				if (object.ReferenceEquals(this.objectType, null))
				{
					if (string.IsNullOrEmpty(this.typeName))
					{
						this.typeName = typeof(Object).get_FullName();
					}
					this.objectType = ReflectionUtils.GetGlobalType(this.typeName);
				}
				return this.objectType;
			}
			set
			{
				this.objectType = value;
				if (object.ReferenceEquals(this.objectType, null))
				{
					this.objectType = typeof(Object);
				}
				if (!object.ReferenceEquals(this.value, null))
				{
					Type type = this.value.GetType();
					if (!type.IsAssignableFrom(this.objectType) && !type.IsSubclassOf(this.objectType))
					{
						this.value = null;
					}
				}
				this.typeName = this.objectType.get_FullName();
			}
		}
		public string TypeName
		{
			get
			{
				return this.typeName;
			}
		}
		public Object Value
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
				this.value = (Object)value;
			}
		}
		public override VariableType VariableType
		{
			get
			{
				return VariableType.Object;
			}
		}
		public SkillObject()
		{
		}
		public SkillObject(string name) : base(name)
		{
			this.typeName = typeof(Object).get_FullName();
			this.objectType = typeof(Object);
		}
		public SkillObject(SkillObject source) : base(source)
		{
			this.value = source.value;
			this.typeName = source.typeName;
			this.objectType = source.objectType;
		}
		public override NamedVariable Clone()
		{
			return new SkillObject(this);
		}
		public override string ToString()
		{
			if (!(this.value == null))
			{
				return this.value.ToString();
			}
			return "None";
		}
		public static implicit operator SkillObject(Object value)
		{
			return new SkillObject
			{
				value = value
			};
		}
		public override bool TestTypeConstraint(VariableType variableType, Type _objectType = null)
		{
			return variableType == VariableType.Unknown || (base.TestTypeConstraint(variableType, this.objectType) && (object.ReferenceEquals(_objectType, null) || object.ReferenceEquals(_objectType, typeof(Object)) || object.ReferenceEquals(this.ObjectType, _objectType) || _objectType.IsAssignableFrom(this.ObjectType)));
		}
	}
}
