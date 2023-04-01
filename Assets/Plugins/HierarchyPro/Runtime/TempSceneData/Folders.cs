#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

//This class using only for the current editor session and objects will not save to the scene asset. 
//Just that the Unity engine requires that the MonoBehaviour scripts places outside the editor folder, even it using only for editor.

namespace EMX.HierarchyPlugin.Editor
{

    public class Folders
    {

        public const string ASSET_NAME = "Hierarchy Pro";

		public const string PN_FOLDER = "HierarchyPro";
		public const string PREFS_PATH = "EMX." + PN_FOLDER + "/";
        const string DEFAULT_PATH = "Assets/Plugins/" + PN_FOLDER + "";

#if !EMX_H_LITE
		public const string WIKI_PAGE = "https://emem.store/wiki?HierarchyPro%20Extended&";
#else
		public const string WIKI_PAGE = "https://emem.store/wiki?HierarchyPro&";
#endif

        // static bool hasInternal = false;
        //      public static string EMInternalFolder = null;
        //      public static string EMExternalFolder = null;
        public static bool init = false;
        [NonSerialized]
        static ScriptableObject _icons;
        static internal ScriptableObject icons { get { return _icons ?? (_icons = AssetDatabase.LoadAssetAtPath<ScriptableObject>( _PluginInternalFolder + "/Editor/Icons/IconsArray.asset" )); } }
        [NonSerialized]
        static bool wasForce = false;

        public static string UNITY_SYSTEM_PATH {
            get {
                return _UNITY_SYSTEM_PATH ?? (_UNITY_SYSTEM_PATH = Application.dataPath.Remove( Application.dataPath.Length - "Assets".Length ));
            }
        }
        static string _UNITY_SYSTEM_PATH;

        public static string PluginInternalFolder {
            get {
                if ( _PluginInternalFolder == null ) _PluginInternalFolder = EditorPrefs.GetString( PREFS_PATH + "PluginInternalFolder", DEFAULT_PATH );
                if ( !wasForce )
                {
                    CheckFolders( !icons );
                    wasForce = true;
                }
                return _PluginInternalFolder;
            }
            set {
                if ( _PluginInternalFolder == value ) return;
                EditorPrefs.SetString( PREFS_PATH + "PluginInternalFolder", _PluginInternalFolder = value );
            }
        }
        static string _PluginInternalFolder = null;
        public static string PluginExternalFolder {
            get {
                if ( _PluginExternalFolder == null ) _PluginExternalFolder = UNITY_SYSTEM_PATH + PluginInternalFolder;
                if ( !wasForce )
                {
                    CheckFolders( !icons );
                    wasForce = true;
                }
                return _PluginExternalFolder;
            }
            set {
                if ( _PluginExternalFolder == value ) return;
                _PluginExternalFolder = value;
            }
        }
        static string _PluginExternalFolder = null;

        static public AssemblyDefinitionAsset asd;

