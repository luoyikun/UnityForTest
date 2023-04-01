using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;

using UnityEngine;


#pragma warning disable

namespace EMX.HierarchyPlugin.Unused
{
	public class OldBottomHelperReference
    {
        Adapter adapter;
        float lastLineHeight = -1;
        FieldInfo lastLineHeightField ;
        FieldInfo lastLineHeightField_m_Value ;
        object astsvcvalue ;
        bool lastLineHeightFieldTryGet;
        float lastHeight;
        private int UNITY_CURRENT_VERSION;
        private int UNITY_2019_1_1_VERSION;
        private float HEIGHT;
        private float? defaultextraInsertionMarkerIndent;

        internal void BOTTOM_UPDATE_POSITION( EditorWindow IN_FitteringWindow, bool log = false )     // MonoBehaviour.print(GETID());
        {

            // if ( Event.current != null && Event.current.type != EventType.Layout ) return;
            // log = true;
            if ( IN_FitteringWindow != null )
            {
                var treeView = adapter.m_TreeView(IN_FitteringWindow);

                if ( treeView == null ) return;

                var gui = adapter.guiProp.GetValue(treeView, null);

                if ( adapter.k_LineHeight != null )
                {
                    var H = GetLineHeight();


                    // adapter.HEIGHT_RIX_FUNCTIUON( IN_FitteringWindow, treeView );
                    adapter.k_LineHeight.SetValue( gui, H );

                    // Debug.Log( "ASD" );
                    adapter.m_UseHorizontalScroll.SetValue( gui, adapter.par.USE_HORISONTAL_SCROLL );

                    // MonoBehaviour.print(H);
                    if ( H != EditorGUIUtility.singleLineHeight || adapter.NeedBottomPositionUpdate )
                    {
                        adapter.NeedBottomPositionUpdate = true;
                        adapter.foldoutYOffset.SetValue( gui, Mathf.RoundToInt( (H - EditorGUIUtility.singleLineHeight) / 2 ) );

                        //MonoBehaviour.print(Event.current);

                        if ( Event.current != null )
                        {
                            var ping = adapter.m_Ping.GetValue(gui);
                            var style = adapter. m_PingStyle.GetValue(ping);

                            if ( style != null ) adapter.fixedHeight.SetValue( style, H, null );


                            var gostyle = adapter.s_GOStyles != null ? adapter.s_GOStyles.GetValue(null) : null;
                            // var gostyle = s_GOStyles.GetValue(null);
                            // if (gostyle != null)
                            {


                                //if ( Adapter.UNITY_CURRENT_VERSION >= Adapter.UNITY_2019_2_0_VERSION )
                                {
                                    var sceneStyle = adapter.sceneHeaderBg.GetValue(gostyle) as GUIStyle;
                                    sceneStyle.fixedHeight = adapter.parLINE_HEIGHT;
                                    sceneStyle.alignment = TextAnchor.MiddleLeft;

                                    if ( adapter.gs != null )
                                    {

                                        if ( !lastLineHeightFieldTryGet )
                                        {
                                            lastLineHeightFieldTryGet = true;
                                            lastLineHeightField = adapter.gs.GetField( "sceneHeaderWidth", (BindingFlags)(-1) );

                                            if ( lastLineHeightField != null )
                                            {
                                                lastLineHeightField_m_Value =
                                                    lastLineHeightField.FieldType.GetField( "m_Value", (BindingFlags)(-1) );
                                            }
                                        }

                                        var targetOffset = ( 1 - (adapter.parLINE_HEIGHT - 16) / 2 );

                                        if ( lastLineHeightField != null && (lastLineHeight != adapter.parLINE_HEIGHT || !ReferenceEquals( lastLineHeightField.GetValue( null ), astsvcvalue )
                                                                             || (float)lastLineHeightField_m_Value.GetValue( lastLineHeightField.GetValue( null ) ) != targetOffset)
                                           )
                                        {   /* var newval = Activator.CreateInstance( width .FieldType, new object[] { "SceneTopBarBg", "border-bottom-width", 0f, new long[0] } );    //new StyleState
											     width.SetValue( null, newval );*/
                                            astsvcvalue = lastLineHeightField.GetValue( null );
                                            lastLineHeightField_m_Value.SetValue( astsvcvalue, targetOffset );
                                            lastLineHeightField.SetValue( null, astsvcvalue );

                                            /*
											var v =    width .FieldType.GetProperty( "value", (BindingFlags)(-1) );
											Debug.Log((float) v.GetValue( width.GetValue( null ) ));*/
                                            lastLineHeight = adapter.parLINE_HEIGHT;
                                        }
                                    }


                                    // var prefabLabel = adapter. GetValue_Field(gs, "prefabLabel") as GUIStyle;
                                }

                                //else
                                //{	var sceneStyle = adapter.sceneHeaderBg.GetValue(gostyle);
                                //	adapter.fixedHeight.SetValue( sceneStyle, H == EditorGUIUtility.singleLineHeight ? EditorGUIUtility.singleLineHeight : 0, null );
                                //	adapter.alignment.SetValue( sceneStyle, TextAnchor.MiddleLeft, null );
                                //}




                                if ( lastHeight != H )
                                {
                                    lastHeight = H;

                                    if ( UNITY_CURRENT_VERSION < UNITY_2019_1_1_VERSION )
                                    {
                                        foreach ( var treestyle in adapter.treestyles )
                                        {
                                            var st = treestyle.GetValue(gostyle);
                                            adapter.fixedHeight.SetValue( st, H == EditorGUIUtility.singleLineHeight ? EditorGUIUtility.singleLineHeight : 0, null );
                                        }
                                    }

                                    if ( adapter.lineStyle != null )
                                    {
                                        var getst = adapter.s_Style != null ? adapter.s_Style.GetValue(null) : null;
                                        var st = adapter.lineStyle.GetValue(getst);
                                        adapter.alignment.SetValue( st, TextAnchor.MiddleLeft, null );
                                        adapter.alignment.SetValue( adapter.lineBoldStyle.GetValue( getst ), TextAnchor.MiddleLeft,
                                                                    null );
                                        // MonoBehaviour.print(fixedHeight.GetValue(st,null));
                                        //fixedHeight.SetValue(st, parLINE_HEIGHT == 16 ? 16 : 0, null);

                                        var pad3 = (RectOffset)adapter.paddingProp.GetValue(st, null);
                                        pad3.top = 0;
                                    }
                                }
                            }


                            if ( H == EditorGUIUtility.singleLineHeight && Event.current.type == EventType.Repaint )
                                adapter.NeedBottomPositionUpdate = false;
                        }
                    }

                    /* for ( int i = 1 ; i < adapter.hierarchy_windows.Count ; i++ )
					 {   var t2 = adapter.m_TreeView(adapter.hierarchy_windows[i] as EditorWindow);
					     if ( t2 == null ) continue;
					     var g2 = adapter.guiProp.GetValue(t2, null);
					     adapter.k_BaseIndent.SetValue( g2, 2 );
					     adapter.m_UseHorizontalScroll.SetValue( gui, false );
					     adapter.k_IndentWidth.SetValue( g2, 14 );
					     adapter.k_LineHeight.SetValue( g2, EditorGUIUtility.singleLineHeight );
					     adapter.foldoutYOffset.SetValue( g2, 0 );
					
					     if ( Event.current != null )
					     {   var ping = adapter.m_Ping.GetValue(g2);
					         var style = adapter.m_PingStyle.GetValue(ping);
					         if ( style != null ) adapter.fixedHeight.SetValue( style, EditorGUIUtility.singleLineHeight, null );
					     }
					 }*/
                }

                if ( adapter.k_BottomRowMargin != null )
                {
                    adapter.k_BottomRowMargin.SetValue( gui, Mathf.RoundToInt( Mathf.RoundToInt( HEIGHT ) ) );
                }

                if ( !defaultextraInsertionMarkerIndent.HasValue ) defaultextraInsertionMarkerIndent = (float)adapter.k_BaseIndent.GetValue( gui );

                var addIndent = 0;

                if ( adapter.par.DEEP_WIDTH != null )
                {
                    addIndent = Mathf.RoundToInt( 14 - adapter.par.DEEP_WIDTH.Value );
                    adapter.k_IndentWidth.SetValue( gui, adapter.par.DEEP_WIDTH.Value );
                    //k_BaseIndent.SetValue(gui, Math.Max(2, 2 + 14 - par.DEEP_WIDTH.Value));
                    //  var old = (k_BaseIndent.GetValue(gui));
                    // k_BaseIndent.SetValue(gui, 2 + 14 - par.DEEP_WIDTH.Value);
                    //MonoBehaviour.print(old + " " + k_BaseIndent.GetValue(gui));
                    //
                }

                // if ( log ) Debug.Log( "ASD" );
                if ( adapter.par.USEdefaultIconSize ) adapter.k_IconWidth.SetValue( gui, adapter.par.defaultIconSize );
                else adapter.k_IconWidth.SetValue( gui, EditorGUIUtility.singleLineHeight );

                //  k_BaseIndent.SetValue( gui, defaultextraInsertionMarkerIndent + addIndent + (adapter.par.COLOR_ICON_SIZE - 12) );
                adapter.k_BaseIndent.SetValue( gui, defaultextraInsertionMarkerIndent + addIndent );

                /*  if (extraSpaceBeforeIconAndLabel != null)
				{
				    extraSpaceBeforeIconAndLabel.SetValue(gui, (parLINE_HEIGHT - 16f) / 2, null);
				}*/
            }
        }

