using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class ProjectSettingsParams_Window : ScriptableObject
    {

    }



    [CustomEditor( typeof( ProjectSettingsParams_Window ) )]
    class SETGUI_ProjectSettings : MainRoot
    {


        internal static string set_text =  USE_STR + "Main Project Settings (Project Window)";
        internal static string set_key = "USE_PROJECT_SETTINGS";


        public override void OnInspectorGUI()
        {
            _GUI( (IRepaint)this );
        }
        public static void _GUI( IRepaint w )
        {

            Draw.RESET(w);

            Draw.BACK_BUTTON( w );
            Draw.TOG_TIT( set_text, set_key );

            //	GUI.Button(Draw.R2, "Project Settings", Draw.s("preToolbar"));
            // GUI.Button( Draw.R, "Common Settings", Draw.s( "insertionMarker" ) );
            using ( ENABLE(w).USE( set_key ) )
            using ( GRO(w).UP( 0 ) )
            {



                SETGUI_MainSettings.QWE( w, p.par_e.PROJ_WIN_SET, () => {
                }, () => {
                }, () => {
                } );




            }
        }

    }
}
