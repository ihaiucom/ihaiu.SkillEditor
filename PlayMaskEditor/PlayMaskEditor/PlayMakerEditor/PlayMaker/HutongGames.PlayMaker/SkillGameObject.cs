using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class SkillGameObject : NamedVariable
	{
		[SerializeField]
		private GameObject value;
		public GameObject Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}
		public override object RawValue
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = (value as GameObject);
			}
		}
		public override VariableType VariableType
		{
			get
			{
				return VariableType.GameObject;
			}
		}
		public override void SafeAssign(object val)
		{
			this.value = (val as GameObject);
		}
		public SkillGameObject()
		{
		}
		public SkillGameObject(string name) : base(name)
		{
		}
		public SkillGameObject(SkillGameObject source) : base(source)
		{
			if (source != null)
			{
				this.value = source.value;
			}
		}
		public override NamedVariable Clone()
		{
			return new SkillGameObject(this);
		}
		public override string ToString()
		{
			if (!(this.value == null))
			{
				return this.value.get_name();
			}
			return "None";
		}
		public static implicit operator SkillGameObject(GameObject value)
		{
			return new SkillGameObject(string.Empty)
			{
				value = value
			};
		}
	}
}
