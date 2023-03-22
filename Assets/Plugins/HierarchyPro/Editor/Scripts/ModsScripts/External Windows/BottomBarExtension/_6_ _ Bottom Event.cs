

        //Vector2 LastTotalSize;
        //internal  Rect? lastBottomRectUI, lastBottomRectSelectLine;


        //internal void BottomEventGUI( Rect selectRect, Adapter.HierarchyObject _o, EditorWindow w )
        //{
        //
        //    if ( !cacheInit ) RefreshMemCache( LastActiveScene.GetHashCode() );
        //    if ( !adapter.ENABLE_BOTTOMDOCK_PROPERTY && HEIGHT == 0  /*|| BrakeBottom*/) return;
        //
        //    /* if ( Event.current.type != EventType.Repaint )
        //         NEW_BOTTOM( selectRect, _o, w );*/
        //    if ( Event.current.type != EventType.Repaint )
        //        OLD_EVENTS( selectRect, w );
        //}



















       // void OLD_EVENTS( Rect selectRect, EditorWindow w )
       // {
       //
       //
       //     var rect = GetNavigatorRect( /*treeView,*/ selectRect.x + selectRect.width);
       //     HierarchyController.ModuleRect = rect;
       //
       //
       //     if ( rect.Contains( Event.current.mousePosition ) )
       //     {
       //         if ( Event.current.type == EventType.MouseDrag || Event.current.type == EventType.DragUpdated ||
       //                 Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragExited || Event.current.type == EventType.Layout )
       //         {
       //             var line = GetLineRect(rect);
       //             GetFoldOutRect( ref line );
       //             DoLines( line, w );
       //         }
       //
       //
       //
       //         if ( Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform ||
       //                 Event.current.type == EventType.DragExited ) EventUse();
       //
       //
       //
       //         if ( Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp )
       //         {
       //             var line = GetLineRect(rect);
       //             var foldRect = GetFoldOutRect(ref line);
       //
       //
       //
       //             DRAW_FOLD_ICONS( ref foldRect, LastActiveScene.GetHashCode() );
       //
       //             if ( anycatsenable )
       //             {
       //                 if ( foldRect.Contains( Event.current.mousePosition ) )
       //                 {
       //                     HierarchyController.selection_window = w;
       //                     HierarchyController.selection_button = 0;
       //                     HierarchyController.selection_action = ( mouseUp, deltaTIme ) => {
       //                         if ( mouseUp && foldRect.Contains( Event.current.mousePosition ) )
       //                         {
       //                             adapter.par.BOTTOM_AUTOHIDE = !adapter.par.BOTTOM_AUTOHIDE;
       //                             adapter.SavePrefs();
       //                         }
       //                         return Event.current.delta.x == 0 && Event.current.delta.x == 0;
       //
       //                     };
       //                 }
       //
       //             }
       //
       //
       //             DoLines( line, w );
       //
       //
       //             GUIUtility.hotControl = 0;
       //             EventUse();
       //             adapter.RepaintWindow( true );
       //         }
       //     }
       //
       //
       //
       // }

       // void OLD_PAINT( Rect selectRect, EditorWindow w )
       // {
       //     if ( UNITY_CURRENT_VERSION >= UNITY_2019_VERSION && Event.current.type == EventType.Repaint )     // FIX LEFT COLUMN
       //     {
       //         if ( EditorGUIUtility.isProSkin )
       //         {
       //             Adapter.DrawRect( new Rect( 0, selectRect.y + selectRect.height, adapter.TOTAL_LEFT_PADDING_FORBOTTOM, mTotalRectGet2.height ), leftFixColorPro );
       //         }
       //         else
       //         {
       //             Adapter.DrawRect( new Rect( 0, selectRect.y + selectRect.height, adapter.TOTAL_LEFT_PADDING_FORBOTTOM, mTotalRectGet2.height ), leftFixColorPersonal );
       //         }
       //     }
       //
       //
       //
       //
       //     var navRect = GetNavigatorRect( /*treeView,*/ selectRect.x + selectRect.width);
       //     var lineRect = GetLineRect(navRect);
       //     var foldRect = GetFoldOutRect(ref lineRect);
       //
       //     lastBottomRectUI = navRect;
       //     lastBottomRectSelectLine = selectRect;
       //
       //     //FadeRect
       //     var defColor = GUI.color;
       //     var c = Adapter.EditorBGColor;
       //     c.a = 1;
       //     GUI.color *= c;
       //     GUI.DrawTexture( navRect, EditorGUIUtility.whiteTexture );
       //     GUI.color = defColor;
       //     //FadeRect
       //
       //
       //     if ( Adapter.UNITY_CURRENT_VERSION < Adapter.UNITY_2019_3_0_VERSION )
       //         Adapter.GET_SKIN().window.Draw( navRect, /*new GUIContent("Navigator"),*/ false, false, false, false );
       //     else
       //         Adapter.GET_SKIN().textArea.Draw( navRect, /*new GUIContent("Navigator"),*/ false, false, false, false );
       //
       //
       //     DRAW_FOLD_ICONS( ref foldRect, LastActiveScene.GetHashCode() );
       //
       //
       //     // if ( !adapter.BOT_DRAW_STACK.START_DRAW_PARTLY_TRYDRAW( o ) ) return;
       //     // if ( Event.current.type == EventType.Repaint ) adapter.BOT_DRAW_STACK.START_DRAW_PARTLY_CREATEINSTANCE( o );
       //     DoLines( lineRect, w );
       //     //adapter.BOT_DRAW_STACK.END_DRAW( o );
       // }