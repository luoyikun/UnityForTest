
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using EMX.HierarchyPlugin.Editor.Settings;

namespace EMX.HierarchyPlugin.Editor
{


	class LegacyWindowsCheck2019_2
	{
		internal static void CLAMP_WINDOS_RECT(EditorWindow w )
		{
#if UNITY_2020_1_OR_NEWER
			return;
#else
			var p = w.position;
			var c = false;
			if (p.x < 0 )
			{
				c = true;
				p.x = 0;
			}
			if (p.y < 0 )
			{
				c = true;
				p.y = 0;
			}
		
			if (p.x + p.width > Screen.currentResolution.width )
			{
				c = true;
				p.x -= p.x + p.width - Screen.currentResolution.width; 
			}
			if ( p.y + p.height > Screen.currentResolution.width )
			{
				c = true;
				p.y -= p.y + p.height - Screen.currentResolution.height;
			}
			if ( c ) w.position = p;
#endif
		}

		internal static void CLAMP_WINDOS_RECT( ref Rect p )
		{
#if UNITY_2020_1_OR_NEWER
			return;
#else
			if ( p.x < 0 )
			{
				p.x = 0;
			}
			if ( p.y < 0 )
			{
				p.y = 0;
			}

			if ( p.x + p.width > Screen.currentResolution.width )
			{
				p.x -= p.x + p.width - Screen.currentResolution.width;
			}
			if ( p.y + p.height > Screen.currentResolution.width )
			{
				p.y -= p.y + p.height - Screen.currentResolution.height;
			}
#endif
		}
	}


	class SettingsScreen : EditorWindow, IRepaint
	{

		const int buttonW = 240;

		public static void Init( Rect? __source, Type so_type, bool internalNavigation ) // var w = GetWindow<WelcomeScreen>( "Post Presets - Welcome Screen" , true, Params.WindowType,)
		{
			_source = __source;
			//EditorApplication.update += showw;
			showw( so_type, internalNavigation );
		}

		int id = 0;
		public int ID()
		{
			return GetInstanceID() + id;
		}
		const int SHR = 20;
		public int? currentWidth()
		{
			return (int)position.width / 2 - SHR * 2 - 16;
		}

		class TypeSaver
		{
			internal int category;
			Type cached_type;
			internal Type SAVED_TYPE {
				get {
					if ( cached_type != null ) return cached_type;

					var CAT = SessionState.GetString( "EMX|HierarchyPRO|Settings|Selected_" + category, "" );
					var t = CAT;
					if ( t == "" ) return null;
					else
					{
						cached_type = GetType().Assembly.GetType( t );
						//if ( type == null ) type = default_value;
					}
					return cached_type;
				}
				set {
					cached_type = value;
					if ( value == null ) SessionState.SetString( "EMX|HierarchyPRO|Settings|Selected_" + category, "" );
					else SessionState.SetString( "EMX|HierarchyPRO|Settings|Selected_" + category, value.FullName );
				}
			}
		}

		static Dictionary<string, TypeSaver> selected_category_dic = new Dictionary<string, TypeSaver>();
		static void SET_SEL_CAT_SO( string key, Type so_type, bool internalNavigation = true )
		{

			//Debug.Log( key + " 1 " + DRAW_CATEGORIES_IND );

			if ( !internalNavigation && DRAW_CATEGORIES_IND != 0 )
			{
				var pc = CONVERT_TO_CAT_FROM_SO( so_type );
				DRAW_CATEGORIES_IND = pc;
				key = cats_names[ DRAW_CATEGORIES_IND ];
			}


			if ( key == null )
			{
				var pc = CONVERT_TO_CAT_FROM_SO( so_type );
				if ( pc != DRAW_CATEGORIES_IND && DRAW_CATEGORIES_IND != 0 )
					DRAW_CATEGORIES_IND = pc;
				key = cats_names[ DRAW_CATEGORIES_IND ];
			}

			//Debug.Log( key + " 2 " + DRAW_CATEGORIES_IND );

			if ( !selected_category_dic.ContainsKey( key ) ) selected_category_dic.Add( key, new TypeSaver() { category = DRAW_CATEGORIES_IND } );
			selected_category_dic[ key ].SAVED_TYPE = so_type;
		}
		static Type GET_SEL_CAT_SO( string key )
		{
			if ( !selected_category_dic.ContainsKey( key ) ) selected_category_dic.Add( key, new TypeSaver() { category = DRAW_CATEGORIES_IND } );
			//if ( !selected_category_dic.ContainsKey( key ) ) return null;
			return selected_category_dic[ key ].SAVED_TYPE;
		}

