using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public static class ActionUtility
	{
		public class ActionCreationParams
		{
			public Skill fsm;
			public SkillState state;
			public Type actionType;
			public ActionTarget actionTarget;
			public Object parameter;
			public SkillStateAction beforeAction;
			public Vector2 position;
			public ActionCreationParams()
			{
			}
			public ActionCreationParams(ActionUtility.ActionCreationParams source)
			{
				this.fsm = source.fsm;
				this.state = source.state;
				this.actionType = source.actionType;
				this.actionTarget = source.actionTarget;
				this.parameter = source.parameter;
				this.beforeAction = source.beforeAction;
				this.position = source.position;
			}
		}
		public static void ShowObjectContextMenu(Skill fsm, SkillState state, Object obj, SkillStateAction beforeAction = null)
		{
			if (obj == null)
			{
				return;
			}
			Actions.BuildListIfNeeded();
			ActionUtility.ActionCreationParams actionCreationParams = new ActionUtility.ActionCreationParams
			{
				state = state,
				parameter = obj,
				beforeAction = beforeAction
			};
			GenericMenu genericMenu = new GenericMenu();
			GameObject gameObject = obj as GameObject;
			if (gameObject != null)
			{
				if (EditorUtility.IsPersistent(gameObject))
				{
					ActionUtility.AddMenuTitle(ref genericMenu, Strings.get_Menu_Prefab_Actions());
					ActionUtility.AddPrefabMenuItems(ref genericMenu, actionCreationParams);
				}
				else
				{
					ActionUtility.AddMenuTitle(ref genericMenu, Strings.get_Menu_GameObject_Actions());
					ActionUtility.AddObjectMenuItems(ref genericMenu, actionCreationParams, true);
					Component[] components = gameObject.GetComponents<Component>();
					Component[] array = components;
					for (int i = 0; i < array.Length; i++)
					{
						Component parameter = array[i];
						actionCreationParams.parameter = parameter;
						ActionUtility.AddObjectMenuItems(ref genericMenu, actionCreationParams, true);
					}
				}
			}
			else
			{
				ActionUtility.AddMenuTitle(ref genericMenu, string.Format(Strings.get_Menu_Type_Actions(), obj.GetType().get_Name()));
				ActionUtility.AddObjectMenuItems(ref genericMenu, actionCreationParams, false);
			}
			if (genericMenu.GetItemCount() == 2)
			{
				genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_No_Context_Actions_Found()));
			}
			genericMenu.ShowAsContext();
		}
		private static void AddMenuTitle(ref GenericMenu menu, string title)
		{
			menu.AddDisabledItem(new GUIContent(title));
			menu.AddSeparator("");
		}
		private static void AddPrefabMenuItems(ref GenericMenu menu, ActionUtility.ActionCreationParams actionParams)
		{
			Type type = actionParams.parameter.GetType();
			List<Type> actionsSortedByCategory = ActionTargets.GetActionsSortedByCategory();
			using (List<Type>.Enumerator enumerator = actionsSortedByCategory.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					ActionUtility.ActionCreationParams actionCreationParams = new ActionUtility.ActionCreationParams(actionParams);
					List<ActionTarget> actionTargets = ActionTargets.GetActionTargets(current);
					using (List<ActionTarget>.Enumerator enumerator2 = actionTargets.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							ActionTarget current2 = enumerator2.get_Current();
							if (current2.get_AllowPrefabs() && current2.get_ObjectType().IsAssignableFrom(type))
							{
								actionCreationParams.actionType = current;
								actionCreationParams.actionTarget = current2;
								menu.AddItem(new GUIContent(Labels.GetActionLabel(current)), false, new GenericMenu.MenuFunction2(ActionUtility.AddAction), actionCreationParams);
							}
						}
					}
				}
			}
		}
		private static void AddObjectMenuItems(ref GenericMenu menu, ActionUtility.ActionCreationParams actionParams, bool isSubMenu = false)
		{
			Type type = actionParams.parameter.GetType();
			string text = isSubMenu ? (Labels.StripNamespace(type.get_FullName()) + '/') : "";
			List<Type> actionsSortedByCategory = ActionTargets.GetActionsSortedByCategory();
			using (List<Type>.Enumerator enumerator = actionsSortedByCategory.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					ActionUtility.ActionCreationParams actionCreationParams = new ActionUtility.ActionCreationParams(actionParams);
					List<ActionTarget> actionTargets = ActionTargets.GetActionTargets(current);
					using (List<ActionTarget>.Enumerator enumerator2 = actionTargets.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							ActionTarget current2 = enumerator2.get_Current();
							if (ActionUtility.PinToTopOfMenu(current) && current2.get_ObjectType().IsAssignableFrom(type))
							{
								actionCreationParams.actionType = current;
								actionCreationParams.actionTarget = current2;
								menu.AddItem(new GUIContent(text + Labels.GetActionLabel(current)), false, new GenericMenu.MenuFunction2(ActionUtility.AddAction), actionCreationParams);
							}
						}
					}
				}
			}
			if (menu.GetItemCount() > 2)
			{
				menu.AddSeparator(text);
			}
			using (List<Type>.Enumerator enumerator3 = actionsSortedByCategory.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Type current3 = enumerator3.get_Current();
					ActionUtility.ActionCreationParams actionCreationParams2 = new ActionUtility.ActionCreationParams(actionParams);
					List<ActionTarget> actionTargets2 = ActionTargets.GetActionTargets(current3);
					using (List<ActionTarget>.Enumerator enumerator4 = actionTargets2.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							ActionTarget current4 = enumerator4.get_Current();
							if (!ActionUtility.PinToTopOfMenu(current3) && current4.get_ObjectType().IsAssignableFrom(type))
							{
								actionCreationParams2.actionType = current3;
								actionCreationParams2.actionTarget = current4;
								string actionLabel = Labels.GetActionLabel(current3);
								string text2 = Actions.GetActionCategory(current3) + '/';
								menu.AddItem(new GUIContent(text + text2 + actionLabel), false, new GenericMenu.MenuFunction2(ActionUtility.AddAction), actionCreationParams2);
							}
						}
					}
				}
			}
		}
		[Localizable(false)]
		private static bool PinToTopOfMenu(Type actionType)
		{
			string fullName = actionType.get_FullName();
			return fullName == "HutongGames.PlayMaker.Actions.GetProperty" || fullName == "HutongGames.PlayMaker.Actions.SetProperty";
		}
		private static void AddAction(object userdata)
		{
			ActionUtility.ActionCreationParams actionCreationParams = (ActionUtility.ActionCreationParams)userdata;
			SkillStateAction action = ActionUtility.AddAction(actionCreationParams.state, actionCreationParams.actionType, actionCreationParams.actionTarget, actionCreationParams.parameter, actionCreationParams.beforeAction);
			SkillEditor.SelectAction(action, true);
		}
		public static SkillStateAction AddAction(SkillState state, Type actionType, ActionTarget actionTarget, Object targetObject = null, SkillStateAction beforeAction = null)
		{
			SkillStateAction fsmStateAction = SkillEditor.Builder.InsertAction(state, actionType, beforeAction);
			if (!string.IsNullOrEmpty(actionTarget.get_FieldName()))
			{
				string[] array = actionTarget.get_FieldName().Split(new char[]
				{
					','
				});
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i];
					ActionUtility.SetActionFieldValue(fsmStateAction, text.Trim(), targetObject);
				}
			}
			fsmStateAction.OnActionTargetInvoked(targetObject);
			SkillEditor.SaveActions(state, true);
			return fsmStateAction;
		}
		public static SkillStateAction AddAction(SkillState state, string actionTypeName, SkillStateAction beforeAction = null)
		{
			Type actionType = ActionData.GetActionType(actionTypeName);
			if (actionType == null)
			{
				Debug.LogError(string.Format(Strings.get_ActionUtility_AddAction_Missing_Action(), actionTypeName));
				return null;
			}
			return SkillEditor.Builder.InsertAction(state, actionType, beforeAction);
		}
		public static void SetOwnerDefault(Skill fsm, SkillStateAction action, Component component, string fieldName = "gameObject")
		{
			if (component == null)
			{
				return;
			}
			ActionUtility.SetOwnerDefault(fsm, action, component.get_gameObject(), fieldName);
		}
		public static void SetOwnerDefault(Skill fsm, SkillStateAction action, GameObject targetGO, string fieldName = "gameObject")
		{
			FieldInfo field = action.GetType().GetField(fieldName, 20);
			if (field != null && fsm.get_GameObject() != targetGO)
			{
				FieldInfo arg_3C_0 = field;
				SkillOwnerDefault fsmOwnerDefault = new SkillOwnerDefault();
				fsmOwnerDefault.set_OwnerOption(1);
				fsmOwnerDefault.set_GameObject(targetGO);
				arg_3C_0.SetValue(action, fsmOwnerDefault);
			}
		}
		public static void SetActionFieldValue(SkillStateAction action, string fieldName, object value)
		{
			if (action == null)
			{
				Debug.LogError(Strings.get_Error_Action_is_null_());
				return;
			}
			FieldInfo field = action.GetType().GetField(fieldName, 20);
			if (field != null)
			{
				if (field.get_FieldType().IsInstanceOfType(value))
				{
					field.SetValue(action, value);
					return;
				}
				if (!ActionUtility.TrySetValue(action, field, value))
				{
					Debug.LogError(string.Format(Strings.get_Error_Could_Not_Set_Action_Field_Value(), fieldName));
					return;
				}
			}
			else
			{
				Debug.LogError(string.Format(Strings.get_Error_Could_Not_Find_Action_Field(), fieldName));
			}
		}
		private static bool TrySetValue(SkillStateAction action, FieldInfo fieldInfo, object value)
		{
			object value2 = fieldInfo.GetValue(action);
			if (fieldInfo.get_FieldType().get_IsArray())
			{
				Type elementType = fieldInfo.get_FieldType().GetElementType();
				if (elementType == null)
				{
					return false;
				}
				value = ActionUtility.TryConvertValue(elementType, value2, value);
				if (elementType.IsInstanceOfType(value))
				{
					Array array = Array.CreateInstance(elementType, 1);
					array.SetValue(value, 0);
					fieldInfo.SetValue(action, array);
					return true;
				}
				return false;
			}
			else
			{
				value = ActionUtility.TryConvertValue(fieldInfo.get_FieldType(), value2, value);
				if (fieldInfo.get_FieldType().IsInstanceOfType(value))
				{
					fieldInfo.SetValue(action, value);
					return true;
				}
				return false;
			}
		}
		private static object TryConvertValue(Type fieldType, object currentValue, object value)
		{
			if (value == null)
			{
				return null;
			}
			Type type = value.GetType();
			if (fieldType == typeof(GameObject))
			{
				if (type.IsSubclassOf(typeof(Component)))
				{
					return ((Component)value).get_gameObject();
				}
			}
			else
			{
				if (fieldType == typeof(SkillGameObject))
				{
					if (type == typeof(GameObject))
					{
						return new SkillGameObject(value as GameObject);
					}
					if (type.IsSubclassOf(typeof(Component)))
					{
						return new SkillGameObject(((Component)value).get_gameObject());
					}
				}
				else
				{
					if (fieldType == typeof(SkillOwnerDefault))
					{
						if (type == typeof(GameObject))
						{
							SkillOwnerDefault fsmOwnerDefault = new SkillOwnerDefault();
							fsmOwnerDefault.set_OwnerOption(1);
							fsmOwnerDefault.set_GameObject(value as GameObject);
							return fsmOwnerDefault;
						}
						if (type.IsSubclassOf(typeof(Component)))
						{
							GameObject gameObject = ((Component)value).get_gameObject();
							if (gameObject != SkillEditor.SelectedFsmGameObject)
							{
								SkillOwnerDefault fsmOwnerDefault2 = new SkillOwnerDefault();
								fsmOwnerDefault2.set_OwnerOption(1);
								fsmOwnerDefault2.set_GameObject(gameObject);
								return fsmOwnerDefault2;
							}
							return new SkillOwnerDefault();
						}
					}
					else
					{
						if (fieldType == typeof(SkillProperty))
						{
							SkillProperty fsmProperty = currentValue as SkillProperty;
							SkillProperty fsmProperty2 = new SkillProperty();
							SkillProperty arg_14D_0 = fsmProperty2;
							SkillObject fsmObject = new SkillObject();
							fsmObject.set_Value(value as Object);
							arg_14D_0.TargetObject = fsmObject;
							fsmProperty2.setProperty = (fsmProperty != null && fsmProperty.setProperty);
							return fsmProperty2;
						}
						if (fieldType == typeof(SkillObject))
						{
							SkillObject fsmObject2 = new SkillObject();
							fsmObject2.set_Value(value as Object);
							return fsmObject2;
						}
						if (fieldType == typeof(SkillMaterial))
						{
							SkillMaterial fsmMaterial = new SkillMaterial();
							fsmMaterial.set_Value(value as Material);
							return fsmMaterial;
						}
						if (fieldType == typeof(SkillTexture))
						{
							SkillTexture fsmTexture = new SkillTexture();
							fsmTexture.set_Value(value as Texture);
							return fsmTexture;
						}
						if (fieldType == typeof(SkillEventTarget))
						{
							if (type == typeof(PlayMakerFSM))
							{
								return new SkillEventTarget
								{
									target = 3,
									fsmComponent = value as PlayMakerFSM
								};
							}
							if (type == typeof(GameObject))
							{
								SkillEventTarget fsmEventTarget = new SkillEventTarget();
								fsmEventTarget.target = 1;
								SkillEventTarget arg_253_0 = fsmEventTarget;
								SkillOwnerDefault fsmOwnerDefault3 = new SkillOwnerDefault();
								fsmOwnerDefault3.set_OwnerOption(1);
								fsmOwnerDefault3.set_GameObject(value as GameObject);
								arg_253_0.gameObject = fsmOwnerDefault3;
								return fsmEventTarget;
							}
						}
						else
						{
							if (fieldType == typeof(SkillString))
							{
								if (type == typeof(PlayMakerFSM))
								{
									SkillString fsmString = new SkillString();
									fsmString.set_Value(((PlayMakerFSM)value).get_FsmName());
									return fsmString;
								}
								if (type == typeof(AnimationClip))
								{
									SkillString fsmString2 = new SkillString();
									fsmString2.set_Value(((AnimationClip)value).get_name());
									return fsmString2;
								}
							}
						}
					}
				}
			}
			return value;
		}
		public static SkillStateAction AddPlayAnimationAction(Skill targetFsm, SkillState state, AnimationClip anim, SkillStateAction beforeAction = null)
		{
			SkillStateAction fsmStateAction = ActionUtility.AddAction(state, "HutongGames.PlayMaker.Actions.PlayAnimation", beforeAction);
			if (fsmStateAction == null)
			{
				return null;
			}
			if (!ActionUtility.GameObjectHasAnimationClip(targetFsm.get_GameObject(), anim.get_name()) && Dialogs.YesNoDialog(Strings.get_ActionUtility_Add_Animation_Clip_to_GameObject()))
			{
				ActionUtility.AddAnimationClip(targetFsm.get_GameObject(), anim);
			}
			FieldInfo field = fsmStateAction.GetType().GetField("animName", 20);
			if (field != null)
			{
				FieldInfo arg_68_0 = field;
				object arg_68_1 = fsmStateAction;
				SkillString fsmString = new SkillString();
				fsmString.set_Value(anim.get_name());
				arg_68_0.SetValue(arg_68_1, fsmString);
			}
			SkillEditor.SetFsmDirty(targetFsm, true, false, true);
			SkillEditor.SaveActions(targetFsm);
			return fsmStateAction;
		}
		public static void AddAnimationClip(GameObject go, AnimationClip anim)
		{
			if (go == null || anim == null)
			{
				return;
			}
			Animation animation = go.GetComponent<Animation>() ?? go.AddComponent<Animation>();
			animation.AddClip(anim, anim.get_name());
		}
		public static bool GameObjectHasAnimationClip(GameObject go, string clipName)
		{
			if (go == null || string.IsNullOrEmpty(clipName))
			{
				return false;
			}
			Animation component = go.GetComponent<Animation>();
			if (component == null)
			{
				return false;
			}
			AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(go);
			AnimationClip[] array = animationClips;
			for (int i = 0; i < array.Length; i++)
			{
				AnimationClip animationClip = array[i];
				if (!(animationClip == null) && animationClip.get_name() == clipName)
				{
					return true;
				}
			}
			return false;
		}
		public static SkillStateAction AddPlaySoundAction(Skill targetFsm, SkillState state, AudioClip audioClip, SkillStateAction beforeAction = null)
		{
			SkillStateAction fsmStateAction = ActionUtility.AddAction(state, "HutongGames.PlayMaker.Actions.PlaySound", beforeAction);
			if (fsmStateAction == null)
			{
				return null;
			}
			FieldInfo field = fsmStateAction.GetType().GetField("clip", 20);
			if (field != null)
			{
				FieldInfo arg_38_0 = field;
				object arg_38_1 = fsmStateAction;
				SkillObject fsmObject = new SkillObject();
				fsmObject.set_Value(audioClip);
				arg_38_0.SetValue(arg_38_1, fsmObject);
			}
			SkillEditor.SetFsmDirty(targetFsm, true, false, true);
			SkillEditor.SaveActions(targetFsm);
			return fsmStateAction;
		}
	}
}
