#if UNITY_EDITOR && ODIN_INSPECTOR
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ThunderFireUITool
{
    public class PrefabYamlUtils : ScriptableObject
    {
        public static void LoadFromString(string content, ref List<string> assignedInstanceIdList)
        {
            List<string> ignoreKeys = new List<string>()
            {
                "m_Script",
                "m_GameObject",
                "m_PrefabAsset",
                "m_Father",
                "m_Children",
                "m_CorrespondingSourceObject",
                "m_PrefabInstance",
                "m_Font",
                "m_Sprite",
                "m_Material"
            };
            string currentKey = "";
            int currentDepth = 0;

            string[] lines = content.Split('\n');

            int[] depths = new int[lines.Length];
            for (int i = 0; i < lines.Length; ++i)
            {
                string temp = lines[i];
                byte spaceCount = 0;
                for (int j = 0; j < temp.Length; ++j)
                {
                    if (temp[j] == ' ')
                    {
                        spaceCount++;
                    }
                    else break;
                }
                depths[i] = spaceCount;
                lines[i] = lines[i].Substring(spaceCount, lines[i].Length - spaceCount);
            }

            for (int i = 0; i < lines.Length; ++i)
            {
                string temp = lines[i];
                currentDepth = depths[i];

                if (temp.Length == 0) continue;

                string key = temp;
                string val = null;
                int idx = key.IndexOf(':');
                if (idx != -1)
                {
                    val = key.Substring(idx + 1);
                    key = key.Substring(0, idx);
                    if (!key.StartsWith("-"))
                    {
                        // - 开头的是列表中的元素 没有自己的Key
                        currentKey = key;
                    }
                }

                if (ignoreKeys.Contains(currentKey)) continue;

                if (val != null && (val.Contains("instanceID") || key.Contains("instanceID")))
                {
                    string insId = Regex.Replace(val, @"[^0-9]+", "");
                    if (insId != "" && insId != "0")
                    {
                        assignedInstanceIdList.Add(insId);
                    }
                }
            }
        }
    }
}
#endif