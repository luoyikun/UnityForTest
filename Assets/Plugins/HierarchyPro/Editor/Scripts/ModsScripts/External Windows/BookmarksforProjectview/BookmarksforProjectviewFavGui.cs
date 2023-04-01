using System;
using UnityEngine;
using UnityEditor;



namespace EMX.HierarchyPlugin.Editor.Mods.BookProject
{

	partial class BookmarksforProjectviewModInstance
	{



		int LINE_HEIGHT()
		{
			return adapter.par_e.BOOKMARKS_FOLDER_LINE_HEIGHT;
		}

		int SHOW_ALL_CONTENT = 1;

		int? drawg1 = null;
		int? drawg2 = null;
		int? drawg3 = null;
		float LH;
		int FONT_SIZE { get { return adapter.par_e.BOOKMARKS_FOLDER_FONTSIZE; } }

		int indents = 8;

		// bool SKIP;
		//List<Int32ListArray> refBookmarks;
		//List<Int32List> refBookmarks;

		/*public class ColabseCache
		{
			PluginInstance adapter;

			public ColabseCache(Adapter adapter)
			{
				this.adapter = adapter;
			}

			static Dictionary<int, Dictionary<string, bool>>[] colabsed_cache = new[] { new Dictionary<int, Dictionary<string, bool>>(), new Dictionary<int, Dictionary<string, bool>>() };

			public bool Get(int window, Int32ListArray categoryid, string hierarchyid, bool IS_ROOT)
			{
				if (!colabsed_cache[window].ContainsKey(categoryid.InstanceID)) colabsed_cache[window].Add(categoryid.InstanceID, new Dictionary<string, bool>());
				if (!colabsed_cache[window][categoryid.InstanceID].ContainsKey(hierarchyid))
				{
					var v = SessionState.GetInt("EModules/Hierarchy/" + adapter.pluginname + "ColCach" + window + " " + categoryid + " " + hierarchyid, 0);
			
					colabsed_cache[window][categoryid.InstanceID].Add(hierarchyid, v == 1);
				}

				return colabsed_cache[window][categoryid.InstanceID][hierarchyid];
			}


			public void Set(bool value, int window, Int32ListArray categoryid, string hierarchyid, bool IS_ROOT)
			{
				if (IS_ROOT) hierarchyid += "1";
				var ov = Get(window, categoryid, hierarchyid, IS_ROOT);
				if (ov == value) return;
				// Debug.Log(window + " " + categoryid + " " + hierarchyid);
				SessionState.SetInt("EModules/Hierarchy/" + adapter.pluginname + "ColCach" + window + " " + categoryid + " " + hierarchyid, value ? 1 : 0);
				colabsed_cache[window][categoryid.InstanceID][hierarchyid] = value;
			}
		}

		ColabseCache __ColabseCache;

		ColabseCache COLABSE_CACHE
		{
			get { return __ColabseCache ?? (__ColabseCache = new ColabseCache(adapter)); }
		}*/

		float?[] HEIGHT = new float?[2];
		int HL_DR = 0;
		int CONTROLLER;


		bool IsSelected(int id)
		{
			return false;
		}

		internal void repaint()
		{
			if (CURRENT_CONTROLLER != null )
			{
				CURRENT_CONTROLLER.tempRoot.FORCE_REPAINT_THROUGH_LAYOUT = true;
				CURRENT_CONTROLLER.RepaintNow();
			}
			else
			{
				adapter.RepaintWindowInUpdate( 1 );
			}
		}

