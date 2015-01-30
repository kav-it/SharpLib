using System;

namespace NAudio.MediaFoundation
{
    [Flags]
    internal enum _MFT_OUTPUT_STATUS_FLAGS
    {
        None = 0,

        MFT_OUTPUT_STATUS_SAMPLE_READY = 0x00000001
    }
}