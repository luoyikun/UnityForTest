//#define EXM_SKIP_TEMP_DIS
//#define EMX_USE_FAST_BOTTOM_FIX

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using UnityEditor.SceneManagement;
using EMX.HierarchyPlugin.Editor.Windows;

namespace EMX.HierarchyPlugin.Editor
{
    [InitializeOnLoad]
    public class Root
    {

        internal const string VER =
#if !EMX_H_LITE
            "Pro Extended " +
#endif
            "2021.1 u13";

        public const string PN_SHORT = "Hierarchy";
        public const string HierarchyPro = Folders.ASSET_NAME;
        public const string PN_NS = "HierarchyPlugin";
        public const string CUST_NS = "CustomizationHierarchy";
        public const string PN_FOLDER = Folders.PN_FOLDER;
        static PluginInstance[] __p = null;
        internal static PluginInstance[] p {
            get {
                if (!created || __p == null) CREATE();
                return __p;
            }
        }
        //

#if EXM_SKIP_TEMP_DIS
		internal static bool SKIP_TEMP_DIS = true;
#else
        internal static bool SKIP_TEMP_DIS = false;
#endif
        static bool created = false;
        static bool _TemperaryPluginDisabled = false;
        internal static bool TemperaryPluginDisabled {
            get {
                return _TemperaryPluginDisabled || TemperaryPluginDisabled_ForRoot > 3;
            }
            set {
                _TemperaryPluginDisabled = value;
            }
        }
        internal static int TemperaryPluginDisabled_ForWindows = 0;
        internal static int TemperaryPluginDisabled_ForRoot = 0;

        internal static void TemperaryDisableThePlugin(Exception ex)
        {
            TemperaryPluginDisabled = true;
            foreach (var item in IWindow.__inputWindow)
            {
                if (item.Value) item.Value.CloseThis();
            }
            IWindow.__inputWindow.Clear();
            LogExc(ex);
            EditorGUIUtility.ExitGUI();
        }
        internal static void TemperaryDisableThePlugin_FromCache()
        {
            TemperaryPluginDisabled = true;
            foreach (var item in IWindow.__inputWindow)
            {
                if (item.Value) item.Value.CloseThis();
            }
            IWindow.__inputWindow.Clear();
            LogExc(new Exception("Cache not finalized"));
            EditorGUIUtility.ExitGUI();
            //throw new Exception(  );
        }
        internal static void TemperaryDisableThePlugin_FromWindow(Exception ex)
        {
            TemperaryPluginDisabled_ForWindows++;
            if (TemperaryPluginDisabled_ForWindows > 3)
            {
                Debug.Log("HierarchyPro detect errors in the loop and was temporary disabled");
                EditorApplication.update += _repaintInUpdateAllView;
            }
            foreach (var item in IWindow.__inputWindow)
            {
                if (item.Value) item.Value.CloseThis();
            }
            IWindow.__inputWindow.Clear();
            LogExc(ex);
            EditorGUIUtility.ExitGUI();
        }

        internal static void TemperaryDisableThePlugin_FromRoot(Exception ex)
        {
            TemperaryPluginDisabled_ForRoot++;
            if (TemperaryPluginDisabled_ForRoot > 3)
            {
                Debug.Log("HierarchyPro detect errors in the loop and was temporary disabled");
                EditorApplication.update += _repaintInUpdateAllView;
            }

            foreach (var item in IWindow.__inputWindow)
            {
                if (item.Value) item.Value.CloseThis();
            }
            IWindow.__inputWindow.Clear();
            LogExc(ex);
            EditorGUIUtility.ExitGUI();
        }

        static Dictionary<int, bool> messagePool = new Dictionary<int, bool>();
        static void LogExc(Exception ex)
        {
            EditorApplication.update += _repainAll;
            if (messagePool.ContainsKey(ex.Message.GetHashCode() ^ ex.StackTrace.GetHashCode())) return;
            messagePool.Add(ex.Message.GetHashCode() ^ ex.StackTrace.GetHashCode(), false);
            //ex.Message = ex.Message;
            ex = new Exception("HierarchyPro: " + ex.Message, ex);
            try
            {
                if (Root.p[0].par_e.LOG_SETTINGS_RED_MODE)
                {
                    throwEx.Add(ex);
                }
                else if (Root.p[0].par_e.LOG_SETTINGS_WHITE_MODE) Debug.Log(ex);
            }
            catch (Exception ex2)
            {
                Debug.Log("HierarchyPro: Serious problems with settings of the " + Root.HierarchyPro + ": " + ex2.Message + "\n\n" + ex2.StackTrace + "\n\nRoot problem: " + ex.Message + "\n\n" + ex.StackTrace);
            }
        }
        static List<Exception> throwEx = new List<Exception>();
        static void _repainAll()
        {

            EditorApplication.update -= _repainAll;

            if (throwEx.Count != 0)
            {

                var e = throwEx.Last();
                throwEx.RemoveAt(throwEx.Count - 1);
                if (throwEx.Count != 0) EditorApplication.update += _repainAll;
                else
                    EditorApplication.update += _repaintInUpdateAllView;
                //InternalEditorUtility.RepaintAllViews();

                throw e;
            }

        }

        static void _repaintInUpdateAllView()
        {
            InternalEditorUtility.RepaintAllViews();
            EditorApplication.update -= _repaintInUpdateAllView;
        }

        static Icons _icons = null;
        static internal Icons icons { get { return _icons ?? (_icons = AssetDatabase.LoadAssetAtPath<Icons>(Folders.PluginInternalFolder + "/Editor/Icons/IconsArray.asset") ?? throw new Exception("Cannot load icons at path: " + Folders.PluginInternalFolder + "/Editor/Icons/IconsArray.asset")); } }
        internal static bool hasInit = false;
        internal static bool hasMainConstructorInit = false;
        static Root()
        {
            if (hasMainConstructorInit) return;
            if (CREATE())
            {
                Root.p[0].modsController.REBUILD_PLUGINS();
                hasMainConstructorInit = true;
            }

        }

        static bool CREATE()
        {
            if (created) return false;
            created = true;
            Folders.CheckFolders();
            //Settings.MainSettingsEnabler_Window.CheckSettings();
            __p = new PluginInstance[2];
            __p[0] = PluginInstance.CreateInstance("Hierarchy");
            __p[0].par_e = new EditorSettingsAdapter(0);
            __p[0].gl = new GlDrawer(__p[0]);
            if (!__p[0].par_e.ENABLE_ALL)
            {
                return false;
            }
            Init();
            return true;
            //p[0].par_e
        }
        static void Init()
        {
            if (hasInit) return;

            hasInit = true;
            InitializeRoot();

            var hierarchyPlugin = PluginInstance.CreateInstance("Hierarchy");
            p[0] = hierarchyPlugin;
            // p[ 1 ] = hierarchyPlugin;
            for (int i = 0; i < p.Length; i++) if (p[i] != null) p[i].Init(i);
            ENABLE(true);

            if (EditorPrefs.GetInt(Folders.PREFS_PATH + "|showWelcomeStart", 0) != 2 ||
                p[0].par_e.WELCOME_SHOW_IN_EVERY_PROJECTS && !p[0].par_e.WELCOME_WERE_IN_EVERY_PROJECTS)
            {
                p[0].par_e.WELCOME_WERE_IN_EVERY_PROJECTS = true;
                EditorPrefs.SetInt(Folders.PREFS_PATH + "|showWelcomeStart", 2);
                WelcomeScreen.Init(null);
            }

            hasMainConstructorInit = true;

            Window.TRY_INIT_ON_INITIALIZATION();
        }

        internal static void ClearCacheOnCompile()
        {
            if (!hasInit) return;
            if (p == null) return;
            if (p.Length == 0) return;
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] == null) continue;
                if (p[i].modsController == null) continue;
#if !EMX_H_LITE
                if (p[i].modsController.highLighterMod != null) p[i].modsController.highLighterMod.ClearCacheAdditional();
#endif
                p[i].RESET_DRAWSTACK(i);
            }
        }

#if !EMX_H_LITE
        internal static void SET_EXTERNAl_MOD(Mods.ExternalModRoot mod)
        {
            if (!p[0].par_e.ENABLE_ALL) return;
            p[0].modsController.activeExternalMods.RemoveAll(m => !m || m.currentWindow == mod.currentWindow);
            p[0].modsController.activeExternalMods.Add(mod);
            // p[ 0 ].modsController.REBUILD_PLUGINS( true );
            //RebuildasD();
            p[0].modsController.REBUILD_PLUGINS_FAST();
        }
        internal static void REMOVE_EXTERNAl_MOD(Mods.ExternalModRoot mod)
        {
            if (!p[0].par_e.ENABLE_ALL) return;
            p[0].modsController.activeExternalMods.RemoveAll(m => !m || m.currentWindow == mod.currentWindow);
            p[0].modsController.activeExternalMods.Remove(mod);
            // p[ 0 ].modsController.REBUILD_PLUGINS( true );
            //RebuildasD();
            p[0].modsController.REBUILD_PLUGINS_FAST();
        }

