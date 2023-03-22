#define DISABLE_PING

using System;
using System.Collections.Generic;

using UnityEngine;




namespace EMX.HierarchyPlugin.Editor.Mods
{



	internal partial class ComponentsIcons_Mod : DrawStackAdapter, ISearchable
	{






		PluginInstance adapter = null;
		internal ComponentsIcons_Mod( int pid ) : base( pid )
		{
			adapter = Root.p[ pid ];
		}



		//SEARCH
		public override bool callFromExternal() { return callFromExternal_objects != null; }
		public Windows.SearchWindow.FillterData_Inputs callFromExternal_objects { get; set; }
		public Type typeFillter { get; set; }
		public string SearchHelper { get { return _SearchHelper; } set { _SearchHelper = value; } }
		string _SearchHelper = "Components Icons";
		public virtual float GetInputWidth() { return -1; }
		//SEARCH
		internal Event EVENT { get { return callFromExternal() ? Event.current : adapter.EVENT; } }


		internal override bool PERFOMANCE_BARS { get { return base.PERFOMANCE_BARS && !callFromExternal(); } }

		//internal static Dictionary<int, bool> null_cache = new Dictionary<int, bool>();
		// internal static Dictionary<int, bool> missing_cache = new Dictionary<int, bool>();

		/*    adapter.onSelectionChanged -= RawOnRemoveRaw;
               adapter.onSelectionChanged += RawOnRemoveRaw;
               adapter.onPlayModeChanged -= RawOnRemoveRaw;
               adapter.onPlayModeChanged += RawOnRemoveRaw;*/


		/*  bool drawIcon = false;
          Rect drawRect;
          Rect firstRect;*/
		//  GameObject o;


		//  Color t1 = new Color(.8f, .8f, .8f, .03f);








		//static Component[] redrawComps = new Component[1];
		// static bool DRAW_NEXTTONAME;
		// static float Y, HEIGHT;

		//static Color TC;

		// int i;

		// int asd;

		// int index;
		// Dictionary<string, int> customIconsIndexOf = new Dictionary<string, int>();
		//   Dictionary<string, Texture2D> customIconsTexture2D = new Dictionary<string, Texture2D>();
		//  Dictionary<int, string> menuText = new Dictionary<int, string>();
		// string keyGuid;
		// bool have222 = false;
		//  int[] indexes = new int[20];

		//  int[] sortindexes = new int[20];

		// private Hierarchy_GUI.CustomIconParams[] loaded = new Hierarchy_GUI.CustomIconParams[20];
		/* int max = -1;
         int findindex = -1;
         int lim = -1;
         int interator = -1;*/


		internal struct DrawCompsStack
		{
			internal string _type_twocharsname;
			internal string type_twocharsname {
				get {
					if ( _type_twocharsname == null )
					{
						var n = type.Name;
						_type_twocharsname = "";
						for ( int i = 0, L = n.Length; i < L; i++ )
						{
							if ( n[ i ] != '_' )
							{
								_type_twocharsname += n[ i ];
								if ( _type_twocharsname.Length > 1 ) break;
							}
						}
					}
					return _type_twocharsname;
				}
			}

			internal Type type;
			internal Component comp;
			internal bool isMono;
			internal bool hasCustomIcon;
			internal Texture2D buildInIcon;
			internal Texture2D customIcon;
			internal Color customColor;
			internal bool hasEnable;
			internal bool enable;
			internal bool skip;
		}
		DrawCompsStack[] drawCompsStack = new DrawCompsStack[100];
		int di = 0;
		object[] invoke_args = new object[1];
		Rect _drawRect;
		//  int i;
		// HierarchyObject __o;

		public void DrawSearch( Rect rect, HierarchyObject o )
		{
			_drawRect = rect;
			adapter.o = o;
			Draw();
		}

