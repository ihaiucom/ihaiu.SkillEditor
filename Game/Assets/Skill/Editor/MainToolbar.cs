using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ihaiu
{
    internal class MainToolbar
    {
        public void OnGUI()
        {
            float num = SkillEditor.Window.position.width - 350f;
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[]
                {
                    GUILayout.Width(num)
                });
            MainToolbar.DoFsmSelectorGUI();
            MainToolbar.DoPrefabTypeGUI();
            GUILayout.FlexibleSpace();
            bool flag = GUILayout.Toggle(SkillEditorSettings.GraphViewShowMinimap, SkillEditorContent.MainToolbarShowMinimap, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            if (flag != SkillEditorSettings.GraphViewShowMinimap)
            {
                SkillEditorSettings.GraphViewShowMinimap = flag;
                SkillEditorSettings.SaveSettings();
            }
            EditorGUILayout.EndHorizontal();
        }
        private static void DoFsmSelectorGUI()
        {
//            SkillEditorContent.MainToolbarSelectedGO.text = (SkillEditor.SelectedFsmGameObject == null) ? Strings.Label_None : SkillEditor.SelectedFsmGameObject.name;
//            SkillEditorContent.MainToolbarSelectedGO.tooltip = Strings.Hint_Select_Game_Object;
//            EditorGUI.BeginDisabledGroup(!SkillEditor.SelectionHistory.CanMoveBack());
//            if (GUILayout.Button(SkillEditorContent.BackButton, EditorStyles.toolbarButton, new GUILayoutOption[0]))
//            {
//                SkillEditor.SelectFsm(SkillEditor.SelectionHistory.MoveBack());
//            }
//            EditorGUI.EndDisabledGroup();
//            EditorGUI.BeginDisabledGroup(!SkillEditor.SelectionHistory.CanMoveForward());
//            if (GUILayout.Button(SkillEditorContent.ForwardButton, EditorStyles.toolbarButton, new GUILayoutOption[0]))
//            {
//                SkillEditor.SelectFsm(SkillEditor.SelectionHistory.MoveForward());
//            }
//            EditorGUI.EndDisabledGroup();
//            EditorGUI.BeginDisabledGroup(SkillEditor.SelectionHistory.RecentlySelectedCount <= 0);
//            if (GUILayout.Button(SkillEditorContent.RecentButton, EditorStyles.toolbarButton, new GUILayoutOption[0]))
//            {
//                GenericMenu genericMenu = new GenericMenu();
//                List<Skill> recentlySelectedFSMs = SkillEditor.SelectionHistory.GetRecentlySelectedFSMs();
//                using (List<Skill>.Enumerator enumerator = recentlySelectedFSMs.GetEnumerator())
//                {
//                    while (enumerator.MoveNext())
//                    {
//                        Skill current = enumerator.get_Current();
//                        genericMenu.AddItem(new GUIContent(Labels.GetFullFsmLabel(current)), current == SkillEditor.SelectedFsm, new GenericMenu.MenuFunction2(SkillEditor.SelectFsm), current);
//                    }
//                }
//                genericMenu.ShowAsContext();
//                return;
//            }
//            EditorGUI.EndDisabledGroup();
//            if (GUILayout.Button(SkillEditorContent.MainToolbarSelectedGO, EditorStyles.toolbarDropDown, new GUILayoutOption[]
//                {
//                    GUILayout.MinWidth(100f)
//                }))
//            {
//                SkillEditorGUILayout.GenerateFsmGameObjectSelectionMenu(true).ShowAsContext();
//            }
//            SkillEditorContent.MainToolbarSelectedFSM.set_text((SkillEditor.SelectedFsm == null) ? "" : SkillEditor.SelectedFsm.get_Name());
//            SkillEditorContent.MainToolbarSelectedFSM.set_tooltip(Strings.get_Tooltip_Select_FSM());
//            if (GUILayout.Button(SkillEditorContent.MainToolbarSelectedFSM, EditorStyles.toolbarDropDown, new GUILayoutOption[]
//                {
//                    GUILayout.MinWidth(100f)
//                }))
//            {
//                SkillEditorGUILayout.GenerateGameObjectFsmSelectionMenu().ShowAsContext();
//            }
//            FsmEditorSettings.LockGraphView = GUILayout.Toggle(FsmEditorSettings.LockGraphView, SkillEditorContent.MainToolbarLock, EditorStyles.toolbarButton, new GUILayoutOption[0]);
//            if (GUI.get_changed())
//            {
//                FsmEditorSettings.SaveSettings();
//            }
        }
        private static void DoPrefabTypeGUI()
        {
//            if (SkillEditor.SelectedFsmGameObject == null || SkillEditor.SelectedTemplate != null)
//            {
//                return;
//            }
//            bool isModifiedPrefabInstance = SkillEditor.SelectedFsm.IsModifiedPrefabInstance();
//            string text = "";
//            if (isModifiedPrefabInstance)
//            {
//                text = Strings.get_Label_Modified_postfix();
//            }
//            switch (SkillEditor.Selection.ActiveFsmPrefabType)
//            {
//                case 0:
//                    if (GUILayout.Button(SkillEditorContent.MainToolbarPrefabTypeNone, EditorStyles.toolbarButton, new GUILayoutOption[0]))
//                    {
//                        SkillEditor.SelectFsmGameObject();
//                        return;
//                    }
//                    break;
//                case 1:
//                    if (GUILayout.Button(Strings.get_Label_Prefab(), EditorStyles.toolbarDropDown, new GUILayoutOption[0]))
//                    {
//                        GenericMenu genericMenu = new GenericMenu();
//                        genericMenu.AddItem(new GUIContent(Strings.get_Menu_Select_Prefab()), false, new GenericMenu.MenuFunction(SkillEditor.SelectFsmGameObject));
//                        genericMenu.AddItem(new GUIContent(Strings.get_Menu_Make_Instance()), false, new GenericMenu.MenuFunction(SkillEditor.InstantiatePrefab));
//                        genericMenu.ShowAsContext();
//                        return;
//                    }
//                    break;
//                case 2:
//                    if (GUILayout.Button(Strings.get_Label_Model_Prefab(), EditorStyles.toolbarDropDown, new GUILayoutOption[0]))
//                    {
//                        GenericMenu genericMenu2 = new GenericMenu();
//                        genericMenu2.AddItem(new GUIContent(Strings.get_Menu_Select_GameObject()), false, new GenericMenu.MenuFunction(SkillEditor.SelectFsmGameObject));
//                        genericMenu2.AddItem(new GUIContent(Strings.get_Menu_Make_Instance()), false, new GenericMenu.MenuFunction(SkillEditor.InstantiatePrefab));
//                        genericMenu2.ShowAsContext();
//                        return;
//                    }
//                    break;
//                case 3:
//                    if (GUILayout.Button(Strings.get_Label_Prefab_Instance() + text, EditorStyles.toolbarDropDown, new GUILayoutOption[0]))
//                    {
//                        GenericMenu genericMenu3 = new GenericMenu();
//                        genericMenu3.AddItem(new GUIContent(Strings.get_Menu_Select_GameObject()), false, new GenericMenu.MenuFunction(SkillEditor.SelectFsmGameObject));
//                        if (isModifiedPrefabInstance)
//                        {
//                            genericMenu3.AddItem(new GUIContent(Strings.get_Menu_Revert_To_Prefab()), false, new GenericMenu.MenuFunction(SkillEditor.ResetToPrefabState));
//                        }
//                        else
//                        {
//                            genericMenu3.AddDisabledItem(new GUIContent(Strings.get_Menu_Revert_To_Prefab()));
//                        }
//                        genericMenu3.AddItem(new GUIContent(Strings.get_Menu_Select_Prefab()), false, new GenericMenu.MenuFunction(SkillEditor.SelectPrefabParent));
//                        genericMenu3.ShowAsContext();
//                        return;
//                    }
//                    break;
//                case 4:
//                    if (GUILayout.Button(Strings.get_Label_Model_Prefab_Instance(), EditorStyles.toolbarDropDown, new GUILayoutOption[0]))
//                    {
//                        GenericMenu genericMenu4 = new GenericMenu();
//                        genericMenu4.AddItem(new GUIContent(Strings.get_Menu_Select_GameObject()), false, new GenericMenu.MenuFunction(SkillEditor.SelectFsmGameObject));
//                        if (isModifiedPrefabInstance)
//                        {
//                            genericMenu4.AddItem(new GUIContent(Strings.get_Menu_Revert_To_Prefab()), false, new GenericMenu.MenuFunction(SkillEditor.ResetToPrefabState));
//                        }
//                        else
//                        {
//                            genericMenu4.AddDisabledItem(new GUIContent(Strings.get_Menu_Revert_To_Prefab()));
//                        }
//                        genericMenu4.AddItem(new GUIContent(Strings.get_Menu_Select_Prefab_Parent()), false, new GenericMenu.MenuFunction(SkillEditor.SelectPrefabParent));
//                        genericMenu4.ShowAsContext();
//                        return;
//                    }
//                    break;
//                case 5:
//                    break;
//                case 6:
//                    if (GUILayout.Button(Strings.get_Label_Prefab_Instance_disconnected(), EditorStyles.toolbarDropDown, new GUILayoutOption[0]))
//                    {
//                        GenericMenu genericMenu5 = new GenericMenu();
//                        genericMenu5.AddItem(new GUIContent(Strings.get_Menu_Select_GameObject()), false, new GenericMenu.MenuFunction(SkillEditor.SelectFsmGameObject));
//                        genericMenu5.AddItem(new GUIContent(Strings.get_Menu_Reconnect_to_Prefab()), false, new GenericMenu.MenuFunction(SkillEditor.ReconnectToLastPrefab));
//                        genericMenu5.ShowAsContext();
//                        return;
//                    }
//                    break;
//                case 7:
//                    if (GUILayout.Button(Strings.get_Label_Model_Prefab_Instance_disconnected(), EditorStyles.toolbarDropDown, new GUILayoutOption[0]))
//                    {
//                        GenericMenu genericMenu6 = new GenericMenu();
//                        genericMenu6.AddItem(new GUIContent(Strings.get_Menu_Select_GameObject()), false, new GenericMenu.MenuFunction(SkillEditor.SelectFsmGameObject));
//                        genericMenu6.AddItem(new GUIContent(Strings.get_Menu_Reconnect_to_Prefab()), false, new GenericMenu.MenuFunction(SkillEditor.ReconnectToLastPrefab));
//                        genericMenu6.ShowAsContext();
//                    }
//                    break;
//                default:
//                    return;
//            }
        }
    }
}
