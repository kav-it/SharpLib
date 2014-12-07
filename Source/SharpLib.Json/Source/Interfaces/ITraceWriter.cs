using System;
using System.Diagnostics;

namespace SharpLib.Json
{
    public interface ITraceWriter
    {
        #region Свойства

        TraceLevel LevelFilter { get; }

        #endregion

        #region Методы

        void Trace(TraceLevel level, string message, Exception ex);

        #endregion
    }
}