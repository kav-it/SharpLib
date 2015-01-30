using System;
using System.Runtime.InteropServices;
using System.Threading;

using SharpLib.Audio.CoreAudioApi;

namespace SharpLib.Audio.Wave
{
    internal class WasapiOut : IWavePlayer, IWavePosition
    {
        #region Поля

        private readonly bool isUsingEventSync;

        private readonly MMDevice mmDevice;

        private readonly AudioClientShareMode shareMode;

        private readonly SynchronizationContext syncContext;

        private AudioClient audioClient;

        private int bufferFrameCount;

        private int bytesPerFrame;

        private bool dmoResamplerNeeded;

        private EventWaitHandle frameEventWaitHandle;

        private int latencyMilliseconds;

        private WaveFormat outputFormat;

        private Thread playThread;

        private volatile PlaybackState playbackState;

        private byte[] readBuffer;

        private AudioRenderClient renderClient;

        private IWaveProvider sourceProvider;

        #endregion

        #region Свойства

        public WaveFormat OutputWaveFormat
        {
            get { return outputFormat; }
        }

        public PlaybackState PlaybackState
        {
            get { return playbackState; }
        }

        public float Volume
        {
            get { return mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Volume must be between 0.0 and 1.0");
                }
                if (value > 1)
                {
                    throw new ArgumentOutOfRangeException("value", "Volume must be between 0.0 and 1.0");
                }
                mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar = value;
            }
        }

        #endregion

        #region События

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        #endregion

        #region Конструктор

        public WasapiOut(AudioClientShareMode shareMode, int latency) :
            this(GetDefaultAudioEndpoint(), shareMode, true, latency)
        {
        }

        public WasapiOut(AudioClientShareMode shareMode, bool useEventSync, int latency) :
            this(GetDefaultAudioEndpoint(), shareMode, useEventSync, latency)
        {
        }

        public WasapiOut(MMDevice device, AudioClientShareMode shareMode, bool useEventSync, int latency)
        {
            audioClient = device.AudioClient;
            mmDevice = device;
            this.shareMode = shareMode;
            isUsingEventSync = useEventSync;
            latencyMilliseconds = latency;
            syncContext = SynchronizationContext.Current;
        }

        #endregion

        #region Методы

