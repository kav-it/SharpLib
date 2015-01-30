namespace NAudio.CoreAudioApi.Interfaces
{
    internal interface IAudioSessionEventsHandler
    {
        #region Методы

        void OnVolumeChanged(float volume, bool isMuted);

        #endregion
    }
}