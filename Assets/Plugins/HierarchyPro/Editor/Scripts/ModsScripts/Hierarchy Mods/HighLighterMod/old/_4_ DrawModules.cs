#if FALSE
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if PROJECT
	using EModules.Project;
#endif
//namespace EModules



namespace EModules.EModulesInternal

{
internal partial class Adapter {


	bool ChechButton( bool cv )
	{	if ( par.USE_BUTTON_TO_INTERACT_AHC == 0 || !cv ) return false;
	
		if ( Event.current == null ) return false;
		
		switch ( par.USE_BUTTON_TO_INTERACT_AHC & 3 )
		{	case 1: return !Event.current.alt;
		
			case 2: return !Event.current.shift;
			
			case 3: return !Event.current.control;
		}
		
		return false;
	}
	
	
	
	int _IsDraggedCache_lastID, _IsDraggedCache_LastCount = -1;
	Dictionary<int, bool> _IsDraggedCache = new Dictionary<int, bool>();
	
	int _IsSelectedCache_lastID, _IsSelectedCache_LastCount = -1;
	Dictionary<int, bool> _IsSelectedCache = new Dictionary<int, bool>();
	internal int selMax = 0;
	internal bool IsSelected( int id )
	{
	
		selMax = 0;
		
		if ( current_DragSelection_List.Count != 0 )
		{	if ( _IsDraggedCache_LastCount != _IsDraggedCache.Count || _IsDraggedCache_lastID != current_DragSelection_List[0] )
			{	_IsDraggedCache_lastID = current_DragSelection_List[0];
				_IsDraggedCache_LastCount = current_DragSelection_List.Count;
				_IsDraggedCache.Clear();
				_IsDraggedCache = current_DragSelection_List.ToDictionary(k=>k, k=>true);
			}
			
			selMax = current_DragSelection_List.Count;
			
			return _IsDraggedCache.ContainsKey(id);
			
			/*if ( !_IsDraggedCache.ContainsKey( id ) )
				_IsDraggedCache.Add( id, current_DragSelection_List.Contains( id ) );  //(adapter._mSelectedO.ContainsKey(__o.id) )
			
			return _IsDraggedCache[id];*/
		}
		
		if ( current_selectedIDs.Count != 0 )
		{	if ( _IsSelectedCache_LastCount != _IsSelectedCache.Count || _IsSelectedCache_lastID != current_selectedIDs[0] )
			{	_IsSelectedCache_lastID = current_selectedIDs[0];
				_IsSelectedCache_LastCount = current_selectedIDs.Count;
				_IsSelectedCache.Clear();
				_IsSelectedCache = current_selectedIDs.ToDictionary(k=>k, k=>true);
			}
			
			selMax = current_selectedIDs.Count;
			
			return _IsSelectedCache.ContainsKey(id);
			/*if ( !_IsSelectedCache.ContainsKey( id ) )
				_IsSelectedCache.Add( id, current_selectedIDs.Contains( id ) );  //(adapter._mSelectedO.ContainsKey(__o.id) )
			
			return _IsSelectedCache[id];*/
		}
		
		return false;
	}
	
	static GUIContent _GET_CONTENT = new GUIContent();
	internal static GUIContent GET_CONTENT( string text )
	{	_GET_CONTENT.text = text;
		return _GET_CONTENT;
	}
#pragma warning disable
	static float MinHeight, MaxHeight;
#pragma warning restore
	
