using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using EMX.HierarchyPlugin.Editor.Windows;

namespace EMX.HierarchyPlugin.Editor.Mods
{
	internal partial class HighlighterMod : DrawStackAdapter, IModSaver
	{







		/*
		internal TempColorClass MCOLOR_NEEDGETCOLOR(HierarchyObject o)
		{
		
		    if (!adapter.ENABLE_LEFTDOCK_PROPERTY || adapter.DISABLE_DES()) return null;
		
		    if (HasKey( o.scene, o ))
		    {   return DrawColoredBG( null, o, o, false );
		    }
		    else if (anyNeedBroadcast)
		    {   var parent = o.parent(adapter);
		        while (parent != null)
		        {   if (HasKey( o.scene, parent ))
		            {   return
		                    DrawColoredBG( null, parent, o, false );
		            }
		            parent = parent.parent( adapter );
		        }
		    }
		    return null;
		}
		
		*/

#pragma warning disable
		GUIContent content = new GUIContent() { tooltip = "HighLighter" };
		GUIContent contentNull = new GUIContent() { tooltip = "Empty Transform" };
		GUIContent contentMis = new GUIContent() { tooltip = "This Object Has Missing MonoScript" };
#pragma warning restore

		Color prefabColorPro = new Color32(0x7d, 0xad, 0xf3, 255);
		//Color prefabColorPro = new Color32(76, 128, 217, 255);
		Color prefabColorPersonal = new Color32(0, 39, 131, 255);

		//Color prefabMissinColorPro = Color.Lerp( new Color32(0x7d, 0xad, 0xf3, 255), Color.white, 0.5f);
		Color prefabMissinColorPro = new Color32(164, 94, 94, 255);
		Color prefabMissinColorPersonal = new Color32(63, 13, 13, 255);

		GUIContent switchedConten = new GUIContent();
		//  Color contentColor = new Color32(255, 255, 255, 90);


