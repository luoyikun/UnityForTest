namespace EMX.HierarchyPlugin.Editor
{

	partial class EditorSettingsAdapter
    {

		internal bool SCENE_HISTORY_DRAW_FULLPATH_FOR_MENU { get { return GET( "SCENE_HISTORY_DRAW_FULLPATH_FOR_MENU", true ); } set { var r = SCENE_HISTORY_DRAW_FULLPATH_FOR_MENU; SET( "SCENE_HISTORY_DRAW_FULLPATH_FOR_MENU", value ); } }


		/*
        internal bool ATTACH_TO_INSPECT_ONOPEN { get { return GET("ATTACH_TO_INSPECT_ONOPEN", false); } set {var r = qwe; SET( "ATTACH_TO_INSPECT_ONOPEN", value); } }

        internal int HYPERGRAPH_FONTSIZE { get { return GET("HYPERGRAPH_FONTSIZE", 12); } set {var r = qwe; SET( "HYPERGRAPH_FONTSIZE", value); } }

        internal float HYPERGRAPH_SCALE { get { return Mathf.Clamp(GET("HYPERGRAPH_SCALE", 1f), 0.5f, 2f); } set {var r = qwe; SET( "HYPERGRAPH_SCALE", value); } }
        i
        */
	}
}
