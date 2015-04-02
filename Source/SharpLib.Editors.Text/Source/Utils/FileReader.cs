using System;
using System.IO;
using System.Text;

namespace SharpLib.Notepad.Utils
{
    public static class FileReader
    {
        #region Поля

        private static readonly Encoding UTF8NoBOM = new UTF8Encoding(false);

        #endregion

        #region Методы

        public static bool IsUnicode(Encoding encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            switch (encoding.CodePage)
            {
                case 65000:
                case 65001:
                case 1200:
                case 1201:
                case 12000:
                case 12001:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsASCIICompatible(Encoding encoding)
        {
            var bytes = encoding.GetBytes("Az");
            return bytes.Length == 2 && bytes[0] == 'A' && bytes[1] == 'z';
        }

        private static Encoding RemoveBOM(Encoding encoding)
        {
            switch (encoding.CodePage)
            {
                case 65001:
                    return UTF8NoBOM;
                default:
                    return encoding;
            }
        }

        public static string ReadFileContent(Stream stream, Encoding defaultEncoding)
        {
            using (var reader = OpenStream(stream, defaultEncoding))
            {
                return reader.ReadToEnd();
            }
        }

        public static string ReadFileContent(string fileName, Encoding defaultEncoding)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return ReadFileContent(fs, defaultEncoding);
            }
        }

        public static StreamReader OpenFile(string fileName, Encoding defaultEncoding)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                return OpenStream(fs, defaultEncoding);
            }
            catch
            {
                fs.Dispose();
                throw;
            }
        }

        public static StreamReader OpenStream(Stream stream, Encoding defaultEncoding)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (stream.Position != 0)
            {
                throw new ArgumentException("stream is not positioned at beginning.", "stream");
            }
            if (defaultEncoding == null)
            {
                throw new ArgumentNullException("defaultEncoding");
            }

            if (stream.Length >= 2)
            {
                int firstByte = stream.ReadByte();
                int secondByte = stream.ReadByte();
                switch ((firstByte << 8) | secondByte)
                {
                    case 0x0000:
                    case 0xfffe:
                    case 0xfeff:
                    case 0xefbb:

                        stream.Position = 0;
                        return new StreamReader(stream);
                    default:
                        return AutoDetect(stream, (byte)firstByte, (byte)secondByte, defaultEncoding);
                }
            }
            if (defaultEncoding != null)
            {
                return new StreamReader(stream, defaultEncoding);
            }
            return new StreamReader(stream);
        }

        private static StreamReader AutoDetect(Stream fs, byte firstByte, byte secondByte, Encoding defaultEncoding)
        {
            int max = (int)Math.Min(fs.Length, 500000);
            const int ASCII = 0;
            const int Error = 1;
            const int UTF8 = 2;
            const int UTF8Sequence = 3;
            int state = ASCII;
            int sequenceLength = 0;
            byte b;
            for (int i = 0; i < max; i++)
            {
                if (i == 0)
                {
                    b = firstByte;
                }
                else if (i == 1)
                {
                    b = secondByte;
                }
                else
                {
                    b = (byte)fs.ReadByte();
                }
                if (b < 0x80)
                {
                    if (state == UTF8Sequence)
                    {
                        state = Error;
                        break;
                    }
                }
                else if (b < 0xc0)
                {
                    if (state == UTF8Sequence)
                    {
                        --sequenceLength;
                        if (sequenceLength < 0)
                        {
                            state = Error;
                            break;
                        }
                        if (sequenceLength == 0)
                        {
                            state = UTF8;
                        }
                    }
                    else
                    {
                        state = Error;
                        break;
                    }
                }
                else if (b >= 0xc2 && b < 0xf5)
                {
                    if (state == UTF8 || state == ASCII)
                    {
                        state = UTF8Sequence;
                        if (b < 0xe0)
                        {
                            sequenceLength = 1;
                        }
                        else if (b < 0xf0)
                        {
                            sequenceLength = 2;
                        }
                        else
                        {
                            sequenceLength = 3;
                        }
                    }
                    else
                    {
                        state = Error;
                        break;
                    }
                }
                else
                {
                    state = Error;
                    break;
                }
            }
            fs.Position = 0;
            switch (state)
            {
                case ASCII:
                    return new StreamReader(fs, IsASCIICompatible(defaultEncoding) ? RemoveBOM(defaultEncoding) : Encoding.ASCII);
                case Error:

                    if (IsUnicode(defaultEncoding))
                    {
                        defaultEncoding = Encoding.Default;
                    }
                    return new StreamReader(fs, RemoveBOM(defaultEncoding));
                default:
                    return new StreamReader(fs, UTF8NoBOM);
            }
        }

        #endregion
    }
}