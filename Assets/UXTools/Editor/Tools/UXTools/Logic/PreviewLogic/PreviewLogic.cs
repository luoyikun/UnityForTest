#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThunderFireUITool
{
    [InitializeOnLoad]
    [System.Serializable]
    public class PreviewLogic
    {
        static PreviewLogic()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        public static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                ExitPreview();
            }
        }

        /// <summary>
        /// 打开PreviewScene, 将当前正在编辑的prefab放到UXPreviewCanvas下
        /// 切换到playmode进行预览
        /// </summary>
        public static void Preview()
        {

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                //预览是预览一个Prefab的动画等，因此必须要在PrefabMode中才能用
                EditorUtility.DisplayDialog("messageBox",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_请打开Prefab后再进行预览),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                SceneViewToolBar.ButtonReleased();
                return;
            }

            string prefabPath = prefabStage.GetAssetPath();

            var tempPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            //缓存当前Prefab路径,退出预览时重新打开
            EditorPrefs.SetString(ThunderFireUIToolConfig.PreviewPrefabPath, prefabPath);

            GameObject previewCanvas = GameObject.Find("UXPreviewCanvas");
            if (previewCanvas == null)
            {
                //缓存当前Scene路径,退出预览时重新打开
                Scene OriginScene = SceneManager.GetActiveScene();
                string OriginScenePath = OriginScene.path;
                EditorPrefs.SetString(ThunderFireUIToolConfig.PreviewOriginScene, OriginScenePath);

                if (EditorUtility.DisplayDialog("Save",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_是否想要保存场景),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消)))
                {
                    EditorSceneManager.SaveScene(OriginScene);
                }

                EditorSceneManager.OpenScene(ThunderFireUIToolConfig.ScenePath + "PreviewScene.unity", OpenSceneMode.Single);
                previewCanvas = GameObject.Find("UXPreviewCanvas");
                InitPreviewCanvas(tempPrefab, previewCanvas);
            }
            else
            {
                RefreshPreviewCanvas(tempPrefab, previewCanvas);
            }
            Utils.ExitPrefabStage();
            Utils.EnterPlayMode();
        }
        public static void PreviewGuide(GameObject gameobj, string id)
        {

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                int iid = gameobj.GetInstanceID();
                EditorPrefs.SetString("gameobjiid", iid.ToString());
                
                GameObject tempobj = new GameObject("TempGuideObject");
                EditorPrefs.SetString("tempobjname", "TempGuideObject");
                var guideDataList = gameobj.GetComponent<UIBeginnerGuideDataList>();
                var so = new SerializedObject(guideDataList);
                SerializedProperty previewSp = so.FindProperty("PreviewMode");
                previewSp.boolValue = true;
                so.ApplyModifiedProperties();

                // Undo.RecordObject(gameobj, "modify test value");
                // gameobj.GetComponent<UIBeginnerGuideDataList>().PreviewMode = true;
                // EditorUtility.SetDirty(gameobj);
                var manager = Object.FindObjectOfType<UIBeginnerGuideManager>();
                if (manager != null)
                {
                    manager.SetGuideID(id);
                    tempobj.AddComponent<PreviewGuideMono>();
                    EditorPrefs.SetString("tempmanager", manager.name);
                    Utils.ExitPrefabStage();
                    Utils.EnterPlayMode();
                    return;
                }
                
                tempobj.AddComponent<UIBeginnerGuideManager>();
                tempobj.GetComponent<UIBeginnerGuideManager>().SetGuideID(id);
                tempobj.AddComponent<PreviewGuideMono>();
                Utils.ExitPrefabStage();
                Utils.EnterPlayMode();

                return;
            }
            string prefabPath = prefabStage.GetAssetPath();

            var tempPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            //缓存当前Prefab路径,退出预览时重新打开
            EditorPrefs.SetString(ThunderFireUIToolConfig.PreviewPrefabPath, prefabPath);

            GameObject previewCanvas = GameObject.Find("UXPreviewCanvas");
            if (previewCanvas == null)
            {
                //缓存当前Scene路径,退出预览时重新打开
                Scene OriginScene = SceneManager.GetActiveScene();
                string OriginScenePath = OriginScene.path;
                EditorPrefs.SetString(ThunderFireUIToolConfig.PreviewOriginScene, OriginScenePath);

                if (EditorUtility.DisplayDialog("Save",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_是否想要保存场景),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消)))
                {
                    EditorSceneManager.SaveScene(OriginScene);
                }

                EditorSceneManager.OpenScene(ThunderFireUIToolConfig.ScenePath + "PreviewScene.unity", OpenSceneMode.Single);
                previewCanvas = GameObject.Find("UXPreviewCanvas");
                tempPrefab = InitPreviewCanvas(tempPrefab, previewCanvas);
            }
            else
            {
                tempPrefab = RefreshPreviewCanvas(tempPrefab, previewCanvas);
            }
            tempPrefab.GetComponentInChildren<UIBeginnerGuideDataList>().PreviewMode = true;
            EditorUtility.SetDirty(tempPrefab);
            GameObject obj = new GameObject("TempGuideObject");
            obj.AddComponent<UIBeginnerGuideManager>();
            obj.GetComponent<UIBeginnerGuideManager>().SetGuideID(id);
            obj.AddComponent<PreviewGuideMono>();
            EditorPrefs.SetString("tempobjname", "TempGuideObject");
            Utils.ExitPrefabStage();
            Utils.EnterPlayMode();

        }
        public static GameObject InitPreviewCanvas(GameObject prefab, GameObject previewCanvas)
        {
            return Object.Instantiate(prefab, previewCanvas.transform);
        }
        public static GameObject RefreshPreviewCanvas(GameObject prefab, GameObject previewCanvas)
        {
            ClearPreviewCanvas(previewCanvas);
            return InitPreviewCanvas(prefab, previewCanvas);
        }

        public static void ClearPreviewCanvas(GameObject previewCanvas)
        {
            int count = previewCanvas.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                GameObject.DestroyImmediate(previewCanvas.transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 退出预览窗口并且重新打开之前的场景和prefab
        /// </summary>
        public static void ExitPreview()
        {
            Utils.StopPlayMode();
            string prefabPath = EditorPrefs.GetString(ThunderFireUIToolConfig.PreviewPrefabPath);
            string OriginScenePath = EditorPrefs.GetString(ThunderFireUIToolConfig.PreviewOriginScene);
            string tempobjname = EditorPrefs.GetString("tempobjname");
            string managername = EditorPrefs.GetString("tempmanager");
            string iid = EditorPrefs.GetString("gameobjiid");
            if (!string.IsNullOrEmpty(iid))
            {
                GameObject obj = (GameObject)EditorUtility.InstanceIDToObject(System.Int32.Parse(iid));
                obj.GetComponent<UIBeginnerGuideDataList>().PreviewMode = false;
                EditorPrefs.DeleteKey("gameobjiid");
            }
            if (!string.IsNullOrEmpty(managername))
            {
                var manager = Object.FindObjectOfType<UIBeginnerGuideManager>();
                manager.SetGuideID("");
                EditorPrefs.DeleteKey("tempmanager");
            }
            if (!string.IsNullOrEmpty(tempobjname))
            {
                Object.DestroyImmediate(GameObject.Find(tempobjname));
                EditorPrefs.DeleteKey("tempobjname");
            }
            if (!string.IsNullOrEmpty(OriginScenePath))
            {
                EditorSceneManager.OpenScene(OriginScenePath, OpenSceneMode.Single);
                Utils.OpenPrefab(prefabPath);
            }


            EditorPrefs.DeleteKey(ThunderFireUIToolConfig.PreviewPrefabPath);
            EditorPrefs.DeleteKey(ThunderFireUIToolConfig.PreviewOriginScene);
            SceneViewToolBar.TryOpenToolbar();
        }
    }
}
#endif