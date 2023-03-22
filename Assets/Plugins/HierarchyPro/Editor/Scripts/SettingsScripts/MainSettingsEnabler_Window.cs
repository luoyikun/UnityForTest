using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class MainSettingsEnabler_Window : ScriptableObject
	{

		// [MenuItem( "Tools/Hierarchy/Select Main Settings", false, 10000 )]
		// [CreateAssetMenu(fileName = "New UI Manager", menuName = "Modern UI Pack/New UI Manager")]
		//[MenuItem("CONTEXT/GameObject/Select Main Settings")]
		//   [MenuItem("GameObject/Hierarchy/Select Main Settings", false, 10000)]
		static Type inspectorType;
		public static bool InspectTarget( ScriptableObject target )
		{
			try
			{
				EditorWindow w = null;
				var oldWindow = SessionState.GetInt(Root.HierarchyPro + " Settings ID", -1 );
				if ( oldWindow != -1 ) w = InternalEditorUtility.GetObjectFromInstanceID( oldWindow ) as EditorWindow;
				if ( inspectorType == null ) inspectorType = typeof( UnityEditor.Editor ).Assembly.GetType( "UnityEditor.InspectorWindow" );
				if ( !w )
				{
					w = ScriptableObject.CreateInstance( inspectorType ) as EditorWindow;
					w.titleContent = new GUIContent( Root.HierarchyPro + " Settings" );
					w.name = (Root.HierarchyPro + " Settings");
					w.Show();
					SessionState.SetInt( Root.HierarchyPro + " Settings ID", w.GetInstanceID() );
				}


				///var r= EditorGUIUtility.GetStateObject(typeof(EditorWindow) , EditorGUIUtility.GetControlID( FocusType.Passive ) );
				///Debug.Log( r?.GetType().Name );

				var prevSelection = Selection.objects;
				var prevSelectionA = Selection.activeObject;
				var isLocked = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public);
				isLocked.GetSetMethod().Invoke( w, new object[] { false } );
				Selection.activeObject = target;
				isLocked.GetSetMethod().Invoke( w, new object[] { true } );
				Selection.objects = prevSelection;
				Selection.SetActiveObjectWithContext( prevSelectionA, null );
				return true;
			}
			catch
			{
				return false;
			}
		}

		internal static ScriptableObject lastSettingsGET {
			get {
				CheckSettings();
				var asd = settings.First( s => s.GetType() == lastSettings );
				return asd;
			}
		}
		internal static Type lastSettings = null;
		internal static void Select<T>( bool internalNavigation = false, bool forceSelect = false, bool skipOpenWindow = false ) where T : ScriptableObject
		{

			var p = Root.p[ 0 ];
			lastSettings = typeof( T );// 

			if ( p.par_e.OPEN_SETTINGS_WINDOW == 0 && !skipOpenWindow )
			{
				SettingsScreen.Init( null, typeof( T ), internalNavigation );
				return;
			}

			var OPEN_SETTINGS_IN_NEW_WINDOW = true;
			if ( p.par_e.OPEN_SETTINGS_WINDOW == 0 ) OPEN_SETTINGS_IN_NEW_WINDOW = false;
			else if ( p == null ) OPEN_SETTINGS_IN_NEW_WINDOW = false;
			else if ( p.par_e == null ) OPEN_SETTINGS_IN_NEW_WINDOW = false;
			if ( forceSelect || p.par_e.OPEN_SETTINGS_WINDOW == 2 ) OPEN_SETTINGS_IN_NEW_WINDOW = false;


			CheckSettings();
			var asd = settings.First( s => s.GetType() == lastSettings );

			if ( !OPEN_SETTINGS_IN_NEW_WINDOW || !InspectTarget( asd ) )
				Selection.objects = new[] { asd };

		}


		internal static List<ScriptableObject> settings = new List<ScriptableObject>();
		const string ModsSettings = ""; //"Mods Settings/";
		const string Window = "Windows"; //"Mods Settings/";
		internal const string SettingsWindow = "Settings Windows"; //"Mods Settings/";
		static int settingsIncriment = 0;
		static int settingsLeng = 22;
		internal static void CheckSettings()
		{

			try
			{
				settings.Clear();
				_Check<MainSettingsEnabler_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/MainSettingsEnabler.asset" );
				_Check<MainSettingsParams_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/MainSettingsParams.asset" );

				_Check<AS_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Integrated Mods/AutosaveSceneMod" + Window + ".asset" );
				_Check<ST_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Integrated Mods/SnapTransformMod" + Window + ".asset" );
				_Check<TB_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Integrated Mods/TopBarsMod" + Window + ".asset" );
				_Check<RC_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Integrated Mods/RightClickMenu" + Window + ".asset" );

#if !EMX_H_LITE
				_Check<EW_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "External Windows/HotButtons" + Window + ".asset" );
				_Check<BB_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "BottomBar/BottomBar" + Window + ".asset" );
				_Check<HG_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "External Windows/HyperGraph" + Window + ".asset" );
				_Check<BO_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "External Windows/BookmarksforGameObjects" + Window + ".asset" );
				_Check<BF_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "External Windows/BookmarksforProjectview" + Window + ".asset" );
				_Check<LS_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "External Windows/ScenesHistory" + Window + ".asset" );
				_Check<LO_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "External Windows/GameObjectsSelectionHistory" + Window + ".asset" );
				_Check<HE_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "External Windows/HierarchyExpandedMem" + Window + ".asset" );
