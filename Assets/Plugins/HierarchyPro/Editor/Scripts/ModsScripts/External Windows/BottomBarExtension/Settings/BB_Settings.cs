namespace EMX.HierarchyPlugin.Editor
{

	partial class EditorSettingsAdapter
    {

        //internal bool BOTTOM_DRAWFOR_ONEHIERARHYWIN = true;
        ////internal int BOTTOM_LINE_HEIGHT = 20;
        //internal bool SHOW_SCENES_ROWS = true;
        //internal bool SHOW_HIERARCHYSLOTS_ROWS = true;
        //internal bool SHOW_LAST_ROWS = true;
        //internal bool SHOW_BOOKMARKS_ROWS = true;
        //internal int BOOKMARKS_LINEHEIGHT = 22;
        //internal int LAST_LINEHEIGHT = 16;
        //internal int HIER_LINEHEIGHT = 16;
        //internal int SCENES_LINEHEIGHT = 16;
        //internal bool SHOW_PARENT_TREE = false;
        //internal bool SHOW_PARENT_TREE_CURRENTOBJECT = false;

        internal partial class WindowSettings
        {
            internal bool BOTTOMBAR_DRAW_HOT_BUTTON { get { return s.GET( "BOTTOMBAR_DRAW_HOT_BUTTON", true ); } set { var r = BOTTOMBAR_DRAW_HOT_BUTTON; s.SET( "BOTTOMBAR_DRAW_HOT_BUTTON", value ); } }
			internal bool BOTTOMBAR_QUICK_BUTTON_LEFT_CLICK_CLOSE { get { return s.GET( "BOTTOMBAR_QUICK_BUTTON_LEFT_CLICK_CLOSE", false ); } set { var r = BOTTOMBAR_QUICK_BUTTON_LEFT_CLICK_CLOSE; s.SET( "BOTTOMBAR_QUICK_BUTTON_LEFT_CLICK_CLOSE", value ); } }

            internal bool BOTTOMBAR_BOTTOM_DRAWFOR_ONEHIERARHYWIN { get { return s.GET( "BOTTOMBAR_BOTTOM_DRAWFOR_ONEHIERARHYWIN", true ); } set { var r = BOTTOMBAR_BOTTOM_DRAWFOR_ONEHIERARHYWIN; s.SET( "BOTTOMBAR_BOTTOM_DRAWFOR_ONEHIERARHYWIN", value ); } }

			internal int BOTTOM_ICONS_SIZE_OFFSET {
				get { return s.GET( KEY + "_BOTTOM_ICONS_SIZE_OFFSET", 0 ); }
				set {
					var r = BOTTOM_ICONS_SIZE_OFFSET;
					if ( s.SET( KEY + "_BOTTOM_ICONS_SIZE_OFFSET", value ) ) p.RepaintWindowInUpdate_PlusResetStack( pluginID );
				}
			}
			//internal bool BOTTOMBAR_SHOW_SCENES_ROWS { get { return s.GET( "BOTTOMBAR_SHOW_SCENES_ROWS", true ); } set { var r = BOTTOMBAR_SHOW_SCENES_ROWS; s.SET( "BOTTOMBAR_SHOW_SCENES_ROWS", value ); p.modsController.REBUILD_PLUGINS_FAST(); } }
			//internal bool BOTTOMBAR_SHOW_HIERARCHYSLOTS_ROWS { get { return s.GET( "BOTTOMBAR_SHOW_HIERARCHYSLOTS_ROWS", true ); } set { var r = BOTTOMBAR_SHOW_HIERARCHYSLOTS_ROWS; s.SET( "BOTTOMBAR_SHOW_HIERARCHYSLOTS_ROWS", value ); p.modsController.REBUILD_PLUGINS_FAST();} }
			//internal bool BOTTOMBAR_SHOW_LAST_ROWS { get { return s.GET( "BOTTOMBAR_SHOW_LAST_ROWS", true ); } set { var r = BOTTOMBAR_SHOW_LAST_ROWS; s.SET( "BOTTOMBAR_SHOW_LAST_ROWS", value ); p.modsController.REBUILD_PLUGINS_FAST();} }
			//internal bool BOTTOMBAR_SHOW_BOOKMARKS_ROWS { get { return s.GET( "BOTTOMBAR_SHOW_BOOKMARKS_ROWS", true ); } set { var r = BOTTOMBAR_SHOW_BOOKMARKS_ROWS; s.SET( "BOTTOMBAR_SHOW_BOOKMARKS_ROWS", value ); p.modsController.REBUILD_PLUGINS_FAST();} }

			internal int BOTTOMBAR_HEADER_OPTION { get { return s.GET( "BOTTOMBAR_HEADER_OPTION", 1 ); } set { var r = BOTTOMBAR_HEADER_OPTION; s.SET( "BOTTOMBAR_HEADER_OPTION", value ); } }


			internal int BOTTOMBAR_HEADER_FONT_SIZE { get { return s.GET( "BOTTOMBAR_HEADER_FONT_SIZE", 8 ); } set { var r = BOTTOMBAR_HEADER_FONT_SIZE; s.SET( "BOTTOMBAR_HEADER_FONT_SIZE", value ); } }
			internal int BOTTOMBAR_ICONS_SPACE { get { return s.GET( "BOTTOMBAR_ICONS_SPACE", 4 ); } set { var r = BOTTOMBAR_ICONS_SPACE; s.SET( "BOTTOMBAR_ICONS_SPACE", value ); } }

			//internal int BOTTOMBAR_BOOKMARKS_LINEHEIGHT { get { return s.GET( "BOTTOMBAR_BOOKMARKS_LINEHEIGHT", 22 ); } set { var r = BOTTOMBAR_BOOKMARKS_LINEHEIGHT; s.SET( "BOTTOMBAR_BOOKMARKS_LINEHEIGHT", value ); } }
			//internal int BOTTOMBAR_LAST_LINEHEIGHT { get { return s.GET( "BOTTOMBAR_LAST_LINEHEIGHT", 16 ); } set { var r = BOTTOMBAR_LAST_LINEHEIGHT; s.SET( "BOTTOMBAR_LAST_LINEHEIGHT", value ); } }
			//internal int BOTTOMBAR_HIER_LINEHEIGHT { get { return s.GET( "BOTTOMBAR_HIER_LINEHEIGHT", 16 ); } set { var r = BOTTOMBAR_HIER_LINEHEIGHT; s.SET( "BOTTOMBAR_HIER_LINEHEIGHT", value ); } }
			//internal int BOTTOMBAR_SCENES_LINEHEIGHT { get { return s.GET( "BOTTOMBAR_SCENES_LINEHEIGHT", 16 ); } set { var r = BOTTOMBAR_SCENES_LINEHEIGHT; s.SET( "BOTTOMBAR_SCENES_LINEHEIGHT", value ); } }

			internal bool BOTTOMBAR_USE_DOUBLE_CLICK { get { return s.GET( "BOTTOMBAR_USE_DOUBLE_CLICK", false ); } set { var r = BOTTOMBAR_USE_DOUBLE_CLICK; s.SET( "BOTTOMBAR_USE_DOUBLE_CLICK", value ); } }
			internal bool BOTTOMBAR_SHOW_CAT_NAME {
				get {
					return BOTTOMBAR_HEADER_OPTION == 1;
					//return s.GET( "BOTTOMBAR_SHOW_PARENT_TREE", false ); 
					//} set { var r = BOTTOMBAR_SHOW_PARENT_TREE; s.SET( "BOTTOMBAR_SHOW_PARENT_TREE", value ); }
				}
			}

			internal bool BOTTOMBAR_SHOW_PARENT_TREE {
                get {
                    return BOTTOMBAR_HEADER_OPTION == 2;
                    //return s.GET( "BOTTOMBAR_SHOW_PARENT_TREE", false ); 
                    //} set { var r = BOTTOMBAR_SHOW_PARENT_TREE; s.SET( "BOTTOMBAR_SHOW_PARENT_TREE", value ); }
                }
            }
            internal bool BOTTOMBAR_SHOW_PARENT_TREE_CURRENTOBJECT { get { return s.GET( "BOTTOMBAR_SHOW_PARENT_TREE_CURRENTOBJECT", false ); } set { var r = BOTTOMBAR_SHOW_PARENT_TREE_CURRENTOBJECT; s.SET( "BOTTOMBAR_SHOW_PARENT_TREE_CURRENTOBJECT", value ); } }

        }
    }
}
