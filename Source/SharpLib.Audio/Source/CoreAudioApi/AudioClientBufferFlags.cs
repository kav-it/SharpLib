using System;

namespace SharpLib.Audio.CoreAudioApi
{
    [Flags]
    internal enum AudioClientBufferFlags
    {
        None,

        DataDiscontinuity = 0x1,

        Silent = 0x2,

        TimestampError = 0x4
    }
}