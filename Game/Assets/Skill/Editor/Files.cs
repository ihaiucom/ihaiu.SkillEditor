using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace ihaiu
{
    [Localizable(false)]
    public class Files
    {
        private static List<string> scriptList;
        private static string[] scriptPopupNames;
        public static List<string> ScriptList
        {
            get
            {
                if (Files.scriptList == null)
                {
                    Files.BuildScriptList();
                }
                return Files.scriptList;
            }
        }
        public static string[] ScriptPopupNames
        {
            get
            {
                if (Files.scriptList == null)
                {
                    Files.BuildScriptList();
                }
                return Files.scriptPopupNames;
            }
        }
        public static Texture2D LoadTextureFromDll(string resourceName, int width, int height)
        {
            string[] array = new string[]
                {
                    "timelineBar",
                    "smallLeftArrow",
                    "smallRightArrow"
                };
            Texture2D texture2D;
            if (EditorApp.IsSourceCodeVersion)
            {
                texture2D = (Resources.Load(resourceName, typeof(Texture2D)) as Texture2D);
                if (texture2D == null)
                {
                    Debug.LogError(Strings.Could_not_load_resource + resourceName);
                    texture2D = new Texture2D(2, 2);
                }
                return texture2D;
            }
            texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false, true);
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("HutongGames.PlayMakerEditor.Playmaker.source.unity.Assets.PlayMaker.Editor.Resources." + resourceName + ".png");
            if (manifestResourceStream != null)
            {
                texture2D.LoadImage(Files.ReadToEnd(manifestResourceStream));
                if (Enumerable.Contains<string>(array, resourceName))
                {
                    texture2D.filterMode = FilterMode.Point;
                }
                manifestResourceStream.Close();
            }
            else
            {
                Debug.LogWarning(string.Format(Strings.Label_Missing_Dll_resource, resourceName));
            }
            texture2D.hideFlags = HideFlags.HideAndDontSave;
            return texture2D;
        }
        private static byte[] ReadToEnd(Stream stream)
        {
            
            long position = stream.Position;
            stream.Position = 0L;
            byte[] result;
            try
            {
                byte[] array = new byte[4096];
                int num = 0;
                int num2;
                while ((num2 = stream.Read(array, num, array.Length - num)) > 0)
                {
                    num += num2;
                    if (num == array.Length)
                    {
                        int num3 = stream.ReadByte();
                        if (num3 != -1)
                        {
                            byte[] array2 = new byte[array.Length * 2];
                            Buffer.BlockCopy(array, 0, array2, 0, array.Length);
                            Buffer.SetByte(array2, num, (byte)num3);
                            array = array2;
                            num++;
                        }
                    }
                }
                byte[] array3 = array;
                if (array.Length != num)
                {
                    array3 = new byte[num];
                    Buffer.BlockCopy(array, 0, array3, 0, num);
                }
                result = array3;
            }
            finally
            {
                stream.Position = position;
            }
            return result;
        }
        public static void LoadAllAssetsOfType(string type)
        {
            HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
            hierarchyProperty.SetSearchFilter(type, 2);
            while (hierarchyProperty.Next(null))
            {
                AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(hierarchyProperty.instanceID));
            }
        }
        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption = 0)
        {
            string[] array = searchPattern.Split(new char[]
                {
                    '|'
                });
            List<string> list = new List<string>();
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string text = array2[i];
                list.AddRange(Directory.GetFiles(path, text, searchOption));
            }
            list.Sort();
            return list.ToArray();
        }
        public static bool CreateFilePath(string fullFileName)
        {
            string directoryName = Path.GetDirectoryName(fullFileName);
            if (string.IsNullOrEmpty(directoryName))
            {
                Debug.LogError(string.Format(Strings.Error_Invalid_path, fullFileName));
                return false;
            }
            try
            {
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }
            catch (Exception)
            {
                Debug.LogError(string.Format(Strings.Error_Could_not_create_directory, directoryName));
                return false;
            }
            return true;
        }
        public static string GetFsmSavePath(Skill fsm)
        {
            if (fsm == null)
            {
                return "";
            }
            if (fsm.UsedInTemplate)
            {
                return "Template." + fsm.UsedInTemplate.Name;
            }
            string text = fsm.OwnerName + '.' + Labels.GetFsmLabel(fsm);
            if (SkillPrefabs.IsPrefab(fsm))
            {
                return "Prefab." + text;
            }
            return text;
        }
        public static void BuildScriptList()
        {
            Files.scriptList = new List<string>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];
                try
                {
                    Type[] exportedTypes = assembly.GetExportedTypes();
                    Type[] array = exportedTypes;
                    for (int j = 0; j < array.Length; j++)
                    {
                        Type type = array[j];
                        if (Files.ShowScriptInMenu(type))
                        {
                            Files.scriptList.Add(type.FullName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    NotSupportedException arg_65_0 = ex as NotSupportedException;
                }
            }
            Files.scriptList.Sort();
            List<string> list = new List<string>();
            list.Add(Strings.Label_None);
            List<string> list2 = list;
            using (List<string>.Enumerator enumerator = Files.ScriptList.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    if (!Enumerable.Contains<char>(current, '.'))
                    {
                        list2.Add("No Namespace/" + current);
                    }
                    else
                    {
                        list2.Add(current.Replace('.', '/'));
                    }
                }
            }
            Files.scriptPopupNames = list2.ToArray();
        }
        private static bool ShowScriptInMenu(Type type)
        {
            return false;
//            return type.IsSubclassOf(typeof(Component)) && !type.IsSubclassOf(typeof(PlayMakerProxyBase)) && type.get_IsClass() && !type.get_IsAbstract() && type != typeof(Behaviour) && type != typeof(MonoBehaviour) && type != typeof(PlayMakerProxyBase) && type != typeof(PlayMakerOnGUI);
        }
        public static string GetScriptName(int popupIndex)
        {
            if (popupIndex <= 0 || popupIndex >= Files.ScriptPopupNames.Length)
            {
                return "";
            }
            return Files.ScriptPopupNames[popupIndex].Replace("No Namespace/", "").Replace('/', '.');
        }
        public static List<string> LoadAllPlaymakerPrefabs()
        {
            return Files.LoadGameObjectsWithFsmComponent();
        }
        private static List<string> LoadGameObjectsWithFsmComponent()
        {
            return new List<string>();
//            Debug.Log(Strings.get_Log_Loading_all_FSMs());
//            DirectoryInfo directoryInfo = new DirectoryInfo(Application.get_dataPath());
//            FileInfo[] files = directoryInfo.GetFiles("*.prefab", 1);
//            List<string> list = new List<string>();
//            float num = (float)files.Length;
//            for (int i = 0; i < files.Length; i++)
//            {
//                FileInfo fileInfo = files[i];
//                EditorUtility.DisplayProgressBar(Strings.get_Dialog_Loading_FSM_Prefabs(), Strings.get_Label_Note_disable_in_preferences(), (float)i / num);
//                string text = fileInfo.get_FullName().Replace('\\', '/').Replace(Application.get_dataPath(), "Assets");
//                string[] dependencies = AssetDatabase.GetDependencies(new string[]
//                    {
//                        text
//                    });
//                string[] array = dependencies;
//                for (int j = 0; j < array.Length; j++)
//                {
//                    string text2 = array[j];
//                    if (text2.Contains("/PlayMaker.dll"))
//                    {
//                        Debug.Log(Strings.get_Found_Prefab_with_FSM() + text);
//                        list.Add(text);
//                        AssetDatabase.LoadAssetAtPath(text, typeof(GameObject));
//                    }
//                }
//            }
//            EditorUtility.ClearProgressBar();
//            return list;
        }
    }
}
