using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NAudio.Utils;

namespace NAudio.Wave
{
    internal class Id3v2Tag
    {
        #region ����

        private readonly byte[] rawData;

        private long tagEndPosition;

        private long tagStartPosition;

        #endregion

        #region ��������

        public byte[] RawData
        {
            get { return rawData; }
        }

        #endregion

        #region �����������

        private Id3v2Tag(Stream input)
        {
            tagStartPosition = input.Position;
            BinaryReader reader = new BinaryReader(input);
            byte[] headerBytes = reader.ReadBytes(10);
            if ((headerBytes[0] == (byte)'I') &&
                (headerBytes[1] == (byte)'D') &&
                (headerBytes[2] == '3'))
            {
                if ((headerBytes[5] & 0x40) == 0x40)
                {
                    byte[] extendedHeader = reader.ReadBytes(4);
                    int extendedHeaderLength = extendedHeader[0] * (1 << 21);
                    extendedHeaderLength += extendedHeader[1] * (1 << 14);
                    extendedHeaderLength += extendedHeader[2] * (1 << 7);
                    extendedHeaderLength += extendedHeader[3];
                }

                int dataLength = headerBytes[6] * (1 << 21);
                dataLength += headerBytes[7] * (1 << 14);
                dataLength += headerBytes[8] * (1 << 7);
                dataLength += headerBytes[9];
                byte[] tagData = reader.ReadBytes(dataLength);

                if ((headerBytes[5] & 0x10) == 0x10)
                {
                    byte[] footer = reader.ReadBytes(10);
                }
            }
            else
            {
                input.Position -= 10;
                throw new FormatException("Not an ID3v2 tag");
            }
            tagEndPosition = input.Position;
            input.Position = tagStartPosition;
            rawData = reader.ReadBytes((int)(tagEndPosition - tagStartPosition));
        }

        #endregion

        #region ������

        public static Id3v2Tag ReadTag(Stream input)
        {
            try
            {
                return new Id3v2Tag(input);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public static Id3v2Tag Create(IEnumerable<KeyValuePair<string, string>> tags)
        {
            return Id3v2Tag.ReadTag(CreateId3v2TagStream(tags));
        }

        private static byte[] FrameSizeToBytes(int n)
        {
            byte[] result = BitConverter.GetBytes(n);
            Array.Reverse(result);
            return result;
        }

        private static byte[] CreateId3v2Frame(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            if (key.Length != 4)
            {
                throw new ArgumentOutOfRangeException("key", "key " + key + " must be 4 characters long");
            }

            const byte UnicodeEncoding = 01;
            byte[] UnicodeOrder = { 0xff, 0xfe };
            byte[] language = { 0, 0, 0 };
            byte[] shortDescription = { 0, 0 };

            byte[] body;
            if (key == "COMM")
            {
                body = ByteArrayExtensions.Concat(
                    new[] { UnicodeEncoding },
                    language,
                    shortDescription,
                    UnicodeOrder,
                    Encoding.Unicode.GetBytes(value));
            }
            else
            {
                body = ByteArrayExtensions.Concat(
                    new[] { UnicodeEncoding },
                    UnicodeOrder,
                    Encoding.Unicode.GetBytes(value));
            }

            return ByteArrayExtensions.Concat(
                Encoding.UTF8.GetBytes(key),
                FrameSizeToBytes(body.Length),
                new byte[] { 0, 0 },
                body);
        }

        private static byte[] GetId3TagHeaderSize(int size)
        {
            byte[] result = new byte[4];
            for (int idx = result.Length - 1; idx >= 0; idx--)
            {
                result[idx] = (byte)(size % 128);
                size = size / 128;
            }

            return result;
        }

        private static byte[] CreateId3v2TagHeader(IEnumerable<byte[]> frames)
        {
            int size = 0;
            foreach (byte[] frame in frames)
            {
                size += frame.Length;
            }

            byte[] tagHeader = ByteArrayExtensions.Concat(
                Encoding.UTF8.GetBytes("ID3"),
                new byte[] { 3, 0 },
                new byte[] { 0 },
                GetId3TagHeaderSize(size));
            return tagHeader;
        }

        private static Stream CreateId3v2TagStream(IEnumerable<KeyValuePair<string, string>> tags)
        {
            List<byte[]> frames = new List<byte[]>();
            foreach (KeyValuePair<string, string> tag in tags)
            {
                frames.Add(CreateId3v2Frame(tag.Key, tag.Value));
            }

            byte[] header = CreateId3v2TagHeader(frames);

            MemoryStream ms = new MemoryStream();
            ms.Write(header, 0, header.Length);
            foreach (byte[] frame in frames)
            {
                ms.Write(frame, 0, frame.Length);
            }

            ms.Position = 0;
            return ms;
        }

        #endregion
    }
}