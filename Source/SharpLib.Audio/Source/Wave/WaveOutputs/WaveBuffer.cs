using System;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.Wave
{
    [StructLayout(LayoutKind.Explicit, Pack = 2)]
    internal class WaveBuffer : IWaveBuffer
    {
        [FieldOffset(0)]
        public int numberOfBytes;

        [FieldOffset(8)]
        private byte[] byteBuffer;

        [FieldOffset(8)]
        private float[] floatBuffer;

        [FieldOffset(8)]
        private short[] shortBuffer;

        [FieldOffset(8)]
        private int[] intBuffer;

        public WaveBuffer(int sizeToAllocateInBytes)
        {
            int aligned4Bytes = sizeToAllocateInBytes % 4;
            sizeToAllocateInBytes = (aligned4Bytes == 0) ? sizeToAllocateInBytes : sizeToAllocateInBytes + 4 - aligned4Bytes;

            byteBuffer = new byte[sizeToAllocateInBytes];
            numberOfBytes = 0;
        }

        public WaveBuffer(byte[] bufferToBoundTo)
        {
            BindTo(bufferToBoundTo);
        }

        public void BindTo(byte[] bufferToBoundTo)
        {
            byteBuffer = bufferToBoundTo;
            numberOfBytes = 0;
        }

        public static implicit operator byte[](WaveBuffer waveBuffer)
        {
            return waveBuffer.byteBuffer;
        }

        public static implicit operator float[](WaveBuffer waveBuffer)
        {
            return waveBuffer.floatBuffer;
        }

        public static implicit operator int[](WaveBuffer waveBuffer)
        {
            return waveBuffer.intBuffer;
        }

        public static implicit operator short[](WaveBuffer waveBuffer)
        {
            return waveBuffer.shortBuffer;
        }

        public byte[] ByteBuffer
        {
            get { return byteBuffer; }
        }

        public float[] FloatBuffer
        {
            get { return floatBuffer; }
        }

        public short[] ShortBuffer
        {
            get { return shortBuffer; }
        }

        public int[] IntBuffer
        {
            get { return intBuffer; }
        }

        public int MaxSize
        {
            get { return byteBuffer.Length; }
        }

        public int ByteBufferCount
        {
            get { return numberOfBytes; }
            set { numberOfBytes = CheckValidityCount("ByteBufferCount", value, 1); }
        }

        public int FloatBufferCount
        {
            get { return numberOfBytes / 4; }
            set { numberOfBytes = CheckValidityCount("FloatBufferCount", value, 4); }
        }

        public int ShortBufferCount
        {
            get { return numberOfBytes / 2; }
            set { numberOfBytes = CheckValidityCount("ShortBufferCount", value, 2); }
        }

        public int IntBufferCount
        {
            get { return numberOfBytes / 4; }
            set { numberOfBytes = CheckValidityCount("IntBufferCount", value, 4); }
        }

        public void Clear()
        {
            Array.Clear(byteBuffer, 0, byteBuffer.Length);
        }

        public void Copy(Array destinationArray)
        {
            Array.Copy(byteBuffer, destinationArray, numberOfBytes);
        }

        private int CheckValidityCount(string argName, int value, int sizeOfValue)
        {
            int newNumberOfBytes = value * sizeOfValue;
            if ((newNumberOfBytes % 4) != 0)
            {
                throw new ArgumentOutOfRangeException(argName, String.Format("{0} cannot set a count ({1}) that is not 4 bytes aligned ", argName, newNumberOfBytes));
            }

            if (value < 0 || value > (byteBuffer.Length / sizeOfValue))
            {
                throw new ArgumentOutOfRangeException(argName, String.Format("{0} cannot set a count that exceed max count {1}", argName, byteBuffer.Length / sizeOfValue));
            }
            return newNumberOfBytes;
        }
    }
}