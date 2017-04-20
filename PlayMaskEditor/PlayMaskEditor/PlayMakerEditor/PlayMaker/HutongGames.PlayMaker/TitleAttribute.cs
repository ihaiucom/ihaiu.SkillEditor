using System;
namespace HutongGames.PlayMaker
{
	[AttributeUsage]
	public sealed class TitleAttribute : Attribute
	{
		private readonly string text;
		public string Text
		{
			get
			{
				return this.text;
			}
		}
		public TitleAttribute(string text)
		{
			this.text = text;
		}
	}
}
