namespace SharpLib.Notepad.Highlighting
{
    public interface IHighlightingDefinitionReferenceResolver
    {
        #region Методы

        IHighlightingDefinition GetDefinitionByName(string name);

        #endregion
    }
}