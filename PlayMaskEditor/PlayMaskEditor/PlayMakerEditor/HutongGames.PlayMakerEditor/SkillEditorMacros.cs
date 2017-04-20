using HutongGames.PlayMaker;
using System;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public static class SkillEditorMacros
	{
		private static Skill droppedOnFsm;
		private static SkillState droppedOnState;
		private static SkillStateAction droppedOnAction;
		private static Object droppedObject;
		private static Vector2 droppedPosition;
		[Localizable(false)]
		public static void DropObjectOnStateInspector(SkillState state, Object obj, SkillStateAction action)
		{
			bool flag = AssetDatabase.Contains(obj);
			SkillEditorMacros.droppedOnFsm = state.get_Fsm();
			SkillEditorMacros.droppedOnState = state;
			SkillEditorMacros.droppedOnAction = action;
			SkillEditorMacros.droppedObject = obj;
			GenericMenu genericMenu = new GenericMenu();
			if (flag)
			{
				if (SkillEditorMacros.droppedObject is AnimationClip)
				{
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_Play_Animation()), false, new GenericMenu.MenuFunction(SkillEditorMacros.AddPlayAnimationAction));
				}
				else
				{
					if (SkillEditorMacros.droppedObject is AudioClip)
					{
						genericMenu.AddItem(new GUIContent(Strings.get_Menu_Play_Sound()), false, new GenericMenu.MenuFunction(SkillEditorMacros.AddPlaySoundAction));
					}
					else
					{
						if (SkillEditorMacros.droppedObject is GameObject)
						{
							genericMenu.AddItem(new GUIContent(Strings.get_Menu_Create_Object()), false, new GenericMenu.MenuFunction(SkillEditorMacros.AddCreateObjectAction));
						}
						else
						{
							genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_No_Drag_and_Drop_Actions_Found_For_This_Asset_Type()));
							Debug.Log(SkillEditorMacros.droppedObject.GetType().get_FullName());
						}
					}
				}
			}
			else
			{
				genericMenu.AddItem(new GUIContent("Get Property"), false, new GenericMenu.MenuFunction(SkillEditorMacros.AddGetPropertyAction));
				genericMenu.AddItem(new GUIContent("Set Property"), false, new GenericMenu.MenuFunction(SkillEditorMacros.AddSetPropertyAction));
				if (SkillEditorMacros.droppedObject is GameObject)
				{
					genericMenu.AddSeparator("");
					genericMenu.AddItem(new GUIContent("Send Message"), false, new GenericMenu.MenuFunction(SkillEditorMacros.AddSendMessageAction));
				}
			}
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Cancel()), false, new GenericMenu.MenuFunction(EditorCommands.Cancel));
			genericMenu.ShowAsContext();
		}
		[Localizable(false)]
		private static void AddGetPropertyAction()
		{
			Type actionType = ActionData.GetActionType("HutongGames.PlayMaker.Actions.GetProperty");
			if (actionType == null)
			{
				Dialogs.MissingAction("Get Property");
				return;
			}
			SkillStateAction fsmStateAction = SkillEditor.Builder.InsertAction(SkillEditorMacros.droppedOnState, actionType, SkillEditorMacros.droppedOnAction);
			FieldInfo field = actionType.GetField("targetProperty", 20);
			if (field != null)
			{
				FieldInfo arg_64_0 = field;
				object arg_64_1 = fsmStateAction;
				SkillProperty fsmProperty = new SkillProperty();
				SkillProperty arg_5E_0 = fsmProperty;
				SkillObject fsmObject = new SkillObject();
				fsmObject.set_Value(SkillEditorMacros.droppedObject);
				arg_5E_0.TargetObject = fsmObject;
				arg_64_0.SetValue(arg_64_1, fsmProperty);
			}
			SkillEditor.SetFsmDirty(SkillEditorMacros.droppedOnFsm, true, false, true);
			SkillEditor.SaveActions(SkillEditorMacros.droppedOnFsm);
		}
		[Localizable(false)]
		private static void AddSetPropertyAction()
		{
			Type actionType = ActionData.GetActionType("HutongGames.PlayMaker.Actions.SetProperty");
			if (actionType == null)
			{
				Dialogs.MissingAction("Set Property");
				return;
			}
			SkillStateAction fsmStateAction = SkillEditor.Builder.InsertAction(SkillEditorMacros.droppedOnState, actionType, SkillEditorMacros.droppedOnAction);
			FieldInfo field = actionType.GetField("targetProperty", 20);
			if (field != null)
			{
				FieldInfo arg_6B_0 = field;
				object arg_6B_1 = fsmStateAction;
				SkillProperty fsmProperty = new SkillProperty();
				SkillProperty arg_5E_0 = fsmProperty;
				SkillObject fsmObject = new SkillObject();
				fsmObject.set_Value(SkillEditorMacros.droppedObject);
				arg_5E_0.TargetObject = fsmObject;
				fsmProperty.setProperty = true;
				arg_6B_0.SetValue(arg_6B_1, fsmProperty);
			}
			SkillEditor.SetFsmDirty(SkillEditorMacros.droppedOnFsm, true, false, true);
			SkillEditor.SaveActions(SkillEditorMacros.droppedOnFsm);
		}
		[Localizable(false)]
		private static void AddSendMessageAction()
		{
			SkillStateAction fsmStateAction = ActionUtility.AddAction(SkillEditor.SelectedState, "HutongGames.PlayMaker.Actions.SendMessage", SkillEditorMacros.droppedOnAction);
			if (fsmStateAction == null)
			{
				Dialogs.MissingAction("Send Message");
				return;
			}
			ActionUtility.SetOwnerDefault(SkillEditorMacros.droppedOnFsm, fsmStateAction, SkillEditorMacros.droppedObject as GameObject, "gameObject");
			SkillEditor.SetFsmDirty(SkillEditorMacros.droppedOnFsm, true, false, true);
			SkillEditor.SaveActions(SkillEditorMacros.droppedOnFsm);
		}
		[Localizable(false)]
		private static void AddCreateObjectAction()
		{
			SkillStateAction fsmStateAction = ActionUtility.AddAction(SkillEditor.SelectedState, "HutongGames.PlayMaker.Actions.CreateObject", SkillEditorMacros.droppedOnAction);
			SkillStateAction arg_32_0 = fsmStateAction;
			string arg_32_1 = "gameObject";
			SkillGameObject fsmGameObject = new SkillGameObject();
			fsmGameObject.set_Value(SkillEditorMacros.droppedObject as GameObject);
			ActionUtility.SetActionFieldValue(arg_32_0, arg_32_1, fsmGameObject);
			SkillEditor.SetFsmDirty(SkillEditorMacros.droppedOnFsm, true, false, true);
			SkillEditor.SaveActions(SkillEditorMacros.droppedOnFsm);
		}
		private static void AddPlayAnimationAction()
		{
			ActionUtility.AddPlayAnimationAction(SkillEditorMacros.droppedOnFsm, SkillEditor.SelectedState, SkillEditorMacros.droppedObject as AnimationClip, SkillEditorMacros.droppedOnAction);
		}
		private static void AddPlaySoundAction()
		{
			ActionUtility.AddPlaySoundAction(SkillEditorMacros.droppedOnFsm, SkillEditor.SelectedState, SkillEditorMacros.droppedObject as AudioClip, SkillEditorMacros.droppedOnAction);
		}
		public static void DropObjectOnGraphView(Skill targetFsm, Object obj, Vector2 position = default(Vector2))
		{
			SkillEditorMacros.droppedOnFsm = targetFsm;
			SkillEditorMacros.droppedObject = obj;
			SkillEditorMacros.droppedPosition = position;
			if (targetFsm == null)
			{
				GameObject gameObject = obj as GameObject;
				PlayMakerFSM fsmComponent = SkillBuilder.AddFsmToGameObject(gameObject, true, null);
				Selection.set_activeGameObject(gameObject);
				SkillEditor.SelectFsm(fsmComponent);
				return;
			}
			if (obj is AnimationClip)
			{
				SkillEditorMacros.DropAnimationClipOntoGraphView(SkillEditor.SelectedFsm, obj as AnimationClip, position);
				return;
			}
			if (obj is Animation)
			{
				SkillEditorMacros.DropAnimationComponentOntoGraphView(SkillEditor.SelectedFsm, obj as Animation, position);
			}
		}
		public static void DropAnimationComponentOntoGraphView(Skill targetFsm, Animation animationComponent, Vector2 position = default(Vector2))
		{
			SkillEditorMacros.droppedOnFsm = targetFsm;
			SkillEditorMacros.droppedObject = animationComponent;
			SkillEditorMacros.droppedPosition = position;
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Make_Animation_States()), false, new GenericMenu.MenuFunction(SkillEditorMacros.MakeAnimationStatesFromComponent));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_With_Global_Transitions()), false, new GenericMenu.MenuFunction(SkillEditorMacros.MakeAnimationStatesWithGlobalTransitions));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Cancel()), false, new GenericMenu.MenuFunction(EditorCommands.Cancel));
			genericMenu.ShowAsContext();
		}
		public static void DropAnimationClipOntoGraphView(Skill targetFsm, AnimationClip animationClip, Vector2 position = default(Vector2))
		{
			SkillEditorMacros.droppedOnFsm = targetFsm;
			SkillEditorMacros.droppedObject = animationClip;
			SkillEditorMacros.droppedPosition = position;
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Make_Animation_State()), false, new GenericMenu.MenuFunction(SkillEditorMacros.MakeAnimationStateFromClip));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_With_Global_Transition()), false, new GenericMenu.MenuFunction(SkillEditorMacros.MakeAnimationStateWithGlobalTransitions));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Cancel()), false, new GenericMenu.MenuFunction(EditorCommands.Cancel));
			genericMenu.ShowAsContext();
		}
		private static void MakeAnimationStateFromClip()
		{
			SkillEditorMacros.DoMakeAnimationState(false);
		}
		private static void MakeAnimationStateWithGlobalTransitions()
		{
			SkillEditorMacros.DoMakeAnimationState(true);
		}
		private static void MakeAnimationStatesFromComponent()
		{
			SkillEditorMacros.DoMakeAnimationStates(false);
		}
		private static void MakeAnimationStatesWithGlobalTransitions()
		{
			SkillEditorMacros.DoMakeAnimationStates(true);
		}
		private static void DoMakeAnimationState(bool addGlobalTransition)
		{
			Skill fsm = SkillEditorMacros.droppedOnFsm;
			AnimationClip animationClip = SkillEditorMacros.droppedObject as AnimationClip;
			Vector2 position = SkillEditorMacros.droppedPosition;
			if (fsm == null || animationClip == null)
			{
				return;
			}
			SkillEditor.RegisterUndo(fsm, "Drag and Drop");
			SkillEditorMacros.AddAnimationState(fsm, animationClip, position, addGlobalTransition);
			SkillEditor.SetFsmDirty(fsm, true, false, true);
		}
		private static void DoMakeAnimationStates(bool addGlobalTransition)
		{
			Skill fsm = SkillEditorMacros.droppedOnFsm;
			Animation animation = SkillEditorMacros.droppedObject as Animation;
			Vector2 position = SkillEditorMacros.droppedPosition;
			if (fsm == null || animation == null)
			{
				return;
			}
			SkillEditor.RegisterUndo(fsm, "Drag and Drop");
			AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(animation.get_gameObject());
			AnimationClip[] array = animationClips;
			for (int i = 0; i < array.Length; i++)
			{
				AnimationClip animationClip = array[i];
				if (animationClip != null)
				{
					SkillEditorMacros.AddAnimationState(fsm, animationClip, position, addGlobalTransition);
					position = new Vector2(position.x + 200f, position.y);
				}
			}
			SkillEditor.SetFsmDirty(fsm, true, false, true);
		}
		public static SkillState AddAnimationState(Skill targetFsm, AnimationClip anim, Vector2 position, bool addGlobalTransition)
		{
			if (targetFsm == null || anim == null || string.IsNullOrEmpty(anim.get_name()))
			{
				return null;
			}
			SkillState fsmState = SkillEditor.GraphView.AddState(position);
			SkillEditor.Builder.SetStateName(fsmState, anim.get_name());
			SkillEditor.GraphView.UpdateStateSize(fsmState);
			ActionUtility.AddPlayAnimationAction(targetFsm, fsmState, anim, null);
			if (addGlobalTransition)
			{
				SkillEvent fsmEvent = SkillEditor.Builder.AddEvent(anim.get_name());
				SkillEditor.Builder.AddGlobalTransition(fsmState, fsmEvent);
			}
			return fsmState;
		}
	}
}
