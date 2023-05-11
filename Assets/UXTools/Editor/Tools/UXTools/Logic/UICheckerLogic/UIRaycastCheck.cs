#if UNITY_EDITOR && ODIN_INSPECTOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

namespace ThunderFireUITool
{
    public class UIRaycastCheck
    {
        private static HashSet<Graphic> s_HasRef = new HashSet<Graphic>();

        [MenuItem("ThunderFireUXTool/射线检查(UIRaycastCheck)")]
        private static void BeginRaycastCheck()
        {
            var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
            if(prefabStage == null)
            {
                Debug.LogWarning("请打开一个Prefab后再执行此操作！");
                return;
            }

            StringBuilder builder = new StringBuilder("多余的Raycast Target: ");

            var selectableArr = prefabStage.prefabContentsRoot.GetComponentsInChildren<Selectable>();
            foreach(var selectable in selectableArr)
            {
                if(selectable.targetGraphic != null)
                {
                    s_HasRef.Add(selectable.targetGraphic);
                }
            }

            var raycastArr = prefabStage.prefabContentsRoot.GetComponentsInChildren<ICanvasRaycastFilter>();
            foreach(var raycast in raycastArr)
            {
                var graphic = raycast as Graphic;
                if(graphic == null || !graphic.raycastTarget || s_HasRef.Contains(graphic))
                    continue;
                builder.Append(graphic.name);
                builder.Append("  ");
            }

            Debug.Log(builder);

            s_HasRef.Clear();
        }
    }
}
#endif