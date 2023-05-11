#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;
using System;

using Object = UnityEngine.Object;
using System.Linq;

namespace ThunderFireUITool
{
    public static class ToolUtils
    {
        #region Prefab
        public static GameObject CreatePrefab(GameObject go, string name, string path, bool isPrefab = false, string label = null)
        {
            string assetpath = $"{path}{name}.prefab";
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetpath);
            if (asset != null)
            {
                if (isPrefab)
                {
                    if (label != null)
                    {
                        AssetDatabase.SetLabels(asset, new string[] { label });
                    }
                    AddGUID(assetpath);
                    return asset;
                }
                if (EditorUtility.DisplayDialog(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_警告), EditorLocalization.GetLocalization(EditorLocalizationStorage.是否覆盖), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_覆盖), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消)))
                {
                    //覆盖
                    asset = PrefabUtility.SaveAsPrefabAssetAndConnect(go, assetpath, InteractionMode.UserAction);
                    if (label != null)
                    {
                        AssetDatabase.SetLabels(asset, new string[] { label });
                    }

                    AddGUID(assetpath);
                    AssetDatabase.Refresh();
                    return asset;
                }
                else
                {
                    //取消
                    //Object.DestroyImmediate(go);
                    return null;
                }
            }

            asset = PrefabUtility.SaveAsPrefabAssetAndConnect(go, assetpath, InteractionMode.UserAction);
            if (label != null)
            {
                AssetDatabase.SetLabels(asset, new string[] { label });
            }
            AddGUID(assetpath);
            AssetDatabase.Refresh();
            return asset;
        }

        public static GameObject CreatePrefabWithPack(GameObject go, string name, string path, bool isPrefab = false, string label = null, bool isPack = false)
        {
            string assetpath = $"{path}{name}.prefab";
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetpath);
            if (asset != null)
            {
                if (isPrefab)
                {
                    if (isPack)
                    {
                        if (label != null)
                        {
                            AssetDatabase.SetLabels(asset, new string[] { "Pack", label });
                        }
                        else
                        {
                            AssetDatabase.SetLabels(asset, new string[] { "Pack" });
                        }
                    }
                    else
                    {
                        if (label != null)
                        {
                            AssetDatabase.SetLabels(asset, new string[] { "UnPack", label });
                        }
                        else
                        {
                            AssetDatabase.SetLabels(asset, new string[] { "UnPack" });
                        }
                    }
                    AddGUID(assetpath);
                    return asset;
                }
                if (EditorUtility.DisplayDialog(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_警告), EditorLocalization.GetLocalization(EditorLocalizationStorage.是否覆盖), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_覆盖), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消)))
                {
                    //覆盖
                    asset = PrefabUtility.SaveAsPrefabAssetAndConnect(go, assetpath, InteractionMode.UserAction);
                    if (isPack)
                    {
                        if (label != null)
                        {
                            AssetDatabase.SetLabels(asset, new string[] { "Pack", label });
                        }
                        else
                        {
                            AssetDatabase.SetLabels(asset, new string[] { "Pack" });
                        }
                    }
                    else
                    {
                        if (label != null)
                        {
                            AssetDatabase.SetLabels(asset, new string[] { "UnPack", label });
                        }
                        else
                        {
                            AssetDatabase.SetLabels(asset, new string[] { "UnPack" });
                        }
                    }

                    AddGUID(assetpath);
                    AssetDatabase.Refresh();
                    return asset;
                }
                else
                {
                    //取消
                    //Object.DestroyImmediate(go);
                    return null;
                }
            }

            asset = PrefabUtility.SaveAsPrefabAssetAndConnect(go, assetpath, InteractionMode.UserAction);
            if (isPack)
            {
                if (label != null)
                {
                    AssetDatabase.SetLabels(asset, new string[] { "Pack", label });
                }
                else
                {
                    AssetDatabase.SetLabels(asset, new string[] { "Pack" });
                }
            }
            else
            {
                if (label != null)
                {
                    AssetDatabase.SetLabels(asset, new string[] { "UnPack", label });
                }
                else
                {
                    AssetDatabase.SetLabels(asset, new string[] { "UnPack" });
                }
            }
            AddGUID(assetpath);
            AssetDatabase.Refresh();
            return asset;
        }

        public static void AddGUID(string path)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            var PrefabSettled = SettingAssetsUtils.GetAssets<PrefabSettledSetting>();
            var PrefabSettledList = PrefabSettled.List;
            if (!PrefabSettledList.Contains(guid))
            {
                PrefabSettled.Add(guid);
            }
            else
            {
                PrefabSettled.ResortLast(guid);
            }
        }
        #endregion

        #region Texutre&Icon
        //读取和缓存Icon图片
        static Dictionary<string, Texture> m_IconDict = new Dictionary<string, Texture>();
        public static Texture GetIcon(string name)
        {
            if (!m_IconDict.TryGetValue(name, out var tex))
            {
                tex = AssetDatabase.LoadAssetAtPath<Texture>($"{ThunderFireUIToolConfig.IconPath}{name}.png");
                m_IconDict[name] = tex;
            }
            return tex;
        }
        #endregion
    }
}
#endif