		public void FAVORIT_GUI(Rect fullRect, FavorControllerWindow controller)
		{
			//if (!adapter.bottomInterface.m_memCache.ContainsKey(Adapter.BottomInterface.MemType.Custom)) return;

			CURRENT_CONTROLLER = controller;
			INIT_STYLES();


			// SKIP = false;
			//Mathf.Clamp( current_controller.GetCategoryIndex( scene
			// var indents = adapter.par.DEEP_WIDTH ?? 14;


			LH = Mathf.Max(16, LINE_HEIGHT());
			var refBookmarks = GET_BOOKMARKS();
			var L = refBookmarks.Count;
			//  var L = 1;

			// var L = adapter.bottomInterface.m_memCache[Adapter.BottomInterface.MemType.Custom].Length;
			// refBookmarks[]

			//  Debug.Log( fullRect );
			var fullCell = new Rect(fullRect.x, fullRect.y, fullRect.width, LH);
			CONTROL_ID = idOffset;

			CONTROLLER = controller.MAIN ? 0 : 1;

			float height = 0;
			if (HEIGHT[CONTROLLER] == null)
			{
				for (int catIndex = 0; catIndex < L; catIndex++)
				{
					var item = refBookmarks[catIndex];
					//	var memoryRoot = adapter.bottomInterface.m_memCache[Adapter.BottomInterface.MemType.Custom][catIndex];
					/* if (!colabsed_cache[C_I].ContainsKey( refBookmarks[catIndex].InstanceID ))
					 {   colabsed_cache[C_I].Add( refBookmarks[catIndex].InstanceID, new Dictionary<long, bool>() );
						 if (controller.MAIN)
						 {   if (refBookmarks[catIndex].ColabsedItems == null) refBookmarks[catIndex].ColabsedItems = new List<long>();
							 colabsed_cache[C_I][refBookmarks[catIndex].InstanceID] = refBookmarks[catIndex].ColabsedItems.ToDictionary( i => i, i => false );
						 }
						 else
						 {   if (refBookmarks[catIndex].Windows_ColabsedItems == null) refBookmarks[catIndex].Windows_ColabsedItems = new List<long>();
							 colabsed_cache[C_I][refBookmarks[catIndex].InstanceID] = refBookmarks[catIndex].Windows_ColabsedItems.ToDictionary( i => i, i => false );
						 }
					 }*/
					//	var item = GET_TREE_ITEM(refBookmarks[catIndex].InstanceID.ToString(), refBookmarks[catIndex], CURRENT_CONTROLLER, false);
					height += fullCell.height * 2;
					if (!item.expanded) continue;
					for (int i = 0; i < item.slots.Count; i++)
					{
						if (!item.slots[i].IsValid()) continue;
						height += fullCell.height;
					}
				}

				height += EditorGUIUtility.singleLineHeight + 6;
			}
			else
			{
				height = HEIGHT[CONTROLLER] ?? 0;
			}
			if (float.IsNaN(refBookmarks[0].scrollY)) refBookmarks[0].scrollY = 0;

			var workscroll = Math.Max(height - fullRect.height, 0);
			scrollPos.y = refBookmarks[0].scrollY;
			if (HEIGHT[CONTROLLER] != null && (scrollPos.y < 0 || scrollPos.y > workscroll))
			{
				scrollPos.y = Mathf.Clamp(scrollPos.y, 0, workscroll);
				refBookmarks[0].scrollY = scrollPos.y;
				// SET_DIRTY( scene );
			}

			var scrollRect = fullRect;
			var drawH = (scrollRect.height);
			if (controller.MAIN) drawH -= fullCell.height;
			var HH = Mathf.Min(scrollRect.height / height, 1) * drawH;
			var XX = scrollPos.y / workscroll * (drawH - HH);
			scrollRect.x += scrollRect.width - 7;
			scrollRect.width = 4;
			scrollRect.y = XX;
			if (controller.MAIN) scrollRect.y += fullCell.height;
			scrollRect.height = HH;
			EVENT_EVENT_EVENT(scrollRect, "SCROLL_DOWN", "SCROLL_DRAG", null, controller, controller, CONTROL_ID++, true);
			pixelCost[CONTROLLER] = (drawH - scrollRect.height) == 0 ? 0 : workscroll / (drawH - scrollRect.height);
			// Debug.Log( pixelCost );

			drawg1 = null;
			drawg2 = null;
			drawg3 = null;
			if (DragAndDrop.objectReferences.Length != 0)
			{
				drawg1 = DragAndDrop.GetGenericData("FavI1") as int?;
				drawg2 = DragAndDrop.GetGenericData("FavI2") as int?;
				drawg3 = DragAndDrop.GetGenericData("FavI3") as int?;

				var drag = DragAndDrop.GetGenericData("FavI4") as int?;
				HL_DR = drag.HasValue && IsSelected(drag.Value) ? drag.Value : 0;
			}
			else
			{
				HL_DR = 0;
			}
			//Debug.Log( EditorApplication.isCompiling + " " + Event.current.type + " " + (HEIGHT[ CONTROLLER ] == null) + " " + CURRENT_WIN.FORCE_REPAINT_THROUGH_LAYOUT);
			// GUI.BeginScrollView( fullRect, scrollPos, new Rect( fullRect.x, fullRect.y, fullRect.width, height ), true, true, vsstyle(), vsstyle() );
			GUI.BeginGroup(new Rect(fullRect.x, fullRect.y - scrollPos.y, fullRect.width, height));
			for (int catIndex = 0; catIndex < L && catIndex < refBookmarks.Count; catIndex++)
			{
				var memoryRoot = refBookmarks[catIndex];
				// var categoryName = refBookmarks[catIndex].name;
				var categoryColor = !adapter.par_e.BOOKMARKS_FOLDER_DRAW_BG_COLOR ? Color.clear : refBookmarks[catIndex].BgColor;

				var DEEP = 0;
				// cell = fullCell;
				fullCell.height *= 2;
				var cell = fullCell;
				cell.x = indents * DEEP;
				cell.width -= indents * DEEP;


				if (categoryColor.HasValue) adapter.gl.DRAW_TAP_GLOW(Shrink(fullCell, 1), categoryColor.Value);
				// EditorGUIUtility.AddCursorRect( fullCell, MouseCursor.Link );


				DO_FOLD(false, ref cell, refBookmarks[catIndex], null, null);
				//var item =
				mouseEventStruct.Set_Get(null, null, -1, catIndex, -1, CURRENT_CONTROLLER, 0);




				//	var fs = labelStyle.fontStyle;
				content.text = refBookmarks[catIndex].category_name;


				//	labelStyle.fontStyle = FontStyle.Bold;

				var t = (refBookmarks[catIndex].ShowContent) == SHOW_ALL_CONTENT ? FAVORIT_FOLDERS_ICON_OFF : FAVORIT_FOLDERS_ICON;
				var ir = cell;
				cell.x += t.width;
				cell.width -= t.width;
				ir.width = t.width;
				// string tooltipstr= "Show only special chosen file types";
				string tooltipstr = "Include all assets in folder";
				if ((refBookmarks[catIndex].ShowContent) == SHOW_ALL_CONTENT) tooltipstr += " (disabled)";
				else tooltipstr += " - Enabled";
				EVENT_EVENT_EVENT(ir, null, null, "FAVORIT_FOLDERS_ICON_METHOD", mouseEventStruct, CURRENT_CONTROLLER, CONTROL_ID++, true, 0, tooltip: tooltipstr);
				ir.y += (cell.height - t.height) / 2;
				ir.height = t.height;
				DrawTexture(ir, t);
				DRAW_LABEL(cell, content, FONT_SIZE, Color.white);
				//labelStyle.fontStyle = fs;


				var si = FAVORIT_FILTER_ICON;
				ir = cell;
				ir.x = ir.x + ir.width - ir.height / 2;
				ir.y += ir.height / 2;
				ir.width = ir.height = si.height;
				ir.x -= ir.width / 2;
				ir.y -= ir.height / 2;
				/* if ((refBookmarks[catIndex].FavParams ) == SHOW_ALL_CONTENT)
				 {   GUI.Box(ir, "");
					 Adapter.TOOLTIP(ir, tooltipstr);
				 }
				 else
				 {   Adapter.DrawTexture( ir, si );
					 Adapter.TOOLTIP(ir, tooltipstr);
				 }*/

				if (memoryRoot.expanded)
				{
					ir = cell;
					ir.x = ir.x + ir.width - EditorGUIUtility.singleLineHeight / 2;
					ir.width = EditorGUIUtility.singleLineHeight * 6;
					ir.x -= ir.width;
					ir.y += (ir.height - EditorGUIUtility.singleLineHeight) / 2;
					ir.height = EditorGUIUtility.singleLineHeight;
					string[] cats = new[] {  "Deep" , "Simple" }; //ΞΞΞ //☵☱☷
					var ov = refBookmarks[catIndex].ShowContent == SHOW_ALL_CONTENT ? 1 : 0;
					var nv = GUI.Toolbar(ir, ov, cats, EditorStyles.toolbarButton);
					if (ov != nv)
					{
                        HierarchyCommonData.Instance().SetUndo( "Apply Project Bookmarks" );
                        refBookmarks[ catIndex].ShowContent = SHOW_ALL_CONTENT;
						if (nv != 1) refBookmarks[catIndex].ShowContent = 1 - refBookmarks[catIndex].ShowContent;
                        HierarchyCommonData.Instance().SetDirty();
                    }

                    TOOLTIP(ir, tooltipstr);
				}


				DRAG_DRAG(fullCell, true, controller, -1, ref refBookmarks, drawg1, drawg3, -1, ref refBookmarks, catIndex, refBookmarks.Count);
				EVENT_EVENT_EVENT(fullCell, null, null, "CATEGORY_TITLE2", mouseEventStruct, CURRENT_CONTROLLER, CONTROL_ID++, true, 0);
				EVENT_EVENT_EVENT(fullCell, null, null, "CATEGORY_TITLE2", mouseEventStruct, CURRENT_CONTROLLER, CONTROL_ID++, true, 1);


				fullCell.y += fullCell.height;
				fullCell.height /= 2;

				//BREAK
				if (!memoryRoot.expanded) continue;
				catIndex = Mathf.Clamp(catIndex, 0, refBookmarks.Count);


				DRAW_CATEGORY_DATA drawData = new DRAW_CATEGORY_DATA();
				drawData.fullCell = fullCell;
				drawData.scrollRect = fullRect;
				drawData.memoryRoot = refBookmarks;
				drawData.catIndex = catIndex;
				drawData.scene = -1;
				drawData.controller = controller;
				FAV_NEW_GUI(ref drawData);
				fullCell = drawData.fullCell;
			}

			GUI.EndGroup();
			fullCell.y += EditorGUIUtility.singleLineHeight + 6;
			if (HEIGHT[CONTROLLER] == null)
			{
				repaint();
				//adapter.RepaintWindowInUpdate(1);
			}
			if (HEIGHT[CONTROLLER].HasValue && HEIGHT[CONTROLLER].Value != fullCell.y)
			{
				HEIGHT[CONTROLLER] = fullCell.y;

				repaint();
			}
			else
			{
				HEIGHT[CONTROLLER] = fullCell.y;
			}

			fullCell.y -= EditorGUIUtility.singleLineHeight + 6;

			var ch = fullCell.y;
			var crr = new Rect(fullRect.x, ch + fullRect.y - scrollPos.y, fullRect.width, Mathf.Max(EditorGUIUtility.singleLineHeight + 6, fullRect.height - ch));
			if (crr.height > 0)
			{
				adapter.gl.DRAW_TAP_GLOW(Shrink(crr, 1), new Color32(47, 75, 114, 20));
				mouseEventStruct.Set_Get(null, null, -1, -1, -1, controller, 0);
				EVENT_EVENT_EVENT(crr, null, null, "CREATE_NEW_CAT", mouseEventStruct, CURRENT_CONTROLLER, CONTROL_ID++, true, 0);
				if (Event.current.type == EventType.Repaint) adapter.button.Draw(crr, "Create New Category", false, false, false, false);
			}


			if (Event.current.type == EventType.Repaint)
			{
				var shRect = fullRect;
				/*  shRect.height -= INTERFACE_SIZE;
							  shRect.y += INTERFACE_SIZE;*/
				shRect.height = Math.Max(shRect.height, shadow.border.bottom * 2);
				shadow.Draw(shRect, false, false, false, false);
			}

			GUI.DrawTexture(scrollRect, adapter.STYLE_DEFBUTTON.active.background);
		}

























