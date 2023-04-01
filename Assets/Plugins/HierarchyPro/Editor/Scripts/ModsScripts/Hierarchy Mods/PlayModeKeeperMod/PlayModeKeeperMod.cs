#define DISABLE_PING
//#define DISABLE_ERROR


using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EMX.HierarchyPlugin.Editor.Mods.PlayModeSaver;
using UnityEngine.SceneManagement;

namespace EMX.HierarchyPlugin.Editor.Mods
{




	internal partial class PlayModeKeeperMod : DrawStackAdapter, ISearchable, IModSaver
	{



		PluginInstance adapter = null;
		internal string _SearchHelper = "Show 'GameObjects' whose 'Components' will be preserved in play mode";
		internal string HeaderText = "PlayMode Preserving";
		internal string ContextHelper = "PlayMode Preserving";//Data Keeper
		internal string HeaderTexture2D = "STORAGE_PASSIVE";

		//SEARCH
		public override bool callFromExternal() { return callFromExternal_objects != null; }
		public Windows.SearchWindow.FillterData_Inputs callFromExternal_objects { get; set; }
		public Type typeFillter { get; set; }
		public string SearchHelper { get { return _SearchHelper; } set { _SearchHelper = value; } }
		public virtual float GetInputWidth() { return -1; }
		//SEARCH
		internal Event EVENT { get { return callFromExternal() ? Event.current : adapter.EVENT; } }
		public void DrawSearch( Rect rect, HierarchyObject o )
		{
			_drawRect = rect;
			_drawRect.width = SI * 2;
			// _drawRect.x += _drawRect.width;
			firstRect = _drawRect;
			adapter.o = o;
			Draw();
		}

		internal override bool PERFOMANCE_BARS { get { return base.PERFOMANCE_BARS && !callFromExternal(); } }

		internal const int _K_ALL = 1, _K_SETACTIVE = 2, _K_SINBLING = 4 , _K_ADDREMOVECOMP = 8;
		internal class SetSpecialOptionsHelper
		{
			//internal bool IS_EMPTY {
			//    get {
			//        //return !(ALL ?? false) && !(SETACTIVE ?? false) && !(SINBLING ?? false) && !(ADDREMOVECOMP ?? false);
			//        // return !ALL.HasValue && !SETACTIVE.HasValue && !SINBLING.HasValue && !ADDREMOVECOMP.HasValue;
			//        return (ALL != true) && (SETACTIVE != true) && (SINBLING != true) && (ADDREMOVECOMP != true);
			//    }
			//}

			int _convert( bool? val, int mask, int previos )
			{
				if ( val.HasValue ) return val.Value ? mask : 0;
				return previos & mask;
			}

			internal int CONVERT( int prevois )
			{
				return
				   _convert( ALL, _K_ALL, prevois ) | _convert( SETACTIVE, _K_SETACTIVE, prevois ) |
				   _convert( SINBLING, _K_SINBLING, prevois ) | _convert( ADDREMOVECOMP, _K_ADDREMOVECOMP, prevois );
				//                 return
				//  (ALL == true ? _K_ALL : (prevois & _K_ALL)) |
				//  (SETACTIVE == true ? _K_SETACTIVE : (prevois & _K_SETACTIVE)) |
				//  (SINBLING == true ? _K_SINBLING : (prevois & _K_SINBLING)) |
				//  (ADDREMOVECOMP == true ? _K_ADDREMOVECOMP : (prevois & _K_ADDREMOVECOMP));
			}

			internal bool? ALL = null;
			internal bool? SETACTIVE = null;
			internal bool? SINBLING = null;
			internal bool? ADDREMOVECOMP = null;
			static SetSpecialOptionsHelper _get = new SetSpecialOptionsHelper();
			internal static SetSpecialOptionsHelper GET {
				get {
					if ( _get.ALL.HasValue ) _get.ALL = null;
					if ( _get.SETACTIVE.HasValue ) _get.SETACTIVE = null;
					if ( _get.SINBLING.HasValue ) _get.SINBLING = null;
					if ( _get.ADDREMOVECOMP.HasValue ) _get.ADDREMOVECOMP = null;
					return _get;
				}
			}

			internal static SetSpecialOptionsHelper GET_FROM_LIST( List<int> getted )
			{
				_get.ALL = (getted[ 0 ] & _K_ALL) != 0 ? true : (bool?)null;
				_get.SETACTIVE = (getted[ 0 ] & _K_SETACTIVE) != 0 ? true : (bool?)null;
				_get.SINBLING = (getted[ 0 ] & _K_SINBLING) != 0 ? true : (bool?)null;
				_get.ADDREMOVECOMP = (getted[ 0 ] & _K_ADDREMOVECOMP) != 0 ? true : (bool?)null;
				return _get;
			}
		}

		// internal static ObjectCacheHelper<GameObject, SingleList> DataKeeperCache;
		internal PlayModeKeeperMod( int pid ) : base( pid )
		{

			// DataKeeperCache = new ObjectCacheHelper<GameObject, SingleList>( property => property.GetHash9(), property => property.GetHash10(), Adapter.CacherType.KeeperData, adapter, "DataKeeperCache" );
			adapter = Root.p[ pid ];
			if ( !adapter.par_e.PLAYMODESAVER_TEMP_SKIP_INIT )
			{
				adapter.par_e.PLAYMODESAVER_TEMP_WERE_PLAYED = false;
				adapter.par_e.PLAYMODESAVER_TEMP_WERE_LAST_SAVED = false;
			}
			if ( Application.isPlaying ) adapter.par_e.PLAYMODESAVER_TEMP_SKIP_INIT = false;
		}

		internal void Subscribe( EditorSubscriber sbs )
		{
			sbs.OnPlayModeStateChanged += PlaymodeStateChanged;
			sbs.BuildedOnGUI_first.Add( PreCalcRect );
			//sbs.BuildedOnGUI_middle += prepare_draw_inhierarchy;
			sbs.AddBuildedOnGUI_middle( prepare_draw_inhierarchy );
			sbs.saveModsInterator.Add( this );
		}


		List<SaverType> _GetSaverTypes = new List<SaverType>() { SaverType.ModPlayKeeper };
		List<SaverType> IModSaver.GetSaverTypes { get { return _GetSaverTypes; } }
		bool IModSaver.SaveToString( HierarchyObject o, ref string result )
		{
			var has = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModPlayKeeper, o);
			if ( has != null && has.HasList() )
			{
				var list = has.GetIntList();
				int firstValue = list[ 0 ];
				if ( firstValue < 0 ) throw new Exception( "PlayModeKeeper firstValue < 0" );
				ulong resultKeeper = 0;
				if ( (firstValue & _K_ALL) == 0 )
				{
					///  var comps = HierarchyExtensions.Utilities.GetComponentFast<Component>.GetAll(o.go).Select(c => c.GetInstanceID()).ToList();
					var comps = o.GetComponents().Select(c => c.GetInstanceID()).ToList();
					for ( int i = 1; i < list.Count; i++ )
					{
						var ind = comps.IndexOf(list[i]);
						if ( ind < 0 ) continue;
						if ( ind >= 56 ) continue;
						resultKeeper |= ((ulong)1) << ind;
					}
				}

				if ( resultKeeper != 0 || firstValue != 0 )
				{
					//  if ( !string.IsNullOrEmpty( buildString ) ) buildString += "⅜";
					// buildString += "[K]" + resultKeeper;
					result = firstValue.ToString() + "_" + resultKeeper.ToString();
					return true;
				}
			}
			return false;
		}

