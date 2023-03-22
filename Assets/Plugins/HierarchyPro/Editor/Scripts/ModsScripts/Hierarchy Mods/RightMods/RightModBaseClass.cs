using System;
using UnityEngine;
using UnityEditor;
using EMX.HierarchyPlugin.Editor.Mods;

namespace EMX.HierarchyPlugin.Editor
{

	internal interface ISearchable
	{
		bool callFromExternal();
		Windows.SearchWindow.FillterData_Inputs callFromExternal_objects { get; set; }
		Type typeFillter { get; set; }
		string SearchHelper { get; set; }
		float GetInputWidth();
		void DrawSearch( Rect rect, HierarchyObject o );
	}


	abstract class RightModBaseClass : DrawStackAdapter, ISearchable
	{

		//SEARCH
		public override bool callFromExternal() { return callFromExternal_objects != null; }
		public Windows.SearchWindow.FillterData_Inputs callFromExternal_objects { get; set; }
		public Type typeFillter { get; set; }
		public string SearchHelper { get { return _SearchHelper; } set { _SearchHelper = value; } }
		string _SearchHelper = "-";
		public virtual float GetInputWidth() { return -1; }
		public void DrawSearch( Rect rect, HierarchyObject o )
		{
			drawRect = rect;
			adapter.o = o;
			Draw();
		}
		//SEARCH
		internal Event EVENT { get { return callFromExternal() ? Event.current : adapter.EVENT; } }

		abstract internal void Subscribe( EditorSubscriber sbs );
		abstract internal void ModuleCommonGenericMenu( GenericMenu menu, GameObject o, object data, string sub );

		public abstract void Draw();
		internal Rect drawRect;

		internal PluginInstance adapter;
		internal string ContextHelper = "-";
		internal string HeaderText = "-";
		internal string HeaderTexture2D = null;
		internal Color? headOverrideTexture = null;
		//   internal bool ENABLE = false;

		internal override bool PERFOMANCE_BARS { get { return base.PERFOMANCE_BARS && !callFromExternal(); } }
		internal virtual object SetCustomModule( object customModule ) { throw new System.NotImplementedException(); }
		// internal virtual bool SKIP( ) { return false; }
		// internal virtual int STATIC_WIDTH( ) { return 0; }
		// internal virtual void STATIC_MENU( ) { }
		internal virtual bool enableOverride() { return true; }
		internal virtual string enableOverrideMessage() { return null; }
		internal virtual void InitializeModule() { }
		protected virtual void OnEnableChange( bool value ) { }


		internal abstract Windows.SearchWindow.FillterData_Inputs CallHeader();

		internal string MODULE_KEY {
			get {
				return GetType().Name;
				/*if (this is Mod_UserModulesRoot )
                {
                    var m = (Mod_UserModulesRoot)this;

                    return "Mod_UserModulesRoot" + 
                }*/
			}
		}

		[NonSerialized]
		internal ModSavedData savedData;

		internal RightModBaseClass( int def_width, int def_sibbildPos, bool def_enable, PluginInstance adapter ) : base( adapter.pluginID )
		{
			this.adapter = adapter;
			InitializeModule();
			savedData = new ModSavedData( MODULE_KEY, adapter, def_sibbildPos, def_width, def_enable );
			OnEnableChange( savedData.enabled );
		}



		internal virtual bool USE_CONTENT_SHRINKING()
		{
			return false;
		}


		internal GUIContent GetEmptyConten_ForceEmpty = new GUIContent();
		//internal GUIContent GetEmptyConten_ForceEmpty( RightModBaseClass mod )
		//{
		//	//__GetEmptyConten_ForceText.text = text;
		//	//__GetEmptyConten_ForceText.tooltip = mod.ContextHelper;
		//	return __GetEmptyConten_Common;
		//}
		internal GUIContent GetEmptyConten_Common = new GUIContent();
		//internal GUIContent GetEmptyConten_Common( RightModBaseClass mod )
		//{
		//	//__GetEmptyConten_Common.tooltip = mod.ContextHelper;
		//	return __GetEmptyConten_Common;
		//}





		static protected Color B_PASSIVE {
			get { return Colors.B_PASSIVE; }
		}

		static protected Color B_ACTIVE {
			get { return Colors.B_ACTIVE; }
		}


	}
}
