using System;
using System.Linq;
using UnityEngine;
using UnityEditor;




namespace EMX.HierarchyPlugin.Editor.Mods
{





	internal class SetActiveMod : DrawStackAdapter
	{
		internal string HeaderText = "SetActive GameObject";
		internal string ContextHelper = "SetActive GameObject";


		PluginInstance adapter = null;
		internal SetActiveMod( int pid ) : base( pid )
		{

			adapter = Root.p[ pid ];
			/*  if (UnityVersion.UNITY_CURRENT_VERSION >= UnityVersion.UNITY_2019_VERSION)
			  {
				  var duringSceneGui = typeof(SceneView).GetField("duringSceneGui", (System.Reflection.BindingFlags)(-1)) ?? throw new Exception("duringSceneGui");
				  var eventInfo = (Action<SceneView>)duringSceneGui.GetValue(null);
				  eventInfo -= scenegui;
				  eventInfo += scenegui;
				  duringSceneGui.SetValue(eventInfo, eventInfo);
			  }
			  else
			  {

  #pragma warning disable
				 // SceneView.onSceneGUIDelegate -= scenegui;
				//  SceneView.onSceneGUIDelegate += scenegui;
  #pragma warning restore
			  }*/



			t2A = Colors.B_ACTIVE;
			t3A = Colors.B_PASSIVE;
		}
		static void scenegui( SceneView sceneView )
		{
			if ( stateForDrag_B1 != null )
			{
				if ( Event.current.type == EventType.Repaint )
					Handles.PositionHandle( stateForDrag_B1.transform.position,
						UnityEditor.Tools.pivotRotation == PivotRotation.Global ? Quaternion.identity : stateForDrag_B1.transform.rotation );
			}
		}

		Color //t1A = new Color(.1f, .1f, .1f, 1),
			t1A = new Color(.15f, .15f, .17f, 1),
			t1B = new Color(.9f, .9f, .9f, 1),
			t2A = new Color32(200, 200, 200, 255),
			t2B = new Color32(200, 200, 200, 255),
			 t3A = new Color32(55, 55, 55, 35),
		 t3B = new Color32(55, 55, 55, 35);
		// Color t1A = new Color(.1f*2, .1f*2, .1f*2, 1),
		//     t1B = new Color(.3f*2, .35f*2, .4f*2, 1),
		//     t2A = new Color32(200, 200, 200, 255),
		//     t2B = new Color32(230, 230, 230, 255),
		// t3A = new Color32(55/7, 55/7, 55/7, 255),
		// t3B = new Color32(55/7, 55/7, 55/7, 255);
		//static internal int IW = 22;
		internal const int ONE_POS_IW = 10;
		internal static int ONE_POS_IW_FORCOLOR = (ONE_POS_IW - 4) * 2;

		int rawIW;
		internal int rawIW_hard {
			get {
				if ( !adapter.par_e.USE_SETACTIVE_MOD ) return 0;
				var res = adapter.par_e.SET_ACTIVE_POSITION > 0 ? ONE_POS_IW : 22;
				//if ( adapter.par_e.RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER ) res += adapter.par_e.RIGHT_RIGHT_PADDING;

				return res;
			}
		}




		void FRAME( GameObject o )
		{
			if ( SceneView.lastActiveSceneView == null ) return;

			var combinedBounds = new Bounds() { center = o.transform.position };
			var rr = o.GetComponent<Renderer>();

			if ( rr != null ) combinedBounds.Encapsulate( rr.bounds );

			/*  foreach (var render in o.GetComponentsInChildren<Renderer>())
                  if (render != rr) combinedBounds.Encapsulate(render.bounds);*/

			var pars = Root.SceneFrameMethod.GetParameters().Select(p =>
				{
					return p.DefaultValue;
					/*  if (p.ParameterType.IsValueType)
                      {
                          return Activator.CreateInstance(p.ParameterType);
                      }
                      return null;*/
				}
			).ToArray();
			pars[ 0 ] = combinedBounds;

			if ( pars.Length > 1 ) pars[ 1 ] = !adapter.par_e.SET_ACTIVE_SMOOTH_FRAME;

			Root.SceneFrameMethod.Invoke( SceneView.lastActiveSceneView, pars );
			//SceneView.lastActiveSceneView.Frame(combinedBounds, false);
		}

