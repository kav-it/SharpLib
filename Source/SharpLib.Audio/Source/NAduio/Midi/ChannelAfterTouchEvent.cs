using System;
using System.IO;

namespace SharpLib.Audio.Midi
{
    internal class ChannelAfterTouchEvent : MidiEvent
    {
        #region Поля

        private byte afterTouchPressure;

        #endregion

        #region Свойства

        public int AfterTouchPressure
        {
            get { return afterTouchPressure; }
            set
            {
                if (value < 0 || value > 127)
                {
                    throw new ArgumentOutOfRangeException("value", "After touch pressure must be in the range 0-127");
                }
                afterTouchPressure = (byte)value;
            }
        }

        #endregion

        #region Конструктор

        public ChannelAfterTouchEvent(BinaryReader br)
        {
            afterTouchPressure = br.ReadByte();
            if ((afterTouchPressure & 0x80) != 0)
            {
                throw new FormatException("Invalid afterTouchPressure");
            }
        }

        public ChannelAfterTouchEvent(long absoluteTime, int channel, int afterTouchPressure)
            : base(absoluteTime, channel, MidiCommandCode.ChannelAfterTouch)
        {
            AfterTouchPressure = afterTouchPressure;
        }

        #endregion

        #region Методы

        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write(afterTouchPressure);
        }

        #endregion
    }
}