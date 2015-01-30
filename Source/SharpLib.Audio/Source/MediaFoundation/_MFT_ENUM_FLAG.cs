using System;

namespace SharpLib.Audio.MediaFoundation
{
    [Flags]
    internal enum _MFT_ENUM_FLAG
    {
        None = 0,

        MFT_ENUM_FLAG_SYNCMFT = 0x00000001,

        MFT_ENUM_FLAG_ASYNCMFT = 0x00000002,

        MFT_ENUM_FLAG_HARDWARE = 0x00000004,

        MFT_ENUM_FLAG_FIELDOFUSE = 0x00000008,

        MFT_ENUM_FLAG_LOCALMFT = 0x00000010,

        MFT_ENUM_FLAG_TRANSCODE_ONLY = 0x00000020,

        MFT_ENUM_FLAG_SORTANDFILTER = 0x00000040,

        MFT_ENUM_FLAG_ALL = 0x0000003F
    }
}