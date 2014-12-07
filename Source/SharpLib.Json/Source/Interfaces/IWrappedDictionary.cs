using System.Collections;

namespace SharpLib.Json
{
    internal interface IWrappedDictionary : IDictionary
    {
        #region Свойства

        object UnderlyingDictionary { get; }

        #endregion
    }
}