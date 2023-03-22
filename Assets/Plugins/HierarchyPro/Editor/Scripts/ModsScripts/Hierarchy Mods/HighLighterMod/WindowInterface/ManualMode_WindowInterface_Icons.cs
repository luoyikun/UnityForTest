using System;
using UnityEngine;
using UnityEditor;
using EMX.HierarchyPlugin.Editor.Mods;


namespace EMX.HierarchyPlugin.Editor.Windows
{
	public partial class Root_HighlighterWindowInterface : Windows.IWindow
	{


		float PADDD = 5;
		void DRAWICON(Rect inputrect)
		{

			if (source == null || !source.Validate()) return;

			inputrect.width -= 5;


			/*  inputrect.x += PADDD;
			  inputrect.width -= PADDD * 2;*/

			/* inputrect.height = EditorGUIUtility.singleLineHeight + 4;
			 GUI.Box(inputrect, "");
			 Label(Shrink(inputrect, 2), "Use Icon");
			 inputrect.y += inputrect.height + 5;*/

			var OFFSET_X = 60;
			/* var R_CLOSE = new Rect(inputrect.x, inputrect.y + (LINE_H - 10) / 2, 10, 10);
			 var R_LABEL = new Rect(inputrect.x + LINE_H, inputrect.y, 48, LINE_H);*/
			var R_CLOSE = new Rect(inputrect.x + OFFSET_X - 14, inputrect.y + (LINE_H - 14) / 2, 14, 14);
			var R_LABEL = new Rect(inputrect.x, inputrect.y, 48, LINE_H);
			R_LABEL.height *= 2;
			var R_COLOR = new Rect(R_LABEL.x + R_LABEL.width + R_LABEL.x + OFFSET_X, inputrect.y + (LINE_H - 23) / 2, 55, 23);
			R_COLOR.height *= 2;
			// var R_LAST = new Rect(R_COLOR.x + R_COLOR.width, inputrect.y, 134, LINE_H);
			var R_CHILD = new Rect(R_COLOR.x + R_COLOR.width, inputrect.y, 0, LINE_H);
			R_CHILD.width = inputrect.x + inputrect.width - R_CHILD.x;
			R_CHILD.x += 3;
			R_CHILD.y += 3;
			R_CHILD.width -= 6;
			R_CHILD.height -= 6;



			var im = HighlighterCache_Icons.GetImageForObject_OnlyCacher(source,true, mod).add_icon;
			if (im != null)
			{
				GUI.DrawTexture(R_CLOSE, Root.p[0].GetExternalModOld(  "HIPERUI_CLOSE"));
				if (Button(R_CLOSE, ""))
				{
					SetIconImage(null, "Set Icon");
					REPAINT_ALL_HIERW();
					// InternalEditorUtility.RepaintAllViews();
					TryToClose();
#if CLOSE_AFTERICON
                CloseThis();
#endif
				}


				HighlighterCache_Icons.CheckIconIfAlreadyHas(source, im, mod);
			}

			//var newEn = EditorGUI.ToggleLeft(R_LABEL, "Background Color", en);

			var refCol = Color.white;
			//* ICON */
			if (im != null )
			{

				R_COLOR.y += (R_COLOR.height - 23)/2;
				R_COLOR.height = 23;

				R_COLOR.x += 10;
				GUI.Label(R_COLOR, "Color:");
				R_COLOR.x += R_COLOR.width + 20;
				//R_COLOR.y += R_COLOR.height;


				var getcolor = GetIconColor();
				refCol = getcolor ?? Color.white;


				emptyContent.tooltip = "Icon Tint Color";

				RESTORE_GUI();
				var newCol = PICKER(R_COLOR, emptyContent, refCol);
				CHANGE_GUI();

				if (newCol != refCol)
				{   /*var new32 = (Color32)newCol;
                if (new32.a == 0 && (new32.r != 0 || new32.g != 0 || new32.b != 0)) new32.a = 255;
                SetIconColor( new SingleList() { list = new[] { (int)new32.r, new32.g, new32.b, new32.a } .ToList() } );*/
					SetIconColor(newCol, "Change Icon Color");

					/*  var new32 = (Color32)newCol;
					  if (new32.a == 0 && (new32.r != 0 || new32.g != 0 || new32.b != 0)) new32.a = 255;
					  getcolor.list[0] = new32.r;
					  getcolor.list[1] = new32.g;
					  getcolor.list[2] = new32.b;
					  getcolor.list[3] = new32.a;
					  if (new32.r != 0 || new32.g != 0 || new32.b != 0 || new32.a != 0) UpdateLastHiglightColors(new32);
					  SetHiglightColor(getcolor);*/
				}
			}
			else
			{
				var en = GUI.enabled;
				GUI.enabled = false;
				RESTORE_GUI();
				emptyContent.tooltip = "No icon";
				PICKER(R_COLOR, emptyContent, Color.white);
				CHANGE_GUI();
				GUI.enabled = en;
			}
			RESTORE_GUI();

			var TR = R_LABEL;
			//  TR.x += R_CLOSE.width + 3;
			TR.x += OFFSET_X;
			TR.width = TR.height;
			if (im != null)
			{
				var asdasd = GUI.color;
				var cc = refCol;
				cc.a = (byte)Mathf.RoundToInt(Mathf.Clamp01(cc.a / 255f / 1.5f + 0.33f) * 255);
				if (Event.current.type == EventType.Repaint) GUI.skin.textArea.Draw(TR, "", false, false, false, false);
				GUI.color *= cc;

				GUI.DrawTexture(Shrink(TR, 4), im, ScaleMode.ScaleToFit);
				GUI.color = asdasd;
			}
			else
			{
				var en = GUI.enabled;
				GUI.enabled = false;
				// TR.y += 10;
				// Label(Shrink(TR, 4), "User\nIcon" );
				GUI.Box(Shrink(TR, 4), "User\nIcon", PluginInstance.STYLE_DEFBOX);
				GUI.enabled = en;
			}

			/*  R_COLOR.x += R_COLOR.width + 10;
			  Label(R_COLOR, "Use Icon");*/



			R_LABEL.y += R_LABEL.height + 5;


			var ll = R_LABEL;
			ll.width = inputrect.width;
			ll.height = singleLineHeight;

			var nv = 1;


			// string[]  cats = new [] { "External Texture2D Icons", "Internal Unity Icons" };
			//if (!adapter.IS_PROJECT())
			{   //cats = new [] {  "External Texture2D Icons"};
				string[] cats = new[] { "Internal Unity Icons", "External Texture2D Icons" };
				var ov = SessionState.GetInt("EMX|IconCat", 0);
				ov = Mathf.Clamp(ov, 0, cats.Length - 1);
				nv = GUI.Toolbar(ll, ov, cats, EditorStyles.toolbarButton);
				if (ov != nv)
				{
					SessionState.SetInt("EMX|IconCat", nv);
				}
				R_LABEL.y += ll.height + 5;
			}




			if (nv == 1)
			{   /*  var ll = R_LABEL;
            ll.width = inputrect.width;
            ll.height = EditorGUIUtility.singleLineHeight;
            Label(ll, "External Icons:");
            R_LABEL.y += EditorGUIUtility.singleLineHeight;*/


				//   R_LABEL.height = inputrect.height * 2;
				R_LABEL.width = R_CHILD.x - R_LABEL.x - 6;
				EditorGUIUtility.AddCursorRect(R_LABEL, MouseCursor.Link);



				if (GUI.Button(R_LABEL, "Other..."))
				{
					Texture2D s = im;
					if (current.StartsWith("GUID="))
					{
						var path = AssetDatabase.GUIDToAssetPath(current.Substring(5));
						if (path != null) s = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
					}
					conrollerID = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
					pickerId = conrollerID;
					EditorGUIUtility.ShowObjectPicker<Texture2D>(s, false, "", conrollerID);

					pickerAc = (t) =>
					{
						if (s != t) SetIconImage((Texture2D)t, "Set Icon");
					// Hierarchy.RepaintAllViews();
					REPAINT_ALL_HIERW();
					// InternalEditorUtility.RepaintAllViews();
					TryToClose();
#if CLOSE_AFTERICON
                    CloseThis();
#endif
				};
				}
				CHANGE_GUI();

				//* LAST *//
				//  DRAW_LAST_ICONS( new Rect( R_CHILD.x, R_CHILD.y + R_CHILD.height / 2 + 3, R_CHILD.width, R_CHILD.height / 2 - 3 ) );
				var lr = R_LABEL;
				lr.x += lr.width + 10;
				lr.width = inputrect.width - lr.x + 10;
				DRAW_LAST_ICONS(lr);
				//* LAST *//
				//* ICON *//
			}
			else
			{

				CHANGE_GUI();

				//  R_LABEL.x += R_LABEL.width;
				// R_LABEL.width = R_CHILD.width;
				//  R_LABEL.y += R_LABEL.height;
				R_LABEL.width = inputrect.width;
				/*   var asdd = R_LABEL;
				   asdd.height = EditorGUIUtility.singleLineHeight;
				   Label(asdd, "Internal Icons:");
				   R_LABEL.y += EditorGUIUtility.singleLineHeight;*/

				//* DEFAULT *//

				//if (adapter.IS_HIERARCHY())
				{   // Adapter. INTERNAL_BOX( R_LABEL, "" );
					// DRAW_DEFAULT_ICONS( new Rect( R_CHILD.x, R_CHILD.y, R_CHILD.width, R_CHILD.height / 2 - 3 ) );
					//  R_LABEL.height += 20;
					if (Event.current.type == EventType.Repaint) GUI.skin.textArea.Draw(R_LABEL, "", false, false, false, false);
					R_LABEL.x += 20;
					R_LABEL.width -= 20;
					DRAW_DEFAULT_ICONS(R_LABEL);

				}
			}


			//* DEFAULT *//





		}


