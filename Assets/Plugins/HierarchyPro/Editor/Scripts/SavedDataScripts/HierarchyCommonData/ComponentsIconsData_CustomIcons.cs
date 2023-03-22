using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using static EMX.HierarchyPlugin.Editor.HierarchyCommonData;

namespace EMX.HierarchyPlugin.Editor
{

	internal enum MemType
    {
        Custom = 2000, Scenes = 1, Last = 2, Hier = 3
    }
    partial class HierarchyCommonData : ScriptableObject
    {



        [Serializable]
        internal class CustromIconData
        {
            [SerializeField]
            internal string texture = null;
            [SerializeField]
            internal Color color = Color.white;
        }

        [SerializeField]
        List<string> HasCustomIconKeys = new List<string>();
        [SerializeField]
        List<CustromIconData> HidedComponentsIconsValues = new List<CustromIconData>();
        internal Dictionary<string, CustromIconData> _HasCustomIcon;


        internal List<string> GetCustomIconsLeyslist() { return HasCustomIconKeys; }
        internal List<CustromIconData> GetCustomIconsValuesList() { return HidedComponentsIconsValues; }

        internal void PutCustomIconsLeyslist( List<string> v )
        {
            SetUndo( "Apply Custom Icon Settings" );
            //Undo.RecordObject(this, "SetHasCustomIcon" );
            _HasCustomIcon = null;
            //if ( !Application.isPlaying ) EditorUtility.SetDirty( this );
            SetDirty();
        }


