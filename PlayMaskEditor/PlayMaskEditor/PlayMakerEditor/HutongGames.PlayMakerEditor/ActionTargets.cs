using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public static class ActionTargets
	{
		private static readonly Dictionary<Type, List<ActionTarget>> lookup = new Dictionary<Type, List<ActionTarget>>();
		public static void Init()
		{
			ActionTargets.lookup.Clear();
			using (List<Type>.Enumerator enumerator = Actions.List.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					IEnumerable<Attribute> attributes = CustomAttributeHelpers.GetAttributes(current, typeof(ActionTarget));
					using (IEnumerator<Attribute> enumerator2 = attributes.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Attribute current2 = enumerator2.get_Current();
							ActionTargets.AddActionTarget(current, (ActionTarget)current2);
						}
					}
				}
			}
			using (List<Type>.Enumerator enumerator3 = Actions.List.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Type current3 = enumerator3.get_Current();
					if (!ActionTargets.HasNoActionTargetsAttribute(current3) && !ActionTargets.HasActionTargets(current3))
					{
						ActionTargets.GenerateActionTargets(current3);
					}
				}
			}
		}
		public static List<Type> GetActions()
		{
			List<Type> list = Enumerable.ToList<Type>(ActionTargets.lookup.get_Keys());
			list.Sort((Type a, Type b) => string.Compare(a.get_Name(), b.get_Name(), 5));
			return list;
		}
		public static List<Type> GetActionsSortedByCategory()
		{
			List<Type> list = Enumerable.ToList<Type>(ActionTargets.lookup.get_Keys());
			list.Sort((Type a, Type b) => string.Compare(Actions.GetCategory(a) + a.get_Name(), Actions.GetCategory(b) + b.get_Name(), 5));
			return list;
		}
		public static List<ActionTarget> GetActionTargets(Type actionType)
		{
			List<ActionTarget> result;
			if (!ActionTargets.lookup.TryGetValue(actionType, ref result))
			{
				return new List<ActionTarget>();
			}
			return result;
		}
		public static bool HasNoActionTargetsAttribute(Type actionType)
		{
			return CustomAttributeHelpers.HasAttribute<NoActionTargetsAttribute>(actionType);
		}
		public static bool HasActionTargets(Type actionType)
		{
			List<ActionTarget> list;
			return ActionTargets.lookup.TryGetValue(actionType, ref list) && list.get_Count() > 0;
		}
		public static bool HasActionTargetForType(Type actionType, Type targetType)
		{
			List<ActionTarget> list;
			return ActionTargets.lookup.TryGetValue(actionType, ref list) && Enumerable.Any<ActionTarget>(list, (ActionTarget actionTarget) => actionTarget.get_ObjectType() == targetType);
		}
		private static void AddActionTarget(Type actionType, ActionTarget actionTarget)
		{
			List<ActionTarget> list;
			if (ActionTargets.lookup.TryGetValue(actionType, ref list))
			{
				if (!ActionTargets.HasActionTargetForType(actionType, actionTarget.get_ObjectType()))
				{
					list.Add(actionTarget);
					return;
				}
			}
			else
			{
				List<ActionTarget> list2 = new List<ActionTarget>();
				list2.Add(actionTarget);
				list = list2;
				ActionTargets.lookup.Add(actionType, list);
			}
		}
		private static void GenerateActionTargets(Type actionType)
		{
			FieldInfo[] fields = actionType.GetFields(20);
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo field = array[i];
				if (!CustomAttributeHelpers.HasUIHint(field, 10))
				{
					ActionTargets.FindCheckForComponentAttribute(actionType, field);
					ActionTargets.FindObjectTypeAttribute(actionType, field);
					ActionTargets.FindMaterialParameters(actionType, field);
					ActionTargets.FindGameObjectParameters(actionType, field);
					ActionTargets.FindColliderParameters(actionType, field);
					ActionTargets.FindUIHintParameters(actionType, field);
				}
			}
		}
		private static void FindCheckForComponentAttribute(Type actionType, FieldInfo field)
		{
			CheckForComponentAttribute attribute = CustomAttributeHelpers.GetAttribute<CheckForComponentAttribute>(field);
			if (attribute != null)
			{
				if (attribute.get_Type0() != null)
				{
					ActionTargets.AddActionTarget(actionType, new ActionTarget(attribute.get_Type0(), field.get_Name(), false));
				}
				if (attribute.get_Type1() != null)
				{
					ActionTargets.AddActionTarget(actionType, new ActionTarget(attribute.get_Type1(), field.get_Name(), false));
				}
				if (attribute.get_Type2() != null)
				{
					ActionTargets.AddActionTarget(actionType, new ActionTarget(attribute.get_Type2(), field.get_Name(), false));
				}
			}
		}
		private static void FindObjectTypeAttribute(Type actionType, FieldInfo field)
		{
			ObjectTypeAttribute attribute = CustomAttributeHelpers.GetAttribute<ObjectTypeAttribute>(field);
			if (attribute == null || attribute.get_ObjectType() == null)
			{
				return;
			}
			ActionTargets.AddActionTarget(actionType, new ActionTarget(attribute.get_ObjectType(), field.get_Name(), false));
		}
		private static void FindMaterialParameters(Type actionType, FieldInfo field)
		{
			if (field.get_FieldType() == typeof(SkillMaterial) || field.get_FieldType() == typeof(Material))
			{
				ActionTargets.AddActionTarget(actionType, new ActionTarget(typeof(Material), field.get_Name(), false));
				return;
			}
			if (field.get_FieldType() == typeof(SkillTexture) || field.get_FieldType() == typeof(Texture))
			{
				ActionTargets.AddActionTarget(actionType, new ActionTarget(typeof(Texture), field.get_Name(), false));
			}
		}
		private static void FindGameObjectParameters(Type actionType, FieldInfo field)
		{
			if ((field.get_FieldType() == typeof(SkillOwnerDefault) || field.get_FieldType() == typeof(SkillGameObject) || field.get_FieldType() == typeof(GameObject)) && !CustomAttributeHelpers.HasAttribute<CheckForComponentAttribute>(field))
			{
				ActionTargets.AddActionTarget(actionType, new ActionTarget(typeof(GameObject), field.get_Name(), false));
			}
		}
		private static void FindColliderParameters(Type actionType, FieldInfo field)
		{
			Type fieldType = field.get_FieldType();
			if (fieldType == typeof(CollisionType) || fieldType == typeof(TriggerType))
			{
				ActionTargets.AddActionTarget(actionType, new ActionTarget(typeof(Collider), "", false));
				ActionTargets.AddActionTarget(actionType, new ActionTarget(typeof(Rigidbody), "", false));
				ActionTargets.AddActionTarget(actionType, new ActionTarget(typeof(GameObject), "", false));
				return;
			}
			if (fieldType == typeof(Collision2DType) || fieldType == typeof(Trigger2DType))
			{
				ActionTargets.AddActionTarget(actionType, new ActionTarget(typeof(Collider2D), "", false));
				ActionTargets.AddActionTarget(actionType, new ActionTarget(typeof(Rigidbody2D), "", false));
				ActionTargets.AddActionTarget(actionType, new ActionTarget(typeof(GameObject), "", false));
			}
		}
		private static void FindUIHintParameters(Type actionType, FieldInfo field)
		{
			IEnumerable<UIHintAttribute> attributes = CustomAttributeHelpers.GetAttributes<UIHintAttribute>(field);
			using (IEnumerator<UIHintAttribute> enumerator = attributes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					UIHintAttribute current = enumerator.get_Current();
					UIHint hint = current.get_Hint();
					UIHint uIHint = hint;
					if (uIHint == 6)
					{
						ActionTargets.AddActionTarget(actionType, new ActionTarget(typeof(AnimationClip), field.get_Name(), false));
					}
				}
			}
		}
	}
}
