#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

//This class using only for the current editor session and objects will not save to the scene asset. 
//Just that the Unity engine requires that the MonoBehaviour scripts places outside the editor folder, even it using only for editor.

namespace EMX.HierarchyPlugin.Editor
{


    public class StringFlush : MonoBehaviour
    {
        public List<string> cachedData = new List<string>();
    }
}
#endif