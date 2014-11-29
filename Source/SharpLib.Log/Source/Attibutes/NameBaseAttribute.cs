using System;

namespace NLog.Config
{
    public abstract class NameBaseAttribute : Attribute
    {
        #region ��������

        public string Name { get; private set; }

        #endregion

        #region �����������

        protected NameBaseAttribute(string name)
        {
            Name = name;
        }

        #endregion
    }
}