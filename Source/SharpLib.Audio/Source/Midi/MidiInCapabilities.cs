using System;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.Midi
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct MidiInCapabilities
    {
        private readonly UInt16 manufacturerId;

        private readonly UInt16 productId;

        private readonly UInt32 driverVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxProductNameLength)]
        private readonly string productName;

        private readonly Int32 support;

        private const int MaxProductNameLength = 32;

        public Manufacturers Manufacturer
        {
            get { return (Manufacturers)manufacturerId; }
        }

        public int ProductId
        {
            get { return productId; }
        }

        public string ProductName
        {
            get { return productName; }
        }
    }
}