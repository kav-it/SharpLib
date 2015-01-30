using System;
using System.IO;

namespace NAudio.Midi
{
    internal class TrackSequenceNumberEvent : MetaEvent
    {
        #region Поля

        private readonly ushort sequenceNumber;

        #endregion

        #region Конструктор

        public TrackSequenceNumberEvent(BinaryReader br, int length)
        {
            if (length != 2)
            {
                throw new FormatException("Invalid sequence number length");
            }
            sequenceNumber = (ushort)((br.ReadByte() << 8) + br.ReadByte());
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return String.Format("{0} {1}", base.ToString(), sequenceNumber);
        }

        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write((byte)((sequenceNumber >> 8) & 0xFF));
            writer.Write((byte)(sequenceNumber & 0xFF));
        }

        #endregion
    }
}