using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace EMX.HierarchyPlugin.Editor
{
	class LeftClickOnRightModsHeaderMenu
	{

		internal static void SHOW_HIER_SETTINGS_GENERICMENU()
		{
			Settings.MainSettingsEnabler_Window.Select<Settings.MainSettingsEnabler_Window>();
		}




		static PluginInstance p { get { return Root.p[ 0 ]; } }

		internal static void Open( bool addMenu = false, Action<GenericMenu> topBuilder = null )
		{
			var menu = new GenericMenu();

			//  if (addMenu)
			var pid = p.pluginID;

			if ( topBuilder != null ) topBuilder( menu );

			GUIContent cont = null;
			bool hasDisable = false;


			var WAS_DEFAULT_MOD = false;
			var WAS_USER_MOD_HEADER = false;
			var WAS_CUSTOM_MODS = false;

			foreach ( var mod in p.modsController.rightModsManager.rightMods )
			{
				if ( mod.savedData.sibling == -1 ) continue;

				var wasEnable = mod.savedData.enabled;
				var captured_mod = mod;
				// if (mod.HeaderTexture2D != null) cont = new GUIContent(GetIcon(mod.HeaderTexture2D));
				/*else*/

				if ( !mod.enableOverride() )
				{
					if ( string.IsNullOrEmpty( mod.enableOverrideMessage() ) )
					{
						continue;
						/*menu.AddDisabledItem(new GUIContent(mod.ContextHelper.ToString() + " (Pro Only)")); */
					}

					else
					{
						if ( !hasDisable )
						{
							menu.AddDisabledItem( new GUIContent( mod.ContextHelper.ToString() + " " + mod.enableOverrideMessage() ) );
						}

						hasDisable = true;
					}
				}

				else
				{


					var userType = typeof(Mod_UserModulesRoot);
					var isCustom = userType.IsAssignableFrom(mod.GetType());
					var txt =  /* is Adapter.M_UserModulesRoot*/ isCustom ? mod.ContextHelper : mod.HeaderText;

					if ( !isCustom ) WAS_DEFAULT_MOD = true;
					else WAS_CUSTOM_MODS = true;

					if ( isCustom && !WAS_USER_MOD_HEADER )
					{
						if ( WAS_DEFAULT_MOD ) menu.AddSeparator( "" );
						menu.AddItem( new GUIContent( "- Use custom modules" ), p.par_e.RIGHT_USE_CUSTOMMODULES, () => { p.par_e.RIGHT_USE_CUSTOMMODULES = !p.par_e.RIGHT_USE_CUSTOMMODULES; } );
						menu.AddSeparator( "" );
						WAS_USER_MOD_HEADER = true;
					}


					cont = new GUIContent( "- " + txt.ToString().Trim() + " -" );
					//if ( !mod.savedData.enabled && !userType.IsAssignableFrom( mod.GetType() ) ) cont.text = ("[ " + cont.text + " ]");

					menu.AddItem( cont, mod.savedData.enabled, () => {
						captured_mod.savedData.enabled = !wasEnable;
						p.RepaintWindow( pid, true );
					} );
				}
			}

			if ( !WAS_USER_MOD_HEADER )
			{
				if ( WAS_DEFAULT_MOD ) menu.AddSeparator( "" );
				menu.AddItem( new GUIContent( "- Use custom modules" ), p.par_e.RIGHT_USE_CUSTOMMODULES, () => { p.par_e.RIGHT_USE_CUSTOMMODULES = !p.par_e.RIGHT_USE_CUSTOMMODULES; } );
				WAS_CUSTOM_MODS = true;
			}



			if ( WAS_CUSTOM_MODS ) menu.AddSeparator( "" );




			//  var mp = EditorGUIUtility.GUIToScreenPoint( Event.current.mousePosition);
			var pos = new MousePos(null, MousePos.Type.ModulesListWindow_380_700, false, p); //new Rect(Event.current.mousePosition.x - 190, Event.current.mousePosition.y, 0, 0)
			var w = p.window;


			menu.AddItem( new GUIContent( "[ Open modules table ☷ ]" ), false, () => { Windows.ModulesWindow.Init( pos, w ); } );
			menu.AddSeparator( "" );


			//* Static Members **/
			CONTEXTMENU_STATICMODULES( menu );

			// menu.AddSeparator( "" );

			var ws = p.par_e;
#if !EMX_H_LITE
			menu.AddItem( new GUIContent( "- Use BottomBar" ), ws.USE_BOTTOMBAR_MOD, () => { ws.USE_BOTTOMBAR_MOD = !ws.USE_BOTTOMBAR_MOD; } );
#endif

			menu.AddSeparator( "" );


			menu.AddItem( new GUIContent( "Auto-hide if width < " + p.par_e.RIGHTDOCK_TEMPHIDEMINWIDTH ),
				p.par_e.RIGHTDOCK_TEMPHIDE, () => {
					p.par_e.RIGHTDOCK_TEMPHIDE = !p.par_e.RIGHTDOCK_TEMPHIDE;
				} );

			if ( p.pluginID == 1 )
			{
				menu.AddItem( new GUIContent( "'*.*' Display files extension" ), p.par_e.DRAW_EXTENSION_IN_PROJECT && p.par_e.DRAW_EXTENSION_IN_PROJECT, () => {
					if ( !p.par_e.DRAW_EXTENSION_IN_PROJECT ) p.par_e.DRAW_EXTENSION_IN_PROJECT = true;
					p.par_e.DRAW_EXTENSION_IN_PROJECT = !p.par_e.DRAW_EXTENSION_IN_PROJECT;
				} );
			}

			if ( EditorSceneManager.sceneCount < 2 )
				menu.AddItem( new GUIContent( "Bind header to the scene line" ), p.par_e.RIGHT_HEADER_BIND_TO_SCENE_LINE, () => {
					p.par_e.RIGHT_HEADER_BIND_TO_SCENE_LINE = !p.par_e.RIGHT_HEADER_BIND_TO_SCENE_LINE;
				} );
			else
				menu.AddDisabledItem( new GUIContent( "Bind header to the scene line" ) );





			menu.AddSeparator( "" );


			ADD_LAYOUTS( ref menu );

			menu.AddItem( new GUIContent( CONTENT_SIZE + "Default" ), ws.RIGHTDOCK_SHRINK_BUTTONS_INT == 0, () => { ws.RIGHTDOCK_SHRINK_BUTTONS_INT = 0; } );
			menu.AddItem( new GUIContent( CONTENT_SIZE + "Shrink clicking area" ), ws.RIGHTDOCK_SHRINK_BUTTONS_INT == 1, () => { ws.RIGHTDOCK_SHRINK_BUTTONS_INT = 1; } );
			menu.AddItem( new GUIContent( CONTENT_SIZE + "Compact style" ), ws.RIGHTDOCK_SHRINK_BUTTONS_INT == 2, () => { ws.RIGHTDOCK_SHRINK_BUTTONS_INT = 2; } );


			menu.AddSeparator( "" );

			/*menu.AddItem( new GUIContent( "[ Open settings ]/Open settings" ), false, () => {
				SHOW_HIER_SETTINGS_GENERICMENU();
			} );
			menu.AddItem( new GUIContent( "[ Open settings - Right bar ]" ), false, () => {
				Settings.MainSettingsEnabler_Window.Select<Settings.RM_Window>();
			} );*/

			menu.AddItem( new GUIContent( "[ Open settings ]/Open Settings" ), false, () => {
				SHOW_HIER_SETTINGS_GENERICMENU();
			} );
			menu.AddItem( new GUIContent( "[ Open settings ]/Open Right Bar Settings" ), false, () => {
				Settings.MainSettingsEnabler_Window.Select<Settings.RM_Window>();
			} );



			menu.AddSeparator( "[ Open settings ]/" );
			var s = EditorSceneManager.GetActiveScene();
			if ( s.IsValid() && !string.IsNullOrEmpty( s.path ) ) menu.AddItem( new GUIContent(  "[ Open settings ]/" + "> Select scene file in project" ), false, () => {
				UnityEditor.EditorUtility.FocusProjectWindow();
				var ap = AssetDatabase.LoadAssetAtPath<SceneAsset>( s.path );
				Selection.objects = new[] { ap };
			} );
			else menu.AddDisabledItem( new GUIContent(  "[ Open settings ]/" +"> Select scene file in project" ) );
			menu.AddItem( new GUIContent(  "[ Open settings ]/" +"> Select cache file in project" ), false, () => {

				//var s = EditorSceneManager.GetActiveScene();
				//var sp= s.path;
				UnityEditor.EditorUtility.FocusProjectWindow();
				//var sp = HierarchyExternalSceneData.GetScenePath(s);
				var p = HierarchyExternalSceneData.GetProjectHash(s);
				Selection.objects = new[] { p };
				//var d = MOI.des(-1);
				//Selection.objects = new[] { d as UnityEngine.Object };
			} );




			if ( !addMenu )
			{ /*   menu.AddSeparator("");
			       // menu.AddDisabledItem(new GUIContent("Click on the item for more options"));
			       // menu.AddDisabledItem(new GUIContent("Click below to get more options.."));
			       menu.AddDisabledItem(new GUIContent("Click on the object's line to configure"));*/
			}
			else
			{
				menu.AddSeparator( "" );
				menu.AddDisabledItem( new GUIContent( "Drag this icon to change size of right bar" ) );
			}


			menu.ShowAsContext();
			// EventUse();
		}

		const string CATEGORY = "Right Bar - Layouts/";
		const string CONTENT_SIZE = "Right Bar - Content Style/";





		static void CONTEXTMENU_STATICMODULES( GenericMenu menu )
		{
			var HierarchyAdapterInstance = Root.p[0];

			GUIContent cont = null;
			/*  if ( HierarchyAdapterInstance.par.DataKeeperParams.ENABLE )
                  cont = new GUIContent( HierarchyAdapterInstance.modules.First( m => m is M_PlayModeKeeper ).HeaderText.ToString() );
              else*/
#if !EMX_H_LITE

			cont = new GUIContent( "- Use " + p.modsController.playModeKeeperMod.ContextHelper.ToString()  );//+ " -"
			menu.AddItem( cont, p.par_e.USE_PLAYMODE_SAVER_MOD, () => {
				p.par_e.USE_PLAYMODE_SAVER_MOD = !p.par_e.USE_PLAYMODE_SAVER_MOD;
			} );
#endif

			cont = new GUIContent( "- Use " + p.modsController.componentsIconsMod.SearchHelper.ToString()  );
			menu.AddItem( cont, p.par_e.USE_COMPONENTS_ICONS_MOD, () => {
				p.par_e.USE_COMPONENTS_ICONS_MOD = !p.par_e.USE_COMPONENTS_ICONS_MOD;
			} );
			/* if ( HierarchyAdapterInstance.par.DataKeeperParams.ENABLE )
                 cont = new GUIContent( HierarchyAdapterInstance.modules.First( m => m is M_SetActive ).HeaderText.ToString() );
             else*/
			cont = new GUIContent( "- Use " + p.modsController.setActiveMod.ContextHelper.ToString() );
			menu.AddItem( cont, p.par_e.USE_SETACTIVE_MOD, () => {
				p.par_e.USE_SETACTIVE_MOD = !p.par_e.USE_SETACTIVE_MOD;
			} );
			//if ( !p.par_e.USE_SETACTIVE_MOD )
			//{
			//	cont = new GUIContent( "- Setactive module style" );
			//	menu.AddDisabledItem( cont );
			//}
			//else
			//{
			//	/*  cont = new GUIContent( "- SetActive Module Style/Left Small" );
            //      menu.AddItem( cont, HierarchyAdapterInstance.SETACTIVE_POSITION == 1, ( ) =>
            //       {
            //           HierarchyAdapterInstance.SETACTIVE_POSITION = 1;
            //       } );
            //       */
			//
			//	cont = new GUIContent( "- Setactive module style/Right default" );
			//	menu.AddItem( cont, p.par_e.SET_ACTIVE_POSITION == 0, () => {
			//		p.par_e.SET_ACTIVE_POSITION = 0;
			//	} );
			//
			//	cont = new GUIContent( "- Setactive module style/Right small" );
			//	menu.AddItem( cont, p.par_e.SET_ACTIVE_POSITION == 2, () => {
			//		p.par_e.SET_ACTIVE_POSITION = 2;
			//	} );
			//
			//	menu.AddSeparator( "- Setactive module style/" );
			//
			//	cont = new GUIContent( "- Setactive module style/Contrast style" );
			//	menu.AddItem( cont, p.par_e.SET_ACTIVE_STYLE == 1, () => {
			//		p.par_e.SET_ACTIVE_STYLE = 1 - p.par_e.SET_ACTIVE_STYLE;
			//	} );
			//}


		}


		static void ADD_LAYOUTS( ref GenericMenu menu )
		{
			var current_state = TakeModulesSnapShot();
			var saved_states = saved_states_get;
			var findIndex = saved_states.FindIndex(s => s.data == current_state.data);


			var pos = new MousePos(Event.current.mousePosition, MousePos.Type.Input_128_68, false, p);

			var w = p.window;
			//  var pos = InputData.WidnwoRect(FocusRoot.WidnwoRectType.Clamp, Event.current.mousePosition, 128, 68, this);
			if ( findIndex != -1 ) menu.AddDisabledItem( new GUIContent( CATEGORY + "Save current" ) );
			else
				menu.AddItem( new GUIContent( CATEGORY + "Save current" ), false, () => {
					Windows.InputWindow.Init( pos, "New Item", w, ( str ) => {
						if ( string.IsNullOrEmpty( str ) ) return;

						str = str.Trim();
						current_state.name = str;
						saved_states.Add( current_state );
						saved_states_get = saved_states;
					}, textInputSet: "MyWorkflow" );
				} );

			if ( findIndex == -1 ) menu.AddDisabledItem( new GUIContent( CATEGORY + "Remove current" ) );
			else
				menu.AddItem( new GUIContent( CATEGORY + "Remove current" ), false, () => {
					saved_states.RemoveAt( findIndex );
					saved_states_get = saved_states;
				} );

			if ( saved_states.Count != 0 ) menu.AddSeparator( CATEGORY );
			var pid = p.pluginID;

			for ( int i = 0; i < saved_states.Count; i++ )
			{
				var captureName = saved_states[i].name;
				var captureSnap = saved_states[i].data;
				menu.AddItem( new GUIContent( CATEGORY + captureName ), findIndex == i, () => { SetDirtyModulesSnapShots( captureName, captureSnap, pid ); } );
			}


			menu.AddSeparator( CATEGORY );

			menu.AddItem( new GUIContent( CATEGORY + "- default" ), NOW_DEFAULT(), () => {
				SET_TO_DEFAULT();
				p.RepaintWindow( pid, true );
			} );

			menu.AddItem( new GUIContent( CATEGORY + "- show all" ), p.modsController.rightModsManager.rightMods.All( m => m.savedData.enabled ) && p.par_e.USE_PLAYMODE_SAVER_MOD, () => {
				foreach ( var module in p.modsController.rightModsManager.rightMods ) module.savedData.enabled = true;
				p.par_e.USE_PLAYMODE_SAVER_MOD = true;
				p.RepaintWindow( pid, true );
			} );
			menu.AddItem( new GUIContent( CATEGORY + "- hide all" ), p.modsController.rightModsManager.rightMods.All( m => !m.savedData.enabled ) && !p.par_e.USE_PLAYMODE_SAVER_MOD, () => {
				foreach ( var module in p.modsController.rightModsManager.rightMods ) module.savedData.enabled = false;
				p.par_e.USE_PLAYMODE_SAVER_MOD = false;
				//   modules.First( m => m is IModuleOnnector_M_CustomIcons ).enable = true;
				//   modules.First( m => m is IModuleOnnector_M_CustomIcons ).enable = true;
				p.RepaintWindow( pid, true );
			} );
		}

		static bool NOW_DEFAULT()
		{
			var modules = p.modsController.rightModsManager.rightMods;
			foreach ( var module in modules ) if ( !module.savedData.NowDefault() ) return false;
			if ( p.par_e.USE_PLAYMODE_SAVER_MOD ) return false;
			return true;
		}
		static internal void SET_TO_DEFAULT()
		{
			var modules = p.modsController.rightModsManager.rightMods;
			foreach ( var module in modules )
			{
				if ( module.savedData.sibling == -1 ) continue;

				module.savedData.SetToDefault();
				// Debug.Log( DefaulTypes[0] + " " + module.GetType().FullName );
				// if ( DefaulTypes.Any( d => d == (module.GetType().FullName) ) ) module.enable = true;
				// else module.enable = false;
			}
			p.par_e.USE_PLAYMODE_SAVER_MOD = false;
			p.par_e.USE_SETACTIVE_MOD = true;
			// p.par_e.RIGHT_PADDING_LEFT_READABLE
		}


		class snap_res
		{
			internal string name;
			internal string data;
		}
		static char[] trim= {'\n','\r',' '};
		static snap_res TakeModulesSnapShot()
		{

			var final = "";
			var modules = p.modsController.rightModsManager.rightMods;
			for ( int i = 0; i < modules.Length; i++ )
			{
				var result = "";
				result += modules[ i ].MODULE_KEY + "#";
				result += modules[ i ].savedData.enabled + "#";
				result += modules[ i ].savedData.sibling + "#";
				result += modules[ i ].savedData.width + "\n\r";
				final += result;
			}
			final += "KEEPER#" + p.par_e.USE_PLAYMODE_SAVER_MOD + "\n\r";
			final += "SETACTIVE#" + p.par_e.USE_SETACTIVE_MOD + "\n\r";

			return new snap_res() { name = "", data = final.Trim( trim ) };
		}

		static void SetDirtyModulesSnapShots( string name, string data, int pid )
		{

			foreach ( var module in p.modsController.rightModsManager.rightMods ) module.savedData.enabled = false;
			p.par_e.USE_PLAYMODE_SAVER_MOD = false;
			p.par_e.USE_SETACTIVE_MOD = false;

			var rioghtMods = p.modsController.rightModsManager.rightMods.ToDictionary(m=>m.MODULE_KEY,v=>v);

			foreach ( var _i in data.Split( '\n' ) )
			{
				var i = _i.Trim(trim);

				if ( i.StartsWith( "KEEPER#" ) ) p.par_e.USE_PLAYMODE_SAVER_MOD = bool.Parse( i.Split( '#' )[ 1 ].Trim( trim ) );
				else if ( i.StartsWith( "SETACTIVE#" ) ) p.par_e.USE_SETACTIVE_MOD = bool.Parse( i.Split( '#' )[ 1 ].Trim( trim ) );
				else
				{
					var read = i.Split('#').Select(s=>s.Trim(trim)).ToArray();
					if ( rioghtMods.ContainsKey( read[ 0 ] ) )
					{
						var m = rioghtMods[read[0]];
						m.savedData.enabled = bool.Parse( read[ 1 ] );
						m.savedData.sibling = int.Parse( read[ 2 ] );
						m.savedData.width = int.Parse( read[ 3 ] );
					}
				}
			}

			p.RepaintWindow( pid, true );
			// RepaintAllViews();
		}


		/* int GetCountSnapShots()
         {
             return EditorPrefs.GetInt( pluginname + "/Layouts/SnapsCount", 0 );
         }*/

		static List<snap_res> saved_states_get {
			get {
				var r = p.par_e.GET("RIGHT_MODULES_STATES","");
				//  var snaps = EditorPrefs.GetString(pluginname + "/Layouts/SnapsSer", null);
				if ( !string.IsNullOrEmpty( r ) )
				{
					List<snap_res> res = new List<snap_res>();
					foreach ( var s in r.Split( '*' ) )
					{
						var read = s.Trim(trim).Split('$');
						var c = new snap_res();
						c.name = read[ 0 ].Trim( trim );
						c.data = read[ 1 ].Trim( trim );
						res.Add( c );
					}
					return res;
				}
				return new List<snap_res>();
			}

			set {
				// var ser = SERIALIZE_SINGLE(value);
				//  EditorPrefs.SetString( pluginname + "/Layouts/SnapsSer", ser );
				var r = p.par_e.GET("RIGHT_MODULES_STATES","");
				p.par_e.SET( "RIGHT_MODULES_STATES", value.Select( v => v.name + "$\n\r" + v.data ).Aggregate( ( a, b ) => a + "*\n\r" + b ) );
			}
		}

	}
}
