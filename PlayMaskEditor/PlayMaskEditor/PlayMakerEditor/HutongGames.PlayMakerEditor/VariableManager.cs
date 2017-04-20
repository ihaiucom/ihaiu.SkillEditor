using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	internal class VariableManager
	{
		private FsmVariablesEditor _fsmVariablesEditor;
		private SkillTemplate clipboard;
		private FsmVariablesEditor fsmVariablesEditor
		{
			get
			{
				if (this._fsmVariablesEditor == null)
				{
					this.Reset();
				}
				return this._fsmVariablesEditor;
			}
		}
		public void Reset()
		{
			Object selectedFsmOwner = SkillEditor.SelectedFsmOwner;
			this._fsmVariablesEditor = new FsmVariablesEditor(SkillEditor.Window, selectedFsmOwner);
			FsmVariablesEditor expr_1D = this._fsmVariablesEditor;
			expr_1D.SettingsButtonClicked = (EditorApplication.CallbackFunction)Delegate.Combine(expr_1D.SettingsButtonClicked, new EditorApplication.CallbackFunction(this.DoSettingsMenu));
			FsmVariablesEditor expr_44 = this._fsmVariablesEditor;
			expr_44.VariableContextClicked = (FsmVariablesEditor.ContextClickVariable)Delegate.Combine(expr_44.VariableContextClicked, new FsmVariablesEditor.ContextClickVariable(this.DoVariableContextMenu));
			this._fsmVariablesEditor.Reset();
		}
		public void UpdateView()
		{
			this.fsmVariablesEditor.UpdateView();
		}
		public void OnGUI()
		{
			this.fsmVariablesEditor.OnGUI();
		}
		private void DoSettingsMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Debug_Variables()), FsmEditorSettings.DebugVariables, new GenericMenu.MenuFunction(EditorCommands.ToggleDebugVariables));
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Delete_Unused_Variables()), false, new GenericMenu.MenuFunction(this.DeleteUnusedVariables));
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Global_Variables()), false, new GenericMenu.MenuFunction(SkillEditor.OpenGlobalVariablesWindow));
			genericMenu.AddSeparator("");
			if (SkillEditor.SelectedFsmComponent != null)
			{
				genericMenu.AddItem(new GUIContent(Strings.get_Menu_Copy_Variables()), false, new GenericMenu.MenuFunction(this.CopyVariables));
				if (this.clipboard != null)
				{
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_Paste_Variables()), false, new GenericMenu.MenuFunction(this.PasteVariables));
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_Paste_Variable_Values()), false, new GenericMenu.MenuFunction(this.PasteVariableValues));
				}
				else
				{
					genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Paste_Variables()));
					genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Paste_Variable_Values()));
				}
			}
			else
			{
				genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Copy_Variables()));
				genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Paste_Variables()));
			}
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Online_Help()), false, new GenericMenu.MenuFunction(VariableManager.OpenOnlineHelp));
			genericMenu.ShowAsContext();
		}
		private void CreateClipboard()
		{
			this.clipboard = (SkillTemplate)ScriptableObject.CreateInstance(typeof(SkillTemplate));
		}
		private void CopyVariables()
		{
			if (this.clipboard == null)
			{
				this.CreateClipboard();
			}
			SkillBuilder.CopyFsmToTemplate(SkillEditor.SelectedFsm, this.clipboard);
		}
		private void PasteVariables()
		{
			if (this.clipboard != null)
			{
				SkillEditor.RegisterUndo(Strings.get_Menu_Paste_Variables());
				bool overwriteValues = false;
				if (SkillBuilder.VariableInSourceExistsInTarget(this.clipboard, SkillEditor.SelectedFsm))
				{
					overwriteValues = Dialogs.YesNoDialog(Strings.get_Dialog_Paste_Variables(), Strings.get_PasteVariables_Warning_Some_variables_already_exist__overwrite_values());
				}
				SkillBuilder.PasteVariables(SkillEditor.SelectedFsm, this.clipboard, overwriteValues);
				SkillEditor.SetFsmDirty();
				this.fsmVariablesEditor.Reset();
			}
		}
		private void PasteVariableValues()
		{
			if (this.clipboard != null)
			{
				SkillEditor.RegisterUndo(Strings.get_Menu_Paste_Variable_Values());
				SkillEditor.SelectedFsm.get_Variables().ApplyVariableValuesCareful(this.clipboard.fsm.get_Variables());
				SkillEditor.SetFsmDirty();
				this.fsmVariablesEditor.Reset();
			}
		}
		private void DeleteUnusedVariables()
		{
			SkillSearch.Update(SkillEditor.SelectedFsm);
			List<SkillVariable> unusedVariables = SkillSearch.GetUnusedVariables(SkillEditor.SelectedFsm);
			if (unusedVariables.get_Count() == 0)
			{
				EditorUtility.DisplayDialog(Strings.get_Command_Delete_Unused_Variables(), Strings.get_Label_No_unused_variables(), Strings.get_OK());
				return;
			}
			if (Dialogs.YesNoDialog(Strings.get_Command_Delete_Unused_Variables(), string.Format(Strings.get_Command_Delete_Variables_Are_you_sure(), unusedVariables.get_Count())))
			{
				SkillEditor.RegisterUndo(Strings.get_Menu_Delete_Unused_Variables());
				List<SkillVariable> list = new List<SkillVariable>(unusedVariables);
				using (List<SkillVariable>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SkillVariable current = enumerator.get_Current();
						this.fsmVariablesEditor.DeleteVariable(current, false, false);
					}
				}
				this.Reset();
			}
		}
		private static void OpenOnlineHelp()
		{
			EditorCommands.OpenWikiPage(WikiPages.VariableManager);
		}
		private void DoVariableContextMenu(SkillVariable variable)
		{
			GenericMenu genericMenu = new GenericMenu();
			List<SkillInfo> variableUsageList = SkillSearch.GetVariableUsageList(SkillEditor.SelectedFsm, variable.NamedVar);
			if (variableUsageList.get_Count() == 0)
			{
				genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Unused_Variable()));
			}
			else
			{
				using (List<SkillInfo>.Enumerator enumerator = variableUsageList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SkillInfo current = enumerator.get_Current();
						genericMenu.AddItem(new GUIContent(current.state.get_Name()), SkillEditor.SelectedState == current.state, new GenericMenu.MenuFunction2(VariableManager.SelectFsmInfo), current);
					}
				}
			}
			genericMenu.AddSeparator("");
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Move_To_Global_Variables()), false, new GenericMenu.MenuFunction2(this.MoveToGlobals), variable);
			genericMenu.ShowAsContext();
		}
		private static void SelectFsmInfo(object userdata)
		{
			SkillInfo fsmInfo = userdata as SkillInfo;
			if (fsmInfo != null)
			{
				SkillEditor.SelectState(fsmInfo.state, true);
				SkillEditor.SelectAction(fsmInfo.action, true);
				SkillEditor.Inspector.SetMode(InspectorMode.StateInspector);
			}
		}
		private void MoveToGlobals(object userdata)
		{
			SkillVariable fsmVariable = userdata as SkillVariable;
			if (fsmVariable != null && fsmVariable.NamedVar != null)
			{
				if (SkillVariables.get_GlobalVariables().Contains(fsmVariable.Name))
				{
					NamedVariable variable = SkillVariables.get_GlobalVariables().GetVariable(fsmVariable.Name);
					if (variable.get_VariableType() != fsmVariable.NamedVar.get_VariableType())
					{
						Dialogs.OkDialog(Strings.get_Dialog_Make_Global_Variable(), Strings.get_VariableManager_MoveToGlobals_Warning());
						return;
					}
					if (Dialogs.YesNoDialog(Strings.get_Dialog_Make_Global_Variable(), Strings.get_VariableManager_MoveToGlobals_Confirm()))
					{
						this.RemoveLocalVariable(fsmVariable);
						return;
					}
				}
				else
				{
					if (Dialogs.AreYouSure(Strings.get_Dialog_Make_Global_Variable()))
					{
						SkillVariable.AddVariable(SkillVariables.get_GlobalVariables(), fsmVariable.NamedVar.Clone());
						this.RemoveLocalVariable(fsmVariable);
					}
				}
			}
		}
		private void RemoveLocalVariable(SkillVariable variable)
		{
			SkillVariable.DeleteVariable(SkillEditor.SelectedFsm.get_Variables(), variable);
			SkillEditor.SelectedFsm.Reinitialize();
			SkillEditor.SetFsmDirty();
			this.Reset();
			GlobalVariablesWindow.ResetView();
		}
		public static void SortVariables(Skill fsm)
		{
			if (fsm == null)
			{
				return;
			}
			VariableManager.SortVariables(fsm.get_Variables());
		}
		public static void SortVariables(SkillVariables fsmVariables)
		{
			if (fsmVariables == null)
			{
				return;
			}
			fsmVariables.set_BoolVariables(ArrayUtility.Sort<SkillBool>(fsmVariables.get_BoolVariables()));
			fsmVariables.set_FloatVariables(ArrayUtility.Sort<SkillFloat>(fsmVariables.get_FloatVariables()));
			fsmVariables.set_IntVariables(ArrayUtility.Sort<SkillInt>(fsmVariables.get_IntVariables()));
			fsmVariables.set_Vector2Variables(ArrayUtility.Sort<SkillVector2>(fsmVariables.get_Vector2Variables()));
			fsmVariables.set_Vector3Variables(ArrayUtility.Sort<SkillVector3>(fsmVariables.get_Vector3Variables()));
			fsmVariables.set_GameObjectVariables(ArrayUtility.Sort<SkillGameObject>(fsmVariables.get_GameObjectVariables()));
			fsmVariables.set_ObjectVariables(ArrayUtility.Sort<SkillObject>(fsmVariables.get_ObjectVariables()));
			fsmVariables.set_RectVariables(ArrayUtility.Sort<SkillRect>(fsmVariables.get_RectVariables()));
			fsmVariables.set_QuaternionVariables(ArrayUtility.Sort<SkillQuaternion>(fsmVariables.get_QuaternionVariables()));
			fsmVariables.set_MaterialVariables(ArrayUtility.Sort<SkillMaterial>(fsmVariables.get_MaterialVariables()));
			fsmVariables.set_TextureVariables(ArrayUtility.Sort<SkillTexture>(fsmVariables.get_TextureVariables()));
			fsmVariables.set_StringVariables(ArrayUtility.Sort<SkillString>(fsmVariables.get_StringVariables()));
			fsmVariables.set_ColorVariables(ArrayUtility.Sort<SkillColor>(fsmVariables.get_ColorVariables()));
		}
	}
}
