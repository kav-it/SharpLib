using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NAudio.Mixer
{
    internal class Mixer
    {
        #region Поля

        private readonly IntPtr mixerHandle;

        private readonly MixerFlags mixerHandleType;

        private MixerInterop.MIXERCAPS caps;

        #endregion

        #region Свойства

        public static int NumberOfDevices
        {
            get { return MixerInterop.mixerGetNumDevs(); }
        }

        public int DestinationCount
        {
            get { return (int)caps.cDestinations; }
        }

        public String Name
        {
            get { return caps.szPname; }
        }

        public Manufacturers Manufacturer
        {
            get { return (Manufacturers)caps.wMid; }
        }

        public int ProductID
        {
            get { return caps.wPid; }
        }

        public IEnumerable<MixerLine> Destinations
        {
            get
            {
                for (int destination = 0; destination < DestinationCount; destination++)
                {
                    yield return GetDestination(destination);
                }
            }
        }

        public static IEnumerable<Mixer> Mixers
        {
            get
            {
                for (int device = 0; device < Mixer.NumberOfDevices; device++)
                {
                    yield return new Mixer(device);
                }
            }
        }

        #endregion

        #region Конструктор

        public Mixer(int mixerIndex)
        {
            if (mixerIndex < 0 || mixerIndex >= NumberOfDevices)
            {
                throw new ArgumentOutOfRangeException("mixerID");
            }
            caps = new MixerInterop.MIXERCAPS();
            MmException.Try(MixerInterop.mixerGetDevCaps((IntPtr)mixerIndex, ref caps, Marshal.SizeOf(caps)), "mixerGetDevCaps");
            mixerHandle = (IntPtr)mixerIndex;
            mixerHandleType = MixerFlags.Mixer;
        }

        #endregion

        #region Методы

        public MixerLine GetDestination(int destinationIndex)
        {
            if (destinationIndex < 0 || destinationIndex >= DestinationCount)
            {
                throw new ArgumentOutOfRangeException("destinationIndex");
            }
            return new MixerLine(mixerHandle, destinationIndex, mixerHandleType);
        }

        #endregion
    }
}