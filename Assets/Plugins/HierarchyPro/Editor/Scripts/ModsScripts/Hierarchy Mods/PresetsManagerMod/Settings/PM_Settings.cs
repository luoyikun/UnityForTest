namespace EMX.HierarchyPlugin.Editor
{

	partial class EditorSettingsAdapter
    {

        

        internal bool PRESETS_SAVE_GAMEOBJEST { get {
                //     return false;
                return GET("PRESETS_SAVE_GAMEOBJEST", true); 
                } set {var r = PRESETS_SAVE_GAMEOBJEST; SET( "PRESETS_SAVE_GAMEOBJEST", value); } }

        internal bool PRESETS_SKIP_NULL_REPLACE
        {
            get
            {
                //     return false;
                return GET("PRESETS_SKIP_NULL_REPLACE", false);
            }
            set {var r = PRESETS_SKIP_NULL_REPLACE; SET( "PRESETS_SKIP_NULL_REPLACE", value); }
        }

    }
}