		static Rect? _source;
		static void showw( Type so_type, bool internalNavigation )
		{
			var pw = Resources.FindObjectsOfTypeAll<SettingsScreen>().FirstOrDefault();
			if ( pw )
			{
				//cats_names[]
				// pw. winScrollPos.Clear();
				SET_SEL_CAT_SO( null, so_type, internalNavigation );
				//pw.type = t;
				Draw.RESET( pw );
				LegacyWindowsCheck2019_2.CLAMP_WINDOS_RECT( pw );
				pw.Repaint();
				pw.Show();
				return;
			}
			//var w = GetWindow<SettingsScreen>(true, "" + Root.PN + " - Settings", true);
			var w = CreateWindow<SettingsScreen>( "" + Root.HierarchyPro + " - Settings");
			if ( !SessionState.GetBool( Folders.PREFS_PATH + "was settings", false ) )
			{
				var d  = Screen.currentResolution.height / 1080f;
				var source = _source ?? new Rect(0, d * 140, Screen.currentResolution.width, Screen.currentResolution.height - d * 280);
				var thisR = new Rect(0, source.y, buttonW + (Screen.currentResolution.width < 1500 ? 430 : Screen.currentResolution.width < 2100 ? 750 : 1000), Math.Max(source.height,
				Math.Min(Screen.currentResolution.height, 1080) - d *280));
				thisR.x = source.x + source.width / 2 - thisR.width / 2;
				thisR.y = source.y + source.height / 2 - thisR.height / 2;
				w.position = thisR;
				SessionState.SetBool( Folders.PREFS_PATH + "was settings", true );
			}

			SET_SEL_CAT_SO( null, so_type, internalNavigation );
			//w.type = t;
			w.GENERATE_RANDOM();
			// w.winScrollPos.Clear();
			// EditorApplication.update -= showw;
		}

		Vector2 mainScrollPos;
		Dictionary<int, Vector2> winScrollPos = new Dictionary<int, Vector2>();
		//int CAT {
		//    get { return SessionState.GetInt( "EMX|HierarchyPRO|Settings|Category", 0 ); }
		//    set { SessionState.SetInt( "EMX|HierarchyPRO|Settings|Category", value ); }
		//}
		//string DRAW_CATEGORIES_SLOT {
		//	get { return SessionState.GetString( "EMX|HierarchyPRO|Settings|Category", "" ); }
		//	set { SessionState.SetString( "EMX|HierarchyPRO|Settings|Category", value ); }
		//}
		//static int DRAW_CATEGORIES_IND {
		//	get { return SessionState.GetInt( "EMX|HierarchyPRO|Settings|Category", 0 ); }
		//	set { SessionState.SetInt( "EMX|HierarchyPRO|Settings|Category", value ); }
		//}
		static int DRAW_CATEGORIES_IND {
			get { return Root.p[ 0 ].par_e.SETTINGS_FILTER_INDEX; }
			set { Root.p[ 0 ].par_e.SETTINGS_FILTER_INDEX = value; }
		}

		Rect Shrink( Rect r, float v )
		{
			r.x += v;
			r.y += v;
			r.width -= v + v;
			r.height -= v + v;
			return r;
		}

