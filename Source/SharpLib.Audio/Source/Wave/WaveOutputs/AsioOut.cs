using System;
using System.Threading;

using SharpLib.Audio.Wave.Asio;

namespace SharpLib.Audio.Wave
{
    internal class AsioOut : IWavePlayer
    {
        #region Поля

        private readonly string driverName;

        private readonly SynchronizationContext syncContext;

        private ASIOSampleConvertor.SampleConvertor convertor;

        private ASIODriverExt driver;

        private int nbSamples;

        private PlaybackState playbackState;

        private IWaveProvider sourceStream;

        private byte[] waveBuffer;

        #endregion

        #region Свойства

        public int PlaybackLatency
        {
            get
            {
                int latency, temp;
                driver.Driver.GetLatencies(out temp, out latency);
                return latency;
            }
        }

        public PlaybackState PlaybackState
        {
            get { return playbackState; }
        }

        public string DriverName
        {
            get { return driverName; }
        }

        public int NumberOfOutputChannels { get; private set; }

        public int NumberOfInputChannels { get; private set; }

        public int DriverInputChannelCount
        {
            get { return driver.Capabilities.NbInputChannels; }
        }

        public int DriverOutputChannelCount
        {
            get { return driver.Capabilities.NbOutputChannels; }
        }

        public int ChannelOffset { get; set; }

        public int InputChannelOffset { get; set; }

        [Obsolete("this function will be removed in a future NAudio as ASIO does not support setting the volume on the device")]
        public float Volume
        {
            get { return 1.0f; }
            set
            {
                if (value != 1.0f)
                {
                    throw new InvalidOperationException("AsioOut does not support setting the device volume");
                }
            }
        }

        #endregion

        #region События

        public event EventHandler<AsioAudioAvailableEventArgs> AudioAvailable;

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        #endregion

        #region Конструктор

        public AsioOut()
            : this(0)
        {
        }

        public AsioOut(String driverName)
        {
            syncContext = SynchronizationContext.Current;
            InitFromName(driverName);
        }

        public AsioOut(int driverIndex)
        {
            syncContext = SynchronizationContext.Current;
            String[] names = GetDriverNames();
            if (names.Length == 0)
            {
                throw new ArgumentException("There is no ASIO Driver installed on your system");
            }
            if (driverIndex < 0 || driverIndex > names.Length)
            {
                throw new ArgumentException(String.Format("Invalid device number. Must be in the range [0,{0}]", names.Length));
            }
            driverName = names[driverIndex];
            InitFromName(driverName);
        }

        ~AsioOut()
        {
            Dispose();
        }

        #endregion

        #region Методы

        public void Dispose()
        {
            if (driver != null)
            {
                if (playbackState != PlaybackState.Stopped)
                {
                    driver.Stop();
                }
                driver.ReleaseDriver();
                driver = null;
            }
        }

        public static String[] GetDriverNames()
        {
            return ASIODriver.GetASIODriverNames();
        }

        public static bool isSupported()
        {
            return GetDriverNames().Length > 0;
        }

        private void InitFromName(String driverName)
        {
            ASIODriver basicDriver = ASIODriver.GetASIODriverByName(driverName);

            driver = new ASIODriverExt(basicDriver);
            ChannelOffset = 0;
        }

        public void ShowControlPanel()
        {
            driver.ShowControlPanel();
        }

        public void Play()
        {
            if (playbackState != PlaybackState.Playing)
            {
                playbackState = PlaybackState.Playing;
                driver.Start();
            }
        }

        public void Stop()
        {
            playbackState = PlaybackState.Stopped;
            driver.Stop();
            RaisePlaybackStopped(null);
        }

        public void Pause()
        {
            playbackState = PlaybackState.Paused;
            driver.Stop();
        }

        public void Init(IWaveProvider waveProvider)
        {
            InitRecordAndPlayback(waveProvider, 0, -1);
        }

        public void InitRecordAndPlayback(IWaveProvider waveProvider, int recordChannels, int recordOnlySampleRate)
        {
            if (sourceStream != null)
            {
                throw new InvalidOperationException("Already initialised this instance of AsioOut - dispose and create a new one");
            }
            int desiredSampleRate = waveProvider != null ? waveProvider.WaveFormat.SampleRate : recordOnlySampleRate;

            if (waveProvider != null)
            {
                sourceStream = waveProvider;

                NumberOfOutputChannels = waveProvider.WaveFormat.Channels;

                convertor = ASIOSampleConvertor.SelectSampleConvertor(waveProvider.WaveFormat, driver.Capabilities.OutputChannelInfos[0].type);
            }
            else
            {
                NumberOfOutputChannels = 0;
            }

            if (!driver.IsSampleRateSupported(desiredSampleRate))
            {
                throw new ArgumentException("SampleRate is not supported");
            }
            if (driver.Capabilities.SampleRate != desiredSampleRate)
            {
                driver.SetSampleRate(desiredSampleRate);
            }

            driver.FillBufferCallback = driver_BufferUpdate;

            NumberOfInputChannels = recordChannels;

            nbSamples = driver.CreateBuffers(NumberOfOutputChannels, NumberOfInputChannels, false);
            driver.SetChannelOffset(ChannelOffset, InputChannelOffset);

            if (waveProvider != null)
            {
                waveBuffer = new byte[nbSamples * NumberOfOutputChannels * waveProvider.WaveFormat.BitsPerSample / 8];
            }
        }

        private void driver_BufferUpdate(IntPtr[] inputChannels, IntPtr[] outputChannels)
        {
            if (NumberOfInputChannels > 0)
            {
                var audioAvailable = AudioAvailable;
                if (audioAvailable != null)
                {
                    var args = new AsioAudioAvailableEventArgs(inputChannels, outputChannels, nbSamples,
                        driver.Capabilities.InputChannelInfos[0].type);
                    audioAvailable(this, args);
                    if (args.WrittenToOutputBuffers)
                    {
                        return;
                    }
                }
            }

            if (NumberOfOutputChannels > 0)
            {
                int read = sourceStream.Read(waveBuffer, 0, waveBuffer.Length);
                if (read < waveBuffer.Length)
                {
                }

                unsafe
                {
                    fixed (void* pBuffer = &waveBuffer[0])
                    {
                        convertor(new IntPtr(pBuffer), outputChannels, NumberOfOutputChannels, nbSamples);
                    }
                }

                if (read == 0)
                {
                    Stop();
                }
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

        public string AsioInputChannelName(int channel)
        {
            return channel > DriverInputChannelCount ? "" : driver.Capabilities.InputChannelInfos[channel].name;
        }

        public string AsioOutputChannelName(int channel)
        {
            return channel > DriverOutputChannelCount ? "" : driver.Capabilities.OutputChannelInfos[channel].name;
        }

        #endregion
    }
}