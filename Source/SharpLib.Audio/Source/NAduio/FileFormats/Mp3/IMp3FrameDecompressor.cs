using System;

namespace SharpLib.Audio.Wave
{
    internal interface IMp3FrameDecompressor : IDisposable
    {
        #region Свойства

        WaveFormat OutputFormat { get; }

        #endregion

        #region Методы

        int DecompressFrame(Mp3Frame frame, byte[] dest, int destOffset);

        void Reset();

        #endregion
    }
}