		internal void Subscribe( EditorSubscriber sbs )
		{
			// sbs.OnPlayModeStateChanged += PlaymodeStateChanged;
			drawCallIndex = 0;
			sbs.BuildedOnGUI_first.Add( PreCalcRect );
			sbs.AddBuildedOnGUI_middle( prepare_hierarchy_draw );
		}
		int catSpace;
		int mono_display_type;
		void PreCalcRect()
		{
			mono_display_type = adapter.par_e.COMPONENTS_MONO_ICON_TYPE;

			catSpace = adapter.par_e.COMPONENTS_ICONS_CAT_SPACE;

			DMI = adapter.par_e.COMPONENTS_DRAW_MONO_ICONS;
			DCI = adapter.par_e.COMPONENTS_DRAW_GLOBALCUSTOM_ICONS;
			DDI = adapter.par_e.COMPONENTS_DRAW_DEFAULT_ICONS;
			DCI_FI = adapter.par_e.COMPONENTS_DRAW_CUSTOM_ICONS_FROM_ISPECTOR;
			DCI_FS = adapter.par_e.COMPONENTS_DRAW_CUSTOM_ICONS_FROM_SETTINGS;


			if ( adapter.WIN_SET.USE_OVERRIDE_DEFAULT_ICONS_SIZE ) offset.x = adapter.WIN_SET.OVERRIDE_DEFAULT_ICONS_SIZE;
			else offset.x = Window.DefaultIconSize( adapter );
			offset.x += adapter.par_e.COMPONENTS_NEXT_TO_NAME_PADDING + adapter.ha.LEFT_PADDING - adapter.ha.LEFT_PADDING;
			offset.y = adapter.par_e.COMPONENTS_MARGIN_TOP;
		}
		Vector2 offset;
		Dictionary<int, bool> id_to_mono_dic = new Dictionary<int, bool>();
		Dictionary<string, bool> string_to_mono_dic = new Dictionary<string, bool>(){
		{  "js Script Icon", false },
		{  "dll Script Icon", false },
		{  "cs Script Icon", false },
		{  "d_js Script Icon", false },
		{  "d_dll Script Icon", false },
		{  "d_cs Script Icon", false }
	};

		void prepare_hierarchy_draw()
		{

			var content_size = adapter.o.GetContentSize();
			//content_size.x += ;
			// GUI.DrawTexture(adapter.selectionRect, Texture2D.whiteTexture);
			_drawRect = adapter.selectionRect;
			_drawRect.x += content_size.x;

			_drawRect.x += offset.x;
			_drawRect.y += offset.y;

			_drawRect.width = GetClampedRect_Graphic( _drawRect );

			//rect.width = currentRect.x - rect.x + 4;
			Draw();


			if ( CURRENT_STACK != null ) Root.TemperaryDisableThePlugin_FromCache();
		}

