#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

namespace ThunderFireUITool
{

    public static class SceneViewContextMenu
    {
        static bool legal;//右键合法性
        static bool RightMouseDown;//右键按下事件

        public delegate void AddContextMenuFunc();
        public static AddContextMenuFunc addContextMenuFunc;
        [InitializeOnLoadMethod]
        static void Init()
        {
            legal = false;
            RightMouseDown = false;
            SceneView.duringSceneGui += OnSceneGUI;
        }
        private class DuplicateKeyComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return -string.Compare(x, y);
            }

        }
        static void OnSceneGUI(SceneView sceneView)
        {
            //不是右键不处理
            if (Event.current == null || Event.current.button != 1)
            {
                return;
            }

            //如果点击在辅助线上就不处理
            if (LocationLineLogic.HasInstance && LocationLineLogic.Instance.CheckPosInLines(Event.current.mousePosition, out LocationLine line))
            {
                return;
            }

            if (Event.current.type == EventType.MouseDown)
            {
                legal = true;
                RightMouseDown = true;
            }

            if (Event.current.type == EventType.MouseDrag && RightMouseDown)
            {
                legal = false;
            }

            bool is_handled = false;
            if (Event.current.type == EventType.MouseUp && RightMouseDown && legal)
            {
                if (SwitchSetting.CheckValid(SwitchSetting.SwitchType.RightClickList))
                {
                    var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();


                    List<RectTransform> inSceneObjs = new List<RectTransform>();
                    if (prefabStage != null)
                    {

                        RectTransform[] allObjects = prefabStage.prefabContentsRoot.GetComponentsInChildren<RectTransform>();
                        foreach (RectTransform obj in allObjects)
                        {
                            if (EditorLogic.ObjectFit(obj.gameObject))
                            {
                                inSceneObjs.Add(obj);
                            }
                        }
                    }
                    else
                    {
                        Scene scene = SceneManager.GetActiveScene();
                        GameObject[] allObjects = scene.GetRootGameObjects();
                        foreach (GameObject obj in allObjects)
                        {
                            RectTransform[] child = obj.GetComponentsInChildren<RectTransform>();
                            foreach (RectTransform rect in child)
                            {
                                if (EditorLogic.ObjectFit(rect.gameObject))
                                {
                                    inSceneObjs.Add(rect);
                                }
                            }
                        }
                    }

                    Camera camera = SceneView.currentDrawingSceneView.camera; //获取到编辑器模式下的相机，这个相机是看不到的，但是可以拿到
                    Vector3 pos = Event.current.mousePosition; //低版本可能要×2
                    pos = new Vector3(pos.x, camera.pixelHeight - pos.y);
                    //Vector2 pos = Event.current.mousePosition;
                    //Debug.Log(pos);

                    //排序
                    List<string> liststr = new List<string>();
                    foreach (var rect in inSceneObjs)
                    {
                        string str = GetString("", rect);
                        liststr.Add(str);
                    }
                    SortedList<string, RectTransform> stlist = new SortedList<string, RectTransform>(new DuplicateKeyComparer());
                    for (int i = 0; i < inSceneObjs.Count; i++)
                    {
                        stlist.Add(liststr[i], inSceneObjs[i]);
                    }

                    foreach (string d in stlist.Keys)
                    {
                        RectTransform obj = stlist[d];
                        if (RectTransformUtility.RectangleContainsScreenPoint(obj, pos, camera))
                        {

                            ContextMenu.AddItem(obj.name, false, () =>
                            {
                                Selection.activeGameObject = obj.gameObject;
                            });
                        }
                    }
                    if (!ContextMenu.IsEmpty())
                    {
                        is_handled = true;
                    }

                    if (Selection.gameObjects != null && Selection.gameObjects.Length != 0 && Selection.gameObjects[0].transform is RectTransform)
                    {
                        if (is_handled)
                        {
                            ContextMenu.AddSeparator("");
                        }
                        if (SwitchSetting.CheckValid(SwitchSetting.SwitchType.RightClickList))
                        {
                            is_handled = true;
                        }

                        ContextMenu.AddCommonItems(Selection.gameObjects);

                        ContextMenu.AddSeparator("");

                        if (CombineWidgetLogic.CanCombine(Selection.gameObjects))
                        {
                            ContextMenu.AddItem(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组合), false, () =>
                            {
                                CombineWidgetLogic.GenCombineRootRect(Selection.gameObjects);
                            });
                        }
                        else
                        {
                            ContextMenu.AddDisabledItem(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组合));
                        }

                        addContextMenuFunc?.Invoke();
                    }

                    ContextMenu.Show();
                    legal = false;
                    RightMouseDown = false;

                }

                if (is_handled)
                {
                    Event.current.Use();
                }
            }
        }
        static string GetString(string oldstr, Transform trans)
        {
            string str;
            if (oldstr == "") str = trans.GetSiblingIndex().ToString();
            else str = trans.GetSiblingIndex().ToString() + "." + oldstr;
            if (trans.parent != null)
            {
                return GetString(str, trans.parent);
            }
            return str;
        }
    }
}
#endif