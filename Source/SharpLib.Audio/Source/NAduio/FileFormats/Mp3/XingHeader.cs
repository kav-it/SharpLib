using System;

namespace SharpLib.Audio.Wave
{
    internal class XingHeader
    {
        #region Перечисления

        [Flags]
        private enum XingHeaderOptions
        {
            Frames = 1,

            Bytes = 2,

            Toc = 4,

            VbrScale = 8
        }

        #endregion

        #region Поля

        private static int[] sr_table = { 44100, 48000, 32000, 99999 };

        private int bytesOffset = -1;

        private int endOffset;

        private Mp3Frame frame;

        private int framesOffset = -1;

        private int startOffset;

        private int tocOffset = -1;

        private int vbrScale = -1;

        #endregion

        #region Свойства

        public int Frames
        {
            get
            {
                if (framesOffset == -1)
                {
                    return -1;
                }
                return ReadBigEndian(frame.RawData, framesOffset);
            }
            set
            {
                if (framesOffset == -1)
                {
                    throw new InvalidOperationException("Frames flag is not set");
                }
                WriteBigEndian(frame.RawData, framesOffset, value);
            }
        }

        public int Bytes
        {
            get
            {
                if (bytesOffset == -1)
                {
                    return -1;
                }
                return ReadBigEndian(frame.RawData, bytesOffset);
            }
            set
            {
                if (framesOffset == -1)
                {
                    throw new InvalidOperationException("Bytes flag is not set");
                }
                WriteBigEndian(frame.RawData, bytesOffset, value);
            }
        }

        public int VbrScale
        {
            get { return vbrScale; }
        }

        public Mp3Frame Mp3Frame
        {
            get { return frame; }
        }

        #endregion

        #region Конструктор

        private XingHeader()
        {
        }

        #endregion

        #region Методы

        private static int ReadBigEndian(byte[] buffer, int offset)
        {
            int x;

            x = buffer[offset + 0];
            x <<= 8;
            x |= buffer[offset + 1];
            x <<= 8;
            x |= buffer[offset + 2];
            x <<= 8;
            x |= buffer[offset + 3];

            return x;
        }

        private void WriteBigEndian(byte[] buffer, int offset, int value)
        {
            byte[] littleEndian = BitConverter.GetBytes(value);
            for (int n = 0; n < 4; n++)
            {
                buffer[offset + 4 - n] = littleEndian[n];
            }
        }

        public static XingHeader LoadXingHeader(Mp3Frame frame)
        {
            XingHeader xingHeader = new XingHeader();
            xingHeader.frame = frame;
            int offset = 0;

            if (frame.MpegVersion == MpegVersion.Version1)
            {
                if (frame.ChannelMode != ChannelMode.Mono)
                {
                    offset = 32 + 4;
                }
                else
                {
                    offset = 17 + 4;
                }
            }
            else if (frame.MpegVersion == MpegVersion.Version2)
            {
                if (frame.ChannelMode != ChannelMode.Mono)
                {
                    offset = 17 + 4;
                }
                else
                {
                    offset = 9 + 4;
                }
            }
            else
            {
                return null;
            }

            if ((frame.RawData[offset + 0] == 'X') &&
                (frame.RawData[offset + 1] == 'i') &&
                (frame.RawData[offset + 2] == 'n') &&
                (frame.RawData[offset + 3] == 'g'))
            {
                xingHeader.startOffset = offset;
                offset += 4;
            }
            else
            {
                return null;
            }

            XingHeaderOptions flags = (XingHeaderOptions)ReadBigEndian(frame.RawData, offset);
            offset += 4;

            if ((flags & XingHeaderOptions.Frames) != 0)
            {
                xingHeader.framesOffset = offset;
                offset += 4;
            }
            if ((flags & XingHeaderOptions.Bytes) != 0)
            {
                xingHeader.bytesOffset = offset;
                offset += 4;
            }
            if ((flags & XingHeaderOptions.Toc) != 0)
            {
                xingHeader.tocOffset = offset;
                offset += 100;
            }
            if ((flags & XingHeaderOptions.VbrScale) != 0)
            {
                xingHeader.vbrScale = ReadBigEndian(frame.RawData, offset);
                offset += 4;
            }
            xingHeader.endOffset = offset;
            return xingHeader;
        }

        #endregion
    }
}