		internal void Subscribe( EditorSubscriber sbs )
		{
			sbs.BuildedOnGUI_first.Add( FirstFrameOnGUI );
			sbs.AddBuildedOnGUI_middle( Draw );
			sbs.duringSceneGui += scenegui;
		}


		Rect drawRect, firstRect, bgrect;
		bool prefabOffset;
		internal void FirstFrameOnGUI()
		{
			SMALL = adapter.par_e.SET_ACTIVE_POSITION == 2;
			STYLE = adapter.par_e.SET_ACTIVE_STYLE;
			drawRect = adapter.fullLineRect;


			rawIW = rawIW_hard;

			bgrect = drawRect;

			bgrect.x = bgrect.x + bgrect.width - rawIW + 4;
			bgrect.x -= adapter.rightOffset;

			if ( SMALL ) bgrect.x -= 3;
			if ( SMALL ) bgrect.width = rawIW;
			else bgrect.width = rawIW - 3;

			adapter.rightOffset += bgrect.width;



			prefabOffset = adapter.par_e.SET_ACTIVE_PREFAB_BUTTON_OFFSET;
			adapter.ha.prebapButtonStyle.margin.right = adapter.par_e.SET_ACTIVE_PREFAB_BUTTON_OFFSET ? (SMALL ? (rawIW) : (rawIW - 3)) : 0;

			if ( adapter.par_e.RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER && adapter.par_e.USE_RIGHT_ALL_MODS )
			{
				adapter.ha.prebapButtonStyle.margin.right += adapter.par_e.RIGHT_RIGHT_PADDING;
				bgrect.x -= adapter.par_e.RIGHT_RIGHT_PADDING;
			}
			firstRect = bgrect;

			///bgrect.width += 40;
			///a
			///

			if ( adapter.ha.hasShowingPrefabHeader /*&& adapter.o.HasBrefabButton*/)
				if ( prefabOffset )
					bgrect.x += adapter.ha.PREFAB_BUTTON_SIZE - 2;
			bgrect.y = adapter._first_FullLineRect.y;
			bgrect.height = adapter._last_FullLineRect.y + adapter._last_FullLineRect.height - bgrect.y;
			bgrect.width += adapter.ha.PREFAB_BUTTON_SIZE;

			//if ( adapter.GET_SLOW_oldProSkin )
			//{
			//	if ( !SMALL )//|| !o.activeInHierarchy )
			//	{
			//		var c = t1A;
			//		c.a *= SMALL ? 0.8f : 1;
			//		EditorGUI.DrawRect( bgrect, c );
			//	}
			//}
			//else
			//{
			//	// if ( o.activeInHierarchy )
			//	{
			//		
			//		var c = Color.Lerp( Color.white, Colors.EditorBGColor, 0.5f);
			//		// if ( UnityVersion.UNITY_CURRENT_VERSION >= UnityVersion.UNITY_2019_3_0_VERSION ) c.a *= 0.75f;
			//		c.a *= 0.5f;
			//		if ( SMALL ) c.a *= 0.5f;
			//		EditorGUI.DrawRect( bgrect, t1B * c );
			//	}
			//}

			bgrect.x += 3;
			bgrect.x += 7;
			bgrect.width -= 22;
			//EditorGUI.DrawRect( bgrect, c );



			FIRST = true;

			//GL.PushMatrix();
			//GL.Begin( GL.QUADS );
			//GUI.depth = 1;
			//adapter.HL_SET.DEFAULT_SHADER_SHADER.ExternalMaterialReference.SetPass( 0 );
			//DRAW(bgrect,c);
			//GL.End();
			//GL.PopMatrix();
		}
		bool FIRST = false;
		//static int _MainTex = Shader.PropertyToID("_MainTex");
		//void DRAW( Rect rect, Color color )
		//{
		//	GL.Color( color );
		//	GL.Vertex3( rect.x, rect.y, 0 );
		//	GL.Vertex3( rect.x, rect.y + rect.height, 0 );
		//	GL.Vertex3( rect.x + rect.width, rect.y + rect.height, 0 );
		//	GL.Vertex3( rect.x + rect.width, rect.y, 0 );
		//}


