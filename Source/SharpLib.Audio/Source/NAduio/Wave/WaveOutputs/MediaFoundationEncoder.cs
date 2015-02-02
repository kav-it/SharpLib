using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using SharpLib.Audio.MediaFoundation;
using SharpLib.Audio.Utils;

namespace SharpLib.Audio.Wave
{
    internal class MediaFoundationEncoder : IDisposable
    {
        #region Поля

        private readonly MediaType outputMediaType;

        private bool disposed;

        #endregion

        #region Конструктор

        public MediaFoundationEncoder(MediaType outputMediaType)
        {
            if (outputMediaType == null)
            {
                throw new ArgumentNullException("outputMediaType");
            }
            this.outputMediaType = outputMediaType;
        }

        ~MediaFoundationEncoder()
        {
            Dispose(false);
        }

        #endregion

        #region Методы

        public static int[] GetEncodeBitrates(Guid audioSubtype, int sampleRate, int channels)
        {
            return GetOutputMediaTypes(audioSubtype)
                .Where(mt => mt.SampleRate == sampleRate && mt.ChannelCount == channels)
                .Select(mt => mt.AverageBytesPerSecond * 8)
                .Distinct()
                .OrderBy(br => br)
                .ToArray();
        }

        public static MediaType[] GetOutputMediaTypes(Guid audioSubtype)
        {
            IMFCollection availableTypes;
            try
            {
                MediaFoundationInterop.MFTranscodeGetAudioOutputAvailableTypes(
                    audioSubtype, _MFT_ENUM_FLAG.MFT_ENUM_FLAG_ALL, null, out availableTypes);
            }
            catch (COMException c)
            {
                if (c.GetHResult() == MediaFoundationErrors.MF_E_NOT_FOUND)
                {
                    return new MediaType[0];
                }
                throw;
            }
            int count;
            availableTypes.GetElementCount(out count);
            var mediaTypes = new List<MediaType>(count);
            for (int n = 0; n < count; n++)
            {
                object mediaTypeObject;
                availableTypes.GetElement(n, out mediaTypeObject);
                var mediaType = (IMFMediaType)mediaTypeObject;
                mediaTypes.Add(new MediaType(mediaType));
            }
            Marshal.ReleaseComObject(availableTypes);
            return mediaTypes.ToArray();
        }

        public static void EncodeToWma(IWaveProvider inputProvider, string outputFile, int desiredBitRate = 192000)
        {
            var mediaType = SelectMediaType(AudioSubtypes.MFAudioFormat_WMAudioV8, inputProvider.WaveFormat, desiredBitRate);
            if (mediaType == null)
            {
                throw new InvalidOperationException("No suitable WMA encoders available");
            }
            using (var encoder = new MediaFoundationEncoder(mediaType))
            {
                encoder.Encode(outputFile, inputProvider);
            }
        }

        public static void EncodeToMp3(IWaveProvider inputProvider, string outputFile, int desiredBitRate = 192000)
        {
            var mediaType = SelectMediaType(AudioSubtypes.MFAudioFormat_MP3, inputProvider.WaveFormat, desiredBitRate);
            if (mediaType == null)
            {
                throw new InvalidOperationException("No suitable MP3 encoders available");
            }
            using (var encoder = new MediaFoundationEncoder(mediaType))
            {
                encoder.Encode(outputFile, inputProvider);
            }
        }

        public static void EncodeToAac(IWaveProvider inputProvider, string outputFile, int desiredBitRate = 192000)
        {
            var mediaType = SelectMediaType(AudioSubtypes.MFAudioFormat_AAC, inputProvider.WaveFormat, desiredBitRate);
            if (mediaType == null)
            {
                throw new InvalidOperationException("No suitable AAC encoders available");
            }
            using (var encoder = new MediaFoundationEncoder(mediaType))
            {
                encoder.Encode(outputFile, inputProvider);
            }
        }