		static      string[] cats_names = {"[ ALL ]","- Main -","- Internal Mods -","- Left Mods -","- Right Mods -","- External Mods -","[ Project Mods ]" };
		static Type GET_DEFAULT_CAT_SO( int type )
		{
			switch ( type )
			{
				//case 0: return typeof( MainSettingsEnabler_Window );
				//case 1: return typeof( MainSettingsEnabler_Window );
				//case 2: return typeof( SETGUI_TopBar );
				//case 3: return typeof( SETGUI_HL_HierarchyManual );
				//case 4: return typeof( SETGUI_SetActive );
				//case 5: return typeof( SETGUI_HyperGraph );
				//case 6: return typeof( SETGUI_ProjectSettings );
				case 0: return typeof( MainSettingsParams_Window );
				case 1: return typeof( MainSettingsParams_Window );
				case 2: return typeof( TB_Window );
#if !EMX_H_LITE
				case 3: return typeof( HLM_Window );
#else
				case 3: return typeof( MainSettingsParams_Window );
#endif
				case 4: return typeof( SA_Window );
#if !EMX_H_LITE
				case 5: return typeof( BB_Window );
#else
				case 5: return typeof( MainSettingsParams_Window );
#endif
				case 6:
#if !EMX_H_LITE
					return typeof( ProjectSettingsParams_Window );
#else
					return typeof( PW_Window );
#endif
				default:
					throw new Exception( "GET default cat win" );
			}
			throw new Exception( "GET default cat win" );
		}
		int? lastCat = null;
		Type GET_CAT_FROM_SO( ref Type type, int cat )
		{
			var default_value = GET_DEFAULT_CAT_SO(cat);

			bool generate_Random = false;
			if ( type != null )
			{
				//Debug.Log( pert + "  " + DRAW_CATEGORIES_IND );

				var pert = CONVERT_TO_CAT_FROM_SO( type );
				if ( DRAW_CATEGORIES_IND != 0 && DRAW_CATEGORIES_IND != pert )
				{
					type = null; generate_Random = true;
				}
			}

			if ( type == null )
			{
				type = default_value;
				generate_Random = true;
			}
			//if ( type == null || type == default_value )
			//{
			//	var t = CAT;
			//	if ( t == "" ) type = default_value;
			//	else
			//	{
			//		type = GetType().Assembly.GetType( t );
			//		if ( type == null ) type = default_value;
			//	}
			//
			//	//Debug.Log( default_value );
			//
			//	GENERATE_RANDOM();
			//}
			if ( type != GET_SEL_CAT_SO( cats_names[ DRAW_CATEGORIES_IND ] ) )
			{
				SET_SEL_CAT_SO( cats_names[ DRAW_CATEGORIES_IND ], type );
				generate_Random = true;
				//CAT = type.FullName;
			}

			if ( !lastCat.HasValue ) lastCat = DRAW_CATEGORIES_IND;
			if ( DRAW_CATEGORIES_IND != lastCat )
			{
				lastCat = DRAW_CATEGORIES_IND;
				generate_Random = true;
			}

			if ( generate_Random )
			{
				GENERATE_RANDOM();
			}

			if ( type == typeof( MainSettingsEnabler_Window ) ) return typeof( SETGUI_MainSettings );
			if ( type == typeof( MainSettingsParams_Window ) ) return typeof( SETGUI_MainSettings );
			if ( type == typeof( AB_Cache ) ) return typeof( SETGUI_AboutCache );
			if ( type == typeof( AB_Extensions ) ) return typeof( SETGUI_AboutExtensions );

			if ( type == typeof( AS_Window ) ) return typeof( SETGUI_Autosave );
			if ( type == typeof( RC_Window ) ) return typeof( SETGUI_RightClickMenu );
			if ( type == typeof( ST_Window ) ) return typeof( SETGUI_Snap );
			if ( type == typeof( TB_Window ) ) return typeof( SETGUI_TopBar );

			if ( type == typeof( IC_Window ) ) return typeof( SETGUI_Icons );
			if ( type == typeof( RM_Window ) ) return typeof( SETGUI_RightBar );
			if ( type == typeof( SA_Window ) ) return typeof( SETGUI_SetActive );

			if ( type == typeof( PW_Window ) ) return typeof( SETGUI_ProjectWindow );


#if !EMX_H_LITE
			if ( type == typeof( HLA_Window ) ) return typeof( SETGUI_HL_HierarchyAuto );
			if ( type == typeof( HLM_Window ) ) return typeof( SETGUI_HL_HierarchyManual );
			if ( type == typeof( PLA_Window ) ) return typeof( SETGUI_HL_ProjectAuto );
			if ( type == typeof( PLM_Window ) ) return typeof( SETGUI_HL_ProjectManual );

			if ( type == typeof( PK_Window ) ) return typeof( SETGUI_PlayModeKeeper );

			if ( type == typeof( EW_Window ) ) return typeof( SETGUI_ExternalWindows );
			if ( type == typeof( BB_Window ) ) return typeof( SETGUI_BottomBar );
			if ( type == typeof( BO_Window ) ) return typeof( SETGUI_BookmarkObjectsQuick );
			if ( type == typeof( BF_Window ) ) return typeof( SETGUI_ProjectFolders );
			if ( type == typeof( LO_Window ) ) return typeof( SETGUI_SelectionHistory );
			if ( type == typeof( HE_Window ) ) return typeof( SETGUI_ExpandedTreeItems );
			if ( type == typeof( HG_Window ) ) return typeof( SETGUI_HyperGraph );
			if ( type == typeof( LS_Window ) ) return typeof( SETGUI_LastScenes );

			if ( type == typeof( PM_Window ) ) return typeof( SETGUI_PresetsManager );
			if ( type == typeof( ProjectSettingsParams_Window ) ) return typeof( SETGUI_ProjectSettings );
#endif

			if ( back != 0 ) throw new Exception( "no window " + type );

			Debug.LogWarning( "no window " + type );
			type = null;
			back++;
			return GET_CAT_FROM_SO( ref type, cat );
		}
		int back = 0;