		bool DMI;
		bool DCI, DCI_FI, DCI_FS;
		bool DDI;
		static Type root_type = typeof(UnityEngine.Object);
		public void Draw()
		{

			// var o = _o.go;
			//if (Adapter.OPT_EV_BR(EVENT)) return 0;
			// __o = _o;
			//  if ( !_o.go  || (Application.isPlaying && adapter.par.PLAYMODE_HideComponents2) ) return width;
			//ClearIconPos( _o.id );
			//  DRAW_NEXTTONAME = adapter.par.COMPONENTS_NEXT_TO_NAME && !callFromExternal();


			if ( !START_DRAW( _drawRect, adapter.o ) ) return;
			if ( !DCI && !DDI && !DMI )
			{
				END_DRAW( adapter.o, -1 );
				return;
			}

			var _comps = adapter.o.GetComponents();
			di = 0;
			if ( _comps.Length + 1 > drawCompsStack.Length ) Array.Resize( ref drawCompsStack, _comps.Length + 1 );

			for ( int i = 0; i < _comps.Length; i++ )
			{
				if ( !_comps[ i ] ) continue;

				// if ( !nowMono && _IsMonoBehaviour( comps[ i ] ) ) nowMono = true;
				//  bool was = false;
				var mon = _IsMonoBehaviour(_comps[i]);
				var t = _comps[i].GetType();
				if ( !mon && DDI && !HierarchyCommonData.Instance().IsComponentIconHided( t.FullName ) )
				{
					if ( _comps[ i ] is Transform || _comps[ i ] is MeshFilter || _comps[ i ] is CanvasRenderer ) continue;

					var buildInIcon = TODO_Tools.GetObjectBuildinIcon(t);
					if ( !buildInIcon.add_icon ) continue;

					drawCompsStack[ di ].type = t;
					drawCompsStack[ di ].comp = _comps[ i ];
					drawCompsStack[ di ].buildInIcon = buildInIcon.add_icon;
					drawCompsStack[ di ].isMono = false;
					drawCompsStack[ di ].hasCustomIcon = false;
					drawCompsStack[ di ].hasEnable = HasEnable( _comps[ i ] );
					if ( drawCompsStack[ di ].hasEnable ) drawCompsStack[ di ].enable = GetEnableFast( _comps[ i ] );
					drawCompsStack[ di ].skip = false;
					di++;
					continue;
				}

				if ( mon && (DMI || DCI) )
				{

					drawCompsStack[ di ].type = t;
					drawCompsStack[ di ].comp = _comps[ i ];
					drawCompsStack[ di ].hasEnable = true;
					drawCompsStack[ di ].enable = ((MonoBehaviour)_comps[ i ]).enabled;
					drawCompsStack[ di ].isMono = true;
					drawCompsStack[ di ].hasCustomIcon = false;
					if ( DCI )
					{
						if ( DCI_FS && HierarchyCommonData.Instance().HasCustomIcon( _comps[ i ] ) )
						{
							// NEW MOD
							var type = t;
							HierarchyCommonData.CustromIconData load = null;
							var ins = HierarchyCommonData.Instance();
							while (type != root_type)
                            {
								if (!ins._HasCustomIcon.ContainsKey(type.FullName))
                                {
									type = type.BaseType;
									continue;
								}
								load = HierarchyCommonData.Instance()._HasCustomIcon[type.FullName];
                            }
                             
							drawCompsStack[ di ].customIcon = TODO_Tools.GUI_TO_OBJECT( ref load.texture ) as Texture2D;
							drawCompsStack[ di ].customColor = load.color;
							drawCompsStack[ di ].hasCustomIcon = drawCompsStack[ di ].customIcon;
						}
						if ( DCI_FI && !drawCompsStack[ di ].hasCustomIcon && (drawCompsStack[ di ].customIcon = TODO_Tools.GetObjectBuildinIcon( _comps[ i ], t ).add_icon) )
						{
							if ( !string_to_mono_dic.ContainsKey( drawCompsStack[ di ].customIcon.name ) )
							{
								drawCompsStack[ di ].hasCustomIcon = true;
								drawCompsStack[ di ].customColor = Color.white;
							}
						}
						if ( drawCompsStack[ di ].hasCustomIcon ) drawCompsStack[ di ].skip = false;
					}

					if ( DMI ) drawCompsStack[ di ].skip = false;

					di++;
					continue;
				}
			}
			drawCompsStack[ di ].skip = true;
			drawCompsStack[ di ].comp = null;



			// if (adapter.par_e.OVERRIDE_SIZE_FOR_DEFAULT_ICONS && !callFromExternal()) _drawRect.x += adapter.par_e.DEFAULT_ICON_SIZE;
			//  else _drawRect.x += Tools.singleLineHeight;

			var iconRect = _drawRect;
			/*   Y = drawRect.y;
               HEIGHT = drawRect.height;
               firstRect = drawRect;*/

			var cellRect = iconRect;
			var oldH = iconRect.height;
			iconRect.width = iconRect.height = adapter.par_e.COMPONENTS_ICONS_SIZE;
			iconRect.y += (oldH - iconRect.height) / 2;
			iconRect.x -= 1;

			iconRect.x += iconRect.width / 2;

			bool drawed = false;
			//SEARCH FIRST
			if ( typeFillter != null )
			{
				for ( int i = 0; i < di; i++ )
				{
					if ( drawCompsStack[ i ].type == typeFillter )
					{
						drawCompsStack[ i ].skip = true;
						DrawIcon( cellRect, iconRect, adapter.o, ref drawCompsStack[ i ], !drawCompsStack[ i ].isMono );
						iconRect.x += iconRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE;
						cellRect.x += iconRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE;
						drawed = true;
					}
				}
			}
			if ( drawed )
			{
				iconRect.x += catSpace;
				cellRect.x += catSpace;
				drawed = false;
			}
			//DEFAULT
			for ( int i = 0; i < di; i++ )
			{
				if ( drawCompsStack[ i ].skip ) continue;
				if ( !drawCompsStack[ i ].isMono )
				{
					drawCompsStack[ i ].skip = true;
					DrawIcon( cellRect, iconRect, adapter.o, ref drawCompsStack[ i ], true );
					iconRect.x += iconRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE;
					cellRect.x += iconRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE;
					drawed = true;
				}
			}

			if ( drawed )
			{
				iconRect.x += catSpace;
				cellRect.x += catSpace;
				drawed = false;
			}
			//MONO

			if ( adapter.par_e.COMPONENTS_MONO_SPLIT_MODE == 0 )
			{
				int notSkipped = 0;
				for ( int i = 0; i < di; i++ )
				{
					if ( drawCompsStack[ i ].skip || drawCompsStack[ i ].hasCustomIcon ) continue;
					notSkipped++;
				}
				if ( notSkipped != 0 )
				{
					DrawCompsStack[] comps = new DrawCompsStack[notSkipped];

					//  var comps = drawCompsStack.Where(c => !c.skip).ToArray();
					if ( comps.Length != 0 )
					{
						for ( int i = 0, x = 0; i < di; i++ ) if ( !drawCompsStack[ i ].skip && !drawCompsStack[ i ].hasCustomIcon ) comps[ x++ ] = drawCompsStack[ i ];
						DrawIcon( cellRect, iconRect, adapter.o, ref comps[ 0 ], false, comps );
						iconRect.x += iconRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE;
						cellRect.x += iconRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE;
						foreach ( var item in comps ) DrawAttributes( ref cellRect, ref iconRect, item.type, item.comp );
						drawed = true;
					}
				}

			}
			else if ( adapter.par_e.COMPONENTS_MONO_SPLIT_MODE == 1 )
			{

				int c_1 = 0, c_2 = 0;
				for ( int i = 0; i < di; i++ )
				{
					if ( !drawCompsStack[ i ].skip && !drawCompsStack[ i ].hasCustomIcon && drawCompsStack[ i ].enable || !drawCompsStack[ i ].hasEnable ) c_1++;
					else if ( !drawCompsStack[ i ].skip && !drawCompsStack[ i ].hasCustomIcon && !drawCompsStack[ i ].enable ) c_2++;
				}

				if ( c_1 != 0 || c_2 != 0 )
				{
					DrawCompsStack[] comps1 = new DrawCompsStack[c_1];
					DrawCompsStack[] comps2 = new DrawCompsStack[c_2];
					for ( int i = 0, x = 0, y = 0; i < di; i++ )
					{
						if ( !drawCompsStack[ i ].skip && !drawCompsStack[ i ].hasCustomIcon && drawCompsStack[ i ].enable || !drawCompsStack[ i ].hasEnable ) comps1[ x++ ] = drawCompsStack[ i ];
						else if ( !drawCompsStack[ i ].skip && !drawCompsStack[ i ].hasCustomIcon && !drawCompsStack[ i ].enable ) comps2[ y++ ] = drawCompsStack[ i ];
					}
					//     Debug.Log(adapter.o.name + " " + comps1.Length);
					//  var comps1 = drawCompsStack.Where(c => !c.skip && c.enable || !c.hasEnable).ToArray();
					//   var comps2 = drawCompsStack.Where(c => !c.skip && !c.enable).ToArray();
					if ( comps1.Length != 0 )
					{
						DrawIcon( cellRect, iconRect, adapter.o, ref comps1[ 0 ], false, comps1 );
						iconRect.x += iconRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE;
						cellRect.x += iconRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE;
					}
					if ( comps2.Length != 0 )
					{
						DrawIcon( cellRect, iconRect, adapter.o, ref comps2[ 0 ], false, comps2 );
						iconRect.x += iconRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE;
						cellRect.x += iconRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE;
					}
					foreach ( var item in comps1 ) DrawAttributes( ref cellRect, ref iconRect, item.type, item.comp );
					foreach ( var item in comps2 ) DrawAttributes( ref cellRect, ref iconRect, item.type, item.comp );
					drawed = true;
				}
			}
			else // == 2
			{
				for ( int i = 0; i < di; i++ )
				{
					if ( drawCompsStack[ i ].skip ) continue;
					DrawIcon( cellRect, iconRect, adapter.o, ref drawCompsStack[ i ], false );
					iconRect.x += iconRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE;
					cellRect.x += iconRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE;
					DrawAttributes( ref cellRect, ref iconRect, drawCompsStack[ i ].type, drawCompsStack[ i ].comp );
					drawed = true;
				}
			}

			if ( drawed )
			{
				iconRect.x += catSpace;
				cellRect.x += catSpace;
				drawed = false;
			}
			//CUSTOM ICON
			if ( adapter.par_e.COMPONENTS_MONO_SPLIT_MODE != 2 )
				for ( int i = 0; i < di; i++ )
				{
					if ( drawCompsStack[ i ].skip ) continue;
					if ( drawCompsStack[ i ].hasCustomIcon )
					{
						drawCompsStack[ i ].skip = true;
						DrawIcon( cellRect, iconRect, adapter.o, ref drawCompsStack[ i ], false );
						iconRect.x += iconRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE;
						cellRect.x += iconRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE;
						DrawAttributes( ref cellRect, ref iconRect, drawCompsStack[ i ].type, drawCompsStack[ i ].comp );
						drawed = true;
					}
				}





			//afterCategory

			// if ( !menuText.ContainsKey( c.GetInstanceID() ) ) menuText.Add( c.GetInstanceID(), "Hide " + c.GetType().Name + " icon" );




			END_DRAW( adapter.o, -1 );

			return;
		}







	}
}








