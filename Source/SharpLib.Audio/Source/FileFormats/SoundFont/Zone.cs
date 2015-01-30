using System;

namespace SharpLib.Audio.SoundFont
{
    internal class Zone
    {
        #region Поля

        internal ushort generatorCount;

        internal ushort generatorIndex;

        internal ushort modulatorCount;

        internal ushort modulatorIndex;

        #endregion

        #region Свойства

        public Modulator[] Modulators { get; set; }

        public Generator[] Generators { get; set; }

        #endregion

        #region Методы

        public override string ToString()
        {
            return String.Format("Zone {0} Gens:{1} {2} Mods:{3}", generatorCount, generatorIndex,
                modulatorCount, modulatorIndex);
        }

        #endregion
    }
}