
/*
using System;
using System.Collections;
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


namespace EModules.EModulesInternal


{
internal partial class Adapter {

    internal void Label( Rect r, string s, TextAnchor an )
    {   var a  = label.alignment;
        label.alignment = an;
        GUI.Label( r, s, label );
        label.alignment = a;
    }
    internal void Label( Rect r, string s )
    {   GUI.Label( r, s, label );
    }
    internal void Label( Rect r, GUIContent s )
    {   GUI.Label( r, s, label );
    }
    
    internal bool Button( Rect r, string s )
    {   return GUI.Button( r, s, button );
    }
    internal bool Button( Rect r, string s, TextAnchor an )
    {   var a  = button.alignment;
        button.alignment = an;
        var res = GUI.Button( r, s, button );
        button.alignment = a;
        return res;
    }
    internal bool Button( Rect r, GUIContent s )
    {   return GUI.Button( r, s, button );
    }
    internal bool Button( Rect r, GUIContent s, TextAnchor an )
    {   var a  = button.alignment;
        button.alignment = an;
        var res = GUI.Button( r, s, button );
        button.alignment = a;
        return res;
        
    }
    
    
    
    internal sealed partial class BottomInterface {
        internal void Label( Rect r, string s, TextAnchor an )
        {   var a  = adapter. label.alignment;
            adapter.label.alignment = an;
            GUI.Label( r, s, adapter.label );
            adapter.label.alignment = a;
        }
        internal void Label( Rect r, string s )
        {   GUI.Label( r, s, adapter.label );
        }
        internal void Label( Rect r, GUIContent s )
        {   GUI.Label( r, s, adapter.label );
        }
        
        internal bool Button( Rect r, string s )
        {   return GUI.Button( r, s, adapter.button );
        }
        internal bool Button( Rect r, string s, TextAnchor an )
        {   var a  = adapter.button.alignment;
            adapter.button.alignment = an;
            var res = GUI.Button( r, s, adapter.button );
            adapter.button.alignment = a;
            return res;
        }
        internal bool Button( Rect r, GUIContent s )
        {   return GUI.Button( r, s, adapter.button );
        }
        internal bool Button( Rect r, GUIContent s, TextAnchor an )
        {   var a  = adapter.button.alignment;
            adapter.button.alignment = an;
            var res = GUI.Button( r, s, adapter. button );
            adapter.button.alignment = a;
            return res;
            
        }
        
        
        
        
        

        
      
        
        
        
       
        
        
        
        
        
        
        
        // GEGEGENENENERIRIRICICIC MEEEENNNUUU
        // GEGEGENENENERIRIRICICIC MEEEENNNUUU
        // GEGEGENENENERIRIRICICIC MEEEENNNUUU
        
        
        
        internal void CREATE_BUTTON_CUSTOM_MENU( BottomController controller, int scene )
        {   controller.adapter.bottomInterface.GET_BOOKMARKS( ref list, scene );
            m_CREATE_BUTTON_CUSTOM_MENU( controller, scene, true );
        }
        internal GenericMenu CREATE_BUTTON_CUSTOM_MENU( BottomController controller, int scene, bool showMenu, GenericMenu menu )
        {   controller.adapter.bottomInterface.GET_BOOKMARKS( ref list, scene );
            return m_CREATE_BUTTON_CUSTOM_MENU( controller, scene, showMenu, menu );
        }
        void DO_BUTTON_DESCRIPTION( BottomController controller, int scene )
        {   if ( controller.IS_MAIN ) adapter.par.BottomParams.SHOW_ALL_DESCRIPTIONS_INHIER = !adapter.par.BottomParams.SHOW_ALL_DESCRIPTIONS_INHIER;
            else adapter.par.BottomParams.SHOW_ALL_DESCRIPTIONS_INWIN = !adapter.par.BottomParams.SHOW_ALL_DESCRIPTIONS_INWIN;
            
            controller.adapter.SavePrefs();
            controller.adapter.RepaintWindowInUpdate();
        }
        internal void DO_BUTTON_COLOR( BottomController controller, int scene )      // var pos = InputData.WidnwoRect(controller.IS_MAIN, Event.current.mousePosition, 190, 68, controller.adapter );
        {   var pos = new MousePos( Event.current.mousePosition, MousePos.Type.ColorChanger_230_0, controller.IS_MAIN, controller.adapter);
            _W__BottomWindow_ColorCategories.Init( pos, controller.adapter, scene );
        }
        
        internal GenericMenu m_CREATE_BUTTON_CUSTOM_MENU( BottomController controller, int m_scene, bool showMenu, GenericMenu menu = null )
        {   return SHOW_CATEGORY_MENU( controller, m_scene, ( scene ) => { return controller.GetCategoryIndex( scene ); }, false, showMenu, _menu: menu );
        }
        
        internal void AddFavCategory( List<Int32ListArray> capture_list, int VAR_CAT_INDEX, int scene, BottomController controller )
        {   SHOW_STRING( "New Category Name", capture_list[VAR_CAT_INDEX].name, ( value ) =>
            {   if ( string.IsNullOrEmpty( value ) ) return;
                adapter.bottomInterface.GET_BOOKMARKS( ref capture_list, scene );
                if ( capture_list.Any( b => b.name == value ) ) return;
                
                adapter.CreateUndoActiveDescription( "New Category", scene );
                
                var result = new Int32ListArray()
                {   name = value,
                        array = new List<Int32List>(),
                        
                };
                result.FavParams = 0;
                result.SET_COLOR( capture_list[VAR_CAT_INDEX].GET_COLOR() ?? adapter.bottomInterface.RowsParams[0].BgColorValue );
                capture_list.Add( result );
                adapter.SetDirtyActiveDescription( scene );
                
                controller.SetCategoryIndex( capture_list.Count - 1, scene );
                
                adapter.bottomInterface.RefreshMemCache( scene );
                
                controller.REPAINT( adapter );
                
            }, controller );
        }
    
        
        
        
        
     
        
    }
}
}
*/