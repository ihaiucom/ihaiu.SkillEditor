using System;
using System.ComponentModel;
using System.Globalization;
namespace HutongGames.Editor
{
	[Localizable(false)]
	public static class StringHelper
	{
		public static string TrimAndLower(this string input)
		{
			return input.Trim().ToLower();
		}
		public static string ToCamelCase(this string input)
		{
			if (input == null)
			{
				return "";
			}
			TextInfo textInfo = new CultureInfo("en-US", false).get_TextInfo();
			return textInfo.ToTitleCase(input.Trim()).Replace(" ", "");
		}
	}
}
