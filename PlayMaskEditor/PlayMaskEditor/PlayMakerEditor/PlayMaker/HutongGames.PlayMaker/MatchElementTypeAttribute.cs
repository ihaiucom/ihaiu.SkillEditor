using System;
namespace HutongGames.PlayMaker
{
	[AttributeUsage]
	public sealed class MatchElementTypeAttribute : Attribute
	{
		private readonly string fieldName;
		public string FieldName
		{
			get
			{
				return this.fieldName;
			}
		}
		public MatchElementTypeAttribute(string fieldName)
		{
			this.fieldName = fieldName;
		}
	}
}
