using System;

namespace ICSharpCode.AvalonEdit.Snippets
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