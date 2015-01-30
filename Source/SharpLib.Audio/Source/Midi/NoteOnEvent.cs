using System;
using System.IO;

namespace NAudio.Midi
{
    internal class NoteOnEvent : NoteEvent
    {
        #region Поля

        private NoteEvent offEvent;

        #endregion

        #region Свойства

        public NoteEvent OffEvent
        {
            get { return offEvent; }
            set
            {
                if (!MidiEvent.IsNoteOff(value))
                {
                    throw new ArgumentException("OffEvent must be a valid MIDI note off event");
                }
                if (value.NoteNumber != NoteNumber)
                {
                    throw new ArgumentException("Note Off Event must be for the same note number");
                }
                if (value.Channel != Channel)
                {
                    throw new ArgumentException("Note Off Event must be for the same channel");
                }
                offEvent = value;
            }
        }

        public override int NoteNumber
        {
            get { return base.NoteNumber; }
            set
            {
                base.NoteNumber = value;
                if (OffEvent != null)
                {
                    OffEvent.NoteNumber = NoteNumber;
                }
            }
        }

        public override int Channel
        {
            get { return base.Channel; }
            set
            {
                base.Channel = value;
                if (OffEvent != null)
                {
                    OffEvent.Channel = Channel;
                }
            }
        }

        public int NoteLength
        {
            get { return (int)(offEvent.AbsoluteTime - AbsoluteTime); }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("NoteLength must be 0 or greater");
                }
                offEvent.AbsoluteTime = AbsoluteTime + value;
            }
        }

        #endregion

        #region Конструктор

        public NoteOnEvent(BinaryReader br)
            : base(br)
        {
        }

        public NoteOnEvent(long absoluteTime,
            int channel,
            int noteNumber,
            int velocity,
            int duration)
            : base(absoluteTime, channel, MidiCommandCode.NoteOn, noteNumber, velocity)
        {
            OffEvent = new NoteEvent(absoluteTime, channel, MidiCommandCode.NoteOff,
                noteNumber, 0);
            NoteLength = duration;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            if ((Velocity == 0) && (OffEvent == null))
            {
                return String.Format("{0} (Note Off)",
                    base.ToString());
            }
            return String.Format("{0} Len: {1}",
                base.ToString(),
                (OffEvent == null) ? "?" : NoteLength.ToString());
        }

        #endregion
    }
}