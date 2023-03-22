using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EMX.HierarchyPlugin.Editor.Mods;
using System.Text;

namespace EMX.HierarchyPlugin.Editor.Windows
{


	public partial class Root_HighlighterWindowInterface : Windows.IWindow
    {







        /*  new void Repaint()
		  {   Debug.Log("R");
			  base.Repaint();
		  }*/



        static Dictionary<EditorWindow, FilterHelper> __FH = new Dictionary<EditorWindow, FilterHelper>();
        static internal FilterHelper GetFH( EditorWindow w )
        {
            if ( __FH.ContainsKey( w ) ) return __FH[ w ];

            __FH.Add( w, new FilterHelper( w ) );
            return
                __FH[ w ];

        }

        void DrawFilts( Rect rect )
        {
            var f = GetFH(this);
            f.source = source;
            f.DrawFilts( rect );
        }





















        internal class FilterHelper
        {

            float FilterLineHeight = 16;


            internal HierarchyObject source;
            internal FilterHelper( EditorWindow w )
            {
                ew = w;
                //adapter = a;
            }
            void Repaint()
            {
                mod.ClearCacheAdditional();
                ew.Repaint();
                Root.p[ 0 ].RepaintWindowInUpdate_PlusResetStack( adapter.pluginID );
            }
            EditorWindow ew;
            GUIContent emptyContent = new GUIContent();
            //	internal PluginInstance adapter;
            //HighlighterMod HL_MOD { get { return adapter.modsController.highLighterMod; } }

            void CHANGE_GUI() { Root_HighlighterWindowInterface.CHANGE_GUI(); }

            static Dictionary<int, float> _scrollPos = new Dictionary<int, float>();
            float scrollPos {
                get {
                    if ( !_scrollPos.ContainsKey( adapter.pluginID ) ) _scrollPos.Add( adapter.pluginID, SessionState.GetFloat( "EMX|FilterScroll" + adapter.pluginID, 0 ) );

                    return _scrollPos[ adapter.pluginID ];
                }

                set {
                    if ( value != _scrollPos[ adapter.pluginID ] ) SessionState.SetFloat( "EMX|FilterScroll" + adapter.pluginID, value );

                    _scrollPos[ adapter.pluginID ] = value;
                }
            }
            Dictionary<int, int>[] _editIndex = new Dictionary<int, int>[2];
            Dictionary<int, int> editIndex {
                get {
                    if ( _editIndex[ adapter.pluginID ] == null )
                    {
                        _editIndex[ adapter.pluginID ] = new Dictionary<int, int>();
                        int i = 0;
                        int key;

                        while ( (key = SessionState.GetInt( "EMX|EditIndex" + adapter.pluginID + " " + i, -1 )) != -1 )
                        {
                            _editIndex[ adapter.pluginID ].Add( key, -1 );
                            i++;
                        }
                    }

                    //if (!_editIndex.ContainsKey(adapter.pluginID)) _editIndex.Add(adapter.pluginID, EditorPrefs.GetInt("EModules/Hierarchy/EditIndex" + adapter.pluginID, -1) );
                    return _editIndex[ adapter.pluginID ];
                }

                set {


                    _editIndex[ adapter.pluginID ] = value;
                    currentY = new float[ 0 ];
                    int i = 0;

                    while ( SessionState.GetInt( "EMX|EditIndex" + adapter.pluginID + " " + i, -1 ) != -1 )
                    {
                        SessionState.EraseInt( "EMX|EditIndex" + adapter.pluginID + " " + i );
                        i++;
                    }

                    i = 0;

                    foreach ( var item in value )
                    {
                        SessionState.SetInt( "EMX|EditIndex" + adapter.pluginID + " " + i, item.Key );
                        i++;
                    }

                    //  if (value != _editIndex[adapter.pluginID])
                    //     _editIndex[adapter.pluginID] = value;
                }
            }







            /// <summary>
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// ///////////////////////////MAIN DRAWER////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// </summary>

            Vector2 dragPos;
            // float MouseY;
            float[] currentH = new float[0];
            float[] currentY = new float[0];
            float[] targetY = new float[0];
            //HighlighterMod mod;
            internal float DrawFilts( Rect inputrect )
            {   // if ( Event.current.type == EventType.Layout && currentY.Length != Hierarchy_GUI.Instance( a ).ColorFilters.Count ) return 0;
                //adapter = a;
                //this.mod = mod;
                //Root_HighlighterWindowInterface.adapter = a;

                if ( inputrect.height != -1 ) inputrect.height -= 10;

                inputrect.width -= 4;






                var W = inputrect.width;
                var PAD = 2;
                var H = FilterLineHeight + 2 + PAD;
                var EDIT_H = FilterLineHeight * 4 + 23 * 3 + EditorStyles.toolbarButton.fixedHeight * 2;

                var customFilters = HighLighterCommonData.GetColorFilters(source.pluginID);

                var lr = inputrect;
                lr.height = FilterLineHeight;
                //INTERNAL_BOX( new Rect( 0, 0, Math.Min( lr.width, W ) - 15, CusomIconsHeight ), PlusContentEmpty );
                //MouseY = EditorGUIUtility.GUIToScreenPoint( Vector2.zero ).y;

                // if (editIndex != -1) editIndex = Mathf.Clamp(editIndex, 0, customIcons.Count - 1);
                var startY = inputrect.height == -1 ? inputrect.y : 0f;

                if ( currentY.Length != customFilters.Count )
                {
                    currentY = new float[ customFilters.Count ];
                    targetY = new float[ customFilters.Count ];
                    currentH = new float[ customFilters.Count ];
                    float _y_interator = startY;

                    for ( int i = 0; i < customFilters.Count; i++ )
                    {
                        currentY[ i ] = _y_interator;
                        currentH[ i ] = H;

                        if ( editIndex.ContainsKey( i ) )     // currentY[i] += EDIT_H;
                        {
                            currentH[ i ] += EDIT_H;
                        }

                        targetY[ i ] = currentY[ i ];

                        _y_interator += currentH[ i ];
                    }
                }

                var contentH = currentY.Length != 0 ? targetY[currentY.Length - 1] + currentH[currentH.Length - 1] + EDIT_H : (startY + EDIT_H);

                //if (editIndex != -1)contentH += EDIT_H;
                if ( inputrect.height != -1 )
                {
                    var scrollHeightOver = inputrect.height != -1 && contentH > inputrect.height;
                    scrollPos = GUI.BeginScrollView( inputrect, new Vector2( 0, scrollPos ), new Rect( 0, 0, inputrect.width - (scrollHeightOver ? GUI.skin.verticalScrollbar.fixedWidth : 0), contentH ) ).y;
                    W -= 10;

                    if ( scrollHeightOver ) W -= GUI.skin.verticalScrollbar.fixedWidth;
                }

                for ( int i = 0; i < customFilters.Count && i < currentY.Length; i++ )
                {
                    var r = new Rect(lr.x, currentY[i], W, currentH[i] - PAD);

                    if ( dragIndex == i )     // r.x = Event.current.mousePosition.x - r.height / 2;
                    {
                        r.y = Event.current.mousePosition.y - dragPos.y - 1;
                    }

                    DrawLine( i, r );
                }


                // if (currentY.Length != 0) startY = targetY.Last() + H;
                /*  var olds = GET_SKIN().button.alignment;
				  GET_SKIN().button.alignment = TextAnchor.MiddleCenter;*/
                var lineRect = new Rect(lr.x, contentH + PAD, W, EDIT_H - PAD * 2);
                lineRect.y -= EDIT_H + PAD;
                // if (editIndex == currentY.Length - 1 && currentY.Length != 0)lineRect.y += EDIT_H;
                GUI.Box( lineRect, "" );
                //  ExampleDragDropGUI( lineRect, null, DRAG_VALIDATOR_MONOANDTEXTURE, DRAG_PERFORM_USERICONS );

                /*  if (lineRect.Contains(Event.current.mousePosition))
				  {
					  if (Event.current.type.Equals(EventType.Repaint)) GUI.DrawTexture(lineRect,Hierarchy.sec);
				  }*/

                // lineRect.width -= 15;
                if ( Button( lineRect, PlusContent, TextAnchor.MiddleCenter ) )
                {
                    if ( Event.current.button == 0 )
                    {
                        FromObjectButton( source, int.MaxValue );
                    }
                }

                // GET_SKIN().button.alignment = olds;


                if ( inputrect.height != -1 )
                    GUI.EndScrollView();




                if ( (Event.current.type == EventType.Repaint || Event.current.rawType == EventType.MouseUp || Event.current.rawType == EventType.Used) && currentY.Length != 0 && (dragIndex != -1
                        || Event.current.rawType == EventType.MouseUp || Event.current.rawType == EventType.Used
                || currentY.Select( ( c, i ) => new { c, t = targetY[ i ] }
                                          ).Any( ( c ) => Mathf.Abs( c.t - c.c ) > 0.001f )) )
                {

                    //   var estimPos = Mathf.RoundToInt((EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition).y  - inputrect.y - MouseY - (H - PAD) / 2) / (float)H);
                    var mp = Event.current.mousePosition.y;

                    if ( inputrect.height != -1 ) mp -= inputrect.y;

                    int I = 0;

                    for ( I = 0; I < currentY.Length; I++ ) if ( mp < targetY[ I ] + currentH[ I ] ) break;

                    var estimPos = I;

                    tempDragIndex = dragIndex == -1 ? -1 : Mathf.Clamp( estimPos, 0, targetY.Length - 1 );

                    if ( Event.current.type == EventType.Repaint && currentY.Length != 0 )
                    {
                        bool notCompleted = false;

                        for ( int i = 0, sib = 0; i < currentY.Length; i++, sib++ )
                        {
                            if ( dragIndex != -1 && i > dragIndex && i <= tempDragIndex ) sib = i - 1;
                            else
                                if ( dragIndex != -1 && i < dragIndex && i >= tempDragIndex ) sib = i + 1;
                            else sib = i;

                            sib = Mathf.Clamp( sib, 0, currentY.Length - 1 );
                            currentY[ i ] = Mathf.Lerp( currentY[ i ], targetY[ sib ] /*+ currentH[sib] - (dragIndex == sib ? H + EDIT_H : H)*/, 0.25f );
                            var fin = Mathf.Abs(currentY[i] - targetY[sib]) <= 1;
                            notCompleted |= fin;

                            if ( fin ) currentY[ i ] = targetY[ sib ];
                        }

                        //print(tempDragIndex);
                        if ( dragIndex != -1 || notCompleted )
                        {
                            Repaint();
                        }
                    }

                    if ( Event.current.rawType == EventType.MouseUp || Event.current.rawType == EventType.Used )
                    {
                        OnRawUp( Events.MouseRawUp.WantMouseLeaveType.MouseUp );
                    }
                }



                if ( Event.current.keyCode == KeyCode.Escape ) dragIndex = -1;







                return contentH;

            }
            GUIContent PlusContent = new GUIContent()
            {
                text = "Create New",
                tooltip = "Create new empty filter or from current selection"
            };


