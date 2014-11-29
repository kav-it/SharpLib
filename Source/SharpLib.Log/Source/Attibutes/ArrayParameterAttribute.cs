using System;

namespace NLog.Config
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ArrayParameterAttribute : Attribute
    {
        #region ��������

        public Type ItemType { get; private set; }

        public string ElementName { get; private set; }

        #endregion

        #region �����������

        public ArrayParameterAttribute(Type itemType, string elementName)
        {
            ItemType = itemType;
            ElementName = elementName;
        }

        #endregion
    }
}