using System;

namespace SharpLib.Json
{
    public class JsonStringContract : JsonPrimitiveContract
    {
        #region Конструктор

        public JsonStringContract(Type underlyingType)
            : base(underlyingType)
        {
            ContractType = JsonContractType.String;
        }

        #endregion
    }
}