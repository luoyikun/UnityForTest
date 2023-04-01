using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor
{


	partial class HierarchyCommonData : ScriptableObject
    {



        [SerializeField]
        List<string> PlayModeSaverPersistScripts = new List<string>();
        /* [SerializeField]
         List<CustromIconData> HidedComponentsIconsValues = new List<CustromIconData>();*/
        internal Dictionary<string, MonoScript> _HasPlayModePreserveScripts;
        [NonSerialized] Dictionary<GameObject, bool> _dataKeeperObjects = null;
        Dictionary<GameObject, bool> dataKeeperObjects { get { return _dataKeeperObjects ?? (_dataKeeperObjects = new Dictionary<GameObject, bool>()); } }


        internal List<string> PlayModeSaverPreservedScripts_GET
        {
            get { return PlayModeSaverPersistScripts; }
            set
            {
                PlayModeSaverPersistScripts = value;
                _HasPlayModePreserveScripts = null;
                _dataKeeperObjects = null;
            }
        }



        internal Dictionary<string, MonoScript> GetPlayModeSaverPreservedScriptList()
        {
            if (_HasPlayModePreserveScripts == null)
            {
                // for ( int i = 0, L = Math.Min( PlayModePersistScripts.Count, HasCustomIconValues.Count ) ; i < L ; i++ )
                _HasPlayModePreserveScripts = new Dictionary<string, MonoScript>();
                for (int i = 0, L = PlayModeSaverPersistScripts.Count; i < L; i++)
                {

                    var path = AssetDatabase.GUIDToAssetPath(PlayModeSaverPersistScripts[i]);
                    if (string.IsNullOrEmpty(path)) continue;
                    var mono = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                    if (!mono) continue;
                    if (!_HasPlayModePreserveScripts.ContainsKey(mono.GetClass().FullName))
                    {
                        _HasPlayModePreserveScripts.Add(mono.GetClass().FullName, mono);
                    }
                }
            }
            return _HasPlayModePreserveScripts;
        }



        internal bool HasPlayModeSaverPreservedScript(Component comp)
        {

            if (_HasPlayModePreserveScripts == null)
            {
                // for ( int i = 0, L = Math.Min( PlayModePersistScripts.Count, HasCustomIconValues.Count ) ; i < L ; i++ )
                _HasPlayModePreserveScripts = new Dictionary<string, MonoScript>();
                for (int i = 0, L = PlayModeSaverPersistScripts.Count; i < L; i++)
                {
                    var path = AssetDatabase.GUIDToAssetPath(PlayModeSaverPersistScripts[i]);
                    if (string.IsNullOrEmpty(path)) continue;
                    var mono = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                    if (!mono) continue;
                    if (!_HasPlayModePreserveScripts.ContainsKey(mono.GetClass().FullName))
                    {
                        _HasPlayModePreserveScripts.Add(mono.GetClass().FullName, mono);
                    }
                }
            }
            if (!_HasPlayModePreserveScripts.ContainsKey(comp.GetType().FullName)) return false;
            return true;
        }
     /*   internal void SetPlayModeSaverPersistScript(Component comp, bool value)
        {
            Undo.RecordObject(this, "SetHasCustomIcon");
            if (value)
            {
                if (_HasPlayModePersistScripts != null && _HasPlayModePersistScripts.ContainsKey(comp.GetType().FullName)) return;
                if (_HasPlayModePersistScripts == null) _HasPlayModePersistScripts = new Dictionary<string, MonoScript>();
                _HasPlayModePersistScripts.Add(comp.GetType().FullName, MonoScript.FromMonoBehaviour(comp as MonoBehaviour));
                var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(comp as MonoBehaviour)));
                PlayModeSaverPersistScripts.Add(guid);
            }
            else
            {
                if (_HasPlayModePersistScripts == null) _HasPlayModePersistScripts = new Dictionary<string, MonoScript>();
                _HasPlayModePersistScripts.Remove(comp.GetType().FullName);
                var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(comp as MonoBehaviour)));
                while (true)
                {
                    var i = PlayModeSaverPersistScripts.IndexOf(guid);
                    if (i == -1) break;
                    PlayModeSaverPersistScripts.RemoveAt(i);
                }
            }
            EditorUtility.SetDirty(this);
        }*/


        internal bool DataKeeper_IsObjectIncluded(HierarchyObject o)
        {
            if (!dataKeeperObjects.ContainsKey(o.go))
            {
                //  var comps = HierarchyExtensions.Utilities.GetComponentFast<Component>.GetAll(o);
                var comps = o.GetComponents();
                var contains = false;

                for (int i = 0; i < comps.Length; i++)
                {
                    if (!comps[i]) continue;

                    if (HasPlayModeSaverPreservedScript((comps[i])))
                    {
                        contains = true;
                        break;
                    }
                }

                dataKeeperObjects.Add(o.go, contains);
            }

            return dataKeeperObjects[o.go];
        }








    }
}
