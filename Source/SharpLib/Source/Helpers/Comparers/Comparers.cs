using System;

namespace SharpLib
{
    public static class Comparers
    {
        #region Поля

        private static readonly Lazy<ComparersFilename> _filename = new Lazy<ComparersFilename>();

        #endregion

        #region Свойства

        /// <summary>
        /// Сравнение по именам файлов
        /// </summary>
        public static ComparersFilename Filename
        {
            get { return _filename.Value; }
        }

        #endregion
    }
}