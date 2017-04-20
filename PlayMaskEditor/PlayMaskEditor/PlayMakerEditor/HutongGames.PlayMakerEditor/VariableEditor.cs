using HutongGames.Editor;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public static class VariableEditor
	{
		private static object copiedValue;
		private static SkillEnum editingFsmEnumType;
		public static bool DebugVariables
		{
			get;
			set;
		}
		private static bool VariableToggle(bool useVariable)
		{
			GUI.set_backgroundColor(Color.get_white());
			SkillEditorContent.VariableButton.set_tooltip((DragAndDropManager.mode == DragAndDropManager.DragMode.None) ? Strings.get_Tooltip_Use_Variable() : null);
			return GUILayout.Toggle(useVariable, SkillEditorContent.VariableButton, SkillEditorStyles.MiniButtonPadded, new GUILayoutOption[0]);
		}
		private static NamedVariable VariableToggle(NamedVariable variable, string label)
		{
			bool flag = VariableEditor.VariableToggle(variable.get_UseVariable());
			if (flag != variable.get_UseVariable())
			{
				if (!flag)
				{
					return SkillVariable.GetNewVariableOfSameType(variable);
				}
				variable.set_UseVariable(true);
				variable.set_Name(null);
				if (EditorGUI.get_actionKey())
				{
					return EditorCommands.AddVariable(SkillVariable.GetVariableType(variable), label.Trim(new char[]
					{
						'*'
					}).ToCamelCase());
				}
			}
			return variable;
		}
		private static void EndVariableEditor(INamedVariable variable)
		{
			EditorGUILayout.EndHorizontal();
			if (variable != null && VariableEditor.DebugVariables && variable.get_UseVariable() && !string.IsNullOrEmpty(variable.get_Name()))
			{
				SkillEditorGUILayout.ReadonlyTextField(variable.ToString(), new GUILayoutOption[0]);
			}
		}
		[Localizable(false)]
		private static void DoVariableContextMenu(INamedVariable variable)
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Copy_Value()), false, new GenericMenu.MenuFunction2(VariableEditor.CopyVariableValue), variable);
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Paste_Value()), false, new GenericMenu.MenuFunction2(VariableEditor.PasteVariableValue), variable);
			genericMenu.ShowAsContext();
		}
		private static void CopyVariableValue(object userdata)
		{
			NamedVariable namedVariable = (NamedVariable)userdata;
			VariableEditor.copiedValue = namedVariable.get_RawValue();
		}
		private static void PasteVariableValue(object userdata)
		{
			NamedVariable namedVariable = (NamedVariable)userdata;
			namedVariable.set_RawValue(VariableEditor.copiedValue);
		}
		public static bool IsReallyAssignableFrom(this Type t, Type other)
		{
			return t.IsAssignableFrom(other) || t.GetMethod("op_Implicit", new Type[]
			{
				other
			}) != null;
		}
		private static SkillVar DoFsmVarPopup(GUIContent label, Skill fsm, SkillVar fsmVar, VariableType typeConstraint, Type objectConstraint)
		{
			ActionEditor.DoVariableSelector(label, fsm, fsmVar.get_Type(), fsmVar.get_NamedVar(), typeConstraint, objectConstraint);
			fsmVar.useVariable = true;
			return fsmVar;
		}
		public static SkillVar FsmVarPopup(GUIContent label, Skill fsm, SkillVar fsmVar, VariableType variableType = -1, Type objectConstraint = null)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmVar = VariableEditor.DoFsmVarPopup(label, fsm, fsmVar, variableType, objectConstraint);
			VariableEditor.EndVariableEditor(fsmVar.get_NamedVar());
			return fsmVar;
		}
		private static SkillFloat DoFsmFloatPopup(GUIContent label, Skill fsm, SkillFloat fsmFloat)
		{
			ActionEditor.DoVariableSelector(label, fsm, 0, fsmFloat, -1, null);
			fsmFloat.set_UseVariable(true);
			return fsmFloat;
		}
		public static SkillFloat FsmFloatPopup(GUIContent label, Skill fsm, SkillFloat fsmFloat)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmFloat = VariableEditor.DoFsmFloatPopup(label, fsm, fsmFloat);
			VariableEditor.EndVariableEditor(fsmFloat);
			return fsmFloat;
		}
		public static SkillFloat FsmFloatField(GUIContent label, Skill fsm, SkillFloat fsmFloat)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (fsmFloat.get_UseVariable())
			{
				fsmFloat = VariableEditor.DoFsmFloatPopup(label, fsm, fsmFloat);
			}
			else
			{
				fsmFloat.set_Value(EditorGUILayout.FloatField(label, fsmFloat.get_Value(), new GUILayoutOption[0]));
			}
			fsmFloat = (SkillFloat)VariableEditor.VariableToggle(fsmFloat, label.get_text());
			VariableEditor.EndVariableEditor(fsmFloat);
			return fsmFloat;
		}
		public static SkillFloat FsmFloatSlider(GUIContent label, Skill fsm, SkillFloat fsmFloat, float minSliderValue, float maxSliderValue)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (fsmFloat.get_UseVariable())
			{
				fsmFloat = VariableEditor.DoFsmFloatPopup(label, fsm, fsmFloat);
			}
			else
			{
				fsmFloat.set_Value(SkillEditorGUILayout.FloatSlider(label, fsmFloat.get_Value(), minSliderValue, maxSliderValue));
			}
			fsmFloat = (SkillFloat)VariableEditor.VariableToggle(fsmFloat, label.get_text());
			VariableEditor.EndVariableEditor(fsmFloat);
			return fsmFloat;
		}
		private static SkillInt DoFsmIntPopup(GUIContent label, Skill fsm, SkillInt fsmInt)
		{
			ActionEditor.DoVariableSelector(label, fsm, 1, fsmInt, -1, null);
			fsmInt.set_UseVariable(true);
			return fsmInt;
		}
		public static SkillInt FsmIntPopup(GUIContent label, Skill fsm, SkillInt fsmInt)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmInt = VariableEditor.DoFsmIntPopup(label, fsm, fsmInt);
			VariableEditor.EndVariableEditor(fsmInt);
			return fsmInt;
		}
		public static SkillInt FsmIntField(GUIContent label, Skill fsm, SkillInt fsmInt, object[] attributes)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (fsmInt.get_UseVariable())
			{
				fsmInt = VariableEditor.DoFsmIntPopup(label, fsm, fsmInt);
			}
			else
			{
				UIHint uIHint = CustomAttributeHelpers.GetUIHint(attributes);
				UIHint uIHint2 = uIHint;
				if (uIHint2 == 8)
				{
					fsmInt.set_Value(EditorGUILayout.LayerField(label, Convert.ToInt32(fsmInt.get_Value(), CultureInfo.get_CurrentCulture()), new GUILayoutOption[0]));
				}
				else
				{
					fsmInt.set_Value(EditorGUILayout.IntField(label, fsmInt.get_Value(), new GUILayoutOption[0]));
				}
			}
			fsmInt = (SkillInt)VariableEditor.VariableToggle(fsmInt, label.get_text());
			VariableEditor.EndVariableEditor(fsmInt);
			return fsmInt;
		}
		private static SkillBool DoFsmBoolPopup(GUIContent label, Skill fsm, SkillBool fsmBool)
		{
			ActionEditor.DoVariableSelector(label, fsm, 2, fsmBool, -1, null);
			fsmBool.set_UseVariable(true);
			return fsmBool;
		}
		public static SkillBool FsmBoolPopup(GUIContent label, Skill fsm, SkillBool fsmBool)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmBool = VariableEditor.DoFsmBoolPopup(label, fsm, fsmBool);
			VariableEditor.EndVariableEditor(fsmBool);
			return fsmBool;
		}
		public static SkillBool FsmBoolField(GUIContent label, Skill fsm, SkillBool fsmBool)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (fsmBool.get_UseVariable())
			{
				fsmBool = VariableEditor.DoFsmBoolPopup(label, fsm, fsmBool);
			}
			else
			{
				fsmBool.set_Value(EditorGUILayout.Toggle(label, fsmBool.get_Value(), new GUILayoutOption[0]));
			}
			fsmBool = (SkillBool)VariableEditor.VariableToggle(fsmBool, label.get_text());
			VariableEditor.EndVariableEditor(fsmBool);
			return fsmBool;
		}
		private static SkillString DoFsmStringPopup(GUIContent label, Skill fsm, SkillString fsmString)
		{
			ActionEditor.DoVariableSelector(label, fsm, 4, fsmString, -1, null);
			fsmString.set_UseVariable(true);
			return fsmString;
		}
		public static SkillString FsmStringPopup(GUIContent label, Skill fsm, SkillString fsmString)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmString = VariableEditor.DoFsmStringPopup(label, fsm, fsmString);
			VariableEditor.EndVariableEditor(fsmString);
			return fsmString;
		}
		public static SkillString FsmStringField(GUIContent label, Skill fsm, SkillString fsmString, object[] attributes)
		{
			if (fsmString.get_UseVariable())
			{
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				fsmString = VariableEditor.DoFsmStringPopup(label, fsm, fsmString);
			}
			else
			{
				if (fsmString.get_Value() == null)
				{
					fsmString.set_Value(string.Empty);
				}
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				UIHint uIHint = CustomAttributeHelpers.GetUIHint(attributes);
				switch (uIHint)
				{
				case 1:
				{
					GUILayout.BeginVertical(new GUILayoutOption[0]);
					GUILayout.Label(label, new GUILayoutOption[0]);
					Rect rect = GUILayoutUtility.GetRect(SkillEditorContent.TempContent(fsmString.get_Value(), ""), SkillEditorStyles.TextArea, new GUILayoutOption[]
					{
						GUILayout.MinHeight(44f)
					});
					rect.set_width(340f);
					fsmString.set_Value(EditorGUI.TextArea(rect, fsmString.get_Value()));
					GUILayout.EndVertical();
					goto IL_363;
				}
				case 2:
					fsmString.set_Value(ActionEditor.EditComponentName(label, fsmString.get_Value(), typeof(Behaviour)));
					goto IL_363;
				case 3:
					fsmString.set_Value(ActionEditor.EditComponentName(label, fsmString.get_Value(), typeof(MonoBehaviour)));
					ActionEditor.TrySetBehaviourContext(fsmString.get_Value());
					goto IL_363;
				case 4:
					fsmString.set_Value(ActionEditor.EditMethodName(label, fsmString.get_Value(), false));
					goto IL_363;
				case 6:
					fsmString.set_Value(EditorGUILayout.TextField(label, fsmString.get_Value(), new GUILayoutOption[0]));
					ActionEditor.AnimationNamePopup(fsmString);
					goto IL_363;
				case 7:
					fsmString.set_Value(EditorGUILayout.TagField(label, fsmString.get_Value(), new GUILayoutOption[0]));
					goto IL_363;
				case 8:
					StringEditor.LayerNamePopup(label, fsmString, null, null);
					goto IL_363;
				case 11:
				{
					fsmString.set_Value(EditorGUILayout.TextField(label, fsmString.get_Value(), new GUILayoutOption[0]));
					EditorGUI.BeginChangeCheck();
					string text = SkillEditorGUILayout.ScriptListPopup();
					if (EditorGUI.EndChangeCheck() && text != "")
					{
						fsmString.set_Value(text);
						goto IL_363;
					}
					goto IL_363;
				}
				case 15:
					fsmString.set_Value(EditorGUILayout.TextField(label, fsmString.get_Value(), new GUILayoutOption[0]));
					ActionEditor.FsmNamePopup(fsmString);
					goto IL_363;
				case 16:
					fsmString.set_Value(ActionEditor.EditFsmEvent(label, fsmString.get_Value()));
					goto IL_363;
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
					fsmString.set_Value(EditorGUILayout.TextField(label, fsmString.get_Value(), new GUILayoutOption[0]));
					ActionEditor.VariablePopup(fsmString, uIHint);
					goto IL_363;
				case 32:
					fsmString.set_Value(EditorGUILayout.TextField(label, fsmString.get_Value(), new GUILayoutOption[0]));
					ActionEditor.AnimatorFloatPopup(fsmString);
					goto IL_363;
				case 33:
					fsmString.set_Value(EditorGUILayout.TextField(label, fsmString.get_Value(), new GUILayoutOption[0]));
					ActionEditor.AnimatorBoolPopup(fsmString);
					goto IL_363;
				case 34:
					fsmString.set_Value(EditorGUILayout.TextField(label, fsmString.get_Value(), new GUILayoutOption[0]));
					ActionEditor.AnimatorIntPopup(fsmString);
					goto IL_363;
				case 35:
					fsmString.set_Value(EditorGUILayout.TextField(label, fsmString.get_Value(), new GUILayoutOption[0]));
					ActionEditor.AnimatorTriggerPopup(fsmString);
					goto IL_363;
				case 36:
					StringEditor.SortingLayerNamePopup(label, fsmString, null, null);
					goto IL_363;
				}
				fsmString.set_Value(EditorGUILayout.TextField(label, fsmString.get_Value(), new GUILayoutOption[0]));
			}
			IL_363:
			fsmString = (SkillString)VariableEditor.VariableToggle(fsmString, label.get_text());
			VariableEditor.EndVariableEditor(fsmString);
			return fsmString;
		}
		private static SkillRect DoFsmRectPopup(GUIContent label, Skill fsm, SkillRect fsmRect)
		{
			ActionEditor.DoVariableSelector(label, fsm, 8, fsmRect, -1, null);
			fsmRect.set_UseVariable(true);
			return fsmRect;
		}
		public static SkillRect FsmRectPopup(GUIContent label, Skill fsm, SkillRect fsmRect)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmRect = VariableEditor.DoFsmRectPopup(label, fsm, fsmRect);
			VariableEditor.EndVariableEditor(fsmRect);
			return fsmRect;
		}
		public static SkillRect FsmRectField(GUIContent label, Skill fsm, SkillRect fsmRect)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (fsmRect.get_UseVariable())
			{
				fsmRect = VariableEditor.DoFsmRectPopup(label, fsm, fsmRect);
			}
			else
			{
				fsmRect.set_Value(EditorGUILayout.RectField(label.get_text(), fsmRect.get_Value(), new GUILayoutOption[0]));
			}
			fsmRect = (SkillRect)VariableEditor.VariableToggle(fsmRect, label.get_text());
			VariableEditor.EndVariableEditor(fsmRect);
			return fsmRect;
		}
		private static SkillQuaternion DoFsmQuaternionPopup(GUIContent label, Skill fsm, SkillQuaternion fsmQauternion)
		{
			ActionEditor.DoVariableSelector(label, fsm, 11, fsmQauternion, -1, null);
			fsmQauternion.set_UseVariable(true);
			return fsmQauternion;
		}
		public static SkillQuaternion FsmQuaternionPopup(GUIContent label, Skill fsm, SkillQuaternion fsmQauternion)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmQauternion = VariableEditor.DoFsmQuaternionPopup(label, fsm, fsmQauternion);
			VariableEditor.EndVariableEditor(fsmQauternion);
			return fsmQauternion;
		}
		public static SkillQuaternion FsmQuaternionField(GUIContent label, Skill fsm, SkillQuaternion fsmQauternion)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (fsmQauternion.get_UseVariable())
			{
				fsmQauternion = VariableEditor.DoFsmQuaternionPopup(label, fsm, fsmQauternion);
			}
			else
			{
				Vector3 vector = EditorGUILayout.Vector3Field(label.get_text(), fsmQauternion.get_Value().get_eulerAngles(), new GUILayoutOption[0]);
				if (vector != fsmQauternion.get_Value().get_eulerAngles())
				{
					fsmQauternion.set_Value(Quaternion.Euler(vector));
				}
			}
			fsmQauternion = (SkillQuaternion)VariableEditor.VariableToggle(fsmQauternion, label.get_text());
			VariableEditor.EndVariableEditor(fsmQauternion);
			return fsmQauternion;
		}
		private static SkillVector2 DoFsmVector2Popup(GUIContent label, Skill fsm, SkillVector2 fsmVector2)
		{
			ActionEditor.DoVariableSelector(label, fsm, 5, fsmVector2, -1, null);
			fsmVector2.set_UseVariable(true);
			return fsmVector2;
		}
		public static SkillVector2 FsmVector2Popup(GUIContent label, Skill fsm, SkillVector2 fsmVector2)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmVector2 = VariableEditor.DoFsmVector2Popup(label, fsm, fsmVector2);
			VariableEditor.EndVariableEditor(fsmVector2);
			return fsmVector2;
		}
		public static SkillVector2 FsmVector2Field(GUIContent label, Skill fsm, SkillVector2 fsmVector2)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (fsmVector2.get_UseVariable())
			{
				fsmVector2 = VariableEditor.DoFsmVector2Popup(label, fsm, fsmVector2);
			}
			else
			{
				fsmVector2.set_Value(EditorGUILayout.Vector2Field(label.get_text(), fsmVector2.get_Value(), new GUILayoutOption[0]));
			}
			fsmVector2 = (SkillVector2)VariableEditor.VariableToggle(fsmVector2, label.get_text());
			VariableEditor.EndVariableEditor(fsmVector2);
			return fsmVector2;
		}
		private static SkillVector3 DoFsmVector3Popup(GUIContent label, Skill fsm, SkillVector3 fsmVector3)
		{
			ActionEditor.DoVariableSelector(label, fsm, 6, fsmVector3, -1, null);
			fsmVector3.set_UseVariable(true);
			return fsmVector3;
		}
		public static SkillVector3 FsmVector3Popup(GUIContent label, Skill fsm, SkillVector3 fsmVector3)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmVector3 = VariableEditor.DoFsmVector3Popup(label, fsm, fsmVector3);
			VariableEditor.EndVariableEditor(fsmVector3);
			return fsmVector3;
		}
		public static SkillVector3 FsmVector3Field(GUIContent label, Skill fsm, SkillVector3 fsmVector3)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (fsmVector3.get_UseVariable())
			{
				fsmVector3 = VariableEditor.DoFsmVector3Popup(label, fsm, fsmVector3);
			}
			else
			{
				fsmVector3.set_Value(EditorGUILayout.Vector3Field(label.get_text(), fsmVector3.get_Value(), new GUILayoutOption[0]));
			}
			fsmVector3 = (SkillVector3)VariableEditor.VariableToggle(fsmVector3, label.get_text());
			VariableEditor.EndVariableEditor(fsmVector3);
			return fsmVector3;
		}
		private static SkillColor DoFsmColorPopup(GUIContent label, Skill fsm, SkillColor fsmColor)
		{
			ActionEditor.DoVariableSelector(label, fsm, 7, fsmColor, -1, null);
			fsmColor.set_UseVariable(true);
			return fsmColor;
		}
		public static SkillColor FsmColorPopup(GUIContent label, Skill fsm, SkillColor fsmColor)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmColor = VariableEditor.DoFsmColorPopup(label, fsm, fsmColor);
			VariableEditor.EndVariableEditor(fsmColor);
			return fsmColor;
		}
		public static SkillColor FsmColorField(GUIContent label, Skill fsm, SkillColor fsmColor)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (fsmColor.get_UseVariable())
			{
				fsmColor = VariableEditor.DoFsmColorPopup(label, fsm, fsmColor);
			}
			else
			{
				fsmColor.set_Value(EditorGUILayout.ColorField(label, fsmColor.get_Value(), new GUILayoutOption[0]));
			}
			fsmColor = (SkillColor)VariableEditor.VariableToggle(fsmColor, label.get_text());
			VariableEditor.EndVariableEditor(fsmColor);
			return fsmColor;
		}
		private static SkillGameObject DoFsmGameObjectPopup(GUIContent label, Skill fsm, SkillGameObject fsmGameObject)
		{
			ActionEditor.DoVariableSelector(label, fsm, 3, fsmGameObject, -1, null);
			fsmGameObject.set_UseVariable(true);
			return fsmGameObject;
		}
		public static SkillGameObject FsmGameObjectPopup(GUIContent label, Skill fsm, SkillGameObject fsmGameObject)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmGameObject = VariableEditor.DoFsmGameObjectPopup(label, fsm, fsmGameObject);
			VariableEditor.EndVariableEditor(fsmGameObject);
			return fsmGameObject;
		}
		public static SkillGameObject FsmGameObjectField(GUIContent label, Skill fsm, SkillGameObject fsmGameObject)
		{
			if (label == null)
			{
				label = GUIContent.none;
			}
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (fsmGameObject.get_UseVariable())
			{
				fsmGameObject = VariableEditor.DoFsmGameObjectPopup(label, fsm, fsmGameObject);
			}
			else
			{
				fsmGameObject.set_Value((GameObject)EditorGUILayout.ObjectField(label, fsmGameObject.get_Value(), typeof(GameObject), !SkillEditor.SelectedFsmIsPersistent(), new GUILayoutOption[0]));
			}
			fsmGameObject = (SkillGameObject)VariableEditor.VariableToggle(fsmGameObject, label.get_text());
			VariableEditor.EndVariableEditor(fsmGameObject);
			return fsmGameObject;
		}
		private static SkillObject DoFsmObjectPopup(GUIContent label, Skill fsm, SkillObject fsmObject, Type objectType)
		{
			ActionEditor.DoVariableSelector(label, fsm, 12, fsmObject, 12, objectType);
			fsmObject.set_UseVariable(true);
			return fsmObject;
		}
		public static SkillObject FsmObjectPopup(GUIContent label, Skill fsm, SkillObject fsmObject, Type objectType)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmObject = VariableEditor.DoFsmObjectPopup(label, fsm, fsmObject, objectType);
			VariableEditor.EndVariableEditor(fsmObject);
			return fsmObject;
		}
		public static SkillObject FsmObjectField(GUIContent label, Skill fsm, SkillObject fsmObject, Type objectTypeConstraint, object[] attributes)
		{
			if (label == null)
			{
				label = GUIContent.none;
			}
			if (fsmObject == null)
			{
				SkillObject fsmObject2 = new SkillObject();
				fsmObject2.set_ObjectType(objectTypeConstraint);
				fsmObject = fsmObject2;
			}
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (!objectTypeConstraint.IsAssignableFrom(fsmObject.get_ObjectType()))
			{
				fsmObject.set_ObjectType(objectTypeConstraint);
			}
			if (fsmObject.get_UseVariable())
			{
				fsmObject = VariableEditor.DoFsmObjectPopup(label, fsm, fsmObject, objectTypeConstraint);
			}
			else
			{
				ObjectPropertyDrawer objectPropertyDrawer = ObjectPropertyDrawers.GetObjectPropertyDrawer(fsmObject.get_ObjectType());
				if (objectPropertyDrawer != null)
				{
					fsmObject.set_Value(objectPropertyDrawer.OnGUI(label, fsmObject.get_Value(), !SkillEditor.SelectedFsmIsPersistent(), attributes));
				}
				else
				{
					fsmObject.set_Value(EditorGUILayout.ObjectField(label, fsmObject.get_Value(), fsmObject.get_ObjectType(), !SkillEditor.SelectedFsmIsPersistent(), new GUILayoutOption[0]));
				}
			}
			fsmObject = (SkillObject)VariableEditor.VariableToggle(fsmObject, label.get_text());
			VariableEditor.EndVariableEditor(fsmObject);
			return fsmObject;
		}
		private static SkillMaterial DoFsmMaterialPopup(GUIContent label, Skill fsm, SkillMaterial fsmObject)
		{
			ActionEditor.DoVariableSelector(label, fsm, 9, fsmObject, -1, null);
			fsmObject.set_UseVariable(true);
			return fsmObject;
		}
		public static SkillMaterial FsmMaterialPopup(GUIContent label, Skill fsm, SkillMaterial fsmObject)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmObject = VariableEditor.DoFsmMaterialPopup(label, fsm, fsmObject);
			VariableEditor.EndVariableEditor(fsmObject);
			return fsmObject;
		}
		public static SkillMaterial FsmMaterialField(GUIContent label, Skill fsm, SkillMaterial fsmObject)
		{
			if (label == null)
			{
				label = GUIContent.none;
			}
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (fsmObject.get_UseVariable())
			{
				fsmObject = VariableEditor.DoFsmMaterialPopup(label, fsm, fsmObject);
			}
			else
			{
				fsmObject.set_Value((Material)EditorGUILayout.ObjectField(label, fsmObject.get_Value(), typeof(Material), !SkillEditor.SelectedFsmIsPersistent(), new GUILayoutOption[0]));
			}
			fsmObject = (SkillMaterial)VariableEditor.VariableToggle(fsmObject, label.get_text());
			VariableEditor.EndVariableEditor(fsmObject);
			return fsmObject;
		}
		private static SkillTexture DoFsmTexturePopup(GUIContent label, Skill fsm, SkillTexture fsmObject)
		{
			ActionEditor.DoVariableSelector(label, fsm, 10, fsmObject, -1, null);
			fsmObject.set_UseVariable(true);
			return fsmObject;
		}
		public static SkillTexture FsmTexturePopup(GUIContent label, Skill fsm, SkillTexture fsmObject)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmObject = VariableEditor.DoFsmTexturePopup(label, fsm, fsmObject);
			VariableEditor.EndVariableEditor(fsmObject);
			return fsmObject;
		}
		public static SkillTexture FsmTextureField(GUIContent label, Skill fsm, SkillTexture fsmObject)
		{
			if (label == null)
			{
				label = GUIContent.none;
			}
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (fsmObject.get_UseVariable())
			{
				fsmObject = VariableEditor.DoFsmTexturePopup(label, fsm, fsmObject);
			}
			else
			{
				EditorGUILayout.PrefixLabel(label);
				fsmObject.set_Value((Texture)EditorGUILayout.ObjectField(fsmObject.get_Value(), typeof(Texture), !SkillEditor.SelectedFsmIsPersistent(), new GUILayoutOption[]
				{
					GUILayout.Width(80f),
					GUILayout.Height(80f)
				}));
				GUILayout.FlexibleSpace();
			}
			fsmObject = (SkillTexture)VariableEditor.VariableToggle(fsmObject, label.get_text());
			VariableEditor.EndVariableEditor(fsmObject);
			return fsmObject;
		}
		private static SkillArray DoFsmArrayPopup(GUIContent label, Skill fsm, SkillArray fsmArray, VariableType typeConstraint)
		{
			ActionEditor.DoVariableSelector(label, fsm, 13, fsmArray, typeConstraint, fsmArray.get_ObjectType());
			fsmArray.set_UseVariable(true);
			return fsmArray;
		}
		public static SkillArray FsmArrayPopup(GUIContent label, Skill fsm, SkillArray fsmArray, VariableType typeConstraint)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmArray = VariableEditor.DoFsmArrayPopup(label, fsm, fsmArray, typeConstraint);
			VariableEditor.EndVariableEditor(fsmArray);
			return fsmArray;
		}
		public static SkillArray FsmArrayField(GUIContent label, Skill fsm, SkillArray fsmArray, VariableType typeConstraint)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (fsmArray.get_UseVariable())
			{
				fsmArray = VariableEditor.DoFsmArrayPopup(label, fsm, fsmArray, typeConstraint);
			}
			else
			{
				fsmArray.SetType(typeConstraint);
				GUILayout.Label(label, new GUILayoutOption[0]);
			}
			fsmArray = (SkillArray)VariableEditor.VariableToggle(fsmArray, label.get_text());
			VariableEditor.EndVariableEditor(fsmArray);
			return fsmArray;
		}
		public static void EnumTypeSelector(SkillEnum fsmEnum)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel(Strings.get_Label_Enum_Type());
			if (GUILayout.Button(SkillEditorContent.TempContent(Labels.GetShortTypeName(fsmEnum.get_EnumType()), fsmEnum.get_EnumName()), EditorStyles.get_popup(), new GUILayoutOption[0]))
			{
				VariableEditor.DoFsmEnumTypeMenu(fsmEnum);
			}
			GUILayout.EndHorizontal();
		}
		public static GenericMenu DoFsmEnumTypeMenu(SkillEnum fsmEnum)
		{
			VariableEditor.editingFsmEnumType = fsmEnum;
			GenericMenu genericMenu = new GenericMenu();
			using (List<Type>.Enumerator enumerator = TypeHelpers.EnumTypeList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					string fullName = current.get_FullName();
					string text = fullName.Replace('.', '/');
					genericMenu.AddItem(new GUIContent(text), fullName == fsmEnum.get_EnumName(), new GenericMenu.MenuFunction2(VariableEditor.SetFsmEnumType), fullName);
				}
			}
			genericMenu.ShowAsContext();
			return genericMenu;
		}
		private static void SetFsmEnumType(object userdata)
		{
			if (VariableEditor.editingFsmEnumType == null)
			{
				return;
			}
			string text = userdata as string;
			VariableEditor.editingFsmEnumType.set_EnumType(ReflectionUtils.GetGlobalType(text));
		}
		private static SkillEnum DoFsmEnumPopup(GUIContent label, Skill fsm, SkillEnum fsmEnum, Type objectType)
		{
			ActionEditor.DoVariableSelector(label, fsm, 14, fsmEnum, 14, objectType);
			fsmEnum.set_UseVariable(true);
			return fsmEnum;
		}
		public static SkillEnum FsmEnumPopup(GUIContent label, Skill fsm, SkillEnum fsmEnum, Type objectType)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			fsmEnum = VariableEditor.DoFsmEnumPopup(label, fsm, fsmEnum, objectType);
			VariableEditor.EndVariableEditor(fsmEnum);
			return fsmEnum;
		}
		public static SkillEnum FsmEnumField(GUIContent label, Skill fsm, SkillEnum fsmEnum, Type objectType)
		{
			fsmEnum.set_EnumType(objectType);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (fsmEnum.get_UseVariable())
			{
				fsmEnum = VariableEditor.DoFsmEnumPopup(label, fsm, fsmEnum, objectType);
			}
			else
			{
				fsmEnum.set_Value(EditorGUILayout.EnumPopup(label, fsmEnum.get_Value(), new GUILayoutOption[0]));
			}
			fsmEnum = (SkillEnum)VariableEditor.VariableToggle(fsmEnum, label.get_text());
			VariableEditor.EndVariableEditor(fsmEnum);
			return fsmEnum;
		}
	}
}
