#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditorInternal;

//This class using only for the current editor session and objects will not save to the scene asset. 
//Just that the Unity engine requires that the MonoBehaviour scripts places outside the editor folder, even it using only for editor.

namespace EMX.HierarchyPlugin.Editor
{

    public class HierarchyTempScenePrefabs : MonoBehaviour
    {


        public static HierarchyTempScenePrefabs InstanceFast( Scene s )
        {
            var sh = s.GetHashCode();
            if ( !_Inctances.ContainsKey( sh ) ) _Inctances.Add( sh, null );
            if ( !(_T = _Inctances[ sh ]) ) _T = _Inctances[ sh ] = InstanceSlow( s );
            return _T;
        }

        static HierarchyTempScenePrefabs InstanceSlow( Scene s )
        {
            // var l = SceneManager.GetActiveScene().GetRootGameObjects().LastOrDefault();

            var oldID = SessionState.GetInt("HierarchyTempScenePrefabs" + s.GetHashCode(), -1);
            GameObject l = EditorUtility.InstanceIDToObject(oldID) as GameObject;

         //   var l = s.GetRootGameObjects().LastOrDefault();
          //  if ( l && (l.hideFlags & HierarchyTempSceneData.FLAGS) == HierarchyTempSceneData.FLAGS && l.name == HierarchyTempSceneData.GO_NAME ) { }
           // else l = null;
            if ( !l )
            {
                l = new GameObject( HierarchyTempSceneData.GO_NAME );
                l.hideFlags = HierarchyTempSceneData.FLAGS;
				SessionState.SetInt("HierarchyTempScenePrefabs" + s.GetHashCode(), l.GetInstanceID());
            }
            if ( l.GetComponent<HierarchyTempScenePrefabs>() ) return l.GetComponent<HierarchyTempScenePrefabs>();
            return l.AddComponent<HierarchyTempScenePrefabs>();
        }
        static Dictionary<int, HierarchyTempScenePrefabs> _Inctances = new Dictionary<int, HierarchyTempScenePrefabs>();
        static  HierarchyTempScenePrefabs _T;



        // PREFABS CACHE
        #region
        const int STEP = 30;


        [SerializeField] int _prefab_key_Count = 0;
        [SerializeField] List<int> GET_prefab_key = new int[STEP].ToList();
        [SerializeField] List<UnityEngine.Object> GET_prefab_value = new UnityEngine.Object[STEP].ToList();


        bool? _prafab_init;
        Dictionary<int, int> _prefab_dic = new Dictionary<int, int>();



        int _COUNT
        {
            get { return _prefab_key_Count; }
            set { _prefab_key_Count = value; }
        }
        List<int> _KEYS
        {
            get { return GET_prefab_key; }
            set { GET_prefab_key = value; }
        }
        List<UnityEngine.Object> _VALUES
        {
            get { return GET_prefab_value; }
            set { GET_prefab_value = value; }
        }








        public void ClearPrefabs( )
        {
            _prafab_init = false;
            _prefab_dic.Clear();
        }

        public Dictionary<int, int> PrefabsDic
        {
            get
            {
                if ( _prafab_init != true )
                {
                    _prafab_init = true;
                    if ( !Application.isPlaying )
                    {
                        _COUNT = 0;
                        _prefab_dic.Clear();
                        /*= new int[0];
                        GET_prefab_value = new UnityEngine.Object[0];
                        _prefab_dic.Clear();*/
                    }
                }

                if ( _prefab_dic.Count != _COUNT )
                {
                    if ( _KEYS.Count != _VALUES.Count || _COUNT != _VALUES.Count || _COUNT != _VALUES.Count )
                    {
                        var asdasd = _VALUES.ToArray();
                        System.Array.Resize( ref asdasd, _COUNT = _KEYS.Count );
                        _VALUES = asdasd.ToList();
                    }

                    _prefab_dic.Clear();
                    for ( int i = 0 ; i < _KEYS.Count ; i++ )
                    {
                        _prefab_dic.Add( _KEYS[ i ], i );
                    }
                }

                return _prefab_dic;
            }
        }

        int tempValue;

        public bool GetValueByKey( int key, out UnityEngine.Object value )
        {
            if ( !_prefab_dic.TryGetValue( key, out tempValue ) )
            {
                value = null;
                return false;
            }

            value = _VALUES[ tempValue ];
            return true;
        }
        public void PrefabsDicAdd( int key, UnityEngine.Object value )
        {
            if ( _prefab_dic.ContainsKey( key ) )
            {
                var i = _prefab_dic[key];
                if ( i != -1 )
                {
                    _VALUES[ i ] = value;
                    return;
                }
                else
                {
                    internal_Add_OnlyArray( key, value );
                    _prefab_dic[ key ] = _COUNT - 1;
                    return;
                }
            }
            else
            {
                internal_Add_OnlyArray( key, value );
                _prefab_dic.Add( key, _COUNT - 1 );
            }
        }


        void internal_Add_OnlyArray( int key, UnityEngine.Object value )
        {
            var c = _COUNT;
            if ( c >= _KEYS.Count )
            {
                _KEYS.AddRange( Enumerable.Repeat( -1, STEP ) );
                _VALUES.AddRange( Enumerable.Repeat( (UnityEngine.Object)null, STEP ) );
            }

            _KEYS[ c ] = key;
            _VALUES[ c ] = value;
            _COUNT = c + 1;
        }








        #endregion
    }
}
#endif