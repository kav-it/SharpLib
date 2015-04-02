using System.Collections.Generic;

namespace SharpLib.Texter.Document
{
    public interface ITextSourceVersion
    {
        #region Ìועמהû

        bool BelongsToSameDocumentAs(ITextSourceVersion other);

        int CompareAge(ITextSourceVersion other);

        IEnumerable<TextChangeEventArgs> GetChangesTo(ITextSourceVersion other);

        int MoveOffsetTo(ITextSourceVersion other, int oldOffset, AnchorMovementType movement = AnchorMovementType.Default);

        #endregion
    }
}