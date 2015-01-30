using System.Runtime.InteropServices;

namespace NAudio.Wave.Compression
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal class WaveFilter
    {
        public int StructureSize = Marshal.SizeOf(typeof(WaveFilter));

        public int FilterTag = 0;

        public int Filter = 0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] Reserved = null;
    }
}