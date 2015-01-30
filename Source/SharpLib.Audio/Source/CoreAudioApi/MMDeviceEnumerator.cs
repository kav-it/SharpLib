using System;
using System.Runtime.InteropServices;

using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
    internal class MMDeviceEnumerator
    {
        #region Поля

        private readonly IMMDeviceEnumerator realEnumerator;

        #endregion

        #region Конструктор

        public MMDeviceEnumerator()
        {
#if !NETFX_CORE
            if (System.Environment.OSVersion.Version.Major < 6)
            {
                throw new NotSupportedException("This functionality is only supported on Windows Vista or newer.");
            }
#endif
            realEnumerator = new MMDeviceEnumeratorComObject() as IMMDeviceEnumerator;
        }

        #endregion

        #region Методы

        public MMDeviceCollection EnumerateAudioEndPoints(DataFlow dataFlow, DeviceState dwStateMask)
        {
            IMMDeviceCollection result;
            Marshal.ThrowExceptionForHR(realEnumerator.EnumAudioEndpoints(dataFlow, dwStateMask, out result));
            return new MMDeviceCollection(result);
        }

        public MMDevice GetDefaultAudioEndpoint(DataFlow dataFlow, Role role)
        {
            IMMDevice device = null;
            Marshal.ThrowExceptionForHR(realEnumerator.GetDefaultAudioEndpoint(dataFlow, role, out device));
            return new MMDevice(device);
        }

        public MMDevice GetDevice(string id)
        {
            IMMDevice device = null;
            Marshal.ThrowExceptionForHR(realEnumerator.GetDevice(id, out device));
            return new MMDevice(device);
        }

        public int RegisterEndpointNotificationCallback([In] [MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
        {
            return realEnumerator.RegisterEndpointNotificationCallback(client);
        }

        public int UnregisterEndpointNotificationCallback([In] [MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
        {
            return realEnumerator.UnregisterEndpointNotificationCallback(client);
        }

        #endregion
    }
}