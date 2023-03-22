using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using System.Linq;

namespace EMX.HierarchyPlugin.Editor
{


    internal partial class PluginInstance
    {

        internal float deltaTime = 0;
        internal double lastTime = 0;
        internal bool[] _SKIP_PREFAB_ESCAPE = new bool[2];
        internal bool SKIP_PREFAB_ESCAPE {
            get { return _SKIP_PREFAB_ESCAPE[pluginID]; }
            set { _SKIP_PREFAB_ESCAPE[pluginID] = value; }
        }
        internal bool wereUndoForLockerMod = false;
        internal bool need_ClearHierarchyObjects1 = false;

        internal void RepaintWindowInUpdate_PlusResetStack(int pluginId)
        {
            _RepaintWindowInUpdate_PlusResetStack[pluginId] = true;
            RepaintWindowInUpdate(pluginId);
        }
        internal void RepaintWindowInUpdate(int pluginId)
        {
            _RepaintAfterUpadate[pluginId] = true;
        }
        bool[] _RepaintWindowInUpdate_PlusResetStack = new bool[2];
        bool[] _RepaintAfterUpadate = new bool[2];

        internal bool try_to_detect_scene_changing()
        {

            if (!Application.isPlaying)
            {
                if (EditorSceneManager.sceneCount != LastActiveScenesList_IsValis.Length) Array.Resize(ref LastActiveScenesList_IsValis, EditorSceneManager.sceneCount);
                if (EditorSceneManager.sceneCount != LastActiveScenesList_HashCode.Length) Array.Resize(ref LastActiveScenesList_HashCode, EditorSceneManager.sceneCount);
                if (EditorSceneManager.sceneCount != LastActiveScenesList_Guids.Length) Array.Resize(ref LastActiveScenesList_Guids, EditorSceneManager.sceneCount);
                bool sceneChanged = false;
                var ac = EditorSceneManager.GetActiveScene();
                for (int i = 0; i < LastActiveScenesList_HashCode.Length; i++)
                {
                    if (LastActiveScenesList_HashCode[i] != (EditorSceneManager.GetSceneAt(i).GetHashCode() ^ EditorSceneManager.GetSceneAt(i).path.GetHashCode()))
                    {
                        sceneChanged = true;
                        LastActiveScenesList_HashCode[i] = (EditorSceneManager.GetSceneAt(i).GetHashCode() ^ EditorSceneManager.GetSceneAt(i).path.GetHashCode());
                        var p = EditorSceneManager.GetSceneAt(i).path;
                        if (p != null && p != "") p = AssetDatabase.AssetPathToGUID(p);
                        LastActiveScenesList_Guids[i] = p;
                    }
                }

                for (int i = 0; i < LastActiveScenesList_IsValis.Length; i++)
                {
                    if (LastActiveScenesList_IsValis[i] != EditorSceneManager.GetSceneAt(i).IsValid() && EditorSceneManager.GetSceneAt(i).isLoaded)
                    {
                        sceneChanged = true;
                        LastActiveScenesList_IsValis[i] = EditorSceneManager.GetSceneAt(i).IsValid() && EditorSceneManager.GetSceneAt(i).isLoaded;
                    }
                }

                if (sceneChanged)
                {
                  // int activeIndex = 0;
                  // for (int i = 0; i < LastActiveScenesList_HashCode.Length; i++)
                  // {
                  //     if (EditorSceneManager.GetSceneAt(i).GetHashCode() == ac.GetHashCode()) activeIndex = i;
                  // }
                  // if (activeIndex != 0)
                  // {
                  //     var a1 = LastActiveScenesList_Guids[activeIndex];
                  //     var a2 = LastActiveScenesList_HashCode[activeIndex];
                  //     var a3 = LastActiveScenesList_IsValis[activeIndex];
                  //     LastActiveScenesList_Guids[activeIndex] = LastActiveScenesList_Guids[0];
                  //     LastActiveScenesList_HashCode[activeIndex] = LastActiveScenesList_HashCode[0];
                  //     LastActiveScenesList_IsValis[activeIndex] = LastActiveScenesList_IsValis[0];
                  //     LastActiveScenesList_Guids[0] = a1;
                  //     LastActiveScenesList_HashCode[0] = a2;
                  //     LastActiveScenesList_IsValis[0] = a3;
                  // }
                    return true;
                }
            }
            return false;
        }

