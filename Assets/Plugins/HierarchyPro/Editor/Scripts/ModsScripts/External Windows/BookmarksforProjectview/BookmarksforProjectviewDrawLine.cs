using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;



namespace EMX.HierarchyPlugin.Editor.Mods.BookProject
{

	partial class BookmarksforProjectviewModInstance
	{

		/*internal void Label(Rect r, string s, TextAnchor an)
		{
			var a = adapter.label.alignment;
			adapter.label.alignment = an;
			GUI.Label(r, s, adapter.label);
			adapter.label.alignment = a;
		}

		internal void Label(Rect r, string s)
		{
			GUI.Label(r, s, adapter.label);
		}

		internal void Label(Rect r, GUIContent s)
		{
			GUI.Label(r, s, adapter.label);
		}

		internal bool Button(Rect r, string s)
		{
			return GUI.Button(r, s, adapter.button);
		}

		internal bool Button(Rect r, string s, TextAnchor an)
		{
			var a = adapter.button.alignment;
			adapter.button.alignment = an;
			var res = GUI.Button(r, s, adapter.button);
			adapter.button.alignment = a;
			return res;
		}

		internal bool Button(Rect r, GUIContent s)
		{
			return GUI.Button(r, s, adapter.button);
		}

		internal bool Button(Rect r, GUIContent s, TextAnchor an)
		{
			var a = adapter.button.alignment;
			adapter.button.alignment = an;
			var res = GUI.Button(r, s, adapter.button);
			adapter.button.alignment = a;
			return res;
		}
		*/


		struct DRAW_CATEGORY_DATA
		{
			public Rect fullCell; // out lilne rect
			public Rect scrollRect;
			public List<FolderBookmark> memoryRoot;
			public int catIndex; // old CURRENT_CAT
			public int scene;
			public FavorControllerWindow controller;

			internal int m_favString_index
			{
				get { return 0; }
			}
		}

		struct DRAW_ROOTITEM_DATA
		{
			public bool HasChildren;
			public int SELECTION_CURSOR_STATE; // mouse hover state
			internal int memoryRootIndex;

			internal bool higlightDrag;

			//internal HierarchyObject h;
			internal bool ISOUTOFSCREEN;
			public TempColorClass tempColor; // category custom color
			internal string ExtenstionFilter;
			internal bool FLAT_FOLDERS;
			public bool IS_ROOT;
			public bool DRAW_FOLDERS_CONTENT;

			internal DRAW_ROOTITEM_DATA CopyNonRoot()
			{
				var res = this;
				res.IS_ROOT = false;
				return res;
			}
		}



		void FAV_NEW_GUI(ref DRAW_CATEGORY_DATA _cData)
		{


			var category = _cData.memoryRoot[_cData.catIndex];
			for (int i = 0; i < category.slots.Count; i++) //memoryRoot.Count
			{
				var DEEP = 1;


				if (!category.slots[i].IsValid()) continue;

				Rect cell = _cData.fullCell;
				cell.x = indents * DEEP;
				cell.width -= indents * DEEP;


				///DRAW_ROOTITEM_DATA
				var rootData = new DRAW_ROOTITEM_DATA();

				rootData.memoryRootIndex = i;
				if (rootData.memoryRootIndex < 0) rootData.memoryRootIndex = 0;
				if (rootData.memoryRootIndex >= category.slots.Count) rootData.memoryRootIndex = category.slots.Count - 1;

				/*if (adapter.bottomInterface.INT32_ACTIVE(category.slots[i].InstanceID))
				{
					rootData.tempColor = adapter.ColorModule.needdrawGetColor(adapter.bottomInterface.INT32__ACTIVE_TOHIERARCHYOBJECT(category.slots[i].InstanceID));
				}
				else
				{
					rootData.tempColor = null;
				}*/
				rootData.tempColor = null;

				rootData.HasChildren = category.slots[i].guids.Count > 0;
				//rootData.SELECTION_CURSOR_STATE = category.slots[i].GET_SELECTION_STATE();
				rootData.SELECTION_CURSOR_STATE = 0;
				rootData.higlightDrag = drawg1 == _cData.catIndex && drawg2 == i;
				rootData.IS_ROOT = true;
				//rootData.h = adapter.bottomInterface.INT32__ACTIVE_TOHIERARCHYOBJECT(category.slots[i].InstanceID);
				var o = category.slots[i].ConvertToObjects();
				if (o.Length == 0) continue;
				rootData.ExtenstionFilter = category.slots[i].FilterString;
				if (string.IsNullOrEmpty(rootData.ExtenstionFilter)) rootData.ExtenstionFilter = null;
				rootData.DRAW_FOLDERS_CONTENT = category.ShowContent != SHOW_ALL_CONTENT;
				rootData.FLAT_FOLDERS = category.slots[rootData.memoryRootIndex].ShowContentInh == SHOW_ALL_CONTENT;

				if (_cData.fullCell.y - scrollPos.y > _cData.scrollRect.height) rootData.ISOUTOFSCREEN = true;
				else rootData.ISOUTOFSCREEN = false;
				///DRAW_ROOTITEM_DATA


				mouseEventStruct.Set_Get(category.slots[i], o, _cData.scene, _cData.catIndex, i, _cData.controller, rootData.SELECTION_CURSOR_STATE);

				tc[0].folder_object = o[ 0 ];
				tc[0].path = ASSET_PATH(o[0]);
				OBJECT_ITEM_GUI(ref _cData, rootData, tc, DEEP);
			}
		}

