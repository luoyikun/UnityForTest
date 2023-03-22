using UnityEngine;
using UnityEditor;
using System.Reflection;
#pragma warning disable
namespace EMX.HierarchyPlugin.Editor
{
	class WinBounds
    {

        internal static Vector2 MAX_WINDOW_HEIGHT
        {
            get
            {
                var WB = WINBORDER;
                return new Vector2( WB.y, WB.height );
            }
        }

        internal static Vector2 MAX_WINDOW_WIDTH
        {
            get
            {
                var WB = WINBORDER;
                return new Vector2( WB.x, WB.width );
            }
        }


        internal static Rect WINBORDER
        {
            get
            {
#if UNITY_2020_1_OR_NEWER
                return EditorGUIUtility.GetMainWindowPosition();
#else
                float TOP = float.MaxValue, RIGHT = float.MinValue, LEFT = float.MaxValue, BOTTOM = float.MinValue;
                foreach ( var item in TODO_Tools.ALL_WINDOWS )
                {
                    object hostView ;
                    Rect tempR;
                    try
                    {
                        hostView = hostView_type.GetValue( item );
                        if ( hostView == null ) continue;
                    }
                    catch { continue; }
                    // var cntxw = m_Window.GetValue(hostView);
                    try
                    {
                        while ( parent.GetValue( hostView, null ) != null )
                        {
                            hostView = parent.GetValue( hostView, null );
                        }
                    }
                    catch { continue; }
                    try
                    {
                        tempR = (Rect)screenPosition.GetValue( hostView, null );
                    }
                    catch { continue; }
                    if ( tempR.x < LEFT ) LEFT = tempR.x;
                    if ( tempR.x + tempR.width > RIGHT ) RIGHT = tempR.x + tempR.width;
                    if ( tempR.y < TOP ) TOP = tempR.y;
                    if ( tempR.y + tempR.height > BOTTOM ) BOTTOM = tempR.y + tempR.height;
                }
                if ( TOP == float.MaxValue )
                {
                    TOP = 0;
                    RIGHT = Screen.currentResolution.width;
                    BOTTOM = Screen.currentResolution.height;
                    LEFT = 0;
                }
                return new Rect( LEFT, TOP, RIGHT - LEFT, BOTTOM - TOP );
#endif
            }
        }


        static System.Reflection.FieldInfo hostView_type { get { return __hostView_type ?? (__hostView_type = typeof( EditorWindow ).GetField( "m_Parent", ~(BindingFlags.Static | BindingFlags.InvokeMethod) )); } }
        static System.Reflection.FieldInfo __hostView_type;

        static System.Reflection.PropertyInfo parent { get { return __parent ?? (__parent = hostView_type.FieldType.BaseType.BaseType.GetProperty( "parent", ~(BindingFlags.Static | BindingFlags.InvokeMethod) )); } }
        static System.Reflection.PropertyInfo __parent;

        static System.Reflection.FieldInfo m_Window { get { return __m_Window ?? (__m_Window = hostView_type.FieldType.BaseType.BaseType.GetField( "m_Window", ~(BindingFlags.Static | BindingFlags.InvokeMethod) )); } }
        static System.Reflection.FieldInfo __m_Window;

        static System.Reflection.PropertyInfo windowPosition { get { return __windowPosition ?? (__windowPosition = hostView_type.FieldType.BaseType.BaseType.GetProperty( "windowPosition", ~(BindingFlags.Static | BindingFlags.InvokeMethod) )); } }
        static System.Reflection.PropertyInfo __windowPosition;

        static System.Reflection.PropertyInfo screenPosition { get { return __screenPosition ?? (__screenPosition = hostView_type.FieldType.BaseType.BaseType.GetProperty( "screenPosition", ~(BindingFlags.Static | BindingFlags.InvokeMethod) )); } }
        static System.Reflection.PropertyInfo __screenPosition;
    }
}
#pragma warning restore