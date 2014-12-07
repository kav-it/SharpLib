using System.Collections;

namespace SharpLib.Json
{
    internal interface IWrappedCollection : IList
    {
        #region Свойства

        object UnderlyingCollection { get; }

        #endregion
    }
}