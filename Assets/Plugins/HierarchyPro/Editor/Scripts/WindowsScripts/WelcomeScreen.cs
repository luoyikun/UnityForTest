using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EMX.HierarchyPlugin.Editor.Settings;

namespace EMX.HierarchyPlugin.Editor
{


	class WelcomeScreen : EditorWindow, Icons.GetTextureObject, IRepaint
	{
		const int buttonW = 240;

		public static void Init( Rect? __source ) // var w = GetWindow<WelcomeScreen>( "Post Presets - Welcome Screen" , true, Params.WindowType,)
		{
			_source = __source;
			EditorApplication.update += showw;
		}

		static Rect? _source;
		static void showw()
		{
			var d  = Screen.currentResolution.height / 1080f;
			var source = _source ?? new Rect(0, d * 140, Screen.currentResolution.width, Screen.currentResolution.height - d * 280);
			//var w = GetWindow<WelcomeScreen>(true, "" + Root.PN + " - Welcome", true);
			var w = CreateWindow<WelcomeScreen>( "" + Root.HierarchyPro + " - Welcome");
			var thisR = new Rect(0, source.y, buttonW + (Screen.currentResolution.width < 1500 ? 430 : Screen.currentResolution.width < 2100 ? 750 : 1000), Math.Max(source.height,
				Math.Min(Screen.currentResolution.height, 1080) - d *280));
			thisR.x = source.x + source.width / 2 - thisR.width / 2;
			thisR.y = source.y + source.height / 2 - thisR.height / 2;
			w.position = thisR;
			EditorApplication.update -= showw;
		}

