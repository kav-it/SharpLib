using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using NAudio.Utils;

namespace NAudio.Wave.Compression
{
    internal class AcmDriver : IDisposable
    {
        #region ����

        private static List<AcmDriver> drivers;

        private readonly IntPtr driverId;

        private AcmDriverDetails details;

        private IntPtr driverHandle;

        private List<AcmFormatTag> formatTags;

        private IntPtr localDllHandle;

        private List<AcmFormat> tempFormatsList;

        #endregion

        #region ��������

        public int MaxFormatSize
        {
            get
            {
                int maxFormatSize;
                MmException.Try(AcmInterop.acmMetrics(driverHandle, AcmMetrics.MaxSizeFormat, out maxFormatSize), "acmMetrics");
                return maxFormatSize;
            }
        }

        public string ShortName
        {
            get { return details.shortName; }
        }

        public string LongName
        {
            get { return details.longName; }
        }

        public IntPtr DriverId
        {
            get { return driverId; }
        }

        public IEnumerable<AcmFormatTag> FormatTags
        {
            get
            {
                if (formatTags == null)
                {
                    if (driverHandle == IntPtr.Zero)
                    {
                        throw new InvalidOperationException("Driver must be opened first");
                    }
                    formatTags = new List<AcmFormatTag>();
                    AcmFormatTagDetails formatTagDetails = new AcmFormatTagDetails();
                    formatTagDetails.structureSize = Marshal.SizeOf(formatTagDetails);
                    MmException.Try(AcmInterop.acmFormatTagEnum(driverHandle, ref formatTagDetails, AcmFormatTagEnumCallback, IntPtr.Zero, 0), "acmFormatTagEnum");
                }
                return formatTags;
            }
        }

        #endregion

        #region �����������

        private AcmDriver(IntPtr hAcmDriver)
        {
            driverId = hAcmDriver;
            details = new AcmDriverDetails();
            details.structureSize = Marshal.SizeOf(details);
            MmException.Try(AcmInterop.acmDriverDetails(hAcmDriver, ref details, 0), "acmDriverDetails");
        }

        #endregion

        #region ������

        public static bool IsCodecInstalled(string shortName)
        {
            foreach (AcmDriver driver in EnumerateAcmDrivers())
            {
                if (driver.ShortName == shortName)
                {
                    return true;
                }
            }
            return false;
        }

        public static AcmDriver AddLocalDriver(string driverFile)
        {
            IntPtr handle = NativeMethods.LoadLibrary(driverFile);
            if (handle == IntPtr.Zero)
            {
                throw new ArgumentException("Failed to load driver file");
            }
            var driverProc = NativeMethods.GetProcAddress(handle, "DriverProc");
            if (driverProc == IntPtr.Zero)
            {
                NativeMethods.FreeLibrary(handle);
                throw new ArgumentException("Failed to discover DriverProc");
            }
            IntPtr driverHandle;
            var result = AcmInterop.acmDriverAdd(out driverHandle,
                handle, driverProc, 0, AcmDriverAddFlags.Function);
            if (result != MmResult.NoError)
            {
                NativeMethods.FreeLibrary(handle);
                throw new MmException(result, "acmDriverAdd");
            }
            var driver = new AcmDriver(driverHandle);

            if (string.IsNullOrEmpty(driver.details.longName))
            {
                driver.details.longName = "Local driver: " + Path.GetFileName(driverFile);
                driver.localDllHandle = handle;
            }
            return driver;
        }

        public static void RemoveLocalDriver(AcmDriver localDriver)
        {
            if (localDriver.localDllHandle == IntPtr.Zero)
            {
                throw new ArgumentException("Please pass in the AcmDriver returned by the AddLocalDriver method");
            }
            var removeResult = AcmInterop.acmDriverRemove(localDriver.driverId, 0);
            NativeMethods.FreeLibrary(localDriver.localDllHandle);
            MmException.Try(removeResult, "acmDriverRemove");
        }

