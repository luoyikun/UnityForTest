using System.Collections.Generic;
using UnityEngine;

namespace xasset.example
{
    public class LoadAssetWithCycleReference : MonoBehaviour
    {
        private readonly List<InstantiateObject> objects = new List<InstantiateObject>();

        private void Start()
        {
            objects.Add(InstantiateObject.InstantiateAsync("Children"));
            objects.Add(InstantiateObject.InstantiateAsync("Children2"));
        }

        private void OnDestroy()
        {
            foreach (var instantiateObject in objects)
            {
                instantiateObject.Destroy();
            }

            objects.Clear();
        }
    }
}