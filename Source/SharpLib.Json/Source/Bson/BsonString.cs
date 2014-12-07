namespace SharpLib.Json
{
    internal class BsonString : BsonValue
    {
        #region ��������

        public int ByteCount { get; set; }

        public bool IncludeLength { get; set; }

        #endregion

        #region �����������

        public BsonString(object value, bool includeLength)
            : base(value, BsonType.String)
        {
            IncludeLength = includeLength;
        }

        #endregion
    }
}