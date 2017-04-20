using System;
namespace HutongGames.PlayMaker
{
	[AttributeUsage]
	public sealed class ActionTarget : Attribute
	{
		private readonly Type objectType;
		private readonly string fieldName;
		private readonly bool allowPrefabs;
		public Type ObjectType
		{
			get
			{
				return this.objectType;
			}
		}
		public string FieldName
		{
			get
			{
				return this.fieldName;
			}
		}
		public bool AllowPrefabs
		{
			get
			{
				return this.allowPrefabs;
			}
		}
		public ActionTarget(Type objectType, string fieldName = "", bool allowPrefabs = false)
		{
			this.objectType = objectType;
			this.fieldName = fieldName;
			this.allowPrefabs = allowPrefabs;
		}
		public bool IsSameAs(ActionTarget actionTarget)
		{
			return object.ReferenceEquals(this.objectType, actionTarget.objectType) && this.fieldName == actionTarget.fieldName;
		}
		public override string ToString()
		{
			return "ActionTarget: " + ((!object.ReferenceEquals(this.objectType, null)) ? this.objectType.get_FullName() : "null") + " , " + ((!string.IsNullOrEmpty(this.fieldName)) ? this.fieldName : "none");
		}
	}
}