		bool?[] collabse = new bool?[1024];
		int[] deeppath = new int[1024];

		//OLD 3 LEVELS
		//                void OLD_DRAW(ref DRAW_CATEGORY_DATA _data)
		//                {
		//
		//                    //2 level
		//                    for (int i = 0 ; i < 1
		//                            //memoryRoot.Count
		//                            ; i++)
		//                    {
		//
		//                        if (!_data.memoryRoot[i].IsValid()) continue;
		//
		//                        var  DEEP = 1;
		//                        Rect cell = _data.fullCell;
		//                        cell.x = indents * DEEP;
		//                        cell.width -= indents * DEEP;
		//
		//                        ////////////////
		//                        var mayFolded = adapter.bottomInterface.INT32_COUNT(_data.memoryRoot[i].InstanceID) > 1;
		//
		//                        // Color32? bgColor = null;
		//                        //  Color32? textColor = null;
		//                        if (adapter.bottomInterface.INT32_ACTIVE(_data. memoryRoot[i].InstanceID ))
		//                        {   tempColor = adapter.MOI.M_Colors.needdrawGetColor( adapter.bottomInterface.INT32__ACTIVE_TOHIERARCHYOBJECT(_data.memoryRoot[i].InstanceID));
		//                            /* bgColor = col[0];
		//                                   textColor = col[1];
		//                                   if (textColor.HasValue)
		//                                   { */ /*  if (tempColor != null && tempColor.HAS_LABEL_COLOR)
		//                                        {    var c = tempColor.LABELCOLOR;
		//                                        if (c.r == 0 && c.g == 0 && c.b == 0 && c.a == 0)
		//                                        tempColor = null;
		//
		//
		//
		//                                        }*/
		//                        }
		//
		//                        //if (fullCell.Contains( Event.current.mousePosition ))
		//                        CURRENT_STATE = _data.memoryRoot[i].GET_SELECTION_STATE();
		//                        CURRENT_CAT = Mathf.Clamp( _data.catIndex, 0, refBookmarks.Count - 1);
		//                        CURRENT_INDEX = i;
		//
		//
		//
		//
		//
		//
		//
		//                        var higlightDrag = drawg1 == _data.catIndex && drawg2 == i;
		//                        //long ID = memoryRoot[i].InstanceID.InstanceID;
		//                        bool IS_ROOT = true;
		//                        var h = adapter.bottomInterface.INT32__ACTIVE_TOHIERARCHYOBJECT(_data.memoryRoot[i].InstanceID);
		//
		//                        mouseEventStruct.Set_Get(_data. memoryRoot[i], h, h.scene, _data.catIndex, i, _data.controller, CURRENT_STATE );
		//                        //var favString = memoryRoot[i].InstanceID.FavString;
		//
		//
		//                        /* if (fullCell.y + fullCell.height - scrollPos.y < 0) SKIP = true;
		//                         else*/
		//                        if (_data.fullCell.y - scrollPos.y > _data.scrollRect.height) SKIP = true;
		//                        else SKIP = false;
		//
		//                        // deeppath[0] = "";
		//                        // collabse = "";
		//                        DRAW_OBJECT_ITEM( h, _data.scene.GetHashCode(), tempColor, IS_ROOT, mayFolded, higlightDrag, _data.memoryRoot[i].InstanceID, _data.m_favString_index, DEEP, true, false );
		//                        ////////////////
		//
		//
		//
		//                    }
		//                }




