using System;
using System.Collections.Generic;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillTemplateControl
	{
		public SkillTemplate fsmTemplate;
		public SkillVarOverride[] fsmVarOverrides = new SkillVarOverride[0];
		[NonSerialized]
		private Skill runFsm;
		public int ID
		{
			get;
			set;
		}
		public Skill RunFsm
		{
			get
			{
				return this.runFsm;
			}
			private set
			{
				this.runFsm = value;
			}
		}
		public SkillTemplateControl()
		{
			this.fsmVarOverrides = new SkillVarOverride[0];
		}
		public SkillTemplateControl(SkillTemplateControl source)
		{
			this.fsmTemplate = source.fsmTemplate;
			this.fsmVarOverrides = SkillTemplateControl.CopyOverrides(source);
		}
		public void SetFsmTemplate(SkillTemplate template)
		{
			this.fsmTemplate = template;
			this.ClearOverrides();
			this.UpdateOverrides();
		}
		public Skill InstantiateFsm()
		{
			this.RunFsm = new Skill(this.fsmTemplate.fsm, null);
			this.ApplyOverrides(this.RunFsm);
			return this.RunFsm;
		}
		private static SkillVarOverride[] CopyOverrides(SkillTemplateControl source)
		{
			SkillVarOverride[] array = new SkillVarOverride[source.fsmVarOverrides.Length];
			for (int i = 0; i < source.fsmVarOverrides.Length; i++)
			{
				array[i] = new SkillVarOverride(source.fsmVarOverrides[i]);
			}
			return array;
		}
		private void ClearOverrides()
		{
			this.fsmVarOverrides = new SkillVarOverride[0];
		}
		public void UpdateOverrides()
		{
			if (this.fsmTemplate != null)
			{
				List<SkillVarOverride> list = new List<SkillVarOverride>(this.fsmVarOverrides);
				List<SkillVarOverride> list2 = new List<SkillVarOverride>();
				NamedVariable[] allNamedVariables = this.fsmTemplate.fsm.Variables.GetAllNamedVariables();
				for (int i = 0; i < allNamedVariables.Length; i++)
				{
					NamedVariable namedVariable = allNamedVariables[i];
					if (namedVariable.ShowInInspector)
					{
						SkillVarOverride fsmVarOverride = list.Find((SkillVarOverride o) => o.variable.Name == namedVariable.Name);
						list2.Add(fsmVarOverride ?? new SkillVarOverride(namedVariable));
					}
				}
				this.fsmVarOverrides = list2.ToArray();
				return;
			}
			this.fsmVarOverrides = new SkillVarOverride[0];
		}
		public void UpdateValues()
		{
			SkillVarOverride[] array = this.fsmVarOverrides;
			for (int i = 0; i < array.Length; i++)
			{
				SkillVarOverride fsmVarOverride = array[i];
				fsmVarOverride.fsmVar.UpdateValue();
			}
		}
		public void ApplyOverrides(Skill overrideFsm)
		{
			SkillVarOverride[] array = this.fsmVarOverrides;
			for (int i = 0; i < array.Length; i++)
			{
				SkillVarOverride fsmVarOverride = array[i];
				fsmVarOverride.Apply(overrideFsm.Variables);
			}
		}
	}
}
