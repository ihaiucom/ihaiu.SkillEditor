using System;
namespace HutongGames.PlayMaker
{
	[AttributeUsage]
	public sealed class ActionCategoryAttribute : Attribute
	{
		private readonly string category;
		public string Category
		{
			get
			{
				return this.category;
			}
		}
		public ActionCategoryAttribute(string category)
		{
			this.category = category;
		}
		public ActionCategoryAttribute(ActionCategory category)
		{
			this.category = category.ToString();
		}
	}
}
