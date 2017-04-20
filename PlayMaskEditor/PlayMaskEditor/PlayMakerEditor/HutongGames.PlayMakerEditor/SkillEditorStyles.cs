using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	public static class SkillEditorStyles
	{
		public enum ColorScheme
		{
			Default,
			DarkBackground,
			LightBackground
		}
		public const string toolbarSearchTextFieldStyle = "ToolbarSeachTextField";
		public const string toolbarSeachCancelButtonStyle = "ToolbarSeachCancelButton";
		public const string buttonStyle = "Button";
		public const string labelStyle = "Label";
		public const string toggleStyle = "Toggle";
		public const string boxStyle = "Box";
		public const string newline = "\n";
		public const string tab = "\t";
		public const string tab2 = "\t\t";
		public const float DefaultStateRowHeight = 16f;
		public const float StateMinWidth = 100f;
		public const float StateMaxWidth = 400f;
		public const float StateWidthPadding = 20f;
		public const float DescriptionHeight = 44f;
		public const float MaxFsmDescriptionWidth = 200f;
		private static bool usingProSkin;
		public static float StateRowHeight = 16f;
		public static readonly Color ErrorTextColor = new Color(1f, 0f, 0f);
		public static readonly Color ErrorTextColorIndie = new Color(0.8f, 0f, 0f);
		public static readonly Color[] LinkColors = new Color[]
		{
			new Color(0.1f, 0.1f, 0.1f),
			new Color(0.24f, 0.38f, 0.57f),
			new Color(0.06f, 0.44f, 0.06f),
			new Color(1f, 0f, 0f),
			new Color(1f, 0f, 0f),
			new Color(0.8f, 0.8f, 0f)
		};
		public static Color[] HighlightColors = new Color[6];
		public static readonly Color[] ActionColors = new Color[]
		{
			new Color(1f, 1f, 1f),
			new Color(1f, 1f, 1f),
			new Color(0.06f, 1f, 0.06f),
			new Color(1f, 0f, 0f),
			new Color(1f, 1f, 1f),
			new Color(0.8f, 0.8f, 0f)
		};
		public static readonly float[] LinkWidths = new float[]
		{
			1.5f,
			3f,
			3f,
			3f,
			3f,
			1.5f
		};
		private static Color graphTextColor;
		private static float initScale;
		private static bool scaleInitialized;
		private static bool initialized;
		private static Texture2D[] gameStateIcons;
		private static GUIStyle[] logTypeStyles;
		public static Color DefaultBackgroundColor = new Color(0.3f, 0.3f, 0.3f);
		public static Color LabelTextColor
		{
			get;
			private set;
		}
		public static Texture2D SelectedBG
		{
			get;
			private set;
		}
		public static GUIStyle TimelineLabelLeft
		{
			get;
			private set;
		}
		public static GUIStyle TimelineLabelRight
		{
			get;
			private set;
		}
		public static GUIStyle TimelineBarText
		{
			get;
			private set;
		}
		public static GUIStyle TimelineDebugLine
		{
			get;
			private set;
		}
		public static GUIStyle TimelineBar
		{
			get;
			private set;
		}
		public static GUIStyle DarkPreviewBg
		{
			get;
			private set;
		}
		public static GUIStyle RightAlignedLabel
		{
			get;
			private set;
		}
		public static GUIStyle ColorSwatch
		{
			get;
			private set;
		}
		public static GUIStyle CenteredLabel
		{
			get;
			private set;
		}
		public static GUIStyle LabelWithWordWrap
		{
			get;
			private set;
		}
		public static GUIStyle BoldLabelWithWordWrap
		{
			get;
			private set;
		}
		public static GUIStyle ActionPreviewTitle
		{
			get;
			private set;
		}
		public static GUIStyle ToolbarTab
		{
			get;
			private set;
		}
		public static GUIStyle TextAreaWithWordWrap
		{
			get;
			private set;
		}
		public static GUIStyle Background
		{
			get;
			private set;
		}
		public static GUIStyle InnerGlowBox
		{
			get;
			private set;
		}
		public static GUIStyle SelectionBox
		{
			get;
			private set;
		}
		public static GUIStyle DropShadowBox
		{
			get;
			private set;
		}
		public static GUIStyle SinglePixelFrame
		{
			get;
			private set;
		}
		public static GUIStyle SelectionRect
		{
			get;
			private set;
		}
		public static GUIStyle StateBox
		{
			get;
			private set;
		}
		public static GUIStyle StateTitleBox
		{
			get;
			private set;
		}
		public static GUIStyle StateTitleLongBox
		{
			get;
			private set;
		}
		public static GUIStyle TransitionBox
		{
			get;
			private set;
		}
		public static GUIStyle TransitionBoxSelected
		{
			get;
			private set;
		}
		public static GUIStyle GlobalTransitionBox
		{
			get;
			private set;
		}
		public static GUIStyle StartTransitionBox
		{
			get;
			private set;
		}
		public static GUIStyle BreakpointOff
		{
			get;
			private set;
		}
		public static GUIStyle BreakpointOn
		{
			get;
			private set;
		}
		public static Texture2D LineTexture
		{
			get;
			private set;
		}
		public static Texture2D TitleIcon
		{
			get;
			private set;
		}
		public static Texture2D LeftArrow
		{
			get;
			private set;
		}
		public static Texture2D RightArrow
		{
			get;
			private set;
		}
		public static Texture2D StartArrow
		{
			get;
			private set;
		}
		public static Texture2D GlobalArrow
		{
			get;
			private set;
		}
		public static Texture2D StateErrorIcon
		{
			get;
			private set;
		}
		public static Texture2D BroadcastIcon
		{
			get;
			private set;
		}
		public static GUIStyle Divider
		{
			get;
			private set;
		}
		public static GUIStyle DividerSequence
		{
			get;
			private set;
		}
		public static GUIStyle ActionFoldout
		{
			get;
			private set;
		}
		public static GUIStyle ActionToggle
		{
			get;
			private set;
		}
		public static GUIStyle ActionTitle
		{
			get;
			private set;
		}
		public static GUIStyle ActionTitleError
		{
			get;
			private set;
		}
		public static GUIStyle ActionTitleSelected
		{
			get;
			private set;
		}
		public static GUIStyle CategoryFoldout
		{
			get;
			private set;
		}
		public static GUIStyle VersionInfo
		{
			get;
			private set;
		}
		public static GUIStyle ActionErrorBox
		{
			get;
			private set;
		}
		public static GUIStyle EventBox
		{
			get;
			private set;
		}
		public static GUIStyle SelectedEventBox
		{
			get;
			private set;
		}
		public static GUIStyle TableRowHeader
		{
			get;
			private set;
		}
		public static GUIStyle TableRowBox
		{
			get;
			private set;
		}
		public static GUIStyle TableRowBoxNoDivider
		{
			get;
			private set;
		}
		public static GUIStyle ErrorBox
		{
			get;
			private set;
		}
		public static GUIStyle InfoBox
		{
			get;
			private set;
		}
		public static GUIStyle HintBox
		{
			get;
			private set;
		}
		public static GUIStyle MiniButton
		{
			get;
			private set;
		}
		public static GUIStyle MiniButtonPadded
		{
			get;
			private set;
		}
		public static GUIStyle ActionItem
		{
			get;
			private set;
		}
		public static GUIStyle ActionItemSelected
		{
			get;
			private set;
		}
		public static GUIStyle ActionLabel
		{
			get;
			private set;
		}
		public static GUIStyle ActionLabelSelected
		{
			get;
			private set;
		}
		public static GUIStyle ErrorCount
		{
			get;
			private set;
		}
		public static Texture2D NoErrors
		{
			get;
			private set;
		}
		public static Texture2D Errors
		{
			get;
			private set;
		}
		public static GUIStyle RightAlignedToolbarDropdown
		{
			get;
			private set;
		}
		public static GUIStyle ToolbarHeading
		{
			get;
			private set;
		}
		public static GUIStyle TableRow
		{
			get;
			private set;
		}
		public static GUIStyle TableRowSelected
		{
			get;
			private set;
		}
		public static GUIStyle TableRowCheckBox
		{
			get;
			private set;
		}
		public static GUIStyle LogoLarge
		{
			get;
			private set;
		}
		public static GUIStyle CommentBox
		{
			get;
			private set;
		}
		public static GUIStyle HintBoxTextOnly
		{
			get;
			private set;
		}
		public static GUIStyle TextArea
		{
			get;
			private set;
		}
		public static GUIStyle BoldFoldout
		{
			get;
			private set;
		}
		public static GUIStyle TableRowText
		{
			get;
			private set;
		}
		public static GUIStyle TableRowTextSelected
		{
			get;
			private set;
		}
		public static GUIStyle SelectedRow
		{
			get;
			private set;
		}
		public static GUIStyle InsertLine
		{
			get;
			private set;
		}
		public static GUIStyle LogBackground
		{
			get;
			private set;
		}
		public static GUIStyle LogLine
		{
			get;
			private set;
		}
		public static GUIStyle LogLine2
		{
			get;
			private set;
		}
		public static GUIStyle LogLineSelected
		{
			get;
			private set;
		}
		public static GUIStyle LogLineTimeline
		{
			get;
			private set;
		}
		public static GUIStyle InlineErrorIcon
		{
			get;
			set;
		}
		public static Color GuiContentErrorColor
		{
			get;
			private set;
		}
		public static Color GuiBackgroundErrorColor
		{
			get;
			private set;
		}
		public static Color MinimapFrameColor
		{
			get;
			private set;
		}
		public static Color MinimapViewRectColor
		{
			get;
			private set;
		}
		public static Color WatermarkTint
		{
			get;
			private set;
		}
		public static Color WatermarkTintSolid
		{
			get;
			private set;
		}
		public static Texture DefaultWatermark
		{
			get;
			private set;
		}
		public static GUIStyle Watermark
		{
			get;
			set;
		}
		public static GUIStyle LargeWatermarkText
		{
			get;
			private set;
		}
		public static GUIStyle LargeText
		{
			get;
			private set;
		}
		public static GUIStyle SmallWatermarkText
		{
			get;
			private set;
		}
		public static GUIStyle LargeTitleText
		{
			get;
			private set;
		}
		public static GUIStyle LargeTitleWithLogo
		{
			get;
			private set;
		}
		public static GUIStyle PlaymakerHeader
		{
			get;
			private set;
		}
		public static GUIStyle WelcomeLink
		{
			get;
			private set;
		}
		public static Texture BasicsIcon
		{
			get;
			private set;
		}
		public static Texture DocsIcon
		{
			get;
			private set;
		}
		public static Texture VideoIcon
		{
			get;
			private set;
		}
		public static Texture ForumIcon
		{
			get;
			private set;
		}
		public static Texture SamplesIcon
		{
			get;
			private set;
		}
		public static Texture PhotonIcon
		{
			get;
			private set;
		}
		public static Texture AddonsIcon
		{
			get;
			private set;
		}
		public static Texture BlackBerryAddonIcon
		{
			get;
			private set;
		}
		public static Texture WP8AddonIcon
		{
			get;
			private set;
		}
		public static Texture MetroAddonIcon
		{
			get;
			private set;
		}
		public static Texture BackButton
		{
			get;
			private set;
		}
		public static GUIStyle DefaultStateBoxStyle
		{
			get;
			private set;
		}
		public static Color ActiveHighlightColor
		{
			get
			{
				return SkillEditorStyles.HighlightColors[2];
			}
		}
		public static Color PausedHighlightColor
		{
			get
			{
				return SkillEditorStyles.HighlightColors[5];
			}
		}
		public static Color BreakpointHighlightColor
		{
			get
			{
				return SkillEditorStyles.HighlightColors[4];
			}
		}
		public static GUIStyle LogInfo
		{
			get;
			private set;
		}
		[Localizable(false)]
		public static bool UsingProSkin()
		{
			return EditorGUIUtility.get_isProSkin();
		}
		public static bool IsInitialized()
		{
			return SkillEditorStyles.initialized && SkillEditorStyles.scaleInitialized && SkillEditorStyles.usingProSkin == SkillEditorStyles.UsingProSkin() && SkillEditorStyles.LeftArrow != null;
		}
		public static void Reinitialize()
		{
			SkillEditorStyles.initialized = false;
		}
		public static void Init()
		{
			if (SkillEditorStyles.IsInitialized())
			{
				return;
			}
			SkillEditorStyles.InitCommon();
			SkillEditorStyles.usingProSkin = SkillEditorStyles.UsingProSkin();
			if (SkillEditorStyles.usingProSkin)
			{
				SkillEditorStyles.InitProSkin();
			}
			else
			{
				SkillEditorStyles.InitIndieSkin();
			}
			SkillEditorContent.Init(SkillEditorStyles.usingProSkin);
			SkillEditorStyles.InitColorScheme(FsmEditorSettings.ColorScheme);
			SkillEditorStyles.SetScale(SkillEditorStyles.initScale);
			SkillEditorStyles.initialized = true;
		}
		private static void InitCommon()
		{
			GUIStyle gUIStyle = new GUIStyle();
			gUIStyle.get_normal().set_textColor(Color.get_white());
			gUIStyle.set_alignment(3);
			gUIStyle.set_padding(new RectOffset(2, 0, 0, 1));
			SkillEditorStyles.TimelineBarText = gUIStyle;
			GUIStyle gUIStyle2 = new GUIStyle(SkillEditorStyles.TimelineBarText);
			gUIStyle2.get_normal().set_background(Files.LoadTextureFromDll("smallLeftArrow", 6, 20));
			gUIStyle2.set_border(new RectOffset(5, 0, 0, 0));
			gUIStyle2.set_padding(new RectOffset(6, 0, 0, 1));
			SkillEditorStyles.TimelineLabelLeft = gUIStyle2;
			GUIStyle gUIStyle3 = new GUIStyle(SkillEditorStyles.TimelineBarText);
			gUIStyle3.get_normal().set_background(Files.LoadTextureFromDll("smallRightArrow", 6, 20));
			gUIStyle3.set_border(new RectOffset(0, 5, 0, 0));
			gUIStyle3.set_padding(new RectOffset(0, 6, 0, 1));
			SkillEditorStyles.TimelineLabelRight = gUIStyle3;
			GUIStyle gUIStyle4 = new GUIStyle();
			gUIStyle4.get_normal().set_background(Files.LoadTextureFromDll("whiteVertical", 5, 2));
			gUIStyle4.get_normal().set_textColor(Color.get_white());
			gUIStyle4.set_fixedWidth(5f);
			SkillEditorStyles.TimelineDebugLine = gUIStyle4;
			GUIStyle gUIStyle5 = new GUIStyle();
			gUIStyle5.get_normal().set_background(Files.LoadTextureFromDll("timelineBar", 4, 16));
			gUIStyle5.set_border(new RectOffset(1, 1, 1, 1));
			SkillEditorStyles.TimelineBar = gUIStyle5;
			GUIStyle gUIStyle6 = new GUIStyle();
			gUIStyle6.get_normal().set_background(Files.LoadTextureFromDll("darkPreviewBg", 32, 32));
			gUIStyle6.set_border(new RectOffset(13, 13, 13, 13));
			SkillEditorStyles.DarkPreviewBg = gUIStyle6;
			GUIStyle gUIStyle7 = new GUIStyle("Box");
			gUIStyle7.get_normal().set_background(Files.LoadTextureFromDll("swatchBox", 16, 16));
			gUIStyle7.set_border(new RectOffset(2, 2, 2, 2));
			SkillEditorStyles.ColorSwatch = gUIStyle7;
			GUIStyle gUIStyle8 = new GUIStyle(EditorStyles.get_label());
			gUIStyle8.set_alignment(5);
			SkillEditorStyles.RightAlignedLabel = gUIStyle8;
			GUIStyle gUIStyle9 = new GUIStyle(EditorStyles.get_label());
			gUIStyle9.set_alignment(4);
			SkillEditorStyles.CenteredLabel = gUIStyle9;
			GUIStyle gUIStyle10 = new GUIStyle(EditorStyles.get_textField());
			gUIStyle10.set_wordWrap(true);
			SkillEditorStyles.TextAreaWithWordWrap = gUIStyle10;
			GUIStyle gUIStyle11 = new GUIStyle(EditorStyles.get_label());
			gUIStyle11.set_wordWrap(true);
			SkillEditorStyles.LabelWithWordWrap = gUIStyle11;
			GUIStyle gUIStyle12 = new GUIStyle(EditorStyles.get_boldLabel());
			gUIStyle12.set_wordWrap(true);
			SkillEditorStyles.BoldLabelWithWordWrap = gUIStyle12;
			GUIStyle gUIStyle13 = new GUIStyle(EditorStyles.get_textField());
			gUIStyle13.set_wordWrap(true);
			SkillEditorStyles.TextArea = gUIStyle13;
			SkillEditorStyles.ToolbarTab = new GUIStyle(EditorStyles.get_toolbarButton());
			GUIStyle gUIStyle14 = new GUIStyle(SkillEditorStyles.BoldLabelWithWordWrap);
			gUIStyle14.set_padding(new RectOffset(2, 2, -3, 5));
			SkillEditorStyles.ActionPreviewTitle = gUIStyle14;
			GUIStyle gUIStyle15 = new GUIStyle();
			gUIStyle15.get_normal().set_background(Files.LoadTextureFromDll("playMakerLogo", 256, 67));
			gUIStyle15.set_margin(new RectOffset(4, 4, 4, 8));
			gUIStyle15.set_fixedWidth(256f);
			gUIStyle15.set_fixedHeight(67f);
			SkillEditorStyles.LogoLarge = gUIStyle15;
			SkillEditorStyles.graphTextColor = new Color(1f, 1f, 1f, 0.7f);
			SkillEditorStyles.LabelTextColor = EditorStyles.get_label().get_normal().get_textColor();
			GUIStyle gUIStyle16 = new GUIStyle(EditorStyles.get_toolbarDropDown());
			gUIStyle16.set_alignment(5);
			SkillEditorStyles.RightAlignedToolbarDropdown = gUIStyle16;
			GUIStyle gUIStyle17 = new GUIStyle(EditorStyles.get_toolbarButton());
			gUIStyle17.set_alignment(3);
			SkillEditorStyles.ToolbarHeading = gUIStyle17;
			GUIStyle gUIStyle18 = new GUIStyle(EditorStyles.get_toolbarButton());
			gUIStyle18.set_padding(new RectOffset(20, 5, 2, 2));
			gUIStyle18.set_alignment(3);
			SkillEditorStyles.ErrorCount = gUIStyle18;
			SkillEditorStyles.Errors = Files.LoadTextureFromDll("errorCount", 14, 14);
			SkillEditorStyles.NoErrors = Files.LoadTextureFromDll("noErrors", 14, 14);
			GUIStyle gUIStyle19 = new GUIStyle();
			gUIStyle19.get_normal().set_background(Files.LoadTextureFromDll("graphBackground", 32, 32));
			gUIStyle19.set_border(new RectOffset(16, 16, 20, 10));
			SkillEditorStyles.Background = gUIStyle19;
			GUIStyle gUIStyle20 = new GUIStyle();
			gUIStyle20.get_normal().set_background(Files.LoadTextureFromDll("innerGlowBox", 32, 32));
			gUIStyle20.set_border(new RectOffset(14, 14, 14, 14));
			SkillEditorStyles.InnerGlowBox = gUIStyle20;
			GUIStyle gUIStyle21 = new GUIStyle();
			gUIStyle21.get_normal().set_background(Files.LoadTextureFromDll("outerGlow", 32, 32));
			gUIStyle21.set_border(new RectOffset(11, 11, 11, 11));
			gUIStyle21.set_margin(new RectOffset(3, 3, 3, 3));
			gUIStyle21.set_overflow(new RectOffset(10, 10, 10, 10));
			SkillEditorStyles.SelectionBox = gUIStyle21;
			GUIStyle gUIStyle22 = new GUIStyle();
			gUIStyle22.get_normal().set_background(Files.LoadTextureFromDll("selectionRect", 8, 8));
			gUIStyle22.set_border(new RectOffset(3, 3, 3, 3));
			SkillEditorStyles.SelectionRect = gUIStyle22;
			GUIStyle gUIStyle23 = new GUIStyle();
			gUIStyle23.get_normal().set_background(Files.LoadTextureFromDll("dropShadowBox", 64, 64));
			gUIStyle23.set_border(new RectOffset(31, 31, 16, 16));
			gUIStyle23.set_margin(new RectOffset(3, 3, 3, 3));
			gUIStyle23.set_overflow(new RectOffset(15, 15, 15, 15));
			SkillEditorStyles.DropShadowBox = gUIStyle23;
			GUIStyle gUIStyle24 = new GUIStyle();
			gUIStyle24.get_normal().set_background(Files.LoadTextureFromDll("stateBox", 16, 16));
			gUIStyle24.set_border(new RectOffset(2, 2, 2, 2));
			gUIStyle24.set_overflow(new RectOffset(1, 1, 1, 1));
			SkillEditorStyles.StateBox = gUIStyle24;
			GUIStyle gUIStyle25 = new GUIStyle();
			gUIStyle25.get_normal().set_background(Files.LoadTextureFromDll("stateTitleBox", 16, 16));
			gUIStyle25.get_normal().set_textColor(SkillEditorStyles.graphTextColor);
			gUIStyle25.set_border(new RectOffset(1, 1, 1, 1));
			gUIStyle25.set_alignment(4);
			gUIStyle25.set_fontStyle(1);
			gUIStyle25.set_fontSize(12);
			gUIStyle25.set_contentOffset(new Vector2(0f, -1f));
			gUIStyle25.set_fixedHeight(SkillEditorStyles.StateRowHeight);
			SkillEditorStyles.StateTitleBox = gUIStyle25;
			SkillEditorStyles.DefaultStateBoxStyle = new GUIStyle(SkillEditorStyles.StateTitleBox);
			GUIStyle gUIStyle26 = new GUIStyle(SkillEditorStyles.StateTitleBox);
			gUIStyle26.set_alignment(3);
			SkillEditorStyles.StateTitleLongBox = gUIStyle26;
			GUIStyle gUIStyle27 = new GUIStyle();
			gUIStyle27.get_normal().set_background(Files.LoadTextureFromDll("transitionBox", 16, 16));
			gUIStyle27.get_normal().set_textColor(Color.get_white());
			gUIStyle27.set_border(new RectOffset(2, 2, 2, 2));
			gUIStyle27.set_fixedHeight(SkillEditorStyles.StateRowHeight);
			gUIStyle27.set_alignment(4);
			SkillEditorStyles.TransitionBox = gUIStyle27;
			GUIStyle gUIStyle28 = new GUIStyle(SkillEditorStyles.TransitionBox);
			gUIStyle28.get_normal().set_background(Files.LoadTextureFromDll("transitionBoxSelected", 16, 16));
			gUIStyle28.get_normal().set_textColor(SkillEditorStyles.graphTextColor);
			SkillEditorStyles.TransitionBoxSelected = gUIStyle28;
			GUIStyle gUIStyle29 = new GUIStyle(SkillEditorStyles.TransitionBox);
			gUIStyle29.get_normal().set_background(Files.LoadTextureFromDll("globalTransitionBox", 16, 16));
			gUIStyle29.get_normal().set_textColor(SkillEditorStyles.graphTextColor);
			gUIStyle29.set_fontStyle(1);
			SkillEditorStyles.GlobalTransitionBox = gUIStyle29;
			GUIStyle gUIStyle30 = new GUIStyle(SkillEditorStyles.GlobalTransitionBox);
			gUIStyle30.get_normal().set_background(Files.LoadTextureFromDll("startTransitionBox", 16, 16));
			SkillEditorStyles.StartTransitionBox = gUIStyle30;
			GUIStyle gUIStyle31 = new GUIStyle();
			gUIStyle31.get_normal().set_background(Files.LoadTextureFromDll("singlePixelFrame", 16, 16));
			gUIStyle31.set_border(new RectOffset(8, 8, 8, 8));
			gUIStyle31.set_padding(new RectOffset(0, 0, -10, 0));
			SkillEditorStyles.SinglePixelFrame = gUIStyle31;
			GUIStyle gUIStyle32 = new GUIStyle();
			gUIStyle32.get_normal().set_background(Files.LoadTextureFromDll("breakpointOff", 5, 16));
			SkillEditorStyles.BreakpointOff = gUIStyle32;
			GUIStyle gUIStyle33 = new GUIStyle();
			gUIStyle33.get_normal().set_background(Files.LoadTextureFromDll("breakpointOn", 5, 16));
			SkillEditorStyles.BreakpointOn = gUIStyle33;
			SkillEditorStyles.TitleIcon = Files.LoadTextureFromDll("wanIcon", 20, 20);
			SkillEditorStyles.LineTexture = Files.LoadTextureFromDll("line", 2, 4);
			SkillEditorStyles.LeftArrow = Files.LoadTextureFromDll("leftArrow", 23, 14);
			SkillEditorStyles.RightArrow = Files.LoadTextureFromDll("rightArrow", 23, 14);
			SkillEditorStyles.StartArrow = Files.LoadTextureFromDll("startArrow", 28, 64);
			SkillEditorStyles.GlobalArrow = Files.LoadTextureFromDll("globalArrow", 16, 32);
			SkillEditorStyles.StateErrorIcon = Files.LoadTextureFromDll("errorCount", 14, 14);
			SkillEditorStyles.BroadcastIcon = Files.LoadTextureFromDll("broadcastIcon", 16, 16);
			SkillEditorStyles.gameStateIcons = new Texture2D[]
			{
				default(Texture2D),
				Files.LoadTextureFromDll("playIcon", 64, 64),
				Files.LoadTextureFromDll("breakIcon", 64, 64),
				Files.LoadTextureFromDll("pauseIcon", 64, 64),
				Files.LoadTextureFromDll("errorIcon", 64, 64)
			};
			GUIStyle gUIStyle34 = new GUIStyle();
			gUIStyle34.get_normal().set_background(Files.LoadTextureFromDll("infoBox", 16, 16));
			gUIStyle34.get_normal().set_textColor(SkillEditorStyles.graphTextColor);
			gUIStyle34.set_border(new RectOffset(2, 2, 2, 2));
			gUIStyle34.set_padding(new RectOffset(5, 5, 3, 3));
			gUIStyle34.set_margin(new RectOffset(3, 3, 3, 3));
			gUIStyle34.set_alignment(0);
			gUIStyle34.set_wordWrap(true);
			gUIStyle34.set_font(EditorStyles.get_standardFont());
			SkillEditorStyles.CommentBox = gUIStyle34;
			GUIStyle gUIStyle35 = new GUIStyle();
			gUIStyle35.get_normal().set_background(Files.LoadTextureFromDll("divider", 32, 2));
			gUIStyle35.set_border(new RectOffset(1, 1, 2, 0));
			gUIStyle35.set_fixedHeight(2f);
			SkillEditorStyles.Divider = gUIStyle35;
			GUIStyle gUIStyle36 = new GUIStyle();
			gUIStyle36.get_normal().set_background(Files.LoadTextureFromDll("dividerSequence", 42, 10));
			gUIStyle36.set_border(new RectOffset(37, 0, 0, 0));
			gUIStyle36.set_fixedHeight(10f);
			SkillEditorStyles.DividerSequence = gUIStyle36;
			GUIStyle gUIStyle37 = new GUIStyle(EditorStyles.get_miniButton());
			gUIStyle37.set_overflow(new RectOffset(0, 0, 0, 2));
			gUIStyle37.set_padding(new RectOffset(0, 0, 0, 0));
			gUIStyle37.set_margin(new RectOffset(0, 0, 3, 2));
			gUIStyle37.set_stretchWidth(false);
			gUIStyle37.set_stretchHeight(false);
			SkillEditorStyles.MiniButton = gUIStyle37;
			GUIStyle gUIStyle38 = new GUIStyle(EditorStyles.get_miniButton());
			gUIStyle38.set_overflow(new RectOffset(0, 0, 0, 2));
			gUIStyle38.set_padding(new RectOffset(0, 0, 0, 0));
			gUIStyle38.set_stretchWidth(false);
			gUIStyle38.set_stretchHeight(false);
			SkillEditorStyles.MiniButtonPadded = gUIStyle38;
			GUIStyle gUIStyle39 = new GUIStyle(EditorStyles.get_foldout());
			gUIStyle39.set_fixedWidth(15f);
			gUIStyle39.set_margin(new RectOffset(2, 0, -2, 0));
			SkillEditorStyles.ActionFoldout = gUIStyle39;
			GUIStyle gUIStyle40 = new GUIStyle(EditorStyles.get_toggle());
			gUIStyle40.set_fixedWidth(15f);
			gUIStyle40.set_margin(new RectOffset(0, 0, -2, 0));
			SkillEditorStyles.ActionToggle = gUIStyle40;
			GUIStyle gUIStyle41 = new GUIStyle(EditorStyles.get_boldLabel());
			gUIStyle41.set_padding(new RectOffset(2, 0, 2, 0));
			gUIStyle41.set_margin(new RectOffset(0, 0, 0, 0));
			gUIStyle41.set_fixedHeight(20f);
			SkillEditorStyles.ActionTitle = gUIStyle41;
			GUIStyle gUIStyle42 = new GUIStyle(SkillEditorStyles.ActionTitle);
			gUIStyle42.get_normal().set_textColor(new Color(0.7f, 0.7f, 0.7f));
			SkillEditorStyles.ActionTitleError = gUIStyle42;
			GUIStyle gUIStyle43 = new GUIStyle(EditorStyles.get_boldLabel());
			gUIStyle43.get_normal().set_textColor(Color.get_white());
			gUIStyle43.set_padding(new RectOffset(2, 0, 2, 0));
			gUIStyle43.set_margin(new RectOffset(0, 0, 0, 0));
			gUIStyle43.set_fixedHeight(20f);
			SkillEditorStyles.ActionTitleSelected = gUIStyle43;
			SkillEditorStyles.SelectedBG = Files.LoadTextureFromDll("selectedColor", 2, 2);
			GUIStyle gUIStyle44 = new GUIStyle();
			gUIStyle44.get_normal().set_background(SkillEditorStyles.SelectedBG);
			gUIStyle44.get_normal().set_textColor(Color.get_white());
			gUIStyle44.set_padding(new RectOffset(2, 0, 2, 0));
			gUIStyle44.set_margin(new RectOffset(0, 0, 0, 0));
			gUIStyle44.set_fixedHeight(20f);
			SkillEditorStyles.SelectedRow = gUIStyle44;
			GUIStyle gUIStyle45 = new GUIStyle(EditorStyles.get_foldout());
			gUIStyle45.set_fontStyle(1);
			SkillEditorStyles.CategoryFoldout = gUIStyle45;
			GUIStyle gUIStyle46 = new GUIStyle();
			gUIStyle46.get_normal().set_background(Files.LoadTextureFromDll("infoBox", 16, 16));
			gUIStyle46.get_normal().set_textColor(SkillEditorStyles.LabelTextColor);
			gUIStyle46.set_border(new RectOffset(2, 2, 2, 2));
			gUIStyle46.set_padding(new RectOffset(5, 5, 3, 3));
			gUIStyle46.set_margin(new RectOffset(5, 5, 3, 3));
			gUIStyle46.set_alignment(6);
			gUIStyle46.set_wordWrap(true);
			SkillEditorStyles.InfoBox = gUIStyle46;
			GUIStyle gUIStyle47 = new GUIStyle(SkillEditorStyles.InfoBox);
			gUIStyle47.get_normal().set_background(Files.LoadTextureFromDll("hintBox", 16, 16));
			gUIStyle47.get_normal().set_textColor(Color.get_white());
			SkillEditorStyles.HintBox = gUIStyle47;
			GUIStyle gUIStyle48 = new GUIStyle(SkillEditorStyles.InfoBox);
			gUIStyle48.get_normal().set_background(Files.LoadTextureFromDll("errorBox", 16, 16));
			gUIStyle48.get_normal().set_textColor(SkillEditorStyles.ErrorTextColor);
			SkillEditorStyles.ErrorBox = gUIStyle48;
			SkillEditorStyles.ActionErrorBox = new GUIStyle(SkillEditorStyles.ErrorBox);
			GUIStyle gUIStyle49 = new GUIStyle();
			gUIStyle49.get_normal().set_background(Files.LoadTextureFromDll("transitionBox", 16, 16));
			gUIStyle49.set_border(new RectOffset(5, 5, 5, 5));
			gUIStyle49.set_padding(new RectOffset(5, 0, 3, 0));
			gUIStyle49.set_alignment(3);
			gUIStyle49.set_fixedHeight(20f);
			SkillEditorStyles.EventBox = gUIStyle49;
			GUIStyle gUIStyle50 = new GUIStyle(SkillEditorStyles.EventBox);
			gUIStyle50.get_normal().set_background(SkillEditorStyles.SelectedBG);
			gUIStyle50.get_normal().set_textColor(Color.get_white());
			gUIStyle50.set_fixedHeight(22f);
			SkillEditorStyles.SelectedEventBox = gUIStyle50;
			GUIStyle gUIStyle51 = new GUIStyle();
			gUIStyle51.get_normal().set_background(Files.LoadTextureFromDll("tableRowBox", 16, 16));
			gUIStyle51.set_border(new RectOffset(5, 5, 5, 5));
			gUIStyle51.set_padding(new RectOffset(5, 0, 3, 0));
			gUIStyle51.set_alignment(3);
			gUIStyle51.set_fixedHeight(22f);
			SkillEditorStyles.TableRowBox = gUIStyle51;
			GUIStyle gUIStyle52 = new GUIStyle();
			gUIStyle52.set_border(new RectOffset(5, 5, 5, 5));
			gUIStyle52.set_padding(new RectOffset(5, 0, 3, 0));
			gUIStyle52.set_alignment(3);
			gUIStyle52.set_fixedHeight(22f);
			SkillEditorStyles.TableRowBoxNoDivider = gUIStyle52;
			GUIStyle gUIStyle53 = new GUIStyle("Label");
			gUIStyle53.set_alignment(3);
			SkillEditorStyles.TableRowHeader = gUIStyle53;
			GUIStyle gUIStyle54 = new GUIStyle();
			gUIStyle54.set_padding(new RectOffset(5, 5, 0, 0));
			gUIStyle54.set_alignment(8);
			SkillEditorStyles.VersionInfo = gUIStyle54;
			GUIStyle gUIStyle55 = new GUIStyle();
			gUIStyle55.get_normal().set_background(Files.LoadTextureFromDll("logInfoIcon", 20, 20));
			gUIStyle55.get_normal().set_textColor(EditorStyles.get_label().get_normal().get_textColor());
			gUIStyle55.set_border(new RectOffset(20, 0, 0, 0));
			gUIStyle55.set_padding(new RectOffset(24, 0, 0, 0));
			gUIStyle55.set_margin(new RectOffset(3, 3, 0, 0));
			gUIStyle55.set_alignment(3);
			gUIStyle55.set_fixedHeight(20f);
			SkillEditorStyles.LogInfo = gUIStyle55;
			GUIStyle gUIStyle56 = new GUIStyle(SkillEditorStyles.LogInfo);
			GUIStyle gUIStyle57 = new GUIStyle(SkillEditorStyles.LogInfo);
			gUIStyle57.get_normal().set_background(null);
			gUIStyle57.set_fontStyle(1);
			GUIStyle gUIStyle58 = gUIStyle57;
			GUIStyle gUIStyle59 = new GUIStyle(gUIStyle58);
			GUIStyle gUIStyle60 = new GUIStyle(SkillEditorStyles.LogInfo);
			gUIStyle60.get_normal().set_background(Files.LoadTextureFromDll("logWarningIcon", 20, 20));
			GUIStyle gUIStyle61 = gUIStyle60;
			GUIStyle gUIStyle62 = new GUIStyle(SkillEditorStyles.LogInfo);
			gUIStyle62.get_normal().set_background(Files.LoadTextureFromDll("logErrorIcon", 20, 20));
			GUIStyle gUIStyle63 = gUIStyle62;
			GUIStyle gUIStyle64 = new GUIStyle(SkillEditorStyles.LogInfo);
			gUIStyle64.get_normal().set_background(Files.LoadTextureFromDll("logTransitionIcon", 20, 20));
			GUIStyle gUIStyle65 = gUIStyle64;
			SkillEditorStyles.logTypeStyles = new GUIStyle[]
			{
				SkillEditorStyles.LogInfo,
				gUIStyle61,
				gUIStyle63,
				SkillEditorStyles.LogInfo,
				gUIStyle65,
				SkillEditorStyles.LogInfo,
				gUIStyle65,
				SkillEditorStyles.LogInfo,
				gUIStyle56,
				gUIStyle58,
				gUIStyle59
			};
			GUIStyle gUIStyle66 = new GUIStyle();
			gUIStyle66.get_normal().set_textColor(EditorStyles.get_label().get_normal().get_textColor());
			gUIStyle66.set_padding(new RectOffset(16, 0, 0, 0));
			gUIStyle66.set_margin(new RectOffset(3, 4, 0, 0));
			gUIStyle66.set_alignment(0);
			gUIStyle66.set_fixedHeight(16f);
			SkillEditorStyles.ActionItem = gUIStyle66;
			GUIStyle gUIStyle67 = new GUIStyle(SkillEditorStyles.ActionItem);
			gUIStyle67.get_normal().set_background(SkillEditorStyles.SelectedBG);
			gUIStyle67.get_normal().set_textColor(Color.get_white());
			SkillEditorStyles.ActionItemSelected = gUIStyle67;
			GUIStyle gUIStyle68 = new GUIStyle(EditorStyles.get_label());
			gUIStyle68.set_margin(new RectOffset(3, 4, 0, 0));
			gUIStyle68.set_alignment(0);
			gUIStyle68.set_fixedHeight(16f);
			SkillEditorStyles.ActionLabel = gUIStyle68;
			GUIStyle gUIStyle69 = new GUIStyle(SkillEditorStyles.ActionLabel);
			gUIStyle69.get_normal().set_textColor(Color.get_white());
			SkillEditorStyles.ActionLabelSelected = gUIStyle69;
			GUIStyle gUIStyle70 = new GUIStyle();
			gUIStyle70.get_normal().set_textColor(EditorStyles.get_label().get_normal().get_textColor());
			gUIStyle70.set_padding(new RectOffset(1, 0, 3, 2));
			gUIStyle70.set_fixedHeight(18f);
			SkillEditorStyles.TableRow = gUIStyle70;
			GUIStyle gUIStyle71 = new GUIStyle(SkillEditorStyles.TableRow);
			gUIStyle71.get_normal().set_background(SkillEditorStyles.SelectedBG);
			gUIStyle71.get_normal().set_textColor(Color.get_white());
			SkillEditorStyles.TableRowSelected = gUIStyle71;
			SkillEditorStyles.TableRowText = new GUIStyle("Label");
			GUIStyle gUIStyle72 = new GUIStyle(SkillEditorStyles.TableRowText);
			gUIStyle72.get_normal().set_textColor(Color.get_white());
			SkillEditorStyles.TableRowTextSelected = gUIStyle72;
			GUIStyle gUIStyle73 = new GUIStyle("Toggle");
			gUIStyle73.set_padding(new RectOffset(0, 0, 0, 0));
			gUIStyle73.set_margin(new RectOffset(4, 0, 1, 0));
			SkillEditorStyles.TableRowCheckBox = gUIStyle73;
			GUIStyle gUIStyle74 = new GUIStyle(EditorStyles.get_foldout());
			gUIStyle74.set_fontStyle(1);
			SkillEditorStyles.BoldFoldout = gUIStyle74;
			GUIStyle gUIStyle75 = new GUIStyle();
			gUIStyle75.get_normal().set_background(Files.LoadTextureFromDll("logEntryBox", 16, 16));
			gUIStyle75.get_normal().set_textColor(Color.get_white());
			SkillEditorStyles.LogBackground = gUIStyle75;
			GUIStyle gUIStyle76 = new GUIStyle();
			gUIStyle76.get_normal().set_textColor(Color.get_white());
			gUIStyle76.set_padding(new RectOffset(1, 0, 3, 2));
			gUIStyle76.set_fixedHeight(18f);
			SkillEditorStyles.LogLine = gUIStyle76;
			GUIStyle gUIStyle77 = new GUIStyle(SkillEditorStyles.LogLine);
			gUIStyle77.set_padding(new RectOffset(27, 0, 3, 2));
			SkillEditorStyles.LogLine2 = gUIStyle77;
			GUIStyle gUIStyle78 = new GUIStyle(SkillEditorStyles.LogLine);
			gUIStyle78.get_normal().set_background(SkillEditorStyles.SelectedBG);
			gUIStyle78.get_normal().set_textColor(Color.get_white());
			SkillEditorStyles.LogLineSelected = gUIStyle78;
			GUIStyle gUIStyle79 = new GUIStyle();
			gUIStyle79.get_normal().set_background(Files.LoadTextureFromDll("yellow", 2, 6));
			gUIStyle79.get_normal().set_textColor(Color.get_white());
			gUIStyle79.set_fixedHeight(6f);
			SkillEditorStyles.LogLineTimeline = gUIStyle79;
			GUIStyle gUIStyle80 = new GUIStyle();
			gUIStyle80.get_normal().set_background(Files.LoadTextureFromDll("pasteDivider", 2, 2));
			gUIStyle80.get_normal().set_textColor(Color.get_white());
			gUIStyle80.set_fixedHeight(2f);
			SkillEditorStyles.InsertLine = gUIStyle80;
			SkillEditorStyles.DefaultWatermark = Files.LoadTextureFromDll("playMakerWatermark", 256, 256);
			GUIStyle gUIStyle81 = new GUIStyle();
			gUIStyle81.set_alignment(8);
			SkillEditorStyles.Watermark = gUIStyle81;
			GUIStyle gUIStyle82 = new GUIStyle(EditorStyles.get_label());
			gUIStyle82.get_normal().set_textColor(new Color(1f, 1f, 1f, 0.1f));
			gUIStyle82.set_fontSize(32);
			gUIStyle82.set_fontStyle(1);
			SkillEditorStyles.LargeWatermarkText = gUIStyle82;
			GUIStyle gUIStyle83 = new GUIStyle();
			gUIStyle83.get_normal().set_textColor(new Color(1f, 1f, 1f, 0.15f));
			gUIStyle83.set_padding(new RectOffset(5, 0, 0, 0));
			gUIStyle83.set_font(EditorStyles.get_standardFont());
			gUIStyle83.set_fontSize(14);
			gUIStyle83.set_fontStyle(0);
			gUIStyle83.set_wordWrap(true);
			SkillEditorStyles.SmallWatermarkText = gUIStyle83;
			GUIStyle gUIStyle84 = new GUIStyle(EditorStyles.get_label());
			gUIStyle84.set_fontSize(32);
			gUIStyle84.set_fontStyle(1);
			SkillEditorStyles.LargeTitleText = gUIStyle84;
			GUIStyle gUIStyle85 = new GUIStyle();
			gUIStyle85.get_normal().set_textColor(Color.get_white());
			gUIStyle85.set_fontSize(32);
			gUIStyle85.set_fontStyle(1);
			SkillEditorStyles.LargeText = gUIStyle85;
			GUIStyle gUIStyle86 = new GUIStyle();
			gUIStyle86.get_normal().set_background(Files.LoadTextureFromDll("wanLarge", 42, 42));
			gUIStyle86.get_normal().set_textColor(Color.get_white());
			gUIStyle86.set_border(new RectOffset(42, 0, 0, 0));
			gUIStyle86.set_padding(new RectOffset(42, 0, 0, 0));
			gUIStyle86.set_margin(new RectOffset(0, 0, 0, 0));
			gUIStyle86.set_contentOffset(new Vector2(0f, 0f));
			gUIStyle86.set_alignment(3);
			gUIStyle86.set_fixedHeight(42f);
			gUIStyle86.set_fontSize(32);
			gUIStyle86.set_fontStyle(1);
			SkillEditorStyles.LargeTitleWithLogo = gUIStyle86;
			GUIStyle gUIStyle87 = new GUIStyle();
			gUIStyle87.get_normal().set_background(Files.LoadTextureFromDll("playMakerHeader", 253, 60));
			gUIStyle87.get_normal().set_textColor(Color.get_white());
			gUIStyle87.set_border(new RectOffset(253, 0, 0, 0));
			SkillEditorStyles.PlaymakerHeader = gUIStyle87;
			GUIStyle gUIStyle88 = new GUIStyle();
			gUIStyle88.get_normal().set_textColor(EditorStyles.get_label().get_normal().get_textColor());
			gUIStyle88.set_border(new RectOffset(64, 0, 0, 0));
			gUIStyle88.set_padding(new RectOffset(66, 0, 0, 0));
			gUIStyle88.set_margin(new RectOffset(20, 20, 20, 0));
			gUIStyle88.set_alignment(0);
			gUIStyle88.set_fixedHeight(64f);
			SkillEditorStyles.WelcomeLink = gUIStyle88;
			SkillEditorStyles.DocsIcon = Files.LoadTextureFromDll("linkDocs", 48, 48);
			SkillEditorStyles.BasicsIcon = Files.LoadTextureFromDll("linkBasics", 64, 64);
			SkillEditorStyles.VideoIcon = Files.LoadTextureFromDll("linkVideos", 48, 48);
			SkillEditorStyles.ForumIcon = Files.LoadTextureFromDll("linkForums", 48, 48);
			SkillEditorStyles.SamplesIcon = Files.LoadTextureFromDll("linkSamples", 48, 48);
			SkillEditorStyles.PhotonIcon = Files.LoadTextureFromDll("photonIcon", 48, 48);
			SkillEditorStyles.BlackBerryAddonIcon = Files.LoadTextureFromDll("bb10Icon", 48, 48);
			SkillEditorStyles.WP8AddonIcon = Files.LoadTextureFromDll("wp8Icon", 48, 48);
			SkillEditorStyles.MetroAddonIcon = Files.LoadTextureFromDll("metroIcon", 48, 48);
			SkillEditorStyles.AddonsIcon = Files.LoadTextureFromDll("linkAddons", 48, 48);
			SkillEditorStyles.BackButton = Files.LoadTextureFromDll("backButton", 123, 24);
			GUIStyle gUIStyle89 = new GUIStyle();
			gUIStyle89.set_padding(new RectOffset(2, 0, 2, 0));
			SkillEditorStyles.InlineErrorIcon = gUIStyle89;
		}
		private static void InitProSkin()
		{
			SkillEditorStyles.BroadcastIcon = Files.LoadTextureFromDll("broadcastIcon", 16, 16);
			SkillEditorStyles.GuiContentErrorColor = new Color(1f, 0.1f, 0.1f);
			SkillEditorStyles.GuiBackgroundErrorColor = new Color(1f, 0.4f, 0.4f);
			GUIStyle gUIStyle = new GUIStyle(SkillEditorStyles.InfoBox);
			gUIStyle.get_normal().set_background(Files.LoadTextureFromDll("hintBox", 16, 16));
			gUIStyle.get_normal().set_textColor(new Color(0.6f, 0.7f, 0.8f));
			SkillEditorStyles.HintBox = gUIStyle;
			GUIStyle gUIStyle2 = new GUIStyle(SkillEditorStyles.HintBox);
			gUIStyle2.get_normal().set_background(null);
			SkillEditorStyles.HintBoxTextOnly = gUIStyle2;
		}
		private static void InitIndieSkin()
		{
			SkillEditorStyles.ErrorBox.get_normal().set_background(SkillEditorStyles.InfoBox.get_normal().get_background());
			SkillEditorStyles.ErrorBox.get_normal().set_textColor(SkillEditorStyles.ErrorTextColorIndie);
			SkillEditorStyles.ActionErrorBox.get_normal().set_textColor(SkillEditorStyles.ErrorTextColorIndie);
			SkillEditorStyles.BroadcastIcon = Files.LoadTextureFromDll("broadcastIcon_indie", 16, 16);
			SkillEditorStyles.SelectedBG = Files.LoadTextureFromDll("selectedColor_indie", 2, 2);
			SkillEditorStyles.SelectedRow.get_normal().set_background(SkillEditorStyles.SelectedBG);
			SkillEditorStyles.ActionItemSelected.get_normal().set_background(SkillEditorStyles.SelectedBG);
			SkillEditorStyles.SelectedEventBox.get_normal().set_background(SkillEditorStyles.SelectedBG);
			SkillEditorStyles.TableRowSelected.get_normal().set_background(SkillEditorStyles.SelectedBG);
			SkillEditorStyles.GuiContentErrorColor = new Color(1f, 0f, 0f);
			SkillEditorStyles.GuiBackgroundErrorColor = new Color(1f, 0.3f, 0.3f);
			GUIStyle gUIStyle = new GUIStyle(SkillEditorStyles.InfoBox);
			gUIStyle.get_normal().set_background(Files.LoadTextureFromDll("hintBox", 16, 16));
			gUIStyle.get_normal().set_textColor(new Color(0.9f, 0.95f, 1f));
			SkillEditorStyles.HintBox = gUIStyle;
			GUIStyle gUIStyle2 = new GUIStyle(SkillEditorStyles.HintBox);
			gUIStyle2.get_normal().set_background(null);
			SkillEditorStyles.HintBoxTextOnly = gUIStyle2;
		}
		private static void InitColorScheme(SkillEditorStyles.ColorScheme colorScheme)
		{
			if (colorScheme == SkillEditorStyles.ColorScheme.Default)
			{
				colorScheme = (SkillEditorStyles.usingProSkin ? SkillEditorStyles.ColorScheme.DarkBackground : SkillEditorStyles.ColorScheme.LightBackground);
			}
			switch (colorScheme)
			{
			case SkillEditorStyles.ColorScheme.DarkBackground:
				SkillEditorStyles.LinkColors[0] = new Color(0.1f, 0.1f, 0.1f);
				SkillEditorStyles.LargeWatermarkText.get_normal().set_textColor(new Color(1f, 1f, 1f, 0.15f));
				SkillEditorStyles.SmallWatermarkText.get_normal().set_textColor(new Color(1f, 1f, 1f, 0.2f));
				SkillEditorStyles.MinimapViewRectColor = new Color(1f, 1f, 1f, 0.3f);
				SkillEditorStyles.MinimapFrameColor = new Color(1f, 1f, 1f, 0.05f);
				SkillEditorStyles.WatermarkTint = new Color(1f, 1f, 1f, 0.05f);
				SkillEditorStyles.WatermarkTintSolid = new Color(0.8f, 0.8f, 0.8f);
				SkillEditorStyles.HighlightColors = new Color[]
				{
					new Color(0f, 0f, 0f),
					new Color(0.24f, 0.38f, 0.57f),
					new Color(0.06f, 0.8f, 0.06f),
					new Color(1f, 0f, 0f),
					new Color(1f, 0f, 0f),
					new Color(0.8f, 0.8f, 0f)
				};
				return;
			case SkillEditorStyles.ColorScheme.LightBackground:
				SkillEditorStyles.LinkColors[0] = new Color(0.25f, 0.25f, 0.25f);
				SkillEditorStyles.Background.get_normal().set_background(Files.LoadTextureFromDll("graphBackground_indie", 32, 32));
				SkillEditorStyles.CommentBox.get_normal().set_background(Files.LoadTextureFromDll("infoBox_indie", 16, 16));
				SkillEditorStyles.GlobalTransitionBox.get_normal().set_background(Files.LoadTextureFromDll("globalTransitionBox_indie", 16, 16));
				SkillEditorStyles.StartTransitionBox.get_normal().set_background(Files.LoadTextureFromDll("startTransitionBox_indie", 16, 16));
				SkillEditorStyles.LargeWatermarkText.get_normal().set_textColor(new Color(0f, 0f, 0f, 0.2f));
				SkillEditorStyles.SmallWatermarkText.get_normal().set_textColor(new Color(0f, 0f, 0f, 0.3f));
				SkillEditorStyles.MinimapViewRectColor = new Color(1f, 1f, 1f, 0.5f);
				SkillEditorStyles.MinimapFrameColor = new Color(0f, 0f, 0f, 0.1f);
				SkillEditorStyles.WatermarkTint = new Color(0f, 0f, 0f, 0.075f);
				SkillEditorStyles.WatermarkTintSolid = new Color(0.2f, 0.2f, 0.2f);
				SkillEditorStyles.HighlightColors = new Color[]
				{
					new Color(0f, 0f, 0f),
					new Color(0.24f, 0.5f, 0.875f),
					new Color(0.06f, 0.8f, 0.06f),
					new Color(1f, 0f, 0f),
					new Color(1f, 0f, 0f),
					new Color(0.8f, 0.8f, 0f)
				};
				return;
			default:
				throw new ArgumentOutOfRangeException("colorScheme");
			}
		}
		public static Texture2D[] GetGameStateIcons()
		{
			return SkillEditorStyles.gameStateIcons;
		}
		public static GUIStyle[] GetLogTypeStyles()
		{
			return SkillEditorStyles.logTypeStyles;
		}
		public static void OnDestroy()
		{
		}
		public static void InitScale(float scale)
		{
			SkillEditorStyles.scaleInitialized = false;
			SkillEditorStyles.initScale = scale;
		}
		public static void SetScale(float scale)
		{
			int fontSize = Mathf.CeilToInt(Mathf.Clamp(scale * 12f, 4f, 12f));
			SkillEditorStyles.StateRowHeight = 16f * scale;
			SkillEditorStyles.StateTitleBox.set_fontSize(fontSize);
			SkillEditorStyles.StateTitleBox.set_fixedHeight(SkillEditorStyles.StateRowHeight);
			SkillEditorStyles.TransitionBox.set_fontSize(fontSize);
			SkillEditorStyles.TransitionBox.set_fixedHeight(SkillEditorStyles.StateRowHeight);
			SkillEditorStyles.TransitionBoxSelected.set_fontSize(fontSize);
			SkillEditorStyles.TransitionBoxSelected.set_fixedHeight(SkillEditorStyles.StateRowHeight);
			SkillEditorStyles.GlobalTransitionBox.set_fontSize(fontSize);
			SkillEditorStyles.GlobalTransitionBox.set_fixedHeight(SkillEditorStyles.StateRowHeight);
			SkillEditorStyles.StartTransitionBox.set_fontSize(fontSize);
			SkillEditorStyles.StartTransitionBox.set_fixedHeight(SkillEditorStyles.StateRowHeight);
			SkillEditorStyles.CommentBox.set_fontSize(fontSize);
			SkillEditorStyles.CommentBox.set_padding(new RectOffset((int)(5f * scale), (int)(5f * scale), (int)(3f * scale), (int)(3f * scale)));
			SkillEditorStyles.scaleInitialized = true;
		}
	}
}