        public static void CheckFolders( bool force = false )
        {
            if ( !init || force )
            {
                init = true;



                string PAT = "EMX." + ASSET_NAME + ".Editor.asmdef";

                if ( !File.Exists( _PluginInternalFolder + "/Editor/" + PAT ) )
                {
                    bool pretty = false;
                    while ( true )
                    {
                        var o = ScriptableObject.CreateInstance( typeof( PluginPath ) );
                        if ( !o ) break;
                        var s = MonoScript.FromScriptableObject(o);
                        if ( !s ) break;
                        var candidate = AssetDatabase.GetAssetPath( s );
                        if ( string.IsNullOrEmpty( candidate ) ) break;
                        if ( !Application.isPlaying ) ScriptableObject.DestroyImmediate( o, true );
                        else ScriptableObject.Destroy( o );

                        try
                        {
                            candidate = candidate.Replace( '\\', '/' );
                            candidate = candidate.Remove( candidate.LastIndexOf( '/' ) );
                            candidate = candidate.Remove( candidate.LastIndexOf( '/' ) );
                            candidate = candidate.Remove( candidate.LastIndexOf( '/' ) );
                        }
                        catch
                        {
                            break;
                        }
                        // Debug.Log( "ASD " + candidate );

                        PluginInternalFolder = candidate;
                        _PluginExternalFolder = UNITY_SYSTEM_PATH + _PluginInternalFolder;
                        pretty = true;
                        break;
                    }

                    if ( !pretty )
                    {

                        var candidate = AssetDatabase.GetAllAssetPaths().Where(p => !string.IsNullOrEmpty(p) && p[p.Length - 1] == 'f').FirstOrDefault(a => a.EndsWith(PAT));
                        if ( candidate != null )
                        {
                            candidate = candidate.Replace( '\\', '/' );
                            candidate = candidate.Remove( candidate.LastIndexOf( '/' ) );
                            candidate = candidate.Remove( candidate.LastIndexOf( '/' ) );
                        }

                        if ( candidate == null )
                        {
                            throw new Exception( "Cannot find hierarchy assembly: " + PAT );
                        }
                        else
                        {
                            // var r = _PluginInternalFolder;
                            // _PluginExternalFolder = UNITY_SYSTEM_PATH + _PluginInternalFolder;
                            // if ( !Directory.Exists( _PluginInternalFolder ) ) Directory.CreateDirectory( _PluginInternalFolder );

                            PluginInternalFolder = candidate;
                            _PluginExternalFolder = UNITY_SYSTEM_PATH + _PluginInternalFolder;
                            if ( !Directory.Exists( _PluginInternalFolder ) ) Directory.CreateDirectory( _PluginInternalFolder );
                        }
                    }
                    /*var candidate = AssetDatabase.GetAllAssetPaths().Where(p => !string.IsNullOrEmpty(p) && p[p.Length - 1] == 'f').FirstOrDefault(a => a.EndsWith(PAT));
					if (candidate != null)
					{
						candidate = candidate.Replace('\\', '/');
						candidate = candidate.Remove(candidate.LastIndexOf('/'));
						candidate = candidate.Remove(candidate.LastIndexOf('/'));
						_PluginInternalFolder = candidate;
					}

					if (candidate == null)
					{
						throw new Exception("Cannot find hierarchy assembly: " + PAT);
					}
					else
					{
						var r = _PluginInternalFolder;
						_PluginExternalFolder = UNITY_SYSTEM_PATH + _PluginInternalFolder;
						if (!Directory.Exists(_PluginInternalFolder)) Directory.CreateDirectory(_PluginInternalFolder);
					}*/
                    //if ( PluginInternalFolder == null ) PluginInternalFolder = DEFAULT_PATH;

                }


                /* if ( !hasInternal )
                 {
                     hasInternal = true;
                     EMInternalFolder = PluginInternalFolder.Remove( PluginInternalFolder.LastIndexOf( '/' ) );
                     EMExternalFolder = PluginExternalFolder.Remove( PluginExternalFolder.LastIndexOf( '/' ) );
                 }*/
            }
        }


        public static string FIX_NAME( string old_name )
        {
            if ( string.IsNullOrEmpty( old_name ) ) old_name = "_";
            if ( old_name.IndexOf( '/' ) != -1 ) old_name = old_name.Replace( '/', '\\' );
            if ( old_name == "Default" ) old_name = "Default (1)";
            return old_name;
        }


        static Scene dasdasd;
        static Dictionary<int, Scene> cache_scenes = new Dictionary<int, Scene>();
        public static Scene GET_SCENE_BY_ID( int id )
        {
            if ( cache_scenes.TryGetValue( id, out dasdasd ) && dasdasd.IsValid() ) return dasdasd;
            for ( int i = 0; i < SceneManager.sceneCount; i++ )
            {
                if ( SceneManager.GetSceneAt( i ).GetHashCode() == id )
                {
                    if ( !cache_scenes.ContainsKey( id ) ) cache_scenes.Add( id, SceneManager.GetSceneAt( i ) );
                    else cache_scenes[ id ] = SceneManager.GetSceneAt( i );

                    return SceneManager.GetSceneAt( i );
                }
            }
            if ( EditorSceneManager.GetActiveScene().GetHashCode() == id )
                if ( !cache_scenes.ContainsKey( id ) ) cache_scenes.Add( id, EditorSceneManager.GetActiveScene() );
                else cache_scenes[ id ] = EditorSceneManager.GetActiveScene();

            return EditorSceneManager.GetActiveScene();
        }
























        public class CACHE_PATH_GETTER
        {

            Func<string> __DATA_PATH_IN; Func<string> __DATA_PATH_LY; Func<string> __DATA_PATH_EX;
            internal CACHE_PATH_GETTER( Func<string> __DATA_PATH_IN, Func<string> __DATA_PATH_LY, Func<string> __DATA_PATH_EX )
            {
                this.__DATA_PATH_IN = __DATA_PATH_IN;
                this.__DATA_PATH_LY = __DATA_PATH_LY;
                this.__DATA_PATH_EX = __DATA_PATH_EX;
            }

