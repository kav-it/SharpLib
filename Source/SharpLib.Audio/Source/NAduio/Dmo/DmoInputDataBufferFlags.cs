using System;

namespace SharpLib.Audio.Dmo
{
    [Flags]
    internal enum DmoInputDataBufferFlags
    {
        None,

        SyncPoint = 0x00000001,

        Time = 0x00000002,

        TimeLength = 0x00000004
    }
}