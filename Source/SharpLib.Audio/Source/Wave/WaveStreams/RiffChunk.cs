using System;
using System.Text;

namespace NAudio.Wave
{
    internal class RiffChunk
    {
        #region Свойства

        public int Identifier { get; private set; }

        public string IdentifierAsString
        {
            get { return Encoding.UTF8.GetString(BitConverter.GetBytes(Identifier)); }
        }

        public int Length { get; private set; }

        public long StreamPosition { get; private set; }

        #endregion

        #region Конструктор

        public RiffChunk(int identifier, int length, long streamPosition)
        {
            Identifier = identifier;
            Length = length;
            StreamPosition = streamPosition;
        }

        #endregion
    }
}