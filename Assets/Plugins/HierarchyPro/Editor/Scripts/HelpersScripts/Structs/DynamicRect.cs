using System;
using UnityEngine;
using UnityEditor;
using EMX.HierarchyPlugin.Editor.Mods;

namespace EMX.HierarchyPlugin.Editor
{
#if EMX_H_LITE
    class HighlighterMod
    {
        public PluginInstance adapter { get { return Root.p[ 0 ]; } }
        public HighlighterMod_Set set = new HighlighterMod_Set();
    }
    class HighlighterMod_Set
    {
        public int HIGHLIGHTER_BGCOLOR_PADDING { get { return 0; } }
        public int HIGHLIGHTER_TEXTURE_GROW { get { return 0; } }
    }
#endif
	internal class DynamicRect
    {
        internal void Set( Rect selection, Rect fade, bool isMain, HierarchyObject o, bool HasIcon, float MinLeft )
        {
            ref_selectionRect = selection;
            ref_fadeRect = fade;
            this.isMain = isMain;
            this.HasIcon = HasIcon;
            this.o = o;
            this.MinLeft = MinLeft;
            ref_selectionRect.x += MinLeft;
        }


        internal HighlighterMod mod;
        internal PluginInstance adapter { get { return mod.adapter; } }
        //	internal HighlighterMod hl;
        internal DynamicRect( HighlighterMod p )
        {

            mod = p;
#if EMX_H_LITE
            if ( mod == null ) mod = new HighlighterMod();
#endif

            //this.hl = hl;
        }

        internal Rect ref_selectionRect;
        internal Rect ref_fadeRect;
        internal bool isMain;
        internal bool HasIcon;
        internal HierarchyObject o;
        internal float? labelSize = null;
        internal float labelSizeFont = 0;
        internal float MinLeft;

        static float MinHeight;
        static float MaxHeight;
        internal float GET_LEFT( BgAligmentLeft align )
        {
            switch ( align )
            {
                case BgAligmentLeft.MinLeft: return MinLeft;

                case BgAligmentLeft.Fold:
                    {
                        var res = ref_selectionRect.x - EditorGUIUtility.singleLineHeight;

                        if ( isMain && UnityVersion.UNITY_CURRENT_VERSION >= UnityVersion.UNITY_2019_2_0_VERSION )
                            res -= adapter.ha.LEFT_PADDING;

                        return res;
                    }

                /*  if (_S_bgIconsPlace != 0) return ref_selectionRect.x - EditorGUIUtility.singleLineHeight ;
                  else return ref_selectionRect.x;*/
                case BgAligmentLeft.BeginLabel:
                    {
                        var res = ref_selectionRect.x;

                        //if (HasIcon) res += adapter.par_e.HIGHLIGHTER_DEFAULT_ICON_SIZE;
                        if ( HasIcon ) res += adapter.DEFAULT_ICON_SIZE;

                        if ( isMain && UnityVersion.UNITY_CURRENT_VERSION >= UnityVersion.UNITY_2019_2_0_VERSION )
                            res -= adapter.ha.LEFT_PADDING;

                        return res;
                    }

                case BgAligmentLeft.EndLabel:
                    { /*if ( adapter.M_CustomIcontsEnable && adapter.par.ENABLE_RIGHTDOCK_FIX && par.COMPONENTS_NEXT_TO_NAME )
					{   var pos = adapter.M_CustomIconsModule.GetIconPos( o.id );
					if (pos != -1 )
					{   return pos;
					}
					}*/
                        if ( !labelSize.HasValue || labelSizeFont != adapter.labelStyle.fontSize )
                        {
                            labelSizeFont = adapter.labelStyle.fontSize;
                            adapter.labelStyle.CalcMinMaxWidth( Tools.GET_CONTENT( o.name ), out MinHeight, out MaxHeight );
                        }
                        else MinHeight = labelSize.Value;

                        var res = ref_selectionRect.x + MinHeight;

                        if ( isMain && UnityVersion.UNITY_CURRENT_VERSION >= UnityVersion.UNITY_2019_2_0_VERSION ) res -= adapter.ha.LEFT_PADDING;

                        //	if (HasIcon) return res + adapter.par_e.HIGHLIGHTER_DEFAULT_ICON_SIZE + EditorGUIUtility.singleLineHeight / 3;
                        if ( HasIcon ) return res + adapter.DEFAULT_ICON_SIZE + EditorGUIUtility.singleLineHeight / 3;

                        return res;
                    }

                case BgAligmentLeft.Modules: return ref_fadeRect.x;
            }

            throw new Exception();
        }


        internal float GET_RIGHT( BgAligmentRight align )
        {
            switch ( align )
            {
                case BgAligmentRight.Icon: return GET_LEFT( BgAligmentLeft.Fold );

                case BgAligmentRight.BeginLabel: return GET_LEFT( BgAligmentLeft.BeginLabel );

                case BgAligmentRight.EndLabel: return GET_LEFT( BgAligmentLeft.EndLabel );

                case BgAligmentRight.Modules: return GET_LEFT( BgAligmentLeft.Modules );

                case BgAligmentRight.MaxRight:
                    return ref_selectionRect.x + ref_selectionRect.width - adapter.ha.LEFT_PADDING + adapter.ha.PREFAB_BUTTON_SIZE;

                case BgAligmentRight.WidthFixedGradient: return -1;
            }

            throw new Exception();
        }


        static Rect tr = new Rect();

        internal Rect ConvertToBGFromTempColor( TempColorClass tempColor )
        {
            tr.x = GET_LEFT( tempColor.BG_ALIGMENT_LEFT_CONVERTED );
            tr.width = GET_RIGHT( tempColor.BG_ALIGMENT_RIGHT_CONVERTED ) - tr.x;

            //if (tempColor.BG_ALIGMENT_LEFT_CONVERTED == BgAligmentLeft.BeginLabel && adapter.par_e.HIGHLIGHTER_USE_LABEL_OFFSET)
            { /* tr.x -= 3;
				tr.width += 3;*/
            }

            if ( tempColor.BG_ALIGMENT_RIGHT_CONVERTED == BgAligmentRight.WidthFixedGradient )
                tr.width = tempColor.BG_WIDTH * 2;

            tr.y = ref_selectionRect.y;
            tr.height = ref_selectionRect.height;

            if ( tempColor.BG_HEIGHT == 1 )
            {
                var h = adapter.labelStyle.CalcHeight(Tools.GET_CONTENT(o.name), 10000) - 4 - mod.set.HIGHLIGHTER_TEXTURE_GROW;
                var d = (ref_selectionRect.height - h);
                tr.y += d / 2;
                tr.height -= d;
            }

            else if ( tempColor.BG_HEIGHT == 2 )
            {
                //tr.y += tr.height / 2;
                tr.height = 1;
            }

            else
            {
                var odd = Mathf.Abs(mod.set.HIGHLIGHTER_BGCOLOR_PADDING) % 2;
                var div = mod.set.HIGHLIGHTER_BGCOLOR_PADDING / 2;
                tr.y += div;
                tr.height -= div * 2 + odd;
            }

            return tr;
        }

    }

}
