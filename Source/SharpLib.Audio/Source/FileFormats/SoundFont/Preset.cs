using System;

namespace NAudio.SoundFont
{
    internal class Preset
    {
        #region Поля

        private ushort bank;

        internal ushort endPresetZoneIndex;

        internal uint genre;

        internal uint library;

        internal uint morphology;

        private string name;

        private ushort patchNumber;

        internal ushort startPresetZoneIndex;

        #endregion

        #region Свойства

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public ushort PatchNumber
        {
            get { return patchNumber; }
            set { patchNumber = value; }
        }

        public ushort Bank
        {
            get { return bank; }
            set { bank = value; }
        }

        public Zone[] Zones { get; set; }

        #endregion

        #region Методы

        public override string ToString()
        {
            return String.Format("{0}-{1} {2}", bank, patchNumber, name);
        }

        #endregion
    }
}