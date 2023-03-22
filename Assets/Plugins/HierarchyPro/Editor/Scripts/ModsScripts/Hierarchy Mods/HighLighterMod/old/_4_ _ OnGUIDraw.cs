using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EMX.HierarchyPlugin.Editor.Windows;

namespace EMX.HierarchyPlugin.Editor.Mods
{



	//class HighlighterAdapter
	//{
	//	internal Func<bool> USE_MANUAL_HIGHLIGHTER_MOD;
	//	internal Func<bool> USE_AUTO_HIGHLIGHTER_MOD;
	//	internal Func<bool> USE_CUSTOM_PRESETS_MOD;
	//}




	internal partial class HighlighterMod : DrawStackAdapter, IModSaver, ISearchable
	{


		internal HighlighterAutoMode_Instance autoMod;

		internal string _SearchHelper = "Show Objects with Icons";
		public override bool callFromExternal() { return callFromExternal_objects != null; }
		public Windows.SearchWindow.FillterData_Inputs callFromExternal_objects { get; set; }
		public Type typeFillter { get; set; }
		public string SearchHelper { get { return _SearchHelper; } set { _SearchHelper = value; } }
		public virtual float GetInputWidth() { return -1; }
		//SEARCH
		internal Event EVENT { get { return callFromExternal() ? Event.current : adapter.EVENT; } }
		public void DrawSearch( Rect rect, HierarchyObject o )
		{
			/*_drawRect = rect;
			adapter.o = o;
			Draw();*/
		}

		internal override bool PERFOMANCE_BARS { get { return base.PERFOMANCE_BARS && !callFromExternal(); } }





		internal int pluginID;
		internal PluginInstance adapter;

		internal HighlighterMod( PluginInstance p, int pid ) : base( p.pluginID )
		{
			pluginID = pid;
			adapter = p;
		}
		GlDrawer momentumGl;
		internal void Subscribe( EditorSubscriber sbs )
		{

			_s( sbs, adapter.par_e.HIER_WIN_SET, adapter.par_e.HIER_HIGH_SET );



		}

		//internal HighlighterAdapter root;
		internal EditorSettingsAdapter.HighlighterSettings set;
		internal EditorSettingsAdapter.WindowSettings wse;
		internal void _s( EditorSubscriber sbs, EditorSettingsAdapter.WindowSettings wse, EditorSettingsAdapter.HighlighterSettings set )
		{
			this.set = set;
			this.wse = wse;
			bool M = set.USE_MANUAL_HIGHLIGHTER_MOD;
			bool A = set.USE_AUTO_HIGHLIGHTER_MOD;
			bool P = set.USE_CUSTOM_PRESETS_MOD;

			if ( !P && !M && !A ) return;

			autoMod = new HighlighterAutoMode_Instance( set.pluginID );
			momentumGl = new GlDrawer( Root.p[ 0 ] ) { disableBatching = true };
			//_IS_HIERARCHY = sbs.pid == 0;
			// sbs.OnPlayModeStateChanged += PlaymodeStateChanged;
			if ( M || A )
			{
				sbs.BuildedOnGUI_first.Add( FirstFrame);
				//sbs.BuildedOnGUI_last_butBeforeGL += DrawLabelsInLastFrame;
				sbs.AddBuildedOnGUI_middle_plusLayout( null, prepare_hierarchy_draw );
				// sbs.BuildedOnGUI_middle_plusLayout += prepare_hierarchy_draw;
				sbs.saveModsInterator.Add( this );

				//manualMod.SubscribeAutoColorHighLighter(sbs);
				//sbs.OnResetDrawStack += ResetStack;

				sbs.OnUndoAction += OnUndoAction;
				sbs.OnSceneOpening += OnUndoAction;
				//sbs.OnSelectionChanged += OnUndoAction;
				sbs.OnHierarchyChanged += ResetStack;

				autoMod.SubscribeAutoColorHighLighter( sbs, set );
				//sbs.saveModsInterator.Add(this);

			}

			if ( M || A || P )
			{
				sbs.AddBuildedOnGUI_middle_plusLayout( null, Draw_Button_Action );
				// sbs.BuildedOnGUI_middle_plusLayout += Draw_Button_Action;
			}

			ClearCacheAdditional();
		}


		void OnUndoAction()
		{
			ClearCacheAdditional();
		}



		void prepare_hierarchy_draw()
		{
			m_Main( adapter.o, adapter.selectionRect );
		}




