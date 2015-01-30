namespace NAudio.Wave
{
    internal enum WaveCallbackStrategy
    {
        FunctionCallback,

        NewWindow,

        ExistingWindow,

        Event,
    }
}