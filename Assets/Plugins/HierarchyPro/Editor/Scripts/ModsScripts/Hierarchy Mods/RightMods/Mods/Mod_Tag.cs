using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EMX.HierarchyPlugin.Editor.Mods
{




	class Mod_Tag : RightModBaseClass
	{
		public Mod_Tag( int restWidth, int sibbildPos, bool enable, PluginInstance adapter ) : base( restWidth, sibbildPos, enable, adapter ) { }
		internal override void Subscribe( EditorSubscriber sbs ) { }

		// static SerializedProperty _findProperty;
		private string[] tags {
			get { return UnityEditorInternal.InternalEditorUtility.tags; }
		}
		internal override bool USE_CONTENT_SHRINKING() { return true; }

		static Color alpha = new Color(1, 1, 1, 0.3f);

		static GUIContent _content = new GUIContent();

		internal override void ModuleCommonGenericMenu( GenericMenu menu, GameObject o, object data, string sub = "" )
		{



			var pos = new MousePos(EVENT.mousePosition, MousePos.Type.Input_128_68, !callFromExternal(), adapter);
			//  var pos = InputData.WidnwoRect(!callFromExternal(), EVENT.mousePosition, 128, 68, adapter );
			var w= adapter.window;


			var l = tags;

			// var select = -1;
			// var ordered = l.OrderBy(f => f.Key).Select(f => f.Value).ToArray();va
			Action<int> Callback = (res) =>
				{
					SetTAg(o, l[res]);
                    /*  Undo.RecordObject(o, "Change tag");
                      o.tag = l[res];
                      Hierarchy.SetDirty(o);
                      if (Hierarchy.par.ENABLE_PING_Fix) Tools.TRY_PING_OBJECT(o);*/
                };


			if ( o || adapter.ha.SELECTED_GAMEOBJECTS().Length != 0 )
			{
				var content =  data as GUIContent;
				if ( content == null && Selection.activeObject ) content = new GUIContent();
				if ( content != null )
				{
					var oldSelect = content.text;
					for ( int i = 0; i < l.Length; i++ )
					{
						var ind = i;
						var c = new GUIContent(content);
						c.text = sub + l[ i ];
						//  menu.AddItem( c, MT.GET_STRING( data.content, adapter.par_e.RIGHT_TAGS_UPPERCASE ) == oldSelect, ( ) => Callback( ind ) );
						menu.AddItem( c, c.text == oldSelect, () => Callback( ind ) );
					}
				}
			}


			menu.AddSeparator( sub );


			if ( !o && adapter.ha.SELECTED_GAMEOBJECTS().Length == 0 ) menu.AddDisabledItem( new GUIContent( sub + "+ Assign a New tag" ) );
			else menu.AddItem( new GUIContent( sub + "+ Assign a New tag" ), false, () => {
				Windows.InputWindow.Init( pos, "New tag name's", w, ( str ) => {
					if ( string.IsNullOrEmpty( str ) ) return;
					str = str.Trim();
					var lowwer = l.Select(ord => ord.ToLower()).ToList();
					var ind = lowwer.IndexOf(str.ToLower());
					if ( ind != -1 )
					{
						SetTAg( o, l[ ind ] );
						/* Undo.RecordObject(o, "Change tag");
                         o.tag = l[ind];
                         Hierarchy.SetDirty(o);
                         if (Hierarchy.par.ENABLE_PING_Fix) Tools.TRY_PING_OBJECT(o);*/
					}
					else
					{
						UnityEditorInternal.InternalEditorUtility.AddTag( str );

						SetTAg( o, str );
						/*  Undo.RecordObject(o, "Change tag");
                          o.tag = str;
                          Hierarchy.SetDirty(o);
                          if (Hierarchy.par.ENABLE_PING_Fix) Tools.TRY_PING_OBJECT(o);*/
					}
				} );
			} );

			menu.AddSeparator( sub );

			menu.AddItem( new GUIContent( sub + "Show only uppercase letters" ), adapter.par_e.RIGHT_TAGS_UPPERCASE != 0, () => {
				adapter.par_e.RIGHT_TAGS_UPPERCASE = 1 - adapter.par_e.RIGHT_TAGS_UPPERCASE;
			} );
			menu.AddSeparator( sub );
			menu.AddItem( new GUIContent( sub + "Show 'Tags And Layers' Settings" ), false, () => {
				Selection.objects = AssetDatabase.LoadAllAssetsAtPath( "ProjectSettings/TagManager.asset" );
				Tools.FocusToInspector();
			} );
		}

		public override void Draw()
		{
			if ( !START_DRAW( drawRect, adapter.o ) ) return;

			//if ( adapter.o.lastContentRect[savedData.temp_i].HasValue ) adapter.o.lastContentRect[savedData.temp_i] = null;
			var o = adapter.o.go;

			_content.text = _content.tooltip = o.tag;
			// content.tooltip = base.ContextHelper;
			MT.GET_STRING( _content, callFromExternal() ? 0 : adapter.par_e.RIGHT_TAGS_UPPERCASE );
			if ( _content.text == "" ) _content.text = "...Missing";
			// content.tooltip = content.text;
			var hasContent = false;
			//            !callFromExternal() ? adapter.STYLE_LABEL_8_right : adapter.STYLE_LABEL_8_WINDOWS_right
			var style =  !callFromExternal() ? RightModsStyles.STYLE_LABEL_8_right : RightModsStyles.STYLE_LABEL_8_WINDOWS_right;

			Rect r;

			if ( !string.IsNullOrEmpty( o.tag ) && o.tag != "Untagged" )
			{ /*  var fs = Adapter.GET_SKIN().label.fontSize;
                var al = Adapter.GET_SKIN().label.alignment;
                Adapter.GET_SKIN().label.alignment = TextAnchor.MiddleLeft;
                if (!callFromExternal()) Adapter.GET_SKIN().label.fontSize = adapter.FONT_8();
                else Adapter.GET_SKIN().label.fontSize = adapter.WINDOW_FONT_8();*/
				// Adapter.GET_SKIN().label.fontSize = Hierarchy.FONT_8();
				hasContent = true;
				/* GUI.enabled = o.activeInHierarchy;
                 GUI.Label( drawRect, content, !callFromExternal() ? adapter.STYLE_LABEL_8 : adapter.STYLE_LABEL_8_WINDOWS );
                 GUI.enabled = true;*/
				r = adapter.modsController.rightModsManager.DrawCursorRect( ref drawRect, style, _content );
				Draw_Label( r, _content, style, true );

				/*  Adapter.GET_SKIN().label.alignment = al;
                  Adapter.GET_SKIN().label.fontSize = fs;*/
			}
			else
			{
				//if ( MT.__ == null )
				//{
				//    MT.__ = new GUIStyle( adapter.label );
				//    MT.__.alignment = MT.__Align;
				//}

				/*  var c = GUI.color;
                  GUI.color *= alpha;
                  var a = adapter.label.alignment;
                  adapter.label.alignment = __;
                  GUI.Label( drawRect, "-", adapter.label );
                  adapter.label.alignment = a;
                  GUI.color = c;*/
				if ( adapter.par_e.RIGHT_SKIP_HYPHEN_FOR_TAGS )
				{
					r = adapter.modsController.rightModsManager.DrawCursorRect( ref drawRect, style, GetEmptyConten_ForceEmpty );
					Draw_Label( r, GetEmptyConten_ForceEmpty, style, true, alpha );
				}
				else
				{
					r = adapter.modsController.rightModsManager.DrawCursorRect( ref drawRect, style, GetEmptyConten_Common );
					Draw_Label( r, GetEmptyConten_Common, style, true, alpha );
				}

			}

			//  var bg = Adapter.GET_SKIN().button.active.background;
			// Adapter.GET_SKIN().button.active.background = Hierarchy.GetIcon("BUT");

			// if (drawRect.Contains(EVENT.mousePosition) && EVENT.type != EventType.repaint) MonoBehaviour.print(EVENT.type);



			Draw_ModuleButton( r, _content, BUTTON_ACTION_HASH, hasContent, drawPointer: true );

			/* if ( adapter.ModuleButton( drawRect, null, hasContent ) )
             {
            
            
             }*/

			// Adapter.GET_SKIN().button.active.background = bg;

			END_DRAW( adapter.o, savedData.temp_i );
			//if ( adapter.EVENT.type == EventType.Layout ) 
			if (savedData.temp_i != -1)	adapter.o.lastContentRectLayout[ savedData.temp_i ].SET(ref r);

		}



		DrawStackMethodsWrapper __BUTTON_ACTION_HASH = null;

		DrawStackMethodsWrapper BUTTON_ACTION_HASH {
			get { return __BUTTON_ACTION_HASH ?? (__BUTTON_ACTION_HASH = new DrawStackMethodsWrapper( BUTTON_ACTION )); }
		}

		void BUTTON_ACTION( Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o )
		{
			var o = _o.go;

#pragma warning disable
			var content = data.content;
#pragma warning restore
			if ( EVENT.button == adapter.MOUSE_BUTTON_0 )
			{
				GenericMenu menu = new GenericMenu();



				/*  menu.AddItem(new GUIContent("Show 'Tags And Layers' Settings"), false, () => {
                      Selection.objects = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
                  });
                  menu.AddSeparator("");*/



				ModuleCommonGenericMenu( menu, o, data.content );

				menu.ShowAsContext();
				Tools.EventUse();
			}


			if ( EVENT.button == adapter.MOUSE_BUTTON_1 )
			{
				Tools.EventUse();
				/*  int[] contentCost = new int[0];
                  GameObject[] obs = new GameObject[0];
                
                  if (Validate(o))
                  {
                      if (EditorSceneManager.GetActiveScene().rootCount != 0) CallHeaderFiltered(out obs, out contentCost, o.tag);
                  } else
                  {
                      CallHeader(out obs, out contentCost);
                  }
                
                  FillterData.Init(EVENT.mousePosition, SearchHelper, o.tag, obs, contentCost, null, this);
                */
				var mp = new MousePos(EVENT.mousePosition, MousePos.Type.Search_356_0, !callFromExternal(), adapter);

				Windows.SearchWindow.Init( mp, SearchHelper, Validate( o ) ? o.tag : "All tags",
					Validate( o ) ? CallHeaderFiltered( o.tag ) : CallHeader(),
					this, adapter.window, _o );
				// EditorGUIUtility.ic
			}
		}




		void SetTAg( GameObject o, string tag )
		{
			if ( o && adapter.ha.SELECTED_GAMEOBJECTS().All( selO => selO.go != o ) )
			{
				Undo.RecordObject( o, "Change tag" );
				o.tag = tag;
				adapter.SetDirty( o );
				ResetStack( o.GetInstanceID() );
				adapter.MarkSceneDirty( o.scene );
				if ( adapter.par_e.ENABLE_OBJECTS_PING ) Tools.TRY_PING_OBJECT( o );
			}
			else
			{
				foreach ( var objectToUndo in adapter.ha.SELECTED_GAMEOBJECTS() )
				{
					Undo.RecordObject( objectToUndo.go, "Change tag" );
					objectToUndo.go.tag = tag;
					adapter.SetDirty( objectToUndo.go );
					ResetStack( objectToUndo.go.GetInstanceID() );
					adapter.MarkSceneDirty( objectToUndo.scene );
					//  if (Hierarchy.par.ENABLE_PING_Fix) Tools.TRY_PING_OBJECT(objectToUndo);
				}
			}
		}



		bool Validate( GameObject o )
		{
			return !string.IsNullOrEmpty( o.tag ) && o.tag != "Untagged";
		}



		/* FillterData.Init(EVENT.mousePosition, SearchHelper, LayerMask.LayerToName(o.layer),
                     Validate(o) ?
                     CallHeaderFiltered(LayerMask.LayerToName(o.layer)) :
                     CallHeader(),
                     this);*/
		/** CALL HEADER */
		internal override Windows.SearchWindow.FillterData_Inputs CallHeader()
		{
			var result = new Windows.SearchWindow.FillterData_Inputs(callFromExternal_objects)
			{
				Valudator = (oo) => Validate(oo.go),
				SelectCompareString = (d, i) => d.go.tag,
				SelectCompareCostInt = (d, i) =>
				{
					var cost = i;
					cost += d.go.activeInHierarchy ? 0 : 100000000;
					return cost;
				}
			};
			return result;
		}

		internal Windows.SearchWindow.FillterData_Inputs CallHeaderFiltered( string filter )
		{
			var result = CallHeader();
			result.Valudator = s => Validate( s.go ) && s.go.tag == filter;
			return result;
		}
		/** CALL HEADER */


		/*
                    internal override bool CallHeader(out GameObject[] obs, out int[] contentCost)
                    {
                        obs = Utilities.AllSceneObjects().Where(Validate).ToArray();
                        contentCost = obs
                           .Select((d, i) => new { name = d.tag, startIndex = i, obj = d })
                           .OrderBy(d => d.name)
                            .Select((d, i) => {
                                var cost = i;
                                cost += d.obj.activeInHierarchy ? 0 : 100000000;
                                return new { d.startIndex, cost = cost };
                            })
                           .OrderBy(d => d.startIndex)
                           .Select(d => d.cost).ToArray();
                        return true;
                    }
        
                    internal void CallHeaderFiltered(out GameObject[] obs, out int[] contentCost, string filter)
                    {
                        obs = Utilities.AllSceneObjects().Where(s => Validate(s) && s.tag == filter).ToArray();
                        contentCost = obs
                           .Select((d, i) => new { name = d.tag, startIndex = i, obj = d })
                           .OrderBy(d => d.name)
                           .Select((d, i) => {
                               var cost = i;
                               cost += d.obj.activeInHierarchy ? 0 : 100000000;
                               return new { d.startIndex, cost = cost };
                           })
                           .OrderBy(d => d.startIndex)
                           .Select(d => d.cost).ToArray();
                    }*/
	}






	class MT
	{
		internal static TextAnchor __Align = TextAnchor.MiddleRight;
		//internal static GUIStyle __;

		static Dictionary<string, string> uppercast = new Dictionary<string, string>();

		internal static void GET_STRING( GUIContent str, int upper )
		{ /*   if (str == "MySortingLayer")
               MonoBehaviour.print("ASD");*/
			if ( upper == 0 || string.IsNullOrEmpty( str.text ) ) return;
			if ( !uppercast.ContainsKey( str.text ) )
			{
				string s= "";
				var temp = str.text.ToUpper();
				for ( int i = 0; i < temp.Length; i++ )
					if ( temp[ i ] == str.text[ i ] ) s += temp[ i ];
				uppercast.Add( str.text, s );
				/*   
                   .ToCharArray()
               .Select((s, i) => s == str.text[i] ? s.ToString() : null)
               .Where(s => !string.IsNullOrEmpty(s) && s != " ");
               var res = temp.Count() == 0 ? str[0].ToString() : temp.Aggregate((a, b) => a + b);
               uppercast.Add( str, res );*/
			}
			str.text = uppercast[ str.text ];
			// return uppercast[ str ];
		}
	}

}
