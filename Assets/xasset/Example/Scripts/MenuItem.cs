using System;
using UnityEngine;
using UnityEngine.UI;

namespace xasset.example
{
    [Serializable]
    public class MenuItem : MonoBehaviour
    {
        public Slider slider;
        public Text progress;
        public Text intro;
        public Text title;
        public ExampleScene scene;

        public void Enter()
        {
            PreloadManager.Instance.ShowProgress(Scene.LoadAsync(name));
        }

        public void Bind(MenuItemConfig config)
        {
            name = config.title;
            title.text = config.title;
            intro.text = config.desc;
            scene = config.scene;
        }
    }
}