		/*
		bool TryToFindInternalIcon(HierarchyObject o)
		{	var iac = EditorGUIUtility.ObjectContent(o.go, adapter.t_GameObject).image;
		
			if (iac && (iac.name.StartsWith("sv_icon_dot") || iac.name.StartsWith("sv_label_")))
			{	if (AssetDatabase.GetAssetPath(iac).StartsWith("Library/"))
				{	// var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath( iac ));
					SetImageForObject_OnlyCacher(o, "Library/" + iac.name);
					return true;
				}
			}
			
			return false;
		}
		
		Dictionary<GameObject, int> addIconChecherCache = new Dictionary<GameObject, int>();
		internal Dictionary<string, Texture> cached_colors = new Dictionary<string, Texture>();
		
		TempColorClass __GetImageForObject_OnlyCacherTempColor_Empty = new TempColorClass().AddIcon(null);
		// TempColorClass __GetImageForObject_OnlyCacherTempColor = new TempColorClass();
		internal TempColorClass GetImageForObject_OnlyCacher(HierarchyObject o)
		{	return GetImageForObject_OnlyCacher(o, false);
		}
		
		internal TempColorClass GetImageForObject_OnlyCacher(HierarchyObject o, bool skipFilter)
		{	if (adapter.NEW_PERFOMANCE && new_perfomance_onlycaher_icons.ContainsKey(o.id))
				return new_perfomance_onlycaher_icons[o.id];
				
				
			if (IconImageCacher == null || !IconImageCacher.HasKey(o.scene, o))
			{	if (IconImageCacher == null)
					IconImageCacher = new ObjectCacheHelper<GoGuidPair, Int32List>(
					    property => property.GetHash_IconImageKey(), property => property.GetHash_IconImageValue(),
					    Adapter.CacherType.ImageIcon, adapter, "M_ImageIcons");
					    
				bool tryToApplyilter = true;
				
				if (IconImageCacher != null && adapter.pluginID == 0 && HAS_LABEL_ICON())
				{	if (o.go && !addIconChecherCache.ContainsKey(o.go))
					{	addIconChecherCache.Add(o.go, 0);
						tryToApplyilter = !TryToFindInternalIcon(o);
					}
				}
				
				
				if (tryToApplyilter)
				{	if (!skipFilter)
					{	if (IconImageCacher != null) //#-COLUP
						{	var filtered = this.GetFilter(adapter, o);
						
							if (filtered != null)
							{	if (adapter.NEW_PERFOMANCE)
								{	var result = new TempColorClass();
									filtered.OverrideTo(ref result);
									new_perfomance_onlycaher_icons.Add(o.id, result);
								}
								
								return filtered;
							}
						}
					}
					
					if (adapter.NEW_PERFOMANCE)
					{	new_perfomance_onlycaher_icons.Add(o.id, __GetImageForObject_OnlyCacherTempColor_Empty);
					}
					
					return __GetImageForObject_OnlyCacherTempColor_Empty;
				}
			}
			
			
			var value = IconImageCacher != null && IconImageCacher.HasKey(o.scene, o)
			            ? IconImageCacher.GetValue(o.scene, o)
			            : null;
			            
			            
			if (value != null && !cached_colors.ContainsKey(value.GUIDsActiveGameObject_CheckAndGet))
			{
#pragma warning disable
				bool _else = true;
				
				if (value.GUIDsActiveGameObject.StartsWith("Library/"))
				{	var resource = value.GUIDsActiveGameObject.Substring(value.GUIDsActiveGameObject.IndexOf('/') + 1);
				
					if ((resource.StartsWith("sv_icon_dot") || resource.StartsWith("sv_label_")))
					{	var t = EditorGUIUtility.IconContent(resource).image;
						cached_colors.Add(value.GUIDsActiveGameObject_CheckAndGet, t);
						_else = false;
					}
					
#pragma warning restore
				}
				
				if (_else)
				{	var path = AssetDatabase.GUIDToAssetPath(value.GUIDsActiveGameObject_CheckAndGet);
				
					if (string.IsNullOrEmpty(path))
					{	
						//  if ( TryToFindInternalIcon (o) )
						
						var newGuid = AssetDatabase.AssetPathToGUID(value.PATHsActiveGameObject);
						
						if (string.IsNullOrEmpty(newGuid))
						{	cached_colors.Add(value.GUIDsActiveGameObject_CheckAndGet, null);
							// return __GetImageForObject_OnlyCacherTempColor_Empty;
							goto skip;
						}
						
						value.GUIDsActiveGameObject_CheckAndGet = newGuid;
						adapter.SetDirtyActiveDescription((o.scene));
						path = value.PATHsActiveGameObject;
					}
					
					else
						if (path != value.PATHsActiveGameObject)
						{	value.PATHsActiveGameObject = path;
							adapter.SetDirtyActiveDescription((o.scene));
						}
						
					var t = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
					cached_colors.Add(value.GUIDsActiveGameObject_CheckAndGet, t);
				}
				
				
skip: ;
			}
			
			Color32? ic = null;
			var icon = value != null ? cached_colors[value.GUIDsActiveGameObject_CheckAndGet] : null;
			
			if (!adapter.DISABLE_DES() && IconColorCacher != null && IconColorCacher.HasKey(o.scene, o))
			{	var color = IconColorCacher.GetValue(o.scene, o);
				var hasIconColor = color.list.Count > 3 &&
				                   (color.list[0] > 0 || color.list[1] > 0 || color.list[2] > 0 || color.list[3] > 0);
				                   
				if (hasIconColor)
					ic = new Color32((byte) color.list[0], (byte) color.list[1], (byte) color.list[2],
					                 (byte) color.list[3]);
					                 
				// return __GetImageForObject_OnlyCacherTempColor.AddIcon(icon, hasIconColor, color);
			}
			
			else // return __GetImageForObject_OnlyCacherTempColor.AddIcon(icon);
			{ }
			
			//IconColorCacher
			var __GetImageForObject_OnlyCacherTempColor = new TempColorClass();
			__GetImageForObject_OnlyCacherTempColor.add_icon = icon;
			__GetImageForObject_OnlyCacherTempColor.add_hasiconcolor = ic.HasValue;
			
			if (ic.HasValue) __GetImageForObject_OnlyCacherTempColor.add_iconcolor = ic.Value;
			
			if (adapter.NEW_PERFOMANCE)
				new_perfomance_onlycaher_icons.Add(o.id, __GetImageForObject_OnlyCacherTempColor);
				
				
			if (adapter.NEW_PERFOMANCE && adapter.pluginID == 0)
			{	if (EditorGUIUtility.ObjectContent(o.go, adapter.t_GameObject).image !=
				        __GetImageForObject_OnlyCacherTempColor.add_icon)
				{	if (TryToFindInternalIcon(o))
					{	if (cached_colors.ContainsKey(value.GUIDsActiveGameObject_CheckAndGet))
							cached_colors[value.GUIDsActiveGameObject_CheckAndGet] = EditorGUIUtility.ObjectContent(
							            o.go,
							            adapter.t_GameObject).image;
							            
						__GetImageForObject_OnlyCacherTempColor.add_icon =
						    EditorGUIUtility.ObjectContent(o.go, adapter.t_GameObject).image;
					}
				}
			}
			
			
			return __GetImageForObject_OnlyCacherTempColor;
		} //GetImageForObject_OnlyCacher
		
		Dictionary<int, TempColorClass> new_perfomance_onlycaher_icons = new Dictionary<int, TempColorClass>();
		
		internal void SetImageForObject_OnlyCacher(HierarchyObject o, string guid)
		{	adapter.CreateUndoActiveDescription("Change Icon", (o.scene));
		
			if (string.IsNullOrEmpty(guid)) IconImageCacher.SetValue(null, o.scene, SetPair(o), true);
			else
				IconImageCacher.SetValue(
				    new Int32List()
			{	GUIDsActiveGameObject_CheckAndGet = guid,
				    PATHsActiveGameObject = AssetDatabase.GUIDToAssetPath(guid)
			}, o.scene, SetPair(o), true);
			adapter.SetDirtyActiveDescription((o.scene));
		} //GetImageForObject_OnlyCacher
		
		internal void SetImageForObject_OnlyCacher_2(HierarchyObject o, Texture2D t)
		{	adapter.CreateUndoActiveDescription("Change Icon", (o.scene));
		
			if (!t) IconImageCacher.SetValue(null, o.scene, SetPair(o), true);
			else
			{	var p = AssetDatabase.GetAssetPath(t);
				var guid = AssetDatabase.AssetPathToGUID(p);
				IconImageCacher.SetValue(
				new Int32List() {GUIDsActiveGameObject_CheckAndGet = guid, PATHsActiveGameObject = p}, o.scene,
				SetPair(o), true);
			}
			
			adapter.SetDirtyActiveDescription((o.scene));
		} //GetImageForObject_OnlyCacher
		
		Dictionary<int, bool> _CHeckIcon_cache = new Dictionary<int, bool>();
		
		internal void CHeckIcon(HierarchyObject o, Texture texture)
		{	if (IS_PROJECT()) return;
		
			if (_CHeckIcon_cache.ContainsKey(o.id)) return;
			
			_CHeckIcon_cache.Add(o.id, true);
			
			if (!AssetDatabase.GetAssetPath(texture).StartsWith("Assets")) return;
			
			var p = AssetDatabase.GetAssetPath(texture);
			var guid = AssetDatabase.AssetPathToGUID(p);
			
			if (IconImageCacher.HasKey(o) &&
			        IconImageCacher.GetValue(o.scene, o).GUIDsActiveGameObject_CheckAndGet == guid
			   ) return;
			   
			IconImageCacher.SetValue(
			new Int32List() {GUIDsActiveGameObject_CheckAndGet = guid, PATHsActiveGameObject = p}, o.scene,
			SetPair(o), true);
			// var guid =         "Library/" + texture.name ;
		
			ClearCache();
			
			
		}
		
		*/

