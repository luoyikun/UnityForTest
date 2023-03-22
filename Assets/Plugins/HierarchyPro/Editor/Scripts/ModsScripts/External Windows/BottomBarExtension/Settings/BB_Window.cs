using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EMX.HierarchyPlugin.Editor.Mods;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class BB_Window : ScriptableObject
	{
	}


	[CustomEditor( typeof( BB_Window ) )]
	class SETGUI_BottomBar : MainRoot
	{

		//EMX_TODO Change settings for external and internal windows

		internal static string set_text =  USE_STR + "Bottom Bar - Integration Settings";
		internal static string set_key = "USE_BOTTOMBAR_MOD";
		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}
		public override void OnInspectorGUI()
		{
			_GUI( (IRepaint)this );
		}
		public static void _GUI( IRepaint w )
		{
			Draw.RESET( w );

			Draw.BACK_BUTTON( w );
			Draw.TOG_TIT( set_text, set_key ,WIKI: WIKI_5_BOTTOMBAR);
			Draw.Sp( 10 );

			using ( ENABLE( w ).USE( set_key ) )
			{

				QWE( w, p.par_e.HIER_WIN_SET );
			}



			Draw.Sp( 10 );
			//Draw.HRx2();
			//GUI.Label( Draw.R, "" + LEFT + " Area:" );
			using ( GRO( w ).UP( 0 ) )
			{

				// Draw.TOG_TIT( "" + LEFT + " Area:" );

				Draw.TOG_TIT( "Quick tips:" );
				Draw.HELP( w, "Use LMB on the bottom bar's header to show/hide.", drawTog: true );
				//Draw.HELP( w, "Use double-click on empty space of header to show/hide bottom bar.", drawTog: true );
				Draw.HELP( w, "Use CTRL+CHIFT+M to show/hide bottom bar.", drawTog: true );
				Draw.HELP( w, "Use CTRL+CHIFT+X to show/hide hyper graph.", drawTog: true );
				Draw.HELP( w, "Drag the blue line to change hyper graph height.", drawTog: true );
				Draw.HELP( w, "Drag objects to other windows or from other windows.", drawTog: true );
				//Draw.HELP( w, "Use right-click to remove object.", drawTog: true );

				Draw.HELP_TEXTURE_OLD( w, "BOTTOMHELP" );

				//  Draw.HELP_TEXTURE( w, "TAP_LAST" );
				//  Draw.HELP( w, "Use the right-click on the icon to open a special menu for quick access to mod functions.", drawTog: true );
				//  Draw.HELP_TEXTURE( w, "DRAG_LAST" );

			}
		}


		internal static void QWE( IRepaint w, EditorSettingsAdapter.WindowSettings KEY )
		{


			// using ( MainRoot.GRO( w ).UP( 0 ) )
			// {
			//     Draw.Sp( 10 );
			//
			//     // Draw.TOG_TIT( "Style" );
			//     Draw.TOG( "Override lines height", /*-*/"USE_LINE_HEIGHT", overrideObject: KEY, EnableRed: false );
			//
			//
			//
			// }


			var p = Mods.DrawButtonsOld.GET_DISPLAY_PARAMS(MemType.Last);

			// Draw.Sp( 10 );
			using ( GRO( w ).UP( 0 ) )
			{
				//Draw.TOG_TIT( "Common settings:", EnableRed: true );
				//Draw.COLOR( "Window background color", p._BgOverrideColor_KEY, overrideObject: p );
				//Draw.Sp( 4 );
				// Draw.TOG( "Draw tooltips for buttons", p._DrawTooltips_KEY, overrideObject: p );
				//Draw.TOG( "Integrate only to the main hierarchy window", /*-*/"BOTTOMBAR_BOTTOM_DRAWFOR_ONEHIERARHYWIN", overrideObject: KEY, EnableRed: false );


				Draw.TOG_TIT( "Header Options:", EnableRed: true );
				Draw.TOOLBAR( new[] { "None", "Category Name", "Selection Tree" }, "BOTTOMBAR_HEADER_OPTION", overrideObject: KEY );

				//Draw.TOG( "Draw parents of the selection on the header", /*-*/"BOTTOMBAR_SHOW_PARENT_TREE", overrideObject: KEY, EnableRed: false );
				if ( Root.p[ 0 ].WIN_SET.BOTTOMBAR_HEADER_OPTION == 1 )
				{
					Draw.HELP( w, "Draw current bookmark category name" );
					//Draw.HELP( w, "It will replace bookmarks category name" );
					using ( ENABLE( w ).USE( "BOTTOMBAR_SHOW_CAT_NAME", overrideObject: KEY ) )
					{
						Draw.FIELD( "Category name font size", /*-*/"BOTTOMBAR_HEADER_FONT_SIZE", 1, 40, overrideObject: KEY );
					}
				}
				if ( Root.p[ 0 ].WIN_SET.BOTTOMBAR_HEADER_OPTION == 2 )
				{
					Draw.HELP( w, "Draw parent for selected object" );
					using ( ENABLE( w ).USE( "BOTTOMBAR_SHOW_PARENT_TREE", overrideObject: KEY ) )
					{
						Draw.FIELD( "Parent names font size", /*-*/"BOTTOMBAR_HEADER_FONT_SIZE", 1, 40, overrideObject: KEY );
						Draw.TOG( "Draw selected object name", /*-*/"BOTTOMBAR_SHOW_PARENT_TREE_CURRENTOBJECT", overrideObject: KEY, EnableRed: false );
					}
				}


				Draw.HRx1();

				Draw.TOG( "Use double-click instead of single-click to expand bottom bar", /*-*/"BOTTOMBAR_USE_DOUBLE_CLICK", overrideObject: KEY, EnableRed: false );

				Draw.Sp( 8 );

				Draw.Sp( 10 );

			

				//Draw.FIELD( "Font size", "BOTTOMBAR_BOOKMARKS_LINEHEIGHT", 4, 30, overrideObject: KEY );
				//Draw.FIELD( "Font size", "BOTTOMBAR_LAST_LINEHEIGHT" , 4, 30, overrideObject: KEY );

			}
			Draw.Sp( 10 );

			using ( GRO( w ).UP( 0 ) )
			{

				Draw.TOG_TIT( "Bottom Bar Settings:", EnableRed: true );
				Draw.Sp( 4 );
				Draw.FIELD( "All objects icons size offset", /*-*/"BOTTOM_ICONS_SIZE_OFFSET", -20, 20, overrideObject: KEY );

				Draw.Sp( 8 );

				DRAW_ROWS( w, KEY );

				Draw.Sp( 8 );
				//Draw.TOG_TIT( "Common settings:", EnableRed: false );
			}


			// internal bool BOTTOMBAR_BOTTOM_DRAWFOR_ONEHIERARHYWIN { get { return GET( "BOTTOMBAR_BOTTOM_DRAWFOR_ONEHIERARHYWIN", true ); } set { var r = BOTTOMBAR_BOTTOM_DRAWFOR_ONEHIERARHYWIN; SET( "BOTTOMBAR_BOTTOM_DRAWFOR_ONEHIERARHYWIN", value ); } }
			//
			//
			//
			//
			// internal bool BOTTOMBAR_SHOW_PARENT_TREE { get { return GET( "BOTTOMBAR_SHOW_PARENT_TREE", false ); } set { var r = BOTTOMBAR_SHOW_PARENT_TREE; SET( "BOTTOMBAR_SHOW_PARENT_TREE", value ); } }
			// internal bool BOTTOMBAR_SHOW_PARENT_TREE_CURRENTOBJECT { get { return GET( "BOTTOMBAR_SHOW_PARENT_TREE_CURRENTOBJECT", false ); } set { var r = BOTTOMBAR_SHOW_PARENT_TREE_CURRENTOBJECT; SET( "BOTTOMBAR_SHOW_PARENT_TREE_CURRENTOBJECT", value ); } }


			Draw.Sp( 10 );
			using ( GRO( w ).UP( 0 ) )
			{
				Draw.TOG_TIT( "External Mods Buttons:" );


				var en = GUI.enabled;
				GUI.enabled &= KEY.BOTTOMBAR_DRAW_HOT_BUTTON || Root.p[ 0 ].par_e.DRAW_BOTTOM_HOTBUTTONS;
				{
				Draw.Sp( 4 );
					Draw.FIELD( "Space between buttons ", "BOTTOMBAR_ICONS_SPACE", 0, 60, "px", overrideObject: KEY );
					Draw.Sp( 10 );
				}
				GUI.enabled = en;
				//Draw.HRx05( Draw.R05 );

				//Draw.TOG_TIT( "QuickButs:", EnableRed: false );


				//Draw.TOG( "Draw quickbuts for external mods", /*-*/"BOTTOMBAR_DRAW_HOT_BUTTON", overrideObject: KEY, EnableRed: false );
				Draw.TOG_TIT( "Default QuickButs for External Mods", "BOTTOMBAR_DRAW_HOT_BUTTON", EnableRed: false , overrideObject: KEY);


				using ( ENABLE( w ).USE( "BOTTOMBAR_DRAW_HOT_BUTTON", 0, overrideObject: KEY ) )
				{

					Draw.TOG( "LMB - Quick show/hide module instead of opening the menu (RMB - menu)", /*-*/"BOTTOMBAR_QUICK_BUTTON_LEFT_CLICK_CLOSE", overrideObject: KEY );


					//Draw.FIELD( "Font size", p._fontSize_KEY, 4, 30, overrideObject: p );

					// Draw.TOG( "Draw tooltips for buttons", p._DrawTooltips_KEY, overrideObject: p );
					// Draw.Sp( 4 );
					//Draw.HRx1();
					//GUI.Label( Draw.R, "Icons for external shortbuts:" );
					//Draw.TOG_TIT( "Icons:", EnableRed: false );
					//using ( ENABLE( w ).USE( "BOTTOMBAR_DRAW_HOT_BUTTON", "DRAW_BOTTOM_HOTBUTTONS", CLASS_ENALBE.operation.OR ) )

				}
				Draw.Sp( 10 );


				//Draw.TOG_TIT( "HotButtons:", EnableRed: false );

				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( "Draw External Mods HotButtons", "DRAW_BOTTOM_HOTBUTTONS", EnableRed: false );
					//Draw.TOG( "Draw External Mods HotButtons", "DRAW_BOTTOM_HOTBUTTONS", rov: Draw.R );
					using ( ENABLE( w ).USE( "DRAW_BOTTOM_HOTBUTTONS" ) )
					{
						Draw.FIELD( "External Mods Buttons Size", "BOTTOM_HOTBUTTON_SEZE", 3, 60, "px" );
						//using ( GRO( w ).UP( 30 ) )
						if ( Root.p[ 0 ].par_e.DRAW_BOTTOM_HOTBUTTONS ) Root.p[ 0 ].par_e.DrawHotButtonsArray();
					}
				}



			}

			// internal bool BOTTOMBAR_SHOW_SCENES_ROWS { get { return GET( "BOTTOMBAR_SHOW_SCENES_ROWS", true ); } set { var r = BOTTOMBAR_SHOW_SCENES_ROWS; SET( "BOTTOMBAR_SHOW_SCENES_ROWS", value ); } }
			// internal bool BOTTOMBAR_SHOW_HIERARCHYSLOTS_ROWS { get { return GET( "BOTTOMBAR_SHOW_HIERARCHYSLOTS_ROWS", true ); } set { var r = BOTTOMBAR_SHOW_HIERARCHYSLOTS_ROWS; SET( "BOTTOMBAR_SHOW_HIERARCHYSLOTS_ROWS", value ); } }
			// internal bool BOTTOMBAR_SHOW_LAST_ROWS { get { return GET( "BOTTOMBAR_SHOW_LAST_ROWS", true ); } set { var r = BOTTOMBAR_SHOW_LAST_ROWS; SET( "BOTTOMBAR_SHOW_LAST_ROWS", value ); } }
			// internal bool BOTTOMBAR_SHOW_BOOKMARKS_ROWS { get { return GET( "BOTTOMBAR_SHOW_BOOKMARKS_ROWS", true ); } set { var r = BOTTOMBAR_SHOW_BOOKMARKS_ROWS; SET( "BOTTOMBAR_SHOW_BOOKMARKS_ROWS", value ); } }

			/*
            Draw.Sp( 10 );
            using ( GRO( w ).UP( 0 ) )
            {
                Draw.TOG_TIT( "Other Modules Interaction:" );
                Draw.TOG( "Draw highlighter colors", p._DrawHiglighter_KEY, overrideObject: p );
                using ( ENABLE( w ).USE( set_key ) ) Draw.FIELD( "Highlighter colors opacity", p._HiglighterOpacity_KEY, 0, 1, overrideObject: p );

                Draw.Sp( 4 );

            }
            */
		}


		//static int[] intMaxItemsPopUp;
		//static int[] rowsPopUp;
		static  float REF_ = 292 + 55;
		//static  int[] _OBJECT_X =      {    0,  21,   39,    62,     182, 222, 267,    292};
		static  int[] _OBJECT_SPACE = {     05, -1,   04,      0,    2,  7,  9,     0};
		static  int[] _OBJECT_WIDTH = {     16, 19,   19,    120,    42,  42,  20,     20};

		static  int[] OBJECT_X_CACHE;
		static int OBJECT_X( int d )
		{
			if ( OBJECT_X_CACHE == null ) OBJECT_X_CACHE = new int[ _OBJECT_SPACE.Length ];
			for ( int i = 1; i < OBJECT_X_CACHE.Length; i++ )
			{
				OBJECT_X_CACHE[ i ] = OBJECT_X_CACHE[ i - 1 ] + _OBJECT_SPACE[ i - 1 ] + _OBJECT_WIDTH[ i - 1 ];
			}
			return Mathf.RoundToInt( OBJECT_X_CACHE[ d ] / REF_ * SOURCE_RECT.width );
		}
		static  int[] OBJECT_WIDTH_CACHE;
		static int[] OBJECT_WIDTH {
			get {
				if ( OBJECT_WIDTH_CACHE == null ) OBJECT_WIDTH_CACHE = new int[ _OBJECT_SPACE.Length ];
				for ( int i = 0; i < OBJECT_WIDTH_CACHE.Length; i++ )
				{
					//OBJECT_WIDTH_CACHE[ i ] = OBJECT_X_CACHE[ i + 1 ] - OBJECT_X_CACHE[ i ] - _OBJECT_SPACE[ i ];
					OBJECT_WIDTH_CACHE[ i ] = Mathf.RoundToInt( _OBJECT_WIDTH[ i ] / REF_ * SOURCE_RECT.width );
				}
				return OBJECT_WIDTH_CACHE;
			}
		}
		//( "Rows number", p._Rows_KEY, 1, 10, overrideObject: p );
		//            Draw.FIELD( "Max buttons", p

		// static  string ROWS_CONTENT = "Category Name Sort Order Cells and Rows Amount Buttons Cells Amount Apply Highlighter Colors Background Color";
		static  GUIContent CatName = new GUIContent() {tooltip = "Category name"};
		static  GUIContent EnableDisable = new GUIContent() {tooltip = "Enable/disable"};
		static  GUIContent Sorting = new GUIContent() {tooltip = "Sort order"};
		static  GUIContent SortingUP = new GUIContent() {tooltip = "Sort order", text = "▲"};
		static GUIStyle _but; static GUIStyle but { get { return _but ?? (_but = new GUIStyle( GUI.skin.button ) { fontSize = Math.Max( 2, GUI.skin.button.fontSize - 6 ) }); } }

		static  GUIContent SortingDOWN = new GUIContent() {tooltip = "Sort order", text = "▼"};

		//    GUIContent CellsRowsCount = new GUIContent() { tooltip = "Cells and Rows Amount" };
		static  GUIContent ButtonsCount = new GUIContent() {tooltip = "Max buttons"};
		static  GUIContent RowsCount = new GUIContent() {tooltip = "Rows number"};
		static  GUIContent higlighterColor = new GUIContent() {tooltip = "Use highlighter colors"};
		static  GUIContent backgroundColor = new GUIContent() {tooltip = "Background color"};
		static  Rect SOURCE_RECT;

		const int PAD = 16;
		static int[] DRAW_INDEX;

		static void DRAW_ROWS( IRepaint w, EditorSettingsAdapter.WindowSettings KEY )
		{
			DRAW_INDEX = BottomBarExtension.SORT_DRAW_ROWS_AND_GETNEW_ARRAY();

			//
			// if ( intMaxItemsPopUp == null ) intMaxItemsPopUp = Enumerable.Repeat( 0, HierParams.MAX_SELECTION_ITEMS - 3 ).Select( ( e, i ) => i + 3 ).ToArray();
			//
			// if ( rowsPopUp == null ) rowsPopUp = Enumerable.Repeat( 0, 3 ).Select( ( e, i ) => i + 1 ).ToArray();


			//var r = SOURCE_RECT = EditorGUILayout.GetControlRect(GUILayout.Height(23));
			var r = SOURCE_RECT = Draw.R;
			r.y += (SOURCE_RECT.height - EditorGUIUtility.singleLineHeight) / 2;
			SOURCE_RECT.x += PAD;

			r.x = SOURCE_RECT.x + OBJECT_X( 1 );
			r.width = OBJECT_WIDTH[ 1 ] * 2;
			GUI.Label( r, "Sort" );
			GUI.Label( r, Sorting );
			r.x = SOURCE_RECT.x + OBJECT_X( 3 );
			r.width = OBJECT_WIDTH[ 3 ];
			GUI.Label( r, "Name" );
			GUI.Label( r, CatName );
			r.x = SOURCE_RECT.x + OBJECT_X( 4 );
			r.width = OBJECT_WIDTH[ 4 ];
			//GUI.Label( r, "Buttons" );
			GUI.Label( r, ButtonsCount );
			r.x = SOURCE_RECT.x + OBJECT_X( 5 );
			r.width = OBJECT_WIDTH[ 5 ];
			//GUI.Label( r, "Rows" );
			GUI.Label( r, RowsCount );
			r.x = SOURCE_RECT.x + OBJECT_X( 6 );
			r.width = OBJECT_WIDTH[ 6 ];
			var r23 = r;
			r23.x -= 20;
			r23.width += 40;
			GUI.Label( r23, "HiLi" );
			GUI.Label( r, higlighterColor );
			r.x = SOURCE_RECT.x + OBJECT_X( 7 );
			r.width = OBJECT_WIDTH[ 7 ];
			 r23 = r;
			r23.x -= 20;
			r23.width += 40;
			GUI.Label( r23, "ButCol" );
			GUI.Label( r, backgroundColor );

			var oldEn = GUI.enabled;
			var RowsClasses = DrawButtonsOld.ISET_ROWS_ARRAY;
			Draw.Sp( 10 );
			for ( int __index = 0; __index < DRAW_INDEX.Length; __index++ )
			{



				var i = DRAW_INDEX[__index];

				switch ( RowsClasses[ i ].type )
				{
					case MemType.Custom:
						GUI.enabled = oldEn & p.par_e.USE_BOOKMARKS_HIERARCHY_MOD;
						break;
					case MemType.Scenes:
						GUI.enabled = oldEn & p.par_e.USE_LAST_SCENES_MOD;
						break;
					case MemType.Last:
						GUI.enabled = oldEn & p.par_e.USE_LAST_SELECTION_MOD;
						break;
					case MemType.Hier:
						GUI.enabled = oldEn & p.par_e.USE_HIER_EXPANDED_MOD;
						break;
				}

				var not = !GUI.enabled;
				SOURCE_RECT = Draw.R;
				r2 = Draw.R;
				var ASD = SOURCE_RECT;
				ASD.height += r2.height;
				dr_l( i, __index );
				//if ( not ) Root.SetMouseTooltip( new GUIContent( RowsClasses[ i ].NAME + " modules disabled in the settings" ), ASD );

				GUI.Label( ASD, new GUIContent( "", RowsClasses[ i ].NAME + " modules disabled in the settings" ) );
			}

			GUI.enabled = oldEn;
		}
		static   Rect r2;
		static void dr_l( int i, int __index )
		{


			var RowsClasses = DrawButtonsOld.ISET_ROWS_ARRAY;

			//SOURCE_RECT = EditorGUILayout.GetControlRect( GUILayout.Height( 23 ) );


			var shr = 5;
			var asd = new Rect( SOURCE_RECT.x - shr, SOURCE_RECT.y - shr, SOURCE_RECT.width + shr + shr, r2.height + SOURCE_RECT.height + shr + shr );

			if ( Event.current.type == EventType.Repaint ) EditorStyles.textArea.Draw( asd, new GUIContent(), false, false, false, false );

			SOURCE_RECT.x += PAD;

			var r = SOURCE_RECT;
			var _bY = (SOURCE_RECT.height - EditorGUIUtility.singleLineHeight) / 2;
			r.y += _bY;

			r.height = EditorGUIUtility.singleLineHeight;

			r.x = SOURCE_RECT.x + OBJECT_X( 0 );
			r.width = OBJECT_WIDTH[ 0 ];
			RowsClasses[ i ].Enable = EditorGUI.Toggle( r, EnableDisable, RowsClasses[ i ].Enable );
			GUI.Label( r, EnableDisable );


			GUI.enabled &= RowsClasses[ i ].Enable;

			r.x = SOURCE_RECT.x + OBJECT_X( 1 );
			r.width = OBJECT_WIDTH[ 1 ];
			var e = GUI.enabled;
			GUI.enabled &= __index != 0;

			if ( GUI.Button( r, SortingUP, but ) )
			{
				var CI = DRAW_INDEX[__index];
				var NI = DRAW_INDEX[__index - 1];
				var OLD = RowsClasses[CI].IndexPos;
				RowsClasses[ CI ].IndexPos = RowsClasses[ NI ].IndexPos;
				RowsClasses[ NI ].IndexPos = OLD;
				//DRAW_STACK.ValueChanged();
			}

			r.x = SOURCE_RECT.x + OBJECT_X( 2 );
			r.width = OBJECT_WIDTH[ 2 ];
			GUI.enabled = e & __index < DRAW_INDEX.Length - 1;

			if ( GUI.Button( r, SortingDOWN, but ) )
			{
				var CI = DRAW_INDEX[__index];
				var NI = DRAW_INDEX[__index + 1];
				var OLD = RowsClasses[CI].IndexPos;
				RowsClasses[ CI ].IndexPos = RowsClasses[ NI ].IndexPos;
				RowsClasses[ NI ].IndexPos = OLD;
				//DRAW_STACK.ValueChanged();
			}

			GUI.enabled = e;

			r.x = SOURCE_RECT.x + OBJECT_X( 3 );
			r.width = OBJECT_WIDTH[ 3 ];
			GUI.Label( r, "- " + RowsClasses[ i ].NAME + ":" );




			///r.x = SOURCE_RECT.x + OBJECT_X( 4 );
			///r.width = OBJECT_WIDTH[ 4 ];
			/////  RowsClasses[i].MaxItems = EditorGUI.IntField(r, RowsClasses[i].MaxItems, null, intMaxItemsPopUp);
			///var a1 = EditorGUI.IntField(r, ButtonsCount, RowsClasses[i].MaxItems);
			///if ( a1 != RowsClasses[ i ].MaxItems )
			///{
			///	RowsClasses[ i ].MaxItems = a1;
			///	//DRAW_STACK.ValueChanged();
			///}
			///GUI.Label( r, ButtonsCount );

			//r.x = SOURCE_RECT.x + OBJECT_X( 5 );
			//r.width = OBJECT_WIDTH[ 5 ];
			//var a2 = EditorGUI.IntField(r, RowsCount, RowsClasses[i].Rows);
			//if ( a2 != RowsClasses[ i ].Rows )
			//{
			//	RowsClasses[ i ].Rows = a2;
			//	//DRAW_STACK.ValueChanged();
			//}
			//GUI.Label( r, RowsCount );




			//  if ( RowsClasses[ i ].DrawHiglighter )
			{
				r.x = SOURCE_RECT.x + OBJECT_X( 6 );
				r.width = OBJECT_WIDTH[ 6 ];
				var a3 = EditorGUI.Toggle(r, higlighterColor, RowsClasses[i].DrawHiglighter);

				if ( a3 != RowsClasses[ i ].DrawHiglighter )
				{
					RowsClasses[ i ].DrawHiglighter = a3;
					//DRAW_STACK.ValueChanged();
				}

				GUI.Label( r, higlighterColor );
				r.x += 5;
			}

			//if ( RowsClasses[ i ].AllowBgColor )
			{
				r.x = SOURCE_RECT.x + OBJECT_X( 7 );
				r.width = OBJECT_WIDTH[ 7 ];
				r.y = SOURCE_RECT.y;
				r.height = SOURCE_RECT.height;

				var newC2 = Draw.COLOR_TRUERECT( r, RowsClasses[ i ].BgOverrideColor ); //new Rect( r.x + r.width - 55, r.y, 55, 23 )

				// var newC2 = M_Colors_Window.PICKER(new Rect(r.x + r.width - 55, r.y, 55, 23), backgroundColor, RowsClasses[i].BgColorValue);

				if ( RowsClasses[ i ].BgOverrideColor != newC2 )
				{
					RowsClasses[ i ].BgOverrideColor = newC2;
					//DRAW_STACK.ValueChanged();
				}
			}


			var start = OBJECT_X( 2 );
			r2.width = r2.x + r2.width - start - 100;
			r2.x = start;
			r2.width /= 4;


			var p = RowsClasses[ i ];
			Draw.FIELD_R( r2, "Font size", p._fontSize_KEY, 4, 30, overrideObject: p, lablelOffset: OBJECT_WIDTH[ 5 ]  );
			r2.x += r2.width;
			Draw.FIELD_R( r2, "Height", p._RowHeight_KEY, 1, 90, overrideObject: p, lablelOffset: OBJECT_WIDTH[ 5 ] );
			r2.x += r2.width;
			Draw.FIELD_R( r2, "Rows", p._Rows_KEY, 1, 10, overrideObject: p, lablelOffset: OBJECT_WIDTH[ 5 ]  );

			r2.x += r2.width;
			Draw.FIELD_R( r2, "Buttons", p._MaxItems_KEY, 1, 30, overrideObject: p, lablelOffset: OBJECT_WIDTH[ 5 ]  );

			//Draw.FIELD( "Rows number", p._Rows_KEY, 1, 10, overrideObject: p );
			//Draw.FIELD( "Max buttons", p._MaxItems_KEY, 1, 30, overrideObject: p );

			Draw.Sp( 7 );
		}
	}
}
