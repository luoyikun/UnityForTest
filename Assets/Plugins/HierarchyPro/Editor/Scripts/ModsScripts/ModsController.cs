using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;
using EMX.HierarchyPlugin.Editor.Mods;

namespace EMX.HierarchyPlugin.Editor
{


	interface IExternalWindowMod
    {
        bool Alive();
        bool RepaintNow();

        Func<bool, float, bool> currentAction { get; set; }
    }

    class EditorSubscriber
    {
        internal PluginInstance pluginInstance = null;
        internal EditorSubscriber( PluginInstance pluginInstance ) { 
            this.pluginInstance = pluginInstance;
            this.sbs_i = sbs_index++;
        }
        internal int sbs_i;
        static int sbs_index;


        internal Action OnSceneOpening = null;
        internal Action OnUndoAction = null;
        internal Action OnSelectionChanged = null;
        internal Action OnHierarchyChanged = null;
        internal Action OnPlayModeStateChanged = null;
        internal Action OnUpdate = null;
        internal Action<bool> OnGlobalKeyPressed = null;
        internal Action<string, Rect> OnProjectWindow = null;
        internal Action<SceneView> duringSceneGui = null;
        internal Action OnModifyKeyChanged = null;
        internal List<Func<bool>> OnEditorWantsToQuit = new List<Func<bool>>();
        internal Action OnAssetImport = null;
        internal Action OnResetDrawStack = null;
        internal Dictionary<AdditionalSubscriber, List<Action>> REBUILD_ADDITIONAL_EVENTS = new Dictionary<AdditionalSubscriber, List<Action>>();

        internal void copy_to( EditorSubscriber sbs )
        {
            if ( OnSceneOpening != null ) sbs.OnSceneOpening += OnSceneOpening;
            if ( OnUndoAction != null ) sbs.OnUndoAction += OnUndoAction;
            if ( OnSelectionChanged != null ) sbs.OnSelectionChanged += OnSelectionChanged;
            if ( OnHierarchyChanged != null ) sbs.OnHierarchyChanged += OnHierarchyChanged;
            if ( OnPlayModeStateChanged != null ) sbs.OnPlayModeStateChanged += OnPlayModeStateChanged;
            if ( OnUpdate != null ) sbs.OnUpdate += OnUpdate;
            if ( OnGlobalKeyPressed != null ) sbs.OnGlobalKeyPressed += OnGlobalKeyPressed;
            if ( OnProjectWindow != null ) sbs.OnProjectWindow += OnProjectWindow;
            if ( OnModifyKeyChanged != null ) sbs.OnModifyKeyChanged += OnModifyKeyChanged;
            //if (OnEditorWantsToQuit != null) sbs.OnEditorWantsToQuit += OnEditorWantsToQuit;
            if ( OnAssetImport != null ) sbs.OnAssetImport += OnAssetImport;
            if ( OnResetDrawStack != null ) sbs.OnResetDrawStack += OnResetDrawStack;
        }

        internal Action<Window> OnAssignWindowFirstFrame = null;

        internal List<Action> BuildedOnGUI_first = new List<Action>();
        internal List<PluginInstance.actions_pair> BuildedOnGUI_middle = new List<PluginInstance.actions_pair>();
        internal void AddBuildedOnGUI_middle( Action middle )
        {
            BuildedOnGUI_middle.Add( new PluginInstance.actions_pair() { middle = middle } );
        }
        internal void AddBuildedOnGUI_middle_plusLayout( Action middle, Action middle_plusLayout )
        {
            BuildedOnGUI_middle.Add( new PluginInstance.actions_pair() { middle = middle, middle_plusLayout = middle_plusLayout } );
        }
        // internal Action BuildedOnGUI_middle = null;
        // internal Action BuildedOnGUI_middle_plusLayout = null;
        internal Action BuildedOnGUI_last = null;
        internal Action BuildedOnGUI_last_butBeforeGL = null;

        internal List<IModSaver> saveModsInterator = new List<IModSaver>();

#if !EMX_H_LITE
        internal List<ExternalMod_Button> ExternalMod_Buttons = new List<ExternalMod_Button>();
        internal List<ExternalMod_MenuItem> ExternalMod_MenuItems = new List<ExternalMod_MenuItem>();
#endif
    }
#if !EMX_H_LITE
    class ExternalMod_Button
    {
        internal ExternalMod_Button( Type wint )
        {
            if ( !wint.IsSubclassOf( typeof( ExternalModRoot ) ) ) throw new Exception( wint.Name );
            this.windowType = wint;
        }