		const int B = 2;

		void DRAW_DEFAULT_ICONS(Rect inputrect)
		{   /*  inputrect.x += 2;
          inputrect.y += 2;
          inputrect.width -= 4;
          inputrect.height -= 4;*/
			var PAD = 5;
			var GROUP_RECT = new Rect(inputrect.x + 5, inputrect.y, inputrect.width / 4 - PAD, inputrect.height * 4);
			// var w10 = inputrect.height / 2 - PAD;
			var w10 = GROUP_RECT.width / 4.75f - 1;
			var wH = (inputrect.height - 5) / 2;
			GROUP_RECT.width *= 2;
			//  ICON_RECT.height = w10 / 1.5f;
			var IHH = w10 / 1.1f;
			var d = (wH - w10 / 1.5f) / 2;

			if (HighlighterCache_Icons.labelIcons != null && HighlighterCache_Icons.labelIcons.Length >= 8)
			{   // GUI.BeginVertical();//label
				//  var GROUP_RECT = new Rect(lastRect.x + PAD, lastRect.y + PAD, 4 * w10, lastRect.height);
				GUI.BeginGroup(GROUP_RECT);
				var ICON_RECT = GROUP_RECT;
				//  ICON_RECT.y +=8;
				ICON_RECT.width = w10;
				ICON_RECT.width *= 2.5f;
				ICON_RECT.height = IHH;
				for (int y = 0; y < 2; y++)
				{   //  GUI.BeginHorizontal();//label
					for (int x = 0; x < 4; x++)
					{   /*var bg = Adapter.GET_SKIN().box.normal.background;
                    Adapter.GET_SKIN().box.normal.background = (Texture2D)Adapter.M_Colors.labelIcons[x + y * 4].image;
                    
                    Adapter. INTERNAL_BOX( ICON_RECT, "" );
                    Adapter.GET_SKIN().box.normal.background = bg;*/

						ICON_RECT.x = x * w10 * 2 + 2;
						ICON_RECT.y = y * wH + d;
						/*   labelStyle.border =  new RectOffset(B, B, B, B );
						   labelStyle.normal.background = (Texture2D)Adapter.M_Colors.labelIcons[x + y * 4].image;
						   GUI.Box( ICON_RECT, "", labelStyle )*/
						if (bs == null) bs = new GUIStyle() { border = new RectOffset(5, 5, 5, 5) };
						bs.normal.background = (Texture2D)HighlighterCache_Icons.labelIcons[x + y * 4].image;
						if (Event.current.type == EventType.Repaint) bs.Draw(ICON_RECT, "", false, false, false, false);


						//  GUI.DrawTexture( ICON_RECT , (Texture2D)Adapter.M_Colors.labelIcons[x + y * 4].image , ScaleMode.StretchToFill );
						//Adapter.DrawTexture( ICON_RECT , (Texture2D)Adapter.M_Colors.labelIcons[x + y * 4].image );

						EditorGUIUtility.AddCursorRect(ICON_RECT, MouseCursor.Link);

						var c = label.normal.textColor;
						label.normal.textColor = black;
						GUI.Label(ICON_RECT, "Label", label);
						label.normal.textColor = c;

						ICON_RECT.y -= 1;
						if (Button(ICON_RECT, ""))
						{
							SetIconImage(HighlighterCache_Icons.labelIcons[x + y * 4].image, "Set Icon");
							REPAINT_ALL_HIERW();
							//InternalEditorUtility.RepaintAllViews();
							TryToClose();
#if CLOSE_AFTERICON
                        CloseThis();
#endif
						}
					}
					// GUI.EndHorizontal();
				}
				GUI.EndGroup();

				var cur = ICON_RECT.x + GROUP_RECT.x + ICON_RECT.width;
				var tar = GROUP_RECT.x + GROUP_RECT.width + PAD;
				var est = (tar - cur) / 2 + cur;
				Root.p[0].INTERNAL_BOX(new Rect(est, GROUP_RECT.y, 1, inputrect.height));
				// GUI.EndVertical();//label
			}
			GROUP_RECT.width /= 2;


			GROUP_RECT.x += GROUP_RECT.width + PAD;
			GROUP_RECT.x += GROUP_RECT.width;


			if (HighlighterCache_Icons.largeIcons != null && HighlighterCache_Icons.largeIcons.Length >= 16)
			{   //   GROUP_RECT = new Rect(lastRect.x + 4 * w10 + PAD, lastRect.y, 4 * w10 + PAD, lastRect.height);
				GUI.BeginGroup(GROUP_RECT);
				var ICON_RECT = GROUP_RECT;
				ICON_RECT.width = w10;
				ICON_RECT.height = IHH;
				for (int y = 0; y < 2; y++)
				{
					for (int x = 0; x < 4; x++)
					{   /*var bg = Adapter.GET_SKIN().box.normal.background;
                    Adapter.GET_SKIN().box.normal.background = (Texture2D)Adapter.M_Colors.largeIcons[x + y * 4].image;
                    ICON_RECT.x = x * w10;
                    ICON_RECT.y = y * wH + d ;
                    //  ICON_RECT.y = y * w10 / 1.5f;
                    Adapter. INTERNAL_BOX( ICON_RECT, "" );
                    Adapter.GET_SKIN().box.normal.background = bg;*/


						ICON_RECT.x = x * w10;
						ICON_RECT.y = y * wH + d;
						/* labelStyle.border = new RectOffset(B, B, B, B );
						 labelStyle.normal.background = (Texture2D)Adapter.M_Colors.largeIcons[x + y * 4].image;
						 GUI.Box( ICON_RECT, "", labelStyle );*/
						// Adapter.DrawTexture( ICON_RECT , (Texture2D)Adapter.M_Colors.largeIcons[x + y * 4].image );


						GUI.DrawTexture(ICON_RECT, (Texture2D)HighlighterCache_Icons.largeIcons[x + y * 4].image, ScaleMode.ScaleToFit);



						EditorGUIUtility.AddCursorRect(ICON_RECT, MouseCursor.Link);

						ICON_RECT.y -= 1;
						if (Button(ICON_RECT, ""))
						{
							SetIconImage((Texture2D)HighlighterCache_Icons.largeIcons[x + y * 4].image, "Set Icon");
							REPAINT_ALL_HIERW();
							// InternalEditorUtility.RepaintAllViews();
							TryToClose();
#if CLOSE_AFTERICON
                        
                        CloseThis();
#endif
						}
					}
				}
				GUI.EndGroup();
				var cur = ICON_RECT.x + GROUP_RECT.x + ICON_RECT.width;
				var tar = GROUP_RECT.x + GROUP_RECT.width + PAD;
				var est = (tar - cur) / 2 + cur;
				Root.p[0].INTERNAL_BOX(new Rect(est, GROUP_RECT.y, 1, inputrect.height));

				GROUP_RECT.x += GROUP_RECT.width + PAD;

				// GROUP_RECT = new Rect(lastRect.x + 8 * w10 + PAD, lastRect.y, 4 * w10 + PAD, lastRect.height);

				GUI.BeginGroup(GROUP_RECT);
				ICON_RECT = GROUP_RECT;
				ICON_RECT.width = w10;
				ICON_RECT.height = IHH;
				for (int y = 2; y < 4; y++)
				{
					for (int x = 0; x < 4; x++)
					{   /*var bg = Adapter.GET_SKIN().box.normal.background;
                    Adapter.GET_SKIN().box.normal.background = (Texture2D)Adapter.M_Colors.largeIcons[x + y * 4].image;
                    ICON_RECT.x = (x) * w10;
                    ICON_RECT.y = (y - 2) * wH + d ;
                    // ICON_RECT.y = (y - 2) * w10 / 1.5f;
                    Adapter. INTERNAL_BOX( ICON_RECT, "" );
                    Adapter.GET_SKIN().box.normal.background = bg;*/

						ICON_RECT.x = (x) * w10;
						ICON_RECT.y = (y - 2) * wH + d;
						/*labelStyle.border = new RectOffset(B, B, B, B );
						labelStyle.normal.background = (Texture2D)Adapter.M_Colors.largeIcons[x + y * 4].image;

						GUI.Box( ICON_RECT, "", labelStyle );*/

						GUI.DrawTexture(ICON_RECT, (Texture2D)HighlighterCache_Icons.largeIcons[x + y * 4].image, ScaleMode.ScaleToFit);
						// Adapter.DrawTexture( ICON_RECT , (Texture2D)Adapter.M_Colors.largeIcons[x + y * 4].image );


						EditorGUIUtility.AddCursorRect(ICON_RECT, MouseCursor.Link);

						ICON_RECT.y -= 1;
						if (Button(ICON_RECT, ""))
						{
							SetIconImage((Texture2D)HighlighterCache_Icons.largeIcons[x + y * 4].image, "Set Icon");
							REPAINT_ALL_HIERW();
							//InternalEditorUtility.RepaintAllViews();
							TryToClose();
#if CLOSE_AFTERICON
                        CloseThis();
#endif
						}
					}
				}
				GUI.EndGroup();
			}
		}













