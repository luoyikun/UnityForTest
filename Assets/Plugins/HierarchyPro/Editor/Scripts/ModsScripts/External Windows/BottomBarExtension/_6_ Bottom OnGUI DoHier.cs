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
    
    
        internal void DoHier_Plus( int? asd, BottomController controller )
        {
        
        
            var scene = asd ?? adapter.GET_ACTIVE_SCENE;
            
            GameObject[] snapShot_hierarchy = adapter.IS_HIERARCHY() ? CREATE_EXPAND_GO_SNAPSHOT(scene) : new GameObject[0];
            string[] snapShot_project = adapter.IS_PROJECT() ? CREATE_EXPAND_GO_SNAPSHOT_FORPROJECT() : new string[0];
            string[] paths = snapShot_project.Select(AssetDatabase.GUIDToAssetPath).ToArray();
            
#pragma warning disable
            SHOW_STRING( "Hierarchy Expanded Objects", "Expanded 1", ( value ) =>
            {   if ( string.IsNullOrEmpty( value ) ) return;
            
                var d = adapter.MOI.des(scene);
                var ier = d.HierarchyCache();
              
                
                UniversalAddAndRefresh( ref ier, new HierarchySnapShotArray() { array = snapShot_hierarchy, GUIDarray = snapShot_project, PATHarray = paths, name = value }, scene );
            }, controller );
#pragma warning restore
            
           
        }
        
        
        private void DoHier( Rect line, int LH, BottomController controller, int scene )
        {   refStyle = EditorStyles.toolbarButton;
            refColor = Color.white;
            
            
            
            
            
            
            var l = line;
            
            l.width -= (LINE_HEIGHT( controller.IS_MAIN ));
            var plus = l;
            plus.x += plus.width - 2;
            plus.width = LINE_HEIGHT( controller.IS_MAIN ) + 2;
            if ( Event.current.type == EventType.MouseDown && plus.Contains( Event.current.mousePosition ) )
            {   var capturedPlus = plus;
                controller.selection_button = 10;
                controller.selection_window = controller.REFERENCE_WINDOW;
                controller.selection_action = ( mouseUp, deltaTIme ) =>
                {   if ( mouseUp && capturedPlus.Contains( Event.current.mousePosition ) )     //  var scene = Adapter.LastActiveScene;
                    {   DoHier_Plus( scene, controller );
                    }
                    return Event.current.delta.x == 0 && Event.current.delta.x == 0;
                    
                }; // ACTION
            }
            //    plus.height -= 3;
            if ( Event.current.type == EventType.Repaint )
            {   adapter.STYLE_HIERSEL_BUTTON.Draw( plus, plusContent, false, false, false,
                                                   plus.Contains( Event.current.mousePosition ) && controller.selection_button == 10 );
            }
            Label( plus, plusContentLabel );
            EditorGUIUtility.AddCursorRect( plus, MouseCursor.Link );
            
            
            
            
            l.width -= (LINE_HEIGHT( controller.IS_MAIN ) + 4);
            
            //l.width -= LINE_HEIGHT( controller.IS_MAIN ) - 2;
            //  plus.height += 3;
            // plus = l;
            plus.x -= plus.width;
            //l.x += LINE_HEIGHT( controller.IS_MAIN ) - 2;
            // plus.width = LINE_HEIGHT( controller.IS_MAIN );
            if ( Event.current.type == EventType.MouseDown && plus.Contains( Event.current.mousePosition ) )
            {   var capturedPlus = plus;
                controller.selection_button = 11;
                controller.selection_window = controller.REFERENCE_WINDOW;
                controller.selection_action = ( mouseUp, deltaTIme ) =>
                {   if ( mouseUp && capturedPlus.Contains( Event.current.mousePosition ) )
                    {   SET_EXPAND_NULL();
                    }
                    return Event.current.delta.x == 0 && Event.current.delta.x == 0;
                    
                }; // ACTION
            }
            // plus.height -= 3;
            if ( Event.current.type == EventType.Repaint )
            {   adapter.STYLE_HIERSEL_PLUS.Draw( plus, hierCollapce, false, false, false,
                                                 plus.Contains( Event.current.mousePosition ) && controller.selection_button == 11 );
            
            }
            Label( plus, hierCollapceLabel );
            EditorGUIUtility.AddCursorRect( plus, MouseCursor.Link );
            
            
            //   wasSceneDraw = false;
            //  if (Event.current.type == EventType.Repaint)                    EditorStyles.helpBox.Draw(line,  false, false, false, false);
            
            if ( !DrawButtons( l, LH, MemType.Hier, WHITE, controller, scene ) )
            {   var tooltip = GETTOOLTIPPEDCONTENT(MemType.Hier, null, controller);
                tooltip.text = "";
                Label( l, tooltip );
                
                Label( l, "-" );
                
            }
        }
        
    }
}
}
*/