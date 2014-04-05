// ****************************************************************************
//
// Имя файла    : 'Files.cs'
// Заголовок    : Вспомогательный модуль работы с файлами 
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/06/2012
//
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;

namespace SharpLib
{

    #region Перечисление FileError

    public enum FileError
    {
        Unknow = -1,

        None = 0,

        WrongFormat = 1,

        FileNotFound = 2,

        DirNotFound = 3,

        IoException = 4,
    }

    #endregion Перечисление FileError

    #region Перечисление Stdout

    public enum Stdout
    {
        Unknow = 0,

        Output = 1,

        Error = 2,

        Warning = 3
    }

    #endregion Перечисление Stdout

    #region Делегат

    public delegate void ProcessBatExecute(Stdout std, String message);

    #endregion Делегат

    #region Класс Files

    public static class Files
    {
        #region Константы

        public const String PATH_SEPARATOR = @"\";

        #endregion

        #region Свойства

        public static FileError LastError { get; set; }

        #endregion

        #region Методы

        private static void SetLastError(Exception ex)
        {
            if (ex is ArgumentException) LastError = FileError.WrongFormat;
            else if (ex is PathTooLongException) LastError = FileError.WrongFormat;
            else if (ex is DirectoryNotFoundException) LastError = FileError.DirNotFound;
            else if (ex is FileNotFoundException) LastError = FileError.FileNotFound;
            else if (ex is IOException) LastError = FileError.IoException;
            else
                LastError = FileError.Unknow;
        }

