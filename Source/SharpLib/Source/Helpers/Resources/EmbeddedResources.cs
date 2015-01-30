using System;
using System.Globalization;

namespace SharpLib
{
    /// <summary>
    /// Класс-помошник работы с Embedded ресурсами
    /// </summary>
    public static class EmbeddedResources
    {
        /// <summary>
        /// Извлечение имени ресурса из полного пути
        /// </summary>
        /// <remarks>
        /// В проекте : SharpLib - Assets - file1.txt
        /// Путь      : SharpLib.Assets.file1.txt
        /// Имя       : file1.txt
        /// </remarks>
        public static string ExtractName(string path)
        {
            var chunks = path.Split('.');

            if (chunks.Length < 3)
            {
                throw new Exception("Имя ресурса должно содержать расширение");
            }

            var index = chunks.Length - 2;
            var result = string.Format("{0}.{1}", chunks[index], chunks[index + 1]);

            return result;
        }
    }
}