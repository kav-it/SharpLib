using System;
using System.IO;

namespace SharpLib.Audio.Midi
{
    internal class MidiEvent
    {
        #region Поля

        private long absoluteTime;

        private int channel;

        private MidiCommandCode commandCode;

        private int deltaTime;

        #endregion

        #region Свойства

        public virtual int Channel
        {
            get { return channel; }
            set
            {
                if ((value < 1) || (value > 16))
                {
                    throw new ArgumentOutOfRangeException("value", value,
                        String.Format("Channel must be 1-16 (Got {0})", value));
                }
                channel = value;
            }
        }

        public int DeltaTime
        {
            get { return deltaTime; }
        }

        public long AbsoluteTime
        {
            get { return absoluteTime; }
            set { absoluteTime = value; }
        }

        public MidiCommandCode CommandCode
        {
            get { return commandCode; }
        }

        #endregion

        #region Конструктор

        protected MidiEvent()
        {
        }

        public MidiEvent(long absoluteTime, int channel, MidiCommandCode commandCode)
        {
            this.absoluteTime = absoluteTime;
            Channel = channel;
            this.commandCode = commandCode;
        }

        #endregion

        #region Методы

        public static MidiEvent FromRawMessage(int rawMessage)
        {
            long absoluteTime = 0;
            int b = rawMessage & 0xFF;
            int data1 = (rawMessage >> 8) & 0xFF;
            int data2 = (rawMessage >> 16) & 0xFF;
            MidiCommandCode commandCode;
            int channel = 1;

            if ((b & 0xF0) == 0xF0)
            {
                commandCode = (MidiCommandCode)b;
            }
            else
            {
                commandCode = (MidiCommandCode)(b & 0xF0);
                channel = (b & 0x0F) + 1;
            }

            MidiEvent me;
            switch (commandCode)
            {
                case MidiCommandCode.NoteOn:
                case MidiCommandCode.NoteOff:
                case MidiCommandCode.KeyAfterTouch:
                    if (data2 > 0 && commandCode == MidiCommandCode.NoteOn)
                    {
                        me = new NoteOnEvent(absoluteTime, channel, data1, data2, 0);
                    }
                    else
                    {
                        me = new NoteEvent(absoluteTime, channel, commandCode, data1, data2);
                    }
                    break;
                case MidiCommandCode.ControlChange:
                    me = new ControlChangeEvent(absoluteTime, channel, (MidiController)data1, data2);
                    break;
                case MidiCommandCode.PatchChange:
                    me = new PatchChangeEvent(absoluteTime, channel, data1);
                    break;
                case MidiCommandCode.ChannelAfterTouch:
                    me = new ChannelAfterTouchEvent(absoluteTime, channel, data1);
                    break;
                case MidiCommandCode.PitchWheelChange:
                    me = new PitchWheelChangeEvent(absoluteTime, channel, data1 + (data2 << 7));
                    break;
                case MidiCommandCode.TimingClock:
                case MidiCommandCode.StartSequence:
                case MidiCommandCode.ContinueSequence:
                case MidiCommandCode.StopSequence:
                case MidiCommandCode.AutoSensing:
                    me = new MidiEvent(absoluteTime, channel, commandCode);
                    break;
                case MidiCommandCode.MetaEvent:
                case MidiCommandCode.Sysex:
                default:
                    throw new FormatException(String.Format("Unsupported MIDI Command Code for Raw Message {0}", commandCode));
            }
            return me;
        }

