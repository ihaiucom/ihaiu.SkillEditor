using System;
using UnityEngine;
public class PlayMakerPrefs : ScriptableObject
{
	private static PlayMakerPrefs instance;
	private static readonly Color[] defaultColors = new Color[]
	{
		Color.get_grey(),
		new Color(0.545098066f, 0.670588255f, 0.9411765f),
		new Color(0.243137255f, 0.7607843f, 0.6901961f),
		new Color(0.431372553f, 0.7607843f, 0.243137255f),
		new Color(1f, 0.8745098f, 0.1882353f),
		new Color(1f, 0.5529412f, 0.1882353f),
		new Color(0.7607843f, 0.243137255f, 0.2509804f),
		new Color(0.545098066f, 0.243137255f, 0.7607843f)
	};
	private static readonly string[] defaultColorNames = new string[]
	{
		"Default",
		"Blue",
		"Cyan",
		"Green",
		"Yellow",
		"Orange",
		"Red",
		"Purple"
	};
	[SerializeField]
	private Color[] colors = new Color[]
	{
		Color.get_grey(),
		new Color(0.545098066f, 0.670588255f, 0.9411765f),
		new Color(0.243137255f, 0.7607843f, 0.6901961f),
		new Color(0.431372553f, 0.7607843f, 0.243137255f),
		new Color(1f, 0.8745098f, 0.1882353f),
		new Color(1f, 0.5529412f, 0.1882353f),
		new Color(0.7607843f, 0.243137255f, 0.2509804f),
		new Color(0.545098066f, 0.243137255f, 0.7607843f),
		Color.get_grey(),
		Color.get_grey(),
		Color.get_grey(),
		Color.get_grey(),
		Color.get_grey(),
		Color.get_grey(),
		Color.get_grey(),
		Color.get_grey(),
		Color.get_grey(),
		Color.get_grey(),
		Color.get_grey(),
		Color.get_grey(),
		Color.get_grey(),
		Color.get_grey(),
		Color.get_grey(),
		Color.get_grey()
	};
	[SerializeField]
	private string[] colorNames = new string[]
	{
		"Default",
		"Blue",
		"Cyan",
		"Green",
		"Yellow",
		"Orange",
		"Red",
		"Purple",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		""
	};
	private static Color[] minimapColors;
	public static PlayMakerPrefs Instance
	{
		get
		{
			if (PlayMakerPrefs.instance == null)
			{
				PlayMakerPrefs.instance = (Resources.Load("PlayMakerPrefs") as PlayMakerPrefs);
				if (PlayMakerPrefs.instance == null)
				{
					PlayMakerPrefs.instance = ScriptableObject.CreateInstance<PlayMakerPrefs>();
				}
			}
			return PlayMakerPrefs.instance;
		}
	}
	public static Color[] Colors
	{
		get
		{
			return PlayMakerPrefs.Instance.colors;
		}
		set
		{
			PlayMakerPrefs.Instance.colors = value;
		}
	}
	public static string[] ColorNames
	{
		get
		{
			return PlayMakerPrefs.Instance.colorNames;
		}
		set
		{
			PlayMakerPrefs.Instance.colorNames = value;
		}
	}
	public static Color[] MinimapColors
	{
		get
		{
			if (PlayMakerPrefs.minimapColors == null)
			{
				PlayMakerPrefs.UpdateMinimapColors();
			}
			return PlayMakerPrefs.minimapColors;
		}
	}
	public void ResetDefaultColors()
	{
		for (int i = 0; i < PlayMakerPrefs.defaultColors.Length; i++)
		{
			this.colors[i] = PlayMakerPrefs.defaultColors[i];
			this.colorNames[i] = PlayMakerPrefs.defaultColorNames[i];
		}
	}
	public static void SaveChanges()
	{
		PlayMakerPrefs.UpdateMinimapColors();
	}
	private static void UpdateMinimapColors()
	{
		PlayMakerPrefs.minimapColors = new Color[PlayMakerPrefs.Colors.Length];
		for (int i = 0; i < PlayMakerPrefs.Colors.Length; i++)
		{
			Color color = PlayMakerPrefs.Colors[i];
			PlayMakerPrefs.minimapColors[i] = new Color(color.r, color.g, color.b, 0.5f);
		}
	}
}
