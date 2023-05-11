#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    [Serializable]
    public class LocationLineData
    {
        public int Id;
        public bool Horizontal;
        public float Pos;
    }

    [Serializable]
    public class LocationLinesData : ScriptableObject
    {
        public List<LocationLineData> List = new List<LocationLineData>();

        public int LastLineId
        {
            get { return List.Count > 0 ? List.Max(data => data.Id) : 0; }
        }

        public void Add(LocationLineData line)
        {
            List.Add(line);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            OnValueChanged();
        }

        public void Remove(int id)
        {
            var index = List.FindIndex(l => id == l.Id);
            if (index >= 0)
            {
                List.RemoveAt(index);
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            OnValueChanged();
        }

        public void Modify(LocationLineData line)
        {
            var index = List.FindIndex(l => line.Id == l.Id);
            if (index >= 0)
            {
                List[index].Horizontal = line.Horizontal;
                List[index].Pos = line.Pos;
            }
        }

        public void Clear()
        {
            List.Clear();
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static LocationLinesData Create()
        {
            var data = CreateInstance<LocationLinesData>();
            if (data == null)
                Debug.LogError("Create LocationLinesData Failed!");

            if (!Directory.Exists(ThunderFireUIToolConfig.UserDataPath))
                Directory.CreateDirectory(ThunderFireUIToolConfig.UserDataPath);

            var assetPath = ThunderFireUIToolConfig.LocationLinesDataPath;
            AssetDatabase.CreateAsset(data, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return data;
        }

        private void OnValueChanged()
        {
        }
    }

    public class CreateLocationLinesData : Editor
    {
        [MenuItem("ThunderFireUXTool/新建配置文件 (Create Assets)/LocationLinesData")]
        public static void Create()
        {
            var data = CreateInstance<LocationLinesData>();
            if (data == null)
                Debug.LogError("Create LocationLinesData Failed!");

            if (!Directory.Exists(ThunderFireUIToolConfig.UserDataPath))
                Directory.CreateDirectory(ThunderFireUIToolConfig.UserDataPath);

            var assetPath = ThunderFireUIToolConfig.LocationLinesDataPath;
            AssetDatabase.CreateAsset(data, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif