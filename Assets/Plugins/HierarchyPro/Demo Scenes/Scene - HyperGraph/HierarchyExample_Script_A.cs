
#if UNITY_EDITOR
#pragma warning disable

using UnityEngine;

namespace Examples.HierarchyPlugin
{
    class HierarchyExample_Script_A : MonoBehaviour
    {
        public HierarchyExample_Script_B TriggerEffect = null;
    }
}
 
#pragma warning restore
#endif