        internal Func<string> icon = null;
        internal string text = "Graph";
        internal Action<int, string> release = null;
        internal Action<GenericMenu, string, ExternalDrawContainer> menuGen = null;
        internal int priority = 0;
        internal bool? enabled = false;
        internal Type windowType = null;
        internal object additionObject= null;
    }
#endif
    class ExternalMod_MenuItem
    {
        internal string text = "Graph";
        internal int priority = 0;
        internal string path = "A/B";
        internal Action<int, string> release = null;
    }

    internal partial class ModsController
    {

        PluginInstance p;
        internal ModsController( PluginInstance p )
        {
            this.p = p;
            toolBarModification = new Mods.ToolBarModification( p );

            // funEditorFontsModification = new FunEditorFontsModification();
            backgroundDecorations = new Mods.BackgroundDecorations( 0 );

#if !EMX_H_LITE
            playModeKeeperMod = new Mods.PlayModeKeeperMod( p.pluginID );
#endif
            setActiveMod = new Mods.SetActiveMod( p.pluginID );
            rightModsManager = new Mods.RightModsManager( p );
            componentsIconsMod = new Mods.ComponentsIcons_Mod( p.pluginID );
#if !EMX_H_LITE
            highLighterMod = new Mods.HighlighterMod( p, 0 );

            activeExternalMods = new List<Mods.ExternalModRoot>();

            projectWindowExtensions = new HighlighterModForProject();
            bottomBarForHierarchy = new BottomBarExtension( p, 0 );
#endif
            /*externalMods.Add(ex_BookmarksforGameObjectsMod = new Mods.BookmarksforGameObjectsMod(p));
			externalMods.Add(ex_BookmarksforProjectviewMod = new Mods.BookmarksforProjectviewMod(p));
			externalMods.Add(ex_HyperGraphMod = new Mods.HyperGraph.HyperGraphMod(p));
			externalMods.Add(ex_LastScenesHistoryMod = new Mods.LastScenesHistoryMod(p));
			externalMods.Add(ex_LastSelectionHistoryMod = new Mods.LastSelectionHistoryMod(p));*/
        }

        internal Mods.ToolBarModification toolBarModification;

        //EDITOR
        // FunEditorFontsModification funEditorFontsModification;
        internal Mods.BackgroundDecorations backgroundDecorations;

        //MODS
        internal Mods.SetActiveMod setActiveMod;
        internal Mods.RightModsManager rightModsManager;
        internal Mods.ComponentsIcons_Mod componentsIconsMod;
#if !EMX_H_LITE
        internal Mods.PlayModeKeeperMod playModeKeeperMod;
        internal Mods.HighlighterMod highLighterMod;

        internal HighlighterModForProject projectWindowExtensions;

        internal List<Mods.ExternalModRoot> activeExternalMods = new List<Mods.ExternalModRoot>();
        /*	internal Mods.BookmarksforGameObjectsMod ex_BookmarksforGameObjectsMod;
			internal Mods.BookmarksforProjectviewMod ex_BookmarksforProjectviewMod;
			internal Mods.HyperGraph.HyperGraphMod ex_HyperGraphMod;
			internal Mods.LastScenesHistoryMod ex_LastScenesHistoryMod;
			internal Mods.LastSelectionHistoryMod ex_LastSelectionHistoryMod;*/

        //MODS

        internal AdditionalSubscriber externalModsAddisionSubscriber = new AdditionalSubscriber(){
            ID = 100
        };

        internal void INVOKE_FOR_EXTERNAL<T>( Action<T> ac ) where T : Mods.ExternalModRoot
        {
            foreach ( var item in activeExternalMods.ToList() )
            {
                if ( item is T )
                {
                    var r = item as T;
                    if ( !r ) continue;
                    ac( r );
                }
            }
        }


        
        internal BottomBarExtension bottomBarForHierarchy;

        List<Type> externalModsList = new List<Type>(){
            typeof (Mods.BookObject.BookmarksforGameObjectsModWindow),
            typeof (Mods.BookProject.BookmarksforProjectviewModWindow),
            typeof (ScenesHistoryModWindow),
            typeof (Mods.HyperGraph.HyperGraphModWindow),
            typeof (HierarchyExpandedMemWindow),
            typeof (GameObjectsSelectionHistoryModWindow),
        };

        internal void RebuildExternalModsWindows()
        {
            externalModsAddisionSubscriber.Clear();
            foreach ( var item in activeExternalMods )
                item.SubscribeEditorInstance( externalModsAddisionSubscriber );
        }

#endif


