using System;
using System.IO;

namespace SharpLib
{
    public static class Files
    {
        public static FileError LastError { get; set; }

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
        /// Проверка является ли путь директорией
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
            LastError = FileError.None;

            try
            {
                string text = File.ReadAllText(filename);

                return text;
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            return null;
        }

        /// <summary>
        /// Чтение содержимого файла как текст
        /// </summary>
        public static bool WriteText(string filename, string text)
        {
            LastError = FileError.None;

            try
            {
                File.WriteAllText(filename, text);

                return true;
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            return false;
        }

        private static void SetLastError(Exception ex)
        {
            if (ex is ArgumentException)
            {
                LastError = FileError.WrongFormat;
            }
            else if (ex is PathTooLongException)
            {
                LastError = FileError.WrongFormat;
            }
            else if (ex is DirectoryNotFoundException)
            {
                LastError = FileError.DirNotFound;
            }
            else if (ex is FileNotFoundException)
            {
                LastError = FileError.FileNotFound;
            }
            else if (ex is IOException)
            {
                LastError = FileError.IoException;
            }
            else
            {
                LastError = FileError.Unknow;
            }
        }



    }
}