        private float GetLineHeight()
        {
            throw new System.NotImplementedException();
        }
    }

    class parpar
    {
        internal int defaultIconSize;

        public int? DEEP_WIDTH { get; internal set; }
        public bool USEdefaultIconSize { get; internal set; }
        public object USE_HORISONTAL_SCROLL { get; internal set; }
    }

    internal class Adapter
    {
        internal  PropertyInfo guiProp;
        internal  FieldInfo k_LineHeight;
        internal  FieldInfo m_UseHorizontalScroll;
        internal  parpar par;
        internal  PropertyInfo foldoutYOffset;
        internal  FieldInfo m_Ping;
        internal  FieldInfo m_PingStyle;
        internal  PropertyInfo fixedHeight;
        internal  FieldInfo s_GOStyles;
        internal  PropertyInfo sceneHeaderBg;
        internal  float parLINE_HEIGHT;
        internal  Type gs;
        internal  IEnumerable<FieldInfo> treestyles;
        internal  PropertyInfo lineStyle;
        internal  PropertyInfo lineBoldStyle;
        internal  PropertyInfo paddingProp;
        internal  FieldInfo s_Style;
        internal  FieldInfo k_BottomRowMargin;
        internal  FieldInfo k_BaseIndent;
        internal  FieldInfo k_IndentWidth;
        internal  FieldInfo k_IconWidth;
        internal  PropertyInfo alignment;
        internal  string pluginname;
        internal  int pluginID;

