using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor.Mods.HyperGraph
{
	partial class HyperGraphModInstance
	{








		private bool SKANNING = false;
#pragma warning disable
		private bool ERROR = false;
#pragma warning restore

		private UnityEngine.Object CurrentSelection = null;

		//  internal  bool wasInit = false;
		//   SortedDictionary<UnityEngine.Object, activeComps> TARGET_COMPS = new SortedDictionary<int, activeComps>();
		private Dictionary<int, ObjectDisplay> TARGET_COMPS = new Dictionary<int, ObjectDisplay>();
		//   private  Dictionary<int, ObjectDisplay> SELF_TARGETS = new Dictionary<int, ObjectDisplay>();

		private float __TARGET_HEIGHT = 0;

		private float TARGET_HEIGHT {
			get { return Mathf.RoundToInt( __TARGET_HEIGHT ); }

			set { __TARGET_HEIGHT = value; }
		}

		private float __TARGET_CURRENT_Y = 0;

		private float TARGET_CURRENT_Y {
			get { return Mathf.RoundToInt( __TARGET_CURRENT_Y ); }

			set { __TARGET_CURRENT_Y = value; }
		}

		private float __INPUT_CURRENT_Y = 0;

		private float INPUT_CURRENT_Y {
			get { return Mathf.RoundToInt( __INPUT_CURRENT_Y ); }

			set { __INPUT_CURRENT_Y = value; }
		}

		/*     findedComopnentsAttached
				 findedComopnentsAttached.Add(currentList[currentIndex].GetInstanceID(), new activeComps() {
						 comps = activeComps,
						 DRAW_A_POSES = new Vector2[activeComps.Count],
						 DRAW_B_POSES = new Vector2[fieldsCount],
						 height = height
					 });*/
		private List<int> PTR = new List<int>();

		private GUIContent tootipContent = new GUIContent();

		private Rect moduleRect = new Rect();
		private Rect GAMEOBJECTRECT = new Rect();
		private GUIContent content = new GUIContent();
		private UnityEngine.Object[] comps = null;
		private bool[] compsinitialized = new bool[0];
		readonly private SortedList<int, int> compsSorted = new SortedList<int, int>();

		private Vector2[] comps_inPos = new Vector2[0];

		/*  private float __selectObject_height_WINDOW;
		  private float __selectObject_height_MAIN;*/
		private float __selectObject_height;

		private float SelectObject_height //   get { return Mathf.RoundToInt( CURRENT_CONTROLLER.MAIN ? __selectObject_height_MAIN : __selectObject_height_WINDOW ); }
		{
			get { return __selectObject_height; }

			set { __selectObject_height = value; }

			/*set {
			  if (CURRENT_CONTROLLER.MAIN) __selectObject_height_MAIN = value;
			  else __selectObject_height_WINDOW = value;
			}*/
		}
		/*void add_selectObject_height(float additional_value)
		{
		  __selectObject_height_MAIN += adapter.par.HiperGraphParams.SCALE * additional_value;
		  __selectObject_height_WINDOW += adapter.par.HiperGraphParams.WINDIOW_SCALE * additional_value;
		}
		void set_selectObject_height(float value)
		{
		  __selectObject_height_MAIN = value / CURRENT_SCALE * adapter.par.HiperGraphParams.SCALE;
		  __selectObject_height_WINDOW = value / CURRENT_SCALE * adapter.par.HiperGraphParams.WINDIOW_SCALE;
		}
		*/


		private List<UnityEngine.Object> UndoList = new List<UnityEngine.Object>();
		private int UndoPos = 0;
		private bool skipUndo = false;
		private GUIContent scanningContent = new GUIContent("?   ", "Searching for references...");
		private int DRAWING_INDEX = 0;
		const int MAX20 = 20;
		private Vector2 p1,
			p2;

		private Color alpha = new Color(1, 1, 1, 0.3f);

		//BUTTON 19
		//COMP 12
		//VAR 7
		//
		//////////////////////////////////////////////////
		//////////////////////////////////////////////////  DRAW TARGET
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		private Rect singleRect = new Rect();
		private Rect ARROW_RECT = new Rect();
		private int DRAWSINGLECOMP_TARGET_EVENT_START = 10000;
		private int DRAWSINGLECOMP_TARGET_EVENT_M2 = 20000;
		private int DRAWSINGLECOMP_TARGET_EVENT_INPUT = 30000;
		private Vector2 DRAWSINGLECOMP_RESULT = new Vector2();
		private Color prefab_color = Color.Lerp( new Color32(0x7f, 0xec, 0xff, 255), Color.white, 0.0f);
		//private Color event_color = new Color(0.35f, 0.9f, 1f);
		private Color event_color = Color.Lerp( new Color32(0x7f, 0xec, 0xff, 255), Color.white, 1.0f);
		//private Color asset_color = new Color(0.6f, 0.85f, 0.55f);
		//private Color asset_color = Color.Lerp( new Color32(0xd1, 0x7e, 0x2b, 255), Color.white, 0.0f);
		//private Color asset_color = Color.Lerp( new Color32(0xef, 0xfc, 0xd5, 255), Color.white, 0.5f);
		private Color asset_color = Color.Lerp( new Color32(0xaa, 0xaa, 0xaa, 255), Color.white, 0.0f);
#pragma warning disable
		//private Color arrowC1 = new Color32(236, 198, 60, 155);
		//private Color arrowC1personal = new Color32(56 / 2, 89 / 2, 102 / 2, 255);
		private Color arrowC1 = new Color32(236, 236, 236, 155);
		private Color arrowC1personal = new Color32(28, 28, 28, 255);
		//   private  Color arrowC1personal = Color.black;
		//  private  Color arrowC1personal = new Color32(236 / 5, 198 / 5, 60 / 5, 185);
		//private  Color arrowC1personal = new Color32(236 / 2, 198 / 2, 60 / 2, 185);
		// private  Color arrowC2 = new Color32(41, 41, 41, 255);
		//  private  Color arrowC3 = new Color32(255, 204, 9, 255);
#pragma warning restore

		//  Rect arrowRect = new Rect();
		private Rect point = new Rect(0, 0, 1, 1);
		private Rect? ScreenRect = null;

		/// <summary>
		/// ARRAY SUPPORT
		/// </summary>
		private static Type compType = typeof(UnityEngine.Object);

		//	private Type gameobjectType = typeof( GameObject );
		// private  Dictionary<string, Type> PTR_COMP = new Dictionary<string, Type>();

		private Dictionary<Type, FieldsAccessor> FiledsInfo = null;




		// private Rect INPUT_RECT = new Rect();

		private bool WASSCAN = false;

		/*  readonly private DoubleList<int, FIELD_PARAMS> findedList =
			  new DoubleList<int, FIELD_PARAMS>();*/
		readonly private Dictionary<int, FIELD_PARAMS> findedList =
			new Dictionary<int, FIELD_PARAMS>();

		readonly private Dictionary<int, ObjectDisplay> INPUT_COMPS =
			new Dictionary<int, ObjectDisplay>();

		readonly internal List<ObjectDisplay> SCANNING_COMPS =
			new List<ObjectDisplay>();

		//  readonly List<int> SCANNING_COMPS_REMOVER = new List<int>();

		private IEnumerator<HierarchyObject> currentList = null;
		int CountProgress = 0;
		private int currentIndex = 0;


		private float SKANNING_PROGRESS {
			get {
				if ( currentList == null || CountProgress == 0 ) return 0;

				return CountProgress < 2 ? 0 : (currentIndex / (CountProgress - 1f));
			}
		}

		private float DEFAULTWIDTH( ExternalDrawContainer
			controller ) //return Math.Max( Mathf.RoundToInt( controller.WIDTH / 5 * 4 / 3.5f ), 100 ) * controller.DEFAULT_WIDTH( controller );
		{
			var result = Math.Max(Mathf.RoundToInt(controller.WIDTH( IExternalWindowType.HYPER_GRAPH  ) / 5 * 4 / 3.5f), 100);

			if ( !controller.MAIN ) result = Mathf.RoundToInt( result / 1.5f );

			return result;
		}



		/*
		static UnityEngine.Object GET_OBJECT(object target, FieldInfo fieldInfo)
		{
		   var result = fieldInfo.GetValue(target);
		   if (result != null && !(result is UnityEngine.Object) )
		   {
			   if (!deep_analizer_Serializer_property_dropper.ContainsKey(fieldInfo)) return null;
			   else
			   {

			   }
		   }
		   return result as UnityEngine.Object;
		}


		static readonly List<FieldInfo> cache_serializable_fieldsList = new List<FieldInfo>();
		static Dictionary<FieldInfo, bool> cache_serializable = new Dictionary<FieldInfo, bool>();
		static Dictionary<FieldInfo, FieldInfo> deep_analizer_Serializer_property_dropper = new Dictionary<FieldInfo, FieldInfo>();
		static bool FindSerializableInSerializable(FieldInfo type)
		{
		   if (cache_serializable.ContainsKey(type)) return cache_serializable[type];


		   GET_FIELDS(type);
		   var result = cache_serializable_fieldsList.FirstOrDefault(f=> IsFieldReturnObject(f));

		   if (result != null)
		   {
			   cache_serializable.Add(type, true);
			   return result;
		   }


			   cache_serializable
		}

		static void GET_FIELDS(FieldInfo type)
		{
		   var t = type.FieldType;
		   cache_serializable_fieldsList.Clear();
		   while (t != null)
		   {
			   cache_serializable_fieldsList.AddRange(t.GetFields(flags));
			   t = t.BaseType;
		   }
		}*/
		/* Type serType = typeof(SerializeField);
		 static bool IsFieldReturnObject(FieldInfo f)
		 {
			 bool first = false;
			 //.. bool secont = false;
			 if (f.IsPublic) first = true;
			 if (!first)
			 {
				 // if (f.GetCustomAttribute<SerializeField>() != null) first = true;
				 if ((f.Attributes & FieldAttributes.NotSerialized) == 0) first = true;
			   //  if (f.GetCustomAttributes( ser ) != null) first = true;
				 //  adapter.logProxy.Log(f.GetCustomAttributes(true).Length == 0 ? "" : f.GetCustomAttributes(true).Select(s => s.ToString()).Aggregate((a, b) => a + " " + b));
			 }
			 // (f.IsPublic ||  != null)
			 if (first && compType.IsAssignableFrom( f.FieldType )) return true;
			 return false;
		 }*/


		private void INIT_COMPS()
		{
			//comps = HierarchyExtensions.Utilities.GetComponentFast<Component>.GetAll(CurrentSelection as GameObject).Where(c => c).ToArray();
			comps = (CurrentSelection as GameObject).GetComponents<Component>().Where( c => c ).ToArray();

			Array.Resize( ref compsinitialized, comps.Length );
			Array.Resize( ref sizememory, comps.Length + 1 );
			SEL_INC++;
			selection_id = CurrentSelection ? CurrentSelection.GetInstanceID() : -1;
			//GetAllValuesCache.Clear();
			compsSorted.Clear();

			for ( int i = 0; i < comps.Length; i++ )
			{
				compsSorted.Add( comps[ i ].GetInstanceID(), i );
				compsinitialized[ i ] = false;
			}

			if ( comps.Length + 1 != comps_inPos.Length ) Array.Resize( ref comps_inPos, comps.Length + 1 );

			//  repaintComps = true;
			//selectObject_height = SIZES.OBJECT();
			SelectObject_height = SIZE.OBJECT();
			// add_selectObject_height( SIZES.OBJECT() / CURRENT_SCALE );
			TARGET_HEIGHT = 0;


			TARGET_COMPS.Clear();
			// SELF_TARGETS.Clear();
			PTR.Clear();

			// int interator = 0;
			for ( int i = 0; i < comps.Length; i++ )
			{
				if ( !comps[ i ] ) continue;

				GetReflectionFields( comps[ i ] );
				// selectObject_height += SIZES.SPACE() + SIZES.COMP();
				SelectObject_height += SIZE.SPACE() + SIZE.COMP();
			}

			//ConcurrentDictionary
			/*   foreach (var objectDisplay in TARGET_COMPS)
			   {
				   objectDisplay.Value.DRAW_A_POSES = new Vector2[objectDisplay.Value.comps.Count + 1];
			   }*/
		}



		float HALF_SCALE()
		{
			return (CURRENT_SCALE - 1) / 2 + 1;
		}

		void Reset()
		{
			//adapter.OneFrameActionOnUpdateAC = () => { comps = null; };
			//adapter.OneFrameActionOnUpdate = true;
			adapter.PUSH_UPDATE_ONESHOT( 0, () => { comps = null; } );
		}


		internal int FONT_SIZE_INTERFACE()
		{
			return Mathf.RoundToInt( adapter.par_e.HYPERGRAPH_INT_FONTSIZE );
		}
		internal int FONT_SIZE()
		{
			return Mathf.RoundToInt( adapter.par_e.HYPERGRAPH_OB_FONTSIZE ) - 2;
		}

		internal int FONT_SIZE10()
		{
			return Mathf.RoundToInt( adapter.par_e.HYPERGRAPH_OB_FONTSIZE );
		}

		internal float FONT_FACTOR_INTERFACE()
		{
			return Mathf.Min( (adapter.par_e.HYPERGRAPH_INT_FONTSIZE + 10) / 20f, 1 );
		}





		//  int fieldsCount = 0;
		private float DRAWOBJECT( ExternalDrawContainer controller, bool calculate = false )
		{ /* controller.scrollPos_.x
			      controller.scrollPos.y;*/
			moduleRect.x = -DEFAULTWIDTH( controller ) / 2;
			moduleRect.y = 0;
			moduleRect.width = DEFAULTWIDTH( controller );
			moduleRect.height = EditorGUIUtility.singleLineHeight;

			/*  MonoBehaviour.print(Event.current.type);
			  DragAndDrop.*/
			// MonoBehaviour.print(HIPER_HEIGHT());
			if ( CurrentSelection == null )
			{
				if ( !calculate ) LABEL( moduleRect, "No GameObjects" );

				//GUI .Label( moduleRect, "No GameObjects" );
				//selectObject_height = moduleRect.height;
				SelectObject_height = moduleRect.height;
				TARGET_COMPS.Clear();
				//SELF_TARGETS.Clear();
				return SelectObject_height;
				// fieldsCount = 0;
			}

			if ( comps == null )
			{
				INIT_COMPS();
				//  repaintComps = true;
			}

			/*  if (currentJob != null)
			  {
				  moduleRect.height = EditorGUIUtility.singleLineHeight;
				  if (!calculate) GUI .Label(moduleRect, "Updating...");
				  height = moduleRect.height;
				  return height;
				  // fieldsCount = 0;
			  }*/

			{ //  bool repaintComps = false;

				/*  if (repaintComps)
				  {
					  height = 19;
					  fieldsCount = 0;
					  for (int i = 0; i < comps.Length; i++)
					  {
						  var f = GetReflectionFields(comps[i]);
						  if (f.Count == 0) continue;
						  height += SPACE + 12;
						  for (int fIndex = 0; fIndex < f.Count; fIndex++)
						  {
							  height += SPACE + 7;
							  fieldsCount++;
						  }
					  }
				  }*/
				if ( calculate ) return SelectObject_height;


				var R = moduleRect;
				R.x -= 1;
				R.width = 5;
				R.height = SelectObject_height;

				// DEBUG_HORISONTAL(scrollPos.y,Color.red);
				if ( Event.current.type == EventType.Repaint )
					Draw( R, HIPERUI_LINE_BOX, false, false, false, false );

				foreach ( var objectDisplay in INPUT_COMPS )
				{
					objectDisplay.Value.DRAW = false;
				}

				foreach ( var objectDisplay in TARGET_COMPS )
				{
					objectDisplay.Value.DRAW = false;
				}

				moduleRect.height = SIZE.OBJECT();
				moduleRect.width = DEFAULTWIDTH( controller );
				///////////////GAMEOBJECT
				content.text = CurrentSelection.name;
				content.tooltip = content.text + " - GAMEOBJECT";
				HIPERUI_GAMEOBJECT.fontSize = Mathf.RoundToInt( FONT_SIZE() * HALF_SCALE() );
				DO_BUTTON_FORSCROLL( controller, moduleRect, content, HIPERUI_GAMEOBJECT, 15, CurrentSelection, draw3Pass: true );
				GAMEOBJECTRECT = moduleRect;
				GAMEOBJECTRECT.height = SelectObject_height - moduleRect.height;
				GAMEOBJECTRECT.y += moduleRect.height;

				/*  DEBUG_HORISONTAL(GAMEOBJECTRECT.y, Color.red);
				  DEBUG_HORISONTAL(GAMEOBJECTRECT.y + GAMEOBJECTRECT.height * 0.5f, Color.blue);*/

				WRITE_ARROW_A_POSES( moduleRect, comps.Length );
				sizememory[ comps.Length ] = moduleRect.width;
				DO_ARROW_A( moduleRect, comps.Length );
				///////////////GAMEOBJECT//////////////////////////////////////////////////
				moduleRect.y += moduleRect.height + SIZE.SPACE();
				DRAWING_INDEX = 0;
				TARGET_CURRENT_Y = 0;
				var mre = moduleRect.y;

				for ( int i = 0; i < comps.Length; i++ )
				{
					if ( !comps[ i ] )
					{
						Reset();
						continue;
					}

					var accessor = GetReflectionFields(comps[i]);

					if ( !accessor.completed_thread ) continue;

					if ( !compsinitialized[ i ] )
					{
						if ( accessor.InitializeTarget_InMainThread( (comps[ i ]) ) )
							compsinitialized[ i ] = true;
					}

					moduleRect.height = SIZE.COMP();
					WRITE_ARROW_A_POSES( moduleRect, i );
					moduleRect.y += moduleRect.height + SIZE.SPACE();

					for ( int fIndex = 0; fIndex < accessor.faList.Length; fIndex++ )
					{
						moduleRect.height = SIZE.VAR();
						moduleRect.y += moduleRect.height + SIZE.SPACE();
					}
				}


				moduleRect.y = mre;
				drawIndex = 0;

				for ( int i = 0; i < comps.Length; i++ )
				{
					if ( !comps[ i ] )
					{
						Reset();
						continue;
					}

					var accessor = GetReflectionFields(comps[i]);

					if ( !accessor.completed_thread ) continue;

					if ( !compsinitialized[ i ] )
					{
						if ( accessor.InitializeTarget_InMainThread( (comps[ i ]) ) )
							compsinitialized[ i ] = true;
					}

					//if (f.Count == 0) continue;
					moduleRect.height = SIZE.COMP();

					////////////////COMPONENT
					content.text = comps[ i ].GetType().Name;
					content.tooltip = content.text + " : Component";
					HIPERUI_LINE_RDTRIANGLE.fontSize = Mathf.RoundToInt( (FONT_SIZE() - 1) * HALF_SCALE() );
					sizememory[ i ] = HIPERUI_LINE_RDTRIANGLE.CalcSize( content ).x / CURRENT_SCALE;
					sizememory[ i ] = moduleRect.width = Math.Min( sizememory[ i ] + 10, DEFAULTWIDTH( controller ) );

					if ( Event.current.type == EventType.Repaint )
					{ /* var ccc = GUI.color;
						     if (!EditorGUIUtility.isProSkin)GUI.color *= alpha;*/
						Draw( moduleRect, content, HIPERUI_LINE_RDTRIANGLE, false, false, false, false );
						// GUI.color = ccc;
					}

					TOOLTIP_WITH_SCALE( moduleRect, content );

					DO_ARROW_A( moduleRect, i );
					////////////////COMPONENT/////////////////////////////////////////////////
					moduleRect.y += moduleRect.height + SIZE.SPACE();

					for ( int fIndex = 0; fIndex < accessor.faList.Length; fIndex++ )
					{
						var value = accessor.faList[fIndex].GetValue(comps[i]) /*as UnityEngine.Object*/;
						// var asd = GUI.contentColor;
						bool isnull = Tools.IsObjectNull(value);
						/* if (isnull)
						 {   GUI.contentColor = adapter.par.HiperGraphParams.RED_HIGKLIGHTING ? Color.red : alpha;
						 }*/

						// MonoBehaviour.print(value);
						moduleRect.height = SIZE.VAR();
						var lineRect = moduleRect;
						lineRect.x += 6;
						lineRect.width = lineRect.height;


						if ( Event.current.type == EventType.Repaint )
						{
							var asdas = GUI.color;

							if ( isnull && adapter.par_e.HYPERGRAPH_DRAW_RED_FOR_NULLS > 0 )
							{
								//GUI.color *= adapter.par_e.HYPERGRAPH_RED_HIGKLIGHTING ? Color.red : alpha; 
								lineRect.x += lineRect.width / 3;
								lineRect.width /= 3;
								GUI.color *= Color.red;
							}
							else
							{
								//GUI.color *= alpha;
							}

							Draw( lineRect, HIPERUI_MARKER_BOX, false, false, false, false );
							GUI.color = asdas;
						}

						lineRect.x += lineRect.width;
						lineRect.width = DEFAULTWIDTH( controller ) - 6 - lineRect.width;

						// content.text = value == null ? "null" : value
						///////////////FIELD
						content.text = accessor.faList[ fIndex ].Name;
						content.tooltip = "var " + content.text;
						HIPERUI_LINE_BLUEGB_PERSONAL.fontSize = HIPERUI_LINE_BLUEGB.fontSize = Mathf.RoundToInt( (FONT_SIZE() - 1) * HALF_SCALE() );

						//  HIPERUI_LINE_BLUEGB_PERSONAL.fontSize = FONT_8() - 1;
						if ( Event.current.type == EventType.Repaint )
						{

							var ts = EditorGUIUtility.isProSkin  ? HIPERUI_LINE_BLUEGB : HIPERUI_LINE_BLUEGB_PERSONAL;



							//GUI.color *= adapter.par_e.HYPERGRAPH_RED_HIGKLIGHTING ? Color.red : alpha; 
							var otc = ts.normal.textColor;
							var oal = ts.alignment;
							var oc= ts.clipping;
							if ( isnull && adapter.par_e.HYPERGRAPH_DRAW_RED_FOR_NULLS > 1 ) ts.normal.textColor = Color.red;
							if ( adapter.par_e.HYPERGRAPH_FIELD_NAMES_ALIGNMENT == 1 ) ts.alignment = TextAnchor.MiddleRight;
							else ts.alignment = TextAnchor.MiddleLeft;
							if ( adapter.par_e.HYPERGRAPH_CLIP_NAMES ) ts.clipping = TextClipping.Clip;

							if ( adapter.par_e.HYPERGRAPH_CLIP_NAMES && adapter.par_e.HYPERGRAPH_FIELD_NAMES_ALIGNMENT == 1 )
								if ( lineRect.width * CURRENT_SCALE < ts.CalcSize( content ).x )
									ts.alignment = TextAnchor.MiddleLeft;

							try { Draw( lineRect, content, ts, false, false, false, false ); }
							catch { }

							if ( adapter.par_e.HYPERGRAPH_CLIP_NAMES ) ts.clipping = oc;
							if ( isnull && adapter.par_e.HYPERGRAPH_DRAW_RED_FOR_NULLS > 1 ) ts.normal.textColor = otc;
							ts.alignment = oal;

						}

						TOOLTIP_WITH_SCALE( lineRect, content );


						// if (SELF_TARGETS.ContainsKey()
						accessor.faList[ fIndex ].CheckID( selection_id, SEL_INC );

						if ( accessor.faList[ fIndex ].GetAllValuesCache == null || !accessor.faList[ fIndex ].GetAllValuesCache.ContainsKey( comps[ i ].GetInstanceID() ) )
						{
							if ( accessor.faList[ fIndex ].GetAllValuesCache == null ) accessor.faList[ fIndex ].GetAllValuesCache = new Dictionary<int, Dictionary<string, object>>();


							accessor.faList[ fIndex ].GetAllValuesCache.Add( comps[ i ].GetInstanceID(), accessor.faList[ fIndex ].GetAllValues( comps[ i ], 0, adapter.par_e.HYPERGRAPH_EVENTS_MODE | adapter.par_e.HYPERGRAPH_SKIP_ARRAYS ) );
						}

						foreach ( var item in accessor.faList[ fIndex ].GetAllValuesCache[ comps[ i ].GetInstanceID() ] )
						{
							var v = item.Value as UnityEngine.Object;

							if ( !v ) continue;

							DO_ARROW_B( controller, lineRect, v, fIndex );
						}

						/*// OLD SINLE VALUE
							DO_ARROW_B( controller , lineRect , value , fIndex );
						*/ // OLD SINLE VALUE
						   ///////////////FIELD/////////////////////////////////////////////////


						moduleRect.y += moduleRect.height + SIZE.SPACE();

						//GUI.contentColor = asd;
					}
				}

				DRAWARROWED_BEZIER_COMPOLETE();

				DRAWINPUTS( controller );


				//107 19 head
				//5 12 line box bg

				//10 12 arrow
				//18 7 line blue bg
				//39 12 line
				//7 7 marker
			}


			return SelectObject_height;
		}


		int CURRENT_COMP_INDEX;

		Rect? stateForDrag_B0;
		UnityEngine.Object stateForDrag_B1;

		bool rawOnUp( Events.MouseRawUp.WantMouseLeaveType t )
		{
			stateForDrag_B0 = null;
			stateForDrag_B1 = null;
			return true;
		}

		private void DO_BUTTON_FORSCROLL( ExternalDrawContainer controller, Rect rect, GUIContent content, GUIStyle style, int EVENT_ID,
			UnityEngine.Object newGo, Action action = null, bool draw3Pass = false )
		{
			_mConvertRect( ref rect );

			EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

			if ( Event.current.type == EventType.Repaint )
			{

				var oldClip = style.clipping;


				if ( draw3Pass )
				{
					var t = style.normal.background;
					var data = Icons.GetIconDataFromTexture( t );
					var color = GUI.color;

					//var D = 20* CURRENT_SCALE;
					//var L = 10* CURRENT_SCALE;
					var D = rect.height;
					var L = rect.height / 2;

					var uvl = 20;

					data.SetUV( 0, 0, 60, style.normal.background.height );
					data.SetTexture( t );
					var r = rect;
					r.width = D;
					adapter.gl._DrawTexture_ForExternalWindow( r, data, ref color );

					data.SetUV( 60, 0, style.normal.background.width - uvl - 60, style.normal.background.height );
					data.SetTexture( t );
					r.x += r.width;
					r.width = rect.width - (D + L);
					adapter.gl._DrawTexture_ForExternalWindow( r, data, ref color );

					data.SetUV( style.normal.background.width - uvl, 0, uvl, style.normal.background.height );
					data.SetTexture( t );
					r.x += r.width;
					r.width = L;
					adapter.gl._DrawTexture_ForExternalWindow( r, data, ref color );


					var bg = style.normal.background;
					style.normal.background = null;
					style.Draw( rect, content, false, false, false, false );
					style.normal.background = bg;

				}
				else
				{

					style.Draw( rect, content, false, false, false, false );
				}




				style.clipping = oldClip;


				TOOLTIP( rect, content );

				if ( actions.HOVER( EVENT_ID, rect, controller ) )
				{
					var h = rect;
					var GLOW = 8;
					h.x -= GLOW;
					h.y -= GLOW;
					h.width += GLOW * 2;
					h.height += GLOW * 2;


					HIPERUI_BUTTONGLOW.Draw( h, false, false, false, false );
				}
			}


			/* ------ */
			if ( newGo )
			{
				if ( Event.current.rawType == EventType.MouseUp )
				{
					rawOnUp( Events.MouseRawUp.WantMouseLeaveType.MouseUp );
				}

				//  if (Event.current.type == EventType.MouseDrag) Debug.Log( stateForDrag_B1 == newGo );
				//  if (stateForDrag_B0.HasValue ) adapter.RepaintWindowInUpdate();
				if ( stateForDrag_B0.HasValue )
				{
					var m = Event.current.mousePosition + Event.current.delta;
					var drag = !stateForDrag_B0.Value.Contains(m);
					drag |= m.x < 3;
					drag |= m.x > controller.WIDTH( IExternalWindowType.HYPER_GRAPH ) - 9;

					// Debug.Log( m.x + "  " + controller.WIDTH );
					if ( stateForDrag_B1 == newGo && Event.current.type == EventType.MouseDrag && drag ) // DragAndDrop.PrepareStartDrag();// reset data
					{
						adapter.ha.InternalClearDrag();
						if ( newGo )
						{
							DragAndDrop.objectReferences = new[] { (UnityEngine.Object)newGo };
							// drawComps = emptyArr;
							DragAndDrop.StartDrag( "Dragging HyperGraph UnityEngine.Object" );
							DragAndDrop.SetGenericData( "Dragging HyperGraph", true );
							// DragAndDrop.
							//  EventUse();

						}
						stateForDrag_B0 = null;
						adapter.RepaintWindowInUpdate( 0 );
						Tools.EventUse();
					}
				}

				if ( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
				{
					if ( Event.current.button == 0 )
					{
						if ( !stateForDrag_B0.HasValue )
						{
							/*if (controller.tempWin == adapter.window())
							{
								adapter.PUSH_ONMOUSEUP(rawOnUp);
							}

							else*/
							{
								var w = CURRENT_WIN;// as _6__BottomWindow_HyperGraphWindow;
													//controller.tempRoot.
								if ( !ReferenceEquals( w, null ) && w.OvValide() ) controller.tempRoot.PUSH_ONMOUSEUP( rawOnUp );
							}
						}

						stateForDrag_B0 = rect;
						stateForDrag_B1 = newGo;
					}
				}
			}

			/* ------ */


			if ( Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
				rect.Contains( Event.current.mousePosition ) )
			{
				Tools.EventUse();
				actions.ADD_ACTION( EVENT_ID, rect, contains => { return false; }, contains => {
					if ( contains )
					{
						if ( newGo != null )
						{
							Selection.objects = new UnityEngine.Object[] { newGo };
						}

						if ( action != null )
						{
							action();
						}
					}
				}, controller );
			}
		}

		private void SETUNDO()
		{
			if ( UndoList.Count != 0 && UndoList[ UndoList.Count - 1 ] == CurrentSelection ) return;

			if ( UndoPos < UndoList.Count - 1 ) UndoList.RemoveRange( UndoPos + 1, UndoList.Count - UndoPos - 1 );

			/* if (UndoList.Count == 0) UndoList.Add(CurrentSelection);
			 else UndoList.Insert(0, CurrentSelection);*/

			UndoList.Add( CurrentSelection );

			if ( UndoList.Count > MAX20 )
			{
				while ( UndoList.Count > MAX20 ) UndoList.RemoveAt( 0 );
			}

			UndoPos = UndoList.Count - 1;
		}

		private void DO_REDO()
		{
			if ( UndoPos >= UndoList.Count - 1 ) return;

			UndoPos++;
			// Selection.objects = new[] { UndoList[UndoPos] };
			skipUndo = true;
			// CHANGE_SELECTION_OVVERIDE(true);
			var n = UndoList[UndoPos];
			CHANGE_SELECTION_OVVERIDE( true, n );
		}

		private void DO_UNDO()
		{
			if ( UndoPos <= 0 ) return;

			UndoPos--;
			// Selection.objects = new[] { UndoList[UndoPos] };
			skipUndo = true;

			var n = UndoList[UndoPos];
			CHANGE_SELECTION_OVVERIDE( true, n );
			// CHANGE_SELECTION_OVVERIDE(true);
		}


		void WRITE_ARROW_A_POSES( Rect lineRect, int index )
		{
			lineRect.x -= 2 + SIZE.ARROW_WIDTH();
			lineRect.y = lineRect.y + lineRect.height / 2 - 6;
			lineRect.width = SIZE.ARROW_WIDTH();
			lineRect.height = SIZE.COMP();

			p1.x = lineRect.x + 3;
			p1.y = lineRect.y + lineRect.height / 2;
			comps_inPos[ index ] = p1;
		}

		private void DO_ARROW_A( Rect lineRect /*, UnityEngine.Object source*/, int index = -1 )
		{
			lineRect.x -= 2 + SIZE.ARROW_WIDTH();
			lineRect.y = lineRect.y + lineRect.height / 2 - 6;
			lineRect.width = SIZE.ARROW_WIDTH();
			lineRect.height = SIZE.COMP();

			if ( Event.current.type == EventType.Repaint )
				Draw( lineRect, HIPERUI_INOUT_A, false, false, false, false );

			if ( index != -1 )
			{ /*  p1.x = lineRect.x + 3;
				      p1.y = lineRect.y + lineRect.height / 2;
				      comps_inPos[index] = p1;*/

				if ( SKANNING )
				{
					lineRect.x -= SIZE.DEFAULT_PAD();
					lineRect.width = SIZE.DEFAULT_PAD();
					/*var asd = GUI.color;
					GUI.color = Color.yellow;*/
					LABEL( lineRect, scanningContent );
					//GUI.color = asd;
				}

				// if (source != null)
			}
		}




		private void DO_ARROW_B( ExternalDrawContainer controller, Rect lineRect, UnityEngine.Object target, int index )
		{
			//if ( adapter._S_HG_EventsMode && !(target is UnityEngine.Events.UnityEvent) ) return;

			lineRect.x += lineRect.width;
			lineRect.y -= 2;
			lineRect.width = SIZE.ARROW_WIDTH();
			lineRect.height = SIZE.COMP();

			var asd = GUI.color;

			if ( target == null && index != -1 ) GUI.color *= alpha;

			if ( Event.current.type == EventType.Repaint )
				Draw( lineRect, HIPERUI_INOUT_B, false, false, false, false );

			GUI.color = asd;


			/*   MonoBehaviour.print(target);
			   MonoBehaviour.print(target as Component);
			   MonoBehaviour.print(target as GameObject);
			   MonoBehaviour.print(Equals( target , null));*/
			if ( target != null ) // var FH = 35 + 4;
			{
				var fieldsHeight = TARGET_HEIGHT;
				var mid = GAMEOBJECTRECT.y + GAMEOBJECTRECT.height * 0.5f;

				p1.x = lineRect.x + lineRect.width + 5;
				p1.y = lineRect.y + lineRect.height / 2;

				p2.x = p1.x + SIZE.DEFAULT_PAD();
				p2.y = mid - fieldsHeight / 2 + TARGET_CURRENT_Y;

				/* DEBUG_HORISONTAL(mid, Color.blue);
				 DEBUG_HORISONTAL(p2.y, Color.red);*/
				//   MonoBehaviour.print(index + " " + fieldsCount);
				GRAPH_ARGS _a;
				var targetPoint = DRAWSINGLECOMP(controller, p2, target, DRAWING_INDEX, true, out _a);

				if ( _a.isAsset && !adapter.par_e.HYPERGRAPH_DISPLAY_ASSETS ) return;

				DRAWING_INDEX++;


				targetPoint.x += 2;

				if ( !_a.isBezier )
				{
					DRAWARROWED_LINE( p1, targetPoint );

					p2 = p1;
					p2.x -= 5;
					DRAWLINE( p1, p2 );
				}

				else
				{
					p1.x -= 9;
					// targetPoint.x += sizememory.x;
					// p1.x = targetPoint.x + 15;
					DRAWARROWED_BEZIER( CURRENT_COMP_INDEX, p1, targetPoint );
				}

				/* p1.y -= 2;
				 targetPoint.y -= 2;
				 DRAWARROWED_LINE(p1, targetPoint);*/
			}

			else // lineRect.x += lineRect.width;
			{ /*  lineRect.y -= 1;
				  lineRect.height += 2;
				  lineRect.width = 50;
				  content.text = "X";
				  var fs = Adapter.GET_SKIN().label.fontSize;
				  var a = Adapter.GET_SKIN().label.alignment;
				  var fb = Adapter.GET_SKIN().label.fontStyle;
				  Adapter.GET_SKIN().label.fontSize = HIPERUI_LINE_BLUEGB.fontSize;
				  Adapter.GET_SKIN().label.alignment = TextAnchor.MiddleLeft;
				  Adapter.GET_SKIN().label.fontStyle = FontStyle.Bold;
				  if (Event.current.type == EventType.repaint)
				      Adapter.GET_SKIN().label. Draw(lineRect, content, false, false, false, false);
				  Adapter.GET_SKIN().label.fontSize = fs;
				  Adapter.GET_SKIN().label.alignment = a;
				  Adapter.GET_SKIN().label.fontStyle = fb;*/
			}
		}

		float[] sizememory;

		//	bool bezier = false;
		//	bool isAsset;
		private Vector2 DRAWSINGLECOMP( ExternalDrawContainer controller, Vector2 startPos, UnityEngine.Object target, int index, bool DRAW_A, out GRAPH_ARGS _a,
			object fieldParams = null )
		{
			singleRect.x = startPos.x + 12;
			singleRect.y = startPos.y - 7;
			singleRect.width = DEFAULTWIDTH( controller );


			Component component = target as Component;
			GameObject gameObject = component != null ? component.gameObject : target as GameObject;
			// if (!go) continue;


			var isPrefab = gameObject && !gameObject.scene.IsValid();
			_a.isAsset = !gameObject;
			_a.isBezier = false;

			singleRect.height = SIZE.OBJECT();
			content.text = target.name;


			if ( _a.isAsset )
			{
				if ( !adapter.par_e.HYPERGRAPH_DISPLAY_ASSETS ) return startPos;

				var type = target.GetType().Name;
				content.tooltip = type.ToUpper() + ": " + content.text;
				content.text = "(" + type.ToLower() + ")" + content.text;

				component = null;
			}

			else if ( isPrefab )
			{
				content.tooltip = "PREFAB: " + content.text;
				content.text = "(prefab)" + content.text;
			}

			else
			{
				content.tooltip = content.text + " - GAMEOBJECT";
			}

			if ( gameObject ) target = gameObject;

			var ID = target.GetInstanceID();

			// if (isAsset) Debug.Log(DRAW_A);

			HIPERUI_GAMEOBJECT.fontSize = Mathf.RoundToInt( (FONT_SIZE() - 1) * HALF_SCALE() );

			_a.isBezier = false;

			if ( DRAW_A )
			{
				if ( !TARGET_COMPS.ContainsKey( ID ) )
				{ //if ( isAsset ) Debug.Log( DRAW_A );
					return startPos;
				}

				var dis = TARGET_COMPS[ID];

				if ( target == CurrentSelection )
				{
					CURRENT_COMP_INDEX = component != null ? compsSorted[ component.GetInstanceID() ] : compsSorted.Count;
					_a.isBezier = true;
					return comps_inPos[ CURRENT_COMP_INDEX ];
					/* DRAW_B_POS
					 return comp != null ? dis.DRAW_A_POSES[dis.objecComps[comp.GetInstanceID()]] : dis.DRAW_A_POSES[dis.objecComps.Count];*/
				}

				else if ( !dis.DRAW )
				{
					dis.DRAW = true;

					var asd = GUI.color;

					if ( adapter.par_e.HYPERGRAPH_EVENTS_MODE_BOOL ) GUI.color *= event_color;
					else if ( isPrefab ) GUI.color *= prefab_color;
					else if ( _a.isAsset ) GUI.color *= asset_color;

					DO_BUTTON_FORSCROLL( controller, singleRect, content, HIPERUI_GAMEOBJECT, DRAWSINGLECOMP_TARGET_EVENT_START + index,
						target, draw3Pass: true );
					GUI.color = asd;

					DO_ARROW_A( singleRect );
					ARROW_RECT = singleRect;
					ARROW_RECT.y += ARROW_RECT.height / 2;
					ARROW_RECT.x -= 2 + SIZE.ARROW_WIDTH();
					DRAWSINGLECOMP_RESULT.Set( ARROW_RECT.x /*+ 5*/, ARROW_RECT.y );
					dis.DRAW_A_POSES[ dis.objecComps.Count ] = DRAWSINGLECOMP_RESULT;


					foreach ( var i in dis.objecComps ) //  }
					{ // if (comp != null && !prefab)
					  // {
						singleRect.y += singleRect.height;
						singleRect.height = SIZE.COMP();
						var ob = EditorUtility.InstanceIDToObject(i.Key);

						if ( !ob ) continue;

						content.text = (ob).GetType().Name;
						content.tooltip = content.text + " : Component";
						HIPERUI_LINE_RDTRIANGLE.fontSize = Mathf.RoundToInt( (FONT_SIZE() - 1) * HALF_SCALE() );
						var size = HIPERUI_LINE_RDTRIANGLE.CalcSize(content);
						size.x = singleRect.width = Math.Min( size.x + 10, DEFAULTWIDTH( controller ) );
						DO_BUTTON_FORSCROLL( controller, singleRect, content, HIPERUI_LINE_RDTRIANGLE,
							DRAWSINGLECOMP_TARGET_EVENT_M2 + index, target );
						DO_ARROW_A( singleRect );


						ARROW_RECT = singleRect;
						ARROW_RECT.y += ARROW_RECT.height / 2;
						ARROW_RECT.x -= 2 + SIZE.ARROW_WIDTH();
						DRAWSINGLECOMP_RESULT.Set( ARROW_RECT.x /*+ 5*/, ARROW_RECT.y );
						dis.DRAW_A_POSES[ i.Value ] = DRAWSINGLECOMP_RESULT;
					}


					TARGET_CURRENT_Y += dis.height;
				}


				return component != null ? dis.DRAW_A_POSES[ dis.objecComps[ component.GetInstanceID() ] ] : dis.DRAW_A_POSES[ dis.objecComps.Count ];
			}


			// DRAW_B /////////////
			{
				var dis = INPUT_COMPS[ID];

				if ( !dis.DRAW )
				{
					dis.DRAW = true;

					var c = GUI.color;
					if ( adapter.par_e.HYPERGRAPH_EVENTS_MODE_BOOL ) GUI.color *= event_color;

					DO_BUTTON_FORSCROLL( controller, singleRect, content, HIPERUI_GAMEOBJECT, DRAWSINGLECOMP_TARGET_EVENT_START + index,
						target, draw3Pass: true );

					GUI.color = c; ;

					foreach ( var CompFields in dis.AllFields )
					{
						singleRect.y += singleRect.height;
						singleRect.height = SIZE.COMP();
						var ob = EditorUtility.InstanceIDToObject(CompFields.Key);

						if ( !ob ) continue;

						content.text = (ob).GetType().Name;
						content.tooltip = content.text + " : Component";
						HIPERUI_LINE_RDTRIANGLE_INVERSE.fontSize = Mathf.RoundToInt( (FONT_SIZE() - 1) * HALF_SCALE() );
						var size = HIPERUI_LINE_RDTRIANGLE_INVERSE.CalcSize(content);
						size.x = singleRect.width = Math.Min( size.x + 10, DEFAULTWIDTH( controller ) );
						DO_BUTTON_FORSCROLL( controller, singleRect, content, HIPERUI_LINE_RDTRIANGLE_INVERSE,
							DRAWSINGLECOMP_TARGET_EVENT_M2 + index, target );
						// DO_ARROW_B(singleRect, null, -1);


						foreach ( var field1 in CompFields.Value )
						{
							singleRect.y += singleRect.height;
							singleRect.width = DEFAULTWIDTH( controller );
							singleRect.height = SIZE.VAR();

							content.text = field1.Key;
							content.tooltip = "var " + content.text;
							HIPERUI_LINE_BLUEGB_PERSONAL.fontSize = HIPERUI_LINE_BLUEGB.fontSize = Mathf.RoundToInt( (FONT_SIZE() - 1) * HALF_SCALE() );

							// HIPERUI_LINE_BLUEGB.fontSize = FONT_8() - 1;
							if ( Event.current.type == EventType.Repaint )
							{

								//if ( adapter.par_e.HYPERGRAPH_RED_HIGKLIGHTING || EditorGUIUtility.isProSkin ) Draw( singleRect, content, HIPERUI_LINE_BLUEGB, false, false, false, false );
								//else Draw( singleRect, content, HIPERUI_LINE_BLUEGB_PERSONAL, false, false, false, false );
								// HIPERUI_LINE_BLUEGB. Draw(singleRect, content, false, false, false, false);


								var ts = EditorGUIUtility.isProSkin  ? HIPERUI_LINE_BLUEGB : HIPERUI_LINE_BLUEGB_PERSONAL;

								Draw( singleRect, content, ts, false, false, false, false );

								//if ( isnull && adapter.par_e.HYPERGRAPH_DRAW_RED_FOR_NULLS > 1 )
								//{
								//	//GUI.color *= adapter.par_e.HYPERGRAPH_RED_HIGKLIGHTING ? Color.red : alpha; 
								//	var otc = ts.normal.textColor;
								//	ts.normal.textColor = Color.red;
								//	try { Draw( lineRect, content, ts, false, false, false, false ); }
								//	catch { }
								//	ts.normal.textColor = otc;
								//}
								//else
								//{
								//	//GUI.color *= alpha;
								//	Draw( lineRect, content, ts, false, false, false, false );
								//}


							}

							TOOLTIP_WITH_SCALE( singleRect, content );
							// GUI .Label(singleRect, content);
							DO_ARROW_B( controller, singleRect, null, -1 );


							ARROW_RECT = singleRect;
							ARROW_RECT.y += ARROW_RECT.height / 2;
							ARROW_RECT.x += ARROW_RECT.width + 12 + 3;
							DRAWSINGLECOMP_RESULT.Set( ARROW_RECT.x /*+ 5*/, ARROW_RECT.y );
							dis.DRAW_B_POSES[ field1.Value ] = DRAWSINGLECOMP_RESULT;
						}
					}

					INPUT_CURRENT_Y += dis.height;
				}

				var FP = (FIELD_PARAMS)fieldParams;


				if ( _a.isAsset ) return startPos;

				return dis.DRAW_B_POSES[ dis.AllFields[ component.GetInstanceID() ][ FP.field.Name ] ];
				// return dis;
			}
		}

		private void DRAWARROWED_LINE( Vector2 p1, Vector2 p2, Color? col = null )
		{
			if ( Event.current.type != EventType.Repaint || p1 == p2 ) return;

			/*  p1 = EditorGUIUtility.GUIToScreenPoint(p1);
			  p2 = EditorGUIUtility.GUIToScreenPoint(p2);*/
			/* p1.x += RECT.x;
			 p1.y += RECT.y;*/
			GL_BEGIN();

			var c = col ?? (EditorGUIUtility.isProSkin ? arrowC1 : arrowC1personal);
			GL.Color( c );
			//	mat.SetColor(adapter._Color, col ?? Color.white);
			ALIAS( p1, p2, c );
			GL_VERTEX3( p1 );
			GL_VERTEX3( p2 );
			//GL.GL_VERTEX3Vertex3(p2.x, p2.y - 0.1f, 1);
			//GL.GL_VERTEX3Vertex3(p1.x-0.1f, p1.y, 0);
			//DRAWLINE( p1, p2, col );

			m_DrawArrow( p1, p2, col, c );
			GL_END();

			/*var ARROW_SIZE = 10;
			var arr = (p1 - p2).normalized * ARROW_SIZE;
			var a1 = arr;
			a1.y = -a1.x;
			a1.x = arr.y;
			var a2 = -a1;


			var s1 = Vector2.Lerp(arr, a1, 0.2f) + p2;
			var s2 = Vector2.Lerp(arr, a2, 0.2f) + p2;
			var len = ARROW_SIZE /#1# 2#1#;
			//  MonoBehaviour.print( p2);
			var c = col ?? (EditorGUIUtility.isProSkin ? arrowC1 : arrowC1personal);
			for (float i = 0; i < len - 1; i += 0.5f)
			{
				var t1 = Vector2.Lerp(s1, p2, i / (len - 1f));
				var t2 = Vector2.Lerp(s2, p2, i / (len - 1f));
				DRAWLINE(t1, t2, c);
			}*/

			//GL.PopMatrix();

			/*  var matrixBackup = GUI.matrix;
			  var n = (p1 - p2).normalized;
			  var dot = Vector2.Dot(Vector2.down, n) < 0 ? -1 : 1;


			  Graphics.DrawTexture(
			  GUIUtility.RotateAroundPivot(dot * Vector2.Angle(Vector2.left, n), p2);
			  var dist = (p1 - p2).magnitude;
			  arrowRect.x = p2.x - dist;
			  arrowRect.width = dist;
			  arrowRect.height = 10;
			  arrowRect.y = p2.y - 5;
			  ARROW. Draw(arrowRect, false, false, false, false);
			  GUI.matrix = matrixBackup;*/
		}


		int[] COMPIND = new int[100];
		Vector2[] BA0 = new Vector2[100];
		Vector2[] BA1 = new Vector2[100];
		Vector2[] BA2 = new Vector2[100];
		Vector2[] BA3 = new Vector2[100];
		Color?[] BAC = new Color?[100];

		bool[] BAassign = new bool[100];

		/* PolyLineSegment GetBezierApproximation( int outputSegmentCount)
		 {
			 //Point[] points = new Point[outputSegmentCount + 1];
			 for (int i = 0; i <= outputSegmentCount; i++)
			 {
				 double t = (double)i / outputSegmentCount;
				 bezierarray[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
			 }
			 return new PolyLineSegment(points, true);
		 }*/
		Vector2 m__tb;

		Vector2 GetBezierPoint( int getindex, float t, int index, int count )
		{
			if ( count == 1 )
			{
				switch ( index )
				{
					case 0: return BA0[ getindex ];

					case 1: return BA1[ getindex ];

					case 2: return BA2[ getindex ];

					case 3: return BA3[ getindex ];
				}
			}

			var P0 = GetBezierPoint(getindex, t, index, count - 1);
			var P1 = GetBezierPoint(getindex, t, index + 1, count - 1);
			m__tb.Set( (1 - t) * P0.x + t * P1.x, (1 - t) * P0.y + t * P1.y );
			return m__tb;
		}

		/*    float t_1;
		   int index = 0;
			Vector2 GetBezierPoint4(float t)
		   {
			   t_1 = 1 - t;
			   Math.Pow(t_1, 3) * BA[3].x + 3 * t * t_1 * t_1!
				 (1−t)3p1x + 3t(1−t)2p2x + 3t2(1−t)p3x + t3p4x

			   if (count == 1) return BA[index];
			   var P0 = GetBezierPoint(t, index, count - 1);
			   var P1 = GetBezierPoint(t, index + 1, count - 1);
			   m__tb.Set((1 - t) * P0.x + t * P1.x, (1 - t) * P0.y + t * P1.y);
			   return m__tb;
		   }*/
		Vector2 tp1, tp2;
		int drawIndex = 0;

		private void DRAWARROWED_BEZIER( int compIndex, Vector2 p1, Vector2 p2, Color? col = null )
		{
			if ( Event.current.type != EventType.Repaint || p1 == p2 ) return;

			/*  p1 = EditorGUIUtility.GUIToScreenPoint(p1);
			  p2 = EditorGUIUtility.GUIToScreenPoint(p2);*/
			/* p1.x += RECT.x;
			 p1.y += RECT.y;*/
			// p2.x += size.x;
			// if ( Mathf.Abs( p1.x - p2.x ) < 20 ) p2.x = p1.x - 20;

			if ( !adapter.par_e.HYPERGRAPH_CONNECT_TO_SELFT ) return;

			BA0[ drawIndex ] = p1;
			BA1[ drawIndex ] = p1;
			BA2[ drawIndex ] = p2;
			BA3[ drawIndex ] = p2;
			/*    BA1[drawIndex].y = p2.y;
				BA2[drawIndex].y = p1.y;*/

			/*  BA2[drawIndex].x = BA1[drawIndex].x;
			  //  BA[1].x += 15;
			  BAC[drawIndex] = col;*/
			BAC[ drawIndex ] = col;
			BAassign[ drawIndex ] = true;

			COMPIND[ drawIndex ] = compIndex;

			drawIndex++;

			if ( drawIndex >= BAassign.Length )
			{
				Array.Resize( ref COMPIND, drawIndex + 1 );
				Array.Resize( ref BA0, drawIndex + 1 );
				Array.Resize( ref BA1, drawIndex + 1 );
				Array.Resize( ref BA2, drawIndex + 1 );
				Array.Resize( ref BA3, drawIndex + 1 );
				Array.Resize( ref BAC, drawIndex + 1 );
				Array.Resize( ref BAassign, drawIndex + 1 );
			}

			BAassign[ drawIndex ] = false;
		}

		private void DRAWARROWED_BEZIER_COMPOLETE()
		{
			if ( Event.current.type != EventType.Repaint ) return;

			GL_BEGIN();
			//GL.Color(BAC[jjj] ??  (EditorGUIUtility.isProSkin ? arrowC1 : arrowC1personal));
			var c = (EditorGUIUtility.isProSkin ? arrowC1 : arrowC1personal);
			GL.Color( c );


			for ( int LLI = 0; LLI < drawIndex; LLI++ )
			{
				if ( !BAassign[ LLI ] ) break;

				BA3[ LLI ] = comps_inPos[ COMPIND[ LLI ] ];
				BA3[ LLI ].x += (9 + 3);
				//BA3[ LLI ].x += sizememory[ COMPIND[ LLI ] ] + (9 + 3);


				// BA3[jjj].x += sizememory[COMPIND[jjj]] + (9 + 3) * CURRENT_SCALE;
				// BA3[jjj].y += 4;
				BA2[ LLI ] = BA3[ LLI ];
				var d = (BA3[ LLI ].x + BA1[ LLI ].x) ;
				BA1[ LLI ].x = d;
				BA2[ LLI ].x = (BA0[ LLI ].x - BA3[ LLI ].x) / 1.1f + BA3[ LLI ].x;

				BA2[ LLI ].y = Mathf.Lerp( BA1[ LLI ].y, BA2[ LLI ].y, .6f );
				//BA1[ LLI ].y = Mathf.Lerp( BA1[ LLI ].y, BA2[ LLI ].y, .5f );

				tp2 = BA0[ LLI ];
				var L = 20;
				var II = L / 2;
				//mat.SetColor(adapter._Color, BAC[jjj] ?? Color.white);

				for ( int i = 0; i < L; i++ )
				{
					tp1 = tp2;
					tp2 = GetBezierPoint( LLI, i / (L - 1f), i / II, 3 );
					ALIAS( tp1, tp2, c );
					GL_VERTEX3( tp1 );
					GL_VERTEX3( tp2 );

					//GL. Vertex3(tp2.x, tp2.y-0.1f, 1);
					//GL. Vertex3(tp1.x-0.1f, tp1.y, 0);
				}

				m_DrawArrow( tp1, tp2, BAC[ LLI ], c );
			}

			GL_END();
		}



		void m_DrawArrow_( Vector2 p1, Vector2 p2, Color? col = null )
		{
			var ARROW_SIZE = 10;
			var arr = (p1 - p2).normalized * ARROW_SIZE;
			var a1 = arr;
			a1.y = -a1.x;
			a1.x = arr.y;
			var a2 = -a1;


			var s1 = Vector2.Lerp(arr, a1, 0.2f) + p2;
			var s2 = Vector2.Lerp(arr, a2, 0.2f) + p2;
			float len = ARROW_SIZE /*/ 2*/;
			//  MonoBehaviour.print( p2);
			var c = col ?? (EditorGUIUtility.isProSkin ? arrowC1 : arrowC1personal);

			for ( float i = 0; i < len - 1; i += 0.5f / CURRENT_SCALE )
			{
				var t1 = Vector2.Lerp(s1, p2, i / (len - 1f));
				var t2 = Vector2.Lerp(s2, p2, i / (len - 1f));
				DRAWLINE( t1, t2, c );
			}
		}

		void m_DrawArrow( Vector2 p1, Vector2 p2, Color? col, Color glColor )
		{
			var ARROW_SIZE = 10;
			var arr = (p1 - p2).normalized * ARROW_SIZE;
			var a1 = arr;
			a1.y = -a1.x;
			a1.x = arr.y;
			var a2 = -a1;


			var s1 = Vector2.Lerp(arr, a1, 0.2f) + p2;
			var s2 = Vector2.Lerp(arr, a2, 0.2f) + p2;
			float len = ARROW_SIZE /*/ 2*/;
			//  MonoBehaviour.print( p2);
			//var c = col ?? (EditorGUIUtility.isProSkin ? arrowC1 : arrowC1personal);

			for ( float i = 0; i < len - 1; i += 0.5f / CURRENT_SCALE )
			{
				var t1 = Vector2.Lerp(s1, p2, i / (len - 1f));
				var t2 = Vector2.Lerp(s2, p2, i / (len - 1f));
				ALIAS( t1, t2, glColor );
				GL_VERTEX3( t1 );
				GL_VERTEX3( t2 );

				//GL.Ver tex3(t2.x, t2.y-0.1f, 0);
				//GL.V ertex3(t2.x-0.1f, t2.y, 0);
			}
		}


		private void DRAWLINE_( Vector2 p1, Vector2 p2, Color? color = null ) // var oc = GUI.color;
		{ //  MonoBehaviour.print(p1 + " " +  p2);
			if ( ScreenRect == null )
				ScreenRect = new Rect( 0, 0, Screen.currentResolution.width, Screen.currentResolution.height );

			bool issX = Math.Abs(p2.x - p1.x) > Math.Abs(p2.y - p1.y);
			// var leng = issX ? p2.x - p1.x : p2.y - p1.y;
			float left = issX ? (int)p1.x : (int)p1.y;
			float right = issX ? (int)p2.x : (int)p2.y;
			float d = Math.Abs(right - (float)left);
			float step = right - left < 0 ? -1f : 1f;
			step /= CURRENT_SCALE;

			/*  GL.PushMatrix();
			  GL.LoadPixelMatrix();*/
			//GL.LoadPixelMatrix(0, 0 + RECT.width , 0 + RECT.height, 0);
			for ( float i = left; step > 0 ? i <= right : i >= right; i += step )
			{
				if ( issX )
				{
					point.x = i;
					point.y = Mathf.Lerp( p1.y, p2.y, Math.Abs( i - left ) / d );
				}

				else
				{
					point.x = Mathf.Lerp( p1.x, p2.x, Math.Abs( i - left ) / d );
					point.y = i;
				}

				// GL.
				// GUI.DrawTexture(point, Texture2D.whiteTexture);
				if ( point.x < LOCAL_RECT.x || point.y < LOCAL_RECT.y || point.x > LOCAL_RECT.x + LOCAL_RECT.width || point.y > LOCAL_RECT.y + LOCAL_RECT.height )
				{
					continue;
				}

				/*point.width = 1;
				point.height = 1;*/
				if ( color.HasValue )
				{
					DrawTexture_Unscalable( point, ScreenRect.Value, color.Value );
					/* Graphics.DrawTexture( point, Texture2D.whiteTexture, ScreenRect.Value, 0, 0, 0, 0,
						 color.Value );*/
				}

				else
				{ /* point.x -= 1;
					     Graphics.DrawTexture(point, Texture2D.whiteTexture, ScreenRect.Value, 0, 0, 0, 0, arrowC2,null,-1);
					     Graphics.DrawTexture(point, Texture2D.whiteTexture, ScreenRect.Value, 0, 0, 0, 0, arrowC2,null,-1);*/
					DrawTexture_Unscalable( point, ScreenRect.Value, EditorGUIUtility.isProSkin ? arrowC1 : arrowC1personal );
					// Graphics.DrawTexture( point, Texture2D.whiteTexture, ScreenRect.Value, 0, 0, 0, 0, EditorGUIUtility.isProSkin ? arrowC1 : arrowC1personal );
					/*  point.x += 1;
					point.y -= 1;
					  Graphics.DrawTexture(point, Texture2D.whiteTexture, ScreenRect.Value, 0, 0, 0, 0, arrowC2);*/
				}


				// Graphics.DrawTexture(point, Texture2D.whiteTexture);
			}

			//GUI.color = oc;
		}



		private void DRAWLINE( Vector2 p1, Vector2 p2, Color? color = null ) // var oc = GUI.color;
		{
			if ( Event.current.type != EventType.Repaint ) return;

			GL_BEGIN();

			var c=  color ?? (EditorGUIUtility.isProSkin ? arrowC1 : arrowC1personal);
			//	mat.SetColor(adapter._Color, color ?? Color.white);
			ALIAS( p1, p2, c );
			GL.Color( c );
			GL_VERTEX3( p1 );
			GL_VERTEX3( p2 );
			GL_END();
		}

		void ALIAS( Vector3 p1, Vector3 p2, Color _c )
		{
			var c = _c;
			c.a *= 0.5f;
			do_al( ref c, 0.3f, ref p1, ref p2 );
			c.a *= 0.5f;
			do_al( ref c, 0.5f, ref p1, ref p2 );
			GL.Color( _c );
		}
		void do_al( ref Color c, float D, ref Vector3 p1, ref Vector3 p2 )
		{
			if ( !bold_lines ) return;
			GL.Color( c );
			p1.x -= D;
			p2.x -= D;
			p1.y -= D;
			p2.y -= D;
			GL_VERTEX3( p1 );
			GL_VERTEX3( p2 );
			p1.x += 2 * D;
			p2.x += 2 * D;
			GL_VERTEX3( p1 );
			GL_VERTEX3( p2 );
			p1.y += 2 * D;
			p2.y += 2 * D;
			GL_VERTEX3( p1 );
			GL_VERTEX3( p2 );
			p1.x -= 2 * D;
			p2.x -= 2 * D;
			GL_VERTEX3( p1 );
			GL_VERTEX3( p2 );
		}

		Material mat;

		internal void GL_BEGIN()
		{
			adapter.gl.GL_BEGIN();

			//GL.End();
			//GL.Begin( GL.QUADS );

		}

		internal void GL_END()
		{
			adapter.gl.GL_END();
		}

		/*	internal void GL_DRAW()
			{	if (Event.current.type != EventType.Repaint) return;




				foreach (var item in glStack)
				{	mat.SetTexture(_MainTex, item.texture);
					GL.Color(item.color);
					GL.TexCoord(new Vector3(0, 0, 0));
					GL.TexCoord(new Vector3(0, 1, 0));
					GL.TexCoord(new Vector3(1, 1, 0));
					GL.TexCoord(new Vector3(1, 0, 0));
					GL. Vertex3(item.rect.x, item.rect.y, 0);
					GL. Vertex3(item.rect.x, item.rect.y + item.rect.height, 0);
					GL. Vertex3(item.rect.x + item.rect.width, item.rect.y + item.rect.height, 0);
					GL. Vertex3(item.rect.x + item.rect.width, item.rect.y, 0);
				}


				//}
			}*/


		float? _FAST;

		float FAST {
			get {
				if ( _FAST.HasValue ) return _FAST.Value;

				if ( SystemInfo.processorFrequency >= 4000 ) return (_FAST = 0.5f).Value;

				if ( SystemInfo.processorFrequency < 3000 ) return (_FAST = 1.5f).Value;

				return (_FAST = 1.0f).Value;
			}
		}


		/*	private void DEBUG_HORISONTAL(float y, Color color)
            {
                var asd = GUI.color;
                GUI.color *= color;
                //	GUI.DrawTexture(new Rect(0, y, adapter.window().position.width, 2), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(0, y, position.width, 2), Texture2D.whiteTexture);
                GUI.color = asd;
            }*/

		//GRAPH_ARGS temp_args;
		internal struct GRAPH_ARGS
		{
			internal bool isAsset;
			internal bool isBezier;
		}

		private void DRAWINPUTS( ExternalDrawContainer controller )
		{
			var x = GAMEOBJECTRECT.x - SIZE.DEFAULT_PAD() - DEFAULTWIDTH(controller) - 12 - SIZE.DEFAULT_PAD() - 10;
			// INPUT_RECT.width = DEFAULT_WIDTH;

			// var FH = 35 + 7 + 4;
			// var fieldsHeight = findedList.Count * FH;
			var fieldsHeight = INPUT_COMPS.Values.Sum(v => v.height);
			var mid = GAMEOBJECTRECT.y + GAMEOBJECTRECT.height * 0.5f;
			/*   DEBUG_HORISONTAL(GAMEOBJECTRECT.y,Color.red);
			   DEBUG_HORISONTAL(GAMEOBJECTRECT.y + GAMEOBJECTRECT.height * 0.5f,Color.blue);*/
			//  INPUT_RECT.height = FH;


			/*     p1.x = lineRect.x + lineRect.width + 5;
				 p1.y = lineRect.y + lineRect.height / 2;

				 p2.x = p1.x + DEFAULT_PAD;
				 p2.y = mid - fieldsHeight / 2 + index * FH;*/

			var i = 0;
			INPUT_CURRENT_Y = 0;

			foreach ( var fieldParams in findedList )
			{
				var ob = EditorUtility.InstanceIDToObject(fieldParams.Key);

				if ( !ob )
				{
					StoptBroadcasting();
					StartBroadcasting();
					return;
				}


				p2.x = x;
				p2.y = mid - fieldsHeight / 2 + INPUT_CURRENT_Y;

				// DEBUG_HORISONTAL(mid, Color.blue);
				// DEBUG_HORISONTAL(p2.y, Color.red);
				GRAPH_ARGS _a;
				p1 = DRAWSINGLECOMP( controller, p2, ob, DRAWSINGLECOMP_TARGET_EVENT_INPUT + i, false, out _a,
					fieldParams.Value );

				if ( _a.isAsset && !adapter.par_e.HYPERGRAPH_DISPLAY_ASSETS ) continue;

				// DEBUG_HORISONTAL(p1.y, Color.yellow);

				for ( int tI = 0; tI < fieldParams.Value.targetGameObject.Count; tI++ )
				{
					if ( !fieldParams.Value.targetGameObject[ tI ] ) continue;

					p2 = comps_inPos[ comps_inPos.Length - 1 ];
					DRAWARROWED_LINE( (p1), (p2) );
					p2 = p1;
					p2.x -= 5;
					DRAWLINE( (p1), (p2) );
				}

				for ( int tI = 0; tI < fieldParams.Value.targetComponent.Count; tI++ )
				{
					if ( !fieldParams.Value.targetComponent[ tI ] ) continue;

					var findIndex = -1;
					var cc = fieldParams.Value.targetComponent[tI].GetInstanceID();

					if ( compsSorted.ContainsKey( cc ) ) findIndex = compsSorted[ cc ];

					/* for ( int j = 0 ; j < comps.Length ; j++ ) {
						 if ( cc == comps[j] ) {
							 findIndex = j;
							 break;
						 }
					 }*/
					if ( findIndex == -1 ) continue;

					p2 = comps_inPos[ findIndex ];
					DRAWARROWED_LINE( (p1), (p2) );
					p2 = p1;
					p2.x -= 5;
					DRAWLINE( (p1), (p2) );
				}


				/*// OLD SINLE VALUE
				if ( fieldParams.Value.targetGameObject != null ) {
				p2 = comps_inPos[comps_inPos.Length - 1];
				} else {
				var findIndex = -1;
				var cc = fieldParams.Value.targetComponent;
				for ( int j = 0 ; j < comps.Length ; j++ ) {
				if ( cc == comps[j] ) {
					findIndex = j;
					break;
				}
				}
				if ( findIndex == -1 ) continue;
				p2 = comps_inPos[findIndex];
				}



				// p1.x += 12;
				DRAWARROWED_LINE( (p1) , (p2) );
				// DRAWARROWED_LINE( _mConvertRect( p1 ), _mConvertRect( p2 ) );

				p2 = p1;
				p2.x -= 5;
				DRAWLINE( (p1) , (p2) );
				// DRAWLINE( _mConvertRect( p1 ), _mConvertRect( p2 ) );
				*/ // OLD SINLE VALUE

				i++;
			}
		}





		private void StoptBroadcasting()
		{
			currentList = null;
			findedList.Clear();
			INPUT_COMPS.Clear();
			SKANNING = false;
			ERROR = false;
			WASSCAN = false;
			//Debug.Log( "stop" );
		}

		private void StartBroadcasting()
		{
			if ( SKANNING ) return;

			// MonoBehaviour.print("ASD");
			///Debug.Log( "ASD" );
			//Debug.Log( "ASD" );

			WASSCAN = true;
			SKANNING = true;
			currentList = Tools.AllSceneObjectsInterator( 0, true ).GetEnumerator();
			CountProgress = Tools.AllSceneObjectsInteratorCount( 0, true );
			findedList.Clear();
			INPUT_COMPS.Clear();
			currentIndex = 0;

			if ( adapter.par_e.HYPERGRAPH_SCANPERFOMANCE == 10 ) CalcBroadCast();
		}

		// int fdsHotControl = 0;
		double time;
		static int selection_id = -1;
		static int SEL_INC = 1;
		System.Diagnostics.Stopwatch WATCH_CLONE = System.Diagnostics.Stopwatch.StartNew();

		internal void CalcBroadCast()
		{
			if ( !SKANNING || !adapter.par_e.ENABLE_ALL || currentList == null ||
				!CurrentSelection ) return;

			if ( comps == null ) INIT_COMPS();

			// fdsHotControl = 0;
			var stopped = false;
			double counter = 0;
			bool start = false;

			while ( currentList.MoveNext() )
			// foreach (var current in currentList)
			//  while (currentIndex < currentList.Count)
			{
				if ( !start )
				{
					WATCH_CLONE.Start();
					start = true;
				}


				var current = currentList.Current;

				if ( ! /*currentList[currentIndex]*/current.Active() ) //var last2 = currentIndex;
				{
					currentIndex = Tools.AllSceneObjectsInteratorProgress();
					// if (last2 != currentIndex) Repaint();
					//currentIndex++;
					continue;
				}


				if ( (current.go.hideFlags & Tools.SearchFlags) == Tools.SearchFlags ) //  var last2 = currentIndex;
				{
					currentIndex = Tools.AllSceneObjectsInteratorProgress();
					//  if (last2 != currentIndex) Repaint();
					// currentIndex++;
					continue;
				}



				if ( current.go && current.go != CurrentSelection )
				{
					//	var currentComps = HierarchyExtensions.Utilities.GetComponentFast<Component>.GetAll(current.go);
					var currentComps = current.GetComponents();
					// var activeComps = new Dictionary<Component, int>();
					// var fieldsCount = 0;

					var result = new ObjectDisplay(current.go.GetInstanceID(), this)
					{
						fAccessor =
							new Dictionary<int, FieldsAccessor>(),
						GetComponents = currentComps
					};

					for ( int i = 0; i < currentComps.Length; i++ )
					{
						if ( !currentComps[ i ] ) continue;


						/*  if ("MoodBoxes" == current.name)
							  MonoBehaviour.print(current.name); */

						var f = GetReflectionFields(currentComps[i], result);
						result.fAccessor.Add( currentComps[ i ].GetInstanceID(), f );
					}

					/*     resilt.comps = activeComps;
						 resilt.DRAW_A_POSES = new Vector2[activeComps.Count];
						 resilt.DRAW_B_POSES = new Vector2[fieldsCount];
						 findedComopnentsAttached.Add(currentList[currentIndex].GetInstanceID(), resilt);*/


					if ( !result.WasAccessorInitialize_InMainThread )
					{
						if ( result.AllowInitialize_InMainThread() )
						{
							result.InitializeAccessor_InMainThread();
						}

						else // SCANNING_COMPS.Add(current.GetInstanceID(), result);
						{ }
					}
				}


				/* try
				 {
					 brc(dsc, currentList[currentIndex]);
				 }
				 catch
				 {
					 Clear();
					 currentIndex = 0;
					 dsc.Broadcasting = false;
					 ERROR = true;
					 dsc.WasFirst = false;
					 return;
				 }*/
				// var last = currentIndex;
				currentIndex = Tools.AllSceneObjectsInteratorProgress();
				// if (last != currentIndex) Repaint();
				//     if ( Math.Abs( time - EditorApplication.timeSinceStartup ) > 0.5f )

				if ( Math.Abs( time - EditorApplication.timeSinceStartup ) > FAST )
				{
					time = EditorApplication.timeSinceStartup;
					RepaintNow(); //WindowHyperController
				}

				//currentIndex++;

				start = false;
				WATCH_CLONE.Stop();
				counter += WATCH_CLONE.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency;
				WATCH_CLONE.Reset();


				// if (

				if ( //par.HiperGraphParams.SCANPERFOMANCE != 1 &&
					/* interator > (par.HiperGraphParams.SCANPERFOMANCE - 0.2f) * 1600 + 0.2f * 150*/
					CHECK_PERFOMANCE( counter ) /*> par.HiperGraphParams.SCANPERFOMANCE / 80*/)
				{
					stopped = true;
					break;
				}
			}

			//   MonoBehaviour.print(dsc.TEXTUREobjects.Count + " " + dsc.OBJECTtexture.Count);
			if ( !stopped ) // MonoBehaviour.print("ASD");
			{
				SKANNING = false;
				ERROR = false;
				RepaintNow(); //WindowHyperController
			}

			else { }
		}

		bool CHECK_PERFOMANCE( double counter )
		{
			switch ( adapter.par_e.HYPERGRAPH_SCANPERFOMANCE )
			{
				case 2: return counter > 0.1f / 80;

				case 4: return counter > 0.2f / 80;

				case 6: return counter > 1f / 80;

				case 8: return counter > 8f / 80;

				case 10: return false;

				default: throw new Exception( "unknowing performance" );
			}
		}


		internal class ObjectDisplay
		{
			internal int gameObjectId;
			internal HyperGraphModInstance hyperGraph;

			internal ObjectDisplay( int id, HyperGraphModInstance hyperGraph )
			{
				gameObjectId = id;
				this.hyperGraph = hyperGraph;
			}

			internal Component[] GetComponents;

			bool AllowInitialize_InMainThread_cache;

			/*  internal bool WAS_INIT_CONTAINS(int id)
			  {
				  lock (wasInit)
				  {
					  return (wasInit.ContainsKey(id));
				  }
			  }*/
			//  Dictionary<int, int> wasInit = new Dictionary<int, int>();

			internal bool AllowInitialize_InMainThread()
			{
				lock ( this )
				{
					if ( AllowInitialize_InMainThread_cache ) return true;

					lock ( hyperGraph.LOCKER )
					{
						foreach ( var fieldsAccessor in fAccessor )
						{
							if ( !fieldsAccessor.Value.completed_thread ) return false;
						}

						AllowInitialize_InMainThread_cache = true;

						return true;
					}
				}
			}

			internal bool WasAccessorInitialize_InMainThread;

			internal void InitializeAccessor_InMainThread()
			{
				if ( !AllowInitialize_InMainThread_cache ) LogProxy.LogError( "InitializeAccessor_InMainThread" );

				SEL_INC++;

				lock ( this )
				{
					if ( WasAccessorInitialize_InMainThread ) return;

					WasAccessorInitialize_InMainThread = true;
				}

				/* lock (wasInit)
				 {
					 if (wasInit.ContainsKey(current.GetInstanceID())) return;
					 wasInit.Add(current.GetInstanceID(), 0);
				 }*/
				/* WasAccessorInitialize_InMainThread = true;
				wasInit
				 var height = SIZES.OBJECT + SIZES.padding;

				 var fdsIndex = 0;

				 foreach (var fieldsAccessor in fAccessor)
				 {
					 if (!fieldsAccessor.Key) continue;
					 var f = fieldsAccessor.Value;
					 if (!f.completed_thead) continue;
					 if (!f.Was_InitializeBroadcasting_InMainThread) f.InitializeBroadcasting_InMainThread(fieldsAccessor.Key);


				 }*/


				/* if (AllFields.Count != 0)
				 {
					 result.DRAW_B_POSES = new Vector2[fdsIndex];
					 result.height = height;
					 INPUT_COMPS.Add(current.GetInstanceID(), result);
				 }*/


				/*
										if ("MoodBoxes" == EditorUtility.InstanceIDToObject(gameObjectId).name)
											MonoBehaviour.print(EditorUtility.InstanceIDToObject(gameObjectId).name);*/

				// var currentComps = current.GetComponents<Component>();
				// var activeComps = new Dictionary<Component, int>();
				// var fieldsCount = 0;
				var height = hyperGraph.SIZE.OBJECT() + hyperGraph.SIZE.padding_y();
				/* var result = new ObjectDisplay() {
					 AllFields =
														  new Dictionary<Component, Dictionary<string, int>>()
				 };*/
				var fdsIndex = 0;

				foreach ( var c in GetComponents )
				{
					// for (int i = 0; i < currentComps.Length; i++)
					// {
					// var c = EditorUtility.InstanceIDToObject(fieldsAccessor.Key);
					if ( !c ) continue;

					// var f = fAccessor[c.GetInstanceID()].f;
					var id = c.GetInstanceID();
					var f = fAccessor[id].faList;

					if ( f.Length == 0 ) continue;

					// resilt.ComponentToBPosIndex.Add(currentComps[i], fieldsCount);
					var needAdd = false;
					var fds = new Dictionary<string, int>();
					/*  if (f.Length != 0 && c.gameObject.name == "Main Camera")
						  MonoBehaviour.print(f.Length);*/

					for ( int fIndex = 0; fIndex < f.Length; fIndex++ )
					{
						bool haveChange = false;

						f[ fIndex ].CheckID( selection_id, SEL_INC );

						if ( f[ fIndex ].GetAllValuesCache == null || !f[ fIndex ].GetAllValuesCache.ContainsKey( c.GetInstanceID() ) )
						{
							if ( f[ fIndex ].GetAllValuesCache == null ) f[ fIndex ].GetAllValuesCache = new Dictionary<int, Dictionary<string, object>>();

							f[ fIndex ].GetAllValuesCache.Add( c.GetInstanceID(), f[ fIndex ].GetAllValues( c, 0, hyperGraph.adapter.par_e.HYPERGRAPH_EVENTS_MODE | hyperGraph.adapter.par_e.HYPERGRAPH_SKIP_ARRAYS ) );
						}

						foreach ( var item in f[ fIndex ].GetAllValuesCache[ c.GetInstanceID() ] )
						{
							if ( item.Value == null ) continue;

							if ( item.Value is GameObject )
							{
								if ( (GameObject)item.Value == hyperGraph.CurrentSelection )
								{
									if ( !hyperGraph.findedList.ContainsKey( id ) )
										hyperGraph.findedList.Add( id,
											new FIELD_PARAMS( f[ fIndex ] ) );

									hyperGraph.findedList[ id ].targetGameObject.Add( hyperGraph.CurrentSelection );
									// activeComps.Add(currentComps[i], i);
									haveChange = true;
								}
							}

							else
							{
								var getComp = item.Value as UnityEngine.Object;

								if ( getComp && hyperGraph.compsSorted.ContainsKey( getComp.GetInstanceID() ) )
								{
									if ( !hyperGraph.findedList.ContainsKey( id ) )
										hyperGraph.findedList.Add( id,
											new FIELD_PARAMS( f[ fIndex ] ) );

									hyperGraph.findedList[ id ].targetComponent.Add( getComp );
									haveChange = true;
								}
							}
						}

						/*// OLD SINLE VALUE
						if ( f[fIndex].ObjectType == hyperGraph.GameObjectType ) {
						if ( (GameObject)f[fIndex].GetValue( c ) == hyperGraph.CurrentSelection ) {
						hyperGraph.findedList.Add( id ,
								   new FIELD_PARAMS( f[fIndex] , hyperGraph.CurrentSelection , null , hyperGraph.fdsHotControl++ ) );
						// activeComps.Add(currentComps[i], i);
						haveChange = true;
						}
						} else {
						var getComp = (UnityEngine.Object)f[fIndex].GetValue(c);
						if ( getComp && hyperGraph.compsSorted.ContainsKey( getComp.GetInstanceID() ) ) {


						hyperGraph.findedList.Add( id ,
								   new FIELD_PARAMS( f[fIndex] , null , getComp , hyperGraph.fdsHotControl++ ) );
						// activeComps.Add(currentComps[i], i);
						haveChange = true;
						}
						}
						*/ // OLD SINLE VALUE

						if ( haveChange && !fds.ContainsKey( f[ fIndex ].Name ) ) // fieldsCount++;
						{
							height += hyperGraph.SIZE.VAR();
							fds.Add( f[ fIndex ].Name, fdsIndex++ );
							// result.fields.Add(f[fIndex].Name, result.fields.Count);
							needAdd = true;
						}
					}

					if ( needAdd )
					{
						this.AllFields.Add( c.GetInstanceID(), fds );
						height += hyperGraph.SIZE.COMP();
					}
				}

				/*     resilt.comps = activeComps;
				 resilt.DRAW_A_POSES = new Vector2[activeComps.Count];
				 resilt.DRAW_B_POSES = new Vector2[fieldsCount];
				 findedComopnentsAttached.Add(currentList[currentIndex].GetInstanceID(), resilt);*/
				if ( this.AllFields.Count != 0 )
				{
					this.DRAW_B_POSES = new Vector2[ fdsIndex ];
					this.height = height;
					hyperGraph.INPUT_COMPS.Add( gameObjectId, this );
				}
			}


			// internal FieldsAccessor fAccessor;
			internal Dictionary<int, FieldsAccessor> fAccessor = null;
			internal Dictionary<int, Dictionary<string, int>> AllFields = new Dictionary<int, Dictionary<string, int>>();
			internal Dictionary<int, int> objecComps = new Dictionary<int, int>();
			internal bool DRAW;

			internal List<Vector2> DRAW_A_POSES = new List<Vector2>();
			internal Vector2[] DRAW_B_POSES;

			internal float __height;

			internal float height //get { return Mathf.RoundToInt( __height/* * hyperGraph.CURRENT_SCALE*/ ); }
			{
				get { return __height /* * hyperGraph.CURRENT_SCALE*/; }

				set { __height = value; }
			}

			// internal Dictionary<Component, int> ComponentToBPosIndex;
		}

		private struct FIELD_PARAMS
		{
			internal FIELD_PARAMS( Tools.FieldAdapter field )
			{
				this.field = field;
				this.targetGameObject = new List<UnityEngine.Object>();
				this.targetComponent = new List<UnityEngine.Object>();
				//this.POS_INDEX = POS_INDEX;
			}

			internal Tools.FieldAdapter field;
			internal List<UnityEngine.Object> targetGameObject;

			internal List<UnityEngine.Object> targetComponent;
			//internal UnityEngine.Object targetComponent;
			// internal int POS_INDEX;
		}

		//////////////////////////////////////////////////
		//////////////////////////////////////////////////  DRAW TARGET
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	}
}