using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillVar
	{
		public string variableName;
		public string objectType;
		public bool useVariable;
		[NonSerialized]
		private NamedVariable namedVar;
		[NonSerialized]
		private Type namedVarType;
		private Type enumType;
		private Enum enumValue;
		[NonSerialized]
		private Type _objectType;
		[SerializeField]
		private VariableType type = VariableType.Unknown;
		public float floatValue;
		public int intValue;
		public bool boolValue;
		public string stringValue;
		public Vector4 vector4Value;
		public Object objectReference;
		public SkillArray arrayValue;
		private Vector2 vector2 = default(Vector2);
		private Vector3 vector3 = default(Vector3);
		private Rect rect = default(Rect);
		public NamedVariable NamedVar
		{
			get
			{
				if (this.namedVar == null)
				{
					this.InitNamedVar();
				}
				return this.namedVar;
			}
			set
			{
				if (value != null)
				{
					this.UpdateType(value);
					this.namedVar = value;
					this.namedVarType = value.GetType();
					this.variableName = this.namedVar.Name;
					this.useVariable = value.UseVariable;
				}
				else
				{
					this.namedVar = null;
					this.namedVarType = null;
					this.variableName = null;
				}
				this.UpdateValue();
			}
		}
		public Type NamedVarType
		{
			get
			{
				if (object.ReferenceEquals(this.namedVarType, null) && this.NamedVar != null)
				{
					this.namedVarType = this.NamedVar.GetType();
				}
				return this.namedVarType;
			}
		}
		public Type EnumType
		{
			get
			{
				if (object.ReferenceEquals(this.enumType, null))
				{
					this.InitEnumType();
				}
				return this.enumType;
			}
			set
			{
				if (!object.ReferenceEquals(this.enumType, value))
				{
					this.enumType = (value ?? typeof(None));
					this.ObjectType = this.enumType;
					this.intValue = 0;
					this.enumValue = null;
				}
				SkillEnum fsmEnum = this.NamedVar as SkillEnum;
				if (fsmEnum != null)
				{
					fsmEnum.EnumType = this.enumType;
				}
				SkillArray fsmArray = this.NamedVar as SkillArray;
				if (fsmArray != null)
				{
					fsmArray.ObjectType = this.enumType;
				}
			}
		}
		public Enum EnumValue
		{
			get
			{
				if (this.enumValue == null)
				{
					this.enumValue = (Enum)Enum.ToObject(this.EnumType, this.intValue);
				}
				return this.enumValue;
			}
			set
			{
				if (value != null)
				{
					this.EnumType = value.GetType();
					this.enumValue = value;
					this.intValue = Convert.ToInt32(value);
					return;
				}
				this.enumValue = (Enum)Activator.CreateInstance(this.EnumType);
			}
		}
		public Type ObjectType
		{
			get
			{
				if (object.ReferenceEquals(this._objectType, null))
				{
					this._objectType = ReflectionUtils.GetGlobalType(this.objectType);
				}
				if (object.ReferenceEquals(this._objectType, null))
				{
					this._objectType = typeof(Object);
					this.objectType = this._objectType.get_FullName();
				}
				return this._objectType;
			}
			set
			{
				this._objectType = value;
				if (!object.ReferenceEquals(this._objectType, null))
				{
					this.objectType = this._objectType.get_FullName();
				}
				else
				{
					this._objectType = typeof(Object);
					this.objectType = this._objectType.get_FullName();
				}
				if (this.namedVar != null)
				{
					this.NamedVar.ObjectType = this._objectType;
				}
			}
		}
		public VariableType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				if (value != this.type)
				{
					this.type = value;
					this.InitNamedVar();
				}
			}
		}
		public Type RealType
		{
			get
			{
				switch (this.type)
				{
				case VariableType.Unknown:
					return null;
				case VariableType.Float:
					return typeof(float);
				case VariableType.Int:
					return typeof(int);
				case VariableType.Bool:
					return typeof(bool);
				case VariableType.GameObject:
					return typeof(GameObject);
				case VariableType.String:
					return typeof(string);
				case VariableType.Vector2:
					return typeof(Vector2);
				case VariableType.Vector3:
					return typeof(Vector3);
				case VariableType.Color:
					return typeof(Color);
				case VariableType.Rect:
					return typeof(Rect);
				case VariableType.Material:
					return typeof(Material);
				case VariableType.Texture:
					return typeof(Texture);
				case VariableType.Quaternion:
					return typeof(Quaternion);
				case VariableType.Object:
					return this.ObjectType;
				case VariableType.Array:
					return this.arrayValue.RealType();
				case VariableType.Enum:
					return this.EnumType;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}
		public bool IsNone
		{
			get
			{
				return this.useVariable && string.IsNullOrEmpty(this.variableName);
			}
		}
		public Vector2 vector2Value
		{
			get
			{
				this.vector2.Set(this.vector4Value.x, this.vector4Value.y);
				return this.vector2;
			}
			set
			{
				this.vector4Value.Set(value.x, value.y, 0f, 0f);
			}
		}
		public Vector3 vector3Value
		{
			get
			{
				this.vector3.Set(this.vector4Value.x, this.vector4Value.y, this.vector4Value.z);
				return this.vector3;
			}
			set
			{
				this.vector4Value.Set(value.x, value.y, value.z, 0f);
			}
		}
		public Color colorValue
		{
			get
			{
				return new Color(this.vector4Value.x, this.vector4Value.y, this.vector4Value.z, this.vector4Value.w);
			}
			set
			{
				this.vector4Value.Set(value.r, value.g, value.b, value.a);
			}
		}
		public Rect rectValue
		{
			get
			{
				this.rect.Set(this.vector4Value.x, this.vector4Value.y, this.vector4Value.z, this.vector4Value.w);
				return this.rect;
			}
			set
			{
				this.vector4Value.Set(value.get_x(), value.get_y(), value.get_width(), value.get_height());
			}
		}
		public Quaternion quaternionValue
		{
			get
			{
				return new Quaternion(this.vector4Value.x, this.vector4Value.y, this.vector4Value.z, this.vector4Value.w);
			}
			set
			{
				this.vector4Value.Set(value.x, value.y, value.z, value.w);
			}
		}
		public GameObject gameObjectValue
		{
			get
			{
				return this.objectReference as GameObject;
			}
			set
			{
				this.objectReference = value;
			}
		}
		public Material materialValue
		{
			get
			{
				return this.objectReference as Material;
			}
			set
			{
				this.objectReference = value;
			}
		}
		public Texture textureValue
		{
			get
			{
				return this.objectReference as Texture;
			}
			set
			{
				this.objectReference = value;
			}
		}
		public SkillVar()
		{
		}
		public SkillVar(Type type)
		{
			this.type = SkillVar.GetVariableType(type);
			if (type.get_IsEnum())
			{
				this.EnumType = type;
				return;
			}
			if (type.get_IsArray())
			{
				Type elementType = type.GetElementType();
				this.arrayValue = new SkillArray
				{
					ElementType = SkillVar.GetVariableType(elementType)
				};
				if (elementType.get_IsEnum() || typeof(Object).IsAssignableFrom(elementType))
				{
					this.arrayValue.ObjectType = elementType;
					return;
				}
			}
			else
			{
				if (type.IsSubclassOf(typeof(Object)))
				{
					this.ObjectType = type;
				}
			}
		}
		public SkillVar(SkillVar source)
		{
			this.variableName = source.variableName;
			this.useVariable = source.useVariable;
			this.type = source.type;
			this.GetValueFrom(source.NamedVar);
		}
		public SkillVar(INamedVariable variable)
		{
			this.type = variable.VariableType;
			this.ObjectType = variable.ObjectType;
			this.variableName = variable.Name;
			this.GetValueFrom(variable);
		}
		public void Init(NamedVariable variable)
		{
			if (variable != null)
			{
				this.type = variable.VariableType;
				this.variableName = variable.Name;
			}
			else
			{
				this.variableName = "";
			}
			this.NamedVar = variable;
		}
		private void UpdateType(INamedVariable sourceVar)
		{
			if (sourceVar == null)
			{
				this.Type = VariableType.Unknown;
				return;
			}
			this.Type = sourceVar.VariableType;
			this.ObjectType = sourceVar.ObjectType;
		}
		private void InitNamedVar()
		{
			switch (this.type)
			{
			case VariableType.Unknown:
				this.namedVar = null;
				this.namedVarType = null;
				return;
			case VariableType.Float:
				this.namedVar = new SkillFloat(this.variableName)
				{
					Value = this.floatValue
				};
				break;
			case VariableType.Int:
				this.namedVar = new SkillInt(this.variableName)
				{
					Value = this.intValue
				};
				break;
			case VariableType.Bool:
				this.namedVar = new SkillBool(this.variableName)
				{
					Value = this.boolValue
				};
				break;
			case VariableType.GameObject:
				this.namedVar = new SkillGameObject(this.variableName)
				{
					Value = this.gameObjectValue
				};
				break;
			case VariableType.String:
				this.namedVar = new SkillString(this.variableName)
				{
					Value = this.stringValue
				};
				break;
			case VariableType.Vector2:
				this.namedVar = new SkillVector2(this.variableName)
				{
					Value = this.vector2Value
				};
				break;
			case VariableType.Vector3:
				this.namedVar = new SkillVector3(this.variableName)
				{
					Value = this.vector3Value
				};
				break;
			case VariableType.Color:
				this.namedVar = new SkillColor(this.variableName)
				{
					Value = this.colorValue
				};
				break;
			case VariableType.Rect:
				this.namedVar = new SkillRect(this.variableName)
				{
					Value = this.rectValue
				};
				break;
			case VariableType.Material:
				this.namedVar = new SkillMaterial(this.variableName)
				{
					Value = this.materialValue
				};
				break;
			case VariableType.Texture:
				this.namedVar = new SkillTexture(this.variableName)
				{
					Value = this.textureValue
				};
				break;
			case VariableType.Quaternion:
				this.namedVar = new SkillQuaternion(this.variableName)
				{
					Value = this.quaternionValue
				};
				break;
			case VariableType.Object:
				this.namedVar = new SkillObject(this.variableName)
				{
					ObjectType = this.ObjectType,
					Value = this.objectReference
				};
				break;
			case VariableType.Array:
			{
				SkillArray fsmArray = new SkillArray(this.variableName)
				{
					ElementType = this.arrayValue.ElementType,
					ObjectType = this.arrayValue.ObjectType
				};
				fsmArray.CopyValues(this.arrayValue);
				fsmArray.SaveChanges();
				this.namedVar = fsmArray;
				break;
			}
			case VariableType.Enum:
				this.namedVar = new SkillEnum(this.variableName)
				{
					EnumType = this.EnumType,
					Value = this.EnumValue
				};
				break;
			default:
				throw new ArgumentOutOfRangeException("Type");
			}
			if (this.namedVar != null)
			{
				this.namedVarType = this.namedVar.GetType();
				this.namedVar.UseVariable = this.useVariable;
			}
		}
		private void InitEnumType()
		{
			this.enumType = ReflectionUtils.GetGlobalType(this.objectType);
			if (object.ReferenceEquals(this.enumType, null) || this.enumType.get_IsAbstract() || !this.enumType.get_IsEnum())
			{
				this.enumType = typeof(None);
				this.objectType = this.enumType.get_FullName();
			}
		}
		public object GetValue()
		{
			if (this.namedVar == null)
			{
				this.InitNamedVar();
			}
			switch (this.type)
			{
			case VariableType.Unknown:
				return null;
			case VariableType.Float:
				return this.floatValue;
			case VariableType.Int:
				return this.intValue;
			case VariableType.Bool:
				return this.boolValue;
			case VariableType.GameObject:
				return this.gameObjectValue;
			case VariableType.String:
				return this.stringValue;
			case VariableType.Vector2:
				return this.vector2Value;
			case VariableType.Vector3:
				return this.vector3Value;
			case VariableType.Color:
				return this.colorValue;
			case VariableType.Rect:
				return this.rectValue;
			case VariableType.Material:
				return this.materialValue;
			case VariableType.Texture:
				return this.textureValue;
			case VariableType.Quaternion:
				return this.quaternionValue;
			case VariableType.Object:
				return this.objectReference;
			case VariableType.Array:
				return this.arrayValue.Values;
			case VariableType.Enum:
				return this.enumValue;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		public void GetValueFrom(INamedVariable variable)
		{
			if (variable == null)
			{
				return;
			}
			switch (this.type)
			{
			case VariableType.Unknown:
				return;
			case VariableType.Float:
				this.floatValue = ((SkillFloat)variable).Value;
				return;
			case VariableType.Int:
				this.intValue = ((SkillInt)variable).Value;
				return;
			case VariableType.Bool:
				this.boolValue = ((SkillBool)variable).Value;
				return;
			case VariableType.GameObject:
				this.objectReference = ((SkillGameObject)variable).Value;
				return;
			case VariableType.String:
				this.stringValue = ((SkillString)variable).Value;
				return;
			case VariableType.Vector2:
				this.vector2Value = ((SkillVector2)variable).Value;
				return;
			case VariableType.Vector3:
				this.vector3Value = ((SkillVector3)variable).Value;
				return;
			case VariableType.Color:
				this.colorValue = ((SkillColor)variable).Value;
				return;
			case VariableType.Rect:
				this.rectValue = ((SkillRect)variable).Value;
				return;
			case VariableType.Material:
				this.objectReference = ((SkillMaterial)variable).Value;
				return;
			case VariableType.Texture:
				this.objectReference = ((SkillTexture)variable).Value;
				return;
			case VariableType.Quaternion:
				this.quaternionValue = ((SkillQuaternion)variable).Value;
				return;
			case VariableType.Object:
				this.objectReference = ((SkillObject)variable).Value;
				return;
			case VariableType.Array:
				this.arrayValue = new SkillArray((SkillArray)variable);
				return;
			case VariableType.Enum:
				this.EnumValue = ((SkillEnum)variable).Value;
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		public void UpdateValue()
		{
			this.GetValueFrom(this.NamedVar);
		}
		public void ApplyValueTo(INamedVariable targetVariable)
		{
			if (targetVariable == null)
			{
				return;
			}
			switch (this.type)
			{
			case VariableType.Unknown:
				return;
			case VariableType.Float:
				((SkillFloat)targetVariable).Value = this.floatValue;
				return;
			case VariableType.Int:
				((SkillInt)targetVariable).Value = this.intValue;
				return;
			case VariableType.Bool:
				((SkillBool)targetVariable).Value = this.boolValue;
				return;
			case VariableType.GameObject:
				((SkillGameObject)targetVariable).Value = (this.objectReference as GameObject);
				return;
			case VariableType.String:
				((SkillString)targetVariable).Value = this.stringValue;
				return;
			case VariableType.Vector2:
				((SkillVector2)targetVariable).Value = this.vector2Value;
				return;
			case VariableType.Vector3:
				((SkillVector3)targetVariable).Value = this.vector3Value;
				return;
			case VariableType.Color:
				((SkillColor)targetVariable).Value = this.colorValue;
				return;
			case VariableType.Rect:
				((SkillRect)targetVariable).Value = this.rectValue;
				return;
			case VariableType.Material:
				((SkillMaterial)targetVariable).Value = (this.objectReference as Material);
				return;
			case VariableType.Texture:
				((SkillTexture)targetVariable).Value = (this.objectReference as Texture);
				return;
			case VariableType.Quaternion:
				((SkillQuaternion)targetVariable).Value = this.quaternionValue;
				return;
			case VariableType.Object:
				((SkillObject)targetVariable).Value = this.objectReference;
				return;
			case VariableType.Array:
				((SkillArray)targetVariable).CopyValues(this.arrayValue);
				return;
			case VariableType.Enum:
				((SkillEnum)targetVariable).Value = this.EnumValue;
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		public string DebugString()
		{
			if (string.IsNullOrEmpty(this.variableName))
			{
				return "None";
			}
			return this.variableName + ": " + this.NamedVar;
		}
		public override string ToString()
		{
			if (this.NamedVar != null)
			{
				return this.NamedVar.ToString();
			}
			return "None";
		}
		public void SetValue(object value)
		{
			switch (this.type)
			{
			case VariableType.Unknown:
				Debug.LogError("Unsupported type: " + ((value != null) ? value.GetType().ToString() : "null"));
				break;
			case VariableType.Float:
				this.floatValue = ((value != null) ? ((float)value) : 0f);
				break;
			case VariableType.Int:
				this.intValue = ((value != null) ? ((int)value) : 0);
				break;
			case VariableType.Bool:
				this.boolValue = (value != null && (bool)value);
				break;
			case VariableType.GameObject:
				this.gameObjectValue = (value as GameObject);
				break;
			case VariableType.String:
				this.stringValue = (value as string);
				break;
			case VariableType.Vector2:
				this.vector2Value = ((value != null) ? ((Vector2)value) : Vector2.get_zero());
				break;
			case VariableType.Vector3:
				this.vector3Value = ((value != null) ? ((Vector3)value) : Vector3.get_zero());
				break;
			case VariableType.Color:
				this.colorValue = ((value != null) ? ((Color)value) : Color.get_white());
				break;
			case VariableType.Rect:
				this.rectValue = ((value != null) ? ((Rect)value) : default(Rect));
				break;
			case VariableType.Material:
				this.materialValue = (value as Material);
				break;
			case VariableType.Texture:
				this.textureValue = (value as Texture);
				break;
			case VariableType.Quaternion:
				this.quaternionValue = ((value != null) ? ((Quaternion)value) : Quaternion.get_identity());
				break;
			case VariableType.Object:
				this.objectReference = (value as Object);
				break;
			case VariableType.Array:
			{
				Array array = value as Array;
				if (array != null)
				{
					object[] array2 = new object[array.get_Length()];
					for (int i = 0; i < array.get_Length(); i++)
					{
						array2[i] = array.GetValue(i);
					}
					this.arrayValue.Values = array2;
					this.arrayValue.SaveChanges();
				}
				break;
			}
			case VariableType.Enum:
				this.EnumValue = (value as Enum);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			this.ApplyValueTo(this.namedVar);
		}
		private void DebugLog()
		{
			Debug.Log("Type: " + this.type);
			Debug.Log("UseVariable: " + this.useVariable);
		}
		public static VariableType GetVariableType(Type type)
		{
			if (object.ReferenceEquals(type, typeof(Material)))
			{
				return VariableType.Material;
			}
			if (object.ReferenceEquals(type, typeof(Texture)))
			{
				return VariableType.Texture;
			}
			if (object.ReferenceEquals(type, typeof(float)))
			{
				return VariableType.Float;
			}
			if (object.ReferenceEquals(type, typeof(int)))
			{
				return VariableType.Int;
			}
			if (object.ReferenceEquals(type, typeof(bool)))
			{
				return VariableType.Bool;
			}
			if (object.ReferenceEquals(type, typeof(string)))
			{
				return VariableType.String;
			}
			if (object.ReferenceEquals(type, typeof(GameObject)))
			{
				return VariableType.GameObject;
			}
			if (object.ReferenceEquals(type, typeof(Vector2)))
			{
				return VariableType.Vector2;
			}
			if (object.ReferenceEquals(type, typeof(Vector3)))
			{
				return VariableType.Vector3;
			}
			if (object.ReferenceEquals(type, typeof(Rect)))
			{
				return VariableType.Rect;
			}
			if (object.ReferenceEquals(type, typeof(Quaternion)))
			{
				return VariableType.Quaternion;
			}
			if (object.ReferenceEquals(type, typeof(Color)))
			{
				return VariableType.Color;
			}
			if (typeof(Object).IsAssignableFrom(type))
			{
				return VariableType.Object;
			}
			if (type.get_IsEnum())
			{
				return VariableType.Enum;
			}
			if (type.get_IsArray())
			{
				return VariableType.Array;
			}
			return VariableType.Unknown;
		}
	}
}
