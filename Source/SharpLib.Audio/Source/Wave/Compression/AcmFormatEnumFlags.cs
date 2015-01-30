using System;

namespace NAudio.Wave.Compression
{
    [Flags]
    internal enum AcmFormatEnumFlags
    {
        None = 0,

        Convert = 0x00100000,

        Hardware = 0x00400000,

        Input = 0x00800000,

        Channels = 0x00020000,

        SamplesPerSecond = 0x00040000,

        Output = 0x01000000,

        Suggest = 0x00200000,

        BitsPerSample = 0x00080000,

        FormatTag = 0x00010000,
    }
}