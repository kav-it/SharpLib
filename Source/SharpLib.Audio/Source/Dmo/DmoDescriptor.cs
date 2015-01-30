using System;

namespace NAudio.Dmo
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