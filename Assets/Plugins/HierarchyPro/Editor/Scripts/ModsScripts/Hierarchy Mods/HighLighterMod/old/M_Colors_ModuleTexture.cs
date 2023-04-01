using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor.Mods
{
	internal partial class HighlighterMod : DrawStackAdapter, IModSaver
	{


		internal override void ResetStack()
		{
			foreach ( var nc in new_child_cache_dic )
			{
				nc.Value.wasLastAssign = false;
			}
			base.ResetStack();
			ICON_STACK.ResetStack();
		}
		//internal void RESET_COLOR_STACKS()
		//{
		//	ResetStack();
		//	//ICON_STACK.ResetStack();
		//}


		internal override void ResetStack( int id, bool disableLog = false )
		{
			foreach ( var nc in new_child_cache_dic )
			{
				nc.Value.wasLastAssign = false;
			}
			base.ResetStack( id, disableLog );
			ICON_STACK.ResetStack( id, disableLog );
		}


		Rect MoveToClip( Rect rect, Rect clip )
		{
			//EMX_TODO local convertion disabled
			//rect.x -= clip.x;
			//rect.y -= clip.y;
			return rect;
		}

		Rect FULL_RECT;
		Rect RIGHT;

		Rect LEFT;

		Rect COMBINE_CLIP( Rect source, Rect? target )
		{
			if ( !target.HasValue ) return source;
			var xMin = Mathf.Max(source.xMin, target.Value.xMin);
			var yMin = Mathf.Max(source.yMin, target.Value.yMin);
			var xMax = Mathf.Min(source.xMax, target.Value.xMax);
			var yMax = Mathf.Min(source.yMax, target.Value.yMax);

			//	var xMax = Mathf.Min(source.x + source.width, target.Value.x + target.Value.width);
			//	var yMax = Mathf.Min(source.y + source.height, target.Value.y + target.Value.height);
			//
			source.Set( xMin, yMin, xMax - xMin, yMax - yMin );

			//if (target.Value.x + target.Value.width > source.x + source.width)
			//{
			//	source.xMax
			//	source.width -= target.Value.x - source.x;
			//	source.x = target.Value.x;
			//}
			//if (target.Value.y > source.y)
			//{
			//	source.height -= target.Value.y - source.y;
			//	source.y = target.Value.y;
			//}
			//if (target.Value.width < source.width) source.width = target.Value.width;
			//if (target.Value.height < source.height) source.height = target.Value.height;
			return source;
		}

		//  Color asdasd4 = ;
		bool EQ( float a, float b )
		{
			return Mathf.Abs( a - b ) < 2;
		}
		void DrawGradient( Rect rect, TempColorClass tempColor, bool isMain, DynamicColor color,
						  HierarchyObject currentObject, Rect? clip = null ) // if ( Event.current.type != EventType.Repaint ) return;
		{
			var BGCOLOR = (Color)tempColor.BGCOLOR;

			if ( rect.height > 2 )
			{

				var G = set.HIGHLIGHTER_TEXTURE_GROW;
				if ( G != 0 )
				{
					if ( clip.HasValue )
					{
						var c = clip.Value;
						if ( EQ( c.x, rect.x ) )
						{
							c.x -= G;
							c.width += G;
						}
						if ( EQ( c.y, rect.y ) )
						{
							c.y -= G;
							c.height += G;
						}
						if ( EQ( c.x + c.width, rect.x + rect.width ) ) c.width += G;
						if ( EQ( c.y + c.height, rect.y + rect.height ) ) c.height += G;
						clip = c;
					}

					rect.x -= G;
					rect.y -= G;
					rect.width += 2 * G;
					rect.height += 2 * G;
				}
			}



			if ( tempColor.BG_ALIGMENT_RIGHT_CONVERTED == BgAligmentRight.WidthFixedGradient )
			{   /*var c = GUI.color;
				             GUI.color *= color;
				             // GUI.DrawTexture( rect, adapter.GetIcon( EditorGUIUtility.isProSkin ? "HL_GRADIENT" : "HL_GRADIENT PERSONAL" ) );
				             BG_TEXTURE_STYLE.border.bottom = BG_TEXTURE_STYLE.border.top = BG_TEXTURE_STYLE.border.left = BG_TEXTURE_STYLE.border.right = 0;
				             BG_TEXTURE_STYLE.normal.background = adapter.GetIcon( EditorGUIUtility.isProSkin ? "HL_GRADIENT" : "HL_GRADIENT PERSONAL" );
				             BG_TEXTURE_STYLE.Draw( rect, emptyCont, false, false, false, false );
				             GUI.color = c;*/
				//     Draw_GUITexture( rect, adapter.GetIcon( EditorGUIUtility.isProSkin ? "HL_GRADIENT" : "HL_GRADIENT PERSONAL" ), color );
				//Debug.Log(clip);

				if ( isMain && tempColor.BG_ALIGMENT_LEFT_CONVERTED == BgAligmentLeft.MinLeft &&
						set.HIGHLIGHTER_LEFT_OVERLAP == 1
						&& raw_old_leftpadding != 0 ) // rect.x -= adapter.raw_old_leftpadding;
				{   // rect.width += adapter.raw_old_leftpadding;

					var r = rect;

					r.x -= raw_old_leftpadding;
					//r.width = raw_old_leftpadding;
					rect = r;
				}


				DRAW_BGTEXTURE( BGCOLOR, rect, adapter.GetExternalModOld( EditorGUIUtility.isProSkin ? "HL_GRADIENT" : "HL_GRADIENT PERSONAL" ), color, clip: clip );
				//DRAW_BGTEXTURE(BGCOLOR, MoveToClip(FULL_RECT, RIGHT), color, clip: clip);
			}

			else
			{
				bool RIGHTHasValue = false;
				bool LEFTHasValue = false;


				if ( isMain && tempColor.BG_ALIGMENT_RIGHT_CONVERTED == BgAligmentRight.MaxRight )
				{
					if ( UnityVersion.UNITY_CURRENT_VERSION >= UnityVersion.UNITY_2019_2_0_VERSION )
						rect.width -= adapter.ha.PREFAB_BUTTON_SIZE;

					//rect.width -= adapter.raw_old_leftpadding;


					if ( adapter.ha.PREFAB_BUTTON_SIZE != 0 && (adapter.pluginID == 0
															? Prefabs.FindPrefabRoot( currentObject.go ) == currentObject.go
															: false) /*|| true*/)
					{
						var r = rect;
						r.x += rect.width;
						r.width = adapter.ha.PREFAB_BUTTON_SIZE;
						RIGHTHasValue = true;
						RIGHT = r;
					}

					else
					{
						rect.width += adapter.ha.PREFAB_BUTTON_SIZE;
					}


					// GUI.DrawTexture( r, BG_TEXTURE );
				}

				// if ( isMain && tempColor.BG_ALIGMENT_RIGHT_CONVERTED == BgAligmentRight.MaxRight && adapter.HIGHLIGHTER_LEFT_OVERFLOW == 1)
				if ( isMain && tempColor.BG_ALIGMENT_LEFT_CONVERTED == BgAligmentLeft.MinLeft &&
						set.HIGHLIGHTER_LEFT_OVERLAP == 1
						&& raw_old_leftpadding != 0 ) // rect.x -= adapter.raw_old_leftpadding;
				{   // rect.width += adapter.raw_old_leftpadding;

					var r = rect;

					r.x -= raw_old_leftpadding;
					r.width = raw_old_leftpadding;
					LEFTHasValue = true;
					LEFT = r;
				}



				if ( RIGHTHasValue || LEFTHasValue )
				{
					FULL_RECT = LEFTHasValue ? LEFT : rect;
					var t = RIGHTHasValue ? RIGHT : rect;
					FULL_RECT.width = t.x + t.width - FULL_RECT.x;
					/*LEFT.x -= clipRect.x;
					LEFT.y -= clipRect.y;
					RIGHT.x -= clipRect.x;
					RIGHT.y -= clipRect.y;
					rect.x -= clipRect.x;
					rect.y -= clipRect.y;*/
				}

				bool drawClip = set.HIGHLIGHTER_TEXTURE_BORDER != 0 && set.HIGHLIGHTER_TEXTURE_STYLE != 0;


				if ( RIGHTHasValue )
				{   // var c = color;
					// c.a *= 0.2f;
					if ( drawClip ) // GUI.BeginClip( RIGHT );
					{//	Draw_BeginClip(RIGHT);

						DRAW_BGTEXTURE( BGCOLOR, MoveToClip( FULL_RECT, RIGHT ), color, 0.2f, COMBINE_CLIP( RIGHT, clip ) );
						//Draw_EndClip();
						//  GUI.EndClip();
					}

					else
						DRAW_BGTEXTURE( BGCOLOR, RIGHT, color, 0.2f, clip );

					/*var c = GUI.color;
					GUI.color *= new Color( 1, 1, 1, 0.2f );
					
					if ( drawClip )
					{   GUI.BeginClip( RIGHT );
					    DRAW_BGTEXTURE( MoveToClip( FULL_RECT, RIGHT ), color );
					    GUI.EndClip();
					}
					else
					    DRAW_BGTEXTURE( RIGHT, color );
					
					
					GUI.color = c;*/
				}

				if ( LEFTHasValue )
				{   /*  var c = GUI.color;
					                   GUI.color *= new Color( 1, 1, 1, 0.4f );
					
					                   if ( drawClip )
					                   {   GUI.BeginClip( LEFT );
					                       DRAW_BGTEXTURE( MoveToClip( FULL_RECT, LEFT ), color );
					                       GUI.EndClip();
					                   }
					                   else      //  GUI.DrawTexture( r, BG_TEXTURE );
					                       DRAW_BGTEXTURE( LEFT, color );
					                   GUI.color = c;*/

					//  var c = color;
					//  c.a *= 0.4f;
					if ( drawClip ) // GUI.BeginClip( RIGHT );
					{//	Draw_BeginClip(LEFT);
					 //Debug.Log( FULL_RECT );

						DRAW_BGTEXTURE( BGCOLOR, MoveToClip( FULL_RECT, LEFT ), color, 0.4f, COMBINE_CLIP( LEFT, clip ) );
						//Draw_EndClip();
						//  GUI.EndClip();
					}

					else
						DRAW_BGTEXTURE( BGCOLOR, LEFT, color, 0.4f, clip: clip );
				}


				if ( (RIGHTHasValue || LEFTHasValue) && drawClip ) // GUI.BeginClip( rect );
				{//	Draw_BeginClip(rect);
					DRAW_BGTEXTURE( BGCOLOR, MoveToClip( FULL_RECT, rect ), color, clip: COMBINE_CLIP( rect, clip ) );
					//Draw_EndClip();
					// GUI.EndClip();
				}

				else
				{
					DRAW_BGTEXTURE( BGCOLOR, rect, color, clip: clip );
				}

				//GUI.DrawTexture( rect, BG_TEXTURE );

				// GUI.Button(rect, "");
			}
		}


		//  GUIContent emptyCont = new GUIContent();
		internal void DRAW_BGTEXTURE_OLD( Rect rect, Color color )
		{
			if ( Event.current.type == EventType.Repaint )
			{
				var border = set.HIGHLIGHTER_TEXTURE_BORDER;



				if ( DRAW_WITHOU_MATERIAL || rect.height == 1 /*!set.HIGHLIGHTER_USE_SPECUAL_SHADER || !set.HIghlighterExternalMaterial ||
						set.HIGHLIGHTER_TEXTURE_STYLE == 0 || !BG_TEXTURE*/)
				{
					if ( rect.height == 1 || border == 0
						)
					{
						if ( rect.height == 1 ) rect.height = 0.99f;
						GUI.DrawTexture( rect, BG_TEXTURE( ref rect ) ?? Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 1, color, 0, border );
					}
					else
					{
						var tex = BG_TEXTURE( ref rect );
						Root.p[ 0 ].gl._DrawTexture_ForExternalWindow( rect, Icons.GetIconDataFromTexture( tex ?? Texture2D.whiteTexture ), ref color, border, null );
					}
				}

				else
				{
					//set.HIghlighterExternalMaterial.SetColor(colorProperty, color);
					//var c = GUI.color;
					//GUI.color *= color;
					//Graphics.DrawTexture(rect, BG_TEXTURE, border, border, border, border, set.HIghlighterExternalMaterial, 0);
					Root.p[ 0 ].gl._DrawTexture_ForExternalWindow( rect, Icons.GetIconDataFromTexture( BG_TEXTURE( ref rect ) ), ref color, border, set.HIghlighterExternalMaterial );

					//GUI.color = c;
				}
			}
		}

		bool DRAW_WITHOU_MATERIAL {
			get {
				return !set.HIGHLIGHTER_USE_SPECUAL_SHADER || !set.HIghlighterExternalMaterial ||
						set.HIGHLIGHTER_TEXTURE_STYLE == 0; //|| !BG_TEXTURE
			}
		}

		internal void DRAW_BGTEXTURE( Color BGCOLOR, Rect rect, DynamicColor color, float alpha = 1, Rect? clip = null )
		{
			if ( Event.current.type == EventType.Repaint )
			{
				DrawBgDynamicColorHelperStructStr.alpha = alpha;
				DrawBgDynamicColorHelperStructStr.BGCOLOR = BGCOLOR;

				var tex = BG_TEXTURE( ref rect );

				var border = set.HIGHLIGHTER_TEXTURE_BORDER;
				if ( border > Mathf.Min( tex.width, tex.height ) / 2 )
					border = (int)Mathf.Min( tex.width, tex.height ) / 2;

				if ( border > Mathf.Min( rect.width, rect.height ) / 2 )
					border = (int)Mathf.RoundToInt( Mathf.Min( rect.width, rect.height ) / 2 );

				if ( DRAW_WITHOU_MATERIAL || rect.height == 1 )
				{   /*var c = GUI.color;
					                 GUI.color *= color;
					                 BG_TEXTURE_STYLE.border.bottom = BG_TEXTURE_STYLE.border.top = BG_TEXTURE_STYLE.border.left = BG_TEXTURE_STYLE.border.right = border;
					                 BG_TEXTURE_STYLE.normal.background = BG_TEXTURE ?? Texture2D.whiteTexture;
					                 BG_TEXTURE_STYLE.Draw( rect, emptyCont, false, false, false, false );
					                 GUI.color = c;
					                 Draw_Style
					                 Draw_StyleWithBorders()*/
					if ( rect.height == 1 ) rect.height = 0.99f;
					Draw_GUITextureWithBorders( rect, Icons.GetIconDataFromTexture( tex ), border, color,
											   DrawBgDynamicColorHelperStructStr, clip );
				}

				else
				{   /* adapter.HIghlighterExternalMaterialNormal.SetColor( colorProperty, color * GUI.color * new Color(1, 1, 1, 0.3f));
					                  Graphics.DrawTexture( rect, BG_TEXTURE, border, border, border, border,
					                                        adapter.HIghlighterExternalMaterialNormal, 0 );*/


					Draw_GUITextureWithBordersAndMaterial( rect, Icons.GetIconDataFromTexture( tex ), border, set.HIghlighterExternalMaterial,
														  color, DrawBgDynamicColorHelperStructStr, clip );
					/* adapter.HIghlighterExternalMaterial.SetColor( colorProperty, color * GUI.color );
					 Graphics.DrawTexture( rect, BG_TEXTURE, border, border, border, border,
					                       adapter.HIghlighterExternalMaterial, 0 );*/
				}

				// GUI.DrawTexture(rect, BG_TEXTURE, ScaleMode.StretchToFill,true,1)
				//  HIghlighterExternalShader
			}
		}

		internal void DRAW_BGTEXTURE( Color BGCOLOR, Rect rect, Texture2D BG_TEXTURE, DynamicColor color, Rect? clip = null )
		{
			if ( Event.current.type == EventType.Repaint )
			{
				DrawBgDynamicColorHelperStructStr.alpha = 1;
				DrawBgDynamicColorHelperStructStr.BGCOLOR = BGCOLOR;

				if ( DRAW_WITHOU_MATERIAL || rect.height == 1/*!set.HIGHLIGHTER_USE_SPECUAL_SHADER || !set.HIghlighterExternalMaterial ||
						set.HIGHLIGHTER_TEXTURE_STYLE == 0 || !BG_TEXTURE*/)
				{
					if ( rect.height == 1 ) rect.height = 0.99f;
					Draw_GUITextureWithBorders( rect, Icons.GetIconDataFromTexture( BG_TEXTURE ?? Texture2D.whiteTexture ), 0, color,
											   DrawBgDynamicColorHelperStructStr, clip );
				}

				else
				{
					Draw_GUITextureWithBordersAndMaterial( rect, Icons.GetIconDataFromTexture( BG_TEXTURE ?? Texture2D.whiteTexture ), 0, set.HIghlighterExternalMaterial,
														  color, DrawBgDynamicColorHelperStructStr, clip );
				}
			}
		}

		DrawBgDynamicColorHelperStruct DrawBgDynamicColorHelperStructStr;
		GUIStyle __BG_TEXTURE_STYLE;

		GUIStyle BG_TEXTURE_STYLE {
			get { return __BG_TEXTURE_STYLE ?? (__BG_TEXTURE_STYLE = new GUIStyle()); }
		}

		//Texture2D __BG_TEXTURE_TEXT;
		//Texture2D __BG_TEXTURE_BOX;
		//Texture2D __BG_PROXY
		//{
		//	get
		//	{
		//		if (set.HIGHLIGHTER_TEXTURE_STYLE == 0) return Texture2D.whiteTexture;
		//
		//		if (set.HIGHLIGHTER_TEXTURE_STYLE == 1)
		//		{
		//			if (__BG_TEXTURE_BOX == null)
		//				__BG_TEXTURE_BOX = adapter.GET_SKIN().box.normal.background ??
		//								   adapter.GET_SKIN().box.normal.scaledBackgrounds[0];
		//
		//			return __BG_TEXTURE_BOX;
		//		}
		//
		//		if (set.HIGHLIGHTER_TEXTURE_STYLE == 2)
		//		{
		//			if (__BG_TEXTURE_TEXT == null)
		//				__BG_TEXTURE_TEXT = adapter.GET_SKIN().textArea.normal.background ??
		//									adapter.GET_SKIN().textArea.normal.scaledBackgrounds[0];
		//
		//			return __BG_TEXTURE_TEXT;
		//		}
		//
		//		if (set.HIGHLIGHTER_TEXTURE_STYLE == 3) return set.HIghlighterExternalTexture;
		//		throw new Exception("ASD");
		//	}
		//}


		DrawStackMethodsWrapper __ATTR_BUT_HASH = null;

		DrawStackMethodsWrapper ATTR_BUT_HASH {
			get { return __ATTR_BUT_HASH ?? (__ATTR_BUT_HASH = new DrawStackMethodsWrapper( ATTR_BUT )); }
		}

		void ATTR_BUT( Rect worldOffset, Rect colorRect, DrawStackMethodsWrapperData data, HierarchyObject _o )
		{
			//var args = (AttrArgs)data.args;
			//var el = args.el;

			var r2 = colorRect;
			_ChessLineBgColorV = BackgroundDecorations.GetColor( adapter, _o, ref r2 );
			if ( _ChessLineBgColorV.HasValue )
			{
				if ( adapter.ha.IsSelected( _o ) ) return;
				colorRect.width = r2.x + r2.width - colorRect.x;
				//Draw_AdapterTextureWithDynamicColor( colorRect, ChessLineBgColor, SWITCHER: 1 );
				EditorGUI.DrawRect( colorRect, _ChessLineBgColorV.Value );
			}
		}



		Texture2D BG_TEXTURE( ref Rect rect )
		{
			//get {
			if ( rect.height < 2 ) return Texture2D.whiteTexture;
			return set.BG_TEXTURE ?? Texture2D.whiteTexture;
			//}

			//  {   get { return Adapter.GET_SKIN().textArea.normal.scaledBackgrounds[0]; }
		}

		// SingleList _DrawColoredBGEmpty = new SingleList() {list = Enumerable.Repeat(0, 9).ToList()};
		Rect tr;
		Rect? lastBgRect;

		internal TempColorClass DrawColoredBG( DynamicRect drawRect, HierarchyObject parentObject,
											  HierarchyObject currentObject, bool breakIfNoBroadCast, int SKIPLABEL = 0, bool resetFonts = true,
											  new_child_struct overrideRect = null, bool returnAfterLastRect = false )
		{
			if ( drawRect != null ) currentObject.ah.BACKGROUNDED = 0;


			if ( parentObject.ah.localTempColor == null )
				parentObject.ah.localTempColor = new TempColorClass().AssignFromList( 0, true );

			var tempColor = parentObject.ah.localTempColor;

			/* if (EVENT.type == EventType.Repaint)
			     if ( currentObject.name == "SpawnPoint" ) Debug.Log( currentObject.MIXINCOLOR.HAS_LABEL_COLOR );*/


			if ( EVENT.type == EventType.Repaint && currentObject.ah.MIXINCOLOR != null )
			{
				tempColor = currentObject.ah.MIXINCOLOR;
			}

			else
			{
				bool anywayelse = true;
				//loopag:;

				var res = HighlighterCache_Colors.HighlighterHasKey(parentObject, this);
				if ( res.IsTrue( breakIfNoBroadCast ) )
				{

					var ptr = res.id;

					if ( ptr != -1 )
					{
						anywayelse = false;

						//var d = adapter.MOI.des(currentObject.scene);
						//if (ptr > d.GetHash6().Count - 1)
						//{	if (loop)
						//	{	adapter.logProxy.LogWarning("GetHighlighterValue Out Of Range please contact support");
						//	}
						//	
						//	loop = true;
						//	cacheDichighlighter.Clear();
						//	goto loopag;
						//}
						//loop = false;
						//// Debug.LogError( ptr + " is out of range " + d.GetHash6().Count );
						//var el = d.GetHash6()[ptr];
						//tempColor.CopyFromList(el.list);

						tempColor.CopyFromList( ref res.data );


						if ( breakIfNoBroadCast && !tempColor.child ) return null;

						//  Debug.Log(ptr + " " + parentObject.name);
					}
				}

				if ( anywayelse ) //tempColor.AssignFromList(_DrawColoredBGEmpty);
				{   // if (!tempColor.HAS_BG_COLOR && !tempColor.HAS_LABEL_COLOR)
					var filter = autoMod.GetFilter(parentObject);

					if ( filter == null || breakIfNoBroadCast && !filter.child ||
							!filter.HAS_BG_COLOR && !filter.HAS_LABEL_COLOR ) return null;

					// filter.OverrideTo(ref tempColor);
					tempColor.CopyFromFilter( filter );
				}
			}


			// if (tempColor.BG_ALIGMENT_LEFT < 2)tempColor.BGCOLOR.a = (byte)(tempColor.BGCOLOR.a / 255f * 200);

			//	if (SKIPLABEL != 2)	if (tempColor.HAS_BG_COLOR)


			if ( drawRect == null )
			{
				return tempColor;
			}


			var colorRect = drawRect.ref_selectionRect;
			colorRect.x -= raw_old_leftpadding;
			/*  if ( adapter.HIGHLIGHTER_LEFT_OVERFLOW != 1 )
			      colorRect.x -= adapter.raw_old_leftpadding;*/

			if ( UnityVersion.UNITY_CURRENT_VERSION < UnityVersion.UNITY_2019_2_0_VERSION )
				colorRect.x += adapter.ha.LEFT_PADDING;

			var BgRect = overrideRect == null ? drawRect.ConvertToBGFromTempColor(tempColor) : overrideRect.LocalRect;


			if ( returnAfterLastRect )
			{
				if ( SKIPLABEL != 2 && tempColor.HAS_BG_COLOR )
				{
					lastBgRect = BgRect;
					return tempColor;
				}

				return tempColor;
			}





#if UNITY_2019_3_OR_NEWER
#endif
			colorRect.x += 2;

			colorRect.x += DEFAULT_ICON_SIZE;
			colorRect.width -= DEFAULT_ICON_SIZE;

			var LABEL_STYLE = adapter.ha.lineStyle ?? adapter.labelStyle;
			var LABEL_COLOR = LABEL_STYLE.normal.textColor;

			var oldLabelColor = LABEL_COLOR;
			var al1 = tempColor.BG_ALIGMENT_RIGHT_CONVERTED;
			var al2 = tempColor.BG_ALIGMENT_LEFT_CONVERTED;

			//var NEW_NEED_LABEL = al2 == BgAligmentLeft.EndLabel || al2 == BgAligmentLeft.Modules || al1 == BgAligmentRight.BeginLabel || al1 == BgAligmentRight.Icon;
			var NEW_NEED_LABEL = al2 == BgAligmentLeft.Modules || al1 == BgAligmentRight.BeginLabel || al1 == BgAligmentRight.Icon;
			NEW_NEED_LABEL = !NEW_NEED_LABEL;

			bool needLabel = false;

			if ( SKIPLABEL != 1 )
			{
#pragma warning disable
#pragma warning restore

				if ( tempColor.HAS_LABEL_COLOR )
				{
					needLabel = true;

					LABEL_COLOR = tempColor.LABELCOLOR;
				}

				else
				{
					var type = IS_HIERARCHY()
							   ? Prefabs.GetPrefabType(currentObject.go)
							   : PrefabInstanceStatus.NotAPrefab;

					// if (!(type == PrefabType.None || type == PrefabType.DisconnectedModelPrefabInstance || type == PrefabType.DisconnectedPrefabInstance))
					if ( !(type == PrefabInstanceStatus.NotAPrefab || type == PrefabInstanceStatus.Disconnected) )
					{
						var co = EditorGUIUtility.isProSkin ? prefabColorPro : prefabColorPersonal;

						//if (!currentObject.Active()) co.a /= 2;

						LABEL_COLOR = co;
						needLabel = NEW_NEED_LABEL;
					}

					else
						if ( (type == PrefabInstanceStatus.MissingAsset) )
					{
						var co = EditorGUIUtility.isProSkin ? prefabMissinColorPro : prefabMissinColorPersonal;

						//if (!currentObject.Active()) co.a /= 2;

						LABEL_COLOR = co;
						needLabel = NEW_NEED_LABEL;
					}

					else
					// {   Adapter.GET_SKIN().label.normal.textColor = parentObject.cache_prefab ? m_PrefabColorPro : adapter.oldColl;
					{
						LABEL_COLOR = oldLabelColor;
					}
				}
			}


			/*   oc = GUI.contentColor;
			   contentColor.a = list[3] / 255f * 90;
			   contentColor.a /= 2;
			   GUI.contentColor = contentColor;*/


			if ( ( /***currentObject.Active() ||***/ needLabel) && SKIPLABEL != 1 )
			{
				var sourceBgColor = SourceBGColor; //currentObject.id


				/***  if (SKIPLABEL != 2 && tempColor.HAS_BG_COLOR) //SKIPLABEL == 2 it never uses
				  {
				   sourceBgColor =  Color.Lerp( sourceBgColor,  (Color)tempColor.BGCOLOR,  ((Color)tempColor.BGCOLOR).a) ;
				      sourceBgColor.a = 1;
				  }***/
				/**/
				BEGIN_DRAW_SWITCHER( sourceBgColor_fadeBg_swithcer_HASH );

				// if ( adapter.hashoveredItem && adapter.hoverID == currentObject.id )
				{   // var _oc = LABEL_COLOR;
					//var _oc = adapter.oldColl;
					//LABEL_COLOR = sourceBgColor;
					//  colorRect.x -= 0.5f;
					var DDD = 0.5f;
					// var O = 0.75f;
					var frect = colorRect;
					Draw_LabelWithTextDynamicColor( frect, currentObject.name, sourceBgColor, LABEL_STYLE, false,
												   SWITCHER: 0 );
					frect.x += DDD;
					frect.y -= DDD;

					Draw_LabelWithTextDynamicColor( frect, currentObject.name, sourceBgColor, LABEL_STYLE, false,
												   SWITCHER: 0 );
					// GUI.Label( frect, currentObject.name, LABEL_STYLE );
					// frect.x += 1;
					frect.y += DDD * 2;
					Draw_LabelWithTextDynamicColor( frect, currentObject.name, sourceBgColor, LABEL_STYLE, false,
												   SWITCHER: 0 );
					//  GUI.Label( frect, currentObject.name, LABEL_STYLE );
					frect.x -= DDD * 2;

					// frect.x -= DDD;
					Draw_LabelWithTextDynamicColor( frect, currentObject.name, sourceBgColor, LABEL_STYLE, false,
												   SWITCHER: 0 );
					// GUI.Label( frect, currentObject.name, LABEL_STYLE );
					// frect.x += 1;
					frect.y -= DDD * 2;
					Draw_LabelWithTextDynamicColor( frect, currentObject.name, sourceBgColor, LABEL_STYLE, false,
												   SWITCHER: 0 );
					// GUI.Label( frect, currentObject.name, LABEL_STYLE );
					// frect.x -= DDD;
					//   GUI.Label( frect, currentObject.name, LABEL_STYLE );
					// frect.y -= O;
					// colorRect.x -= 0.5f;
					//  colorRect.x += 1;
					//GUI.Label( colorRect, currentObject.name );
					//LABEL_COLOR = _oc;
				}
				//   else
				{   /* GUI .color *= sourceBgColor;
					                  GUI.DrawTexture( colorRect, Texture2D.whiteTexture );
					                  //GUI.Box(colorRect, "");
					                  // DrawGradient( colorRect );
					                  GUI .color = oc;*/
					Rect r = colorRect;
					r.height -= 2;
					Draw_AdapterTextureWithDynamicColor( r, sourceBgColor, SWITCHER: 1 );
					//if ( adapter.WIN_SET_G( pluginID ).DRAW_HIERARHCHY_CHESS_FILLS != 0 )
					//{
					//    var r2 = colorRect;
					//    _ChessLineBgColorV = BackgroundDecorations.GetColor( adapter, pluginID, ref r2 );
					//    r.width =  r2 .x +r2.width - r.x;
					//    if ( _ChessLineBgColorV.HasValue )
					//    {
					//        Draw_AdapterTextureWithDynamicColor( r, ChessLineBgColor, SWITCHER: 1 );
					//    }
					//}

					if ( adapter.WIN_SET_G( pluginID ).DRAW_HIERARHCHY_CHESS_FILLS != 0 && adapter.WIN_SET_G( pluginID ).USE_BACKGROUNDDECORATIONS_MOD )
					{
						BackgroundDecorations.DRAW_HIERARHCHY_CHESS_FILLS = adapter.WIN_SET_G( pluginID ).DRAW_HIERARHCHY_CHESS_FILLS;
						BackgroundDecorations.COLOR_HIERARHCHY_CHESS_FILLS = adapter.WIN_SET_G( pluginID ).COLOR_HIERARHCHY_CHESS_FILLS;
						//Draw_Action( r, ATTR_BUT_HASH, null, 0 );
						Draw_Action( r, ATTR_BUT_HASH, null, 1 );
					}
				}

				END_DRAW_SWITCHER();
			}

			if ( SKIPLABEL != 2 )
				if ( tempColor.HAS_BG_COLOR )
				{   //GUI .color *= sourceBgColor;
					//GU I.color = oc;
					lastBgRect = BgRect;

					//EMX_TODO probably dynamic rect better than just rect
					//if (overrideRect != null)
					//	//GUI.BeginClip( overrideRect.ModifiedSelectionRect( adapter ) );
					//	Draw_BeginClip(overrideRect.ModifiedSelectionRect(adapter));
					//
					//DrawGradient(BgRect, tempColor, drawRect.isMain, DrawBgDynamicColorHelper, currentObject);
					//
					//if (overrideRect != null)
					//	//GUI.EndClip();
					//	Draw_EndClip();
					if ( overrideRect != null )
						DrawGradient( BgRect, tempColor, drawRect.isMain, DrawBgDynamicColorHelper, currentObject, overrideRect.ModifiedSelectionRect( adapter ) );
					else
						DrawGradient( BgRect, tempColor, drawRect.isMain, DrawBgDynamicColorHelper, currentObject );


					needLabel = NEW_NEED_LABEL;


					var LEFT_BG = overrideRect != null
								  ? overrideRect.ModifiedSelectionRect(adapter).x + BgRect.x
								  : BgRect.x;
					var LEFT_LABEL = colorRect.x;

					LEFT_LABEL -= DEFAULT_ICON_SIZE + 16;

					/*  var RIGHT_BG = LEFT_BG + BgRect.width ;
					  var RIGHT_LABEL = colorRect.x + colorRect.width;
					  if ( overrideRect != null )
					  {   Debug.Log( RIGHT_BG + " " + RIGHT_LABEL );
					  }*/
					/*if ( tempColor.BG_ALIGMENT_LEFT < 2 && tempColor.BG_ALIGMENT_RIGHT != 4 )
					    currentObject.BACKGROUNDEsourceBgColorD = sourceBgColor;
					else
					    currentObject.BACKGROUNDEsourceBgColorD = Color.black;*/

					var LEFT_SKIP = LEFT_LABEL < LEFT_BG;

					if ( tempColor.BG_ALIGMENT_RIGHT_CONVERTED == BgAligmentRight.BeginLabel ||
							tempColor.BG_ALIGMENT_RIGHT_CONVERTED == BgAligmentRight.Icon )
						LEFT_SKIP |= LEFT_LABEL > LEFT_BG + BgRect.width;


					// var RIGHT_SKIP =  RIGHT_LABEL > RIGHT_BG;
					var RIGHT_SKIP = false;

					if ( tempColor.BG_ALIGMENT_LEFT_CONVERTED == BgAligmentLeft.Modules ) RIGHT_SKIP = true;

					if ( !RIGHT_SKIP ) currentObject.ah.FLAGS |= 1;


					var worldRect = BgRect;

					if ( overrideRect != null )
					{
						worldRect.x += overrideRect.ModifiedSelectionRect( adapter ).x;
						worldRect.y += overrideRect.ModifiedSelectionRect( adapter ).y;
					}

					currentObject.ah.BG_RECT = worldRect;
					// || colorRect.x > X
					/* if ( overrideRect != null  && colorRect.x > X )
					 {   currentObject.FLAGS |= 1;
					 }*/
					currentObject.ah.BACKGROUNDED =
						(tempColor.BG_HEIGHT == 2 && overrideRect == null || LEFT_SKIP) ? 2 : 1;
				}


			if ( (needLabel || tempColor.HAS_LABEL_COLOR) && SKIPLABEL != 1 )
			{
				/* var p = Adapter.GET_SKIN().label.padding;
				 var left = p.left;
				 var right = p.right;
				 var bottom = p.bottom;
				 var top = p.top;
				 p.bottom = p.top = p.left = p.right = 0;
				 Adapter.GET_SKIN().label.fontSize = Adapter.GET_SKIN().textArea.fontSize;*/

				if ( !HAS_LABEL_ICON() )
				{
					colorRect.x -= 2;
				}

				//  Adapter.GET_SKIN().label.fontStyle = FontStyle.Italic;
				var labelRect = colorRect;

				if ( UnityVersion.UNITY_CURRENT_VERSION < UnityVersion.UNITY_2019_3_0_VERSION && wse.USE_LINE_HEIGHT )
				{
					if ( UnityVersion.UNITY_CURRENT_VERSION < UnityVersion.UNITY_2019_VERSION )
					{
						if ( Mathf.RoundToInt( wse.LINE_HEIGHT ) % 2 == 1 )
							labelRect.y = Mathf.CeilToInt( labelRect.y - 0.1f );
						else
							labelRect.y = Mathf.CeilToInt( labelRect.y + 0.1f );
					}
					else
					{
						if ( Mathf.RoundToInt( wse.LINE_HEIGHT ) % 2 != 1 ) labelRect.y -= 1;
					}
				}

				/* var c  = Color.white;
				 if ( !currentObject.Active() )
				 {   // oldGuiColor = GUI
				     c.a *= 0.35f;
				     // GUI. color *= gcn;
				 }*/

				if ( tempColor != null && tempColor.HAS_LABEL_COLOR && tempColor.LABEL_SHADOW == true )
				{
					var _oc2 = LABEL_COLOR;
					// var c2 = adapter.oldColl;
					var c2 = Color.black;
					c2.a = _oc2.a;
					LABEL_COLOR = c2;
					// if ( currentObject.name == "CubeMaps" ) Debug.Log( needLabel && SKIPLABEL != 1 );

					if ( UnityVersion.UNITY_CURRENT_VERSION < UnityVersion.UNITY_2019_3_0_VERSION )
					{
						labelRect.y += 0.5f; //0.5f;
						labelRect.x -= 1; //0.5f;
										  // GUI.Label( labelRect, currentObject.name, LABEL_STYLE );
						Draw_LabelWithTextColor( labelRect, currentObject.name, LABEL_COLOR, LABEL_STYLE, true );
						labelRect.y -= 0.5f; // 0.5f;
						labelRect.x += 1; // 0.5f;
					}

					else
					{
						labelRect.y += 2f / 2f; //0.5f;
						labelRect.x -= 1 / 2f; //0.5f;
						Draw_LabelWithTextColor( labelRect, currentObject.name, LABEL_COLOR, LABEL_STYLE, true );
						//GUI.Label( labelRect, currentObject.name, LABEL_STYLE );
						labelRect.x += 2 / 2f; //0.5f;
						labelRect.y -= 1 / 2f; // 0.5f;
						Draw_LabelWithTextColor( labelRect, currentObject.name, LABEL_COLOR, LABEL_STYLE, true );
						//  GUI.Label( labelRect, currentObject.name, LABEL_STYLE );
						labelRect.y -= 1 / 2f; // 0.5f;
						labelRect.x -= 1 / 2f; // 0.5f;
					}

					// var fs = LABEL_STYLE.fontStyle;
					// LABEL_STYLE.fontStyle = FontStyle .Bold;
					// LABEL_STYLE.fontStyle = fs;
					LABEL_COLOR = _oc2;
				}

				// colorRect.y = Mathf.CeilToInt(colorRect.y);
				/* var h = labelRect.height - GUI.skin.label.CalcHeight(Adapter.GET_CONTENT(currentObject.name), 10000);
				 labelRect.y += h / 2;
				 labelRect.height -= h;*/
				// EditorGUI.DrawRect( labelRect, LABEL_COLOR );
				Draw_LabelWithTextColor( labelRect, currentObject.name, LABEL_COLOR, LABEL_STYLE, true );

				// GUI.Label( labelRect, currentObject.name, LABEL_STYLE );


				//if ( !currentObject.Active() ) GUI .color = oldGuiColor;

				// Adapter.GET_SKIN().label.fontStyle = FontStyle.Normal;
				/*  if (!EditorGUIUtility.isProSkin && (Adapter.GET_SKIN().label.normal.textColor.r + Adapter.GET_SKIN().label.normal.textColor.g + Adapter.GET_SKIN().label.normal.textColor.b) / 3 > 0.5f)
				      GUI.Label( colorRect, currentObject.name );*/

				/*  p.left = left;
				  p.right = right;
				  p.bottom = bottom;
				  p.top = top;*/
			}

			// GUI.contentColor = oc;
			//                 Adapter.GET_SKIN().label.alignment = a;
			//                 Adapter.GET_SKIN().label.fontSize = af;
			//LABEL_STYLE.normal.textColor = oldLabelColor;

			return tempColor;
		}





		private Color oldGuiColor;

		private Color gcn = new Color(1, 1, 1, 0.35f);
		//static int ICON_HEIGHT = 16;

		static Color? _ChessLineBgColorV;
		static DynamicColor _ChessLineBgColor;
		static DynamicColor ChessLineBgColor // if ( adapter.IsSelected( id ) ) Debug.Log( GUI.GetNameOfFocusedControl() );
		{
			get {
				if ( _ChessLineBgColor == null )
				{
					_ChessLineBgColor = new DynamicColor() {
						GET = ( _o, args ) => {
							if ( !_ChessLineBgColorV.HasValue ) return Color.clear;
							return _ChessLineBgColorV.Value;
						}
					};
				}

				return _ChessLineBgColor;
			}
		}

		DynamicColor __SourceBGColor;
		DynamicColor SourceBGColor // if ( adapter.IsSelected( id ) ) Debug.Log( GUI.GetNameOfFocusedControl() );
		{
			get {
				if ( __SourceBGColor == null )
				{
					__SourceBGColor = new DynamicColor() {
						GET = ( _o, args ) => {
							var res = adapter.ha.IsSelected(_o) ? Colors.SelectColor :
								   adapter.hoverID == _o.id && !wse.HIDE_HOVER_BG ? Colors.HoverColor : Colors.EditorBGColor;
							return res;
						}
					};
				}

				return __SourceBGColor;
			}
		}


		HighlighterResult child_Res;

		//internal TempColorClass needdrawGetColor(HierarchyObject o)
		//{
		//	if (!set.USE_MANUAL_HIGHLIGHTER_MOD || set.USE_AUTO_HIGHLIGHTER_MOD) return null;
		//
		//	if ((child_Res = HighlighterCache_Colors.HighlighterHasKey_IncludeFilters(o)).IsTrue(false))
		//	{   /*   var drawhild = EVENT.type == EventType.Repaint && child_Res.BroadCast && new_child_cache.ContainsKey(o.id);
		//		   lastBgRect = null;
		//		   var output = DrawColoredBG( null, o, o, false, overrideRect: drawhild ? (Rect ? ) new_child_cache[o.id] .rect : null );
		//		
		//		   if ( lastBgRect.HasValue && child_Res.BroadCast )
		//		   {   if ( !new_child_cache.ContainsKey( o.id ) )
		//		       {   new_child_cache.Add( o.id, new new_child_struct() { rect = lastBgRect.Value} );
		//		       }
		//		       else
		//		       {   new_child_cache[o.id].rect = lastBgRect.Value;
		//		       }
		//		   }
		//		   return output;*/
		//
		//		return DrawColoredBG(null, o, o, false);
		//	}
		//
		//	// else if (anyNeedBroadcast)
		//	else
		//	{
		//		var parent = o.parent();
		//
		//		while (parent != null)
		//		{
		//			if ((child_Res = HighlighterCache_Colors.HighlighterHasKey_IncludeFilters(parent)).IsTrue(true))
		//			{   /*    var drawhild = EVENT.type == EventType.Repaint && child_Res.BroadCast && new_child_cache.ContainsKey(child_Res.BroadCast.GetInstanceID());
		//				    lastBgRect = null;
		//				    var output = DrawColoredBG( null, parent, o, true, overrideRect: drawhild ? (Rect ? ) new_child_cache[child_Res.BroadCast.GetInstanceID()] .rect : null );
		//				
		//				    if ( lastBgRect.HasValue && child_Res.BroadCast && new_child_cache.ContainsKey( child_Res.BroadCast.GetInstanceID() ) )
		//				    {   new_child_cache[child_Res.BroadCast.GetInstanceID()].SetMax( lastBgRect.Value);
		//				    }
		//				
		//				    return output;*/
		//				return DrawColoredBG(null, parent, o, true);
		//			}
		//
		//			parent = parent.parent();
		//		}
		//	}
		//
		//	return null;
		//}

		//bool wasInitDes = false;
		/*   void UndoClear() {   //  ClearCache();
		       //Debug.Log( "ASD" );
		
		
		   }*/


		TempColorClass __masd123( int id, Rect? SelectionRect, EditorWindow w, DynamicRect drawRect,
								 HierarchyObject o,
								 int needLabel_override, bool resetFonts, HierarchyObject parent, bool? returnAfterLastRect = null )
		{
			var drawhild = false;
			TempColorClass output;

			// if ( adapter.pluginID == 0 && o.name == "FootstepAudioSource" ) Debug.Log("start " + child_Res.BroadCast  + " " + adapter.IS_LAYOUT );
			// if ( /*!adapter.RedrawInit &&*/ child_Res.BroadCast != -1 && ( adapter.IS_LAYOUT || !new_child_cache_dic.ContainsKey( id ) ) || adapter.CHILD_GROUP_FIX_FLAG ) //
			if ( child_Res.BroadCast != -1 && (adapter.EVENT.type == EventType.Layout) ) //|| adapter.RedrawInit
			{
				lastBgRect = null;
				output = DrawColoredBG( drawRect, parent, o, true, needLabel_override, resetFonts,
									   returnAfterLastRect: returnAfterLastRect ?? true );

				if ( lastBgRect.HasValue )
				{   //  bool shouldSet = adapter.IS_LAYOUT;

					if ( !new_child_cache_dic.ContainsKey( id )
					   ) //  if ( adapter.pluginID == 0 && o.name == "Blob" ) Debug.Log( "BLob " + drawhild + " " + id );
					{   // Debug.Log( o.name + " " + id + " " + adapter.IS_LAYOUT );
						new_child_cache_dic.Add( id, new new_child_struct() { } );
						/*  shouldSet = true;
						  if ( !adapter.IS_LAYOUT )
						  {   adapter.CHILD_GROUP_FIX_FLAG = true;
						  }*/
					}


					/*   if ( adapter.pluginID == 0
					           && o.name == "test" ) Debug.Log( SelectionRect.HasValue + " - " + (adapter.GROUPING_CHILD_MODE == 0) +  " - " +  child_Res.BroadCast + " - " + new_child_cache_dic[id].wasInit + " REPAINT " +
					                       adapter.CHILD_GROUP_FIX_FLAG + " " + id );*/

					//     if ( adapter.pluginID == 0 &&  o.name == "Directional Light (19)" ) Debug.Log( new_child_cache_dic[id].wasInit + " REPAINT " + adapter.CHILD_GROUP_FIX_FLAG + " " + id );
					//  if ( shouldSet )
					{
						if ( o.id == parent.id )
						{
							if ( !new_child_cache_dic[ id ].wasInit )
							{
								new_child_cache_dic[ id ].SetRect( lastBgRect.Value,
																		adapter.pluginID == 0 ? Prefabs.FindPrefabRoot( o.go ) == o.go : false );
								// if ( adapter.pluginID == 0 &&  o.name == "Blob" ) Debug.Log( "ASD" );
							}
							else
							{
								var target = new_child_cache_dic[id].rect;
								var dif = lastBgRect.Value.y - target.y;
								target.y += dif;
								target.height -= dif;
								//new_child_cache[id].rect = target;

								new_child_cache_dic[ id ].SetRect( target,
																		adapter.pluginID == 0 ? Prefabs.FindPrefabRoot( o.go ) == o.go : false );
								//  if ( adapter.pluginID == 0 &&  o.name == "Blob" ) Debug.Log( "ASD" );
							}
						}

						else
							if ( !new_child_cache_dic[ id ].wasInit )
						{
							var target = lastBgRect.Value;

							if ( o.id != parent.id )
							{
								target.y -= target.height;
								target.height += target.height;
							}

							new_child_cache_dic[ id ].SetRect( target,
																	adapter.pluginID == 0 ? Prefabs.FindPrefabRoot( o.go ) == o.go : false );
							//  if ( adapter.pluginID == 0 &&  o.name == "Blob" ) Debug.Log( "ASD" );
						}
					}

					// if ( adapter.pluginID == 0 &&  o.name == "Blob" ) Debug.Log( lastBgRect.Value +  " "  + SelectionRect.Value );
					//Debug.Log	(lastBgRect.Value + " " + SelectionRect.Value);
					new_child_cache_dic[ id ].SetMax( lastBgRect.Value, SelectionRect.Value, adapter, output );
					// new_child_cache_dic[id].lastObjject = id;
				}
			}

			drawhild = new_child_cache_dic.ContainsKey( id ) && new_child_cache_dic[ id ].wasInit;

			output = DrawColoredBG( drawRect, parent, o, true, needLabel_override, resetFonts,
								   overrideRect: drawhild
								   ? new_child_cache_dic[ id ].ConvertToLocal( SelectionRect.Value, adapter )
								   : null, returnAfterLastRect: returnAfterLastRect ?? false );

			return output;
		}


		internal TempColorClass DrawBackground( Rect? SelectionRect, EditorWindow w, DynamicRect drawRect,
											   HierarchyObject _o, int needLabel_override = 0, bool resetFonts = true )
		{   //if ( _o == null ) return null;

			if ( EVENT.type != EventType.Repaint && EVENT.type != EventType.Layout ) return null;



			/* if ( EVENT.type == EventType.Repaint )
			 {   if ( HighlighterHasKey_IncludeFilters( o.scene, o ).IsTrue( false ) )
			     {   return DrawColoredBG( drawRect, o, o, false, needLabel_override, resetFonts );
			     }
			     else   //if (anyNeedBroadcast)
			     {   var parent = o.parent(adapter);
			         while ( parent != null )
			         {   if ( HighlighterHasKey_IncludeFilters( o.scene, parent ).IsTrue( true ) )
			             {   return DrawColoredBG( drawRect, parent, o, true, needLabel_override, resetFonts );
			                 // break;
			             }
			             parent = parent.parent( adapter );
			         }
			     }
			 }*/
			// Debug.Log( EVENT.type );
			var LAY = EVENT.type == EventType.Layout || !SelectionRect.HasValue ;
			if ( LAY ) _o.ah.MIXINCOLOR = null;

			TempColorClass output = null;
			bool TryToMixOn = false;

			TempColorClass parent_o = null;
			//if (anyNeedBroadcast)
			//  if ( (child_Res = HighlighterHasKey_IncludeFilters( _o.scene, _o )).IsTrue( true ) )
			{
				var parent = _o.parent();

				while ( parent != null )
				{
					if ( (child_Res = HighlighterCache_Colors.HighlighterHasKey_IncludeFilters( parent, NowPrefabIsOpened, this )).IsTrue( true ) )
					{
						if ( !SelectionRect.HasValue || set.HIGHLIGHTER_GROUPING_CHILD_MODE == 0 || child_Res.BroadCast == -1 )
						{
							if ( LAY )
								parent_o = DrawColoredBG( drawRect, parent, _o, true, needLabel_override, resetFonts,
														 returnAfterLastRect: true );
							else parent_o = DrawColoredBG( drawRect, parent, _o, true, needLabel_override, resetFonts );
						}

						else
						{
							var id = parent.id;

							if ( LAY )
								parent_o = __masd123( id, SelectionRect, w, drawRect, _o, needLabel_override, resetFonts,
													 parent, returnAfterLastRect: true );
							else
								parent_o = __masd123( id, SelectionRect, w, drawRect, _o, needLabel_override, resetFonts,
													 parent );
						}

						if ( LAY /*&& (parent_o.HAS_LABEL_COLOR && !parent_o.HAS_BG_COLOR || !parent_o.HAS_LABEL_COLOR && parent_o.HAS_BG_COLOR)*/ )
						{
							TryToMixOn = true;
							break;
						}

						else
						{
							return parent_o;
						}
					}

					if ( parent == parent.parent() ) break;
					parent = parent.parent();
				}
			}

			// if ( TryToMixOn ) Debug.Log( _o.name );
			if ( (child_Res = HighlighterCache_Colors.HighlighterHasKey_IncludeFilters( _o, NowPrefabIsOpened, this )).IsTrue( false ) )
			//  if ( adapter.pluginID == 0 && o.name == "BloodPos" ) Debug.Log( SelectionRect.HasValue + " - " + (adapter.GROUPING_CHILD_MODE == 0) +  " - " +  child_Res.BroadCast );
			{
				if ( !SelectionRect.HasValue || set.HIGHLIGHTER_GROUPING_CHILD_MODE == 0 || child_Res.BroadCast == -1 )
				//  if ( o.name == "Misc" ) Debug.Log( EVENT.type + " " +  DrawColoredBG( drawRect, o, o, false, needLabel_override, resetFonts, returnAfterLastRect: true ).BG_ALIGMENT_LEFT_CONVERTED );
				{
					if ( LAY )
						output = DrawColoredBG( drawRect, _o, _o, false, needLabel_override, resetFonts,
											   returnAfterLastRect: true );
					else output = DrawColoredBG( drawRect, _o, _o, false, needLabel_override, resetFonts );
				}

				else
				{
					var id = _o.id;

					if ( LAY )
						return __masd123( id, SelectionRect, w, drawRect, _o, needLabel_override, resetFonts, _o,
										 returnAfterLastRect: true );
					else output = __masd123( id, SelectionRect, w, drawRect, _o, needLabel_override, resetFonts, _o );
				}


				if ( !TryToMixOn ) return output;

				bool anyChange = false;
				if ( output != null )
				{

					if ( output.HAS_BG_COLOR /* && parent_o.HAS_LABEL_COLOR*/)
					{
						if ( parent_o == null ) parent_o = new TempColorClass();
						TempColorClass.CopyFromTo( CopyType.BG, output, ref parent_o );
						anyChange = true;
					}

					if ( output.HAS_LABEL_COLOR /*&& parent_o.HAS_BG_COLOR*/)
					{
						if ( parent_o == null ) parent_o = new TempColorClass();
						TempColorClass.CopyFromTo( CopyType.LABEL, output, ref parent_o );
						anyChange = true;
					}


				}
				if ( anyChange )
				{
					if ( _o.ah.localTempColor == null ) _o.ah.localTempColor = new TempColorClass().AssignFromList( 0, true );

					// _o.ah.localTempColor.CopyFromList( parent_o.el.list );
					TempColorClass.CopyFromTo( CopyType.BG, parent_o, ref _o.ah.localTempColor );
					TempColorClass.CopyFromTo( CopyType.LABEL, parent_o, ref _o.ah.localTempColor );
					// if ( _o.name == "SpawnPoint" ) Debug.Log( _o.ah.MIXINCOLOR.HAS_LABEL_COLOR );
					return _o.ah.MIXINCOLOR = _o.ah.localTempColor;
				}

				else
				{
					return parent_o;
				}
			}

			return null;
		}

		/*Color white = Color.white;
		internal Color GetColorForIcon(HierarchyObject o)
		{   if (IconColorCacher == null) return white;
		    if (!IconColorCacher.HasKey( o.scene, o )) return white;
		    var get = IconColorCacher.GetValue(o.scene, o);
		    tc.r = get.list[0] / 255f;
		    tc.g = get.list[1] / 255f;
		    tc.b = get.list[2] / 255f;
		    // tc.a = Mathf.Clamp01( get.list[3] / 255f / 2 + 0.5f );
		    tc.a = Mathf.Clamp01( get.list[3] / 255f );
		    return tc;
		}*/







		SwithcerMethodsWrapper __sourceBgColor_fadeBg_swithcer_HASH = null;

		SwithcerMethodsWrapper sourceBgColor_fadeBg_swithcer_HASH {
			get {
				return __sourceBgColor_fadeBg_swithcer_HASH ?? (__sourceBgColor_fadeBg_swithcer_HASH
						= new SwithcerMethodsWrapper( sourceBgColor_fadeBg_swithcer ));
			}
		}

		int sourceBgColor_fadeBg_swithcer( HierarchyObject _o )
		{
			return adapter.hashoveredItem && adapter.hoverID == _o.id && !wse.HIDE_HOVER_BG ? 0 : 1;
		}



		internal struct DrawBgDynamicColorHelperStruct
		{
			internal Color BGCOLOR;
			internal float alpha;

		}

		DynamicColor __DrawBgDynamicColorHelper;

		DynamicColor DrawBgDynamicColorHelper {   // if ( adapter.IsSelected( id ) ) Debug.Log( GUI.GetNameOfFocusedControl() );
			get {
				if ( __DrawBgDynamicColorHelper == null )
				{
					__DrawBgDynamicColorHelper = new DynamicColor() {
						GET = ( currentObject, args ) => {
							var str = (DrawBgDynamicColorHelperStruct)args;
							var BGCOLOR = str.BGCOLOR;


							if ( LocalIsSelected( currentObject ) )
							{
								BGCOLOR.a = (BGCOLOR.a * 200 / 255f);
							}

							else
								if ( adapter.hashoveredItem && adapter.hoverID == currentObject.id && !wse.HIDE_HOVER_BG )
							{
								BGCOLOR.a = (BGCOLOR.a * (adapter.current_DragSelection_List.Count == 0
														  ? 200 / 255f
														  : 200 / 255f));
							}


							if ( !currentObject.Active() )
							{   // LABELCOLOR.a /= 2;
								BGCOLOR.a /= 4;
							}


							// var tempColor = (TempColorClass)args;
							//var BGCOLOR = str.BGCOLOR;
							var sourceBgColor = SourceBGColor.GET(currentObject, null); //currentObject.id
							/*if (LocalIsSelected(currentObject.id))
								{
								// sourceBgColor = Color.Lerp(sourceBgColor, (Color) BGCOLOR, ((Color) BGCOLOR).a);
								sourceBgColor = BGCOLOR;
							   // sourceBgColor.a = 1;
								}
							else */
							sourceBgColor = BGCOLOR;

							sourceBgColor.a *= set.HIGHLIGHTER_COLOR_OPACITY;

							sourceBgColor.a *= str.alpha;

							currentObject.ah.BACKGROUNDEsourceBgColorD = sourceBgColor;

							return sourceBgColor;
						}
					};
				}

				return __DrawBgDynamicColorHelper;
			}
		}




		//





















		internal TempColorClass old_DrawColoredBG( DynamicRect drawRect, HierarchyObject parentObject,
				HierarchyObject currentObject, bool breakIfNoBroadCast, int SKIPLABEL = 0, bool resetFonts = true,
				new_child_struct overrideRect = null, bool returnAfterLastRect = false )
		{
			if ( drawRect != null ) currentObject.ah.BACKGROUNDED = 0;


			if ( parentObject.ah.localTempColor == null )
				parentObject.ah.localTempColor = new TempColorClass().AssignFromList( 0, true );

			var tempColor = parentObject.ah.localTempColor;

			bool anywayelse = true;
			//	loopag:;


			var res = HighlighterCache_Colors.HighlighterHasKey(parentObject, this);

			if ( res.IsTrue( breakIfNoBroadCast ) )
			{
				var ptr = res.id;


				if ( ptr != -1 )
				{
					anywayelse = false;

					//var d = adapter.MOI.des(currentObject.scene);
					//if (ptr > d.GetHash6().Count - 1)
					//{
					//	if (loop)
					//	{
					//		adapter.logProxy.LogWarning("GetHighlighterValue Out Of Range please contact support");
					//	}
					//	loop = true;
					//	cacheDichighlighter.Clear();
					//	goto loopag;
					//}
					//loop = false;
					//// Debug.LogError( ptr + " is out of range " + d.GetHash6().Count );
					//var el = d.GetHash6()[ptr];
					//tempColor.CopyFromList(el.list);
					tempColor.CopyFromList( ref res.data );

					if ( breakIfNoBroadCast && !tempColor.child ) return null;

					//  Debug.Log(ptr + " " + parentObject.name);
				}
			}

			if ( anywayelse ) //tempColor.AssignFromList(_DrawColoredBGEmpty);
			{   // if (!tempColor.HAS_BG_COLOR && !tempColor.HAS_LABEL_COLOR)
				var filter = autoMod.GetFilter(parentObject);

				if ( filter == null || breakIfNoBroadCast && !filter.child ||
						!filter.HAS_BG_COLOR && !filter.HAS_LABEL_COLOR ) return null;

				// filter.OverrideTo(ref tempColor);
				tempColor.CopyFromFilter( filter );
			}

			var BC = tempColor.BGCOLOR;
			var LC = tempColor.LABELCOLOR;
			var BC_C = false;
			var LC_C = false;

			if ( LocalIsSelected( currentObject ) )
			{
				BC.a = (byte)(BC.a / 255f * 80);
				BC_C = true;
			}

			else
				if ( adapter.hashoveredItem && adapter.hoverID == currentObject.id && !wse.HIDE_HOVER_BG )
			{
				BC.a = (byte)(BC.a / 255f * (adapter.current_DragSelection_List.Count == 0 ? 200 : 100));
				BC_C = true;
			}


			if ( !currentObject.Active() )
			{
				LC.a /= 2;
				BC.a /= 2;
				BC_C = true;
				LC_C = true;
			}

			if ( BC_C ) tempColor.BGCOLOR = BC;
			if ( LC_C ) tempColor.LABELCOLOR = LC;



			// if (tempColor.BG_ALIGMENT_LEFT < 2)tempColor.BGCOLOR.a = (byte)(tempColor.BGCOLOR.a / 255f * 200);


			if ( drawRect == null )
			{   /*if (list.Count >= 9 && (list[5] != 0 || list[6] != 0 || list[7] != 0 || list[8] != 0))
				                 {   tempColor.LABELCOLOR.r = (byte)list[5];
				                     tempColor.LABELCOLOR.g = (byte)list[6];
				                     tempColor.LABELCOLOR.b = (byte)list[7];
				                     //  tempColor[1].a = (byte)(Math.Max( 10, list[8] ));
				                     tempColor.LABELCOLOR.a = (byte)list[8];
				
				
				                 }*/

				return tempColor;
			}

			/* var af = Adapter.GET_SKIN().label.fontSize;
			 if ( resetFonts ) Adapter.GET_SKIN().label.fontSize = adapter.oldFOntl;
			 var a = Adapter.GET_SKIN().label.alignment;
			 Adapter.GET_SKIN().label.alignment = TextAnchor.MiddleLeft;*/
			// var c = adapter.oldColl;

			var oc = GUI.color;
			var oldLabelColor = adapter.labelStyle.normal.textColor;


			var colorRect = drawRect.ref_selectionRect;
			colorRect.x -= raw_old_leftpadding;
			/*  if ( adapter.HIGHLIGHTER_LEFT_OVERFLOW != 1 )
				  colorRect.x -= adapter.raw_old_leftpadding;*/

			if ( UnityVersion.UNITY_CURRENT_VERSION < UnityVersion.UNITY_2019_2_0_VERSION )
				colorRect.x += adapter.ha.LEFT_PADDING;

			var BgRect = overrideRect == null ? drawRect.ConvertToBGFromTempColor(tempColor) : overrideRect.LocalRect;

			if ( returnAfterLastRect )
			{
				if ( SKIPLABEL != 2 && tempColor.HAS_BG_COLOR )
				{
					lastBgRect = BgRect;
					return tempColor;
				}

				return tempColor;
			}

			colorRect.x += DEFAULT_ICON_SIZE;
			colorRect.width -= DEFAULT_ICON_SIZE;




			/*** if (tempColor.HAS_BG_COLOR)
			 {   GUI.color *= tempColor.BGCOLOR;
				 // colorRect.x -= 3;
				 //colorRect.x = 0;
				 // colorRect.width = adapter.window().position.width - colorRect.x + 3;
				 // colorRect.width = colorRect.width + 3;
				 //   Adapter. INTERNAL_BOX(drawRect, "");
				 if (SKIPLABEL != 2) DrawGradient(colorRect );  //SKIPLABEL == 2 it never uses
				 GUI.color = oc;
			 }***/
			var al1 = tempColor.BG_ALIGMENT_RIGHT_CONVERTED;
			var al2 = tempColor.BG_ALIGMENT_LEFT_CONVERTED;

			var NEW_NEED_LABEL = al2 == BgAligmentLeft.EndLabel || al2 == BgAligmentLeft.Modules ||
								 al1 == BgAligmentRight.BeginLabel || al1 == BgAligmentRight.Icon;
			NEW_NEED_LABEL = !NEW_NEED_LABEL;

			bool needLabel = false;

			if ( SKIPLABEL != 1 )
			{
#pragma warning disable
#pragma warning restore

				if ( tempColor.HAS_LABEL_COLOR )
				{
					needLabel = true;

					adapter.labelStyle.normal.textColor = tempColor.LABELCOLOR;
				}

				else
				{
					var type = IS_HIERARCHY()
							   ? Prefabs.GetPrefabType(currentObject.go)
							   : PrefabInstanceStatus.NotAPrefab;

					// if (!(type == PrefabType.None || type == PrefabType.DisconnectedModelPrefabInstance || type == PrefabType.DisconnectedPrefabInstance))
					if ( !(type == PrefabInstanceStatus.NotAPrefab || type == PrefabInstanceStatus.Disconnected) )
					{
						var co = EditorGUIUtility.isProSkin ? prefabColorPro : prefabColorPersonal;

						if ( !currentObject.Active() ) co.a /= 2;

						adapter.labelStyle.normal.textColor = co;
						needLabel = NEW_NEED_LABEL;
					}

					else
						if ( (type == PrefabInstanceStatus.MissingAsset) )
					{
						var co = EditorGUIUtility.isProSkin ? prefabMissinColorPro : prefabMissinColorPersonal;

						if ( !currentObject.Active() ) co.a /= 2;

						adapter.labelStyle.normal.textColor = co;
						needLabel = NEW_NEED_LABEL;
					}

					else
					// {   Adapter.GET_SKIN().label.normal.textColor = parentObject.cache_prefab ? m_PrefabColorPro : adapter.oldColl;
					{
						adapter.labelStyle.normal.textColor = oldLabelColor;
					}
				}
			}


			/*   oc = GUI.contentColor;
			   contentColor.a = list[3] / 255f * 90;
			   contentColor.a /= 2;
			   GUI.contentColor = contentColor;*/


			var sourceBgColor = SourceBGColor.GET(currentObject, null);

			if ( ( /***currentObject.Active() ||***/ needLabel) && SKIPLABEL != 1 )
			{   /***  if (SKIPLABEL != 2 && tempColor.HAS_BG_COLOR) //SKIPLABEL == 2 it never uses
				  {
				   sourceBgColor =  Color.Lerp( sourceBgColor,  (Color)tempColor.BGCOLOR,  ((Color)tempColor.BGCOLOR).a) ;
				      sourceBgColor.a = 1;
				  }***/
				/**/
				if ( !adapter.hashoveredItem || adapter.hoverID == currentObject.id && !wse.HIDE_HOVER_BG )
				{
					var _oc = adapter.labelStyle.normal.textColor;
					//var _oc = adapter.oldColl;
					adapter.labelStyle.normal.textColor = sourceBgColor;
					//  colorRect.x -= 0.5f;
					var D = 0.75f;
					var O = 0.75f;
					colorRect.y += O;
					colorRect.y -= D;
					GUI.Label( colorRect, currentObject.name, adapter.labelStyle );
					// colorRect.x += 1;
					colorRect.y += D * 2;
					GUI.Label( colorRect, currentObject.name, adapter.labelStyle );
					colorRect.y -= D;

					colorRect.x -= D;
					GUI.Label( colorRect, currentObject.name, adapter.labelStyle );
					// colorRect.x += 1;
					colorRect.x += D * 2;
					GUI.Label( colorRect, currentObject.name, adapter.labelStyle );
					colorRect.x -= D;
					GUI.Label( colorRect, currentObject.name, adapter.labelStyle );
					colorRect.y -= O;
					// colorRect.x -= 0.5f;
					//  colorRect.x += 1;
					//GUI.Label( colorRect, currentObject.name );
					adapter.labelStyle.normal.textColor = _oc;
				}

				else
				{
					GUI.color *= sourceBgColor;
					GUI.DrawTexture( colorRect, Texture2D.whiteTexture );
					//GUI.Box(colorRect, "");
					// DrawGradient( colorRect );
					GUI.color = oc;
				}
			}

			if ( SKIPLABEL != 2 )
				if ( tempColor.HAS_BG_COLOR )
				{
					if ( LocalIsSelected( currentObject ) )
					{
						sourceBgColor = Color.Lerp( sourceBgColor, (Color)tempColor.BGCOLOR,
												   ((Color)tempColor.BGCOLOR).a );
						sourceBgColor.a = 1;
					}

					else sourceBgColor = tempColor.BGCOLOR;

					sourceBgColor.a *= set.HIGHLIGHTER_COLOR_OPACITY;
					//GUI.color *= sourceBgColor;
					GUI.color = oc;
					lastBgRect = BgRect;


					if ( overrideRect != null ) GUI.BeginClip( overrideRect.ModifiedSelectionRect( adapter ) );

					DrawGradient( BgRect, tempColor, drawRect.isMain, SourceBGColor, currentObject );

					if ( overrideRect != null ) GUI.EndClip();

					needLabel = NEW_NEED_LABEL;


					var LEFT_BG = overrideRect != null
								  ? overrideRect.ModifiedSelectionRect(adapter).x + BgRect.x
								  : BgRect.x;
					var LEFT_LABEL = colorRect.x;

					LEFT_LABEL -= DEFAULT_ICON_SIZE + 16;

					/*  var RIGHT_BG = LEFT_BG + BgRect.width ;
					  var RIGHT_LABEL = colorRect.x + colorRect.width;
					  if ( overrideRect != null )
					  {   Debug.Log( RIGHT_BG + " " + RIGHT_LABEL );
					  }*/
					/*if ( tempColor.BG_ALIGMENT_LEFT < 2 && tempColor.BG_ALIGMENT_RIGHT != 4 )
						currentObject.BACKGROUNDEsourceBgColorD = sourceBgColor;
					else
						currentObject.BACKGROUNDEsourceBgColorD = Color.black;*/
					currentObject.ah.BACKGROUNDEsourceBgColorD = sourceBgColor;

					var LEFT_SKIP = LEFT_LABEL < LEFT_BG;

					if ( tempColor.BG_ALIGMENT_RIGHT_CONVERTED == BgAligmentRight.BeginLabel ||
							tempColor.BG_ALIGMENT_RIGHT_CONVERTED == BgAligmentRight.Icon )
						LEFT_SKIP |= LEFT_LABEL > LEFT_BG + BgRect.width;

					// var RIGHT_SKIP =  RIGHT_LABEL > RIGHT_BG;
					var RIGHT_SKIP = false;

					if ( tempColor.BG_ALIGMENT_LEFT_CONVERTED == BgAligmentLeft.Modules ) RIGHT_SKIP = true;

					if ( !RIGHT_SKIP ) currentObject.ah.FLAGS |= 1;

					var worldRect = BgRect;

					if ( overrideRect != null )
					{
						worldRect.x += overrideRect.ModifiedSelectionRect( adapter ).x;
						worldRect.y += overrideRect.ModifiedSelectionRect( adapter ).y;
					}

					currentObject.ah.BG_RECT = worldRect;
					// || colorRect.x > X
					/* if ( overrideRect != null  && colorRect.x > X )
					 {   currentObject.FLAGS |= 1;
					 }*/
					currentObject.ah.BACKGROUNDED =
						(tempColor.BG_HEIGHT == 2 && overrideRect == null || LEFT_SKIP) ? 2 : 1;
				}


			if ( (needLabel || tempColor.HAS_LABEL_COLOR) && SKIPLABEL != 1 )
			{
				/* var p = Adapter.GET_SKIN().label.padding;
				 var left = p.left;
				 var right = p.right;
				 var bottom = p.bottom;
				 var top = p.top;
				 p.bottom = p.top = p.left = p.right = 0;
				 Adapter.GET_SKIN().label.fontSize = Adapter.GET_SKIN().textArea.fontSize;*/

				if ( !HAS_LABEL_ICON() )
				{
					colorRect.x -= 2;
				}

				//  Adapter.GET_SKIN().label.fontStyle = FontStyle.Italic;
				var labelRect = colorRect;

				if ( UnityVersion.UNITY_CURRENT_VERSION < UnityVersion.UNITY_2019_3_0_VERSION && wse.USE_LINE_HEIGHT )
				{
					if ( UnityVersion.UNITY_CURRENT_VERSION < UnityVersion.UNITY_2019_VERSION )
					{
						if ( Mathf.RoundToInt( wse.LINE_HEIGHT ) % 2 == 1 )
							labelRect.y = Mathf.CeilToInt( labelRect.y - 0.1f );
						else
							labelRect.y = Mathf.CeilToInt( labelRect.y + 0.1f );
					}

					else
					{
						if ( Mathf.RoundToInt( wse.LINE_HEIGHT ) % 2 != 1 ) labelRect.y -= 1;
					}
				}


				if ( !currentObject.Active() )
				{
					oldGuiColor = GUI.color;
					GUI.color *= gcn;
				}

				if ( tempColor != null && tempColor.HAS_LABEL_COLOR && tempColor.LABEL_SHADOW == true )
				{
					var _oc2 = adapter.labelStyle.normal.textColor;
					// var c2 = adapter.oldColl;
					var c2 = Color.black;
					c2.a = _oc2.a;
					adapter.labelStyle.normal.textColor = c2;

					if ( UnityVersion.UNITY_CURRENT_VERSION < UnityVersion.UNITY_2019_3_0_VERSION )
					{
						labelRect.y -= 0.5f; //0.5f;
						labelRect.x -= 1; //0.5f;


						GUI.Label( labelRect, currentObject.name, adapter.labelStyle );
						labelRect.y += 0.5f; // 0.5f;
						labelRect.x += 1; // 0.5f;
					}

					else
					{
						labelRect.y -= 1f; //0.5f;
						labelRect.x -= 1; //0.5f;
						GUI.Label( labelRect, currentObject.name, adapter.labelStyle );
						labelRect.x -= 1f; //0.5f;
						GUI.Label( labelRect, currentObject.name, adapter.labelStyle );
						labelRect.y += 1; // 0.5f;
						labelRect.x += 2; // 0.5f;
					}

					// var fs = adapter.labelStyle.fontStyle;
					// adapter.labelStyle.fontStyle = FontStyle .Bold;
					// adapter.labelStyle.fontStyle = fs;
					adapter.labelStyle.normal.textColor = _oc2;
				}

				// colorRect.y = Mathf.CeilToInt(colorRect.y);
				/* var h = labelRect.height - GUI.skin.label.CalcHeight(Adapter.GET_CONTENT(currentObject.name), 10000);
				 labelRect.y += h / 2;
				 labelRect.height -= h;*/
				GUI.Label( labelRect, currentObject.name, adapter.labelStyle );


				if ( !currentObject.Active() ) GUI.color = oldGuiColor;

				// Adapter.GET_SKIN().label.fontStyle = FontStyle.Normal;
				/*  if (!EditorGUIUtility.isProSkin && (Adapter.GET_SKIN().label.normal.textColor.r + Adapter.GET_SKIN().label.normal.textColor.g + Adapter.GET_SKIN().label.normal.textColor.b) / 3 > 0.5f)
					  GUI.Label( colorRect, currentObject.name );*/

				/*  p.left = left;
				  p.right = right;
				  p.bottom = bottom;
				  p.top = top;*/
			}

			// GUI.contentColor = oc;
			//                 Adapter.GET_SKIN().label.alignment = a;
			//                 Adapter.GET_SKIN().label.fontSize = af;
			adapter.labelStyle.normal.textColor = oldLabelColor;

			return tempColor;
		}



	}
}
