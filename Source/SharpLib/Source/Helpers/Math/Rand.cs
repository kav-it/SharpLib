using System;

namespace SharpLib
{
    /// <summary>
    /// Реализация генерации случайных чисел
    /// </summary>
    public class Rand
    {
        #region Поля

        public static readonly Random Value;

        private static readonly Random _rand;

        #endregion

        #region Конструктор

        static Rand()
        {
            Value = new Random();
            _rand = new Random(DateTime.Now.Ticks.GetHashCode());
        }

        #endregion

        #region Методы

        public static UInt32 Get(UInt32 min, UInt32 max)
        {
            int value = _rand.Next((int)min, (int)max);

            return (UInt32)value;
        }

        public static UInt32 Get(UInt32 max)
        {
            return Get(0, max);
        }

        public static UInt32 Get()
        {
            return Get(0, int.MaxValue);
        }

        public static int GetInt()
        {
            return (int)Get(0, int.MaxValue);
        }

        public static int GetInt(int max)
        {
            return (int)Get(0, (UInt32)max);
        }

        public static Byte[] GetBuffer(int size)
        {
            Byte[] buffer = new Byte[size];

            _rand.NextBytes(buffer);

            return buffer;
        }

        #endregion
    }
}