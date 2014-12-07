namespace SharpLib.Json
{
    internal abstract class BsonToken
    {
        #region ��������

        public abstract BsonType Type { get; }

        public BsonToken Parent { get; set; }

        public int CalculatedSize { get; set; }

        #endregion
    }
}