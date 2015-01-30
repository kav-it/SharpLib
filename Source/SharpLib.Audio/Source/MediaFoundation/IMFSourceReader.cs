using System;
using System.Runtime.InteropServices;

using SharpLib.Audio.CoreAudioApi.Interfaces;

namespace SharpLib.Audio.MediaFoundation
{
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("70ae66f2-c809-4e4f-8915-bdcb406b7993")]
    internal interface IMFSourceReader
    {
        void GetStreamSelection([In] int dwStreamIndex, [Out, MarshalAs(UnmanagedType.Bool)] out bool pSelected);

        void SetStreamSelection([In] int dwStreamIndex, [In, MarshalAs(UnmanagedType.Bool)] bool pSelected);

        void GetNativeMediaType([In] int dwStreamIndex, [In] int dwMediaTypeIndex, [Out] out IMFMediaType ppMediaType);

        void GetCurrentMediaType([In] int dwStreamIndex, [Out] out IMFMediaType ppMediaType);

        void SetCurrentMediaType([In] int dwStreamIndex, IntPtr pdwReserved, [In] IMFMediaType pMediaType);

        void SetCurrentPosition([In, MarshalAs(UnmanagedType.LPStruct)] Guid guidTimeFormat, [In] ref PropVariant varPosition);

        void ReadSample([In] int dwStreamIndex,
            [In] int dwControlFlags,
            [Out] out int pdwActualStreamIndex,
            [Out] out MF_SOURCE_READER_FLAG pdwStreamFlags,
            [Out] out UInt64 pllTimestamp,
            [Out] out IMFSample ppSample);

        void Flush([In] int dwStreamIndex);

        void GetServiceForStream([In] int dwStreamIndex,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidService,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [Out] out IntPtr ppvObject);

        [PreserveSig]
        int GetPresentationAttribute([In] int dwStreamIndex, [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidAttribute, [Out] out PropVariant pvarAttribute);
    }

    [Flags]
    internal enum MF_SOURCE_READER_FLAG
    {
        None = 0,

        MF_SOURCE_READERF_ERROR = 0x00000001,

        MF_SOURCE_READERF_ENDOFSTREAM = 0x00000002,

        MF_SOURCE_READERF_NEWSTREAM = 0x00000004,

        MF_SOURCE_READERF_NATIVEMEDIATYPECHANGED = 0x00000010,

        MF_SOURCE_READERF_CURRENTMEDIATYPECHANGED = 0x00000020,

        MF_SOURCE_READERF_STREAMTICK = 0x00000100,

        MF_SOURCE_READERF_ALLEFFECTSREMOVED = 0x00000200
    }
}