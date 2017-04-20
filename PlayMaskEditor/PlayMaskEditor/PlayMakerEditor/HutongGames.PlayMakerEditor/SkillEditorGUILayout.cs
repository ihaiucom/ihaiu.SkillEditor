using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class SkillEditorGUILayout
	{
		private static string password;
		public static void LabelWidth(float labelWidth)
		{
			EditorGUIUtility.set_labelWidth(labelWidth);
		}
		public static int TableRow(GUIContent[] columnText, float[] columnWidths, bool selected, bool hasError, params GUILayoutOption[] layoutOptions)
		{
			int result = -1;
			float fixedHeight = SkillEditorStyles.TableRow.get_fixedHeight();
			Rect rect = GUILayoutUtility.GetRect(0f, 0f, fixedHeight, fixedHeight, layoutOptions);
			if (selected)
			{
				GUI.Box(rect, "", SkillEditorStyles.TableRowSelected);
			}
			if (Event.get_current().get_type() == 7 && hasError)
			{
				Rect rect2 = new Rect(rect);
				rect2.set_width(14f);
				rect2.set_height(14f);
				Rect rect3 = rect2;
				rect3.set_x(rect3.get_x() + 2f);
				rect3.set_y(rect3.get_y() + 2f);
				GUI.DrawTexture(rect3, SkillEditorStyles.Errors);
			}
			rect.set_x(rect.get_x() + 18f);
			rect.set_width(rect.get_width() - 18f);
			int num = 0;
			float num2 = rect.get_x();
			for (int i = 0; i < columnText.Length; i++)
			{
				GUIContent gUIContent = columnText[i];
				Rect rect4 = new Rect(rect);
				rect4.set_x(num2);
				rect4.set_width(rect.get_width() * columnWidths[num]);
				Rect rect5 = rect4;
				if (GUI.Button(rect5, gUIContent, selected ? SkillEditorStyles.TableRowTextSelected : SkillEditorStyles.TableRowText))
				{
					result = num;
				}
				num2 += rect5.get_width();
				num++;
			}
			return result;
		}
		public static GenericMenu GenerateFsmGameObjectSelectionMenu(bool hasAddFsmOption)
		{
			SkillEditor.RebuildFsmList();
			List<GameObject> list = new List<GameObject>();
			List<Skill> sortedFsmList = SkillEditor.SortedFsmList;
			using (List<Skill>.Enumerator enumerator = sortedFsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					if (!list.Contains(current.get_GameObject()))
					{
						list.Add(current.get_GameObject());
					}
				}
			}
			GenericMenu genericMenu = new GenericMenu();
			List<string> list2 = new List<string>();
			using (List<GameObject>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					GameObject current2 = enumerator2.get_Current();
					if (!(current2 == null))
					{
						string text = Labels.GenerateUniqueLabel(list2, current2.get_name());
						if (FsmEditorSettings.AddPrefabLabel && PrefabUtility.GetPrefabType(current2) == 1)
						{
							text += Strings.get_Label_Prefab_postfix();
						}
						list2.Add(text);
						genericMenu.AddItem(new GUIContent(text), SkillEditor.Selection.ActiveFsmGameObject == current2, new GenericMenu.MenuFunction2(SkillEditorGUILayout.SelectGameObject), current2);
					}
				}
			}
			if (hasAddFsmOption)
			{
				genericMenu.AddSeparator(string.Empty);
				if (Selection.get_activeGameObject() != null)
				{
					string text2 = string.Format(Strings.get_Menu_Add_FSM_to__(), Selection.get_activeGameObject().get_name());
					genericMenu.AddItem(new GUIContent(text2), false, new GenericMenu.MenuFunction(SkillEditor.AddFsm));
				}
				else
				{
					genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Disabled_Add_FSM_to_selected_object()));
				}
			}
			return genericMenu;
		}
		private static void SelectGameObject(object userdata)
		{
			GameObject activeGameObject = (GameObject)userdata;
			bool lockGraphView = FsmEditorSettings.LockGraphView;
			FsmEditorSettings.LockGraphView = false;
			Selection.set_activeGameObject(activeGameObject);
			SkillEditor.Instance.OnSelectionChange();
			FsmEditorSettings.LockGraphView = lockGraphView;
		}
		public static GenericMenu GenerateGameObjectFsmSelectionMenu()
		{
			List<Skill> list = new List<Skill>();
			List<Skill> sortedFsmList = SkillEditor.SortedFsmList;
			using (List<Skill>.Enumerator enumerator = sortedFsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					if (current.get_GameObject() == SkillEditor.Selection.ActiveFsmGameObject)
					{
						list.Add(current);
					}
				}
			}
			GenericMenu genericMenu = new GenericMenu();
			if (list.get_Count() == 0)
			{
				genericMenu.AddDisabledItem(new GUIContent(Strings.get_Label_None()));
			}
			else
			{
				using (List<Skill>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Skill current2 = enumerator2.get_Current();
						string fsmLabel = Labels.GetFsmLabel(current2);
						genericMenu.AddItem(new GUIContent(fsmLabel), SkillEditor.SelectedFsm == current2, new GenericMenu.MenuFunction2(SkillEditor.SelectFsm), current2);
					}
				}
			}
			genericMenu.AddSeparator(string.Empty);
			if (Selection.get_activeGameObject() != null)
			{
				string text = string.Format(Strings.get_Menu_Add_FSM_to__(), Selection.get_activeGameObject().get_name());
				genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction(SkillEditor.AddFsm));
			}
			else
			{
				genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Disabled_Add_FSM_to_selected_object()));
			}
			return genericMenu;
		}
		public static GenericMenu GenerateFsmSelectionMenu(bool includeAssets, bool hasAddFsmOption)
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_None_FSM()), false, new GenericMenu.MenuFunction2(SkillEditor.SelectFsm), null);
			List<string> list = new List<string>();
			List<Skill> sortedFsmList = SkillEditor.SortedFsmList;
			using (List<Skill>.Enumerator enumerator = sortedFsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					if (current != null && (includeAssets || !SkillPrefabs.IsPersistent(current)))
					{
						string text = Labels.GenerateUniqueLabel(list, Labels.GetFullFsmLabel(current));
						list.Add(text);
						genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(SkillEditor.SelectFsm), current);
					}
				}
			}
			if (hasAddFsmOption)
			{
				genericMenu.AddSeparator(string.Empty);
				if (Selection.get_activeGameObject() != null)
				{
					string text2 = string.Format(Strings.get_Menu_Add_FSM_to__(), Selection.get_activeGameObject().get_name());
					genericMenu.AddItem(new GUIContent(text2), false, new GenericMenu.MenuFunction(SkillEditor.AddFsm));
				}
				else
				{
					genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Disabled_Add_FSM_to_selected_object()));
				}
			}
			return genericMenu;
		}
		public static GenericMenu GenerateEventSelectionMenu(Skill fsm, SkillEvent selectedEvent, GenericMenu.MenuFunction2 selectFunction, GenericMenu.MenuFunction newFunction)
		{
			GenericMenu genericMenu = new GenericMenu();
			bool flag = SkillEvent.IsNullOrEmpty(selectedEvent);
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_None_Event()), flag, selectFunction, null);
			SkillEvent[] events = fsm.get_Events();
			for (int i = 0; i < events.Length; i++)
			{
				SkillEvent fsmEvent = events[i];
				flag = (fsmEvent == selectedEvent);
				genericMenu.AddItem(new GUIContent(fsmEvent.get_Name()), flag, selectFunction, fsmEvent);
			}
			genericMenu.AddSeparator(string.Empty);
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_New_Event()), false, newFunction);
			return genericMenu;
		}
		public static GenericMenu GenerateStateSelectionMenu(Skill fsm, string selectedState, GenericMenu.MenuFunction2 selectFunction)
		{
			GenericMenu genericMenu = new GenericMenu();
			bool flag = string.IsNullOrEmpty(selectedState);
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_None_State()), flag, selectFunction, null);
			SkillState[] states = fsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				flag = (fsmState.get_Name() == selectedState);
				genericMenu.AddItem(new GUIContent(fsmState.get_Name()), flag, selectFunction, fsmState.get_Name());
			}
			return genericMenu;
		}
		public static GenericMenu GenerateStateSelectionMenu(Skill fsm)
		{
			GenericMenu genericMenu = new GenericMenu();
			SkillState[] states = fsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				genericMenu.AddItem(new GUIContent(fsmState.get_Name()), false, new GenericMenu.MenuFunction2(SkillEditor.SelectStateFromMenu), fsmState.get_Name());
			}
			return genericMenu;
		}
		public static SkillEvent EventPopup(GUIContent label, List<SkillEvent> eventList, SkillEvent selectedEvent)
		{
			GUIContent[] eventNamesFromList = Events.GetEventNamesFromList(eventList);
			string text = (selectedEvent == null) ? null : selectedEvent.get_Name();
			int num = -1;
			int num2 = 0;
			GUIContent[] array = eventNamesFromList;
			for (int i = 0; i < array.Length; i++)
			{
				GUIContent gUIContent = array[i];
				if (gUIContent.get_text() == text)
				{
					num = num2;
					break;
				}
				num2++;
			}
			int num3 = EditorGUILayout.Popup(label, num, eventNamesFromList, new GUILayoutOption[0]);
			if (num3 > 0)
			{
				string text2 = eventNamesFromList[num3].get_text();
				using (List<SkillEvent>.Enumerator enumerator = eventList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SkillEvent current = enumerator.get_Current();
						if (current.get_Name() == text2)
						{
							return current;
						}
					}
				}
				return null;
			}
			return null;
		}
		public static string MethodNamePopup(GameObject gameobject, MonoBehaviour behaviour, int maxParameters, bool coroutinesOnly)
		{
			int indentLevel = EditorGUI.get_indentLevel();
			EditorGUI.set_indentLevel(0);
			List<string> list = new List<string>();
			list.Add(Strings.get_Label_None());
			List<string> list2 = list;
			if (gameobject != null)
			{
				if (behaviour == null)
				{
					MonoBehaviour[] components = gameobject.GetComponents<MonoBehaviour>();
					for (int i = 0; i < components.Length; i++)
					{
						MonoBehaviour behaviour2 = components[i];
						list2.AddRange(SkillEditorGUILayout.GetMethodNames(behaviour2, maxParameters, coroutinesOnly));
					}
				}
				else
				{
					list2.AddRange(SkillEditorGUILayout.GetMethodNames(behaviour, maxParameters, coroutinesOnly));
				}
			}
			int num = EditorGUILayout.Popup(-1, list2.ToArray(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(20f)
			});
			EditorGUI.set_indentLevel(indentLevel);
			if (num <= 0)
			{
				return "";
			}
			return list2.get_Item(num);
		}
		private static IEnumerable<string> GetMethodNames(MonoBehaviour behaviour, int maxParameters, bool coroutinesOnly)
		{
			List<string> list = new List<string>();
			list.Add(Strings.get_Label_None());
			List<string> list2 = list;
			if (behaviour != null)
			{
				Type type = behaviour.GetType();
				while (type != typeof(MonoBehaviour) && type != null)
				{
					MethodInfo[] methods = type.GetMethods(54);
					for (int i = 0; i < methods.Length; i++)
					{
						MethodInfo methodInfo = methods[i];
						if (!coroutinesOnly || methodInfo.get_ReturnType() == typeof(IEnumerator))
						{
							string name = methodInfo.get_Name();
							string text;
							if ((text = name) == null || (!(text == "Main") && !(text == "Start") && !(text == "Awake")))
							{
								ParameterInfo[] parameters = methodInfo.GetParameters();
								if (parameters.Length <= maxParameters)
								{
									if (parameters.Length > 0)
									{
										Type parameterType = parameters[0].get_ParameterType();
										if (TypeHelpers.IsSupportedParameterType(parameterType))
										{
											list2.Add(name);
										}
									}
									else
									{
										list2.Add(name);
									}
								}
							}
						}
					}
					type = type.get_BaseType();
				}
			}
			return list2;
		}
		[Localizable(false)]
		public static string ParameterTypePopup(string label, string selectedTypeName)
		{
			string[] array = new string[]
			{
				"None",
				"bool",
				"int",
				"float",
				"string",
				"Vector2",
				"Vector3",
				"Rect",
				"GameObject",
				"Material",
				"Texture",
				"Color",
				"Quaternion",
				"Object",
				"Enum",
				"Array"
			};
			int num = 0;
			int num2 = 0;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (text == selectedTypeName)
				{
					num2 = num;
				}
				num++;
			}
			num2 = EditorGUILayout.Popup(label, num2, array, new GUILayoutOption[0]);
			if (num2 == -1)
			{
				return Strings.get_Label_None();
			}
			return array[num2];
		}
		public static string ComponentNamePopup(GameObject gameObject, Type componentType)
		{
			int indentLevel = EditorGUI.get_indentLevel();
			EditorGUI.set_indentLevel(0);
			Component[] array = new Component[0];
			if (gameObject != null)
			{
				array = gameObject.GetComponents(componentType);
			}
			List<string> list = new List<string>();
			list.Add(Strings.get_Label_None());
			List<string> list2 = list;
			Component[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Component component = array2[i];
				if (component != null)
				{
					list2.Add(Labels.StripUnityEngineNamespace(component.GetType().ToString()));
				}
			}
			int num = EditorGUILayout.Popup(-1, list2.ToArray(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(20f)
			});
			EditorGUI.set_indentLevel(indentLevel);
			if (num <= 0)
			{
				return "";
			}
			return list2.get_Item(num);
		}
		[Obsolete("Removed. Use StringEditor.FsmNamePopup instead.")]
		public static string FsmNamePopup(GameObject gameObject)
		{
			List<string> list = new List<string>();
			list.Add(Strings.get_Label_None());
			List<string> list2 = list;
			using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					if (current.get_GameObject() == gameObject)
					{
						list2.Add(current.get_Name());
					}
				}
			}
			int num = EditorGUILayout.Popup(-1, list2.ToArray(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(20f)
			});
			if (num <= 0)
			{
				return "";
			}
			return list2.get_Item(num);
		}
		public static string FsmEventListPopup()
		{
			int indentLevel = EditorGUI.get_indentLevel();
			EditorGUI.set_indentLevel(0);
			List<string> list = new List<string>();
			using (List<SkillEvent>.Enumerator enumerator = SkillEvent.get_EventList().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEvent current = enumerator.get_Current();
					list.Add(current.get_Name());
				}
			}
			int num = EditorGUILayout.Popup(-1, list.ToArray(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(20f)
			});
			EditorGUI.set_indentLevel(indentLevel);
			if (num <= 0)
			{
				return "";
			}
			return list.get_Item(num);
		}
		public static string FsmEventPopup(GameObject gameObject, string fsmName)
		{
			int indentLevel = EditorGUI.get_indentLevel();
			EditorGUI.set_indentLevel(0);
			List<string> list = new List<string>();
			list.Add(Strings.get_Label_None());
			List<string> list2 = list;
			using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					if (current.get_GameObject() == gameObject && current.get_Name() == fsmName)
					{
						SkillEvent[] events = current.get_Events();
						for (int i = 0; i < events.Length; i++)
						{
							SkillEvent fsmEvent = events[i];
							list2.Add(fsmEvent.get_Name());
						}
					}
				}
			}
			int num = EditorGUILayout.Popup(-1, list2.ToArray(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(20f)
			});
			EditorGUI.set_indentLevel(indentLevel);
			if (num <= 0)
			{
				return "";
			}
			return list2.get_Item(num);
		}
		[Obsolete("Removed. Use StringEditor.VariablesPopup instead.")]
		public static string FsmVariablePopup(GameObject gameObject, string fsmName, UIHint hint)
		{
			return string.Empty;
		}
		[Obsolete("Removed. Use StringEditor.AnimationNamePopup instead.")]
		public static string AnimationNamePopup(GameObject gameObject)
		{
			return string.Empty;
		}
		public static string ScriptListPopup()
		{
			int indentLevel = EditorGUI.get_indentLevel();
			EditorGUI.set_indentLevel(0);
			int popupIndex = EditorGUILayout.Popup(-1, Files.ScriptPopupNames, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(20f)
			});
			string scriptName = Files.GetScriptName(popupIndex);
			EditorGUI.set_indentLevel(indentLevel);
			return scriptName;
		}
		[Obsolete("Use ScriptListPopup with TextField instead.")]
		public static string ScriptListPopup(GUIContent label, string selectedComponentName)
		{
			int popupIndex = EditorGUILayout.Popup(label.get_text(), -1, Files.ScriptPopupNames, new GUILayoutOption[0]);
			return Files.GetScriptName(popupIndex);
		}
		public static AnimationState AnimationStatePopup(GUIContent label, AnimationState selectedAnimationState, GameObject gameObject)
		{
			List<AnimationState> list = new List<AnimationState>();
			string text = Strings.get_Label_None();
			if (gameObject != null)
			{
				Animation component = gameObject.GetComponent<Animation>();
				if (component != null)
				{
					IEnumerator enumerator = component.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							AnimationState animationState = (AnimationState)enumerator.get_Current();
							list.Add(animationState);
						}
						goto IL_71;
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
				text = Strings.get_Error_Missing_Animation_Component();
			}
			else
			{
				text = Strings.get_Label_Select_GameObject();
			}
			IL_71:
			GUIContent[] array = new GUIContent[list.get_Count() + 1];
			array[0] = new GUIContent(text);
			int num = 0;
			for (int i = 0; i < list.get_Count(); i++)
			{
				array[i + 1] = new GUIContent(list.get_Item(i).get_name());
				if (selectedAnimationState == list.get_Item(i))
				{
					num = i + 1;
				}
			}
			num = EditorGUILayout.Popup(label, num, array, new GUILayoutOption[0]);
			if (num <= 0)
			{
				return null;
			}
			return list.get_Item(num - 1);
		}
		public static Behaviour BehaviorPopup(GUIContent label, Behaviour selectedBehavior, GameObject gameObject)
		{
			Behaviour[] array = new Behaviour[0];
			string text = Strings.get_Label_None();
			if (gameObject != null)
			{
				array = gameObject.GetComponents<Behaviour>();
			}
			else
			{
				text = Strings.get_Label_Select_GameObject();
			}
			GUIContent[] array2 = new GUIContent[array.Length + 1];
			array2[0] = new GUIContent(text);
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				array2[i + 1] = new GUIContent(ObjectNames.GetInspectorTitle(array[i]));
				if (selectedBehavior == array[i])
				{
					num = i + 1;
				}
			}
			num = EditorGUILayout.Popup(label, num, array2, new GUILayoutOption[0]);
			if (num <= 0)
			{
				return null;
			}
			return array[num - 1];
		}
		public static MonoBehaviour BehaviorPopup(GUIContent label, MonoBehaviour selectedBehavior, GameObject gameObject)
		{
			MonoBehaviour[] array = new MonoBehaviour[0];
			string text = Strings.get_Label_None();
			if (gameObject != null)
			{
				array = gameObject.GetComponents<MonoBehaviour>();
			}
			else
			{
				text = Strings.get_Label_Select_GameObject();
			}
			GUIContent[] array2 = new GUIContent[array.Length + 1];
			array2[0] = new GUIContent(text);
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				array2[i + 1] = new GUIContent(ObjectNames.GetInspectorTitle(array[i]));
				if (selectedBehavior == array[i])
				{
					num = i + 1;
				}
			}
			num = EditorGUILayout.Popup(label, num, array2, new GUILayoutOption[0]);
			if (num <= 0)
			{
				return null;
			}
			return array[num - 1];
		}
		public static bool BrowseButton(bool enabled, string tooltip)
		{
			EditorGUI.BeginDisabledGroup(!enabled);
			GUI.set_backgroundColor(Color.get_white());
			SkillEditorContent.PopupButton.set_tooltip((DragAndDropManager.mode == DragAndDropManager.DragMode.None) ? tooltip : null);
			bool result = GUILayout.Button(SkillEditorContent.PopupButton, SkillEditorStyles.MiniButtonPadded, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			return result;
		}
		public static void PrefixLabel(GUIContent label)
		{
			GUIStyle followingStyle = "Button";
			SkillEditorGUILayout.PrefixLabel(label, followingStyle);
		}
		public static void PrefixLabel(string text)
		{
			SkillEditorGUILayout.PrefixLabel(SkillEditorContent.TempContent(text, ""));
		}
		public static void PrefixLabel(GUIContent label, GUIStyle followingStyle)
		{
			float num = (float)followingStyle.get_margin().get_left();
			Rect rect = GUILayoutUtility.GetRect(150f - num, 16f, followingStyle, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			if (Event.get_current().get_type() == 7)
			{
				float num2 = (float)EditorGUI.get_indentLevel() * 9f;
				rect.set_x(rect.get_x() + num2);
				rect.set_width(rect.get_width() + (num - num2));
				GUI.Label(rect, label);
			}
		}
		public static void DisabledLabel(string text)
		{
			bool enabled = GUI.get_enabled();
			GUI.set_enabled(false);
			GUILayout.Label(text, new GUILayoutOption[0]);
			GUI.set_enabled(enabled);
		}
		public static void ResetGUIColors()
		{
			GUI.set_backgroundColor(Color.get_white());
			GUI.set_contentColor(Color.get_white());
			GUI.set_color(Color.get_white());
		}
		public static void ResetGUIColorsKeepAlpha()
		{
			GUI.set_backgroundColor(Color.get_white());
			GUI.set_contentColor(new Color(1f, 1f, 1f, GUI.get_contentColor().a));
			GUI.set_color(new Color(1f, 1f, 1f, GUI.get_color().a));
		}
		public static string TextAreaWithHint(string text, string hint, params GUILayoutOption[] layoutOptions)
		{
			bool changed = GUI.get_changed();
			Color contentColor = GUI.get_contentColor();
			string text2 = text;
			if (string.IsNullOrEmpty(text2))
			{
				text2 = hint;
				Color textColor = EditorStyles.get_label().get_normal().get_textColor();
				textColor.a = 0.5f;
				GUI.set_contentColor(textColor);
			}
			GUI.set_changed(false);
			text2 = EditorGUILayout.TextArea(text2, SkillEditorStyles.TextAreaWithWordWrap, layoutOptions);
			GUI.set_contentColor(contentColor);
			if (GUI.get_changed())
			{
				return text2;
			}
			GUI.set_changed(changed);
			return text;
		}
		public static string TextFieldWithHint(string text, string hint, params GUILayoutOption[] layoutOptions)
		{
			bool changed = GUI.get_changed();
			Color contentColor = GUI.get_contentColor();
			string text2 = text;
			if (string.IsNullOrEmpty(text2))
			{
				text2 = hint;
				Color textColor = EditorStyles.get_label().get_normal().get_textColor();
				textColor.a = 0.5f;
				GUI.set_contentColor(textColor);
			}
			GUI.set_changed(false);
			text2 = EditorGUILayout.TextField(text2, layoutOptions);
			GUI.set_contentColor(contentColor);
			if (GUI.get_changed())
			{
				return text2;
			}
			GUI.set_changed(changed);
			return text;
		}
		public static bool TextFieldButtonWithHint(string text, string hint, params GUILayoutOption[] layoutOptions)
		{
			Color contentColor = GUI.get_contentColor();
			if (string.IsNullOrEmpty(text))
			{
				text = hint;
				Color textColor = EditorStyles.get_label().get_normal().get_textColor();
				textColor.a = 0.5f;
				GUI.set_contentColor(textColor);
			}
			bool result = GUILayout.Button(text, EditorStyles.get_textField(), layoutOptions);
			GUI.set_contentColor(contentColor);
			return result;
		}
		public static bool BoldFoldout(bool state, GUIContent content)
		{
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			return EditorGUILayout.Foldout(state, content, SkillEditorStyles.BoldFoldout);
		}
		public static bool ActionFoldout(bool isOpen)
		{
			return GUILayout.Toggle(isOpen, "", SkillEditorStyles.ActionFoldout, new GUILayoutOption[0]);
		}
		public static bool DeleteButton()
		{
			SkillEditorContent.DeleteButton.set_tooltip((DragAndDropManager.mode == DragAndDropManager.DragMode.None) ? Strings.get_Command_Delete() : null);
			return SkillEditorGUILayout.MiniButtonPadded(SkillEditorContent.DeleteButton, new GUILayoutOption[0]);
		}
		public static bool ToolbarDeleteButton()
		{
			SkillEditorContent.DeleteButton.set_tooltip((DragAndDropManager.mode == DragAndDropManager.DragMode.None) ? Strings.get_Command_Delete() : null);
			return GUILayout.Button(SkillEditorContent.DeleteButton, EditorStyles.get_toolbarButton(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(24f)
			});
		}
		public static bool ResetButton()
		{
			SkillEditorContent.ResetButton.set_tooltip((DragAndDropManager.mode == DragAndDropManager.DragMode.None) ? Strings.get_Command_Reset() : null);
			return SkillEditorGUILayout.MiniButton(SkillEditorContent.ResetButton, new GUILayoutOption[0]);
		}
		public static bool HelpButtonSmall(string tooltip)
		{
			SkillEditorContent.HelpButton.set_tooltip((DragAndDropManager.mode == DragAndDropManager.DragMode.None) ? tooltip : null);
			return GUILayout.Button(SkillEditorContent.HelpButton, GUIStyle.get_none(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(18f)
			});
		}
		public static bool HelpButton(string tooltip = "Online Help")
		{
			SkillEditorContent.HelpButton.set_tooltip((DragAndDropManager.mode == DragAndDropManager.DragMode.None) ? tooltip : null);
			return GUILayout.Button(SkillEditorContent.HelpButton, "Label", new GUILayoutOption[]
			{
				GUILayout.MaxWidth(18f)
			});
		}
		public static bool SettingsButton()
		{
			SkillEditorContent.SettingsButton.set_tooltip((DragAndDropManager.mode == DragAndDropManager.DragMode.None) ? Strings.get_SettingsButton_Tooltip() : null);
			return GUILayout.Button(SkillEditorContent.SettingsButton, GUIStyle.get_none(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(20f)
			});
		}
		public static bool SettingsButtonPadded()
		{
			SkillEditorContent.SettingsButton.set_tooltip((DragAndDropManager.mode == DragAndDropManager.DragMode.None) ? Strings.get_SettingsButton_Tooltip() : null);
			return GUILayout.Button(SkillEditorContent.SettingsButton, "Label", new GUILayoutOption[]
			{
				GUILayout.MaxWidth(18f)
			});
		}
		public static bool ToolbarSettingsButton()
		{
			return GUILayout.Button(SkillEditorContent.SettingsButton, EditorStyles.get_label(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(24f)
			});
		}
		public static GUIContent GetTooltipLabelContent(string labelText, string tooltip)
		{
			if (DragAndDropManager.mode == DragAndDropManager.DragMode.None)
			{
				return new GUIContent(labelText, tooltip);
			}
			return new GUIContent(labelText);
		}
		public static bool MiniButton(GUIContent content, params GUILayoutOption[] layoutOptions)
		{
			return GUILayout.Button(content, SkillEditorStyles.MiniButton, layoutOptions);
		}
		public static bool MiniButtonPadded(GUIContent content, params GUILayoutOption[] layoutOptions)
		{
			return GUILayout.Button(content, SkillEditorStyles.MiniButtonPadded, layoutOptions);
		}
		public static void ReadonlyTextField(string value, params GUILayoutOption[] layoutOptions)
		{
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.TextField(value, layoutOptions);
			EditorGUI.EndDisabledGroup();
		}
		public static void ReadonlyTextField(GUIContent label, float labelWidth, string value, params GUILayoutOption[] layoutOptions)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.LabelField(label, new GUILayoutOption[]
			{
				GUILayout.Width(labelWidth)
			});
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.TextField(value, layoutOptions);
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}
		public static float FloatSlider(GUIContent label, float value, float minSliderValue, float maxSliderValue)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (label != null && label != GUIContent.none)
			{
				GUILayout.Label(label, new GUILayoutOption[]
				{
					GUILayout.MinWidth(50f),
					GUILayout.MaxWidth(146f)
				});
			}
			value = EditorGUILayout.FloatField(value, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(75f)
			});
			float num = value;
			bool changed = GUI.get_changed();
			GUI.set_changed(false);
			num = GUILayout.HorizontalSlider(num, minSliderValue, maxSliderValue, new GUILayoutOption[0]);
			if (GUI.get_changed())
			{
				value = num;
			}
			else
			{
				GUI.set_changed(changed);
			}
			EditorGUILayout.EndHorizontal();
			return value;
		}
		public static Quaternion QuaternionField(string label, Quaternion quaternion)
		{
			bool changed = GUI.get_changed();
			EditorGUI.BeginChangeCheck();
			Vector3 vector = EditorGUILayout.Vector3Field(label, quaternion.get_eulerAngles(), new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				GUI.set_changed(true);
				return Quaternion.Euler(vector);
			}
			GUI.set_changed(changed);
			return quaternion;
		}
		public static bool RightAlignedToggle(GUIContent content, bool value)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(content, SkillEditorStyles.LabelWithWordWrap, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(320f)
			});
			GUILayout.FlexibleSpace();
			bool result = EditorGUILayout.Toggle(value, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(15f)
			});
			EditorGUILayout.EndHorizontal();
			return result;
		}
		public static void Divider(params GUILayoutOption[] layoutOptions)
		{
			GUILayout.Box(GUIContent.none, SkillEditorStyles.Divider, layoutOptions);
		}
		public static void LightDivider(params GUILayoutOption[] layoutOptions)
		{
			Color color = GUI.get_color();
			GUI.set_color(new Color(1f, 1f, 1f, 0.25f));
			GUILayout.Box(GUIContent.none, SkillEditorStyles.Divider, layoutOptions);
			GUI.set_color(color);
		}
		public static void SequenceDivider(params GUILayoutOption[] layoutOptions)
		{
			GUILayout.Box(GUIContent.none, SkillEditorStyles.DividerSequence, layoutOptions);
		}
		public static void UnlockFsmGUI(Skill fsm)
		{
			if (fsm != null && fsm.get_Locked())
			{
				EditorGUILayout.HelpBox(Strings.get_Label_This_FSM_is_Locked(), 1);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				SkillEditorGUILayout.password = EditorGUILayout.TextField(SkillEditorGUILayout.password, new GUILayoutOption[0]);
				if (GUILayout.Button(Strings.get_Label_Unlock(), new GUILayoutOption[]
				{
					GUILayout.MaxWidth(60f)
				}))
				{
					SkillEditorGUILayout.UnlockFSM(fsm);
				}
				GUILayout.EndHorizontal();
			}
		}
		public static void LockFsmGUI(Skill fsm)
		{
			if (fsm.get_Locked())
			{
				return;
			}
			SkillEditorGUILayout.Divider(new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(Strings.get_Label_Password(), new GUILayoutOption[]
			{
				GUILayout.MaxWidth(70f)
			});
			SkillEditorGUILayout.password = EditorGUILayout.TextField(SkillEditorGUILayout.password, new GUILayoutOption[0]);
			if (GUILayout.Button(SkillEditorContent.LockFsmButton, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(60f)
			}))
			{
				SkillEditorGUILayout.LockFSM(fsm);
			}
			GUILayout.EndHorizontal();
		}
		public static void UnlockFSM(Skill fsm)
		{
			fsm.Unlock(SkillEditorGUILayout.Md5Sum(SkillEditorGUILayout.password));
			if (fsm.get_Locked())
			{
				Dialogs.OkDialog(Strings.get_ProductName(), Strings.get_Dialog_UnlockFSM_Wrong_Password());
				return;
			}
			SkillEditor.SetFsmDirty(fsm, false, false, true);
		}
		public static void LockFSM(Skill fsm)
		{
			fsm.Lock(SkillEditorGUILayout.Md5Sum(SkillEditorGUILayout.password));
			SkillEditorGUILayout.password = string.Empty;
			SkillEditor.SetFsmDirty(fsm, false, false, true);
		}
		[Localizable(false)]
		private static string Md5Sum(string strToEncrypt)
		{
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			byte[] bytes = uTF8Encoding.GetBytes(strToEncrypt + "pudding");
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text += Convert.ToString(array[i], 16).PadLeft(2, '0');
			}
			return text.PadLeft(32, '0');
		}
		public static void ToolWindowLargeTitle(EditorWindow window, string title)
		{
			GUI.Box(new Rect(0f, 0f, window.get_position().get_width(), 42f), title, SkillEditorStyles.LargeTitleWithLogo);
			GUILayoutUtility.GetRect(window.get_position().get_width(), 42f);
		}
		public static void PlaymakerHeader(EditorWindow window)
		{
			GUI.Box(new Rect(0f, 0f, window.get_position().get_width(), 60f), "", SkillEditorStyles.PlaymakerHeader);
			GUILayoutUtility.GetRect(window.get_position().get_width(), 60f);
		}
		public static bool ToolWindowsCommonGUI(EditorWindow window)
		{
			if (SkillEditor.Instance == null)
			{
				window.Close();
				return false;
			}
			if (SkillEditorGUILayout.DoToolWindowsDisabledGUI())
			{
				return false;
			}
			if (EditorApplication.get_isCompiling())
			{
				GUI.set_enabled(false);
			}
			SkillEditorStyles.Init();
			if (Event.get_current().get_type() == 4 && Event.get_current().get_keyCode() == 282)
			{
				EditorCommands.ToggleShowHints();
				return false;
			}
			EditorGUI.set_indentLevel(0);
			return true;
		}
		private static bool DoToolWindowsDisabledGUI()
		{
			if (SkillEditorGUILayout.DoEditorDisabledGUI())
			{
				return true;
			}
			if (EditorApplication.get_isPlaying() && FsmEditorSettings.DisableToolWindowsWhenPlaying)
			{
				GUILayout.Label(Strings.get_Label_Tool_Windows_disabled_when_playing(), new GUILayoutOption[0]);
				FsmEditorSettings.DisableToolWindowsWhenPlaying = !GUILayout.Toggle(!FsmEditorSettings.DisableToolWindowsWhenPlaying, Strings.get_Label_Enable_Tool_Windows_When_Playing(), new GUILayoutOption[0]);
				if (GUI.get_changed())
				{
					FsmEditorSettings.SaveSettings();
				}
				return FsmEditorSettings.DisableToolWindowsWhenPlaying;
			}
			return false;
		}
		public static bool DoEditorDisabledGUI()
		{
			if (EditorApplication.get_isPlaying() && FsmEditorSettings.DisableEditorWhenPlaying)
			{
				GUILayout.Label(Strings.get_Label_Editor_disabled_when_playing(), new GUILayoutOption[0]);
				FsmEditorSettings.DisableEditorWhenPlaying = !GUILayout.Toggle(!FsmEditorSettings.DisableEditorWhenPlaying, Strings.get_Label_Enable_Editor_When_Playing(), new GUILayoutOption[0]);
				if (GUI.get_changed())
				{
					FsmEditorSettings.SaveSettings();
				}
				return FsmEditorSettings.DisableEditorWhenPlaying;
			}
			return false;
		}
		public static void PreviewVersionHelpBox()
		{
			EditorGUILayout.HelpBox(Strings.get_Label_PREVIEW_VERSION(), 0);
		}
		public static void PreviewVersionContextMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Preview_Version()));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Buy_Playmaker()), false, new GenericMenu.MenuFunction(EditorCommands.OpenOnlineStore));
			genericMenu.ShowAsContext();
		}
	}
}
