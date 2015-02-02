using System;

namespace SharpLib.Audio.Dmo
{
    [Flags]
    internal enum DmoProcessOutputFlags
    {
        None,

        DiscardWhenNoBuffer = 0x00000001
    }
}