		int STYLE;
		bool SMALL;
		// bool SMALL { get { return false; } }

		void casda( ref Rect drawRect )
		{
			if ( drawRect.x % 1 != 0 ) drawRect.x = Mathf.RoundToInt( drawRect.x );
			if ( drawRect.y % 1 != 0 ) drawRect.y = Mathf.CeilToInt( drawRect.y );
			if ( drawRect.width % 1 != 0 ) drawRect.width = Mathf.RoundToInt( drawRect.width );
			if ( drawRect.height % 1 != 0 ) drawRect.height = Mathf.RoundToInt( drawRect.height );

		}

		// GUIContent content = new GUIContent();
		public void Draw()
		{

			if ( FIRST )
			{
				var c = Color.Lerp( adapter.GET_SLOW_oldProSkin ? t1A : t1B, Colors.EditorBGColor, (SMALL ? 0.75f : 0.5f)-0.25f);
				c.a = 0.8f;
				//adapter.HL_SET.DEFAULT_SHADER_SHADER.ExternalMaterialReference.SetPass( 0 );
				//adapter.HL_SET.DEFAULT_SHADER_SHADER.ExternalMaterialReference.SetTexture( _MainTex, null );
				//GL.PushMatrix();
				//GL.Begin( GL.QUADS );
				//DRAW( bgrect, c );
				//GL.End();
				//GL.PopMatrix();
				EditorGUI.DrawRect( bgrect, c );
				FIRST = false;
			}



			drawRect = firstRect;
			drawRect.y = adapter.fullLineRect.y;
			//drawRect.width = firstRect.width;
			///	drawRect.height = firstRect.height;
			if ( adapter.EVENT.type == EventType.Repaint && !adapter.baked_HARD_BAKE_ENABLED ) adapter.FADE_IF_NO_BAKE( drawRect );

			if ( !START_DRAW( drawRect, adapter.o ) ) return;


			var o = adapter.o.go;
			// var IW = (float) rawIW;



			///	drawRect.x = drawRect.x + drawRect.width - rawIW + 4;
			if ( SMALL ) drawRect.x -= 3;
			if ( SMALL ) drawRect.width = rawIW;
			else drawRect.width = rawIW - 3;
			//if ( adapter.par_e.SET_ACTIVE_POSITION == 2 && Adapter.UNITY_CURRENT_VERSION < Adapter.UNITY_2019_1_1_VERSION ) drawRect.x -= 4;
			/* if (drawRect.y == 0)
             {
                 drawRect.y += 1;
                 drawRect.height -= 1;
             }*/





			var buttonRect = drawRect;



			/*    tR.width += 40;
                tR.x = tR.x + tR.width - 18;*/
			//  tR.y += 1;

			// var oldW = drawRect.width;
			var oldH = drawRect.height;


			if ( !SMALL )
			{
				drawRect.width = 12;
				drawRect.height = 10;
				drawRect.x += 3;
			}

			else
			{
				drawRect.height = 10;

				if ( !SMALL )
					drawRect.width = rawIW - 4;
				else
				{
					drawRect.width = rawIW - 1;

					/*   if ( adapter.hashoveredItem && adapter.hoverID == _o.id)
                       drawRect.width = IW ;
                    else
                       drawRect.width = IW - 6;*/
				}
			}

			/**/
			//////////////////


			// drawRect.x += (oldW - drawRect.width) / 2;
			drawRect.y += (oldH - drawRect.height) / 2;







			if ( adapter.ha.hasShowingPrefabHeader /*&& adapter.o.HasBrefabButton*/)
			{


				if ( prefabOffset )
				{
					if ( !SMALL )
					{
						drawRect.x += buttonRect.width - 3;
						buttonRect.x += buttonRect.width - 3;
					}
					else
					{
						drawRect.x += buttonRect.width + 7;
						buttonRect.x += buttonRect.width + 7;
					}

				}
				else
				{

				}


				///globalgui	PrefabModeButton
				//	GameObjectTreeViewItem
				///	showPrefabModeButton
				//	PrefabUtility.GetIconForGameObject( objectPptr );
			}








			// float t=  0;

			// var oldBoxT = Adapter.GET_SKIN().box.normal.background;
			bool skip = false;

			if ( !SMALL )
			{
				if ( !o.activeSelf ) drawRect.x += drawRect.width / 2;
				else drawRect.width = 16;
			}
			else
			{
				if ( !o.activeSelf && (!o.transform.parent || o.transform.parent.gameObject.activeInHierarchy) ) drawRect.width /= 1.5f;
				else if ( !o.activeSelf && STYLE == 1 ) skip = true;
				else if ( o.activeSelf && !o.activeInHierarchy ) drawRect.width /= 1.5f;
				else if ( !o.activeInHierarchy ) skip = true;
			}


			var rr = drawRect;

			if ( SMALL )
			{
				rr.x += 2;
				rr.width -= 4;
			}

			if ( !SMALL && STYLE == 1 ) rr.x += 2;


			if ( !skip )
			{
				if ( o.activeSelf )
				{
					if ( STYLE == 0 )
					{
						//if ( EditorGUIUtility.isProSkin ) Draw_GUITexture( rr, o.activeInHierarchy ? t2A : t3A );
						//else Draw_GUITexture( rr, o.activeInHierarchy ? t2B : t3B );

						if ( adapter.GET_SLOW_oldProSkin ) Draw_GUITexture( rr, adapter.GetNewIcon( NewIconTexture.RightMods, "SETACTIVE_BG_WHITE" ), USE_GO: true, guiColor: o.activeInHierarchy ? t2A : t3A );
						else Draw_GUITexture( rr, adapter.GetNewIcon( NewIconTexture.RightMods, "SETACTIVE_BG_WHITE" ), USE_GO: true, guiColor: o.activeInHierarchy ? t2B : t3B );
					}
					else
					{
						// Draw_GUITexture( rr, new Color( 0, 0, 0, o.activeInHierarchy ? 0.3f : 0.0f ) );
						Draw_GUITexture( rr, adapter.GetNewIcon( NewIconTexture.RightMods, "SETACTIVE_BG_WHITE" ), USE_GO: true, guiColor: new Color( 0, 0, 0, o.activeInHierarchy ? 0.3f : 0.0f ) );
					}



				}
				else
				{
					var c = Color.white;

					if ( !SMALL && STYLE == 1 ) rr.width /= 1.5f;
					if ( !SMALL || STYLE == 1 ) c *= new Color( 1, 1, 1, 0.4f );

					if ( STYLE == 0 )
					{
						if ( adapter.GET_SLOW_oldProSkin ) Draw_GUITexture( rr, o.activeInHierarchy ? t2A : t3A, c );
						else Draw_GUITexture( rr, o.activeInHierarchy ? t2B : t3B, c );
					}
					else
					{
						Draw_GUITexture( rr, new Color( 0, 0, 0, o.activeInHierarchy ? 0.3f : 0.0f ), c );
					}

					//  GUI. color = c;
				}

			}




			rr.width = rr.height;
			//Debug.Log(rr);
			casda( ref rr );
			//Draw_GUITexture( rr, adapter.GetNewIcon( NewIconTexture.RightMods, adapter.GET_SLOW_oldProSkin ? "SETACTIVE" : "SETACTIVEPERSONAL" ), USE_GO: true );
			var C = GUI.color;
			if ( STYLE == 0 && !adapter.GET_SLOW_oldProSkin )
			{
				C.r *= 0.4f;
				C.g *= 0.4f;
				C.b *= 0.4f;
			}
			Draw_GUITexture( rr, adapter.GetNewIcon( NewIconTexture.RightMods, adapter.GET_SLOW_oldProSkin ? "SETACTIVE" : "SETACTIVEPERSONAL" ), USE_GO: true, guiColor: C );

			if ( STYLE == 2 )
			{
				if ( SMALL )
				{ // if ( (o.activeSelf && o.activeInHierarchy || !o.activeSelf && !o.activeInHierarchy) )
					if ( !skip )
					{
						if ( o.activeInHierarchy )
						{
							//adapter.gl.DRAW_TAP_GLOW(drawRect);
						}
						else
						{ /*var c = GUI .color;
							GUI .color *= new Color( 1, 1, 1, 0.3f );*/
							///p.gl.DRAW_TAP_GLOW( drawRect, 0.3f );
							throw new NullReferenceException();
							///Draw_GUITexture( drawRect, adapter.GetIcon( "SETUP_BUTTON_HOVER" ), new Color( 1, 1, 1, 0.3f ) );
							// GUI .color = c;
						}
					}
				}

				else
				{
					if ( o.activeInHierarchy ) //Draw_GUITexture( drawRect, adapter.GetIcon( "SETUP_BUTTON_HOVER" ) );
						throw new NullReferenceException();
					//p.gl.DRAW_TAP_GLOW( drawRect );

					else
					{
						if ( !o.activeSelf )
						{
							throw new NullReferenceException();
							/*var c = GUI .color;
							GUI .color *= new Color( 1, 1, 1, 0.5f );*/
							///p.gl.DRAW_TAP_GLOW( drawRect, 0.5f );
							//Draw_GUITexture( drawRect, adapter.GetIcon( "SETUP_BUTTON_HOVER" ), new Color( 1, 1, 1, 0.5f ) );
							// GUI .color = c;
						}
						else
						{
							throw new NullReferenceException();
							/* var c = GUI .color;
							 GUI .color *= new Color( 1, 1, 1, 0.25f );*/
							///p.gl.DRAW_TAP_GLOW( drawRect, 0.25f );
							//Draw_GUITexture( drawRect, adapter.GetIcon( "SETUP_BUTTON_HOVER" ), new Color( 1, 1, 1, 0.25f ) );

							//GUI .color = c;
						}
					}
				}
			}

			buttonRect.height = firstRect.height;

			if ( !SMALL ) buttonRect.width = rawIW - 4;
			else buttonRect.width = rawIW;

			//   if ( adapter.PREFAB_BUTTON_SIZE != 0 && adapter.par_e.SET_ACTIVE_POSITION != 1 && adapter.FindPrefabRoot( _o.go ) != _o.go ) tR.width += adapter.PREFAB_BUTTON_SIZE;

			CHANGE_CURSOR = adapter.par_e.SET_ACTIVE_CHANGE_BUTTON_CURSOR;
			Draw_Action( buttonRect, SET_ACTIVE_ACTION_HASH, null );



			END_DRAW( adapter.o, -1 );
		}