        /// <summary>
        /// Создание каталога
        /// </summary>
        /// <param name="dirPath"></param>
        public static Boolean CreateDir(String dirPath)
        {
            LastError = FileError.None;

            try
            {
                Directory.CreateDirectory(dirPath);

                return true;
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            return false;
        }

        /// <summary>
        /// Создание каталога
        /// </summary>
        /// <param name="dirPath"></param>
        public static Boolean DeleteDir(String dirPath)
        {
            LastError = FileError.None;

            try
            {
                var listSubFolders = Directory.GetDirectories(dirPath);

                foreach (var subFolder in listSubFolders)
                {
                    DeleteDir(subFolder);
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
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            return false;
        }

        /// <summary>
        /// Очистка каталога
        /// </summary>
        public static Boolean EraseDir(String dirPath)
        {
            Boolean result = DeleteDir(dirPath);
            if (result == false) return false;

            // Ожидание завершения операции удаления каталога
            Thread.Sleep(50);

            result = CreateDir(dirPath);

            return result;
        }

        /// <summary>
        /// Чтение содержимого файла
        /// </summary>
        /// <param name="filename">Полный путь к файлу</param>
        /// <returns>Бинарное содержимого файла</returns>
        public static Byte[] Read(String filename)
        {
            LastError = FileError.None;

            try
            {
                Byte[] buffer = File.ReadAllBytes(filename);

                return buffer;
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
        /// <param name="filename">Полный путь к файлу</param>
        /// <returns>Текстовое содержимого файла</returns>
        public static String ReadText(String filename)
        {
            LastError = FileError.None;

            try
            {
                String text = File.ReadAllText(filename);

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
        public static Boolean Write(String filename, Byte[] data)
        {
            LastError = FileError.None;

            try
            {
                File.WriteAllBytes(filename, data);

                return true;
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            return false;
        }

        /// <summary>
        /// Модификация данных в файле
        /// </summary>
        public static Boolean Modify(String filename, Byte[] data, int offset)
        {
            using (FileStream stream = File.Open(filename, FileMode.Open))
            {
                LastError = FileError.None;

                try
                {
                    stream.Position = offset;
                    stream.Write(data, 0, data.Length);

                    return true;
                }
                catch (Exception ex)
                {
                    SetLastError(ex);
                }
            }

            return false;
        }

        /// <summary>
        /// Чтение содержимого файла как текст
        /// </summary>
        public static Boolean WriteText(String filename, String text)
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

        /// <summary>
        /// Определение размера файла
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Int64 GetSize(String filename)
        {
            FileInfo info = new FileInfo(filename);

            if (info.Exists)
                return info.Length;

            return -1;
        }

        public static DateTime GetTime(String filename)
        {
            DateTime creationTime = File.GetLastWriteTime(filename);

            return creationTime;
        }

        /// <summary>
        /// Добавление разделителя пути (если необходимо)
        /// <example>
        /// Input:  'C:\Folder 1'
        /// Result: 'C:\Folder 1\'
        /// </example>
        /// </summary>
        public static String AddPathSeparator(String path)
        {
            if (path.IsValid())
            {
                if (path.EndsWith(PATH_SEPARATOR) == false)
                    path += PATH_SEPARATOR;
            }

            return path;
        }

        /// <summary>
        /// Сравнение 2-х путей с учетом разделителя
        /// <example>
        /// PathA : 'C:\Folder 1'
        /// PathB : 'C:/Folder 2'
        /// Result: true
        /// </example>
        /// </summary>
        public static Boolean ComparePaths(String pathA, String pathB)
        {
            String fullPathA = Path.GetFullPath(pathA);
            String fullPathB = Path.GetFullPath(pathB);

            Boolean result = fullPathA.Equals(fullPathB);

            return result;
        }

        /// <summary>
        /// Определение наличия файла
        /// </summary>
        /// <returns />
        public static Boolean IsExists(String path)
        {
            Boolean result = (IsDirectory(path) || IsFile(path));

            return result;
        }

        /// <summary>
        /// Проверка является ли путь директорией
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Boolean IsDirectory(String path)
        {
            if (String.IsNullOrEmpty(path)) return false;

            Boolean result = Directory.Exists(path);

            return result;
        }

        /// <summary>
        /// Проверка является ли путь файлом
        /// </summary>
        public static Boolean IsFile(String path)
        {
            if (String.IsNullOrEmpty(path)) return false;

            Boolean result = File.Exists(path);

            return result;
        }

        /// <summary>
        /// Проверка является ли путь логическим диском
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Boolean IsDrive(String path)
        {
            DirectoryInfo info = new DirectoryInfo(path);

            return (info.Parent == null);
        }

        /// <summary>
        /// Проверка пустой директории
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static Boolean IsEmptyDir(String dirPath)
        {
            DirectoryInfo info = new DirectoryInfo(dirPath);

            FileSystemInfo[] fsInfo = info.GetFileSystemInfos();

            return (fsInfo.Length == 0);
        }

        /// <summary>
        /// Проверка наличия расширения
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Boolean HasExt(String filename)
        {
            String ext = GetFileExt(filename);

            return ext.IsValid();
        }

        /// <summary>
        /// Удаление расширение файла из пути
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static String RemoveExt(String filename)
        {
            String ext = Path.GetExtension(filename);

            filename = filename.TrimEndEx(ext);

            return filename;
        }

        /// <summary>
        /// Извлечение имени файла БЕЗ расширения из полного имени и пути
        /// </summary>
        public static String GetFileName(String filepath)
        {
            String text;

            if (IsDirectory(filepath))
            {
                DirectoryInfo dinfo = new DirectoryInfo(filepath);

                text = dinfo.Name;
            }
            else
                text = Path.GetFileNameWithoutExtension(filepath);

            return text;
        }

        /// <summary>
        /// Извлечение имени файла из полного имени и пути
        /// </summary>
        public static String GetFileNameAndExt(String filepath)
        {
            String text = Path.GetFileName(filepath);

            return text;
        }

        /// <summary>
        /// Чтение расширения файла
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static String GetFileExt(String filepath)
        {
            String ext = Path.GetExtension(filepath);

            if (ext != null) ext = ext.TrimStart('.');

            return ext;
        }

        /// <summary>
        /// Извлечение полного пути к файлу/директории
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static String GetPath(String path)
        {
            if (path.IsValid())
            {
                path = Path.GetDirectoryName(path);
                if (path.IsValid()) path = AddPathSeparator(path);
            }

            return path;
        }

        /// <summary>
        /// Извлечение пути папки-родителя
        /// </summary>
        public static String GetPathParent(String path)
        {
            path = path.TrimEnd('\\');

            DirectoryInfo info = Directory.GetParent(path);

            path = info.ToString();

            return path;
        }

        /// <summary>
        /// Извлечение имени логического диска пути
        /// </summary>
        public static String GetPathDrive(String path)
        {
            if (path.IsNotValid()) return "";

            path = Path.GetPathRoot(path);

            return path;
        }

        /// <summary>
        /// Определение относительного пути (path_1 относительно path_2)
        /// </summary>
        public static String GetPathRelative(String path1, String path2)
        {
            path1 = AddPathSeparator(path1);
            path2 = AddPathSeparator(path2);

            // Определение минимальной вложенности
            Uri uri_1 = new Uri(path1);
            Uri uri_2 = new Uri(path2);
            Uri uri_rel = uri_1.MakeRelativeUri(uri_2);
            String result = uri_rel.ToString();

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
        public static String GetPathAbsolute(String basePath, String relativePath)
        {
            basePath = AddPathSeparator(basePath);
            if (relativePath.IsNotValid()) return basePath;
            relativePath = AddPathSeparator(relativePath);

            Uri baseUri = new Uri(basePath);
            Uri absUri = new Uri(baseUri, relativePath);
            String result = absUri.LocalPath;

            return result;
        }

        /// <summary>
        /// Копирование файла
        /// </summary>
        public static String CopyFile(String destPath, String filePath)
        {
            if (String.IsNullOrEmpty(destPath)) return "";
            if (String.IsNullOrEmpty(filePath)) return "";

            destPath = destPath.TrimEnd('\\');

            LastError = FileError.None;

            try
            {
                if (Directory.Exists(destPath) == false)
                    Files.CreateDir(destPath);

                String filename = Files.GetFileNameAndExt(filePath);
                String newPath = destPath + "\\" + filename;

                if (File.Exists(newPath))
                    Files.DeleteFile(newPath);

                File.Copy(filePath, newPath);

                return newPath;
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            return "";
        }

        /// <summary>
        /// Удаление файла
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Boolean DeleteFile(String filename)
        {
            LastError = FileError.None;

            try
            {
                File.Delete(filename);

                return true;
            }
            catch (Exception ex)
            {
                SetLastError(ex);
            }

            return true;
        }

        /// <summary>
        /// Перенос файла
        /// </summary>
        public static Boolean MoveFile(String dest, String source)
        {
            CopyFile(dest, source);
            DeleteFile(source);

            return true;
        }

        /// <summary>
        /// Смена имени файла/директории
        /// </summary>
        public static Boolean Rename(String path, String newName)
        {
            if (path.IsValid() == false) return false;
            if (newName.IsValid() == false) return false;
            if (Files.IsExists(path) == false) return false;

            if (Files.IsFile(path))
            {
                // Путь является файлом
                // Составление полного пути получателя
                String destPath = Files.GetPath(path) + newName;

                // Файл уже так называется
                if (destPath == path) return true;

                // Если файл существует: Удаление файла
                if (Files.IsExists(destPath))
                    DeleteFile(destPath);

                FileInfo info = new FileInfo(path);
                info.MoveTo(destPath);

                return true;
            }

            if (Files.IsDirectory(path))
            {
                // Путь является директорией
                // Составление полного пути получателя
                String destPath = Files.GetPathParent(path) + "\\" + newName;

                // Если файл существует: Удаление файла
                if (Files.IsExists(destPath))
                    DeleteDir(destPath);

                DirectoryInfo info = new DirectoryInfo(path);
                info.MoveTo(destPath);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Сохранение потока в файл
        /// </summary>
        public static Boolean Save(this Stream stream, String filename)
        {
            if (stream == null || filename.IsValid() == false)
                return false;

            if (stream.Length != 0)
            {
                // Создание директории файла
                String destPath = Files.GetPath(filename);
                if (Directory.Exists(destPath) == false)
                    Files.CreateDir(destPath);

                using (FileStream fileStream = System.IO.File.Create(filename, (int)stream.Length))
                {
                    // Создание и заполнение массива данными из потокаwith the stream data
                    Byte[] bytesInStream = new Byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(bytesInStream, 0, bytesInStream.Length);

                    // Запись в поток
                    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                }

                // После using вызовется fileStream.Close() и файл будет записан
            }

            return true;
        }

        public static void RunExe(String exeName, String param)
        {
            Process process = new Process();
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.FileName = exeName;
            process.StartInfo.Arguments = param;
            process.Start();
        }

        /// <summary>
        /// Запуск файла на выполнение
        /// </summary>
        public static Boolean RunBat(String exeName, String param, ProcessBatExecute callback)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo(exeName, param);

            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardInput = true;

            Process process = Process.Start(processInfo);
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.OutputDataReceived += (sender, e) =>
            {
                if (callback != null && e.Data.IsValid())
                    callback(Stdout.Output, e.Data);
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (callback != null && e.Data.IsValid())
                    callback(Stdout.Error, e.Data);
            };

            process.WaitForExit();

            int exitCode = process.ExitCode;

            process.Close();

            return (exitCode == 0);
        }

        /// <summary>
        /// Открытие папки в проводнике Windows
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void OpenFolder(String path)
        {
            RunExe("explorer.exe", path);
        }

        /// <summary>
        /// Загрузка изображения
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static ImageSource LoadImage(String filename)
        {
            String path = filename;

            ImageSourceConverter conv = new ImageSourceConverter();
            ImageSource source = conv.ConvertFromString(path) as ImageSource;

            return source;
        }

        /// <summary>
        /// Сохранение изображения
        /// </summary>
        public static void SaveImage(ImageSource imageSource, String filename, ImageTyp typ)
        {
            String ext = Files.GetFileExt(filename).ToLower();
            String addExt = "";

            if (typ == ImageTyp.Unknow && ext.IsValid())
            {
                switch (ext)
                {
                    case "bmp":
                        typ = ImageTyp.Bmp;
                        break;
                    case "jpg":
                    case "jpeg":
                        typ = ImageTyp.Jpg;
                        break;
                    case "gif":
                        typ = ImageTyp.Gif;
                        break;
                    case "tif":
                    case "tiff":
                        typ = ImageTyp.Tif;
                        break;
                    case "png":
                        typ = ImageTyp.Png;
                        break;
                }
            }

            switch (typ)
            {
                case ImageTyp.Bmp:
                    addExt = "bmp";
                    break;
                case ImageTyp.Tif:
                    addExt = "tif";
                    break;
                case ImageTyp.Jpg:
                    addExt = "jpg";
                    break;
                case ImageTyp.Gif:
                    addExt = "gif";
                    break;

                default:
                    addExt = "png";
                    break;
            }

            // Формирование полного имени файла (с расширением)
            if (addExt != ext)
                filename = String.Format("{0}.{1}", filename, addExt);

            MemoryStream stream = Gui.ImageSourceToStream(imageSource, typ);

            // Сохранение потока в файла (с созданием пути)
            stream.Save(filename);
        }

        /// <summary>
        /// Проверка запущена ли программа
        /// </summary>
        internal static Boolean IsRun(String processName)
        {
            processName = processName.ToLower();
            processName = processName.TrimEndEx(".exe");
            processName = processName.TrimEndEx(".dll");

            Process[] processes = Process.GetProcesses();

            int total = 0;
            foreach (Process process in processes)
            {
                String currName = process.ProcessName.ToLower();

                if (currName == processName)
                    total++;
            }

            return (total != 0);
        }

        /// <summary>
        /// Поиск файла
        /// </summary>
        public static String Search(String fileName, String baseDir)
        {
            String[] files = Directory.GetFiles(baseDir, fileName, SearchOption.AllDirectories);

            files.SortEx();

            if (files.Length > 0)
                return files[0];

            return "";
        }

        /// <summary>
        /// Поиск списка файлов в указанной директории по шаблону поиска
        /// </summary>
        public static List<String> GetFiles(String dir, String searchPattern, SearchOption option = SearchOption.TopDirectoryOnly)
        {
            String[] files = Directory.GetFiles(dir, searchPattern, option);

            files.SortEx();

            List<String> result = files.ToList();

            return result;
        }

        /// <summary>
        /// Создание временной директории в %TEMP%
        /// </summary>
        public static String GetTempDirectory(Boolean isCreate = true)
        {
            String path = Path.GetTempPath();

            if (isCreate)
                CreateDir(path);

            return path;
        }

        /// <summary>
        /// Генерация имени временного файла в %TEMP%
        /// </summary>
        public static String GetTempFilename()
        {
            String path = Path.GetTempFileName();

            return path;
        }

        /// <summary>
        /// Генерация случайного имени файла
        /// </summary>
        /// <returns></returns>
        public static String GetRandFilename()
        {
            String path = Path.GetRandomFileName();

            return path;
        }

        #endregion
    }

    #endregion Класс Files

    #region Класс FileLocation

    public class FileLocation
    {
        #region Свойства

        /// <summary>
        /// Путь к файлу
        /// <para>C:\Programm Files\Application.exe</para>
        /// </summary>
        public String ExePath { get; private set; }

        /// <summary>
        /// Имя файла с расширением
        /// <para>Application.exe</para>
        /// </summary>
        public String FileName
        {
            get { return Path.GetFileName(ExePath); }
        }

        /// <summary>
        /// Имя файла без расширения
        /// <para>Application</para>
        /// </summary>
        public String ExeName
        {
            get { return Path.GetFileNameWithoutExtension(FileName); }
        }

        /// <summary>
        /// Путь к директории расположения файла
        /// <para>C:\Programm Files\</para>
        /// </summary>
        public String DirName
        {
            get { return Path.GetDirectoryName(ExePath); }
        }

        /// <summary>
        /// Расширение файла
        /// <para>exe</para>
        /// </summary>
        public String Ext
        {
            get { return Files.GetFileExt(ExePath); }
        }

        /// <summary>
        /// Путь к файлу, НО БЕЗ расширения
        /// <para>C:\Programm Files\Application</para>
        /// </summary>
        public String ExePathWithoutExt
        {
            get { return Path.Combine(DirName, ExeName); }
        }

        #endregion

        #region Конструктор

        public FileLocation(String exePath)
        {
            ExePath = exePath;
        }

        #endregion
    }

    #endregion Класс FileLocation
}