using System;
using System.Runtime.InteropServices;

using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
    internal class AudioEndpointVolumeCallback : IAudioEndpointVolumeCallback
    {
        #region Поля

        private readonly AudioEndpointVolume parent;

        #endregion

        #region Конструктор

        internal AudioEndpointVolumeCallback(AudioEndpointVolume parent)
        {
            this.parent = parent;
        }

        #endregion

        #region Методы

        public void OnNotify(IntPtr notifyData)
        {
            var data = (AudioVolumeNotificationDataStruct)Marshal.PtrToStructure(notifyData, typeof(AudioVolumeNotificationDataStruct));

            var offset = Marshal.OffsetOf(typeof(AudioVolumeNotificationDataStruct), "ChannelVolume");

            var firstFloatPtr = (IntPtr)((long)notifyData + (long)offset);

            var voldata = new float[data.nChannels];

            for (int i = 0; i < data.nChannels; i++)
            {
                voldata[i] = (float)Marshal.PtrToStructure(firstFloatPtr, typeof(float));
            }

            var notificationData = new AudioVolumeNotificationData(data.guidEventContext, data.bMuted, data.fMasterVolume, voldata);
            parent.FireNotification(notificationData);
        }

        #endregion
    }
}