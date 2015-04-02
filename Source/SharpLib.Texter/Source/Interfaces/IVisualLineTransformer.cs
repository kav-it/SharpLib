using System.Collections.Generic;

namespace SharpLib.Texter.Rendering
{
    public interface IVisualLineTransformer
    {
        #region Методы

        void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements);

        #endregion
    }
}