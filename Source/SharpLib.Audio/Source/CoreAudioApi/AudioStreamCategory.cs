namespace NAudio.CoreAudioApi
{
    internal enum AudioStreamCategory
    {
        Other = 0,

        ForegroundOnlyMedia,

        BackgroundCapableMedia,

        Communications,

        Alerts,

        SoundEffects,

        GameEffects,

        GameMedia,
    }
}