using System;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.Wave
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal class AdpcmWaveFormat : WaveFormat
    {
        private readonly short samplesPerBlock;

        private readonly short numCoeff;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        private readonly short[] coefficients;

        private AdpcmWaveFormat()
            : this(8000, 1)
        {
        }

        public int SamplesPerBlock
        {
            get { return samplesPerBlock; }
        }

        public int NumCoefficients
        {
            get { return numCoeff; }
        }

        public short[] Coefficients
        {
            get { return coefficients; }
        }

        public AdpcmWaveFormat(int sampleRate, int channels) :
            base(sampleRate, 0, channels)
        {
            waveFormatTag = WaveFormatEncoding.Adpcm;

            extraSize = 32;
            switch (this.sampleRate)
            {
                case 8000:
                case 11025:
                    blockAlign = 256;
                    break;
                case 22050:
                    blockAlign = 512;
                    break;
                case 44100:
                default:
                    blockAlign = 1024;
                    break;
            }

            bitsPerSample = 4;
            samplesPerBlock = (short)((((blockAlign - (7 * channels)) * 8) / (bitsPerSample * channels)) + 2);
            averageBytesPerSecond =
                ((SampleRate * blockAlign) / samplesPerBlock);

            numCoeff = 7;
            coefficients = new short[14]
            {
                256, 0, 512, -256, 0, 0, 192, 64, 240, 0, 460, -208, 392, -232
            };
        }

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(samplesPerBlock);
            writer.Write(numCoeff);
            foreach (short coefficient in coefficients)
            {
                writer.Write(coefficient);
            }
        }

        public override string ToString()
        {
            return String.Format("Microsoft ADPCM {0} Hz {1} channels {2} bits per sample {3} samples per block",
                SampleRate, channels, bitsPerSample, samplesPerBlock);
        }
    }
}