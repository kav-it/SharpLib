namespace ICSharpCode.AvalonEdit.Editing
{
    public interface ITextAreaInputHandler
    {
        #region Ñגמיסעגא

        TextArea TextArea { get; }

        #endregion

        #region Ìועמהû

        void Attach();

        void Detach();

        #endregion
    }
}