using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public static class TypeHelpers
	{
		private static readonly Dictionary<Type, string> friendlyNames;
		private static List<Type> objectTypeList;
		private static List<Type> enumTypeList;
		private static SkillProperty targetFsmProperty;
		private static SkillVariable targetFsmVariable;
		private static NamedVariable targetVariable;
		private static string subPropertyPath;
		private static readonly string[] filterGameObjectMembers;
		public static List<Type> ObjectTypeList
		{
			get
			{
				List<Type> arg_1F_0;
				if ((arg_1F_0 = TypeHelpers.objectTypeList) == null)
				{
					arg_1F_0 = (TypeHelpers.objectTypeList = TypeHelpers.GetDerivedTypeList(typeof(Object), true));
				}
				return arg_1F_0;
			}
		}
		public static List<Type> EnumTypeList
		{
			get
			{
				List<Type> arg_1F_0;
				if ((arg_1F_0 = TypeHelpers.enumTypeList) == null)
				{
					arg_1F_0 = (TypeHelpers.enumTypeList = TypeHelpers.GetDerivedTypeList(typeof(Enum), false));
				}
				return arg_1F_0;
			}
		}
		static TypeHelpers()
		{
			TypeHelpers.friendlyNames = new Dictionary<Type, string>();
			TypeHelpers.filterGameObjectMembers = new string[]
			{
				"animation",
				"audio",
				"camera",
				"collider",
				"collider2D",
				"constantForce",
				"gameObject",
				"guiElement",
				"guiText",
				"guiTexture",
				"hingeJoint",
				"light",
				"networkView",
				"particleEmitter",
				"particleSystem",
				"renderer",
				"rigidbody",
				"rigidbody2D"
			};
			TypeHelpers.friendlyNames.Clear();
			TypeHelpers.friendlyNames.Add(typeof(void), "void");
			TypeHelpers.friendlyNames.Add(typeof(bool), "bool");
			TypeHelpers.friendlyNames.Add(typeof(int), "int");
			TypeHelpers.friendlyNames.Add(typeof(float), "float");
			TypeHelpers.friendlyNames.Add(typeof(string), "string");
		}
		public static string GetFriendlyName(Type t)
		{
			if (t == null)
			{
				return "";
			}
			if (t.get_IsArray())
			{
				return TypeHelpers.GetFriendlyName(t.GetElementType()) + "[]";
			}
			string result;
			if (!TypeHelpers.friendlyNames.TryGetValue(t, ref result))
			{
				return t.get_Name();
			}
			return result;
		}
		public static List<Type> GetDerivedTypeList(Type ofType, bool includeBaseType = true)
		{
			List<Type> list = new List<Type>();
			if (includeBaseType)
			{
				list.Add(ofType);
			}
			Assembly[] assemblies = AppDomain.get_CurrentDomain().GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				if (!TypeHelpers.IsEditorAssembly(assembly))
				{
					try
					{
						Type[] exportedTypes = assembly.GetExportedTypes();
						Type[] array = exportedTypes;
						for (int j = 0; j < array.Length; j++)
						{
							Type type = array[j];
							if (!TypeHelpers.IsEditorType(type) && type.IsSubclassOf(ofType))
							{
								list.Add(type);
							}
						}
					}
					catch (Exception ex)
					{
						NotSupportedException arg_75_0 = ex as NotSupportedException;
					}
				}
			}
			list.Sort((Type o1, Type o2) => string.CompareOrdinal(o1.ToString(), o2.ToString()));
			return list;
		}
		private static bool IsEditorType(Type type)
		{
			return type.IsSubclassOf(typeof(Editor)) || type.IsSubclassOf(typeof(EditorWindow));
		}
		[Localizable(false)]
		private static bool IsEditorAssembly(Assembly assembly)
		{
			string name = assembly.GetName().get_Name();
			return name == "UnityEditor" || name == "PlayMakerEditor";
		}
		private static void RebuildTypeList()
		{
			TypeHelpers.objectTypeList = null;
		}
		public static IEnumerable<FieldInfo> GetSerializedFields(Type type)
		{
			return Enumerable.Where<FieldInfo>(Enumerable.Where<FieldInfo>(Enumerable.Where<FieldInfo>(type.GetFields(52), (FieldInfo f) => f.get_IsPublic() || f.IsDefined(typeof(SerializeField), false)), (FieldInfo f) => f.get_FieldType() != typeof(Skill) && f.get_FieldType() != typeof(SkillTemplate)), (FieldInfo f) => !f.get_FieldType().IsSubclassOf(typeof(Object)));
		}
		public static GenericMenu GenerateMethodMenu(Type type, GenericMenu.MenuFunction2 selectionFunction)
		{
			return TypeHelpers.GenerateMethodMenu(type, selectionFunction, 20);
		}
		public static GenericMenu GenerateStaticMethodMenu(Type type, GenericMenu.MenuFunction2 selectionFunction)
		{
			return TypeHelpers.GenerateMethodMenu(type, selectionFunction, 24);
		}
		public static GenericMenu GenerateMethodMenu(Type type, GenericMenu.MenuFunction2 selectionFunction, BindingFlags bindingFlags)
		{
			GenericMenu genericMenu = new GenericMenu();
			if (type == null)
			{
				return genericMenu;
			}
			MethodInfo[] methods = type.GetMethods(bindingFlags);
			MethodInfo[] array = methods;
			for (int i = 0; i < array.Length; i++)
			{
				MethodInfo methodInfo = array[i];
				if (TypeHelpers.IsValidMethod(methodInfo))
				{
					string text = TypeHelpers.GetMethodSignature(methodInfo);
					if (methodInfo.get_DeclaringType() == typeof(MonoBehaviour) || methodInfo.get_DeclaringType() == typeof(Component) || methodInfo.get_DeclaringType() == typeof(Behaviour) || methodInfo.get_DeclaringType() == typeof(Object))
					{
						text = "Inherited/" + text;
					}
					genericMenu.AddItem(new GUIContent(text), false, selectionFunction, methodInfo);
				}
			}
			return genericMenu;
		}
		public static string GetMethodSignature(MethodInfo method)
		{
			string text = TypeHelpers.GetFriendlyName(method.get_ReturnType()) + " " + method.get_Name() + " (";
			bool flag = true;
			ParameterInfo[] parameters = method.GetParameters();
			ParameterInfo[] array = parameters;
			for (int i = 0; i < array.Length; i++)
			{
				ParameterInfo parameterInfo = array[i];
				if (!flag)
				{
					text += ", ";
				}
				text += TypeHelpers.GetFriendlyName(parameterInfo.get_ParameterType());
				flag = false;
			}
			return text + ")";
		}
		public static string GetMethodSignature(string methodName, SkillVar[] parameters, SkillVar result)
		{
			if (parameters == null || result == null)
			{
				return "";
			}
			Type t = result.get_RealType() ?? typeof(void);
			string text = TypeHelpers.GetFriendlyName(t) + " " + methodName + " (";
			bool flag = true;
			for (int i = 0; i < parameters.Length; i++)
			{
				SkillVar fsmVar = parameters[i];
				if (!flag)
				{
					text += ", ";
				}
				text += TypeHelpers.GetFriendlyName(fsmVar.get_RealType());
				flag = false;
			}
			return text + ")";
		}
		public static MethodInfo FindMethod(Type type, string methodSignature)
		{
			if (type == null)
			{
				return null;
			}
			MethodInfo[] methods = type.GetMethods(20);
			MethodInfo[] array = methods;
			for (int i = 0; i < array.Length; i++)
			{
				MethodInfo methodInfo = array[i];
				if (!methodInfo.get_IsSpecialName() && string.Equals(methodSignature, TypeHelpers.GetMethodSignature(methodInfo), 5))
				{
					return methodInfo;
				}
			}
			return null;
		}
		private static bool IsValidMethod(MethodInfo method)
		{
			if (method.get_IsGenericMethod() || method.get_IsSpecialName())
			{
				return false;
			}
			if (method.get_ReturnType() != typeof(void) && !TypeHelpers.IsSupportedParameterType(method.get_ReturnType()))
			{
				return false;
			}
			ParameterInfo[] parameters = method.GetParameters();
			ParameterInfo[] array = parameters;
			for (int i = 0; i < array.Length; i++)
			{
				ParameterInfo parameterInfo = array[i];
				if (!TypeHelpers.IsSupportedParameterType(parameterInfo.get_ParameterType()))
				{
					return false;
				}
			}
			return true;
		}
		public static GenericMenu GenerateObjectTypesMenu(SkillProperty fsmProperty)
		{
			TypeHelpers.RebuildTypeList();
			TypeHelpers.targetFsmProperty = fsmProperty;
			GenericMenu genericMenu = new GenericMenu();
			using (List<Type>.Enumerator enumerator = TypeHelpers.ObjectTypeList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					string fullName = current.get_FullName();
					if (fullName != null)
					{
						string text = fullName.Replace('.', '/');
						genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(TypeHelpers.SetFsmPropertyTargetType), fullName);
					}
				}
			}
			return genericMenu;
		}
		private static void SetFsmPropertyTargetType(object userdata)
		{
			if (TypeHelpers.targetFsmProperty != null)
			{
				TypeHelpers.targetFsmProperty.TargetTypeName = (userdata as string);
				SkillEditor.SetFsmDirty(true, false);
				SkillEditor.SaveActions();
			}
		}
		public static GenericMenu GenerateObjectTypesMenu(SkillVariable fsmVariable)
		{
			TypeHelpers.targetFsmVariable = fsmVariable;
			GenericMenu genericMenu = new GenericMenu();
			using (List<Type>.Enumerator enumerator = TypeHelpers.ObjectTypeList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					string fullName = current.get_FullName();
					string text = fullName.Replace('.', '/');
					genericMenu.AddItem(new GUIContent(text), fullName == fsmVariable.TypeName, new GenericMenu.MenuFunction2(TypeHelpers.SetFsmVariableObjectType), fullName);
				}
			}
			return genericMenu;
		}
		public static GenericMenu GenerateObjectTypesMenu(NamedVariable variable)
		{
			TypeHelpers.targetVariable = variable;
			GenericMenu genericMenu = new GenericMenu();
			using (List<Type>.Enumerator enumerator = TypeHelpers.ObjectTypeList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					string fullName = current.get_FullName();
					string text = fullName.Replace('.', '/');
					genericMenu.AddItem(new GUIContent(text), fullName == variable.get_ObjectType().get_Name(), new GenericMenu.MenuFunction2(TypeHelpers.SetVariableObjectType), fullName);
				}
			}
			return genericMenu;
		}
		private static void SetVariableObjectType(object userdata)
		{
			if (TypeHelpers.targetVariable != null)
			{
				TypeHelpers.targetVariable.set_ObjectType(ReflectionUtils.GetGlobalType(userdata as string));
				SkillEditor.SetFsmDirty(true, false);
				SkillEditor.SaveActions();
			}
		}
		private static void SetFsmVariableObjectType(object userdata)
		{
			if (TypeHelpers.targetFsmVariable != null)
			{
				TypeHelpers.targetFsmVariable.TypeName = (userdata as string);
				TypeHelpers.targetFsmVariable.ObjectType = ReflectionUtils.GetGlobalType(TypeHelpers.targetFsmVariable.TypeName);
				TypeHelpers.targetFsmVariable.UpdateVariableValue();
				SkillEditor.SetFsmDirty(true, false);
				SkillEditor.SaveActions();
			}
		}
		public static GenericMenu GenerateEnumTypesMenu(SkillVariable fsmVariable)
		{
			TypeHelpers.targetFsmVariable = fsmVariable;
			GenericMenu genericMenu = new GenericMenu();
			using (List<Type>.Enumerator enumerator = TypeHelpers.EnumTypeList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					string fullName = current.get_FullName();
					string text = fullName.Replace('.', '/');
					genericMenu.AddItem(new GUIContent(text), fullName == fsmVariable.TypeName, new GenericMenu.MenuFunction2(TypeHelpers.SetFsmVariableObjectType), fullName);
				}
			}
			return genericMenu;
		}
		public static GenericMenu GenerateEnumTypesMenu(NamedVariable variable)
		{
			TypeHelpers.targetVariable = variable;
			GenericMenu genericMenu = new GenericMenu();
			using (List<Type>.Enumerator enumerator = TypeHelpers.EnumTypeList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					string fullName = current.get_FullName();
					string text = fullName.Replace('.', '/');
					genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(TypeHelpers.SetVariableObjectType), fullName);
				}
			}
			return genericMenu;
		}
		public static GenericMenu GeneratePropertyMenu(SkillProperty fsmProperty)
		{
			TypeHelpers.targetFsmProperty = fsmProperty;
			GenericMenu result = new GenericMenu();
			Type globalType = ReflectionUtils.GetGlobalType(fsmProperty.TargetTypeName);
			if (globalType != null)
			{
				TypeHelpers.AddPropertyMenuItems(ref result, globalType, "", 0, fsmProperty.setProperty);
			}
			return result;
		}
		private static void GenerateSubPropertyMenu(object userdata)
		{
			TypeHelpers.subPropertyPath = (userdata as string);
			SkillEditor.OnRepaint = (SkillEditor.RepaintDelegate)Delegate.Combine(SkillEditor.OnRepaint, new SkillEditor.RepaintDelegate(TypeHelpers.OpenSubPropertyMenu));
		}
		private static void OpenSubPropertyMenu()
		{
			SkillEditor.OnRepaint = (SkillEditor.RepaintDelegate)Delegate.Remove(SkillEditor.OnRepaint, new SkillEditor.RepaintDelegate(TypeHelpers.OpenSubPropertyMenu));
			string text = TypeHelpers.subPropertyPath;
			SkillProperty fsmProperty = TypeHelpers.targetFsmProperty;
			GenericMenu genericMenu = new GenericMenu();
			Type globalType = ReflectionUtils.GetGlobalType(fsmProperty.TargetTypeName);
			if (globalType != null)
			{
				string path = TypeHelpers.NicifyPropertyPath(text);
				Type propertyType = ReflectionUtils.GetPropertyType(globalType, text);
				TypeHelpers.AddPropertyMenuItems(ref genericMenu, propertyType, path, 0, fsmProperty.setProperty);
			}
			TypeHelpers.subPropertyPath = "";
			genericMenu.ShowAsContext();
		}
		public static bool IsSupportedParameterType(Type parameterType)
		{
			while (parameterType != null)
			{
				if (!parameterType.get_IsArray())
				{
					return parameterType == typeof(string) || parameterType == typeof(int) || parameterType == typeof(float) || parameterType == typeof(Vector2) || parameterType == typeof(Vector3) || parameterType == typeof(Color) || parameterType == typeof(bool) || parameterType == typeof(Quaternion) || parameterType == typeof(Material) || parameterType == typeof(Texture) || parameterType == typeof(Rect) || parameterType.get_IsEnum() || parameterType.IsSubclassOf(typeof(Object));
				}
				parameterType = parameterType.GetElementType();
			}
			return false;
		}
		private static void AddPropertyMenuItems(ref GenericMenu menu, Type type, string path, int depth, bool setProperty)
		{
			if (type != null && !type.get_IsEnum() && depth < 3)
			{
				List<MemberInfo> fieldsAndProperties = ReflectionUtils.GetFieldsAndProperties(type, 20);
				fieldsAndProperties.Sort((MemberInfo x, MemberInfo y) => string.CompareOrdinal(x.get_Name(), y.get_Name()));
				using (List<MemberInfo>.Enumerator enumerator = fieldsAndProperties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MemberInfo current = enumerator.get_Current();
						if (!TypeHelpers.FilterMember(current))
						{
							Type memberUnderlyingType = ReflectionUtils.GetMemberUnderlyingType(current);
							string name = current.get_Name();
							bool flag = memberUnderlyingType.get_IsClass() || (setProperty ? TypeHelpers.CanSetProperty(current) : TypeHelpers.CanGetProperty(current));
							if (flag)
							{
								string text = (path != "") ? (path + '/' + name) : name;
								if (TypeHelpers.HasProperties(memberUnderlyingType))
								{
									if (TypeHelpers.CanSetProperty(current))
									{
										menu.AddItem(new GUIContent(text + '/' + Labels.StripNamespace(memberUnderlyingType.ToString())), false, new GenericMenu.MenuFunction2(TypeHelpers.SetFsmPropertyName), text);
									}
								}
								else
								{
									menu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(TypeHelpers.SetFsmPropertyName), text);
								}
							}
							if (string.IsNullOrEmpty(TypeHelpers.subPropertyPath) && depth > 0 && memberUnderlyingType.IsSubclassOf(typeof(Component)))
							{
								menu.AddItem(new GUIContent(path + Strings.get_SubMenu_More_()), false, new GenericMenu.MenuFunction2(TypeHelpers.GenerateSubPropertyMenu), path);
								break;
							}
							if (flag || memberUnderlyingType.IsSubclassOf(typeof(Component)))
							{
								TypeHelpers.AddPropertyMenuItems(ref menu, memberUnderlyingType, (path != "") ? (path + '/' + name) : name, depth + 1, setProperty);
							}
						}
					}
				}
			}
		}
		private static bool FilterMember(MemberInfo memberInfo)
		{
			if (memberInfo.get_DeclaringType() == typeof(GameObject) && Enumerable.Contains<string>(TypeHelpers.filterGameObjectMembers, memberInfo.get_Name()))
			{
				return true;
			}
			if (memberInfo.get_DeclaringType() == typeof(Component))
			{
				return true;
			}
			Type memberUnderlyingType = ReflectionUtils.GetMemberUnderlyingType(memberInfo);
			return memberInfo.get_DeclaringType() == memberUnderlyingType || memberUnderlyingType == typeof(Matrix4x4);
		}
		private static bool CanSetProperty(MemberInfo member)
		{
			if (!ReflectionUtils.CanSetMemberValue(member))
			{
				return false;
			}
			Type memberUnderlyingType = ReflectionUtils.GetMemberUnderlyingType(member);
			return TypeHelpers.IsSupportedParameterType(memberUnderlyingType);
		}
		private static bool CanGetProperty(MemberInfo member)
		{
			if (!ReflectionUtils.CanGetMemberValue(member))
			{
				return false;
			}
			Type memberUnderlyingType = ReflectionUtils.GetMemberUnderlyingType(member);
			return TypeHelpers.IsSupportedParameterType(memberUnderlyingType);
		}
		private static bool HasProperties(Type type)
		{
			return type.get_IsClass() || (!type.get_IsPrimitive() && !type.get_IsEnum());
		}
		private static void SetFsmPropertyName(object userdata)
		{
			if (TypeHelpers.targetFsmProperty != null)
			{
				TypeHelpers.targetFsmProperty.SetPropertyName(TypeHelpers.NicifyPropertyPath(userdata as string));
			}
			SkillEditor.SetFsmDirty(true, false);
			SkillEditor.SaveActions();
		}
		private static string NicifyPropertyPath(string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				return path.Replace('/', '.');
			}
			return "";
		}
	}
}
