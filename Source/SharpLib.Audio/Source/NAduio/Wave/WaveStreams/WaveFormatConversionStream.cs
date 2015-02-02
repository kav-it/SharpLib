using System;
using System.Diagnostics;

using SharpLib.Audio.Wave.Compression;

namespace SharpLib.Audio.Wave
{
    internal class WaveFormatConversionStream : WaveStream
    {
        #region Поля

        private readonly int leftoverSourceBytes = 0;

        private readonly long length;

        private readonly int preferredSourceReadSize;

        private readonly WaveFormat targetFormat;

        private AcmStream conversionStream;

        private int leftoverDestBytes;

        private int leftoverDestOffset;

        private long position;

        private WaveStream sourceStream;

        #endregion

        #region Свойства

        public override long Length
        {
            get { return length; }
        }

        public override long Position
        {
            get { return position; }
            set
            {
                value -= (value % BlockAlign);

                var desiredSourcePosition = EstimateDestToSource(value);
                sourceStream.Position = desiredSourcePosition;
                position = EstimateSourceToDest(sourceStream.Position);
                leftoverDestBytes = 0;
                leftoverDestOffset = 0;
                conversionStream.Reposition();
            }
        }

        public override WaveFormat WaveFormat
        {
            get { return targetFormat; }
        }

        #endregion

        #region Конструктор

        public WaveFormatConversionStream(WaveFormat targetFormat, WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            this.targetFormat = targetFormat;

            conversionStream = new AcmStream(sourceStream.WaveFormat, targetFormat);

            length = EstimateSourceToDest((int)sourceStream.Length);

            position = 0;
            preferredSourceReadSize = Math.Min(sourceStream.WaveFormat.AverageBytesPerSecond, conversionStream.SourceBuffer.Length);
            preferredSourceReadSize -= (preferredSourceReadSize % sourceStream.WaveFormat.BlockAlign);
        }

        #endregion

        #region Методы

        public static WaveStream CreatePcmStream(WaveStream sourceStream)
        {
            if (sourceStream.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
            {
                return sourceStream;
            }
            WaveFormat pcmFormat = AcmStream.SuggestPcmFormat(sourceStream.WaveFormat);
            if (pcmFormat.SampleRate < 8000)
            {
                if (sourceStream.WaveFormat.Encoding == WaveFormatEncoding.G723)
                {
                    pcmFormat = new WaveFormat(8000, 16, 1);
                }
                else
                {
                    throw new InvalidOperationException("Invalid suggested output format, please explicitly provide a target format");
                }
            }
            return new WaveFormatConversionStream(pcmFormat, sourceStream);
        }

        [Obsolete("can be unreliable, use of this method not encouraged")]
        public int SourceToDest(int source)
        {
            return (int)EstimateSourceToDest(source);
        }

        private long EstimateSourceToDest(long source)
        {
            var dest = ((source * targetFormat.AverageBytesPerSecond) / sourceStream.WaveFormat.AverageBytesPerSecond);
            dest -= (dest % targetFormat.BlockAlign);
            return dest;
        }

        private long EstimateDestToSource(long dest)
        {
            var source = ((dest * sourceStream.WaveFormat.AverageBytesPerSecond) / targetFormat.AverageBytesPerSecond);
            source -= (source % sourceStream.WaveFormat.BlockAlign);
            return (int)source;
        }

        [Obsolete("can be unreliable, use of this method not encouraged")]
        public int DestToSource(int dest)
        {
            return (int)EstimateDestToSource(dest);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;
            if (count % BlockAlign != 0)
            {
                count -= (count % BlockAlign);
            }

            while (bytesRead < count)
            {
                int readFromLeftoverDest = Math.Min(count - bytesRead, leftoverDestBytes);
                if (readFromLeftoverDest > 0)
                {
                    Array.Copy(conversionStream.DestBuffer, leftoverDestOffset, buffer, offset + bytesRead, readFromLeftoverDest);
                    leftoverDestOffset += readFromLeftoverDest;
                    leftoverDestBytes -= readFromLeftoverDest;
                    bytesRead += readFromLeftoverDest;
                }
                if (bytesRead >= count)
                {
                    break;
                }

                if (leftoverSourceBytes > 0)
                {
                }

                int sourceBytesRead = sourceStream.Read(conversionStream.SourceBuffer, 0, preferredSourceReadSize);
                if (sourceBytesRead == 0)
                {
                    break;
                }

                int sourceBytesConverted;
                int destBytesConverted = conversionStream.Convert(sourceBytesRead, out sourceBytesConverted);
                if (sourceBytesConverted == 0)
                {
                    Debug.WriteLine("Warning: couldn't convert anything from {0}", sourceBytesRead);

                    break;
                }
                if (sourceBytesConverted < sourceBytesRead)
                {
                    sourceStream.Position -= (sourceBytesRead - sourceBytesConverted);
                }

                if (destBytesConverted > 0)
                {
                    int bytesRequired = count - bytesRead;
                    int toCopy = Math.Min(destBytesConverted, bytesRequired);

                    if (toCopy < destBytesConverted)
                    {
                        leftoverDestBytes = destBytesConverted - toCopy;
                        leftoverDestOffset = toCopy;
                    }
                    Array.Copy(conversionStream.DestBuffer, 0, buffer, bytesRead + offset, toCopy);
                    bytesRead += toCopy;
                }
                else
                {
                    Debug.WriteLine("sourceBytesRead: {0}, sourceBytesConverted {1}, destBytesConverted {2}", sourceBytesRead, sourceBytesConverted, destBytesConverted);

                    break;
                }
            }
            position += bytesRead;
            return bytesRead;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (conversionStream != null)
                {
                    conversionStream.Dispose();
                    conversionStream = null;
                }
                if (sourceStream != null)
                {
                    sourceStream.Dispose();
                    sourceStream = null;
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "WaveFormatConversionStream was not disposed");
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}