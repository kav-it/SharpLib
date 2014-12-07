namespace SharpLib.Json
{
    internal class BsonString : BsonValue
    {
        #region Свойства

        public int ByteCount { get; set; }

        public bool IncludeLength { get; set; }

        #endregion

        #region Конструктор

        public BsonString(object value, bool includeLength)
            : base(value, BsonType.String)
        {
            IncludeLength = includeLength;
        }

        #endregion
    }
}