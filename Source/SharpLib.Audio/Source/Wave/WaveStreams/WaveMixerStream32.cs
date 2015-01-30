using System;
using System.Collections.Generic;

namespace NAudio.Wave
{
    internal class WaveMixerStream32 : WaveStream
    {
        #region Поля

        private readonly int bytesPerSample;

        private readonly List<WaveStream> inputStreams;

        private readonly object inputsLock;

        private long length;

        private long position;

        private WaveFormat waveFormat;

        #endregion

        #region Свойства

        public int InputCount
        {
            get { return inputStreams.Count; }
        }

        public bool AutoStop { get; set; }

        public override int BlockAlign
        {
            get { return waveFormat.BlockAlign; }
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
                lock (inputsLock)
                {
                    value = Math.Min(value, Length);
                    foreach (WaveStream inputStream in inputStreams)
                    {
                        inputStream.Position = Math.Min(value, inputStream.Length);
                    }
                    position = value;
                }
            }
        }

        public override WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        #endregion

        #region Конструктор

        public WaveMixerStream32()
        {
            AutoStop = true;
            waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
            bytesPerSample = 4;
            inputStreams = new List<WaveStream>();
            inputsLock = new object();
        }

        public WaveMixerStream32(IEnumerable<WaveStream> inputStreams, bool autoStop)
            : this()
        {
            AutoStop = autoStop;

            foreach (var inputStream in inputStreams)
            {
                AddInputStream(inputStream);
            }
        }

        #endregion

        #region Методы

        public void AddInputStream(WaveStream waveStream)
        {
            if (waveStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Must be IEEE floating point", "waveStream");
            }
            if (waveStream.WaveFormat.BitsPerSample != 32)
            {
                throw new ArgumentException("Only 32 bit audio currently supported", "waveStream");
            }

            if (inputStreams.Count == 0)
            {
                int sampleRate = waveStream.WaveFormat.SampleRate;
                int channels = waveStream.WaveFormat.Channels;
                waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
            }
            else
            {
                if (!waveStream.WaveFormat.Equals(waveFormat))
                {
                    throw new ArgumentException("All incoming channels must have the same format", "waveStream");
                }
            }

            lock (inputsLock)
            {
                inputStreams.Add(waveStream);
                length = Math.Max(length, waveStream.Length);

                waveStream.Position = Position;
            }
        }

        public void RemoveInputStream(WaveStream waveStream)
        {
            lock (inputsLock)
            {
                if (inputStreams.Remove(waveStream))
                {
                    long newLength = 0;
                    foreach (var inputStream in inputStreams)
                    {
                        newLength = Math.Max(newLength, inputStream.Length);
                    }
                    length = newLength;
                }
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (AutoStop)
            {
                if (position + count > length)
                {
                    count = (int)(length - position);
                }

                System.Diagnostics.Debug.Assert(count >= 0, "length and position mismatch");
            }

            if (count % bytesPerSample != 0)
            {
                throw new ArgumentException("Must read an whole number of samples", "count");
            }

            Array.Clear(buffer, offset, count);
            int bytesRead = 0;

            var readBuffer = new byte[count];
            lock (inputsLock)
            {
                foreach (var inputStream in inputStreams)
                {
                    if (inputStream.HasData(count))
                    {
                        int readFromThisStream = inputStream.Read(readBuffer, 0, count);

                        bytesRead = Math.Max(bytesRead, readFromThisStream);
                        if (readFromThisStream > 0)
                        {
                            Sum32BitAudio(buffer, offset, readBuffer, readFromThisStream);
                        }
                    }
                    else
                    {
                        bytesRead = Math.Max(bytesRead, count);
                        inputStream.Position += count;
                    }
                }
            }
            position += count;
            return count;
        }

        private static unsafe void Sum32BitAudio(byte[] destBuffer, int offset, byte[] sourceBuffer, int bytesRead)
        {
            fixed (byte* pDestBuffer = &destBuffer[offset],
                pSourceBuffer = &sourceBuffer[0])
            {
                float* pfDestBuffer = (float*)pDestBuffer;
                float* pfReadBuffer = (float*)pSourceBuffer;
                int samplesRead = bytesRead / 4;
                for (int n = 0; n < samplesRead; n++)
                {
                    pfDestBuffer[n] += pfReadBuffer[n];
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (inputsLock)
                {
                    foreach (WaveStream inputStream in inputStreams)
                    {
                        inputStream.Dispose();
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "WaveMixerStream32 was not disposed");
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}