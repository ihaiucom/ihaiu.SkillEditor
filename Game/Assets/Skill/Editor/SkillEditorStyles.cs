using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace ihaiu
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
            return EditorGUIUtility.isProSkin;
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
            SkillEditorStyles.InitColorScheme(SkillEditorSettings.ColorScheme);
            SkillEditorStyles.SetScale(SkillEditorStyles.initScale);
            SkillEditorStyles.initialized = true;
        }
        private static void InitCommon()
        {
            GUIStyle gUIStyle = new GUIStyle();
            gUIStyle.normal.textColor = Color.white;
            gUIStyle.alignment = TextAnchor.MiddleLeft;
            gUIStyle.padding = new RectOffset(2, 0, 0, 1);
            SkillEditorStyles.TimelineBarText = gUIStyle;

            GUIStyle gUIStyle2 = new GUIStyle(SkillEditorStyles.TimelineBarText);
            gUIStyle2.normal.background = Files.LoadTextureFromDll("smallLeftArrow", 6, 20);

            gUIStyle2.normal.background = (Files.LoadTextureFromDll("smallLeftArrow", 6, 20));
            gUIStyle2.border = (new RectOffset(5, 0, 0, 0));
            gUIStyle2.padding = (new RectOffset(6, 0, 0, 1));
            SkillEditorStyles.TimelineLabelLeft = gUIStyle2;

            GUIStyle gUIStyle3 = new GUIStyle(SkillEditorStyles.TimelineBarText);
            gUIStyle3.normal.background = (Files.LoadTextureFromDll("smallRightArrow", 6, 20));
            gUIStyle3.border = (new RectOffset(0, 5, 0, 0));
            gUIStyle3.padding = (new RectOffset(0, 6, 0, 1));
            SkillEditorStyles.TimelineLabelRight = gUIStyle3;
            GUIStyle gUIStyle4 = new GUIStyle();
            gUIStyle4.normal.background = (Files.LoadTextureFromDll("whiteVertical", 5, 2));
            gUIStyle4.normal.textColor = Color.white;
            gUIStyle4.fixedWidth = (5f);
            SkillEditorStyles.TimelineDebugLine = gUIStyle4;
            GUIStyle gUIStyle5 = new GUIStyle();
            gUIStyle5.normal.background = (Files.LoadTextureFromDll("timelineBar", 4, 16));
            gUIStyle5.border = (new RectOffset(1, 1, 1, 1));
            SkillEditorStyles.TimelineBar = gUIStyle5;
            GUIStyle gUIStyle6 = new GUIStyle();
            gUIStyle6.normal.background = (Files.LoadTextureFromDll("darkPreviewBg", 32, 32));
            gUIStyle6.border = (new RectOffset(13, 13, 13, 13));
            SkillEditorStyles.DarkPreviewBg = gUIStyle6;
            GUIStyle gUIStyle7 = new GUIStyle("Box");
            gUIStyle7.normal.background = (Files.LoadTextureFromDll("swatchBox", 16, 16));
            gUIStyle7.border = (new RectOffset(2, 2, 2, 2));
            SkillEditorStyles.ColorSwatch = gUIStyle7;
            GUIStyle gUIStyle8 = new GUIStyle(EditorStyles.label);
            gUIStyle8.alignment = TextAnchor.MiddleRight;
            SkillEditorStyles.RightAlignedLabel = gUIStyle8;
            GUIStyle gUIStyle9 = new GUIStyle(EditorStyles.label);
            gUIStyle9.alignment = TextAnchor.MiddleCenter;
            SkillEditorStyles.CenteredLabel = gUIStyle9;
            GUIStyle gUIStyle10 = new GUIStyle(EditorStyles.textField);
            gUIStyle10.wordWrap = (true);
            SkillEditorStyles.TextAreaWithWordWrap = gUIStyle10;
            GUIStyle gUIStyle11 = new GUIStyle(EditorStyles.label);
            gUIStyle11.wordWrap = (true);
            SkillEditorStyles.LabelWithWordWrap = gUIStyle11;
            GUIStyle gUIStyle12 = new GUIStyle(EditorStyles.boldLabel);
            gUIStyle12.wordWrap = (true);
            SkillEditorStyles.BoldLabelWithWordWrap = gUIStyle12;
            GUIStyle gUIStyle13 = new GUIStyle(EditorStyles.textField);
            gUIStyle13.wordWrap = (true);
            SkillEditorStyles.TextArea = gUIStyle13;
            SkillEditorStyles.ToolbarTab = new GUIStyle(EditorStyles.toolbarButton);
            GUIStyle gUIStyle14 = new GUIStyle(SkillEditorStyles.BoldLabelWithWordWrap);
            gUIStyle14.padding = (new RectOffset(2, 2, -3, 5));
            SkillEditorStyles.ActionPreviewTitle = gUIStyle14;
            GUIStyle gUIStyle15 = new GUIStyle();
            gUIStyle15.normal.background = (Files.LoadTextureFromDll("playMakerLogo", 256, 67));
            gUIStyle15.margin = (new RectOffset(4, 4, 4, 8));
            gUIStyle15.fixedWidth = (256f);
            gUIStyle15.fixedHeight = (67f);
            SkillEditorStyles.LogoLarge = gUIStyle15;
            SkillEditorStyles.graphTextColor = new Color(1f, 1f, 1f, 0.7f);
            SkillEditorStyles.LabelTextColor = EditorStyles.label.normal.textColor;
            GUIStyle gUIStyle16 = new GUIStyle(EditorStyles.toolbarDropDown);
            gUIStyle16.alignment = TextAnchor.MiddleRight;
            SkillEditorStyles.RightAlignedToolbarDropdown = gUIStyle16;
            GUIStyle gUIStyle17 = new GUIStyle(EditorStyles.toolbarButton);
            gUIStyle17.alignment = TextAnchor.MiddleLeft;
            SkillEditorStyles.ToolbarHeading = gUIStyle17;
            GUIStyle gUIStyle18 = new GUIStyle(EditorStyles.toolbarButton);
            gUIStyle18.padding = (new RectOffset(20, 5, 2, 2));
            gUIStyle18.alignment = TextAnchor.MiddleLeft;
            SkillEditorStyles.ErrorCount = gUIStyle18;
            SkillEditorStyles.Errors = Files.LoadTextureFromDll("errorCount", 14, 14);
            SkillEditorStyles.NoErrors = Files.LoadTextureFromDll("noErrors", 14, 14);
            GUIStyle gUIStyle19 = new GUIStyle();
            gUIStyle19.normal.background = (Files.LoadTextureFromDll("graphBackground", 32, 32));
            gUIStyle19.border = (new RectOffset(16, 16, 20, 10));
            SkillEditorStyles.Background = gUIStyle19;
            GUIStyle gUIStyle20 = new GUIStyle();
            gUIStyle20.normal.background = (Files.LoadTextureFromDll("innerGlowBox", 32, 32));
            gUIStyle20.border = (new RectOffset(14, 14, 14, 14));
            SkillEditorStyles.InnerGlowBox = gUIStyle20;
            GUIStyle gUIStyle21 = new GUIStyle();
            gUIStyle21.normal.background = (Files.LoadTextureFromDll("outerGlow", 32, 32));
            gUIStyle21.border = (new RectOffset(11, 11, 11, 11));
            gUIStyle21.margin = (new RectOffset(3, 3, 3, 3));
            gUIStyle21.overflow = (new RectOffset(10, 10, 10, 10));
            SkillEditorStyles.SelectionBox = gUIStyle21;
            GUIStyle gUIStyle22 = new GUIStyle();
            gUIStyle22.normal.background = (Files.LoadTextureFromDll("selectionRect", 8, 8));
            gUIStyle22.border = (new RectOffset(3, 3, 3, 3));
            SkillEditorStyles.SelectionRect = gUIStyle22;
            GUIStyle gUIStyle23 = new GUIStyle();
            gUIStyle23.normal.background = (Files.LoadTextureFromDll("dropShadowBox", 64, 64));
            gUIStyle23.border = (new RectOffset(31, 31, 16, 16));
            gUIStyle23.margin = (new RectOffset(3, 3, 3, 3));
            gUIStyle23.overflow = (new RectOffset(15, 15, 15, 15));
            SkillEditorStyles.DropShadowBox = gUIStyle23;
            GUIStyle gUIStyle24 = new GUIStyle();
            gUIStyle24.normal.background = (Files.LoadTextureFromDll("stateBox", 16, 16));
            gUIStyle24.border = (new RectOffset(2, 2, 2, 2));
            gUIStyle24.overflow = (new RectOffset(1, 1, 1, 1));
            SkillEditorStyles.StateBox = gUIStyle24;
            GUIStyle gUIStyle25 = new GUIStyle();
            gUIStyle25.normal.background = (Files.LoadTextureFromDll("stateTitleBox", 16, 16));
            gUIStyle25.normal.textColor = (SkillEditorStyles.graphTextColor);
            gUIStyle25.border = (new RectOffset(1, 1, 1, 1));
            gUIStyle25.alignment = TextAnchor.MiddleCenter;
            gUIStyle25.fontStyle = FontStyle.Bold;
            gUIStyle25.fontSize = (12);
            gUIStyle25.contentOffset = (new Vector2(0f, -1f));
            gUIStyle25.fixedHeight = (SkillEditorStyles.StateRowHeight);
            SkillEditorStyles.StateTitleBox = gUIStyle25;
            SkillEditorStyles.DefaultStateBoxStyle = new GUIStyle(SkillEditorStyles.StateTitleBox);
            GUIStyle gUIStyle26 = new GUIStyle(SkillEditorStyles.StateTitleBox);
            gUIStyle26.alignment = TextAnchor.MiddleLeft;
            SkillEditorStyles.StateTitleLongBox = gUIStyle26;
            GUIStyle gUIStyle27 = new GUIStyle();
            gUIStyle27.normal.background = (Files.LoadTextureFromDll("transitionBox", 16, 16));
            gUIStyle27.normal.textColor = (Color.white);
            gUIStyle27.border = (new RectOffset(2, 2, 2, 2));
            gUIStyle27.fixedHeight = (SkillEditorStyles.StateRowHeight);
            gUIStyle27.alignment = TextAnchor.MiddleCenter;
            SkillEditorStyles.TransitionBox = gUIStyle27;
            GUIStyle gUIStyle28 = new GUIStyle(SkillEditorStyles.TransitionBox);
            gUIStyle28.normal.background = (Files.LoadTextureFromDll("transitionBoxSelected", 16, 16));
            gUIStyle28.normal.textColor = (SkillEditorStyles.graphTextColor);
            SkillEditorStyles.TransitionBoxSelected = gUIStyle28;
            GUIStyle gUIStyle29 = new GUIStyle(SkillEditorStyles.TransitionBox);
            gUIStyle29.normal.background = (Files.LoadTextureFromDll("globalTransitionBox", 16, 16));
            gUIStyle29.normal.textColor = (SkillEditorStyles.graphTextColor);
            gUIStyle29.fontStyle = FontStyle.Bold;
            SkillEditorStyles.GlobalTransitionBox = gUIStyle29;
            GUIStyle gUIStyle30 = new GUIStyle(SkillEditorStyles.GlobalTransitionBox);
            gUIStyle30.normal.background = (Files.LoadTextureFromDll("startTransitionBox", 16, 16));
            SkillEditorStyles.StartTransitionBox = gUIStyle30;
            GUIStyle gUIStyle31 = new GUIStyle();
            gUIStyle31.normal.background = (Files.LoadTextureFromDll("singlePixelFrame", 16, 16));
            gUIStyle31.border = (new RectOffset(8, 8, 8, 8));
            gUIStyle31.padding = (new RectOffset(0, 0, -10, 0));
            SkillEditorStyles.SinglePixelFrame = gUIStyle31;
            GUIStyle gUIStyle32 = new GUIStyle();
            gUIStyle32.normal.background = (Files.LoadTextureFromDll("breakpointOff", 5, 16));
            SkillEditorStyles.BreakpointOff = gUIStyle32;
            GUIStyle gUIStyle33 = new GUIStyle();
            gUIStyle33.normal.background = (Files.LoadTextureFromDll("breakpointOn", 5, 16));
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
            gUIStyle34.normal.background = (Files.LoadTextureFromDll("infoBox", 16, 16));
            gUIStyle34.normal.textColor = (SkillEditorStyles.graphTextColor);
            gUIStyle34.border = (new RectOffset(2, 2, 2, 2));
            gUIStyle34.padding = (new RectOffset(5, 5, 3, 3));
            gUIStyle34.margin = (new RectOffset(3, 3, 3, 3));
            gUIStyle34.alignment = (0);
            gUIStyle34.wordWrap = (true);
            gUIStyle34.font = (EditorStyles.standardFont);
            SkillEditorStyles.CommentBox = gUIStyle34;
            GUIStyle gUIStyle35 = new GUIStyle();
            gUIStyle35.normal.background = (Files.LoadTextureFromDll("divider", 32, 2));
            gUIStyle35.border = (new RectOffset(1, 1, 2, 0));
            gUIStyle35.fixedHeight = (2f);
            SkillEditorStyles.Divider = gUIStyle35;
            GUIStyle gUIStyle36 = new GUIStyle();
            gUIStyle36.normal.background = (Files.LoadTextureFromDll("dividerSequence", 42, 10));
            gUIStyle36.border = (new RectOffset(37, 0, 0, 0));
            gUIStyle36.fixedHeight = (10f);
            SkillEditorStyles.DividerSequence = gUIStyle36;
            GUIStyle gUIStyle37 = new GUIStyle(EditorStyles.miniButton);
            gUIStyle37.overflow = (new RectOffset(0, 0, 0, 2));
            gUIStyle37.padding = (new RectOffset(0, 0, 0, 0));
            gUIStyle37.margin = (new RectOffset(0, 0, 3, 2));
            gUIStyle37.stretchWidth = (false);
            gUIStyle37.stretchHeight = (false);
            SkillEditorStyles.MiniButton = gUIStyle37;
            GUIStyle gUIStyle38 = new GUIStyle(EditorStyles.miniButton);
            gUIStyle38.overflow = (new RectOffset(0, 0, 0, 2));
            gUIStyle38.padding = (new RectOffset(0, 0, 0, 0));
            gUIStyle38.stretchWidth = (false);
            gUIStyle38.stretchHeight = (false);
            SkillEditorStyles.MiniButtonPadded = gUIStyle38;
            GUIStyle gUIStyle39 = new GUIStyle(EditorStyles.foldout);
            gUIStyle39.fixedWidth = (15f);
            gUIStyle39.margin = (new RectOffset(2, 0, -2, 0));
            SkillEditorStyles.ActionFoldout = gUIStyle39;
            GUIStyle gUIStyle40 = new GUIStyle(EditorStyles.toggle);
            gUIStyle40.fixedWidth = (15f);
            gUIStyle40.margin = (new RectOffset(0, 0, -2, 0));
            SkillEditorStyles.ActionToggle = gUIStyle40;
            GUIStyle gUIStyle41 = new GUIStyle(EditorStyles.boldLabel);
            gUIStyle41.padding = (new RectOffset(2, 0, 2, 0));
            gUIStyle41.margin = (new RectOffset(0, 0, 0, 0));
            gUIStyle41.fixedHeight = (20f);
            SkillEditorStyles.ActionTitle = gUIStyle41;
            GUIStyle gUIStyle42 = new GUIStyle(SkillEditorStyles.ActionTitle);
            gUIStyle42.normal.textColor = (new Color(0.7f, 0.7f, 0.7f));
            SkillEditorStyles.ActionTitleError = gUIStyle42;
            GUIStyle gUIStyle43 = new GUIStyle(EditorStyles.boldLabel);
            gUIStyle43.normal.textColor = (Color.white);
            gUIStyle43.padding = (new RectOffset(2, 0, 2, 0));
            gUIStyle43.margin = (new RectOffset(0, 0, 0, 0));
            gUIStyle43.fixedHeight = (20f);
            SkillEditorStyles.ActionTitleSelected = gUIStyle43;
            SkillEditorStyles.SelectedBG = Files.LoadTextureFromDll("selectedColor", 2, 2);
            GUIStyle gUIStyle44 = new GUIStyle();
            gUIStyle44.normal.background = (SkillEditorStyles.SelectedBG);
            gUIStyle44.normal.textColor = (Color.white);
            gUIStyle44.padding = (new RectOffset(2, 0, 2, 0));
            gUIStyle44.margin = (new RectOffset(0, 0, 0, 0));
            gUIStyle44.fixedHeight = (20f);
            SkillEditorStyles.SelectedRow = gUIStyle44;
            GUIStyle gUIStyle45 = new GUIStyle(EditorStyles.foldout);
            gUIStyle45.fontStyle = FontStyle.Bold;
            SkillEditorStyles.CategoryFoldout = gUIStyle45;
            GUIStyle gUIStyle46 = new GUIStyle();
            gUIStyle46.normal.background = (Files.LoadTextureFromDll("infoBox", 16, 16));
            gUIStyle46.normal.textColor = (SkillEditorStyles.LabelTextColor);
            gUIStyle46.border = (new RectOffset(2, 2, 2, 2));
            gUIStyle46.padding = (new RectOffset(5, 5, 3, 3));
            gUIStyle46.margin = (new RectOffset(5, 5, 3, 3));
            gUIStyle46.alignment = TextAnchor.LowerLeft;
            gUIStyle46.wordWrap = (true);
            SkillEditorStyles.InfoBox = gUIStyle46;
            GUIStyle gUIStyle47 = new GUIStyle(SkillEditorStyles.InfoBox);
            gUIStyle47.normal.background = (Files.LoadTextureFromDll("hintBox", 16, 16));
            gUIStyle47.normal.textColor = (Color.white);
            SkillEditorStyles.HintBox = gUIStyle47;
            GUIStyle gUIStyle48 = new GUIStyle(SkillEditorStyles.InfoBox);
            gUIStyle48.normal.background = (Files.LoadTextureFromDll("errorBox", 16, 16));
            gUIStyle48.normal.textColor = (SkillEditorStyles.ErrorTextColor);
            SkillEditorStyles.ErrorBox = gUIStyle48;
            SkillEditorStyles.ActionErrorBox = new GUIStyle(SkillEditorStyles.ErrorBox);
            GUIStyle gUIStyle49 = new GUIStyle();
            gUIStyle49.normal.background = (Files.LoadTextureFromDll("transitionBox", 16, 16));
            gUIStyle49.border = (new RectOffset(5, 5, 5, 5));
            gUIStyle49.padding = (new RectOffset(5, 0, 3, 0));
            gUIStyle49.alignment = TextAnchor.MiddleLeft;
            gUIStyle49.fixedHeight = (20f);
            SkillEditorStyles.EventBox = gUIStyle49;
            GUIStyle gUIStyle50 = new GUIStyle(SkillEditorStyles.EventBox);
            gUIStyle50.normal.background = (SkillEditorStyles.SelectedBG);
            gUIStyle50.normal.textColor = (Color.white);
            gUIStyle50.fixedHeight = (22f);
            SkillEditorStyles.SelectedEventBox = gUIStyle50;
            GUIStyle gUIStyle51 = new GUIStyle();
            gUIStyle51.normal.background = (Files.LoadTextureFromDll("tableRowBox", 16, 16));
            gUIStyle51.border = (new RectOffset(5, 5, 5, 5));
            gUIStyle51.padding = (new RectOffset(5, 0, 3, 0));
            gUIStyle51.alignment = TextAnchor.MiddleLeft;
            gUIStyle51.fixedHeight = (22f);
            SkillEditorStyles.TableRowBox = gUIStyle51;
            GUIStyle gUIStyle52 = new GUIStyle();
            gUIStyle52.border = (new RectOffset(5, 5, 5, 5));
            gUIStyle52.padding = (new RectOffset(5, 0, 3, 0));
            gUIStyle52.alignment = TextAnchor.MiddleLeft;
            gUIStyle52.fixedHeight = (22f);
            SkillEditorStyles.TableRowBoxNoDivider = gUIStyle52;
            GUIStyle gUIStyle53 = new GUIStyle("Label");
            gUIStyle53.alignment = TextAnchor.MiddleLeft;
            SkillEditorStyles.TableRowHeader = gUIStyle53;
            GUIStyle gUIStyle54 = new GUIStyle();
            gUIStyle54.padding = (new RectOffset(5, 5, 0, 0));
            gUIStyle54.alignment = TextAnchor.LowerRight;
            SkillEditorStyles.VersionInfo = gUIStyle54;
            GUIStyle gUIStyle55 = new GUIStyle();
            gUIStyle55.normal.background = (Files.LoadTextureFromDll("logInfoIcon", 20, 20));
            gUIStyle55.normal.textColor = (EditorStyles.label.normal.textColor);
            gUIStyle55.border = (new RectOffset(20, 0, 0, 0));
            gUIStyle55.padding = (new RectOffset(24, 0, 0, 0));
            gUIStyle55.margin = (new RectOffset(3, 3, 0, 0));
            gUIStyle55.alignment = TextAnchor.MiddleLeft;
            gUIStyle55.fixedHeight = (20f);
            SkillEditorStyles.LogInfo = gUIStyle55;
            GUIStyle gUIStyle56 = new GUIStyle(SkillEditorStyles.LogInfo);
            GUIStyle gUIStyle57 = new GUIStyle(SkillEditorStyles.LogInfo);
            gUIStyle57.normal.background = (null);
            gUIStyle57.fontStyle = FontStyle.Bold;
            GUIStyle gUIStyle58 = gUIStyle57;
            GUIStyle gUIStyle59 = new GUIStyle(gUIStyle58);
            GUIStyle gUIStyle60 = new GUIStyle(SkillEditorStyles.LogInfo);
            gUIStyle60.normal.background = (Files.LoadTextureFromDll("logWarningIcon", 20, 20));
            GUIStyle gUIStyle61 = gUIStyle60;
            GUIStyle gUIStyle62 = new GUIStyle(SkillEditorStyles.LogInfo);
            gUIStyle62.normal.background = (Files.LoadTextureFromDll("logErrorIcon", 20, 20));
            GUIStyle gUIStyle63 = gUIStyle62;
            GUIStyle gUIStyle64 = new GUIStyle(SkillEditorStyles.LogInfo);
            gUIStyle64.normal.background = (Files.LoadTextureFromDll("logTransitionIcon", 20, 20));
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
            gUIStyle66.normal.textColor = (EditorStyles.label.normal.textColor);
            gUIStyle66.padding = (new RectOffset(16, 0, 0, 0));
            gUIStyle66.margin = (new RectOffset(3, 4, 0, 0));
            gUIStyle66.alignment = (0);
            gUIStyle66.fixedHeight = (16f);
            SkillEditorStyles.ActionItem = gUIStyle66;
            GUIStyle gUIStyle67 = new GUIStyle(SkillEditorStyles.ActionItem);
            gUIStyle67.normal.background = (SkillEditorStyles.SelectedBG);
            gUIStyle67.normal.textColor = (Color.white);
            SkillEditorStyles.ActionItemSelected = gUIStyle67;
            GUIStyle gUIStyle68 = new GUIStyle(EditorStyles.label);
            gUIStyle68.margin = (new RectOffset(3, 4, 0, 0));
            gUIStyle68.alignment = (0);
            gUIStyle68.fixedHeight = (16f);
            SkillEditorStyles.ActionLabel = gUIStyle68;
            GUIStyle gUIStyle69 = new GUIStyle(SkillEditorStyles.ActionLabel);
            gUIStyle69.normal.textColor = (Color.white);
            SkillEditorStyles.ActionLabelSelected = gUIStyle69;
            GUIStyle gUIStyle70 = new GUIStyle();
            gUIStyle70.normal.textColor = (EditorStyles.label.normal.textColor);
            gUIStyle70.padding = (new RectOffset(1, 0, 3, 2));
            gUIStyle70.fixedHeight = (18f);
            SkillEditorStyles.TableRow = gUIStyle70;
            GUIStyle gUIStyle71 = new GUIStyle(SkillEditorStyles.TableRow);
            gUIStyle71.normal.background = (SkillEditorStyles.SelectedBG);
            gUIStyle71.normal.textColor = (Color.white);
            SkillEditorStyles.TableRowSelected = gUIStyle71;
            SkillEditorStyles.TableRowText = new GUIStyle("Label");
            GUIStyle gUIStyle72 = new GUIStyle(SkillEditorStyles.TableRowText);
            gUIStyle72.normal.textColor = (Color.white);
            SkillEditorStyles.TableRowTextSelected = gUIStyle72;
            GUIStyle gUIStyle73 = new GUIStyle("Toggle");
            gUIStyle73.padding = (new RectOffset(0, 0, 0, 0));
            gUIStyle73.margin = (new RectOffset(4, 0, 1, 0));
            SkillEditorStyles.TableRowCheckBox = gUIStyle73;
            GUIStyle gUIStyle74 = new GUIStyle(EditorStyles.foldout);
            gUIStyle74.fontStyle = FontStyle.Bold;
            SkillEditorStyles.BoldFoldout = gUIStyle74;
            GUIStyle gUIStyle75 = new GUIStyle();
            gUIStyle75.normal.background = (Files.LoadTextureFromDll("logEntryBox", 16, 16));
            gUIStyle75.normal.textColor = (Color.white);
            SkillEditorStyles.LogBackground = gUIStyle75;
            GUIStyle gUIStyle76 = new GUIStyle();
            gUIStyle76.normal.textColor = (Color.white);
            gUIStyle76.padding = (new RectOffset(1, 0, 3, 2));
            gUIStyle76.fixedHeight = (18f);
            SkillEditorStyles.LogLine = gUIStyle76;
            GUIStyle gUIStyle77 = new GUIStyle(SkillEditorStyles.LogLine);
            gUIStyle77.padding = (new RectOffset(27, 0, 3, 2));
            SkillEditorStyles.LogLine2 = gUIStyle77;
            GUIStyle gUIStyle78 = new GUIStyle(SkillEditorStyles.LogLine);
            gUIStyle78.normal.background = (SkillEditorStyles.SelectedBG);
            gUIStyle78.normal.textColor = (Color.white);
            SkillEditorStyles.LogLineSelected = gUIStyle78;
            GUIStyle gUIStyle79 = new GUIStyle();
            gUIStyle79.normal.background = (Files.LoadTextureFromDll("yellow", 2, 6));
            gUIStyle79.normal.textColor = (Color.white);
            gUIStyle79.fixedHeight = (6f);
            SkillEditorStyles.LogLineTimeline = gUIStyle79;
            GUIStyle gUIStyle80 = new GUIStyle();
            gUIStyle80.normal.background = (Files.LoadTextureFromDll("pasteDivider", 2, 2));
            gUIStyle80.normal.textColor = (Color.white);
            gUIStyle80.fixedHeight = (2f);
            SkillEditorStyles.InsertLine = gUIStyle80;
            SkillEditorStyles.DefaultWatermark = Files.LoadTextureFromDll("playMakerWatermark", 256, 256);
            GUIStyle gUIStyle81 = new GUIStyle();
            gUIStyle81.alignment = TextAnchor.LowerRight;
            SkillEditorStyles.Watermark = gUIStyle81;
            GUIStyle gUIStyle82 = new GUIStyle(EditorStyles.label);
            gUIStyle82.normal.textColor = (new Color(1f, 1f, 1f, 0.1f));
            gUIStyle82.fontSize = (32);
            gUIStyle82.fontStyle = FontStyle.Bold;
            SkillEditorStyles.LargeWatermarkText = gUIStyle82;
            GUIStyle gUIStyle83 = new GUIStyle();
            gUIStyle83.normal.textColor = (new Color(1f, 1f, 1f, 0.15f));
            gUIStyle83.padding = (new RectOffset(5, 0, 0, 0));
            gUIStyle83.font = (EditorStyles.standardFont);
            gUIStyle83.fontSize = (14);
            gUIStyle83.fontStyle = FontStyle.Normal;
            gUIStyle83.wordWrap = (true);
            SkillEditorStyles.SmallWatermarkText = gUIStyle83;
            GUIStyle gUIStyle84 = new GUIStyle(EditorStyles.label);
            gUIStyle84.fontSize = (32);
            gUIStyle84.fontStyle = FontStyle.Bold;
            SkillEditorStyles.LargeTitleText = gUIStyle84;
            GUIStyle gUIStyle85 = new GUIStyle();
            gUIStyle85.normal.textColor = (Color.white);
            gUIStyle85.fontSize = (32);
            gUIStyle85.fontStyle = FontStyle.Bold;
            SkillEditorStyles.LargeText = gUIStyle85;
            GUIStyle gUIStyle86 = new GUIStyle();
            gUIStyle86.normal.background = (Files.LoadTextureFromDll("wanLarge", 42, 42));
            gUIStyle86.normal.textColor = (Color.white);
            gUIStyle86.border = (new RectOffset(42, 0, 0, 0));
            gUIStyle86.padding = (new RectOffset(42, 0, 0, 0));
            gUIStyle86.margin = (new RectOffset(0, 0, 0, 0));
            gUIStyle86.contentOffset = (new Vector2(0f, 0f));
            gUIStyle86.alignment = TextAnchor.MiddleLeft;
            gUIStyle86.fixedHeight = (42f);
            gUIStyle86.fontSize = (32);
            gUIStyle86.fontStyle = FontStyle.Bold;
            SkillEditorStyles.LargeTitleWithLogo = gUIStyle86;
            GUIStyle gUIStyle87 = new GUIStyle();
            gUIStyle87.normal.background = (Files.LoadTextureFromDll("playMakerHeader", 253, 60));
            gUIStyle87.normal.textColor = (Color.white);
            gUIStyle87.border = (new RectOffset(253, 0, 0, 0));
            SkillEditorStyles.PlaymakerHeader = gUIStyle87;
            GUIStyle gUIStyle88 = new GUIStyle();
            gUIStyle88.normal.textColor = (EditorStyles.label.normal.textColor);
            gUIStyle88.border = (new RectOffset(64, 0, 0, 0));
            gUIStyle88.padding = (new RectOffset(66, 0, 0, 0));
            gUIStyle88.margin = (new RectOffset(20, 20, 20, 0));
            gUIStyle88.alignment = (0);
            gUIStyle88.fixedHeight = (64f);
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
            gUIStyle89.padding = (new RectOffset(2, 0, 2, 0));
            SkillEditorStyles.InlineErrorIcon = gUIStyle89;
        }
        private static void InitProSkin()
        {
            SkillEditorStyles.BroadcastIcon = Files.LoadTextureFromDll("broadcastIcon", 16, 16);
            SkillEditorStyles.GuiContentErrorColor = new Color(1f, 0.1f, 0.1f);
            SkillEditorStyles.GuiBackgroundErrorColor = new Color(1f, 0.4f, 0.4f);
            GUIStyle gUIStyle = new GUIStyle(SkillEditorStyles.InfoBox);
            gUIStyle.normal.background = (Files.LoadTextureFromDll("hintBox", 16, 16));
            gUIStyle.normal.textColor = (new Color(0.6f, 0.7f, 0.8f));
            SkillEditorStyles.HintBox = gUIStyle;
            GUIStyle gUIStyle2 = new GUIStyle(SkillEditorStyles.HintBox);
            gUIStyle2.normal.background = (null);
            SkillEditorStyles.HintBoxTextOnly = gUIStyle2;
        }
        private static void InitIndieSkin()
        {
            SkillEditorStyles.ErrorBox.normal.background = (SkillEditorStyles.InfoBox.normal.background);
            SkillEditorStyles.ErrorBox.normal.textColor = (SkillEditorStyles.ErrorTextColorIndie);
            SkillEditorStyles.ActionErrorBox.normal.textColor = (SkillEditorStyles.ErrorTextColorIndie);
            SkillEditorStyles.BroadcastIcon = Files.LoadTextureFromDll("broadcastIcon_indie", 16, 16);
            SkillEditorStyles.SelectedBG = Files.LoadTextureFromDll("selectedColor_indie", 2, 2);
            SkillEditorStyles.SelectedRow.normal.background = (SkillEditorStyles.SelectedBG);
            SkillEditorStyles.ActionItemSelected.normal.background = (SkillEditorStyles.SelectedBG);
            SkillEditorStyles.SelectedEventBox.normal.background = (SkillEditorStyles.SelectedBG);
            SkillEditorStyles.TableRowSelected.normal.background = (SkillEditorStyles.SelectedBG);
            SkillEditorStyles.GuiContentErrorColor = new Color(1f, 0f, 0f);
            SkillEditorStyles.GuiBackgroundErrorColor = new Color(1f, 0.3f, 0.3f);
            GUIStyle gUIStyle = new GUIStyle(SkillEditorStyles.InfoBox);
            gUIStyle.normal.background = (Files.LoadTextureFromDll("hintBox", 16, 16));
            gUIStyle.normal.textColor = (new Color(0.9f, 0.95f, 1f));
            SkillEditorStyles.HintBox = gUIStyle;
            GUIStyle gUIStyle2 = new GUIStyle(SkillEditorStyles.HintBox);
            gUIStyle2.normal.background = (null);
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
                    SkillEditorStyles.LargeWatermarkText.normal.textColor = (new Color(1f, 1f, 1f, 0.15f));
                    SkillEditorStyles.SmallWatermarkText.normal.textColor = (new Color(1f, 1f, 1f, 0.2f));
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
                    SkillEditorStyles.Background.normal.background = (Files.LoadTextureFromDll("graphBackground_indie", 32, 32));
                    SkillEditorStyles.CommentBox.normal.background = (Files.LoadTextureFromDll("infoBox_indie", 16, 16));
                    SkillEditorStyles.GlobalTransitionBox.normal.background = (Files.LoadTextureFromDll("globalTransitionBox_indie", 16, 16));
                    SkillEditorStyles.StartTransitionBox.normal.background = (Files.LoadTextureFromDll("startTransitionBox_indie", 16, 16));
                    SkillEditorStyles.LargeWatermarkText.normal.textColor = (new Color(0f, 0f, 0f, 0.2f));
                    SkillEditorStyles.SmallWatermarkText.normal.textColor = (new Color(0f, 0f, 0f, 0.3f));
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
            SkillEditorStyles.StateTitleBox.fontSize = fontSize;
            SkillEditorStyles.StateTitleBox.fixedHeight = SkillEditorStyles.StateRowHeight;
            SkillEditorStyles.TransitionBox.fontSize = fontSize;
            SkillEditorStyles.TransitionBox.fixedHeight = SkillEditorStyles.StateRowHeight;
            SkillEditorStyles.TransitionBoxSelected.fontSize = fontSize;
            SkillEditorStyles.TransitionBoxSelected.fixedHeight = SkillEditorStyles.StateRowHeight;
            SkillEditorStyles.GlobalTransitionBox.fontSize = fontSize;
            SkillEditorStyles.GlobalTransitionBox.fixedHeight = SkillEditorStyles.StateRowHeight;
            SkillEditorStyles.StartTransitionBox.fontSize = fontSize;
            SkillEditorStyles.StartTransitionBox.fixedHeight = SkillEditorStyles.StateRowHeight;
            SkillEditorStyles.CommentBox.fontSize = fontSize;
            SkillEditorStyles.CommentBox.padding = new RectOffset((int)(5f * scale), (int)(5f * scale), (int)(3f * scale), (int)(3f * scale));
            SkillEditorStyles.scaleInitialized = true;
        }
    }
}
