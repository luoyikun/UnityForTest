namespace EMX.HierarchyPlugin.Editor
{

	internal enum BgAligmentLeft
    {
        MinLeft = 0,
        Fold = 1,
        BeginLabel = 2,
        EndLabel = 3,
        Modules = 4
    }
       internal enum BgAligmentRight
        {
            MaxRight = 0,
            Modules = 1,
            EndLabel = 2,
            BeginLabel = 3,
            Icon = 4,
            WidthFixedGradient = 5
        }
    internal partial class PluginInstance
    {



        internal static BgAligmentLeft[] BgAligmentLeftArray = {BgAligmentLeft.MinLeft, BgAligmentLeft.Fold, BgAligmentLeft.BeginLabel, BgAligmentLeft.EndLabel, BgAligmentLeft.Modules};


        internal BgAligmentLeft BgAligmentToLeft( int source )
        {
            return (BgAligmentLeft)(source & 7);
        }

        internal int BgLeftToAligment( int source, BgAligmentLeft leftAligment )
        {
            return source & ~(7) | (int)leftAligment;
        }


        internal static BgAligmentRight[] BgAligmentToRightArray = {BgAligmentRight.MaxRight, BgAligmentRight.Modules, BgAligmentRight.EndLabel, BgAligmentRight.BeginLabel, BgAligmentRight.Icon, BgAligmentRight.WidthFixedGradient};

        internal BgAligmentRight BgAligmentToRight( int source )
        {
            return (BgAligmentRight)((source & (7 << 3) >> 3));
        }

        internal int BgRightToAligment( int source, BgAligmentRight leftAligment )
        {
            return source & ~(7 << 3) | ((int)leftAligment << 3);
        }

        internal enum BgAligmentHeight
        {
            FullHeight = 0,
            Narrow = 1
        }

        internal BgAligmentHeight BgAligmentToHeight( int source )
        {
            return (BgAligmentHeight)((source & (1 << 6) >> 6));
        }

        internal int BgHeightToAligment( int source, BgAligmentHeight heightAligment )
        {
            return source & ~(1 << 6) | ((int)heightAligment << 6);
        }

    }
}
