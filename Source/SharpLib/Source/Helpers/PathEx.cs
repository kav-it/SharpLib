﻿using System.IO;

namespace SharpLib
{
    public static class PathEx
    {
        #region Методы

        /// <summary>
        /// Объединение путей
        /// </summary>
        public static string Combine(string path1, string path2)
        {
            if (path1 == null || path2 == null)
            {
                return path1;
            }

            return Path.Combine(path1, path2);
        }

        /// <summary>
        /// Объединение путей
        /// </summary>
        public static string Combine(string path1, string path2, string path3)
        {
            if (path1 == null || path2 == null || path3 == null)
            {
                return path1;
            }

            return Path.Combine(path1, path2, path3);
        }

        /// <summary>
        /// Переход на директорию выше
        /// </summary>
        public static string SetUp(string path)
        {
            return Path.Combine(path, "..");
        }

        #endregion
    }
}