		//internal override void ResetStack(int id, bool disableLog = false)
		//{
		//	foreach (var nc in new_child_cache_dic)
		//	{
		//		nc.Value.wasLastAssign = false;
		//	}
		//	base.ResetStack(id, disableLog);
		//	ICON_STACK.ResetStack(id, disableLog);
		//}


		//bool _IS_HIERARCHY;
		bool IS_HIERARCHY() { return adapter.pluginID == 0; }
		bool IS_PROJECT() { return adapter.pluginID == 1; }
		bool HAS_LABEL_ICON() { return true; }

		internal void ClearCacheAdditional()
		{

			//addIconChecherCache.Clear();
			//cached_colors.Clear();
			ClearGroupingCache();

			HighlighterCache_Colors.cacheDichighlighter.Clear();
			HighlighterCache_Icons.new_perfomance_onlycaher_icons_if.Clear();
			HighlighterCache_Icons.new_perfomance_onlycaher_icons_sf.Clear();
			HighlighterCache_Icons.new_perfomance_includecaher_icons.Clear();

			//if (IconColorCacher != null) IconColorCacher.cacheDic.Clear();
			//if (additionalClear != null) additionalClear();

			//Hierarchy.ClearM_CustomIconsCache(); // nulll and missings
			//HighlighterCache_Icons.icon_cacher.Clear(); //IconExternalData

			ResetStack();
		}
		internal void ClearByObject( HierarchyObject o )
		{
			//HighlighterDrawWorks.ClearGroupingCache();
			o.ah.filterAssigned = false; // replaced clear group

			HighlighterCache_Colors.cacheDichighlighter.Remove( o.id );
			HighlighterCache_Icons.new_perfomance_onlycaher_icons_if.Remove( o.id );
			HighlighterCache_Icons.new_perfomance_onlycaher_icons_sf.Remove( o.id );
			HighlighterCache_Icons.new_perfomance_includecaher_icons.Remove( o.id );

			ResetStack();
			adapter.RepaintWindowInUpdate( adapter.pluginID );
		}

		internal void ClearGroupingCache()
		{
			foreach ( var nc in new_child_cache_dic )
			{
				nc.Value.wasInit = false;
				nc.Value.wasLastAssign = false;
			}
		}


		/*internal void ClearCacheAdditional()
	{	addIconChecherCache.Clear();
		cached_colors.Clear();
		ClearGroupingCache();
		adapter.RESET_COLOR_STACKS();
		new_perfomance_onlycaher_icons.Clear();
		Utilities.new_perfomance_includecaher_icons.Clear();
		if (cacheDichighlighter != null) cacheDichighlighter.Clear();
		if (IconColorCacher != null) IconColorCacher.cacheDic.Clear();
		if (additionalClear != null) additionalClear();
		Hierarchy.ClearM_CustomIconsCache();
	}*/

		/*
	void ClearGroupingCache()
	{
		foreach (var nc in adapter.new_child_cache_dic)
		{ 
			nc.Value.wasInit = false;
			nc.Value.wasLastAssign = false;
		}

		//k  Debug.Log( "ASD" );
	}
*/

		internal float COLOR_ICON_SIZE {
			get {
				return adapter.DEFAULT_ICON_SIZE;
				//return DEFAULT_ICON_SIZE;
			}
		}
		internal float DEFAULT_ICON_SIZE {
			get {
				return adapter.DEFAULT_ICON_SIZE;
				//return DEFAULT_ICON_SIZE;
			}
		}
		int _S_bgIconsPlace {
			get { return set.HIGHLIGHTER_CUSTOM_ICON_LOCATION; }
		}
		//	int _S_bgButtonForIconsPlace
		//	{
		//		get { return set.HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION; }
		//	}

		//internal float DEFAULT_ICON_SIZE
		//{
		//	get
		//	{
		//		float res;
		//
		//		if (par.USEdefaultIconSize) res = par.defaultIconSize;
		//		else res = EditorGUIUtility.singleLineHeight;
		//
		//		// return Mathf.Min(par.LINE_HEIGHT, res);
		//		return res;
		//	}
		//}


		//internal DynamicRect _TryToFaeBG_Rect = (null);
		//internal DynamicRect TryToFaeBG_Rect { get { return _TryToFaeBG_Rect ?? (_TryToFaeBG_Rect = new );} }
		internal TempColorClass __INTERNAL_TempColor_Empty = new TempColorClass().AddIcon(null);

