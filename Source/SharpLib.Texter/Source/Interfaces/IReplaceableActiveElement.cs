using System;

namespace SharpLib.Texter.Snippets
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