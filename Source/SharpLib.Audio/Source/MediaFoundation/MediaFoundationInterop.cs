using System;
using System.Runtime.InteropServices;

using NAudio.Wave;

namespace NAudio.MediaFoundation
{
    internal static class MediaFoundationInterop
    {
        [DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
        public static extern void MFStartup(int version, int dwFlags = 0);

        [DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
        public static extern void MFShutdown();

        [DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
        internal static extern void MFCreateMediaType(out IMFMediaType ppMFType);

        [DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
        internal static extern void MFInitMediaTypeFromWaveFormatEx([In] IMFMediaType pMFType, [In] WaveFormat pWaveFormat, [In] int cbBufSize);

        [DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
        internal static extern void MFCreateWaveFormatExFromMFMediaType(IMFMediaType pMFType, ref IntPtr ppWF, ref int pcbSize, int flags = 0);

        [DllImport("mfreadwrite.dll", ExactSpelling = true, PreserveSig = false)]
        public static extern void MFCreateSourceReaderFromURL([In, MarshalAs(UnmanagedType.LPWStr)] string pwszURL,
            [In] IMFAttributes pAttributes,
            [Out, MarshalAs(UnmanagedType.Interface)] out IMFSourceReader ppSourceReader);

        [DllImport("mfreadwrite.dll", ExactSpelling = true, PreserveSig = false)]
        public static extern void MFCreateSourceReaderFromByteStream([In] IMFByteStream pByteStream,
            [In] IMFAttributes pAttributes,
            [Out, MarshalAs(UnmanagedType.Interface)] out IMFSourceReader ppSourceReader);

        [DllImport("mfreadwrite.dll", ExactSpelling = true, PreserveSig = false)]
        public static extern void MFCreateSinkWriterFromURL([In, MarshalAs(UnmanagedType.LPWStr)] string pwszOutputURL,
            [In] IMFByteStream pByteStream,
            [In] IMFAttributes pAttributes,
            [Out] out IMFSinkWriter ppSinkWriter);

        [DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
        public static extern void MFCreateMFByteStreamOnStreamEx([MarshalAs(UnmanagedType.IUnknown)] object punkStream, out IMFByteStream ppByteStream);

#if !NETFX_CORE

        [DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
        public static extern void MFTEnumEx([In] Guid guidCategory,
            [In] _MFT_ENUM_FLAG flags,
            [In] MFT_REGISTER_TYPE_INFO pInputType,
            [In] MFT_REGISTER_TYPE_INFO pOutputType,
            [Out] out IntPtr pppMFTActivate,
            [Out] out int pcMFTActivate);
#endif

        [DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
        internal static extern void MFCreateSample([Out] out IMFSample ppIMFSample);

        [DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
        internal static extern void MFCreateMemoryBuffer(
            int cbMaxLength,
            [Out] out IMFMediaBuffer ppBuffer);

        [DllImport("mfplat.dll", ExactSpelling = true, PreserveSig = false)]
        internal static extern void MFCreateAttributes(
            [Out, MarshalAs(UnmanagedType.Interface)] out IMFAttributes ppMFAttributes,
            [In] int cInitialSize);

#if !NETFX_CORE

        [DllImport("mf.dll", ExactSpelling = true, PreserveSig = false)]
        public static extern void MFTranscodeGetAudioOutputAvailableTypes(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidSubType,
            [In] _MFT_ENUM_FLAG dwMFTFlags,
            [In] IMFAttributes pCodecConfig,
            [Out, MarshalAs(UnmanagedType.Interface)] out IMFCollection ppAvailableTypes);
#endif

        public const int MF_SOURCE_READER_ALL_STREAMS = unchecked((int)0xFFFFFFFE);

        public const int MF_SOURCE_READER_FIRST_AUDIO_STREAM = unchecked((int)0xFFFFFFFD);

        public const int MF_SOURCE_READER_FIRST_VIDEO_STREAM = unchecked((int)0xFFFFFFFC);

        public const int MF_SOURCE_READER_MEDIASOURCE = unchecked((int)0xFFFFFFFF);

        public const int MF_SDK_VERSION = 0x2;

        public const int MF_API_VERSION = 0x70;

        public const int MF_VERSION = (MF_SDK_VERSION << 16) | MF_API_VERSION;
    }
}