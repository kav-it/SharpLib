using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using SharpLib.Audio.Wave;

namespace SharpLib.Audio.CoreAudioApi
{
    internal class WasapiCapture : IWaveIn
    {
        #region Константы

        private const long REFTIMES_PER_MILLISEC = 10000;

        private const long REFTIMES_PER_SEC = 10000000;

        #endregion

        #region Поля

        private readonly SynchronizationContext syncContext;

        private AudioClient audioClient;

        private int bytesPerFrame;

        private Thread captureThread;

        private bool initialized;

        private byte[] recordBuffer;

        private volatile bool requestStop;

        private WaveFormat waveFormat;

        #endregion

        #region Свойства

        public AudioClientShareMode ShareMode { get; set; }

        public virtual WaveFormat WaveFormat
        {
            get
            {
                var wfe = waveFormat as WaveFormatExtensible;
                if (wfe != null)
                {
                    try
                    {
                        return wfe.ToStandardWaveFormat();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
                return waveFormat;
            }
            set { waveFormat = value; }
        }

        #endregion

        #region События

        public event EventHandler<WaveInEventArgs> DataAvailable;

        public event EventHandler<StoppedEventArgs> RecordingStopped;

        #endregion

        #region Конструктор

        public WasapiCapture() :
            this(GetDefaultCaptureDevice())
        {
        }

        public WasapiCapture(MMDevice captureDevice)
        {
            syncContext = SynchronizationContext.Current;
            audioClient = captureDevice.AudioClient;
            ShareMode = AudioClientShareMode.Shared;

            waveFormat = audioClient.MixFormat;
        }

        #endregion

        #region Методы

        public static MMDevice GetDefaultCaptureDevice()
        {
            var devices = new MMDeviceEnumerator();
            return devices.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
        }

        private void InitializeCaptureDevice()
        {
            if (initialized)
            {
                return;
            }

            long requestedDuration = REFTIMES_PER_MILLISEC * 100;

            if (!audioClient.IsFormatSupported(ShareMode, waveFormat))
            {
                throw new ArgumentException("Unsupported Wave Format");
            }

            var streamFlags = GetAudioClientStreamFlags();

            audioClient.Initialize(ShareMode,
                streamFlags,
                requestedDuration,
                0,
                waveFormat,
                Guid.Empty);

            int bufferFrameCount = audioClient.BufferSize;
            bytesPerFrame = waveFormat.Channels * waveFormat.BitsPerSample / 8;
            recordBuffer = new byte[bufferFrameCount * bytesPerFrame];
            Debug.WriteLine("record buffer size = {0}", recordBuffer.Length);

            initialized = true;
        }

        protected virtual AudioClientStreamFlags GetAudioClientStreamFlags()
        {
            return AudioClientStreamFlags.None;
        }

        public void StartRecording()
        {
            if (captureThread != null)
            {
                throw new InvalidOperationException("Previous recording still in progress");
            }
            InitializeCaptureDevice();
            ThreadStart start = () => CaptureThread(audioClient);
            captureThread = new Thread(start);

            Debug.WriteLine("Thread starting...");
            requestStop = false;
            captureThread.Start();
        }

        public void StopRecording()
        {
            requestStop = true;
        }

        private void CaptureThread(AudioClient client)
        {
            Exception exception = null;
            try
            {
                DoRecording(client);
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                client.Stop();
            }
            captureThread = null;
            RaiseRecordingStopped(exception);
            Debug.WriteLine("Stop wasapi");
        }

        private void DoRecording(AudioClient client)
        {
            Debug.WriteLine("Client buffer frame count: {0}", client.BufferSize);
            int bufferFrameCount = client.BufferSize;

            long actualDuration = (long)((double)REFTIMES_PER_SEC *
                                         bufferFrameCount / waveFormat.SampleRate);
            int sleepMilliseconds = (int)(actualDuration / REFTIMES_PER_MILLISEC / 2);

            AudioCaptureClient capture = client.AudioCaptureClient;
            client.Start();
            Debug.WriteLine("sleep: {0} ms", sleepMilliseconds);
            while (!requestStop)
            {
                Thread.Sleep(sleepMilliseconds);
                ReadNextPacket(capture);
            }
        }

        private void RaiseRecordingStopped(Exception e)
        {
            var handler = RecordingStopped;
            if (handler == null)
            {
                return;
            }
            if (syncContext == null)
            {
                handler(this, new StoppedEventArgs(e));
            }
            else
            {
                syncContext.Post(state => handler(this, new StoppedEventArgs(e)), null);
            }
        }

        private void ReadNextPacket(AudioCaptureClient capture)
        {
            int packetSize = capture.GetNextPacketSize();
            int recordBufferOffset = 0;

            while (packetSize != 0)
            {
                int framesAvailable;
                AudioClientBufferFlags flags;
                IntPtr buffer = capture.GetBuffer(out framesAvailable, out flags);

                int bytesAvailable = framesAvailable * bytesPerFrame;

                int spaceRemaining = Math.Max(0, recordBuffer.Length - recordBufferOffset);
                if (spaceRemaining < bytesAvailable && recordBufferOffset > 0)
                {
                    if (DataAvailable != null)
                    {
                        DataAvailable(this, new WaveInEventArgs(recordBuffer, recordBufferOffset));
                    }
                    recordBufferOffset = 0;
                }

                if ((flags & AudioClientBufferFlags.Silent) != AudioClientBufferFlags.Silent)
                {
                    Marshal.Copy(buffer, recordBuffer, recordBufferOffset, bytesAvailable);
                }
                else
                {
                    Array.Clear(recordBuffer, recordBufferOffset, bytesAvailable);
                }
                recordBufferOffset += bytesAvailable;
                capture.ReleaseBuffer(framesAvailable);
                packetSize = capture.GetNextPacketSize();
            }
            if (DataAvailable != null)
            {
                DataAvailable(this, new WaveInEventArgs(recordBuffer, recordBufferOffset));
            }
        }

        public void Dispose()
        {
            StopRecording();
            if (captureThread != null)
            {
                captureThread.Join();
                captureThread = null;
            }
            if (audioClient != null)
            {
                audioClient.Dispose();
                audioClient = null;
            }
        }

        #endregion
    }
}