            int __dragIndex = -1;
            int dragIndex {
                get {
                    return __dragIndex;
                }

                set {
                    __dragIndex = value;

                    if ( value != -1 )
                    {
                        var win = ew as Windows.IWindow;

                        if ( win )
                            win.PUSH_ONMOUSEUP( OnRawUp, win );
                    }
                }
            }


            int tempDragIndex;

            bool OnRawUp( Events.MouseRawUp.WantMouseLeaveType t )
            {
                if ( dragIndex != -1 && tempDragIndex != -1 && tempDragIndex != dragIndex )
                {
                    Swap( dragIndex, tempDragIndex );
                    //  if (editIndex == tempDragIndex) editIndex = dragIndex;
                    currentY = new float[ 0 ];
                    SaveFilters( null );

                }

                dragIndex = -1;
                return true;
            }


            static string back = "{(asdasd;;;asdasd)}";
            void CopyToTextBuffer( int index )
            {
                if ( index > HighLighterCommonData.GetColorFilters( source.pluginID ).Count ) return;

                var targetFilter = HighLighterCommonData.GetColorFilters(source.pluginID)[index];
                var sb = new StringBuilder();
                targetFilter.SaveToString( ref sb );
                var res = sb.ToString();
                if ( res.Contains( "✖" ) ) res = res.Replace( "\n", back );
                else res = res.Replace( "\n", "✖" );
                res = res.Replace( "\r", "" );
                EditorGUIUtility.systemCopyBuffer = '[' + res + ']';
            }


           static string[] Split( string s, string pat )
            {
                if ( s == null ) s = "";
                List<string> result = new List<string>();
                int ind;
                do
                {
                    ind = s.IndexOf( pat );
                    if ( ind == -1 ) break;
                    result.Add( s.Remove( ind ) );
                    s = s.Substring( ind );
                    if ( s.Length == pat.Length ) s = "";
                    s = s.Substring( pat.Length );
                }
                while ( true );
                result.Add( s );
                return result.ToArray();
            }

            internal static ColorFilter GetBuffer( int i , string data = null)
            {
                try
                {
                    var res = (data ?? EditorGUIUtility.systemCopyBuffer).Trim(new[]{ ' ' , '\n', '\r','\t' } );
                    if ( res.Length < 2 ) return null;
                    if ( res[ 0 ] == '[' ) res = res.Substring( 1 );
                    if ( res[ res.Length - 1 ] == ']' ) res = res.Remove( res.Length - 1 );
                    // var s1 =Split(res ,back  );
                    // var s2 = res.Split( '✖');
                    // if ( s1.Length == 4 && s2.Length < 4 ) return "";
                    // if (s1.Length > s2.Length) return

                    string[] s = null;
                    if ( i == 0 ) s = res.Split( '✖' ); ;
                    if ( i == 1 ) s = Split( res, back );
                    var q =  s.Aggregate( ( a, b ) => a + '\n' + b );

                    var reader = new StringReader(q);
                    return ColorFilter.ReadFromString( ref reader );
                }
                catch
                {
                    return null;
                }
            }

            void PasteFromTextBuffer( int index )
            {
                if ( index > HighLighterCommonData.GetColorFilters( source.pluginID ).Count ) return;
                if ( !ValidateTextBuffer( index ) ) return;

                ColorFilter f = null;
                if ( f == null ) f = GetBuffer( 0 );
                if ( f == null ) f = GetBuffer( 1 );
                if ( f == null ) return;

                HighLighterCommonData.Undo( "Change Color Filters" );
                HighLighterCommonData.GetColorFilters( source.pluginID )[ index ] = f;
                SaveFilters( null );
            }

            bool ValidateTextBuffer( int index )
            {
                if ( string.IsNullOrEmpty( EditorGUIUtility.systemCopyBuffer ) ) return false;
                if ( index > HighLighterCommonData.GetColorFilters( source.pluginID ).Count ) return false;

                if ( GetBuffer( 0 ) == null && GetBuffer( 1 ) == null ) return false;
                return true;
            }


            void ToObjectButton( HierarchyObject reference, int index )
            {
                if ( source == null ) return;

                if ( index > HighLighterCommonData.GetColorFilters( source.pluginID ).Count ) return;

                var targetFilter = HighLighterCommonData.GetColorFilters(source.pluginID)[index];


                SetIconImage( targetFilter._icon, "Apply Filter To Object" );

                if ( targetFilter.hasColorIcon ) SetIconColor( targetFilter.colorIcon, "Apply Filter To Object" );
                else SetIconColor( Color.clear, "Apply Filter To Object" );

                //  var tempColor = new TempColorClass().AssignFromList(new SingleList() { list = Enumerable.Repeat(0, 9).ToList()});
                var tempColor = new TempColorClass().AssignFromList(0, false);
                targetFilter.AS_TEMPCOLOR_ALIGN_ONLY.OverrideTo( ref tempColor );
                SetHiglightData( tempColor, "Apply Filter To Object" );
            }


