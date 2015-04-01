using System;

namespace ICSharpCode.AvalonEdit.Snippets
{
    public interface IReplaceableActiveElement : IActiveElement
    {
        #region ��������

        string Text { get; }

        #endregion

        #region �������

        event EventHandler TextChanged;

        #endregion
    }
}