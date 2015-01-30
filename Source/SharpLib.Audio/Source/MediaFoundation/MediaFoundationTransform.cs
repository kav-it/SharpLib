using System;
using System.Runtime.InteropServices;

using NAudio.Utils;
using NAudio.Wave;

namespace NAudio.MediaFoundation
{
    internal abstract class MediaFoundationTransform : IWaveProvider, IDisposable
    {
        #region Поля

        protected readonly WaveFormat outputWaveFormat;

        private readonly byte[] sourceBuffer;

        protected readonly IWaveProvider sourceProvider;

        private bool disposed;

        private bool initializedForStreaming;

        private long inputPosition;

        private byte[] outputBuffer;

        private int outputBufferCount;

        private int outputBufferOffset;

        private long outputPosition;

        private IMFTransform transform;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return outputWaveFormat; }
        }

        #endregion

        #region Конструктор

        public MediaFoundationTransform(IWaveProvider sourceProvider, WaveFormat outputFormat)
        {
            outputWaveFormat = outputFormat;
            this.sourceProvider = sourceProvider;
            sourceBuffer = new byte[sourceProvider.WaveFormat.AverageBytesPerSecond];
            outputBuffer = new byte[outputWaveFormat.AverageBytesPerSecond + outputWaveFormat.BlockAlign];
        }

        ~MediaFoundationTransform()
        {
            Dispose(false);
        }

        #endregion

        #region Методы

        private void InitializeTransformForStreaming()
        {
            transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_COMMAND_FLUSH, IntPtr.Zero);
            transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_NOTIFY_BEGIN_STREAMING, IntPtr.Zero);
            transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_NOTIFY_START_OF_STREAM, IntPtr.Zero);
            initializedForStreaming = true;
        }

        protected abstract IMFTransform CreateTransform();

        protected virtual void Dispose(bool disposing)
        {
            if (transform != null)
            {
                Marshal.ReleaseComObject(transform);
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (transform == null)
            {
                transform = CreateTransform();
                InitializeTransformForStreaming();
            }

            int bytesWritten = 0;

            if (outputBufferCount > 0)
            {
                bytesWritten += ReadFromOutputBuffer(buffer, offset, count - bytesWritten);
            }

            while (bytesWritten < count)
            {
                var sample = ReadFromSource();
                if (sample == null)
                {
                    EndStreamAndDrain();

                    bytesWritten += ReadFromOutputBuffer(buffer, offset + bytesWritten, count - bytesWritten);
                    break;
                }

                if (!initializedForStreaming)
                {
                    InitializeTransformForStreaming();
                }

                transform.ProcessInput(0, sample, 0);

                Marshal.ReleaseComObject(sample);

                int readFromTransform;

                readFromTransform = ReadFromTransform();
                bytesWritten += ReadFromOutputBuffer(buffer, offset + bytesWritten, count - bytesWritten);
            }

            return bytesWritten;
        }

        private void EndStreamAndDrain()
        {
            transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_NOTIFY_END_OF_STREAM, IntPtr.Zero);
            transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_COMMAND_DRAIN, IntPtr.Zero);
            int read;
            do
            {
                read = ReadFromTransform();
            } while (read > 0);
            outputBufferCount = 0;
            outputBufferOffset = 0;
            inputPosition = 0;
            outputPosition = 0;
            transform.ProcessMessage(MFT_MESSAGE_TYPE.MFT_MESSAGE_NOTIFY_END_STREAMING, IntPtr.Zero);
            initializedForStreaming = false;
        }

        private int ReadFromTransform()
        {
            var outputDataBuffer = new MFT_OUTPUT_DATA_BUFFER[1];

            var sample = MediaFoundationApi.CreateSample();
            var pBuffer = MediaFoundationApi.CreateMemoryBuffer(outputBuffer.Length);
            sample.AddBuffer(pBuffer);
            sample.SetSampleTime(outputPosition);
            outputDataBuffer[0].pSample = sample;

            _MFT_PROCESS_OUTPUT_STATUS status;
            var hr = transform.ProcessOutput(_MFT_PROCESS_OUTPUT_FLAGS.None,
                1, outputDataBuffer, out status);
            if (hr == MediaFoundationErrors.MF_E_TRANSFORM_NEED_MORE_INPUT)
            {
                Marshal.ReleaseComObject(pBuffer);
                Marshal.ReleaseComObject(sample);

                return 0;
            }
            if (hr != 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            IMFMediaBuffer outputMediaBuffer;
            outputDataBuffer[0].pSample.ConvertToContiguousBuffer(out outputMediaBuffer);
            IntPtr pOutputBuffer;
            int outputBufferLength;
            int maxSize;
            outputMediaBuffer.Lock(out pOutputBuffer, out maxSize, out outputBufferLength);
            outputBuffer = BufferHelpers.Ensure(outputBuffer, outputBufferLength);
            Marshal.Copy(pOutputBuffer, outputBuffer, 0, outputBufferLength);
            outputBufferOffset = 0;
            outputBufferCount = outputBufferLength;
            outputMediaBuffer.Unlock();
            outputPosition += BytesToNsPosition(outputBufferCount, WaveFormat);
            Marshal.ReleaseComObject(pBuffer);
            Marshal.ReleaseComObject(sample);
            Marshal.ReleaseComObject(outputMediaBuffer);
            return outputBufferLength;
        }

        private static long BytesToNsPosition(int bytes, WaveFormat waveFormat)
        {
            long nsPosition = (10000000L * bytes) / waveFormat.AverageBytesPerSecond;
            return nsPosition;
        }

        private IMFSample ReadFromSource()
        {
            int bytesRead = sourceProvider.Read(sourceBuffer, 0, sourceBuffer.Length);
            if (bytesRead == 0)
            {
                return null;
            }

            var mediaBuffer = MediaFoundationApi.CreateMemoryBuffer(bytesRead);
            IntPtr pBuffer;
            int maxLength, currentLength;
            mediaBuffer.Lock(out pBuffer, out maxLength, out currentLength);
            Marshal.Copy(sourceBuffer, 0, pBuffer, bytesRead);
            mediaBuffer.Unlock();
            mediaBuffer.SetCurrentLength(bytesRead);

            var sample = MediaFoundationApi.CreateSample();
            sample.AddBuffer(mediaBuffer);

            sample.SetSampleTime(inputPosition);
            long duration = BytesToNsPosition(bytesRead, sourceProvider.WaveFormat);
            sample.SetSampleDuration(duration);
            inputPosition += duration;
            Marshal.ReleaseComObject(mediaBuffer);
            return sample;
        }

        private int ReadFromOutputBuffer(byte[] buffer, int offset, int needed)
        {
            int bytesFromOutputBuffer = Math.Min(needed, outputBufferCount);
            Array.Copy(outputBuffer, outputBufferOffset, buffer, offset, bytesFromOutputBuffer);
            outputBufferOffset += bytesFromOutputBuffer;
            outputBufferCount -= bytesFromOutputBuffer;
            if (outputBufferCount == 0)
            {
                outputBufferOffset = 0;
            }
            return bytesFromOutputBuffer;
        }

        public void Reposition()
        {
            if (initializedForStreaming)
            {
                EndStreamAndDrain();
                InitializeTransformForStreaming();
            }
        }

        #endregion
    }
}