		internal static int ICON_WIDTH = 20;


		Color? bgCol;
		Rect tt;

		Rect tRr;
		// Rect standrardRect = new Rect();
		//Color alpha = new Color(1, 1, 1, 0.8f);
		//    Color backCol = new Color();
		// Color tc = new Color();



		Rect GetIconRectIfNextToLabel( Rect selectionRect, GetIconRectIfNextToLabelType type, float size )
		{
			TryToFaeBG_Rect.ref_selectionRect = selectionRect;

			//TryToFaeBG_Rect.ref_selectionRect.x += adapter.ha.LEFT_PADDING;
			// TryToFaeBG_Rect.HasIcon = _S_bgIconsPlace == 0;
			TryToFaeBG_Rect.mod = this;
			TryToFaeBG_Rect.HasIcon = true;
			TryToFaeBG_Rect.MinLeft = adapter.ha.LEFT_PADDING;

			// var size = type == GetIconRectIfNextToLabelType.DefaultIcon ? adapter. DEFAULT_ICON_SIZE : EditorGUIUtility.singleLineHeight;
			// size = (adapter. DEFAULT_ICON_SIZE - EditorGUIUtility.singleLineHeight) / 2 + EditorGUIUtility.singleLineHeight;
			// if (type == GetIconRectIfNextToLabelType.DefaultIcon)
			{ }
			/* else
			 {
			
			 }*/

			tt.Set(
				TryToFaeBG_Rect.GET_LEFT( BgAligmentLeft.BeginLabel ) - size,
				selectionRect.y,
				size, selectionRect.height );

			//  tt.x += adapter.raw_old_leftpadding;
			size = Mathf.Min( selectionRect.height, size );

			//  if (type == GetIconRectIfNextToLabelType.DefaultIcon) size = EditorGUIUtility.singleLineHeight;

			tt.y += (tt.height - size) / 2;
			tt.height = Mathf.RoundToInt( size );
			tt.x += (tt.width - tt.height) / 2;
			tt.width = tt.height;

			return tt;
		}
		internal int lastIconPlace;
		internal Rect GetIconRect( Rect selectionRect, int? overrideValue, int? overrideSBGIconPlace, float size )
		{
			var icon_place = overrideValue ??
							 (callFromExternal() ? 2 : (overrideSBGIconPlace ?? _S_bgIconsPlace));
			lastIconPlace = icon_place;
			var icon_rect = Rect.zero;

			switch ( icon_place )
			{
				case 0:
					icon_rect = GetIconRectIfNextToLabel( selectionRect, GetIconRectIfNextToLabelType.CustomIcon, size );

					break;

				case 1:
				case 2:


					size = Mathf.Min( size, selectionRect.height );
					if ( icon_place == 2 ) icon_rect.x = adapter.ha.LEFT_PADDING;
					else icon_rect.x = selectionRect.x - EditorGUIUtility.singleLineHeight - size;

					icon_rect.y = selectionRect.y;
					icon_rect.height = selectionRect.height;
					//  icon_rect.x += (icon_rect.width - icon_rect.width) / 2 + (adapter.par.COLOR_ICON_SIZE - 12) / 2f;
					icon_rect.width = size;

					icon_rect.y += (icon_rect.height - size) / 2;
					icon_rect.height = size;

					break;
			}

			return icon_rect;
		}

