namespace SharpLib.Audio.Wave
{
    internal enum WaveCallbackStrategy
    {
        FunctionCallback,

        NewWindow,

        ExistingWindow,

        Event,
    }
}