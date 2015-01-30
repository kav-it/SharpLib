﻿using System;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi.Interfaces
{
    internal enum AudioSessionState
    {
        AudioSessionStateInactive = 0,

        AudioSessionStateActive = 1,

        AudioSessionStateExpired = 2
    }

    internal enum AudioSessionDisconnectReason
    {
        DisconnectReasonDeviceRemoval = 0,

        DisconnectReasonServerShutdown = 1,

        DisconnectReasonFormatChanged = 2,

        DisconnectReasonSessionLogoff = 3,

        DisconnectReasonSessionDisconnected = 4,

        DisconnectReasonExclusiveModeOverride = 5
    }

    [Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionEvents
    {
        [PreserveSig]
        int OnDisplayNameChanged(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string displayName,
            [In] ref Guid eventContext);

        [PreserveSig]
        int OnIconPathChanged(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string iconPath,
            [In] ref Guid eventContext);

        [PreserveSig]
        int OnSimpleVolumeChanged(
            [In] [MarshalAs(UnmanagedType.R4)] float volume,
            [In] [MarshalAs(UnmanagedType.Bool)] bool isMuted,
            [In] ref Guid eventContext);

        [PreserveSig]
        int OnChannelVolumeChanged(
            [In] [MarshalAs(UnmanagedType.U4)] UInt32 channelCount,
            [In] [MarshalAs(UnmanagedType.SysInt)] IntPtr newVolumes,
            [In] [MarshalAs(UnmanagedType.U4)] UInt32 channelIndex,
            [In] ref Guid eventContext);

        [PreserveSig]
        int OnGroupingParamChanged(
            [In] ref Guid groupingId,
            [In] ref Guid eventContext);

        [PreserveSig]
        int OnStateChanged(
            [In] AudioSessionState state);

        [PreserveSig]
        int OnSessionDisconnected(
            [In] AudioSessionDisconnectReason disconnectReason);
    }
}