		// bool internalIcon = false;
		internal void TryToFadeBG( Rect selectionRect, HierarchyObject _o )
		{
			_o.ah.internalIcon = false;

			if ( !_o.Validate() ) return;

			if ( !HAS_LABEL_ICON() ) return;

			//var context = HighlighterCache_Icons.ObjectContent_IncludeCacher(o, t_GameObject, includePrefab, this);
			_o.ah.drawIcon = GET_CONTENT( _o );

			if ( _S_bgIconsPlace != 0 && _o.ah.switchType != 1 ) return;

			//  if ( _o.switchType == 1 ) Debug.Log( "ASD" );
			_o.ah.internalIcon = true;

			if ( (!_o.ah.drawIcon.add_icon
				//|| !HighlighterHasKey_IncludeFilters(__o.scene, __o).IsTrue(false)
				) && (_o.ah.switchType == 0) ) return;


			tt = GetIconRectIfNextToLabel( selectionRect, GetIconRectIfNextToLabelType.CustomIcon, DEFAULT_ICON_SIZE );
			//tt.Set( defaultX - 3, defaultY, EditorGUIUtility.singleLineHeight + 6, defaultHeight );

			/*  var style = new GUIStyle("WhiteBackground");
			if ( Event.current.type == EventType.Repaint ) style.Draw( tt, "", false, false, false, false );*/
			// tt.width -= adapter.PREFAB_BUTTON_SIZE;
			/* var oldC = GUI.color;
			 GUI.color *= SourceBGColor( _o.id );
			 GUI.DrawTexture( tt, Texture2D.whiteTexture );
			 GUI.color = oldC;*/
			// Draw_AdapterTexture( tt, SourceBGColor( _o.id ) );

			Draw_AdapterTextureWithDynamicColor( tt, SourceBGColor );

			/* tt.x += tt.width - 1;
			 tt.width = 1;
			 adapter.  tempDynamicRect.Set(tt, tt, true, __o, IS_HIERARCHY() && Adapter.USE2018_3 || IS_PROJECT());
			
			 if (adapter.ENABLE_LEFTDOCK_PROPERTY && !adapter.DISABLE_DESCRIPTION( __o ))
			 {   adapter.MOI.M_Colors.DrawBackground(callFromExternal() ? null : adapter.window(), adapter.  tempDynamicRect, __o, 1 );
			 }*/
		}

