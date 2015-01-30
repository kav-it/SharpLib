using System;
using System.Runtime.InteropServices;
using System.Threading;

using NAudio.Mixer;

namespace NAudio.Wave
{
    internal class WaveIn : IWaveIn
    {
        #region Поля

        private readonly WaveInterop.WaveCallback callback;

        private readonly SynchronizationContext syncContext;

        private WaveInBuffer[] buffers;

        private WaveCallbackInfo callbackInfo;

        private int lastReturnedBufferIndex;

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

        public WaveIn()
            : this(WaveCallbackInfo.NewWindow())
        {
        }

        public WaveIn(IntPtr windowHandle)
            : this(WaveCallbackInfo.ExistingWindow(windowHandle))
        {
        }

        public WaveIn(WaveCallbackInfo callbackInfo)
        {
            syncContext = SynchronizationContext.Current;
            if ((callbackInfo.Strategy == WaveCallbackStrategy.NewWindow || callbackInfo.Strategy == WaveCallbackStrategy.ExistingWindow) &&
                syncContext == null)
            {
                throw new InvalidOperationException("Use WaveInEvent to record on a background thread");
            }
            DeviceNumber = 0;
            WaveFormat = new WaveFormat(8000, 16, 1);
            BufferMilliseconds = 100;
            NumberOfBuffers = 3;
            callback = Callback;
            this.callbackInfo = callbackInfo;
            callbackInfo.Connect(callback);
        }

        #endregion

        #region Методы

        public static WaveInCapabilities GetCapabilities(int devNumber)
        {
            var caps = new WaveInCapabilities();
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

        private void Callback(IntPtr waveInHandle, WaveInterop.WaveMessage message, IntPtr userData, WaveHeader waveHeader, IntPtr reserved)
        {
            if (message == WaveInterop.WaveMessage.WaveInData)
            {
                if (recording)
                {
                    var hBuffer = (GCHandle)waveHeader.userData;
                    var buffer = (WaveInBuffer)hBuffer.Target;
                    if (buffer == null)
                    {
                        return;
                    }

                    lastReturnedBufferIndex = Array.IndexOf(buffers, buffer);
                    RaiseDataAvailable(buffer);
                    try
                    {
                        buffer.Reuse();
                    }
                    catch (Exception e)
                    {
                        recording = false;
                        RaiseRecordingStopped(e);
                    }
                }
            }
        }

        private void RaiseDataAvailable(WaveInBuffer buffer)
        {
            var handler = DataAvailable;
            if (handler != null)
            {
                handler(this, new WaveInEventArgs(buffer.Data, buffer.BytesRecorded));
            }
        }

        private void RaiseRecordingStopped(Exception e)
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

        private void OpenWaveInDevice()
        {
            CloseWaveInDevice();
            MmResult result = callbackInfo.WaveInOpen(out waveInHandle, DeviceNumber, WaveFormat, callback);
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
            EnqueueBuffers();
            MmException.Try(WaveInterop.waveInStart(waveInHandle), "waveInStart");
            recording = true;
        }

        private void EnqueueBuffers()
        {
            foreach (var buffer in buffers)
            {
                if (!buffer.InQueue)
                {
                    buffer.Reuse();
                }
            }
        }

        public void StopRecording()
        {
            if (recording)
            {
                recording = false;
                MmException.Try(WaveInterop.waveInStop(waveInHandle), "waveInStop");

                for (int n = 0; n < buffers.Length; n++)
                {
                    int index = (n + lastReturnedBufferIndex + 1) % buffers.Length;
                    var buffer = buffers[index];
                    if (buffer.Done)
                    {
                        RaiseDataAvailable(buffer);
                    }
                }
                RaiseRecordingStopped(null);
            }
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
                if (callbackInfo != null)
                {
                    callbackInfo.Disconnect();
                    callbackInfo = null;
                }
            }
        }

        private void CloseWaveInDevice()
        {
            if (waveInHandle == IntPtr.Zero)
            {
                return;
            }

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