        internal void REBUILD_PLUGINS( bool skipRepaint = false )
        {

            

            if ( p.par_e.USE_WHOLE_FUN_UNITY_FONT_SIZE )
            {
                if ( Event.current == null )
                {
                    p.PUSH_GUI_ONESHOT_SKIP_CHECK_WINDOW_AWALIABLE( 0, () => {
                        FunEditorFontsModification.Modificate( p.par_e.WHOLE_FUN_UNITY_FONT_SIZE );
                    } );
                }
                else FunEditorFontsModification.Modificate( p.par_e.WHOLE_FUN_UNITY_FONT_SIZE );
            }

            var sbs = new EditorSubscriber(p);


            backgroundDecorations.SubscribeLast( sbs );//

#if !EMX_H_LITE
            highLighterMod.Subscribe( sbs ); //setactive mod //INVOKE BEFORE RIGHT MODS
#endif
            if ( p.par_e.USE_COMPONENTS_ICONS_MOD ) componentsIconsMod.Subscribe( sbs ); //setactive mod //INVOKE BEFORE RIGHT MODS

                                                    
			if ( p.par_e.USE_RIGHT_ALL_MODS ) rightModsManager.Subscribe( sbs ); //other right mods + custom

			//if ( p.par_e.HIER_WIN_SET.USE_BACKGROUNDDECORATIONS_MOD )
			//backgroundDecorations.Subscribe( sbs );// background colors mod //INVOKE FIRSTGUI ONLY AFTER RIGHT MODS CALCULATION
												   //COLOR BACKGROUND HERE

			if ( p.par_e.USE_SETACTIVE_MOD ) setActiveMod.Subscribe( sbs ); //setactive mod
#if !EMX_H_LITE
            if ( p.par_e.USE_PLAYMODE_SAVER_MOD ) playModeKeeperMod.Subscribe( sbs );  //playmodesaver mod
#endif
            if ( p.par_e.USE_RIGHT_ALL_MODS ) rightModsManager.SubscribePreCalc( sbs ); //other right mods + custom

			backgroundDecorations.SubscribeFirst( sbs );//



			if ( p.par_e.USE_PROJECT_GUI_EXTENSIONS ) (new Mods.ProjectGuiExtension( p )).Subscribe( sbs ); //PROJECT


            if ( p.par_e.USE_AUTOSAVE_MOD ) Mods.AutoSaveMod.Subscribe( sbs );
            if ( p.par_e.USE_SNAP_MOD ) Mods.SnapMod.Subscribe( sbs );






#if !EMX_H_LITE
            bottomBarForHierarchy.SubscribeEditorInstance( sbs );

            //EXTERNAL MODS
            //Mods.HyperGraph.HyperGraphModWindow.SubscribeButtonsAndMenu( sbs );
            //Mods.BookObject.BookmarksforGameObjectsModWindow.SubscribeButtonsAndMenu( sbs );
            //Mods.BookProject.BookmarksforProjectviewModWindow.SubscribeButtonsAndMenu( sbs );
            //Mods.GameObjectsSelectionHistoryModWindow.SubscribeButtonsAndMenu( sbs );
            //Mods.ScenesHistoryModWindow.SubscribeButtonsAndMenu( sbs );
            //Mods.HierarchyExpandedMemWindow.SubscribeButtonsAndMenu( sbs );
            // hierarchy header hot buttons
            //Mods.HyperGraph.HyperGraphMod.SubscribeButtonsAndMenu(sbs);
            foreach ( var item in externalModsList ) item.GetMethod( "SubscribeEditorInstanceStatic", (BindingFlags)~0 ).Invoke( null, new[] { sbs } );
            //  foreach ( var exMod in activeExternalMods.ToList() ) exMod.SubscribeEditorInstance( sbs );
            sbs.REBUILD_ADDITIONAL_EVENTS.Add( externalModsAddisionSubscriber, new List<Action>() { RebuildExternalModsWindows } );

#endif

            if ( p.par_e.USE_TOPBAR_MOD ) toolBarModification.Install( sbs, false );
            else toolBarModification.Remove( sbs );
            //toolBarModification.hotButtons.Subscribe(sbs);
            if ( p.par_e.USE_RIGHT_CLICK_MENU_MOD )
            {
                if ( !menuAdded )
                {
                    menuAdded = true;
                    try
                    {
                        EventInfo e = (typeof(EditorSceneManager).Assembly.GetType("UnityEditor.SceneManagement.SceneHierarchyHooks") ?? throw new Exception("Cannot find UnityEditor.SceneManagement.SceneHierarchyHooks")).GetEvent("addItemsToGameObjectContextMenu", ~(BindingFlags.Instance | BindingFlags.InvokeMethod)) ?? throw new Exception("Cannot find addItemsToGameObjectContextMenu");
                        MethodInfo mi = typeof(RightClickOnGameObjectMenuRegistrator).GetMethod("ContextMenuForPlugin_0", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new Exception("Cannot find ContextMenu");
                        var d = Delegate.CreateDelegate(e.EventHandlerType, null, mi);
                        object[] args = { d };
                        MethodInfo addHandler = e.GetAddMethod();
                        addHandler.Invoke( null, args );
                    }
                    catch ( Exception ex )
                    {
                        Debug.LogWarning( "Cannot create " + Root.HierarchyPro + " menu items\n" + ex.Message + "\n\n" + ex.StackTrace );
                    }
                }
                RightClickOnGameObjectMenuRegistrator.SubscribeEvents( sbs );
            }

#if !EMX_H_LITE
            projectWindowExtensions.Subscribe( sbs );
#endif

            foreach ( var item in sbs.REBUILD_ADDITIONAL_EVENTS )
                foreach ( var subscribe in item.Value )
                    subscribe();

            HierarchyTempGlobalData.SubscribeUndoActions( sbs );
            p._rebuild_editor_events( sbs );

            if ( !skipRepaint ) p.RepaintWindowInUpdate_PlusResetStack( 0 );
            if ( !skipRepaint ) p.RepaintWindowInUpdate_PlusResetStack( 1 );
        }


        internal void REBUILD_PLUGINS_FAST()
        {
            foreach ( var item in p._REBUILD_ADDITIONAL_EVENTS )
                foreach ( var subscribe in item.Value )
                    subscribe();
            p.RepaintWindowInUpdate_PlusResetStack( 0 );
            p.RepaintWindowInUpdate_PlusResetStack( 1 );
        }


        bool menuAdded = false;
        internal void RESET_DRAW_STACKS( int pluginID )
        {

            if ( pluginID == 0 )
            {
                setActiveMod.ResetStack();
                componentsIconsMod.ResetStack();
                rightModsManager.RESET_DRAW_STACKS();
#if !EMX_H_LITE
                playModeKeeperMod.ResetStack();
                highLighterMod.ResetStack();
#endif
            }
#if !EMX_H_LITE
            else
            {
                projectWindowExtensions.RESET_DRAW_STACKS();
            }
#endif


            // foreach ( var m in internalMods ) m.Value.ResetDrawStack();
            //  foreach ( var m in externalMods ) m.Value.ResetDrawStack();
            /*for ( int i = 0 ; i < modules.Length ; i++ )
            {
                foreach ( var stack in modules[ i ].DRAW_STACK )
                {
                    stack.Value.ResetStack();
                }
            }*/
        }




        internal List<IModSaver> saveModsInterator = new List<IModSaver>();








    }

