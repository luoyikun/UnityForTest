namespace EMX.HierarchyPlugin.Editor
{
	class UNDO
    {


        internal static bool wasUndo;
       // internal Action onUndoAction;
        internal void UNDO_AC( )
        {
            UNDO.wasUndo = true;

            //bool anyEnabled = false;
            //for ( int i = 0 ; i < Root.p.Length ; i++ )
            //{
            //    if ( Root.p[ i ] == null ) continue;
            //    if ( !Root.p[ i ].par_e.ENABLE_ALL ) continue;
            //    Root.p[ i ].wereUndoForLockerMod = true;
            //    anyEnabled = true;
            //}
            //if ( !anyEnabled ) return;
            // bottomInterface.NeedRefreshBOttom = true;

            //if ( onUndoAction != null ) onUndoAction();

            //for ( int i = 0 ; i < Root.p.Length ; i++ )
            //{
            //    if ( Root.p[ i ] == null ) continue;
            //    Root.p[ i ].RepaintWindow( 0, true );
            //    Root.p[ i ].ha.InternalClearDrag();
            //}
        }
    }
}
