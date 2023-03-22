using UnityEngine;



namespace EMX.HierarchyPlugin.Editor
{

	partial class EditorSettingsAdapter
    {

        
      //  internal bool DISPLAY_FILES_EXTENSION { get { return GET( "DISPLAY_FILES_EXTENSION", true ); } set {var r = qwe; SET( "DISPLAY_FILES_EXTENSION", value ); p.modsController.REBUILD_PLUGINS(); } }
        internal bool DRAW_EXTENSION_IN_PROJECT {
            get { return USE_PROJECT_GUI_EXTENSIONS; }
            set { USE_PROJECT_GUI_EXTENSIONS = value; }
           // get { return GET("DRAW_EXTENSION_IN_PROJECT", true); } set {var r = qwe; SET( "DRAW_EXTENSION_IN_PROJECT", value); p.modsController.REBUILD_PLUGINS(); } 
        }

        internal Color DRAW_EXTENSION_COLOR { get { return GET("DRAW_EXTENSION_COLOR", Color.gray); } set {var r = DRAW_EXTENSION_COLOR; SET( "DRAW_EXTENSION_COLOR", value); p.RepaintAllViews(); } }
        internal int DRAW_EXTENSION_FONT_SIZE { get { return GET("DRAW_EXTENSION_FONT_SIZE", 7); } set {var r = DRAW_EXTENSION_FONT_SIZE; SET( "DRAW_EXTENSION_FONT_SIZE", value); p.RepaintAllViews(); } }
        internal int DRAW_EXTENSION_OFFSET_X { get { return GET("DRAW_EXTENSION_OFFSET_X", 0); } set {var r = DRAW_EXTENSION_OFFSET_X; SET( "DRAW_EXTENSION_OFFSET_X", value); p.RepaintAllViews(); } }
        internal int DRAW_EXTENSION_OFFSET_Y { get { return GET("DRAW_EXTENSION_OFFSET_Y", 0); } set {var r = DRAW_EXTENSION_OFFSET_Y; SET( "DRAW_EXTENSION_OFFSET_Y", value); p.RepaintAllViews(); } }

    }
}
