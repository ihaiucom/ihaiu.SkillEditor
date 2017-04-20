using System;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillEventTarget
	{
		public enum EventTarget
		{
			Self,
			GameObject,
			GameObjectFSM,
			FSMComponent,
			BroadcastAll,
			HostFSM,
			SubFSMs
		}
		private static SkillEventTarget self;
		public SkillEventTarget.EventTarget target;
		public SkillBool excludeSelf;
		public SkillOwnerDefault gameObject;
		public SkillString fsmName;
		public SkillBool sendToChildren;
		public PlayMakerFSM fsmComponent;
		public static SkillEventTarget Self
		{
			get
			{
				SkillEventTarget arg_14_0;
				if ((arg_14_0 = SkillEventTarget.self) == null)
				{
					arg_14_0 = (SkillEventTarget.self = new SkillEventTarget());
				}
				return arg_14_0;
			}
		}
		public SkillEventTarget()
		{
			this.ResetParameters();
		}
		public SkillEventTarget(SkillEventTarget source)
		{
			this.target = source.target;
			this.excludeSelf = new SkillBool(source.excludeSelf);
			this.gameObject = new SkillOwnerDefault(source.gameObject);
			this.fsmName = new SkillString(source.fsmName);
			this.sendToChildren = new SkillBool(source.sendToChildren);
			this.fsmComponent = source.fsmComponent;
		}
		public void ResetParameters()
		{
			this.excludeSelf = false;
			this.gameObject = null;
			this.fsmName = "";
			this.sendToChildren = false;
			this.fsmComponent = null;
		}
	}
}
