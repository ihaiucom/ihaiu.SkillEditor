using HutongGames.Editor;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class ActionEditor
	{
		public static GameObject GameObjectContext;
		private static string fsmNameContext;
		private static MonoBehaviour behaviourContext;
		private static SkillEventTarget fsmEventTargetContext;
		public static SkillEventTarget FSMEventTargetContextGlobal;
		public static bool PreviewMode;
		private SkillStateAction initAction;
		private static SkillStateAction editingAction;
		private static SkillStateAction editingArrayInAction;
		private static FieldInfo editingField;
		private static string editingFieldName;
		private static object editingObject;
		private static Array editingArray;
		private static SkillArray editingFsmArray;
		private static Type editingType;
		private static VariableType editingTypeConstraint;
		private static int editingIndex;
		private static NamedVariable editingVariable;
		private static VariableType editingVariableType;
		private int arrayControlID;
		private int tempArraySize;
		private bool actionIsDirty;
		private string realActionName;
		private static object contextMenuObject;
		private static FieldInfo contextMenuField;
		private static string contextMenuFieldName;
		private static int contextMenuIndex;
		private static VariableType contextVariableType;
		private static SkillArray contextFsmArray;
		private static VariableType contextTypeConstraint;
		private static Type contextType;
		private static NamedVariable contextVariable;
		private static bool exitGUI;
		private bool showRequiredFieldFootnote;
		private readonly List<CompoundArrayAttribute> compoundArrayList = new List<CompoundArrayAttribute>();
		private readonly List<FieldInfo> compoundArrayParent = new List<FieldInfo>();
		private readonly List<FieldInfo> compoundArrayChild = new List<FieldInfo>();
		private bool openParent;
		private static FieldInfo FsmEventTargetFsmNameField;
		private static FieldInfo FsmEventTargetExcludeSelfField;
		private static FieldInfo FsmEventTargetSendToChildrenField;
		private static FieldInfo FsmOwnerDefaultGameObjectField;
		private GameObject addComponentTarget;
		public ActionEditor()
		{
			ActionEditor.FsmEventTargetFsmNameField = typeof(SkillEventTarget).GetField("fsmName", 20);
			ActionEditor.FsmEventTargetExcludeSelfField = typeof(SkillEventTarget).GetField("excludeSelf", 20);
			ActionEditor.FsmEventTargetSendToChildrenField = typeof(SkillEventTarget).GetField("sendToChildren", 20);
			ActionEditor.FsmOwnerDefaultGameObjectField = typeof(SkillOwnerDefault).GetField("gameObject", 36);
		}
		private void InitAction(SkillStateAction action)
		{
			this.compoundArrayList.Clear();
			this.compoundArrayParent.Clear();
			this.compoundArrayChild.Clear();
			FieldInfo[] fields = ActionData.GetFields(action.GetType());
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = array[i];
				if (this.openParent)
				{
					this.compoundArrayChild.Add(fieldInfo);
					this.openParent = false;
				}
				object[] customAttributes = CustomAttributeHelpers.GetCustomAttributes(fieldInfo);
				CompoundArrayAttribute attribute = CustomAttributeHelpers.GetAttribute<CompoundArrayAttribute>(customAttributes);
				if (attribute != null)
				{
					this.compoundArrayList.Add(attribute);
					this.compoundArrayParent.Add(fieldInfo);
					this.openParent = true;
				}
			}
			this.realActionName = Labels.GetActionLabel(action);
			this.initAction = action;
		}
		public void Reset()
		{
			this.showRequiredFieldFootnote = false;
		}
		public static void SetVariableSelectionContext(object obj, FieldInfo field)
		{
			ActionEditor.contextMenuObject = obj;
			ActionEditor.contextMenuField = field;
		}
		public bool OnGUI(SkillStateAction action)
		{
			if (ActionEditor.exitGUI)
			{
				SkillEditor.Repaint(true);
				GUIUtility.ExitGUI();
			}
			if (action == null)
			{
				return false;
			}
			ActionEditor.editingAction = action;
			this.actionIsDirty = false;
			ActionEditor.GameObjectContext = null;
			ActionEditor.behaviourContext = null;
			ActionEditor.fsmEventTargetContext = null;
			if (!CustomActionEditors.HasCustomEditor(action.GetType()))
			{
				return this.DrawDefaultInspector(action);
			}
			CustomActionEditor customEditor = CustomActionEditors.GetCustomEditor(action);
			if (customEditor != null)
			{
				return customEditor.OnGUI();
			}
			EditorGUILayout.HelpBox(Strings.get_Error_Failed_to_draw_inspector(), 3);
			return false;
		}
		public bool DrawDefaultInspector(SkillStateAction action)
		{
			if (action == null)
			{
				return false;
			}
			if (this.initAction != action)
			{
				this.InitAction(action);
			}
			VariableEditor.DebugVariables = (FsmEditorSettings.DebugActionParameters && !ActionEditor.PreviewMode);
			Type type = action.GetType();
			FieldInfo[] fields = ActionData.GetFields(type);
			string note = CustomAttributeHelpers.GetNote(CustomAttributeHelpers.GetCustomAttributes(type));
			if (note != null)
			{
				GUILayout.Label(note, SkillEditorStyles.LabelWithWordWrap, new GUILayoutOption[0]);
			}
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo field = array[i];
				this.EditField(action, field);
			}
			EditorGUILayout.Space();
			return this.actionIsDirty;
		}
		public void EditField(SkillStateAction action, string fieldName)
		{
			FieldInfo field = action.GetType().GetField(fieldName);
			if (field != null)
			{
				this.EditField(action, field);
				SkillEditorGUILayout.ResetGUIColorsKeepAlpha();
			}
		}
		public void EditField(SkillStateAction action, FieldInfo field)
		{
			VariableEditor.DebugVariables = (FsmEditorSettings.DebugActionParameters && !ActionEditor.PreviewMode);
			SkillEditorGUILayout.ResetGUIColorsKeepAlpha();
			bool changed = GUI.get_changed();
			GUI.set_changed(false);
			ActionEditor.editingField = field;
			Type fieldType = field.get_FieldType();
			object value = field.GetValue(action);
			object[] customAttributes = CustomAttributeHelpers.GetCustomAttributes(field);
			string labelText = Labels.NicifyVariableName(field.get_Name());
			List<FsmError> parameterErrors = FsmErrorChecker.GetParameterErrors(action, field.get_Name());
			using (List<FsmError>.Enumerator enumerator = parameterErrors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.Type == FsmError.ErrorType.requiredField)
					{
						GUI.set_backgroundColor(SkillEditorStyles.GuiBackgroundErrorColor);
						this.showRequiredFieldFootnote = true;
						break;
					}
				}
			}
			string actionSection = CustomAttributeHelpers.GetActionSection(customAttributes);
			if (actionSection != null)
			{
				SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
				if (actionSection != string.Empty)
				{
					GUILayout.Label(actionSection, EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
				}
			}
			bool flag = !ActionEditor.PreviewMode && (SkillEditor.SelectedState.get_HideUnused() || FsmEditorSettings.DimUnusedActionParameters) && ActionEditor.HideField(fieldType, value, customAttributes);
			if (flag && SkillEditor.SelectedState.get_HideUnused())
			{
				return;
			}
			Color color = GUI.get_color();
			if (flag && FsmEditorSettings.DimUnusedActionParameters)
			{
				GUI.set_color(new Color(1f, 1f, 1f, GUI.get_color().a * 0.3f));
			}
			if (fieldType.get_IsArray())
			{
				if (!this.compoundArrayParent.Contains(field))
				{
					if (this.compoundArrayChild.Contains(field))
					{
						this.EditCompoundArray(action, this.compoundArrayChild.IndexOf(field));
					}
					else
					{
						this.EditField(action, field, labelText, fieldType, value, customAttributes);
					}
				}
			}
			else
			{
				this.EditField(action, field, labelText, fieldType, value, customAttributes);
			}
			using (List<FsmError>.Enumerator enumerator2 = parameterErrors.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					FsmError current2 = enumerator2.get_Current();
					if (current2.Type != FsmError.ErrorType.requiredField)
					{
						if (current2.Type == FsmError.ErrorType.missingRequiredComponent)
						{
							if (GUILayout.Button(current2.ErrorString + Environment.get_NewLine() + Strings.get_ActionEditor_Click_to_Add_Required_Component(), SkillEditorStyles.ActionErrorBox, new GUILayoutOption[0]) && current2.GameObject != null)
							{
								if (current2.ObjectType == typeof(Collider))
								{
									this.DoAddComponentMenu(current2.GameObject, new Type[]
									{
										typeof(BoxCollider),
										typeof(SphereCollider),
										typeof(CapsuleCollider),
										typeof(MeshCollider),
										typeof(WheelCollider),
										typeof(TerrainCollider)
									});
								}
								else
								{
									if (current2.ObjectType == typeof(Renderer))
									{
										UndoUtility.RegisterUndo(current2.GameObject, Strings.get_ActionEditor_Add_Component_MeshRenderer());
										EditorCommands.AddComponent(current2.GameObject, typeof(MeshRenderer));
									}
									else
									{
										if (current2.ObjectType.IsSubclassOf(typeof(Component)) && !current2.ObjectType.get_IsAbstract())
										{
											UndoUtility.RegisterUndo(current2.GameObject, string.Format(Strings.get_ActionEditor_Add_Component_XXX(), Labels.StripNamespace(current2.ObjectType.ToString())));
											EditorCommands.AddComponent(current2.GameObject, current2.ObjectType);
										}
										else
										{
											Debug.LogError(string.Format(Strings.get_ActionEditor_Cannot_Add_Component_Type_XXX(), current2.ObjectType.get_Name()));
										}
									}
								}
							}
						}
						else
						{
							if (current2.Type == FsmError.ErrorType.missingTransitionEvent)
							{
								if (GUILayout.Button(current2.ErrorString + Environment.get_NewLine() + Strings.get_ActionEditor_Click_to_Add_Transition_to_State(), SkillEditorStyles.ActionErrorBox, new GUILayoutOption[0]))
								{
									EditorCommands.AddTransitionToSelectedState(current2.info);
								}
							}
							else
							{
								GUILayout.Box(current2.ErrorString, SkillEditorStyles.ActionErrorBox, new GUILayoutOption[0]);
							}
						}
					}
				}
			}
			if (GUI.get_changed())
			{
				this.actionIsDirty = true;
			}
			else
			{
				GUI.set_changed(changed);
			}
			GUI.set_color(color);
		}
		private void DoAddComponentMenu(GameObject go, params Type[] componentTypes)
		{
			this.addComponentTarget = go;
			GenericMenu genericMenu = new GenericMenu();
			for (int i = 0; i < componentTypes.Length; i++)
			{
				Type type = componentTypes[i];
				genericMenu.AddItem(new GUIContent(Labels.StripUnityEngineNamespace(type.get_FullName())), false, new GenericMenu.MenuFunction2(this.DoAddComponent), type);
			}
			genericMenu.ShowAsContext();
		}
		private void DoAddComponent(object userdata)
		{
			Type type = userdata as Type;
			if (type != null)
			{
				EditorCommands.AddComponent(this.addComponentTarget, type);
			}
		}
		[Localizable(false)]
		private void EditField(object obj, FieldInfo field, string labelText, Type fieldType, object fieldValue, object[] attributes)
		{
			try
			{
				ActionEditor.editingObject = obj;
				ActionEditor.editingField = field;
				object obj2 = this.GUIForFieldTypes(labelText, fieldType, fieldValue, attributes);
				if (fieldValue != obj2)
				{
					field.SetValue(obj, obj2);
				}
			}
			catch (Exception ex)
			{
				if (ex is ExitGUIException)
				{
					throw;
				}
				EditorGUILayout.HelpBox(string.Format("Error editing field: {0}\n{1}", labelText, ex.get_Message()), 3);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				if (GUILayout.Button("Show More Info...", new GUILayoutOption[0]))
				{
					EditorUtility.DisplayDialog("Action Error", ex.ToString(), "OK");
				}
				if (GUILayout.Button("Copy To Clipboard", new GUILayoutOption[0]))
				{
					EditorGUIUtility.set_systemCopyBuffer(ex.ToString());
				}
				GUILayout.EndHorizontal();
			}
		}
		[Localizable(false)]
		private void EditCompoundArray(SkillStateAction action, int compoundArrayIndex)
		{
			GUIContent gUIContent = new GUIContent(this.compoundArrayList.get_Item(compoundArrayIndex).get_Name());
			FieldInfo fieldInfo = this.compoundArrayParent.get_Item(compoundArrayIndex);
			Type fieldType = fieldInfo.get_FieldType();
			object value = fieldInfo.GetValue(action);
			Type elementType = fieldType.GetElementType();
			object[] customAttributes = CustomAttributeHelpers.GetCustomAttributes(fieldInfo);
			string firstArrayName = this.compoundArrayList.get_Item(compoundArrayIndex).get_FirstArrayName();
			if (elementType == null)
			{
				return;
			}
			Array array;
			if (value != null)
			{
				array = (Array)value;
			}
			else
			{
				array = Array.CreateInstance(elementType, 0);
				fieldInfo.SetValue(ActionEditor.editingAction, array);
			}
			FieldInfo fieldInfo2 = this.compoundArrayChild.get_Item(compoundArrayIndex);
			Type fieldType2 = fieldInfo2.get_FieldType();
			object value2 = fieldInfo2.GetValue(action);
			Type elementType2 = fieldType2.GetElementType();
			object[] customAttributes2 = CustomAttributeHelpers.GetCustomAttributes(fieldInfo2);
			string secondArrayName = this.compoundArrayList.get_Item(compoundArrayIndex).get_SecondArrayName();
			if (elementType2 == null)
			{
				return;
			}
			Array array2;
			if (value2 != null)
			{
				array2 = (Array)value2;
			}
			else
			{
				array2 = Array.CreateInstance(elementType2, 0);
				fieldInfo2.SetValue(ActionEditor.editingAction, array2);
			}
			if (array2.get_Length() != array.get_Length())
			{
				array2 = this.ResizeArray(array2, array.get_Length());
			}
			bool changed = GUI.get_changed();
			if (ActionEditor.editingArray == array)
			{
				this.tempArraySize = EditorGUILayout.IntField(gUIContent, this.tempArraySize, new GUILayoutOption[0]);
				if (GUIUtility.get_keyboardControl() != this.arrayControlID || Event.get_current().get_keyCode() == 13)
				{
					if (this.tempArraySize != array.get_Length())
					{
						fieldInfo.SetValue(ActionEditor.editingArrayInAction, this.ResizeArray(array, this.tempArraySize));
						fieldInfo2.SetValue(ActionEditor.editingArrayInAction, this.ResizeArray(array2, this.tempArraySize));
						this.actionIsDirty = true;
					}
					return;
				}
			}
			else
			{
				int num = EditorGUILayout.IntField(gUIContent, array.get_Length(), new GUILayoutOption[0]);
				if (num != array.get_Length())
				{
					ActionEditor.editingArrayInAction = ActionEditor.editingAction;
					this.arrayControlID = GUIUtility.get_keyboardControl();
					ActionEditor.editingArray = array;
					this.tempArraySize = num;
				}
			}
			GUI.set_changed(changed);
			if (array.get_Length() > 0)
			{
				SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
			}
			for (int i = 0; i < array2.get_Length(); i++)
			{
				try
				{
					ActionEditor.editingObject = array;
					ActionEditor.editingIndex = i;
					array.SetValue(this.GUIForFieldTypes(" " + firstArrayName, elementType, array.GetValue(i), customAttributes), i);
				}
				catch
				{
					SkillEditor.Repaint(true);
					GUIUtility.ExitGUI();
					return;
				}
				ActionEditor.editingObject = array2;
				ActionEditor.editingIndex = i;
				array2.SetValue(this.GUIForFieldTypes(" " + secondArrayName, elementType2, array2.GetValue(i), customAttributes2), i);
				SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
			}
			if (GUI.get_changed())
			{
				fieldInfo.SetValue(action, array);
				fieldInfo2.SetValue(action, array2);
				this.actionIsDirty = true;
				return;
			}
		}
		private static bool HideField(Type type, object fieldValue, object[] attributes)
		{
			if (type.get_IsEnum())
			{
				return false;
			}
			bool flag = false;
			for (int i = 0; i < attributes.Length; i++)
			{
				Attribute attribute = (Attribute)attributes[i];
				if (attribute is RequiredFieldAttribute)
				{
					return false;
				}
				if (attribute is UIHintAttribute)
				{
					UIHintAttribute uIHintAttribute = attribute as UIHintAttribute;
					flag = (uIHintAttribute.get_Hint() == 10);
				}
			}
			if (type.IsSubclassOf(typeof(NamedVariable)))
			{
				NamedVariable namedVariable = (NamedVariable)fieldValue;
				if (namedVariable == null)
				{
					return true;
				}
				if (namedVariable.get_UseVariable() || flag)
				{
					return string.IsNullOrEmpty(namedVariable.get_Name());
				}
				if (type == typeof(SkillGameObject))
				{
					return ((SkillGameObject)fieldValue).get_Value() == null;
				}
				if (type == typeof(SkillString))
				{
					return string.IsNullOrEmpty(((SkillString)fieldValue).get_Value());
				}
				if (type == typeof(SkillTexture))
				{
					return ((SkillTexture)fieldValue).get_Value() == null;
				}
				if (type == typeof(SkillMaterial))
				{
					return ((SkillMaterial)fieldValue).get_Value() == null;
				}
				if (type == typeof(SkillObject))
				{
					return ((SkillObject)fieldValue).get_Value() == null;
				}
			}
			else
			{
				if (type == typeof(SkillEvent))
				{
					SkillEvent fsmEvent = (SkillEvent)fieldValue;
					return SkillEvent.IsNullOrEmpty(fsmEvent);
				}
				if (type.get_IsArray())
				{
					Array array = fieldValue as Array;
					return array == null || array.get_Length() == 0;
				}
			}
			return false;
		}
		public object EditField(string labelText, Type type, object fieldValue, object[] attributes)
		{
			return this.GUIForFieldTypes(labelText, type, fieldValue, attributes);
		}
		private object GUIForFieldTypes(string labelText, Type type, object fieldValue, object[] attributes)
		{
			if (type == null)
			{
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.EnumPopup(labelText, 0, new GUILayoutOption[0]);
				GUILayout.Toggle(false, SkillEditorContent.VariableButton, SkillEditorStyles.MiniButtonPadded, new GUILayoutOption[0]);
				EditorGUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
				return fieldValue;
			}
			string title = CustomAttributeHelpers.GetTitle(attributes);
			if (title != null)
			{
				labelText = title;
			}
			GUIContent tooltipLabelContent = SkillEditorGUILayout.GetTooltipLabelContent(labelText, CustomAttributeHelpers.GetTooltip(type, attributes));
			if (type == typeof(int))
			{
				return ActionEditor.EditIntValue(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(float))
			{
				return ActionEditor.EditFloatValue(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(bool))
			{
				return EditorGUILayout.Toggle(tooltipLabelContent, (bool)fieldValue, new GUILayoutOption[0]);
			}
			if (type == typeof(string))
			{
				return ActionEditor.EditStringValue(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(Vector2))
			{
				return EditorGUILayout.Vector2Field(labelText, (Vector2)fieldValue, new GUILayoutOption[0]);
			}
			if (type == typeof(Vector3))
			{
				return EditorGUILayout.Vector3Field(labelText, (Vector3)fieldValue, new GUILayoutOption[0]);
			}
			if (type == typeof(Vector4))
			{
				return EditorGUILayout.Vector4Field(labelText, (Vector4)fieldValue, new GUILayoutOption[0]);
			}
			if (type == typeof(Rect))
			{
				return EditorGUILayout.RectField(labelText, (Rect)fieldValue, new GUILayoutOption[0]);
			}
			if (type == typeof(FunctionCall))
			{
				return this.EditFunctionCall(fieldValue, attributes);
			}
			if (type == typeof(SkillTemplateControl))
			{
				return this.EditFsmTemplateControl(fieldValue);
			}
			if (type == typeof(SkillVar))
			{
				return this.EditFsmVar(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(SkillEventTarget))
			{
				return this.EditFsmEventTarget(fieldValue);
			}
			if (type == typeof(LayoutOption))
			{
				return ActionEditor.EditLayoutOption(fieldValue);
			}
			if (type == typeof(SkillFloat))
			{
				return ActionEditor.EditFsmFloat(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(SkillInt))
			{
				return ActionEditor.EditFsmInt(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(SkillBool))
			{
				return ActionEditor.EditFsmBool(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(SkillVector2))
			{
				return ActionEditor.EditFsmVector2(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(SkillVector3))
			{
				return ActionEditor.EditFsmVector3(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(SkillRect))
			{
				return ActionEditor.EditFsmRect(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(SkillQuaternion))
			{
				return ActionEditor.EditFsmQuaternion(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(SkillObject))
			{
				return ActionEditor.EditFsmObject(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(SkillMaterial))
			{
				return ActionEditor.EditFsmMaterial(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(SkillTexture))
			{
				return ActionEditor.EditFsmTexture(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(SkillColor))
			{
				return ActionEditor.EditFsmColor(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(SkillGameObject))
			{
				SkillGameObject fsmGameObject = ActionEditor.EditFsmGameObject(tooltipLabelContent, fieldValue, attributes);
				if (fsmGameObject.get_Value() != null)
				{
					ActionEditor.GameObjectContext = fsmGameObject.get_Value();
				}
				return fsmGameObject;
			}
			if (type == typeof(SkillOwnerDefault))
			{
				return ActionEditor.EditFsmOwnerDefault(tooltipLabelContent, fieldValue);
			}
			if (type == typeof(SkillEvent))
			{
				EventTargetAttribute attribute = CustomAttributeHelpers.GetAttribute<EventTargetAttribute>(attributes);
				if (attribute != null)
				{
					if (ActionEditor.fsmEventTargetContext == null)
					{
						ActionEditor.fsmEventTargetContext = new SkillEventTarget();
					}
					ActionEditor.fsmEventTargetContext.target = attribute.get_Target();
				}
				ActionEditor.EventSelector(tooltipLabelContent, fieldValue as SkillEvent, ActionEditor.fsmEventTargetContext ?? ActionEditor.FSMEventTargetContextGlobal);
				return fieldValue;
			}
			if (type == typeof(SkillString))
			{
				return ActionEditor.EditFsmString(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(AnimationState))
			{
				return SkillEditorGUILayout.AnimationStatePopup(tooltipLabelContent, (AnimationState)fieldValue, ActionEditor.GameObjectContext);
			}
			if (type == typeof(Behaviour))
			{
				return SkillEditorGUILayout.BehaviorPopup(tooltipLabelContent, (Behaviour)fieldValue, ActionEditor.GameObjectContext);
			}
			if (type == typeof(MonoBehaviour))
			{
				return SkillEditorGUILayout.BehaviorPopup(tooltipLabelContent, (MonoBehaviour)fieldValue, ActionEditor.GameObjectContext);
			}
			if (type == typeof(Color))
			{
				return EditorGUILayout.ColorField(tooltipLabelContent, (Color)fieldValue, new GUILayoutOption[0]);
			}
			if (type == typeof(SkillAnimationCurve))
			{
				SkillAnimationCurve fsmAnimationCurve = ((SkillAnimationCurve)fieldValue) ?? new SkillAnimationCurve();
				fsmAnimationCurve.curve = EditorGUILayout.CurveField(labelText, fsmAnimationCurve.curve, new GUILayoutOption[0]);
				return fsmAnimationCurve;
			}
			if (type == typeof(GameObject))
			{
				GameObject gameObject = ActionEditor.GameObjectField(tooltipLabelContent, fieldValue);
				if (gameObject != null)
				{
					ActionEditor.GameObjectContext = gameObject;
				}
				return gameObject;
			}
			if (type.get_IsArray())
			{
				return this.EditArray(tooltipLabelContent, type.GetElementType(), fieldValue, attributes);
			}
			if (type == typeof(SkillArray))
			{
				return this.EditFsmArray(tooltipLabelContent, fieldValue, attributes);
			}
			if (type == typeof(SkillEnum))
			{
				return this.EditFsmEnum(tooltipLabelContent, fieldValue, attributes);
			}
			Enum @enum = fieldValue as Enum;
			if (@enum != null)
			{
				return EditorGUILayout.EnumPopup(tooltipLabelContent, @enum, new GUILayoutOption[0]);
			}
			if (type.IsSubclassOf(typeof(Object)) || type == typeof(Object))
			{
				return EditorGUILayout.ObjectField(tooltipLabelContent.get_text(), (Object)fieldValue, type, !SkillEditor.SelectedFsmIsPersistent(), new GUILayoutOption[0]);
			}
			if (type == typeof(SkillProperty))
			{
				return this.EditFsmProperty(fieldValue, attributes);
			}
			if (type.get_IsClass())
			{
				return this.EditCustomType(tooltipLabelContent, type, fieldValue, attributes);
			}
			GUILayout.Label(string.Format(Strings.get_ActionEditor_Error_Unsupported_Type_XXX(), type), SkillEditorStyles.ErrorBox, new GUILayoutOption[0]);
			return fieldValue;
		}
		private object EditCustomType(GUIContent label, Type customType, object fieldValue, object[] attributes)
		{
			if (fieldValue == null)
			{
				fieldValue = Activator.CreateInstance(customType);
			}
			ActionEditor.editingObject = fieldValue;
			GUILayout.Label(label, new GUILayoutOption[0]);
			PropertyDrawer propertyDrawer = PropertyDrawers.GetPropertyDrawer(customType);
			if (propertyDrawer != null)
			{
				return propertyDrawer.OnGUI(label, fieldValue, !SkillEditor.SelectedFsmIsPersistent(), attributes);
			}
			FieldInfo[] fields = ActionData.GetFields(customType);
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = array[i];
				string labelText = Labels.NicifyVariableName(fieldInfo.get_Name());
				ActionEditor.editingField = fieldInfo;
				attributes = CustomAttributeHelpers.GetCustomAttributes(fieldInfo);
				this.EditField(fieldValue, fieldInfo, labelText, fieldInfo.get_FieldType(), fieldInfo.GetValue(fieldValue), attributes);
			}
			SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
			return fieldValue;
		}
		private Array EditArray(GUIContent label, Type elementType, object fieldValue, object[] attributes)
		{
			Array array;
			if (fieldValue != null)
			{
				array = (Array)fieldValue;
			}
			else
			{
				array = Array.CreateInstance(elementType, 0);
			}
			return this.EditArray(label, elementType, array, attributes);
		}
		[Localizable(false)]
		private Array EditArray(GUIContent label, Type elementType, Array array, object[] attributes)
		{
			ArrayEditorAttribute arrayEditorAttribute = CustomAttributeHelpers.GetAttribute<ArrayEditorAttribute>(attributes) ?? new ArrayEditorAttribute(-1, "", 0, 0, 65536);
			bool changed = GUI.get_changed();
			if (arrayEditorAttribute.get_Resizable())
			{
				if (ActionEditor.editingArray == array)
				{
					this.tempArraySize = EditorGUILayout.IntField(label, this.tempArraySize, new GUILayoutOption[0]);
					this.tempArraySize = Mathf.Clamp(this.tempArraySize, arrayEditorAttribute.get_MinSize(), arrayEditorAttribute.get_MaxSize());
					if (GUIUtility.get_keyboardControl() != this.arrayControlID || Event.get_current().get_keyCode() == 13)
					{
						return this.ResizeArray();
					}
				}
				else
				{
					int num = EditorGUILayout.IntField(label, array.get_Length(), new GUILayoutOption[0]);
					if (num != array.get_Length())
					{
						ActionEditor.editingArrayInAction = ActionEditor.editingAction;
						this.arrayControlID = GUIUtility.get_keyboardControl();
						ActionEditor.editingArray = array;
						this.tempArraySize = Mathf.Clamp(num, arrayEditorAttribute.get_MinSize(), arrayEditorAttribute.get_MaxSize());
					}
				}
			}
			else
			{
				if (array.get_Length() != arrayEditorAttribute.get_FixedSize())
				{
					ActionEditor.editingArray = array;
					this.tempArraySize = arrayEditorAttribute.get_FixedSize();
					return this.ResizeArray();
				}
			}
			if (array.get_Length() < arrayEditorAttribute.get_MinSize())
			{
				ActionEditor.editingArray = array;
				this.tempArraySize = arrayEditorAttribute.get_MinSize();
				return this.ResizeArray();
			}
			if (array.get_Length() > arrayEditorAttribute.get_MaxSize())
			{
				ActionEditor.editingArray = array;
				this.tempArraySize = arrayEditorAttribute.get_MaxSize();
				return this.ResizeArray();
			}
			GUI.set_changed(changed);
			EditorGUI.set_indentLevel(EditorGUI.get_indentLevel() + 1);
			string text = "Element ";
			if (!string.IsNullOrEmpty(arrayEditorAttribute.get_ElementName()))
			{
				text = arrayEditorAttribute.get_ElementName() + " ";
			}
			for (int i = 0; i < array.get_Length(); i++)
			{
				ActionEditor.editingObject = array;
				ActionEditor.editingIndex = i;
				if (elementType == typeof(SkillVar))
				{
					SkillVar fsmVar = array.GetValue(i) as SkillVar;
					if (fsmVar != null)
					{
						ActionEditor.editingType = fsmVar.get_ObjectType();
					}
				}
				array.SetValue(this.GUIForFieldTypes(text + i, elementType, array.GetValue(i), attributes), i);
			}
			SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
			EditorGUI.set_indentLevel(EditorGUI.get_indentLevel() - 1);
			if (GUI.get_changed())
			{
				this.actionIsDirty = true;
			}
			ActionEditor.editingFsmArray = null;
			return array;
		}
		public bool Update()
		{
			ActionEditor.exitGUI = false;
			this.UpdateHelpBoxes();
			return false;
		}
		private void UpdateHelpBoxes()
		{
			StateInspector.ShowRequiredFieldFootnote = this.showRequiredFieldFootnote;
		}
		private Array ResizeArray()
		{
			Array array = this.ResizeArray(ActionEditor.editingArray, this.tempArraySize);
			if (ActionEditor.editingFsmArray != null)
			{
				ActionEditor.editingFsmArray.set_Values((object[])array);
				ActionEditor.editingFsmArray.SaveChanges();
			}
			ActionEditor.exitGUI = true;
			ActionEditor.editingArray = null;
			ActionEditor.editingFsmArray = null;
			this.actionIsDirty = true;
			return array;
		}
		private Array ResizeArray(Array array, int newSize)
		{
			if (newSize < 0)
			{
				newSize = 0;
			}
			if (array != null && newSize != array.get_Length())
			{
				SkillEditor.RegisterUndo(Strings.get_ActionEditor_Undo_Resize_Array());
				Type elementType = array.GetType().GetElementType();
				Array array2 = Array.CreateInstance(elementType, newSize);
				Array.Copy(array, array2, Math.Min(array.get_Length(), newSize));
				return array2;
			}
			return array;
		}
		public static object GetDefault(Type type)
		{
			if (!type.get_IsValueType())
			{
				return null;
			}
			return Activator.CreateInstance(type);
		}
		private static GameObject GameObjectField(GUIContent label, object fieldValue)
		{
			return (GameObject)EditorGUILayout.ObjectField(label, (GameObject)fieldValue, typeof(GameObject), !SkillEditor.SelectedFsmIsPersistent(), new GUILayoutOption[0]);
		}
		private static LayoutOption EditLayoutOption(object fieldValue)
		{
			LayoutOption layoutOption = ((LayoutOption)fieldValue) ?? new LayoutOption();
			layoutOption.option = (LayoutOption.LayoutOptionType)EditorGUILayout.EnumPopup(Strings.get_ActionEditor_EditLayoutOption_Option(), layoutOption.option, new GUILayoutOption[0]);
			switch (layoutOption.option)
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
				layoutOption.floatParam = VariableEditor.FsmFloatField(GUIContent.none, SkillEditor.SelectedFsm, layoutOption.floatParam);
				break;
			case 6:
			case 7:
				layoutOption.boolParam = VariableEditor.FsmBoolField(GUIContent.none, SkillEditor.SelectedFsm, layoutOption.boolParam);
				break;
			}
			return layoutOption;
		}
		[Localizable(false)]
		private SkillProperty EditFsmProperty(object fieldValue, object[] attributes)
		{
			SkillProperty fsmProperty = ((SkillProperty)fieldValue) ?? new SkillProperty();
			fsmProperty.CheckForReinitialize();
			bool changed = GUI.get_changed();
			GUI.set_changed(false);
			ActionEditor.editingObject = fsmProperty;
			ActionEditor.editingFieldName = "TargetObject";
			SkillObject targetObject = ActionEditor.EditFsmObject(new GUIContent(Strings.get_ActionEditor_EditFsmProperty_Target_Object(), Strings.get_ActionEditor_EditFsmProperty_Target_Object_Tooltip()), fsmProperty.TargetObject, attributes);
			if (GUI.get_changed())
			{
				fsmProperty.TargetObject = targetObject;
				fsmProperty.TargetTypeName = ((fsmProperty.TargetObject.get_Value() != null) ? fsmProperty.TargetObject.get_Value().GetType().ToString() : null);
				if (fsmProperty.TargetObject.get_Value() == null)
				{
					fsmProperty.TargetTypeName = null;
				}
				fsmProperty.Init();
			}
			if (!GUI.get_changed())
			{
				GUI.set_changed(changed);
			}
			if ((fsmProperty.TargetObject.get_Value() == null && !fsmProperty.TargetObject.get_UseVariable()) || fsmProperty.TargetObject.get_IsNone())
			{
				SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
				return fsmProperty;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(Strings.get_ActionEditor_EditFsmProperty_Object_Type(), new GUILayoutOption[]
			{
				GUILayout.Width(146f)
			});
			SkillEditorGUILayout.ReadonlyTextField(fsmProperty.TargetTypeName, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			string text = Strings.get_Label_None();
			if (!string.IsNullOrEmpty(fsmProperty.PropertyName))
			{
				text = fsmProperty.PropertyName;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			SkillEditorGUILayout.PrefixLabel(SkillEditorContent.TempContent(Strings.get_Label_Property(), Strings.get_Label_Property_Tooltip()));
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.get_popup());
			if (GUI.Button(rect, SkillEditorContent.TempContent(text, text), EditorStyles.get_popup()))
			{
				TypeHelpers.GeneratePropertyMenu(fsmProperty).DropDown(rect);
			}
			GUILayout.EndHorizontal();
			if (string.IsNullOrEmpty(fsmProperty.PropertyName))
			{
				SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
				return fsmProperty;
			}
			if (fsmProperty.PropertyType == null)
			{
				if (!string.IsNullOrEmpty(fsmProperty.PropertyName))
				{
					fsmProperty.SetPropertyName(fsmProperty.PropertyName);
				}
				if (fsmProperty.PropertyType == null)
				{
					SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
					return fsmProperty;
				}
			}
			string typeTooltip = Labels.GetTypeTooltip(fsmProperty.PropertyType);
			GUIContent label = SkillEditorContent.TempContent(Strings.get_Label_Set_Value(), typeTooltip);
			ActionEditor.editingFieldName = "SetVariable";
			if (fsmProperty.PropertyType.get_IsArray())
			{
				if (fsmProperty.ArrayParameter.get_ElementType() == -1)
				{
					Type elementType = fsmProperty.PropertyType.GetElementType();
					fsmProperty.ArrayParameter.set_ElementType(SkillVar.GetVariableType(elementType));
					fsmProperty.ArrayParameter.set_ObjectType(elementType);
				}
				ActionEditor.editingType = fsmProperty.ArrayParameter.get_ObjectType();
			}
			if (fsmProperty.setProperty)
			{
				if (fsmProperty.PropertyType.IsAssignableFrom(typeof(bool)))
				{
					fsmProperty.BoolParameter = VariableEditor.FsmBoolField(label, SkillEditor.SelectedFsm, fsmProperty.BoolParameter);
				}
				else
				{
					if (fsmProperty.PropertyType.IsAssignableFrom(typeof(float)))
					{
						fsmProperty.FloatParameter = VariableEditor.FsmFloatField(label, SkillEditor.SelectedFsm, fsmProperty.FloatParameter);
					}
					else
					{
						if (fsmProperty.PropertyType.IsAssignableFrom(typeof(int)))
						{
							fsmProperty.IntParameter = VariableEditor.FsmIntField(label, SkillEditor.SelectedFsm, fsmProperty.IntParameter, null);
						}
						else
						{
							if (fsmProperty.PropertyType.IsAssignableFrom(typeof(GameObject)))
							{
								fsmProperty.GameObjectParameter = VariableEditor.FsmGameObjectField(label, SkillEditor.SelectedFsm, fsmProperty.GameObjectParameter);
							}
							else
							{
								if (fsmProperty.PropertyType.IsAssignableFrom(typeof(string)))
								{
									fsmProperty.StringParameter = VariableEditor.FsmStringField(label, SkillEditor.SelectedFsm, fsmProperty.StringParameter, null);
								}
								else
								{
									if (fsmProperty.PropertyType.IsAssignableFrom(typeof(Vector2)))
									{
										fsmProperty.Vector2Parameter = VariableEditor.FsmVector2Field(label, SkillEditor.SelectedFsm, fsmProperty.Vector2Parameter);
									}
									else
									{
										if (fsmProperty.PropertyType.IsAssignableFrom(typeof(Vector3)))
										{
											fsmProperty.Vector3Parameter = VariableEditor.FsmVector3Field(label, SkillEditor.SelectedFsm, fsmProperty.Vector3Parameter);
										}
										else
										{
											if (fsmProperty.PropertyType.IsAssignableFrom(typeof(Rect)))
											{
												fsmProperty.RectParamater = VariableEditor.FsmRectField(label, SkillEditor.SelectedFsm, fsmProperty.RectParamater);
											}
											else
											{
												if (fsmProperty.PropertyType.IsAssignableFrom(typeof(Material)))
												{
													fsmProperty.MaterialParameter = VariableEditor.FsmMaterialField(label, SkillEditor.SelectedFsm, fsmProperty.MaterialParameter);
												}
												else
												{
													if (fsmProperty.PropertyType.IsAssignableFrom(typeof(Texture)))
													{
														fsmProperty.TextureParameter = VariableEditor.FsmTextureField(label, SkillEditor.SelectedFsm, fsmProperty.TextureParameter);
													}
													else
													{
														if (fsmProperty.PropertyType.IsAssignableFrom(typeof(Color)))
														{
															fsmProperty.ColorParameter = VariableEditor.FsmColorField(label, SkillEditor.SelectedFsm, fsmProperty.ColorParameter);
														}
														else
														{
															if (fsmProperty.PropertyType.IsAssignableFrom(typeof(Quaternion)))
															{
																fsmProperty.QuaternionParameter = VariableEditor.FsmQuaternionField(label, SkillEditor.SelectedFsm, fsmProperty.QuaternionParameter);
															}
															else
															{
																if (fsmProperty.PropertyType.IsSubclassOf(typeof(Object)))
																{
																	fsmProperty.ObjectParameter = VariableEditor.FsmObjectField(label, SkillEditor.SelectedFsm, fsmProperty.ObjectParameter, fsmProperty.PropertyType, attributes);
																}
																else
																{
																	if (fsmProperty.PropertyType.get_IsEnum())
																	{
																		fsmProperty.EnumParameter = VariableEditor.FsmEnumField(label, SkillEditor.SelectedFsm, fsmProperty.EnumParameter, fsmProperty.PropertyType);
																	}
																	else
																	{
																		if (fsmProperty.PropertyType.get_IsArray())
																		{
																			fsmProperty.ArrayParameter = this.EditFsmArray(label, fsmProperty.ArrayParameter, attributes);
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			else
			{
				if (fsmProperty.PropertyType.IsAssignableFrom(typeof(bool)))
				{
					fsmProperty.BoolParameter = VariableEditor.FsmBoolPopup(new GUIContent(Strings.get_ActionEditor_Store_Bool(), typeTooltip), SkillEditor.SelectedFsm, fsmProperty.BoolParameter);
				}
				else
				{
					if (fsmProperty.PropertyType.IsAssignableFrom(typeof(float)))
					{
						fsmProperty.FloatParameter = VariableEditor.FsmFloatPopup(new GUIContent(Strings.get_ActionEditor_Store_Float(), typeTooltip), SkillEditor.SelectedFsm, fsmProperty.FloatParameter);
					}
					else
					{
						if (fsmProperty.PropertyType.IsAssignableFrom(typeof(int)))
						{
							fsmProperty.IntParameter = VariableEditor.FsmIntPopup(new GUIContent(Strings.get_ActionEditor_Store_Int(), typeTooltip), SkillEditor.SelectedFsm, fsmProperty.IntParameter);
						}
						else
						{
							if (fsmProperty.PropertyType.IsAssignableFrom(typeof(GameObject)))
							{
								fsmProperty.GameObjectParameter = VariableEditor.FsmGameObjectPopup(new GUIContent(Strings.get_ActionEditor_Store_GameObject(), typeTooltip), SkillEditor.SelectedFsm, fsmProperty.GameObjectParameter);
							}
							else
							{
								if (fsmProperty.PropertyType.IsAssignableFrom(typeof(string)))
								{
									fsmProperty.StringParameter = VariableEditor.FsmStringPopup(new GUIContent(Strings.get_ActionEditor_Store_String(), typeTooltip), SkillEditor.SelectedFsm, fsmProperty.StringParameter);
								}
								else
								{
									if (fsmProperty.PropertyType.IsAssignableFrom(typeof(Vector2)))
									{
										fsmProperty.Vector2Parameter = VariableEditor.FsmVector2Popup(new GUIContent(Strings.get_ActionEditor_Store_Vector2(), typeTooltip), SkillEditor.SelectedFsm, fsmProperty.Vector2Parameter);
									}
									else
									{
										if (fsmProperty.PropertyType.IsAssignableFrom(typeof(Vector3)))
										{
											fsmProperty.Vector3Parameter = VariableEditor.FsmVector3Popup(new GUIContent(Strings.get_ActionEditor_Store_Vector3(), typeTooltip), SkillEditor.SelectedFsm, fsmProperty.Vector3Parameter);
										}
										else
										{
											if (fsmProperty.PropertyType.IsAssignableFrom(typeof(Rect)))
											{
												fsmProperty.RectParamater = VariableEditor.FsmRectPopup(new GUIContent(Strings.get_ActionEditor_Store_Rect(), typeTooltip), SkillEditor.SelectedFsm, fsmProperty.RectParamater);
											}
											else
											{
												if (fsmProperty.PropertyType.IsAssignableFrom(typeof(Material)))
												{
													fsmProperty.MaterialParameter = VariableEditor.FsmMaterialPopup(new GUIContent(Strings.get_ActionEditor_Store_Material(), typeTooltip), SkillEditor.SelectedFsm, fsmProperty.MaterialParameter);
												}
												else
												{
													if (fsmProperty.PropertyType.IsAssignableFrom(typeof(Texture)))
													{
														fsmProperty.TextureParameter = VariableEditor.FsmTexturePopup(new GUIContent(Strings.get_ActionEditor_Store_Texture(), typeTooltip), SkillEditor.SelectedFsm, fsmProperty.TextureParameter);
													}
													else
													{
														if (fsmProperty.PropertyType.IsAssignableFrom(typeof(Color)))
														{
															fsmProperty.ColorParameter = VariableEditor.FsmColorPopup(new GUIContent(Strings.get_ActionEditor_Store_Color(), typeTooltip), SkillEditor.SelectedFsm, fsmProperty.ColorParameter);
														}
														else
														{
															if (fsmProperty.PropertyType.IsAssignableFrom(typeof(Quaternion)))
															{
																fsmProperty.QuaternionParameter = VariableEditor.FsmQuaternionPopup(new GUIContent(Strings.get_ActionEditor_Store_Quaternion(), typeTooltip), SkillEditor.SelectedFsm, fsmProperty.QuaternionParameter);
															}
															else
															{
																if (fsmProperty.PropertyType.IsSubclassOf(typeof(Object)))
																{
																	fsmProperty.ObjectParameter = VariableEditor.FsmObjectPopup(new GUIContent(Strings.get_ActionEditor_Store_Object(), typeTooltip), SkillEditor.SelectedFsm, fsmProperty.ObjectParameter, fsmProperty.PropertyType);
																}
																else
																{
																	if (fsmProperty.PropertyType.get_IsEnum())
																	{
																		fsmProperty.EnumParameter = VariableEditor.FsmEnumPopup(new GUIContent(Strings.get_ActionEditor_Store_Enum(), typeTooltip), SkillEditor.SelectedFsm, fsmProperty.EnumParameter, fsmProperty.PropertyType);
																	}
																	else
																	{
																		if (fsmProperty.PropertyType.get_IsArray())
																		{
																			fsmProperty.ArrayParameter = VariableEditor.FsmArrayPopup(new GUIContent("Store Array", typeTooltip), SkillEditor.SelectedFsm, fsmProperty.ArrayParameter, SkillVar.GetVariableType(fsmProperty.PropertyType.GetElementType()));
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
			return fsmProperty;
		}
		public SkillVar EditFsmVar(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillVar fsmVar = ((SkillVar)fieldValue) ?? new SkillVar();
			MatchElementTypeAttribute attribute = CustomAttributeHelpers.GetAttribute<MatchElementTypeAttribute>(attributes);
			if (attribute != null)
			{
				bool flag = this.MatchElementType(fsmVar, attribute.get_FieldName());
				if (flag)
				{
					if (fsmVar.get_Type() != -1)
					{
						fsmVar.set_NamedVar(SkillVariable.ResetVariableReference(fsmVar.get_NamedVar()));
					}
					ActionEditor.exitGUI = true;
					return fsmVar;
				}
			}
			else
			{
				if (!CustomAttributeHelpers.HasAttribute<HideTypeFilter>(attributes))
				{
					VariableType variableType = (VariableType)EditorGUILayout.EnumPopup(new GUIContent(Strings.get_Label_Type()), fsmVar.get_Type(), new GUILayoutOption[0]);
					if (variableType != fsmVar.get_Type())
					{
						fsmVar.variableName = "";
						fsmVar.set_Type(variableType);
					}
					else
					{
						if (fsmVar.get_Type() == 12)
						{
							GUILayout.BeginHorizontal(new GUILayoutOption[0]);
							EditorGUILayout.PrefixLabel(Strings.get_Label_Object_Type());
							if (GUILayout.Button(SkillEditorContent.TempContent(fsmVar.get_ObjectType().get_Name(), fsmVar.get_ObjectType().get_FullName()), EditorStyles.get_popup(), new GUILayoutOption[0]))
							{
								ActionEditor.contextMenuObject = fsmVar;
								GenericMenu genericMenu = new GenericMenu();
								using (List<Type>.Enumerator enumerator = TypeHelpers.ObjectTypeList.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										Type current = enumerator.get_Current();
										string fullName = current.get_FullName();
										string text = fullName.Replace('.', '/');
										genericMenu.AddItem(new GUIContent(text), fullName == fsmVar.get_ObjectType().get_FullName(), new GenericMenu.MenuFunction2(this.SetFsmVarObjectType), fullName);
									}
								}
								genericMenu.ShowAsContext();
							}
							GUILayout.EndHorizontal();
						}
					}
				}
			}
			ActionEditor.editingObject = fsmVar;
			ActionEditor.editingVariableType = fsmVar.get_Type();
			ActionEditor.editingType = fsmVar.get_ObjectType();
			if (CustomAttributeHelpers.GetUIHint(attributes) == 10)
			{
				if (fsmVar.get_Type() == 13)
				{
					VariableEditor.FsmVarPopup(label, SkillEditor.SelectedFsm, fsmVar, fsmVar.arrayValue.get_ElementType(), fsmVar.arrayValue.get_ObjectType());
				}
				else
				{
					VariableEditor.FsmVarPopup(label, SkillEditor.SelectedFsm, fsmVar, (attribute != null) ? fsmVar.get_Type() : -1, fsmVar.get_RealType());
				}
			}
			else
			{
				fsmVar.set_NamedVar((NamedVariable)this.GUIForFieldTypes(label.get_text(), fsmVar.get_NamedVarType(), fsmVar.get_NamedVar(), attributes));
			}
			return fsmVar;
		}
		private void SetFsmVarEnumType(object userdata)
		{
			SkillVar fsmVar = ActionEditor.contextMenuObject as SkillVar;
			if (fsmVar != null)
			{
				fsmVar.set_EnumType(ReflectionUtils.GetGlobalType(userdata as string));
			}
		}
		private void SetFsmVarObjectType(object userdata)
		{
			SkillVar fsmVar = ActionEditor.contextMenuObject as SkillVar;
			if (fsmVar != null)
			{
				fsmVar.set_NamedVar(null);
				fsmVar.set_ObjectType(ReflectionUtils.GetGlobalType(userdata as string));
			}
		}
		private bool MatchElementType(SkillVar fsmVar, string fieldName)
		{
			bool flag = false;
			FieldInfo[] fields = ActionEditor.editingAction.GetType().GetFields();
			FieldInfo[] array = fields;
			int i = 0;
			while (i < array.Length)
			{
				FieldInfo fieldInfo = array[i];
				if (fieldInfo.get_Name() == fieldName)
				{
					if (fieldInfo.get_FieldType() != typeof(SkillArray))
					{
						break;
					}
					SkillArray fsmArray = (SkillArray)fieldInfo.GetValue(ActionEditor.editingAction);
					if (fsmVar.get_Type() != fsmArray.get_ElementType())
					{
						fsmVar.set_Type(fsmArray.get_ElementType());
						flag = true;
					}
					if (fsmVar.get_Type() == 14)
					{
						if (flag || !object.ReferenceEquals(fsmVar.get_EnumType(), fsmArray.get_ObjectType()))
						{
							fsmVar.set_EnumType(fsmArray.get_ObjectType());
							flag = true;
							break;
						}
						break;
					}
					else
					{
						if (fsmVar.get_Type() == 12 && (flag || !object.ReferenceEquals(fsmVar.get_ObjectType(), fsmArray.get_ObjectType())))
						{
							fsmVar.set_ObjectType(fsmArray.get_ObjectType());
							flag = true;
							break;
						}
						break;
					}
				}
				else
				{
					i++;
				}
			}
			return flag;
		}
		private void MatchFieldType(SkillEnum fsmEnum, string fieldName)
		{
			FieldInfo[] fields = ActionEditor.editingAction.GetType().GetFields();
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = array[i];
				if (fieldInfo.get_Name() == fieldName)
				{
					if (fieldInfo.get_FieldType() == typeof(SkillEnum))
					{
						fsmEnum.set_EnumType(((SkillEnum)fieldInfo.GetValue(ActionEditor.editingAction)).get_EnumType());
					}
					else
					{
						if (fieldInfo.get_FieldType().get_IsEnum())
						{
							fsmEnum.set_EnumType(fieldInfo.get_FieldType());
						}
					}
				}
			}
		}
		private void ClearTemplate()
		{
			SkillTemplateControl fsmTemplateControl = (SkillTemplateControl)ActionEditor.contextMenuObject;
			fsmTemplateControl.SetFsmTemplate(null);
		}
		private void SelectTemplate(object userdata)
		{
			SkillTemplateControl fsmTemplateControl = (SkillTemplateControl)ActionEditor.contextMenuObject;
			fsmTemplateControl.SetFsmTemplate((SkillTemplate)userdata);
		}
		private SkillTemplateControl EditFsmTemplateControl(object fieldValue)
		{
			SkillTemplateControl fsmTemplateControl = ((SkillTemplateControl)fieldValue) ?? new SkillTemplateControl();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			SkillTemplate fsmTemplate = (SkillTemplate)EditorGUILayout.ObjectField(fsmTemplateControl.fsmTemplate, typeof(SkillTemplate), false, new GUILayoutOption[0]);
			if (GUILayout.Button(new GUIContent(Strings.get_Label_Browse(), Strings.get_Tooltip_Browse_Templates()), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(30f)
			}))
			{
				ActionEditor.contextMenuObject = fsmTemplateControl;
				Templates.DoSelectTemplateMenu(fsmTemplateControl.fsmTemplate, new GenericMenu.MenuFunction(this.ClearTemplate), new GenericMenu.MenuFunction2(this.SelectTemplate));
			}
			if (fsmTemplate != null)
			{
				if (!Application.get_isPlaying())
				{
					if (GUILayout.Button(Strings.get_Command_Edit(), new GUILayoutOption[]
					{
						GUILayout.Width(45f)
					}))
					{
						SkillEditor.SelectTemplateDelayed(fsmTemplate);
					}
				}
				else
				{
					if (GUILayout.Button(Strings.get_Command_Show(), new GUILayoutOption[]
					{
						GUILayout.Width(45f)
					}))
					{
						SkillEditor.SelectFsmDelayed(fsmTemplateControl.get_RunFsm());
					}
				}
			}
			else
			{
				if (GUILayout.Button(Strings.get_Command_New(), new GUILayoutOption[]
				{
					GUILayout.Width(45f)
				}))
				{
					SkillTemplate fsmTemplate2 = SkillBuilder.CreateTemplate();
					if (fsmTemplate2 != null)
					{
						fsmTemplate2.fsm.Reset(null);
						fsmTemplateControl.fsmTemplate = fsmTemplate2;
					}
				}
			}
			GUILayout.EndHorizontal();
			if (fsmTemplate != fsmTemplateControl.fsmTemplate)
			{
				fsmTemplateControl.SetFsmTemplate(fsmTemplate);
			}
			if (fsmTemplate != null)
			{
				fsmTemplateControl.UpdateOverrides();
				if (fsmTemplateControl.fsmVarOverrides.Length > 0)
				{
					GUILayout.Label(Strings.get_ActionEditor_Label_Input(), EditorStyles.get_boldLabel(), new GUILayoutOption[0]);
				}
				if (fsmTemplateControl.fsmVarOverrides != null)
				{
					for (int i = 0; i < fsmTemplateControl.fsmVarOverrides.Length; i++)
					{
						SkillVarOverride fsmVarOverride = fsmTemplateControl.fsmVarOverrides[i];
						if (fsmVarOverride != null)
						{
							NamedVariable namedVar = fsmVarOverride.fsmVar.get_NamedVar();
							ActionEditor.editingObject = fsmVarOverride.fsmVar;
							ActionEditor.editingType = namedVar.get_ObjectType();
							fsmTemplateControl.fsmVarOverrides[i].fsmVar.set_NamedVar((NamedVariable)this.GUIForFieldTypes(fsmVarOverride.variable.get_Name(), namedVar.GetType(), namedVar, null));
						}
					}
				}
			}
			SkillEditorGUILayout.LightDivider(new GUILayoutOption[0]);
			return fsmTemplateControl;
		}
		private FunctionCall EditFunctionCall(object fieldValue, object[] attributes)
		{
			FunctionCall functionCall = ((FunctionCall)fieldValue) ?? new FunctionCall();
			ActionEditor.editingObject = functionCall;
			bool coroutinesOnly = CustomAttributeHelpers.GetUIHint(attributes) == 5;
			functionCall.FunctionName = ActionEditor.EditMethodName(new GUIContent(Strings.get_ActionEditor_Method_Name()), functionCall.FunctionName, coroutinesOnly);
			string text = SkillEditorGUILayout.ParameterTypePopup(Strings.get_ActionEditor_Parameter(), functionCall.get_ParameterType());
			if (text != functionCall.get_ParameterType())
			{
				functionCall.set_ParameterType(text);
				functionCall.ResetParameters();
			}
			string parameterType;
			if ((parameterType = functionCall.get_ParameterType()) != null)
			{
				if (<PrivateImplementationDetails>{9F518054-9A7A-4388-8A0B-9CF872B8F518}.$$method0x60003cc-1 == null)
				{
					Dictionary<string, int> expr_89 = new Dictionary<string, int>(15);
					expr_89.Add("int", 0);
					expr_89.Add("float", 1);
					expr_89.Add("string", 2);
					expr_89.Add("Vector2", 3);
					expr_89.Add("Vector3", 4);
					expr_89.Add("Rect", 5);
					expr_89.Add("bool", 6);
					expr_89.Add("GameObject", 7);
					expr_89.Add("Material", 8);
					expr_89.Add("Texture", 9);
					expr_89.Add("Color", 10);
					expr_89.Add("Quaternion", 11);
					expr_89.Add("Object", 12);
					expr_89.Add("Enum", 13);
					expr_89.Add("Array", 14);
					<PrivateImplementationDetails>{9F518054-9A7A-4388-8A0B-9CF872B8F518}.$$method0x60003cc-1 = expr_89;
				}
				int num;
				if (<PrivateImplementationDetails>{9F518054-9A7A-4388-8A0B-9CF872B8F518}.$$method0x60003cc-1.TryGetValue(parameterType, ref num))
				{
					switch (num)
					{
					case 0:
						functionCall.IntParameter = VariableEditor.FsmIntField(new GUIContent(Strings.get_Label_Int()), SkillEditor.SelectedFsm, functionCall.IntParameter, attributes);
						break;
					case 1:
						functionCall.FloatParameter = VariableEditor.FsmFloatField(new GUIContent(Strings.get_Label_Float()), SkillEditor.SelectedFsm, functionCall.FloatParameter);
						break;
					case 2:
						functionCall.StringParameter = VariableEditor.FsmStringField(new GUIContent(Strings.get_Label_String()), SkillEditor.SelectedFsm, functionCall.StringParameter, attributes);
						break;
					case 3:
						functionCall.Vector2Parameter = VariableEditor.FsmVector2Field(new GUIContent(Strings.get_Label_Vector2()), SkillEditor.SelectedFsm, functionCall.Vector2Parameter);
						break;
					case 4:
						functionCall.Vector3Parameter = VariableEditor.FsmVector3Field(new GUIContent(Strings.get_Label_Vector3()), SkillEditor.SelectedFsm, functionCall.Vector3Parameter);
						break;
					case 5:
						functionCall.RectParamater = VariableEditor.FsmRectField(new GUIContent(Strings.get_Label_Rect()), SkillEditor.SelectedFsm, functionCall.RectParamater);
						break;
					case 6:
						functionCall.BoolParameter = VariableEditor.FsmBoolField(new GUIContent(Strings.get_Label_Bool()), SkillEditor.SelectedFsm, functionCall.BoolParameter);
						break;
					case 7:
						functionCall.GameObjectParameter = VariableEditor.FsmGameObjectField(new GUIContent(Strings.get_Label_GameObject()), SkillEditor.SelectedFsm, functionCall.GameObjectParameter);
						break;
					case 8:
						functionCall.MaterialParameter = VariableEditor.FsmMaterialField(new GUIContent(Strings.get_Label_Material()), SkillEditor.SelectedFsm, functionCall.MaterialParameter);
						break;
					case 9:
						functionCall.TextureParameter = VariableEditor.FsmTextureField(new GUIContent(Strings.get_Label_Texture()), SkillEditor.SelectedFsm, functionCall.TextureParameter);
						break;
					case 10:
						functionCall.ColorParameter = VariableEditor.FsmColorField(new GUIContent(Strings.get_Label_Color()), SkillEditor.SelectedFsm, functionCall.ColorParameter);
						break;
					case 11:
						functionCall.QuaternionParameter = VariableEditor.FsmQuaternionField(new GUIContent(Strings.get_Label_Quaternion()), SkillEditor.SelectedFsm, functionCall.QuaternionParameter);
						break;
					case 12:
						ActionEditor.editingType = functionCall.ObjectParameter.get_ObjectType();
						functionCall.ObjectParameter = VariableEditor.FsmObjectField(new GUIContent(Strings.get_Label_Object()), SkillEditor.SelectedFsm, functionCall.ObjectParameter, typeof(Object), attributes);
						break;
					case 13:
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						EditorGUILayout.PrefixLabel(Strings.get_Label_Enum_Type());
						if (GUILayout.Button(SkillEditorContent.TempContent(functionCall.EnumParameter.get_EnumType().get_Name(), functionCall.EnumParameter.get_EnumType().get_FullName()), EditorStyles.get_popup(), new GUILayoutOption[0]))
						{
							ActionEditor.contextMenuObject = functionCall;
							GenericMenu genericMenu = new GenericMenu();
							using (List<Type>.Enumerator enumerator = TypeHelpers.EnumTypeList.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									Type current = enumerator.get_Current();
									string fullName = current.get_FullName();
									string text2 = fullName.Replace('.', '/');
									genericMenu.AddItem(new GUIContent(text2), fullName == functionCall.EnumParameter.get_EnumType().get_FullName(), new GenericMenu.MenuFunction2(ActionEditor.SetFunctionCallEnumType), fullName);
								}
							}
							genericMenu.ShowAsContext();
						}
						GUILayout.EndHorizontal();
						ActionEditor.editingType = functionCall.EnumParameter.get_EnumType();
						functionCall.EnumParameter = VariableEditor.FsmEnumField(new GUIContent(Strings.get_Label_Enum_Value()), SkillEditor.SelectedFsm, functionCall.EnumParameter, functionCall.EnumParameter.get_EnumType());
						break;
					case 14:
						ActionEditor.contextFsmArray = functionCall.ArrayParameter;
						ActionEditor.contextTypeConstraint = 4;
						functionCall.ArrayParameter.SetType(4);
						functionCall.ArrayParameter = VariableEditor.FsmArrayField(new GUIContent("Array"), SkillEditor.SelectedFsm, functionCall.ArrayParameter, functionCall.ArrayParameter.get_TypeConstraint());
						break;
					}
				}
			}
			return functionCall;
		}
		private static void SetFunctionCallEnumType(object userdata)
		{
			FunctionCall functionCall = ActionEditor.contextMenuObject as FunctionCall;
			if (functionCall != null)
			{
				functionCall.EnumParameter.set_EnumType(ReflectionUtils.GetGlobalType(userdata as string));
			}
		}
		private SkillEventTarget EditFsmEventTarget(object fieldValue)
		{
			SkillEventTarget fsmEventTarget = ((SkillEventTarget)fieldValue) ?? new SkillEventTarget();
			ActionEditor.editingObject = fsmEventTarget;
			SkillEventTarget.EventTarget eventTarget = (SkillEventTarget.EventTarget)EditorGUILayout.EnumPopup(Strings.get_ActionEditor_Event_Target(), fsmEventTarget.target, new GUILayoutOption[0]);
			if (eventTarget != fsmEventTarget.target)
			{
				fsmEventTarget.target = eventTarget;
				fsmEventTarget.ResetParameters();
			}
			if (this.realActionName == Strings.get_ActionEditor_Set_Event_Target())
			{
				ActionEditor.FSMEventTargetContextGlobal = fsmEventTarget;
			}
			else
			{
				ActionEditor.fsmEventTargetContext = fsmEventTarget;
			}
			switch (fsmEventTarget.target)
			{
			case 1:
				if (FsmEditorSettings.ShowHints)
				{
					GUILayout.Box(Strings.get_Hint_Send_Event_to_GameObject(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
				}
				fsmEventTarget.gameObject = ActionEditor.EditFsmOwnerDefault(new GUIContent(Strings.get_Label_GameObject(), Strings.get_ActionEditor_Tooltip_Send_Event_to_GameObject()), fsmEventTarget.gameObject);
				ActionEditor.editingObject = fsmEventTarget;
				ActionEditor.editingField = ActionEditor.FsmEventTargetSendToChildrenField;
				fsmEventTarget.sendToChildren = VariableEditor.FsmBoolField(new GUIContent(Strings.get_ActionEditor_Send_To_Children(), Strings.get_ActionEditor_Send_To_Children_Tooltip()), SkillEditor.SelectedFsm, fsmEventTarget.sendToChildren);
				break;
			case 2:
				if (FsmEditorSettings.ShowHints)
				{
					GUILayout.Box(Strings.get_Hint_Send_Event_to_FSM_on_GameObject(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
				}
				fsmEventTarget.gameObject = ActionEditor.EditFsmOwnerDefault(new GUIContent(Strings.get_Label_GameObject(), Strings.get_ActionEditor_Send_Event_to_FSM_on_GameObject_Tooltip()), fsmEventTarget.gameObject);
				ActionEditor.editingObject = fsmEventTarget;
				ActionEditor.editingField = ActionEditor.FsmEventTargetFsmNameField;
				fsmEventTarget.fsmName = VariableEditor.FsmStringField(new GUIContent(Strings.get_ActionEditor_EditFsmEventTarget_FSM_Name(), Strings.get_ActionEditor_EditFsmEventTarget_FSM_Name_Tooltip()), SkillEditor.SelectedFsm, fsmEventTarget.fsmName, new object[]
				{
					new UIHintAttribute(15)
				});
				break;
			case 3:
				if (FsmEditorSettings.ShowHints)
				{
					GUILayout.Box(Strings.get_Hint_Send_Event_to_FsmComponent(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
				}
				fsmEventTarget.fsmComponent = (PlayMakerFSM)EditorGUILayout.ObjectField(new GUIContent(Strings.get_ActionEditor_EditFsmEventTarget_FSM_Component(), Strings.get_ActionEditor_EditFsmEventTarget_FSM_Component_Tooltip()), fsmEventTarget.fsmComponent, typeof(PlayMakerFSM), !SkillEditor.SelectedFsmIsPersistent(), new GUILayoutOption[0]);
				SkillEditorGUILayout.ReadonlyTextField((fsmEventTarget.fsmComponent == null) ? "" : fsmEventTarget.fsmComponent.get_Fsm().get_Name(), new GUILayoutOption[0]);
				break;
			case 4:
				if (FsmEditorSettings.ShowHints)
				{
					GUILayout.Box(Strings.get_Hint_Broadcast_Event(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
				}
				ActionEditor.editingField = ActionEditor.FsmEventTargetExcludeSelfField;
				fsmEventTarget.excludeSelf = VariableEditor.FsmBoolField(new GUIContent(Strings.get_ActionEditor_EditFsmEventTarget_Exclude_Self()), SkillEditor.SelectedFsm, fsmEventTarget.excludeSelf);
				break;
			case 5:
				if (FsmEditorSettings.ShowHints)
				{
					GUILayout.Box(Strings.get_Hint_Send_Event_To_Host(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
				}
				break;
			case 6:
				if (FsmEditorSettings.ShowHints)
				{
					GUILayout.Box(Strings.get_Hint_Send_Event_To_SubFSMs(), SkillEditorStyles.HintBox, new GUILayoutOption[0]);
				}
				break;
			}
			return fsmEventTarget;
		}
		private static string EditStringValue(GUIContent label, object fieldValue, object[] attributes)
		{
			string text = ((string)fieldValue) ?? string.Empty;
			UIHint uIHint = CustomAttributeHelpers.GetUIHint(attributes);
			switch (uIHint)
			{
			case 1:
				GUILayout.Label(label, new GUILayoutOption[0]);
				return EditorGUILayout.TextArea(text, SkillEditorStyles.TextArea, new GUILayoutOption[]
				{
					GUILayout.MinHeight(44f)
				});
			case 2:
				return ActionEditor.EditComponentName(label, text, typeof(Behaviour));
			case 3:
				text = ActionEditor.EditComponentName(label, text, typeof(MonoBehaviour));
				ActionEditor.TrySetBehaviourContext(text);
				return text;
			case 4:
				return ActionEditor.EditMethodName(label, text, false);
			case 5:
			case 8:
			case 10:
				break;
			case 6:
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				text = EditorGUILayout.TextField(label, text, new GUILayoutOption[0]);
				StringEditor.AnimationNamePopup(ActionEditor.GameObjectContext, null, ActionEditor.editingAction, ActionEditor.editingField);
				GUILayout.EndHorizontal();
				return text;
			case 7:
				return EditorGUILayout.TagField(label, text, new GUILayoutOption[0]);
			case 9:
				GUILayout.Box(text, SkillEditorStyles.InfoBox, new GUILayoutOption[0]);
				return text;
			case 11:
				return ActionEditor.EditScriptName(label, text);
			case 12:
				return EditorGUILayout.TextArea(text, SkillEditorStyles.TextArea, new GUILayoutOption[]
				{
					GUILayout.MinHeight(44f)
				});
			default:
				switch (uIHint)
				{
				case 32:
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					text = EditorGUILayout.TextField(label, text, new GUILayoutOption[0]);
					StringEditor.AnimatorFloatPopup(ActionEditor.GameObjectContext, null, ActionEditor.editingAction, ActionEditor.editingField);
					GUILayout.EndHorizontal();
					return text;
				case 33:
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					text = EditorGUILayout.TextField(label, text, new GUILayoutOption[0]);
					StringEditor.AnimatorBoolPopup(ActionEditor.GameObjectContext, null, ActionEditor.editingAction, ActionEditor.editingField);
					GUILayout.EndHorizontal();
					return text;
				case 34:
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					text = EditorGUILayout.TextField(label, text, new GUILayoutOption[0]);
					StringEditor.AnimatorIntPopup(ActionEditor.GameObjectContext, null, ActionEditor.editingAction, ActionEditor.editingField);
					GUILayout.EndHorizontal();
					return text;
				case 35:
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					text = EditorGUILayout.TextField(label, text, new GUILayoutOption[0]);
					StringEditor.AnimatorTriggerPopup(ActionEditor.GameObjectContext, null, ActionEditor.editingAction, ActionEditor.editingField);
					GUILayout.EndHorizontal();
					return text;
				}
				break;
			}
			return EditorGUILayout.TextField(label, text, new GUILayoutOption[0]);
		}
		public static string EditScriptName(GUIContent label, string name)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			name = EditorGUILayout.TextField(label, name, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			string text = SkillEditorGUILayout.ScriptListPopup();
			if (EditorGUI.EndChangeCheck() && text != "")
			{
				name = text;
				GUI.set_changed(true);
			}
			GUILayout.EndHorizontal();
			return name;
		}
		public static string EditComponentName(GUIContent label, string name, Type componentType)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			name = EditorGUILayout.TextField(label, name, new GUILayoutOption[0]);
			bool changed = GUI.get_changed();
			GUI.set_changed(false);
			string text = SkillEditorGUILayout.ComponentNamePopup(ActionEditor.GameObjectContext, componentType);
			if (GUI.get_changed())
			{
				name = text;
			}
			else
			{
				GUI.set_changed(changed);
			}
			GUILayout.EndHorizontal();
			return name;
		}
		[Obsolete]
		public static string EditFsmName(GUIContent label, string name)
		{
			return name;
		}
		public static string EditFsmEvent(GUIContent label, string name)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			name = EditorGUILayout.TextField(label, name, new GUILayoutOption[0]);
			bool changed = GUI.get_changed();
			GUI.set_changed(false);
			string text = SkillEditorGUILayout.FsmEventPopup(ActionEditor.GameObjectContext, ActionEditor.fsmNameContext);
			if (GUI.get_changed())
			{
				name = text;
			}
			else
			{
				GUI.set_changed(changed);
			}
			GUILayout.EndHorizontal();
			return name;
		}
		public static void AnimatorFloatPopup(SkillString fsmString)
		{
			StringEditor.AnimatorFloatPopup(ActionEditor.GameObjectContext, fsmString, null, null);
		}
		public static void AnimatorIntPopup(SkillString fsmString)
		{
			StringEditor.AnimatorIntPopup(ActionEditor.GameObjectContext, fsmString, null, null);
		}
		public static void AnimatorBoolPopup(SkillString fsmString)
		{
			StringEditor.AnimatorBoolPopup(ActionEditor.GameObjectContext, fsmString, null, null);
		}
		public static void AnimatorTriggerPopup(SkillString fsmString)
		{
			StringEditor.AnimatorTriggerPopup(ActionEditor.GameObjectContext, fsmString, null, null);
		}
		public static void AnimationNamePopup(SkillString fsmString)
		{
			StringEditor.AnimationNamePopup(ActionEditor.GameObjectContext, fsmString, null, null);
		}
		public static void FsmNamePopup(SkillString fsmString)
		{
			ActionEditor.fsmNameContext = fsmString.get_Value();
			StringEditor.FsmNamePopup(ActionEditor.GameObjectContext, fsmString, null, null);
		}
		public static void VariablePopup(SkillString fsmString, UIHint hint)
		{
			StringEditor.VariablesPopup(ActionEditor.GameObjectContext, ActionEditor.fsmNameContext, hint, fsmString);
		}
		[Obsolete]
		public static string EditFsmVariableName(GUIContent label, string name, UIHint hint)
		{
			return name;
		}
		public static string EditMethodName(GUIContent label, string name, bool coroutinesOnly)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			name = EditorGUILayout.TextField(label, name, new GUILayoutOption[0]);
			bool changed = GUI.get_changed();
			GUI.set_changed(false);
			string text = SkillEditorGUILayout.MethodNamePopup(ActionEditor.GameObjectContext, ActionEditor.behaviourContext, 1, coroutinesOnly);
			if (GUI.get_changed())
			{
				name = text;
			}
			else
			{
				GUI.set_changed(changed);
			}
			GUILayout.EndHorizontal();
			return name;
		}
		private static float EditFloatValue(GUIContent label, object fieldValue, IEnumerable<object> attributes)
		{
			HasFloatSliderAttribute attribute = CustomAttributeHelpers.GetAttribute<HasFloatSliderAttribute>(attributes);
			if (attribute == null)
			{
				return EditorGUILayout.FloatField(label, Convert.ToSingle(fieldValue), new GUILayoutOption[0]);
			}
			return SkillEditorGUILayout.FloatSlider(label, (float)fieldValue, attribute.get_MinValue(), attribute.get_MaxValue());
		}
		private static int EditIntValue(GUIContent label, object fieldValue, object[] attributes)
		{
			if (CustomAttributeHelpers.GetUIHint(attributes) != 8)
			{
				return EditorGUILayout.IntField(label, Convert.ToInt32(fieldValue, CultureInfo.get_CurrentCulture()), new GUILayoutOption[0]);
			}
			return EditorGUILayout.LayerField(label, Convert.ToInt32(fieldValue, CultureInfo.get_CurrentCulture()), new GUILayoutOption[0]);
		}
		public static SkillFloat EditFsmFloat(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillFloat fsmFloat = ((SkillFloat)fieldValue) ?? new SkillFloat(string.Empty);
			if (CustomAttributeHelpers.GetUIHint(attributes) == 10)
			{
				return VariableEditor.FsmFloatPopup(label, SkillEditor.SelectedFsm, fsmFloat);
			}
			HasFloatSliderAttribute attribute = CustomAttributeHelpers.GetAttribute<HasFloatSliderAttribute>(attributes);
			if (attribute == null)
			{
				return VariableEditor.FsmFloatField(label, SkillEditor.SelectedFsm, fsmFloat);
			}
			return VariableEditor.FsmFloatSlider(label, SkillEditor.SelectedFsm, fsmFloat, attribute.get_MinValue(), attribute.get_MaxValue());
		}
		private static SkillInt EditFsmInt(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillInt fsmInt = ((SkillInt)fieldValue) ?? new SkillInt("");
			if (CustomAttributeHelpers.GetUIHint(attributes) != 10)
			{
				return VariableEditor.FsmIntField(label, SkillEditor.SelectedFsm, fsmInt, attributes);
			}
			return VariableEditor.FsmIntPopup(label, SkillEditor.SelectedFsm, fsmInt);
		}
		private static SkillBool EditFsmBool(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillBool fsmBool = ((SkillBool)fieldValue) ?? new SkillBool("");
			if (CustomAttributeHelpers.GetUIHint(attributes) != 10)
			{
				return VariableEditor.FsmBoolField(label, SkillEditor.SelectedFsm, fsmBool);
			}
			return VariableEditor.FsmBoolPopup(label, SkillEditor.SelectedFsm, fsmBool);
		}
		private static SkillString EditFsmString(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillString fsmString = ((SkillString)fieldValue) ?? new SkillString();
			if (CustomAttributeHelpers.GetUIHint(attributes) != 10)
			{
				return VariableEditor.FsmStringField(label, SkillEditor.SelectedFsm, fsmString, attributes);
			}
			return VariableEditor.FsmStringPopup(label, SkillEditor.SelectedFsm, fsmString);
		}
		private static SkillVector2 EditFsmVector2(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillVector2 fsmVector = ((SkillVector2)fieldValue) ?? new SkillVector2();
			if (CustomAttributeHelpers.GetUIHint(attributes) != 10)
			{
				return VariableEditor.FsmVector2Field(label, SkillEditor.SelectedFsm, fsmVector);
			}
			return VariableEditor.FsmVector2Popup(label, SkillEditor.SelectedFsm, fsmVector);
		}
		private static SkillVector3 EditFsmVector3(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillVector3 fsmVector = ((SkillVector3)fieldValue) ?? new SkillVector3();
			if (CustomAttributeHelpers.GetUIHint(attributes) != 10)
			{
				return VariableEditor.FsmVector3Field(label, SkillEditor.SelectedFsm, fsmVector);
			}
			return VariableEditor.FsmVector3Popup(label, SkillEditor.SelectedFsm, fsmVector);
		}
		private static SkillRect EditFsmRect(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillRect fsmRect = ((SkillRect)fieldValue) ?? new SkillRect();
			if (CustomAttributeHelpers.GetUIHint(attributes) != 10)
			{
				return VariableEditor.FsmRectField(label, SkillEditor.SelectedFsm, fsmRect);
			}
			return VariableEditor.FsmRectPopup(label, SkillEditor.SelectedFsm, fsmRect);
		}
		private static SkillQuaternion EditFsmQuaternion(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillQuaternion fsmQauternion = ((SkillQuaternion)fieldValue) ?? new SkillQuaternion();
			if (CustomAttributeHelpers.GetUIHint(attributes) != 10)
			{
				return VariableEditor.FsmQuaternionField(label, SkillEditor.SelectedFsm, fsmQauternion);
			}
			return VariableEditor.FsmQuaternionPopup(label, SkillEditor.SelectedFsm, fsmQauternion);
		}
		private static SkillObject EditFsmObject(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillObject fsmObject = ((SkillObject)fieldValue) ?? new SkillObject();
			if (CustomAttributeHelpers.GetUIHint(attributes) != 10)
			{
				return VariableEditor.FsmObjectField(label, SkillEditor.SelectedFsm, fsmObject, CustomAttributeHelpers.GetObjectType(attributes, fsmObject.get_ObjectType()), attributes);
			}
			return VariableEditor.FsmObjectPopup(label, SkillEditor.SelectedFsm, fsmObject, CustomAttributeHelpers.GetObjectType(attributes, null));
		}
		private static SkillMaterial EditFsmMaterial(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillMaterial fsmObject = ((SkillMaterial)fieldValue) ?? new SkillMaterial();
			if (CustomAttributeHelpers.GetUIHint(attributes) != 10)
			{
				return VariableEditor.FsmMaterialField(label, SkillEditor.SelectedFsm, fsmObject);
			}
			return VariableEditor.FsmMaterialPopup(label, SkillEditor.SelectedFsm, fsmObject);
		}
		private static SkillTexture EditFsmTexture(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillTexture fsmObject = ((SkillTexture)fieldValue) ?? new SkillTexture();
			if (CustomAttributeHelpers.GetUIHint(attributes) != 10)
			{
				return VariableEditor.FsmTextureField(label, SkillEditor.SelectedFsm, fsmObject);
			}
			return VariableEditor.FsmTexturePopup(label, SkillEditor.SelectedFsm, fsmObject);
		}
		private static SkillColor EditFsmColor(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillColor fsmColor = ((SkillColor)fieldValue) ?? new SkillColor();
			if (CustomAttributeHelpers.GetUIHint(attributes) != 10)
			{
				return VariableEditor.FsmColorField(label, SkillEditor.SelectedFsm, fsmColor);
			}
			return VariableEditor.FsmColorPopup(label, SkillEditor.SelectedFsm, fsmColor);
		}
		private static SkillGameObject EditFsmGameObject(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillGameObject fsmGameObject = ((SkillGameObject)fieldValue) ?? new SkillGameObject(string.Empty);
			if (CustomAttributeHelpers.GetUIHint(attributes) == 10)
			{
				return VariableEditor.FsmGameObjectPopup(label, SkillEditor.SelectedFsm, fsmGameObject);
			}
			fsmGameObject = VariableEditor.FsmGameObjectField(label, SkillEditor.SelectedFsm, fsmGameObject);
			if (fsmGameObject.get_Value() != null)
			{
				ActionEditor.GameObjectContext = fsmGameObject.get_Value();
			}
			return fsmGameObject;
		}
		private SkillEnum EditFsmEnum(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillEnum fsmEnum = ((SkillEnum)fieldValue) ?? new SkillEnum();
			Type type = null;
			ObjectTypeAttribute attribute = CustomAttributeHelpers.GetAttribute<ObjectTypeAttribute>(attributes);
			if (attribute != null)
			{
				type = attribute.get_ObjectType();
				fsmEnum.set_EnumType(type);
			}
			MatchFieldTypeAttribute attribute2 = CustomAttributeHelpers.GetAttribute<MatchFieldTypeAttribute>(attributes);
			if (attribute2 != null)
			{
				this.MatchFieldType(fsmEnum, attribute2.get_FieldName());
				type = fsmEnum.get_EnumType();
			}
			if (type == null && !CustomAttributeHelpers.HasAttribute<HideTypeFilter>(attributes))
			{
				VariableEditor.EnumTypeSelector(fsmEnum);
			}
			bool flag = CustomAttributeHelpers.GetUIHint(attributes) == 10;
			if (flag)
			{
				return VariableEditor.FsmEnumPopup(label, SkillEditor.SelectedFsm, fsmEnum, type);
			}
			return VariableEditor.FsmEnumField(label, SkillEditor.SelectedFsm, fsmEnum, fsmEnum.get_EnumType());
		}
		private SkillArray EditFsmArray(GUIContent label, object fieldValue, object[] attributes)
		{
			SkillArray fsmArray = ((SkillArray)fieldValue) ?? new SkillArray();
			ActionEditor.editingFsmArray = fsmArray;
			bool flag = CustomAttributeHelpers.GetUIHint(attributes) == 10;
			ArrayEditorAttribute attribute = CustomAttributeHelpers.GetAttribute<ArrayEditorAttribute>(attributes);
			VariableType variableType;
			if (attribute != null)
			{
				variableType = attribute.get_VariableType();
				if (attribute.get_ObjectType() != null)
				{
					fsmArray.set_ObjectType(attribute.get_ObjectType());
				}
			}
			else
			{
				variableType = fsmArray.get_ElementType();
			}
			if (flag)
			{
				ActionEditor.editingTypeConstraint = variableType;
				return VariableEditor.FsmArrayPopup(label, SkillEditor.SelectedFsm, fsmArray, variableType);
			}
			if (variableType == -1 && fsmArray.get_ElementType() == -1)
			{
				throw new ArrayTypeMismatchException(Strings.get_EditFsmArray_Unknown_FsmArray_Type());
			}
			if (attribute == null)
			{
				variableType = fsmArray.get_ElementType();
			}
			fsmArray = VariableEditor.FsmArrayField(label, SkillEditor.SelectedFsm, fsmArray, variableType);
			ActionEditor.editingTypeConstraint = variableType;
			if (!fsmArray.get_UseVariable())
			{
				Type type = SkillUtility.GetVariableRealType(variableType);
				if (type == typeof(Object))
				{
					type = fsmArray.get_ObjectType();
				}
				fsmArray.set_Values((object[])this.EditArray(new GUIContent(Strings.get_Label_Size()), type, fsmArray.get_Values(), attributes));
				if (this.actionIsDirty)
				{
					fsmArray.SaveChanges();
				}
			}
			ActionEditor.editingFsmArray = null;
			return fsmArray;
		}
		[Localizable(false)]
		private static SkillOwnerDefault EditFsmOwnerDefault(GUIContent label, object fieldValue)
		{
			SkillOwnerDefault fsmOwnerDefault = ((SkillOwnerDefault)fieldValue) ?? new SkillOwnerDefault();
			fsmOwnerDefault.set_OwnerOption((OwnerDefaultOption)EditorGUILayout.EnumPopup(label, fsmOwnerDefault.get_OwnerOption(), new GUILayoutOption[0]));
			if (fsmOwnerDefault.get_OwnerOption() == 1)
			{
				ActionEditor.editingObject = fsmOwnerDefault;
				ActionEditor.editingField = ActionEditor.FsmOwnerDefaultGameObjectField;
				fsmOwnerDefault.set_GameObject(VariableEditor.FsmGameObjectField(new GUIContent("  "), SkillEditor.SelectedFsm, fsmOwnerDefault.get_GameObject()));
				if (fsmOwnerDefault.get_GameObject().get_Value() != null)
				{
					ActionEditor.GameObjectContext = fsmOwnerDefault.get_GameObject().get_Value();
				}
			}
			else
			{
				if (SkillEditor.SelectedFsm != null)
				{
					ActionEditor.GameObjectContext = SkillEditor.SelectedFsm.get_GameObject();
				}
			}
			return fsmOwnerDefault;
		}
		public static void TrySetBehaviourContext(string behaviourName)
		{
			if (ActionEditor.GameObjectContext == null || string.IsNullOrEmpty(behaviourName))
			{
				return;
			}
			Type globalType = ReflectionUtils.GetGlobalType(behaviourName);
			if (globalType != null)
			{
				ActionEditor.behaviourContext = (MonoBehaviour)ActionEditor.GameObjectContext.GetComponent(globalType);
			}
		}
		public static void DoVariableSelector(GUIContent label, Skill fsm, VariableType variableType, NamedVariable selected, VariableType typeConstraint = -1, Type objectConstraint = null)
		{
			ActionEditor.editingVariable = selected;
			ActionEditor.editingVariableType = variableType;
			ActionEditor.editingTypeConstraint = typeConstraint;
			ActionEditor.editingType = objectConstraint;
			if (selected != null && variableType != -1 && (selected.get_VariableType() != variableType || !selected.TestTypeConstraint(typeConstraint, objectConstraint)))
			{
				if (selected.get_UsesVariable())
				{
					ActionEditor.editingField.SetValue(ActionEditor.editingObject, SkillVariable.ResetVariableReference(selected));
					ActionEditor.exitGUI = true;
					return;
				}
				selected = null;
			}
			string text = (selected != null) ? selected.GetDisplayName() : Strings.get_Label_None();
			EditorGUILayout.PrefixLabel(label);
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.get_popup());
			if (GUI.Button(rect, text, EditorStyles.get_popup()))
			{
				ActionEditor.SaveEditingContext();
				if (Event.get_current().get_control())
				{
					if (ActionEditor.CanAddNewVariable(variableType, typeConstraint, objectConstraint))
					{
						SkillProperty fsmProperty = ActionEditor.editingObject as SkillProperty;
						if (fsmProperty != null)
						{
							ActionEditor.DoNewVariableDropdown(rect, fsmProperty.PropertyName);
							return;
						}
						ActionEditor.DoNewVariableDropdown(rect, label.get_text());
						return;
					}
				}
				else
				{
					GenericMenu genericMenu = new GenericMenu();
					genericMenu.AddItem(new GUIContent(Strings.get_Label_None()), selected == null || selected.get_IsNone(), new GenericMenu.MenuFunction2(ActionEditor.DoVariableSelection), null);
					if (fsm != null)
					{
						NamedVariable[] namedVariables = fsm.get_Variables().GetNamedVariables(variableType);
						List<NamedVariable> list = new List<NamedVariable>();
						NamedVariable[] array = namedVariables;
						for (int i = 0; i < array.Length; i++)
						{
							NamedVariable namedVariable = array[i];
							if (namedVariable.TestTypeConstraint(typeConstraint, objectConstraint))
							{
								list.Add(namedVariable);
							}
						}
						list.Sort();
						using (List<NamedVariable>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								NamedVariable current = enumerator.get_Current();
								genericMenu.AddItem(new GUIContent(current.get_Name()), selected == current, new GenericMenu.MenuFunction2(ActionEditor.DoVariableSelection), current);
							}
						}
					}
					NamedVariable[] namedVariables2 = SkillVariables.get_GlobalVariables().GetNamedVariables(variableType);
					List<NamedVariable> list2 = new List<NamedVariable>();
					NamedVariable[] array2 = namedVariables2;
					for (int j = 0; j < array2.Length; j++)
					{
						NamedVariable namedVariable2 = array2[j];
						if (namedVariable2.TestTypeConstraint(typeConstraint, objectConstraint))
						{
							list2.Add(namedVariable2);
						}
					}
					list2.Sort();
					using (List<NamedVariable>.Enumerator enumerator2 = list2.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							NamedVariable current2 = enumerator2.get_Current();
							genericMenu.AddItem(new GUIContent(Strings.get_Menu_GlobalsRoot() + current2.get_Name()), selected == current2, new GenericMenu.MenuFunction2(ActionEditor.DoVariableSelection), current2);
						}
					}
					if (ActionEditor.CanAddNewVariable(ActionEditor.contextVariableType, typeConstraint, objectConstraint))
					{
						genericMenu.AddSeparator("");
						genericMenu.AddItem(new GUIContent(Strings.get_Menu_NewVariable()), false, new GenericMenu.MenuFunction2(ActionEditor.DoNewVariableDropdown), rect);
						genericMenu.AddItem(new GUIContent(Strings.get_Menu_NewGlobalVariable()), false, new GenericMenu.MenuFunction2(ActionEditor.DoNewGlobalVariableDropdown), rect);
					}
					genericMenu.DropDown(rect);
				}
			}
		}
		private static bool CanAddNewVariable(VariableType variableType, VariableType typeConstraint, Type objectConstraint)
		{
			if (variableType == 13)
			{
				return ActionEditor.CanAddNewVariable(typeConstraint, typeConstraint, objectConstraint);
			}
			if (variableType == -1)
			{
				return false;
			}
			if (variableType == 12 || variableType == 14)
			{
				if (objectConstraint == typeof(MonoBehaviour))
				{
					return false;
				}
				if (objectConstraint == typeof(Component))
				{
					return false;
				}
				if (objectConstraint == typeof(Object))
				{
					return false;
				}
				if (objectConstraint == typeof(Enum))
				{
					return false;
				}
				if (objectConstraint != null && objectConstraint.get_IsAbstract())
				{
					return false;
				}
			}
			return true;
		}
		private static void DoNewVariableDropdown(object userdata)
		{
			Rect buttonRect = (Rect)userdata;
			ActionEditor.DoNewVariableDropdown(buttonRect, "");
		}
		private static void DoNewVariableDropdown(Rect buttonRect, string variableName)
		{
			buttonRect.set_x(buttonRect.get_x() + (SkillEditor.Window.get_position().get_x() + SkillEditor.Window.get_position().get_width() - 350f));
			buttonRect.set_y(buttonRect.get_y() + (SkillEditor.Window.get_position().get_y() + StateInspector.ActionsPanelRect.get_y() + 3f - SkillEditor.StateInspector.scrollPosition.y));
			NewVariableWindow newVariableWindow = NewVariableWindow.CreateDropdown(Strings.get_Title_NewVariable(), buttonRect, SkillEditor.SelectedFsm.get_Variables(), variableName);
			NewVariableWindow expr_96 = newVariableWindow;
			expr_96.EditCommited = (TextField.EditCommitedCallback)Delegate.Combine(expr_96.EditCommited, new TextField.EditCommitedCallback(ActionEditor.DoNewVariable));
		}
		private static void DoNewVariable(TextField textField)
		{
			SkillEditor.RegisterUndo(Strings.get_Command_Add_Variable());
			SkillVariable.AddVariable(SkillEditor.SelectedFsm.get_Variables(), ActionEditor.contextVariableType, textField.Text, ActionEditor.contextType, ActionEditor.contextTypeConstraint);
			NamedVariable variable = SkillEditor.SelectedFsm.get_Variables().GetVariable(textField.Text);
			ActionEditor.DoVariableSelection(variable);
		}
		private static void DoNewGlobalVariableDropdown(object userdata)
		{
			Rect buttonRect = (Rect)userdata;
			buttonRect.set_x(buttonRect.get_x() + (SkillEditor.Window.get_position().get_x() + SkillEditor.Window.get_position().get_width() - 350f));
			buttonRect.set_y(buttonRect.get_y() + (SkillEditor.Window.get_position().get_y() + StateInspector.ActionsPanelRect.get_y() + 3f - SkillEditor.StateInspector.scrollPosition.y));
			NewVariableWindow newVariableWindow = NewVariableWindow.CreateDropdown(Strings.get_Title_NewGlobalVariable(), buttonRect, SkillVariables.get_GlobalVariables(), "");
			NewVariableWindow expr_9D = newVariableWindow;
			expr_9D.EditCommited = (TextField.EditCommitedCallback)Delegate.Combine(expr_9D.EditCommited, new TextField.EditCommitedCallback(ActionEditor.DoNewGlobalVariable));
		}
		private static void DoNewGlobalVariable(TextField textField)
		{
			SkillEditor.RegisterGlobalsUndo(Strings.get_Command_Add_Variable());
			SkillVariable.AddVariable(SkillVariables.get_GlobalVariables(), ActionEditor.contextVariableType, textField.Text, ActionEditor.contextType, ActionEditor.contextTypeConstraint);
			NamedVariable variable = SkillVariables.get_GlobalVariables().GetVariable(textField.Text);
			GlobalVariablesWindow.ResetView();
			ActionEditor.DoVariableSelection(variable);
		}
		[Localizable(false)]
		private static void DoVariableSelection(object userdata)
		{
			NamedVariable namedVariable = userdata as NamedVariable;
			if (namedVariable == null)
			{
				namedVariable = SkillVariable.GetNewVariableOfSameType(ActionEditor.contextVariable);
				if (namedVariable != null)
				{
					namedVariable.set_UseVariable(true);
				}
			}
			Array array = ActionEditor.contextMenuObject as Array;
			if (array != null)
			{
				array.SetValue(namedVariable, ActionEditor.contextMenuIndex);
			}
			else
			{
				if (ActionEditor.contextMenuObject is SkillVar)
				{
					SkillVar fsmVar = (SkillVar)ActionEditor.contextMenuObject;
					fsmVar.set_NamedVar(namedVariable);
				}
				else
				{
					if (ActionEditor.contextMenuObject is SkillProperty)
					{
						SkillProperty fsmProperty = (SkillProperty)ActionEditor.contextMenuObject;
						if (ActionEditor.contextMenuFieldName == "TargetObject")
						{
							fsmProperty.TargetObject = (namedVariable as SkillObject);
						}
						else
						{
							if (ActionEditor.contextMenuFieldName == "SetVariable")
							{
								fsmProperty.SetVariable(namedVariable);
								if (namedVariable != null)
								{
									namedVariable.set_ObjectType(fsmProperty.PropertyType.get_IsArray() ? fsmProperty.PropertyType.GetElementType() : fsmProperty.PropertyType);
								}
							}
						}
					}
					else
					{
						if (ActionEditor.contextMenuObject is FunctionCall)
						{
							FunctionCall functionCall = (FunctionCall)ActionEditor.contextMenuObject;
							string parameterType;
							if ((parameterType = functionCall.get_ParameterType()) != null)
							{
								if (<PrivateImplementationDetails>{9F518054-9A7A-4388-8A0B-9CF872B8F518}.$$method0x60003f7-1 == null)
								{
									Dictionary<string, int> expr_123 = new Dictionary<string, int>(16);
									expr_123.Add("None", 0);
									expr_123.Add("bool", 1);
									expr_123.Add("int", 2);
									expr_123.Add("float", 3);
									expr_123.Add("string", 4);
									expr_123.Add("Vector2", 5);
									expr_123.Add("Vector3", 6);
									expr_123.Add("Rect", 7);
									expr_123.Add("GameObject", 8);
									expr_123.Add("Material", 9);
									expr_123.Add("Texture", 10);
									expr_123.Add("Color", 11);
									expr_123.Add("Quaternion", 12);
									expr_123.Add("Object", 13);
									expr_123.Add("Enum", 14);
									expr_123.Add("Array", 15);
									<PrivateImplementationDetails>{9F518054-9A7A-4388-8A0B-9CF872B8F518}.$$method0x60003f7-1 = expr_123;
								}
								int num;
								if (<PrivateImplementationDetails>{9F518054-9A7A-4388-8A0B-9CF872B8F518}.$$method0x60003f7-1.TryGetValue(parameterType, ref num))
								{
									switch (num)
									{
									case 1:
										functionCall.BoolParameter = (namedVariable as SkillBool);
										break;
									case 2:
										functionCall.IntParameter = (namedVariable as SkillInt);
										break;
									case 3:
										functionCall.FloatParameter = (namedVariable as SkillFloat);
										break;
									case 4:
										functionCall.StringParameter = (namedVariable as SkillString);
										break;
									case 5:
										functionCall.Vector2Parameter = (namedVariable as SkillVector2);
										break;
									case 6:
										functionCall.Vector3Parameter = (namedVariable as SkillVector3);
										break;
									case 7:
										functionCall.RectParamater = (namedVariable as SkillRect);
										break;
									case 8:
										functionCall.GameObjectParameter = (namedVariable as SkillGameObject);
										break;
									case 9:
										functionCall.MaterialParameter = (namedVariable as SkillMaterial);
										break;
									case 10:
										functionCall.TextureParameter = (namedVariable as SkillTexture);
										break;
									case 11:
										functionCall.ColorParameter = (namedVariable as SkillColor);
										break;
									case 12:
										functionCall.QuaternionParameter = (namedVariable as SkillQuaternion);
										break;
									case 13:
										functionCall.ObjectParameter = (namedVariable as SkillObject);
										break;
									case 14:
										functionCall.EnumParameter = (namedVariable as SkillEnum);
										break;
									case 15:
										functionCall.ArrayParameter = (namedVariable as SkillArray);
										break;
									}
								}
							}
						}
						else
						{
							ActionEditor.contextMenuField.SetValue(ActionEditor.contextMenuObject, namedVariable);
						}
					}
				}
			}
			SkillEditor.SaveActions();
			SkillEditor.SetFsmDirty();
			ActionEditor.UpdateErrorChecks();
		}
		private static void UpdateErrorChecks()
		{
			if (!EditorApplication.get_isPlayingOrWillChangePlaymode() && FsmErrorChecker.FsmHasErrors(SkillEditor.SelectedFsm))
			{
				ActionReport.Remove(SkillEditor.SelectedFsm.get_Owner() as PlayMakerFSM);
				SkillEditor.SelectedFsm.Reinitialize();
			}
		}
		public static void EventSelector(GUIContent label, SkillEvent selected, SkillEventTarget eventTarget)
		{
			string text = (selected != null) ? selected.get_Name() : "";
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel(label);
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.get_popup());
			if (GUI.Button(rect, text, EditorStyles.get_popup()))
			{
				ActionEditor.SaveEditingContext();
				if (Event.get_current().get_control())
				{
					ActionEditor.DoNewEvent(rect);
				}
				else
				{
					GenericMenu genericMenu = ActionEditor.GenerateEventMenu(SkillEditor.SelectedFsm, selected, eventTarget, rect);
					genericMenu.DropDown(rect);
				}
			}
			GUILayout.EndHorizontal();
		}
		private static GenericMenu GenerateEventMenu(Skill fsm, SkillEvent selected, SkillEventTarget eventTarget, Rect buttonRect)
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Label_None()), selected == null, new GenericMenu.MenuFunction2(ActionEditor.DoEventSelection), null);
			if (eventTarget == null)
			{
				eventTarget = SkillEventTarget.get_Self();
			}
			switch (eventTarget.target)
			{
			case 0:
				genericMenu = ActionEditor.AddFsmEventsMenu(fsm, genericMenu, selected, new GenericMenu.MenuFunction2(ActionEditor.DoEventSelection));
				genericMenu.AddSeparator("");
				genericMenu = ActionEditor.AddCommonEventMenus(genericMenu, "", selected, new GenericMenu.MenuFunction2(ActionEditor.DoEventSelection));
				genericMenu = ActionEditor.AddNewEvent(genericMenu, buttonRect);
				break;
			case 1:
			case 4:
			case 6:
				genericMenu = ActionEditor.AddFsmGlobalEventsMenu(fsm, genericMenu, selected, new GenericMenu.MenuFunction2(ActionEditor.DoEventSelection));
				genericMenu.AddSeparator("");
				genericMenu = ActionEditor.AddGlobalEventsMenus(genericMenu, "", selected, new GenericMenu.MenuFunction2(ActionEditor.DoEventSelection));
				genericMenu = ActionEditor.AddNewGlobalEvent(genericMenu, buttonRect);
				break;
			case 2:
				if (fsm != null)
				{
					GameObject ownerDefaultTarget = fsm.GetOwnerDefaultTarget(eventTarget.gameObject);
					Skill targetFSM = ActionEditor.GetTargetFSM(ownerDefaultTarget, eventTarget.fsmName);
					genericMenu = ActionEditor.AddFsmGlobalEventsMenu(targetFSM, genericMenu, selected, new GenericMenu.MenuFunction2(ActionEditor.DoEventSelection));
				}
				genericMenu.AddSeparator("");
				genericMenu = ActionEditor.AddGlobalEventsMenus(genericMenu, "", selected, new GenericMenu.MenuFunction2(ActionEditor.DoEventSelection));
				genericMenu = ActionEditor.AddNewGlobalEvent(genericMenu, buttonRect);
				break;
			case 3:
				if (eventTarget.fsmComponent != null)
				{
					genericMenu = ActionEditor.AddFsmGlobalEventsMenu(eventTarget.fsmComponent.get_Fsm(), genericMenu, selected, new GenericMenu.MenuFunction2(ActionEditor.DoEventSelection));
				}
				genericMenu.AddSeparator("");
				genericMenu = ActionEditor.AddGlobalEventsMenus(genericMenu, "", selected, new GenericMenu.MenuFunction2(ActionEditor.DoEventSelection));
				genericMenu = ActionEditor.AddNewGlobalEvent(genericMenu, buttonRect);
				break;
			case 5:
				genericMenu.AddSeparator("");
				genericMenu = ActionEditor.AddFinishedEvent(genericMenu, selected, new GenericMenu.MenuFunction2(ActionEditor.DoEventSelection));
				genericMenu = ActionEditor.AddCommonEventMenus(genericMenu, "", selected, new GenericMenu.MenuFunction2(ActionEditor.DoEventSelection));
				genericMenu = ActionEditor.AddNewEvent(genericMenu, buttonRect);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return genericMenu;
		}
		[Localizable(false)]
		public static GenericMenu AddFinishedEvent(GenericMenu menu, SkillEvent selectedEvent, GenericMenu.MenuFunction2 menuFunction)
		{
			menu.AddItem(new GUIContent("FINISHED"), selectedEvent == SkillEvent.get_Finished(), menuFunction, SkillEvent.get_Finished());
			return menu;
		}
		public static GenericMenu AddEventListMenu(GenericMenu menu, string menuRoot, IEnumerable<SkillEvent> eventList, SkillEvent selectedEvent, GenericMenu.MenuFunction2 menuFunction)
		{
			string text = (selectedEvent != null) ? selectedEvent.get_Name() : "";
			if (eventList != null)
			{
				using (IEnumerator<SkillEvent> enumerator = eventList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SkillEvent current = enumerator.get_Current();
						menu.AddItem(new GUIContent(current.get_Name()), text == current.get_Name(), new GenericMenu.MenuFunction2(ActionEditor.DoEventSelection), current);
					}
				}
			}
			return menu;
		}
		public static GenericMenu AddFsmEventsMenu(Skill fsm, GenericMenu menu, SkillEvent selectedEvent, GenericMenu.MenuFunction2 menuFunction)
		{
			menu = ActionEditor.AddEventListMenu(menu, "", new List<SkillEvent>((fsm != null) ? fsm.get_Events() : new SkillEvent[0]), selectedEvent, menuFunction);
			return menu;
		}
		public static GenericMenu AddCommonEventMenus(GenericMenu menu, string menuRoot, SkillEvent selectedEvent, GenericMenu.MenuFunction2 menuFunction)
		{
			menu = ActionEditor.AddCustomEventsMenu(menu, menuRoot, selectedEvent, menuFunction);
			menu = ActionEditor.AddSystemEventsMenu(menu, menuRoot, selectedEvent, menuFunction);
			return menu;
		}
		public static GenericMenu AddFsmGlobalEventsMenu(Skill fsm, GenericMenu menu, SkillEvent selectedEvent, GenericMenu.MenuFunction2 menuFunction)
		{
			string text = (selectedEvent != null) ? selectedEvent.get_Name() : "";
			if (fsm != null)
			{
				SkillEvent[] events = fsm.get_Events();
				for (int i = 0; i < events.Length; i++)
				{
					SkillEvent fsmEvent = events[i];
					if (fsmEvent.get_IsGlobal())
					{
						menu.AddItem(new GUIContent(fsmEvent.get_Name()), text == fsmEvent.get_Name(), new GenericMenu.MenuFunction2(ActionEditor.DoEventSelection), fsmEvent);
					}
				}
			}
			return menu;
		}
		public static GenericMenu AddCustomEventsMenu(GenericMenu menu, string eventRoot, SkillEvent selectedEvent, GenericMenu.MenuFunction2 menuFunction)
		{
			string text = (selectedEvent != null) ? selectedEvent.get_Name() : "";
			string text2 = Menus.MakeMenuRoot(eventRoot + Strings.get_Menu_GraphView_CustomEvents());
			List<SkillEvent> list = Enumerable.ToList<SkillEvent>(Enumerable.OrderBy<SkillEvent, string>(SkillEvent.get_EventList(), (SkillEvent o) => o.get_Name()));
			using (List<SkillEvent>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					if (!current.get_IsSystemEvent())
					{
						menu.AddItem(new GUIContent(text2 + current.get_Name()), text == current.get_Name(), menuFunction, current);
					}
				}
			}
			return menu;
		}
		public static GenericMenu AddGlobalEventsMenus(GenericMenu menu, string eventRoot, SkillEvent selectedEvent, GenericMenu.MenuFunction2 menuFunction)
		{
			string text = (selectedEvent != null) ? selectedEvent.get_Name() : "";
			string text2 = Menus.MakeMenuRoot(eventRoot + Strings.get_Menu_GraphView_GlobalEvents());
			List<SkillEvent> list = Enumerable.ToList<SkillEvent>(Enumerable.OrderBy<SkillEvent, string>(SkillEvent.get_EventList(), (SkillEvent o) => o.get_Name()));
			using (List<SkillEvent>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					if (current.get_IsGlobal())
					{
						menu.AddItem(new GUIContent(text2 + current.get_Name()), text == current.get_Name(), menuFunction, current);
					}
				}
			}
			return menu;
		}
		public static GenericMenu AddSystemEventsMenu(GenericMenu menu, string eventRoot, SkillEvent selectedEvent, GenericMenu.MenuFunction2 menuFunction)
		{
			string text = (selectedEvent != null) ? selectedEvent.get_Name() : "";
			List<SkillEvent> list = Enumerable.ToList<SkillEvent>(Enumerable.OrderBy<SkillEvent, string>(SkillEvent.get_EventList(), (SkillEvent o) => o.get_Name()));
			using (List<SkillEvent>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					if (current.get_IsSystemEvent() && current != SkillEvent.get_Finished())
					{
						menu.AddItem(new GUIContent(eventRoot + current.get_Path() + current.get_Name()), text == current.get_Name(), menuFunction, current);
					}
				}
			}
			return menu;
		}
		public static GenericMenu AddNewEvent(GenericMenu menu, Rect buttonRect)
		{
			menu.AddSeparator("");
			menu.AddItem(new GUIContent(Strings.get_Menu_NewEvent()), false, new GenericMenu.MenuFunction2(ActionEditor.DoNewEvent), buttonRect);
			return menu;
		}
		public static GenericMenu AddNewGlobalEvent(GenericMenu menu, Rect buttonRect)
		{
			menu.AddSeparator("");
			menu.AddItem(new GUIContent(Strings.get_Menu_NewGlobalEvent()), false, new GenericMenu.MenuFunction2(ActionEditor.DoNewGlobalEvent), buttonRect);
			return menu;
		}
		private static void DoNewEvent(object userdata)
		{
			Rect buttonRect = (Rect)userdata;
			ActionEditor.DoNewEvent(buttonRect, "");
		}
		private static void DoNewGlobalEvent(object userdata)
		{
			Rect buttonRect = (Rect)userdata;
			ActionEditor.DoNewGlobalEvent(buttonRect, "");
		}
		private static void DoNewEvent(Rect buttonRect, string eventName)
		{
			buttonRect.set_x(buttonRect.get_x() + (SkillEditor.Window.get_position().get_x() + SkillEditor.Window.get_position().get_width() - 350f));
			buttonRect.set_y(buttonRect.get_y() + (SkillEditor.Window.get_position().get_y() + StateInspector.ActionsPanelRect.get_y() + 3f - SkillEditor.StateInspector.scrollPosition.y));
			NewEventWindow newEventWindow = NewEventWindow.CreateDropdown(Strings.get_Title_NewEvent(), buttonRect, eventName);
			NewEventWindow expr_8C = newEventWindow;
			expr_8C.EditCommited = (TextField.EditCommitedCallback)Delegate.Combine(expr_8C.EditCommited, new TextField.EditCommitedCallback(ActionEditor.DoNewEvent));
		}
		private static void DoNewGlobalEvent(Rect buttonRect, string eventName)
		{
			buttonRect.set_x(buttonRect.get_x() + (SkillEditor.Window.get_position().get_x() + SkillEditor.Window.get_position().get_width() - 350f));
			buttonRect.set_y(buttonRect.get_y() + (SkillEditor.Window.get_position().get_y() + StateInspector.ActionsPanelRect.get_y() + 3f - SkillEditor.StateInspector.scrollPosition.y));
			NewEventWindow newEventWindow = NewEventWindow.CreateDropdown(Strings.get_Command_New_Global_Event(), buttonRect, eventName);
			NewEventWindow expr_8C = newEventWindow;
			expr_8C.EditCommited = (TextField.EditCommitedCallback)Delegate.Combine(expr_8C.EditCommited, new TextField.EditCommitedCallback(ActionEditor.DoNewGlobalEvent));
		}
		private static void DoNewEvent(TextField textField)
		{
			ActionEditor.DoEventSelection(SkillEvent.GetFsmEvent(textField.Text));
		}
		private static void DoNewGlobalEvent(TextField textField)
		{
			SkillEvent fsmEvent = SkillEvent.GetFsmEvent(textField.Text);
			fsmEvent.set_IsGlobal(true);
			ActionEditor.DoEventSelection(fsmEvent);
		}
		private static void DoEventSelection(object userdata)
		{
			SkillEvent fsmEvent = userdata as SkillEvent;
			Array array = ActionEditor.contextMenuObject as Array;
			if (array != null)
			{
				array.SetValue(fsmEvent, ActionEditor.contextMenuIndex);
			}
			else
			{
				ActionEditor.contextMenuField.SetValue(ActionEditor.contextMenuObject, fsmEvent);
			}
			SkillEditor.Builder.AddEvent(SkillEditor.SelectedFsm, fsmEvent);
			SkillEditor.SaveActions();
			SkillEditor.SetFsmDirty();
		}
		private static Skill GetTargetFSM(SkillGameObject go, SkillString fsmName)
		{
			if (go.get_UseVariable() || fsmName.get_UseVariable())
			{
				return null;
			}
			if (string.IsNullOrEmpty(fsmName.get_Value()))
			{
				using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Skill current = enumerator.get_Current();
						if (current.get_GameObject() == go.get_Value())
						{
							Skill result = current;
							return result;
						}
					}
					goto IL_BD;
				}
			}
			using (List<Skill>.Enumerator enumerator2 = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Skill current2 = enumerator2.get_Current();
					if (current2.get_GameObject() == go.get_Value() && fsmName.get_Value() == current2.get_Name())
					{
						Skill result = current2;
						return result;
					}
				}
			}
			IL_BD:
			return null;
		}
		private static void SaveEditingContext()
		{
			ActionEditor.contextVariable = ActionEditor.editingVariable;
			ActionEditor.contextMenuObject = ActionEditor.editingObject;
			ActionEditor.contextMenuField = ActionEditor.editingField;
			ActionEditor.contextMenuIndex = ActionEditor.editingIndex;
			ActionEditor.contextMenuFieldName = ActionEditor.editingFieldName;
			ActionEditor.contextVariableType = ActionEditor.editingVariableType;
			ActionEditor.contextFsmArray = ActionEditor.editingFsmArray;
			ActionEditor.contextType = ActionEditor.editingType;
			ActionEditor.contextTypeConstraint = ActionEditor.editingTypeConstraint;
			if (ActionEditor.contextFsmArray != null)
			{
				ActionEditor.contextType = ActionEditor.contextFsmArray.get_ObjectType();
			}
		}
	}
}