        public static MediaType SelectMediaType(Guid audioSubtype, WaveFormat inputFormat, int desiredBitRate)
        {
            return GetOutputMediaTypes(audioSubtype)
                .Where(mt => mt.SampleRate == inputFormat.SampleRate && mt.ChannelCount == inputFormat.Channels)
                .Select(mt => new
                {
                    MediaType = mt,
                    Delta = Math.Abs(desiredBitRate - mt.AverageBytesPerSecond * 8)
                })
                .OrderBy(mt => mt.Delta)
                .Select(mt => mt.MediaType)
                .FirstOrDefault();
        }

        public void Encode(string outputFile, IWaveProvider inputProvider)
        {
            if (inputProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm && inputProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Encode input format must be PCM or IEEE float");
            }

            var inputMediaType = new MediaType(inputProvider.WaveFormat);

            var writer = CreateSinkWriter(outputFile);
            try
            {
                int streamIndex;
                writer.AddStream(outputMediaType.MediaFoundationObject, out streamIndex);

                writer.SetInputMediaType(streamIndex, inputMediaType.MediaFoundationObject, null);

                PerformEncode(writer, streamIndex, inputProvider);
            }
            finally
            {
                Marshal.ReleaseComObject(writer);
                Marshal.ReleaseComObject(inputMediaType.MediaFoundationObject);
            }
        }

        private static IMFSinkWriter CreateSinkWriter(string outputFile)
        {
            IMFSinkWriter writer;
            var attributes = MediaFoundationApi.CreateAttributes(1);
            attributes.SetUINT32(MediaFoundationAttributes.MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, 1);
            try
            {
                MediaFoundationInterop.MFCreateSinkWriterFromURL(outputFile, null, attributes, out writer);
            }
            catch (COMException e)
            {
                if (e.GetHResult() == MediaFoundationErrors.MF_E_NOT_FOUND)
                {
                    throw new ArgumentException("Was not able to create a sink writer for this file extension");
                }
                throw;
            }
            finally
            {
                Marshal.ReleaseComObject(attributes);
            }
            return writer;
        }

        private void PerformEncode(IMFSinkWriter writer, int streamIndex, IWaveProvider inputProvider)
        {
            int maxLength = inputProvider.WaveFormat.AverageBytesPerSecond * 4;
            var managedBuffer = new byte[maxLength];

            writer.BeginWriting();

            long position = 0;
            long duration = 0;
            do
            {
                duration = ConvertOneBuffer(writer, streamIndex, inputProvider, position, managedBuffer);
                position += duration;
            } while (duration > 0);

            writer.DoFinalize();
        }

        private static long BytesToNsPosition(int bytes, WaveFormat waveFormat)
        {
            long nsPosition = (10000000L * bytes) / waveFormat.AverageBytesPerSecond;
            return nsPosition;
        }

        private long ConvertOneBuffer(IMFSinkWriter writer, int streamIndex, IWaveProvider inputProvider, long position, byte[] managedBuffer)
        {
            long durationConverted = 0;
            int maxLength;
            IMFMediaBuffer buffer = MediaFoundationApi.CreateMemoryBuffer(managedBuffer.Length);
            buffer.GetMaxLength(out maxLength);

            IMFSample sample = MediaFoundationApi.CreateSample();
            sample.AddBuffer(buffer);

            IntPtr ptr;
            int currentLength;
            buffer.Lock(out ptr, out maxLength, out currentLength);
            int read = inputProvider.Read(managedBuffer, 0, maxLength);
            if (read > 0)
            {
                durationConverted = BytesToNsPosition(read, inputProvider.WaveFormat);
                Marshal.Copy(managedBuffer, 0, ptr, read);
                buffer.SetCurrentLength(read);
                buffer.Unlock();
                sample.SetSampleTime(position);
                sample.SetSampleDuration(durationConverted);
                writer.WriteSample(streamIndex, sample);
            }
            else
            {
                buffer.Unlock();
            }

            Marshal.ReleaseComObject(sample);
            Marshal.ReleaseComObject(buffer);
            return durationConverted;
        }

        protected void Dispose(bool disposing)
        {
            Marshal.ReleaseComObject(outputMediaType.MediaFoundationObject);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                Dispose(true);
            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}