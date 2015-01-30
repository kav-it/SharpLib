using System;
using System.Runtime.InteropServices;

namespace NAudio.Midi
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct MidiOutCapabilities
    {
        private readonly Int16 manufacturerId;

        private readonly Int16 productId;

        private readonly int driverVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxProductNameLength)]
        private readonly string productName;

        private readonly Int16 wTechnology;

        private readonly Int16 wVoices;

        private readonly Int16 wNotes;

        private readonly UInt16 wChannelMask;

        private readonly MidiOutCapabilityFlags dwSupport;

        private const int MaxProductNameLength = 32;

        [Flags]
        private enum MidiOutCapabilityFlags
        {
            Volume = 1,

            LeftRightVolume = 2,

            PatchCaching = 4,

            Stream = 8,
        }

        public Manufacturers Manufacturer
        {
            get { return (Manufacturers)manufacturerId; }
        }

        public short ProductId
        {
            get { return productId; }
        }

        public String ProductName
        {
            get { return productName; }
        }

        public int Voices
        {
            get { return wVoices; }
        }

        public int Notes
        {
            get { return wNotes; }
        }

        public bool SupportsAllChannels
        {
            get { return wChannelMask == 0xFFFF; }
        }

        public bool SupportsChannel(int channel)
        {
            return (wChannelMask & (1 << (channel - 1))) > 0;
        }

        public bool SupportsPatchCaching
        {
            get { return (dwSupport & MidiOutCapabilityFlags.PatchCaching) != 0; }
        }

        public bool SupportsSeparateLeftAndRightVolume
        {
            get { return (dwSupport & MidiOutCapabilityFlags.LeftRightVolume) != 0; }
        }

        public bool SupportsMidiStreamOut
        {
            get { return (dwSupport & MidiOutCapabilityFlags.Stream) != 0; }
        }

        public bool SupportsVolumeControl
        {
            get { return (dwSupport & MidiOutCapabilityFlags.Volume) != 0; }
        }

        public MidiOutTechnology Technology
        {
            get { return (MidiOutTechnology)wTechnology; }
        }
    }
}