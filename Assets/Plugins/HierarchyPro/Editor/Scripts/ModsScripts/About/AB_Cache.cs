


using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class AB_Cache : ScriptableObject
    {
    }
    [CustomEditor( typeof( AB_Cache ) )]
    class SETGUI_AboutCache : MainRoot
    {

        internal static string set_text = "About Cache";
        internal static string set_key = "";
        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }
        public override void OnInspectorGUI()
        {
            _GUI( (IRepaint)this );
        }


        static bool ValidateInternalTrue( string folder )
        {
            return true;
        }
        static bool ValidateInternalLybrary( string res )
        {
            if ( !res.StartsWith( Folders.UNITY_SYSTEM_PATH, StringComparison.OrdinalIgnoreCase ) || res.Length <= Folders.UNITY_SYSTEM_PATH.Length )
            {
                EditorUtility.DisplayDialog( "Warning! Folder cannot be selected", "You have to choose folder in the your project folder Library or Temp or etc\n" + res, "Ok" );
                return false;
            }
            return true;
        }
        static string TrimLybrary( string res )
        {
            return res.Substring( Folders.UNITY_SYSTEM_PATH.Length ).Trim( '/' );
        }

        static  CacheFolderDrawer.LABELS_DATA settings_labels = new CacheFolderDrawer.LABELS_DATA(){
            HEADER = "Plugin's Settings Path:",
            TOOLBAR = new[] { "Internal Folder", "Assets Folder", "Project Folder", "External Folder" },
            PATH_LABEL = "Settings",
        };
        static  CacheFolderDrawer.KEYS_DATA settings_keys = new CacheFolderDrawer.KEYS_DATA(
            DATA_SETTINGS_PATH_USE_DEFAULT: "DATA_SETTINGS_PATH_USE_DEFAULT",
            __DATA_SETTINGS_PATH_IN: "__DATA_SETTINGS_PATH_IN",
            __DATA_SETTINGS_PATH_LY: "__DATA_SETTINGS_PATH_LY",
            __DATA_SETTINGS_PATH_EX: "__DATA_SETTINGS_PATH_EX",
            DATA_SETTINGS_PATH_TEMP: "DATA_SETTINGS_PATH_TEMP",
            DATA_SETTINGS_ID_TEMP: "DATA_SETTINGS_ID_TEMP",
            GET_SETTINGS_PATH_EXTERNAL: "GET_SETTINGS_PATH_EXTERNAL",
            GET_SETTINGS_PATH_TOSTRING: "GET_SETTINGS_PATH_TOSTRING",
            GET_SETTINGS_PATH_TOSTRING_CHANGABLE: "GET_SETTINGS_PATH_TOSTRING_CHANGABLE",
            GET_SETTINGS_PATH_TOSTRING_NOTCHNGABLE: "GET_SETTINGS_PATH_TOSTRING_NOTCHANGABLE"
        );
        static CacheFolderDrawer.FOLDER_DATA settings_folders = new CacheFolderDrawer.FOLDER_DATA(
            prefix : "",
            valudate : ValidateInternalTrue,
            doMovingConten : true,
            isInternal: false,
            isDelayedFinalization : true,
            CheckHiddenFoldersForInternal : false,
            trimResult: null,
            SubFolders : EditorSettingsAdapter._check_settings_data_folder,
            SubFiles : null
        );



        static  CacheFolderDrawer.LABELS_DATA scenes_labels = new CacheFolderDrawer.LABELS_DATA(){
            HEADER = "Scenes Cache Path:",
            TOOLBAR = new[] { "Internal Folder", "Assets Folder" },
            PATH_LABEL = "Scenes Cache",
        };
        static  CacheFolderDrawer.KEYS_DATA scenes_keys = new CacheFolderDrawer.KEYS_DATA(
            DATA_SETTINGS_PATH_USE_DEFAULT: "DATA_SCENES_PATH_USE_DEFAULT",
            __DATA_SETTINGS_PATH_IN: "__DATA_SCENES_PATH_IN",
            __DATA_SETTINGS_PATH_LY: "",
            __DATA_SETTINGS_PATH_EX: "",
            DATA_SETTINGS_PATH_TEMP: "DATA_SCENES_PATH_TEMP",
            DATA_SETTINGS_ID_TEMP: "DATA_SCENES_ID_TEMP",
            GET_SETTINGS_PATH_EXTERNAL: "GET_SCENESDATA_PATH_EXTERNAL",
               GET_SETTINGS_PATH_TOSTRING: "GET_SCENESDATA_PATH_TOSTRING",
            GET_SETTINGS_PATH_TOSTRING_CHANGABLE: "GET_SCENESDATA_PATH_TOSTRING_CHANGABLE",
            GET_SETTINGS_PATH_TOSTRING_NOTCHNGABLE: "GET_SCENESDATA_PATH_TOSTRING_NOTCHANGABLE"

        );
        static CacheFolderDrawer.FOLDER_DATA scenes_folders = new CacheFolderDrawer.FOLDER_DATA(
            prefix : "",
            valudate : ValidateInternalTrue,
            doMovingConten : true,
            isInternal: false,
            isDelayedFinalization : true,
            CheckHiddenFoldersForInternal : true,
            trimResult: null,
            SubFolders : new[]{HierarchyExternalSceneData.DATA_SUB_FOLDER } ,
            SubFiles : Folders.Clearably._check_scenes_data_files
        );



        static CacheFolderDrawer settings_drawer = new CacheFolderDrawer(Folders.CacheFolderType.SettingsData,settings_labels, settings_keys,settings_folders );
        static CacheFolderDrawer scenes_drawer = new CacheFolderDrawer( Folders.CacheFolderType.ScenesData, scenes_labels, scenes_keys,scenes_folders );
        public static void DRAW_CACHE_BUTTONS_WITH_ENABLER( IRepaint w )
        {
            using ( ENABLE( w ).USE( "ENABLE_ALL", 0 ) )
            {
                var L = 0.5f;
                var C = GUI.color;
                GUI.color = C * Color.Lerp( new Color32( 150, 150, 150, 255 ), Color.white, L );

                //Draw.TOG_TIT( "Save Settings to:" );
                //Draw.TOG_TIT( "Save Scenes/Project Data to:" );
                //  Draw.HRx1();
                // Draw.TOG_TIT( "Cache:", EnableRed: false );
                GUI.Label( Draw.R2, "Cache:" );
                using ( GRO( w ).UP( 20 ) )
                {
                    settings_drawer.VOID_DRAW_LINE( w );
                    Draw.Sp( 5 );
                    //Draw.HRx1();
                    scenes_drawer.VOID_DRAW_LINE( w );
                    Draw.Sp( 5 );
                }
                var r = Draw.R;
                r.width /= 3;
                var r2 = r;
                r2.width *= 2;
                GUI.color *= new Color( 1, 1, 1, 0.5f );
                EditorGUIUtility.AddCursorRect( r2, MouseCursor.Link );
                if ( GUI.Button( r2, Draw.CONT( "Open " + Root.PN_FOLDER + " Folder" ) ) )
                {
                    SETGUI_MODS_ENABLER.REV( Folders.PluginExternalFolder );
                }
                r.x += r.width * 2;
                EditorGUIUtility.AddCursorRect( r, MouseCursor.Link );
                if ( GUI.Button( r, Draw.CONT( "Force Reload Cache", "Force Reload Cache\nUse it if your moved files manually" ) ) )
                {
                    if ( EditorUtility.DisplayDialog( "Reload Cache", "Are you sure?", "Yes", "No" ) )
                        ClearCacheAndUpdate();
                }
                Draw.Sp( 20 );
                GUI.color = C;

            }
            // Draw.HRx1();
        }


        internal static void ClearCacheAndUpdate()
        {
            EditorUtility.DisplayProgressBar( "Reloading HierarchyPro", "HierarchyPro is reloading, please wait:", 0.5f );
            try
            {
                Root.p[ 0 ].par_e.ClearCache();
                HierarchyTempSceneData.RemoveCache();
                Root.p[ 0 ].invoke_ReloadAfterAssetDeletingOrPasting();
                settings_drawer.Reset();
                scenes_drawer.Reset();
                Folders.Clearably.ClearPrefs();
                AssetDatabase.Refresh( ImportAssetOptions.ForceSynchronousImport );
                Root.RequestScriptReload();
            }
            catch ( Exception ex )
            {
                EditorUtility.ClearProgressBar();
                throw new Exception( ex.Message + '\n' + ex.StackTrace, ex );
            }

            EditorUtility.ClearProgressBar();

        }

        /*

        GUI.Label( Draw.R, Draw.CONT( "Save Scenes Cache Path:" ) );
        //Draw.TOG_TIT( "Save scenes cache path:" );
        //Draw.TOG_TIT( "Scenes cache path:" );
        Draw.TOOLBAR( new[] { "Hierarchy Folder", "Asset Folder", "External Folder" }, "DATA_SCENES_PATH_USE_DEFAULT",
            enabled: new bool[ 3 ] { true, true, false } );
        switch ( p.par_e.DATA_SETTINGS_PATH_USE_DEFAULT )
        {
            case 0:
                break;
            case 1:
                Draw.FOLDER( w, "Settings:", "__DATA_SCENES_PATH_IN", "", ValidateInternal, true, true );
                break;
            case 2:
                Draw.FOLDER( w, "Settings:", "__DATA_SCENES_PATH_EX", "", ValidateInternal, true, false );
                break;
        }
        /// GUI.Label( Draw.R, Draw.CONT( "Example: " + p.par_e.GET_SCENESDATA_PATH_TOSTRING ) );
        ///     GUI.Label( Draw.R, Draw.CONT( "Example: " ) );
        ///            GUILayout.BeginHorizontal();
        GUI.Label( Draw.R, Draw.CONT( "Example: " ) );
        GUILayout.BeginHorizontal();
        GUILayout.Space( 20 );
        GUILayout.TextArea( p.par_e.GET_SCENESDATA_PATH_TOSTRING );
        GUILayout.Space( 30 );
        GUILayout.EndHorizontal();
        */


        public static void _GUI( IRepaint w )
        {
            Draw.RESET( w );

            Draw.BACK_BUTTON( w );
            Draw.TOG_TIT( set_text, WIKI: WIKI_1_CACHE );
            Draw.Sp( 10 );

            DRAW_CACHE_BUTTONS_WITH_ENABLER( w );


            Draw.HELP_TEXTURE( w, "HELP_CACHE_A" );



            Draw.HELP( w, "All cache stored separate from the scenes, there are no any data saved in the scene, only temporarily 'EDITOR_ONLY' cache, for the current editor session and removing after closing the editor.", drawTog: true );
            Draw.Sp( 10 );
            Draw.HELP( w, "You can add 'EMX/" + Root.PN_FOLDER + "/' to git ignore file, to ensure compatibility with people who does not own a " + Root.HierarchyPro + ".", drawTog: true );
            Draw.HELP( w, "You can add 'EMX/" + Root.PN_FOLDER + "/Editor/_SAVED_DATA' to git ignore file, to ensure compatibility with people with different scenes settings.", drawTog: true );
            //Draw.Sp(10);
            //Draw.HELP(w,"All data has an improved caching system to reach maximum performance in the editor during initialization.", drawTog: true);


            Draw.Sp( 10 );

            Draw.HELP( w, "You can move scenes cache and settings to another folder. You may set common external folder on your hard drive and use the same settings for different projects. Path to 'Plugin Settings' stored in the 'EditorPrefs' and shared between different projects, Path to 'Scenes Cache' considered a local param and stored in the '...HierarchyPro/Editor/.InternalSettings'", drawTog: false );
            Draw.Sp( 20 );

            //Draw.Sp( 10 );
            //Draw.HRx2();

            //	GUI.Label(Draw.R, "However you may lost some data.");
            Draw.HELP( w, "All data for each scene stored in separate file, you can find it in the '_SAVED_DATA' folder by default. Might data lost? Probably, yes. When you will rename, move or copy scene some links to scenes may missing, but in this case, you can reassign it manually, there're a special copy/paste methods for data files. Also, there're a special automatic methods that tracking scenes naming changes or moving or duplicating.", drawTog: true );
            Draw.HELP_TEXTURE( w, "HELP_CACHE_B" );
            Draw.HELP( w, "You can manually rename, copy or move data files in '_SAVED_DATA' by default.", drawTog: true );
            Draw.HELP( w, "You can copy data from another scene using special copy/paste buttons.", drawTog: true );

            //Draw.Sp( 10 );
            //Draw.HELP( w, "There will be a special manager that will allow to recover or reassign each objects manually but only in the future. Anyway hope this version will work fine for you, because there is a completely different caching system compared with the previous versions.", drawTog: true );


            Draw.Sp( 10 );
            Draw.HRx2();
            Draw.HELP( w, "Yeah, all editor settings located by default in the 'EMX/" + Root.PN_FOLDER + "/Editor/_SAVED_DATA/.EditorSettings/' so you can copy it to your other project using file browser, or move it to folder outside of unity projects.", drawTog: true );
            Draw.HELP( w, "Yeah, and if you wanna reset settings to default just remove this folder: '/.EditorSettings/'.", drawTog: true );
            Draw.HELP( w, "If you assign external folder outside of unity project, every new project will load or create settings in the external folders'.", drawTog: true );
            //Draw.HELP( w, "Yeah, some data located to 'Library/HierarchyPro/' like icons history, colors history, to avoid unnecessary changes in ScriptableObjects.", drawTog: true );

            Draw.Sp( 10 );
            Draw.HRx2();
            Draw.HELP( w, "If you have problems disabling any module, you can find a 'HardDisableMods.cs' script in the root, and replace the field value to 'false'.", drawTog: true );
            Draw.Sp( 10 );

            Draw.HRx2();





        }


        internal class CacheFolderDrawer
        {
            internal struct LABELS_DATA
            {
                internal string HEADER;
                internal string[] TOOLBAR;
                internal string PATH_LABEL;
            }

            internal struct FOLDER_DATA
            {
                internal string prefix;
                internal Func<string, bool> valudate;
                internal bool doMovingConten;
                internal  bool isInternal;
                internal bool isDelayedFinalization;
                internal Func<string, string> trimResult;
                internal bool CheckHiddenFoldersForInternal;
                internal string[] SubFolders;
                internal string[] SubFiles;

                public FOLDER_DATA( string prefix, Func<string, bool> valudate, bool doMovingConten, bool isInternal, bool isDelayedFinalization, Func<string, string> trimResult, bool CheckHiddenFoldersForInternal, string[] SubFolders, string[] SubFiles )
                {
                    this.prefix = prefix;
                    this.valudate = valudate;
                    this.doMovingConten = doMovingConten;
                    this.isInternal = isInternal;
                    this.isDelayedFinalization = isDelayedFinalization;
                    this.trimResult = trimResult;
                    this.CheckHiddenFoldersForInternal = CheckHiddenFoldersForInternal;
                    this.SubFolders = SubFolders;
                    this.SubFiles = SubFiles;
                }
            }
            internal struct KEYS_DATA
            {
                FIELD_SETTER _DATA_SETTINGS_PATH_USE_DEFAULT; internal FIELD_SETTER DATA_SETTINGS_PATH_USE_DEFAULT { get { return _DATA_SETTINGS_PATH_USE_DEFAULT; } }
                FIELD_SETTER _DATA_SETTINGS_PATH_IN; internal FIELD_SETTER __DATA_SETTINGS_PATH_IN { get { return _DATA_SETTINGS_PATH_IN; } }
                FIELD_SETTER _DATA_SETTINGS_PATH_LY; internal FIELD_SETTER __DATA_SETTINGS_PATH_LY { get { return _DATA_SETTINGS_PATH_LY; } }
                FIELD_SETTER _DATA_SETTINGS_PATH_EX; internal FIELD_SETTER __DATA_SETTINGS_PATH_EX { get { return _DATA_SETTINGS_PATH_EX; } }
                // FIELD_SETTER _DATA_SETTINGS_PATH_TEMP; internal FIELD_SETTER DATA_SETTINGS_PATH_TEMP { get { return _DATA_SETTINGS_PATH_TEMP; } }
                // FIELD_SETTER _DATA_SETTINGS_ID_TEMP; internal FIELD_SETTER DATA_SETTINGS_ID_TEMP { get { return _DATA_SETTINGS_ID_TEMP; } }
                // FIELD_SETTER _GET_SETTINGS_PATH_EXTERNAL; internal FIELD_SETTER GET_SETTINGS_PATH_EXTERNAL { get { return _GET_SETTINGS_PATH_EXTERNAL; } }
                // FIELD_SETTER _GET_SETTINGS_PATH_TOSTRING; internal FIELD_SETTER GET_SETTINGS_PATH_TOSTRING { get { return _GET_SETTINGS_PATH_TOSTRING; } }
                // FIELD_SETTER _GET_SETTINGS_PATH_TOSTRING_CHANGABLE; internal FIELD_SETTER GET_SETTINGS_PATH_TOSTRING_CHANGABLE { get { return _GET_SETTINGS_PATH_TOSTRING_CHANGABLE; } }
                // FIELD_SETTER _GET_SETTINGS_PATH_TOSTRING_NOTCHNGABLE; internal FIELD_SETTER GET_SETTINGS_PATH_TOSTRING_NOTCHNGABLE { get { return _GET_SETTINGS_PATH_TOSTRING_NOTCHNGABLE; } }

                internal KEYS_DATA(
                    string DATA_SETTINGS_PATH_USE_DEFAULT,
                    string __DATA_SETTINGS_PATH_IN,
                    string __DATA_SETTINGS_PATH_LY,
                    string __DATA_SETTINGS_PATH_EX,
                    string DATA_SETTINGS_PATH_TEMP,
                    string DATA_SETTINGS_ID_TEMP,
                    string GET_SETTINGS_PATH_EXTERNAL,
                    string GET_SETTINGS_PATH_TOSTRING,
                    string GET_SETTINGS_PATH_TOSTRING_CHANGABLE,
                    string GET_SETTINGS_PATH_TOSTRING_NOTCHNGABLE )
                {
                    if ( !string.IsNullOrEmpty( DATA_SETTINGS_PATH_USE_DEFAULT ) ) this._DATA_SETTINGS_PATH_USE_DEFAULT = Draw.GetSetter( DATA_SETTINGS_PATH_USE_DEFAULT );
                    else this._DATA_SETTINGS_PATH_USE_DEFAULT = null;
                    if ( !string.IsNullOrEmpty( __DATA_SETTINGS_PATH_IN ) ) this._DATA_SETTINGS_PATH_IN = Draw.GetSetter( __DATA_SETTINGS_PATH_IN );
                    else this._DATA_SETTINGS_PATH_IN = null;
                    if ( !string.IsNullOrEmpty( __DATA_SETTINGS_PATH_LY ) ) this._DATA_SETTINGS_PATH_LY = Draw.GetSetter( __DATA_SETTINGS_PATH_LY );
                    else this._DATA_SETTINGS_PATH_LY = null;
                    if ( !string.IsNullOrEmpty( __DATA_SETTINGS_PATH_EX ) ) this._DATA_SETTINGS_PATH_EX = Draw.GetSetter( __DATA_SETTINGS_PATH_EX );
                    else this._DATA_SETTINGS_PATH_EX = null;
                    // if ( !string.IsNullOrEmpty( DATA_SETTINGS_PATH_TEMP ) ) this._DATA_SETTINGS_PATH_TEMP = Draw.GetSetter( DATA_SETTINGS_PATH_TEMP );
                    // else this._DATA_SETTINGS_PATH_TEMP = null;
                    // if ( !string.IsNullOrEmpty( DATA_SETTINGS_ID_TEMP ) ) this._DATA_SETTINGS_ID_TEMP = Draw.GetSetter( DATA_SETTINGS_ID_TEMP );
                    // else this._DATA_SETTINGS_ID_TEMP = null;
                    // if ( !string.IsNullOrEmpty( GET_SETTINGS_PATH_EXTERNAL ) ) this._GET_SETTINGS_PATH_EXTERNAL = Draw.GetSetter( GET_SETTINGS_PATH_EXTERNAL );
                    // else this._GET_SETTINGS_PATH_EXTERNAL = null;
                    // if ( !string.IsNullOrEmpty( GET_SETTINGS_PATH_TOSTRING ) ) this._GET_SETTINGS_PATH_TOSTRING = Draw.GetSetter( GET_SETTINGS_PATH_TOSTRING );
                    // else this._GET_SETTINGS_PATH_TOSTRING = null;
                    // if ( !string.IsNullOrEmpty( GET_SETTINGS_PATH_TOSTRING_CHANGABLE ) ) this._GET_SETTINGS_PATH_TOSTRING_CHANGABLE = Draw.GetSetter( GET_SETTINGS_PATH_TOSTRING_CHANGABLE );
                    // else this._GET_SETTINGS_PATH_TOSTRING_CHANGABLE = null;
                    // if ( !string.IsNullOrEmpty( GET_SETTINGS_PATH_TOSTRING_NOTCHNGABLE ) ) this._GET_SETTINGS_PATH_TOSTRING_NOTCHNGABLE = Draw.GetSetter( GET_SETTINGS_PATH_TOSTRING_NOTCHNGABLE );
                    // else this._GET_SETTINGS_PATH_TOSTRING_NOTCHNGABLE = null;
                }
            }

            Folders.CacheFolderType type;
            LABELS_DATA LBS;
            FOLDER_DATA DSP_D;
            KEYS_DATA KEYS;

            internal CacheFolderDrawer( Folders.CacheFolderType type, LABELS_DATA l, KEYS_DATA k, FOLDER_DATA d )
            {
                this.type = type;
                LBS = l;
                DSP_D = d;
                KEYS = k;
                Reset();
            }

            Action[] temp_delayedFinalizationInternal = new Action[10];
            string[] temp_folder_to_apply = new string[10];
            int temp_type_to_apply = -1;


            internal void Reset()
            {
                for ( int i = 0; i < temp_folder_to_apply.Length; i++ ) temp_folder_to_apply[ i ] = null;
                for ( int i = 0; i < temp_delayedFinalizationInternal.Length; i++ ) temp_delayedFinalizationInternal[ i ] = null;
                temp_type_to_apply = (int)KEYS.DATA_SETTINGS_PATH_USE_DEFAULT.value;
            }


            void DrawPath( Folders.CACHE_PATH_GETTER getter, bool doColor )
            {
                var c = getter.GET_PATH_TOSTRING_CHANGABLE;
                var nc = getter.GET_PATH_TOSTRING_NOTCHANGABLE;


                GUILayout.BeginHorizontal();
                GUILayout.Space( 20 );
                var H = GUILayout.Height(GUI.skin.textArea.fixedHeight != 0 ? GUI.skin.textArea.fixedHeight : 15 );
                var E = GUILayout.ExpandWidth(false);

                bool wt = !string.IsNullOrEmpty( c );

                var CC = GUI.color;
                if ( doColor ) GUI.color *= new Color( 1.5f, 0.2f, 0.1f, 1 );
                if ( !string.IsNullOrEmpty( nc ) )
                {
                    if ( !wt ) GUILayout.Label( nc, H, E );
                    else
                    {
                        //var r = GUILayoutUtility.GetLastRect();
                        // GUILayout.BeginArea( r );
                        GUILayout.Label( nc, H, E );
                        //GUILayout.EndArea();
                    }
                }
                if ( wt ) GUILayout.TextArea( c );
                GUI.color = CC;

                GUILayout.Space( 30 );
                GUILayout.EndHorizontal();
            }

            void DrawChangeFolderButtons( IRepaint w, string cf_String = null )
            {
                switch ( temp_type_to_apply )
                {
                    case 0:

                        break;
                    case 1:
                        DSP_D.isInternal = true;
                        DSP_D.prefix = "Assets/";
                        DSP_D.valudate = ValidateInternalTrue;
                        DSP_D.trimResult = null;
                        Root.p[ 0 ].par_e.STRING_TEMP = (string)KEYS.__DATA_SETTINGS_PATH_IN.value;
                        Action<string,string[],string[]> delayedFinalization1;
                        Draw.FOLDER( w, LBS.PATH_LABEL + ":", "STRING_TEMP", DSP_D, out delayedFinalization1, drawDecorations: false, cf_String: cf_String );
                        if ( delayedFinalization1 != null )
                        {
                            if ( !string.Equals( (string)KEYS.__DATA_SETTINGS_PATH_IN.value, Root.p[ 0 ].par_e.STRING_TEMP, StringComparison.OrdinalIgnoreCase ) )
                            {
                                var d1 = KEYS.__DATA_SETTINGS_PATH_IN.KEY;
                                var d2 = DSP_D.SubFiles;
                                var d3 = DSP_D.SubFolders;
                                temp_folder_to_apply[ temp_type_to_apply ] = (string)Root.p[ 0 ].par_e.STRING_TEMP;
                                temp_delayedFinalizationInternal[ temp_type_to_apply ] = () => {
                                    delayedFinalization1( d1, d2, d3 );
                                };
                            }
                        }
                        break;
                    case 2:
                        DSP_D.isInternal = false;
                        DSP_D.prefix = "";
                        DSP_D.valudate = ValidateInternalLybrary;
                        DSP_D.trimResult = TrimLybrary;
                        Root.p[ 0 ].par_e.STRING_TEMP = (string)KEYS.__DATA_SETTINGS_PATH_LY.value;
                        Action<string,string[],string[]> delayedFinalization2;
                        var res = Draw.FOLDER( w, LBS.PATH_LABEL + ":",  "STRING_TEMP", DSP_D, out delayedFinalization2, drawDecorations: false, cf_String: cf_String );
                        if ( delayedFinalization2 != null )
                        {
                            if ( !string.Equals( (string)KEYS.__DATA_SETTINGS_PATH_LY.value, Root.p[ 0 ].par_e.STRING_TEMP, StringComparison.OrdinalIgnoreCase ) )
                            // if ( KEYS.__DATA_SETTINGS_PATH_LY != KEYS.DATA_SETTINGS_PATH_TEMP )
                            {
                                var d1 = KEYS.__DATA_SETTINGS_PATH_LY.KEY;
                                var d2 = DSP_D.SubFiles;
                                var d3 = DSP_D.SubFolders;
                                temp_folder_to_apply[ temp_type_to_apply ] = (string)Root.p[ 0 ].par_e.STRING_TEMP;
                                temp_delayedFinalizationInternal[ temp_type_to_apply ] = () => {
                                    delayedFinalization2( d1, d2, d3 );
                                };
                            }
                        }
                        break;
                    case 3:
                        DSP_D.isInternal = false;
                        DSP_D.prefix = "";
                        DSP_D.valudate = ValidateInternalTrue;
                        DSP_D.trimResult = null;
                        Root.p[ 0 ].par_e.STRING_TEMP = (string)KEYS.__DATA_SETTINGS_PATH_EX.value;
                        Action<string,string[],string[]> delayedFinalization3;
                        Draw.FOLDER( w, LBS.PATH_LABEL + ":", "STRING_TEMP", DSP_D, out delayedFinalization3, drawDecorations: false, cf_String: cf_String );
                        if ( delayedFinalization3 != null )
                        {
                            if ( !string.Equals( (string)KEYS.__DATA_SETTINGS_PATH_EX.value, Root.p[ 0 ].par_e.STRING_TEMP, StringComparison.OrdinalIgnoreCase ) )
                            //if ( KEYS.__DATA_SETTINGS_PATH_EX != KEYS.DATA_SETTINGS_PATH_TEMP )
                            {
                                var d1 = KEYS.__DATA_SETTINGS_PATH_EX.KEY;
                                var d2 = DSP_D.SubFiles;
                                var d3 = DSP_D.SubFolders;
                                temp_folder_to_apply[ temp_type_to_apply ] = (string)Root.p[ 0 ].par_e.STRING_TEMP;
                                temp_delayedFinalizationInternal[ temp_type_to_apply ] = () => {
                                    delayedFinalization3( d1, d2, d3 );
                                };
                            }
                        }
                        break;
                }
            }

            internal void VOID_DRAW_LINE( IRepaint w )
            {
                var s = GUI.skin.label.fontStyle;
                GUI.skin.label.fontStyle = FontStyle.Bold;
                // try { GUI.Label( Draw.R, Draw.CONT( "- " + LBS.HEADER ) ); } catch { }
                try { Draw.TOG_TIT( Draw.CONT( "- " + LBS.HEADER ) ); } catch { }
                GUI.skin.label.fontStyle = s;
                Draw.Sp( 4 );
                //Draw.TOG_TIT( ":" );
                //Draw.TOG_TIT( "Settings path:" );
                Root.p[ 0 ].par_e.INT_TEMP = temp_type_to_apply;
                Draw.TOOLBAR( LBS.TOOLBAR, "INT_TEMP" , disableResetToDefault: true);
                temp_type_to_apply = Root.p[ 0 ].par_e.INT_TEMP;
                //switch ( KEYS.DATA_SETTINGS_PATH_USE_DEFAULT )

                ///////////
                //EXAMPLE
                var mem_t =  Folders.DataGetterByType( type , (int)KEYS.DATA_SETTINGS_PATH_USE_DEFAULT.value );
                var temp_t = Folders.DataGetterByTypeForOverrides( type  );
                temp_t.SetOverrides( temp_folder_to_apply, temp_type_to_apply );

                GUI.Label( Draw.R, Draw.CONT( "   Current Folder: " ) );//LBS.PATH_LABEL +
                                                                        //var c = (string)KEYS.GET_SETTINGS_PATH_TOSTRING_CHANGABLE.value;
                                                                        // var nc = (string)KEYS.GET_SETTINGS_PATH_TOSTRING_NOTCHNGABLE.value;
                DrawPath( mem_t, false );
                //EXAMPLE
                ///////////


                if ( GUI.Button( Draw.R, "Open Current Folder" ) )
                {
                    //MainSettingsEditor.REV( (string)KEYS.GET_SETTINGS_PATH_TOSTRING.value );
                    SETGUI_MODS_ENABLER.REV( mem_t.GET_PATH_TOSTRING );
                }

                //KEYS.DATA_SETTINGS_PATH_TEMP.value = "";


                //////////
                //APPLY
                var doApplyFin = (int)temp_type_to_apply > 0 && temp_delayedFinalizationInternal[ temp_type_to_apply ] != null;
                var doApplyCat = temp_type_to_apply != (int)KEYS.DATA_SETTINGS_PATH_USE_DEFAULT.value;
                //if ( doApplyCat )
                //{
                //    var p1 = temp_t.GET_PATH_EXTERNAL;
                //    var p2 = mem_t.GET_PATH_EXTERNAL;
                //    if ( string.Equals( p1, p2, StringComparison.OrdinalIgnoreCase ) ) doApplyCat = false;
                //}


                if ( !doApplyFin && !doApplyCat )
                    DrawChangeFolderButtons( w );


                if ( doApplyFin || doApplyCat )
                {
                    using ( GRO( w ).UP( 0 ) )
                    {



                        // KEYS.GET_SETTINGS_PATH_EXTERNAL
                        GUI.Label( Draw.R, Draw.CONT( "   New Folder: " ) );//LBS.PATH_LABEL +
                        DrawChangeFolderButtons( w, "Change New Folder" );

                        DrawPath( temp_t, true );




                        //var displayFolder = Folders.DataGetterByType(type).SEND(temp_type_to_apply).GET_PATH_TOSTRING;
                        //GUI.color *= new Color( 1.5f, 0.2f, 0.1f, 1 );
                        //GUILayout.BeginHorizontal();
                        //GUILayout.Space( 20 );
                        //GUILayout.TextArea( displayFolder );
                        ////GUILayout.TextArea( temp_folder_to_apply[ temp_type_to_apply ] );
                        //GUILayout.Space( 30 );
                        //GUILayout.EndHorizontal();

                        var CC = GUI.color;
                        GUI.color *= new Color( 1.5f, 0.2f, 0.1f, 1 );
                        var r= Draw.R2;
                        r.width /= 4;
                        var www = r.width;
                        r.width *= 3;


                        //if ( GUI.Button( r, Draw.CONT( "Apply",
                        //    "Change path from:\n...-" + KEYS.GET_SETTINGS_PATH_TOSTRING.value + "\nto:\n...-" + temp_string + "\n\nfor "
                        //    + LBS.PATH_LABEL.ToLower() + " location" ) ) )
                        //{
                        if ( GUI.Button( r, Draw.CONT( "Apply New Folder",
                            "Change path for " + LBS.PATH_LABEL.ToLower() + " location" ) ) )
                        {

                            var data = new Draw.MOVE_COMPLETED_ACTION_DATA(mem_t,  DSP_D.SubFiles, DSP_D.SubFolders);
                            KEYS.DATA_SETTINGS_PATH_USE_DEFAULT.value = temp_type_to_apply;
                            if ( temp_delayedFinalizationInternal[ temp_type_to_apply ] != null ) temp_delayedFinalizationInternal[ temp_type_to_apply ]();
                            data.SET_TARGET( Folders.DataGetterByType( type, (int)KEYS.DATA_SETTINGS_PATH_USE_DEFAULT.value ) );

                            Draw.MOVE_COMPLETED_ACTION( data );
                            ClearCacheAndUpdate();
                        }
                        GUI.color = CC;
                        r.x += r.width;
                        r.width = www;
                        if ( GUI.Button( r, Draw.CONT( "Cancel",
                            "Cancel to apply path:\n...-" + temp_t.GET_PATH_TOSTRING + "\nfor "
                            + LBS.PATH_LABEL.ToLower() + " location" ) ) )
                        {
                            Reset();
                        }


                        if ( GUI.Button( Draw.R, "Open New Folder" ) )
                        {
                            //MainSettingsEditor.REV( (string)KEYS.GET_SETTINGS_PATH_TOSTRING.value );
                            //Debug.Log( temp_t.GET_PATH_TOSTRING );
                            SETGUI_MODS_ENABLER.REV( temp_t.GET_PATH_TOSTRING );
                        }
                    }
                    //APPLY
                    //////////
                    ///

                    Draw.Sp( 15 );

                    //GUI.Label( Draw.R, Draw.CONT( "Example: " + p.par_e.GET_SETTINGS_PATH_TOSTRING ) );
                    // GUI.TextArea( Draw.R, Draw.CONT( "Example: " + p.par_e.GET_SETTINGS_PATH_TOSTRING ) );



                }
                Draw.Sp( 5 );
            }


        }

    }



}

