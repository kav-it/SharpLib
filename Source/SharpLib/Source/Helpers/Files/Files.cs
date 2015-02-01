using System;
using System.IO;

namespace SharpLib
{
    public static class Files
    {
        public const string PATH_SEPARATOR = @"\";

        /// <summary>
        /// Добавление разделителя пути (если необходимо)
        /// <example>
        /// Input:  'C:\Folder 1'
        /// Result: 'C:\Folder 1\'
        /// </example>
        /// </summary>
        public static string AddPathSeparator(string path)
        {
            if (path.IsValid())
            {
                if (path.EndsWith(PATH_SEPARATOR) == false)
                {
                    path += PATH_SEPARATOR;
                }
            }

            return path;
        }

        /// <summary>
        /// Формирование относительного пути
        /// </summary>
        public static string GetPathRelative(string path1, string path2)
        {
            path1 = AddPathSeparator(path1);
            path2 = AddPathSeparator(path2);

            // Определение минимальной вложенности
            var uri1 = new Uri(path1);
            var uri2 = new Uri(path2);
            var uriRel = uri1.MakeRelativeUri(uri2);
            string result = uriRel.ToString();

            return result;
        }

        /// <summary>
        /// Извлечение имени файла БЕЗ расширения из полного имени и пути
        /// </summary>
        public static string GetFileName(string filepath)
        {
            string text;

            if (IsDirectory(filepath))
            {
                var dinfo = new DirectoryInfo(filepath);

                text = dinfo.Name;
            }
            else
            {
                text = Path.GetFileNameWithoutExtension(filepath);
            }

            return text;
        }

        /// <summary>
        /// Чтение расширения файла (без '.'. Например: avi)
        /// </summary>
        public static string GetExtension(string filepath)
        {
            // ReSharper disable PossibleNullReferenceException
            return Path.GetExtension(filepath).TrimStart('.');
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Чтение директории файла (для директории возвращает ее саму)
        /// </summary>
        /// <remarks>
        /// C:/1.txt => C:/
        /// </remarks>
        public static string GetDirectory(string location)
        {
            if (IsDirectory(location))
            {
                return location;
            }

            return Path.GetDirectoryName(location);
        }

        /// <summary>
        /// Чтение родительской директории
        /// </summary>
        public static string GetDirectoryParent(string location)
        {
            try
            {
                location = GetDirectory(location);

                var parent = Directory.GetParent(location);

                return parent.FullName;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Создание каталога
        /// </summary>
        public static bool CreateDirectory(string location)
        {
            try
            {
                Directory.CreateDirectory(location);

                return true;
            }
            catch 
            {
            }

            return false;
        }

        /// <summary>
        /// Проверка является ли путь директорией
        /// </summary>
        public static bool IsDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            bool result = Directory.Exists(path);

            return result;
        }

        /// <summary>
        /// Проверка является ли путь файлом
        /// </summary>
        public static bool IsFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            bool result = File.Exists(path);

            return result;
        }

        /// <summary>
        /// Извлечение имени файла из полного имени и пути
        /// </summary>
        public static string GetFileNameAndExt(string filepath)
        {
            string text = Path.GetFileName(filepath);

            return text;
        }

        /// <summary>
        /// Чтение содержимого файла как текст
        /// </summary>
        /// <param name="filename">Полный путь к файлу</param>
        /// <returns>Текстовое содержимого файла</returns>
        public static string ReadText(string filename)
        {
            try
            {
                string text = File.ReadAllText(filename);

                return text;
            }
            catch
            {
            }
            
            return null;
        }

        /// <summary>
        /// Чтение содержимого файла как текст
        /// </summary>
        public static bool WriteText(string filename, string text)
        {
            try
            {
                File.WriteAllText(filename, text);

                return true;
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Сохранение потока в файл
        /// </summary>
        public static bool WriteStream(Stream stream, string filename)
        {
            return stream.WriteToFileEx(filename);
        }

    }
}