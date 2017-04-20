using System;
namespace HutongGames.PlayMaker
{
	[AttributeUsage]
	public sealed class MatchFieldTypeAttribute : Attribute
	{
		private readonly string fieldName;
		public string FieldName
		{
			get
			{
				return this.fieldName;
			}
		}
		public MatchFieldTypeAttribute(string fieldName)
		{
			this.fieldName = fieldName;
		}
	}
}
