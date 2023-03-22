using System;
using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor
{

	partial class EditorSettingsAdapter
	{




		internal partial class HighlighterSettings
		{

			internal string KEY = "";
			internal int pluginID;
			internal string pluginName;
			internal HighlighterSettings( string key, int pluginID )
			{
				KEY = key;
				this.pluginID = pluginID;
				pluginName = p.pluginname;
			}

			PluginInstance p { get { return Root.p[ 0 ]; } }
			EditorSettingsAdapter s { get { return Root.p[ 0 ].par_e; } }


			internal bool USE_AUTO_HIGHLIGHTER_MOD {
				get { return pluginID == 0 ? p.par_e.USE_HIERARCHY_AUTO_HIGHLIGHTER_MOD : p.par_e.USE_PROJECT_AUTO_HIGHLIGHTER_MOD; }
				set { if ( pluginID == 0 ) p.par_e.USE_HIERARCHY_AUTO_HIGHLIGHTER_MOD = value; else p.par_e.USE_PROJECT_AUTO_HIGHLIGHTER_MOD = value; }
			}
			internal bool USE_MANUAL_HIGHLIGHTER_MOD {
				get { return pluginID == 0 ? p.par_e.USE_HIERARCHY_MANUAL_HIGHLIGHTER_MOD : p.par_e.USE_PROJECT_MANUAL_HIGHLIGHTER_MOD; }
				set { if ( pluginID == 0 ) p.par_e.USE_HIERARCHY_MANUAL_HIGHLIGHTER_MOD = value; else p.par_e.USE_PROJECT_MANUAL_HIGHLIGHTER_MOD = value; }
			}
			internal bool USE_CUSTOM_PRESETS_MOD {
				get { return pluginID == 0 ? p.par_e.USE_CUSTOM_PRESETS_MOD : false; }
				set { if ( pluginID == 0 ) p.par_e.USE_CUSTOM_PRESETS_MOD = value; }
			}



			//AUTO HIGHLIHGER

			internal bool AUTOHIGHLIGHTER_USE_DEFAULT_FILTERS {
				get {
					return s.GET( KEY + "_AUTOHIGHLIGHTER_USE_DEFAULT_FILTERS", EditorPrefs.GetBool( "EMX/AUTOHIGHLIGHTER_USE_DEFAULT_FILTERS", true ) );
				}
				set {
					EditorPrefs.SetBool( "EMX/AUTOHIGHLIGHTER_USE_DEFAULT_FILTERS", value );
					var r = AUTOHIGHLIGHTER_USE_DEFAULT_FILTERS; s.SET( KEY + "_AUTOHIGHLIGHTER_USE_DEFAULT_FILTERS", value );
					p.RESET_DRAWSTACK( pluginID )
						;
				}
			}


			//AUTO HIGHLIGHER


			internal bool HIGHLIGHTER_DRAW_ICONS_SHADOW { get { return s.GET( KEY + "_HIGHLIGHTER_DRAW_ICONS_SHADOW", true ); } set { var r = HIGHLIGHTER_DRAW_ICONS_SHADOW; s.SET( KEY + "_HIGHLIGHTER_DRAW_ICONS_SHADOW", value ); p.RESET_DRAWSTACK( pluginID ); } }


			// 0 - Label ; 1 - Fold ; 2 - Align Left
			//   internal bool OVERRIDE_OBJECT_DEFAULT_ICON_SIZE { get { return s.GET("OVERRIDE_OBJECT_DEFAULT_ICON_SIZE", false ); } set { var r = qwe; s.SET( "OVERRIDE_OBJECT_DEFAULT_ICON_SIZE", value ); } }
			//   internal int OBJECT_DEFAULT_ICON_SIZE { get { return s.GET("OBJECT_DEFAULT_ICON_SIZE", Tools.singleLineHeight ); } set { var r = qwe; s.SET( "OBJECT_DEFAULT_ICON_SIZE", value ); } }
			internal int HIGHLIGHTER_CUSTOM_ICON_LOCATION { get { return s.GET( KEY + "_CUSTOM_ICON_LOCATION", 2 ); } set { var r = HIGHLIGHTER_CUSTOM_ICON_LOCATION; s.SET( KEY + "_CUSTOM_ICON_LOCATION", value ); p.RESET_DRAWSTACK( pluginID ); } }
			internal bool HIGHLIGHTER_SHOW_PREFAB_ICON { get { return s.GET( KEY + "_SHOW_PREFAB_ICON", false ); } set { var r = HIGHLIGHTER_SHOW_PREFAB_ICON; s.SET( KEY + "_SHOW_PREFAB_ICON", value ); p.RESET_DRAWSTACK( pluginID ); } }


			// 0 - Hide / 1 - Left Narrow ; 1 - Left Wide ; 2 - Icon
			internal int HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION { get { return s.GET( KEY + "_HIERARCHY_BUTTON_LOCATION", pluginID == 0 ? 1 : 3 ); } set { var r = HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION; s.SET( KEY + "_HIERARCHY_BUTTON_LOCATION", value ); p.RESET_DRAWSTACK( pluginID ); } }
			// 0 - Hide / 1 - Show Window / 2 - Show Always
			internal int HIGHLIGHTER_HIERARCHY_DRAW_BUTTON_RECTMARKER { get { return s.GET( KEY + "_HIERARCHY_DRAW_BUTTON_RECTMARKER", 2 ); } set { var r = HIGHLIGHTER_HIERARCHY_DRAW_BUTTON_RECTMARKER; s.SET( KEY + "_HIERARCHY_DRAW_BUTTON_RECTMARKER", value ); p.RESET_DRAWSTACK( pluginID ); } }
			internal int HIGHLIGHTER_HIERARCHY_BUTTON_RECTMARKER_SIZE { get { return s.GET( KEY + "_HIERARCHY_BUTTON_RECTMARKER_SIZE", 3 ); } set { var r = HIGHLIGHTER_HIERARCHY_BUTTON_RECTMARKER_SIZE; s.SET( KEY + "_HIERARCHY_BUTTON_RECTMARKER_SIZE", value ); p.RESET_DRAWSTACK( pluginID ); } }
			// 0 - every line separately / 1 - group close lines
			internal int HIGHLIGHTER_GROUPING_CHILD_MODE { get { return s.GET( KEY + "_GROUPING_CHILD_MODE", 1 ); } set { var r = HIGHLIGHTER_GROUPING_CHILD_MODE; s.SET( KEY + "_GROUPING_CHILD_MODE", value ); p.RESET_DRAWSTACK( pluginID ); } }
			internal bool HIGHLIGHTER_CHANGE_BUTTON_CURSOR { get { return s.GET( KEY + "_CHANGE_CURSOR", true ); } set { var r = HIGHLIGHTER_CHANGE_BUTTON_CURSOR; s.SET( KEY + "_CHANGE_CURSOR", value ); p.RESET_DRAWSTACK( pluginID ); } }



			internal bool HIGHLIGHTER_DRAW_ICON_IF_CUSTOM_ASIGNED { get { return s.GET( KEY + "_DRAW_ICON_IF_CUSTOM_ASIGNED", false ); } set { var r = HIGHLIGHTER_DRAW_ICON_IF_CUSTOM_ASIGNED; s.SET( KEY + "_DRAW_ICON_IF_CUSTOM_ASIGNED", value ); p.RESET_DRAWSTACK( pluginID ); } }



			internal float HIGHLIGHTER_COLOR_OPACITY { get { return s.GET( KEY + "_COLOR_OPACITY", 1f ); } set { var r = HIGHLIGHTER_COLOR_OPACITY; s.SET( KEY + "_COLOR_OPACITY", value ); p.RESET_DRAWSTACK( pluginID ); } }
			internal int HIGHLIGHTER_USE_SPECUAL_SHADER_TYPE { get { return s.GET( KEY + "_USE_SPECUAL_SHADER_TYPE", 0 ); } set { var r = HIGHLIGHTER_USE_SPECUAL_SHADER_TYPE; s.SET( KEY + "_USE_SPECUAL_SHADER_TYPE", value ); p.RESET_DRAWSTACK( pluginID ); } }
			internal bool HIGHLIGHTER_USE_SPECUAL_SHADER { get { return s.GET( KEY + "_USE_SPECUAL_SHADER", true ); } set { var r = HIGHLIGHTER_USE_SPECUAL_SHADER; s.SET( KEY + "_USE_SPECUAL_SHADER", value ); p.RESET_DRAWSTACK( pluginID ); } }



			//internal int HIGHLIGHTER_DEFAULT_ICON_SIZE { get { return s.GET(KEY + "_DEFAULT_ICON_SIZE", 16); } set { var r = qwe; s.SET(KEY + "_DEFAULT_ICON_SIZE", value); p.RESET_DRAWSTACK(pluginID); } }


			internal int HIGHLIGHTER_BGCOLOR_PADDING {
				get {
					if ( HIGHLIGHTER_GROUPING_CHILD_MODE == 1 ) return 0;
					return s.GET( KEY + "_BGCOLOR_PADDING", 0 );
				}
				set {
					if ( HIGHLIGHTER_GROUPING_CHILD_MODE == 1 ) return;
					var r = HIGHLIGHTER_BGCOLOR_PADDING; s.SET( KEY + "_BGCOLOR_PADDING", value ); p.RESET_DRAWSTACK( pluginID );
				}
			}
			internal int HIGHLIGHTER_TEXTURE_GROW {
				get {
					if ( HIGHLIGHTER_GROUPING_CHILD_MODE != 1 ) return 0;
					return s.GET( KEY + "_TEXTURE_GROW", 0 );
				}
				set {
					if ( HIGHLIGHTER_GROUPING_CHILD_MODE != 1 ) return;
					var r = HIGHLIGHTER_TEXTURE_GROW; s.SET( KEY + "_TEXTURE_GROW", value ); p.RESET_DRAWSTACK( pluginID );
				}
			}


			internal int HIGHLIGHTER_TEXTURE_STYLE { get { return s.GET( KEY + "_TEXTURE_STYLE", 3 ); } set { var r = HIGHLIGHTER_TEXTURE_STYLE; s.SET( KEY + "_TEXTURE_STYLE", value ); p.RESET_DRAWSTACK( pluginID ); } }
			internal int HIGHLIGHTER_TEXTURE_BORDER {
				get {
					if ( HIGHLIGHTER_TEXTURE_STYLE == 0 ) return 0;
					return s.GET( KEY + "_TEXTURE_BORDER", 10 );
				}
				set {
					if ( HIGHLIGHTER_TEXTURE_STYLE == 0 ) return;
					var r = HIGHLIGHTER_TEXTURE_BORDER; s.SET( KEY + "_TEXTURE_BORDER", value ); p.RESET_DRAWSTACK( pluginID );
				}
			}
			internal bool HIGHLIGHTER_TEXTURE_BORDER_ALLOW { get { return HIGHLIGHTER_TEXTURE_STYLE != 0; } }

			//bool? _HIGHLIGHTER_USE_LABEL_OFFSET;
			//internal bool HIGHLIGHTER_USE_LABEL_OFFSET
			//{
			//	get { return _HIGHLIGHTER_USE_LABEL_OFFSET ?? (_HIGHLIGHTER_USE_LABEL_OFFSET = HIGHLIGHTER_TEXTURE_GUID == "19b7d3f9eb031ad4a9d63d48600cb49b").Value; }
			//}
			
			internal bool HIGHLIGHTER_TEXTURE_GUID_ALLOW { get { return HIGHLIGHTER_TEXTURE_STYLE == 3; } }
			internal string HIGHLIGHTER_TEXTURE_GUID {
				get { return s.GET( KEY + "_EXTERNAL_TEXTURE_GUID", "587af57de0784184ba0824f938affe58" ); } //3378a9f961119764984803df786fee6b
																											//get { return s.GET( KEY + "_EXTERNAL_TEXTURE_GUID", "3378a9f961119764984803df786fee6b" ); } //3378a9f961119764984803df786fee6b
																											//6a1045120676a7345b3e63d9d8893f38
				set {
					var r = HIGHLIGHTER_TEXTURE_GUID; s.SET( KEY + "_EXTERNAL_TEXTURE_GUID", value );
					//_HIGHLIGHTER_USE_LABEL_OFFSET = null;
					p.RESET_DRAWSTACK( pluginID );
				}
			}


			internal int HIGHLIGHTER_LEFT_OVERLAP {
				get {
					if ( UnityVersion.UNITY_CURRENT_VERSION < UnityVersion.UNITY_2019_VERSION ) return 0;
					return s.GET( KEY + "_LEFT_OVERLAP", 1 );
				}
				set {
					var r = HIGHLIGHTER_LEFT_OVERLAP; s.SET( KEY + "_LEFT_OVERLAP", value );
					p.RESET_DRAWSTACK( pluginID );
				}
			}
			// "587af57de0784184ba0824f938affe58"

			internal bool DO_FOLD_INVERSION_TOGGLE_ALLOW {
				get {
					if ( HIGHLIGHTER_TEXTURE_STYLE == 3 && (HIGHLIGHTER_TEXTURE_GUID == "1c7f36910e76f8846b8900c17609639d" || HIGHLIGHTER_TEXTURE_GUID == "2bfbda4e08d99ad4b831956e4ed84967") ) return false;
					return true;
				}
			}
			internal bool DO_FOLD_INVERSION {
				get {
					if ( !DO_FOLD_INVERSION_TOGGLE_ALLOW ) return false;
					return s.GET( KEY + "_DO_FOLD_INVERSION", true );
				}
				set { var r = DO_FOLD_INVERSION; s.SET( KEY + "_DO_FOLD_INVERSION", value ); p.RESET_DRAWSTACK( pluginID ); }
			}

			bool HIghlighterExternalTexture_init;
			Texture2D _HIghlighterExternalTexture;
			internal Texture2D HIghlighterExternalTexture {
				get {
					if ( !HIghlighterExternalTexture_init )
					{
						HIghlighterExternalTexture_init = true;
						if ( !string.IsNullOrEmpty( HIGHLIGHTER_TEXTURE_GUID ) )
						{
							var path = AssetDatabase.GUIDToAssetPath(HIGHLIGHTER_TEXTURE_GUID);
							if ( !string.IsNullOrEmpty( path ) ) _HIghlighterExternalTexture = AssetDatabase.LoadAssetAtPath<Texture>( path ) as Texture2D;
						}
					}
					return _HIghlighterExternalTexture;
				}

				set {
					if ( value != HIghlighterExternalTexture )
					{
						_HIghlighterExternalTexture = value;
						if ( !value ) HIGHLIGHTER_TEXTURE_GUID = "";
						else
						{
							var path = AssetDatabase.GetAssetPath(value);
							if ( !string.IsNullOrEmpty( path ) )
							{
								var guid = AssetDatabase.AssetPathToGUID(path);
								if ( !string.IsNullOrEmpty( guid ) ) HIGHLIGHTER_TEXTURE_GUID = guid;
								else HIGHLIGHTER_TEXTURE_GUID = "";
							}
							else HIGHLIGHTER_TEXTURE_GUID = "";
						}
						if ( HIGHLIGHTER_TEXTURE_GUID == "" ) _HIghlighterExternalTexture = null;
						p.RESET_DRAWSTACK( pluginID );
					}
				}
			}
			GUIStyle __BG_TEXTURE_STYLE;

			internal GUIStyle BG_TEXTURE_STYLE {
				get { return __BG_TEXTURE_STYLE ?? (__BG_TEXTURE_STYLE = new GUIStyle()); }
			}

			Texture2D __BG_TEXTURE_TEXT;
			Texture2D __BG_TEXTURE_BOX;
			Rect tr;

			internal Texture2D BG_TEXTURE {
				get {
					if ( HIGHLIGHTER_TEXTURE_STYLE == 0 ) return Texture2D.whiteTexture;
					if ( HIGHLIGHTER_TEXTURE_STYLE == 1 )
					{
						if ( __BG_TEXTURE_BOX == null )
							// __BG_TEXTURE_BOX = Settings.Draw.s( "preToolbar" ).normal.background ?? Settings.Draw.s( "preToolbar" ).normal.scaledBackgrounds[ 0 ];
							__BG_TEXTURE_BOX = EditorStyles.toolbarButton.onNormal.background ?? EditorStyles.toolbarButton.onNormal.scaledBackgrounds[ 0 ];
						//__BG_TEXTURE_BOX = EditorStyles.toolbarButton.active.background ?? Root.p[0].GET_SKIN().box.normal.scaledBackgrounds[0];
						return __BG_TEXTURE_BOX;
					}
					if ( HIGHLIGHTER_TEXTURE_STYLE == 2 )
					{
						if ( __BG_TEXTURE_TEXT == null ) __BG_TEXTURE_TEXT = Root.p[ 0 ].GET_SKIN().textArea.normal.background ?? Root.p[ 0 ].GET_SKIN().textArea.normal.scaledBackgrounds[ 0 ];
						return __BG_TEXTURE_TEXT;
					}
					if ( HIGHLIGHTER_TEXTURE_STYLE == 3 ) return HIghlighterExternalTexture;
					return null;
				}
			}





			internal class SHADER_HELPER
			{
				string keyMat /*, keyShader*/;
				HighlighterSettings adapter;

				internal SHADER_HELPER( string key, HighlighterSettings adapter )
				{
					this.adapter = adapter;
					this.keyMat = key + "-Material";
					// this.keyShader = key + "-Shader";
				}

				internal Func<string> GET_SHADER_GUID;
				internal Func<string> GET_SHADER_LOCAL_PATH;
				internal Action<string> SET_SHADER_GUID;

				Shader oldSHader;
				Material _HIghlighterExternalMaterial;



				int matID {
					get {
						return SessionState.GetInt( keyMat, -1 );
						// return adapter.s.GET( keyMat, -1 );
					}
					set {
						if ( matID == value ) return;
						SessionState.SetInt( keyMat, value );
						//var r = matID; adapter. s.SET( keyMat, value );
					}
				}


				internal Material ExternalMaterialReference {
					get {
						if ( oldSHader != ExternalShaderReference )
						{
							oldSHader = ExternalShaderReference;
							if ( oldSHader == null ) _HIghlighterExternalMaterial = null;
							else _HIghlighterExternalMaterial = new Material( _HIghlighterExternalShader );
							matID = _HIghlighterExternalMaterial == null ? -1 : _HIghlighterExternalMaterial.GetInstanceID();
						}
						if ( !_HIghlighterExternalMaterial && matID != -1 )
						{
							_HIghlighterExternalMaterial = EditorUtility.InstanceIDToObject( matID ) as Material;
							if ( !_HIghlighterExternalMaterial && ExternalShaderReference )
							{
								oldSHader = null;
								return ExternalMaterialReference;
							}
						}
						return _HIghlighterExternalMaterial;
					}
				}


				bool HIghlighterExternalShader_init;
				Shader _HIghlighterExternalShader;

				internal Shader ExternalShaderReference {
					get {
						if ( !HIghlighterExternalShader_init || _HIghlighterExternalShader == null )
						{
							HIghlighterExternalShader_init = true;

							if ( !string.IsNullOrEmpty( GET_SHADER_GUID() ) )
							{
								var path = AssetDatabase.GUIDToAssetPath(GET_SHADER_GUID());
								if ( string.IsNullOrEmpty( path ) && GET_SHADER_LOCAL_PATH != null ) path = Folders.PluginInternalFolder + GET_SHADER_LOCAL_PATH();
								if ( !string.IsNullOrEmpty( path ) )
								{
									_HIghlighterExternalShader = AssetDatabase.LoadAssetAtPath<Shader>( path ) as Shader;
								}
							}
						}

						return _HIghlighterExternalShader;
					}

					set {
						if ( value != ExternalShaderReference )
						{
							_HIghlighterExternalShader = value;

							if ( !value )
							{
								SET_SHADER_GUID( "" );
							}

							else
							{
								var path = AssetDatabase.GetAssetPath(value);

								if ( !string.IsNullOrEmpty( path ) )
								{
									var guid = AssetDatabase.AssetPathToGUID(path);

									if ( !string.IsNullOrEmpty( guid ) )
									{
										SET_SHADER_GUID( guid );
									}

									else SET_SHADER_GUID( "" );
								}

								else SET_SHADER_GUID( "" );
							}

							if ( GET_SHADER_GUID() == "" ) _HIghlighterExternalShader = null;
						}
					}
				}
			}

			
			internal SHADER_HELPER _DEFAULT_SHADER_SHADER;
			internal SHADER_HELPER DEFAULT_SHADER_SHADER {
				get {
					if ( _DEFAULT_SHADER_SHADER == null )
					{
						_DEFAULT_SHADER_SHADER = new SHADER_HELPER( KEY + "_DEFAULT_SHADER_SHADER", this ) {
							SET_SHADER_GUID = ( guid ) => { },
							GET_SHADER_GUID = () => { return "70c76382e3a8a0e4f9f719883a135eff"; },
							GET_SHADER_LOCAL_PATH = () => { return "/Editor/Materials/Highlighter - Default GUI Shader.shader"; }
						};
					}

					return _DEFAULT_SHADER_SHADER;
				}
			}
			internal Material HIghlighterExternalMaterialNormal {
				get { return SHADER_A.ExternalMaterialReference; }
			}
			internal Material HIghlighterExternalMaterial {
				get { return HIGHLIGHTER_USE_SPECUAL_SHADER_TYPE == 0 ? SHADER_A.ExternalMaterialReference : SHADER_B.ExternalMaterialReference; }
			}
			internal string HIGHLIGHTER_SHADER_GUID_MAIN { get { return s.GET( KEY + "_SHADER_GUID_MAIN", "830e0b361750b98468ce6493b692d717" ); } set { var r = HIGHLIGHTER_SHADER_GUID_MAIN; s.SET( KEY + "_SHADER_GUID_MAIN", value ); } }
			internal string HIGHLIGHTER_SHADER_GUID_ADD { get { return s.GET( KEY + "_SHADER_GUID_ADD", "12ace602f83e8b941a0cec6ee38c1a79" ); } set { var r = HIGHLIGHTER_SHADER_GUID_ADD; s.SET( KEY + "_SHADER_GUID_ADD", value ); } }



			SHADER_HELPER _SHADER_A, _SHADER_B;

			internal SHADER_HELPER SHADER_A {
				get {
					if ( _SHADER_A == null )
					{
						_SHADER_A = new SHADER_HELPER( KEY + "_SHADER_A", this ) {
							SET_SHADER_GUID = ( guid ) => { HIGHLIGHTER_SHADER_GUID_MAIN = guid; },
							GET_SHADER_GUID = () => { return HIGHLIGHTER_SHADER_GUID_MAIN; },
							GET_SHADER_LOCAL_PATH = () => { return "/Editor/Materials/Textures for background/Highlighter - Neon Background.shader"; }
						};
					}

					return _SHADER_A;
				}
			}

			internal SHADER_HELPER SHADER_B {
				get {
					if ( _SHADER_B == null )
					{
						_SHADER_B = new SHADER_HELPER( KEY + "_SHADER_B", this ) {
							SET_SHADER_GUID = ( guid ) => { HIGHLIGHTER_SHADER_GUID_ADD = guid; },
							GET_SHADER_GUID = () => { return HIGHLIGHTER_SHADER_GUID_ADD; },
							GET_SHADER_LOCAL_PATH = () => { return "/Editor/Materials/Textures for background/Highlighter - Neon Background Soft Additive.shader"; }
						};
					}

					return _SHADER_B;
				}
			}
		}


	}
}