		folderClass[] tc = { new folderClass() };


		Dictionary<int, bool> _is_folder = new Dictionary<int, bool>();
		bool IS_FOLDER(UnityEngine.Object o)
		{
			if (!o) return false;
			if (!_is_folder.ContainsKey(o.GetInstanceID()))
			{
				var p = AssetDatabase.GetAssetPath(o);
				if (p == null || p == "")
				{
					_is_folder.Add(o.GetInstanceID(), false);
				}
				else
				{
					if (Directory.Exists(p)) _is_folder.Add(o.GetInstanceID(), true);
					else _is_folder.Add(o.GetInstanceID(), false);
				}
			}
			return _is_folder[o.GetInstanceID()];
		}
		string ASSET_PATH(UnityEngine.Object o) { if (!o) return ""; return AssetDatabase.GetAssetPath(o); }





		void OBJECT_ITEM_GUI(ref DRAW_CATEGORY_DATA _cData, DRAW_ROOTITEM_DATA _itemData, folderClass[] o, int DEEP)
		{

			if (o.Length == 0) return;

			if (!_itemData.ISOUTOFSCREEN) DRAW_LINE(ref _cData, ref _itemData, o, DEEP);


			_cData.fullCell.y += _cData.fullCell.height;
			var cat = _cData.memoryRoot[_cData.catIndex];

			if (_itemData.IS_ROOT && IS_FOLDER(o[0].folder_object) && _itemData.DRAW_FOLDERS_CONTENT && cat.slots[_itemData.memoryRootIndex].slot_expanded)
			{
				//var item = GET_TREE_ITEM(o.project.guid, refBookmarks[_cData.catIndex], CURRENT_CONTROLLER, _itemData.IS_ROOT);
				//if (item != null && item.Expand)
				//if (o[0].folder_expanded) //cat.slots[_itemData.memoryRootIndex].slot_expanded



				{
					DRAW_FOLDERS(ref _cData, ref _itemData, o, DEEP + 1);
				}
			}
		}

