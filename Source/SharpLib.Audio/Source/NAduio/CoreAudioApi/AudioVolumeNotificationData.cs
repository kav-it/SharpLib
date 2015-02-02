using System;

namespace SharpLib.Audio.CoreAudioApi
{
    internal class AudioVolumeNotificationData
    {
        #region ����

        private readonly float[] channelVolume;

        private readonly int channels;

        private readonly Guid eventContext;

        private readonly float masterVolume;

        private readonly bool muted;

        #endregion

        #region ��������

        public Guid EventContext
        {
            get { return eventContext; }
        }

        public bool Muted
        {
            get { return muted; }
        }

        public float MasterVolume
        {
            get { return masterVolume; }
        }

        public int Channels
        {
            get { return channels; }
        }

        public float[] ChannelVolume
        {
            get { return channelVolume; }
        }

        #endregion

        #region �����������

        public AudioVolumeNotificationData(Guid eventContext, bool muted, float masterVolume, float[] channelVolume)
        {
            this.eventContext = eventContext;
            this.muted = muted;
            this.masterVolume = masterVolume;
            channels = channelVolume.Length;
            this.channelVolume = channelVolume;
        }

        #endregion
    }
}