using System;
using System.Runtime.InteropServices;

namespace NAudio.MediaFoundation
{
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("bf94c121-5b05-4e6f-8000-ba598961414d")]
    internal interface IMFTransform
    {
        void GetStreamLimits([Out] out int pdwInputMinimum, [Out] out int pdwInputMaximum, [Out] out int pdwOutputMinimum, [Out] out int pdwOutputMaximum);

        void GetStreamCount([Out] out int pcInputStreams, [Out] out int pcOutputStreams);

        void GetStreamIds([In] int dwInputIDArraySize, [In, Out] int[] pdwInputIDs, [In] int dwOutputIDArraySize, [In, Out] int[] pdwOutputIDs);

        void GetInputStreamInfo([In] int dwInputStreamID, [Out] out MFT_INPUT_STREAM_INFO pStreamInfo);

        void GetOutputStreamInfo([In] int dwOutputStreamID, [Out] out MFT_OUTPUT_STREAM_INFO pStreamInfo);

        void GetAttributes([Out] out IMFAttributes pAttributes);

        void GetInputStreamAttributes([In] int dwInputStreamID, [Out] out IMFAttributes pAttributes);

        void GetOutputStreamAttributes([In] int dwOutputStreamID, [Out] out IMFAttributes pAttributes);

        void DeleteInputStream([In] int dwOutputStreamID);

        void AddInputStreams([In] int cStreams, [In] int[] adwStreamIDs);

        void GetInputAvailableType([In] int dwInputStreamID, [In] int dwTypeIndex, [Out] out IMFMediaType ppType);

        void GetOutputAvailableType([In] int dwOutputStreamID, [In] int dwTypeIndex, [Out] out IMFMediaType ppType);

        void SetInputType([In] int dwInputStreamID, [In] IMFMediaType pType, [In] _MFT_SET_TYPE_FLAGS dwFlags);

        void SetOutputType([In] int dwOutputStreamID, [In] IMFMediaType pType, [In] _MFT_SET_TYPE_FLAGS dwFlags);

        void GetInputCurrentType([In] int dwInputStreamID, [Out] out IMFMediaType ppType);

        void GetOutputCurrentType([In] int dwOutputStreamID, [Out] out IMFMediaType ppType);

        void GetInputStatus([In] int dwInputStreamID, [Out] out _MFT_INPUT_STATUS_FLAGS pdwFlags);

        void GetOutputStatus([In] int dwInputStreamID, [Out] out _MFT_OUTPUT_STATUS_FLAGS pdwFlags);

        void SetOutputBounds([In] long hnsLowerBound, [In] long hnsUpperBound);

        void ProcessEvent([In] int dwInputStreamID, [In] IMFMediaEvent pEvent);

        void ProcessMessage([In] MFT_MESSAGE_TYPE eMessage, [In] IntPtr ulParam);

        void ProcessInput([In] int dwInputStreamID, [In] IMFSample pSample, int dwFlags);

        [PreserveSig]
        int ProcessOutput([In] _MFT_PROCESS_OUTPUT_FLAGS dwFlags,
            [In] int cOutputBufferCount,
            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] MFT_OUTPUT_DATA_BUFFER[] pOutputSamples,
            [Out] out _MFT_PROCESS_OUTPUT_STATUS pdwStatus);
    }
}