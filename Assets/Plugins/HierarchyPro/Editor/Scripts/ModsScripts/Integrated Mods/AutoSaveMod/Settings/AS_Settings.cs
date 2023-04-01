using System.IO;
using System.Linq;

using UnityEngine;



namespace EMX.HierarchyPlugin.Editor
{

	partial class EditorSettingsAdapter
    {


        internal int AS_SAVE_INTERVAL_IN_MIN { get { return Mathf.Clamp( GET( "AS_SAVE_INTERVAL_IN_MIN", 5 ), 1, 60 ); } set { var r = AS_SAVE_INTERVAL_IN_MIN; SET( "AS_SAVE_INTERVAL_IN_MIN", value ); } }
        internal bool AS_LOG { get { return GET( "AS_LOG", false ); } set { var r = AS_LOG; SET( "AS_LOG", value ); } }

        internal int AS_SAVE_INTERVAL_IN_SEC {
            get { return (int)AS_SAVE_INTERVAL_IN_MIN * 60; }
            set { AS_SAVE_INTERVAL_IN_MIN = (value / 60); ; }
        }

        internal int AS_FILES_COUNT { get { return GET( "AS_FILES_COUNT", 10 ); } set { var r = AS_FILES_COUNT; SET( "AS_FILES_COUNT", value ); } }
        internal string AS_LOCATION {
            get {
                var res =  GET("AS_LOCATION", "AutoSave");
                if ( string.IsNullOrEmpty( res ) ) return "AutoSave";
                return res;
            }
            set { var r = AS_LOCATION; SET( "AS_LOCATION", value ); }
        }

        internal int AS_FILES_STYLE { get { return GET( "AS_FILES_STYLE", 1 ); } set { var r = AS_FILES_STYLE; SET( "AS_FILES_STYLE", value ); } }
        internal string AS_FILES_PATTERN {
            get { return GET( "AS_FILES_NAME_PATTERN", " [SCENE] (MM.DD.YY) hhhmmmsss" ); }
            set {
                var r = AS_FILES_STYLE;
                var inv = Path.GetInvalidPathChars();
                var res = value.ToCharArray().Where( c => !inv.Contains( c ) ).Select(c=>c.ToString()).Aggregate( ( a, b ) => a.ToString() + b.ToString() )
                    .Trim(new[]{ '\n' ,'\t' ,'\b' }).Split('\n').FirstOrDefault();
                SET( "AS_FILES_NAME_PATTERN", (res ?? "").Trim( new[] { '\n', '\t', '\b' } ) );
            }
        }


        //Not Serialized



    }
}
