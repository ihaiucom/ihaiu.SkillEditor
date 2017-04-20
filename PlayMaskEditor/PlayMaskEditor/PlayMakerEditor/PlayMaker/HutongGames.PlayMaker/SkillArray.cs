using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillArray : NamedVariable
	{
		[SerializeField]
		private VariableType type = VariableType.Unknown;
		[SerializeField]
		private string objectTypeName;
		private Type objectType;
		public float[] floatValues;
		public int[] intValues;
		public bool[] boolValues;
		public string[] stringValues;
		public Vector4[] vector4Values;
		public Object[] objectReferences;
		[NonSerialized]
		private Array sourceArray;
		[NonSerialized]
		private object[] values;
		public override object RawValue
		{
			get
			{
				return this.values;
			}
			set
			{
				this.values = (object[])value;
			}
		}
		public override Type ObjectType
		{
			get
			{
				if (object.ReferenceEquals(this.objectType, null))
				{
					if (string.IsNullOrEmpty(this.objectTypeName))
					{
						if (this.ElementType == VariableType.Enum)
						{
							this.objectTypeName = typeof(None).get_FullName();
						}
						else
						{
							this.objectTypeName = typeof(Object).get_FullName();
						}
					}
					this.objectType = ReflectionUtils.GetGlobalType(this.objectTypeName);
				}
				return this.objectType;
			}
			set
			{
				if (!object.ReferenceEquals(this.objectType, value))
				{
					this.Reset();
					if (this.ElementType == VariableType.Enum)
					{
						if (object.ReferenceEquals(value, null))
						{
							value = typeof(None);
						}
						else
						{
							if (!value.get_IsEnum())
							{
								value = typeof(None);
							}
						}
					}
					else
					{
						if (this.ElementType == VariableType.Object)
						{
							if (object.ReferenceEquals(value, null))
							{
								value = typeof(Object);
							}
							else
							{
								if (!typeof(Object).IsAssignableFrom(value))
								{
									value = typeof(None);
								}
							}
						}
						else
						{
							if (object.ReferenceEquals(value, null))
							{
								value = typeof(Object);
							}
						}
					}
					this.objectType = value;
					this.objectTypeName = this.objectType.get_FullName();
				}
			}
		}
		public string ObjectTypeName
		{
			get
			{
				return this.objectTypeName;
			}
		}
		public object[] Values
		{
			get
			{
				if (this.values == null)
				{
					this.InitArray();
				}
				return this.values;
			}
			set
			{
				this.values = value;
			}
		}
		public int Length
		{
			get
			{
				return this.Values.Length;
			}
		}
		public override VariableType TypeConstraint
		{
			get
			{
				return this.type;
			}
		}
		public VariableType ElementType
		{
			get
			{
				return this.type;
			}
			set
			{
				this.SetType(value);
			}
		}
		public override VariableType VariableType
		{
			get
			{
				return VariableType.Array;
			}
		}
		private void InitArray()
		{
			this.sourceArray = this.GetSourceArray();
			if (this.sourceArray != null)
			{
				this.values = new object[this.sourceArray.get_Length()];
				for (int i = 0; i < this.values.Length; i++)
				{
					this.values[i] = this.Load(i);
				}
				return;
			}
			this.values = new object[0];
		}
		public object Get(int index)
		{
			return this.Values[index];
		}
		public void Set(int index, object value)
		{
			this.Values[index] = value;
		}
		private object Load(int index)
		{
			switch (this.type)
			{
			case VariableType.Unknown:
				return null;
			case VariableType.Float:
				return this.floatValues[index];
			case VariableType.Int:
				return this.intValues[index];
			case VariableType.Bool:
				return this.boolValues[index];
			case VariableType.GameObject:
				return this.objectReferences[index] as GameObject;
			case VariableType.String:
				return this.stringValues[index];
			case VariableType.Vector2:
			{
				Vector4 vector = this.vector4Values[index];
				return new Vector2(vector.x, vector.y);
			}
			case VariableType.Vector3:
			{
				Vector4 vector = this.vector4Values[index];
				return new Vector3(vector.x, vector.y, vector.z);
			}
			case VariableType.Color:
			{
				Vector4 vector = this.vector4Values[index];
				return new Color(vector.x, vector.y, vector.z, vector.w);
			}
			case VariableType.Rect:
			{
				Vector4 vector = this.vector4Values[index];
				return new Rect(vector.x, vector.y, vector.z, vector.w);
			}
			case VariableType.Material:
				return this.objectReferences[index] as Material;
			case VariableType.Texture:
				return this.objectReferences[index] as Texture;
			case VariableType.Quaternion:
			{
				Vector4 vector = this.vector4Values[index];
				return new Quaternion(vector.x, vector.y, vector.z, vector.w);
			}
			case VariableType.Object:
				return this.objectReferences[index];
			case VariableType.Array:
				Debug.LogError("Nested arrays are not supported yet!");
				return null;
			case VariableType.Enum:
				return Enum.ToObject(this.ObjectType, this.intValues[index]);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		private void Save(int index, object value)
		{
			switch (this.type)
			{
			case VariableType.Unknown:
				return;
			case VariableType.Float:
				this.floatValues[index] = ((value != null) ? ((float)value) : 0f);
				return;
			case VariableType.Int:
				this.intValues[index] = ((value != null) ? ((int)value) : 0);
				return;
			case VariableType.Bool:
				this.boolValues[index] = (value != null && (bool)value);
				return;
			case VariableType.GameObject:
				this.objectReferences[index] = (value as GameObject);
				return;
			case VariableType.String:
				this.stringValues[index] = (value as string);
				return;
			case VariableType.Vector2:
				this.vector4Values[index] = ((value != null) ? ((Vector2)value) : Vector2.get_zero());
				return;
			case VariableType.Vector3:
				this.vector4Values[index] = ((value != null) ? ((Vector3)value) : Vector3.get_zero());
				return;
			case VariableType.Color:
				this.vector4Values[index] = ((value != null) ? ((Color)value) : Color.get_white());
				return;
			case VariableType.Rect:
			{
				Rect rect = (value != null) ? ((Rect)value) : new Rect(0f, 0f, 0f, 0f);
				this.vector4Values[index] = new Vector4(rect.get_x(), rect.get_y(), rect.get_width(), rect.get_height());
				return;
			}
			case VariableType.Material:
				this.objectReferences[index] = (value as Material);
				return;
			case VariableType.Texture:
				this.objectReferences[index] = (value as Texture);
				return;
			case VariableType.Quaternion:
			{
				Quaternion quaternion = (value != null) ? ((Quaternion)value) : Quaternion.get_identity();
				this.vector4Values[index] = new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
				return;
			}
			case VariableType.Object:
				this.objectReferences[index] = (value as Object);
				return;
			case VariableType.Array:
				Debug.LogError("Nested arrays are not supported yet!");
				return;
			case VariableType.Enum:
				this.intValues[index] = Convert.ToInt32(value);
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		public void SetType(VariableType newType)
		{
			if (this.type != newType)
			{
				this.type = newType;
				this.ObjectType = null;
				this.Reset();
				this.ConformSourceArraySize();
			}
		}
		public void SaveChanges()
		{
			this.ConformSourceArraySize();
			for (int i = 0; i < this.values.Length; i++)
			{
				this.Save(i, this.values[i]);
				this.values[i] = this.Load(i);
			}
		}
		public void CopyValues(SkillArray source)
		{
			if (source == null)
			{
				return;
			}
			this.Resize(source.Length);
			object[] array = source.Values;
			for (int i = 0; i < array.Length; i++)
			{
				this.Set(i, array[i]);
			}
			this.SaveChanges();
		}
		private void ConformSourceArraySize()
		{
			switch (this.type)
			{
			case VariableType.Unknown:
			case VariableType.Array:
				return;
			case VariableType.Float:
				this.floatValues = new float[this.Values.Length];
				return;
			case VariableType.Int:
			case VariableType.Enum:
				this.intValues = new int[this.Values.Length];
				return;
			case VariableType.Bool:
				this.boolValues = new bool[this.Values.Length];
				return;
			case VariableType.GameObject:
			case VariableType.Material:
			case VariableType.Texture:
			case VariableType.Object:
				this.objectReferences = new Object[this.Values.Length];
				return;
			case VariableType.String:
				this.stringValues = new string[this.Values.Length];
				return;
			case VariableType.Vector2:
			case VariableType.Vector3:
			case VariableType.Color:
			case VariableType.Rect:
			case VariableType.Quaternion:
				this.vector4Values = new Vector4[this.Values.Length];
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		private Array GetSourceArray()
		{
			switch (this.type)
			{
			case VariableType.Unknown:
				return null;
			case VariableType.Float:
			{
				float[] arg_6D_0;
				if ((arg_6D_0 = this.floatValues) == null)
				{
					arg_6D_0 = (this.floatValues = new float[0]);
				}
				return arg_6D_0;
			}
			case VariableType.Int:
			case VariableType.Enum:
			{
				int[] arg_87_0;
				if ((arg_87_0 = this.intValues) == null)
				{
					arg_87_0 = (this.intValues = new int[0]);
				}
				return arg_87_0;
			}
			case VariableType.Bool:
			{
				bool[] arg_A1_0;
				if ((arg_A1_0 = this.boolValues) == null)
				{
					arg_A1_0 = (this.boolValues = new bool[0]);
				}
				return arg_A1_0;
			}
			case VariableType.GameObject:
			case VariableType.Material:
			case VariableType.Texture:
			case VariableType.Object:
			{
				Object[] arg_BD_0;
				if ((arg_BD_0 = this.objectReferences) == null)
				{
					arg_BD_0 = (this.objectReferences = new Object[0]);
				}
				return arg_BD_0;
			}
			case VariableType.String:
			{
				string[] arg_D9_0;
				if ((arg_D9_0 = this.stringValues) == null)
				{
					arg_D9_0 = (this.stringValues = new string[0]);
				}
				return arg_D9_0;
			}
			case VariableType.Vector2:
			case VariableType.Vector3:
			case VariableType.Color:
			case VariableType.Rect:
			case VariableType.Quaternion:
			{
				Vector4[] arg_F5_0;
				if ((arg_F5_0 = this.vector4Values) == null)
				{
					arg_F5_0 = (this.vector4Values = new Vector4[0]);
				}
				return arg_F5_0;
			}
			case VariableType.Array:
				return null;
			default:
				Debug.LogError(this.type);
				throw new ArgumentOutOfRangeException();
			}
		}
		public void Resize(int newLength)
		{
			if (newLength == this.Values.Length)
			{
				return;
			}
			if (newLength < 0)
			{
				newLength = 0;
			}
			Type elementType = this.Values.GetType().GetElementType();
			Array array = Array.CreateInstance(elementType, newLength);
			Array.Copy(this.values, array, Math.Min(this.values.Length, newLength));
			this.Values = (object[])array;
			this.SaveChanges();
		}
		public void Reset()
		{
			this.floatValues = null;
			this.intValues = null;
			this.boolValues = null;
			this.stringValues = null;
			this.vector4Values = null;
			this.objectReferences = null;
			this.objectType = null;
			this.objectTypeName = null;
			this.InitArray();
		}
		public SkillArray()
		{
		}
		public SkillArray(string name) : base(name)
		{
		}
		public SkillArray(SkillArray source) : base(source)
		{
			if (source != null)
			{
				this.type = source.type;
				this.ObjectType = source.ObjectType;
				this.CopyValues(source);
				this.SaveChanges();
			}
		}
		public override NamedVariable Clone()
		{
			return new SkillArray(this);
		}
		public override string ToString()
		{
			string text = string.Empty;
			for (int i = 0; i < this.Values.Length; i++)
			{
				object obj = this.Values[i];
				if (obj == null)
				{
					text += "null";
				}
				else
				{
					Object @object = obj as Object;
					if (@object != null)
					{
						text += @object.get_name();
					}
					else
					{
						text += obj.ToString();
					}
				}
				if (i < this.Values.Length - 1)
				{
					text += ", ";
				}
			}
			if (text == string.Empty)
			{
				text = "Empty";
			}
			return text;
		}
		public override bool TestTypeConstraint(VariableType variableType, Type _objectType = null)
		{
			return variableType == VariableType.Unknown || (base.TestTypeConstraint(variableType, this.objectType) && (object.ReferenceEquals(this.ObjectType, _objectType) || object.ReferenceEquals(_objectType, null)));
		}
		public Type RealType()
		{
			switch (this.type)
			{
			case VariableType.Unknown:
				return null;
			case VariableType.Float:
				return typeof(float[]);
			case VariableType.Int:
				return typeof(int[]);
			case VariableType.Bool:
				return typeof(bool[]);
			case VariableType.GameObject:
				return typeof(GameObject[]);
			case VariableType.String:
				return typeof(string[]);
			case VariableType.Vector2:
				return typeof(Vector2[]);
			case VariableType.Vector3:
				return typeof(Vector3[]);
			case VariableType.Color:
				return typeof(Color[]);
			case VariableType.Rect:
				return typeof(Rect[]);
			case VariableType.Material:
				return typeof(Material[]);
			case VariableType.Texture:
				return typeof(Texture[]);
			case VariableType.Quaternion:
				return typeof(Quaternion[]);
			case VariableType.Object:
				return this.ObjectType.MakeArrayType();
			case VariableType.Array:
				Debug.LogError("Nested arrays are not supported yet!");
				return null;
			case VariableType.Enum:
				return this.ObjectType.MakeArrayType();
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
