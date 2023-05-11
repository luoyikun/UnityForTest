using ThunderFireUITool;
using UnityEditor;

namespace UnityEngine.UI
{
    public static class UIEffectWrapDrawer
    {
        static string LabelStr;
        static string ShadowStr;
        static string OutlineStr;

        public static void InitInspectorString()
        {
            LabelStr = EditorLocalization.GetLocalization("UXImage", "UXEffect");
            ShadowStr = EditorLocalization.GetLocalization("UXImage", "UXShadow");
            OutlineStr = EditorLocalization.GetLocalization("UXImage", "UXOutline");
        }


        public static void Draw(Rect position, GameObject target)
        {
            var nameRect = new Rect(position)
            {
                width = position.width / 2
            };

            EditorGUI.LabelField(nameRect, LabelStr);

            var shadowRect = new Rect(position)
            {
                x = position.x + EditorGUIUtility.labelWidth + 2,
                width = 60
            };

            if (GUI.Button(shadowRect, ShadowStr))
            {
                GenShadowComponent(target);
            }

            var outlineRect = new Rect(position)
            {
                x = shadowRect.x + 50 + 20,
                width = 60
            };

            if (GUI.Button(outlineRect, OutlineStr))
            {
                GenOutLineComponent(target);
            }
        }

        private static void GenOutLineComponent(GameObject target)
        {
            target.TryAddComponent<Outline>();
        }

        private static void GenShadowComponent(GameObject target)
        {
            //暂时无法处理 有继承关系的Component 单独判定区分outline
            //target.TryAddComponent<Shadow>();
            // Shadow[] components = target.GetComponents<Shadow>();

            // bool hasShadow = false;
            // for (int i = 0; i < components.Length; i++)
            // {
            //     Outline outline = components[i] as Outline;
            //     //有一个不是OutLine就认为是Shadow
            //     if (outline == null)
            //     {
            //         hasShadow = true;
            //     }
            // }

            //if (!hasShadow)
            //{
                target.AddComponent<Shadow>();
            //}
        }

        private static T TryAddComponent<T>(this GameObject target) where T : Component
        {
            //暂时无法处理 有继承关系的Component 挠头
            target.TryGetComponent<T>(out T component);
            //if (component == null)
            //{
                component = target.AddComponent<T>();
            //}

            return component;
        }
    }
}

