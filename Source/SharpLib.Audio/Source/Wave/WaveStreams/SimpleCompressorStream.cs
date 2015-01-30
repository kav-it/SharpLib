using System;

using SharpLib.Audio.Dsp;

namespace SharpLib.Audio.Wave
{
    internal class SimpleCompressorStream : WaveStream
    {
        #region Поля

        private readonly int bytesPerSample;

        private readonly int channels;

        private readonly object lockObject = new object();

        private readonly SimpleCompressor simpleCompressor;

        private byte[] sourceBuffer;

        private WaveStream sourceStream;

        #endregion

        #region Свойства

        public double MakeUpGain
        {
            get { return simpleCompressor.MakeUpGain; }
            set
            {
                lock (lockObject)
                {
                    simpleCompressor.MakeUpGain = value;
                }
            }
        }

        public double Threshold
        {
            get { return simpleCompressor.Threshold; }
            set
            {
                lock (lockObject)
                {
                    simpleCompressor.Threshold = value;
                }
            }
        }

        public double Ratio
        {
            get { return simpleCompressor.Ratio; }
            set
            {
                lock (lockObject)
                {
                    simpleCompressor.Ratio = value;
                }
            }
        }

        public double Attack
        {
            get { return simpleCompressor.Attack; }
            set
            {
                lock (lockObject)
                {
                    simpleCompressor.Attack = value;
                }
            }
        }

        public double Release
        {
            get { return simpleCompressor.Release; }
            set
            {
                lock (lockObject)
                {
                    simpleCompressor.Release = value;
                }
            }
        }

        public bool Enabled { get; set; }

        public override long Length
        {
            get { return sourceStream.Length; }
        }

        public override long Position
        {
            get { return sourceStream.Position; }
            set
            {
                lock (lockObject)
                {
                    sourceStream.Position = value;
                }
            }
        }

        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        public override int BlockAlign
        {
            get { return sourceStream.BlockAlign; }
        }

        #endregion

        #region Конструктор

        public SimpleCompressorStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            channels = sourceStream.WaveFormat.Channels;
            bytesPerSample = sourceStream.WaveFormat.BitsPerSample / 8;
            simpleCompressor = new SimpleCompressor(5.0, 10.0, sourceStream.WaveFormat.SampleRate);
            simpleCompressor.Threshold = 16;
            simpleCompressor.Ratio = 6;
            simpleCompressor.MakeUpGain = 16;
        }

        #endregion

        #region Методы

        public override bool HasData(int count)
        {
            return sourceStream.HasData(count);
        }

        private void ReadSamples(byte[] buffer, int start, out double left, out double right)
        {
            if (bytesPerSample == 4)
            {
                left = BitConverter.ToSingle(buffer, start);
                if (channels > 1)
                {
                    right = BitConverter.ToSingle(buffer, start + bytesPerSample);
                }
                else
                {
                    right = left;
                }
            }
            else if (bytesPerSample == 2)
            {
                left = BitConverter.ToInt16(buffer, start) / 32768.0;
                if (channels > 1)
                {
                    right = BitConverter.ToInt16(buffer, start + bytesPerSample) / 32768.0;
                }
                else
                {
                    right = left;
                }
            }
            else
            {
                throw new InvalidOperationException(String.Format("Unsupported bytes per sample: {0}", bytesPerSample));
            }
        }

        private void WriteSamples(byte[] buffer, int start, double left, double right)
        {
            if (bytesPerSample == 4)
            {
                Array.Copy(BitConverter.GetBytes((float)left), 0, buffer, start, bytesPerSample);
                if (channels > 1)
                {
                    Array.Copy(BitConverter.GetBytes((float)right), 0, buffer, start + bytesPerSample, bytesPerSample);
                }
            }
            else if (bytesPerSample == 2)
            {
                Array.Copy(BitConverter.GetBytes((short)(left * 32768.0)), 0, buffer, start, bytesPerSample);
                if (channels > 1)
                {
                    Array.Copy(BitConverter.GetBytes((short)(right * 32768.0)), 0, buffer, start + bytesPerSample, bytesPerSample);
                }
            }
        }

        public override int Read(byte[] array, int offset, int count)
        {
            lock (lockObject)
            {
                if (Enabled)
                {
                    if (sourceBuffer == null || sourceBuffer.Length < count)
                    {
                        sourceBuffer = new byte[count];
                    }
                    int sourceBytesRead = sourceStream.Read(sourceBuffer, 0, count);
                    int sampleCount = sourceBytesRead / (bytesPerSample * channels);
                    for (int sample = 0; sample < sampleCount; sample++)
                    {
                        int start = sample * bytesPerSample * channels;
                        double in1;
                        double in2;
                        ReadSamples(sourceBuffer, start, out in1, out in2);
                        simpleCompressor.Process(ref in1, ref in2);
                        WriteSamples(array, offset + start, in1, in2);
                    }
                    return count;
                }
                return sourceStream.Read(array, offset, count);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (sourceStream != null)
                {
                    sourceStream.Dispose();
                    sourceStream = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}