		static int CONVERT_TO_CAT_FROM_SO( Type type )
		{

			if ( type == typeof( MainSettingsEnabler_Window ) ) return 1;
			if ( type == typeof( MainSettingsParams_Window ) ) return 1;

			if ( type == typeof( AB_Cache ) ) return 1;
			if ( type == typeof( AB_Extensions ) ) return 1;

			if ( type == typeof( TB_Window ) ) return 2;//ypeof( SETGUI_TopBar );
			if ( type == typeof( RC_Window ) ) return 2;//ypeof( SETGUI_RightClickMenu );
			if ( type == typeof( AS_Window ) ) return 2;//ypeof( SETGUI_Autosave );
			if ( type == typeof( ST_Window ) ) return 2;//typeof( SETGUI_Snap );


			if ( type == typeof( IC_Window ) ) return 4;//ypeof( SETGUI_Icons );
			if ( type == typeof( RM_Window ) ) return 4;//ypeof( SETGUI_RightBar );
			if ( type == typeof( SA_Window ) ) return 4;//typeof( SETGUI_SetActive );

			if ( type == typeof( PW_Window ) ) return 6;// typeof( SETGUI_ProjectWindow );


#if !EMX_H_LITE
			if ( type == typeof( HLA_Window ) ) return 3; //ypeof( SETGUI_HL_HierarchyAuto );
			if ( type == typeof( HLM_Window ) ) return 3; //ypeof( SETGUI_HL_HierarchyManual );
			if ( type == typeof( PLA_Window ) ) return 3; //ypeof( SETGUI_HL_ProjectAuto );
			if ( type == typeof( PLM_Window ) ) return 3; //typeof( SETGUI_HL_ProjectManual );
			if ( type == typeof( PM_Window ) ) return 3;

			if ( type == typeof( PK_Window ) ) return 4;

			if ( type == typeof( EW_Window ) ) return 5;//  ExternalWindows );
			if ( type == typeof( BB_Window ) ) return 5;//  BottomBar );
			if ( type == typeof( BO_Window ) ) return 5;//  BookmarkObjectsQuick );
			if ( type == typeof( BF_Window ) ) return 5;//  ProjectFolders );
			if ( type == typeof( LO_Window ) ) return 5;//  SelectionHistory );
			if ( type == typeof( HE_Window ) ) return 5;//  ExpandedTreeItems );
			if ( type == typeof( HG_Window ) ) return 5;//  HyperGraph );
			if ( type == typeof( LS_Window ) ) return 5;//  LastScenes );

			if ( type == typeof( ProjectSettingsParams_Window ) ) return 6;
#endif

			throw new Exception( "ASDAS" + " " + type );
		}



