using HutongGames.PlayMaker;
using System;
namespace HutongGames.PlayMakerEditor
{
	public abstract class CustomActionEditor
	{
		public SkillStateAction target;
		public virtual void OnEnable()
		{
		}
		public virtual void OnFocus()
		{
		}
		public abstract bool OnGUI();
		public virtual void OnSceneGUI()
		{
		}
		public bool DrawDefaultInspector()
		{
			return SkillEditor.ActionEditor.DrawDefaultInspector(this.target);
		}
		public void EditField(string fieldName)
		{
			SkillEditor.ActionEditor.EditField(this.target, fieldName);
		}
		public virtual void OnDisable()
		{
		}
	}
}
