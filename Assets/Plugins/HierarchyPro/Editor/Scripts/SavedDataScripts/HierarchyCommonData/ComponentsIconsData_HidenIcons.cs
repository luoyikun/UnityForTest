using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor
{


	partial class HierarchyCommonData : ScriptableObject
    {

        [SerializeField]
        List<string> HidedComponentsIconsKeys = new List<string>();
        /* [SerializeField]
         List<string> HidedComponentsIconsValues = new List<string>();*/
        Dictionary<string, bool> _IsComponentIconHided;
        internal List<string> GetComponentIconHidedList()
        {
            return HidedComponentsIconsKeys;
        }
        internal void ComponentIconHidedListRemoveAt(int index)
        {
            Undo.RecordObject(this, "SetComponentIconHide" );
            HidedComponentsIconsKeys.RemoveAt( index );
            _IsComponentIconHided = null;
            if(!Application.isPlaying) EditorUtility.SetDirty( this );
        }
        internal bool IsComponentIconHided( string comp)
        {
            if (_IsComponentIconHided == null)
            {
                // for ( int i = 0, L = Math.Min( HidedComponentsIconsKeys.Count, HidedComponentsIconsValues.Count ) ; i < L ; i++ )
                _IsComponentIconHided = new Dictionary<string, bool>();
                for (int i = 0, L = HidedComponentsIconsKeys.Count; i < L; i++)
                {
                    if (!_IsComponentIconHided.ContainsKey(HidedComponentsIconsKeys[i]))
                        _IsComponentIconHided.Add(HidedComponentsIconsKeys[i], false);
                }
            }
            if (!_IsComponentIconHided.ContainsKey(comp)) return false;
            return true;
        }
        internal void SetComponentIconHide(string comp, bool value)
        {
            Undo.RecordObject(this, "SetComponentIconHide");
            if (value)
            {
                if (_IsComponentIconHided != null && _IsComponentIconHided.ContainsKey(comp)) return;
                if (_IsComponentIconHided == null) _IsComponentIconHided = new Dictionary<string, bool>();
                _IsComponentIconHided.Add(comp, true);
                HidedComponentsIconsKeys.Add(comp);
            }
            else
            {
                if (_IsComponentIconHided == null) _IsComponentIconHided = new Dictionary<string, bool>();
                _IsComponentIconHided.Remove(comp);
                while (HidedComponentsIconsKeys.Remove(comp)) ;
            }
            if(!Application.isPlaying) SetDirty();
        }
    }
}
