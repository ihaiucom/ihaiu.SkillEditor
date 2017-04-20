using UnityEngine;
using System.Collections;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEditor;

namespace ihaiu
{
    [Localizable(false)]
    [Serializable]
    public class SkillEditor 
    {
        private bool repaint;
        private bool repaintAll;
        private bool mouseUp;
        private bool mouseDown;

        private static SkillEditor instance;

        public static SkillEditor Instance
        {
            get
            {
                return SkillEditor.instance;
            }
        }

        private static List<Skill> fsmList;
        public static List<Skill> FsmList
        {
            get
            {
                if (SkillEditor.fsmList == null)
                {
                    SkillEditor.RebuildFsmList();
                }
                return SkillEditor.fsmList;
            }
        }


        public static void RebuildFsmList()
        {
           
        }

        public static void RepaintAll()
        {
            if (SkillEditor.instance != null)
            {
                SkillEditor.instance.repaintAll = true;
            }
        }

    }
}