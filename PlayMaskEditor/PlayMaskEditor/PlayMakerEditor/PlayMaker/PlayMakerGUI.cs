using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("PlayMaker/PlayMakerGUI"), ExecuteInEditMode]
public class PlayMakerGUI : MonoBehaviour
{
	private const float MaxLabelWidth = 200f;
	private static readonly List<PlayMakerFSM> fsmList = new List<PlayMakerFSM>();
	public static Skill SelectedFSM;
	private static readonly GUIContent labelContent = new GUIContent();
	public bool previewOnGUI = true;
	public bool enableGUILayout = true;
	public bool drawStateLabels = true;
	public bool GUITextureStateLabels = true;
	public bool GUITextStateLabels = true;
	public bool filterLabelsWithDistance;
	public float maxLabelDistance = 10f;
	public bool controlMouseCursor = true;
	public float labelScale = 1f;
	private static readonly List<PlayMakerFSM> SortedFsmList = new List<PlayMakerFSM>();
	private static GameObject labelGameObject;
	private static float fsmLabelIndex;
	private static PlayMakerGUI instance;
	private static GUISkin guiSkin;
	private static Color guiColor = Color.get_white();
	private static Color guiBackgroundColor = Color.get_white();
	private static Color guiContentColor = Color.get_white();
	private static Matrix4x4 guiMatrix = Matrix4x4.get_identity();
	private static GUIStyle stateLabelStyle;
	private static Texture2D stateLabelBackground;
	private float initLabelScale;
	public static bool EnableStateLabels
	{
		get
		{
			if (PlayMakerGUI.instance == null)
			{
				PlayMakerGUI.instance = (PlayMakerGUI)Object.FindObjectOfType(typeof(PlayMakerGUI));
			}
			return PlayMakerGUI.instance != null && PlayMakerGUI.instance.drawStateLabels;
		}
		set
		{
			if (PlayMakerGUI.instance == null)
			{
				PlayMakerGUI.instance = (PlayMakerGUI)Object.FindObjectOfType(typeof(PlayMakerGUI));
			}
			if (PlayMakerGUI.instance != null)
			{
				PlayMakerGUI.instance.drawStateLabels = value;
			}
		}
	}
	public static PlayMakerGUI Instance
	{
		get
		{
			if (PlayMakerGUI.instance == null)
			{
				PlayMakerGUI.instance = (PlayMakerGUI)Object.FindObjectOfType(typeof(PlayMakerGUI));
				if (PlayMakerGUI.instance == null)
				{
					GameObject gameObject = new GameObject("PlayMakerGUI");
					PlayMakerGUI.instance = gameObject.AddComponent<PlayMakerGUI>();
				}
			}
			return PlayMakerGUI.instance;
		}
	}
	public static bool Enabled
	{
		get
		{
			return PlayMakerGUI.instance != null && PlayMakerGUI.instance.get_enabled();
		}
	}
	public static GUISkin GUISkin
	{
		get
		{
			return PlayMakerGUI.guiSkin;
		}
		set
		{
			PlayMakerGUI.guiSkin = value;
		}
	}
	public static Color GUIColor
	{
		get
		{
			return PlayMakerGUI.guiColor;
		}
		set
		{
			PlayMakerGUI.guiColor = value;
		}
	}
	public static Color GUIBackgroundColor
	{
		get
		{
			return PlayMakerGUI.guiBackgroundColor;
		}
		set
		{
			PlayMakerGUI.guiBackgroundColor = value;
		}
	}
	public static Color GUIContentColor
	{
		get
		{
			return PlayMakerGUI.guiContentColor;
		}
		set
		{
			PlayMakerGUI.guiContentColor = value;
		}
	}
	public static Matrix4x4 GUIMatrix
	{
		get
		{
			return PlayMakerGUI.guiMatrix;
		}
		set
		{
			PlayMakerGUI.guiMatrix = value;
		}
	}
	public static Texture MouseCursor
	{
		get;
		set;
	}
	public static bool LockCursor
	{
		get;
		set;
	}
	public static bool HideCursor
	{
		get;
		set;
	}
	private void InitLabelStyle()
	{
		if (PlayMakerGUI.stateLabelBackground != null)
		{
			Object.Destroy(PlayMakerGUI.stateLabelBackground);
		}
		PlayMakerGUI.stateLabelBackground = new Texture2D(1, 1);
		PlayMakerGUI.stateLabelBackground.SetPixel(0, 0, Color.get_white());
		PlayMakerGUI.stateLabelBackground.Apply();
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.get_normal().set_background(PlayMakerGUI.stateLabelBackground);
		gUIStyle.get_normal().set_textColor(Color.get_white());
		gUIStyle.set_fontSize((int)(10f * this.labelScale));
		gUIStyle.set_alignment(3);
		gUIStyle.set_padding(new RectOffset(4, 4, 1, 1));
		PlayMakerGUI.stateLabelStyle = gUIStyle;
		this.initLabelScale = this.labelScale;
	}
	private void DrawStateLabels()
	{
		PlayMakerGUI.SortedFsmList.Clear();
		int count = PlayMakerFSM.FsmList.get_Count();
		for (int i = 0; i < count; i++)
		{
			PlayMakerFSM playMakerFSM = PlayMakerFSM.FsmList.get_Item(i);
			if (playMakerFSM.Active)
			{
				PlayMakerGUI.SortedFsmList.Add(playMakerFSM);
			}
		}
		PlayMakerGUI.SortedFsmList.Sort((PlayMakerFSM x, PlayMakerFSM y) => string.CompareOrdinal(x.get_gameObject().get_name(), y.get_gameObject().get_name()));
		PlayMakerGUI.labelGameObject = null;
		count = PlayMakerGUI.SortedFsmList.get_Count();
		for (int j = 0; j < count; j++)
		{
			PlayMakerFSM playMakerFSM2 = PlayMakerGUI.SortedFsmList.get_Item(j);
			if (playMakerFSM2.Fsm.ShowStateLabel)
			{
				this.DrawStateLabel(playMakerFSM2);
			}
		}
	}
	private void DrawStateLabel(PlayMakerFSM fsm)
	{
		if (PlayMakerGUI.stateLabelStyle == null || Math.Abs(this.initLabelScale - this.labelScale) > 0.1f)
		{
			this.InitLabelStyle();
		}
		if (Camera.get_main() == null)
		{
			return;
		}
		if (fsm.get_gameObject() == Camera.get_main())
		{
			return;
		}
		if (fsm.get_gameObject() == PlayMakerGUI.labelGameObject)
		{
			PlayMakerGUI.fsmLabelIndex += 1f;
		}
		else
		{
			PlayMakerGUI.fsmLabelIndex = 0f;
			PlayMakerGUI.labelGameObject = fsm.get_gameObject();
		}
		string text = PlayMakerGUI.GenerateStateLabel(fsm);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		Vector2 vector = default(Vector2);
		PlayMakerGUI.labelContent.set_text(text);
		Vector2 vector2 = PlayMakerGUI.stateLabelStyle.CalcSize(PlayMakerGUI.labelContent);
		vector2.x = Mathf.Clamp(vector2.x, 10f * this.labelScale, 200f * this.labelScale);
		if (this.GUITextureStateLabels && fsm.GuiTexture != null)
		{
			vector.x = fsm.get_gameObject().get_transform().get_position().x * (float)Screen.get_width() + fsm.GuiTexture.get_pixelInset().get_x();
			vector.y = fsm.get_gameObject().get_transform().get_position().y * (float)Screen.get_height() + fsm.GuiTexture.get_pixelInset().get_y();
		}
		else
		{
			if (this.GUITextStateLabels && fsm.GuiText != null)
			{
				vector.x = fsm.get_gameObject().get_transform().get_position().x * (float)Screen.get_width();
				vector.y = fsm.get_gameObject().get_transform().get_position().y * (float)Screen.get_height();
			}
			else
			{
				if (this.filterLabelsWithDistance)
				{
					float num = Vector3.Distance(Camera.get_main().get_transform().get_position(), fsm.get_transform().get_position());
					if (num > this.maxLabelDistance)
					{
						return;
					}
				}
				if (Camera.get_main().get_transform().InverseTransformPoint(fsm.get_transform().get_position()).z <= 0f)
				{
					return;
				}
				vector = Camera.get_main().WorldToScreenPoint(fsm.get_transform().get_position());
				vector.x -= vector2.x * 0.5f;
			}
		}
		vector.y = (float)Screen.get_height() - vector.y - PlayMakerGUI.fsmLabelIndex * 15f * this.labelScale;
		Color backgroundColor = GUI.get_backgroundColor();
		Color color = GUI.get_color();
		int num2 = 0;
		if (fsm.Fsm.ActiveState != null)
		{
			num2 = fsm.Fsm.ActiveState.ColorIndex;
		}
		Color color2 = PlayMakerPrefs.Colors[num2];
		GUI.set_backgroundColor(new Color(color2.r, color2.g, color2.b, 0.5f));
		GUI.set_contentColor(Color.get_white());
		GUI.Label(new Rect(vector.x, vector.y, vector2.x, vector2.y), text, PlayMakerGUI.stateLabelStyle);
		GUI.set_backgroundColor(backgroundColor);
		GUI.set_color(color);
	}
	private static string GenerateStateLabel(PlayMakerFSM fsm)
	{
		if (fsm.Fsm.ActiveState == null)
		{
			return "[DISABLED]";
		}
		return fsm.Fsm.ActiveState.Name;
	}
	private void Awake()
	{
		if (PlayMakerGUI.instance == null)
		{
			PlayMakerGUI.instance = this;
			return;
		}
		Debug.LogWarning("There should only be one PlayMakerGUI per scene!");
	}
	private void OnEnable()
	{
	}
	private void OnGUI()
	{
		base.set_useGUILayout(this.enableGUILayout);
		if (PlayMakerGUI.GUISkin != null)
		{
			GUI.set_skin(PlayMakerGUI.GUISkin);
		}
		GUI.set_color(PlayMakerGUI.GUIColor);
		GUI.set_backgroundColor(PlayMakerGUI.GUIBackgroundColor);
		GUI.set_contentColor(PlayMakerGUI.GUIContentColor);
		if (this.previewOnGUI && !Application.get_isPlaying())
		{
			PlayMakerGUI.DoEditGUI();
			return;
		}
		PlayMakerGUI.fsmList.Clear();
		PlayMakerGUI.fsmList.AddRange(PlayMakerFSM.FsmList);
		for (int i = 0; i < PlayMakerGUI.fsmList.get_Count(); i++)
		{
			PlayMakerFSM playMakerFSM = PlayMakerGUI.fsmList.get_Item(i);
			if (!(playMakerFSM == null) && playMakerFSM.Active && playMakerFSM.Fsm.ActiveState != null && !playMakerFSM.Fsm.HandleOnGUI)
			{
				this.CallOnGUI(playMakerFSM.Fsm);
				for (int j = 0; j < playMakerFSM.Fsm.SubFsmList.get_Count(); j++)
				{
					Skill fsm = playMakerFSM.Fsm.SubFsmList.get_Item(j);
					this.CallOnGUI(fsm);
				}
			}
		}
		if (Application.get_isPlaying() && Event.get_current().get_type() == 7)
		{
			Matrix4x4 matrix = GUI.get_matrix();
			GUI.set_matrix(Matrix4x4.get_identity());
			if (PlayMakerGUI.MouseCursor != null)
			{
				Rect rect = new Rect(Input.get_mousePosition().x - (float)PlayMakerGUI.MouseCursor.get_width() * 0.5f, (float)Screen.get_height() - Input.get_mousePosition().y - (float)PlayMakerGUI.MouseCursor.get_height() * 0.5f, (float)PlayMakerGUI.MouseCursor.get_width(), (float)PlayMakerGUI.MouseCursor.get_height());
				GUI.DrawTexture(rect, PlayMakerGUI.MouseCursor);
			}
			if (this.drawStateLabels && PlayMakerGUI.EnableStateLabels)
			{
				this.DrawStateLabels();
			}
			GUI.set_matrix(matrix);
			PlayMakerGUI.GUIMatrix = Matrix4x4.get_identity();
			if (this.controlMouseCursor)
			{
				Cursor.set_lockState(PlayMakerGUI.LockCursor ? 1 : 0);
				Cursor.set_visible(!PlayMakerGUI.HideCursor);
			}
		}
	}
	private void CallOnGUI(Skill fsm)
	{
		if (fsm.ActiveState != null)
		{
			SkillStateAction[] actions = fsm.ActiveState.Actions;
			SkillStateAction[] array = actions;
			for (int i = 0; i < array.Length; i++)
			{
				SkillStateAction fsmStateAction = array[i];
				if (fsmStateAction.Active)
				{
					fsmStateAction.OnGUI();
				}
			}
		}
	}
	private void OnDisable()
	{
		if (PlayMakerGUI.instance == this)
		{
			PlayMakerGUI.instance = null;
		}
	}
	private static void DoEditGUI()
	{
		if (PlayMakerGUI.SelectedFSM != null && !PlayMakerGUI.SelectedFSM.HandleOnGUI)
		{
			SkillState editState = PlayMakerGUI.SelectedFSM.EditState;
			if (editState != null && editState.IsInitialized)
			{
				SkillStateAction[] actions = editState.Actions;
				SkillStateAction[] array = actions;
				for (int i = 0; i < array.Length; i++)
				{
					SkillStateAction fsmStateAction = array[i];
					if (fsmStateAction.Enabled)
					{
						fsmStateAction.OnGUI();
					}
				}
			}
		}
	}
	public void OnApplicationQuit()
	{
		PlayMakerGUI.instance = null;
	}
}