//GameObjectCacher<string> cache = new GameObjectCacher<string>();
/* Dictionary<int, string> GUID_cache = new Dictionary<int, string>();
 string ComponentToGUID( Component component )
 {
     if ( !GUID_cache.ContainsKey( component.GetInstanceID() ) )
     {
         GUID_cache.Add( component.GetInstanceID(), null );
         var t = component as MonoBehaviour;
         if ( t )
         {
             var mono = MonoScript.FromMonoBehaviour(t);
             if ( mono != null )
             {
                 var keyPath = AssetDatabase.GetAssetPath(mono);

                 if ( !string.IsNullOrEmpty( keyPath ) )
                     GUID_cache[ component.GetInstanceID() ] = AssetDatabase.AssetPathToGUID( keyPath );
             }
         }
     }
     return GUID_cache[ component.GetInstanceID() ];
 }*/



/*  internal static void ClearM_CustomIconsCache( )
  {
      updateTimer.Clear();
      cache.Clear();
      cacheMono.Clear();
      null_cache.Clear();
      missing_cache.Clear();
  }*/

//  internal static Dictionary<int, double> updateTimer = new Dictionary<int, double>();
// internal static Dictionary<int, Component[]> cache = new Dictionary<int, Component[]>();

/*  internal static Component[] get_from_cache( int id, GameObject go )
  {
      if ( !Hierarchy.cache.ContainsKey( id ) )
      { //Hierarchy.cache.Add( id, HierarchyExtensions.Utilities.GetComponentFast<Component>.GetAll( go ) );

          bool? haveMissing = null;
          var comps = HierarchyExtensions.Utilities.GetComponentFast<Component>.GetAll(go).Where(c =>
      {
          var result = (bool) c && (c.hideFlags & HideFlags.HideInInspector) == 0;

          if (!c) haveMissing = true;

          return result;
      }).ToArray();

          Hierarchy.cache.Add( id, comps );

          if ( Adapter.HierAdapter.par.SHOW_MISSINGCOMPONENTS && haveMissing.HasValue )
          {
              if ( !missing_cache.ContainsKey( id ) ) missing_cache.Add( id, false );

              missing_cache[ id ] = haveMissing.Value;
          }
      }

      return Hierarchy.cache[ id ];
  }*/