#endif

        static bool enabled = false;
        internal static void DISABLE()
        {
            if (!enabled) return;
            enabled = false;
#if !EMX_H_LITE
            foreach (var item in Root.p[0].modsController.activeExternalMods.ToList())
            {
                if (!item) continue;
                item.Close();
            }
#endif
            var sbs = new EditorSubscriber(Root.p[0]);
            Root.p[0]._rebuild_editor_events(sbs);

            foreach (var item in p)
            {
                if (item == null) continue;
                EditorApplication.hierarchyWindowItemOnGUI -= item.hierarchy_gui;
                EditorApplication.update -= item.Update;
                EditorSceneManager.sceneOpened -= item.invoke_EditorSceneManagerOnSceneOpening;



                Selection.selectionChanged -= item.invoke_onSelectionChanged;


                EditorApplication.hierarchyChanged -= item.invoke_onHierarchyChanged;
                EditorApplication.playModeStateChanged -= item.invoke_onPlayModeStateChanged;

                UnityEditor.Undo.undoRedoPerformed -= item.invoke_unUndo;
                EditorApplication.projectWindowItemOnGUI -= item.invoke_OnProjectWindow;
                EditorApplication.wantsToQuit -= item.invoke_OnEditorWantsToQuit;
#if !UNITY_2017_1_OR_NEWER
                SceneView.onSceneGUIDelegate -= item.invoke_DuringSceneGUI;
#else
                SceneView.duringSceneGui -= item.invoke_DuringSceneGUI;
#endif
                EditorApplication.modifierKeysChanged -= item.invoke_ModifiyKeyChanged;
                var info = typeof(EditorApplication).GetField("globalEventHandler", (BindingFlags)(-1));
                var value = (EditorApplication.CallbackFunction)info.GetValue(null);
                value -= item.EditorGlobalKeyPress;
                info.SetValue(null, value);
            }
            if (Mods.SnapMod.SET_ENABLE(Root.p[0].par_e.USE_SNAP_MOD && Root.p[0].par_e.ENABLE_ALL) || Root.p[0].par_e.USE_PROJECT_AUTO_HIGHLIGHTER_MOD || Root.p[0].par_e.USE_PROJECT_MANUAL_HIGHLIGHTER_MOD)
                RequestScriptReload();
            hasMainConstructorInit = false;
        }



        internal static void ENABLE(bool skipReload = false)
        {
            if (enabled) return;
            Init();
            enabled = true;
            foreach (var item in p)
            {
                if (item == null) continue;
                EditorApplication.hierarchyWindowItemOnGUI += item.hierarchy_gui;
                EditorApplication.update += item.Update;
                EditorSceneManager.sceneOpened += item.invoke_EditorSceneManagerOnSceneOpening;
                //  EditorSceneManager.activeSceneChangedInEditMode += item.invoke_EditorSceneManagerOnSceneOpening2;
                //  EditorSceneManager.sce += item.invoke_EditorSceneManagerOnSceneOpening3;


                Selection.selectionChanged += item.invoke_onSelectionChanged;


                EditorApplication.hierarchyChanged += item.invoke_onHierarchyChanged;
                EditorApplication.playModeStateChanged += item.invoke_onPlayModeStateChanged;
                UnityEditor.Undo.undoRedoPerformed += item.invoke_unUndo;
                EditorApplication.projectWindowItemOnGUI += item.invoke_OnProjectWindow;
                EditorApplication.wantsToQuit += item.invoke_OnEditorWantsToQuit;
#if !UNITY_2017_1_OR_NEWER
                SceneView.onSceneGUIDelegate += item.invoke_DuringSceneGUI;
#else
                SceneView.duringSceneGui += item.invoke_DuringSceneGUI;
#endif
                EditorApplication.modifierKeysChanged += item.invoke_ModifiyKeyChanged;
                var info = typeof(EditorApplication).GetField("globalEventHandler", (BindingFlags)(-1));
                var value = (EditorApplication.CallbackFunction)info.GetValue(null);
                value += item.EditorGlobalKeyPress;
                info.SetValue(null, value);
            }
            Root.p[0].modsController.REBUILD_PLUGINS();
            if (Mods.SnapMod.SET_ENABLE(Root.p[0].par_e.USE_SNAP_MOD && Root.p[0].par_e.ENABLE_ALL))
                if (!skipReload) RequestScriptReload();

        }

        internal static void RequestScriptReload()
        {
#if UNITY_2019_3_OR_NEWER
            EditorUtility.RequestScriptReload();
#else
			InternalEditorUtility.RequestScriptReload();
#endif
        }

        internal static PropertyInfo GUIView_current, View_window, View_position, View_windowPosition, DockArea_selected, HostView_actualView;
        internal static Type DockArea;
        internal static FieldInfo DockArea_m_Panes;
        internal static Type UnityEventArgsType;
        internal static MethodInfo SceneFrameMethod;

        static MethodInfo _SetMouseTooltip;

        static void InitializeRoot()
        {
            var GUIView = Assembly.GetAssembly(typeof(EditorGUIUtility)).GetType("UnityEditor.GUIView") ?? throw new Exception("GUIView");
            GUIView_current = GUIView.GetProperty("current", ~(BindingFlags.Instance | BindingFlags.InvokeMethod)) ?? throw new Exception("current");
            View_window = GUIView.BaseType.GetProperty("window", ~(BindingFlags.Static | BindingFlags.InvokeMethod)) ?? throw new Exception("window");
            View_position = GUIView.BaseType.GetProperty("position", ~(BindingFlags.Static | BindingFlags.InvokeMethod)) ?? throw new Exception("position");
            View_windowPosition = GUIView.BaseType.GetProperty("windowPosition", ~(BindingFlags.Static | BindingFlags.InvokeMethod)) ?? throw new Exception("windowPosition");
            DockArea = Assembly.GetAssembly(typeof(EditorGUIUtility)).GetType("UnityEditor.DockArea") ?? throw new Exception("DockArea");
            DockArea_m_Panes = DockArea.GetField("m_Panes", ~(BindingFlags.Static | BindingFlags.InvokeMethod)) ?? throw new Exception("m_Panes");
            DockArea_selected = DockArea.GetProperty("selected", ~(BindingFlags.Static | BindingFlags.InvokeMethod)) ?? throw new Exception("selected");
            UnityEventArgsType = Assembly.GetAssembly(typeof(UnityEngine.Events.UnityEvent)).GetType("UnityEngine.Events.ArgumentCache", true) ?? throw new Exception("ArgumentCache");
            SceneFrameMethod = (typeof(SceneView).GetMethod("Frame", ~(BindingFlags.Static | BindingFlags.GetField))) ?? throw new Exception("Frame");
            InitSetTooltipFunction();
            var HostView = Assembly.GetAssembly(typeof(EditorGUIUtility)).GetType("UnityEditor.HostView") ?? throw new Exception("HostView");
            HostView_actualView = HostView.GetProperty("actualView", ~(BindingFlags.Static | BindingFlags.InvokeMethod)) ?? throw new Exception("actualView");
            Window.InitFields();


        }

        static bool init_set_was_init = false;
        static void InitSetTooltipFunction()
        {
            if (init_set_was_init) return;
            init_set_was_init = true;
            _SetMouseTooltip = (typeof(GUIStyle).GetMethod("SetMouseTooltip", ~(BindingFlags.Instance | BindingFlags.GetField))) ?? throw new Exception("SetMouseTooltip");
        }

        static object[] _setTooltipArgs = new object[2];
        internal static void SetMouseTooltip(string content, Rect rect)
        {
            _c.tooltip = content;
            SetMouseTooltip(_c, rect);
        }
        static GUIContent _c = new GUIContent();
        internal static void SetMouseTooltip(GUIContent content, Rect rect, Event ev = null)
        {

            if (content.tooltip == null || content.tooltip == "") return;

            if (!rect.Contains((ev ?? Root.p[0].EVENT).mousePosition) || ev == null && !Root.p[0].trueNulled_GUIClip_visibleRect.Contains((ev ?? Root.p[0].EVENT).mousePosition)) return;

            _setTooltipArgs[0] = content.tooltip;
            _setTooltipArgs[1] = rect;
            InitSetTooltipFunction();
            _SetMouseTooltip.Invoke(null, _setTooltipArgs);
        }

    }



    internal class StyleInitHelper
    {
        public static implicit operator bool(StyleInitHelper h)
        {
            return h.value == true && h.proSkin == EditorGUIUtility.isProSkin;
        }
        public static implicit operator StyleInitHelper(bool h)
        {
            return new StyleInitHelper() { proSkin = PluginInstance.WAS_GUI_FLAG ? EditorGUIUtility.isProSkin : (bool?)null, value = h };
        }
        internal bool? value = null;
        internal bool? proSkin = null;
    }




    internal partial class PluginInstance
    {

        internal int pluginID;
        internal HierarchyObject o;
        internal static Window windowEmpty = new Window();
        internal Window window;
        internal Window lastHierarchyWindw;
        internal Window lastProjectWindw;
        internal Window firstWindow(int pid) { { return Window._update.Values.FirstOrDefault(w => w.pluginID == pid); } }
        //internal Window firstHierarchyWindow { get { return Window._update.Values.FirstOrDefault(w => w.pluginID == 0) ?? window; } }
        //internal Window firstProjectWindow { get { return Window._update.Values.FirstOrDefault(w => w.pluginID == 1) ?? window; } }
        internal bool UseRootWindow_0;
        internal bool UseRootWindow_1;
        internal bool UseRootWindow { get { return pluginID == 0 ? UseRootWindow_0 : UseRootWindow_1; } }
        internal string pluginname;

        Events.HierarchyActions ha_0;
        Events.HierarchyActions ha_1;
        internal Events.HierarchyActions ha { get { return pluginID == 0 ? ha_0 : ha_1; } }
        internal Events.HierarchyActions ha_G(int i) { { return i == 0 ? ha_0 : ha_1; } }
        internal EditorSettingsAdapter par_e;
        DuplicateHelper duplicate;
        WindowsManager windowsManager;
        //	internal HierarchyModification hierarchyModification;
        internal ModsController modsController;
        internal GlDrawer gl;
        internal EditorSettingsAdapter.WindowSettings WIN_SET { get { return pluginID == 0 ? par_e.HIER_WIN_SET : par_e.PROJ_WIN_SET; } }
        internal EditorSettingsAdapter.WindowSettings WIN_SET_INVERSE { get { return pluginID == 0 ? par_e.PROJ_WIN_SET : par_e.HIER_WIN_SET; } }
        internal EditorSettingsAdapter.WindowSettings WIN_SET_G(int pluginID) { return pluginID == 0 ? par_e.HIER_WIN_SET : par_e.PROJ_WIN_SET; }

        internal EditorSettingsAdapter.HighlighterSettings HL_SET { get { return pluginID == 0 ? par_e.HIER_HIGH_SET : par_e.PROJ_HIGH_SET; } }
        internal EditorSettingsAdapter.HighlighterSettings HL_SET_G(int pluginID) { return pluginID == 0 ? par_e.HIER_HIGH_SET : par_e.PROJ_HIGH_SET; }




        internal MethodInfo gui_getFirstAndLastRowVisible, data_FindItem_Slow,
                        gui_GetRowRect, ExpansionAnimator_CullRow, data_m_dataSetExpanded, data_m_dataIsExpanded, data_m_dataSetExpandWithChildrens, hierwin_DuplicateGO;
        FieldInfo _AssetTreeState, _FolderTreeState, _TreeViewController_0, _FoldView, _AssetsView, _ViewMode, /*gui_m_LineHeight, gui_k_IndentWidth, gui_k_IconWidth,  gui_customFoldoutYOffset,*/   tree_m_ContentRect, m_UseExpansionAnimation
                // ,tree_m_KeyboardControlIDField
                ;
        internal FieldInfo state_scrollPos, tree_m_ExpansionAnimator, m_SearchFieldText, tree_m_TotalRect, m_SearchFilter, tree_m_VisibleRect, s;
        internal MethodInfo data_GetRows, PrefabModeButton, data_GetItemRowFast, IsSearching,
#pragma warning disable
        GetInstanceIDFromGUID, GetMainAssetOrInProgressProxyInstanceID;
#pragma warning restore
        internal int hoverID;
        internal bool _hashoveredItem;
        internal bool hashoveredItem {
            get { return _hashoveredItem && !WIN_SET.HIDE_HOVER_BG; }
            set { _hashoveredItem = value; }
        }
        internal PropertyInfo _data, _gui, _state, hoveredItem, showPrefabModeButton, tree_animatingExpansion, ExpansionAnimator_endRow, data_rowCount;
        internal PropertyInfo data_m_RootItem, guiclip_visibleRect;
        object[] args = new object[2];
        int firstRowVisible, lastRowVisible;

        // ???
        // ???
        // ???
        //internal Rect _currentClipRect;
        // ???
        // ???
        // ???


        internal IconData GetNewIcon(NewIconTexture t, string key) { return Root.icons.GetNewIcon(t, ref key); }
        internal IconData GetOldIcon(string s, bool includePersonal = false)
        {
            if (!includePersonal) return Root.icons.GetOldIcon(ref s);
            if (!GET_SLOW_oldProSkin) s += " PERSONAL";
            return Root.icons.GetOldIcon(ref s);
        }
        internal Texture2D GetExternalModOld(string s) { return Root.icons.GetOldExternalMod(ref s); }
        //  delegate void GetFirstAndLastRowVisible( out int firstRowVisible, out int lastRowVisible );
        //  GetFirstAndLastRowVisible gui_getFirstAndLastRowVisible;
        //(GetFirstAndLastRowVisible)Delegate.CreateDelegate( typeof( GetFirstAndLastRowVisible ), this,


        //internal bool NEW_PERFOMANCE { get { return pluginID == 0; } }
        internal bool NEW_PERFOMANCE = true;
        // return UNITY_CURRENT_VERSION >= UNITY_2019_VERSION;



        internal static PluginInstance CreateInstance(string name)
        {
            var res = new PluginInstance();
            res.pluginname = name;
            return res;
        }
        internal void Init(int pId)
        {
            pluginID = pId;
            Init();
            gl = new GlDrawer(this);
            par_e = new EditorSettingsAdapter(pId);
            ha_0 = new Events.HierarchyActions(0);
            ha_1 = new Events.HierarchyActions(1);
            duplicate = new DuplicateHelper(pId);
            windowsManager = new WindowsManager(pId);
            _mouse_uo_helper[0] = new Events.MouseRawUp();
            _mouse_uo_helper[1] = new Events.MouseRawUp();
            //hierarchyModification = new HierarchyModification(this);
            modsController = new ModsController(this);
            window = new Window();


            modsController.REBUILD_PLUGINS(true);
        }



        internal Event EVENT;
        object[] argsa = new object[1];

        //FIELDS- A
        internal EventType?[] lastEvent = new EventType?[2];
        internal class AdapterData
        {
            internal AdapterData(int plugind)
            {
                this.pluginId = plugind;
            }
            internal int pluginId;

            internal bool firstLaunch = true;
            internal int fullFrame = -1;
            internal int firstFrame = -1;

            internal int bakedRightClickFrame;
            internal MousePos? bakedRightClickPos;
            internal MousePos GET_CURRENT_MOUSE_POS_FOR_MENU {
                get {
                    //Debug.Log( bakedRightClickFrame + " " + fullFrame );
                    if (Mathf.Abs(bakedRightClickFrame - fullFrame) > 5) return new MousePos((Event.current ?? Root.p[0].EVENT).mousePosition, MousePos.Type.Input_190_68, true, Root.p[0]);
                    return bakedRightClickPos ?? new MousePos((Event.current ?? Root.p[0].EVENT).mousePosition, MousePos.Type.Input_190_68, true, Root.p[0]);
                }
            }
        }



        AdapterData[] _ad = { new AdapterData(0), new AdapterData(1) };
        internal AdapterData AD {
            get { return _ad[pluginID]; }
            //set { _ad[ pluginID ] = value; }
        }
        //int[] _firstFrame= Enumerable.Repeat(-1,10).ToArray();
        //internal int firstFrame {
        //	get { return _firstFrame[ pluginID ]; }
        //	set { _firstFrame[ pluginID ] = value; }
        //}


        //FIELDS- B
        int index = 0;
        int num = 0;
        int numVisibleRows = 0;
        int rowCount;
        internal Vector2 scrollPos;
        bool animatingExpansion;
        object m_ExpansionAnimator;
        internal Rect m_TotalRect, m_ContentRect, trueNulled_GUIClip_visibleRect; //, treeVisibleRectValue
        bool showingVerticalScrollBar;
        bool p_showingVerticalScrollBarInit;
        PropertyInfo p_showingVerticalScrollBar;
        int endRow;
        float rowWidth;
        int CalcRow(ref int index, ref int num)
        {
            int row = -1;
            for (; index < numVisibleRows; ++index)
            {
                row = firstRowVisible + index;
                if (this.animatingExpansion)
                {
                    // int endRow = this.m_ExpansionAnimator.endRow;
                    //  if ( this.m_ExpansionAnimator.CullRow( row, this.gui ) )
                    args[0] = row;
                    args[1] = gui_currentTree;
                    var res = (bool)ExpansionAnimator_CullRow.Invoke(m_ExpansionAnimator, args);
                    if (res)
                    {
                        ++num;
                        row = endRow + num;
                    }
                    else
                        row += num;
                    // if ( row >= this.data.rowCount )
                    if (row >= rowCount)
                        continue;
                }
                else
                {
                    args[0] = row;
                    args[1] = 0f;
                    var res = (Rect)gui_GetRowRect.Invoke(gui_currentTree, args);
                    if ((double)(res.y - scrollPos.y) > m_TotalRect.height)
                        continue;
                }
                break;
            }
            return row;
        }
        // -

        internal float DEFAULT_ICON_SIZE {
            get {
                //EditorGUIUtility.GetIconSize
                return Window.DefaultIconSize(this);
                //	return EditorGUIUtility.GetIconSize().x;
                //return DEFAULT_ICON_SIZE;
            }
        }

        internal void PUSH_MENU_OPENING_ACTION(GenericMenu menu, Action ClearAction)
        {
            var pi = pluginID;
            PUSH_GUI_ONESHOT(pluginID, () => {
                menu.ShowAsContext();
                //p.PUSH_UPDATE_ONESHOT( PluginId, () => {} );
                if (ClearAction != null) PUSH_UPDATE_ONESHOT(pi, () => { ClearAction(); });
            });
        }

        //EVENTS-
        internal void PUSH_ONMOUSEUP(int plug, Func<Events.MouseRawUp.WantMouseLeaveType, bool> ac, EditorWindow win = null) { _mouse_uo_helper[plug].PUSH_ONMOUSEUP(ac, win); }
        Events.MouseRawUp[] _mouse_uo_helper = new Events.MouseRawUp[2];
        internal bool PUSH_GUI_ONESHOT(int plug, Action ac)
        {
            bool allow = false;
            foreach (var w in WindowsData(plug)) if (w.Value.w.Instance) allow = true;
            if (!allow)
            {
                //ac();
                return false;
            }
            _oneShotGui[plug].Add(new gui_1_s() { frame = last_CHECK_ONESHOT_GUI_frame, action = ac });
            RepaintWindowInUpdate(plug);
            return true;
        }
        internal bool PUSH_GUI_ONESHOT_SKIP_CHECK_WINDOW_AWALIABLE(int plug, Action ac)
        {
            _oneShotGui[plug].Add(new gui_1_s() { frame = last_CHECK_ONESHOT_GUI_frame, action = ac });
            RepaintWindowInUpdate(plug);
            return true;
        }
        internal bool PUSH_GUI_ONESHOT_FORCE_FIRSTONLY(int plug, Action ac)
        {
            bool allow = false;
            foreach (var w in WindowsData(plug)) if (w.Value.w.Instance) allow = true;
            if (!allow)
            {
                //ac();
                return false;
            }
            _oneShotGuiForceFirstOnly[plug].Add(ac);
            RepaintWindowInUpdate(plug);
            return true;
        }
        struct gui_1_s
        {
            internal int frame;
            internal Action action;
        }
        List<gui_1_s>[] _oneShotGui = { new List<gui_1_s>(), new List<gui_1_s>() };
        List<Action>[] _oneShotGuiForceFirstOnly = { new List<Action>(), new List<Action>() };
        internal void PUSH_UPDATE_ONESHOT(int plugin, Action ac) { _oneShotUpdate[plugin].Add(ac); }
        List<Action>[] _oneShotUpdate = { new List<Action>(), new List<Action>() };
        // -

        // OTHER
        bool? __GET_SLOW_oldProSkin;
        internal bool GET_SLOW_oldProSkin {
            get {
                if (!__GET_SLOW_oldProSkin.HasValue) __GET_SLOW_oldProSkin = EditorGUIUtility.isProSkin;
                return __GET_SLOW_oldProSkin.Value;
            }
        }
        class IS_PRO_SKIN
        {
            internal bool HasValue { get { return SessionState.GetInt(Folders.PREFS_PATH + "IS_PRO_SKIN", -1) != -1; } }
            internal bool Value {
                get { return SessionState.GetInt(Folders.PREFS_PATH + "IS_PRO_SKIN", -1) == 1; }
                set { SessionState.SetInt(Folders.PREFS_PATH + "IS_PRO_SKIN", value ? 1 : 0); }
            }
        }
        IS_PRO_SKIN oldProSkin = new IS_PRO_SKIN();
        // --

        //MAIN GUI-
        internal object TreeController_current;
        internal object gui_currentTree, data_currentTree, state_currentTree;



        internal void WriteTreeController(EditorWindow w = null)
        {
            TreeController_current = GetTreeViewontroller(pluginID, w);
            //if ( t != TreeController_current )
            //{
            //	TreeController_current = t;
            //}
            gui_currentTree = _gui.GetValue(TreeController_current);
            data_currentTree = _data.GetValue(TreeController_current);
            state_currentTree = _state.GetValue(TreeController_current);
        }

#if UNITY_2021_1_OR_NEWER
        internal HashSet<int> current_DragSelection_List = new HashSet<int>();
        internal HashSet<int> current_selectedIDs = new HashSet<int>();

#else
		internal IList<int> current_DragSelection_List = new List<int>();
		internal IList<int> current_selectedIDs = new List<int>();
#endif
        bool GuiReady = true;
        internal Rect selectionRect;
        internal Rect fullLineRect, _first_FullLineRect, _last_FullLineRect;
        internal float rightOffset = 0;

        internal class EditorWindowData
        {
            internal int pid;
            internal EditorWindowData(int pid)
            {
                this.pid = pid;
            }
            internal Window w;
            internal object lastTree;
            internal bool nowSearch;
        }
        static Dictionary<int, EditorWindowData> allWindowsData = new Dictionary<int, EditorWindowData>();
        internal static Dictionary<int, EditorWindowData> HierarchyWindowsData { get { return allWindowsData.Where(d => d.Value.pid == 0).ToDictionary(k => k.Key, v => v.Value); } }
        internal static Dictionary<int, EditorWindowData> ProjectWindowsData { get { return allWindowsData.Where(d => d.Value.pid == 1).ToDictionary(k => k.Key, v => v.Value); } }
        internal static Dictionary<int, EditorWindowData> WindowsData(int pid) { return pid == 0 ? HierarchyWindowsData : ProjectWindowsData; }
        int last_CHECK_ONESHOT_GUI_frame = -1;
        void CHECK_ONESHOT_GUI()
        {
            if ((EVENT.type == EventType.Repaint) && _oneShotGui[pluginID].Count > 0)
            {
                var ex = _oneShotGui[pluginID].ToList();
                _oneShotGui[pluginID].Clear();
                for (int i = 0; i < ex.Count; i++)
                {
                    //if ( ex[ i ].frame == last_CHECK_ONESHOT_GUI_frame ) continue;
                    ex[i].action();
                    //Debug.Log( ex[ i ].frame + " " + last_CHECK_ONESHOT_GUI_frame);
                    ex.RemoveAt(i);
                    i--;
                }
                if (ex.Count != 0) _oneShotGui[pluginID].AddRange(ex);
                //foreach ( var item in _oneShotGui[ pluginID ] ) item();
                //_oneShotGui[ pluginID ].Clear();
            }
            if (EVENT.type == EventType.Repaint && _oneShotGuiForceFirstOnly[pluginID].Count > 0)
            {
                var ex = _oneShotGuiForceFirstOnly[pluginID].ToList();
                _oneShotGuiForceFirstOnly[pluginID].Clear();
                foreach (var item in ex) item();
            }
            last_CHECK_ONESHOT_GUI_frame = AD.fullFrame;
        }
        void CHECK_ONESHOT_GUI_FIRSTONLY()
        {
            if (EVENT.type == EventType.Repaint && _oneShotGuiForceFirstOnly[pluginID].Count > 0)
            {
                var ex = _oneShotGuiForceFirstOnly[pluginID].ToList();
                _oneShotGuiForceFirstOnly[pluginID].Clear();
                foreach (var item in ex) item();
            }
        }
        static bool firstFixDraw = false;
        //static bool firstFixDrawStart = false;
        internal bool HoverDisabled = false;
        bool shouldBuildedBeginGUI = false;

        internal int MOUSE_BUTTON_0 { get { return par_e.USE_SWAP_FOR_BUTTONS_ACTION ? 1 : 0; } }
        internal int MOUSE_BUTTON_1 { get { return par_e.USE_SWAP_FOR_BUTTONS_ACTION ? 0 : 1; } }

        internal static bool WAS_GUI_FLAG = false;
        //bool[] firstFrameBool = new bool[2];
        // bool sended = false;
        internal void hierarchy_gui(int instanceID, Rect selectionRect)
        {
            if (!Root.SKIP_TEMP_DIS)
            {
                if (Root.TemperaryPluginDisabled) return;

                pluginID = 0;
                try
                {
                    gui(instanceID, ref selectionRect);
                }
                catch (Exception ex)
                {
                    Root.TemperaryDisableThePlugin_FromRoot(ex);
                }
            }
            else
            {
                gui(instanceID, ref selectionRect);
            }

        }
        internal bool baked_HARD_BAKE_ENABLED;
        internal void FADE_IF_NO_BAKE(Rect opasRect)
        {
            var c = Colors.SceneColor;
            c.a = par_e.RIGHT_BG_OPACITY;
            var old = GUI.color;
            var gc = GUI.color;
            gc.a = 1;
            GUI.color = gc;
            EditorGUI.DrawRect(opasRect, c);
            GUI.color = old;
        }
        float windtrFix;
        static EditorWindow[] lastWindow = new EditorWindow[2];
        internal int keyboardControl_current;
        internal bool thisIsLast;


        void FixFloatScroll(ref Rect r)
        {
            r.width = (int)r.width;
            //r.x -= scrollPos.x % 1;
            //r.y -= scrollPos.y % 1;
        }

        internal void gui(int instanceID, ref Rect selectionRect)
        {

            //if ( pluginID == 1 && Event.current.type == EventType.Layout )
            //{
            //    lastEvent = null;
            //    return;
            //}

            //if ( firstFrameBool[ pluginID ] )
            //{
            //    //Debug.Log("ASD" + Event.current.type);
            //    if ( Event.current.type == EventType.Repaint )
            //    {
            //        firstFrameBool[ pluginID ] = false;
            //        // Window.AssignInstance(pluginID, ref window, pluginID == 0 ? SceneHierarchyWindowRoot : ProjectBrowserWindowType); // WINDOW INIT
            //        window.Instance.SendEvent( new Event() { type = EventType.Layout } );
            //        window.Instance.SendEvent( new Event() { type = EventType.Repaint } );
            //        EditorGUIUtility.ExitGUI();
            //        return;
            //    }
            //
            //}
            // if ( Event.current.type == EventType.Layout ) return;
            if (!WAS_GUI_FLAG) WAS_GUI_FLAG = true;
            this.selectionRect = selectionRect;




            EVENT = Event.current;
            //var _go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;


            // bool asd = false;
            if (GuiReady/* || !_go*/  || lastEvent[pluginID] != EVENT.type)
            {
                //GameObject asd = null;
                //Debug.Log( asd.name );
                CompilationTimer();


                if (!oldProSkin.HasValue) oldProSkin.Value = EditorGUIUtility.isProSkin;
                if (oldProSkin.Value != EditorGUIUtility.isProSkin)
                {
                    Cache.ClearHierarchyObjects(false);
                    Cache.ClearHierarchyObjects(true);
                    oldProSkin.Value = EditorGUIUtility.isProSkin;
                    __GET_SLOW_oldProSkin = null;
                    par_e.ClearCache();
                    Root.p[0].invoke_ReloadAfterAssetDeletingOrPasting();
                    //Root.p[ 0 ].modsController.REBUILD_PLUGINS();
                }

                if (EVENT.type == EventType.MouseDown && EVENT.button == 1)
                {
                    AD.bakedRightClickPos = new MousePos(Event.current.mousePosition, MousePos.Type.Input_190_68, true, this);
                    AD.bakedRightClickFrame = AD.fullFrame;
                }
                if (AD.bakedRightClickPos.HasValue && Mathf.Abs(AD.bakedRightClickFrame - AD.fullFrame) > 5)
                {
                    AD.bakedRightClickPos = null;
                }

                keyboardControl_current = GUIUtility.keyboardControl;
                Window.AssignInstance(pluginID, ref window, pluginID == 0 ? SceneHierarchyWindowRoot : ProjectBrowserWindowType); // WINDOW INIT

                if (!window.Instance)
                {
                    lastEvent[pluginID] = null;
                    return;
                }
                if (lastWindow[pluginID] && lastWindow[pluginID] != window.Instance)
                {
                    window.RESET_GAMEOBJECTS_NAMES(pluginID);
                    return;
                }

                //Debug.Log( EditorGUIUtility.GetControlID( FocusType.Passive ) );
                //Debug.Log( GUIUtility.GetControlID( 0, FocusType.Passive ));
                lastWindow[pluginID] = window.Instance;


                var evallow = AD.firstLaunch || EVENT.type == EventType.Layout;
                if (AD.firstLaunch) AD.firstLaunch = false;
                if (AD.firstFrame < 5 && evallow) AD.firstFrame++;
                if (evallow) AD.fullFrame++;
                if (AD.fullFrame > 10000000) AD.fullFrame = 0;


                for (int i = 0; i < Window._windowsList.Count; i++)
                {
                    if (Window._windowsList[i].pluginID != pluginID) continue;
                    Window._windowsList[i].thisIsAFirstWindow = false;
                }
                window.thisIsAFirstWindow = true;

                //Debug.Log( window.W_OFFSET );

#if EMX_USE_FAST_BOTTOM_FIX
				if ( window.W_OFFSET != 0 )
				{
					//selectionRect.width -= window.W_OFFSET;
					//this.selectionRect.width -= window.W_OFFSET;
				}
#endif

                _mouse_uo_helper[pluginID].Invoke();
                modsController.rightModsManager.headerEventsBlockRect = null;
                HoverDisabled = false;
                CHECK_ONESHOT_GUI_FIRSTONLY();
                //CHECK_ONESHOT_GUI();



                GuiReady = true;
                lastEvent[pluginID] = EVENT.type;


                var t = GetTreeViewontroller(pluginID, window.Instance);
                if (t != TreeController_current)
                {
                    TreeController_current = t;
                    //var info = TreeController_current.GetType().GetProperty( "itemSingleClickedCallback", (BindingFlags)(-1) );
                    //var value = (Action<int>)info.GetValue(TreeController_current);
                    //value += invoke_onSelectionChanged;
                    //info.SetValue( TreeController_current, value, null );
                }
                gui_currentTree = _gui.GetValue(TreeController_current);
                data_currentTree = _data.GetValue(TreeController_current);
                state_currentTree = _state.GetValue(TreeController_current);

                ha.TryToInitializeDefaultStyles();
                animatingExpansion = (bool)tree_animatingExpansion.GetValue(TreeController_current, null);
                if (animatingExpansion)
                {
                    m_ExpansionAnimator = tree_m_ExpansionAnimator.GetValue(TreeController_current);
                    endRow = (int)ExpansionAnimator_endRow.GetValue(m_ExpansionAnimator, null);
                    /*if ( !sended && lastEvent.Value == EventType.Layout)
					{
							EditorGUIUtility.ExitGUI();
							window.Instance.SendEvent( new Event() { type = lastEvent.Value } );
					}
					sended = true;*/
                }

                if (g[pluginID].OnAssignWindowFirstFrame != null) g[pluginID].OnAssignWindowFirstFrame(window);



                // if ( window.bottomParams != null )
                // {
                //     // var estim = _gui.PropertyType;
                //     var estim = _gui.PropertyType.Assembly.GetType("UnityEditor.IMGUI.Controls.TreeViewGUI");
                //     // var estim = _gui.PropertyType.Assembly.GetType("UnityEditor.GameObjectTreeViewGUI");
                //
                //     // while ( estim.BaseType != null || estim.BaseType.BaseType != null ) estim = estim.BaseType;
                //     var b = estim.GetField("k_BottomRowMargin" , (BindingFlags)~0);
                //     //FieldInfo b = null;
                //     //foreach ( var item in estim.GetFields() )
                //     //{
                //     //    if ( item.Name == "k_BottomRowMargin" )
                //     //    {
                //     //        b = item;
                //     //        break;
                //     //    }
                //     //}
                // }




                //if ( window.bottomParams != null )
                {
                    // var ps = typeof(EditorWindow).GetField("m_Pos", (BindingFlags)~0);
                    // var trueNulled_GUIClip_visibleRect = (Rect)ps.GetValue( window.Instance );
                    // trueNulled_GUIClip_visibleRect.height -= window.bottomParams.HEIGHT;
                    // ps.SetValue( window.Instance, trueNulled_GUIClip_visibleRect );
                    // //var trueNulled_GUIClip_visibleRect = (Rect)p.guiclip_visibleRect.GetValue( null, null );
                    // //trueNulled_GUIClip_visibleRect.height -= p.window.bottomParams.HEIGHT;
                    // //p.guiclip_visibleRect.SetValue( null, trueNulled_GUIClip_visibleRect );
                    // var tr = (Rect)tree_m_VisibleRect.GetValue( TreeController_current ); ;
                    // tr.height -= window.bottomParams.HEIGHT;
                    // tree_m_VisibleRect.SetValue( TreeController_current, tr );

                    //  var typ = typeof( GUI ).Assembly.GetType( "UnityEngine.ScrollViewState" );
                    //  var s = GUIUtility.GetStateObject(typ, GUIUtility.GetControlID("scrollView".GetHashCode(), FocusType.Passive));
                    //
                    //
                    //  var visibleRectF = typ.GetField( "visibleRect", (BindingFlags)~0 );
                    //  var visibleRect = (Rect)visibleRectF.GetValue( s );
                    //  visibleRect.height -= window.bottomParams.HEIGHT;
                    //  visibleRectF.SetValue( s, visibleRect );
                    //  var viewRectF = typ.GetField( "viewRect", (BindingFlags)~0 );
                    //  var viewRect = (Rect)viewRectF.GetValue( s );
                    //  viewRect.height -= window.bottomParams.HEIGHT;
                    //  viewRectF.SetValue( s, viewRect );
                }
                // }
                //
                //
                //
                //
                // if ( asd )
                // {

                m_TotalRect = (Rect)tree_m_TotalRect.GetValue(TreeController_current);
                m_ContentRect = (Rect)tree_m_ContentRect.GetValue(TreeController_current);
                trueNulled_GUIClip_visibleRect = (Rect)guiclip_visibleRect.GetValue(null, null);
                window.GUIClip_visibleRect = trueNulled_GUIClip_visibleRect;

#if EMX_USE_FAST_BOTTOM_FIX
				if ( window.W_OFFSET != 0 )
				{
					//m_TotalRect.width -= window.W_OFFSET;
					//m_ContentRect.width -= window.W_OFFSET;
				}
#endif

                //treeVisibleRectValue = (Rect)tree_m_VisibleRect.GetValue( TreeController_current );

                var tmr = (Rect)tree_m_VisibleRect.GetValue(TreeController_current);
                if (tmr.height > 0)
                {
                    //window.GUIClip_visibleRect = m_TotalRect;
                    if (!p_showingVerticalScrollBarInit)
                    {
                        p_showingVerticalScrollBarInit = true;
                        p_showingVerticalScrollBar = TreeController_current.GetType().GetProperty("showingVerticalScrollBar", (BindingFlags)(-1));
                    }
                    if (p_showingVerticalScrollBar != null)
                    {
                        showingVerticalScrollBar = (bool)p_showingVerticalScrollBar.GetValue(TreeController_current, null);
                        // if ( showingVerticalScrollBar ) window.GUIClip_visibleRect.width -= 16;
                        /// Debug.Log( showingVerticalScrollBar );
                    }
                }
                var s = GET_SKIN().verticalScrollbar;//GET_SKIN().verticalScrollbarUpButton
                windtrFix = s.fixedWidth + s.margin.left + s.margin.right + s.padding.left + s.padding.right + 1;

                //Debug.Log( showingVerticalScrollBar );

                // Debug.Log( m_TotalRect + " " + m_ContentRect + "  " + GUIClip_visibleRect + " " + Event.current.type );
                // showingVerticalScrollBar
                //var asd = p.TreeController_current.GetType().GetField("m_CachedSelection", ~(BindingFlags.InvokeMethod & BindingFlags.Static)).
                //GetValue(p.TreeController_current) as IList<int>;

                if (!allWindowsData.ContainsKey(window.Instance.GetInstanceID()))
                {
                    allWindowsData.Add(window.Instance.GetInstanceID(), new EditorWindowData(pluginID) { w = window, lastTree = TreeController_current, nowSearch = false });
                    window.SetHeightAndIndents(this); // WINDOW SET
                }
                ha.BAKE_SEARCH();
                if (!ReferenceEquals(allWindowsData[window.Instance.GetInstanceID()].lastTree, TreeController_current) || allWindowsData[window.Instance.GetInstanceID()].nowSearch != ha.IS_SEARCH_MOD_OPENED())
                {
                    allWindowsData[window.Instance.GetInstanceID()].lastTree = TreeController_current;
                    allWindowsData[window.Instance.GetInstanceID()].nowSearch = ha.IS_SEARCH_MOD_OPENED();
                    window.SetHeightAndIndents(this); // WINDOW SET
                }
                var o1 = WIN_SET.USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE;
                var o2 = WIN_SET_INVERSE.USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE;
                if (o1 || o2) window.SetHeightAndIndents_Again(this); // WINDOW SET



                window.FinalEvents(this);

                args[0] = args[1] = 0;
                gui_getFirstAndLastRowVisible.Invoke(gui_currentTree, args);
                firstRowVisible = (int)args[0];
                lastRowVisible = (int)args[1];
                numVisibleRows = lastRowVisible - firstRowVisible + 1;

                scrollPos = (Vector2)state_scrollPos.GetValue(state_currentTree);
                //if (scrollPos.y % 1 !=0 || scrollPos.x % 1 != 0 )
                {
                    FixFloatScroll(ref selectionRect);
                    this.selectionRect = selectionRect;
                }

                //if (scrollPos.y % 1 !=0 || scrollPos.x % 1 != 0 )
                //{
                //	scrollPos.x = Mathf.RoundToInt( scrollPos.x );
                //	scrollPos.y = Mathf.RoundToInt( scrollPos.y );
                //	state_scrollPos.SetValue( state_currentTree, scrollPos );
                //}
                rowCount = (int)data_rowCount.GetValue(data_currentTree);
                rowWidth = Mathf.Max(window.GUIClip_visibleRect.width, m_ContentRect.width);

                //rowWidth = m_TotalRect.width;
                // currentRow = firstRowVisible;
                index = 0; num = 0;

                args[0] = lastRowVisible;
                args[1] = 0f;
                //args[ 1 ] = 0f;
                _last_FullLineRect = (Rect)gui_GetRowRect.Invoke(gui_currentTree, args);
                //if ( scrollPos.y % 1 != 0 || scrollPos.x % 1 != 0 )
                {
                    FixFloatScroll(ref _last_FullLineRect);
                }
                //rowWidth = _last_FullLineRect.width;

                // if ( window.bottomParams != null )
                // {
                //     var tr = (Rect)tree_m_TotalRect.GetValue( TreeController_current ); ;
                //     tr.height -= window.bottomParams.HEIGHT;
                //     tree_m_TotalRect.SetValue( TreeController_current, tr );
                // }

                ha.UpdateBGHover();

                window.CheckMouseEvents();
                o = null;

                gl.ButtonsEvents();

                // if ( window.bottomParams != null )
                // {
                //     var tr = (Rect)tree_m_TotalRect.GetValue( TreeController_current ); ;
                //     tr.height -= window.bottomParams.HEIGHT;
                //     tree_m_TotalRect.SetValue( TreeController_current, tr );
                // }
            }
            else
            {
                if (lastWindow[pluginID] && lastWindow[pluginID] != window.Instance)
                {
                    return;
                }

                index++;
                //currentRow++;
            }



            //if ( !firstFixDraw )
            //{
            //    firstFixDraw = true;
            //    firstFixDrawStart = true;
            //    fixDrawLastEvent = EVENT.type;
            //}
            //if ( firstFixDrawStart )
            //{
            //    if ( fixDrawLastEvent != EVENT.type )
            //    {
            //        firstFixDrawStart = false;
            //    }
            //    else
            //    {
            //        //return;
            //    }
            //}

            int row = CalcRow(ref index, ref num);
            var fakeIndex = index + 1;
            var fakeNum = num;
            CalcRow(ref fakeIndex, ref fakeNum);
            thisIsLast = fakeIndex == numVisibleRows || row == lastRowVisible;



            if (row == -1) return;

#if !EMX_H_LITE
            if (!thisIsLast && window.bottomParams != null)
                if (window.bottomParams.Clip(ref selectionRect))
                    thisIsLast = true;
#endif

            argsa[0] = row;
            var currentTreeItemFast = data_GetItemRowFast.Invoke(data_currentTree, argsa);
            args[0] = row;
            args[1] = rowWidth;
            //args[ 1 ] = selectionRect.x + selectionRect.width;

            fullLineRect = (Rect)gui_GetRowRect.Invoke(gui_currentTree, args);

            //if ( scrollPos.y % 1 != 0 || scrollPos.x % 1 != 0 )
            {
                //Debug.Log( fullLineRect + " " + scrollPos );
                FixFloatScroll(ref fullLineRect);
            }

            if (EVENT.type != EventType.Repaint)
            {
                if (showingVerticalScrollBar) fullLineRect.width = window.position.width - windtrFix;
            }
            // lineRect.width = selectionRect.x + selectionRect.width;
            ///fullLineRect.width = rowWidth;
            /// 
            //  selectionRect.y = fullLineRect.y;
            //if ( o != null && o.name == "light_probe" )
            //{
            //    Debug.Log( fullLineRect + " " + selectionRect );
            //}
            if (animatingExpansion && Mathf.Abs(fullLineRect.y - selectionRect.y) > 25)
            {
                selectionRect.height = 0;
            }

            fullLineRect.y = selectionRect.y;
            ///fullLineRect.width = selectionRect.width;

#if EMX_USE_FAST_BOTTOM_FIX
			if ( window.W_OFFSET != 0 )
			{
				fullLineRect.width -= window.W_OFFSET;
				//_last_FullLineRect.width -= window.W_OFFSET;
				//rowWidth -= window.W_OFFSET;
			}
#endif

            //fullLineRect.width = selectionRect.x + selectionRect.width - fullLineRect.x;

            o = pluginID == 0 ? Cache.GetHierarchyObjectByInstanceID(instanceID, null) : Cache.GetHierarchyObjectByGUID(instanceID);
            if (o != null)
            {
                o._visibleTreeItem = currentTreeItemFast as UnityEditor.IMGUI.Controls.TreeViewItem;
                //if ( pluginID == 1 ) o.WriteSibling();
                o.hierarchyIndex = ((int)(fullLineRect.y / fullLineRect.height)) % 2;
            }



            if (GuiReady)
            {

                //modsController.toolBarModification. hotButtons.DrawButtonsOnTopBar();
                //modsController.toolBarModification.layoutsMod.DrawLayers();
                // _first_SelectionRect = selectionRect;
                //Debug.Log( EVENT.type + " " + fullLineRect + " " +selectionRect );
                _first_FullLineRect = fullLineRect;
                baked_HARD_BAKE_ENABLED = gl.HARD_BAKE_ENABLED;
                if (!firstFixDraw)
                {
                    firstFixDraw = true;
                    //EditorGUIUtility.ExitGUI();
                    //window.Instance.SendEvent( new Event() { type =  EventType.Layout } );
                    //window.Instance.SendEvent( new Event() { type =  EventType.Repaint } );
                    ////return;
                    /*var s = GUI.skin.verticalScrollbarUpButton.fixedWidth;
					fullLineRect.width -=s;
					rowWidth -=s;
					_last_FullLineRect.width -=s;
					_last_FullLineRect.width -=s; */
                    //RepaintWindow( true );
                }
                GuiReady = false;
                rightOffset = 0;
                if (ha.hasShowingPrefabHeader) ha.prebapButtonStyle.margin.right = 0;
                ///if ( par_e.RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER && par_e.USE_RIGHT_ALL_MODS ) 
                rightOffset += ha.PREFAB_BUTTON_SIZE;
                if (pluginID == 0) ButtonsActionsDetect();
                if (pluginID == 0) ha.OnSelectionChanged_SaveCache();

                InternalOnGUI_first();

                shouldBuildedBeginGUI = true;
                for (int i = 0; i < g[pluginID].BuildedOnGUI_first.Count; i++)
                    g[pluginID].BuildedOnGUI_first[i]();
                //if (  != null ) g[ pluginID ].BuildedOnGUI_first();
                window.drew_mods_count = 0;

                if (lastEvent[pluginID] != EVENT.type && EVENT.type == EventType.Used) lastEvent[pluginID] = EVENT.type;

            }


            //  Debug.Log(row + " " + lineRect);
            //selectionRect.height = lineRect.height = 16;
            // Debug.Log(lineRect.y);

            if (!o.Validate() || selectionRect.height <= 0 /*|| animatingExpansion && par_e.DISABLE_DRAWING_ANIMATING_ITEMS*/) // THIS IS SCENE
            {
                //EditorUtility.InstanceIDToObject( instanceID ) as SceneAsset;
                //return;
                goto end;
            }

            if (shouldBuildedBeginGUI)
            {

                shouldBuildedBeginGUI = false;
            }


            o.lastFullLineRect = fullLineRect;
            o.lastSelectionRect = selectionRect;


            if (window.drew_mods_count > window.drew_mods_objects.Length) Array.Resize(ref window.drew_mods_objects, window.drew_mods_objects.Length + 100);
            window.drew_mods_objects[window.drew_mods_count].o = o;
            window.drew_mods_objects[window.drew_mods_count].selectionRect = selectionRect;
            window.drew_mods_count++;


            // Debug.Log( row );
            // if ( row == 1 || lineRect.y != 0 ) 
            // GUI.BeginClip( selectionRect );
            // selectionRect.x -= lineRect.x;
            // selectionRect.y = 0;
            //  lineRect.x = lineRect.y = 0;
            //OnGUI();
            for (int i = 0; i < g[pluginID].BuildedOnGUI_middle.Count; i++)
            {
                if (g[pluginID].BuildedOnGUI_middle[i].middle != null)
                    g[pluginID].BuildedOnGUI_middle[i].middle();
                if (g[pluginID].BuildedOnGUI_middle[i].middle_plusLayout != null)
                    g[pluginID].BuildedOnGUI_middle[i].middle_plusLayout();
            }
            //if ( g[ pluginID ].BuildedOnGUI_middle != null && EVENT.type != EventType.Layout ) g[ pluginID ].BuildedOnGUI_middle();
            //if ( g[ pluginID ].BuildedOnGUI_middle_plusLayout != null ) g[ pluginID ].BuildedOnGUI_middle_plusLayout();
            // GUI.EndClip();

            end:;
            EditorActions_EveryFrame_AfterOnGUI();
            if (thisIsLast)
            {
                GuiReady = true;
                CHECK_ONESHOT_GUI();
                if (g[pluginID].BuildedOnGUI_last_butBeforeGL != null && EVENT.type != EventType.Layout) g[pluginID].BuildedOnGUI_last_butBeforeGL();
                gl.ReleaseStack();
                if (g[pluginID].BuildedOnGUI_last != null && EVENT.type != EventType.Layout) g[pluginID].BuildedOnGUI_last();
                // sended = false;

                if (pluginID == 0)
                {
                    if (!Application.isPlaying && oldPlay)
                    {
                        // Debug.Log( "ASD" );
                        KeepLastActiveGameObbject = true;
                        wasdasd = true;
                        playModeCounter = 0;
                    }
                    if (playModeCounter < C)
                    {
                        if (wasdasd && !Application.isPlaying && !oldPlay)
                        {
                            //  Debug.Log( "123" );
                            if (EVENT.type == EventType.Repaint)
                            {
                                playModeCounter++;
                                if (playModeCounter >= C)
                                {
                                    KeepLastActiveGameObbject = false;
                                    RESET_DRAWSTACK(0);
                                }
                            }
                        }
                    }
                    oldPlay = Application.isPlaying;
                }

                //CHECK_ONESHOT_GUI();


            }

            if (HoverDisabled) ha.internal_DisableHover();


            //if ( lastEvent[ pluginID ] != EVENT.type && EVENT.type == EventType.Used ) lastEvent[ pluginID ] = EVENT.type;

            // Debug.Log( GetTreeItem( instanceID ) );
            // current._visibleTreeItem =
        }
        bool oldPlay, wasdasd;
        internal int playModeCounter = C;
        static internal bool KeepLastActiveGameObbject = false;
        const int C = 1;




        void InternalOnGUI_first()
        {

            //Init
            Colors.UpdateColorsBefore_OnGUI(this);

            //RESET_DRAW_STACKS-
            if (window.position.width != _drawStack_oldWPos[pluginID].width || window.position.height != _drawStack_oldWPos[pluginID].height)
            {
                _drawStack_oldWPos[pluginID] = window.position;
                _drawStack_reset_stacks(pluginID);
            }
            if (!_drawStack_lastCacheClean.HasValue) _drawStack_lastCacheClean = EditorApplication.timeSinceStartup;
            if (Math.Abs(_drawStack_lastCacheClean.Value - EditorApplication.timeSinceStartup) > DRAWSTACK_RESET_TIME)
            {
                _drawStack_RESET_TIMER_DRAWSTACKS();
                // Debug.Log( Math.Abs( lastCacheClean.Value - EditorApplication.timeSinceStartup ) );
                _drawStack_lastCacheClean = EditorApplication.timeSinceStartup;
            }
            if (EVENT.type == EventType.Layout && (_drawStack_resetStacks[pluginID] || _drawStack_resetTimerStack[pluginID]))
            {
                _drawStack_reset_stacks(pluginID);
                _drawStack_resetStacks[pluginID] = false;
                _drawStack_resetTimerStack[pluginID] = false;
            }
            //-

            //ANIMATION EXPAND
            if (par_e.USE_EXPANSION_ANIMATION != (bool)m_UseExpansionAnimation.GetValue(TreeController_current))
                m_UseExpansionAnimation.SetValue(TreeController_current, par_e.USE_EXPANSION_ANIMATION);


            //DUPLICATE
            duplicate.Duplicate_FirstFrame_OnGUI();


            //HOVER LINES
            if (hashoveredItem && !HoverDisabled)
            {

                if (windowsManager.InputWindowsCount() > 0) ha.internal_DisableHover();
                /*if ( GetNavigatorRect( 10000 ).y - HEIGHT < EVENT.mousePosition.y )
		DISABLE_HOVER();*/
                var h = hoveredItem.GetValue(TreeController_current, null) as UnityEditor.IMGUI.Controls.TreeViewItem;
                //hoveredItem.SetValue(tree, null, null);
                if (h != null) hoverID = h.id;
                else hoverID = -1;
            }
            else
            {
                hoverID = -1;
            }
        }




        // DRAW_STACK
        const double DRAWSTACK_RESET_TIME = 4;
        double? _drawStack_lastCacheClean;
        Rect[] _drawStack_oldWPos = new Rect[2];
        bool[] _drawStack_resetStacks = new bool[2];
        bool[] _drawStack_resetTimerStack = new bool[2];
        internal void RESET_DRAWSTACK(int plug)
        {
            if (_OnResetDrawStack != null) _OnResetDrawStack();
            _drawStack_resetStacks[plug] = true;

            //EMX_TODO check for a little optimization
            RepaintWindowInUpdate(plug);

#if EMX_HIERARCHY_DEBUG_STACKS
		Debug.Log( "RESET_DRAW_STACKS" );
#endif
        }
        void _drawStack_RESET_TIMER_DRAWSTACKS()
        { //  __reset_stacks();
            _drawStack_resetTimerStack[0] = true;
            _drawStack_resetTimerStack[1] = true;
        }
        void _drawStack_reset_stacks(int plug)
        {

            modsController.RESET_DRAW_STACKS(plug);
            // foreach ( var m in internalMods ) m.Value.ResetDrawStack();
            // foreach ( var m in externalMods ) m.Value.ResetDrawStack();
            /*for ( int i = 0 ; i < modules.Length ; i++ )
			{
					foreach ( var stack in modules[ i ].DRAW_STACK )
					{
							stack.Value.ResetStack();
					}
			}*/
        }
        // -





        /// <summary>
        /// #############################################################################################################################################################
        /// </summary>

        static object[] ob_arr2 = new object[1];
        static UnityEditor.IMGUI.Controls.TreeViewItem tiv;
        Dictionary<object, UnityEditor.IMGUI.Controls.TreeViewItem> __ti = new Dictionary<object, UnityEditor.IMGUI.Controls.TreeViewItem>();
        UnityEditor.IMGUI.Controls.TreeViewItem emptyreeitem = new UnityEditor.IMGUI.Controls.TreeViewItem();

        internal UnityEditor.IMGUI.Controls.TreeViewItem GetTreeItem(int id)
        {
            // var tree = m_TreeViewontroller();
            var tree = TreeController_current;
            //var tree = GetTreeViewontroller(pluginID);
            if (tree == null) return null;

            if (!__ti.TryGetValue(tree, out tiv) || tiv == null || tiv.id != id)
            {

                var data = _data.GetValue(tree);
                ob_arr2[0] = id;
                // ob_arr2[ 1 ] = data_m_RootItem.GetValue( data, null );
                var res = data_FindItem_Slow.Invoke(data, ob_arr2) as UnityEditor.IMGUI.Controls.TreeViewItem;

                if (__ti.ContainsKey(tree))
                    __ti[tree] = res;
                else
                    __ti.Add(tree, res);
            }
            return tiv ?? emptyreeitem;
        }


        internal object GetSubHierarchy(int pid, EditorWindow w = null)
        {
            if (UseRootWindow) return m_TreeViewFieldInfo.GetValue(w ?? window.Instance);
            var sub = _SceneHierarchy.GetValue(w ?? window.Instance);
            return sub;
        }


        internal object GetTreeViewontroller(int pid, EditorWindow win = null)
        {

            if (!win)
            {
                var _w_temp = Window._windowsList.FirstOrDefault(w => w.pluginID == pid && w.thisIsAFirstWindow);
                if (_w_temp == null) _w_temp = Window._windowsList.FirstOrDefault(w => w.pluginID == pid);
                if (_w_temp == null) return null;
                win = _w_temp.Instance;
                if (!win) return null;
            }

            if (pid == 0)
            {
                if (UseRootWindow) return m_TreeViewFieldInfo.GetValue(win);
                var sub = _SceneHierarchy.GetValue(win);
                return m_TreeViewFieldInfo.GetValue(sub);
            }
            else
            {
                return m_TreeViewFieldInfoForProject(win).GetValue(win);
            }

        }

        internal FieldInfo m_TreeViewFieldInfo {
            get {
                return _TreeViewController_0;
                //return pluginID == 0 ? _TreeViewController_0 : _TreeViewController_1;
                // if ( !window( true ) ) return _FoldView;

            }
        }
        FieldInfo m_TreeViewFieldInfoForProject(EditorWindow w)
        {
            //return _AssetsView;


            if (ProjectViewMode(w) == 1) return _FoldView;
            return _AssetsView;
        }
        internal int ProjectViewMode(EditorWindow w)
        {
            return (int)_ViewMode.GetValue(w);
        }

        internal Type SceneHierarchyWindow(int pid)
        {
            //get
            {
                if (pid == 0)
                {
                    if (_SceneHierarchyWindow == null)
                    {
                        var ass = Assembly.GetAssembly(typeof(EditorWindow));
                        _SceneHierarchyWindow = ass.GetType("UnityEditor.SceneHierarchy");
                        if (UseRootWindow_0 = _SceneHierarchyWindow == null) { if ((_SceneHierarchyWindow = ass.GetType("UnityEditor.SceneHierarchyWindow")) == null) throw new Exception("UnityEditor.SceneHierarchyWindow"); }
                        else if ((_SceneHierarchy = SceneHierarchyWindowRoot.GetField("m_SceneHierarchy", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("m_SceneHierarchy");
                    }
                    return _SceneHierarchyWindow;
                }
                else
                {
                    if (_ProjectWindow == null)
                    {
                        var ass = Assembly.GetAssembly(typeof(EditorWindow));
                        if ((_ProjectWindow = ass.GetType("UnityEditor.ProjectBrowser")) == null) throw new Exception("UnityEditor.ProjectBrowser");
                        UseRootWindow_1 = true;
                    }
                    return _ProjectWindow;
                }

            }
        }
        Type _SceneHierarchyWindow;
        Type _ProjectWindow;
        FieldInfo _SceneHierarchy;
        internal Type SceneHierarchyWindowRoot {
            get {
                if (_SceneHierarchyWindowRoot == null)
                {
                    _SceneHierarchyWindowRoot = Assembly.GetAssembly(typeof(EditorWindow)).GetType(
                            pluginID == 0 ? "UnityEditor.SceneHierarchyWindow" : "UnityEditor.ProjectBrowser"
                    );
                }

                return _SceneHierarchyWindowRoot;
            }
        }
        Type _SceneHierarchyWindowRoot;
        Type _ProjectBrowserWindowType;
        internal Type ProjectBrowserWindowType {
            get {
                if (_ProjectBrowserWindowType == null)
                {
                    _ProjectBrowserWindowType = Assembly.GetAssembly(typeof(EditorWindow)).GetType("UnityEditor.ProjectBrowser"
                    );
                }

                return _ProjectBrowserWindowType;
            }
        }


        //	internal bool tempAdapterDisableCache = false;
        internal bool DisabledSavedData()
        {
            return false;//|| !wasAdapterInitalize;
        }

        internal void Modules_SetDirty()
        {

        }

        internal void Modules_RefreshBookmarks()
        {

        }


    }



}
