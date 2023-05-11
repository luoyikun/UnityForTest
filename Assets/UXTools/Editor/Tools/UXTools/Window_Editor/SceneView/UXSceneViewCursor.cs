#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Reflection;
using System;

namespace ThunderFireUITool
{
    //统一控制UXTools中鼠标样式

    public enum UXCursorType
    {
        None,
        Crosshair,
        Updown,
        Leftright
    }

    public class UXSceneViewCursor : UXSingleton<UXSceneViewCursor>
    {
        Dictionary<UXCursorType, Texture2D> CursorTexturesDic;
        Texture2D CurCursorTexture;

        static Vector2 mouseDownPos;
        static bool Drag = false;
        static GameObject[] selection = null;
        static string quickCreateType = null;

        public override void Init()
        {
            UXCustomSceneView.AddDelegate(OnSceneGUI);
            InitCursorTexture();
        }

        private void InitCursorTexture()
        {
            //Todo load from assets
            CursorTexturesDic = new Dictionary<UXCursorType, Texture2D>();
            CursorTexturesDic.Add(UXCursorType.Crosshair, ToolUtils.GetIcon("PointerCrosshair") as Texture2D);
            CursorTexturesDic.Add(UXCursorType.Updown, ToolUtils.GetIcon("SplitResizeUpDown") as Texture2D);
            CursorTexturesDic.Add(UXCursorType.Leftright, ToolUtils.GetIcon("SplitResizeLeftRight") as Texture2D);
        }

        private Texture2D GetCursorTexture(UXCursorType cursorType)
        {
            if (CursorTexturesDic != null && CursorTexturesDic.ContainsKey(cursorType))
            {
                return CursorTexturesDic[cursorType];
            }
            else
            {
                return null;
            }
        }

        public void SetCursor(UXCursorType cursorType)
        {
            if (cursorType == UXCursorType.None)
            {

                CurCursorTexture = null;
                Cursor.SetCursor(null, new Vector2(16, 16), CursorMode.Auto);
            }
            else
            {
                CurCursorTexture = GetCursorTexture(cursorType);
            }

        }

        void OnSceneGUI(SceneView sceneView)
        {
            if (CurCursorTexture != null)
            {
                Utils.SetCursor(CurCursorTexture);
                //Cursor.SetCursor(CurCursorTexture, new Vector2(16, 16), CursorMode.Auto);
                HandleUtility.AddDefaultControl(0);
            }
        }

        public void StartQuickCreate(string type)
        {
            quickCreateType = type;
            UXCustomSceneView.RemoveDelegate(OnQuickCreate);
            EditorApplication.delayCall += () =>
             {
                 UXCustomSceneView.AddDelegate(OnQuickCreate);
             };

            SetCursor(UXCursorType.Crosshair);
        }

        public void StopQuickCreate()
        {
            UXCustomSceneView.RemoveDelegate(OnQuickCreate);
            SetCursor(UXCursorType.None);
        }

        public void OnQuickCreate(SceneView sceneView)
        {
            //Cursor.visible = OutBounds(sceneView);
            //Handles.BeginGUI();
            //GUI.DrawTexture(new Rect(Event.current.mousePosition.x - CurCursorTexture.width / 4, Event.current.mousePosition.y - CurCursorTexture.height / 4, CurCursorTexture.width / 2, CurCursorTexture.height / 2), CurCursorTexture);
            //Handles.EndGUI();


            //点下鼠标开始拖动
            if (Event.current.type == EventType.MouseDown && !SceneViewToolBar.DrawState(quickCreateType))
            {
                StartDarg(sceneView);
            }

            if (Drag)
            {
                Handles.BeginGUI();
                GUI.Box(new Rect(mouseDownPos.x, mouseDownPos.y, Event.current.mousePosition.x - mouseDownPos.x, Event.current.mousePosition.y - mouseDownPos.y), "");
                Handles.EndGUI();
                //抬起鼠标或光标离开可视范围即完成框选
                if (Event.current.type == EventType.MouseUp || OutBounds(sceneView))
                {
                    EndDarg(sceneView);
                }
            }
            HandleUtility.AddDefaultControl(0);
            sceneView.Repaint();
        }

