using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Method,
        AllowMultiple = false, Inherited = true)]
    internal sealed class StringFormatMethodAttribute : Attribute
    {
        #region Свойства

        public string FormatParameterName { get; private set; }

        #endregion

        #region Конструктор

        public StringFormatMethodAttribute(string formatParameterName)
        {
            FormatParameterName = formatParameterName;
        }

        #endregion
    }
}