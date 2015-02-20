using System;

namespace SharpLib.Docking.Layout
{
    [Flags]
    public enum AnchorableShowStrategy : byte
    {
        Most = (1 << 0),

        Left = (1 << 1),

        Right = (1 << 2),

        Top = (1 << 4),

        Bottom = (1 << 5),
    }
}