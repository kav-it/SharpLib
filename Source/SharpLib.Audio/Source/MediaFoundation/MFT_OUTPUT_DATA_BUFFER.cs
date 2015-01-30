using System.Runtime.InteropServices;

namespace NAudio.MediaFoundation
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MFT_OUTPUT_DATA_BUFFER
    {
        public int dwStreamID;

        public IMFSample pSample;

        public _MFT_OUTPUT_DATA_BUFFER_FLAGS dwStatus;

        public IMFCollection pEvents;
    }
}