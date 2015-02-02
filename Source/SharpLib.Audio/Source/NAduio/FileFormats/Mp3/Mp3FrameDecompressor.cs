using System;

using SharpLib.Audio.Wave.Compression;

namespace SharpLib.Audio.Wave
{
    internal class AcmMp3FrameDecompressor : IMp3FrameDecompressor
    {
        #region Поля

        private readonly AcmStream conversionStream;

        private readonly WaveFormat pcmFormat;

        private bool disposed;

        #endregion

        #region Свойства

        public WaveFormat OutputFormat
        {
            get { return pcmFormat; }
        }

        #endregion

        #region Конструктор

        public AcmMp3FrameDecompressor(WaveFormat sourceFormat)
        {
            pcmFormat = AcmStream.SuggestPcmFormat(sourceFormat);
            try
            {
                conversionStream = new AcmStream(sourceFormat, pcmFormat);
            }
            catch (Exception)
            {
                disposed = true;
                GC.SuppressFinalize(this);
                throw;
            }
        }

        ~AcmMp3FrameDecompressor()
        {
            System.Diagnostics.Debug.Assert(false, "AcmMp3FrameDecompressor Dispose was not called");
            Dispose();
        }

        #endregion

        #region Методы

        public int DecompressFrame(Mp3Frame frame, byte[] dest, int destOffset)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame", "You must provide a non-null Mp3Frame to decompress");
            }
            Array.Copy(frame.RawData, conversionStream.SourceBuffer, frame.FrameLength);
            int sourceBytesConverted = 0;
            int converted = conversionStream.Convert(frame.FrameLength, out sourceBytesConverted);
            if (sourceBytesConverted != frame.FrameLength)
            {
                throw new InvalidOperationException(String.Format("Couldn't convert the whole MP3 frame (converted {0}/{1})",
                    sourceBytesConverted, frame.FrameLength));
            }
            Array.Copy(conversionStream.DestBuffer, 0, dest, destOffset, converted);
            return converted;
        }

        public void Reset()
        {
            conversionStream.Reposition();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                if (conversionStream != null)
                {
                    conversionStream.Dispose();
                }
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}