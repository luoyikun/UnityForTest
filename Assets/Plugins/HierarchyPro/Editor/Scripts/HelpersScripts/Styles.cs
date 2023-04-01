using System;
using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor
{


	internal class RightModsStyles
	{
		static GUISkin GET_SKIN()
		{
			return Root.p[ 0 ].GET_SKIN();
		}
		static EditorSettingsAdapter par_e {
			get {
				return Root.p[ 0 ].par_e;
			}
		}

		static internal float FACTOR_8()
		{
			return FONT_8() / 8f;
		}

		static internal float HALFFACTOR_8()
		{
			return ((FONT_8() - 8) / 3 + 8) / 8f;
		}

		static internal float FACTOR_10()
		{
			return FONT_10() / 10f;
		}


		static internal int FONT_10_RIGHT_MOD_HEADER()
		{
			return Mathf.RoundToInt( par_e.HIER_WIN_SET.RIGHTMOD_HEADER_FONT_SIZE );
		}


		static internal int FONT_8()
		{
			return Mathf.RoundToInt( par_e.HIER_WIN_SET.RIGHTMOD_LABELS_FONT_SIZE ) - 2;
		}
		static internal int FONT_10()
		{
			return Mathf.RoundToInt( par_e.HIER_WIN_SET.RIGHTMOD_LABELS_FONT_SIZE );
		}

		static public int WINDOW_FONT_12() //FILTER DATA HUGE BUTTONS
		{
			return Mathf.RoundToInt( 12 * HALFFACTOR_8() );
		}

		static public int WINDOW_FONT_10() //FILTER DATA HUGE BUTTONS
		{
			return Mathf.RoundToInt( 10 * HALFFACTOR_8() );
		}

		static public int WINDOW_FONT_8() //FILTER DATA HUGE BUTTONS
		{
			return Mathf.RoundToInt( 8 * HALFFACTOR_8() );
		}


		internal static void ClearLabels()
		{
			__STYLE_LABEL_8_WINDOWS_middle = null;
			__STYLE_LABEL_8_right = null;
			__STYLE_LABEL_8_WINDOWS_right = null;
			__STYLE_LABEL_10_middle_clipColored = null;
			__STYLE_LABEL_8_middle_clipColored = null;
			Mods.Mod_Vertices.STYLE_M_BLACKCOLOR = null;
			Mods.Mod_Vertices.STYLE_M_WARMCOLOR = null;
			Mods.Mod_Vertices.STYLE_M_NORMALCOLOR = null;
		}

		static internal GUIStyle __STYLE_LABEL_8_WINDOWS_middle;
		static internal GUIStyle STYLE_LABEL_8_WINDOWS_middle {
			get {
				if ( __STYLE_LABEL_8_WINDOWS_middle == null )
				{
					__STYLE_LABEL_8_WINDOWS_middle = new GUIStyle( GET_SKIN().label );
					__STYLE_LABEL_8_WINDOWS_middle.alignment = TextAnchor.MiddleCenter;
					__STYLE_LABEL_8_WINDOWS_middle.clipping = TextClipping.Overflow;
					__STYLE_LABEL_8_WINDOWS_middle.normal.textColor = par_e.RIGHT_LABELS_COLOR;
				}
				__STYLE_LABEL_8_WINDOWS_middle.fontSize = WINDOW_FONT_8();

				return __STYLE_LABEL_8_WINDOWS_middle;
			}
		}

		static internal GUIStyle __STYLE_LABEL_8_right;
		static internal GUIStyle STYLE_LABEL_8_right {
			get {
				if ( __STYLE_LABEL_8_right == null )
				{
					__STYLE_LABEL_8_right = new GUIStyle( GET_SKIN().label );
					__STYLE_LABEL_8_right.clipping = TextClipping.Overflow;
					__STYLE_LABEL_8_right.padding.right = 3;
					__STYLE_LABEL_8_right.padding.left = 3;
					//__STYLE_LABEL_8_right.margin.right = 3;
					//__STYLE_LABEL_8_right.overflow.left = 3;
					__STYLE_LABEL_8_right.normal.textColor = par_e.RIGHT_LABELS_COLOR;
				}
				if ( __STYLE_LABEL_8_right.alignment != TextAnchor.MiddleRight )
					__STYLE_LABEL_8_right.alignment = TextAnchor.MiddleRight;
				__STYLE_LABEL_8_right.fontSize = FONT_8();

				return __STYLE_LABEL_8_right;
			}
		}

		static internal GUIStyle __STYLE_LABEL_8_WINDOWS_right;
		static internal GUIStyle STYLE_LABEL_8_WINDOWS_right {
			get {
				if ( __STYLE_LABEL_8_WINDOWS_right == null )
				{
					__STYLE_LABEL_8_WINDOWS_right = new GUIStyle( GET_SKIN().label );
					__STYLE_LABEL_8_WINDOWS_right.clipping = TextClipping.Overflow;
					__STYLE_LABEL_8_WINDOWS_right.padding.right = 3;
					__STYLE_LABEL_8_WINDOWS_right.padding.left = 3;
					//__STYLE_LABEL_8_WINDOWS_right.margin.right = 3;
					//__STYLE_LABEL_8_WINDOWS_right.margin.left = 3;
					__STYLE_LABEL_8_WINDOWS_right.normal.textColor = par_e.RIGHT_LABELS_COLOR;
				}
				if ( __STYLE_LABEL_8_WINDOWS_right.alignment != TextAnchor.MiddleRight )
					__STYLE_LABEL_8_WINDOWS_right.alignment = TextAnchor.MiddleRight;
				__STYLE_LABEL_8_WINDOWS_right.fontSize = WINDOW_FONT_8();

				return __STYLE_LABEL_8_WINDOWS_right;
			}
		}
		static internal GUIStyle __STYLE_LABEL_10_middle_clipColored;
		static internal GUIStyle STYLE_LABEL_10_middle_clipColored {
			get {
				if ( __STYLE_LABEL_10_middle_clipColored == null )
				{
					__STYLE_LABEL_10_middle_clipColored = new GUIStyle( Root.p[ 0 ].STYLE_LABEL_10_middle );
					__STYLE_LABEL_10_middle_clipColored.clipping = TextClipping.Clip;
					__STYLE_LABEL_10_middle_clipColored.normal.textColor = par_e.RIGHT_LABELS_COLOR;
				}
				__STYLE_LABEL_10_middle_clipColored.fontSize = WINDOW_FONT_10();

				return __STYLE_LABEL_10_middle_clipColored;
			}
		}

		static  internal GUIStyle __STYLE_LABEL_8_middle_clipColored;
		static internal GUIStyle STYLE_LABEL_8_middle_clipColored {
			get {
				if ( __STYLE_LABEL_8_middle_clipColored == null )
				{
					__STYLE_LABEL_8_middle_clipColored = new GUIStyle( Root.p[ 0 ].STYLE_LABEL_8_middle );
					__STYLE_LABEL_8_middle_clipColored.clipping = TextClipping.Clip;
					__STYLE_LABEL_8_middle_clipColored.normal.textColor = par_e.RIGHT_HEADER_COLOR;
				}
				__STYLE_LABEL_8_middle_clipColored.fontSize = WINDOW_FONT_8();

				return __STYLE_LABEL_8_middle_clipColored;
			}
		}

	}


	internal partial class PluginInstance
	{

		internal GUISkin[] DEFAUL_SKIN = new GUISkin[2];
		internal GUISkin GET_SKIN()
		{
			if ( ReferenceEquals( DEFAUL_SKIN[ pluginID ], null ) ) DEFAUL_SKIN[ pluginID ] = GUI.skin;
			return DEFAUL_SKIN[ pluginID ];
		}






		/*

				internal static void FadeSceneRect( Rect drawRect, float alpha = 0.6f )
				{
					if ( alpha < 0.01f ) return;

					var defColor = GUI.color;
					var c = Colors.SceneColor;
					c.a = alpha;
					GUI.color *= c;

					GUI.DrawTexture( drawRect, EditorGUIUtility.whiteTexture );
					GUI.color = defColor;
				}*/

		internal static void FadeRect( Rect drawRect, float alpha = 0.6f )
		{
			if ( alpha < 0.01f ) return;

			//  var defColor = GUI.color;
			var c = Colors.EditorBGColor;
			c.a = alpha;
			EditorGUI.DrawRect( drawRect, c );
			/* GUI.color *= c;

			 GUI.DrawTexture( drawRect, EditorGUIUtility.whiteTexture );
			 GUI.color = defColor;*/

		}
		static Color tttCC;
		internal void SelectRect( Rect drawRect, float alpha = 1, bool? overrideSelect = null )
		{
			///var defColor = GUI.color;

			if ( overrideSelect.HasValue )
			{
				tttCC = Colors.SelectColorOverrided( overrideSelect.Value );
			}

			else
			{
				tttCC = Colors.SelectColor;
			}

			tttCC.a = alpha;
			EditorGUI.DrawRect( drawRect, tttCC );
			// tttCC.a = (byte)Mathf.Clamp( Mathf.RoundToInt( alpha * 255 ), 0, 255 );
			////GUI.color *= tttCC;
			/*  if (EditorGUIUtility.isProSkin)
                GUI.color *= new Color32( 62, 95, 150, (byte)Mathf.Clamp( Mathf.RoundToInt( alpha * 255 ), 0, 255 ) );
              else GUI.color *= new Color32( 62, 125, 231, (byte)Mathf.Clamp( Mathf.RoundToInt( alpha * 255 ), 0, 255 ) );*/

			///GUI.DrawTexture( drawRect, EditorGUIUtility.whiteTexture );
			////GUI.color = defColor;
		}




		internal GUIStyle __button, __box, __label;

		internal GUIStyle button {
			get {
				return __button ?? (

				  __button = new GUIStyle( STYLE_DEFBUTTON ));
			}
		}

		internal GUIStyle box {
			get {
				if ( __box == null )
				{
					__box = new GUIStyle( GET_SKIN().box );
					__box.clipping = TextClipping.Overflow;
				}
				return __box;
			}
			// get { return __box ?? (__box = new GUIStyle(GET_SKIN().box)); }
		}

		internal GUIStyle label {
			get {
				if ( __label == null )
				{
					__label = new GUIStyle( GET_SKIN().label );
					if ( Window.fonts[ 0 ].HasValue ) __label.fontSize = Window.fonts[ 0 ].Value;
					__label.clipping = TextClipping.Overflow;
				}
				return __label;
			}
		}



		internal float FACTOR_8()
		{
			return FONT_8() / 8f;
		}

		internal float HALFFACTOR_8()
		{
			return ((FONT_8() - 8) / 3 + 8) / 8f;
		}

		internal float FACTOR_10()
		{
			return FONT_10() / 10f;
		}
#if !EMX_H_LITE
		internal int FONT_8_FOR_BOTTOM()
		{
			return Mathf.RoundToInt( WIN_SET.BOTTOMBAR_HEADER_FONT_SIZE ) + 2;
		}
#endif

		internal int FONT_8()
		{
			return Mathf.RoundToInt( WIN_SET.COMMON_LABELS_FONT_SIZE ) - 2;
		}
		internal int FONT_8_FORICONSMODS()
		{
			return Mathf.RoundToInt( WIN_SET.ICONSMOD_LABELS_FONT_SIZE ) - 2;
		}
		internal int FONT_10()
		{
			return Mathf.RoundToInt( WIN_SET.COMMON_LABELS_FONT_SIZE );
		}



		public int WINDOW_FONT_12() //FILTER DATA HUGE BUTTONS
		{
			return Mathf.RoundToInt( 12 * HALFFACTOR_8() );
		}

		public int WINDOW_FONT_10() //FILTER DATA HUGE BUTTONS
		{
			return Mathf.RoundToInt( 10 * HALFFACTOR_8() );
		}

		public int WINDOW_FONT_8() //FILTER DATA HUGE BUTTONS
		{
			return Mathf.RoundToInt( 8 * HALFFACTOR_8() );
		}


		GUIStyle a, b, c;
		bool guichange;
		internal void RestoreGUI()
		{
			if ( !guichange ) return;
			guichange = false;
			__button = a;
			__box = b;
			__label = c;
		}
		internal void ChangeGUI( bool changeColorText = true )
		{
			if ( guichange ) RestoreGUI();
			guichange = true;
			a = __button;
			b = __box;
			c = __label;
			__button = STYLE_DEFBUTTON;
			if ( changeColorText ) __label = STYLE_LABEL_10_COLORED;
			else __label = STYLE_LABEL_10;
			if ( !EditorGUIUtility.isProSkin ) __box = STYLE_DEFBOX;
		}

		internal Texture BoxTexture()
		{
			if ( box.normal.background ) return box.normal.background;
			return box.normal.scaledBackgrounds[ 0 ];
		}


		internal void ResetStyles()
		{
			__labelStyle = null;
			__STYLE_LASTSEL_BUTTON = null;
			__STYLE_HIERSEL_BUTTON = null;
			__STYLE_HIERSEL_PLUS = null;
			__STYLE_InternalBoxStyle = null;
			__STYLE_DEFBUTTON_middle = null;
			__STYLE_DEFBUTTON = null;
			__STYLE_HYPERGRAPH_DEFBUTTON = null;
			__STYLE_LABEL_8 = null;
			__STYLE_LABEL_8_middle = null;
			__STYLE_LABEL_10_middle = null;
			__STYLE_LABEL_10 = null;
			__STYLE_LABEL_10_COLORED = null;
			__STYLE_DEFBOX = null;
			RightModsStyles.ClearLabels();
		}



		internal GUIStyle __labelStyle;
		internal GUIStyle labelStyle {
			get {
				if ( __labelStyle == null ) __labelStyle = new GUIStyle( GET_SKIN().label );

				if ( __labelStyle.alignment != TextAnchor.MiddleLeft ) __labelStyle.alignment = TextAnchor.MiddleLeft;

				//__labelStyle.fontSize = par_e.OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE;
				//if (__labelStyle.fontSize != GET_SKIN().label.fontSize) __labelStyle.fontSize = GET_SKIN().label.fontSize;
				if ( __labelStyle.fontSize != ha.currentDefaultFontSize ) __labelStyle.fontSize = ha.currentDefaultFontSize;

				/*  if ( pluginID == 0 ) 
                  else
                      if ( par.PLUGIN_FONT_AFFECT_HIERARCHYWINDOW ) __labelStyle.fontSize = par.PLUGIN_FONT_SIZE;
                  else __labelStyle.fontSize = lastFontSize;*/

				return __labelStyle;
			}
		}
		internal GUIStyle __STYLE_LASTSEL_BUTTON;
		internal GUIStyle STYLE_LASTSEL_BUTTON {
			get {
				if ( __STYLE_LASTSEL_BUTTON == null )
				{
					__STYLE_LASTSEL_BUTTON = new GUIStyle( STYLE_DEFBUTTON );
					__STYLE_LASTSEL_BUTTON.normal.textColor = new Color32( 20, 20, 20, 255 );
					//                     __STYLE_LASTSEL_BUTTON.padding.top = 5;
					//                     __STYLE_LASTSEL_BUTTON.padding.left = 3;
					//                     __STYLE_LASTSEL_BUTTON.padding.right = 1;
					//                     __STYLE_LASTSEL_BUTTON.padding.bottom = 3;
					__STYLE_LASTSEL_BUTTON.alignment = TextAnchor.MiddleCenter;
					// __STYLE_LASTSEL_BUTTON.fontSize = Mathf.RoundToInt( GET_SKIN().button.fontSize / 1.5f );
					if ( EditorGUIUtility.isProSkin ) __STYLE_LASTSEL_BUTTON.normal.textColor = new Color32( 220, 220, 220, 255 );
				}

				__STYLE_LASTSEL_BUTTON.fontSize = STYLE_LABEL_8.fontSize - 2;
				return __STYLE_LASTSEL_BUTTON;
			}
		}
		internal GUIStyle __STYLE_HIERSEL_BUTTON;
		internal GUIStyle STYLE_HIERSEL_BUTTON {
			get {
				if ( __STYLE_HIERSEL_BUTTON == null )
				{
					__STYLE_HIERSEL_BUTTON = new GUIStyle( STYLE_DEFBUTTON );
					__STYLE_HIERSEL_BUTTON.normal.textColor = new Color32( 20, 20, 20, 255 );
					//                     __STYLE_HIERSEL_BUTTON.padding.top = 5;
					//                     __STYLE_HIERSEL_BUTTON.padding.left = 1;
					//                     __STYLE_HIERSEL_BUTTON.padding.right = 1;
					__STYLE_HIERSEL_BUTTON.alignment = TextAnchor.MiddleCenter;
					__STYLE_HIERSEL_BUTTON.fontStyle = FontStyle.Bold;
					if ( EditorGUIUtility.isProSkin ) __STYLE_HIERSEL_BUTTON.normal.textColor = new Color32( 220, 220, 220, 255 );
				}
				__STYLE_HIERSEL_BUTTON.fontSize = FONT_10();
				return __STYLE_HIERSEL_BUTTON;
			}
		}


		internal GUIStyle __STYLE_HIERSEL_PLUS;
		internal GUIStyle STYLE_HIERSEL_PLUS {
			get {
				if ( __STYLE_HIERSEL_PLUS == null )
				{
					__STYLE_HIERSEL_PLUS = new GUIStyle( STYLE_DEFBUTTON );
					__STYLE_HIERSEL_PLUS.normal.textColor = new Color32( 20, 20, 20, 255 );
					__STYLE_HIERSEL_PLUS.alignment = TextAnchor.MiddleCenter;
					if ( EditorGUIUtility.isProSkin ) __STYLE_HIERSEL_PLUS.normal.textColor = new Color32( 220, 220, 220, 255 );
					// __STYLE_HIERSEL_PLUS.padding.top = 5;
				}
				__STYLE_HIERSEL_PLUS.fontSize = FONT_10();
				return __STYLE_HIERSEL_PLUS;
			}
		}


		internal GUIStyle __STYLE_InternalBoxStyle;
		internal GUIStyle STYLE_InternalBoxStyle {
			get {
				if ( __STYLE_InternalBoxStyle == null )
				{
					__STYLE_InternalBoxStyle = new GUIStyle( GET_SKIN().textArea );
					__STYLE_InternalBoxStyle.alignment = TextAnchor.MiddleCenter;
				}

				return __STYLE_InternalBoxStyle;
			}
		}
		internal GUIStyle __STYLE_DEFBUTTON_middle;
		internal GUIStyle STYLE_DEFBUTTON_middle {
			get {
				if ( __STYLE_DEFBUTTON_middle == null )
				{
					__STYLE_DEFBUTTON_middle = new GUIStyle( STYLE_DEFBUTTON );
					__STYLE_DEFBUTTON_middle.alignment = TextAnchor.MiddleCenter;
				}

				return __STYLE_DEFBUTTON_middle;
			}
		}
		internal GUIStyle __STYLE_DEFBUTTON_right;
		internal GUIStyle STYLE_DEFBUTTON_right {
			get {
				if ( __STYLE_DEFBUTTON_right == null )
				{
					__STYLE_DEFBUTTON_right = new GUIStyle( STYLE_DEFBUTTON );
					__STYLE_DEFBUTTON_right.alignment = TextAnchor.MiddleRight;
				}

				return __STYLE_DEFBUTTON_right;
			}
		}
		[NonSerialized]
		internal GUIStyle __STYLE_DEFBUTTON;
		internal static int s_ButonHash = "Button".GetHashCode();
		internal GUIStyle STYLE_DEFBUTTON2 {
			get {
				if ( __STYLE_DEFBUTTON == null )
				{
					__STYLE_DEFBUTTON = new GUIStyle( GUI.skin.button );
					__STYLE_DEFBUTTON.normal.background = Texture2D.blackTexture;
					__STYLE_DEFBUTTON.hover.background = GetOldIcon( "BUTHOV" ).texture;
					__STYLE_DEFBUTTON.onHover.background = GetOldIcon( "BUTHOV" ).texture;
					__STYLE_DEFBUTTON.focused.background = GetOldIcon( "BUTBLUE" ).texture;
					__STYLE_DEFBUTTON.onFocused.background = GetOldIcon( "BUTBLUE" ).texture;
					__STYLE_DEFBUTTON.active.background = GetOldIcon( "BUTBLUE" ).texture;
					__STYLE_DEFBUTTON.onActive.background = GetOldIcon( "BUTBLUE" ).texture;
					__STYLE_DEFBUTTON.normal.scaledBackgrounds = new[] { Texture2D.blackTexture };
					__STYLE_DEFBUTTON.hover.scaledBackgrounds = new[] { Texture2D.blackTexture };
					__STYLE_DEFBUTTON.focused.scaledBackgrounds = new[] { Texture2D.blackTexture };
					__STYLE_DEFBUTTON.active.scaledBackgrounds = new[] { Texture2D.blackTexture };
					__STYLE_DEFBUTTON.border = new RectOffset( 3, 3, 3, 3 );
					__STYLE_DEFBUTTON.margin = new RectOffset();
					__STYLE_DEFBUTTON.stretchHeight = __STYLE_DEFBUTTON.stretchWidth = true;
					__STYLE_DEFBUTTON.fixedHeight = __STYLE_DEFBUTTON.fixedWidth = 0;
					__STYLE_DEFBUTTON.clipping = TextClipping.Overflow;
					__STYLE_DEFBUTTON.alignment = TextAnchor.MiddleLeft;
					__STYLE_DEFBUTTON.imagePosition = ImagePosition.ImageAbove;
					__STYLE_DEFBUTTON.overflow = new RectOffset();
					__STYLE_DEFBUTTON.contentOffset = Vector2.zero;

				}

				__STYLE_DEFBUTTON.fontSize = FONT_8();
				return __STYLE_DEFBUTTON;
			}
		}
		internal GUIStyle STYLE_DEFBUTTON {
			get {
				if ( __STYLE_DEFBUTTON == null )
				{

					/*	if (Root.p[pluginID].ha.hasShowingPrefabHeader)
                        {
                            __STYLE_DEFBUTTON = new GUIStyle( Root.p[pluginID].ha.hoveredItemBackgroundStyle ?? throw new Exception("ASD"));
                            __STYLE_DEFBUTTON.border = new RectOffset();
                            __STYLE_DEFBUTTON.margin = new RectOffset();
                            __STYLE_DEFBUTTON.stretchHeight = __STYLE_DEFBUTTON.stretchWidth = true;
                            __STYLE_DEFBUTTON.fixedHeight = __STYLE_DEFBUTTON.fixedWidth = 0;
                            __STYLE_DEFBUTTON.clipping = TextClipping.Overflow;
                            __STYLE_DEFBUTTON.alignment = TextAnchor.MiddleLeft;
                            __STYLE_DEFBUTTON.imagePosition = ImagePosition.ImageAbove;
                            __STYLE_DEFBUTTON.overflow = new RectOffset();
                            __STYLE_DEFBUTTON.contentOffset = Vector2.zero;
                            var hov = GetOldIcon("BUTHOV").texture;
                            Texture2D NULL = Texture2D.blackTexture;
                            //__STYLE_DEFBUTTON.onHover.textColor = Color.red;
                            Debug.Log(__STYLE_DEFBUTTON.onNormal.textColor);
                            Debug.Log(__STYLE_DEFBUTTON.onHover.textColor);
                            Debug.Log(__STYLE_DEFBUTTON.onNormal.background);
                            Debug.Log(__STYLE_DEFBUTTON.onNormal.scaledBackgrounds.Length);
                            Debug.Log(__STYLE_DEFBUTTON.onHover.background);
                            Debug.Log(__STYLE_DEFBUTTON.onHover.background);
                            //__STYLE_DEFBUTTON.normal.background = GetOldIcon("BUTTRANS").texture; ;
                            //__STYLE_DEFBUTTON.normal.scaledBackgrounds = new Texture2D[] { GetOldIcon("BUTTRANS").texture };
                        }
                        else*/
					{
						//  __STYLE_DEFBUTTON = new GUIStyle(GUI.skin.button);
						__STYLE_DEFBUTTON = new GUIStyle( GUI.skin.button );

						Texture2D NULL = Texture2D.blackTexture;
						//	Texture2D NULL = GetOldIcon("BUTTRANS").texture;
						var hov = (par_e.USE_HOVER_FOR_BUTTONS) ? GetOldIcon("BUTHOV").texture : NULL;
						var tap = GetOldIcon("BUTBLUE").texture;
						//NULL = tap;
						//hov.name = "btn@2x";
						//tap.name = "btn@2x";

						//__STYLE_DEFBUTTON.normal = new GUIStyleState() { background = NULL };
						//__STYLE_DEFBUTTON.hover = new GUIStyleState() { background = hov };
						//
						//typeof( GUIStyle ).GetMethod( "InternalOnAfterDeserialize", (BindingFlags)~1 ).Invoke( __STYLE_DEFBUTTON, null );
						__STYLE_DEFBUTTON.normal.background = NULL;
						__STYLE_DEFBUTTON.normal.scaledBackgrounds = new Texture2D[] { NULL };
						__STYLE_DEFBUTTON.onNormal.background = NULL;
						__STYLE_DEFBUTTON.onNormal.scaledBackgrounds = new Texture2D[] { NULL };

						__STYLE_DEFBUTTON.onHover.background = hov;
						__STYLE_DEFBUTTON.onHover.scaledBackgrounds = new[] { hov };
						__STYLE_DEFBUTTON.hover.background = hov;
						__STYLE_DEFBUTTON.hover.scaledBackgrounds = new[] { hov };

						__STYLE_DEFBUTTON.active.background = tap;
						__STYLE_DEFBUTTON.active.scaledBackgrounds = new Texture2D[] { tap };
						__STYLE_DEFBUTTON.focused.background = tap;
						__STYLE_DEFBUTTON.focused.scaledBackgrounds = new Texture2D[] { tap };
						__STYLE_DEFBUTTON.onActive.background = tap;
						__STYLE_DEFBUTTON.onActive.scaledBackgrounds = new Texture2D[] { tap };
						__STYLE_DEFBUTTON.onFocused.background = tap;
						__STYLE_DEFBUTTON.onFocused.scaledBackgrounds = new Texture2D[] { tap };


						__STYLE_DEFBUTTON.border = new RectOffset();
						__STYLE_DEFBUTTON.margin = new RectOffset();
						__STYLE_DEFBUTTON.stretchHeight = __STYLE_DEFBUTTON.stretchWidth = true;
						__STYLE_DEFBUTTON.fixedHeight = __STYLE_DEFBUTTON.fixedWidth = 0;
						__STYLE_DEFBUTTON.clipping = TextClipping.Overflow;
						__STYLE_DEFBUTTON.alignment = TextAnchor.MiddleLeft;
						__STYLE_DEFBUTTON.imagePosition = ImagePosition.ImageAbove;
						__STYLE_DEFBUTTON.overflow = new RectOffset();
						__STYLE_DEFBUTTON.contentOffset = Vector2.zero;
					}
					/**/
				}

				__STYLE_DEFBUTTON.fontSize = FONT_8();
				return __STYLE_DEFBUTTON;
			}
		}
		internal GUIStyle __STYLE_HYPERGRAPH_DEFBUTTON;
		internal GUIStyle STYLE_HYPERGRAPH_DEFBUTTON {
			get {
				if ( __STYLE_HYPERGRAPH_DEFBUTTON == null )
				{
					__STYLE_HYPERGRAPH_DEFBUTTON = new GUIStyle( GUI.skin.button );
					__STYLE_HYPERGRAPH_DEFBUTTON.normal.textColor = Color.black;
					__STYLE_HYPERGRAPH_DEFBUTTON.padding.top = 3;
					__STYLE_HYPERGRAPH_DEFBUTTON.padding.left = 3;
					__STYLE_HYPERGRAPH_DEFBUTTON.padding.right = 2;
					__STYLE_HYPERGRAPH_DEFBUTTON.padding.bottom = 3;
				}

				__STYLE_HYPERGRAPH_DEFBUTTON.fontSize = Mathf.RoundToInt( STYLE_DEFBUTTON.fontSize / 1.5f );
				return __STYLE_HYPERGRAPH_DEFBUTTON;
			}
		}

		internal GUIStyle __STYLE_LABEL_8;
		internal GUIStyle STYLE_LABEL_8 {
			get {
				if ( __STYLE_LABEL_8 == null )
				{
					__STYLE_LABEL_8 = new GUIStyle( GET_SKIN().label );
					__STYLE_LABEL_8.alignment = TextAnchor.MiddleLeft;
					__STYLE_LABEL_8.clipping = TextClipping.Overflow;
					__STYLE_LABEL_8.padding.left = 5;
				}
				__STYLE_LABEL_8.fontSize = FONT_8();

				return __STYLE_LABEL_8;
			}
		}
		//internal GUIStyle __STYLE_LABEL_8_WINDOWS;
		//internal GUIStyle STYLE_LABEL_8_WINDOWS {
		//    get {
		//        if ( __STYLE_LABEL_8_WINDOWS == null )
		//        {
		//            __STYLE_LABEL_8_WINDOWS = new GUIStyle( GET_SKIN().label );
		//            __STYLE_LABEL_8_WINDOWS.alignment = TextAnchor.MiddleLeft;
		//            __STYLE_LABEL_8_WINDOWS.clipping = TextClipping.Overflow;
		//            __STYLE_LABEL_8_WINDOWS.padding.left = 5;
		//        }
		//        __STYLE_LABEL_8_WINDOWS.fontSize = WINDOW_FONT_8();
		//
		//        return __STYLE_LABEL_8_WINDOWS;
		//    }
		//}
		//
		internal GUIStyle __STYLE_LABEL_8_middle;
		internal GUIStyle STYLE_LABEL_8_middle {
			get {
				if ( __STYLE_LABEL_8_middle == null )
				{
					__STYLE_LABEL_8_middle = new GUIStyle( GET_SKIN().label );
					__STYLE_LABEL_8_middle.alignment = TextAnchor.MiddleCenter;
					__STYLE_LABEL_8_middle.clipping = TextClipping.Overflow;
				}
				__STYLE_LABEL_8_middle.fontSize = FONT_8();

				return __STYLE_LABEL_8_middle;
			}
		}
		//internal GUIStyle __STYLE_LABEL_8_WINDOWS_middle;
		//internal GUIStyle STYLE_LABEL_8_WINDOWS_middle {
		//    get {
		//        if ( __STYLE_LABEL_8_WINDOWS_middle == null )
		//        {
		//            __STYLE_LABEL_8_WINDOWS_middle = new GUIStyle( GET_SKIN().label );
		//            __STYLE_LABEL_8_WINDOWS_middle.alignment = TextAnchor.MiddleCenter;
		//            __STYLE_LABEL_8_WINDOWS_middle.clipping = TextClipping.Overflow;
		//        }
		//        __STYLE_LABEL_8_WINDOWS_middle.fontSize = WINDOW_FONT_8();
		//
		//        return __STYLE_LABEL_8_WINDOWS_middle;
		//    }
		//}
		//
		//internal GUIStyle __STYLE_LABEL_8_right;
		//internal GUIStyle STYLE_LABEL_8_right {
		//    get {
		//        if ( __STYLE_LABEL_8_right == null )
		//        {
		//            __STYLE_LABEL_8_right = new GUIStyle( GET_SKIN().label );
		//            __STYLE_LABEL_8_right.alignment = TextAnchor.MiddleRight;
		//            __STYLE_LABEL_8_right.clipping = TextClipping.Overflow;
		//            __STYLE_LABEL_8_right.padding.right = 5;
		//        }
		//        __STYLE_LABEL_8_right.fontSize = FONT_8();
		//
		//        return __STYLE_LABEL_8_right;
		//    }
		//}


		// internal GUIStyle __STYLE_LABEL_8_WINDOWS_right;
		// internal GUIStyle STYLE_LABEL_8_WINDOWS_right {
		//     get {
		//         if ( __STYLE_LABEL_8_WINDOWS_right == null )
		//         {
		//             __STYLE_LABEL_8_WINDOWS_right = new GUIStyle( GET_SKIN().label );
		//             __STYLE_LABEL_8_WINDOWS_right.alignment = TextAnchor.MiddleRight;
		//             __STYLE_LABEL_8_WINDOWS_right.clipping = TextClipping.Overflow;
		//             __STYLE_LABEL_8_WINDOWS_right.padding.right = 5;
		//         }
		//         __STYLE_LABEL_8_WINDOWS_right.fontSize = WINDOW_FONT_8();
		//
		//         return __STYLE_LABEL_8_WINDOWS_right;
		//     }
		// }

		// internal GUIStyle __STYLE_LABEL_10_middle_clipColored;
		// internal GUIStyle STYLE_LABEL_10_middle_clipColored {
		//     get {
		//         if ( __STYLE_LABEL_10_middle_clipColored == null )
		//         {
		//             __STYLE_LABEL_10_middle_clipColored = new GUIStyle( STYLE_LABEL_10_middle );
		//             __STYLE_LABEL_10_middle_clipColored.clipping = TextClipping.Clip;
		//         }
		//
		//         return __STYLE_LABEL_10_middle_clipColored;
		//     }
		// }
		//internal GUIStyle __STYLE_LABEL_8_middle_clipColored;
		//internal GUIStyle STYLE_LABEL_8_middle_clipColored {
		//    get {
		//        if ( __STYLE_LABEL_8_middle_clipColored == null )
		//        {
		//            __STYLE_LABEL_8_middle_clipColored = new GUIStyle( STYLE_LABEL_8_middle );
		//            __STYLE_LABEL_8_middle_clipColored.clipping = TextClipping.Clip;
		//        }
		//
		//        return __STYLE_LABEL_8_middle_clipColored;
		//    }
		//}


		internal GUIStyle __STYLE_LABEL_10_middle;
		internal GUIStyle STYLE_LABEL_10_middle {
			get {
				if ( __STYLE_LABEL_10_middle == null )
				{
					__STYLE_LABEL_10_middle = new GUIStyle( GET_SKIN().label );
					__STYLE_LABEL_10_middle.alignment = TextAnchor.MiddleCenter;
					__STYLE_LABEL_10_middle.clipping = TextClipping.Overflow;
				}
				__STYLE_LABEL_10_middle.fontSize = FONT_10();

				return __STYLE_LABEL_10_middle;
			}
		}
		internal GUIStyle __STYLE_LABEL_10_middle_clip;
		internal GUIStyle STYLE_LABEL_10_middle_clip {
			get {
				if ( __STYLE_LABEL_10_middle_clip == null )
				{
					__STYLE_LABEL_10_middle_clip = new GUIStyle( STYLE_LABEL_10_middle );
					__STYLE_LABEL_10_middle_clip.clipping = TextClipping.Clip;
				}

				return __STYLE_LABEL_10_middle_clip;
			}
		}



		internal GUIStyle __STYLE_LABEL_10;
		internal GUIStyle STYLE_LABEL_10 {
			get {
				if ( __STYLE_LABEL_10 == null )
				{
					__STYLE_LABEL_10 = new GUIStyle( GET_SKIN().label );
					__STYLE_LABEL_10.alignment = TextAnchor.MiddleLeft;
					__STYLE_LABEL_10.clipping = TextClipping.Overflow;
				}
				__STYLE_LABEL_10.fontSize = FONT_10();

				return __STYLE_LABEL_10;
			}
		}
		//
		internal GUIStyle __STYLE_LABEL_10_COLORED;
		internal GUIStyle STYLE_LABEL_10_COLORED {
			get {
				if ( __STYLE_LABEL_10_COLORED == null )
				{
					__STYLE_LABEL_10_COLORED = new GUIStyle( GET_SKIN().label );
					__STYLE_LABEL_10_COLORED.alignment = TextAnchor.MiddleLeft;
					__STYLE_LABEL_10_COLORED.clipping = TextClipping.Overflow;
					//__STYLE_LABEL_10_COLORED.normal.textColor = par_e.RIGHT_LABELS_COLOR;
				}
				__STYLE_LABEL_10_COLORED.fontSize = FONT_10();

				return __STYLE_LABEL_10_COLORED;
			}
		}
		internal static GUIStyle __STYLE_DEFBOX;
		internal static GUIStyle STYLE_DEFBOX {
			get {
				if ( __STYLE_DEFBOX == null )
				{   /* __STYLE_DEFBOX = new GUIStyle( GET_SKIN().textArea );
				 __STYLE_DEFBOX.alignment = TextAnchor.MiddleCenter;*/

					__STYLE_DEFBOX = GUI.skin.FindStyle( "Tooltip" ) ?? EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).FindStyle( "Tooltip" );

					if ( __STYLE_DEFBOX == null )
					{
						__STYLE_DEFBOX = GUI.skin.box;
					}

					__STYLE_DEFBOX = new GUIStyle( __STYLE_DEFBOX );
					__STYLE_DEFBOX.alignment = TextAnchor.MiddleCenter;
					__STYLE_DEFBOX.clipping = TextClipping.Overflow;
				}

				return __STYLE_DEFBOX;
			}
		}

		/*  internal static GUIStyle m_InspectorTitlebar;
		  internal void HR( Rect R, float HR = 0 )
		  {
			  if ( m_InspectorTitlebar == null )
			  {
				  m_InspectorTitlebar = GUI.skin.FindStyle( "m_InspectorTitlebar" ) ?? EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).FindStyle( "m_InspectorTitlebar" );

				  if ( m_InspectorTitlebar == null )
				  {
					  m_InspectorTitlebar = GUI.skin.box;
				  }
			  }

			  R.height = 1;

			  if ( HR != 0 )
			  {
				  float HRP = HR;
				  HRP -= R.width / 4;
				  R.x -= (HRP - 5);
				  R.width += (HRP - 5) * 2;
			  }

			  R.y -= 8;
			  DrawRect( R, new Color32( 20, 20, 20, 255 ) );

			  // Adapter.LABEL( R , stack[i].text , m_ProgressBarBar );
			  //Adapter.LABEL( R, " ", m_InspectorTitlebar );
		  }
		  */



		GUIContent tc = new GUIContent();

		public void INTERNAL_BOX( Rect r, string text )
		{
			tc.text = text;
			INTERNAL_BOX( r, tc );
		}

		public void INTERNAL_BOX( Rect r )
		{
			INTERNAL_BOX( r, "" );
		}

		// static bool? haveInternalBox = null;
		Color InternalBoxColor = new Color(1, 1, 1, 0.5f);

		public void INTERNAL_BOX( Rect r, GUIContent tc )
		{
			if ( !string.IsNullOrEmpty( tc.tooltip ) )
			{
				GUI.Label( r, tc );
			}

			if ( Event.current.type == EventType.Repaint )
			{
				var oc = GUI.color;
				GUI.color *= InternalBoxColor;
				STYLE_InternalBoxStyle.Draw( r, tc.text, false, false, false, false );
				GUI.color = oc;
			}

			/*if ( !haveInternalBox.HasValue ) haveInternalBox = Adapter.GET_SKIN().box.normal.background;
            if ( haveInternalBox.Value ) GUI.Box( r , tc );
            else {
                if ( Event.current.type == EventType.Repaint ) {
                    var oc = GUI.color;
                    GUI.color *= InternalBoxColor;
                    Adapter.GET_SKIN().textArea.Draw( r , tc.text , false , false , false , false );
                    GUI.color = oc;
                }
                if ( !string.IsNullOrEmpty( tc.tooltip ) ) {
                    oc = GUI.color;
                    GUI.color = new Color( 0 , 0 , 0 , 0 );
                    GUI.Label( r , tc );
                    GUI.color = oc;
                }

            }*/
		}




		/*internal GUIStyle InitializeStyle( string icon )
		{
			return InitializeStyle( icon, 0, 0, 0, 0 );
		}
		internal GUIStyle InitializeStyle( string icon, float border )
		{
			return InitializeStyle( icon, border, border, border, border );
		}	*/
		internal GUIStyle InitializeStyle( string icon, float LEFT_B, float RIGHT_B, float TOP_B, float BOTTOM_B, TextClipping clipping = TextClipping.Overflow, GUIStyle refStyle = null, bool externalPMod = false )
		{
			var txt = externalPMod ? Root.icons.GetOldExternalMod(ref icon) : GetOldIcon(icon).texture;
			GUIStyle result;


			result = refStyle != null ? new GUIStyle( refStyle ) : new GUIStyle();

			result.normal = new GUIStyleState() { background = txt };
			result.focused = new GUIStyleState() { background = txt };
			result.hover = new GUIStyleState() { background = txt };
			result.border = new RectOffset( (int)(txt.width * LEFT_B), (int)(txt.width * RIGHT_B), (int)(txt.height * TOP_B), (int)(txt.height * BOTTOM_B) );
			result.padding = new RectOffset( 3, 3, -1, -1 );
			result.alignment = TextAnchor.MiddleLeft;

			result.focused.textColor = result.active.textColor = result.hover.textColor = result.normal.textColor = Color.white;
			result.clipping = clipping;
			//  result.overflow = new RectOffset(5,5,5,5);
			//result.wordWrap = true;

			return result;
		}
		/*internal GUIStyle InitializeStyle( string icon, string iconhover, float LEFT_B, float RIGHT_B, float TOP_B, float BOTTOM_B, TextClipping clipping = TextClipping.Clip, GUIStyle refStyle = null )
		{

			var result = InitializeStyle( icon, LEFT_B, RIGHT_B, TOP_B, BOTTOM_B, clipping, refStyle );
			// result.hover.background = result.focused.background = result.active.background = GetIcon(iconhover);
			result.active = new GUIStyleState() { background = GetIcon( iconhover ) };
			return result;
		}
			 */

	}


}
