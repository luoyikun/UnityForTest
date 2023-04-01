using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor
{




	[CustomEditor( typeof( HierarchyExternalSceneData ) )]
	class HierarchyExternalSceneDataEditor : UnityEditor.Editor
	{


		static HierarchyExternalSceneData copiedScriptableObject;

		public override void OnInspectorGUI()
		{
			if ( Application.isPlaying )
			{
				GUILayout.Label( "Not allow while application is playing" );
				return;
			}

			if ( GUILayout.Button( "Copy", GUILayout.Height( 30 ) ) ) copiedScriptableObject = target as HierarchyExternalSceneData;
			var en = GUI.enabled;
			GUI.enabled &= copiedScriptableObject != null;
			var name = "Paste";
			if ( copiedScriptableObject ) name += " " + copiedScriptableObject.name;
			if ( GUILayout.Button( name, GUILayout.Height( 30 ) ) )
			{
				var temp = (target) as HierarchyExternalSceneData;
				if ( temp )
				{

					Undo.RecordObject( temp, "Paste DescriptionHelper" );

					temp.CopyFrom( copiedScriptableObject );

					HierarchyTempSceneData.RemoveCache();
					Root.p[ 0 ].invoke_ReloadAfterAssetDeletingOrPasting();
					//HierarchyExternalSceneData.();
					EditorUtility.SetDirty( temp );
					//   adapter.MarkSceneDirty(SceneManager.GetActiveScene());
				}

				/* var path = AssetDatabase.GetAssetPath(target);
                 if (temp && !string.IsNullOrEmpty( path ))
                 {
                   AssetDatabase.DeleteAsset( path );
                   Hierarchy.M_Descript.RemoveIHashPropertY( temp );
                   AssetDatabase.CreateAsset( copiedScriptableObject, path );
                 }*/
			}

			GUI.enabled = en;

			GUILayout.Space( 20 );
			if ( GUILayout.Button( "Update Scene Data (Hot fix if bugs after undo)", GUILayout.Height( 16 ) ) )
			{
				var temp = (target) as HierarchyExternalSceneData;
				if ( temp )
				{
					Undo.RecordObject( temp, "Paste DescriptionHelper" );
					HierarchyTempSceneData.RemoveCache();
					Root.p[ 0 ].invoke_ReloadAfterAssetDeletingOrPasting();
					EditorUtility.SetDirty( temp );
				}
			}
			GUILayout.Space( 20 );

			if ( GUILayout.Button( "Clear All Scene Data", GUILayout.Height( 16 ) ) )
			{
				if ( EditorUtility.DisplayDialog( "Do you want to remove all data?", "Do you want to remove all data?", "Yes", "No" ) )
				{
					var temp = (target) as HierarchyExternalSceneData;
					if ( temp )
					{
						Undo.RecordObject( temp, "Paste DescriptionHelper" );
						temp.ClearData();
						HierarchyTempSceneData.RemoveCache();
						Root.p[ 0 ].invoke_ReloadAfterAssetDeletingOrPasting();
						EditorUtility.SetDirty( temp );
					}
				}
			}



			GUILayout.Space( 50 );
			var c= Color.white;
			c.a = 0.5f;
			GUI.color *= c;
			GUILayout.Label( "Internal Debug Data:" );
			DrawDefaultInspector();
			/*     GUI.enabled = true;
                  var temp = (target) as HierarchyExternalSceneData;
                  GUILayout.Space( 10 );
                  if ( temp && GUILayout.Button( "Apply to current Scene", GUILayout.Height( 30 ) ) )
                  {
                      for ( int i = 0 ; i < EditorSceneManager.sceneCount ; i++ )
                      {
                          var s = EditorSceneManager.GetSceneAt(i);
                          if ( !s.IsValid() || !s.isLoaded ) continue;
                          var d = Hierarchy.M_Descript.des(s.GetHashCode());
                          var assetPath = AssetDatabase.GetAssetPath(target);
                          assetPath = assetPath.Remove( assetPath.LastIndexOf( '.' ) );
                          var scenePath = Adapter.GetScenePath(s);
                          scenePath = scenePath.Remove( scenePath.LastIndexOf( '.' ) );
                          if ( d == null || !assetPath.EndsWith( scenePath ) ) continue;

                          Hierarchy.M_Descript.RemoveIHashPropertY( d );
                          Hierarchy.HierarchyAdapterInstance.SET_UNDO( d, "Apply to current Scene" );
                          Adapter.SET_HASH_WITHOUT_LOCALID( temp, d );
                          Hierarchy.HierarchyAdapterInstance.SetDirtyDescription( d, s );

                          Hierarchy.HierarchyAdapterInstance.EditorSceneManagerOnSceneOpening( null, OpenSceneMode.Single );
                      }
                  }*/
		}
	}
}
