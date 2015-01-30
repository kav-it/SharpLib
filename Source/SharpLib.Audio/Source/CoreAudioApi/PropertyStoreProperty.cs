using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
    internal class PropertyStoreProperty
    {
        #region Поля

        private readonly PropertyKey propertyKey;

        private PropVariant propertyValue;

        #endregion

        #region Свойства

        public PropertyKey Key
        {
            get { return propertyKey; }
        }

        public object Value
        {
            get { return propertyValue.Value; }
        }

        #endregion

        #region Конструктор

        internal PropertyStoreProperty(PropertyKey key, PropVariant value)
        {
            propertyKey = key;
            propertyValue = value;
        }

        #endregion
    }
}