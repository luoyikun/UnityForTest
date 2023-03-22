using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor
{
	/*[Serializable]
	public class Int32ListArray : ICloneable
	{
		[SerializeField]
		public float ColorValue_R;

		[SerializeField]
		public float ColorValue_G;

		[SerializeField]
		public float ColorValue_B;

		[SerializeField]
		public float ColorValue_A;

		[SerializeField]
		public bool SKIP_COLOR;
		public bool NO_EXPAND_ITEMS;

		private static Color c;
		public List<Int32List> array;
		public float Hierarchy_ScrollY;

		public List<long> ColabsedItems;
	}*/

	[Serializable]
    internal class FolderBookmark : IBgColor
    {
        internal FolderBookmark()
        {
            InstanceID = (new System.Random()).Next( 10001, int.MaxValue );
        }

        [SerializeField]
        internal float scrollY = 0;

        [SerializeField]
        internal List<BookSlot> slots = new List<BookSlot>();
        [SerializeField]
        internal bool expanded = true;
        [SerializeField]
        internal int InstanceID = 10001;
        [SerializeField]
        internal int ShowContent = 0;
        [SerializeField]
        internal string name = "";


        public bool isDefault = false;
        public string category_name {
            get {
                if ( isDefault ) return "Default";
                return name;
            }
            set {
                if ( isDefault ) return;
                name = Folders.FIX_NAME( value );
            }
        }
        public void SetAsDefault()
        {
            if ( isDefault ) return;
            isDefault = true;
            name = "Default";
        }

        public string get_name { get { return name; } }
        public Color? BgColor {
            get {
                if ( !draw_BgColor ) return (Color?)null;
                return color_BgColor;
            }
            set {
                if ( value == null ) draw_BgColor = false;
                else
                {
                    draw_BgColor = true;
                    color_BgColor = value.Value;
                }
            }
        }
        [SerializeField]
        bool draw_BgColor = false;
        [SerializeField]
        Color color_BgColor = Color.clear;

    }
    [Serializable]
    internal class BookSlot
    {
        [SerializeField]
        internal bool slot_expanded = true;
        [SerializeField]
        internal string description = "";
        [SerializeField]
        internal List<BookObject> guids = new List<BookObject>();
        [SerializeField]
        internal int ShowContentInh = 0;
        [SerializeField]
        internal string FilterString = "";

        internal bool IsValid() { return true; }
        internal UnityEngine.Object[] ConvertToObjects()
        {
            return guids.Select( g => g.ConvertToObject() ).Where( o => o ).ToArray();
        }
        internal void SetFromObjects( UnityEngine.Object[] obs )
        {
            guids.Clear();
            foreach ( var o in obs )
            {
                if ( !o ) continue;
                guids.Add( new BookObject() );
                if ( !guids[ guids.Count - 1 ].SetFromObject( o ) )
                {
                    guids.RemoveAt( guids.Count - 1 );
                }
            }
        }

        public bool OnClick( bool par1, int scene )
        {

            var result = ConvertToObjects();
            if ( result.Length == 0 ) return false;
            if ( Event.current != null && Event.current.isMouse )
            {
                var STATE = Tools.GET_SELECTION_STATE(false, true, true);

                switch ( STATE )
                {
                    case 0:
                    case 3:

                        //  Selection.Set
                        //if (STATE == 3) bottomInterface.adapter.SAVE_SCROLL();

                        Selection.objects = result;
                        //bottomInterface.LastIndex = ArrayIndex;
                        break;

                    case 1:
                        Selection.objects = Selection.objects.Concat( result ).ToArray();
                        break;

                    case 2:
                        Selection.objects = Selection.objects.Except( result ).ToArray();
                        break;
                }
            }

            else
            {
                Selection.objects = result;
            }
            return true;
        }

        static Dictionary<int, bool> _cache = new Dictionary<int, bool>();
        string _key( int id )
        {
            var b0 = (char)(byte)(id & 0xFF);
            var b1 = (char)(byte)((id >> 8) & 0xFF);
            var b2 = (char)(byte)((id >> 16) & 0xFF);
            var b3 = (char)(byte)((id >> 24) & 0xFF);
            return String.Concat( b0, b1, b2, b3 );
        }
        void _check( int id )
        {
            if ( !_cache.ContainsKey( id ) )
            {

                _cache.Add( id, SessionState.GetBool( "EMX_HU_" + _key( id ), false ) );
            }
        }
        internal void SwitchExpandInstanceID( int id )
        {
            _check( id );
            _cache[ id ] = !_cache[ id ];
            SessionState.SetBool( "EMX_HU_" + _key( id ), _cache[ id ] );

        }
        internal bool GetExpandInstanceID( int id )
        {
            _check( id );
            return _cache[ id ];
        }
    }
    [Serializable]
    internal class BookObject
    {
        [SerializeField]
        internal string guid;

        internal UnityEngine.Object ConvertToObject()
        {
            var p = AssetDatabase.GUIDToAssetPath(guid);
            if ( p == null || p == "" ) return null;
            return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( p );
        }
        internal bool SetFromObject( UnityEngine.Object o )
        {
            guid = "";
            if ( !o ) return false;
            var p = AssetDatabase.GetAssetPath(o);
            if ( p == null || p == "" ) return false;
            guid = AssetDatabase.AssetPathToGUID( p );
            return (guid != null && guid != "");
        }
    }

    partial class HierarchyCommonData : ScriptableObject
    {





        [SerializeField]
        internal List<FolderBookmark> FoolderBookmarkList = new List<FolderBookmark>();


    }



}
