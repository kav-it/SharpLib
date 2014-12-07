using System;

namespace SharpLib.Json
{
    public interface IContractResolver
    {
        #region Методы

        JsonContract ResolveContract(Type type);

        #endregion
    }
}