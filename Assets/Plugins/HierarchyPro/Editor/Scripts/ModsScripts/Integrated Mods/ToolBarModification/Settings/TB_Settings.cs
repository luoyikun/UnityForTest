using System;
using UnityEngine;
using EMX.HierarchyPlugin.Editor.Settings;

namespace EMX.HierarchyPlugin.Editor
{

    partial class EditorSettingsAdapter
    {

        internal string[] DRAW_TOPBAR_NAMES = {
        "HyperGraph",
                       "Project Folders",
                     "Hierarchy Bookmarks",
                      "Hierarchy Scenes",
                    "Hierarchy Last Selection",
                   "Hierarchy Expanded Objects" };

        internal bool DRAW_TOPBAR_H1 { get { return GET("DRAW_TOPBAR_H1", true); } set { var r = DRAW_TOPBAR_H1; SET("DRAW_TOPBAR_H1", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal bool DRAW_TOPBAR_H2 { get { return GET("DRAW_TOPBAR_H2", true); } set { var r = DRAW_TOPBAR_H2; SET("DRAW_TOPBAR_H2", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal bool DRAW_TOPBAR_H3 { get { return GET("DRAW_TOPBAR_H3", true); } set { var r = DRAW_TOPBAR_H3; SET("DRAW_TOPBAR_H3", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal bool DRAW_TOPBAR_H4 { get { return GET("DRAW_TOPBAR_H4", true); } set { var r = DRAW_TOPBAR_H4; SET("DRAW_TOPBAR_H4", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal bool DRAW_TOPBAR_H5 { get { return GET("DRAW_TOPBAR_H5", true); } set { var r = DRAW_TOPBAR_H5; SET("DRAW_TOPBAR_H5", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal bool DRAW_TOPBAR_H6 { get { return GET("DRAW_TOPBAR_H6", true); } set { var r = DRAW_TOPBAR_H6; SET("DRAW_TOPBAR_H6", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }

        internal int ORDER_TOPBAR_H1 { get { return GET("ORDER_TOPBAR_H1", 0); } set { var r = ORDER_TOPBAR_H1; SET("ORDER_TOPBAR_H1", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal int ORDER_TOPBAR_H2 { get { return GET("ORDER_TOPBAR_H2", 5); } set { var r = ORDER_TOPBAR_H2; SET("ORDER_TOPBAR_H2", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal int ORDER_TOPBAR_H3 { get { return GET("ORDER_TOPBAR_H3", 1); } set { var r = ORDER_TOPBAR_H3; SET("ORDER_TOPBAR_H3", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal int ORDER_TOPBAR_H4 { get { return GET("ORDER_TOPBAR_H4", 3); } set { var r = ORDER_TOPBAR_H4; SET("ORDER_TOPBAR_H4", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal int ORDER_TOPBAR_H5 { get { return GET("ORDER_TOPBAR_H5", 2); } set { var r = ORDER_TOPBAR_H5; SET("ORDER_TOPBAR_H5", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal int ORDER_TOPBAR_H6 { get { return GET("ORDER_TOPBAR_H6", 4); } set { var r = ORDER_TOPBAR_H6; SET("ORDER_TOPBAR_H6", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }

        const int L6 = 6;
        int[] order = new int[0];
        int[] GET_ORDERED_HOTBUTTONS {
            get {
                if (order == null || order.Length != L6)
                {
                    if (order == null) order = new int[0];
                    Array.Resize(ref order, L6);
                    for (int i = 0; i < L6; i++)
                    {
                        var ind_setter = Draw.GetSetter("ORDER_TOPBAR_H" + (i + 1));
                        var cur = (int)ind_setter.value;
                        order[cur] = i;
                    }
                }
                return order;
            }
        }

        static GUIContent SortingUP = new GUIContent() { tooltip = "Sort Order", text = "▲" };
        static GUIContent SortingDOWN = new GUIContent() { tooltip = "Sort Order", text = "▼" };


        GUIStyle _but; GUIStyle but { get { return _but ?? (_but = new GUIStyle(GUI.skin.button) { fontSize = Math.Max(2, GUI.skin.button.fontSize - 6) }); } }


        internal void DrawHotButtonsArray()
        {

            var e = GUI.enabled;
            var L6 = 6;
            var or = GET_ORDERED_HOTBUTTONS;
            for (int _i = 0; _i < L6; _i++)
            {

                var __index = or[_i];

                var r = Draw.R;
                var MOVE = 19;
                r.width -= MOVE * 2 + 10;

                GUI.enabled = e;
                var val = Draw.GetSetter("DRAW_TOPBAR_H" + (__index + 1)).AS_BOOL;
                //var ind_setter = Draw.GetSetter( "ORDER_TOPBAR_H" + (__index + 1) );

                Draw.TOG(DRAW_TOPBAR_NAMES[__index], "DRAW_TOPBAR_H" + (__index + 1), rov: r);
                r.x += r.width;
                r.width = MOVE;

                //var __index = (int)ind_setter.value;

                GUI.enabled = val & e & _i != 0;

                if (GUI.Button(r, SortingUP, but))
                {
                    var CI = or[_i];
                    var NI = or[_i - 1];
                    var ind_setter1 = Draw.GetSetter("ORDER_TOPBAR_H" + (CI + 1));
                    var ind_setter2 = Draw.GetSetter("ORDER_TOPBAR_H" + (NI + 1));
                    var OLD = (int)ind_setter1.value;
                    ind_setter1.value = (int)ind_setter2.value;
                    ind_setter2.value = OLD;
                    order = null;
                    p.modsController.REBUILD_PLUGINS();
                }

                //r.x = SOURCE_RECT.x + OBJECT_X( 2 );
                //r.width = OBJECT_WIDTH[ 2 ];
                r.x += r.width;

                GUI.enabled = val & e & _i < L6 - 1;

                if (GUI.Button(r, SortingDOWN, but))
                {
                    //var CI = DRAW_INDEX[__index];
                    //var NI = DRAW_INDEX[__index + 1];
                    //var OLD = RowsClasses[CI].IndexPos;
                    //RowsClasses[ CI ].IndexPos = RowsClasses[ NI ].IndexPos;
                    //RowsClasses[ NI ].IndexPos = OLD;
                    var CI = or[_i];
                    var NI = or[_i + 1];
                    var ind_setter1 = Draw.GetSetter("ORDER_TOPBAR_H" + (CI + 1));
                    var ind_setter2 = Draw.GetSetter("ORDER_TOPBAR_H" + (NI + 1));
                    var OLD = (int)ind_setter1.value;
                    ind_setter1.value = (int)ind_setter2.value;
                    ind_setter2.value = OLD;
                    order = null;
                    p.modsController.REBUILD_PLUGINS();
                }
            }

            GUI.enabled = e;

        }





        //BUTTONS
        top_Bar_but_settings[] _top_bar_cache;
        public top_Bar_but_settings[] topbar_but {
            get {
                if (_top_bar_cache == null)
                {
                    _top_bar_cache = new top_Bar_but_settings[4]{
                        new top_Bar_but_settings(this) { key = 0 },
            new top_Bar_but_settings(this) { key = 1 },
            new top_Bar_but_settings(this) { key = 2 },
            new top_Bar_but_settings(this) { key = 3 } };
                }
                return _top_bar_cache;
            }
        }



        public class top_Bar_but_settings
        {
            public top_Bar_but_settings(EditorSettingsAdapter a) { this.a = a; }
            internal EditorSettingsAdapter a;
            internal int key;
            public bool TOPBAR_BUT_APPLY_Y {
                get { return a.GET("TOPBAR_BUT_APPLY_Y_" + key, true); }
                set { var r = TOPBAR_BUT_APPLY_Y; a.SET("TOPBAR_BUT_APPLY_Y_" + key, value); a.p.RepaintAllViews(); }
            }
            public int TOPBAR_BUT_INDEX {
                get { return a.GET("TOPBAR_BUT_INDEX_" + key, key); }
                set { var r = TOPBAR_BUT_INDEX; a.SET("TOPBAR_BUT_INDEX_" + key, value); a.p.RepaintAllViews(); }
            }
            public bool ENABLED {
                get {
                    switch (key)
                    {
                        case 0: return a.DRAW_TOPBAR_HOTBUTTONS;
                        case 1: return a.DRAW_TOPBAR_CUSTOM_LEFT;
                        case 2: return a.DRAW_TOPBAR_CUSTOM_RIGHT;
                        case 3: return a.DRAW_TOPBAR_LAYOUTS_BAR;
                    }
                    throw new IndexOutOfRangeException();
                }
            }
            public string NAME {
                get {
                    return cats[key];
                }
            }
            static string[] cats = {

               "Hot Buttons"
                // "- Empty Slot -"

					, "- Custom Left -", "- Custom Right -", "- LAYOUTS -" };
        }




        //CUSTOM
        internal bool DRAW_TOPBAR_CUSTOM_LEFT { get { return GET("DRAW_TOPBAR_CUSTOM_LEFT", true); } set { var r = DRAW_TOPBAR_CUSTOM_LEFT; SET("DRAW_TOPBAR_CUSTOM_LEFT", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal bool DRAW_TOPBAR_CUSTOM_RIGHT { get { return GET("DRAW_TOPBAR_CUSTOM_RIGHT", false); } set { var r = DRAW_TOPBAR_CUSTOM_RIGHT; SET("DRAW_TOPBAR_CUSTOM_RIGHT", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }

        //LAYOUTS
        internal bool DRAW_TOPBAR_LAYOUTS_BAR { get { return GET("DRAW_TOPBAR_LAYOUTS_BAR", true); } set { var r = DRAW_TOPBAR_LAYOUTS_BAR; SET("DRAW_TOPBAR_LAYOUTS_BAR", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal bool TOPBAR_LAYOUTS_DRAWCROSS { get { return GET("TOPBAR_LAYOUTS_DRAWCROSS", true); } set { var r = TOPBAR_LAYOUTS_DRAWCROSS; SET("TOPBAR_LAYOUTS_DRAWCROSS", value); p.RepaintAllViews(); } }
        internal bool TOPBAR_LAYOUTS_AUTOSAVE { get { return GET("TOPBAR_LAYOUTS_AUTOSAVE", false); } set { var r = TOPBAR_LAYOUTS_AUTOSAVE; SET("TOPBAR_LAYOUTS_AUTOSAVE", value); p.RepaintAllViews(); } }
        internal bool TOPBAR_LAYOUTS_SAVE_ONLY_CUSTOM { get { return GET("TOPBAR_LAYOUTS_SAVE_ONLY_CUSTOM", true); } set { var r = TOPBAR_LAYOUTS_SAVE_ONLY_CUSTOM; SET("TOPBAR_LAYOUTS_SAVE_ONLY_CUSTOM", value); p.RepaintAllViews(); } }
        //internal int TOPBAR_LAYOUTS_MIN_Y_OFFSET { get { return GET( "TOPBAR_LAYOUTS_MIN_Y_OFFSET", 0 ); } set { var r = TOPBAR_LAYOUTS_MIN_Y_OFFSET; SET( "TOPBAR_LAYOUTS_MIN_Y_OFFSET", value ); p.RepaintAllViews(); } }
        //internal int TOPBAR_LAYOUTS_MAN_Y_OFFSET { get { return GET( "TOPBAR_LAYOUTS_MAN_Y_OFFSET", 0 ); } set { var r = TOPBAR_LAYOUTS_MAN_Y_OFFSET; SET( "TOPBAR_LAYOUTS_MAN_Y_OFFSET", value ); p.RepaintAllViews(); } }
        internal bool TOPBAR_LAYOUTS_EXPAND_HEIGHT { get { return GET("TOPBAR_LAYOUTS_EXPAND_HEIGHT", false); } set { var r = TOPBAR_LAYOUTS_EXPAND_HEIGHT; SET("TOPBAR_LAYOUTS_EXPAND_HEIGHT", value); p.RepaintAllViews(); } }
        internal bool TOPBAR_LAYOUTS_USE_FIXED_WIDTH { get { return GET("TOPBAR_LAYOUTS_USE_FIXED_WIDTH", false); } set { var r = TOPBAR_LAYOUTS_USE_FIXED_WIDTH; SET("TOPBAR_LAYOUTS_USE_FIXED_WIDTH", value); p.RepaintAllViews(); } }
        internal int TOPBAR_LAYOUTS_FIXED_WIDTH_AMOUNT { get { return GET("TOPBAR_LAYOUTS_FIXED_WIDTH_AMOUNT", 60); } set { var r = TOPBAR_LAYOUTS_FIXED_WIDTH_AMOUNT; SET("TOPBAR_LAYOUTS_FIXED_WIDTH_AMOUNT", value); p.RepaintAllViews(); } }


        //HOT BUTTONS
        internal bool DRAW_TOPBAR_HOTBUTTONS { get { return GET("DRAW_TOPBAR_HOTBUTTONS", true); } set { var r = DRAW_TOPBAR_HOTBUTTONS; SET("DRAW_TOPBAR_HOTBUTTONS", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal int TOPBAR_HOTBUTTON_SIZE { get { return GET("TOP_EXTBUTTON_SIZE", 20); } set { var r = TOPBAR_HOTBUTTON_SIZE; SET("TOP_EXTBUTTON_SIZE", value); p.RepaintAllViews(); } }
        internal bool DRAW_HEADER_HOTBUTTONS { get { return GET("DRAW_HEADER_HOTBUTTONS", false); } set { var r = DRAW_HEADER_HOTBUTTONS; SET("DRAW_HEADER_HOTBUTTONS", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal bool DRAW_BOTTOM_HOTBUTTONS { get { return GET("DRAW_BOTTOM_HOTBUTTONS", false); } set { var r = DRAW_BOTTOM_HOTBUTTONS; SET("DRAW_BOTTOM_HOTBUTTONS", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        internal int HEADER_HOTBUTTON_SEZE { get { return GET("HEADER_EXTBUTTON_SEZE", 12); } set { var r = HEADER_HOTBUTTON_SEZE; SET("HEADER_EXTBUTTON_SEZE", value); p.RepaintAllViews(); } }
        internal int BOTTOM_HOTBUTTON_SEZE { get { return GET("BOTTOM_HOTBUTTON_SEZE", 12); } set { var r = BOTTOM_HOTBUTTON_SEZE; SET("BOTTOM_HOTBUTTON_SEZE", value); p.RepaintAllViews(); } }


        internal int TOPBAR_LAYOUTS_MIN_Y_OFFSET { get { return GET("TOPBAR_LAYOUTS_MIN_Y_OFFSET", 5); } set { var r = TOPBAR_LAYOUTS_MIN_Y_OFFSET; SET("TOPBAR_LAYOUTS_MIN_Y_OFFSET", value); p.RepaintAllViews(); } }
        internal int TOPBAR_LAYOUTS_MAX_Y_OFFSET { get { return GET("TOPBAR_LAYOUTS_MAX_Y_OFFSET", 5); } set { var r = TOPBAR_LAYOUTS_MAX_Y_OFFSET; SET("TOPBAR_LAYOUTS_MAX_Y_OFFSET", value); p.RepaintAllViews(); } }


        internal int TOPBAR_LEFT_MIN_BORDER_OFFSET { get { return GET("LEFT_MIN_BORDER_OFFSET_UNITY2021", 0); } set { var r = TOPBAR_LEFT_MIN_BORDER_OFFSET; SET("LEFT_MIN_BORDER_OFFSET_UNITY2021", value); p.RepaintAllViews(); } }
        internal int TOPBAR_LEFT_MAX_BORDER_OFFSET { get { return GET("LEFT_MAX_BORDER_OFFSET_UNITY2021", 0); } set { var r = TOPBAR_LEFT_MAX_BORDER_OFFSET; SET("LEFT_MAX_BORDER_OFFSET_UNITY2021", value); p.RepaintAllViews(); } }
        internal int TOPBAR_RIGHT_MIN_BORDER_OFFSET { get { return GET("RIGHT_MIN_BORDER_OFFSET_UNITY2021", 0); } set { var r = TOPBAR_RIGHT_MIN_BORDER_OFFSET; SET("RIGHT_MIN_BORDER_OFFSET_UNITY2021", value); p.RepaintAllViews(); } }
        internal int TOPBAR_RIGHT_MAX_BORDER_OFFSET {
            get {
                return GET("RIGHT_MAX_BORDER_OFFSET_UNITY2021",
#if UNITY_2021_1_OR_NEWER
            0
#elif UNITY_2020_1_OR_NEWER
           - 130
#else
            0
#endif
            );
            }
            set { var r = TOPBAR_RIGHT_MAX_BORDER_OFFSET; SET("RIGHT_MAX_BORDER_OFFSET_UNITY2021", value); p.RepaintAllViews(); }
        }


        //		internal bool TOPBAR_SWAP_LEFT_RIGHT { get { return GET( "TOPBAR_SWAP_LEFT_RIGHT", false ); } set { var r = TOPBAR_SWAP_LEFT_RIGHT; SET( "TOPBAR_SWAP_LEFT_RIGHT", value ); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
        //		internal int TOPBAR_LEFT_MIN_BORDER_OFFSET { get { return GET( "LEFT_MIN_BORDER_OFFSET_UNITY2021", 0 ); } set { var r = TOPBAR_LEFT_MIN_BORDER_OFFSET; SET( "LEFT_MIN_BORDER_OFFSET_UNITY2021", value ); p.RepaintAllViews(); } }
        //		internal int TOPBAR_LEFT_MAX_BORDER_OFFSET { get { return GET( "LEFT_MAX_BORDER_OFFSET_UNITY2021", 0 ); } set { var r = TOPBAR_LEFT_MAX_BORDER_OFFSET; SET( "LEFT_MAX_BORDER_OFFSET_UNITY2021", value ); p.RepaintAllViews(); } }
        //		internal int TOPBAR_RIGHT_MIN_BORDER_OFFSET { get { return GET( "RIGHT_MIN_BORDER_OFFSET_UNITY2021", 0 ); } set { var r = TOPBAR_RIGHT_MIN_BORDER_OFFSET; SET( "RIGHT_MIN_BORDER_OFFSET_UNITY2021", value ); p.RepaintAllViews(); } }
        //		internal int TOPBAR_RIGHT_MAX_BORDER_OFFSET {
        //			get {
        //				return GET( "RIGHT_MAX_BORDER_OFFSET_UNITY2021",
        //#if UNITY_2021_1_OR_NEWER
        //			0
        //#elif UNITY_2020_1_OR_NEWER
        //           - 130
        //#else
        //			0
        //#endif
        //			);
        //			}
        //			set { var r = TOPBAR_RIGHT_MAX_BORDER_OFFSET; SET( "RIGHT_MAX_BORDER_OFFSET_UNITY2021", value ); p.RepaintAllViews(); }
        //		}

    }
}