		internal DynamicRect _TryToFaeBG_Rect = null;
		internal DynamicRect TryToFaeBG_Rect { get { return _TryToFaeBG_Rect ?? (_TryToFaeBG_Rect = new DynamicRect( this )); } }

		Color inactiveColor = new Color(1, 1, 1, 0.2f);

		object[] obj_array = new object[1];
		protected virtual bool DoFoldout( Rect rect, UnityEditor.IMGUI.Controls.TreeViewItem item, int id ) // if (!active) return;
		{
			obj_array[ 0 ] = id;
			var expandedState =
				(bool)adapter.data_m_dataIsExpanded.Invoke(adapter.data_currentTree, obj_array);
			return expandedState;
			////  Rect foldoutRect = new Rect(rect.x + foldoutIndent, this.GetFoldoutYPosition(rect.y), foldoutStyleWidth, EditorGUIUtility.singleLineHeight);
			// var on = GUI.color;
			// if (!active)GUI.color *= inactiveColor;
			//  adapter.foldoutStyle.Draw(rect,  GUIContent.none, adapter.foldoutStyle, );
			// GUI.color = on;
		}


		OverlayHelper __iverlayHelper = null;
		OverlayHelper iverlayHelper { get { return __iverlayHelper ?? (__iverlayHelper = new OverlayHelper() { mod = this }); } }
		class OverlayHelper
		{
			internal HighlighterMod mod;

			PropertyInfo _iconOverlayGUI; internal PropertyInfo iconOverlayGUI { get { Init(); return _iconOverlayGUI; } }
			PropertyInfo _labelOverlayGUI; internal PropertyInfo labelOverlayGUI { get { Init(); return _labelOverlayGUI; } }
			bool _haslabelOverlayGUI = false; internal bool haslabelOverlayGUI { get { Init(); return _haslabelOverlayGUI; } }
			PropertyInfo _overlayIcon; internal PropertyInfo overlayIcon { get { Init(); return _overlayIcon; } }
			bool _HasoverlayIcon; internal bool HasoverlayIcon { get { Init(); return _HasoverlayIcon; } }

			bool wasInit = false;
			void Init()
			{
				if ( wasInit ) return;
				wasInit = true;

				var GameObjectTreeViewGUI = Assembly.GetAssembly(typeof(EditorWindow)).GetType("UnityEditor.GameObjectTreeViewGUI");
				var treeViewBaseType = GameObjectTreeViewGUI.BaseType;

				_iconOverlayGUI = treeViewBaseType.GetProperty( "iconOverlayGUI", (BindingFlags)(-1) );
				_labelOverlayGUI = treeViewBaseType.GetProperty( "labelOverlayGUI", (BindingFlags)(-1) );
				if ( _labelOverlayGUI != null ) _haslabelOverlayGUI = true;

				if ( mod.IS_HIERARCHY() )
				{
					var goti = Assembly.GetAssembly(typeof(EditorWindow)).GetType("UnityEditor.GameObjectTreeViewItem", false);
					if ( goti != null )
					{
						_overlayIcon = goti.GetProperty( "overlayIcon", (BindingFlags)(-1) );
						if ( overlayIcon != null ) _HasoverlayIcon = true;
					}
				}
			}
		}


