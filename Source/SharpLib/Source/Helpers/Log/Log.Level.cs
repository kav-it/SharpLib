using System;

namespace SharpLib.Log
{
    [Flags]
    public enum LogLevel
    {
        Off = (0 << 0),

        Debug = (1 << 0),

        Info = (1 << 1),

        Warn = (1 << 2),

        Error = (1 << 3),

        In = (1 << 10),

        Out = (1 << 11),

        On = (Debug | Info | Warn | Error),

        All = (On | In | Out)
    }
}