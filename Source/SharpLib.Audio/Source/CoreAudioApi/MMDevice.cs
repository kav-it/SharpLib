using System;
using System.Runtime.InteropServices;

using SharpLib.Audio.CoreAudioApi.Interfaces;

namespace SharpLib.Audio.CoreAudioApi
{
    internal class MMDevice
    {
        #region Поля

        private static Guid IDD_IAudioSessionManager = new Guid("BFA971F1-4D5E-40BB-935E-967039BFBEE4");

        private static Guid IID_IAudioClient = new Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2");

        private static Guid IID_IAudioEndpointVolume = new Guid("5CDF2C82-841E-4546-9722-0CF74078229A");

        private static Guid IID_IAudioMeterInformation = new Guid("C02216F6-8C67-4B5B-9D00-D008E73E0064");

        private readonly IMMDevice deviceInterface;

        private AudioEndpointVolume audioEndpointVolume;

        private AudioMeterInformation audioMeterInformation;

        private AudioSessionManager audioSessionManager;

        private PropertyStore propertyStore;

        #endregion

        #region Свойства

        public AudioClient AudioClient
        {
            get { return GetAudioClient(); }
        }

        public AudioMeterInformation AudioMeterInformation
        {
            get
            {
                if (audioMeterInformation == null)
                {
                    GetAudioMeterInformation();
                }

                return audioMeterInformation;
            }
        }

        public AudioEndpointVolume AudioEndpointVolume
        {
            get
            {
                if (audioEndpointVolume == null)
                {
                    GetAudioEndpointVolume();
                }

                return audioEndpointVolume;
            }
        }

        public AudioSessionManager AudioSessionManager
        {
            get
            {
                if (audioSessionManager == null)
                {
                    GetAudioSessionManager();
                }
                return audioSessionManager;
            }
        }

        public PropertyStore Properties
        {
            get
            {
                if (propertyStore == null)
                {
                    GetPropertyInformation();
                }
                return propertyStore;
            }
        }

        public string FriendlyName
        {
            get
            {
                if (propertyStore == null)
                {
                    GetPropertyInformation();
                }
                if (propertyStore.Contains(PropertyKeys.PKEY_Device_FriendlyName))
                {
                    return (string)propertyStore[PropertyKeys.PKEY_Device_FriendlyName].Value;
                }
                return "Unknown";
            }
        }

        public string DeviceFriendlyName
        {
            get
            {
                if (propertyStore == null)
                {
                    GetPropertyInformation();
                }
                if (propertyStore.Contains(PropertyKeys.PKEY_DeviceInterface_FriendlyName))
                {
                    return (string)propertyStore[PropertyKeys.PKEY_DeviceInterface_FriendlyName].Value;
                }
                return "Unknown";
            }
        }

        public string ID
        {
            get
            {
                string result;
                Marshal.ThrowExceptionForHR(deviceInterface.GetId(out result));
                return result;
            }
        }

        public DataFlow DataFlow
        {
            get
            {
                DataFlow result;
                var ep = deviceInterface as IMMEndpoint;
                ep.GetDataFlow(out result);
                return result;
            }
        }

        public DeviceState State
        {
            get
            {
                DeviceState result;
                Marshal.ThrowExceptionForHR(deviceInterface.GetState(out result));
                return result;
            }
        }

        #endregion

        #region Конструктор

        internal MMDevice(IMMDevice realDevice)
        {
            deviceInterface = realDevice;
        }

        #endregion

        #region Методы

        private void GetPropertyInformation()
        {
            IPropertyStore propstore;
            Marshal.ThrowExceptionForHR(deviceInterface.OpenPropertyStore(StorageAccessMode.Read, out propstore));
            propertyStore = new PropertyStore(propstore);
        }

        private AudioClient GetAudioClient()
        {
            object result;
            Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioClient, ClsCtx.ALL, IntPtr.Zero, out result));
            return new AudioClient(result as IAudioClient);
        }

        private void GetAudioMeterInformation()
        {
            object result;
            Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioMeterInformation, ClsCtx.ALL, IntPtr.Zero, out result));
            audioMeterInformation = new AudioMeterInformation(result as IAudioMeterInformation);
        }

        private void GetAudioEndpointVolume()
        {
            object result;
            Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioEndpointVolume, ClsCtx.ALL, IntPtr.Zero, out result));
            audioEndpointVolume = new AudioEndpointVolume(result as IAudioEndpointVolume);
        }

        private void GetAudioSessionManager()
        {
            object result;
            Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IDD_IAudioSessionManager, ClsCtx.ALL, IntPtr.Zero, out result));
            audioSessionManager = new AudioSessionManager(result as IAudioSessionManager);
        }

        public override string ToString()
        {
            return FriendlyName;
        }

        #endregion
    }
}