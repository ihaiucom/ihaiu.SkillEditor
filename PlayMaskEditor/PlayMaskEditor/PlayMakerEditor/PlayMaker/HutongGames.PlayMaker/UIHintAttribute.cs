using System;
namespace HutongGames.PlayMaker
{
	[AttributeUsage]
	public sealed class UIHintAttribute : Attribute
	{
		private readonly UIHint hint;
		public UIHint Hint
		{
			get
			{
				return this.hint;
			}
		}
		public UIHintAttribute(UIHint hint)
		{
			this.hint = hint;
		}
	}
}
