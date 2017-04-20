using HutongGames.Editor;
using HutongGames.PlayMaker;
using HutongGames.PlayMakerEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
public static class StringEditor
{
	private static SkillString editingVariable;
	private static object editingObject;
	private static FieldInfo editingField;
	public static void AnimatorFloatPopup(GameObject go, SkillString variable, object obj = null, FieldInfo field = null)
	{
		StringEditor.DoAnimatorParameterPopup(go, 1, variable, obj, field);
	}
	public static void AnimatorIntPopup(GameObject go, SkillString variable, object obj = null, FieldInfo field = null)
	{
		StringEditor.DoAnimatorParameterPopup(go, 3, variable, obj, field);
	}
	public static void AnimatorBoolPopup(GameObject go, SkillString variable, object obj = null, FieldInfo field = null)
	{
		StringEditor.DoAnimatorParameterPopup(go, 4, variable, obj, field);
	}
	public static void AnimatorTriggerPopup(GameObject go, SkillString variable, object obj = null, FieldInfo field = null)
	{
		StringEditor.DoAnimatorParameterPopup(go, 9, variable, obj, field);
	}
	private static void DoAnimatorParameterPopup(GameObject go, AnimatorControllerParameterType parameterType, SkillString variable, object obj = null, FieldInfo field = null)
	{
		if (SkillEditorGUILayout.BrowseButton(go != null, string.Format(Strings.get_Tooltip_Browse_Animator_Parameters(), parameterType)))
		{
			StringEditor.editingVariable = variable;
			StringEditor.editingObject = obj;
			StringEditor.editingField = field;
			StringEditor.DoAnimatorParameterMenu(go, parameterType);
		}
	}
	private static void DoAnimatorParameterMenu(GameObject go, AnimatorControllerParameterType parameterType)
	{
		GenericMenu genericMenu = new GenericMenu();
		IEnumerable<string> animatorParameterNames = StringEditor.GetAnimatorParameterNames(go, parameterType);
		using (IEnumerator<string> enumerator = animatorParameterNames.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.get_Current();
				genericMenu.AddItem(new GUIContent(current), false, new GenericMenu.MenuFunction2(StringEditor.SetStringValue), current);
			}
		}
		if (genericMenu.GetItemCount() == 0)
		{
			genericMenu.AddDisabledItem(new GUIContent(string.Format(Strings.get_Menu_No_Animator_Parameters(), parameterType)));
		}
		genericMenu.ShowAsContext();
	}
	public static IEnumerable<string> GetAnimatorParameterNames(GameObject go, AnimatorControllerParameterType ofType)
	{
		List<string> list = new List<string>();
		Animator component = go.GetComponent<Animator>();
		if (component == null)
		{
			return list;
		}
		AnimatorControllerParameter[] parameters = component.get_parameters();
		for (int i = 0; i < parameters.Length; i++)
		{
			AnimatorControllerParameter animatorControllerParameter = parameters[i];
			if (animatorControllerParameter.get_type() == ofType)
			{
				list.Add(animatorControllerParameter.get_name());
			}
		}
		return list;
	}
	public static void AnimationNamePopup(GameObject go, SkillString variable, object obj = null, FieldInfo field = null)
	{
		if (SkillEditorGUILayout.BrowseButton(go != null, Strings.get_Tooltip_Browse_Animations_on_GameObject()))
		{
			StringEditor.editingVariable = variable;
			StringEditor.editingObject = obj;
			StringEditor.editingField = field;
			StringEditor.DoAnimationNameMenu(go);
		}
	}
	private static void DoAnimationNameMenu(GameObject go)
	{
		GenericMenu genericMenu = new GenericMenu();
		AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(go);
		AnimationClip[] array = animationClips;
		for (int i = 0; i < array.Length; i++)
		{
			AnimationClip animationClip = array[i];
			if (animationClip != null)
			{
				genericMenu.AddItem(new GUIContent(animationClip.get_name()), false, new GenericMenu.MenuFunction2(StringEditor.SetStringValue), animationClip.get_name());
			}
		}
		if (genericMenu.GetItemCount() == 0)
		{
			genericMenu.AddDisabledItem(new GUIContent(Strings.get_Label_No_Animations_On_Object()));
		}
		genericMenu.ShowAsContext();
	}
	public static void SortingLayerNamePopup(GUIContent label, SkillString variable, object obj = null, FieldInfo field = null)
	{
		SkillEditorGUILayout.PrefixLabel(label);
		if (GUILayout.Button(variable.get_Value(), EditorStyles.get_popup(), new GUILayoutOption[0]))
		{
			StringEditor.editingVariable = variable;
			StringEditor.editingObject = obj;
			StringEditor.editingField = field;
			StringEditor.DoSortingLayerMenu();
		}
	}
	public static void SortingLayerNameBrowseButton(SkillString variable, object obj = null, FieldInfo field = null)
	{
		if (SkillEditorGUILayout.BrowseButton(true, Strings.get_Label_Sorting_Layers()))
		{
			StringEditor.editingVariable = variable;
			StringEditor.editingObject = obj;
			StringEditor.editingField = field;
			StringEditor.DoSortingLayerMenu();
		}
	}
	private static void DoSortingLayerMenu()
	{
		GenericMenu genericMenu = new GenericMenu();
		string[] array = EditorHacks.UpdateSortingLayerNames();
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string text = array2[i];
			genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(StringEditor.SetStringValue), text);
		}
		genericMenu.ShowAsContext();
	}
	public static void LayerNamePopup(GUIContent label, SkillString variable, object obj = null, FieldInfo field = null)
	{
		SkillEditorGUILayout.PrefixLabel(label);
		if (GUILayout.Button(variable.get_Value(), EditorStyles.get_popup(), new GUILayoutOption[0]))
		{
			StringEditor.editingVariable = variable;
			StringEditor.editingObject = obj;
			StringEditor.editingField = field;
			StringEditor.DoLayerMenu();
		}
	}
	private static void DoLayerMenu()
	{
		GenericMenu genericMenu = new GenericMenu();
		for (int i = 0; i < 32; i++)
		{
			string text = LayerMask.LayerToName(i);
			if (!string.IsNullOrEmpty(text))
			{
				genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(StringEditor.SetStringValue), text);
			}
		}
		genericMenu.ShowAsContext();
	}
	public static void FsmNamePopup(GameObject go, SkillString variable, object obj = null, FieldInfo field = null)
	{
		if (SkillEditorGUILayout.BrowseButton(go != null, Strings.get_Tooltip_Browse_FSMs_on_GameObject()))
		{
			StringEditor.editingVariable = variable;
			StringEditor.editingObject = obj;
			StringEditor.editingField = field;
			StringEditor.DoFsmNameMenu(go);
		}
	}
	private static void DoFsmNameMenu(GameObject go)
	{
		GenericMenu genericMenu = new GenericMenu();
		using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Skill current = enumerator.get_Current();
				if (current.get_GameObject() == go)
				{
					genericMenu.AddItem(new GUIContent(current.get_Name()), false, new GenericMenu.MenuFunction2(StringEditor.SetStringValue), current.get_Name());
				}
			}
		}
		if (genericMenu.GetItemCount() == 0)
		{
			genericMenu.AddDisabledItem(new GUIContent(Strings.get_Label_None()));
		}
		genericMenu.ShowAsContext();
	}
	public static void VariablesPopup(GameObject go, string fsmName, UIHint hint, SkillString variable)
	{
		if (SkillEditorGUILayout.BrowseButton(go != null, Strings.get_Tooltip_Browse_variables_in_FSM()))
		{
			StringEditor.editingVariable = variable;
			StringEditor.DoVariablesMenu(go, fsmName, hint);
		}
	}
	private static void DoVariablesMenu(GameObject go, string fsmName, UIHint hint)
	{
		GenericMenu genericMenu = new GenericMenu();
		using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Skill current = enumerator.get_Current();
				if (current.get_GameObject() == go && current.get_Name() == fsmName)
				{
					NamedVariable[] names = current.get_Variables().GetNames(SkillVariable.GetVariableType(hint));
					NamedVariable[] array = names;
					for (int i = 0; i < array.Length; i++)
					{
						NamedVariable namedVariable = array[i];
						genericMenu.AddItem(new GUIContent(namedVariable.get_Name()), false, new GenericMenu.MenuFunction2(StringEditor.SetStringValue), namedVariable.get_Name());
					}
				}
			}
		}
		if (genericMenu.GetItemCount() == 0)
		{
			genericMenu.AddDisabledItem(new GUIContent(Strings.get_Label_None()));
		}
		genericMenu.ShowAsContext();
	}
	private static void SetStringValue(object userdata)
	{
		Keyboard.ResetFocus();
		if (StringEditor.editingVariable != null)
		{
			StringEditor.editingVariable.set_Value(userdata as string);
			return;
		}
		if (StringEditor.editingObject != null && StringEditor.editingField != null)
		{
			StringEditor.editingField.SetValue(StringEditor.editingObject, userdata as string);
		}
	}
}
