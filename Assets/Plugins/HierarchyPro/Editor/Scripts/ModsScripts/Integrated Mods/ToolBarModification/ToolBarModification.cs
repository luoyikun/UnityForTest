#if UNITY_2021_1_OR_NEWER //2021
#define U2021
#endif
#if UNITY_2020_1_OR_NEWER //2020
#define U2020
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

#if U2021
using UnityEngine.UIElements;
#endif

namespace EMX.CustomizationHierarchy
{
    public class TopGUI
    {
        /// <summary>
        /// Rect - windowSize;
        /// </summary>
        static public Action<Rect> onLeftLayoutGUI;

        /// <summary>
        /// Rect - windowSize;
        /// </summary>
        static public Action<Rect> onRightLayoutGUI;

        static GUIContent _temp_content = new GUIContent();

        /// <summary>
        /// Button same as GUI.Button but uses special editor style;
        /// </summary>
        static public bool Button(Rect r, string text)
        {
            _temp_content.text = _temp_content.tooltip = text;
            return Button(r, _temp_content);
        }
        /// <summary>
        /// Button same as GUI.Button but uses special editor style;
        /// </summary>
        static public bool Button(Rect r, GUIContent text)
        {
            EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
            return GUI.Button(r, text, EMX.HierarchyPlugin.Editor.Root.p[0].STYLE_DEFBUTTON);
        }
        /// <summary>
        /// Button same as GUI.Button but uses special editor style;
        /// </summary>
        static public bool ButtonFitContent(ref Rect r, string text)
        {
            _temp_content.text = _temp_content.tooltip = text;
            return ButtonFitContent(ref r, _temp_content);
        }
        /// <summary>
        /// Button same as GUI.Button but uses special editor style;
        /// </summary>
        static public bool ButtonFitContent(ref Rect r, GUIContent text)
        {
            r.width = EMX.HierarchyPlugin.Editor.Root.p[0].STYLE_DEFBUTTON.CalcSize(text).x;
            EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
            return GUI.Button(r, text, EMX.HierarchyPlugin.Editor.Root.p[0].STYLE_DEFBUTTON);
        }
    }
}

namespace EMX.HierarchyPlugin.Editor.Mods
{



    //   [InitializeOnLoad]
    class ToolBarModification
    {

        PluginInstance p;
        internal HotButtons hotButtons;
        internal LayoutsMod layoutsMod;
        internal ToolBarModification(PluginInstance p)
        {
            this.p = p;
            hotButtons = new HotButtons(p);
            layoutsMod = new LayoutsMod(p);
        }



        // static object chld;

        // static ScriptableObject last;
        //  static EventInfo   disable;
        object oldtb;
        Type tb;
        FieldInfo gettb;
        object gettvvalue {
            get {
                var _tb = tb ?? (tb = typeof(EditorWindow).Assembly.GetType("UnityEditor.Toolbar") ?? throw new Exception("Toolbar")) ?? throw new Exception("Toolbar");
                return (gettb ?? (gettb = _tb.GetField("get", ~(BindingFlags.Instance)))).GetValue(null);
            }
        }
        void Update()
        {

            var newtb = gettvvalue;
            if (!ReferenceEquals(newtb, oldtb))
            {
                oldtb = newtb;
                if (!p.par_e.DRAW_TOPBAR_LAYOUTS_BAR || !p.par_e.ENABLE_ALL) return;
                //Debug.Log("A" + ReferenceEquals(newtb, oldtb));
                _install(false, false);
            }

            if (!installed) return;
            reinstall();
        }
        bool installed = false;
        Action reinstall = null;
        EditorSettingsAdapter Local_Root { get { return p.par_e; } }

