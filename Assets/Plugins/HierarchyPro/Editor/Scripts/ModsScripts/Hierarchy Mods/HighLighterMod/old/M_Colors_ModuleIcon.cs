using System;
using UnityEngine;
using UnityEditor;
using EMX.HierarchyPlugin.Editor.Windows;

namespace EMX.HierarchyPlugin.Editor.Mods
{
	internal partial class HighlighterMod : DrawStackAdapter, IModSaver
	{












		Color32 S_COL1 = new Color32(8, 8, 8, 50);
		Color32 S_COL2 = new Color32(132, 132, 132, 50);

		Color32 S_COL3 = new Color32(8, 8, 8, 20);
		Color32 S_COL4 = new Color32(255, 255, 255, 50);



		Rect ButtonRect(Rect selectionRect, HierarchyObject o)
		{
			//var icon_rect = HighlighterGetRect.GetIconRect(selectionRect, false, IconPlace);

			switch (set.HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION)
			{
				case 1:
				case 2:
					{
						var left_rect = selectionRect;
						left_rect.x = 0;
						if (UnityVersion.UNITY_CURRENT_VERSION >= UnityVersion.UNITY_2019_2_0_VERSION)
							//if (set.HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION < 3)
							left_rect.x += adapter.ha.LEFT_PADDING;

						left_rect.width = selectionRect.x - left_rect.x;
						if (o.ChildCount() > 0) left_rect.width -= foldoutStyleWidth;
						if (set.HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION == 1) left_rect.width = Mathf.Min(left_rect.height, left_rect.width);

						return left_rect;
					}
				case 3:
					{
						//GetIconRectIfNextToLabel(selectionRect, GetIconRectIfNextToLabelType.CustomIcon, size);
						var label_rect = GetIconRectIfNextToLabel(selectionRect, GetIconRectIfNextToLabelType.CustomIcon, DEFAULT_ICON_SIZE);
						return label_rect;
					}
			}
			throw new Exception("ButtonRect");

			//internal static Rect GetIconRectIfNextToLabel(Rect selectionRect, GetIconRectIfNextToLabelType type)

		}
		internal Rect HoverFullRect(Rect selectionRect)
		{
			//internal Rect GetIconRect(Rect selectionRect, int? overrideValue , int? overrideSBGIconPlace, float size)
			var icon_rect = GetIconRect(selectionRect, 0, 0, DEFAULT_ICON_SIZE);
			//  var size = selectionRect.height / 8 * 2;
			if (UnityVersion.UNITY_CURRENT_VERSION == UnityVersion.UNITY_2019_2_0_VERSION) icon_rect.x += adapter.ha.LEFT_PADDING;
			return icon_rect;
		}
		internal Rect HoverRectNew(Rect selectionRect, HierarchyObject o)
		{
			var icon_rect = ButtonRect(selectionRect, o);
			icon_rect.width = icon_rect.height;


			//var size = EditorGUIUtility.singleLineHeight / 4;
			var size = set.HIGHLIGHTER_HIERARCHY_BUTTON_RECTMARKER_SIZE;
			if (set.HIGHLIGHTER_HIERARCHY_BUTTON_LOCATION < 3) size -= 1;

			//  var size = EditorGUIUtility.singleLineHeight / 2.5f;
			//  var size = parLINE_HEIGHT / 1.5f;
			//if (IconPlace != 0)
			icon_rect.x += (icon_rect.width - size) / 2;

			icon_rect.y += (icon_rect.height - size) / 2;
			icon_rect.width = icon_rect.height = size;
			return icon_rect;
		}
		internal Rect HoverRect(HierarchyObject _o, Rect selectionRect, int IconPlace, int? overrideBgIconPlace = null)
		{
			var icon_rect = GetIconRect(selectionRect, IconPlace == 0 /*|| _S_bgButtonForIconsPlace == 2*/ ? 2 : 0, overrideBgIconPlace, DEFAULT_ICON_SIZE); //false, this,

			//  var size = selectionRect.height / 8 * 2;
			if (IconPlace == 0)
			{
				if (UnityVersion.UNITY_CURRENT_VERSION == UnityVersion.UNITY_2019_2_0_VERSION)
				{
					icon_rect.x += adapter.ha.LEFT_PADDING;
				}
			}

			if (lastIconPlace == 2 || lastIconPlace == 0)
			{
				/*if (SETACTIVE_POSITION == 1 && ENABLE_ACTIVEGMAOBJECTMODULE)
				{
					if ((_o.parent(this) != null || _o.ChildCount(this) == 0))
						icon_rect.x += 8;
					else
						icon_rect.x += 4;
				}*/
				icon_rect.x += 4;
				icon_rect.x += 2;
			}


			var size = EditorGUIUtility.singleLineHeight / 4;

			if (lastIconPlace == 0) size -= 1;

			//  var size = EditorGUIUtility.singleLineHeight / 2.5f;
			//  var size = parLINE_HEIGHT / 1.5f;
			if (IconPlace != 0)
				icon_rect.x += (icon_rect.width - size) / 2;

			icon_rect.y += (icon_rect.height - size) / 2;
			icon_rect.width = icon_rect.height = size;
			return icon_rect;
		}
		//internal int hoverID;

















