using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Text;

namespace EMX.HierarchyPlugin.Editor
{


	partial class PresetsCommonData : ScriptableObject
    {

        [SerializeField]
        public string presetsData = "";


        //internal const string TypeName = "PresetsCommonData.asset";
        internal static Func<PresetsCommonData> Instance = () =>
        {

            Folders.CheckFolders();
            return (Instance = () =>
            {
                if (_Instance) return _Instance;
                // var g = EditorPrefs.GetInt(Folders.PREFS_PATH + "|PRSbjGUID" + TypeName, -1);
                var g = Folders.Clearably.GET_EDITOR_CLEARABLY(Folders.Clearably.PresetsCommonData);
                if (g != -1 && (InternalEditorUtility.GetObjectFromInstanceID(g) as PresetsCommonData))
                {
                    Folders.CheckFolders(true);
                    return (_Instance = InternalEditorUtility.GetObjectFromInstanceID(g) as PresetsCommonData);
                }

                var ASSET_PATH = HierarchyCommonData.COMMON_SCENES_FOLDER + Folders.Clearably.PresetsCommonData_TypeName;
                //var ASSET_PATH = Folders.PluginInternalFolder + "/Editor/_SAVED_DATA/" + TypeName;

				//var loaded = AssetDatabase.LoadAssetAtPath<PresetsCommonData>(ASSET_PATH);
                PresetsCommonData loaded = null;
                try
                {
                    try
                    {
                        loaded = AssetDatabase.LoadAssetAtPath<PresetsCommonData>(ASSET_PATH);
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
                    //if (!Directory.Exists(Folders.PluginInternalFolder + "/Editor/_SAVED_DATA/")) Directory.CreateDirectory(Folders.PluginInternalFolder + "/Editor/_SAVED_DATA/");
                    if (!Directory.Exists(HierarchyCommonData.COMMON_SCENES_FOLDER)) Directory.CreateDirectory(HierarchyCommonData.COMMON_SCENES_FOLDER);

                    var preCache = ScriptableObject.CreateInstance<PresetsCommonData>();
                    preCache.hideFlags = HideFlags.DontSaveInBuild;

                    if (File.Exists(ASSET_PATH)) File.Delete(ASSET_PATH);
                    AssetDatabase.CreateAsset(preCache, ASSET_PATH);
					//HierarchyExternalSceneData.SaveAssets();
					AssetDatabase.SaveAssets();

                    AssetDatabase.ImportAsset(ASSET_PATH, ImportAssetOptions.ForceUpdate);

                    loaded = preCache;
                }

                Folders.Clearably.SET_EDITOR_CLEARABLY(Folders.Clearably.PresetsCommonData, loaded.GetInstanceID());
               // EditorPrefs.SetInt(Folders.PREFS_PATH + "|PRSbjGUID" + TypeName, loaded.GetInstanceID());
                return (_Instance = loaded);
            })();
        };
        static PresetsCommonData _Instance;

        internal static void Undo( string text )
        {
            UnityEditor.Undo.RecordObject( Instance(), text );
        }
        internal static new void SetDirty()
        {
            EditorUtility.SetDirty( Instance() );
            HierarchyExternalSceneData.SaveAssets(Instance());
        }
    }




    [Serializable]
    public enum PRESET_TYPE
    {
        [SerializeField]
        NOT_INITIALIZED = 0,
        [SerializeField]
        SINGLE_COMPONENT = 1,
        [SerializeField]
        FULL_HIERARCHY = 2
    }

    [Serializable]
    public class PresetsData
    {
        const int save_version = 1;
        [SerializeField] public PresetPart singlecomp_categories;
        [SerializeField] public PresetPart fullhierarrchy_categories;
        public int Count { get { return singlecomp_categories.categories.Length + fullhierarrchy_categories.categories.Length; } }
        public PresetSet GetSetByIndex( int index )
        {
            if ( index < 0 )
            {
                throw new ArgumentOutOfRangeException();
            }
            if ( index < singlecomp_categories.categories.Length )
            {
                return singlecomp_categories.categories[ index ];
            }
            if ( index - singlecomp_categories.categories.Length < fullhierarrchy_categories.categories.Length )
            {
                return fullhierarrchy_categories.categories[ index - singlecomp_categories.categories.Length ];
            }
            throw new ArgumentOutOfRangeException();
        }

