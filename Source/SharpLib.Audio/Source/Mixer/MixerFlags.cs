using System;

namespace NAudio.Mixer
{
    [Flags]
    internal enum MixerFlags
    {
        #region Objects

        Handle = unchecked((int)0x80000000),

        Mixer = 0,

        MixerHandle = Mixer | Handle,

        WaveOut = 0x10000000,

        WaveOutHandle = WaveOut | Handle,

        WaveIn = 0x20000000,

        WaveInHandle = WaveIn | Handle,

        MidiOut = 0x30000000,

        MidiOutHandle = MidiOut | Handle,

        MidiIn = 0x40000000,

        MidiInHandle = MidiIn | Handle,

        Aux = 0x50000000,

        #endregion

        #region Get/Set control details

        Value = 0,

        ListText = 1,

        QueryMask = 0xF,

        #endregion

        #region get line controls

        All = 0,

        OneById = 1,

        OneByType = 2,

        #endregion

        GetLineInfoOfDestination = 0,

        GetLineInfoOfSource = 1,

        GetLineInfoOfLineId = 2,

        GetLineInfoOfComponentType = 3,

        GetLineInfoOfTargetType = 4,

        GetLineInfoOfQueryMask = 0xF,
    }
}