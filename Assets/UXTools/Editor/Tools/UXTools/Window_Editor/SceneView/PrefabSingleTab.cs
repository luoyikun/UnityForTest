#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine;

namespace ThunderFireUITool
{
    public class PrefabSingleTab : VisualElement
    {
        private static int m_maxCharacters = 26;
        public Button visual;
        private string m_guid;

        public PrefabSingleTab(FileInfo info, string guid)
        {
            VisualTreeAsset tabTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "PrefabSingleTab.uxml");
            visual = tabTreeAsset.CloneTree().Q<Button>("Tab");
            Label label = visual.Q<Label>("Label");
            Button close = visual.Q<Button>("Close");

            m_guid = guid;
            string fileName = Path.GetFileNameWithoutExtension(info.Name);
            label.text = SetTextWithEllipsis(fileName);

            close.clickable.activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            close.clicked += OnClose;
            close.RegisterCallback<MouseEnterEvent, VisualElement>(OnHoverClose, close);
            close.RegisterCallback<MouseLeaveEvent, VisualElement>(OnHoverClose, close);
            visual.RegisterCallback<MouseEnterEvent>(OnHoverChange);
            visual.RegisterCallback<MouseLeaveEvent>(OnHoverChange);
            visual.clickable.activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            visual.clicked += OnClick;
        }

        private void OnHoverChange(EventBase e)
        {
            if(m_guid == PrefabTabs.SelectedGuid) return;
            if(e.eventTypeId == MouseEnterEvent.TypeId())
            {
                visual.style.backgroundColor = new Color(78f / 255, 78f / 255, 78f / 255, 1);
            }
            else if(e.eventTypeId == MouseLeaveEvent.TypeId())
            {
                visual.style.backgroundColor = new Color(60f / 255, 60f / 255, 60f / 255, 1);
            }
        }
        private void OnHoverClose(EventBase e, VisualElement close)
        {
            if(e.eventTypeId == MouseEnterEvent.TypeId())
            {
                close.style.backgroundColor = new Color(60f / 255, 60f / 255, 60f / 255, 1);
            }
            else if(e.eventTypeId == MouseLeaveEvent.TypeId())
            {
                close.style.backgroundColor = new Color(60f / 255, 60f / 255, 60f / 255, 0);
            }
        }

        private void OnClick()
        {
            PrefabTabs.OpenTab(m_guid, true);
        }
        private void OnClose()
        {
            PrefabTabs.CloseTab(m_guid, true);
        }

        private string SetTextWithEllipsis(string name)
        {
            if(name.Length <= m_maxCharacters)
            {
                return name;
            }
            else
            {
                string ans = name.Substring(0, m_maxCharacters - 3);
                ans += "...";
                return ans;
            }
        }
    }
}
#endif