		/*if (FOLDER == 1 )
				{   // var favStringList = mouseEventStruct.memoryRoot.InstanceID.GET_FavString();
					//var favString = mouseEventStruct.memoryRoot.InstanceID.GET_FavString();
					var guidList = mouseEventStruct.memoryRoot.InstanceID.GUIDsList;
					var pathList = mouseEventStruct.memoryRoot.InstanceID.GET_PATHsList(adapter.pluginname);
					var captureMeem = mouseEventStruct.memoryRoot.InstanceID;

					for (int i = 0 ; i < guidList.Count ; i++)
					{   if (guidList[i] == mouseEventStruct.memoryRoot.InstanceID.GUIDsActiveGameObject) continue;

						var nextDEEP = m_DEEP + 1;
						cell = fullCell;
						cell.x = indents * nextDEEP;
						cell.width -= indents * nextDEEP;

						var guid = guidList[i];
						var h = adapter.GetHierarchyObjectByGUID(ref guid, pathList[i]);


						var higlightDrag =  HL_DR == h.id ;
						// long ID = (int)m_id << 32 | h.id;
						// var favString = favStringList[i];
						// var me = mouseEventStruct.Clone();
						mouseEventStruct.Set_Get( null, h, h.scene, CURRENT_CAT, -1, CURRENT_CONTROLLER, CURRENT_STATE );

						DRAW_OBJECT_ITEM( h, m_scene, tempColor, false, false, higlightDrag,  captureMeem, i, nextDEEP, true, false );
					}
				}*/


		//DynamicRect tempDynamicRect = new DynamicRect();

		string[] popDisplay = new string[1];
		GUIContent cc2 = new GUIContent() { tooltip = "Filter files within a folder by a certain type" };
		GUIContent cc3 = new GUIContent() { tooltip = "Hide folders and show objects as one list" };
		GUIContent desctool = new GUIContent() { tooltip = "Add descriptions" };
		TempColorClass iconTempColor = new TempColorClass();

		GUIContent TypeButtonGUIContent = new GUIContent();

