using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;



namespace EMX.HierarchyPlugin.Editor.Mods
{
	internal partial class HighlighterAutoMode_Instance
    {
        //HighlighterMod root;
        //PluginInstance adapter;
        int pluginID;
        internal HighlighterAutoMode_Instance( int pluginID )
        {
            //adapter = a; this.root = root;
            this.pluginID = pluginID;
        }


        internal void ClearCache()
        {
            if ( pluginID == 0 )
                foreach ( var item in Cache.m_instanceIdToHierObject_0 )
                {
                    item.Value.ah.filterAssigned = false;
                }
            else
                foreach ( var item in Cache.m_instanceIdToHierObject_1 )
                {
                    item.Value.ah.filterAssigned = false;
                }
            //Cache.ClearHierarchyObjects(pluginID == 1);
            //adapter.ClearHierarchyObjects(false);
        }
        //internal void ClearCacheFull()
        //{
        //	Cache.ClearHierarchyObjects(pluginID == 1);
        //	//adapter.ClearHierarchyObjects(true);
        //}
        internal void __FilterCacheClear()
        {
            ClearCache();
            __FilterCacheClear2();
        }
        void __FilterCacheClear2()
        {
            __FilterToColor_cache.Clear();
            __ComponentToColor_cache.Clear();

            __NameToCache.Clear();
            __ComponentToCache.Clear();

            __TagToCache.Clear();
            __LayerToCache.Clear();

            //Cache.ClearHierarchyObjects( false );
            ClearCache();

            //Root.p[ 0 ].modsController.highLighterMod.ClearCacheAdditional();
            //Root.p[ 0 ].RESET_DRAWSTACK( pluginID );
        }

        ComparePair FilterTempPair;
        internal struct ComparePair : IEquatable<ComparePair>, IEqualityComparer<ComparePair>
        {

            public void Set( Component[] o )
            {
                isids = true;
                comps = o;
                idhash = -1;
                compTypes = new string[ o.Length ];

                for ( int i = 0; i < o.Length; i++ )
                {
                    if ( !comps[ i ] || (comps[ i ].hideFlags & HideFlags.HideInInspector) != 0 ) continue;

                    compTypes[ i ] = o[ i ].GetType().Name;
                    idhash += compTypes[ i ].GetHashCode();
                }
            }
            public void Set( UnityEngine.Object o )
            {
                isob = true;
                isids = false;
                this.o = o;
                this.name = o.GetType().Name;
            }

            public void Set( string o )
            {
                isob = false;
                isids = false;
                this.name = o;
            }

            public string[] compTypes;
            public Component[] comps;
            public int idhash;
            public bool isids;
            public bool isob;
            public UnityEngine.Object o;
            public string name;

            public static bool operator ==( ComparePair x, ComparePair y )
            {
                return x.Equals( y );
            }
            public static bool operator !=( ComparePair x, ComparePair y )
            {
                return !x.Equals( y );
            }

            public bool Equals( ComparePair other )
            {
                if ( isob ) return o == other.o;

                if ( isids ) return idhash == other.idhash;

                return name == other.name;
            }

            public override int GetHashCode()
            {
                if ( isob ) return o.GetInstanceID();

                if ( isids ) return idhash;

                return name.GetHashCode();
            }

            public override bool Equals( object obj )
            {
                return Equals( (ComparePair)obj );
            }

            public override string ToString()
            {
                if ( isids ) throw new Exception( "ToString exception, use compTypes for Components" );

                return name;
            }

            public bool Equals( ComparePair x, ComparePair y )
            {
                return x.Equals( y );
            }

            public int GetHashCode( ComparePair obj )
            {
                return obj.GetHashCode();
            }
        }

        internal class CompareClass
        {
            internal Dictionary<ComparePair, Candidate[]> CompareToColor = new Dictionary<ComparePair, Candidate[]>();
            internal Dictionary<ComparePair, FakeFstrClass> CompareToFilter = new Dictionary<ComparePair, FakeFstrClass>();
            internal void Clear()
            {
                CompareToColor.Clear();
                CompareToFilter.Clear();
            }
        }

