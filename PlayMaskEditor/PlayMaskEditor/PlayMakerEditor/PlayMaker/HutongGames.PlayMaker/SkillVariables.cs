using HutongGames.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillVariables
	{
		[SerializeField]
		private SkillFloat[] floatVariables;
		[SerializeField]
		private SkillInt[] intVariables;
		[SerializeField]
		private SkillBool[] boolVariables;
		[SerializeField]
		private SkillString[] stringVariables;
		[SerializeField]
		private SkillVector2[] vector2Variables;
		[SerializeField]
		private SkillVector3[] vector3Variables;
		[SerializeField]
		private SkillColor[] colorVariables;
		[SerializeField]
		private SkillRect[] rectVariables;
		[SerializeField]
		private SkillQuaternion[] quaternionVariables;
		[SerializeField]
		private SkillGameObject[] gameObjectVariables;
		[SerializeField]
		private SkillObject[] objectVariables;
		[SerializeField]
		private SkillMaterial[] materialVariables;
		[SerializeField]
		private SkillTexture[] textureVariables;
		[SerializeField]
		private SkillArray[] arrayVariables;
		[SerializeField]
		private SkillEnum[] enumVariables;
		[SerializeField]
		private string[] categories = new string[]
		{
			""
		};
		[SerializeField]
		private int[] variableCategoryIDs = new int[0];
		public static PlayMakerGlobals GlobalsComponent
		{
			get
			{
				return PlayMakerGlobals.Instance;
			}
		}
		public static SkillVariables GlobalVariables
		{
			get
			{
				return PlayMakerGlobals.Instance.Variables;
			}
		}
		public static bool GlobalVariablesSynced
		{
			get;
			set;
		}
		public string[] Categories
		{
			get
			{
				return this.categories;
			}
			set
			{
				this.categories = value;
			}
		}
		public int[] CategoryIDs
		{
			get
			{
				return this.variableCategoryIDs;
			}
			set
			{
				this.variableCategoryIDs = value;
			}
		}
		public SkillFloat[] FloatVariables
		{
			get
			{
				return this.floatVariables ?? Arrays<SkillFloat>.Empty;
			}
			set
			{
				this.floatVariables = value;
			}
		}
		public SkillInt[] IntVariables
		{
			get
			{
				return this.intVariables ?? Arrays<SkillInt>.Empty;
			}
			set
			{
				this.intVariables = value;
			}
		}
		public SkillBool[] BoolVariables
		{
			get
			{
				return this.boolVariables ?? Arrays<SkillBool>.Empty;
			}
			set
			{
				this.boolVariables = value;
			}
		}
		public SkillString[] StringVariables
		{
			get
			{
				return this.stringVariables ?? Arrays<SkillString>.Empty;
			}
			set
			{
				this.stringVariables = value;
			}
		}
		public SkillVector2[] Vector2Variables
		{
			get
			{
				return this.vector2Variables ?? Arrays<SkillVector2>.Empty;
			}
			set
			{
				this.vector2Variables = value;
			}
		}
		public SkillVector3[] Vector3Variables
		{
			get
			{
				return this.vector3Variables ?? Arrays<SkillVector3>.Empty;
			}
			set
			{
				this.vector3Variables = value;
			}
		}
		public SkillRect[] RectVariables
		{
			get
			{
				return this.rectVariables ?? Arrays<SkillRect>.Empty;
			}
			set
			{
				this.rectVariables = value;
			}
		}
		public SkillQuaternion[] QuaternionVariables
		{
			get
			{
				return this.quaternionVariables ?? Arrays<SkillQuaternion>.Empty;
			}
			set
			{
				this.quaternionVariables = value;
			}
		}
		public SkillColor[] ColorVariables
		{
			get
			{
				return this.colorVariables ?? Arrays<SkillColor>.Empty;
			}
			set
			{
				this.colorVariables = value;
			}
		}
		public SkillGameObject[] GameObjectVariables
		{
			get
			{
				return this.gameObjectVariables ?? Arrays<SkillGameObject>.Empty;
			}
			set
			{
				this.gameObjectVariables = value;
			}
		}
		public SkillArray[] ArrayVariables
		{
			get
			{
				return this.arrayVariables ?? Arrays<SkillArray>.Empty;
			}
			set
			{
				this.arrayVariables = value;
			}
		}
		public SkillEnum[] EnumVariables
		{
			get
			{
				return this.enumVariables ?? Arrays<SkillEnum>.Empty;
			}
			set
			{
				this.enumVariables = value;
			}
		}
		public SkillObject[] ObjectVariables
		{
			get
			{
				return this.objectVariables ?? Arrays<SkillObject>.Empty;
			}
			set
			{
				this.objectVariables = value;
			}
		}
		public SkillMaterial[] MaterialVariables
		{
			get
			{
				return this.materialVariables ?? Arrays<SkillMaterial>.Empty;
			}
			set
			{
				this.materialVariables = value;
			}
		}
		public SkillTexture[] TextureVariables
		{
			get
			{
				return this.textureVariables ?? Arrays<SkillTexture>.Empty;
			}
			set
			{
				this.textureVariables = value;
			}
		}
		public NamedVariable[] GetAllNamedVariables()
		{
			List<NamedVariable> list = new List<NamedVariable>();
			list.AddRange(this.FloatVariables);
			list.AddRange(this.IntVariables);
			list.AddRange(this.BoolVariables);
			list.AddRange(this.StringVariables);
			list.AddRange(this.Vector2Variables);
			list.AddRange(this.Vector3Variables);
			list.AddRange(this.RectVariables);
			list.AddRange(this.QuaternionVariables);
			list.AddRange(this.GameObjectVariables);
			list.AddRange(this.ObjectVariables);
			list.AddRange(this.MaterialVariables);
			list.AddRange(this.TextureVariables);
			list.AddRange(this.ColorVariables);
			list.AddRange(this.ArrayVariables);
			list.AddRange(this.EnumVariables);
			return list.ToArray();
		}
		public NamedVariable[] GetNamedVariables(VariableType type)
		{
			switch (type)
			{
			case VariableType.Unknown:
				return this.GetAllNamedVariables();
			case VariableType.Float:
				return this.FloatVariables;
			case VariableType.Int:
				return this.IntVariables;
			case VariableType.Bool:
				return this.BoolVariables;
			case VariableType.GameObject:
				return this.GameObjectVariables;
			case VariableType.String:
				return this.StringVariables;
			case VariableType.Vector2:
				return this.Vector2Variables;
			case VariableType.Vector3:
				return this.Vector3Variables;
			case VariableType.Color:
				return this.ColorVariables;
			case VariableType.Rect:
				return this.RectVariables;
			case VariableType.Material:
				return this.MaterialVariables;
			case VariableType.Texture:
				return this.TextureVariables;
			case VariableType.Quaternion:
				return this.QuaternionVariables;
			case VariableType.Object:
				return this.ObjectVariables;
			case VariableType.Array:
				return this.ArrayVariables;
			case VariableType.Enum:
				return this.EnumVariables;
			default:
				throw new ArgumentOutOfRangeException("type");
			}
		}
		public bool Contains(string variableName)
		{
			NamedVariable[] allNamedVariables = this.GetAllNamedVariables();
			NamedVariable[] array = allNamedVariables;
			for (int i = 0; i < array.Length; i++)
			{
				NamedVariable namedVariable = array[i];
				if (namedVariable.Name == variableName)
				{
					return true;
				}
			}
			return false;
		}
		public bool Contains(NamedVariable variable)
		{
			NamedVariable[] allNamedVariables = this.GetAllNamedVariables();
			NamedVariable[] array = allNamedVariables;
			for (int i = 0; i < array.Length; i++)
			{
				NamedVariable namedVariable = array[i];
				if (namedVariable == variable)
				{
					return true;
				}
			}
			return false;
		}
		public NamedVariable[] GetNames(Type ofType)
		{
			if (object.ReferenceEquals(ofType, typeof(SkillFloat)))
			{
				return this.FloatVariables;
			}
			if (object.ReferenceEquals(ofType, typeof(SkillInt)))
			{
				return this.IntVariables;
			}
			if (object.ReferenceEquals(ofType, typeof(SkillBool)))
			{
				return this.BoolVariables;
			}
			if (object.ReferenceEquals(ofType, typeof(SkillString)))
			{
				return this.StringVariables;
			}
			if (object.ReferenceEquals(ofType, typeof(SkillVector2)))
			{
				return this.Vector2Variables;
			}
			if (object.ReferenceEquals(ofType, typeof(SkillVector3)))
			{
				return this.Vector3Variables;
			}
			if (object.ReferenceEquals(ofType, typeof(SkillRect)))
			{
				return this.RectVariables;
			}
			if (object.ReferenceEquals(ofType, typeof(SkillQuaternion)))
			{
				return this.QuaternionVariables;
			}
			if (object.ReferenceEquals(ofType, typeof(SkillObject)))
			{
				return this.ObjectVariables;
			}
			if (object.ReferenceEquals(ofType, typeof(SkillMaterial)))
			{
				return this.MaterialVariables;
			}
			if (object.ReferenceEquals(ofType, typeof(SkillTexture)))
			{
				return this.TextureVariables;
			}
			if (object.ReferenceEquals(ofType, typeof(SkillColor)))
			{
				return this.ColorVariables;
			}
			if (object.ReferenceEquals(ofType, typeof(SkillGameObject)))
			{
				return this.GameObjectVariables;
			}
			if (object.ReferenceEquals(ofType, typeof(SkillArray)))
			{
				return this.ArrayVariables;
			}
			if (object.ReferenceEquals(ofType, typeof(SkillEnum)))
			{
				return this.EnumVariables;
			}
			return new NamedVariable[0];
		}
		public SkillVariables()
		{
		}
		public SkillVariables(SkillVariables source)
		{
			if (source == null)
			{
				return;
			}
			if (source.floatVariables != null)
			{
				this.floatVariables = new SkillFloat[source.floatVariables.Length];
				for (int i = 0; i < source.floatVariables.Length; i++)
				{
					this.floatVariables[i] = new SkillFloat(source.floatVariables[i]);
				}
			}
			if (source.intVariables != null)
			{
				this.intVariables = new SkillInt[source.intVariables.Length];
				for (int j = 0; j < source.intVariables.Length; j++)
				{
					this.intVariables[j] = new SkillInt(source.intVariables[j]);
				}
			}
			if (source.boolVariables != null)
			{
				this.boolVariables = new SkillBool[source.boolVariables.Length];
				for (int k = 0; k < source.boolVariables.Length; k++)
				{
					this.boolVariables[k] = new SkillBool(source.boolVariables[k]);
				}
			}
			if (source.gameObjectVariables != null)
			{
				this.gameObjectVariables = new SkillGameObject[source.gameObjectVariables.Length];
				for (int l = 0; l < source.gameObjectVariables.Length; l++)
				{
					this.gameObjectVariables[l] = new SkillGameObject(source.gameObjectVariables[l]);
				}
			}
			if (source.colorVariables != null)
			{
				this.colorVariables = new SkillColor[source.colorVariables.Length];
				for (int m = 0; m < source.colorVariables.Length; m++)
				{
					this.colorVariables[m] = new SkillColor(source.colorVariables[m]);
				}
			}
			if (source.vector2Variables != null)
			{
				this.vector2Variables = new SkillVector2[source.vector2Variables.Length];
				for (int n = 0; n < source.vector2Variables.Length; n++)
				{
					this.vector2Variables[n] = new SkillVector2(source.vector2Variables[n]);
				}
			}
			if (source.vector3Variables != null)
			{
				this.vector3Variables = new SkillVector3[source.vector3Variables.Length];
				for (int num = 0; num < source.vector3Variables.Length; num++)
				{
					this.vector3Variables[num] = new SkillVector3(source.vector3Variables[num]);
				}
			}
			if (source.rectVariables != null)
			{
				this.rectVariables = new SkillRect[source.rectVariables.Length];
				for (int num2 = 0; num2 < source.rectVariables.Length; num2++)
				{
					this.rectVariables[num2] = new SkillRect(source.rectVariables[num2]);
				}
			}
			if (source.quaternionVariables != null)
			{
				this.quaternionVariables = new SkillQuaternion[source.quaternionVariables.Length];
				for (int num3 = 0; num3 < source.quaternionVariables.Length; num3++)
				{
					this.quaternionVariables[num3] = new SkillQuaternion(source.quaternionVariables[num3]);
				}
			}
			if (source.objectVariables != null)
			{
				this.objectVariables = new SkillObject[source.objectVariables.Length];
				for (int num4 = 0; num4 < source.objectVariables.Length; num4++)
				{
					this.objectVariables[num4] = new SkillObject(source.objectVariables[num4]);
				}
			}
			if (source.materialVariables != null)
			{
				this.materialVariables = new SkillMaterial[source.materialVariables.Length];
				for (int num5 = 0; num5 < source.materialVariables.Length; num5++)
				{
					this.materialVariables[num5] = new SkillMaterial(source.materialVariables[num5]);
				}
			}
			if (source.textureVariables != null)
			{
				this.textureVariables = new SkillTexture[source.textureVariables.Length];
				for (int num6 = 0; num6 < source.textureVariables.Length; num6++)
				{
					this.textureVariables[num6] = new SkillTexture(source.textureVariables[num6]);
				}
			}
			if (source.stringVariables != null)
			{
				this.stringVariables = new SkillString[source.stringVariables.Length];
				for (int num7 = 0; num7 < source.stringVariables.Length; num7++)
				{
					this.stringVariables[num7] = new SkillString(source.stringVariables[num7]);
				}
			}
			if (source.arrayVariables != null)
			{
				this.arrayVariables = new SkillArray[source.arrayVariables.Length];
				for (int num8 = 0; num8 < source.arrayVariables.Length; num8++)
				{
					this.arrayVariables[num8] = new SkillArray(source.arrayVariables[num8]);
				}
			}
			if (source.enumVariables != null)
			{
				this.enumVariables = new SkillEnum[source.enumVariables.Length];
				for (int num9 = 0; num9 < source.enumVariables.Length; num9++)
				{
					this.enumVariables[num9] = new SkillEnum(source.enumVariables[num9]);
				}
			}
			if (source.categories != null)
			{
				this.categories = new string[source.categories.Length];
				for (int num10 = 0; num10 < source.categories.Length; num10++)
				{
					this.categories[num10] = source.categories[num10];
				}
			}
			if (source.CategoryIDs != null)
			{
				this.CategoryIDs = new int[source.CategoryIDs.Length];
				for (int num11 = 0; num11 < source.CategoryIDs.Length; num11++)
				{
					this.CategoryIDs[num11] = source.CategoryIDs[num11];
				}
			}
			if (source.Categories != null)
			{
				this.Categories = new string[source.Categories.Length];
				for (int num12 = 0; num12 < source.Categories.Length; num12++)
				{
					this.Categories[num12] = source.Categories[num12];
				}
			}
		}
		public void OverrideVariableValues(SkillVariables source)
		{
			for (int i = 0; i < source.FloatVariables.Length; i++)
			{
				for (int j = 0; j < this.FloatVariables.Length; j++)
				{
					if (this.floatVariables[j].ShowInInspector && source.floatVariables[i].Name == this.floatVariables[j].Name)
					{
						this.floatVariables[j].Value = source.floatVariables[i].Value;
					}
				}
			}
			for (int k = 0; k < source.IntVariables.Length; k++)
			{
				for (int l = 0; l < this.IntVariables.Length; l++)
				{
					if (this.intVariables[l].ShowInInspector && source.intVariables[k].Name == this.intVariables[l].Name)
					{
						this.intVariables[l].Value = source.intVariables[k].Value;
					}
				}
			}
			for (int m = 0; m < source.BoolVariables.Length; m++)
			{
				for (int n = 0; n < this.BoolVariables.Length; n++)
				{
					if (this.boolVariables[n].ShowInInspector && source.boolVariables[m].Name == this.boolVariables[n].Name)
					{
						this.boolVariables[n].Value = source.boolVariables[m].Value;
					}
				}
			}
			for (int num = 0; num < source.GameObjectVariables.Length; num++)
			{
				for (int num2 = 0; num2 < this.GameObjectVariables.Length; num2++)
				{
					if (this.gameObjectVariables[num2].ShowInInspector && source.gameObjectVariables[num].Name == this.gameObjectVariables[num2].Name)
					{
						this.gameObjectVariables[num2].Value = source.gameObjectVariables[num].Value;
					}
				}
			}
			for (int num3 = 0; num3 < source.ColorVariables.Length; num3++)
			{
				for (int num4 = 0; num4 < this.ColorVariables.Length; num4++)
				{
					if (this.colorVariables[num4].ShowInInspector && source.colorVariables[num3].Name == this.colorVariables[num4].Name)
					{
						this.colorVariables[num4].Value = source.colorVariables[num3].Value;
					}
				}
			}
			for (int num5 = 0; num5 < source.Vector2Variables.Length; num5++)
			{
				for (int num6 = 0; num6 < this.Vector2Variables.Length; num6++)
				{
					if (this.vector2Variables[num6].ShowInInspector && source.vector2Variables[num5].Name == this.vector2Variables[num6].Name)
					{
						this.vector2Variables[num6].Value = source.vector2Variables[num5].Value;
					}
				}
			}
			for (int num7 = 0; num7 < source.Vector3Variables.Length; num7++)
			{
				for (int num8 = 0; num8 < this.Vector3Variables.Length; num8++)
				{
					if (this.vector3Variables[num8].ShowInInspector && source.vector3Variables[num7].Name == this.vector3Variables[num8].Name)
					{
						this.vector3Variables[num8].Value = source.vector3Variables[num7].Value;
					}
				}
			}
			for (int num9 = 0; num9 < source.RectVariables.Length; num9++)
			{
				for (int num10 = 0; num10 < this.RectVariables.Length; num10++)
				{
					if (this.rectVariables[num10].ShowInInspector && source.rectVariables[num9].Name == this.rectVariables[num10].Name)
					{
						this.rectVariables[num10].Value = source.rectVariables[num9].Value;
					}
				}
			}
			for (int num11 = 0; num11 < source.QuaternionVariables.Length; num11++)
			{
				for (int num12 = 0; num12 < this.QuaternionVariables.Length; num12++)
				{
					if (this.quaternionVariables[num12].ShowInInspector && source.quaternionVariables[num11].Name == this.quaternionVariables[num12].Name)
					{
						this.quaternionVariables[num12].Value = source.quaternionVariables[num11].Value;
					}
				}
			}
			for (int num13 = 0; num13 < source.ObjectVariables.Length; num13++)
			{
				for (int num14 = 0; num14 < this.ObjectVariables.Length; num14++)
				{
					if (this.objectVariables[num14].ShowInInspector && source.objectVariables[num13].Name == this.objectVariables[num14].Name)
					{
						this.objectVariables[num14].Value = source.objectVariables[num13].Value;
					}
				}
			}
			for (int num15 = 0; num15 < source.MaterialVariables.Length; num15++)
			{
				for (int num16 = 0; num16 < this.MaterialVariables.Length; num16++)
				{
					if (this.materialVariables[num16].ShowInInspector && source.materialVariables[num15].Name == this.materialVariables[num16].Name)
					{
						this.materialVariables[num16].Value = source.materialVariables[num15].Value;
					}
				}
			}
			for (int num17 = 0; num17 < source.TextureVariables.Length; num17++)
			{
				for (int num18 = 0; num18 < this.TextureVariables.Length; num18++)
				{
					if (this.textureVariables[num18].ShowInInspector && source.textureVariables[num17].Name == this.textureVariables[num18].Name)
					{
						this.textureVariables[num18].Value = source.textureVariables[num17].Value;
					}
				}
			}
			for (int num19 = 0; num19 < source.StringVariables.Length; num19++)
			{
				for (int num20 = 0; num20 < this.StringVariables.Length; num20++)
				{
					if (this.stringVariables[num20].ShowInInspector && source.stringVariables[num19].Name == this.stringVariables[num20].Name)
					{
						this.stringVariables[num20].Value = source.stringVariables[num19].Value;
					}
				}
			}
			for (int num21 = 0; num21 < source.ArrayVariables.Length; num21++)
			{
				for (int num22 = 0; num22 < this.ArrayVariables.Length; num22++)
				{
					if (this.arrayVariables[num22].ShowInInspector && source.arrayVariables[num21].Name == this.arrayVariables[num22].Name)
					{
						this.arrayVariables[num22].CopyValues(source.arrayVariables[num21]);
					}
				}
			}
			for (int num23 = 0; num23 < source.EnumVariables.Length; num23++)
			{
				for (int num24 = 0; num24 < this.EnumVariables.Length; num24++)
				{
					if (this.enumVariables[num24].ShowInInspector && source.enumVariables[num23].Name == this.enumVariables[num24].Name)
					{
						this.enumVariables[num24].Value = source.enumVariables[num23].Value;
					}
				}
			}
		}
		public void ApplyVariableValues(SkillVariables source)
		{
			if (source == null)
			{
				return;
			}
			for (int i = 0; i < source.FloatVariables.Length; i++)
			{
				this.floatVariables[i].Value = source.floatVariables[i].Value;
			}
			for (int j = 0; j < source.IntVariables.Length; j++)
			{
				this.intVariables[j].Value = source.intVariables[j].Value;
			}
			for (int k = 0; k < source.BoolVariables.Length; k++)
			{
				this.boolVariables[k].Value = source.boolVariables[k].Value;
			}
			for (int l = 0; l < source.GameObjectVariables.Length; l++)
			{
				this.gameObjectVariables[l].Value = source.gameObjectVariables[l].Value;
			}
			for (int m = 0; m < source.ColorVariables.Length; m++)
			{
				this.colorVariables[m].Value = source.colorVariables[m].Value;
			}
			for (int n = 0; n < source.Vector2Variables.Length; n++)
			{
				this.vector2Variables[n].Value = source.vector2Variables[n].Value;
			}
			for (int num = 0; num < source.Vector3Variables.Length; num++)
			{
				this.vector3Variables[num].Value = source.vector3Variables[num].Value;
			}
			for (int num2 = 0; num2 < source.RectVariables.Length; num2++)
			{
				this.rectVariables[num2].Value = source.rectVariables[num2].Value;
			}
			for (int num3 = 0; num3 < source.QuaternionVariables.Length; num3++)
			{
				this.quaternionVariables[num3].Value = source.quaternionVariables[num3].Value;
			}
			for (int num4 = 0; num4 < source.ObjectVariables.Length; num4++)
			{
				this.objectVariables[num4].Value = source.objectVariables[num4].Value;
			}
			for (int num5 = 0; num5 < source.MaterialVariables.Length; num5++)
			{
				this.materialVariables[num5].Value = source.materialVariables[num5].Value;
			}
			for (int num6 = 0; num6 < source.TextureVariables.Length; num6++)
			{
				this.textureVariables[num6].Value = source.textureVariables[num6].Value;
			}
			for (int num7 = 0; num7 < source.StringVariables.Length; num7++)
			{
				this.stringVariables[num7].Value = source.stringVariables[num7].Value;
			}
			for (int num8 = 0; num8 < source.EnumVariables.Length; num8++)
			{
				this.enumVariables[num8].Value = source.enumVariables[num8].Value;
			}
			for (int num9 = 0; num9 < source.ArrayVariables.Length; num9++)
			{
				this.arrayVariables[num9].CopyValues(source.arrayVariables[num9]);
			}
		}
		public void ApplyVariableValuesCareful(SkillVariables source)
		{
			if (source == null)
			{
				return;
			}
			for (int i = 0; i < source.FloatVariables.Length; i++)
			{
				SkillFloat fsmFloat = this.FindFsmFloat(source.floatVariables[i].Name);
				if (fsmFloat != null)
				{
					fsmFloat.Value = source.floatVariables[i].Value;
				}
			}
			for (int j = 0; j < source.IntVariables.Length; j++)
			{
				SkillInt fsmInt = this.FindFsmInt(source.IntVariables[j].Name);
				if (fsmInt != null)
				{
					fsmInt.Value = source.IntVariables[j].Value;
				}
			}
			for (int k = 0; k < source.BoolVariables.Length; k++)
			{
				SkillBool fsmBool = this.FindFsmBool(source.BoolVariables[k].Name);
				if (fsmBool != null)
				{
					fsmBool.Value = source.BoolVariables[k].Value;
				}
			}
			for (int l = 0; l < source.GameObjectVariables.Length; l++)
			{
				SkillBool fsmBool2 = this.FindFsmBool(source.BoolVariables[l].Name);
				if (fsmBool2 != null)
				{
					fsmBool2.Value = source.BoolVariables[l].Value;
				}
			}
			for (int m = 0; m < source.ColorVariables.Length; m++)
			{
				SkillBool fsmBool3 = this.FindFsmBool(source.BoolVariables[m].Name);
				if (fsmBool3 != null)
				{
					fsmBool3.Value = source.BoolVariables[m].Value;
				}
			}
			for (int n = 0; n < source.Vector2Variables.Length; n++)
			{
				SkillBool fsmBool4 = this.FindFsmBool(source.BoolVariables[n].Name);
				if (fsmBool4 != null)
				{
					fsmBool4.Value = source.BoolVariables[n].Value;
				}
			}
			for (int num = 0; num < source.Vector3Variables.Length; num++)
			{
				SkillBool fsmBool5 = this.FindFsmBool(source.BoolVariables[num].Name);
				if (fsmBool5 != null)
				{
					fsmBool5.Value = source.BoolVariables[num].Value;
				}
			}
			for (int num2 = 0; num2 < source.RectVariables.Length; num2++)
			{
				SkillRect fsmRect = this.FindFsmRect(source.RectVariables[num2].Name);
				if (fsmRect != null)
				{
					fsmRect.Value = source.RectVariables[num2].Value;
				}
			}
			for (int num3 = 0; num3 < source.QuaternionVariables.Length; num3++)
			{
				SkillQuaternion fsmQuaternion = this.FindFsmQuaternion(source.QuaternionVariables[num3].Name);
				if (fsmQuaternion != null)
				{
					fsmQuaternion.Value = source.QuaternionVariables[num3].Value;
				}
			}
			for (int num4 = 0; num4 < source.ObjectVariables.Length; num4++)
			{
				SkillObject fsmObject = this.FindFsmObject(source.ObjectVariables[num4].Name);
				if (fsmObject != null)
				{
					fsmObject.Value = source.ObjectVariables[num4].Value;
				}
			}
			for (int num5 = 0; num5 < source.MaterialVariables.Length; num5++)
			{
				SkillMaterial fsmMaterial = this.FindFsmMaterial(source.MaterialVariables[num5].Name);
				if (fsmMaterial != null)
				{
					fsmMaterial.Value = source.MaterialVariables[num5].Value;
				}
			}
			for (int num6 = 0; num6 < source.TextureVariables.Length; num6++)
			{
				SkillTexture fsmTexture = this.FindFsmTexture(source.TextureVariables[num6].Name);
				if (fsmTexture != null)
				{
					fsmTexture.Value = source.TextureVariables[num6].Value;
				}
			}
			for (int num7 = 0; num7 < source.StringVariables.Length; num7++)
			{
				SkillString fsmString = this.FindFsmString(source.StringVariables[num7].Name);
				if (fsmString != null)
				{
					fsmString.Value = source.StringVariables[num7].Value;
				}
			}
			for (int num8 = 0; num8 < source.EnumVariables.Length; num8++)
			{
				SkillEnum fsmEnum = this.FindFsmEnum(source.EnumVariables[num8].Name);
				if (fsmEnum != null)
				{
					fsmEnum.Value = source.EnumVariables[num8].Value;
				}
			}
			for (int num9 = 0; num9 < source.ArrayVariables.Length; num9++)
			{
				SkillArray fsmArray = this.FindFsmArray(source.ArrayVariables[num9].Name);
				if (fsmArray != null)
				{
					fsmArray.CopyValues(source.arrayVariables[num9]);
				}
			}
		}
		public NamedVariable GetVariable(string name)
		{
			SkillFloat[] array = this.FloatVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillFloat fsmFloat = array[i];
				if (fsmFloat.Name == name)
				{
					NamedVariable result = fsmFloat;
					return result;
				}
			}
			SkillInt[] array2 = this.IntVariables;
			for (int j = 0; j < array2.Length; j++)
			{
				SkillInt fsmInt = array2[j];
				if (fsmInt.Name == name)
				{
					NamedVariable result = fsmInt;
					return result;
				}
			}
			SkillBool[] array3 = this.BoolVariables;
			for (int k = 0; k < array3.Length; k++)
			{
				SkillBool fsmBool = array3[k];
				if (fsmBool.Name == name)
				{
					NamedVariable result = fsmBool;
					return result;
				}
			}
			SkillVector2[] array4 = this.Vector2Variables;
			for (int l = 0; l < array4.Length; l++)
			{
				SkillVector2 fsmVector = array4[l];
				if (fsmVector.Name == name)
				{
					NamedVariable result = fsmVector;
					return result;
				}
			}
			SkillVector3[] array5 = this.Vector3Variables;
			for (int m = 0; m < array5.Length; m++)
			{
				SkillVector3 fsmVector2 = array5[m];
				if (fsmVector2.Name == name)
				{
					NamedVariable result = fsmVector2;
					return result;
				}
			}
			SkillString[] array6 = this.StringVariables;
			for (int n = 0; n < array6.Length; n++)
			{
				SkillString fsmString = array6[n];
				if (fsmString.Name == name)
				{
					NamedVariable result = fsmString;
					return result;
				}
			}
			SkillRect[] array7 = this.RectVariables;
			for (int num = 0; num < array7.Length; num++)
			{
				SkillRect fsmRect = array7[num];
				if (fsmRect.Name == name)
				{
					NamedVariable result = fsmRect;
					return result;
				}
			}
			SkillColor[] array8 = this.ColorVariables;
			for (int num2 = 0; num2 < array8.Length; num2++)
			{
				SkillColor fsmColor = array8[num2];
				if (fsmColor.Name == name)
				{
					NamedVariable result = fsmColor;
					return result;
				}
			}
			SkillMaterial[] array9 = this.MaterialVariables;
			for (int num3 = 0; num3 < array9.Length; num3++)
			{
				SkillMaterial fsmMaterial = array9[num3];
				if (fsmMaterial.Name == name)
				{
					NamedVariable result = fsmMaterial;
					return result;
				}
			}
			SkillTexture[] array10 = this.TextureVariables;
			for (int num4 = 0; num4 < array10.Length; num4++)
			{
				SkillTexture fsmTexture = array10[num4];
				if (fsmTexture.Name == name)
				{
					NamedVariable result = fsmTexture;
					return result;
				}
			}
			SkillObject[] array11 = this.ObjectVariables;
			for (int num5 = 0; num5 < array11.Length; num5++)
			{
				SkillObject fsmObject = array11[num5];
				if (fsmObject.Name == name)
				{
					NamedVariable result = fsmObject;
					return result;
				}
			}
			SkillGameObject[] array12 = this.GameObjectVariables;
			for (int num6 = 0; num6 < array12.Length; num6++)
			{
				SkillGameObject fsmGameObject = array12[num6];
				if (fsmGameObject.Name == name)
				{
					NamedVariable result = fsmGameObject;
					return result;
				}
			}
			SkillQuaternion[] array13 = this.QuaternionVariables;
			for (int num7 = 0; num7 < array13.Length; num7++)
			{
				SkillQuaternion fsmQuaternion = array13[num7];
				if (fsmQuaternion.Name == name)
				{
					NamedVariable result = fsmQuaternion;
					return result;
				}
			}
			SkillEnum[] array14 = this.EnumVariables;
			for (int num8 = 0; num8 < array14.Length; num8++)
			{
				SkillEnum fsmEnum = array14[num8];
				if (fsmEnum.Name == name)
				{
					NamedVariable result = fsmEnum;
					return result;
				}
			}
			SkillArray[] array15 = this.ArrayVariables;
			for (int num9 = 0; num9 < array15.Length; num9++)
			{
				SkillArray fsmArray = array15[num9];
				if (fsmArray.Name == name)
				{
					NamedVariable result = fsmArray;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				array = SkillVariables.GlobalVariables.FloatVariables;
				for (int i = 0; i < array.Length; i++)
				{
					SkillFloat fsmFloat2 = array[i];
					if (fsmFloat2.Name == name)
					{
						NamedVariable result = fsmFloat2;
						return result;
					}
				}
				array2 = SkillVariables.GlobalVariables.IntVariables;
				for (int i = 0; i < array2.Length; i++)
				{
					SkillInt fsmInt2 = array2[i];
					if (fsmInt2.Name == name)
					{
						NamedVariable result = fsmInt2;
						return result;
					}
				}
				array3 = SkillVariables.GlobalVariables.BoolVariables;
				for (int i = 0; i < array3.Length; i++)
				{
					SkillBool fsmBool2 = array3[i];
					if (fsmBool2.Name == name)
					{
						NamedVariable result = fsmBool2;
						return result;
					}
				}
				array4 = SkillVariables.GlobalVariables.Vector2Variables;
				for (int i = 0; i < array4.Length; i++)
				{
					SkillVector2 fsmVector3 = array4[i];
					if (fsmVector3.Name == name)
					{
						NamedVariable result = fsmVector3;
						return result;
					}
				}
				array5 = SkillVariables.GlobalVariables.Vector3Variables;
				for (int i = 0; i < array5.Length; i++)
				{
					SkillVector3 fsmVector4 = array5[i];
					if (fsmVector4.Name == name)
					{
						NamedVariable result = fsmVector4;
						return result;
					}
				}
				array6 = SkillVariables.GlobalVariables.StringVariables;
				for (int i = 0; i < array6.Length; i++)
				{
					SkillString fsmString2 = array6[i];
					if (fsmString2.Name == name)
					{
						NamedVariable result = fsmString2;
						return result;
					}
				}
				array7 = SkillVariables.GlobalVariables.RectVariables;
				for (int i = 0; i < array7.Length; i++)
				{
					SkillRect fsmRect2 = array7[i];
					if (fsmRect2.Name == name)
					{
						NamedVariable result = fsmRect2;
						return result;
					}
				}
				array8 = SkillVariables.GlobalVariables.ColorVariables;
				for (int i = 0; i < array8.Length; i++)
				{
					SkillColor fsmColor2 = array8[i];
					if (fsmColor2.Name == name)
					{
						NamedVariable result = fsmColor2;
						return result;
					}
				}
				array9 = SkillVariables.GlobalVariables.MaterialVariables;
				for (int i = 0; i < array9.Length; i++)
				{
					SkillMaterial fsmMaterial2 = array9[i];
					if (fsmMaterial2.Name == name)
					{
						NamedVariable result = fsmMaterial2;
						return result;
					}
				}
				array10 = SkillVariables.GlobalVariables.TextureVariables;
				for (int i = 0; i < array10.Length; i++)
				{
					SkillTexture fsmTexture2 = array10[i];
					if (fsmTexture2.Name == name)
					{
						NamedVariable result = fsmTexture2;
						return result;
					}
				}
				array11 = SkillVariables.GlobalVariables.ObjectVariables;
				for (int i = 0; i < array11.Length; i++)
				{
					SkillObject fsmObject2 = array11[i];
					if (fsmObject2.Name == name)
					{
						NamedVariable result = fsmObject2;
						return result;
					}
				}
				array12 = SkillVariables.GlobalVariables.GameObjectVariables;
				for (int i = 0; i < array12.Length; i++)
				{
					SkillGameObject fsmGameObject2 = array12[i];
					if (fsmGameObject2.Name == name)
					{
						NamedVariable result = fsmGameObject2;
						return result;
					}
				}
				array13 = SkillVariables.GlobalVariables.QuaternionVariables;
				for (int i = 0; i < array13.Length; i++)
				{
					SkillQuaternion fsmQuaternion2 = array13[i];
					if (fsmQuaternion2.Name == name)
					{
						NamedVariable result = fsmQuaternion2;
						return result;
					}
				}
				array14 = SkillVariables.GlobalVariables.EnumVariables;
				for (int i = 0; i < array14.Length; i++)
				{
					SkillEnum fsmEnum2 = array14[i];
					if (fsmEnum2.Name == name)
					{
						NamedVariable result = fsmEnum2;
						return result;
					}
				}
				array15 = SkillVariables.GlobalVariables.ArrayVariables;
				for (int i = 0; i < array15.Length; i++)
				{
					SkillArray fsmArray2 = array15[i];
					if (fsmArray2.Name == name)
					{
						NamedVariable result = fsmArray2;
						return result;
					}
				}
			}
			return null;
		}
		public SkillFloat GetFsmFloat(string name)
		{
			SkillFloat[] array = this.FloatVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillFloat fsmFloat = array[i];
				if (fsmFloat.Name == name)
				{
					SkillFloat result = fsmFloat;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillFloat[] array2 = SkillVariables.GlobalVariables.FloatVariables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillFloat fsmFloat2 = array2[j];
					if (fsmFloat2.Name == name)
					{
						SkillFloat result = fsmFloat2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillFloat(name);
		}
		public SkillObject GetFsmObject(string name)
		{
			SkillObject[] array = this.ObjectVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillObject fsmObject = array[i];
				if (fsmObject.Name == name)
				{
					SkillObject result = fsmObject;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillObject[] array2 = SkillVariables.GlobalVariables.ObjectVariables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillObject fsmObject2 = array2[j];
					if (fsmObject2.Name == name)
					{
						SkillObject result = fsmObject2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillObject(name);
		}
		public SkillMaterial GetFsmMaterial(string name)
		{
			SkillMaterial[] array = this.MaterialVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillMaterial fsmMaterial = array[i];
				if (fsmMaterial.Name == name)
				{
					SkillMaterial result = fsmMaterial;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillMaterial[] array2 = SkillVariables.GlobalVariables.MaterialVariables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillMaterial fsmMaterial2 = array2[j];
					if (fsmMaterial2.Name == name)
					{
						SkillMaterial result = fsmMaterial2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillMaterial(name);
		}
		public SkillTexture GetFsmTexture(string name)
		{
			SkillTexture[] array = this.TextureVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillTexture fsmTexture = array[i];
				if (fsmTexture.Name == name)
				{
					SkillTexture result = fsmTexture;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillTexture[] array2 = SkillVariables.GlobalVariables.TextureVariables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillTexture fsmTexture2 = array2[j];
					if (fsmTexture2.Name == name)
					{
						SkillTexture result = fsmTexture2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillTexture(name);
		}
		public SkillInt GetFsmInt(string name)
		{
			SkillInt[] array = this.IntVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillInt fsmInt = array[i];
				if (fsmInt.Name == name)
				{
					SkillInt result = fsmInt;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillInt[] array2 = SkillVariables.GlobalVariables.IntVariables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillInt fsmInt2 = array2[j];
					if (fsmInt2.Name == name)
					{
						SkillInt result = fsmInt2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillInt(name);
		}
		public SkillBool GetFsmBool(string name)
		{
			SkillBool[] array = this.BoolVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillBool fsmBool = array[i];
				if (fsmBool.Name == name)
				{
					SkillBool result = fsmBool;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillBool[] array2 = SkillVariables.GlobalVariables.BoolVariables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillBool fsmBool2 = array2[j];
					if (fsmBool2.Name == name)
					{
						SkillBool result = fsmBool2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillBool(name);
		}
		public SkillString GetFsmString(string name)
		{
			SkillString[] array = this.StringVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillString fsmString = array[i];
				if (fsmString.Name == name)
				{
					SkillString result = fsmString;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillString[] array2 = SkillVariables.GlobalVariables.StringVariables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillString fsmString2 = array2[j];
					if (fsmString2.Name == name)
					{
						SkillString result = fsmString2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillString(name);
		}
		public SkillVector2 GetFsmVector2(string name)
		{
			SkillVector2[] array = this.Vector2Variables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillVector2 fsmVector = array[i];
				if (fsmVector.Name == name)
				{
					SkillVector2 result = fsmVector;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillVector2[] array2 = SkillVariables.GlobalVariables.Vector2Variables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillVector2 fsmVector2 = array2[j];
					if (fsmVector2.Name == name)
					{
						SkillVector2 result = fsmVector2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillVector2(name);
		}
		public SkillVector3 GetFsmVector3(string name)
		{
			SkillVector3[] array = this.Vector3Variables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillVector3 fsmVector = array[i];
				if (fsmVector.Name == name)
				{
					SkillVector3 result = fsmVector;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillVector3[] array2 = SkillVariables.GlobalVariables.Vector3Variables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillVector3 fsmVector2 = array2[j];
					if (fsmVector2.Name == name)
					{
						SkillVector3 result = fsmVector2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillVector3(name);
		}
		public SkillRect GetFsmRect(string name)
		{
			SkillRect[] array = this.RectVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillRect fsmRect = array[i];
				if (fsmRect.Name == name)
				{
					SkillRect result = fsmRect;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillRect[] array2 = SkillVariables.GlobalVariables.RectVariables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillRect fsmRect2 = array2[j];
					if (fsmRect2.Name == name)
					{
						SkillRect result = fsmRect2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillRect(name);
		}
		public SkillQuaternion GetFsmQuaternion(string name)
		{
			SkillQuaternion[] array = this.QuaternionVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillQuaternion fsmQuaternion = array[i];
				if (fsmQuaternion.Name == name)
				{
					SkillQuaternion result = fsmQuaternion;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillQuaternion[] array2 = SkillVariables.GlobalVariables.QuaternionVariables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillQuaternion fsmQuaternion2 = array2[j];
					if (fsmQuaternion2.Name == name)
					{
						SkillQuaternion result = fsmQuaternion2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillQuaternion(name);
		}
		public SkillColor GetFsmColor(string name)
		{
			SkillColor[] array = this.ColorVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillColor fsmColor = array[i];
				if (fsmColor.Name == name)
				{
					SkillColor result = fsmColor;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillColor[] array2 = SkillVariables.GlobalVariables.ColorVariables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillColor fsmColor2 = array2[j];
					if (fsmColor2.Name == name)
					{
						SkillColor result = fsmColor2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillColor(name);
		}
		public SkillGameObject GetFsmGameObject(string name)
		{
			SkillGameObject[] array = this.GameObjectVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillGameObject fsmGameObject = array[i];
				if (fsmGameObject.Name == name)
				{
					SkillGameObject result = fsmGameObject;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillGameObject[] array2 = SkillVariables.GlobalVariables.GameObjectVariables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillGameObject fsmGameObject2 = array2[j];
					if (fsmGameObject2.Name == name)
					{
						SkillGameObject result = fsmGameObject2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillGameObject(name);
		}
		public SkillArray GetFsmArray(string name)
		{
			SkillArray[] array = this.ArrayVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillArray fsmArray = array[i];
				if (fsmArray.Name == name)
				{
					SkillArray result = fsmArray;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillArray[] array2 = SkillVariables.GlobalVariables.ArrayVariables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillArray fsmArray2 = array2[j];
					if (fsmArray2.Name == name)
					{
						SkillArray result = fsmArray2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillArray(name);
		}
		public SkillEnum GetFsmEnum(string name)
		{
			SkillEnum[] array = this.EnumVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillEnum fsmEnum = array[i];
				if (fsmEnum.Name == name)
				{
					SkillEnum result = fsmEnum;
					return result;
				}
			}
			if (SkillVariables.GlobalVariables != null)
			{
				SkillEnum[] array2 = SkillVariables.GlobalVariables.EnumVariables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillEnum fsmEnum2 = array2[j];
					if (fsmEnum2.Name == name)
					{
						SkillEnum result = fsmEnum2;
						return result;
					}
				}
			}
			this.LogMissingVariable(name);
			return new SkillEnum(name);
		}
		private void LogMissingVariable(string name)
		{
			if (SkillExecutionStack.ExecutingFsm != null)
			{
				ActionHelpers.LogWarning("Missing Variable: " + name);
			}
		}
		public NamedVariable FindVariable(string name)
		{
			SkillFloat[] array = this.FloatVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillFloat fsmFloat = array[i];
				if (fsmFloat.Name == name)
				{
					NamedVariable result = fsmFloat;
					return result;
				}
			}
			SkillInt[] array2 = this.IntVariables;
			for (int j = 0; j < array2.Length; j++)
			{
				SkillInt fsmInt = array2[j];
				if (fsmInt.Name == name)
				{
					NamedVariable result = fsmInt;
					return result;
				}
			}
			SkillBool[] array3 = this.BoolVariables;
			for (int k = 0; k < array3.Length; k++)
			{
				SkillBool fsmBool = array3[k];
				if (fsmBool.Name == name)
				{
					NamedVariable result = fsmBool;
					return result;
				}
			}
			SkillVector2[] array4 = this.Vector2Variables;
			for (int l = 0; l < array4.Length; l++)
			{
				SkillVector2 fsmVector = array4[l];
				if (fsmVector.Name == name)
				{
					NamedVariable result = fsmVector;
					return result;
				}
			}
			SkillVector3[] array5 = this.Vector3Variables;
			for (int m = 0; m < array5.Length; m++)
			{
				SkillVector3 fsmVector2 = array5[m];
				if (fsmVector2.Name == name)
				{
					NamedVariable result = fsmVector2;
					return result;
				}
			}
			SkillString[] array6 = this.StringVariables;
			for (int n = 0; n < array6.Length; n++)
			{
				SkillString fsmString = array6[n];
				if (fsmString.Name == name)
				{
					NamedVariable result = fsmString;
					return result;
				}
			}
			SkillRect[] array7 = this.RectVariables;
			for (int num = 0; num < array7.Length; num++)
			{
				SkillRect fsmRect = array7[num];
				if (fsmRect.Name == name)
				{
					NamedVariable result = fsmRect;
					return result;
				}
			}
			SkillColor[] array8 = this.ColorVariables;
			for (int num2 = 0; num2 < array8.Length; num2++)
			{
				SkillColor fsmColor = array8[num2];
				if (fsmColor.Name == name)
				{
					NamedVariable result = fsmColor;
					return result;
				}
			}
			SkillMaterial[] array9 = this.MaterialVariables;
			for (int num3 = 0; num3 < array9.Length; num3++)
			{
				SkillMaterial fsmMaterial = array9[num3];
				if (fsmMaterial.Name == name)
				{
					NamedVariable result = fsmMaterial;
					return result;
				}
			}
			SkillTexture[] array10 = this.TextureVariables;
			for (int num4 = 0; num4 < array10.Length; num4++)
			{
				SkillTexture fsmTexture = array10[num4];
				if (fsmTexture.Name == name)
				{
					NamedVariable result = fsmTexture;
					return result;
				}
			}
			SkillObject[] array11 = this.ObjectVariables;
			for (int num5 = 0; num5 < array11.Length; num5++)
			{
				SkillObject fsmObject = array11[num5];
				if (fsmObject.Name == name)
				{
					NamedVariable result = fsmObject;
					return result;
				}
			}
			SkillGameObject[] array12 = this.GameObjectVariables;
			for (int num6 = 0; num6 < array12.Length; num6++)
			{
				SkillGameObject fsmGameObject = array12[num6];
				if (fsmGameObject.Name == name)
				{
					NamedVariable result = fsmGameObject;
					return result;
				}
			}
			SkillQuaternion[] array13 = this.QuaternionVariables;
			for (int num7 = 0; num7 < array13.Length; num7++)
			{
				SkillQuaternion fsmQuaternion = array13[num7];
				if (fsmQuaternion.Name == name)
				{
					NamedVariable result = fsmQuaternion;
					return result;
				}
			}
			SkillEnum[] array14 = this.EnumVariables;
			for (int num8 = 0; num8 < array14.Length; num8++)
			{
				SkillEnum fsmEnum = array14[num8];
				if (fsmEnum.Name == name)
				{
					NamedVariable result = fsmEnum;
					return result;
				}
			}
			SkillArray[] array15 = this.ArrayVariables;
			for (int num9 = 0; num9 < array15.Length; num9++)
			{
				SkillArray fsmArray = array15[num9];
				if (fsmArray.Name == name)
				{
					NamedVariable result = fsmArray;
					return result;
				}
			}
			return null;
		}
		public NamedVariable FindVariable(VariableType type, string name)
		{
			switch (type)
			{
			case VariableType.Unknown:
				break;
			case VariableType.Float:
			{
				SkillFloat[] array = this.FloatVariables;
				for (int i = 0; i < array.Length; i++)
				{
					SkillFloat fsmFloat = array[i];
					if (fsmFloat.Name == name)
					{
						NamedVariable result = fsmFloat;
						return result;
					}
				}
				break;
			}
			case VariableType.Int:
			{
				SkillInt[] array2 = this.IntVariables;
				for (int j = 0; j < array2.Length; j++)
				{
					SkillInt fsmInt = array2[j];
					if (fsmInt.Name == name)
					{
						NamedVariable result = fsmInt;
						return result;
					}
				}
				break;
			}
			case VariableType.Bool:
			{
				SkillBool[] array3 = this.BoolVariables;
				for (int k = 0; k < array3.Length; k++)
				{
					SkillBool fsmBool = array3[k];
					if (fsmBool.Name == name)
					{
						NamedVariable result = fsmBool;
						return result;
					}
				}
				break;
			}
			case VariableType.GameObject:
			{
				SkillGameObject[] array4 = this.GameObjectVariables;
				for (int l = 0; l < array4.Length; l++)
				{
					SkillGameObject fsmGameObject = array4[l];
					if (fsmGameObject.Name == name)
					{
						NamedVariable result = fsmGameObject;
						return result;
					}
				}
				break;
			}
			case VariableType.String:
			{
				SkillString[] array5 = this.StringVariables;
				for (int m = 0; m < array5.Length; m++)
				{
					SkillString fsmString = array5[m];
					if (fsmString.Name == name)
					{
						NamedVariable result = fsmString;
						return result;
					}
				}
				break;
			}
			case VariableType.Vector2:
			{
				SkillVector2[] array6 = this.Vector2Variables;
				for (int n = 0; n < array6.Length; n++)
				{
					SkillVector2 fsmVector = array6[n];
					if (fsmVector.Name == name)
					{
						NamedVariable result = fsmVector;
						return result;
					}
				}
				break;
			}
			case VariableType.Vector3:
			{
				SkillVector3[] array7 = this.Vector3Variables;
				for (int num = 0; num < array7.Length; num++)
				{
					SkillVector3 fsmVector2 = array7[num];
					if (fsmVector2.Name == name)
					{
						NamedVariable result = fsmVector2;
						return result;
					}
				}
				break;
			}
			case VariableType.Color:
			{
				SkillColor[] array8 = this.ColorVariables;
				for (int num2 = 0; num2 < array8.Length; num2++)
				{
					SkillColor fsmColor = array8[num2];
					if (fsmColor.Name == name)
					{
						NamedVariable result = fsmColor;
						return result;
					}
				}
				break;
			}
			case VariableType.Rect:
			{
				SkillRect[] array9 = this.RectVariables;
				for (int num3 = 0; num3 < array9.Length; num3++)
				{
					SkillRect fsmRect = array9[num3];
					if (fsmRect.Name == name)
					{
						NamedVariable result = fsmRect;
						return result;
					}
				}
				break;
			}
			case VariableType.Material:
			{
				SkillMaterial[] array10 = this.MaterialVariables;
				for (int num4 = 0; num4 < array10.Length; num4++)
				{
					SkillMaterial fsmMaterial = array10[num4];
					if (fsmMaterial.Name == name)
					{
						NamedVariable result = fsmMaterial;
						return result;
					}
				}
				break;
			}
			case VariableType.Texture:
			{
				SkillTexture[] array11 = this.TextureVariables;
				for (int num5 = 0; num5 < array11.Length; num5++)
				{
					SkillTexture fsmTexture = array11[num5];
					if (fsmTexture.Name == name)
					{
						NamedVariable result = fsmTexture;
						return result;
					}
				}
				break;
			}
			case VariableType.Quaternion:
			{
				SkillQuaternion[] array12 = this.QuaternionVariables;
				for (int num6 = 0; num6 < array12.Length; num6++)
				{
					SkillQuaternion fsmQuaternion = array12[num6];
					if (fsmQuaternion.Name == name)
					{
						NamedVariable result = fsmQuaternion;
						return result;
					}
				}
				break;
			}
			case VariableType.Object:
			{
				SkillObject[] array13 = this.ObjectVariables;
				for (int num7 = 0; num7 < array13.Length; num7++)
				{
					SkillObject fsmObject = array13[num7];
					if (fsmObject.Name == name)
					{
						NamedVariable result = fsmObject;
						return result;
					}
				}
				break;
			}
			case VariableType.Array:
			{
				SkillArray[] array14 = this.ArrayVariables;
				for (int num8 = 0; num8 < array14.Length; num8++)
				{
					SkillArray fsmArray = array14[num8];
					if (fsmArray.Name == name)
					{
						NamedVariable result = fsmArray;
						return result;
					}
				}
				break;
			}
			case VariableType.Enum:
			{
				SkillEnum[] array15 = this.EnumVariables;
				for (int num9 = 0; num9 < array15.Length; num9++)
				{
					SkillEnum fsmEnum = array15[num9];
					if (fsmEnum.Name == name)
					{
						NamedVariable result = fsmEnum;
						return result;
					}
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("type");
			}
			return null;
		}
		public SkillFloat FindFsmFloat(string name)
		{
			SkillFloat[] array = this.FloatVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillFloat fsmFloat = array[i];
				if (fsmFloat.Name == name)
				{
					return fsmFloat;
				}
			}
			return null;
		}
		public SkillObject FindFsmObject(string name)
		{
			SkillObject[] array = this.ObjectVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillObject fsmObject = array[i];
				if (fsmObject.Name == name)
				{
					return fsmObject;
				}
			}
			return null;
		}
		public SkillMaterial FindFsmMaterial(string name)
		{
			SkillMaterial[] array = this.MaterialVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillMaterial fsmMaterial = array[i];
				if (fsmMaterial.Name == name)
				{
					return fsmMaterial;
				}
			}
			return null;
		}
		public SkillTexture FindFsmTexture(string name)
		{
			SkillTexture[] array = this.TextureVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillTexture fsmTexture = array[i];
				if (fsmTexture.Name == name)
				{
					return fsmTexture;
				}
			}
			return null;
		}
		public SkillInt FindFsmInt(string name)
		{
			SkillInt[] array = this.IntVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillInt fsmInt = array[i];
				if (fsmInt.Name == name)
				{
					return fsmInt;
				}
			}
			return null;
		}
		public SkillBool FindFsmBool(string name)
		{
			SkillBool[] array = this.BoolVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillBool fsmBool = array[i];
				if (fsmBool.Name == name)
				{
					return fsmBool;
				}
			}
			return null;
		}
		public SkillString FindFsmString(string name)
		{
			SkillString[] array = this.StringVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillString fsmString = array[i];
				if (fsmString.Name == name)
				{
					return fsmString;
				}
			}
			return null;
		}
		public SkillVector2 FindFsmVector2(string name)
		{
			SkillVector2[] array = this.Vector2Variables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillVector2 fsmVector = array[i];
				if (fsmVector.Name == name)
				{
					return fsmVector;
				}
			}
			return null;
		}
		public SkillVector3 FindFsmVector3(string name)
		{
			SkillVector3[] array = this.Vector3Variables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillVector3 fsmVector = array[i];
				if (fsmVector.Name == name)
				{
					return fsmVector;
				}
			}
			return null;
		}
		public SkillRect FindFsmRect(string name)
		{
			SkillRect[] array = this.RectVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillRect fsmRect = array[i];
				if (fsmRect.Name == name)
				{
					return fsmRect;
				}
			}
			return null;
		}
		public SkillQuaternion FindFsmQuaternion(string name)
		{
			SkillQuaternion[] array = this.QuaternionVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillQuaternion fsmQuaternion = array[i];
				if (fsmQuaternion.Name == name)
				{
					return fsmQuaternion;
				}
			}
			return null;
		}
		public SkillColor FindFsmColor(string name)
		{
			SkillColor[] array = this.ColorVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillColor fsmColor = array[i];
				if (fsmColor.Name == name)
				{
					return fsmColor;
				}
			}
			return null;
		}
		public SkillGameObject FindFsmGameObject(string name)
		{
			SkillGameObject[] array = this.GameObjectVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillGameObject fsmGameObject = array[i];
				if (fsmGameObject.Name == name)
				{
					return fsmGameObject;
				}
			}
			return null;
		}
		public SkillEnum FindFsmEnum(string name)
		{
			SkillEnum[] array = this.EnumVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillEnum fsmEnum = array[i];
				if (fsmEnum.Name == name)
				{
					return fsmEnum;
				}
			}
			return null;
		}
		public SkillArray FindFsmArray(string name)
		{
			SkillArray[] array = this.ArrayVariables;
			for (int i = 0; i < array.Length; i++)
			{
				SkillArray fsmArray = array[i];
				if (fsmArray.Name == name)
				{
					return fsmArray;
				}
			}
			return null;
		}
	}
}
