namespace SharpLib.Texter.Indentation.CSharp
{
    public interface IDocumentAccessor
    {
        #region Свойства

        bool IsReadOnly { get; }

        int LineNumber { get; }

        string Text { get; set; }

        #endregion

        #region Методы

        bool MoveNext();

        #endregion
    }
}