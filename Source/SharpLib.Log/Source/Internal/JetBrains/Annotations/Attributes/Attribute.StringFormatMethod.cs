using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Method,
        AllowMultiple = false, Inherited = true)]
    internal sealed class StringFormatMethodAttribute : Attribute
    {
        #region ��������

        public string FormatParameterName { get; private set; }

        #endregion

        #region �����������

        public StringFormatMethodAttribute(string formatParameterName)
        {
            FormatParameterName = formatParameterName;
        }

        #endregion
    }
}