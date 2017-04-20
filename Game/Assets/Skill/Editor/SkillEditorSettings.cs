using System;
using System.ComponentModel;
using UnityEngine;

namespace ihaiu
{
    [Serializable]
    public class SkillEditorSettings 
    {
        public const string ProductUrl = "http://blog.ihaiu.com/";
        public const float GraphViewMinZoom = 0.3f;
        public const float GraphViewMaxZoom = 1f;
        private static bool settingsLoaded;
        public static string ProductName = "SkillEditor";
        public static string ProductCopyright = "ihaiu.com";

        [Localizable(false)]
        public static void SaveSettings()
        {
            if (!SkillEditorSettings.settingsLoaded)
            {
                Debug.LogWarning("PlayMaker: Attempting to SaveSettings before LoadSettings.");
                return;
            }
        }

        [Localizable(false)]
        public static void LoadSettings()
        {
            if (SkillEditorSettings.settingsLoaded)
            {
                return;
            }
            SkillEditorSettings.settingsLoaded = true;
        }


        public static SkillEditorStyles.ColorScheme ColorScheme
        {
            get;
            set;
        }


        public static bool DisableActionBrowerWhenPlaying
        {
            get;
            set;
        }
        public static bool DisableEventBrowserWhenPlaying
        {
            get;
            set;
        }
        public static bool DisableEditorWhenPlaying
        {
            get;
            set;
        }
        public static bool DisableInspectorWhenPlaying
        {
            get;
            set;
        }
        public static bool DisableToolWindowsWhenPlaying
        {
            get;
            set;
        }
        public static bool ShowHints
        {
            get;
            set;
        }
        public static bool CloseActionBrowserOnEnter
        {
            get;
            set;
        }

        public static bool GraphViewShowMinimap
        {
            get;
            set;
        }
    }
}