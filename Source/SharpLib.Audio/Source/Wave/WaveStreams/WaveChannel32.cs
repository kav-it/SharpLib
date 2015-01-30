using System;

using NAudio.Wave.SampleProviders;

namespace NAudio.Wave
{
    internal class WaveChannel32 : WaveStream, ISampleNotifier
    {
        #region Поля

        private readonly int destBytesPerSample;

        private readonly long length;

        private readonly object lockObject = new object();

        private readonly SampleEventArgs sampleEventArgs = new SampleEventArgs(0, 0);

        private readonly ISampleChunkConverter sampleProvider;

        private readonly int sourceBytesPerSample;

        private readonly WaveFormat waveFormat;

        private volatile float pan;

        private long position;

        private WaveStream sourceStream;

        private volatile float volume;

        #endregion

        #region Свойства

        public override int BlockAlign
        {
            get { return (int)SourceToDest(sourceStream.BlockAlign); }
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
                    if (value < 0)
                    {
                        sourceStream.Position = 0;
                    }
                    else
                    {
                        sourceStream.Position = DestToSource(value);
                    }

                    position = SourceToDest(sourceStream.Position);
                }
            }
        }

        public bool PadWithZeroes { get; set; }

        public override WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public float Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        public float Pan
        {
            get { return pan; }
            set { pan = value; }
        }

        #endregion

        #region События

        public event EventHandler<SampleEventArgs> Sample;

        #endregion

        #region Конструктор

        public WaveChannel32(WaveStream sourceStream, float volume, float pan)
        {
            PadWithZeroes = true;

            var providers = new ISampleChunkConverter[]
            {
                new Mono8SampleChunkConverter(),
                new Stereo8SampleChunkConverter(),
                new Mono16SampleChunkConverter(),
                new Stereo16SampleChunkConverter(),
                new Mono24SampleChunkConverter(),
                new Stereo24SampleChunkConverter(),
                new MonoFloatSampleChunkConverter(),
                new StereoFloatSampleChunkConverter()
            };
            foreach (var provider in providers)
            {
                if (provider.Supports(sourceStream.WaveFormat))
                {
                    sampleProvider = provider;
                    break;
                }
            }

            if (sampleProvider == null)
            {
                throw new ArgumentException("Unsupported sourceStream format");
            }

            waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sourceStream.WaveFormat.SampleRate, 2);
            destBytesPerSample = 8;

            this.sourceStream = sourceStream;
            this.volume = volume;
            this.pan = pan;
            sourceBytesPerSample = sourceStream.WaveFormat.Channels * sourceStream.WaveFormat.BitsPerSample / 8;

            length = SourceToDest(sourceStream.Length);
            position = 0;
        }

        public WaveChannel32(WaveStream sourceStream)
            :
                this(sourceStream, 1.0f, 0.0f)
        {
        }

        #endregion

        #region Методы

        private long SourceToDest(long sourceBytes)
        {
            return (sourceBytes / sourceBytesPerSample) * destBytesPerSample;
        }

        private long DestToSource(long destBytes)
        {
            return (destBytes / destBytesPerSample) * sourceBytesPerSample;
        }

        public override int Read(byte[] destBuffer, int offset, int numBytes)
        {
            lock (lockObject)
            {
                int bytesWritten = 0;
                WaveBuffer destWaveBuffer = new WaveBuffer(destBuffer);

                if (position < 0)
                {
                    bytesWritten = (int)Math.Min(numBytes, 0 - position);
                    for (int n = 0; n < bytesWritten; n++)
                    {
                        destBuffer[n + offset] = 0;
                    }
                }
                if (bytesWritten < numBytes)
                {
                    sampleProvider.LoadNextChunk(sourceStream, (numBytes - bytesWritten) / 8);
                    float left, right;

                    int outIndex = (offset / 4) + bytesWritten / 4;
                    while (sampleProvider.GetNextSample(out left, out right) && bytesWritten < numBytes)
                    {
                        left = (pan <= 0) ? left : (left * (1 - pan) / 2.0f);
                        right = (pan >= 0) ? right : (right * (pan + 1) / 2.0f);
                        left *= volume;
                        right *= volume;
                        destWaveBuffer.FloatBuffer[outIndex++] = left;
                        destWaveBuffer.FloatBuffer[outIndex++] = right;
                        bytesWritten += 8;
                        if (Sample != null)
                        {
                            RaiseSample(left, right);
                        }
                    }
                }

                if (PadWithZeroes && bytesWritten < numBytes)
                {
                    Array.Clear(destBuffer, offset + bytesWritten, numBytes - bytesWritten);
                    bytesWritten = numBytes;
                }
                position += bytesWritten;
                return bytesWritten;
            }
        }

        public override bool HasData(int count)
        {
            bool sourceHasData = sourceStream.HasData(count);

            if (sourceHasData)
            {
                if (position + count < 0)
                {
                    return false;
                }
                return (position < length) && (volume != 0);
            }
            return false;
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
            else
            {
                System.Diagnostics.Debug.Assert(false, "WaveChannel32 was not Disposed");
            }
            base.Dispose(disposing);
        }

        private void RaiseSample(float left, float right)
        {
            sampleEventArgs.Left = left;
            sampleEventArgs.Right = right;
            Sample(this, sampleEventArgs);
        }

        #endregion
    }
}