using System;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.CoreAudioApi
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct AudioClientProperties
    {
        public UInt32 cbSize;

        public int bIsOffload;

        public AudioStreamCategory eCategory;

        public AudioClientStreamOptions Options;
    }
}