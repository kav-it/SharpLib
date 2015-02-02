using System;

namespace SharpLib.Audio.MediaFoundation
{
    [Flags]
    internal enum _MFT_PROCESS_OUTPUT_FLAGS
    {
        None,

        MFT_PROCESS_OUTPUT_DISCARD_WHEN_NO_BUFFER = 0x00000001,

        MFT_PROCESS_OUTPUT_REGENERATE_LAST_OUTPUT = 0x00000002
    }
}