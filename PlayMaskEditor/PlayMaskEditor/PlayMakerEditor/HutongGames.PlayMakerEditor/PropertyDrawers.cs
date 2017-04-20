using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	internal class PropertyDrawers
	{
		private static Dictionary<Type, PropertyDrawer> drawers;
		public static PropertyDrawer GetPropertyDrawer(Type objType)
		{
			if (PropertyDrawers.drawers == null)
			{
				PropertyDrawers.Rebuild();
			}
			PropertyDrawer result;
			PropertyDrawers.drawers.TryGetValue(objType, ref result);
			return result;
		}
		public static void Rebuild()
		{
			PropertyDrawers.drawers = new Dictionary<Type, PropertyDrawer>();
			Assembly[] assemblies = AppDomain.get_CurrentDomain().GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				try
				{
					Type[] exportedTypes = assembly.GetExportedTypes();
					Type[] array = exportedTypes;
					for (int j = 0; j < array.Length; j++)
					{
						Type type = array[j];
						if (typeof(PropertyDrawer).IsAssignableFrom(type) && type.get_IsClass() && !type.get_IsAbstract())
						{
							PropertyDrawerAttribute attribute = CustomAttributeHelpers.GetAttribute<PropertyDrawerAttribute>(type);
							Type type2 = (attribute != null) ? attribute.InspectedType : null;
							if (type2 != null && !PropertyDrawers.drawers.ContainsKey(type2))
							{
								PropertyDrawers.drawers.Add(type2, (PropertyDrawer)Activator.CreateInstance(type));
							}
						}
					}
				}
				catch (Exception ex)
				{
					NotSupportedException arg_B2_0 = ex as NotSupportedException;
				}
			}
		}
	}
}
