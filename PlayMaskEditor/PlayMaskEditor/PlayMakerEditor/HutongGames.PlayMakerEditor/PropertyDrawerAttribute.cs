using System;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public sealed class PropertyDrawerAttribute : Attribute
	{
		public Type InspectedType;
		public PropertyDrawerAttribute(Type inspectedType)
		{
			if (inspectedType == null)
			{
				Debug.LogError(Strings.get_Error_Failed_to_load_Property_Drawer());
			}
			this.InspectedType = inspectedType;
		}
	}
}
