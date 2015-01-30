namespace SharpLib.Audio.Wave
{
    internal interface IWaveBuffer
    {
        #region ��������

        byte[] ByteBuffer { get; }

        float[] FloatBuffer { get; }

        short[] ShortBuffer { get; }

        int[] IntBuffer { get; }

        int MaxSize { get; }

        int ByteBufferCount { get; }

        int FloatBufferCount { get; }

        int ShortBufferCount { get; }

        int IntBufferCount { get; }

        #endregion
    }
}