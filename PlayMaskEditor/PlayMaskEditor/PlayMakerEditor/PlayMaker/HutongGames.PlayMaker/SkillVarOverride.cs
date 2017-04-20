using System;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillVarOverride
	{
		public NamedVariable variable;
		public SkillVar fsmVar;
		public bool isEdited;
		public SkillVarOverride(SkillVarOverride source)
		{
			this.variable = new NamedVariable(source.variable.Name);
			this.fsmVar = new SkillVar(source.fsmVar);
			this.isEdited = source.isEdited;
		}
		public SkillVarOverride(NamedVariable namedVar)
		{
			this.variable = namedVar;
			this.fsmVar = new SkillVar(this.variable);
			this.isEdited = false;
		}
		public void Apply(SkillVariables variables)
		{
			this.variable = variables.GetVariable(this.variable.Name);
			this.fsmVar.ApplyValueTo(this.variable);
		}
	}
}
