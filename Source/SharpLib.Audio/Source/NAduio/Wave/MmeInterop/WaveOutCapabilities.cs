using System;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.Wave
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct WaveOutCapabilities
    {
        private readonly short manufacturerId;

        private readonly short productId;

        private readonly int driverVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxProductNameLength)]
        private readonly string productName;

        private readonly SupportedWaveFormat supportedFormats;

        private readonly short channels;

        private readonly short reserved;

        private readonly WaveOutSupport support;

        private readonly Guid manufacturerGuid;

        private readonly Guid productGuid;

        private readonly Guid nameGuid;

        private const int MaxProductNameLength = 32;

        public int Channels
        {
            get { return channels; }
        }

        public bool SupportsPlaybackRateControl
        {
            get { return (support & WaveOutSupport.PlaybackRate) == WaveOutSupport.PlaybackRate; }
        }

        public string ProductName
        {
            get { return productName; }
        }

        public bool SupportsWaveFormat(SupportedWaveFormat waveFormat)
        {
            return (supportedFormats & waveFormat) == waveFormat;
        }

        public Guid NameGuid
        {
            get { return nameGuid; }
        }

        public Guid ProductGuid
        {
            get { return productGuid; }
        }

        public Guid ManufacturerGuid
        {
            get { return manufacturerGuid; }
        }
    }

    [Flags]
    internal enum SupportedWaveFormat
    {
        WAVE_FORMAT_1M08 = 0x00000001,

        WAVE_FORMAT_1S08 = 0x00000002,

        WAVE_FORMAT_1M16 = 0x00000004,

        WAVE_FORMAT_1S16 = 0x00000008,

        WAVE_FORMAT_2M08 = 0x00000010,

        WAVE_FORMAT_2S08 = 0x00000020,

        WAVE_FORMAT_2M16 = 0x00000040,

        WAVE_FORMAT_2S16 = 0x00000080,

        WAVE_FORMAT_4M08 = 0x00000100,

        WAVE_FORMAT_4S08 = 0x00000200,

        WAVE_FORMAT_4M16 = 0x00000400,

        WAVE_FORMAT_4S16 = 0x00000800,

        WAVE_FORMAT_44M08 = 0x00000100,

        WAVE_FORMAT_44S08 = 0x00000200,

        WAVE_FORMAT_44M16 = 0x00000400,

        WAVE_FORMAT_44S16 = 0x00000800,

        WAVE_FORMAT_48M08 = 0x00001000,

        WAVE_FORMAT_48S08 = 0x00002000,

        WAVE_FORMAT_48M16 = 0x00004000,

        WAVE_FORMAT_48S16 = 0x00008000,

        WAVE_FORMAT_96M08 = 0x00010000,

        WAVE_FORMAT_96S08 = 0x00020000,

        WAVE_FORMAT_96M16 = 0x00040000,

        WAVE_FORMAT_96S16 = 0x00080000,
    }
}