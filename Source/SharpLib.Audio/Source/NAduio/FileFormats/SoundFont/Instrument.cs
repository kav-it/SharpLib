namespace SharpLib.Audio.SoundFont
{
    internal class Instrument
    {
        #region ����

        internal ushort endInstrumentZoneIndex;

        private string name;

        internal ushort startInstrumentZoneIndex;

        #endregion

        #region ��������

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Zone[] Zones { get; set; }

        #endregion

        #region ������

        public override string ToString()
        {
            return name;
        }

        #endregion
    }
}