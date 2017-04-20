using System;
namespace HutongGames.PlayMaker
{
	[AttributeUsage]
	public sealed class HelpUrlAttribute : Attribute
	{
		private readonly string url;
		public string Url
		{
			get
			{
				return this.url;
			}
		}
		public HelpUrlAttribute(string url)
		{
			this.url = url;
		}
	}
}
