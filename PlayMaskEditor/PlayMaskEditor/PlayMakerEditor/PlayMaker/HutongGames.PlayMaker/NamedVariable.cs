using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class NamedVariable : INameable, INamedVariable, IComparable
	{
		[SerializeField]
		private bool useVariable;
		[SerializeField]
		private string name;
		[SerializeField]
		private string tooltip = "";
		[SerializeField]
		private bool showInInspector;
		[SerializeField]
		private bool networkSync;
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}
		public virtual VariableType VariableType
		{
			get
			{
				throw new Exception("VariableType not implemented: " + base.GetType().get_FullName());
			}
		}
		public virtual Type ObjectType
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual VariableType TypeConstraint
		{
			get
			{
				return this.VariableType;
			}
		}
		public virtual object RawValue
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		public string Tooltip
		{
			get
			{
				return this.tooltip;
			}
			set
			{
				this.tooltip = value;
			}
		}
		public bool UseVariable
		{
			get
			{
				return this.useVariable;
			}
			set
			{
				this.useVariable = value;
			}
		}
		public bool ShowInInspector
		{
			get
			{
				return this.showInInspector;
			}
			set
			{
				this.showInInspector = value;
			}
		}
		public bool NetworkSync
		{
			get
			{
				return this.networkSync;
			}
			set
			{
				this.networkSync = value;
			}
		}
		public bool IsNone
		{
			get
			{
				return this.useVariable && string.IsNullOrEmpty(this.name);
			}
		}
		public bool UsesVariable
		{
			get
			{
				return this.useVariable && !string.IsNullOrEmpty(this.name);
			}
		}
		public static bool IsNullOrNone(NamedVariable variable)
		{
			return variable == null || variable.IsNone;
		}
		public NamedVariable()
		{
			this.name = "";
			this.tooltip = "";
		}
		public NamedVariable(string name)
		{
			this.name = name;
			if (!string.IsNullOrEmpty(name))
			{
				this.useVariable = true;
			}
		}
		public NamedVariable(NamedVariable source)
		{
			if (source != null)
			{
				this.useVariable = source.useVariable;
				this.name = source.name;
				this.showInInspector = source.showInInspector;
				this.tooltip = source.tooltip;
				this.networkSync = source.networkSync;
			}
		}
		public virtual bool TestTypeConstraint(VariableType variableType, Type objectType = null)
		{
			return variableType == VariableType.Unknown || this.TypeConstraint == variableType;
		}
		public virtual void SafeAssign(object val)
		{
			throw new NotImplementedException();
		}
		public virtual NamedVariable Clone()
		{
			throw new NotImplementedException();
		}
		public string GetDisplayName()
		{
			if (string.IsNullOrEmpty(this.Name))
			{
				return "None";
			}
			return this.Name;
		}
		public int CompareTo(object obj)
		{
			NamedVariable namedVariable = obj as NamedVariable;
			if (namedVariable == null)
			{
				return 0;
			}
			return string.CompareOrdinal(this.name, namedVariable.name);
		}
	}
}