		static GUIStyle __foldoutStyle;
		static internal GUIStyle foldoutStyle {
			get { return __foldoutStyle ?? (__foldoutStyle = (GUIStyle)"IN Foldout"); }
		}
		static internal float foldoutStyleWidth {
			get { return foldoutStyle.fixedWidth + 2; }
		}
		static internal float foldoutStyleHeight {
			get { return foldoutStyle.fixedHeight != 0 ? foldoutStyle.fixedHeight : EditorGUIUtility.singleLineHeight; }
		}
		internal bool LocalIsSelected( HierarchyObject id )
		{
			//	if (!set.USE_MANUAL_HIGHLIGHTER_MOD) return false;
			return adapter.ha.IsSelected( id );
		}








		public bool LoadFromString( string s, HierarchyObject o )
		{
			/*	TempSceneObjectPTR data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModManualHighligher, o);
				if (data == null) data = new TempSceneObjectPTR(o.go, -1);
				Array.Resize(ref data.highLighterData, 1);
				data.highLighterData[0].Load(s);
				HierarchyTempSceneDataGetter.SetObjectData(SaverType.ModManualHighligher, o, data);*/

			if ( o.pluginID == 1 ) throw new Exception( "LoadFromString for project exception" );

			if ( s == null || s == "" ) return false;
			var split = s.Split('½');
			if ( split.Length != 2 ) return false;
			if ( split[ 0 ] != "" )
			{
				TempSceneObjectPTR data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModManualHighligher, o);
				if ( data == null ) data = new TempSceneObjectPTR( o.go, -1 );
				if ( data.highLighterData.Length < 1 )
				{
					Array.Resize( ref data.highLighterData, 1 );
					data.highLighterData[ 0 ] = new HighlighterExternalData();
				}
				data.highLighterData[ 0 ].Load( split[ 0 ] );
				HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModManualHighligher, o, data, true );
			}
			if ( split[ 1 ] != "" )
			{
				TempSceneObjectPTR data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModManualIcons, o);
				if ( data == null ) data = new TempSceneObjectPTR( o.go, -1 );
				if ( data.iconData.Length < 1 )
				{
					Array.Resize( ref data.iconData, 1 );
					data.iconData[ 0 ] = new IconExternalData();
				}
				data.iconData[ 0 ].Load( split[ 1 ] );
				HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModManualIcons, o, data, true );
			}
			return true;
		}

		List<SaverType> _GetSaverTypes = new List<SaverType>() { SaverType.ModManualHighligher, SaverType.ModManualIcons };
		List<SaverType> IModSaver.GetSaverTypes { get { return _GetSaverTypes; } }
		public bool SaveToString( HierarchyObject o, ref string result )
		{
			if ( o.pluginID == 1 ) throw new Exception( "SaveToString for project exception" );

			TempSceneObjectPTR data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModManualHighligher, o);
			bool changed = false;

			if ( data != null && data.highLighterData.Length != 0 )
			{
				var save = data.highLighterData[0].Save();
				if ( save != null )
				{
					result = save;
					changed |= true;
				}
			}
			result += '½';
			data = HierarchyTempSceneDataGetter.GetObjectData( SaverType.ModManualIcons, o );
			if ( data != null && data.iconData.Length != 0 )
			{
				var save = data.iconData[0].Save();
				if ( save != null )
				{
					result += save;
					changed |= true;
				}
			}
			return changed;
			// throw new NotImplementedException();
		}

		TempColorClass _tempTempColorClass = new TempColorClass();
		internal TempColorClass GetSavedColor( HierarchyObject h )
		{
			if ( h == null ) return null;
			//if ( h.name == "light_probe" )
			//	Debug.Log( "ASD" );
			if ( set.USE_AUTO_HIGHLIGHTER_MOD || set.USE_MANUAL_HIGHLIGHTER_MOD  )
			{
				//if ( h.ah.localTempColor != null ) return h.ah.localTempColor;
				//if ( h.ah.MIXINCOLOR != null ) return h.ah.MIXINCOLOR;
				//EMX_TODO child background do not assigned
				//tempDynamicRect.o = h;


				var c = HighlighterCache_Colors.GetHighlighterValue( h, adapter.modsController.highLighterMod );
				if ( c != null && c.Length > 0 ) return _tempTempColorClass.AssignFromList( ref c );
				else return _tempTempColorClass.empty;

				//var d = DrawBackground( null, null, tempDynamicRect, h, 1, resetFonts: true );
				//Rect selR = Rect.zero;
				//var t= EVENT.type;
				//EVENT.type = EventType.Layout;
				//var oldha=  h.ah;
				//h.ah = new AutoHighlighterColor();
				//var d = DrawBackground( selR, null, tempDynamicRect, h, 1, resetFonts: false );
				//h.ah = oldha;
				//EVENT.type = t;
				//return d;
			}
			else return null;
		}

		int colorProperty = Shader.PropertyToID("_Color");
		internal void DRAW_BGTEXTURE_OLD( Rect rect, Color32 color, int pluginID )
		{
			//if (Event.current.type == EventType.Repaint)
			//{
			//	var border = set.HIGHLIGHTER_TEXTURE_BORDER;
			//	if (!set.HIGHLIGHTER_USE_SPECUAL_SHADER || !set.HIghlighterExternalMaterial || set.HIGHLIGHTER_TEXTURE_STYLE == 0 || !set.BG_TEXTURE)
			//	{
			//		//Adapter.DrawTexture(rect, set.BG_TEXTURE ?? Texture2D.whiteTexture, ScaleMode.ScaleToFit, true, 1, color, border, 0);
			//		GUI.DrawTexture(rect, set.BG_TEXTURE ?? Texture2D.whiteTexture, ScaleMode.ScaleToFit, true, 1, color, border, 0);
			//	}
			//	else
			//	{
			//		set.HIghlighterExternalMaterial.SetColor(colorProperty, color);
			//		Graphics.DrawTexture(rect, set.BG_TEXTURE, border, border, border, border, set.HIghlighterExternalMaterial, 0);
			//	}
			//}

			if ( pluginID == 0 ) adapter.modsController.highLighterMod.DRAW_BGTEXTURE_OLD( rect, color );
			else adapter.modsController.projectWindowExtensions.highLighterMod.DRAW_BGTEXTURE_OLD( rect, color );
		}

















		/*
				private void prepare_hierarchy_draw()
				{
					DrawButton(ref adapter.selectionRect, adapter.o);
					//  throw new NotImplementedException();
				}
				*/




		DrawStackAdapter _colorAdditionalStackAdapter;
		internal DrawStackAdapter ICON_STACK {
			get { return _colorAdditionalStackAdapter ?? (_colorAdditionalStackAdapter = new DrawStackAdapter( adapter.pluginID )); }
		}
		internal bool CHILD_GROUP_FIX_FLAG = false;
		internal Dictionary<int, new_child_struct> new_child_cache_dic = new Dictionary<int, new_child_struct>();
		//bool oldPrefab = false;



		//EventType? ev = null;
		//bool prefabWas = false;
		//bool prefab = false;
		//bool PlayModeChanged = false;
		//bool oldAPpPlay = false;
		//Scene oldSc;
		//Scene lastScene;
		Rect oldWPos;
		Vector2 oldScrollPos;
		Dictionary<int, float> previousObjects = new Dictionary<int, float>();
		Dictionary<int, float> checkObjects = new Dictionary<int, float>();
		//Material oldMaterial;
		//static bool labelsInit = false;
		//static internal int lastFontSize = 11;
		//bool? oldProSkin;
		//static int cacheFontSize = -1;

		//internal bool SKIP_CHILDCOUNT = false;
		//bool LockChess = false;
		//internal Color TintColor;
		//internal bool mayscroll = false;
		//internal bool? applicationIsPlaying;
		//bool groupintLayout;
		//EventType? hierScrilEv = null;
		//EventType? hierScrilEv23 = null;
		//List<EditorWindow> repaintImmidiately = new List<EditorWindow>();

		internal bool NowPrefabIsOpened = false;

		void FirstFrame()
		{

			NowPrefabIsOpened = adapter.ha.IS_PREFAB_MOD_OPENED();

			var p = adapter.window.position;

			if ( p.width != oldWPos.width || p.height != oldWPos.height )
			{
				oldWPos = adapter.window.position;
				ResetStack();
			}

			if ( oldScrollPos.y != adapter.scrollPos.y )
			{
				oldScrollPos = adapter.scrollPos;
				// ColorModule.ResetStack(  );
				if ( previousObjects.Count > 0 )
				{
					checkObjects = previousObjects.ToDictionary( k => k.Key, v => v.Value );
					checkObjects.Remove( adapter.o.id );
					if ( checkObjects.Count != 0 )
					{
						var m = checkObjects.Values.Max();
						var findedId = -1;

						foreach ( var item in checkObjects )
						{
							if ( item.Value == m )
							{
								findedId = item.Key;
								break;
							}
						}
						if ( findedId != -1 ) checkObjects.Remove( findedId );
					}

				}
				else checkObjects.Clear();
				previousObjects.Clear();
			}
		}



		private void m_Main( HierarchyObject o, Rect selectionrect )
		{

			//	if (adapter.ha.IS_PREFAB_MOD_OPENED()) return;

			//EMX_TODO ResetStackDisabled
			//if (Event.current.isMouse)
			{
				//ResetStack();
			}
			//if (Event.current.type == EventType.Repaint) prefab = false;
			//if (Event.current.type != ev)
			{
				//ev = Event.current.type;

				//if (!oldProSkin.HasValue) oldProSkin = EditorGUIUtility.isProSkin;
				//if (oldProSkin != EditorGUIUtility.isProSkin) CLearAdditionalCache();
				//START_LABELS();
				//BACK_WIN();
				//SEARCH_HAS_BAKE();

				//if (IS_SEARCH_MODE_OR_PREFAB_OPENED() != oldPrefab)
				{
					//oldPrefab = IS_SEARCH_MODE_OR_PREFAB_OPENED();
					//HeightFixIfDrag();
					//firstFrame = 0;
					//prefab = true;
					//prefabWas = true;
				}
			}








			if ( Event.current.type == EventType.Layout )
			{   //selectionrect.width -= lastOffset;
				if ( !previousObjects.ContainsKey( o.id ) ) previousObjects.Add( o.id, selectionrect.y );

				if ( !checkObjects.ContainsKey( o.id ) )
				{
					checkObjects.Add( o.id, selectionrect.y );
					ResetStack(o.id);
				}


			}










			if ( Event.current.type == EventType.DragPerform )
			{
				ClearCacheAdditional();
			}





			//if (!Event.current.isMouse && Event.current.type != EventType.Repaint && Event.current.type != EventType.Ignore && Event.current.type != EventType.Used ||
			//		Event.current.type == EventType.MouseMove/* || Event.current.type == EventType.mouseDrag*/ )
			//{
			//	groupintLayout = set.HIGHLIGHTER_GROUPING_CHILD_MODE == 1 && Event.current.type == EventType.Layout;
			//}




			/*	if (hierScrilEv != Event.current.type || Event.current.type == EventType.Repaint)
				{
					hierScrilEv = Event.current.type;
					HierWinScrollPos = GetHierarchyWindowScrollPos();
				}8*/






			/*
				var headRect = selectionRect;
				if (!Layout)
				{
					if ((!DrawHeaderOther.HasValue || DrawHeaderOther == o.id))
					{
						DrawHeaderOther = o.id;
						_DG(headRect);
					}

				}*/


			//SKIP_CHILDCOUNT = false;

			//var oc = button.normal.textColor;
			var fadeRect = adapter.selectionRect;
			if ( adapter.modsController.rightModsManager.headerEventsBlockRect.HasValue ) fadeRect.x = adapter.modsController.rightModsManager.headerEventsBlockRect.Value.x + adapter.modsController.rightModsManager.propWidth;
			else fadeRect.x = fadeRect.x + fadeRect.width;
			//Debug.Log( fadeRect );
			DrawBG( o, adapter.selectionRect, fadeRect );

			if ( Event.current.type != EventType.Layout ) Draw( adapter.selectionRect, o );





		}


		void CLampB( ref Rect br )
		{
			if ( adapter.thisIsLast && adapter.window.bottomParams != null  )
			{
				//adapter.treeVisibleRectValue = (Rect)adapter.tree_m_VisibleRect.GetValue( adapter.TreeController_current );
				//Debug.Log( br + " " + adapter.treeVisibleRectValue );
				var Y = adapter.window.bottomParams.Y_POS();
				if ( br.y + br.height > Y )
				{
					var target =  Y - (br.y + br.height);
					br.height = Mathf.Max( 0, target );
				}
			}
			//if ( br.y + br.height > adapter.trueNulled_GUIClip_visibleRect.y + adapter.trueNulled_GUIClip_visibleRect.height )
			//{
			//	var target =  adapter.trueNulled_GUIClip_visibleRect.y + adapter.trueNulled_GUIClip_visibleRect.height - (br.y + br.height);
			//	br.height = Mathf.Max( 0, target );
			//}
		}


		void Draw_Button_Action()
		{




			if ( set.HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION > 0 )
			{
				var br = ButtonRect(adapter.selectionRect, adapter.o);

				CLampB( ref br );
				//colButStr.localSelectionRect = ICON_STACK.ConvertToLocal(adapter.selectionRect);
				if ( adapter.pluginID == 0 ? adapter.par_e.HIER_HIGH_SET.HIGHLIGHTER_CHANGE_BUTTON_CURSOR : adapter.par_e.PROJ_HIGH_SET.HIGHLIGHTER_CHANGE_BUTTON_CURSOR )
					EditorGUIUtility.AddCursorRect( br, MouseCursor.Link );
				if ( adapter.SimpleButton( br, switchedConten ) )
				{
					colButStr.localSelectionRect = (adapter.selectionRect);
					BUTTON_ACTION( Rect.zero, Rect.zero, new DrawStackMethodsWrapperData() { args = colButStr }, adapter.o );
				}
				//ICON_STACK.Draw_SimpleButton(br, switchedConten, BUTTON_ACTION_HASH, colButStr, true);

			}


			if ( set.HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION > 0 )
			{

				//colButStr.localSelectionRect = ICON_STACK.ConvertToLocal(selectionRect);
				//ICON_STACK.Draw_SimpleButton(buttonRectLeft, switchedConten, BUTTON_ACTION_HASH, colButStr, true);


				var fixedRebutRectt = ButtonRect(adapter.selectionRect, adapter.o);

				CLampB( ref fixedRebutRectt );


				//EditorGUI.DrawRect(fixedRebutRectt, Color.white);
				if ( adapter.pluginID == 0 ? adapter.par_e.HIER_HIGH_SET.HIGHLIGHTER_CHANGE_BUTTON_CURSOR : adapter.par_e.PROJ_HIGH_SET.HIGHLIGHTER_CHANGE_BUTTON_CURSOR )
					EditorGUIUtility.AddCursorRect( fixedRebutRectt, MouseCursor.Link );
				if ( adapter.SimpleButton( fixedRebutRectt, buttonContent, adapter.button ) )
					BUTTON_ACTION( Rect.zero, Rect.zero, new DrawStackMethodsWrapperData() { args = new ColorButtonStr() { localSelectionRect = adapter.selectionRect } }, adapter.o );


				//if (!adapter.ha.IS_PREFAB_MOD_OPENED())
				{

					if ( Root_HighlighterWindowInterface.CURRENT_WINDOW && Root_HighlighterWindowInterface.source == adapter.o && set.HIGHLIGHTER_HIERARCHY_DRAW_BUTTON_RECTMARKER > 0 )
					{
						EditorGUI.DrawRect(
							//HoverRect(_o, selectionRect, set.HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION),
							HoverRectNew( adapter.selectionRect, adapter.o ),
							EditorGUIUtility.isProSkin ? Color.white : new Color( 1, 0, 0, 0.5f ) );
						/* GUI.DrawTexture( HoverRect( selectionRect,
													_S_bgButtonForIconsPlace ), GetIcon( "SETUP_SORT1" ),
										 ScaleMode.ScaleToFit );      */     //HIGHLIGHTER_COLOR_PICKER   SETUP_BLUELINE  STORAGE_ONECOMP   BOTTOM_DESBUT FAVORIT_LIST_ICON FAVORIT_LIST_ICON ON STAR SETUP_SORT1 SETUP_SLIDER_HOVER

						//Adapter.DrawRect(new Rect(0, selectionRect.y + selectionRect.height / 8 * 3, selectionRect.height / 4 * 3, selectionRect.height / 8 * 2), Color.red);
					}

					if ( adapter.hashoveredItem && !adapter.HoverDisabled && set.HIGHLIGHTER_HIERARCHY_DRAW_BUTTON_RECTMARKER == 2 )
					{
						if ( adapter.hoverID == adapter.o.id )
						{
							EditorGUI.DrawRect(
								//HoverRectNew(_o, selectionRect, set.HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION),
								HoverRectNew( adapter.selectionRect, adapter.o ),
								EditorGUIUtility.isProSkin ? Color.white : new Color( 1, 0, 0, 0.5f ) );
							/*  GUI.DrawTexture( HoverRect( selectionRect, _S_bgButtonForIconsPlace ), GetIcon( "SETUP_SORT1" ), ScaleMode.ScaleToFit);*/

						}
					}
				}
			}


		}



		GUIContent buttonContent = new GUIContent();





		DynamicRect __tempDynamicRect;
		DynamicRect tempDynamicRect { get { return __tempDynamicRect ?? (__tempDynamicRect = new DynamicRect( this )); } }
		float raw_old_leftpadding {
			get {
				if ( IS_HIERARCHY() ) return adapter.ha.LEFT_PADDING;
				else return 0;
			}
		}

		void DrawBG( HierarchyObject _o, Rect selectionRect, Rect fadeRect )
		{
			//if (LastHeaderRect.Contains(Event.current.mousePosition) && Event.current.isMouse) //|| IS_PREFAB_MOD_OPENED()
			{
				//return;
			}


			// MonoBehaviour.print(window().position);
			// if (Event.current.type == EventType.repaint) currentClipRect = window().position;


			if ( Event.current.type == EventType.Repaint || Event.current.type == EventType.Layout )
			{


				//Debug.Log(adapter.o.name + " " + Event.current.type);

				//callFromExternal = null;
				/* if ( _o.name == "Misc" ) Debug.Log( ColorModule.DRAW_STACK.ContainsKey( _o.id ) );
				 if ( ColorModule.DRAW_STACK.ContainsKey( _o.id ) && _o.name == "Misc" ) Debug.Log( ColorModule.DRAW_STACK[_o.id].currentStackPos );
				 if ( ColorModule.DRAW_STACK.ContainsKey( _o.id ) && _o.name == "Misc" ) Debug.Log( ColorModule.DRAW_STACK[_o.id].GO_ENABLE_STATE );
				 if ( ColorModule.DRAW_STACK.ContainsKey( _o.id ) && _o.name == "Misc" ) Debug.Log( _o.Active() );*/
				var res = START_DRAW_PARTLY_TRYDRAW(selectionRect, _o);

				// if ( res ) Debug.Log( _o.name );
				if ( !res ) return;

				START_DRAW_PARTLY_CREATEINSTANCE( selectionRect, _o, Event.current.type == EventType.Repaint, momentumGl );
				//if (Event.current.type == EventType.Layout)
				{

					_o.ah.BG_RECT = null;
					_o.ah.BACKGROUNDED = 0;
					_o.ah.FLAGS = 0;
				}

				TryToFadeBG( selectionRect, _o );

				//var w = window();
				var w = adapter.window.Instance;
				var targetRect = selectionRect;


				if ( UnityVersion.UNITY_CURRENT_VERSION < UnityVersion.UNITY_2019_2_0_VERSION ) targetRect.width -= raw_old_leftpadding;


				tempDynamicRect.Set( targetRect, fadeRect, true, _o, true, raw_old_leftpadding );//IS_HIERARCHY() &&Adapter.USE2018_3 || IS_PROJECT()
				var cm = selectionRect;
				cm.width += cm.x;
				cm.x = 0;
				DrawBackground( cm, w, tempDynamicRect, _o );


				END_DRAW( _o , -1);


			}

			/* if ( ENABLE_LEFTDOCK_PROPERTY )
			 {   ColorModule.callFromExternal_objects = null;

				 o.BG_RECT = null;
				 o.BACKGROUNDED = 0;
				 o.FLAGS = 0;

				 if (Event.current.type == EventType.Repaint) ColorModule.TryToFadeBG( selectionRect, o );

				 if ( !DISABLE_DESCRIPTION( o ) )
				 {   var w = window();
					 var targetRect = selectionRect;
					 if ( HIGHLIGHTER_LEFT_OVERFLOW  == 1 )     // targetRect.width += raw_old_leftpadding; //HIGHLIGHTER_LEFT_OVERFLOW == 1 ? 0 :
					 {
					 }
					 if (Adapter.UNITY_CURRENT_VERSION < Adapter.UNITY_2019_2_0_VERSION)
						 targetRect.width -= raw_old_leftpadding;
					 tempDynamicRect.Set( targetRect, fadeRect, true, o, IS_HIERARCHY() && Adapter.USE2018_3 || IS_PROJECT(), raw_old_leftpadding  );
					 var cm = selectionRect;
					 cm.width += cm.x;
					 cm.x = 0;
					 ColorModule.DrawBackground( cm, w, tempDynamicRect, o );
				 }

			 }*/
		}

	}
}
