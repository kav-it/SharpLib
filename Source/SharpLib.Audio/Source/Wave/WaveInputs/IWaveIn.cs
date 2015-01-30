using System;

namespace SharpLib.Audio.Wave
{
    internal interface IWaveIn : IDisposable
    {
        #region Свойства

        WaveFormat WaveFormat { get; set; }

        #endregion

        #region События

        event EventHandler<WaveInEventArgs> DataAvailable;

        event EventHandler<StoppedEventArgs> RecordingStopped;

        #endregion

        #region Методы

        void StartRecording();

        void StopRecording();

        #endregion
    }
}