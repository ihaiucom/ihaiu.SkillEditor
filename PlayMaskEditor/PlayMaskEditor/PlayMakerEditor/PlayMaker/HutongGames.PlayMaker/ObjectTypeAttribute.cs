using System;
namespace HutongGames.PlayMaker
{
	[AttributeUsage]
	public sealed class ObjectTypeAttribute : Attribute
	{
		private readonly Type objectType;
		public Type ObjectType
		{
			get
			{
				return this.objectType;
			}
		}
		public ObjectTypeAttribute(Type objectType)
		{
			this.objectType = objectType;
		}
	}
}
