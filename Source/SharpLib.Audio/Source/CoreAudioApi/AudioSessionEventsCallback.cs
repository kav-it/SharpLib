using System;
using System.Runtime.InteropServices;

using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
    internal class AudioSessionEventsCallback : IAudioSessionEvents
    {
        #region Поля

        private readonly IAudioSessionEventsHandler audioSessionEventsHandler;

        #endregion

        #region Конструктор

        internal AudioSessionEventsCallback(IAudioSessionEventsHandler handler)
        {
            audioSessionEventsHandler = handler;
        }

        #endregion

        #region Методы

        public int OnDisplayNameChanged(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string displayName,
            [In] ref Guid eventContext)
        {
            return 0;
        }

        public int OnIconPathChanged(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string iconPath,
            [In] ref Guid eventContext)
        {
            return 0;
        }

        public int OnSimpleVolumeChanged(
            [In] [MarshalAs(UnmanagedType.R4)] float volume,
            [In] [MarshalAs(UnmanagedType.Bool)] bool isMuted,
            [In] ref Guid eventContext)
        {
            audioSessionEventsHandler.OnVolumeChanged(volume, isMuted);

            return 0;
        }

        public int OnChannelVolumeChanged(
            [In] [MarshalAs(UnmanagedType.U4)] UInt32 channelCount,
            [In] [MarshalAs(UnmanagedType.SysInt)] IntPtr newVolumes,
            [In] [MarshalAs(UnmanagedType.U4)] UInt32 channelIndex,
            [In] ref Guid eventContext)
        {
            return 0;
        }

        public int OnGroupingParamChanged(
            [In] ref Guid groupingId,
            [In] ref Guid eventContext)
        {
            return 0;
        }

        public int OnStateChanged(
            [In] AudioSessionState state)
        {
            return 0;
        }

        public int OnSessionDisconnected(
            [In] AudioSessionDisconnectReason disconnectReason)
        {
            return 0;
        }

        #endregion
    }
}