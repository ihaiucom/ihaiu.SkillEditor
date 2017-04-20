using System;
using System.Text.RegularExpressions;
namespace HutongGames.Utility
{
	public static class StringUtils
	{
		public static string IncrementStringCounter(string s)
		{
			Match match = Regex.Match(s, "(?<Name>.*)\\s(?<Number>[0-9]+)$");
			if (!match.get_Success())
			{
				return s + " 2";
			}
			int length = match.get_Groups().get_Item("Number").get_Length();
			int num;
			if (!int.TryParse(match.get_Groups().get_Item("Number").get_Value(), ref num))
			{
				num = 1;
			}
			return match.get_Groups().get_Item("Name").get_Value() + " " + (num + 1).ToString("D" + length);
		}
	}
}
