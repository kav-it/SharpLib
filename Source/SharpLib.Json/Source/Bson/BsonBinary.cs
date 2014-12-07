namespace SharpLib.Json
{
    internal class BsonBinary : BsonValue
    {
        #region Свойства

        public BsonBinaryType BinaryType { get; set; }

        #endregion

        #region Конструктор

        public BsonBinary(byte[] value, BsonBinaryType binaryType)
            : base(value, BsonType.Binary)
        {
            BinaryType = binaryType;
        }

        #endregion
    }
}