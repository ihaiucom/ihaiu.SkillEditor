using System;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public abstract class PropertyDrawer
	{
		public abstract object OnGUI(GUIContent label, object obj, bool isSceneObject, params object[] attributes);
		public object EditField(string label, Type type, object obj, object[] attributes)
		{
			return SkillEditor.ActionEditor.EditField(label, type, obj, attributes);
		}
	}
}
