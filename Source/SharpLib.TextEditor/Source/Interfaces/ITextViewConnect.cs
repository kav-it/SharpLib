namespace ICSharpCode.AvalonEdit.Rendering
{
    public interface ITextViewConnect
    {
        #region Методы

        void AddToTextView(TextView textView);

        void RemoveFromTextView(TextView textView);

        #endregion
    }
}