		Dictionary<object, Action<UnityEditor.IMGUI.Controls.TreeViewItem, Rect>> __ti = new
		Dictionary<object, Action<UnityEditor.IMGUI.Controls.TreeViewItem, Rect>>();

		void IconOverlayGUI( Rect rect, UnityEditor.IMGUI.Controls.TreeViewItem item ) // rect1.width = this.k_IconWidth + this.iconTotalPadding;
		{

			var tree = adapter.TreeController_current;

			if ( !__ti.ContainsKey( tree ) )
			{
				__ti.Add( tree, null );
				//var gui = adapter.guiProp.GetValue(tree, null);
				__ti[ tree ] = iverlayHelper.iconOverlayGUI.GetValue( adapter.gui_currentTree, null ) as Action<UnityEditor.IMGUI.Controls.TreeViewItem, Rect>;
			}
			if ( __ti[ tree ] == null ) return;
			__ti[ tree ].Invoke( item, rect );
		}

		Dictionary<object, Action<UnityEditor.IMGUI.Controls.TreeViewItem, Rect>> __lo = new
		Dictionary<object, Action<UnityEditor.IMGUI.Controls.TreeViewItem, Rect>>();

		void LabelOverlayGUI( Rect rect, UnityEditor.IMGUI.Controls.TreeViewItem item ) // rect1.width = this.k_IconWidth + this.iconTotalPadding;
		{
			if ( !iverlayHelper.haslabelOverlayGUI ) return;

			var tree = adapter.TreeController_current;

			if ( !__lo.ContainsKey( tree ) )
			{
				__lo.Add( tree, null );
				var gui = adapter.gui_currentTree;
				__lo[ tree ] =
					iverlayHelper.labelOverlayGUI.GetValue( gui, null ) as
					Action<UnityEditor.IMGUI.Controls.TreeViewItem, Rect>;
			}

			if ( __lo[ tree ] == null ) return;

			__lo[ tree ].Invoke( item, rect );
		}

		void OverlayIconGUI( Rect rect, UnityEditor.IMGUI.Controls.TreeViewItem item, bool active ) /* */
		{
			if ( !iverlayHelper.HasoverlayIcon ) return;

			var icon = iverlayHelper.overlayIcon.GetValue(item, null) as Texture2D;

			if ( !icon ) return;

			if ( active ) GUI.DrawTexture( rect, icon );
			else
			{
				var c = GUI.color;
				c *= inactiveColor;
				GUI.DrawTexture( rect, icon );
				GUI.color = c;
			}
		}



		// bool cache_prefab = false;

		// TempColorClass __INTERNAL_GetContentTempColor_Empty = new TempColorClass().AddIcon(null);
		TempColorClass __INTERNAL_GetContentTempColor = new TempColorClass();
		TempColorClass INTERNAL_GetContent( HierarchyObject o, bool includePrefab = true )
		{
			//var context = HighlighterCache_Icons.ObjectContent_IncludeCacher(o, t_GameObject, includePrefab, this);
			var context = HighlighterCache_Icons.ObjectContent_IncludeCacher(o, t_GameObject, includePrefab, this);
			o.ah.switchType = 0;
			return context;
		}

		public TempColorClass GET_CONTENT( HierarchyObject __o )
		{
			return INTERNAL_GetContent( __o );
			//if (!IS_PROJECT())return INTERNAL_GetContent(__o);
			//else return HighlighterCache_Icons.GetImageForObject_OnlyCacher(__o, this);
		}

		public Texture GetUnityContent( HierarchyObject o, bool includePrefab = true )
		{
			var t = HighlighterCache_Icons.ObjectContent_IncludeCacher(o, o.GET_TYPE(), this);

			if ( !t.add_icon ) return null;

			var context = t.add_icon;
			//  if (context != null)
			{
				if ( context == NullContext )
				{
					context = null;
				}

				else
					if ( IS_HIERARCHY() && HighlighterCache_Icons.IsPrefabIcon( context ) )
				{
					if ( set.HIGHLIGHTER_SHOW_PREFAB_ICON && includePrefab )
					{
						var prefab_root = Prefabs.FindPrefabRoot(o.go);

						// var prefab_src = PrefabUtility.GetPrefabParent(prefab_root);
						if ( prefab_root != o.go ) context = null;
					}

					else
					{
						context = null;
					}
				}
			}

			return context;
		}