		void drawTexture( Texture2D texture )
		{
			var dif = Mathf.Clamp((position.width - buttonW) / texture.width, 0.01f, 1);
			var rect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.RoundToInt(texture.height * dif * 0.95f)));
			rect.height += 2;
			GUI.DrawTexture( rect, texture, ScaleMode.ScaleToFit );
		}

		Vector2 scrollPos;
		Dictionary<int, List<Texture2D>> t = new Dictionary<int, List<Texture2D>>();
		char[] parts = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
		GUIStyle __emptyStyle;

		GUIStyle emptyStyle {
			get {
				return __emptyStyle ?? (__emptyStyle = new GUIStyle() {
					active = new GUIStyleState() { background = GUI.skin.box.normal.background ?? GUI.skin.box.normal.scaledBackgrounds[ 0 ] },
					clipping = TextClipping.Overflow,
					alignment = TextAnchor.MiddleLeft,
					border = new RectOffset( 5, 5, 5, 5 )
				});
			}
		}




		string[] Chapters = { "HELLO!" , "CACHE FOLDERS", 
			/*"INTRO", 
#if !EMX_H_LITE
            "HIGHLIGHTER",
#endif
            "OBJECT MENU", "HEADER", "RIGHT BAR", "COMPONENTS ICONS", 
#if !EMX_H_LITE
            "EXTERNAL MODS", 
#endif
            "SEARCH", "CACHE", "SNAP" };
		string[] Icons = {  "", "" , "WELCOME_HI",
#if !EMX_H_LITE
            "WELCOME_01_HIGH",
#endif
            "WELCOME_02_RC_MENU", "WELCOME_03_HEADER", "WELCOME_04_RIGHTBAR", "WELCOME_05_COMP_ICONS", 
#if !EMX_H_LITE
            "WELCOME_06_EXTERNAL",
#endif
            "WELCOME_07_SEARCH", "WELCOME_08_CACHE", "WELCOME_09_SNAP" 
			*/
			};
		GUIStyle _label;
		GUIStyle label {
			get {
				if ( _label == null )
				{
					_label = new GUIStyle( GUI.skin.label );
					_label.fontSize += 2;
					_label.fontStyle = FontStyle.Bold;
				}

				return _label;
			}
		}
		void LegacyButton( string t, int ind )
		{
			var R = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f), GUILayout.Width(buttonW));
			R.width *= 0.8f;
			/*R.x += 16 * t.Count( c => c == '/' );
            if ( t.IndexOf( '/' ) != -1 ) t = t.Substring( t.LastIndexOf( '/' ) + 1 );
            t = "└ " + t;*/
			if ( GUI.Button( R, "", emptyStyle ) )
			{
				currentWelcomeChapter = ind;
				scrollPos = Vector2.zero;
			}

			if ( ind == currentWelcomeChapter && Event.current.type == EventType.Repaint )
			{
				GUI.skin.box.Draw( R, true, true, true, true );
				var r = R;
				r.x += r.width;
				r.width = 3;
				EditorGUI.DrawRect( r, Color.red );
			}
			GUI.Label( R, t, label );
			EditorGUIUtility.AddCursorRect( R, MouseCursor.Link );
		}

		static int currentWelcomeChapter = 0;

		Action<Icons.GetTextureObject> ac;
		public void SubscribeOnDestroy( Action<Icons.GetTextureObject> ac )
		{
			this.ac = ac;
		}

		private void OnDestroy()
		{
			if ( ac != null ) ac( this );
		}
		GUIStyle bs;
		//  Rect lastPosition;
		private void OnGUI()
		{

			if ( Root.TemperaryPluginDisabled ) return;

			scrollPos.y = Mathf.RoundToInt( scrollPos.y );


			//if ( !Root.p[ 0 ].par_e.ENABLE_ALL )
			//{
			//    Close();
			//    return;
			//}


			GUILayout.Label( "Quick hierarchy style settings:", label );

			var lastRect =(int)GUILayoutUtility.GetLastRect().width - 16;
			cr = lastRect;
			Draw.RESET( this );

			GUILayout.BeginHorizontal();
			var H = 50;
			Draw.TOG_TIT( "Background Decorations", "WELCOME_BACKGROUND", rov: EditorGUILayout.GetControlRect( GUILayout.Height( H ) ), EnableRed: false );
			Draw.TOG_TIT( "Child Lines", "WELCOME_CHILD", rov: EditorGUILayout.GetControlRect( GUILayout.Height( H ) ), EnableRed: false );
			Draw.TOG_TIT( "Right Mods Fading", "WELCOME_FILL_RIGHT", rov: EditorGUILayout.GetControlRect( GUILayout.Height( H ) ), EnableRed: false );
			GUILayout.EndHorizontal();
			Draw.HRx1( EditorGUILayout.GetControlRect() );
			GUILayout.Space( 20 );
			// GUILayout.Label( "Tips:" );

			cr = buttonW;
			Draw.RESET( this );

			GUILayout.BeginHorizontal();

			GUILayout.BeginVertical( GUILayout.Width( buttonW ) );



			GUILayout.FlexibleSpace();
			GUILayout.Label( Root.HierarchyPro + ":" );
			GUILayout.Space( 6 );
			for ( int i = 0; i < Chapters.Length; i++ )
			{
				LegacyButton( "-   " + Chapters[ i ], i );
				if ( i == 1 )
				{
					GUILayout.Space( 16 );
					//GUILayout.Label( "Old Help 1.0 (online wiki 2.0 better):" );
					GUILayout.Space( 6 );
				}
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();

			cr = lastRect - buttonW;
			Draw.RESET( this );

			GUILayout.BeginVertical();

			//  GUILayout.Label( Chapters[currentWelcomeChapter] );
			//   lastPosition = GUILayoutUtility.GetLastRect();
			// if ( !t.ContainsKey( currentWelcomeChapter ) )
			// {
			//     var res = new List<Texture2D>();
			//     var p = 0;
			//     while ( true )
			//     {
			//         var path = Adapter.HierAdapter.PluginInternalFolder + "/Documentations/Welcome Screen/Hierarchy-PRO---Welcome-Screen---S0" + currentWelcomeChapter + parts[p++] + ".png";
			//         var load = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
			//         if ( !load ) break;
			//         res.Add( load );
			//     }
			//
			//     t.Add( currentWelcomeChapter, res );
			// }

			scrollPos = GUILayout.BeginScrollView( scrollPos );
			//for ( int i = 0 ; i < t[ currentWelcomeChapter ].Count ; i++ ) drawTexture( t[ currentWelcomeChapter ][ i ] );

			if ( currentWelcomeChapter == 0 )
			{

				GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();

				string P = "-  ";
				if ( bs == null )
				{
					bs = new GUIStyle( GUI.skin.button );
					bs.alignment = TextAnchor.MiddleLeft;
				}

				GUILayout.Label( P + @"Hi! If you are new, let me introduce some little tips:", label );
				GUILayout.Space( 20 );
				Draw.HRx05( EditorGUILayout.GetControlRect() );
				GUILayout.Space( 20 );
				GUILayout.Label( P + @"  How to open hierarchy settings from top menu: ""Tools/"""+ Root.HierarchyPro+@"/.. ( at the bottom )
            There another ways to open settings from hierarchy modules context menus." );
				GUILayout.Space( 20 );
				GUILayout.Label( P + @"In the settings the first red button ""Main Settings"" includes main settings of " + Root.HierarchyPro + " asset." );
				GUILayout.Space( 20 );
				GUILayout.Label( P + @"You can disable any module (it will remove all mods events from editor loops), 
            You can also use ""HardDisableMods.cs"" if settings doesn't work." );
				GUILayout.Space( 20 );
				GUILayout.Label( P + @"In the settings at the bottom you can open ""Welcome Screen"" again"+
            //""Welcome Screen"" includes legacy pics and description of different parts of " + Root.HierarchyPro + @".
            "You may visit our website, you can find wiki page to learn more about this asset." );
				GUILayout.Space( 20 );
				GUILayout.Label( P + @"By default, asset located in the ‘Assets/Plugins/HierarchyPro’ folder.
            But you can move asset to any another folder manually.");
				GUILayout.Space( 20 );

				GUILayout.Label( P +@"Note that!
            All cache is stored separately from the scenes, no any data will be saved in the scene, 
            Asset creates only temporarily session data, for current editor session only.
            You can safely copy your scene to anyone, who don't have this asset without any problems.
            ( to get more information about how ‘Cache’ works, check the ‘Cache’ section on the https://emem.store/wiki )" );
				/*GUILayout.Label( P + @"You can move Hierarchy to another folder, but not into Editor, mono scripts cannot be placed into Editor,
but all monobehaviour of " + Root.HierarchyPro + @" are marked as ""Don't Save"" and contain the ""#if UNITY_EDITOR"" condition
so it will not be included in your build or saved in your scene, all of these objects are removed when the editor is closed
and you can send your 'Scene' to those who haven't the " + Root.HierarchyPro +"." );*/
				GUILayout.Space( 20 );


				GUILayout.Label( P + @"Learn more about this assets:" );
				if ( GUI.Button( Draw.R2, Draw.CONT( "         - Online Documentation: [ https://emem.store/wiki ]" ), bs ) ) Application.OpenURL( "https://emem.store/wiki" );
				EditorGUIUtility.AddCursorRect( Draw.last, MouseCursor.Link );

				//GUILayout.Label( P + @"HierarchyPro Extended.pdf - includes old docs, you can find better quality on the emem.store site." );
				//
				GUILayout.Space( 20 );
				Draw.HRx05( EditorGUILayout.GetControlRect() );
				GUILayout.Space( 20 );
				GUILayout.Label( @"Try these quick presets, if you are new (line height/ indents, background):" );

				GUILayout.BeginHorizontal();
				if ( GUILayout.Button( "Revert Back" ) ) Root.p[ 0 ].par_e.WELCOME_PRESETS = 0;
				if ( GUILayout.Button( "Preset 1" ) ) Root.p[ 0 ].par_e.WELCOME_PRESETS = 1;
				if ( GUILayout.Button( "Preset 2" ) ) Root.p[ 0 ].par_e.WELCOME_PRESETS = 2;
				if ( GUILayout.Button( "Preset 3" ) ) Root.p[ 0 ].par_e.WELCOME_PRESETS = 3;
				GUILayout.EndHorizontal();

				GUILayout.Space( 20 );
				Draw.HRx05( EditorGUILayout.GetControlRect() );

				GUILayout.Space( 20 );


				GUILayout.Label( @"You can also try to switch quick toggles at the top of this window, 
all these settings you can find in the hierarchy settings." );
				GUILayout.Label( "Thank you!", label );

				GUILayout.Space( 20 );

				Draw.TOG_TIT( "Show 'Welcome Screen' for every new projects", "WELCOME_SHOW_IN_EVERY_PROJECTS", rov: EditorGUILayout.GetControlRect( GUILayout.Height( H ) ), EnableRed: false );

				GUILayout.EndVertical();
				GUILayout.Space( 20 );
				GUILayout.EndHorizontal();


			}
			else if ( currentWelcomeChapter == 1 )
			{
				//
				//GUILayout.Space( 20 );
				GUI.Label( Draw.R, "Yay, you can change cache folder right now, or in the settings later:" );
				// Draw.TOG_TIT( "Yay, you can change cache folder now, or in the settings later" , EnableRed: false );
				Draw.HRx05( EditorGUILayout.GetControlRect() );
				//GUILayout.TOG( ":" );

				// cr = (int)Draw.last.width;

				SETGUI_AboutCache.DRAW_CACHE_BUTTONS_WITH_ENABLER( this );
			}
			else
			{
				//var texture = Root.icons.GetHelpTexture(Icons[currentWelcomeChapter ], 1, this);
				//var dif = Mathf.Clamp((position.width - buttonW) / texture.width, 0.01f, 1);
				//var rect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.RoundToInt(texture.height * dif * 0.95f)));
				//rect.height += 2;
				//var color = Color.white;
				//Root.p[ 0 ].gl._DrawTexture_ForExternalWindow( rect, texture, ref color );

			}


			GUILayout.EndScrollView();

			GUILayout.EndHorizontal();
			GUILayout.EndVertical();


			GUILayout.Space( 20 );

			if ( GUILayout.Button( "Select " +Root.HierarchyPro + " Settings", GUILayout.Height( 50 ) ) )
			{
				Settings.MainSettingsEnabler_Window.Select<Settings.MainSettingsEnabler_Window>( forceSelect: true );
			}
			Draw.HRx1( EditorGUILayout.GetControlRect() );

			/* GUILayout.Space(10);
             if (GUILayout.Button("Close", GUILayout.Height(50)))
             {
                 Close();
             }*/
		}

		public int ID()
		{
			return GetInstanceID();
		}
		int cr;
		public int? currentWidth()
		{
			return cr;
		}

		[SettingsProvider]
		public static SettingsProvider CreateMyCustomSettingsProvider()
		{
			// First parameter is the path in the Settings window.
			// Second parameter is the scope of this setting: it only appears in the Project Settings window.
			//var provider = new SettingsProvider("Project/MyCustomIMGUISettings", SettingsScope.Project)
			var provider = new SettingsProvider("Preferences/HierarchyPro", SettingsScope.User)
		{
				// By default the last token of the path is used as display name if no label is provided.
				label = Root.HierarchyPro,
				// Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
				guiHandler = (searchContext) =>
			{

				GUILayout.Space(20);
				GUILayout.Label(Root.HierarchyPro + Root.VER);
				GUILayout.Space(20);
				if ( GUILayout.Button( "Open Welcome Screen", GUILayout.Height( 50 ) ) )
				{
					WelcomeScreen.Init( null );
				}
				EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), MouseCursor.Link );
				GUILayout.Space(20);
				if ( GUILayout.Button( "Select " +Root.HierarchyPro + " Settings", GUILayout.Height( 50 ) ) )
				{
					Settings.MainSettingsEnabler_Window.Select<Settings.MainSettingsEnabler_Window>(forceSelect: true );
				}
				EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), MouseCursor.Link );
			},

				// Populate the search keywords to enable smart search filtering and label highlighting:
				keywords = new HashSet<string>(new[] { "Hierarchy", "Pro" })
			};

			return provider;
		}




		// public bool Shown { get { return true; } }
	}


}