		object[] ob=new object[1];
		Dictionary<int, MethodInfo> _mc = new Dictionary<int, MethodInfo>();
		void DrawGUI( Type t )
		{
			var i = t.FullName.GetHashCode();
			if ( !_mc.ContainsKey( i ) )
			{
				_mc.Add( i, t.GetMethod( "_GUI", (BindingFlags)(~0) ) );
			}
			ob[ 0 ] = this;

			if ( _mc[ i ] == null )
			{
				Debug.Log( t.FullName );
			}

			using ( MainRoot.ENABLE( this ).USE( "ENABLE_ALL", 0 ) )
				_mc[ i ].Invoke( null, ob );
		}
		bool changedCat = true;

		void GENERATE_RANDOM()
		{
			start_line = UnityEngine.Random.Range( 0.2f, 0.5f );
			end_line = UnityEngine.Random.Range( 1.7f, 2.1f );
			var RR = 5;
			for ( int i = 0; i < p.Length; i++ )
			{
				rofp[ i ].x = UnityEngine.Random.Range( -RR, RR );
				rofp[ i ].y = UnityEngine.Random.Range( -RR, RR );
				rofp2[ i ].x = UnityEngine.Random.Range( -RR, RR );
				rofp2[ i ].y = UnityEngine.Random.Range( -RR, RR );
			}
			changedCat = true;
		}

		Vector3[] rofp2 = new Vector3[24];
		Vector3[] rofp = new Vector3[24];
		Vector3[] p = new Vector3[24];
		float start_line, end_line;
		internal static Color32 dad { get { return Root.p[ 0 ].par_e.COLORS_FOR_CATEGORIES; } }
		Vector3 tp1, tp2;

