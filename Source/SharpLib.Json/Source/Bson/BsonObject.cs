using System.Collections;
using System.Collections.Generic;

namespace SharpLib.Json
{
    internal class BsonObject : BsonToken, IEnumerable<BsonProperty>
    {
        #region Поля

        private readonly List<BsonProperty> _children = new List<BsonProperty>();

        #endregion

        #region Свойства

        public override BsonType Type
        {
            get { return BsonType.Object; }
        }

        #endregion

        #region Методы

        public void Add(string name, BsonToken token)
        {
            _children.Add(new BsonProperty
            {
                Name = new BsonString(name, false),
                Value = token
            });
            token.Parent = this;
        }

        public IEnumerator<BsonProperty> GetEnumerator()
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