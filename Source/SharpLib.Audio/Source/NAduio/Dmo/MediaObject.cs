using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using SharpLib.Audio.Utils;
using SharpLib.Audio.Wave;

namespace SharpLib.Audio.Dmo
{
    internal class MediaObject : IDisposable
    {
        #region Поля

        private readonly int inputStreams;

        private readonly int outputStreams;

        private IMediaObject mediaObject;

        #endregion

        #region Свойства

        public int InputStreamCount
        {
            get { return inputStreams; }
        }

        public int OutputStreamCount
        {
            get { return outputStreams; }
        }

        #endregion

        #region Конструктор

        internal MediaObject(IMediaObject mediaObject)
        {
            this.mediaObject = mediaObject;
            mediaObject.GetStreamCount(out inputStreams, out outputStreams);
        }

        #endregion

        #region Методы

        public DmoMediaType? GetInputType(int inputStream, int inputTypeIndex)
        {
            try
            {
                DmoMediaType mediaType;
                int hresult = mediaObject.GetInputType(inputStream, inputTypeIndex, out mediaType);
                if (hresult == HResult.S_OK)
                {
                    DmoInterop.MoFreeMediaType(ref mediaType);
                    return mediaType;
                }
            }
            catch (COMException e)
            {
                if (e.GetHResult() != (int)DmoHResults.DMO_E_NO_MORE_ITEMS)
                {
                    throw;
                }
            }
            return null;
        }

        public DmoMediaType? GetOutputType(int outputStream, int outputTypeIndex)
        {
            try
            {
                DmoMediaType mediaType;
                int hresult = mediaObject.GetOutputType(outputStream, outputTypeIndex, out mediaType);
                if (hresult == HResult.S_OK)
                {
                    DmoInterop.MoFreeMediaType(ref mediaType);
                    return mediaType;
                }
            }
            catch (COMException e)
            {
                if (e.GetHResult() != (int)DmoHResults.DMO_E_NO_MORE_ITEMS)
                {
                    throw;
                }
            }
            return null;
        }

        public DmoMediaType GetOutputCurrentType(int outputStreamIndex)
        {
            DmoMediaType mediaType;
            int hresult = mediaObject.GetOutputCurrentType(outputStreamIndex, out mediaType);
            if (hresult == HResult.S_OK)
            {
                DmoInterop.MoFreeMediaType(ref mediaType);
                return mediaType;
            }
            if (hresult == (int)DmoHResults.DMO_E_TYPE_NOT_SET)
            {
                throw new InvalidOperationException("Media type was not set.");
            }
            throw Marshal.GetExceptionForHR(hresult);
        }

        public IEnumerable<DmoMediaType> GetInputTypes(int inputStreamIndex)
        {
            int typeIndex = 0;
            DmoMediaType? mediaType;
            while ((mediaType = GetInputType(inputStreamIndex, typeIndex)) != null)
            {
                yield return mediaType.Value;
                typeIndex++;
            }
        }

        public IEnumerable<DmoMediaType> GetOutputTypes(int outputStreamIndex)
        {
            int typeIndex = 0;
            DmoMediaType? mediaType;
            while ((mediaType = GetOutputType(outputStreamIndex, typeIndex)) != null)
            {
                yield return mediaType.Value;
                typeIndex++;
            }
        }

        public bool SupportsInputType(int inputStreamIndex, DmoMediaType mediaType)
        {
            return SetInputType(inputStreamIndex, mediaType, DmoSetTypeFlags.DMO_SET_TYPEF_TEST_ONLY);
        }

        private bool SetInputType(int inputStreamIndex, DmoMediaType mediaType, DmoSetTypeFlags flags)
        {
            int hResult = mediaObject.SetInputType(inputStreamIndex, ref mediaType, flags);
            if (hResult != HResult.S_OK)
            {
                if (hResult == (int)DmoHResults.DMO_E_INVALIDSTREAMINDEX)
                {
                    throw new ArgumentException("Invalid stream index");
                }
                if (hResult == (int)DmoHResults.DMO_E_TYPE_NOT_ACCEPTED)
                {
                    Debug.WriteLine("Media type was not accepted");
                }

                return false;
            }
            return true;
        }

        public void SetInputType(int inputStreamIndex, DmoMediaType mediaType)
        {
            if (!SetInputType(inputStreamIndex, mediaType, DmoSetTypeFlags.None))
            {
                throw new ArgumentException("Media Type not supported");
            }
        }

        public void SetInputWaveFormat(int inputStreamIndex, WaveFormat waveFormat)
        {
            DmoMediaType mediaType = CreateDmoMediaTypeForWaveFormat(waveFormat);
            bool set = SetInputType(inputStreamIndex, mediaType, DmoSetTypeFlags.None);
            DmoInterop.MoFreeMediaType(ref mediaType);
            if (!set)
            {
                throw new ArgumentException("Media Type not supported");
            }
        }

        public bool SupportsInputWaveFormat(int inputStreamIndex, WaveFormat waveFormat)
        {
            DmoMediaType mediaType = CreateDmoMediaTypeForWaveFormat(waveFormat);
            bool supported = SetInputType(inputStreamIndex, mediaType, DmoSetTypeFlags.DMO_SET_TYPEF_TEST_ONLY);
            DmoInterop.MoFreeMediaType(ref mediaType);
            return supported;
        }

