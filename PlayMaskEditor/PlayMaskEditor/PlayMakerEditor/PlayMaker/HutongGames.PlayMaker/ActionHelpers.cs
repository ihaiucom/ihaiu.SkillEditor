using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	public static class ActionHelpers
	{
		private static Texture2D whiteTexture;
		public static RaycastHit mousePickInfo;
		private static float mousePickRaycastTime;
		private static float mousePickDistanceUsed;
		private static int mousePickLayerMaskUsed;
		public static Texture2D WhiteTexture
		{
			get
			{
				if (ActionHelpers.whiteTexture == null)
				{
					ActionHelpers.whiteTexture = new Texture2D(1, 1);
					ActionHelpers.whiteTexture.SetPixel(0, 0, Color.get_white());
					ActionHelpers.whiteTexture.Apply();
				}
				return ActionHelpers.whiteTexture;
			}
		}
		public static bool IsVisible(GameObject go)
		{
			if (go == null)
			{
				return false;
			}
			Renderer component = go.GetComponent<Renderer>();
			return component != null && component.get_isVisible();
		}
		[Obsolete("Use LogError instead.")]
		public static void RuntimeError(SkillStateAction action, string error)
		{
			action.LogError(action + " : " + error);
		}
		public static PlayMakerFSM GetGameObjectFsm(GameObject go, string fsmName)
		{
			if (!string.IsNullOrEmpty(fsmName))
			{
				PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
				PlayMakerFSM[] array = components;
				for (int i = 0; i < array.Length; i++)
				{
					PlayMakerFSM playMakerFSM = array[i];
					if (playMakerFSM.FsmName == fsmName)
					{
						return playMakerFSM;
					}
				}
				Debug.LogWarning("Could not find FSM: " + fsmName);
			}
			return go.GetComponent<PlayMakerFSM>();
		}
		public static int GetRandomWeightedIndex(SkillFloat[] weights)
		{
			float num = 0f;
			for (int i = 0; i < weights.Length; i++)
			{
				SkillFloat fsmFloat = weights[i];
				num += fsmFloat.Value;
			}
			float num2 = Random.Range(0f, num);
			for (int j = 0; j < weights.Length; j++)
			{
				if (num2 < weights[j].Value)
				{
					return j;
				}
				num2 -= weights[j].Value;
			}
			return -1;
		}
		public static bool HasAnimationFinished(AnimationState anim, float prevTime, float currentTime)
		{
			return anim.get_wrapMode() != 2 && anim.get_wrapMode() != 4 && (((anim.get_wrapMode() == null || anim.get_wrapMode() == 1) && prevTime > 0f && currentTime.Equals(0f)) || (prevTime < anim.get_length() && currentTime >= anim.get_length()));
		}
		public static Vector3 GetPosition(SkillGameObject fsmGameObject, SkillVector3 fsmVector3)
		{
			Vector3 result;
			if (fsmGameObject.Value != null)
			{
				result = ((!fsmVector3.IsNone) ? fsmGameObject.Value.get_transform().TransformPoint(fsmVector3.Value) : fsmGameObject.Value.get_transform().get_position());
			}
			else
			{
				result = fsmVector3.Value;
			}
			return result;
		}
		public static bool IsMouseOver(GameObject gameObject, float distance, int layerMask)
		{
			return !(gameObject == null) && gameObject == ActionHelpers.MouseOver(distance, layerMask);
		}
		public static RaycastHit MousePick(float distance, int layerMask)
		{
			if (!ActionHelpers.mousePickRaycastTime.Equals((float)Time.get_frameCount()) || ActionHelpers.mousePickDistanceUsed < distance || ActionHelpers.mousePickLayerMaskUsed != layerMask)
			{
				ActionHelpers.DoMousePick(distance, layerMask);
			}
			return ActionHelpers.mousePickInfo;
		}
		public static GameObject MouseOver(float distance, int layerMask)
		{
			if (!ActionHelpers.mousePickRaycastTime.Equals((float)Time.get_frameCount()) || ActionHelpers.mousePickDistanceUsed < distance || ActionHelpers.mousePickLayerMaskUsed != layerMask)
			{
				ActionHelpers.DoMousePick(distance, layerMask);
			}
			if (ActionHelpers.mousePickInfo.get_collider() != null && ActionHelpers.mousePickInfo.get_distance() < distance)
			{
				return ActionHelpers.mousePickInfo.get_collider().get_gameObject();
			}
			return null;
		}
		private static void DoMousePick(float distance, int layerMask)
		{
			if (Camera.get_main() == null)
			{
				return;
			}
			Ray ray = Camera.get_main().ScreenPointToRay(Input.get_mousePosition());
			Physics.Raycast(ray, ref ActionHelpers.mousePickInfo, distance, layerMask);
			ActionHelpers.mousePickLayerMaskUsed = layerMask;
			ActionHelpers.mousePickDistanceUsed = distance;
			ActionHelpers.mousePickRaycastTime = (float)Time.get_frameCount();
		}
		public static int LayerArrayToLayerMask(SkillInt[] layers, bool invert)
		{
			int num = 0;
			for (int i = 0; i < layers.Length; i++)
			{
				SkillInt fsmInt = layers[i];
				num |= 1 << fsmInt.Value;
			}
			if (invert)
			{
				num = ~num;
			}
			if (num != 0)
			{
				return num;
			}
			return -5;
		}
		public static bool IsLoopingWrapMode(WrapMode wrapMode)
		{
			return wrapMode == 2 || wrapMode == 4;
		}
		public static string CheckRayDistance(float rayDistance)
		{
			if (rayDistance > 0f)
			{
				return "";
			}
			return "Ray Distance should be greater than zero!\n";
		}
		public static string CheckForValidEvent(SkillState state, string eventName)
		{
			if (state == null)
			{
				return "Invalid State!";
			}
			if (string.IsNullOrEmpty(eventName))
			{
				return "";
			}
			SkillTransition[] globalTransitions = state.Fsm.GlobalTransitions;
			for (int i = 0; i < globalTransitions.Length; i++)
			{
				SkillTransition fsmTransition = globalTransitions[i];
				if (fsmTransition.EventName == eventName)
				{
					string result = "";
					return result;
				}
			}
			SkillTransition[] transitions = state.Transitions;
			for (int j = 0; j < transitions.Length; j++)
			{
				SkillTransition fsmTransition2 = transitions[j];
				if (fsmTransition2.EventName == eventName)
				{
					string result = "";
					return result;
				}
			}
			return "Fsm will not respond to Event: " + eventName;
		}
		public static string CheckPhysicsSetup(SkillOwnerDefault ownerDefault)
		{
			if (ownerDefault == null)
			{
				return "";
			}
			if (ownerDefault.OwnerOption != OwnerDefaultOption.UseOwner)
			{
				return ActionHelpers.CheckPhysicsSetup(ownerDefault.GameObject.Value);
			}
			return ActionHelpers.CheckOwnerPhysicsSetup(ownerDefault.GameObject.Value);
		}
		public static string CheckOwnerPhysicsSetup(GameObject gameObject)
		{
			string text = string.Empty;
			if (gameObject != null && gameObject.GetComponent<Collider>() == null && gameObject.GetComponent<Rigidbody>() == null)
			{
				text += "Owner requires a RigidBody or Collider component!\n";
			}
			return text;
		}
		public static string CheckOwnerPhysics2dSetup(GameObject gameObject)
		{
			string text = string.Empty;
			if (gameObject != null && gameObject.GetComponent<Collider2D>() == null && gameObject.GetComponent<Rigidbody2D>() == null)
			{
				text += "Owner requires a RigidBody2D or Collider2D component!\n";
			}
			return text;
		}
		public static string CheckPhysicsSetup(GameObject gameObject)
		{
			string text = string.Empty;
			if (gameObject != null && gameObject.GetComponent<Collider>() == null && gameObject.GetComponent<Rigidbody>() == null)
			{
				text += "GameObject requires RigidBody/Collider!\n";
			}
			return text;
		}
		public static void DebugLog(Skill fsm, LogLevel logLevel, string text, bool sendToUnityLog = false)
		{
			if (!SkillLog.LoggingEnabled || fsm == null)
			{
				return;
			}
			switch (logLevel)
			{
			case LogLevel.Info:
				fsm.MyLog.LogAction(SkillLogType.Info, text, sendToUnityLog);
				return;
			case LogLevel.Warning:
				fsm.MyLog.LogAction(SkillLogType.Warning, text, sendToUnityLog);
				return;
			case LogLevel.Error:
				fsm.MyLog.LogAction(SkillLogType.Error, text, sendToUnityLog);
				return;
			default:
				return;
			}
		}
		public static void LogError(string text)
		{
			ActionHelpers.DebugLog(SkillExecutionStack.ExecutingFsm, LogLevel.Error, text, true);
		}
		public static void LogWarning(string text)
		{
			ActionHelpers.DebugLog(SkillExecutionStack.ExecutingFsm, LogLevel.Warning, text, true);
		}
		public static string GetValueLabel(INamedVariable variable)
		{
			if (variable == null)
			{
				return "[null]";
			}
			if (variable.IsNone)
			{
				return "[none]";
			}
			if (variable.UseVariable)
			{
				return variable.Name;
			}
			object rawValue = variable.RawValue;
			if (object.ReferenceEquals(rawValue, null))
			{
				return "null";
			}
			if (rawValue is string)
			{
				return "\"" + rawValue + "\"";
			}
			if (rawValue is Array)
			{
				return "Array";
			}
			return variable.RawValue.ToString();
		}
		public static string GetValueLabel(Skill fsm, SkillOwnerDefault ownerDefault)
		{
			if (ownerDefault == null)
			{
				return "[null]";
			}
			if (ownerDefault.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				return "Owner";
			}
			return ActionHelpers.GetValueLabel(ownerDefault.GameObject);
		}
		public static void AddAnimationClip(GameObject go, AnimationClip animClip)
		{
			if (animClip == null)
			{
				return;
			}
			Animation component = go.GetComponent<Animation>();
			if (component != null)
			{
				component.AddClip(animClip, animClip.get_name());
			}
		}
	}
}
