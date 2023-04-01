using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace EMX.HierarchyPlugin.Editor.Mods
{




	internal partial class DrawButtonsOld
	{

		internal int DESCRIPTION_MULTY( int fontSize, int lh )
		{
			/*if (main) return adapter.FONT_8() + 2;
			else return (int)(adapter.FONT_8() * 1.5f);*/
			var res = (int)(fontSize * 1.5f);
			if ( res > lh / 2 ) res = lh / 2;
			return res;
		}
		//internal static int idOffset = 500000;

		GUIContent content = new GUIContent();
		Texture2D OBJECTCONTENTCOUNT;
		Texture2D BOTTOM_SCENE_DOWN;
		Texture2D BOTTOM_SCENE_ACTIVE;
		Texture2D SCENE;
		Texture2D NEW_BOTTOM_HIERARCHY_ICON;
		Texture2D FAV;
		Texture2D BOTTOM_INFO_DISABLE;
		Texture2D BOTTOM_INFO_DISABLE_PERSONAL;
		Texture2D BOTTOM_INFO;
		Texture2D BOTTOM_INFO_PERSONAL;
		Texture2D HIPERUI_BUTTONGLOW;

		static bool sceneIconPosRight = false;
		bool _INIT_ICONS;
		void INIT_ICONS()
		{
			if ( _INIT_ICONS ) return;
			_INIT_ICONS = true;
			OBJECTCONTENTCOUNT = Root.icons.GetOldExternalMod( "OBJECTCONTENTCOUNT" );
			BOTTOM_SCENE_DOWN = Root.icons.GetOldExternalMod( "BOTTOM_SCENE_DOWN" );
			BOTTOM_SCENE_ACTIVE = Root.icons.GetOldExternalMod( "BOTTOM_SCENE_ACTIVE" );
			SCENE = Root.icons.GetOldExternalMod( "SCENE" );
			NEW_BOTTOM_HIERARCHY_ICON = Root.icons.GetOldExternalMod( "NEW_BOTTOM_HIERARCHY_ICON" );
			FAV = Root.icons.GetOldExternalMod( "FAV" );
			BOTTOM_INFO_DISABLE = Root.icons.GetOldExternalMod( "BOTTOM_INFO_DISABLE" );
			BOTTOM_INFO_DISABLE_PERSONAL = Root.icons.GetOldExternalMod( "BOTTOM_INFO_DISABLE_PERSONAL" );
			BOTTOM_INFO = Root.icons.GetOldExternalMod( "BOTTOM_INFO" );
			BOTTOM_INFO_PERSONAL = Root.icons.GetOldExternalMod( "BOTTOM_INFO_PERSONAL" );
			HIPERUI_BUTTONGLOW = Root.icons.GetOldExternalMod( "HIPERUI_BUTTONGLOW" );
		}
		/*	void SetDragData(UnityEngine.Object[] data, MemType? type)
			{
				if (data != null)
				{
					adapter.ha.InternalClearDrag();
					DragAndDrop.objectReferences = data;
					DragAndDrop.SetGenericData(adapter.pluginname, type);
				}
			}*/
		private Color tst = new Color32(240, 240, 255, 255);
		//private Color ts2t = new Color32(248, 240, 225, 255);
		GUIStyle refStyle;


		GUIStyle _miniButtonMid; GUIStyle miniButtonMid {
			get {
				return _miniButtonMid ?? (
_miniButtonMid = EditorGUIUtility.isProSkin ?
				new GUIStyle( EditorStyles.miniButtonMid ) { fixedHeight = 0, margin = new RectOffset(), padding = new RectOffset(), fixedWidth = 0 } :
				 new GUIStyle( EditorStyles.objectField ) { fixedHeight = 0, margin = new RectOffset(), padding = new RectOffset(), fixedWidth = 0 }
				 );
			}
		}
		GUIStyle _toolbarButton; GUIStyle toolbarButton { get { return _toolbarButton ?? (_toolbarButton = new GUIStyle( EditorStyles.toolbarButton ) { fixedHeight = 0 }); } }
		GUIStyle _ScenesbarButton; GUIStyle ScenesbarButton {
			get {
				return _ScenesbarButton ?? (
_ScenesbarButton = new GUIStyle( EditorStyles.textArea ) {
	fixedHeight = 0
,
	hover = EditorStyles.textArea.normal,
	onNormal = EditorStyles.textArea.normal,
	onActive = EditorStyles.textArea.normal,
	onHover = EditorStyles.textArea.normal,
	focused = EditorStyles.textArea.normal,
	onFocused = EditorStyles.textArea.normal,
});
			}
		}
		//GUIStyle _miniButtonMid; GUIStyle miniButtonMid { get { return _miniButtonMid ?? (_miniButtonMid = new GUIStyle( EditorStyles.miniButtonMid ) { fixedHeight = 0 }); } }

		static bool wasLog;
		Color refColor;
		internal static Dictionary<int, bool> was_draw_dic = new Dictionary<int, bool>();
		internal static Dictionary<int, bool> was_draw_dic_B = new Dictionary<int, bool>();
		internal bool DrawButtons( Rect line, MemType type, Color styleColor, ExternalDrawContainer controller, Scene scene )
		{

			line.x = (int)line.x;
			line.y = (int)line.y;
			line.width = (int)line.width;
			line.height = (int)line.height;

			//if ( !adapter.GET_SLOW_oldProSkin ) 
			if (!controller.MAIN) EditorGUI.DrawRect( line, Colors.EditorBGColor );

			if ( DRAW_BG_Y && type == MemType.Custom)
			{
				EditorGUI.DrawRect( DRAW_BG_LINE, DRAW_BG_COLOR );
			}

			var OOFF2  = adapter.WIN_SET.BOTTOM_ICONS_SIZE_OFFSET;

			line.width -= 2;
			line.height -= 2;
			line.x += 2;
			INIT_ICONS();
			Rect ModuleRect = line;
			int __LH = (int)line.height;
			int idOffset = -1;
			switch ( type )
			{
				case MemType.Custom:
					refStyle = miniButtonMid;
					refColor = tst;
					idOffset = 10000000;
					break;
				case MemType.Scenes:
					refStyle = ScenesbarButton;
					refColor = Color.white;
					idOffset = 20000000;
					break;
				case MemType.Last:
					refStyle = adapter.box;
					refColor = Color.white;
					idOffset = 30000000;
					break;
				case MemType.Hier:
					refStyle = toolbarButton;
					refColor = Color.white;
					idOffset = 40000000;
					break;
			}
			// refStyle = EditorStyles.toolbarButton;
			var CUSTOM_DRAWER = type == MemType.Custom;

			var BOOKMARKS_OB_SHOWDESCRIPTS =  adapter.par_e.BOOKMARKS_OB_SHOWDESCRIPTS && adapter.par_e.USE_RIGHT_ALL_MODS;

			var SHOW_DES = type == MemType.Custom && BOOKMARKS_OB_SHOWDESCRIPTS ;
			/*controller.type = type;*/
			var memoryRoot = GET_OBJECTS_LIST(type, controller, scene);
			var ROW_PAR = GET_DISPLAY_PARAMS(type, controller);


			if ( type == MemType.Last )
				for ( int i = 0; i < memoryRoot.Count; i++ )
				{
					if ( !was_draw_dic.ContainsKey( memoryRoot[ i ].unique_id ) ) was_draw_dic.Add( memoryRoot[ i ].unique_id, false );
					was_draw_dic[ memoryRoot[ i ].unique_id ] = false;
				}
			var itemscount = 0;
			if ( type == MemType.Last )
				for ( int i = 0; i < memoryRoot.Count; i++ )
					if ( !memoryRoot[ i ].IsValid() ) continue;
					else if ( was_draw_dic.ContainsKey( memoryRoot[ i ].unique_id ) && was_draw_dic[ memoryRoot[ i ].unique_id ] ) continue;
					else
					{
						if ( was_draw_dic.ContainsKey( memoryRoot[ i ].unique_id ) ) was_draw_dic[ memoryRoot[ i ].unique_id ] = true;
						itemscount++;
					}
			else
				for ( int i = 0; i < memoryRoot.Count; i++ )
					if ( !memoryRoot[ i ].IsValid() ) continue;
					else itemscount++;
			if ( CUSTOM_DRAWER ) itemscount++;
			if ( itemscount <= 0 ) return false;


			var ROWS_COUNT = ROW_PAR.Rows;

			if ( itemscount > ROW_PAR.MaxItems ) itemscount = ROW_PAR.MaxItems;

			var COUNT_PER_ROW = 0;

			if ( type == MemType.Custom ) COUNT_PER_ROW = Mathf.CeilToInt( ROW_PAR.MaxItems / (float)ROWS_COUNT );
			else COUNT_PER_ROW = Mathf.CeilToInt( itemscount / (float)ROWS_COUNT );

			var WIDTH = line.width;
			//var WIDTH = controller.WIDTH;
			var __cell = line;



			line.width -= 3;
			__cell.y += 2;
			var maxLines = Mathf.CeilToInt(itemscount / (float)COUNT_PER_ROW);
			//ROWS_COUNT = Mathf.Min(ROWS_COUNT, maxLines);
			//	__cell.height = __LH * ((float)ROWS_COUNT / maxLines);
			__LH = (int)(__LH / ((float)maxLines));
			__cell.height = __LH;
			//if (__cell.height > line.height) throw new Exception(__cell.height + " " + line.height + " " + ROWS_COUNT + " " + maxLines);
			//var backuopcall = __cell;
			if ( type == MemType.Custom && SHOW_DES )
			{
				__LH = (int)(__LH - DESCRIPTION_MULTY( ROW_PAR.fontSize, __LH ));
				__cell.height -= DESCRIPTION_MULTY( ROW_PAR.fontSize, __LH );
			}

			__cell.height -= 2;


			__cell.x = (int)__cell.x;
			__cell.y = (int)__cell.y;
			__cell.width = (int)__cell.width;
			__cell.height = (int)__cell.height;



			//backuopcall.height -= 2;
			bool? wasSelect = null;
			var defaultCell = __cell;
			var wasDraw = false;
			var interator = 0;

			if ( type == MemType.Last )
				for ( int i = 0; i < memoryRoot.Count; i++ )
				{
					if ( !was_draw_dic.ContainsKey( memoryRoot[ i ].unique_id ) ) was_draw_dic.Add( memoryRoot[ i ].unique_id, false );
					was_draw_dic[ memoryRoot[ i ].unique_id ] = false;
				}

			for ( int __i = 0; CUSTOM_DRAWER || __i < memoryRoot.Count && interator < itemscount; __i++ )
			{
				var i = __i;
				bool DisableCursor = false;
				if ( !CUSTOM_DRAWER && !memoryRoot[ i ].IsValid() ) continue;

				if ( (type == MemType.Last || type == MemType.Custom) && i < memoryRoot.Count && memoryRoot[ i ].type != type )
				{
					if ( !wasLog )
					{
						Debug.LogWarning( "Unknown type detected: " + memoryRoot[ i ].type.ToString() + " " + type.ToString() );
						wasLog = true;
					}
					memoryRoot[ i ].type = type;
				}

				if ( type == MemType.Last )
					if ( was_draw_dic.ContainsKey( memoryRoot[ __i ].unique_id ) && was_draw_dic[ memoryRoot[ __i ].unique_id ] ) continue;
				if ( type == MemType.Last )
					if ( was_draw_dic.ContainsKey( memoryRoot[ __i ].unique_id ) ) was_draw_dic[ memoryRoot[ __i ].unique_id ] = true;

				if ( !CUSTOM_DRAWER ) memoryRoot[ i ].Arrayindex = __i;
				//var h = type == MemType.Last || type == MemType.Custom ? INT32__ACTIVE_TOHIERARCHYOBJECT(memoryRoot[i].InstanceID) : null;
				//if ((type == MemType.Last || type == MemType.Custom) && !memoryRoot[i].IsValid()) continue;
				var lineIndex = !CUSTOM_DRAWER ? (interator / COUNT_PER_ROW) : ((interator + 1) / COUNT_PER_ROW);
				//if (CUSTOM_DRAWER) lineIndex++;
				var cell = GET_CELL_RECT(defaultCell, line, type, interator, itemscount, COUNT_PER_ROW, ROW_PAR.SortButtonsOrder);
				++interator;

				var contains = cell.Contains(Event.current.mousePosition);
				if ( !CUSTOM_DRAWER && (memoryRoot[ i ].IsSelectablePlus() || memoryRoot[ i ].IsSelectableMinus()) )
				{
					var STATE = memoryRoot[i].GET_SELECTION_STATE();
					if ( (/*type == MemType.Last ||*/ type == MemType.Custom) && adapter.par_e.BOOKMARKS_OB_ALT_TO_INSTANTIATE && Event.current.shift ) STATE = 10;
					if ( STATE != 0 && type != MemType.Scenes )
					{
						if ( STATE == 10 ) EditorGUIUtility.AddCursorRect( cell, MouseCursor.FPS );
						else EditorGUIUtility.AddCursorRect( cell, STATE == 1 ? MouseCursor.ArrowPlus : STATE == 2 ? MouseCursor.ArrowMinus : MouseCursor.ScaleArrow );
						DisableCursor = true;
					}

					if ( type == MemType.Scenes )
					{
						if ( Event.current.shift /*|| Event.current.alt*/) EditorGUIUtility.AddCursorRect( cell, MouseCursor.ArrowPlus );
						else if ( Event.current.control ) EditorGUIUtility.AddCursorRect( cell, MouseCursor.Zoom );

						DisableCursor = true;
					}
				}
				//if ( !DisableCursor && type == MemType.Scenes )
				//{
				//	var LH = Math.Min(__LH, (int)cell.width / 3);
				//	var r2= cell;
				//	if ( sceneIconPosRight )
				//	{
				//	}
				//	else
				//	{
				//		r2.width = LH;
				//	}
				//	EditorGUIUtility.AddCursorRect( r2, MouseCursor.Link );
				//}

				wasDraw = true;

				if ( CUSTOM_DRAWER )
				{
					var buttonRect = cell;
					//var buttonRect = backuopcall;
					if ( SHOW_DES ) buttonRect.height += DESCRIPTION_MULTY( ROW_PAR.fontSize, __LH );//- 3

					buttonRect.y = (int)(buttonRect.y);
					buttonRect.height = (int)(buttonRect.height);

					BookObject.BookmarksforGameObjectsModInstance.DRAW_CATEGORY( idOffset, buttonRect, controller, scene, BookObject.BookmarksforGameObjectsModWindow.GetStaticInstance, itemscount == 1 );
					CUSTOM_DRAWER = false;
					__i--;
					continue;
				}


				if ( SHOW_DES )
				{
					//var LH = Math.Min(__LH, (int)cell.width / 3);
					//lastDESrect.x = cell.x + LH / 2;
					//lastDESrect.width = cell.width - LH / 2;
					//lastDESrect.y = cell.y + cell.height;
					//lastDESrect.height = DESCRIPTION_MULTY(ROW_PAR.fontSize, __LH) - 3;

					lastDESrect = cell;
					lastDESrect.y = cell.y + cell.height;
					lastDESrect.height = DESCRIPTION_MULTY( ROW_PAR.fontSize, __LH );
					//r_des.height -= r_des.height;


					//EditorGUIUtility.AddCursorRect( lastDESrect, MouseCursor.ArrowPlus );
				}
				if ( Event.current.type == EventType.MouseDown && SHOW_DES && lastDESrect.Contains( Event.current.mousePosition ) )
				{
					if ( Event.current.button == 0 ) // Debug.Log("ASD");
					{
						controller.selection_button = idOffset + i + 100;
						controller.selection_window = controller.tempRoot;
						controller.lastRect = lastDESrect;
						var captureCell = lastDESrect;
						var captureI = i;
						//  var arrayIndex = memoryRoot[i].Arrayindex;
						var pos = new MousePos(Event.current.mousePosition, MousePos.Type.Input_190_68, false, adapter);

						// var  pos = InputData.WidnwoRect( false, Event.current.mousePosition, 190, 68, adapter  );
						controller.selection_action = ( mouseUp, deltaTIme ) => {
							if ( mouseUp && captureCell.Contains( Event.current.mousePosition ) )
							{
								if ( !memoryRoot[ captureI ].IsValid() ) return false;


								var dMod = (Mod_Descript)adapter.modsController.rightModsManager.rightMods.First(m => m is Mod_Descript);
								dMod.CREATE_NEW_ESCRIPTION( adapter, pos, Cache.GetHierarchyObjectByInstanceID( memoryRoot[ captureI ].gos_get()[ 0 ] ), true );
								/*	var d = adapter.MOI.des(INT32_SCENE(memoryRoot[captureI].InstanceID));
									if (d == null) return false;
									adapter.DescriptionModule.CREATE_NEW_ESCRIPTION(adapter, pos, INT32__ACTIVE_TOHIERARCHYOBJECT(memoryRoot[captureI].InstanceID), scene, true);*/
							}

							return Event.current.delta.x == 0 && Event.current.delta.x == 0;
						}; // ACTION
					} //if button
				}
				else if ( contains && Event.current.type == EventType.MouseDown )
				{
					var LH = Math.Min(__LH, (int)cell.width / 3);

					if ( Event.current.button == 0 && type == MemType.Scenes &&
							(sceneIconPosRight && Event.current.mousePosition.x > cell.x + cell.width - LH ||
							!sceneIconPosRight && Event.current.mousePosition.x < cell.x + LH
							) )
					{
						controller.selection_button = 1000000 + i;
						var r = controller.selection_button;
						controller.selection_window = controller.tempRoot;
						var captureCell = cell;
						//  var captureI = i;
						var rar = memoryRoot;
						var Arrayindex = memoryRoot[i].Arrayindex;
						controller.selection_action = ( mouseUp, deltaTIme ) => {
							var cc = captureCell.Contains(Event.current.mousePosition) &&
							(sceneIconPosRight &&Event.current.mousePosition.x > captureCell.x + cell.width - captureCell.height||
							!sceneIconPosRight && Event.current.mousePosition.x < captureCell.x + captureCell.height
							)
							;

							if ( cc ) controller.selection_button = r;
							else controller.selection_button = -1;
							if ( mouseUp && cc )
							{


								rar[ Arrayindex ].pin = !rar[ Arrayindex ].pin;
								SET_OBJECTS_LIST( rar, MemType.Scenes, controller, scene );
								//HierarchyCommonData.Instance().ScenesTabs[Arrayindex].pin = !HierarchyCommonData.Instance().ScenesTabs[Arrayindex].pin;

								/*	GET_OBJECTS_LIST
									if (arrayIndex < Hierarchy_GUI.GetLastScenes(adapter).Count && Hierarchy_GUI.GetLastScenes(adapter)[arrayIndex] != null)
									{
										Undo.RecordObject(Hierarchy_GUI.Instance(adapter), Hierarchy_GUI.GetLastScenes(adapter)[arrayIndex].pin ? "UnPin Scene" : "Pin Scene");
										Hierarchy_GUI.GetLastScenes(adapter)[arrayIndex].pin = !Hierarchy_GUI.GetLastScenes(adapter)[arrayIndex].pin;
										Hierarchy_GUI.SetDirtyObject(adapter);
										RefreshMemCache(scene);
										ClearAction();
									}*/
							}

							return Event.current.delta.x == 0 && Event.current.delta.x == 0;
						}; // ACTION
					}
					else if ( Event.current.button == 0 )
					{
						controller.selection_button = idOffset + i;
						controller.selection_window = controller.tempRoot;
						var captureCell = cell;
						var captureI = i;
						// var pos = Event.current.mousePosition;
						//Debug.Log(cell + " " + i);

						var startRect = cell;
						var startMouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
						var startX = startRect.x;
						var startY = startRect.y;
						controller.wasDrag = false;

						var arrayIndex = memoryRoot[i].Arrayindex;
						var backupArraIndex = arrayIndex;
						var captureType = type;

						var captureLine = lineIndex;
						var LineMin = GUIUtility.GUIToScreenPoint(new Vector2(cell.x, cell.y)).y;
						var LineMax = GUIUtility.GUIToScreenPoint(new Vector2(cell.x + cell.width, cell.y + cell.height)).y;

						if ( SHOW_DES ) LineMax += DESCRIPTION_MULTY( ROW_PAR.fontSize, __LH );

						var LineDif = LineMax - LineMin + 2;
						var swap = ROW_PAR.SortButtonsOrder > 1;

						if ( swap ) LineDif = -LineDif;

						var startLineMin = LineMin;

						var r_info = GET_INFO_RECT(type, cell);
						var infoContains = type == MemType.Custom && !SHOW_DES && BOOKMARKS_OB_SHOWDESCRIPTS && r_info.Contains(Event.current.mousePosition);
						controller.selection_info = infoContains;
						var rar = memoryRoot;
						var backRar = memoryRoot.ToArray();

						controller.selection_action = ( mouseUp, deltaTIme ) => /////////////////////////////
						{
							var next = arrayIndex;
							if ( controller.wasDrag )
							{
								//var interator_2 = 0;
								var interator_2 = (type== MemType.Custom) ? 1 : 0;
								// var LINE = -1;
								//var __rect = defaultCell;
								//Debug.Log(interator_2 + " " + CUSTOM_DRAWER + " " + type);
								if ( type == MemType.Last )
									for ( int asd = 0; asd < memoryRoot.Count; asd++ )
									{
										if ( !was_draw_dic_B.ContainsKey( memoryRoot[ asd ].unique_id ) ) was_draw_dic_B.Add( memoryRoot[ asd ].unique_id, false );
										was_draw_dic_B[ memoryRoot[ asd ].unique_id ] = false;
									}

								for ( int index = 0; index < memoryRoot.Count && interator_2 < itemscount; index++ ) // Adapter.GET_SKIN().button.Draw(cell, new GUIContent("Object" + (i + 1)), false, false, false, false);
								{
									if ( !memoryRoot[ index ].IsValid() ) continue;
									if ( type == MemType.Last )
										if ( was_draw_dic_B.ContainsKey( memoryRoot[ index ].unique_id ) && was_draw_dic_B[ memoryRoot[ index ].unique_id ] ) continue;
									if ( type == MemType.Last )
										if ( was_draw_dic_B.ContainsKey( memoryRoot[ index ].unique_id ) ) was_draw_dic_B[ memoryRoot[ index ].unique_id ] = true;


									var LINE = interator_2 / COUNT_PER_ROW;
									var asd = interator_2;
									//if (CUSTOM_DRAWER) asd--;
									var rect = GET_CELL_RECT(defaultCell, line, type, asd, itemscount, COUNT_PER_ROW, ROW_PAR.SortButtonsOrder);
									++interator_2;

									var p1 = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
									var p2 = GUIUtility.GUIToScreenPoint(new Vector2(rect.x + rect.width, rect.y + rect.height));
									var worldRect = new Rect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y);

									//MonoBehaviour.print(GUIUtility.GUIToScreenPoint(Event.current.mousePosition) + " " + worldRect.Contains(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)) + " " + worldRect + " " + LINE + "  " + captureLine + "  " + interator_2);
									if ( worldRect.Contains( GUIUtility.GUIToScreenPoint( Event.current.mousePosition ) ) && memoryRoot[ index ].Arrayindex != next ) //Debug.Log(memoryRoot[index].Arrayindex + " " + next);
									{
										next = memoryRoot[ index ].Arrayindex;

										if ( LINE > captureLine )
										{
											LineMin += LineDif;
											LineMax += LineDif;
										}

										if ( LINE < captureLine )
										{
											LineMin -= LineDif;
											LineMax -= LineDif;
										}

										captureLine = LINE;
										startRect.width = rect.width;
										break;
									}
								}
							}

							DragRect = startRect;
							DragRect.x = -(int)startMouse.x + (int)GUIUtility.GUIToScreenPoint( Event.current.mousePosition ).x + startX;
							DragRect.y = startRect.y + LineMin - startLineMin;

							if ( controller.wasDrag && next != arrayIndex ) // var target = memoryRoot[next];
							{
								/*;*/

								/*var b = rar[arrayIndex];
								rar[arrayIndex] = rar[next];
								rar[next] = b;*/

								var b = rar[arrayIndex];
								rar.RemoveAt( arrayIndex );
								if ( next >= rar.Count ) rar.Add( b );
								else rar.Insert( next, b );
								for ( int q = 0; q < rar.Count; q++ )
								{
									rar[ q ].Arrayindex = q;
								}

								/*switch (captureType)
								{
									case MemType.Last:
										{

											
											
											var l1 = adapter.MOI.des(scene).GetHash3();
											var b = l1[arrayIndex];
											l1.RemoveAt(arrayIndex);
											if (next >= l1.Count) l1.Add(b);
											else l1.Insert(next, b);
										}
										break;

									case MemType.Custom:
										{
											List<Int32List> l1 = controller.GetCategoryIndex(scene) == 0 ? adapter.MOI.des(scene).GetHash4() : adapter.MOI.des(scene).GetBookMarks()[controller.GetCategoryIndex(scene)].array;
											HierarchyTempSceneData.InstanceFast().adapter.CreateUndoActiveDescription("Move Favorite", scene);
											var b = l1[arrayIndex];
											l1.RemoveAt(arrayIndex);
											if (next >= l1.Count) l1.Add(b);
											else l1.Insert(next, b);
											adapter.SetDirtyActiveDescription(scene);
										}
										break;

									case MemType.Hier:
										{
											adapter.CreateUndoActiveDescription("Move Hierarchy SLot", scene);
											var l1 = adapter.MOI.des(scene).HierarchyCache();
											var b = l1[arrayIndex];
											l1.RemoveAt(arrayIndex);
											if (next >= l1.Count) l1.Add(b);
											else l1.Insert(next, b);
											adapter.SetDirtyActiveDescription(scene);
										}
										break;

									case MemType.Scenes:
										{
											adapter.CreateUndoActiveDescription("Move Scene", scene);
											var l1 = Hierarchy_GUI.GetLastScenes(adapter);
											var b = l1[arrayIndex];
											l1.RemoveAt(arrayIndex);
											if (next >= l1.Count) l1.Add(b);
											else l1.Insert(next, b);
											adapter.SetDirtyActiveDescription(scene);
										}
										break;
								}*/
								if ( next > arrayIndex ) startRect.x -= startRect.width + SPACE;
								else startRect.x += startRect.width + SPACE;
								arrayIndex = next;
								controller.RepaintNow();
							}


							if ( Math.Abs( startMouse.x - (int)GUIUtility.GUIToScreenPoint( Event.current.mousePosition ).x ) > 5
								|| Math.Abs( startMouse.y - (int)GUIUtility.GUIToScreenPoint( Event.current.mousePosition ).y ) > 5 )
							{
								if ( !infoContains ) controller.wasDrag = true;
							}
							if ( controller.wasDrag && mouseUp )
							{
								for ( int _bri = 0; _bri < backRar.Length; _bri++ )
								{
									if ( backRar[ _bri ] != rar[ _bri ] )
									{
										SET_OBJECTS_LIST( rar, captureType, controller, scene );
										break;
									}
								}
							}

							if ( !controller.wasDrag && mouseUp && captureCell.Contains( Event.current.mousePosition ) )
							{
								if ( infoContains )
								{
									if ( !memoryRoot[ captureI ].IsValid() ) return false;
									/*	var d = adapter.MOI.des(INT32_SCENE(memoryRoot[captureI].InstanceID));
										if (d == null) return false;
										adapter.DescriptionModule.CREATE_NEW_ESCRIPTION(adapter, pos, INT32__ACTIVE_TOHIERARCHYOBJECT(memoryRoot[captureI].InstanceID), scene, true);*/

									var pos = new MousePos(Event.current.mousePosition, MousePos.Type.Input_190_68, false, adapter);
									var dMod = (Mod_Descript)adapter.modsController.rightModsManager.rightMods.First(m => m is Mod_Descript);
									dMod.CREATE_NEW_ESCRIPTION( adapter, pos, Cache.GetHierarchyObjectByInstanceID( memoryRoot[ captureI ].gos_get()[ 0 ] ), true );
								}

								else
								{
									if ( !memoryRoot[ captureI ].IsValid() ) return false;
									if ( !memoryRoot[ captureI ].OnClick( false, scene.GetHashCode(), controller ) )
									{
										rar.RemoveAt( arrayIndex );
										SET_OBJECTS_LIST( rar, captureType, controller, scene );
										//Tools.EventUse();
										controller.ClearAction();
										controller.RepaintNow();
										//RemoveAndRefresh(type, arrayIndex, controller.GetCategoryIndex(scene), scene);
									}
									//else if (type == MemType.Custom || type == MemType.Last) ROW_PAR.LastIndex = -1;
								}
							}

							else if ( Event.current.keyCode == KeyCode.Escape )
							{
								adapter.SKIP_PREFAB_ESCAPE = true;

								var b = rar[arrayIndex];
								rar.RemoveAt( arrayIndex );
								if ( backupArraIndex >= rar.Count ) rar.Add( b );
								else rar.Insert( backupArraIndex, b );
								SET_OBJECTS_LIST( rar, captureType, controller, scene, skipUndo: true );
								/*switch (captureType)
								{
									case MemType.Last:
										{
											var targetList = adapter.MOI.des(scene).GetHash3();
											Utilities.MoveFromTo(ref targetList, arrayIndex, backupArraIndex);
										}

										break;

									case MemType.Custom:
										{
											var targetList = controller.GetCategoryIndex(scene) == 0 ? adapter.MOI.des(scene).GetHash4() : adapter.MOI.des(scene).GetBookMarks()[controller.GetCategoryIndex(scene)].array;
											Utilities.MoveFromTo(ref targetList, arrayIndex, backupArraIndex);
										}
										break;

									case MemType.Hier:
										{
											var targetList = adapter.MOI.des(scene).HierarchyCache();
											Utilities.MoveFromTo(ref targetList, arrayIndex, backupArraIndex);
										}
										break;

									case MemType.Scenes:
										{
											var targetList = Hierarchy_GUI.GetLastScenes(adapter);
											Utilities.MoveFromTo(ref targetList, arrayIndex, backupArraIndex);
											RefreshMemCache(scene);
										}
										break;
								}*/

								//	Tools.EventUse();
								controller.ClearAction();
								controller.RepaintNow();
							}

							else if ( (type == MemType.Last || type == MemType.Custom || type == MemType.Scenes) )
							{
								var m = Event.current.mousePosition + Event.current.delta;
								var drag = !ModuleRect.Contains(m) /*|| type == MemType.Last && controller.CustomLineRect.Contains(m))*/;
								drag |= m.x < 3;
								drag |= m.x > WIDTH - 9;

								if ( (!infoContains) && Event.current.type == EventType.MouseDrag && !Event.current.control && !Event.current.shift && !Event.current.alt && drag )
								{

									var b = rar[arrayIndex];
									rar.RemoveAt( arrayIndex );
									if ( backupArraIndex >= rar.Count ) rar.Add( b );
									else rar.Insert( backupArraIndex, b );
									SET_OBJECTS_LIST( rar, captureType, controller, scene );
									/*switch (captureType)
									{
										case MemType.Last:
											{
												var targetList = adapter.MOI.des(scene).GetHash3();
												Utilities.MoveFromTo(ref targetList, arrayIndex, backupArraIndex);
											}
											break;

										case MemType.Custom:
											{
												var targetList = controller.GetCategoryIndex(scene) == 0 ? adapter.MOI.des(scene).GetHash4() : adapter.MOI.des(scene).GetBookMarks()[controller.GetCategoryIndex(scene)].array;
												Utilities.MoveFromTo(ref targetList, arrayIndex, backupArraIndex);
											}
											break;

										case MemType.Hier:
											{
												var targetList = adapter.MOI.des(scene).HierarchyCache();
												Utilities.MoveFromTo(ref targetList, arrayIndex, backupArraIndex);
											}
											break;

										case MemType.Scenes:
											{
												var targetList = Hierarchy_GUI.GetLastScenes(adapter);
												Utilities.MoveFromTo(ref targetList, arrayIndex, backupArraIndex);
											}
											break;
									}*/


									if ( type == MemType.Scenes )
									{
										var sm = (SceneMemory)rar[backupArraIndex];
										var path = sm.additional_GUID.Select(g => AssetDatabase.GUIDToAssetPath(g)).Where(g => !string.IsNullOrEmpty(g));
										CustomDragData.SetDragData( path.Select( p => AssetDatabase.LoadAssetAtPath<SceneAsset>( p ) ).Where( s => s ).ToArray(), type );
										DragAndDrop.StartDrag( "Dragging Scenes" );
									}

									else if ( type == MemType.Last )
									{
										//var targetList = adapter.MOI.des(scene).GetHash3();
										//	SetDragData(INT32_TOOBJECTASLISTCT(targetList[backupArraIndex]), type);
										CustomDragData.SetDragData( rar[ backupArraIndex ].gos_get(), type );
										DragAndDrop.StartDrag( "Dragging GameObject" );
									}

									else if ( type == MemType.Custom )
									{
										//var targetList = controller.GetCategoryIndex(scene) == 0 ? adapter.MOI.des(scene).GetHash4() : adapter.MOI.des(scene).GetBookMarks()[controller.GetCategoryIndex(scene)].array;
										//SetDragData(INT32_TOOBJECTASLISTCT(targetList[backupArraIndex]), type);
										CustomDragData.SetDragData( rar[ backupArraIndex ].gos_get(), type );
										DragAndDrop.StartDrag( "Dragging GameObject" );
									}


									Tools.EventUse();
									controller.ClearAction();
									controller.RepaintNow();
								}
							}


							return Event.current.delta.x == 0 && Event.current.delta.x == 0;


						}; //ACTION
					}

					else if ( Event.current.button == 2 || Event.current.button == 1 )
					{
						controller.selection_button = idOffset + i;
						controller.selection_window = controller.tempRoot;
						var captureCell = cell;
						var arrayIndex = memoryRoot[i].Arrayindex;
						var rar = memoryRoot;
						var captureType = type;
						var b = Event.current.button;
						controller.selection_action = ( mouseUp, deltaTIme ) => {
							if ( mouseUp && captureCell.Contains( Event.current.mousePosition ) )
							{
								//RemoveAndRefresh(type, arrayIndex, controller.GetCategoryIndex(scene), scene);
								Action result = () =>{
									rar.RemoveAt( arrayIndex );
									SET_OBJECTS_LIST( rar, captureType, controller, scene );
									//Tools.EventUse();
									controller.ClearAction();
									controller.RepaintNow();
								};
								if ( b == 1 )
								{
									GenericMenu menu = new GenericMenu();

									if ( type == MemType.Scenes )
									{
										var rar2 = memoryRoot;
										var Arrayindex = memoryRoot[i].Arrayindex;
										menu.AddItem( new GUIContent( "Pin" ), rar2[ Arrayindex ].pin, () => {
											rar2[ Arrayindex ].pin = !rar2[ Arrayindex ].pin;
											SET_OBJECTS_LIST( rar2, MemType.Scenes, controller, scene );
										} );
										menu.AddSeparator( "" );
									}


									menu.AddItem( new GUIContent( "Remove" ), false, () => result() );
									menu.AddSeparator( "" );
									menu.AddDisabledItem( new GUIContent( "MMB - quick remove" ) );
									menu.ShowAsContext();
								}
								else
								{
									result();
								}

							}

							return Event.current.delta.x == 0 && Event.current.delta.x == 0;
						}; // ACTION
					} //if button
				} //contains

				if ( Event.current.type == EventType.Repaint )
				{
					var style = refStyle;
					var oldColor = GUI.color;
					GUI.color *= refColor;
					var oldA = style.alignment;
					style.alignment = TextAnchor.MiddleLeft;
					var oldB = style.border;

					var oldF = style.fontSize;
					style.fontSize = adapter.FONT_8();
					var padl = style.padding.left;
					var padt = style.padding.top;
					var padb = style.padding.bottom;
					var over = style.wordWrap;
					style.wordWrap = false;
					style.padding.top = type == MemType.Last ? 3 : 0;
					if ( controller.controller_type == 1 )
					{
						style.padding.top = 0;
						style.padding.bottom = 0;
					}
					content.text = memoryRoot[ i ].ToString();
					var ymon = style.fixedHeight;
					style.fixedHeight = 0;

					var r_info = GET_INFO_RECT(type, cell);
					var infoContains = type == MemType.Custom && !SHOW_DES && BOOKMARKS_OB_SHOWDESCRIPTS && r_info.Contains(Event.current.mousePosition);

					var cellRectContains = cell.Contains(Event.current.mousePosition);
					var cellAction = ExternalModRoot.EQUALS(controller.selection_window , controller.tempRoot ) && controller.selection_button == idOffset + i;
					var active = cellAction && !controller.selection_info && cellRectContains && !infoContains;
					//var active = cellAction;

					Color iconColor = Color.white;
					Texture icon = null;

					if ( type == MemType.Scenes )
					{
						var checkPin = ExternalModRoot.EQUALS(controller.selection_window , controller.tempRoot) && controller.selection_button == 1000000 + i;

						if ( checkPin && !memoryRoot[ i ].pin ) icon = BOTTOM_SCENE_DOWN;
						else if ( memoryRoot[ i ].pin ) icon = BOTTOM_SCENE_ACTIVE;
						else icon = SCENE;

						var guid_equals = memoryRoot[i].GuidEquals();

						active |= guid_equals;
					}
					else if ( type == MemType.Hier )
					{
						icon = NEW_BOTTOM_HIERARCHY_ICON;
					}
					else
					{
						var g = memoryRoot[i].gos_get_first();
						if ( g )
						{
							var h = Cache.GetHierarchyObjectByInstanceID(g);
							var context = h.GetIconForExternal();
							{
								icon = context.add_icon;
								if ( icon && context.add_hasiconcolor )
									iconColor = context.add_iconcolor;
								if ( !icon ) icon = EditorGUIUtility.ObjectContent( h.go, Tools.unityGameObjectType ).image;
							}
						}
					}


					Rect drawCell = cell;
					if ( type == MemType.Custom && !icon ) icon = FAV;

					tempColor = null;
					///////////////////////////////////////
					///////////////////////////////////////
					///////////////////////////////////////
					////// SYNCHRONIZATION FAVORITE ///////
					var drawCount = (type == MemType.Last || type == MemType.Custom) && (memoryRoot[i].gos_get().Length > 1) || type == MemType.Scenes && memoryRoot[i].additional_GUID != null
																																							  && memoryRoot[i].additional_GUID.Length > 1;
					if ( icon != null || drawCount ) //style.padding.left = 6;
					{
						var LH = Math.Min(__LH, (int)cell.width / 3);
						var drawOffset = drawCell;
						if ( !sceneIconPosRight || type != MemType.Scenes ) style.padding.left = LH;
						if ( type == MemType.Last && memoryRoot[ i ].IsSelectedHadrScan( controller ) ) active = true;
						if ( (type == MemType.Last || type == MemType.Custom) )
						{
							if ( ROW_PAR.DrawHiglighter && memoryRoot[ i ].IsActive() )
							{
								//if (memoryRoot[i].gos_get()[ 0 ].name == "probe backed" ) Debug.Log( tempColor == null );
								tempColor = adapter.modsController.highLighterMod.GetSavedColor( Cache.GetHierarchyObjectByInstanceID( memoryRoot[ i ].gos_get()[ 0 ] ) );
							}
						}

						var bC = styleColor;
						var cC = Color.white;


						var isSelect = wasSelect ?? memoryRoot[i].IsValid() && memoryRoot[i].IsSelectedHadrScan(controller);



						DRAWSTYLE_A( style, bC, cC, drawOffset, active, type, controller, memoryRoot[ i ], idOffset + i, tempColor );

						if ( isSelect )
						{
							EditorGUI.DrawRect( drawCell, type == MemType.Custom ? Colors.SelectColor : Colors.SelectColorOverrided( false ) /* adapter.SelectColor*/);
							//wasSelect = false;
						}


						if ( tempColor != null && tempColor.HAS_BG_COLOR )
						{

							var c = tempColor.BGCOLOR;
							c.a = (byte)(c.a * adapter.HL_SET_G( 0 ).HIGHLIGHTER_COLOR_OPACITY * ROW_PAR.HiglighterOpacity);
							var oc = GUI.color;
							if ( isSelect ) c.a = (byte)(c.a / 255f * 200f);
							var bgrect = drawCell;
							if ( tempColor.BG_HEIGHT == 1 )
							{
								var newH = bgrect.height - adapter.labelStyle.CalcHeight(content, 10000) + 4;
								bgrect.y += newH / 2;
								bgrect.height -= newH;
							}

							else if ( tempColor.BG_HEIGHT == 2 )
							{
								bgrect.y += bgrect.height / 2;
								bgrect.height = 1;
							}

							float LEFT = 0;
							float RIGHT = bgrect.width;
							if ( tempColor.BG_ALIGMENT_LEFT >= 3 ) LEFT = 0.75f * bgrect.width;
							if ( tempColor.BG_ALIGMENT_RIGHT == 3 || tempColor.BG_ALIGMENT_RIGHT == 4 ) RIGHT = 0.15f * bgrect.width;
							bgrect.x += LEFT;
							bgrect.width -= (bgrect.width - RIGHT);
							bgrect.width -= LEFT;
							if ( tempColor.BG_HEIGHT != 2 )
							{
								bgrect.y += 1;
								bgrect.height -= 2;
							}
							if ( tempColor.BG_HEIGHT == 2 )
							{
								GUI.BeginClip( bgrect );
								bgrect.x = bgrect.y = 0;
							}
							adapter.modsController.highLighterMod.DRAW_BGTEXTURE_OLD( bgrect, c );
							if ( tempColor.BG_HEIGHT == 2 ) GUI.EndClip();
							GUI.color = oc;
						}
						if ( tempColor != null && tempColor.HAS_LABEL_COLOR )
						{
							var c = tempColor.LABELCOLOR;
							if ( c.r != 0 || c.g != 0 || c.b != 0 || c.a != 0 )
							{
								SetStyleColor( style, c );
								cC = Color.white;
							}
						}
						DRAWSTYLE_B( style, bC, cC, drawOffset, active, type, controller, memoryRoot[ i ], idOffset + i, tempColor, ROW_PAR.fontSize, __LH );
						RestoreStyleColor( style );
						////// SYNCHRONIZATION FAVORITE ///////
						///////////////////////////////////////
						///////////////////////////////////////
						///////////////////////////////////////



						//////////////////////////////////////////////////////
						if ( active && style == adapter.box ) EditorGUI.DrawRect( drawOffset, Colors.colorStatic );

						drawOffset = drawCell;

						var oldH = drawOffset.height;
						drawOffset.height = LH;
						drawOffset.y += (oldH - drawOffset.height) / 2;
						drawOffset.width = drawOffset.height;

						var OOFF =  controller.controller_type == 0 ? 2 : ( -OOFF2 -1);

						if ( type == MemType.Last )
						{
							if ( controller.controller_type == 0 )
							{
								drawOffset.y += OOFF / 2;
								drawOffset.width -= OOFF;
								drawOffset.height -= OOFF;
							}
							else
							{
								drawOffset = Shrink( drawOffset, OOFF / 2 );
							}
						}

						if ( icon != null )
						{

							if ( type == MemType.Scenes )
							{
								drawOffset = Shrink( drawOffset, OOFF + 1 );
								if ( sceneIconPosRight )
									drawOffset.x = drawCell.width + drawOffset.x - drawOffset.width;
							}

							if ( type == MemType.Custom || type == MemType.Last )
							{
								//if ( controller.controller_type == 0 )
								drawOffset = Shrink( drawOffset, OOFF );
								if ( type == MemType.Custom && controller.controller_type == 1 )
									drawOffset = Shrink( drawOffset, 3 );
							}


							var c = GUI.color;
							GUI.color *= iconColor;
							var ir = drawOffset;
							if ( ir.height > 50 )
							{
								ir.y += (ir.height - 50) / 2;

								if ( sceneIconPosRight )
									if ( type == MemType.Scenes ) ir.x = ir.x + ir.width - 50;

								ir.width = ir.height = 50;
							}
							//ir.x = Mathf.RoundToInt( ir.x );
							//ir.y = Mathf.RoundToInt( ir.y );
							//ir.width = Mathf.RoundToInt( ir.width );
							//ir.height = Mathf.RoundToInt( ir.height );


							if ( type == MemType.Scenes )
							{
								var asd = drawCell;

								asd.width = ir.x + ir.width / 1.2f - asd.x;
								GUI.DrawTexture( asd, FAV );
							}


							GUI.DrawTexture( ir, icon );
							GUI.color = c;
						}
					}

					else
					{
						style.padding.left = 2;
						if ( type == MemType.Last && memoryRoot[ i ].IsSelectedHadrScan( controller ) ) active = true;
						if ( (type == MemType.Last || type == MemType.Custom) )
						{
							if ( ROW_PAR.DrawHiglighter && memoryRoot[ i ].IsActive() )
								tempColor = adapter.modsController.highLighterMod.GetSavedColor( Cache.GetHierarchyObjectByInstanceID( memoryRoot[ i ].gos_get()[ 0 ] ) );
						}

						var bC = styleColor;
						var cC = styleColor;
						var isSelect = wasSelect ?? memoryRoot[i].IsActive() && memoryRoot[i].IsSelectedHadrScan(controller);


						DRAWSTYLE_A( style, bC, cC, drawCell, active, type, controller, memoryRoot[ i ], idOffset + i, tempColor );

						if ( isSelect )
						{
							EditorGUI.DrawRect( drawCell, type == MemType.Custom ? Colors.SelectColor : Colors.SelectColorOverrided( false ) /* adapter.SelectColor*/);
							//wasSelect = false;
						}


						if ( tempColor != null && tempColor.HAS_BG_COLOR )
						{
							var c = tempColor.BGCOLOR;
							c.a = (byte)(c.a * adapter.HL_SET_G( 0 ).HIGHLIGHTER_COLOR_OPACITY * ROW_PAR.HiglighterOpacity);
							var oc = GUI.color;
							if ( isSelect ) c.a = (byte)(c.a / 255f * 200f);
							var bgrect = drawCell;
							if ( tempColor.BG_HEIGHT == 1 )
							{
								var newH = bgrect.height - adapter.labelStyle.CalcHeight(content, 10000) + 4;
								bgrect.y += newH / 2;
								bgrect.height -= newH;
							}
							else if ( tempColor.BG_HEIGHT == 2 )
							{
								bgrect.y += bgrect.height / 2;
								bgrect.height = 1;
							}

							float LEFT = 0;
							float RIGHT = bgrect.width;
							if ( tempColor.BG_ALIGMENT_LEFT >= 3 ) LEFT = 0.75f * bgrect.width;
							if ( tempColor.BG_ALIGMENT_RIGHT == 3 || tempColor.BG_ALIGMENT_RIGHT == 4 ) RIGHT = 0.15f * bgrect.width;
							bgrect.x += LEFT;
							bgrect.width -= (bgrect.width - RIGHT);
							bgrect.width -= LEFT;

							if ( tempColor.BG_HEIGHT != 2 )
							{
								bgrect.y += 1;
								bgrect.height -= 2;
							}
							if ( tempColor.BG_HEIGHT == 2 )
							{
								GUI.BeginClip( bgrect );
								bgrect.x = bgrect.y = 0;
							}
							adapter.modsController.highLighterMod.DRAW_BGTEXTURE_OLD( bgrect, c );
							if ( tempColor.BG_HEIGHT == 2 ) GUI.EndClip();
							GUI.color = oc;
						}

						var oldC = style.normal.textColor;
						if ( tempColor != null && tempColor.HAS_LABEL_COLOR )
						{
							var c = tempColor.LABELCOLOR;

							if ( c.r != 0 || c.g != 0 || c.b != 0 || c.a != 0 )
							{
								SetStyleColor( style, c );
								cC = Color.white;
							}
						}
						DRAWSTYLE_B( style, bC, cC, drawCell, active, type, controller, memoryRoot[ i ], idOffset + i, tempColor, ROW_PAR.fontSize, __LH );
						RestoreStyleColor( style );
						if ( active && style == adapter.box ) EditorGUI.DrawRect( drawCell, Colors.colorStatic );
					}



					if ( type == MemType.Custom && !SHOW_DES && BOOKMARKS_OB_SHOWDESCRIPTS )
					{
						var o = memoryRoot[i].IsActive() ? Cache.GetHierarchyObjectByInstanceID(memoryRoot[i].gos_get_first()) : null;
						if ( o != null )
						{
							var data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModDescription, o);
							var containsKey = false;
							if ( data != null && !string.IsNullOrEmpty( data.stringValue ) )
							{
								containsKey = true;
							}
							var c = GUI.color;
							if ( !containsKey )
							{
								GUI.color *= coloAlpha;
								GUI.DrawTexture( r_info, EditorGUIUtility.isProSkin ? BOTTOM_INFO_DISABLE : BOTTOM_INFO_DISABLE_PERSONAL );
							}
							else
							{
								GUI.DrawTexture( r_info, EditorGUIUtility.isProSkin ? BOTTOM_INFO : BOTTOM_INFO_PERSONAL );
							}

							GUI.color = c;
							EditorGUIUtility.AddCursorRect( r_info, MouseCursor.ArrowPlus );
							if ( infoContains )
							{
								if ( cellAction && cellRectContains )
								{
									GUI.DrawTexture( Shrink( r_info, -5 ), HIPERUI_BUTTONGLOW );
								}
							}

							if ( r_info.Contains( Event.current.mousePosition ) )
							{
								if ( containsKey )
								{

									//	var d = adapter.DescriptionModule.GetValue(scene.GetHashCode(), o);
									//Label(r_info, new GUIContent() { tooltip = "- " + d });
									GUI.Label( r_info, new GUIContent() { tooltip = "- " + data.stringValue } );
								}
								else
								{
									//Label(r_info, new GUIContent() { tooltip = "No Description\nLeft CLICK to add Description" });
									GUI.Label( r_info, new GUIContent() { tooltip = "No Description\nClick to add Description" } );
								}
							}
						}

					}


					if ( drawCount )
					{
						var drawOffset = drawCell;
						var LH = Math.Min(__LH, (int)cell.width / 3);
						DRAW_COUNT( drawOffset, LH, type, memoryRoot[ i ], type == MemType.Last && !icon );
					}


					style.alignment = oldA;
					style.border = oldB;
					style.fixedHeight = ymon;
					style.wordWrap = over;
					style.fontSize = oldF;
					style.padding.left = padl;
					style.padding.top = padt;
					style.padding.bottom = padb;
					GUI.color = oldColor;
				}


				if ( !DisableCursor ) EditorGUIUtility.AddCursorRect( cell, MouseCursor.Link );

				var LH2 = Math.Min(__LH, (int)cell.width / 3);

				if ( type == MemType.Scenes )
				{
					var linkRect = cell;
					if ( sceneIconPosRight )
					{
						linkRect.x = cell.x + cell.width - LH2;
						linkRect.width = cell.x + cell.width - linkRect.x;
					}
					else
					{
						linkRect.width = LH2;
					}
					EditorGUIUtility.AddCursorRect( linkRect, MouseCursor.Link );
				}

				if ( ROW_PAR.DrawTooltips && cell.Contains( Event.current.mousePosition ) ) //|| type == MemType.Scenes
				{
					//var ctt = GETTOOLTIPPEDCONTENT(type, type == MemType.Custom ? memoryRoot[i].ToString() : memoryRoot[i].FullString(), controller);
					var ctt = GETTOOLTIPPEDCONTENT(type,  memoryRoot[i].FullString(), controller);
					if ( type == MemType.Scenes &&
						(sceneIconPosRight && Event.current.mousePosition.x > cell.x + cell.width - LH2 ||
						!sceneIconPosRight && Event.current.mousePosition.x < cell.x + LH2)
						)
					{
						ctt.tooltip += '\n';
						ctt.tooltip += pinScene.tooltip;
					}
					else if ( type == MemType.Custom )
					{
						var o = memoryRoot[i].IsActive() ? Cache.GetHierarchyObjectByInstanceID(memoryRoot[i].gos_get_first()) : null;
						if ( type == MemType.Custom && o != null )
						{

							var data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModDescription, o);
							if ( data != null && !string.IsNullOrEmpty( data.stringValue ) )
							{
								var d = data.stringValue;
								ctt.tooltip += "- " + d;
							}

							else
							{
								ctt.tooltip += "- No Description";
							}
						}

						else
						{
							ctt.tooltip += "- ...";
						}
					}
					/*else
					{
						ctt.tooltip = "";
					}*/

					if ( !string.IsNullOrEmpty( ctt.tooltip ) && (DragAndDrop.visualMode == DragAndDropVisualMode.None && !controller.wasDrag) )
					{
						ctt.tooltip = ctt.tooltip.Trim( tr );
						GUI.Label( cell, ctt );
						//Label(cell, ctt);
					}
				}

			}


			if ( controller.selection_action != null && controller.wasDrag )
			{
				var c = GUI.color;
				var c2 = c;
				c2.a = 0.5f;
				GUI.color = c2;
				GUI.DrawTexture( DragRect, adapter.button.active.background );
				GUI.color = c;
			}


			return wasDraw;
		}


	}
}
