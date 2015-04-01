using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Rendering
{
    public interface IVisualLineTransformer
    {
        #region Методы

        void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements);

        #endregion
    }
}