		void RefreshNullsAndMissings()
		{


			/*	if (IS_HIERARCHY())
				{	foreach (var allSceneObject in Utilities.AllSceneObjects())
					{	var id = allSceneObject.GetInstanceID();

						// var comps = allSceneObject.GetComponents<Component>();
						var comps = HierarchyExtensions.Utilities.GetComponentFast<Component>.GetAll(allSceneObject);


						if (adapter.par.SHOW_NULLS && comps.Length == 1)
						{	if (!Hierarchy.null_cache.ContainsKey(id)) Hierarchy.null_cache.Add(id, false);

							Hierarchy.null_cache[id] = true;
						}

						if (adapter.par.SHOW_MISSINGCOMPONENTS && comps.Any(c => !c))
						{	if (!Hierarchy.missing_cache.ContainsKey(id)) Hierarchy.missing_cache.Add(id, false);

							Hierarchy.missing_cache[id] = true;
						}
					}
				}*/


		}




		/* FillterData.Init(Event.current.mousePosition, SearchHelper, LayerMask.LayerToName(o.layer),
		             Validate(o) ?
		             CallHeaderFiltered(LayerMask.LayerToName(o.layer)) :
		             CallHeader(),
		             this);*/
		/** CALL HEADER */



		//var context = INTERNAL_ GetContent(o);
		// if (context != null && context.name.Contains("Prefab")) MonoBehaviour.print(context.name);
		// return !(context == null || context == NullContext || context.name == "PrefabNormal Icon" || context.name == "PrefabModel Icon");

		//var context = INTERNAL_ GetContent(o);
		// if (context != null && context.name.Contains("Prefab")) MonoBehaviour.print(context.name);
		// return !(context == null || context == NullContext || context.name == "PrefabNormal Icon" || context.name == "PrefabModel Icon");

		bool ValidateWithoutNulls( HierarchyObject o )
		{
			return temp_get( o ) != "";
			//if ( IS_PROJECT() ) return HighlighterCache_Icons.HasKey( o );
			//return GetUnityContent( o, false ) != null;
		}

		bool ValidateIncludeNulls( HierarchyObject o )
		{
			return temp_get( o ) != "";
			//if ( IS_PROJECT() ) return true;
			//return INTERNAL_GetContent( o, true ).add_icon;
		}

		TempColorClass _searchColor = new TempColorClass();
		TempColorClass gettexture( HierarchyObject b )
		{
			var c = HighlighterCache_Colors.HighlighterHasKey_IncludeFilters( b, NowPrefabIsOpened, this );
			if ( !c.IsTrue( false ) ) return null;
			_searchColor.CopyFromList( ref c.data );
			return _searchColor;
			//return adapter.modsController.highLighterMod.DrawBackground( null, null, tempDynamicRect, b, 1, resetFonts: false );
		}
		//TempColorClass gettexture( HierarchyObject b )
		//{
		//	child_Res = HighlighterCache_Colors.HighlighterHasKey_IncludeFilters( b, NowPrefabIsOpened, this ).IsTrue( false );
		//		return child_Res.
		//
		//	//return adapter.modsController.highLighterMod.DrawBackground( null, null, tempDynamicRect, b, 1, resetFonts: false );
		//}
		Texture2D geticon( HierarchyObject b )
		{
			


			var ci = Cache.LOAD_CUSTOM_ICON_USING_HIGHLIGHTER(b);
			if ( ci == null ) return null;
			if ( !ci.add_icon ) return null;
			return ci.add_icon;
			//return b.GetIconForExternal();
		}

		string temp_get( HierarchyObject b )
		{
			if ( b == null ) return "";

			if ( b.name == "TirGames" && b.parent().name == "TirGames" )
				Debug.Log( "Asd" );

			var icon = geticon(b);
			//var icon = context.add_icon;
			if ( icon ) return icon.name;
			//if ( icon && context.add_hasiconcolor )
			//	iconColor = context.add_iconcolor;
			var t = gettexture(b);

			//Debug.Log(b.name);
			if ( t == null || !t.HAS_BG_COLOR && !t.HAS_LABEL_COLOR ) return "";
			if ( t.HAS_BG_COLOR && t.HAS_LABEL_COLOR ) return (t.BGCOLOR.ToString().GetHashCode() ^ t.LABELCOLOR.ToString().GetHashCode()).ToString();

			if ( t.HAS_BG_COLOR ) return t.BGCOLOR.ToString();
			return t.LABELCOLOR.ToString();
			/*var r = gettexture(b).add_icon;

			if (r == null) return "";

			return r.name;*/
		}


