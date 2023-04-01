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
    
        internal void DoScenes_Plus( int? asd )
        {
        
            var scene = asd ?? adapter.GET_ACTIVE_SCENE;
            if ( scene == -1 ) scene = SceneManager.GetActiveScene().GetHashCode();
            var all = new object[SceneManager.sceneCount].Select((s, i) => SceneManager.GetSceneAt(i)).Where(s => s.GetHashCode() != scene).ToArray();
            adapter.bottomInterface.Scene_SetDityMemory( (scene), all, true );
            RefreshMemCache( asd ?? adapter.GET_ACTIVE_SCENE );
            ClearAction();
        }
        
        
        
        private void DoScenes( Rect line, int LH, BottomController controller, int scene )
        {
        
        
            var l = line;
            
            l.width -= LINE_HEIGHT( controller.IS_MAIN );
            var plus = l;
            plus.x += plus.width - 2;
            plus.width = LINE_HEIGHT( controller.IS_MAIN ) + 2;
            if ( Event.current.type == EventType.MouseDown && plus.Contains( Event.current.mousePosition ) )
            {   var capturedPlus = plus;
                controller.selection_button = 12;
                controller.selection_window = controller.REFERENCE_WINDOW;
                controller.selection_action = ( mouseUp, deltaTIme ) =>
                {   if ( mouseUp && capturedPlus.Contains( Event.current.mousePosition ) )
                    {   DoScenes_Plus( scene );
                    }
                    return Event.current.delta.x == 0 && Event.current.delta.x == 0;
                }; // ACTION
            }
            // plus.height -= 3;
            if ( Event.current.type == EventType.Repaint )
            {   adapter.STYLE_HIERSEL_BUTTON.Draw( plus, plusContent, false, false, false,
                                                   plus.Contains( Event.current.mousePosition ) && controller.selection_button == 12 );
               
            }
            Label( plus, plusContentSceneLabel );
            EditorGUIUtility.AddCursorRect( plus, MouseCursor.Link );
            
            
            
            
        
            
            
            
            refStyle = EditorStyles.toolbarButton;
            refColor = Color.white;
            //   wasSceneDraw = false;
            if ( Event.current.type == EventType.Repaint )
                EditorStyles.helpBox.Draw( l, false, false, false, false );
            // line.x = 0;
            if ( !DrawButtons( l, LH, MemType.Scenes, WHITE, controller, scene ) )
            {   var tooltip = GETTOOLTIPPEDCONTENT(MemType.Scenes, null, controller);
                tooltip.text = "";
                Label( l, tooltip );
            }
        }
        
        
    }
}
}
            */