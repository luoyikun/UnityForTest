using UnityEngine;



namespace EMX.HierarchyPlugin.Editor.Mods
{

	internal class new_child_struct
	{
		// internal Rect lastRect;
		Rect __rect;
		internal Rect rect { get { return __rect; } }
		internal void SetRect( Rect rect, bool isPrefab )
		{
			__rect = rect;
			isMaxRight = false;
			this.isPrefab = isPrefab;

			if ( !wasInit ) wasInit = true;
		}
#pragma warning disable
		internal bool wasInit;
		internal bool wasLastAssign;
		bool isPrefab = false;
		bool isMaxRight = false;
#pragma warning restore
		internal void SetMax( Rect value, Rect SelectionRect, PluginInstance adapter, TempColorClass tc )
		{
			if ( tc.BG_ALIGMENT_RIGHT_CONVERTED == BgAligmentRight.BeginLabel || tc.BG_ALIGMENT_RIGHT_CONVERTED == BgAligmentRight.Icon )
			{
				if ( value.x < __rect.x )
				{
					__rect.width += __rect.x - value.x;
					__rect.x = value.x;
				}

				if ( value.x + value.width < __rect.x + __rect.width ) __rect.width = value.x + value.width - __rect.x;
			}

			else
				if ( tc.BG_ALIGMENT_LEFT_CONVERTED == BgAligmentLeft.EndLabel )
			{
				if ( value.x > __rect.x )
				{
					__rect.width += __rect.x - value.x;
					__rect.x = value.x;
				}

				if ( value.x + value.width > __rect.x + __rect.width ) __rect.width = value.x + value.width - __rect.x;
			}

			else
			{
				if ( value.x < __rect.x )
				{
					__rect.width += __rect.x - value.x;
					__rect.x = value.x;
				}

				if ( value.x + value.width > __rect.x + __rect.width ) __rect.width = value.x + value.width - __rect.x;
			}


			if ( tc.BG_ALIGMENT_LEFT_CONVERTED == BgAligmentLeft.Modules && adapter.modsController.rightModsManager.headerEventsBlockRect.HasValue )
			{
				var off = adapter.modsController.rightModsManager.headerEventsBlockRect.Value;
				//if ( value.x > off.x ) value.x = off.x;
				__rect.x = off.x;
			}



			if ( value.y < __rect.y )
			{
				__rect.height += __rect.y - value.y;
				__rect.y = value.y;
			}

			if ( value.y + value.height > __rect.y + __rect.height ) __rect.height = value.y + value.height - __rect.y;

			isMaxRight = tc.BG_ALIGMENT_RIGHT_CONVERTED == BgAligmentRight.MaxRight;
			SelectionRect.width += adapter.ha.PREFAB_BUTTON_SIZE;

			if ( tc.BG_ALIGMENT_RIGHT_CONVERTED == BgAligmentRight.Modules && adapter.modsController.rightModsManager.headerEventsBlockRect.HasValue )
			{
				var off = adapter.modsController.rightModsManager.headerEventsBlockRect.Value;
				//if ( __rect.x + __rect.width > off.x ) __rect.width = off.x - __rect.x;
				__rect.width = off.x - __rect.x;
			}
			else
			{
				if ( __rect.x + __rect.width > SelectionRect.x + SelectionRect.width ) __rect.width = SelectionRect.x + SelectionRect.width - __rect.x;
			}

			if ( !wasLastAssign )
			{
				wasLastAssign = true;

				if ( __rect.y + __rect.height > SelectionRect.y + SelectionRect.height ) __rect.height = SelectionRect.y + SelectionRect.height - __rect.y;
			}
		}

		internal Rect ModifiedSelectionRect( PluginInstance adapter )
		{
			var res = SelectionRect;
			res.width += adapter.ha.PREFAB_BUTTON_SIZE;
			return res;
		}

		internal Rect SelectionRect;
		internal Rect LocalRect;
		internal new_child_struct ConvertToLocal( Rect SelectionRect, PluginInstance adapter )
		{
			LocalRect = __rect;

			if ( LocalRect.x + LocalRect.width > SelectionRect.x + SelectionRect.width + adapter.ha.PREFAB_BUTTON_SIZE ) LocalRect.width = SelectionRect.x + SelectionRect.width - LocalRect.x +
						adapter.ha.PREFAB_BUTTON_SIZE;


			//EMX_TODO local convertion disabled
			//LocalRect.x -= SelectionRect.x;
			//LocalRect.y -= SelectionRect.y;

			if ( adapter.ha.PREFAB_BUTTON_SIZE != 0 && !isPrefab )     //LocalRect.width += adapter.PREFAB_BUTTON_SIZE;
			{
			}


			this.SelectionRect = SelectionRect;
			return this;
		}
		// internal int lastObjject;
	}


}