        internal CompareClass __NameToCache = new CompareClass();
        internal CompareClass __ComponentToCache = new CompareClass();
        internal CompareClass __TagToCache = new CompareClass();
        internal CompareClass __LayerToCache = new CompareClass();
        internal Dictionary<ColorFilter, TempColorClass> __FilterToColor_cache = new Dictionary<ColorFilter, TempColorClass>();
        internal Dictionary<int, Candidate[]> __ComponentToColor_cache = new Dictionary<int, Candidate[]>();

        // SingleList _el = new SingleList() {list = new int[9].ToList() };
        //TempColorClass GetFilterResult = new TempColorClass();
        Candidate[][] temp_GetFilterResult = new Candidate[4][];

        /*class  CandidateUnion {
		    Candidate[] candidate;*/

        public void OverrideTo( ref Candidate[] source, ref Candidate[] target, int r/*, bool OverrideIfFalse = true*/)
        {
            if ( source == null ) return;

            for ( int i = 0; i < source.Length; i++ )
            {   //for (int r = 0; r < source[i].requestArray.Length; r++)
                // && ( source[i].tempColor.HAS_BG_COLOR || source[i].tempColor.HAS_LABEL_COLOR || source[i].tempColor.add_icon )
                //	{	if (source[i] != null &&  (OverrideIfFalse && source[i].requestArray[r].HasValue || !OverrideIfFalse && source[i].requestArray[r] == true))
                {
                    if ( source[ i ] != null && source[ i ].requestArray[ r ].HasValue )
                    {   //if (target[i].requestArray[r] != true)
                        target[ i ].requestArray[ r ] = source[ i ].requestArray[ r ];

                        if ( target[ i ].requestArray[ r ] == true )
                            target[ i ].tempColor = source[ i ].tempColor;

                        // if (overrideColor) source[i].tempColor.OverrideTo(ref target[i].tempColor);
                    }
                }
            }
        }
        /*  }*/

        internal void SubscribeAutoColorHighLighter( EditorSubscriber sbs, EditorSettingsAdapter.HighlighterSettings r )
        {
            auto_highlighter_using = false;

            if ( !r.USE_AUTO_HIGHLIGHTER_MOD ) return;
            sbs.OnUndoAction += __FilterCacheClear2;
            auto_highlighter_using = true;
        }

        bool auto_highlighter_using = false;


