using System;
using System.IO;

namespace NAudio.Midi
{
    internal class KeySignatureEvent : MetaEvent
    {
        #region Поля

        private readonly byte majorMinor;

        private readonly byte sharpsFlats;

        #endregion

        #region Свойства

        public int SharpsFlats
        {
            get { return sharpsFlats; }
        }

        public int MajorMinor
        {
            get { return majorMinor; }
        }

        #endregion

        #region Конструктор

        public KeySignatureEvent(BinaryReader br, int length)
        {
            if (length != 2)
            {
                throw new FormatException("Invalid key signature length");
            }
            sharpsFlats = br.ReadByte();
            majorMinor = br.ReadByte();
        }

        public KeySignatureEvent(int sharpsFlats, int majorMinor, long absoluteTime)
            : base(MetaEventType.KeySignature, 2, absoluteTime)
        {
            this.sharpsFlats = (byte)sharpsFlats;
            this.majorMinor = (byte)majorMinor;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", base.ToString(), sharpsFlats, majorMinor);
        }

        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write(sharpsFlats);
            writer.Write(majorMinor);
        }

        #endregion
    }
}