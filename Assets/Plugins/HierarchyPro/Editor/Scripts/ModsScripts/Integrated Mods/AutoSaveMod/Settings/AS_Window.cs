using System;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EMX.HierarchyPlugin.Editor.Mods;
using static EMX.HierarchyPlugin.Editor.Settings.SETGUI_AboutCache;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class AS_Window : ScriptableObject
	{
	}

	[CustomEditor( typeof( AS_Window ) )]
	class SETGUI_Autosave : MainRoot
	{

		internal static string set_text =  USE_STR + "AutoSave (Background)";//
		internal static string set_key = "USE_AUTOSAVE_MOD";
		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}
		public override void OnInspectorGUI()
		{
			_GUI( (IRepaint)this );
		}
		static CacheFolderDrawer.FOLDER_DATA data = new CacheFolderDrawer.FOLDER_DATA(
			prefix: "Assets/",
			valudate: ValidateFolder,
			doMovingConten: false,
			isInternal: true,
			isDelayedFinalization: false,
			CheckHiddenFoldersForInternal: true,
			trimResult: null,
			SubFolders: null,
			SubFiles: null
		);
		struct fd : IDisposable
		{
			public fd( Action a1, Action a2 )
			{
				if ( a1 != null ) a1();
				d = a2;
			}
			Action d;
			public void Dispose()
			{
				if ( d != null ) d();
			}
		}

		public static void _GUI( IRepaint w )
		{
			Draw.RESET( w );

			Draw.BACK_BUTTON( w );
			Draw.TOG_TIT( set_text, set_key, WIKI: WIKI_2_AUTOSAVE );
			Draw.Sp( 10 );

			using ( ENABLE( w ).USE( set_key ) )
			{


				using ( GRO( w ).UP( 0 ) )
				{
					Draw.FIELD( "Save Every (Minutes)", "AS_SAVE_INTERVAL_IN_MIN", 1, 60 );
					Draw.FIELD( "Maximum Files Version", "AS_FILES_COUNT", 1, 999 );

					//Draw.HELP( w, "You already have files that exceed the maximum number of files, you must manually delete the files, otherwise the asset will continue to use them." );
					//if (D)
					//var r = Draw.R;
					//r.width /= 3;
					//var r2 = r;
					//r2.width *= 2;

					using ( new fd( () => Draw.FOLDER_PAD = 20, () => Draw.FOLDER_PAD = 0 ) ) Draw.FOLDER( w, "Autosave location:", "AS_LOCATION", data );



					GUI.Label( Draw.R, "File names style:" );


					Draw.TOOLBAR( new[] { "Simple counter", "Special names" }, "AS_FILES_STYLE" );
					if ( p.par_e.AS_FILES_STYLE == 0 )
					{
						GUI.Label( Draw.R, "Example: 'AutoSave_" + Enumerable.Repeat( 0, p.par_e.AS_FILES_COUNT.ToString().Length ).Select( a => a.ToString() ).Aggregate( ( a, b ) => a + b ) + ".unity'" );
					}
					else
					{

						Draw.HELP( w, @"Use these settings to adjust your files name
SCENE : source file name
YYYY MM DD - date
hh mm ss - time" );
						Draw.STRING( "Pattern: ", "AS_FILES_PATTERN" );
						// GUI.Label( Draw.R, "Example: 'AutoSave" + AutoSaveMod.GET_PATTERN( AutoSaveMod.GET_SCENE_NAME(), 2021, 4, 24, 10, 11, 30 ) + ".unity'" );
						GUI.Label( Draw.R, "Example: 'AutoSave" + AutoSaveMod.GET_PATTERN( AutoSaveMod.GET_SCENE_NAME(), DateTime.Now ) + ".unity'" );

					}

					Draw.Sp( 10 );
					Draw.HRx1();
					Draw.TOG( "Log", "AS_LOG" );
					using ( ENABLE( w ).USE( "AS_LOG", padding: 0 ) )
					{
						if ( GUI.Button( Draw.R, "Debug Save Scene Now" ) )
						{
							AutoSaveMod.SaveScene();
						}
					}
					Draw.HELP( w, "Autosave timer stops during the PlayMode, for example, you set saving in every 5 minutes, and when there will 2 minutes left before saving, and you click PlayMode, and you will play 15 minutes, it will be also remained 2 minutes" );
					Draw.Sp( 10 );


				}
			}
		}

		static bool ValidateFolder( string p )
		{
			if ( Directory.GetFiles( p ).Length != 0 )
			{
				return EditorUtility.DisplayDialog(
					"Warning!",
					"Folder:\n...-" + p + "\nContains files, so if you will continue, some files started with 'AutoSave*' and ended with '*.unity' may be overwritten.\n\nDo you want to continue?",
					"Yes", "Cancel" );
			}
			return true;
		}
	}
}
