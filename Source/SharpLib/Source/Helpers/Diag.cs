using System;
using System.Diagnostics;

namespace SharpLib
{
    /// <summary>
    /// Вспомогательный класс для диагностики/отладки
    /// </summary>
    public static class Diag
    {
        /// <summary>
        /// Вывод в отладку с параметрами
        /// </summary>
        public static void WriteLine(string format, params object[] args)
        {
            format = string.Format(format, args);
            format = string.Format("[{0:hh:mm:ss.fff}] {1}", DateTime.Now, format);

            Debug.WriteLine(format);
        }
    }
}