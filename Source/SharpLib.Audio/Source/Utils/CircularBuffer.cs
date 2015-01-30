using System;
using System.Diagnostics;

namespace SharpLib.Audio.Utils
{
    internal class CircularBuffer
    {
        #region Поля

        private readonly byte[] buffer;

        private readonly object lockObject;

        private int byteCount;

        private int readPosition;

        private int writePosition;

        #endregion

        #region Свойства

        public int MaxLength
        {
            get { return buffer.Length; }
        }

        public int Count
        {
            get { return byteCount; }
        }

        #endregion

        #region Конструктор

        public CircularBuffer(int size)
        {
            buffer = new byte[size];
            lockObject = new object();
        }

        #endregion

        #region Методы

        public int Write(byte[] data, int offset, int count)
        {
            lock (lockObject)
            {
                var bytesWritten = 0;
                if (count > buffer.Length - byteCount)
                {
                    count = buffer.Length - byteCount;
                }

                int writeToEnd = Math.Min(buffer.Length - writePosition, count);
                Array.Copy(data, offset, buffer, writePosition, writeToEnd);
                writePosition += writeToEnd;
                writePosition %= buffer.Length;
                bytesWritten += writeToEnd;
                if (bytesWritten < count)
                {
                    Debug.Assert(writePosition == 0);

                    Array.Copy(data, offset + bytesWritten, buffer, writePosition, count - bytesWritten);
                    writePosition += (count - bytesWritten);
                    bytesWritten = count;
                }
                byteCount += bytesWritten;
                return bytesWritten;
            }
        }

        public int Read(byte[] data, int offset, int count)
        {
            lock (lockObject)
            {
                if (count > byteCount)
                {
                    count = byteCount;
                }
                int bytesRead = 0;
                int readToEnd = Math.Min(buffer.Length - readPosition, count);
                Array.Copy(buffer, readPosition, data, offset, readToEnd);
                bytesRead += readToEnd;
                readPosition += readToEnd;
                readPosition %= buffer.Length;

                if (bytesRead < count)
                {
                    Debug.Assert(readPosition == 0);
                    Array.Copy(buffer, readPosition, data, offset + bytesRead, count - bytesRead);
                    readPosition += (count - bytesRead);
                    bytesRead = count;
                }

                byteCount -= bytesRead;
                Debug.Assert(byteCount >= 0);
                return bytesRead;
            }
        }

        public void Reset()
        {
            byteCount = 0;
            readPosition = 0;
            writePosition = 0;
        }

        public void Advance(int count)
        {
            if (count >= byteCount)
            {
                Reset();
            }
            else
            {
                byteCount -= count;
                readPosition += count;
                readPosition %= MaxLength;
            }
        }

        #endregion
    }
}