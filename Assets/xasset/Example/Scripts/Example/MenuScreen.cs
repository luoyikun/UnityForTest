using System;
using UnityEngine;

namespace xasset.example
{
    [Serializable]
    public class MenuItemConfig
    {
        public string title;

        public ExampleScene scene;

        [TextArea] public string desc;
    }

    public class MenuScreen : MonoBehaviour
    {
        public MenuItemConfig[] configs;
        public GameObject template;
        public Transform itemRoot;

        private void Start()
        {
            foreach (var config in configs)
            {
                AsyncUpdate.RunAsync(() =>
                {
                    var go = Instantiate(template, itemRoot);
                    go.SetActive(true);
                    var item = go.GetComponent<MenuItem>();
                    item.Bind(config);
                });
            }
        }
    }
}