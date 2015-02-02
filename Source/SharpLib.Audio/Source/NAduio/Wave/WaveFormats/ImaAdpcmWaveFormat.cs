using System.Runtime.InteropServices;

namespace SharpLib.Audio.Wave
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal class ImaAdpcmWaveFormat : WaveFormat
    {
        private short samplesPerBlock;

        private ImaAdpcmWaveFormat()
        {
        }

        public ImaAdpcmWaveFormat(int sampleRate, int channels, int bitsPerSample)
        {
            waveFormatTag = WaveFormatEncoding.DviAdpcm;
            this.sampleRate = sampleRate;
            this.channels = (short)channels;
            this.bitsPerSample = (short)bitsPerSample;
            extraSize = 2;
            blockAlign = 0;
            averageBytesPerSecond = 0;
            samplesPerBlock = 0;
        }
    }
}