		void DRAW_LINE(ref DRAW_CATEGORY_DATA _cData, ref DRAW_ROOTITEM_DATA _itemData, folderClass[] _o, int DEEP)
		{
			var o = _o[0].folder_object;
			var rect = _cData.fullCell;
			rect.x += indents * DEEP;
			rect.width -= indents * DEEP;
			//var h = paths[i].foldersObjects[i];
			//                    var higlightDrag =  HL_DR == h.id ;

			if (_itemData.SELECTION_CURSOR_STATE != 0)
			{
				EditorGUIUtility.AddCursorRect(rect, _itemData.SELECTION_CURSOR_STATE == 1 ? MouseCursor.ArrowPlus : _itemData.SELECTION_CURSOR_STATE == 2 ? MouseCursor.ArrowMinus : MouseCursor.ScaleArrow);
			}


			var HAS_TYPEBUTTON = _itemData.IS_ROOT && _itemData.HasChildren && IS_FOLDER(o) && _itemData.DRAW_FOLDERS_CONTENT;
			var HAS_DESCRIPTION = adapter.par_e.BOOKMARKS_FOLDER_SHOW_ALL_DESCRIPTIONS_INHIER;
			var HAS_FOLD = _itemData.DRAW_FOLDERS_CONTENT && IS_FOLDER(o);


			var index = -1;
			float type_width = 0;
			if (HAS_TYPEBUTTON)
			{
				var ext = GET_PATHS(ASSET_PATH(o)).extensions;
				if (_itemData.ExtenstionFilter != null && ext.ContainsKey(_itemData.ExtenstionFilter))
				{
					popDisplay[0] = _itemData.ExtenstionFilter;
					index = ext[_itemData.ExtenstionFilter];
				}
				else
				{
					popDisplay[0] = "type";
				}


				TypeButtonGUIContent.text = popDisplay[0];
				TypeButtonGUIContent.tooltip = "Select the extension that will be displayed for this folder";
				EditorStyles.popup.CalcMinMaxWidth(TypeButtonGUIContent, out type_width, out type_width);
				//if (_cData.catIndex < refBookmarks[_cData.catIndex].array.Count)
				{
					var lit_icon = _itemData.FLAT_FOLDERS ? FAVORIT_LIST_ICON_ON : FAVORIT_LIST_ICON;
					type_width += lit_icon.width;
					rect.width -= type_width;
				}
				/* else
				 {   HAS_TYPEBUTTON = false;
				 }*/
			}


			var refBookmarks = _cData.memoryRoot;
			var category = refBookmarks[_cData.catIndex];
			var slot = _itemData.IS_ROOT ? category.slots[_itemData.memoryRootIndex] : null;
			/*var guid = slot.guids[0].guid; //o.project.guid
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var name = Path.GetFileName(path);  // o.name;*/
			//		var guid =   : AssetDatabase.GetAssetPath(_o[0].folder_object);
			//		var path = slot != null ? AssetDatabase.GUIDToAssetPath(slot.guids[0].guid): _o[0].path ;
			var path = _o[0].path;
			//var guid = slot != null ? slot.guids[0].guid : AssetDatabase.AssetPathToGUID(path);
			var name = Path.GetFileName(path);
			//de	Debug.Log(path);

			content.text = name;
			content.tooltip = "";
			if (_itemData.DRAW_FOLDERS_CONTENT && _itemData.IS_ROOT)
			{
				if (string.IsNullOrEmpty(ASSET_PATH(o))) content.text = "Error lo load: Missing guid";
				//	if (string.IsNullOrEmpty(ASSET_PATH(o[0]))) content.text = "Error lo load: " + (string.IsNullOrEmpty(o.project.assetPath) ? "Missing guid" : o.project.assetPath);
				// else content.text = "<i>" + o.project.assetFolder + "</i>/<b>" + content.text + "</b>";
				else
				{
					content.tooltip = ASSET_PATH(o); //o.project.assetFolder + '/' + content.text;
					content.text = "<i><b>" + content.text + "</b></i>";
				}
			}

			float labelWidth = CALC_LABEL(rect, content, FONT_SIZE);


			//** HIGHLIGHTER BG **////////////////////////
			//** HIGHLIGHTER BG **////////////////////////
			// if (!_itemData.IS_ROOT)
			if (o)
			{
				_itemData.higlightDrag = HL_DR == o.GetInstanceID();
				mouseEventStruct.Set_Get(_itemData.IS_ROOT ? mouseEventStruct.memoryRoot : null, new[] { o }, -1, _cData.catIndex, -1, CURRENT_CONTROLLER, _itemData.SELECTION_CURSOR_STATE);
				// _itemData.tempColor = adapter.MOI.M_Colors.needdrawGetColor(o);
				/*	tempDynamicRect.adapter = adapter;
					tempDynamicRect.labelSize = labelWidth;
					tempDynamicRect.Set(new Rect(rect.x + FOLD_WIDTH, rect.y, rect.width - FOLD_WIDTH + (HAS_DESCRIPTION ? _cData.fullCell.width / 3 : 0), rect.height),
						new Rect(_cData.fullCell.x + _cData.fullCell.width / 3 * 2, _cData.fullCell.y, _cData.fullCell.width, _cData.fullCell.height),
						false, o[0], true, 0);*/

				// _itemData.tempColor = adapter.ColorModule.DrawBackground(null, null, tempDynamicRect, o, 1, resetFonts: false );
			}
            //** HIGHLIGHTER BG **////////////////////////
            //** HIGHLIGHTER BG **////////////////////////


            //** SLECTION BG **////////////////////////
            if ( o )
            {
                if (!drawg1.HasValue
					&& adapter.par_e.BOOKMARKS_FOLDER_SHOW_SELECTIONS
					&& IsSelected(o.GetInstanceID()))
					adapter.SelectRect(_cData.fullCell, 1f);

				/*if (_itemData.tempColor != null && _itemData.tempColor.HAS_BG_COLOR)
				{   var oc = GUI.color;
					var c = _itemData.tempColor.BGCOLOR;
					//  c.a /= 2;
					c.a = (byte)(c.a / 1.2f);
					GUI.color *= c;
					GUI.DrawTexture( _cData.fullCell, Texture2D.whiteTexture );
					GUI.color = oc;
				}*/
			}
            //** SLECTION BG **////////////////////////


            //** FOLD **////////////////////////



            if ( o )
            {
                if (HAS_FOLD)
				{

					DO_FOLD(false, ref rect, null, category.slots[_itemData.memoryRootIndex], _itemData.IS_ROOT ? (int?)null : o.GetInstanceID());
				}
				else
				{
					rect.x += FOLD_WIDTH;
					rect.width -= FOLD_WIDTH;
				}
			}
			//** FOLD **////////////////////////


			//** ICON **////////////////////////
			{
				var drawOffset = rect;
				var oldH = drawOffset.height;
				drawOffset.height = adapter.par_e.BOOKMARKS_FOLDER_DEFAULT_ICON_SIZE;
				drawOffset.y += (oldH - drawOffset.height) / 2;
				drawOffset.width = drawOffset.height;

				//var iconTempColor = adapter.bottomInterface.GetContent(o);
				Texture2D icon = InternalEditorUtility.GetIconForFile(path);
				//Tools.GET_SCENE_BY_ID()
				//	Texture2D icon = AssetDatabase.GetCachedIcon(path) as Texture2D;// iconTempColor.add_icon;
				if ( IS_FOLDER(o)) icon = STAR; //_itemData.IS_ROOT &&  ||

				if (string.IsNullOrEmpty(name))
				{ /*  var ol = adapter.label.alignment;
                          var oc = adapter.label.normal.textColor;
                          adapter.label.alignment = TextAnchor.MiddleLeft;
                          adapter.label.normal.textColor = Color.red;
                          GUI.Label( rect, "Error load", adapter.label );
                          adapter.label.alignment = ol;
                          adapter.label.normal.textColor = oc;*/
				}
				else if (icon != null)
				{
					var c = GUI.color;
					if (iconTempColor.add_hasiconcolor) GUI.color *= iconTempColor.add_iconcolor;
					DrawTexture(drawOffset, icon);
					if (iconTempColor.add_hasiconcolor) GUI.color = c;
					//** COUNT **////////////////////////
					/*  if (m_drawCount)
					  {   drawOffset = rect;

						  adapter.bottomInterface.DRAW_COUNT( drawOffset, (int)LH, Adapter.BottomInterface.MemType.Custom, mouseEventStruct.memoryRoot, false );
					  }*/
				}
			}
			//** ICON **////////////////////////


			//** DRAG **////////////////////////
			{
				if (_itemData.higlightDrag && FAVOR_DRAG_VALIDATOR()) adapter.gl.DRAW_TAP_GLOW(_cData.fullCell, Colors.colorStatic);
			}
			//** DRAG **////////////////////////


			//** TEXT **////////////////////////
			//** TEXT **////////////////////////
			//** TEXT **////////////////////////

			rect.x += LH;
			rect.width -= LH;
			if (!string.IsNullOrEmpty(content.text))
			{
				if (_itemData.DRAW_FOLDERS_CONTENT && labelWidth > rect.width)
				{
					var c = rect;
					c.width = 10;
					Label(c, "...");
					c.x += c.width;
					c.width = rect.width - c.width;
					DRAW_LABEL(c, content, FONT_SIZE, _itemData.tempColor, TextAnchor.MiddleRight);
					rect = c;
				}
				else
				{
					rect.width = DRAW_LABEL_STYLE.CalcSize(content).x;
					DRAW_LABEL(rect, content, FONT_SIZE, _itemData.tempColor);
				}
			}
			//** TEXT **////////////////////////
			//** TEXT **////////////////////////
			//** TEXT **////////////////////////


			//** DRAG **////////////////////////
			if (_itemData.IS_ROOT)
			{
				DRAG_DRAG(rect, false, CURRENT_CONTROLLER, _cData.scene, ref refBookmarks, drawg1, drawg3, -1, ref refBookmarks, _cData.catIndex,
					_itemData.memoryRootIndex
					//	_cData.memoryRoot[_itemData.memoryRootIndex].ArrayIndex
					);
			}
			//** DRAG **////////////////////////





			//** TYPE BUTTON **////////////////////////
			//** TYPE BUTTON **////////////////////////
			//** TYPE BUTTON **////////////////////////
			if (HAS_TYPEBUTTON)
			{
				var ic = rect;
				ic.x += rect.width;
				var lit_icon = _itemData.FLAT_FOLDERS ? FAVORIT_LIST_ICON_ON : FAVORIT_LIST_ICON;
				ic.width = type_width - lit_icon.width;
				//ic.y += (rect.height - (EditorStyles.popup.normal.background ? EditorStyles.popup.normal.background.height : EditorStyles.popup.normal.scaledBackgrounds[0].height)) / 2;
				// ic.height = (EditorStyles.popup.normal.background ? EditorStyles.popup.normal.background.height : EditorStyles.popup.normal.scaledBackgrounds[0].height);
				if (Event.current.type == EventType.Repaint) EditorStyles.popup.Draw(ic, TypeButtonGUIContent, false, false, false, false);

				var nms = mouseEventStruct.Clone();
				nms.hierarchy_obect = new[] { o };
				nms.itemIndex = index;
				nms.categoryIndex = _cData.m_favString_index;
				nms.otherstring = refBookmarks[_cData.catIndex].slots[_itemData.memoryRootIndex];
				// nms.otherstring = refBookmarks[_cData.catIndex].InstanceID;
				nms.scene = _cData.scene;
				EVENT_EVENT_EVENT(ic, null, null, "FILTER_ON_DOWN", nms, CURRENT_CONTROLLER, CONTROL_ID++, true);

				if (Event.current.type == EventType.Repaint) Label(ic, cc2);

				ic.x += ic.width;
				ic.width = lit_icon.width;
				DrawTexture(ic, lit_icon);
				nms.categoryIndex = _cData.catIndex;
				nms.itemIndex = _itemData.memoryRootIndex;
				EVENT_EVENT_EVENT(ic, null, null, "LIST_ON_DOWN", nms, CURRENT_CONTROLLER, CONTROL_ID++, true);
				if (Event.current.type == EventType.Repaint) Label(ic, cc3);
			}
			//** TYPE BUTTON **////////////////////////
			//** TYPE BUTTON **////////////////////////
			//** TYPE BUTTON **////////////////////////


			if (HAS_DESCRIPTION) rect.width -= _cData.fullCell.width / 3;

			rect.x += rect.width;

			//** DESCRIPTION **////////////////////////
			//** DESCRIPTION **////////////////////////
			//** DESCRIPTION **////////////////////////
			//style.Draw( drawOffset, content, active, active, false, active );


			if (HAS_DESCRIPTION && slot != null)
			{
				rect.width = _cData.fullCell.width / 3;
				rect.x = _cData.fullCell.x + _cData.fullCell.width / 3 * 2;

				if (mouseEventStruct.memoryRoot != null) content_des = GET_DESCRIPTION(mouseEventStruct.memoryRoot);
				else content_des = GET_DESCRIPTION(slot);

				if (Event.current.type == EventType.Repaint) //var c = GUI.color;
				{ // GUI.color *= adapter.bottomInterface.coloAlpha* adapter.bottomInterface.coloAlpha * adapter.bottomInterface.coloAlpha * adapter.bottomInterface.coloAlpha;
				  //GUI.DrawTexture(new Rect(rect.x, rect.y, 2, rect.height), SEPARATOR);
					/* GUI.DrawTexture( new Rect( cell.x, cell.y, 1, cell.height ), adapter.GetIcon())
					 EditorGUI.DrawRect( new Rect( cell.x, cell.y, 1, cell.height ),  adapter.HR_COLOR );*/
					//GUI.color = c;
				}

				rect.x += 2;
				rect.width -= 2;
				rect.width -= rect.height;
				//	if (Event.current.type == EventType.Repaint)
				//PluginInstance.STYLE_DEFBOX.Draw(rect, "", false, false, false, false);

				DRAW_LABEL(rect, content_des, adapter.FONT_8(), align: TextAnchor.MiddleRight);

			}

			if (HAS_DESCRIPTION && slot != null)
			{
				rect.x += rect.width;
				rect.width = rect.height;
				DrawTexture(rect, FAVORIT_FILTER_ICON);
				if (Event.current.type == EventType.Repaint) Label(rect, desctool);

				mouseEventStruct.MousePosStruct = new MousePos(Event.current.mousePosition, MousePos.Type.Input_190_68, false, adapter);
				EVENT_EVENT_EVENT(rect, null, null, "DESCRIPTION_ON_UP", mouseEventStruct, CURRENT_CONTROLLER, CONTROL_ID++, true);
				EVENT_EVENT_EVENT(rect, null, null, "DESCRIPTION_ON_UP_RIGHT", mouseEventStruct, CURRENT_CONTROLLER, CONTROL_ID++, true, 1);
			}
			//** DESCRIPTION **////////////////////////
			//** DESCRIPTION **////////////////////////
			//** DESCRIPTION **////////////////////////


			//  mouseEventStruct.memoryRoot = _cData.memoryRoot[ _itemData.memoryRootIndex];

			mouseEventStruct.LocalModuleRect = _cData.fullCell;
			EVENT_EVENT_EVENT(_cData.fullCell, null, "ITEM_ON_DRAG", "ITEM_ON_DOWN", mouseEventStruct, CURRENT_CONTROLLER, CONTROL_ID++, true);
			mouseEventStruct.categoryIndex = _cData.catIndex;
			mouseEventStruct.itemIndex = _itemData.memoryRootIndex;
			EVENT_EVENT_EVENT(_cData.fullCell, null, null, "MENUCALL", mouseEventStruct, CURRENT_CONTROLLER, CONTROL_ID++, true, 1);
		}








