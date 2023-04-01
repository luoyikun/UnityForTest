using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using EMX.HierarchyPlugin.Editor.Mods;

namespace EMX.HierarchyPlugin.Editor
{

    internal partial class PluginInstance
    {



        internal void invoke_unUndo()
        {
            if (Root.TemperaryPluginDisabled) return;

            if (_OnUndoAction != null) _OnUndoAction();
        }

        /*   internal void EditorSceneManagerOnSceneUnloaded( Scene arg0, LoadSceneMode loadSceneMode )
           {
               if ( SubcripeSceneLoader_method != null ) SubcripeSceneLoader_method();
               //ltg.Clear(); NEW_CACHE
               CloseWindows();
           }*/



        internal void invoke_DuringSceneGUI(SceneView sv)
        {
            if (Root.TemperaryPluginDisabled) return;

            if (_DuringSceneGui != null) _DuringSceneGui(sv);
        }
        internal void invoke_ModifiyKeyChanged()
        {
            if (Root.TemperaryPluginDisabled) return;

            if (_OnModifyKeyChanged != null) _OnModifyKeyChanged();

        }

        internal static bool[] LastActiveScenesList_IsValis = new bool[0];
        internal static int[] LastActiveScenesList_HashCode = new int[0];
        internal static string[] LastActiveScenesList_Guids = new string[0];
        /*private int lastSceneID = -1;
		private string lastScenePath = null;
		private string lastSceneGUID = null;*/
        //private string[] lastSceneGUID_ALL = new string[0];
        /*	internal void invoke_EditorSceneManagerOnSceneOpening3()
			{
				__scene();
			}
			internal void invoke_EditorSceneManagerOnSceneOpening2(Scene s, Scene s2)
			{
				__scene();
			}*/

        internal void invoke_EditorSceneManagerOnSceneOpening(Scene scene, OpenSceneMode mode)
        {

            HierarchyTempSceneDataGetter.FullClearObjects(scene);
            if (scene.IsValid() && scene.isLoaded)
            {
                var s = HierarchyTempSceneData.InstanceFast(scene);
                if (Application.isPlaying) UnityEngine.Object.Destroy(s);
                else UnityEngine.Object.DestroyImmediate(s, false);
            }


           // HighlighterCache_Colors.cacheDichighlighter.Clear();
          //  Cache.ClearHierarchyObjects(false);
          //  Root.ClearCacheOnCompile();

            Root.TemperaryPluginDisabled = false;

            invoke_SceneChanging();
            
        }
        internal void invoke_SceneChanging()
        {
            if (_OnSceneOpening != null) _OnSceneOpening();

            need_ClearHierarchyObjects1 = true;

            LastActiveScene = SceneManager.GetActiveScene();
            //Debug.Log("asd");
            /*lastScenePath = null;
			lastSceneID = -1;
			lastSceneGUID = null;*/
            //ltg.Clear(); NEW_CACHE
            windowsManager.CloseWindows();
            gl.ClearStacks();
        }
        internal bool wasSceneMoved = false;
        internal void invoke_SceneMoved()
        {
            if (!wasSceneMoved) return;
            wasSceneMoved = false;
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var s = EditorSceneManager.GetSceneAt(i);
                if (!s.IsValid() || !s.isLoaded) continue;
                HierarchyTempSceneData.SaveOnScenePathChanged(s);
            }
            try_to_detect_scene_changing();
            if (_OnSceneOpening != null) _OnSceneOpening();
        }


        internal Scene _LastActiveScene;
        internal Scene LastActiveScene {
            get {
                if (!_LastActiveScene.IsValid()) _LastActiveScene = SceneManager.GetActiveScene();
                return _LastActiveScene;
            }
            set {
                if (_LastActiveScene != value)
                {
                    _LastActiveScene = value;
#if !EMX_H_LITE
                    modsController.INVOKE_FOR_EXTERNAL<ExternalModRoot>(r => r.RepaintNow());
#endif
                    /*	lastSceneID = -1;
						lastScenePath = null;
						lastSceneGUID = null;*/
                }
            }
        }


