
using System;

namespace SharpLib.Log
{
    public abstract class NameBaseAttribute : Attribute
    {
        #region Свойства

        public string Name { get; private set; }

        #endregion

        #region Конструктор

        protected NameBaseAttribute(string name)
        {
            Name = name;
        }

        #endregion
    }
}
