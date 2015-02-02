using System.Runtime.InteropServices;

using SharpLib.Audio.CoreAudioApi.Interfaces;

namespace SharpLib.Audio.CoreAudioApi
{
    internal class AudioEndpointVolumeChannels
    {
        #region Поля

        private readonly IAudioEndpointVolume audioEndPointVolume;

        private readonly AudioEndpointVolumeChannel[] channels;

        #endregion

        #region Свойства

        public int Count
        {
            get
            {
                int result;
                Marshal.ThrowExceptionForHR(audioEndPointVolume.GetChannelCount(out result));
                return result;
            }
        }

        public AudioEndpointVolumeChannel this[int index]
        {
            get { return channels[index]; }
        }

        #endregion

        #region Конструктор

        internal AudioEndpointVolumeChannels(IAudioEndpointVolume parent)
        {
            int ChannelCount;
            audioEndPointVolume = parent;

            ChannelCount = Count;
            channels = new AudioEndpointVolumeChannel[ChannelCount];
            for (int i = 0; i < ChannelCount; i++)
            {
                channels[i] = new AudioEndpointVolumeChannel(audioEndPointVolume, i);
            }
        }

        #endregion
    }
}