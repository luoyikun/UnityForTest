
#if UNITY_EDITOR
#pragma warning disable

using UnityEngine;

namespace Examples.HierarchyPlugin
{
    class HierarchyExample_Script_B : MonoBehaviour
    {
        public GameObject target_gmeObject = null;
        public Transform lookat_transform = null;
        public HierarchyExample_Script_C ScriptC_mover = null;
        [SerializeField]
        HierarchyExample_Script_C ScriptC_observer = null;
    }
}

#pragma warning restore
#endif
