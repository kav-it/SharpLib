using System.IO;

namespace SharpLib
{
    public class FileLocation
    {
        #region Свойства

        /// <summary>
        /// Путь к файлу
        /// <para>C:\Programm Files\Application.exe</para>
        /// </summary>
        public string ExePath { get; private set; }

        /// <summary>
        /// Имя файла с расширением
        /// <para>Application.exe</para>
        /// </summary>
        public string FileName
        {
            get { return Path.GetFileName(ExePath); }
        }

        /// <summary>
        /// Имя файла без расширения
        /// <para>Application</para>
        /// </summary>
        public string ExeName
        {
            get { return Path.GetFileNameWithoutExtension(FileName); }
        }

        /// <summary>
        /// Путь к директории расположения файла
        /// <para>C:\Programm Files\</para>
        /// </summary>
        public string DirName
        {
            get { return Path.GetDirectoryName(ExePath); }
        }

        /// <summary>
        /// Расширение файла
        /// <para>exe</para>
        /// </summary>
        public string Ext
        {
            get { return Path.GetExtension(ExePath); }
        }

        /// <summary>
        /// Путь к файлу, НО БЕЗ расширения
        /// <para>C:\Programm Files\Application</para>
        /// </summary>
        public string ExePathWithoutExt
        {
            get { return Path.Combine(DirName, ExeName); }
        }

        #endregion

        #region Конструктор

        public FileLocation(string exePath)
        {
            ExePath = exePath;
        }

        #endregion
    }

}
