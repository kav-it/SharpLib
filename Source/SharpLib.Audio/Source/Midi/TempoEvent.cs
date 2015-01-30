using System;
using System.IO;

namespace SharpLib.Audio.Midi
{
    internal class TempoEvent : MetaEvent
    {
        #region Поля

        private int microsecondsPerQuarterNote;

        #endregion

        #region Свойства

        public int MicrosecondsPerQuarterNote
        {
            get { return microsecondsPerQuarterNote; }
            set { microsecondsPerQuarterNote = value; }
        }

        public double Tempo
        {
            get { return (60000000.0 / microsecondsPerQuarterNote); }
            set { microsecondsPerQuarterNote = (int)(60000000.0 / value); }
        }

        #endregion

        #region Конструктор

        public TempoEvent(BinaryReader br, int length)
        {
            if (length != 3)
            {
                throw new FormatException("Invalid tempo length");
            }
            microsecondsPerQuarterNote = (br.ReadByte() << 16) + (br.ReadByte() << 8) + br.ReadByte();
        }

        public TempoEvent(int microsecondsPerQuarterNote, long absoluteTime)
            : base(MetaEventType.SetTempo, 3, absoluteTime)
        {
            this.microsecondsPerQuarterNote = microsecondsPerQuarterNote;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return String.Format("{0} {2}bpm ({1})",
                base.ToString(),
                microsecondsPerQuarterNote,
                (60000000 / microsecondsPerQuarterNote));
        }

        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write((byte)((microsecondsPerQuarterNote >> 16) & 0xFF));
            writer.Write((byte)((microsecondsPerQuarterNote >> 8) & 0xFF));
            writer.Write((byte)(microsecondsPerQuarterNote & 0xFF));
        }

        #endregion
    }
}