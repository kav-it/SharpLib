using System;

namespace SharpLib.Json
{
    public class JsonPrimitiveContract : JsonContract
    {
        #region Свойства

        internal PrimitiveTypeCode TypeCode { get; set; }

        #endregion

        #region Конструктор

        public JsonPrimitiveContract(Type underlyingType)
            : base(underlyingType)
        {
            ContractType = JsonContractType.Primitive;

            TypeCode = ConvertUtils.GetTypeCode(underlyingType);
            IsReadOnlyOrFixedSize = true;
        }

        #endregion
    }
}