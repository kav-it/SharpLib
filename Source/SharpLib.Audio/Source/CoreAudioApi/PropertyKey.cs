using System;

namespace NAudio.CoreAudioApi
{
    internal struct PropertyKey
    {
        #region Поля

        public Guid formatId;

        public int propertyId;

        #endregion

        #region Конструктор

        public PropertyKey(Guid formatId, int propertyId)
        {
            this.formatId = formatId;
            this.propertyId = propertyId;
        }

        #endregion
    }
}