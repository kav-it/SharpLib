using System.Runtime.InteropServices;

using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
    internal class AudioEndpointVolumeStepInformation
    {
        #region ����

        private readonly uint step;

        private readonly uint stepCount;

        #endregion

        #region ��������

        public uint Step
        {
            get { return step; }
        }

        public uint StepCount
        {
            get { return stepCount; }
        }

        #endregion

        #region �����������

        internal AudioEndpointVolumeStepInformation(IAudioEndpointVolume parent)
        {
            Marshal.ThrowExceptionForHR(parent.GetVolumeStepInfo(out step, out stepCount));
        }

        #endregion
    }
}