using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EMX.HierarchyPlugin.Editor.Mods;


namespace EMX.HierarchyPlugin.Editor.Windows
{
	public partial class Root_HighlighterWindowInterface : Windows.IWindow
	{
		static float singleLineHeight {
			get {
				return 16;// EditorGUIUtility.singleLineHeight;
			}
		}
		static void Shrink( ref Rect rect, int amount )
		{

			rect.x += amount;
			rect.y += amount;
			rect.width -= 2 * amount;
			rect.height -= 2 * amount;
		}
		static Rect Shrink( Rect rect, int amount )
		{
			rect.x += amount;
			rect.y += amount;
			rect.width -= 2 * amount;
			rect.height -= 2 * amount;
			rect.x = (int)rect.x;
			rect.y = (int)rect.y;
			rect.width = (int)rect.width;
			rect.height = (int)rect.height;
			return rect;
		}

		static void ListToTextColor( TempColorClass list, out Color color, out bool hasColor )
		{
			color = list.LABELCOLOR;
			hasColor = color[ 0 ] > 0 || color[ 1 ] > 0 || color[ 2 ] > 0 || color[ 3 ] > 0;
		}
		static void ListToBgColor( TempColorClass list, out Color color, out bool hasColor )
		{
			color = list.BGCOLOR;
			hasColor = color[ 0 ] > 0 || color[ 1 ] > 0 || color[ 2 ] > 0 || color[ 3 ] > 0;
		}
		static void ListToBgColor( IntList list, out Color color, out bool hasColor )
		{
			color = list.list.Length < 4 ? Color.clear : (Color)new Color32( (byte)list.list[ 0 ], (byte)list.list[ 1 ], (byte)list.list[ 2 ], (byte)list.list[ 3 ] );
			hasColor = color[ 0 ] > 0 || color[ 1 ] > 0 || color[ 2 ] > 0 || color[ 3 ] > 0;
		}
		/*   static   void ListToTextColor(IntList list, out Color color, out bool hasColor)
		   {   color = list.list.Count < 9 ? TRANSP_COLOR : (Color)new Color32((byte)list.list[5], (byte)list.list[6], (byte)list.list[7], (byte)list.list[8]);
			   hasColor = color[0] > 0 || color[1] > 0 || color[2] > 0 || color[3] > 0;
		   }
		   static  void ListToBgColor(IntList list, out Color color, out bool hasColor)
		   {   color =  list.list.Count < 4 ? TRANSP_COLOR : (Color)new Color32((byte)list.list[0], (byte)list.list[1], (byte)list.list[2], (byte)list.list[3]);
			   hasColor = color[0] > 0 || color[1] > 0 || color[2] > 0 || color[3] > 0;
		   }*/


		//  DrawStroke(PICKER_RECT, "Text Color", newCol2);
		//GameObject debugsource;


		void drawdord( Rect R_LABEL, bool enable )
		{
			if ( !enable ) return;
			if ( Event.current.type == EventType.Repaint )
			{   // EditorStyles.helpBox.Draw( new Rect( R_LABEL.x , R_LABEL.y , R_LABEL.width , R_LABEL.height ) , "" , true , true , true , true );
				var fh = EditorStyles.toolbarButton.fixedHeight;
				EditorStyles.toolbarButton.fixedHeight = 0;
				if ( UnityVersion.UNITY_CURRENT_VERSION >= UnityVersion.UNITY_2019_3_0_VERSION )
					EditorStyles.toolbarButton.Draw( new Rect( R_LABEL.x, R_LABEL.y, R_LABEL.width, R_LABEL.height ), "", false, false, false, false );
				else
					EditorStyles.helpBox.Draw( new Rect( R_LABEL.x, R_LABEL.y, R_LABEL.width, R_LABEL.height ), "", true, true, true, true );
				EditorStyles.toolbarButton.fixedHeight = fh;
			}

		}


