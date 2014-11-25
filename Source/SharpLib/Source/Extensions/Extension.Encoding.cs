using System.Text;

namespace SharpLib
{
    public static class ExtensionEncoding
    {
        #region Поля

        private static Encoding _utf8;

        #endregion

        #region Свойства

        public static Encoding Utf8
        {
            get { return _utf8 ?? (_utf8 = new UTF8Encoding(false)); }
        }

        #endregion
    }

}
