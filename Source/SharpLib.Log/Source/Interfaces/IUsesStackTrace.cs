using System;
using System.Collections.Generic;

namespace SharpLib.Log
{
    public interface IUsesStackTrace
    {
        #region Свойства

        StackTraceUsage StackTraceUsage { get; }

        #endregion
    }
}