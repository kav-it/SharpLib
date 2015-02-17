using System;

namespace SharpLib
{
    public static class ExtensionLong
    {
        #region Методы

        /// <summary>
        /// Преобразование значение в файловый формат (1565485 = 1.5MB)
        /// </summary>
        public static string ToFileSizeEx(this long self, int divider = 1000)
        {
            const string FORMAT_TEMPLATE = "{0}{1:0.#} {2}";

            string[] sizeSuffixes =
            {
                "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"
            };

            string text;

            if (self == 0)
            {
                text = string.Format(FORMAT_TEMPLATE, null, 0, sizeSuffixes[0]);
            }
            else
            {
                double absSize = Math.Abs((double)self);
                double fpPower = Math.Log(absSize, divider);
                int intPower = (int)fpPower;
                int iUnit = (intPower >= sizeSuffixes.Length) ? sizeSuffixes.Length - 1 : intPower;
                double normSize = absSize / Math.Pow(divider, iUnit);

                text = String.Format(FORMAT_TEMPLATE,
                    (self < 0) ? "-" : null,
                    normSize,
                    sizeSuffixes[iUnit]);
            }

            text = text.Replace(',', '.');

            return text;
        }

        #endregion
    }
}