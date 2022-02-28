using System.Collections.Generic;
using UnityEngine;

namespace xasset.example
{
    public class LoadSceneAdditive : MonoBehaviour
    {
        public string sceneName;
        private readonly List<Scene> _scenes = new List<Scene>();

        public void Unload()
        {
            if (_scenes.Count > 0)
            {
                var index = _scenes.Count - 1;
                var scene = _scenes[index];
                scene.Release();
                _scenes.RemoveAt(index);
            }
        }

        public void Load()
        {
            _scenes.Add(Scene.Load(sceneName, true));
        }
    }
}