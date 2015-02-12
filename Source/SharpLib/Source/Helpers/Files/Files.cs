using System;
using System.IO;
using System.Threading;

namespace SharpLib
{
    public static class Files
    {
        #region Константы

        public const string PATH_SEPARATOR = @"\";

        #endregion

        #region Методы

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
        public static string GetPathRelative(string path1, string path2, bool removeLastDelimetr = false)
        {
            path1 = AddPathSeparator(path1);
            path2 = AddPathSeparator(path2);

            // Определение минимальной вложенности
            var uri1 = new Uri(path1);
            var uri2 = new Uri(path2);
            var uriRel = uri1.MakeRelativeUri(uri2);
            string result = uriRel.ToString();

            if (removeLastDelimetr)
            {
                result = result.TrimEnd('/', '\\');
            }

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
        /// Создание каталога в указанном родительском
        /// </summary>
        public static string CreateDirectory(string parent, string name)
        {
            var path = Path.Combine(parent, name);

            CreateDirectory(path);

            return path;
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

        /// <summary>
        /// Копирование файла
        /// </summary>
        public static string CopyFile(string destPath, string filePath)
        {
            if (string.IsNullOrEmpty(destPath))
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

            destPath = destPath.TrimEnd('\\');

            try
            {
                if (Directory.Exists(destPath) == false)
                {
                    CreateDirectory(destPath);
                }

                string filename = GetFileNameAndExt(filePath);
                string newPath = PathEx.Combine(destPath, filename);

                if (File.Exists(newPath))
                {
                    DeleteFile(newPath);
                }

                File.Copy(filePath, newPath);

                return newPath;
            }
            catch
            {
            }

            return string.Empty;
        }

        /// <summary>
        /// Копирование директории (с рекурсивным содержимым)
        /// </summary>
        public static void CopyDirectory(string destDir, string srcDir)
        {
            // Создание директорий назначений
            foreach (string dirPath in Directory.GetDirectories(srcDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(srcDir, destDir));
            }

            // Копирование всех файлов
            foreach (string newPath in Directory.GetFiles(srcDir, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(srcDir, destDir), true);
            }
        }

        /// <summary>
        /// Удаление файла
        /// </summary>
        public static bool DeleteFile(string filename, bool throwEx = false)
        {
            try
            {
                File.SetAttributes(filename, FileAttributes.Normal);
                File.Delete(filename);

                return true;
            }
            catch
            {
                if (throwEx)
                {
                    throw;
                }

                return false;
            }
        }

        /// <summary>
        /// Смена имени файла/директории
        /// </summary>
        public static string Rename(string path, string newName)
        {
            if (path.IsValid() == false)
            {
                return string.Empty;
            }
            if (newName.IsValid() == false)
            {
                return string.Empty;
            }
            if (File.Exists(path) == false)
            {
                return string.Empty;
            }

            if (IsFile(path))
            {
                // Путь является файлом
                // Составление полного пути получателя
                string destPath = PathEx.Combine(GetDirectory(path), newName + "." + GetExtension(path));

                // Файл уже так называется
                if (destPath == path)
                {
                    return destPath;
                }

                // Если файл существует: Удаление файла
                if (File.Exists(destPath))
                {
                    DeleteFile(destPath);
                }

                var info = new FileInfo(path);
                info.MoveTo(destPath);

                return destPath;
            }

            if (IsDirectory(path))
            {
                // Путь является директорией
                // Составление полного пути получателя
                var destPath = PathEx.Combine(GetDirectoryParent(path), newName);

                // Если файл существует: Удаление файла
                if (File.Exists(destPath))
                {
                    DeleteDirectory(destPath);
                }

                var info = new DirectoryInfo(path);
                info.MoveTo(destPath);

                return destPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Удаление каталога
        /// </summary>
        public static bool DeleteDirectory(string dirPath)
        {
            try
            {
                var listSubFolders = Directory.GetDirectories(dirPath);

                foreach (var subFolder in listSubFolders)
                {
                    DeleteDirectory(subFolder);
                }

                var files = Directory.GetFiles(dirPath);
                foreach (var f in files)
                {
                    var attr = File.GetAttributes(f);

                    if (attr.IsFlagSet(FileAttributes.ReadOnly))
                    {
                        File.SetAttributes(f, attr ^ FileAttributes.ReadOnly);
                    }

                    File.Delete(f);
                }

                Directory.Delete(dirPath);

                return true;
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Очистка каталога
        /// </summary>
        public static bool EraseDirectory(string dirPath)
        {
            Boolean result = DeleteDirectory(dirPath);
            if (result == false)
            {
                return false;
            }

            // Ожидание завершения операции удаления каталога
            Thread.Sleep(50);

            result = CreateDirectory(dirPath);

            return result;
        }

        /// <summary>
        /// Сброс атрибутов
        /// </summary>
        public static void ClearAttribute(string location, FileAttributes value)
        {
            var attributes = File.GetAttributes(location);

            if ((int)(attributes & value) != 0)
            {
                // Есть атрибуты, которые нужно сбросить
                attributes &= ~value;

                File.SetAttributes(location, attributes);
            }
        }

        /// <summary>
        /// Создание временной директории в %TEMP%
        /// </summary>
        public static String GetTempDirectory(bool isCreate = true)
        {
            var path = Path.GetTempPath();

            if (isCreate)
            {
                path = Path.Combine(path, Path.GetRandomFileName());
                CreateDirectory(path);
            }

            return path;
        }

        /// <summary>
        /// Генерация имени временного файла в %TEMP%
        /// </summary>
        public static string GetTempFilename(string startPart = null)
        {
            var path = Path.GetTempFileName();

            if (startPart.IsValid())
            {
                path = Path.Combine(
                    GetDirectory(path),
                    Path.GetFileName(startPart) + Path.GetFileName(path));
            }

            return path;
        }

        #endregion
    }
}