		bool IModSaver.LoadFromString( string res, HierarchyObject o )
		{
			var vals = res.Split('_');
			int firstValue;
			ulong resultKeeper;
			if ( !int.TryParse( vals[ 0 ], out firstValue ) ) throw new Exception( "PlayModeKeeper !TryParse firstValue" );
			if ( !ulong.TryParse( vals[ 1 ], out resultKeeper ) ) throw new Exception( "PlayModeKeeper !resultKeeper firstValue" );


			//if ( ulong.TryParse( res, out resultKeeper ) )
			{
				if ( firstValue != 0 && resultKeeper == 0 )
				{
					//Hierarchy.M_PlayModeKeeper.DataKeeperCache.SetValue( new SingleList() { list = new[] { 1 }.ToList() }, o.go.scene.GetHashCode(), o.go, true );
					oneList[ 0 ] = firstValue;
					return HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModPlayKeeper, o, oneList, true );
				}
				else
				{
					//  var comps = HierarchyExtensions.Utilities.GetComponentFast<Component>.GetAll(o.go).Select(c => c.GetInstanceID()).ToList();
					var comps = o.GetComponents().Select(c => c.GetInstanceID()).ToArray();
					List<int> tempList = new[] { firstValue }.ToList();

					for ( int i = 0; i < 56; i++ )
					{
						if ( i >= comps.Length ) break;
						if ( (resultKeeper & ((ulong)1 << i)) != 0 )
						{
							tempList.Add( comps[ i ] );
						}
					}
					// Hierarchy.M_PlayModeKeeper.DataKeeperCache.SetValue( new SingleList() { list = tempList }, o.go.scene.GetHashCode(), o.go, true );
					return HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModPlayKeeper, o, tempList, true );
				}
			}
			//return false;
		}
		void prepare_draw_inhierarchy()
		{
			callFromExternal_objects = null;
			Draw();
			// if ( CURRENT_STACK != null ) throw new Exception( "Cache not finalizing" );
			if ( CURRENT_STACK != null ) Root.TemperaryDisableThePlugin_FromCache();
		}
		private void PlaymodeStateChanged()
		{
			if ( !Application.isPlaying ) adapter.par_e.PLAYMODESAVER_TEMP_SKIP_INIT = true;
			// MonoBehaviour.print(Application.isPlaying + " " + adapter.par_e.PLAYMODESAVER_TEMP_WERE_LAST_SAVED);
			if ( !Application.isPlaying )
			{
				if ( adapter.par_e.PLAYMODESAVER_TEMP_WERE_PLAYED ) //
				{
					//MonoBehaviour.print("stop Load()");
					adapter.par_e.PLAYMODESAVER_TEMP_WERE_LAST_SAVED = false;
					adapter.par_e.PLAYMODESAVER_TEMP_WERE_PLAYED = false;
					Load();
				}
				else
				{
					//MonoBehaviour.print("stop SaveLast()");
					SaveLast();
					adapter.par_e.PLAYMODESAVER_TEMP_WERE_LAST_SAVED = true;
				}
			}

			if ( Application.isPlaying ) // 
			{
				//MonoBehaviour.print("isPlaying | PLAYMODESAVER_TEMP_WERE_LAST_SAVED=" + adapter.par_e.PLAYMODESAVER_TEMP_WERE_LAST_SAVED);
				if ( !adapter.par_e.PLAYMODESAVER_TEMP_WERE_LAST_SAVED ) SaveLast();
				if ( adapter.par_e.PLAYMODESAVER_TEMP_WERE_PLAYED ) SaveCurrent();
				adapter.par_e.PLAYMODESAVER_TEMP_WERE_LAST_SAVED = true;
				adapter.par_e.PLAYMODESAVER_TEMP_WERE_PLAYED = true;
			}
		}


		static KeeperDataUnityJsonData ToJson( UnityEngine.Object obj )
		{
			var jsonData = new KeeperDataUnityJsonData();
			jsonData.default_json = EditorJsonUtility.ToJson( obj );
			if ( Root.p[ 0 ].par_e.PLAYMODESAVER_SAVE_UNITYOBJECT )
			{
				var f = Tools.GET_FIELDS_AND_VALUES(obj, obj.GetType());
				jsonData.fields_name = new string[ f.Length ];
				// f.Keys.CopyTo( jsonData.fields_name , 0 );
				/*jsonData.fields_new_value = new long[f.Count];
                for ( int i = 0 ; i < jsonData.fields_name.Length ; i++ ) {
                    var v = ((UnityEngine.Object)f[jsonData.fields_name[i]].GetValue(obj));
                    jsonData.fields_value[i] = v ? v.GetInstanceID() : -1;
                }*/

				jsonData.fields_new_value = new KeeperDataFieldValue[ f.Length ];
				for ( int i = 0; i < jsonData.fields_name.Length; i++ )
				{ //var v = ((UnityEngine.Object)f[jsonData.fields_name[i]].GetValue(obj));
					jsonData.fields_name[ i ] = f[ i ].Key;
					var v = f[i].Value.Value as UnityEngine.Object;
					jsonData.fields_new_value[ i ] = new KeeperDataFieldValue() { FILEID = v ? v.GetInstanceID() : -1 };
				}
			}

			return jsonData;
		}

		static void FromJsonOverwrite( KeeperDataUnityJsonData jsonData, UnityEngine.Object obj ) //  if (json.Length != 2) return;
		{
			if ( jsonData == null ) return;
			/*  MonoBehaviour.print(obj);
              MonoBehaviour.print( jsonData );
              MonoBehaviour.print( jsonData.default_json);*/
			if ( !string.IsNullOrEmpty( jsonData.default_json ) )
			{
				var fff = Tools.GET_FIELDS(obj.GetType());
				var f = fff.Values.Select(_f => new { _f, value = _f.GetAllValues(obj, 0, 0) }).ToArray();
				EditorJsonUtility.FromJsonOverwrite( jsonData.default_json, obj );
				foreach ( var item in f )
				{
					item._f.SetAllValues( obj, item.value );
				}
			}

			if ( jsonData.fields_name != null ) //MonoBehaviour.print(obj);
			{ //Debug.Log( "ASD" );
				var fff = Tools.GET_FIELDS_AND_VALUES(obj, obj.GetType(), searchVals: 4); //.ToDictionary(v=>v.Key, v=>v.Value)
				Dictionary<Tools.FieldAdapter, Dictionary<string, object>> result = new Dictionary<Tools.FieldAdapter, Dictionary<string, object>>();
				foreach ( var item in fff )
				{
					if ( !result.ContainsKey( item.Value.Key ) ) result.Add( item.Value.Key, new Dictionary<string, object>() );
					result[ item.Value.Key ].Add( item.Key, item.Value.Value );
				}

				foreach ( var item in result )
				{
					for ( int i = 0; i < jsonData.fields_name.Length; i++ )
					{
						if ( !item.Value.ContainsKey( jsonData.fields_name[ i ] ) /*|| !Adapter.unityObjectType.IsAssignableFrom( f[jsonData.fields_name[i]].FieldType )*/) continue;
						if ( jsonData.fields_new_value[ i ].FILEID == -1 ) item.Value[ jsonData.fields_name[ i ] ] = null;
						else
						{
							var newV = PlayModeKeeperMod.GET_ID((int)jsonData.fields_new_value[i].FILEID, m_OLD_NEW);
							if ( newV ) item.Value[ jsonData.fields_name[ i ] ] = newV;
						}
					}

					item.Key.SetAllValues( obj, item.Value );
				}

				/* var f = Adapter.GET_FIELDS(obj.GetType());
                 for ( int i = 0 ; i < jsonData.fields_name.Length ; i++ ) {
                     if ( !f.ContainsKey( jsonData.fields_name[i] )/ * || !Adapter.unityObjectType.IsAssignableFrom( f[jsonData.fields_name[i]].FieldType )* /) continue;
                     if ( jsonData.fields_value[i] == -1 ) f[jsonData.fields_name[i]].SetValue( obj , (UnityEngine.Object)null );
                     else f[jsonData.fields_name[i]].SetValue( obj , M_PlayModeKeeper.GET_ID( jsonData.fields_value[i] , m_OLD_NEW ) );
                 }*/
			}
		}









		// static bool adapter.par_e.PLAYMODESAVER_TEMP_SKIP_INIT = false;





		const int POS = 2000;

		[MenuItem( "CONTEXT/Component/Remove Component from 'PlayMode Keeper'", true, POS + 1 )]
		public static bool STATIC_REMOVE_VALID( MenuCommand menuCommand )
		{
			if ( !Root.p[ 0 ].par_e.USE_PLAYMODE_SAVER_MOD ) return false;

			var comp = menuCommand.context as Component;
			if ( !comp ) return false;

			/* foreach (var gameObject in SELECTED_GAMEOBJECTS())
             {
                 if (!gameObject.scene.IsValid()) continue;
                 var getted = DataKeeperCache.GetValue(gameObject.scene, gameObject);
                 if (getted != null && getted.list.Count > 0 && getted.list[0] == 1) return true;
             }*/

			if ( !comp.gameObject.scene.IsValid() ) return false;
			var getted = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModPlayKeeper, Cache.GetHierarchyObjectByInstanceID(comp.gameObject));
			if ( getted != null && getted.HasList() && getted.GetIntList().Contains( comp.GetInstanceID() ) )
			{
				var flags = getted.GetIntList()[0];
				if ( (flags & _K_ALL) != 0 ) return false;
				return true;
			}


			return false;
		}

		[MenuItem( "CONTEXT/Component/Remove Component from 'PlayMode Keeper'", false, POS + 1 )]
		public static void STATIC_REMOVE( MenuCommand menuCommand )
		{

			var comp = menuCommand.context as Component;
			if ( !comp ) return;

			/* foreach (var gameObject in SELECTED_GAMEOBJECTS())
             {
                 var s = gameObject.scene;
                 if (!s.IsValid()) continue;
                 if (DISABLE_DESCRIPTION(s)) continue;
                 var d = M_Descript.des(s);
                 if (d == null) return;
                 Undo.RecordObject(d.component, "Remove Selected Objects from 'PlayMode Data Keeper'");
                 ((M_PlayModeKeeper)modules.First(m => m is M_PlayModeKeeper)).SetValue(gameObject, false, null);
             }*/

			var s = comp.gameObject.scene;
			if ( !s.IsValid() ) return;

			//  if ( Hierarchy.HierarchyAdapterInstance.DISABLE_DESCRIPTION( s ) ) return;



			//  var d = M_Descript.des(s.GetHashCode());
			//  if ( d == null ) return;
			//  Hierarchy.HierarchyAdapterInstance.SET_UNDO( d, "Remove Selected Component from 'PlayMode Data Keeper'" );

			var getted = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModPlayKeeper, Cache.GetHierarchyObjectByInstanceID(comp.gameObject));
			List<int> list;
			if ( getted == null || !getted.HasList() || getted.GetIntList().Count == 0 ) list = new List<int>( 1 ) { 0 };
			else list = getted.GetIntList();
			list.Remove( comp.GetInstanceID() );
			list.RemoveAt( 0 );

			var values = SetSpecialOptionsHelper.GET;
			values.ALL = false;

			Root.p[ 0 ].modsController.playModeKeeperMod.SetValueMulty( comp.gameObject, GET_HOBEJCTS( comp.gameObject ), values, list.ToArray() );
			Root.p[ 0 ].RepaintWindowInUpdate( 0 );
			//Hierarchy.HierarchyAdapterInstance.RepaintWindowInUpdate();
		}

		[MenuItem( "CONTEXT/Component/Add Component to 'PlayMode Keeper'", true, POS )]
		public static bool STATIC_ADD_VALID( MenuCommand menuCommand )
		{
			if ( !Root.p[ 0 ].par_e.USE_PLAYMODE_SAVER_MOD ) return false;
			//  if ( !Hierarchy.HierarchyAdapterInstance.ENABLE_RIGHTDOCK_PROPERTY || !Hierarchy.HierarchyAdapterInstance.par.DataKeeperParams.ENABLE ) return false;

			var comp = menuCommand.context as Component;
			if ( !comp ) return false;


			/*  foreach (var gameObject in SELECTED_GAMEOBJECTS())
              {
                  if (!gameObject.scene.IsValid()) continue;
                  var getted = DataKeeperCache.GetValue(gameObject.scene, gameObject);
                  if (getted == null || getted.list.Count == 0 || getted.list[0] != 1) return true;
              }*/

			if ( !comp.gameObject.scene.IsValid() ) return false;

			var getted = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModPlayKeeper, Cache.GetHierarchyObjectByInstanceID(comp.gameObject));
			// if ( getted == null || !getted.HasList() || !getted.GetIntList().Contains( comp.GetInstanceID() ) ) return true;


			if ( getted == null || !getted.HasList() || !getted.GetIntList().Contains( comp.GetInstanceID() ) )
			{
				if ( getted != null && getted.HasList() && getted.GetIntList().Count > 0 )
				{
					var flags = getted.GetIntList()[0];
					if ( (flags & _K_ALL) != 0 ) return false;
				}
				return true;
			}
			//   var getted = DataKeeperCache.GetValue(comp.gameObject.scene, Cache.GetHierarchyObjectByInstanceID(comp.gameObject));
			//   if ( getted == null || getted.list.Count == 0 || !getted.list.Contains( comp.GetInstanceID() ) ) return true;


			return false;
		}

		[MenuItem( "CONTEXT/Component/Add Component to 'PlayMode Keeper'", false, POS )]
		public static void STATIC_ADD( MenuCommand menuCommand )
		{
			var comp = menuCommand.context as Component;
			if ( !comp ) return;
			/* foreach (var gameObject in SELECTED_GAMEOBJECTS())
             {
                 var s = gameObject.scene;
                 if (!s.IsValid()) continue;
                 if (DISABLE_DESCRIPTION(s)) continue;
                 var d = M_Descript.des(s);
                 if (d == null) return;
                 Undo.RecordObject(d.component, "Add Selected Component to 'PlayMode Data Keeper'");
                 ((M_PlayModeKeeper)modules.First(m => m is M_PlayModeKeeper)).SetValue(gameObject, true, null);
             }*/


			var s = comp.gameObject.scene;
			if ( !s.IsValid() ) return;
			//  if ( Hierarchy.HierarchyAdapterInstance.DISABLE_DESCRIPTION( s ) ) return;
			//  var d = M_Descript.des(s.GetHashCode());
			//  if ( d == null ) return;
			//  Hierarchy.HierarchyAdapterInstance.SET_UNDO( d, "Add Selected Component to 'PlayMode Data Keeper'" );

			// var getted = DataKeeperCache.GetValue(comp.gameObject.scene, Hierarchy.HierarchyAdapterInstance.GetHierarchyObjectByInstanceID(comp.gameObject));
			// if ( getted == null ) getted = new SingleList() { list = new List<int>( 1 ) { 0 } };
			var getted = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModPlayKeeper, Cache.GetHierarchyObjectByInstanceID(comp.gameObject));
			List<int> list;
			if ( getted == null || !getted.HasList() || getted.GetIntList().Count == 0 ) list = new List<int>( 1 ) { 0 };
			else list = getted.GetIntList();

			if ( !list.Contains( comp.GetInstanceID() ) ) list.Add( comp.GetInstanceID() );
			list.RemoveAt( 0 );

			//  ((PlayModeKeeperMod)Hierarchy.HierarchyAdapterInstance.modules.First( m => m is PlayModeKeeperMod )).SetValue( comp.gameObject, false, getted.list.ToArray() );

			//  Hierarchy.HierarchyAdapterInstance.RepaintWindowInUpdate();
			var values = SetSpecialOptionsHelper.GET;
			values.ALL = false;
			// Root.p[ 0 ].modsController.playModeKeeperMod.SetValue_( comp.gameObject, values, list.ToArray(), true, true );
			Root.p[ 0 ].modsController.playModeKeeperMod.SetValueMulty( comp.gameObject, GET_HOBEJCTS( comp.gameObject ), values, list.ToArray() );
			Root.p[ 0 ].RepaintWindowInUpdate( 0 );
		}


		static Dictionary<int, int> m_EMPTY = new Dictionary<int, int>();
		static Dictionary<int, int> m_NEW_OLD = new Dictionary<int, int>();
		static Dictionary<int, int> m_OLD_NEW = new Dictionary<int, int>();



		static int gameObject_ID;
		static int comp_ID;

		internal static void RECORD_FLUSH( KeeperData source, Component comp, bool write_skip ) /* RECORD */
		{
			gameObject_ID = comp.gameObject.GetInstanceID();
			comp_ID = comp.GetInstanceID();
			if ( !source.comp_to_Type.ContainsKey( comp_ID ) ) //  MonoBehaviour.print(comp.GetType().FullName);
			{
				source.comp_to_Type.Add( comp_ID, comp.GetType().FullName );
			}

			if ( !source.field_records.ContainsKey( gameObject_ID ) )
				source.field_records.Add( gameObject_ID, new KeeperDataItem() {
					GameObject_ParentID = comp.gameObject.transform.parent ? comp.gameObject.transform.parent.gameObject.GetInstanceID() : -1,
					GameObject_SiblingPos = comp.gameObject.transform.GetSiblingIndex(),
					GameObject_Name = comp.gameObject.name,
					GameObject_Active = comp.gameObject.activeSelf,
					records = new Dictionary<int, KeeperDataUnityJsonData>(),
					SkipCreateObject = write_skip
				} );
			if ( !write_skip && source.field_records[ gameObject_ID ].SkipCreateObject ) source.field_records[ gameObject_ID ].SkipCreateObject = false;
			if ( !source.field_records[ gameObject_ID ].records.ContainsKey( comp_ID ) ) //    if (comp is PrefabLibrary)   MonoBehaviour.print(((PrefabLibrary)comp).Explosiont_Prefab);
			{
				source.field_records[ gameObject_ID ].records.Add( comp_ID, ToJson( comp ) );
				/* source.prop_records.Add(id, new Dictionary<PropertyInfo, object>());
                 //   MonoBehaviour.print(comp.GetType() + " " + comp.GetType().GetFields(FieldFlags).Length);
                
                 var type = comp.GetType();
                 if (!baked_types.ContainsKey(type.Name)) baked_types.Add(type.Name,
                      new BakedFields() {
                          fields = type.GetFields(FieldFlags).Where(field => field.IsPublic || field.GetCustomAttributes(false).Length != 0).ToArray(),
                          props = type.GetProperties(PropertyFlags).Where(p => p.CanRead && p.CanWrite).ToArray(),
                      });
                 var baked = baked_types[type.Name];
                 foreach (var field in baked.fields) source.field_records[id].Add(field, field.GetValue(comp));
                 foreach (var prop in baked.props) source.prop_records[id].Add(prop, prop.GetValue(comp, null));*/
			}
			/* RECORD */


			/*
                            Editor.
                             editor = Editor.CreateEditor(activeGO.transform);
            
                            SerializedObject so = new SerializedObject(Selection.activeGameObject.GetComponent<Renderer>());
            
                            so.FindProperty("m_ScaleInLightmap").cop
                            so.ApplyModifiedProperties();
                            new SerializedObject()
                            so.*/
		}




		string flush_current()
		{
			if ( !adapter.par_e.USE_PLAYMODE_SAVER_MOD ) return "";

			var result = new KeeperData();
			if ( adapter.par_e.PLAYMODESAVER_SAVE_USE_PERMANENT_LIST_OF_MONOSCRIPTS )
			{

				foreach ( var mDataKeeperValue in HierarchyCommonData.Instance().GetPlayModeSaverPreservedScriptList() )
				{
					if ( !mDataKeeperValue.Value || mDataKeeperValue.Value.GetClass() == null ) continue;
					foreach ( var finded in Resources.FindObjectsOfTypeAll( mDataKeeperValue.Value.GetClass() ) )
					{
						var comp = finded as Component;
						if ( !comp || !comp.gameObject || !comp.gameObject.scene.IsValid() ) continue;
						/* RECORD */
						RECORD_FLUSH( result, comp, true );
						/* if (!result.records.ContainsKey(comp.GetInstanceID()))
                         {
                             var id = comp.GetInstanceID();
                             result.records.Add(id, new Dictionary<FieldInfo, object>());
                             foreach (var field in comp.GetType().GetFields(flags))
                             {
                                 if (field.IsPublic || field.GetCustomAttributes(false).Length != 0)
                                 {
                                     result.records[id].Add(field, field.GetValue(comp));
                                 }
                             }
                        
                         }*/
						/* RECORD */
					}
				}
			}
			List<int> removeList = new List<int>();
			for ( int sce = 0, L = UnityEditor.SceneManagement.EditorSceneManager.sceneCount; sce < L; sce++ )
			{

				var scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(sce);
				if ( !scene.IsValid() || !scene.isLoaded ) continue;

				Dictionary<int, TempSceneObjectPTR> all = HierarchyTempSceneData.GetAllObjectData(SaverType.ModPlayKeeper, scene);

				//  foreach ( var cache in all )
				//  {
				//     var s = cache.Key;
				if ( removeList.Count != 0 ) removeList.Clear();

				//List<int> removeList = new List<int>();
				// foreach ( var obj in cache.Value )
				foreach ( var obj in all )
				{
					//  var o = EditorUtility.InstanceIDToObject(obj.Key.id) as GameObject;
					//  if ( !o || !o.scene.IsValid() ) continue;
					// MonoBehaviour.print("ASD");
					if ( !obj.Value.target ) continue;

					//  var getted = HierarchyTempSceneDataGetter.GetObjectData( SaverType.ModPlayKeeper, Cache.GetHierarchyObjectByInstanceID(o));
					if ( !obj.Value.HasList() )
					{
						removeList.Add( obj.Key );
						continue;
					}
					var list = obj.Value.GetIntList();
					//   var getted = DataKeeperCache.GetValue(s, Cache.GetHierarchyObjectByInstanceID(o));
					//    if ( getted == null || getted.list.Count == 0 ) continue;

					//EMX_TODO - removeList SHOULD CONTAIN PREVIS VALUES / NOW EVERY OBJECT WILL BE REMOVED
					if ( obj.Value.boolValue ) removeList.Add( obj.Key );

					// bool wasWRite = false;
					if ( list[ 0 ] > 0 )
					{
						foreach ( var comp in obj.Value.target.GetComponents<Component>() )
						{
							if ( !comp ) continue;
							RECORD_FLUSH( result, comp, false );
							// wasWRite = true;
						}

						//if ( wasWRite ) result.field_records[/* obj.Key.id */obj.Key ].ALL = true;
						result.field_records[/* obj.Key.id */obj.Key ].SPECIAL_OPTIONS = list[ 0 ];
					}
					else
					{
						for ( int i = 1; i < list.Count; i++ )
						{
							var comp = EditorUtility.InstanceIDToObject(list[i]) as Component;
							if ( !comp ) continue;
							RECORD_FLUSH( result, comp, false );
							//wasWRite = true;
						}


						// if ( wasWRite ) result.field_records[ /*obj.Key.id*/obj.Key ].ALL = false;

						//else removeList.Add(obj.Key);
						// result.ALL = false;
					}
				}

				/*if (removeList.Count != 0)
					foreach (var i in removeList)
					{
						all.Remove(i);
					}*/
				//   }
				/*if (removeList.Count != 0)
				{
					foreach (var r in removeList)
					{
						all.Remove(r);
					}
					HierarchyTempSceneDataGetter.SetAllObjectDataAndSave(SaverType.ModPlayKeeper, scene, all, false);
				}*/
			}

			return Serializer.SERIALIZE_SINGLE( result );
		}




		void ClearTemp()
		{
			List<int> removeList = new List<int>();
			for ( int sce = 0, L = UnityEditor.SceneManagement.EditorSceneManager.sceneCount; sce < L; sce++ )
			{

				var scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(sce);
				if ( !scene.IsValid() || !scene.isLoaded ) continue;
				Dictionary<int, TempSceneObjectPTR> all = HierarchyTempSceneData.GetAllObjectData(SaverType.ModPlayKeeper, scene);
				if ( removeList.Count != 0 ) removeList.Clear();

				foreach ( var obj in all )
				{
					if ( !obj.Value.target ) continue;
					if ( !obj.Value.HasList() )
					{
						removeList.Add( obj.Key );
						continue;
					}
					var list = obj.Value.GetIntList();
					if ( obj.Value.boolValue ) removeList.Add( obj.Key );
				}
				if ( removeList.Count != 0 )
				{
					foreach ( var r in removeList )
					{
						all.Remove( r );
					}
					HierarchyTempSceneDataGetter.SetAllObjectDataAndSave( SaverType.ModPlayKeeper, scene, all, false );
				}
			}

		}



		void SaveLast()
		{
#if LOG
            MonoBehaviour.print("SaveLast");
#endif
			/*  MonoBehaviour.print("Save");*/
			var flush = flush_current();
			/*if (editMode)
            {
                editMode = false;*/
			SessionState.SetString( "EMX|Data Keeper|Last State", flush );
			//  }
			// SessionState.SetString("EMX|Data Keeper|Current State", flush);
		}

		void SaveCurrent()
		{
#if LOG
            MonoBehaviour.print("SaveCurrent");
#endif
			/*  MonoBehaviour.print("Save");*/
			var flush = flush_current();
			/*   if (editMode)
               {
                   editMode = false;
                   SessionState.SetString("EMX|Data Keeper|Last State", flush);
               }*/
			SessionState.SetString( "EMX|Data Keeper|Current State", flush );
		}

		List<Component> writeChanges = new List<Component>();
		int OB_ID = 0;
		int COMPID = 0;

		/*  static UnityEngine.Object GET_OBJECT_BYID(int id)
          {
              if (m_NEW_OLD.ContainsKey(id)) return EditorUtility.Instance IDToObject(m_NEW_OLD[id]);
              return EditorUtility.Instance IDToObject(id);
          }*/


		void Load()
		{
#if LOG
            MonoBehaviour.print("Load");
#endif
			var last = SessionState.GetString("EMX|Data Keeper|Last State", "");
			var current = SessionState.GetString("EMX|Data Keeper|Current State", "");
			SessionState.SetString( "EMX|Data Keeper|Current State", "" );

			if ( !string.IsNullOrEmpty( last ) )
			{
#if !DISABLE_ERROR
				try
#endif
				{
					last_state = Serializer.DESERIALIZE_SINGLE<KeeperData>( last );
				}
#if !DISABLE_ERROR
				catch (Exception ex){ 
				Debug.LogWarning( "Hierarchy PlayMode Keeper - Preserve Last Error!+\n\n" + ex.Message + "\n\n" + ex.StackTrace );
					last_state = null;
				}
#endif
				// last_state = (KeeperData)DESERIALIZE_SINGLE(last, formatter);
				/*                    try { last_state = (KeeperData)DESERIALIZE_SINGLE(last, formatter); }
                                    catch { last_state = new KeeperData(); }*/
			}

			if ( string.IsNullOrEmpty( current ) ) return;

#if !DISABLE_ERROR
			try
			{
#endif
				current_state = Serializer.DESERIALIZE_SINGLE<KeeperData>( current );
#if !DISABLE_ERROR
			}
			catch (Exception ex) {
				Debug.LogWarning( "Hierarchy PlayMode Keeper - Preserve Current Error!+\n\n" + ex.Message + "\n\n" + ex.StackTrace );
				current_state = null;
			}
#endif
			m_NEW_OLD.Clear();
			m_OLD_NEW.Clear();

			if ( current_state != null )
			{
				List<Scene> scenes = new List<Scene>();
				//  var use_add_remove = ;


				Dictionary<GameObject, KeeperDataItem> hierarchychanges = new Dictionary<GameObject, KeeperDataItem>();

				//CREATE GAMEOBJECT
				TRY_CREATE_OBJECTS( true, current_state, "Perform PlayMode Data Keeper", ref hierarchychanges );
				//CREATE GAMEOBJECT

				var addRemoveCompPref = adapter.par_e.PLAYMODESAVER_USE_ADD_REMOVE_COMPONENTS;
				foreach ( var obValue in current_state.field_records )
				{
					var o = GET_ID(obValue.Key, m_OLD_NEW) as GameObject;
					var changes = false;
					changes = DO_CHANGES( o, obValue.Key, addRemoveCompPref && obValue.Value.SAVE_COMPONENTS, current_state, last_state, "Perform PlayMode Data Keeper", ref hierarchychanges, false );
					if ( changes && !scenes.Contains( o.scene ) ) scenes.Add( o.scene );
				}

				foreach ( var obValue in current_state.field_records )
				{
					var o = GET_ID(obValue.Key, m_OLD_NEW) as GameObject;
					var changes = false;
					changes = DO_CHANGES( o, obValue.Key, addRemoveCompPref && obValue.Value.SAVE_COMPONENTS, current_state, last_state, "Perform PlayMode Data Keeper", ref hierarchychanges, true );
					if ( changes && !scenes.Contains( o.scene ) ) scenes.Add( o.scene );
				}

				CHACNGE_HIERARCHY( hierarchychanges, "Perform PlayMode Data Keeper", true );


				current_state_back = current_state;
				current_state = null;

				foreach ( var scene in scenes )
				{
					adapter.MarkSceneDirty( scene );
				}

				writeChanges.Clear();
			}
			ClearTemp();
		}

		KeeperData current_state_back;
		int[] keysArray = new int[20];
		int inter;

		KeeperDataUnityJsonData kpLast;
		/* List<int> removearray = new List<int>();
         List<int> addarray = new List<int>();*/


		static UnityEngine.Object GET_ID( int _id, Dictionary<int, int> id_translater )
		{
			var id = (int)_id;
			if ( id_translater.ContainsKey( id ) ) return EditorUtility.InstanceIDToObject( id_translater[ id ] );
			return EditorUtility.InstanceIDToObject( id );
		}

		static Dictionary<GameObject, int> createdObject = new Dictionary<GameObject, int>();
		static bool wasCreated;

		void TRY_CREATE_OBJECTS( bool WRITE_OLD_NEW, KeeperData source, string UNDO_TEXT, ref Dictionary<GameObject, KeeperDataItem> hierarchychanges )
		{
			wasCreated = false;
			foreach ( var obValue in source.field_records )
			{
				if ( obValue.Value.SkipCreateObject ) continue;

				var o = EditorUtility.InstanceIDToObject(obValue.Key) as GameObject;

				if ( !o )
				{
					if ( !wasCreated ) createdObject.Clear();
					wasCreated = true;
					var ob = new GameObject();
					ob.name = obValue.Value.GameObject_Name;
					ob.SetActive( obValue.Value.GameObject_Active );
					createdObject.Add( ob, obValue.Key );

					// Debug.Log(ob.name + " " + obValue.Value.GameObject_Active + " " +  obValue.Value.GameObject_SiblingPos);

					if ( WRITE_OLD_NEW )
					{
						m_NEW_OLD.Add( ob.GetInstanceID(), (int)obValue.Key );
						m_OLD_NEW.Add( (int)obValue.Key, ob.GetInstanceID() );


						foreach ( var t in source.comp_to_Type )
						{
							if ( t.Value == "UnityEngine.Transform" )
							{
								var trID = ob.GetComponent<Transform>().GetInstanceID();

								m_NEW_OLD.Add( trID, (int)t.Key );
								m_OLD_NEW.Add( (int)t.Key, trID );
								//Debug.Log("ASD");
								break;
							}
						}
					}


					// UnityEngine.Object.DestroyImmediate(ob.GetComponent<Transform>());
					/*
                    source.field_records[]*/
					// current_state.field_records[obValue.Key]. = ob.GetInstanceID();
				}
			}

			if ( wasCreated )
			{
				foreach ( var o in createdObject )
				{
					var parent = GET_ID(source.field_records[o.Value].GameObject_ParentID, m_OLD_NEW);
					Undo.SetTransformParent( o.Key.transform, parent ? ((GameObject)parent).transform : null, UNDO_TEXT );
					Undo.RegisterCreatedObjectUndo( o.Key, UNDO_TEXT );
					hierarchychanges.Add( o.Key, source.field_records[ o.Value ] );
				}

				/*  var sorted = createdObject.Select(o => new { o, source.field_records[o.Value].GameObject_SiblingPos }).OrderBy(s => s.GameObject_SiblingPos).ToArray();
                  foreach (var o in sorted)
                  {
                      Undo.RegisterFullObjectHierarchyUndo(o.o.Key, UNDO_TEXT);
                      o.o.Key.transform.SetAsLastSibling();
                  }
                
                
                  foreach (var o in sorted)
                  {
                      o.o.Key.transform.SetSiblingIndex(o.GameObject_SiblingPos);
                      // Hierarchy.SetDirty(o.o);
                      Undo.RegisterCreatedObjectUndo(o.o.Key, UNDO_TEXT);
                  }*/
			}
		}


		void CHACNGE_HIERARCHY( Dictionary<GameObject, KeeperDataItem> hierarchychanges, string UNDO_TEXT, bool use_new )
		{
			if ( hierarchychanges.Count == 0 ) return;


			foreach ( var o in hierarchychanges )
			{
				var parent = use_new ? GET_ID(o.Value.GameObject_ParentID, m_OLD_NEW) : GET_ID(o.Value.GameObject_ParentID, m_EMPTY);
				Undo.SetTransformParent( o.Key.transform, parent ? ((GameObject)parent).transform : null, UNDO_TEXT );
			}

			var sorted = hierarchychanges.Select(o => new { o.Key, o.Value.GameObject_SiblingPos }).OrderBy(s => s.GameObject_SiblingPos).ToArray();
			foreach ( var o in sorted )
			{
				if ( o.Key.transform.parent ) Undo.RegisterFullObjectHierarchyUndo( o.Key.transform.parent, UNDO_TEXT );
				else Undo.RegisterFullObjectHierarchyUndo( o.Key.transform, UNDO_TEXT );
				o.Key.transform.SetAsLastSibling();
			}


			foreach ( var o in sorted )
			{
				o.Key.transform.SetSiblingIndex( o.GameObject_SiblingPos );
				adapter.SetDirty( o.Key );
				// Undo.RegisterCreatedObjectUndo(o.o.Key, UNDO_TEXT);
			}
		}



		private bool DO_CHANGES(
			GameObject o, int recordId, bool use_add_remove,
			KeeperData source,
			KeeperData last,
			string UNDO_TEXT, ref Dictionary<GameObject, KeeperDataItem> hierarchychanges, bool APPPY_JSON )
		{
			if ( !o || !o.scene.IsValid() || !o.scene.isLoaded )
			{
				return false;
			}

			/* if (o.name == "Directional Light")
                 Debug.Log(o.name);*/
			//OB_ID = GET_ID(o.GetInstanceID(), source_translator).;
			/* if (m_NEW_OLD.ContainsKey(id)) OB_ID = (m_NEW_OLD[id]);
             else OB_ID = (o.GetInstanceID());*/
			OB_ID = (int)recordId;
			if ( last == null ) last = new KeeperData() { comp_to_Type = new Dictionary<int, string>(), field_records = new Dictionary<int, KeeperDataItem>() };

			if ( !source.field_records.ContainsKey( OB_ID ) )
			{
				LogProxy.LogWarning( "[EMX] Error load '" + o.name + "' keeper state" );
				return false;
			}

			// OB_ID = GET_ID(o.GetInstanceID(), source_translator).GetInstanceID();
			var LAST_HAVE_OBJECT = last.field_records.ContainsKey(OB_ID);
			var haveChanges = false;


			/*  if (createdObject.Count != 0)
              {
                  foreach (var o in createdObject)
                  {
                      var parent = EditorUtility.Instance IDToObject(current_state.field_records[m_NEW_OLD[o.GetInstanceID()]].GameObject_ParentID) as Transform;
                      Undo.SetTransformParent(o.transform, parent, UNDO_TEXT);
                  }
            
                  var sorted = createdObject.Select(o => new { o, current_state.field_records[id].GameObject_SiblingPos }).OrderBy(s => s.GameObject_SiblingPos).ToArray();
                  foreach (var o in sorted)
                  {
                      Undo.RegisterFullObjectHierarchyUndo(o.o, UNDO_TEXT);
                      o.o.transform.SetAsLastSibling();
                  }
            
            
                  foreach (var o in sorted)
                  {
                      o.o.transform.SetSiblingIndex(o.GameObject_SiblingPos);
                      Hierarchy.SetDirty(o.o);
                  }
              }*/


			if ( !APPPY_JSON )
			{
				var s = source.field_records[OB_ID];
				var l = LAST_HAVE_OBJECT ? last.field_records[OB_ID] : null;

				//SET ACTIVE
				//if (adapter.par_e.PLAYMODESAVER_SAVE_ENABLINGDISABLING_GAMEOBJEST && s.ALL )
				if ( adapter.par_e.PLAYMODESAVER_SAVE_ENABLINGDISABLING_GAMEOBJEST_MENU && s.SAVE_SETACTIVE )
				{
					if ( !LAST_HAVE_OBJECT || s.GameObject_Active != l.GameObject_Active )
					{
						//Undo.RecordObject(o, UNDO_TEXT);
						Undo.RegisterFullObjectHierarchyUndo( o, UNDO_TEXT );

						o.SetActive( s.GameObject_Active );
						if ( !Application.isPlaying ) EditorUtility.SetDirty( o );
						haveChanges = true;
					}
				}

				//EMX_MBYFIX
				//PARENT
				if ( adapter.par_e.PLAYMODESAVER_SAVE_GAMEOBJET_HIERARCHY_MENU && s.SAVE_SIBLING )
				{
					if ( !LAST_HAVE_OBJECT || s.GameObject_ParentID != l.GameObject_ParentID ||
						s.GameObject_SiblingPos != l.GameObject_SiblingPos )
					{
						hierarchychanges.Add( o, s );
						haveChanges = true;
					}
				}
			}


			if ( source.field_records[ OB_ID ].records.Count > keysArray.Length ) Array.Resize( ref keysArray, source.field_records[ OB_ID ].records.Count );
			inter = 0;
			foreach ( var kp in source.field_records[ OB_ID ].records )
			{
				keysArray[ inter++ ] = (int)kp.Key;
				//Debug.Log(kp.Key);
			}

			//  Dictionary<string, Type> ass = null;
			for ( int i = 0; i < inter; i++ )
			{
				COMPID = keysArray[ i ];
				if ( m_OLD_NEW.ContainsKey( COMPID ) ) COMPID = m_OLD_NEW[ COMPID ];
				/*  }
                
                  foreach (var record in source.field_records[OB_ID].records)
                  {*/
				var comp = EditorUtility.InstanceIDToObject(COMPID) as Component;
				var LAST_HAVE_COMP = LAST_HAVE_OBJECT && last.field_records[OB_ID].records.ContainsKey(COMPID);

				if ( !APPPY_JSON && use_add_remove )
				{
					if ( !comp && !LAST_HAVE_COMP ) // System.AppDomain.CurrentDomain.GetAssemblies(
					{ /* if (ass == null)
                         {
                        
                             ; 
                             /*  foreach (var assembly in )
                               {
                                   //if (assembly.GetTypes().Any(at=>at.FullName == source.comp_to_Type[COMPID]))
                                   Debug.Log(assembly.FullName);
                               }#1#
                             // ass = System.AppDomain.CurrentDomain.GetAssemblies().Where(a=>a.SelectMany(a => a.GetTypes()).ToDictionary(asd => FullName);
                             /*   foreach (var type in ass) {
                                    MonoBehaviour.print(type.FullName);
                                }#1#
                         }*/
						// var t = ass.First(a => a.FullName == source.comp_to_Type[COMPID]);
						//Type getted_t;
						// Dictionary<string,Type> st_to_ty = new Dictionary<string, Type>();
						// if (!st_to_ty.TryGetValue(source.comp_to_Type[COMPID], out getted_t)) st_to_ty.Add(source.comp_to_Type[COMPID], getted_t = )
						var getted_t = TODO_Tools.GET_TYPE_BY_STRING(source.comp_to_Type[COMPID]);

						if ( getted_t != null ) //  MonoBehaviour.print(t);
						{ // bool needRegistrate = false;
							comp = o.AddComponent( getted_t );
							// comp = o.AddComponent(Type.GetType(source.comp_to_Type[COMPID]));
							var kp = source.field_records[OB_ID].records[COMPID];
							source.field_records[ OB_ID ].records.Remove( COMPID );
							if ( last.field_records.ContainsKey( OB_ID ) && last.field_records[ OB_ID ].records.ContainsKey( COMPID ) )
							{
								kpLast = last.field_records[ OB_ID ].records[ COMPID ];
								last.field_records[ OB_ID ].records.Remove( COMPID );
							}
							else kpLast = null;

							m_NEW_OLD.Add( comp.GetInstanceID(), COMPID );
							m_OLD_NEW.Add( COMPID, comp.GetInstanceID() );

							COMPID = comp.GetInstanceID();
							source.field_records[ OB_ID ].records.Add( COMPID, kp );
							source.comp_to_Type.Add( COMPID, comp.GetType().FullName );
							if ( kpLast != null )
							{
								last.field_records[ OB_ID ].records.Add( COMPID, kpLast );
								last.comp_to_Type.Add( COMPID, comp.GetType().FullName );
							}

							Undo.RegisterCreatedObjectUndo( comp, UNDO_TEXT );
						}
						else
						{
							LogProxy.LogWarning( "[EMX] Reference save error '" + source.comp_to_Type[ COMPID ] + "'" );
						}

						/*  EditorUtility.SetDirty(comp);
                          EditorUtility.SetDirty(comp.gameObject);*/

						/*  Undo.RecordObject(comp, UNDO_TEXT);
                          EditorJsonUtility.FromJsonOverwrite(kp, comp);
                          EditorUtility.SetDirty(comp);
                          EditorUtility.SetDirty(comp.gameObject);
                          haveChanges = true;*/
					}
				}

				if ( APPPY_JSON )
				{
					if ( !comp )
					{
						continue;
					}

					if ( source.field_records[ OB_ID ].records.ContainsKey( COMPID ) ) //    var id = comp.GetInstanceID();
					{
						if ( source.field_records[ OB_ID ].records[ COMPID ] != ToJson( comp ) ) // MonoBehaviour.print("ASD");
						{
							Undo.RecordObject( comp, UNDO_TEXT );
							FromJsonOverwrite( source.field_records[ OB_ID ].records[ COMPID ], comp );
							if ( !Application.isPlaying ) EditorUtility.SetDirty( comp );
							if ( !Application.isPlaying ) EditorUtility.SetDirty( comp.gameObject );
							haveChanges = true;
						}
					}
				}
			}

			/*if (removearray.Count != 0) {
                for (int i = 0; i < removearray.Count; i++) {
                    source.field_records[OB_ID].records.Remove(i);
            
                }
            }*/


			//EMX_MBYFIX
			if ( !APPPY_JSON && use_add_remove )
			{
				if ( LAST_HAVE_OBJECT && last.field_records[ OB_ID ].SAVE_COMPONENTS ) //  MonoBehaviour.print("ASD");
				{
					foreach ( var lastcompID in last.field_records[ OB_ID ].records )
					{
						if ( !source.field_records[ OB_ID ].records.ContainsKey( lastcompID.Key ) )
						{
							var forDestroy = EditorUtility.InstanceIDToObject(lastcompID.Key) as Component;
							// MonoBehaviour.print("1 " + forDestroy);
							if ( forDestroy ) Undo.DestroyObjectImmediate( forDestroy );
						}
					}

					/*   foreach (var lastcompID in source.field_records[OB_ID].records)
                       {
                           if (!last.field_records[OB_ID].records.ContainsKey(lastcompID.Key))
                           {
                               var forDestroy = EditorUtility.InstanceIDToObject(lastcompID.Key) as Component;
                               MonoBehaviour.print("2 " + forDestroy);
                               if (forDestroy) Undo.DestroyObjectImmediate(forDestroy);
                           }
                       }*/
				}
				else if ( source.field_records[ OB_ID ].SAVE_COMPONENTS ) //  MonoBehaviour.print("2");
				{
					var comps = o.GetComponents<Component>();
					for ( int i = comps.Length - 1; i >= 0; i-- )
					{
						if ( !comps[ i ] ) continue;
						if ( !source.field_records[ OB_ID ].records.ContainsKey( comps[ i ].GetInstanceID() ) )
						{
							Undo.DestroyObjectImmediate( comps[ i ] );
						}
					}
				}
			}

			return haveChanges;
		}




		bool LAST_VALIDATE_UNDO( GameObject o, Component[] comps )
		{
			if ( last_state == null || current_state_back == null || Application.isPlaying ) return false;

			if ( !o || !o.scene.IsValid() || m_NEW_OLD.ContainsKey( o.GetInstanceID() ) ) return false;

			var ob_id = o.GetInstanceID();

			var LAST_HAVE_OBJECT = last_state.field_records.ContainsKey(ob_id);


			////EMX_TODO while cancellation for new created objects during playmode not allow
			if ( !LAST_HAVE_OBJECT ) return false;
			// if ( !last_state.field_records.ContainsKey( o.GetInstanceID() ) ) return false;


			var currentids = comps.Select(c => c.GetInstanceID()).ToArray();
			//EMX_MBYFIX
			if ( adapter.par_e.PLAYMODESAVER_USE_ADD_REMOVE_COMPONENTS && last_state.field_records[ ob_id ].SAVE_COMPONENTS )
			{
				if ( LAST_HAVE_OBJECT )
				{
					foreach ( var lastcompID in last_state.field_records[ ob_id ].records )
					{
						if ( !currentids.Contains( lastcompID.Key ) )
						{
							return true;
						}
					}

					if ( last_state.field_records[ ob_id ].SAVE_ALL )
						foreach ( var lastcompID in currentids )
						{
							if ( !last_state.field_records[ ob_id ].records.ContainsKey( lastcompID ) )
							{
								return true;
							}
						}
				}

				if ( current_state_back.field_records.ContainsKey( ob_id ) && current_state_back.field_records[ ob_id ].SAVE_ALL )
				{
					for ( int i = comps.Length - 1; i >= 0; i-- )
					{
						if ( !comps[ i ] ) continue;
						if ( !current_state_back.field_records[ ob_id ].records.ContainsKey( comps[ i ].GetInstanceID() ) )
						{
							return true;
						}
					}
				}
			}


			if ( LAST_HAVE_OBJECT )
			{
				if ( current_state_back.field_records.ContainsKey( ob_id ) )
				{
					var l = last_state.field_records[ob_id];
					// var c = current_state_back.field_records[ob_id];
					/*                        var pp =   l.GameObject_ParentID != c.GameObject_ParentID || l.GameObject_SiblingPos != c.GameObject_SiblingPos;
                                            var aa = l.GameObject_Active != c.GameObject_Active;
                                            if (l.GameObject_Name != c.GameObject_Name ||*/
					var pp = l.SAVE_SIBLING && adapter.par_e.PLAYMODESAVER_SAVE_GAMEOBJET_HIERARCHY_MENU && l.GameObject_ParentID != (o.transform.parent ? o.transform.parent.gameObject.GetInstanceID() : -1) || l.GameObject_SiblingPos != o.transform.GetSiblingIndex();
					var aa = l.SAVE_SETACTIVE && adapter.par_e.PLAYMODESAVER_SAVE_ENABLINGDISABLING_GAMEOBJEST_MENU && l.GameObject_Active != o.activeSelf;
					if (
						//EMX_TODO keeper name
						//l.GameObject_Name != o.name ||
						pp ||
						aa
					) return true;
				}
			}

			/*  if (par.DataKeeperParams.USE_ADD_REMOVE)
              {
                  if (LAST && !comp || comp && !LAST) return true;
              }
            */

			foreach ( var comp in comps )
			{
				var LAST = last_state.field_records[ob_id].records.ContainsKey(comp.GetInstanceID());
				// var CURRENT = current_state.field_records[ob_id].ContainsKey(comp.GetInstanceID());

				if ( adapter.par_e.PLAYMODESAVER_USE_ADD_REMOVE_COMPONENTS && last_state.field_records[ ob_id ].SAVE_COMPONENTS )
				{
					if ( LAST && !comp ) return true;
				}

				if ( !comp ) continue;
				if ( last_state.field_records[ o.GetInstanceID() ].records.ContainsKey( comp.GetInstanceID() ) )
				{
					if ( last_state.field_records[ o.GetInstanceID() ].records[ comp.GetInstanceID() ] != ToJson( comp ) )
					{
						return true;
					}
				}
			}

			return false;
		}



		void LAST_DO_UNDO( GameObject o, Component[] comps )
		{
			if ( last_state == null ) return;

			Dictionary<GameObject, KeeperDataItem> hierarchychanges = new Dictionary<GameObject, KeeperDataItem>();

			////EMX_TODO  Now I added disable cancellation for the created objects
			// Now I added disable undo for the created objects so just o.GetInstanceID ()
			var changes = false;
			changes = DO_CHANGES( o, o.GetInstanceID(), true, last_state, current_state_back, "Revert Last PlayMode Changes", ref hierarchychanges, false );
			changes |= DO_CHANGES( o, o.GetInstanceID(), true, last_state, current_state_back, "Revert Last PlayMode Changes", ref hierarchychanges, true );
			if ( changes ) adapter.MarkSceneDirty( o.scene );
			/*  if (DO_CHANGES(o, o.GetInstanceID(), true, last_state, current_state_back, "Revert Last PlayMode Changes", ref hierarchychanges))
              {
            
            
            
                  EditorSceneManager.MarkSceneDirty(o.scene);
              }*/

			CHACNGE_HIERARCHY( hierarchychanges, "Revert Last PlayMode Changes", true );

			/* OB_ID = o.GetInstanceID();
             foreach (var comp in comps)
             {
                 if (!comp || !o || !o.scene.IsValid() || !last_state.field_records.ContainsKey(OB_ID)) continue;
                 COMPID = comp.GetInstanceID();
                 //  if (!comp || !comp.gameObject || !comp.gameObject.scene.IsValid()) continue;
                 if (last_state.field_records[OB_ID].ContainsKey(COMPID)))
                 {
            
                     if (par.DataKeeperParams.USE_ADD_REMOVE)
                     {
            
                     } else
                     {
            
                     }
            
                     var id = comp.GetInstanceID();
                     if (last_state.field_records[id] != EditorJsonUtility.ToJson(comp))
                     {
                         Undo.RecordObject(comp, "Revert Last PlayMode Changes");
                         EditorJsonUtility.FromJsonOverwrite(last_state.field_records[id], comp);
                         EditorUtility.SetDirty(comp);
                         EditorUtility.SetDirty(comp.gameObject);
                         if (!scenes.Contains(comp.gameObject.scene)) scenes.Add(comp.gameObject.scene);
                     }
            
                 }
             }*/

			writeChanges.Clear();
		}



		//  static bool editMode = true;
		private void Update()
		{
			/* if (!Application.isPlaying)
             {
                 if (applyOnUpdate != null)
                 {
                     applyOnUpdate();
                     applyOnUpdate = null;
                 }
            
                 editMode = true;
            
            
             }*/
		}




		KeeperData last_state, current_state;





		GUIContent PLAY_CONT_LINES = new GUIContent() { tooltip = "Select All Components\n( Ctrl+Click - Include children )\nRight-Click - Search" };

		// GUIContent EDIT_CONT_STORE = new GUIContent() { text = "", tooltip = "" };
		GUIContent PLAY_CONT_STORE = new GUIContent() { tooltip = "Choose Component\n( Ctrl+Click - Include children )\nRight-Click - Search" };

		// GUIContent EDIT_CONT_LINES = new GUIContent() { text = "", tooltip = "" };
		IconData compstexture, storagetexture;
		//SingleList currentList = null;

		Rect borderR = new Rect(), labelrect = new Rect(), leftR = new Rect(), rightR = new Rect();
		Color alpha = new Color(1, 1, 1, 0.4f);




		List<int> GET_CURRENT_LIST( HierarchyObject _o, ref bool contains )
		{
			var currentList = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModPlayKeeper, _o);
			if ( currentList == null )
			{
				contains = false;
				return null;
			}

			var list = currentList.GetIntList();

			if ( list.Count == 0 ) list.Add( 0 );

			if ( (list[ 0 ] & _K_ALL) != 0 )
			{
				compstexture = adapter.GetNewIcon( NewIconTexture.RightMods, "STORAGE_ALLCOMPS" );
				storagetexture = adapter.GetNewIcon( NewIconTexture.RightMods, "STORAGE_ACTIVE" );
			}
			else if ( (list[ 0 ] & ~_K_ALL) != 0 )
			{
				compstexture = adapter.GetNewIcon( NewIconTexture.RightMods, "STORAGE_ONECOMP" );
				storagetexture = adapter.GetNewIcon( NewIconTexture.RightMods, "STORAGE_ACTIVE" );
			}
			else
			{
				for ( int i = list.Count - 1; i > 0; i-- )
				{
					if ( !EditorUtility.InstanceIDToObject( list[ i ] ) ) // MonoBehaviour.print("ASD");
					{
						list.RemoveAt( i );
					}
				}

				if ( list.Count > 1 )
				{
					compstexture = adapter.GetNewIcon( NewIconTexture.RightMods, "STORAGE_ONECOMP" );
					storagetexture = adapter.GetNewIcon( NewIconTexture.RightMods, "STORAGE_ACTIVE" );
				}
				else
				{
					contains = false;
					return null;
				}
			}

			return list;
		}

		static List<int> oneList = new List<int>(1) { 0 };
		//static List<int >emptyList = new List<int>();
		internal void INTERNAL__SetValue( Scene _s, HierarchyObject o, SetSpecialOptionsHelper _OPTIONS, int[] comps )
		{
			// if ( Hierarchy.HierarchyAdapterInstance.DISABLE_DESCRIPTION( _s ) ) return;
			if ( !o.Validate() ) return;


			//   var d = M_Descript.des(_s.GetHashCode());
			// if ( d == null ) return;

			var s = _s.GetHashCode();
			var converted = _OPTIONS.CONVERT(0);

			if ( comps == null )
			{
				comps = new int[ 0 ];
				var has = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModPlayKeeper, o);
				if ( has != null )
				{
					var list = has.GetIntList();
					for ( int i = 1; i < list.Count; i++ )
					{
						ArrayUtility.Add( ref comps, list[ i ] );
					}
					if ( list.Count > 0 ) converted = _OPTIONS.CONVERT( list[ 0 ] );
				}
				/*if ( DataKeeperCache.HasKey( s, o ) )
                {
                    var v = DataKeeperCache.GetValue(s, o);
                    for ( int i = 1 ; i < v.list.Count ; i++ )
                    {
                        ArrayUtility.Add( ref comps, v.list[ i ] );
                    }
                }*/
			}
			var isEmpty = converted == 0;

			if ( isEmpty && comps.Length == 0 )
			{
				// adapter.SET_UNDO( d, "Change PlayMode Data Keeper" );
				// DataKeeperCache.SetValue( null, s, o.go, true );
				//	HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModPlayKeeper, o, emptyList );
				HierarchyTempSceneDataGetter.RemoveObjectData( SaverType.ModPlayKeeper, o );
			}
			else
			{
				// adapter.SET_UNDO( d, "Change PlayMode Data Keeper" );
				// if ( !DataKeeperCache.HasKey( s, o ) ) DataKeeperCache.SetValue( new SingleList() { list = new List<int>() { 0 } }, s, o.go, true );
				// var v = DataKeeperCache.GetValue(s, o);

				if ( !isEmpty )
				{
					var has = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModPlayKeeper, o);
					if ( has != null )
					{
						var list = has.GetIntList();
						if ( list.Count > 0 )
						{
							list[ 0 ] = converted;
							HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModPlayKeeper, o, list );
						}
						else
						{
							oneList[ 0 ] = converted;
							HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModPlayKeeper, o, oneList );
						}
					}
					else
					{
						oneList[ 0 ] = converted;
						HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModPlayKeeper, o, oneList );
					}
					// v.list[ 0 ] = 1;
					// DataKeeperCache.SetValue( v, s, o.go, true );
				}
				else
				{
					var list = new List<int>(1) { 0 };
					for ( int i = 0; i < comps.Length; i++ )
					{
						list.Add( comps[ i ] );
					}

					var r = o.go.GetComponents<Component>();
					if ( r.Select( c => c.GetInstanceID() ).Count( i => list.Contains( i ) ) == r.Length ) list[ 0 ] |= _K_ALL;
					HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModPlayKeeper, o, list );
					//DataKeeperCache.SetValue( v, s, o.go, true );
				}
				if ( Application.isPlaying ) HierarchyTempSceneDataGetter.SetObjectData( SaverType.ModPlayKeeper, o, true );
			}

			// if (Application.isPlaying && last_state)
			/* Hierarchy.SetDirty(d.component);
             Hierarchy.SetDirty(d.gameObject);
             Hierarchy.MarkSceneDirty(d.gameObject.scene);*/
		}

		// static Dictionary<int, KeeperDataItem> baked_new_objects = new Dictionary<int, KeeperDataItem>();

		internal struct GID : IEquatable<GID>, IEqualityComparer<GID>
		{
			public static bool operator ==( GID x, GID y )
			{
				return x.Equals( y );
			}

			public static bool operator !=( GID x, GID y )
			{
				return !x.Equals( y );
			}

			internal Component component;
			internal int index;

			public bool Equals( GID other )
			{
				return ((GID)other).index == index;
			}

			public override bool Equals( object obj )
			{
				return ((GID)obj).index == index;
			}

			public bool Equals( GID x, GID y )
			{
				return x.Equals( y );
			}

			public int GetHashCode( GID obj )
			{
				return index;
			}

			public override int GetHashCode()
			{
				return index;
			}
		}

		Dictionary<Type, List<GID>> GetIndexesDic( HierarchyObject o, Component[] comps )
		{
			if ( comps.Length == 0 ) return new Dictionary<Type, List<GID>>();
			var ttt = o.GetComponents().GroupBy(c => c.GetType()).ToDictionary(g => g.Key, g => g.ToList());
			return
				comps.Where( c => c && ttt.ContainsKey( c.GetType() ) )
					.Select( c => new { c, index = ttt[ c.GetType() ].IndexOf( c ) } )
					.Where( i => i.index != -1 ).GroupBy( c => c.c.GetType() ).ToDictionary( c => c.Key, c => c.Select( ac => new GID() { component = ac.c, index = ac.index } ).ToList() );
		}

		void SetValueFlags( GameObject o, SetSpecialOptionsHelper All )
		{
			var selO = Cache.GetHierarchyObjectByInstanceID(o);
			INTERNAL__SetValue( o.scene, selO, All, null );
		}


		static HierarchyObject[] GET_HOBEJCTS( GameObject o )
		{
			return Root.p[ 0 ].ha.SELECTED_GAMEOBJECTS().All( selO => selO.go != o ) ? new[] { Cache.GetHierarchyObjectByInstanceID( o ) } : Root.p[ 0 ].ha.SELECTED_GAMEOBJECTS();
		}
		static List<GameObject> GET_OBEJCTS( GameObject o )
		{
			return Root.p[ 0 ].ha.SELECTED_GAMEOBJECTS().All( selO => selO.go != o ) ? new List<GameObject>( 1 ) { o } : Root.p[ 0 ].ha.SELECTED_GAMEOBJECTS().Select( g => g.go ).ToList();
		}

		void SetValue_( GameObject o, SetSpecialOptionsHelper All, int[] comps, bool single, bool usePing )
		{ /*                List<GID> g1 = new List<GID>(1) { new GID() { component = o.transform, index = 3 } };
                            List<GID> g2 = new List<GID>(1) { new GID() { component = o.GetComponents<Component>()[1], index = 3 } };
                            Debug.Log(g1.Intersect(g2).Count());*/

			//if ( single ) // || adapter.ha.SELECTED_GAMEOBJECTS().All( selO => selO.go != o )
			{
				var selO = Cache.GetHierarchyObjectByInstanceID(o);
				INTERNAL__SetValue( o.scene, selO, All, comps );
				/* Undo.RecordObject(r, "Change sortingLayerName");
                 r.sortingLayerName = sortingLayer;
                 Hierarchy.SetDirty(r);
                 Hierarchy.MarkSceneDirty(o.scene);*/

				if ( usePing ) Tools.TRY_PING_OBJECT( selO.go );
			}

		}

		void SetValueMulty( GameObject activeO, HierarchyObject[] __o, SetSpecialOptionsHelper All, int[] comps )
		{
			var casd = (comps ?? new int[0]).Select(c => EditorUtility.InstanceIDToObject(c) as Component).Where(c => c).ToArray();
			Dictionary<Type, List<GID>> refComps = GetIndexesDic(Cache.GetHierarchyObjectByInstanceID(activeO), casd);
			//  Debug.Log(refComps.First().Key);

			HierarchyTempSceneDataGetter.SetUndoListStart( "Apply playmodeKeeper" );

			foreach ( var objectToUndo in __o ) //adapter.ha.SELECTED_GAMEOBJECTS()
			{ /* var targetComps = objectToUndo.GetComponents<Component>().Where(c => refComps.ContainsKey(c.GetType())).GroupBy(c => c.GetType()).ToDictionary(g => g.Key, g => g.ToList());
                         .GroupBy(c => c.GetType()).ToDictionary(g => g.Key.GetType(), g => g.ToList());
                    */
				HierarchyTempSceneDataGetter.SetUndoList( objectToUndo.scene );


				var targetComps = GetIndexesDic(objectToUndo, objectToUndo.GetComponents().Where(c => refComps.ContainsKey(c.GetType())).ToArray());
				//  Debug.Log(targetComps.First().Value[0].index + " " + targetComps.Count + " " + targetComps.First().Value.Count);
				//   Debug.Log(refComps.First().Value[0].index + " " + refComps.Count + " " + refComps.First().Value.Count);
				var result = targetComps.Select(t => t.Value.Intersect(refComps[t.Key])).SelectMany(s => s.ToArray()).Select(c => c.component.GetInstanceID()).ToArray();

				//Debug.Log(result[0]);

				INTERNAL__SetValue( objectToUndo.go.scene, objectToUndo, All, result );

				/*   var c = cache.GetValue(objectToUndo.GetInstanceID());
                   if (!c) continue;
                   Undo.RecordObject(c, "Change sortingLayerName");
                   c.sortingLayerName = sortingLayer;
                   Hierarchy.SetDirty(c);
                   Hierarchy.MarkSceneDirty(c.gameObject.scene);*/
				//  if (Hierarchy.par.ENABLE_PING_Fix) adapter.TRY_PING_OBJECT(objectToUndo);
			}

			HierarchyTempSceneDataGetter.SetDirtyList();
		}

		List<int> _GetValueEmptyList = new List<int>(1){0 };
		List<int> GetValue( GameObject o )
		{
			var selO = Cache.GetHierarchyObjectByInstanceID(o);
			var getted = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModPlayKeeper, selO);
			if ( getted != null && getted.GetIntList().Count > 0 ) return getted.GetIntList();
			return _GetValueEmptyList;
		}

		List<int> GetAddedIDs( GameObject o, Component[] comps )
		{
			var selO = Cache.GetHierarchyObjectByInstanceID(o);

			var getted = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModPlayKeeper, selO);
			//  var getted = DataKeeperCache.GetValue(o.scene, selO);
			List<int> added = null;
			if ( getted != null && getted.GetIntList().Count > 0 )
			{
				var list = getted.GetIntList();
				if ( (list[ 0 ] & _K_ALL) == _K_ALL ) added = comps.Select( c => c.GetInstanceID() ).ToList();
				else
				{
					added = list.ToList();
					if ( added.Count > 0 ) added.RemoveAt( 0 );
				}
			}
			else
			{
				added = new List<int>();
			}

			return added;
		}
		struct addedComp
		{
			internal addedComp( Component c )
			{
				id = c.GetInstanceID();
				comp = c;
			}
			internal addedComp( int id )
			{
				this.id = id;
				comp = EditorUtility.InstanceIDToObject( id ) as Component;
			}
			internal int id;
			internal Component comp;
		}
		List<addedComp> GetAddedComps( GameObject o, Component[] comps )
		{
			var selO = Cache.GetHierarchyObjectByInstanceID(o);

			var getted = HierarchyTempSceneDataGetter.GetObjectData(SaverType.ModPlayKeeper, selO);
			//  var getted = DataKeeperCache.GetValue(o.scene, selO);
			List<addedComp> added = null;
			if ( getted != null && getted.GetIntList().Count > 0 )
			{
				var list = getted.GetIntList();
				if ( (list[ 0 ] & _K_ALL) == _K_ALL ) added = comps.Select( c => new addedComp( c ) ).ToList();
				else
				{
					added = list.Select( id => new addedComp( id ) ).ToList();
					if ( added.Count > 0 ) added.RemoveAt( 0 );
				}
			}
			else
			{
				added = new List<addedComp>();
			}

			return added;
		}


		void ApplyToSelected( GameObject clickedOb, List<GameObject> __o, Dictionary<Component, bool> enablelist, SetSpecialOptionsHelper data )
		{

			//var top = EMX.Utility.GetOnlyTopObjects( __o );
			HierarchyTempSceneDataGetter.SetUndoListStart( "Apply playmodekeeper" );

			foreach ( var selectedOb in __o )
			//    foreach ( var componentsInChild in o.GetComponentsInChildren<Transform>( true ) )
			{

				if ( clickedOb == selectedOb ) continue;

				var childComps = selectedOb.GetComponents<Component>();
				var childadded = GetAddedIDs(selectedOb.gameObject, childComps);
				foreach ( var component in enablelist ) //if (!component.Value) continue;
				{
					var type = (component.Key.GetType().Name);
					var comp = childComps.FirstOrDefault(c => (c.GetType().Name) == type);
					if ( !comp ) continue;
					var iidd = comp.GetInstanceID();
					if ( !component.Value ) childadded.RemoveAll( a => a == iidd );
					else if ( !childadded.Contains( iidd ) ) childadded.Add( iidd );
				}

				//var all = childadded.Count == childComps.Length;.
				//var data = SetSpecialOptionsHelper.GET;
				data.ALL = childadded.Count == childComps.Length;
				if ( data.ALL == true ) for ( int i = 0; i < childadded.Count; i++ ) if ( !childadded.Contains( childComps[ i ].GetInstanceID() ) )
						{
							data.ALL = false;
							break;
						}

				HierarchyTempSceneDataGetter.SetUndoList( selectedOb.scene );

				SetValue_( selectedOb.gameObject, data, childadded.ToArray(), true, false );
			}
			HierarchyTempSceneDataGetter.SetDirtyList();
		}

		void ApplyToChild_( List<GameObject> __o, Dictionary<Component, bool> enablelist )
		{

			var top = EMX.Utility.GetOnlyTopObjects( __o );

			HierarchyTempSceneDataGetter.SetUndoListStart( "Apply playmodekeeper" );

			foreach ( var o in top )
				foreach ( var componentsInChild in o.GetComponentsInChildren<Transform>( true ) )
				{
					if ( componentsInChild.gameObject == o ) continue;
					var childComps = componentsInChild.GetComponents<Component>();
					var childadded = GetAddedIDs(componentsInChild.gameObject, childComps);
					foreach ( var component in enablelist ) //if (!component.Value) continue;
					{
						var type = (component.Key.GetType().Name);
						var comp = childComps.FirstOrDefault(c => (c.GetType().Name) == type);
						if ( !comp ) continue;
						var iidd = comp.GetInstanceID();
						if ( !component.Value ) childadded.RemoveAll( a => a == iidd );
						else if ( !childadded.Contains( iidd ) ) childadded.Add( iidd );
					}

					//var all = childadded.Count == childComps.Length;.
					var data = SetSpecialOptionsHelper.GET;
					data.ALL = childadded.Count == childComps.Length;
					if ( data.ALL == true ) for ( int i = 0; i < childadded.Count; i++ ) if ( !childadded.Contains( childComps[ i ].GetInstanceID() ) )
							{
								data.ALL = false;
								break;
							}

					HierarchyTempSceneDataGetter.SetUndoList( componentsInChild.gameObject.scene );

					SetValue_( componentsInChild.gameObject, data, childadded.ToArray(), true, false );
				}

			HierarchyTempSceneDataGetter.SetDirtyList();

		}



	}
}
