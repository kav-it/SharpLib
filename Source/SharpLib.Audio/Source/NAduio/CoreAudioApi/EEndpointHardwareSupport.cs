using System;

namespace SharpLib.Audio.CoreAudioApi
{
    [Flags]
    internal enum EEndpointHardwareSupport
    {
        Volume = 0x00000001,

        Mute = 0x00000002,

        Meter = 0x00000004
    }
}