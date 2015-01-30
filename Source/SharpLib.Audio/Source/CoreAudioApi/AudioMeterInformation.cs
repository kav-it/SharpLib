using System.Runtime.InteropServices;

using SharpLib.Audio.CoreAudioApi.Interfaces;

namespace SharpLib.Audio.CoreAudioApi
{
    internal class AudioMeterInformation
    {
        #region Поля

        private readonly IAudioMeterInformation audioMeterInformation;

        private readonly AudioMeterInformationChannels channels;

        private readonly EEndpointHardwareSupport hardwareSupport;

        #endregion

        #region Свойства

        public AudioMeterInformationChannels PeakValues
        {
            get { return channels; }
        }

        public EEndpointHardwareSupport HardwareSupport
        {
            get { return hardwareSupport; }
        }

        public float MasterPeakValue
        {
            get
            {
                float result;
                Marshal.ThrowExceptionForHR(audioMeterInformation.GetPeakValue(out result));
                return result;
            }
        }

        #endregion

        #region Конструктор

        internal AudioMeterInformation(IAudioMeterInformation realInterface)
        {
            int hardwareSupp;

            audioMeterInformation = realInterface;
            Marshal.ThrowExceptionForHR(audioMeterInformation.QueryHardwareSupport(out hardwareSupp));
            hardwareSupport = (EEndpointHardwareSupport)hardwareSupp;
            channels = new AudioMeterInformationChannels(audioMeterInformation);
        }

        #endregion
    }
}