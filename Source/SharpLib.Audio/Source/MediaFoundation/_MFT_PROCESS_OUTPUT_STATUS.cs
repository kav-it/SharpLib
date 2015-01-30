using System;

namespace NAudio.MediaFoundation
{
    [Flags]
    internal enum _MFT_PROCESS_OUTPUT_STATUS
    {
        None = 0,

        MFT_PROCESS_OUTPUT_STATUS_NEW_STREAMS = 0x00000100
    }
}