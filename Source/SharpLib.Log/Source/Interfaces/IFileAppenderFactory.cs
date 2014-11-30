using System;
using System.Collections.Generic;

namespace SharpLib.Log
{
    internal interface IFileAppenderFactory
    {
        #region Методы

        BaseFileAppender Open(string fileName, ICreateFileParameters parameters);

        #endregion
    }
}