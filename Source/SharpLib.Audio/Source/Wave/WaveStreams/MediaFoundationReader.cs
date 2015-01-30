using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using NAudio.CoreAudioApi.Interfaces;
using NAudio.MediaFoundation;

namespace NAudio.Wave
{
    internal class MediaFoundationReader : WaveStream
    {
        #region Поля

        private readonly string file;

        private readonly long length;

        private readonly MediaFoundationReaderSettings settings;

        private byte[] decoderOutputBuffer;

        private int decoderOutputCount;

        private int decoderOutputOffset;

        private IMFSourceReader pReader;

        private long position;

        private long repositionTo = -1;

        private WaveFormat waveFormat;

        #endregion

        #region Свойства

        public override WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public override long Length
        {
            get { return length; }
        }

        public override long Position
        {
            get { return position; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Position cannot be less than 0");
                }
                if (settings.RepositionInRead)
                {
                    repositionTo = value;
                    position = value;
                }
                else
                {
                    Reposition(value);
                }
            }
        }

        #endregion

        #region События

        public event EventHandler WaveFormatChanged;

        #endregion

        #region Конструктор

        public MediaFoundationReader(string file)
            : this(file, new MediaFoundationReaderSettings())
        {
        }

        public MediaFoundationReader(string file, MediaFoundationReaderSettings settings)
        {
            MediaFoundationApi.Startup();
            this.settings = settings;
            this.file = file;
            var reader = CreateReader(settings);

            waveFormat = GetCurrentWaveFormat(reader);

            reader.SetStreamSelection(MediaFoundationInterop.MF_SOURCE_READER_FIRST_AUDIO_STREAM, true);
            length = GetLength(reader);

            if (settings.SingleReaderObject)
            {
                pReader = reader;
            }
        }

        #endregion

        #region Методы

        private WaveFormat GetCurrentWaveFormat(IMFSourceReader reader)
        {
            IMFMediaType uncompressedMediaType;
            reader.GetCurrentMediaType(MediaFoundationInterop.MF_SOURCE_READER_FIRST_AUDIO_STREAM, out uncompressedMediaType);

            var outputMediaType = new MediaType(uncompressedMediaType);
            Guid actualMajorType = outputMediaType.MajorType;
            Debug.Assert(actualMajorType == MediaTypes.MFMediaType_Audio);
            Guid audioSubType = outputMediaType.SubType;
            int channels = outputMediaType.ChannelCount;
            int bits = outputMediaType.BitsPerSample;
            int sampleRate = outputMediaType.SampleRate;

            return audioSubType == AudioSubtypes.MFAudioFormat_PCM
                ? new WaveFormat(sampleRate, bits, channels)
                : WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
        }

        protected virtual IMFSourceReader CreateReader(MediaFoundationReaderSettings settings)
        {
            IMFSourceReader reader;
            MediaFoundationInterop.MFCreateSourceReaderFromURL(file, null, out reader);
            reader.SetStreamSelection(MediaFoundationInterop.MF_SOURCE_READER_ALL_STREAMS, false);
            reader.SetStreamSelection(MediaFoundationInterop.MF_SOURCE_READER_FIRST_AUDIO_STREAM, true);

            var partialMediaType = new MediaType();
            partialMediaType.MajorType = MediaTypes.MFMediaType_Audio;
            partialMediaType.SubType = settings.RequestFloatOutput ? AudioSubtypes.MFAudioFormat_Float : AudioSubtypes.MFAudioFormat_PCM;

            reader.SetCurrentMediaType(MediaFoundationInterop.MF_SOURCE_READER_FIRST_AUDIO_STREAM, IntPtr.Zero, partialMediaType.MediaFoundationObject);
            return reader;
        }

        private long GetLength(IMFSourceReader reader)
        {
            PropVariant variant;

            int hResult = reader.GetPresentationAttribute(MediaFoundationInterop.MF_SOURCE_READER_MEDIASOURCE,
                MediaFoundationAttributes.MF_PD_DURATION, out variant);
            if (hResult == MediaFoundationErrors.MF_E_ATTRIBUTENOTFOUND)
            {
                return 0;
            }
            if (hResult != 0)
            {
                Marshal.ThrowExceptionForHR(hResult);
            }
            var lengthInBytes = (((long)variant.Value) * waveFormat.AverageBytesPerSecond) / 10000000L;
            variant.Clear();
            return lengthInBytes;
        }

