using System;

namespace SharpLib.Audio.Wave.Compression
{
    [Flags]
    internal enum AcmDriverDetailsSupportFlags
    {
        Codec = 0x00000001,

        Converter = 0x00000002,

        Filter = 0x00000004,

        Hardware = 0x00000008,

        Async = 0x00000010,

        Local = 0x40000000,

        Disabled = unchecked((int)0x80000000),
    }
}