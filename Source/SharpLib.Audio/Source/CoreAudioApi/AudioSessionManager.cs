using System;

using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
    internal class AudioSessionManager
    {
        #region Поля

        private readonly IAudioSessionManager audioSessionInterface;

        private AudioSessionControl audioSessionControl;

        private SimpleAudioVolume simpleAudioVolume;

        #endregion

        #region Свойства

        public SimpleAudioVolume SimpleAudioVolume
        {
            get
            {
                if (simpleAudioVolume == null)
                {
                    ISimpleAudioVolume simpleAudioInterface;

                    audioSessionInterface.GetSimpleAudioVolume(Guid.Empty, 0, out simpleAudioInterface);

                    simpleAudioVolume = new SimpleAudioVolume(simpleAudioInterface);
                }
                return simpleAudioVolume;
            }
        }

        public AudioSessionControl AudioSessionControl
        {
            get
            {
                if (audioSessionControl == null)
                {
                    IAudioSessionControl audioSessionControlInterface;

                    audioSessionInterface.GetAudioSessionControl(Guid.Empty, 0, out audioSessionControlInterface);

                    audioSessionControl = new AudioSessionControl(audioSessionControlInterface);
                }
                return audioSessionControl;
            }
        }

        #endregion

        #region Конструктор

        internal AudioSessionManager(IAudioSessionManager audioSessionManager)
        {
            audioSessionInterface = audioSessionManager;
        }

        #endregion
    }
}