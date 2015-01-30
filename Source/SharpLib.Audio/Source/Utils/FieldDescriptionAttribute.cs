using System;

namespace NAudio.Utils
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class FieldDescriptionAttribute : Attribute
    {
        #region Свойства

        public string Description { get; private set; }

        #endregion

        #region Конструктор

        public FieldDescriptionAttribute(string description)
        {
            Description = description;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return Description;
        }

        #endregion
    }
}