		//                void DRAW_FOLDERS_OLD(string path, string filter, int m_DEEP,  bool flat)
		//                {   var paths = GET_PATHS(path).scanneditems;
		//                    bool hasFilter = !string.IsNullOrEmpty(filter);
		//
		//                    //int deep = 0;
		//                    deeppath[0] = "";
		//                    collabse = "";
		//                    //int dropL = -1;
		//
		//                    for (int i = 0 ; i < paths.Count ; i++)
		//                    {   if (hasFilter && paths[i].extension != filter) continue;
		//
		//                        /*  if (dropL != -1 && paths[i].DEEP > dropL) continue;
		//                          if (dropL != -1) dropL = -1;*/
		//                        //  Debug.Log(paths[i].name + "  " + paths[i].DEEP);
		//                        if (!flat)
		//                        {   for (int D_D = 1 ; D_D < paths[i].DEEP ; D_D++)
		//                            {   if (paths[i].folders[D_D] == collabse) goto __CONT;
		//                                if (deeppath[D_D] != paths[i].folders[D_D])
		//                                {
		//
		//                                    var d = D_D;
		//                                    // for (int z = D_D ; z < paths[i].DEEP ; z++)
		//                                    //for (int d = D_D ; d < paths[i].DEEP ; d++)
		//                                    {   //
		//
		//                                        deeppath[d] = paths[i].folders[d];
		//                                        DRAW_SINGLE_ITEM( paths[i].GetFoldByIndex( d ), d + m_DEEP,  true ); //draw fold
		//
		//                                        if (!GET_TREE_ITEM( paths[i].GetFoldByIndex( d ).project.guid, CURRENT_CAT, CURRENT_CONTROLLER, false ).Expand)
		//                                        {   // dropL = d + 1;
		//                                            collabse = paths[i].folders[d];
		//                                            goto __CONT;
		//                                        }
		//                                    }
		//                                    break;
		//                                }
		//                            }
		//                        }
		//
		//
		//                        if (null == paths[i].CurrentObject)
		//                        {   var guid = AssetDatabase.AssetPathToGUID( paths[i].fullPath);
		//                            paths[i].CurrentObject = adapter.GetHierarchyObjectByGUID( ref guid, "" );
		//                        }
		//                        //if (!paths[i].CurrentObject.project.IsFolder)
		//                        DRAW_SINGLE_ITEM( paths[i].CurrentObject, flat ? m_DEEP : (paths[i].DEEP + m_DEEP),  false ); //draw item
		//
		//                        //  Debug.Log(paths[i].fullPath);
		//__CONT:;
		//                    }
		//                }
		//                //MouseEventStruct dsi;
		//                void DRAW_SINGLE_ITEM_OLD(Adapter.HierarchyObject h, int DEEP,  bool overrideFold)
		//                {   cell = fullCell;
		//                    cell.x = indents * DEEP;
		//                    cell.width -= indents * DEEP;
		//
		//
		//                    //var h = paths[i].foldersObjects[i];
		//                    var higlightDrag =  HL_DR == h.id ;
		//                    // long ID = (int)masked_id << 32 | h.id;
		//                    // var favString = favStringList[i];
		//                    mouseEventStruct.Set_Get( null, h, h.scene, CURRENT_CAT, -1, CURRENT_CONTROLLER, CURRENT_STATE );
		//
		//
		//                    tempColor = adapter.MOI.M_Colors.needdrawGetColor( h);
		//                    /* if (col != null)
		//                     {   temp_bgColor = col[0];
		//                         temp_textColor = col[1];
		//                     }
		//                     else
		//                     {   temp_bgColor = null;
		//                         temp_textColor = null;
		//                     }*/
		//
		//
		//                    DRAW_OBJECT_ITEM( h, h.scene, tempColor, false, false, higlightDrag,  emptyString, -1, -1, false, overrideFold);
		//                }

	}
}
