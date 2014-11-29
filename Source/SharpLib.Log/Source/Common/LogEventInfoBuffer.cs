using System;

namespace NLog.Common
{
    public class LogEventInfoBuffer
    {
        #region Поля

        private readonly bool growAsNeeded;

        private readonly int growLimit;

        private AsyncLogEventInfo[] buffer;

        private int count;

        private int getPointer;

        private int putPointer;

        #endregion

        #region Свойства

        public int Size
        {
            get { return buffer.Length; }
        }

        #endregion

        #region Конструктор

        public LogEventInfoBuffer(int size, bool growAsNeeded, int growLimit)
        {
            this.growAsNeeded = growAsNeeded;
            buffer = new AsyncLogEventInfo[size];
            this.growLimit = growLimit;
            getPointer = 0;
            putPointer = 0;
        }

        #endregion

        #region Методы

        public int Append(AsyncLogEventInfo eventInfo)
        {
            lock (this)
            {
                if (count >= buffer.Length)
                {
                    if (growAsNeeded && buffer.Length < growLimit)
                    {
                        int newLength = buffer.Length * 2;
                        if (newLength >= growLimit)
                        {
                            newLength = growLimit;
                        }

                        var newBuffer = new AsyncLogEventInfo[newLength];
                        Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
                        buffer = newBuffer;
                    }
                    else
                    {
                        getPointer = getPointer + 1;
                    }
                }

                putPointer = putPointer % buffer.Length;
                buffer[putPointer] = eventInfo;
                putPointer = putPointer + 1;
                count++;
                if (count >= buffer.Length)
                {
                    count = buffer.Length;
                }

                return count;
            }
        }

        public AsyncLogEventInfo[] GetEventsAndClear()
        {
            lock (this)
            {
                int cnt = count;
                var returnValue = new AsyncLogEventInfo[cnt];

                for (int i = 0; i < cnt; ++i)
                {
                    int p = (getPointer + i) % buffer.Length;
                    var e = buffer[p];
                    buffer[p] = default(AsyncLogEventInfo);
                    returnValue[i] = e;
                }

                count = 0;
                getPointer = 0;
                putPointer = 0;

                return returnValue;
            }
        }

        #endregion
    }
}