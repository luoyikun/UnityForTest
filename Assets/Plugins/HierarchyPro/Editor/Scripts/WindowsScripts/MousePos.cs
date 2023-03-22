using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor
{

	internal struct WindowParams
    {
        internal float Width, Height;
    }

    internal struct MousePos
    {


        Vector2 _creenMp;
        internal Type type;
        internal bool Clamp;
        internal PluginInstance adapter;

        internal static Dictionary<Type, WindowParams> w_params = new Dictionary<Type, WindowParams>()
    {
        {Type.Input_128_68, new WindowParams() {Width = 128, Height = 68}}, //80
        {Type.Input_190_68, new WindowParams() {Width = 190, Height = 68}}, //80
        {Type.Search_356_0, new WindowParams() {Width = 356, Height = 0}},
        {Type.Highlighter_410_0, new WindowParams() {Width = 410, Height = 0}},
        {Type.ModulesListWindow_380_700, new WindowParams() {Width = 380, Height = 440}},
        {Type.ColorChanger_230_0, new WindowParams() {Width = 330, Height = 0}},
        {Type.SceneScanner_X_X, new WindowParams() {Width = 0, Height = 0}},
    };

        internal enum Type
        {
            Input_190_68,
            Input_128_68,
            Search_356_0,
            Highlighter_410_0,
            ModulesListWindow_380_700,
            ColorChanger_230_0,
            SceneScanner_X_X
        }

        internal MousePos( Vector2? mouse, Type type, bool Clamp, PluginInstance adapter )
        {
            if ( !mouse.HasValue )
            {
                Rect rect;
                /* if (adapter.window())
                 {   var p = adapter.window().position;
                     rect = new Rect( p.x + p.width / 2 - w_params[type].Width / 2, p.y + p.height / 2 - w_params[type].Height / 2, w_params[type].Width, w_params[type].Height );
                 }
                 else*/
                {
                    rect = new Rect( WinBounds.MAX_WINDOW_WIDTH.x + WinBounds.MAX_WINDOW_WIDTH.y / 2 - w_params[ type ].Width / 2,
                        WinBounds.MAX_WINDOW_HEIGHT.x + WinBounds.MAX_WINDOW_HEIGHT.y / 2 - w_params[ type ].Height / 2, w_params[ type ].Width, w_params[ type ].Height );
                }
                this._creenMp = new Vector2( rect.x, rect.y );
            }
            else
            {
                this._creenMp = EditorGUIUtility.GUIToScreenPoint( mouse.Value );
            }


            this.type = type;
            this.Clamp = Clamp;
            this.adapter = adapter;
            this.__Height = null;
            this.__Width = null;
        }

        internal MousePos( Rect mouse, Type type, bool Clamp, PluginInstance adapter )
        {
            this._creenMp = EditorGUIUtility.GUIToScreenPoint( new Vector2( mouse.x, mouse.y ) );
            this.type = type;
            this.Clamp = Clamp;
            this.adapter = adapter;
            this.__Height = null;
            this.__Width = null;
        }

        /*  internal void Set(Vector2 mouse)
         {   _creenMp = EditorGUIUtility.GUIToScreenPoint(mouse);
         }
        internal void Set(float x, float y)
         {   _creenMp = EditorGUIUtility.GUIToScreenPoint(new Vector2(x, y));
         }*/
        internal Vector2 ScreenMousePosition
        {
            get { return _creenMp; }
            set { _creenMp = value; }
        }

        internal Vector2 GUIMousePosition
        {
            get { return EditorGUIUtility.ScreenToGUIPoint( _creenMp ); }
        }

        internal Rect GetRect( Window a)
        {
            return WidnwoRect( this, Width, Height, a );
        }

        float? __Height;

        internal float Height
        {
            get { return __Height ?? w_params[ type ].Height; }
            set
            {
                if ( value == 0 ) throw new Exception( "value cannot be null" );
                __Height = value;
            }
        }

        float? __Width;

        internal float Width
        {
            get { return (__Width ?? 1) * w_params[ type ].Width; }
            set { __Width = value; }
        }

        internal float X
        {
            get { return _creenMp.x; }
            set { _creenMp.Set( value, _creenMp.y ); }
        }

        internal float Y
        {
            get { return _creenMp.y; }
            set { _creenMp.Set( _creenMp.x, value ); }
        }



        internal static Rect WidnwoRect( MousePos? _mouse, float width, float height, Window a, MousePos? savePosition = null
        /*, bool lockPos = false*/
        ) //if (!lockPos) width *= adapter.HALFFACTOR_8();
        {
            if ( savePosition.HasValue ) _mouse = savePosition.Value;
            // else mouse = GUIUtility.GUIToScreenPoint( mouse );

            var mouse = _mouse.Value.ScreenMousePosition;

            var hierWin = a.Instance;
            // MonoBehaviour.print(hierWin);

            if ( hierWin != null && _mouse.Value.Clamp ) //   if (width > hierWin.position.width) width = hierWin.position.width;
            { // if (height > hierWin.position.height - 45) height = hierWin.position.height - 65;
                /*  if (height > Screen.currentResolution.height - 100) height = Screen.currentResolution.height - 100;


                  if (mouse.x < hierWin.position.x) mouse.x = hierWin.position.x;
                  if (mouse.y < hierWin.position.y) mouse.y = hierWin.position.y;
                  if (mouse.x - hierWin.position.x + width > hierWin.position.width) mouse.x = hierWin.position.x + hierWin.position.width - width;
                  if (mouse.y - hierWin.position.y + height > hierWin.position.height) mouse.y = hierWin.position.y + hierWin.position.height - height - 65;*/

                //  var max = Screen.currentResolution.height * (HEIGH_CLAMPER ?? 0.9f);
                var max = Windows.IWindow.MAX_HEIGHT(a);
                if ( height > max ) height = max;


                if ( mouse.x < hierWin.position.x ) mouse.x = hierWin.position.x;
                if ( mouse.y < hierWin.position.y ) mouse.y = hierWin.position.y;
                if ( mouse.x - hierWin.position.x + width > hierWin.position.width ) mouse.x = hierWin.position.x + hierWin.position.width - width;
                if ( mouse.y - hierWin.position.y + height > hierWin.position.height )
                {
                    if ( mouse.y - hierWin.position.y > height )
                    {
                        mouse.y = mouse.y - height - EditorGUIUtility.singleLineHeight * 2;
                    }
                    else
                    {
                        mouse.y = hierWin.position.y + hierWin.position.height - height;
                    }
                }
            }
            else
            {
                if ( mouse.y - WinBounds.MAX_WINDOW_HEIGHT.x + height > WinBounds.MAX_WINDOW_HEIGHT.y )
                {
                    if ( mouse.y - WinBounds.MAX_WINDOW_HEIGHT.x > height )
                    {
                        mouse.y = mouse.y - height - EditorGUIUtility.singleLineHeight * 2;
                    }
                    else
                    {
                        mouse.y = WinBounds.MAX_WINDOW_HEIGHT.x + WinBounds.MAX_WINDOW_HEIGHT.y - height;
                    }
                }


                /*Debug.Log(Adapter.MAX_WINDOW_WIDTH);
                Debug.Log(Adapter.MAX_WINDOW_HEIGHT);*/
                /* EditorGUIUtility.mo*/
                /*   if (mouse.y  + height > Adapter.MAX_WINDOW_HEIGHT)
                   {   if (mouse.y  > height)
                       {   mouse.y = mouse.y - height - EditorGUIUtility.singleLineHeight * 2;
                       }
                       else
                       {   mouse.y = Adapter.MAX_WINDOW_HEIGHT - height;
                       }

                   }*/
            }

            //else
            { //var W = Screen.currentResolution.width;
              // var H = Screen.currentResolution.height;

                /* foreach (var item in Resources.FindObjectsOfTypeAll<EditorWindow>())
                 {   Debug.Log(item.GetType().Name);
                 }*/
                /* foreach (var item in Resources.FindObjectsOfTypeAll(typeof(EditorWindow).Assembly.GetType("UnityEngine.UIElements.Panel") ))
                 {

                     var n =  typeof(EditorWindow).Assembly.GetType("UnityEngine.UIElements.Panel").GetField("m_PanelName", (System.Reflection.BindingFlags)(-1)).GetValue(item);
                     Debug.Log(n);
                 }*/

                // float TOP = float.MaxValue, RIGHT = float.MinValue, LEFT = float.MaxValue, BOTTOM = float.MinValue;

                Rect windRect = WinBounds.WINBORDER;
                float RIGHT = windRect.x + windRect.width;
                float BOTTOM = windRect.y + windRect.height;

                //Debug.Log(windRect);
                /*var all_t = typeof(EditorWindow).Assembly.GetType("UnityEditor.ContainerWindow");
                var all_list = all_t.GetField("s_AllWindows", (System.Reflection.BindingFlags)(-1)).GetValue(null) as System.Collections.IList;
                Debug.Log(all_list.Count);
                var all_rect = all_t.GetField("m_PixelRect", (System.Reflection.BindingFlags)(-1));
                foreach (var item in all_list)
                {   Debug.Log(all_rect.GetValue(item));
                }*/

                var P = 5;
                if ( mouse.x < P + windRect.x ) mouse.x = P + windRect.x;
                if ( mouse.y < P + 15 + windRect.y ) mouse.x = P + 15 + windRect.y;
                if ( mouse.x + width + P > RIGHT ) mouse.x = RIGHT - width - P;
                if ( mouse.y + height + P > BOTTOM ) mouse.y = BOTTOM - height - P;
                //  if (mouse.y - hierWin.position.y + height > hierWin.position.height) mouse.y = hierWin.position.y + hierWin.position.height - height;
            }
            // if (mouse.y < 20) mouse.y = 20;
            // if (mouse.x < 0) mouse.x = 0;
            var res = new Rect(mouse.x, mouse.y, width, height);


            return res;
        }



        internal void SetType( Type type )
        {
            this.type = type;
        }
    }


}
