using System.Collections;
using System.Collections.Generic;

namespace SharpLib.Json
{
    internal class BsonArray : BsonToken, IEnumerable<BsonToken>
    {
        #region ����

        private readonly List<BsonToken> _children = new List<BsonToken>();

        #endregion

        #region ��������

        public override BsonType Type
        {
            get { return BsonType.Array; }
        }

        #endregion

        #region ������

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