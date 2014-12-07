namespace SharpLib.Json
{
    internal abstract class BsonToken
    {
        #region Свойства

        public abstract BsonType Type { get; }

        public BsonToken Parent { get; set; }

        public int CalculatedSize { get; set; }

        #endregion
    }
}