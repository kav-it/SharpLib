using System;
using System.IO;

namespace SharpLib.Audio.Midi
{
    internal class PitchWheelChangeEvent : MidiEvent
    {
        #region Поля

        private int pitch;

        #endregion

        #region Свойства

        public int Pitch
        {
            get { return pitch; }
            set
            {
                if (value < 0 || value > 0x4000)
                {
                    throw new ArgumentOutOfRangeException("value", "Pitch value must be in the range 0 - 0x4000");
                }
                pitch = value;
            }
        }

        #endregion

        #region Конструктор

        public PitchWheelChangeEvent(BinaryReader br)
        {
            byte b1 = br.ReadByte();
            byte b2 = br.ReadByte();
            if ((b1 & 0x80) != 0)
            {
                throw new FormatException("Invalid pitchwheelchange byte 1");
            }
            if ((b2 & 0x80) != 0)
            {
                throw new FormatException("Invalid pitchwheelchange byte 2");
            }

            pitch = b1 + (b2 << 7);
        }

        public PitchWheelChangeEvent(long absoluteTime, int channel, int pitchWheel)
            : base(absoluteTime, channel, MidiCommandCode.PitchWheelChange)
        {
            Pitch = pitchWheel;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return String.Format("{0} Pitch {1} ({2})",
                base.ToString(),
                pitch,
                pitch - 0x2000);
        }

        public override int GetAsShortMessage()
        {
            return base.GetAsShortMessage() + ((pitch & 0x7f) << 8) + (((pitch >> 7) & 0x7f) << 16);
        }

        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write((byte)(pitch & 0x7f));
            writer.Write((byte)((pitch >> 7) & 0x7f));
        }

        #endregion
    }
}