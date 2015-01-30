using System.Runtime.InteropServices;

using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
    internal class AudioMeterInformationChannels
    {
        #region Поля

        private readonly IAudioMeterInformation audioMeterInformation;

        #endregion

        #region Свойства

        public int Count
        {
            get
            {
                int result;
                Marshal.ThrowExceptionForHR(audioMeterInformation.GetMeteringChannelCount(out result));
                return result;
            }
        }

        public float this[int index]
        {
            get
            {
                var peakValues = new float[Count];
                GCHandle Params = GCHandle.Alloc(peakValues, GCHandleType.Pinned);
                Marshal.ThrowExceptionForHR(audioMeterInformation.GetChannelsPeakValues(peakValues.Length, Params.AddrOfPinnedObject()));
                Params.Free();
                return peakValues[index];
            }
        }

        #endregion

        #region Конструктор

        internal AudioMeterInformationChannels(IAudioMeterInformation parent)
        {
            audioMeterInformation = parent;
        }

        #endregion
    }
}