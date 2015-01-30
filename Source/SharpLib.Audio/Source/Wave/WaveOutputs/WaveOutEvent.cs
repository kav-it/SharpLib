using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace SharpLib.Audio.Wave
{
    internal class WaveOutEvent : IWavePlayer, IWavePosition
    {
        #region Поля

        private readonly SynchronizationContext _syncContext;

        private readonly object _waveOutLock;

        private WaveOutBuffer[] _buffers;

        private AutoResetEvent _callbackEvent;

        private IntPtr _hWaveOut;

        private volatile PlaybackState _playbackState;

        private float _volume;

        private IWaveProvider _waveStream;

        #endregion

        #region Свойства

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

        [Obsolete]
        public float Volume
        {
            get { return _volume; }
            set
            {
                WaveOut.SetWaveOutVolume(value, _hWaveOut, _waveOutLock);
                _volume = value;
            }
        }

        #endregion

        #region События

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        #endregion

        #region Конструктор

        public WaveOutEvent()
        {
            _volume = 1.0f;
            _syncContext = SynchronizationContext.Current;
            if (_syncContext != null &&
                ((_syncContext.GetType().Name == "LegacyAspNetSynchronizationContext") ||
                 (_syncContext.GetType().Name == "AspNetSynchronizationContext")))
            {
                _syncContext = null;
            }

            DeviceNumber = 0;
            DesiredLatency = 300;
            NumberOfBuffers = 2;

            _waveOutLock = new object();
        }

        ~WaveOutEvent()
        {
            Debug.Assert(false, "WaveOutEvent device was not closed");
            Dispose(false);
        }

        #endregion

        #region Методы

        public void Init(IWaveProvider waveProvider)
        {
            if (_playbackState != PlaybackState.Stopped)
            {
                throw new InvalidOperationException("Can't re-initialize during playback");
            }
            if (_hWaveOut != IntPtr.Zero)
            {
                DisposeBuffers();
                CloseWaveOut();
            }

            _callbackEvent = new AutoResetEvent(false);

            _waveStream = waveProvider;
            int bufferSize = waveProvider.WaveFormat.ConvertLatencyToByteSize((DesiredLatency + NumberOfBuffers - 1) / NumberOfBuffers);

            MmResult result;
            lock (_waveOutLock)
            {
                result = WaveInterop.waveOutOpenWindow(out _hWaveOut, (IntPtr)DeviceNumber, _waveStream.WaveFormat, _callbackEvent.SafeWaitHandle.DangerousGetHandle(), IntPtr.Zero,
                    WaveInterop.WaveInOutOpenFlags.CallbackEvent);
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
            if (_buffers == null || _waveStream == null)
            {
                throw new InvalidOperationException("Must call Init first");
            }
            if (_playbackState == PlaybackState.Stopped)
            {
                _playbackState = PlaybackState.Playing;
                _callbackEvent.Set();
                ThreadPool.QueueUserWorkItem(state => PlaybackThread(), null);
            }
            else if (_playbackState == PlaybackState.Paused)
            {
                Resume();
                _callbackEvent.Set();
            }
        }

        private void PlaybackThread()
        {
            Exception exception = null;
            try
            {
                DoPlayback();
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                _playbackState = PlaybackState.Stopped;

                RaisePlaybackStoppedEvent(exception);
            }
        }

        private void DoPlayback()
        {
            while (_playbackState != PlaybackState.Stopped)
            {
                if (!_callbackEvent.WaitOne(DesiredLatency))
                {
                    Debug.WriteLine("WARNING: WaveOutEvent callback event timeout");
                }

                if (_playbackState == PlaybackState.Playing)
                {
                    int queued = 0;
                    foreach (var buffer in _buffers)
                    {
                        if (buffer.InQueue || buffer.OnDone())
                        {
                            queued++;
                        }
                    }
                    if (queued == 0)
                    {
                        _playbackState = PlaybackState.Stopped;
                        _callbackEvent.Set();
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

        private void Resume()
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
                _callbackEvent.Set();
            }
        }

        public long GetPosition()
        {
            lock (_waveOutLock)
            {
                var mmTime = new MmTime();
                mmTime.wType = MmTime.TIME_BYTES;
                MmException.Try(WaveInterop.waveOutGetPosition(_hWaveOut, out mmTime, Marshal.SizeOf(mmTime)), "waveOutGetPosition");

                if (mmTime.wType != MmTime.TIME_BYTES)
                {
                    throw new Exception(string.Format("waveOutGetPosition: wType -> Expected {0}, Received {1}", MmTime.TIME_BYTES, mmTime.wType));
                }

                return mmTime.cb;
            }
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
                DisposeBuffers();
            }

            CloseWaveOut();
        }

        private void CloseWaveOut()
        {
            if (_callbackEvent != null)
            {
                _callbackEvent.Close();
                _callbackEvent = null;
            }
            lock (_waveOutLock)
            {
                if (_hWaveOut != IntPtr.Zero)
                {
                    WaveInterop.waveOutClose(_hWaveOut);
                    _hWaveOut = IntPtr.Zero;
                }
            }
        }

        private void DisposeBuffers()
        {
            if (_buffers != null)
            {
                foreach (var buffer in _buffers)
                {
                    buffer.Dispose();
                }
                _buffers = null;
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