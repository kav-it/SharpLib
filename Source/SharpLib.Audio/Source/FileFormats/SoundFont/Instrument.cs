namespace SharpLib.Audio.SoundFont
{
    internal class Instrument
    {
        #region Поля

        internal ushort endInstrumentZoneIndex;

        private string name;

        internal ushort startInstrumentZoneIndex;

        #endregion

        #region Свойства

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Zone[] Zones { get; set; }

        #endregion

        #region Методы

        public override string ToString()
        {
            return name;
        }

        #endregion
    }
}