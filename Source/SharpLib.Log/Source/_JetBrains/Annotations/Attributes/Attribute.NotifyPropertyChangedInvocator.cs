using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    internal sealed class NotifyPropertyChangedInvocatorAttribute : Attribute
    {
        #region Свойства

        public string ParameterName { get; private set; }

        #endregion

        #region Конструктор

        public NotifyPropertyChangedInvocatorAttribute()
        {
        }

        public NotifyPropertyChangedInvocatorAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        #endregion
    }
}