        public static MidiEvent ReadNextEvent(BinaryReader br, MidiEvent previous)
        {
            int deltaTime = MidiEvent.ReadVarInt(br);
            MidiCommandCode commandCode;
            int channel = 1;
            byte b = br.ReadByte();
            if ((b & 0x80) == 0)
            {
                commandCode = previous.CommandCode;
                channel = previous.Channel;
                br.BaseStream.Position--;
            }
            else
            {
                if ((b & 0xF0) == 0xF0)
                {
                    commandCode = (MidiCommandCode)b;
                }
                else
                {
                    commandCode = (MidiCommandCode)(b & 0xF0);
                    channel = (b & 0x0F) + 1;
                }
            }

            MidiEvent me;
            switch (commandCode)
            {
                case MidiCommandCode.NoteOn:
                    me = new NoteOnEvent(br);
                    break;
                case MidiCommandCode.NoteOff:
                case MidiCommandCode.KeyAfterTouch:
                    me = new NoteEvent(br);
                    break;
                case MidiCommandCode.ControlChange:
                    me = new ControlChangeEvent(br);
                    break;
                case MidiCommandCode.PatchChange:
                    me = new PatchChangeEvent(br);
                    break;
                case MidiCommandCode.ChannelAfterTouch:
                    me = new ChannelAfterTouchEvent(br);
                    break;
                case MidiCommandCode.PitchWheelChange:
                    me = new PitchWheelChangeEvent(br);
                    break;
                case MidiCommandCode.TimingClock:
                case MidiCommandCode.StartSequence:
                case MidiCommandCode.ContinueSequence:
                case MidiCommandCode.StopSequence:
                    me = new MidiEvent();
                    break;
                case MidiCommandCode.Sysex:
                    me = SysexEvent.ReadSysexEvent(br);
                    break;
                case MidiCommandCode.MetaEvent:
                    me = MetaEvent.ReadMetaEvent(br);
                    break;
                default:
                    throw new FormatException(String.Format("Unsupported MIDI Command Code {0:X2}", (byte)commandCode));
            }
            me.channel = channel;
            me.deltaTime = deltaTime;
            me.commandCode = commandCode;
            return me;
        }

        public virtual int GetAsShortMessage()
        {
            return (channel - 1) + (int)commandCode;
        }

        public static bool IsNoteOff(MidiEvent midiEvent)
        {
            if (midiEvent != null)
            {
                if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                {
                    NoteEvent ne = (NoteEvent)midiEvent;
                    return (ne.Velocity == 0);
                }
                return (midiEvent.CommandCode == MidiCommandCode.NoteOff);
            }
            return false;
        }

        public static bool IsNoteOn(MidiEvent midiEvent)
        {
            if (midiEvent != null)
            {
                if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                {
                    NoteEvent ne = (NoteEvent)midiEvent;
                    return (ne.Velocity > 0);
                }
            }
            return false;
        }

        public static bool IsEndTrack(MidiEvent midiEvent)
        {
            if (midiEvent != null)
            {
                MetaEvent me = midiEvent as MetaEvent;
                if (me != null)
                {
                    return me.MetaEventType == MetaEventType.EndTrack;
                }
            }
            return false;
        }

        public override string ToString()
        {
            if (commandCode >= MidiCommandCode.Sysex)
            {
                return String.Format("{0} {1}", absoluteTime, commandCode);
            }
            return String.Format("{0} {1} Ch: {2}", absoluteTime, commandCode, channel);
        }

        public static int ReadVarInt(BinaryReader br)
        {
            int value = 0;
            byte b;
            for (int n = 0; n < 4; n++)
            {
                b = br.ReadByte();
                value <<= 7;
                value += (b & 0x7F);
                if ((b & 0x80) == 0)
                {
                    return value;
                }
            }
            throw new FormatException("Invalid Var Int");
        }

        public static void WriteVarInt(BinaryWriter writer, int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value", value, "Cannot write a negative Var Int");
            }
            if (value > 0x0FFFFFFF)
            {
                throw new ArgumentOutOfRangeException("value", value, "Maximum allowed Var Int is 0x0FFFFFFF");
            }

            int n = 0;
            byte[] buffer = new byte[4];
            do
            {
                buffer[n++] = (byte)(value & 0x7F);
                value >>= 7;
            } while (value > 0);

            while (n > 0)
            {
                n--;
                if (n > 0)
                {
                    writer.Write((byte)(buffer[n] | 0x80));
                }
                else
                {
                    writer.Write(buffer[n]);
                }
            }
        }

        public virtual void Export(ref long absoluteTime, BinaryWriter writer)
        {
            if (this.absoluteTime < absoluteTime)
            {
                throw new FormatException("Can't export unsorted MIDI events");
            }
            WriteVarInt(writer, (int)(this.absoluteTime - absoluteTime));
            absoluteTime = this.absoluteTime;
            int output = (int)commandCode;
            if (commandCode != MidiCommandCode.MetaEvent)
            {
                output += (channel - 1);
            }
            writer.Write((byte)output);
        }

        #endregion
    }
}