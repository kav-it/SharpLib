using System.Runtime.InteropServices;

namespace SharpLib.Audio.MediaFoundation
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MFT_OUTPUT_STREAM_INFO
    {
        public _MFT_OUTPUT_STREAM_INFO_FLAGS dwFlags;

        public int cbSize;

        public int cbAlignment;
    }
}