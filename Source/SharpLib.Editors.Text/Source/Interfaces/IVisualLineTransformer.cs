using System.Collections.Generic;

namespace SharpLib.Notepad.Rendering
{
    public interface IVisualLineTransformer
    {
        #region Методы

        void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements);

        #endregion
    }
}