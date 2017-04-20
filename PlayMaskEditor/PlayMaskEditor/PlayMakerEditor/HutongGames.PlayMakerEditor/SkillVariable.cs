using HutongGames.Editor;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	public class SkillVariable : IComparable
	{
		public static GUIContent[] VariableTypeNames = new GUIContent[]
		{
			new GUIContent("Float"),
			new GUIContent("Int"),
			new GUIContent("Bool"),
			new GUIContent("GameObject"),
			new GUIContent("String"),
			new GUIContent("Vector2"),
			new GUIContent("Vector3"),
			new GUIContent("Color"),
			new GUIContent("Rect"),
			new GUIContent("Material"),
			new GUIContent("Texture"),
			new GUIContent("Quaternion"),
			new GUIContent("Object"),
			new GUIContent("Array"),
			new GUIContent("Enum")
		};
		private SkillVariables fsmVariables;
		private Object owner;
		private string typeName;
		private bool ownerIsPrefab;
		private bool editingArraySize;
		private int arrayControlID;
		private int tempArraySize;
		private Vector2 arrayScrollViewPos;
		private bool initialized;
		public Object Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
				if (this.owner != null)
				{
					this.ownerIsPrefab = (PrefabUtility.GetPrefabType(this.owner) == 1);
					PlayMakerFSM playMakerFSM = this.owner as PlayMakerFSM;
					if (playMakerFSM != null)
					{
						this.fsmVariables = playMakerFSM.get_Fsm().get_Variables();
					}
					SkillTemplate fsmTemplate = this.owner as SkillTemplate;
					if (fsmTemplate != null)
					{
						this.fsmVariables = fsmTemplate.fsm.get_Variables();
					}
					PlayMakerGlobals playMakerGlobals = this.owner as PlayMakerGlobals;
					if (playMakerGlobals != null)
					{
						this.fsmVariables = playMakerGlobals.get_Variables();
						return;
					}
				}
				else
				{
					Debug.Log("Owner == null!");
					this.ownerIsPrefab = false;
					this.fsmVariables = null;
				}
			}
		}
		public NamedVariable NamedVar
		{
			get;
			set;
		}
		public VariableType Type
		{
			get;
			set;
		}
		public string TypeName
		{
			get
			{
				return this.typeName;
			}
			set
			{
				this.typeName = value;
				this.TypeNameShort = Labels.StripNamespace(this.typeName);
			}
		}
		public string TypeNameShort
		{
			get;
			private set;
		}
		public Type ObjectType
		{
			get;
			set;
		}
		public int CategoryID
		{
			get;
			set;
		}
		public string Category
		{
			get;
			set;
		}
		public float FloatValue
		{
			get
			{
				return ((SkillFloat)this.NamedVar).get_Value();
			}
			set
			{
				((SkillFloat)this.NamedVar).set_Value(value);
			}
		}
		public int IntValue
		{
			get
			{
				return ((SkillInt)this.NamedVar).get_Value();
			}
			set
			{
				((SkillInt)this.NamedVar).set_Value(value);
			}
		}
		public bool BoolValue
		{
			get
			{
				return ((SkillBool)this.NamedVar).get_Value();
			}
			set
			{
				((SkillBool)this.NamedVar).set_Value(value);
			}
		}
		public Object ObjectValue
		{
			get
			{
				return ((SkillObject)this.NamedVar).get_Value();
			}
			set
			{
				((SkillObject)this.NamedVar).set_Value(value);
			}
		}
		public GameObject GameObject
		{
			get
			{
				return ((SkillGameObject)this.NamedVar).get_Value();
			}
			set
			{
				((SkillGameObject)this.NamedVar).set_Value(value);
			}
		}
		public string StringValue
		{
			get
			{
				return ((SkillString)this.NamedVar).get_Value();
			}
			set
			{
				((SkillString)this.NamedVar).set_Value(value);
			}
		}
		public Vector2 Vector2Value
		{
			get
			{
				return ((SkillVector2)this.NamedVar).get_Value();
			}
			set
			{
				((SkillVector2)this.NamedVar).set_Value(value);
			}
		}
		public Vector3 Vector3Value
		{
			get
			{
				return ((SkillVector3)this.NamedVar).get_Value();
			}
			set
			{
				((SkillVector3)this.NamedVar).set_Value(value);
			}
		}
		public Rect RectValue
		{
			get
			{
				return ((SkillRect)this.NamedVar).get_Value();
			}
			set
			{
				((SkillRect)this.NamedVar).set_Value(value);
			}
		}
		public Quaternion QuaternionValue
		{
			get
			{
				return ((SkillQuaternion)this.NamedVar).get_Value();
			}
			set
			{
				((SkillQuaternion)this.NamedVar).set_Value(value);
			}
		}
		public Color ColorValue
		{
			get
			{
				return ((SkillColor)this.NamedVar).get_Value();
			}
			set
			{
				((SkillColor)this.NamedVar).set_Value(value);
			}
		}
		public Enum EnumValue
		{
			get
			{
				return ((SkillEnum)this.NamedVar).get_Value();
			}
			set
			{
				((SkillEnum)this.NamedVar).set_Value(value);
			}
		}
		public object[] ArrayValue
		{
			get
			{
				return ((SkillArray)this.NamedVar).get_Values();
			}
			set
			{
				((SkillArray)this.NamedVar).set_Values(value);
			}
		}
		public string Name
		{
			get
			{
				return this.NamedVar.get_Name();
			}
			set
			{
				this.NamedVar.set_Name(value);
			}
		}
		public string Tooltip
		{
			get
			{
				return this.NamedVar.get_Tooltip();
			}
			set
			{
				this.NamedVar.set_Tooltip(value);
			}
		}
		public bool ShowInInspector
		{
			get
			{
				return this.NamedVar.get_ShowInInspector();
			}
			set
			{
				this.NamedVar.set_ShowInInspector(value);
			}
		}
		public bool NetworkSync
		{
			get
			{
				return this.NamedVar.get_NetworkSync();
			}
			set
			{
				this.NamedVar.set_NetworkSync(value);
			}
		}
		public SkillVariable(Object owner, SkillFloat fsmFloat)
		{
			this.Owner = owner;
			this.NamedVar = fsmFloat;
			this.Type = 0;
			this.FloatValue = fsmFloat.get_Value();
		}
		public SkillVariable(Object owner, SkillInt fsmInt)
		{
			this.Owner = owner;
			this.NamedVar = fsmInt;
			this.Type = 1;
			this.IntValue = fsmInt.get_Value();
		}
		public SkillVariable(Object owner, SkillBool fsmBool)
		{
			this.Owner = owner;
			this.NamedVar = fsmBool;
			this.Type = 2;
			this.BoolValue = fsmBool.get_Value();
		}
		public SkillVariable(Object owner, SkillColor fsmColor)
		{
			this.Owner = owner;
			this.NamedVar = fsmColor;
			this.Type = 7;
			this.ColorValue = fsmColor.get_Value();
		}
		public SkillVariable(Object owner, SkillVector2 fsmVector2)
		{
			this.Owner = owner;
			this.NamedVar = fsmVector2;
			this.Type = 5;
			this.Vector2Value = fsmVector2.get_Value();
		}
		public SkillVariable(Object owner, SkillVector3 fsmVector3)
		{
			this.Owner = owner;
			this.NamedVar = fsmVector3;
			this.Type = 6;
			this.Vector3Value = fsmVector3.get_Value();
		}
		public SkillVariable(Object owner, SkillRect fsmRect)
		{
			this.Owner = owner;
			this.NamedVar = fsmRect;
			this.Type = 8;
			this.RectValue = fsmRect.get_Value();
		}
		public SkillVariable(Object owner, SkillQuaternion fsmQuaternion)
		{
			this.Owner = owner;
			this.NamedVar = fsmQuaternion;
			this.Type = 11;
			this.QuaternionValue = fsmQuaternion.get_Value();
		}
		public SkillVariable(Object owner, SkillGameObject fsmGameObject)
		{
			this.Owner = owner;
			this.NamedVar = fsmGameObject;
			this.Type = 3;
			this.GameObject = fsmGameObject.get_Value();
		}
		public SkillVariable(Object owner, SkillString fsmString)
		{
			this.Owner = owner;
			this.NamedVar = fsmString;
			this.Type = 4;
			this.StringValue = fsmString.get_Value();
		}
		public SkillVariable(Object owner, SkillObject fsmObject)
		{
			this.Owner = owner;
			this.NamedVar = fsmObject;
			this.Type = 12;
			this.TypeName = fsmObject.get_TypeName();
			this.ObjectType = ReflectionUtils.GetGlobalType(this.TypeName);
			this.ObjectValue = fsmObject.get_Value();
		}
		public SkillVariable(Object owner, SkillMaterial fsmObject)
		{
			this.Owner = owner;
			this.NamedVar = fsmObject;
			this.Type = 9;
			this.ObjectValue = fsmObject.get_Value();
		}
		public SkillVariable(Object owner, SkillTexture fsmObject)
		{
			this.Owner = owner;
			this.NamedVar = fsmObject;
			this.Type = 10;
			this.ObjectValue = fsmObject.get_Value();
		}
		public SkillVariable(Object owner, SkillArray fsmArray)
		{
			this.Owner = owner;
			this.NamedVar = fsmArray;
			this.Type = 13;
			this.ObjectType = fsmArray.get_ObjectType();
			this.TypeName = fsmArray.get_ObjectTypeName();
			this.ArrayValue = fsmArray.get_Values();
		}
		public SkillVariable(Object owner, SkillEnum fsmEnum)
		{
			this.Owner = owner;
			this.NamedVar = fsmEnum;
			this.Type = 14;
			this.ObjectType = fsmEnum.get_EnumType();
			this.TypeName = this.ObjectType.get_FullName();
			this.EnumValue = fsmEnum.get_Value();
		}
		public void DoInspectorGUI(GUIContent label = null, bool isAsset = false)
		{
			this.BeginChangeCheck();
			switch (this.Type)
			{
			case -1:
				Debug.LogError("Unknown variable type!");
				break;
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
				this.DoEditorGUI(label, isAsset);
				break;
			case 12:
			{
				ObjectPropertyDrawer objectPropertyDrawer = ObjectPropertyDrawers.GetObjectPropertyDrawer(this.ObjectType);
				this.ObjectValue = ((objectPropertyDrawer != null) ? objectPropertyDrawer.OnGUI(label, this.ObjectValue, !isAsset && !this.ownerIsPrefab, new object[0]) : EditorGUILayout.ObjectField(label, this.ObjectValue, this.ObjectType ?? typeof(Object), !isAsset && !this.ownerIsPrefab, new GUILayoutOption[0]));
				break;
			}
			case 13:
				GUILayout.Label(label, new GUILayoutOption[0]);
				this.EditFsmArraySize();
				this.EditFsmArrayValues(false);
				break;
			case 14:
				this.EnumValue = EditorGUILayout.EnumPopup(label, this.EnumValue, new GUILayoutOption[0]);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			this.EndChangeCheck();
		}
		[Obsolete("Use DoEditorGUI or DoInspectorGUI instead.")]
		public void DoValueGUI(GUIContent label = null, bool isAsset = false)
		{
			this.DoEditorGUI(label, isAsset);
		}
		public void DoEditorGUI(GUIContent label = null, bool isAsset = false)
		{
			if (label == null)
			{
				label = SkillEditorContent.VariableValueLabel;
				label.set_tooltip(this.Type.ToString());
			}
			this.BeginChangeCheck();
			switch (this.Type)
			{
			case 0:
				this.FloatValue = EditorGUILayout.FloatField(label, this.FloatValue, new GUILayoutOption[0]);
				break;
			case 1:
				this.IntValue = EditorGUILayout.IntField(label, this.IntValue, new GUILayoutOption[0]);
				break;
			case 2:
			{
				TrueFalseOption trueFalseOption = TrueFalseOption.False;
				if (this.BoolValue)
				{
					trueFalseOption = TrueFalseOption.True;
				}
				this.BoolValue = ((TrueFalseOption)EditorGUILayout.EnumPopup(label, trueFalseOption, new GUILayoutOption[0]) == TrueFalseOption.True);
				break;
			}
			case 3:
				this.GameObject = (GameObject)EditorGUILayout.ObjectField(label, this.GameObject, typeof(GameObject), !isAsset && !this.ownerIsPrefab, new GUILayoutOption[0]);
				break;
			case 4:
				this.StringValue = EditorGUILayout.TextField(label, this.StringValue, new GUILayoutOption[0]);
				break;
			case 5:
				this.Vector2Value = EditorGUILayout.Vector2Field(label.get_text(), this.Vector2Value, new GUILayoutOption[0]);
				break;
			case 6:
				this.Vector3Value = EditorGUILayout.Vector3Field(label.get_text(), this.Vector3Value, new GUILayoutOption[0]);
				break;
			case 7:
				this.ColorValue = EditorGUILayout.ColorField(label, this.ColorValue, new GUILayoutOption[0]);
				break;
			case 8:
				this.RectValue = EditorGUILayout.RectField(label, this.RectValue, new GUILayoutOption[0]);
				break;
			case 9:
				this.ObjectValue = EditorGUILayout.ObjectField(label, this.ObjectValue, typeof(Material), !isAsset && !this.ownerIsPrefab, new GUILayoutOption[0]);
				break;
			case 10:
				this.ObjectValue = EditorGUILayout.ObjectField(label, this.ObjectValue, typeof(Texture), !isAsset && !this.ownerIsPrefab, new GUILayoutOption[0]);
				break;
			case 11:
				this.QuaternionValue = SkillEditorGUILayout.QuaternionField(label.get_text(), this.QuaternionValue);
				break;
			case 12:
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Object Type", new GUILayoutOption[]
				{
					GUILayout.Width(96f)
				});
				if (GUILayout.Button(SkillEditorContent.TempContent(this.TypeNameShort, this.TypeName), EditorStyles.get_popup(), new GUILayoutOption[0]))
				{
					TypeHelpers.GenerateObjectTypesMenu(this).ShowAsContext();
				}
				GUILayout.EndHorizontal();
				ObjectPropertyDrawer objectPropertyDrawer = ObjectPropertyDrawers.GetObjectPropertyDrawer(this.ObjectType);
				if (objectPropertyDrawer != null)
				{
					this.ObjectValue = objectPropertyDrawer.OnGUI(label, this.ObjectValue, !isAsset && !this.ownerIsPrefab, new object[0]);
				}
				else
				{
					this.ObjectValue = EditorGUILayout.ObjectField(label, this.ObjectValue, this.ObjectType ?? typeof(Object), !isAsset && !this.ownerIsPrefab, new GUILayoutOption[0]);
				}
				break;
			}
			case 13:
			{
				EditorGUI.BeginChangeCheck();
				SkillArray fsmArray = (SkillArray)this.NamedVar;
				VariableType variableType = (VariableType)EditorGUILayout.EnumPopup(new GUIContent("Array Type"), fsmArray.get_ElementType(), new GUILayoutOption[0]);
				if (variableType != fsmArray.get_ElementType())
				{
					fsmArray.SetType(variableType);
					this.ObjectType = fsmArray.get_ObjectType();
					Keyboard.ResetFocus();
				}
				if (fsmArray.get_ElementType() == 12)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label("Object Type", new GUILayoutOption[]
					{
						GUILayout.Width(96f)
					});
					if (GUILayout.Button(SkillEditorContent.TempContent(this.TypeNameShort, this.TypeName), EditorStyles.get_popup(), new GUILayoutOption[0]))
					{
						TypeHelpers.GenerateObjectTypesMenu(this).ShowAsContext();
					}
					GUILayout.EndHorizontal();
				}
				else
				{
					if (fsmArray.get_ElementType() == 14)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label("Enum Type", new GUILayoutOption[]
						{
							GUILayout.Width(96f)
						});
						if (GUILayout.Button(SkillEditorContent.TempContent(this.TypeNameShort, this.TypeName), EditorStyles.get_popup(), new GUILayoutOption[0]))
						{
							TypeHelpers.GenerateEnumTypesMenu(this).ShowAsContext();
						}
						GUILayout.EndHorizontal();
					}
					else
					{
						if (fsmArray.get_ElementType() == 13)
						{
							EditorGUILayout.HelpBox("Nested Arrays are not supported yet!", 3);
							fsmArray.Resize(0);
						}
					}
				}
				this.EditFsmArraySize();
				SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
				this.arrayScrollViewPos = GUILayout.BeginScrollView(this.arrayScrollViewPos, new GUILayoutOption[0]);
				this.EditFsmArrayValues(isAsset);
				GUILayout.EndScrollView();
				SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					fsmArray.SaveChanges();
				}
				break;
			}
			case 14:
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Enum Type", new GUILayoutOption[]
				{
					GUILayout.Width(96f)
				});
				if (GUILayout.Button(SkillEditorContent.TempContent(this.TypeNameShort, this.TypeName), EditorStyles.get_popup(), new GUILayoutOption[0]))
				{
					TypeHelpers.GenerateEnumTypesMenu(this).ShowAsContext();
				}
				GUILayout.EndHorizontal();
				this.EnumValue = EditorGUILayout.EnumPopup(label, this.EnumValue, new GUILayoutOption[0]);
				break;
			}
			this.EndChangeCheck();
		}
		private void EditFsmArraySize()
		{
			SkillArray fsmArray = (SkillArray)this.NamedVar;
			if (this.editingArraySize)
			{
				this.tempArraySize = EditorGUILayout.IntField("Size", this.tempArraySize, new GUILayoutOption[0]);
				if (GUIUtility.get_keyboardControl() != this.arrayControlID || Event.get_current().get_keyCode() == 13)
				{
					fsmArray.Resize(this.tempArraySize);
					this.editingArraySize = false;
					return;
				}
			}
			else
			{
				this.tempArraySize = EditorGUILayout.IntField("Size", fsmArray.get_Length(), new GUILayoutOption[0]);
				if (this.tempArraySize != fsmArray.get_Length())
				{
					this.editingArraySize = true;
					this.arrayControlID = GUIUtility.get_keyboardControl();
				}
			}
		}
		private void BeginChangeCheck()
		{
			if (this.Owner != null)
			{
				UndoUtility.SetSnapshotTarget(this.Owner, Strings.get_Label_Edit_Variable());
			}
			EditorGUI.BeginChangeCheck();
		}
		private void EndChangeCheck()
		{
			if (EditorGUI.EndChangeCheck())
			{
				this.UpdateVariableValue();
				if (this.Owner != null)
				{
					if (this.Owner is PlayMakerGlobals)
					{
						SkillEditor.SaveGlobals();
						return;
					}
					PlayMakerFSM playMakerFSM = this.Owner as PlayMakerFSM;
					if (playMakerFSM != null)
					{
						SkillEditor.SetFsmDirty(playMakerFSM.get_Fsm(), false, false, true);
						return;
					}
					SkillTemplate fsmTemplate = this.Owner as SkillTemplate;
					if (fsmTemplate != null)
					{
						fsmTemplate.fsm.set_UsedInTemplate(fsmTemplate);
						SkillEditor.SetFsmDirty(fsmTemplate.fsm, false, false, true);
					}
				}
			}
		}
		public void EditFsmArrayValues(bool isAsset = false)
		{
			SkillArray fsmArray = (SkillArray)this.NamedVar;
			if (fsmArray.get_Values().Length == 0)
			{
				GUILayout.Label("[Empty]", new GUILayoutOption[0]);
				return;
			}
			EditorGUI.BeginChangeCheck();
			for (int i = 0; i < fsmArray.get_Values().Length; i++)
			{
				string text = "Element " + i;
				switch (fsmArray.get_ElementType())
				{
				case -1:
				case 13:
					break;
				case 0:
					fsmArray.get_Values()[i] = EditorGUILayout.FloatField(text, (float)fsmArray.get_Values()[i], new GUILayoutOption[0]);
					break;
				case 1:
					fsmArray.get_Values()[i] = EditorGUILayout.IntField(text, (int)fsmArray.get_Values()[i], new GUILayoutOption[0]);
					break;
				case 2:
					fsmArray.get_Values()[i] = EditorGUILayout.Toggle(text, (bool)fsmArray.get_Values()[i], new GUILayoutOption[0]);
					break;
				case 3:
					fsmArray.get_Values()[i] = EditorGUILayout.ObjectField(text, (GameObject)fsmArray.get_Values()[i], typeof(GameObject), !isAsset && !this.ownerIsPrefab, new GUILayoutOption[0]);
					break;
				case 4:
					fsmArray.get_Values()[i] = EditorGUILayout.TextField(text, ((string)fsmArray.get_Values()[i]) ?? "", new GUILayoutOption[0]);
					break;
				case 5:
					fsmArray.get_Values()[i] = EditorGUILayout.Vector2Field(text, (Vector2)fsmArray.get_Values()[i], new GUILayoutOption[0]);
					break;
				case 6:
					fsmArray.get_Values()[i] = EditorGUILayout.Vector3Field(text, (Vector3)fsmArray.get_Values()[i], new GUILayoutOption[0]);
					break;
				case 7:
					fsmArray.get_Values()[i] = EditorGUILayout.ColorField(text, (Color)fsmArray.get_Values()[i], new GUILayoutOption[0]);
					break;
				case 8:
					fsmArray.get_Values()[i] = EditorGUILayout.RectField(text, (Rect)fsmArray.get_Values()[i], new GUILayoutOption[0]);
					break;
				case 9:
					fsmArray.get_Values()[i] = EditorGUILayout.ObjectField(text, (Material)fsmArray.get_Values()[i], typeof(Material), !isAsset && !this.ownerIsPrefab, new GUILayoutOption[0]);
					break;
				case 10:
					fsmArray.get_Values()[i] = EditorGUILayout.ObjectField(text, (Texture)fsmArray.get_Values()[i], typeof(Texture), !isAsset && !this.ownerIsPrefab, new GUILayoutOption[0]);
					break;
				case 11:
					fsmArray.get_Values()[i] = SkillEditorGUILayout.QuaternionField(text, (Quaternion)fsmArray.get_Values()[i]);
					break;
				case 12:
					fsmArray.get_Values()[i] = EditorGUILayout.ObjectField(text, fsmArray.get_Values()[i] as Object, fsmArray.get_ObjectType() ?? typeof(Object), !isAsset && !this.ownerIsPrefab, new GUILayoutOption[0]);
					break;
				case 14:
					fsmArray.get_Values()[i] = EditorGUILayout.EnumPopup(text, (Enum)fsmArray.get_Values()[i], new GUILayoutOption[0]);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			if (EditorGUI.EndChangeCheck())
			{
				fsmArray.SaveChanges();
			}
		}
		public void UpdateVariableValue()
		{
			switch (this.Type)
			{
			case 0:
				((SkillFloat)this.NamedVar).set_Value(this.FloatValue);
				return;
			case 1:
				((SkillInt)this.NamedVar).set_Value(this.IntValue);
				return;
			case 2:
				((SkillBool)this.NamedVar).set_Value(this.BoolValue);
				return;
			case 3:
				((SkillGameObject)this.NamedVar).set_Value(this.GameObject);
				return;
			case 4:
				((SkillString)this.NamedVar).set_Value(this.StringValue);
				return;
			case 5:
				((SkillVector2)this.NamedVar).set_Value(this.Vector2Value);
				return;
			case 6:
				((SkillVector3)this.NamedVar).set_Value(this.Vector3Value);
				return;
			case 7:
				((SkillColor)this.NamedVar).set_Value(this.ColorValue);
				return;
			case 8:
				((SkillRect)this.NamedVar).set_Value(this.RectValue);
				return;
			case 9:
				((SkillMaterial)this.NamedVar).set_Value((Material)this.ObjectValue);
				return;
			case 10:
				((SkillTexture)this.NamedVar).set_Value((Texture)this.ObjectValue);
				return;
			case 11:
				((SkillQuaternion)this.NamedVar).set_Value(this.QuaternionValue);
				return;
			case 12:
			{
				SkillObject fsmObject = (SkillObject)this.NamedVar;
				fsmObject.set_Value(this.ObjectValue);
				fsmObject.set_ObjectType(this.ObjectType);
				this.ObjectValue = fsmObject.get_Value();
				return;
			}
			case 13:
			{
				SkillArray fsmArray = (SkillArray)this.NamedVar;
				fsmArray.set_Values(this.ArrayValue);
				fsmArray.set_ObjectType(this.ObjectType);
				return;
			}
			case 14:
			{
				SkillEnum fsmEnum = (SkillEnum)this.NamedVar;
				fsmEnum.set_Value(this.EnumValue);
				fsmEnum.set_EnumType(this.ObjectType);
				this.EnumValue = fsmEnum.get_Value();
				return;
			}
			default:
				return;
			}
		}
		public static void AddVariable(SkillVariables variables, VariableType type, string name, Type objectType = null, VariableType typeConstraint = 0)
		{
			switch (type)
			{
			case 0:
				variables.set_FloatVariables(ArrayUtility.AddAndSort<SkillFloat>(variables.get_FloatVariables(), new SkillFloat(name)));
				break;
			case 1:
				variables.set_IntVariables(ArrayUtility.AddAndSort<SkillInt>(variables.get_IntVariables(), new SkillInt(name)));
				break;
			case 2:
				variables.set_BoolVariables(ArrayUtility.AddAndSort<SkillBool>(variables.get_BoolVariables(), new SkillBool(name)));
				break;
			case 3:
				variables.set_GameObjectVariables(ArrayUtility.AddAndSort<SkillGameObject>(variables.get_GameObjectVariables(), new SkillGameObject(name)));
				break;
			case 4:
				variables.set_StringVariables(ArrayUtility.AddAndSort<SkillString>(variables.get_StringVariables(), new SkillString(name)));
				break;
			case 5:
				variables.set_Vector2Variables(ArrayUtility.AddAndSort<SkillVector2>(variables.get_Vector2Variables(), new SkillVector2(name)));
				break;
			case 6:
				variables.set_Vector3Variables(ArrayUtility.AddAndSort<SkillVector3>(variables.get_Vector3Variables(), new SkillVector3(name)));
				break;
			case 7:
				variables.set_ColorVariables(ArrayUtility.AddAndSort<SkillColor>(variables.get_ColorVariables(), new SkillColor(name)));
				break;
			case 8:
				variables.set_RectVariables(ArrayUtility.AddAndSort<SkillRect>(variables.get_RectVariables(), new SkillRect(name)));
				break;
			case 9:
				variables.set_MaterialVariables(ArrayUtility.AddAndSort<SkillMaterial>(variables.get_MaterialVariables(), new SkillMaterial(name)));
				break;
			case 10:
				variables.set_TextureVariables(ArrayUtility.AddAndSort<SkillTexture>(variables.get_TextureVariables(), new SkillTexture(name)));
				break;
			case 11:
				variables.set_QuaternionVariables(ArrayUtility.AddAndSort<SkillQuaternion>(variables.get_QuaternionVariables(), new SkillQuaternion(name)));
				break;
			case 12:
			{
				SkillObject[] arg_5F_0 = variables.get_ObjectVariables();
				SkillObject fsmObject = new SkillObject(name);
				fsmObject.set_ObjectType(objectType);
				variables.set_ObjectVariables(ArrayUtility.AddAndSort<SkillObject>(arg_5F_0, fsmObject));
				break;
			}
			case 13:
			{
				SkillArray[] arg_1F5_0 = variables.get_ArrayVariables();
				SkillArray fsmArray = new SkillArray(name);
				fsmArray.set_ElementType(typeConstraint);
				fsmArray.set_ObjectType(objectType);
				variables.set_ArrayVariables(ArrayUtility.AddAndSort<SkillArray>(arg_1F5_0, fsmArray));
				break;
			}
			case 14:
			{
				SkillEnum[] arg_1CB_0 = variables.get_EnumVariables();
				SkillEnum fsmEnum = new SkillEnum(name);
				fsmEnum.set_EnumType(objectType);
				variables.set_EnumVariables(ArrayUtility.AddAndSort<SkillEnum>(arg_1CB_0, fsmEnum));
				break;
			}
			}
			variables.set_CategoryIDs(ArrayUtility.Add<int>(variables.get_CategoryIDs(), 0));
			SkillVariable.SetNewVariableCategory(variables, name, "");
			if (variables == SkillVariables.get_GlobalVariables())
			{
				SkillEditor.SaveGlobals();
			}
		}
		public static void AddVariable(SkillVariables variables, NamedVariable variable)
		{
			switch (variable.get_VariableType())
			{
			case 0:
				variables.set_FloatVariables(ArrayUtility.AddAndSort<SkillFloat>(variables.get_FloatVariables(), (SkillFloat)variable));
				break;
			case 1:
				variables.set_IntVariables(ArrayUtility.AddAndSort<SkillInt>(variables.get_IntVariables(), (SkillInt)variable));
				break;
			case 2:
				variables.set_BoolVariables(ArrayUtility.AddAndSort<SkillBool>(variables.get_BoolVariables(), (SkillBool)variable));
				break;
			case 3:
				variables.set_GameObjectVariables(ArrayUtility.AddAndSort<SkillGameObject>(variables.get_GameObjectVariables(), (SkillGameObject)variable));
				break;
			case 4:
				variables.set_StringVariables(ArrayUtility.AddAndSort<SkillString>(variables.get_StringVariables(), (SkillString)variable));
				break;
			case 5:
				variables.set_Vector2Variables(ArrayUtility.AddAndSort<SkillVector2>(variables.get_Vector2Variables(), (SkillVector2)variable));
				break;
			case 6:
				variables.set_Vector3Variables(ArrayUtility.AddAndSort<SkillVector3>(variables.get_Vector3Variables(), (SkillVector3)variable));
				break;
			case 7:
				variables.set_ColorVariables(ArrayUtility.AddAndSort<SkillColor>(variables.get_ColorVariables(), (SkillColor)variable));
				break;
			case 8:
				variables.set_RectVariables(ArrayUtility.AddAndSort<SkillRect>(variables.get_RectVariables(), (SkillRect)variable));
				break;
			case 9:
				variables.set_MaterialVariables(ArrayUtility.AddAndSort<SkillMaterial>(variables.get_MaterialVariables(), (SkillMaterial)variable));
				break;
			case 10:
				variables.set_TextureVariables(ArrayUtility.AddAndSort<SkillTexture>(variables.get_TextureVariables(), (SkillTexture)variable));
				break;
			case 11:
				variables.set_QuaternionVariables(ArrayUtility.AddAndSort<SkillQuaternion>(variables.get_QuaternionVariables(), (SkillQuaternion)variable));
				break;
			case 12:
				variables.set_ObjectVariables(ArrayUtility.AddAndSort<SkillObject>(variables.get_ObjectVariables(), (SkillObject)variable));
				break;
			case 13:
				variables.set_ArrayVariables(ArrayUtility.AddAndSort<SkillArray>(variables.get_ArrayVariables(), (SkillArray)variable));
				break;
			case 14:
				variables.set_EnumVariables(ArrayUtility.AddAndSort<SkillEnum>(variables.get_EnumVariables(), (SkillEnum)variable));
				break;
			}
			variables.set_CategoryIDs(ArrayUtility.Add<int>(variables.get_CategoryIDs(), 0));
			SkillVariable.SetNewVariableCategory(variables, variable.get_Name(), "");
			if (variables == SkillVariables.get_GlobalVariables())
			{
				SkillEditor.SaveGlobals();
			}
		}
		public static void RenameVariable(SkillVariables variables, SkillVariable variable, string name)
		{
			if (variable == null || variable.Name == name)
			{
				return;
			}
			variable.Name = name;
			switch (variable.Type)
			{
			case 0:
				variables.set_FloatVariables(ArrayUtility.Sort<SkillFloat>(variables.get_FloatVariables()));
				return;
			case 1:
				variables.set_IntVariables(ArrayUtility.Sort<SkillInt>(variables.get_IntVariables()));
				return;
			case 2:
				variables.set_BoolVariables(ArrayUtility.Sort<SkillBool>(variables.get_BoolVariables()));
				return;
			case 3:
				variables.set_GameObjectVariables(ArrayUtility.Sort<SkillGameObject>(variables.get_GameObjectVariables()));
				return;
			case 4:
				variables.set_StringVariables(ArrayUtility.Sort<SkillString>(variables.get_StringVariables()));
				return;
			case 5:
				variables.set_Vector2Variables(ArrayUtility.Sort<SkillVector2>(variables.get_Vector2Variables()));
				return;
			case 6:
				variables.set_Vector3Variables(ArrayUtility.Sort<SkillVector3>(variables.get_Vector3Variables()));
				return;
			case 7:
				variables.set_ColorVariables(ArrayUtility.Sort<SkillColor>(variables.get_ColorVariables()));
				return;
			case 8:
				variables.set_RectVariables(ArrayUtility.Sort<SkillRect>(variables.get_RectVariables()));
				return;
			case 9:
				variables.set_MaterialVariables(ArrayUtility.Sort<SkillMaterial>(variables.get_MaterialVariables()));
				return;
			case 10:
				variables.set_TextureVariables(ArrayUtility.Sort<SkillTexture>(variables.get_TextureVariables()));
				return;
			case 11:
				variables.set_QuaternionVariables(ArrayUtility.Sort<SkillQuaternion>(variables.get_QuaternionVariables()));
				return;
			case 12:
				variables.set_ObjectVariables(ArrayUtility.Sort<SkillObject>(variables.get_ObjectVariables()));
				return;
			case 13:
				variables.set_ArrayVariables(ArrayUtility.Sort<SkillArray>(variables.get_ArrayVariables()));
				return;
			case 14:
				variables.set_EnumVariables(ArrayUtility.Sort<SkillEnum>(variables.get_EnumVariables()));
				return;
			default:
				return;
			}
		}
		public int CompareTo(object obj)
		{
			SkillVariable fsmVariable = obj as SkillVariable;
			if (fsmVariable != null)
			{
				return string.Compare(this.Name, fsmVariable.Name, 0);
			}
			return 0;
		}
		public static int CompareByType(SkillVariable var1, SkillVariable var2)
		{
			if (var1 == null)
			{
				if (var2 == null)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (var2 == null)
				{
					return 1;
				}
				string text = var1.Type.ToString();
				string text2 = var2.Type.ToString();
				int num = string.Compare(text, text2, 0);
				if (num == 0)
				{
					return var1.CompareTo(var2);
				}
				return num;
			}
		}
		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (obj.GetType() == typeof(SkillVariable) && this.Equals((SkillVariable)obj)));
		}
		public bool Equals(SkillVariable other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || object.Equals(other.Name, this.Name));
		}
		public override int GetHashCode()
		{
			if (this.Name == null)
			{
				return 0;
			}
			return this.Name.GetHashCode();
		}
		public static List<SkillVariable> GetUnsortedFsmVariableList(Object owner)
		{
			SkillVariables variables = SkillVariable.GetVariables(owner);
			List<SkillVariable> list = new List<SkillVariable>();
			if (variables == null)
			{
				return list;
			}
			SkillObject[] objectVariables = variables.get_ObjectVariables();
			for (int i = 0; i < objectVariables.Length; i++)
			{
				SkillObject fsmObject = objectVariables[i];
				list.Add(new SkillVariable(owner, fsmObject));
			}
			SkillMaterial[] materialVariables = variables.get_MaterialVariables();
			for (int j = 0; j < materialVariables.Length; j++)
			{
				SkillMaterial fsmObject2 = materialVariables[j];
				list.Add(new SkillVariable(owner, fsmObject2));
			}
			SkillTexture[] textureVariables = variables.get_TextureVariables();
			for (int k = 0; k < textureVariables.Length; k++)
			{
				SkillTexture fsmObject3 = textureVariables[k];
				list.Add(new SkillVariable(owner, fsmObject3));
			}
			SkillFloat[] floatVariables = variables.get_FloatVariables();
			for (int l = 0; l < floatVariables.Length; l++)
			{
				SkillFloat fsmFloat = floatVariables[l];
				list.Add(new SkillVariable(owner, fsmFloat));
			}
			SkillInt[] intVariables = variables.get_IntVariables();
			for (int m = 0; m < intVariables.Length; m++)
			{
				SkillInt fsmInt = intVariables[m];
				list.Add(new SkillVariable(owner, fsmInt));
			}
			SkillBool[] boolVariables = variables.get_BoolVariables();
			for (int n = 0; n < boolVariables.Length; n++)
			{
				SkillBool fsmBool = boolVariables[n];
				list.Add(new SkillVariable(owner, fsmBool));
			}
			SkillString[] stringVariables = variables.get_StringVariables();
			for (int num = 0; num < stringVariables.Length; num++)
			{
				SkillString fsmString = stringVariables[num];
				list.Add(new SkillVariable(owner, fsmString));
			}
			SkillGameObject[] gameObjectVariables = variables.get_GameObjectVariables();
			for (int num2 = 0; num2 < gameObjectVariables.Length; num2++)
			{
				SkillGameObject fsmGameObject = gameObjectVariables[num2];
				list.Add(new SkillVariable(owner, fsmGameObject));
			}
			SkillVector2[] vector2Variables = variables.get_Vector2Variables();
			for (int num3 = 0; num3 < vector2Variables.Length; num3++)
			{
				SkillVector2 fsmVector = vector2Variables[num3];
				list.Add(new SkillVariable(owner, fsmVector));
			}
			SkillVector3[] vector3Variables = variables.get_Vector3Variables();
			for (int num4 = 0; num4 < vector3Variables.Length; num4++)
			{
				SkillVector3 fsmVector2 = vector3Variables[num4];
				list.Add(new SkillVariable(owner, fsmVector2));
			}
			SkillRect[] rectVariables = variables.get_RectVariables();
			for (int num5 = 0; num5 < rectVariables.Length; num5++)
			{
				SkillRect fsmRect = rectVariables[num5];
				list.Add(new SkillVariable(owner, fsmRect));
			}
			SkillQuaternion[] quaternionVariables = variables.get_QuaternionVariables();
			for (int num6 = 0; num6 < quaternionVariables.Length; num6++)
			{
				SkillQuaternion fsmQuaternion = quaternionVariables[num6];
				list.Add(new SkillVariable(owner, fsmQuaternion));
			}
			SkillColor[] colorVariables = variables.get_ColorVariables();
			for (int num7 = 0; num7 < colorVariables.Length; num7++)
			{
				SkillColor fsmColor = colorVariables[num7];
				list.Add(new SkillVariable(owner, fsmColor));
			}
			SkillArray[] arrayVariables = variables.get_ArrayVariables();
			for (int num8 = 0; num8 < arrayVariables.Length; num8++)
			{
				SkillArray fsmArray = arrayVariables[num8];
				list.Add(new SkillVariable(owner, fsmArray));
			}
			SkillEnum[] enumVariables = variables.get_EnumVariables();
			for (int num9 = 0; num9 < enumVariables.Length; num9++)
			{
				SkillEnum fsmEnum = enumVariables[num9];
				list.Add(new SkillVariable(owner, fsmEnum));
			}
			if (variables.get_CategoryIDs().Length != list.get_Count())
			{
				int[] array = new int[Mathf.Max(list.get_Count(), variables.get_CategoryIDs().Length)];
				for (int num10 = 0; num10 < variables.get_CategoryIDs().Length; num10++)
				{
					array[num10] = variables.get_CategoryIDs()[num10];
				}
				variables.set_CategoryIDs(array);
			}
			using (List<SkillVariable>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					current.GetCategory();
				}
			}
			return list;
		}
		public static SkillVariables GetVariables(Object owner)
		{
			PlayMakerFSM playMakerFSM = owner as PlayMakerFSM;
			if (playMakerFSM != null)
			{
				return playMakerFSM.get_FsmVariables();
			}
			SkillTemplate fsmTemplate = owner as SkillTemplate;
			if (fsmTemplate != null)
			{
				return fsmTemplate.fsm.get_Variables();
			}
			PlayMakerGlobals playMakerGlobals = owner as PlayMakerGlobals;
			if (playMakerGlobals != null)
			{
				return playMakerGlobals.get_Variables();
			}
			return null;
		}
		public static List<SkillVariable> GetFsmVariableList(Object owner)
		{
			SkillVariables variables = SkillVariable.GetVariables(owner);
			List<SkillVariable> unsortedFsmVariableList = SkillVariable.GetUnsortedFsmVariableList(owner);
			return SkillVariable.SortByName(variables, unsortedFsmVariableList);
		}
		public static List<SkillVariable> SortByName(SkillVariables variables, List<SkillVariable> fsmVariables)
		{
			return Enumerable.ToList<SkillVariable>(Enumerable.OrderBy<SkillVariable, string>(fsmVariables, (SkillVariable i) => i.GetCategory() + "_" + i.Name));
		}
		public static List<SkillVariable> SortByType(SkillVariables variables, List<SkillVariable> fsmVariables)
		{
			return Enumerable.ToList<SkillVariable>(Enumerable.OrderBy<SkillVariable, string>(fsmVariables, (SkillVariable i) => i.GetCategory() + "_" + i.Type.ToString()));
		}
		public static void DeleteVariable(SkillVariables variables, SkillVariable fsmVariable)
		{
			if (variables == null || fsmVariable == null)
			{
				return;
			}
			int variableIndex = SkillVariable.GetVariableIndex(variables, fsmVariable.NamedVar);
			variables.get_CategoryIDs()[variableIndex] = 0;
			variables.set_CategoryIDs(ArrayUtility.RemoveAt<int>(variables.get_CategoryIDs(), variableIndex));
			switch (fsmVariable.Type)
			{
			case 0:
				variables.set_FloatVariables(ArrayUtility.Remove<SkillFloat>(variables.get_FloatVariables(), variables.GetFsmFloat(fsmVariable.Name)));
				return;
			case 1:
				variables.set_IntVariables(ArrayUtility.Remove<SkillInt>(variables.get_IntVariables(), variables.GetFsmInt(fsmVariable.Name)));
				return;
			case 2:
				variables.set_BoolVariables(ArrayUtility.Remove<SkillBool>(variables.get_BoolVariables(), variables.GetFsmBool(fsmVariable.Name)));
				return;
			case 3:
				variables.set_GameObjectVariables(ArrayUtility.Remove<SkillGameObject>(variables.get_GameObjectVariables(), variables.GetFsmGameObject(fsmVariable.Name)));
				return;
			case 4:
				variables.set_StringVariables(ArrayUtility.Remove<SkillString>(variables.get_StringVariables(), variables.GetFsmString(fsmVariable.Name)));
				return;
			case 5:
				variables.set_Vector2Variables(ArrayUtility.Remove<SkillVector2>(variables.get_Vector2Variables(), variables.GetFsmVector2(fsmVariable.Name)));
				return;
			case 6:
				variables.set_Vector3Variables(ArrayUtility.Remove<SkillVector3>(variables.get_Vector3Variables(), variables.GetFsmVector3(fsmVariable.Name)));
				return;
			case 7:
				variables.set_ColorVariables(ArrayUtility.Remove<SkillColor>(variables.get_ColorVariables(), variables.GetFsmColor(fsmVariable.Name)));
				return;
			case 8:
				variables.set_RectVariables(ArrayUtility.Remove<SkillRect>(variables.get_RectVariables(), variables.GetFsmRect(fsmVariable.Name)));
				return;
			case 9:
				variables.set_MaterialVariables(ArrayUtility.Remove<SkillMaterial>(variables.get_MaterialVariables(), variables.GetFsmMaterial(fsmVariable.Name)));
				return;
			case 10:
				variables.set_TextureVariables(ArrayUtility.Remove<SkillTexture>(variables.get_TextureVariables(), variables.GetFsmTexture(fsmVariable.Name)));
				return;
			case 11:
				variables.set_QuaternionVariables(ArrayUtility.Remove<SkillQuaternion>(variables.get_QuaternionVariables(), variables.GetFsmQuaternion(fsmVariable.Name)));
				return;
			case 12:
				variables.set_ObjectVariables(ArrayUtility.Remove<SkillObject>(variables.get_ObjectVariables(), variables.GetFsmObject(fsmVariable.Name)));
				return;
			case 13:
				variables.set_ArrayVariables(ArrayUtility.Remove<SkillArray>(variables.get_ArrayVariables(), variables.GetFsmArray(fsmVariable.Name)));
				return;
			case 14:
				variables.set_EnumVariables(ArrayUtility.Remove<SkillEnum>(variables.get_EnumVariables(), variables.GetFsmEnum(fsmVariable.Name)));
				return;
			default:
				return;
			}
		}
		public static SkillVariable GetVariable(List<SkillVariable> variables, string name)
		{
			using (List<SkillVariable>.Enumerator enumerator = variables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					if (current.Name == name)
					{
						return current;
					}
				}
			}
			return null;
		}
		public static VariableType GetVariableType(List<SkillVariable> variables, string name)
		{
			SkillVariable variable = SkillVariable.GetVariable(variables, name);
			if (variable == null)
			{
				return -1;
			}
			return variable.Type;
		}
		public static NamedVariable ResetVariableReference(INamedVariable variable)
		{
			bool useVariable = variable.get_UseVariable();
			NamedVariable newVariableOfSameType = SkillVariable.GetNewVariableOfSameType(variable);
			newVariableOfSameType.set_UseVariable(useVariable);
			return newVariableOfSameType;
		}
		public static NamedVariable GetNewVariableOfSameType(INamedVariable variable)
		{
			Type type = variable.GetType();
			if (type == typeof(SkillMaterial))
			{
				return new SkillMaterial();
			}
			if (type == typeof(SkillTexture))
			{
				return new SkillTexture();
			}
			if (type == typeof(SkillFloat))
			{
				return new SkillFloat();
			}
			if (type == typeof(SkillInt))
			{
				return new SkillInt();
			}
			if (type == typeof(SkillBool))
			{
				return new SkillBool();
			}
			if (type == typeof(SkillString))
			{
				return new SkillString();
			}
			if (type == typeof(SkillGameObject))
			{
				return new SkillGameObject();
			}
			if (type == typeof(SkillVector2))
			{
				return new SkillVector2();
			}
			if (type == typeof(SkillVector3))
			{
				return new SkillVector3();
			}
			if (type == typeof(SkillRect))
			{
				return new SkillRect();
			}
			if (type == typeof(SkillQuaternion))
			{
				return new SkillQuaternion();
			}
			if (type == typeof(SkillColor))
			{
				return new SkillColor();
			}
			if (type == typeof(SkillArray))
			{
				SkillArray fsmArray = (SkillArray)variable;
				SkillArray fsmArray2 = new SkillArray(fsmArray);
				fsmArray2.set_UseVariable(false);
				fsmArray2.set_Name(null);
				fsmArray2.Resize(0);
				return fsmArray2;
			}
			if (type == typeof(SkillEnum))
			{
				SkillEnum fsmEnum = (SkillEnum)variable;
				SkillEnum fsmEnum2 = new SkillEnum(fsmEnum);
				fsmEnum2.set_UseVariable(false);
				fsmEnum2.set_Name(null);
				return fsmEnum2;
			}
			if (type == typeof(SkillObject))
			{
				SkillObject fsmObject = (SkillObject)variable;
				SkillObject fsmObject2 = new SkillObject(fsmObject);
				fsmObject2.set_UseVariable(false);
				fsmObject2.set_Name(null);
				return fsmObject2;
			}
			Debug.LogError(Strings.get_Error_Unknown_variable_type());
			return null;
		}
		public static VariableType GetVariableType(NamedVariable variable)
		{
			Type type = variable.GetType();
			if (type == typeof(SkillMaterial))
			{
				return 9;
			}
			if (type == typeof(SkillTexture))
			{
				return 10;
			}
			if (type == typeof(SkillFloat))
			{
				return 0;
			}
			if (type == typeof(SkillInt))
			{
				return 1;
			}
			if (type == typeof(SkillBool))
			{
				return 2;
			}
			if (type == typeof(SkillString))
			{
				return 4;
			}
			if (type == typeof(SkillGameObject))
			{
				return 3;
			}
			if (type == typeof(SkillVector2))
			{
				return 5;
			}
			if (type == typeof(SkillVector3))
			{
				return 6;
			}
			if (type == typeof(SkillRect))
			{
				return 8;
			}
			if (type == typeof(SkillQuaternion))
			{
				return 11;
			}
			if (type == typeof(SkillColor))
			{
				return 7;
			}
			if (type == typeof(SkillObject))
			{
				return 12;
			}
			if (type == typeof(SkillEnum))
			{
				return 14;
			}
			if (type == typeof(SkillArray))
			{
				return 13;
			}
			Debug.LogError(Strings.get_Error_Unknown_variable_type());
			return -1;
		}
		public static bool CanNetworkSync(NamedVariable variable)
		{
			Type type = variable.GetType();
			return type == typeof(SkillFloat) || type == typeof(SkillInt) || type == typeof(SkillBool) || type == typeof(SkillVector2) || type == typeof(SkillVector3) || type == typeof(SkillQuaternion) || type == typeof(SkillString) || type == typeof(SkillColor);
		}
		public static bool VariableNameUsed(List<SkillVariable> variables, string name)
		{
			using (List<SkillVariable>.Enumerator enumerator = variables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					if (current.Name == name)
					{
						return true;
					}
				}
			}
			return false;
		}
		public static string GetUniqueVariableName(List<SkillVariable> variables, string name)
		{
			int num = 2;
			string text = name;
			while (SkillVariable.VariableNameUsed(variables, name))
			{
				name = text + " " + num;
				num++;
			}
			return name;
		}
		public static int GetCategoryIndex(SkillVariables variables, string category)
		{
			int num = Array.FindIndex<string>(variables.get_Categories(), (string i) => i == category);
			if (num != -1)
			{
				return num;
			}
			variables.set_Categories(ArrayUtility.Add<string>(variables.get_Categories(), category));
			return variables.get_Categories().Length - 1;
		}
		private static int GetVariableIndex(SkillVariables variables, NamedVariable variable)
		{
			NamedVariable[] allNamedVariables = variables.GetAllNamedVariables();
			return ArrayUtility.FindIndex<NamedVariable>(allNamedVariables, (NamedVariable i) => i == variable);
		}
		public static string GetCategory(SkillVariables variables, NamedVariable variable)
		{
			int variableIndex = SkillVariable.GetVariableIndex(variables, variable);
			int num = variables.get_CategoryIDs()[variableIndex];
			return variables.get_Categories()[num];
		}
		public string GetCategory()
		{
			if (!this.initialized)
			{
				int variableIndex = SkillVariable.GetVariableIndex(this.fsmVariables, this.NamedVar);
				if (variableIndex < 0 || variableIndex >= this.fsmVariables.get_CategoryIDs().Length)
				{
					this.CategoryID = 0;
					this.Category = "";
				}
				else
				{
					this.CategoryID = this.fsmVariables.get_CategoryIDs()[variableIndex];
					if (this.CategoryID >= this.fsmVariables.get_Categories().Length)
					{
						Debug.LogError("Fixed bad variable category index!");
						this.fsmVariables.get_CategoryIDs()[variableIndex] = 0;
						this.CategoryID = 0;
						this.Category = "";
					}
					else
					{
						this.Category = this.fsmVariables.get_Categories()[this.CategoryID];
					}
				}
				this.initialized = true;
			}
			return this.Category;
		}
		public void SetCategory(string category)
		{
			this.Category = category;
			this.CategoryID = SkillVariable.GetCategoryIndex(this.fsmVariables, category);
			SkillVariable.SetVariableCategory(this.fsmVariables, this.NamedVar, category);
			this.initialized = true;
		}
		public static void SetNewVariableCategory(SkillVariables variables, string variableName, string category)
		{
			int variableIndex = SkillVariable.GetVariableIndex(variables, variables.GetVariable(variableName));
			int categoryIndex = SkillVariable.GetCategoryIndex(variables, category);
			variables.get_CategoryIDs()[variableIndex] = categoryIndex;
		}
		public static void SetVariableCategory(SkillVariables variables, NamedVariable variable, string category)
		{
			int variableIndex = SkillVariable.GetVariableIndex(variables, variable);
			int categoryIndex = SkillVariable.GetCategoryIndex(variables, category);
			variables.get_CategoryIDs()[variableIndex] = categoryIndex;
		}
		public static void RemapVariableCategories(SkillVariables variables, List<SkillVariable> fsmVariables)
		{
			using (List<SkillVariable>.Enumerator enumerator = fsmVariables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVariable current = enumerator.get_Current();
					current.RemapVariableCategory();
				}
			}
		}
		public void RemapVariableCategory()
		{
			this.CategoryID = SkillVariable.GetCategoryIndex(this.fsmVariables, this.Category);
			int variableIndex = SkillVariable.GetVariableIndex(this.fsmVariables, this.NamedVar);
			int categoryIndex = SkillVariable.GetCategoryIndex(this.fsmVariables, this.Category);
			if (variableIndex >= 0 && variableIndex < this.fsmVariables.get_CategoryIDs().Length)
			{
				this.fsmVariables.get_CategoryIDs()[variableIndex] = categoryIndex;
			}
		}
		public static bool FsmHasVariable(Skill fsm, string name)
		{
			List<SkillVariable> fsmVariableList = SkillVariable.GetFsmVariableList(fsm.get_Owner());
			return Enumerable.Any<SkillVariable>(fsmVariableList, (SkillVariable variable) => variable.Name == name);
		}
		public static Type GetVariableType(UIHint hint)
		{
			switch (hint)
			{
			case 17:
				return typeof(SkillFloat);
			case 18:
				return typeof(SkillInt);
			case 19:
				return typeof(SkillBool);
			case 20:
				return typeof(SkillString);
			case 21:
				return typeof(SkillVector3);
			case 22:
				return typeof(SkillGameObject);
			case 23:
				return typeof(SkillColor);
			case 24:
				return typeof(SkillRect);
			case 25:
				return typeof(SkillMaterial);
			case 26:
				return typeof(SkillTexture);
			case 27:
				return typeof(SkillQuaternion);
			case 28:
				return typeof(SkillObject);
			case 29:
				return typeof(SkillVector2);
			case 30:
				return typeof(SkillEnum);
			case 31:
				return typeof(SkillArray);
			default:
				Debug.LogError(string.Format(Strings.get_Error_Unrecognized_variable_type__(), hint));
				return null;
			}
		}
	}
}
