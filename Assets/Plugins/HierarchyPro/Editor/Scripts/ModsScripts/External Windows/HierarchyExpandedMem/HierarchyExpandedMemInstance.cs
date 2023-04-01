using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace EMX.HierarchyPlugin.Editor.Mods
{

	class HierarchyExpandedMemInstance
	{

		static MemType cType = MemType.Hier;

		PluginInstance adapter { get { return Root.p[0]; } }


        internal  void SubscribeEditorInstance( AdditionalSubscriber sbs )
        {
            //sbs.ExternalMod_MenuItems
        }


        internal void DoHier(Rect line, ExternalDrawContainer controller, Scene scene)
		{


			var l = line;

			l.width -= (ExternalModStyles.LINE_HEIGHT_FOR_BUTTONS(line.height));
			var plus = l;
			plus.x += plus.width - 2;
			plus.width = ExternalModStyles.LINE_HEIGHT_FOR_BUTTONS(line.height) + 2;
			if (Event.current.type == EventType.MouseDown && plus.Contains(Event.current.mousePosition))
			{
				var capturedPlus = plus;
				controller.selection_button = 10;
				controller.selection_window = controller.tempRoot;
				controller.selection_action = (mouseUp, deltaTIme) =>
				{
					if (mouseUp && capturedPlus.Contains(Event.current.mousePosition)) //  var scene = Adapter.LastActiveScene;
					{
						DrawButtonsOld.Add_HierExpands(controller, scene);
					}

					return Event.current.delta.x == 0 && Event.current.delta.x == 0;
				}; // ACTION
			}

			//    plus.height -= 3;
			if (Event.current.type == EventType.Repaint)
			{
				adapter.STYLE_HIERSEL_BUTTON.Draw(plus, ExternalModStyles.plusContent, false, false, false,
					plus.Contains(Event.current.mousePosition) && controller.selection_button == 10);
			}

			GUI.Label(plus, ExternalModStyles.plusContentLabel);
			EditorGUIUtility.AddCursorRect(plus, MouseCursor.Link);


			l.width -= (ExternalModStyles.LINE_HEIGHT_FOR_BUTTONS(line.height) + 4);

			//l.width -= LINE_HEIGHT( controller.IS_MAIN ) - 2;
			//  plus.height += 3;
			// plus = l;
			plus.x -= plus.width;
			//l.x += LINE_HEIGHT( controller.IS_MAIN ) - 2;
			// plus.width = LINE_HEIGHT( controller.IS_MAIN );
			if (Event.current.type == EventType.MouseDown && plus.Contains(Event.current.mousePosition))
			{
				var capturedPlus = plus;
				controller.selection_button = 11;
				controller.selection_window = controller.tempRoot;
				controller.selection_action = (mouseUp, deltaTIme) =>
				{
					if (mouseUp && capturedPlus.Contains(Event.current.mousePosition))
					{
						Tools.SET_EXPAND_NULL(scene);
					}

					return Event.current.delta.x == 0 && Event.current.delta.x == 0;
				}; // ACTION
			}

			// plus.height -= 3;
			if (Event.current.type == EventType.Repaint)
			{
				adapter.STYLE_HIERSEL_PLUS.Draw(plus, ExternalModStyles.hierCollapce, false, false, false,
					plus.Contains(Event.current.mousePosition) && controller.selection_button == 11);
				/* Adapter.STYLE_LASTSEL_BUTTON.Draw( plus , ContentSelForw , false , false , false ,
                                             plus.Contains( Event.current.mousePosition ) && controller.selection_button == 11 );*/
			}

			GUI.Label(plus, ExternalModStyles.hierCollapceLabel);
			EditorGUIUtility.AddCursorRect(plus, MouseCursor.Link);


			//   wasSceneDraw = false;
			//  if (Event.current.type == EventType.Repaint)                    EditorStyles.helpBox.Draw(line, /* new GUIContent(""),*/ false, false, false, false);

			/*	if (!DrawButtons(l, LH, MemType.Hier, WHITE, controller, scene))
				{
					var tooltip = GETTOOLTIPPEDCONTENT(MemType.Hier, null, controller);
					tooltip.text = "";
					GUI.Label(l, tooltip);

					GUI.Label(l, "-");
				}*/

			line.width -= plus.width;
			line.width -= plus.width;
			var rowParams = DrawButtonsOld.GET_DISPLAY_PARAMS(cType);

			//if (!dob.DrawButtons( line, cType, rowParams.BgOverrideColor.a != 0 ? rowParams.BgOverrideColor : Color.white, controller, scene ) )
				if ( !dob.DrawButtons( line, cType, rowParams.BgOverrideColor, controller, scene))
			{
				var tooltip = DrawButtonsOld.GETTOOLTIPPEDCONTENT(cType, null, controller);
				tooltip.text = "";
				GUI.Label(line, tooltip);
				GUI.Label(l, "- - -", adapter.STYLE_LABEL_10_middle);
			}
		}
		internal DrawButtonsOld dob = new DrawButtonsOld();
    }
}