		private void OnGUI()
		{
			if ( Root.TemperaryPluginDisabled ) return;



			// if ( EditorApplication.isCompiling ) return;

			//  GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
			// var r1= EditorGUILayout.GetControlRect(GUILayout.Height(1),GUILayout.ExpandWidth(true));
			//var r1= EditorGUILayout.GetControlRect();
			//var r2= EditorGUILayout.GetControlRect();
			//var r3= EditorGUILayout.GetControlRect();
			// GUILayout.EndHorizontal();
			var r1 = position;
			r1.x = 0;
			r1.y = 0;


			var header_r = r1;
			header_r.height = EditorGUIUtility.singleLineHeight * 2;
			//var cats = new[]{"All","Main","Internal","Left","Right","External","Other"};

#if !EMX_H_LITE
			var cats_en = new[]{true,true,true,true,true,true,true};
#else
			var cats_en = new[]{true,true,true,false,true,false,true};
#endif

			var oldT = DRAW_CATEGORIES_IND;
			Draw.TOOLBAR( cats_names, "SETTINGS_FILTER_INDEX", rect: header_r, enabled: cats_en );
			if ( oldT != DRAW_CATEGORIES_IND )
			{
				SETGUI_MODS_ENABLER._WritePos.Clear();
			}
			header_r.y += header_r.height + 8;
			header_r.height = 16;
			Draw.HRx05( header_r );

			if ( Draw.lastActiveToolBarRect.HasValue )
			{
				//EditorGUI.DrawRect( Draw.lastActiveToolBarRect.Value, dad );
			}

			r1.y += header_r.height + header_r.y - 27;
			r1.height -= header_r.height + header_r.y - 25;

			header_r = r1;
			header_r.y += header_r.height - 16 - 11;
			header_r.height = 16;
			Draw.HRx05( header_r );

			//var hr = r1;
			//hr.height = 8;
			//hr.y += SHR / 2;
			//Draw.HRx1( hr );
			//
			//hr.y = r1.y + r1.height - SHR / 2;
			//Draw.HRx1( hr );

			// var hr = Shrink( r1, SHR );
			//if (Event.current.type == EventType.Repaint)
			//Draw.s( "preToolbar" ).Draw( r1, new GUIContent(), false, false, false, false );

			var gg = r1;
			gg = Shrink( gg, SHR );

			r1.width /= 2;
			r1 = Shrink( r1, SHR );
			r1.width += SHR / 2;


			GUILayout.BeginArea( r1 );
			// GUILayout.Label( "ASD" );
			mainScrollPos = GUILayout.BeginScrollView( mainScrollPos );

			// mainScrollPos = GUILayout.BeginScrollView( mainScrollPos );
			var so_type = GET_SEL_CAT_SO(cats_names[DRAW_CATEGORIES_IND]);
			if ( so_type == typeof( MainSettingsEnabler_Window ) ) so_type = typeof( MainSettingsParams_Window );
			back = 0;
			var c = GET_CAT_FROM_SO(ref so_type, DRAW_CATEGORIES_IND);
			SET_SEL_CAT_SO( cats_names[ DRAW_CATEGORIES_IND ], so_type );

			SETGUI_MODS_ENABLER._GUI( this, DRAW_CATEGORIES_IND );

			if ( Event.current.type == EventType.Repaint )
				if ( SETGUI_MODS_ENABLER._WritePos.ContainsKey( so_type ) )
				{
					// var L = 20;
					// var II = L / 2;
					var rect = SETGUI_MODS_ENABLER._WritePos[so_type];




					p[ 0 ] = new Vector3( rect.x, rect.y, 0 );
					p[ 2 ] = new Vector3( rect.x, rect.y + rect.height, 0 );
					p[ 4 ] = new Vector3( rect.x + rect.width, rect.y + rect.height, 0 );
					p[ 6 ] = new Vector3( rect.x + rect.width, rect.y, 0 );
					for ( int i = 0; i < 4; i++ )
						p[ i * 2 + 1 ] = Vector3.Lerp( p[ i * 2 + 0 ], p[ (i * 2 + 2) % 8 ], 0.5f );
					for ( int i = 8; i < p.Length; i++ ) p[ i ] = p[ i - 8 ];

					for ( int i = 0; i < p.Length; i++ )
					{
						p[ i ].x += rofp[ i ].x;
						p[ i ].y += rofp[ i ].y;
					}


					Root.p[ 0 ].gl.GL_BEGIN();
					tp2 = p[ 0 ];
					/// for ( int i = 0; i < L; i++ )
					/// {
					///     tp1 = tp2;
					///     tp2 = GetBezierPoint( p, i / (L - 1f), i / II, 3 );
					///     var dad =  new Color( 155, 125, 55 );
					///     GL.Color( dad );
					///     ALIAS( tp1, tp2, dad );
					///     GL_VERTEX3( tp1 );
					///     GL_VERTEX3( tp2 );
					/// }
					GL.Color( dad );





					for ( float t = start_line; t <= end_line; t += 0.05f )
					{
						var ind = (int)(t*2)*3;
						if ( ind + 4 >= p.Length ) break;
						if ( tp2 == p[ 0 ] ) tp2 = GetPoint( (t * 2f) % 1f, p.Skip( ind ).ToArray() );
						tp1 = tp2;
						tp2 = GetPoint( (t * 2f) % 1f, p.Skip( ind ).ToArray() );
						ALIAS( tp1, tp2, dad );
						GL_VERTEX3( tp1 );
						GL_VERTEX3( tp2 );
					}

					Root.p[ 0 ].gl.GL_END();
				}

			GUILayout.EndScrollView();

			GUILayout.EndArea();

			var r2=  r1;
			r2.x += r2.width + SHR;
			//r2.width += r2.width + SHR;
			GUILayout.BeginArea( r2 );

			//Debug.Log( EditorGUIUtility.hotControl );

			var id = c.FullName.GetHashCode();
			var key = Folders.PREFS_PATH + "SettingsScroll/" + id + ' ';

			var scr = new Vector2( SessionState.GetFloat( key + 'x', 0 ), SessionState.GetFloat( key + 'y', 0 ));

			var newScr = GUILayout.BeginScrollView( scr );
			if ( newScr.x != scr.x ) SessionState.SetFloat( key + 'x', newScr.x );
			if ( newScr.y != scr.y ) SessionState.SetFloat( key + 'y', newScr.y );
			if ( !winScrollPos.ContainsKey( id ) ) winScrollPos.Add( id, newScr );
			winScrollPos[ id ] = newScr;



			DrawGUI( c );
			var s = new Rect( 0, 75, currentWidth() ?? 0 - 50, 2 );
			if ( SETGUI_MODS_ENABLER._WritePos.ContainsKey( so_type ) )
				EditorGUI.DrawRect( s, dad );
			/* {
                 var rect = s;
                 rect.height = 40;
                 rect.y -= rect.height - 10;
                 p[ 0 ] = new Vector3( rect.x, rect.y, 0 );
                 p[ 2 ] = new Vector3( rect.x, rect.y + rect.height, 0 );
                 p[ 4 ] = new Vector3( rect.x + rect.width, rect.y + rect.height, 0 );
                 p[ 6 ] = new Vector3( rect.x + rect.width, rect.y, 0 );
                 for ( int i = 0; i < 4; i++ )
                     p[ i * 2 + 1 ] = Vector3.Lerp( p[ i * 2 + 0 ], p[ (i * 2 + 2) % 8 ], 0.5f );
                 for ( int i = 8; i < p.Length; i++ ) p[ i ] = p[ i - 8 ];
                 for ( int i = 0; i < p.Length; i++ )
                 {
                     p[ i ].x += rofp2[ i ].x;
                     p[ i ].y += rofp2[ i ].y;
                 }

                 Root.p[ 0 ].gl.GL_BEGIN();
                 tp2 = p[ 0 ];
                 GL.Color( dad );
                 for ( float t = start_line; t <= end_line; t += 0.05f )
                 {
                     var ind = (int)(t*2)*3;
                     if ( ind + 4 >= p.Length ) break;
                     if ( tp2 == p[ 0 ] ) tp2 = GetPoint( (t * 2f) % 1f, p.Skip( ind ).ToArray() );
                     tp1 = tp2;
                     tp2 = GetPoint( (t * 2f) % 1f, p.Skip( ind ).ToArray() );
                     ALIAS( tp1, tp2, dad );
                     GL_VERTEX3( tp1 );
                     GL_VERTEX3( tp2 );
                 }
                 Root.p[ 0 ].gl.GL_END();
             }*/

			GUILayout.EndScrollView();

			GUILayout.EndArea();

			//var gg = position;
			//gg.x = 0;
			//gg.y = 0;
			//gg = Shrink( gg, SHR );
			gg.x -= SHR;
			gg.width += SHR * 2;
			if ( Event.current.type == EventType.Repaint )
			{
				GUI.BeginClip( gg );
				if ( SETGUI_MODS_ENABLER._WritePos.ContainsKey( so_type ) )
				{
					bool LINE = true;
#pragma warning disable
					var rect = SETGUI_MODS_ENABLER._WritePos[so_type];
					var L = LINE ? 3 : 6; //3
					var D = 0f;
					p[ 0 ] = new Vector3( rect.x + rect.width, rect.y, 0 );
					p[ 0 ].x += -mainScrollPos.x + r1.x - gg.x;
					p[ 0 ].y += -mainScrollPos.y + r1.y - gg.y + 4;
					p[ L ].x = s.x + r2.x - winScrollPos[ id ].x - gg.x;
					p[ L ].y = s.y + r2.y - winScrollPos[ id ].y - gg.y + 1;
					for ( int i = 1; i < L; i++ ) p[ i ] = Vector3.Lerp( p[ 0 ], p[ L ], i / (L - D) );


					if ( LINE )
					{
						for ( int i = 1; i < 3; i++ )
						{
							p[ i ].x += rofp[ i ].x * 5;
							p[ i ].y += rofp[ i ].y * 5;
						}
						p[ 2 ].y = Mathf.Lerp( p[ 2 ].y, p[ 3 ].y, 0.95f );
					}
					else
					{
						for ( int i = 1; i < L; i++ )
						{
							p[ i ].x += rofp[ i ].x * 10 * (1 - i / (L - D));
							p[ i ].y += rofp[ i ].y * 1;
							var ler = i/(L-1f);
							if ( ler < 0.5f )
							{
								p[ i ].y = Mathf.Lerp( p[ 0 ].y, p[ i ].y, i / (L - D) * 2 );
								p[ i ].y = Mathf.Lerp( p[ 0 ].y, p[ i ].y, i / (L - D) * 2 );
							}
							else
							{
								p[ i ].y = Mathf.Lerp( p[ i ].y, p[ L ].y, i / (L - D) * 2 - 1 );
								p[ i ].y = Mathf.Lerp( p[ i ].y, p[ L ].y, i / (L - D) * 2 - 1 );
							}
						}
					}



					//p[ 2 ].y = Mathf.Lerp( p[ 2 ].y, p[ 3 ].y, 0.95f );

					Root.p[ 0 ].gl.GL_BEGIN();
					GL.Color( dad );
					//ALIAS( p[ 6 ], p[ 7 ], dad );
					//GL_VERTEX3( p[ 6 ] );
					//GL_VERTEX3( p[ 7 ] );
					tp2 = p[ 0 ];
					for ( float t = 0; t <= 1f; t += 0.05f )
					{
						if ( LINE )
						{
							tp1 = tp2;
							tp2 = GetPoint( (t), p );
						}
						else
						{
							var ind = (int)(t*4);
							if ( ind + 4 >= p.Length ) break;
							tp1 = tp2;
							tp2 = GetPoint( (t) % 1f, p.Skip( ind ).ToArray() );
						}

						// if ( tp2 == p[ 0 ] ) tp2 = GetBezierPoint(p, (t), 0 );

						//tp2 = GetBezierPoint(p, (t), 0 );

						ALIAS( tp1, tp2, dad );
						GL_VERTEX3( tp1 );
						GL_VERTEX3( tp2 );
					}
					tp1 = tp2;
					tp2 = p[ L ];
					ALIAS( tp1, tp2, dad );
					GL_VERTEX3( tp1 );
					GL_VERTEX3( tp2 );
					Root.p[ 0 ].gl.GL_END();
#pragma warning restore
				}
				GUI.EndClip();
			}


			if ( changedCat && Event.current.type == EventType.Repaint )
			{
				Repaint();
				changedCat = false;
			}
		}