        internal void Remove(EditorSubscriber sbs)
        {
            if (!oldAssigned) return;
            oldAssigned = false;
            Install(sbs, false, true);
        }
        bool oldAssigned = false;
        static object lastMain = null;
        internal void Install(EditorSubscriber sbs, bool resinstall, bool remove = false)
        {

            if (sbs != null)
            {
                sbs.OnUpdate += Update;
                // EditorApplication.hierarchyChanged += () => {  Update(); };
                sbs.OnPlayModeStateChanged += () => { Update(); };
                // sbs.BuildedOnGUI_first.Add( () => { Update(); };
                if (p.par_e.DRAW_TOPBAR_LAYOUTS_BAR && p.par_e.ENABLE_ALL) layoutsMod.Subscribe(sbs);

                layoutsMod.ClearTimers();
            }

            _install(resinstall, remove);

        }

#pragma warning disable
        bool? oldSwap = null;
        Rect? oldLocalBounds = null;
        internal const int MARGIN_Y = 0;
#pragma warning restore
#if !U2021 //2021
        object oldContainer = null;
        Action customGui;
#endif
        void _install(bool resinstall, bool remove)
        {

            if (resinstall)
            {
                lastMain = null;
            }
            if (remove)
            {
                lastMain = null;
                SessionState.GetBool("EXM_TOOLBAR_BOOL", false);
            }
            // Debug.Log( ReferenceEquals( lastMain, gettvvalue ) );

            if (ReferenceEquals(lastMain, gettvvalue)) return;
            lastMain = gettvvalue;

            var b = typeof(EditorWindow).Assembly.GetType("UnityEditor.View") ?? throw new Exception("View");
            var v = typeof(EditorWindow).Assembly.GetType("UnityEditor.GUIView") ?? throw new Exception("GUIView");
            var tb = typeof(EditorWindow).Assembly.GetType("UnityEditor.Toolbar") ?? throw new Exception("Toolbar");
            var ll = tb.GetProperty("lastLoadedLayoutName", ~(BindingFlags.Instance)) ?? throw new Exception("lastLoadedLayoutName");
            var p3 = b.GetProperty("position", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("position");
#if U2021 //2021

            //var be = v.GetProperty("windowBackend", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("windowBackend");
            //var t2 = typeof(UnityEditor.UIElements.Toolbar).Assembly.GetType("UnityEditor.UIElements.DefaultWindowBackend") ?? throw new Exception("DefaultWindowBackend");
            //var visualTree = t2.GetField("visualTree", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("visualTree");
            var get = tb.GetField("get", ~(BindingFlags.Instance)) ?? throw new Exception("get");
            var m_Root = tb.GetField("m_Root", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("m_Root");
            var tb_instance = get.GetValue(null) ?? throw new Exception("tb_instance");
            var m_Root_instance = m_Root.GetValue(tb_instance) as VisualElement ?? throw new Exception("VisualElement");

            //             m_Root_instance.style.overflow = Overflow.Visible;
            //             m_Root_instance.hierarchy[ 0 ].style.overflow = Overflow.Visible;
            //             m_Root_instance.hierarchy[ 0 ].hierarchy[ 0 ].style.overflow = Overflow.Visible;
            //             m_Root_instance.hierarchy[ 0 ].hierarchy[ 0 ].hierarchy[ 0 ].style.overflow = Overflow.Visible;
            //             m_Root_instance.style.unityOverflowClipBox = OverflowClipBox.ContentBox;
            //             m_Root_instance.hierarchy[ 0 ].style.unityOverflowClipBox = OverflowClipBox.ContentBox;
            //             m_Root_instance.hierarchy[ 0 ].hierarchy[ 0 ].style.unityOverflowClipBox = OverflowClipBox.ContentBox;
            //             m_Root_instance.hierarchy[ 0 ].hierarchy[ 0 ].hierarchy[ 0 ].style.unityOverflowClipBox = OverflowClipBox.ContentBox;
            // 

            var target = m_Root_instance.hierarchy[0].hierarchy[0].hierarchy[0];
            var l = target.hierarchy[0]; // 
            // var h  = l.style.height;
            // Action<Button> SetupButton = ( Button button ) =>
            //              {
            //                  //var buttonIcon = button.Q(className: "quicktool-button-icon");
            //                  //var iconPath = "Icons/" + button.parent.name + "-icon";
            //                 // var iconAsset = Resources.Load<Texture2D>(iconPath);
            //                  //button.style.backgroundImage = iconAsset;
            //                  //button.clickable.clicked += () => CreateObject( button.parent.name );
            //                  button.tooltip = "QWE";
            //                  button.text = "ASD";
            //                  button.style.height = h;
            //                  button.style.width = h;
            //                  button.style.paddingTop = l.style.paddingTop;
            //                  button.style.paddingBottom = l.style.paddingBottom;
            //                  button.style.marginTop = l.style.marginTop;
            //                  button.style.marginBottom = l.style.marginBottom;
            //                  button.style.marginTop = button.style.marginBottom = 0;
            //                  button.style.flexDirection = FlexDirection.Row;
            //                  //button.style.flexWrap = Wrap.Wrap;
            //                 // button.style.flexGrow = 1;
            //              };
            //
            // var button = new Button();
            // SetupButton( button );
            // m_Root_instance.hierarchy[ 0 ].hierarchy[ 0 ].hierarchy[ 0 ].Add( button );
            //            m_Root_instance.hierarchy.( button );
            //var toolButtons = root.Query<Button>();
            //toolButtons.ForEach( SetupButton );

            var defaultMargin = 9;

            Action<IMGUIContainer> SetupIMGUI = ( IMGUIContainer button ) =>
                         {
                             //button.style.height = h;
                            // Debug.Log(m_Root_instance.hierarchy[0].h.contentRect.height);
                            // button.style.height = m_Root_instance.contentRect.height;
                            
                             button.style.paddingTop = l.style.paddingTop;
                             button.style.paddingBottom = l.style.paddingBottom;
                             button.style.marginTop = l.style.marginTop;
                             button.style.marginBottom = l.style.marginBottom;
                             button.style.marginTop = button.style.marginBottom = -MARGIN_Y;
//                              button.style.overflow = Overflow.Visible;
//                              button.style.unityOverflowClipBox = OverflowClipBox.ContentBox;
// //                              button.style.borderBottomLeftRadius =
//                              button.style.borderBottomRightRadius =
//                              button.style.borderTopLeftRadius =
//                              button.style.borderTopRightRadius =3;
                             button.style.marginLeft = button.style.marginRight = defaultMargin;
                             button.style.flexDirection = FlexDirection.Row;
                             //button.style.flexWrap = Wrap.Wrap;
                             button.style.flexGrow = 1;
                         };

           
            if ( remove )
            {
                List<int> remove_intdexes_list = new List<int>();
                var visualElement_target = m_Root_instance.hierarchy[ 0 ].hierarchy[ 0 ].hierarchy[ 0 ];
               // Debug.Log(visualElement_target.childCount);
                for ( int i = 0; i < visualElement_target.childCount; i++ ) 
                    { 
                       // Debug.Log(visualElement_target.hierarchy[ i ].viewDataKey);
                    if ( visualElement_target.hierarchy[ i ].viewDataKey == "imgui_emxh_left" ) remove_intdexes_list.Add( i ); }
                for ( int i = remove_intdexes_list.Count - 1; i >= 0; i-- ) visualElement_target.RemoveAt( remove_intdexes_list[ i ] );

                remove_intdexes_list.Clear();
                visualElement_target = m_Root_instance.hierarchy[ 0 ].hierarchy[ 0 ].hierarchy[ 2 ];
                for ( int i = 0; i < visualElement_target.childCount; i++ ) if ( visualElement_target.hierarchy[ i ].viewDataKey == "imgui_emxh_right" ) remove_intdexes_list.Add( i );
                for ( int i = remove_intdexes_list.Count - 1; i >= 0; i-- ) visualElement_target.RemoveAt( remove_intdexes_list[ i ] );

            }
            else
            {

                var imgui_left = new UnityEngine.UIElements.IMGUIContainer();
                SetupIMGUI( imgui_left );
                imgui_left.cullingEnabled = true;
                m_Root_instance.hierarchy[ 0 ].hierarchy[ 0 ].hierarchy[ 0 ].Add( imgui_left );
                imgui_left.viewDataKey = "imgui_emxh_left";

                imgui_left.onGUIHandler = () =>
                   {

                       if (!oldLocalBounds.HasValue || oldLocalBounds.Value.width != m_Root_instance.localBound.width)
                       {
                           oldLocalBounds = m_Root_instance.localBound;
                           layoutsMod.ClearTimers();
                       }

                       if (Local_Root.TOPBAR_LEFT_MIN_BORDER_OFFSET != (int)imgui_left.style.marginLeft.value.value - defaultMargin)
                       {
                           imgui_left.style.marginLeft = defaultMargin + Local_Root.TOPBAR_LEFT_MIN_BORDER_OFFSET;
                           layoutsMod.ClearTimers();
                       }
                       if (Local_Root.TOPBAR_LEFT_MAX_BORDER_OFFSET != (int)imgui_left.style.marginRight.value.value - defaultMargin)
                       {
                           imgui_left.style.marginRight = defaultMargin + Local_Root.TOPBAR_LEFT_MAX_BORDER_OFFSET;
                           layoutsMod.ClearTimers();
                       }
                       var _b1 = imgui_left.contentRect;

                       gui_method_left(ref _b1);


                       //                      // if ( EditorGUIUtility.hotControl != 0 ) Debug.Log( EditorGUIUtility.hotControl );
                       //
                       //                       if ( p.par_e.TOPBAR_LEFT_MIN_BORDER_OFFSET != (int)imgui_left.style.marginLeft.value.value - defaultMargin )
                       //                           imgui_left.style.marginLeft = defaultMargin + p.par_e.TOPBAR_LEFT_MIN_BORDER_OFFSET;
                       //                       if ( p.par_e.TOPBAR_LEFT_MAX_BORDER_OFFSET != (int)imgui_left.style.marginRight.value.value - defaultMargin )
                       //                           imgui_left.style.marginRight = defaultMargin + p.par_e.TOPBAR_LEFT_MAX_BORDER_OFFSET;
                       //
                       //
                       //                   // GUI.Label(new Rect(0,0,20,20), "ASD");
                       //                   /*    float o1 = 416f;
                       //                       var _p = (Rect)p3.GetValue(t[0], null);
                       //                       float shrink = (_p.width - 1000) / 920 * 50;
                       //                       int _bp = Mathf.RoundToInt((float)((_p.width - 140.0) / 2.0));
                       //                       var _b1 = new Rect(o1, 0, _bp - o1 - shrink - 10, _p.height);
                       //                       _b1.x += p.par_e.TOPBAR_LEFT_MIN_BORDER_OFFSET;
                       //                       _b1.width -= p.par_e.TOPBAR_LEFT_MIN_BORDER_OFFSET - p.par_e.TOPBAR_LEFT_MAX_BORDER_OFFSET;
                       //    */
                       //                       var _b1 = imgui_left.contentRect;
                       //
                       //
                       //                       GUILayout.BeginArea( _b1 );
                       //                       GUILayout.BeginHorizontal();
                       //                       if ( p.par_e.TOPBAR_SWAP_LEFT_RIGHT )
                       //                       {
                       //                           if ( p.par_e.DRAW_TOPBAR_LAYOUTS_BAR )
                       //                               layoutsMod.DrawLayers();
                       //                       }
                       //                       else if ( p.par_e.DRAW_TOPBAR_HOTBUTTONS )
                       //                       {
                       //
                       //#if !EMX_H_LITE
                       //                           hotButtons.DrawButtonsOnTopBar();
                       //#endif
                       //
                       //                       }
                       //                   /*  if (GUILayout.Button("save" , GUILayout.ExpandHeight(true)))
                       //                     {
                       //                         chld = c.GetValue(m);
                       //                     }
                       //                     if (GUILayout.Button("load" , GUILayout.ExpandHeight(true)))
                       //                     {
                       //                         c.SetValue(m, chld);
                       //                         Install(true);
                       //                         InternalEditorUtility.RepaintAllViews();
                       //                     }*/
                       //                       if ( p.par_e.DRAW_TOPBAR_CUSTOM_LEFT )
                       //                           if ( EMX.CustomizationHierarchy.TopGUI.onLeftLayoutGUI != null )
                       //                           {
                       //                               _b1 = EditorGUILayout.GetControlRect( GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );
                       //                               _b1.y += MARGIN_Y;
                       //                               _b1.height -= MARGIN_Y * 2;
                       //                               EMX.CustomizationHierarchy.TopGUI.onLeftLayoutGUI( _b1 ); //_p was
                       //                       }
                       //                       GUILayout.EndHorizontal();
                       //                       GUILayout.EndArea();
                       //
                       //
                       //                   /*   int o2 = Mathf.RoundToInt((float)((_p.width + 40) / 2.0) + shrink);
                       //                      o2 += 16;
                       //                      var _b2 = new Rect(o2, 0, (_p.width - 850) / 2.2f - shrink, _p.height);
                       //                      _b2.x += p.par_e.TOPBAR_RIGHT_MIN_BORDER_OFFSET;
                       //                      _b2.width -= p.par_e.TOPBAR_RIGHT_MIN_BORDER_OFFSET - p.par_e.TOPBAR_RIGHT_MAX_BORDER_OFFSET;
                       //
                       //                      GUILayout.BeginArea( _b2 );
                       //                      GUILayout.BeginHorizontal();
                       //                      if ( p.par_e.TOPBAR_SWAP_LEFT_RIGHT ) {
                       //#if !EMX_H_LITE
                       //                      if ( p.par_e.DRAW_TOPBAR_HOTBUTTONS ) hotButtons.DrawButtonsOnTopBar();
                       //#endif
                       //                      }
                       //                      else if ( p.par_e.DRAW_TOPBAR_LAYOUTS_BAR ) layoutsMod.DrawLayers();
                       //                      if ( p.par_e.DRAW_TOPBAR_CUSTOM_RIGHT ) if ( EMX.CustomizationHierarchy.TopGUI.onRightLayoutGUI != null ) {
                       //                        _b1 = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                       //                            _b1.y += MARGIN_Y;
                       //                            _b1.height -= MARGIN_Y * 2;
                       //                      EMX.CustomizationHierarchy.TopGUI.onRightLayoutGUI( _b1 );
                       //                      }
                       //                      GUILayout.EndHorizontal();
                       //                      GUILayout.EndArea();*/

                   };

                var imgui_right = new UnityEngine.UIElements.IMGUIContainer();
                SetupIMGUI( imgui_right );
                imgui_right.cullingEnabled = true;
                
                m_Root_instance.hierarchy[ 0 ].hierarchy[ 0 ].hierarchy[ 2 ].Add( imgui_right );
                imgui_right.viewDataKey = "imgui_emxh_right";

                imgui_right.onGUIHandler = () =>
                   {

                       if (!oldLocalBounds.HasValue || oldLocalBounds.Value.width != m_Root_instance.localBound.width)
                       {
                           oldLocalBounds = m_Root_instance.localBound;
                           layoutsMod.ClearTimers();
                       }


                       if (Local_Root.TOPBAR_RIGHT_MIN_BORDER_OFFSET != (int)imgui_right.style.marginLeft.value.value - defaultMargin)
                       {
                           imgui_right.style.marginLeft = defaultMargin + Local_Root.TOPBAR_RIGHT_MIN_BORDER_OFFSET;
                           layoutsMod.ClearTimers();
                       }
                       if (Local_Root.TOPBAR_RIGHT_MAX_BORDER_OFFSET != (int)imgui_right.style.marginRight.value.value - defaultMargin)
                       {
                           imgui_right.style.marginRight = defaultMargin + Local_Root.TOPBAR_RIGHT_MAX_BORDER_OFFSET;
                           layoutsMod.ClearTimers();
                       }

                       var _b2 = imgui_right.contentRect;

                       gui_method_right(ref _b2);


                       //                       if (!oldLocalBounds.HasValue || oldLocalBounds.Value.width != m_Root_instance.localBound.width )
                       //					   {
                       //                           oldLocalBounds = m_Root_instance.localBound;
                       //                           layoutsMod.ClearTimers();
                       //					   }
                       //
                       //
                       //                       if ( p.par_e.TOPBAR_RIGHT_MIN_BORDER_OFFSET != (int)imgui_right.style.marginLeft.value.value - defaultMargin )
                       //                       { 
                       //                           imgui_right.style.marginLeft = defaultMargin + p.par_e.TOPBAR_RIGHT_MIN_BORDER_OFFSET;
                       //						   layoutsMod.ClearTimers();
                       //					   }
                       //					   if ( p.par_e.TOPBAR_RIGHT_MAX_BORDER_OFFSET != (int)imgui_right.style.marginRight.value.value - defaultMargin )
                       //                       {
                       //                           imgui_right.style.marginRight = defaultMargin + p.par_e.TOPBAR_RIGHT_MAX_BORDER_OFFSET;
                       //						   layoutsMod.ClearTimers();
                       //					   }
                       //
                       //					   var _b2 = imgui_right.contentRect;
                       //
                       //                   /*  int o2 = Mathf.RoundToInt((float)((_p.width + 40) / 2.0) + shrink);
                       //                     o2 += 16;
                       //                     var _b2 = new Rect(o2, 0, (_p.width - 850) / 2.2f - shrink, _p.height);
                       //                     _b2.x += p.par_e.TOPBAR_RIGHT_MIN_BORDER_OFFSET;
                       //                     _b2.width -= p.par_e.TOPBAR_RIGHT_MIN_BORDER_OFFSET - p.par_e.TOPBAR_RIGHT_MAX_BORDER_OFFSET;
                       //  */
                       //                       GUILayout.BeginArea( _b2 );
                       //                       GUILayout.BeginHorizontal();
                       //
                       //                       if (p.par_e.TOPBAR_SWAP_LEFT_RIGHT != oldSwap )
                       //					   {
                       //                           oldSwap = p.par_e.TOPBAR_SWAP_LEFT_RIGHT;
                       //						   layoutsMod.ClearTimers();
                       //					   }
                       //
                       //                       if ( p.par_e.TOPBAR_SWAP_LEFT_RIGHT )
                       //                       {
                       //                           GUILayout.FlexibleSpace();
                       //                           if ( p.par_e.DRAW_TOPBAR_CUSTOM_RIGHT )
                       //                               if ( EMX.CustomizationHierarchy.TopGUI.onRightLayoutGUI != null )
                       //                               {
                       //                                   _b2 = EditorGUILayout.GetControlRect( GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );
                       //                                   _b2.y += MARGIN_Y;
                       //                                   _b2.height -= MARGIN_Y * 2;
                       //                                   EMX.CustomizationHierarchy.TopGUI.onRightLayoutGUI( _b2 ); //_p was
                       //                           }
                       //#if !EMX_H_LITE
                       //                           if ( p.par_e.DRAW_TOPBAR_HOTBUTTONS ) { hotButtons.DrawButtonsOnTopBar(); }
                       //#endif
                       //                       }
                       //                       else
                       //                       {
                       //
                       //                           if ( p.par_e.DRAW_TOPBAR_LAYOUTS_BAR ) layoutsMod.DrawLayers();
                       //
                       //                           if ( p.par_e.DRAW_TOPBAR_CUSTOM_RIGHT )
                       //                               if ( EMX.CustomizationHierarchy.TopGUI.onRightLayoutGUI != null )
                       //                               {
                       //                                   _b2 = EditorGUILayout.GetControlRect( GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );
                       //                                   _b2.y += MARGIN_Y;
                       //                                   _b2.height -= MARGIN_Y * 2;
                       //                                   EMX.CustomizationHierarchy.TopGUI.onRightLayoutGUI( _b2 ); //_p was
                       //                           }
                       //                       }
                       //
                       //
                       //                       GUILayout.EndHorizontal();
                       //                       GUILayout.EndArea();

                   };

                oldAssigned = true;

            }

#else

#if U2020
            var be = v.GetProperty("windowBackend", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("windowBackend");
			var t2 = typeof(UnityEditor.UIElements.Toolbar).Assembly.GetType("UnityEditor.UIElements.DefaultWindowBackend") ?? throw new Exception("DefaultWindowBackend");
			var r = t2.GetField("imguiContainer", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("imguiContainer");
			var ongui_type = r.FieldType;
			Func<object, object> get_r = (w) =>
			 {
				 var def_w = be.GetValue(w, null);
				 return r.GetValue(def_w);
			 };
			Action<object, object> set_r = (w, s) =>
			{
				var def_w = be.GetValue(w, null);
				r.SetValue(def_w, s);
			};
#else
            var r = v.GetProperty("imguiContainer", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("imguiContainer");
            var ongui_type = r.PropertyType;
            Func<object, object> get_r = (w) => {
                return r.GetValue(w, null);
            };
            Action<object, object> set_r = (w, s) => {
                r.SetValue(w, s, null);
            };
            //var pp = v.GetProperty("panel", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("panel");
            //var mp = v.GetField("m_Panel", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("m_Panel");
#endif

            var ongui = ongui_type.GetField("m_OnGUIHandler", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("m_OnGUIHandler");
            var c = b.GetField("m_Children", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("m_Children");
            ScriptableObject m = null;
            List<ScriptableObject> targets = new List<ScriptableObject>();
            foreach (var item in Resources.FindObjectsOfTypeAll<ScriptableObject>())
            {
                if (!b.IsAssignableFrom(item.GetType())) continue;
                if (item.GetType().Name != "MainView") continue;
                // var test = (object[])c.GetValue(item);
                // var es = test[0] as ScriptableObject;
                // if ( !es ) continue;
                /* var test = (object[])c.GetValue(item);
                 if ( resinstall && r.GetValue( test[ 0 ], null ) == null )
                 {
                     Debug.Log( "skip" );
                     continue;
                 }*/
                targets.Add(item);
                //m = item;
                break;
            }

            installed = false;
            //  Debug.Log( targets.Count );
            if (targets.Count == 0)
            {
                if (remove) throw new Exception("Remove Exception");
                EditorApplication.delayCall += () => {
                    Install(null, true);
                };
                return;
            }



            //  var OnDisable= t[ 0 ].GetType().GetMethod("OnDisable",  ~(BindingFlags.InvokeMethod | BindingFlags.Static));
            //  var OnEnable=  t[ 0 ].GetType().GetMethod("OnEnable",  ~(BindingFlags.InvokeMethod | BindingFlags.Static));

            /*  EditorApplication.CallbackFunction up = null;
              up = () =>
              {
                  if (r.GetValue(t[0], null) == null)
                  {
                      EditorApplication.update -= up;
                      // Debug.Log(last);
                      //  Debug.Log((bool)last);
                      // if (disable != null) disable.DynamicInvoke(null);
                      // Application.Run(exFormAsObj);
                      //  disable.
                      // if ( last ) OnDisable.Invoke( last, null );
                      Install(null, true);
                  }
              };
              EditorApplication.update += up;*/


            m = targets[0];
            var t = (object[])c.GetValue(m);

            // Debug.Log( "install" );
            object inst;
            MethodInfo OldOnGUI = null;
            if (!remove)
            {

                installed = true;
                reinstall = () => {
                    if (get_r(t[0]) == null)
                    {
                        Install(null, true);
                    }
                };



                customGui = () => {

                    (OldOnGUI ?? (OldOnGUI = t[0].GetType().GetMethod("OldOnGUI", ~(BindingFlags.GetProperty | BindingFlags.Static)))).Invoke(t[0], null);


                    // if (Local_Root.LayoutsButtonsHelper._oneShotGui.Count != 0)
                    // {
                    //     var ex = Local_Root.LayoutsButtonsHelper._oneShotGui.ToArray();
                    //     Local_Root.LayoutsButtonsHelper._oneShotGui.Clear();
                    //     foreach (var item in ex) item();
                    // }

                    // GUI.Label(new Rect(0,0,20,20), "ASD");
                    float o1 = 416f;
                    var _p = (Rect)p3.GetValue(t[0], null);
                    float shrink = (_p.width - 1000) / 920 * 50;
                    int _bp = Mathf.RoundToInt((float)((_p.width - 140.0) / 2.0));
                    var _b1 = new Rect(o1, 0, _bp - o1 - shrink - 10, _p.height);


                    _b1.x += Local_Root.TOPBAR_LEFT_MIN_BORDER_OFFSET;
                    _b1.width -= Local_Root.TOPBAR_LEFT_MIN_BORDER_OFFSET - Local_Root.TOPBAR_LEFT_MAX_BORDER_OFFSET;

                    gui_method_left(ref _b1);

                    int o2 = Mathf.RoundToInt((float)((_p.width + 40) / 2.0) + shrink);
                    o2 += 16;
                    var _b2 = new Rect(o2, 0, (_p.width - 850) / 2.2f - shrink, _p.height);
                    _b2.x += Local_Root.TOPBAR_RIGHT_MIN_BORDER_OFFSET;
                    _b2.width -= Local_Root.TOPBAR_RIGHT_MIN_BORDER_OFFSET - Local_Root.TOPBAR_RIGHT_MAX_BORDER_OFFSET;

                    gui_method_right(ref _b2);

                    /*
                    // GUI.Label(new Rect(0,0,20,20), "ASD");
                    float o1 = 416f;
                    var _p = (Rect)p3.GetValue(t[0], null);
                    float shrink = (_p.width - 1000) / 920 * 50;
                    int _bp = Mathf.RoundToInt((float)((_p.width - 140.0) / 2.0));
                    var _b1 = new Rect(o1, 0, _bp - o1 - shrink - 10, _p.height);
                    _b1.x += p.par_e.TOPBAR_LEFT_MIN_BORDER_OFFSET;
                    _b1.width -= p.par_e.TOPBAR_LEFT_MIN_BORDER_OFFSET - p.par_e.TOPBAR_LEFT_MAX_BORDER_OFFSET;

                    GUILayout.BeginArea(_b1);
                    GUILayout.BeginHorizontal();
                    if (p.par_e.TOPBAR_SWAP_LEFT_RIGHT) { if (p.par_e.DRAW_TOPBAR_LAYOUTS_BAR) layoutsMod.DrawLayers(); }
                    else
#if !EMX_H_LITE
                    if (p.par_e.DRAW_TOPBAR_HOTBUTTONS) hotButtons.DrawButtonsOnTopBar();
#endif
                    / *  if (GUILayout.Button("save" , GUILayout.ExpandHeight(true)))
					  {
						  chld = c.GetValue(m);
					  }
					  if (GUILayout.Button("load" , GUILayout.ExpandHeight(true)))
					  {
						  c.SetValue(m, chld);
						  Install(true);
						  InternalEditorUtility.RepaintAllViews();
					  }* /
                    if (p.par_e.DRAW_TOPBAR_CUSTOM_LEFT) if (EMX.CustomizationHierarchy.TopGUI.onLeftLayoutGUI != null)
                        {
                            _b1 = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                            _b1.y += MARGIN_Y;
                            _b1.height -= MARGIN_Y * 2;
                            EMX.CustomizationHierarchy.TopGUI.onLeftLayoutGUI(_b1);
                        }
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();


                    int o2 = Mathf.RoundToInt((float)((_p.width + 40) / 2.0) + shrink);
                    o2 += 16;
                    var _b2 = new Rect(o2, 0, (_p.width - 850) / 2.2f - shrink, _p.height);
                    _b2.x += p.par_e.TOPBAR_RIGHT_MIN_BORDER_OFFSET;
                    _b2.width -= p.par_e.TOPBAR_RIGHT_MIN_BORDER_OFFSET - p.par_e.TOPBAR_RIGHT_MAX_BORDER_OFFSET;

                    GUILayout.BeginArea(_b2);
                    GUILayout.BeginHorizontal();
                    if (p.par_e.TOPBAR_SWAP_LEFT_RIGHT)
                    {
#if !EMX_H_LITE
                        if (p.par_e.DRAW_TOPBAR_HOTBUTTONS) hotButtons.DrawButtonsOnTopBar();
#endif
                    }
                    else if (p.par_e.DRAW_TOPBAR_LAYOUTS_BAR) layoutsMod.DrawLayers();
                    if (p.par_e.DRAW_TOPBAR_CUSTOM_RIGHT) if (EMX.CustomizationHierarchy.TopGUI.onRightLayoutGUI != null)
                        {
                            _b1 = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                            _b1.y += MARGIN_Y;
                            _b1.height -= MARGIN_Y * 2;
                            EMX.CustomizationHierarchy.TopGUI.onRightLayoutGUI(_p);
                        }
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();
                    */
                };

                try
                {
                }
                catch { }
                // last = t[ 0 ] as ScriptableObject;
                inst = Activator.CreateInstance(ongui_type, customGui);

                if (!oldAssigned)
                {
                    oldAssigned = true;
                    oldContainer = get_r(t[0]);
                }
            }
            else
            {
                installed = false;
                reinstall = null;
                customGui = () => {
                    (OldOnGUI ?? (OldOnGUI = t[0].GetType().GetMethod("OldOnGUI", ~(BindingFlags.GetProperty | BindingFlags.Static)))).Invoke(t[0], null);
                };
                inst = Activator.CreateInstance(ongui_type, customGui);
                //inst = oldContainer;
            }



            if (get_r(t[0]) == null) set_r(t[0], inst);
            // OnDisable.Invoke( t[ 0 ], null );
#endif





            /*  var d = Delegate.CreateDelegate(typeof(Action), null, OnDisable);
               MethodInfo addHandler = disable.GetAddMethod();
   object[] addHandlerArgs = { d };
   addHandler.Invoke(t[ 0 ], addHandlerArgs);*/
            //  var old = mp.GetValue( t[ 0 ] );
            //  if ( old != null ) old.GetType().BaseType.GetMethods().First( q => q.Name == "Dispose" && q.GetParameters().Length == 0 ).Invoke( old, null );

            // pp.GetValue( t[ 0 ], null );

            var toolbar_integrated = SessionState.GetBool("EXM_TOOLBAR_BOOL", false);
            LayoutsMod.InitProperties();

            //bool needRegistrate = false;
            //Debug.Log((string)ll.GetValue(null, null));
            {

                //var old = mp.GetValue(t[0]);
                //if (old != null)
                {
                    //owner = old.GetType().BaseType.GetProperty("ownerObject", ~(BindingFlags.InvokeMethod | BindingFlags.Static)).GetValue(old, null) as ScriptableObject ?? throw new Exception("ownerObject");
                    //var utiputy = typeof(UnityEngine.UIElements.VisualElement).Assembly.GetType("UnityEngine.UIElements.UIElementsUtility") ?? throw new Exception("UIElementsUtility");
                    //var rmw = utiputy.GetMethod("RemoveCachedPanel", ~(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance)) ?? throw new Exception("RemoveCachedPanel");
                    //rmw.Invoke(null, new object[] { owner.GetInstanceID() });
                    //needRegistrate = true;
                }
                SessionState.SetBool("EXM_TOOLBAR_BOOL", true);
                SessionState.SetString("EXM_TOOLBAR_LASTPATH", (string)ll.GetValue(null, null));
            }


#if !U2020
            //mp.SetValue(t[0], null);
#endif

#if !U2021 //2021

            var cont = get_r(t[0]);
            var ac = (Action)ongui.GetValue(cont);
            ac = customGui;
            ongui.SetValue(cont, ac);

            //r.SetValue(t[0], inst, null);


            if (!toolbar_integrated || (string)ll.GetValue(null, null) != SessionState.GetString("EXM_TOOLBAR_LASTPATH", ""))
            {
                if (inst != null)
                {
                    //UnityEngine.UIElements.VisualElementExtensions.StretchToParentSize(inst as UnityEngine.UIElements.VisualElement);
                    //(r.PropertyType.GetField("useOwnerObjectGUIState", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("useOwnerObjectGUIState")).SetValue(inst, true);
                    //(r.PropertyType.BaseType.GetProperty("viewDataKey", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("viewDataKey")).SetValue(inst, "Dockarea", null);
                }
            }

#endif

            //var panel =
#if !U2020
            //pp.GetValue(t[0], null);
#endif

            //	if (needRegistrate)
            {
                /*	owner = panel.GetType().BaseType.GetProperty("ownerObject", ~(BindingFlags.InvokeMethod | BindingFlags.Static)).GetValue(panel, null) as ScriptableObject ?? throw new Exception("ownerObject");
					var utiputy = typeof(UnityEngine.UIElements.VisualElement).Assembly.GetType("UnityEngine.UIElements.UIElementsUtility") ?? throw new Exception("UIElementsUtility");
					var rmw = utiputy.GetMethod("RemoveCachedPanel", ~(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance)) ?? throw new Exception("RemoveCachedPanel");
					rmw.Invoke(null, new object[] { owner.GetInstanceID() });
					var add = utiputy.GetMethod("RegisterCachedPanel", ~(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance)) ?? throw new Exception("RegisterCachedPanel");
					add.Invoke(null, new object[] { owner.GetInstanceID(), panel });*/
            }

            if (remove)
            {
                lastMain = null;
                SessionState.GetBool("EXM_TOOLBAR_BOOL", false);
            }
            /*   if ( mp.GetValue( t[ 0 ] ) != null )
               {
                   Debug.Log( "ASD" );
                   var a = mp.GetValue( t[ 0 ] );
                   var re = a.GetType().GetProperty("visualTree",  ~(BindingFlags.InvokeMethod | BindingFlags.Static)).GetValue(a);
                   re.GetType().GetMethod( "Insert", ~(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Static) ).Invoke( re, new object[] { 0, inst } );
               }*/
            /*
            var pd = v.GetMethod("UpdateDrawChainRegistration",  ~(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Static)  );

            pd.Invoke( t[ 0 ], new object[] { true } );

            var ev = typeof( UnityEngine.UIElements.VisualElement ).Assembly.GetType( "UnityEngine.UIElements.Panel" );
            var bef = ev.GetField("BeforeUpdaterChange",  ~(BindingFlags.InvokeMethod | BindingFlags.Instance) );
            var aft = ev.GetField("AfterUpdaterChange",  ~(BindingFlags.InvokeMethod | BindingFlags.Instance) );
            bef.SetValue( null, ((Action)bef.GetValue( null )) + (( ) => { pd.Invoke( t[ 0 ], new object[] { false } ); }) );
            bef.SetValue( null,  (Action)(( ) => { Debug.Log("QWE"); }) );
            aft.SetValue( null, ((Action)aft.GetValue( null )) + (( ) => { pd.Invoke( t[ 0 ], new object[] { true } ); }) );*/
            // OnEnable.Invoke( t[ 0 ], null );
            //r.SetValue( t[ 0 ], inst, null );
        }

        ScriptableObject owner;










        void gui_method_left(ref Rect _b1)
        {

            // if (Local_Root.LayoutsButtonsHelper._oneShotGui.Count != 0)
            // {
            //     var ex = Local_Root.LayoutsButtonsHelper._oneShotGui.ToArray();
            //     Local_Root.LayoutsButtonsHelper._oneShotGui.Clear();
            //     foreach (var item in ex) item();
            // }

            // if ( EditorGUIUtility.hotControl != 0 ) Debug.Log( EditorGUIUtility.hotControl );


            // GUI.Label(new Rect(0,0,20,20), "ASD");



            GUILayout.BeginArea(_b1);
            if (Local_Root.TOPBAR_LAYOUTS_MIN_Y_OFFSET != 0) GUILayout.Space(Local_Root.TOPBAR_LAYOUTS_MIN_Y_OFFSET);
            GUILayout.BeginHorizontal();


            draw_swapped(_b1, false);


            GUILayout.EndHorizontal();
            if (Local_Root.TOPBAR_LAYOUTS_MAX_Y_OFFSET != 0) GUILayout.Space(Local_Root.TOPBAR_LAYOUTS_MAX_Y_OFFSET);
            GUILayout.EndArea();


        }


        float default_height {
            get {
                return 30 - Local_Root.TOPBAR_LAYOUTS_MAX_Y_OFFSET - Local_Root.TOPBAR_LAYOUTS_MIN_Y_OFFSET;
            }
        }


        void draw_by_index(int index)
        {





            switch (index)
            {
                case 0:
                    if (Local_Root.DRAW_TOPBAR_HOTBUTTONS)
                    {
#if !EMX_H_LITE
                        hotButtons.DrawButtonsOnTopBar();
#endif
                    }
                    break;
                case 1:
                    if (Local_Root.DRAW_TOPBAR_CUSTOM_LEFT)
                        if (EMX.CustomizationHierarchy.TopGUI.onLeftLayoutGUI != null)
                        {
                            var _b1 = EditorGUILayout.GetControlRect(GUILayout.Width(0), GUILayout.ExpandHeight(true)); //GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)
                            if (_b1.height < 3) _b1.height = old_height ?? default_height;
                            else old_height = _b1.height;
                            //_b1.y += MARGIN_Y;
                            //_b1.height -= MARGIN_Y * 2;
                            EMX.CustomizationHierarchy.TopGUI.onLeftLayoutGUI(_b1); //_p was
                        }
                    break;
                case 2:

                    if (Local_Root.DRAW_TOPBAR_CUSTOM_RIGHT)
                        if (EMX.CustomizationHierarchy.TopGUI.onRightLayoutGUI != null)
                        {
                            // _b1 = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                            var _b1 = EditorGUILayout.GetControlRect(GUILayout.Width(0), GUILayout.ExpandHeight(true));
                            if (_b1.height < 3) _b1.height = old_height ?? default_height;
                            else old_height = _b1.height;
                            //_b1.y += MARGIN_Y;
                            //_b1.height -= MARGIN_Y * 2;
                            EMX.CustomizationHierarchy.TopGUI.onRightLayoutGUI(_b1); //_p was
                        }

                    break;
                case 3:
                    if (Local_Root.DRAW_TOPBAR_LAYOUTS_BAR) layoutsMod.DrawLayers();
                    break;
            }
        }

        float? old_height;

        void draw_swapped(Rect _b1, bool swap)
        {

            if (!swap)
                for (int i = 0; i < 2; i++)
                    for (int x = 0; x < Local_Root.topbar_but.Length; x++)
                        if (Local_Root.topbar_but[x].TOPBAR_BUT_INDEX == i)
                            draw_by_index(Local_Root.topbar_but[x].key);

            if (swap)
                for (int i = 2; i < 4; i++)
                    for (int x = 0; x < Local_Root.topbar_but.Length; x++)
                        if (Local_Root.topbar_but[x].TOPBAR_BUT_INDEX == i)
                            draw_by_index(Local_Root.topbar_but[x].key);

            // if (swap)
            // {
            //
            //
            //  
            //
            //
            // }
            // else
            // {
            //
            //
            //     if (Local_Root.DRAW_TOPBAR_CUSTOM_LEFT)
            //         if (EM.LayoutsButtonsCustomization.TopGUI.onLeftLayoutGUI != null)
            //         {
            //             _b1 = EditorGUILayout.GetControlRect(GUILayout.Width(0), GUILayout.ExpandHeight(true)); //GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)
            //             _b1.y += MARGIN_Y;
            //             _b1.height -= MARGIN_Y * 2;
            //             EM.LayoutsButtonsCustomization.TopGUI.onLeftLayoutGUI(_b1); //_p was
            //         }
            // }
        }

        void gui_method_right(ref Rect _b2)
        {

            // if (Local_Root.LayoutsButtonsHelper._oneShotGui.Count != 0)
            // {
            //     var ex = Local_Root.LayoutsButtonsHelper._oneShotGui.ToArray();
            //     Local_Root.LayoutsButtonsHelper._oneShotGui.Clear();
            //     foreach (var item in ex) item();
            // }

            // if (Local_Root.TOPBAR_SWAP_LEFT_RIGHT != oldSwap)
            // {
            //     oldSwap = Local_Root.TOPBAR_SWAP_LEFT_RIGHT;
            //     layoutsMod.ClearTimers();
            // }


            GUILayout.BeginArea(_b2);
            if (Local_Root.TOPBAR_LAYOUTS_MIN_Y_OFFSET != 0) GUILayout.Space(Local_Root.TOPBAR_LAYOUTS_MIN_Y_OFFSET);
            GUILayout.BeginHorizontal();


            draw_swapped(_b2, true);
            //  if (Local_Root.TOPBAR_SWAP_LEFT_RIGHT)
            //  {
            //      GUILayout.FlexibleSpace();
            //      if (Local_Root.DRAW_TOPBAR_CUSTOM_RIGHT)
            //          if (EM.LayoutsButtonsCustomization.TopGUI.onRightLayoutGUI != null)
            //          {
            //              _b2 = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            //              _b2.y += MARGIN_Y;
            //              _b2.height -= MARGIN_Y * 2;
            //              EM.LayoutsButtonsCustomization.TopGUI.onRightLayoutGUI(_b2); //_p was
            //          }
            //      if (Local_Root.DRAW_TOPBAR_HOTBUTTONS)
            //      {
            //          //hotButtons.DrawButtonsOnTopBar();
            //      }
            //  }
            //  else
            //  {
            //
            //      if (Local_Root.DRAW_TOPBAR_LAYOUTS_BAR) layoutsMod.DrawLayers();
            //
            //      if (Local_Root.DRAW_TOPBAR_CUSTOM_RIGHT)
            //          if (EM.LayoutsButtonsCustomization.TopGUI.onRightLayoutGUI != null)
            //          {
            //              _b2 = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            //              _b2.y += MARGIN_Y;
            //              _b2.height -= MARGIN_Y * 2;
            //              EM.LayoutsButtonsCustomization.TopGUI.onRightLayoutGUI(_b2); //_p was
            //          }
            //  }


            GUILayout.EndHorizontal();
            if (Local_Root.TOPBAR_LAYOUTS_MAX_Y_OFFSET != 0) GUILayout.Space(Local_Root.TOPBAR_LAYOUTS_MAX_Y_OFFSET);
            GUILayout.EndArea();
        }


        /*    public static void SaveGUI()
    {
      UnityEditor.SaveWindowLayout.Show(WindowLayout.FindMainView().screenPosition);
    }

    public static void DeleteGUI()
    {
      DeleteWindowLayout.Show(WindowLayout.FindMainView().screenPosition);
    }
*/
    }
}
