namespace ICSharpCode.AvalonEdit.Highlighting
{
    public interface IHighlightingDefinitionReferenceResolver
    {
        #region Методы

        IHighlightingDefinition GetDefinition(string name);

        #endregion
    }
}