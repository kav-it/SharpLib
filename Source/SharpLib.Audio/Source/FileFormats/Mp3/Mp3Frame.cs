using System;
using System.IO;

namespace NAudio.Wave
{
    internal class Mp3Frame
    {
        #region Константы

        private const int MaxFrameLength = 16 * 1024;

        #endregion

        #region Поля

        private static readonly int[,,] bitRates =
        {
            {
                { 0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448 },
                { 0, 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384 },
                { 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 }
            },
            {
                { 0, 32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256 },
                { 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160 },
                { 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160 }
            }
        };

        private static readonly int[] sampleRatesVersion1 = { 44100, 48000, 32000 };

        private static readonly int[] sampleRatesVersion2 = { 22050, 24000, 16000 };

        private static readonly int[] sampleRatesVersion25 = { 11025, 12000, 8000 };

        private static readonly int[,] samplesPerFrame =
        {
            {
                384,
                1152,
                1152
            },
            {
                384,
                1152,
                576
            }
        };

        #endregion

        #region Свойства

        public int SampleRate { get; private set; }

        public int FrameLength { get; private set; }

        public int BitRate { get; private set; }

        public byte[] RawData { get; private set; }

        public MpegVersion MpegVersion { get; private set; }

        public MpegLayer MpegLayer { get; private set; }

        public ChannelMode ChannelMode { get; private set; }

        public int SampleCount { get; private set; }

        public int ChannelExtension { get; private set; }

        public int BitRateIndex { get; private set; }

        public bool Copyright { get; private set; }

        public bool CrcPresent { get; private set; }

        public long FileOffset { get; private set; }

        #endregion

        #region Конструктор

        private Mp3Frame()
        {
        }

        #endregion

        #region Методы

        public static Mp3Frame LoadFromStream(Stream input)
        {
            return LoadFromStream(input, true);
        }

        public static Mp3Frame LoadFromStream(Stream input, bool readData)
        {
            var frame = new Mp3Frame();
            frame.FileOffset = input.Position;
            byte[] headerBytes = new byte[4];
            int bytesRead = input.Read(headerBytes, 0, headerBytes.Length);
            if (bytesRead < headerBytes.Length)
            {
                return null;
            }
            while (!IsValidHeader(headerBytes, frame))
            {
                headerBytes[0] = headerBytes[1];
                headerBytes[1] = headerBytes[2];
                headerBytes[2] = headerBytes[3];
                bytesRead = input.Read(headerBytes, 3, 1);
                if (bytesRead < 1)
                {
                    return null;
                }
                frame.FileOffset++;
            }

            int bytesRequired = frame.FrameLength - 4;
            if (readData)
            {
                frame.RawData = new byte[frame.FrameLength];
                Array.Copy(headerBytes, frame.RawData, 4);
                bytesRead = input.Read(frame.RawData, 4, bytesRequired);
                if (bytesRead < bytesRequired)
                {
                    throw new EndOfStreamException("Unexpected end of stream before frame complete");
                }
            }
            else
            {
                input.Position += bytesRequired;
            }

            return frame;
        }

        private static bool IsValidHeader(byte[] headerBytes, Mp3Frame frame)
        {
            if ((headerBytes[0] == 0xFF) && ((headerBytes[1] & 0xE0) == 0xE0))
            {
                frame.MpegVersion = (MpegVersion)((headerBytes[1] & 0x18) >> 3);
                if (frame.MpegVersion == MpegVersion.Reserved)
                {
                    return false;
                }

                frame.MpegLayer = (MpegLayer)((headerBytes[1] & 0x06) >> 1);

                if (frame.MpegLayer == MpegLayer.Reserved)
                {
                    return false;
                }
                int layerIndex = frame.MpegLayer == MpegLayer.Layer1 ? 0 : frame.MpegLayer == MpegLayer.Layer2 ? 1 : 2;
                frame.CrcPresent = (headerBytes[1] & 0x01) == 0x00;
                frame.BitRateIndex = (headerBytes[2] & 0xF0) >> 4;
                if (frame.BitRateIndex == 15)
                {
                    return false;
                }
                int versionIndex = frame.MpegVersion == Wave.MpegVersion.Version1 ? 0 : 1;
                frame.BitRate = bitRates[versionIndex, layerIndex, frame.BitRateIndex] * 1000;
                if (frame.BitRate == 0)
                {
                    return false;
                }
                int sampleFrequencyIndex = (headerBytes[2] & 0x0C) >> 2;
                if (sampleFrequencyIndex == 3)
                {
                    return false;
                }

                if (frame.MpegVersion == MpegVersion.Version1)
                {
                    frame.SampleRate = sampleRatesVersion1[sampleFrequencyIndex];
                }
                else if (frame.MpegVersion == MpegVersion.Version2)
                {
                    frame.SampleRate = sampleRatesVersion2[sampleFrequencyIndex];
                }
                else
                {
                    frame.SampleRate = sampleRatesVersion25[sampleFrequencyIndex];
                }

                bool padding = (headerBytes[2] & 0x02) == 0x02;
                bool privateBit = (headerBytes[2] & 0x01) == 0x01;
                frame.ChannelMode = (ChannelMode)((headerBytes[3] & 0xC0) >> 6);
                frame.ChannelExtension = (headerBytes[3] & 0x30) >> 4;
                if (frame.ChannelExtension != 0 && frame.ChannelMode != ChannelMode.JointStereo)
                {
                    return false;
                }

                frame.Copyright = (headerBytes[3] & 0x08) == 0x08;
                bool original = (headerBytes[3] & 0x04) == 0x04;
                int emphasis = (headerBytes[3] & 0x03);

                int nPadding = padding ? 1 : 0;

                frame.SampleCount = samplesPerFrame[versionIndex, layerIndex];
                int coefficient = frame.SampleCount / 8;
                if (frame.MpegLayer == MpegLayer.Layer1)
                {
                    frame.FrameLength = (coefficient * frame.BitRate / frame.SampleRate + nPadding) * 4;
                }
                else
                {
                    frame.FrameLength = (coefficient * frame.BitRate) / frame.SampleRate + nPadding;
                }

                if (frame.FrameLength > MaxFrameLength)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        #endregion
    }
}