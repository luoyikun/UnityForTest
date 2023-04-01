#define DISABLE_PING

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace EMX.HierarchyPlugin.Editor.Mods
{



	internal partial class ComponentsIcons_Mod : DrawStackAdapter, ISearchable
    {








        void DrawIcon( Rect cellRect, Rect iconRect, HierarchyObject __o, ref DrawCompsStack data, bool allowHide, DrawCompsStack[] arr = null )
        // void DrawIcon(Component[] drawComps, HierarchyObject __o, Rect drawRect, Texture2D overImage, Type callbackType, bool allowHide, ref string MenuText)
        {

            // iconRect.width += 3;
            // iconRect.x -= 1;


            var o = __o.go;
            var col = data.hasCustomIcon ? data.customColor : Color.white;
            if ( !o.activeInHierarchy ) col.a *= 0.2f;



            //  Adapter.DrawTexture( r, context.image );
            var mayTransparent = data.isMono && !data.hasCustomIcon /*&& adapter.par_e.COMPONENTS_MONO_SPLIT_MODE == 2*/;
            var transparent = !adapter.par_e.COMPONENTS_DRAW_ICONS_MONO_BG && mayTransparent;

            if ( data.isMono && !data.hasCustomIcon )
            {
                iconRect.x -= 1;
                iconRect.y -= 1;
                iconRect.width += 2;
                iconRect.height += 2;
            }

            if ( adapter.par_e.COMPONENTS_DRAW_ICONS_SHADOW && !transparent
       //  || (__o.FLAGS & 1) != 0 && __o.BG_RECT.HasValue && __o.BG_RECT.Value.x + __o.BG_RECT.Value.width > r.x + r.width
       )
            {
                var S = 4;
                var R = iconRect;
                R.x -= S;
                R.y -= S;
                R.width += S * 2;
                R.height += S * 2;
                //   Adapter.DrawTexture( R, adapter.GetIcon( "HIPERUI_BUTTONGLOW" ), Color.black );
                Draw_GUITexture( R, adapter.GetNewIcon( NewIconTexture.MonoIcons, "HIPERUI_BUTTONGLOW" ), col * Color.black, _drawRect );
            }




            //  GUI.color = oldC;
            ///if (mayTransparent  )
            if ( arr != null || data.isMono && !data.hasCustomIcon )
            {

                var targetDic = transparent ? NewIconTexture.MonoIcons_Transparent : NewIconTexture.MonoIcons;

                if ( mono_display_type == 0 || arr != null && arr.Length > 1 )
                {
                    if ( arr == null || arr.Length == 1 )
                        Draw_GUITexture( iconRect, adapter.GetNewIcon( targetDic, "MONO_1" ), col, _drawRect );
                    else if ( arr.Length > 9 )
                        Draw_GUITexture( iconRect, adapter.GetNewIcon( targetDic, "MONO_-" ), col, _drawRect );
                    else
                        Draw_GUITexture( iconRect, adapter.GetNewIcon( targetDic, "MONO_" + arr.Length ), col, _drawRect );
                }
                else if ( mono_display_type == 1 )
                {

                    Draw_GUITexture( iconRect, adapter.GetNewIcon( targetDic, data.type_twocharsname[ 0 ].ToString() ), col, _drawRect );
                }
                else if ( mono_display_type == 2 )
                {
                    Draw_GUITexture( iconRect, adapter.GetNewIcon( targetDic, data.type_twocharsname ), col, _drawRect );
                }
            }
            else if ( data.hasCustomIcon ) Draw_GUITexture( iconRect, texture: Icons.GetIconDataFromTexture( data.customIcon ), guiColor: data.customColor * col, _drawRect );
            else Draw_GUITexture( iconRect, Icons.GetIconDataFromTexture( data.buildInIcon ), col, _drawRect );

            if ( arr != null )
            {
                var i = 0;
                for ( int j = 0; j < arr.Length; j++ ) if ( arr[ j ].hasEnable && !arr[ j ].enable ) i++;
                if ( i != 0 ) Draw_GUITexture( iconRect, adapter.GetNewIcon( NewIconTexture.MonoIcons, i == arr.Length ? "DISABLE" : "DISABLEHALF" ), null, _drawRect );
            }
            else
            {
                if ( data.hasEnable && !data.enable ) Draw_GUITexture( iconRect, adapter.GetNewIcon( NewIconTexture.MonoIcons, "DISABLE" ), null, _drawRect );
            }


            /*
			iconRect = drawRect;
			iconRect.x -= 1;
			iconRect.width += 2;
			iconRect.y -= 1;
			iconRect.height *= 0.75f;
			iconRect.height += 2;

			iconRect.y = firstRect.y;
			iconRect.height = firstRect.height;
			iconRect.width += 2;*/

            /* ------ */

            cellRect.x = iconRect.x;
            cellRect.width = iconRect.width;

            args.drawCompSingle = data.comp;
            // Draw_Action( cellRect, SET_ACTIVE_ACTION_HASH, args );

            /*    sendContent.text = "";
				sendContent.image = null;
				if ( drawComps.Length > 1 )  sendContent.tooltip = drawComps.Select( Adapter.GetTypeName ).Aggregate( ( a, b ) => a + '\n' + b );
				else sendContent.tooltip = drawComps[ 0 ].GetType().Name;*/

            if ( arr != null ) MakeContentFromArray( ref arr, sendContent );
            else sendContent.tooltip = data.type.Name;
            // args.drawCompsArr = drawComps.ToArray();
            args.drawCompSingle = data.comp;
            args.drawCompsArr = arr;
            args.callbackType = arr == null ? data.type : null;
            args.allowHide = allowHide;

            Draw_Action( cellRect, SET_ACTIVE_ACTION_HASH, args );

            // args.MenuText = MenuText;
            cellRect.width = GetClampedRect_Event( cellRect );


            // cellRect.width = Math.Min( cellRect.x + cellRect.width, adapter.fullLineRect.x + adapter.fullLineRect.width - adapter.rightOffset ) - cellRect.x;
            if ( cellRect.width > 0 ) Draw_ModuleButton( cellRect, sendContent, BUTTON_ACTION_HASH, true, args, true,
                drawPointer: adapter.par_e.COMPONENTS_CHANGE_MOUSE_CURSOR );
        }

     
		//DrawCompsStack[] emptyDrawCompStack = new DrawCompsStack[0];
		StringBuilder sb = new StringBuilder();
        void MakeContentFromArray( ref DrawCompsStack[] arr, GUIContent content )
        {
            sb.Clear();
            for ( int i = 0; i < arr.Length; i++ )
            {
                if ( i != 0 ) sb.AppendLine();
                sb.Append( arr[ i ].type.Name );
            }
            content.tooltip = sb.ToString();
        }

        ARGS args;
        GUIContent sendContent = new GUIContent();

        DrawStackMethodsWrapper __SET_ACTIVE_ACTION_HASH = null;

        DrawStackMethodsWrapper SET_ACTIVE_ACTION_HASH {
            get { return __SET_ACTIVE_ACTION_HASH ?? (__SET_ACTIVE_ACTION_HASH = new DrawStackMethodsWrapper( SET_ACTIVE_ACTION )); }
        }

        // int SET_ACTIVE_ACTION_HASH = "SET_ACTIVE_ACTION".GetHashCode();
        void SET_ACTIVE_ACTION( Rect worldOffset, Rect r, DrawStackMethodsWrapperData data, HierarchyObject _o )
        {
            if ( EVENT.rawType == EventType.MouseUp )
            {
                RawOnUP( Events.MouseRawUp.WantMouseLeaveType.MouseUp );
            }

            RawOnUpDragComponents( Events.MouseRawUp.WantMouseLeaveType.DoesNotClear );

            if ( EVENT.keyCode == KeyCode.Escape )
            {
                if ( RawOnUpDragComponents_Array != null ) RawOnUpDragComponents( Events.MouseRawUp.WantMouseLeaveType.Escape );
            }


            if ( EVENT.control && stateForDrag_B0.HasValue && stateForDrag_B1 == _o.go && EVENT.type == EventType.MouseDrag
                && !stateForDrag_B0.Value.Contains( EVENT.mousePosition ) ) //DragAndDrop.PrepareStartDrag();// reset data
            {
                adapter.ha.InternalClearDrag();


                if ( EVENT.shift )
                {
                    var targetList = new List<Component>();

                    foreach ( var item in stateForDrag_B2 )
                    {
                        try
                        {
                            var newC = GameObject.Instantiate(item); // o.AddComponent();
                            Tools.Copy( item );

                            if ( Tools.PastValidate( newC ) ) Tools.Paste( newC );

                            newC.gameObject.hideFlags = RawOnUpDragComponents_Flags;
                            Undo.RegisterCreatedObjectUndo( newC, "Move Component" );
                            Undo.RegisterCreatedObjectUndo( newC.gameObject, "Move Component" );
                            targetList.Add( newC );
                        }

                        catch { }
                    }

                    if ( RawOnUpDragComponents_Array != null ) RawOnUpDragComponents( Events.MouseRawUp.WantMouseLeaveType.DragOut );

                    RawOnUpDragComponents_Array = targetList.ToArray();
                    // DragAndDrop.SetGenericData( "MoveComp", RawOnUpDragComponents_Array );
                    stateForDrag_B2 = targetList.ToArray();
                    adapter.PUSH_ONMOUSEUP( 0, RawOnUpDragComponents );
                }
                //Debug.Log( "StartDrag " + stateForDrag_B2[ 0 ].name + " " + stateForDrag_B2.Length );
                DragAndDrop.AcceptDrag();
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = stateForDrag_B2.Where( g => g ).ToArray();
                DragAndDrop.paths = new string[ 0 ];
                //  DragAndDrop.paths = stateForDrag_B2. Select(c=>!c || !(c is MonoBehaviour) ? null : MonoScript.FromMonoBehaviour(c as MonoBehaviour )).Select
                DragAndDrop.SetGenericData( "Drag Component Module", this );
                // drawComps = emptyArr;
                DragAndDrop.StartDrag( "Drag Components" );
                // DragAndDrop.
                //  EventUse();
                adapter.RepaintWindowInUpdate( 0 );
                Tools.EventUse();
            }
            // Debug.Log(MouseClamped);
            if ( EVENT.type == EventType.MouseDown && r.width > 0 && r.Contains( EVENT.mousePosition ) && !MouseClamped )
            {
                if ( EVENT.button == 0 )
                {
                    if ( !stateForDrag_B0.HasValue ) adapter.PUSH_ONMOUSEUP( 0, RawOnUP );

                    stateForDrag_B0 = r;
                    stateForDrag_B1 = _o.go;
                    var args = (ARGS)data.args;
                    //  var drawComps = get_drawComps(_o.id);
                    var drawComps = args.drawCompsArr != null && args.drawCompsArr.Length > 0 ?
                        args.drawCompsArr.Where(c => c.comp).Select(c => c.comp).ToList() :
                        new[] {args.drawCompSingle }.ToList();
                    if ( drawComps.Count == 0 && args.drawCompSingle ) drawComps.Add( args.drawCompSingle );
                    stateForDrag_B2 = drawComps.ToArray();
                    //Debug.Log( "Assign " + drawComps[ 0 ].GetType().Name + " " + drawComps.Count );
                    //Debug.Log( "Assign " + stateForDrag_B2[ 0 ].name + " " + stateForDrag_B2.Length );

                }
            }
        }

        bool MouseClamped {
            get {
                if ( adapter.par_e.COMPONENTS_CLAMP_EVENTS ) return EVENT.mousePosition.x >= adapter.fullLineRect.x + adapter.fullLineRect.width - adapter.rightOffset;
                return EVENT.mousePosition.x >= adapter.fullLineRect.x + adapter.fullLineRect.width - adapter.ha.PREFAB_BUTTON_SIZE - adapter.modsController.setActiveMod.rawIW_hard + adapter.par_e.COMPONENTS_DISABLED_CLAMP_OFFEST;
            }
        }
		float GetClampedRect_Graphic( Rect cellRect )
		{
			if ( adapter.par_e.COMPONENTS_CLAMP_GRAPHIC ) cellRect.width = Math.Min( cellRect.x + cellRect.width, adapter.fullLineRect.x + adapter.fullLineRect.width - adapter.rightOffset ) - cellRect.x;
			else cellRect.width = Math.Min( cellRect.x + cellRect.width, adapter.fullLineRect.x + adapter.fullLineRect.width - adapter.ha.PREFAB_BUTTON_SIZE - adapter.modsController.setActiveMod.rawIW_hard + adapter.par_e.COMPONENTS_DISABLED_CLAMP_OFFEST ) - cellRect.x;
			// if ( adapter.par_e.RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER ) cellRect.width += adapter.par_e.RIGHT_RIGHT_PADDING;

			return cellRect.width;
		}

		float GetClampedRect_Event( Rect cellRect )
		{
			if ( adapter.par_e.COMPONENTS_CLAMP_GRAPHIC || adapter.par_e.COMPONENTS_CLAMP_EVENTS ) cellRect.width = Math.Min( cellRect.x + cellRect.width, adapter.fullLineRect.x + adapter.fullLineRect.width - adapter.rightOffset ) - cellRect.x;
			else cellRect.width = Math.Min( cellRect.x + cellRect.width, adapter.fullLineRect.x + adapter.fullLineRect.width - adapter.ha.PREFAB_BUTTON_SIZE - adapter.modsController.setActiveMod.rawIW_hard + adapter.par_e.COMPONENTS_DISABLED_CLAMP_OFFEST ) - cellRect.x;
			// if ( adapter.par_e.RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER ) cellRect.width += adapter.par_e.RIGHT_RIGHT_PADDING;

			return cellRect.width;
		}
	}
}






