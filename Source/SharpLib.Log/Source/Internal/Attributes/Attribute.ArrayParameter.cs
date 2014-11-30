
using System;

namespace SharpLib.Log
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ArrayParameterAttribute : Attribute
    {
        #region Свойства

        public Type ItemType { get; private set; }

        public string ElementName { get; private set; }

        #endregion

        #region Конструктор

        public ArrayParameterAttribute(Type itemType, string elementName)
        {
            ItemType = itemType;
            ElementName = elementName;
        }

        #endregion
    }
}
