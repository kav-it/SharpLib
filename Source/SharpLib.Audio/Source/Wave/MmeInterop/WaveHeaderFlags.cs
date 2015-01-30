using System;

namespace NAudio.Wave
{
    [Flags]
    internal enum WaveHeaderFlags
    {
        BeginLoop = 0x00000004,

        Done = 0x00000001,

        EndLoop = 0x00000008,

        InQueue = 0x00000010,

        Prepared = 0x00000002
    }
}