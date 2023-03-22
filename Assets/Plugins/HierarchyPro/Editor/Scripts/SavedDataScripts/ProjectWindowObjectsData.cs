

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace EMX.HierarchyPlugin.Editor
{


	partial class ProjectWindowObjectsData : ScriptableObject
    {

        [SerializeField]
        TempSceneObjectPTR[] objects = new TempSceneObjectPTR[0];


        internal static Func<ProjectWindowObjectsData> Instance = () =>
        {

            Folders.CheckFolders();
            return (Instance = () =>
            {
                if (_Instance) return _Instance;

                // var g = EditorPrefs.GetInt(Folders.PREFS_PATH + "|PWObjGUID" + TypeName, -1);
                var g =  Folders.Clearably.GET_EDITOR_CLEARABLY(Folders.Clearably.ProjectWindowObjectsData);

                if (g != -1 && (InternalEditorUtility.GetObjectFromInstanceID(g) as ProjectWindowObjectsData))
                {
                    Folders.CheckFolders(true);
                    return (_Instance = InternalEditorUtility.GetObjectFromInstanceID(g) as ProjectWindowObjectsData);
                }

                var ASSET_PATH = HierarchyCommonData.COMMON_SCENES_FOLDER + Folders.Clearably.ProjectWindowObjectsData_TypeName;

                ProjectWindowObjectsData loaded = null;
                try
                {
                    try
                    {
                        loaded = AssetDatabase.LoadAssetAtPath<ProjectWindowObjectsData>(ASSET_PATH);
                    }
                    catch (Exception ex)
                    {
                        if (File.Exists(ASSET_PATH))
                        {
                            var newpath = ASSET_PATH + "_backup";
                            if (File.Exists(newpath)) File.Delete(newpath);
                            File.Move(ASSET_PATH, newpath);
                            Debug.LogWarning(ex.Message + "\n\nAsset saved to: "  + newpath);
                        }
                    }
                }
                catch
                {
                    if (File.Exists(ASSET_PATH))
                    {
                        var newpath = ASSET_PATH + "_backup";
                        if (File.Exists(newpath)) File.Delete(newpath);
                        File.Move(ASSET_PATH, newpath);
                        Debug.LogWarning( "Asset saved to: "  + newpath);
                    }
                }

                if (!loaded)
                {
                    // if (!Directory.Exists(Folders.PluginInternalFolder + "/Editor/_SAVED_DATA/")) Directory.CreateDirectory(Folders.PluginInternalFolder + "/Editor/_SAVED_DATA/");
                    if (!Directory.Exists(HierarchyCommonData.COMMON_SCENES_FOLDER)) Directory.CreateDirectory(HierarchyCommonData.COMMON_SCENES_FOLDER);

                    var preCache = ScriptableObject.CreateInstance<ProjectWindowObjectsData>();
                    preCache.hideFlags = HideFlags.DontSaveInBuild;

                    if (File.Exists(ASSET_PATH)) File.Delete(ASSET_PATH);
                    AssetDatabase.CreateAsset(preCache, ASSET_PATH);
                    AssetDatabase.SaveAssets();
                    //HierarchyExternalSceneData.SaveAssets();
                    AssetDatabase.ImportAsset(ASSET_PATH, ImportAssetOptions.ForceUpdate);

                    loaded = preCache;
                }
                Folders.Clearably.SET_EDITOR_CLEARABLY(Folders.Clearably.ProjectWindowObjectsData, loaded.GetInstanceID());

               // EditorPrefs.SetInt(Folders.PREFS_PATH + "|PWObjGUID" + TypeName, loaded.GetInstanceID());
                return (_Instance = loaded);
            })();
        };
        static ProjectWindowObjectsData _Instance;

        internal static void Undo( string text )
        {
            UnityEditor.Undo.RecordObject( Instance(), text );
        }
        internal static new void SetDirty()
        {

            var i = Instance();
            if ( i.guid_to_cache == null ) return;
            Array.Resize( ref i.objects, i._cache.Count );
            int a = 0;
            foreach ( var item in i._cache )
            {
                i.objects[ a ] = item.Value;
                a++;
            }
            if ( !Application.isPlaying ) EditorUtility.SetDirty( Instance() );
            HierarchyExternalSceneData.SaveAssets(Instance());
        }


        static TempSceneObjectPTR get;
        [NonSerialized]
        Dictionary<string, TempSceneObjectPTR> guid_to_cache = null;
        [NonSerialized]
        Dictionary<string, TempSceneObjectPTR> path_to_cache = null;
        [NonSerialized]
        Dictionary<int, TempSceneObjectPTR> _cache = new Dictionary<int, TempSceneObjectPTR>();

        void init_cache()
        {
            guid_to_cache = new Dictionary<string, TempSceneObjectPTR>();
            path_to_cache = new Dictionary<string, TempSceneObjectPTR>();
            foreach ( var item in objects )
            {
                if ( item.guid == null ) continue;
                if ( !guid_to_cache.ContainsKey( item.guid ) ) guid_to_cache.Add( item.guid, item );
                if ( !path_to_cache.ContainsKey( item.path ) ) path_to_cache.Add( item.path, item );
            }
        }

        TempSceneObjectPTR _g( HierarchyObject o )
        {
            if ( _cache.TryGetValue( o.id, out get ) ) return get;
            if ( o.project == null ) throw new Exception( "o.project == null" );
            if ( guid_to_cache == null ) init_cache();


            if ( !guid_to_cache.ContainsKey( o.project.guid ) )
            {
                if ( path_to_cache.ContainsKey( o.project.assetPath ) )
                {
                    get = path_to_cache[ o.project.assetPath ];
                    guid_to_cache.Add( o.project.guid, get );
                }
                else
                {

                    //guid_to_cache.Add(o.project.guid, get);
                    //path_to_cache.Add(o.project.assetPath, get);
                    get = null;
                }
            }
            else
            {
                get = guid_to_cache[ o.project.guid ];
                if ( !path_to_cache.ContainsKey( o.project.assetPath ) ) path_to_cache.Add( o.project.assetPath, get );
            }


            _cache.Add( o.id, get );
            return get;
        }

        internal TempSceneObjectPTR GetObjectData( HierarchyObject o )
        {
            var g = _g(o);
            if ( g != null )
            {
                if ( g.path != o.project.assetPath ) g.path = o.project.assetPath;
                if ( g.guid != o.project.guid ) g.guid = o.project.guid;
            }
            return g;
        }

        internal bool RemoveObjectData( HierarchyObject o )
        {
            if ( o.project == null ) throw new Exception( "o.project == null" );

            var g = GetObjectData(o);
            if ( g == null ) return false;
            _cache.Remove( o.id );
            guid_to_cache.Remove( o.project.guid );
            path_to_cache.Remove( o.project.assetPath );
            SetDirty();
            return true;
        }


        internal bool SetObjectData( HierarchyObject o, TempSceneObjectPTR data, bool skipSave )
        {
            if ( o.project == null ) throw new Exception( "o.project == null" );
            var g = GetObjectData(o);
            //bool hasChange = false;
            if ( g == null )
            {
                g = new TempSceneObjectPTR( null, -1 );
                g.guid = o.project.guid;
                g.path = o.project.assetPath;

                if ( !path_to_cache.ContainsKey( o.project.assetPath ) ) path_to_cache.Add( o.project.assetPath, g );
                else path_to_cache[ o.project.assetPath ] = g;
                if ( !guid_to_cache.ContainsKey( o.project.guid ) ) guid_to_cache.Add( o.project.guid, g );
                else guid_to_cache[ o.project.guid ] = g;
                if ( !_cache.ContainsKey( o.id ) ) _cache.Add( o.id, g );
                else _cache[ o.id ] = g;
                //hasChange = true;
            }

            //hasChange |= !g.EQUALS( data, TempSceneObjectPTR.EQUALS_TYPE.Project );
            // Debug.Log( data.highLighterData.Length );
            //  Debug.Log( data.highLighterData[0].TempColorData.Length );
            // Debug.Log( data.highLighterData[0].TempColorData[9]);
            if ( !skipSave )
            {
                g.highLighterData = data.highLighterData;
                g.iconData = data.iconData;
                SetDirty();
            }
            return true;
        }
    }
}

