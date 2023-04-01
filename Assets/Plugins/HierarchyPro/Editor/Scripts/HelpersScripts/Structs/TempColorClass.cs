using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace EMX.HierarchyPlugin.Editor
{



	internal enum CopyType
    {
        BG,
        LABEL
    }
    internal class TempColorClass
    {

        //internal string tooltip = null;//12
        //bool _add_hasiconcolor;//13
        //Color _add_iconcolor;//14
        internal string[] el = new string[L];

        const int L = 15;
        static Color color32;
        internal static string[] _el = new string[L];
        bool protect_add;
        internal bool LockOverwrite = false;




        Texture2D _add_icon_cache;//11
        string guid_cache = "";
        internal Texture2D add_icon {
            get {
                if ( el.Length < L ) Array.Resize( ref el, L );
                if ( el[ 11 ] == "" || el[ 11 ] == null ) return null;
                if ( guid_cache == el[ 11 ] ) return _add_icon_cache;
                guid_cache = el[ 11 ];
                var p = AssetDatabase.GUIDToAssetPath(el[11]);
                _add_icon_cache = null;
                if ( p == null || p == "" ) return null;
                var t = AssetDatabase.LoadAssetAtPath<Texture2D>(p);
                _add_icon_cache = t;
                return t;
            }
            set {
                //if (protect_add) throw new Exception("TempColor locked");
                if ( el.Length < L ) Array.Resize( ref el, L );
                _add_icon_cache = value;
                if ( !value )
                {
                    guid_cache = el[ 11 ] = null;
                }
                else
                {
                    guid_cache = el[ 11 ] = AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( value ) );
                }
            }
        }
        internal string icon_tooltip {
            get {
                if ( el.Length < L ) Array.Resize( ref el, L );
                return el[ 12 ] ?? "";
            }
            set {
                //if (protect_add) throw new Exception("TempColor locked");
                if ( el.Length < L ) Array.Resize( ref el, L );
                el[ 12 ] = value;
            }
        }
        internal bool add_hasiconcolor {
            get {
                if ( el.Length < L ) Array.Resize( ref el, L );
                return el[ 13 ] == "1";
            }
            set {
                //if (protect_add) throw new Exception("TempColor locked");
                if ( el.Length < L ) Array.Resize( ref el, L );
                el[ 13 ] = value ? "1" : "";
            }
        }

        Color32 _color_cache;//11
        string color_guid_cache = "";
        internal Color add_iconcolor {
            get {
                if ( el.Length < L ) Array.Resize( ref el, L );
                if ( color_guid_cache != el[ 14 ] )
                {
                    color_guid_cache = el[ 14 ];
                    _color_cache = _getColor( ref el[ 14 ] );
                }
                return _color_cache;
            }
            set {
                //if (protect_add) throw new Exception("TempColor locked");
                if ( el.Length < L ) Array.Resize( ref el, L );
                _color_cache = value;
                el[ 14 ] = color_guid_cache = _getColor( _color_cache );
            }
        }

        static Color32 _getColor_cache;
        static Color32 _getColor( ref string s )
        {
            if ( s == null || s == "" ) return Color.clear;
            var split = s.Split('#');
            if ( split.Length != 4 )
            {
                _getColor_cache = Color.white;
                //_getColor_cache = Color.clear;
            }
            else
            {
                _getColor_cache.r = parse( ref split[ 0 ] );
                _getColor_cache.g = parse( ref split[ 1 ] );
                _getColor_cache.b = parse( ref split[ 2 ] );
                _getColor_cache.a = parse( ref split[ 3 ] );
            }
            return _getColor_cache;
        }
        static string _getColor( Color32 c )
        {
            return c.r + "#" + c.g + "#" + c.b + "#" + c.a;
        }
        static byte parse( ref string s )
        {
            if ( s == "" || s == null ) return 0;
            return byte.Parse( s );
        }

        int lastBGColorHash = -1;
        Color32 lastBGColorValue;
        internal Color32 BGCOLOR {
            get {
                if ( el.Length < 9 ) Array.Resize( ref el, 9 );

                var h = el[0] == null ? 0 : ( el[0].GetHashCode() ^ el[1].GetHashCode() ^el[2].GetHashCode() ^ el[3].GetHashCode() ) ;
                if ( h == lastBGColorHash ) return lastBGColorValue;

                lastBGColorValue = new Color32( parse( ref el[ 0 ] ), parse( ref el[ 1 ] ), parse( ref el[ 2 ] ), parse( ref el[ 3 ] ) );
                lastBGColorHash = h;
                return lastBGColorValue;
            }
            set {
                if ( el.Length < 9 ) Array.Resize( ref el, 9 );
                el[ 0 ] = value.r.ToString();
                el[ 1 ] = value.g.ToString();
                el[ 2 ] = value.b.ToString();
                el[ 3 ] = value.a.ToString();

                var h =  el[0] == null ? 0 : ( el[0].GetHashCode() ^ el[1].GetHashCode() ^el[2].GetHashCode() ^ el[3].GetHashCode() ) ;
                lastBGColorHash = h;
                lastBGColorValue = value;
            }
        }

        int lastLabelColorHash = -1;
        Color32 lastLabelColorValue;
        internal Color32 LABELCOLOR {
            get {
                if ( el.Length < 9 ) Array.Resize( ref el, 9 );
                //return new Color32((byte)el[5], (byte)el[6], (byte)el[7], (byte)el[8]);
                var h = el[5] == null ? 0 :( el[5].GetHashCode() ^ el[6].GetHashCode() ^el[7].GetHashCode() ^ el[8].GetHashCode());
                if ( h == lastLabelColorHash ) return lastLabelColorValue;


                lastLabelColorValue = new Color32( parse( ref el[ 5 ] ), parse( ref el[ 6 ] ), parse( ref el[ 7 ] ), parse( ref el[ 8 ] ) );
                lastLabelColorHash = h;
                return lastLabelColorValue;

                //return new Color32( parse( ref el[ 5 ] ), parse( ref el[ 6 ] ), parse( ref el[ 7 ] ), parse( ref el[ 8 ] ) );
            }
            set {
                if ( el.Length < 9 ) Array.Resize( ref el, 9 );
                el[ 5 ] = value.r.ToString();
                el[ 6 ] = value.g.ToString();
                el[ 7 ] = value.b.ToString();
                el[ 8 ] = value.a.ToString();

                var h = el[5] == null ? 0 :( el[5].GetHashCode() ^ el[6].GetHashCode() ^el[7].GetHashCode() ^ el[8].GetHashCode());
                lastLabelColorHash = h;
                lastLabelColorValue = value;
            }
        }



        public override int GetHashCode()
        {
            if ( el == null ) return -1;
            int _th = 0;
            var hashCode = -1973547999;
            //hashCode = hashCode * -1521134295 + EqualityComparer<Color32>.Default.GetHashCode(BGCOLOR);
            //hashCode = hashCode * -1236236295 + EqualityComparer<Color32>.Default.GetHashCode(LABELCOLOR);
            //hashCode = hashCode * -1525235251 + child.GetHashCode();
            for ( int i = 0; i < el.Length; i++ ) _th ^= _th * -0525623251 + (string.IsNullOrEmpty( el[ i ] ) ? "0" : el[ i ]).GetHashCode() * 1000;
            // if ( el.list.Count >= 5 ) _th ^= _th * -0525623251 + el.list[ 4 ] * 1000;
            hashCode = hashCode * -1521134295 + _th;
            return hashCode;
        }

        public static bool operator ==( TempColorClass a, TempColorClass b )
        {
            if ( ReferenceEquals( a, null ) && ReferenceEquals( b, null ) ) return true;
            if ( ReferenceEquals( a, null ) || ReferenceEquals( b, null ) ) return false;

            if ( !ReferenceEquals( a, null ) && a.protect_add && ReferenceEquals( b, null ) )
            {
                throw new Exception( "Protected TempColor cannot be null" );
                //return a.add_icon == null;
            }


            var result = true;
            result &= Colors.Eq( a.BGCOLOR, b.BGCOLOR );
            result &= Colors.Eq( a.LABELCOLOR, b.LABELCOLOR );
            result &= a.child == b.child;

            if ( result )
            {
                if ( a.el == null || b.el == null ) return false;

                if ( a.el.Length <= 9 && b.el.Length <= 9 ) return true;

                if ( a.el.Length != b.el.Length ) return false;

                for ( int i = 9; i < b.el.Length; i++ )
                {
                    if ( (string.IsNullOrEmpty( a.el[ i ] ) ? "0" : a.el[ i ]) != (string.IsNullOrEmpty( b.el[ i ] ) ? "0" : b.el[ i ]) ) return false;
                }
            }

            return result;
        }

        public static bool operator !=( TempColorClass a, TempColorClass b )
        {
            return !(a == b);
        }

        public override bool Equals( object obj )
        {
            return (obj as TempColorClass) == this;
        }
        /*  internal bool? _mchild {get {return null; } set { } }
          internal bool? _mLABEL_SHADOW {get {return null; } set { } }
          internal int? _mBG_ALIGMENT_LEFT {get {return null; } set { } }
          internal int? _mBG_ALIGMENT_RIGHT {get {return null; } set { } }
          internal int? _mBG_FILL {get {return null; } set { } }
          internal int? _mBG_HEIGHT {get {return null; } set { } }*/


        internal TempColorClass AddIcon( Texture2D icon, bool hascolor, int[] color )
        {
            this.add_icon = icon;
            this.add_hasiconcolor = hascolor;
            color32.r = color[ 0 ] / 255f;
            color32.g = color[ 1 ] / 255f;
            color32.b = color[ 2 ] / 255f;
            color32.a = color[ 3 ] / 255f;
            this.add_iconcolor = color32;
            protect_add = true;
            return this;
        }

        internal TempColorClass AddIcon( Texture2D icon, bool hascolor, Color color )
        {
            this.add_icon = icon;
            this.add_hasiconcolor = hascolor;
            this.add_iconcolor = color;
            protect_add = true;
            return this;
        }

        internal TempColorClass AddIcon( Texture2D icon )
        {
            this.add_icon = icon;
            this.add_hasiconcolor = false;
            protect_add = true;
            return this;
        }

        internal TempColorClass AddIcon( Texture2D icon, string tooltip )
        {
            this.icon_tooltip = tooltip;
            AddIcon( icon );
            return this;
        }




        internal void Reset( ref string[] __el )
        {
            //LABELCOLOR = BGCOLOR = Color.clear;
            //add_hasiconcolor = false;
            //add_icon = null;
            //add_iconcolor = Color.clear;

            if ( LockOverwrite ) throw new Exception( "LockOverwrite" );

            if ( el == null ) el = new string[ L ];
            if ( el.Length < __el.Length ) Array.Resize( ref el, __el.Length );
            for ( int i = 0; i < __el.Length; i++ )
                if ( el[ i ] != __el[ i ] ) el[ i ] = __el[ i ];

            for ( int i = __el.Length; i < el.Length; i++ )
                if ( el[ i ] == null || el[ i ].Length != 1 || el[ i ][ 0 ] != '0' ) el[ i ] = "0";
        }

        internal void Clear() // Reset( el );
        {
            LABELCOLOR = BGCOLOR = Color.clear;
            add_hasiconcolor = false;
            add_icon = null;
            add_iconcolor = Color.clear;

            if ( el == null ) el = new string[ L ];
            if ( el.Length < L ) Array.Resize( ref el, L );
            //if (el != null) //el = new int[ 0 ];

            for ( int i = 0; i < el.Length; i++ )
                //return IS_STRING_EMPTY( ref el[ 5 ] ) || IS_STRING_EMPTY( ref el[ 6 ] ) || IS_STRING_EMPTY( ref el[ 7 ] ) || IS_STRING_EMPTY( ref el[ 8 ] );
                if ( el[ i ] == null || el[ i ].Length != 1 || el[ i ][ 0 ] != '0' ) el[ i ] = "0";
        }

        internal TempColorClass empty {
            get { /*Reset( _el );
				for ( int i = 0 ; i < el.list.Count ; i++ )
				{   el.list[i] = 0;
				}*/
                Clear();
                return this;
            }
        }



        internal static void CopyFromTo( CopyType type, TempColorClass from, ref TempColorClass to, bool skipChild = false )
        {
            switch ( type )
            {
                case CopyType.LABEL:
                    to.LABELCOLOR = from.LABELCOLOR;
                    to.LABEL_SHADOW = from.LABEL_SHADOW;
                    if ( !skipChild && from.child ) to.child = true;
                    break;

                case CopyType.BG:
                    to.BGCOLOR = from.BGCOLOR;
                    to.BG_ALIGMENT_LEFT = from.BG_ALIGMENT_LEFT;
                    to.BG_ALIGMENT_RIGHT = from.BG_ALIGMENT_RIGHT;
                    to.BG_HEIGHT = from.BG_HEIGHT;
                    to.BG_WIDTH = from.BG_WIDTH;
                    to.BG_FILL = from.BG_FILL;
                    if ( !skipChild && from.child ) to.child = true;
                    break;
            }

            // to.el.list = from.ToList();
        }

        internal void OverrideTo( ref TempColorClass to )
        {
            if ( this.HAS_BG_COLOR || this.HAS_LABEL_COLOR )
            {
                to.child = this.child;

                if ( this.HAS_BG_COLOR ) CopyFromTo( CopyType.BG, this, ref to );

                if ( this.HAS_LABEL_COLOR ) CopyFromTo( CopyType.LABEL, this, ref to );
            }

            if ( this.add_icon )
            {
                to.add_icon = this.add_icon;

                if ( this.add_hasiconcolor )
                {
                    to.add_hasiconcolor = this.add_hasiconcolor;
                    to.add_iconcolor = this.add_iconcolor;
                }
            }
        }
        internal void OverrideToIcon( ref TempColorClass to )
        {


            if ( this.add_icon )
            {
                to.add_icon = this.add_icon;

                if ( this.add_hasiconcolor )
                {
                    to.add_hasiconcolor = this.add_hasiconcolor;
                    to.add_iconcolor = this.add_iconcolor;
                }
            }
        }



        internal TempColorClass AssignFromList( int v, bool andLock ) // var _ConverterFull = this;
        {
            //BGCOLOR.r = 0;
            //BGCOLOR.g = 0;
            //BGCOLOR.b = 0;
            //BGCOLOR.a = 0;

            if ( el == null ) el = new string[ L ];
            else for ( int i = 0; i < el.Length; i++ ) el[ i ] = "0";

            //  child = false;
            //LABELCOLOR.r = 0;
            //LABELCOLOR.g = 0;
            //LABELCOLOR.b = 0;
            //LABELCOLOR.a = 0;
            LockOverwrite = andLock;
            return this;
        }

        internal TempColorClass AssignFromList( ref string[] source, bool andLock )
        {
            var res = AssignFromList(ref source);
            LockOverwrite = andLock;
            return res;
        }

        internal TempColorClass AssignFromList( ref string[] source ) // var _ConverterFull = this;
        {
            Reset( ref source );
            //BGCOLOR.r = (byte)source[0];
            //BGCOLOR.g = (byte)source[1];
            //BGCOLOR.b = (byte)source[2];
            //BGCOLOR.a = (byte)source[3];
            //child = source[4] == 1;

            if ( source.Length > 5 )
            {
                //LABELCOLOR.r = (byte)source[5];
                //LABELCOLOR.g = (byte)source[6];
                //LABELCOLOR.b = (byte)source[7];
                //LABELCOLOR.a = (byte)source[8];
            }

            return this;
        }

        internal TempColorClass CopyFromFilter( TempColorClass source ) // var _ConverterFull = this;
        { // Reset(null);

            var L = Mathf.Max(el.Length, source.el.Length);
            if ( el.Length < source.el.Length ) Array.Resize( ref el, source.el.Length );
            for ( int i = 9; i < L; i++ )
                el[ i ] = i < source.el.Length ? source.el[ i ] : "0";

            BGCOLOR = source.BGCOLOR;
            child = source.child;
            LABELCOLOR = source.LABELCOLOR;

            return this;
        }

        internal TempColorClass CopyFromList( ref string[] source ) //var _ConverterFull = this;
        { // Reset(null);
          //BGCOLOR.r = (byte)source[0];
          //BGCOLOR.g = (byte)source[1];
          //BGCOLOR.b = (byte)source[2];
          //BGCOLOR.a = (byte)source[3];

            if ( source.Length > 5 )
            {
                //LABELCOLOR.r = (byte)source[5];
                //LABELCOLOR.g = (byte)source[6];
                //LABELCOLOR.b = (byte)source[7];
                //LABELCOLOR.a = (byte)source[8];
            }
            //else
            //LABELCOLOR = Color.clear;

            var L = Mathf.Max(el.Length, source.Length);
            if ( el.Length < source.Length ) Array.Resize( ref el, source.Length );
            for ( int i = 0; i < L; i++ )
                el[ i ] = i < source.Length ? source[ i ] : "0";

            //child = source[4] == 1;


            return this;
        }

        /* internal bool child {get {return _mchild ?? (_mchild = el. list[4] == 1).Value; } set { el.SetByte(4, 0, 1, (_mchild = value).Value ? 1 : 0);} }
         internal bool LABEL_SHADOW {get {return  _mLABEL_SHADOW ?? (_mLABEL_SHADOW = el.GetByte(10, 0, 1) == 1).Value; } }
         internal int BG_ALIGMENT_LEFT  {get {return  _mBG_ALIGMENT_LEFT ?? (_mBG_ALIGMENT_LEFT = el.GetByte(9, 0, 3) ).Value; } }
         internal int BG_ALIGMENT_RIGHT  {get {return   _mBG_ALIGMENT_RIGHT ?? (_mBG_ALIGMENT_RIGHT = el.GetByte(9, 3, 3) ).Value;} }
         internal int BG_FILL  {get {return  _mBG_FILL ?? (_mBG_FILL = el.GetByte(9, 6, 1) ).Value; } }
         internal int BG_HEIGHT  {get {return  _mBG_HEIGHT ?? (_mBG_HEIGHT = el.GetByte(9, 7, 1)).Value ;} }*/
        internal bool child {
            get { return el[ 4 ] == "1"; }//&& BG_HEIGHT != 2

            set { el[ 4 ] = value ? "1" : "0"; }
        } //set { el.SetByte(4, 0, 1,  value ? 1 : 0);} }

        static bool IS_STRING_NOTEMPTY( ref string s )
        {
            if ( s == null || s == "" || s.Length == 1 && s[ 0 ] == '0' ) return false;
            return true;
        }

        internal bool HAS_BG_COLOR {
            get {
                if ( el.Length < 9 ) Array.Resize( ref el, 9 );
                //return new Color32( parse( ref el[ 0 ] ), parse( ref el[ 1 ] ), parse( ref el[ 2 ] ), parse( ref el[ 3 ] ) );
                return IS_STRING_NOTEMPTY( ref el[ 0 ] ) || IS_STRING_NOTEMPTY( ref el[ 1 ] ) || IS_STRING_NOTEMPTY( ref el[ 2 ] ) || IS_STRING_NOTEMPTY( ref el[ 3 ] );
                //return el[ 0 ] != "0" || el[ 1 ] != "0" || el[ 2 ] != "0" || el[ 3 ] != "0";
                //return BGCOLOR.r > 0 || BGCOLOR.g > 0 || BGCOLOR.b > 0 || BGCOLOR.a > 0; }
            }
        }

        internal bool HAS_LABEL_COLOR {
            get {
                if ( el.Length < 9 ) Array.Resize( ref el, 9 );
                return IS_STRING_NOTEMPTY( ref el[ 5 ] ) || IS_STRING_NOTEMPTY( ref el[ 6 ] ) || IS_STRING_NOTEMPTY( ref el[ 7 ] ) || IS_STRING_NOTEMPTY( ref el[ 8 ] );
                //return el[ 5 ] != "0" || el[ 6 ] != "0" || el[ 7 ] != "0" || el[ 8 ] != "0";
                //return LABELCOLOR.r > 0 || LABELCOLOR.g > 0 || LABELCOLOR.b > 0 || LABELCOLOR.a > 0; 
            }
        }

        internal bool LABEL_SHADOW {
            get { return Tools.GetByte( el[ 10 ], 0, 1 ) == 1; }

            set { Tools.SetByte( ref el[ 10 ], 0, 1, value ? 1 : 0 ); }
        }

        internal int BG_WIDTH {
            get {
                var t = Tools.GetByte(el[10], 1, 8);

                if ( t == 0 ) return 80;

                return t;
            }

            set { Tools.SetByte( ref el[ 10 ], 1, 8, value ); }
        }

        internal BgAligmentLeft BG_ALIGMENT_LEFT_CONVERTED {
            get { return PluginInstance.BgAligmentLeftArray[ BG_ALIGMENT_LEFT ]; }
        }

        internal int BG_ALIGMENT_LEFT {
            get { return Tools.GetByte( el[ 9 ], 0, 3 ); }

            set {
                if ( value < 5 && BG_ALIGMENT_RIGHT < 5 )
                {
                    var estv = 4 - value;

                    if ( estv < BG_ALIGMENT_RIGHT ) BG_ALIGMENT_RIGHT = estv;
                }

                Tools.SetByte( ref el[ 9 ], 0, 3, value );
            }
        }

        //  internal string[] ALIGMENT_LEFT_CATEGORIES = new [] { "<<Min", "•Icon", "•Label", "Label•", "•Modules"};
        internal string[] ALIGMENT_LEFT_CATEGORIES = new[] { "<<Min", "•Fold", "•Label", "Label•", "•Modules" };

        internal BgAligmentRight BG_ALIGMENT_RIGHT_CONVERTED {
            get { return PluginInstance.BgAligmentToRightArray[ BG_ALIGMENT_RIGHT ]; }
        }

        internal int BG_ALIGMENT_RIGHT {
            get { return Tools.GetByte( el[ 9 ], 3, 3 ); }

            set {
                if ( value < 5 && BG_ALIGMENT_LEFT < 5 )
                {
                    var estv = 4 - value;

                    if ( estv < BG_ALIGMENT_LEFT ) BG_ALIGMENT_LEFT = estv;
                }

                Tools.SetByte( ref el[ 9 ], 3, 3, value );
            }

            /* get {return    el.GetByte(9, 3, 3) ;} set
            {   if (value < 5 && BG_ALIGMENT_LEFT < 5)
                {   var estv = 4 - BG_ALIGMENT_LEFT;
                    if (value > estv) BG_ALIGMENT_LEFT = 4 - value;
                }
                el.SetByte(9, 3, 3,  value );
            }*/
        }

        internal string[] ALIGMENT_RIGHT_CATEGORIES = new[] { "•Fold", "•Label", "Label•", "•Modules", "Max>>" };

        //internal string[] ALIGMENT_RIGHT_CATEGORIES = new [] { "•Icon", "•Label", "Label•", "•Modules", "Max>>"};
        internal int BG_FILL {
            get { return Tools.GetByte( el[ 9 ], 6, 1 ); }

            set { Tools.SetByte( ref el[ 9 ], 6, 1, value ); }
        }

        internal int BG_HEIGHT {
            get {
                if ( child )
                {
                    var res = Tools.GetByte(el[9], 7, 2);
                    if ( res == 2 ) return 0;
                }
                return Tools.GetByte( el[ 9 ], 7, 2 );
            }

            set { Tools.SetByte( ref el[ 9 ], 7, 2, value ); }
        }

        internal string[] ToListNoClone()
        {
            if ( el == null ) el = new string[ L ];
            return el;
        }

        internal string[] ToList()
        {
            if ( el == null ) el = new string[ L ];
            if ( el.Length < L ) Array.Resize( ref el, L );
            var result = el.ToArray();
            //result[0] = (BGCOLOR.r);
            //result[1] = (BGCOLOR.g);
            //result[2] = (BGCOLOR.b);
            //result[3] = (BGCOLOR.a);
            //result[5] = (LABELCOLOR.r);
            //result[6] = (LABELCOLOR.g);
            //result[7] = (LABELCOLOR.b);
            //result[8] = (LABELCOLOR.a);
            //
            //result[4] = child == true ? 1 : 0;


            /*   result.Add( BGCOLOR.r );
               result.Add( BGCOLOR.g );
               result.Add( BGCOLOR.b );
               result.Add( BGCOLOR.a );
               result.Add( child == true ? 1 : 0 );
               result.Add( LABELCOLOR.r );
               result.Add( LABELCOLOR.g );
               result.Add( LABELCOLOR.b );
               result.Add( LABELCOLOR.a );
               result.Add( BG_ALIGMENT_LEFT << 0 | BG_ALIGMENT_RIGHT << 3 | BG_FILL << 6 | BG_HEIGHT << 7 );
               result.Add( (LABEL_SHADOW ? 1 : 0) | (BG_WIDTH << 1) );*/
            return result;
        }



        static StringBuilder sb = new StringBuilder();
        internal string ConvertToString()
        {
            var list = ToListNoClone();
            if ( list.Length < L ) Array.Resize( ref list, L );
            //list[9] = BG_ALIGMENT_LEFT << 0 | BG_ALIGMENT_RIGHT << 3 | BG_FILL << 6 | BG_HEIGHT << 7;
            //list[10] = (LABEL_SHADOW ? 1 : 0) | (BG_WIDTH << 1);
            sb.Clear();
            for ( int i = 0; i < list.Length; i++ )
            {
                if ( i != 0 ) sb.Append( ' ' );
                sb.Append( list[ i ] == "" || list[ i ] == null ? "0" : list[ i ] );
            }

            return sb.ToString();
        }
        internal void SetFromString( string source )
        {
            var split = source.Split(' ');
            var ar = new string[split.Length];
            for ( int i = 0; i < split.Length; i++ ) ar[ i ] = (split[ i ]); //
            AssignFromList( ref ar );
        }
    }

}
