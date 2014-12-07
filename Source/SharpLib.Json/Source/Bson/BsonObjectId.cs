
using System;

namespace SharpLib.Json
{
    public class BsonObjectId
    {
        #region Свойства

        public byte[] Value { get; private set; }

        #endregion

        #region Конструктор

        public BsonObjectId(byte[] value)
        {
            ValidationUtils.ArgumentNotNull(value, "value");
            if (value.Length != 12)
            {
                throw new ArgumentException("An ObjectId must be 12 bytes", "value");
            }

            Value = value;
        }

        #endregion
    }
}