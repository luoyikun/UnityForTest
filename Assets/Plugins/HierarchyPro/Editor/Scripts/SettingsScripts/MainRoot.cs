using System.Collections.Generic;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class MainRoot : UnityEditor.Editor, IRepaint
    {

        internal const string USE_STR = ""; //"Use 

        //internal PluginInstance p { get { return Root.p[ 0 ]; } }

        public int ID()
        {
            return GetInstanceID();
        }

        public int? currentWidth()
        {
            return null;
        }

        protected static PluginInstance p { get { return Root.p[ 0 ]; } }
        static Dictionary<int, CLASS_GROUP>  __GROUP = new Dictionary<int, CLASS_GROUP>();
        internal static CLASS_GROUP GRO( IRepaint w )
        {
            if ( !__GROUP.ContainsKey( w.ID() ) ) __GROUP.Add( w.ID(), new CLASS_GROUP() { A = p, ir = w } );
            return __GROUP[ w.ID() ];
        }
        static Dictionary<int, CLASS_ENALBE>  __ENABLE = new Dictionary<int, CLASS_ENALBE>();
        internal static CLASS_ENALBE ENABLE( IRepaint w )
        {
            if ( !__ENABLE.ContainsKey( w.ID() ) ) __ENABLE.Add( w.ID(), new CLASS_ENALBE() { A = p, ir = w } );
            return __ENABLE[ w.ID() ];
            //get { return __ENABLE ?? (__ENABLE = new CLASS_ENALBE() { A = p }); }
        }

        //CLASS_GROUP __GROUP;
        //
        //internal CLASS_GROUP GRO
        //{
        //    get { return __GROUP ?? (__GROUP = new CLASS_GROUP() { A = p }); }
        //}
        //
        //CLASS_ENALBE __ENABLE;
        //
        //internal CLASS_ENALBE ENABLE
        //{
        //    get { return __ENABLE ?? (__ENABLE = new CLASS_ENALBE() { A = p }); }
        //}







        

        internal const string WIKI_1_CACHE = "H%20%5B%20Getting%20Started&Cache";
        internal const string WIKI_1_STYLE = "H%20%5B%20Main%20Hierarchy%20Settings&Window%20Style";
        internal const string WIKI_1_EVENTS = "H%20%5B%20Main%20Hierarchy%20Settings&Events";
        internal const string WIKI_1_OTHER = "H%20%5B%20Main%20Hierarchy%20Settings&Other";
        internal const string WIKI_2_TOPBAR = "I%20%5B%20Internal%20Mods&Top%20Bar";
        internal const string WIKI_2_RIGHTCLICK = "I%20%5B%20Internal%20Mods&RightClick%20Object%20Menu";
        internal const string WIKI_2_AUTOSAVE = "I%20%5B%20Internal%20Mods&AutoSave";
        internal const string WIKI_2_SNAP = "I%20%5B%20Internal%20Mods&Snap";
        internal const string WIKI_3_A_HIGH = "E%20%5B%20Left%20Drop-Down%20Window&Auto%20HighLighter";
        internal const string WIKI_3_M_HIGH = "E%20%5B%20Left%20Drop-Down%20Window&Manual%20HighLighter";
        internal const string WIKI_3_PRESETS = "E%20%5B%20Left%20Drop-Down%20Window&Presets%20Manager";
        internal const string WIKI_4_ICONS = "E%20%5B%20Right%20Bar&Icons%20for%20Components";
        internal const string WIKI_4_SETACTIVE = "E%20%5B%20Right%20Bar&SetActive%20GameObject";
        internal const string WIKI_4_PLAYMODE = "E%20%5B%20Right%20Bar&PlayMode%20Preserving";
        internal const string WIKI_4_RIGHTBAR = "E%20%5B%20Right%20Bar&RightBar%20-%20Mods%20Pack%20(9+9)";
        internal const string WIKI_4_SEARCH = "E%20%5B%20Right%20Bar&Search%20Window";

		internal const string WIKI_4_RM_DES = "E%20%5B%20Right%20Bar&RB%20-%20Descriptions";
		internal const string WIKI_4_RM_LAY = "E%20%5B%20Right%20Bar&RB%20-%20Order%20Tags%20Layers";
		internal const string WIKI_4_RM_MEM = "E%20%5B%20Right%20Bar&RB%20-%20Memory%20Info";
		internal const string WIKI_4_RM_LOC = "E%20%5B%20Right%20Bar&RB%20-%20Lock%20State";
		internal const string WIKI_4_RM_AUD = "E%20%5B%20Right%20Bar&RB%20-%20Audio";
		internal const string WIKI_4_RM_CUSTOM = "E%20%5B%20Right%20Bar&RB%20-%20Custom%20Modules";


		internal const string WIKI_5_BOTTOMBAR = "E%20%5B%20BB%20+%20External%20Mods&Bottom%20Bar";
		internal const string WIKI_5_HYPERGRAPH = "E%20%5B%20BB%20+%20External%20Mods&%20HyperGraph";
		internal const string WIKI_5_BOOKMARKS = "E%20%5B%20BB%20+%20External%20Mods&%20Bookmarks";
		internal const string WIKI_5_SELECTION = "E%20%5B%20BB%20+%20External%20Mods&%20Selection%20History";
		internal const string WIKI_5_SCENES = "E%20%5B%20BB%20+%20External%20Mods&%20Scenes%20History";
		internal const string WIKI_5_EXPAND = "E%20%5B%20BB%20+%20External%20Mods&%20Hierarchy%20Expand";
		internal const string WIKI_6_PROJECTFOLDERS = "P%20%5B%20Project%20Mods&Folders%20For%20Project";


    }
}