#endif

				_Check<SA_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Right Side Mods/GameObjectSetActiveMod" + Window + ".asset" );
#if !EMX_H_LITE
				_Check<PK_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Right Side Mods/PlayModeSaverMode" + Window + ".asset" );
#endif
				_Check<RM_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Right Side Mods/RightHierarchyMods" + Window + ".asset" );
				_Check<IC_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Right Side Mods/IconsForComponentsMod" + Window + ".asset" );

#if !EMX_H_LITE
				_Check<HLM_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Left Drop-Down Window Mods/HighlighterManualMod" + Window + ".asset" );
				_Check<HLA_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Left Drop-Down Window Mods/HighlighterAutoMod" + Window + ".asset" );
				_Check<PM_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Left Drop-Down Window Mods/PresetsManagerMod" + Window + ".asset" );

				_Check<ProjectSettingsParams_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Project Window Mods/ProjectMainWindo" + Window + ".asset" );
				_Check<PLM_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Project Window Mods/ProjectlighterManualMod" + Window + ".asset" );
				_Check<PLA_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Project Window Mods/ProjectlighterAutoMod" + Window + ".asset" );
				_Check<PW_Window>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "Project Window Mods/ProjectFilesExtensions" + Window + ".asset" );
#endif

				_Check<AB_Extensions>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "About/AboutExtensions.asset" );
				_Check<AB_Cache>( Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "About/AboutCache.asset" );
			}
			catch ( Exception ex )
			{
				Debug.Log( ex.Message + "\n" + ex.StackTrace );
			}

			EditorUtility.ClearProgressBar();
		}




		static void _Check<T>( string path ) where T : ScriptableObject
		{
			//  ScriptableObject load ;
			// var asset = path.EndsWith(".asset") ? path.Remove(path.LastIndexOf('.')) : path;
			settingsIncriment++;
			if ( settingsIncriment > settingsLeng ) settingsIncriment = settingsLeng;

			ScriptableObject load = null;

			var id = SessionState.GetInt( Folders.PREFS_PATH + path, -1 );

			if ( id != -1 ) load = InternalEditorUtility.GetObjectFromInstanceID( id ) as ScriptableObject;

			if ( !load ) EditorUtility.DisplayProgressBar( "Creating Settings Inspector Files", "Creating Windows Files", settingsIncriment / (float)settingsLeng );

			if ( !load )
				try
				{
					load = AssetDatabase.LoadAssetAtPath<T>( path );
					SessionState.SetInt( Folders.PREFS_PATH + path, load.GetInstanceID() );
				}
				catch { }

			if ( !load )
			{
				Folders.CheckFolders();
				var dir = Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "";// path.Remove(path.LastIndexOf('/'));
				if ( !Directory.Exists( dir ) ) Directory.CreateDirectory( dir );
				var file = path.Substring(path.LastIndexOf('/') + 1);
				var finded = Directory.GetFiles(dir, "*.asset", SearchOption.AllDirectories).FirstOrDefault(f => f.EndsWith(file, StringComparison.OrdinalIgnoreCase));
				if ( !string.IsNullOrEmpty( finded ) )
				{
					finded = finded.Replace( '\\', '/' );
					finded = Folders.PluginInternalFolder + "/Editor/" + SettingsWindow + "/" + ModsSettings + "" + finded.Substring( finded.LastIndexOf( "/Editor/" + SettingsWindow + "/" ) +( "/Editor/" + SettingsWindow + "/").Length );
					load = AssetDatabase.LoadAssetAtPath<T>( finded );
					if ( load )
					{
						SessionState.SetInt( Folders.PREFS_PATH + path, load.GetInstanceID() );
					}
				}
			}

			if ( !load )
			{
				// Debug.Log(path);
				if ( File.Exists( path ) ) File.Delete( path );
				var inst = CreateInstance(typeof(T));
				if ( !Directory.Exists( path.Remove( path.LastIndexOf( '/' ) ) ) ) Directory.CreateDirectory( path.Remove( path.LastIndexOf( '/' ) ) );
				AssetDatabase.CreateAsset( load = inst, path );
				AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate );
				SessionState.SetInt( Folders.PREFS_PATH + path, load.GetInstanceID() );
			}

			settings.Add( load );

			/*if ( !sbs)
            {
                sbs = true;
               

            }*/
		}
		// static bool sbs = false;


	}


	[CustomEditor( typeof( MainSettingsEnabler_Window ) )]
	class SETGUI_MODS_ENABLER : MainRoot
	{

		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}
		public override void OnInspectorGUI()
		{
			_GUI( (IRepaint)this );
		}

		public static  Dictionary<Type, Rect> _WritePos = new Dictionary<Type, Rect>();
		public static void WritePos( Type y )
		{
			if ( !_WritePos.ContainsKey( y ) )
			{
				_WritePos.Add( y, Draw.last );
			}
			_WritePos[ y ] = Draw.last;
		}


		public static void _GUI( IRepaint w, int DRAW_CATEGORIES = 0 )
		{

			if ( Root.TemperaryPluginDisabled ) return;

			var incpector = w is MainRoot;

			// base.OnInspectorGUI();


			/*    if ( EditorGUILayout.BeginToggleGroup( "ASD", true ) )
                {

                }
                EditorGUILayout.EndToggleGroup();
                if ( EditorGUILayout.BeginFoldoutHeaderGroup( true, "ASD" ) )
                {

                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                if ( EditorGUILayout.ToggleLeft( "ASD", true ) )
                {

                }*/


			//GUI.Button( R, "gray line", s( "preToolbar2" ) );
			// GUI.Button( R, "3", s( "preToolbarLabel" ) );
			// GUI.Button( R, "border top", s( "footer" ) );
			// GUI.Button( R, "drop", s( "typeSelection" ) ); Sp( 10 );
			//   var r = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));



			var DRAW_MAIN = DRAW_CATEGORIES == 0 || DRAW_CATEGORIES == 1;
			var DRAW_INTERNAL = DRAW_CATEGORIES == 0 || DRAW_CATEGORIES == 2;
			var DRAW_LEFT = DRAW_CATEGORIES == 0 || DRAW_CATEGORIES == 3;
			var DRAW_RIGHT= DRAW_CATEGORIES == 0 || DRAW_CATEGORIES == 4;
			var DRAW_EXTERNAL= DRAW_CATEGORIES == 0 || DRAW_CATEGORIES == 5;
			var DRAW_PROJECT = DRAW_CATEGORIES == 0 || DRAW_CATEGORIES == 6;







			Draw.RESET( w );

			Draw.Sp( 8 );
			//bool use_color = p.par_e.USE_COLORS_FOR_CATEGORIES;
			bool use_color = false;
			var a = GUI.color;
			var b = GUI.backgroundColor;
			var C = GUI.color;
			var L = 0.5f;


			if ( bs == null )
			{
				bs = new GUIStyle( GUI.skin.button );
				bs.alignment = TextAnchor.MiddleLeft;
			}


			if ( DRAW_MAIN )
			{
				GUI.Button( Draw.R2, "" + Root.PN_SHORT + " " + Root.VER + " - Modules:", Draw.s( "preToolbar" ) );
			}
			else
			{
				GUI.Button( Draw.R2, "Modules:", Draw.s( "preToolbar" ) );
				Draw.Sp( 8 );
			}


			if ( DRAW_MAIN )
			{


				GUI.color *= new Color( 1, 1, 1, 0.3f );
				GUI.backgroundColor = Color.clear;
				if ( GUI.Button( Draw.R, Draw.CONT( "         - online documentation: https://emem.store/wiki" ), bs ) ) Application.OpenURL( "https://emem.store/wiki" );
				EditorGUIUtility.AddCursorRect( Draw.last, MouseCursor.Link );
				GUI.color = a;




				GUI.backgroundColor = b;
				Draw.Sp( 8 );


				Draw.TOG_TIT( "Enable " + Root.HierarchyPro, "ENABLE_ALL", ( valBefore ) => {
					//if ( !valBefore.AS_BOOL && EditorUtility.DisplayDialog( "" + Root.HierarchyPro + "", "Do you want to enable " + Root.HierarchyPro + " plugin?", "Let's rock", "No" ) ) 
					//	return true;
					//if ( valBefore.AS_BOOL && EditorUtility.DisplayDialog( "" + Root.HierarchyPro + "", "Do you want to disable " + Root.HierarchyPro + " plugin?", "Yeah, piece of cake", "No" ) )
					//	return true;
					//return false;
					return true;
				},
				  ( valAfter ) => {
					  if ( valAfter.AS_BOOL ) Root.ENABLE();
					  if ( !valAfter.AS_BOOL ) Root.DISABLE();
				  }, EnableRed: false );

				Draw.Sp( 16 );

				using ( ENABLE( w ).USE( "ENABLE_ALL", 0 ) )
				{


					if ( use_color )
						GUI.color = C * Color.Lerp( new Color32( 255, 150, 150, 255 ), Color.white, 0 );
					// if (Draw.BUT("Main Hierarchy Settings")) { MainSettingsEnabler_Window.Select<MainSettingsParams_Window>(skipOpenWindow: incpector); }
					Draw.TOG_TIT( "Main Hierarchy Settings", rightOffset: 1, EnableRed: true );
					{ }
					if ( Draw.BUT( Draw.last, "Main Hierarchy Settings" ) ) { MainSettingsEnabler_Window.Select<MainSettingsParams_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( MainSettingsParams_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_MAIN" );

					GUI.color = C;


				}




				if ( DRAW_CATEGORIES == 0 )
				{
					Draw.Sp( 20 );
					Draw.HRx2();
					Draw.Sp( 4 );
				}
				else
				{
					Draw.Sp( 16 );
				}
			}


			if ( DRAW_INTERNAL )
			{

				GUI.color = C;
				GUI.Button( Draw.R2, "Internal Mods:", Draw.s( "preToolbar" ) );
				if ( use_color )
					GUI.color = C * Color.Lerp( new Color32( 150, 200, 222, 255 ), Color.white, L );
				/*using (GRO(w).UP())
				{
					Draw.HELP(w,"Other");
				}*/
				if ( use_color )
					GUI.color = C * Color.Lerp( new Color32( 150, 200, 222, 255 ), Color.white, L );
				using ( ENABLE( w ).USE( "ENABLE_ALL", 0 ) )
				{
					using ( ENABLE( w ).USE( Draw.TOG_TIT( SETGUI_TopBar.set_text, SETGUI_TopBar.set_key, rightOffset: 1, EnableRed: true ) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_TopBar.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<TB_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( TB_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_TB" );
					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_RightClickMenu.set_text, SETGUI_RightClickMenu.set_key, rightOffset: 1 )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_RightClickMenu.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<RC_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( RC_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_RC" );
					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_Autosave.set_text, SETGUI_Autosave.set_key, rightOffset: 1 )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_Autosave.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<AS_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( AS_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_AS" );
					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_Snap.set_text, SETGUI_Snap.set_key, rightOffset: 1, AreYouSureText: "Scripts compilation will start now. Are you sure?" )
						) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_Snap.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<ST_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( ST_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_SM" );
				}





				Draw.Sp( 16 );
			}


			if ( DRAW_LEFT )
			{

#if !EMX_H_LITE

				GUI.color = C;
				GUI.Button( Draw.R2, "Left Drop-Down Window:", Draw.s( "preToolbar" ) );

				// if ( use_color ) GUI.color = C * Color.Lerp( new Color32( 130, 130, 158, 255 ), Color.white, L );

				using ( ENABLE( w ).USE( "ENABLE_ALL", 0 ) )
				{

					Draw.TOG_TIT( SETGUI_HL_HierarchyManual.set_text, rightOffset: 1, EnableRed: true );
					{ }
					if ( Draw.BUT( Draw.last, SETGUI_HL_HierarchyManual.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<HLM_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( HLM_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "HL_A" );

					//                 using ( ENABLE(w).USE( (Draw.TOG_TIT( HighlighterManualModSettingsEditor.set_text, HighlighterManualModSettingsEditor.set_key, rightOffset: 1, EnableRed: true )) ) ) { }
					//                 if ( Draw.BUT( Draw.last, HighlighterManualModSettingsEditor.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<HLM_Window>(skipOpenWindow: incpector); }
					//                 Draw.HELP_TEXTURE(Draw.last, "HL_A" );
					//using ( ENABLE(w).USE( (Draw.TOG_TIT( HighlighterAutoModSettingsEditor.set_text, HighlighterAutoModSettingsEditor.set_key, //rightOffset: 1 )) ) ) { } if ( Draw.BUT( Draw.last, HighlighterAutoModSettingsEditor.set_text + " Settings" ) ) //{ MainSettingsEnabler_Window.Select<HLA_Window>(skipOpenWindow: incpector); }
					//Draw.HELP_TEXTURE(Draw.last, "HL_B" );
					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_PresetsManager.set_text, SETGUI_PresetsManager.set_key, rightOffset: 1 )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_PresetsManager.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<PM_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( PM_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "HL_C" );



					// using ( ENABLE(w).USE( (Draw.TOG_TIT( ProjectlighterAutoModSettingsEditor.set_text, //ProjectlighterAutoModSettingsEditor.set_key, rightOffset: 1 )) ) ) { } if ( Draw.BUT( Draw.last, //ProjectlighterAutoModSettingsEditor.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<PLA_Window>(skipOpenWindow: incpector); }
					// Draw.HELP_TEXTURE(Draw.last, "HL_B" );

					//Draw.TOG_TIT(PresetsManagerModSettingsEditor.set_text);
				}
				Draw.Sp( 16 );
#endif
			}

			if ( DRAW_RIGHT )
			{


				GUI.color = C;
				GUI.Button( Draw.R2, "Right Bar:", Draw.s( "preToolbar" ) );
				if ( use_color )
					GUI.color = C * Color.Lerp( new Color32( 150, 222, 188, 255 ), Color.white, L );
				/*	using (GRO(w).UP())
					{
						Draw.HELP(w,"Mods integrated into Hierarchy Window");
					}*/
				GUI.color = C;
				//if ( use_color ) GUI.color = C * Color.Lerp( new Color32( 150, 222, 188, 255 ), Color.white, L );

				using ( ENABLE( w ).USE( "ENABLE_ALL", 0 ) )
				{

					//Draw.TOG_TIT(HighlighterModSettingsEditor.set_text);
					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_RightBar.set_text, SETGUI_RightBar.set_key, rightOffset: 1, EnableRed: true )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_RightBar.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<RM_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( RM_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_RM" );


					//GUILayout.Label( "Default Modules:" );
					//Space( 4 );

					if ( !DRAW_MAIN )
					{
						var en = GUI.enabled;
						GUI.enabled &= p.par_e.ENABLE_ALL && p.par_e.USE_RIGHT_ALL_MODS;
						Root.p[ 0 ].ChangeGUI( false );
						Windows.ModulesWindow.DRAW_MODULES( Root.p[ 0 ], Draw.RH( Windows.ModulesWindow.HH, 40, 120 ), 3, 3, true );
						Root.p[ 0 ].RestoreGUI();
						var asd = Draw.R;
						asd.x += 40;
						asd.width -= 40;
						var newM = Settings.Draw.TOG(asd, "Use Custom Modules:", Root.p[0].par_e.RIGHT_USE_CUSTOMMODULES );
						if ( newM != Root.p[ 0 ].par_e.RIGHT_USE_CUSTOMMODULES )
						{
							Root.p[ 0 ].par_e.RIGHT_USE_CUSTOMMODULES = newM;
							InternalEditorUtility.RepaintAllViews();
						}
						GUI.enabled &= Root.p[ 0 ].par_e.RIGHT_USE_CUSTOMMODULES;
						Root.p[ 0 ].ChangeGUI( false );
						Windows.ModulesWindow.DrawCustomModules1( Root.p[ 0 ], Draw.RH( Windows.ModulesWindow.HH, 40, 120 ) );
						Root.p[ 0 ].RestoreGUI();
						GUI.enabled = en;
						Draw.Sp( 8 );
					}




					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_SetActive.set_text, SETGUI_SetActive.set_key, rightOffset: 1 )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_SetActive.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<SA_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( SA_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_SA" );
					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_Icons.set_text, SETGUI_Icons.set_key, rightOffset: 1 )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_Icons.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<IC_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( IC_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_IC" );

#if !EMX_H_LITE
					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_PlayModeKeeper.set_text, SETGUI_PlayModeKeeper.set_key, rightOffset: 1 )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_PlayModeKeeper.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<PK_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( PK_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_PM" );
#endif




				}
				Draw.Sp( 16 );

			}


			if ( DRAW_EXTERNAL )
			{

			



#if !EMX_H_LITE


				//if ( use_color ) GUI.color = C * Color.Lerp( new Color32( 120, 202, 255, 255 ), Color.white, L );
				/*	using (GRO(w).UP())
					{
						Draw.HELP(w,"Mods integrated into Hierarchy Window");
					}*/
				//if ( use_color ) GUI.color = C * Color.Lerp( new Color32( 150, 222, 188, 255 ), Color.white, L );




				GUI.color = C;
				GUI.Button( Draw.R2, "External Mods - Integration Settings:", Draw.s( "preToolbar" ) );
				if ( use_color ) GUI.color = C * Color.Lerp( new Color32( 222, 200, 150, 255 ), Color.white, L );
				using ( ENABLE( w ).USE( "ENABLE_ALL", 0 ) )
				{
					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_BottomBar.set_text, SETGUI_BottomBar.set_key, rightOffset: 1, EnableRed: true )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_BottomBar.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<BB_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( BB_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "BB_M" );
					Draw.DRAW_NEW( Draw.last );
				}

				//GUI.color = C;
				//GUI.Button( Draw.R2, "External Windows", Draw.s( "preToolbar" ) );
				if ( use_color ) GUI.color = C * Color.Lerp( new Color32( 222, 200, 150, 255 ), Color.white, L );
				using ( ENABLE( w ).USE( "ENABLE_ALL", 0 ) )
				{
					Draw.TOG_TIT( SETGUI_ExternalWindows.set_text, rightOffset: 1, EnableRed: false );
					//if (  )
					//using ( ENABLE( w ).USE( () ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_ExternalWindows.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<EW_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( EW_Window ) );
				}




				GUI.color = C;

				GUI.Button( Draw.R2, "External Mods For - Bottom Bar & External Windows:", Draw.s( "preToolbar" ) );
				if ( use_color ) GUI.color = C * Color.Lerp( new Color32( 222, 200, 150, 255 ), Color.white, L );
				using ( ENABLE( w ).USE( "ENABLE_ALL", 0 ) )
				{
					//Draw.TOG_TIT( HyperGraphSettingsEditor.set_text, rightOffset: 1, EnableRed: true );
					//{ }
					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_HyperGraph.set_text, SETGUI_HyperGraph.set_key, rightOffset: 1, EnableRed: false )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_HyperGraph.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<HG_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( HG_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_HG" );

					// if ( Draw.BUT( Draw.last, HyperGraphSettingsEditor.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<HG_Window>(skipOpenWindow: incpector); }
					// WritePos( typeof( HG_Window ) );
					// Draw.HELP_TEXTURE( Draw.last, "IC_HG" );

					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_BookmarkObjectsQuick.set_text, SETGUI_BookmarkObjectsQuick.set_key, rightOffset: 1 )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_BookmarkObjectsQuick.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<BO_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( BO_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_BO" );


					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_SelectionHistory.set_text, SETGUI_SelectionHistory.set_key, rightOffset: 1 )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_SelectionHistory.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<LO_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( LO_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_HO" );

					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_LastScenes.set_text, SETGUI_LastScenes.set_key, rightOffset: 1 )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_LastScenes.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<LS_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( LS_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_HS" );

					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_ExpandedTreeItems.set_text, SETGUI_ExpandedTreeItems.set_key, rightOffset: 1 )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_ExpandedTreeItems.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<HE_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( HE_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_HE" );


				}









				GUI.color = C;


#endif

				Draw.Sp( 16 );

			}

			if ( DRAW_MAIN && DRAW_PROJECT )
			{
				Draw.Sp( 4 );
				Draw.HRx2();
				Draw.Sp( 4 );
			}

			if ( DRAW_PROJECT )
			{

				GUI.color = C;
				GUI.Button( Draw.R2, "Project Window Extensions - For Project:", Draw.s( "preToolbar" ) );
				using ( ENABLE( w ).USE( "ENABLE_ALL", 0 ) )
				{

					//if ( use_color )  GUI.color = C * Color.Lerp( new Color32( 150, 200, 222, 255 ), Color.white, L );
#if !EMX_H_LITE

					if ( use_color ) GUI.color = C * Color.Lerp( new Color32( 255, 150, 150, 255 ), Color.white, 0 );

					// if (Draw.BUT("Main Hierarchy Settings")) { MainSettingsEnabler_Window.Select<MainSettingsParams_Window>(skipOpenWindow: incpector); }
					//using (ENABLE(w).USE((Draw.TOG_TIT(HighlighterManualModSettingsEditor.set_text, HighlighterManualModSettingsEditor.set_key, rightOffset: 1)))) if (Draw.BUT(Draw.last, HighlighterManualModSettingsEditor.set_text + " Settings")) { MainSettingsEnabler_Window.Select<HLM_Window>(skipOpenWindow: incpector); }
					//	Draw.TOG_TIT("Project Window Settings", rightOffset: 1); if (Draw.BUT(Draw.last, "Main Hierarchy Settings")) { MainSettingsEnabler_Window.Select<MainSettingsParams_Window>(skipOpenWindow: incpector); }
					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_ProjectSettings.set_text, SETGUI_ProjectSettings.set_key, rightOffset: 1, EnableRed: true )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_ProjectSettings.set_text ) ) { MainSettingsEnabler_Window.Select<ProjectSettingsParams_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( ProjectSettingsParams_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_MAIN" );

					//GUI.color = C * Color.Lerp(new Color32(222, 200, 150, 255), Color.white, L);
					//GUI.color = C * Color.Lerp(new Color32(130, 130, 158, 255), Color.white, L);
					GUI.color = C;
#endif

					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_ProjectWindow.set_text, SETGUI_ProjectWindow.set_key, rightOffset: 1, EnableRed: true )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_ProjectWindow.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<PW_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( PW_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_PV" );

					GUI.color = C;
#if !EMX_H_LITE


					GUI.Button( Draw.R2, "Left Drop-Down Window Mods - For Project:", Draw.s( "preToolbar" ) );
					Draw.TOG_TIT( SETGUI_HL_ProjectManual.set_text, rightOffset: 1, EnableRed: true );
					if ( Draw.BUT( Draw.last, SETGUI_HL_ProjectManual.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<PLM_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( PLM_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "HL_A" );

					GUI.color = C;


					//   if ( use_color ) GUI.color = C * Color.Lerp( new Color32( 150, 200, 222, 255 ), Color.white, L );




					//Draw.Sp( 8 );

					GUI.Button( Draw.R2, "External Mods - For Project:", Draw.s( "preToolbar" ) );
					if ( use_color ) GUI.color = C * Color.Lerp( new Color32( 222, 200, 150, 255 ), Color.white, L );
					using ( ENABLE( w ).USE( (Draw.TOG_TIT( SETGUI_ProjectFolders.set_text, SETGUI_ProjectFolders.set_key, rightOffset: 1, EnableRed: true )) ) ) { }
					if ( Draw.BUT( Draw.last, SETGUI_ProjectFolders.set_text + " Settings" ) ) { MainSettingsEnabler_Window.Select<BF_Window>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( BF_Window ) );
					Draw.HELP_TEXTURE( Draw.last, "IC_BF" );
					GUI.color = C;
#endif
				}


				Draw.Sp( 16 );
			}

			if ( DRAW_MAIN && DRAW_PROJECT )
			{
				Draw.Sp( 4 );
				Draw.HRx2();
				Draw.Sp( 4 );
			}
			if ( DRAW_MAIN )
			{

				//Draw.Sp( 16 );
				//Draw.Sp( 8 );

				// Draw.HRx4RED();

				using ( ENABLE( w ).USE( "ENABLE_ALL", 0 ) )
				{
					Draw.TOG_TIT( SETGUI_AboutExtensions.set_text, rightOffset: 1 );
					{ }
					if ( Draw.BUT( Draw.last, SETGUI_AboutExtensions.set_text ) ) { MainSettingsEnabler_Window.Select<AB_Extensions>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( AB_Extensions ) );
					Draw.TOG_TIT( SETGUI_AboutCache.set_text, rightOffset: 1 );
					{ }
					if ( Draw.BUT( Draw.last, SETGUI_AboutCache.set_text ) ) { MainSettingsEnabler_Window.Select<AB_Cache>( true, skipOpenWindow: incpector ); }
					WritePos( typeof( AB_Cache ) );
					// if ( use_color )   GUI.color = C * Color.Lerp( new Color32( 150, 150, 150, 255 ), Color.white, L );
				}
				GUI.color = C;
				SETGUI_AboutCache.DRAW_CACHE_BUTTONS_WITH_ENABLER( w );



				Draw.Sp( 10 );
				a = GUI.color;
				b = GUI.backgroundColor;
				GUI.backgroundColor = Color.clear;
				GUI.color *= new Color( 1, 1, 1, 0.7f );

				// GUI.color = C * new Color( 1, 1, 1, 0.7f );
				GUI.backgroundColor = b;
				// GUI.color = C * Color.Lerp( new Color32( 255, 150, 150, 255 ), Color.white, 0 );
				Draw.Sp( 8 );

				var r=  Draw.R2;
				var r2=  Draw.R2;
				r.height += r2.height;
				EditorGUIUtility.AddCursorRect( r, MouseCursor.Link );
				if ( GUI.Button( r, Draw.CONT( "Open Welcome Screen" ) ) ) WelcomeScreen.Init( null );

				GUI.backgroundColor = Color.clear;

				GUI.color = a * new Color( 1, 1, 1, 0.7f );
				// Draw.Sp( 16 );
				//if ( GUI.Button( Draw.R2, Draw.CONT( "         - Open Documentation Folder: '" + Folders.PluginInternalFolder + "/Help/..'", Folders.PluginInternalFolder + "/Help/.." ), bs ) )
				//	REV( Folders.PluginExternalFolder + "/Help/Editor" );
				//EditorGUIUtility.AddCursorRect( Draw.last, MouseCursor.Link );
				// Application.OpenURL( "file://"+  );
				if ( GUI.Button( Draw.R2, Draw.CONT( "         - Online Documentation: https://emem.store/wiki" ), bs ) ) Application.OpenURL( "https://emem.store/wiki" );
				EditorGUIUtility.AddCursorRect( Draw.last, MouseCursor.Link );
				GUI.color = a;
				GUI.backgroundColor = b;

			}

			GUI.color = C;
			GUI.color = a;
			GUI.backgroundColor = b;

			//   GUI.Button( Draw.R, "6", Draw.s( "insertionMarker" ) );
			//   GUI.Button( Draw.R, "7", Draw.s( "preBackground" ) ); Draw.Sp( 10 );

			/*   var toolbar = new Toolbar();
              root.Add(toolbar);

               TemplateContainer e1 = (EditorGUIUtility.Load("UXML/InspectorWindow/InspectorWindow.uxml") as VisualTreeAsset).CloneTree();
              e1.st*/
			/* e1.AddToClassList(InspectorWindow.s_MainContainerClassName);
             this.rootVisualElement.hierarchy.Add((VisualElement) e1);
             this.m_ScrollView = e1.Q<ScrollView>((string) null, (string) null);
             VisualElement e2 = this.rootVisualElement.Q((string) null, InspectorWindow.s_MultiEditClassName);
             e2.Query<TextElement>((string) null, (string) null).ForEach<string>((Func<TextElement, string>) (label => label.text = L10n.Tr(label.text)));
             e2.RemoveFromHierarchy();
             this.m_MultiEditLabel = e2;
             this.rootVisualElement.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
             this.rootVisualElement.AddStyleSheetPath("StyleSheets/InspectorWindow/InspectorWindow.uss");*/
		}


		public static void REV( string p )
		{
			p = p.Trim( '/' ).Trim( '\\' ) + '/';
			if ( Directory.Exists( p ) )
			{
				var dirs =  Directory.GetDirectories(p,"*",SearchOption.TopDirectoryOnly);
				var files =  Directory.GetFiles(p,"*",SearchOption.TopDirectoryOnly);
				if ( dirs.Length != 0 ) EditorUtility.RevealInFinder( dirs[ 0 ] );
				else if ( files.Length != 0 ) EditorUtility.RevealInFinder( files[ 0 ] );
				else EditorUtility.RevealInFinder( p );
			}
			else
			{
				if ( File.Exists( p ) )
					EditorUtility.RevealInFinder( p );
				else
					EditorUtility.DisplayDialog( "Cannot find " + p, "File: " + p + "\nnot found", "Ok" );
			}
		}

		static GUIStyle bs;
		public static bool IsInMacOS {
			get {
				return UnityEngine.SystemInfo.operatingSystem.IndexOf( "Mac OS" ) != -1;
			}
		}

		public static bool IsInWinOS {
			get {
				return UnityEngine.SystemInfo.operatingSystem.IndexOf( "Windows" ) != -1;
			}
		}

		//[UnityEditor.MenuItem( "Window/Test OpenInFileBrowser" )]
		public static void Test()
		{
			Open( UnityEngine.Application.dataPath );
		}

		public static void OpenInMac( string path )
		{
			bool openInsidesOfFolder = false;

			string macPath = path.Replace("\\", "/");

			if ( System.IO.Directory.Exists( macPath ) )
			{
				openInsidesOfFolder = true;
			}

			if ( !macPath.StartsWith( "\"" ) )
			{
				macPath = "\"" + macPath;
			}

			if ( !macPath.EndsWith( "\"" ) )
			{
				macPath = macPath + "\"";
			}

			string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;

			try
			{
				System.Diagnostics.Process.Start( "open", arguments );
			}
			catch ( System.ComponentModel.Win32Exception e )
			{
				e.HelpLink = "";
			}
		}

		public static void OpenInWin( string path )
		{
			bool openInsidesOfFolder = false;

			string winPath = path.Replace("/", "\\");

			if ( System.IO.Directory.Exists( winPath ) )
			{
				openInsidesOfFolder = true;
			}

			try
			{
				System.Diagnostics.Process.Start( "explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath );
			}
			catch ( System.ComponentModel.Win32Exception e )
			{
				e.HelpLink = "";
			}
		}

		public static void Open( string path )
		{
			if ( IsInWinOS )
			{
				OpenInWin( path );
			}
			else if ( IsInMacOS )
			{
				OpenInMac( path );
			}
			else
			{
				OpenInWin( path );
				OpenInMac( path );
			}
		}


	}





}