        private void EnsureBuffer(int bytesRequired)
        {
            if (decoderOutputBuffer == null || decoderOutputBuffer.Length < bytesRequired)
            {
                decoderOutputBuffer = new byte[bytesRequired];
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (pReader == null)
            {
                pReader = CreateReader(settings);
            }
            if (repositionTo != -1)
            {
                Reposition(repositionTo);
            }

            int bytesWritten = 0;

            if (decoderOutputCount > 0)
            {
                bytesWritten += ReadFromDecoderBuffer(buffer, offset, count - bytesWritten);
            }

            while (bytesWritten < count)
            {
                IMFSample pSample;
                MF_SOURCE_READER_FLAG dwFlags;
                ulong timestamp;
                int actualStreamIndex;
                pReader.ReadSample(MediaFoundationInterop.MF_SOURCE_READER_FIRST_AUDIO_STREAM, 0, out actualStreamIndex, out dwFlags, out timestamp, out pSample);
                if ((dwFlags & MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_ENDOFSTREAM) != 0)
                {
                    break;
                }
                if ((dwFlags & MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_CURRENTMEDIATYPECHANGED) != 0)
                {
                    waveFormat = GetCurrentWaveFormat(pReader);
                    OnWaveFormatChanged();
                }
                else if (dwFlags != 0)
                {
                    throw new InvalidOperationException(String.Format("MediaFoundationReadError {0}", dwFlags));
                }

                IMFMediaBuffer pBuffer;
                pSample.ConvertToContiguousBuffer(out pBuffer);
                IntPtr pAudioData;
                int cbBuffer;
                int pcbMaxLength;
                pBuffer.Lock(out pAudioData, out pcbMaxLength, out cbBuffer);
                EnsureBuffer(cbBuffer);
                Marshal.Copy(pAudioData, decoderOutputBuffer, 0, cbBuffer);
                decoderOutputOffset = 0;
                decoderOutputCount = cbBuffer;

                bytesWritten += ReadFromDecoderBuffer(buffer, offset + bytesWritten, count - bytesWritten);

                pBuffer.Unlock();
                Marshal.ReleaseComObject(pBuffer);
                Marshal.ReleaseComObject(pSample);
            }
            position += bytesWritten;
            return bytesWritten;
        }

        private int ReadFromDecoderBuffer(byte[] buffer, int offset, int needed)
        {
            int bytesFromDecoderOutput = Math.Min(needed, decoderOutputCount);
            Array.Copy(decoderOutputBuffer, decoderOutputOffset, buffer, offset, bytesFromDecoderOutput);
            decoderOutputOffset += bytesFromDecoderOutput;
            decoderOutputCount -= bytesFromDecoderOutput;
            if (decoderOutputCount == 0)
            {
                decoderOutputOffset = 0;
            }
            return bytesFromDecoderOutput;
        }

        private void Reposition(long desiredPosition)
        {
            long nsPosition = (10000000L * repositionTo) / waveFormat.AverageBytesPerSecond;
            var pv = PropVariant.FromLong(nsPosition);

            pReader.SetCurrentPosition(Guid.Empty, ref pv);
            decoderOutputCount = 0;
            decoderOutputOffset = 0;
            position = desiredPosition;
            repositionTo = -1;
        }

        protected override void Dispose(bool disposing)
        {
            if (pReader != null)
            {
                Marshal.ReleaseComObject(pReader);
                pReader = null;
            }
            base.Dispose(disposing);
        }

        private void OnWaveFormatChanged()
        {
            var handler = WaveFormatChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Вложенный класс: MediaFoundationReaderSettings

        public class MediaFoundationReaderSettings
        {
            #region Свойства

            public bool RequestFloatOutput { get; set; }

            public bool SingleReaderObject { get; set; }

            public bool RepositionInRead { get; set; }

            #endregion

            #region Конструктор

            public MediaFoundationReaderSettings()
            {
                RepositionInRead = true;
            }

            #endregion
        }

        #endregion
    }
}