        //internal void invoke_onSelectionChanged(int id )
        //{
        //    Debug.Log( id );
        //    invoke_onSelectionChanged();
        //}
        internal bool SkipSwitch = false;
        internal bool SkipRemove = false;

        internal int SelectionID = 0;
        internal void invoke_onSelectionChanged()
        {
            if (Root.TemperaryPluginDisabled) return;

            SelectionID++;
            if (SelectionID > 100000) SelectionID = 0;
            ha._IsSelectedCache.Clear();
            ha._IsDraggedCache.Clear();
            ha.OnSelectionChanged_SaveCache();
            if (_OnSelectionChanged != null) _OnSelectionChanged();
            SkipSwitch = false;
            SkipRemove = false;
        }
        internal void invoke_onHierarchyChanged()
        {
            if (Root.TemperaryPluginDisabled) return;

            invoke_onHierarchyChanged(false);
        }
        internal void invoke_onHierarchyChanged(bool skipClear)
        {
            if (_OnHierarchyChanged != null) _OnHierarchyChanged();
            //  Cache.ClearAdditionalCache();
            if (!skipClear) Cache.ClearHierarchyObjects(false);
        }
        internal void invoke_onPlayModeStateChanged(PlayModeStateChange state)
        {
            Root.TemperaryPluginDisabled = false;

            if (_OnPlayModeStateChanged != null) _OnPlayModeStateChanged();
            // RESET_DRAWSTACK(0);
            //modsController.REBUILD_PLUGINS();
            //indow.Instance.SendEvent( new Event() { type = EventType.Layout } );
            // window.Instance.SendEvent( new Event() { type = EventType.Repaint } );
        }


        internal void invoke_OnProjectWindow(string guid, Rect selectionrect)
        {

            if (!Root.SKIP_TEMP_DIS)
            {
                if (Root.TemperaryPluginDisabled) return;

                try
                {
                    if (_OnProjectWindow != null) _OnProjectWindow(guid, selectionrect);
                }
                catch (Exception ex)
                {
                    Root.TemperaryDisableThePlugin_FromRoot(ex);
                }
            }
            else
            {
                if (_OnProjectWindow != null) _OnProjectWindow(guid, selectionrect);
            }
        }

        internal bool invoke_OnEditorWantsToQuit()
        {
            var res = true;
            foreach (var item in _OnEditorWantsToQuit)
                res &= item();
            return res;
        }

        Action<bool> _OnGlobalKeyPressed = null;
        Action _OnSceneOpening = null;
        Action _OnUndoAction = null;
        Action _OnSelectionChanged = null;
        Action _OnHierarchyChanged = null;
        Action _OnPlayModeStateChanged = null;
        Action _OnUpdate = null;
        Action<string, Rect> _OnProjectWindow = null;
        Action<SceneView> _DuringSceneGui = null;
        Action _OnModifyKeyChanged = null;
        List<Func<bool>> _OnEditorWantsToQuit = new List<Func<bool>>();
        Action _OnAssetImport = null;
        Action _OnResetDrawStack = null;

        internal class actions_pair
        {
            internal Action middle;
            internal Action middle_plusLayout;
        }

        internal class gui_actions
        {
            internal List<Action> BuildedOnGUI_first = new List<Action>();
            internal List<actions_pair> BuildedOnGUI_middle = new List<actions_pair>();

            internal Action<Window> OnAssignWindowFirstFrame = null;

            //internal Action BuildedOnGUI_middle;
            //internal Action BuildedOnGUI_middle_plusLayout;
            internal Action BuildedOnGUI_last;
            internal Action BuildedOnGUI_last_butBeforeGL;
        }
        internal gui_actions[] g = { new gui_actions(), new gui_actions() };


