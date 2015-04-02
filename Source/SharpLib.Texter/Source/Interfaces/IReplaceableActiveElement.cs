using System;

namespace SharpLib.Texter.Snippets
{
    public interface IReplaceableActiveElement : IActiveElement
    {
        #region Свойства

        string Text { get; }

        #endregion

        #region События

        event EventHandler TextChanged;

        #endregion
    }
}