#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using HierarchyExtensions;

namespace EModules {
[ExecuteInEditMode]
class DescriptionRegistrator : MonoBehaviour, IDescriptionRegistrator {
    [HideInInspector]
    public string _cachedData;
    
    void OnEnable()
    {   if ((hideFlags & HideFlags.DontSaveInBuild) == 0) hideFlags |= HideFlags.DontSaveInBuild;
        Utilities.RegistrateDescription(this);
    }
    
    public string cachedData
    {   get { return _cachedData; }
        set { _cachedData = value; }
    }
    
    public Component component
    {   get { return this; }
    }
}
}
#endif