            void FromObjectButton( HierarchyObject reference, int index )
            {

                bool hasColorText = false, hasColorBg = false, hasColorIcon = false;
                Color colorText = Color.black, colotBg = Color.black, colorIcon = Color.black;
                bool child = false;
                Texture2D _icon = null;
                string _name = "New name";

                //Load
                TempColorClass data;

                if ( reference != null )
                {
                    data = GetHiglightData();
                    var cont = HighlighterCache_Icons.GetImageForObject_OnlyCacher(reference, mod);
                    _icon = cont.add_icon as Texture2D;
                    hasColorIcon = cont.add_hasiconcolor;
                    colorIcon = cont.add_iconcolor;
                    _name = reference.name;
                    //ListToBgColor(GetIconColor(), out colorIcon, out hasColorIcon);
                    var g = GetIconColor();
                    hasColorIcon = g.HasValue;
                    colorIcon = g ?? Color.clear;
                }

                else
                {
                    data = new TempColorClass();
                    data = HighLighterCommonData.GetLastTempColor();
                }

                ListToTextColor( data, out colorText, out hasColorText );
                ListToBgColor( data, out colotBg, out hasColorBg );
                child = data.child;

                //Load

                var value = new ColorFilter()
                {
                    hasColorText = hasColorText,
                    hasColorBg = hasColorBg,
                    hasColorIcon = hasColorIcon,
                    colorText = colorText,
                    colorBg = colotBg,
                    colorIcon = colorIcon,
                    _icon = _icon,
					// child = child,
					NAME = _name,
                    NameFilter = ColorFilter.ENDWITH + ColorFilter.SEPARATOR + ColorFilter.STARTWITH + ColorFilter.SEPARATOR +
                                     _name,
                    ComponentFilter = "",
                    TagFilter = "",
                    LayerFilter = ""
                };






                var tempColor = value.AS_TEMPCOLOR_ALIGN_ONLY;
                tempColor.child = data.child;
                tempColor.BG_WIDTH = data.BG_WIDTH;
                tempColor.BG_HEIGHT = data.BG_HEIGHT;
                tempColor.LABEL_SHADOW = data.LABEL_SHADOW;
                tempColor.BG_ALIGMENT_LEFT = data.BG_ALIGMENT_LEFT;
                tempColor.BG_ALIGMENT_RIGHT = data.BG_ALIGMENT_RIGHT;
                value.AS_TEMPCOLOR_ALIGN_ONLY = tempColor;










                HighLighterCommonData.Undo( "Change Color Filters" );

                ColorFilter oldFilter = null;
                if ( index >= HighLighterCommonData.GetColorFilters( source.pluginID ).Count )
                {
                    index = HighLighterCommonData.GetColorFilters( source.pluginID ).Count;
                    HighLighterCommonData.GetColorFilters( source.pluginID ).Add( value );
                }
                else
                {
                    oldFilter = HighLighterCommonData.GetColorFilters( source.pluginID )[ index ];
                    HighLighterCommonData.GetColorFilters( source.pluginID )[ index ] = value;
                }

                if ( oldFilter != null )
                {
                    HighLighterCommonData.GetColorFilters( source.pluginID )[ index ].ENABLE = oldFilter.ENABLE;
                    HighLighterCommonData.GetColorFilters( source.pluginID )[ index ].NAME = oldFilter.NAME;
                    HighLighterCommonData.GetColorFilters( source.pluginID )[ index ].NameFilter = oldFilter.NameFilter;
                    HighLighterCommonData.GetColorFilters( source.pluginID )[ index ].ComponentFilter = oldFilter.ComponentFilter;
                    HighLighterCommonData.GetColorFilters( source.pluginID )[ index ].TagFilter = oldFilter.TagFilter;
                    HighLighterCommonData.GetColorFilters( source.pluginID )[ index ].LayerFilter = oldFilter.LayerFilter;
                }

                SaveFilters( null );

            }
            void RemoveLine( int index )
            {
                if ( index < 0 || index >= HighLighterCommonData.GetColorFilters( source.pluginID ).Count ) return;

                HighLighterCommonData.Undo( "Change Color Filters" );

                HighLighterCommonData.GetColorFilters( source.pluginID ).RemoveAt( index );

                SaveFilters( null );

            }
            void Swap( int i1, int i2 )
            {
                HighLighterCommonData.Undo( "Change Color Filters" );

                var v1 = HighLighterCommonData.GetColorFilters(source.pluginID)[i1];
                HighLighterCommonData.GetColorFilters( source.pluginID ).RemoveAt( i1 );

                if ( i2 >= HighLighterCommonData.GetColorFilters( source.pluginID ).Count ) HighLighterCommonData.GetColorFilters( source.pluginID ).Add( v1 );
                else HighLighterCommonData.GetColorFilters( source.pluginID ).Insert( i2, v1 );

                SaveFilters( null );
            }








            /// <summary>
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// ////////////////////////////////HEADER FUNCS//////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// </summary>




            private void DrawColoredLabel( Rect labelRect, ColorFilter filter )
            {
                EditorGUIUtility.AddCursorRect( labelRect, MouseCursor.Text );

                // INTERNAL_BOX( labelRect );

                //labelRect = Shrink( labelRect, 1 );
                var tempColor = filter.AS_TEMPCOLOR_ALIGN_ONLY;

                if ( filter.hasColorBg )
                {
                    var bgcolorold = filter.colorBg;
                    var bgrect = labelRect;

                    if ( tempColor.BG_HEIGHT == 1 )
                    {
                        var newH = bgrect.height - label.CalcHeight(Tools.GET_CONTENT(filter.NAME), 10000);
                        bgrect.y += newH / 2;
                        bgrect.height += newH;
                    }

                    else
                        if ( tempColor.BG_HEIGHT == 2 )
                    {
                        //bgrect.y += bgrect.height / 2;
                        bgrect.height = 1;
                    }

                    bgcolorold.a *= mod.set.HIGHLIGHTER_COLOR_OPACITY;

                    mod.DRAW_BGTEXTURE_OLD( bgrect, bgcolorold );
                    // EditorGUI.DrawRect( bgrect, bgcolorold );
                }

                else
                {
                    EditorGUI.DrawRect( labelRect, Colors.EditorBGColor );
                }

                var c = label.normal.textColor;

                if ( filter.hasColorText )
                {
                    label.normal.textColor = filter.colorText;
                }

                if ( filter.hasColorText && tempColor.LABEL_SHADOW == true )
                {

                    var _oc2 = label.normal.textColor;
                    var c2 = Color.black;
                    c2.a = _oc2.a;
                    label.normal.textColor = c2;
                    labelRect.y -= 0.5f;
                    labelRect.x -= 1f;
                    Label( labelRect, filter.NAME );
                    label.normal.textColor = _oc2;
                    labelRect.y += 0.5f;
                    labelRect.x += 1f;
                }


                Label( labelRect, filter.NAME );
                label.normal.textColor = c;


            }




