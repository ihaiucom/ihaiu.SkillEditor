using System;
namespace HutongGames.PlayMaker
{
	[AttributeUsage]
	public sealed class VariableTypeAttribute : Attribute
	{
		private readonly VariableType type;
		public VariableType Type
		{
			get
			{
				return this.type;
			}
		}
		public VariableTypeAttribute(VariableType type)
		{
			this.type = type;
		}
	}
}