		float DrawHiglighter( Rect inputrect )
		{
			var getcolor = GetHiglightData();


			bool Enable_BackgroundColor = false;
			bool LabelEnable = false;
			Color refLabelCol;
			ListToTextColor( getcolor, out refLabelCol, out LabelEnable );
			/* if ( debugsource  != source.go ) {
				 debugsource = source.go;
				 Debug.Log( getcolor.LABELCOLOR );
			 }*/
			Color refBGCol;
			ListToBgColor( getcolor, out refBGCol, out Enable_BackgroundColor );


			var o_n = GUI.enabled;
			GUI.enabled = Enable_BackgroundColor || LabelEnable;

			inputrect.height = singleLineHeight;
			HighLighterStyle.LABEL( inputrect, "Highlighter:" );



			{
				var rad = inputrect;

				rad.width -= PADDD * 2 + 2;
				rad.width /= 4;
				rad.x += rad.width;

				if ( !(HighlighterCache_Icons.GET_CONTENT( source, mod ).add_icon || Enable_BackgroundColor || LabelEnable) )
				{
					rad.width *= 3;
					HighLighterStyle.LABEL( rad, "Use ctrl click to close the window immediately" );
				}


				string[] cats = new[] { "This", "Child" };
				if ( GUI.enabled )
				{
					var nv = GUI.Toolbar(new Rect(rad.x, rad.y, rad.width * 2, rad.height), getcolor.child ? 1 : 0, cats, EditorStyles.toolbarButton) == 1;
					if ( getcolor.child != nv )
					{
						getcolor.child = nv;
						SetHiglightData( getcolor, "Change Child Color" );
					}
				}
				HighLighterStyle.TOOLTIP( rad, "If the child is on, the highlighter settings will affect all children." );

				rad.x += rad.width * 2;
				GUI.enabled = o_n;

				if ( (HighlighterCache_Icons.GET_CONTENT( source, mod ).add_icon || Enable_BackgroundColor || LabelEnable) && GUI.Button( rad, "Clear", Root.p[ 0 ].STYLE_DEFBUTTON_right ) )
				{
					getcolor.LABELCOLOR = Color.clear;
					SetHiglightData( getcolor, "Clear" );

					getcolor.BGCOLOR = Color.clear;
					SetHiglightData( getcolor, "Clear" );

					SetIconImage( null, "Clear" );
					REPAINT_ALL_HIERW();

					CloseThis();
				}


				/*  rad.width /= 2;
				  rad.x += rad.width;
				  rad.width -= PADDD * 2 + 2;*/


				/*   var newchild = adapter.TOGGLE_LEFT(rad, "Apply To <b>Child</b>", getcolor.child);
				   if (newchild != getcolor.child)
				   {   getcolor.child = newchild;
					   SetHiglightData(getcolor, "Change Child Color");
				   }*/




			}
			inputrect.y += inputrect.height;
			GUI.enabled = o_n;

			inputrect.x += PADDD;
			inputrect.width -= PADDD * 2;

			// var LINE_H = inputrect.height / 2;


			/* var R_CLOSE = new Rect(inputrect.x, inputrect.y + (LINE_H - 14) / 2, 14, 14);
			 var R_LABEL = new Rect(inputrect.x + R_CLOSE.width + 3, inputrect.y, 48, LINE_H);
			 var R_COLOR = new Rect(R_LABEL.x + R_LABEL.width, inputrect.y + (LINE_H - 23) / 2, 55, 23);
			 var R_LAST = new Rect(R_COLOR.x + R_COLOR.width, inputrect.y, 134, LINE_H);
			 var R_CHILD = new Rect(R_LAST.x + R_LAST.width, inputrect.y, 0, inputrect.height);*/


			// var R_CLOSE = new Rect(inputrect.x, inputrect.y + (LINE_H - 14) / 2, 14, 14);
			// var R_CLOSE = new Rect(inputrect.x, inputrect.y + (LINE_H - 14) / 2, 0, 14);
			var LABEL_H = singleLineHeight + 8;
			var R_LABEL = new Rect(inputrect.x, inputrect.y + LABEL_H + singleLineHeight, inputrect.width - 4, singleLineHeight * 4 + 4); //- 4
			var R_LAST = new Rect(inputrect.x + PADDD, inputrect.y + singleLineHeight, inputrect.width - PADDD * 2 - 2, LINE_H);

			var R_COLOR = new Rect(inputrect.x, inputrect.y + R_LABEL.height, 55, 23);

			var R_TOOLBAR = new Rect(R_LABEL.x, R_LABEL.y + R_LAST.height, R_LABEL.width, R_LAST.height);

			R_LABEL.height -= singleLineHeight;

			R_COLOR.y += 3;
			R_LAST.y += 3;
			R_LABEL.height += 8;
			R_COLOR.height += 4;
			R_COLOR.width += 14;
			var R_CHILD = new Rect(inputrect.x, inputrect.y + R_LABEL.height, 0, singleLineHeight);
			R_CHILD.height += 8;
			R_CHILD.width = R_LABEL.width;
			//  R_LAST.x += 3;
			//  R_LAST.width -= 6;
			R_LAST.y += 3;
			R_LAST.height -= 6;



			Color newCol2, newCol;

			//* TEXT COLOR *//
			{

				//var  refCol = getcolor.list.Count < 9 ? TRANSP_COLOR : (Color)new Color32((byte)getcolor.list[5], (byte)getcolor.list[6], (byte)getcolor.list[7], (byte)getcolor.list[8]);
				// en2 = refCol[0] > 0 || refCol[1] > 0 || refCol[2] > 0 || refCol[3] > 0;


				// if (refCol[0] > 0 || refCol[1] > 0 || refCol[1] > 0 || refCol[1] > 0)
				/*  {   var asdasd = GUI.color;
					  if (!en) GUI.color *= new Color(1, 1, 1, 0.4f);
					  GUI.DrawTexture(R_CLOSE, adapter.GetIcon("HIPERUI_CLOSE"));
					  GUI.color = asdasd;
					  if (en && Button(R_CLOSE, ""))
					  {   getcolor.list[5] = getcolor.list[6] = getcolor.list[7] = getcolor.list[8] = 0;
						  SetHiglightColor(getcolor);
						  REPAINT();
					  }
				  }

				  var asdasdasd = GUI.color;
				  if (en) GUI.color *= refCol;
				  Label(R_LABEL, "Label Color");
				  GUI.color = asdasdasd;*/

				//                 if ( Event.current.type == EventType.Repaint )
				//                     GUI.skin.textArea.Draw( new Rect( R_LABEL.x , R_LABEL.y , R_LABEL.width , R_LABEL.height ) , "" , false , false , false , false );
				drawdord( R_LABEL, LabelEnable );

				Shrink( ref R_LABEL, 2 );
				Shrink( ref R_COLOR, 2 );

				var R2 = new Rect(R_LABEL.x + R_LABEL.width / 2, R_LABEL.y, R_LABEL.width / 2, LABEL_H);
				//  EditorGUI.DrawRect(Shrink(R2, 2), refCol);

				var on = GUI.enabled;
				GUI.enabled = LabelEnable;
				emptyContent.tooltip = "Label Color";
				RESTORE_GUI();
				newCol2 = refLabelCol;
				if ( GUI.enabled ) newCol2 = PICKER( R2, emptyContent, refLabelCol );
				CHANGE_GUI();
				GUI.enabled = on;

				var newLabelENable = EditorGUI.ToggleLeft(Shrink(new Rect(R_LABEL.x, R_LABEL.y, R_LABEL.width, singleLineHeight + 4), 2), "Use Label Color", LabelEnable);

				GUI.enabled = LabelEnable;



				if ( newLabelENable != LabelEnable )
				{
					LabelEnable = newLabelENable;
					// while (getcolor.list.Count < 9)getcolor.list.Add(0);
					if ( !LabelEnable )
					{
						getcolor.LABELCOLOR = Color.clear;
						SetHiglightData( getcolor, "Change Text Color" );
						REPAINT_ALL_HIERW();
					}
					else
					{   /*var lastText = Hierarchy_GUI.GetLastHiglightTextList( adapter );
                        if (lastText.Count == 0) getcolor.LABELCOLOR = Color.white;
                        else
                        {   var new32 = lastText[0];
                            getcolor.LABELCOLOR = new32;
                        }*/
						TempColorClass.CopyFromTo( CopyType.LABEL, from: HighLighterCommonData.GetLastTempColor(), to: ref getcolor );

						// getcolor.LABELCOLOR = getcolor.GetLastTempColor(adapter).LABELCOLOR;
						SetHiglightData( getcolor, "Change Text Color" );
						REPAINT_ALL_HIERW();
					}
				}


				// if (en2)
				{

					var l = HighLighterCommonData.GetTextColorsHistory();

					//                     if ( Event.current.type == EventType.Repaint )
					//                         STYLE_DEFBOX.Draw( new Rect( R_LAST.x , R_LAST.y , R_LAST.width , R_LAST.height ) , "" , false , false , false , false );
					/* if (LabelEnable)*/
					DRAW_LAST_HIGHLIGHTER( Shrink( R_LAST, 0 ), ref newCol2, ref l, false );

					if ( newCol2 != refLabelCol )
					{




						var new32 = (Color32)newCol2;
						if ( new32.a == 0 && (new32.r != 0 || new32.g != 0 || new32.b != 0) ) new32.a = 255;
						// while (getcolor.list.Count < 9) getcolor.list.Add(0);
						getcolor.LABELCOLOR = new32;
						if ( new32.r != 0 || new32.g != 0 || new32.b != 0 || new32.a != 0 ) UpdateLastHiglightTextColors( new32 );
						SetHiglightData( getcolor, "Change Text Color" );
					}


					if ( LabelEnable )
					{
						var r = R_TOOLBAR;
						//  r.width /= 10;
						r.x += singleLineHeight;
						r.width -= singleLineHeight * 2;
						r.height = singleLineHeight + 2;
						r.y += (singleLineHeight * 2 - r.height) / 2;
						/* var newVal = adapter.TOGGLE_LEFT(r, "Shadow", getcolor.LABEL_SHADOW, skipMark: true);
						 if (newVal != getcolor.LABEL_SHADOW)
						 {   getcolor.LABEL_SHADOW = newVal;
							 SetHiglightData(getcolor, "Change Shadow Color");
						 }*/

						string[] cats = new[] { "Disable", "Enable Shadow" };
						var nv = GUI.Toolbar(r, getcolor.LABEL_SHADOW ? 1 : 0, cats, EditorStyles.toolbarButton) == 1;
						if ( getcolor.LABEL_SHADOW != nv )
						{
							getcolor.LABEL_SHADOW = nv;
							SetHiglightData( getcolor, "Change Shadow Color" );
						}

					}
				}

				GUI.enabled = on;


				Shrink( ref R_LABEL, -2 );
				Shrink( ref R_COLOR, -2 );
			}
			//* TEXT COLOR *//
			//* LAST *//
			//* LAST *//

			HighLighterStyle.HR( R_LAST );

			//  if (en2)
			{
				R_LABEL.y += LINE_H;
				R_COLOR.y += LINE_H;
				R_LAST.y += LINE_H;
				R_CHILD.y += LINE_H;
				R_TOOLBAR.y += LINE_H;
			}
			var offset = 20;
			R_LABEL.y += LINE_H * 2 + offset;
			R_COLOR.y += LINE_H * 2 + offset;
			R_LAST.y += LINE_H * 2 + offset;
			R_CHILD.y += LINE_H * 2 + offset;
			R_TOOLBAR.y += LINE_H * 2 + offset;

			HighLighterStyle.HR( R_LAST );

			R_LABEL.height += singleLineHeight * 4;
			//* HIGLIGHTER */
			{



				//var refCol = getcolor.list.Count < 4 ? TRANSP_COLOR : (Color)new Color32((byte)getcolor.list[0], (byte)getcolor.list[1], (byte)getcolor.list[2], (byte)getcolor.list[3]);
				// en = refCol[0] > 0 || refCol[1] > 0 || refCol[2] > 0 || refCol[3] > 0;
				// if (refCol[0] > 0 || refCol[1] > 0 || refCol[1] > 0 || refCol[1] > 0)
				/* {   var asdasd = GUI.color;
					 if (!en) GUI.color *= new Color(1, 1, 1, 0.4f);
					 GUI.DrawTexture(R_CLOSE, adapter.GetIcon("HIPERUI_CLOSE"));
					 GUI.color = asdasd;
					 if (en && Button(R_CLOSE, ""))
					 {   getcolor.list[0] = getcolor.list[1] = getcolor.list[2] = getcolor.list[3] = 0;
						 SetHiglightColor(getcolor);
						 REPAINT();
					 }
				 }
				 var asdasdasd = GUI.color;
				 if (en) GUI.color *= refCol;
				 EditorGUI.DrawRect(R_LABEL, GUI.color);
				 GUI.color = asdasdasd;
				 Label(R_LABEL, "Background Color");*/
				//if (en) EditorGUI.DrawRect(new Rect(R_LABEL.x + R_LABEL.width - R_LABEL.height, R_LABEL.y, R_LABEL.height, R_LABEL.height), refCol);

				//                 if ( Event.current.type == EventType.Repaint )
				//                     GUI.skin.textArea.Draw( new Rect( R_LABEL.x , R_LABEL.y , R_LABEL.width , R_LABEL.height ) , "" , false , false , false , false );
				drawdord( R_LABEL, Enable_BackgroundColor );
				/*  if ( Event.current.type == EventType.Repaint )
					  EditorStyles.helpBox.Draw( new Rect( R_LABEL.x, R_LABEL.y, R_LABEL.width, R_LABEL.height ), "", false, false, false, false );*/
				Shrink( ref R_LABEL, 2 );
				Shrink( ref R_COLOR, 2 );

				//   GUI.Box(R_LABEL, "");
				var R2 = new Rect(R_LABEL.x + R_LABEL.width / 2, R_LABEL.y, R_LABEL.width / 2, LABEL_H);
				//EditorGUI.DrawRect(Shrink(R2, 2), refCol);

				var on = GUI.enabled;
				GUI.enabled = Enable_BackgroundColor;
				emptyContent.tooltip = "Background Color";
				RESTORE_GUI();
				newCol = refBGCol;
				if ( GUI.enabled ) newCol = PICKER( R2, emptyContent, refBGCol );
				CHANGE_GUI();

				GUI.enabled = on;
				//  EditorGUI.DrawRect(Shrink(R_LABEL, 2), refCol);
				var newEn = EditorGUI.ToggleLeft(Shrink(new Rect(R_LABEL.x, R_LABEL.y, R_LABEL.width, singleLineHeight + 4), 2), "Use Background Color", Enable_BackgroundColor);

				GUI.enabled = Enable_BackgroundColor;


				if ( newEn != Enable_BackgroundColor )
				{
					Enable_BackgroundColor = newEn;
					if ( !Enable_BackgroundColor )
					{
						getcolor.BGCOLOR = Color.clear;
						SetHiglightData( getcolor, "Change Highlighter Color" );
						REPAINT_ALL_HIERW();
					}
					else
					{   /*var lastHiglight = Hierarchy_GUI.GetLastHiglightList( adapter );
                        if (lastHiglight.Count == 0) getcolor.BGCOLOR = Color.white;
                        else
                        {   var new32 = lastHiglight[0];
                            getcolor.BGCOLOR =  new32;
                        }*/

						//	getcolor.BGCOLOR = HighLighterCommonData.GetLastTempColor().BGCOLOR;

						TempColorClass.CopyFromTo( CopyType.BG, from: HighLighterCommonData.GetLastTempColor(), to: ref getcolor );

						SetHiglightData( getcolor, "Change Highlighter Color" );
						REPAINT_ALL_HIERW();
					}
				}



				//if (en)
				{

					var l = HighLighterCommonData.GetBackGroundColorsHistory();
					/*  if ( Event.current.type == EventType.Repaint )
					  {   var c = GUI.color;
						  GUI.color *= new Color( 1, 1, 1, 0.8f );
						  GUI.skin.textArea.Draw( new Rect( R_LAST.x, R_LAST.y, R_LAST.width, R_LAST.height ), "", false, false, false, false );
						  GUI.color = c;
					  }*/
					//    if ( Event.current.type == EventType.Repaint )
					//        STYLE_DEFBOX.Draw( new Rect( R_LAST.x , R_LAST.y , R_LAST.width , R_LAST.height ) , "" , false , false , false , false );
					/*  if ( Enable_BackgroundColor )*/
					DRAW_LAST_HIGHLIGHTER( Shrink( R_LAST, 0 ), ref newCol, ref l, true );

					if ( newCol != refBGCol )
					{
						var new32 = (Color32)newCol;
						if ( new32.a == 0 && (new32.r != 0 || new32.g != 0 || new32.b != 0) ) new32.a = 255;
						getcolor.BGCOLOR = new32;
						if ( new32.r != 0 || new32.g != 0 || new32.b != 0 || new32.a != 0 ) UpdateLastHiglightColors( new32 );
						SetHiglightData( getcolor, "Change Highlighter Color" );
					}


					if ( Enable_BackgroundColor )
					{

						var r = R_TOOLBAR;
						//  r.width /= 10;
						r.x += singleLineHeight;
						r.width -= singleLineHeight * 2;
						r.height = singleLineHeight + 2;
						r.y += (singleLineHeight * 2 - r.height) / 2;
						int nv;
						Rect RECT;
						// string[]  cats;
						/*  cats = new [] { "Full Height", "Narrow Height"};
						  nv = GUI.Toolbar(r,   getcolor.BG_HEIGHT, cats, EditorStyles.toolbarButton) ;
						  if (getcolor.BG_HEIGHT != nv)
						  {   getcolor.BG_HEIGHT = nv;
							  SetHiglightData(getcolor, "Change Narrow Color");
						  }

						  r.y += r.height;*/

						var r2 = r;
						r2.width /= 2;
						nv = HighLighterStyle.TOGGLE_LEFT( r2, "Narrow Height", getcolor.BG_HEIGHT == 1, skipMark: true ) ? 1 : getcolor.BG_HEIGHT == 1 ? 0 : getcolor.BG_HEIGHT;
						if ( nv != getcolor.BG_HEIGHT )
						{
							getcolor.BG_HEIGHT = nv;
							SetHiglightData( getcolor, "Change Narrow Color" );
						}
						r2.x += r2.width;
						var enn = GUI.enabled;
						GUI.enabled &= !getcolor.child;
						nv = HighLighterStyle.TOGGLE_LEFT( r2, "1 pixel Height", getcolor.BG_HEIGHT == 2, skipMark: true ) ? 2 : getcolor.BG_HEIGHT == 2 ? 0 : getcolor.BG_HEIGHT;
						if ( nv != getcolor.BG_HEIGHT )
						{
							getcolor.BG_HEIGHT = nv;
							SetHiglightData( getcolor, "Change Narrow Color" );
						}
						HighLighterStyle.TOOLTIP( r, "The size of the background will be calculated based on font size and not the height of the line." );
						GUI.enabled = enn;
						r.y += r.height + 2;



						RECT = r;
						HighLighterStyle.LABEL( RECT, "Horizontal Align:" );
						r.y += r.height;


						RECT = r;
						/* LABEL(RECT, "Left Align:");
						 RECT.x += RECT.width / 5;
						 RECT.width -= RECT.width / 5;*/
						RECT.width -= RECT.width / 6;
						nv = GUI.Toolbar( RECT, getcolor.BG_ALIGMENT_LEFT, getcolor.ALIGMENT_LEFT_CATEGORIES, EditorStyles.toolbarButton );
						if ( getcolor.BG_ALIGMENT_LEFT != nv )
						{
							getcolor.BG_ALIGMENT_LEFT = nv;
							SetHiglightData( getcolor, "Change Left Align Color" );
						}
						HighLighterStyle.TOOLTIP( RECT, "Left Align Position for Background Color." );
						r.y += r.height + 2;
						//internal enum BgAligmentRight {MaxRight = 0, Modules = 1, EndLabel = 2, BeginLabel = 3, Icon = 4, WidthFixedGradient = 5}

						RECT = r;
						/*  LABEL(RECT, "Right Align:");
						  RECT.x += RECT.width / 5;
						  RECT.width -= RECT.width / 5;*/

						var er = RECT;
						er.width = er.width / 6;
						nv = HighLighterStyle.TOGGLE_LEFT( er, "Fixed", getcolor.BG_ALIGMENT_RIGHT == 5, skipMark: true ) ? 5 : 0;

						RECT.x += RECT.width / 6;
						RECT.width -= RECT.width / 6;

						if ( nv != getcolor.BG_ALIGMENT_RIGHT && (getcolor.BG_ALIGMENT_RIGHT == 5 || nv == 5) )
						{
							getcolor.BG_ALIGMENT_RIGHT = nv;
							SetHiglightData( getcolor, "Change Gradient Color" );
						}
						if ( getcolor.BG_ALIGMENT_RIGHT == 5 )
						{

							RECT.y += Mathf.RoundToInt( (RECT.height - singleLineHeight) / 2f );

							nv = Mathf.Clamp( EditorGUI.IntField( RECT, "Right Width:", getcolor.BG_WIDTH ), 10, 255 );
							if ( nv != getcolor.BG_WIDTH )
							{
								getcolor.BG_WIDTH = nv;
								SetHiglightData( getcolor, "Change Width Color" );
							}
						}
						else
						{
							var o___n = GUI.enabled;
							GUI.enabled = getcolor.BG_ALIGMENT_RIGHT != 5;
							var ov = 4 - getcolor.BG_ALIGMENT_RIGHT;
							nv = GUI.Toolbar( RECT, ov, getcolor.ALIGMENT_RIGHT_CATEGORIES, EditorStyles.toolbarButton );
							if ( nv != ov )
							{
								getcolor.BG_ALIGMENT_RIGHT = 4 - nv;
								SetHiglightData( getcolor, "Change Right Align Color" );
							}
							HighLighterStyle.TOOLTIP( RECT, "Right Align Position for Background Color." );
							GUI.enabled = o___n;
						}







						/*   EditorGUI.EnumPopup(r, (BgAligmentLeft) getcolor.BG_ALIGMENT_LEFT);
						   r.y += r.height;
						   EditorGUI.EnumPopup(r, (BgAligmentRight) getcolor.BG_ALIGMENT_RIGHT);
						   var o__n = GUI.enabled;
						   GUI.enabled = (BgAligmentRight) getcolor.BG_ALIGMENT_RIGHT == BgAligmentRight.WidthFixedGradient;
						   r.x += r.width;
						   EditorGUI.IntField(r, "Width:", getcolor.BG_WIDTH );
						   GUI.enabled = o__n;*/
						/*  r.x += r.width;
						  r.x += r.width;
						  EditorGUI.EnumPopup(r, (BgAligmentHeight) getcolor.BG_HEIGHT);*/

					}
				}


				GUI.enabled = on;

				//* HIGLIGHTER *//
				//* LAST *//
				//* LAST *//
			}












			//* CHILDREN *//
			//  var child = getcolor.child;

			/*if (child)
			{   var contC = GUI.color;
				GUI.color *= newCol.r == 0 && newCol.g == 0 && newCol.b == 0 && newCol.a == 0 ? newCol2 : newCol;
				GUI.DrawTexture(R_CHILD, Texture2D.whiteTexture);
				DrawStroke(new Rect(R_CHILD.x + GET_SKIN().button.padding.left, R_CHILD.y, R_CHILD.width, R_CHILD.height), "Apply\nTo Childes", GUI.color);
				GUI.color = contC;
			}*/
			// var newchild = child;

			// RESTORE_GUI();
			// EditorGUIUtility.AddCursorRect(R_CHILD, MouseCursor.Link);


			/* if (Event.current.type == EventType.Repaint)
				 GUI.skin.textArea.Draw(R_CHILD, "", false, false, false, false);*/
			//  GUI.Box(R_CHILD, "");
			/*   GUI.enabled = Enable_BackgroundColor || LabelEnable;

			 newchild = EditorGUI.ToggleLeft(Shrink(new Rect(R_CHILD.x + 80, R_CHILD.y, R_CHILD.width, R_CHILD.height), 2), "Apply To Children", child);
			  GUI.enabled = true;
			  //  if (Button(R_CHILD, "Apply\nTo Childes")) newchild = !child;
			  if (child != newchild)
			  {

				  getcolor.child = newchild ;
				  SetHiglightData(getcolor, "Change Child Color");
			  }*/

			/* R_CHILD.x += R_CHILD.width - 10 - 42;
			 R_CHILD.y += (R_CHILD.height - 42) / 2;
			 R_CHILD.width = R_CHILD.height = 42;
			 GUI.DrawTexture(R_CHILD, adapter.GetIcon(EditorGUIUtility.isProSkin ? "HIGHLIGHTER_CHILD" : "HIGHLIGHTER_CHILD PERSONAL"));*/

			// CHANGE_GUI();

			//  GET_SKIN().button.fontSize = oldS;
			//* CHILDREN *//



			return R_LABEL.y + R_LABEL.height + singleLineHeight;
		}



