using System;

namespace NAudio.Wave
{
    internal class Wave32To16Stream : WaveStream
    {
        #region Поля

        private readonly long length;

        private readonly object lockObject = new object();

        private readonly WaveFormat waveFormat;

        private bool clip;

        private long position;

        private WaveStream sourceStream;

        private float volume;

        #endregion

        #region Свойства

        public float Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        public override int BlockAlign
        {
            get { return sourceStream.BlockAlign / 2; }
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
                lock (lockObject)
                {
                    value -= (value % BlockAlign);
                    sourceStream.Position = value * 2;
                    position = value;
                }
            }
        }

        public override WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public bool Clip
        {
            get { return clip; }
            set { clip = value; }
        }

        #endregion

        #region Конструктор

        public Wave32To16Stream(WaveStream sourceStream)
        {
            if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Only 32 bit Floating point supported");
            }
            if (sourceStream.WaveFormat.BitsPerSample != 32)
            {
                throw new ArgumentException("Only 32 bit Floating point supported");
            }

            waveFormat = new WaveFormat(sourceStream.WaveFormat.SampleRate, 16, sourceStream.WaveFormat.Channels);
            volume = 1.0f;
            this.sourceStream = sourceStream;
            length = sourceStream.Length / 2;
            position = sourceStream.Position / 2;
        }

        #endregion

        #region Методы

        public override int Read(byte[] destBuffer, int offset, int numBytes)
        {
            lock (lockObject)
            {
                byte[] sourceBuffer = new byte[numBytes * 2];
                int bytesRead = sourceStream.Read(sourceBuffer, 0, numBytes * 2);
                Convert32To16(destBuffer, offset, sourceBuffer, bytesRead);
                position += (bytesRead / 2);
                return bytesRead / 2;
            }
        }

        private unsafe void Convert32To16(byte[] destBuffer, int offset, byte[] sourceBuffer, int bytesRead)
        {
            fixed (byte* pDestBuffer = &destBuffer[offset],
                pSourceBuffer = &sourceBuffer[0])
            {
                short* psDestBuffer = (short*)pDestBuffer;
                float* pfSourceBuffer = (float*)pSourceBuffer;

                int samplesRead = bytesRead / 4;
                for (int n = 0; n < samplesRead; n++)
                {
                    float sampleVal = pfSourceBuffer[n] * volume;
                    if (sampleVal > 1.0f)
                    {
                        psDestBuffer[n] = short.MaxValue;
                        clip = true;
                    }
                    else if (sampleVal < -1.0f)
                    {
                        psDestBuffer[n] = short.MinValue;
                        clip = true;
                    }
                    else
                    {
                        psDestBuffer[n] = (short)(sampleVal * 32767);
                    }
                }
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