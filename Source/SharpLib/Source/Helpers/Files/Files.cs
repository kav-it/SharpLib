using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// Удаление последнего разделителя (если он есть)
        /// </summary>
        /// <example>
        /// Input:  'C:\Folder 1\'
        /// Result: 'C:\Folder 1'
        /// </example>
        public static string RemoveLastSeparator(string path)
        {
            return path.TrimEnd('/', '\\');
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
                result = RemoveLastSeparator(result);
            }

            result = Uri.UnescapeDataString(result);

            return result;
        }

        /// <summary>
        /// Определение абсолютного пути на основании основного
        /// <example>
        /// basePath     = "C:\Folder 1\Folder 2\"
        /// relativaPath = "..\file.txt"
        /// result       = "C:\Folder 1\file.txt
        /// </example>
        /// </summary>
        public static string GetPathAbsolute(string basePath, string relativePath)
        {
            if (IsFile(basePath))
            {
                basePath = GetDirectory(basePath);
            }

            basePath = AddPathSeparator(basePath);
            if (relativePath.IsNotValid())
            {
                return basePath;
            }

            bool removeLastDelimetr = false;
            if (Directory.Exists(relativePath))
            {
                relativePath = AddPathSeparator(relativePath);
            }
            else
            {
                removeLastDelimetr = true;
            }

            var baseUri = new Uri(basePath);
            var absUri = new Uri(baseUri, relativePath);
            var result = absUri.LocalPath;

            if (removeLastDelimetr)
            {
                result = RemoveLastSeparator(result);
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
            if (Directory.Exists(location))
            {
                return location;
            }

            if (File.Exists(location))
            {
                return Path.GetDirectoryName(location);
            }

            // Элемент не существует - Считается что файл
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

                if (parent != null)
                {
                    return parent.FullName;
                }

                return null;
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
            return CheckTyp(path) == FileTyp.Folder;
        }

        /// <summary>
        /// Проверка является ли путь файлом
        /// </summary>
        public static bool IsFile(string path)
        {
            return CheckTyp(path) == FileTyp.File;
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
        public static string CopyFile(string srcPath, string destPath, string newName = null)
        {
            if (string.IsNullOrEmpty(destPath))
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(srcPath))
            {
                return string.Empty;
            }

            destPath = destPath.TrimEnd('\\');

            try
            {
                string filename = newName ?? GetFileNameAndExt(srcPath);
                string newPath = PathEx.Combine(destPath, filename);

                if (File.Exists(newPath))
                {
                    DeleteFile(newPath);
                }

                File.Copy(srcPath, newPath);

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
        public static string CopyDirectory(string srcDir, string destDir, string newName = null)
        {
            destDir = newName == null
                ? Path.Combine(destDir, GetFileName(srcDir))
                : Path.Combine(destDir, newName);

            if (Directory.Exists(destDir) == false)
            {
                Directory.CreateDirectory(destDir);
            }

            // Создание директорий назначений
            var dirs = GetDirectories(srcDir).ToList();
            foreach (string dirPath in dirs)
            {
                var newDir = dirPath.Replace(srcDir, destDir);
                Directory.CreateDirectory(newDir);
            }

            // Копирование всех файлов
            var files = GetFiles(srcDir).ToList();
            foreach (string srcPath in files)
            {
                var destFile = srcPath.Replace(srcDir, destDir);

                if (File.Exists(destFile))
                {
                    File.Delete(destFile);
                }

                File.Copy(srcPath, destFile);
            }

            return destDir;
        }

        /// <summary>
        /// Чтение списка файлов в директории
        /// </summary>
        public static IEnumerable<string> GetFiles(string path, bool recursive = true, bool includeHidden = true, string mask = "*.*")
        {
            var result = Directory.GetFiles(path, mask, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            if (includeHidden == false && result.Any())
            {
                var filtered = result.Where(x => new DirectoryInfo(x).Attributes.HasFlag(FileAttributes.Hidden) == false);

                return filtered;
            }

            return result;
        }

        /// <summary>
        /// Чтение списка директорий в директории
        /// </summary>
        public static IEnumerable<string> GetDirectories(string path, bool recursive = true, bool includeHidden = true, string mask = "*")
        {
            var result = Directory.GetDirectories(path, mask, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            if (includeHidden == false && result.Any())
            {
                var filtered = result.Where(x => new DirectoryInfo(x).Attributes.HasFlag(FileAttributes.Hidden) == false);

                return filtered;
            }

            return result;
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
        public static string Rename(string path, string newName, bool onlyName = true)
        {
            if (path.IsValid() == false)
            {
                return string.Empty;
            }
            if (newName.IsValid() == false)
            {
                return string.Empty;
            }

            var typ = CheckTyp(path);

            if (typ == FileTyp.File)
            {
                // Путь является файлом
                // Составление полного пути получателя
                var ext = GetExtension(path);
                if (ext.IsValid())
                {
                    // Если расширение существует добавление разделителя
                    ext = "." + ext;
                }
                var destPath = onlyName
                    ? PathEx.Combine(GetDirectory(path), newName + ext)
                    : PathEx.Combine(GetDirectory(path), newName);

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

            if (typ == FileTyp.Folder)
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
        /// Удаление директории/файла
        /// </summary>
        public static void Delete(string path)
        {
            if (path.IsValid() == false)
            {
                return;
            }

            var typ = CheckTyp(path);

            if (typ == FileTyp.File)
            {
                DeleteFile(path);
            }
            else if (typ == FileTyp.Folder)
            {
                DeleteDirectory(path);
            }
        }

        /// <summary>
        /// Удаление каталога
        /// </summary>
        public static void DeleteDirectory(string dirPath)
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
        }

        /// <summary>
        /// Очистка каталога
        /// </summary>
        public static void EraseDirectory(string dirPath)
        {
            DeleteDirectory(dirPath);

            // Ожидание завершения операции удаления каталога
            Thread.Sleep(50);

            CreateDirectory(dirPath);
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

        /// <summary>
        /// Чтение размера
        /// </summary>
        public static long GetFileSize(string location)
        {
            var info = new FileInfo(location);

            return info.Length;
        }

        /// <summary>
        /// Определение типа записи файловой системы
        /// </summary>
        public static FileTyp CheckTyp(string location)
        {
            if (File.Exists(location))
            {
                var fileInfo = new FileInfo(location);

                if (fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {
                    return FileTyp.LinkFile;
                }

                return FileTyp.File;
            }

            if (Directory.Exists(location))
            {
                return FileTyp.Folder;
            }

            return FileTyp.Unknown;
        }

        public static bool IsHidden(string path)
        {
            var typ = CheckTyp(path);

            if (typ == FileTyp.File)
            {
                var attr = File.GetAttributes(path);

                return attr.HasFlag(FileAttributes.Hidden);
            }

            if (typ == FileTyp.Folder)
            {
                var info = new DirectoryInfo(path);

                return info.Attributes.HasFlag(FileAttributes.Hidden);
            }

            return false;
        }

        #endregion
    }
}