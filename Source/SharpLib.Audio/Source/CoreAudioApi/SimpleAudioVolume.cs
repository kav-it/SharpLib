using System;
using System.Runtime.InteropServices;

using SharpLib.Audio.CoreAudioApi.Interfaces;

namespace SharpLib.Audio.CoreAudioApi
{
    internal class SimpleAudioVolume : IDisposable
    {
        #region Поля

        private readonly ISimpleAudioVolume simpleAudioVolume;

        #endregion

        #region Свойства

        public float Volume
        {
            get
            {
                float result;
                Marshal.ThrowExceptionForHR(simpleAudioVolume.GetMasterVolume(out result));
                return result;
            }
            set
            {
                if ((value >= 0.0) && (value <= 1.0))
                {
                    Marshal.ThrowExceptionForHR(simpleAudioVolume.SetMasterVolume(value, Guid.Empty));
                }
            }
        }

        public bool Mute
        {
            get
            {
                bool result;

                Marshal.ThrowExceptionForHR(simpleAudioVolume.GetMute(out result));

                return result;
            }
            set { Marshal.ThrowExceptionForHR(simpleAudioVolume.SetMute(value, Guid.Empty)); }
        }

        #endregion

        #region Конструктор

        internal SimpleAudioVolume(ISimpleAudioVolume realSimpleVolume)
        {
            simpleAudioVolume = realSimpleVolume;
        }

        ~SimpleAudioVolume()
        {
            Dispose();
        }

        #endregion

        #region Методы

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}