            void DrawFilterText( ref string text, ColorFilter filter, int case_, Rect pos )
            {
                var conditions = text.Split('╩').Select(c => c == null ? "" : c).ToArray();

                if ( conditions.Length == 0 ) conditions = new string[ 1 ] { "" };

                //  var activeIndex = Array.FindIndex(conditions,  s => s.StartsWith( ColorFilter.ACTIVE));
                var activeIndex = conditions.ToList().FindIndex(s => s.StartsWith(ColorFilter.ACTIVE));

                //   if (case_ == 0) Debug.Log(conditions[0] + " " + conditions[1] + " " + conditions[1][0] + " " + activeIndex);
                if ( activeIndex == -1 ) activeIndex = 0;


                var _case = conditions[activeIndex].Split(ColorFilter.SEPARATOR);

                if ( _case.Length == 0 ) _case = new string[ 1 ] { "" };

                pos.width -= pos.height * 1.5f;

                var newName = GUI.TextField(pos, _case.Last(), textFieldStyle);//.Trim(' ');

                if ( newName != _case.Last() )
                {
                    _case[ _case.Length - 1 ] = newName;
                    conditions[ activeIndex ] = _case.Aggregate( ( a, b ) => a + ColorFilter.SEPARATOR + b );
                    text = SetDirtyConditions( ref conditions );
                    /* currentFilter = filter;
					 if ( case_ == 0) SetDirtyConditions(ref conditions, SetTextAction0);
					 else SetDirtyConditions(ref conditions, SetTextAction1);*/
                    // Debug.Log(conditions[activeIndex]);
                }


                pos.x += pos.width;
                pos.width = pos.height * 1.2f;

                RESTORE_GUI();

                F1();
                var but = Button(pos, "▼");
                F2();

                if ( but )
                {
                    var menu = new GenericMenu();
                    currentFilter = filter;
                    Action<string> setTextAction = GetActionByIndex(case_);

					// var targetString = newName

					for ( int i = 0; i < conditions.Length; i++ )
                    {
                        var captureI = i;
                        var states = currentFilter.GetAllStates(text)[captureI];
                        var asd = ConditionToName(ref conditions[i]);
                        var name = states.GetComparationString().Replace("A",asd);

                        if ( states.NOT ) name = "NOT " + name;

                        if ( i != 0 ) name =name + " " + states.GetConditionString() ;

                        menu.AddItem( new GUIContent( name ), activeIndex == i, () => {
                            SetActiveCondition( ref conditions, setTextAction, captureI );
                        } );
                    }

                    menu.AddSeparator( "" );
                    menu.AddItem( new GUIContent( "Create New Condition" ), false, () => {
                        Array.Resize( ref conditions, conditions.Length + 1 );
                        conditions[ conditions.Length - 1 ] = "";
                        SetActiveCondition( ref conditions, setTextAction, conditions.Length - 1 );
                    } );

                    if ( conditions.Length > 1 )
                        menu.AddItem( new GUIContent( "Remove" ), false, () => {
                            UnityEditor.ArrayUtility.RemoveAt( ref conditions, activeIndex );
                            SetActiveCondition( ref conditions, setTextAction, Mathf.Clamp( activeIndex, 0, conditions.Length - 1 ) );
                        } );
                    else menu.AddDisabledItem( new GUIContent( "Remove" ) );

                    menu.ShowAsContext();
                }

                CHANGE_GUI();
            }
            int ofs;
            FontStyle ob;
            void F1()
            {
                ofs = GUI.skin.button.fontSize;
                ob = GUI.skin.button.fontStyle;
                GUI.skin.button.fontSize = Mathf.RoundToInt( FilterLineHeight / 2 );
                GUI.skin.button.fontStyle = FontStyle.Bold;
            }
            void F2()
            {
                GUI.skin.button.fontSize = ofs;
                GUI.skin.button.fontStyle = ob;
            }
            void DrawFilterConditions( ref string text, ColorFilter filter, int case_, Rect pos, bool divWidth = true )
            {
                var conditions = text.Split('╩').Select(c => c == null ? "" : c).ToArray();

                if ( conditions.Length == 0 ) conditions = new string[ 1 ] { "" };

                var activeIndex = conditions.ToList().FindIndex(s => s.StartsWith(ColorFilter.ACTIVE));

                if ( activeIndex == -1 ) activeIndex = 0;

                var states = filter.GetAllStates(text)[activeIndex];
                var r = pos;

                if ( divWidth ) r.width /= 3f;
                else r.height /= 3;

                RESTORE_GUI();

                string o1;
                bool B1;

                var en = GUI.enabled;
                GUI.enabled = !string.IsNullOrEmpty( states.filter );



                var o2 = !states.STARTWITH && !states.ENDWITH ? "Contains" : states.STARTWITH && !states.ENDWITH ? "StartWith" : states.ENDWITH && !states.STARTWITH ? "EndWith" : "Equals";
                var o2simb = states.GetComparationString();
                F1();
                if ( divWidth ) B1 = Button( r, new GUIContent( o2simb, o2 + " (" + o2simb + ")" + "\nString comparison conditions" ) );
                else B1 = Button( r, new GUIContent( "Comparison: '" + o2 + " (" + o2simb + ")" + "' ▼", "String comparison conditions" ) );
                F2();
                if ( B1 )
                {
                    var menu = new GenericMenu();
                    currentFilter = filter;
					const string asdas =  "...";


					menu.AddItem( new GUIContent( "Contains :  "+asdas+" str "+asdas+"" ), !states.STARTWITH && !states.ENDWITH, () => {
                        var s = states.SWAP_STARTWITH(false);
                        s = s.SWAP_ENDWITH( false );
                        conditions[ activeIndex ] = s.ConvertToString();
                        SetDirtyConditions( ref conditions, GetActionByIndex( case_ ) );
                    } );
                    menu.AddItem( new GUIContent( "StartWith :  str "+asdas+"" ), states.STARTWITH && !states.ENDWITH, () => {
                        var s = states.SWAP_STARTWITH(true);
                        s = s.SWAP_ENDWITH( false );
                        conditions[ activeIndex ] = s.ConvertToString();
                        SetDirtyConditions( ref conditions, GetActionByIndex( case_ ) );
                    } );
                    menu.AddItem( new GUIContent( "EndWith :  "+asdas+" str" ), !states.STARTWITH && states.ENDWITH, () => {
                        var s = states.SWAP_STARTWITH(false);
                        s = s.SWAP_ENDWITH( true );
                        conditions[ activeIndex ] = s.ConvertToString();
                        SetDirtyConditions( ref conditions, GetActionByIndex( case_ ) );
                    } );
                    menu.AddItem( new GUIContent( "Equals :   ==" ), states.STARTWITH && states.ENDWITH, () => {
                        var s = states.SWAP_STARTWITH(true);
                        s = s.SWAP_ENDWITH( true );
                        conditions[ activeIndex ] = s.ConvertToString();
                        SetDirtyConditions( ref conditions, GetActionByIndex( case_ ) );
                    } );
                    menu.ShowAsContext();
                }


                if ( divWidth ) r.x += r.width;
                else r.y += r.height;


                F1();
                if ( divWidth ) B1 = Button( r, new GUIContent( "Other", o2 + "\nOther Options" ) );
                else B1 = Button( r, new GUIContent( "Other ▼", "Other Options" ) );
                F2();
                if ( B1 )
                {
                    var menu = new GenericMenu();
                    currentFilter = filter;
                    //  if (activeIndex != 0)
                    menu.AddItem( new GUIContent( "! - Not equal" ), states.NOT && !states.OR, () => {
                        var s = states.SWAP_NOT(!states.NOT);
                        conditions[ activeIndex ] = s.ConvertToString();
                        SetDirtyConditions( ref conditions, GetActionByIndex( case_ ) );
                    } );
                    //  else  menu.AddDisabledItem(new GUIContent("Not (!..) - Not equal"));
                    menu.AddItem( new GUIContent( "Ignore Case (AbCd)" ), states.IGNORECASE, () => {
                        var s = states.SWAP_IGNORECASE(!states.IGNORECASE);
                        conditions[ activeIndex ] = s.ConvertToString();
                        SetDirtyConditions( ref conditions, GetActionByIndex( case_ ) );
                    } );
                    menu.ShowAsContext();
                }



                GUI.enabled = en;



                if ( divWidth )
                {
                    //r.x += r.width / 2;
                    //r.width *= 1.1f;
                    r.x += r.width;
                }
                else r.y += r.height;


                if ( activeIndex != 0 )
                {
                    GUI.enabled = !string.IsNullOrEmpty( states.filter );
                    o1 = states.GetConditionString();
                    B1 = false;

                    F1();

                    if ( divWidth ) B1 = Button( r, new GUIContent( o1, o1 + "\nConditions for several conditions (Option available for additional conditions)" ) );
                    else B1 = Button( r, new GUIContent( "Operator: '" + o1 + "' ▼", "Conditions for several conditions (Option available for additional conditions)" ) );

                    F2();

                    if ( B1 )
                    {
                        var menu = new GenericMenu();
                        currentFilter = filter;
                        menu.AddItem( new GUIContent( "&&" ), !states.OR, () => {
                            var s = states.SWAP_OR(false);
                            s = s.SWAP_AND( true );
                            //states.OR = false;
                            // states.AND = true;
                            conditions[ activeIndex ] = s.ConvertToString();
                            SetDirtyConditions( ref conditions, GetActionByIndex( case_ ) );
                        } );
                        menu.AddItem( new GUIContent( "||" ), states.OR, () => {
                            var s = states.SWAP_OR(true);
                            s = s.SWAP_AND( false );
                            conditions[ activeIndex ] = s.ConvertToString();
                            SetDirtyConditions( ref conditions, GetActionByIndex( case_ ) );
                        } );
                        menu.ShowAsContext();
                    }
                }

                /*var newNo = EditorGUI.ToggleLeft(r, new GUIContent("Not", "The result of the comparison will be excluded (Option available for additional conditions)"), states.NOT);
				if (newNo != states.NOT)
				{   states.NOT = newNo;
					currentFilter = filter;
					conditions[activeIndex] = states.ConvertToString();
					if ( case_ == 0) SetDirtyConditions(ref conditions, SetTextAction0);
					else SetDirtyConditions(ref conditions, SetTextAction1);
				}*/

                CHANGE_GUI();
                GUI.enabled = true;
            }