        public bool NeedBottomPositionUpdate { get; internal set; }
        public int GET_ACTIVE_SCENE { get; internal set; }

        internal object m_TreeView( EditorWindow iN_FitteringWindow )
        {
            throw new System.NotImplementedException();
        }

        internal void InternalClearDrag()
        {
            throw new NotImplementedException();
        }

        internal static void DrawRect( Rect dropArea, Color c )
        {
            throw new NotImplementedException();
        }

        internal bool IS_HIERARCHY()
        {
            throw new NotImplementedException();
        }

        internal static string isProjectObject( UnityEngine.Object o )
        {
            throw new NotImplementedException();
        }
    }







    internal partial class BottomInterfaceDrag
    {
        Adapter adapter;
        bool dragReady = false;
        bool IsValidDrag()
        {
            var type = (Editor.MemType? )DragAndDrop.GetGenericData( adapter.pluginname );
            if ( type.HasValue && type.Value == Editor.MemType.Custom ) return false;
            return GetDragData().Length != 0;
        }

        UnityEngine.Object[] GetDragData()
        {
            if ( adapter.pluginID == 0 ) return DragAndDrop.objectReferences.Select( o => o as GameObject ).Where( o => o && o.transform
                          && string.IsNullOrEmpty( AssetDatabase.GetAssetPath( o ) ) ).ToArray();

            return DragAndDrop.objectReferences.Where( o => !string.IsNullOrEmpty( Adapter.isProjectObject( o ) ) ).ToArray();
        }

        void SetDragData( UnityEngine.Object[] data, Editor.MemType? type )
        {
            if ( data != null )
            {
                adapter.InternalClearDrag();
                DragAndDrop.objectReferences = data.Where(g=>g).ToArray();
                DragAndDrop.SetGenericData( adapter.pluginname, type );
            }
        }

        void UpdateDragArea( Rect dropArea, BottomController controller )
        {
            Event currentEvent = Event.current;
            EventType currentEventType = currentEvent.rawType;
            if ( currentEventType == EventType.DragExited )
            {
                adapter.InternalClearDrag();
            }

            switch ( currentEventType )
            {
                case EventType.DragUpdated:
                    if ( IsValidDrag() )
                    {
                        dragReady = true;
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    }
                    else DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

                    EventUse();
                    break;
                case EventType.Repaint:
                    if (
                        DragAndDrop.visualMode == DragAndDropVisualMode.None ||
                        DragAndDrop.visualMode == DragAndDropVisualMode.Rejected || !dragReady ) break;
                    var c = Color.grey;
                    c.a = 0.2f;
                    Adapter.DrawRect( dropArea, c );
                    break;
                case EventType.DragPerform:
                    DragAndDrop.AcceptDrag();
                    if ( IsValidDrag() )
                    {
                        if ( adapter.IS_HIERARCHY() )
                        {
                            var res = GetDragData();
                            var ss = res.Select(r => r as GameObject).ToArray();
                            var sc = ss.FirstOrDefault();
                            if ( sc != null )
                            {
                                var scene = sc.scene.GetHashCode();
                                var result = ss.Where( s => s.scene.GetHashCode() == scene ).ToArray();
                                AddAndRefreshCustom( result, result[ 0 ], controller.GetCategoryIndex( scene ), scene );
                            }
                        }
                        else
                        {
                            var result = GetDragData().Where(o => o).ToArray();
                            if ( result.Length != 0 )
                                AddAndRefreshCustom( result, result[ 0 ], controller.GetCategoryIndex( adapter.GET_ACTIVE_SCENE ), adapter.GET_ACTIVE_SCENE );
                        }
                    }

                    adapter.InternalClearDrag();

                    EventUse();
                    break;
                case EventType.MouseUp:
                    adapter.InternalClearDrag();
                    break;

                case EventType.MouseDown:
                    break;
            }
        }

        private void AddAndRefreshCustom( UnityEngine.Object[] result, UnityEngine.Object gameObject, object p, int scene )
        {
            throw new NotImplementedException();
        }

        private void EventUse()
        {
            throw new NotImplementedException();
        }
    }

    internal class BottomController
    {
        internal object GetCategoryIndex( int scene )
        {
            throw new NotImplementedException();
        }
    }
}

#pragma warning restore