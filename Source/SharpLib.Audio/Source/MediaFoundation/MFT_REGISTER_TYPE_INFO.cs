using System;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.MediaFoundation
{
    [StructLayout(LayoutKind.Sequential)]
    internal class MFT_REGISTER_TYPE_INFO
    {
        public Guid guidMajorType;

        public Guid guidSubtype;
    }
}