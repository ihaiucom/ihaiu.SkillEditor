using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace HutongGames.PlayMaker
{
	public static class ReflectionUtils
	{
		private static List<string> assemblyNames;
		private static Assembly[] loadedAssemblies;
		private static readonly Dictionary<string, Type> typeLookup = new Dictionary<string, Type>();
		public static Assembly[] GetLoadedAssemblies()
		{
			return AppDomain.get_CurrentDomain().GetAssemblies();
		}
		[Localizable(false)]
		public static Type GetGlobalType(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
			{
				return null;
			}
			Type type;
			ReflectionUtils.typeLookup.TryGetValue(typeName, ref type);
			if (!object.ReferenceEquals(type, null))
			{
				return type;
			}
			type = (Type.GetType(typeName + ",Assembly-CSharp") ?? Type.GetType(typeName + ",PlayMaker"));
			if (object.ReferenceEquals(type, null))
			{
				type = (Type.GetType(typeName + ",Assembly-CSharp-firstpass") ?? Type.GetType(typeName));
			}
			if (object.ReferenceEquals(type, null))
			{
				if (ReflectionUtils.assemblyNames == null)
				{
					ReflectionUtils.assemblyNames = new List<string>();
					ReflectionUtils.loadedAssemblies = AppDomain.get_CurrentDomain().GetAssemblies();
					Assembly[] array = ReflectionUtils.loadedAssemblies;
					for (int i = 0; i < array.Length; i++)
					{
						Assembly assembly = array[i];
						ReflectionUtils.assemblyNames.Add(assembly.get_FullName());
					}
				}
				using (List<string>.Enumerator enumerator = ReflectionUtils.assemblyNames.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.get_Current();
						type = Type.GetType(typeName + "," + current);
						if (!object.ReferenceEquals(type, null))
						{
							break;
						}
					}
				}
				if (object.ReferenceEquals(type, null))
				{
					for (int j = 0; j < ReflectionUtils.loadedAssemblies.Length; j++)
					{
						Type[] types = ReflectionUtils.loadedAssemblies[j].GetTypes();
						for (int k = 0; k < types.Length; k++)
						{
							if (types[k].get_Name() == typeName && (types[k].get_Namespace() == "UnityEngine" || types[k].get_Namespace() == "HutongGames.PlayMaker" || types[k].get_Namespace() == "HutongGames.PlayMaker.Actions"))
							{
								type = types[k];
								ReflectionUtils.typeLookup.set_Item(typeName, type);
								return type;
							}
						}
					}
				}
			}
			ReflectionUtils.typeLookup.Remove(typeName);
			ReflectionUtils.typeLookup.set_Item(typeName, type);
			return type;
		}
		public static Type GetPropertyType(Type type, string path)
		{
			string[] array = path.Split(new char[]
			{
				'.'
			});
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				PropertyInfo property = type.GetProperty(text);
				if (!object.ReferenceEquals(property, null))
				{
					type = property.get_PropertyType();
				}
				else
				{
					FieldInfo field = type.GetField(text);
					if (object.ReferenceEquals(field, null))
					{
						return null;
					}
					type = field.get_FieldType();
				}
			}
			return type;
		}
		public static MemberInfo[] GetMemberInfo(Type type, string path)
		{
			if (object.ReferenceEquals(type, null))
			{
				return null;
			}
			string[] array = path.Split(new char[]
			{
				'.'
			});
			MemberInfo[] array2 = new MemberInfo[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				PropertyInfo property = type.GetProperty(text);
				if (!object.ReferenceEquals(property, null))
				{
					array2[i] = property;
					type = property.get_PropertyType();
				}
				else
				{
					FieldInfo field = type.GetField(text);
					if (object.ReferenceEquals(field, null))
					{
						return null;
					}
					array2[i] = field;
					type = field.get_FieldType();
				}
			}
			return array2;
		}
		public static bool CanReadMemberValue(MemberInfo member)
		{
			MemberTypes memberType = member.get_MemberType();
			return memberType == 4 || (memberType == 16 && ((PropertyInfo)member).get_CanRead());
		}
		public static bool CanSetMemberValue(MemberInfo member)
		{
			MemberTypes memberType = member.get_MemberType();
			return memberType == 4 || (memberType == 16 && ((PropertyInfo)member).get_CanWrite());
		}
		public static bool CanGetMemberValue(MemberInfo member)
		{
			MemberTypes memberType = member.get_MemberType();
			return memberType == 4 || (memberType == 16 && ((PropertyInfo)member).get_CanRead());
		}
		public static Type GetMemberUnderlyingType(MemberInfo member)
		{
			MemberTypes memberType = member.get_MemberType();
			switch (memberType)
			{
			case 2:
				return ((EventInfo)member).get_EventHandlerType();
			case 3:
				break;
			case 4:
				return ((FieldInfo)member).get_FieldType();
			default:
				if (memberType == 16)
				{
					return ((PropertyInfo)member).get_PropertyType();
				}
				break;
			}
			throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo or EventInfo", "member");
		}
		public static object GetMemberValue(MemberInfo[] memberInfo, object target)
		{
			for (int i = 0; i < memberInfo.Length; i++)
			{
				target = ReflectionUtils.GetMemberValue(memberInfo[i], target);
			}
			return target;
		}
		public static object GetMemberValue(MemberInfo member, object target)
		{
			MemberTypes memberType = member.get_MemberType();
			if (memberType != 4)
			{
				if (memberType == 16)
				{
					try
					{
						return ((PropertyInfo)member).GetValue(target, null);
					}
					catch (TargetParameterCountException ex)
					{
						throw new ArgumentException("MemberInfo has index parameters", "member", ex);
					}
				}
				throw new ArgumentException("MemberInfo is not of type FieldInfo or PropertyInfo", "member");
			}
			return ((FieldInfo)member).GetValue(target);
		}
		public static void SetMemberValue(MemberInfo member, object target, object value)
		{
			MemberTypes memberType = member.get_MemberType();
			if (memberType == 4)
			{
				((FieldInfo)member).SetValue(target, value);
				return;
			}
			if (memberType != 16)
			{
				throw new ArgumentException("MemberInfo must be if type FieldInfo or PropertyInfo", "member");
			}
			((PropertyInfo)member).SetValue(target, value, null);
		}
		public static void SetMemberValue(MemberInfo[] memberInfo, object target, object value)
		{
			object parent = null;
			MemberInfo targetInfo = null;
			for (int i = 0; i < memberInfo.Length - 1; i++)
			{
				parent = target;
				targetInfo = memberInfo[i];
				target = ReflectionUtils.GetMemberValue(memberInfo[i], target);
			}
			if (target.GetType().get_IsValueType())
			{
				ReflectionUtils.SetBoxedMemberValue(parent, targetInfo, target, memberInfo[memberInfo.Length - 1], value);
				return;
			}
			ReflectionUtils.SetMemberValue(memberInfo[memberInfo.Length - 1], target, value);
		}
		public static void SetBoxedMemberValue(object parent, MemberInfo targetInfo, object target, MemberInfo propertyInfo, object value)
		{
			ReflectionUtils.SetMemberValue(propertyInfo, target, value);
			ReflectionUtils.SetMemberValue(targetInfo, parent, target);
		}
		public static List<MemberInfo> GetFieldsAndProperties<T>(BindingFlags bindingAttr)
		{
			return ReflectionUtils.GetFieldsAndProperties(typeof(T), bindingAttr);
		}
		public static List<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr)
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.AddRange(type.GetFields(bindingAttr));
			list.AddRange(type.GetProperties(bindingAttr));
			return list;
		}
		public static FieldInfo[] GetPublicFields(this Type type)
		{
			return type.GetFields(20);
		}
		public static PropertyInfo[] GetPublicProperties(this Type type)
		{
			return type.GetProperties(20);
		}
	}
}
