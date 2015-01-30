namespace NAudio.Midi
{
    internal enum MetaEventType : byte
    {
        TrackSequenceNumber = 0x00,

        TextEvent = 0x01,

        Copyright = 0x02,

        SequenceTrackName = 0x03,

        TrackInstrumentName = 0x04,

        Lyric = 0x05,

        Marker = 0x06,

        CuePoint = 0x07,

        ProgramName = 0x08,

        DeviceName = 0x09,

        MidiChannel = 0x20,

        MidiPort = 0x21,

        EndTrack = 0x2F,

        SetTempo = 0x51,

        SmpteOffset = 0x54,

        TimeSignature = 0x58,

        KeySignature = 0x59,

        SequencerSpecific = 0x7F,
    }
}