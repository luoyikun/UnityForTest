using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace ThunderFireUITool
{
    public class SwitchSetting : ScriptableObject
    {
        [SerializeField]
        private bool[] m_values;
        public enum SwitchType
        {
            RecentlyOpened,
            AlignSnap,
            RightClickList,
            QuickCopy,
            MovementShortcuts,
            PrefabMultiOpen,
            ResolutionAdjustment,
            PrefabResourceCheck,
            AutoConvertTex
        }
        private static SwitchSetting m_instance;

        public static void ChangeSwitch(Toggle[] toggles)
        {
            m_instance = ScriptableObject.CreateInstance<SwitchSetting>();
            m_instance.m_values = new bool[toggles.Length];
            for (int i = 0; i < toggles.Length; i++)
            {
                m_instance.m_values[i] = toggles[i].value;
            }
            AssetDatabase.CreateAsset(m_instance, ThunderFireUIToolConfig.SwitchSettingPath);
            SceneViewToolBar.CloseFunction();
            SceneViewToolBar.InitFunction();
#if ODIN_INSPECTOR
            var atlasData = AssetDatabase.LoadAssetAtPath<UIAtlasCheckUserData>(ThunderFireUIToolConfig.UICheckUserDataFullPath);
            atlasData.Save(CheckValid(SwitchType.PrefabResourceCheck));
#endif
        }

        public static bool CheckValid(int x)
        {
            if (m_instance == null)
            {
                m_instance = AssetDatabase.LoadAssetAtPath<SwitchSetting>(ThunderFireUIToolConfig.SwitchSettingPath);
            }
            if (m_instance == null || m_instance.m_values == null || m_instance.m_values.Length <= x)
            {
                return true;
            }
            return m_instance.m_values[x];
        }
        public static bool CheckValid(SwitchType type)
        {
            return CheckValid((int)type);
        }
    }
}