    interface IModSaver
    {
        bool SaveToString( HierarchyObject o, ref string result );
        bool LoadFromString( string s, HierarchyObject o );
        List<SaverType> GetSaverTypes { get; }

    }


    internal class AdditionalSubscriber : IComparer<AdditionalSubscriber>, IEquatable<AdditionalSubscriber>
    {
        public int ID = 0;
        internal Action OnSceneOpening = null;
        internal Action OnSelectionChanged = null;
        internal Action OnPlayModeStateChanged = null;
        internal Action OnUpdate = null;
        internal Action OnAssetImport = null;
        internal void Clear()
        {
            OnSceneOpening = null;
            OnSelectionChanged = null;
            OnPlayModeStateChanged = null;
            OnUpdate = null;
        }

        void OnAssetImport_invoke()
        {
            if ( OnAssetImport != null ) OnAssetImport();
        }

        void OnSceneOpening_invoke()
        {
            if ( OnSceneOpening != null ) OnSceneOpening();
        }
        void OnSelectionChanged_invoke()
        {
        
            if ( OnSelectionChanged != null ) OnSelectionChanged();
        }
        void OnPlayModeStateChanged_invoke()
        {
            if ( OnPlayModeStateChanged != null ) OnPlayModeStateChanged();
        }
        void OnUpdate_invoke()
        {
            if ( OnUpdate != null ) OnUpdate();
        }

        internal void Subscribe( EditorSubscriber sbs )
        {
            sbs.OnSceneOpening += OnSceneOpening_invoke;
            sbs.OnSelectionChanged += OnSelectionChanged_invoke;
            sbs.OnPlayModeStateChanged += OnPlayModeStateChanged_invoke;
            sbs.OnUpdate += OnUpdate_invoke;
            sbs.OnAssetImport += OnAssetImport_invoke;
        }




        public int Compare( AdditionalSubscriber x, AdditionalSubscriber y )
        {
            return x.ID - y.ID;
        }
        public bool Equals( AdditionalSubscriber other )
        {
            return ID == other.ID;
        }
        public override bool Equals( object obj )
        {
            return ((AdditionalSubscriber)this).Equals( (AdditionalSubscriber)obj );
        }
        public override int GetHashCode()
        {
            return ID;
        }


    }


}
