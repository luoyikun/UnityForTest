#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace EModules {
public class DescriptionFlush : MonoBehaviour, IDescriptionFlush {
    [SerializeField]
    string _cachedData;
    public string cachedData {get {return _cachedData;} set {_cachedData = value;}}
}
}
#endif