		internal void Draw(Rect selectionRect, HierarchyObject _o)
		{   //if (OPT_EV_BR(Event.current)) return 0;

			//if (!ICON_STACK.START_DRAW(selectionRect, _o, momentumGl)) return;
			var res = ICON_STACK.START_DRAW_PARTLY_TRYDRAW(selectionRect, _o);

			if (!res) return;

			if (!_o.Validate())// ||  adapter.ha.IS_PREFAB_MOD_OPENED()
			{
				//ICON_STACK.END_DRAW(_o);
				return;
			}


			ICON_STACK.START_DRAW_PARTLY_CREATEINSTANCE(selectionRect, _o, Event.current.type == EventType.Repaint, momentumGl);

			if (callFromExternal() || !_o.ah.internalIcon) _o.ah.drawIcon = GET_CONTENT(_o);

			_o.ah.internalIcon = false;
			var labelPlace = _o.ah.switchType == 1;
			var icon_place = labelPlace ? 0 : _S_bgIconsPlace;

			if (callFromExternal()) icon_place = 2;

			var buttonRectLabel = selectionRect;
			buttonRectLabel.width = EditorGUIUtility.singleLineHeight;
			var buttonRectLeft = selectionRect;


			buttonRectLeft.width = selectionRect.x - foldoutStyleWidth;

			if (_o.ChildCount() == 0) buttonRectLeft.width += foldoutStyleWidth / 2;

			var left_padding = raw_old_leftpadding;



			buttonRectLeft.x = left_padding;
			// EditorGUI.DrawRect( buttonRectLeft, Color.white );

			if (UnityVersion.UNITY_CURRENT_VERSION >= UnityVersion.UNITY_2019_2_0_VERSION) buttonRectLeft.width -= left_padding;

			var icon_rect = labelPlace ? GetIconRect(selectionRect, null, overrideSBGIconPlace: 0, size: DEFAULT_ICON_SIZE) : GetIconRect(selectionRect, null, null, size: DEFAULT_ICON_SIZE);

			//EditorGUIUtility.SetIconSize(Vector2.zero);


			bool auto = _o.ah.switchType > 0;//|| _o.ah.cache_prefab;
											 // float __IS = COLOR_ICON_SIZE;
											 //float __IS = COLOR_ICON_SIZE - (DEFAULT_ICON_SIZE - EditorGUIUtility.singleLineHeight) / 2;
											 //// if  (__o. cache_prefab )__IS = DEFAULT_ICON_SIZE;
											 //if (_o.ah.switchType == 1) __IS = DEFAULT_ICON_SIZE;
											 //if (_o.ah.switchType == 2) __IS = EditorGUIUtility.singleLineHeight;
											 //var ICON_SIZE = Mathf.CeilToInt(__IS - EditorGUIUtility.singleLineHeight);

			//var ICON_SIZE = DEFAULT_ICON_SIZE;
			//
			//if (icon_place != 2) icon_rect.x -= Mathf.CeilToInt(ICON_SIZE / 2f);
			//
			//icon_rect.y -= ICON_SIZE / 2;
			//icon_rect.width += ICON_SIZE;
			//icon_rect.height += ICON_SIZE;


			/* if ( _S_bgIconsPlace == 1 )
			 {   var centerX = icon_rect.x + icon_rect.width / 2;
			     var centerY = icon_rect.y + icon_rect.height / 2;
			     buttonRectLeft.x = centerX - buttonRectLeft.width / 2 ;
			     buttonRectLeft.y = centerY - buttonRectLeft.height / 2;
			 }*/



			if (!callFromExternal()  /* && adapter.IsSelected(__o.id)*/)
			{




				//GUI.DrawTexture(label_icon_rect, Texture2D.whiteTexture, ScaleMode.ScaleToFit, true, 0.0f, active ? Color.white : inactiveColor, 0.0f, 0.0f);
				// if ( HighlighterHasKey(__o.scene, __o))
				if (_o.ah.BACKGROUNDED != 0 && _o.ah.switchType != 1)
				{
					//Debug.Log("BACKGROUNDED= " + _o.ah.BACKGROUNDED + " ASD");

					var label_icon_rect = GetIconRectIfNextToLabel(selectionRect, GetIconRectIfNextToLabelType.DefaultIcon, DEFAULT_ICON_SIZE);
					var treeItem = !adapter.ha.IS_SEARCH_MOD_OPENED() && !callFromExternal() ? _o.GetTreeItem() : null;
					var active = _o.Active();

					if ((!_o.ah.drawIcon.add_icon || _S_bgIconsPlace != 0))
					{

						/*  var  label_icon_rect = GetIconRectIfNextToLabel(selectionRect, GetIconRectIfNextToLabelType.DefaultIcon);
						  var treeItem = __o.GetTreeItem(adapter);
						  var active = __o.Active();*/
						//  EditorUtility.GetIconInActiveState( this.GetIconForItem( item ) );
						var targetIcon = treeItem != null ? treeItem.icon : null;
						var skipoverlay = !targetIcon;

						//if ( !targetIcon && HAS_LABEL_ICON()  ) targetIcon = (Texture2D)Utilities.__internal_ObjectContent(adapter, __o.GetHardLoadObject(), __o.GET_TYPE(adapter)).add_icon;
						if (!targetIcon)
						{

							/* MethodInfo gi = null;
							 if ( gi == null )
							 {   var ty = typeof(EditorGUIUtility);
							     gi = ty.GetMethod( "GetIconForObject",  (BindingFlags) (-1));
							
							 }
							 targetIcon = gi.Invoke( null, new object[1] { __o.GetHardLoadObject() } ) as Texture2D;*/
							if (IS_PROJECT())
							{
								targetIcon = EditorGUIUtility.ObjectContent(_o.GetHardLoadObjectSlow(), _o.GET_TYPE()).image as Texture2D;

							}

							else
							{
								var loadObject = _o.go;

								if (Prefabs.FindPrefabRoot(loadObject) != loadObject) loadObject = null;

								targetIcon = EditorGUIUtility.ObjectContent(loadObject, _o.GET_TYPE()).image as Texture2D;

								if (_o.ah.drawIcon.add_icon && _o.ah.drawIcon.add_icon == targetIcon)
								{
									HighlighterCache_Icons.____SetIconOnlyInternal(_o, null);
									targetIcon = EditorGUIUtility.ObjectContent(loadObject, _o.GET_TYPE()).image as Texture2D;
									HighlighterCache_Icons.____SetIconOnlyInternal(_o, _o.ah.drawIcon.add_icon as Texture2D);
								}
							}

						}

						if (targetIcon && active)
						{



							 if ( adapter.HL_SET_G(pluginID).HIGHLIGHTER_DRAW_ICONS_SHADOW )
							{
								var S = 4;
								var R = label_icon_rect;
								R.x -= S;
								R.y -= S;
								R.width += S * 2;
								R.height += S * 2;


								//  Adapter.DrawTexture( R, adapter.GetIcon( "HIPERUI_BUTTONGLOW" ), Color.black );
								ICON_STACK.Draw_GUITexture(R, Icons.GetIconDataFromTexture(adapter.GetExternalModOld("HIPERUI_BUTTONGLOW")), Color.black, true);
							}

							if (!_o.Active())
							{   /*var oldC = GUI.color;
								GUI.color *= inactiveColor;
								
								Adapter.DrawTexture( label_icon_rect, targetIcon, active ? Color.white : inactiveColor );
								GUI.color = oldC;*/
								ICON_STACK.Draw_GUITexture(label_icon_rect, Icons.GetIconDataFromTexture(targetIcon), active ? Color.white : inactiveColor, false);
							}

							else
							{   // Adapter.DrawTexture( label_icon_rect, targetIcon, active ? Color.white : inactiveColor );
								ICON_STACK.Draw_GUITexture(label_icon_rect, Icons.GetIconDataFromTexture(targetIcon), active ? Color.white : inactiveColor, false);
							}



							if (!skipoverlay)
							{

								overlayButStr.treeItem = treeItem;
								overlayButStr.active = active;
								ICON_STACK.Draw_Action(label_icon_rect, ICON_OVERLAY_ACTION_HASH, overlayButStr);
							}
						}
					}

					if (treeItem != null)
					{


						//LabelOverlayGUI( selectionRect, treeItem );
						overlayButStr.treeItem = treeItem;
						ICON_STACK.Draw_Action(selectionRect, LABEL_OVERLAY_ACTION_HASH, overlayButStr);

						if (  /*treeItem.hasChildren &&*/ active && (_o.ah.BACKGROUNDED != 2 || (_o.ah.FLAGS & 1) != 1) && _o.ChildCount() != 0 && !adapter.ha.IS_SEARCH_MOD_OPENED()
							&& adapter.HL_SET.DO_FOLD_INVERSION)
						{


							/*   if ( Adapter.UNITY_CURRENT_VERSION < Adapter.UNITY_2019_3_0_VERSION )
							   {   label_icon_rect.x -= foldoutStyleWidth + 1;
							       label_icon_rect.width = foldoutStyle.fixedWidth;
							       var d = label_icon_rect.height - foldoutStyleWidth;
							       label_icon_rect.y += d / 2 - 1;
							       / *   if ( adapter.USE_LABEL_OFFSET )* /
							       label_icon_rect.y -= 3;
							       label_icon_rect.y = Mathf.FloorToInt( label_icon_rect.y );
							       label_icon_rect.height = foldoutStyle.fixedWidth;
							   }
							   else*/
							{
								label_icon_rect.x = label_icon_rect.x + (label_icon_rect.width - DEFAULT_ICON_SIZE) / 2;
								label_icon_rect.x -= foldoutStyleWidth + 1;
								label_icon_rect.width = foldoutStyle.fixedWidth;
								var d = label_icon_rect.height - foldoutStyleHeight;
								label_icon_rect.y += d / 2;
								//label_icon_rect.y -= EditorGUIUtility.singleLineHeight / 2;
								label_icon_rect.y = Mathf.FloorToInt(label_icon_rect.y);
								label_icon_rect.height = foldoutStyleHeight;
							}

							//EditorGUI.DrawRect(label_icon_rect, Color.white);

							overlayButStr.treeItem = treeItem;
							ICON_STACK.Draw_Action(label_icon_rect, FOLD_ACTION_HASH, overlayButStr);
							//ICON_STACK.Draw_GUITexture(label_icon_rect, Color.white);


						}
					}


				}

			}



			/* var defaultX = icon_rect.x;
			 var defaultY = icon_rect.y;
			 var defaultHeight = icon_rect.height;
			 var HHH = Mathf.Min(EditorGUIUtility.singleLineHeight, icon_rect.height);
			 var WWW = icon_rect.x - HHH;*/

			// icon_rect.x -= COLOR_ICON_SIZE + 20;

			/* var  buttonRect = icon_rect;
			 buttonRect.height -= 2;
			 if (!adapter.IS_PROJECT())
			 {   buttonRect.x = 0;
			     buttonRect.width = WWW;
			 }*/

			/*  icon_rect.width = icon_rect.height = COLOR_ICON_SIZE;
			  icon_rect.x += (icon_rect.width - icon_rect.width) / 2 + (COLOR_ICON_SIZE - 12) / 2f;
			  icon_rect.y += (icon_rect.height - icon_rect.height) / 2;*/

			/* standrardRect.width = standrardRect.height = EditorGUIUtility.singleLineHeight;
			 standrardRect.x += (oldW - standrardRect.width) / 2 + (EditorGUIUtility.singleLineHeight - 12) / 2f;
			 standrardRect.y += (oldH - standrardRect.height) / 2;*/










			if (_o.ah.drawIcon.add_icon)
			{




				//if (__o.switchType == 0)  Debug.Log(__o.ah.drawIcon.add_hasiconcolor);
				var COLOR = Color.white;

				if (_o.ah.switchType == 0 && _o.ah.drawIcon.add_hasiconcolor)
				{   //backCol = GUI.color;
					/*if (!adapter.DISABLE_DES() && IconColorCacher.HasKey( __o.scene, __o ))
					{   var get = IconColorCacher.GetValue(__o.scene, __o );
					    tc.r = get.list[0] / 255f;
					    tc.g = get.list[1] / 255f;
					    tc.b = get.list[2] / 255f;
					    tc.a = Mathf.Clamp01( get.list[3] / 255f  );
					    GUI.color *= tc;
					}*/
					COLOR *= _o.ah.drawIcon.add_iconcolor;
					// if ( IconImageCacher.HasKey( __o.scene , __o ) ) Debug.Log( GUI.color );
				}


				/*  if (!__o.Active())
				  {   var oldC = GUI.color;
				      GUI.color *= inactiveColor;
				      GUI.DrawTexture( icon_rect, drawIcon, ScaleMode.ScaleToFit);
				      GUI.color = oldC;
				  }
				  else
				  {   GUI.DrawTexture( icon_rect, drawIcon, ScaleMode.ScaleToFit);
				      {
				  }*/
				/*if ( adapter.SETACTIVE_POSITION == 1 && Adapter.UNITY_CURRENT_VERSION < Adapter.UNITY_2019_VERSION
				        && adapter.par.ENABLE_ACTIVEGMAOBJECTMODULE && _S_bgIconsPlace == 2 ) icon_rect.x += EModules.EModulesInternal.Hierarchy.M_SetActive.ONE_POS_IW_FORCOLOR;
				    */
				var c = (_o.ah.switchType == 2 || _o.Active() ? Color.white : inactiveColor) * COLOR;

				if (_o.ah.switchType == 1)
				{
					var dw = (icon_rect.width - 7) / 2;
					var dh = (icon_rect.height - 7) / 2;
					// var d = (icon_rect.width - adapter.parLINE_HEIGHT) / 2;
					//   Adapter.DrawTexture( new Rect( icon_rect.x + dw, icon_rect.y + dh, 7, 7 ), _o.ah.drawIcon.add_icon, _o.switchType == 2 || _o.Active() ? Color.white : inactiveColor );
					ICON_STACK.Draw_GUITexture(new Rect(icon_rect.x + dw, icon_rect.y + dh, 7, 7), Icons.GetIconDataFromTexture((Texture2D)_o.ah.drawIcon.add_icon), c, false);
				}

				else
				{   //Adapter.DrawTexture( icon_rect, _o.ah.drawIcon.add_icon, _o.switchType == 2 || _o.Active() ? Color.white : inactiveColor );
					ICON_STACK.Draw_GUITexture(icon_rect, Icons.GetIconDataFromTexture((Texture2D)_o.ah.drawIcon.add_icon), c, false);

					if (_S_bgIconsPlace == 2)
					{
						ICON_STACK.Draw_Action(icon_rect, SKIP_CHILD_COUNT_ACTION_HASH, null);
					}
				}

				if (!adapter.ha.IS_SEARCH_MOD_OPENED() && !callFromExternal())
				{   //IconOverlayGUI( icon_rect, _o.GetTreeItem( adapter ) );
					overlayButStr.treeItem = _o.GetTreeItem();
					ICON_STACK.Draw_Action(icon_rect, SECOND_ICON_OVERLAY_ACTION_HASH, overlayButStr);
				}

				if (!auto && /*adapter.IS_PROJECT() &&*/ set.HIGHLIGHTER_DRAW_ICON_IF_CUSTOM_ASIGNED)
				{   //Adapter.DrawTexture( icon_rect, adapter.GetIcon( "FOLDER_STARMARK" ), Color.white );
					ICON_STACK.Draw_GUITexture(icon_rect, Icons.GetIconDataFromTexture(adapter.GetExternalModOld("FOLDER_STARMARK")), Color.white, true);

				}

				/*   if (!o.activeInHierarchy)
				   {
				       Hierarchy.FadeRect(drawRect, 0.7f);
				   }*/
				if (_o.ah.switchType == 0 && _o.ah.drawIcon.add_hasiconcolor)
				{   // GUI.color = backCol;
				}


				switch (_o.ah.switchType)
				{
					case 0: switchedConten.tooltip = !_o.ah.drawIcon.add_icon ? "" : _o.ah.drawIcon.add_icon.name; break;

					case 1: switchedConten.tooltip = "" /*contentNull.tooltip*/; break;

					case 2: switchedConten.tooltip = contentMis.tooltip; break;
				}
			}

			else
				if (!IS_PROJECT())
			{   /* tRr = oldRect;
					     tRr.width -= 10;
					     tRr.y += (tRr.height - tRr.width - 4) / 2;
					     tRr.height = tRr.width;
					     tRr.y += 2;
					     if (!ICON_LEFT) tRr.x += COLOR_ICON_SIZE / 2 - tRr.width / 2;
					     var c = GUI.color;
					     GUI.color = alpha;
					     GUI.DrawTexture( tRr, adapter.GetIcon( "UNLOCK" ) );
					     GUI.color = c;*/

				//  if ( __o.name == "Directional Light (6)" ) Debug.Log( "ASD" );
				// CHeckIcon( __o ,)
				switchedConten.tooltip = content.tooltip;

			}





			// content.tooltip = base.ContextHelper;
			// oldRect.width = Math.Max(16,par.COLOR_ICON_SIZE);
			/*if (switchedConten.image) {
			     switchedConten.image = null;
			     GUI.DrawTexture( oldRect, switchedConten.image );
			   }*/






			//var DIF = 2;
			//buttonRectLeft.y += DIF;
			//buttonRectLeft.height -= DIF * 2;
			//
			//if (_S_bgButtonForIconsPlace == 0 || _S_bgButtonForIconsPlace == 2)
			//{ 
			//	colButStr.localSelectionRect = ICON_STACK.ConvertToLocal(selectionRect);
			//	ICON_STACK.Draw_SimpleButton(buttonRectLeft, switchedConten, BUTTON_ACTION_HASH, colButStr, true);
			//}
			//if (_S_bgButtonForIconsPlace == 1 || _S_bgButtonForIconsPlace == 2)
			//{
			//	var fixedRect = HoverFullRect(selectionRect);
			//	buttonRectLabel.x = fixedRect.x;
			//	buttonRectLabel.width = fixedRect.width - 2;
			//	colButStr.localSelectionRect = ICON_STACK.ConvertToLocal(selectionRect);
			//	ICON_STACK.Draw_SimpleButton(buttonRectLabel, switchedConten, BUTTON_ACTION_HASH, colButStr, true);
			//}









			//EditorGUI.DrawRect( buttonRectLeft, Color.white );
			//EditorGUI.DrawRect( buttonRectLabel, Color.white );



			/* if ( but && )
			 {
			 }*/

			ICON_STACK.END_DRAW(_o, -1);
		}



