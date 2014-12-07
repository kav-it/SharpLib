using System;

namespace SharpLib.Json
{
    public class ErrorContext
    {
        #region Свойства

        internal bool Traced { get; set; }

        public Exception Error { get; private set; }

        public object OriginalObject { get; private set; }

        public object Member { get; private set; }

        public string Path { get; private set; }

        public bool Handled { get; set; }

        #endregion

        #region Конструктор

        internal ErrorContext(object originalObject, object member, string path, Exception error)
        {
            OriginalObject = originalObject;
            Member = member;
            Error = error;
            Path = path;
        }

        #endregion
    }
}