		void DRAW_LAST_ICONS(Rect inputrect)
		{

			/*    var commandName = Event.current.commandName;
				if (!string.IsNullOrEmpty(commandName)) MonoBehaviour.print(commandName);
				if (commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == pickerId)
				{
					MonoBehaviour.print(EditorGUIUtility.GetObjectPickerObject());
					//comformAction((Texture2D)EditorGUIUtility.GetObjectPickerObject());
				}*/

			//  GUI.BeginVertical(GUI.Width(50)); //last icon
			//  GUI.EndVertical(); //last icon


			var lastRect = inputrect;

			/*   lastRect.height /= 2;
			   lastRect.y += lastRect.height;*/

			var PAD = 5;
			var O = 80;
			Label(new Rect(lastRect.x + 5, lastRect.y, O, lastRect.height), "Last Icons:");

			/*   GUI.BeginHorizontal();
			   GUI.Space(20);
			   GUI.BeginVertical(GUI.Height(180)); //last icon*/

			var iconSize = 25;
			var LCOUNT = 7;
			int interator = 0;
			//  Label("");

			lastRect.x += O;
			lastRect.width -= O;

			lastRect.width -= PAD;
			//Adapter. INTERNAL_BOX( lastRect, "" );
			if (Event.current.type == EventType.Repaint) GUI.skin.textArea.Draw(lastRect, "", false, false, false, false);
			lastRect.y += 2;

			GUI.BeginGroup(lastRect);
			lastRect.x = 0;
			lastRect.y = 0;


			

			for (int i = 0, Interator = 0; i < Math.Min(HighLighterCommonData.GetIconsHistory().Count, LCOUNT) && Interator < HighLighterCommonData.GetIconsHistory().Count; i++, Interator++)
			{
				if (string.IsNullOrEmpty(HighLighterCommonData.GetIconsHistory()[Interator])) continue;
				Texture2D display = null;
				if (HighLighterCommonData.GetIconsHistory()[Interator].StartsWith("GUID="))
				{
					var path = AssetDatabase.GUIDToAssetPath(HighLighterCommonData.GetIconsHistory()[Interator].Substring(5));
					if (path != null) display = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
				}
				else
				{
					HighlighterCache_Icons.Init_InternatlDefault_IconsList();
					display = HighlighterCache_Icons.Get_InternalDefault_Icon(HighLighterCommonData.GetIconsHistory()[Interator]);
				}


				if (display == null)
				{
					i--;
					continue;
				}
				var rect = new Rect(5 + i * iconSize, (lastRect.height - iconSize) / 2, iconSize, iconSize);
				EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
				Label(rect, "");

				GUI.DrawTexture(rect, display, ScaleMode.ScaleToFit);
				interator++;
				if (Button(rect, ""))
				{

					SetIconImage(display, "Set Icon");
					TryToClose();
#if CLOSE_AFTERICON
                CloseThis();
#endif
				}
			}


			GUI.EndGroup();
		}

		GUIStyle bs;

	}
}
