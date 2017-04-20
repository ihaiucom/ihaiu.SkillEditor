using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
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
		public static void Update(Skill fsm)
		{
			if (fsm == null)
			{
				return;
			}
			Labels.fsmName.Remove(fsm);
			Labels.fsmFullName.Remove(fsm);
		}
		public static string NicifyVariableName(string name)
		{
			string text;
			if (Labels.niceVariableNames.TryGetValue(name, ref text))
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
		public static GUIContent GetEventLabel(SkillTransition transition)
		{
			SkillEditorContent.EventLabel.set_text("...");
			SkillEditorContent.EventLabel.set_tooltip("");
			if (!SkillEvent.IsNullOrEmpty(transition.get_FsmEvent()))
			{
				SkillEditorContent.EventLabel.set_text(transition.get_FsmEvent().get_Name());
			}
			return SkillEditorContent.EventLabel;
		}
		public static string GetCurrentStateLabel(Skill fsm)
		{
			if (EditorApplication.get_isPlaying())
			{
				if (fsm.get_ActiveState() != null)
				{
					return fsm.get_ActiveState().get_Name();
				}
				return "No Active State";
			}
			else
			{
				SkillState state = fsm.GetState(fsm.get_StartState());
				if (state != null)
				{
					return state.get_Name();
				}
				return "No Start State";
			}
		}
		public static string GetFsmLabel(Skill fsm)
		{
			if (fsm == null)
			{
				return "None (Fsm)";
			}
			string text;
			if (Labels.fsmName.TryGetValue(fsm, ref text))
			{
				return text;
			}
			text = (fsm.get_IsSubFsm() ? (fsm.get_Host().get_Name() + " : " + fsm.get_Name()) : fsm.get_Name());
			int fsmNameIndex = Labels.GetFsmNameIndex(fsm);
			if (fsmNameIndex > 0)
			{
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					" (",
					fsmNameIndex + 1,
					")"
				});
			}
			Labels.fsmName.Add(fsm, text);
			return text;
		}
		public static string GetFullFsmLabel(PlayMakerFSM fsmComponent)
		{
			if (!(fsmComponent == null))
			{
				return Labels.GetFullFsmLabel(fsmComponent.get_Fsm());
			}
			return "None (PlayMakerFSM)";
		}
		public static string GetFullFsmLabel(Skill fsm)
		{
			if (fsm == null)
			{
				return "None (FSM)";
			}
			if (fsm.get_OwnerObject() == null)
			{
				return "Missing Owner";
			}
			string text;
			if (Labels.fsmFullName.TryGetValue(fsm, ref text))
			{
				return text;
			}
			if (fsm.get_UsedInTemplate() != null)
			{
				text = "Template: " + fsm.get_UsedInTemplate().get_name();
			}
			else
			{
				text = fsm.get_OwnerName() + " : " + Labels.GetFsmLabel(fsm);
				if (FsmEditorSettings.AddPrefabLabel && PrefabUtility.GetPrefabType(fsm.get_Owner()) == 1)
				{
					text += " (Prefab)";
				}
			}
			Labels.fsmFullName.Add(fsm, text);
			return text;
		}
		public static string GetRuntimeFsmLabel(Skill fsm)
		{
			if (fsm == null)
			{
				return "None (FSM)";
			}
			if (fsm.get_Owner() == null)
			{
				return fsm.get_Name();
			}
			return fsm.get_OwnerName() + " : " + fsm.get_Name();
		}
		public static string GetFullFsmLabelWithInstanceID(Skill fsm)
		{
			string text = string.Empty;
			if (fsm != null && fsm.get_OwnerObject() != null)
			{
				text = " [" + fsm.get_OwnerObject().GetInstanceID() + "]";
			}
			return Labels.GetFullFsmLabel(fsm) + text;
		}
		public static string GetFullFsmLabelWithInstanceID(PlayMakerFSM fsm)
		{
			string text = string.Empty;
			if (fsm != null)
			{
				text = " [" + fsm.GetInstanceID() + "]";
			}
			return Labels.GetFullFsmLabel(fsm) + text;
		}
		public static GUIContent GetRuntimeFsmLabelToFit(Skill fsm, float width, GUIStyle style)
		{
			string runtimeFsmLabel = Labels.GetRuntimeFsmLabel(fsm);
			float x = style.CalcSize(new GUIContent(runtimeFsmLabel)).x;
			if (x < width)
			{
				return new GUIContent(runtimeFsmLabel, runtimeFsmLabel);
			}
			return new GUIContent(fsm.get_Name(), runtimeFsmLabel);
		}
		public static string GetFullStateLabel(SkillState state)
		{
			if (state == null)
			{
				return "None (State)";
			}
			return Labels.GetFullFsmLabel(state.get_Fsm()) + " : " + state.get_Name();
		}
		public static string GetUniqueFsmName(GameObject go)
		{
			PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
			string text = Strings.get_FSM();
			int num = 1;
			while (Labels.FsmNameExists(components, text))
			{
				num++;
				text = "FSM " + num;
			}
			return text;
		}
		private static bool FsmNameExists(IEnumerable<PlayMakerFSM> fsmComponents, string name)
		{
			using (IEnumerator<PlayMakerFSM> enumerator = fsmComponents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PlayMakerFSM current = enumerator.get_Current();
					if (current.get_Fsm().get_Name() == name)
					{
						return true;
					}
				}
			}
			return false;
		}
		public static string GetShortTypeName(Type type)
		{
			if (type == null)
			{
				return "";
			}
			string text;
			if (Labels.shortTypeNames.TryGetValue(type, ref text))
			{
				return text;
			}
			text = Labels.StripNamespace(type.get_FullName());
			Labels.shortTypeNames.Add(type, text);
			return text;
		}
		public static string GetActionLabel(Type actionType)
		{
			if (actionType == null)
			{
				return "";
			}
			string text;
			if (Labels.actionNames.TryGetValue(actionType, ref text))
			{
				return text;
			}
			text = Labels.NicifyVariableName(Labels.StripNamespace(actionType.ToString()));
			Labels.actionNames.Add(actionType, text);
			return text;
		}
		public static string GetActionLabel(SkillStateAction action)
		{
			if (action == null)
			{
				return Strings.get_Label_None_Action();
			}
			if (string.IsNullOrEmpty(action.get_Name()))
			{
				return Labels.GetActionLabel(action.GetType());
			}
			return action.get_Name();
		}
		public static string GetTypeTooltip(Type type)
		{
			if (type == null)
			{
				return "";
			}
			string text;
			if (Labels.typeTooltips.TryGetValue(type, ref text))
			{
				return text;
			}
			text = "Type: ";
			if (type == typeof(SkillOwnerDefault))
			{
				text += "GameObject";
			}
			else
			{
				if (type == typeof(SkillEvent))
				{
					text += "FsmEvent";
				}
				else
				{
					if (type == typeof(SkillVar))
					{
						text += "FsmVar";
					}
					else
					{
						if (type == typeof(SkillArray))
						{
							text += "Array";
						}
						else
						{
							if (type.IsSubclassOf(typeof(NamedVariable)))
							{
								PropertyInfo property = type.GetProperty("Value");
								if (property != null)
								{
									text += Labels.StripUnityEngineNamespace(TypeHelpers.GetFriendlyName(property.get_PropertyType()));
								}
								else
								{
									text += "Unknown";
								}
							}
							else
							{
								if (type.get_IsArray())
								{
									text = Labels.GetTypeTooltip(type.GetElementType()) + " Array";
								}
								else
								{
									text += TypeHelpers.GetFriendlyName(type);
								}
							}
						}
					}
				}
			}
			Labels.typeTooltips.Add(type, Labels.NicifyTypeTooltip(text));
			return text;
		}
		private static string NicifyTypeTooltip(string tooltip)
		{
			if (tooltip != null)
			{
				if (tooltip == "Single")
				{
					return Strings.get_Label_Float();
				}
				if (tooltip == "FsmOwnerDefault")
				{
					return Strings.get_Label_GameObject();
				}
			}
			return tooltip;
		}
		public static int GetFsmIndex(Skill fsm)
		{
			if (fsm == null || fsm.get_Owner() == null || fsm.get_GameObject() == null)
			{
				return -1;
			}
			int num = 0;
			PlayMakerFSM[] components = fsm.get_GameObject().GetComponents<PlayMakerFSM>();
			PlayMakerFSM[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				PlayMakerFSM playMakerFSM = array[i];
				if (playMakerFSM.get_Fsm() == fsm)
				{
					return num;
				}
				num++;
			}
			return -1;
		}
		public static int GetFsmNameIndex(Skill fsm)
		{
			if (fsm == null || fsm.get_Owner() == null || fsm.get_GameObject() == null)
			{
				return 0;
			}
			int num = 0;
			PlayMakerFSM[] components = fsm.get_GameObject().GetComponents<PlayMakerFSM>();
			PlayMakerFSM[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				PlayMakerFSM playMakerFSM = array[i];
				if (playMakerFSM.get_Fsm() == fsm)
				{
					return num;
				}
				if (playMakerFSM.get_Fsm().get_Name() == fsm.get_Name())
				{
					num++;
				}
			}
			return 0;
		}
	}
}
