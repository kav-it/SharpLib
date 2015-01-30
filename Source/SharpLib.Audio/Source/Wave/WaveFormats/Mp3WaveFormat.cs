using System;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.Wave
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    internal class Mp3WaveFormat : WaveFormat
    {
        public Mp3WaveFormatId id;

        public Mp3WaveFormatFlags flags;

        public ushort blockSize;

        public ushort framesPerBlock;

        public ushort codecDelay;

        private const short MP3_WAVE_FORMAT_EXTRA_BYTES = 12;

        public Mp3WaveFormat(int sampleRate, int channels, int blockSize, int bitRate)
        {
            waveFormatTag = WaveFormatEncoding.MpegLayer3;
            this.channels = (short)channels;
            averageBytesPerSecond = bitRate / 8;
            bitsPerSample = 0;
            blockAlign = 1;
            this.sampleRate = sampleRate;

            extraSize = MP3_WAVE_FORMAT_EXTRA_BYTES;
            id = Mp3WaveFormatId.Mpeg;
            flags = Mp3WaveFormatFlags.PaddingIso;
            this.blockSize = (ushort)blockSize;
            framesPerBlock = 1;
            codecDelay = 0;
        }
    }

    [Flags]
    internal enum Mp3WaveFormatFlags
    {
        PaddingIso = 0,

        PaddingOn = 1,

        PaddingOff = 2,
    }

    internal enum Mp3WaveFormatId : ushort
    {
        Unknown = 0,

        Mpeg = 1,

        ConstantFrameSize = 2
    }
}