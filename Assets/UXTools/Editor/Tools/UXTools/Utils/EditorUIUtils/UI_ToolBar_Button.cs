#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UITButton
    {
        private VisualElement container;
        private Action clickAction;
        private Action nextClickAction;

        public UITButton(VisualElement c, Action ca, Action nca, string tooltip = "")
        {
            container = c;
            container.tooltip = tooltip;
            clickAction = ca;
            nextClickAction = nca;

            container.RegisterCallback<MouseEnterEvent>(MouseEnter);

            if (clickAction != null)
            {
                container.RegisterCallback((MouseDownEvent e) =>
                {

                    if (e.button == 0)
                    {

                        if (SceneViewToolBar.getButtonBeOccupied() == this)
                        {
                            UnSelected();
                            SceneViewToolBar.ButtonReleased();
                            container.UnregisterCallback<MouseEnterEvent>(MouseEnter);
                            container.RegisterCallback<MouseLeaveEvent>(MouseOut);
                        }
                        else
                        {
                            SceneViewToolBar.ButtonReleased();
                            SceneViewToolBar.setButtonBeOccupied(this);
                            BeSelected();
                            Click();
                            container.UnregisterCallback<MouseLeaveEvent>(MouseOut);
                            container.RegisterCallback<MouseEnterEvent>(MouseEnter);

                        }

                    }

                });

            }

        }

        public void Click()
        {
            if (clickAction != null)
            {
                clickAction.Invoke();
            }
        }

        public void nextClick()
        {
            if (nextClickAction != null)
            {
                nextClickAction.Invoke();
            }

        }

        public void UnSelected()
        {
            container.style.backgroundColor = Color.white;
            container.style.unityBackgroundImageTintColor = Color.black;
            if (container.childCount > 0)
            {
                foreach (VisualElement ele in container.Children())
                {
                    ele.style.backgroundColor = Color.white;
                    ele.style.unityBackgroundImageTintColor = Color.black;
                }
            }
        }

        public void BeSelected()
        {
            container.style.backgroundColor = new Color(0.14f, 0.39f, 0.76f, 1f);
            container.style.unityBackgroundImageTintColor = Color.white;
            if (container.childCount > 0)
            {
                foreach (VisualElement ele in container.Children())
                {
                    ele.style.backgroundColor = new Color(0.14f, 0.39f, 0.76f, 1f);
                    ele.style.unityBackgroundImageTintColor = Color.white;
                }
            }
        }

        public void MouseEnter(MouseEnterEvent e)
        {
            container.UnregisterCallback<MouseEnterEvent>(MouseEnter);
            if (SceneViewToolBar.getButtonBeOccupied() != this)
            {
                BeSelected();
                container.RegisterCallback<MouseLeaveEvent>(MouseOut);
            }
        }

        public void MouseOut(MouseLeaveEvent e)
        {
            container.UnregisterCallback<MouseLeaveEvent>(MouseOut);
            if (SceneViewToolBar.getButtonBeOccupied() != this)
            {
                UnSelected();
                container.RegisterCallback<MouseEnterEvent>(MouseEnter);
            }


        }
    }
}
#endif