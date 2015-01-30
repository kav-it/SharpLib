using System;
using System.Runtime.InteropServices;

namespace NAudio.Mixer
{
    internal class BooleanMixerControl : MixerControl
    {
        #region Поля

        private MixerInterop.MIXERCONTROLDETAILS_BOOLEAN boolDetails;

        #endregion

        #region Свойства

        public bool Value
        {
            get
            {
                GetControlDetails();
                return (boolDetails.fValue == 1);
            }
            set
            {
                boolDetails.fValue = (value) ? 1 : 0;
                mixerControlDetails.paDetails = Marshal.AllocHGlobal(Marshal.SizeOf(boolDetails));
                Marshal.StructureToPtr(boolDetails, mixerControlDetails.paDetails, false);
                MmException.Try(MixerInterop.mixerSetControlDetails(mixerHandle, ref mixerControlDetails, MixerFlags.Value | mixerHandleType), "mixerSetControlDetails");
                Marshal.FreeHGlobal(mixerControlDetails.paDetails);
            }
        }

        #endregion

        #region Конструктор

        internal BooleanMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
        {
            this.mixerControl = mixerControl;
            this.mixerHandle = mixerHandle;
            this.mixerHandleType = mixerHandleType;
            this.nChannels = nChannels;
            mixerControlDetails = new MixerInterop.MIXERCONTROLDETAILS();

            GetControlDetails();
        }

        #endregion

        #region Методы

        protected override void GetDetails(IntPtr pDetails)
        {
            boolDetails = (MixerInterop.MIXERCONTROLDETAILS_BOOLEAN)Marshal.PtrToStructure(pDetails, typeof(MixerInterop.MIXERCONTROLDETAILS_BOOLEAN));
        }

        #endregion
    }
}