        internal TempColorClass GetFilter( HierarchyObject o )
        {

            //if (o.name == "About")
            //{
            //	Debug.Log("ASD");
            //}

            if ( !auto_highlighter_using ) return null;
            if ( !o.Validate() ) return null;
            if ( o.ah.filterAssigned ) return o.ah.filterColor;
            o.ah.filterAssigned = true;
            if ( o.ah.filterColor == null ) o.ah.filterColor = new TempColorClass().empty;
            o.ah.filterColor.Clear();
            m_GetFilter( o, ref o.ah.filterColor );
            return o.ah.filterColor;
        }
        Candidate[] GetFilterResult;
        // Dictionary<int, MonoScript> CompToMonoScript = new Dictionary<int, MonoScript>();
        // Dictionary<int, MonoScript> CompToMonoScript = new Dictionary<int, MonoScript>();
        // MonoScript tScript;
        void m_GetFilter( HierarchyObject o, ref TempColorClass __GetFilterResult )
        {

            //	if (o.name == "About") Debug.Log(pluginID);

            var filters = HighLighterCommonData.GetColorFilters(pluginID);
            if ( filters.Count == 0 ) return;
            if ( GetFilterResult == null ) GetFilterResult = new Candidate[ filters.Count ];
            if ( GetFilterResult.Length != filters.Count ) Array.Resize( ref GetFilterResult, filters.Count );
            for ( int i = 0; i < filters.Count; i++ )
            {
                if ( GetFilterResult[ i ] == null ) GetFilterResult[ i ] = new Candidate() { requestArray = new bool?[ 4 ] };
                for ( int asd = 0; asd < filters[ i ].GetFilterByNameToCompLength; asd++ )
                {
                    if ( !filters[ i ].IsNullOrEmptyGetFilterByNameToComp( asd ) ) GetFilterResult[ i ].requestArray[ asd ] = false;
                    else GetFilterResult[ i ].requestArray[ asd ] = null;
                }
            }


            Component[] comps = o.GetComponents();
            temp_GetFilterResult[ 1 ] = null;
            int Hash = -1;
            unchecked
            {
                for ( int i = 0; i < comps.Length; i++ )
                {
                    if ( !comps[ i ] || (comps[ i ].hideFlags & HideFlags.HideInInspector) != 0 ) continue;
                    Hash += comps[ i ].GetInstanceID();
                }
            }

            if ( __ComponentToColor_cache.ContainsKey( Hash ) )
            {
                if ( __ComponentToColor_cache[ Hash ] != null )
                    temp_GetFilterResult[ 1 ] = __ComponentToColor_cache[ Hash ];
            }
            else
            {
                FilterTempPair.Set( comps );
                if ( (temp_GetFilterResult[ 1 ] = CheckFilter( FilterTempPair, filters, 1, ref __ComponentToCache )) != null )
                    __ComponentToColor_cache.Add( Hash, temp_GetFilterResult[ 1 ] );
            }

            if ( temp_GetFilterResult[ 1 ] != null )
            {
                OverrideTo( ref temp_GetFilterResult[ 1 ], ref GetFilterResult, 1 );
            }




            FilterTempPair.Set( o.name );
            if ( (temp_GetFilterResult[ 0 ] = CheckFilter( FilterTempPair, filters, 0, ref __NameToCache )) != null )
            {   // hasRes = true;
                if ( temp_GetFilterResult[ 0 ] != null ) OverrideTo( ref temp_GetFilterResult[ 0 ], ref GetFilterResult, 0 );
            }


            if ( o.go )
            {
                if ( !string.IsNullOrEmpty( o.go.tag ) )
                {
                    FilterTempPair.Set( o.go.tag != "Untagged" ? o.go.tag : "±" );
                    if ( (temp_GetFilterResult[ 2 ] = CheckFilter( FilterTempPair, filters, 2, ref __TagToCache )) != null )
                    {   //hasRes = true;
                        if ( temp_GetFilterResult[ 2 ] != null )
                        {
                            OverrideTo( ref temp_GetFilterResult[ 2 ], ref GetFilterResult, 2 );
                        }
                        // temp_GetFilterResult.OverrideTo(ref GetFilterResult);
                    }
                }
                if ( LayerMask.LayerToName( o.go.layer ) != "Default" )
                {
                    FilterTempPair.Set( LayerMask.LayerToName( o.go.layer )/*.ToString()*/ );
                    if ( (temp_GetFilterResult[ 3 ] = CheckFilter( FilterTempPair, filters, 3, ref __LayerToCache )) != null )
                    {   //hasRes = true;
                        if ( temp_GetFilterResult[ 3 ] != null ) OverrideTo( ref temp_GetFilterResult[ 3 ], ref GetFilterResult, 3 );
                        // temp_GetFilterResult.OverrideTo(ref GetFilterResult);
                    }
                }
            }


            //  hasRes = false;
            for ( int i = 0; i < GetFilterResult.Length; i++ )
            // for (int i = GetFilterResult.Length - 1; i >= 0 ; i--)
            {


                bool AllNull = true;
                bool AnyFalse = false;
                for ( int r = 0; r < GetFilterResult[ i ].requestArray.Length; r++ )
                {
                    if ( GetFilterResult[ i ].requestArray[ r ].HasValue )
                    {
                        AllNull = false;
                        var val = GetFilterResult[i].requestArray[r];

                        // if ( GetFilterResult[i].NOT[r]) val = !val;
                        if ( val.HasValue && val == false ) AnyFalse = true;
                    }
                }

                if ( AllNull || AnyFalse ) continue;
                //if (!GetFilterResult[i].targetFilter.ENABLE) continue;
                //   hasRes = true;
                GetFilterResult[ i ].tempColor.OverrideTo( ref __GetFilterResult );
                /*if (o.go.name == "Mesh Cube 3 Children")
				{   // Debug.Log(GetFilterResult[i].targetFilter.NAME);
				    Debug.Log(GetFilterResult[i].tempColor.BG_ALIGMENT_LEFT_CONVERTED);
				}*/
                // break;
            }

            // if (!hasRes) return ;

            // return __GetFilterResult;
        }

        FakeFstrClassStruckt b_value;
        string s_value;
        Candidate[] _getNamerColor;
        FakeFstrClass _fstrget;
        internal class FakeFstrClass
        {
            internal Dictionary<string, FakeFstrClassStruckt> dic = new Dictionary<string, FakeFstrClassStruckt>();
        }
        internal struct FakeFstrClassStruckt
        {
            internal bool? BOOL;
            // internal bool NOT;
        }


