using System;
using System.Reflection;

namespace SharpLib.Log
{
    internal class FactoryHelper
    {
        #region Поля

        private static readonly object[] _emptyParams = new object[0];

        private static readonly Type[] _emptyTypes = new Type[0];

        #endregion

        #region Конструктор

        private FactoryHelper()
        {
        }

        #endregion

        #region Методы

        internal static object CreateInstance(Type t)
        {
            ConstructorInfo constructor = t.GetConstructor(_emptyTypes);
            if (constructor != null)
            {
                return constructor.Invoke(_emptyParams);
            }

            return null;
        }

        #endregion
    }
}