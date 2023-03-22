//#define DISABLE_PING

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using EMX.HierarchyPlugin.Editor.Windows;



namespace EMX.HierarchyPlugin.Editor.Mods
{



	internal partial class ComponentsIcons_Mod
    {

        /*  static string TransformTypeName = typeof(Transform).Name;
        static string CanvasRendererTypeName = typeof(CanvasRenderer).Name;*/

        //  static Texture monotexturedefault = null;


        //string MenuText = "Hide 'MonoBehaviour' icon";


        // Component[] emptyArr = new Component[0];
        // GUIContent tempContent = new GUIContent();
        //Component[] tempArr = new Component[1];

        Dictionary<int, bool> IsMonoBehaviour_helper = new Dictionary<int, bool>();

        //  static Type monoType = typeof()
        bool _IsMonoBehaviour( Component comp )
        {
            if ( !comp ) return false;

            if ( !IsMonoBehaviour_helper.ContainsKey( comp.GetInstanceID() ) )
                IsMonoBehaviour_helper.Add( comp.GetInstanceID(), comp is MonoBehaviour && TODO_Tools.GetObjectBuildinIcon( comp.GetType() ).add_icon == null );

            return IsMonoBehaviour_helper[ comp.GetInstanceID() ];
        }










        Dictionary<int, Component[]> Validate_cache = new Dictionary<int, Component[]>();

        Component[] Validate( HierarchyObject o, Type fillter )
        {
            if ( !o.Validate() ) return new Component[ 0 ];

            if ( Validate_cache.ContainsKey( o.id ) ) return Validate_cache[ o.id ];

            var comps = o.GetComponents();

            Validate_cache.Add( o.id, comps );

            if ( comps.Length == 0 ) return comps;

            if ( fillter == null ) throw new Exception( "fillter cannot be null" );
            //   var tempBool = true;
            //  bool haveFilter = fillter != null;
            comps = comps.Where( c => {
                if ( !c ) return false;

                if ( (c).GetType() != fillter ) return false;
                return true;
                /*  var compInd = GetComponentType(c);
                  var value = tempBool || compInd != 1;

                  if ( compInd == 1 )
                  {
                      tempBool = false;
                  }

                  return value && compInd != -1;*/
            } ).ToArray();

            Validate_cache[ o.id ] = comps;

            if ( comps.Length == 0 ) return comps;

            //  comps = comps.OrderBy( GetComponentType ).ToArray();


            Validate_cache[ o.id ] = comps;
            return comps;


        }





        internal SearchWindow.FillterData_Inputs CallHeaderFiltered( Type fillter )
        {
            Action<SearchWindow.FillterData_Inputs> updateCache = (IN) =>
            { //  IN.Registrate_FillterData_Inputs(IN);
                Validate_cache.Clear();
                //IN.analizeEnumerator = null;
            };

            //  Debug.Log( fillter );
            var result = new SearchWindow.FillterData_Inputs(callFromExternal_objects)
            {
                UpdateCache = updateCache,
                Valudator = o => Validate(o, fillter).Length != 0,
                SelectCompareString = (b, i) =>
                { // if (!Validate_cache.ContainsKey(b.go.GetInstanceID())) Validate(b.go, fillter);
                    if (!Validate_cache.ContainsKey(b.go.GetInstanceID())) return "";

                    var Components = Validate_cache[b.go.GetInstanceID()];

                    if (Components.Length == 0) return "";

                    var res = (Math.Min(999, Components.Length) / 1000f).ToString();

                    for (int j = 0; j < Components.Length; j++)
                    {
                        var temp = (Components[j]).GetType().Name;
                        // MonoBehaviour.print(CompNameLeng[j] + " " + temp.Length + " " + b.Components[j].GetType().ToString());
                        // var add = new string(Enumerable.Repeat(' ', CompNameLeng[j] - temp.Length).ToArray());
                        res += temp /*+ add*/;
                    }

                    return res;
                },
                SelectCompareCostInt = (s, i) =>
                { //  if (!Validate_cache.ContainsKey(s.go.GetInstanceID())) Validate(s.go, fillter);
                    if (!Validate_cache.ContainsKey(s.go.GetInstanceID())) return 0;

                    var Components = Validate_cache[s.go.GetInstanceID()];

                    if (Components.Length == 0) return 0;

                    var cost = i;
                   // var compType = GetComponentType(Components[0]);
                    var A1 = GetEnable(Components[0], true) ? 0 : 500000;
                   // var A2 = 1000000;
                    var A3 = s.go.activeInHierarchy ? 0 : 100000000;
                    cost += A1 + /*compType * A2 */+ A3;
                    return cost;
                }
            };
            return result;
        }

