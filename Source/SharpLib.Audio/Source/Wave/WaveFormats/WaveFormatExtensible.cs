using System;
using System.Runtime.InteropServices;

using SharpLib.Audio.Dmo;

namespace SharpLib.Audio.Wave
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    internal class WaveFormatExtensible : WaveFormat
    {
        private readonly short wValidBitsPerSample;

        private readonly int dwChannelMask;

        private Guid subFormat;

        private WaveFormatExtensible()
        {
        }

        public WaveFormatExtensible(int rate, int bits, int channels)
            : base(rate, bits, channels)
        {
            waveFormatTag = WaveFormatEncoding.Extensible;
            extraSize = 22;
            wValidBitsPerSample = (short)bits;
            for (int n = 0; n < channels; n++)
            {
                dwChannelMask |= (1 << n);
            }
            if (bits == 32)
            {
                subFormat = AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT;
            }
            else
            {
                subFormat = AudioMediaSubtypes.MEDIASUBTYPE_PCM;
            }
        }

        public WaveFormat ToStandardWaveFormat()
        {
            if (subFormat == AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT && bitsPerSample == 32)
            {
                return CreateIeeeFloatWaveFormat(sampleRate, channels);
            }
            if (subFormat == AudioMediaSubtypes.MEDIASUBTYPE_PCM)
            {
                return new WaveFormat(sampleRate, bitsPerSample, channels);
            }
            throw new InvalidOperationException("Not a recognised PCM or IEEE float format");
        }

        public Guid SubFormat
        {
            get { return subFormat; }
        }

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(wValidBitsPerSample);
            writer.Write(dwChannelMask);
            byte[] guid = subFormat.ToByteArray();
            writer.Write(guid, 0, guid.Length);
        }

        public override string ToString()
        {
            return String.Format("{0} wBitsPerSample:{1} dwChannelMask:{2} subFormat:{3} extraSize:{4}",
                base.ToString(),
                wValidBitsPerSample,
                dwChannelMask,
                subFormat,
                extraSize);
        }
    }
}