            public CACHE_PATH_GETTER( CACHE_PATH_GETTER source, int data )
            {
                this.__DATA_PATH_IN = source.__DATA_PATH_IN;
                this.__DATA_PATH_LY = source.__DATA_PATH_LY;
                this.__DATA_PATH_EX = source.__DATA_PATH_EX;
                DATA_PATH_USE_DEFAULT = data;
            }
            int DATA_PATH_USE_DEFAULT;

            public string GET_PATH_INTERNAL_EXCEPTIONWARNING {
                get {
                    if ( DATA_PATH_USE_DEFAULT == 0 ) return Folders.default_internal_path;
                    if ( DATA_PATH_USE_DEFAULT == 1 ) return "Assets/" + __DATA_PATH_IN();
                    //if ( DATA_SCENES_PATH_USE_DEFAULT == 2 ) return __DATA_PATH_EX;
                    throw new Exception( "GET_PATH_INTERNAL" );
                }
                set {
                    throw new Exception( "SET_PATH_INTERNAL" );
                }
            }

            public string GET_PATH_EXTERNAL {
                get {
                    if ( DATA_PATH_USE_DEFAULT == 0 ) return Folders.default_external_path;
                    if ( DATA_PATH_USE_DEFAULT == 1 ) return Folders.UNITY_SYSTEM_PATH + "Assets/" + __DATA_PATH_IN();
                    if ( DATA_PATH_USE_DEFAULT == 2 ) return Folders.UNITY_SYSTEM_PATH + __DATA_PATH_LY();
                    if ( DATA_PATH_USE_DEFAULT == 3 ) return __DATA_PATH_EX();
                    throw new Exception( "GET_PATH_EXTERNAL" );
                }
                set {
                    throw new Exception( "SET_PATH_EXTERNAL" );
                }
            }

            public string GET_PATH_TOSTRING {
                get {
                    if ( DATA_PATH_USE_DEFAULT == 0 ) return Folders.default_internal_path;
                    if ( DATA_PATH_USE_DEFAULT == 1 ) return "Assets/" + __DATA_PATH_IN();
                    if ( DATA_PATH_USE_DEFAULT == 2 ) return __DATA_PATH_LY();
                    if ( DATA_PATH_USE_DEFAULT == 3 ) return __DATA_PATH_EX();
                    throw new Exception( "GET_PATH_TOSTRING" );
                }
                set {
                    throw new Exception( "SET_PATH_INTERNAL_MIX" );
                }
            }

            public string GET_PATH_TOSTRING_CHANGABLE {
                get {
                    if ( DATA_PATH_USE_DEFAULT == 0 ) return "";
                    if ( DATA_PATH_USE_DEFAULT == 1 ) return __DATA_PATH_IN();
                    if ( DATA_PATH_USE_DEFAULT == 2 ) return __DATA_PATH_LY();
                    if ( DATA_PATH_USE_DEFAULT == 3 ) return __DATA_PATH_EX();
                    throw new Exception( "GET_PATH_TOSTRING" );
                }
                set {
                    throw new Exception( "SET_PATH_INTERNAL_MIX" );
                }
            }
            public string GET_PATH_TOSTRING_NOTCHANGABLE {
                get {
                    if ( DATA_PATH_USE_DEFAULT == 0 ) return Folders.default_internal_path;
                    if ( DATA_PATH_USE_DEFAULT == 1 ) return "Assets/";
                    if ( DATA_PATH_USE_DEFAULT == 2 ) return "";
                    if ( DATA_PATH_USE_DEFAULT == 3 ) return "";
                    throw new Exception( "GET_PATH_TOSTRING" );
                }
                set {
                    throw new Exception( "SET_PATH_INTERNAL_MIX" );
                }
            }

            public void SetOverrides( string[] overrides, int data )
            {
                DATA_PATH_USE_DEFAULT = data;
                int i = 1;
                if ( overrides[ i ] != null ) { var cap = overrides[i]; __DATA_PATH_IN = () => cap; }
                i++;
                if ( overrides[ i ] != null ) { var cap = overrides[i]; __DATA_PATH_LY = () => cap; }
                i++;
                if ( overrides[ i ] != null ) { var cap = overrides[i]; __DATA_PATH_EX = () => cap; }
            }
        }