            string ConditionToName( ref string condition )
            {
                var arr = condition.Split(ColorFilter.SEPARATOR);

                if ( arr.Length == 0 ) return "- no filter string -";

                return string.IsNullOrEmpty( arr.Last() ) ? "- no filter string -" : arr.Last();
            }
            string SetDirtyConditions( ref string[] conditionslots )
            {
                return conditionslots.Aggregate( ( s1, s2 ) => s1 + '╩' + s2 );
            }
            void SetDirtyConditions( ref string[] conditionslots, Action<string> setText )
            {
                HighLighterCommonData.Undo( "Change Color Filters" );
                setText( conditionslots.Aggregate( ( s1, s2 ) => s1 + '╩' + s2 ) );
                SaveFilters( null );

            }
            void SetActiveCondition( ref string[] conditions, Action<string> setText, int captureI )
            {
                if ( conditions == null ) throw new Exception( "ref string[] conditions == null" );

                for ( int n = 0; n < conditions.Length; n++ ) if ( conditions[ n ].StartsWith( ColorFilter.ACTIVE ) ) conditions[ n ] = conditions[ n ].Substring( ColorFilter.ACTIVE.Length );

                conditions[ captureI ] = ColorFilter.ACTIVE + conditions[ captureI ];
                SetDirtyConditions( ref conditions, setText );
            }
            ColorFilter currentFilter;
            void SetTextAction0( string t )     //var ac = currentFilter.NameFilter.StartsWith(ColorFilter.ACTIVE);
            {
                currentFilter.NameFilter = t;
                // if (ac && !currentFilter.NameFilter.StartsWith(ColorFilter.ACTIVE)) currentFilter.NameFilter = ColorFilter.ACTIVE + currentFilter.NameFilter;
            }
            void SetTextAction1( string t )     //var ac = currentFilter.ComponentFilter.StartsWith(ColorFilter.ACTIVE);
            {
                currentFilter.ComponentFilter = t;
                //if (ac && !currentFilter.ComponentFilter.StartsWith(ColorFilter.ACTIVE)) currentFilter.ComponentFilter = ColorFilter.ACTIVE + currentFilter.ComponentFilter;
            }

            void SetTextAction2( string t )     //var ac = currentFilter.ComponentFilter.StartsWith(ColorFilter.ACTIVE);
            {
                currentFilter.TagFilter = t;
                //if (ac && !currentFilter.ComponentFilter.StartsWith(ColorFilter.ACTIVE)) currentFilter.ComponentFilter = ColorFilter.ACTIVE + currentFilter.ComponentFilter;
            }
            void SetTextAction3( string t )     //var ac = currentFilter.ComponentFilter.StartsWith(ColorFilter.ACTIVE);
            {
                currentFilter.LayerFilter = t;
                //if (ac && !currentFilter.ComponentFilter.StartsWith(ColorFilter.ACTIVE)) currentFilter.ComponentFilter = ColorFilter.ACTIVE + currentFilter.ComponentFilter;
            }

            Action<string> GetActionByIndex( int index )
            {
                switch ( index )
                {
                    case 0: return SetTextAction0;

                    case 1: return SetTextAction1;

                    case 2: return SetTextAction2;

                    case 3: return SetTextAction3;
                }

                return null;
            }







            /// <summary>
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// ///////////////////////////////COMMON OPTIONS SUCH ALIGN//////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// </summary>
            void SaveFilters( ColorFilter filter )
            {
                mod.autoMod.__FilterCacheClear();

                if ( filter != null ) HighLighterCommonData.SetLastTempColor( filter.AS_TEMPCOLOR_ALIGN_ONLY );

                HighLighterCommonData.SetDirty();
                Repaint();
                REPAINT_ALL_HIERW();
            }
            void LABEL_DrawOtherButton( Rect lineRect, ColorFilter filter, bool enalbe )
            {   //  var tempColor = filter.AS_TEMPCOLOR_ALIGN_ONLY;

                F1();
                var ge = GUI.enabled;
                GUI.enabled &= enalbe;
                var B1 = Button(lineRect, new GUIContent("Other ▼", "Other Options"));
                GUI.enabled = ge;
                F2();

                if ( B1 )
                {
                    var menu = new GenericMenu();
                    currentFilter = filter;

                    //else  menu.AddDisabledItem(new GUIContent("Not (!..) - Not equal"));
                    menu.AddItem( new GUIContent( "Has Label Shadow" ), filter.LABEL_SHADOW, () => {
                        HighLighterCommonData.Undo( "Change Color Filters" );
                        filter.LABEL_SHADOW = !filter.LABEL_SHADOW;
                        // filter.AS_TEMPCOLOR_ALIGN_ONLY = tempColor;
                        SaveFilters( filter );
                    } );
                    menu.ShowAsContext();
                }
            }
            void BACKGROUND_DrawOtherButton( Rect lineRect, ColorFilter filter, bool enalbe )
            {   // var tempColor = filter.AS_TEMPCOLOR_ALIGN_ONLY;

                F1();
                var ge = GUI.enabled;
                GUI.enabled &= enalbe;
                var B1 = Button(lineRect, new GUIContent("Other ▼", "Other Options"));
                GUI.enabled = ge;
                F2();

                if ( B1 )
                {
                    var menu = new GenericMenu();
                    currentFilter = filter;

                    //else  menu.AddDisabledItem(new GUIContent("Not (!..) - Not equal"));
                    menu.AddItem( new GUIContent( "Narrow Height" ), filter.BG_HEIGHT == 1, () => {
                        HighLighterCommonData.Undo( "Change Color Filters" );

                        if ( filter.BG_HEIGHT != 1 ) filter.BG_HEIGHT = 1;
                        else filter.BG_HEIGHT = 0;

                        //  filter.AS_TEMPCOLOR_ALIGN_ONLY = tempColor;
                        SaveFilters( filter );
                    } );
                    menu.AddItem( new GUIContent( "1 pixel Height" ), filter.BG_HEIGHT == 2, () => {
                        HighLighterCommonData.Undo( "Change Color Filters" );

                        if ( filter.BG_HEIGHT != 2 ) filter.BG_HEIGHT = 2;
                        else filter.BG_HEIGHT = 0;

                        // filter.AS_TEMPCOLOR_ALIGN_ONLY = tempColor;
                        SaveFilters( filter );
                    } );
                    menu.ShowAsContext();
                }
            }
            void BACKGROUND_DrawAlignLine( Rect lineRect, ColorFilter filter )
            {
                lineRect.height = FilterLineHeight;
                var tempColor = filter.AS_TEMPCOLOR_ALIGN_ONLY;

                var oldl = tempColor.BG_ALIGMENT_LEFT;
                var oldr = tempColor.BG_ALIGMENT_RIGHT;
                var oldw = tempColor.BG_WIDTH;

                var RECT = lineRect;
                RECT.width /= 3;

                /*RECT.width /= 2;
				LABEL (RECT, "<i>Left:</i>");
				RECT.x += RECT.width;
				RECT.width *= 2;*/

                tempColor.BG_ALIGMENT_LEFT = EditorGUI.Popup( RECT, tempColor.BG_ALIGMENT_LEFT, tempColor.ALIGMENT_LEFT_CATEGORIES, EditorStyles.toolbarButton );
                HighLighterStyle.TOOLTIP( RECT, "Left Align Position for Background Color." );
                RECT.x += RECT.width;

                /* RECT.width /= 2;
				 LABEL (RECT, "<i>Right:</i>");
				 RECT.x += RECT.width;
				 RECT.width *= 2;*/

                var cats = tempColor.ALIGMENT_RIGHT_CATEGORIES.ToList();
                cats.Reverse();
                cats.Add( "Fixed Width" );
                cats.Reverse();
                tempColor.BG_ALIGMENT_RIGHT = 5 - EditorGUI.Popup( RECT, 5 - tempColor.BG_ALIGMENT_RIGHT, cats.ToArray(), EditorStyles.toolbarButton );
                HighLighterStyle.TOOLTIP( RECT, "Right Align Position for Background Color." );
                RECT.x += RECT.width;

                if ( tempColor.BG_ALIGMENT_RIGHT == 5 )
                {
                    tempColor.BG_WIDTH = Mathf.Clamp( EditorGUI.IntField( RECT, tempColor.BG_WIDTH, textFieldStyle ), 10, 255 );
                }


                if (
                    oldl != tempColor.BG_ALIGMENT_LEFT ||
                    oldr != tempColor.BG_ALIGMENT_RIGHT ||
                    oldw != tempColor.BG_WIDTH )
                {
                    HighLighterCommonData.Undo( "Change Color Filters" );
                    filter.AS_TEMPCOLOR_ALIGN_ONLY = tempColor;
                    SaveFilters( filter );
                }

            }


            /// <summary>
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////DRAW LINE///////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// </summary>


