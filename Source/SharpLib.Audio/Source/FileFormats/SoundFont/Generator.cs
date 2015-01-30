using System;

namespace SharpLib.Audio.SoundFont
{
    internal class Generator
    {
        #region Поля

        private GeneratorEnum generatorType;

        private Instrument instrument;

        private ushort rawAmount;

        private SampleHeader sampleHeader;

        #endregion

        #region Свойства

        public GeneratorEnum GeneratorType
        {
            get { return generatorType; }
            set { generatorType = value; }
        }

        public ushort UInt16Amount
        {
            get { return rawAmount; }
            set { rawAmount = value; }
        }

        public short Int16Amount
        {
            get { return (short)rawAmount; }
            set { rawAmount = (ushort)value; }
        }

        public byte LowByteAmount
        {
            get { return (byte)(rawAmount & 0x00FF); }
            set
            {
                rawAmount &= 0xFF00;
                rawAmount += value;
            }
        }

        public byte HighByteAmount
        {
            get { return (byte)((rawAmount & 0xFF00) >> 8); }
            set
            {
                rawAmount &= 0x00FF;
                rawAmount += (ushort)(value << 8);
            }
        }

        public Instrument Instrument
        {
            get { return instrument; }
            set { instrument = value; }
        }

        public SampleHeader SampleHeader
        {
            get { return sampleHeader; }
            set { sampleHeader = value; }
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            if (generatorType == GeneratorEnum.Instrument)
            {
                return String.Format("Generator Instrument {0}", instrument.Name);
            }
            if (generatorType == GeneratorEnum.SampleID)
            {
                return String.Format("Generator SampleID {0}", sampleHeader);
            }
            return String.Format("Generator {0} {1}", generatorType, rawAmount);
        }

        #endregion
    }
}