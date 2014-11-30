using System;
using System.Collections.Generic;
using System.IO;

namespace SharpLib.Log
{
    [Flags]
    public enum Win32FileAttributes
    {
        ReadOnly = 0x00000001,

        Hidden = 0x00000002,

        System = 0x00000004,

        Archive = 0x00000020,

        Device = 0x00000040,

        Normal = 0x00000080,

        Temporary = 0x00000100,

        SparseFile = 0x00000200,

        ReparsePoint = 0x00000400,

        Compressed = 0x00000800,

        NotContentIndexed = 0x00002000,

        Encrypted = 0x00004000,

        WriteThrough = unchecked((int)0x80000000),

        NoBuffering = 0x20000000,

        DeleteOnClose = 0x04000000,

        PosixSemantics = 0x01000000,
    }
}