		static bool CHANGE_CURSOR;
		//GUIStyle style;
		static bool? stateForDrag_B0 = null;
		static GameObject stateForDrag_B1 = null;

		GUIContent _CONTENT_STYLE_1, _CONTENT_STYLE_OTHER;

		GUIContent CONTENT_STYLE_1 {
			get {
				if ( _CONTENT_STYLE_1 == null )
				{
					_CONTENT_STYLE_1 = new GUIContent();
					_CONTENT_STYLE_1.text = "";
					_CONTENT_STYLE_1.tooltip = "Click or Drag - Show/Hide GameObjects \n( Right-Click or Right-Drag - Frame object in the SceneView )";
				}

				return _CONTENT_STYLE_1;
			}
		}

		GUIContent CONTENT_STYLE_OTHER {
			get {
				if ( _CONTENT_STYLE_OTHER == null )
				{
					_CONTENT_STYLE_OTHER = new GUIContent();
					_CONTENT_STYLE_OTHER.text = "O";
					_CONTENT_STYLE_OTHER.tooltip = "Click or Drag - Show/Hide GameObject \n( Right-Click or Right-Drag - Frame object in the SceneView )";
				}

				return _CONTENT_STYLE_OTHER;
			}
		}

		GUIContent _CONTENT_STYLE_1_disabled, _CONTENT_STYLE_OTHER_disabled;

