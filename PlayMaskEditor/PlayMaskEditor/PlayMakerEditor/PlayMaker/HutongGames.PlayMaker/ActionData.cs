using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class ActionData
	{
		public class Context
		{
			public Skill currentFsm;
			public SkillState currentState;
			public SkillStateAction currentAction;
			public int currentActionIndex;
			public string currentParameter;
		}
		private const string autoNameString = "~AutoName";
		private const int MUST_BE_LESS_THAN = 100000000;
		private static readonly Dictionary<string, Type> ActionTypeLookup = new Dictionary<string, Type>();
		public static readonly Dictionary<Type, FieldInfo[]> ActionFieldsLookup = new Dictionary<Type, FieldInfo[]>();
		public static readonly Dictionary<Type, int> ActionHashCodeLookup = new Dictionary<Type, int>();
		private static bool resaveActionData;
		private static readonly List<int> UsedIndices = new List<int>();
		private static readonly List<FieldInfo> InitFields = new List<FieldInfo>();
		[SerializeField]
		private List<string> actionNames = new List<string>();
		[SerializeField]
		private List<string> customNames = new List<string>();
		[SerializeField]
		private List<bool> actionEnabled = new List<bool>();
		[SerializeField]
		private List<bool> actionIsOpen = new List<bool>();
		[SerializeField]
		private List<int> actionStartIndex = new List<int>();
		[SerializeField]
		private List<int> actionHashCodes = new List<int>();
		[SerializeField]
		private List<Object> unityObjectParams;
		[SerializeField]
		private List<SkillGameObject> fsmGameObjectParams;
		[SerializeField]
		private List<SkillOwnerDefault> fsmOwnerDefaultParams;
		[SerializeField]
		private List<SkillAnimationCurve> animationCurveParams;
		[SerializeField]
		private List<FunctionCall> functionCallParams;
		[SerializeField]
		private List<SkillTemplateControl> fsmTemplateControlParams;
		[SerializeField]
		private List<SkillEventTarget> fsmEventTargetParams;
		[SerializeField]
		private List<SkillProperty> fsmPropertyParams;
		[SerializeField]
		private List<LayoutOption> layoutOptionParams;
		[SerializeField]
		private List<SkillString> fsmStringParams;
		[SerializeField]
		private List<SkillObject> fsmObjectParams;
		[SerializeField]
		private List<SkillVar> fsmVarParams;
		[SerializeField]
		private List<SkillArray> fsmArrayParams;
		[SerializeField]
		private List<SkillEnum> fsmEnumParams;
		[SerializeField]
		private List<SkillFloat> fsmFloatParams;
		[SerializeField]
		private List<SkillInt> fsmIntParams;
		[SerializeField]
		private List<SkillBool> fsmBoolParams;
		[SerializeField]
		private List<SkillVector2> fsmVector2Params;
		[SerializeField]
		private List<SkillVector3> fsmVector3Params;
		[SerializeField]
		private List<SkillColor> fsmColorParams;
		[SerializeField]
		private List<SkillRect> fsmRectParams;
		[SerializeField]
		private List<SkillQuaternion> fsmQuaternionParams;
		[SerializeField]
		private List<string> stringParams;
		[SerializeField]
		private List<byte> byteData = new List<byte>();
		[NonSerialized]
		private byte[] byteDataAsArray;
		[SerializeField]
		private List<int> arrayParamSizes;
		[SerializeField]
		private List<string> arrayParamTypes;
		[SerializeField]
		private List<int> customTypeSizes;
		[SerializeField]
		private List<string> customTypeNames;
		[SerializeField]
		private List<ParamDataType> paramDataType = new List<ParamDataType>();
		[SerializeField]
		private List<string> paramName = new List<string>();
		[SerializeField]
		private List<int> paramDataPos = new List<int>();
		[SerializeField]
		private List<int> paramByteDataSize = new List<int>();
		private int nextParamIndex;
		public int ActionCount
		{
			get
			{
				return this.actionNames.get_Count();
			}
		}
		public ActionData Copy()
		{
			return new ActionData
			{
				actionNames = new List<string>(this.actionNames),
				customNames = new List<string>(this.customNames),
				actionEnabled = new List<bool>(this.actionEnabled),
				actionIsOpen = new List<bool>(this.actionIsOpen),
				actionStartIndex = new List<int>(this.actionStartIndex),
				actionHashCodes = new List<int>(this.actionHashCodes),
				fsmFloatParams = this.CopyFsmFloatParams(),
				fsmIntParams = this.CopyFsmIntParams(),
				fsmBoolParams = this.CopyFsmBoolParams(),
				fsmColorParams = this.CopyFsmColorParams(),
				fsmVector2Params = this.CopyFsmVector2Params(),
				fsmVector3Params = this.CopyFsmVector3Params(),
				fsmRectParams = this.CopyFsmRectParams(),
				fsmQuaternionParams = this.CopyFsmQuaternionParams(),
				stringParams = this.CopyStringParams(),
				byteData = new List<byte>(this.byteData),
				unityObjectParams = (this.unityObjectParams != null) ? new List<Object>(this.unityObjectParams) : null,
				fsmStringParams = this.CopyFsmStringParams(),
				fsmObjectParams = this.CopyFsmObjectParams(),
				fsmGameObjectParams = this.CopyFsmGameObjectParams(),
				fsmOwnerDefaultParams = this.CopyFsmOwnerDefaultParams(),
				animationCurveParams = this.CopyAnimationCurveParams(),
				functionCallParams = this.CopyFunctionCallParams(),
				fsmTemplateControlParams = this.CopyFsmTemplateControlParams(),
				fsmVarParams = this.CopyFsmVarParams(),
				fsmArrayParams = this.CopyFsmArrayParams(),
				fsmEnumParams = this.CopyFsmEnumParams(),
				fsmPropertyParams = this.CopyFsmPropertyParams(),
				fsmEventTargetParams = this.CopyFsmEventTargetParams(),
				layoutOptionParams = this.CopyLayoutOptionParams(),
				arrayParamSizes = (this.arrayParamSizes != null) ? new List<int>(this.arrayParamSizes) : null,
				arrayParamTypes = (this.arrayParamTypes != null) ? new List<string>(this.arrayParamTypes) : null,
				customTypeSizes = (this.customTypeSizes != null) ? new List<int>(this.customTypeSizes) : null,
				customTypeNames = (this.customTypeNames != null) ? new List<string>(this.customTypeNames) : null,
				paramName = new List<string>(this.paramName),
				paramDataPos = new List<int>(this.paramDataPos),
				paramByteDataSize = new List<int>(this.paramByteDataSize),
				paramDataType = new List<ParamDataType>(this.paramDataType)
			};
		}
		private List<string> CopyStringParams()
		{
			if (this.stringParams == null)
			{
				return null;
			}
			return new List<string>(this.stringParams);
		}
		private List<SkillFloat> CopyFsmFloatParams()
		{
			if (this.fsmFloatParams == null)
			{
				return null;
			}
			List<SkillFloat> list = new List<SkillFloat>();
			using (List<SkillFloat>.Enumerator enumerator = this.fsmFloatParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillFloat current = enumerator.get_Current();
					list.Add(new SkillFloat(current));
				}
			}
			return list;
		}
		private List<SkillInt> CopyFsmIntParams()
		{
			if (this.fsmIntParams == null)
			{
				return null;
			}
			List<SkillInt> list = new List<SkillInt>();
			using (List<SkillInt>.Enumerator enumerator = this.fsmIntParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillInt current = enumerator.get_Current();
					list.Add(new SkillInt(current));
				}
			}
			return list;
		}
		private List<SkillBool> CopyFsmBoolParams()
		{
			if (this.fsmBoolParams == null)
			{
				return null;
			}
			List<SkillBool> list = new List<SkillBool>();
			using (List<SkillBool>.Enumerator enumerator = this.fsmBoolParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillBool current = enumerator.get_Current();
					list.Add(new SkillBool(current));
				}
			}
			return list;
		}
		private List<SkillVector2> CopyFsmVector2Params()
		{
			if (this.fsmVector2Params == null)
			{
				return null;
			}
			List<SkillVector2> list = new List<SkillVector2>();
			using (List<SkillVector2>.Enumerator enumerator = this.fsmVector2Params.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVector2 current = enumerator.get_Current();
					list.Add(new SkillVector2(current));
				}
			}
			return list;
		}
		private List<SkillVector3> CopyFsmVector3Params()
		{
			if (this.fsmVector3Params == null)
			{
				return null;
			}
			List<SkillVector3> list = new List<SkillVector3>();
			using (List<SkillVector3>.Enumerator enumerator = this.fsmVector3Params.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVector3 current = enumerator.get_Current();
					list.Add(new SkillVector3(current));
				}
			}
			return list;
		}
		private List<SkillColor> CopyFsmColorParams()
		{
			if (this.fsmColorParams == null)
			{
				return null;
			}
			List<SkillColor> list = new List<SkillColor>();
			using (List<SkillColor>.Enumerator enumerator = this.fsmColorParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillColor current = enumerator.get_Current();
					list.Add(new SkillColor(current));
				}
			}
			return list;
		}
		private List<SkillRect> CopyFsmRectParams()
		{
			if (this.fsmRectParams == null)
			{
				return null;
			}
			List<SkillRect> list = new List<SkillRect>();
			using (List<SkillRect>.Enumerator enumerator = this.fsmRectParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillRect current = enumerator.get_Current();
					list.Add(new SkillRect(current));
				}
			}
			return list;
		}
		private List<SkillQuaternion> CopyFsmQuaternionParams()
		{
			if (this.fsmQuaternionParams == null)
			{
				return null;
			}
			List<SkillQuaternion> list = new List<SkillQuaternion>();
			using (List<SkillQuaternion>.Enumerator enumerator = this.fsmQuaternionParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillQuaternion current = enumerator.get_Current();
					list.Add(new SkillQuaternion(current));
				}
			}
			return list;
		}
		private List<SkillString> CopyFsmStringParams()
		{
			if (this.fsmStringParams == null)
			{
				return null;
			}
			List<SkillString> list = new List<SkillString>();
			using (List<SkillString>.Enumerator enumerator = this.fsmStringParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillString current = enumerator.get_Current();
					list.Add(new SkillString(current));
				}
			}
			return list;
		}
		private List<SkillObject> CopyFsmObjectParams()
		{
			if (this.fsmObjectParams == null)
			{
				return null;
			}
			List<SkillObject> list = new List<SkillObject>();
			using (List<SkillObject>.Enumerator enumerator = this.fsmObjectParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillObject current = enumerator.get_Current();
					list.Add(new SkillObject(current));
				}
			}
			return list;
		}
		private List<SkillGameObject> CopyFsmGameObjectParams()
		{
			if (this.fsmGameObjectParams == null)
			{
				return null;
			}
			List<SkillGameObject> list = new List<SkillGameObject>();
			using (List<SkillGameObject>.Enumerator enumerator = this.fsmGameObjectParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillGameObject current = enumerator.get_Current();
					list.Add(new SkillGameObject(current));
				}
			}
			return list;
		}
		private List<SkillOwnerDefault> CopyFsmOwnerDefaultParams()
		{
			if (this.fsmOwnerDefaultParams == null)
			{
				return null;
			}
			List<SkillOwnerDefault> list = new List<SkillOwnerDefault>();
			using (List<SkillOwnerDefault>.Enumerator enumerator = this.fsmOwnerDefaultParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillOwnerDefault current = enumerator.get_Current();
					list.Add(new SkillOwnerDefault(current));
				}
			}
			return list;
		}
		private List<SkillAnimationCurve> CopyAnimationCurveParams()
		{
			if (this.animationCurveParams == null)
			{
				return null;
			}
			List<SkillAnimationCurve> list = new List<SkillAnimationCurve>();
			using (List<SkillAnimationCurve>.Enumerator enumerator = this.animationCurveParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillAnimationCurve current = enumerator.get_Current();
					SkillAnimationCurve fsmAnimationCurve = new SkillAnimationCurve();
					fsmAnimationCurve.curve.set_keys(current.curve.get_keys());
					SkillAnimationCurve fsmAnimationCurve2 = fsmAnimationCurve;
					fsmAnimationCurve2.curve.set_preWrapMode(current.curve.get_preWrapMode());
					fsmAnimationCurve2.curve.set_postWrapMode(current.curve.get_postWrapMode());
					list.Add(fsmAnimationCurve2);
				}
			}
			return list;
		}
		private List<FunctionCall> CopyFunctionCallParams()
		{
			if (this.functionCallParams == null)
			{
				return null;
			}
			List<FunctionCall> list = new List<FunctionCall>();
			using (List<FunctionCall>.Enumerator enumerator = this.functionCallParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FunctionCall current = enumerator.get_Current();
					list.Add(new FunctionCall(current));
				}
			}
			return list;
		}
		private List<SkillTemplateControl> CopyFsmTemplateControlParams()
		{
			if (this.fsmTemplateControlParams == null)
			{
				return null;
			}
			List<SkillTemplateControl> list = new List<SkillTemplateControl>();
			using (List<SkillTemplateControl>.Enumerator enumerator = this.fsmTemplateControlParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillTemplateControl current = enumerator.get_Current();
					list.Add(new SkillTemplateControl(current));
				}
			}
			return list;
		}
		private List<SkillVar> CopyFsmVarParams()
		{
			if (this.fsmVarParams == null)
			{
				return null;
			}
			List<SkillVar> list = new List<SkillVar>();
			using (List<SkillVar>.Enumerator enumerator = this.fsmVarParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillVar current = enumerator.get_Current();
					list.Add(new SkillVar(current));
				}
			}
			return list;
		}
		private List<SkillArray> CopyFsmArrayParams()
		{
			if (this.fsmArrayParams == null)
			{
				return null;
			}
			List<SkillArray> list = new List<SkillArray>();
			using (List<SkillArray>.Enumerator enumerator = this.fsmArrayParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillArray current = enumerator.get_Current();
					list.Add(new SkillArray(current));
				}
			}
			return list;
		}
		private List<SkillEnum> CopyFsmEnumParams()
		{
			if (this.fsmEnumParams == null)
			{
				return null;
			}
			List<SkillEnum> list = new List<SkillEnum>();
			using (List<SkillEnum>.Enumerator enumerator = this.fsmEnumParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEnum current = enumerator.get_Current();
					list.Add(new SkillEnum(current));
				}
			}
			return list;
		}
		private List<SkillProperty> CopyFsmPropertyParams()
		{
			if (this.fsmPropertyParams == null)
			{
				return null;
			}
			List<SkillProperty> list = new List<SkillProperty>();
			using (List<SkillProperty>.Enumerator enumerator = this.fsmPropertyParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillProperty current = enumerator.get_Current();
					list.Add(new SkillProperty(current));
				}
			}
			return list;
		}
		private List<SkillEventTarget> CopyFsmEventTargetParams()
		{
			if (this.fsmEventTargetParams == null)
			{
				return null;
			}
			List<SkillEventTarget> list = new List<SkillEventTarget>();
			using (List<SkillEventTarget>.Enumerator enumerator = this.fsmEventTargetParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillEventTarget current = enumerator.get_Current();
					list.Add(new SkillEventTarget(current));
				}
			}
			return list;
		}
		private List<LayoutOption> CopyLayoutOptionParams()
		{
			if (this.layoutOptionParams == null)
			{
				return null;
			}
			List<LayoutOption> list = new List<LayoutOption>();
			using (List<LayoutOption>.Enumerator enumerator = this.layoutOptionParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LayoutOption current = enumerator.get_Current();
					list.Add(new LayoutOption(current));
				}
			}
			return list;
		}
		private void ClearActionData()
		{
			this.actionNames.Clear();
			this.customNames.Clear();
			this.actionEnabled.Clear();
			this.actionIsOpen.Clear();
			this.actionStartIndex.Clear();
			this.actionHashCodes.Clear();
			this.byteData.Clear();
			this.unityObjectParams = null;
			this.fsmStringParams = null;
			this.fsmObjectParams = null;
			this.fsmVarParams = null;
			this.fsmArrayParams = null;
			this.fsmEnumParams = null;
			this.fsmGameObjectParams = null;
			this.fsmOwnerDefaultParams = null;
			this.animationCurveParams = null;
			this.functionCallParams = null;
			this.fsmTemplateControlParams = null;
			this.fsmPropertyParams = null;
			this.fsmEventTargetParams = null;
			this.layoutOptionParams = null;
			this.arrayParamSizes = null;
			this.arrayParamTypes = null;
			this.customTypeNames = null;
			this.customTypeSizes = null;
			this.fsmFloatParams = null;
			this.fsmIntParams = null;
			this.fsmBoolParams = null;
			this.fsmVector2Params = null;
			this.fsmVector3Params = null;
			this.fsmColorParams = null;
			this.fsmRectParams = null;
			this.fsmQuaternionParams = null;
			this.stringParams = null;
			this.paramDataPos.Clear();
			this.paramByteDataSize.Clear();
			this.paramDataType.Clear();
			this.paramName.Clear();
			this.nextParamIndex = 0;
		}
		public static Type GetActionType(string actionName)
		{
			Type globalType;
			if (ActionData.ActionTypeLookup.TryGetValue(actionName, ref globalType))
			{
				return globalType;
			}
			globalType = ReflectionUtils.GetGlobalType(actionName);
			if (object.ReferenceEquals(globalType, null))
			{
				return null;
			}
			ActionData.ActionTypeLookup.set_Item(actionName, globalType);
			return globalType;
		}
		public static FieldInfo[] GetFields(Type actionType)
		{
			FieldInfo[] publicFields;
			if (ActionData.ActionFieldsLookup.TryGetValue(actionType, ref publicFields))
			{
				return publicFields;
			}
			publicFields = actionType.GetPublicFields();
			ActionData.ActionFieldsLookup.set_Item(actionType, publicFields);
			return publicFields;
		}
		private static int GetActionTypeHashCode(Type actionType)
		{
			int stableHash;
			if (ActionData.ActionHashCodeLookup.TryGetValue(actionType, ref stableHash))
			{
				return stableHash;
			}
			string text = "";
			FieldInfo[] fields = ActionData.GetFields(actionType);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				text = text + fieldInfo.get_FieldType() + "|";
			}
			stableHash = ActionData.GetStableHash(text);
			ActionData.ActionHashCodeLookup.set_Item(actionType, stableHash);
			return stableHash;
		}
		private static int GetStableHash(string s)
		{
			uint num = 0u;
			byte[] bytes = Encoding.get_Unicode().GetBytes(s);
			for (int i = 0; i < bytes.Length; i++)
			{
				byte b = bytes[i];
				num += (uint)b;
				num += num << 10;
				num ^= num >> 6;
			}
			num += num << 3;
			num ^= num >> 11;
			num += num << 15;
			return (int)(num % 100000000u);
		}
		public SkillStateAction[] LoadActions(SkillState state)
		{
			ActionData.Context context = new ActionData.Context
			{
				currentState = state,
				currentFsm = state.Fsm
			};
			List<SkillStateAction> list = new List<SkillStateAction>();
			this.byteDataAsArray = this.byteData.ToArray();
			ActionData.resaveActionData = false;
			for (int i = 0; i < this.actionNames.get_Count(); i++)
			{
				SkillStateAction fsmStateAction = this.CreateAction(context, i);
				if (fsmStateAction != null)
				{
					list.Add(fsmStateAction);
				}
			}
			if (ActionData.resaveActionData && !PlayMakerGlobals.IsBuilding)
			{
				this.SaveActions(state, list.ToArray());
				list = new List<SkillStateAction>(this.LoadActions(state));
				state.Fsm.setDirty = true;
			}
			return list.ToArray();
		}
		public SkillStateAction CreateAction(SkillState state, int actionIndex)
		{
			return this.CreateAction(new ActionData.Context
			{
				currentState = state,
				currentFsm = state.Fsm
			}, actionIndex);
		}
		public SkillStateAction CreateAction(ActionData.Context context, int actionIndex)
		{
			context.currentActionIndex = actionIndex;
			SkillState currentState = context.currentState;
			if (currentState.Fsm == null)
			{
				Debug.LogError("state.Fsm == null");
			}
			string text = this.actionNames.get_Item(actionIndex);
			Type actionType = ActionData.GetActionType(text);
			if (object.ReferenceEquals(actionType, null))
			{
				string text2 = ActionData.TryFixActionName(text);
				actionType = ActionData.GetActionType(text2);
				if (object.ReferenceEquals(actionType, null))
				{
					MissingAction missingAction = (MissingAction)Activator.CreateInstance(typeof(MissingAction));
					string text3 = SkillUtility.StripNamespace(text);
					missingAction.actionName = text3;
					context.currentAction = missingAction;
					ActionData.LogError(context, "Could Not Create Action: " + text3 + " (Maybe the script was removed?)");
					Debug.LogError("Could Not Create Action: " + SkillUtility.GetPath(currentState) + text3 + " (Maybe the script was removed?)");
					return missingAction;
				}
				string info = "Action : " + text + " Updated To: " + text2;
				ActionData.LogInfo(context, info);
				text = text2;
				ActionData.resaveActionData = true;
			}
			SkillStateAction fsmStateAction = Activator.CreateInstance(actionType) as SkillStateAction;
			if (fsmStateAction == null)
			{
				MissingAction missingAction2 = (MissingAction)Activator.CreateInstance(typeof(MissingAction));
				string text4 = SkillUtility.StripNamespace(text);
				missingAction2.actionName = text4;
				context.currentAction = missingAction2;
				ActionData.LogError(context, "Could Not Create Action: " + text4 + " (Maybe the script was removed?)");
				Debug.LogError("Could Not Create Action: " + SkillUtility.GetPath(currentState) + text4 + " (Maybe the script was removed?)");
				return missingAction2;
			}
			context.currentAction = fsmStateAction;
			bool flag = true;
			if (this.paramDataType.get_Count() != this.paramDataPos.get_Count() || this.paramName.get_Count() != this.paramDataPos.get_Count())
			{
				ActionData.resaveActionData = true;
				flag = false;
			}
			int num = this.actionHashCodes.get_Item(actionIndex);
			if (num != ActionData.GetActionTypeHashCode(actionType))
			{
				fsmStateAction.Reset();
				ActionData.resaveActionData = true;
				flag = false;
				if (this.paramDataType.get_Count() != this.paramDataPos.get_Count())
				{
					ActionData.LogError(context, "Action has changed since FSM was saved. Could not recover parameters. Parameters reset to default values.");
					Debug.LogError("Action script has changed since Fsm was saved: " + SkillUtility.GetPath(currentState) + SkillUtility.StripNamespace(text) + ". Parameters reset to default values...");
				}
				else
				{
					try
					{
						fsmStateAction = this.TryRecoverAction(context, actionType, fsmStateAction, actionIndex);
					}
					catch
					{
						ActionData.LogError(context, "Action has changed since FSM was saved. Could not recover parameters. Parameters reset to default values.");
						throw;
					}
				}
			}
			this.nextParamIndex = this.actionStartIndex.get_Item(actionIndex);
			if (flag)
			{
				FieldInfo[] fields = ActionData.GetFields(actionType);
				FieldInfo[] array = fields;
				for (int i = 0; i < array.Length; i++)
				{
					FieldInfo fieldInfo = array[i];
					try
					{
						context.currentParameter = fieldInfo.get_Name();
						this.LoadActionField(context.currentFsm, fsmStateAction, fieldInfo, this.nextParamIndex);
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Concat(new object[]
						{
							"Error Loading Action: ",
							currentState.Name,
							" : ",
							text,
							" : ",
							fieldInfo.get_Name(),
							"\n",
							ex
						}));
					}
					this.nextParamIndex++;
				}
			}
			if (this.customNames.get_Count() > actionIndex)
			{
				fsmStateAction.Name = this.customNames.get_Item(actionIndex);
				if (!PlayMakerGlobals.IsBuilding && !PlayMakerFSM.NotMainThread && fsmStateAction.Name == "~AutoName")
				{
					fsmStateAction.Name = fsmStateAction.AutoName();
					fsmStateAction.IsAutoNamed = true;
				}
			}
			if (this.actionEnabled.get_Count() > actionIndex)
			{
				fsmStateAction.Enabled = this.actionEnabled.get_Item(actionIndex);
			}
			fsmStateAction.IsOpen = (this.actionIsOpen.get_Count() <= actionIndex || this.actionIsOpen.get_Item(actionIndex));
			return fsmStateAction;
		}
		private void LoadActionField(Skill fsm, object obj, FieldInfo field, int paramIndex)
		{
			Type fieldType = field.get_FieldType();
			object obj2 = null;
			if (object.ReferenceEquals(fieldType, typeof(SkillGameObject)))
			{
				obj2 = this.GetFsmGameObject(fsm, paramIndex);
			}
			else
			{
				if (object.ReferenceEquals(fieldType, typeof(SkillEvent)))
				{
					if (fsm.DataVersion > 1 && this.stringParams != null && this.stringParams.get_Count() > 0)
					{
						string text = this.stringParams.get_Item(this.paramDataPos.get_Item(paramIndex));
						obj2 = (string.IsNullOrEmpty(text) ? null : SkillEvent.GetFsmEvent(text));
					}
					else
					{
						obj2 = SkillUtility.ByteArrayToFsmEvent(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex), this.paramByteDataSize.get_Item(paramIndex));
					}
				}
				else
				{
					if (object.ReferenceEquals(fieldType, typeof(SkillFloat)))
					{
						obj2 = this.GetFsmFloat(fsm, paramIndex);
					}
					else
					{
						if (object.ReferenceEquals(fieldType, typeof(SkillInt)))
						{
							obj2 = this.GetFsmInt(fsm, paramIndex);
						}
						else
						{
							if (object.ReferenceEquals(fieldType, typeof(SkillBool)))
							{
								obj2 = this.GetFsmBool(fsm, paramIndex);
							}
							else
							{
								if (object.ReferenceEquals(fieldType, typeof(SkillVector2)))
								{
									obj2 = this.GetFsmVector2(fsm, paramIndex);
								}
								else
								{
									if (object.ReferenceEquals(fieldType, typeof(SkillVector3)))
									{
										obj2 = this.GetFsmVector3(fsm, paramIndex);
									}
									else
									{
										if (object.ReferenceEquals(fieldType, typeof(SkillRect)))
										{
											obj2 = this.GetFsmRect(fsm, paramIndex);
										}
										else
										{
											if (object.ReferenceEquals(fieldType, typeof(SkillQuaternion)))
											{
												obj2 = this.GetFsmQuaternion(fsm, paramIndex);
											}
											else
											{
												if (object.ReferenceEquals(fieldType, typeof(SkillColor)))
												{
													obj2 = this.GetFsmColor(fsm, paramIndex);
												}
												else
												{
													if (object.ReferenceEquals(fieldType, typeof(SkillObject)))
													{
														obj2 = this.GetFsmObject(fsm, paramIndex);
													}
													else
													{
														if (object.ReferenceEquals(fieldType, typeof(SkillMaterial)))
														{
															obj2 = this.GetFsmMaterial(fsm, paramIndex);
														}
														else
														{
															if (object.ReferenceEquals(fieldType, typeof(SkillTexture)))
															{
																obj2 = this.GetFsmTexture(fsm, paramIndex);
															}
															else
															{
																if (object.ReferenceEquals(fieldType, typeof(FunctionCall)))
																{
																	obj2 = this.GetFunctionCall(fsm, paramIndex);
																}
																else
																{
																	if (object.ReferenceEquals(fieldType, typeof(SkillTemplateControl)))
																	{
																		obj2 = this.GetFsmTemplateControl(fsm, paramIndex);
																	}
																	else
																	{
																		if (object.ReferenceEquals(fieldType, typeof(SkillVar)))
																		{
																			obj2 = this.GetFsmVar(fsm, paramIndex);
																		}
																		else
																		{
																			if (object.ReferenceEquals(fieldType, typeof(SkillEnum)))
																			{
																				obj2 = this.GetFsmEnum(fsm, paramIndex);
																			}
																			else
																			{
																				if (object.ReferenceEquals(fieldType, typeof(SkillArray)))
																				{
																					obj2 = this.GetFsmArray(fsm, paramIndex);
																				}
																				else
																				{
																					if (object.ReferenceEquals(fieldType, typeof(SkillProperty)))
																					{
																						obj2 = this.GetFsmProperty(fsm, paramIndex);
																					}
																					else
																					{
																						if (object.ReferenceEquals(fieldType, typeof(SkillEventTarget)))
																						{
																							obj2 = this.GetFsmEventTarget(fsm, paramIndex);
																						}
																						else
																						{
																							if (object.ReferenceEquals(fieldType, typeof(LayoutOption)))
																							{
																								obj2 = this.GetLayoutOption(fsm, paramIndex);
																							}
																							else
																							{
																								if (object.ReferenceEquals(fieldType, typeof(SkillOwnerDefault)))
																								{
																									obj2 = this.GetFsmOwnerDefault(fsm, paramIndex);
																								}
																								else
																								{
																									if (object.ReferenceEquals(fieldType, typeof(SkillAnimationCurve)))
																									{
																										obj2 = (this.animationCurveParams.get_Item(this.paramDataPos.get_Item(paramIndex)) ?? new SkillAnimationCurve());
																									}
																									else
																									{
																										if (object.ReferenceEquals(fieldType, typeof(SkillString)))
																										{
																											obj2 = this.GetFsmString(fsm, paramIndex);
																										}
																										else
																										{
																											if (object.ReferenceEquals(fieldType, typeof(float)))
																											{
																												obj2 = SkillUtility.BitConverter.ToSingle(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex));
																											}
																											else
																											{
																												if (object.ReferenceEquals(fieldType, typeof(int)))
																												{
																													obj2 = SkillUtility.BitConverter.ToInt32(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex));
																												}
																												else
																												{
																													if (object.ReferenceEquals(fieldType, typeof(bool)))
																													{
																														obj2 = SkillUtility.BitConverter.ToBoolean(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex));
																													}
																													else
																													{
																														if (object.ReferenceEquals(fieldType, typeof(Color)))
																														{
																															obj2 = SkillUtility.ByteArrayToColor(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex));
																														}
																														else
																														{
																															if (object.ReferenceEquals(fieldType, typeof(Vector2)))
																															{
																																obj2 = SkillUtility.ByteArrayToVector2(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex));
																															}
																															else
																															{
																																if (object.ReferenceEquals(fieldType, typeof(Vector3)))
																																{
																																	obj2 = SkillUtility.ByteArrayToVector3(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex));
																																}
																																else
																																{
																																	if (object.ReferenceEquals(fieldType, typeof(Vector4)))
																																	{
																																		obj2 = SkillUtility.ByteArrayToVector4(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex));
																																	}
																																	else
																																	{
																																		if (object.ReferenceEquals(fieldType, typeof(Rect)))
																																		{
																																			obj2 = SkillUtility.ByteArrayToRect(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex));
																																		}
																																		else
																																		{
																																			if (object.ReferenceEquals(fieldType, typeof(string)))
																																			{
																																				if (fsm.DataVersion > 1 && this.stringParams != null && this.stringParams.get_Count() > 0)
																																				{
																																					obj2 = this.stringParams.get_Item(this.paramDataPos.get_Item(paramIndex));
																																				}
																																				else
																																				{
																																					obj2 = SkillUtility.ByteArrayToString(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex), this.paramByteDataSize.get_Item(paramIndex));
																																				}
																																			}
																																			else
																																			{
																																				if (fieldType.get_IsEnum())
																																				{
																																					obj2 = Enum.ToObject(fieldType, SkillUtility.BitConverter.ToInt32(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex)));
																																				}
																																				else
																																				{
																																					if (typeof(SkillObject).IsAssignableFrom(fieldType))
																																					{
																																						SkillObject fsmObject = this.fsmObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex));
																																						if (fsmObject != null)
																																						{
																																							field.SetValue(obj, fsmObject);
																																							return;
																																						}
																																					}
																																					else
																																					{
																																						if (typeof(Object).IsAssignableFrom(fieldType))
																																						{
																																							Object @object = this.unityObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex));
																																							if (!object.ReferenceEquals(@object, null))
																																							{
																																								if (!object.ReferenceEquals(@object.GetType(), typeof(Object)))
																																								{
																																									field.SetValue(obj, @object);
																																								}
																																								return;
																																							}
																																						}
																																						else
																																						{
																																							if (fieldType.get_IsArray())
																																							{
																																								Type globalType = ReflectionUtils.GetGlobalType(this.arrayParamTypes.get_Item(this.paramDataPos.get_Item(paramIndex)));
																																								int num = this.arrayParamSizes.get_Item(this.paramDataPos.get_Item(paramIndex));
																																								Array array = Array.CreateInstance(globalType, num);
																																								for (int i = 0; i < num; i++)
																																								{
																																									this.nextParamIndex++;
																																									this.LoadArrayElement(fsm, array, globalType, i, this.nextParamIndex);
																																								}
																																								field.SetValue(obj, array);
																																								return;
																																							}
																																							if (fieldType.get_IsClass())
																																							{
																																								Type globalType2 = ReflectionUtils.GetGlobalType(this.customTypeNames.get_Item(this.paramDataPos.get_Item(paramIndex)));
																																								object obj3 = Activator.CreateInstance(globalType2);
																																								int num2 = this.customTypeSizes.get_Item(this.paramDataPos.get_Item(paramIndex));
																																								for (int j = 0; j < num2; j++)
																																								{
																																									this.nextParamIndex++;
																																									FieldInfo field2 = globalType2.GetField(this.paramName.get_Item(this.nextParamIndex));
																																									if (!object.ReferenceEquals(field2, null))
																																									{
																																										this.LoadActionField(fsm, obj3, field2, this.nextParamIndex);
																																									}
																																								}
																																								field.SetValue(obj, obj3);
																																								return;
																																							}
																																							Debug.LogError("ActionData: Missing LoadActionField for type: " + fieldType);
																																							field.SetValue(obj, null);
																																							return;
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
						}
					}
				}
			}
			field.SetValue(obj, obj2);
		}
		private void LoadArrayElement(Skill fsm, Array field, Type fieldType, int elementIndex, int paramIndex)
		{
			if (elementIndex >= field.GetLength(0))
			{
				return;
			}
			if (paramIndex >= this.paramDataPos.get_Count())
			{
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillGameObject)))
			{
				field.SetValue(this.GetFsmGameObject(fsm, paramIndex), elementIndex);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(FunctionCall)))
			{
				field.SetValue(this.GetFunctionCall(fsm, paramIndex), elementIndex);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillProperty)))
			{
				field.SetValue(this.GetFsmProperty(fsm, paramIndex), elementIndex);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(LayoutOption)))
			{
				field.SetValue(this.GetLayoutOption(fsm, paramIndex), elementIndex);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillOwnerDefault)))
			{
				field.SetValue(this.GetFsmOwnerDefault(fsm, paramIndex), elementIndex);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillAnimationCurve)))
			{
				field.SetValue(this.animationCurveParams.get_Item(this.paramDataPos.get_Item(paramIndex)) ?? new SkillAnimationCurve(), elementIndex);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillVar)))
			{
				field.SetValue(this.GetFsmVar(fsm, paramIndex), elementIndex);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillArray)))
			{
				field.SetValue(this.GetFsmArray(fsm, paramIndex), elementIndex);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillString)))
			{
				field.SetValue(this.GetFsmString(fsm, paramIndex), elementIndex);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillObject)))
			{
				field.SetValue(this.GetFsmObject(fsm, paramIndex), elementIndex);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillMaterial)))
			{
				field.SetValue(this.GetFsmMaterial(fsm, paramIndex), elementIndex);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillTexture)))
			{
				field.SetValue(this.GetFsmTexture(fsm, paramIndex), elementIndex);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillEnum)))
			{
				field.SetValue(this.GetFsmEnum(fsm, paramIndex), elementIndex);
				return;
			}
			if (fieldType.get_IsArray())
			{
				Debug.LogError("Nested arrays are not supported!");
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillEvent)))
			{
				if (fsm.DataVersion > 1 && this.stringParams != null && this.stringParams.get_Count() > 0)
				{
					string text = this.stringParams.get_Item(this.paramDataPos.get_Item(paramIndex));
					field.SetValue(string.IsNullOrEmpty(text) ? null : SkillEvent.GetFsmEvent(text), elementIndex);
					return;
				}
				field.SetValue(SkillUtility.ByteArrayToFsmEvent(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex), this.paramByteDataSize.get_Item(paramIndex)), elementIndex);
				return;
			}
			else
			{
				if (object.ReferenceEquals(fieldType, typeof(SkillFloat)))
				{
					field.SetValue(this.GetFsmFloat(fsm, paramIndex), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(SkillInt)))
				{
					field.SetValue(this.GetFsmInt(fsm, paramIndex), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(SkillBool)))
				{
					field.SetValue(this.GetFsmBool(fsm, paramIndex), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(SkillVector2)))
				{
					field.SetValue(this.GetFsmVector2(fsm, paramIndex), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(SkillVector3)))
				{
					field.SetValue(this.GetFsmVector3(fsm, paramIndex), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(SkillRect)))
				{
					field.SetValue(this.GetFsmRect(fsm, paramIndex), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(SkillQuaternion)))
				{
					field.SetValue(this.GetFsmQuaternion(fsm, paramIndex), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(SkillColor)))
				{
					field.SetValue(this.GetFsmColor(fsm, paramIndex), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(float)))
				{
					field.SetValue(SkillUtility.BitConverter.ToSingle(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex)), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(int)))
				{
					field.SetValue(SkillUtility.BitConverter.ToInt32(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex)), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(bool)))
				{
					field.SetValue(SkillUtility.BitConverter.ToBoolean(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex)), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(Color)))
				{
					field.SetValue(SkillUtility.ByteArrayToColor(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex)), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(Vector2)))
				{
					field.SetValue(SkillUtility.ByteArrayToVector2(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex)), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(Vector3)))
				{
					field.SetValue(SkillUtility.ByteArrayToVector3(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex)), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(Vector4)))
				{
					field.SetValue(SkillUtility.ByteArrayToVector4(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex)), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(Rect)))
				{
					field.SetValue(SkillUtility.ByteArrayToRect(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex)), elementIndex);
					return;
				}
				if (object.ReferenceEquals(fieldType, typeof(string)))
				{
					if (fsm.DataVersion > 1 && this.stringParams != null && this.stringParams.get_Count() > 0)
					{
						field.SetValue(this.stringParams.get_Item(this.paramDataPos.get_Item(paramIndex)), elementIndex);
						return;
					}
					field.SetValue(SkillUtility.ByteArrayToString(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex), this.paramByteDataSize.get_Item(paramIndex)), elementIndex);
					return;
				}
				else
				{
					if (fieldType.get_IsEnum())
					{
						object obj = Enum.ToObject(fieldType, SkillUtility.BitConverter.ToInt32(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex)));
						field.SetValue(obj, elementIndex);
						return;
					}
					if (typeof(SkillObject).IsAssignableFrom(fieldType))
					{
						SkillObject fsmObject = this.fsmObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex));
						if (fsmObject != null)
						{
							field.SetValue(fsmObject, elementIndex);
							return;
						}
					}
					else
					{
						if (typeof(Object).IsAssignableFrom(fieldType))
						{
							Object @object = this.unityObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex));
							if (!object.ReferenceEquals(@object, null))
							{
								field.SetValue(@object, elementIndex);
								return;
							}
						}
						else
						{
							if (fieldType.get_IsClass())
							{
								Type globalType = ReflectionUtils.GetGlobalType(this.customTypeNames.get_Item(this.paramDataPos.get_Item(paramIndex)));
								object obj2 = Activator.CreateInstance(globalType);
								int num = this.customTypeSizes.get_Item(this.paramDataPos.get_Item(paramIndex));
								for (int i = 0; i < num; i++)
								{
									this.nextParamIndex++;
									FieldInfo field2 = globalType.GetField(this.paramName.get_Item(this.nextParamIndex));
									if (!object.ReferenceEquals(field2, null))
									{
										this.LoadActionField(fsm, obj2, field2, this.nextParamIndex);
									}
								}
								field.SetValue(obj2, elementIndex);
								return;
							}
							field.SetValue(null, elementIndex);
						}
					}
					return;
				}
			}
		}
		public static void LogError(ActionData.Context context, string error)
		{
			if (context.currentState == null || context.currentAction == null || context.currentFsm == null || object.ReferenceEquals(context.currentFsm.Owner, null))
			{
				return;
			}
			PlayMakerFSM fsm = context.currentFsm.Owner as PlayMakerFSM;
			ActionReport.LogError(fsm, context.currentState, context.currentAction, context.currentActionIndex, context.currentParameter, error);
		}
		private static void LogInfo(ActionData.Context context, string info)
		{
			if (context.currentState == null || context.currentAction == null)
			{
				return;
			}
			PlayMakerFSM fsm = context.currentFsm.Owner as PlayMakerFSM;
			ActionReport.Log(fsm, context.currentState, context.currentAction, context.currentActionIndex, context.currentParameter, info, false);
		}
		private SkillFloat GetFsmFloat(Skill fsm, int paramIndex)
		{
			if (fsm == null)
			{
				Debug.LogError("fsm == null!");
				return new SkillFloat();
			}
			int num = this.paramDataPos.get_Item(paramIndex);
			if (fsm.DataVersion > 1)
			{
				if (this.fsmFloatParams == null || this.fsmFloatParams.get_Count() <= num)
				{
					return new SkillFloat();
				}
				SkillFloat fsmFloat = this.fsmFloatParams.get_Item(num);
				if (fsmFloat == null)
				{
					return new SkillFloat();
				}
				if (string.IsNullOrEmpty(fsmFloat.Name))
				{
					return fsmFloat;
				}
				return fsm.GetFsmFloat(fsmFloat.Name);
			}
			else
			{
				if (this.paramByteDataSize == null)
				{
					Debug.LogError("paramByteDataSize == null! Data Version: " + fsm.DataVersion);
					return new SkillFloat();
				}
				return SkillUtility.ByteArrayToFsmFloat(fsm, this.byteDataAsArray, num, this.paramByteDataSize.get_Item(paramIndex));
			}
		}
		private SkillInt GetFsmInt(Skill fsm, int paramIndex)
		{
			int num = this.paramDataPos.get_Item(paramIndex);
			if (fsm.DataVersion <= 1)
			{
				return SkillUtility.ByteArrayToFsmInt(fsm, this.byteDataAsArray, num, this.paramByteDataSize.get_Item(paramIndex));
			}
			if (this.fsmIntParams == null || this.fsmIntParams.get_Count() <= num)
			{
				return new SkillInt();
			}
			SkillInt fsmInt = this.fsmIntParams.get_Item(num);
			if (fsmInt == null)
			{
				return new SkillInt();
			}
			if (string.IsNullOrEmpty(fsmInt.Name))
			{
				return fsmInt;
			}
			return fsm.GetFsmInt(fsmInt.Name);
		}
		private SkillBool GetFsmBool(Skill fsm, int paramIndex)
		{
			int num = this.paramDataPos.get_Item(paramIndex);
			if (fsm.DataVersion <= 1)
			{
				return SkillUtility.ByteArrayToFsmBool(fsm, this.byteDataAsArray, num, this.paramByteDataSize.get_Item(paramIndex));
			}
			if (this.fsmBoolParams == null || this.fsmBoolParams.get_Count() <= num)
			{
				return new SkillBool();
			}
			SkillBool fsmBool = this.fsmBoolParams.get_Item(num);
			if (fsmBool == null)
			{
				return new SkillBool();
			}
			if (string.IsNullOrEmpty(fsmBool.Name))
			{
				return fsmBool;
			}
			return fsm.GetFsmBool(fsmBool.Name);
		}
		private SkillVector2 GetFsmVector2(Skill fsm, int paramIndex)
		{
			int num = this.paramDataPos.get_Item(paramIndex);
			if (fsm.DataVersion <= 1)
			{
				return SkillUtility.ByteArrayToFsmVector2(fsm, this.byteDataAsArray, num, this.paramByteDataSize.get_Item(paramIndex));
			}
			if (this.fsmVector2Params == null || this.fsmVector2Params.get_Count() <= num)
			{
				return new SkillVector2();
			}
			SkillVector2 fsmVector = this.fsmVector2Params.get_Item(num);
			if (fsmVector == null)
			{
				return new SkillVector2();
			}
			if (string.IsNullOrEmpty(fsmVector.Name))
			{
				return fsmVector;
			}
			return fsm.GetFsmVector2(fsmVector.Name);
		}
		private SkillVector3 GetFsmVector3(Skill fsm, int paramIndex)
		{
			int num = this.paramDataPos.get_Item(paramIndex);
			if (fsm.DataVersion <= 1)
			{
				return SkillUtility.ByteArrayToFsmVector3(fsm, this.byteDataAsArray, num, this.paramByteDataSize.get_Item(paramIndex));
			}
			if (this.fsmVector3Params == null || this.fsmVector3Params.get_Count() <= num)
			{
				return new SkillVector3();
			}
			SkillVector3 fsmVector = this.fsmVector3Params.get_Item(num);
			if (fsmVector == null)
			{
				return new SkillVector3();
			}
			if (string.IsNullOrEmpty(fsmVector.Name))
			{
				return fsmVector;
			}
			return fsm.GetFsmVector3(fsmVector.Name);
		}
		private SkillColor GetFsmColor(Skill fsm, int paramIndex)
		{
			int num = this.paramDataPos.get_Item(paramIndex);
			if (fsm.DataVersion <= 1)
			{
				return SkillUtility.ByteArrayToFsmColor(fsm, this.byteDataAsArray, num, this.paramByteDataSize.get_Item(paramIndex));
			}
			if (this.fsmColorParams == null || this.fsmColorParams.get_Count() <= num)
			{
				return new SkillColor();
			}
			SkillColor fsmColor = this.fsmColorParams.get_Item(num);
			if (fsmColor == null)
			{
				return new SkillColor();
			}
			if (string.IsNullOrEmpty(fsmColor.Name))
			{
				return fsmColor;
			}
			return fsm.GetFsmColor(fsmColor.Name);
		}
		private SkillRect GetFsmRect(Skill fsm, int paramIndex)
		{
			int num = this.paramDataPos.get_Item(paramIndex);
			if (fsm.DataVersion <= 1)
			{
				return SkillUtility.ByteArrayToFsmRect(fsm, this.byteDataAsArray, num, this.paramByteDataSize.get_Item(paramIndex));
			}
			if (this.fsmRectParams == null || this.fsmRectParams.get_Count() <= num)
			{
				return new SkillRect();
			}
			SkillRect fsmRect = this.fsmRectParams.get_Item(num);
			if (fsmRect == null)
			{
				return new SkillRect();
			}
			if (string.IsNullOrEmpty(fsmRect.Name))
			{
				return fsmRect;
			}
			return fsm.GetFsmRect(fsmRect.Name);
		}
		private SkillQuaternion GetFsmQuaternion(Skill fsm, int paramIndex)
		{
			int num = this.paramDataPos.get_Item(paramIndex);
			if (fsm.DataVersion <= 1)
			{
				return SkillUtility.ByteArrayToFsmQuaternion(fsm, this.byteDataAsArray, num, this.paramByteDataSize.get_Item(paramIndex));
			}
			if (this.fsmQuaternionParams == null || this.fsmQuaternionParams.get_Count() <= num)
			{
				return new SkillQuaternion();
			}
			SkillQuaternion fsmQuaternion = this.fsmQuaternionParams.get_Item(num);
			if (fsmQuaternion == null)
			{
				return new SkillQuaternion();
			}
			if (string.IsNullOrEmpty(fsmQuaternion.Name))
			{
				return fsmQuaternion;
			}
			return fsm.GetFsmQuaternion(fsmQuaternion.Name);
		}
		private SkillGameObject GetFsmGameObject(Skill fsm, int paramIndex)
		{
			SkillGameObject fsmGameObject = this.fsmGameObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex));
			if (fsmGameObject == null)
			{
				return new SkillGameObject();
			}
			if (string.IsNullOrEmpty(fsmGameObject.Name))
			{
				return fsmGameObject;
			}
			return fsm.GetFsmGameObject(fsmGameObject.Name);
		}
		private SkillTemplateControl GetFsmTemplateControl(Skill fsm, int paramIndex)
		{
			SkillTemplateControl fsmTemplateControl = this.fsmTemplateControlParams.get_Item(this.paramDataPos.get_Item(paramIndex));
			if (fsmTemplateControl.fsmVarOverrides != null)
			{
				SkillVarOverride[] fsmVarOverrides = fsmTemplateControl.fsmVarOverrides;
				for (int i = 0; i < fsmVarOverrides.Length; i++)
				{
					SkillVarOverride fsmVarOverride = fsmVarOverrides[i];
					if (!object.ReferenceEquals(fsmTemplateControl.fsmTemplate, null) && !object.ReferenceEquals(fsmTemplateControl.fsmTemplate.fsm, null) && fsmVarOverride.variable.UsesVariable)
					{
						fsmVarOverride.variable = fsmTemplateControl.fsmTemplate.fsm.Variables.GetVariable(fsmVarOverride.variable.Name);
					}
					if (fsmVarOverride.fsmVar.NamedVar.UsesVariable)
					{
						fsmVarOverride.fsmVar.NamedVar = fsm.Variables.GetVariable(fsmVarOverride.fsmVar.variableName);
					}
				}
			}
			return fsmTemplateControl;
		}
		private SkillVar GetFsmVar(Skill fsm, int paramIndex)
		{
			SkillVar fsmVar = this.fsmVarParams.get_Item(this.paramDataPos.get_Item(paramIndex)) ?? new SkillVar();
			if (!string.IsNullOrEmpty(fsmVar.variableName))
			{
				fsmVar.NamedVar = fsm.Variables.GetVariable(fsmVar.variableName);
			}
			return fsmVar;
		}
		private SkillArray GetFsmArray(Skill fsm, int paramIndex)
		{
			SkillArray fsmArray = this.fsmArrayParams.get_Item(this.paramDataPos.get_Item(paramIndex)) ?? new SkillArray();
			if (string.IsNullOrEmpty(fsmArray.Name))
			{
				return fsmArray;
			}
			return fsm.GetFsmArray(fsmArray.Name);
		}
		private SkillEnum GetFsmEnum(Skill fsm, int paramIndex)
		{
			SkillEnum fsmEnum = this.fsmEnumParams.get_Item(this.paramDataPos.get_Item(paramIndex)) ?? new SkillEnum();
			if (string.IsNullOrEmpty(fsmEnum.Name))
			{
				return fsmEnum;
			}
			return fsm.GetFsmEnum(fsmEnum.Name);
		}
		private FunctionCall GetFunctionCall(Skill fsm, int paramIndex)
		{
			FunctionCall functionCall = this.functionCallParams.get_Item(this.paramDataPos.get_Item(paramIndex));
			if (functionCall == null)
			{
				return new FunctionCall();
			}
			if (!string.IsNullOrEmpty(functionCall.BoolParameter.Name))
			{
				functionCall.BoolParameter = fsm.GetFsmBool(functionCall.BoolParameter.Name);
			}
			if (!string.IsNullOrEmpty(functionCall.FloatParameter.Name))
			{
				functionCall.FloatParameter = fsm.GetFsmFloat(functionCall.FloatParameter.Name);
			}
			if (!string.IsNullOrEmpty(functionCall.GameObjectParameter.Name))
			{
				functionCall.GameObjectParameter = fsm.GetFsmGameObject(functionCall.GameObjectParameter.Name);
			}
			if (!string.IsNullOrEmpty(functionCall.IntParameter.Name))
			{
				functionCall.IntParameter = fsm.GetFsmInt(functionCall.IntParameter.Name);
			}
			if (!string.IsNullOrEmpty(functionCall.MaterialParameter.Name))
			{
				functionCall.MaterialParameter = fsm.GetFsmMaterial(functionCall.MaterialParameter.Name);
			}
			if (!string.IsNullOrEmpty(functionCall.ObjectParameter.Name))
			{
				functionCall.ObjectParameter = fsm.GetFsmObject(functionCall.ObjectParameter.Name);
			}
			if (!string.IsNullOrEmpty(functionCall.QuaternionParameter.Name))
			{
				functionCall.QuaternionParameter = fsm.GetFsmQuaternion(functionCall.QuaternionParameter.Name);
			}
			if (!string.IsNullOrEmpty(functionCall.RectParamater.Name))
			{
				functionCall.RectParamater = fsm.GetFsmRect(functionCall.RectParamater.Name);
			}
			if (!string.IsNullOrEmpty(functionCall.StringParameter.Name))
			{
				functionCall.StringParameter = fsm.GetFsmString(functionCall.StringParameter.Name);
			}
			if (!string.IsNullOrEmpty(functionCall.TextureParameter.Name))
			{
				functionCall.TextureParameter = fsm.GetFsmTexture(functionCall.TextureParameter.Name);
			}
			if (!string.IsNullOrEmpty(functionCall.Vector2Parameter.Name))
			{
				functionCall.Vector2Parameter = fsm.GetFsmVector2(functionCall.Vector2Parameter.Name);
			}
			if (!string.IsNullOrEmpty(functionCall.Vector3Parameter.Name))
			{
				functionCall.Vector3Parameter = fsm.GetFsmVector3(functionCall.Vector3Parameter.Name);
			}
			if (!string.IsNullOrEmpty(functionCall.EnumParameter.Name))
			{
				functionCall.EnumParameter = fsm.GetFsmEnum(functionCall.EnumParameter.Name);
			}
			if (!string.IsNullOrEmpty(functionCall.ArrayParameter.Name))
			{
				functionCall.ArrayParameter = fsm.GetFsmArray(functionCall.ArrayParameter.Name);
			}
			return functionCall;
		}
		private SkillProperty GetFsmProperty(Skill fsm, int paramIndex)
		{
			SkillProperty fsmProperty = this.fsmPropertyParams.get_Item(this.paramDataPos.get_Item(paramIndex));
			if (fsmProperty == null)
			{
				return new SkillProperty();
			}
			if (!string.IsNullOrEmpty(fsmProperty.TargetObject.Name))
			{
				fsmProperty.TargetObject = fsm.GetFsmObject(fsmProperty.TargetObject.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.BoolParameter.Name))
			{
				fsmProperty.BoolParameter = fsm.GetFsmBool(fsmProperty.BoolParameter.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.FloatParameter.Name))
			{
				fsmProperty.FloatParameter = fsm.GetFsmFloat(fsmProperty.FloatParameter.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.GameObjectParameter.Name))
			{
				fsmProperty.GameObjectParameter = fsm.GetFsmGameObject(fsmProperty.GameObjectParameter.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.IntParameter.Name))
			{
				fsmProperty.IntParameter = fsm.GetFsmInt(fsmProperty.IntParameter.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.MaterialParameter.Name))
			{
				fsmProperty.MaterialParameter = fsm.GetFsmMaterial(fsmProperty.MaterialParameter.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.ObjectParameter.Name))
			{
				fsmProperty.ObjectParameter = fsm.GetFsmObject(fsmProperty.ObjectParameter.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.QuaternionParameter.Name))
			{
				fsmProperty.QuaternionParameter = fsm.GetFsmQuaternion(fsmProperty.QuaternionParameter.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.RectParamater.Name))
			{
				fsmProperty.RectParamater = fsm.GetFsmRect(fsmProperty.RectParamater.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.StringParameter.Name))
			{
				fsmProperty.StringParameter = fsm.GetFsmString(fsmProperty.StringParameter.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.TextureParameter.Name))
			{
				fsmProperty.TextureParameter = fsm.GetFsmTexture(fsmProperty.TextureParameter.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.ColorParameter.Name))
			{
				fsmProperty.ColorParameter = fsm.GetFsmColor(fsmProperty.ColorParameter.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.Vector2Parameter.Name))
			{
				fsmProperty.Vector2Parameter = fsm.GetFsmVector2(fsmProperty.Vector2Parameter.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.Vector3Parameter.Name))
			{
				fsmProperty.Vector3Parameter = fsm.GetFsmVector3(fsmProperty.Vector3Parameter.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.EnumParameter.Name))
			{
				fsmProperty.EnumParameter = fsm.GetFsmEnum(fsmProperty.EnumParameter.Name);
			}
			if (!string.IsNullOrEmpty(fsmProperty.ArrayParameter.Name))
			{
				fsmProperty.ArrayParameter = fsm.GetFsmArray(fsmProperty.ArrayParameter.Name);
			}
			return fsmProperty;
		}
		private SkillEventTarget GetFsmEventTarget(Skill fsm, int paramIndex)
		{
			SkillEventTarget fsmEventTarget = this.fsmEventTargetParams.get_Item(this.paramDataPos.get_Item(paramIndex));
			if (fsmEventTarget == null)
			{
				return new SkillEventTarget();
			}
			if (!string.IsNullOrEmpty(fsmEventTarget.excludeSelf.Name))
			{
				fsmEventTarget.excludeSelf = fsm.GetFsmBool(fsmEventTarget.excludeSelf.Name);
			}
			string name = fsmEventTarget.gameObject.GameObject.Name;
			if (!string.IsNullOrEmpty(name))
			{
				fsmEventTarget.gameObject.GameObject = fsm.GetFsmGameObject(name);
			}
			if (!string.IsNullOrEmpty(fsmEventTarget.fsmName.Name))
			{
				fsmEventTarget.fsmName = fsm.GetFsmString(fsmEventTarget.fsmName.Name);
			}
			if (!string.IsNullOrEmpty(fsmEventTarget.sendToChildren.Name))
			{
				fsmEventTarget.sendToChildren = fsm.GetFsmBool(fsmEventTarget.sendToChildren.Name);
			}
			return fsmEventTarget;
		}
		private LayoutOption GetLayoutOption(Skill fsm, int paramIndex)
		{
			LayoutOption layoutOption = this.layoutOptionParams.get_Item(this.paramDataPos.get_Item(paramIndex));
			if (layoutOption == null)
			{
				return new LayoutOption();
			}
			if (!string.IsNullOrEmpty(layoutOption.boolParam.Name))
			{
				layoutOption.boolParam = fsm.GetFsmBool(layoutOption.boolParam.Name);
			}
			if (!string.IsNullOrEmpty(layoutOption.floatParam.Name))
			{
				layoutOption.floatParam = fsm.GetFsmFloat(layoutOption.floatParam.Name);
			}
			return layoutOption;
		}
		private SkillOwnerDefault GetFsmOwnerDefault(Skill fsm, int paramIndex)
		{
			SkillOwnerDefault fsmOwnerDefault = this.fsmOwnerDefaultParams.get_Item(this.paramDataPos.get_Item(paramIndex));
			if (fsmOwnerDefault == null)
			{
				return new SkillOwnerDefault();
			}
			if (fsmOwnerDefault.OwnerOption != OwnerDefaultOption.UseOwner)
			{
				string name = fsmOwnerDefault.GameObject.Name;
				if (!string.IsNullOrEmpty(name))
				{
					fsmOwnerDefault.GameObject = fsm.GetFsmGameObject(name);
				}
			}
			return fsmOwnerDefault;
		}
		private SkillString GetFsmString(Skill fsm, int paramIndex)
		{
			SkillString fsmString = this.fsmStringParams.get_Item(this.paramDataPos.get_Item(paramIndex));
			if (fsmString == null)
			{
				return new SkillString();
			}
			if (string.IsNullOrEmpty(fsmString.Name))
			{
				return fsmString;
			}
			return fsm.GetFsmString(fsmString.Name);
		}
		private SkillObject GetFsmObject(Skill fsm, int paramIndex)
		{
			SkillObject fsmObject = this.fsmObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex));
			if (fsmObject == null)
			{
				return new SkillObject();
			}
			if (string.IsNullOrEmpty(fsmObject.Name))
			{
				return fsmObject;
			}
			return fsm.GetFsmObject(fsmObject.Name);
		}
		private SkillMaterial GetFsmMaterial(Skill fsm, int paramIndex)
		{
			SkillObject fsmObject = this.fsmObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex));
			if (fsmObject == null)
			{
				return new SkillMaterial();
			}
			if (string.IsNullOrEmpty(fsmObject.Name))
			{
				return new SkillMaterial(fsmObject);
			}
			return fsm.GetFsmMaterial(fsmObject.Name);
		}
		private SkillTexture GetFsmTexture(Skill fsm, int paramIndex)
		{
			SkillObject fsmObject = this.fsmObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex));
			if (fsmObject == null)
			{
				return new SkillTexture();
			}
			if (string.IsNullOrEmpty(fsmObject.Name))
			{
				return new SkillTexture(fsmObject);
			}
			return fsm.GetFsmTexture(fsmObject.Name);
		}
		public bool UsesDataVersion2()
		{
			return (this.fsmArrayParams != null && this.fsmArrayParams.get_Count() > 0) || (this.fsmEnumParams != null && this.fsmEnumParams.get_Count() > 0) || (this.fsmFloatParams != null && this.fsmFloatParams.get_Count() > 0) || (this.fsmIntParams != null && this.fsmIntParams.get_Count() > 0) || (this.fsmBoolParams != null && this.fsmBoolParams.get_Count() > 0) || (this.fsmVector2Params != null && this.fsmVector2Params.get_Count() > 0) || (this.fsmVector3Params != null && this.fsmVector3Params.get_Count() > 0) || (this.fsmColorParams != null && this.fsmColorParams.get_Count() > 0) || (this.fsmRectParams != null && this.fsmRectParams.get_Count() > 0) || (this.fsmQuaternionParams != null && this.fsmQuaternionParams.get_Count() > 0) || (this.stringParams != null && this.stringParams.get_Count() > 0);
		}
		private static string TryFixActionName(string actionName)
		{
			if (actionName == "HutongGames.PlayMaker.Actions.FloatAddMutiple")
			{
				return "HutongGames.PlayMaker.Actions.FloatAddMultiple";
			}
			return "HutongGames.PlayMaker.Actions." + actionName;
		}
		private SkillStateAction TryRecoverAction(ActionData.Context context, Type actionType, SkillStateAction action, int actionIndex)
		{
			ActionData.UsedIndices.Clear();
			ActionData.InitFields.Clear();
			int num = this.actionStartIndex.get_Item(actionIndex);
			int num2 = (actionIndex < this.actionNames.get_Count() - 1) ? this.actionStartIndex.get_Item(actionIndex + 1) : this.paramDataPos.get_Count();
			if (this.paramName.get_Count() == this.paramDataPos.get_Count())
			{
				for (int i = num; i < num2; i++)
				{
					FieldInfo fieldInfo = this.FindField(actionType, i);
					if (!object.ReferenceEquals(fieldInfo, null))
					{
						this.nextParamIndex = i;
						this.LoadActionField(context.currentFsm, action, fieldInfo, i);
						ActionData.UsedIndices.Add(i);
						ActionData.InitFields.Add(fieldInfo);
					}
				}
				for (int j = num; j < num2; j++)
				{
					if (!ActionData.UsedIndices.Contains(j))
					{
						FieldInfo fieldInfo2 = ActionData.FindField(actionType, this.paramName.get_Item(j));
						if (!object.ReferenceEquals(fieldInfo2, null))
						{
							this.nextParamIndex = j;
							if (this.TryConvertParameter(context, action, fieldInfo2, j))
							{
								ActionData.UsedIndices.Add(j);
								ActionData.InitFields.Add(fieldInfo2);
							}
						}
					}
				}
			}
			FieldInfo[] fields = ActionData.GetFields(actionType);
			for (int k = 0; k < fields.Length; k++)
			{
				FieldInfo fieldInfo3 = fields[k];
				if (!ActionData.InitFields.Contains(fieldInfo3))
				{
					ActionData.LogInfo(context, "New parameter: " + fieldInfo3.get_Name() + " (set to default value).");
				}
			}
			return action;
		}
		private FieldInfo FindField(Type actionType, int paramIndex)
		{
			string text = this.paramName.get_Item(paramIndex);
			ParamDataType paramDataType = this.paramDataType.get_Item(paramIndex);
			FieldInfo[] fields = ActionData.GetFields(actionType);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				ParamDataType paramDataType2 = ActionData.GetParamDataType(fieldInfo.get_FieldType());
				if (fieldInfo.get_Name() == text && paramDataType2 == paramDataType && !ActionData.InitFields.Contains(fieldInfo))
				{
					FieldInfo result;
					if (paramDataType2 == ParamDataType.Array)
					{
						Type elementType = fieldInfo.GetType().GetElementType();
						if (object.ReferenceEquals(elementType, null))
						{
							result = null;
							return result;
						}
						if (this.arrayParamTypes.get_Item(paramIndex) == elementType.get_FullName())
						{
							result = fieldInfo;
							return result;
						}
					}
					result = fieldInfo;
					return result;
				}
			}
			return null;
		}
		private static FieldInfo FindField(Type actionType, string name)
		{
			FieldInfo[] fields = ActionData.GetFields(actionType);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				if (fieldInfo.get_Name() == name && !ActionData.InitFields.Contains(fieldInfo))
				{
					return fieldInfo;
				}
			}
			return null;
		}
		private bool TryConvertParameter(ActionData.Context context, SkillStateAction action, FieldInfo field, int paramIndex)
		{
			Type fieldType = field.get_FieldType();
			ParamDataType paramDataType = this.paramDataType.get_Item(paramIndex);
			ParamDataType paramDataType2 = ActionData.GetParamDataType(fieldType);
			if (paramDataType2 != ParamDataType.Array && paramDataType == paramDataType2)
			{
				this.LoadActionField(context.currentFsm, action, field, paramIndex);
			}
			else
			{
				if (paramDataType == ParamDataType.Enum && paramDataType2 == ParamDataType.FsmEnum)
				{
					ActionData.LogInfo(context, field.get_Name() + ": Upgraded from Enum to FsmEnum");
					object[] customAttributes = field.GetCustomAttributes(true);
					Type enumType = typeof(Enum);
					object[] array = customAttributes;
					for (int i = 0; i < array.Length; i++)
					{
						object obj = array[i];
						ObjectTypeAttribute objectTypeAttribute = obj as ObjectTypeAttribute;
						if (objectTypeAttribute != null)
						{
							enumType = objectTypeAttribute.ObjectType;
							break;
						}
					}
					field.SetValue(action, new SkillEnum("", enumType, SkillUtility.BitConverter.ToInt32(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))));
				}
			}
			if (paramDataType == ParamDataType.String && paramDataType2 == ParamDataType.FsmString)
			{
				ActionData.LogInfo(context, field.get_Name() + ": Upgraded from string to FsmString");
				if (context.currentFsm.DataVersion > 1 && this.stringParams != null && this.stringParams.get_Count() > 0)
				{
					field.SetValue(action, new SkillString
					{
						Value = this.stringParams.get_Item(this.paramDataPos.get_Item(paramIndex))
					});
				}
				else
				{
					field.SetValue(action, new SkillString
					{
						Value = SkillUtility.ByteArrayToString(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex), this.paramByteDataSize.get_Item(paramIndex))
					});
				}
			}
			else
			{
				if (paramDataType == ParamDataType.Integer && paramDataType2 == ParamDataType.FsmInt)
				{
					ActionData.LogInfo(context, field.get_Name() + ": Upgraded from int to FsmInt");
					field.SetValue(action, new SkillInt
					{
						Value = SkillUtility.BitConverter.ToInt32(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
					});
				}
				else
				{
					if (paramDataType == ParamDataType.Float && paramDataType2 == ParamDataType.FsmFloat)
					{
						ActionData.LogInfo(context, field.get_Name() + ": Upgraded from float to FsmFloat");
						field.SetValue(action, new SkillFloat
						{
							Value = SkillUtility.BitConverter.ToSingle(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
						});
					}
					else
					{
						if (paramDataType == ParamDataType.Boolean && paramDataType2 == ParamDataType.FsmBool)
						{
							ActionData.LogInfo(context, field.get_Name() + ": Upgraded from bool to FsmBool");
							field.SetValue(action, new SkillBool
							{
								Value = SkillUtility.BitConverter.ToBoolean(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
							});
						}
						else
						{
							if (paramDataType == ParamDataType.GameObject && paramDataType2 == ParamDataType.FsmGameObject)
							{
								ActionData.LogInfo(context, field.get_Name() + ": Upgraded from from GameObject to FsmGameObject");
								field.SetValue(action, new SkillGameObject
								{
									Value = (GameObject)this.unityObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex))
								});
							}
							else
							{
								if (paramDataType == ParamDataType.GameObject && paramDataType2 == ParamDataType.FsmOwnerDefault)
								{
									ActionData.LogInfo(context, field.get_Name() + ": Upgraded from GameObject to FsmOwnerDefault");
									SkillOwnerDefault fsmOwnerDefault = new SkillOwnerDefault
									{
										GameObject = new SkillGameObject
										{
											Value = (GameObject)this.unityObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex))
										}
									};
									field.SetValue(action, fsmOwnerDefault);
								}
								else
								{
									if (paramDataType == ParamDataType.FsmGameObject && paramDataType2 == ParamDataType.FsmOwnerDefault)
									{
										ActionData.LogInfo(context, field.get_Name() + ": Converted from FsmGameObject to FsmOwnerDefault");
										SkillOwnerDefault fsmOwnerDefault2 = new SkillOwnerDefault
										{
											GameObject = this.fsmGameObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex)),
											OwnerOption = OwnerDefaultOption.SpecifyGameObject
										};
										field.SetValue(action, fsmOwnerDefault2);
									}
									else
									{
										if (paramDataType == ParamDataType.Vector3 && paramDataType2 == ParamDataType.FsmVector3)
										{
											ActionData.LogInfo(context, field.get_Name() + ": Upgraded from Vector3 to FsmVector3");
											field.SetValue(action, new SkillVector3
											{
												Value = SkillUtility.ByteArrayToVector3(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
											});
										}
										else
										{
											if (paramDataType == ParamDataType.Vector2 && paramDataType2 == ParamDataType.FsmVector2)
											{
												ActionData.LogInfo(context, field.get_Name() + ": Upgraded from Vector2 to FsmVector2");
												field.SetValue(action, new SkillVector2
												{
													Value = SkillUtility.ByteArrayToVector2(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
												});
											}
											else
											{
												if (paramDataType == ParamDataType.Rect && paramDataType2 == ParamDataType.FsmRect)
												{
													ActionData.LogInfo(context, field.get_Name() + ": Upgraded from Rect to FsmRect");
													field.SetValue(action, new SkillRect
													{
														Value = SkillUtility.ByteArrayToRect(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
													});
												}
												else
												{
													if (paramDataType == ParamDataType.Quaternion && paramDataType2 == ParamDataType.Quaternion)
													{
														ActionData.LogInfo(context, field.get_Name() + ": Upgraded from Quaternion to FsmQuaternion");
														field.SetValue(action, new SkillQuaternion
														{
															Value = SkillUtility.ByteArrayToQuaternion(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
														});
													}
													else
													{
														if (paramDataType == ParamDataType.Color && paramDataType2 == ParamDataType.FsmColor)
														{
															ActionData.LogInfo(context, field.get_Name() + ": Upgraded from Color to FsmColor");
															field.SetValue(action, new SkillColor
															{
																Value = SkillUtility.ByteArrayToColor(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
															});
														}
														else
														{
															if (paramDataType2 == ParamDataType.FsmMaterial && paramDataType == ParamDataType.ObjectReference)
															{
																ActionData.LogInfo(context, field.get_Name() + ": Upgraded from Material to FsmMaterial");
																field.SetValue(action, new SkillMaterial
																{
																	Value = this.unityObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex)) as Material
																});
															}
															else
															{
																if (paramDataType2 == ParamDataType.FsmTexture && paramDataType == ParamDataType.ObjectReference)
																{
																	ActionData.LogInfo(context, field.get_Name() + ": Upgraded from Texture to FsmTexture");
																	field.SetValue(action, new SkillTexture
																	{
																		Value = this.unityObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex)) as Texture
																	});
																}
																else
																{
																	if (paramDataType2 == ParamDataType.FsmObject && paramDataType == ParamDataType.ObjectReference)
																	{
																		ActionData.LogInfo(context, field.get_Name() + ": Upgraded from Object to FsmObject");
																		field.SetValue(action, new SkillObject
																		{
																			Value = this.unityObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex))
																		});
																	}
																	else
																	{
																		if (paramDataType2 != ParamDataType.Array)
																		{
																			return false;
																		}
																		Type globalType = ReflectionUtils.GetGlobalType(this.arrayParamTypes.get_Item(this.paramDataPos.get_Item(paramIndex)));
																		Type elementType = fieldType.GetElementType();
																		if (object.ReferenceEquals(elementType, null))
																		{
																			ActionData.LogError(context, "Could not make array: " + field.get_Name());
																			return false;
																		}
																		int num = this.arrayParamSizes.get_Item(this.paramDataPos.get_Item(paramIndex));
																		Array array2 = Array.CreateInstance(elementType, num);
																		if (!object.ReferenceEquals(globalType, elementType))
																		{
																			ParamDataType paramDataType3 = ActionData.GetParamDataType(globalType);
																			ParamDataType paramDataType4 = ActionData.GetParamDataType(elementType);
																			for (int j = 0; j < num; j++)
																			{
																				this.nextParamIndex++;
																				if (!this.TryConvertArrayElement(context.currentFsm, array2, paramDataType3, paramDataType4, j, this.nextParamIndex))
																				{
																					ActionData.LogError(context, string.Concat(new object[]
																					{
																						"Failed to convert Array: ",
																						field.get_Name(),
																						" From: ",
																						paramDataType3,
																						" To: ",
																						paramDataType4
																					}));
																					return false;
																				}
																				ActionData.LogInfo(context, string.Concat(new object[]
																				{
																					field.get_Name(),
																					": Upgraded Array from ",
																					globalType.get_FullName(),
																					" to ",
																					paramDataType4
																				}));
																			}
																		}
																		else
																		{
																			for (int k = 0; k < num; k++)
																			{
																				this.nextParamIndex++;
																				this.LoadArrayElement(context.currentFsm, array2, globalType, k, this.nextParamIndex);
																			}
																		}
																		field.SetValue(action, array2);
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
			return true;
		}
		private bool TryConvertArrayElement(Skill fsm, Array field, ParamDataType originalParamType, ParamDataType currentParamType, int elementIndex, int paramIndex)
		{
			if (elementIndex >= field.GetLength(0))
			{
				Debug.LogError("Bad array index: " + elementIndex);
				return false;
			}
			if (paramIndex >= this.paramDataPos.get_Count())
			{
				Debug.LogError("Bad param index: " + paramIndex);
				return false;
			}
			object obj = this.ConvertType(fsm, originalParamType, currentParamType, paramIndex);
			if (obj == null)
			{
				return false;
			}
			field.SetValue(obj, elementIndex);
			return true;
		}
		private object ConvertType(Skill fsm, ParamDataType originalParamType, ParamDataType currentParamType, int paramIndex)
		{
			if (originalParamType == ParamDataType.String && currentParamType == ParamDataType.FsmString)
			{
				string value;
				if (fsm.DataVersion > 1 && this.stringParams != null && this.stringParams.get_Count() > 0)
				{
					value = this.stringParams.get_Item(this.paramDataPos.get_Item(paramIndex));
				}
				else
				{
					value = SkillUtility.ByteArrayToString(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex), this.paramByteDataSize.get_Item(paramIndex));
				}
				return new SkillString
				{
					Value = value
				};
			}
			if (originalParamType == ParamDataType.Integer && currentParamType == ParamDataType.FsmInt)
			{
				return new SkillInt
				{
					Value = SkillUtility.BitConverter.ToInt32(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
				};
			}
			if (originalParamType == ParamDataType.Float && currentParamType == ParamDataType.FsmFloat)
			{
				return new SkillFloat
				{
					Value = SkillUtility.BitConverter.ToSingle(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
				};
			}
			if (originalParamType == ParamDataType.Boolean && currentParamType == ParamDataType.FsmBool)
			{
				return new SkillBool
				{
					Value = SkillUtility.BitConverter.ToBoolean(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
				};
			}
			if (originalParamType == ParamDataType.GameObject && currentParamType == ParamDataType.FsmGameObject)
			{
				return new SkillGameObject
				{
					Value = (GameObject)this.unityObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex))
				};
			}
			if (originalParamType == ParamDataType.GameObject && currentParamType == ParamDataType.FsmOwnerDefault)
			{
				return new SkillOwnerDefault
				{
					GameObject = new SkillGameObject
					{
						Value = (GameObject)this.unityObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex))
					},
					OwnerOption = OwnerDefaultOption.SpecifyGameObject
				};
			}
			if (originalParamType == ParamDataType.FsmGameObject && currentParamType == ParamDataType.FsmOwnerDefault)
			{
				return new SkillOwnerDefault
				{
					GameObject = this.fsmGameObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex)),
					OwnerOption = OwnerDefaultOption.SpecifyGameObject
				};
			}
			if (originalParamType == ParamDataType.Vector2 && currentParamType == ParamDataType.FsmVector2)
			{
				return new SkillVector2
				{
					Value = SkillUtility.ByteArrayToVector2(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
				};
			}
			if (originalParamType == ParamDataType.Vector3 && currentParamType == ParamDataType.FsmVector3)
			{
				return new SkillVector3
				{
					Value = SkillUtility.ByteArrayToVector3(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
				};
			}
			if (originalParamType == ParamDataType.Rect && currentParamType == ParamDataType.FsmRect)
			{
				return new SkillRect
				{
					Value = SkillUtility.ByteArrayToRect(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
				};
			}
			if (originalParamType == ParamDataType.Quaternion && currentParamType == ParamDataType.Quaternion)
			{
				return new SkillQuaternion
				{
					Value = SkillUtility.ByteArrayToQuaternion(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
				};
			}
			if (originalParamType == ParamDataType.Color && currentParamType == ParamDataType.FsmColor)
			{
				return new SkillColor
				{
					Value = SkillUtility.ByteArrayToColor(this.byteDataAsArray, this.paramDataPos.get_Item(paramIndex))
				};
			}
			if (currentParamType == ParamDataType.FsmMaterial && originalParamType == ParamDataType.ObjectReference)
			{
				return new SkillMaterial
				{
					Value = this.unityObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex)) as Material
				};
			}
			if (currentParamType == ParamDataType.FsmTexture && originalParamType == ParamDataType.ObjectReference)
			{
				return new SkillTexture
				{
					Value = this.unityObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex)) as Texture
				};
			}
			if (currentParamType == ParamDataType.FsmObject && originalParamType == ParamDataType.ObjectReference)
			{
				return new SkillObject
				{
					Value = this.unityObjectParams.get_Item(this.paramDataPos.get_Item(paramIndex))
				};
			}
			return null;
		}
		public void SaveActions(SkillState state, SkillStateAction[] actions)
		{
			this.ClearActionData();
			for (int i = 0; i < actions.Length; i++)
			{
				SkillStateAction action = actions[i];
				this.SaveAction(state.Fsm, action);
			}
		}
		private void SaveAction(Skill fsm, SkillStateAction action)
		{
			if (action == null)
			{
				return;
			}
			Type type = action.GetType();
			ActionData.ActionHashCodeLookup.Remove(type);
			this.actionNames.Add(type.ToString());
			this.customNames.Add(action.IsAutoNamed ? "~AutoName" : action.Name);
			this.actionEnabled.Add(action.Enabled);
			this.actionIsOpen.Add(action.IsOpen);
			this.actionStartIndex.Add(this.nextParamIndex);
			this.actionHashCodes.Add(ActionData.GetActionTypeHashCode(type));
			FieldInfo[] fields = ActionData.GetFields(type);
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = array[i];
				Type fieldType = fieldInfo.get_FieldType();
				object value = fieldInfo.GetValue(action);
				this.paramName.Add(fieldInfo.get_Name());
				this.SaveActionField(fsm, fieldType, value);
				this.nextParamIndex++;
			}
		}
		private void SaveActionField(Skill fsm, Type fieldType, object obj)
		{
			if (object.ReferenceEquals(fieldType, typeof(SkillAnimationCurve)))
			{
				if (this.animationCurveParams == null)
				{
					this.animationCurveParams = new List<SkillAnimationCurve>();
				}
				this.paramDataType.Add(ParamDataType.FsmAnimationCurve);
				this.paramDataPos.Add(this.animationCurveParams.get_Count());
				this.paramByteDataSize.Add(0);
				this.animationCurveParams.Add(obj as SkillAnimationCurve);
				return;
			}
			if (typeof(Object).IsAssignableFrom(fieldType))
			{
				if (this.unityObjectParams == null)
				{
					this.unityObjectParams = new List<Object>();
				}
				this.paramDataType.Add(object.ReferenceEquals(fieldType, typeof(GameObject)) ? ParamDataType.GameObject : ParamDataType.ObjectReference);
				this.paramDataPos.Add(this.unityObjectParams.get_Count());
				this.paramByteDataSize.Add(0);
				this.unityObjectParams.Add(obj as Object);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(FunctionCall)))
			{
				if (this.functionCallParams == null)
				{
					this.functionCallParams = new List<FunctionCall>();
				}
				this.paramDataType.Add(ParamDataType.FunctionCall);
				this.paramDataPos.Add(this.functionCallParams.get_Count());
				this.paramByteDataSize.Add(0);
				this.functionCallParams.Add(obj as FunctionCall);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillTemplateControl)))
			{
				if (this.fsmTemplateControlParams == null)
				{
					this.fsmTemplateControlParams = new List<SkillTemplateControl>();
				}
				this.paramDataType.Add(ParamDataType.FsmTemplateControl);
				this.paramDataPos.Add(this.fsmTemplateControlParams.get_Count());
				this.paramByteDataSize.Add(0);
				this.fsmTemplateControlParams.Add(obj as SkillTemplateControl);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillVar)))
			{
				if (this.fsmVarParams == null)
				{
					this.fsmVarParams = new List<SkillVar>();
				}
				this.paramDataType.Add(ParamDataType.FsmVar);
				this.paramDataPos.Add(this.fsmVarParams.get_Count());
				this.paramByteDataSize.Add(0);
				this.fsmVarParams.Add(obj as SkillVar);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillProperty)))
			{
				if (this.fsmPropertyParams == null)
				{
					this.fsmPropertyParams = new List<SkillProperty>();
				}
				this.paramDataType.Add(ParamDataType.FsmProperty);
				this.paramDataPos.Add(this.fsmPropertyParams.get_Count());
				this.paramByteDataSize.Add(0);
				this.fsmPropertyParams.Add(obj as SkillProperty);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillEventTarget)))
			{
				if (this.fsmEventTargetParams == null)
				{
					this.fsmEventTargetParams = new List<SkillEventTarget>();
				}
				this.paramDataType.Add(ParamDataType.FsmEventTarget);
				this.paramDataPos.Add(this.fsmEventTargetParams.get_Count());
				this.paramByteDataSize.Add(0);
				this.fsmEventTargetParams.Add(obj as SkillEventTarget);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(LayoutOption)))
			{
				if (this.layoutOptionParams == null)
				{
					this.layoutOptionParams = new List<LayoutOption>();
				}
				this.paramDataType.Add(ParamDataType.LayoutOption);
				this.paramDataPos.Add(this.layoutOptionParams.get_Count());
				this.paramByteDataSize.Add(0);
				this.layoutOptionParams.Add(obj as LayoutOption);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillGameObject)))
			{
				if (this.fsmGameObjectParams == null)
				{
					this.fsmGameObjectParams = new List<SkillGameObject>();
				}
				this.paramDataType.Add(ParamDataType.FsmGameObject);
				this.paramDataPos.Add(this.fsmGameObjectParams.get_Count());
				this.paramByteDataSize.Add(0);
				this.fsmGameObjectParams.Add(obj as SkillGameObject);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillOwnerDefault)))
			{
				if (this.fsmOwnerDefaultParams == null)
				{
					this.fsmOwnerDefaultParams = new List<SkillOwnerDefault>();
				}
				this.paramDataType.Add(ParamDataType.FsmOwnerDefault);
				this.paramDataPos.Add(this.fsmOwnerDefaultParams.get_Count());
				this.paramByteDataSize.Add(0);
				this.fsmOwnerDefaultParams.Add(obj as SkillOwnerDefault);
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillString)))
			{
				if (this.fsmStringParams == null)
				{
					this.fsmStringParams = new List<SkillString>();
				}
				this.paramDataType.Add(ParamDataType.FsmString);
				this.paramDataPos.Add(this.fsmStringParams.get_Count());
				this.paramByteDataSize.Add(0);
				this.fsmStringParams.Add(obj as SkillString);
				return;
			}
			if (fieldType.get_IsArray())
			{
				Type elementType = fieldType.GetElementType();
				if (object.ReferenceEquals(elementType, null))
				{
					return;
				}
				Array array;
				if (obj != null)
				{
					array = (Array)obj;
				}
				else
				{
					array = Array.CreateInstance(elementType, 0);
				}
				if (this.arrayParamSizes == null)
				{
					this.arrayParamSizes = new List<int>();
					this.arrayParamTypes = new List<string>();
				}
				this.paramDataType.Add(ParamDataType.Array);
				this.paramDataPos.Add(this.arrayParamSizes.get_Count());
				this.paramByteDataSize.Add(0);
				this.arrayParamSizes.Add(array.get_Length());
				this.arrayParamTypes.Add(elementType.get_FullName());
				IEnumerator enumerator = array.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object current = enumerator.get_Current();
						this.nextParamIndex++;
						this.paramName.Add("");
						this.SaveActionField(fsm, elementType, current);
					}
					return;
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
			if (object.ReferenceEquals(fieldType, typeof(float)))
			{
				this.paramDataType.Add(ParamDataType.Float);
				this.AddByteData(SkillUtility.BitConverter.GetBytes((float)obj));
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(int)))
			{
				this.paramDataType.Add(ParamDataType.Integer);
				this.AddByteData(SkillUtility.BitConverter.GetBytes((int)obj));
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(bool)))
			{
				this.paramDataType.Add(ParamDataType.Boolean);
				this.AddByteData(SkillUtility.BitConverter.GetBytes((bool)obj));
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(Color)))
			{
				this.paramDataType.Add(ParamDataType.Color);
				this.AddByteData(SkillUtility.ColorToByteArray((Color)obj));
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(Vector2)))
			{
				this.paramDataType.Add(ParamDataType.Vector2);
				this.AddByteData(SkillUtility.Vector2ToByteArray((Vector2)obj));
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(Vector3)))
			{
				this.paramDataType.Add(ParamDataType.Vector3);
				this.AddByteData(SkillUtility.Vector3ToByteArray((Vector3)obj));
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(Vector4)))
			{
				this.paramDataType.Add(ParamDataType.Vector4);
				this.AddByteData(SkillUtility.Vector4ToByteArray((Vector4)obj));
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(Rect)))
			{
				this.paramDataType.Add(ParamDataType.Rect);
				this.AddByteData(SkillUtility.RectToByteArray((Rect)obj));
				return;
			}
			if (object.ReferenceEquals(fieldType, typeof(SkillFloat)))
			{
				this.paramDataType.Add(ParamDataType.FsmFloat);
				if (fsm.DataVersion > 1)
				{
					if (this.fsmFloatParams == null)
					{
						this.fsmFloatParams = new List<SkillFloat>();
					}
					this.paramDataPos.Add(this.fsmFloatParams.get_Count());
					this.paramByteDataSize.Add(0);
					this.fsmFloatParams.Add(obj as SkillFloat);
					return;
				}
				this.AddByteData(SkillUtility.FsmFloatToByteArray(obj as SkillFloat));
				return;
			}
			else
			{
				if (object.ReferenceEquals(fieldType, typeof(SkillInt)))
				{
					this.paramDataType.Add(ParamDataType.FsmInt);
					if (fsm.DataVersion > 1)
					{
						if (this.fsmIntParams == null)
						{
							this.fsmIntParams = new List<SkillInt>();
						}
						this.paramDataPos.Add(this.fsmIntParams.get_Count());
						this.paramByteDataSize.Add(0);
						this.fsmIntParams.Add(obj as SkillInt);
						return;
					}
					this.AddByteData(SkillUtility.FsmIntToByteArray(obj as SkillInt));
					return;
				}
				else
				{
					if (object.ReferenceEquals(fieldType, typeof(SkillBool)))
					{
						this.paramDataType.Add(ParamDataType.FsmBool);
						if (fsm.DataVersion > 1)
						{
							if (this.fsmBoolParams == null)
							{
								this.fsmBoolParams = new List<SkillBool>();
							}
							this.paramDataPos.Add(this.fsmBoolParams.get_Count());
							this.paramByteDataSize.Add(0);
							this.fsmBoolParams.Add(obj as SkillBool);
							return;
						}
						this.AddByteData(SkillUtility.FsmBoolToByteArray(obj as SkillBool));
						return;
					}
					else
					{
						if (object.ReferenceEquals(fieldType, typeof(SkillVector2)))
						{
							this.paramDataType.Add(ParamDataType.FsmVector2);
							if (fsm.DataVersion > 1)
							{
								if (this.fsmVector2Params == null)
								{
									this.fsmVector2Params = new List<SkillVector2>();
								}
								this.paramDataPos.Add(this.fsmVector2Params.get_Count());
								this.paramByteDataSize.Add(0);
								this.fsmVector2Params.Add(obj as SkillVector2);
								return;
							}
							this.AddByteData(SkillUtility.FsmVector2ToByteArray(obj as SkillVector2));
							return;
						}
						else
						{
							if (object.ReferenceEquals(fieldType, typeof(SkillVector3)))
							{
								this.paramDataType.Add(ParamDataType.FsmVector3);
								if (fsm.DataVersion > 1)
								{
									if (this.fsmVector3Params == null)
									{
										this.fsmVector3Params = new List<SkillVector3>();
									}
									this.paramDataPos.Add(this.fsmVector3Params.get_Count());
									this.paramByteDataSize.Add(0);
									this.fsmVector3Params.Add(obj as SkillVector3);
									return;
								}
								this.AddByteData(SkillUtility.FsmVector3ToByteArray(obj as SkillVector3));
								return;
							}
							else
							{
								if (object.ReferenceEquals(fieldType, typeof(SkillRect)))
								{
									this.paramDataType.Add(ParamDataType.FsmRect);
									if (fsm.DataVersion > 1)
									{
										if (this.fsmRectParams == null)
										{
											this.fsmRectParams = new List<SkillRect>();
										}
										this.paramDataPos.Add(this.fsmRectParams.get_Count());
										this.paramByteDataSize.Add(0);
										this.fsmRectParams.Add(obj as SkillRect);
										return;
									}
									this.AddByteData(SkillUtility.FsmRectToByteArray(obj as SkillRect));
									return;
								}
								else
								{
									if (object.ReferenceEquals(fieldType, typeof(SkillQuaternion)))
									{
										this.paramDataType.Add(ParamDataType.FsmQuaternion);
										if (fsm.DataVersion > 1)
										{
											if (this.fsmQuaternionParams == null)
											{
												this.fsmQuaternionParams = new List<SkillQuaternion>();
											}
											this.paramDataPos.Add(this.fsmQuaternionParams.get_Count());
											this.paramByteDataSize.Add(0);
											this.fsmQuaternionParams.Add(obj as SkillQuaternion);
											return;
										}
										this.AddByteData(SkillUtility.FsmQuaternionToByteArray(obj as SkillQuaternion));
										return;
									}
									else
									{
										if (object.ReferenceEquals(fieldType, typeof(SkillColor)))
										{
											this.paramDataType.Add(ParamDataType.FsmColor);
											if (fsm.DataVersion > 1)
											{
												if (this.fsmColorParams == null)
												{
													this.fsmColorParams = new List<SkillColor>();
												}
												this.paramDataPos.Add(this.fsmColorParams.get_Count());
												this.paramByteDataSize.Add(0);
												this.fsmColorParams.Add(obj as SkillColor);
												return;
											}
											this.AddByteData(SkillUtility.FsmColorToByteArray(obj as SkillColor));
											return;
										}
										else
										{
											if (object.ReferenceEquals(fieldType, typeof(SkillEvent)))
											{
												this.paramDataType.Add(ParamDataType.FsmEvent);
												if (fsm.DataVersion > 1)
												{
													this.SaveString((obj != null) ? ((SkillEvent)obj).Name : string.Empty);
													return;
												}
												this.AddByteData(SkillUtility.FsmEventToByteArray(obj as SkillEvent));
												return;
											}
											else
											{
												if (object.ReferenceEquals(fieldType, typeof(string)))
												{
													this.paramDataType.Add(ParamDataType.String);
													if (fsm.DataVersion > 1)
													{
														this.SaveString(obj as string);
														return;
													}
													this.AddByteData(SkillUtility.StringToByteArray(obj as string));
													return;
												}
												else
												{
													if (object.ReferenceEquals(fieldType, typeof(SkillObject)))
													{
														if (this.fsmObjectParams == null)
														{
															this.fsmObjectParams = new List<SkillObject>();
														}
														this.paramDataType.Add(ParamDataType.FsmObject);
														this.paramDataPos.Add(this.fsmObjectParams.get_Count());
														this.paramByteDataSize.Add(0);
														this.fsmObjectParams.Add(obj as SkillObject);
														return;
													}
													if (object.ReferenceEquals(fieldType, typeof(SkillArray)))
													{
														if (this.fsmArrayParams == null)
														{
															this.fsmArrayParams = new List<SkillArray>();
														}
														this.paramDataType.Add(ParamDataType.FsmArray);
														this.paramDataPos.Add(this.fsmArrayParams.get_Count());
														this.paramByteDataSize.Add(0);
														this.fsmArrayParams.Add(obj as SkillArray);
														return;
													}
													if (object.ReferenceEquals(fieldType, typeof(SkillEnum)))
													{
														if (this.fsmEnumParams == null)
														{
															this.fsmEnumParams = new List<SkillEnum>();
														}
														this.paramDataType.Add(ParamDataType.FsmEnum);
														this.paramDataPos.Add(this.fsmEnumParams.get_Count());
														this.paramByteDataSize.Add(0);
														this.fsmEnumParams.Add(obj as SkillEnum);
														return;
													}
													if (object.ReferenceEquals(fieldType, typeof(SkillMaterial)))
													{
														if (this.fsmObjectParams == null)
														{
															this.fsmObjectParams = new List<SkillObject>();
														}
														this.paramDataType.Add(ParamDataType.FsmMaterial);
														this.paramDataPos.Add(this.fsmObjectParams.get_Count());
														this.paramByteDataSize.Add(0);
														this.fsmObjectParams.Add(obj as SkillObject);
														return;
													}
													if (object.ReferenceEquals(fieldType, typeof(SkillTexture)))
													{
														if (this.fsmObjectParams == null)
														{
															this.fsmObjectParams = new List<SkillObject>();
														}
														this.paramDataType.Add(ParamDataType.FsmTexture);
														this.paramDataPos.Add(this.fsmObjectParams.get_Count());
														this.paramByteDataSize.Add(0);
														this.fsmObjectParams.Add(obj as SkillObject);
														return;
													}
													if (fieldType.get_IsEnum())
													{
														this.paramDataType.Add(ParamDataType.Enum);
														this.AddByteData(SkillUtility.BitConverter.GetBytes((int)obj));
														return;
													}
													if (fieldType.get_IsClass())
													{
														if (this.customTypeSizes == null)
														{
															this.customTypeSizes = new List<int>();
															this.customTypeNames = new List<string>();
														}
														if (obj == null)
														{
															obj = Activator.CreateInstance(fieldType);
														}
														this.paramDataType.Add(ParamDataType.CustomClass);
														this.paramDataPos.Add(this.customTypeSizes.get_Count());
														this.customTypeNames.Add(fieldType.get_FullName());
														this.paramByteDataSize.Add(0);
														FieldInfo[] fields = ActionData.GetFields(fieldType);
														this.customTypeSizes.Add(fields.Length);
														FieldInfo[] array2 = fields;
														for (int i = 0; i < array2.Length; i++)
														{
															FieldInfo fieldInfo = array2[i];
															this.nextParamIndex++;
															this.paramName.Add(fieldInfo.get_Name());
															this.SaveActionField(fsm, fieldInfo.get_FieldType(), fieldInfo.GetValue(obj));
														}
														return;
													}
													if (obj != null)
													{
														Debug.LogError("Save Action: Unsupported parameter type: " + fieldType);
														this.paramDataType.Add(ParamDataType.Unsupported);
														this.paramDataPos.Add(this.byteData.get_Count());
														this.paramByteDataSize.Add(0);
														return;
													}
													this.paramDataType.Add(ParamDataType.Unsupported);
													this.paramDataPos.Add(this.byteData.get_Count());
													this.paramByteDataSize.Add(0);
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
		private void AddByteData(ICollection<byte> bytes)
		{
			this.paramDataPos.Add(this.byteData.get_Count());
			if (bytes != null)
			{
				this.paramByteDataSize.Add(bytes.get_Count());
				this.byteData.AddRange(bytes);
				return;
			}
			this.paramByteDataSize.Add(0);
		}
		private void SaveString(string str)
		{
			if (this.stringParams == null)
			{
				this.stringParams = new List<string>();
			}
			this.paramDataPos.Add(this.stringParams.get_Count());
			this.paramByteDataSize.Add(0);
			this.stringParams.Add(str ?? string.Empty);
		}
		private static ParamDataType GetParamDataType(Type type)
		{
			if (object.ReferenceEquals(type, typeof(SkillOwnerDefault)))
			{
				return ParamDataType.FsmOwnerDefault;
			}
			if (object.ReferenceEquals(type, typeof(SkillEventTarget)))
			{
				return ParamDataType.FsmEventTarget;
			}
			if (object.ReferenceEquals(type, typeof(SkillEvent)))
			{
				return ParamDataType.FsmEvent;
			}
			if (object.ReferenceEquals(type, typeof(SkillFloat)))
			{
				return ParamDataType.FsmFloat;
			}
			if (object.ReferenceEquals(type, typeof(SkillInt)))
			{
				return ParamDataType.FsmInt;
			}
			if (object.ReferenceEquals(type, typeof(SkillBool)))
			{
				return ParamDataType.FsmBool;
			}
			if (object.ReferenceEquals(type, typeof(SkillString)))
			{
				return ParamDataType.FsmString;
			}
			if (object.ReferenceEquals(type, typeof(SkillGameObject)))
			{
				return ParamDataType.FsmGameObject;
			}
			if (object.ReferenceEquals(type, typeof(FunctionCall)))
			{
				return ParamDataType.FunctionCall;
			}
			if (object.ReferenceEquals(type, typeof(SkillProperty)))
			{
				return ParamDataType.FsmProperty;
			}
			if (object.ReferenceEquals(type, typeof(SkillVector2)))
			{
				return ParamDataType.FsmVector2;
			}
			if (object.ReferenceEquals(type, typeof(SkillVector3)))
			{
				return ParamDataType.FsmVector3;
			}
			if (object.ReferenceEquals(type, typeof(SkillRect)))
			{
				return ParamDataType.FsmRect;
			}
			if (object.ReferenceEquals(type, typeof(SkillQuaternion)))
			{
				return ParamDataType.FsmQuaternion;
			}
			if (object.ReferenceEquals(type, typeof(SkillObject)))
			{
				return ParamDataType.FsmObject;
			}
			if (object.ReferenceEquals(type, typeof(SkillMaterial)))
			{
				return ParamDataType.FsmMaterial;
			}
			if (object.ReferenceEquals(type, typeof(SkillTexture)))
			{
				return ParamDataType.FsmTexture;
			}
			if (object.ReferenceEquals(type, typeof(SkillColor)))
			{
				return ParamDataType.FsmColor;
			}
			if (object.ReferenceEquals(type, typeof(int)))
			{
				return ParamDataType.Integer;
			}
			if (object.ReferenceEquals(type, typeof(bool)))
			{
				return ParamDataType.Boolean;
			}
			if (object.ReferenceEquals(type, typeof(float)))
			{
				return ParamDataType.Float;
			}
			if (object.ReferenceEquals(type, typeof(string)))
			{
				return ParamDataType.String;
			}
			if (object.ReferenceEquals(type, typeof(Color)))
			{
				return ParamDataType.Color;
			}
			if (object.ReferenceEquals(type, typeof(LayerMask)))
			{
				return ParamDataType.LayerMask;
			}
			if (object.ReferenceEquals(type, typeof(Vector2)))
			{
				return ParamDataType.Vector2;
			}
			if (object.ReferenceEquals(type, typeof(Vector3)))
			{
				return ParamDataType.Vector3;
			}
			if (object.ReferenceEquals(type, typeof(Vector4)))
			{
				return ParamDataType.Vector4;
			}
			if (object.ReferenceEquals(type, typeof(Quaternion)))
			{
				return ParamDataType.Quaternion;
			}
			if (object.ReferenceEquals(type, typeof(Rect)))
			{
				return ParamDataType.Rect;
			}
			if (object.ReferenceEquals(type, typeof(AnimationCurve)))
			{
				return ParamDataType.AnimationCurve;
			}
			if (object.ReferenceEquals(type, typeof(GameObject)))
			{
				return ParamDataType.GameObject;
			}
			if (object.ReferenceEquals(type, typeof(LayoutOption)))
			{
				return ParamDataType.LayoutOption;
			}
			if (object.ReferenceEquals(type, typeof(SkillVar)))
			{
				return ParamDataType.FsmVar;
			}
			if (object.ReferenceEquals(type, typeof(SkillEnum)))
			{
				return ParamDataType.FsmEnum;
			}
			if (object.ReferenceEquals(type, typeof(SkillArray)))
			{
				return ParamDataType.FsmArray;
			}
			if (object.ReferenceEquals(type, typeof(SkillTemplateControl)))
			{
				return ParamDataType.FsmTemplateControl;
			}
			if (object.ReferenceEquals(type, typeof(SkillAnimationCurve)))
			{
				return ParamDataType.FsmAnimationCurve;
			}
			if (type.get_IsArray())
			{
				return ParamDataType.Array;
			}
			if (type.IsSubclassOf(typeof(Object)))
			{
				return ParamDataType.ObjectReference;
			}
			if (type.get_IsEnum())
			{
				return ParamDataType.Enum;
			}
			if (type.get_IsClass())
			{
				return ParamDataType.CustomClass;
			}
			return ParamDataType.Unsupported;
		}
	}
}