		DrawStackMethodsWrapper __FOLD_ACTION_HASH = null;
		DrawStackMethodsWrapper FOLD_ACTION_HASH { get { return __FOLD_ACTION_HASH ?? (__FOLD_ACTION_HASH = new DrawStackMethodsWrapper(FOLD_ACTION)); } }
		void FOLD_ACTION(Rect worldOffset, Rect label_icon_rect, DrawStackMethodsWrapperData data, HierarchyObject _o)
		{

			var overlayButStr = (OverlayButtonStr)data.args;
			var treeItem = overlayButStr.treeItem;
			//label_icon_rect.x += worldOffset.x;
			//label_icon_rect.y += worldOffset.y;

			if (EditorGUIUtility.isProSkin)
			{


				var c = GUI.color;
				var CCC = _o.ah.BACKGROUNDEsourceBgColorD;
				var l = (CCC.r + CCC.g + CCC.b);
				CCC.g = CCC.r = CCC.b = 1 - l / 2;
				CCC.a = 1;
				var expandedState = DoFoldout(label_icon_rect, treeItem, _o.id);

				/* GUI.color = Color.black ;
				 if (Event.current.type == EventType.Repaint) foldoutStyle.Draw(new Rect(label_icon_rect.x - 1, label_icon_rect.y, label_icon_rect.width, label_icon_rect.height)
				             , GUIContent.none, 0,
				             expandedState);
				 if (Event.current.type == EventType.Repaint) foldoutStyle.Draw(new Rect(label_icon_rect.x + 1, label_icon_rect.y, label_icon_rect.width, label_icon_rect.height)
				             , GUIContent.none, 0, expandedState);*/
				//if (!label_icon_rect.Contains( Event.current.mousePosition ) || Event.current.keyCode != KeyCode.Mouse0)
				GUI.color *= CCC;

				//EditorGUI.DrawRect(label_icon_rect, Color.white);
				//if ( Event.current.type == EventType.Repaint ) foldoutStyle.Draw( label_icon_rect,
				//	        GUIContent.none, false, label_icon_rect.Contains( Event.current.mousePosition ) && Event.current.button == 0, expandedState, false );


				GUI.Toggle(label_icon_rect, expandedState, GUIContent.none, foldoutStyle);

				GUI.color = c;
			}

			else
			{
				var expandedState = DoFoldout(label_icon_rect, treeItem, _o.id);
				GUI.Toggle(label_icon_rect, expandedState, GUIContent.none, foldoutStyle);

			}
		}