        public enum CacheFolderType { ScenesData = 100, SettingsData = 200 }
        static CACHE_PATH_GETTER _DataScenesGetter = new CACHE_PATH_GETTER(()=>__DATA_SCENES_PATH_IN,()=>throw new Exception("Scenes Ly"),()=>__DATA_SCENES_PATH_EX);
        static CACHE_PATH_GETTER _DataSettingsGetter = new CACHE_PATH_GETTER(()=>__DATA_SETTINGS_PATH_IN,()=>__DATA_SETTINGS_PATH_LY,()=>__DATA_SETTINGS_PATH_EX);
        static  Dictionary<int, CACHE_PATH_GETTER> _DataGetterByType = new Dictionary<int, CACHE_PATH_GETTER>();
        public static CACHE_PATH_GETTER DataGetterByType( CacheFolderType type, int data )
        {
            var key = (int)type + data;
            if ( !_DataGetterByType.ContainsKey( key ) )
            {
                if ( type == CacheFolderType.ScenesData ) _DataGetterByType.Add( key, new CACHE_PATH_GETTER( _DataScenesGetter, data ) );
                else if ( type == CacheFolderType.SettingsData ) _DataGetterByType.Add( key, new CACHE_PATH_GETTER( _DataSettingsGetter, data ) );
                else throw new Exception( "CacheFolderDrawerType" );
            }
            return _DataGetterByType[ key ];
        }

        public static CACHE_PATH_GETTER DataGetterByTypeForOverrides( CacheFolderType type )
        {
            var key = (int)type + 1000;
            if ( !_DataGetterByType.ContainsKey( key ) )
            {
                if ( type == CacheFolderType.ScenesData ) _DataGetterByType.Add( key, new CACHE_PATH_GETTER( _DataScenesGetter, -1 ) );
                else if ( type == CacheFolderType.SettingsData ) _DataGetterByType.Add( key, new CACHE_PATH_GETTER( _DataSettingsGetter, -1 ) );
                else throw new Exception( "CacheFolderDrawerType" );
            }
            return _DataGetterByType[ key ];
        }

        static char[] trimpath = {' ','/'};
        internal static string default_external_path { get { return Folders.PluginExternalFolder + "/Editor/_SAVED_DATA"; } }
        internal static string default_internal_path { get { return Folders.PluginInternalFolder + "/Editor/_SAVED_DATA"; } }
        internal static string default_insideasset_path { get { return default_internal_path.Substring( "Assets/".Length ); } }



        public static int DATA_SCENES_PATH_USE_DEFAULT {
            get { return GET_INTERNAL( "DATA_SCENES_PATH_USE_DEFAULT", 0 ); }
            set { var r = DATA_SCENES_PATH_USE_DEFAULT; SET_INTERNAL( "DATA_SCENES_PATH_USE_DEFAULT", value ); }
        }
        static bool __DATA_SCENES_PATH_IN_CHECK = false;
        public static string __DATA_SCENES_PATH_IN {
            get {
                var r = GET_INTERNAL( "DATA_SCENES_PATH_IN", default_insideasset_path );
                if ( !__DATA_SCENES_PATH_IN_CHECK )
                {
                    if ( r.StartsWith( "Assets/", StringComparison.OrdinalIgnoreCase ) )
                    {
                        if ( r.Length == "Assets/".Length ) r = default_insideasset_path;
                        else r = r.Substring( "Assets/".Length );
                        SET_INTERNAL( "DATA_SCENES_PATH_IN", r );
                    }
                }
                return r;
            }
            set {
                var r = __DATA_SCENES_PATH_IN;
                SET_INTERNAL( "DATA_SCENES_PATH_IN", value.Trim( trimpath ) );
                __DATA_SCENES_PATH_IN_CHECK = false;
            }
        }
        //  public static string __DATA_SCENES_PATH_LY { get { return GET_INTERNAL( "DATA_SCENES_PATH_LY", default_internal_path ); } set { var r = __DATA_SCENES_PATH_LY; SET_INTERNAL( "DATA_SCENES_PATH_LY", value.Trim( trimpath ) ); } }
        public static string __DATA_SCENES_PATH_EX {
            get {
                throw new Exception( "External folder deprecated" );
                //return GET_INTERNAL( "DATA_SCENES_PATH_EX", default_external_path ); 
            }
            set {
                throw new Exception( "External folder deprecated" );
                //var r = __DATA_SCENES_PATH_EX; SET_INTERNAL( "DATA_SCENES_PATH_EX", value.Trim( trimpath ) );
            }
        }


