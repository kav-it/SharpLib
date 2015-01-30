using System.IO;
using System.Runtime.InteropServices;

namespace NAudio.Wave
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal class TrueSpeechWaveFormat : WaveFormat
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        private readonly short[] unknown;

        public TrueSpeechWaveFormat()
        {
            waveFormatTag = WaveFormatEncoding.DspGroupTrueSpeech;
            channels = 1;
            averageBytesPerSecond = 1067;
            bitsPerSample = 1;
            blockAlign = 32;
            sampleRate = 8000;

            extraSize = 32;
            unknown = new short[16];
            unknown[0] = 1;
            unknown[1] = 0xF0;
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            foreach (short val in unknown)
            {
                writer.Write(val);
            }
        }
    }
}