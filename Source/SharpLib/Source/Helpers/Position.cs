namespace SharpLib
{
    public struct Position
    {
        #region Поля

        private static readonly Position _zero = CreateEmptySize();

        #endregion

        #region Свойства

        public int X { get; set; }

        public int Y { get; set; }

        public static Position Zero
        {
            get { return _zero; }
        }

        public bool IsZero
        {
            get { return X == 0 && Y == 0; }
        }

        #endregion

        #region Конструктор

        static Position()
        {
        }

        public Position(int x, int y)
            : this()
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Методы

        private static Position CreateEmptySize()
        {
            return new Position
            {
                X = 0,
                Y = 0
            };
        }

        public static bool Equals(Position value1, Position value2)
        {
            if (value1.IsZero)
            {
                return value2.IsZero;
            }
            if (value1.X.Equals(value2.X))
            {
                return value1.Y.Equals(value2.Y);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (!(o is Position))
            {
                return false;
            }
            return Equals(this, (Position)o);
        }

        public bool Equals(Position value)
        {
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            if (IsZero)
            {
                return 0;
            }
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        #endregion

        public static bool operator ==(Position value1, Position value2)
        {
            if (value1.X == value2.X)
            {
                return value1.Y == value2.Y;
            }

            return false;
        }

        public static bool operator !=(Position value1, Position value2)
        {
            return !(value1 == value2);
        }
    }
}