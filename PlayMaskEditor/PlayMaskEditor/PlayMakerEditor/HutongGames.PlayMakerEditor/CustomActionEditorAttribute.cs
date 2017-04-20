using System;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public sealed class CustomActionEditorAttribute : Attribute
	{
		public Type InspectedType;
		public CustomActionEditorAttribute(Type inspectedType)
		{
			if (inspectedType == null)
			{
				Debug.LogError(Strings.get_Error_Failed_to_load_Custom_Action_Editor());
			}
			this.InspectedType = inspectedType;
		}
	}
}
