namespace ICSharpCode.AvalonEdit.Editing
{
    public interface ITextAreaInputHandler
    {
        #region ��������

        TextArea TextArea { get; }

        #endregion

        #region ������

        void Attach();

        void Detach();

        #endregion
    }
}