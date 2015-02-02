using System;

namespace SharpLib.Audio.CoreAudioApi.Interfaces
{
    internal struct AudioVolumeNotificationDataStruct
    {
        #region Поля

        public float ChannelVolume;

        public bool bMuted;

        public float fMasterVolume;

        public Guid guidEventContext;

        public uint nChannels;

        #endregion

        #region Методы

        private void FixCS0649()
        {
            guidEventContext = Guid.Empty;
            bMuted = false;
            fMasterVolume = 0;
            nChannels = 0;
            ChannelVolume = 0;
        }

        #endregion
    }
}