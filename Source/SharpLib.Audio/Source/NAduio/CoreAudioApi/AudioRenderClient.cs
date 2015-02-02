using System;
using System.Runtime.InteropServices;

using SharpLib.Audio.CoreAudioApi.Interfaces;

namespace SharpLib.Audio.CoreAudioApi
{
    internal class AudioRenderClient : IDisposable
    {
        #region Поля

        private IAudioRenderClient audioRenderClientInterface;

        #endregion

        #region Конструктор

        internal AudioRenderClient(IAudioRenderClient audioRenderClientInterface)
        {
            this.audioRenderClientInterface = audioRenderClientInterface;
        }

        #endregion

        #region Методы

        public IntPtr GetBuffer(int numFramesRequested)
        {
            IntPtr bufferPointer;
            Marshal.ThrowExceptionForHR(audioRenderClientInterface.GetBuffer(numFramesRequested, out bufferPointer));
            return bufferPointer;
        }

        public void ReleaseBuffer(int numFramesWritten, AudioClientBufferFlags bufferFlags)
        {
            Marshal.ThrowExceptionForHR(audioRenderClientInterface.ReleaseBuffer(numFramesWritten, bufferFlags));
        }

        public void Dispose()
        {
            if (audioRenderClientInterface != null)
            {
                Marshal.ReleaseComObject(audioRenderClientInterface);
                audioRenderClientInterface = null;
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}