		int event_interator;
		const int FOLD_WIDTH = 14;

		void DO_FOLD(bool fullRect, ref Rect cell, FolderBookmark category, BookSlot slot, int? instanceID)
		{
			//var item = GET_TREE_ITEM(id, category, controller, IS_ROOT);

			var drawCell = cell;
			if (!fullRect) drawCell.width = FOLD_WIDTH;
			cell.x += FOLD_WIDTH;
			cell.width -= FOLD_WIDTH;

			bool expanded = false;
			if (category != null)
			{
				expanded = category.expanded;
			}
			else if (instanceID.HasValue)
			{
				expanded = slot.GetExpandInstanceID(instanceID.Value);
			}
			else
			{
				expanded = slot.slot_expanded;
			}

			if (drawCell.Contains(Event.current.mousePosition) && Event.current.button == 0 && Event.current.type == EventType.MouseDown)
			{
				Tools.EventUse();
				CURRENT_CONTROLLER.ClearAction();
				actions.ADD_ACTION(CONTROL_ID, drawCell, contains => { return false; }, contains =>
				{
					if (contains)
					{



						if (category != null)
						{
							category.expanded = !category.expanded;
						}
						else if (instanceID.HasValue)
						{
							slot.SwitchExpandInstanceID(instanceID.Value);
						}
						else
						{
							slot.slot_expanded = !slot.slot_expanded;
						}
						//adapter.PUSH_UPDATE_ONESHOT( 1, () => {
						//	//Debug.Log( "ASD" );
						//	CURRENT_CONTROLLER.RepaintNow();
						//} );
						//COLABSE_CACHE.Set(item.Expand, controller.MAIN ? 0 : 1, category, id, IS_ROOT);

						/*if (controller.MAIN)
						{   if (category.ColabsedItems == null) category.ColabsedItems = new List<long>();
							if (item.Expand) category.ColabsedItems.RemoveAll( i => i == id );
							else category.ColabsedItems.Add( id );
						}
						else
						{   if (category.Windows_ColabsedItems == null) category.Windows_ColabsedItems = new List<long>();
							if (item.Expand) category.Windows_ColabsedItems.RemoveAll( i => i == id );
							else category.Windows_ColabsedItems.Add( id );
						}
						// ClearHeight();
						SET_DIRTY( scene );*/
						repaint();
					}
				}, CURRENT_CONTROLLER);
			}

			var hover = Event.current.type == EventType.Repaint && actions.HOVER(CONTROL_ID, null, CURRENT_CONTROLLER);
			if (Event.current.type == EventType.Repaint)
			{
				var dif = drawCell.height - (EditorStyles.foldout.normal.background ? EditorStyles.foldout.normal.background.height : EditorStyles.foldout.normal.scaledBackgrounds[0].height);
				drawCell.y += dif / 2 - 1;
				drawCell.height = (EditorStyles.foldout.normal.background ? EditorStyles.foldout.normal.background.height : EditorStyles.foldout.normal.scaledBackgrounds[0].height);
				EditorStyles.foldout.Draw(drawCell, REALEMPTY_CONTENT, hover, hover, expanded, false);
			}

			CONTROL_ID++;

		}
















