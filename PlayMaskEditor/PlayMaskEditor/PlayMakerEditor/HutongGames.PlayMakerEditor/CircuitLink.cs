using HutongGames.PlayMaker;
using System;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	internal static class CircuitLink
	{
		private const float TangentYFactor = 0.4f;
		private const float MinTangentLength = 30f;
		private const float MaxTangentLength = 48f;
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
			Vector3 vector3 = vector;
			Vector3 vector4 = vector2;
			float num8 = Mathf.Abs(num2 - num3) * 0.4f;
			num8 = Mathf.Clamp(num8, 30f, 48f) * scale;
			float num9 = 30f * scale;
			bool flag = false;
			switch (linkConstraint)
			{
			case 0:
				if (num7 + num9 < num4 - num9)
				{
					vector.x = num4;
					vector2.x = num7;
					float num10 = vector.x - num8;
					float num11 = vector2.x + num8;
					vector3.x = num10 + (num11 - num10) * 0.3f;
					vector4.x = vector3.x;
					flag = true;
				}
				else
				{
					if (num6 < num4)
					{
						vector.x = num4;
						vector2.x = num6;
						float num12 = vector.x - num8;
						float num13 = vector2.x - num8;
						vector3.x = Mathf.Min(num12, num13);
						vector4.x = vector3.x;
					}
					else
					{
						if (num6 - num9 > num5 + num9)
						{
							vector.x = num5;
							vector2.x = num6;
							float num14 = vector.x + num8;
							float num15 = vector2.x - num8;
							vector3.x = num14 + (num15 - num14) * 0.3f;
							vector4.x = vector3.x;
						}
						else
						{
							vector.x = num5;
							vector2.x = num7;
							float num16 = vector.x + num8;
							float num17 = vector2.x + num8;
							vector3.x = Mathf.Max(num16, num17);
							vector4.x = vector3.x;
							flag = true;
						}
					}
				}
				break;
			case 1:
				if (num7 < num4)
				{
					vector.x = num4;
					vector2.x = num7;
					float num18 = vector.x - num8;
					float num19 = vector2.x + num8;
					vector3.x = num18 + (num19 - num18) * 0.3f;
					vector4.x = vector3.x;
					flag = true;
				}
				else
				{
					vector.x = num4;
					vector2.x = num6;
					float num20 = vector.x - num8;
					float num21 = vector2.x - num8;
					vector3.x = Mathf.Min(num20, num21);
					vector4.x = vector3.x;
				}
				break;
			default:
				if (num6 > num5)
				{
					vector.x = num5;
					vector2.x = num6;
					float num22 = vector.x + num8;
					float num23 = vector2.x - num8;
					vector3.x = num22 + (num23 - num22) * 0.3f;
					vector4.x = vector3.x;
				}
				else
				{
					vector.x = num5;
					vector2.x = num7;
					float num24 = vector.x + num8;
					float num25 = vector2.x + num8;
					vector3.x = Mathf.Max(num24, num25);
					vector4.x = vector3.x;
					flag = true;
				}
				break;
			}
			CircuitLink.DrawLine(vector, vector3, linkColor, linkWidth);
			CircuitLink.DrawLine(vector4, vector2, linkColor, linkWidth);
			float y = Mathf.Max(vector3.y, vector4.y) + 1f;
			float y2 = Mathf.Min(vector3.y, vector4.y) - 1f;
			vector3.y = y;
			vector4.y = y2;
			CircuitLink.DrawLine(vector3, vector4, linkColor, linkWidth);
			if (!flag)
			{
				leftArrow = rightArrow;
				vector2.x -= (float)leftArrow.get_width() * scale;
			}
			Link.DrawArrowHead(leftArrow, vector2, linkColor, false, scale);
		}
		private static void DrawLine(Vector3 fromPos, Vector3 toPos, Color color, float width)
		{
			Handles.DrawBezier(fromPos, toPos, fromPos, toPos, color, SkillEditorStyles.LineTexture, width * 2f);
		}
	}
}
