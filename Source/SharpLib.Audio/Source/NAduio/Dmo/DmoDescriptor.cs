using System;

namespace SharpLib.Audio.Dmo
{
    internal class DmoDescriptor
    {
        #region Свойства

        public string Name { get; private set; }

        public Guid Clsid { get; private set; }

        #endregion

        #region Конструктор

        public DmoDescriptor(string name, Guid clsid)
        {
            Name = name;
            Clsid = clsid;
        }

        #endregion
    }
}