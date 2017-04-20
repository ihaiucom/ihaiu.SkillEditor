using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	internal class ObjectPropertyDrawers
	{
		private static Dictionary<Type, ObjectPropertyDrawer> drawers;
		public static ObjectPropertyDrawer GetObjectPropertyDrawer(Type objType)
		{
			if (ObjectPropertyDrawers.drawers == null)
			{
				ObjectPropertyDrawers.Rebuild();
			}
			ObjectPropertyDrawer result;
			ObjectPropertyDrawers.drawers.TryGetValue(objType, ref result);
			return result;
		}
		public static void Rebuild()
		{
			ObjectPropertyDrawers.drawers = new Dictionary<Type, ObjectPropertyDrawer>();
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
						if (typeof(ObjectPropertyDrawer).IsAssignableFrom(type) && type.get_IsClass() && !type.get_IsAbstract())
						{
							ObjectPropertyDrawerAttribute attribute = CustomAttributeHelpers.GetAttribute<ObjectPropertyDrawerAttribute>(type);
							Type type2 = (attribute != null) ? attribute.InspectedType : null;
							if (type2 != null && !ObjectPropertyDrawers.drawers.ContainsKey(type2))
							{
								ObjectPropertyDrawers.drawers.Add(type2, (ObjectPropertyDrawer)Activator.CreateInstance(type));
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
