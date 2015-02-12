using System;
using System.Text;

namespace SharpLib
{
    /// <summary>
    /// Методы расширения для класса 'StringBuilder'
    /// </summary>
    public static class ExtensionStringBuilder
    {
        #region Методы

        /// <summary>
        /// Добавление строки с форматированием
        /// </summary>
        public static void AppendLineEx(this StringBuilder self, string format, params object[] parameters)
        {
            self.AppendFormat(format, parameters);
            self.Append(Environment.NewLine);
        }

        #endregion
    }
}