		DrawStackMethodsWrapper __SKIP_CHILD_COUNT_ACTION_HASH = null;
		DrawStackMethodsWrapper SKIP_CHILD_COUNT_ACTION_HASH { get { return __SKIP_CHILD_COUNT_ACTION_HASH ?? (__SKIP_CHILD_COUNT_ACTION_HASH = new DrawStackMethodsWrapper(SKIP_CHILD_COUNT_ACTION)); } }
		void SKIP_CHILD_COUNT_ACTION(Rect worldOffset, Rect label_icon_rect, DrawStackMethodsWrapperData data, HierarchyObject _o)
		{
			//adapter.SKIP_CHILDCOUNT = true;
		}



		internal struct OverlayButtonStr
		{
			internal UnityEditor.IMGUI.Controls.TreeViewItem treeItem;
			internal bool active;
		}
		OverlayButtonStr overlayButStr;
		DrawStackMethodsWrapper __ICON_OVERLAY_ACTION_HASH = null;
		DrawStackMethodsWrapper ICON_OVERLAY_ACTION_HASH { get { return __ICON_OVERLAY_ACTION_HASH ?? (__ICON_OVERLAY_ACTION_HASH = new DrawStackMethodsWrapper(ICON_OVERLAY_ACTION)); } }
		void ICON_OVERLAY_ACTION(Rect worldOffset, Rect label_icon_rect, DrawStackMethodsWrapperData data, HierarchyObject _o)
		{

			var overlayButStr = (OverlayButtonStr)data.args;
			IconOverlayGUI(label_icon_rect, overlayButStr.treeItem);
			OverlayIconGUI(label_icon_rect, overlayButStr.treeItem, overlayButStr.active);
		}
		DrawStackMethodsWrapper __SECOND_ICON_OVERLAY_ACTION_HASH = null;
		DrawStackMethodsWrapper SECOND_ICON_OVERLAY_ACTION_HASH { get { return __SECOND_ICON_OVERLAY_ACTION_HASH ?? (__SECOND_ICON_OVERLAY_ACTION_HASH = new DrawStackMethodsWrapper(SECOND_ICON_OVERLAY_ACTION)); } }
		void SECOND_ICON_OVERLAY_ACTION(Rect worldOffset, Rect label_icon_rect, DrawStackMethodsWrapperData data, HierarchyObject _o)
		{

			var overlayButStr = (OverlayButtonStr)data.args;
			IconOverlayGUI(label_icon_rect, overlayButStr.treeItem);
			//   OverlayIconGUI( label_icon_rect , overlayButStr.treeItem , overlayButStr.active );
		}
		DrawStackMethodsWrapper __LABEL_OVERLAY_ACTION_HASH = null;
		DrawStackMethodsWrapper LABEL_OVERLAY_ACTION_HASH { get { return __LABEL_OVERLAY_ACTION_HASH ?? (__LABEL_OVERLAY_ACTION_HASH = new DrawStackMethodsWrapper(LABEL_OVERLAY_ACTION)); } }
		void LABEL_OVERLAY_ACTION(Rect worldOffset, Rect selectionRect, DrawStackMethodsWrapperData data, HierarchyObject _o)
		{
			var overlayButStr = (OverlayButtonStr)data.args;
			LabelOverlayGUI(selectionRect, overlayButStr.treeItem);
		}



