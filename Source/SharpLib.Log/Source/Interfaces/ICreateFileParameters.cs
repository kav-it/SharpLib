using System;
using System.Collections.Generic;

namespace SharpLib.Log
{
    internal interface ICreateFileParameters
    {
        int ConcurrentWriteAttemptDelay { get; }

        int ConcurrentWriteAttempts { get; }

        bool ConcurrentWrites { get; }

        bool CreateDirs { get; }

        bool EnableFileDelete { get; }

        int BufferSize { get; }

        bool ForceManaged { get; }

        Win32FileAttributes FileAttributes { get; }
    }
}