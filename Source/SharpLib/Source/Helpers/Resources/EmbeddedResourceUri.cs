using System;
using System.Linq;

namespace SharpLib
{
    /// <summary>
    /// Класс Uri Embedded ресурсов
    /// </summary>
    public class EmbeddedResourceUri
    {
        #region Свойства

        /// <summary>
        /// Путь к ресурсу (в терминах Project)
        /// </summary>
        public string Directory { get; private set; }

        /// <summary>
        /// Имя ресурса
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Путь с символами '/' (человеческое представление)
        /// </summary>
        public string SlashPath
        {
            get { return string.Format("{0}/{1}", Directory, Name); }
        }

        /// <summary>
        /// Путь с символами '.' (microsoft представление)
        /// </summary>
        public string DotPath
        {
            get { return string.Format("{0}.{1}", Directory.Replace('/', '.'), Name); }
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// </summary>
        public EmbeddedResourceUri(string fullpath)
        {
            Parse(fullpath);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Разбор полного пути на составляющие
        /// </summary>
        /// <remarks>
        /// Путь      : SharpLib/Source/Assets/file.txt
        /// Directory : SharpLib/Source/Assets
        /// Name      : file.txt
        /// </remarks>
        private void Parse(string uri)
        {
            uri = uri.Replace("\\", "/");

            Directory = string.Empty;
            Name = string.Empty;

            var chunks = uri.Split('/');

            if (chunks.Length == 0)
            {
                throw new Exception("Неверный формат uri");
            }

            Directory = chunks.Take(chunks.Length - 1).JoinEx("/");
            Name = chunks.Last(); 
        }

        /// <summary>
        /// Текстовое представление
        /// </summary>
        public override string ToString()
        {
            return SlashPath;
        }

        #endregion
    }
}