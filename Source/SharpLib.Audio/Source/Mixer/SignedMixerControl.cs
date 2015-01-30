using System;
using System.Runtime.InteropServices;

namespace NAudio.Mixer
{
    internal class SignedMixerControl : MixerControl
    {
        #region Поля

        private MixerInterop.MIXERCONTROLDETAILS_SIGNED signedDetails;

        #endregion

        #region Свойства

        public int Value
        {
            get
            {
                GetControlDetails();
                return signedDetails.lValue;
            }
            set
            {
                signedDetails.lValue = value;
                mixerControlDetails.paDetails = Marshal.AllocHGlobal(Marshal.SizeOf(signedDetails));
                Marshal.StructureToPtr(signedDetails, mixerControlDetails.paDetails, false);
                MmException.Try(MixerInterop.mixerSetControlDetails(mixerHandle, ref mixerControlDetails, MixerFlags.Value | mixerHandleType), "mixerSetControlDetails");
                Marshal.FreeHGlobal(mixerControlDetails.paDetails);
            }
        }

        public int MinValue
        {
            get { return mixerControl.Bounds.minimum; }
        }

        public int MaxValue
        {
            get { return mixerControl.Bounds.maximum; }
        }

        public double Percent
        {
            get { return 100.0 * (Value - MinValue) / (MaxValue - MinValue); }
            set { Value = (int)(MinValue + (value / 100.0) * (MaxValue - MinValue)); }
        }

        #endregion

        #region Конструктор

        internal SignedMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
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
            signedDetails = (MixerInterop.MIXERCONTROLDETAILS_SIGNED)Marshal.PtrToStructure(mixerControlDetails.paDetails, typeof(MixerInterop.MIXERCONTROLDETAILS_SIGNED));
        }

        public override string ToString()
        {
            return String.Format("{0} {1}%", base.ToString(), Percent);
        }

        #endregion
    }
}