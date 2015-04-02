using System;
using System.IO;
using System.Reflection;

namespace SharpLib.Notepad.Highlighting
{
    internal class HighlightingResources
    {
        #region Константы

        private const string PREFIX = "Source/Assets/xshd/";

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
            var stream = Assembly.GetExecutingAssembly().GetEmbeddedResourceAsStreamEx(PREFIX + name);
            if (stream == null)
            {
                throw new FileNotFoundException("The resource file '" + name + "' was not found.");
            }

            return stream;
        }

        #endregion
    }
}