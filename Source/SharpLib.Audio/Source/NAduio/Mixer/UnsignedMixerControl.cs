using System;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.Mixer
{
    internal class UnsignedMixerControl : MixerControl
    {
        #region Поля

        private MixerInterop.MIXERCONTROLDETAILS_UNSIGNED[] unsignedDetails;

        #endregion

        #region Свойства

        public uint Value
        {
            get
            {
                GetControlDetails();
                return unsignedDetails[0].dwValue;
            }
            set
            {
                int structSize = Marshal.SizeOf(unsignedDetails[0]);

                mixerControlDetails.paDetails = Marshal.AllocHGlobal(structSize * nChannels);
                for (int channel = 0; channel < nChannels; channel++)
                {
                    unsignedDetails[channel].dwValue = value;
                    long pointer = mixerControlDetails.paDetails.ToInt64() + (structSize * channel);
                    Marshal.StructureToPtr(unsignedDetails[channel], (IntPtr)pointer, false);
                }
                MmException.Try(MixerInterop.mixerSetControlDetails(mixerHandle, ref mixerControlDetails, MixerFlags.Value | mixerHandleType), "mixerSetControlDetails");
                Marshal.FreeHGlobal(mixerControlDetails.paDetails);
            }
        }

        public UInt32 MinValue
        {
            get { return (uint)mixerControl.Bounds.minimum; }
        }

        public UInt32 MaxValue
        {
            get { return (uint)mixerControl.Bounds.maximum; }
        }

        public double Percent
        {
            get { return 100.0 * (Value - MinValue) / (MaxValue - MinValue); }
            set { Value = (uint)(MinValue + (value / 100.0) * (MaxValue - MinValue)); }
        }

        #endregion

        #region Конструктор

        internal UnsignedMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
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
            unsignedDetails = new MixerInterop.MIXERCONTROLDETAILS_UNSIGNED[nChannels];
            for (int channel = 0; channel < nChannels; channel++)
            {
                unsignedDetails[channel] = (MixerInterop.MIXERCONTROLDETAILS_UNSIGNED)Marshal.PtrToStructure(mixerControlDetails.paDetails, typeof(MixerInterop.MIXERCONTROLDETAILS_UNSIGNED));
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1}%", base.ToString(), Percent);
        }

        #endregion
    }
}