		internal struct ColorButtonStr
		{
			internal Rect localSelectionRect;

		}
		ColorButtonStr colButStr;
		DrawStackMethodsWrapper __BUTTON_ACTION_HASH = null;
		DrawStackMethodsWrapper BUTTON_ACTION_HASH { get { return __BUTTON_ACTION_HASH ?? (__BUTTON_ACTION_HASH = new DrawStackMethodsWrapper(BUTTON_ACTION)); } }
		void BUTTON_ACTION(Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o)
		{


			//if (adapter.IsSceneHaveToSave(_o)) return;
			var colButStr = (ColorButtonStr)data.args;
			//Texture2D drawIcon = colButStr.drawIcon;
			var selectionRect = colButStr.localSelectionRect;

			EditorGUI.DrawRect(selectionRect, Color.white);

			if (Event.current.button == adapter.MOUSE_BUTTON_0 /*&& !Application.isPlaying*/)
			{   /*var pos = InputData.WidnwoRect( !callFromExternal(),
				                                            //  Event.current.mousePosition - new Vector2(0, EditorGUIUtility.singleLineHeight * 2)
				                                            new Vector2(Event.current.mousePosition.x, currentRect.y - EditorStyles.foldout.lineHeight * 1.25f)
				                                            , 0, 0, adapter, lockPos: true);*/



				selectionRect.x += worldOffset.x;
				selectionRect.y += worldOffset.y;
				var capture_o = _o;

				OpenHighlighterWindow(selectionRect, capture_o);
				//  var mp = Event.current.mousePosition;

				/*  mp = GUIUtility.GUIToScreenPoint( mp );
				  var pos = new Rect(mp.x, mp.y, 0, 0);*/


			}

			if (Event.current.button == adapter.MOUSE_BUTTON_1)
			{
				Tools.EventUse();

				/*var icon = _o.ah.drawIcon.add_icon;
				//	var icon = _o.drawIcon.add_icon;
				if (IS_PROJECT())
				{
					if (!icon)
					{
						icon = AssetDatabase.GetCachedIcon(_o.project.assetPath) as Texture2D;
						if (AssetDatabase.GetAssetPath(icon) == "Library/unity editor resources" && icon.name == "Folder Icon") icon = null;
					}
				}
				var mp = new MousePos(Event.current.mousePosition, MousePos.Type.Search_356_0, !callFromExternal(), adapter);
				if (icon)
				{
					//var ext = adapter.IS_HIERARCHY() ? null : _o.project.fileExtension;
					var ttt = icon.name.Replace(adapter.pluginname + "_KEY_#1", "default");
					Windows.SearchWindow.Init(mp, SearchHelper + " '" + ttt + "' icon", "'" + ttt + "' icon",
										   CallHeaderFiltered(icon),
										   this, adapter.window, _o, null);
				}
				else
				{
					Windows.SearchWindow.Init(mp, SearchHelper + " User's icons Only", "any user's icons", CallHeader(), this, adapter.window, _o);
				}*/

			
				var mp = new MousePos(Event.current.mousePosition, MousePos.Type.Search_356_0, !callFromExternal(), adapter);
				if ( ValidateIncludeNulls(_o) )
				{
					//var ext = adapter.IS_HIERARCHY() ? null : _o.project.fileExtension;
					//var ttt = icon.name.Replace(adapter.pluginname + "_KEY_#1", "default");
					var ob = temp_get(_o);
					Windows.SearchWindow.Init( mp, SearchHelper + " highlighter", "assigned colors or icons",
										   CallHeaderFiltered( ob ),
										   this, adapter.window, _o, null );
				}
				else
				{
					Windows.SearchWindow.Init( mp, SearchHelper + " highlighter", "any colors or icons", CallHeader(), this, adapter.window, _o );
				}

				// EditorGUIUtility.ic
			}







		}