        public bool OutBounds(SceneView sceneView)
        {
            if ((Event.current.mousePosition.y > sceneView.camera.pixelHeight || Event.current.mousePosition.y < 0 || Event.current.mousePosition.x > sceneView.camera.pixelWidth || Event.current.mousePosition.x < 0) || SceneViewToolBar.OverRange())
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void StartDarg(SceneView sceneView)
        {
            mouseDownPos = Event.current.mousePosition;
            selection = Selection.gameObjects;
            Selection.objects = new UnityEngine.Object[0];
            Drag = true;
        }

        public void EndDarg(SceneView sceneView)
        {
            Cursor.visible = true;
            Drag = false;
            //找出视图上方ribbon的间距，解决y值的offset，可能以后有用
            //var style = (GUIStyle)"GV Gizmo DropDown";
            //Vector2 ribbon = style.CalcSize(sceneView.titleContent);
            //ribbon.y equal 18
            //把屏幕坐标转为世界坐标
            Transform parent = FindContainerLogic.GetObjectParent(selection);
            Vector2 startScreenPos = new Vector2(mouseDownPos.x, sceneView.camera.pixelHeight - mouseDownPos.y);
            Vector2 endScreenPos = new Vector2(Event.current.mousePosition.x, sceneView.camera.pixelHeight - Event.current.mousePosition.y);

            Vector3 startWorldPos = sceneView.camera.ScreenToWorldPoint(startScreenPos);
            Vector3 endWorldPos = sceneView.camera.ScreenToWorldPoint(endScreenPos);

            Vector2 startPos = parent.InverseTransformPoint(startWorldPos);
            Vector2 endPos = parent.InverseTransformPoint(endWorldPos);

            //RectTransform rect = parent as RectTransform;
            //Canvas canvas = parent.GetComponentsInParent<Canvas>(true)[0];
            //Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, startScreenPos, cam, out startPos);
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, endScreenPos, cam, out endPos);


            //Vector3 startPos = sceneView.camera.ScreenToWorldPoint(new Vector3(mouseDownPos.x, sceneView.camera.pixelHeight - mouseDownPos.y, 0));
            //Vector3 endPos = sceneView.camera.ScreenToWorldPoint(new Vector3(Event.current.mousePosition.x, sceneView.camera.pixelHeight - Event.current.mousePosition.y, 0));
            Vector2 size = new Vector2(System.Math.Abs(startPos.x - endPos.x), System.Math.Abs(startPos.y - endPos.y));
            if (size.x == 0 || size.y == 0)
            {
                size.x = 100;
                size.y = 100;
            }

            Vector3 localPosition = new Vector3((startPos.x + endPos.x) / 2, (startPos.y + endPos.y) / 2, 0);

            GameObject obj = WidgetGenerator.CreateUIObj(quickCreateType, localPosition, size, selection);
            if (obj)
            {
                switch (quickCreateType)
                {
                    case "Image":
                        Assembly imageAssembly = System.Reflection.Assembly.Load(ThunderFireUIToolConfig.ImageAssemblyName);
                        Type imageType = imageAssembly.GetType(ThunderFireUIToolConfig.ImageClassName);
                        obj.AddComponent(imageType);
                        break;
                    case "Text":
                        Assembly textAssembly = System.Reflection.Assembly.Load(ThunderFireUIToolConfig.TextAssemblyName);
                        Type textType = textAssembly.GetType(ThunderFireUIToolConfig.TextClassName);
                        obj.AddComponent(textType);
                        break;
                }
            }
            //生成节点被选中
            Selection.activeObject = obj;
            QuickCreateCommand cmd = new QuickCreateCommand(obj);
            cmd.Execute();

            SceneViewToolBar.ButtonReleased();
            StopQuickCreate();
        }
    }
}
#endif


