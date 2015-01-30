using System.IO;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.Wave
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal class Gsm610WaveFormat : WaveFormat
    {
        private readonly short samplesPerBlock;

        public Gsm610WaveFormat()
        {
            waveFormatTag = WaveFormatEncoding.Gsm610;
            channels = 1;
            averageBytesPerSecond = 1625;
            bitsPerSample = 0;
            blockAlign = 65;
            sampleRate = 8000;

            extraSize = 2;
            samplesPerBlock = 320;
        }

        public short SamplesPerBlock
        {
            get { return samplesPerBlock; }
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(samplesPerBlock);
        }
    }
}