		internal void OpenHighlighterWindow(Rect selectionRect, HierarchyObject captured_o)
		{

			var mp = new Vector2(Event.current.mousePosition.x, selectionRect.y + selectionRect.height * 1.4f);
			var pos = new MousePos(mp, MousePos.Type.Highlighter_410_0, false, adapter);



			Action<Texture, string> _SetIconImage = (currentSelection, undoStr) =>
			{

             

                if (currentSelection == null)
				{
					HighlighterCache_Icons.SetIcon(captured_o, (Texture2D)null, this);
				}

				else
				{
					var library = AssetDatabase.GetAssetPath(currentSelection).StartsWith("Library/", StringComparison.OrdinalIgnoreCase);
					string texName = library
									 ? currentSelection.name
									 : "GUID=" + AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(currentSelection));

					if (!library)
					{


						var last = HighLighterCommonData.GetIconsHistory();
						last.RemoveAll(s => s == texName);
						if (last.Count == 0) last.Add(texName);
						else last.Insert(0, texName);
						while (last.Count > 20) last.RemoveAt(20);
						HighLighterCommonData.SetDirty();
						//	Hierarchy_GUI.Undo(adapter, undoStr);
						//	Hierarchy_GUI.GetLastList(adapter).RemoveAll(t => t == texName);
						//	if (Hierarchy_GUI.GetLastList(adapter).Count == 0) Hierarchy_GUI.GetLastList(adapter).Add(texName);
						//	else Hierarchy_GUI.GetLastList(adapter).Insert(0, texName);
						//	while (Hierarchy_GUI.GetLastList(adapter).Count > 20) Hierarchy_GUI.GetLastList(adapter).RemoveAt(20);
						//Hierarchy_GUI.SetDirtyObject(adapter);
					}

					HighlighterCache_Icons.SetIcon(captured_o, (Texture2D)currentSelection, this);
					//adapter.RepaintWindowInUpdate_PlusResetStack();
					ClearCacheAdditional();
				}



            };
			//

