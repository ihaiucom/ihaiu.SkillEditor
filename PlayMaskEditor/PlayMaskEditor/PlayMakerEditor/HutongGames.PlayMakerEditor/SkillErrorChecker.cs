using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	internal static class FsmErrorChecker
	{
		private static GameObject gameObject;
		private static Skill checkingFsm;
		private static SkillState checkingState;
		private static SkillStateAction checkingAction;
		private static string checkingParameter;
		private static bool ownerIsAsset;
		private static object[] attributes;
		private static readonly List<FsmError> FsmErrorList = new List<FsmError>();
		private static SkillEventTarget fsmEventTargetContext;
		private static SkillEventTarget fsmEventTargetContextGlobal;
		private static bool checkForErrors;
		private static Skill checkFsm;
		private static FsmError AddError(FsmError error)
		{
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.SameAs(error))
					{
						return error;
					}
				}
			}
			FsmErrorChecker.FsmErrorList.Add(error);
			return error;
		}
		private static FsmError AddError(SkillState state, SkillStateAction action, string parameter, string error)
		{
			return FsmErrorChecker.AddError(new FsmError(state, action, parameter, error));
		}
		private static FsmError AddError(SkillState state, SkillTransition transition, string error)
		{
			return FsmErrorChecker.AddError(new FsmError(state, transition, error));
		}
		private static FsmError AddParameterError(string error)
		{
			return FsmErrorChecker.AddError(FsmErrorChecker.checkingState, FsmErrorChecker.checkingAction, FsmErrorChecker.checkingParameter, error);
		}
		private static FsmError AddRequiredFieldError()
		{
			FsmError fsmError = FsmErrorChecker.AddError(FsmErrorChecker.checkingState, FsmErrorChecker.checkingAction, FsmErrorChecker.checkingParameter, Strings.get_FsmErrorChecker_RequiredFieldError());
			fsmError.Type = FsmError.ErrorType.requiredField;
			return fsmError;
		}
		public static FsmError AddRuntimeError(string error)
		{
			FsmError error2 = new FsmError
			{
				Fsm = SkillExecutionStack.get_ExecutingFsm(),
				State = SkillExecutionStack.get_ExecutingState(),
				Action = SkillExecutionStack.get_ExecutingAction(),
				ErrorString = error,
				RuntimeError = true
			};
			return FsmErrorChecker.AddError(error2);
		}
		public static void ClearErrors(bool deleteRuntimeErrors = true)
		{
			if (!Application.get_isPlaying() && !deleteRuntimeErrors)
			{
				List<FsmError> list = new List<FsmError>();
				using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FsmError current = enumerator.get_Current();
						if (!current.RuntimeError)
						{
							list.Add(current);
						}
					}
				}
				using (List<FsmError>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						FsmError current2 = enumerator2.get_Current();
						FsmErrorChecker.FsmErrorList.Remove(current2);
					}
					goto IL_8E;
				}
			}
			FsmErrorChecker.FsmErrorList.Clear();
			IL_8E:
			FsmErrorChecker.gameObject = null;
			FsmErrorChecker.checkingFsm = null;
			FsmErrorChecker.checkingState = null;
			FsmErrorChecker.checkingAction = null;
			FsmErrorChecker.checkingParameter = null;
			FsmErrorChecker.attributes = null;
			FsmErrorChecker.checkForErrors = false;
			FsmErrorChecker.checkFsm = null;
			FsmErrorChecker.fsmEventTargetContext = null;
			FsmErrorChecker.fsmEventTargetContextGlobal = null;
		}
		public static void Refresh()
		{
			FsmErrorChecker.CheckForErrors();
		}
		public static void CheckForErrors()
		{
			FsmErrorChecker.checkForErrors = true;
		}
		public static void CheckFsmForErrors(Skill fsm, bool immediate = false)
		{
			if (fsm != null)
			{
				if (immediate)
				{
					FsmErrorChecker.DoCheckFsmForErrors(fsm);
					return;
				}
				FsmErrorChecker.checkFsm = fsm;
			}
		}
		public static void Update()
		{
			if (FsmErrorChecker.SkipErrorCheck())
			{
				return;
			}
			if (SkillEditor.NeedRepaint)
			{
				return;
			}
			if (FsmErrorChecker.checkForErrors)
			{
				FsmErrorChecker.DoCheckForErrors();
				SkillSelector.RefreshView();
			}
			else
			{
				if (FsmErrorChecker.checkFsm != null)
				{
					FsmErrorChecker.DoCheckFsmForErrors(FsmErrorChecker.checkFsm);
					SkillSelector.RefreshView();
				}
			}
			FsmErrorChecker.checkForErrors = false;
			FsmErrorChecker.checkFsm = null;
		}
		private static bool SkipErrorCheck()
		{
			return !FsmEditorSettings.EnableRealtimeErrorChecker || PlayMakerFSM.ApplicationIsQuitting || (FsmEditorSettings.DisableErrorCheckerWhenPlaying && Application.get_isPlaying());
		}
		private static void DoCheckForErrors()
		{
			FsmErrorChecker.ClearErrors(false);
			try
			{
				using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Skill current = enumerator.get_Current();
						FsmErrorChecker.DoCheckFsmForErrors(current);
					}
				}
				using (List<SkillTemplate>.Enumerator enumerator2 = Templates.List.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SkillTemplate current2 = enumerator2.get_Current();
						if (!(current2 == SkillBuilder.Clipboard))
						{
							FsmErrorChecker.DoCheckFsmForErrors(current2.fsm);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
				throw;
			}
			SkillEditor.RepaintAll();
		}
		private static void DoCheckFsmForErrors(Skill fsm)
		{
			if (fsm == null || fsm.get_OwnerObject() == null)
			{
				return;
			}
			PlayMakerFSM playMakerFSM = fsm.get_Owner() as PlayMakerFSM;
			if (playMakerFSM != null && playMakerFSM.get_UsesTemplate())
			{
				return;
			}
			try
			{
				fsm.InitData();
				FsmErrorChecker.checkingFsm = fsm;
				FsmErrorChecker.ClearFsmErrors(fsm);
				SkillState[] states = fsm.get_States();
				for (int i = 0; i < states.Length; i++)
				{
					SkillState fsmState = states[i];
					fsmState.set_Fsm(fsm);
					FsmErrorChecker.fsmEventTargetContextGlobal = null;
					SkillStateAction[] actions = fsmState.get_Actions();
					for (int j = 0; j < actions.Length; j++)
					{
						SkillStateAction fsmStateAction = actions[j];
						if (fsmStateAction.get_Enabled())
						{
							FsmErrorChecker.CheckActionForErrors(fsmState, fsmStateAction);
						}
					}
					FsmErrorChecker.CheckTransitionsForErrors(fsmState);
				}
				FsmErrorChecker.CheckActionReportForErrors();
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
				throw;
			}
			SkillEditor.RepaintAll();
		}
		private static void ClearFsmErrors(Skill fsm)
		{
			List<FsmError> list = new List<FsmError>();
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.Fsm == fsm && !current.RuntimeError)
					{
						list.Add(current);
					}
					if (!SkillEditor.FsmList.Contains(current.Fsm))
					{
						list.Add(current);
					}
				}
			}
			using (List<FsmError>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					FsmError current2 = enumerator2.get_Current();
					FsmErrorChecker.FsmErrorList.Remove(current2);
				}
			}
		}
		[Localizable(false)]
		private static void CheckActionForErrors(SkillState state, SkillStateAction action)
		{
			if (action == null)
			{
				FsmErrorChecker.AddError(new FsmError(state, null, Strings.get_FsmErrorChecker_MissingActionError()));
				return;
			}
			action.Init(state);
			FsmErrorChecker.fsmEventTargetContext = null;
			string actionLabel = Labels.GetActionLabel(action);
			if (FsmEditorSettings.CheckForMissingActions && action is MissingAction)
			{
				FsmErrorChecker.AddError(new FsmError(state, action, Strings.get_FsmErrorChecker_StateHasMissingActionError()));
				return;
			}
			if (FsmEditorSettings.CheckForObsoleteActions)
			{
				string obsoleteMessage = CustomAttributeHelpers.GetObsoleteMessage(action.GetType());
				if (!string.IsNullOrEmpty(obsoleteMessage))
				{
					FsmErrorChecker.AddError(new FsmError(state, action, obsoleteMessage));
				}
			}
			Type type = action.GetType();
			FieldInfo[] fields = ActionData.GetFields(type);
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = array[i];
				Type fieldType = fieldInfo.get_FieldType();
				object value = fieldInfo.GetValue(action);
				if (fieldType == typeof(SkillEventTarget))
				{
					SkillEventTarget fsmEventTarget = (SkillEventTarget)value;
					if (actionLabel == "Set Event Target")
					{
						FsmErrorChecker.fsmEventTargetContextGlobal = fsmEventTarget;
					}
					else
					{
						FsmErrorChecker.fsmEventTargetContext = fsmEventTarget;
					}
				}
				FsmErrorChecker.CheckActionParameter(state, action, fieldInfo);
			}
			string text = "";
			try
			{
				text = action.ErrorCheck();
			}
			catch (Exception ex)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Bad ErrorCheck: ",
					type,
					"\n",
					ex
				}));
			}
			if (!string.IsNullOrEmpty(text))
			{
				FsmErrorChecker.AddError(new FsmError(state, action, text));
			}
		}
		private static void CheckActionParameter(SkillState state, SkillStateAction action, FieldInfo field)
		{
			if (state == null || action == null || field == null || state.get_Fsm() == null)
			{
				return;
			}
			Object ownerObject = state.get_Fsm().get_OwnerObject();
			FsmErrorChecker.ownerIsAsset = SkillPrefabs.IsPersistent(ownerObject);
			FsmErrorChecker.gameObject = state.get_Fsm().get_GameObject();
			FsmErrorChecker.checkingFsm = state.get_Fsm();
			FsmErrorChecker.checkingState = state;
			FsmErrorChecker.checkingAction = action;
			FsmErrorChecker.checkingParameter = field.get_Name();
			Type fieldType = field.get_FieldType();
			object value = field.GetValue(action);
			FsmErrorChecker.attributes = CustomAttributeHelpers.GetCustomAttributes(field);
			FsmErrorChecker.CheckParameterType(fieldType, value);
		}
		private static void CheckParameterType(Type type, object fieldValue)
		{
			if (type == null)
			{
				return;
			}
			if (type == typeof(SkillGameObject))
			{
				FsmErrorChecker.CheckFsmGameObjectParameter((SkillGameObject)fieldValue);
			}
			else
			{
				if (type == typeof(SkillOwnerDefault))
				{
					FsmErrorChecker.CheckOwnerDefaultParameter((SkillOwnerDefault)fieldValue);
				}
				else
				{
					if (type == typeof(GameObject))
					{
						FsmErrorChecker.CheckGameObjectParameter((GameObject)fieldValue);
					}
					else
					{
						if (type == typeof(SkillEvent))
						{
							FsmErrorChecker.CheckFsmEventParameter((SkillEvent)fieldValue);
						}
						else
						{
							if (type == typeof(SkillString))
							{
								FsmErrorChecker.CheckFsmStringParameter((SkillString)fieldValue);
							}
							else
							{
								if (type == typeof(string))
								{
									FsmErrorChecker.CheckStringParameter((string)fieldValue);
								}
								else
								{
									if (type.get_IsArray())
									{
										Array array = (Array)fieldValue;
										if (array != null)
										{
											Type elementType = type.GetElementType();
											for (int i = 0; i < array.get_Length(); i++)
											{
												FsmErrorChecker.CheckParameterType(elementType, array.GetValue(i));
											}
										}
									}
									else
									{
										Object @object = fieldValue as Object;
										if (@object != null)
										{
											FsmErrorChecker.CheckObjectParameter(@object);
										}
									}
								}
							}
						}
					}
				}
			}
			if (type.IsSubclassOf(typeof(NamedVariable)))
			{
				if (FsmEditorSettings.CheckForRequiredField && FsmErrorChecker.IsRequiredField())
				{
					if (fieldValue == null)
					{
						FsmErrorChecker.AddRequiredFieldError();
						return;
					}
					NamedVariable namedVariable = (NamedVariable)fieldValue;
					if ((namedVariable.get_UseVariable() || FsmErrorChecker.IsVariableField()) && string.IsNullOrEmpty(namedVariable.get_Name()))
					{
						FsmErrorChecker.AddRequiredFieldError();
						return;
					}
				}
			}
			else
			{
				if (type == typeof(SkillVar) && FsmEditorSettings.CheckForRequiredField && FsmErrorChecker.IsRequiredField())
				{
					SkillVar fsmVar = (SkillVar)fieldValue;
					if (fsmVar.useVariable && (fsmVar.get_NamedVar() == null || fsmVar.get_NamedVar().get_IsNone()))
					{
						FsmErrorChecker.AddRequiredFieldError();
					}
				}
			}
		}
		private static void CheckFsmStringParameter(SkillString fsmString)
		{
			if (fsmString != null && !fsmString.get_UseVariable())
			{
				FsmErrorChecker.CheckStringParameter(fsmString.get_Value());
			}
		}
		private static void CheckStringParameter(string text)
		{
			if (FsmEditorSettings.CheckForRequiredField && string.IsNullOrEmpty(text) && FsmErrorChecker.IsRequiredField())
			{
				FsmErrorChecker.AddRequiredFieldError();
			}
		}
		private static void CheckFsmEventParameter(SkillEvent fsmEvent)
		{
			if (FsmEditorSettings.CheckForRequiredField && fsmEvent == null && FsmErrorChecker.IsRequiredField())
			{
				FsmErrorChecker.AddRequiredFieldError();
				return;
			}
			if (FsmEditorSettings.CheckForEventNotUsed)
			{
				FsmErrorChecker.CheckForEventErrors(fsmEvent);
			}
		}
		private static void CheckForEventErrors(SkillEvent fsmEvent)
		{
			if (SkillEvent.IsNullOrEmpty(fsmEvent))
			{
				return;
			}
			SkillEventTarget fsmEventTarget = FsmErrorChecker.fsmEventTargetContextGlobal;
			if (FsmErrorChecker.fsmEventTargetContext != null)
			{
				fsmEventTarget = FsmErrorChecker.fsmEventTargetContext;
			}
			if (fsmEventTarget == null)
			{
				fsmEventTarget = new SkillEventTarget();
			}
			Skill fsmTarget = Events.GetFsmTarget(FsmErrorChecker.checkingFsm, fsmEventTarget);
			switch (fsmEventTarget.target)
			{
			case 0:
				if (FsmErrorChecker.checkingState != null && !Events.FsmStateRespondsToEvent(FsmErrorChecker.checkingState, fsmEvent))
				{
					FsmError fsmError = FsmErrorChecker.AddParameterError(Strings.get_FsmErrorChecker_InvalidEventError());
					fsmError.Type = FsmError.ErrorType.missingTransitionEvent;
					fsmError.info = fsmEvent.get_Name();
				}
				break;
			case 1:
			case 2:
				break;
			case 3:
				if (fsmTarget != null && !Events.FsmRespondsToEvent(fsmTarget, fsmEvent))
				{
					FsmErrorChecker.AddParameterError(Strings.get_FsmErrorChecker_TargetFsmMissingEventError());
				}
				return;
			case 4:
				FsmErrorChecker.CheckGlobalEvent(fsmEvent);
				return;
			default:
				return;
			}
		}
		private static void CheckGlobalEvent(SkillEvent fsmEvent)
		{
			if (!fsmEvent.get_IsGlobal())
			{
				SkillEditor.Builder.SetEventIsGlobal(fsmEvent);
			}
		}
		private static void CheckOwnerDefaultParameter(SkillOwnerDefault ownerDefault)
		{
			if (ownerDefault == null)
			{
				ownerDefault = new SkillOwnerDefault();
			}
			if (ownerDefault.get_OwnerOption() == null)
			{
				FsmErrorChecker.CheckBaseGameObject(FsmErrorChecker.gameObject);
				return;
			}
			FsmErrorChecker.CheckFsmGameObjectParameter(ownerDefault.get_GameObject());
		}
		private static void CheckFsmGameObjectParameter(SkillGameObject fsmGameObject)
		{
			if (fsmGameObject == null)
			{
				fsmGameObject = new SkillGameObject(string.Empty);
			}
			if (fsmGameObject.get_UseVariable())
			{
				if (FsmEditorSettings.CheckForRequiredField && string.IsNullOrEmpty(fsmGameObject.get_Name()) && FsmErrorChecker.IsRequiredField())
				{
					FsmErrorChecker.AddRequiredFieldError();
					return;
				}
				FsmErrorChecker.CheckBaseGameObject(fsmGameObject.get_Value());
				return;
			}
			else
			{
				if (FsmEditorSettings.CheckForRequiredField && fsmGameObject.get_Value() == null && FsmErrorChecker.IsRequiredField())
				{
					FsmErrorChecker.AddRequiredFieldError();
					return;
				}
				FsmErrorChecker.CheckBaseGameObject(fsmGameObject.get_Value());
				return;
			}
		}
		private static void CheckPrefabRestrictions(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			if (FsmErrorChecker.ownerIsAsset)
			{
				PrefabType prefabType = PrefabUtility.GetPrefabType(go);
				if (prefabType != 1 && prefabType != 2)
				{
					FsmErrorChecker.AddParameterError(Strings.get_FsmErrorChecker_PrefabReferencingSceneObjectError());
				}
			}
		}
		private static void CheckGameObjectParameter(GameObject go)
		{
			if (go == null)
			{
				if (FsmEditorSettings.CheckForRequiredField && FsmErrorChecker.IsRequiredField())
				{
					FsmErrorChecker.AddRequiredFieldError();
				}
				return;
			}
			FsmErrorChecker.CheckBaseGameObject(go);
		}
		private static void CheckBaseGameObject(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			if (FsmEditorSettings.CheckForRequiredComponent)
			{
				FsmErrorChecker.CheckForRequiredComponents(go);
			}
			if (FsmEditorSettings.CheckForPrefabRestrictions)
			{
				FsmErrorChecker.CheckPrefabRestrictions(go);
			}
		}
		private static void CheckForRequiredComponents(GameObject go)
		{
			if (!FsmEditorSettings.CheckForRequiredComponent || go == null)
			{
				return;
			}
			object[] array = FsmErrorChecker.attributes;
			for (int i = 0; i < array.Length; i++)
			{
				Attribute attribute = (Attribute)array[i];
				CheckForComponentAttribute checkForComponentAttribute = attribute as CheckForComponentAttribute;
				if (checkForComponentAttribute != null)
				{
					FsmErrorChecker.CheckGameObjectHasComponent(go, checkForComponentAttribute.get_Type0());
					FsmErrorChecker.CheckGameObjectHasComponent(go, checkForComponentAttribute.get_Type1());
					FsmErrorChecker.CheckGameObjectHasComponent(go, checkForComponentAttribute.get_Type2());
				}
			}
		}
		private static void CheckGameObjectHasComponent(GameObject go, Type component)
		{
			if (go == null || component == null)
			{
				return;
			}
			if (go.GetComponent(component) == null)
			{
				FsmError fsmError = FsmErrorChecker.AddParameterError(Strings.get_FsmErrorChecker_RequiresComponentError() + Labels.StripUnityEngineNamespace(component.ToString()) + " Component!");
				fsmError.Type = FsmError.ErrorType.missingRequiredComponent;
				fsmError.GameObject = go;
				fsmError.ObjectType = component;
			}
		}
		private static void CheckObjectParameter(Object unityObject)
		{
		}
		private static bool IsRequiredField()
		{
			object[] array = FsmErrorChecker.attributes;
			for (int i = 0; i < array.Length; i++)
			{
				Attribute attribute = (Attribute)array[i];
				if (attribute is RequiredFieldAttribute)
				{
					return true;
				}
			}
			return false;
		}
		private static bool IsVariableField()
		{
			object[] array = FsmErrorChecker.attributes;
			for (int i = 0; i < array.Length; i++)
			{
				Attribute attribute = (Attribute)array[i];
				UIHintAttribute uIHintAttribute = attribute as UIHintAttribute;
				if (uIHintAttribute != null && uIHintAttribute.get_Hint() == 10)
				{
					return true;
				}
			}
			return false;
		}
		private static void CheckTransitionsForErrors(SkillState state)
		{
			List<string> list = new List<string>();
			SkillTransition[] transitions = state.get_Transitions();
			for (int i = 0; i < transitions.Length; i++)
			{
				SkillTransition fsmTransition = transitions[i];
				if (FsmEditorSettings.CheckForTransitionMissingEvent && string.IsNullOrEmpty(fsmTransition.get_EventName()))
				{
					FsmErrorChecker.AddError(state, fsmTransition, Strings.get_FsmErrorChecker_TransitionMissingEventError());
				}
				if (FsmEditorSettings.CheckForDuplicateTransitionEvent && list.Contains(fsmTransition.get_EventName()))
				{
					FsmErrorChecker.AddError(state, fsmTransition, Strings.get_FsmErrorChecker_DuplicateTransitionEventError());
				}
				if (!string.IsNullOrEmpty(fsmTransition.get_EventName()))
				{
					list.Add(fsmTransition.get_EventName());
				}
				if (FsmEditorSettings.CheckForTransitionMissingTarget && string.IsNullOrEmpty(fsmTransition.get_ToState()))
				{
					FsmErrorChecker.AddError(state, fsmTransition, Strings.get_FsmErrorChecker_TransitionMissingTargetError());
				}
				if (state.get_Fsm() != null)
				{
					SkillEvent fsmEvent = fsmTransition.get_FsmEvent();
					if (fsmEvent != null && fsmEvent.get_IsSystemEvent())
					{
						FsmErrorChecker.CheckSystemEventsForErrors(state, fsmTransition, fsmEvent);
					}
				}
			}
		}
		[Localizable(false)]
		private static void CheckSystemEventsForErrors(SkillState state, SkillTransition transition, SkillEvent fsmEvent)
		{
			GameObject gameObject = state.get_Fsm().get_GameObject();
			if (gameObject == null)
			{
				return;
			}
			if (FsmEditorSettings.CheckForMouseEventErrors && fsmEvent.get_Name().Contains("MOUSE") && gameObject.GetComponent<Collider>() == null && gameObject.GetComponent<GUIElement>() == null)
			{
				FsmErrorChecker.AddError(state, transition, Strings.get_FsmErrorChecker_MouseEventsNeedCollider());
			}
			if ((FsmEditorSettings.CheckForCollisionEventErrors && fsmEvent.get_Name().Contains("COLLISION")) || fsmEvent.get_Name().Contains("TRIGGER"))
			{
				if (fsmEvent.get_Name().Contains("2D"))
				{
					if (gameObject.GetComponent<Collider2D>() == null && gameObject.GetComponent<Rigidbody2D>() == null)
					{
						FsmErrorChecker.AddError(state, transition, Strings.get_FsmErrorChecker_CollisionEventsNeedCollider2D());
					}
				}
				else
				{
					if (gameObject.GetComponent<Collider>() == null && gameObject.GetComponent<Rigidbody>() == null)
					{
						FsmErrorChecker.AddError(state, transition, Strings.get_FsmErrorChecker_CollisionEventsNeedCollider());
					}
				}
			}
			if (FsmEditorSettings.CheckForCollisionEventErrors && fsmEvent.get_Name().Contains("CONTROLLER COLLIDER") && gameObject.GetComponent<CharacterController>() == null)
			{
				FsmErrorChecker.AddError(state, transition, Strings.get_FsmErrorChecker_ControllerCollisionEventsNeedController());
			}
		}
		private static void CheckActionReportForErrors()
		{
			using (List<ActionReport>.Enumerator enumerator = ActionReport.ActionReportList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ActionReport current = enumerator.get_Current();
					if (current.isError && current.actionIndex < current.state.get_Actions().Length)
					{
						FsmErrorChecker.AddError(current.state, current.state.get_Actions()[current.actionIndex], current.parameter, current.logText);
					}
				}
			}
		}
		public static List<FsmError> GetErrors()
		{
			return FsmErrorChecker.FsmErrorList;
		}
		public static FsmError FindError(FsmError error)
		{
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.SameAs(error))
					{
						return current;
					}
				}
			}
			return null;
		}
		public static int CountAllErrors()
		{
			return FsmErrorChecker.FsmErrorList.get_Count();
		}
		public static int CountSetupErrors()
		{
			int num = 0;
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (!current.RuntimeError)
					{
						num++;
					}
				}
			}
			return num;
		}
		public static int CountRuntimeErrors()
		{
			int num = 0;
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.RuntimeError)
					{
						num++;
					}
				}
			}
			return num;
		}
		public static int CountFsmErrors(Skill fsm)
		{
			int num = 0;
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.Fsm == fsm)
					{
						num++;
					}
				}
			}
			return num;
		}
		public static int CountStateErrors(SkillState state)
		{
			int num = 0;
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.State == state)
					{
						num++;
					}
				}
			}
			return num;
		}
		public static int CountActionErrors(SkillStateAction action)
		{
			int num = 0;
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.Action == action)
					{
						num++;
					}
				}
			}
			return num;
		}
		public static List<FsmError> GetParameterErrors(SkillStateAction action, string parameter)
		{
			List<FsmError> list = new List<FsmError>();
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.Action == action && current.Parameter == parameter)
					{
						list.Add(current);
					}
				}
			}
			return list;
		}
		public static List<string> GetTransitionErrors(SkillTransition transition)
		{
			List<string> list = new List<string>();
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.Transition == transition)
					{
						list.Add(current.ErrorString);
					}
				}
			}
			return list;
		}
		public static string GetStateErrors(SkillState state)
		{
			string text = "";
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.State == state)
					{
						text += current.ErrorString;
					}
				}
			}
			return text;
		}
		public static List<string> GetRuntimeErrors(SkillStateAction action)
		{
			List<string> list = new List<string>();
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.RuntimeError && current.Action == action)
					{
						list.Add(current.ErrorString);
					}
				}
			}
			return list;
		}
		public static bool FsmHasErrors(Skill fsm)
		{
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.Fsm == fsm)
					{
						return true;
					}
				}
			}
			return false;
		}
		public static bool StateHasErrors(SkillState state)
		{
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.State == state)
					{
						return true;
					}
				}
			}
			return false;
		}
		public static bool StateHasActionErrors(SkillState state)
		{
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.State == state && current.Action != null)
					{
						return true;
					}
				}
			}
			return false;
		}
		public static bool ActionHasErrors(SkillStateAction action)
		{
			using (List<FsmError>.Enumerator enumerator = FsmErrorChecker.FsmErrorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FsmError current = enumerator.get_Current();
					if (current.Action == action)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