		private void OnEnable()
		{
			//type = null;
			changedCat = true;
			// GENERATE_RANDOM();
			// Repaint();
		}

		protected void GL_VERTEX3( Vector3 r )
		{
			GL.Vertex3( r.x, r.y, 0 );
		}
		void ALIAS( Vector3 p1, Vector3 p2, Color _c )
		{
			var c = _c;
			c.a *= 0.5f;
			do_al( ref c, 0.3f, ref p1, ref p2 );
			c.a *= 0.5f;
			do_al( ref c, 0.5f, ref p1, ref p2 );
			GL.Color( _c );
		}
		void do_al( ref Color c, float D, ref Vector3 p1, ref Vector3 p2 )
		{
			GL.Color( c );
			p1.x -= D;
			p2.x -= D;
			p1.y -= D;
			p2.y -= D;
			GL_VERTEX3( p1 );
			GL_VERTEX3( p2 );
			p1.x += 2 * D;
			p2.x += 2 * D;
			GL_VERTEX3( p1 );
			GL_VERTEX3( p2 );
			p1.y += 2 * D;
			p2.y += 2 * D;
			GL_VERTEX3( p1 );
			GL_VERTEX3( p2 );
			p1.x -= 2 * D;
			p2.x -= 2 * D;
			GL_VERTEX3( p1 );
			GL_VERTEX3( p2 );
		}
		Vector2 m__tb;
		Vector2 GetBezierPoint( Vector3[] BA, float t, int index, int count = 3 )
		{
			if ( count == 1 )
			{
				return BA[ index ];
			}

			var P0 = GetBezierPoint(BA, t, index, count - 1);
			var P1 = GetBezierPoint(BA, t, index + 1, count - 1);
			m__tb.Set( (1 - t) * P0.x + t * P1.x, (1 - t) * P0.y + t * P1.y );
			return m__tb;
		}
		private Vector2 GetPoint( float t, Vector3[] p )
		{
			float cx = 3 * (p[1].x - p[0].x);
			float cy = 3 * (p[1].y - p[0].y);
			float bx = 3 * (p[2].x- p[1].x) - cx;
			float by = 3 * (p[2].y - p[1].y) - cy;
			float ax = p[3].x - p[0].x - cx - bx;
			float ay = p[3].y- p[0].y - cy - by;
			float Cube = t * t * t;
			float Square = t * t;

			float resX = (ax * Cube) + (bx * Square) + (cx * t) + p[0].x;
			float resY = (ay * Cube) + (by * Square) + (cy * t) + p[0].y;

			return new Vector2( resX, resY );
		}

		// public bool Shown { get { return true; } }
	}


}
