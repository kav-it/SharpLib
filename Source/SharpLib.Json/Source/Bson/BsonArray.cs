using System.Collections;
using System.Collections.Generic;

namespace SharpLib.Json
{
    internal class BsonArray : BsonToken, IEnumerable<BsonToken>
    {
        #region Поля

        private readonly List<BsonToken> _children = new List<BsonToken>();

        #endregion

        #region Свойства

        public override BsonType Type
        {
            get { return BsonType.Array; }
        }

        #endregion

        #region Методы

        public void Add(BsonToken token)
        {
            _children.Add(token);
            token.Parent = this;
        }

        public IEnumerator<BsonToken> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}