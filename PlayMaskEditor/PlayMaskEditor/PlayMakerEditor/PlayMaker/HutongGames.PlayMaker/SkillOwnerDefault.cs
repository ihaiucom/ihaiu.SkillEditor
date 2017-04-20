using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillOwnerDefault
	{
		[SerializeField]
		private OwnerDefaultOption ownerOption;
		[SerializeField]
		private SkillGameObject gameObject;
		public OwnerDefaultOption OwnerOption
		{
			get
			{
				return this.ownerOption;
			}
			set
			{
				this.ownerOption = value;
			}
		}
		public SkillGameObject GameObject
		{
			get
			{
				return this.gameObject;
			}
			set
			{
				this.gameObject = value;
			}
		}
		public SkillOwnerDefault()
		{
			this.ownerOption = OwnerDefaultOption.UseOwner;
			this.gameObject = new SkillGameObject(string.Empty);
		}
		public SkillOwnerDefault(SkillOwnerDefault source)
		{
			if (source != null)
			{
				this.ownerOption = source.ownerOption;
				this.gameObject = new SkillGameObject(source.GameObject);
			}
		}
	}
}
