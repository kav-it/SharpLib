using System;

namespace SharpLib.Json
{
    public class JsonLinqContract : JsonContract
    {
        #region Конструктор

        public JsonLinqContract(Type underlyingType)
            : base(underlyingType)
        {
            ContractType = JsonContractType.Linq;
        }

        #endregion
    }
}