			//var GET_TEXTURE = TODO_Tools.GetObjectBuildinIcon.ObjectContent_NoCacher(adapter, EditorUtility.InstanceIDToObject(_o.id), adapter.t_GameObject).add_icon;
			var _GetTexture = TODO_Tools.GetObjectBuildinIcon(EditorUtility.InstanceIDToObject(captured_o.id), Tools.unityGameObjectType).add_icon;

			Func<TempColorClass> _GetHiglightColor = () =>
			{
				var l_o = captured_o;
				TempColorClass _tempColor = new TempColorClass();
				if (!l_o.Validate(true)) return _tempColor.empty;
				//var c = GetHighlighterValue(l_o.scene, l_o);
				var c = HighlighterCache_Colors.GetHighlighterValue(l_o, this);
				//Debug.Log(c.Length);
				if (c != null && c.Length > 0) return _tempColor.AssignFromList(ref c);
				else return _tempColor.empty;
			};

			Action<TempColorClass, string> _SetHiglightColor = (el, undoName) =>
			{
				HighlighterCache_Icons.SetAction_MixedUndo(captured_o, (in_o) =>
				{
					if (!in_o.Validate(true)) return;
					//var d = adapter.MOI.des(in_o.scene);
					//adapter.SET_UNDO(d, undoName);
					//SetHighlighterValue(el, in_o.scene, in_o);
					
					//Debug.Log(pluginID);
					HighlighterCache_Colors.SetHighlighterValu_MixedUndoe(el, in_o, this);
					//ClearCacheAdditional();
				},  undoName);
			};


