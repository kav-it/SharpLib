using System;

namespace NAudio.Dmo
{
    [Flags]
    internal enum DmoProcessOutputFlags
    {
        None,

        DiscardWhenNoBuffer = 0x00000001
    }
}