        public static bool ShowFormatChooseDialog(
            IntPtr ownerWindowHandle,
            string windowTitle,
            AcmFormatEnumFlags enumFlags,
            WaveFormat enumFormat,
            out WaveFormat selectedFormat,
            out string selectedFormatDescription,
            out string selectedFormatTagDescription)
        {
            AcmFormatChoose formatChoose = new AcmFormatChoose();
            formatChoose.structureSize = Marshal.SizeOf(formatChoose);
            formatChoose.styleFlags = AcmFormatChooseStyleFlags.None;
            formatChoose.ownerWindowHandle = ownerWindowHandle;
            int maxFormatSize = 200;
            formatChoose.selectedWaveFormatPointer = Marshal.AllocHGlobal(maxFormatSize);
            formatChoose.selectedWaveFormatByteSize = maxFormatSize;
            formatChoose.title = windowTitle;
            formatChoose.name = null;
            formatChoose.formatEnumFlags = enumFlags;
            formatChoose.waveFormatEnumPointer = IntPtr.Zero;
            if (enumFormat != null)
            {
                IntPtr enumPointer = Marshal.AllocHGlobal(Marshal.SizeOf(enumFormat));
                Marshal.StructureToPtr(enumFormat, enumPointer, false);
                formatChoose.waveFormatEnumPointer = enumPointer;
            }
            formatChoose.instanceHandle = IntPtr.Zero;
            formatChoose.templateName = null;

            MmResult result = AcmInterop.acmFormatChoose(ref formatChoose);
            selectedFormat = null;
            selectedFormatDescription = null;
            selectedFormatTagDescription = null;
            if (result == MmResult.NoError)
            {
                selectedFormat = WaveFormat.MarshalFromPtr(formatChoose.selectedWaveFormatPointer);
                selectedFormatDescription = formatChoose.formatDescription;
                selectedFormatTagDescription = formatChoose.formatTagDescription;
            }

            Marshal.FreeHGlobal(formatChoose.waveFormatEnumPointer);
            Marshal.FreeHGlobal(formatChoose.selectedWaveFormatPointer);
            if (result != MmResult.AcmCancelled && result != MmResult.NoError)
            {
                throw new MmException(result, "acmFormatChoose");
            }
            return result == MmResult.NoError;
        }

        public static AcmDriver FindByShortName(string shortName)
        {
            foreach (AcmDriver driver in AcmDriver.EnumerateAcmDrivers())
            {
                if (driver.ShortName == shortName)
                {
                    return driver;
                }
            }
            return null;
        }

        public static IEnumerable<AcmDriver> EnumerateAcmDrivers()
        {
            drivers = new List<AcmDriver>();
            MmException.Try(AcmInterop.acmDriverEnum(DriverEnumCallback, IntPtr.Zero, 0), "acmDriverEnum");
            return drivers;
        }

        private static bool DriverEnumCallback(IntPtr hAcmDriver, IntPtr dwInstance, AcmDriverDetailsSupportFlags flags)
        {
            drivers.Add(new AcmDriver(hAcmDriver));
            return true;
        }

        public override string ToString()
        {
            return LongName;
        }

        public IEnumerable<AcmFormat> GetFormats(AcmFormatTag formatTag)
        {
            if (driverHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Driver must be opened first");
            }
            tempFormatsList = new List<AcmFormat>();
            var formatDetails = new AcmFormatDetails();
            formatDetails.structSize = Marshal.SizeOf(formatDetails);

            formatDetails.waveFormatByteSize = 1024;
            formatDetails.waveFormatPointer = Marshal.AllocHGlobal(formatDetails.waveFormatByteSize);
            formatDetails.formatTag = (int)formatTag.FormatTag;
            var result = AcmInterop.acmFormatEnum(driverHandle,
                ref formatDetails, AcmFormatEnumCallback, IntPtr.Zero,
                AcmFormatEnumFlags.None);
            Marshal.FreeHGlobal(formatDetails.waveFormatPointer);
            MmException.Try(result, "acmFormatEnum");
            return tempFormatsList;
        }

        public void Open()
        {
            if (driverHandle == IntPtr.Zero)
            {
                MmException.Try(AcmInterop.acmDriverOpen(out driverHandle, DriverId, 0), "acmDriverOpen");
            }
        }

        public void Close()
        {
            if (driverHandle != IntPtr.Zero)
            {
                MmException.Try(AcmInterop.acmDriverClose(driverHandle, 0), "acmDriverClose");
                driverHandle = IntPtr.Zero;
            }
        }

        private bool AcmFormatTagEnumCallback(IntPtr hAcmDriverId, ref AcmFormatTagDetails formatTagDetails, IntPtr dwInstance, AcmDriverDetailsSupportFlags flags)
        {
            formatTags.Add(new AcmFormatTag(formatTagDetails));
            return true;
        }

        private bool AcmFormatEnumCallback(IntPtr hAcmDriverId, ref AcmFormatDetails formatDetails, IntPtr dwInstance, AcmDriverDetailsSupportFlags flags)
        {
            tempFormatsList.Add(new AcmFormat(formatDetails));
            return true;
        }

        public void Dispose()
        {
            if (driverHandle != IntPtr.Zero)
            {
                Close();
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}