	static GUIContent __ModuleButtonContent = new GUIContent
	();
	/*   public  bool ModuleButton(Rect drawRect, string content)
	   {   __ModuleButtonContent.text  = content;
	       return ModuleButton(drawRect, __ModuleButtonContent);
	   }*/
	public bool SimpleButton( Rect drawRect, GUIContent content,  GUIStyle style = null )
	{	style = style ?? button;
		return GUI.Button( drawRect, content, style );
	}
	public bool ModuleButton( Rect drawRect, GUIContent content, bool hasContent, GUIStyle style = null )
	{	style = style ?? button;
	
		if ( content == null )
		{
		
			content = __ModuleButtonContent;
		}
		
		if ( par.USE_BUTTON_TO_INTERACT_AHC == 0 || (par.USE_BUTTON_TO_INTERACT_AHC & 8) == 0 && hasContent )
		{	//  return GUI.Button( drawRect , content );
			return GUI.Button( drawRect, content, style );
		}
		
		bool Allow = false;
		
		switch ( par.USE_BUTTON_TO_INTERACT_AHC & 3 )
		{	case 1: Allow = Event.current.alt; break;
		
			case 2: Allow = Event.current.shift; break;
			
			case 3: Allow = Event.current.control; break;
			
			// default: return GUI.Button( drawRect , content );
			default: return GUI.Button( drawRect, content, style );
		}
		
		if ( !Allow )
		{	// GUI.enabled = false; GUI.Button( drawRect , content , buttonStyle ); GUI.enabled = true;
			GUI.enabled = false; GUI.Button( drawRect, content, style ); GUI.enabled = true;
			return false;
		}
		
		// Adapter.DrawRect(drawRect, new Color(1, 1, 1, 0.05f));
		// return GUI.Button( drawRect , content );
		return GUI.Button( drawRect, content, style );
	}
	
	internal bool DISABLE_DESCRIPTION( HierarchyObject o )
	{	if ( pluginID == Initializator.PROJECT_ID ) return tempAdapterDisableCache;
	
		if ( !o.go || !o.go.scene.isLoaded || tempAdapterDisableCache ) return true;
		
		return DISABLE_DES();
	}
	internal bool DISABLE_DESCRIPTION( int s )
	{	if ( IS_HIERARCHY() && !Adapter.GET_SCENE_BY_ID( s ).isLoaded || tempAdapterDisableCache ) return true;
	
		return DISABLE_DES();
	}
	internal bool DISABLE_DESCRIPTION( Scene s )
	{	if ( IS_HIERARCHY() && !(s).isLoaded || tempAdapterDisableCache ) return true;
	
		return DISABLE_DES();
	}
	internal bool DISABLE_DES()
	{	return tempAdapterDisableCache || !wasAdapterInitalize;
	}
	
	
	
	
	DynamicRect __tempDynamicRect;
	DynamicRect tempDynamicRect { get { return __tempDynamicRect ?? (__tempDynamicRect = new DynamicRect() { adapter = this }); } }
	
