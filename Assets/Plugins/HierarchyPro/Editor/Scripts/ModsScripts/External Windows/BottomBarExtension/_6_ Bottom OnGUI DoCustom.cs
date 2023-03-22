/*using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if PROJECT
    using EModules.Project;
#endif
//namespace EModules



namespace EModules.EModulesInternal

{
internal partial class Adapter {
    internal Hierarchy.M_CustomIcons M_CustomIconsModule;
    // internal IModuleOnnector_M_CustomIcons IM_CustomIconsModule;
    
    internal partial class BottomInterface {
    
    
    
    
    
        private void DoCustom( Rect line, int LH, BottomController controller, int scene )       // refStyle = EditorStyles.helpBox;
        {   refStyle = EditorStyles.toolbarButton;
            refColor = tst;
            adapter.bottomInterface.GET_BOOKMARKS( ref list, scene );
            
            if ( !controller.CheckCategoryIndex( scene ) )
            {   var c = controller.GetCurerentCategoryName();
            
                var tooltip = GETTOOLTIPPEDCONTENT(MemType.Custom, null, controller);
                tooltip.text = "\"" + Adapter.GET_SCENE_BY_ID( scene ).name + "\" doesn't contain a \"" + c + "\" category";
                tooltip.tooltip = "";
                Label( line, tooltip );
                
                return;
            }
            var BG_COLOR = list[controller.GetCategoryIndex(scene)].GET_COLOR() ?? RowsParams[PLUGIN_ID.BOOKMARKS].BgColorValue;
            if ( controller.GetCategoryIndex( scene ) == 0 ) BG_COLOR = adapter.bottomInterface.RowsParams[0].BgColorValue;
            
            if ( Event.current.type == EventType.Repaint )
            {   if ( BG_COLOR.a != 0 )
                    Adapter.DrawRect( line, BG_COLOR * GUI.color );
            }
        
            var l = line;
            l.y -= 1;
           
            if ( !DrawButtons( l, LH, MemType.Custom,
                               BG_COLOR.a != 0 ? Color.Lerp( WHITE, BG_COLOR, BG_COLOR.a ) : WHITE
                               , controller, scene ) )
            {   var tooltip = GETTOOLTIPPEDCONTENT(MemType.Custom, null, controller);
                tooltip.text = "Drop object on me";
                Label( l, tooltip );
            }
            
            UpdateDragArea( line, controller );
        }
        
        
        
        
        
        
        
        void DRAW_CATEGORY( Rect buttonRect, BottomController controller, int scene )
        {   var idOffset = IDOFFSET(MemType.Custom);
            var HHH =   Math.Min( 24, buttonRect.height - 2);
           
            var colorR = buttonRect;
            
            colorR.width = HHH;
            colorR.y += (buttonRect.height - HHH) / 2;
            colorR.y = Mathf.RoundToInt( colorR.y );
            colorR.height = colorR.width;
            colorR.x += (buttonRect.width - HHH) / 2;
            
            var clamp = colorR.x;
            adapter.bottomInterface.GET_BOOKMARKS( ref list, scene );
            
            DO_BUTTON( controller, scene, buttonRect, idOffset + 200, SET_BOOK_2 );
            
            Adapter.INTERNAL_BOX( Shrink( colorR, 4 ), "" );
            var BG_COLOR = list[controller.GetCategoryIndex(scene)].GET_COLOR() ?? adapter.bottomInterface.RowsParams[0].BgColorValue;
            Adapter.DrawRect( colorR, BG_COLOR );
            GUI.Label( colorR, categoryColorContent, adapter.STYLE_LABEL_8_middle );
            
            EditorGUIUtility.AddCursorRect( buttonRect, MouseCursor.Link );
        }
        
        
    }
}
}
*/