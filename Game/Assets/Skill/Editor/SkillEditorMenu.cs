using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ihaiu
{
    public class SkillEditorMenu 
    {

        private const string MenuRoot = "Skill/"; 

        [MenuItem(MenuRoot + "Skill Editor Window", false, 0)]
        public static void OpenSkillEditorWindow()
        {
            SkillEditorWindow.OpenWindow();
        }
    }
}