			Action<Color32, string> _SetIconColor =
				(c, undoStr) =>
				{
					HighlighterCache_Icons.SetAction_MixedUndo(captured_o, (l_o) =>
					{
						if (!l_o.Validate(true)) return;
						HighlighterCache_Icons.SetImageColor_OnlyCache_MixedUndor(l_o, c, this);
						//adapter.RepaintWindowInUpdate_PlusResetStack();
						//ClearCacheAdditional();
					},  undoStr);
				};
			Func<Color32?> _GetIconColor = () =>
			{
				var l_o = captured_o;
				if (!l_o.Validate(true)) return null;
				TempSceneObjectPTR ob_dat = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModManualIcons, captured_o);
				if (ob_dat == null || ob_dat.iconData.Length == 0 || !ob_dat.iconData[0].has_icon_color) return null;
				return ob_dat.iconData[0].icon_color;
			};


			//	_SetIconImage, Texture _GetTexture, Action< TempColorClass, string> _SetHiglightColor , Action< Color32, string> _SetIconColor, Func<Color32?> _GetIconColor
			//M_Colors_Window.Init(pos, "Select Icon", SET_TEXTURE, _o.drawIcon != null && _o.drawIcon.add_icon ? GET_TEXTURE : null, SET_HIGLIGHT_COLOR, GET_HIGLIGHTER_COLOR
			Root_HighlighterWindowInterface.Init(pos, "Select Icon", _SetIconImage,
				//drawIcon ? _GetTexture : null
				_GetTexture

				, _SetHiglightColor, _GetHiglightColor, _SetIconColor, _GetIconColor
			, captured_o
			, adapter.window

								);
		}
	}
}

