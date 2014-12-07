using System;

namespace SharpLib.Json
{
    internal class StringBuffer
    {
        #region Поля

        private static readonly char[] _emptyBuffer = new char[0];

        private char[] _buffer;

        #endregion

        #region Свойства

        public int Position { get; set; }

        #endregion

        #region Конструктор

        public StringBuffer()
        {
            _buffer = _emptyBuffer;
        }

        public StringBuffer(int initalSize)
        {
            _buffer = new char[initalSize];
        }

        #endregion

        #region Методы

        public void Append(char value)
        {
            if (Position == _buffer.Length)
            {
                EnsureSize(1);
            }

            _buffer[Position++] = value;
        }

        public void Append(char[] buffer, int startIndex, int count)
        {
            if (Position + count >= _buffer.Length)
            {
                EnsureSize(count);
            }

            Array.Copy(buffer, startIndex, _buffer, Position, count);

            Position += count;
        }

        public void Clear()
        {
            _buffer = _emptyBuffer;
            Position = 0;
        }

        private void EnsureSize(int appendLength)
        {
            char[] newBuffer = new char[(Position + appendLength) * 2];

            Array.Copy(_buffer, newBuffer, Position);

            _buffer = newBuffer;
        }

        public override string ToString()
        {
            return ToString(0, Position);
        }

        public string ToString(int start, int length)
        {
            return new string(_buffer, start, length);
        }

        public char[] GetInternalBuffer()
        {
            return _buffer;
        }

        #endregion
    }
}