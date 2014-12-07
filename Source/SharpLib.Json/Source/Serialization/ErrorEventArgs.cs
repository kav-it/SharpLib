using System;

namespace SharpLib.Json
{
    public class ErrorEventArgs : EventArgs
    {
        #region ��������

        public object CurrentObject { get; private set; }

        public ErrorContext ErrorContext { get; private set; }

        #endregion

        #region �����������

        public ErrorEventArgs(object currentObject, ErrorContext errorContext)
        {
            CurrentObject = currentObject;
            ErrorContext = errorContext;
        }

        #endregion
    }
}