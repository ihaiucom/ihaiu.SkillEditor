using System;
namespace HutongGames.PlayMaker
{
	[AttributeUsage]
	public sealed class EventTargetAttribute : Attribute
	{
		private readonly SkillEventTarget.EventTarget target;
		public SkillEventTarget.EventTarget Target
		{
			get
			{
				return this.target;
			}
		}
		public EventTargetAttribute(SkillEventTarget.EventTarget target)
		{
			this.target = target;
		}
	}
}