        internal Dictionary<AdditionalSubscriber, List<Action>> _REBUILD_ADDITIONAL_EVENTS = new Dictionary<AdditionalSubscriber, List<Action>>();

        //internal void REBUILD_ADDITIONAL_EVENTS( AdditionalSubscriber sbs )
        //{
        //    if (!_REBUILD_ADDITIONAL_EVENTS.Contains)
        //}

        internal void _rebuild_editor_events(EditorSubscriber sbs)
        {


            _REBUILD_ADDITIONAL_EVENTS = sbs.REBUILD_ADDITIONAL_EVENTS;
            foreach (var item in _REBUILD_ADDITIONAL_EVENTS)
            {
                item.Key.Subscribe(sbs);
            }


#if !EMX_H_LITE
            modsController.toolBarModification.hotButtons.RegistrateButton(sbs);
            if (par_e.USE_RIGHT_CLICK_MENU_MOD && par_e.INCLUDE_RIGHTCLIK_MENU_OPENPLUGINWINDOWS_BUTTONS) RightClickOnGameObjectMenuRegistrator.RegistrateMenuItem(sbs.ExternalMod_MenuItems);
            else
#endif
                RightClickOnGameObjectMenuRegistrator.RegistrateMenuItem(null);


            g[0].BuildedOnGUI_first = sbs.BuildedOnGUI_first;
            g[0].BuildedOnGUI_middle = sbs.BuildedOnGUI_middle;
            // g[ 0 ].BuildedOnGUI_middle_plusLayout = sbs.BuildedOnGUI_middle_plusLayout;
            g[0].BuildedOnGUI_last = sbs.BuildedOnGUI_last;
            g[0].OnAssignWindowFirstFrame = sbs.OnAssignWindowFirstFrame;
            g[0].BuildedOnGUI_last_butBeforeGL = sbs.BuildedOnGUI_last_butBeforeGL;

            _OnSceneOpening = sbs.OnSceneOpening;
            _OnUndoAction = sbs.OnUndoAction;
            _OnUndoAction += new UNDO().UNDO_AC;
            _OnSelectionChanged = sbs.OnSelectionChanged;
            _OnHierarchyChanged = sbs.OnHierarchyChanged;
            _OnPlayModeStateChanged = sbs.OnPlayModeStateChanged;
            _OnUpdate = sbs.OnUpdate;
            _OnGlobalKeyPressed = sbs.OnGlobalKeyPressed;
            _DuringSceneGui = sbs.duringSceneGui;
            _OnModifyKeyChanged = sbs.OnModifyKeyChanged;
            _OnProjectWindow = sbs.OnProjectWindow;
            _OnEditorWantsToQuit = sbs.OnEditorWantsToQuit;
            _OnAssetImport = sbs.OnAssetImport;
            _OnResetDrawStack = sbs.OnResetDrawStack;

            modsController.saveModsInterator = sbs.saveModsInterator;
        }






        internal void invoke_ON_ASSET_IMPORT()
        {

            if (_OnAssetImport != null) _OnAssetImport();
            if (pluginID == 1)
            {
                //need_ClearHierarchyObjects1 = true;
                //ClearTree( true );
            }
            TODO_Tools.ALL_ASSETS_PATHS = null;
        }






        internal void invoke_ReloadAfterAssetDeletingOrPasting() //old = Again_Reloder_UsingWhenCopyPastOrAssets
        {

            invoke_EditorSceneManagerOnSceneOpening(EditorSceneManager.GetActiveScene(), OpenSceneMode.Single);
            RepaintWindowInUpdate_PlusResetStack(0);
            RepaintWindowInUpdate_PlusResetStack(1);
            // if ( Hierarchy.HierarchyAdapterInstance.onDidReloadScript != null ) Hierarchy.HierarchyAdapterInstance.onDidReloadScript();
            // if ( onUndoAction != null ) onUndoAction();

        }

    }


}
