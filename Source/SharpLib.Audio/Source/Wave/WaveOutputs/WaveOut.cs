using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace NAudio.Wave
{
    internal class WaveOut : IWavePlayer, IWavePosition
    {
        #region Поля

        private readonly WaveInterop.WaveCallback _callback;

        private readonly WaveCallbackInfo _callbackInfo;

        private readonly SynchronizationContext _syncContext;

        private readonly object _waveOutLock;

        private WaveOutBuffer[] _buffers;

        private IntPtr _hWaveOut;

        private volatile PlaybackState _playbackState;

        private int _queuedBuffers;

        private float _volume;

        private IWaveProvider _waveStream;

        #endregion

        #region Свойства

        public static Int32 DeviceCount
        {
            get { return WaveInterop.waveOutGetNumDevs(); }
        }

        public int DesiredLatency { get; set; }

        public int NumberOfBuffers { get; set; }

        public int DeviceNumber { get; set; }

        public WaveFormat OutputWaveFormat
        {
            get { return _waveStream.WaveFormat; }
        }

        public PlaybackState PlaybackState
        {
            get { return _playbackState; }
        }

        public float Volume
        {
            get { return _volume; }
            set
            {
                SetWaveOutVolume(value, _hWaveOut, _waveOutLock);
                _volume = value;
            }
        }

        #endregion

        #region События

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        #endregion

        #region Конструктор

        public WaveOut()
            : this(WaveCallbackInfo.FunctionCallback())
        {
        }

        public WaveOut(IntPtr windowHandle)
            : this(WaveCallbackInfo.ExistingWindow(windowHandle))
        {
        }

        public WaveOut(WaveCallbackInfo callbackInfo)
        {
            _volume = 1;
            _syncContext = SynchronizationContext.Current;

            DeviceNumber = 0;
            DesiredLatency = 300;
            NumberOfBuffers = 2;

            _callback = Callback;
            _waveOutLock = new object();
            _callbackInfo = callbackInfo;
            callbackInfo.Connect(_callback);
        }

        ~WaveOut()
        {
            Debug.Assert(false, "WaveOut device was not closed");
            Dispose(false);
        }

        #endregion

        #region Методы

        public static WaveOutCapabilities GetCapabilities(int devNumber)
        {
            WaveOutCapabilities caps = new WaveOutCapabilities();
            int structSize = Marshal.SizeOf(caps);
            MmException.Try(WaveInterop.waveOutGetDevCaps((IntPtr)devNumber, out caps, structSize), "waveOutGetDevCaps");
            return caps;
        }

        public void Init(IWaveProvider waveProvider)
        {
            _waveStream = waveProvider;
            int bufferSize = waveProvider.WaveFormat.ConvertLatencyToByteSize((DesiredLatency + NumberOfBuffers - 1) / NumberOfBuffers);

            MmResult result;
            lock (_waveOutLock)
            {
                result = _callbackInfo.WaveOutOpen(out _hWaveOut, DeviceNumber, _waveStream.WaveFormat, _callback);
            }
            MmException.Try(result, "waveOutOpen");

            _buffers = new WaveOutBuffer[NumberOfBuffers];
            _playbackState = PlaybackState.Stopped;
            for (int n = 0; n < NumberOfBuffers; n++)
            {
                _buffers[n] = new WaveOutBuffer(_hWaveOut, bufferSize, _waveStream, _waveOutLock);
            }
        }

        public void Play()
        {
            if (_playbackState == PlaybackState.Stopped)
            {
                _playbackState = PlaybackState.Playing;
                Debug.Assert(_queuedBuffers == 0, "Buffers already queued on play");
                EnqueueBuffers();
            }
            else if (_playbackState == PlaybackState.Paused)
            {
                EnqueueBuffers();
                Resume();
                _playbackState = PlaybackState.Playing;
            }
        }

        private void EnqueueBuffers()
        {
            for (int n = 0; n < NumberOfBuffers; n++)
            {
                if (!_buffers[n].InQueue)
                {
                    if (_buffers[n].OnDone())
                    {
                        Interlocked.Increment(ref _queuedBuffers);
                    }
                    else
                    {
                        _playbackState = PlaybackState.Stopped;
                        break;
                    }
                }
            }
        }

        public void Pause()
        {
            if (_playbackState == PlaybackState.Playing)
            {
                MmResult result;
                lock (_waveOutLock)
                {
                    result = WaveInterop.waveOutPause(_hWaveOut);
                }
                if (result != MmResult.NoError)
                {
                    throw new MmException(result, "waveOutPause");
                }
                _playbackState = PlaybackState.Paused;
            }
        }

        public void Resume()
        {
            if (_playbackState == PlaybackState.Paused)
            {
                MmResult result;
                lock (_waveOutLock)
                {
                    result = WaveInterop.waveOutRestart(_hWaveOut);
                }
                if (result != MmResult.NoError)
                {
                    throw new MmException(result, "waveOutRestart");
                }
                _playbackState = PlaybackState.Playing;
            }
        }

        public void Stop()
        {
            if (_playbackState != PlaybackState.Stopped)
            {
                _playbackState = PlaybackState.Stopped;
                MmResult result;
                lock (_waveOutLock)
                {
                    result = WaveInterop.waveOutReset(_hWaveOut);
                }
                if (result != MmResult.NoError)
                {
                    throw new MmException(result, "waveOutReset");
                }

                if (_callbackInfo.Strategy == WaveCallbackStrategy.FunctionCallback)
                {
                    RaisePlaybackStoppedEvent(null);
                }
            }
        }

        public long GetPosition()
        {
            lock (_waveOutLock)
            {
                MmTime mmTime = new MmTime();
                mmTime.wType = MmTime.TIME_BYTES;
                MmException.Try(WaveInterop.waveOutGetPosition(_hWaveOut, out mmTime, Marshal.SizeOf(mmTime)), "waveOutGetPosition");

                if (mmTime.wType != MmTime.TIME_BYTES)
                {
                    throw new Exception(string.Format("waveOutGetPosition: wType -> Expected {0}, Received {1}", MmTime.TIME_BYTES, mmTime.wType));
                }

                return mmTime.cb;
            }
        }

        internal static void SetWaveOutVolume(float value, IntPtr hWaveOut, object lockObject)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value", "Volume must be between 0.0 and 1.0");
            }
            if (value > 1)
            {
                throw new ArgumentOutOfRangeException("value", "Volume must be between 0.0 and 1.0");
            }
            float left = value;
            float right = value;

            int stereoVolume = (int)(left * 0xFFFF) + ((int)(right * 0xFFFF) << 16);
            MmResult result;
            lock (lockObject)
            {
                result = WaveInterop.waveOutSetVolume(hWaveOut, stereoVolume);
            }
            MmException.Try(result, "waveOutSetVolume");
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            Stop();

            if (disposing)
            {
                if (_buffers != null)
                {
                    for (int n = 0; n < _buffers.Length; n++)
                    {
                        if (_buffers[n] != null)
                        {
                            _buffers[n].Dispose();
                        }
                    }
                    _buffers = null;
                }
            }

            lock (_waveOutLock)
            {
                WaveInterop.waveOutClose(_hWaveOut);
            }
            if (disposing)
            {
                _callbackInfo.Disconnect();
            }
        }

        private void Callback(IntPtr hWaveOut, WaveInterop.WaveMessage uMsg, IntPtr dwInstance, WaveHeader wavhdr, IntPtr dwReserved)
        {
            if (uMsg == WaveInterop.WaveMessage.WaveOutDone)
            {
                GCHandle hBuffer = (GCHandle)wavhdr.userData;
                WaveOutBuffer buffer = (WaveOutBuffer)hBuffer.Target;
                Interlocked.Decrement(ref _queuedBuffers);
                Exception exception = null;

                if (PlaybackState == PlaybackState.Playing)
                {
                    lock (_waveOutLock)
                    {
                        try
                        {
                            if (buffer.OnDone())
                            {
                                Interlocked.Increment(ref _queuedBuffers);
                            }
                        }
                        catch (Exception e)
                        {
                            exception = e;
                        }
                    }
                }
                if (_queuedBuffers == 0)
                {
                    if (_callbackInfo.Strategy == WaveCallbackStrategy.FunctionCallback && _playbackState == Wave.PlaybackState.Stopped)
                    {
                    }
                    else
                    {
                        _playbackState = PlaybackState.Stopped;
                        RaisePlaybackStoppedEvent(exception);
                    }
                }
            }
        }

        private void RaisePlaybackStoppedEvent(Exception e)
        {
            var handler = PlaybackStopped;
            if (handler != null)
            {
                if (_syncContext == null)
                {
                    handler(this, new StoppedEventArgs(e));
                }
                else
                {
                    _syncContext.Post(state => handler(this, new StoppedEventArgs(e)), null);
                }
            }
        }

        #endregion
    }
}