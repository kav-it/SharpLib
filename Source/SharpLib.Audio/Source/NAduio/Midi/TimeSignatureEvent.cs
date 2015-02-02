using System;
using System.IO;

namespace SharpLib.Audio.Midi
{
    internal class TimeSignatureEvent : MetaEvent
    {
        #region Поля

        private readonly byte denominator;

        private readonly byte no32ndNotesInQuarterNote;

        private readonly byte numerator;

        private readonly byte ticksInMetronomeClick;

        #endregion

        #region Свойства

        public int Numerator
        {
            get { return numerator; }
        }

        public int Denominator
        {
            get { return denominator; }
        }

        public int TicksInMetronomeClick
        {
            get { return ticksInMetronomeClick; }
        }

        public int No32ndNotesInQuarterNote
        {
            get { return no32ndNotesInQuarterNote; }
        }

        public string TimeSignature
        {
            get
            {
                string den = String.Format("Unknown ({0})", denominator);
                switch (denominator)
                {
                    case 1:
                        den = "2";
                        break;
                    case 2:
                        den = "4";
                        break;
                    case 3:
                        den = "8";
                        break;
                    case 4:
                        den = "16";
                        break;
                    case 5:
                        den = "32";
                        break;
                }
                return String.Format("{0}/{1}", numerator, den);
            }
        }

        #endregion

        #region Конструктор

        public TimeSignatureEvent(BinaryReader br, int length)
        {
            if (length != 4)
            {
                throw new FormatException(String.Format("Invalid time signature length: Got {0}, expected 4", length));
            }
            numerator = br.ReadByte();
            denominator = br.ReadByte();
            ticksInMetronomeClick = br.ReadByte();
            no32ndNotesInQuarterNote = br.ReadByte();
        }

        public TimeSignatureEvent(long absoluteTime, int numerator, int denominator, int ticksInMetronomeClick, int no32ndNotesInQuarterNote)
            :
                base(MetaEventType.TimeSignature, 4, absoluteTime)
        {
            this.numerator = (byte)numerator;
            this.denominator = (byte)denominator;
            this.ticksInMetronomeClick = (byte)ticksInMetronomeClick;
            this.no32ndNotesInQuarterNote = (byte)no32ndNotesInQuarterNote;
        }

        [Obsolete("Use the constructor that has absolute time first")]
        public TimeSignatureEvent(int numerator, int denominator, int ticksInMetronomeClick, int no32ndNotesInQuarterNote, long absoluteTime)
            : base(MetaEventType.TimeSignature, 4, absoluteTime)
        {
            this.numerator = (byte)numerator;
            this.denominator = (byte)denominator;
            this.ticksInMetronomeClick = (byte)ticksInMetronomeClick;
            this.no32ndNotesInQuarterNote = (byte)no32ndNotesInQuarterNote;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return String.Format("{0} {1} TicksInClick:{2} 32ndsInQuarterNote:{3}",
                base.ToString(), TimeSignature, ticksInMetronomeClick, no32ndNotesInQuarterNote);
        }

        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write(numerator);
            writer.Write(denominator);
            writer.Write(ticksInMetronomeClick);
            writer.Write(no32ndNotesInQuarterNote);
        }

        #endregion
    }
}