        static Dictionary<Type, bool> have_enable_helper = new Dictionary<Type, bool>();
        static Dictionary<Type, PropertyInfo> have_enable_field = new Dictionary<Type, PropertyInfo>();

        internal static bool HasEnable( Component comp )
        {
            if ( !have_enable_helper.ContainsKey( comp.GetType() ) )
            {
                have_enable_helper.Add( comp.GetType(), true );
                var pr = comp.GetType().GetProperty("enabled", BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if ( pr == null || !pr.CanWrite || pr.PropertyType != typeof( bool ) ) have_enable_helper[ comp.GetType() ] = false;

                have_enable_field.Add( comp.GetType(), pr );
                // else have_enable_helper[comp.GetInstanceID()] = true;
            }

            return have_enable_helper[ comp.GetType() ];
        }

        internal static bool GetEnable( Component comp, bool en = false )
        { /* var pr = comp.GetType().GetProperty("enabled", BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			 if (pr == null || !pr.CanWrite || pr.PropertyType != typeof(bool)) return false;*/
            if ( !comp ) return false;
            if ( !HasEnable( comp ) ) return en;
            return (bool)have_enable_field[ comp.GetType() ].GetValue( comp, null );
        }
        static bool GetEnableFast( Component comp ) { return (bool)have_enable_field[ comp.GetType() ].GetValue( comp, null ); }

        internal static void SetEnable( Component comp, bool value )
        {
            if ( !HasEnable( comp ) ) return;

            /*   var pr = comp.GetType().GetProperty("enabled", BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
               if (pr == null || !pr.CanWrite || pr.PropertyType != typeof(bool)) return;*/
            have_enable_field[ comp.GetType() ].SetValue( comp, value, null );
        }

        /*    bool DrawDisable( Component comp )
            {
                if ( !HasEnable( comp ) ) return false;

                return !GetEnable( comp );
            }
            */

        //   Rect GetClipRect()
        void _S_( GameObject o, Component target, bool value )
        {
            ResetStack();

            if ( adapter.ha.SELECTED_GAMEOBJECTS().All( selO => selO.go != o ) )
            {
                Undo.RecordObject( target, "Enable/Disable Component" );
                SetEnable( target, value );
                adapter.SetDirty( target );
                adapter.MarkSceneDirty( target.gameObject.scene );
#if !DISABLE_PING
                if ( adapter.par_e.ENABLE_OBJECTS_PING ) Tools.TRY_PING_OBJECT( o );

#endif
            }

            else
            {
                var index = o.GetComponents(target.GetType()).ToList().IndexOf(target);

                if ( index == -1 ) return;

                foreach ( var objectToUndo in adapter.ha.SELECTED_GAMEOBJECTS() )
                {
                    var c = objectToUndo.go.GetComponents(target.GetType());

                    if ( index >= c.Length ) continue;

                    var variable = c[index];
                    // foreach (var variable in c)
                    {
                        Undo.RecordObject( variable, "Enable/Disable Component" );
                        SetEnable( variable, value );
                        adapter.SetDirty( variable );
                        adapter.MarkSceneDirty( variable.gameObject.scene );
                    }

                    //  if (Hierarchy.par.ENABLE_PING_Fix) adapter.TRY_PING_OBJECT(objectToUndo);
                }
            }
        }



        Rect? stateForDrag_B0;
        GameObject stateForDrag_B1;
        Component[] stateForDrag_B2;


        Component[] RawOnUpDragComponents_Array;
        HideFlags RawOnUpDragComponents_Flags = HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy | HideFlags.HideInInspector;

        bool RawOnUpDragComponents( Events.MouseRawUp.WantMouseLeaveType t )
        {
            if ( RawOnUpDragComponents_Array != null )

            {
                bool remove = true;

                if ( RawOnUpDragComponents_Array.Length == DragAndDrop.objectReferences.Length )
                {
                    var drawlist = DragAndDrop.objectReferences.Select(c => (c ? c.GetInstanceID() : -1)).ToList();
                    bool all = true;

                    foreach ( var item in RawOnUpDragComponents_Array )
                    {
                        if ( !item ) continue;

                        if ( drawlist.Contains( item.GetInstanceID() ) ) continue;

                        all = false;
                        break;
                    }

                    if ( all ) remove = false;
                }

                if ( remove /* || DragAndDrop.GetGenericData("MoveComp") != RawOnUpDragComponents_Array*/)
                {
                    RawOnRemoveRaw();
                    RawOnUpDragComponents_Array = null;
                }
            }
            return t != Events.MouseRawUp.WantMouseLeaveType.DoesNotClear;
        }

        void RawOnRemoveRaw()
        {
            if ( RawOnUpDragComponents_Array == null ) return;

            foreach ( var item in RawOnUpDragComponents_Array )
            {
                if ( item && item.gameObject )
                {
                    if ( (item.gameObject.hideFlags & RawOnUpDragComponents_Flags) != 0 )
                    {
                        if ( Application.isPlaying ) GameObject.Destroy( item.gameObject );
                        else GameObject.DestroyImmediate( item.gameObject );
                    }
                }
            }
        }

        bool RawOnUP( Events.MouseRawUp.WantMouseLeaveType t )
        {
            if ( t == Events.MouseRawUp.WantMouseLeaveType.DragOut ) return false;

            stateForDrag_B0 = null;
            stateForDrag_B1 = null;
            return true;
        }


        static int SHORT = 1;
        static SearchWindow lastFocusRoot;
        // private Hierarchy_GUI.CustomIconParams load;
    }
}










// Type type;
//Type types;

// Dictionary<Type, int> GetComponentType_helper = new Dictionary<Type, int>();

/*   int GetComponentType( Component comp )
   {
       if ( !comp ) return -1;



       type = Adapter.GetType_( comp );
       types = type;

       if ( GetComponentType_helper.ContainsKey( types ) ) return GetComponentType_helper[ types ];

       if ( comp is Transform || comp is CanvasRenderer )
       // if (types == TransformTypeName  || types == CanvasRendererTypeName)
       {
           GetComponentType_helper.Add( types, -1 );
           return -1;
       }

       var result = -1;

       if ( _IsMonoBehaviour( comp ) )
       {
           if ( HaveUserIcon( comp ) && Hierarchy.HierarchyAdapterInstance.par.FD_Icons_user ) result = 0;
           else if ( Hierarchy.HierarchyAdapterInstance.par.FD_Icons_mono ) result = 1;
           else
           {
               GetComponentType_helper.Add( types, -1 );
               return -1;
           }
       }

       if ( !Hierarchy.HierarchyAdapterInstance.par.FD_Icons_default || Hierarchy_GUI.Instance( adapter ).HiddenComponents.Contains( type.FullName ) )
       {
           GetComponentType_helper.Add( types, -1 );
           return -1;
       }

       if ( result == -1 ) result = 2;

       if ( typeFillter != null ) //  MonoBehaviour.print(typeFillter);
       {
           if ( typeFillter == type )
           {
               GetComponentType_helper.Add( types, 0 );
               return 0;
           }

           GetComponentType_helper.Add( types, result + 1 );
           return result + 1;
       }

       GetComponentType_helper.Add( types, result );
       return result;
   }*/

/*      void MonDrawer( HierarchyObject _o, Component[] cc, Type callbackType, bool allowHide )
        {
            if ( cc == null || cc.Length == 0 ) return;


            // if (!DRAW_NEXTTONAME) drawRect.x -= drawRect.width + Hierarchy.HierarchyAdapterInstance.par.ICONSPACE;


            Texture image = null;



            callbackType = cc.Length == 1 ? cc[ 0 ].GetType() : null;
           // color = Color.white;

            if ( adapter.par_e.COMPONENTS_MONO_SPLIT_MODE == 2 )
            {
                var getted = TODO.GetObjectBuildinIcon(adapter, cc[0], type);
                image = getted.add_icon ? getted.add_icon : null;

                if ( image )
                {
                    if ( !id_to_mono_dic.ContainsKey( image.GetInstanceID() ) )
                        id_to_mono_dic.Add( image.GetInstanceID(), string_to_mono_dic.ContainsKey( image.name ) );

                    if ( id_to_mono_dic[ image.GetInstanceID() ] ) image = null;
                }
            }

          //  drawName = !image;

            if ( !image ) image = adapter.par_e.COMPONENTS_MONO_SPLIT_MODE == 2 && callbackType != null ? adapter.GetIcon( "MONOCLEAN" ) : adapter.GetIcon( "MONO" );


            DrawIcon( cc, _o, drawRect, (Texture2D)image, callbackType, allowHide, ref MenuText );

            // if (DRAW_NEXTTONAME) 
            drawRect.x += drawRect.width + adapter.par_e.COMPONENTS_ICONS_SPACE - 0.65f;

            bool wasDraw = false;

            for ( int i = 0 ; i < cc.Length ; i++ ) //  if (!wasDraw && !DRAW_NEXTTONAME) drawRect.x -= Hierarchy.par.ICONSPACE;
            {
                if ( DrawAttributes( cc[ i ].GetType(), cc[ i ] ) ) wasDraw = true;
            }

            if ( wasDraw )
            {
                // if ( DRAW_NEXTTONAME )
                drawRect.x += adapter.par_e.COMPONENTS_ICONS_SPACE;
            }
        }*/