        internal bool HasCustomIcon( Component comp )
        {
            if ( _HasCustomIcon == null )
            {
                // for ( int i = 0, L = Math.Min( HasCustomIconKeys.Count, HasCustomIconValues.Count ) ; i < L ; i++ )
                _HasCustomIcon = new Dictionary<string, CustromIconData>();
                if ( HidedComponentsIconsValues.Count != HasCustomIconKeys.Count )
                {
                    while ( HidedComponentsIconsValues.Count > HasCustomIconKeys.Count ) HidedComponentsIconsValues.RemoveAt( HasCustomIconKeys.Count );
                    while ( HidedComponentsIconsValues.Count < HasCustomIconKeys.Count ) HasCustomIconKeys.RemoveAt( HasCustomIconKeys.Count );
                }

                for ( int i = 0, L = HasCustomIconKeys.Count; i < L; i++ )
                {

                    var path = AssetDatabase.GUIDToAssetPath(HasCustomIconKeys[i]);
                    if ( string.IsNullOrEmpty( path ) ) continue;
                    var mono = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                    if ( !mono ) continue;
                    if ( !_HasCustomIcon.ContainsKey( mono.GetClass().FullName ) )
                    {
                        _HasCustomIcon.Add( mono.GetClass().FullName, HidedComponentsIconsValues[ i ] );
                    }
                }
            }
            if ( !_HasCustomIcon.ContainsKey( comp.GetType().FullName ) ) return false;
            return true;
        }
        internal void SetHasCustomIcon( Component comp, CustromIconData value )
        {
            SetUndo( "Apply Custom Icon Settings" );
            //Undo.RecordObject( this, "SetHasCustomIcon" );
            if ( value != null )
            {
                if ( _HasCustomIcon != null && _HasCustomIcon.ContainsKey( comp.GetType().FullName ) ) return;
                if ( _HasCustomIcon == null ) _HasCustomIcon = new Dictionary<string, CustromIconData>();
                _HasCustomIcon.Add( comp.GetType().FullName, value );
                var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(comp as MonoBehaviour)));
                HasCustomIconKeys.Add( guid );
                HidedComponentsIconsValues.Add( value );
            }
            else
            {
                if ( _HasCustomIcon == null ) _HasCustomIcon = new Dictionary<string, CustromIconData>();
                _HasCustomIcon.Remove( comp.GetType().FullName );
                var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(comp as MonoBehaviour)));
                while ( true )
                {
                    var i = HasCustomIconKeys.IndexOf(guid);
                    if ( i == -1 ) break;
                    HasCustomIconKeys.RemoveAt( i );
                    if ( i < HidedComponentsIconsValues.Count ) HidedComponentsIconsValues.RemoveAt( i );
                }
            }
            SetDirty();
            //if (!Application.isPlaying) EditorUtility.SetDirty(this);
        }
    }











    internal class DrawCustomIconsClassOld
    {

        internal static float IC_H = EditorGUIUtility.singleLineHeight * 2 + 6;
        //   internal const float IC_H = 36;
        internal PluginInstance A;
        void Undo( string s )
        {
            HierarchyCommonData.Instance().SetUndo( "Apply Custom Icon Settings" );
            //UnityEditor.Undo.RecordObject(HierarchyCommonData.Instance(), s );
        }
        void SetDirty()
        {
            HierarchyCommonData.Instance().SetDirty();
            //if ( !Application.isPlaying ) EditorUtility.SetDirty( HierarchyCommonData.Instance() );
        }
        internal List<string> customIcons {
            get { return HierarchyCommonData.Instance().GetCustomIconsLeyslist(); }
            set {
                HierarchyCommonData.Instance().PutCustomIconsLeyslist( value );
            }
        }
        List<CustromIconData> customIconsValues { get { return HierarchyCommonData.Instance().GetCustomIconsValuesList(); } }


        public float CusomIconsHeight {
            get { return customIcons.Count * IC_H + IC_H; }
        }

        /*   public DoubleList<string, Hierarchy_GUI.CustomIconParams> customIcons
           {
               get { return Hierarchy_GUI.Get(A); }
           }*/


        public void Updater( EditorWindow win )
        {
            if ( Event.current.type == EventType.Repaint && currentY.Length != 0 )
            {
                var tempDragIndex = dragIndex == -1
                    ? -1
                    : Mathf.Clamp(
                        Mathf.RoundToInt((EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition).y - MouseY -
                                          IC_H / 2) / (float)IC_H), 0, currentY.Length - 1);

                for ( int i = 0,
                    sib = 0;
                    i < currentY.Length;
                    i++, sib++ ) // if (tempDragIndex == i && i > dragIndex) sib--;
                { //if (tempDragIndex == i && i < dragIndex) sib++;
                    if ( dragIndex != -1 && i > dragIndex && i <= tempDragIndex ) sib = i - 1;
                    else if ( dragIndex != -1 && i < dragIndex && i >= tempDragIndex ) sib = i + 1;
                    else sib = i;
                    currentY[ i ] = Mathf.Lerp( currentY[ i ], sib * IC_H, 0.5f );
                }

                //print(tempDragIndex);
                if ( dragIndex != -1 ) // Repaint();
                {
                    win.Repaint();
                    //Hierarchy.RepaintAllView();
                }
            }

            if ( Event.current.rawType == EventType.MouseUp )
            {
                var tempDragIndex = dragIndex == -1
                    ? -1
                    : Mathf.Clamp(
                        Mathf.RoundToInt((EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition).y - MouseY -
                                          IC_H / 2) / (float)IC_H), 0, currentY.Length - 1);
                if ( dragIndex != -1 && tempDragIndex != -1 && tempDragIndex != dragIndex )
                {
                    ReplaceFirstToSecond( dragIndex, tempDragIndex );
                    currentY = new float[ 0 ];
                    win.Repaint();
                    A.RepaintWindowInUpdate( 0 );
                    // Hierarchy.RepaintWindow();
                }

                dragIndex = -1;
            }
        }

        public float MouseY = -1;
        float[] currentY = new float[0];
        EditorWindow win;

        public void DrawCustomIcons( EditorWindow win, Rect lr )
        {
            this.win = win;

            A.ChangeGUI();
            int i;
            // content.tooltip = "User Icons";
            // GUILayout.Label("");
            //  var lr = GUILayoutUtility.GetLastRect();

            var XX = 7;
            var YY = 7;

            A.INTERNAL_BOX( new Rect( XX, YY, lr.width - 15, CusomIconsHeight ), PlusContentEmpty );

            MouseY = EditorGUIUtility.GUIToScreenPoint( Vector2.zero ).y;


            if ( currentY.Length != customIcons.Count )
            {
                currentY = new float[ customIcons.Count ];
                for ( i = 0; i < customIcons.Count; i++ )
                    currentY[ i ] = i * IC_H;
            }

            // var lineRect = new Rect(0, 0, W, H);
            for ( i = 0; i < customIcons.Count; i++ )
            {
                // var customIcon = Hierarchy.par.customIcons[i];
                // var r = new Rect(0, currentY[i], lr.width, IC_H);
                var r = new Rect(XX, YY + currentY[i], lr.width, IC_H);

                if ( dragIndex == i )
                {
                    r.x = Event.current.mousePosition.x - IC_H / 2;
                    r.y = Event.current.mousePosition.y - IC_H / 2;
                }

                // GUI.BeginClip(r);
                DrawLine( i, lr, r.x, r.y );
                // GUI.EndClip();
                // lineRect.y += H;
            }


            var lineRect = new Rect(XX, YY + customIcons.Count * IC_H, lr.width, IC_H);
            CustomDragData.ExampleDragDropGUI( A, lineRect, null, CustomDragData.DRAG_VALIDATOR_MONOANDTEXTURE, DRAG_PERFORM_USERICONS );
            /*  if (lineRect.Contains(Event.current.mousePosition))
              {
                  if (Event.current.type.Equals(EventType.Repaint)) GUI.DrawTexture(lineRect,Hierarchy.sec);
              }*/
            lineRect.width -= 15;
            var olds = A.STYLE_DEFBUTTON_middle.fontSize;
            A.STYLE_DEFBUTTON_middle.fontSize = 20;
            var butres = GUI.Button(lineRect, PlusContent, A.STYLE_DEFBUTTON_middle);
            A.STYLE_DEFBUTTON_middle.fontSize = olds;
            if ( butres )
            {
                if ( Event.current.button == 0 ) CreateLine( null, null, int.MaxValue );
            }

            A.RestoreGUI();
        }

        #region CUSTOM ICONS

        void ReplaceFirstToSecond( int i1, int i2 )
        {
            // Hierarchy_GUI.Undo(A, "Change Custom Icons");
            Undo( "Set Custom Icon" );


            // var min = Math.Min(i1, i2);
            // var max = Math.Max(i1, i2);
            var v1 = customIcons[i1];
            var v2 = customIconsValues[i1];
            customIcons.RemoveAt( i1 );
            customIconsValues.RemoveAt( i1 );

            if ( i2 >= customIcons.Count ) customIcons.Add( v1 );
            else customIcons.Insert( i2, v1 );
            if ( i2 >= customIconsValues.Count ) customIconsValues.Add( v2 );
            else customIconsValues.Insert( i2, v2 );

            //  Hierarchy_GUI.SetDirtyObject(A);
            customIcons = customIcons;
            A.RESET_DRAWSTACK( 0 );
            A.RepaintWindowInUpdate( 0 );
            // Hierarchy.RepaintAllView();
            SetDirty();
        }

        void RemoveLine( int index )
        {
            if ( index < 0 || index >= customIcons.Count ) return;
            //    Hierarchy_GUI.Undo(A, "Change Custom Icons");

            Undo( "Remove Custom Icon" );

            customIcons.RemoveAt( index );
            customIconsValues.RemoveAt( index );
            //  Hierarchy_GUI.SetDirtyObject(A);
            //  A.RepaintWindowInUpdate(0);
            customIcons = customIcons;
            A.RESET_DRAWSTACK( 0 );
            A.RepaintWindowInUpdate( 0 );
            //  Hierarchy.RepaintAllView();
            SetDirty();
        }

        void CreateLine( MonoScript component, Texture2D icon, int index )
        {
            string key = null;
            Undo( "Create Custom Icon" );

            var value = new CustromIconData();
            if ( component != null ) key = AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( component ) );
            if ( icon != null ) value.texture = AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( icon ) );
            value.color = Color.white;

            if ( index >= customIcons.Count ) customIcons.Add( key );
            else customIcons.Insert( index, key );
            if ( index >= customIconsValues.Count ) customIconsValues.Add( value );
            else customIconsValues.Insert( index, value );


            customIcons = customIcons;
            //   var init = Hierarchy_GUI.Initialize();
            //  Hierarchy_GUI.Undo(A, "Change Custom Icons");

            //  Hierarchy_GUI.SetDirtyObject(A);
            // A.RepaintWindowInUpdate(0);

            A.RESET_DRAWSTACK( 0 );
            A.RepaintWindowInUpdate( 0 );
            // Hierarchy.RepaintAllView();
            SetDirty();
        }


        //  bool dragContent = false;









        GUIContent PlusContent = new GUIContent()
        {
            text = "+",
            tooltip = "Drag MonoBehaviour Script or Texture here"
        };

        GUIContent PlusContentEmpty = new GUIContent()
        {
            text = "",
            tooltip = "Drag MonoBehaviour Script or Texture here"
        };

        int dragIndex = -1;

        void DrawLine( int i, Rect lr, float xOffset, float yOffset ) // ExampleDragDropGUI(lineRect, new CustomDragData() { index = i });
        {
            MonoScript script = null;
            Texture2D icon = null;
            if ( !string.IsNullOrEmpty( customIcons[ i ] ) )
            {
                var scriptPath = AssetDatabase.GUIDToAssetPath(customIcons[i]);
                if ( !string.IsNullOrEmpty( scriptPath ) ) script = AssetDatabase.LoadAssetAtPath<MonoScript>( scriptPath );
            }

            if ( !string.IsNullOrEmpty( customIconsValues[ i ].texture ) )
            {
                var scriptPath = AssetDatabase.GUIDToAssetPath(customIconsValues[i].texture);
                if ( !string.IsNullOrEmpty( scriptPath ) ) icon = AssetDatabase.LoadAssetAtPath<Texture2D>( scriptPath );
            }

            var r = new Rect(0, 0, IC_H/2, IC_H);

            r.x += xOffset;
            r.y += yOffset;

            var oldsl = A.STYLE_LABEL_10_middle_clip.fontSize;
            A.STYLE_LABEL_10_middle_clip.fontSize = (int)EditorGUIUtility.singleLineHeight;
            //GUI.Label(r, "■");
            //  if (GUI.Button(r, "▲"))
            GUI.Label( r, "=", A.STYLE_LABEL_10_middle_clip );
            A.STYLE_LABEL_10_middle_clip.fontSize = oldsl;
            // if (r.Contains( Event.current.mousePosition )) A.RepaintWindow();

            if ( r.Contains( Event.current.mousePosition ) && Event.current.type == EventType.MouseDown )
            {
                if ( Event.current.button == 0 )
                {
                    dragIndex = i;
                    /*InternalEditorUtility.repa*/
                    if ( win ) win.Repaint();
                    A.RepaintWindowInUpdate( 0 );
                }
            }

            EditorGUIUtility.AddCursorRect( r, MouseCursor.Link );
            /* if (dragContent)*/

            // ExampleDragDropGUI(r, new CustomDragData() { index = i });

            A.RestoreGUI();

            // r.Set( r.width + 10 , (r.height - EditorGUIUtility.singleLineHeight) / 2 , lr.width - (IC_H + 10 + IC_H + 10 + IC_H) - IC_H / 3 , EditorGUIUtility.singleLineHeight );
            //  r.Set(r.width + 10, (r.height - EditorGUIUtility.singleLineHeight) / 2, lr.width - (IC_H + 10 + IC_H + 10 + IC_H) - IC_H / 3, EditorGUIUtility.singleLineHeight);
            r.Set( r.width + 10, 3, lr.width - (IC_H + IC_H + IC_H) - IC_H / 3, EditorGUIUtility.singleLineHeight );

            r.x += xOffset;
            r.y += yOffset;


            UnityEngine.Object newScript = script;
            try
            {
                newScript = EditorGUI.ObjectField( r, script, typeof( MonoScript ), false );
            }
            catch
            {
                newScript = script;
            }

            //r.Set(lr.width - IC_H - IC_H, 0, IC_H, IC_H);
            r.Set( r.x + 3, EditorGUIUtility.singleLineHeight + 6, r.width - 3, EditorGUIUtility.singleLineHeight );

            r.y += yOffset;

            // var cRect = r;
            /*cRect.x -= cRect.width - 20;
            cRect.width -= 20;
            cRect.y += 4;
            cRect.height -= 8;*/
            var oldCol = customIconsValues[i].color;
            var newCol = oldCol;
            try
            {
                // newCol = Settings.Draw.COLOR(r, oldCol);
                newCol = Tools.PICKER( r, null, oldCol );
            }
            catch { }

            EditorGUIUtility.AddCursorRect( r, MouseCursor.Link );

            r.Set( r.x + r.width + 10, yOffset, IC_H, IC_H );

            UnityEngine.Object newicon = icon;
            try
            {
                var asdasd = GUI.color;
                GUI.color *= new Color( newCol.r, newCol.g, newCol.b, 1 );
                newicon = EditorGUI.ObjectField( r, icon, typeof( Texture2D ), false );
                GUI.color = asdasd;
            }
            catch
            {
                newicon = icon;
            }

            A.ChangeGUI();

            if ( newScript != script )
            {
                var v = customIcons[i];
                Undo( "Change script for Custom Icon" );
                v = AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( newScript ) );
                customIcons[ i ] = v;
                customIcons = customIcons;
                A.RESET_DRAWSTACK( 0 );
                A.RepaintWindowInUpdate( 0 );
                // Hierarchy.RepaintAllView();
                SetDirty();
            }

            if ( newicon != icon )
            {
                var v = customIconsValues[i];
                Undo( "Change icon for Custom Icon" );
                v.texture = AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( newicon ) );
                customIconsValues[ i ] = v;
                customIcons = customIcons;
                A.RESET_DRAWSTACK( 0 );
                A.RepaintWindowInUpdate( 0 );
                //Hierarchy.RepaintAllView();
                SetDirty();
            }

            if ( oldCol != newCol )
            {
                var v = customIconsValues[i];
                Undo( "Change color for Custom Icon" );
                v.color = newCol;
                customIconsValues[ i ] = v;
                customIcons = customIcons;
                A.RESET_DRAWSTACK( 0 );
                A.RepaintWindowInUpdate( 0 );
                SetDirty();
            }

            r.Set( lr.width - IC_H - 3, 0, IC_H - 10, IC_H );
            r.x += xOffset;
            r.y += yOffset;
            if ( GUI.Button( r, "X", A.button ) )
            {
                if ( Event.current.button == 0 )
                {
                    RemoveLine( i );
                    //CreateLine(null, null, int.MaxValue);
                }

                //dragContent = true;
            }
        }

        #endregion

        internal void DRAG_PERFORM_USERICONS()
        {
            CreateLine( DragAndDrop.objectReferences[ 0 ] as MonoScript, DragAndDrop.objectReferences[ 0 ] as Texture2D, int.MaxValue );
        }

    }




    public class CustomDragData
    {


        public int index = -1;
        /* public int originalIndex;
         public List<object> originalList;*/
        internal static void SetDragData( UnityEngine.Object[] data, MemType? type )
        {
            if ( data != null )
            {
                Root.p[ 0 ].ha.InternalClearDrag();
                DragAndDrop.objectReferences = data.Where(g=>g).ToArray();
                DragAndDrop.SetGenericData( Root.p[ 0 ].pluginname, type );
            }
        }
        internal static bool DRAG_VALIDATOR_MONOANDTEXTURE()
        {
            return DragAndDrop.objectReferences.Length == 1 && (DragAndDrop.objectReferences[ 0 ] is Texture2D || IsMonoScript( DragAndDrop.objectReferences[ 0 ] ));
        }
        internal static bool DRAG_VALIDATOR_ONLYMONO()
        {
            return DragAndDrop.objectReferences.Length == 1 && IsMonoScript( DragAndDrop.objectReferences[ 0 ] );
        }
        internal static bool IsMonoScript( UnityEngine.Object ob )
        {
            return (ob is MonoScript) && ((MonoScript)ob).GetClass() != null;
        }

        internal static void ExampleDragDropGUI( PluginInstance A, Rect dropArea, CustomDragData data, Func<bool> validate, Action perform, Color? color = null ) // Cache References:
        { //  Event currentEvent = Event.current;
            EventType currentEventType = Event.current.type;

            // The DragExited event does not have the same mouse position data as the other events,
            // so it must be checked now:
            if ( currentEventType == EventType.DragExited )
                A.ha.InternalClearDrag();

            if ( !dropArea.Contains( Event.current.mousePosition ) ) return;

            switch ( currentEventType )
            {
                case EventType.MouseDown:
                    if ( data != null ) //dragContent = true;
                    {
                        A.ha.InternalClearDrag();
                        // DragAndDrop.PrepareStartDrag();// reset data

                        /*                    CustomDragData dragData = new CustomDragData();
                                            dragData.originalIndex = somethingYouGotFromYourProperty;
                                            dragData.originalList = this.targetList;

                                            DragAndDrop.SetGenericData(dragDropIdentifier, dragData);*/

                        DragAndDrop.SetGenericData( "HY1", data );
                        //var objectReferences = new[] { property };// Careful, null values cause exceptions in existing editor code.
                        // DragAndDrop.objectReferences = objectReferences;// Note: this object won't be 'get'-able until the next GUI event.

                        Tools.EventUseFast();
                    }


                    break;
                case EventType.MouseDrag:
                    // If drag was started here:
                    CustomDragData existingDragData = DragAndDrop.GetGenericData("HY1") as CustomDragData;

                    if ( existingDragData != null )
                    {
                        DragAndDrop.StartDrag( "Dragging List ELement" );
                        Tools.EventUseFast();
                    }

                    break;
                case EventType.DragUpdated:
                    if ( validate() ) DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    else DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

                    Tools.EventUseFast();
                    break;
                case EventType.Repaint:
                    if (
                        DragAndDrop.visualMode == DragAndDropVisualMode.None ||
                        DragAndDrop.visualMode == DragAndDropVisualMode.Rejected ) break;

                    if ( validate() ) EditorGUI.DrawRect( dropArea, color ?? Color.grey );
                    break;
                case EventType.DragPerform:
                    DragAndDrop.AcceptDrag();
                    if ( data == null )
                    {
                        if ( validate() )
                        {
                            perform();
                        }
                    }
                    /*   CustomDragData receivedDragData = DragAndDrop.GetGenericData(dragDropIdentifier) as CustomDragData;

                       if (receivedDragData != null && receivedDragData.originalList == this.targetList) ReorderObject();
                       else AddDraggedObjectsToList();*/

                    Tools.EventUseFast();
                    break;
                case EventType.MouseUp:
                    // Clean up, in case MouseDrag never occurred:
                    A.ha.InternalClearDrag();
                    //ADragAndDrop.PrepareStartDrag();
                    break;
            }
        }




    }



}
