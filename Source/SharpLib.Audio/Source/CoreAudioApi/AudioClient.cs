using System;
using System.Runtime.InteropServices;

using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;

namespace NAudio.CoreAudioApi
{
    internal class AudioClient : IDisposable
    {
        #region Поля

        private AudioCaptureClient audioCaptureClient;

        private IAudioClient audioClientInterface;

        private AudioClockClient audioClockClient;

        private AudioRenderClient audioRenderClient;

        private WaveFormat mixFormat;

        #endregion

        #region Свойства

        public WaveFormat MixFormat
        {
            get
            {
                if (mixFormat == null)
                {
                    IntPtr waveFormatPointer;
                    Marshal.ThrowExceptionForHR(audioClientInterface.GetMixFormat(out waveFormatPointer));
                    var waveFormat = WaveFormat.MarshalFromPtr(waveFormatPointer);
                    Marshal.FreeCoTaskMem(waveFormatPointer);
                    mixFormat = waveFormat;
                }
                return mixFormat;
            }
        }

        public int BufferSize
        {
            get
            {
                uint bufferSize;
                Marshal.ThrowExceptionForHR(audioClientInterface.GetBufferSize(out bufferSize));
                return (int)bufferSize;
            }
        }

        public long StreamLatency
        {
            get { return audioClientInterface.GetStreamLatency(); }
        }

        public int CurrentPadding
        {
            get
            {
                int currentPadding;
                Marshal.ThrowExceptionForHR(audioClientInterface.GetCurrentPadding(out currentPadding));
                return currentPadding;
            }
        }

        public long DefaultDevicePeriod
        {
            get
            {
                long defaultDevicePeriod;
                long minimumDevicePeriod;
                Marshal.ThrowExceptionForHR(audioClientInterface.GetDevicePeriod(out defaultDevicePeriod, out minimumDevicePeriod));
                return defaultDevicePeriod;
            }
        }

        public long MinimumDevicePeriod
        {
            get
            {
                long defaultDevicePeriod;
                long minimumDevicePeriod;
                Marshal.ThrowExceptionForHR(audioClientInterface.GetDevicePeriod(out defaultDevicePeriod, out minimumDevicePeriod));
                return minimumDevicePeriod;
            }
        }

        public AudioClockClient AudioClockClient
        {
            get
            {
                if (audioClockClient == null)
                {
                    object audioClockClientInterface;
                    var audioClockClientGuid = new Guid("CD63314F-3FBA-4a1b-812C-EF96358728E7");
                    Marshal.ThrowExceptionForHR(audioClientInterface.GetService(audioClockClientGuid, out audioClockClientInterface));
                    audioClockClient = new AudioClockClient((IAudioClock)audioClockClientInterface);
                }
                return audioClockClient;
            }
        }

        public AudioRenderClient AudioRenderClient
        {
            get
            {
                if (audioRenderClient == null)
                {
                    object audioRenderClientInterface;
                    var audioRenderClientGuid = new Guid("F294ACFC-3146-4483-A7BF-ADDCA7C260E2");
                    Marshal.ThrowExceptionForHR(audioClientInterface.GetService(audioRenderClientGuid, out audioRenderClientInterface));
                    audioRenderClient = new AudioRenderClient((IAudioRenderClient)audioRenderClientInterface);
                }
                return audioRenderClient;
            }
        }

        public AudioCaptureClient AudioCaptureClient
        {
            get
            {
                if (audioCaptureClient == null)
                {
                    object audioCaptureClientInterface;
                    var audioCaptureClientGuid = new Guid("c8adbd64-e71e-48a0-a4de-185c395cd317");
                    Marshal.ThrowExceptionForHR(audioClientInterface.GetService(audioCaptureClientGuid, out audioCaptureClientInterface));
                    audioCaptureClient = new AudioCaptureClient((IAudioCaptureClient)audioCaptureClientInterface);
                }
                return audioCaptureClient;
            }
        }

        #endregion

        #region Конструктор

        internal AudioClient(IAudioClient audioClientInterface)
        {
            this.audioClientInterface = audioClientInterface;
        }

        #endregion

        #region Методы

        public void Initialize(AudioClientShareMode shareMode,
            AudioClientStreamFlags streamFlags,
            long bufferDuration,
            long periodicity,
            WaveFormat waveFormat,
            Guid audioSessionGuid)
        {
            int hresult = audioClientInterface.Initialize(shareMode, streamFlags, bufferDuration, periodicity, waveFormat, ref audioSessionGuid);
            Marshal.ThrowExceptionForHR(hresult);

            mixFormat = null;
        }

        public bool IsFormatSupported(AudioClientShareMode shareMode,
            WaveFormat desiredFormat)
        {
            WaveFormatExtensible closestMatchFormat;
            return IsFormatSupported(shareMode, desiredFormat, out closestMatchFormat);
        }

        public bool IsFormatSupported(AudioClientShareMode shareMode, WaveFormat desiredFormat, out WaveFormatExtensible closestMatchFormat)
        {
            int hresult = audioClientInterface.IsFormatSupported(shareMode, desiredFormat, out closestMatchFormat);

            if (hresult == 0)
            {
                return true;
            }
            if (hresult == 1)
            {
                return false;
            }
            if (hresult == (int)AudioClientErrors.UnsupportedFormat)
            {
                return false;
            }
            Marshal.ThrowExceptionForHR(hresult);

            throw new NotSupportedException("Unknown hresult " + hresult);
        }

        public void Start()
        {
            audioClientInterface.Start();
        }

        public void Stop()
        {
            audioClientInterface.Stop();
        }

        public void SetEventHandle(IntPtr eventWaitHandle)
        {
            audioClientInterface.SetEventHandle(eventWaitHandle);
        }

        public void Reset()
        {
            audioClientInterface.Reset();
        }

        public void Dispose()
        {
            if (audioClientInterface != null)
            {
                if (audioClockClient != null)
                {
                    audioClockClient.Dispose();
                    audioClockClient = null;
                }
                if (audioRenderClient != null)
                {
                    audioRenderClient.Dispose();
                    audioRenderClient = null;
                }
                if (audioCaptureClient != null)
                {
                    audioCaptureClient.Dispose();
                    audioCaptureClient = null;
                }
                Marshal.ReleaseComObject(audioClientInterface);
                audioClientInterface = null;
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}