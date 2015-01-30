using System;
using System.Diagnostics;

using NAudio.Dmo;

namespace NAudio.Wave
{
    internal class ResamplerDmoStream : WaveStream
    {
        #region Поля

        private readonly IWaveProvider inputProvider;

        private readonly WaveStream inputStream;

        private readonly WaveFormat outputFormat;

        private DmoResampler dmoResampler;

        private MediaBuffer inputMediaBuffer;

        private DmoOutputDataBuffer outputBuffer;

        private long position;

        #endregion

        #region Свойства

        public override WaveFormat WaveFormat
        {
            get { return outputFormat; }
        }

        public override long Length
        {
            get
            {
                if (inputStream == null)
                {
                    throw new InvalidOperationException("Cannot report length if the input was an IWaveProvider");
                }
                return InputToOutputPosition(inputStream.Length);
            }
        }

        public override long Position
        {
            get { return position; }
            set
            {
                if (inputStream == null)
                {
                    throw new InvalidOperationException("Cannot set position if the input was not a WaveStream");
                }
                inputStream.Position = OutputToInputPosition(value);
                position = InputToOutputPosition(inputStream.Position);
                dmoResampler.MediaObject.Discontinuity(0);
            }
        }

        #endregion

        #region Конструктор

        public ResamplerDmoStream(IWaveProvider inputProvider, WaveFormat outputFormat)
        {
            this.inputProvider = inputProvider;
            inputStream = inputProvider as WaveStream;
            this.outputFormat = outputFormat;
            dmoResampler = new DmoResampler();
            if (!dmoResampler.MediaObject.SupportsInputWaveFormat(0, inputProvider.WaveFormat))
            {
                throw new ArgumentException("Unsupported Input Stream format", "inputStream");
            }

            dmoResampler.MediaObject.SetInputWaveFormat(0, inputProvider.WaveFormat);
            if (!dmoResampler.MediaObject.SupportsOutputWaveFormat(0, outputFormat))
            {
                throw new ArgumentException("Unsupported Output Stream format", "outputStream");
            }

            dmoResampler.MediaObject.SetOutputWaveFormat(0, outputFormat);
            if (inputStream != null)
            {
                position = InputToOutputPosition(inputStream.Position);
            }
            inputMediaBuffer = new MediaBuffer(inputProvider.WaveFormat.AverageBytesPerSecond);
            outputBuffer = new DmoOutputDataBuffer(outputFormat.AverageBytesPerSecond);
        }

        #endregion

        #region Методы

        private long InputToOutputPosition(long inputPosition)
        {
            double ratio = (double)outputFormat.AverageBytesPerSecond
                           / inputProvider.WaveFormat.AverageBytesPerSecond;
            long outputPosition = (long)(inputPosition * ratio);
            if (outputPosition % outputFormat.BlockAlign != 0)
            {
                outputPosition -= outputPosition % outputFormat.BlockAlign;
            }
            return outputPosition;
        }

        private long OutputToInputPosition(long outputPosition)
        {
            double ratio = (double)outputFormat.AverageBytesPerSecond
                           / inputProvider.WaveFormat.AverageBytesPerSecond;
            long inputPosition = (long)(outputPosition / ratio);
            if (inputPosition % inputProvider.WaveFormat.BlockAlign != 0)
            {
                inputPosition -= inputPosition % inputProvider.WaveFormat.BlockAlign;
            }
            return inputPosition;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int outputBytesProvided = 0;

            while (outputBytesProvided < count)
            {
                if (dmoResampler.MediaObject.IsAcceptingData(0))
                {
                    int inputBytesRequired = (int)OutputToInputPosition(count - outputBytesProvided);
                    byte[] inputByteArray = new byte[inputBytesRequired];
                    int inputBytesRead = inputProvider.Read(inputByteArray, 0, inputBytesRequired);
                    if (inputBytesRead == 0)
                    {
                        break;
                    }

                    inputMediaBuffer.LoadData(inputByteArray, inputBytesRead);

                    dmoResampler.MediaObject.ProcessInput(0, inputMediaBuffer, DmoInputDataBufferFlags.None, 0, 0);

                    outputBuffer.MediaBuffer.SetLength(0);
                    outputBuffer.StatusFlags = DmoOutputDataBufferFlags.None;

                    dmoResampler.MediaObject.ProcessOutput(DmoProcessOutputFlags.None, 1, new[] { outputBuffer });

                    if (outputBuffer.Length == 0)
                    {
                        Debug.WriteLine("ResamplerDmoStream.Read: No output data available");
                        break;
                    }

                    outputBuffer.RetrieveData(buffer, offset + outputBytesProvided);
                    outputBytesProvided += outputBuffer.Length;

                    Debug.Assert(!outputBuffer.MoreDataAvailable, "have not implemented more data available yet");
                }
                else
                {
                    Debug.Assert(false, "have not implemented not accepting logic yet");
                }
            }

            position += outputBytesProvided;
            return outputBytesProvided;
        }

        protected override void Dispose(bool disposing)
        {
            if (inputMediaBuffer != null)
            {
                inputMediaBuffer.Dispose();
                inputMediaBuffer = null;
            }
            outputBuffer.Dispose();
            if (dmoResampler != null)
            {
                dmoResampler = null;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}