		SearchWindow.FillterData_Inputs m_CallHeader()
		{
			//Func<HierarchyObject, TempColorClass> gettexture = null;
			//Func<HierarchyObject, TempColorClass> gettexture = null;
			//Func<HierarchyObject, TempColorClass> geticon = null;
			//GetImageForObject_OnlyCacher;
			/* if ( IS_PROJECT() ) gettexture = INTERNAL_GetContent( b , false );
			 else*/
			//gettexture = (b) => INTERNAL_GetContent(b, false);
			//gettexture = ( b ) => adapter.modsController.highLighterMod.GetSavedColor( b );


			var result = new SearchWindow.FillterData_Inputs(callFromExternal_objects)
			{
				Valudator = ValidateWithoutNulls,
				SelectCompareString = (b,i)=>temp_get(b),
				SelectCompareCostInt = (b, i) =>
				{
					var cost = i;
					var icon = geticon(b);
					//var icon = context.add_icon;
					if (icon ) cost += 1000000;

					cost += temp_get(b).GetHashCode() % 1000000;
					return cost;

					/*var cost = i;
					cost += b.Active() ? 0 : 100000000;
					var c = gettexture(b).add_icon;

					if (c != null && c.name.StartsWith("GUID=")) cost += 1000000; ////////////////////////////////////

					return cost;*/
				}
			};
			return result;
		}

		internal SearchWindow.FillterData_Inputs CallHeader()
		{   //if (adapter.par_e.SHOW_NULLS || adapter.par.SHOW_MISSINGCOMPONENTS) RefreshNullsAndMissings();

			return m_CallHeader();
		}

		internal SearchWindow.FillterData_Inputs CallHeaderFiltered( Texture contentTexture )
		{

			Func<HierarchyObject, bool> gettexture = null;
			/* if ( IS_PROJECT() ) gettexture = s => GetImageForObject_OnlyCacher( s ).add_icon == contentTexture;
			 else gettexture = s => ValidateIncludeNulls( s ) && INTERNAL_GetContent( s , true ).add_icon == contentTexture;*/

			if ( IS_PROJECT() ) gettexture = s => INTERNAL_GetContent( s, true ).add_icon == contentTexture;
			else gettexture = s => ValidateIncludeNulls( s ) && INTERNAL_GetContent( s, true ).add_icon == contentTexture;

			/*	if (IS_HIERARCHY())
				{	if (adapter.par_e.SHOW_NULLS && contentTexture == adapter.GetIcon("NULL") ||
							adapter.par.SHOW_MISSINGCOMPONENTS && contentTexture == adapter.GetIcon("WARNING"))
						RefreshNullsAndMissings();
				}
				*/
			var result = m_CallHeader();
			result.Valudator = gettexture;
			return result;
		}
		/** CALL HEADER */




		internal SearchWindow.FillterData_Inputs CallHeaderFiltered( string hash )
		{


			var result = new SearchWindow.FillterData_Inputs(callFromExternal_objects)
			{
				Valudator = (b) => {return ValidateWithoutNulls(b) && temp_get(b) == hash; },
				SelectCompareString = (b,i)=>temp_get(b),
				SelectCompareCostInt = (b, i) =>
				{
					var cost = i;
					var icon = geticon(b);
					//var icon = context.add_icon;
					if (icon ) cost += 1000000;

					cost += temp_get(b).GetHashCode() % 1000000;
					return cost;

					/*var cost = i;
					cost += b.Active() ? 0 : 100000000;
					var c = gettexture(b).add_icon;

					if (c != null && c.name.StartsWith("GUID=")) cost += 1000000; ////////////////////////////////////

					return cost;*/
				}
			};
			return result;
		}





	}








}

