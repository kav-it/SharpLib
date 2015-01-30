using System;
using System.Runtime.InteropServices;

using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
    internal class AudioSessionControl : IDisposable
    {
        #region Поля

        private readonly IAudioSessionControl audioSessionControlInterface;

        private AudioSessionEventsCallback audioSessionEventCallback;

        #endregion

        #region Свойства

        public AudioSessionState State
        {
            get
            {
                AudioSessionState state;

                Marshal.ThrowExceptionForHR(audioSessionControlInterface.GetState(out state));

                return state;
            }
        }

        public string DisplayName
        {
            get
            {
                string displayName = String.Empty;

                Marshal.ThrowExceptionForHR(audioSessionControlInterface.GetDisplayName(out displayName));

                return displayName;
            }
            set
            {
                if (value != String.Empty)
                {
                    Marshal.ThrowExceptionForHR(audioSessionControlInterface.SetDisplayName(value, Guid.Empty));
                }
            }
        }

        public string IconPath
        {
            get
            {
                string iconPath = String.Empty;

                Marshal.ThrowExceptionForHR(audioSessionControlInterface.GetIconPath(out iconPath));

                return iconPath;
            }
            set
            {
                if (value != String.Empty)
                {
                    Marshal.ThrowExceptionForHR(audioSessionControlInterface.SetIconPath(value, Guid.Empty));
                }
            }
        }

        #endregion

        #region Конструктор

        internal AudioSessionControl(IAudioSessionControl audioSessionControl)
        {
            audioSessionControlInterface = audioSessionControl;
        }

        ~AudioSessionControl()
        {
            if (audioSessionEventCallback != null)
            {
                Marshal.ThrowExceptionForHR(audioSessionControlInterface.UnregisterAudioSessionNotification(audioSessionEventCallback));
            }
            Dispose();
        }

        #endregion

        #region Методы

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public Guid GetGroupingParam()
        {
            Guid groupingId = Guid.Empty;

            Marshal.ThrowExceptionForHR(audioSessionControlInterface.GetGroupingParam(out groupingId));

            return groupingId;
        }

        public void SetGroupingParam(Guid groupingId, Guid context)
        {
            Marshal.ThrowExceptionForHR(audioSessionControlInterface.SetGroupingParam(groupingId, context));
        }

        public void RegisterEventClient(IAudioSessionEventsHandler eventClient)
        {
            audioSessionEventCallback = new AudioSessionEventsCallback(eventClient);
            Marshal.ThrowExceptionForHR(audioSessionControlInterface.RegisterAudioSessionNotification(audioSessionEventCallback));
        }

        public void UnRegisterEventClient(IAudioSessionEventsHandler eventClient)
        {
            if (audioSessionEventCallback != null)
            {
                Marshal.ThrowExceptionForHR(audioSessionControlInterface.UnregisterAudioSessionNotification(audioSessionEventCallback));
            }
        }

        #endregion
    }
}