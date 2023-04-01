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
        // internal  const int BUT_WIDTH = 48;
        //internal const int BUT_WIDTH = 80;
        internal int BUT_WIDTH {
            get {
                return adapter.par_e.COMPONENTS_ATTRIBUTES_NAME_FOR_BUTTONS ? 80 : 16;
            }
        }
        // internal const int FIELD_WIDTH = 32;
        internal const int FIELD_WIDTH = 80;

        internal class MyAttribute
        {
            internal MyAttribute( string name, float? width, Color? color )
            {
                this.tooltip = this.name = name;
                this.width = width;
                this.color = color;
            }

            internal bool IsStatic = false;
            internal string name;
            internal string tooltip;
            internal float? width;
            internal Color? color = null;
        }

        internal class AttributeButton : MyAttribute
        {
            internal AttributeButton( string name, string tooltip, float? width, Color? color ) : base( name, width, color )
            {
                this.tooltip = tooltip;
            }
            internal object[] parameters;

            internal MethodInfo method;
        }

        internal class AttributeField : MyAttribute
        {
            internal AttributeField( string name, float? width, Color? color ) : base( name, width, color ) { }

            internal bool IsProperty = false;
            internal FieldInfo field;
            internal PropertyInfo property;
        }

        AttrArgs attrArgs;

        internal struct AttrArgs
        {
            internal Component script;
            internal AttributeButton el;
            internal AttributeField field;
            internal  Rect? clampRect;
            internal bool isNull;
            internal string content;
        }
        internal override void ResetStack( int id, bool disableLog = false )
        {
            buttons.Clear();
            fields.Clear();
            base.ResetStack( id, disableLog );
        }
        internal override void ResetStack()
        {
            buttons.Clear();
            fields.Clear();
            base.ResetStack();
        }

        GUIContent emptyContent = new GUIContent();
        Dictionary<Type, List<AttributeButton>> buttons = new Dictionary<Type, List<AttributeButton>>();
        Dictionary<Type, List<AttributeField>> fields = new Dictionary<Type, List<AttributeField>>();
        GUIStyle buttonArrtStyle, labelAttrStyle, textAttrStyle, textAttrStyleRedColor, popStyle;

        bool DrawAttributes( ref Rect cellRect, ref Rect iconRect, Type t, Component script )
        {
            if ( !adapter.par_e.COMPONENTS_ATTRIBUTES_BUTTONS && !adapter.par_e.COMPONENTS_ATTRIBUTES_FIELDS ) return false;

            if ( callFromExternal() ) return false;

            var key = t.Name;
            var wasDraw = false;
            ///// INITIALIZE

            if ( adapter.par_e.COMPONENTS_ATTRIBUTES_BUTTONS ) /** par.COMP_ATTRIBUTES_BUTTONS */
            {
                if ( !buttons.ContainsKey( t ) )
                {
                    buttons.Add( t, new List<AttributeButton>() );

                    foreach ( var methodInfo in t.GetMethods( ~(BindingFlags.GetField | BindingFlags.GetProperty) ) )
                    {
                        if ( methodInfo.IsGenericMethod ) continue;
                        var result = methodInfo.GetCustomAttributes(typeof(DRAW_IN_HIER), false);
                        if ( result != null )
                        {
                            foreach ( var o1 in result )
                            {
                                var o = (DRAW_IN_HIER)o1;
                                var value = new AttributeButton(
                                    !adapter.par_e.COMPONENTS_ATTRIBUTES_NAME_FOR_BUTTONS ? "●" : methodInfo.Name,
                                     methodInfo.Name
                                    , o.width, o.color) { IsStatic = methodInfo.IsStatic, method = methodInfo };
                                value.parameters = methodInfo.GetParameters().Select( p => {
                                    if ( !p.ParameterType.IsClass )
                                    {
                                        return Activator.CreateInstance( p.ParameterType );
                                    }

                                    return null;
                                }
                                ).ToArray();
                                /*  foreach (var p in value.parameters) {
                                      MonoBehaviour.print(p.GetType());
                                  }*/
                                buttons[ t ].Add( value );
                            }
                        }
                    }
                }
            } /** par.COMP_ATTRIBUTES_BUTTONS */


            if ( adapter.par_e.COMPONENTS_ATTRIBUTES_FIELDS ) /** par.COMP_ATTRIBUTES_FIELDS */
            {
                if ( !fields.ContainsKey( t ) )
                {
                    fields.Add( t, new List<AttributeField>() );

                    foreach ( var propertyInfo in t.GetProperties( ~(BindingFlags.InvokeMethod) ) )
                    {
                        if ( !propertyInfo.CanRead ) continue;
                        var result = propertyInfo.GetCustomAttributes(typeof(DRAW_IN_HIER), false);
                        if ( result != null )
                        {
                            foreach ( var o1 in result )
                            {
                                var o = (DRAW_IN_HIER)o1;
                                var value = new AttributeField(propertyInfo.Name, o.width, o.color) { IsStatic = propertyInfo.GetAccessors(true)[0].IsStatic, IsProperty = true, property = propertyInfo };
                                fields[ t ].Add( value );
                            }
                        }
                    }

                    foreach ( var fieldInfo in t.GetFields( ~(BindingFlags.InvokeMethod) ) )
                    {
                        var result = fieldInfo.GetCustomAttributes(typeof(DRAW_IN_HIER), false);
                        if ( result != null )
                        {
                            foreach ( var o1 in result )
                            {
                                var o = (DRAW_IN_HIER)o1;
                                var value = new AttributeField(fieldInfo.Name, o.width, o.color) { IsStatic = fieldInfo.IsStatic, IsProperty = false, field = fieldInfo };
                                fields[ t ].Add( value );
                            }
                        }
                    }
                }
            } /** par.COMP_ATTRIBUTES_FIELDS */

            ///// INITIALIZE
            var width = 0f;
            // cellRect.x += adapter.par_e.COMPONENTS_ATTRIBUTES_MARGIN;
            //att_Rect = drawRect;
            var HEIGHT = cellRect.height;
            var Y = cellRect.y;
            var newH = Math.Max(12, HEIGHT - 8);
            att_Rect.y = Y + (HEIGHT - newH) / 2;
            att_Rect.height = newH;

            /* att_Rect.y = Y;
             att_Rect.height = HEIGHT;*/
            //  att_Rect.y -= 2;

            if ( buttonArrtStyle == null )
            {
                buttonArrtStyle = new GUIStyle( adapter.button );
                // buttonArrtStyle = new GUIStyle(EditorStyles.toolbarButton);
                buttonArrtStyle.fixedHeight = 0;
                buttonArrtStyle.stretchHeight = true;
                buttonArrtStyle.alignment = TextAnchor.MiddleCenter;
                buttonArrtStyle.clipping = TextClipping.Clip;
                buttonArrtStyle.normal.textColor = EditorGUIUtility.isProSkin ? TCCC : TCCC2;
                buttonArrtStyle.padding = new RectOffset( 2, 2, 2, 2 );
                buttonArrtStyle.padding.right = 1;
            }

            buttonArrtStyle.fontSize = adapter.FONT_8_FORICONSMODS(); //Mathf.RoundToInt( adapter.button.fontSize * 0.8f );
            if ( labelAttrStyle == null )
            {
                labelAttrStyle = new GUIStyle( adapter.label );
                labelAttrStyle.alignment = TextAnchor.MiddleCenter;
                labelAttrStyle.clipping = TextClipping.Clip;
            }

            labelAttrStyle.fontSize = buttonArrtStyle.fontSize;
            if ( textAttrStyle == null )
            {
                textAttrStyle = new GUIStyle( adapter.GET_SKIN().textField );
                textAttrStyle.alignment = TextAnchor.MiddleCenter;
                textAttrStyle.clipping = TextClipping.Clip;
            }

            textAttrStyle.fontSize = buttonArrtStyle.fontSize;
            if ( textAttrStyleRedColor == null )
            {
                textAttrStyleRedColor = new GUIStyle( textAttrStyle );
                textAttrStyleRedColor.normal.textColor = Color.red;
            }

            textAttrStyleRedColor.fontSize = buttonArrtStyle.fontSize;
            if ( popStyle == null )
            {
                popStyle = new GUIStyle( EditorStyles.popup );
                popStyle.alignment = TextAnchor.MiddleCenter;
                popStyle.clipping = TextClipping.Clip;
                popStyle.fixedHeight = 0;
                popStyle.stretchHeight = true;
            }

            popStyle.fontSize = buttonArrtStyle.fontSize;


            if ( adapter.par_e.COMPONENTS_ATTRIBUTES_BUTTONS )
            {
                var list = buttons[t];
                var col = Color.white;
                for ( int i = 0; i < list.Count; i++ )
                {
                    wasDraw = true;

                    att_content.text = list[ i ].name;
                    att_content.tooltip = list[ i ].tooltip;
                    att_Rect.width = Math.Min( BUT_WIDTH, buttonArrtStyle.CalcSize( att_content ).x );

                    if ( list[ i ].color.HasValue ) col *= list[ i ].color.Value;

                    if ( width == 0 ) width = adapter.par_e.COMPONENTS_ATTRIBUTES_MARGIN;
                    att_Rect.x = cellRect.x + width;

                   attrArgs.clampRect = _drawRect;
                    attrArgs.script = script;
                    attrArgs.el = list[ i ];

                   // att_Rect.width = GetClampedRect( att_Rect );

                    Draw_Action( att_Rect, BUTTON_REPAINT_ACTION_HASH, attrArgs, content: att_content );


                    // Draw_Style(att_Rect, style, col, true);
                    // att_content.text = null;


                    //  attrArgs.content = att_content.text;
                    var mRect = att_Rect;
                    // mRect.width = Math.Min(mRect.x + mRect.width, adapter.fullLineRect.x + adapter.fullLineRect.width - adapter.rightOffset) - mRect.x;
                    mRect.width = GetClampedRect_Event( mRect );
                    // Debug.Log(mRect.width);
                    if ( mRect.width > 0 ) Draw_ModuleButton( mRect, null, ATTR_BUT_HASH, true, attrArgs, true, buttonArrtStyle, USE_GO: true, clampRect: _drawRect );

                    att_content.text = null;
                    if ( mRect.width > 0 ) Draw_Label( att_Rect, att_content, labelAttrStyle, true, col, clampRect: _drawRect );

                    width += att_Rect.width;
                }

                //**//GUI.color = c;
            }

            if ( adapter.par_e.COMPONENTS_ATTRIBUTES_FIELDS )
            {
                var list = fields[t];
                //***// var c = GUI.color;
                var col = Color.white;
                for ( int i = 0; i < list.Count; i++ )
                {
                    wasDraw = true;
                    var isnull = false;
                    var hasSetter = false;
                    if ( list[ i ].IsProperty )
                    {
                        object result = null;
                        try
                        {
                            result = list[ i ].property.GetValue( list[ i ].IsStatic ? null : script, null );
                        }
                        catch { }

                        att_content.text = (isnull = Tools.IsObjectNull( /*list[i].property.PropertyType ,*/ result )) ? "null" : result.ToString();
                        hasSetter = list[ i ].property.GetSetMethod() != null;
                    }
                    else
                    {
                        object result = list[i].field.GetValue(list[i].IsStatic ? null : script);

                        att_content.text = (isnull = Tools.IsObjectNull( /*list[i].field.FieldType ,*/ result )) ? "null" : result.ToString();

                        hasSetter = true;
                    }


                    // att_Rect.width = list[i].width;
                    var captureList = list[i];
                    var type = captureList.IsProperty ? captureList.property.PropertyType : captureList.field.FieldType;
                    att_content.tooltip = list[ i ].name + ": " + att_content.text;
                    if ( type.IsEnum )
                        att_Rect.width = Math.Min( FIELD_WIDTH, popStyle.CalcSize( att_content ).x + popStyle.padding.left );
                    else
                        att_Rect.width = Math.Min( FIELD_WIDTH, textAttrStyle.CalcSize( att_content ).x );

                    //  GUI.color = /*c **/ (list[i].color ?? Color.white);
                    if ( list[ i ].color.HasValue ) col *= list[ i ].color.Value;


                    if ( width == 0 ) width = adapter.par_e.COMPONENTS_ATTRIBUTES_MARGIN;
                    att_Rect.x = cellRect.x + width;

                    if ( hasSetter ) //var newText = EditorGUI.TextField(att_Rect, att_content.text );
                    { //if (newText != att_content.text)


                        attrArgs.script = script;
                        attrArgs.field = captureList;
                        attrArgs.isNull = isnull;
                        attrArgs.content = att_content.text;
                        Draw_Action( att_Rect, SETTER_REPAINT_ACTION_HASH, attrArgs, content: att_content );



                        var mRect = att_Rect;
                        // mRect.width = Math.Min( mRect.x + mRect.width, adapter.fullLineRect.x + adapter.fullLineRect.width - adapter.rightOffset ) - mRect.x;
                        mRect.width = GetClampedRect_Event( mRect );

                        if ( mRect.width > 0 ) Draw_ModuleButton( mRect, null, SETTER_BUT_HASH, true, attrArgs, false, null, USE_GO: true );


                        // if (i==1) Debug.Log(mRect.width);
                        // GUI.Label( att_Rect, att_content, labelAttrStyle);
                        att_content.text = null;
                        if ( mRect.width > 0 ) Draw_Label( att_Rect, att_content, labelAttrStyle, true, col );
                    }
                    else
                    {
                        var ccc = textAttrStyle;
                        if ( adapter.par_e.COMPONENTS_ATTRIBUTES_DISPLAYING_NULLSISRED && isnull ) ccc = textAttrStyleRedColor;
                        Draw_Label( att_Rect, att_content, ccc, true, col );
                    }

                    width += att_Rect.width;
                }

                //**// GUI.color = c;
            }


            iconRect.x += width;
            cellRect.x += width;
            return wasDraw;
            // return width;
        }



        Dictionary<Type, List<string>> enumToNames = new Dictionary<Type, List<string>>();

        static Rect att_Rect = new Rect();
        static GUIContent att_content = new GUIContent();
        static Color TCCC = new Color(0.85f, 0.94f, 0.94f, 1);
        static Color TCCC2 = new Color(1 - 0.85f, 1 - 0.94f, 1 - 0.94f, 1);


        DrawStackMethodsWrapper __ATTR_BUT_HASH = null;

        DrawStackMethodsWrapper ATTR_BUT_HASH {
            get { return __ATTR_BUT_HASH ?? (__ATTR_BUT_HASH = new DrawStackMethodsWrapper( ATTR_BUT )); }
        }

        void ATTR_BUT( Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o )
        {
            var args = (AttrArgs)data.args;
            var el = args.el;
            adapter.PUSH_UPDATE_ONESHOT( 0, () => {
                if ( el.IsStatic ) el.method.Invoke( null, el.parameters );
                else if ( args.script ) el.method.Invoke( args.script, el.parameters );
                adapter.RepaintWindow( 0 );
            } );
        }

        DrawStackMethodsWrapper __SETTER_BUT_HASH = null;

        DrawStackMethodsWrapper SETTER_BUT_HASH {
            get { return __SETTER_BUT_HASH ?? (__SETTER_BUT_HASH = new DrawStackMethodsWrapper( SETTER_BUT )); }
        }

        void SETTER_BUT( Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o )
        {
            var args = (AttrArgs)data.args;
            // var att_content = args.content;
#pragma warning disable
            var att_content = data.content;
#pragma warning restore
            var captureList = args.field;
            var script = captureList.IsStatic ? null : args.script;
            var type = captureList.IsProperty ? captureList.property.PropertyType : captureList.field.FieldType;
            var allowUndo = script && !captureList.IsStatic && captureList.IsStatic && !Application.isPlaying && (!captureList.IsProperty || type.IsEnum);
            //   if ( Button( att_Rect, "" ) )
            {
                /* var captureList = list[i];
                  var type = captureList.IsProperty ? captureList.property.PropertyType : captureList.field.FieldType;*/


                if ( type.IsEnum )
                {
                    if ( !enumToNames.ContainsKey( type ) )
                    {
                        enumToNames.Add( type, Enum.GetNames( type ).ToList() );
                    }

                    var ind = enumToNames[type].IndexOf(args.content);
                    ind = Mathf.Clamp( ind, 0, enumToNames[ type ].Count - 1 );

                    CustomHierarchyMod.SHOW_DropDownMenu( ind, Enum.GetNames( type ), ( newValue ) => {
                        if ( !captureList.IsStatic && !script ) return;
                        var nv = Enum.Parse(type, Enum.GetNames(type)[newValue]);
                        if ( newValue != ind )
                        {
                            if ( allowUndo ) Undo.RecordObject( script, "Change Field" );
                            if ( captureList.IsProperty )
                                captureList.property.SetValue( captureList.IsStatic ? null : script, nv, null );
                            else
                                captureList.field.SetValue( captureList.IsStatic ? null : script, nv );
                            if ( script && !Application.isPlaying ) EditorUtility.SetDirty( script );
                            if ( allowUndo ) adapter.MarkSceneDirty( script.gameObject.scene );
                            ResetStack();
                        }
                    } );
                }
                else if ( type == typeof( int ) )
                {
                    CustomHierarchyMod.SHOW_StringInput( args.content, ( newValue ) => {
                        int result;
                        if ( !captureList.IsStatic && !script ) return;
                        if ( int.TryParse( newValue, out result ) )
                        {
                            if ( allowUndo ) Undo.RecordObject( script, "Change Field" );
                            if ( captureList.IsProperty )
                                captureList.property.SetValue( captureList.IsStatic ? null : script, result, null );
                            else
                                captureList.field.SetValue( captureList.IsStatic ? null : script, result );
                            if ( script && !Application.isPlaying ) EditorUtility.SetDirty( script );
                            if ( allowUndo ) adapter.MarkSceneDirty( script.gameObject.scene );
                            ResetStack();
                        }
                    } );
                }
                else if ( type == typeof( float ) ) //float result;
                { // if (float.TryParse(newText, out result)) captureList.field.SetValue(captureList.IsStatic ? null : script, result);
                    CustomHierarchyMod.SHOW_StringInput( args.content, ( newValue ) => {
                        float result;
                        if ( !captureList.IsStatic && !script ) return;
                        if ( float.TryParse( newValue, out result ) )
                        {
                            if ( allowUndo ) Undo.RecordObject( script, "Change Field" );
                            if ( captureList.IsProperty )
                                captureList.property.SetValue( captureList.IsStatic ? null : script, result, null );
                            else
                                captureList.field.SetValue( captureList.IsStatic ? null : script, result );
                            if ( script && !Application.isPlaying ) EditorUtility.SetDirty( script );
                            if ( allowUndo ) adapter.MarkSceneDirty( script.gameObject.scene );
                            ResetStack();
                        }
                    } );
                }
                else if ( type == typeof( string ) )
                {
                    CustomHierarchyMod.SHOW_StringInput( args.content, ( newValue ) => {
                        if ( !captureList.IsStatic && !script ) return;

                        if ( allowUndo ) Undo.RecordObject( script, "Change Field" );
                        if ( captureList.IsProperty )
                            captureList.property.SetValue( captureList.IsStatic ? null : script, newValue, null );
                        else
                            captureList.field.SetValue( captureList.IsStatic ? null : script, newValue );
                        if ( script && !Application.isPlaying ) EditorUtility.SetDirty( script );
                        if ( allowUndo ) adapter.MarkSceneDirty( script.gameObject.scene );
                        ResetStack();
                    } );
                }


                //Undo.RecordObject( _o.GetHardLoadObject() , "Change Field" );
            }
        }




        DrawStackMethodsWrapper __SETTER_REPAINT_ACTION_HASH = null;

        DrawStackMethodsWrapper SETTER_REPAINT_ACTION_HASH {
            get { return __SETTER_REPAINT_ACTION_HASH ?? (__SETTER_REPAINT_ACTION_HASH = new DrawStackMethodsWrapper( SETTER_REPAINT_ACTION )); }
        }

        void SETTER_REPAINT_ACTION( Rect worldOffset, Rect att_Rect, DrawStackMethodsWrapperData data, HierarchyObject _o )
        {
            if ( att_Rect.width <= 0 ) return;
            if ( EVENT.type == EventType.Repaint )
            {
                var args = (AttrArgs)data.args;
                var cl = args.clampRect  ;
               // var cl = attrArgs.clampRect as Rect? ?? Rect.zero;
               //var stack = data.stackItem as DrawStackItem;
               //cl.x += stack.localRect.x;
               //cl.y += stack.localRect.y;

                var att_content = data.content;
                var captureList = args.field;
                var isnull = (bool)args.isNull;
                att_content.tooltip = null;
               // var c = GUI.color;
               // if ( !_o.Active() ) GUI.color *= new Color( 1, 1, 1, 0.5f );
                var type = captureList.IsProperty ? captureList.property.PropertyType : captureList.field.FieldType;
                if ( type.IsEnum )
                {
                    // popStyle.Draw( att_Rect, att_content, false, false, false, false );
                    adapter.gl._DrawStyleWithText( att_Rect, popStyle, att_content, TextClipping.Clip, !MouseClamped, cl ,_o);
                }
                else
                {
                    var ccc = textAttrStyle;
                    if ( adapter.par_e.COMPONENTS_ATTRIBUTES_DISPLAYING_NULLSISRED && isnull ) ccc = textAttrStyleRedColor;
                    // ccc.Draw( att_Rect, att_content, false, false, false, false );
                    adapter.gl._DrawStyleWithText( att_Rect, ccc, att_content, TextClipping.Clip, !MouseClamped, cl,_o );
                }

                //GUI.color = c;
            }

            if ( !MouseClamped ) EditorGUIUtility.AddCursorRect( att_Rect, MouseCursor.Link );
        }


        DrawStackMethodsWrapper __BUTTON_REPAINT_ACTION_HASH = null;

        DrawStackMethodsWrapper BUTTON_REPAINT_ACTION_HASH {
            get { return __BUTTON_REPAINT_ACTION_HASH ?? (__BUTTON_REPAINT_ACTION_HASH = new DrawStackMethodsWrapper( BUTTON_REPAINT_ACTION )); }
        }

        void BUTTON_REPAINT_ACTION( Rect worldOffset, Rect att_Rect, DrawStackMethodsWrapperData data, HierarchyObject _o )
        {
            if ( att_Rect.width <= 0 ) return;
            if ( EVENT.type == EventType.Repaint )
            {
                var att_content = data.content;
                var args = (AttrArgs)data.args;
                var cl = args.clampRect ;
                //var cl = attrArgs.clampRect as Rect?;
               // var c = GUI.color;
               // if ( !_o.Active() ) GUI.color *= new Color( 1, 1, 1, 0.5f );
                // var style = EditorStyles.toolbarButton;
              
                adapter.gl._DrawStyleWithText( att_Rect, buttonArrtStyle, att_content, TextClipping.Clip, !MouseClamped, cl , _o);
               // GUI.color = c;
                if ( !MouseClamped ) EditorGUIUtility.AddCursorRect( att_Rect, MouseCursor.Link );
            }

        }

        // static GUIContent empty_content = new GUIContent();
    }
}
