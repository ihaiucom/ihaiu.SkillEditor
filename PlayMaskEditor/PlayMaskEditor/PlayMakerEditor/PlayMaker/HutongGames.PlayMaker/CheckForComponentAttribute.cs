using System;
namespace HutongGames.PlayMaker
{
	[AttributeUsage]
	public sealed class CheckForComponentAttribute : Attribute
	{
		private readonly Type type0;
		private readonly Type type1;
		private readonly Type type2;
		public Type Type0
		{
			get
			{
				return this.type0;
			}
		}
		public Type Type1
		{
			get
			{
				return this.type1;
			}
		}
		public Type Type2
		{
			get
			{
				return this.type2;
			}
		}
		public CheckForComponentAttribute(Type type0)
		{
			this.type0 = type0;
		}
		public CheckForComponentAttribute(Type type0, Type type1)
		{
			this.type0 = type0;
			this.type1 = type1;
		}
		public CheckForComponentAttribute(Type type0, Type type1, Type type2)
		{
			this.type0 = type0;
			this.type1 = type1;
			this.type2 = type2;
		}
	}
}
