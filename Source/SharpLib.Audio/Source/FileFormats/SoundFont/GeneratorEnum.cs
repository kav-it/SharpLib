namespace SharpLib.Audio.SoundFont
{
    internal enum GeneratorEnum
    {
        StartAddressOffset = 0,

        EndAddressOffset,

        StartLoopAddressOffset,

        EndLoopAddressOffset,

        StartAddressCoarseOffset,

        ModulationLFOToPitch,

        VibratoLFOToPitch,

        ModulationEnvelopeToPitch,

        InitialFilterCutoffFrequency,

        InitialFilterQ,

        ModulationLFOToFilterCutoffFrequency,

        ModulationEnvelopeToFilterCutoffFrequency,

        EndAddressCoarseOffset,

        ModulationLFOToVolume,

        Unused1,

        ChorusEffectsSend,

        ReverbEffectsSend,

        Pan,

        Unused2,

        Unused3,

        Unused4,

        DelayModulationLFO,

        FrequencyModulationLFO,

        DelayVibratoLFO,

        FrequencyVibratoLFO,

        DelayModulationEnvelope,

        AttackModulationEnvelope,

        HoldModulationEnvelope,

        DecayModulationEnvelope,

        SustainModulationEnvelope,

        ReleaseModulationEnvelope,

        KeyNumberToModulationEnvelopeHold,

        KeyNumberToModulationEnvelopeDecay,

        DelayVolumeEnvelope,

        AttackVolumeEnvelope,

        HoldVolumeEnvelope,

        DecayVolumeEnvelope,

        SustainVolumeEnvelope,

        ReleaseVolumeEnvelope,

        KeyNumberToVolumeEnvelopeHold,

        KeyNumberToVolumeEnvelopeDecay,

        Instrument,

        Reserved1,

        KeyRange,

        VelocityRange,

        StartLoopAddressCoarseOffset,

        KeyNumber,

        Velocity,

        InitialAttenuation,

        Reserved2,

        EndLoopAddressCoarseOffset,

        CoarseTune,

        FineTune,

        SampleID,

        SampleModes,

        Reserved3,

        ScaleTuning,

        ExclusiveClass,

        OverridingRootKey,

        Unused5,

        UnusedEnd
    }
}