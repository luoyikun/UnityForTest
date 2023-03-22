using UnityEngine;



namespace EMX.HierarchyPlugin.Editor
{

	partial class EditorSettingsAdapter
    {

        
		//internal int HYPERGRAPH_LABELS_FONT_SIZE { get { return GET( "HYPERGRAPH_LABELS_FONT_SIZE", 10 ) ; } set { var r = HYPERGRAPH_LABELS_FONT_SIZE; SET( "HYPERGRAPH_LABELS_FONT_SIZE", value ); } }


		internal bool HYPERGRAPH_EVENTS_MODE_BOOL {
            get { return GET( "HYPERGRAPH_EVENTS_MODE_BOOL", false ); }
            set {
                var r = HYPERGRAPH_EVENTS_MODE_BOOL;
               SET( "HYPERGRAPH_EVENTS_MODE_BOOL", value );
                p.modsController.INVOKE_FOR_EXTERNAL<Mods.HyperGraph.HyperGraphModWindow>( h => h.instance.RefreshWithCurrentSelection() );
            }
        }
        internal int HYPERGRAPH_EVENTS_MODE { get { return HYPERGRAPH_EVENTS_MODE_BOOL ? 1 : 0; } }
        internal bool HYPERGRAPH_SKIP_ARRAYS_BOOL_INVERCE { 
            get { return !HYPERGRAPH_SKIP_ARRAYS_BOOL; } 
            set { HYPERGRAPH_SKIP_ARRAYS_BOOL = !value; } }
        internal bool HYPERGRAPH_SKIP_ARRAYS_BOOL {
            get { return GET( "HYPERGRAPH_SKIP_ARRAYS_BOOL", false ); }
            set {
                var r = HYPERGRAPH_SKIP_ARRAYS_BOOL;
               SET( "HYPERGRAPH_SKIP_ARRAYS_BOOL", value );
                p.modsController.INVOKE_FOR_EXTERNAL<Mods.HyperGraph.HyperGraphModWindow>( h => h.instance.RefreshWithCurrentSelection() );
            }
        }
        internal int HYPERGRAPH_SKIP_ARRAYS { get { return HYPERGRAPH_SKIP_ARRAYS_BOOL ? 2 : 0; } }
        internal bool HYPERGRAPH_DISPLAY_ASSETS {
            get { return GET( "HYPERGRAPH_DISPLAY_ASSETS", true ); }
            set {
                var r = HYPERGRAPH_DISPLAY_ASSETS;
               SET( "HYPERGRAPH_DISPLAY_ASSETS", value );
                p.modsController.INVOKE_FOR_EXTERNAL<Mods.HyperGraph.HyperGraphModWindow>( h => h.instance.RefreshWithCurrentSelection() );
            }
        }
        internal bool HYPERGRAPH_CONNECT_TO_SELFT {
            get { return GET( "HYPERGRAPH_CONNECT_TO_SELFT", true ); }
            set {
                var r = HYPERGRAPH_CONNECT_TO_SELFT;
               SET( "HYPERGRAPH_CONNECT_TO_SELFT", value );
                p.modsController.INVOKE_FOR_EXTERNAL<Mods.HyperGraph.HyperGraphModWindow>( h => h.instance.RefreshWithCurrentSelection() );
            }
        }

		internal bool HYPERGRAPH_DRAWBOLD_LABEL { get { return GET( "HYPERGRAPH_DRAWBOLD_LABEL", true ); } set { var r = HYPERGRAPH_DRAWBOLD_LABEL; SET( "HYPERGRAPH_DRAWBOLD_LABEL", value ); } }
		internal bool HYPERGRAPH_CLIP_NAMES { get { return GET( "HYPERGRAPH_CLIP_NAMES", true ); } set { var r = HYPERGRAPH_CLIP_NAMES; SET( "HYPERGRAPH_CLIP_NAMES", value ); } }
		//internal bool HYPERGRAPH_DRAWBOLD_LABEL { get { return GET( "HYPERGRAPH_DRAWBOLD_LABEL", true ); } set { var r = HYPERGRAPH_DRAWBOLD_LABEL; SET( "HYPERGRAPH_DRAWBOLD_LABEL", value ); } }

		internal int HYPERGRAPH_DRAW_RED_FOR_NULLS { get { return GET( "HYPERGRAPH_DRAW_RED_FOR_NULLS", 1 ); } set { var r = HYPERGRAPH_DRAW_RED_FOR_NULLS; SET( "HYPERGRAPH_DRAW_RED_FOR_NULLS", value ); } }
		internal int HYPERGRAPH_FIELD_NAMES_ALIGNMENT { get { return GET( "HYPERGRAPH_FIELD_NAMES_ALIGNMENT", 1 ); } set { var r = HYPERGRAPH_FIELD_NAMES_ALIGNMENT; SET( "HYPERGRAPH_FIELD_NAMES_ALIGNMENT", value ); } }

		internal bool HYPERGRAPH_RESET_SCROLL_ONRELOAD { get { return GET( "HYPERGRAPH_RESET_SCROLL_ONRELOAD", false ); } set { var r = HYPERGRAPH_RESET_SCROLL_ONRELOAD; SET( "HYPERGRAPH_RESET_SCROLL_ONRELOAD", value ); } }


		internal bool ATTACH_TO_INSPECT_ONOPEN { get { return GET( "ATTACH_TO_INSPECT_ONOPEN", true ); } set {var r = ATTACH_TO_INSPECT_ONOPEN; SET( "ATTACH_TO_INSPECT_ONOPEN", value ); } }

        internal int HYPERGRAPH_OB_FONTSIZE { get { return GET( "HYPERGRAPH_FONTSIZE", 13 ); } set { var r = HYPERGRAPH_OB_FONTSIZE; SET( "HYPERGRAPH_FONTSIZE", value ); } }
		internal int HYPERGRAPH_INT_FONTSIZE { get { return GET( "HYPERGRAPH_INT_FONTSIZE", 11 ); } set { var r = HYPERGRAPH_INT_FONTSIZE;SET( "HYPERGRAPH_INT_FONTSIZE", value ); } }

        
        //internal float HYPERGRAPH_HEIGHT { get { return GET( "HYPERGRAPH_HEIGHT", 200f ); } set { var r = HYPERGRAPH_HEIGHT;SET( "HYPERGRAPH_HEIGHT", value ); } }
        internal int HYPERGRAPH_SCANPERFOMANCE04 { get { return (HYPERGRAPH_SCANPERFOMANCE - 2) / 2; } set { HYPERGRAPH_SCANPERFOMANCE = value * 2 + 2; } }
        internal int HYPERGRAPH_SCANPERFOMANCE { get { return Mathf.Clamp( (GET( "HYPERGRAPH_SCANPERFOMANCE", 4 ) - 2) / 2, 0, 4 ) * 2 + 2; } set {var r = HYPERGRAPH_SCANPERFOMANCE; SET( "HYPERGRAPH_SCANPERFOMANCE", value ); } }
        internal bool HYPERGRAPH_SHOWUPDATINGINDICATOR { get { return GET( "HYPERGRAPH_SHOWUPDATINGINDICATOR", true ); } set {var r = HYPERGRAPH_SHOWUPDATINGINDICATOR; SET( "HYPERGRAPH_SHOWUPDATINGINDICATOR", value ); } }


        internal bool HYPERGRAPH_USE_BOLD_LINES { get { return GET( "HYPERGRAPH_USE_BOLD_LINES", true ); } set { var r = HYPERGRAPH_USE_BOLD_LINES; SET( "HYPERGRAPH_USE_BOLD_LINES", value ); } }




    }
}
