using System;
namespace HutongGames.PlayMaker
{
	[AttributeUsage]
	public sealed class HasFloatSliderAttribute : Attribute
	{
		private readonly float minValue;
		private readonly float maxValue;
		public float MinValue
		{
			get
			{
				return this.minValue;
			}
		}
		public float MaxValue
		{
			get
			{
				return this.maxValue;
			}
		}
		public HasFloatSliderAttribute(float minValue, float maxValue)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
		}
	}
}
