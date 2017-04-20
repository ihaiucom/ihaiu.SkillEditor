using System;
namespace HutongGames.PlayMaker
{
	public interface INamedVariable
	{
		string Name
		{
			get;
		}
		bool UseVariable
		{
			get;
			set;
		}
		bool UsesVariable
		{
			get;
		}
		bool NetworkSync
		{
			get;
			set;
		}
		bool IsNone
		{
			get;
		}
		VariableType VariableType
		{
			get;
		}
		VariableType TypeConstraint
		{
			get;
		}
		Type ObjectType
		{
			get;
			set;
		}
		object RawValue
		{
			get;
			set;
		}
		string GetDisplayName();
		bool TestTypeConstraint(VariableType variableType, Type objectType = null);
		void SafeAssign(object val);
	}
}
