using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor
{

	internal enum NewIconTexture { MonoIcons = 0, MonoIcons_Transparent = 1, RightMods = 2 }
	internal class IconData
	{
		internal int id;
		internal Texture2D texture;
		internal Vector2 uv_start;
		internal Vector2 uv_end;
		internal float width, height;

		internal void SetUV( float x, float y, float width, float? height = null )
		{
			this.width = width;
			this.height = height ?? width;
			uv_start.Set( x, y + this.height );
			uv_end.Set( x + this.width, y );
		}
		internal void SetTexture( Texture2D t )
		{
			uv_start.x /= t.width;
			uv_start.y /= t.height;
			uv_end.x /= t.width;
			uv_end.y /= t.height;
			texture = t;
			id = t.GetInstanceID();
		}
	}
	class Icons : ScriptableObject
	{
		public List<Texture2D> icons_old = null;
		public List<Texture2D> externalmods_old = null;
		public List<Texture2D> icons_new = null;
		public List<Texture2D> icons_help = null;
		public List<UnityEngine.Object> example_folders = null;

		[NonSerialized]
		Texture2D[] icons_help_huge = new Texture2D[3];

		[ContextMenu( "ClearIconsCache" )]
		void ClearCache()
		{
			_GetIconDataFromTexture.Clear();
			_newIcons.Clear();
		}

		[InitializeOnLoadMethod]
		static void ASD()
		{
			bool was = false;
			EditorApplication.update += () => {
				if ( was ) return;
				var d= Resources.FindObjectsOfTypeAll<Icons>().FirstOrDefault();
				if ( !d ) return;
				was = true;
				//d.CheckFolder();
				_newIcons.Clear();
			};
		}

		[ContextMenu( "CheckIconsFolder" )]
		void CheckFolder()
		{
			foreach ( var a in new[] { icons_old, externalmods_old, icons_new, icons_help } )
				foreach ( var item in a )
					if ( item && !AssetDatabase.GetAssetPath( item ).StartsWith( "Assets/EMX/" + Root.PN_FOLDER + "/Editor/Icons", StringComparison.OrdinalIgnoreCase ) )
					{
						Debug.LogError( AssetDatabase.GetAssetPath( item ) );
						return;
					}
		}
		static Dictionary<int, IconData> _GetIconDataFromTexture = new Dictionary<int, IconData>();
		static Vector2 vs = new Vector2(0, 1);
		static Vector2 ve = new Vector2(1, 0);
		internal static IconData GetIconDataFromTexture( Texture2D t )
		{
			//var _t = (int)NewIconTexture.RightMods;
			//if (!_GetIconDataFromTexture.ContainsKey(_t)) _GetIconDataFromTexture.Add(_t, new Dictionary<string, IconData>());
			if ( !_GetIconDataFromTexture.TryGetValue( t.GetInstanceID(), out _getNewIcon ) ) _GetIconDataFromTexture.Add( t.GetInstanceID(), _getNewIcon = new IconData() {
				id = t.GetInstanceID(),
				texture = t,
				width = t.width,
				height = t.height,
				uv_start = vs,
				uv_end = ve
			} );
			return _getNewIcon;
		}


		static Dictionary<int, Dictionary<string, IconData>> _newIcons = new Dictionary<int, Dictionary<string, IconData>>();
		static IconData _getNewIcon;
		internal IconData GetNewIcon( NewIconTexture t, ref string key )
		{
			var _t = (int)t;
			if ( !_newIcons.ContainsKey( _t ) ) _newIcons.Add( _t, new Dictionary<string, IconData>() );
			if ( !_newIcons[ _t ].TryGetValue( key, out _getNewIcon ) ) _newIcons[ _t ].Add( key, _getNewIcon = _getNewiconData( t, ref key ) );
			return _getNewIcon;
		}
		string mono_blank = "MONO_BLANK";
		Dictionary<char, int> char_to_index = new string[] { "ABCDEFGHIJKLMNOPQRSTUVWXYZ" }.Select(a => a + a.ToLower())
			.First().ToCharArray().Select((c, i) => new { c, i }).ToDictionary(c => c.c, i => i.i % 26);
		IconData _getNewiconData( NewIconTexture t, ref string s )
		{

			var res = new IconData();
			switch ( t )
			{
				case NewIconTexture.MonoIcons_Transparent:
				case NewIconTexture.MonoIcons:
					{

						var res_t = icons_new[t == NewIconTexture.MonoIcons ? 1 : 2];
						if ( !res_t )
						{
							Debug.LogWarning( "Hierarchy cannot find '" + s + "' NewiconData NewIconTexture.MonoIcons texture. Please check 'Editor/Icons/IconsArray.asset'" );
							res.SetTexture( Texture2D.whiteTexture );
							return res;
						}
						var S15 = 15;
						var H = res_t.height - S15;
						switch ( s )
						{
							case "MONO_BLANK": res.SetUV( 0 * S15, H - 0 * S15, S15 ); break;
							case "MONO_1": res.SetUV( 1 * S15, H - 0 * S15, S15 ); break;
							case "MONO_2": res.SetUV( 2 * S15, H - 0 * S15, S15 ); break;
							case "MONO_3": res.SetUV( 3 * S15, H - 0 * S15, S15 ); break;
							case "MONO_4": res.SetUV( 4 * S15, H - 0 * S15, S15 ); break;
							case "MONO_5": res.SetUV( 5 * S15, H - 0 * S15, S15 ); break;
							case "MONO_6": res.SetUV( 6 * S15, H - 0 * S15, S15 ); break;
							case "MONO_7": res.SetUV( 7 * S15, H - 0 * S15, S15 ); break;
							case "MONO_8": res.SetUV( 8 * S15, H - 0 * S15, S15 ); break;
							case "MONO_9": res.SetUV( 9 * S15, H - 0 * S15, S15 ); break;
							case "MONO_-": res.SetUV( 10 * S15, H - 0 * S15, S15 ); break;
							case "HIPERUI_BUTTONGLOW": res.SetUV( 11 * S15, H - 0 * S15, S15 ); break;
							case "DISABLEHALF": res.SetUV( 12 * S15, H - 0 * S15, S15 ); break;
							case "DISABLE": res.SetUV( 13 * S15, H - 0 * S15, S15 ); break;
							default:
								{
									if ( s.Length > 2 ) throw new Exception( "Get Mono Error - " + s );
									if ( s.Length == 1 )
									{
										if ( !char_to_index.ContainsKey( s[ 0 ] ) ) return _getNewiconData( NewIconTexture.MonoIcons, ref mono_blank );
										res.SetUV( char_to_index[ s[ 0 ] ] * S15, H - 1 * S15, S15 ); break;
									}
									else
									{
										var fir = -1;
										var sec = -1;
										if ( char_to_index.ContainsKey( s[ 0 ] ) ) fir = char_to_index[ s[ 0 ] ];
										if ( char_to_index.ContainsKey( s[ 1 ] ) ) sec = char_to_index[ s[ 1 ] ];
										if ( fir == -1 && sec == -1 ) return _getNewiconData( NewIconTexture.MonoIcons, ref mono_blank );
										if ( fir == -1 ) { res.SetUV( sec * S15, H - 1 * S15, S15 ); break; }
										if ( sec == -1 ) { res.SetUV( fir * S15, H - 1 * S15, S15 ); break; }
										res.SetUV( sec * S15, H - (2 + fir) * S15, S15 );
										break;
									}
								}
						}

						res.SetTexture( res_t );
						return res;
					}



				case NewIconTexture.RightMods:
					{

						var res_t = icons_new[0];
						if ( !res_t )
						{
							Debug.LogWarning( "Hierarchy cannot find '" + s + "' NewiconData NewIconTexture.RightMods texture. Please check 'Editor/Icons/IconsArray.asset'" );
							res.SetTexture( Texture2D.whiteTexture );
							return res;
						}

						switch ( s )
						{

							case "N1_I": res.SetUV( 0 * 16, 64, 16 ); break;
							case "N1": res.SetUV( 0 * 16, 0, 16 ); break;
							case "N2": res.SetUV( 1 * 16, 0, 16 ); break;
							case "N3": res.SetUV( 2 * 16, 0, 16 ); break;
							case "N4": res.SetUV( 3 * 16, 0, 16 ); break;
							case "N5": res.SetUV( 4 * 16, 0, 16 ); break;
							case "N6": res.SetUV( 5 * 16, 0, 16 ); break;
							case "N7": res.SetUV( 6 * 16, 0, 16 ); break;
							case "N8": res.SetUV( 7 * 16, 0, 16 ); break;
							case "N9": res.SetUV( 8 * 16, 0, 16 ); break;
							case "N0": res.SetUV( 9 * 16, 0, 16 ); break;

							case "LOCATOR": res.SetUV( 0 * 16, 16, 16 ); break;
							case "PREF": res.SetUV( 1 * 16, 16, 16 ); break;
							case "AUDIOPLAYLOCK": res.SetUV( 2 * 16, 16, 16 ); break;
							case "AUDIOPLAY": res.SetUV( 3 * 16, 16, 16 ); break;
							case "AUDIOSTOP": res.SetUV( 4 * 16, 16, 16 ); break;
							case "AUDIO": res.SetUV( 5 * 16, 16, 16 ); break;
							case "SETACTIVEPERSONAL": res.SetUV( 6 * 16, 16, 16 ); break;
							case "SETACTIVE": res.SetUV( 7 * 16, 16, 16 ); break;
							case "TRANS": res.SetUV( 8 * 16, 16, 16 ); break;
							case "MARKER": res.SetUV( 9 * 16, 16, 16 ); break;

							case "SETACTIVE_BG_WHITE": res.SetUV( 35, 256-26, 26, 26 ); break;


							case "STORAGE_PASSIVE": res.SetUV( 0 * 16, 32, 16 ); break;
							case "STORAGE_ACTIVE": res.SetUV( 1 * 16, 32, 16 ); break;
							case "STORAGE_NOCOMP": res.SetUV( 2 * 16, 32, 16 ); break;
							case "STORAGE_ONECOMP": res.SetUV( 3 * 16, 32, 16 ); break;
							case "STORAGE_ALLCOMPS": res.SetUV( 4 * 16, 32, 16 ); break;
							case "UNLOCK": res.SetUV( 5 * 16, 32, 16 ); break;
							case "LOCK": res.SetUV( 6 * 16, 32, 16 ); break;
							case "TRI": res.SetUV( 7 * 16, 32, 16 ); break;
							case "PROP": res.SetUV( 8 * 16, 32, 16 ); break;

							//case "": return icons_old[ 1 ];

							case "STORAGE_AUTO": res.SetUV( 0, 48, 23, 9 ); break;
						}

						res.SetTexture( res_t );
						return res;
					}
			}
			throw new Exception();
		}


		/*internal Texture2D GetOldIconWithPersonal( string s )
		{
			var texture = GetOldIcon(s + (EditorGUIUtility.isProSkin ? "" : " PERSONAL"));

			if ( texture==Texture2D.whiteTexture ) texture=GetOldIcon( s );

			return texture;
		}*/

		// internal Texture2D GetIcon( string s )
		//{
		// if ( icons.Count == 0 ) return Texture2D.blackTexture;

		/*    #if TEST
			if (pacl == null)
			{   ICON_WAS_INIT = false;
				if (pacl == null) pacl = Resources.FindObjectsOfTypeAll<PACKER>().FirstOrDefault();
				if (pacl == null) return Texture2D.whiteTexture;
				icons = pacl.icons.ToList();
				ICON_WAS_INIT = true;
			}
			#endif*/


		//  return _getticon( s ) /*?? Texture2D.blackTexture*/;
		//  }


		internal Texture2D LOADING_TEXTURE()
		{
			var index = Mathf.FloorToInt((float)(EditorApplication.timeSinceStartup % 2) * 4) % 8;
			string st0 = "LOADING_0";
			string st1 = "LOADING_1";
			string st2 = "LOADING_2";
			string st3 = "LOADING_3";
			string st4 = "LOADING_4";
			string st5 = "LOADING_5";
			string st6 = "LOADING_6";
			string st7 = "LOADING_7";
			// MonoBehaviour.print(EditorApplication.timeSinceStartup + " " + index);
			switch ( index )
			{
				case 0: return _oldTexture( ref st0 );
				case 1: return _oldTexture( ref st1 );
				case 2: return _oldTexture( ref st2 );
				case 3: return _oldTexture( ref st3 );
				case 4: return _oldTexture( ref st4 );
				case 5: return _oldTexture( ref st5 );
				case 6: return _oldTexture( ref st6 );
				case 7: return _oldTexture( ref st7 );
			}
			throw new Exception( "get LOADING_TEXTURE" );
		}


		//IconData GetOldIcon( string s ) { return GetOldIcon( ref s ); }
		internal IconData GetOldIcon( ref string s )
		{

			var t = _oldTexture(ref s);
			return GetIconDataFromTexture( t );
			// Debug.Log(s);
		}



		Texture2D _oldTexture( ref string s )
		{

			switch ( s )
			{
				//	case "TRI": return icons_old[ 0 ];
				//case "PROP": return icons_old[ 1 ];
				case "AUTOHIDEPIN": return icons_old[ 2 ];
				case "AUTOHIDEFREE": return icons_old[ 3 ];
				case "BUTBLUE": return icons_old[ 4 ];
				case "BUTHOV": return icons_old[ 5 ];
				case "BUTTRANS": return icons_old[ 6 ];
				//case "AUDIO": return icons_old[ 6 ];
				case "SHADOW": return icons_old[ 7 ];
				//	case "UNLOCK": return icons_old[ 2 ];
				///	case "PREF": return icons_old[ 3 ];
				case "LOADING_0": return icons_old[ 8 ];
				case "LOADING_1": return icons_old[ 9 ];
				case "LOADING_2": return icons_old[ 10 ];
				case "LOADING_3": return icons_old[ 11 ];
				case "LOADING_4": return icons_old[ 12 ];
				case "LOADING_5": return icons_old[ 13 ];
				case "LOADING_6": return icons_old[ 14 ];
				case "LOADING_7": return icons_old[ 15 ];

				case "SNAP_NORM": return icons_old[ 16 ];
				case "SNAP_SURF": return icons_old[ 17 ];
				case "SNAP_MAGN": return icons_old[ 18 ];


				case "HYPER_ICON": return icons_old[ 19 ];
				case "LAST_SELECTION_ICON": return icons_old[ 20 ];
				case "LAST_SCENES_ICON": return icons_old[ 21 ];
				case "BOOKMARKS_ICON": return icons_old[ 22 ];
				case "PROJECT_FOLDERS_ICON": return icons_old[ 23 ];
				case "HIER_EXPAND_ICON": return icons_old[ 24 ];

				case "HYPER_ICON PERSONAL": return icons_old[ 19 + 6 ];
				case "LAST_SELECTION_ICON PERSONAL": return icons_old[ 20+ 6 ];
				case "LAST_SCENES_ICON PERSONAL": return icons_old[ 21 + 6];
				case "BOOKMARKS_ICON PERSONAL": return icons_old[ 22 + 6];
				case "PROJECT_FOLDERS_ICON PERSONAL": return icons_old[ 23 + 6];
				case "HIER_EXPAND_ICON PERSONAL": return icons_old[ 24 + 6];
				default:
					{
						Debug.LogWarning( s );
						return Texture2D.whiteTexture;
						//throw new System.Exception(s);
					}

			}
		}




		internal Texture2D GetOldExternalMod( string s )
		{
			var res = _oldExternalMod(ref s);
			if ( !res )
			{
				Debug.LogWarning( "Hierarchy cannot find '" + s + "' ExternalMod texture. Please check 'Editor/Icons/IconsArray.asset'" );
				res = (Texture2D.whiteTexture);
			}
			return res;
		}
		internal Texture2D GetOldExternalMod( ref string s )
		{
			var res = _oldExternalMod(ref s);
			if ( !res )
			{
				Debug.LogWarning( "Hierarchy cannot find '" + s + "' ExternalMod texture. Please check 'Editor/Icons/IconsArray.asset'" );
				res = (Texture2D.whiteTexture);
			}
			return res;
		}
		Texture2D _oldExternalMod( ref string s )
		{

			switch ( s )
			{
				case "HIPERUI_GAMEOBJECT": return externalmods_old[ 0 ];
				case "HIPERUI_LINE_RDTRIANGLE": return externalmods_old[ 1 ];
				case "HIPERUI_BUTTONGLOW": return externalmods_old[ 2 ];
				case "ZOOM_MINUS": return externalmods_old[ 3 ];
				case "ZOOM_PLUS": return externalmods_old[ 4 ];
				case "ZOOM_THUMB": return externalmods_old[ 5 ];
				case "HIPERUI_CLOSE": return externalmods_old[ 6 ];
				case "HIPERGRAPH_DOCK": return externalmods_old[ 7 ];
				case "REFRESH": return externalmods_old[ 8 ];
				case "HIPERUI_INOUT_A": return externalmods_old[ 9 ];
				case "HIPERUI_INOUT_B": return externalmods_old[ 10 ];
				case "HIPERUI_LINE_BLUEGB": return externalmods_old[ 11 ];
				case "HIPERUI_LINE_BLUEGB PERSONAL": return externalmods_old[ 12 ];
				case "HIPERUI_LINE_BOX": return externalmods_old[ 13 ];
				case "HIPERUI_MARKER_BOX": return externalmods_old[ 14 ];
				case "GRID": return externalmods_old[ 15 ];
				case "GRID_PERSONAL": return externalmods_old[ 16 ];

				case "DRAG_BOX": return externalmods_old[ 17 ];
				case "DRAG_LINE": return externalmods_old[ 18 ];
				case "FAVORIT_FOLDERS_ICON": return externalmods_old[ 19 ];
				case "FAVORIT_FOLDERS_ICON OFF": return externalmods_old[ 20 ];
				case "FAVORIT_FILTER_ICON": return externalmods_old[ 21 ];
				case "FAVORIT_LIST_ICON": return externalmods_old[ 22 ];
				case "FAVORIT_LIST_ICON ON": return externalmods_old[ 23 ];
				case "SEPARATOR": return externalmods_old[ 24 ];
				case "STAR": return externalmods_old[ 25 ];

				case "OBJECTCONTENTCOUNT": return externalmods_old[ 26 ];
				case "BOTTOM_SCENE_DOWN": return externalmods_old[ 27 ];
				case "BOTTOM_SCENE_ACTIVE": return externalmods_old[ 28 ];
				case "SCENE": return externalmods_old[ 29 ];
				case "NEW_BOTTOM_HIERARCHY_ICON": return externalmods_old[ 30 ];
				case "FAV": return externalmods_old[ 31 ];
				case "BOTTOM_INFO_DISABLE": return externalmods_old[ 32 ];
				case "BOTTOM_INFO_DISABLE_PERSONAL": return externalmods_old[ 33 ];
				case "BOTTOM_INFO": return externalmods_old[ 34 ];
				case "BOTTOM_INFO_PERSONAL": return externalmods_old[ 35 ];

				case "HIGHLIGHTER_PRESETS_SELECTION": return externalmods_old[ 36 ];
				case "HIGHLIGHTER_PRESETS_HL": return externalmods_old[ 37 ];
				case "HL_GRADIENT": return externalmods_old[ 38 ];
				case "HL_GRADIENT PERSONAL": return externalmods_old[ 39 ];
				case "FOLDER_STARMARK": return externalmods_old[ 40 ];

				case "HIPERGRAPH PERSONAL": return externalmods_old[ 41 ];
				case "HIPERGRAPH": return externalmods_old[ 42 ];

				case "HIPERGRAPH_ACTIVE PERSONAL": return externalmods_old[ 43 ];
				case "HIPERGRAPH_ACTIVE": return externalmods_old[ 44 ];

				case "NEW_BOTTOM_ARROW_DOWN": return externalmods_old[ 45 ];
				case "NEW_BOTTOM_ARROW_UP": return externalmods_old[ 46 ];

				case "NEW_BOTTOM_BUTTON_BOOKMARKS OFF": return externalmods_old[ 47 ];
				case "NEW_BOTTOM_BUTTON_BOOKMARKS PERSONAL OFF": return externalmods_old[ 48 ];
				case "NEW_BOTTOM_BUTTON_BOOKMARKS PERSONAL": return externalmods_old[ 49 ];
				case "NEW_BOTTOM_BUTTON_BOOKMARKS": return externalmods_old[ 50 ];

				case "NEW_BOTTOM_BUTTON_HIERARCHY OFF": return externalmods_old[ 51 ];
				case "NEW_BOTTOM_BUTTON_HIERARCHY PERSONAL OFF": return externalmods_old[ 52 ];
				case "NEW_BOTTOM_BUTTON_HIERARCHY PERSONAL": return externalmods_old[ 53 ];
				case "NEW_BOTTOM_BUTTON_HIERARCHY": return externalmods_old[ 54 ];

				case "NEW_BOTTOM_BUTTON_LAST OFF": return externalmods_old[ 55 ];
				case "NEW_BOTTOM_BUTTON_LAST PERSONAL OFF": return externalmods_old[ 56 ];
				case "NEW_BOTTOM_BUTTON_LAST PERSONAL": return externalmods_old[ 57 ];
				case "NEW_BOTTOM_BUTTON_LAST": return externalmods_old[ 58 ];

				case "NEW_BOTTOM_BUTTON_SCENE OFF": return externalmods_old[ 59 ];
				case "NEW_BOTTOM_BUTTON_SCENE PERSONAL OFF": return externalmods_old[ 60 ];
				case "NEW_BOTTOM_BUTTON_SCENE PERSONAL": return externalmods_old[ 61 ];
				case "NEW_BOTTOM_BUTTON_SCENE": return externalmods_old[ 62 ];


				case "BOTTOM_SETUP_PARENT": return externalmods_old[ 63 ];

				case "NEW": return externalmods_old[ 64 ];
				case "BOTTOMHELP": return externalmods_old[ 65 ];


				// 
				default:
					{
						throw new Exception( s );
					}


			}
		}

		internal interface GetTextureObject
		{
			int GetInstanceID();
			void SubscribeOnDestroy( Action<GetTextureObject> ac );
		}
		private void OnDestroyGetTextureObject( GetTextureObject o )
		{
			_GetHelpTexture.Remove( o.GetInstanceID() );
			if ( _GetHelpTexture.Count == 0 && icons_help_huge[ 1 ] )
			{
				Resources.UnloadAsset( icons_help_huge[ 1 ] );
				//    Destroy( icons_help_huge[ 1 ] );
				if ( Application.isPlaying ) Destroy( icons_help_huge[ 1 ] );
				else DestroyImmediate( icons_help_huge[ 1 ], true );
				//EditorApplication.GarbageCollectUnusedAssets();
			}
			// Resources.UnloadUnusedAssets();

		}

		Dictionary<int,GetTextureObject> _GetHelpTexture = new Dictionary<int, GetTextureObject>();

		internal IconData GetHelpTexture( string s )
		{
			return GetHelpTexture( s, 0, null );
		}
		internal IconData GetHelpTexture( string s, int index, GetTextureObject o )
		{

			if ( index == 1 )
			{
				if ( _GetHelpTexture.Count == 0 )
				{
					//var p = AssetDatabase.GetAssetPath(this);
					//icons_help_huge[ index ] = AssetDatabase.LoadAssetAtPath<Texture2D>( p.Remove( p.LastIndexOf( '/' ) ) + "/Help/hiul_xh_welcome.png" );
					//if ( !icons_help_huge[ index ] )
					//{
					//	Debug.LogWarning( "Hierarchy cannot find hiul_xh_welcome.png HelpTexture texture. Please check 'Editor/Icons/IconsArray.asset'" );
					//}
				}
				if ( !_GetHelpTexture.ContainsKey( o.GetInstanceID() ) )
				{
					o.SubscribeOnDestroy( OnDestroyGetTextureObject );
					_GetHelpTexture.Add( o.GetInstanceID(), o );
				}
			}

			var res = new IconData();
			var res_t = index == 0 ? icons_help[index] : icons_help_huge[index];

			if ( !res_t )
			{
				Debug.LogWarning( "Hierarchy cannot find '" + s + "' HelpTexture texture. Please check 'Editor/Icons/IconsArray.asset'" );
				res.SetTexture( Texture2D.whiteTexture );
				return res;
			}



			//var H = res_t.height;
			switch ( s )
			{
				case "DRAG_LAST": res.SetUV( 0, 0, 431, 92 ); break;//
				case "DRAG_EXPAND": res.SetUV( 431, 0, 431, 92 ); break;//
				case "DRAG_SCENE": res.SetUV( 862, 0, 431, 92 ); break;//
				case "DRAG_BOOK": res.SetUV( 1293, 0, 431, 92 ); break;//

				case "TAP_LAST": res.SetUV( 0, 92, 302, 177 ); break;//
				case "TAP_SCENE": res.SetUV( 302, 92, 369, 160 ); break;//
				case "TAP_BOOK": res.SetUV( 671, 92, 477, 327 ); break;//
				case "TAP_HYPER": res.SetUV( 1148, 92, 334, 206 ); break;//
				case "TAP_EXPAND": res.SetUV( 1482, 92, 398, 168 ); break;//
				case "TAP_FOLDER": res.SetUV( 1148, 92 + 206, 331, 89 ); break;

				case "HELP_FOLDER": res.SetUV( 0, 269, 332, 294 ); break;

				case "HELP_HYPER": res.SetUV( 0, 563, 362, 209 ); break;//
				case "HELP_LAYOUT": res.SetUV( 362, 563, 572, 141 ); break; //
				case "HELP_RIGHTMOD": res.SetUV( 934, 563, 509, 166 ); break;//
				case "HELP_SNAP": res.SetUV( 1443, 563, 377, 338 ); break; //

				case "HELP_CACHE_A": res.SetUV( 0, 772, 373, 193 ); break;
				case "HELP_CACHE_B": res.SetUV( 373, 772, 358, 207 ); break;
				case "HELP_CUSTOM_ICONS_DRAG": res.SetUV( 731, 772, 609, 278 ); break; //
				case "HELP_RIGHT_MENU": res.SetUV( 731, 1050, 499, 224 ); break; //

				case "HELP_SEARCH": res.SetUV( 0, 965, 373, 392 ); break;//

				case "HELP_SETACTIVE": res.SetUV( 0, 1357, 450, 102 ); break; //
				case "HELP_KEEPER": res.SetUV( 450, 1357, 569, 166 ); break; //
				case "HELP_CUSTOM_ICONS_ATT": res.SetUV( 450 + 569, 1357, 337, 44 ); break; //
				case "HELP_HIGHLIGHTER": res.SetUV( 1442, 2048 - 1145, 484, 523 ); break;//
																						 // case "HELP_HIGHLIGHTER": res.SetUV( 1445, 2048 - 1145, 484, 523 ); break;//

				case "IC_MAIN": res.SetUV( 0, 2048 - 40, 20, 40 ); break;//
				case "IC_TB": res.SetUV( 20, 2048 - 40, 20, 40 ); break;//
				case "IC_RC": res.SetUV( 40, 2048 - 40, 20, 40 ); break;//
				case "IC_AS": res.SetUV( 60, 2048 - 40, 20, 40 ); break;//
				case "IC_SM": res.SetUV( 80, 2048 - 40, 20, 40 ); break;//

				case "IC_SA": res.SetUV( 100, 2048 - 40, 20, 40 ); break;//
				case "IC_IC": res.SetUV( 120, 2048 - 40, 20, 40 ); break;//
				case "IC_RM": res.SetUV( 140, 2048 - 40, 20, 40 ); break;//
				case "IC_PV": res.SetUV( 160, 2048 - 40, 20, 40 ); break;//
				case "IC_PM": res.SetUV( 180, 2048 - 40, 20, 40 ); break;//
				case "IC_HG": res.SetUV( 200, 2048 - 40, 20, 40 ); break;//
				case "IC_BO": res.SetUV( 220, 2048 - 40, 20, 40 ); break;//
				case "IC_BF": res.SetUV( 240, 2048 - 40, 20, 40 ); break;//
				case "IC_HO": res.SetUV( 260, 2048 - 40, 20, 40 ); break;//
				case "IC_HS": res.SetUV( 280, 2048 - 40, 20, 40 ); break;//
				case "IC_HE": res.SetUV( 300, 2048 - 40, 20, 40 ); break;//


				case "HL_A": res.SetUV( 320, 2048 - 40, 20, 40 ); break;//
				case "HL_B": res.SetUV( 340, 2048 - 40, 20, 40 ); break;//
				case "HL_C": res.SetUV( 360, 2048 - 40, 20, 40 ); break;//

				case "BB_M": res.SetUV( 380, 2048 - 40, 20, 40 ); break;//

				//	case "MONO_1": res.SetUV(1 * S15, H - 0 * S15, S15); break;
				//	case "MONO_2": res.SetUV(2 * S15, H - 0 * S15, S15); break;






				case "WELCOME_HI": res.SetUV( WEL_W * 0, 8192 - 3300, WEL_W, 3300 ); break;//
				case "WELCOME_01_HIGH": res.SetUV( 1268, 8192 - 5741, WEL_W, 5741 ); break;//
				case "WELCOME_02_RC_MENU": res.SetUV( 2521, 8192 - 6674, WEL_W, 6674 ); break;//
				case "WELCOME_03_HEADER": res.SetUV( WEL_W * 0, 8192 - 3294 - 4889, WEL_W, 4889 ); break;//
				case "WELCOME_04_RIGHTBAR": res.SetUV( 3785, 8192 - 6332, WEL_W, 6332 ); break;//
				case "WELCOME_05_COMP_ICONS": res.SetUV( 2521, 8192 - 2978 - 3731, WEL_W, 3731 ); break;//
				case "WELCOME_06_EXTERNAL": res.SetUV( 5040, 8192 - 5970, WEL_W, 5970 ); break;//
				case "WELCOME_07_SEARCH": res.SetUV( 6336, 8192 - 3942, WEL_W, 3942 ); break;//
				case "WELCOME_08_CACHE": res.SetUV( 6336, 8192 - 3916 - 1888, WEL_W, 1888 ); break;//
				case "WELCOME_09_SNAP": res.SetUV( 6336, 8192 - 5780 - 2409, WEL_W, 2409 ); break;//




				default: throw new Exception( s );
			}

			res.SetTexture( res_t );
			return res;
		}
		int WEL_W = 1267;
	}

}