        private DmoMediaType CreateDmoMediaTypeForWaveFormat(WaveFormat waveFormat)
        {
            DmoMediaType mediaType = new DmoMediaType();
            int waveFormatExSize = Marshal.SizeOf(waveFormat);
            DmoInterop.MoInitMediaType(ref mediaType, waveFormatExSize);
            mediaType.SetWaveFormat(waveFormat);
            return mediaType;
        }

        public bool SupportsOutputType(int outputStreamIndex, DmoMediaType mediaType)
        {
            return SetOutputType(outputStreamIndex, mediaType, DmoSetTypeFlags.DMO_SET_TYPEF_TEST_ONLY);
        }

        public bool SupportsOutputWaveFormat(int outputStreamIndex, WaveFormat waveFormat)
        {
            DmoMediaType mediaType = CreateDmoMediaTypeForWaveFormat(waveFormat);
            bool supported = SetOutputType(outputStreamIndex, mediaType, DmoSetTypeFlags.DMO_SET_TYPEF_TEST_ONLY);
            DmoInterop.MoFreeMediaType(ref mediaType);
            return supported;
        }

        private bool SetOutputType(int outputStreamIndex, DmoMediaType mediaType, DmoSetTypeFlags flags)
        {
            int hresult = mediaObject.SetOutputType(outputStreamIndex, ref mediaType, flags);
            if (hresult == (int)DmoHResults.DMO_E_TYPE_NOT_ACCEPTED)
            {
                return false;
            }
            if (hresult == HResult.S_OK)
            {
                return true;
            }
            throw Marshal.GetExceptionForHR(hresult);
        }

        public void SetOutputType(int outputStreamIndex, DmoMediaType mediaType)
        {
            if (!SetOutputType(outputStreamIndex, mediaType, DmoSetTypeFlags.None))
            {
                throw new ArgumentException("Media Type not supported");
            }
        }

        public void SetOutputWaveFormat(int outputStreamIndex, WaveFormat waveFormat)
        {
            DmoMediaType mediaType = CreateDmoMediaTypeForWaveFormat(waveFormat);
            bool succeeded = SetOutputType(outputStreamIndex, mediaType, DmoSetTypeFlags.None);
            DmoInterop.MoFreeMediaType(ref mediaType);
            if (!succeeded)
            {
                throw new ArgumentException("Media Type not supported");
            }
        }

        public MediaObjectSizeInfo GetInputSizeInfo(int inputStreamIndex)
        {
            int size;
            int maxLookahead;
            int alignment;
            Marshal.ThrowExceptionForHR(mediaObject.GetInputSizeInfo(inputStreamIndex, out size, out maxLookahead, out alignment));
            return new MediaObjectSizeInfo(size, maxLookahead, alignment);
        }

        public MediaObjectSizeInfo GetOutputSizeInfo(int outputStreamIndex)
        {
            int size;
            int alignment;
            Marshal.ThrowExceptionForHR(mediaObject.GetOutputSizeInfo(outputStreamIndex, out size, out alignment));
            return new MediaObjectSizeInfo(size, 0, alignment);
        }

        public void ProcessInput(int inputStreamIndex,
            IMediaBuffer mediaBuffer,
            DmoInputDataBufferFlags flags,
            long timestamp,
            long duration)
        {
            Marshal.ThrowExceptionForHR(mediaObject.ProcessInput(inputStreamIndex, mediaBuffer, flags, timestamp, duration));
        }

        public void ProcessOutput(DmoProcessOutputFlags flags, int outputBufferCount, DmoOutputDataBuffer[] outputBuffers)
        {
            int reserved;
            Marshal.ThrowExceptionForHR(mediaObject.ProcessOutput(flags, outputBufferCount, outputBuffers, out reserved));
        }

        public void AllocateStreamingResources()
        {
            Marshal.ThrowExceptionForHR(mediaObject.AllocateStreamingResources());
        }

        public void FreeStreamingResources()
        {
            Marshal.ThrowExceptionForHR(mediaObject.FreeStreamingResources());
        }

        public long GetInputMaxLatency(int inputStreamIndex)
        {
            long maxLatency;
            Marshal.ThrowExceptionForHR(mediaObject.GetInputMaxLatency(inputStreamIndex, out maxLatency));
            return maxLatency;
        }

        public void Flush()
        {
            Marshal.ThrowExceptionForHR(mediaObject.Flush());
        }

        public void Discontinuity(int inputStreamIndex)
        {
            Marshal.ThrowExceptionForHR(mediaObject.Discontinuity(inputStreamIndex));
        }

        public bool IsAcceptingData(int inputStreamIndex)
        {
            DmoInputStatusFlags flags;
            int hresult = mediaObject.GetInputStatus(inputStreamIndex, out flags);
            Marshal.ThrowExceptionForHR(hresult);
            return (flags & DmoInputStatusFlags.DMO_INPUT_STATUSF_ACCEPT_DATA) == DmoInputStatusFlags.DMO_INPUT_STATUSF_ACCEPT_DATA;
        }

        public void Dispose()
        {
            if (mediaObject != null)
            {
                Marshal.ReleaseComObject(mediaObject);
                mediaObject = null;
            }
        }

        #endregion
    }
}