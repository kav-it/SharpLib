using System.IO;

namespace SharpLib
{
    /// <summary>
    /// Метод расширения класса "Stream"
    /// </summary>
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

        /// <summary>
        /// Сохранение потока в файл
        /// </summary>
        public static bool WriteToFileEx(this Stream stream, string filename)
        {
            if (stream == null || filename.IsValid() == false)
            {
                return false;
            }

            if (stream.Length != 0)
            {
                // Создание директории файла
                var destPath = Files.GetDirectory(filename);
                if (Directory.Exists(destPath) == false)
                {
                    Files.CreateDirectory(destPath);
                }

                using (var fileStream = File.Create(filename, (int)stream.Length))
                {
                    // Создание и заполнение массива данными из потокаwith the stream data
                    var bytesInStream = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(bytesInStream, 0, bytesInStream.Length);

                    // Запись в поток
                    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                }
            }

            return true;
        }

        #endregion
    }
}