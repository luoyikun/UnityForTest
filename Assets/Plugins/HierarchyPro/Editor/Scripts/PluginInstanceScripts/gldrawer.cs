//#define DEBUG_THIS
#define ONLY_REPAINT
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor
{

	class GlDrawer
	{
		PluginInstance p;
		internal GlDrawer( PluginInstance p )
		{
			this.p = p;
			drawCalls = new DrawCall[ 2 ] { new DrawCall( this ), new DrawCall( this ) };
		}


		//int capturedHoverID = -1;
		internal bool HARD_BAKE_ENABLED {
			get {
				return !PluginInstance.KeepLastActiveGameObbject && !disableBatching &&
#if ONLY_REPAINT
			p.EVENT.type == EventType.Repaint &&
#endif
				p.par_e.USE_DINAMIC_BATCHING;
			}
		}

		public void DrawStackItem( DrawStack drawStack )
		{
			////old draer

			//  wasRest = null;
			dc = drawStack.drawCallIndex;
			BAKE_DRAWER = HARD_BAKE_ENABLED;
			//var use_hover = p.par_e.USE_HOVER_FOR_BUTTONS;
			//var type = Event.current.type;
			//Debug.Log( Event.current.type );
			for ( int i = 0; i < drawStack.Count; i++ )
			{
				//if ( use_hover )
				//{
				//   // Debug.Log( Event.current.type );
				//    if ( type == EventType.MouseMove && 
				//		(drawStack.stack[i].type == ModulesDrawType.ModuleButton || drawStack.stack[i].type == ModulesDrawType.SimpleButton) 
				//		&& capturedHoverID != EditorGUIUtility.GetControlID( FocusType.Passive ) )
				//    {
				//        capturedHoverID = EditorGUIUtility.GetControlID( FocusType.Passive );
				//        Debug.Log( capturedHoverID );
				//    }
				//}
				drawStack.DrawSIngleItem( drawStack.stack[ i ] );
			}
			BAKE_DRAWER = false;


			/*var o = drawStack.current_object;
			for ( int i = 0 ; i <drawStack.Count ; i++ )
			{
				drawStack.stack[ i ].type
				///DrawSIngleItem( stack[ i ], o );
			}

			drawStack
			throw new NotImplementedException();*/
		}
		internal bool BAKE_DRAWER = false;
		public bool disableBatching = false;
		internal void ReleaseStack()
		{
			BAKE_DRAWER = false;
			drawCalls[ 0 ].ReleaseStack();
			if ( drawFade )
			{
				drawFade = false;
				EditorGUI.DrawRect( fadeRect, fadeColor );
			}
			drawCalls[ 1 ].ReleaseStack();
		}
		internal void ButtonsEvents()
		{
			//drawCalls[ 0 ].ButtonsEvents();
			//drawCalls[ 1 ].ButtonsEvents();
		}
		class DrawCall
		{
			GlDrawer drawer;
			internal DrawCall( GlDrawer drawer )
			{
				this.drawer = drawer;
			}
			internal void ReleaseStack()
			{
#if ONLY_REPAINT
				if ( drawer.p.EVENT.type == EventType.Repaint )
#endif
				{
					for ( int i = 0; i < style_stack.glCount; i++ )
					{
						if ( style_stack.glStack[ i ].clipRect.HasValue )
						{
							var startPos = i;
							var endPos = i;
							var compareRect = style_stack.glStack[ i ].clipRect.Value;
							for ( int t = startPos; t < style_stack.glCount; t++ )
							{
								endPos++;
								/*&& compareRect.x == style_stack.glStack[ t ].clipRect.Value.x &&
                                  compareRect.width == style_stack.glStack[ t ].clipRect.Value.width*/
								if ( style_stack.glStack[ t ].clipRect.HasValue )
								{
									if ( compareRect.x > style_stack.glStack[ t ].clipRect.Value.x )
									{
										var dif = compareRect.x - style_stack.glStack[ t ].clipRect.Value.x;
										compareRect.width += dif;
										compareRect.x = style_stack.glStack[ t ].clipRect.Value.x;
									}
									if ( compareRect.x + compareRect.width < style_stack.glStack[ t ].clipRect.Value.x + style_stack.glStack[ t ].clipRect.Value.width )
									{
										compareRect.width = style_stack.glStack[ t ].clipRect.Value.x + style_stack.glStack[ t ].clipRect.Value.width - compareRect.x;
									}
									if ( compareRect.y > style_stack.glStack[ t ].clipRect.Value.y )
									{
										var dif = compareRect.y - style_stack.glStack[ t ].clipRect.Value.y;
										compareRect.height += dif;
										compareRect.y = style_stack.glStack[ t ].clipRect.Value.y;
									}
									if ( compareRect.y + compareRect.height < style_stack.glStack[ t ].clipRect.Value.y + style_stack.glStack[ t ].clipRect.Value.height )
									{
										compareRect.height = style_stack.glStack[ t ].clipRect.Value.y + style_stack.glStack[ t ].clipRect.Value.height - compareRect.y;
									}
								}
								else
								{
									break;
								}
							}
							//  if ( compareRect.height < 20 )
							//      Debug.Log( (endPos - startPos) + " " + compareRect );
							//  startPos = i;
							//  endPos = i;
							//  compareRect = style_stack.glStack[ i ].clipRect.Value;

							GUI.BeginClip( compareRect );

							for ( ; i < endPos; i++ )
							{
								drawer._DrawStyleWithText( ref style_stack.glStack[ i ], compareRect );
							}
							GUI.EndClip();
						}
						else
						{
							drawer._DrawStyleWithText( ref style_stack.glStack[ i ] );
						}
					}
					style_stack.glCount = 0;
					//
					if ( unsorted_glTexture_stacks.Count > temp_stack_a.Length ) Array.Resize( ref temp_stack_a, unsorted_glTexture_stacks.Count );
					foreach ( var item in unsorted_glTexture_stacks ) temp_stack_a[ item.Value.tex_sorted_index ] = item.Value;

					var nulltexture = -1;
					for ( int i = 0; i < unsorted_glTexture_stacks.Count; i++ ) if ( temp_stack_a[ i ].emptyTexture == -1 ) nulltexture = i;
					if ( nulltexture > 0 )
					{
						var temp = temp_stack_a[nulltexture];
						for ( int i = nulltexture; i > 0; i-- ) temp_stack_a[ i ] = temp_stack_a[ i - 1 ];
						temp_stack_a[ 0 ] = temp;
					}

					//int count  = 0;
					for ( int i = 0, L = unsorted_glTexture_stacks.Count; i < L; i++ )
					{
						var S = temp_stack_a[i];
						//Debug.Log(S.glStack.Count);
						if ( S.glStack.Count > temp_stack_b.Length ) Array.Resize( ref temp_stack_b, S.glStack.Count );
						foreach ( var item in S.glStack ) temp_stack_b[ item.Value.mat_sorted_index ] = item.Value;
						for ( int x = 0, L2 = S.glStack.Count; x < L2; x++ )
						{
							var R = temp_stack_b[x];
							var t = R.tex != null ? R.tex.texture : null;
							if ( x == 0 )
							{
								if ( !R.mat )
								{
									//  Debug.LogWarning("No material " + R.tex);
									ClearStacks();
									Root.p[ 0 ].RESET_DRAWSTACK( Root.p[ 0 ].pluginID );
									Root.p[ 0 ].RepaintWindow( Root.p[ 0 ].pluginID, true );
									return;
								}
							}
							/*if (i == 0)
                            {
                                additionalMaterial().SetTexture(_MainTex, t);
                                additionalMaterial().SetPass(0);
                                //break;
                            }
                            else
                            {
                                R.mat.SetTexture(_MainTex, t);
                                R.mat.SetPass(0);
                            }*/
							R.mat.SetTexture( _MainTex, t );
							R.mat.SetPass( 0 );

							GL.PushMatrix();
							GL.Begin( GL.QUADS );

							for ( int z = 0; z < R.glCount; z++ )
							{
								if ( R.glStack[ z ].border <= 0 || !t )
								{
									if ( R.glStack[ z ].clipRect.HasValue )
									{
										drawer.draw_simple_quad_fast_clipRect = R.glStack[ z ].clipRect;
										drawer.draw_simple_quad_fast( R.glStack[ z ].rect, ref R.glStack[ z ].col, R.glStack[ z ].tex, true );
									}
									else
									{
										drawer.draw_simple_quad_fast( R.glStack[ z ].rect, ref R.glStack[ z ].col, R.glStack[ z ].tex );
									}
								}
								else if ( R.glStack[ z ].clipRect.HasValue )
									drawer.__DrawTexture( ref R.glStack[ z ].rect, R.glStack[ z ].tex, ref R.glStack[ z ].col, R.glStack[ z ].border, R.mat, ref R.glStack[ z ].clipRect, fast: true );
								else
									drawer.__DrawTexture( ref R.glStack[ z ].rect, R.glStack[ z ].tex, ref R.glStack[ z ].col, R.glStack[ z ].border, R.mat, ref drawer.nullClip, fast: true );
							}
							R.glCount = 0;
							//  count++;
							GL.End();
							GL.PopMatrix();
							//GL.Clear(false, true,Color.clear);

						}
					}
					// Debug.Log(count);
					//
					for ( int i = 0; i < label_stack.glCount; i++ ) drawer._DrawLabel( ref label_stack.glStack[ i ] );
					label_stack.glCount = 0;
				}
				if (
#if ONLY_REPAINT
					drawer.p.EVENT.type == EventType.Repaint
#endif
					// drawer.p.EVENT.type == EventType.MouseDown ||
					// drawer.p.EVENT.type == EventType.MouseUp ||
					// drawer.p.EVENT.isMouse
					)
				{
					for ( int i = 0; i < button_stack.glCount; i++ ) drawer._DrawButton( ref button_stack.glStack[ i ] );
					button_stack.glCount = 0;
				}

			}

			internal void ButtonsEvents()
			{
				if (
					drawer.p.EVENT.type == EventType.MouseDown ||
				  drawer.p.EVENT.type == EventType.MouseUp ||
				  drawer.p.EVENT.isMouse
				  )
				{
					for ( int i = 0; i < button_stack.glCount; i++ ) drawer._DrawButton( ref button_stack.glStack[ i ] );
					//button_stack.glCount = 0;
				}
			}

			internal GLTextureStack[] temp_stack_a = new GLTextureStack[20];
			internal GLTextureStackAndMaterial[] temp_stack_b = new GLTextureStackAndMaterial[20];
			internal Dictionary<int, GLTextureStack> unsorted_glTexture_stacks = new Dictionary<int, GLTextureStack>() { { -1, new GLTextureStack(0) } };
			internal GLLabelStack label_stack = new GLLabelStack();
			internal GLButtonStack button_stack = new GLButtonStack();
			internal GLStyleStack style_stack = new GLStyleStack();


			internal void ClearStacks()
			{
				foreach ( var item in unsorted_glTexture_stacks )
					item.Value.glStack.Clear();
				style_stack.glCount = 0;
				label_stack.glCount = 0;
				button_stack.glCount = 0;

			}
		}
		internal void ClearStacks()
		{
			foreach ( var item in drawCalls )
			{
				if ( item == null ) continue;
				item.ClearStacks();
			}
		}

		int dc = 0;
		/*  int dc = -1;
          internal void SET_DRAW_CALL()
          {
              dc++;
              if (dc >= drawCalls.Length) throw new Exception("dc");
          }*/
		DrawCall[] drawCalls;

		bool drawFade = false;
		Rect fadeRect;
		Color fadeColor;
		internal void DrawFade( Rect r, Color c )
		{
			drawFade = true;
			fadeRect = r;
			fadeColor = c;
		}


		static int _MainTex = Shader.PropertyToID("_MainTex");
		static int colorProperty = Shader.PropertyToID("_Color");
		Material _defaultMaterial;
		Material defaultMaterial()
		{
			if ( !_defaultMaterial )
				_defaultMaterial = new Material( p.HL_SET.DEFAULT_SHADER_SHADER.ExternalMaterialReference );
			return _defaultMaterial;
		}
		static Material _additionalMaterial;
		static Material additionalMaterial()
		{
			if ( !_additionalMaterial )
				_additionalMaterial = new Material( Root.p[ 0 ].HL_SET.SHADER_B.ExternalMaterialReference );
			return _additionalMaterial;
		}
		Vector3 tv3_a;
		Vector3 tv3_b;
		Vector3 tv3_c;
		Vector3 tv3_d;

		void DoClip( ref Rect rect, ref Rect? clipRect )
		{
			var oX = rect.x;
			var oY = rect.y;
			if ( rect.x < clipRect.Value.x ) rect.x = clipRect.Value.x;
			if ( rect.x > clipRect.Value.x + clipRect.Value.width ) rect.x = clipRect.Value.x + clipRect.Value.width;
			if ( rect.y < clipRect.Value.y ) rect.y = clipRect.Value.y;
			if ( rect.y > clipRect.Value.y + clipRect.Value.height ) rect.y = clipRect.Value.y + clipRect.Value.height;
			rect.width = Mathf.Max( 0, rect.width - (rect.x - oX) );
			rect.height = Mathf.Max( 0, rect.height - (rect.y - oY) );
			if ( rect.width + rect.x > clipRect.Value.x + clipRect.Value.width ) rect.width = Mathf.Max( 0, clipRect.Value.x + clipRect.Value.width - rect.x );
			if ( rect.height + rect.y > clipRect.Value.y + clipRect.Value.height ) rect.height = Mathf.Max( 0, clipRect.Value.y + clipRect.Value.height - rect.y );

		}

		void DoClip( ref Rect rect, Vector2 uv_start, Vector2 uv_end, ref Rect? clipRect )
		{

			var or = rect;
			DoClip( ref rect, ref clipRect );

			if ( or.xMax != rect.xMax || or.xMin != rect.xMin )
			{
				var d = or.xMax - or.xMin;
				var l_A = (rect.xMin - or.xMin) / d;
				var l_B = (rect.xMax - or.xMin) / d;
				uv_start.x = Mathf.LerpUnclamped( uv_start.x, uv_end.x, l_A );
				uv_end.x = Mathf.LerpUnclamped( uv_start.x, uv_end.x, l_B );
			}
			if ( or.yMax != rect.yMax || or.yMin != rect.yMin )
			{
				var d = or.yMax - or.yMin;
				var l_A = (rect.yMin - or.yMin) / d;
				var l_B = (rect.yMax - or.yMin) / d;
				uv_start.y = Mathf.LerpUnclamped( uv_start.y, uv_end.y, l_A );
				uv_end.y = Mathf.LerpUnclamped( uv_start.y, uv_end.y, l_B );
			}

			tv3_a.Set( uv_start.x, uv_start.y, 0 );
			tv3_b.Set( uv_start.x, uv_end.y, 0 );
			tv3_c.Set( uv_end.x, uv_end.y, 0 );
			tv3_d.Set( uv_end.x, uv_start.y, 0 );

		}


		void draw_simple_quad( Rect rect, ref Color c, IconData tex = null, Rect? clipRect = null )
		{
			if ( rect.width <= 0 || rect.height <= 0 ) return;


			//  p.par_e.DEFAULT_SHADER_SHADER.ExternalMaterialReference.SetPass( 0 );
			GL.PushMatrix();
			///  GL.Clear(true, false, Color.black);
			GL.Begin( GL.QUADS );

			GL.Color( c );






			if ( tex != null )
			{

				if ( clipRect.HasValue )
				{
					//  Debug.Log( clipRect );
					DoClip( ref rect, tex.uv_start, tex.uv_end, ref clipRect );
				}
				else
				{
					tv3_a.Set( tex.uv_start.x, tex.uv_start.y, 0 );
					tv3_b.Set( tex.uv_start.x, tex.uv_end.y, 0 );
					tv3_c.Set( tex.uv_end.x, tex.uv_end.y, 0 );
					tv3_d.Set( tex.uv_end.x, tex.uv_start.y, 0 );
				}


				GL.TexCoord( tv3_a );
				GL.Vertex3( rect.x, rect.y, 0 );
				GL.TexCoord( tv3_b );
				GL.Vertex3( rect.x, rect.y + rect.height, 0 );
				GL.TexCoord( tv3_c );
				GL.Vertex3( rect.x + rect.width, rect.y + rect.height, 0 );
				GL.TexCoord( tv3_d );
				GL.Vertex3( rect.x + rect.width, rect.y, 0 );
			}
			else
			{
				if ( clipRect.HasValue ) DoClip( ref rect, ref clipRect );

				GL.Vertex3( rect.x, rect.y, 0 );
				GL.Vertex3( rect.x, rect.y + rect.height, 0 );
				GL.Vertex3( rect.x + rect.width, rect.y + rect.height, 0 );
				GL.Vertex3( rect.x + rect.width, rect.y, 0 );
			}


			GL.End();
			GL.PopMatrix();
		}
		Rect? draw_simple_quad_fast_clipRect = null;
		void draw_simple_quad_fast( Rect rect, ref Color c, IconData tex = null, bool clip = false )
		{
			if ( rect.width <= 0 || rect.height <= 0 ) return;
			GL.Color( c );





			if ( tex != null )
			{

				if ( clip )
				{
					//var oX = rect.x;
					//var oY = rect.y;
					//if (rect.x < clipRect.Value.x) rect.x = clipRect.Value.x;
					//if (rect.x > clipRect.Value.x + clipRect.Value.width) rect.x = clipRect.Value.x + clipRect.Value.width;
					//if (rect.y < clipRect.Value.y) rect.y = clipRect.Value.y;
					//if (rect.y > clipRect.Value.y + clipRect.Value.height) rect.y = clipRect.Value.y + clipRect.Value.height;
					//rect.width = Mathf.Max(0, rect.width - (rect.x - oX));
					//rect.height = Mathf.Max(0, rect.height - (rect.y - oY));
					//if (rect.width + rect.x > clipRect.Value.x + clipRect.Value.width) rect.width = Mathf.Max(0, clipRect.Value.x + clipRect.Value.width - rect.x);
					//if (rect.height + rect.y > clipRect.Value.y + clipRect.Value.height) rect.height = Mathf.Max(0, clipRect.Value.y + clipRect.Value.height - rect.y);
					DoClip( ref rect, tex.uv_start, tex.uv_end, ref draw_simple_quad_fast_clipRect );
				}
				else
				{
					tv3_a.Set( tex.uv_start.x, tex.uv_start.y, 0 );
					tv3_b.Set( tex.uv_start.x, tex.uv_end.y, 0 );
					tv3_c.Set( tex.uv_end.x, tex.uv_end.y, 0 );
					tv3_d.Set( tex.uv_end.x, tex.uv_start.y, 0 );
				}

				//Debug.Log(c);

				//	tv3.Set(tex.uv_start.x, tex.uv_start.y, 0);
				GL.TexCoord( tv3_a );
				GL.Vertex3( rect.x, rect.y, 0 );
				//tv3.Set(tex.uv_start.x, tex.uv_end.y, 0);
				GL.TexCoord( tv3_b );
				GL.Vertex3( rect.x, rect.y + rect.height, 0 );
				//tv3.Set(tex.uv_end.x, tex.uv_end.y, 0);
				GL.TexCoord( tv3_c );
				GL.Vertex3( rect.x + rect.width, rect.y + rect.height, 0 );
				//tv3.Set(tex.uv_end.x, tex.uv_start.y, 0);
				GL.TexCoord( tv3_d );
				GL.Vertex3( rect.x + rect.width, rect.y, 0 );
			}
			else
			{
				if ( clip ) DoClip( ref rect, ref draw_simple_quad_fast_clipRect );

				GL.Vertex3( rect.x, rect.y, 0 );
				GL.Vertex3( rect.x, rect.y + rect.height, 0 );
				GL.Vertex3( rect.x + rect.width, rect.y + rect.height, 0 );
				GL.Vertex3( rect.x + rect.width, rect.y, 0 );
			}
		}


		void draw_uvved_quad( float start_x, float start_y, float rect_width_x, float rect_height_y, float _v1x, float _v1y, float uv_width, float uv_height, ref Rect? clipRect )
		{



			uv_height = -uv_height;
			var end_x = start_x + rect_width_x;
			var end_y = start_y + rect_height_y;
			var _v2x = _v1x + uv_width;
			var _v2y = _v1y + uv_height;
			//start_x = Mathf.RoundToInt( start_x );
			//start_y = Mathf.RoundToInt( start_y );
			//end_x = Mathf.RoundToInt( end_x );
			//end_y = Mathf.RoundToInt( end_y );
			start_x = (int)(start_x);
			//start_y = (int)( start_y );
			end_x = (int)(end_x);
			//end_y = (int)( end_y );

			if ( 0 == rect_height_y ) return;
			if ( 0 == rect_width_x ) return;

			/*_v1x = Mathf.RoundToInt( _v1x );
			_v1y = Mathf.RoundToInt( _v1y );
			_v2x = Mathf.RoundToInt( _v2x );
			_v2y = Mathf.RoundToInt( _v2y );*/

			/// YOU MAY COMMENT IT, ITS SIMPLY JUST A VISUAL DEBUGING
			if ( _v1x < 0 || _v1y < 0 || _v2x < 0 || _v2y < 0 ||
				_v1x > 1 || _v1y > 1 || _v2x > 1 || _v2y > 1 || rect_width_x < 0 || rect_height_y < 0 ) throw new Exception(
					 _v1x + " " + _v1y + " " + _v2x + " " + _v2y + " " +
				 _v1x + " " + _v1y + " " + _v2x + " " + _v2y + " " + rect_width_x + " " + rect_height_y );

			if ( clipRect.HasValue )
			{
				//if (start_x < clipRect.Value.x) start_x = clipRect.Value.x;
				//if (start_x > clipRect.Value.x + clipRect.Value.width) start_x = clipRect.Value.x + clipRect.Value.width;
				//if (start_y < clipRect.Value.y) start_y = clipRect.Value.y;
				//if (start_y > clipRect.Value.y + clipRect.Value.height) start_y = clipRect.Value.y + clipRect.Value.height;
				//if (end_x < clipRect.Value.x) end_x = clipRect.Value.x;
				//if (end_x > clipRect.Value.x + clipRect.Value.width) end_x = clipRect.Value.x + clipRect.Value.width;
				//if (end_y < clipRect.Value.y) end_y = clipRect.Value.y;
				//if (end_y > clipRect.Value.y + clipRect.Value.height) end_y = clipRect.Value.y + clipRect.Value.height;
				//Debug.Log(start_x + " " + start_y + " " + end_x + " " + end_y + " --- " + clipRect);

				var r = new Rect(start_x, start_y, rect_width_x, rect_height_y);
				DoClip( ref r, new Vector2( _v1x, _v1y ), new Vector2( _v2x, _v2y ), ref clipRect );
				start_x = r.x;
				start_y = r.y;
				end_x = start_x + r.width;
				end_y = start_y + r.height;

				{
					//	Debug.Log(start_x + " " + start_y + " " + end_x + " " + end_y + " ||| " + clipRect);

				}
			}
			else
			{
				tv3_a.Set( _v1x, _v1y, 0 );
				tv3_b.Set( _v1x, _v2y, 0 );
				tv3_c.Set( _v2x, _v2y, 0 );
				tv3_d.Set( _v2x, _v1y, 0 );


				//tv3_a.Set(tex.uv_start.x, tex.uv_start.y, 0);
				//tv3_b.Set(tex.uv_start.x, tex.uv_end.y, 0);
				//tv3_c.Set(tex.uv_end.x, tex.uv_end.y, 0);
				//tv3_d.Set(tex.uv_end.x, tex.uv_start.y, 0);
			}


			GL.TexCoord( tv3_a );
			GL.Vertex3( start_x, start_y, 0 );

			GL.TexCoord( tv3_b );
			GL.Vertex3( start_x, end_y, 0 );

			GL.TexCoord( tv3_c );
			GL.Vertex3( end_x, end_y, 0 );

			GL.TexCoord( tv3_d );
			GL.Vertex3( end_x, start_y, 0 );
		}


		Rect? nullClip = null;
		void __DrawTexture( ref Rect rect, IconData tex, ref Color col, float border, Material mat, ref Rect? clipRect, bool fast = false )
		{


			/*	var asd = rect;
                asd.x -= 20;
                asd.width = asd.height;
                defaultMaterial().SetTexture( _MainTex, tex.texture );
                defaultMaterial().SetPass( 0 );
                draw_simple_quad( ref asd, ref col, tex );*/
			if ( !fast )
			{
				//Debug.Log(mat.shader);
				mat.SetTexture( _MainTex, tex.texture );
				mat.SetPass( 0 );
				GL.PushMatrix();
				GL.Begin( GL.QUADS );
			}

			GL.Color( col );

			//rect.y += p.scrollPos.y % 1;
			//	rect.y = Mathf.FloorToInt(rect.y);
			//if ( clipRect.HasValue )
			//{
			//	var r=  clipRect.Value;
			//	r.y += p.scrollPos.y % 1;
			//	r.y = (int)Mathf.FloorToInt(r.y);
			//	clipRect = r;
			//}

			if ( border == 0 )
			{
				//Debug.Log(clipRect);
				//defaultMaterial().SetTexture(_MainTex, tex.texture);
				//defaultMaterial().SetPass(0);
				draw_simple_quad( rect, ref col, tex, clipRect );
			}
			else
			{
				////	var max_w = (tex.uv_end.x - tex.uv_start.x) * tex.texture.width / 2;
				var max_w = Mathf.FloorToInt(tex.width / 2);
				//var dif_w = border - Math.Min(border, max_w);
				var border_x = Math.Min(border, max_w);
				max_w = Mathf.FloorToInt( rect.width / 2 );
				//dif_w += border_x - Math.Min( border_x, max_w);
				border_x = Math.Min( border_x, max_w );

				var max_h = Mathf.FloorToInt(tex.height / 2);
				//var dif_h = border - Math.Min(border, max_h);
				var border_y = Math.Min(border, max_h);
				max_h = Mathf.FloorToInt( rect.height / 2 );
				//dif_h += border_y - Math.Min( border_y, max_h );
				border_y = Math.Min( border_y, max_h );


				//if ( dif_h != 0 ) border_x -= dif_h;
				//if ( dif_w != 0 ) border_y -= dif_w;
				//border_x = border_y = Mathf.Min( border_x, border_y );

				//var n_border = Math.Min(border_x / tex.texture.width, border_y / tex.texture.height);
				var n_border_x = border_x / tex.texture.width;
				var n_border_y = border_y / tex.texture.height;


				//border_x = border_y = 0;
				////n_border = 0f / tex.texture.width;

				var rect_border_x = border_x;// n_border * rect.width;
				var rect_border_y = border_y;// n_border * rect.height;
				var uv_width = tex.uv_end.x - tex.uv_start.x;
				var uv_height = tex.uv_start.y - tex.uv_end.y;


				//if ( !d )
				//{
				//	Debug.Log(rect + " " + clipRect);
				//	Debug.Log( border );
				//	Debug.Log( n_border + " " +border_x + " " +tex.width + " " +tex.texture.width );
				//	Debug.Log( tex.uv_start + " " +tex.uv_end );
				//	Debug.Log( tex.uv_start.x + n_border );
				//	Debug.Log( tex.uv_start.y - n_border );
				//	Debug.Log( uv_width - n_border * 2 );
				//	Debug.Log( uv_height - n_border * 2 );
				//
				//}

				draw_uvved_quad(
						rect.x, rect.y, rect_border_x, rect_border_y,
						tex.uv_start.x, tex.uv_start.y, n_border_x, n_border_y, ref clipRect );
				draw_uvved_quad(
					rect.x + rect.width - rect_border_x, rect.y, rect_border_x, rect_border_y,
					tex.uv_end.x - n_border_x, tex.uv_start.y, n_border_x, n_border_y, ref clipRect );
				draw_uvved_quad(
					rect.x + rect.width - rect_border_x, rect.y + rect.height - rect_border_y, rect_border_x, rect_border_y,
					tex.uv_end.x - n_border_x, tex.uv_end.y + n_border_y, n_border_x, n_border_y, ref clipRect );
				draw_uvved_quad(
					rect.x, rect.y + rect.height - rect_border_y, rect_border_x, rect_border_y,
					tex.uv_start.x, tex.uv_end.y + n_border_y, n_border_x, n_border_y, ref clipRect );


				draw_uvved_quad(
					rect.x + rect_border_x, rect.y, rect.width - rect_border_x * 2, rect_border_y,
					tex.uv_start.x + n_border_x, tex.uv_start.y, uv_width - n_border_x * 2, n_border_y, ref clipRect );
				draw_uvved_quad(
					rect.x + rect_border_x, rect.y + rect.height - rect_border_y, rect.width - rect_border_x * 2, rect_border_y,
					tex.uv_start.x + n_border_x, tex.uv_end.y + n_border_y, uv_width - n_border_x * 2, n_border_y, ref clipRect );
				draw_uvved_quad(
					rect.x, rect.y + rect_border_y, rect_border_x, rect.height - rect_border_y * 2,
					tex.uv_start.x, tex.uv_start.y - n_border_y, n_border_x, uv_height - n_border_y * 2, ref clipRect );
				draw_uvved_quad(
					rect.x + rect.width - rect_border_x, rect.y + rect_border_y, rect_border_x, rect.height - rect_border_y * 2,
					tex.uv_end.x - n_border_x, tex.uv_start.y - n_border_y, n_border_x, uv_height - n_border_y * 2, ref clipRect );

				draw_uvved_quad(
					rect.x + rect_border_x, rect.y + rect_border_y, rect.width - rect_border_x * 2, rect.height - rect_border_y * 2,
					tex.uv_start.x + n_border_x, tex.uv_start.y - n_border_y, uv_width - n_border_x * 2, uv_height - n_border_y * 2, ref clipRect );


			}
			if ( !fast )
			{
				GL.End();
				GL.PopMatrix();
			}

		}







		class GLTextureStackAndMaterial
		{
			internal GLTextureStackAndMaterial( int sorted, IconData tex, Material mat )
			{
				mat_sorted_index = sorted;
				this.mat = mat;
				this.tex = tex;
			}
			internal int mat_sorted_index;
			internal Material mat;
			internal IconData tex;
			internal int glCount = 0;
			internal GLTextureElement[] glStack = new GLTextureElement[2000];
			internal void _put_stack( ref Rect rect, IconData tex, ref Color col, float border )
			{
				if ( glCount >= glStack.Length ) Array.Resize( ref glStack, glCount * 2 );

				glStack[ glCount ].rect = rect;
				glStack[ glCount ].clipRect = null;
				glStack[ glCount ].tex = tex;
				glStack[ glCount ].col = col;
				glStack[ glCount ].border = border;
				glCount++;
			}
			internal void _put_stack( ref Rect rect, IconData tex, ref Color col, float border, ref Rect? clipRect )
			{
				if ( glCount >= glStack.Length ) Array.Resize( ref glStack, glCount * 2 );

				glStack[ glCount ].rect = rect;
				glStack[ glCount ].clipRect = clipRect;
				glStack[ glCount ].tex = tex;
				glStack[ glCount ].col = col;
				glStack[ glCount ].border = border;
				glCount++;
			}
		}
		// DRAW SWITHCER
		class GLTextureStack
		{
			internal GLTextureStack( int sorted )
			{
				tex_sorted_index = sorted;
			}
			internal int tex_sorted_index;
			internal Dictionary<int, GLTextureStackAndMaterial> glStack = new Dictionary<int, GLTextureStackAndMaterial>();
			internal int emptyTexture = 0;
			internal void _put_stack( ref Rect rect, IconData tex, ref Color col, float border, Material mat )
			{
				var matID = mat.GetInstanceID();
				if ( tex == null ) emptyTexture = -1;
				if ( !glStack.ContainsKey( matID ) )
				{
					glStack.Add( matID, new GLTextureStackAndMaterial( glStack.Count, tex, mat ) );
					if ( !glStack[ matID ].mat ) throw new Exception( "2" );
				}
				if ( !glStack[ matID ].mat )
				{
					glStack[ matID ] = new GLTextureStackAndMaterial( glStack.Count, tex, mat );
					if ( !glStack[ matID ].mat ) throw new Exception( "2" );
				}

				glStack[ matID ]._put_stack( ref rect, tex, ref col, border );
#if DEBUG_THIS
                if (glStack[mat.GetInstanceID()].mat != mat) throw new Exception("mat");
                if (glStack[mat.GetInstanceID()].tex?.texture != tex?.texture) throw new Exception("tex " + (tex?.texture) + " + " + glStack[mat.GetInstanceID()].tex?.texture);
#endif
			}
			internal void _put_stack( ref Rect rect, IconData tex, ref Color col, float border, Material mat, ref Rect? clipRect )
			{
				var matID = mat.GetInstanceID();
				if ( tex == null ) emptyTexture = -1;
				if ( !glStack.ContainsKey( matID ) )
				{
					glStack.Add( matID, new GLTextureStackAndMaterial( glStack.Count, tex, mat ) );
					if ( !glStack[ matID ].mat ) throw new Exception( "2" );
				}
				if ( !glStack[ matID ].mat )
				{
					glStack[ matID ] = new GLTextureStackAndMaterial( glStack.Count, tex, mat );
					if ( !glStack[ matID ].mat ) throw new Exception( "2" );
				}

				glStack[ matID ]._put_stack( ref rect, tex, ref col, border, ref clipRect );
#if DEBUG_THIS
                if (glStack[mat.GetInstanceID()].mat != mat) throw new Exception("mat");
                if (glStack[mat.GetInstanceID()].tex?.texture != tex?.texture) throw new Exception("tex " + (tex?.texture) + " + " + glStack[mat.GetInstanceID()].tex?.texture);
#endif
			}

		}
		internal struct GLTextureElement
		{
			internal IconData tex;
			internal Rect rect;
			internal Rect? clipRect;
			internal Color col;
			internal float border;
		}

		
		class GLLabelStack
		{
			internal int glCount = 0;
			internal GLLabelElement[] glStack = new GLLabelElement[200];
			internal void _put_stack( Rect rect, GUIContent self_ContentInstance, GUIStyle style, /*TextAnchor alignment, RectOffset padding,
			 int fontSize,
			 Font font,*/
			  Color textColor,
			 TextClipping clipping )
			{
				if ( glCount >= glStack.Length ) Array.Resize( ref glStack, glCount * 2 );
				//   if (glStack.Any(g => g.rect == rect)) throw new Exception(glStack.First(g => g.rect == rect).self_ContentInstance.text + " - " + self_ContentInstance.text);
				/* int count = 0;
                 for (int i = 0; i < glCount; i++)
                 {
                     if (glStack[i].rect == rect) count++;
                 }
                 if (self_ContentInstance.text == "MainCamera") Debug.Log(count + " " + glCount);*/
				glStack[ glCount ].rect = rect;
				glStack[ glCount ].self_ContentInstance = self_ContentInstance;
				glStack[ glCount ].alignment = style.alignment;
				glStack[ glCount ].padding = style.padding;
				glStack[ glCount ].fontSize = style.fontSize;
				glStack[ glCount ].font = style.font;
				glStack[ glCount ].textColor = textColor;
				glStack[ glCount ].clipping = clipping;
				glStack[ glCount ].fontStyle = style.fontStyle;
				//  glStack[glCount].guiColor = guiColor;
				glCount++;
			}
		}
		internal struct GLLabelElement
		{
			internal Rect rect;
			internal GUIContent self_ContentInstance;
			internal TextAnchor alignment;
			internal RectOffset padding;
			internal int fontSize;
			internal Font font;
			internal Color textColor;
			//internal Color guiColor;
			internal TextClipping clipping;
			internal FontStyle fontStyle;
			/* labelStyle.alignment = style.alignment;
labelStyle.fontSize = style.fontSize;
labelStyle.font = style.font;
labelStyle.normal.textColor = style.normal.textColor;
labelStyle.clipping = clip;
labelStyle.Draw(rect, self_ContentInstance, false, false, false, false);*/
		}
		class GLButtonStack
		{
			internal int glCount = 0;
			internal GLButtonElement[] glStack = new GLButtonElement[100];
			internal GLButtonElement _put_stack( ref Rect rect, GUIContent self_ContentInstance, GUIStyle style, int controlId, bool contains, Color guiColor, bool guiEnabled, bool drawPointer = false )
			{
				if ( glCount >= glStack.Length ) Array.Resize( ref glStack, glCount * 2 );
				glStack[ glCount ].rect = rect;
				glStack[ glCount ].self_ContentInstance = self_ContentInstance;
				glStack[ glCount ].controlId = controlId;
				glStack[ glCount ].contains = contains;
				glStack[ glCount ].style = style;
				glStack[ glCount ].guiColor = guiColor;
				glStack[ glCount ].guiEnabled = guiEnabled;
				glStack[ glCount ].drawPointer = drawPointer;
				return glStack[ glCount++ ];
			}
		}
		internal struct GLButtonElement
		{
			internal Rect rect;
			internal GUIContent self_ContentInstance;
			internal int controlId;
			internal bool contains;
			internal GUIStyle style;
			internal Color guiColor;
			internal bool guiEnabled;
			internal bool drawPointer;

			//internal bool set_drawhover;
		}
		class GLStyleStack
		{
			internal int glCount = 0;
			internal GLStyleElement[] glStack = new GLStyleElement[50];
			internal void _put_stack( ref Rect rect, GUIContent content, GUIStyle style, bool hover, TextClipping clip, Rect? clipRect, HierarchyObject USE_GO )
			{
				if ( glCount >= glStack.Length ) Array.Resize( ref glStack, glCount * 2 );
				glStack[ glCount ].rect = rect;
				glStack[ glCount ].content = content;
				glStack[ glCount ].style = style;
				glStack[ glCount ].hover = hover;
				glStack[ glCount ].clip = clip;
				glStack[ glCount ].USE_GO = USE_GO;
				glStack[ glCount ].clipRect = clipRect;
				// if ( glCount == 0 ) {
				//     DrawStackMethodsWrapperData d;
				//     Debug.Log( clipRect ); 
				// }
				glCount++;
			}
		}
		internal struct GLStyleElement
		{
			internal Rect rect;
			internal Rect? clipRect;
			internal HierarchyObject USE_GO;
			internal GUIContent content; internal GUIStyle style; internal bool hover; internal TextClipping clip;
		}

		void _put_stack( ref Rect rect, IconData tex, ref Color col, float border, Material mat )
		{
#if DEBUG_THIS
            if (tex != null && tex.id != tex.texture.GetInstanceID()) throw new Exception("id !");
#endif
			var id = tex != null ? tex.id : -1;
			if ( !drawCalls[ dc ].unsorted_glTexture_stacks.ContainsKey( id ) ) drawCalls[ dc ].unsorted_glTexture_stacks.Add( id, new GLTextureStack( drawCalls[ dc ].unsorted_glTexture_stacks.Count ) );
			// if (!defaultMaterial()) throw new Exception("ASD");
			//  if (!mat && mat != null) throw new Exception("ASD");
			drawCalls[ dc ].unsorted_glTexture_stacks[ id ]._put_stack( ref rect, tex, ref col, border, mat ?? defaultMaterial() );
		}
		void _put_stack( ref Rect rect, IconData tex, ref Color col, float border, Material mat, ref Rect? clipRect )
		{
#if DEBUG_THIS
            if (tex != null && tex.id != tex.texture.GetInstanceID()) throw new Exception("id !");
#endif
			var id = tex != null ? tex.id : -1;
			if ( !drawCalls[ dc ].unsorted_glTexture_stacks.ContainsKey( id ) ) drawCalls[ dc ].unsorted_glTexture_stacks.Add( id, new GLTextureStack( drawCalls[ dc ].unsorted_glTexture_stacks.Count ) );
			// if (!defaultMaterial()) throw new Exception("ASD");
			//  if (!mat && mat != null) throw new Exception("ASD");
			drawCalls[ dc ].unsorted_glTexture_stacks[ id ]._put_stack( ref rect, tex, ref col, border, mat ?? defaultMaterial(), ref clipRect );
		}


		internal void _DrawTexture( Rect rect, IconData tex, ref Color col, float border )
		{
			if ( p.EVENT.type != EventType.Repaint || rect.width <= 0 || rect.height <= 0 ) return;
			if ( !BAKE_DRAWER ) __DrawTexture( ref rect, tex, ref col, border, defaultMaterial(), ref nullClip );
			else _put_stack( ref rect, tex, ref col, border, defaultMaterial(), ref nullClip );
		}
		internal void _DrawTexture( Rect rect, IconData tex, ref Color col, float border, Rect? clipRect )
		{
			if ( p.EVENT.type != EventType.Repaint || rect.width <= 0 || rect.height <= 0 ) return;
			if ( !BAKE_DRAWER ) __DrawTexture( ref rect, tex, ref col, border, defaultMaterial(), ref clipRect );
			else _put_stack( ref rect, tex, ref col, border, defaultMaterial(), ref clipRect );
		}
		//bool d;
		internal void _DrawTexture( Rect rect, IconData tex, ref Color col, float border, Material mat )
		{
			if ( p.EVENT.type != EventType.Repaint || rect.width <= 0 || rect.height <= 0 ) return;
			if ( !BAKE_DRAWER ) __DrawTexture( ref rect, tex, ref col, border, mat ?? defaultMaterial(), ref nullClip );
			else _put_stack( ref rect, tex, ref col, border, mat ?? defaultMaterial(), ref nullClip );
		}
		internal void _DrawTexture( Rect rect, IconData tex, ref Color col, float border, Material mat, Rect? clipRect )
		{
			if ( p.EVENT.type != EventType.Repaint || rect.width <= 0 || rect.height <= 0 ) return;
			if ( !BAKE_DRAWER ) __DrawTexture( ref rect, tex, ref col, border, mat ?? defaultMaterial(), ref clipRect );
			else _put_stack( ref rect, tex, ref col, border, mat ?? defaultMaterial(), ref clipRect );
		}

		internal void _DrawTexture( Rect rect, ref Color col, Event ev = null ) //  EditorGUI.DrawRect( rect, col );
		{
			if ( ev == null && p.EVENT.type != EventType.Repaint ) return;
			if ( ev != null && ev.type != EventType.Repaint ) return;
			if ( !BAKE_DRAWER )
			{
				defaultMaterial().SetTexture( _MainTex, null );
				defaultMaterial().SetPass( 0 );
				draw_simple_quad( rect, ref col );
			}
			else
			{
				_put_stack( ref rect, null, ref col, 0, null );
			}
		}
		internal void _DrawTextureGlow( Rect rect, ref Color col, Event ev = null ) //  EditorGUI.DrawRect( rect, col );
		{
			if ( ev == null && p.EVENT.type != EventType.Repaint ) return;
			if ( ev != null && ev.type != EventType.Repaint ) return;
			if ( !BAKE_DRAWER )
			{
				additionalMaterial().SetTexture( _MainTex, null );
				additionalMaterial().SetPass( 0 );
				draw_simple_quad( rect, ref col );
			}
			else
			{
				_put_stack( ref rect, null, ref col, 0, additionalMaterial() );
			}
		}
		internal void _DrawTexture_ForExternalWindow( Rect rect, IconData tex, ref Color col )
		{
			p.EVENT = Event.current;
			_DrawTexture( rect, tex, ref col );
		}
		internal void _DrawTexture_ForExternalWindow( Rect rect, IconData tex, ref Color col, float border, Material mat )
		{
			p.EVENT = Event.current;
			var b = BAKE_DRAWER;
			BAKE_DRAWER = false;
			_DrawTexture( rect, tex, ref col, border, mat );
			BAKE_DRAWER = b;
		}
		internal void _DrawTexture( Rect rect, IconData tex, ref Color col )
		{
			if ( p.EVENT.type != EventType.Repaint ) return;
			if ( !BAKE_DRAWER )
			{
				defaultMaterial().SetTexture( _MainTex, tex.texture );
				defaultMaterial().SetPass( 0 );
				draw_simple_quad( rect, ref col, tex );
			}
			else
			{
				_put_stack( ref rect, tex, ref col, 0, null );
			}
		}
		internal void _DrawTexture( Rect rect, IconData tex, ref Color col, Rect? clipRect )
		{
			if ( p.EVENT.type != EventType.Repaint ) return;
			if ( !BAKE_DRAWER )
			{
				defaultMaterial().SetTexture( _MainTex, tex.texture );
				defaultMaterial().SetPass( 0 );
				draw_simple_quad( rect, ref col, tex, clipRect );
			}
			else
			{
				_put_stack( ref rect, tex, ref col, 0, null, ref clipRect );
			}
		}

		GUIStyle labelStyle = new GUIStyle();
		internal void _DrawLabel( Rect rect, GUIContent self_ContentInstance, GUIStyle style, TextClipping clip = TextClipping.Clip )
		{
			if ( p.EVENT.type != EventType.Repaint ) return;
			if ( !BAKE_DRAWER )
			{
				//if (self_ContentInstance.text == null)
				{
					Root.SetMouseTooltip( self_ContentInstance, rect );
				}
				if ( self_ContentInstance.text != null && self_ContentInstance.text != "" )
				{

					labelStyle.alignment = style.alignment;
					labelStyle.fontStyle = style.fontStyle;
					labelStyle.fontSize = style.fontSize;
					labelStyle.font = style.font;
					labelStyle.normal.textColor = style.normal.textColor;
					labelStyle.clipping = clip;
					labelStyle.padding = style.padding;
					labelStyle.Draw( rect, self_ContentInstance, false, false, false, false );
				}
			}
			else
			{
				drawCalls[ dc ].label_stack._put_stack( rect, self_ContentInstance, style
				, style.normal.textColor * GUI.color
				, clip );
			}
			//GUI tooltip if not null
		}
		//  Rect? wasRest;
		void _DrawLabel( ref GLLabelElement e )
		{
			Root.SetMouseTooltip( e.self_ContentInstance, e.rect );
			if ( e.self_ContentInstance.text != null && e.self_ContentInstance.text != "" )
			{
				labelStyle.alignment = e.alignment;
				labelStyle.padding = e.padding;
				labelStyle.fontSize = e.fontSize;
				labelStyle.font = e.font;
				labelStyle.normal.textColor = e.textColor;
				labelStyle.clipping = e.clipping;
				labelStyle.fontStyle = e.fontStyle;
				
				// var c= GUI.color;
				// GUI.color=  e.guiColor;
				labelStyle.Draw( e.rect, e.self_ContentInstance, false, false, false, false );
				// if (wasRest == e.rect) throw new Exception("ASD");
				// wasRest = e.rect;
				// GUI.color = c;
			}
		}



		internal bool _DrawButton( Rect rect, GUIContent self_ContentInstance, GUIStyle style, bool drawPointer = false )
		{



			bool res = false;
			if ( p.par_e.ONDOWN_ACTION_INSTEAD_ONUP && p.EVENT.type == EventType.MouseDown && rect.Contains( p.EVENT.mousePosition ) )
			{
				res = true;
				EditorGUIUtility.hotControl = 0;
				p.EVENT.Use();
			}

			/* var texture = Icons.GetIconDataFromTexture(style.normal.background ?? style.normal.scaledBackgrounds[0]);
           _DrawTexture(rect, texture, ref white, style.border.left);
            if (self_ContentInstance != null && self_ContentInstance.text != null && self_ContentInstance.text != "")
                _DrawLabel(rect, self_ContentInstance, style, style.clipping);
            else if (self_ContentInstance != null && self_ContentInstance.tooltip != null && self_ContentInstance.tooltip != "")
                Root.SetMouseTooltip(self_ContentInstance, rect);*/




			/* if (p.EVENT.type == EventType.MouseDown && !p.par_e.ONDOWN_ACTION_INSTEAD_ONUP && rect.Contains(p.EVENT.mousePosition))
             {
                 capturedControlID = controlId;
                 p.EVENT.Use();
             }
             if (p.EVENT.type == EventType.MouseUp && !p.par_e.ONDOWN_ACTION_INSTEAD_ONUP && rect.Contains(p.EVENT.mousePosition) && capturedControlID == controlId)
             {
                 res = true;
             }*/
			//  if (
#if ONLY_REPAINT
#endif
			// p.EVENT.type != EventType.Repaint ||
			// !BAKE_DRAWER)
			//   if (Event.current.isMouse) Debug.Log(EditorGUIUtility.GetControlID(FocusType.Passive) + " " + EditorGUIUtility.hotControl);
			//  var h = EditorGUIUtility.hotControl;
			//var id = EditorGUIUtility.GetControlID(FocusType.Passive)^self_ContentInstance.GetHashCode();

			//if ( p.EVENT.type == EventType.MouseDown && rect.Contains( p.EVENT.mousePosition ) )
			//	if ( p.EVENT.button == 0 && p.EVENT.isMouse )
			//	{
			//		//Debug.Log( p.EVENT.type );
			//		EditorGUIUtility.hotControl = 0;
			//		EditorGUIUtility.keyboardControl = 0;
			//		Debug.Log( self_ContentInstance.text );
			//	}


			var id = EditorGUIUtility.GetControlID(FocusType.Passive)^ (int)rect.x^ (int)rect.y ^ (int)rect.width ^ (int)rect.height;
			{
				if ( p.EVENT.type == EventType.MouseDown && rect.Contains( p.EVENT.mousePosition ) )
				{
					leaveID = -1;
					capturedControlID = id;
					// p.EVENT.Use();
				}
				if ( p.EVENT.type == EventType.MouseUp )
				{
					leaveID = capturedControlID = -1;
					// p.EVENT.Use();
				}
				if ( p.EVENT.type == EventType.MouseLeaveWindow )
				{
					leaveID = capturedControlID;
					capturedControlID = -1;
				}
				if ( p.EVENT.type == EventType.MouseEnterWindow )
				{
					if ( leaveID != -1 )
					{
						capturedControlID = leaveID;
						leaveID = -1;
					}
				}



				//EMX_TODO now transparent button overlap tap glow on after draw
				if ( p.button == style )
				{
					var c=  GUI.color;
					GUI.color = Color.clear;
					if ( !p.par_e.ONDOWN_ACTION_INSTEAD_ONUP ) res = GUI.Button( rect, self_ContentInstance, style );
					else GUI.Button( rect, self_ContentInstance, style );
					GUI.color = c;
				}
				else
				{
					if ( !p.par_e.ONDOWN_ACTION_INSTEAD_ONUP ) res = GUI.Button( rect, self_ContentInstance, style );
					else GUI.Button( rect, self_ContentInstance, style );
				}

			}


			/*  if (EditorGUIUtility.hotControl != h)
                 Debug.Log(EditorGUIUtility.hotControl + " " + Event.
                     current.type +  "  " + Event.
                     current.button);*/
			//  if (res) Event.current.Use();

			if ( !BAKE_DRAWER )
			{
				//                  if (p.EVENT.type == EventType.Repaint)
				//                   {
				//                       int controlId = GUIUtility.GetControlID(FocusType.Passive, rect);
				//                       if (!GUI.enabled)
				//                       {
				//                           tempColor = GUI.color;
				//                           GUI.color *= alpha;
				//                       }
				//                       // GUI.GrabMouseControl(id);
				//                       style.Draw(rect, self_ContentInstance, controlId, false, rect.Contains(p.EVENT.mousePosition));
				//                       if (capturedControlID == controlId) Debug.Log("ASD");
				//                      style.Draw(rect, self_ContentInstance, rect.Contains(p.EVENT.mousePosition), false, capturedControlID == controlId, false);
				//                       if (!GUI.enabled) GUI.color = tempColor;
				//                   }
				//drawCalls[ dc ].button_stack._put_stack( ref rect, self_ContentInstance, style, id, false, GUI.color, GUI.enabled, drawPointer ); //EditorGUIUtility.hotControl != h ? 1 : 0
				_GLButtonElement.rect = rect;
				_GLButtonElement.style = style;
				_GLButtonElement.self_ContentInstance = self_ContentInstance;
				_GLButtonElement.controlId = id;
				_GLButtonElement.guiColor = GUI.color;
				_GLButtonElement.guiEnabled = GUI.enabled;
				_DrawButton( ref _GLButtonElement );
			}
			else
			{
				//SKIP BAKE
				//SKIP BAKE
				drawCalls[ dc ].button_stack._put_stack( ref rect, self_ContentInstance, style, id, false, GUI.color, GUI.enabled, drawPointer ); //EditorGUIUtility.hotControl != h ? 1 : 0
																																				  //SKIP BAKE
																																				  //SKIP BAKE
			}


			return res;
		}
		GLButtonElement _GLButtonElement;
#pragma warning disable
		Color tempColor;
		Color alpha = new Color(1, 1, 1, 0.5f);
		int capturedControlID = -1;
		int leaveID = -1;
#pragma warning restore
		void _DrawButton( ref GLButtonElement el )
		{
			var style = el.style;
			var rect = el.rect;
			var self_ContentInstance = el.self_ContentInstance;

			if ( p.EVENT.type == EventType.Repaint && !string.IsNullOrEmpty( self_ContentInstance.text ) )
			{
				var c = GUI.color;
				if ( el.guiColor != Color.white ) GUI.color *= el.guiColor;
				var e = GUI.enabled;
				GUI.enabled &= el.guiEnabled;
				//Debug.Log( el.self_ContentInstance.text );
				//var style = el.style;
				if ( !GUI.enabled )
				{
					tempColor = GUI.color;
					GUI.color *= alpha;
				}
				// GUI.GrabMouseControl(id);
				//style.Draw( rect, self_ContentInstance, controlId, false, rect.Contains( p.EVENT.mousePosition ) );
				//if ( capturedControlID == controlId ) Debug.Log( "ASD" );
				// style.Draw( rect, self_ContentInstance, rect.Contains( p.EVENT.mousePosition ), false, capturedControlID == controlId, false );
				// if ( capturedControlID == controlId && rect.Contains( p.EVENT.mousePosition ) )
				//if ( el.set_drawhover )
				style.Draw( rect, self_ContentInstance, false, false, false, false );
				if ( !GUI.enabled ) GUI.color = tempColor;

				if ( c != GUI.color ) GUI.color = c;
				GUI.enabled = e;
			}

			//if ( capturedControlID < 0 ) return;
			if ( capturedControlID < 0 || capturedControlID != el.controlId ) return;


			// Debug.Log( rect + " " + p.EVENT.mousePosition );
			if ( rect.Contains( p.EVENT.mousePosition ) )
			//style.Draw( rect, self_ContentInstance, true, true, true, true );
			{
				defaultMaterial().SetTexture( _MainTex, null );
				defaultMaterial().SetPass( 0 );
				var c= _hoverColor;
				draw_simple_quad( rect, ref c );
			}

			//  el.style.clipping = el.clip;
			//el.style.Draw(el.rect, el.content, el.hover, false, false, false);

#if FALSE
            // if (el.controlId != 0) EditorGUI.DrawRect(el.rect, hoverColor);
            // EditorGUIUtility.hotControl = EditorGUIUtility.GetControlID(FocusType.Passive);
            //  GUI.Button(el.rect, el.self_ContentInstance, el.style);
            var rect = el.rect;
            var self_ContentInstance = el.self_ContentInstance;
            int controlId = GUIUtility.GetControlID(FocusType.Passive, rect);



            if ( p.EVENT.type == EventType.Repaint )
            {
                var c = GUI.color;
                GUI.color = el.guiColor;
                var e = GUI.enabled;
                GUI.enabled = el.guiEnabled;

                var style = el.style;
                if ( !GUI.enabled )
                {
                    tempColor = GUI.color;
                    GUI.color *= alpha;
                }
                // GUI.GrabMouseControl(id);
                //style.Draw( rect, self_ContentInstance, controlId, false, rect.Contains( p.EVENT.mousePosition ) );
                //if ( capturedControlID == controlId ) Debug.Log( "ASD" );
                // style.Draw( rect, self_ContentInstance, rect.Contains( p.EVENT.mousePosition ), false, capturedControlID == controlId, false );
                // if ( capturedControlID == controlId && rect.Contains( p.EVENT.mousePosition ) )
                if ( el.set_drawhover )
                    style.Draw( rect, self_ContentInstance, true, true, true, true );
                if ( !GUI.enabled ) GUI.color = tempColor;

                GUI.color = c;
                GUI.enabled = e;
            }
            else
            {
                // if ( p.EVENT.type != EventType.Repaint ) Debug.Log( p.EVENT.type );
                if ( p.EVENT.type == EventType.MouseDown && rect.Contains( p.EVENT.mousePosition ) )
                {
                    leaveID = 0;
                    capturedControlID = controlId;
                     p.EVENT.Use();
                    //Debug.Log( capturedControlID );
                }
                if ( p.EVENT.type == EventType.MouseUp )
                {
                    leaveID = capturedControlID = 0;
                     p.EVENT.Use();
                }
                if ( p.EVENT.type == EventType.MouseLeaveWindow )
                {
                    leaveID = capturedControlID;
                    capturedControlID = 0;
                }
                if ( p.EVENT.type == EventType.MouseEnterWindow )
                {
                    if ( leaveID != 0 )
                    {
                        capturedControlID = leaveID;
                        leaveID = 0;
                    }
                }

                el.set_drawhover = capturedControlID == controlId && rect.Contains( p.EVENT.mousePosition );
            }


#endif

		}
		internal void _DrawStyleWithText( Rect rect, GUIStyle style, GUIContent content, TextClipping clip, bool enabled, Rect? cl, HierarchyObject USE_GO )
		{

			if ( p.EVENT.type != EventType.Repaint ) return;
			if ( !BAKE_DRAWER )
			{
				// var texture = Icons.GetIconDataFromTexture(style.normal.background ?? style.normal.scaledBackgrounds[0]);
				//  _DrawTexture(rect, texture, ref white, style.border.left);
				//  _DrawLabel(rect, content, style, clip);
				style.clipping = clip;
				// int controlId = enabled ? GUIUtility.GetControlID(FocusType.Passive, rect) : 0;
				//  style.Draw(rect, content, controlId, false, enabled && rect.Contains(Event.current.mousePosition));
				if ( cl.HasValue ) GUI.BeginClip( cl.Value );
				rect.x -= cl.Value.x;
				rect.y -= cl.Value.y;
				if ( USE_GO != null && !USE_GO.Active() )
				{
					var alphacache = GUI.color;
					GUI.color *= DrawStack.alpha;
					style.Draw( rect, content, enabled && rect.Contains( p.EVENT.mousePosition ), false, false, false );
					GUI.color = alphacache;
				}
				else
				{
					style.Draw( rect, content, enabled && rect.Contains( p.EVENT.mousePosition ), false, false, false );
				}
				if ( cl.HasValue ) GUI.EndClip();
			}
			else
			{
				drawCalls[ dc ].style_stack._put_stack( ref rect, content, style, enabled && rect.Contains( p.EVENT.mousePosition ), clip, cl, USE_GO );
			}

		}
		void _DrawStyleWithText( ref GLStyleElement el )
		{
			el.style.clipping = el.clip;
			if ( el.USE_GO != null && !el.USE_GO.Active() )
			{
				var alphacache = GUI.color;
				GUI.color *= DrawStack.alpha;
				el.style.Draw( el.rect, el.content, el.hover, false, false, false );
				GUI.color = alphacache;
			}
			else
			{
				el.style.Draw( el.rect, el.content, el.hover, false, false, false );
			}
		}
		void _DrawStyleWithText( ref GLStyleElement el, Rect cl )
		{
			el.style.clipping = el.clip;
			var r = el.rect;
			r.x -= cl.x;
			r.y -= cl.y;
			if ( el.USE_GO != null && !el.USE_GO.Active() )
			{
				var alphacache = GUI.color;
				GUI.color *= DrawStack.alpha;
				el.style.Draw( r, el.content, el.hover, false, false, false );
				GUI.color = alphacache;
			}
			else
			{
				el.style.Draw( r, el.content, el.hover, false, false, false );
			}
		}

		///////////////////////////
		//TAP GLOW INPORT
		////  static Texture2D hoverTexture;
		Color _hoverColor { get { return p.par_e.BUTTON_TAP_COLOR; } }
		internal void DRAW_TAP_GLOW( Rect rect, float alpha )
		{
			p.EVENT = Event.current;
			Color c = _hoverColor;
			c.a = alpha;
			_DrawTexture( rect, ref c );
			//    EditorGUI.DrawRect(rect, c);
			//Debug.Log( "ASD" );
		}
		internal void DRAW_TAP_GLOW_ADDITIONAL_MAT( Rect rect, float alpha )
		{
			p.EVENT = Event.current;
			Color c = _hoverColor;
			c.a = alpha;
			_DrawTextureGlow( rect, ref c );
			//    EditorGUI.DrawRect(rect, c);

		}
		internal void DRAW_TAP_GLOW( Rect rect )
		{
			//// if ( !hoverTexture ) hoverTexture = Root.p[ 0 ].GetIcon( "TAP_GLOW" );
			/// GUI.DrawTexture( rect, hoverTexture );
			p.EVENT = Event.current;
			var c= _hoverColor;
			_DrawTexture( rect, ref c, Event.current );
			//  EditorGUI.DrawRect(rect, hoverColor);
		}
		internal void DRAW_TAP_GLOW( Rect rect, Color color )
		{
			//// if ( !hoverTexture ) hoverTexture = Root.p[ 0 ].GetIcon( "TAP_GLOW" );
			/// GUI.DrawTexture( rect, hoverTexture );
			p.EVENT = Event.current;
			_DrawTexture( rect, ref color, Event.current );
			// EditorGUI.DrawRect(rect, color);
		}
		//TAP GLOW INPORT
		///////////////////////////

		// DRAW SWITHCER
		/* internal void _DrawTexture(Rect rect, ref Color col, float A)
        {
          if (p.EVENT.type != EventType.Repaint) return;
          col.a *= A;
          defaultMaterial().SetTexture(_MainTex, null);
          defaultMaterial().SetPass(0);
          draw_simple_quad(ref rect, ref col);
        }*/


		internal void _DrawTexture( Rect rect, IconData tex )
		{
			if ( p.EVENT.type != EventType.Repaint ) return;
			var c = Color.white;
			_DrawTexture( rect, tex, ref c );
		}
		internal void _DrawRect( ref Rect rect, ref Color color )
		{
			_DrawTexture( rect, ref color );
		}
		internal void _DrawRect( ref Rect rect, Color color )
		{
			_DrawTexture( rect, ref color );
		}

		internal void GL_BEGIN()
		{
			GL.PushMatrix();
			// Set black as background color
			//GL.LoadPixelMatrix();
			GL.Clear( true, false, Color.black );
			// var mat = adapter.DEFAULT_SHADER_SHADER.HIghlighterExternalMaterial;
			//mat = adapter.HIghlighterExternalMaterialNormal;

			defaultMaterial().SetTexture( _MainTex, null );
			defaultMaterial().SetPass( 0 );
			//	GL.Begin(GL.LINES);
			GL.Begin( GL.LINES );
			//  mat.SetTexture(adapter._MainTex, Texture2D.whiteTexture);
		}

		internal void GL_END()
		{
			GL.End();

			GL.PopMatrix();
		}


		/*	internal void _DrawTexture_PlusGUIColor( Rect rect, IconData tex ) 
		{
			if ( p.EVENT.type != EventType.Repaint ) return;
			var c = GUI.color;
			_DrawTexture( rect, tex, ref c );
		}*/
		/*internal void _DrawTexture_StretchToFill( Rect rect, Texture tex ) 
		{ 
			if ( p.EVENT.type != EventType.Repaint ) return;
			GUI.DrawTexture( rect, tex, ScaleMode.StretchToFill, true, 1, GUI.color, 0, 0 );
		}*/
		/*internal void _DrawRect_PlusGUIColor( ref Rect rect, Color c )
		{
			c *= GUI.color;
			_DrawTexture( rect, ref c );
		}*/
	}
}
