using System;
using System.IO;
using System.Reflection;

namespace SharpLib.Texter.Highlighting
{
    internal class HighlightingResources
    {
        #region Константы

        internal const string PREFIX_XSD = "Source/Assets/xsd/";

        internal const string PREFIX_XSHD = "Source/Assets/xshd/";

        #endregion

        #region Поля

        private static readonly Lazy<HighlightingResources> _instance = new Lazy<HighlightingResources>();

        #endregion

        #region Свойства

        public static HighlightingResources Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        #region Методы

        internal Stream OpenStream(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetEmbeddedResourceAsStreamEx(name);
            if (stream == null)
            {
                throw new FileNotFoundException("The resource file '" + name + "' was not found.");
            }

            return stream;
        }

        #endregion
    }
}