		void DRAW_LAST_HIGHLIGHTER( Rect inputrect, ref Color result, ref List<Color32> list, bool bg )
		{

			var oldEnable = GUI.enabled;
			GUI.enabled = true;


			if (
				Event.current.type == EventType.Repaint )
			{
				var c = GUI.color;
				GUI.color *= new Color( 1, 1, 1, 0.5f );
				//EditorStyles.helpBox.Draw( new Rect( inputrect.x, inputrect.y, inputrect.width, inputrect.height ), "", false, false, false, false );
				GUI.skin.textArea.Draw( new Rect( inputrect.x, inputrect.y, inputrect.width, inputrect.height ), "", false, false, false, false );
				//  STYLE_DEFBOX.Draw( new Rect( inputrect.x, inputrect.y, inputrect.width, inputrect.height ), "", false, false, false, false );
				GUI.color = c;
			}



			var asd = GUI.color;
			//  GUI.color = new Color32(0, 45, 70, 80);
			GUI.color *= new Color32( 0, 0, 0, 30 );
			//GUI.DrawTexture(inputrect, Texture2D.whiteTexture);
			//  INTERNAL_BOX(new Rect(0, 0, FULL_RECT.width + PICKER_RECT.width, FULL_RECT.height), "");
			GUI.color = asd;


			// INTERNAL_BOX(inputrect, "");




			var iconSize = singleLineHeight * 1.15f;
			var LCOUNT = 16;
			int interator = 0;


			GUI.BeginGroup( inputrect );
			var r = inputrect;
			r.x = 0;
			r.y = 0;

			// INTERNAL_BOX(r, "");


			for ( int i = 0, Interator = 0; i < Math.Min( list.Count, LCOUNT ) && Interator < list.Count; i++, Interator++ )
			{
				var w = iconSize * 1.15f;
				var rect = new Rect(singleLineHeight + i * w /*+ 1.5f * i*/, (r.height - iconSize) / 2, w, iconSize);
				rect = new Rect( rect.x, rect.y + 3, rect.width, rect.height - 6 );
				Label( rect, "" );

				var cas = GUI.color;
				GUI.color *= list[ Interator ];
				if ( !bg )
				{
					GUI.DrawTexture( new Rect( rect.x, rect.y + 3, rect.width, rect.height - 6 ), Texture2D.whiteTexture );

				}
				else
				{
					GUI.DrawTexture( rect, Texture2D.whiteTexture );
				}
				GUI.color = cas;

				if ( Event.current.type == EventType.Repaint )
				{
					GUI.color *= Colors.SettingsBGColor;
					//   if ( !bg ) { HIGHLIGHTER_COLOR_FG.Draw( rect , ecc , 0 ); }
					GUI.color = cas;
				}

				interator++;
				EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

				if ( Button( rect, "" ) )
				{
					result = list[ Interator ];
					list.Insert( 0, result );
					list.RemoveAt( Interator + 1 );
					HighLighterCommonData.SetDirty();
					/*                    var new32 = Hierarchy_GUI.GetLastHiglightList[Interator];
										getcolor.list[0] = new32.r;
										getcolor.list[1] = new32.g;
										getcolor.list[2] = new32.b;
										getcolor.list[3] = new32.a;
										UpdateLastHiglightColors(new32);
										SetHiglightColor(getcolor);*/
				}
			}


			GUI.EndGroup();
			GUI.enabled = oldEnable;

		}

	}
}
