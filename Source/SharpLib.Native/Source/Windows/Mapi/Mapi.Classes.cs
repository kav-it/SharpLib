using System;
using System.Runtime.InteropServices;

namespace SharpLib.Native.Windows
{
    public partial class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MapiMessage
        {
            public int Reserved;

            public String Subject;

            public String NoteText;

            public String MessageTyp;

            public String DateReceived;

            public String ConversationID;

            public int Flags;

            internal IntPtr Originator;

            public int RecipCount;

            internal IntPtr Recips;

            public int FileCount;

            internal IntPtr Files;
        }

    }
}