		folderClass[] asdd = new folderClass[1];
		folderClass[] rdy_asd = new folderClass[1] { new folderClass() };
		void DRAW_FOLDERS(ref DRAW_CATEGORY_DATA _cData, ref DRAW_ROOTITEM_DATA _itemData, folderClass[] o, int DEEP)
		{
			var paths = GET_PATHS(ASSET_PATH(o[0].folder_object)).scanneditems;
			bool hasFilter = _itemData.ExtenstionFilter != null;
			var filter = _itemData.ExtenstionFilter;
			var flat = _itemData.FLAT_FOLDERS;

			//int deep = 0;
			deeppath[0] = -1;
			collabse[0] = null;
			//  collabse = "";
			//int dropL = -1;

			for (int i = 0; i < paths.Count; i++)
			{
				if (paths[i].extension == "") continue;

				if (hasFilter && paths[i].extension != filter) continue;


				if (!flat)
				{
					for (int D_D = 0; D_D < paths[i].DEEP; D_D++)
					{
						var d = D_D;
						if (collabse[d] == null)
						{
							collabse[d] = false;
							collabse[d + 1] = null;
						}

						if (deeppath[d] == -1)
						{
							deeppath[d] = -2;
							deeppath[d + 1] = -1;
						}

						if (deeppath[d] != paths[i].folders[d].GetHashCode() )
						{
							deeppath[d] = paths[i].folders[d].GetHashCode();
							//  DRAW_SINGLE_ITEM( paths[i].GetFoldByIndex( d ), d + m_DEEP,  true ); //draw fold
							asdd[0] = paths[i].GetFoldByIndex(d);
							if ( asdd[ 0 ] == null ) break;
							OBJECT_ITEM_GUI(ref _cData, _itemData.CopyNonRoot(), asdd, d + DEEP);


							//var cat = _cData.memoryRoot[_cData.catIndex];
							if (!_cData.memoryRoot[_cData.catIndex].slots[_itemData.memoryRootIndex].GetExpandInstanceID(asdd[0].folder_object.GetInstanceID()))
							//if (!asdd[0].folder_expanded) // collabse[d] = paths[i].folders[d];
							//		if (!GET_TREE_ITEM(new_o.project.guid, _cData.catIndex, _cData.controller, false).Expand) // collabse[d] = paths[i].folders[d];
							{
								collabse[d] = true;
								goto __CONT;
							}
							else
							{
								collabse[d] = false;
							}
						}

						if (collabse[d].Value) goto __CONT;
					}
				}


				//asdd[0] = 
				rdy_asd[0].folder_object = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(paths[i].fullPath);
				rdy_asd[0].path = paths[i].fullPath;
				if (!rdy_asd[0].folder_object)
				{
					//Debug.LogWarning("file not found " + paths[i].fullPath);
					goto __CONT;
				}
				//rdy_asd[0].folder_expanded = true;
				//var guid = AssetDatabase.AssetPathToGUID(paths[i].fullPath);
				//paths[i].CurrentObject = adapter.GetHierarchyObjectByGUID(ref guid, "");

				OBJECT_ITEM_GUI(ref _cData, _itemData.CopyNonRoot(), rdy_asd, flat ? DEEP : (paths[i].DEEP + DEEP));


			/* if (null == paths[i].CurrentObject)
			 {   var guid = AssetDatabase.AssetPathToGUID( paths[i].fullPath);
				 paths[i].CurrentObject = adapter.GetHierarchyObjectByGUID( ref guid, "" );
			 }
			 //if (!paths[i].CurrentObject.project.IsFolder)
			 DRAW_SINGLE_ITEM( paths[i].CurrentObject, flat ? m_DEEP : (paths[i].DEEP + m_DEEP),  false ); //draw item*/

			//  Debug.Log(paths[i].fullPath);
			__CONT:;
			}
		}



	}
}
