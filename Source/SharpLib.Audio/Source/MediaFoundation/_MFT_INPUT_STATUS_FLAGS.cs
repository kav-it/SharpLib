using System;

namespace SharpLib.Audio.MediaFoundation
{
    [Flags]
    internal enum _MFT_INPUT_STATUS_FLAGS
    {
        None = 0,

        MFT_INPUT_STATUS_ACCEPT_DATA = 0x00000001
    }
}