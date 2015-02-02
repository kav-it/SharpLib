using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SharpLib.Audio.Utils;

namespace SharpLib.Audio.Wave
{
    internal class Id3v2Tag
    {
        #region Поля

        private long tagEndPosition;

        private long tagStartPosition;

        #endregion

        #region Свойства

        public byte[] RawData { get; private set; }

        #endregion

        #region Конструктор

        private Id3v2Tag(Stream input)
        {
            tagStartPosition = input.Position;
            var reader = new BinaryReader(input);
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
            RawData = reader.ReadBytes((int)(tagEndPosition - tagStartPosition));
        }

        #endregion

        #region Методы

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
            return ReadTag(CreateId3V2TagStream(tags));
        }

        private static byte[] FrameSizeToBytes(int n)
        {
            byte[] result = BitConverter.GetBytes(n);
            Array.Reverse(result);
            return result;
        }

        private static byte[] CreateId3V2Frame(string key, string value)
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

            const byte UNICODE_ENCODING = 01;
            byte[] unicodeOrder = { 0xff, 0xfe };
            byte[] language = { 0, 0, 0 };
            byte[] shortDescription = { 0, 0 };

            byte[] body;
            if (key == "COMM")
            {
                body = ByteArrayExtensions.Concat(
                    new[] { UNICODE_ENCODING },
                    language,
                    shortDescription,
                    unicodeOrder,
                    Encoding.Unicode.GetBytes(value));
            }
            else
            {
                body = ByteArrayExtensions.Concat(
                    new[] { UNICODE_ENCODING },
                    unicodeOrder,
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

        private static byte[] CreateId3V2TagHeader(IEnumerable<byte[]> frames)
        {
            int size = frames.Sum(frame => frame.Length);

            var tagHeader = ByteArrayExtensions.Concat(
                Encoding.UTF8.GetBytes("ID3"),
                new byte[] { 3, 0 },
                new byte[] { 0 },
                GetId3TagHeaderSize(size));

            return tagHeader;
        }

        private static Stream CreateId3V2TagStream(IEnumerable<KeyValuePair<string, string>> tags)
        {
            var frames = new List<byte[]>();
            foreach (KeyValuePair<string, string> tag in tags)
            {
                frames.Add(CreateId3V2Frame(tag.Key, tag.Value));
            }

            var header = CreateId3V2TagHeader(frames);

            var ms = new MemoryStream();
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