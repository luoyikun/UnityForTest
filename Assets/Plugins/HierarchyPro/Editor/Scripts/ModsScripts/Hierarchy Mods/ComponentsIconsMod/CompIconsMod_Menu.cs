#define DISABLE_PING

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;



namespace EMX.HierarchyPlugin.Editor.Mods
{



	internal partial class ComponentsIcons_Mod : DrawStackAdapter, ISearchable
	{

		internal struct ARGS
		{
			internal Component drawCompSingle;
			internal DrawCompsStack[] drawCompsArr;
			// internal string MenuText;
			internal bool allowHide;
			internal Type callbackType;
		}

		class ComponentMenuItem
		{
			internal string localKey;
			internal string localName;
			internal Component c;
			//internal bool enabled;
		}


		MethodInfo[] methods_static_current = null;
		MethodInfo[] methods_static_base = null;
		MethodInfo[] methods_instance_current = null;
		MethodInfo[] methods_instance_base = null;
		static Type type_skip1 = typeof(object);
		static Type type_skip2 = typeof(Component);
		static Type type_skip3 = typeof(UnityEngine.Object);

		void GET_METHODS( Component component )
		{
			var current_methods = component.GetType().GetMethods(~(BindingFlags.GetField | BindingFlags.GetProperty)).Where(m => !m.IsSpecialName);
			List<MethodInfo> root_methods = new List<MethodInfo>();
			var t = component.GetType().BaseType;
			if ( adapter.par_e.COMPONENTS_MENU_INCLUDEBASECLASES )
				while ( !type_skip3.IsAssignableFrom( t ) && !type_skip2.IsAssignableFrom( t ) && !type_skip1.IsAssignableFrom( t ) )
				{
					root_methods.AddRange( t.GetMethods( ~(BindingFlags.GetField | BindingFlags.GetProperty) ).Where( m => !m.IsSpecialName ) );
					t = t.BaseType;
				}
			methods_static_current = current_methods.Where( m => m.IsStatic ).ToArray();
			methods_static_base = root_methods.Where( m => m.IsStatic ).ToArray();
			methods_instance_current = current_methods.Where( m => !m.IsStatic ).ToArray();
			methods_instance_base = root_methods.Where( m => !m.IsStatic ).ToArray();
			if ( !adapter.par_e.COMPONENTS_MENU_INCLUDESTATICFIELDS )
			{
				methods_static_current = new MethodInfo[ 0 ];
				methods_static_base = new MethodInfo[ 0 ];
			}
		}

		FieldInfo[] fields_static_current = null;
		FieldInfo[] fields_static_base = null;
		FieldInfo[] fields_instance_current = null;
		FieldInfo[] fields_instance_base = null;
		void GET_FIELDS( Component component )
		{
			var current_fields = component.GetType().GetFields(~(BindingFlags.InvokeMethod));
			List<FieldInfo> root_fields = new List<FieldInfo>();
			var t = component.GetType().BaseType;
			if ( adapter.par_e.COMPONENTS_MENU_INCLUDEBASECLASES )
				while ( !type_skip3.IsAssignableFrom( t ) && !type_skip2.IsAssignableFrom( t ) && !type_skip1.IsAssignableFrom( t ) )
				{
					root_fields.AddRange( t.GetFields( ~(BindingFlags.InvokeMethod) ) );
					t = t.BaseType;
				}
			fields_static_current = current_fields.Where( m => m.IsStatic ).ToArray();
			fields_static_base = root_fields.Where( m => m.IsStatic ).ToArray();
			fields_instance_current = current_fields.Where( m => !m.IsStatic ).ToArray();
			fields_instance_base = root_fields.Where( m => !m.IsStatic ).ToArray();
			if ( !adapter.par_e.COMPONENTS_MENU_INCLUDESTATICFIELDS )
			{
				fields_static_current = new FieldInfo[ 0 ];
				fields_static_base = new FieldInfo[ 0 ];
			}
		}
		PropertyInfo[] props_static_current = null;
		PropertyInfo[] props_static_base = null;
		PropertyInfo[] props_instance_current = null;
		PropertyInfo[] props_instance_base = null;
		void GET_PROPS( Component component )
		{
			var current_props = component.GetType().GetProperties(~(BindingFlags.InvokeMethod));
			List<PropertyInfo> root_props = new List<PropertyInfo>();
			var t = component.GetType().BaseType;
			if ( adapter.par_e.COMPONENTS_MENU_INCLUDEBASECLASES )
				while ( !type_skip3.IsAssignableFrom( t ) && !type_skip2.IsAssignableFrom( t ) && !type_skip1.IsAssignableFrom( t ) )
				{
					root_props.AddRange( t.GetProperties( ~(BindingFlags.InvokeMethod) ) );
					t = t.BaseType;
				}
			props_static_current = current_props.Where( m => m.GetGetMethod( true ).IsStatic ).ToArray();
			props_static_base = root_props.Where( m => m.GetGetMethod( true ).IsStatic ).ToArray();
			props_instance_current = current_props.Where( m => !m.GetGetMethod( true ).IsStatic ).ToArray();
			props_instance_base = root_props.Where( m => !m.GetGetMethod( true ).IsStatic ).ToArray();
			if ( !adapter.par_e.COMPONENTS_MENU_INCLUDESTATICFIELDS )
			{
				props_static_current = new PropertyInfo[ 0 ];
				props_static_base = new PropertyInfo[ 0 ];
			}
		}