        int frame = 0;
        internal void Update()
        {
            if (Root.TemperaryPluginDisabled) return;
            frame++;
            if (frame > 3)
            {
                foreach (var item in PluginInstance.allWindowsData)
                {
                    if (!item.Value.w.Instance || item.Value.pid != 0) continue;
                    for (int i = 0; i < item.Value.w.drew_mods_count; i++)
                    {
                        if (item.Value.w.drew_mods_objects[i].o == null) continue;
                        if (!item.Value.w.drew_mods_objects[i].o.go) continue;
                        if (item.Value.w.drew_mods_objects[i].o.CheckComponents())
                        {
                            //modsController.RESET_DRAW_STACKS();
                            RepaintWindowInUpdate_PlusResetStack(0);
                        }
                    }
                }
                frame = 0;
            }



            for (int i = 0; i < 2; i++)
            {
                if (_RepaintAfterUpadate[i]) //Hierarchy.RepaintAllViews();
                {
                    if (_RepaintWindowInUpdate_PlusResetStack[i])
                    {
                        _drawStack_reset_stacks(i);
                    }
                    RepaintWindow(i, true);
                    _RepaintAfterUpadate[i] = false;
                    if (_RepaintWindowInUpdate_PlusResetStack[i])
                    {
                        allWindowsData.Clear();
                        _RepaintWindowInUpdate_PlusResetStack[i] = false;
                    }
                }

                if (_oneShotUpdate[i].Count != 0)
                {
                    var ex = _oneShotUpdate[i].ToArray();
                    _oneShotUpdate[i].Clear();
                    foreach (var item in ex) item();
                }
            }


            if (try_to_detect_scene_changing())
                invoke_SceneChanging();

            if (wasSceneMoved)
                invoke_SceneMoved();

            if (SKIP_PREFAB_ESCAPE) SKIP_PREFAB_ESCAPE = false;
            if (wereUndoForLockerMod) wereUndoForLockerMod = false;
            _deltatime();
            _clean_updater();


            if (_OnUpdate != null) _OnUpdate();

            if (UNDO.wasUndo) UNDO.wasUndo = false;

        }
        static bool compileFlag {
            get {
                return SessionState.GetBool(Folders.PREFS_PATH + "/CompilationFlag", false);
            }
            set {
                SessionState.SetBool(Folders.PREFS_PATH + "/CompilationFlag", value);
            }
        }




        //[InitializeOnLoadMethod]
        void CompilationTimer()
        {
            if (par_e.TRACKING_COMPILE_TIME)
            {
                if (EditorApplication.isCompiling && !compileFlag)
                {
                    compileFlag = true;
                    SessionState.SetFloat(Folders.PREFS_PATH + "/CompilationTime1", (float)(EditorApplication.timeSinceStartup % 1000));
                    SessionState.SetFloat(Folders.PREFS_PATH + "/CompilationTime2", (float)((EditorApplication.timeSinceStartup + 500) % 1000));
                }
                else if (!EditorApplication.isCompiling && compileFlag)
                {
                    compileFlag = false;
                    if (!EditorUtility.scriptCompilationFailed)
                    {
                        var f1 = SessionState.GetFloat(Folders.PREFS_PATH + "/CompilationTime1", 0);
                        var f2 = SessionState.GetFloat(Folders.PREFS_PATH + "/CompilationTime2", 0);
                        var t1 = (float)(EditorApplication.timeSinceStartup % 1000);
                        var t2 = (float)((EditorApplication.timeSinceStartup + 500) % 1000);
                        float compileTime;
                        if (t1 > f1) compileTime = t1 - f1;
                        else compileTime = t2 - f2;
                        Debug.Log("Compilation: " + compileTime.ToString("F2") + "s");
                    }
                }
            }
        }




        void _deltatime()
        {
            if (lastTime == 0) lastTime = EditorApplication.timeSinceStartup;
            deltaTime = (float)(EditorApplication.timeSinceStartup - lastTime);
            lastTime = EditorApplication.timeSinceStartup;
            if (deltaTime > 0.4f) deltaTime = 0.4f;
        }
        void _clean_updater()
        {
            /*    if ( NEW_PERFOMANCE ) return;
                if ( need_ClearHierarchyObjects1) // lock (m_Handle)
            {
                    need_ClearHierarchyObjects1 = false;
                    if ( !EditorApplication.isCompiling )
                    {
                        ClearHierarchyObjects( true );
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    }
                }*/
        }

        internal void RepaintAllViews()
        {
            _drawStack_reset_stacks(0);
            _drawStack_reset_stacks(1);
            InternalEditorUtility.RepaintAllViews();
        }
        internal void RepaintWindow(int pid, bool force = false)
        {
            if (!force)
                if (EVENT != null)
                {
                    RepaintWindowInUpdate(pid);
                    return;
                }

            _RepaintAfterUpadate[pid] = false;

            foreach (var w in PluginInstance.WindowsData(pid))
            {
                if (!w.Value.w.Instance) continue;
                w.Value.w.Instance.Repaint();
            }
#if !EMX_H_LITE
            foreach (var e in modsController.activeExternalMods.ToList())
            {
                if (!e.Alive() || e is Mods.BookProject.BookmarksforProjectviewModWindow) continue;
                e.RepaintNow();
            }
#endif
        }

        internal void RepaintExternalNow()
        {
#if !EMX_H_LITE
            foreach (var e in modsController.activeExternalMods.ToList())
            {
                if (!e.Alive()) continue;
                e.RepaintNow();
            }
#endif
        }

    }
}
