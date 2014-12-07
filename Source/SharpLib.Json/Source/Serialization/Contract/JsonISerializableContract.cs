using System;

namespace SharpLib.Json
{
    public class JsonISerializableContract : JsonContainerContract
    {
        #region Свойства

        public ObjectConstructor<object> ISerializableCreator { get; set; }

        #endregion

        #region Конструктор

        public JsonISerializableContract(Type underlyingType)
            : base(underlyingType)
        {
            ContractType = JsonContractType.Serializable;
        }

        #endregion
    }
}