using System;

namespace SharpLib.Json
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class JsonConverterAttribute : Attribute
    {
        #region Поля

        private readonly Type _converterType;

        #endregion

        #region Свойства

        public Type ConverterType
        {
            get { return _converterType; }
        }

        public object[] ConverterParameters { get; private set; }

        #endregion

        #region Конструктор

        public JsonConverterAttribute(Type converterType)
        {
            if (converterType == null)
            {
                throw new ArgumentNullException("converterType");
            }

            _converterType = converterType;
        }

        public JsonConverterAttribute(Type converterType, params object[] converterParameters)
            : this(converterType)
        {
            ConverterParameters = converterParameters;
        }

        #endregion
    }
}