/*

#pragma warning disable
        internal class IconPosClass
        {
            internal float rightPos;
        }

        internal Dictionary<int, IconPosClass> iconRightPosition = new Dictionary<int, IconPosClass>();

        void PutIconPos(int id, float rightPos)
        {
            return;

            if (!iconRightPosition.ContainsKey(id)) iconRightPosition.Add(id, new IconPosClass() {rightPos = rightPos});
            else
            {
                if (iconRightPosition[id].rightPos < rightPos) iconRightPosition[id].rightPos = rightPos;
            }
        }

        void ClearIconPos(int id)
        { 
            return;

            if (!iconRightPosition.ContainsKey(id)) iconRightPosition.Add(id, new IconPosClass() {rightPos = -1});
            else
            {
                iconRightPosition[id].rightPos = -1;
            }
        }

        internal float GetIconPos(int id)
        {
            return -1;

            if (!iconRightPosition.ContainsKey(id)) return -1;

            return iconRightPosition[id].rightPos;
        }
#pragma warning restore*/





/*  if ( hasText )
  {
      var n = callbackType.Name;


      labelStyleWhite.fontSize = drawBg ? 10 : 8;
      labelStyle.fontSize = drawBg ? 10 : 8;

      if ( n.Length != 0 )
      { //labelStyle.Draw( r, n[0].ToString(), false, false, false, false );
          var texxt = n[0].ToString().ToUpper();

          if ( n.Length > 1 && !drawBg && adapter.par_e.COMPONENTS_DRAW_SECOND_CHAR_FOR_MONO ) texxt += "<size=" + (labelStyle.fontSize - 1) + ">" + n[ 1 ].ToString().ToLower() + "</size>";

          if ( drawBg ) Draw_Style( r, o.activeInHierarchy ? labelStyle : labelStyleWhite, texxt );
          else Draw_Style( r,  labelStyle, texxt, USE_GO: true );
      }
  }*/
