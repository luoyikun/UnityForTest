using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;



namespace EMX.HierarchyPlugin.Editor.Mods
{
	class HighlighterCache_Icons
	{


		static PluginInstance adapter { get { return Root.p[ 0 ]; } }
		//static HighlighterMod HL_MOD { get { return Root.p[0].modsController.highLighterMod; } }





		public static GUIContent[] labelIcons;
		public static GUIContent[] largeIcons;
		public static void Init_InternatlDefault_IconsList()
		{
			if ( labelIcons == null ) labelIcons = _internal( "sv_label_", string.Empty, 0, 8 );
			if ( largeIcons == null ) largeIcons = _internal( "sv_icon_dot", "_pix16_gizmo", 0, 16 );
		}
		static GUIContent[] _internal( string baseName, string postFix, int startIndex, int count )
		{
			GUIContent[] array = new GUIContent[count];
			for ( int i = 0; i < count; i++ ) array[ i ] = EditorGUIUtility.IconContent( baseName + (startIndex + i) + postFix );
			return array;
		}
		internal static Texture2D Get_InternalDefault_Icon( string str )
		{
			GUIContent result = null;
			var arr = labelIcons.Select(l => l.image.name).ToList();
			if ( arr.IndexOf( str ) != -1 ) result = labelIcons[ Mathf.Clamp( arr.IndexOf( str ), 0, labelIcons.Length - 1 ) ];
			arr = largeIcons.Select( l => l.image.name ).ToList();
			if ( arr.IndexOf( str ) != -1 ) result = largeIcons[ Mathf.Clamp( arr.IndexOf( str ), 0, largeIcons.Length - 1 ) ];
			return result == null ? null : (Texture2D)result.image;
		}

		static string pp;
		static bool TryToFindInternalIcon( HierarchyObject o, HighlighterMod mod )
		{
			if ( o.pluginID == 1 ) return false;
			var iac = EditorGUIUtility.ObjectContent(o.go, Tools.unityGameObjectType).image;
			if ( iac && (iac.name.StartsWith( "sv_icon_dot" ) || iac.name.StartsWith( "sv_label_" )) )
			{

				TempSceneObjectPTR data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModManualIcons, o);
				var skip = data == null || data.iconData.Length == 0;
				pp = "Library/" + iac.name;
				if ( !skip && data.iconData[ 0 ].icon_guid != pp ) skip = true;
				if ( skip )
				{
					//if (AssetDatabase.GetAssetPath(iac).StartsWith("Library/"))
					{   // var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath( iac ));
						//	Debug.Log(o.name);
						SetImageForObject_OnlyCacher( o, pp, mod );
						return true;
					}
				}

			}
			return false;
		}



		static Dictionary<int, bool> _CheckIconIfAlreadyHas = new Dictionary<int, bool>();
		internal static void CheckIconIfAlreadyHas( HierarchyObject o, Texture texture, HighlighterMod mod )
		{
			//if (adapter.IS_PROJECT()) return;
			if ( o == null ) return;
			if ( _CheckIconIfAlreadyHas.ContainsKey( o.id ) ) return;
			_CheckIconIfAlreadyHas.Add( o.id, true );
			if ( !AssetDatabase.GetAssetPath( texture ).StartsWith( "Assets" ) ) return;

			var p = AssetDatabase.GetAssetPath(texture);
			var guid = AssetDatabase.AssetPathToGUID(p);

			var data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModManualIcons, o);
			if ( data == null || data.iconData.Length == 0 || data.iconData[ 0 ].icon_guid == guid ) return;

			SetImageForObject_OnlyCacher( o, guid, mod );



