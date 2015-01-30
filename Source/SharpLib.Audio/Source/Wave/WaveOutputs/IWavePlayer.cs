using System;

namespace NAudio.Wave
{
    internal interface IWavePlayer : IDisposable
    {
        #region ��������

        PlaybackState PlaybackState { get; }

        [Obsolete("Not intending to keep supporting this going forward: set the volume on your input WaveProvider instead")]
        float Volume { get; set; }

        #endregion

        #region �������

        event EventHandler<StoppedEventArgs> PlaybackStopped;

        #endregion

        #region ������

        void Play();

        void Stop();

        void Pause();

        void Init(IWaveProvider waveProvider);

        #endregion
    }

    internal interface IWavePosition
    {
        #region ��������

        WaveFormat OutputWaveFormat { get; }

        #endregion

        #region ������

        long GetPosition();

        #endregion
    }
}