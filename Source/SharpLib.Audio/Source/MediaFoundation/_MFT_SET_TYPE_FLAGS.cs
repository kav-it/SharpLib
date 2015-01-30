using System;

namespace SharpLib.Audio.MediaFoundation
{
    [Flags]
    internal enum _MFT_SET_TYPE_FLAGS
    {
        None = 0,

        MFT_SET_TYPE_TEST_ONLY = 0x00000001
    }
}