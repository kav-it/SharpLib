using System;

namespace NAudio.Mixer
{
    internal class CustomMixerControl : MixerControl
    {
        #region �����������

        internal CustomMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
        {
            this.mixerControl = mixerControl;
            this.mixerHandle = mixerHandle;
            this.mixerHandleType = mixerHandleType;
            this.nChannels = nChannels;
            mixerControlDetails = new MixerInterop.MIXERCONTROLDETAILS();
            GetControlDetails();
        }

        #endregion

        #region ������

        protected override void GetDetails(IntPtr pDetails)
        {
        }

        #endregion
    }
}