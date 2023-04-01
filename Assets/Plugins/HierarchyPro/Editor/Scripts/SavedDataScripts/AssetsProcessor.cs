//#define EMX_LOG_A
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor
{
	class HierarchyTempSceneDataPostprocessor : AssetPostprocessor
    {

        static Dictionary<char, char> numbs = Enumerable.Repeat(0, 10).Select((a, i) => i.ToString()[0]).ToDictionary(k => k, v => v);
        static void OnPostprocessAllAssets( string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths ) // Debug.Log( "OnPostprocessAllAssets "  );
        {
            if ( Root.p == null || Root.p.Length == 0 || Root.p[ 0 ] == null ) return;
            if ( !Root.p[ 0 ].par_e.ENABLE_ALL ) return;
            if ( importedAssets.Length != 0 || deletedAssets.Length != 0 || movedAssets.Length != 0 || movedFromAssetPaths.Length != 0 )
                foreach ( var item in Root.p )
                {
                    if ( item == null ) continue;
                    item.invoke_ON_ASSET_IMPORT();
                }

            /*  string debug = "";
			  if (importedAssets.Length != 0) debug += ("--- importedAssets ---\n");
			  foreach (var item in importedAssets) debug += (item + "\n");
			  if (deletedAssets.Length != 0) debug += ("--- deletedAssets ---\n");
			  foreach (var item in deletedAssets) debug += (item + "\n");
			  if (movedAssets.Length != 0) debug += ("--- movedAssets ---\n");
			  foreach (var item in movedAssets) debug += (item + "\n");
			  if (movedFromAssetPaths.Length != 0) debug += ("--- movedFromAssetPaths ---\n");
			  foreach (var item in movedFromAssetPaths) debug += (item + "\n");
			  if (!string.IsNullOrEmpty(debug)) Debug.Log(debug);*/

            {
                bool hasScene = false;
                for ( int i = 0; i < importedAssets.Length; i++ ) hasScene |= importedAssets[ i ].EndsWith( ".unity", StringComparison.OrdinalIgnoreCase );
                if ( !hasScene ) for ( int i = 0; i < movedAssets.Length; i++ ) hasScene |= movedAssets[ i ].EndsWith( ".unity", StringComparison.OrdinalIgnoreCase );
                if ( !hasScene ) for ( int i = 0; i < movedFromAssetPaths.Length; i++ ) hasScene |= movedFromAssetPaths[ i ].EndsWith( ".unity", StringComparison.OrdinalIgnoreCase );
                if ( hasScene )
                {
                    Root.p[ 0 ].wasSceneMoved = true;
                    /*	for (int i = 0; i < EditorSceneManager.sceneCount; i++)
					{
						var s = EditorSceneManager.GetSceneAt(i);
						if (!s.IsValid() || !s.isLoaded) continue;
						HierarchyTempSceneData.SaveOnScenePathChanged(s);
					}
					*/
                }
            }






            TryFoundCopyedAssets( ref importedAssets, ref deletedAssets, ref movedAssets, ref movedFromAssetPaths );
            TryFoundMovedAssets( ref importedAssets, ref deletedAssets, ref movedAssets, ref movedFromAssetPaths );
            TryFoundDeletedAssets( ref importedAssets, ref deletedAssets, ref movedAssets, ref movedFromAssetPaths );




            HierarchyObject._child_count.Clear();
            HierarchyObject._sibling_count.Clear();
            HierarchyObject._sibling_memory.Clear();
            Cache.ClearProejctObjects( ref importedAssets, ref deletedAssets, ref movedAssets, ref movedFromAssetPaths );
            Root.p[ 0 ].RESET_DRAWSTACK( 0 );
            Root.p[ 0 ].RESET_DRAWSTACK( 1 );

            //Initializator.Adapters[Initializator.PROJECT_NAME].ClearHierarchyObjects();
        }


        public static void OnAssetImoortFake()
        {
            Root.p[0].invoke_ON_ASSET_IMPORT();
            HierarchyObject._child_count.Clear();
            HierarchyObject._sibling_count.Clear();
            HierarchyObject._sibling_memory.Clear();
            Cache.ClearProejctObjects();
            Root.p[0].RESET_DRAWSTACK(0);
            Root.p[0].RESET_DRAWSTACK(1);
        }


        static void TryFoundCopyedAssets( ref string[] importedAssets, ref string[] deletedAssets, ref string[] movedAssets, ref string[] movedFromAssetPaths )
        {

            bool hasToCheckFolders = false;

            if ( importedAssets.Length != 0 )
            {
                for ( int i = 0; i < importedAssets.Length; i++ )
                {
                    if ( importedAssets[ i ].EndsWith( ".unity", StringComparison.OrdinalIgnoreCase ) )
                    {

                        //  if ( importedAssets[ i ] == "Scene - DrawInHier" )
                        //      Debug.Log( "ASD" );
                        if ( importedAssets[ i ].StartsWith( Folders.PluginInternalFolder, StringComparison.OrdinalIgnoreCase ) )
                        {
                            //Debug.Log(importedAssets[ i ]);
                            hasToCheckFolders = true;
                            continue;
                        }

                        var estim = importedAssets[i].Remove(importedAssets[i].Length - ".unity".Length);
                        //DETECT COPY SCENE
                        if ( numbs.ContainsKey( estim[ estim.Length - 1 ] ) )
                        {
                            var lastind = estim.LastIndexOf(" ");
                            int ind ;
                            if ( /*estim.Length - lastind <= 3 &&*/ estim.Length - 1 != lastind && int.TryParse( estim.Substring( lastind + 1 ), out ind ) )
                            {
                                /*var numb = estim.Substring( estim.LastIndexOf( " " ) + 1);
                                var numbInt  = -1;
                                var tryCompleted = false;
                                if ( int.TryParse( numb , out numbInt ) ) {

                                    numbInt--;
                                    if ( numbInt >= 0 ) {
                                        var oldPath = estim.Remove( estim.LastIndexOf( " " ) )( numbInt == 0 ? "" : (" "+ numbInt)) + ".unity";
                                        var sa = AssetDatabase.LoadAssetAtPath<SceneAsset>( oldPath );
                                        if ( sa ) {
                                            var newPathExternal = Hierarchy.HierarchyAdapterInstance.GetStoredDataPathExternal(importedAssets[i]);
                                            if ( File.Exists( newPathExternal ) ) {
                                                if ( Hierarchy.M_Descript.TryCreateBackupForCache( Hierarchy.HierarchyAdapterInstance.GetStoredDataPathInternal( importedAssets[i] ) ) ) {
                                                    AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
                                                    Adapter.RequestScriptReload();
                                                }
                                            }
                                            AssetDatabase.CopyAsset( Hierarchy.HierarchyAdapterInstance.GetStoredDataPathInternal( oldPath ) ,
                                   Hierarchy.HierarchyAdapterInstance.GetStoredDataPathInternal( importedAssets[i] ) );
                                            tryCompleted = true;
                                        }
                                    }


                                }

                                if ( !tryCompleted )*/
                                {

                                    var p = estim.Remove(estim.LastIndexOf(" "));
                                    SceneAsset sa = null;
                                    for ( int ff = ind - 1; ff >= 0; ff-- )
                                    {
                                        var str = ind==0?"": (" " +ff.ToString());
                                        if ( File.Exists( p + str + ".unity" ) )
                                        {
                                            sa = AssetDatabase.LoadAssetAtPath<SceneAsset>( p + str + ".unity" ); //unity
                                            break;
                                        }
                                    }
                                    //var p = estim.Remove(estim.LastIndexOf(" "));
                                    //var oldPath = p + ".unity";
                                    //SceneAsset sa = null;
                                    //
                                    //if ( File.Exists( oldPath ) ) sa = AssetDatabase.LoadAssetAtPath<SceneAsset>( oldPath ); //p
                                    //else
                                    //{
                                    //    for ( int i = length - 1; i >= 0; i-- )
                                    //    {
                                    //
                                    //    }
                                    //    for ( int z = 0; z < ind; z++ )
                                    //    {
                                    //        if ( File.Exists( p + " " + ind.ToString() + ".unity" ) )
                                    //        {
                                    //            sa = AssetDatabase.LoadAssetAtPath<SceneAsset>( p + " " + ind.ToString() + ".unity" ); //unity
                                    //            break;
                                    //        }
                                    //
                                    //    }
                                    //}
                                    // FOUND COPYED SCENE
                                    if ( sa )
                                    {

                                        var oldPath = GetStoredDataPathInternal(AssetDatabase.GetAssetPath( sa));

                                        var newPathExternal = GetStoredDataPathExternal(importedAssets[i]);
                                        var newPathInternal = GetStoredDataPathInternal(importedAssets[i]);
										if ( newPathExternal == null )
										{
											Debug.Log( "Cannot move " + movedAssets[ i ] );
                                            continue;
										}

										if ( File.Exists( newPathExternal ) )
                                        {
                                            if ( HierarchyExternalSceneData.TryCreateBackupForCache_ExternalCachePathInput( 
                                                GetStoredDataPathExternal(newPathInternal)  ) )
                                            {
                                                //   AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
                                                //  Adapter.RequestScriptReload();
                                            }
                                            //File.Delete( newPathExternal );
                                            //if ( File.Exists( newPathExternal + ".meta" ) ) File.Delete( newPathExternal + ".meta" );
                                        }
                                        else
                                        {
                                            if ( !Directory.Exists( newPathExternal.Remove( newPathExternal.LastIndexOf( '/' ) ) ) )
                                                Directory.CreateDirectory( newPathExternal.Remove( newPathExternal.LastIndexOf( '/' ) ) );
                                        }
                                        if (!AssetDatabase.CopyAsset( ( oldPath ), newPathInternal ) ){


                                            var __old = AssetDatabase.LoadAssetAtPath<HierarchyExternalSceneData>(oldPath);
                                            var __new = AssetDatabase.LoadAssetAtPath<HierarchyExternalSceneData>(newPathInternal);
                                            if (__old && __new )
                                            {
                                                __new.CopyFrom( __old );
                                            }
                                            else
                                            {
                                                AssetDatabase.DeleteAsset( newPathInternal );
                                                if (!AssetDatabase.CopyAsset( ( oldPath ), newPathInternal ) )
                                                {
                                                    Debug.Log( "Cannot copy scenes data from: '"+oldPath + "' to: '" + newPathInternal + "' you can copy it yourself, using the inspector");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if ( hasToCheckFolders ) CheckFolders();

        }

		public static string GetStoredDataPathExternal( string s )
		{
#if EMX_LOG_A
			Debug.Log( "GetStoredDataPathInternal " + s );
#endif

			var path = Folders.UNITY_SYSTEM_PATH;
			if ( !path.EndsWith( "/" ) ) path += '/';
            var intt = GetStoredDataPathInternal( s );
            if ( intt == null ) return null;
            return path + intt;
		}
		public static string GetStoredDataPathInternal( string p )
		{

#if EMX_LOG_A
			Debug.Log( "GetStoredDataPathInternal " + p );
#endif
			if ( !p.ToLower().EndsWith( ".unity", StringComparison.OrdinalIgnoreCase ) ) return null;

			p = p.Remove( p.Length - ".unity".Length );
			//  return Folders.PluginInternalFolder + SCENED_DATA_FOLDER + p + ".asset";
			return HierarchyExternalSceneData.d + p + ".asset";
		}


		static void CheckFolders()
        {
            Folders.init = false;
            Folders.CheckFolders( true );
        }

        static void TryFoundMovedAssets( ref string[] importedAssets, ref string[] deletedAssets, ref string[] movedAssets, ref string[] movedFromAssetPaths )
        {
            bool hasToCheckFolders = false;
            if ( movedAssets.Length != 0 )
            {
                bool wasBackUp = false;

                for ( int i = 0; i < movedAssets.Length; i++ )
                {
                    //	if (movedAssets[i] == "Scene - DrawInHier")
                    //		Debug.Log( "ASD" );
                    if ( movedAssets[ i ].ToLower().EndsWith( ".unity" , StringComparison.OrdinalIgnoreCase) )
                    {
                        if ( movedFromAssetPaths[ i ].StartsWith( Folders.PluginInternalFolder, StringComparison.OrdinalIgnoreCase ) )
                        {
                            //Debug.Log( movedAssets[ i ] );
                            hasToCheckFolders = true;

                            continue;
                        }


                        var newPathExternal = GetStoredDataPathExternal(movedAssets[i]);
                        var newPathInternal = GetStoredDataPathInternal(movedAssets[i]);
                        if ( newPathExternal == null )
						{
                            Debug.Log( "Cannot move " + movedAssets[ i ] );
                            continue;
						}


                        if ( File.Exists( newPathExternal ) )
                        {
                            if ( HierarchyExternalSceneData.TryCreateBackupForCache_ExternalCachePathInput( 
                                GetStoredDataPathExternal(newPathInternal)  ) ) //HierarchyExternalSceneData.GetStoredDataPathInternal(movedAssets[i])
                            {
                                //  AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
                                //  Adapter.RequestScriptReload();
                                wasBackUp = true;
                            }
                            File.Delete( newPathExternal );
                            if ( File.Exists( newPathExternal + ".meta" ) ) File.Delete( newPathExternal + ".meta" );
                        }
                        else
                        {
                            if ( !Directory.Exists( newPathExternal.Remove( newPathExternal.LastIndexOf( '/' ) ) ) )
                                Directory.CreateDirectory( newPathExternal.Remove( newPathExternal.LastIndexOf( '/' ) ) );
                        }
                        AssetDatabase.MoveAsset( GetStoredDataPathInternal( movedFromAssetPaths[ i ] ), newPathInternal );
                    }
                }

                if ( wasBackUp ) { }
            }
            if ( hasToCheckFolders ) CheckFolders();

        }

        static void TryFoundDeletedAssets( ref string[] importedAssets, ref string[] deletedAssets, ref string[] movedAssets, ref string[] movedFromAssetPaths )
        {
            if ( deletedAssets.Length != 0 )
            {
                foreach ( var item in Root.p )
                {
                    if ( item == null ) continue;
                    item.invoke_ReloadAfterAssetDeletingOrPasting();
                    //    Hierarchy.HierarchyAdapterInstance.Again_Reloder_UsingWhenCopyPastOrAssets();
                }
            }
        }
    }


}
