using System;
using System.Runtime.InteropServices;

using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
    internal class AudioEndpointVolumeChannel
    {
        #region ����

        private readonly IAudioEndpointVolume audioEndpointVolume;

        private readonly uint channel;

        #endregion

        #region ��������

        public float VolumeLevel
        {
            get
            {
                float result;
                Marshal.ThrowExceptionForHR(audioEndpointVolume.GetChannelVolumeLevel(channel, out result));
                return result;
            }
            set { Marshal.ThrowExceptionForHR(audioEndpointVolume.SetChannelVolumeLevel(channel, value, Guid.Empty)); }
        }

        public float VolumeLevelScalar
        {
            get
            {
                float result;
                Marshal.ThrowExceptionForHR(audioEndpointVolume.GetChannelVolumeLevelScalar(channel, out result));
                return result;
            }
            set { Marshal.ThrowExceptionForHR(audioEndpointVolume.SetChannelVolumeLevelScalar(channel, value, Guid.Empty)); }
        }

        #endregion

        #region �����������

        internal AudioEndpointVolumeChannel(IAudioEndpointVolume parent, int channel)
        {
            this.channel = (uint)channel;
            audioEndpointVolume = parent;
        }

        #endregion
    }
}