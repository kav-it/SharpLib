using System.Runtime.InteropServices;

using SharpLib.Audio.CoreAudioApi.Interfaces;

namespace SharpLib.Audio.CoreAudioApi
{
    internal class AudioEndpointVolumeStepInformation
    {
        #region Поля

        private readonly uint step;

        private readonly uint stepCount;

        #endregion

        #region Свойства

        public uint Step
        {
            get { return step; }
        }

        public uint StepCount
        {
            get { return stepCount; }
        }

        #endregion

        #region Конструктор

        internal AudioEndpointVolumeStepInformation(IAudioEndpointVolume parent)
        {
            Marshal.ThrowExceptionForHR(parent.GetVolumeStepInfo(out step, out stepCount));
        }

        #endregion
    }
}