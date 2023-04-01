using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace EMX.HierarchyPlugin.Editor.Events
{
	partial class HierarchyActions
	{
		int pluginID;
		//EditorWindow window { get { return Root.p[0].window.Instance; } }
		// PluginInstance p { get { return Root.p[pluginID]; } }
		internal HierarchyActions( int pId )
		{
			pluginID = pId;

			if ( pluginID == 1 )
			{
				m_SearchFilterString = Root.p[ 0 ].SceneHierarchyWindow( 1 ).GetField( "m_SearchFilter", ~(BindingFlags.Static | BindingFlags.InvokeMethod) );
				m_SearchFilterClass_Has = m_SearchFilterString.FieldType.GetMethod( "IsSearching", ~(BindingFlags.Static | BindingFlags.SetField) );
			}
			if ( pluginID == 0 )
			{
				SearchableWindowType = Assembly.GetAssembly( typeof( EditorWindow ) ).GetType( "UnityEditor.SearchableEditorWindow" );
				m_SearchFilterString = SearchableWindowType.GetField( "m_SearchFilter", ~(BindingFlags.Static | BindingFlags.InvokeMethod) );
#if !UNITY_2020_1_OR_NEWER
                showingPrefabHeader = Root.p[ 0 ].SceneHierarchyWindowRoot.GetProperty( "showingPrefabHeader", ~(BindingFlags.Static | BindingFlags.InvokeMethod) );
                hasShowingPrefabHeader = showingPrefabHeader != null;
#else
				var GetCurrentPrefabState_Type = typeof(UnityEditor.PrefabUtility).Assembly.GetType("UnityEditor.Experimental.SceneManagement.PrefabStageUtility")??
					typeof(UnityEditor.PrefabUtility).Assembly.GetType("UnityEditor.SceneManagement.PrefabStageUtility");
				GetCurrentPrefabState = GetCurrentPrefabState_Type.GetMethod( "GetCurrentPrefabStage", ~(BindingFlags.Instance | BindingFlags.GetProperty) );
				hasShowingPrefabHeader = true;
#endif


			}

		}







		//SearchFilter
		bool? seach_baked;
		FieldInfo m_SearchFilterString;
		MethodInfo m_SearchFilterClass_Has;
#if !UNITY_2020_1_OR_NEWER
        PropertyInfo showingPrefabHeader;
#else
		MethodInfo GetCurrentPrefabState;
#endif
		Type SearchableWindowType;
		internal bool hasShowingPrefabHeader;

		internal void BAKE_SEARCH()
		{
			if ( m_SearchFilterString == null )
			{
				seach_baked = false;
				return;
			}

			if ( pluginID == 0 )
			{
				seach_baked = !string.IsNullOrEmpty( (string)m_SearchFilterString.GetValue( Root.p[ 0 ].window.Instance ) );
				return;
			}


			var sr = Root.p[0].m_SearchFilter.GetValue(Root.p[0].window.Instance);
			//var nowSearch;
			if ( sr != null )
			{
				seach_baked = (bool)Root.p[ 0 ].IsSearching.Invoke( sr, null );
			}
			else
			{
				seach_baked = false;
			}

			//seach_baked = (bool)m_SearchFilterClass_Has.Invoke(m_SearchFilterString.GetValue(Root.p[0].window), null);
		}


		internal bool IS_SEARCH_MOD_OPENED()
		{
			// if ( ChechButton( _S_HideBttomIfNoFunction ) ) return true;

			if ( !seach_baked.HasValue ) BAKE_SEARCH();

			return seach_baked.Value;
		}

		internal bool IS_PREFAB_MOD_OPENED()
		{
			if ( pluginID != 0 ) return false;
#if UNITY_2020_1_OR_NEWER
			// return UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
			if ( !ReferenceEquals( GetCurrentPrefabState.Invoke( null, null ), null ) ) return true;
#else
			if (hasShowingPrefabHeader && showingPrefabHeader.GetValue(Root.p[0].window.Instance, null).Equals(true)) return true;
#endif
			return false;
		}


		// bool? oldIS_SEARCH_MODE_OR_PREFAB_OPENED;

		// internal bool IS_SEARCH_MODE_OR_PREFAB_OPENED()
		// {
		//     var res = __IS_SEARCH_MODE_OR_PREFAB_OPENED();
		//     // if ( !oldIS_SEARCH_MODE_OR_PREFAB_OPENED.HasValue ) oldIS_SEARCH_MODE_OR_PREFAB_OPENED = res;
		//     // if ( oldIS_SEARCH_MODE_OR_PREFAB_OPENED.Value != res ) RedrawInit = true;
		//     return res;
		// }

		internal bool IS_SEARCH_MODE_OR_PREFAB_OPENED()
		{ // return false;

			if ( pluginID == 0 )
			{
#if UNITY_2020_1_OR_NEWER
				if ( !ReferenceEquals( GetCurrentPrefabState.Invoke( null, null ), null ) ) return true;
#else
                if ( hasShowingPrefabHeader && showingPrefabHeader.GetValue( Root.p[ 0 ].window.Instance, null ).Equals( true ) ) return true;
#endif
			}
			if ( !seach_baked.HasValue ) BAKE_SEARCH();

			return seach_baked.Value;
		}

		object inst, em;
		MethodInfo meth;
		List<Type> AllTypesOfIRepository;

		internal void CLOSE_PREFAB_MODE()
		{

			//Debug.Log(PrefabStageUtility.GetCurrentPrefabStage());
#if !UNITY_2020_1_OR_NEWER //2021

            if ( !hasShowingPrefabHeader) return;
			if (showingPrefabHeader == null) return;
#endif

			var w = Root.p[0].lastHierarchyWindw;
			if ( w == null ) return;
			if ( !w.Instance ) return;
			if ( !IS_PREFAB_MOD_OPENED() ) return;


			if ( AllTypesOfIRepository == null )
				AllTypesOfIRepository = (from x in Assembly.GetAssembly( typeof( EditorWindow ) ).GetTypes()
										 let y = x.BaseType
										 where !x.IsAbstract && !x.IsInterface &&
											   y != null && y.IsGenericType &&
											   y.GetGenericTypeDefinition() == typeof( ScriptableSingleton<> )
										 select x
					).ToList();

			/*GUI_ONESHOTPUSH( () =>
            {   UnityEditor.SceneManagement.StageUtility.GoBackToPreviousStage();
            } );*/
			Action closPref = () => {
				foreach ( var asd in AllTypesOfIRepository )
				{
					if ( asd.Name == "StageNavigationManager" )
					{
						if ( meth == null )
						{
							inst = asd.BaseType.GetProperty( "instance", (BindingFlags)(-1) ).GetValue( null, null );
							meth = inst.GetType().GetMethod( "NavigateBack", (BindingFlags)(-1) );
							em = Enum.Parse( meth.GetParameters()[ 0 ].ParameterType, "NavigateBackViaHierarchyHeaderLeftArrow" ); //NavigateBackViaUnknown  NavigateBackViaHierarchyHeaderLeftArrow NavigateViaBreadcrumb
						}

						meth.Invoke( inst, new[] { em } );
					}
				}
			};
			if ( !Root.p[ 0 ].PUSH_GUI_ONESHOT( 0, closPref ))closPref();
			Tools.EventUseFast();

			// SendEventAll( new Event() { type = EventType.Layout, mousePosition = Vector2.zero, button = 0 } );
		}
















		internal Color? hoveredBackgroundColor;
		static internal Type gs, ls;
		// internal FieldInfo __m_AssetTreeState, __m_FolderTreeState;

		object GetValue_Field( Type type, string field )
		{
			var res = type.GetField(field, (BindingFlags)(-1));

			if ( res == null ) return null;

			return res.GetValue( null );
		}

		void SetValue_Field( Type type, string field, object value )
		{
			var res = type.GetField(field, (BindingFlags)(-1));

			if ( res == null ) return;

			res.SetValue( null, value );
		}

		internal void FixStyle( GUIStyle style )
		{
			style.fixedHeight = 0;
			style.stretchHeight = true;
			style.alignment = TextAnchor.MiddleLeft;
			style.padding.top = 0;
			style.padding.bottom = 0;
			style.margin.top = 0;
			style.margin.bottom = 0;
			style.overflow.top = 0;
			style.overflow.bottom = 0;
		}

		bool TryToInitializeDefaultStylesWasInit;
		internal class internalGuiTyle
		{
			internal internalGuiTyle( GUIStyle style )
			{
				this.style = style;
			}
			internal GUIStyle style;
			internal bool setHeight;
			internal void SetHeight( int height )
			{
				if ( setHeight ) style.fixedHeight = height;
			}
		}
		internal int currentDefaultFontSize = 12;
		internal List<internalGuiTyle> INTERNAL_LABEL_STYLES = new List<internalGuiTyle>();
		Color? backBG;
		bool currentDisabled;
		internal void UpdateBGHover()
		{
			if ( pluginID != 0 ) return;
			if ( !Root.p[ 0 ]._hashoveredItem ) return;

			if ( !Root.p[ 0 ].WIN_SET.HIDE_HOVER_BG )
			{
				if ( currentDisabled )
				{
					currentDisabled = false;
					SetValue_Field( gs, "hoveredBackgroundColor", backBG.Value );
					hoveredBackgroundColor = backBG.Value;
				}
				return;
			}
			DisableHover();

			if ( currentDisabled ) return;
			currentDisabled = true;

			var v = GetValue_Field(gs, "hoveredBackgroundColor");
			if ( v == null ) return;
			if ( !backBG.HasValue ) backBG = (Color)v;

			if ( Root.p[ 0 ].WIN_SET.HIDE_HOVER_BG )
			{
				SetValue_Field( gs, "hoveredBackgroundColor", Color.clear );
				hoveredBackgroundColor = Color.clear;
			}

			else
			{
				SetValue_Field( gs, "hoveredBackgroundColor", backBG.Value );
				hoveredBackgroundColor = backBG.Value;
			}
			/*  var v = GetValue_Field(gs, "hoveredBackgroundColor");

			  if ( v == null ) return;

			  if ( !backBG.HasValue ) backBG = (Color)v;


			  if ( p.par_e.HIDE_HOVER_BG )
			  {
				  SetValue_Field( gs, "hoveredBackgroundColor", Color.clear );
				  hoveredBackgroundColor = Color.clear;
			  }

			  else
			  {
				  SetValue_Field( gs, "hoveredBackgroundColor", backBG.Value );
				  hoveredBackgroundColor = backBG.Value;
			  }*/
		}

		internal GUIStyle prebapButtonStyle, hoveredItemBackgroundStyle;
		internal GUIStyle lineStyle;
		// GUIStyle sceneHeaderBg;
		internal void TryToInitializeDefaultStyles()
		{
			if ( TryToInitializeDefaultStylesWasInit ) return;

			TryToInitializeDefaultStylesWasInit = true;

			if ( gs == null ) gs = typeof( EditorWindow ).Assembly.GetType( "UnityEditor.GameObjectTreeViewGUI+GameObjectStyles" ) ?? throw new Exception( "UnityEditor.GameObjectTreeViewGUI+GameObjectStyles" );
			INTERNAL_LABEL_STYLES.Clear();

			if ( gs != null )
			{
				/*foreach ( var m in typeof( EditorWindow ).Assembly.GetTypes() )
                 {   if (m.Name.Contains( "GameObjectStyles" ) )
                         Debug.Log( m.FullName );

                 }*/
				/*  gs.get
                  if ( gs != null ) {*/
				//UnityEditor.GameObjectTreeViewGUI.GameObjectStyles

				if ( ls == null ) ls = typeof( EditorWindow ).Assembly.GetType( "UnityEditor.IMGUI.Controls.TreeViewGUI+Styles" );

				if ( ls != null )
				{
					var l = GetValue_Field(ls, "lineStyle") as GUIStyle;

					if ( l != null )
					{
						lineStyle = l;
						FixStyle( l );
						INTERNAL_LABEL_STYLES.Add( new internalGuiTyle( l ) );
					}
				}

				if ( ls != null )
				{
					var l = GetValue_Field(ls, "lineBoldStyle") as GUIStyle;

					if ( l != null )
					{
						FixStyle( l );
						INTERNAL_LABEL_STYLES.Add( new internalGuiTyle( l ) );
					}
				}



				var disabledLabel = GetValue_Field(gs, "disabledLabel") as GUIStyle;

				if ( disabledLabel != null )
				{
					FixStyle( disabledLabel );
					if ( UnityVersion.UNITY_CURRENT_VERSION >= UnityVersion.UNITY_2019_VERSION && UnityVersion.UNITY_CURRENT_VERSION < UnityVersion.UNITY_2019_3_0_VERSION ) disabledLabel.padding.bottom = 2;

					INTERNAL_LABEL_STYLES.Add( new internalGuiTyle( disabledLabel ) );

				}

				if ( pluginID == 0 )
				{
					var sceneHeaderBg = GetValue_Field(gs, "sceneHeaderBg") as GUIStyle;

					if ( sceneHeaderBg != null )
					{
						FixStyle( sceneHeaderBg );
						if ( UnityVersion.UNITY_CURRENT_VERSION >= UnityVersion.UNITY_2019_VERSION && UnityVersion.UNITY_CURRENT_VERSION < UnityVersion.UNITY_2019_3_0_VERSION ) sceneHeaderBg.padding.bottom = 2;
						INTERNAL_LABEL_STYLES.Add( new internalGuiTyle( sceneHeaderBg ) { setHeight = true } );
					}





					var prefabLabel = GetValue_Field(gs, "prefabLabel") as GUIStyle;
					var disabledPrefabLabel = GetValue_Field(gs, "disabledPrefabLabel") as GUIStyle;
					var brokenPrefabLabel = GetValue_Field(gs, "brokenPrefabLabel") as GUIStyle;
					var disabledBrokenPrefabLabel = GetValue_Field(gs, "disabledBrokenPrefabLabel") as GUIStyle;

					// sceneHeaderBg = GetValue_Field(gs, "sceneHeaderBg" ) as GUIStyle;
					if ( prefabLabel != null )
					{
						FixStyle( prefabLabel );
						INTERNAL_LABEL_STYLES.Add( new internalGuiTyle( prefabLabel ) );
					}

					if ( disabledPrefabLabel != null )
					{
						FixStyle( disabledPrefabLabel );
						INTERNAL_LABEL_STYLES.Add( new internalGuiTyle( disabledPrefabLabel ) );
					}

					if ( brokenPrefabLabel != null )
					{
						FixStyle( brokenPrefabLabel );
						INTERNAL_LABEL_STYLES.Add( new internalGuiTyle( brokenPrefabLabel ) );
					}

					if ( disabledBrokenPrefabLabel != null )
					{
						FixStyle( disabledBrokenPrefabLabel );
						INTERNAL_LABEL_STYLES.Add( new internalGuiTyle( disabledBrokenPrefabLabel ) );
					}


					if ( hasShowingPrefabHeader )
					{
						prebapButtonStyle = GetValue_Field( gs, "rightArrow" ) as GUIStyle;
						prebapButtonStyle.margin.right = 0;
					}

					if ( Root.p[ 0 ].hashoveredItem )
					{
						hoveredItemBackgroundStyle = GetValue_Field( gs, "hoveredItemBackgroundStyle" ) as GUIStyle;
					}
				}

				//  }
			}
			else
			{
				throw new Exception( "Cannot read styles" );
			}



			var ns = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneVisibilityHierarchyGUI+Styles") ?? throw new Exception("UnityEditor.SceneVisibilityHierarchyGUI+Styles");

			if ( ns != null )
			{
				var vicContent = GetValue_Field(ns, "sceneVisibilityStyle") as GUIStyle;

				if ( vicContent != null ) FixStyle( vicContent );

				INTERNAL_LABEL_STYLES.Add( new internalGuiTyle( vicContent ) );
			}



#if UNITY_2019_3_OR_NEWER
			//#if !UNITY_2021_1_OR_NEWER
			{//PING

				var style = UPDATE_PING_STYLE();
				//m_Ping.SetValue(Root.p[0].gui_currentTree, style);

				INTERNAL_LABEL_STYLES.Add( new internalGuiTyle( style ) { setHeight = true } );
			}
			//#endif
#endif

			/*  gs =  typeof( EditorWindow ).Assembly.GetType( "UnityEditor.PrefabUtility+GameObjectStyles" );
              if ( gs != null )
              {

                  var prefabLabel =  GetValue_Field(gs, "prefabLabel") as GUIStyle;
                  var disabledPrefabLabel = GetValue_Field( gs, "disabledPrefabLabel") as GUIStyle;
                  var brokenPrefabLabel = GetValue_Field(gs, "brokenPrefabLabel") as GUIStyle;
                  var disabledBrokenPrefabLabel = GetValue_Field(gs, "disabledBrokenPrefabLabel") as GUIStyle;
                  if ( prefabLabel != null ) FixStyle( prefabLabel );
                  if ( disabledPrefabLabel != null ) FixStyle( disabledPrefabLabel );
                  if ( brokenPrefabLabel != null ) FixStyle( brokenPrefabLabel );
                  if ( disabledBrokenPrefabLabel != null ) FixStyle( disabledBrokenPrefabLabel );
                  //  }
              }*/
		}

		PropertyInfo pingStyle;
		internal GUIStyle UPDATE_PING_STYLE()
		{
#if UNITY_2019_3_OR_NEWER
			if ( pingStyle == null )
			{
				var GameObjectTreeViewGUI = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameObjectTreeViewGUI") ?? throw new Exception("UnityEditor.GameObjectTreeViewGUI");
				var treeViewBaseType = GameObjectTreeViewGUI.BaseType;

				pingStyle = treeViewBaseType.GetProperty( "pingStyle", ~(BindingFlags.Static | BindingFlags.InvokeMethod) ) ?? throw new Exception( "pingStyle" );
			}

			var style = pingStyle.GetValue(Root.p[0].gui_currentTree, null) as GUIStyle;
			FixStyle( style );
			style.fixedHeight = Root.p[ 0 ].WIN_SET_G( pluginID ).LINE_HEIGHT;
			pingStyle.SetValue( Root.p[ 0 ].gui_currentTree, style, null );

			return style;
#else
			return null;
#endif

		}




		float? _TOTAL_LEFT_PADDING;
		internal float LEFT_PADDING {
			get {
				if ( pluginID != 0 ) return 0;

				if ( !_TOTAL_LEFT_PADDING.HasValue )
				{
					if ( UnityVersion.UNITY_CURRENT_VERSION >= UnityVersion.UNITY_2019_VERSION )
					{
						_TOTAL_LEFT_PADDING =
							(float)Root.p[ 0 ].SceneHierarchyWindowRoot.Assembly.GetType( "UnityEditor.SceneVisibilityHierarchyGUI" ).GetField( "utilityBarWidth", (BindingFlags)(-1) )
								.GetValue( null );
						_TOTAL_LEFT_PADDING = _TOTAL_LEFT_PADDING.Value;
					}
					else _TOTAL_LEFT_PADDING = 0;
				}
				return _TOTAL_LEFT_PADDING.Value;
			}
		}

		internal float PREFAB_BUTTON_SIZE {
			get { //if ( UNITY_CURRENT_VERSION >= UNITY_2019_1_1_VERSION ) return 0;

				return pluginID == 0 ? Tools.singleLineHeight : 0;
			}
		}
	}
}
