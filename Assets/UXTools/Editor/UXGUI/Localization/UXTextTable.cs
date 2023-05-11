using System.Collections.Generic;
using UnityEditor;
using ThunderFireUnityEx;
using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEditor.SceneManagement;
using ThunderFireUITool;
using System.IO;
using OfficeOpenXml;

namespace UnityEngine.UI
{
    [InitializeOnLoad]
    public class UXTextTable
    {
        private static bool hasChanged = false;

        static UXTextTable()
        {
            PrefabStageUtils.AddSavedEvent(SyncTextTable);
            // EditorSceneManager.sceneSaved += SyncTextTable;
        }

        public static void CreateRuntimeTable(bool alwaysShowPanel = false)
        {
            string path = UXGUIConfig.RuntimeTablePath;
            if(File.Exists(path) && !alwaysShowPanel) return;
            path = Utils.SaveFile("RuntimeTable", "xlsx");
            if(path == null) return;
            FileInfo newFile = new FileInfo(path);
            if(newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(path);
            }
            using(ExcelPackage package = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Runtime");
                worksheet.Cells[1, 1].Value = "key";
                worksheet.Cells[1, 2].Value = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_路径);
                worksheet.Cells[1, 3].Value = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_原文);
                for(int i = 0; i < LocalizationLanguage.Length; i++)
                {
                    worksheet.Cells[1, 4 + i].Value = LocalizationLanguage.GetLanguage(i);
                }
                package.Save();
            }
            AssetDatabase.ImportAsset(path);
            UXGUIConfig.RuntimeTablePath = path;
        }
        public static void CreatePreviewTable(bool alwaysShowPanel = false)
        {
            string path = UXGUIConfig.PreviewTablePath;
            if(File.Exists(path) && !alwaysShowPanel) return;
            path = Utils.SaveFile("PreviewTable", "xlsx");
            if(path == null) return;
            FileInfo newFile = new FileInfo(path);
            if(newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(path);
            }
            using(ExcelPackage package = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Preview");
                worksheet.Cells[1, 1].Value = "key";
                for(int i = 0; i < LocalizationLanguage.Length; i++)
                {
                    worksheet.Cells[1, 2 + i].Value = LocalizationLanguage.GetLanguage(i);
                }
                package.Save();
            }
            AssetDatabase.ImportAsset(path);
            UXGUIConfig.PreviewTablePath = path;
        }

        [MenuItem("ThunderFireUXTool/本地化 (Localization)/打开静态文本表格 (Open Runtime-Use Text Table)")]
        public static void OpenTextTable()
        {
            CreateRuntimeTable();
            Application.OpenURL(Application.dataPath.Replace("Assets", "") + UXGUIConfig.RuntimeTablePath);
        }
        [MenuItem("ThunderFireUXTool/本地化 (Localization)/打开动态文本表格 (Open Preview Text Table)")]
        public static void OpenPreviewTextTable()
        {
            CreatePreviewTable();
            Application.OpenURL(Application.dataPath.Replace("Assets", "") + UXGUIConfig.PreviewTablePath);
        }

        [MenuItem("ThunderFireUXTool/本地化 (Localization)/将文本表格转为JSON文件 (Convert Text Table to JSON)")]
        private static void ConvertToJSON()
        {
            ExcelToJsonConverter.ConvertExcelFileToJson(new string[] { UXGUIConfig.RuntimeTablePath, UXGUIConfig.PreviewTablePath }, UXGUIConfig.TextLocalizationJsonPath);
            AssetDatabase.ImportAsset(UXGUIConfig.TextLocalizationJsonPath);
            Selection.activeGameObject = null;
        }

        public static string[] ReadRow(string key)
        {
            LocalizationTextRow[] lines = LocalizationHelper.ReadFromJSON();
            if(lines == null) return null;
            foreach(var line in lines)
            {
                if (line.key == key)
                {
                    return line.translates;
                }
            }
            return null;
        }
        private static List<string[]> ReadAll() {
            List<string[]> lines = new List<string[]>();
            if(File.Exists(UXGUIConfig.RuntimeTablePath))
            {
                using(FileStream stream = new FileStream(UXGUIConfig.RuntimeTablePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using(ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet sheet = package.Workbook.Worksheets[1];
                        for(int m = 2; m <= sheet.Dimension.End.Row; m++)
                        {
                            string[] line = new string[LocalizationLanguage.Length + 3];
                            for(int j = 1; j <= LocalizationLanguage.Length + 3; j++)
                            {
                                line[j - 1] = sheet.GetValue(m, j)?.ToString() ?? "";
                            }
                            lines.Add(line);
                        }
                    }
                }
            }
            return lines;
        }
        
        private static void WriteAll(List<string[]> list)
        {
            try
            {
                FileInfo newFile = new FileInfo(UXGUIConfig.RuntimeTablePath);
                if(newFile.Exists)
                {
                    newFile.Delete();
                    newFile = new FileInfo(UXGUIConfig.RuntimeTablePath);
                }
                using(ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Runtime");
                    worksheet.Cells[1, 1].Value = "key";
                    worksheet.Cells[1, 2].Value = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_路径);
                    worksheet.Cells[1, 3].Value = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_原文);
                    for(int i = 0; i < LocalizationLanguage.Length; i++)
                    {
                        worksheet.Cells[1, 4 + i].Value = LocalizationLanguage.GetLanguage(i);
                    }
                    for(int i = 0; i < list.Count; i++)
                    {
                        for(int j = 0; j < list[i].Length; j++)
                        {
                            worksheet.Cells[2 + i, 1 + j].Value = list[i][j];
                        }
                    }
                    package.Save();
                }
            }
            catch(IOException)
            {
                Debug.LogError(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_无法写入表格));
            }
        }

        private static string MergePath(string origin_path, string new_path)
        {
            if (new_path == "") return origin_path;
            if (origin_path == "") return new_path;
            return origin_path + " && " + new_path;
        }

        private static List<string[]> RemovePath(string filePath)
        {
            List<string[]> list = new List<string[]>();
            List<string[]> lines = ReadAll();
            foreach(string[] line in lines)
            {
                string truePath = "";
                string[] paths = line[1].Split(new[]{" && "}, System.StringSplitOptions.None);
                foreach(string p in paths)
                {
                    if(!p.StartsWith(filePath))
                    {
                        truePath = MergePath(truePath, p);
                    }
                }
                if(truePath == "")
                {
                    bool flag = false;
                    for(int i = 3; i < line.Length; i++)
                    {
                        flag |= line[i] != "";
                    }
                    if(flag)
                    {
                        line[1] = line[2] = "";
                        list.Add(line);
                    }
                }
                else
                {
                    line[1] = truePath;
                    list.Add(line);
                }
            }
            return list;
        }

        private static void AddItem(string[] item, List<string[]> list)
        {
            bool flag = false;
            foreach(string[] line in list)
            {
                if(line[0] == item[0])
                {
                    flag = true;
                    if(line[2] != item[2] && line[1] != "")
                    {
                        Debug.LogWarning($"{line[1]} and {item[1]} have the same key, but the original is different");
                    }
                    line[1] = MergePath(line[1], item[1]);
                    line[2] = item[2];
                    break;
                }
            }
            if(!flag)
            {
                list.Add(item);
            }
        }

        // [MenuItem("ThunderFireUXTool/Localization/Refresh TextLocalization Table using current Prefab")]
        public static void SyncTextTable()
        {
            CreateRuntimeTable();

            var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                SyncTextTable(prefabStage.prefabContentsRoot);
            }
            // else
            // {
            //     SyncTextTable(SceneManager.GetActiveScene());
            // }
        }
        // private static void SyncTextTable(Scene scene)
        // {
        //     List<string[]> list = RemovePath(scene.path);
        //     GameObject[] objs = scene.GetRootGameObjects();
        //     foreach(GameObject obj in objs)
        //     {
        //         SyncTextTable(scene.path, obj, list);
        //     }
        //     WriteAll(list);
        // }
        private static void SyncTextTable(GameObject gameObject)
        {
            if(!File.Exists(UXGUIConfig.RuntimeTablePath)) return;
            hasChanged = false;
            var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
            List<string[]> list = RemovePath(prefabStage.GetAssetPath());
            SyncTextTable(prefabStage.GetAssetPath(), prefabStage.prefabContentsRoot, list);
            if(hasChanged)
            {
                WriteAll(list);
            }
        }
        private static void SyncTextTable(string filePath, GameObject root, List<string[]> list)
        {
            ILocalizationText[] uxtexts = root.GetComponentsInChildren<ILocalizationText>(true);
            foreach (var uxt in uxtexts)
            {
                if (!uxt.ignoreLocalization && uxt.localizationType == LocalizationHelper.TextLocalizationType.RuntimeUse && uxt.localizationID != "")
                {
                    hasChanged = true;
                    string[] item = new string[LocalizationLanguage.Length + 3];
                    for(int i = 0; i < item.Length; i++)
                    {
                        item[i] = "";
                    }
                    item[0] = uxt.localizationID;
                    item[2] = uxt.text;
                    Transform trans = uxt.transform;
                    while (trans != root.transform)
                    {
                        item[1] = "/" + trans.name + item[1];
                        trans = trans.parent;
                    }
                    item[1] = filePath + "/" + root.name + item[1];
                    AddItem(item, list);
                }
            }
        }
        [MenuItem("ThunderFireUXTool/本地化 (Localization)/刷新静态文本表格 (Refresh Runtime-Use Text Table)")]
        private static void SyncAllTextTable()
        {
            CreateRuntimeTable();

            List<string[]> list = new List<string[]>();
            List<string[]> lines = ReadAll();
            foreach(string[] line in lines)
            {
                bool flag = false;
                for(int i = 3; i < line.Length; i++)
                {
                    flag |= line[i] != "";
                }
                if(flag)
                {
                    line[1] = line[2] = "";
                    list.Add(line);
                }
            }

            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            foreach (var guid in guids)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
                SyncTextTable(filePath, obj, list);
            }

            // string scenePath = SceneManager.GetActiveScene().path;
            // EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            // guids = AssetDatabase.FindAssets("t:Scene");
            // foreach (var guid in guids)
            // {
            //     string filePath = AssetDatabase.GUIDToAssetPath(guid);
            //     Scene scene = EditorSceneManager.OpenScene(filePath);
            //     GameObject[] objs = scene.GetRootGameObjects();
            //     foreach(GameObject obj in objs)
            //     {
            //         SyncTextTable(filePath, obj, list);
            //     }
            // }
            // EditorSceneManager.OpenScene(scenePath);
            
            WriteAll(list);
        }
    }
}