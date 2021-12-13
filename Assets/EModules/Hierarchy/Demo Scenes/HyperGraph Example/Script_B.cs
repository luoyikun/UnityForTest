
#if UNITY_EDITOR
#pragma warning disable

using UnityEngine;

namespace MyHierarchyMenu_Example
{
    class Script_B : MonoBehaviour
    {
        public GameObject target_gmeObject = null;
        public Transform lookat_transform = null;
        public Script_C ScriptC_mover = null;
        [SerializeField]
        Script_C ScriptC_observer = null;
    }
}

#pragma warning restore
#endif
