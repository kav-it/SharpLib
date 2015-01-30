namespace NAudio.Mixer
{
    internal enum MixerLineComponentType
    {
        DestinationUndefined = 0,

        DestinationDigital = 1,

        DestinationLine = 2,

        DestinationMonitor = 3,

        DestinationSpeakers = 4,

        DestinationHeadphones = 5,

        DestinationTelephone = 6,

        DestinationWaveIn = 7,

        DestinationVoiceIn = 8,

        SourceUndefined = 0x1000,

        SourceDigital = 0x1001,

        SourceLine = 0x1002,

        SourceMicrophone = 0x1003,

        SourceSynthesizer = 0x1004,

        SourceCompactDisc = 0x1005,

        SourceTelephone = 0x1006,

        SourcePcSpeaker = 0x1007,

        SourceWaveOut = 0x1008,

        SourceAuxiliary = 0x1009,

        SourceAnalog = 0x100A,
    }
}