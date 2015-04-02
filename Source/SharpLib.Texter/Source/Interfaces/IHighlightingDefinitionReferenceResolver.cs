namespace SharpLib.Texter.Highlighting
{
    public interface IHighlightingDefinitionReferenceResolver
    {
        #region Методы

        IHighlightingDefinition GetDefinitionByName(string name);

        #endregion
    }
}