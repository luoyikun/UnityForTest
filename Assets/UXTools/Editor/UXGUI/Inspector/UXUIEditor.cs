using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ThunderFireUITool
{
    public class UXImageEditor : Editor
    {
        [MenuItem("GameObject/UI/UXImage")]
        public static void CreateUXImage()
        {
            GenUXComponent<UXImage>();
        }


        [MenuItem("GameObject/UI/UXText")]
        public static void CreateUXText()
        {
            GenUXComponent<UXText>();
        }
        [MenuItem("GameObject/UI/UXTextMeshPro")]
        private static void CreateUXTextMeshPro()
        {
            GenUXComponent<UXTextMeshPro>();
        }

        [MenuItem("GameObject/UI/UXToggle")]
        public static void CreateUXToggle()
        {
            GenUXComponent<UXToggle>();
        }

        [MenuItem("GameObject/UI/UXScrollView")]
        public static void CreateUXScrollView(MenuCommand menuCommand)
        {
            Type MenuOptionsType = typeof(UnityEditor.UI.ImageEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            Utils.InvokeMethod(MenuOptionsType, "AddScrollView", new object[] { menuCommand });
            GameObject obj = Selection.activeGameObject;
            obj.name = "UXScrollView";
            DestroyImmediate(obj.GetComponent<ScrollRect>());
            UXScrollRect scroll = obj.AddComponent<UXScrollRect>();
            scroll.content = scroll.transform.Find("Viewport/Content").GetComponent<RectTransform>();
            scroll.viewport = scroll.transform.Find("Viewport").GetComponent<RectTransform>();
            scroll.horizontalScrollbar = scroll.transform.Find("Scrollbar Horizontal").GetComponent<Scrollbar>();
            scroll.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scroll.horizontalScrollbarSpacing = -3;
            scroll.verticalScrollbar = scroll.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>();
            scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scroll.verticalScrollbarSpacing = -3;
            scroll.content.gameObject.AddComponent<GridLayoutGroup>();
            var contentFitter = scroll.content.gameObject.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        //public方便预览那边直接调用
        public static void GenUXComponent<T>() where T : MonoBehaviour
        {
            T comp = new GameObject(typeof(T).Name).AddComponent<T>();

            //这里处理一些特殊的初始化
            if (typeof(T) == typeof(UXImage))
            {
                var mat = AssetDatabase.LoadAssetAtPath<Material>(UXGUIConfig.UIDefaultMatPath);
                object temp = comp;
                UXImage compEntity = (UXImage)temp;
                compEntity.material = mat;
            }
            if (typeof(T) == typeof(UXText))
            {
                var mat = AssetDatabase.LoadAssetAtPath<Material>(UXGUIConfig.UIDefaultMatPath);
                object temp = comp;
                UXText compEntity = (UXText)temp;
                compEntity.material = mat;
            }
            if (typeof(T) == typeof(UXTextMeshPro))
            {

            }

            if (Selection.activeTransform)
            {
                if (Selection.activeTransform.GetComponentInParent<Canvas>())
                {
                    comp.transform.SetParent(Selection.activeTransform, false);
                }
                else
                {
                    Canvas canvas = new GameObject("Canvas").AddComponent<Canvas>();
                    canvas.transform.SetParent(Selection.activeTransform);
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas.gameObject.AddComponent<CanvasScaler>();
                    canvas.gameObject.AddComponent<CanvasRenderer>();
                    comp.transform.SetParent(canvas.transform, false);
                }
            }
            else
            {
                var prefab = PrefabStageUtils.GetCurrentPrefabStage();
                if(prefab != null){
                    if(prefab.prefabContentsRoot.GetComponent<RectTransform>()!=null){
                        comp.transform.SetParent(prefab.prefabContentsRoot.transform, false);
                    }
                    else {
                        Canvas canvas1 = new GameObject("Canvas").AddComponent<Canvas>();
                    canvas1.transform.SetParent(prefab.prefabContentsRoot.transform);
                    canvas1.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas1.gameObject.AddComponent<CanvasScaler>();
                    canvas1.gameObject.AddComponent<CanvasRenderer>();
                    comp.transform.SetParent(canvas1.transform, false);
                    }
                    return;
                }
                
                var canvas = FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    canvas = new GameObject("Canvas").AddComponent<Canvas>();
                    canvas.gameObject.AddComponent<CanvasScaler>();
                    canvas.gameObject.AddComponent<CanvasRenderer>();
                }
                

                var eventSystem = FindObjectOfType<EventSystem>();
                if (eventSystem == null)
                {
                    eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
                    eventSystem.gameObject.AddComponent<StandaloneInputModule>();
                }

                comp.transform.SetParent(canvas.transform, false);
            }
            Selection.activeTransform = comp.transform;
            Undo.RegisterCreatedObjectUndo(comp.gameObject, "Create" + comp.gameObject.name);
        }
    }
}
