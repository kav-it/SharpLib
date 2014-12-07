namespace SharpLib.Json
{
    internal class BsonValue : BsonToken
    {
        #region ����

        private readonly BsonType _type;

        private readonly object _value;

        #endregion

        #region ��������

        public object Value
        {
            get { return _value; }
        }

        public override BsonType Type
        {
            get { return _type; }
        }

        #endregion

        #region �����������

        public BsonValue(object value, BsonType type)
        {
            _value = value;
            _type = type;
        }

        #endregion
    }
}