        internal class Candidate
        {
            // internal ColorFilter targetFilter;
            internal TempColorClass tempColor;
            internal bool?[] requestArray;
            // internal bool[] NOT;
        }
        // 0 == Name // 1 == Comps
        FakeFstrClassStruckt applyFilter;
        Candidate[] CheckFilter( ComparePair fstr, List<ColorFilter> filters, int NameOrComp, ref CompareClass __c )
        {

            if ( __c.CompareToColor.TryGetValue( fstr, out _getNamerColor ) ) return _getNamerColor;


            if ( !__c.CompareToFilter.TryGetValue( fstr, out _fstrget ) ) __c.CompareToFilter.Add( fstr, _fstrget = new FakeFstrClass() );



            Candidate[] prewcandidates = null;
            //getNamerColor = prewcandidates ?? null;

            for ( int f = 0; f < filters.Count; f++ )
            {
                if ( !filters[ f ].ENABLE ) continue;

                applyFilter.BOOL = null;

                //pplyFilter.NOT = false;
                if ( _fstrget.dic.TryGetValue(
                            NameOrComp == 0 ? filters[ f ].NameFilter :
                            NameOrComp == 1 ? filters[ f ].ComponentFilter :
                            NameOrComp == 2 ? filters[ f ].TagFilter :
                            filters[ f ].LayerFilter
                            , out b_value ) )
                {
                    applyFilter = b_value;


                }

                else
                {
                    var states =
                        NameOrComp == 0 ? filters[f].AllStatesForName :
                        NameOrComp == 1 ? filters[f].AllStatesForComps :
                        NameOrComp == 2 ? filters[f].AllStatesForTagss :
                        filters[f].AllStatesForLayerss;
                    // 0 == or // 1 == and
                    var resultStack = new KeyValuePair<int, bool>[states.Length];



                    for ( int s = 0; s < states.Length; s++ )
                    {
                        if ( string.IsNullOrEmpty( states[ s ].filter ) )
                        {
                            resultStack[ s ] = new KeyValuePair<int, bool>( -1, false );
                            continue;
                        }


                        var dest = states[s].filter.Trim();

                        if ( string.IsNullOrEmpty( dest ) )
                        {
                            resultStack[ s ] = new KeyValuePair<int, bool>( -1, false );
                            continue;
                        }

                        if ( states[ s ].IGNORECASE )
                            dest = dest.ToLower();


                        var result = false;

                        for ( int asd = 0; asd < (fstr.isids ? fstr.comps.Length : 1); asd++ )
                        {

                            if ( fstr.isids && (!fstr.comps[ asd ] || (fstr.comps[ asd ].hideFlags & HideFlags.HideInInspector) != 0) ) continue;

                            var source = fstr.isids ? fstr.compTypes[asd] : fstr.ToString();


                            if ( states[ s ].IGNORECASE )
                            {
                                source = source.ToLower();
                            }

                            switch ( states[ s ].GetCompar )
                            {
                                case ColorFilter.States.Compar.Contains: result |= source.Contains( dest ); break;

                                case ColorFilter.States.Compar.StartWith: result |= source.StartsWith( dest ); break;

                                case ColorFilter.States.Compar.EndWith: result |= source.EndsWith( dest ); break;

                                case ColorFilter.States.Compar.Equals: result |= source.Equals( dest ); break;
                            }

                        }


                        if ( states[ s ].NOT )
                        {   //if (NameOrComp != 1)
                            result = !result;
                            /* else
							     applyFilter.NOT = true;*/
                        }



                        // 0 == or // 1 == and
                        if ( s == 0 ) resultStack[ s ] = new KeyValuePair<int, bool>( 0, result );
                        else resultStack[ s ] = new KeyValuePair<int, bool>( states[ s ].AND ? 1 : 0, result );
                    }

                    var i = -1;
                    bool? currentState = null;

                    while ( ++i < resultStack.Length )
                    {
                        if ( resultStack[ i ].Key == -1 ) continue;

                        if ( resultStack[ i ].Key == 0 )
                        {
                            if ( currentState == true ) break;

                            currentState = true;
                        }

                        currentState = (currentState ?? false) & resultStack[ i ].Value;
                    }

                    applyFilter.BOOL = currentState;
                    _fstrget.dic.Add(
                        NameOrComp == 0 ? filters[ f ].NameFilter :
                        NameOrComp == 1 ? filters[ f ].ComponentFilter :
                        NameOrComp == 2 ? filters[ f ].TagFilter :
                        filters[ f ].LayerFilter
                        , applyFilter );
                }//fill condition dictionary



                if ( applyFilter.BOOL.HasValue ) //was TRUEs !!

                {   // if (!__FilterToColor_cache.TryGetValue(filters[f], out getFilterColor)) __FilterToColor_cache.Add(filters[f], getFilterColor = filters[f].AS_TEMPCOLOR_ALIGN_ONLY );


                    if ( prewcandidates == null ) prewcandidates = new Candidate[ filters.Count ];

                    if ( prewcandidates[ f ] == null )
                    {
                        prewcandidates[ f ] = new Candidate() { /*targetFilter = filters[f],*/ requestArray = new bool?[ 4 ] };

                        //if (!string.IsNullOrEmpty( filters[f].GetFilterByNameToComp(NameOrComp))) prewcandidates[f].requestArray[NameOrComp] = false;

                        /*	if (NameOrComp == 0 && !string.IsNullOrEmpty( filters[f].NameFilter)) prewcandidates[f].requestArray[0] = false;
						
							if (!string.IsNullOrEmpty( filters[f].ComponentFilter)) prewcandidates[f].requestArray[1] = false;
						
							if (!string.IsNullOrEmpty( filters[f].TagFilter)) prewcandidates[f].requestArray[2] = false;
						
							if (!string.IsNullOrEmpty( filters[f].LayerFilter)) prewcandidates[f].requestArray[3] = false;*/
                    }

                    /*if (NameOrComp == 2 || + NameOrComp == 3)
					{	Debug.Log((applyFilter.BOOL.HasValue ?  applyFilter.BOOL.ToString() : "null" ) + " " + fstr.ToString());
					
					}*/

                    TempColorClass getFilterColor;

                    if ( !__FilterToColor_cache.TryGetValue( filters[ f ], out getFilterColor ) )
                    {   // var asd = ColorFilter.__TempColorClass ;

                        var oldAl = filters[f].Aligment;
                        var oldSL = ColorFilter.__SingleList;
                        var oldTC = ColorFilter.__TempColorClass;

                        filters[ f ].Aligment = filters[ f ].Aligment.ToArray();
                        ColorFilter.__SingleList = new IntList() { list = filters[ f ].Aligment };
                        var asd = ColorFilter.__SingleList.list.Select(s=>s.ToString()).ToArray();
                        ColorFilter.__TempColorClass = new TempColorClass().AssignFromList( ref asd );
                        getFilterColor = filters[ f ].AS_TEMPCOLOR_ALIGN_ONLY;
                        __FilterToColor_cache.Add( filters[ f ], getFilterColor );

                        filters[ f ].Aligment = oldAl;
                        ColorFilter.__SingleList = oldSL;
                        ColorFilter.__TempColorClass = oldTC;
                    }

                    prewcandidates[ f ].tempColor = getFilterColor;
                    /*  if (NameOrComp == 0)
					      Debug.Log(filters[f].AllStatesForName[0].filter
					                + "\n" + filters[f].AllStatesForName[0].AND
					                + "\n" + filters[f].AllStatesForName[0].OR
					                + "\n" + filters[f].AllStatesForName[0].ENDWITH
					                + "\n" + filters[f].AllStatesForName[0].STARTWITH);*/


                    //  if (getNamerColor == null) getNamerColor = new TempColorClass().AssignFromList(new SingleList() { list = _el.list.ToList()});
                    //  getFilterColor.OverrideTo(ref getNamerColor);



                    prewcandidates[ f ].requestArray[ NameOrComp ] = applyFilter.BOOL;
                    // getNamerColor[f].NOT[NameOrComp] = applyFilter.NOT;
                }//applyFilter


            }//for filters


            __c.CompareToColor.Add( fstr, prewcandidates );



            return prewcandidates;
            //  if (!__NameToColor_cache.ContainsKey(fstr))__NameToColor_cache.Add(fstr, new Dictionary<string, bool>());

        }

    }
}
