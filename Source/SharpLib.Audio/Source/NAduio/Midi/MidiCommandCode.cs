namespace SharpLib.Audio.Midi
{
    internal enum MidiCommandCode : byte
    {
        NoteOff = 0x80,

        NoteOn = 0x90,

        KeyAfterTouch = 0xA0,

        ControlChange = 0xB0,

        PatchChange = 0xC0,

        ChannelAfterTouch = 0xD0,

        PitchWheelChange = 0xE0,

        Sysex = 0xF0,

        Eox = 0xF7,

        TimingClock = 0xF8,

        StartSequence = 0xFA,

        ContinueSequence = 0xFB,

        StopSequence = 0xFC,

        AutoSensing = 0xFE,

        MetaEvent = 0xFF,
    }
}