			/*	IconImageCacher.SetValue(
				new Int32List() { GUIDsActiveGameObject_CheckAndGet = guid, PATHsActiveGameObject = p }, o.scene,
				SetPair(o), true);

				ClearCache();*/
			// var guid =         "Library/" + texture.name ;
			/*IconImageCacher.SetValue( new Int32List() { GUIDsActiveGameObject_CheckAndGet = guid , PATHsActiveGameObject = AssetDatabase.GUIDToAssetPath( guid ) } ,
			    o.scene , SetPair( o ) , true );*/
			/* if ( texture.name.StartsWith( "sv_icon_dot" ) || texture.name.StartsWith( "sv_label_" ) ) {
			     if ( _CHeckIcon_cache.ContainsKey( o.id ) ) return;
			     _CHeckIcon_cache.Add( o.id , true );
			     if ( AssetDatabase.GetAssetPath( texture ).StartsWith( "Library/" ) ) {
			         var guid =         "Library/" + texture.name ;
			         IconImageCacher.SetValue( new Int32List() { GUIDsActiveGameObject_CheckAndGet = guid , PATHsActiveGameObject = AssetDatabase.GUIDToAssetPath( guid ) } ,
			             o.scene , SetPair( o ) , true );
			     }
			 }*/
		}










		static internal Texture __NullContext; static internal Texture NullContext { get { return __NullContext ?? (__NullContext = TODO_Tools.GetObjectBuildinIcon( Tools.unityGameObjectType ).add_icon); } }
		static Dictionary<Texture, bool> _IsPrefabIcon = new Dictionary<Texture, bool>();
		internal static bool IsPrefabIcon( Texture t )
		{
			if ( !_IsPrefabIcon.ContainsKey( t ) )
			{
				var pref = !AssetDatabase.GetAssetPath(t).StartsWith("Assets");
				if ( pref )
				{
					if ( t.name.StartsWith( "sv_icon_dot" ) || t.name.StartsWith( "sv_label_" ) ) pref = false;

				}
				_IsPrefabIcon.Add( t, pref
							   /*t.name == "Prefab Icon"
							   || t.name == "PrefabNormal Icon"
							   || t.name == "PrefabModel Icon"*/);
			}
			return (_IsPrefabIcon[ t ]);
		}

		public static Texture GetUnityContent( HierarchyObject o, HighlighterMod mod, bool includePrefab = true )
		{
			var t = ObjectContent_IncludeCacher(o, o.GET_TYPE(), mod);
			if ( !t.add_icon ) return null;
			var context = t.add_icon;
			{
				if ( context == NullContext ) context = null;
				else if (/*adapter.IS_HIERARCHY() &&*/ IsPrefabIcon( context ) )
				{
					if ( adapter.HL_SET.HIGHLIGHTER_SHOW_PREFAB_ICON && includePrefab )
					{
						var prefab_root = Prefabs.FindPrefabRoot(o.go);
						if ( prefab_root != o.go ) context = null;
					}
					else context = null;
				}
			}
			return context;
		}













		TempColorClass __INTERNAL_GetContentTempColor = new TempColorClass();

		internal static TempColorClass INTERNAL_GetContent( HierarchyObject o, HighlighterMod mod, bool includePrefab = true )
		{   ///

			return ObjectContent_IncludeCacher( o, Tools.unityGameObjectType, includePrefab, mod );
			/*if (adapter.IS_HIERARCHY())
			{
				if (adapter.M_CustomIcontsEnable && adapter.par.ENABLE_RIGHTDOCK_FIX &&
						adapter.par.SHOW_MISSINGCOMPONENTS && Hierarchy.missing_cache.ContainsKey(o.id)
						&& Hierarchy.missing_cache[o.id])
				{
					o.switchType = 2;
					return __INTERNAL_GetContentTempColor.AddIcon(adapter.GetIcon("WARNING"));
				}
			}*/

			/*var context = ObjectContent_IncludeCacher(adapter, o, adapter.t_GameObject, includePrefab);
			if (adapter.IS_HIERARCHY()) //
			{
				if (adapter.M_CustomIcontsEnable && adapter.par.ENABLE_RIGHTDOCK_FIX && adapter.par.SHOW_NULLS && (!context.add_icon)
						&& ((Hierarchy.null_cache.ContainsKey(o.go.GetInstanceID()) &&
							 Hierarchy.null_cache[o.go.GetInstanceID()]) ||
							(HierarchyExtensions.Utilities.GetComponentFast<Component>.GetAll(o.go).Length < 2)))
				{
					o.switchType = 1;
					return __INTERNAL_GetContentTempColor.AddIcon(adapter.GetIcon("NULL"));
				}
			}
			o.switchType = 0;*/
			//return context;
		}




		//GET_CONTENT LIKE
		public static TempColorClass GET_CONTENT( HierarchyObject __o, HighlighterMod mod )
		{
			//if (!adapter.IS_PROJECT()) return INTERNAL_GetContent(__o);
			//else 
			return GetImageForObject_OnlyCacher( __o, mod );
		}
		static internal TempColorClass GetImageForObject_OnlyCacher( HierarchyObject o, HighlighterMod mod )
		{
			return GetImageForObject_OnlyCacher( o, false, mod );
		}

		internal static Dictionary<int, TempColorClass> new_perfomance_onlycaher_icons_sf = new Dictionary<int, TempColorClass>();
		internal static Dictionary<int, TempColorClass> new_perfomance_onlycaher_icons_if = new Dictionary<int, TempColorClass>();
		static TempColorClass __GetImageForObject_OnlyCacherTempColor = new TempColorClass();
		static TempColorClass __GetImageForObject_OnlyCacherTempColor_Empty = new TempColorClass().AddIcon(null);


		static internal TempColorClass GetImageForObject_OnlyCacher( HierarchyObject o, bool skipFilter, HighlighterMod mod )
		{

			var dic = skipFilter ? new_perfomance_onlycaher_icons_sf : new_perfomance_onlycaher_icons_if;

			if ( dic.ContainsKey( o.id ) ) return dic[ o.id ];
			//if (!o.Validate()) return null;
			//if (!IconImageCacher.ContainsKey(o.id))
			var __GetImageForObject_OnlyCacherTempColor = new TempColorClass();

			{
				bool tryToApplyilter = !TryToFindInternalIcon(o, mod);
				if ( tryToApplyilter )
				{
					if ( !skipFilter )
					{
						var filtered = mod.autoMod.GetFilter(o);

						if ( filtered != null )
						{
							//var result = new TempColorClass();
							filtered.OverrideToIcon( ref __GetImageForObject_OnlyCacherTempColor );
							//dic.Add(o.id, result);

							//return filtered;
						}
					}
					//dic.Add(o.id, __GetImageForObject_OnlyCacherTempColor_Empty);
					//if (! return __GetImageForObject_OnlyCacherTempColor_Empty;
				}
			}

			if ( mod.set.USE_MANUAL_HIGHLIGHTER_MOD )
			{
				var data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModManualIcons, o);
				if ( data != null && data.iconData.Length != 0 )
				{
					var icon = data.iconData[0].GetCheckedTexture();

					if ( o.Validate() )
					{
						var internalIcon = TODO_Tools.GetObjectBuildinIcon(o.go, Tools.unityGameObjectType).add_icon;// EditorGUIUtility.ObjectContent(o.go, Tools.unityGameObjectType).image;
						if ( internalIcon != icon )
						{
							if ( TryToFindInternalIcon( o, mod ) )
							{
								data.iconData[ 0 ].SetExternalCheckedTexture( internalIcon );
								//if (cached_colors.ContainsKey(value.GUIDsActiveGameObject_CheckAndGet)) cached_colors[value.GUIDsActiveGameObject_CheckAndGet] = EditorGUIUtility.ObjectContent(o.go, Tools.unityGameObjectType).image;
								__GetImageForObject_OnlyCacherTempColor.add_icon = internalIcon;
							}
						}
					}


					if ( icon )
					{
						Color32? ic = null;
						if ( data.iconData[ 0 ].has_icon_color ) ic = data.iconData[ 0 ].icon_color;
						__GetImageForObject_OnlyCacherTempColor.add_icon = icon;
						__GetImageForObject_OnlyCacherTempColor.add_hasiconcolor = ic.HasValue;
						if ( ic.HasValue ) __GetImageForObject_OnlyCacherTempColor.add_iconcolor = ic.Value;
					}



				}

			}


			dic.Add( o.id, __GetImageForObject_OnlyCacherTempColor );

			return __GetImageForObject_OnlyCacherTempColor;
		} //GetImageForObject_OnlyCacher

		/*
	Dictionary<GameObject, int> addIconChecherCache = new Dictionary<GameObject, int>();
	internal Dictionary<string, Texture> cached_colors = new Dictionary<string, Texture>();
	 //GetImageForObject_OnlyCacher
*/

		internal static bool HasKey( HierarchyObject o )
		{
			var data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModManualIcons, o);
			if ( data == null || data.iconData.Length == 0 ) return false;
			return data.iconData[ 0 ].icon_guid != null && data.iconData[ 0 ].icon_guid != "";
		}


		internal static Dictionary<int, TempColorClass> new_perfomance_includecaher_icons = new Dictionary<int, TempColorClass>();
		internal static TempColorClass ObjectContent_IncludeCacher( HierarchyObject o, Type type, HighlighterMod mod )
		{
			return ObjectContent_IncludeCacher( o, type, false, mod );
		}
		internal static TempColorClass ObjectContent_IncludeCacher( HierarchyObject o, Type type, bool includePrefab, HighlighterMod mod )
		//  internal static GUIContent ObjectContent(Adapter adapter, UnityEngine.Object o, Type type)
		{

			if ( adapter.NEW_PERFOMANCE && new_perfomance_includecaher_icons.ContainsKey( o.id ) ) return new_perfomance_includecaher_icons[ o.id ];



			//if (o.name == "SavedDataScripts")
			//{
			//	Debug.Log("ASD");
			//}

			var cacher = GetImageForObject_OnlyCacher(o, true, mod);

			if ( !cacher.add_icon ) //#-COLUP
			{
				//if (o.name == "Directional Light (2)")Debug.Log("ASD");
				var filtered = GetImageForObject_OnlyCacher(o, false, mod);
				if ( filtered != null ) cacher = filtered;
				//Debug.Log(cacher.add_hasiconcolor + " " + filtered.add_hasiconcolor + " " + o.name);
				//var filtered = adapter.ColorModule.GetFilter(adapter, o);
				//if (filtered != null) tempColorResult = filtered;
			}

			if ( adapter.NEW_PERFOMANCE )
			{
				var result = new TempColorClass();
				cacher.OverrideTo( ref result );
				new_perfomance_includecaher_icons.Add( o.id, result );
			}

			return new_perfomance_includecaher_icons[ o.id ];


			/*
			if (cacher.add_icon)
			{
				//return __ObjectContent_IncludeCacherTempColor2.AddIcon(  cacher.add_icon, cacher.add_hasiconcolor, cacher.add_iconcolor);
				if (adapter.NEW_PERFOMANCE)
				{
					var result = new Adapter.TempColorClass();
					cacher.OverrideTo(ref result);
					new_perfomance_includecaher_icons.Add(o.id, result);
				}
				return cacher;
			}


			o.cache_prefab = false;


			//if (adapter.IS_HIERARCHY())     // HIERARCHY
			{
				tempColorResult = __internal_ObjectContent(adapter, o != null ? o.GetHardLoadObject() : null, type);
			}
			//else       // PROJECT
			//{
			//	var id = o != null ? o.id : -1;
			//	var filtered2 = adapter.ColorModule.GetFilter(adapter, o);
			//	if (filtered2 != null) tempColorResult = filtered2;
			//	if (!cache_ObjectContent_byId.ContainsKey(id)) cache_ObjectContent_byId.Add(id, new Dictionary<int, Adapter.TempColorClass>());
			//	if (!cache_ObjectContent_byId[id].ContainsKey(o.id))
			//	{
			//		var icon = AssetDatabase.GetCachedIcon(o.project.assetPath);
			//		cache_ObjectContent_byId[id].Add(o.id, new Adapter.TempColorClass().AddIcon(icon));
			//	}
			//	tempColorResult = cache_ObjectContent_byId[id][o.id];
			//}

			//COLUP
			if (tempColorResult.add_icon != null)     // Debug.Log(context.name);
			{
				if (tempColorResult.add_icon == adapter.NullContext)
				{
					tempColorResult = adapter.__INTERNAL_TempColor_Empty;
				}
				else if (adapter.IS_HIERARCHY() && Utilities.IsPrefabIcon(tempColorResult.add_icon))
				{
					if (adapter.par.SHOW_PREFAB_ICON && includePrefab)
					{
						var prefab_root = adapter.FindPrefabRoot(o.go);
						// var prefab_src = PrefabUtility.GetPrefabParent(prefab_root);
						if (prefab_root != o.go) tempColorResult = adapter.__INTERNAL_TempColor_Empty;
						else     // cache_prefab = true;
						{
							o.cache_prefab = true;
						}
					}
					else
					{
						tempColorResult = adapter.__INTERNAL_TempColor_Empty;
					}
				}
			}

			if (!tempColorResult.add_icon) //#-COLUP
			{
				var filtered = adapter.ColorModule.GetFilter(adapter, o);
				if (filtered != null) tempColorResult = filtered;
			}

			if (adapter.NEW_PERFOMANCE)
			{
				var result = new Adapter.TempColorClass();
				result.el = new SingleList() { list = asdasd.ToList() };
				tempColorResult.OverrideTo(ref result);
				new_perfomance_includecaher_icons.Add(o.id, result);
			}

			return tempColorResult;
			*/
		}

























		static internal void SetImageColor_OnlyCache_MixedUndor( HierarchyObject _o, Color c, HighlighterMod mod )
		{


			TempSceneObjectPTR data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModManualIcons, _o);
			if ( data == null ) data = new TempSceneObjectPTR( _o.go, -1 );
			if ( data.iconData.Length < 1 )
			{
				Array.Resize( ref data.iconData, 1 );
				data.iconData[ 0 ] = new IconExternalData();
			}
			data.iconData[ 0 ].has_icon_color = !(c.r + c.g + c.b + c.a == 0 || c.r + c.g + c.b + c.a == 255 * 4);
			data.iconData[ 0 ].icon_color = c;
			HierarchyTempSceneDataGetter.SetUndoList( _o.scene );

			HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModManualIcons, _o, data );


			//HierarchyTempSceneDataGetter.SetDirtyList();

			mod.ClearByObject( _o );
			adapter.RepaintWindowInUpdate( mod.pluginID );
		} //GetImageForObject_OnlyCacher





		static internal void SetImageForObject_OnlyCacher( HierarchyObject _o, string guid, HighlighterMod mod )
		{


			if ( guid == null || guid == "" )
			{
				//list.RemoveAll(SetPair(_o));
				HierarchyTempSceneDataGetter.RemoveObjectData( SaverType.ModManualIcons, _o );
			}
			else
			{

				TempSceneObjectPTR data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModManualIcons, _o);
				if ( data == null ) data = new TempSceneObjectPTR( _o.go, -1 );
				if ( data.iconData.Length < 1 )
				{
					Array.Resize( ref data.iconData, 1 );
					data.iconData[ 0 ] = new IconExternalData();
				}
				data.iconData[ 0 ].SetExternalCheckedTexture( guid, AssetDatabase.GUIDToAssetPath( guid ) );
				HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModManualIcons, _o, data );

				/*	var value = new SingleList() { list = c.ToList() };
					var p = SetPair(_o);
					if (list.ContainsKey(p)) list[p] = value;
					else list.Add(p, value);*/
			}

			/*adapter.CreateUndoActiveDescription("Change Icon", (o.scene));
			if (string.IsNullOrEmpty(guid)) IconImageCacher.SetValue(null, o.scene, SetPair(o), true);
			else
				IconImageCacher.SetValue(
					new Int32List()
					{
						GUIDsActiveGameObject_CheckAndGet = guid,
						PATHsActiveGameObject = AssetDatabase.GUIDToAssetPath(guid)
					}, o.scene, SetPair(o), true);
			adapter.SetDirtyActiveDescription((o.scene));*/



			mod.ClearByObject( _o );
			adapter.RepaintWindowInUpdate( mod.pluginID );

		} //GetImageForObject_OnlyCacher
		internal static void SetImageForObject_OnlyCacher_2( HierarchyObject _o, Texture2D t )
		{


			if ( !t )
			{
				HierarchyTempSceneDataGetter.RemoveObjectData( SaverType.ModManualIcons, _o );
			}
			else
			{
				TempSceneObjectPTR data = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModManualIcons, _o);
				if ( data == null ) data = new TempSceneObjectPTR( _o.go, -1 );
				if ( data.iconData.Length < 1 )
				{
					Array.Resize( ref data.iconData, 1 );
					data.iconData[ 0 ] = new IconExternalData();
				}
				data.iconData[ 0 ].SetExternalCheckedTexture( t );
				HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModManualIcons, _o, data );

			}



		}




		static internal void SetIcon( HierarchyObject o, Texture2D texture, HighlighterMod mod )
		{
			HierarchyTempSceneDataGetter.SetUndoListStart( "Apply Icon" );
			if ( adapter.ha.SELECTED_GAMEOBJECTS().All( selO => selO != o ) )
			{
				HierarchyTempSceneDataGetter.SetUndoList( o.scene );
				adapter.RecordUndo( o, "Apply Icon" );
				____SetIcon( o, texture, mod );
				adapter.SetDirty( o );
			}
			else foreach ( var objectToUndo in adapter.ha.SELECTED_GAMEOBJECTS() )
				{
					HierarchyTempSceneDataGetter.SetUndoList( objectToUndo.scene );
					adapter.RecordUndo( o, "Apply Icon" );
					____SetIcon( objectToUndo, texture, mod );
					adapter.SetDirty( o );
				}
			mod.ClearCacheAdditional();
			adapter.RepaintAllViews();
			HierarchyTempSceneDataGetter.SetDirtyList();

		}

		static MethodInfo mi;

		static private void ____SetIcon( HierarchyObject o, Texture2D texture, HighlighterMod mod )
		{
			if ( !o.Validate() ) return;
			var SAVE_GUID = o.pluginID == 1;
			if ( mi == null )
			{
				var ty = typeof(EditorGUIUtility);
				mi = ty.GetMethod( "SetIconForObject", (BindingFlags)(-1) );
			}
			if ( SAVE_GUID )
			{
				if ( !texture )
				{
					//adapter.CreateUndoActiveDescription("Set Icon", (o.scene));
					SetImageForObject_OnlyCacher( o, null, mod );
					//adapter.SetDirtyActiveDescription((o.scene));
				}
				else
				{
					var path = AssetDatabase.GetAssetPath(texture);
					if ( !string.IsNullOrEmpty( path ) )
					{
						var guid = AssetDatabase.AssetPathToGUID(path);
						//adapter.CreateUndoActiveDescription("Set Icon", (o.scene));
						SetImageForObject_OnlyCacher( o, guid, mod );
						//adapter.SetDirtyActiveDescription((o.scene));
					}
				}
			}
			else
			{
				//var t = adapter.HO_TO_OBJECT(o);
				var t = o.go;
				if ( t )
				{
					//adapter.HO_RECORD_UNDO(o, "Set Icon");
					mi.Invoke( null, new object[] { t, texture } );
					bool setted = false;
					if ( texture && (texture.name.StartsWith( "sv_icon_dot" ) || texture.name.StartsWith( "sv_label_" )) )
					{
						if ( AssetDatabase.GetAssetPath( texture ).StartsWith( "Library/", StringComparison.OrdinalIgnoreCase ) )
						{
							SetImageForObject_OnlyCacher( o, "Library/" + texture.name, mod );
							setted = true;
						}
					}

					if ( !setted ) SetImageForObject_OnlyCacher_2( o, texture );
					//adapter.HO_SETDIRTY(o);
					//adapter.MarkSceneDirty(o.scene);
				}
			}
		}

		internal static void ____SetIconOnlyInternal( HierarchyObject o, Texture2D texture )
		{
			if ( !o.Validate() ) return;
			//if (adapter.IS_PROJECT()) return;
			if ( mi == null )
			{
				var ty = typeof(EditorGUIUtility);
				mi = ty.GetMethod( "SetIconForObject", (BindingFlags)(-1) );
			}
			var t = o.go;
			if ( t ) mi.Invoke( null, new object[] { t, texture } );
		}


		internal static void SetAction_MixedUndo( HierarchyObject o, Action<HierarchyObject> ac, string undoName ) //  var ty = typeof(EditorGUIUtility);
		{
			HierarchyTempSceneDataGetter.SetUndoListStart( undoName );

			if ( adapter.ha.SELECTED_GAMEOBJECTS().All( selO => selO != o ) )
			{
				//HierarchyTempSceneDataGetter.SetUndoList( o.scene );
				ac( o );
			}
			else foreach ( var objectToUndo in adapter.ha.SELECTED_GAMEOBJECTS() )
				{
					// HierarchyTempSceneDataGetter.SetUndoList( objectToUndo.scene );
					ac( objectToUndo );
				}
			HierarchyTempSceneDataGetter.SetDirtyList();
		}





	}
}
