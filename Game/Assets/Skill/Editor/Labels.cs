using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace ihaiu
{
    [Localizable(false)]
    public static class Labels
    {
        private static readonly Dictionary<string, string> niceVariableNames = new Dictionary<string, string>();
        private static readonly Dictionary<Type, string> shortTypeNames = new Dictionary<Type, string>();
        private static readonly Dictionary<Type, string> actionNames = new Dictionary<Type, string>();
        private static readonly Dictionary<Type, string> typeTooltips = new Dictionary<Type, string>();
        private static readonly Dictionary<Skill, string> fsmName = new Dictionary<Skill, string>();
        private static readonly Dictionary<Skill, string> fsmFullName = new Dictionary<Skill, string>();
        public static void Update(Skill skill)
        {
            if (skill == null)
            {
                return;
            }
            Labels.fsmName.Remove(skill);
            Labels.fsmFullName.Remove(skill);
        }
        public static string NicifyVariableName(string name)
        {
            string text;
            if (Labels.niceVariableNames.TryGetValue(name, out text))
            {
                return text;
            }
            text = ObjectNames.NicifyVariableName(name);
            text = text.Replace("Vector 2", "Vector2 ");
            text = text.Replace("Vector 3", "Vector3 ");
            text = text.Replace("GUI", "GUI ");
            text = text.Replace("GUI Layout", "GUILayout");
            text = text.Replace("ITween", "iTween");
            text = text.Replace("IPhone", "iPhone");
            text = text.Replace("i Phone", "iPhone");
            text = text.Replace("Player Prefs", "PlayerPrefs");
            text = text.Replace("Network View ", "NetworkView ");
            text = text.Replace("Master Server ", "MasterServer ");
            text = text.Replace("Rpc ", "RPC ");
            text = text.Replace("Collision 2d", "Collision2D");
            text = text.Replace("Trigger 2d", "Trigger2D");
            Labels.niceVariableNames.Add(name, text);
            return text;
        }
        public static string StripNamespace(string name)
        {
            return name.Substring(name.LastIndexOf(".", 4) + 1);
        }
        public static string StripUnityEngineNamespace(string name)
        {
            if (name.IndexOf("UnityEngine.", 4) != 0)
            {
                return name;
            }
            return name.Replace("UnityEngine.", "");
        }
        public static string FormatTime(float time)
        {
            DateTime dateTime = new DateTime(Convert.ToInt64(Mathf.Max(time, 0f) * 1E+07f), 0);
            return dateTime.ToString("mm:ss:ff");
        }
        public static string GenerateUniqueLabelWithNumber(List<string> labels, string label)
        {
            int num = 2;
            string text = label;
            while (labels.Contains(label))
            {
                label = string.Concat(new object[]
                    {
                        text,
                        " (",
                        num++,
                        ")"
                    });
            }
            return label;
        }
        public static string GenerateUniqueLabel(List<string> labels, string label)
        {
            while (labels.Contains(label))
            {
                label += " ";
            }
            return label;
        }
        public static string NicifyParameterName(string name)
        {
            return Labels.NicifyVariableName(Labels.StripNamespace(name));
        }
        public static string GetStateLabel(string stateName)
        {
            if (!string.IsNullOrEmpty(stateName))
            {
                return stateName;
            }
            return "None (State)";
        }

        public static string GetFsmLabel(Skill fsm)
        {
            if (fsm == null)
            {
                return "None (Fsm)";
            }

            string text;
            if (Labels.fsmName.TryGetValue(fsm, out text))
            {
                return text;
            }
            text = fsm.Name;
//            text = (fsm.get_IsSubFsm() ? (fsm.get_Host().get_Name() + " : " + fsm.get_Name()) : fsm.get_Name());
//            int fsmNameIndex = Labels.GetFsmNameIndex(fsm);
//            if (fsmNameIndex > 0)
//            {
//                object obj = text;
//                text = string.Concat(new object[]
//                    {
//                        obj,
//                        " (",
//                        fsmNameIndex + 1,
//                        ")"
//                    });
//            }
            Labels.fsmName.Add(fsm, text);
            return text;
        }
    }
}
