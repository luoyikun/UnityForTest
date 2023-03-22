using System.Linq;
using System.Collections.Generic;
using UnityEngine;



namespace EMX.HierarchyPlugin.Editor
{
	class UnityVersion
    {
        internal static float UNITY_2019_VERSION { get { return 2019; } }
        internal static float UNITY_2019_2_0_VERSION { get { return 2019.101f; } }
        internal static float UNITY_2019_3_0_VERSION { get { return 2019.3f; } }
        internal static float UNITY_2019_1_1_VERSION { get { return UNITY_2019_VERSION; } }

        static float? __UNITY_CURRENT_VERSION;
        static Dictionary<char,char> _isn ;
        static bool IsNumb( char c )
        {
            if ( _isn == null )
            {
                _isn = Enumerable.Repeat( 0, 10 ).Select( ( s, i ) => i.ToString()[ 0 ] ).ToDictionary( k => k, v => v );
                _isn.Add( '.', '.' );
            }
            return _isn.ContainsKey( c );
        }
        internal static float UNITY_CURRENT_VERSION
        {
            get
            {
                if ( __UNITY_CURRENT_VERSION.HasValue ) return __UNITY_CURRENT_VERSION.Value;

                var v = Application.unityVersion.Replace('f', '.').Replace('b', '.').Replace('a', '.').ToCharArray().Where(IsNumb).Select(c => c.ToString()).Aggregate((a, b) => a + b).Split('.');
                var year = int.Parse(v[0]);
                var quart = int.Parse(v[1]) / 10f;
                var revis = 0f;

                try
                {
                    revis = int.Parse( v[ 2 ] ) / 1000f;
                }

                catch { }

                __UNITY_CURRENT_VERSION = year + quart + revis;
                //	Debug.Log( Application.unityVersion + " " + __UNITY_CURRENT_VERSION + " " + UNITY_2019_1_1_VERSION );
                return __UNITY_CURRENT_VERSION.Value;
            }
        }
    }
}
