using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor.Events
{
	public class MouseRawUp
    {
        //PluginInstance p;
        internal MouseRawUp()
        {
            //this.p = p;
        }

        Vector2? lastRepaintPos = null;

        internal bool Invoke()
        {
            if ( __OnRawMouseUp.Count != 0 )
            {
                /*   if ( win )
                   {   var pw = (new Vector2(win.position.x, win.position.y));
                       var wh = (new Vector2(win.position.x + win.position.width, win.position.y + win.position.height));

                       var mp = EditorGUIUtility.GUIToScreenPoint(e.mousePosition);
                       Debug.Log( mp + " " + wh + " " + pw );
                       if ( mp.x < pw.x || mp.x > wh .x || mp.y < pw.y || mp.y > wh .y)
                       {   return;
                       }
                   }*/

                //S	Debug.Log(Event.current.type);
                //  if ( hierScrilEv22 != e.type )
                var e = Event.current;
                {
                    hierScrilEv22 = e.type;


                    if ( hierScrilEvMousePoss22 != EditorGUIUtility.GUIToScreenPoint( e.mousePosition )
                    //hierScrilEv22 != e.type && window()
                    )
                    {
                        wasMouse = 1;
                        //  Debug.Log( "MOVING : " + hierScrilEvMousePoss22 + " - " + EditorGUIUtility.GUIToScreenPoint( e.mousePosition ) );
                        StartLeaveMouse();

                        if ( win ) win.wantsMouseMove = true;
                    }

                    bool b_wasDrag = false;

                    if ( e.type == EventType.MouseDrag )
                    {
                        wasMouse = 0;
                        wasDrag = 0;

                        if ( win ) win.wantsMouseMove = true;

                        //   Debug.Log( "drag" );
                        b_wasDrag = true;
                    }

                    if ( wasMouse != 0 )
                    {
                        // window().wantsMouseMove = true;


                        if ( !b_wasDrag )
                        { // Debug.Log( e.type + " " + wasMouse );
                            if ( e.type == EventType.Repaint && lastRepaintPos != EditorGUIUtility.GUIToScreenPoint( e.mousePosition ) )
                            {
                                wasMouse--;
                                wasDrag++;
                                // Debug.Log( "REPAINT : " + lastRepaintPos + " - " + EditorGUIUtility.GUIToScreenPoint( e.mousePosition ) );
                                lastRepaintPos = EditorGUIUtility.GUIToScreenPoint( e.mousePosition );

                                if ( win ) win.wantsMouseMove = true;
                            }

                            if ( /*wasMouse == 0*/ wasDrag == 3 )
                            {
                                WantMouseLeave( WantMouseLeaveType.DragOut );

                                // window().wantsMouseMove = false;
                                if ( win ) win.wantsMouseMove = false;
                            }
                        }

                        if ( e.type == EventType.Repaint )
                        {
                            hierScrilEvMousePoss22 = EditorGUIUtility.GUIToScreenPoint( e.mousePosition );
                        }

                        /*
                          hierScrilEvMousePoss22 = e.mousePosition;
                          Debug.Log( e.type );
                          //if (OnRawMouseUp != null )
                          window().wantsMouseMove = true;

                          if ( e.isMouse )
                          {   wasMouse = 2;
                              //Debug.Log( "SET : " + 2 );
                          }
                          if ( e.type == EventType.Used )
                          {   wasMouse = 2;
                              //  Debug.Log( "SET2 : " + 2 );
                          }
                          if ( e.type == EventType.Repaint )
                          {   if ( wasMouse == 0 )
                              {   WantMouseLeave();
                                  window().wantsMouseMove = false;
                                  // Debug.Log( "!=0 : " + wasMouse );
                                  // window().wantsMouseMove = false;
                              }
                              else
                              {   wasMouse--;
                                  //  Debug.Log( "___ : " + wasMouse );
                              }
                          }*/
                    }
                }


                if ( e.type == EventType.MouseUp || /* e.type == EventType.MouseDown || */e.keyCode == KeyCode.Escape )
                {
                    WantMouseLeave( WantMouseLeaveType.MouseUp );
                    wasMouse = 0;
                    //Debug.Log( "ASD" );
                    if ( win ) win.wantsMouseMove = false;

                    //  window().wantsMouseMove = false;
                }
                //EMX_TODO check new __OnRawMouseUp.Count == 0;
                return __OnRawMouseUp.Count == 0;
            }
            return false;
        }

        int wasMouse = 0;
        int wasDrag = 0;
        List< Func<WantMouseLeaveType, bool>> __OnRawMouseUp = new List<Func<WantMouseLeaveType, bool>>();
        Vector2? hierScrilEvMousePoss22 = null;
        EventType? hierScrilEv22 = null;
        EditorWindow win;

        internal void PUSH_ONMOUSEUP( Func<WantMouseLeaveType, bool> ac, EditorWindow win = null )
        {
            __OnRawMouseUp.Add( ac );
            StartLeaveMouse();
            this.win = win;

            if ( win ) win.wantsMouseMove = true;

            wasDrag = 0;
            lastRepaintPos = EditorGUIUtility.GUIToScreenPoint( Event.current.mousePosition );
            // Debug.Log( "PUSH" );
        }

        void StartLeaveMouse()
        {
            hierScrilEvMousePoss22 = EditorGUIUtility.GUIToScreenPoint( Event.current.mousePosition );
            // window().wantsMouseMove = true;
            // hierScrilEv22 = null;
        }
        public enum WantMouseLeaveType { Escape, MouseUp, DragOut, Other, DoesNotClear }
        internal void WantMouseLeave( WantMouseLeaveType type )
        {
            if ( __OnRawMouseUp.Count == 0 ) return;
            for ( int i = __OnRawMouseUp.Count - 1; i >= 0; i-- )
            {
                if ( __OnRawMouseUp[ i ]( type ) ) __OnRawMouseUp.RemoveAt( i );
            }
            //if (__OnRawMouseUp != null)
            //{
            //	__OnRawMouseUp(type);
            //	__OnRawMouseUp = null;
            //}
        }
    }

}
