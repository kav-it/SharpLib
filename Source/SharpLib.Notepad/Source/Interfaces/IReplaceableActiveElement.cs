using System;

namespace SharpLib.Notepad.Snippets
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