	Module[] lastOrdered = null;
	Module __M_CustomIconts;
	Module M_CustomIconts
	{	get
		{	if ( lastOrdered != __modulesOrdered )
			{	lastOrdered = __modulesOrdered;
				__M_CustomIconts = modulesOrdered.First( m => m is IModuleOnnector_M_CustomIcons );
			}
			
			return __M_CustomIconts;
		}
	}
	internal bool M_CustomIcontsEnable { get { return M_CustomIconts == null ? false : M_CustomIconts.ENABLE; } }
	
	
	void DrawBG( HierarchyObject _o, Rect selectionRect, Rect fadeRect )
	{	if ( LastHeaderRect.Contains( Event.current.mousePosition ) && Event.current.isMouse || IS_PREFAB_MOD_OPENED() )
		{	return;
		}
		
		
		// MonoBehaviour.print(window().position);
		// if (Event.current.type == EventType.repaint) currentClipRect = window().position;
		
		
		if ( ENABLE_LEFTDOCK_PROPERTY && (Event.current.type == EventType.Repaint || IS_LAYOUT) )
		{
		
		
		
			ColorModule.callFromExternal_objects = null;
			/* if ( _o.name == "Misc" ) Debug.Log( ColorModule.DRAW_STACK.ContainsKey( _o.id ) );
			 if ( ColorModule.DRAW_STACK.ContainsKey( _o.id ) && _o.name == "Misc" ) Debug.Log( ColorModule.DRAW_STACK[_o.id].currentStackPos );
			 if ( ColorModule.DRAW_STACK.ContainsKey( _o.id ) && _o.name == "Misc" ) Debug.Log( ColorModule.DRAW_STACK[_o.id].GO_ENABLE_STATE );
			 if ( ColorModule.DRAW_STACK.ContainsKey( _o.id ) && _o.name == "Misc" ) Debug.Log( _o.Active() );*/
			var res = ColorModule.START_DRAW_PARTLY_TRYDRAW(selectionRect,  _o );
			
			// if ( res ) Debug.Log( _o.name );
			if ( !res ) return;
			
			ColorModule.START_DRAW_PARTLY_CREATEINSTANCE( selectionRect, _o, Event.current.type == EventType.Repaint );
			
			
			_o.BG_RECT = null;
			_o.BACKGROUNDED = 0;
			_o.FLAGS = 0;
			
			ColorModule.TryToFadeBG( selectionRect, _o );
			
			if ( !DISABLE_DESCRIPTION( _o ) )
			{	var w = window();
				var targetRect = selectionRect;
				
				if ( HIGHLIGHTER_LEFT_OVERFLOW == 1 )     // targetRect.width += raw_old_leftpadding; //HIGHLIGHTER_LEFT_OVERFLOW == 1 ? 0 :
				{
				}
				
				if ( Adapter.UNITY_CURRENT_VERSION < Adapter.UNITY_2019_2_0_VERSION )
					targetRect.width -= raw_old_leftpadding;
					
				tempDynamicRect.Set( targetRect, fadeRect, true, _o, IS_HIERARCHY() && Adapter.USE2018_3 || IS_PROJECT(), raw_old_leftpadding );
				var cm = selectionRect;
				cm.width += cm.x;
				cm.x = 0;
				ColorModule.DrawBackground( cm, w, tempDynamicRect, _o );
			}
			
			
			ColorModule.END_DRAW( _o );
			
			
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
	/*TOTAL_LEFT_PADDING */
	
	void DrawModules( HierarchyObject o, Rect selectionRect, Rect fadeRect )
	{	if ( LastHeaderRect.Contains( Event.current.mousePosition ) && Event.current.isMouse )
		{	return;
		}
		
		var currentRect = selectionRect;
		currentRect.x = currentRect.x + currentRect.width - padding_right;
		
		// MonoBehaviour.print(window().position);
		// if (Event.current.type == EventType.repaint) currentClipRect = window().position;
		
		var drawRect = currentRect;
		
		var w = window();
		
		if ( ENABLE_LEFTDOCK_PROPERTY )
		{	ColorModule.callFromExternal_objects = null;
		
		
			// ColorModule.TryToFadeBG( selectionRect, o );
			
			
			ColorModule.Draw( selectionRect, o );
			
			if ( ColorModule.CURRENT_STACK != null ) throw new Exception( "Cache not finalizing" );
		}
		
		
		if ( ENABLE_RIGHTDOCK_PROPERTY )     //  var clipRect = currentRect;
		{	//  clipRect.x = Math.Max(300 - window().position.width, 0);
			// clipRect.width = window().position.width;
			// MonoBehaviour.print(clipRect);
			// GUI.BeginClip(clipRect);
			/*   drawRect.x = clipRect.x - drawRect.x;
			   drawRect.y -= clipRect.y;
			   currentRect.x = clipRect.x - currentRect.x;
			   currentRect.y -= clipRect.y;*/
			
			if ( par.COMPONENTS_NEXT_TO_NAME )     // var finded = MOI.M_CustomIcons;
			{	// Debug.Log( M_CustomIcontsEnable );
				if ( M_CustomIcontsEnable )
				{	var rect = selectionRect;
					ObjectNameCOntent.text = o.ToString();
					//                         var oldL = Adapter.GET_SKIN().label.fontSize;
					//                         var oldA = Adapter.GET_SKIN().label.alignment;
					//                         Adapter.GET_SKIN().label.fontSize = oldFOntl;
					//                         Adapter.GET_SKIN().label.alignment = oldFOnal;
					rect.x += labelStyle.CalcSize( ObjectNameCOntent ).x + par.COMPONENTS_NEXT_TO_NAME_PADDING + TOTAL_LEFT_PADDING;
					
					if ( IS_PROJECT() ) rect.x += 18;
					
					//                         Adapter.GET_SKIN().label.fontSize = oldL;
					//                         Adapter.GET_SKIN().label.alignment = oldA;
					// if (rect.x < currentRect.x) {
					rect.width = currentRect.x - rect.x + 4;
					M_CustomIconts.callFromExternal_objects = null;
					
					// GUI.DrawTexture( rect, Texture2D.whiteTexture );
					//                         GUI.BeginClip( rect );
					//                         rect.x = 0;
					//                         rect.y = 0;
					M_CustomIconts.Draw( rect, o );
					
					if ( M_CustomIconts.CURRENT_STACK != null ) throw new Exception( "Cache not finalizing" );
					
					// GUI.EndClip();
					//  }
					
				}
			}
			
			
			
			var HIDE_MODULES = ChechButton(_S_HideRightIfNoFunction);
			
			if ( !HIDE_MODULES ) FadeRect( fadeRect, par.HEADER_OPACITY ?? DefaultBGOpacity );
			
			var oldG1 = GUI.color;
			var oldG2 = GUI.contentColor;
			
			
			var width = selectionRect.x + selectionRect.width ;
			var first = !ENABLE_ACTIVEGMAOBJECTMODULE;
			Module lastm = null;
			
			//   foreach ( var drawModule in modulesOrdered ) if ( !SKIPMODULE( drawModule, width ) ) lastm = drawModule;
			foreach ( var drawModule in modulesOrdered )
			{
			
				if ( !drawModule.SKIP_BAKED.HasValue ) drawModule.SKIP_BAKED = SKIPMODULE( drawModule, width, true );
				
				if ( !drawModule.SKIP_BAKED.Value ) lastm = drawModule;
			}
			
			var sel = SHOW_ONLY_HOVERED ?IsSelected(o.id) : false;
			
			foreach ( var drawModule in modulesOrdered )
			{	/* if (!drawModule.enable
				             //  || (par.RIGHTDOCK_TEMPHIDE && window().position.width <= Hierarchy.par.RIGHTDOCK_TEMPHIDEMINWIDTH/* && !(drawModule is M_Freeze)#1#)
				             || (par.RIGHTDOCK_TEMPHIDE && width <= Hierarchy.par.RIGHTDOCK_TEMPHIDEMINWIDTH/* && !(drawModule is M_Freeze)#1#)
				          || par.COMPONENTS_NEXT_TO_NAME && drawModule is M_CustomIcons || drawModule.SKIP()) continue;*/
				
				if ( SHOW_ONLY_HOVERED )
				{	if ( hashoveredItem && hoverID != o.id && !sel) continue;
				
				}
				
				//  if ( SKIPMODULE( drawModule, width ) || HIDE_MODULES ) continue;
				if ( !drawModule.SKIP_BAKED.HasValue ) drawModule.SKIP_BAKED = SKIPMODULE( drawModule, width, true );
				
				if ( drawModule.SKIP_BAKED.Value || HIDE_MODULES ) continue;
				
				
				// if (drawModule.enable && (!par.RIGHTDOCK_TEMPHIDE || window().position.width > Hierarchy.par.RIGHTDOCK_TEMPHIDEMINWIDTH || drawModule is M_Freeze)
				//   && par.COMPONENTS_NEXT_TO_NAME && drawModule.Equals(ComponentsModule)) {
				
				/*  if (par.COMPONENTS_NEXT_TO_NAME && drawModule is M_CustomIcons) {
				      var rect = selectionRect;
				      ObjectNameCOntent.text = o.name;
				      rect.x += Adapter.GET_SKIN().label.CalcSize(ObjectNameCOntent).x + 10;
				
				      drawModule.callFromExternal = false;
				      drawModule.Draw(rect, o);
				      continue;
				  }*/
				
				// currentRect.width = drawModule.sibbildPos == -1 ? drawModule.STATIC_WIDTH() : Math.Max(drawModule.width, defWDTH);
				
				currentRect.width = Math.Max( drawModule.width, defWDTH );
				currentRect.x -= currentRect.width;
				
				
				var MIN = par.PADDING_LEFT;
				
				//  if (window() != null && MIN > window().position.width - RIGHTADD_PadLeft) MIN = window().position.width - RIGHTADD_PadLeft;
				if ( w != null && MIN > width - RIGHTPAD ) MIN = width - RIGHTPAD;
				
				bool fade = (currentRect.x < MIN);
				currentRect = ClipMINSizeRect( currentRect, width );
				
				if ( currentRect.width < 2 ) continue;
				
				//  FadeRect(currentRect, par.HEADER_OPACITY ?? DefaultBGOpacity);
				
				
				if ( fade )
				{	var c = GUI.color;
					var t = currentRect.width / drawModule.width * 2;
					c.a = Mathf.SmoothStep( 0, 1, t );
					GUI.color = c;
					c = GUI.contentColor;
					c.a = Mathf.SmoothStep( 0, 1, t );
					GUI.contentColor = c;
				}
				
				/*
				
				                        if (Selection.gameObjects != null && Selection.gameObjects.Any(g => g == o))
				                            SelectRect(currentRect, 1);*/
				
				
				drawRect = currentRect;
				
				// if (CurrentRect.ContainsKey(drawModule)) drawRect.x = CurrentRect[drawModule].x;
				if ( CurrentRectContainsKey( w, drawModule ) ) drawRect.x = CurrentRect( w, drawModule ).x;
				
				if ( first )
				{	drawRect.width += M_SetActive_WIDTH - 1;
					first = false;
				}
				
				// GUI.BeginGroup(drawRect);
				//GUI.BeginClip( drawRect );
				//drawRect.x = 0;
				//drawRect.y = 0;
				
				
				drawModule.callFromExternal_objects = null;
				drawModule.Draw( drawRect, o );
				
				if ( drawModule.CURRENT_STACK != null ) throw new Exception( "Cache not finalizing" );
				
				//GUI.EndClip();
				// GUI.EndGroup();
				
				drawRect = currentRect;
				
				if ( lastm != drawModule )     // drawRect.x -= 2;
				{	if ( !EditorGUIUtility.isProSkin )
					{	/* drawRect.width = 2;
					
						     GUI.DrawTexture( drawRect, GetIcon( "SEPARATOR" ) );
						     GUI.DrawTexture( drawRect, GetIcon( "SEPARATOR" ) );*/
						
						drawRect.width = 1;
						drawRect.x -= 1;
						Adapter.DrawRect( drawRect, S_COL3 );
						drawRect.x += 1;
						Adapter.DrawRect( drawRect, S_COL4 );
					}
					
					else
					{	drawRect.width = 1;
						drawRect.x -= 1;
						/* GUI.DrawTexture( drawRect, GetIcon( "SEPARATOR" ) );
						 GUI.DrawTexture( drawRect, GetIcon( "SEPARATOR" ) );
						 GUI.DrawTexture( drawRect, GetIcon( "SEPARATOR" ) );*/
						
						Adapter.DrawRect( drawRect, S_COL1 );
						drawRect.x += 1;
						Adapter.DrawRect( drawRect, S_COL2 );
					}
				}
				
				// GUI.DrawTexture(drawRect, colorStatic);
				/*  drawRect.x -= 1;
				  drawRect.width = 2;
				  if (EditorGUIUtility.isProSkin) {
				      GUI.DrawTexture(drawRect, sec);
				      GUI.DrawTexture(drawRect, sec);
				      GUI.DrawTexture(drawRect, sec);
				  }*/
				
			} // foreach
			
			GUI.color = oldG1;
			GUI.contentColor = oldG2;
			
			
			
			if ( pluginID == Initializator.HIERARCHY_ID && ENABLE_ACTIVEGMAOBJECTMODULE )
			{	modules[0].callFromExternal_objects = null;
				modules[0].Draw( selectionRect, o );
				
				if ( modules[0].CURRENT_STACK != null ) throw new Exception( "Cache not finalizing" );
			}
			
			// GUI.EndClip();
		}//RIGHT
		
		/*   if (ENABLE_LEFTDOCK_PROPERTY)
		   {
		       foreach (var module in modules)
		       {
		           if (module.sibbildPos != -1) continue;
		           if (!module.enable) continue;
		
		           module.callFromExternal = false;
		           module.Draw(selectionRect, o);
		       }
		   } else
		   {
		       modules[0].callFromExternal = false;
		       modules[0].Draw(selectionRect, o);
		   }*/
		
		if (!IS_PREFAB_MOD_OPENED() )
		{
		
			if ( _S_hoverState > 0 && M_Colors_Window.Enable && M_Colors_Window.source == o )
			{	Adapter.DrawRect(
				    HoverRect(o, selectionRect, _S_bgButtonForIconsPlace ),
				    EditorGUIUtility.isProSkin ? Color.white : new Color( 1, 0, 0, 0.5f ) );
				/* GUI.DrawTexture( HoverRect( selectionRect,
				                            _S_bgButtonForIconsPlace ), GetIcon( "SETUP_SORT1" ),
				                 ScaleMode.ScaleToFit );      */     //HIGHLIGHTER_COLOR_PICKER   SETUP_BLUELINE  STORAGE_ONECOMP   BOTTOM_DESBUT FAVORIT_LIST_ICON FAVORIT_LIST_ICON ON STAR SETUP_SORT1 SETUP_SLIDER_HOVER
				
				//Adapter.DrawRect(new Rect(0, selectionRect.y + selectionRect.height / 8 * 3, selectionRect.height / 4 * 3, selectionRect.height / 8 * 2), Color.red);
			}
			
			if ( _S_hoverState > 1 && hashoveredItem && w )
			{	if ( hoverID == o.id )
				{	Adapter.DrawRect(
					    HoverRect(o, selectionRect, _S_bgButtonForIconsPlace ),
					    EditorGUIUtility.isProSkin ? Color.white : new Color( 1, 0, 0, 0.5f ) );
					/*  GUI.DrawTexture( HoverRect( selectionRect, _S_bgButtonForIconsPlace ), GetIcon( "SETUP_SORT1" ), ScaleMode.ScaleToFit);*/
					
				}
			}
		}
		
		
		
	}
	
	Color32 S_COL1 = new Color32(8, 8, 8, 50);
	Color32 S_COL2 = new Color32(132, 132, 132, 50);
	
	Color32 S_COL3 = new Color32(8, 8, 8, 20);
	Color32 S_COL4 = new Color32(255, 255, 255, 50);
	
	
	internal Rect HoverFullRect( Rect selectionRect )
	{	var icon_rect = ColorModule.GetIconRect(selectionRect, 0, 0 );
	
		//  var size = selectionRect.height / 8 * 2;
		if ( UNITY_CURRENT_VERSION == UNITY_2019_2_0_VERSION )
		{	icon_rect.x += raw_old_leftpadding;
		}
		
		return icon_rect;
	}
	internal  Rect HoverRect(HierarchyObject _o, Rect selectionRect, int IconPlace, int? overrideBgIconPlace = null)
	{	var icon_rect = ColorModule.GetIconRect(selectionRect, IconPlace == 0 /*|| _S_bgButtonForIconsPlace == 2*/ ? 2 : 0, overrideBgIconPlace );
	
		//  var size = selectionRect.height / 8 * 2;
		if ( IconPlace == 0 )
		{	if ( UNITY_CURRENT_VERSION >= UNITY_2019_2_0_VERSION )
			{	icon_rect.x += raw_old_leftpadding;
			}
		}
		
		if (ColorModule.lastIconPlace == 2 || ColorModule.lastIconPlace == 0)
		{	if (  SETACTIVE_POSITION == 1 && ENABLE_ACTIVEGMAOBJECTMODULE )
			{	if ( (_o.parent(this) != null || _o.ChildCount(this)== 0))
					icon_rect.x += 8;
				else
					icon_rect.x += 4;
			}
			
			icon_rect.x += 4;
			
			icon_rect.x += 2;
		}
		
		
		var size = EditorGUIUtility.singleLineHeight / 4;
		
		if (ColorModule.lastIconPlace == 0) size -=1;
		
		//  var size = EditorGUIUtility.singleLineHeight / 2.5f;
		//  var size = parLINE_HEIGHT / 1.5f;
		if ( IconPlace != 0 )
			icon_rect.x += (icon_rect.width - size) / 2;
			
		icon_rect.y += (icon_rect.height - size) / 2;
		icon_rect.width = icon_rect.height = size;
		return icon_rect;
	}
	internal int hoverID;
	//  Color etet = new Color(1, 1, 1, 0.3f);
	// Color etet = new Color( 1 , 1 , 1 , 1 );
}
}
#endif