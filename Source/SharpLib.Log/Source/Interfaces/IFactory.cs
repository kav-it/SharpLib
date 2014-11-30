using System;

namespace SharpLib.Log
{
    internal interface IFactory
    {
        #region Методы

        void Clear();

        void ScanTypes(Type[] type, string prefix);

        void RegisterType(Type type, string itemNamePrefix);

        #endregion
    }
}