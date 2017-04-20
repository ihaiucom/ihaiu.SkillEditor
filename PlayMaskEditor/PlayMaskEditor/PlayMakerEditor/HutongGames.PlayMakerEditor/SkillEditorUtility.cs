using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	[Localizable(false)]
	public static class SkillEditorUtility
	{
		[Obsolete("Removed")]
		public static string[] StateColorNames = new string[0];
		[Obsolete("Use Actions.List instead.")]
		public static List<Type> Actionslist
		{
			get
			{
				return Actions.List;
			}
		}
		[Obsolete("Use Actions.CategoryLookup instead.")]
		public static List<string> ActionCategoryLookup
		{
			get
			{
				return Actions.CategoryLookup;
			}
		}
		[Obsolete("Use Actions.Categories instead.")]
		public static List<string> ActionCategoryList
		{
			get
			{
				return Actions.Categories;
			}
		}
		[Obsolete("Use ActionScripts.ActionScriptLookup instead.")]
		public static Dictionary<Type, Object> ActionScriptLookup
		{
			get
			{
				return ActionScripts.ActionScriptLookup;
			}
		}
		[Obsolete("Use Templates.List instead.")]
		public static List<SkillTemplate> TemplateList
		{
			get
			{
				return Templates.List;
			}
		}
		[Obsolete("Use Templates.Categories instead.")]
		public static List<string> TemplateCategories
		{
			get
			{
				return Templates.Categories;
			}
		}
		[Obsolete("Use Templates.CategoryLookup instead.")]
		public static Dictionary<SkillTemplate, string> TemplateCategoryLookup
		{
			get
			{
				return Templates.CategoryLookup;
			}
		}
		[Obsolete("Use Files.ScriptList instead.")]
		public static List<string> ScriptList
		{
			get
			{
				return Files.ScriptList;
			}
		}
		[Obsolete("Use TypeHelpers.ObjectTypeList instead.")]
		public static List<Type> ObjectTypeList
		{
			get
			{
				return TypeHelpers.ObjectTypeList;
			}
		}
		[Obsolete("Use TypeHelpers.EnumTypeList instead.")]
		public static List<Type> EnumTypeList
		{
			get
			{
				return TypeHelpers.EnumTypeList;
			}
		}
		[Obsolete("Use Actions.GetUsage instead.")]
		public static List<SkillInfo> GetActionUsage(Type actionType)
		{
			return Actions.GetUsage(actionType);
		}
		[Obsolete("Use Actions.GetUsageCount instead.")]
		public static int GetActionUsageCount(Type actionType)
		{
			return Actions.GetUsageCount(actionType);
		}
		[Obsolete("Use Actions.BuildList instead.")]
		public static void BuildActionsList()
		{
			Actions.BuildList();
		}
		[Obsolete("Use Actions.UpdateUsage instead.")]
		public static void UpdateActionUsage()
		{
			Actions.UpdateUsage();
		}
		[Obsolete("Use Actions.GetCategory instead.")]
		public static string GetActionCategory(Type actionType)
		{
			return Actions.GetCategory(actionType);
		}
		[Obsolete("Use Actions.GetActionIndex instead.")]
		public static int GetActionIndex(SkillState state, SkillStateAction action)
		{
			return Actions.GetActionIndex(state, action);
		}
		[Obsolete("Use Actions.GetTooltip instead.")]
		public static string GetActionTooltip(SkillStateAction action)
		{
			return Actions.GetTooltip(action);
		}
		[Obsolete("Use Actions.UpdateTooltip instead.")]
		public static string UpdateActionTooltip(SkillStateAction action)
		{
			return Actions.UpdateTooltip(action);
		}
		[Obsolete("Use Actions.GetActionCategory instead.")]
		public static string GetActionCategoryAttribute(Type objType)
		{
			return Actions.GetActionCategory(objType);
		}
		[Obsolete("Use Actions.GetActionCategory instead.")]
		public static string GetActionCategoryAttribute(object[] attributes)
		{
			return Actions.GetActionCategory(attributes);
		}
		[Obsolete("Use Actions.GetTooltip instead.")]
		public static string GetTooltipAttribute(object instance)
		{
			return Actions.GetTooltip(instance);
		}
		[Obsolete("Use Actions.GetTooltip instead.")]
		public static string GetTooltipAttribute(Type objType)
		{
			return Actions.GetTooltip(objType);
		}
		[Obsolete("Use Actions.GetTooltip instead.")]
		public static string GetTooltipAttribute(FieldInfo field)
		{
			return Actions.GetTooltip(field);
		}
		[Obsolete("Use Actions.GetTooltip instead.")]
		public static string GetTooltipAttribute(object[] attributes)
		{
			return Actions.GetTooltip(attributes);
		}
		[Obsolete("Use FsmVariable.FsmHasVariable instead.")]
		public static bool FsmHasVariable(Skill fsm, string name)
		{
			return SkillVariable.FsmHasVariable(fsm, name);
		}
		[Obsolete("Use Labels.NicifyVariableName instead.")]
		public static string NicifyVariableName(string name)
		{
			return Labels.NicifyVariableName(name);
		}
		[Obsolete("Use Labels.NicifyParameterName instead.")]
		public static string NicifyParameterName(string name)
		{
			return Labels.NicifyParameterName(name);
		}
		[Obsolete("Use Labels.StripNamespace instead.")]
		public static string StripNamespace(string name)
		{
			return Labels.StripNamespace(name);
		}
		[Obsolete("Use Labels.StripUnityEngineNamespace instead.")]
		public static string StripUnityEngineNamespace(string name)
		{
			return Labels.StripUnityEngineNamespace(name);
		}
		[Obsolete("Use Labels.GenerateUniqueLabel instead.")]
		public static string GenerateUniqueLabel(List<string> labels, string label)
		{
			return Labels.GenerateUniqueLabel(labels, label);
		}
		[Obsolete("Use Labels.GenerateUniqueLabelWithNumber instead.")]
		public static string GenerateUniqueLabelWithNumber(List<string> labels, string label)
		{
			return Labels.GenerateUniqueLabelWithNumber(labels, label);
		}
		[Obsolete("Use Labels.FormateTime instead.")]
		public static string FormatTime(float time)
		{
			return Labels.FormatTime(time);
		}
		[Obsolete("Use Labels.GetStateLabel instead.")]
		public static string GetStateLabel(string stateName)
		{
			return Labels.GetStateLabel(stateName);
		}
		[Obsolete("Use Labels.GetEventLabel instead.")]
		public static GUIContent GetEventLabel(SkillTransition transition)
		{
			return Labels.GetEventLabel(transition);
		}
		[Obsolete("Use Labels.GetCurrentStateLabel instead.")]
		public static string GetCurrentStateLabel(Skill fsm)
		{
			return Labels.GetCurrentStateLabel(fsm);
		}
		[Obsolete("Use Labels.GetFsmLabel instead.")]
		public static string GetFsmLabel(Skill fsm)
		{
			return Labels.GetFsmLabel(fsm);
		}
		[Obsolete("Use Labels.GetFullFsmLabel instead.")]
		public static string GetFullFsmLabel(PlayMakerFSM fsmComponent)
		{
			return Labels.GetFullFsmLabel(fsmComponent);
		}
		[Obsolete("Use Labels.GetFullFsmLabel instead.")]
		public static string GetFullFsmLabel(Skill fsm)
		{
			return Labels.GetFullFsmLabel(fsm);
		}
		[Obsolete("Use Labels.GetRuntimeFsmLabel instead.")]
		public static string GetRuntimeFsmLabel(Skill fsm)
		{
			return Labels.GetRuntimeFsmLabel(fsm);
		}
		[Obsolete("Use Labels.GetFullFsmLabelWithInstanceID instead.")]
		public static string GetFullFsmLabelWithInstanceID(Skill fsm)
		{
			return Labels.GetFullFsmLabelWithInstanceID(fsm);
		}
		[Obsolete("Use Labels.GetFullFsmLabelWithInstanceID instead.")]
		public static string GetFullFsmLabelWithInstanceID(PlayMakerFSM fsm)
		{
			return Labels.GetFullFsmLabelWithInstanceID(fsm);
		}
		[Obsolete("Use Labels.GetRuntimeFsmLabelToFit instead")]
		public static GUIContent GetRuntimeFsmLabelToFit(Skill fsm, float width, GUIStyle style)
		{
			return Labels.GetRuntimeFsmLabelToFit(fsm, width, style);
		}
		[Obsolete("Use Labels.GetFullStateLabel instead.")]
		public static string GetFullStateLabel(SkillState state)
		{
			return Labels.GetFullStateLabel(state);
		}
		[Obsolete("Use Labels.GetUniqueFsmName instead.")]
		public static string GenerateUniqueFsmName(GameObject go)
		{
			return Labels.GetUniqueFsmName(go);
		}
		[Obsolete("Use Labels.GetActionLabel instead.")]
		public static string GetActionLabel(Type actionType)
		{
			return Labels.GetActionLabel(actionType);
		}
		[Obsolete("Use Labels.GetActionLabel instead.")]
		public static string GetActionLabel(SkillStateAction action)
		{
			return Labels.GetActionLabel(action);
		}
		[Obsolete("Use Labels.GetTypeTooltip instead.")]
		public static string GetTypeTooltip(Type type)
		{
			return Labels.GetTypeTooltip(type);
		}
		[Obsolete("Use Labels.GetFsmIndex instead.")]
		public static int GetFsmIndex(Skill fsm)
		{
			return Labels.GetFsmIndex(fsm);
		}
		[Obsolete("Use Labels.GetFsmNameIndex instead.")]
		public static int GetFsmNameIndex(Skill fsm)
		{
			return Labels.GetFsmNameIndex(fsm);
		}
		[Obsolete("Use GlobalsAsset.Export instead.")]
		public static void ExportGlobals()
		{
			GlobalsAsset.Export();
		}
		[Obsolete("Use GlobalsAsset.Import instead.")]
		public static void ImportGlobals()
		{
			GlobalsAsset.Import();
		}
		[Obsolete("Use FsmVariable.Sort instead.")]
		public static void SortFsmVariables(Skill fsm)
		{
			VariableManager.SortVariables(fsm);
		}
		[Obsolete("Use FsmVariable.Sort instead.")]
		public static void SortFsmVariables(SkillVariables fsmVariables)
		{
			VariableManager.SortVariables(fsmVariables);
		}
		[Obsolete("Use Watermarks.GetLabel instead.")]
		public static string GetWatermarkLabel(PlayMakerFSM fsmComponent, string defaultLabel = "No Watermark")
		{
			return Watermarks.GetLabel(fsmComponent, defaultLabel);
		}
		[Obsolete("Use Watermarks.Set instead.")]
		public static Texture SetWatermarkTexture(Skill fsm, string textureName)
		{
			return Watermarks.Set(fsm, textureName);
		}
		[Obsolete("Use Watermarks.Get instead.")]
		public static Texture GetWatermarkTexture(Skill fsm)
		{
			return Watermarks.Get(fsm);
		}
		[Obsolete("Use Watermarks.Load instead.")]
		public static Texture LoadWatermarkTexture(string name)
		{
			return Watermarks.Load(name);
		}
		[Obsolete("Use Watermarks.GetNames instead.")]
		public static string[] GetWatermarkNames()
		{
			return Watermarks.GetNames();
		}
		[Obsolete("Use Watermarks.GetTextures instead.")]
		public static Texture[] GetWatermarkTextures(bool showProgress = true)
		{
			return Watermarks.GetTextures(showProgress);
		}
		[Obsolete("Use ActionScripts.PingAsset instead.")]
		public static void FindActionScript(object userdata)
		{
			ActionScripts.PingAsset(userdata);
		}
		[Obsolete("Use ActionScripts.EditAsset instead.")]
		public static void EditActionScript(object userdata)
		{
			ActionScripts.EditAsset(userdata);
		}
		[Obsolete("Use ActionScripts.PingAssetByType instead.")]
		public static void FindActionTypeScript(object userdata)
		{
			ActionScripts.PingAssetByType(userdata);
		}
		[Obsolete("Use ActionScripts.SelectAssetByType instead.")]
		public static void SelectActionTypeScript(object userdata)
		{
			ActionScripts.SelectAssetByType(userdata);
		}
		[Obsolete("Use ActionScripts.EditAssetByType instead.")]
		public static void EditActionTypeScript(object userdata)
		{
			ActionScripts.EditAssetByType(userdata);
		}
		[Obsolete("Use ActionScripts.GetAsset instead.")]
		public static Object GetActionScriptAsset(SkillStateAction action)
		{
			return ActionScripts.GetAsset(action);
		}
		[Obsolete("Use ActionScripts.GetAsset instead.")]
		public static Object GetActionScriptAsset(Type actionType)
		{
			return ActionScripts.GetAsset(actionType);
		}
		[Obsolete("Use FsmVariable.GetVariableType instead.")]
		public static Type GetUIHintVariableType(UIHint hint)
		{
			return SkillVariable.GetVariableType(hint);
		}
		[Obsolete("Use Templates.LoadAll instead.")]
		public static void LoadAllTemplates()
		{
			Templates.LoadAll();
		}
		[Obsolete("Use Templates.InitList instead.")]
		public static void BuildTemplateList()
		{
			Templates.InitList();
		}
		[Obsolete("Use Templates.SortList instead.")]
		public static void SortTemplateList()
		{
			Templates.SortList();
		}
		[Obsolete("Use Templates.DoSelectTemplateMenu instead.")]
		public static void DoSelectTemplateMenu(SkillTemplate SelectedTemplate, GenericMenu.MenuFunction ClearTemplate, GenericMenu.MenuFunction2 SelectTemplate)
		{
			Templates.DoSelectTemplateMenu(SelectedTemplate, ClearTemplate, SelectTemplate);
		}
		[Obsolete("Use FsmEventManager.SanityCheckEventList instead.")]
		public static void SanityCheckEventList(Skill fsm)
		{
			FsmEventManager.SanityCheckEventList(fsm);
		}
		[Obsolete("Use Events.GetFsmTarget instead.")]
		public static Skill GetFsmTarget(Skill fsm, SkillEventTarget fsmEventTarget)
		{
			return Events.GetFsmTarget(fsm, fsmEventTarget);
		}
		[Obsolete("Use Events.FsmStateRespondsToEvent instead.")]
		public static bool FsmStateRespondsToEvent(SkillState state, SkillEvent fsmEvent)
		{
			return Events.FsmStateRespondsToEvent(state, fsmEvent);
		}
		[Obsolete("Use Events.FsmRespondsToEvent instead.")]
		public static bool FsmRespondsToEvent(Skill fsm, SkillEvent fsmEvent)
		{
			return Events.FsmRespondsToEvent(fsm, fsmEvent);
		}
		[Obsolete("Use Events.FsmRespondsToEvent instead.")]
		public static bool FsmRespondsToEvent(Skill fsm, string fsmEventName)
		{
			return Events.FsmRespondsToEvent(fsm, fsmEventName);
		}
		[Obsolete("Use Events.GetGlobalEventList instead.")]
		public static List<SkillEvent> GetGlobalEventList()
		{
			return Events.GetGlobalEventList();
		}
		[Obsolete("Use Events.GetGlobalEventList instead.")]
		public static List<SkillEvent> GetGlobalEventList(Skill fsm)
		{
			return Events.GetGlobalEventList(fsm);
		}
		[Obsolete("Use Events.GetEventList instead.")]
		public static List<SkillEvent> GetEventList(Skill fsm)
		{
			return Events.GetEventList(fsm);
		}
		[Obsolete("Use Events.GetEventList instead.")]
		public static List<SkillEvent> GetEventList(PlayMakerFSM fsmComponent)
		{
			return Events.GetEventList(fsmComponent);
		}
		[Obsolete("Use Events.GetEventList instead.")]
		public static List<SkillEvent> GetEventList(GameObject go)
		{
			return Events.GetEventList(go);
		}
		[Obsolete("Use Events.GetEventNamesFromList instead.")]
		public static GUIContent[] GetEventNamesFromList(List<SkillEvent> eventList)
		{
			return Events.GetEventNamesFromList(eventList);
		}
		[Obsolete("Use Events.EventListContainsEventName instead.")]
		public static bool EventListContainsEventName(List<SkillEvent> eventList, string fsmEventName)
		{
			return Events.EventListContainsEventName(eventList, fsmEventName);
		}
		[Obsolete("Use Files.LoadTextureFromDll instead.")]
		public static Texture2D LoadDllResource(string resourceName, int width, int height)
		{
			return Files.LoadTextureFromDll(resourceName, width, height);
		}
		[Obsolete("Use Files.LoadAllAssetsOfType instead.")]
		public static void LoadAllAssetsOfType(string type)
		{
			Files.LoadAllAssetsOfType(type);
		}
		[Obsolete("Use Files.GetFiles instead.")]
		public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption = 0)
		{
			return Files.GetFiles(path, searchPattern, searchOption);
		}
		[Obsolete("Use Files.CreateFilePath instead.")]
		public static bool CreateFilePath(string fullFileName)
		{
			return Files.CreateFilePath(fullFileName);
		}
		[Obsolete("Use Files.BuildScriptList instead.")]
		public static void BuildScriptList()
		{
			Files.BuildScriptList();
		}
		[Obsolete("Use Files.LoadAllPlaymakerPrefabs instead.")]
		public static List<string> LoadAllPrefabsInProject()
		{
			return Files.LoadAllPlaymakerPrefabs();
		}
		[Obsolete("Removed. Use CustomAttributeHelpers.GetAttribute<> instead.")]
		public static Type GetCustomActionEditorAttribute(Type objType)
		{
			return null;
		}
		[Obsolete("Removed. Use CustomAttributeHelpers.GetAttribute<> instead.")]
		public static Type GetCustomActionEditorAttribute(object[] attributes)
		{
			return null;
		}
		[Obsolete("Removed. Use CustomAttributeHelpers.GetAttribute<> instead.")]
		public static Type GetPropertyDrawerAttribute(Type objType)
		{
			return null;
		}
		[Obsolete("Removed. Use CustomAttributeHelpers.GetAttribute<> instead.")]
		public static Type GetPropertyDrawerAttribute(object[] attributes)
		{
			return null;
		}
		[Obsolete("Removed. Use CustomAttributeHelpers.GetAttribute<> instead.")]
		public static Type GetObjectPropertyDrawerAttribute(Type objType)
		{
			return null;
		}
		[Obsolete("Removed. Use CustomAttributeHelpers.GetAttribute<> instead.")]
		public static Type GetObjectPropertyDrawerAttribute(object[] attributes)
		{
			return null;
		}
		[Obsolete("Removed. Use CustomAttributeHelpers.GetAttribute<> instead.")]
		public static string GetHelpUrlAttribute(Type objType)
		{
			return null;
		}
		[Obsolete("Removed. Use CustomAttributeHelpers.GetAttribute<> instead.")]
		public static string GetHelpUrlAttribute(object[] attributes)
		{
			return null;
		}
		[Obsolete("Removed. Use CustomAttributeHelpers.GetActionSection instead.")]
		public static string GetFieldSection(object[] attributes)
		{
			return null;
		}
		[Obsolete("Use FsmEditorSettings.PackColorIntoInt instead.")]
		public static int PackColorIntoInt(Color color)
		{
			return FsmEditorSettings.PackColorIntoInt(color);
		}
		[Obsolete("Use FsmEditorSettings.UnpackColorFromInt instead.")]
		public static Color UnpackColorFromInt(int packedValue)
		{
			return FsmEditorSettings.UnpackColorFromInt(packedValue);
		}
		[Obsolete("Use FsmState.GetStateIndex instead.")]
		public static int GetStateIndex(SkillState state)
		{
			return SkillState.GetStateIndex(state);
		}
		[Obsolete("Use FsmPrefabs.LoadUsedPrefabs instead.")]
		public static void LoadUsedPrefabs()
		{
			SkillPrefabs.LoadUsedPrefabs();
		}
		[Obsolete("Use FsmPrefabs.IsModifiedPrefabInstance instead.")]
		public static bool IsModifiedPrefabInstance(Skill fsm)
		{
			return SkillPrefabs.IsModifiedPrefabInstance(fsm);
		}
		[Obsolete("Use FsmPrefabs.UpdateIsModifiedPrefabInstance instead.")]
		public static void UpdateIsModifiedPrefabInstance(Skill fsm)
		{
			SkillPrefabs.UpdateIsModifiedPrefabInstance(fsm);
		}
		[Obsolete("Use FsmPrefabs.ShouldModify instead.")]
		public static bool ShouldModify(Skill fsm)
		{
			return SkillPrefabs.ShouldModify(fsm);
		}
		[Obsolete("Use FsmPrefabs.IsPersistent instead.")]
		public static bool IsPersistent(Object obj)
		{
			return SkillPrefabs.IsPersistent(obj);
		}
		[Obsolete("Use FsmPrefabs.IsPrefab instead.")]
		public static bool IsPrefab(Skill fsm)
		{
			return SkillPrefabs.IsPrefab(fsm);
		}
		[Obsolete("Use FsmPrefabs.IsPrefabInstance instead.")]
		public static bool IsPersistent(Skill fsm)
		{
			return SkillPrefabs.IsPrefabInstance(fsm);
		}
		[Obsolete("Use FsmPrefabs.IsPrefabInstance instead.")]
		public static bool IsPrefabInstance(Skill fsm)
		{
			return SkillPrefabs.IsPrefabInstance(fsm);
		}
		[Obsolete("Use FsmPrefabs.IsFsmInstanceOfPrefab instead.")]
		public static bool IsFsmInstanceOfPrefab(Skill fsm, Skill prefab)
		{
			return SkillPrefabs.IsFsmInstanceOfPrefab(fsm, prefab);
		}
		[Obsolete("Use FsmPrefabs.BuildAssetsWithPlayMakerFSMsList instead.")]
		public static void BuildAssetsWithPlayMakerFSMsList()
		{
			SkillPrefabs.BuildAssetsWithPlayMakerFSMsList();
		}
		[Obsolete("Use FsmPrefabs.AssetHasPlayMakerFSM instead.")]
		public static bool AssetHasPlayMakerFSM(string guid)
		{
			return SkillPrefabs.AssetHasPlayMakerFSM(guid);
		}
		[Obsolete("Use FsmPrefabs.StateExistsInPrefabParent instead.")]
		public static bool StateExistsInPrefabParent(SkillState state)
		{
			return SkillPrefabs.StateExistsInPrefabParent(state);
		}
		[Obsolete("Use TypeHelpers.GetSerializedFields instead.")]
		public static List<Type> GetDerivedTypeList(Type ofType, bool includeBaseType = true)
		{
			return TypeHelpers.GetDerivedTypeList(ofType, includeBaseType);
		}
		[Obsolete("Use TypeHelpers.GetSerializedFields instead.")]
		public static IEnumerable<FieldInfo> GetSerializedFields(Type type)
		{
			return TypeHelpers.GetSerializedFields(type);
		}
		[Obsolete("Use TypeHelpers.GenerateObjectTypesMenu instead.")]
		public static GenericMenu GenerateObjectTypesMenu(SkillProperty fsmProperty)
		{
			return TypeHelpers.GenerateObjectTypesMenu(fsmProperty);
		}
		[Obsolete("Use TypeHelpers.GenerateObjectTypesMenu instead.")]
		public static GenericMenu GenerateObjectTypesMenu(SkillVariable fsmVariable)
		{
			return TypeHelpers.GenerateObjectTypesMenu(fsmVariable);
		}
		[Obsolete("Use TypeHelpers.GenerateEnumTypesMenu instead.")]
		public static GenericMenu GenerateEnumTypesMenu(SkillVariable fsmVariable)
		{
			return TypeHelpers.GenerateEnumTypesMenu(fsmVariable);
		}
		[Obsolete("Use TypeHelpers.GeneratePropertyMenu instead.")]
		public static GenericMenu GeneratePropertyMenu(SkillProperty fsmProperty)
		{
			return TypeHelpers.GeneratePropertyMenu(fsmProperty);
		}
		[Obsolete("Use TypeHelpers.IsSupportedParameterType instead.")]
		public static bool IsSupportedParameterType(Type parameterType)
		{
			return TypeHelpers.IsSupportedParameterType(parameterType);
		}
		[Obsolete("Removed")]
		public static void GenerateSubPropertyMenu(object userdata)
		{
		}
		[Obsolete("Removed")]
		public static void OpenSubPropertyMenu()
		{
		}
		[Obsolete("Removed")]
		public static Skill GetGameObjectFSM(SkillGameObject go, SkillString fsmName)
		{
			return null;
		}
		[Obsolete("Use FsmSelection.FindFsmOnGameObject instead.")]
		public static Skill FindFsmOnGameObject(GameObject go)
		{
			return SkillSelection.FindFsmOnGameObject(go);
		}
		[Obsolete("Use FsmSelection.FindFsmComponentOnGameObject instead.")]
		public static PlayMakerFSM FindFsmComponentOnGameObject(GameObject go)
		{
			return SkillSelection.FindFsmComponentOnGameObject(go);
		}
		[Obsolete("Use FsmSelection.FindFsmOnGameObject instead.")]
		public static Skill FindFsmOnGameObject(GameObject go, string name)
		{
			return SkillSelection.FindFsmOnGameObject(go, name);
		}
		[Obsolete("Use FsmSelection.GameObjectHasFSM instead.")]
		public static bool GameObjectHasFSM(GameObject go)
		{
			return SkillSelection.GameObjectHasFSM(go);
		}
	}
}
