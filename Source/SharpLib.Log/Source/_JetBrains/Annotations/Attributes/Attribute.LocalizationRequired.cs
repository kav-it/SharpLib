using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    internal sealed class LocalizationRequiredAttribute : Attribute
    {
        #region ��������

        public bool Required { get; private set; }

        #endregion

        #region �����������

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