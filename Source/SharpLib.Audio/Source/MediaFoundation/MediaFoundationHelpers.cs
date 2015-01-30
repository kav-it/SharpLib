using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using NAudio.Wave;

namespace NAudio.MediaFoundation
{
    internal static class MediaFoundationApi
    {
        private static bool initialized;

        public static void Startup()
        {
            if (!initialized)
            {
                MediaFoundationInterop.MFStartup(MediaFoundationInterop.MF_VERSION, 0);
                initialized = true;
            }
        }

#if !NETFX_CORE

        public static IEnumerable<IMFActivate> EnumerateTransforms(Guid category)
        {
            IntPtr interfacesPointer;
            int interfaceCount;
            MediaFoundationInterop.MFTEnumEx(category, _MFT_ENUM_FLAG.MFT_ENUM_FLAG_ALL,
                null, null, out interfacesPointer, out interfaceCount);
            var interfaces = new IMFActivate[interfaceCount];
            for (int n = 0; n < interfaceCount; n++)
            {
                var ptr =
                    Marshal.ReadIntPtr(new IntPtr(interfacesPointer.ToInt64() + n * Marshal.SizeOf(interfacesPointer)));
                interfaces[n] = (IMFActivate)Marshal.GetObjectForIUnknown(ptr);
            }

            foreach (var i in interfaces)
            {
                yield return i;
            }
            Marshal.FreeCoTaskMem(interfacesPointer);
        }
#endif

        public static void Shutdown()
        {
            if (initialized)
            {
                MediaFoundationInterop.MFShutdown();
                initialized = false;
            }
        }

        public static IMFMediaType CreateMediaType()
        {
            IMFMediaType mediaType;
            MediaFoundationInterop.MFCreateMediaType(out mediaType);
            return mediaType;
        }

        public static IMFMediaType CreateMediaTypeFromWaveFormat(WaveFormat waveFormat)
        {
            var mediaType = CreateMediaType();
            try
            {
                MediaFoundationInterop.MFInitMediaTypeFromWaveFormatEx(mediaType, waveFormat, Marshal.SizeOf(waveFormat));
            }
            catch (Exception)
            {
                Marshal.ReleaseComObject(mediaType);
                throw;
            }
            return mediaType;
        }

        public static IMFMediaBuffer CreateMemoryBuffer(int bufferSize)
        {
            IMFMediaBuffer buffer;
            MediaFoundationInterop.MFCreateMemoryBuffer(bufferSize, out buffer);
            return buffer;
        }

        public static IMFSample CreateSample()
        {
            IMFSample sample;
            MediaFoundationInterop.MFCreateSample(out sample);
            return sample;
        }

        public static IMFAttributes CreateAttributes(int initialSize)
        {
            IMFAttributes attributes;
            MediaFoundationInterop.MFCreateAttributes(out attributes, initialSize);
            return attributes;
        }

        public static IMFByteStream CreateByteStream(object stream)
        {
            IMFByteStream byteStream;
            MediaFoundationInterop.MFCreateMFByteStreamOnStreamEx(stream, out byteStream);
            return byteStream;
        }

        public static IMFSourceReader CreateSourceReaderFromByteStream(IMFByteStream byteStream)
        {
            IMFSourceReader reader;
            MediaFoundationInterop.MFCreateSourceReaderFromByteStream(byteStream, null, out reader);
            return reader;
        }
    }
}