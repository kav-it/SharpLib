using System;
using System.Runtime.InteropServices;
using System.Threading;

using SharpLib.Audio.Mixer;

namespace SharpLib.Audio.Wave
{
    internal class WaveInEvent : IWaveIn
    {
        #region Поля

        private readonly AutoResetEvent callbackEvent;

        private readonly SynchronizationContext syncContext;

        private WaveInBuffer[] buffers;

        private volatile bool recording;

        private IntPtr waveInHandle;

        #endregion

        #region Свойства

        public static int DeviceCount
        {
            get { return WaveInterop.waveInGetNumDevs(); }
        }

        public int BufferMilliseconds { get; set; }

        public int NumberOfBuffers { get; set; }

        public int DeviceNumber { get; set; }

        public WaveFormat WaveFormat { get; set; }

        #endregion

        #region События

        public event EventHandler<WaveInEventArgs> DataAvailable;

        public event EventHandler<StoppedEventArgs> RecordingStopped;

        #endregion

        #region Конструктор

        public WaveInEvent()
        {
            callbackEvent = new AutoResetEvent(false);
            syncContext = SynchronizationContext.Current;
            DeviceNumber = 0;
            WaveFormat = new WaveFormat(8000, 16, 1);
            BufferMilliseconds = 100;
            NumberOfBuffers = 3;
        }

        #endregion

        #region Методы

        public static WaveInCapabilities GetCapabilities(int devNumber)
        {
            WaveInCapabilities caps = new WaveInCapabilities();
            int structSize = Marshal.SizeOf(caps);
            MmException.Try(WaveInterop.waveInGetDevCaps((IntPtr)devNumber, out caps, structSize), "waveInGetDevCaps");
            return caps;
        }

        private void CreateBuffers()
        {
            int bufferSize = BufferMilliseconds * WaveFormat.AverageBytesPerSecond / 1000;
            if (bufferSize % WaveFormat.BlockAlign != 0)
            {
                bufferSize -= bufferSize % WaveFormat.BlockAlign;
            }

            buffers = new WaveInBuffer[NumberOfBuffers];
            for (int n = 0; n < buffers.Length; n++)
            {
                buffers[n] = new WaveInBuffer(waveInHandle, bufferSize);
            }
        }

        private void OpenWaveInDevice()
        {
            CloseWaveInDevice();
            MmResult result = WaveInterop.waveInOpenWindow(out waveInHandle, (IntPtr)DeviceNumber, WaveFormat,
                callbackEvent.SafeWaitHandle.DangerousGetHandle(), IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackEvent);
            MmException.Try(result, "waveInOpen");
            CreateBuffers();
        }

        public void StartRecording()
        {
            if (recording)
            {
                throw new InvalidOperationException("Already recording");
            }
            OpenWaveInDevice();
            MmException.Try(WaveInterop.waveInStart(waveInHandle), "waveInStart");
            recording = true;
            ThreadPool.QueueUserWorkItem(state => RecordThread(), null);
        }

        private void RecordThread()
        {
            Exception exception = null;
            try
            {
                DoRecording();
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                recording = false;
                RaiseRecordingStoppedEvent(exception);
            }
        }

        private void DoRecording()
        {
            foreach (var buffer in buffers)
            {
                if (!buffer.InQueue)
                {
                    buffer.Reuse();
                }
            }
            while (recording)
            {
                if (callbackEvent.WaitOne())
                {
                    if (recording)
                    {
                        foreach (var buffer in buffers)
                        {
                            if (buffer.Done)
                            {
                                if (DataAvailable != null)
                                {
                                    DataAvailable(this, new WaveInEventArgs(buffer.Data, buffer.BytesRecorded));
                                }
                                buffer.Reuse();
                            }
                        }
                    }
                }
            }
        }

        private void RaiseRecordingStoppedEvent(Exception e)
        {
            var handler = RecordingStopped;
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

        public void StopRecording()
        {
            recording = false;
            callbackEvent.Set();
            MmException.Try(WaveInterop.waveInStop(waveInHandle), "waveInStop");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (recording)
                {
                    StopRecording();
                }

                CloseWaveInDevice();
            }
        }

        private void CloseWaveInDevice()
        {
            WaveInterop.waveInReset(waveInHandle);
            if (buffers != null)
            {
                for (int n = 0; n < buffers.Length; n++)
                {
                    buffers[n].Dispose();
                }
                buffers = null;
            }
            WaveInterop.waveInClose(waveInHandle);
            waveInHandle = IntPtr.Zero;
        }

        public MixerLine GetMixerLine()
        {
            MixerLine mixerLine;
            if (waveInHandle != IntPtr.Zero)
            {
                mixerLine = new MixerLine(waveInHandle, 0, MixerFlags.WaveInHandle);
            }
            else
            {
                mixerLine = new MixerLine((IntPtr)DeviceNumber, 0, MixerFlags.WaveIn);
            }
            return mixerLine;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}