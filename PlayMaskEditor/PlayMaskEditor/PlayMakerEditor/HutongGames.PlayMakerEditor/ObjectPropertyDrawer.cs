using System;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public abstract class ObjectPropertyDrawer
	{
		public abstract Object OnGUI(GUIContent label, Object obj, bool isSceneObject, params object[] attributes);
	}
}
