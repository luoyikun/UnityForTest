#define USE_STACK

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor
{
	internal class DynamicColor
    {
        internal Func<HierarchyObject, object, Color> GET = null;
    }


    internal class DrawStackItem
    {
#pragma warning disable
        internal Rect localRect;

        internal Rect rect( Rect worldOffset )
        {
            // if ( clipRect.HasValue ) clipRect = null;
            worldOffset.x += localRect.x;
            worldOffset.y += localRect.y;
            //EMX_TODO hot fix for float scroll values
           // worldOffset.y = (int)worldOffset.y;
            worldOffset.width = localRect.width;
            worldOffset.height = localRect.height;
            return worldOffset;
        }
        internal Rect? clipRectGet( Rect worldOffset )
        {
            if ( !clipRect.HasValue ) return null;
            tr = clipRect.Value;
            tr.x += worldOffset.x;
            tr.y += worldOffset.y;
            return tr;
        }


        internal void rect( ref Rect worldRect, ref Rect worldOffset )
        {
            localRect.x = worldRect.x - worldOffset.x;
            localRect.y = worldRect.y - worldOffset.y;
            localRect.width = worldRect.width;
            localRect.height = worldRect.height;
            clipRect = null;
        }
        static Rect tr;
        internal void rect( ref Rect worldRect, ref Rect worldOffset, ref Rect? clip )
        {
            localRect.x = worldRect.x - worldOffset.x;
            localRect.y = worldRect.y - worldOffset.y;
            localRect.width = worldRect.width;
            localRect.height = worldRect.height;
            if ( clip.HasValue )
            {
                tr = clip.Value;
                tr.x = tr.x - worldOffset.x;
                tr.y = tr.y - worldOffset.y;
                clipRect = tr;
            }
            else
            {
                clipRect = null;
            }
        }
        internal ModulesDrawType type;

        internal GUIStyle style;

        internal Material material;
        internal IconData texture;
        internal Color color;
        internal DynamicColor dynamicColor;
        internal Color? nullable_Color;
        internal Rect? uv;
        internal GUIContent content;
        internal DrawStackMethodsWrapper action;
        internal GUIContent self_ContentInstance;
        internal bool SetStyleColor;
        internal bool drawPointer;
        internal bool callFromExternal;
        internal bool OverrideEnable;
        internal bool UseGameObjectEnable;
        internal bool hasContent;
        internal bool self_SkipContentInstance;
        internal int borders;
        internal int SWITCHER_MARKER;
        internal SwithcerMethodsWrapper SWITCHER_SELECTOR;
        internal object args;
        internal Rect? clipRect;

        public DrawStackItem ADD_SWITCHER( int v )
        {

            SWITCHER_MARKER = v;
            return this;
        }
#if !UNITY_EDITOR
		object _args;
		internal object args
		{	get
			{	return _args;
			}
			
			set
			{	if ( value != null &&
			
				        value.GetType() != typeof( AudioSource ) &&
				        value.GetType() != typeof( Hierarchy.M_VerticesHelper ) &&
				        value.GetType() != typeof( Hierarchy.M_CustomIcons.AttributeButton ) &&
				        value.GetType() != typeof( Hierarchy.M_CustomIcons.AttributeField ) &&
				        
				        ( value.GetType().IsClass || value.GetType().IsArray) ) throw new Exception( "class exept" );
				        
				_args = value;
			}
		}
#else

#endif
        /*  internal string content ;
          internal string tooltip ;*/
#pragma warning restore

        internal void Clear( ModulesDrawType type )
        {
            this.type = type;
            // if ( this.tooltip != null ) this.tooltip = null;
        }
    }

    internal enum ModulesDrawType
    {
        GUIDrawTexture,
        GUIDrawColor,
        GUIDrawTextureWithBorders,
        GUIDrawTextureWithBordersAndMaterial,
        //AdapterDrawTexture,
        //AdapterDrawColor,
        //Button,
        //Style,

        Label,
        Action,
        ModuleButton,
        SimpleButton,
        BeginClip,
        EndClip,
        BEGIN_DRAW_SWITCHER,
        END_DRAW_SWITCHER
    }


    internal class DrawStack
    {


        PluginInstance adapter { get { return Root.p[ 0 ]; } }
        internal GlDrawer gl;
        internal int drawCallIndex;
        internal DrawStack( int pId, int drawCallIndex, GlDrawer gl )
        {
            this.drawCallIndex = drawCallIndex;
            //adapter = Root.p[pId];
            this.gl = gl;
        }
        //  internal HierarchyObject o;
        //      static GUIContent contentDraw = new GUIContent();
        internal bool GO_ENABLE_STATE;
        internal int Count;
        internal int currentStackPos = -1;
        internal List<DrawStackItem> stack = new List<DrawStackItem>(3) { new DrawStackItem(), new DrawStackItem(), new DrawStackItem() };
        static DrawStackItem emptyStackItem = new DrawStackItem();
        internal Rect worldOffset;

        public DrawStackItem PUSH( ModulesDrawType type )
        {
            if ( DRAW_AS_EMPTY )
            {
                emptyStackItem.Clear( type );
                return emptyStackItem;
            }

            Count++;

            if ( currentStackPos == -2 ) currentStackPos++;

            currentStackPos++;

            if ( currentStackPos >= stack.Count ) stack.Add( new DrawStackItem() );

            stack[ currentStackPos ].Clear( type );
            return stack[ currentStackPos ];
        }
        /* public DrawStackItem POP()
         {   var res = stack[currentStackPos];
             currentStackPos--;
             return res;
         }*/

        public void ResetStack()
        {
            Count = 0;
            currentStackPos = -1;
            NOW_SWITCHER = false;
        }
        /*   public DrawStackItem ACTION( Action action, bool skipSearch )
         {   if ( skipSearch && HAS_SEARCH_STRING() ) return empty;
             if ( !DODRAW_SEARCH() ) return empty;
             var s = PUSH(ModulesDrawType.Action);
             s.action = action;
             return s;
         }*/

        public DrawStackItem BEGIN_DRAW_SWITCHER( SwithcerMethodsWrapper method )
        {
            var s = PUSH(ModulesDrawType.BEGIN_DRAW_SWITCHER);
            s.SWITCHER_SELECTOR = method;
            return s;
        }

        public DrawStackItem END_DRAW_SWITCHER()
        {
            var s = PUSH(ModulesDrawType.END_DRAW_SWITCHER);
            return s;
        }

        public DrawStackItem Draw_BeginClip( Rect rect )
        {
            var s = PUSH(ModulesDrawType.BeginClip);
            s.rect( ref rect, ref worldOffset );
            return s;
        }

        public DrawStackItem Draw_EndClip()
        {
            var s = PUSH(ModulesDrawType.EndClip);
            return s;
        }

        /* public DrawStackItem Draw_Style( Rect rect, GUIStyle style, GUIContent content, Color? color, bool USE_GO ) // return BUTTON( new[] { textArray }, new[] { buttonActionArray }, height );
		 {
			 var s = PUSH(ModulesDrawType.Style);
			 s.rect( ref rect, ref worldOffset );
			 s.style = style;
			 s.content = content;
			 s.UseGameObjectEnable = USE_GO;
			 s.nullable_Color = color;
			 s.OverrideEnable = true;

			 return s;
		 }

		 public DrawStackItem Draw_Style( Rect rect, GUIStyle style, string content, Color? color, bool USE_GO ) // return BUTTON( new[] { textArray }, new[] { buttonActionArray }, height );
		 {
			 var s = PUSH(ModulesDrawType.Style);
			 s.rect( ref rect, ref worldOffset );
			 s.style = style;

			 if ( s.self_ContentInstance == null ) s.self_ContentInstance = new GUIContent();

			 s.self_ContentInstance.text = content;
			 s.self_ContentInstance.tooltip = null;
			 s.content = null;
			 s.UseGameObjectEnable = USE_GO;
			 s.nullable_Color = color;
			 s.OverrideEnable = true;

			 return s;
		 }*/

        /*public DrawStackItem Draw_Button( Rect rect, string textArray, Action buttonActionArray, int? height )
		{ // return BUTTON( new[] { textArray }, new[] { buttonActionArray }, height );
			var s = PUSH(ModulesDrawType.Button);
			s.rect( ref rect, ref worldOffset );
			return s;
		}*/

        public DrawStackItem Draw_GUITexture( Rect rect, IconData icon, Color guiColor, bool USE_GO )
        {
            var s = PUSH(ModulesDrawType.GUIDrawTexture);
            s.rect( ref rect, ref worldOffset );
            s.color = guiColor;
            //s.dynamicColor = dc;
            s.uv = null;
            s.material = null;
            // s.clipRect = Rect.zero;
            /// if ( !icon ) throw new NullReferenceException();

            s.texture = icon;
            s.UseGameObjectEnable = USE_GO;
            return s;
        }
        public DrawStackItem Draw_GUITexture( Rect rect, IconData icon, Color guiColor, bool USE_GO, ref Rect? clipRect )
        {
            var s = PUSH(ModulesDrawType.GUIDrawTexture);
            // clipRect = new Rect(2,2,5,5);
            s.rect( ref rect, ref worldOffset, ref clipRect );
            s.color = guiColor;
            //s.dynamicColor = dc;
            s.uv = null;
            s.material = null;
            // s.clipRect = Rect.zero;
            /// if ( !icon ) throw new NullReferenceException();

            s.texture = icon;
            s.UseGameObjectEnable = USE_GO;
            return s;
        }
        /*public DrawStackItem Draw_GUITexture( Rect rect, Rect uv, IconData icon, Color guiColor, bool USE_GO )
		{
			var s = PUSH(ModulesDrawType.GUIDrawTexture);
			s.rect( ref rect, ref worldOffset );
			s.color = guiColor;
			s.uv = uv;

			// if ( !icon ) throw new NullReferenceException();

			s.texture = icon;
			s.UseGameObjectEnable = USE_GO;
			return s;
		}
		*/

        public DrawStackItem Draw_GUITexture( Rect rect, Color color, bool USE_GO, DynamicColor dc = null )
        {
            var s = PUSH(ModulesDrawType.GUIDrawColor);
            s.rect( ref rect, ref worldOffset );
            s.color = color;
            s.dynamicColor = dc;
            s.UseGameObjectEnable = USE_GO;
            return s;
        }

        public DrawStackItem Draw_ScaleToFit_Texture( Rect rect, IconData icon, Color guiColor, bool USE_GO )
        {

            var a = icon.width / icon.height;
            var s_a = rect.width / rect.height;
            if ( s_a < a ) rect.height = rect.width / a;
            else if ( s_a > a ) rect.width = rect.height * a;

            return Draw_GUITexture( rect, icon, guiColor, USE_GO );

            /* var s = PUSH(ModulesDrawType.AdapterDrawTexture);

             s.rect( ref rect, ref worldOffset );
             s.color = guiColor;
             s.dynamicColor = null;

             if ( !icon ) throw new NullReferenceException();

             s.scaleMode = ScaleMode.ScaleToFit;
             s.texture = icon;
             s.UseGameObjectEnable = USE_GO;
             return s;*/
        }

        /*   public DrawStackItem Draw_ScaleToFit_Texture( Rect rect, Color color, bool USE_GO )
           {
               var s = PUSH(ModulesDrawType.AdapterDrawColor);
               s.rect( ref rect, ref worldOffset );
               s.color = color;
              //s.scaleMode = ScaleMode.ScaleToFit;
               s.UseGameObjectEnable = USE_GO;
               return s;
           }*/

        /*public DrawStackItem Draw_ScaleToFit_Texture( Rect rect, DynamicColor dynamicColor, bool USE_GO )
		{


			var s = PUSH(ModulesDrawType.AdapterDrawColor);
			s.rect( ref rect, ref worldOffset );
			s.dynamicColor = dynamicColor;
			s.UseGameObjectEnable = USE_GO;
			return s;
		}*/

        internal DrawStackItem Draw_GUITextureWithBorders( Rect rect, IconData texture, int borders, Color color, bool USE_GO, Rect? clipRect )
        {
            var s = PUSH(ModulesDrawType.GUIDrawTextureWithBorders);
            //s.rect(ref rect, ref worldOffset);
            s.rect( ref rect, ref worldOffset, ref clipRect );
            s.dynamicColor = null;
            s.color = color;
            s.UseGameObjectEnable = USE_GO;
            s.texture = texture;
            s.borders = borders;
            return s;
        }
        internal DrawStackItem Draw_GUITextureWithBorders( Rect rect, IconData texture, int borders, DynamicColor color, bool USE_GO, object args, Rect? clipRect )
        {
            var s = PUSH(ModulesDrawType.GUIDrawTextureWithBorders);
            //s.rect(ref rect, ref worldOffset);
            s.rect( ref rect, ref worldOffset, ref clipRect );
            s.dynamicColor = color;
            s.UseGameObjectEnable = USE_GO;
            s.texture = texture;
            s.material = null;
            s.borders = borders;
            s.args = args;
            return s;
        }

        internal DrawStackItem Draw_GUITextureWithBordersAndMaterial( Rect rect, IconData texture, int borders, Material mat, DynamicColor color, bool USE_GO, object args, Rect? clipRect )
        {
            var s = PUSH(ModulesDrawType.GUIDrawTextureWithBordersAndMaterial);
            s.rect( ref rect, ref worldOffset, ref clipRect );
            s.dynamicColor = color;
            s.UseGameObjectEnable = USE_GO;
            s.texture = texture;
            s.material = mat;
            s.borders = borders;
            s.args = args;
            return s;
        }



        //*******************////
        //*******************////
        //*******************////
        //*******************////
        public DrawStackItem Draw_Action( Rect rect, DrawStackMethodsWrapper action, object args, GUIContent content )
        {
            var s = PUSH(ModulesDrawType.Action);
            s.rect( ref rect, ref worldOffset );
            s.action = action;
            s.args = args;

            if ( content == null )
                s.content = null;
            else
            {
                if ( s.self_ContentInstance == null ) s.self_ContentInstance = new GUIContent();

                s.self_ContentInstance.text = content.text;
                s.self_ContentInstance.tooltip = content.tooltip;
                s.content = s.self_ContentInstance;
            }

            return s;
        }

        public DrawStackItem Draw_LabelWithTextColor( Rect rect, string content, Color textColor, GUIStyle style, bool ENABLE_ACTIVE_IN_HIERARCHY, Color? c, bool? OVERRIDE_ENABLE, Rect? clampRect = null )
        {
            var s = PUSH(ModulesDrawType.Label);
            //s.rect( ref rect, ref worldOffset );
            if ( clampRect.HasValue ) s.rect( ref rect, ref worldOffset, ref clampRect );
            else s.rect( ref rect, ref worldOffset );

            if ( s.self_ContentInstance == null ) s.self_ContentInstance = new GUIContent();

            s.self_ContentInstance.text = content;
            s.self_ContentInstance.tooltip = null;
            s.style = style;
            s.UseGameObjectEnable = ENABLE_ACTIVE_IN_HIERARCHY;
            s.nullable_Color = c;
            s.color = textColor;
            s.dynamicColor = null;
            s.SetStyleColor = true;
            s.OverrideEnable = OVERRIDE_ENABLE ?? true;
            return s;
        }

        public DrawStackItem Draw_LabelWithTextColor( Rect rect, string content, DynamicColor textColor, GUIStyle style, bool ENABLE_ACTIVE_IN_HIERARCHY, Color? c, bool? OVERRIDE_ENABLE, Rect? clampRect = null )
        {
            var s = PUSH(ModulesDrawType.Label);
            //s.rect( ref rect, ref worldOffset );
            if ( clampRect.HasValue ) s.rect( ref rect, ref worldOffset, ref clampRect );
            else s.rect( ref rect, ref worldOffset );

            if ( s.self_ContentInstance == null ) s.self_ContentInstance = new GUIContent();

            s.self_ContentInstance.text = content;
            s.self_ContentInstance.tooltip = null;
            s.style = style;
            s.UseGameObjectEnable = ENABLE_ACTIVE_IN_HIERARCHY;
            s.nullable_Color = c;
            s.dynamicColor = textColor;
            s.SetStyleColor = true;
            s.OverrideEnable = OVERRIDE_ENABLE ?? true;
            return s;
        }


        public DrawStackItem Draw_Label( Rect rect, string content, GUIStyle style, bool ENABLE_ACTIVE_IN_HIERARCHY, Color? c, bool? OVERRIDE_ENABLE, Rect? clampRect = null )
        {
            var s = PUSH(ModulesDrawType.Label);
            //s.rect( ref rect, ref worldOffset );
            if ( clampRect.HasValue ) s.rect( ref rect, ref worldOffset, ref clampRect );
            else s.rect( ref rect, ref worldOffset );

            if ( s.self_ContentInstance == null ) s.self_ContentInstance = new GUIContent();

            s.self_ContentInstance.text = content;
            s.self_ContentInstance.tooltip = null;
            s.style = style;
            s.UseGameObjectEnable = ENABLE_ACTIVE_IN_HIERARCHY;
            s.dynamicColor = null;
            s.nullable_Color = c;
            s.SetStyleColor = false;
            s.OverrideEnable = OVERRIDE_ENABLE ?? true;
            return s;
        }

        public DrawStackItem Draw_Label( Rect rect, GUIContent content, GUIStyle style, bool ENABLE_ACTIVE_IN_HIERARCHY, Color? c, bool? OVERRIDE_ENABLE, Rect? clampRect = null )
        {
            var s = PUSH(ModulesDrawType.Label);
            //s.rect( ref rect, ref worldOffset );
            if ( clampRect.HasValue ) s.rect( ref rect, ref worldOffset, ref clampRect );
            else s.rect( ref rect, ref worldOffset );

            if ( s.self_ContentInstance == null ) s.self_ContentInstance = new GUIContent();

            s.self_ContentInstance.text = content.text;
            s.self_ContentInstance.tooltip = content.tooltip;
            s.style = style;
            s.UseGameObjectEnable = ENABLE_ACTIVE_IN_HIERARCHY;
            s.dynamicColor = null;
            s.nullable_Color = c;
            s.SetStyleColor = false;
            s.OverrideEnable = OVERRIDE_ENABLE ?? true;
            return s;
        }

        public DrawStackItem Draw_ModuleButton( Rect rect, GUIContent content, DrawStackMethodsWrapper action, bool hasContent, object args, bool useContentForButton, GUIStyle style,
            bool USE_GO = false, bool callFromExternal = false, bool drawPointer = false, Rect? clampRect = null
             )
        {
            var s = PUSH(ModulesDrawType.ModuleButton);
            // s.rect( ref rect, ref worldOffset );
            if ( clampRect.HasValue ) s.rect( ref rect, ref worldOffset, ref clampRect );
            else s.rect( ref rect, ref worldOffset );
            s.action = action;
            s.style = style;
            s.content = null;
            s.callFromExternal = callFromExternal;
            s.drawPointer = drawPointer;

            if ( content != null )
            {
                if ( useContentForButton )
                {
                    if ( s.self_ContentInstance == null ) s.self_ContentInstance = new GUIContent();

                    s.self_ContentInstance.text = content.text;
                    s.self_ContentInstance.tooltip = content.tooltip;
                    s.content = s.self_ContentInstance;
                }

                else
                {
                    if ( s.self_ContentInstance == null ) s.self_ContentInstance = new GUIContent();

                    s.self_ContentInstance.text = content.text;
                    s.self_ContentInstance.tooltip = content.tooltip;
                }

                s.self_SkipContentInstance = false;
            }

            else s.self_SkipContentInstance = true;

            s.hasContent = hasContent;
            s.args = args;
            s.UseGameObjectEnable = USE_GO;
            return s;
        }

        public DrawStackItem Draw_SimpleButton( Rect rect, GUIContent content, DrawStackMethodsWrapper action, object args, bool useContentForButton, GUIStyle style, bool USE_GO = false, bool callFromExternal = false, bool drawPointer = false, Rect? clampRect = null )
        {
            var s = PUSH(ModulesDrawType.SimpleButton);
            if ( clampRect.HasValue ) s.rect( ref rect, ref worldOffset, ref clampRect );
            else s.rect( ref rect, ref worldOffset );

            s.action = action;
            s.style = style;
            s.content = null;
            s.callFromExternal = callFromExternal;
            s.drawPointer = drawPointer;

            if ( content != null )
            {
                if ( useContentForButton )
                {
                    if ( s.self_ContentInstance == null ) s.self_ContentInstance = new GUIContent();

                    s.self_ContentInstance.text = content.text;
                    s.self_ContentInstance.tooltip = content.tooltip;
                    s.content = s.self_ContentInstance;
                }

                else
                {
                    if ( s.self_ContentInstance == null ) s.self_ContentInstance = new GUIContent();

                    s.self_ContentInstance.text = content.text;
                    s.self_ContentInstance.tooltip = content.tooltip;
                }

                s.self_SkipContentInstance = false;
            }

            else s.self_SkipContentInstance = true;

            s.args = args;
            s.UseGameObjectEnable = USE_GO;
            return s;
        }




        /*
		public DrawStackItem LABEL( string text )
		{
			var s = PUSH(ModulesDrawType.Button);
			return s;
		}*/

        internal void Draw( ref Rect worldOffset )
        { // Debug.Log( Count );
            this.worldOffset = worldOffset;

            gl.DrawStackItem( this );
            ///for ( int i = 0 ; i < Count ; i++ )
            {
                ///DrawSIngleItem( stack[ i ], o );
            }
        }

        internal HierarchyObject current_object = null;

        internal void DrawSIngleItem( DrawStackItem stack )
        {
            if ( current_object == null ) throw new NullReferenceException( "current_object" );
            DrawSIngleItem( stack, current_object );
        }

        static int colorProperty = Shader.PropertyToID("_Color");
        Color cache_c;
        SwithcerMethodsWrapper NOW_SWITCHER_SELECTOR;
        bool NOW_SWITCHER;

        //  internal Vector2 CLIP_VECTOR;

        internal void DrawSIngleItem( DrawStackItem stack, HierarchyObject _o )
        {
            if ( NOW_SWITCHER && stack.type != ModulesDrawType.END_DRAW_SWITCHER && NOW_SWITCHER_SELECTOR.action( _o ) != stack.SWITCHER_MARKER ) return;


            switch ( stack.type )
            {
                case ModulesDrawType.BEGIN_DRAW_SWITCHER:
                    {
                        NOW_SWITCHER = true;
                        NOW_SWITCHER_SELECTOR = stack.SWITCHER_SELECTOR;
                        break;
                    }

                case ModulesDrawType.END_DRAW_SWITCHER:
                    {
                        NOW_SWITCHER = false;
                        break;
                    }


                case ModulesDrawType.BeginClip:
                    {
                        GUI.BeginClip( stack.rect( worldOffset ), Vector2.zero, Vector2.zero, false );
                        break;
                    }

                case ModulesDrawType.EndClip:
                    {
                        GUI.EndClip();
                        break;
                    }

                case ModulesDrawType.GUIDrawTextureWithBordersAndMaterial:
                    {
                        if ( adapter.EVENT.type != EventType.Repaint ) return;

                        var c = stack.dynamicColor != null ? stack.dynamicColor.GET(_o, stack.args) : stack.color;

                        if ( stack.UseGameObjectEnable && !_o.Active() ) c.a *= 0.5f;

                        /*
						adapter.par_e.HIghlighterExternalMaterial.SetColor( colorProperty, c * GUI.color );
						Graphics.DrawTexture( stack.rect( worldOffset ), stack.texture, stack.borders, stack.borders, stack.borders, stack.borders,
							adapter.par_e.HIghlighterExternalMaterial, 0 );*/
                        c *= GUI.color;
                        if ( stack.material ) gl._DrawTexture( stack.rect( worldOffset ), stack.texture, ref c, stack.borders, stack.material, stack.clipRectGet( worldOffset ) );
                        else gl._DrawTexture( stack.rect( worldOffset ), stack.texture, ref c, stack.borders, stack.clipRectGet( worldOffset ) );
                        break;
                    }

                case ModulesDrawType.GUIDrawTextureWithBorders:
                    {
                        if ( adapter.EVENT.type != EventType.Repaint ) return;

                        var c = stack.dynamicColor != null ? stack.dynamicColor.GET(_o, stack.args) : stack.color;
                        //  var c = stack.dynamicColor.GET(_o, stack.args);

                        if ( stack.UseGameObjectEnable && !_o.Active() ) c.a *= 0.5f;

                        // bordersWidth.x = bordersWidth.y = bordersWidth.z = bordersWidth.w = stack.borders;
                        //   c = Color.black;
                        //  adapter.GL_DrawTexture( stack.rect, stack.texture, ScaleMode.StretchToFill, true, 1, c, bordersWidth, Vector4.zero );
                        if ( stack.material ) gl._DrawTexture( stack.rect( worldOffset ), stack.texture, ref c, stack.borders, stack.material, stack.clipRectGet( worldOffset ) );
                        else gl._DrawTexture( stack.rect( worldOffset ), stack.texture, ref c, stack.borders, stack.clipRectGet( worldOffset ) );
                        break;
                    }

                case ModulesDrawType.GUIDrawTexture:
                    {
                        if ( adapter.EVENT.type != EventType.Repaint ) return;

                        var c = stack.color;

                        if ( stack.UseGameObjectEnable && !_o.Active() ) c.a *= 0.5f;

                        // adapter.GL_DrawTexture( stack.rect, stack.texture, ScaleMode.StretchToFill, true, 1, c, Vector4.zero, Vector4.zero );
                        if ( stack.clipRect != null )
                        {
                            if ( stack.material ) gl._DrawTexture( stack.rect( worldOffset ), stack.texture, ref c, 0, stack.material, clipRect: stack.clipRectGet( worldOffset ) );
                            else gl._DrawTexture( stack.rect( worldOffset ), stack.texture, ref c, clipRect: stack.clipRectGet( worldOffset ) );
                        }
                        else
                        {
                            if ( stack.material ) gl._DrawTexture( stack.rect( worldOffset ), stack.texture, ref c, 0, stack.material );
                            else gl._DrawTexture( stack.rect( worldOffset ), stack.texture, ref c );
                        }

                        break;
                    }

                case ModulesDrawType.GUIDrawColor:
                    {
                        if ( adapter.EVENT.type != EventType.Repaint ) return;

                        var c = stack.color;
                        if ( stack.dynamicColor != null ) c *= stack.dynamicColor.GET( _o, null );

                        if ( stack.UseGameObjectEnable && !_o.Active() ) c.a *= 0.5f;

                        //  adapter.GL_DrawTexture( stack.rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 1, c, Vector4.zero, Vector4.zero );
                        //if ( stack.clipRect != null )
                        //    gl._DrawTexture( stack.rect( worldOffset ), ref c,  clipRect: stack.clipRectGet( worldOffset ) );
                        //else
                        gl._DrawTexture( stack.rect( worldOffset ), ref c );

                        break;
                    }

                /*	case ModulesDrawType.AdapterDrawTexture:
						{
							if ( adapter.EVENT.type != EventType.Repaint ) return;

							var c = stack.color;

							if ( stack.UseGameObjectEnable && !_o.Active() ) c.a *= 0.5f;

							//   adapter.GL_DrawTexture( stack.rect, stack.texture, stack.scaleMode, true, 1, c, Vector4.zero, Vector4.zero );
							adapter.DrawTexture( stack.rect( worldOffset ), stack.texture, stack.scaleMode, true, 1, c, 0, 0 );
							break;
						}

					case ModulesDrawType.AdapterDrawColor:
						{
							if ( adapter.EVENT.type != EventType.Repaint ) return;

							var c = stack.dynamicColor != null ? stack.dynamicColor.GET(_o, null) : stack.color;

							if ( stack.UseGameObjectEnable && !_o.Active() ) c.a *= 0.5f;

							//  adapter.GL_DrawTexture( stack.rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 1, c, Vector4.zero, Vector4.zero );
							adapter.DrawTexture( stack.rect( worldOffset ), Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 1, c, 0, 0 );
							break;
						}*/

                /*	case ModulesDrawType.Style:
						{
							if ( adapter.EVENT.type == EventType.Repaint )
							{ 

								if ( stack.nullable_Color.HasValue )
								{
									cache_c = GUI.color;
									GUI.color *= stack.nullable_Color.Value;
								}

								if ( stack.UseGameObjectEnable || !stack.OverrideEnable )
								{
									var en = GUI.enabled;
									var a = _o.Active();
									GUI.enabled &= stack.OverrideEnable && a;

									if ( !a )
									{
										alphacache = GUI.color;
										GUI.color *= alpha;
									}

									stack.style.Draw( stack.rect( worldOffset ), stack.content ?? stack.self_ContentInstance, false, false, false, false );

									if ( !a ) GUI.color = alphacache;

									GUI.enabled = en;
								}

								else
								{
									stack.style.Draw( stack.rect( worldOffset ), stack.content ?? stack.self_ContentInstance, false, false, false, false );
								}

								if ( stack.nullable_Color.HasValue ) GUI.color = cache_c;
							}


							break;
						}
						*/
                case ModulesDrawType.Action:
                    {
                        data.args = stack.args;
                        data.content = stack.content;
                        stack.action.action( worldOffset, stack.rect( worldOffset ), data, _o );
                        break;
                    }

                case ModulesDrawType.Label:
                    {
                        bool notol = (stack.self_ContentInstance.tooltip == null || stack.self_ContentInstance.tooltip == "");

                        // bool notol = false;
                        //if ( notol && adapter.EVENT.type != EventType.Repaint ) return;

                        if ( stack.nullable_Color.HasValue )
                        {
                            cache_c = GUI.color;
                            GUI.color *= stack.nullable_Color.Value;
                        }

                        if ( stack.UseGameObjectEnable || !stack.OverrideEnable )
                        {
                            var en = GUI.enabled;
                            var a = _o.Active();
                            GUI.enabled &= stack.OverrideEnable && a;

                            if ( !a )
                            {
                                alphacache = GUI.color;
                                GUI.color *= alpha;
                            }

                            if ( stack.SetStyleColor )
                            {
                                styleCacheColor = stack.style.normal.textColor;

                                if ( stack.dynamicColor != null )
                                {
                                    stack.style.normal.textColor = stack.dynamicColor.GET( _o, null );
                                }

                                else stack.style.normal.textColor = stack.color;

                                // if ( adapter.EVENT.type == EventType.Repaint ) stack.style.Draw( stack.rect, "", false, false, false, false );
                                //if ( notol ) stack.style.Draw( stack.rect( worldOffset ), stack.self_ContentInstance, false, false, false, false );
                                //else GUI.Label( stack.rect( worldOffset ), stack.self_ContentInstance, stack.style );
                                gl._DrawLabel( stack.rect( worldOffset ), stack.self_ContentInstance, stack.style );

                                stack.style.normal.textColor = styleCacheColor;
                            }

                            else
                            {
                                //	if ( notol ) stack.style.Draw( stack.rect( worldOffset ), stack.self_ContentInstance, false, false, false, false );
                                //else GUI.Label( stack.rect( worldOffset ), stack.self_ContentInstance, stack.style );
                                gl._DrawLabel( stack.rect( worldOffset ), stack.self_ContentInstance, stack.style );
                            }

                            if ( !a ) GUI.color = alphacache;

                            GUI.enabled = en;
                        }

                        else
                        {
                            if ( stack.SetStyleColor )
                            {
                                styleCacheColor = stack.style.normal.textColor;

                                if ( stack.dynamicColor != null )
                                {
                                    stack.style.normal.textColor = stack.dynamicColor.GET( _o, null );
                                }

                                else stack.style.normal.textColor = stack.color;

                                //if ( adapter.EVENT.type == EventType.Repaint ) stack.style.Draw( stack.rect, "", false, false, false, false );
                                //if ( notol ) stack.style.Draw( stack.rect( worldOffset ), stack.self_ContentInstance, false, false, false, false );
                                //else GUI.Label( stack.rect( worldOffset ), stack.self_ContentInstance, stack.style );
                                gl._DrawLabel( stack.rect( worldOffset ), stack.self_ContentInstance, stack.style );

                                stack.style.normal.textColor = styleCacheColor;
                            }

                            else
                            {
                                //if ( notol ) stack.style.Draw( stack.rect( worldOffset ), stack.self_ContentInstance, false, false, false, false );
                                //else GUI.Label( stack.rect( worldOffset ), stack.self_ContentInstance, stack.style );
                                gl._DrawLabel( stack.rect( worldOffset ), stack.self_ContentInstance, stack.style );
                            }
                        }

                        if ( stack.nullable_Color.HasValue ) GUI.color = cache_c;

                        break;
                    }

                case ModulesDrawType.ModuleButton: //stack.self_SkipContentInstance ? null : stack.self_ContentInstance
                    {

                        //var drawRect = stack.rect( worldOffset );
                        //  GUI.Button(drawRect,"", adapter.button);
                        //int controlId = GUIUtility.GetControlID(PluginInstance.s_ButonHash, FocusType.Passive, drawRect);
                        //int controlId = GUIUtility.GetControlID(drawRect.GetHashCode(), FocusType.Passive, drawRect);
                        //var b = new GUIStyle(GUI.skin.button);

                        //b.fontSize = 8;
                        //int controlId = GUIUtility.GetControlID(FocusType.Passive,drawRect);
                        //if ( Event.current.type == EventType.Repaint ) adapter.STYLE_DEFBUTTON.Draw( drawRect, new GUIContent(), controlId, false, drawRect.Contains( Event.current.mousePosition ) );

                        //*if ( Event.current.type == EventType.Repaint ) GUI.skin.button.Draw( stack.rect( worldOffset ), new GUIContent(), id, false, stack.rect( worldOffset ).Contains( Event.current.mousePosition) );*/
                        //if (Event.current.type == EventType.MouseDrag) Debug.Log("ASD");
                        ///GUI.Button(stack.rect(worldOffset),"", id);
                        /*if (Event.current.type == EventType.Repaint && stack.rect( worldOffset ).Contains( Event.current.mousePosition)) GUI.DrawTexture(stack.rect(worldOffset), Texture2D.whiteTexture);*/

                        //	if (Event.current.type == EventType.MouseDown)
                        //	Debug.Log((stack.content != null && !string.IsNullOrEmpty(stack.content.text)).ToString() + " " + (adapter.hoverID == _o.id).ToString() + " " + _o.name);
                      
                        if ( stack.UseGameObjectEnable )
                        {
                            var a = _o.Active();

                            if ( !a )
                            {
                                alphacache = GUI.color;
                                GUI.color *= alpha;
                            }

                            if ( (stack.content != null && !string.IsNullOrEmpty( stack.content.text ) || stack.callFromExternal || !adapter.hashoveredItem || adapter.hoverID == _o.id) )
                            {
                                //  var r = stack.rect( worldOffset );
                                var r = stack.rect( worldOffset );
                                bool allowClick;
                                if ( adapter.ModuleButton( r, stack.content ?? emptyContent, stack.hasContent, stack.style, out allowClick ) )
                                {
                                    data.args = stack.args;
                                    data.content = !stack.self_SkipContentInstance ? stack.self_ContentInstance : null;

                                    stack.action.action( worldOffset, stack.rect( worldOffset ), data, _o );
                                }
                                if ( allowClick && stack.drawPointer ) EditorGUIUtility.AddCursorRect( r, MouseCursor.Link );
                            }
                            if ( !a ) GUI.color = alphacache;
                        }
                        else
                        {
                            if ( (stack.content != null && !string.IsNullOrEmpty( stack.content.text ) || stack.callFromExternal || !adapter.hashoveredItem || adapter.hoverID == _o.id) )
                            {
                                var r = stack.rect( worldOffset );

                                bool allowClick;
                                if ( adapter.ModuleButton( r, stack.content ?? emptyContent, stack.hasContent, stack.style, out allowClick ) )
                                {
                                    data.args = stack.args;
                                    data.content = !stack.self_SkipContentInstance ? stack.self_ContentInstance : null;

                                    stack.action.action( worldOffset, stack.rect( worldOffset ), data, _o );
                                }
                                if ( allowClick && stack.drawPointer ) EditorGUIUtility.AddCursorRect( r, MouseCursor.Link );
                            }
                        }

                        break;
                    }

                case ModulesDrawType.SimpleButton: //stack.self_SkipContentInstance ? null : stack.self_ContentInstance
                    {
                        if ( stack.UseGameObjectEnable )
                        {
                            var a = _o.Active();

                            if ( !a )
                            {
                                alphacache = GUI.color;
                                GUI.color *= alpha;
                            }

                            if ( (stack.content != null && !string.IsNullOrEmpty( stack.content.text ) || stack.callFromExternal || !adapter.hashoveredItem || adapter.hoverID == _o.id) )
                            {
                                var r = stack.rect( worldOffset );
                                if ( stack.drawPointer ) EditorGUIUtility.AddCursorRect( r, MouseCursor.Link );

                                if ( adapter.SimpleButton( r, stack.content ?? emptyContent, stack.style ) )
                                {
                                    data.args = stack.args;
                                    data.content = !stack.self_SkipContentInstance ? stack.self_ContentInstance : null;
                                    stack.action.action( worldOffset, stack.rect( worldOffset ), data, _o );
                                }
                            }
                            if ( !a ) GUI.color = alphacache;
                        }

                        else
                        {
                            if ( (stack.content != null && !string.IsNullOrEmpty( stack.content.text ) || stack.callFromExternal || !adapter.hashoveredItem || adapter.hoverID == _o.id) &&
                                adapter.SimpleButton( stack.rect( worldOffset ), stack.content ?? emptyContent, stack.style ) )
                            {
                                data.args = stack.args;
                                data.content = !stack.self_SkipContentInstance ? stack.self_ContentInstance : null;
                                stack.action.action( worldOffset, stack.rect( worldOffset ), data, _o );
                            }
                        }

                        break;
                    }
            }
        }
        static GUIContent emptyContent = new GUIContent();
        static DrawStackMethodsWrapperData data;
        static Color styleCacheColor;
        static Color alphacache = new Color(1, 1, 1, 0.5f);
        internal static Color alpha = new Color(1, 1, 1, 0.5f);
        internal bool DRAW_AS_EMPTY = false;
    }



    internal class DrawStackAdapter
    {
        [Obsolete]
        int pId;
        PluginInstance adapter;
        public virtual bool callFromExternal() { return false; }

        internal DrawStackAdapter( int pId )
        {

            //this.pId = pId;
           // adapter = Root.p[ pId ];
			adapter = Root.p[ 0 ];
			emptyStack = new DrawStack( pId, drawCallIndex, adapter.gl ) { DRAW_AS_EMPTY = true };
        }


        //  internal virtual Adapter adapter { get; set; }
        bool perfadditionalcondition = true;

        internal Rect ConvertToLocal( Rect rect )
        {
            rect.x -= GetCurrentStack().worldOffset.x;
            rect.y -= GetCurrentStack().worldOffset.y;
            return rect;
        }



        internal int drawCallIndex = 1;

        internal bool START_DRAW( Rect worldOffset, HierarchyObject _o, GlDrawer gl = null )
        {
            if ( PERFOMANCE_BARS )
            {
                //if (DRAW_STACK.ContainsKey(_o.id)) DRAW_STACK[_o.id].ResetStack();
                if ( TryToDraw( worldOffset, _o ) ) return false;

                CURRENT_STACK = StackInstance( _o, drawCallIndex, gl ?? adapter.gl );
                CURRENT_STACK.current_object = _o;
                CURRENT_STACK.worldOffset = worldOffset;
            }

            else
            {
#pragma warning disable
                emptyStack.current_object = _o;
                emptyStack.worldOffset = worldOffset;
#pragma warning restore
            }

            return true;
        }

        internal bool START_DRAW_PARTLY_TRYDRAW( Rect worldOffset, HierarchyObject _o )
        {
            if ( PERFOMANCE_BARS )
            {
                if ( TryToDraw( worldOffset, _o ) ) return false;
            }

            else { }

            return true;
        }

        internal void START_DRAW_PARTLY_CREATEINSTANCE( Rect worldOffset, HierarchyObject _o, bool additionalcondition, GlDrawer gl )
        {
            perfadditionalcondition = additionalcondition;

            if ( PERFOMANCE_BARS && additionalcondition )
            {
                CURRENT_STACK = StackInstance( _o, drawCallIndex, gl ?? adapter.gl );
                CURRENT_STACK.current_object = _o;
                CURRENT_STACK.worldOffset = worldOffset;
            }

            else
            {
                emptyStack.current_object = _o;
                emptyStack.worldOffset = worldOffset;
            }
        }

        internal void END_DRAW( HierarchyObject _o , int i)
        {
            if ( CURRENT_STACK != null )
            {
                CURRENT_STACK.Draw( ref CURRENT_STACK.worldOffset );

                if ( CURRENT_STACK.currentStackPos == -1 ) CURRENT_STACK.currentStackPos = -2;

				if (
                    //adapter.EVENT.type == EventType.Layout &&
                    i != -1 && _o.lastContentRectLayout[ i ].assigned ) _o.lastContentRectLayout[ i ].assigned = false;

				CURRENT_STACK = null;
            }
        }

        /*  internal virtual bool PERFOMANCE_BARS
          {
             // get { return adapter.par.CACHING_TEXTURES_STACKS; }
              get { return false; }
          }*/
        //protected bool PERFOMANCE_BARS = true;
        internal virtual bool PERFOMANCE_BARS {
            // get { return adapter.par.CACHING_TEXTURES_STACKS; }
            get { return true; }
        }

        DrawStack GetCurrentStack()
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) return CURRENT_STACK;

            return emptyStack;
        }

        internal void BEGIN_DRAW_SWITCHER( SwithcerMethodsWrapper method )
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.BEGIN_DRAW_SWITCHER( method );
            else emptyStack.DrawSIngleItem( emptyStack.BEGIN_DRAW_SWITCHER( method ) );
        }

        internal void END_DRAW_SWITCHER()
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.END_DRAW_SWITCHER();
            else emptyStack.DrawSIngleItem( emptyStack.END_DRAW_SWITCHER() );
        }

        internal DrawStack _emptyStack;
        internal DrawStack emptyStack {
            get {
                _emptyStack.gl = Root.p[ 0 ].gl;
                return _emptyStack;
            }
            set {
                _emptyStack = value;
            }
        }
        internal DrawStack CURRENT_STACK;

        internal void Draw_BeginClip( Rect rect )
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_BeginClip( rect );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_BeginClip( rect ) );
        }

        internal void Draw_EndClip()
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_EndClip();
            else emptyStack.DrawSIngleItem( emptyStack.Draw_EndClip() );
        }

        /*internal void Draw_GUITexture( Rect rect, Rect uv, IconData texture, Color? guiColor = null, bool USE_GO = false )
		{
			if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_GUITexture( rect, uv, texture, guiColor ?? Color.white, USE_GO );
			else emptyStack.DrawSIngleItem( emptyStack.Draw_GUITexture( rect, uv, texture, guiColor ?? Color.white, USE_GO ) );
		}*/
        internal void Draw_GUITexture( Rect rect, IconData texture, Color? guiColor = null, bool USE_GO = false )
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_GUITexture( rect, texture, guiColor ?? Color.white, USE_GO );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_GUITexture( rect, texture, guiColor ?? Color.white, USE_GO ) );
        }
        internal void Draw_GUITexture( Rect rect, IconData texture, Color? guiColor, Rect? clipRect, bool USE_GO = false )
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_GUITexture( rect, texture, guiColor ?? Color.white, USE_GO, ref clipRect );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_GUITexture( rect, texture, guiColor ?? Color.white, USE_GO, ref clipRect ) );
        }

        internal void Draw_GUITexture( Rect rect, Color color, Color? guiColor = null, bool USE_GO = false )
        {
            if ( guiColor.HasValue ) color *= guiColor.Value;

            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_GUITexture( rect, color, USE_GO );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_GUITexture( rect, color, USE_GO ) );
        }

        internal void Draw_AdapterTextureWithDynamicColor( Rect rect, DynamicColor color, bool USE_GO = false, int SWITCHER = 0 )
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_GUITexture( rect, Color.white, USE_GO, color ).ADD_SWITCHER( SWITCHER );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_GUITexture( rect, Color.white, USE_GO, color ).ADD_SWITCHER( SWITCHER ) );
        }

        internal void Draw_ScaleToFit_Texture( Rect rect, IconData texture, Color? guiColor = null, bool USE_GO = false )
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_ScaleToFit_Texture( rect, texture, guiColor ?? Color.white, USE_GO );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_ScaleToFit_Texture( rect, texture, guiColor ?? Color.white, USE_GO ) );
        }


        internal void Draw_GUITextureWithBorders( Rect rect, IconData texture, int borders, DynamicColor guiColor, object args, Rect? clipRect = null )
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_GUITextureWithBorders( rect, texture, borders, guiColor, false, args, clipRect );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_GUITextureWithBorders( rect, texture, borders, guiColor, false, args, clipRect ) );
        }

        internal void Draw_GUITextureWithBordersAndMaterial( Rect rect, IconData texture, int borders, Material mat, DynamicColor guiColor, object args, Rect? clipRect = null )
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_GUITextureWithBordersAndMaterial( rect, texture, borders, mat, guiColor, false, args, clipRect );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_GUITextureWithBordersAndMaterial( rect, texture, borders, mat, guiColor, false, args, clipRect ) );
        }

        internal void Draw_Style( Rect rect, GUIStyle style, Color? color = null, bool USE_GO = false )
        {
            var texture = Icons.GetIconDataFromTexture(style.normal.background ?? style.normal.scaledBackgrounds[0]);
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_GUITextureWithBorders( rect, texture, style.border.left, color ?? Color.white, USE_GO, null );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_GUITextureWithBorders( rect, texture, style.border.left, color ?? Color.white, USE_GO, null ) );
            /* if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_Style( rect, style, content, color, USE_GO );
			 else emptyStack.DrawSIngleItem( emptyStack.Draw_Style( rect, style, content, color, USE_GO ) );*/
        }

        /*     internal void Draw_Style(Rect rect, GUIStyle style, GUIContent content, Color? color = null, bool USE_GO = false)
			 {
				 var texture = Icons.GetIconDataFromTexture(style.normal.background ?? style.normal.scaledBackgrounds[0]);
				 if (PERFOMANCE_BARS && perfadditionalcondition)
				 {
					 CURRENT_STACK.Draw_GUITextureWithBorders(rect, texture, style.border.left, color ?? Color.white, USE_GO);
					 CURRENT_STACK.Draw_Label(rect, content, style, USE_GO, color, null);
				 }
				 else
				 {
					 emptyStack.DrawSIngleItem(emptyStack.Draw_GUITextureWithBorders(rect, texture, style.border.left, color ?? Color.white, USE_GO));
					 emptyStack.DrawSIngleItem(emptyStack.Draw_Label(rect, content, style, USE_GO, color, null));
				 }
			 }*/

        /*  internal void Draw_Style( Rect rect, GUIStyle style, string content, Color? color = null, bool USE_GO = false )
		  {
			  if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_Style( rect, style, content, color, USE_GO );
			  else emptyStack.DrawSIngleItem( emptyStack.Draw_Style( rect, style, content, color, USE_GO ) );
		  }*/

        internal void Draw_Action( Rect rect, DrawStackMethodsWrapper method, object args, int SHWITCHER )
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_Action( rect, method, args, null ).ADD_SWITCHER(SHWITCHER);
            else emptyStack.DrawSIngleItem( emptyStack.Draw_Action( rect, method, args, null ).ADD_SWITCHER(SHWITCHER) );
        }
        internal void Draw_Action( Rect rect, DrawStackMethodsWrapper method, object args, GUIContent content = null)
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_Action( rect, method, args, content );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_Action( rect, method, args, content ) );
        }

        internal void Draw_Label( Rect rect, GUIContent content, GUIStyle style, bool USE_GO, Color? color = null, bool? ADDITIONAL_ENABLE = null, Rect? clampRect = null )
        {
            if ( !adapter.par_e.RIGHT_DRAW_HYPHEN_FOR_EMPTY_LABELS && content != null && content.text == "-" ) return;

            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_Label( rect, content, style, USE_GO, color, ADDITIONAL_ENABLE, clampRect: clampRect );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_Label( rect, content, style, USE_GO, color, ADDITIONAL_ENABLE, clampRect: clampRect ) );
        }

        internal void Draw_Label( Rect rect, string content, GUIStyle style, bool USE_GO, Color? color = null, bool? ADDITIONAL_ENABLE = null, Rect? clampRect = null )
        {
            if ( !adapter.par_e.RIGHT_DRAW_HYPHEN_FOR_EMPTY_LABELS && content == "-" ) return;

            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_Label( rect, content, style, USE_GO, color, ADDITIONAL_ENABLE, clampRect: clampRect );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_Label( rect, content, style, USE_GO, color, ADDITIONAL_ENABLE, clampRect: clampRect ) );
        }

        internal void Draw_LabelWithTextColor( Rect rect, string content, Color textColor, GUIStyle style, bool USE_GO, Color? color = null, bool? ADDITIONAL_ENABLE = null, Rect? clampRect = null )
        { //if ( !adapter.DRAW_LABEL_FOR_EMPTY_CONTENT && content == "-" ) return;
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_LabelWithTextColor( rect, content, textColor, style, USE_GO, color, ADDITIONAL_ENABLE, clampRect: clampRect );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_LabelWithTextColor( rect, content, textColor, style, USE_GO, color, ADDITIONAL_ENABLE, clampRect: clampRect ) );
        }

        internal void Draw_LabelWithTextDynamicColor( Rect rect, string content, DynamicColor textColor, GUIStyle style, bool USE_GO, Color? color = null,
            bool? ADDITIONAL_ENABLE = null, int SWITCHER = 0, Rect? clampRect = null ) //if ( !adapter.DRAW_LABEL_FOR_EMPTY_CONTENT && content == "-" ) return;
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_LabelWithTextColor( rect, content, textColor, style, USE_GO, color, ADDITIONAL_ENABLE, clampRect: clampRect ).ADD_SWITCHER( SWITCHER );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_LabelWithTextColor( rect, content, textColor, style, USE_GO, color, ADDITIONAL_ENABLE, clampRect: clampRect ).ADD_SWITCHER( SWITCHER ) );
        }

        internal void Draw_ModuleButton( Rect rect, GUIContent content, DrawStackMethodsWrapper method, bool hasContent, object args = null, bool useContentForButton = false,
            GUIStyle style = null,
            bool USE_GO = false, bool drawPointer = false, Rect? clampRect = null )
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_ModuleButton( rect, content, method, hasContent, args,
                useContentForButton, style, USE_GO, callFromExternal(), drawPointer: drawPointer, clampRect: clampRect );
            else if ( PERFOMANCE_BARS ) emptyStack.DrawSIngleItem( emptyStack.Draw_ModuleButton( rect, content, method, hasContent, args,
                useContentForButton, style, USE_GO, callFromExternal(), drawPointer: drawPointer, clampRect: clampRect ) );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_SimpleButton( rect, content, method, args,
                useContentForButton, style, USE_GO, callFromExternal(), drawPointer: drawPointer, clampRect: clampRect ) );
        }

        internal void Draw_SimpleButton( Rect rect, GUIContent content, DrawStackMethodsWrapper method, object args = null, bool useContentForButton = false, GUIStyle style = null,
            bool USE_GO = false, Rect? clampRect = null )
        {
            if ( PERFOMANCE_BARS && perfadditionalcondition ) CURRENT_STACK.Draw_SimpleButton( rect, content, method, args,
                useContentForButton, style, USE_GO, clampRect: clampRect );
            else emptyStack.DrawSIngleItem( emptyStack.Draw_SimpleButton( rect, content, method, args,
                useContentForButton, style, USE_GO, clampRect: clampRect ) );
        }

        internal virtual void ResetStack()
        {
            foreach ( var item in DRAW_STACK )
            {
                item.Value.ResetStack();
            }

#if EMX_HIERARCHY_DEBUG_STACKS
			Debug.Log( "RESET_MODULE_STACK " + GetType().Name );
#endif
            //  adapter.RepaintWindowInUpdate();
        }

        internal virtual void ResetStack( int id, bool disableLog = false )
        {
            if ( !DRAW_STACK.ContainsKey( id ) ) return;

            DRAW_STACK[ id ].ResetStack();
#if EMX_HIERARCHY_DEBUG_STACKS
			if (!disableLog)  Debug.Log( "RESET_OBJECT_STACK " + GetType().Name + " - " + EditorUtility.InstanceIDToObject(id)?.name);

#endif
            //  Debug.Log( "ASD" );
            // adapter.RepaintWindowInUpdate();
        }

        internal Vector2 CLIP_VECTOR;

        internal Dictionary<int, DrawStack> DRAW_STACK = new Dictionary<int, DrawStack>();

        internal bool TryToDraw( Rect worldOffset, HierarchyObject o )
        {
            if ( !DRAW_STACK.ContainsKey( o.id ) ) { return false; }
            if ( DRAW_STACK[ o.id ].currentStackPos == -1 ) { return false; }

            if ( DRAW_STACK[ o.id ].GO_ENABLE_STATE != o.Active() )
            {

                DRAW_STACK[ o.id ].GO_ENABLE_STATE = o.Active();
                DRAW_STACK[ o.id ].ResetStack();
                return false;
            }

            DRAW_STACK[ o.id ].Draw( ref worldOffset );

            return true;
        }

        internal DrawStack StackInstance( HierarchyObject id, int drawCallIndex, GlDrawer gl )
        {
            if ( DRAW_STACK.ContainsKey( id.id ) ) return DRAW_STACK[ id.id ];

            var res = new DrawStack(id.pluginID, drawCallIndex, gl) { GO_ENABLE_STATE = id.Active() };
            DRAW_STACK.Add( id.id, res );
            return res;
        }

        internal void ClearDrawStack()
        {
            if ( DRAW_STACK.Count == 0 ) return;

            DRAW_STACK.Clear();
        }
    }

    internal struct DrawStackMethodsWrapperData
    {
        internal GUIContent content;
        internal object args;
    }

    internal class DrawStackMethodsWrapper
    {
        internal DrawStackMethodsWrapper( Action<Rect, Rect, DrawStackMethodsWrapperData, HierarchyObject> action )
        {
            this.action = action;
        }

        internal Action<Rect, Rect, DrawStackMethodsWrapperData, HierarchyObject> action = null;
    }

    internal class SwithcerMethodsWrapper
    {
        internal SwithcerMethodsWrapper( Func<HierarchyObject, int> action )
        {
            this.action = action;
        }

        internal Func<HierarchyObject, int> action = null;
    }

}
