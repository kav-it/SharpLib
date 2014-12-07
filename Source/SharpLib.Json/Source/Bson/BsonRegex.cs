namespace SharpLib.Json
{
    internal class BsonRegex : BsonToken
    {
        #region Свойства

        public BsonString Pattern { get; set; }

        public BsonString Options { get; set; }

        public override BsonType Type
        {
            get { return BsonType.Regex; }
        }

        #endregion

        #region Конструктор

        public BsonRegex(string pattern, string options)
        {
            Pattern = new BsonString(pattern, false);
            Options = new BsonString(options, false);
        }

        #endregion
    }
}