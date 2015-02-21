using System;
using System.Diagnostics;
using System.Threading;

namespace Standard
{
    internal static class Assert
    {
        #region Делегаты

        public delegate void EvaluateFunction();

        public delegate bool ImplicationFunction();

        #endregion

        #region Методы

        private static void _Break()
        {
            Debug.Assert(false);
        }

        [Conditional("DEBUG")]
        public static void Evaluate(EvaluateFunction argument)
        {
            IsNotNull(argument);
            argument();
        }

        [
            Obsolete("Use Assert.AreEqual instead of Assert.Equals", false),
            Conditional("DEBUG")
        ]
        public static void Equals<T>(T expected, T actual)
        {
            AreEqual(expected, actual);
        }

        [Conditional("DEBUG")]
        public static void AreEqual<T>(T expected, T actual)
        {
            if (null == expected)
            {
                if (null != actual && !actual.Equals(expected))
                {
                    _Break();
                }
            }
            else if (!expected.Equals(actual))
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void AreNotEqual<T>(T notExpected, T actual)
        {
            if (null == notExpected)
            {
                if (null == actual || actual.Equals(notExpected))
                {
                    _Break();
                }
            }
            else if (notExpected.Equals(actual))
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void Implies(bool condition, bool result)
        {
            if (condition && !result)
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void Implies(bool condition, ImplicationFunction result)
        {
            if (condition && !result())
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void IsNeitherNullNorEmpty(string value)
        {
            IsFalse(string.IsNullOrEmpty(value));
        }

        [Conditional("DEBUG")]
        public static void IsNeitherNullNorWhitespace(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                _Break();
            }

            if (value.Trim().Length == 0)
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void IsNotNull<T>(T value) where T : class
        {
            if (null == value)
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void IsDefault<T>(T value) where T : struct
        {
            if (!value.Equals(default(T)))
            {
                Assert.Fail();
            }
        }

        [Conditional("DEBUG")]
        public static void IsNotDefault<T>(T value) where T : struct
        {
            if (value.Equals(default(T)))
            {
                Assert.Fail();
            }
        }

        [Conditional("DEBUG")]
        public static void IsFalse(bool condition)
        {
            if (condition)
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void IsFalse(bool condition, string message)
        {
            if (condition)
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void IsTrue(bool condition)
        {
            if (!condition)
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void IsTrue(bool condition, string message)
        {
            if (!condition)
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void Fail()
        {
            _Break();
        }

        [Conditional("DEBUG")]
        public static void Fail(string message)
        {
            _Break();
        }

        [Conditional("DEBUG")]
        public static void IsNull<T>(T item) where T : class
        {
            if (null != item)
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void BoundedDoubleInc(double lowerBoundInclusive, double value, double upperBoundInclusive)
        {
            if (value < lowerBoundInclusive || value > upperBoundInclusive)
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void BoundedInteger(int lowerBoundInclusive, int value, int upperBoundExclusive)
        {
            if (value < lowerBoundInclusive || value >= upperBoundExclusive)
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void IsApartmentState(ApartmentState expectedState)
        {
            if (Thread.CurrentThread.GetApartmentState() != expectedState)
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void NullableIsNotNull<T>(T? value) where T : struct
        {
            if (null == value)
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void NullableIsNull<T>(T? value) where T : struct
        {
            if (null != value)
            {
                _Break();
            }
        }

        [Conditional("DEBUG")]
        public static void IsNotOnMainThread()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                _Break();
            }
        }

        #endregion
    }
}