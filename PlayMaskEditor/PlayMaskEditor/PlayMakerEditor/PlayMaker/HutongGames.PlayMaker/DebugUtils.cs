using System;
using System.Diagnostics;
namespace HutongGames.PlayMaker
{
	public class DebugUtils
	{
		[Conditional("DEBUG")]
		public static void Assert(bool condition)
		{
			if (!condition)
			{
				throw new Exception();
			}
		}
	}
}
