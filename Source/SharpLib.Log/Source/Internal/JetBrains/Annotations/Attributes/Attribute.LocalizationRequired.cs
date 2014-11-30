using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    internal sealed class LocalizationRequiredAttribute : Attribute
    {
        #region Свойства

        public bool Required { get; private set; }

        #endregion

        #region Конструктор

        public LocalizationRequiredAttribute()
            : this(true)
        {
        }

        public LocalizationRequiredAttribute(bool required)
        {
            Required = required;
        }

        #endregion
    }
}