            GUIStyle _textFieldStyleTransparent;
            GUIStyle textFieldStyleTransparent {
                get {
                    if ( _textFieldStyleTransparent == null )
                    {

                        _textFieldStyleTransparent = new GUIStyle( Root.p[ 0 ].GET_SKIN().textField );
                        _textFieldStyleTransparent.alignment = TextAnchor.MiddleLeft;
                        _textFieldStyleTransparent.normal.textColor = Color.clear;
                    }

                    return _textFieldStyleTransparent;
                }
            }
            GUIStyle _textFieldStyle;
            GUIStyle textFieldStyle {
                get {
                    if ( _textFieldStyle == null )
                    {
                        _textFieldStyle = new GUIStyle( Root.p[ 0 ].GET_SKIN().textField );
                        _textFieldStyle.alignment = TextAnchor.MiddleLeft;
                    }

                    return _textFieldStyle;
                }
            }

            Vector3 sc;

            void DrawLine( int index, Rect lineRect )
            {

                var filter = HighLighterCommonData.GetColorFilters(source.pluginID)[index];

                GUI.Box( lineRect, "" );
                lineRect.y += 1;
                lineRect.x += 1;
                lineRect.width -= 2;
                lineRect.height -= 2;

                var H = FilterLineHeight;
                var rect = new Rect(lineRect.x, lineRect.y, H * 1.5f, H);


                //var om = GUI.matrix;
               // sc[ 0 ] = sc[ 1 ] = sc[ 2 ] = 0.7f;
               // GUI.matrix = Matrix4x4.Scale( sc ).MultiplyVector;
                var new_enable = EditorGUI.ToggleLeft(rect, (string)null, filter.ENABLE);
                //GUI.matrix = om;

                if ( new_enable != filter.ENABLE )
                {
                    HighLighterCommonData.Undo( "Change Color Filters" );
                    filter.ENABLE = new_enable;
                    SaveFilters( null );
                }



                rect.x += rect.width;



                var oldsl = label.fontSize;
                label.fontSize = (int)FilterLineHeight;
                Label( rect, new GUIContent( "=", "Change Order Position" ) );
                label.fontSize = oldsl;

                if ( rect.Contains( Event.current.mousePosition ) && Event.current.type == EventType.MouseDown )
                {
                    if ( Event.current.button == 0 )
                    {
                        dragIndex = index;
                        editIndex = new Dictionary<int, int>();
                        currentY = new float[ 0 ];
                        dragPos = Event.current.mousePosition - new Vector2( lineRect.x, lineRect.y );
                        Event.current.Use();
                        Repaint();
                    }
                }

                EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );


                // lineRect.x += rect.width;
                // lineRect.width -= rect.width;
                var sh = lineRect.height - FilterLineHeight * 2;
                // lineRect.height = FilterLineHeight;




                rect.x += rect.width;
                // rect.width = lineRect.width - rect.width;
                rect.width = rect.height;


                var newExpand = GUI.Toggle(rect, editIndex.ContainsKey(index), GUIContent.none, HighLighterStyle.foldoutStyle);

                if ( newExpand != editIndex.ContainsKey( index ) )
                {
                    var d = editIndex;

                    if ( newExpand ) d.Add( index, -1 );
                    else d.Remove( index );

                    editIndex = d;
                }

                rect.x += rect.width * 1.1f;


                rect.width = 1;
                //if (filter._icon) INTERNAL_BOX(Shrink(rect, 1));
                rect.x += rect.width;
                rect.width = FilterLineHeight;

                if ( filter._icon )     // INTERNAL_BOX(Shrink(rect, 1));
                {
                    DrawTexture( Shrink( rect, 2 ), filter._icon, filter.hasColorIcon ? filter.colorIcon : Color.white );
                }

                rect.x += rect.width;




                rect.width = lineRect.width - (rect.x - lineRect.x);
                rect.width -= rect.height * 12f + 5f;


                //LABEL NAME
                /*  if (filter.hasColorBg) EditorGUI.DrawRect(rect, filter.colorBg);
				  var obc = GUI.backgroundColor;
				  var otc = GUI.skin.textField.normal.textColor;
				  if (filter.hasColorBg && filter.colorBg.a > 0.01 )
				  {   GUI.backgroundColor *= filter.colorBg;
					  GUI.backgroundColor *= new Color(1, 1, 1, 0.5f);
					  EditorGUI.DrawRect(rect, filter.colorBg );
				  }
				  if (filter.hasColorText && filter.colorText.a > 0.01 ) GUI.skin.textField.normal.textColor = filter.colorText;
				  var new_SlotName = GUI.TextFieDld(rect, filter.SlotName);
				  GUI.backgroundColor = obc;
				  GUI.skin.textField.normal.textColor = otc;*/

                /* var oc = GUI.skin.label.normal.textColor;
				 if (filter.hasColorText) GUI.skin.label.normal.textColor = filter.colorText;
				 Label(rect, filter.SlotName);
				 GUI.skin.label.normal.textColor = oc;*/
                //  rect.x += rect.width;

                GUI.SetNextControlName( "MyTextField" );

                var new_SlotName = GUI.TextField(rect, filter.NAME, textFieldStyleTransparent);
                var occ = GUI.color;
                var control = GUI.GetNameOfFocusedControl() == "MyTextField";
                if ( control )
                {
                    GUI.color *= new Color( 1, 1, 1, 0 );

                    if ( Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Escape )
                    {
                        GUI.FocusControl( "" );
                        Root.p[ 0 ].SKIP_PREFAB_ESCAPE = true;
                        // EditorWindow.focusedWindow.Focus();
                        Tools.EventUse();
                    }
                }

                if ( new_SlotName != filter.NAME )
                {
                    HighLighterCommonData.Undo( "Change Color Filters" );
                    filter.NAME = new_SlotName;
                    SaveFilters( null );
                }

                if ( !control ) DrawColoredLabel( rect, filter );
                GUI.color = occ;







                rect.x += rect.width;
                rect.x += 5;

                //PASTE
                // RESTORE_GUI();
                /* if (Button( rect, new GUIContent( "| Edit", "Edit current filter" )))
				 {   if (Event.current.button == 0)
					 {   if (editIndex != index) editIndex = index;
						 else editIndex = new Dictionary<int, int>();
						 currentY = new float[0];
						 Repaint();
					 }
				 }
				 EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
				 if (Event.current.type == EventType.Repaint && editIndex == index) GUI.skin.button.Draw(rect, new GUIContent( "| Edit", "Edit current filter" ), true, true, false, true);
				 rect.x += rect.width;*/
                // rect.x += 5;

                rect.width = lineRect.width - (rect.x - lineRect.x);
                var cross = rect;
                cross.width = cross.height * 1.5f;
                cross.x = rect.x + rect.width - cross.width;
                rect.width -= cross.width;
                rect.width /= 2;

                EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
                var c = new GUIContent("Menu", "Filter's context menu");
                if ( Button( rect, c ) )  //✂
                {
                    var c1 = new GUIContent("Apply current filter to selected object", "Apply current filter to selected object");
                    var c2 = new GUIContent( "Overwrite current filter from selected object", "Overwrite current filter from selected object" );

                    GenericMenu menu = new GenericMenu();
                    if ( source != null )
                    {
                        menu.AddItem( c1, false, () => {
                            ToObjectButton( source, index );
                        } );
                        menu.AddItem( c2, false, () => {
                            FromObjectButton( source, index );
                        } );

                    }
                    else
                    {
                        menu.AddDisabledItem( c1 );
                        menu.AddDisabledItem( c2 );
                    }


                    menu.AddSeparator( "" );

                    var c3 = new GUIContent("Copy to os text buffer");
                    var c4 = new GUIContent( "Paste from os text buffer");

                    menu.AddItem( c3, false, () => {
                        CopyToTextBuffer( index );
                    } );
                    if ( ValidateTextBuffer( index ) )
                        menu.AddItem( c4, false, () => {
                            PasteFromTextBuffer( index );
                        } );
                    else
                        menu.AddDisabledItem( c4 );
                    menu.ShowAsContext();

                }

                /* if (source != null)
                 {
                     var c = new GUIContent("To Object", "Apply current filter to selected object");
                     var or = GUI.skin.button.richText;
                     GUI.skin.button.richText = true;
                     rect.width = GUI.skin.button.CalcSize(c).x;
                     var b = Button(rect, c);
                     GUI.skin.button.richText = or;

                     if (b)
                     {
                         if (Event.current.button == 0)
                         {
                             ToObjectButton(source, index);
                         }
                     }

                     EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);


                     rect.x += rect.width;
                     c = new GUIContent("From Object", "Overwrite current filter from selected object");
                     rect.width = GUI.skin.button.CalcSize(c).x;

                     if (Button(rect, c))  //✂
                     {
                         if (Event.current.button == 0)
                         {
                             FromObjectButton(source, index);
                         }
                     }

                     EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
                 }*/




