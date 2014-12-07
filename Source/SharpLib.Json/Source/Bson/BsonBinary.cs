namespace SharpLib.Json
{
    internal class BsonBinary : BsonValue
    {
        #region ��������

        public BsonBinaryType BinaryType { get; set; }

        #endregion

        #region �����������

        public BsonBinary(byte[] value, BsonBinaryType binaryType)
            : base(value, BsonType.Binary)
        {
            BinaryType = binaryType;
        }

        #endregion
    }
}