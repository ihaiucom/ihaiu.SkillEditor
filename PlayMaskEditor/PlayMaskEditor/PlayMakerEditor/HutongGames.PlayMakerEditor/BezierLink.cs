using HutongGames.PlayMaker;
using System;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	internal static class BezierLink
	{
		public static void Draw(SkillState fromState, SkillState toState, int transitionIndex, Color linkColor, float linkWidth, Texture leftArrow, Texture rightArrow, float scale)
		{
			SkillTransition fsmTransition = fromState.get_Transitions()[transitionIndex];
			SkillTransition.CustomLinkConstraint linkConstraint = fsmTransition.get_LinkConstraint();
			float stateRowHeight = SkillEditorStyles.StateRowHeight;
			float num = stateRowHeight * 0.5f;
			Rect rect = SkillEditor.GraphView.ScaleRect(fromState.get_Position());
			Rect rect2 = SkillEditor.GraphView.ScaleRect(toState.get_Position());
			float num2 = rect.get_y() + (float)(transitionIndex + 1) * stateRowHeight + num;
			float num3 = rect2.get_y() + num;
			float num4 = rect.get_x() - 1f;
			float num5 = num4 + rect.get_width() + 2f;
			float num6 = rect2.get_x() - 1f;
			float num7 = num6 + rect2.get_width() + 2f;
			Vector3 vector = new Vector3(0f, num2, 0f);
			Vector3 vector2 = new Vector3(0f, num3, 0f);
			float num8 = (float)leftArrow.get_width() * scale;
			float num9;
			float num10;
			switch (linkConstraint)
			{
			case 0:
				if (num7 < num4)
				{
					vector.x = num4;
					vector2.x = num7 + num8;
					num9 = -1f;
					num10 = 1f;
				}
				else
				{
					if (num6 < num4)
					{
						vector.x = num4;
						vector2.x = num6 - num8;
						num9 = -1f;
						num10 = -1f;
					}
					else
					{
						if (num6 > num5)
						{
							vector.x = num5;
							vector2.x = num6 - num8;
							num9 = 1f;
							num10 = -1f;
						}
						else
						{
							vector.x = num5;
							vector2.x = num7 + num8;
							num9 = 1f;
							num10 = 1f;
						}
					}
				}
				break;
			case 1:
				if (num7 < num4)
				{
					vector.x = num4;
					vector2.x = num7 + num8;
					num9 = -1f;
					num10 = 1f;
				}
				else
				{
					vector.x = num4;
					vector2.x = num6 - num8;
					num9 = -1f;
					num10 = -1f;
				}
				break;
			default:
				if (num6 > num5)
				{
					vector.x = num5;
					vector2.x = num6 - num8;
					num9 = 1f;
					num10 = -1f;
				}
				else
				{
					vector.x = num5;
					vector2.x = num7 + num8;
					num9 = 1f;
					num10 = 1f;
				}
				break;
			}
			float num11 = (vector - vector2).get_magnitude() * 0.5f * scale;
			float num12 = Mathf.Min(num11, 40f);
			if (fromState == toState)
			{
				num12 = 50f * scale;
			}
			Vector3 vector3 = vector;
			vector3.x += num9 * num12;
			Vector3 vector4 = vector2;
			vector4.x += num10 * num12;
			Handles.DrawBezier(vector, vector2, vector3, vector4, linkColor, SkillEditorStyles.LineTexture, linkWidth * 2f);
			vector2.x -= num10 * num8;
			Color color = GUI.get_color();
			GUI.set_color(Color.get_red());
			if (num10 != 1f)
			{
				leftArrow = rightArrow;
				vector2.x -= num8;
			}
			Link.DrawArrowHead(leftArrow, vector2, linkColor, false, scale);
			GUI.set_color(color);
		}
	}
}