                if ( editIndex.ContainsKey( index ) )
                {


                    // rect.x += 5;

                    //REMOVE

                }
                else
                {   /*rect.width = rect.height;
				    int SHR = 4;
				    if (filter.hasColorText && filter.colorText.a > 0.01 ) EditorGUI.DrawRect(Shrink(rect, SHR), filter.colorText);
				    else GUI.Box(Shrink(rect, SHR), "");
				    Label(rect, new GUIContent("", "Label Color"));
				
				    rect.x += rect.width;
				    if (filter.hasColorBg && filter.colorBg.a > 0.01 ) EditorGUI.DrawRect(Shrink(rect, SHR), filter.colorBg);
				    else GUI.Box(Shrink(rect, SHR), "");
				    Label(rect, new GUIContent("", "BG Color"));
				
				    rect.x += rect.width;
				    if (filter.child) GUI.Box(Shrink(rect, 0), "C");
				    else GUI.Box(Shrink(rect, SHR), "");
				    Label(rect, new GUIContent("", "Apply to child"));
				
				
				    rect.x += rect.width;
				    rect.width *= 2;
				    if ((filter.Aligment & 1 ) != 0 && (filter.Aligment & 2 ) != 0) GUI.Box(Shrink(rect, SHR), "FULL");
				    else GUI.Box(Shrink(rect, SHR), "");
				    Label(rect, new GUIContent("", "Background Align"));
				
				    rect.x += rect.width;
				    rect.width /= 2;
				    if (filter._icon  )
				    {   var asdasd = GUI.color;
				        if (filter.hasColorIcon)  GUI.color *= new Color( filter.colorIcon.r, filter.colorIcon.g, filter.colorIcon.b, 1 );
				        GUI.DrawTexture(Shrink(rect, SHR), filter._icon);
				        GUI.color = asdasd;
				    }
				    else GUI.Box(Shrink(rect, SHR), "");
				    Label(rect,  new GUIContent("", "Icon Texture"));
				
				    rect.x += rect.width;
				    if (filter.hasColorIcon && filter._icon  ) EditorGUI.DrawRect(Shrink(rect, SHR), filter.colorIcon);
				    else GUI.Box(Shrink(rect, SHR), "");
				    Label(rect, new GUIContent("", "Icon Color"));*/
                }

                if ( Button( cross, new GUIContent( "✖", "Remove Filter" ) ) )
                {
                    if ( Event.current.button == 0 )
                    {
                        RemoveLine( index );
                    }
                }

                EditorGUIUtility.AddCursorRect( cross, MouseCursor.Link );
                //  CHANGE_GUI();