        public static int DATA_SETTINGS_PATH_USE_DEFAULT {
            get { return GET_EDITOR( "DATA_SETTINGS_PATH_USE_DEFAULT", 0 ); }
            set { var r = DATA_SETTINGS_PATH_USE_DEFAULT; SET_EDITOR( "DATA_SETTINGS_PATH_USE_DEFAULT", value ); }
        }
        static bool __DATA_SETTINGS_PATH_IN_CHECK = false;
        public static string __DATA_SETTINGS_PATH_IN {
            get {
                var r = GET_EDITOR( "DATA_SETTINGS_PATH_IN", default_insideasset_path );
                if ( !__DATA_SETTINGS_PATH_IN_CHECK )
                {
                    if ( r.StartsWith( "Assets/", StringComparison.OrdinalIgnoreCase ) )
                    {
                        if ( r.Length == "Assets/".Length ) r = default_insideasset_path;
                        else r = r.Substring( "Assets/".Length );
                        SET_EDITOR( "DATA_SETTINGS_PATH_IN", r );
                    }
                }
                return r;
            }
            set {
                var r = __DATA_SETTINGS_PATH_IN;
                SET_EDITOR( "DATA_SETTINGS_PATH_IN", value.Trim( trimpath ) );
                __DATA_SETTINGS_PATH_IN_CHECK = false;
            }
        }
        public static string __DATA_SETTINGS_PATH_LY { get { return GET_EDITOR( "DATA_SETTINGS_PATH_LY", default_internal_path ); } set { var r = __DATA_SETTINGS_PATH_LY; SET_EDITOR( "DATA_SETTINGS_PATH_LY", value.Trim( trimpath ) ); } }
        public static string __DATA_SETTINGS_PATH_EX { get { return GET_EDITOR( "DATA_SETTINGS_PATH_EX", default_external_path ); } set { var r = __DATA_SETTINGS_PATH_EX; SET_EDITOR( "DATA_SETTINGS_PATH_EX", value.Trim( trimpath ) ); } }

        public static string CALC_SETTINGS_PATH_EXTERNAL { get { return DataGetterByType( CacheFolderType.SettingsData, DATA_SETTINGS_PATH_USE_DEFAULT ).GET_PATH_EXTERNAL; } }
        public static string CALC_SCENESDATA_PATH_INTERNAL { get { return DataGetterByType( CacheFolderType.ScenesData, DATA_SCENES_PATH_USE_DEFAULT ).GET_PATH_INTERNAL_EXCEPTIONWARNING; } }

        static char[] trim = new[] { '\n', '\r' };
        static int pluginID = 0;
        static string d_internal {
            get {
                return Folders.PluginExternalFolder + "/Editor/.InternalSettings/";
            }
        }
        static  Dictionary<string, object> cache = new Dictionary<string, object>();
        public static void Clear()
        {
            cache.Clear();
        }

        public static string GET_INTERNAL( string k, string def )
        {
            if ( cache.ContainsKey( k ) ) return (string)cache[ k ];
            var f = d_internal + pluginID + "-" + k;
            string res = def;
            if ( File.Exists( f ) ) res = File.ReadAllText( f ).Trim( trim );
            cache.Add( k, res );
            return res;
        }
        public static void SET_INTERNAL( string k, string val )
        {
            if ( (string)cache[ k ] == val ) return;
            else cache[ k ] = val;
            var f = d_internal + pluginID + "-" + k;
            if ( !Directory.Exists( d_internal ) ) Directory.CreateDirectory( d_internal );
            File.WriteAllText( f, val );
        }
        public static int GET_INTERNAL( string k, int def )
        {
            if ( cache.ContainsKey( k ) ) return (int)cache[ k ];
            var f = d_internal + pluginID + "-" + k;
            int res = def;
            if ( File.Exists( f ) ) res = int.Parse( File.ReadAllText( f ).Trim( trim ) );
            cache.Add( k, res );
            return res;
        }
        public static bool SET_INTERNAL( string k, int val )
        {
            if ( (int)cache[ k ] == val ) return false;
            else cache[ k ] = val;
            var f = d_internal + pluginID + "-" + k;
            if ( !Directory.Exists( d_internal ) ) Directory.CreateDirectory( d_internal );
            File.WriteAllText( f, val.ToString() );
            return true;
        }
        public static bool GET_INTERNAL( string k, bool def )
        {
            if ( cache.ContainsKey( k ) ) return (bool)cache[ k ];
            var f = d_internal + pluginID + "-" + k;
            bool res = def;
            if ( File.Exists( f ) ) res = bool.Parse( File.ReadAllText( f ).Trim( trim ) );
            cache.Add( k, res );
            return res;
        }

