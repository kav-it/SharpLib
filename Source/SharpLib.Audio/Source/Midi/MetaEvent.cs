using System;
using System.IO;
using System.Text;

namespace NAudio.Midi
{
    internal class MetaEvent : MidiEvent
    {
        #region Поля

        private byte[] data;

        internal int metaDataLength;

        private MetaEventType metaEvent;

        #endregion

        #region Свойства

        public MetaEventType MetaEventType
        {
            get { return metaEvent; }
        }

        #endregion

        #region Конструктор

        protected MetaEvent()
        {
        }

        public MetaEvent(MetaEventType metaEventType, int metaDataLength, long absoluteTime)
            : base(absoluteTime, 1, MidiCommandCode.MetaEvent)
        {
            metaEvent = metaEventType;
            this.metaDataLength = metaDataLength;
        }

        #endregion

        #region Методы

        public static MetaEvent ReadMetaEvent(BinaryReader br)
        {
            MetaEventType metaEvent = (MetaEventType)br.ReadByte();
            int length = ReadVarInt(br);

            MetaEvent me = new MetaEvent();
            switch (metaEvent)
            {
                case MetaEventType.TrackSequenceNumber:
                    me = new TrackSequenceNumberEvent(br, length);
                    break;
                case MetaEventType.TextEvent:
                case MetaEventType.Copyright:
                case MetaEventType.SequenceTrackName:
                case MetaEventType.TrackInstrumentName:
                case MetaEventType.Lyric:
                case MetaEventType.Marker:
                case MetaEventType.CuePoint:
                case MetaEventType.ProgramName:
                case MetaEventType.DeviceName:
                    me = new TextEvent(br, length);
                    break;
                case MetaEventType.EndTrack:
                    if (length != 0)
                    {
                        throw new FormatException("End track length");
                    }
                    break;
                case MetaEventType.SetTempo:
                    me = new TempoEvent(br, length);
                    break;
                case MetaEventType.TimeSignature:
                    me = new TimeSignatureEvent(br, length);
                    break;
                case MetaEventType.KeySignature:
                    me = new KeySignatureEvent(br, length);
                    break;
                case MetaEventType.SequencerSpecific:
                    me = new SequencerSpecificEvent(br, length);
                    break;
                case MetaEventType.SmpteOffset:
                    me = new SmpteOffsetEvent(br, length);
                    break;
                default:

                    me.data = br.ReadBytes(length);
                    if (me.data.Length != length)
                    {
                        throw new FormatException("Failed to read metaevent's data fully");
                    }
                    break;
            }
            me.metaEvent = metaEvent;
            me.metaDataLength = length;

            return me;
        }

        public override string ToString()
        {
            if (data == null)
            {
                return String.Format("{0} {1}", AbsoluteTime, metaEvent);
            }
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
            {
                sb.AppendFormat("{0:X2} ", b);
            }
            return String.Format("{0} {1}\r\n{2}", AbsoluteTime, metaEvent, sb);
        }

        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write((byte)metaEvent);
            WriteVarInt(writer, metaDataLength);
            if (data != null)
            {
                writer.Write(data, 0, data.Length);
            }
        }

        #endregion
    }
}