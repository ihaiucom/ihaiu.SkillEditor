using System;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public sealed class ObjectPropertyDrawerAttribute : Attribute
	{
		public Type InspectedType;
		public ObjectPropertyDrawerAttribute(Type inspectedType)
		{
			if (inspectedType == null)
			{
				Debug.LogError(Strings.get_Error_Failed_to_load_Object_Property_Drawer());
			}
			this.InspectedType = inspectedType;
		}
	}
}
