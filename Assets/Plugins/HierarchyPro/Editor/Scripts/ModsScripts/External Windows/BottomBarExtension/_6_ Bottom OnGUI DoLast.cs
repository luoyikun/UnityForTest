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



    internal partial class BottomInterface {
    
    
    
    
        Rect Round( Rect r )
        {   r.x = Mathf.RoundToInt( r.x );
            r.y = Mathf.RoundToInt( r.y );
            r.width = Mathf.RoundToInt( r.width );
            r.height = Mathf.RoundToInt( r.height );
            return r;
        }
        
        
        private void DoLast( Rect line, int LH, BottomController controller, int scene )
        {   refStyle = adapter.box;
            // refStyle = EditorStyles.toolbarButton;
            refColor = Color.white;
            
            // refColor = tst;
            
            if ( Event.current.type == EventType.Repaint )
            {
            
                if ( RowsParams[PLUGIN_ID.LAST].BgColorValue.a != 0 )
                    Adapter.DrawRect( line, RowsParams[PLUGIN_ID.LAST].BgColorValue * GUI.color );
                // else
                //EditorStyles.helpBox.Draw( line,false, false, false, false );
            }
            // line.height -= 2;
            
            var swap = adapter.par.BottomParams.SORT_LINES % 2 == 0;
            // var swap = false;
            line = Round( line );
            var plus = line;
            // var POFF = 3;
            // plus.x += POFF;
            plus.width = Mathf.RoundToInt( LINE_HEIGHT( controller.IS_MAIN ) );
            plus.x = ( !swap ? line.width - plus.width * 2 : 0) + line.x  + 1;
            //  plus.height -= 3;
            
            // if ( UNITY_CURRENT_VERSION >= UNITY_2019_3_0_VERSION ) plus.x += adapter.TOTAL_LEFT_PADDING_FORBOTTOM;
            
            
            if ( Event.current.type == EventType.MouseDown && plus.Contains( Event.current.mousePosition ) )
            {   controller.selection_button = 20;
                controller.selection_window = adapter.window();
                var captureRect = plus;
                controller.selection_action = ( mouseUp, deltaTIme ) =>
                {   if ( mouseUp && captureRect.Contains( Event.current.mousePosition ) )
                    {   if ( swap ) Adapter.MoveSelNext( adapter );
                        else Adapter.MoveSelPrev( adapter );
                        SkipRemoveFix = true;
                    }
                    return Event.current.delta.x == 0 && Event.current.delta.x == 0;
                    
                }; // ACTION
            }
            
            if ( Event.current.type == EventType.Repaint )     //MonoBehaviour.print(Adapter.GET_SKIN().button.normal.textColor);
            {
            
                adapter.STYLE_LASTSEL_BUTTON.Draw( plus, ContentSelBack, false, false, false,
                                                   plus.Contains( Event.current.mousePosition ) && controller.selection_button == 20 );
                                                   
            }
            Label( plus, swap ? ContentSelForwLabel : ContentSelBackLabel );
            EditorGUIUtility.AddCursorRect( plus, MouseCursor.Link );
            
            plus.x += plus.width;
            if ( Event.current.type == EventType.MouseDown && plus.Contains( Event.current.mousePosition ) )
            {   controller.selection_button = 21;
                controller.selection_window = controller.REFERENCE_WINDOW;
                var captureRect = plus;
                controller.selection_action = ( mouseUp, deltaTIme ) =>
                {   if ( mouseUp && captureRect.Contains( Event.current.mousePosition ) )
                    {   if ( swap ) Adapter.MoveSelPrev( adapter );
                        else Adapter.MoveSelNext( adapter );
                        SkipRemoveFix = true;
                    }
                    return Event.current.delta.x == 0 && Event.current.delta.x == 0;
                    
                }; // ACTION
            }
            if ( Event.current.type == EventType.Repaint )
            {   adapter.STYLE_LASTSEL_BUTTON.Draw( plus, ContentSelForw, false, false, false,
                                                   plus.Contains( Event.current.mousePosition ) && controller.selection_button == 21 );
            }
            Label( plus, swap ? ContentSelBackLabel : ContentSelForwLabel );
            EditorGUIUtility.AddCursorRect( plus, MouseCursor.Link );
            
            line.x += !swap ? 0 : plus.width * 2;
            line.width -= plus.width * 2;
            //line.y -= 1;
            
            
            if ( !DrawButtons( line, LH, MemType.Last,
                               // RowsParams[PLUGIN_ID.LAST].HiglighterValue
                                RowsParams[PLUGIN_ID.LAST].BgColorValue.a != 0 ? Color.Lerp( WHITE, RowsParams[PLUGIN_ID.LAST].BgColorValue, RowsParams[PLUGIN_ID.LAST].BgColorValue.a ) : WHITE
                               , controller, scene ) )
            {   var tooltip = GETTOOLTIPPEDCONTENT(MemType.Last, null, controller);
                // tooltip.text = "-";
                tooltip.text = "";
                Label( line, tooltip );
            }
        }
        
        
    }
}
}
*/