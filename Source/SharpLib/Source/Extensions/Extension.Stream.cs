using System.IO;

namespace SharpLib
{
    public static class ExtensionStream
    {
        #region Методы

        public static string ToStringEx(this Stream stream)
        {
            if (stream == null)
            {
                return null;
            }

            var reader = new StreamReader(stream);
            string text = reader.ReadToEnd();

            return text;
        }

        public static byte[] ToByfferEx(this Stream stream)
        {
            byte[] buffer = new byte[(int)stream.Length];

            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            return buffer;
        }

        public static MemoryStream ToMemoryStreamEx(this Stream stream)
        {
            if (stream == null)
            {
                return null;
            }

            var buffer = stream.ToByfferEx();
            var memStream = buffer.ToMemoryStreamEx();

            return memStream;
        }

        #endregion
    }
}