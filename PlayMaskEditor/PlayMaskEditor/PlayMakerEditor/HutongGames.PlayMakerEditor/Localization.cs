using System;
using System.Globalization;
using System.Resources;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	public class Localization
	{
		public static void DebugAvailableCultures()
		{
			ResourceManager resourceManager = Strings.get_ResourceManager();
			string text = "";
			CultureInfo[] cultures = CultureInfo.GetCultures(7);
			CultureInfo[] array = cultures;
			for (int i = 0; i < array.Length; i++)
			{
				CultureInfo cultureInfo = array[i];
				ResourceSet resourceSet = resourceManager.GetResourceSet(cultureInfo, true, false);
				if (resourceSet != null)
				{
					text = text + cultureInfo.get_EnglishName() + Environment.get_NewLine();
				}
			}
			Debug.Log(Strings.get_Label_PlayMaker_Supported_Languages() + Environment.get_NewLine() + text);
		}
	}
}
