using System;
using System.Runtime.InteropServices;

using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
    internal class AudioCaptureClient : IDisposable
    {
        #region Поля

        private IAudioCaptureClient audioCaptureClientInterface;

        #endregion

        #region Конструктор

        internal AudioCaptureClient(IAudioCaptureClient audioCaptureClientInterface)
        {
            this.audioCaptureClientInterface = audioCaptureClientInterface;
        }

        #endregion

        #region Методы

        public IntPtr GetBuffer(
            out int numFramesToRead,
            out AudioClientBufferFlags bufferFlags,
            out long devicePosition,
            out long qpcPosition)
        {
            IntPtr bufferPointer;
            Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetBuffer(out bufferPointer, out numFramesToRead, out bufferFlags, out devicePosition, out qpcPosition));
            return bufferPointer;
        }

        public IntPtr GetBuffer(
            out int numFramesToRead,
            out AudioClientBufferFlags bufferFlags)
        {
            IntPtr bufferPointer;
            long devicePosition;
            long qpcPosition;
            Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetBuffer(out bufferPointer, out numFramesToRead, out bufferFlags, out devicePosition, out qpcPosition));
            return bufferPointer;
        }

        public int GetNextPacketSize()
        {
            int numFramesInNextPacket;
            Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetNextPacketSize(out numFramesInNextPacket));
            return numFramesInNextPacket;
        }

        public void ReleaseBuffer(int numFramesWritten)
        {
            Marshal.ThrowExceptionForHR(audioCaptureClientInterface.ReleaseBuffer(numFramesWritten));
        }

        public void Dispose()
        {
            if (audioCaptureClientInterface != null)
            {
                Marshal.ReleaseComObject(audioCaptureClientInterface);
                audioCaptureClientInterface = null;
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}