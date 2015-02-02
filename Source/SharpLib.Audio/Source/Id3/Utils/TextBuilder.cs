using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using Id3Lib.Exceptions;

namespace Id3Lib
{
    [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
    internal enum TextCode : byte
    {
        Ascii = 0x00,

        Utf16 = 0x01,

        Utf16BE = 0x02,

        Utf8 = 0x03
    };

    internal static class TextBuilder
    {
        #region Ìועמהû

        public static string ReadText(byte[] frame, ref int index, TextCode code)
        {
            switch (code)
            {
                case TextCode.Ascii:
                    {
                        return ReadASCII(frame, ref index);
                    }
                case TextCode.Utf16:
                    {
                        return ReadUTF16(frame, ref index);
                    }
                case TextCode.Utf16BE:
                    {
                        return ReadUTF16BE(frame, ref index);
                    }
                case TextCode.Utf8:
                    {
                        return ReadUTF8(frame, ref index);
                    }
                default:
                    {
                        throw new InvalidFrameException("Invalid text code string type.");
                    }
            }
        }

        public static string ReadTextEnd(byte[] frame, int index, TextCode code)
        {
            switch (code)
            {
                case TextCode.Ascii:
                    {
                        return ReadASCIIEnd(frame, index);
                    }
                case TextCode.Utf16:
                    {
                        return ReadUTF16End(frame, index);
                    }
                case TextCode.Utf16BE:
                    {
                        return ReadUTF16BEEnd(frame, index);
                    }
                case TextCode.Utf8:
                    {
                        return ReadUTF8End(frame, index);
                    }
                default:
                    {
                        throw new InvalidFrameException("Invalid text code string type.");
                    }
            }
        }

        public static string ReadASCII(byte[] frame, ref int index)
        {
            string text = null;
            int count = Memory.FindByte(frame, 0, index);
            if (count == -1)
            {
                throw new InvalidFrameException("Invalid ASCII string size");
            }

            if (count > 0)
            {
                var encoding = Encoding.GetEncoding(1252);
                text = encoding.GetString(frame, index, count);
                index += count;
            }
            index++;
            return text;
        }

        public static string ReadUTF16(byte[] frame, ref int index)
        {
            if (index >= frame.Length - 2)
            {
                throw new InvalidFrameException("ReadUTF16: string must be terminated");
            }

            if (frame[index] == 0xfe && frame[index + 1] == 0xff)
            {
                index += 2;
                return ReadUTF16BE(frame, ref index);
            }
            if (frame[index] == 0xff && frame[index + 1] == 0xfe)
            {
                index += 2;
                return ReadUTF16LE(frame, ref index);
            }
            if (frame[index] == 0x00 && frame[index + 1] == 0x00)
            {
                index += 2;
                return "";
            }

            throw new InvalidFrameException("Invalid UTF16 string.");
        }

        public static string ReadUTF16BE(byte[] frame, ref int index)
        {
            UnicodeEncoding encoding = new UnicodeEncoding(true, false);
            int count = Memory.FindShort(frame, 0, index);
            if (count == -1)
            {
                throw new InvalidFrameException("Invalid UTF16BE string size");
            }

            string text = encoding.GetString(frame, index, count);
            index += count;
            index += 2;
            return text;
        }

        private static string ReadUTF16LE(byte[] frame, ref int index)
        {
            UnicodeEncoding encoding = new UnicodeEncoding(false, false);
            int count = Memory.FindShort(frame, 0, index);
            if (count == -1)
            {
                throw new InvalidFrameException("Invalid UTF16LE string size");
            }

            string text = encoding.GetString(frame, index, count);
            index += count;
            index += 2;
            return text;
        }

        public static string ReadUTF8(byte[] frame, ref int index)
        {
            string text = null;
            int count = Memory.FindByte(frame, 0, index);
            if (count == -1)
            {
                throw new InvalidFrameException("Invalid UTF8 string size");
            }
            if (count > 0)
            {
                text = Encoding.UTF8.GetString(frame, index, count);
                index += count;
            }
            index++;
            return text;
        }

        public static string ReadASCIIEnd(byte[] frame, int index)
        {
            Encoding encoding = Encoding.GetEncoding(1252);
            return encoding.GetString(frame, index, frame.Length - index);
        }

        public static string ReadUTF16End(byte[] frame, int index)
        {
            if (index >= frame.Length - 2)
            {
                return "";
            }

            if (frame[index] == 0xfe && frame[index + 1] == 0xff)
            {
                return ReadUTF16BEEnd(frame, index + 2);
            }

            if (frame[index] == 0xff && frame[index + 1] == 0xfe)
            {
                return ReadUTF16LEEnd(frame, index + 2);
            }

            throw new InvalidFrameException("Invalid UTF16 string.");
        }

        public static string ReadUTF16BEEnd(byte[] frame, int index)
        {
            var encoding = new UnicodeEncoding(true, false);
            return encoding.GetString(frame, index, frame.Length - index);
        }

        private static string ReadUTF16LEEnd(byte[] frame, int index)
        {
            var encoding = new UnicodeEncoding(false, false);
            return encoding.GetString(frame, index, frame.Length - index);
        }

        public static string ReadUTF8End(byte[] frame, int index)
        {
            return Encoding.UTF8.GetString(frame, index, frame.Length - index);
        }

        public static byte[] WriteText(string text, TextCode code)
        {
            switch (code)
            {
                case TextCode.Ascii:
                    {
                        return WriteASCII(text);
                    }
                case TextCode.Utf16:
                    {
                        return WriteUTF16(text);
                    }
                case TextCode.Utf16BE:
                    {
                        return WriteUTF16BE(text);
                    }
                case TextCode.Utf8:
                    {
                        return WriteUTF8(text);
                    }
                default:
                    {
                        throw new InvalidFrameException("Invalid text code string type.");
                    }
            }
        }

        public static byte[] WriteTextEnd(string text, TextCode code)
        {
            switch (code)
            {
                case TextCode.Ascii:
                    {
                        return WriteASCIIEnd(text);
                    }
                case TextCode.Utf16:
                    {
                        return WriteUTF16End(text);
                    }
                case TextCode.Utf16BE:
                    {
                        return WriteUTF16BEEnd(text);
                    }
                case TextCode.Utf8:
                    {
                        return WriteUTF8End(text);
                    }
                default:
                    {
                        throw new InvalidFrameException("Invalid text code string type.");
                    }
            }
        }

        public static byte[] WriteASCII(string text)
        {
            var buffer = new MemoryStream();
            var writer = new BinaryWriter(buffer);
            if (String.IsNullOrEmpty(text))
            {
                writer.Write((byte)0);
                return buffer.ToArray();
            }
            var encoding = Encoding.GetEncoding(1252);
            writer.Write(encoding.GetBytes(text));
            writer.Write((byte)0);
            return buffer.ToArray();
        }

        public static byte[] WriteUTF16(string text)
        {
            var buffer = new MemoryStream();
            var writer = new BinaryWriter(buffer);
            if (String.IsNullOrEmpty(text))
            {
                writer.Write((ushort)0);
                return buffer.ToArray();
            }
            writer.Write((byte)0xff);
            writer.Write((byte)0xfe);
            var encoding = new UnicodeEncoding(false, false);
            writer.Write(encoding.GetBytes(text));
            writer.Write((ushort)0);
            return buffer.ToArray();
        }

        public static byte[] WriteUTF16BE(string text)
        {
            var buffer = new MemoryStream();
            var writer = new BinaryWriter(buffer);
            var encoding = new UnicodeEncoding(true, false);
            if (String.IsNullOrEmpty(text))
            {
                writer.Write((ushort)0);
                return buffer.ToArray();
            }
            writer.Write(encoding.GetBytes(text));
            writer.Write((ushort)0);
            return buffer.ToArray();
        }

        public static byte[] WriteUTF8(string text)
        {
            var buffer = new MemoryStream();
            var writer = new BinaryWriter(buffer);
            if (String.IsNullOrEmpty(text))
            {
                writer.Write((byte)0);
                return buffer.ToArray();
            }
            writer.Write(Encoding.UTF8.GetBytes(text));
            writer.Write((byte)0);
            return buffer.ToArray();
        }

        public static byte[] WriteASCIIEnd(string text)
        {
            var buffer = new MemoryStream();
            var writer = new BinaryWriter(buffer);
            if (String.IsNullOrEmpty(text))
            {
                return buffer.ToArray();
            }
            Encoding encoding = Encoding.GetEncoding(1252);
            writer.Write(encoding.GetBytes(text));
            return buffer.ToArray();
        }

        public static byte[] WriteUTF16End(string text)
        {
            MemoryStream buffer = new MemoryStream(text.Length + 2);
            BinaryWriter writer = new BinaryWriter(buffer);
            if (String.IsNullOrEmpty(text))
            {
                return buffer.ToArray();
            }
            UnicodeEncoding encoding;
            writer.Write((byte)0xff);
            writer.Write((byte)0xfe);
            encoding = new UnicodeEncoding(false, false);
            writer.Write(encoding.GetBytes(text));
            return buffer.ToArray();
        }

        public static byte[] WriteUTF16BEEnd(string text)
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            if (String.IsNullOrEmpty(text))
            {
                return buffer.ToArray();
            }
            UnicodeEncoding encoding = new UnicodeEncoding(true, false);
            writer.Write(encoding.GetBytes(text));
            return buffer.ToArray();
        }

        public static byte[] WriteUTF8End(string text)
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            if (String.IsNullOrEmpty(text))
            {
                return buffer.ToArray();
            }
            writer.Write(Encoding.UTF8.GetBytes(text));
            return buffer.ToArray();
        }

        #endregion
    }
}