        public static bool SET_INTERNAL( string k, bool val )
        {
            if ( cache.ContainsKey( k ) && (bool)cache[ k ] == val ) return false;
            else cache[ k ] = val;
            var f = d_internal + pluginID + "-" + k;
            if ( !Directory.Exists( d_internal ) ) Directory.CreateDirectory( d_internal );
            File.WriteAllText( f, val.ToString() );
            return true;
        }


        public static string GET_EDITOR( string k, string def )
        {
            if ( cache.ContainsKey( k ) ) return (string)cache[ k ];
            //var f = d_internal + pluginID + "-" + k;
            string res = def;
            if ( EditorPrefs.HasKey( PREFS_PATH + k ) ) res = EditorPrefs.GetString( PREFS_PATH + k, def );
            //if ( File.Exists( f ) ) res = File.ReadAllText( f ).Trim( trim );
            cache.Add( k, res );
            return res;
        }
        public static void SET_EDITOR( string k, string val )
        {
            if ( (string)cache[ k ] == val ) return;
            else cache[ k ] = val;
            // var f = d_internal + pluginID + "-" + k;
            //if ( !Directory.Exists( d_internal ) ) Directory.CreateDirectory( d_internal );
            // File.WriteAllText( f, val );
            EditorPrefs.SetString( PREFS_PATH + k, val );
        }
        public static int GET_EDITOR( string k, int def )
        {
            if ( cache.ContainsKey( k ) ) return (int)cache[ k ];
            // var f = d_internal + pluginID + "-" + k;
            int res = def;
            if ( EditorPrefs.HasKey( PREFS_PATH + k ) ) res = EditorPrefs.GetInt( PREFS_PATH + k, def );
            cache.Add( k, res );
            return res;
        }
        public static void SET_EDITOR( string k, int val )
        {
            if ( (int)cache[ k ] == val ) return;
            else cache[ k ] = val;
            //var f = d_internal + pluginID + "-" + k;
            //if ( !Directory.Exists( d_internal ) ) Directory.CreateDirectory( d_internal );
            //File.WriteAllText( f, val.ToString() );
            //return true;
            EditorPrefs.SetInt( PREFS_PATH + k, val );
        }





        public class Clearably
        {


            public const string ProjectWindowObjectsData_TypeName =  "ProjectWindowObjectsData.asset";
            public const string ProjectWindowObjectsData =  Folders.PREFS_PATH + "ID/" +ProjectWindowObjectsData_TypeName;
            public const string HierarchyCommonData_TypeName = "HierarchyCommonData.asset";
            public const string HierarchyCommonData =Folders.PREFS_PATH + "ID/" + HierarchyCommonData_TypeName;
            public const string  HierarchyTempData_TypeName ="HierarchyTempData.asset";
            public const string  HierarchyTempData =Folders.PREFS_PATH + "ID/" +HierarchyTempData_TypeName;
            public const string Lighter_CommonData_TypeName ="HighLighterCommonData.asset";
            public const string Lighter_CommonData =Folders.PREFS_PATH + "ID/" +Lighter_CommonData_TypeName;
            public const string PresetsCommonData_TypeName = "PresetsCommonData.asset";
            public const string PresetsCommonData =Folders.PREFS_PATH + "ID/" + PresetsCommonData_TypeName;

            public static string[] _check_scenes_data_files = {
            HierarchyCommonData_TypeName ,
            Lighter_CommonData_TypeName,
            PresetsCommonData_TypeName ,
            ProjectWindowObjectsData_TypeName,
        };

            public static void ClearPrefs()
            {
                EditorPrefs.DeleteKey( ProjectWindowObjectsData );
                EditorPrefs.DeleteKey( HierarchyCommonData );
                EditorPrefs.DeleteKey( Lighter_CommonData );
                EditorPrefs.DeleteKey( PresetsCommonData );
            }


