using UnityEngine;



namespace EMX.HierarchyPlugin.Editor.Mods
{
	class HighlighterGetRect
	{

		static Rect tt;
		static PluginInstance adapter { get { return Root.p[0]; } }


		//static internal DynamicRect TryToFaeBG_Rect = new DynamicRect();

		//internal static Rect GetIconRectIfNextToLabel(Rect selectionRect, GetIconRectIfNextToLabelType type, HighlighterMod mod)
		//{
		//	mod.TryToFaeBG_Rect.ref_selectionRect = selectionRect;
		//
		//	mod.TryToFaeBG_Rect.ref_selectionRect.x += adapter.ha.LEFT_PADDING;
		//	// TryToFaeBG_Rect.HasIcon = _S_bgIconsPlace == 0;
		//	mod.TryToFaeBG_Rect.adapter = adapter;
		//	mod.TryToFaeBG_Rect.HasIcon = true;
		//	mod.TryToFaeBG_Rect.MinLeft = adapter.ha.LEFT_PADDING;
		//
		//	// var size = type == GetIconRectIfNextToLabelType.DefaultIcon ? adapter. DEFAULT_ICON_SIZE : EditorGUIUtility.singleLineHeight;
		//
		//	//	USE_OVERRIDE_DEFAULT_ICONS_SIZE
		//	//	var size = adapter.par_e.OVERRIDE_DEFAULT_ICONS_SIZE;
		//
		//	var size = (float)Window.k_IconWidth.GetValue(adapter.gui_currentTree);
		//
		//
		//	tt.Set(
		//			mod.TryToFaeBG_Rect.GET_LEFT(BgAligmentLeft.BeginLabel) - size,
		//			selectionRect.y,
		//			size, selectionRect.height);
		//
		//	//  tt.x += adapter.raw_old_leftpadding;
		//	size = Mathf.Min(selectionRect.height, size);
		//
		//	//  if (type == GetIconRectIfNextToLabelType.DefaultIcon) size = EditorGUIUtility.singleLineHeight;
		//
		//	tt.y += (tt.height - size) / 2;
		//	tt.height = Mathf.RoundToInt(size);
		//
		//	return tt;
		//}
		//static internal int lastIconPlace;
		//static internal Rect GetIconRect(Rect selectionRect, bool callFromExternal, HighlighterMod mod, int? overrideValue = null, int? overrideSBGIconPlace = null)
		//{
		//
		//
		//	var icon_place = overrideValue ?? (callFromExternal ? 2 : (overrideSBGIconPlace ?? adapter.par_e.HIGHLIGHTER_CUSTOM_ICON_LOCATION));
		//	lastIconPlace = icon_place;
		//	var icon_rect = Rect.zero;
		//
		//	switch (icon_place)
		//	{
		//		case 0:
		//			icon_rect = GetIconRectIfNextToLabel(selectionRect, GetIconRectIfNextToLabelType.CustomIcon, mod);
		//
		//			break;
		//
		//		case 1:
		//		case 2:
		//			if (icon_place == 2)
		//			{
		//				icon_rect.x = adapter.ha.LEFT_PADDING;
		//			}
		//
		//			else icon_rect.x = selectionRect.x - EditorGUIUtility.singleLineHeight * 2;
		//
		//			icon_rect.y = selectionRect.y;
		//			icon_rect.height = selectionRect.height;
		//			//  icon_rect.x += (icon_rect.width - icon_rect.width) / 2 + (adapter.par.COLOR_ICON_SIZE - 12) / 2f;
		//			icon_rect.width = EditorGUIUtility.singleLineHeight;
		//
		//			icon_rect.y += (icon_rect.height - EditorGUIUtility.singleLineHeight) / 2;
		//			icon_rect.height = EditorGUIUtility.singleLineHeight;
		//
		//			break;
		//	}
		//
		//	return icon_rect;
		//}



	}
}
