using System.Collections.Generic;

namespace SharpLib.Json
{
    internal class DefaultContractResolverState
    {
        #region ����

        public Dictionary<ResolverContractKey, JsonContract> ContractCache;

        public PropertyNameTable NameTable = new PropertyNameTable();

        #endregion
    }
}