		GUIContent CONTENT_STYLE_1_disabled {
			get {
				if ( _CONTENT_STYLE_1_disabled == null )
				{
					_CONTENT_STYLE_1_disabled = new GUIContent();
					_CONTENT_STYLE_1_disabled.text = "";
					_CONTENT_STYLE_1_disabled.tooltip = "Object hidden";
				}

				return _CONTENT_STYLE_1_disabled;
			}
		}

		GUIContent CONTENT_STYLE_OTHER_disabled {
			get {
				if ( _CONTENT_STYLE_OTHER_disabled == null )
				{
					_CONTENT_STYLE_OTHER_disabled = new GUIContent();
					_CONTENT_STYLE_OTHER_disabled.text = "O";
					_CONTENT_STYLE_OTHER_disabled.tooltip = "Object hidden";
				}

				return _CONTENT_STYLE_OTHER_disabled;
			}
		}



		DrawStackMethodsWrapper __SET_ACTIVE_ACTION_HASH = null;

		DrawStackMethodsWrapper SET_ACTIVE_ACTION_HASH {
			get {
				if ( __SET_ACTIVE_ACTION_HASH == null )
				{
					__SET_ACTIVE_ACTION_HASH = new DrawStackMethodsWrapper( SET_ACTIVE_ACTION );
				}

				return __SET_ACTIVE_ACTION_HASH;
			}
		}
		// object[] args = new object[2];
		// int SET_ACTIVE_ACTION_HASH = "SET_ACTIVE_ACTION".GetHashCode();
		void SET_ACTIVE_ACTION( Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o )
		{
			var o = _o.go;


			var objectIsHiddenAndLock = !o.activeInHierarchy && o.transform.parent != null && !o.transform.parent.gameObject.activeInHierarchy;
			var content = objectIsHiddenAndLock
				? (STYLE == 1 || SMALL ? CONTENT_STYLE_1_disabled : CONTENT_STYLE_OTHER_disabled)
				: (STYLE == 1 || SMALL ? CONTENT_STYLE_1 : CONTENT_STYLE_OTHER);


			var contains = (inputRect.Contains(adapter.EVENT.mousePosition) || (adapter.hashoveredItem && adapter.hoverID == _o.id));

			if ( contains ) Root.SetMouseTooltip( content, inputRect );

			if ( CHANGE_CURSOR ) EditorGUIUtility.AddCursorRect( inputRect, MouseCursor.Link );

			/*  if (adapter.ha.hasShowingPrefabHeader && _o.HasBrefabButton)
              {
                  if (prefabOffset)
                  {
                      args[0] = _o.GetTreeItem();
                      var prefabRect = worldOffset;
                      prefabRect.width = 0;
                      args[1] = prefabRect;
                      adapter.PrefabModeButton.Invoke(adapter.gui_currentTree, args);
                  }
              }
              */



			if ( stateForDrag_B0.HasValue && contains && !objectIsHiddenAndLock )
			{
				if ( EditorGUIUtility.isProSkin )
					adapter.gl.DRAW_TAP_GLOW( inputRect );
				///Draw_GUITexture( inputRect, adapter.GetIcon( "SETUP_BUTTON_HOVER" ) );
				else
					adapter.gl.DRAW_TAP_GLOW( inputRect );
				// EditorGUI.DrawRect(inputRect, Color.white);

				///Draw_GUITexture( inputRect, Texture2D.whiteTexture );

				if ( o.activeInHierarchy != stateForDrag_B0.Value )
				{
					Undo.RecordObject( o, "GameObject SetActive" );
					stateForDrag_B0 = !o.activeInHierarchy;
					o.SetActive( !o.activeInHierarchy );
					/* Hierarchy.SetDirty(o);
                     Hierarchy.MarkSceneDirty(o.gameObject.scene);*/
				}

				if ( adapter.EVENT.isMouse ) Tools.EventUse();
			}

			if ( stateForDrag_B1 && contains && !objectIsHiddenAndLock )
			{
				if ( EditorGUIUtility.isProSkin )
					adapter.gl.DRAW_TAP_GLOW( inputRect );
				//Draw_GUITexture( inputRect, adapter.GetIcon( "SETUP_BUTTON_HOVER" ) );
				else
					adapter.gl.DRAW_TAP_GLOW( inputRect );
				// EditorGUI.DrawRect(inputRect, Color.white);
				//Draw_GUITexture( inputRect, Texture2D.whiteTexture );

				FRAME( o );

				if ( stateForDrag_B1 != o ) //SceneView.RepaintAll();
				{ }

				stateForDrag_B1 = o;


				if ( adapter.EVENT.isMouse ) Tools.EventUse();
			}

			if ( inputRect.Contains( adapter.EVENT.mousePosition ) )
			{
				if ( inputRect.Contains( adapter.EVENT.mousePosition )
					&& adapter.EVENT.type == EventType.MouseDown ) //EditorUtility.SetObjectEnabled( markedObjects[ instanceID ], !markedObjects[ instanceID ].activeInHierarchy );
				{
					if ( adapter.EVENT.button == 0 )
					{
						var targetO = new[] { o };
						var sel = adapter.ha.SELECTED_GAMEOBJECTS();

						if ( sel.Any( c => c.id ==
										   _o.id ) /*&& adapter.EVENT.control*/) // targetO = sel.Where(g => g.GetComponentsInParent<Transform>(true).Count(p => sel.Contains(p.gameObject)) == 1).Select(g => g.gameObject).ToArray();
						{
							targetO = Tools.GetOnlyTopObjects( sel ).Select( g => g.go ).ToArray();
							/*  for (int i = 0; i < targetArray.Count; i++)
                              {
                            
                              } */
						}

						if ( !objectIsHiddenAndLock )
						{
							if ( stateForDrag_B0 == null ) adapter.PUSH_ONMOUSEUP( 0, OnMouseUp );

							stateForDrag_B0 = !o.activeInHierarchy;

							foreach ( var gameObject in targetO )
							{
								Undo.RecordObject( gameObject, "GameObject SetActive" );
								gameObject.SetActive( stateForDrag_B0.Value );
								/*  Hierarchy.SetDirty(gameObject);
                                  Hierarchy.MarkSceneDirty(gameObject.scene);*/
							}
						}

						else
						{

							/*   if (objectIsHiddenAndLock)
					{
					   //EMX_TODO if you need the ability to turn off objects inside disabled you need to uncomment
					   // o.SetActive(!o.activeSelf);
					}
					else*/
							{
								var p = o.GetComponentsInParent<Transform>(true).FirstOrDefault(a =>
								!a.gameObject.activeInHierarchy && (a.transform.parent != null && a.transform.parent.gameObject.activeInHierarchy || a.transform.parent == null)
							);
								if ( p != null )
								{
									Tools.EventUse();
									Tools.TRY_PING_OBJECT( p.gameObject );
								}
							}


							// MonoBehaviour.print(p);
						}
					}

					if ( adapter.EVENT.button == 1 )
					{
						if ( !objectIsHiddenAndLock )
						{
							if ( SceneView.lastActiveSceneView == null ) //var pos = InputData.WidnwoRect(!callFromExternal(), adapter.EVENT.mousePosition, 128, 68, adapter );
							{
								var pos = new MousePos(adapter.EVENT.mousePosition, MousePos.Type.Input_128_68, /*!callFromExternal()*/ true, adapter);
								Windows.InputWindow.Init( pos, "", adapter.window, null, null, "Please open 'SceneView' window" );
							}

							else
							{
								if ( stateForDrag_B1 == null ) adapter.PUSH_ONMOUSEUP( 0, OnMouseUp );

								stateForDrag_B1 = o;
								//   SceneView.RepaintAll();
								FRAME( o );
							}
						}
					}
				}

				if ( adapter.EVENT.isMouse ) Tools.EventUse();
			}

			/*
                        if ( style == null )
                        {
                            style = new GUIStyle( adapter.button );
                            style.fontSize = 7;
                            style.padding.bottom = 0;
                            style.padding.top = 0;
                            style.alignment = TextAnchor.MiddleLeft;
                        }

                        style.fontSize = adapter.parLINE_HEIGHT > 18 ? 8 : 7;

                        if ( adapter.EVENT.type == EventType.Repaint )
                        {
                            style.Draw( inputRect, content, false, false, false, false );
                        }*/
		}





		bool OnMouseUp( Events.MouseRawUp.WantMouseLeaveType t )
		{
			if ( stateForDrag_B1 != null ) // Hierarchy.RepaintWindow();
			{ // SceneView.RepaintAll();
			}

			stateForDrag_B0 = null;
			stateForDrag_B1 = null;
			return true;
		}
	}
}
