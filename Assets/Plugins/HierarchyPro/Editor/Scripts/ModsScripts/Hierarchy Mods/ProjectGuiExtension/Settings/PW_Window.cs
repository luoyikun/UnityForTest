using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class PW_Window : ScriptableObject
    {
    }

    [CustomEditor(typeof(PW_Window))]
    class SETGUI_ProjectWindow : MainRoot
    {


        internal static string set_text =  USE_STR + "Files Extensions Drawing";
        internal static string set_key = "USE_PROJECT_GUI_EXTENSIONS";


        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }
        public override void OnInspectorGUI()
        {
            _GUI( (IRepaint)this );
        }
        public static void _GUI( IRepaint w )
        {
            Draw.RESET(w);
            Draw.BACK_BUTTON(w);

            Draw.TOG_TIT(set_text, set_key);
            Draw.Sp(10);
            using (ENABLE(w).USE(set_key))
            {

                using ( GRO(w).UP( 0 ) )
                {
                    //Draw.TOG("Display files extension", "DRAW_EXTENSION_IN_PROJECT");

               // using (ENABLE(w).USE("DRAW_EXTENSION_IN_PROJECT", 0))
                {
                    Draw.COLOR("Extension font color", "DRAW_EXTENSION_COLOR");
                    Draw.FIELD("Extension font size", "DRAW_EXTENSION_FONT_SIZE", 4, 20);
                    Draw.FIELD("Extension offset X", "DRAW_EXTENSION_OFFSET_X", -200, 200);
                    Draw.FIELD("Extension offset Y", "DRAW_EXTENSION_OFFSET_Y", -200, 200);
                }

                }


            }
        }
    }
}

