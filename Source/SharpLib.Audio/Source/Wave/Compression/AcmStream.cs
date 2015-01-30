using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave.Compression
{
    internal class AcmStream : IDisposable
    {
        #region Поля

        private readonly WaveFormat sourceFormat;

        private IntPtr driverHandle;

        private IntPtr streamHandle;

        private AcmStreamHeader streamHeader;

        #endregion

        #region Свойства

        public byte[] SourceBuffer
        {
            get { return streamHeader.SourceBuffer; }
        }

        public byte[] DestBuffer
        {
            get { return streamHeader.DestBuffer; }
        }

        #endregion

        #region Конструктор

        public AcmStream(WaveFormat sourceFormat, WaveFormat destFormat)
        {
            try
            {
                streamHandle = IntPtr.Zero;
                this.sourceFormat = sourceFormat;
                int sourceBufferSize = Math.Max(65536, sourceFormat.AverageBytesPerSecond);
                sourceBufferSize -= (sourceBufferSize % sourceFormat.BlockAlign);
                MmException.Try(AcmInterop.acmStreamOpen(out streamHandle, IntPtr.Zero, sourceFormat, destFormat, null, IntPtr.Zero, IntPtr.Zero, AcmStreamOpenFlags.NonRealTime), "acmStreamOpen");

                int destBufferSize = SourceToDest(sourceBufferSize);
                streamHeader = new AcmStreamHeader(streamHandle, sourceBufferSize, destBufferSize);
                driverHandle = IntPtr.Zero;
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public AcmStream(IntPtr driverId, WaveFormat sourceFormat, WaveFilter waveFilter)
        {
            int sourceBufferSize = Math.Max(16384, sourceFormat.AverageBytesPerSecond);
            this.sourceFormat = sourceFormat;
            sourceBufferSize -= (sourceBufferSize % sourceFormat.BlockAlign);
            MmException.Try(AcmInterop.acmDriverOpen(out driverHandle, driverId, 0), "acmDriverOpen");
            MmException.Try(AcmInterop.acmStreamOpen(out streamHandle, driverHandle,
                sourceFormat, sourceFormat, waveFilter, IntPtr.Zero, IntPtr.Zero, AcmStreamOpenFlags.NonRealTime), "acmStreamOpen");
            streamHeader = new AcmStreamHeader(streamHandle, sourceBufferSize, SourceToDest(sourceBufferSize));
        }

        ~AcmStream()
        {
            System.Diagnostics.Debug.Assert(false, "AcmStream Dispose was not called");
            Dispose(false);
        }

        #endregion

        #region Методы

        public int SourceToDest(int source)
        {
            if (source == 0)
            {
                return 0;
            }
            int convertedBytes;
            var mmResult = AcmInterop.acmStreamSize(streamHandle, source, out convertedBytes, AcmStreamSizeFlags.Source);
            MmException.Try(mmResult, "acmStreamSize");
            return convertedBytes;
        }

        public int DestToSource(int dest)
        {
            if (dest == 0)
            {
                return 0;
            }
            int convertedBytes;
            MmException.Try(AcmInterop.acmStreamSize(streamHandle, dest, out convertedBytes, AcmStreamSizeFlags.Destination), "acmStreamSize");
            return convertedBytes;
        }

        public static WaveFormat SuggestPcmFormat(WaveFormat compressedFormat)
        {
            WaveFormat suggestedFormat = new WaveFormat(compressedFormat.SampleRate, 16, compressedFormat.Channels);
            MmException.Try(AcmInterop.acmFormatSuggest(IntPtr.Zero, compressedFormat, suggestedFormat, Marshal.SizeOf(suggestedFormat), AcmFormatSuggestFlags.FormatTag), "acmFormatSuggest");

            return suggestedFormat;
        }

        public void Reposition()
        {
            streamHeader.Reposition();
        }

        public int Convert(int bytesToConvert, out int sourceBytesConverted)
        {
            if (bytesToConvert % sourceFormat.BlockAlign != 0)
            {
                System.Diagnostics.Debug.WriteLine("Not a whole number of blocks: {0} ({1})", bytesToConvert, sourceFormat.BlockAlign);
                bytesToConvert -= (bytesToConvert % sourceFormat.BlockAlign);
            }

            return streamHeader.Convert(bytesToConvert, out sourceBytesConverted);
        }

        [Obsolete("Call the version returning sourceBytesConverted instead")]
        public int Convert(int bytesToConvert)
        {
            int sourceBytesConverted;
            int destBytes = Convert(bytesToConvert, out sourceBytesConverted);
            if (sourceBytesConverted != bytesToConvert)
            {
                throw new MmException(MmResult.NotSupported, "AcmStreamHeader.Convert didn't convert everything");
            }
            return destBytes;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (streamHeader != null)
                {
                    streamHeader.Dispose();
                    streamHeader = null;
                }
            }

            if (streamHandle != IntPtr.Zero)
            {
                MmResult result = AcmInterop.acmStreamClose(streamHandle, 0);
                streamHandle = IntPtr.Zero;
                if (result != MmResult.NoError)
                {
                    throw new MmException(result, "acmStreamClose");
                }
            }

            if (driverHandle != IntPtr.Zero)
            {
                AcmInterop.acmDriverClose(driverHandle, 0);
                driverHandle = IntPtr.Zero;
            }
        }

        #endregion
    }
}