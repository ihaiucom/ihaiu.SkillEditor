using System;
using System.Collections.Generic;
namespace HutongGames.PlayMakerEditor
{
	internal static class ArrayUtility
	{
		public static T[] Copy<T>(T[] array)
		{
			List<T> list = new List<T>(array);
			return list.ToArray();
		}
		public static T[] Add<T>(T[] array, T item)
		{
			List<T> list = new List<T>(array);
			list.Add(item);
			List<T> list2 = list;
			return list2.ToArray();
		}
		public static T[] AddRange<T>(T[] array, T[] items)
		{
			List<T> list = new List<T>(array);
			list.AddRange(items);
			return list.ToArray();
		}
		public static T[] AddAndSort<T>(T[] array, T item)
		{
			List<T> list = new List<T>(array);
			list.Add(item);
			List<T> list2 = list;
			list2.Sort();
			return list2.ToArray();
		}
		public static T[] Sort<T>(T[] array)
		{
			List<T> list = new List<T>(array);
			list.Sort();
			return list.ToArray();
		}
		public static T[] RemoveAt<T>(T[] array, int index)
		{
			List<T> list = new List<T>(array);
			list.RemoveAt(index);
			return list.ToArray();
		}
		public static T[] Remove<T>(T[] array, T item)
		{
			List<T> list = new List<T>(array);
			list.Remove(item);
			return list.ToArray();
		}
		public static T[] MoveItem<T>(T[] array, int oldIndex, int newIndex)
		{
			List<T> list = new List<T>(array);
			T t = list.get_Item(oldIndex);
			list.RemoveAt(oldIndex);
			list.Insert(newIndex, t);
			return list.ToArray();
		}
		public static string GetDebugString<T>(T[] array)
		{
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				T t = array[i];
				text = text + t + ",";
			}
			return text;
		}
	}
}
