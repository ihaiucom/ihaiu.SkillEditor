using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	public static class SkillUtility
	{
		public static class BitConverter
		{
			public static int ToInt32(byte[] value, int startIndex)
			{
				if (System.BitConverter.IsLittleEndian)
				{
					return System.BitConverter.ToInt32(value, startIndex);
				}
				Array.Reverse(value, startIndex, 4);
				return System.BitConverter.ToInt32(value, startIndex);
			}
			public static float ToSingle(byte[] value, int startIndex)
			{
				if (System.BitConverter.IsLittleEndian)
				{
					return System.BitConverter.ToSingle(value, startIndex);
				}
				Array.Reverse(value, startIndex, 4);
				return System.BitConverter.ToSingle(value, startIndex);
			}
			public static bool ToBoolean(byte[] value, int startIndex)
			{
				return System.BitConverter.ToBoolean(value, startIndex);
			}
			public static byte[] GetBytes(bool value)
			{
				if (System.BitConverter.IsLittleEndian)
				{
					return System.BitConverter.GetBytes(value);
				}
				byte[] bytes = System.BitConverter.GetBytes(value);
				Array.Reverse(bytes);
				return bytes;
			}
			public static byte[] GetBytes(int value)
			{
				if (System.BitConverter.IsLittleEndian)
				{
					return System.BitConverter.GetBytes(value);
				}
				byte[] bytes = System.BitConverter.GetBytes(value);
				Array.Reverse(bytes);
				return bytes;
			}
			public static byte[] GetBytes(float value)
			{
				if (System.BitConverter.IsLittleEndian)
				{
					return System.BitConverter.GetBytes(value);
				}
				byte[] bytes = System.BitConverter.GetBytes(value);
				Array.Reverse(bytes);
				return bytes;
			}
		}
		private static UTF8Encoding encoding;
		public static UTF8Encoding Encoding
		{
			get
			{
				UTF8Encoding arg_14_0;
				if ((arg_14_0 = SkillUtility.encoding) == null)
				{
					arg_14_0 = (SkillUtility.encoding = new UTF8Encoding());
				}
				return arg_14_0;
			}
		}
		[Obsolete("Use VariableType property in NamedVariable")]
		public static VariableType GetVariableType(INamedVariable variable)
		{
			if (variable == null)
			{
				return VariableType.Unknown;
			}
			Type type = variable.GetType();
			if (object.ReferenceEquals(type, typeof(SkillMaterial)))
			{
				return VariableType.Material;
			}
			if (object.ReferenceEquals(type, typeof(SkillTexture)))
			{
				return VariableType.Texture;
			}
			if (object.ReferenceEquals(type, typeof(SkillFloat)))
			{
				return VariableType.Float;
			}
			if (object.ReferenceEquals(type, typeof(SkillInt)))
			{
				return VariableType.Int;
			}
			if (object.ReferenceEquals(type, typeof(SkillBool)))
			{
				return VariableType.Bool;
			}
			if (object.ReferenceEquals(type, typeof(SkillString)))
			{
				return VariableType.String;
			}
			if (object.ReferenceEquals(type, typeof(SkillGameObject)))
			{
				return VariableType.GameObject;
			}
			if (object.ReferenceEquals(type, typeof(SkillVector2)))
			{
				return VariableType.Vector2;
			}
			if (object.ReferenceEquals(type, typeof(SkillVector3)))
			{
				return VariableType.Vector3;
			}
			if (object.ReferenceEquals(type, typeof(SkillRect)))
			{
				return VariableType.Rect;
			}
			if (object.ReferenceEquals(type, typeof(SkillQuaternion)))
			{
				return VariableType.Quaternion;
			}
			if (object.ReferenceEquals(type, typeof(SkillColor)))
			{
				return VariableType.Color;
			}
			if (object.ReferenceEquals(type, typeof(SkillObject)))
			{
				return VariableType.Object;
			}
			if (object.ReferenceEquals(type, typeof(SkillEnum)))
			{
				return VariableType.Enum;
			}
			if (object.ReferenceEquals(type, typeof(SkillArray)))
			{
				return VariableType.Array;
			}
			return VariableType.Unknown;
		}
		public static Type GetVariableRealType(VariableType variableType)
		{
			switch (variableType)
			{
			case VariableType.Unknown:
				return null;
			case VariableType.Float:
				return typeof(float);
			case VariableType.Int:
				return typeof(int);
			case VariableType.Bool:
				return typeof(bool);
			case VariableType.GameObject:
				return typeof(GameObject);
			case VariableType.String:
				return typeof(string);
			case VariableType.Vector2:
				return typeof(Vector2);
			case VariableType.Vector3:
				return typeof(Vector3);
			case VariableType.Color:
				return typeof(Color);
			case VariableType.Rect:
				return typeof(Rect);
			case VariableType.Material:
				return typeof(Material);
			case VariableType.Texture:
				return typeof(Texture);
			case VariableType.Quaternion:
				return typeof(Quaternion);
			case VariableType.Object:
				return typeof(Object);
			case VariableType.Array:
				return typeof(Array);
			case VariableType.Enum:
				return typeof(Enum);
			default:
				throw new ArgumentOutOfRangeException("variableType");
			}
		}
		public static object GetEnum(Type enumType, int enumValue)
		{
			return Enum.ToObject(enumType, enumValue);
		}
		public static ICollection<byte> FsmEventToByteArray(SkillEvent fsmEvent)
		{
			if (fsmEvent == null)
			{
				return null;
			}
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.StringToByteArray(fsmEvent.Name));
			return list;
		}
		public static ICollection<byte> FsmFloatToByteArray(SkillFloat fsmFloat)
		{
			if (fsmFloat == null)
			{
				fsmFloat = new SkillFloat();
			}
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.BitConverter.GetBytes(fsmFloat.Value));
			list.AddRange(SkillUtility.BitConverter.GetBytes(fsmFloat.UseVariable));
			list.AddRange(SkillUtility.StringToByteArray(fsmFloat.Name));
			return list;
		}
		public static ICollection<byte> FsmIntToByteArray(SkillInt fsmInt)
		{
			if (fsmInt == null)
			{
				fsmInt = new SkillInt();
			}
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.BitConverter.GetBytes(fsmInt.Value));
			list.AddRange(SkillUtility.BitConverter.GetBytes(fsmInt.UseVariable));
			list.AddRange(SkillUtility.StringToByteArray(fsmInt.Name));
			return list;
		}
		public static ICollection<byte> FsmBoolToByteArray(SkillBool fsmBool)
		{
			if (fsmBool == null)
			{
				fsmBool = new SkillBool();
			}
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.BitConverter.GetBytes(fsmBool.Value));
			list.AddRange(SkillUtility.BitConverter.GetBytes(fsmBool.UseVariable));
			list.AddRange(SkillUtility.StringToByteArray(fsmBool.Name));
			return list;
		}
		public static ICollection<byte> FsmVector2ToByteArray(SkillVector2 fsmVector2)
		{
			if (fsmVector2 == null)
			{
				fsmVector2 = new SkillVector2();
			}
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.Vector2ToByteArray(fsmVector2.Value));
			list.AddRange(SkillUtility.BitConverter.GetBytes(fsmVector2.UseVariable));
			list.AddRange(SkillUtility.StringToByteArray(fsmVector2.Name));
			return list;
		}
		public static ICollection<byte> FsmVector3ToByteArray(SkillVector3 fsmVector3)
		{
			if (fsmVector3 == null)
			{
				fsmVector3 = new SkillVector3();
			}
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.Vector3ToByteArray(fsmVector3.Value));
			list.AddRange(SkillUtility.BitConverter.GetBytes(fsmVector3.UseVariable));
			list.AddRange(SkillUtility.StringToByteArray(fsmVector3.Name));
			return list;
		}
		public static ICollection<byte> FsmRectToByteArray(SkillRect fsmRect)
		{
			if (fsmRect == null)
			{
				fsmRect = new SkillRect();
			}
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.RectToByteArray(fsmRect.Value));
			list.AddRange(SkillUtility.BitConverter.GetBytes(fsmRect.UseVariable));
			list.AddRange(SkillUtility.StringToByteArray(fsmRect.Name));
			return list;
		}
		public static ICollection<byte> FsmQuaternionToByteArray(SkillQuaternion fsmQuaternion)
		{
			if (fsmQuaternion == null)
			{
				fsmQuaternion = new SkillQuaternion();
			}
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.QuaternionToByteArray(fsmQuaternion.Value));
			list.AddRange(SkillUtility.BitConverter.GetBytes(fsmQuaternion.UseVariable));
			list.AddRange(SkillUtility.StringToByteArray(fsmQuaternion.Name));
			return list;
		}
		public static ICollection<byte> FsmColorToByteArray(SkillColor fsmColor)
		{
			if (fsmColor == null)
			{
				fsmColor = new SkillColor();
			}
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.ColorToByteArray(fsmColor.Value));
			list.AddRange(SkillUtility.BitConverter.GetBytes(fsmColor.UseVariable));
			list.AddRange(SkillUtility.StringToByteArray(fsmColor.Name));
			return list;
		}
		public static ICollection<byte> ColorToByteArray(Color color)
		{
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.BitConverter.GetBytes(color.r));
			list.AddRange(SkillUtility.BitConverter.GetBytes(color.g));
			list.AddRange(SkillUtility.BitConverter.GetBytes(color.b));
			list.AddRange(SkillUtility.BitConverter.GetBytes(color.a));
			return list;
		}
		public static ICollection<byte> Vector2ToByteArray(Vector2 vector2)
		{
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.BitConverter.GetBytes(vector2.get_Item(0)));
			list.AddRange(SkillUtility.BitConverter.GetBytes(vector2.get_Item(1)));
			return list;
		}
		public static ICollection<byte> Vector3ToByteArray(Vector3 vector3)
		{
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.BitConverter.GetBytes(vector3.get_Item(0)));
			list.AddRange(SkillUtility.BitConverter.GetBytes(vector3.get_Item(1)));
			list.AddRange(SkillUtility.BitConverter.GetBytes(vector3.get_Item(2)));
			return list;
		}
		public static ICollection<byte> Vector4ToByteArray(Vector4 vector4)
		{
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.BitConverter.GetBytes(vector4.get_Item(0)));
			list.AddRange(SkillUtility.BitConverter.GetBytes(vector4.get_Item(1)));
			list.AddRange(SkillUtility.BitConverter.GetBytes(vector4.get_Item(2)));
			list.AddRange(SkillUtility.BitConverter.GetBytes(vector4.get_Item(3)));
			return list;
		}
		public static ICollection<byte> RectToByteArray(Rect rect)
		{
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.BitConverter.GetBytes(rect.get_x()));
			list.AddRange(SkillUtility.BitConverter.GetBytes(rect.get_y()));
			list.AddRange(SkillUtility.BitConverter.GetBytes(rect.get_width()));
			list.AddRange(SkillUtility.BitConverter.GetBytes(rect.get_height()));
			return list;
		}
		public static ICollection<byte> QuaternionToByteArray(Quaternion quaternion)
		{
			List<byte> list = new List<byte>();
			list.AddRange(SkillUtility.BitConverter.GetBytes(quaternion.x));
			list.AddRange(SkillUtility.BitConverter.GetBytes(quaternion.y));
			list.AddRange(SkillUtility.BitConverter.GetBytes(quaternion.z));
			list.AddRange(SkillUtility.BitConverter.GetBytes(quaternion.w));
			return list;
		}
		public static byte[] StringToByteArray(string str)
		{
			if (str == null)
			{
				str = "";
			}
			return SkillUtility.Encoding.GetBytes(str);
		}
		public static string ByteArrayToString(byte[] bytes)
		{
			if (bytes.Length == 0)
			{
				return "";
			}
			return SkillUtility.Encoding.GetString(bytes);
		}
		public static string ByteArrayToString(byte[] bytes, int startIndex, int count)
		{
			if (count == 0)
			{
				return string.Empty;
			}
			return SkillUtility.Encoding.GetString(bytes, startIndex, count);
		}
		public static SkillEvent ByteArrayToFsmEvent(byte[] bytes, int startIndex, int size)
		{
			string text = SkillUtility.ByteArrayToString(bytes, startIndex, size);
			if (!string.IsNullOrEmpty(text))
			{
				return SkillEvent.GetFsmEvent(text);
			}
			return null;
		}
		public static SkillFloat ByteArrayToFsmFloat(Skill fsm, byte[] bytes, int startIndex, int totalLength)
		{
			string @string = SkillUtility.Encoding.GetString(bytes, startIndex + 5, totalLength - 5);
			if (@string != string.Empty)
			{
				return fsm.GetFsmFloat(@string);
			}
			return new SkillFloat
			{
				Value = SkillUtility.BitConverter.ToSingle(bytes, startIndex),
				UseVariable = SkillUtility.BitConverter.ToBoolean(bytes, startIndex + 4)
			};
		}
		public static SkillInt ByteArrayToFsmInt(Skill fsm, byte[] bytes, int startIndex, int totalLength)
		{
			string @string = SkillUtility.Encoding.GetString(bytes, startIndex + 5, totalLength - 5);
			if (@string != string.Empty)
			{
				return fsm.GetFsmInt(@string);
			}
			return new SkillInt
			{
				Value = SkillUtility.BitConverter.ToInt32(bytes, startIndex),
				UseVariable = SkillUtility.BitConverter.ToBoolean(bytes, startIndex + 4)
			};
		}
		public static SkillBool ByteArrayToFsmBool(Skill fsm, byte[] bytes, int startIndex, int totalLength)
		{
			string @string = SkillUtility.Encoding.GetString(bytes, startIndex + 2, totalLength - 2);
			if (@string != string.Empty)
			{
				return fsm.GetFsmBool(@string);
			}
			return new SkillBool
			{
				Value = SkillUtility.BitConverter.ToBoolean(bytes, startIndex),
				UseVariable = SkillUtility.BitConverter.ToBoolean(bytes, startIndex + 1)
			};
		}
		public static Color ByteArrayToColor(byte[] bytes, int startIndex)
		{
			float num = SkillUtility.BitConverter.ToSingle(bytes, startIndex);
			float num2 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 4);
			float num3 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 8);
			float num4 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 12);
			return new Color(num, num2, num3, num4);
		}
		public static Vector2 ByteArrayToVector2(byte[] bytes, int startIndex)
		{
			float num = SkillUtility.BitConverter.ToSingle(bytes, startIndex);
			float num2 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 4);
			return new Vector2(num, num2);
		}
		public static SkillVector2 ByteArrayToFsmVector2(Skill fsm, byte[] bytes, int startIndex, int totalLength)
		{
			string @string = SkillUtility.Encoding.GetString(bytes, startIndex + 9, totalLength - 9);
			if (@string != string.Empty)
			{
				return fsm.GetFsmVector2(@string);
			}
			return new SkillVector2
			{
				Value = SkillUtility.ByteArrayToVector2(bytes, startIndex),
				UseVariable = SkillUtility.BitConverter.ToBoolean(bytes, startIndex + 8)
			};
		}
		public static Vector3 ByteArrayToVector3(byte[] bytes, int startIndex)
		{
			float num = SkillUtility.BitConverter.ToSingle(bytes, startIndex);
			float num2 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 4);
			float num3 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 8);
			return new Vector3(num, num2, num3);
		}
		public static SkillVector3 ByteArrayToFsmVector3(Skill fsm, byte[] bytes, int startIndex, int totalLength)
		{
			string @string = SkillUtility.Encoding.GetString(bytes, startIndex + 13, totalLength - 13);
			if (@string != string.Empty)
			{
				return fsm.GetFsmVector3(@string);
			}
			return new SkillVector3
			{
				Value = SkillUtility.ByteArrayToVector3(bytes, startIndex),
				UseVariable = SkillUtility.BitConverter.ToBoolean(bytes, startIndex + 12)
			};
		}
		public static SkillRect ByteArrayToFsmRect(Skill fsm, byte[] bytes, int startIndex, int totalLength)
		{
			string @string = SkillUtility.Encoding.GetString(bytes, startIndex + 17, totalLength - 17);
			if (@string != string.Empty)
			{
				return fsm.GetFsmRect(@string);
			}
			return new SkillRect
			{
				Value = SkillUtility.ByteArrayToRect(bytes, startIndex),
				UseVariable = SkillUtility.BitConverter.ToBoolean(bytes, startIndex + 16)
			};
		}
		public static SkillQuaternion ByteArrayToFsmQuaternion(Skill fsm, byte[] bytes, int startIndex, int totalLength)
		{
			string @string = SkillUtility.Encoding.GetString(bytes, startIndex + 17, totalLength - 17);
			if (@string != string.Empty)
			{
				return fsm.GetFsmQuaternion(@string);
			}
			return new SkillQuaternion
			{
				Value = SkillUtility.ByteArrayToQuaternion(bytes, startIndex),
				UseVariable = SkillUtility.BitConverter.ToBoolean(bytes, startIndex + 16)
			};
		}
		public static SkillColor ByteArrayToFsmColor(Skill fsm, byte[] bytes, int startIndex, int totalLength)
		{
			string @string = SkillUtility.Encoding.GetString(bytes, startIndex + 17, totalLength - 17);
			if (@string != string.Empty)
			{
				return fsm.GetFsmColor(@string);
			}
			return new SkillColor
			{
				Value = SkillUtility.ByteArrayToColor(bytes, startIndex),
				UseVariable = SkillUtility.BitConverter.ToBoolean(bytes, startIndex + 16)
			};
		}
		public static Vector4 ByteArrayToVector4(byte[] bytes, int startIndex)
		{
			float num = SkillUtility.BitConverter.ToSingle(bytes, startIndex);
			float num2 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 4);
			float num3 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 8);
			float num4 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 12);
			return new Vector4(num, num2, num3, num4);
		}
		public static Rect ByteArrayToRect(byte[] bytes, int startIndex)
		{
			float num = SkillUtility.BitConverter.ToSingle(bytes, startIndex);
			float num2 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 4);
			float num3 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 8);
			float num4 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 12);
			return new Rect(num, num2, num3, num4);
		}
		public static Quaternion ByteArrayToQuaternion(byte[] bytes, int startIndex)
		{
			float num = SkillUtility.BitConverter.ToSingle(bytes, startIndex);
			float num2 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 4);
			float num3 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 8);
			float num4 = SkillUtility.BitConverter.ToSingle(bytes, startIndex + 12);
			return new Quaternion(num, num2, num3, num4);
		}
		private static byte[] ReadToEnd(Stream stream)
		{
			long position = stream.get_Position();
			stream.set_Position(0L);
			byte[] result;
			try
			{
				byte[] array = new byte[4096];
				int num = 0;
				int num2;
				while ((num2 = stream.Read(array, num, array.Length - num)) > 0)
				{
					num += num2;
					if (num == array.Length)
					{
						int num3 = stream.ReadByte();
						if (num3 != -1)
						{
							byte[] array2 = new byte[array.Length * 2];
							Buffer.BlockCopy(array, 0, array2, 0, array.Length);
							Buffer.SetByte(array2, num, (byte)num3);
							array = array2;
							num++;
						}
					}
				}
				byte[] array3 = array;
				if (array.Length != num)
				{
					array3 = new byte[num];
					Buffer.BlockCopy(array, 0, array3, 0, num);
				}
				result = array3;
			}
			finally
			{
				stream.set_Position(position);
			}
			return result;
		}
		public static string StripNamespace(string name)
		{
			if (name == null)
			{
				return "[missing name]";
			}
			return name.Substring(name.LastIndexOf(".", 4) + 1);
		}
		public static string GetPath(SkillState state)
		{
			if (state == null)
			{
				return "[missing state]";
			}
			return ((state.Fsm != null) ? (state.Fsm.OwnerDebugName + ": " + state.Fsm.Name) : "[missing FSM]") + ": " + state.Name + ": ";
		}
		public static string GetPath(SkillState state, SkillStateAction action)
		{
			if (action == null)
			{
				return SkillUtility.GetPath(state) + "[missing action] ";
			}
			return SkillUtility.GetPath(state) + action.GetType().get_Name() + ": ";
		}
		public static string GetPath(SkillState state, SkillStateAction action, string parameter)
		{
			return SkillUtility.GetPath(state, action) + parameter + ": ";
		}
		public static string GetFullFsmLabel(Skill fsm)
		{
			if (fsm == null)
			{
				return "None (FSM)";
			}
			if (fsm.UsedInTemplate != null)
			{
				return "Template: " + fsm.UsedInTemplate.get_name();
			}
			if (fsm.Owner == null)
			{
				return "FSM Missing Owner";
			}
			return fsm.OwnerName + " : " + SkillUtility.GetFsmLabel(fsm);
		}
		public static string GetFullFsmLabel(PlayMakerFSM fsm)
		{
			if (fsm == null)
			{
				return "None (PlayMakerFSM)";
			}
			if (fsm.Fsm == null)
			{
				return "None (Fsm)";
			}
			return fsm.get_gameObject().get_name() + " : " + fsm.FsmName;
		}
		public static string GetFsmLabel(Skill fsm)
		{
			if (fsm != null)
			{
				return fsm.Name;
			}
			return "None (Fsm)";
		}
		public static Object GetOwner(Skill fsm)
		{
			if (fsm == null)
			{
				return null;
			}
			if (fsm.UsedInTemplate)
			{
				return fsm.UsedInTemplate;
			}
			return fsm.Owner;
		}
	}
}
