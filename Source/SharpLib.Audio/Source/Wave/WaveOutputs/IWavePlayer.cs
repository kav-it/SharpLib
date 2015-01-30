using System;

namespace NAudio.Wave
{
    internal interface IWavePlayer : IDisposable
    {
        #region Свойства

        PlaybackState PlaybackState { get; }

        [Obsolete("Not intending to keep supporting this going forward: set the volume on your input WaveProvider instead")]
        float Volume { get; set; }

        #endregion

        #region События

        event EventHandler<StoppedEventArgs> PlaybackStopped;

        #endregion

        #region Методы

        void Play();

        void Stop();

        void Pause();

        void Init(IWaveProvider waveProvider);

        #endregion
    }

    internal interface IWavePosition
    {
        #region Свойства

        WaveFormat OutputWaveFormat { get; }

        #endregion

        #region Методы

        long GetPosition();

        #endregion
    }
}