        public string Save()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( save_version.ToString() );
            sb.AppendLine( singlecomp_categories.Save( save_version ) );
            sb.AppendLine( fullhierarrchy_categories.Save( save_version ) );
            return Convert.ToBase64String( Encoding.UTF8.GetBytes( sb.ToString() ) );
        }
        public void Load( string b64 )
        {
            var sr = new StringReader(Encoding.UTF8.GetString(Convert.FromBase64String(b64)));
            var v2 = sr.ReadLine();
            if ( v2 == null || v2 == "" ) return;
            var v = int.Parse(v2);
            singlecomp_categories = new PresetPart();
            singlecomp_categories.Load( sr.ReadLine(), v );
            fullhierarrchy_categories = new PresetPart();
            fullhierarrchy_categories.Load( sr.ReadLine(), v );
        }
    }
    [Serializable]
    public class PresetPart
    {
        [SerializeField] public PRESET_TYPE SetType = PRESET_TYPE.SINGLE_COMPONENT;
        [SerializeField] public PresetSet[] categories = new PresetSet[0];
        public string Save( int v )
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( ((int)SetType).ToString() );
            if ( categories != null ) for ( int i = 0; i < categories.Length; i++ )
                {
                    sb.AppendLine();
                    sb.AppendLine( categories[ i ].Save( v ) );
                }
            return Convert.ToBase64String( Encoding.UTF8.GetBytes( sb.ToString() ) );
        }
        public void Load( string b64, int v )
        {
            var sr = new StringReader(Encoding.UTF8.GetString(Convert.FromBase64String(b64)));
            SetType = (PRESET_TYPE)int.Parse( sr.ReadLine() );
            while ( sr.ReadLine() != null )
            {
                Array.Resize( ref categories, categories.Length + 1 );
                categories[ categories.Length - 1 ] = new PresetSet();
                categories[ categories.Length - 1 ].Load( sr.ReadLine(), v );
            }
        }
    }
    [Serializable]
    public class PresetSet
    {
        [SerializeField] public string name;
        [SerializeField] public PRESET_TYPE SetType = PRESET_TYPE.SINGLE_COMPONENT;
        [SerializeField] public string PRESET_COMPONENT_TYPE;
		[SerializeField] public SetItem[] presets = new SetItem[0];

        [NonSerialized] public Texture cache_IAMGE;
        [NonSerialized] public Type cache_TYPE;

        public string Save( int v )
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( name );
            sb.AppendLine( ((int)SetType).ToString() );
            sb.AppendLine( PRESET_COMPONENT_TYPE );
            if ( presets != null ) for ( int i = 0; i < presets.Length; i++ )
                {
                    sb.AppendLine();
                    sb.AppendLine( presets[ i ].Save() );
                }
            return Convert.ToBase64String( Encoding.UTF8.GetBytes( sb.ToString() ) );
        }
        public void Load( string b64, int v )
        {
            var sr = new StringReader(Encoding.UTF8.GetString(Convert.FromBase64String(b64)));
            name = sr.ReadLine();
            SetType = (PRESET_TYPE)int.Parse( sr.ReadLine() );
            PRESET_COMPONENT_TYPE = sr.ReadLine();
            presets = new SetItem[ 0 ];
            while ( sr.ReadLine() != null )
            {
                Array.Resize( ref presets, presets.Length + 1 );
                presets[ presets.Length - 1 ] = new SetItem();
                presets[ presets.Length - 1 ].Load( sr.ReadLine(), v );
            }
        }
    }

    [Serializable]
    public class SetItem
    {
        [SerializeField] public string name;
        [SerializeField] public int id_in_external_heap;
        [SerializeField]  string _external_heap_guid_copy;
        public string external_heap_guid_copy {
            get {
                return (Encoding.UTF8.GetString( Convert.FromBase64String( _external_heap_guid_copy ) ));
            }
            set {
                _external_heap_guid_copy = Convert.ToBase64String( Encoding.UTF8.GetBytes( value ) );
            }
        }



        [SerializeField] public string json;

        public string Save()
        {
            StringBuilder sb = new StringBuilder();
            //Debug.Log( " + + " + external_heap_guid_copy + " " + json);
            sb.AppendLine( name );
            sb.AppendLine( id_in_external_heap.ToString() );
            sb.AppendLine( _external_heap_guid_copy );
            sb.AppendLine( json );
            return Convert.ToBase64String( Encoding.UTF8.GetBytes( sb.ToString() ) );
        }
        public void Load( string b64, int version )
        {
            var sr = new StringReader(Encoding.UTF8.GetString(Convert.FromBase64String(b64)));
            name = sr.ReadLine();
            id_in_external_heap = int.Parse( sr.ReadLine() );
            _external_heap_guid_copy = sr.ReadLine();
            json = sr.ReadLine();
            //Debug.Log( " - - " + external_heap_guid_copy + " " + json);
        }
    }



}