// internal static Dictionary<int, MonoBehaviour[]> cacheMono = new Dictionary<int, MonoBehaviour[]>();










//    bool? haveNull = null;
//    bool? haveMissing = null;


//  if ( !callFromExternal() && useEvent == null && adapter.pluginID == 0 ) useEvent = EVENT.type;

// var needAssign = false;

//  if ( Application.isPlaying && adapter.par.PLAYMODE_UseBakedComponents ) needAssign = callFromExternal() || !cache.ContainsKey( id );
// else needAssign = callFromExternal() || !adapter.NEW_PERFOMANCE && EVENT.type == useEvent || !cache.ContainsKey( id );



/*   if ( !cacheMono.ContainsKey( id ) ) cacheMono.Add( id, comps.Where( _IsMonoBehaviour ).Select( c => (MonoBehaviour)c ).ToArray() );
       else cacheMono[ id ] = comps.Where( _IsMonoBehaviour ).Select( c => (MonoBehaviour)c ).ToArray();*/



/* if ( typeFillter == null ) _REDRAW( _o, comps, cacheMono[ id ] );
 else
 { 
     var f = comps.Any(c => Adapter.GetType_(c) == typeFillter);

     if ( f )
     {
         var dada = comps.Where(c => Adapter.GetType_(c) == typeFillter).ToArray();
         var mm = dada.Where(_IsMonoBehaviour).Select(c => (MonoBehaviour) c).ToArray();
         _REDRAW( _o, dada, mm );
     }

     { // _REDRAW( comps.Where( c => Adapter.GetType_( c ) != typeFillter ).ToArray() );
         var dada = comps.Where(c => Adapter.GetType_(c) != typeFillter).ToArray();
         var mm = dada.Where(_IsMonoBehaviour).Select(c => (MonoBehaviour) c).ToArray();

         _REDRAW( _o, dada, mm );
     }

     typeFillter = null;
 }*/