		DrawStackMethodsWrapper __BUTTON_ACTION_HASH = null;

		DrawStackMethodsWrapper BUTTON_ACTION_HASH {
			get { return __BUTTON_ACTION_HASH ?? (__BUTTON_ACTION_HASH = new DrawStackMethodsWrapper( BUTTON_ACTION )); }
		}
		GUIContent temp_content = new GUIContent();
		void BUTTON_ACTION( Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o )
		{
			var o = _o.go;


			if ( inputRect.Contains( EVENT.mousePosition ) )
			{
				var arr = (ARGS)data.args;
				var drawComps = arr.drawCompsArr != null ? arr.drawCompsArr.Where(c => c.comp).Select(c => c.comp).ToList() : new List<Component>();
				if ( drawComps.Count == 0 && arr.drawCompSingle ) drawComps.Add( arr.drawCompSingle );

				if ( drawComps.Count > 1 ) temp_content.tooltip = drawComps.Select( c => c.GetType().Name ).Aggregate( ( a, b ) => a + '\n' + b );
				else temp_content.tooltip = drawComps[ 0 ].GetType().Name;
				GUI.Label( inputRect, temp_content );
			}


			if ( EVENT.button == adapter.MOUSE_BUTTON_0 )
			{
				var arr = (ARGS)data.args;
				var drawComps = arr.drawCompsArr != null ? arr.drawCompsArr.Where(c => c.comp).Select(c => c.comp).ToList() : new List<Component>();
				if ( drawComps.Count == 0 && arr.drawCompSingle ) drawComps.Add( arr.drawCompSingle );
				//  var drawComps = get_drawComps(_o.id);
				// var MenuText = arr.MenuText;
				var allowHide = arr.allowHide;
				var callbackType = arr.callbackType;
				// Debug.Log( drawComps[0] );


				var components = new List<Component>();

				/* if (GUID != null) components.Add(readcomps.First(asd => ComponentToGUID(asd) == GUID));
                 else*/
				{
					foreach ( var component in drawComps )
					{
						components.Add( component );
					}
				}

				if ( EVENT.control )
				{
					bool? val = null;

					foreach ( var component in components )
					{
						if ( !component ) continue;

						var target = component;

						if ( HasEnable( target ) )
						{
							if ( val == null ) val = !GetEnable( target );

							_S_( o, component, val.Value );
						}


						/* var target = component;
                         if (HaveEnable(target))
                         {
                             Undo.RecordObject(target, "Enable/Disable Component");
                             SetEnable(target, val.Value);
                             Hierarchy.SetDirty(target);
                         }*/
					}
				}

				else // MonoBehaviour.print(components.Count);
				{
					var menu = new GenericMenu();
					var types = components.Select(c => c.GetType());
					var dic = types.Distinct().ToDictionary(t => t, t => 0);

					foreach ( var component in components )
					{
						var name = components.Count == 1 ? "" : component.GetType().Name;

						if ( components.Count > SHORT ) name = "Enabled/" + name;
						else name = "Enabled " + name;

						if ( HasEnable( component ) )
						{
							var target = component;
							var type = component.GetType();
							string postfix = null;

							if ( types.Count( t => t == type ) > 1 ) postfix = " [" + (dic[ type ]++) + "]";

							menu.AddItem( new GUIContent( /*"Enabled '"*/ name + /*"'" + type.Name + "'"  +*/ (postfix ?? "") + " %click" ), GetEnable( component ), () => { _S_( o, target, !GetEnable( target ) ); } );
						}

						else
						{
							menu.AddDisabledItem( new GUIContent( /*"Enabled '"*/name + "'" + component.GetType().Name + "'" ) );
						}
					}

					menu.AddDisabledItem( new GUIContent( /*"Enabled '"*/ "Ctrl+DRAG to move or Ctrl+Shift+DRAG to copy" ) ); //Drag %drag"
					menu.AddSeparator( "" );

					AudioSource aus = null;

					foreach ( var component in components )
						if ( component is AudioSource )
							aus = component as AudioSource;

					if ( aus )
					{
						if ( !aus.clip )
							menu.AddDisabledItem( new GUIContent( "Play AudioSource" ) );
						else
							menu.AddItem( new GUIContent( aus.isPlaying ? "Stop AudioSource" : "Play AudioSource" ),
								false, () => { Mod_Audio.PlayAudio( aus ); } );

						menu.AddSeparator( "" );
					}


					foreach ( var component in components )
					{
						var target = component;
						var name = "'" + component.GetType().Name + "'";

						if ( components.Count > SHORT ) name = "Filter Selection By Component/" + name;
						else name = "Filter Selection By " + name;

						if ( adapter.ha.SELECTED_GAMEOBJECTS().Any( s => s.go == target.gameObject ) )
						{
							menu.AddItem( new GUIContent( name ), false, () => { Selection.objects = adapter.ha.SELECTED_GAMEOBJECTS().Where( s => s.go.GetComponent( target.GetType() ) ).Select( g => g.go ).ToArray(); } );
						}

						else
						{
							menu.AddDisabledItem( new GUIContent( name ) );
						}
					}


					menu.AddSeparator( "" );
					//DRAW IN HIER

					bool sumAdd = false;
					bool wasAdd = false;


					Dictionary<string,List<ComponentMenuItem>> __cache = new Dictionary<string, List<ComponentMenuItem>>();
					foreach ( var component in components )
					{
						var target = component;
						//var name = "'" + component.GetType().Name + "'";
						var name = "[ " + component.GetType().Name + " ]";
						//else name = "Copy Component " + name;
						if ( !__cache.ContainsKey( name.ToLower() ) ) __cache.Add( name.ToLower(), new List<ComponentMenuItem>() );
						__cache[ name.ToLower() ].Add( new ComponentMenuItem() { localName = name, c = target } );
					}
					Dictionary<string,bool> _tempCompare = new Dictionary<string, bool>();
					foreach ( var item in __cache )
					{
						var key = item.Key;
						int ind = 1;
						int addind = 1;
						foreach ( var t in item.Value )
						{
							var component = t.c;
							var localName = t.localName;
							var localKey = key;
							var targetLocalName = localName;
							var targetLocalKey = localKey;
							if ( item.Value.Count > 1 )
							{
								targetLocalName = localName + ' ' + '(' + ind.ToString() + ')';
								targetLocalKey = localKey + ' ' + '(' + ind.ToString() + ')';
								ind++;
							}
							localName = targetLocalName;
							localKey = targetLocalKey;
							while ( _tempCompare.ContainsKey( targetLocalKey ) )
							{
								targetLocalName = localName + ' ' + '(' + addind.ToString() + ')';
								targetLocalKey = localKey + ' ' + '(' + addind.ToString() + ')';
								addind++;
							}
							localKey = targetLocalKey;
							localName = targetLocalName;

							if ( !GetEnable( component, true ) ) localName += " (disabled)";

							var res = t;
							res.localKey = localKey;
							res.localName = localName;
							_tempCompare.Add( targetLocalKey, false );
						}
					}
					Action<string> drawToggled = (cat) => {
                        //menu.AddSeparator( (  cat  ) );
                        //menu.AddItem( new GUIContent( cat  + "Display only MonoScripts" ), adapter.par_e.COMPONENTS_MENU_INCLUDEONLYMONOSCRIPTS, () => {
                        //    adapter.par_e.COMPONENTS_MENU_INCLUDEONLYMONOSCRIPTS = !adapter.par_e.COMPONENTS_MENU_INCLUDEONLYMONOSCRIPTS;
                        //} );
                        //menu.AddItem( new GUIContent( cat  + "Scan static methods" ), adapter.par_e.COMPONENTS_MENU_INCLUDESTATICFIELDS, () => {
                        //    adapter.par_e.COMPONENTS_MENU_INCLUDESTATICFIELDS = !adapter.par_e.COMPONENTS_MENU_INCLUDESTATICFIELDS;
                        //} );
                        //menu.AddItem( new GUIContent( cat  + "Scan methods from base classes" ), adapter.par_e.COMPONENTS_MENU_INCLUDEBASECLASES, () => {
                        //    adapter.par_e.COMPONENTS_MENU_INCLUDEBASECLASES = !adapter.par_e.COMPONENTS_MENU_INCLUDEBASECLASES;
                        //} );
                    };
					Func<string,string> dec = (s1) => "[ " + s1 +  " ]";
					// foreach ( var component in components )
					foreach ( var item in __cache )
					{
						foreach ( var val in item.Value )
						{
							var c  = val.c;
							if ( adapter.par_e.COMPONENTS_MENU_INCLUDEONLYMONOSCRIPTS && !(c is MonoBehaviour) ) continue;
							if ( c is Transform || c is CanvasRenderer ) continue;
							var comp = c;

							// var methods = component.GetType().GetMethods(~(BindingFlags.GetField | BindingFlags.GetProperty));

							var category = "Invoke Methods" + (components.Count > 1 ? ("/"+val.localName) : "") + "/" ; /* at '" + component.GetType().Name + "'*/
							GET_METHODS( c );
							Action<string, MethodInfo[]> addMethod = (cat, m)=>
							{
								wasAdd = true;
								if (cat!= null) menu.AddDisabledItem( new GUIContent( category + dec(cat ) ) );
								foreach ( var methodInfo in m )
								{
									var capt = methodInfo;
									wasAdd = true;
									menu.AddItem( new GUIContent( category  + methodInfo.Name ), false, () => {
										adapter.PUSH_UPDATE_ONESHOT( 0, () => {
											if ( !comp ) return;
											var pars = capt.GetParameters().Select(p => {
												if (!p.ParameterType.IsClass) return Activator.CreateInstance(p.ParameterType);
												return null;
											}).ToArray();
											var result = capt.IsStatic ? capt.Invoke(null, pars) : capt.Invoke(comp, pars);
											if ( capt.ReturnType != typeof( void ) ) Debug.Log( "'" + capt.Name + "' returned: " + (result == null ? "null" : result.ToString()) + " (" + capt.ReturnType.Name + ")" );
										} );
									} );
								}
							};
							if ( methods_instance_current.Length != 0 ) addMethod( "Methods", methods_instance_current );
							if ( methods_instance_base.Length != 0 ) addMethod( "Base Class", methods_instance_base );
							if ( methods_instance_current.Length != 0 || methods_instance_base.Length != 0 )
								if ( methods_static_current.Length != 0 || methods_static_base.Length != 0 )
									menu.AddSeparator( category );
							if ( methods_static_current.Length != 0 ) addMethod( "Static Methods", methods_static_current );
							if ( methods_static_base.Length != 0 ) addMethod( methods_static_base.Length == 0 ? "Static Methods" : null, methods_static_base );
						}
					}
					if ( wasAdd )
					{
						drawToggled( "Invoke Methods/" );
					}
					// Selection.objects = Selection.gameObjects.Where(s => s.GetComponent(target.GetType())).ToArray();

					/*  NeedApplyMod = true;
                      Mody += () => {
                          GUI_ONESHOT = true;
                          GUI_ONESHOTAC += () => {

                              // EditorUtility.ResetMouseDown();
                              /* GUIUtility.keyboardControl = 0;
                              #1#
                              //  MonoBehaviour.print(Event.PopEvent(new Event() { type = EventType.MouseDown }));
                              var reflectorMenu = new GenericMenu();
                              reflectorMenu.AddItem(new GUIContent("ASD"), false, () => { });
                              reflectorMenu.ShowAsContext();
                              InternalEditorUtility.RepaintAllViews();
                              EditorGUIUtility.CommandEvent("Redraw");
                          };
                      };*/


					//   EditorUtility.DisplayCustomMenu(
					//  RepaintWindowInUpdate());
					//
					/* menu.AddItem(new GUIContent(tc.text + "/asd"), false, () => {

                     });*/
					/*   var target = component;
                       if (Selection.gameObjects.Contains(target.gameObject))
                       {


                       } else
                       {
                           menu.AddDisabledItem(new GUIContent("Invoke Method at '" + component.GetType().Name + "'"));

                       }*/

					sumAdd |= wasAdd;
					wasAdd = false;

					foreach ( var item in __cache )
					{
						foreach ( var val in item.Value )
						{
							var c  = val.c;
							if ( adapter.par_e.COMPONENTS_MENU_INCLUDEONLYMONOSCRIPTS && !(c is MonoBehaviour) ) continue;
							if ( c is Transform || c is CanvasRenderer ) continue;
							var comp = c;

							// var methods = component.GetType().GetMethods(~(BindingFlags.GetField | BindingFlags.GetProperty));

							var category = "Log Fields" + (components.Count > 1 ? ("/"+val.localName) : "") + "/" ; /* at '" + component.GetType().Name + "'*/
							GET_FIELDS( c );
							Action<string, FieldInfo[]> addField = (cat, m)=>
							{
								wasAdd = true;
								if (cat!= null) menu.AddDisabledItem( new GUIContent(category  +dec(cat ) ) );
								foreach ( var methodInfo in m )
								{
									var capt = methodInfo;
									wasAdd = true;
									menu.AddItem( new GUIContent( category  + methodInfo.Name ), false, () => {
										adapter.PUSH_UPDATE_ONESHOT( 0, () => {
											if ( !comp ) return;
											var result = capt.IsStatic ? capt.GetValue(null) : capt.GetValue(comp);
											Debug.Log( /*"'" + capt.Name + "' returned: " +*/ (result == null ? "null" : result.ToString()) );
										} );
									} );
								}
							};
							if ( fields_instance_current.Length != 0 ) addField( "Fields", fields_instance_current );
							if ( fields_instance_base.Length != 0 ) addField( "Base Class", fields_instance_base );
							if ( fields_instance_current.Length != 0 || fields_instance_base.Length != 0 )
								if ( fields_static_current.Length != 0 || fields_static_base.Length != 0 )
									menu.AddSeparator( category );
							if ( fields_static_current.Length != 0 ) addField( "Static Fields", fields_static_current );
							if ( fields_static_base.Length != 0 ) addField( fields_static_base.Length == 0 ? "Static Fields" : null, fields_static_base );
						}
					}
					if ( wasAdd )
					{
						drawToggled( "Log Fields/" );
					}

					sumAdd |= wasAdd;
					wasAdd = false;

					foreach ( var item in __cache )
					{
						foreach ( var val in item.Value )
						{
							var c  = val.c;
							if ( adapter.par_e.COMPONENTS_MENU_INCLUDEONLYMONOSCRIPTS && !(c is MonoBehaviour) ) continue;
							if ( c is Transform || c is CanvasRenderer ) continue;
							var comp = c;

							// var methods = component.GetType().GetMethods(~(BindingFlags.GetField | BindingFlags.GetProperty));

							var category = "Log Properties" + (components.Count > 1 ? ("/"+val.localName) : "") + "/" ; /* at '" + component.GetType().Name + "'*/
							GET_PROPS( c );
							Action<string, PropertyInfo[]> addProperties = (cat, m)=>
							{
								wasAdd = true;
								if (cat!= null) menu.AddDisabledItem( new GUIContent(category  +dec(cat ) ) );
								foreach ( var methodInfo in m )
								{
									var capt = methodInfo;
									wasAdd = true;
									menu.AddItem( new GUIContent( category  + methodInfo.Name ), false, () => {
										adapter.PUSH_UPDATE_ONESHOT( 0, () => {
											if ( !comp ) return;
											var result = capt.GetGetMethod(true).IsStatic ? capt.GetValue(null, null) : capt.GetValue(comp, null);
											Debug.Log( /*"'" + capt.Name + "' returned: " +*/ (result == null ? "null" : result.ToString()) );
										} );
									} );
								}
							};
							if ( props_instance_current.Length != 0 ) addProperties( "Properties", props_instance_current );
							if ( props_instance_base.Length != 0 ) addProperties( "Base Class", props_instance_base );
							if ( props_instance_current.Length != 0 || props_instance_base.Length != 0 )
								if ( props_static_current.Length != 0 || props_static_base.Length != 0 )
									menu.AddSeparator( category );
							if ( props_static_current.Length != 0 ) addProperties( "Static Properties", props_static_current );
							if ( props_static_base.Length != 0 ) addProperties( props_static_base.Length == 0 ? "Static Properties" : null, props_static_base );
						}
					}
					if ( wasAdd )
					{
						drawToggled( "Log Properties/" );
					}

					sumAdd |= wasAdd;
					//DRAW IN HIER


					if ( sumAdd ) menu.AddSeparator( "" );
					//COPY PASTE


					var hasSeparator = components.Count > SHORT;

					var sel = adapter.ha.SELECTED_GAMEOBJECTS();
					var clickedToSelected = sel.Any( selO => selO.go == o );
					//hasSeparator |= clickedToSelected && sel.Length > 1;




					foreach ( var item in __cache )
					{
						foreach ( var val in item.Value )
						{
							var name = val.localName;
							if ( hasSeparator ) name = "Copy Component/" + name;
							else name = "Copy Component " + name;
							var target = val.c;
							menu.AddItem( new GUIContent( name ), false, () => {
								Tools.Copy( target );
							} );
						}
					}

					// if ( t.enabled )
					//     menu.AddItem( new GUIContent( localName ), false, () => // EditorUtility.CopySerialized
					//     {
					//         ac( component );
					//     } );
					// else
					//     menu.AddDisabledItem( new GUIContent( localName ) );

					//
					//





					// fill( ( target ) => Tools.Copy( target ) );
					{
						var hasPaste = false;
						int _hasMultyComponents = 0;
#pragma warning disable
						bool hasMultyComponents = false;
#pragma warning restore
						foreach ( var item in __cache )
						{
							_hasMultyComponents = 0;
							foreach ( var val in item.Value )
							{
								if ( !Tools.PastValidate( val.c ) ) continue;
								hasPaste = true;
								var target = val.c;
								var name = val.localName;
								if ( hasSeparator || clickedToSelected && sel.Length > 1 ) name = "Paste Component Values/" + name;
								else name = "Paste Component Values " + name;

								_hasMultyComponents++;
								//var able = UnityEditorInternal.ComponentUtility.PasteComponentValues(target);
								// Undo.PerformUndo();
								menu.AddItem( new GUIContent( name ), false, () => {
									Tools.Paste( target );
								} );
								// add( name, target, Tools.PastValidate( target ) );
							}
							if ( _hasMultyComponents > 1 ) hasMultyComponents = true;
						}
						if ( hasMultyComponents || clickedToSelected && sel.Length > 1 )
						{

							Dictionary<Type,List<Component>> __dic= new Dictionary<Type, List<Component>>();
							if ( clickedToSelected && sel.Length > 1 )
							{
								foreach ( var s in sel )
									foreach ( var c in s.GetComponents() )
										if ( GetEnable( c, true ) && Tools.PastValidate( c ) )
										{
											if ( !__dic.ContainsKey( c.GetType() ) ) __dic.Add( c.GetType(), new List<Component>() );
											__dic[ c.GetType() ].Add( c );
										}
							}
							else
							{
								foreach ( var item in __cache )
									foreach ( var val in item.Value )
									{
										var target = val.c;
										if ( !GetEnable( target, true ) || !Tools.PastValidate( target ) ) continue;
										if ( !__dic.ContainsKey( target.GetType() ) ) __dic.Add( target.GetType(), new List<Component>() );
										__dic[ target.GetType() ].Add( target );
									}
							}


							if ( __dic.Count != 0 )
							{
								menu.AddSeparator( "Paste Component Values/" );
								foreach ( var item in __dic )
								{
									var n = item.Key.Name;
									var l =item.Value;
									hasPaste = true;
									menu.AddItem( new GUIContent( "Paste Component Values/Paste '" + n + "' for all enabled components and selected gameobjects" ), false, () => {
										foreach ( var s in l )
											if ( Tools.PastValidate( s ) ) Tools.Paste( s );
									} );
								}
							}
						}

						if ( !hasPaste )
						{
							menu.AddDisabledItem( new GUIContent( "Paste Component Values" ) );
						}
					}






					foreach ( var item in __cache )
					{
						foreach ( var val in item.Value )
						{
							var target = val.c;
							//var name = "" ;// val.localName;
							// if ( hasSeparator || clickedToSelected && sel.Length > 1 ) name = "Paste Component as New/" ;
							// else name = "Paste Component as New " ;
							var asdasd = hasSeparator || clickedToSelected && sel.Length > 1;
							var go = components.FirstOrDefault( c => c && c.gameObject ).gameObject;
							//if ( !go || !Tools.PastValidate( null as GameObject ) )
							//{
							//    menu.AddDisabledItem( new GUIContent( "Paste Component as New" ) );
							//}
							//else
							{
								if ( asdasd )
								{
									menu.AddItem( new GUIContent( "Paste Component as New/Paste for current gameobject" ), false, () => {
										if ( !Tools.PasteComponentAsNew( go ) )
											UnityEditor.EditorUtility.DisplayDialog( "Paste Component as New", "You didn't copy any component", "Ok" );
										ResetStack();
									} );
									menu.AddItem( new GUIContent( "Paste Component as New/Paste for all selected gameobjects" ), false, () => {
										foreach ( var s in sel ) if ( s.Validate() )
												if ( !Tools.PasteComponentAsNew( s.go ) )
												{
													UnityEditor.EditorUtility.DisplayDialog( "Paste Component as New", "You didn't copy any component", "Ok" );
													break;
												}
										ResetStack();
									} );
								}
								else
								{
									menu.AddItem( new GUIContent( "Paste Component as New/Paste for current gameobject" ), false, () => {
										if ( !Tools.PasteComponentAsNew( go ) )
											UnityEditor.EditorUtility.DisplayDialog( "Paste Component as New", "You didn't copy any component", "Ok" );
										ResetStack();
									} );
								}

							}
						}
					}



					menu.AddSeparator( "" );
					/*
                    dic = types.Distinct().ToDictionary( t => t, t => 0 );


                    foreach ( var component in components )
                    {
                        var target = component;
                        var type = component.GetType();
                        string postfix = null;

                        if ( types.Count( t => t == type ) > 1 ) postfix = " [" + (dic[ type ]++) + "]";

                        var name = "'" + component.GetType().Name + "'" + (postfix ?? "");

                        if ( components.Count > SHORT ) name = "Remove Component/" + name;
                        else name = "Remove";

                        // else name = "Remove " + name;

                        menu.AddItem( new GUIContent( name ), false, () => { 

                            if ( adapter.ha.SELECTED_GAMEOBJECTS().All( selO => selO.go != o ) )
                            {
                                var oaa = target.gameObject;
                                Undo.DestroyObjectImmediate( target );
                                // Adapter.SetDirty( oaa );
                                adapter.MarkSceneDirty( oaa.scene );
#if !DISABLE_PING
								if ( Hierarchy.par.ENABLE_PING_Fix ) adapter.TRY_PING_OBJECT( o );

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
                                   
                                    {
                                        var oaa = variable.gameObject;
                                        Undo.DestroyObjectImmediate( variable );
                                        // Adapter.SetDirty( oaa );
                                        adapter.MarkSceneDirty( oaa.scene );
                                    }

                                    //  if (Hierarchy.par.ENABLE_PING_Fix) adapter.TRY_PING_OBJECT(objectToUndo);
                                }
                            }

                            ResetStack();
                        } );
                    }

            */


					{
						var hasRemove = false;
						int _hasMultyComponents = 0;
#pragma warning disable
						bool hasMultyComponents = false;
#pragma warning restore
						foreach ( var item in __cache )
						{
							_hasMultyComponents = 0;
							foreach ( var val in item.Value )
							{
								//if ( !Tools.PastValidate( val.c ) ) continue;
								hasRemove = true;
								var target = val.c;
								var name = val.localName;
								if ( hasSeparator || clickedToSelected && sel.Length > 1 ) name = "Remove Component/Remove " + name;
								else name = "Remove Component " + name;

								_hasMultyComponents++;
								//var able = UnityEditorInternal.ComponentUtility.PasteComponentValues(target);
								// Undo.PerformUndo();
								menu.AddItem( new GUIContent( name ), false, () => {
									var oaa = target.gameObject;
									Undo.DestroyObjectImmediate( target );
									adapter.MarkSceneDirty( oaa.scene );
								} );
								// add( name, target, Tools.PastValidate( target ) );
							}
							if ( _hasMultyComponents > 1 ) hasMultyComponents = true;
						}
						if ( hasMultyComponents || clickedToSelected && sel.Length > 1 )
						{
							Dictionary<Type,List<Component>> __dic= new Dictionary<Type, List<Component>>();
							if ( clickedToSelected && sel.Length > 1 )
							{
								foreach ( var s in sel )
									foreach ( var c in s.GetComponents() )
									//if ( GetEnable( c, true ) && Tools.PastValidate( c ) )
									{
										if ( c is Transform || c is RectTransform || c is CanvasRenderer || tr_t.IsAssignableFrom( c.GetType() ) ) continue;
										if ( !__dic.ContainsKey( c.GetType() ) ) __dic.Add( c.GetType(), new List<Component>() );
										__dic[ c.GetType() ].Add( c );
									}
							}
							else
							{
								foreach ( var item in __cache )
									foreach ( var val in item.Value )
									{
										var target = val.c;
										// if ( !GetEnable( target, true ) || !Tools.PastValidate( target ) ) continue;
										if ( !__dic.ContainsKey( target.GetType() ) ) __dic.Add( target.GetType(), new List<Component>() );
										__dic[ target.GetType() ].Add( target );
									}
							}


							if ( __dic.Count != 0 )
							{
								menu.AddSeparator( "Remove Component/" );
								foreach ( var item in __dic )
								{
									var n = item.Key.Name;
									var l =item.Value;
									hasRemove = true;
									menu.AddItem( new GUIContent( "Remove Component/Remove '" + n + "' for all selected gameobjects" ), false, () => {
										foreach ( var s in l )
										{
											var oaa = s.gameObject;
											Undo.DestroyObjectImmediate( s );
											adapter.MarkSceneDirty( oaa.scene );
										}
									} );
								}
							}
						}

						if ( !hasRemove )
						{
							menu.AddDisabledItem( new GUIContent( "Paste Component Values" ) );
						}
					}
					// fill( ( target ) => {
					//     Tools.Paste( target );
					// } );
					/*
                                        if ( clickedToSelected && sel.Length > 1 )
                                        {
                                            menu.AddSeparator( "Paste Component Values/" );
                                            menu.AddItem( new GUIContent( category + "/" + methodInfo.Name ), false, () => { } );
                                        }

                                        if ( adapter.ha.SELECTED_GAMEOBJECTS().All( selO => selO.go != o ) )
                                        {
                                            Tools.Paste( target );
                    #if !DISABLE_PING
                                                        if ( Hierarchy.par.ENABLE_PING_Fix ) adapter.TRY_PING_OBJECT( o );
                    #endif
                                        }
                                        else
                                        {
                                            foreach ( var objectToUndo in adapter.ha.SELECTED_GAMEOBJECTS() ) /////
                                            {
                                                var c = objectToUndo.go.GetComponents(target.GetType());
                                                foreach ( var variable in c ) Tools.Paste( variable );
                                                // if (Hierarchy.par.ENABLE_PING_Fix) adapter.TRY_PING_OBJECT(objectToUndo);
                                            }
                                        }
                    */
					/*
                                        {
                                            var name = "'" + component.GetType().Name + "'";

                                            if ( components.Count > SHORT ) name = "Paste Component Values As New/" + name;
                                            else name = "Paste Component Values As New";

                                            menu.AddItem( new GUIContent( name ), false, () =>
                                               {
                                                   if ( adapter.ha.SELECTED_GAMEOBJECTS().All( selO => selO.go != o ) )
                                                   {
                                                       Tools.PasteComponentAsNew( target );

                    #if !DISABLE_PING
                                                        if ( Hierarchy.par.ENABLE_PING_Fix ) adapter.TRY_PING_OBJECT( o );

                    #endif
                                                   }

                                                   else
                                                   {
                                                       foreach ( var objectToUndo in adapter.ha.SELECTED_GAMEOBJECTS() ) /////
                                                       {
                                                           var c = objectToUndo.go.GetComponents(target.GetType());

                                                           foreach ( var variable in c )
                                                           {
                                                               Tools.PasteComponentAsNew( variable );
                                                           }

                                                           // if (Hierarchy.par.ENABLE_PING_Fix) adapter.TRY_PING_OBJECT(objectToUndo);
                                                       }
                                                   }

                                                   ResetStack();
                                                   // EditorUtility.CopySerialized
                                               } );
                                        }*/
					//COPY PASTE




					menu.AddSeparator( "" );
					bool qweqwe = false;

					if ( components.Count == 1 && callbackType != null )
					{
						if ( components.FirstOrDefault( c => c.GetType() == callbackType ) != null && components.First( c => c.GetType() == callbackType ) is MonoBehaviour )
						{
							var target = MonoScript.FromMonoBehaviour(components.First(c => c.GetType() == callbackType) as MonoBehaviour);
							var path = AssetDatabase.GetAssetPath(target);

							if ( string.IsNullOrEmpty( path ) ) menu.AddDisabledItem( new GUIContent( "Edit Script" ) );
							else
								menu.AddItem( new GUIContent( "Edit Script" ), false, () => //EditorUtility.opebn
								{
									AssetDatabase.OpenAsset( target.GetInstanceID() );
									//InternalEditorUtility.OpenFileAtLineExternal( path, 0 );
									//  Selection.objects = new[] { target };
								} );
							menu.AddItem( new GUIContent( "Select '" + callbackType.Name + "' in Project" ), false, () => { Selection.objects = new[] { target }; } );
							qweqwe = true;
						}
					}

					else
					{
						var res = components.Where(c => c is MonoBehaviour).ToArray();

						if ( res.Length != 0 )
						{
							foreach ( var component in res )
							{
								var target = MonoScript.FromMonoBehaviour(component as MonoBehaviour);
								menu.AddItem( new GUIContent( "Select Component in Project/'" + component.GetType().Name + "'" ), false, () => { Selection.objects = new[] { target }; } );
								qweqwe = true;
							}
						}
					}


					if ( qweqwe ) menu.AddSeparator( "" );


					//  if ( MenuText != null )
					if ( allowHide )
					{



						if ( !HierarchyCommonData.Instance().IsComponentIconHided( arr.drawCompSingle.GetType().FullName ) )
						{
							/*       menu.AddDisabledItem( new GUIContent( arr.drawCompSingle.GetType() + " Hiden" ) );
                              }
                              else
                              {*/
							var MenuText = "Hide " + arr.drawCompSingle.GetType().Name + " icon";

							var type = callbackType;
							menu.AddItem( new GUIContent( MenuText ), false, () => {
								HierarchyCommonData.Instance().SetUndo( "Hide Icon" );
								HierarchyCommonData.Instance().SetComponentIconHide( type.FullName, true );
								ResetStack();
							} );

							qweqwe = true;

							menu.AddSeparator( "" );
						}

						/* menu.AddSeparator("");
                         menu.AddItem(new GUIContent("Open Settings"), false, SETUPROOT.showWindow);*/
					}






					menu.AddItem( new GUIContent( "Open Icons Settings" ), false, () => {
						Settings.MainSettingsEnabler_Window.Select<Settings.IC_Window>();

						//LeftClickOnRightModsHeaderMenu.SHOW_HIER_SETTINGS_GENERICMENU(); 
					} );

					menu.ShowAsContext();
				}


				Tools.EventUse();
			}

			if ( EVENT.button == adapter.MOUSE_BUTTON_1 )
			{
				Tools.EventUse();
				var arr = (ARGS)data.args;
				var drawComps = arr.drawCompsArr != null ? arr.drawCompsArr.Where(c => c.comp).Select(c => c.comp).ToList() : new List<Component>();
				if ( drawComps.Count == 0 && arr.drawCompSingle ) drawComps.Add( arr.drawCompSingle );

				var mp = new MousePos(EVENT.mousePosition, MousePos.Type.Search_356_0, !callFromExternal(), adapter);
				var oldFilt = lastFocusRoot == null ? null : lastFocusRoot.SearchFilter;
				/*  int[] contentCost = new int[0];
                  GameObject[] obs = new GameObject[0];*/
				var captureCall = callFromExternal();
				Action<Type> result = (filt) =>
				{
                    /*  if (captureCall && lastFocusRoot != null)
                      {
                          // contentCost = lastFocusRoot.contentCost.ToArray();
                          CallHeared(lastFocusRoot.objects.ToArray(), out obs, out contentCost, filt);
                      } else
                      {
                    
                          // if (EditorSceneManager.GetActiveScene().rootCount != 0) CallHeared(Utilities.AllSceneObjectsInterator().GetEnumerator(), out obs, out contentCost, filt);
                          if (EditorSceneManager.GetActiveScene().rootCount != 0) CallHeared(Utilities.AllSceneObjects(), out obs, out contentCost, filt);
                      }
                    
                      lastFocusRoot = (FillterData)FillterData.Init(mp, SearchHelper + " " + filt.Name, filt.Name, obs, contentCost, null, this, filt);*/

                    var ttt = filt.Name.Replace(adapter.pluginname + "_KEY_#1", "default");
					lastFocusRoot = (Windows.SearchWindow)Windows.SearchWindow.Init(mp, SearchHelper + " " + ttt, ttt, CallHeaderFiltered(filt), this, adapter.window, _o);

					if (captureCall && oldFilt != null && lastFocusRoot)
						lastFocusRoot.FiltersOf = oldFilt;
				};


				if ( drawComps.Count > 1 )
				{ /* if (GUID != null) callbackType = readcomps.First(asd => ComponentToGUID(asd) == GUID).GetType();
					     else*/
					{
						var menu = new GenericMenu();

						foreach ( var component in drawComps )
						{
							var comp = component;
							menu.AddItem( new GUIContent( comp.GetType().Name ), false, () => {
								result( comp.GetType() );
							} );
						}

						menu.ShowAsContext();
						Tools.EventUse();
						return;
					}
				}


				result( drawComps[ 0 ].GetType() );
			}
		}
		static Type tr_t = typeof(Transform);
	}
}
