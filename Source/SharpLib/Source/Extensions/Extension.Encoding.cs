using System.Text;

namespace SharpLib
{
    public static class ExtensionEncoding
    {
        #region Поля

        /// <summary>
        /// Кодировка UTF-8 без идиотского мелкософтного BOM
        /// </summary>
        private static Encoding _utf8;

        /// <summary>
        /// Кодировка Windows-1251 (FF - русская 'я')
        /// </summary>
        private static Encoding _win1251;

        #endregion

        #region Свойства

        /// <summary>
        /// Кодировка UTF-8 без BOM
        /// </summary>
        public static Encoding Utf8
        {
            get { return _utf8 ?? (_utf8 = new UTF8Encoding(false)); }
        }

        /// <summary>
        /// Кодировка Windows-1251
        /// </summary>
        public static Encoding Windows1251
        {
            get { return _win1251 ?? (_win1251 = Encoding.GetEncoding(1251)); }
        }

        #endregion
    }

}
