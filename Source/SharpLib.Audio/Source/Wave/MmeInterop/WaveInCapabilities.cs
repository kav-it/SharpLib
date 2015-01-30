using System;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace NAudio.Wave
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct WaveInCapabilities
    {
        private readonly short manufacturerId;

        private readonly short productId;

        private readonly int driverVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxProductNameLength)]
        private readonly string productName;

        private readonly SupportedWaveFormat supportedFormats;

        private readonly short channels;

        private readonly short reserved;

        private readonly Guid manufacturerGuid;

        private readonly Guid productGuid;

        private readonly Guid nameGuid;

        private const int MaxProductNameLength = 32;

        public int Channels
        {
            get { return channels; }
        }

        public string ProductName
        {
            get { return productName; }
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

        public bool SupportsWaveFormat(SupportedWaveFormat waveFormat)
        {
            return (supportedFormats & waveFormat) == waveFormat;
        }
    }

    internal static class WaveCapabilitiesHelpers
    {
        #region Поля

        public static readonly Guid DefaultWaveInGuid = new Guid("E36DC311-6D9A-11D1-A21A-00A0C9223196");

        public static readonly Guid DefaultWaveOutGuid = new Guid("E36DC310-6D9A-11D1-A21A-00A0C9223196");

        public static readonly Guid MicrosoftDefaultManufacturerId = new Guid("d5a47fa8-6d98-11d1-a21a-00a0c9223196");

        #endregion

        #region Методы

        public static string GetNameFromGuid(Guid guid)
        {
            string name = null;
            using (var namesKey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\MediaCategories"))
            {
                using (var nameKey = namesKey.OpenSubKey(guid.ToString("B")))
                {
                    if (nameKey != null)
                    {
                        name = nameKey.GetValue("Name") as string;
                    }
                }
            }
            return name;
        }

        #endregion
    }
}