                if ( editIndex.ContainsKey( index ) )
                {
                    rect = lineRect;
                    rect.height = FilterLineHeight;
                    rect.y += rect.height + 5;
                    // rect.width *= 0.95f;
                    //  rect.width /= 2;
                    //  rect.width -= 5;

                    var new_NameFilter = filter.NameFilter;
                    var new_ComponentFilter = filter.ComponentFilter;
                    var new_TagFilter = filter.TagFilter;
                    var new_LayerFilter = filter.LayerFilter;

                    //FILTER NAMES
                    Label( new Rect( rect.x, rect.y, rect.width * 0.2f, rect.height ), new GUIContent( "Name: ", "A string to compare with the name of GameObject" ), Root.p[ 0 ].STYLE_LABEL_8 );
                    GUI.SetNextControlName( "NAME" );
                    DrawFilterText( ref new_NameFilter, filter, 0, new Rect( rect.x + rect.width * 0.2f, rect.y, rect.width * 0.5f, rect.height ) );
                    DrawFilterConditions( ref new_NameFilter, filter, 0, new Rect( rect.x + rect.width * 0.7f, rect.y, rect.width * 0.3f, rect.height ) );
                    //rect.x += rect.width;
                    //  rect.x += 5;
                    rect.y += rect.height;

                    if ( source.pluginID == 0 )     //FILTER COMPS
                    {
                        Label( new Rect( rect.x, rect.y, rect.width * 0.2f, rect.height ), new GUIContent( "Component: ", "A string to compare with the name of all the GameObject's Components" ), Root.p[ 0 ].STYLE_LABEL_8 );
                        GUI.SetNextControlName( "COMP" );
                        DrawFilterText( ref new_ComponentFilter, filter, 1, new Rect( rect.x + rect.width * 0.2f, rect.y, rect.width * 0.5f, rect.height ) );
                        DrawFilterConditions( ref new_ComponentFilter, filter, 1, new Rect( rect.x + rect.width * 0.7f, rect.y, rect.width * 0.3f, rect.height ) );

                        rect.y += rect.height;
                        Label( new Rect( rect.x, rect.y, rect.width * 0.2f, rect.height ), new GUIContent( "Tag: ", "A string to compare with the tag of GameObject" ), Root.p[ 0 ].STYLE_LABEL_8 );
                        GUI.SetNextControlName( "TAG" );
                        DrawFilterText( ref new_TagFilter, filter, 2, new Rect( rect.x + rect.width * 0.2f, rect.y, rect.width * 0.5f, rect.height ) );
                        DrawFilterConditions( ref new_TagFilter, filter, 2, new Rect( rect.x + rect.width * 0.7f, rect.y, rect.width * 0.3f, rect.height ) );

                        rect.y += rect.height;
                        Label( new Rect( rect.x, rect.y, rect.width * 0.2f, rect.height ), new GUIContent( "Layer: ", "A string to compare with the layer of GameObject" ), Root.p[ 0 ].STYLE_LABEL_8 );
                        GUI.SetNextControlName( "LAYER" );
                        DrawFilterText( ref new_LayerFilter, filter, 3, new Rect( rect.x + rect.width * 0.2f, rect.y, rect.width * 0.5f, rect.height ) );
                        DrawFilterConditions( ref new_LayerFilter, filter, 3, new Rect( rect.x + rect.width * 0.7f, rect.y, rect.width * 0.3f, rect.height ) );

                    }



                    lineRect.y = rect.y + rect.height + 5;
                    lineRect.height = sh;
                    rect = lineRect;

                    /*if ( GUI.GetNameOfFocusedControl() == "NAME")
					{   DrawFilterConditions( ref new_NameFilter, filter, 0, rect);
					}
					else if ( GUI.GetNameOfFocusedControl() == "COMP")
					{   DrawFilterConditions( ref new_ComponentFilter, filter, 1, rect);
					}
					else*/
                    {
                        var PPP = 4;

                        var thri = false;

                        var seg = rect;
                        seg.x += PPP;
                        seg.width -= PPP * 2;
                        var FULL_W = seg.width;
                        seg.width = seg.width * 0.65f;
                        // var PARTS = new[] {0.3f, 0.3f, 0.2f, ( seg.height * 2) / FULL_W, 0.4f};
                        // var DIF = (PARTS[0] + PARTS[2] + PARTS[4] ) / (1 - PARTS[3]);
                        // for (int i = 0; i < PARTS.Length; i++) if (i != 3) PARTS[i] /= DIF;
                        seg.height = 23 * 2 + EditorStyles.toolbarButton.fixedHeight * 2 + PPP * 2;
                        Root.p[ 0 ].INTERNAL_BOX( seg );
                        var _R_ = Shrink(seg, PPP);

                        //var ox = t.x;
                        ////////////////////////////////////////////////////////
                        _R_.width = (seg.width - PPP * 2) / 3;
                        _R_.height = 20;
                        var new_hasColorText = HighLighterStyle.TOGGLE_LEFT(_R_, "Label:", filter.hasColorText);

                        if ( new_hasColorText != filter.hasColorText )
                        {

                            if ( new_hasColorText && !filter.AS_TEMPCOLOR_ALIGN_ONLY.HAS_LABEL_COLOR )
                            {
                                var getcolor = filter.AS_TEMPCOLOR_ALIGN_ONLY;
                                TempColorClass.CopyFromTo( CopyType.LABEL, from: HighLighterCommonData.GetLastTempColor(), to: ref getcolor );
                                filter.AS_TEMPCOLOR_ALIGN_ONLY = getcolor;
                                thri = true;
                            }

                            // SaveFilters( filter );
                        }

                        var new_colorText = filter.colorText;
                        _R_.x += _R_.width;
                        _R_.height = 23;
                        var hasC = false;

                        if ( new_hasColorText )
                        {
                            emptyContent.tooltip = "Label Color";
                            RESTORE_GUI();
                            new_colorText = PICKER( _R_, emptyContent, new_colorText );
                            CHANGE_GUI();
                            hasC = true;
                        }

                        else
                        {
                            Label( new Rect( _R_.x, _R_.y, 55, _R_.height ), "-" );
                        }

                        _R_.x += _R_.width;
                        LABEL_DrawOtherButton( _R_, filter, new_hasColorText );
                        ////////////////////////////////////////////////////////
                        //  t.x += t.width;


                        ////////////////////////////////////////////////////////
                        _R_.y += _R_.height;
                        _R_.x = seg.x + PPP;
                        _R_.height = 20;
                        var new_hasColorBg = HighLighterStyle.TOGGLE_LEFT(_R_, "BG   :", filter.hasColorBg);

                        if ( new_hasColorBg != filter.hasColorBg )
                        {
                            if ( new_hasColorBg && !filter.AS_TEMPCOLOR_ALIGN_ONLY.HAS_BG_COLOR )
                            {
                                var getcolor = filter.AS_TEMPCOLOR_ALIGN_ONLY;
                                TempColorClass.CopyFromTo( CopyType.BG, from: HighLighterCommonData.GetLastTempColor(), to: ref getcolor );
                                // Hierarchy_GUI.Undo( adapter , "Change Color Filters" );
                                filter.AS_TEMPCOLOR_ALIGN_ONLY = getcolor;
                                thri = true;
                                // SaveFilters( filter );
                            }
                        }

                        var new_colorBg = filter.colorBg;
                        _R_.x += _R_.width;
                        _R_.height = 23;

                        if ( new_hasColorBg )
                        {
                            emptyContent.tooltip = "BG Color";
                            RESTORE_GUI();
                            new_colorBg = PICKER( _R_, emptyContent, new_colorBg );
                            CHANGE_GUI();
                            hasC = true;
                        }

                        else
                        {
                            Label( new Rect( _R_.x, _R_.y, 55, _R_.height ), "-" );
                        }

                        _R_.x += _R_.width;
                        _R_.width = (seg.width - PPP * 2) - _R_.x;
                        BACKGROUND_DrawOtherButton( _R_, filter, new_hasColorBg );
                        ////////////////////////////////////////////////////////



                        ////////////////////////////////////////////////////////
                        _R_.y += _R_.height;
                        _R_.height = EditorStyles.toolbarButton.fixedHeight;
                        _R_.x = seg.x + PPP;
                        _R_.width = seg.width - PPP * 2;
                        BACKGROUND_DrawAlignLine( _R_, filter );

                        _R_.y += _R_.height;
                        _R_.x = seg.x + PPP;
                        // t.width /= 2;
                        // t.width = (FULL_W - PPP * 2) * PARTS[1];

                        GUI.enabled = hasC;
                        string[] cats = new[] { "This", "Child" };
                        var nv = GUI.Toolbar(_R_, filter.child ? 1 : 0, cats, EditorStyles.toolbarButton) == 1;

                        if ( filter.child != nv )
                        {
                            HighLighterCommonData.Undo( "Change Color Filters" );
                            filter.child = nv;
                            SaveFilters( filter );
                            thri = true;
                        }

                        // var new_child = EditorGUI.ToggleLeft(_R_, "Apply to Child", filter.child);
                        GUI.enabled = true;
                        ////////////////////////////////////////////////////////



                        var ox = seg.x;
                        seg.x += seg.width + PPP;
                        seg.width = FULL_W - (seg.x - ox);

                        Root.p[ 0 ].INTERNAL_BOX( seg );

                        _R_ = Shrink( seg, PPP );
                        _R_.height = FilterLineHeight;

                        HighLighterStyle.LABEL( _R_, "Icon:" );
                        var IR = _R_;
                        IR.x += seg.width * 0.33f;
                        IR.height = IR.width = seg.width * 0.33f;
                        _R_.y += seg.width * 0.33f;

                        // "BG Aligment:"
                        /* EditorGUI.Popup(t, 0,  new[] { "L:FULL - R:FULL", "L:Icon - R:FULL", "L:Label - R:FULL", "L:FULL - R:Half", "L:Icon - R:Half", "L:Label - R:Half"});
						 if (Event.current.type == EventType.Repaint)
						 {   Label(t, new GUIContent() {tooltip = "Background Align\nL-Left Align\nR-Right Align"});
						 }*/
                        // _R_.y -= _R_.height;
                        //  _R_.width = FULL_W * PARTS[3];
                        RESTORE_GUI();
                        Texture2D newicon = filter._icon;

                        try
                        {
                            //var asdasd = GUI.color;

                            //if (filter.hasColorIcon && newicon) GUI.color *= new Color(filter.colorIcon.r, filter.colorIcon.g, filter.colorIcon.b, 0);

                            //	newicon = EditorGUI.ObjectField(new Rect(_R_.x, _R_.y, _R_.height * 2, _R_.height * 2), newicon, typeof(Texture2D), false) as Texture2D;
                            newicon = EditorGUI.ObjectField( IR, newicon, typeof( Texture2D ), false ) as Texture2D;
                            //if (Event.current.type != EventType.Repaint) newicon = EditorGUI.ObjectField(IR, newicon, typeof(Texture2D), false) as Texture2D;
                            //else
                            //{
                            //	var r = IR;
                            //	var S = 1;
                            //	r.x += S;
                            //	r.y += S;
                            //	r.width -= 2 * S;
                            //	r.height -= 2 * S;
                            //	var asdasd = GUI.color;
                            //	if (filter.hasColorIcon && newicon) GUI.color = asdasd * new Color(filter.colorIcon.r, filter.colorIcon.g, filter.colorIcon.b, 1);
                            //	if (newicon) GUI.DrawTexture(r, newicon);
                            //	GUI.color = asdasd;
                            //	GUI.contentColor = Color.clear; 
                            //	EditorGUI.ObjectField(IR, null, typeof(Texture2D), false);
                            //	GUI.contentColor = Color.white;
                            //}


                            //GUI.color = asdasd;
                        }

                        catch
                        {
                            newicon = filter._icon;
                        }

                        CHANGE_GUI();


                        // _R_.y += _R_.height * 2 ;
                        _R_.height = 20;
                        //   _R_.width = FULL_W * PARTS[4] * 1.15f;
                        var fon = GUI.enabled;
                        GUI.enabled = newicon;
                        var new_hasColorIcon = HighLighterStyle.TOGGLE_LEFT(_R_, "Icon Color:", filter.hasColorIcon);
                        if ( new_hasColorIcon != filter.hasColorIcon )
                        {
                            if ( filter.colorIcon == Color.clear ) filter.colorIcon = Color.white;
                        }
                        var new_colorIcon = filter.colorIcon;
                        _R_.y += _R_.height;
                        _R_.height = 23;

                        if ( new_hasColorIcon )
                        {
                            emptyContent.tooltip = "Icon Color";
                            RESTORE_GUI();
                            new_colorIcon = PICKER( _R_, emptyContent, new_colorIcon );
                            CHANGE_GUI();
                        }

                        else
                        {
                            Label( _R_, "-" );
                        }

                        _R_.x += _R_.width;
                        GUI.enabled = fon;


                        var first = newicon != filter._icon
                                    || filter.colorIcon != new_colorIcon
                                    || filter.hasColorIcon != new_hasColorIcon
                                    || filter.colorBg != new_colorBg || new_LayerFilter != filter.LayerFilter
                                    || filter.hasColorBg != new_hasColorBg
                                    || filter.colorText != new_colorText
                                    || filter.hasColorText != new_hasColorText
                                    || filter.ComponentFilter != new_ComponentFilter
                                    || filter.TagFilter != new_TagFilter
                                    || filter.NameFilter != new_NameFilter;
                        var second = filter.NAME != new_SlotName;

                        if ( first || second || thri )
                        {
                            HighLighterCommonData.Undo( "Change Color Filters" );
                            filter._icon = newicon;

                            filter.colorIcon = new_colorIcon;
                            filter.hasColorIcon = new_hasColorIcon;
                            filter.colorBg = new_colorBg;
                            filter.hasColorBg = new_hasColorBg;
                            filter.colorText = new_colorText;
                            filter.hasColorText = new_hasColorText;

                            filter.ComponentFilter = new_ComponentFilter;
                            filter.TagFilter = new_TagFilter;
                            filter.NameFilter = new_NameFilter;
                            filter.LayerFilter = new_LayerFilter;
                            filter.NAME = new_SlotName;

                            SaveFilters( first || thri ? filter : null );
                            //SaveFilters(filter);

                        }


                    }
                }
            }


        }
    }
}