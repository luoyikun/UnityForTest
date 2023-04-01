using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace EMX.HierarchyPlugin.Editor.Mods
{


	partial class RightModsManager
	{


		internal static Dictionary<CustomHierarchyMod, int> RegistrateRightCustomModDictionary = new Dictionary<CustomHierarchyMod, int>();
		internal static void RegistrateRightCustomMod( CustomHierarchyMod mod, int index )
		{
			//if ( !Root.p[ 0 ].par_e.ENABLE_ALL || !Root.p[ 0 ].par_e.USE_RIGHT_ALL_MODS || !Root.p[ 0 ].par_e.RIGHT_USE_CUSTOMMODULES ) return;
			index = Mathf.Clamp( index, 0, 8 );
			if ( RegistrateRightCustomModDictionary.ContainsKey( mod ) ) return;
			RegistrateRightCustomModDictionary.Add( mod, index );
			if ( Root.hasMainConstructorInit ) Root.p[ 0 ].modsController.REBUILD_PLUGINS();

			//var start = Root.p[0].modsController.rightModsManager.rightMods.ToList().FindIndex(m => m.GetType().Name == "M_UserModulesRoot_Slot" + (index + 1).ToString());
			//Root.p[ 0 ].modsController.rightModsManager.rightMods[ start ].SetCustomModule( mod );
		}

		internal void SubscribePreCalc( EditorSubscriber sbs )
		{
			headerEventsBlockRect = null;
			sbs.BuildedOnGUI_first.Add( PreCalcRect );
			sbs.OnHierarchyChanged += OnHC;
		}
		internal void Subscribe( EditorSubscriber sbs )
		{

			sbs.AddBuildedOnGUI_middle( Draw_Middle );
			sbs.BuildedOnGUI_last += Draw_Last;
			sbs.OnGlobalKeyPressed += modifierKeysChangedFix;
			rightMods = CreateInternalModules( sbs.pluginInstance );

			if ( Root.p[ 0 ].par_e.ENABLE_ALL && Root.p[ 0 ].par_e.USE_RIGHT_ALL_MODS && Root.p[ 0 ].par_e.RIGHT_USE_CUSTOMMODULES )
				foreach ( var item in RegistrateRightCustomModDictionary )
				{
					CustomHierarchyMod mod = item.Key; int index = item.Value;
					var start = Root.p[0].modsController.rightModsManager.rightMods.ToList().FindIndex(m => m.GetType().Name == "M_UserModulesRoot_Slot" + (index + 1).ToString());
					Root.p[ 0 ].modsController.rightModsManager.rightMods[ start ].SetCustomModule( mod );
				}

			foreach ( var m in rightMods )
			{
				if ( !m.savedData.enabled ) continue;
				m.Subscribe( sbs );
			}

		}

		internal bool CheckSpecialButtonIfRightHidingEnabled()
		{
			if ( p.par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX == 0 || !p.par_e.USE_RIGHT_ALL_MODS || !p.par_e.RIGHT_USE_HIDE_ISTEAD_LOCK ) return false;
			if ( Event.current == null ) return false;
			switch ( p.par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 3 )
			{
				case 1: return !Event.current.alt;
				case 2: return !Event.current.shift;
				case 3: return !Event.current.control;
			}
			return false;
		}
		internal void RESET_DRAW_STACKS()
		{
			foreach ( var m in rightMods )
			{
				m.ResetStack();
			}
		}




		// int modLim =0;
		bool TEMP_HIDE;
		public bool baked_SHRINK_BUTTONS, baked_CHANGE_CURSOR;



		internal const int MIN_MOD_WIDTH = 8;
		static Color colCache;
		internal static Color grayFreezee = new Color(0.2f, 0.2f, 0.2f, 1);
		GUIContent tCont = new GUIContent();
		GUIContent PropContent = new GUIContent() { tooltip = "Show category list\nDrag the icon to change width" };
		//Rect headerDrawRect;
		internal Rect? headerEventsBlockRect;
		/*#pragma warning disable
                bool header_event_were;
        #pragma warning restore*/
		Color a0 = new Color(1, 1, 1, 0);
		void PreCalcRect()
		{

			baked_SHRINK_BUTTONS = p.par_e.RIGHTDOCK_SHRINK_BUTTONS_INT != 0;
			baked_CHANGE_CURSOR = p.par_e.RIGHTDOCK_CHACGE_CURSOR;
		

			TEMP_HIDE = p.par_e.RIGHTDOCK_TEMPHIDE && p.window.position.width <= p.par_e.RIGHTDOCK_TEMPHIDEMINWIDTH || p.ha.IS_PREFAB_MOD_OPENED();
			if ( TEMP_HIDE ) return;

			var theaderDrawRect = p.fullLineRect;
			if ( !p.par_e.RIGHT_HEADER_BIND_TO_SCENE_LINE && EditorSceneManager.sceneCount < 2 ) theaderDrawRect.y = Mathf.RoundToInt( p.scrollPos.y );
			else theaderDrawRect.y = 0;



			/*       var header_event_were = false;
                var  LastHeaderRect = _dr;
                LastHeaderRect.width = headerRect.width + headerRect.x - LastHeaderRect.x;
                if ( LastHeaderRect.Contains( p.EVENT.mousePosition ) && p.EVENT.isMouse ) header_event_were = true;*/


			AllocateRightMods( theaderDrawRect );


			var c = GUI.color;
			GUI.color *= a0;

			//Debug.Log( p.EVENT.type + " " + theaderDrawRect  + " " + p.rightOffset);

			if ( p.baked_HARD_BAKE_ENABLED ) FadeRightModRect( theaderDrawRect );
			headerEventsBlockRect = DrawHeader( theaderDrawRect );

			// if ( headerEventsBlockRect.Value.Contains( p.EVENT.mousePosition ) ) Tools.EventUse();
			if ( (p.EVENT.type == EventType.MouseDown || p.EVENT.type == EventType.MouseUp) &&
				headerEventsBlockRect.Value.Contains( p.EVENT.mousePosition ) ) Tools.EventUseFast();
			GUI.color = c;

			//headerEventsBlockRect.y = theaderDrawRect.height;
			//Debug.Log( headerEventsBlockRect );
			//	EditorGUI.DrawRect( headerEventsBlockRect.Value, Color.white );


			//var HIDE_MODULES = p.modsController.rightModsManager.CheckSpecialButtonIfRightHidingEnabled();
			//if ( !HIDE_MODULES )
			//{
			//	var fadeRect = headerEventsBlockRect.Value;
			//	fadeRect.y = p._first_FullLineRect.y;
			//	fadeRect.height = p._last_FullLineRect.y + p._last_FullLineRect.height - p._first_FullLineRect.y;
			//	if ( !HIDE_MODULES ) PluginInstance.FadeRect( fadeRect, p.par_e.RIGHT_BG_OPACITY );
			//}

			last_tempoasdasdasd = theaderDrawRect;
			//p.window.drawHeadRectMem = theaderDrawRect;
		}


		void Draw_Middle()
		{
			if ( TEMP_HIDE ) return;
			// if (header_event_were) return; //maybe hot control warning ;; if no - uncomment
			DrawModulesContent();
		}

		Rect last_tempoasdasdasd;
		void Draw_Last()
		{
			if ( TEMP_HIDE ) return;

			if ( p.EVENT.type != EventType.Repaint ) return;

			var HIDE_MODULES = p.modsController.rightModsManager.CheckSpecialButtonIfRightHidingEnabled();
			if ( !HIDE_MODULES ) DrawVerticalLines();


			if ( p.par_e.RIGHT_HEADER_BG_OPACITY != 0 )
			{
				var casd = Colors.EditorBGColor;
				casd.a = p.par_e.RIGHT_HEADER_BG_OPACITY;
				var r = headerEventsBlockRect.Value;
				r.x -= 6;
				r.width += 6;
				EditorGUI.DrawRect( r, casd );
			}

			//p.gl._DrawTexture( headerEventsBlockRect.Value, ref casd );

			DrawHeader( last_tempoasdasdasd );




		}


		static int START_W = 16;

		// internal ComponentsIcons_Mod componentsIconsMod;
		internal RightModBaseClass[] rightMods;
		PluginInstance p;
		internal RightModsManager( PluginInstance plugin )
		{
			p = plugin;
			/* var t = GetType();
             if ( Root.p[ TargetPlugin ].ActiveMods.Any( m => m.Value.GetType() == t ) )
             {
                 var r= Root.p[TargetPlugin].ActiveMods.First(m=>m.Value.GetType() == t );
                 Root.p[ TargetPlugin ].ActiveMods.Remove( r.Key );
             }
             var o = TargetOrder;
             while ( Root.p[ TargetPlugin ].ActiveMods.ContainsKey( o ) ) o++;
             Root.p[ TargetPlugin ].ActiveMods.Add( o, this );*/




			rightMods = CreateInternalModules( plugin );

		}



		RightModBaseClass[] CreateInternalModules( PluginInstance plugin )
		{
			return new RightModBaseClass[]
		 {
           /* new M_SetActive(START_W, -1, true, plugin)
            { // SearchHelper = "Show 'GameObjects' whose 'Components' will persist in play mode",
                HeaderText = "SetActive GameObject",
                ContextHelper = "SetActive GameObject",
                // HeaderTexture2D = "STORAGE_PASSIVE",
                // disableSib = true
            },*/
          /*  HierarchyAdapterInstance.ColorModule ?? new Adapter.M_Colors(START_W, -1, true, HierarchyAdapterInstance)
            {
                SearchHelper = "Show 'GameObjects' with",
                ContextHelper = "Change GameObject Icon"
            },
            new M_Warning(START_W, -1, false, HierarchyAdapterInstance),*/
            new Mod_Freeze(START_W, 0, false, plugin)
			{
				SearchHelper = "Show locked 'gameobjects'",
				ContextHelper = "Use for lock/unlock selecting object",
				HeaderText = "Lock Toggle",
				HeaderTexture2D = "LOCK"
			},
			new Mod_PrefabApply(START_W, 1, false, plugin)
			{
				SearchHelper = "Show prefabs",
				ContextHelper = "Fast apply prefab changes",
				HeaderText = "Prefab Button",
				HeaderTexture2D = "PREF"
			},
			new Mod_Vertices(START_W * 2, 2,true /*SystemInfo.processorFrequency < 3000 ? false : true*/, plugin)
			{
				SearchHelper = "Show 'gameobjects' memory info",
				ContextHelper = "Memory info",
				HeaderText = "Memory Info",
				HeaderTexture2D = "TRI"
			},
			new Mod_Audio(START_W, 3, false, plugin)
			{
				SearchHelper = "Show 'gameobjects' with audiosource",
				ContextHelper = "Play AudioClip",
				HeaderText = "Audio Player",
				HeaderTexture2D = "AUDIO"
			},
			new Mod_Tag(START_W * 2, 4, true, plugin)
			{
				SearchHelper = "Show 'gameobjects' with tag",
				HeaderText = "Tags",
				ContextHelper = "Which tag was assigned to object",
			},
			new Mod_Layers(48, 5, false, plugin) //DISABLE
            {
				SearchHelper = "Show 'gameobjects' with layer",
				HeaderText = "Layers",
				ContextHelper = "Which layer was assigned to object"
			},
         /*   HierarchyAdapterInstance.M_CustomIconsModule ?? new M_CustomIcons(48, 6, true, HierarchyAdapterInstance)
            { //  SearchHelper = "Show GameObjects Which Component With",
                SearchHelper = "Show 'GameObjects' whose 'Components' include",
                HeaderText = "Components",
                ContextHelper = "Custom component icons",
                DRAW_AS_COLUMN = () => !HierarchyAdapterInstance.par.COMPONENTS_NEXT_TO_NAME
            },*/
            new Mod_Descript(68, 8, true, plugin)
			{
				SearchHelper = "Show 'gameobjects' with description",
				HeaderText = "Descriptions",
				ContextHelper = "Short object description",
			},
			new Mod_SpritesOrder(68, 7, false, plugin)
			{
				SearchHelper = "Show 'gameobjects' with sortingLayer",
				HeaderText = "Sprites Order",
				ContextHelper = "SortingLayer and order for sprites",
			},
          //  GET_KEEPER,

            new M_UserModulesRoot_Slot1(68, 9, false, plugin)
			{
				SearchHelper = "Show 'gameobjects' whose have Custom Modules parameters",
				HeaderText_pref = "Custom 1",
				ContextHelper_pref = "Custom Module 1",
            //}/*.SetCustomModule(plugin.m1 == null ? null : Activator.CreateInstance(plugin.m1) as CustomHierarchyMod) as RightModBaseClass*/
        },

			new M_UserModulesRoot_Slot2(68, 10, false, plugin)
			{
				SearchHelper = "Show 'gameobjects' whose have Custom Modules parameters",
				HeaderText_pref = "Custom 2",
				ContextHelper_pref = "Custom Module 2",
           // }.SetCustomModule(plugin.m2 == null ? null : Activator.CreateInstance(plugin.m2) as CustomHierarchyMod) as RightModBaseClass,
        },
			new M_UserModulesRoot_Slot3(68, 11, false, plugin)
			{
				SearchHelper = "Show 'gameobjects' whose have Custom Modules parameters",
				HeaderText_pref = "Custom 3",
				ContextHelper_pref = "Custom Module 3",
          //  }.SetCustomModule(plugin.m3 == null ? null : Activator.CreateInstance(plugin.m3) as CustomHierarchyMod) as RightModBaseClass,
        },

			new M_UserModulesRoot_Slot4(68, 12, false, plugin)
			{
				SearchHelper = "Show 'gameobjects' whose have Custom Modules parameters",
				HeaderText_pref = "Custom 4",
				ContextHelper_pref = "Custom Module 4",
         //   }.SetCustomModule(plugin.m4 == null ? null : Activator.CreateInstance(plugin.m4) as CustomHierarchyMod) as RightModBaseClass,
        },
			new M_UserModulesRoot_Slot5(68, 13, false, plugin)
			{
				SearchHelper = "Show 'gameobjects' whose have Custom Modules parameters",
				HeaderText_pref = "Custom 5",
				ContextHelper_pref = "Custom Module 5",
          //  }.SetCustomModule(plugin.m5 == null ? null : Activator.CreateInstance(plugin.m5) as CustomHierarchyMod) as RightModBaseClass,
        },
			new M_UserModulesRoot_Slot6(68, 14, false, plugin)
			{
				SearchHelper = "Show 'gameobjects' whose have Custom Modules parameters",
				HeaderText_pref = "Custom 6",
				ContextHelper_pref = "Custom Module 6",
          //  }.SetCustomModule(plugin.m6 == null ? null : Activator.CreateInstance(plugin.m6) as CustomHierarchyMod) as RightModBaseClass,
        },

			new M_UserModulesRoot_Slot7(68, 15, false, plugin)
			{
				SearchHelper = "Show 'gameobjects' whose have Custom Modules parameters",
				HeaderText_pref = "Custom 7",
				ContextHelper_pref = "Custom Module 7",
           // }.SetCustomModule(plugin.m7 == null ? null : Activator.CreateInstance(plugin.m7) as CustomHierarchyMod) as RightModBaseClass,
        },
			new M_UserModulesRoot_Slot8(68, 16, false, plugin)
			{
				SearchHelper = "Show 'gameobjects' whose have Custom Modules parameters",
				HeaderText_pref = "Custom 8",
				ContextHelper_pref = "Custom Module 8",
           // }.SetCustomModule(plugin.m8 == null ? null : Activator.CreateInstance(plugin.m8) as CustomHierarchyMod) as RightModBaseClass,
        },
			new M_UserModulesRoot_Slot9(68, 17, false, plugin)
			{
				SearchHelper = "Show 'gameobjects' whose have Custom Modules parameters",
				HeaderText_pref = "Custom 9",
				ContextHelper_pref = "Custom Module 9",
           // }.SetCustomModule(plugin.m9 == null ? null : Activator.CreateInstance(plugin.m9) as CustomHierarchyMod) as RightModBaseClass,
        },
		 };

		}

		bool oldPrev = false;
		bool OLD_SHIFT, OLD_ALT, OLD_CTRL;

		private void modifierKeysChangedFix( bool used ) //if ( Application.isPlaying ) return;
		{


			var DO = Event.current != null && (Event.current.rawType == EventType.KeyDown || Event.current.rawType == EventType.KeyUp);
			if ( !DO ) return;



			var k = Event.current.keyCode;
			var mask = KeyCode.LeftAlt | KeyCode.RightAlt | KeyCode.LeftShift | KeyCode.RightShift | KeyCode.LeftControl | KeyCode.RightControl;

			if ( (k & mask) == 0 ) return;


			var alt = OLD_ALT != Event.current.alt;
			OLD_ALT = Event.current.alt;
			var shift = OLD_SHIFT != Event.current.shift;
			OLD_SHIFT = Event.current.shift;
			var control = OLD_CTRL != Event.current.control;
			OLD_CTRL = Event.current.control;

			//Debug.Log(EditorWindow.focusedWindow.GetType() + " " + hashoveredItem + " " + hoverID);

			if ( (control || shift || alt) && (!EditorWindow.focusedWindow || !EditorWindow.focusedWindow.GetType().Name.Contains( "SceneView" ) && !EditorWindow.focusedWindow.GetType().Name.Contains( "GameView" )
																		  || !p.hashoveredItem || p.hoverID != -1)
			)

				if ( p.par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY )
				{
					var re = p.par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 3;
					var prev = false;

					if ( re == 1 && alt )
					{
						prev = true;
					}

					if ( re == 2 && shift )
					{
						prev = true;
					}

					if ( re == 3 && control )
					{
						prev = true;
					}

					//if (oldPrev != prev)
					{
						p.RESET_DRAWSTACK( 0 );
						p.RepaintWindowInUpdate( 0 );
					}

					oldPrev = prev;


				}

			if ( p.par_e.RIGHT_DRAW_MODS_FOR_SELECTED_ONLY && !p.par_e.RIGHT_USE_HIDE_ISTEAD_LOCK && alt &&
				((!EditorWindow.focusedWindow || !EditorWindow.focusedWindow.GetType().Name.Contains( "SceneView" ) && !EditorWindow.focusedWindow.GetType().Name.Contains( "GameView" ))
				 || p.hashoveredItem && p.hoverID != -1
				)
			)
			{
				p.RESET_DRAWSTACK( 0 );
				p.RepaintWindowInUpdate( 0 );
			}

			//Debug.Log( SHIFT_TO_INSTANTIATE_BOTTOM + " " + shift  + " " +  EditorWindow.focusedWindow   + " " +  EditorWindow.focusedWindow.GetType().Name.Contains( "SceneView") );

			/*  if ( SHIFT_TO_INSTANTIATE_BOTTOM && shift && EditorWindow.focusedWindow && EditorWindow.focusedWindow.GetType().Name.Contains( "SceneView" ) )
              {
                  RepaintWindowInUpdate();
              }*/

			/*	if (EditorWindow.focusedWindow && ((control || shift || alt) &&
                                                   (EditorWindow.focusedWindow.GetType().Name.Contains( "SceneHierarchy" )
                                                    ||  EditorWindow.focusedWindow.GetType().Name.Contains( "Project" )
                                                    ||  EditorWindow.focusedWindow.GetType().Name.Contains( "Inspector" )
                                                    || hashoveredItem && hoverID != -1
                                                   ) ||
                                                   control  &&EditorWindow.focusedWindow.GetType().Name.Contains( "SceneView")
                                                  )
                   )
                    RepaintWindowInUpdate();*/
		}


	}
}
