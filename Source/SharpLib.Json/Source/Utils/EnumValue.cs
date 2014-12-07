namespace SharpLib.Json
{
    internal class EnumValue<T> where T : struct
    {
        #region Поля

        private readonly string _name;

        private readonly T _value;

        #endregion

        #region Свойства

        public string Name
        {
            get { return _name; }
        }

        public T Value
        {
            get { return _value; }
        }

        #endregion

        #region Конструктор

        public EnumValue(string name, T value)
        {
            _name = name;
            _value = value;
        }

        #endregion
    }
}