            public static void SET_EDITOR_CLEARABLY( string k, int val )
            {
                if ( (int)cache[ k ] == val ) return;
                else cache[ k ] = val;
                //var f = d_internal + pluginID + "-" + k;
                //if ( !Directory.Exists( d_internal ) ) Directory.CreateDirectory( d_internal );
                //File.WriteAllText( f, val.ToString() );
                //return true;
                EditorPrefs.SetInt( PREFS_PATH + k, val );
            }
            public static int GET_EDITOR_CLEARABLY( string k, int def = -1 )
            {
                if ( cache.ContainsKey( k ) ) return (int)cache[ k ];
                // var f = d_internal + pluginID + "-" + k;
                int res = def;
                if ( EditorPrefs.HasKey( PREFS_PATH + k ) ) res = EditorPrefs.GetInt( PREFS_PATH + k, def );
                cache.Add( k, res );
                return res;
            }

        }







    }



}



//public static string GET_SCENESDATA_PATH_EXTERNAL {
//    get {
//        if ( DATA_SCENES_PATH_USE_DEFAULT == 0 ) return default_external_path;
//        if ( DATA_SCENES_PATH_USE_DEFAULT == 1 ) return Folders.UNITY_SYSTEM_PATH + "Assets/" + __DATA_SCENES_PATH_IN;
//        //if ( DATA_SCENES_PATH_USE_DEFAULT == 2 ) return __DATA_SCENES_PATH_EX;
//        throw new Exception( "GET_SCENESDATA_PATH_EXTERNAL" );
//    }
//    set {
//        throw new Exception( "SET_SCENESDATA_PATH_EXTERNAL" );
//    }
//}
//public static string GET_SCENESDATA_PATH_INTERNAL {
//    get {
//        if ( DATA_SCENES_PATH_USE_DEFAULT == 0 ) return default_internal_path;
//        if ( DATA_SCENES_PATH_USE_DEFAULT == 1 ) return __DATA_SCENES_PATH_IN;
//        //if ( DATA_SCENES_PATH_USE_DEFAULT == 2 ) return __DATA_SCENES_PATH_EX;
//        throw new Exception( "GET_SCENESDATA_PATH_INTERNAL" );
//    }
//    set {
//        throw new Exception( "SET_SCENESDATA_PATH_INTERNAL" );
//    }
//}
//public static string GET_SCENESDATA_PATH_TOSTRING {
//    get {
//        if ( DATA_SCENES_PATH_USE_DEFAULT == 0 ) return default_internal_path;
//        if ( DATA_SCENES_PATH_USE_DEFAULT == 1 ) return __DATA_SCENES_PATH_IN;
//        //if ( DATA_SCENES_PATH_USE_DEFAULT == 2 ) return __DATA_SCENES_PATH_EX;
//        throw new Exception( "GET_SCENESDATA_PATH_TOSTRING" );
//    }
//    set {
//        throw new Exception( "SET_SCENESDATA_PATH_INTERNAL_MIX" );
//    }
//}
//public static string GET_SCENESDATA_PATH_TOSTRING_CHANGABLE {
//    get {
//        if ( DATA_SCENES_PATH_USE_DEFAULT == 0 ) return "";
//        if ( DATA_SCENES_PATH_USE_DEFAULT == 1 ) return __DATA_SCENES_PATH_IN;
//        //if ( DATA_SCENES_PATH_USE_DEFAULT == 2 ) return __DATA_SCENES_PATH_EX;
//        throw new Exception( "GET_SCENESDATA_PATH_TOSTRING" );
//    }
//    set {
//        throw new Exception( "SET_SCENESDATA_PATH_INTERNAL_MIX" );
//    }
//}
//public static string GET_SCENESDATA_PATH_TOSTRING_NOTCHANGABLE {
//    get {
//        if ( DATA_SCENES_PATH_USE_DEFAULT == 0 ) return default_internal_path;
//        if ( DATA_SCENES_PATH_USE_DEFAULT == 1 ) return "Assets/";
//        //if ( DATA_SCENES_PATH_USE_DEFAULT == 2 ) return __DATA_SCENES_PATH_EX;
//        throw new Exception( "GET_SCENESDATA_PATH_TOSTRING" );
//    }
//    set {
//        throw new Exception( "SET_SCENESDATA_PATH_INTERNAL_MIX" );
//    }
//}



#endif