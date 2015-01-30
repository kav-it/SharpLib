using System.IO;
using System.Text;

namespace SharpLib.Audio.Midi
{
    internal class SequencerSpecificEvent : MetaEvent
    {
        #region Поля

        private byte[] data;

        #endregion

        #region Свойства

        public byte[] Data
        {
            get { return data; }
            set
            {
                data = value;
                metaDataLength = data.Length;
            }
        }

        #endregion

        #region Конструктор

        public SequencerSpecificEvent(BinaryReader br, int length)
        {
            data = br.ReadBytes(length);
        }

        public SequencerSpecificEvent(byte[] data, long absoluteTime)
            : base(MetaEventType.SequencerSpecific, data.Length, absoluteTime)
        {
            this.data = data;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append(" ");
            foreach (var b in data)
            {
                sb.AppendFormat("{0:X2} ", b);
            }
            sb.Length--;
            return sb.ToString();
        }

        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write(data);
        }

        #endregion
    }
}