        private static MMDevice GetDefaultAudioEndpoint()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                throw new NotSupportedException("WASAPI supported only on Windows Vista and above");
            }
            var enumerator = new MMDeviceEnumerator();
            return enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
        }

        private void PlayThread()
        {
            ResamplerDmoStream resamplerDmoStream = null;
            IWaveProvider playbackProvider = sourceProvider;
            Exception exception = null;
            try
            {
                if (dmoResamplerNeeded)
                {
                    resamplerDmoStream = new ResamplerDmoStream(sourceProvider, outputFormat);
                    playbackProvider = resamplerDmoStream;
                }

                bufferFrameCount = audioClient.BufferSize;
                bytesPerFrame = outputFormat.Channels * outputFormat.BitsPerSample / 8;
                readBuffer = new byte[bufferFrameCount * bytesPerFrame];
                FillBuffer(playbackProvider, bufferFrameCount);

                var waitHandles = new WaitHandle[] { frameEventWaitHandle };

                audioClient.Start();

                while (playbackState != PlaybackState.Stopped)
                {
                    int indexHandle = 0;
                    if (isUsingEventSync)
                    {
                        indexHandle = WaitHandle.WaitAny(waitHandles, 3 * latencyMilliseconds, false);
                    }
                    else
                    {
                        Thread.Sleep(latencyMilliseconds / 2);
                    }

                    if (playbackState == PlaybackState.Playing && indexHandle != WaitHandle.WaitTimeout)
                    {
                        int numFramesPadding = 0;
                        if (isUsingEventSync)
                        {
                            numFramesPadding = (shareMode == AudioClientShareMode.Shared) ? audioClient.CurrentPadding : 0;
                        }
                        else
                        {
                            numFramesPadding = audioClient.CurrentPadding;
                        }
                        int numFramesAvailable = bufferFrameCount - numFramesPadding;
                        if (numFramesAvailable > 10)
                        {
                            FillBuffer(playbackProvider, numFramesAvailable);
                        }
                    }
                }
                Thread.Sleep(latencyMilliseconds / 2);
                audioClient.Stop();
                if (playbackState == PlaybackState.Stopped)
                {
                    audioClient.Reset();
                }
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                if (resamplerDmoStream != null)
                {
                    resamplerDmoStream.Dispose();
                }
                RaisePlaybackStopped(exception);
            }
        }

        private void RaisePlaybackStopped(Exception e)
        {
            var handler = PlaybackStopped;
            if (handler != null)
            {
                if (syncContext == null)
                {
                    handler(this, new StoppedEventArgs(e));
                }
                else
                {
                    syncContext.Post(state => handler(this, new StoppedEventArgs(e)), null);
                }
            }
        }

        private void FillBuffer(IWaveProvider playbackProvider, int frameCount)
        {
            IntPtr buffer = renderClient.GetBuffer(frameCount);
            int readLength = frameCount * bytesPerFrame;
            int read = playbackProvider.Read(readBuffer, 0, readLength);
            if (read == 0)
            {
                playbackState = PlaybackState.Stopped;
            }
            Marshal.Copy(readBuffer, 0, buffer, read);
            int actualFrameCount = read / bytesPerFrame;

            renderClient.ReleaseBuffer(actualFrameCount, AudioClientBufferFlags.None);
        }

        public long GetPosition()
        {
            if (playbackState == Wave.PlaybackState.Stopped)
            {
                return 0;
            }
            return (long)audioClient.AudioClockClient.AdjustedPosition;
        }

        public void Play()
        {
            if (playbackState != PlaybackState.Playing)
            {
                if (playbackState == PlaybackState.Stopped)
                {
                    playThread = new Thread(PlayThread);
                    playbackState = PlaybackState.Playing;
                    playThread.Start();
                }
                else
                {
                    playbackState = PlaybackState.Playing;
                }
            }
        }

        public void Stop()
        {
            if (playbackState != PlaybackState.Stopped)
            {
                playbackState = PlaybackState.Stopped;
                playThread.Join();
                playThread = null;
            }
        }

        public void Pause()
        {
            if (playbackState == PlaybackState.Playing)
            {
                playbackState = PlaybackState.Paused;
            }
        }

        public void Init(IWaveProvider waveProvider)
        {
            long latencyRefTimes = latencyMilliseconds * 10000;
            outputFormat = waveProvider.WaveFormat;

            WaveFormatExtensible closestSampleRateFormat;
            if (!audioClient.IsFormatSupported(shareMode, outputFormat, out closestSampleRateFormat))
            {
                if (closestSampleRateFormat == null)
                {
                    WaveFormat correctSampleRateFormat = audioClient.MixFormat;

                    if (!audioClient.IsFormatSupported(shareMode, correctSampleRateFormat))
                    {
                        WaveFormatExtensible[] bestToWorstFormats =
                        {
                            new WaveFormatExtensible(
                                outputFormat.SampleRate, 32,
                                outputFormat.Channels),
                            new WaveFormatExtensible(
                                outputFormat.SampleRate, 24,
                                outputFormat.Channels),
                            new WaveFormatExtensible(
                                outputFormat.SampleRate, 16,
                                outputFormat.Channels)
                        };

                        for (int i = 0; i < bestToWorstFormats.Length; i++)
                        {
                            correctSampleRateFormat = bestToWorstFormats[i];
                            if (audioClient.IsFormatSupported(shareMode, correctSampleRateFormat))
                            {
                                break;
                            }
                            correctSampleRateFormat = null;
                        }

                        if (correctSampleRateFormat == null)
                        {
                            correctSampleRateFormat = new WaveFormatExtensible(outputFormat.SampleRate, 16, 2);
                            if (!audioClient.IsFormatSupported(shareMode, correctSampleRateFormat))
                            {
                                throw new NotSupportedException("Can't find a supported format to use");
                            }
                        }
                    }
                    outputFormat = correctSampleRateFormat;
                }
                else
                {
                    outputFormat = closestSampleRateFormat;
                }

                using (new ResamplerDmoStream(waveProvider, outputFormat))
                {
                }
                dmoResamplerNeeded = true;
            }
            else
            {
                dmoResamplerNeeded = false;
            }
            sourceProvider = waveProvider;

            if (isUsingEventSync)
            {
                if (shareMode == AudioClientShareMode.Shared)
                {
                    audioClient.Initialize(shareMode, AudioClientStreamFlags.EventCallback, 0, 0,
                        outputFormat, Guid.Empty);

                    latencyMilliseconds = (int)(audioClient.StreamLatency / 10000);
                }
                else
                {
                    audioClient.Initialize(shareMode, AudioClientStreamFlags.EventCallback, latencyRefTimes, latencyRefTimes,
                        outputFormat, Guid.Empty);
                }

                frameEventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
                audioClient.SetEventHandle(frameEventWaitHandle.SafeWaitHandle.DangerousGetHandle());
            }
            else
            {
                audioClient.Initialize(shareMode, AudioClientStreamFlags.None, latencyRefTimes, 0,
                    outputFormat, Guid.Empty);
            }

            renderClient = audioClient.AudioRenderClient;
        }

        public void Dispose()
        {
            if (audioClient != null)
            {
                Stop();

                audioClient.Dispose();
                audioClient = null;
                renderClient = null;
            }
        }

        #endregion
    }
}