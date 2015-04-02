using System;

namespace SharpLib.Notepad.Snippets
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