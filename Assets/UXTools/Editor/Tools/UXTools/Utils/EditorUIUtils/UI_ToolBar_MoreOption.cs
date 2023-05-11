using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UIMButton
    {
        private VisualElement container;
        private Label label;
        public UIMButton(VisualElement c, Action clickAction, string text)
        {
            container = c;
            label = container.Q<Label>("label");
            label.text = text;
            container.RegisterCallback<MouseEnterEvent>(MouseEnter);
            if (clickAction != null)
            {
                container.RegisterCallback((MouseDownEvent e) =>
                {
                    if (e.button == 0)
                    {
                        clickAction.Invoke();
                    }
                });
            }
                
        }

        public void UnSelected()
        {
            container.style.backgroundColor = Color.white;
            label.style.color = Color.black;
        }

        public void BeSelected()
        {
            container.style.backgroundColor = new Color(0.14f, 0.39f, 0.76f, 1f);
            label.style.color = Color.white;
        }

        public void MouseEnter(MouseEnterEvent e)
        {
            container.UnregisterCallback<MouseEnterEvent>(MouseEnter);
            BeSelected();
            container.RegisterCallback<MouseLeaveEvent>(MouseOut);
            



        }

        public void MouseOut(MouseLeaveEvent e)
        {
            container.UnregisterCallback<MouseLeaveEvent>(MouseOut);
            UnSelected();
            container.RegisterCallback<MouseEnterEvent>(MouseEnter);


        }

    }
  }
