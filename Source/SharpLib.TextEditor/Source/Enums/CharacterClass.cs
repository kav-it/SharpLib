namespace ICSharpCode.AvalonEdit.Document
{
    public enum CharacterClass
    {
        Other,

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace",
            Justification = "WPF uses 'Whitespace'")]
        Whitespace,

        IdentifierPart,

        LineTerminator,

        CombiningMark
    }
}