using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	[Serializable]
	public class LayoutOption
	{
		public enum LayoutOptionType
		{
			Width,
			Height,
			MinWidth,
			MaxWidth,
			MinHeight,
			MaxHeight,
			ExpandWidth,
			ExpandHeight
		}
		public LayoutOption.LayoutOptionType option;
		public SkillFloat floatParam;
		public SkillBool boolParam;
		public LayoutOption()
		{
			this.ResetParameters();
		}
		public LayoutOption(LayoutOption source)
		{
			this.option = source.option;
			this.floatParam = new SkillFloat(source.floatParam);
			this.boolParam = new SkillBool(source.boolParam);
		}
		public void ResetParameters()
		{
			this.floatParam = 0f;
			this.boolParam = false;
		}
		public GUILayoutOption GetGUILayoutOption()
		{
			switch (this.option)
			{
			case LayoutOption.LayoutOptionType.Width:
				return GUILayout.Width(this.floatParam.Value);
			case LayoutOption.LayoutOptionType.Height:
				return GUILayout.Height(this.floatParam.Value);
			case LayoutOption.LayoutOptionType.MinWidth:
				return GUILayout.MinWidth(this.floatParam.Value);
			case LayoutOption.LayoutOptionType.MaxWidth:
				return GUILayout.MaxWidth(this.floatParam.Value);
			case LayoutOption.LayoutOptionType.MinHeight:
				return GUILayout.MinHeight(this.floatParam.Value);
			case LayoutOption.LayoutOptionType.MaxHeight:
				return GUILayout.MaxHeight(this.floatParam.Value);
			case LayoutOption.LayoutOptionType.ExpandWidth:
				return GUILayout.ExpandWidth(this.boolParam.Value);
			case LayoutOption.LayoutOptionType.ExpandHeight:
				return GUILayout.ExpandHeight(this.boolParam.Value);
			default:
				return null;
			}
		}
	}
}
