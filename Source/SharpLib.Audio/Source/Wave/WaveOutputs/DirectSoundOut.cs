using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace SharpLib.Audio.Wave
{
    internal class DirectSoundOut : IWavePlayer
    {
        #region Делегаты

        private delegate bool DSEnumCallback(IntPtr lpGuid, IntPtr lpcstrDescription, IntPtr lpcstrModule, IntPtr lpContext);

        #endregion

        #region Перечисления

        [Flags]
        internal enum DirectSoundBufferCaps : uint
        {
            DSBCAPS_PRIMARYBUFFER = 0x00000001,

            DSBCAPS_STATIC = 0x00000002,

            DSBCAPS_LOCHARDWARE = 0x00000004,

            DSBCAPS_LOCSOFTWARE = 0x00000008,

            DSBCAPS_CTRL3D = 0x00000010,

            DSBCAPS_CTRLFREQUENCY = 0x00000020,

            DSBCAPS_CTRLPAN = 0x00000040,

            DSBCAPS_CTRLVOLUME = 0x00000080,

            DSBCAPS_CTRLPOSITIONNOTIFY = 0x00000100,

            DSBCAPS_CTRLFX = 0x00000200,

            DSBCAPS_STICKYFOCUS = 0x00004000,

            DSBCAPS_GLOBALFOCUS = 0x00008000,

            DSBCAPS_GETCURRENTPOSITION2 = 0x00010000,

            DSBCAPS_MUTE3DATMAXDISTANCE = 0x00020000,

            DSBCAPS_LOCDEFER = 0x00040000
        }

        internal enum DirectSoundBufferLockFlag : uint
        {
            None = 0,

            FromWriteCursor = 0x00000001,

            EntireBuffer = 0x00000002
        }

        [Flags]
        internal enum DirectSoundBufferStatus : uint
        {
            DSBSTATUS_PLAYING = 0x00000001,

            DSBSTATUS_BUFFERLOST = 0x00000002,

            DSBSTATUS_LOOPING = 0x00000004,

            DSBSTATUS_LOCHARDWARE = 0x00000008,

            DSBSTATUS_LOCSOFTWARE = 0x00000010,

            DSBSTATUS_TERMINATED = 0x00000020
        }

        internal enum DirectSoundCooperativeLevel : uint
        {
            DSSCL_NORMAL = 0x00000001,

            DSSCL_PRIORITY = 0x00000002,

            DSSCL_EXCLUSIVE = 0x00000003,

            DSSCL_WRITEPRIMARY = 0x00000004
        }

        [Flags]
        internal enum DirectSoundPlayFlags : uint
        {
            DSBPLAY_LOOPING = 0x00000001,

            DSBPLAY_LOCHARDWARE = 0x00000002,

            DSBPLAY_LOCSOFTWARE = 0x00000004,

            DSBPLAY_TERMINATEBY_TIME = 0x00000008,

            DSBPLAY_TERMINATEBY_DISTANCE = 0x000000010,

            DSBPLAY_TERMINATEBY_PRIORITY = 0x000000020
        }

        #endregion

        #region Поля

        public static readonly Guid DSDEVID_DefaultCapture = new Guid("DEF00001-9C6D-47ED-AAF1-4DDA8F2B5C03");

        public static readonly Guid DSDEVID_DefaultPlayback = new Guid("DEF00000-9C6D-47ED-AAF1-4DDA8F2B5C03");

        public static readonly Guid DSDEVID_DefaultVoiceCapture = new Guid("DEF00003-9C6D-47ED-AAF1-4DDA8F2B5C03");

        public static readonly Guid DSDEVID_DefaultVoicePlayback = new Guid("DEF00002-9C6D-47ED-AAF1-4DDA8F2B5C03");

        private static List<DirectSoundDeviceInfo> devices;

        private readonly int desiredLatency;

        private readonly Object m_LockObject = new Object();

        private readonly SynchronizationContext syncContext;

        private long bytesPlayed;

        private Guid device;

        private IDirectSound directSound;

        private EventWaitHandle endEventWaitHandle;

        private EventWaitHandle frameEventWaitHandle1;

        private EventWaitHandle frameEventWaitHandle2;

        private int nextSamplesWriteIndex;

        private Thread notifyThread;

        private PlaybackState playbackState;

        private IDirectSoundBuffer primarySoundBuffer;

        private byte[] samples;

        private int samplesFrameSize;

        private int samplesTotalSize;

        private IDirectSoundBuffer secondaryBuffer;

        private WaveFormat waveFormat;

        private IWaveProvider waveStream;

        #endregion

        #region Свойства

        public static IEnumerable<DirectSoundDeviceInfo> Devices
        {
            get
            {
                devices = new List<DirectSoundDeviceInfo>();
                DirectSoundEnumerate(EnumCallback, IntPtr.Zero);
                return devices;
            }
        }

        public TimeSpan PlaybackPosition
        {
            get
            {
                var pos = GetPosition();

                pos /= waveFormat.Channels * waveFormat.BitsPerSample / 8;

                return TimeSpan.FromMilliseconds(pos * 1000.0 / waveFormat.SampleRate);
            }
        }

        public PlaybackState PlaybackState
        {
            get { return playbackState; }
        }

        public float Volume
        {
            get { return 1.0f; }
            set
            {
                if (value != 1.0f)
                {
                    throw new InvalidOperationException("Setting volume not supported on DirectSoundOut, adjust the volume on your WaveProvider instead");
                }
            }
        }

        #endregion

        #region События

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        #endregion

        #region Конструктор

        public DirectSoundOut()
            : this(DSDEVID_DefaultPlayback)
        {
        }

        public DirectSoundOut(Guid device)
            : this(device, 40)
        {
        }

        public DirectSoundOut(int latency)
            : this(DSDEVID_DefaultPlayback, latency)
        {
        }

        public DirectSoundOut(Guid device, int latency)
        {
            if (device == Guid.Empty)
            {
                device = DSDEVID_DefaultPlayback;
            }
            this.device = device;
            desiredLatency = latency;
            syncContext = SynchronizationContext.Current;
        }

        ~DirectSoundOut()
        {
            Dispose();
        }

        #endregion

        #region Методы

        private static bool EnumCallback(IntPtr lpGuid, IntPtr lpcstrDescription, IntPtr lpcstrModule, IntPtr lpContext)
        {
            var device = new DirectSoundDeviceInfo();
            if (lpGuid == IntPtr.Zero)
            {
                device.Guid = Guid.Empty;
            }
            else
            {
                byte[] guidBytes = new byte[16];
                Marshal.Copy(lpGuid, guidBytes, 0, 16);
                device.Guid = new Guid(guidBytes);
            }
            device.Description = Marshal.PtrToStringAnsi(lpcstrDescription);
            if (lpcstrModule != null)
            {
                device.ModuleName = Marshal.PtrToStringAnsi(lpcstrModule);
            }
            devices.Add(device);
            return true;
        }

        public void Play()
        {
            if (playbackState == PlaybackState.Stopped)
            {
                notifyThread = new Thread(PlaybackThreadFunc);

                notifyThread.Priority = ThreadPriority.Normal;
                notifyThread.IsBackground = true;
                notifyThread.Start();
            }

            lock (m_LockObject)
            {
                playbackState = PlaybackState.Playing;
            }
        }

        public void Stop()
        {
            if (Monitor.TryEnter(m_LockObject, 50))
            {
                playbackState = PlaybackState.Stopped;
                Monitor.Exit(m_LockObject);
            }
            else
            {
                if (notifyThread != null)
                {
                    notifyThread.Abort();
                    notifyThread = null;
                }
            }
        }

        public void Pause()
        {
            lock (m_LockObject)
            {
                playbackState = PlaybackState.Paused;
            }
        }

        public long GetPosition()
        {
            if (playbackState != Wave.PlaybackState.Stopped)
            {
                var sbuf = secondaryBuffer;
                if (sbuf != null)
                {
                    uint currentPlayCursor, currentWriteCursor;
                    sbuf.GetCurrentPosition(out currentPlayCursor, out currentWriteCursor);
                    return currentPlayCursor + bytesPlayed;
                }
            }
            return 0;
        }

        public void Init(IWaveProvider waveProvider)
        {
            waveStream = waveProvider;
            waveFormat = waveProvider.WaveFormat;
        }

        private void InitializeDirectSound()
        {
            lock (m_LockObject)
            {
                directSound = null;
                DirectSoundCreate(ref device, out directSound, IntPtr.Zero);

                if (directSound != null)
                {
                    directSound.SetCooperativeLevel(GetDesktopWindow(), DirectSoundCooperativeLevel.DSSCL_PRIORITY);

                    BufferDescription bufferDesc = new BufferDescription();
                    bufferDesc.dwSize = Marshal.SizeOf(bufferDesc);
                    bufferDesc.dwBufferBytes = 0;
                    bufferDesc.dwFlags = DirectSoundBufferCaps.DSBCAPS_PRIMARYBUFFER;
                    bufferDesc.dwReserved = 0;
                    bufferDesc.lpwfxFormat = IntPtr.Zero;
                    bufferDesc.guidAlgo = Guid.Empty;

                    object soundBufferObj;

                    directSound.CreateSoundBuffer(bufferDesc, out soundBufferObj, IntPtr.Zero);
                    primarySoundBuffer = (IDirectSoundBuffer)soundBufferObj;

                    primarySoundBuffer.Play(0, 0, DirectSoundPlayFlags.DSBPLAY_LOOPING);

                    samplesFrameSize = MsToBytes(desiredLatency);

                    BufferDescription bufferDesc2 = new BufferDescription();
                    bufferDesc2.dwSize = Marshal.SizeOf(bufferDesc2);
                    bufferDesc2.dwBufferBytes = (uint)(samplesFrameSize * 2);
                    bufferDesc2.dwFlags = DirectSoundBufferCaps.DSBCAPS_GETCURRENTPOSITION2
                                          | DirectSoundBufferCaps.DSBCAPS_CTRLPOSITIONNOTIFY
                                          | DirectSoundBufferCaps.DSBCAPS_GLOBALFOCUS
                                          | DirectSoundBufferCaps.DSBCAPS_CTRLVOLUME
                                          | DirectSoundBufferCaps.DSBCAPS_STICKYFOCUS
                                          | DirectSoundBufferCaps.DSBCAPS_GETCURRENTPOSITION2;
                    bufferDesc2.dwReserved = 0;
                    GCHandle handleOnWaveFormat = GCHandle.Alloc(waveFormat, GCHandleType.Pinned);
                    bufferDesc2.lpwfxFormat = handleOnWaveFormat.AddrOfPinnedObject();
                    bufferDesc2.guidAlgo = Guid.Empty;

                    directSound.CreateSoundBuffer(bufferDesc2, out soundBufferObj, IntPtr.Zero);
                    secondaryBuffer = (IDirectSoundBuffer)soundBufferObj;
                    handleOnWaveFormat.Free();

                    BufferCaps dsbCaps = new BufferCaps();
                    dsbCaps.dwSize = Marshal.SizeOf(dsbCaps);
                    secondaryBuffer.GetCaps(dsbCaps);

                    nextSamplesWriteIndex = 0;
                    samplesTotalSize = dsbCaps.dwBufferBytes;
                    samples = new byte[samplesTotalSize];
                    System.Diagnostics.Debug.Assert(samplesTotalSize == (2 * samplesFrameSize), "Invalid SamplesTotalSize vs SamplesFrameSize");

                    IDirectSoundNotify notify = (IDirectSoundNotify)soundBufferObj;

                    frameEventWaitHandle1 = new EventWaitHandle(false, EventResetMode.AutoReset);
                    frameEventWaitHandle2 = new EventWaitHandle(false, EventResetMode.AutoReset);
                    endEventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

                    DirectSoundBufferPositionNotify[] notifies = new DirectSoundBufferPositionNotify[3];
                    notifies[0] = new DirectSoundBufferPositionNotify();
                    notifies[0].dwOffset = 0;
                    notifies[0].hEventNotify = frameEventWaitHandle1.SafeWaitHandle.DangerousGetHandle();

                    notifies[1] = new DirectSoundBufferPositionNotify();
                    notifies[1].dwOffset = (uint)samplesFrameSize;
                    notifies[1].hEventNotify = frameEventWaitHandle2.SafeWaitHandle.DangerousGetHandle();

                    notifies[2] = new DirectSoundBufferPositionNotify();
                    notifies[2].dwOffset = 0xFFFFFFFF;
                    notifies[2].hEventNotify = endEventWaitHandle.SafeWaitHandle.DangerousGetHandle();

                    notify.SetNotificationPositions(3, notifies);
                }
            }
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }

        private bool IsBufferLost()
        {
            return (secondaryBuffer.GetStatus() & DirectSoundBufferStatus.DSBSTATUS_BUFFERLOST) != 0 ? true : false;
        }

        private int MsToBytes(int ms)
        {
            int bytes = ms * (waveFormat.AverageBytesPerSecond / 1000);
            bytes -= bytes % waveFormat.BlockAlign;
            return bytes;
        }

        private void PlaybackThreadFunc()
        {
            bool lPlaybackHalted = false;
            bool firstBufferStarted = false;
            bytesPlayed = 0;

            Exception exception = null;

            try
            {
                InitializeDirectSound();
                int lResult = 1;

                if (PlaybackState == PlaybackState.Stopped)
                {
                    secondaryBuffer.SetCurrentPosition(0);
                    nextSamplesWriteIndex = 0;
                    lResult = Feed(samplesTotalSize);
                }

                if (lResult > 0)
                {
                    lock (m_LockObject)
                    {
                        playbackState = PlaybackState.Playing;
                    }

                    secondaryBuffer.Play(0, 0, DirectSoundPlayFlags.DSBPLAY_LOOPING);

                    var waitHandles = new WaitHandle[] { frameEventWaitHandle1, frameEventWaitHandle2, endEventWaitHandle };

                    bool lContinuePlayback = true;
                    while (PlaybackState != PlaybackState.Stopped && lContinuePlayback)
                    {
                        int indexHandle = WaitHandle.WaitAny(waitHandles, 3 * desiredLatency, false);

                        if (indexHandle != WaitHandle.WaitTimeout)
                        {
                            if (indexHandle == 2)
                            {
                                StopPlayback();
                                lPlaybackHalted = true;
                                lContinuePlayback = false;
                            }
                            else
                            {
                                if (indexHandle == 0)
                                {
                                    if (firstBufferStarted)
                                    {
                                        bytesPlayed += samplesFrameSize * 2;
                                    }
                                }
                                else
                                {
                                    firstBufferStarted = true;
                                }

                                indexHandle = (indexHandle == 0) ? 1 : 0;
                                nextSamplesWriteIndex = indexHandle * samplesFrameSize;

                                if (Feed(samplesFrameSize) == 0)
                                {
                                    StopPlayback();
                                    lPlaybackHalted = true;
                                    lContinuePlayback = false;
                                }
                            }
                        }
                        else
                        {
                            StopPlayback();
                            lPlaybackHalted = true;
                            lContinuePlayback = false;

                            throw new Exception("DirectSound buffer timeout");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                exception = e;
            }
            finally
            {
                if (!lPlaybackHalted)
                {
                    try
                    {
                        StopPlayback();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());

                        if (exception == null)
                        {
                            exception = e;
                        }
                    }
                }

                lock (m_LockObject)
                {
                    playbackState = PlaybackState.Stopped;
                }

                bytesPlayed = 0;

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

        private void StopPlayback()
        {
            lock (m_LockObject)
            {
                if (secondaryBuffer != null)
                {
                    secondaryBuffer.Stop();
                    secondaryBuffer = null;
                }
                if (primarySoundBuffer != null)
                {
                    primarySoundBuffer.Stop();
                    primarySoundBuffer = null;
                }
            }
        }

        private int Feed(int bytesToCopy)
        {
            int bytesRead = bytesToCopy;

            if (IsBufferLost())
            {
                secondaryBuffer.Restore();
            }

            if (playbackState == PlaybackState.Paused)
            {
                Array.Clear(samples, 0, samples.Length);
            }
            else
            {
                bytesRead = waveStream.Read(samples, 0, bytesToCopy);

                if (bytesRead == 0)
                {
                    Array.Clear(samples, 0, samples.Length);
                    return 0;
                }
            }

            IntPtr wavBuffer1;
            int nbSamples1;
            IntPtr wavBuffer2;
            int nbSamples2;
            secondaryBuffer.Lock(nextSamplesWriteIndex, (uint)bytesRead,
                out wavBuffer1, out nbSamples1,
                out wavBuffer2, out nbSamples2,
                DirectSoundBufferLockFlag.None);

            if (wavBuffer1 != IntPtr.Zero)
            {
                Marshal.Copy(samples, 0, wavBuffer1, nbSamples1);
                if (wavBuffer2 != IntPtr.Zero)
                {
                    Marshal.Copy(samples, 0, wavBuffer1, nbSamples1);
                }
            }

            secondaryBuffer.Unlock(wavBuffer1, nbSamples1, wavBuffer2, nbSamples2);

            return bytesRead;
        }

        [DllImport("dsound.dll", EntryPoint = "DirectSoundCreate", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern void DirectSoundCreate(ref Guid GUID, [Out, MarshalAs(UnmanagedType.Interface)] out IDirectSound directSound, IntPtr pUnkOuter);

        [DllImport("dsound.dll", EntryPoint = "DirectSoundEnumerateA", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern void DirectSoundEnumerate(DSEnumCallback lpDSEnumCallback, IntPtr lpContext);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        #endregion

        #region Вложенный класс: BufferCaps

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal class BufferCaps
        {
            public int dwSize;

            public int dwFlags;

            public int dwBufferBytes;

            public int dwUnlockTransferRate;

            public int dwPlayCpuOverhead;
        }

        #endregion

        #region Вложенный класс: BufferDescription

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal class BufferDescription
        {
            public int dwSize;

            [MarshalAs(UnmanagedType.U4)]
            public DirectSoundBufferCaps dwFlags;

            public uint dwBufferBytes;

            public int dwReserved;

            public IntPtr lpwfxFormat;

            public Guid guidAlgo;
        }

        #endregion

        #region Вложенный класс: DirectSoundBufferPositionNotify

        [StructLayout(LayoutKind.Sequential)]
        internal struct DirectSoundBufferPositionNotify
        {
            public UInt32 dwOffset;

            public IntPtr hEventNotify;
        }

        #endregion

        #region Вложенный класс: IDirectSound

        [ComImport,
         Guid("279AFA83-4981-11CE-A521-0020AF0BE560"),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
         SuppressUnmanagedCodeSecurity]
        internal interface IDirectSound
        {
            void CreateSoundBuffer([In] BufferDescription desc, [Out, MarshalAs(UnmanagedType.Interface)] out object dsDSoundBuffer, IntPtr pUnkOuter);

            void GetCaps(IntPtr caps);

            void DuplicateSoundBuffer([In, MarshalAs(UnmanagedType.Interface)] IDirectSoundBuffer bufferOriginal, [In, MarshalAs(UnmanagedType.Interface)] IDirectSoundBuffer bufferDuplicate);

            void SetCooperativeLevel(IntPtr HWND, [In, MarshalAs(UnmanagedType.U4)] DirectSoundCooperativeLevel dwLevel);

            void Compact();

            void GetSpeakerConfig(IntPtr pdwSpeakerConfig);

            void SetSpeakerConfig(uint pdwSpeakerConfig);

            void Initialize([In, MarshalAs(UnmanagedType.LPStruct)] Guid guid);
        }

        #endregion

        #region Вложенный класс: IDirectSoundBuffer

        [ComImport,
         Guid("279AFA85-4981-11CE-A521-0020AF0BE560"),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
         SuppressUnmanagedCodeSecurity]
        internal interface IDirectSoundBuffer
        {
            void GetCaps([MarshalAs(UnmanagedType.LPStruct)] BufferCaps pBufferCaps);

            void GetCurrentPosition([Out] out uint currentPlayCursor, [Out] out uint currentWriteCursor);

            void GetFormat();

            [return: MarshalAs(UnmanagedType.I4)]
            int GetVolume();

            void GetPan([Out] out uint pan);

            [return: MarshalAs(UnmanagedType.I4)]
            int GetFrequency();

            [return: MarshalAs(UnmanagedType.U4)]
            DirectSoundBufferStatus GetStatus();

            void Initialize([In, MarshalAs(UnmanagedType.Interface)] IDirectSound directSound, [In] BufferDescription desc);

            void Lock(int dwOffset,
                uint dwBytes,
                [Out] out IntPtr audioPtr1,
                [Out] out int audioBytes1,
                [Out] out IntPtr audioPtr2,
                [Out] out int audioBytes2,
                [MarshalAs(UnmanagedType.U4)] DirectSoundBufferLockFlag dwFlags);

            void Play(uint dwReserved1, uint dwPriority, [In, MarshalAs(UnmanagedType.U4)] DirectSoundPlayFlags dwFlags);

            void SetCurrentPosition(uint dwNewPosition);

            void SetFormat([In] WaveFormat pcfxFormat);

            void SetVolume(int volume);

            void SetPan(uint pan);

            void SetFrequency(uint frequency);

            void Stop();

            void Unlock(IntPtr pvAudioPtr1, int dwAudioBytes1, IntPtr pvAudioPtr2, int dwAudioBytes2);

            void Restore();
        }

        #endregion

        #region Вложенный класс: IDirectSoundNotify

        [ComImport,
         Guid("b0210783-89cd-11d0-af08-00a0c925cd16"),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
         SuppressUnmanagedCodeSecurity]
        internal interface IDirectSoundNotify
        {
            void SetNotificationPositions(UInt32 dwPositionNotifies, [In, MarshalAs(UnmanagedType.LPArray)] DirectSoundBufferPositionNotify[] pcPositionNotifies);
        }

        #endregion
    }

    internal class DirectSoundDeviceInfo
    {
        #region Свойства

        public Guid Guid { get; set; }

        public string Description { get; set; }

        public string ModuleName { get; set; }

        #endregion
    }
}