using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace EMX.HierarchyPlugin.Editor.Mods
{

	class ScenesHistoryModInstance
    {

        /*
					if (INT32_ACTIVE(newIds) && !SkipRemoveFix)
					{
						LastActiveScene = INT32_SCENE(newIds);
						if (adapter.MOI.des(LastActiveScene).GetHash3().Count == 0)
							adapter.MOI.des(LastActiveScene).GetHash3().Add(newIds);
						else adapter.MOI.des(LastActiveScene).GetHash3().Insert(0, newIds);
		LastIndex = 0;
					}

					if (adapter.MOI.des(LastActiveScene).GetHash3().Count > adapter.MAX20)
					{
						var scene = LastActiveScene;
						while (adapter.MOI.des(scene).GetHash3().Count > adapter.MAX20)
							adapter.MOI.des(scene).GetHash3().RemoveAt(adapter.MAX20);
}
*/

        static MemType cType = MemType.Scenes;
        PluginInstance adapter { get { return Root.p[ 0 ]; } }
        internal DrawButtonsOld dob = new DrawButtonsOld();



        internal void SubscribeEditorInstance( AdditionalSubscriber sbs )
        {
            // sbs.OnSceneOpening -= __AddSceneButton_OnSceneChanging;
            //sbs.OnSceneOpening += __AddSceneButton_OnSceneChanging;
            //sbs.ExternalMod_MenuItems
        }

		internal static void SubscribeEditorInstanceStaticOnSceneChaging( EditorSubscriber sbs )
		{
			// sbs.OnSceneOpening -= __AddSceneButton_OnSceneChanging;
			sbs.OnSceneOpening += __AddSceneButton_OnSceneChanging;
			//sbs.ExternalMod_MenuItems
		}
		static void __AddSceneButton_OnSceneChanging()
        {
            AddSceneButton_OnSceneChanging( false );
        }
        //	PluginInstance adapter { get { return Root.p[0]; } }
        static string[] lastScenesSelected = new string[0];
        internal static void AddSceneButton_OnSceneChanging( bool skipPlay = false )
        {
            if ( Application.isPlaying && !skipPlay ) return;


            //	if (_scene == -1) _scene = SceneManager.GetActiveScene().GetHashCode();
            //	var GUID = AssetDatabase.AssetPathToGUID(Adapter.GET_SCENE_BY_ID(_scene).path);
            Root.p[ 0 ].try_to_detect_scene_changing();

            if ( PluginInstance.LastActiveScenesList_Guids.Length == 0 ) return;


            bool skip = true;
            if ( lastScenesSelected.Length != PluginInstance.LastActiveScenesList_Guids.Length )
            {
                skip = false;
                Array.Resize( ref lastScenesSelected, PluginInstance.LastActiveScenesList_Guids.Length );
            }
            for ( int i = 0; i < lastScenesSelected.Length; i++ )
            {
                if ( lastScenesSelected[ i ] != PluginInstance.LastActiveScenesList_Guids[ i ] )
                {
                    skip = false;
                    lastScenesSelected[ i ] = PluginInstance.LastActiveScenesList_Guids[ i ];
                }
            }

            if ( skip ) return;

            //	if (!string.IsNullOrEmpty(GUID))
            {
                //	var newScene = new SceneId(GUID, (additiona_scenes ?? new Scene[0]).Select(s => AssetDatabase.AssetPathToGUID(s.path)).Where(guid => !string.IsNullOrEmpty(guid)).ToArray());
                var newScene = new EMX.HierarchyPlugin.Editor.Mods.DrawButtonsOld.SceneMemory(new ScenesTab_Saved()
                {
                    guid = PluginInstance.LastActiveScenesList_Guids,
                    path = PluginInstance.LastActiveScenesList_Guids.Select(AssetDatabase.GUIDToAssetPath).ToArray(),
                    pin = false
                }, -1);
                var lastScenes = DrawButtonsOld.GET_OBJECTS_LIST(cType, null, EditorSceneManager.GetActiveScene());
                var row_param = DrawButtonsOld.GET_DISPLAY_PARAMS(cType);

                var pinned = lastScenes.Take(row_param.MaxItems).Select((s, i) => new { scene = s, index = i })
                .Where(r => r.scene.pin)
                .OrderBy(r => r.index)
                .ToArray();

                var haveScene = pinned.FirstOrDefault(s => s.scene.additional_GUID[0] == newScene.additional_GUID[0] && s.scene.unique_id == newScene.unique_id);
                if ( haveScene != null && haveScene.scene != null && haveScene.scene.pin ) return;

                var max = pinned.Length == 0 ? -1 : pinned.Max(p => p.index);
                for ( int i = max; i >= 0; i-- )
                {
                    if ( lastScenes[ i ].pin ) lastScenes.RemoveAt( i );
                }
                //lastScenes.RemoveAll(s => s.pin);

                lastScenes.RemoveAll( s => s.additional_GUID[ 0 ] == newScene.additional_GUID[ 0 ] && s.unique_id == newScene.unique_id );

                if ( lastScenes.Count == 0 ) lastScenes.Add( newScene );
                else lastScenes.Insert( 0, newScene );

                for ( int i = 0; i < pinned.Length; i++ )
                {
                    if ( lastScenes.Count == pinned[ i ].index ) lastScenes.Add( pinned[ i ].scene );
                    else lastScenes.Insert( pinned[ i ].index, pinned[ i ].scene );
                }
                while ( lastScenes.Count > 20 ) lastScenes.RemoveAt( 20 );

                DrawButtonsOld.SET_OBJECTS_LIST( lastScenes, cType, null, EditorSceneManager.GetActiveScene(), skipUndo: true );
            }



        }








        internal void DoScenes( Rect line, ExternalDrawContainer controller, Scene scene )
        {
            var l = line;

            l.width -= ExternalModStyles.LINE_HEIGHT_FOR_BUTTONS( line.height );
            var plus = l;
            plus.x += plus.width - 2;
            plus.width = ExternalModStyles.LINE_HEIGHT_FOR_BUTTONS( line.height ) + 2;
            if ( Event.current.type == EventType.MouseDown && plus.Contains( Event.current.mousePosition ) )
            {
                var capturedPlus = plus;
                controller.selection_button = 12;
                controller.selection_window = controller.tempRoot;
                controller.selection_action = ( mouseUp, deltaTIme ) => {
                    if ( mouseUp && capturedPlus.Contains( Event.current.mousePosition ) )
                    {
                        DrawButtonsOld.Add_Scenes( controller, scene );
                    }

                    return Event.current.delta.x == 0 && Event.current.delta.x == 0;
                }; // ACTION
            }

            // plus.height -= 3;
            if ( Event.current.type == EventType.Repaint )
            {
                adapter.STYLE_HIERSEL_BUTTON.Draw( plus, ExternalModStyles.plusContent, false, false, false,
                    plus.Contains( Event.current.mousePosition ) && controller.selection_button == 12 );
                /*  Adapter.GET_SKIN().button.Draw( plus , plusContent , plus.Contains( Event.current.mousePosition ) , false , false ,
                                              plus.Contains( Event.current.mousePosition ) && controller.selection_button == 12 );*/
            }

            GUI.Label( plus, ExternalModStyles.plusContentSceneLabel );
            EditorGUIUtility.AddCursorRect( plus, MouseCursor.Link );


            /* l.width -= LINE_HEIGHT( controller.IS_MAIN ) - 2;
             l.x += LINE_HEIGHT( controller.IS_MAIN ) - 2;*/


            //refStyle = EditorStyles.toolbarButton;
            //refColor = Color.white;
            //   wasSceneDraw = false;
            if ( Event.current.type == EventType.Repaint )
                EditorStyles.helpBox.Draw( l, /* new GUIContent(""),*/ false, false, false, false );
            // line.x = 0;
            /*if (! DrawButtons(l, LH, cType, WHITE, controller, scene))
			{
				var tooltip = GETTOOLTIPPEDCONTENT(cType, null, controller);
				tooltip.text = "";
				Label(l, tooltip);
			}*/

            line.width -= plus.width;

            var rowParams = DrawButtonsOld.GET_DISPLAY_PARAMS(cType, controller);

            //if ( !dob.DrawButtons( line, cType, rowParams.BgOverrideColor.a != 0 ? rowParams.BgOverrideColor : Color.white, controller, scene ) )
				if ( !dob.DrawButtons( line, cType, rowParams.BgOverrideColor, controller, scene ) )
            {
                var tooltip = DrawButtonsOld.GETTOOLTIPPEDCONTENT(cType, null, controller);
                tooltip.text = "";
                GUI.Label( line, tooltip );
                GUI.Label( l, "- - -", adapter.STYLE_LABEL_10_middle );
            }
        }





    }
}
