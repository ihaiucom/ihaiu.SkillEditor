using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ihaiu
{
    public class SkillEditorGUILayout 
    {
        public static bool ToolWindowsCommonGUI(EditorWindow window)
        {
            if (SkillEditor.Instance == null)
            {
                window.Close();
                return false;
            }
            if (SkillEditorGUILayout.DoToolWindowsDisabledGUI())
            {
                return false;
            }
            if (EditorApplication.isCompiling)
            {
                GUI.enabled = false;
            }

            SkillEditorStyles.Init();
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F1 )
            {
                EditorCommands.ToggleShowHints();
                return false;
            }
            EditorGUI.indentLevel = 0;
            return true;
        }

        private static bool DoToolWindowsDisabledGUI()
        {
            if (SkillEditorGUILayout.DoEditorDisabledGUI())
            {
                return true;
            }
            if (EditorApplication.isPlaying && SkillEditorSettings.DisableToolWindowsWhenPlaying)
            {
                GUILayout.Label(Strings.Label_Tool_Windows_disabled_when_playing, new GUILayoutOption[0]);
                SkillEditorSettings.DisableToolWindowsWhenPlaying = !GUILayout.Toggle(!SkillEditorSettings.DisableToolWindowsWhenPlaying, Strings.Label_Enable_Tool_Windows_When_Playing, new GUILayoutOption[0]);
                if (GUI.changed)
                {
                    SkillEditorSettings.SaveSettings();
                }
                return SkillEditorSettings.DisableToolWindowsWhenPlaying;
            }
            return false;
        }

        public static bool DoEditorDisabledGUI()
        {
            if (EditorApplication.isPlaying && SkillEditorSettings.DisableEditorWhenPlaying)
            {
                GUILayout.Label(Strings.Label_Editor_disabled_when_playing, new GUILayoutOption[0]);
                SkillEditorSettings.DisableEditorWhenPlaying = !GUILayout.Toggle(!SkillEditorSettings.DisableEditorWhenPlaying, Strings.Label_Enable_Editor_When_Playing, new GUILayoutOption[0]);
                if (GUI.changed)
                {
                    SkillEditorSettings.SaveSettings();
                }
                return SkillEditorSettings.DisableEditorWhenPlaying;
            }
            return false;
        }
    }
}
