using System.Runtime.InteropServices;

using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
    internal class AudioEndpointVolumeVolumeRange
    {
        #region Поля

        private readonly float volumeIncrementDecibels;

        private readonly float volumeMaxDecibels;

        private readonly float volumeMinDecibels;

        #endregion

        #region Свойства

        public float MinDecibels
        {
            get { return volumeMinDecibels; }
        }

        public float MaxDecibels
        {
            get { return volumeMaxDecibels; }
        }

        public float IncrementDecibels
        {
            get { return volumeIncrementDecibels; }
        }

        #endregion

        #region Конструктор

        internal AudioEndpointVolumeVolumeRange(IAudioEndpointVolume parent)
        {
            Marshal.ThrowExceptionForHR(parent.GetVolumeRange(out volumeMinDecibels, out volumeMaxDecibels, out volumeIncrementDecibels));
        }

        #endregion
    }
}