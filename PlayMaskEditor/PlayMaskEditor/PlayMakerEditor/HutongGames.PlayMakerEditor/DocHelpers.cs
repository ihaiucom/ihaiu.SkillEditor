using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	public static class DocHelpers
	{
		private const string MinicapPath = "C:\\Program Files\\MiniCap\\MiniCap.exe";
		private static StreamWriter sw;
		private static void MiniCap(string arguments, bool waitForExit)
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.set_Arguments(arguments);
			processStartInfo.set_FileName("C:\\Program Files\\MiniCap\\MiniCap.exe");
			processStartInfo.set_WindowStyle(1);
			processStartInfo.set_CreateNoWindow(true);
			ProcessStartInfo processStartInfo2 = processStartInfo;
			if (waitForExit)
			{
				using (Process process = Process.Start(processStartInfo2))
				{
					process.WaitForExit();
					return;
				}
			}
			Process.Start(processStartInfo2);
		}
		private static void CaptureRegion(Rect region, string savePath, string filename)
		{
			string text = " -save \"" + savePath + "\"";
			string text2 = string.Concat(new object[]
			{
				" -captureregion ",
				region.get_xMin(),
				" ",
				region.get_yMin(),
				" ",
				region.get_xMax(),
				" ",
				region.get_yMax(),
				" "
			});
			string arguments = string.Concat(new string[]
			{
				text2,
				text,
				"\"",
				filename,
				".png\" -exit"
			});
			DocHelpers.MiniCap(arguments, true);
		}
		public static void StartStateActionListCapture()
		{
			string text = string.Concat(new string[]
			{
				SkillEditor.SelectedFsmGameObject.get_name(),
				"_",
				SkillEditor.SelectedFsm.get_Name(),
				"_",
				SkillEditor.SelectedState.get_Name(),
				".txt"
			});
			DocHelpers.sw = File.CreateText("C:\\ActionScreens\\SampleScreens\\" + text);
			DocHelpers.sw.WriteLine("<div id=\"actionBreakdown\">");
			DocHelpers.sw.WriteLine("<h3>Overview</h3>");
			DocHelpers.sw.WriteLine("<p>TODO</p>");
			DocHelpers.sw.WriteLine("<h3>Actions</h3>");
			DocHelpers.sw.WriteLine("<table>");
		}
		public static void CaptureStateInspectorAction(Rect region, string actionName, int actionIndex)
		{
			if (DocHelpers.sw == null)
			{
				Debug.LogError("Must call StartStateActionListCapture first!");
				return;
			}
			Debug.Log("CaptureStateInspectorAction: " + actionName);
			string text = Labels.StripNamespace(actionName);
			actionName = string.Concat(new object[]
			{
				SkillEditor.SelectedFsmGameObject.get_name(),
				"_",
				SkillEditor.SelectedFsm.get_Name(),
				"_",
				SkillEditor.SelectedState.get_Name(),
				"_",
				actionIndex,
				"_",
				text
			});
			region.set_x(region.get_x() + (SkillEditor.Window.get_position().get_x() + SkillEditor.Inspector.View.get_x()));
			region.set_y(region.get_y() + (SkillEditor.Window.get_position().get_y() + SkillEditor.Inspector.View.get_y() + 43f));
			DocHelpers.CaptureRegion(region, "C:\\ActionScreens\\SampleScreens\\", actionName);
			DocHelpers.sw.WriteLine("<tr>");
			string text2 = "https://hutonggames.fogbugz.com/default.asp?";
			int wikiPageNumber = EditorCommands.GetWikiPageNumber(text);
			if (wikiPageNumber > 0)
			{
				text2 = text2 + "W" + wikiPageNumber;
			}
			else
			{
				text2 = text2 + "ixWiki=1&pg=pgSearchWiki&qWiki=" + text;
			}
			DocHelpers.sw.WriteLine("<td width=\"301px\"><a href = \"" + text2 + "\">");
			DocHelpers.sw.WriteLine("<div id=\"actionSample\"><img src=\"http://hutonggames.com/docs/img/" + actionName + ".png\" title=\"\" /></div>");
			DocHelpers.sw.WriteLine("</a></td>");
			DocHelpers.sw.WriteLine("<td><p><strong>" + Labels.NicifyVariableName(text) + "</strong></p>");
			DocHelpers.sw.WriteLine("<p>TODO</p></td>");
			DocHelpers.sw.WriteLine("</tr>");
		}
		public static void EndStateActionListCapture()
		{
			if (DocHelpers.sw == null)
			{
				return;
			}
			DocHelpers.sw.WriteLine("</table>");
			DocHelpers.sw.WriteLine("</div>");
			DocHelpers.sw.Close();
		}
	}
}
