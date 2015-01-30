using System;
using System.Runtime.InteropServices;

using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
    internal class AudioEndpointVolume : IDisposable
    {
        #region Поля

        private readonly IAudioEndpointVolume audioEndPointVolume;

        private readonly AudioEndpointVolumeChannels channels;

        private readonly EEndpointHardwareSupport hardwareSupport;

        private readonly AudioEndpointVolumeStepInformation stepInformation;

        private readonly AudioEndpointVolumeVolumeRange volumeRange;

        private AudioEndpointVolumeCallback callBack;

        #endregion

        #region Свойства

        public AudioEndpointVolumeVolumeRange VolumeRange
        {
            get { return volumeRange; }
        }

        public EEndpointHardwareSupport HardwareSupport
        {
            get { return hardwareSupport; }
        }

        public AudioEndpointVolumeStepInformation StepInformation
        {
            get { return stepInformation; }
        }

        public AudioEndpointVolumeChannels Channels
        {
            get { return channels; }
        }

        public float MasterVolumeLevel
        {
            get
            {
                float result;
                Marshal.ThrowExceptionForHR(audioEndPointVolume.GetMasterVolumeLevel(out result));
                return result;
            }
            set { Marshal.ThrowExceptionForHR(audioEndPointVolume.SetMasterVolumeLevel(value, Guid.Empty)); }
        }

        public float MasterVolumeLevelScalar
        {
            get
            {
                float result;
                Marshal.ThrowExceptionForHR(audioEndPointVolume.GetMasterVolumeLevelScalar(out result));
                return result;
            }
            set { Marshal.ThrowExceptionForHR(audioEndPointVolume.SetMasterVolumeLevelScalar(value, Guid.Empty)); }
        }

        public bool Mute
        {
            get
            {
                bool result;
                Marshal.ThrowExceptionForHR(audioEndPointVolume.GetMute(out result));
                return result;
            }
            set { Marshal.ThrowExceptionForHR(audioEndPointVolume.SetMute(value, Guid.Empty)); }
        }

        #endregion

        #region События

        public event AudioEndpointVolumeNotificationDelegate OnVolumeNotification;

        #endregion

        #region Конструктор

        internal AudioEndpointVolume(IAudioEndpointVolume realEndpointVolume)
        {
            uint hardwareSupp;

            audioEndPointVolume = realEndpointVolume;
            channels = new AudioEndpointVolumeChannels(audioEndPointVolume);
            stepInformation = new AudioEndpointVolumeStepInformation(audioEndPointVolume);
            Marshal.ThrowExceptionForHR(audioEndPointVolume.QueryHardwareSupport(out hardwareSupp));
            hardwareSupport = (EEndpointHardwareSupport)hardwareSupp;
            volumeRange = new AudioEndpointVolumeVolumeRange(audioEndPointVolume);
            callBack = new AudioEndpointVolumeCallback(this);
            Marshal.ThrowExceptionForHR(audioEndPointVolume.RegisterControlChangeNotify(callBack));
        }

        ~AudioEndpointVolume()
        {
            Dispose();
        }

        #endregion

        #region Методы

        public void VolumeStepUp()
        {
            Marshal.ThrowExceptionForHR(audioEndPointVolume.VolumeStepUp(Guid.Empty));
        }

        public void VolumeStepDown()
        {
            Marshal.ThrowExceptionForHR(audioEndPointVolume.VolumeStepDown(Guid.Empty));
        }

        internal void FireNotification(AudioVolumeNotificationData notificationData)
        {
            AudioEndpointVolumeNotificationDelegate del = OnVolumeNotification;
            if (del != null)
            {
                del(notificationData);
            }
        }

        public void Dispose()
        {
            if (callBack != null)
            {
                Marshal.ThrowExceptionForHR(audioEndPointVolume.UnregisterControlChangeNotify(callBack));
                callBack = null;
            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}